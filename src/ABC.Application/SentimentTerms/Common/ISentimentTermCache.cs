using ABC.Domain.Entities;

namespace ABC.Application.SentimentTerms.Common;

public interface ISentimentTermCache
{
    Task<IReadOnlyList<SentimentTerm>> GetTermsAsync(CancellationToken cancellationToken = default);
    void Clear();
}