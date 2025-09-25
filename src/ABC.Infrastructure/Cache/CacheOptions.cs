namespace ABC.Infrastructure.Cache;

public class CacheOptions
{
    public const string SectionName = "Cache";
    public int SlidingExpirationHours { get; set; }
}