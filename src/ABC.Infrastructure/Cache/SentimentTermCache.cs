using ABC.Application.SentimentTerms.Common;
using ABC.Domain.Entities;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace ABC.Infrastructure.Cache;

public class SentimentTermCache(
    IMemoryCache memoryCache,
    ISentimentTermRepository sentimentTermRepository,
    IOptions<CacheOptions> cacheOptions) 
    : ISentimentTermCache
{
    private const string CacheKey = "SentimentTerms";
    private readonly CacheOptions _cacheOptions = cacheOptions.Value
        ?? throw new ArgumentNullException(nameof(cacheOptions));
    
    public async Task<IReadOnlyList<SentimentTerm>> GetTermsAsync(CancellationToken cancellationToken = default)
    {
        return await memoryCache.GetOrCreateAsync(CacheKey, async entry =>
        {
            entry.SetSlidingExpiration(TimeSpan.FromHours(_cacheOptions.SlidingExpirationHours));
            return await sentimentTermRepository.GetAllAsync(cancellationToken);
        }) ?? [];
    }

    public void Clear()
    {
        memoryCache.Remove(CacheKey);
    }
}