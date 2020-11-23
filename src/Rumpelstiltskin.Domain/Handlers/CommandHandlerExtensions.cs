using Qowaiv.Identifiers;
using Qowaiv.Validation.Abstractions;
using Rumpelstiltskin.Shared;
using Rumpelstiltskin.Shared.Storage;
using System.Threading.Tasks;

namespace Rumpelstiltskin.Domain.Handlers
{
    internal static class CommandHandlerExtensions
    {
        public static async Task<Result> StoreAsync(
            this Task<Result<NameSelection>> result,
            IEventStore<Id<ForNameSelection>> store)
        {
            var stored = result.ActAsync(selection => store.SaveAsync(selection.Buffer));
            return await stored;
        }
    }
}
