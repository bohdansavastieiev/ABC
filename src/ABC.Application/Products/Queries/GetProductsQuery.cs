using ABC.Application.Products.Common;
using FluentResults;
using MediatR;

namespace ABC.Application.Products.Queries;

public record GetProductsQuery() : IRequest<Result<ProductsDto>>;