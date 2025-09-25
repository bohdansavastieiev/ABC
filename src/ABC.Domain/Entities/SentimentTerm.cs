using ABC.Domain.Abstractions;
using ABC.Domain.Common;

namespace ABC.Domain.Entities;

public class SentimentTerm : Entity
{
    public string Term { get; init; }
    public double Score { get; init; }
    public bool IsActive { get; init; }
    
    public SentimentTerm(string term, double score, bool isActive = true)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(term, nameof(term));
        
        if (score is > DomainConstants.MaxScore or < DomainConstants.MinScore)
        {
            throw new ArgumentException(
                $"Score for a term must be between {DomainConstants.MinScore} and {DomainConstants.MaxScore}",
                nameof(score));
        }
        
        Term = term;
        Score = score;
        IsActive = isActive;
    }
}