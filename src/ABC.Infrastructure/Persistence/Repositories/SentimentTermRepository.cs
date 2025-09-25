using ABC.Application.SentimentTerms.Common;
using ABC.Domain.Entities;
using ABC.Infrastructure.Persistence.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace ABC.Infrastructure.Persistence.Repositories;

public class SentimentTermRepository(ApplicationDbContext dbContext) 
    : Repository<SentimentTerm>(dbContext), ISentimentTermRepository
{
    private readonly ApplicationDbContext _dbContext = dbContext;

    public async Task<IReadOnlyList<SentimentTerm>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.SentimentTerms
            .OrderByDescending(st => st.Score)
            .ThenBy(st => st.Term)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> TermExistsAsync(string term, CancellationToken cancellationToken = default)
    {
        return await _dbContext.SentimentTerms.AnyAsync(st => st.Term == term, cancellationToken);
    }
}