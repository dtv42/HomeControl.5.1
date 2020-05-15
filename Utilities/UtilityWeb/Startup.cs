// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Startup.cs" company="DTV-Online">
//   Copyright(c) 2020 Dr. Peter Trimmel. All rights reserved.
// </copyright>
// <license>
//   Licensed under the MIT license. See the LICENSE file in the project root for more information.
// </license>
// <created>13-5-2020 13:54</created>
// <author>Peter Trimmel</author>
// --------------------------------------------------------------------------------------------------------------------
namespace UtilityWeb
{
    #region Using Directives

    using System.Text.Json;
    using System.Text.Json.Serialization;

    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Diagnostics.HealthChecks;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.OpenApi.Models;

    using UtilityLib;
    using UtilityWeb.Models;
    using UtilityWeb.Services;

    #endregion Using Directives

    /// <summary>
    ///  Standard Startup implementation for a rest based web using controllers and Swagger.
    /// </summary>
    public class Startup
    {
        /// <summary>
        ///  Initializes the configuration property.
        /// </summary>
        /// <param name="configuration"></param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        #region Public Properties

        public IConfiguration Configuration { get; }

        #endregion Public Properties

        /// <summary>
        ///  This method gets called by the runtime. This method adds services to the container.
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers().AddJsonOptions(options => options.JsonSerializerOptions.AddDefaultOptions());

            services.AddSingleton(Configuration.GetSection("GatewaySettings").Get<GatewaySettings>().ValidateAndThrow());
            services.AddSingleton(Configuration.GetSection("PingSettings").Get<PingSettings>().ValidateAndThrow());
            services.AddSingleton(Configuration.GetSection("TestData").Get<TestData>().ValidateAndThrow());

            services.AddHttpClient();
            services.AddSingleton<WebGateway>();

            services.AddHostedService<MonitorService>();

            // Adding Healthchecks.
            services.AddHttpContextAccessor();
            services.AddHealthChecks()
                .AddCheck<StatusCheck<WebGateway>>("Status")
                .AddCheck<PingCheck>("Ping");

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Utility Web API",
                    Description = "This is a sample web application.",
                    Version = "v1"
                });
            });
        }

        /// <summary>
        ///  This method gets called by the runtime. This method configures the HTTP request pipeline.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.ApplicationServices.GetService<WebGateway>().Startup();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Utility Web API V1");
            });

            app.UseHealthChecks("/health", new HealthCheckOptions
            {
                Predicate = _ => true,
                ResponseWriter = UIResponseWriter.WriteResponse
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}