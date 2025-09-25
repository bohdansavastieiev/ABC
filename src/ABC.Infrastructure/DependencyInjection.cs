using ABC.Application.Abstractions;
using ABC.Application.Abstractions.Persistence;
using ABC.Application.Feedbacks.Common;
using ABC.Application.ProductRatings.Common;
using ABC.Application.Products.Common;
using ABC.Application.Security;
using ABC.Application.SentimentTerms.Common;
using ABC.Infrastructure.Cache;
using ABC.Infrastructure.MessageBroker;
using ABC.Infrastructure.Persistence;
using ABC.Infrastructure.Persistence.Repositories;
using ABC.Infrastructure.Persistence.Repositories.Base;
using ABC.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ABC.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<InfrastructureOptionsBuilder> configureOptions)
    {
        var options = new InfrastructureOptionsBuilder();
        configureOptions(options);

        var connectionString = configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrWhiteSpace(connectionString)) 
            throw new InvalidOperationException("Invalid connection string");
        
        services.AddDbContext<ApplicationDbContext>(opt =>
            opt.UseNpgsql(connectionString).UseSnakeCaseNamingConvention());
        
        services.AddScoped<IFeedbackRepository, FeedbackRepository>();
        services.AddScoped<ISentimentTermRepository, SentimentTermRepository>();
        services.AddScoped<IProductRatingRepository, ProductRatingRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        
        if (options.IsApiKeyValidationEnabled)
        {
            services.AddOptions<ApiKeyOptions>()
                .Bind(configuration.GetSection(ApiKeyOptions.SectionName))
                .ValidateOnStart();
            services.AddSingleton<IApiKeyValidator, ApiKeyValidator>();
        }

        if (options.IsCachingEnabled)
        {
            services.AddOptions<CacheOptions>()
                .Bind(configuration.GetSection(CacheOptions.SectionName))
                .ValidateOnStart();
            services.AddSingleton<ISentimentTermCache, SentimentTermCache>();
            services.AddMemoryCache();
        }

        if (options.IsPublisherEnabled)
        {
            services.AddScoped<IEventPublisher, EventPublisher>();
        }
        
        var isConsumer = options.ConsumerTypes.Any();
        services.AddMessageBroker(
            configuration,
            isConsumer: isConsumer,
            consumerTypes: options.ConsumerTypes.ToArray());
        
        services.AddOptions<RatingRecalculationOptions>()
            .Bind(configuration.GetSection(RatingRecalculationOptions.SectionName))
            .ValidateOnStart();
        
        return services;
    }
}