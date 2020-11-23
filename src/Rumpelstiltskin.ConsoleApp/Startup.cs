#pragma warning disable S1200 // Classes should not be coupled to too many other classes (Single Responsibility Principle)
// The whole purpose of this startup is wire up all these dependencies.
using FileSystemEventStore;
using Microsoft.Extensions.DependencyInjection;
using Rumpelstiltskin.Application.Handlers;
using Rumpelstiltskin.Application.Responses;
using Rumpelstiltskin.Domain;
using Rumpelstiltskin.Domain.Commands;
using Rumpelstiltskin.Domain.Events;
using Rumpelstiltskin.Domain.Handlers;
using Rumpelstiltskin.Shared.Handlers;
using Rumpelstiltskin.Shared.Refelection;
using Rumpelstiltskin.Shared.Storage;
using System;
using System.IO;
using NameSelectionId = Qowaiv.Identifiers.Id<Rumpelstiltskin.Shared.ForNameSelection>;

namespace Rumpelstiltskin.ConsoleApp
{
    internal static class Startup
    {
        public static IServiceProvider BuildProvider()
        {
            var provider = new ServiceCollection()
                .AddSingleton<CommandProcessor>()
                .AddSingleton<RequestProcessor>()
                .AddSingleton(GetTypeMap)
                .AddSingleton(GetStorageDirectory)
                .AddSingleton<IEventStore<NameSelectionId>, FileBasedEventStore<NameSelectionId>>()

                .AddCommandHandler<NewNameSelection, CommandHandler>()
                .AddCommandHandler<AddNames, CommandHandler>()
                .AddCommandHandler<Compare, CommandHandler>()
                .AddCommandHandler<SetFirstLetter, CommandHandler>()
                .AddCommandHandler<SetNameLength, CommandHandler>()

                .AddRequestHandler<NameSelectionId, NameSelectionView, NameSelectionViewHandler>()

                .AddSingleton<Program>()

                .BuildServiceProvider();

            return provider;
        }

        private static DirectoryInfo GetStorageDirectory(IServiceProvider provider)
        {
            var dir = new DirectoryInfo("./storage");
            if (!dir.Exists)
            {
                dir.Create();
            }
            return dir;
        }

        private static TypeMap GetTypeMap(IServiceProvider provider)
        {
            return new TypeMap()
                .Add(typeof(Drew), nameof(Drew))
                .Add(typeof(Won), nameof(Won))
                .Add(typeof(Vetoed), nameof(Vetoed))
                .Add(typeof(Created), nameof(Created))
                .Add(typeof(NamesAdded), nameof(NamesAdded))
                .Add(typeof(NameSelection).Assembly)
            ;
        }
    }
}
