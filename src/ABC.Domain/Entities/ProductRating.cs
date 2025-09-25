using ABC.Domain.Abstractions;
using ABC.Domain.Common;

namespace ABC.Domain.Entities;

public class ProductRating : Entity
{
    public Guid ProductId { get; init; }
    public double? Value { get; set; }
    public bool IsCalculated { get; set; }
    public bool IsOutdated { get; set; }
    
    public ProductRating(Guid productId, double? value = null, bool isCalculated = false, bool isOutdated = false)
    {
        if (productId == Guid.Empty)
        {
            throw new ArgumentException("Product ID cannot be empty", nameof(productId));
        }
        if (value is > DomainConstants.MaxScore or < DomainConstants.MinScore)
        {
            throw new ArgumentException(
                $"Product rating cannot be lower than {DomainConstants.MinScore} or higher than {DomainConstants.MaxScore}",
                nameof(value));
        }
        
        ProductId = productId;
        Value = value;
        IsCalculated = isCalculated;
        IsOutdated = isOutdated;
    }
}