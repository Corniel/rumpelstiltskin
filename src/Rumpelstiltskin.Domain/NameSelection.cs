using Qowaiv.DomainModel;
using Qowaiv.Identifiers;
using Qowaiv.Statistics;
using Qowaiv.Validation.Abstractions;
using Rumpelstiltskin.Domain.Events;
using Rumpelstiltskin.Domain.Filters;
using Rumpelstiltskin.Domain.Validation;
using Rumpelstiltskin.Shared;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rumpelstiltskin.Domain
{
    public partial class NameSelection : AggregateRoot<NameSelection, Id<ForNameSelection>>
    {
        public static readonly double K = 30;

        public NameSelection() : this(Id<ForNameSelection>.Next()) { }

        public NameSelection(Id<ForNameSelection> id) : base(id, new NameSelectionValidator())
        {
            Filters = new IFilter[]
            {
                FirstLetter,
                NameLength,
            };
            Names = new CandidateNameCollection(Filters);
        }

        public DateTime CreatedUtc { get; private set; }

        public CandidateNameCollection Names { get; }

        public IReadOnlyCollection<IFilter> Filters { get; }

        public FirstLetterFilter FirstLetter { get; } = new FirstLetterFilter();

        public NameLengthFilter NameLength { get; } = new NameLengthFilter();

        public Result<NameSelection> AddNames(params string[] names)
            => ShouldBeDistinct(names ?? Array.Empty<string>())
            | (m => names.Length == 1
                ? m.ApplyEvent(new NameAdded { Name = names[0] })
                : m.ApplyEvent(new NamesAdded { Names = names.ToArray() }));

        public Result<NameSelection> FilterOnFirstLetter(string allowed)
            => ApplyEvent(new FirstLetterSet
            {
                Allowed = Letters.Normalize(allowed),
            });

        public Result<NameSelection> FilterOnNameLength(int minLength, int maxLength)
            => ApplyEvent(new NameLengthSet
            {
                MinLength = minLength,
                MaxLength = maxLength,
            });

        public Result<NameSelection> Compare(string name1, string name2, NameComparison comparison)
            => MustBeKnown(name1, nameof(name1))
            | (m => m.MustBeKnown(name2, nameof(name2)))
            | (m => m.MustBeDifferent(name1, name2))
            | (m => comparison switch
            {
                NameComparison.Name1Wins => m.ApplyEvent(new Won { Winner = name1, Loser = name2 }),
                NameComparison.Name2Wins => m.ApplyEvent(new Won { Winner = name2, Loser = name1 }),
                NameComparison.VetoName1 => m.ApplyEvents(new Won { Winner = name2, Loser = name1 }, new Vetoed { Name = name1 }),
                NameComparison.VetoName2 => m.ApplyEvents(new Won { Winner = name1, Loser = name2 }, new Vetoed { Name = name2 }),
                NameComparison.Draw => m.ApplyEvent(new Drew { Name1 = name1, Name2 = name2 }),
                _ => m.ApplyEvents(new Vetoed { Name = name1 }, new Vetoed { Name = name2 }),
            });

        internal void When(FirstLetterSet e)
        {
            FirstLetter.Allowed = e.Allowed;
        }

        internal void When(NameLengthSet e)
        {
            NameLength.MinLength = e.MinLength;
            NameLength.MaxLength = e.MaxLength;
        }

        internal void When(NameAdded e)
            => Names.Add(e.Name);

        internal void When(NamesAdded e)
            => Names.AddRange(e.Names);

        internal void When(Won e)
            => Vote(e.Winner, e.Loser, Score.Win);

        internal void When(Drew e)
            => Vote(e.Name1, e.Name2, Score.Draw);

        internal void When(Vetoed e)
        {
            var vetoed = Names.GetCandidate(e.Name);
            vetoed.HasVeto = true;
        }

        internal void When(Created e)
            => CreatedUtc = e.CreatedUtc;

        private void Vote(string name1, string name2, double score)
        {
            var c1 = Names.GetCandidate(name1);
            var c2 = Names.GetCandidate(name2);
            var z = Elo.GetZScore(c1.Elo, c2.Elo);

            var delta = (score - z) * K;

            c1.Elo += delta;
            c2.Elo -= delta;

            c1.Votes++;
            c2.Votes++;
        }

        public static NameSelection Create(Id<ForNameSelection> id, string[] names)
        {
            var selection = new NameSelection(id);
            if (names is null || !names.Any())
            {
                selection.Buffer.Add(new Created());
            }
            else
            {
                selection.ApplyEvents(new Created(), new NamesAdded { Names = names?.ToArray() });
            }
            return selection;
        }
    }
}
