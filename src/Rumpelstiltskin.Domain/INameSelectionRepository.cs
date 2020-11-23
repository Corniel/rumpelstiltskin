using System;
using System.Threading.Tasks;

namespace Rumpelstiltskin.Domain
{
    public interface INameSelectionRepository
    {
        Task<NameSelection> GetByIdAsync(Guid id);

        Task SaveAsync(NameSelection selection);
    }
}
