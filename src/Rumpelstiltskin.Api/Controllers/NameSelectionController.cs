using Microsoft.AspNetCore.Mvc;
using Rumpelstiltskin.Api.Web;
using Rumpelstiltskin.Domain;
using Rumpelstiltskin.Domain.Commands;
using Rumpelstiltskin.Shared.Handlers;
using System.Threading.Tasks;
using NameSelectionId = Qowaiv.Identifiers.Id<Rumpelstiltskin.Shared.ForNameSelection>;

namespace Rumpelstiltskin.Api.Controllers
{
    /// <summary>API Controller for applying domain logic on <see cref="NameSelection"/>.</summary>
    [ApiController]
    [Route("api/name-selection")]
    public class NameSelectionController
    {
        /// <summary>Initializes a new instance of the <see cref="NameSelectionController"/> class.</summary>
        public NameSelectionController(CommandProcessor processor)
            => this.processor = Guard.NotNull(processor, nameof(processor));

        private readonly CommandProcessor processor;

        /// <summary>Creates a name selection.</summary>
        [HttpPost("new")]
        [Produces(typeof(ApiResponse))]
        public Task<IActionResult> New()
        {
            var cmd = new NewNameSelection { Id = NameSelectionId.Next() };
            return ApiAction.PostAsync(processor.SendAsync(cmd), "api/name-selection", cmd.Id);
        }

        /// <summary>Compares two candidate names.</summary>
        [HttpPut("compare")]
        [Produces(typeof(ApiResponse))]
        public Task<IActionResult> Compare(Compare cmd)
            => ApiAction.PutAsync(processor.SendAsync(cmd));
    }
}
