using Qowaiv.Formatting;
using Qowaiv.Statistics;
using System;

#pragma warning disable S1210 // "Equals" and the comparison operators should be overridden when implementing "IComparable"
// We just want to sort.

namespace Rumpelstiltskin.Domain
{
    public sealed class CandidateName : IComparable<CandidateName>
    {
        /// <summary>Gets the initial Elo for a candidate name.</summary>
        public static readonly Elo Intitial = 1600;

        public CandidateName(string name) : this(name, Intitial) { }

        public CandidateName(string name, Elo elo)
        {
            Name = name;
            Elo = elo;
            First = char.ToUpperInvariant(StringFormatter.ToNonDiacritic(name)[0]);
        }

        /// <summary>Gets the name of the candidate.</summary>
        public string Name { get; }

        /// <summary>Gets the first character of the candidate.</summary>
        public char First { get; }

        /// <summary>Gets the Elo of the candidate.</summary>
        public Elo Elo { get; internal set; }

        /// <summary>Gets the number of votes of the candidate.</summary>
        public int Votes { get; internal set; }

        /// <summary>Gets if the candidate has been vetoed or not.</summary>
        public bool HasVeto { get; internal set; }

        /// <inheritdoc />
        public override string ToString() => string.Format("{0:0000} {1}{2} ({3})", Elo, Name, HasVeto ? "*" : string.Empty, Votes);

        /// <inheritdoc />
        public int CompareTo(CandidateName other)
        {
            Guard.NotNull(other, nameof(other));

            var compare = HasVeto.CompareTo(other.HasVeto);

            if (compare == 0)
            {
                compare = other.Elo.CompareTo(Elo);
            }
            return compare;
        }
    }
}
