namespace ABC.Application.SentimentTerms.Common;

public record SentimentTermDto(
    Guid Id,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    string Term,
    double Score);