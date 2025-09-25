using ABC.Application.Abstractions;
using ABC.Application.Abstractions.Persistence;
using ABC.Domain.Entities;

namespace ABC.Application.SentimentTerms.Common;

public interface ISentimentTermRepository : IRepository<SentimentTerm>
{
    Task<IReadOnlyList<SentimentTerm>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<bool> TermExistsAsync(string term, CancellationToken cancellationToken = default);
}