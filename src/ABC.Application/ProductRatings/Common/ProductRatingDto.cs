namespace ABC.Application.ProductRatings.Common;

public record ProductRatingDto(
    Guid Id,
    Guid ProductId,
    DateTime CreatedAt,
    DateTime UpdatedAt, 
    double? Value,
    bool IsCalculated,
    bool IsOutdated);