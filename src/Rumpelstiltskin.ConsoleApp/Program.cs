using Microsoft.Extensions.DependencyInjection;
using Qowaiv.Validation.Abstractions;
using Rumpelstiltskin.Application.Responses;
using Rumpelstiltskin.Domain;
using Rumpelstiltskin.Domain.Commands;
using Rumpelstiltskin.Shared.Handlers;
using Rumpelstiltskin.Shared.Validation;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NameSelectionId = Qowaiv.Identifiers.Id<Rumpelstiltskin.Shared.ForNameSelection>;

namespace Rumpelstiltskin.ConsoleApp
{
    internal class Program
    {
        public static Task Main(string[] args)
        {
            var provider = Startup.BuildProvider();
            var program = provider.GetService<Program>();

            NameSelectionId.TryParse(args?.FirstOrDefault(), out var id);
            if (id == default)
            {
                id = NameSelectionId.Next();
                Console.WriteLine(id);
            }
            return program.RunAsync(id);
        }

        public Program(
            CommandProcessor commandProcessor,
            RequestProcessor requestProcessor,
            DirectoryInfo rootDirectory)
        {
            this.commandProcessor = commandProcessor;
            this.requestProcessor = requestProcessor;
            this.rootDirectory = rootDirectory;
        }

        private readonly CommandProcessor commandProcessor;
        private readonly RequestProcessor requestProcessor;
        private readonly DirectoryInfo rootDirectory;

        private NameSelectionId Id { get; set; }

        private NameSelectionView Context { get; set; }

        public async Task RunAsync(NameSelectionId id)
        {
            Id = id;

            await SelectNames();

            while (true)
            {
                await GetCompare();
                ShowVoteScreen();

                var result = await ReadKeyAsync();
                ShowResult(result);
            }
        }

        private async Task SelectNames()
        {
            var view = await requestProcessor.SendAsync<NameSelectionId, NameSelectionView>(Id);

            if (!view.Messages.OfType<EntityNotFound>().Any())
            {
                return;
            }

            var namesDirectory = new DirectoryInfo(Path.Combine(rootDirectory.FullName, "names"));
            var namesFiles = namesDirectory.GetFiles("*.txt");

            var i = 1;
            foreach (var name in namesFiles)
            {
                Console.WriteLine($"[{i++}] {name.Name}.txt");
            }
            var index = 0;
            Console.WriteLine();
            Console.WriteLine("Select file: ");
            while (index < 1 || index > namesFiles.Length)
            {
                int.TryParse(Console.ReadLine(), out index);
            }

            var file = namesFiles[index - 1];

            using var stream = file.OpenText();

            var str = await stream.ReadToEndAsync();
            var names = str
                .Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(name => name.Trim())
                .ToArray();

            await commandProcessor.SendAsync(new NewNameSelection { Id = Id, Names = names });
        }

        private Task<Result> ReadKeyAsync()
        {
            while (true)
            {
                var key = ReadKey();

                switch (key)
                {
                    case ConsoleKey.F1:
                        return SetFirstLetterAsync();

                    case ConsoleKey.F2:
                        return SetNameLengthAsync();

                    case ConsoleKey.LeftArrow:
                    case ConsoleKey.RightArrow:
                    case ConsoleKey.UpArrow:
                    case ConsoleKey.OemPeriod:
                    case ConsoleKey.DownArrow:
                    case ConsoleKey.X:
                    default:
                        return CompareAsync(key);
                }
            }
        }

        private Task<Result> SetNameLengthAsync()
        {
            Console.Clear();
            Console.Write("Set min length: ");
            int.TryParse(Console.ReadLine(), out int min);
            Console.Write("Set max length: ");
            int.TryParse(Console.ReadLine(), out int max);

            return commandProcessor.SendAsync(new SetNameLength { Id = Id, MinLength = min, MaxLength = max });
        }

        public Task<Result> SetFirstLetterAsync()
        {
            Console.Clear();
            Console.WriteLine("Set forbidden first letters:");
            var forbidden = Console.ReadLine();
            var allowed = Letters.Allowed(forbidden);

            return commandProcessor.SendAsync(new SetFirstLetter { Id = Id, Allowed = allowed });
        }

        private async Task<Result> CompareAsync(ConsoleKey key)
        {
            var vote = new Compare { Name1 = Context.Name1, Name2 = Context.Name2, Id = Id };

            switch (key)
            {
                case ConsoleKey.LeftArrow:
                    vote.Comparison = NameComparison.Name1Wins;
                    break;

                case ConsoleKey.RightArrow:
                    vote.Comparison = NameComparison.Name2Wins;
                    break;

                case ConsoleKey.UpArrow:
                    vote.Comparison = NameComparison.Draw;
                    break;

                case ConsoleKey.OemPeriod:
                case ConsoleKey.DownArrow:
                    var result = HandleVeto();
                    if (!result.HasValue)
                    {
                        return Result.OK;
                    }
                    vote.Comparison = result.GetValueOrDefault();
                    break;

                case ConsoleKey.X:
                    return Result.OK;

                default:
                    return await ReadKeyAsync();
            }

            return await commandProcessor.SendAsync(vote);
        }

        private static NameComparison? HandleVeto()
        {
            ShowVetoScreen();

            switch (ReadKey())
            {
                case ConsoleKey.X: return default;
                case ConsoleKey.LeftArrow: return NameComparison.VetoName1;
                case ConsoleKey.RightArrow: return NameComparison.VetoName2;
                default: return NameComparison.VetoBoth;
            }
        }

        private async Task<Result> GetCompare()
        {
            var result = await requestProcessor.SendAsync<NameSelectionId, NameSelectionView>(Id);
            Context = result.Value;
            return result;
        }

        private void ShowVoteScreen()
        {
            Console.Clear();
            Console.WriteLine(new string('=', 36));

            for (var i = 0; i < Context.Top.Length; i++)
            {
                var candidated = Context.Top[i];

                Console.WriteLine($"{i + 1,4}  {candidated.Label} ({candidated.Elo:0})");
            }
            if (Context.Candidates > Context.Top.Length)
            {
                Console.WriteLine("...");
                Console.WriteLine($"Total {Context.Candidates}");
            }
            Console.WriteLine(new string('-', 36));
            Console.WriteLine($"{Context.Name1} - {Context.Name2}");
            Console.WriteLine("Left [<]  Right[>]  Draw[^]  Veto[.]  Skip[x]");
        }

        private static void ShowVetoScreen()
        {
            Console.WriteLine("Veto:");
            Console.WriteLine("Left [<]  Right[>]  Both[^]  Skip[x]");
        }

        private static void ShowResult(Result result)
        {
            Console.WriteLine();
            foreach (var message in result.Messages)
            {
                switch (message.Severity)
                {
                    case ValidationSeverity.Error:
                        Console.ForegroundColor = ConsoleColor.Red;
                        break;

                    case ValidationSeverity.Warning:
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        break;

                    default:
                        Console.ForegroundColor = ConsoleColor.Gray;
                        break;

                }
                Console.WriteLine(message);
            }
            if (result.Messages.Any())
            {
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.ReadLine();
            }
        }

        private static ConsoleKey ReadKey()
        {
            return Console.ReadKey().Key;
        }
    }
}
