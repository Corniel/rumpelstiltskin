using Qowaiv.Validation.Abstractions;
using Rumpelstiltskin.Domain;
using EventBuffer = Qowaiv.DomainModel.EventBuffer<Qowaiv.Identifiers.Id<Rumpelstiltskin.Shared.ForNameSelection>>;

namespace Rumpelstiltskin.UnitTests.TestTools
{
    internal sealed class TestResult
    {
        public TestResult(Result<NameSelection> result, EventBuffer events)
        {
            Result = result;
            Events = events;
        }

        public EventBuffer Events { get; }
        public NameSelection Selection => Result.Value;
        public Result<NameSelection> Result { get; }

        public static implicit operator NameSelection(TestResult result) => result?.Selection;
    }
}
