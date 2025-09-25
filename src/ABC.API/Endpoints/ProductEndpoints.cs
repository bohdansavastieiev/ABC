using ABC.API.Common.Extensions;
using ABC.Application.ProductRatings.Queries.GetProductRatingByProductId;
using ABC.Application.Products.Common;
using ABC.Application.Products.Queries;
using MediatR;

namespace ABC.API.Endpoints;

public static class ProductEndpoints
{
    public static void MapProductEndpoints(this IEndpointRouteBuilder app)
    {
        var productGroup = app.MapGroup("/api/v1/products")
            .WithTags("Products");

        productGroup.MapGet("/", GetProductsAsync)
            .Produces<ProductsDto>();
    }
    
    private static async Task<IResult> GetProductsAsync(
        ISender mediator,
        HttpContext context,
        CancellationToken cancellationToken = default)
    {
        var query = new GetProductsQuery();
        var result = await mediator.Send(query, cancellationToken);
        
        return result.IsSuccess 
            ? Results.Ok(result.Value) 
            : result.HandleFailure(context);
    }
}