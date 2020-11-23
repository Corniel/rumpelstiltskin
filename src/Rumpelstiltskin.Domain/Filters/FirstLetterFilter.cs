using Qowaiv.Formatting;
using System.Linq;

namespace Rumpelstiltskin.Domain.Filters
{
    /// <summary>Filters on the first letter of a candidate name.</summary>
    public class FirstLetterFilter : IFilter
    {
        /// <summary>Gets the allowed first letters.</summary>
        public string Allowed { get; internal set; } = Letters.Alphabet;

        /// <inheritdoc />
        public bool Includes(CandidateName candidate)
        {
            Guard.NotNull(candidate, nameof(candidate));

            return Allowed.IndexOf(candidate.First) != -1;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"Allowed: {Allowed}, Vetoed: {new string(Letters.Alphabet.Where(ch => Allowed.IndexOf(ch) == -1).ToArray())}";
        }

        /// <summary>Gets a normalized allowed string based on the vetoed letters.</summary>
        public static string Veto(string vetoed)
        {
            return new string(Letters.Alphabet.Where(ch => StringFormatter.ToNonDiacritic(vetoed ?? string.Empty).ToUpperInvariant().IndexOf(ch) == -1).ToArray());
        }
    }
}
