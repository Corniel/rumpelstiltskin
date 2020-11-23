using FluentValidation;
using Qowaiv.Validation.Fluent;

namespace Rumpelstiltskin.Domain.Validation
{
    public class NameSelectionValidator : FluentModelValidator<NameSelection>
    {
        public NameSelectionValidator()
        {
            RuleFor(m => m.NameLength).SetValidator(new NameLengthFilterValdator());
            RuleFor(m => m.Names)
                .HasMinimumSelection(CandidateNames.MinimumSelection);
                //.When(m => m.Version > 1);
        }
    }
}
