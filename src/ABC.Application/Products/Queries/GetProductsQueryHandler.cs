using ABC.Application.Products.Common;
using FluentResults;
using MediatR;

namespace ABC.Application.Products.Queries;

public class GetProductsQueryHandler(
    IProductRepository productRepository) 
    : IRequestHandler<GetProductsQuery, Result<ProductsDto>>
{
    public async Task<Result<ProductsDto>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        var products = await productRepository.GetAllAsync(cancellationToken);
        return products.ToDto();
    }
}