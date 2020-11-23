using Qowaiv.Statistics;

namespace Rumpelstiltskin.Application.Responses
{
    public class NameSelectionView
    {
        public string Name1 { get; internal set; }
        public string Name2 { get; internal set; }
        public Name[] Top { get; set; }

        public int Candidates { get; set; }

        public class Name
        {
            public string Label { get; set; }
            public Elo Elo { get; set; }
        }
    }
}
