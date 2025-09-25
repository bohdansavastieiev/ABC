using ABC.Application.Abstractions;
using ABC.Application.Abstractions.Persistence;
using ABC.Application.Common.Errors;
using ABC.Application.SentimentTerms.Common;
using ABC.Domain.Entities;
using FluentResults;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ABC.Application.SentimentTerms.Commands.CreateSentimentTerm;

public class CreateSentimentTermCommandHandler(
    IUnitOfWork unitOfWork,
    ISentimentTermRepository sentimentTermRepository,
    IEventPublisher eventPublisher,
    ILogger<CreateSentimentTermCommandHandler> logger)
    : IRequestHandler<CreateSentimentTermCommand, Result<SentimentTermDto>>
{
    public async Task<Result<SentimentTermDto>> Handle(CreateSentimentTermCommand request, CancellationToken cancellationToken)
    {
        var lowerCaseTerm = request.Term.ToLowerInvariant();
        if (await sentimentTermRepository.TermExistsAsync(lowerCaseTerm, cancellationToken))
        {
            logger.LogWarning("CreateSentimentTermCommand failed: Term {Term} already exists in the system", lowerCaseTerm);
            return Result.Fail(new PropertyNotUniqueError(
                nameof(SentimentTerm), nameof(SentimentTerm.Term), lowerCaseTerm));
        }
        
        var sentimentTerm = new SentimentTerm(lowerCaseTerm, request.Score);
        sentimentTermRepository.Add(sentimentTerm);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        logger.LogInformation("SentimentTerm {SentimentTermId} created for Term: '{Term}'", sentimentTerm.Id, sentimentTerm.Term);
        
        var sentimentTermCreatedEvent = new SentimentTermCreatedEvent(sentimentTerm.Id);
        await eventPublisher.PublishAsync(sentimentTermCreatedEvent, cancellationToken);
        logger.LogInformation("Published SentimentTermCreatedEvent for SentimentTermId: {SentimentTermId}", sentimentTerm.Id);
        
        return new SentimentTermDto(
            sentimentTerm.Id,
            sentimentTerm.CreatedAt,
            sentimentTerm.UpdatedAt,
            sentimentTerm.Term,
            sentimentTerm.Score);

    }
}