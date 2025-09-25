using System.Diagnostics;
using ABC.API.Common.Constants;
using ABC.Application.Common.Constants;
using ABC.Application.Common.Errors;
using ABC.Application.Common.Errors.Abstractions;
using ABC.Application.Common.Errors.Common;
using ABC.Application.Common.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace ABC.API.Common.Factories;

internal static class ProblemDetailsFactory
{
    public static IResult CreateValidation(ValidationError error, HttpContext context)
    {
        var traceId = Activity.Current?.Id ?? context.TraceIdentifier;
        
        return Results.ValidationProblem(
            error.GetValidationDictionary(),
            instance: context.Request.Path,
            title: "One or more validation errors occurred",
            type: ProblemTypeUrls.BadRequest,
            extensions: new Dictionary<string, object?>
            {
                ["errorCode"] = error.Code,
                ["traceId"] = traceId
            });
    }
    
    public static IResult CreateBadRequest(string message, HttpContext context)
    {
        var traceId = Activity.Current?.Id ?? context.TraceIdentifier;

        return Results.ValidationProblem(
            new Dictionary<string, string[]>
            {
                ["RequestBody"] = [message]
            },
            instance: context.Request.Path,
            title: "Invalid request",
            type: ProblemTypeUrls.BadRequest,
            extensions: new Dictionary<string, object?>
            {
                ["errorCode"] = ErrorCodes.InvalidRequest,
                ["traceId"] = traceId
            });
    }
    
    public static IResult CreateUnauthorized(string message, HttpContext context)
    {
        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status401Unauthorized,
            Type = ProblemTypeUrls.Unauthorized,
            Title = "Unauthorized",
            Detail = message,
            Instance = context.Request.Path
        };
    
        AddCommonExtensions(problemDetails, ErrorCodes.Unauthorized, context);
    
        return Results.Problem(problemDetails);
    }
    
    public static IResult CreateNotFound(NotFoundError error, HttpContext context)
    {
        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status404NotFound,
            Type = ProblemTypeUrls.NotFound,
            Title = error is ReferencedResourceNotFoundError 
                ? "Referenced resource not found"
                : "Resource not found",
            Detail = error.Message,
            Instance = context.Request.Path
        };
        
        AddCommonExtensions(problemDetails, error.Code, context);
        problemDetails.Extensions["resourceType"] = error.ResourceType;
        problemDetails.Extensions["resourceId"] = error.ResourceId;
        
        return Results.Problem(problemDetails);
    }
    
    public static IResult CreatePropertyNotUnique(PropertyNotUniqueError error, HttpContext context)
    {
        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status409Conflict,
            Type = ProblemTypeUrls.Conflict,
            Title = "Property not unique",
            Detail = error.Message,
            Instance = context.Request.Path
        };
        
        AddCommonExtensions(problemDetails, error.Code, context);
        problemDetails.Extensions["resourceType"] = error.ResourceType;
        problemDetails.Extensions["propertyName"] = error.PropertyName;
        problemDetails.Extensions["propertyValue"] = error.PropertyValue;
        
        return Results.Problem(problemDetails);
    }
    
    public static IResult CreateInternalServer(ApplicationError error, HttpContext context)
    {
        var isDevelopment = IsDevelopment(context);
        
        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Type = ProblemTypeUrls.InternalError,
            Title = "An error occurred while processing your request",
            Detail = isDevelopment ? error.Message : null,
            Instance = context.Request.Path
        };
        
        AddCommonExtensions(problemDetails, error.Code, context);
        
        return Results.Problem(problemDetails);
    }
    
    public static ProblemDetails CreateUnexpectedException(Exception exception, HttpContext context)
    {
        var isDevelopment = IsDevelopment(context);
        
        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Type = ProblemTypeUrls.InternalError,
            Title = "An unexpected error occurred",
            Detail = isDevelopment ? exception.Message : null,
            Instance = context.Request.Path
        };
        
        AddCommonExtensions(problemDetails, ErrorCodes.InternalServerError, context);

        if (!isDevelopment) return problemDetails;
        
        problemDetails.Extensions["exception"] = exception.GetType().Name;
        problemDetails.Extensions["stackTrace"] = exception.StackTrace?.Trim();

        if (exception is ResultContractViolationException resultViolationEx)
        {
            problemDetails.Extensions["violationType"] = resultViolationEx.ViolationType;
            problemDetails.Extensions["violationMessage"] = resultViolationEx.Message;
        }
        
        return problemDetails;
    }
    
    private static void AddCommonExtensions(ProblemDetails problemDetails, string errorCode, HttpContext context)
    {
        var traceId = Activity.Current?.Id ?? context.TraceIdentifier;
        problemDetails.Extensions["errorCode"] = errorCode;
        problemDetails.Extensions["traceId"] = traceId;
    }
    
    private static bool IsDevelopment(HttpContext context)
    {
        return context.RequestServices
            .GetRequiredService<IHostEnvironment>()
            .IsDevelopment();
    }
}