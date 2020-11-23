using Rumpelstiltskin.Domain.Filters;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Rumpelstiltskin.Domain
{
    public class CandidateNameCollection : IReadOnlyCollection<CandidateName>
    {
        public CandidateNameCollection(IReadOnlyCollection<IFilter> filters) => Filters = filters;

        public int Count => names.Count;

        protected IReadOnlyCollection<IFilter> Filters { get; }

        /// <summary>Gets all active candidates.</summary>
        public IEnumerable<CandidateName> Active()
            => this.Where(c => !c.HasVeto && Filters.All(filter => filter.Includes(c)));

        /// <summary>Returns true if candidate with the specified name exists.</summary>
        public bool Contains(string name) => names.Contains(new CandidateName(name));

        internal void Add(string name) => names.Add(new CandidateName(name));
        internal void AddRange(string[] names)
        {
            foreach (var name in names)
            {
                Add(name);
            }
        }

        /// <summary>Gets the candidate with the specified name.</summary>
        public CandidateName GetCandidate(string name)
        {
            return this.FirstOrDefault(candidate => candidate.Name == name);
        }

        public IEnumerator<CandidateName> GetEnumerator() => names.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private readonly HashSet<CandidateName> names = new HashSet<CandidateName>(new UniqueNames());

        private class UniqueNames : IEqualityComparer<CandidateName>
        {
            public bool Equals(CandidateName x, CandidateName y) => string.CompareOrdinal(x.Name, y.Name) == 0;

            public int GetHashCode(CandidateName obj)
            {
                var hash = 0;
                foreach (var ch in obj.Name)
                {
                    hash *= 29;
                    hash ^= ch;
                }
                return hash;
            }
        }
    }
}
