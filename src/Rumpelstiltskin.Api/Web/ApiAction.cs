using Microsoft.AspNetCore.Mvc;
using Qowaiv.Validation.Abstractions;
using Rumpelstiltskin.Shared.Validation;
using System.Linq;
using System.Threading.Tasks;

namespace Rumpelstiltskin.Api.Web
{
    /// <summary>API action handler.</summary>
    public static class ApiAction
    {
        /// <summary>Creates a HTTP GET response.</summary>
        /// <param name="promise">
        /// The result potentially containing error messages).
        /// </param>
        /// <typeparam name="TResponse">
        /// The model to return.
        /// </typeparam>
        public static async Task<IActionResult> GetAsync<TResponse>(Task<Result<TResponse>> promise)
        {
            _ = Guard.NotNull(promise, nameof(promise));

            var result = await promise;

            if (result.IsValid)
            {
                return new OkObjectResult(ApiResponse.From(result));
            }
            if (result.Messages.Any(m => m is EntityNotFound))
            {
                return new NotFoundResult();
            }
            return new BadRequestObjectResult(ApiResponse.From(result));
        }

        /// <summary>Creates a HTTP POST response.</summary>
        /// <param name="promise">
        /// The result potentially containing error messages).
        /// </param>
        /// <param name="location">
        /// The location to navigate to.
        /// </param>
        /// <param name="id">
        /// The identifier of newly generated entity.
        /// </param>
        public static async Task<IActionResult> PostAsync(Task<Result> promise, string location, object id)
        {
            _ = Guard.NotNull(promise, nameof(promise));
            var result = await promise;

            if (result.IsValid)
            {
                return new CreatedResult(location, id);
            }
            return new BadRequestObjectResult(ApiResponse.From(result));
        }

        /// <summary>Creates a HTTP PUT response.</summary>
        /// <param name="promise">
        /// The result potentially containing error messages).
        /// </param>
        public static async Task<IActionResult> PutAsync(Task<Result> promise)
        {
            _ = Guard.NotNull(promise, nameof(promise));
            var result = await promise;

            if (result.IsValid)
            {
                if (result.Messages.Any())
                {
                    return new OkObjectResult(ApiResponse.From(result));
                }
                return new NoContentResult();
            }
            if (result.Messages.Any(m => m is EntityNotFound))
            {
                return new NotFoundResult();
            }
            return new BadRequestObjectResult(ApiResponse.From(result));
        }

        /// <summary>Creates a HTTP DELETE response.</summary>
        /// <param name="promise">
        /// The result potentially containing error messages).
        /// </param>
        public static async Task<IActionResult> DeleteAsync(Task<Result> promise)
        {
            _ = Guard.NotNull(promise, nameof(promise));
            var result = await promise;
            if (result.IsValid)
            {
                return new NoContentResult();
            }
            return new BadRequestObjectResult(ApiResponse.From(result));
        }
    }
}
