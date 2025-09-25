using ABC.Application.Feedbacks.Common;
using ABC.Domain.Entities;
using ABC.Infrastructure.Persistence.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace ABC.Infrastructure.Persistence.Repositories;

public class FeedbackRepository(ApplicationDbContext dbContext) 
    : Repository<Feedback>(dbContext), IFeedbackRepository
{
    private readonly ApplicationDbContext _dbContext = dbContext;

    public async Task<IReadOnlyList<Feedback>> GetByProductIdAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Feedbacks
            .AsNoTracking()
            .Where(f => f.ProductId == productId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<string>> GetFeedbackTextsForProductAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Feedbacks
            .AsNoTracking()
            .Where(f => f.ProductId == productId)
            .Select(f => f.Text)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> GetCountByProductIdAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Feedbacks.CountAsync(f => f.ProductId == productId, cancellationToken);
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Feedbacks.AnyAsync(f => f.Id == id, cancellationToken);
    }
}