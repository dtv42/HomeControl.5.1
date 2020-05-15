// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="DTV-Online">
//   Copyright(c) 2020 Dr. Peter Trimmel. All rights reserved.
// </copyright>
// <license>
//   Licensed under the MIT license. See the LICENSE file in the project root for more information.
// </license>
// <created>13-5-2020 13:53</created>
// <author>Peter Trimmel</author>
// --------------------------------------------------------------------------------------------------------------------
namespace UtilityApp
{
    #region Using Directives

    using System;
    using System.Threading.Tasks;

    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Hosting;

    using UtilityLib;
    using UtilityApp.Models;
    using UtilityApp.Commands;

    #endregion Using Directives

    /// <summary>
    ///  Application class providing the main entry point.
    /// </summary>
    static class Program
    {
        /// <summary>
        ///  Main application entrypoint.
        ///  This example shows the use of additional service configurations.
        /// </summary>
        /// <param name="args">The command line arguments</param>
        /// <returns>The exit code</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public static async Task<int> Main(string[] args)
        {
            try
            {
                return await CommandLineHost.CreateDefaultBuilder()
                    .ConfigureAppConfiguration(config =>
                    {
                        config.AddJsonFile("testdata.json", optional: false, reloadOnChange: false);
                    })
                    .ConfigureServices((context, services) =>
                    {
                        services
                            // Add application specific settings.
                            .AddSingleton(context.Configuration.GetSection("AppSettings").Get<AppSettings>().ValidateAndThrow())
                            .AddSingleton(context.Configuration.GetSection("TestData").Get<Testdata>().ValidateAndThrow())

                            // Add commands (sub commands and root command).
                            .AddSingleton<AsyncCommand>()
                            .AddSingleton<SettingsCommand>()
                            .AddSingleton<TestdataCommand>()
                            .AddSingleton<PropertyCommand>()
                            .AddSingleton<ValidateCommand>()
                            .AddSingleton<LogCommand>()
                            .AddSingleton<AppCommand>()

                            // Add the command line service.
                            .AddSingleton<ICommandLineService, CommandLineService<AppCommand>>()
                            ;
                    })
                    .Build()
                    .RunCommandLineAsync(args)
                    ;
            }
            catch (Exception ex)
            {
                ex.WriteToConsole();
                return ExitCodes.UnhandledException;
            }
        }
    }
}