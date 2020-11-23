using Qowaiv;
using System;

namespace Rumpelstiltskin.Domain.Events
{
    public class Created
    {
        public DateTime CreatedUtc { get; set; } = Clock.UtcNow();
    }
}
