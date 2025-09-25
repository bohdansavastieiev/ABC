using ABC.Application.SentimentTerms.Common;
using FluentResults;
using MediatR;

namespace ABC.Application.SentimentTerms.Queries.GetSentimentTerms;

public record GetSentimentTermsQuery() : IRequest<Result<SentimentTermsDto>>;