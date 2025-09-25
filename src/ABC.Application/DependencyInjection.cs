using System.Reflection;
using ABC.Application.Common.Behaviors;
using ABC.Application.RatingCalculation;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace ABC.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services, 
        Action<ApplicationOptionsBuilder>? configureOptions = null)
    {
        var options = new ApplicationOptionsBuilder();
        configureOptions?.Invoke(options);
        
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(ApplicationAssembly.Reference);
            cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });

        if (options.IsRatingServiceEnabled)
        {
            services.AddScoped<IRatingService, RatingService>();
        }
        else
        {
            services.AddScoped<IRatingService, NullRatingService>();
        }
        
        return services;
    }
}