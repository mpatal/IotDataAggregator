using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AggregatorService.Repository;
using AggregatorService.ServiceClients;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Serialization;
using Swashbuckle.AspNetCore.Swagger;

namespace AggregatorService
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .AddEnvironmentVariables();
            Configuration = builder.Build();

            this.hostingEnvironment = env;
        }

        public IConfigurationRoot Configuration { get; }

        protected IHostingEnvironment hostingEnvironment;

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvc()
                .AddJsonOptions(
                     opts =>
                     {
                         opts.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                     });

            services.AddSwaggerGen(c =>
             {
                 c.SwaggerDoc("v1", new Info { Title = "API V1", Version = "v1" });
             });

             // Add the store as a singleton
             services.AddSingleton<IStore>(new Store());

             // Add the historian client
             services.AddTransient<IHistorianService>( 
                 (s) => new HistorianService(new Uri(Configuration["TEMPHISTORIAN"])));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseMvc();

            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "V1 Docs");
            });
        }
    }
}
