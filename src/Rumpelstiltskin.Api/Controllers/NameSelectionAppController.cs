using Microsoft.AspNetCore.Mvc;
using Qowaiv.Identifiers;
using Rumpelstiltskin.Api.Web;
using Rumpelstiltskin.Application.Responses;
using Rumpelstiltskin.Domain;
using Rumpelstiltskin.Shared;
using Rumpelstiltskin.Shared.Handlers;
using System.Threading.Tasks;

namespace Rumpelstiltskin.Api.Controllers
{
    /// <summary>API Controller for applying quering logic on <see cref="NameSelection"/>.</summary>
    [ApiController]
    [Route("api/name-selection")]
    public class NameSelectionAppController
    {
        /// <summary>Initializes a new instance of the <see cref="NameSelectionAppController"/> class.</summary>
        public NameSelectionAppController(RequestProcessor processor)
        {
            this.processor = Guard.NotNull(processor, nameof(processor));
        }

        private readonly RequestProcessor processor;

        /// <summary>Queries the <see cref="NameSelectionView"/>.</summary>
        /// <param name="id">
        /// The identifier of the name selection.
        /// </param>
        [HttpGet("{id}")]
        [Produces(typeof(NameSelectionView))]
        public Task<IActionResult> Get(Id<ForNameSelection> id)
        {
            return ApiAction.GetAsync(processor.SendAsync<Id<ForNameSelection>, NameSelectionView>(id));
        }
    }
}
