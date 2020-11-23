using Microsoft.Extensions.DependencyInjection;
using Qowaiv.DomainModel;
using Qowaiv.Validation.Abstractions;
using Rumpelstiltskin.Domain;
using Rumpelstiltskin.Domain.Commands;
using Rumpelstiltskin.Domain.Handlers;
using Rumpelstiltskin.Shared.Handlers;
using NameSelectionId = Qowaiv.Identifiers.Id<Rumpelstiltskin.Shared.ForNameSelection>;
using EventBuffer = Qowaiv.DomainModel.EventBuffer<Qowaiv.Identifiers.Id<Rumpelstiltskin.Shared.ForNameSelection>>;
using EventStore = Rumpelstiltskin.Shared.Storage.IEventStore<Qowaiv.Identifiers.Id<Rumpelstiltskin.Shared.ForNameSelection>>;
using Rumpelstiltskin.Domain.Events;
using Rumpelstiltskin.Shared.Storage;

namespace Rumpelstiltskin.UnitTests.TestTools
{
    internal static class Selection
    {
        public static readonly NameSelectionId Id = NameSelectionId.Parse("Test.ID");

        public static EventBuffer Empty()
            => new EventBuffer(Id)
            .Add(new Created());

        public static EventBuffer New()
            => new EventBuffer(Id)
            .Add(new Created())
            .Add(new NamesAdded
            {
                Names = new[]
                {
                    "Anna",
                    "Indi",
                    "Ineke",
                    "Janine",
                    "Joraline",
                    "Laura",
                    "Meike",
                    "Meqiza",
                    "Orianthi",
                    "Rozemarijn",
                }
            });

        public static TestResult Handle<TCommand>(this IServiceCollection services, TCommand command) where TCommand : CommandBase
        {
            var provider = services.BuildServiceProvider();
            var processor = provider.GetService<CommandProcessor>();
            var eventstore = provider.GetService<EventStore>();

            var test = processor.SendAsync(command).Result;

            var buffer = eventstore.LoadAsync(command.Id).Result.Value;

            var result = test.IsValid
                ? Result.For(AggregateRoot.FromStorage<NameSelection, NameSelectionId>(buffer), test.Messages)
                : Result.WithMessages<NameSelection>(test.Messages);

            return new TestResult(result, buffer);
        }

        public static IServiceCollection Setup()
            => new ServiceCollection()
            .AddSingleton<EventStore, InMemoryEventStore<NameSelectionId>>()
            .AddSingleton<CommandProcessor>()
            .AddCommandHandler<NewNameSelection, CommandHandler>()
            .AddCommandHandler<AddNames, CommandHandler>()
            .AddCommandHandler<Compare, CommandHandler>()
            .AddCommandHandler<SetFirstLetter, CommandHandler>()
            .AddCommandHandler<SetNameLength, CommandHandler>();

        public static IServiceCollection WithEvents(this IServiceCollection services, EventBuffer events)
            => services.AddSingleton(events);
    }
}
