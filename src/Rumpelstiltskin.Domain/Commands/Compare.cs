namespace Rumpelstiltskin.Domain.Commands
{
    public class Compare : CommandBase
    {
        public string Name1 { get; set; }
        public string Name2 { get; set; }
        public NameComparison Comparison { get; set; }
    }
}
