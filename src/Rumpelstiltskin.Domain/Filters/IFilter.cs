namespace Rumpelstiltskin.Domain.Filters
{
    /// <summary>Filters on candidate names.</summary>
    public interface IFilter
    {
        /// <summary>Returns true for the candidate if it should be included according to this filter.</summary>
        bool Includes(CandidateName candidate);
    }
}
