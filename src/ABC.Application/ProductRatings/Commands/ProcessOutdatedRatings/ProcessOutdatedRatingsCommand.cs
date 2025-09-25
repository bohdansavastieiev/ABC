using FluentResults;
using MediatR;

namespace ABC.Application.ProductRatings.Commands.ProcessOutdatedRatings;

public record ProcessOutdatedRatingsCommand(int BatchSize) : IRequest<Result<int>>;