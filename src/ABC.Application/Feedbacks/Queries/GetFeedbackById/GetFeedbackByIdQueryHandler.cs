using ABC.Application.Common.Errors;
using ABC.Application.Common.Errors.Common;
using ABC.Application.Feedbacks.Common;
using ABC.Domain.Entities;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ABC.Application.Feedbacks.Queries.GetFeedbackById;

public class GetFeedbackByIdQueryHandler(
    IFeedbackRepository feedbackRepository,
    ILogger<GetFeedbackByIdQueryHandler> logger) 
    : IRequestHandler<GetFeedbackByIdQuery, Result<FeedbackDto>>
{
    public async Task<Result<FeedbackDto>> Handle(GetFeedbackByIdQuery request, CancellationToken cancellationToken)
    {
        var feedback = await feedbackRepository.GetByIdAsync(request.Id, cancellationToken);
        if (feedback is null)
        {
            logger.LogWarning("Feedback with ID {RequestId} not found", request.Id);
            return Result.Fail(new NotFoundError(nameof(Feedback), nameof(request.Id), request.Id));
        }
        
        return new FeedbackDto(
                Id: feedback.Id,
                ProductId: feedback.ProductId,
                CreatedAt: feedback.CreatedAt,
                UpdatedAt: feedback.UpdatedAt,
                CustomerName: feedback.CustomerName,
                Text: feedback.Text);
    }
}