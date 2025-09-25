using ABC.Application.Abstractions;
using ABC.Application.Abstractions.Persistence;
using ABC.Domain.Entities;

namespace ABC.Application.ProductRatings.Common;

public interface IProductRatingRepository
{ 
    Task<ProductRating?> GetByProductIdAsync(Guid productId, CancellationToken cancellationToken = default);
    void Update(ProductRating productRating);
    Task<int> MarkOutdatedForTermAsync(string termText, CancellationToken cancellationToken = default);
}