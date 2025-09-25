using ABC.Domain.Entities;

namespace ABC.Application.Products.Common;

public interface IProductRepository
{
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ICollection<Product>> GetProductsWithOutdatedRatingAndFeedbacksAsync(
        int amount,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Product>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Product?> GetProductWithFeedbacksAsync(Guid productId, CancellationToken cancellationToken = default);

}