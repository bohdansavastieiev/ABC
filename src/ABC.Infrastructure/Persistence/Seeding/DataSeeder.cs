using ABC.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ABC.Infrastructure.Persistence.Seeding;

public class DataSeeder(ApplicationDbContext context, ILogger<DataSeeder> logger) : IDataSeeder
{
    public async Task SeedAsync()
    {
        if (await context.SentimentTerms.AnyAsync())
        {
            logger.LogInformation("Database already seeded, skipping...");
            return;
        }

        await using var transaction = await context.Database.BeginTransactionAsync();
        
        try
        {
            var sentimentTerms = new[]
            {
                new SentimentTerm("excellent", 5),
                new SentimentTerm("good", 4),
                new SentimentTerm("average", 3),
                new SentimentTerm("bad", 2),
                new SentimentTerm("very bad", 1)
            };

            var products = new[]
            {
                new Product("EasyWP"),
                new Product("Private Email"),
                new Product("PositiveSSL")
            };
            
            var feedbacks = new[]
            {
                new Feedback(products[0].Id, "UserA", "The setup was excellent, very straightforward."),
                new Feedback(products[0].Id, "UserB", "Performance is just average, nothing special."),
                new Feedback(products[0].Id, "UserC", "Customer support was surprisingly good."),

                new Feedback(products[1].Id, "UserD", "A very good service overall, highly recommend."),
                new Feedback(products[1].Id, "UserE", "The spam filter is average at best."),
                new Feedback(products[1].Id, "UserF", "The migration process was bad, very frustrating."),
                
                new Feedback(products[2].Id, "UserG", "Installation was a very bad experience, docs are unclear."),
                new Feedback(products[2].Id, "UserH", "The certificate works, so that's good."),
                new Feedback(products[2].Id, "UserI", "Validation took longer than expected, which was very bad.")
            };
            
            var productRatings = new[]
            {
                new ProductRating(products[0].Id, 4, true),
                new ProductRating(products[1].Id, 3, true),
                new ProductRating(products[2].Id, 2, true)
            };
            
            await context.SentimentTerms.AddRangeAsync(sentimentTerms);
            await context.Products.AddRangeAsync(products);
            await context.Feedbacks.AddRangeAsync(feedbacks);
            await context.ProductRatings.AddRangeAsync(productRatings);
            await context.SaveChangesAsync();
            
            logger.LogInformation("Database is seeded");
            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}