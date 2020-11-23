using Qowaiv.Identifiers;
using Rumpelstiltskin.Shared;
using Rumpelstiltskin.Shared.Commands;

namespace Rumpelstiltskin.Domain.Commands
{
    public class CommandBase : ICommand
    {
        public Id<ForNameSelection> Id { get; set; }
    }
}
