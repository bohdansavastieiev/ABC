using FluentValidation;

namespace ABC.Application.ProductRatings.Commands.RequestRecalculation;

public class RequestRecalculationCommandValidator : AbstractValidator<RequestRecalculationCommand>
{
    public RequestRecalculationCommandValidator()
    {
        RuleFor(x => x.BatchSize)
            .GreaterThan(0)
            .When(x => x.BatchSize.HasValue);
    }
}