using ABC.Application.Products;
using ABC.Application.Products.Common;
using ABC.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ABC.Infrastructure.Persistence.Repositories;

public class ProductRepository(ApplicationDbContext dbContext) : IProductRepository
{
    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await dbContext.Products.AnyAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<ICollection<Product>> GetProductsWithOutdatedRatingAndFeedbacksAsync(
        int amount,
        CancellationToken cancellationToken = default)
    {
        return await dbContext.Products
            .Include(p => p.Rating)
            .Include(p => p.Feedbacks)
            .Where(p => p.Rating.IsOutdated)
            .OrderByDescending(p => p.Id)
            .Take(amount)
            .ToListAsync(cancellationToken);   
    }

    public async Task<IReadOnlyList<Product>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.Products
            .Include(p => p.Rating)
            .OrderByDescending(p => p.Id)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<Product?> GetProductWithFeedbacksAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        return await dbContext.Products
            .Include(p => p.Feedbacks)
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == productId, cancellationToken);
    }
}