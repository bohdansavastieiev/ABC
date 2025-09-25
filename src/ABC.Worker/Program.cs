using ABC.Application;
using ABC.Application.ProductRatings.Commands.RequestRecalculation;
using ABC.Infrastructure;
using ABC.Worker.Consumers;
using ABC.Worker.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

var host = Host.CreateDefaultBuilder(args)
    .UseSerilog((hostContext, loggerConfiguration) => 
    {
        loggerConfiguration
            .ReadFrom.Configuration(hostContext.Configuration)
            .Enrich.FromLogContext();
    })
    .ConfigureServices((hostContext, services) =>
    {
        var configuration = hostContext.Configuration;
        services
            .AddApplication(options =>
            {
                options.WithRatingService();
            })
            .AddInfrastructure(
                configuration,
                options =>
                {
                    options.WithCaching();
                    options.WithMessageBrokerConsumer(
                        typeof(FeedbackSubmittedEventConsumer),
                        typeof(SentimentTermCreatedEventConsumer),
                        typeof(RecalculationRequestedEventConsumer));
                })
            .AddHostedService<ScheduledRatingRecalculationService>();
    })
    .Build();

await host.RunAsync();