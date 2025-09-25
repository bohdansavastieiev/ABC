using ABC.API.Common.Extensions;
using ABC.API.Common.Filters;
using ABC.API.Requests;
using ABC.Application.ProductRatings.Commands.RequestRecalculation;
using ABC.Application.ProductRatings.Common;
using ABC.Application.ProductRatings.Queries.GetProductRatingByProductId;
using MediatR;

namespace ABC.API.Endpoints;

public static class ProductRatingEndpoints
{
    public static void MapProductRatingEndpoints(this IEndpointRouteBuilder app)
    {
        var productGroup = app.MapGroup("/api/v1/products")
            .WithTags("Products");

        productGroup.MapGet("/{productId:guid}/rating", GetProductRatingAsync)
            .Produces<ProductRatingDto>()
            .Produces(StatusCodes.Status404NotFound);

        var productRatingGroup = app.MapGroup("/api/v1/product-ratings")
            .WithTags("ProductRatings")
            .AddEndpointFilter<RequireApiKeyEndpointFilter>();

        productRatingGroup.MapPost("/recalculate-outdated", RequestRecalculationAsync)
            .Produces(StatusCodes.Status202Accepted)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized);
    }

    private static async Task<IResult> GetProductRatingAsync(
        Guid productId, 
        ISender mediator,
        HttpContext context,
        CancellationToken cancellationToken = default)
    {
        var query = new GetProductRatingByProductIdQuery(productId);
        var result = await mediator.Send(query, cancellationToken);
        
        return result.IsSuccess 
            ? Results.Ok(result.Value) 
            : result.HandleFailure(context);
    }

    private static async Task<IResult> RequestRecalculationAsync(
        RecalculateOutdatedRatingsRequest request,
        ISender mediator,
        HttpContext context,
        CancellationToken cancellationToken = default)
    {
        var command = new RequestRecalculationCommand(request.BatchSize);
        var result = await mediator.Send(command, cancellationToken);

        return result.IsSuccess 
            ? Results.Accepted() 
            : result.HandleFailure(context);
    }
}