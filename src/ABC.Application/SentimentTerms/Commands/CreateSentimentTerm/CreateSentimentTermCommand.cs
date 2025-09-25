using ABC.Application.SentimentTerms.Common;
using FluentResults;
using MediatR;

namespace ABC.Application.SentimentTerms.Commands.CreateSentimentTerm;

public record CreateSentimentTermCommand(
    string Term, double Score) 
    : IRequest<Result<SentimentTermDto>>;