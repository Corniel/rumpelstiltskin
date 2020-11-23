namespace Rumpelstiltskin.Domain.Commands
{
    public class SetNameLength : CommandBase
    {
        public int MinLength { get; set; }
        public int MaxLength { get; set; }
    }
}
