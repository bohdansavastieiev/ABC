using ABC.API.Common.Factories;
using ABC.Application.Security;

namespace ABC.API.Common.Filters;

public class RequireApiKeyEndpointFilter : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(
        EndpointFilterInvocationContext context, 
        EndpointFilterDelegate next)
    {
        var apiKeyValidator = context.HttpContext.RequestServices
            .GetRequiredService<IApiKeyValidator>();

        if (context.HttpContext.Request.Headers.TryGetValue("X-API-Key", out var apiKey)
            && apiKeyValidator.IsValidAdminKey(apiKey))
        {
            return await next(context);
        }
        
        var logger = context.HttpContext.RequestServices
            .GetRequiredService<ILogger<RequireApiKeyEndpointFilter>>();
            
        logger.LogWarning("Invalid or missing API key from IP: {RemoteIpAddress}",
            context.HttpContext.Connection.RemoteIpAddress);
            
        return ProblemDetailsFactory.CreateUnauthorized(
            "Invalid or missing API key", 
            context.HttpContext);
    }
}