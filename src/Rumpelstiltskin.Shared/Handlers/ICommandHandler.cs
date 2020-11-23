using Qowaiv.Validation.Abstractions;
using Rumpelstiltskin.Shared.Commands;
using System.Threading.Tasks;

namespace Rumpelstiltskin.Shared.Handlers
{
    /// <summary>Generic command handler.</summary>
    /// <typeparam name="TCommand">
    /// The type of the command.
    /// </typeparam>
    public interface ICommandHandler<TCommand> where TCommand : class, ICommand
    {
        /// <summary>Handles a command asynchronously.</summary>
        /// <param name="command">
        /// The command to sent.
        /// </param>
        /// <returns>
        /// The response result.
        /// </returns>
        Task<Result> HandleAsync(TCommand command);
    }
}
