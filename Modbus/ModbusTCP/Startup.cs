// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Startup.cs" company="DTV-Online">
//   Copyright(c) 2020 Dr. Peter Trimmel. All rights reserved.
// </copyright>
// <license>
//   Licensed under the MIT license. See the LICENSE file in the project root for more information.
// </license>
// <created>13-5-2020 13:52</created>
// <author>Peter Trimmel</author>
// --------------------------------------------------------------------------------------------------------------------
namespace ModbusTCP
{
    #region Using Directives

    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.OpenApi.Models;

    using ModbusLib;
    using ModbusTCP.Models;
    using ModbusLib.Models;
    using UtilityLib;

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

            // Add a singleton service using the application settings implementing ITcpClientSettings.
            services.AddSingleton((ITcpClientSettings)Configuration.GetSection("AppSettings").Get<AppSettings>());
            services.AddSingleton<ITcpModbusClient, TcpModbusClient>();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Modbus TCP Gateway API",
                    Description = "This is a web gateway service to access Modbus TCP slave devices.",
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
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Modbus TCP Gateway API V1");
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}