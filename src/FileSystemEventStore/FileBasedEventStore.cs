using Newtonsoft.Json;
using Qowaiv.DomainModel;
using Qowaiv.Validation.Abstractions;
using Rumpelstiltskin;
using Rumpelstiltskin.Shared.Serialization;
using Rumpelstiltskin.Shared.Storage;
using Rumpelstiltskin.Shared.Validation;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace FileSystemEventStore
{
    public class FileBasedEventStore<TId> : IEventStore<TId>
    {
        private readonly DirectoryInfo rootStorage;

        public FileBasedEventStore(DirectoryInfo rootStorage)
        {
            Guard.NotNull(rootStorage, nameof(rootStorage));
            this.rootStorage = new DirectoryInfo(Path.Combine(rootStorage.FullName, "events"));
        }

        public Task<Result<int>> GetCurrentVersionAsync(TId aggregateId)
            => LoadAsync(aggregateId)
            .ActAsync(buffer => Result.For(buffer.Version));

        public Task<Result<EventBuffer<TId>>> LoadAsync(TId aggregateId)
        {
            var location = Location(aggregateId);
            if (!location.Exists)
            {
                return Task.FromResult(Result.WithMessages<EventBuffer<TId>>(EntityNotFound.ForId(aggregateId)));
            }
            else
            {
                var buffer = EventBuffer<TId>.FromStorage(aggregateId, Load(location), e => e);
                return Task.FromResult(Result.For(buffer));
            }
        }

        public Task<Result> SaveAsync(EventBuffer<TId> buffer)
        {
            Guard.NotNull(buffer, nameof(buffer));
            var location = Location(buffer.AggregateId);

            var events = location.Exists
                ? Load(location)
                : new List<object>();

            events.AddRange(buffer.Uncommitted);

            Store(location, events);
            return Task.FromResult(Result.OK);
        }

        private static List<object> Load(FileInfo location)
        {
            using var reader = location.OpenText();
            return Json.Deserialize<List<object>>(reader.ReadToEnd());
        }

        private static void Store(FileInfo location, List<object> events)
        {
            if (!location.Directory.Exists)
            {
                location.Directory.Create();
            }
            using var writer = new StreamWriter(new FileStream(location.FullName, FileMode.Create, FileAccess.Write));
            Json.Serialize(events, writer, Formatting.Indented);
        }

        private FileInfo Location(TId id) => new FileInfo(Path.Combine(rootStorage.FullName, $"{id}.json"));
    }
}
