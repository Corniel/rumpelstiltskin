using Qowaiv.DomainModel;
using Qowaiv.Validation.Abstractions;
using Rumpelstiltskin.Domain.Commands;
using Rumpelstiltskin.Shared.Handlers;
using Rumpelstiltskin.Shared.Storage;
using System.Threading.Tasks;
using NameSelectionId = Qowaiv.Identifiers.Id<Rumpelstiltskin.Shared.ForNameSelection>;

namespace Rumpelstiltskin.Domain.Handlers
{
    public class CommandHandler :
        ICommandHandler<NewNameSelection>,
        ICommandHandler<SetFirstLetter>,
        ICommandHandler<SetNameLength>,
        ICommandHandler<AddNames>,
        ICommandHandler<Compare>
    {
        public CommandHandler(IEventStore<NameSelectionId> store)
        {
            this.store = store;
        }
        private readonly IEventStore<NameSelectionId> store;

        public Task<Result> HandleAsync(NewNameSelection command)
        {
            Guard.NotNull(command, nameof(command));
            var selection = NameSelection.Create(command.Id, command.Names);
            return store.SaveAsync(selection.Buffer);
        }

        public Task<Result> HandleAsync(SetFirstLetter command)
         => LoadAsync(command)
            .ActAsync(selection => selection.FilterOnFirstLetter(command.Allowed))
            .StoreAsync(store);

        public Task<Result> HandleAsync(SetNameLength command)
        => LoadAsync(command)
           .ActAsync(selection => selection.FilterOnNameLength(command.MinLength, command.MaxLength))
           .StoreAsync(store);

        public Task<Result> HandleAsync(AddNames command)
            => LoadAsync(command)
            .ActAsync(selection => selection.AddNames(command.Names))
            .StoreAsync(store);

        public Task<Result> HandleAsync(Compare command)
          => LoadAsync(command)
            .ActAsync(selection => selection.Compare(
               name1: command.Name1,
               name2: command.Name2,
               comparison: command.Comparison))
            .StoreAsync(store);

        private Task<Result<NameSelection>> LoadAsync(CommandBase command)
            => store.LoadAsync(command.Id)
            .ActAsync(buffer => Result.For(AggregateRoot.FromStorage<NameSelection, NameSelectionId>(buffer)));
    }
}
