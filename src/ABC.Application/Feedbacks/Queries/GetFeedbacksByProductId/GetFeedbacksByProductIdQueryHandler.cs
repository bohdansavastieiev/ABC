using ABC.Application.Common.Errors;
using ABC.Application.Feedbacks.Common;
using ABC.Application.Products.Common;
using ABC.Domain.Entities;
using FluentResults;
using MediatR;

namespace ABC.Application.Feedbacks.Queries.GetFeedbacksByProductId;

public class GetFeedbacksByProductIdQueryHandler(
    IProductRepository productRepository) 
    : IRequestHandler<GetFeedbacksByProductIdQuery, Result<FeedbacksDto>>
{
    public async Task<Result<FeedbacksDto>> Handle(GetFeedbacksByProductIdQuery request, CancellationToken cancellationToken)
    {
        var productWithFeedbacks = await productRepository
            .GetProductWithFeedbacksAsync(request.ProductId, cancellationToken);
        if (productWithFeedbacks is null)
        {
            return Result.Fail(new ReferencedResourceNotFoundError(
                nameof(Product), nameof(Feedback.ProductId), request.ProductId));
        }
        
        return productWithFeedbacks.ToDtoWithFeedbacks();
    }
}