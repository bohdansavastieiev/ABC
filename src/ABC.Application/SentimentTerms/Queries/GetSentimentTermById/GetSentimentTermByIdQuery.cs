using ABC.Application.SentimentTerms.Common;
using FluentResults;
using MediatR;

namespace ABC.Application.SentimentTerms.Queries.GetSentimentTermById;

public record GetSentimentTermByIdQuery(Guid Id) : IRequest<Result<SentimentTermDto>>;