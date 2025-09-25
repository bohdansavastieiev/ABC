using ABC.Domain.Abstractions;
using ABC.Domain.Common;

namespace ABC.Domain.Entities;

public class Feedback : Entity
{
    public Guid ProductId { get; init; }
    public string CustomerName { get; init; }
    public string Text { get; init; }
    public bool IsActive { get; init; }
    public Feedback(Guid productId, string customerName, string text, bool isActive = true)
    {
        if (productId == Guid.Empty)
        {
            throw new ArgumentException("Product ID cannot be empty", nameof(productId));
        }
        ArgumentException.ThrowIfNullOrWhiteSpace(customerName, nameof(customerName));
        ArgumentException.ThrowIfNullOrWhiteSpace(text, nameof(text));

        ProductId = productId;
        CustomerName = customerName;
        Text = text;
        IsActive = isActive;
    }
}