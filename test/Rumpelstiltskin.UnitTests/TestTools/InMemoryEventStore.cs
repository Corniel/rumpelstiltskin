using Qowaiv.DomainModel;
using Qowaiv.Validation.Abstractions;
using Rumpelstiltskin.Shared.Storage;
using Rumpelstiltskin.Shared.Validation;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Rumpelstiltskin.UnitTests.TestTools
{
    internal class InMemoryEventStore<TId> : IEventStore<TId>
    {
        public InMemoryEventStore(EventBuffer<TId> initial)
        {
            buffers.Add(initial.AggregateId, initial.MarkAllAsCommitted());
        }

        public Task<Result<int>> GetCurrentVersionAsync(TId aggregateId)
        => LoadAsync(aggregateId)
            .ActAsync(buffer => Result.For(buffer.CommittedVersion));

        public Task<Result<EventBuffer<TId>>> LoadAsync(TId aggregateId)
        => Task.FromResult(buffers.TryGetValue(aggregateId, out EventBuffer<TId> buffer)
            ? Result.For(buffer)
            : Result.WithMessages<EventBuffer<TId>>(new EntityNotFound()));

        public Task<Result> SaveAsync(EventBuffer<TId> buffer)
        {
            if (buffers.TryGetValue(buffer.AggregateId, out EventBuffer<TId> existing))
            {
                existing.AddRange(buffer.Uncommitted);
                existing.MarkAllAsCommitted();
                buffer.MarkAllAsCommitted();
            }
            else
            {
                buffers.Add(buffer.AggregateId, buffer.MarkAllAsCommitted());
            }
            return Task.FromResult(Result.OK);
        }

        private readonly Dictionary<TId, EventBuffer<TId>> buffers = new Dictionary<TId, EventBuffer<TId>>();
    }
}
