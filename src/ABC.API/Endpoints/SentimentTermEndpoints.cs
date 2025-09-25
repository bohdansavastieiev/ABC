using ABC.API.Common.Extensions;
using ABC.API.Common.Filters;
using ABC.API.Requests;
using ABC.Application.SentimentTerms.Commands.CreateSentimentTerm;
using ABC.Application.SentimentTerms.Common;
using ABC.Application.SentimentTerms.Queries.GetSentimentTermById;
using ABC.Application.SentimentTerms.Queries.GetSentimentTerms;
using MediatR;

namespace ABC.API.Endpoints;

public static class SentimentTermEndpoints
{
    public static void MapSentimentTermEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/sentiment-terms")
            .WithTags("SentimentTerms")
            .AddEndpointFilter<RequireApiKeyEndpointFilter>();
        
        group.MapGet("/{id:guid}", GetSentimentTermByIdAsync)
            .WithName("GetSentimentTermById")
            .Produces<SentimentTermDto>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound);
        
        group.MapGet("/", GetSentimentTermsAsync)
            .Produces<SentimentTermsDto>()
            .Produces(StatusCodes.Status401Unauthorized);
        
        group.MapPost("/", CreateSentimentTermAsync)
            .Produces<SentimentTermDto>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status409Conflict);
    }

    private static async Task<IResult> GetSentimentTermByIdAsync(
        Guid id,
        ISender mediator,
        HttpContext context,
        CancellationToken cancellationToken = default)
    {
        var query = new GetSentimentTermByIdQuery(id);
        var result = await mediator.Send(query, cancellationToken);
    
        return result.IsSuccess 
            ? Results.Ok(result.Value) 
            : result.HandleFailure(context);
    }
    
    private static async Task<IResult> GetSentimentTermsAsync(
        ISender mediator,
        HttpContext context,
        CancellationToken cancellationToken = default)
    {
        var query = new GetSentimentTermsQuery();
        var result = await mediator.Send(query, cancellationToken);
        
        return result.IsSuccess 
            ? Results.Ok(result.Value) 
            : result.HandleFailure(context);
    }
    
    private static async Task<IResult> CreateSentimentTermAsync(
        CreateSentimentTermRequest request,
        ISender mediator,
        HttpContext context,
        CancellationToken cancellationToken = default)
    {
        var command = new CreateSentimentTermCommand(request.Term, request.Score);
        var result = await mediator.Send(command, cancellationToken);
        
        return result.IsSuccess
            ? Results.CreatedAtRoute(
                "GetSentimentTermById", 
                new { id = result.Value.Id }, 
                result.Value)
            : result.HandleFailure(context);
    }
}