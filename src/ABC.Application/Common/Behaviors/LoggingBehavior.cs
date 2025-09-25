
using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ABC.Application.Common.Behaviors;

public class LoggingBehavior<TRequest, TResponse>(
    ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(
        TRequest request, 
        RequestHandlerDelegate<TResponse> next, 
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        
        logger.LogInformation("Handling {RequestName}...", requestName);

        var stopwatch = Stopwatch.StartNew();
        
        var response = await next(cancellationToken);
        
        stopwatch.Stop();

        logger.LogInformation(
            "{RequestName} handled in {ElapsedMilliseconds} ms.", 
            requestName, 
            stopwatch.ElapsedMilliseconds);
        
        return response;
    }
}