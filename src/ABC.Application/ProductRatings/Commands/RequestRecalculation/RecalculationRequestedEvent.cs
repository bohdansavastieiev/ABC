namespace ABC.Application.ProductRatings.Commands.RequestRecalculation;

public record RecalculationRequestedEvent(int? BatchSize);