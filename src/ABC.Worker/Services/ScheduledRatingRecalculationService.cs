using ABC.Application.ProductRatings.Commands.ProcessOutdatedRatings;
using ABC.Application.ProductRatings.Common;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ABC.Worker.Services;

public class ScheduledRatingRecalculationService(
    IServiceProvider serviceProvider,
    IOptions<RatingRecalculationOptions> options,
    ILogger<ScheduledRatingRecalculationService> logger) 
    : BackgroundService
{
    private readonly RatingRecalculationOptions _options = options.Value;
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var nextRun = GetNextRunTime();
            var delay = nextRun - DateTime.UtcNow;
        
            if (delay > TimeSpan.Zero)
            {
                logger.LogInformation("Next recalculation scheduled at {NextRun:yyyy-MM-dd HH:mm:ss} UTC", nextRun);
                await Task.Delay(delay, stoppingToken);
            }
        
            if (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            
            try
            {
                logger.LogInformation("Starting scheduled rating recalculation");
                
                using var scope = serviceProvider.CreateScope();
                var mediator = scope.ServiceProvider.GetRequiredService<ISender>();
                
                var command = new ProcessOutdatedRatingsCommand(_options.BatchSize);
                var result = await mediator.Send(command, stoppingToken);
                
                if (result.IsSuccess)
                {
                    logger.LogInformation(
                        "Scheduled recalculation completed. Processed {Count} ratings", 
                        result.Value);
                }
                else
                {
                    logger.LogError(
                        "Scheduled recalculation failed: {Errors}", 
                        string.Join(", ", result.Errors));
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in scheduled rating recalculation");
            }
        }
    }
    
    private DateTime GetNextRunTime()
    {
        var now = DateTime.UtcNow;
        var interval = TimeSpan.FromDays(_options.ScheduleIntervalDays);
        var scheduledTime = new TimeOnly(_options.ScheduleHourUtc, _options.ScheduleMinuteUtc);
        
        var nextRun = now.Date.Add(interval)
            .AddHours(scheduledTime.Hour)
            .AddMinutes(scheduledTime.Minute);
            
        var todayRun = now.Date
            .AddHours(scheduledTime.Hour)
            .AddMinutes(scheduledTime.Minute);
            
        if (now < todayRun)
            nextRun = todayRun;
            
        return nextRun;
    }
}