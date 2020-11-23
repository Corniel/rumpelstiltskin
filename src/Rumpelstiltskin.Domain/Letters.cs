using Qowaiv.Formatting;
using System.Linq;

namespace Rumpelstiltskin.Domain
{
    public static class Letters
    {
        public static readonly string Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        public static string Allowed(string forbidden)
        {
            var normalized = Normalize(forbidden);

            return new string(Alphabet.Where(ch => !normalized.Contains(ch)).ToArray());
        }

        /// <summary>Normalizes the allowed string.</summary>
        public static string Normalize(string allowed)
        {
            return new string(StringFormatter.ToNonDiacritic((allowed ?? string.Empty).ToUpperInvariant()).Distinct().OrderBy(ch => ch).ToArray());
        }
    }
}
