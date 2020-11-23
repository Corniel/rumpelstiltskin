using FluentValidation;
using Qowaiv.Validation.Fluent;
using Rumpelstiltskin.Domain.Filters;

namespace Rumpelstiltskin.Domain.Validation
{
    public class NameLengthFilterValdator : FluentModelValidator<NameLengthFilter>
    {
        public NameLengthFilterValdator()
        {
            RuleFor(m => m.MinLength).Must(p => p >= 0).WithMessage("Minimum length can not be negative.");
            RuleFor(m => m.MaxLength).Must((filter, p) => p >= filter.MinLength).WithMessage("Maximum length should be bigger than minimum length.");
        }
    }
}
