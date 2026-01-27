using EuropeanStudentCard.Models_iKnow;
using FluentValidation;

namespace EuropeanStudentCard.Validation_iKnow
{
    public class IKnowStudentValidator : AbstractValidator<IKnowStudentDto>
    {
        public IKnowStudentValidator()
        {
            RuleFor(x => x.index)
                .NotEmpty()
                .MaximumLength(50);

            RuleFor(x => x.name)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(x => x.surname)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(x => x.status)
                .NotEmpty()
                .Must(s => s == 1 || s == -1)
                .WithMessage("Status must be 'Active (1)' or 'Inactive (0)'.");

            RuleFor(x => x.email)
                .EmailAddress()
                .When(x => !string.IsNullOrWhiteSpace(x.email));
        }
    }
   }
