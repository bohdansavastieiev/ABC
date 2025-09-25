using ABC.Application.ProductRatings.Common;
using FluentResults;
using MediatR;

namespace ABC.Application.ProductRatings.Queries.GetProductRatingByProductId;

public record GetProductRatingByProductIdQuery(Guid ProductId) : IRequest<Result<ProductRatingDto>>;