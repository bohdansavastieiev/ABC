using ABC.Application.Common.Errors;
using ABC.Application.Common.Errors.Common;
using ABC.Application.SentimentTerms.Common;
using ABC.Domain.Entities;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ABC.Application.SentimentTerms.Queries.GetSentimentTermById;

public class GetSentimentTermByIdQueryHandler(
    ISentimentTermRepository sentimentTermRepository,
    ILogger<GetSentimentTermByIdQueryHandler> logger)
    : IRequestHandler<GetSentimentTermByIdQuery, Result<SentimentTermDto>> 
{
    public async Task<Result<SentimentTermDto>> Handle(GetSentimentTermByIdQuery request, CancellationToken cancellationToken)
    {
       var sentimentTerm = await sentimentTermRepository.GetByIdAsync(request.Id, cancellationToken);
        if (sentimentTerm == null)
        {
            logger.LogWarning("SentimentTerm with ID {SentimentTermId} not found", request.Id);
            return Result.Fail(new NotFoundError(nameof(SentimentTerm), nameof(SentimentTerm.Id), request.Id));
        }
        
        return new SentimentTermDto(
            sentimentTerm.Id, sentimentTerm.CreatedAt, sentimentTerm.UpdatedAt, sentimentTerm.Term, sentimentTerm.Score);
    }
}