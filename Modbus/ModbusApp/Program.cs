// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="DTV-Online">
//   Copyright(c) 2020 Dr. Peter Trimmel. All rights reserved.
// </copyright>
// <license>
//   Licensed under the MIT license. See the LICENSE file in the project root for more information.
// </license>
// <created>20-4-2020 10:51</created>
// <author>Peter Trimmel</author>
// --------------------------------------------------------------------------------------------------------------------
namespace ModbusApp
{
    #region Using Directives

    using System;
    using System.Threading.Tasks;

    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Hosting;

    using UtilityLib;
    using ModbusLib;

    using ModbusApp.Models;
    using ModbusApp.Commands;
    using ModbusLib.Models;

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
                    .ConfigureServices((context, services) =>
                    {
                        services
                            // Add application specific settings.
                            .AddSingleton(context.Configuration.GetSection("AppSettings").Get<AppSettings>().ValidateAndThrow())

                            // Add a singleton service using the application settings implementing TCP and RTU client settings.
                            .AddSingleton((ITcpClientSettings)context.Configuration.GetSection("AppSettings").Get<AppSettings>())
                            .AddSingleton((IRtuClientSettings)context.Configuration.GetSection("AppSettings").Get<AppSettings>())
                            
                            // Configure the singleton Modbus client instances.
                            .AddSingleton<ITcpModbusClient, TcpModbusClient>()
                            .AddSingleton<IRtuModbusClient, RtuModbusClient>()

                            // Add commands (sub commands and root command).
                            .AddSingleton<RtuReadCommand>()
                            .AddSingleton<RtuWriteCommand>()
                            .AddSingleton<RtuMonitorCommand>()
                            .AddSingleton<TcpReadCommand>()
                            .AddSingleton<TcpWriteCommand>()
                            .AddSingleton<TcpMonitorCommand>()
                            .AddSingleton<RtuCommand>()
                            .AddSingleton<TcpCommand>()
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