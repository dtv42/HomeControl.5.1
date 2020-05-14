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

    using System.Threading.Tasks;

    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Configuration;

    using UtilityLib;
    using ModbusLib;

    using ModbusApp.Models;
    using ModbusApp.Commands;
    using ModbusLib.Models;

    #endregion Using Directives

    /// <summary>
    ///  Application class providing the main entry point.
    /// </summary>
    public class Program : BaseProgram<AppSettings, RootCommand>
    {
        /// <summary>
        ///  Main application entrypoint.
        ///  This example shows the use of additional service configurations.
        /// </summary>
        /// <param name="args">The command line arguments</param>
        /// <returns>The exit code</returns>
        public static async Task<int> Main(string[] args)
            => await CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    // Add a singleton service using the application settings implementing TCP and RTU client settings.
                    services.AddSingleton((ITcpClientSettings)context.Configuration.GetSection("AppSettings").Get<AppSettings>());
                    services.AddSingleton((IRtuClientSettings)context.Configuration.GetSection("AppSettings").Get<AppSettings>());
                    // Configure the singleton Modbus client instances.
                    services.AddSingleton<ITcpModbusClient, TcpModbusClient>();
                    services.AddSingleton<IRtuModbusClient, RtuModbusClient>();
                })
                .BaseProgramRunAsync<AppSettings, RootCommand>(args);
    }
}