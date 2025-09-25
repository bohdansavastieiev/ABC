namespace ABC.Application.SentimentTerms.Common;

public record SentimentTermsDto
{
    public List<SentimentTermDto> SentimentTerms { get; set; } = [];

    public SentimentTermsDto(List<SentimentTermDto> sentimentTerms)
    {
        SentimentTerms = sentimentTerms;
    }
}