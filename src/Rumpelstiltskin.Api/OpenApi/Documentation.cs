using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace Rumpelstiltskin.Api.OpenApi
{
    internal static class Documentation
    {
        /// <summary>Defines how to use Swagger Open API.</summary>
        public static void Swagger(SwaggerOptions options) => options.RouteTemplate = "api-docs/{documentName}/spec.json";

        /// <summary>Configures Swagger Open API UI.</summary>
        public static void SwaggerUI(SwaggerUIOptions options)
        {
            options.SwaggerEndpoint($"./v1/spec.json", $"TJIP Rumpelstiltskin API Documentation (v1)");
            options.RoutePrefix = "api-docs";
        }

        /// <summary>Configures the Open API documentation generation.</summary>
        public static void Generation(SwaggerGenOptions options)
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "v1",
                Title = "TJIP Rumpelstiltskin API",
                Description = "Name selection application",
            });

            // We want all actions ordered alphabetically by there relative path and grouped by their tag name
            // instead of their controller name. Therefor we need to EnableAnnotations and use the Tags property on
            // the SwaggerOperationAttribute.
            options.EnableAnnotations();
            options.OrderActionsBy(descr => $"{descr.GroupName}-{descr.RelativePath}");

            // When using this Startup for integration tests, the name of the assembly will change (to TestHost)
            // Therefor we are hard coding the name of the generated XML comments file to make sure that the original reference
            // to the assembly name will not fail:
            //  {AppContext.BaseDirectory}{Path.DirectorySeparatorChar}{Assembly.GetEntryAssembly().GetName().Name}.xml
            options.IncludeXmlComments($"Rumpelstiltskin.Api.xml");
        }
    }
}
