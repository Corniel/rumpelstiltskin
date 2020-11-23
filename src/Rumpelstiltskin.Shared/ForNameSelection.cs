using Qowaiv;
using Qowaiv.Identifiers;

namespace Rumpelstiltskin.Shared
{
    public sealed class ForNameSelection : StringIdBehavior
    {
        public override object Next()
            => Uuid.NewUuid().ToString();
    }
}
