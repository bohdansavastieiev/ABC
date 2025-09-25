using ABC.API.Common.Extensions;
using ABC.API.Requests;
using ABC.Application.Feedbacks.Commands.SubmitFeedback;
using ABC.Application.Feedbacks.Common;
using ABC.Application.Feedbacks.Queries.GetFeedbackById;
using ABC.Application.Feedbacks.Queries.GetFeedbacksByProductId;
using MediatR;

namespace ABC.API.Endpoints;

public static class FeedbackEndpoints
{
    public static void MapFeedbackEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/feedbacks")
            .WithTags("Feedbacks");

        group.MapGet("/{id:guid}", GetFeedbackByIdAsync)
            .WithName("GetFeedbackById")
            .Produces<FeedbackDto>()
            .Produces(StatusCodes.Status404NotFound);
        
        
        var productGroup = app.MapGroup("/api/v1/products")
            .WithTags("Products");
        
        productGroup.MapGet("/{productId:guid}/feedbacks", GetFeedbacksByProductIdAsync)
            .Produces<FeedbacksDto>();
        
        productGroup.MapPost("/{productId:guid}/feedbacks", CreateFeedbackAsync)
            .Produces<FeedbackDto>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound);
    }

    private static async Task<IResult> GetFeedbackByIdAsync(
        Guid id,
        ISender mediator,
        HttpContext context,
        CancellationToken cancellationToken = default)
    {
        var query = new GetFeedbackByIdQuery(id);
        var result = await mediator.Send(query, cancellationToken);

        return result.IsSuccess
            ? Results.Ok(result.Value)
            : result.HandleFailure(context);
    }
    
    private static async Task<IResult> GetFeedbacksByProductIdAsync(
        Guid productId,
        ISender mediator,
        HttpContext context,
        CancellationToken cancellationToken = default)
    {
        var query = new GetFeedbacksByProductIdQuery(productId);
        var result = await mediator.Send(query, cancellationToken);
        
        return result.IsSuccess 
            ? Results.Ok(result.Value) 
            : result.HandleFailure(context);
    }

    private static async Task<IResult> CreateFeedbackAsync(
        Guid productId,
        CreateFeedbackRequest request,
        ISender mediator,
        HttpContext context,
        CancellationToken cancellationToken = default)
    {
        var command = new SubmitFeedbackCommand(
            productId, request.Text, request.CustomerName);
        var result = await mediator.Send(command, cancellationToken);
        
        return result.IsSuccess
            ? Results.CreatedAtRoute(
                "GetFeedbackById",
                new { id = result.Value.Id },
                result.Value)
            : result.HandleFailure(context);
    }
}