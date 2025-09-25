using ABC.Domain.Entities;

namespace ABC.Application.SentimentTerms.Common;

public static class SentimentTermMapper
{
    public static SentimentTermsDto ToDto(this IEnumerable<SentimentTerm> sentimentTerms)
    {
        var dtoList = sentimentTerms
            .Select(term => new SentimentTermDto(
                term.Id,
                term.CreatedAt,
                term.UpdatedAt,
                term.Term,
                term.Score))
            .ToList();

        return new SentimentTermsDto(dtoList);
    }
}