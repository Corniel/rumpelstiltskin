using Qowaiv.Validation.Abstractions;
using Rumpelstiltskin.Shared.Validation;
using System.Globalization;
using System.Linq;

namespace Rumpelstiltskin.Domain
{
    public partial class NameSelection
    {
        private Result<NameSelection> MustBeKnown(string name, string propertyName)
            => Must.Be(
            model: this,
            condition: Names.Contains(name),
            propertyName: propertyName,
            message: Message.CandidateNotFound);

        private Result<NameSelection> MustBeDifferent(string name1, string name2)
            => Must.NotBe(
            model: this,
            condition: string.CompareOrdinal(name1, name2) == 0,
            propertyName: null,
            message: Message.DifferentNames);


        private Result<NameSelection> ShouldBeDistinct(string[] names)
        {
            var doubles = names.Where(name => Names.Contains(name)).ToArray();

            if (doubles.Length == names.Length)
            {
                return Result.WithMessages<NameSelection>(ValidationMessage.Error(Message.NoNewNames));
            }
            return doubles.Any()
                ? Result.For(this, ValidationMessage.Warn(string.Format(
                    CultureInfo.CurrentCulture,
                    Message.DoubleNames,
                    string.Join(", ", doubles.OrderBy(n => n)))))
                : Result.For(this);
        }
    }
}
