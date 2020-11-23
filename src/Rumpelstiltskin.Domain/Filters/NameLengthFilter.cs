namespace Rumpelstiltskin.Domain.Filters
{
    public class NameLengthFilter : IFilter
    {
        public int MinLength { get; internal set; }
        public int MaxLength { get; internal set; } = int.MaxValue;

        /// <inheritdoc />
        public bool Includes(CandidateName candidate)
        {
            var length = Guard.NotNull(candidate, nameof(candidate)).Name.Length;
            return length >= MinLength && length <= MaxLength;
        }
    }
}
