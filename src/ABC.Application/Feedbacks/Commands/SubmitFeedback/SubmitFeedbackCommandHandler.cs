using ABC.Application.Abstractions;
using ABC.Application.Abstractions.Persistence;
using ABC.Application.Common.Errors;
using ABC.Application.Feedbacks.Common;
using ABC.Application.Products.Common;
using ABC.Domain.Entities;
using FluentResults;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ABC.Application.Feedbacks.Commands.SubmitFeedback;

public class SubmitFeedbackCommandHandler(
    IFeedbackRepository feedbackRepository,
    IProductRepository productRepository,
    IUnitOfWork unitOfWork,
    IEventPublisher eventPublisher,
    ILogger<SubmitFeedbackCommandHandler> logger)
    : IRequestHandler<SubmitFeedbackCommand, Result<FeedbackDto>>
{
    public async Task<Result<FeedbackDto>> Handle(SubmitFeedbackCommand request, CancellationToken cancellationToken)
    {
        var productExists = await productRepository.ExistsAsync(request.ProductId, cancellationToken);
        if (!productExists)
        {
            logger.LogWarning("CreateFeedbackCommand failed: Product with ID {ProductId} not found", request.ProductId);
            return Result.Fail(new ReferencedResourceNotFoundError(
                nameof(Product), nameof(Feedback.ProductId), request.ProductId));
        }
        
        var feedback = new Feedback(request.ProductId, request.CustomerName, request.Text);
        feedbackRepository.Add(feedback);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Feedback {FeedbackId} created for Product {ProductId}", feedback.Id, feedback.ProductId);
        
        var feedbackCreatedEvent = new FeedbackSubmittedEvent(feedback.ProductId, feedback.Id);
        await eventPublisher.PublishAsync(feedbackCreatedEvent, cancellationToken);
        logger.LogInformation("Published FeedbackCreatedEvent for FeedbackId: {FeedbackId}", feedback.Id);
        
        return new FeedbackDto(
            Id: feedback.Id,
            ProductId: feedback.ProductId,
            CreatedAt: feedback.CreatedAt,
            UpdatedAt: feedback.UpdatedAt,
            CustomerName: feedback.CustomerName,
            Text: feedback.Text);
    }
}