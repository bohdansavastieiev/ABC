using ABC.Application.ProductRatings.Commands.RequestRecalculation;
using FluentValidation;

namespace ABC.Application.ProductRatings.Commands.ProcessOutdatedRatings;

public class ProcessOutdatedRatingsCommandValidator : AbstractValidator<RequestRecalculationCommand>
{
    public ProcessOutdatedRatingsCommandValidator()
    {
        RuleFor(x => x.BatchSize)
            .GreaterThan(0);
    }
}