using FluentValidation;
using Qowaiv.Validation.Fluent;
using Rumpelstiltskin.Domain.Commands;

namespace Rumpelstiltskin.Domain.Validation
{
    public class CompareValidator : FluentModelValidator<Compare>
    {
        public CompareValidator()
        {
            RuleFor(m => m.Name1).NotEmpty();
            RuleFor(m => m.Name2).NotEmpty();
            RuleFor(m => m.Comparison).IsInEnum();
        }
    }
}
