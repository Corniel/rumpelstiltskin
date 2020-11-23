using Qowaiv.DomainModel;
using Qowaiv.Validation.Abstractions;
using Rumpelstiltskin.Application.Responses;
using Rumpelstiltskin.Domain;
using Rumpelstiltskin.Domain.Validation;
using Rumpelstiltskin.Shared.Handlers;
using Rumpelstiltskin.Shared.Storage;
using Rumpelstiltskin.Shared.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NameSelectionId = Qowaiv.Identifiers.Id<Rumpelstiltskin.Shared.ForNameSelection>;

namespace Rumpelstiltskin.Application.Handlers
{
    public class NameSelectionViewHandler : IRequestHandler<NameSelectionId, NameSelectionView>
    {
        public NameSelectionViewHandler(IEventStore<NameSelectionId> store)
        {
            this.store = Guard.NotNull(store, nameof(store));
        }

        private readonly IEventStore<NameSelectionId> store;
        private readonly Random rnd = new Random();

        public async Task<Result<NameSelectionView>> HandleAsync(NameSelectionId request)
        {
            var version = await store.GetCurrentVersionAsync(request);

            if (!version.IsValid)
            {
                return Result.WithMessages<NameSelectionView>(version.Messages);
            }

            var buffer = (await store.LoadAsync(request)).Value;
            var selection = AggregateRoot.FromStorage<NameSelection, NameSelectionId>(buffer);

            var candidates = selection.Names.Active().ToArray();

            var noVotes = candidates.Where(c => c.Votes == 0).ToArray();
            var voted = candidates.Where(c => c.Votes != 0).ToArray();

            string name1 = null;
            string name2 = null;

            if (noVotes.Length == 1)
            {
                name1 = noVotes[0].Name;
                name2 = voted[rnd.Next(voted.Length)].Name;
            }
            else if (noVotes.Length == 0)
            {
                voted = SelectTwo(voted);
                name1 = voted[0].Name;
                name2 = voted[1].Name;
            }
            else
            {
                noVotes = SelectTwo(noVotes);
                name1 = noVotes[0].Name;
                name2 = noVotes[1].Name;
            }

            return new NameSelectionView
            {
                Name1 = name1,
                Name2 = name2,
                Top = candidates
                    .Where(c => c.Votes != 0)
                    .Select(c => new NameSelectionView.Name { Label = c.Name, Elo = c.Elo })
                    .OrderByDescending(name => name.Elo)
                    .Take(CandidateNames.MinimumSelection)
                    .ToArray(),
                Candidates = candidates.Length,
            };
        }

        private CandidateName[] SelectTwo(IEnumerable<CandidateName> names) => names.OrderBy(i => rnd.Next()).Take(2).ToArray();
    }
}
