using ABC.Application.SentimentTerms.Common;
using FluentResults;
using MediatR;

namespace ABC.Application.SentimentTerms.Queries.GetSentimentTerms;

public class GetSentimentTermsQueryHandler(
    ISentimentTermRepository sentimentTermRepository) 
    : IRequestHandler<GetSentimentTermsQuery, Result<SentimentTermsDto>>
{
    public async Task<Result<SentimentTermsDto>> Handle(GetSentimentTermsQuery request, CancellationToken cancellationToken)
    {
        var sentimentTerms = await sentimentTermRepository.GetAllAsync(cancellationToken);
        return sentimentTerms.ToDto();
    }
}