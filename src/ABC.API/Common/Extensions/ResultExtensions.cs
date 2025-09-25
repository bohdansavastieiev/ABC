using ABC.API.Common.Factories;
using ABC.Application.Common.Errors;
using ABC.Application.Common.Errors.Abstractions;
using ABC.Application.Common.Errors.Common;
using ABC.Application.Common.Exceptions;
using FluentResults;

namespace ABC.API.Common.Extensions;

public static class ResultExtensions
{
    public static IResult HandleFailure<T>(this Result<T> result, HttpContext context)
    {
        return HandleFailureBase(result, context);
    }

    public static IResult HandleFailure(this Result result, HttpContext context)
    {
        return HandleFailureBase(result, context);
    }

    private static IResult HandleFailureBase(IResultBase result, HttpContext context)
    {
        if (result.IsSuccess)
        {
            throw new ResultContractViolationException(
                ContractViolationType.HandleFailureOnSuccess,
                "HandleFailure called on successful result. Check IsSuccess before calling HandleFailure.");
        }
        
        if (result.Errors.Count == 0)
        {
            throw new ResultContractViolationException(
                ContractViolationType.NoErrorsInFailedResult,
                "Failed result has no errors. This indicates a bug in the handler.");
        }

        var error = result.Errors[0];
        if (error is not ApplicationError applicationError)
        {
            throw new ResultContractViolationException(
                ContractViolationType.NonApplicationError,
                $"Failed result contains non-application error: {error.GetType().Name}. " 
                + "All domain/application errors should inherit from ApplicationError.");
        }
        
        return applicationError switch
        {
            ValidationError validationError => ProblemDetailsFactory.CreateValidation(validationError, context),
            NotFoundError notFoundError => ProblemDetailsFactory.CreateNotFound(notFoundError, context),
            ConflictError conflictError => conflictError switch
            {
                PropertyNotUniqueError propertyNotUniqueError => 
                    ProblemDetailsFactory.CreatePropertyNotUnique(propertyNotUniqueError, context),
                _ => throw new ResultContractViolationException(
                    ContractViolationType.UnexpectedErrorType,
                    $"Unexpected Conflict error type: {conflictError.GetType().Name}")
            },
            _ => ProblemDetailsFactory.CreateInternalServer(applicationError, context)
        };
    }
}