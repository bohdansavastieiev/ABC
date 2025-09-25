using FluentResults;
using MediatR;

namespace ABC.Application.ProductRatings.Commands.RequestRecalculation;

public record RequestRecalculationCommand(int? BatchSize) : IRequest<Result>;