using ABC.Application.Common.Constants;
using FluentValidation;

namespace ABC.Application.Feedbacks.Commands.SubmitFeedback;

public class SubmitFeedbackCommandValidator : AbstractValidator<SubmitFeedbackCommand>
{
    public SubmitFeedbackCommandValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty()
            .WithMessage("Product ID is required");

        RuleFor(x => x.CustomerName)
            .NotEmpty()
            .WithMessage("Customer name is required")
            .MaximumLength(ValidationConstants.MaxNameLength)
            .WithMessage($"Customer name must not exceed {ValidationConstants.MaxNameLength} characters");

        RuleFor(x => x.Text)
            .NotEmpty()
            .WithMessage("Feedback text is required")
            .MaximumLength(ValidationConstants.MaxTextLength)
            .WithMessage($"Feedback text must not exceed {ValidationConstants.MaxTextLength} characters");
    }
}