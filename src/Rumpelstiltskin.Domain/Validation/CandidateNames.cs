using FluentValidation;
using System.Linq;

namespace Rumpelstiltskin.Domain.Validation
{
    public static class CandidateNames
    {
        /// <summary>By default the shortlist of names to choose from should not be less then 20.</summary>
        public static readonly int MinimumSelection = 10;

        /// <summary>There should be at least some names to choose from.</summary>
        public static IRuleBuilderOptions<T, CandidateNameCollection> HasMinimumSelection<T>(this IRuleBuilder<T, CandidateNameCollection> builder, int minSize)
            => Guard.NotNull(builder, nameof(builder))
            .Must(names => names.Active().Count() >= minSize)
            .WithFormattedMessage(Message.MinimumSelectionSize, minSize);
    }
}
