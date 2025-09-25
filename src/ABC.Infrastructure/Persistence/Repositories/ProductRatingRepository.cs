using ABC.Application.ProductRatings.Common;
using ABC.Domain.Entities;
using ABC.Infrastructure.Persistence.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace ABC.Infrastructure.Persistence.Repositories;

public class ProductRatingRepository(ApplicationDbContext dbContext) 
    : Repository<ProductRating>(dbContext), IProductRatingRepository
{
    private readonly ApplicationDbContext _dbContext = dbContext;

    public async Task<ProductRating?> GetByProductIdAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.ProductRatings
            .FirstOrDefaultAsync(pr => pr.ProductId == productId, cancellationToken);
    }

    public void Update(ProductRating productRating)
    {
        _dbContext.ProductRatings.Update(productRating);
    }

    public async Task<int> MarkOutdatedForTermAsync(string termText, CancellationToken cancellationToken)
    {
        return await _dbContext.ProductRatings
            .Where(pr => _dbContext.Feedbacks
                .Any(f => f.ProductId == pr.ProductId && 
                          f.Text.ToLower().Contains(termText.ToLower())))
            .ExecuteUpdateAsync(
                setter => setter.SetProperty(pr => pr.IsOutdated, true), 
                cancellationToken);
    }

    public async Task<ICollection<ProductRating>> GetOutdatedAsync(int amount, CancellationToken cancellationToken = default)
    {
        return await _dbContext.ProductRatings
            .Where(pr => pr.IsOutdated)
            .Take(amount)
            .ToListAsync(cancellationToken);
    }
}