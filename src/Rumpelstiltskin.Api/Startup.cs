using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Qowaiv;
using Qowaiv.Json.Newtonsoft;
using Rumpelstiltskin.Api.ModelBinding;
using Rumpelstiltskin.Api.OpenApi;
using Rumpelstiltskin.Application.Handlers;
using Rumpelstiltskin.Application.Responses;
using Rumpelstiltskin.Domain.Commands;
using Rumpelstiltskin.Domain.Handlers;
using Rumpelstiltskin.Shared.Handlers;
using Rumpelstiltskin.Shared.Serialization;
using System.Text.Json;
using NameSelectionId = Qowaiv.Identifiers.Id<Rumpelstiltskin.Shared.ForNameSelection>;

namespace Rumpelstiltskin.Api
{
    /// <summary>Startup class for Rumpelstiltskin API.</summary>
    public class Startup
    {
        /// <summary>Initializes a new instance of the <see cref="Startup"/> class.</summary>
        public Startup(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        private readonly IConfiguration configuration;

        /// <summary>Configures the services.</summary>
        /// <remarks>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </remarks>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services
                .AddMvc(options =>
                {
                    var binder = new TypeConverterModelBinder().AddAssembly(typeof(Uuid).Assembly);
                    options.ModelBinderProviders.Insert(0, binder);
                })
               .AddNewtonsoftJson(options => options.SerializerSettings.Initialize());

            services
                .AddSingleton<CommandProcessor>()
                .AddSingleton<RequestProcessor>();

            services
                .AddCommandHandler<NewNameSelection, CommandHandler>()
                .AddCommandHandler<AddNames, CommandHandler>()
                .AddCommandHandler<Compare, CommandHandler>()
                .AddCommandHandler<SetFirstLetter, CommandHandler>()
                .AddCommandHandler<SetNameLength, CommandHandler>();

            services
                .AddRequestHandler<NameSelectionId, NameSelectionView, NameSelectionViewHandler>();

            services
                .AddSwaggerGen(Documentation.Generation);
        }

        /// <summary>Configures the API.</summary>
        /// <remarks>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </remarks>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app
               .UseSwagger(Documentation.Swagger)
               .UseSwaggerUI(Documentation.SwaggerUI);

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
