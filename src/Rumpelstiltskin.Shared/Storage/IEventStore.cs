using Qowaiv.DomainModel;
using Qowaiv.Validation.Abstractions;
using System.Threading.Tasks;

namespace Rumpelstiltskin.Shared.Storage
{
    public interface IEventStore<TId>
    {
        /// <summary>Loads an <see cref="EventBuffer{TId}"/> for the specified Aggregate (root).</summary>
        Task<Result<EventBuffer<TId>>> LoadAsync(TId aggregateId);

        /// <summary>Gets the current/committed version of the specified Aggregate (root).</summary>
        Task<Result<int>> GetCurrentVersionAsync(TId aggregateId);

        /// <summary>Saves the uncommitted <see cref="EventBuffer{TId}"/>s of the event stream.</summary>
        Task<Result> SaveAsync(EventBuffer<TId> buffer);
    }
}
