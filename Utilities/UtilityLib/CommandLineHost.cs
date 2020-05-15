// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CommandLineHost.cs" company="DTV-Online">
//   Copyright(c) 2020 Dr. Peter Trimmel. All rights reserved.
// </copyright>
// <license>
//   Licensed under the MIT license. See the LICENSE file in the project root for more information.
// </license>
// <created>13-5-2020 13:53</created>
// <author>Peter Trimmel</author>
// --------------------------------------------------------------------------------------------------------------------
namespace UtilityLib
{
    #region Using Directives

    using System.IO;
    using System.Reflection;
    using System.Runtime.InteropServices;

    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.EventLog;

    using Serilog;
    using Serilog.Core;
    using Serilog.Events;
    using Serilog.Sinks.SystemConsole.Themes;

    #endregion Using Directives

    /// <summary>
    ///  Provides convenience methods for creating instances of <see cref="IHostBuilder"/> with pre-configured defaults.
    ///  See also the implementation at https://github.com/dotnet/runtime/blob/master/src/libraries/Microsoft.Extensions.Hosting/src/Host.cs.
    /// </summary>
    public static class CommandLineHost
    {
        public static LoggingLevelSwitch ConsoleSwitch { get; } =
            new LoggingLevelSwitch() { MinimumLevel = LogEventLevel.Information };

        public static LoggingLevelSwitch FileSwitch { get; } =
            new LoggingLevelSwitch() { MinimumLevel = LogEventLevel.Information };

        /// <summary>
        ///  Initializes a new instance of the <see cref="HostBuilder"/> class with pre-configured defaults.
        /// </summary>
        /// <returns></returns>
        public static IHostBuilder CreateDefaultBuilder()
        {
            var builder = new HostBuilder();

            builder.UseContentRoot(Directory.GetCurrentDirectory());
            builder.ConfigureHostConfiguration(config =>
            {
                config.AddEnvironmentVariables(prefix: "ASPNETCORE_");
            });

            builder.ConfigureAppConfiguration((hostingContext, config) =>
            {
                var env = hostingContext.HostingEnvironment;

                config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                      .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);

                if (env.IsDevelopment() && !string.IsNullOrEmpty(env.ApplicationName))
                {
                    var appAssembly = Assembly.Load(new AssemblyName(env.ApplicationName));
                    if (appAssembly != null)
                    {
                        config.AddUserSecrets(appAssembly, optional: true);
                    }
                }

                config.AddEnvironmentVariables();
            })
            .ConfigureLogging((context, logging) =>
            {
                var isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

                // IMPORTANT: This needs to be added *before* configuration is loaded, this lets
                // the defaults be overridden by the configuration.
                if (isWindows)
                {
                    // Default the EventLogLoggerProvider to warning or above
                    logging.AddFilter<EventLogLoggerProvider>(level => level >= LogLevel.Warning);
                    // Add the EventLogLoggerProvider on windows machines
                    logging.AddEventLog();
                }
            })
            .UseSerilog((context, logger) =>
            {
                ConsoleSwitch.MinimumLevel = context.Configuration.GetValue<LogEventLevel>("Serilog:LevelSwitches:$consoleSwitch");
                FileSwitch.MinimumLevel = context.Configuration.GetValue<LogEventLevel>("Serilog:LevelSwitches:$fileSwitch");

                logger.ReadFrom.Configuration(context.Configuration)
                      .Enrich.FromLogContext()
                      .WriteTo.File(
                          "Logs/log-.log",
                          levelSwitch: FileSwitch,
                          rollingInterval: RollingInterval.Day,
                          outputTemplate: "{Timestamp: HH:mm:ss.fff zzz} {SourceContext} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
                      .WriteTo.Console(
                          levelSwitch: ConsoleSwitch,
                          theme: AnsiConsoleTheme.Code,
                          outputTemplate: "{Timestamp: HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}");
            })
            .UseDefaultServiceProvider((context, options) =>
            {
                var isDevelopment = context.HostingEnvironment.IsDevelopment();
                options.ValidateScopes = isDevelopment;
                options.ValidateOnBuild = isDevelopment;
            });

            return builder;
        }
    }
}