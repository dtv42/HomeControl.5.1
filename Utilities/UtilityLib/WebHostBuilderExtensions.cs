// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WebHostBuilderExtensions.cs" company="DTV-Online">
//   Copyright(c) 2020 Dr. Peter Trimmel. All rights reserved.
// </copyright>
// <license>
//   Licensed under the MIT license. See the LICENSE file in the project root for more information.
// </license>
// <created>19-4-2020 10:59</created>
// <author>Peter Trimmel</author>
// --------------------------------------------------------------------------------------------------------------------
namespace UtilityLib
{
    #region Using Directives

    using System;

    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    using Serilog;
    using Serilog.Core;
    using Serilog.Events;
    using Serilog.Sinks.SystemConsole.Themes;

    #endregion Using Directives

    /// <summary>
    ///  Extensions for standard appsettings setup and Serilog logging support.
    /// </summary>
    public static class WebHostBuilderExtensions
    {
        public static IWebHostBuilder ConfigureBaseHost<TSettings>(this IWebHostBuilder builder) where TSettings : class, new()
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));

            builder.ConfigureServices((context, services) =>
            {
                services.AddSingleton(context.Configuration.GetSection("AppSettings").Get<TSettings>().ValidateAndThrow());
            })
            .UseSerilog((context, logger) =>
            {
                var consoleSwitch = new LoggingLevelSwitch() { MinimumLevel = LogEventLevel.Information };
                var fileSwitch = new LoggingLevelSwitch() { MinimumLevel = LogEventLevel.Information };
                consoleSwitch.MinimumLevel = context.Configuration.GetValue<LogEventLevel>("Serilog:LevelSwitches:$consoleSwitch");
                fileSwitch.MinimumLevel = context.Configuration.GetValue<LogEventLevel>("Serilog:LevelSwitches:$fileSwitch");

                logger.ReadFrom.Configuration(context.Configuration)
                      .WriteTo.File(
                          "Logs/log-.log",
                          levelSwitch: fileSwitch,
                          rollingInterval: RollingInterval.Day,
                          outputTemplate: "{Timestamp: HH:mm:ss.fff zzz} {SourceContext} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
                      .WriteTo.Console(
                          levelSwitch: consoleSwitch,
                          theme: AnsiConsoleTheme.Code,
                          outputTemplate: "{Timestamp: HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}");
            });

            return builder;
        }
    }
}