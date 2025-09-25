using ABC.Application.Common.Errors;
using ABC.Application.Common.Errors.Common;
using ABC.Application.ProductRatings.Common;
using ABC.Application.Products.Common;
using ABC.Domain.Entities;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ABC.Application.ProductRatings.Queries.GetProductRatingByProductId;

public class GetProductRatingByProductIdQueryHandler(
    IProductRatingRepository productRatingRepository,
    ILogger<GetProductRatingByProductIdQueryHandler> logger) 
    : IRequestHandler<GetProductRatingByProductIdQuery, Result<ProductRatingDto>>
{
    public async Task<Result<ProductRatingDto>> Handle(GetProductRatingByProductIdQuery request, CancellationToken cancellationToken)
    {
        var productRating = await productRatingRepository
            .GetByProductIdAsync(request.ProductId, cancellationToken);
        
        if (productRating is null)
        {
            logger.LogWarning("Product rating with Product ID {ProductID} not found", request.ProductId);
            return Result.Fail(new NotFoundError(
                nameof(ProductRating), nameof(ProductRating.ProductId), request.ProductId));
        }

        return new ProductRatingDto(
            Id: productRating.Id,
            ProductId: productRating.ProductId,
            CreatedAt: productRating.CreatedAt,
            UpdatedAt: productRating.UpdatedAt,
            Value: productRating.Value,
            IsCalculated: productRating.IsCalculated,
            IsOutdated: productRating.IsOutdated);
    }
}