using ABC.API.Common.Factories;
using Microsoft.AspNetCore.Diagnostics;

namespace ABC.API.Common.Middleware;

public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger): IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext, 
        Exception exception, 
        CancellationToken cancellationToken)
    {
        logger.LogError(exception, "An unhandled exception occurred: {Message}", exception.Message);
        
        if (exception is BadHttpRequestException badRequestEx)
        {
            var problem = ProblemDetailsFactory.CreateBadRequest(badRequestEx.Message, httpContext);

            httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            httpContext.Response.ContentType = "application/problem+json";
            await problem.ExecuteAsync(httpContext);
            return true;
        }
        
        var internalProblem = ProblemDetailsFactory.CreateUnexpectedException(exception, httpContext);
        
        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
        httpContext.Response.ContentType = "application/problem+json";
        await httpContext.Response.WriteAsJsonAsync(internalProblem, cancellationToken);
        
        return true;
    }
}