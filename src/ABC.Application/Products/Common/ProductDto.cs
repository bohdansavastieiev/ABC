namespace ABC.Application.Products.Common;

public record ProductDto(
    Guid Id,
    string Name,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    RatingDetailsDto RatingDetails);

    
public record RatingDetailsDto(
    Guid Id,
    double? Value,
    bool IsCalculated,
    bool IsOutdated,
    DateTime CreatedAt,
    DateTime UpdatedAt
);