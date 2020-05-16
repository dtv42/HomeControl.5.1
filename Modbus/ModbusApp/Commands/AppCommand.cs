// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RootCommand.cs" company="DTV-Online">
//   Copyright(c) 2020 Dr. Peter Trimmel. All rights reserved.
// </copyright>
// <license>
//   Licensed under the MIT license. See the LICENSE file in the project root for more information.
// </license>
// <created>20-4-2020 13:29</created>
// <author>Peter Trimmel</author>
// --------------------------------------------------------------------------------------------------------------------
namespace ModbusApp.Commands
{
    #region Using Directives

    using System.CommandLine;
    using System.CommandLine.IO;
    using System.CommandLine.Invocation;
    using System.Text.Json;

    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;

    using UtilityLib;

    using ModbusApp.Models;

    #endregion

    /// <summary>
    /// Root command for the application providing an inherited option.
    /// Note that the default value is set from the application settings.
    /// </summary>
    internal sealed class AppCommand : RootCommand
    {
        #region Private Data Members

        private readonly JsonSerializerOptions _jsonoptions = JsonExtensions.DefaultSerializerOptions;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AppCommand"/> class.
        /// </summary>
        /// <param name="rtuCommand"></param>
        /// <param name="tcpCommand"></param>
        /// <param name="configuration"></param>
        /// <param name="settings"></param>
        /// <param name="logger"></param>
        public AppCommand(RtuCommand rtuCommand,
                          TcpCommand tcpCommand,
                          IConfiguration configuration,
                          AppSettings settings,
                          ILogger<AppCommand> logger)
            : base("Allows to read and write Modbus data using Modbus TCP or Modbus RTU.")
        {
            logger.LogDebug("AppCommand()");

            // Setup global and command options.
            AddGlobalOption(new Option<bool>("--verbose", "Show verbose information"));
            AddOption(new Option<bool>("--config", "Show configuration data"));

            // Add sub commands.
            AddCommand(rtuCommand);
            AddCommand(tcpCommand);

            // Add custom validation.
            AddValidator(r =>
            {
                var optionV = r.Children.Contains("verbose");
                var optionC = r.Children.Contains("config");

                if (!optionC && !optionV)
                {
                    return "Specify verbose or config option (--verbose, --config) or use a sub command.";
                }

                return null;
            });

            // Setup execution handler.
            Handler = CommandHandler.Create<IConsole, bool, bool>((console, config, verbose) =>
            {
                logger.LogInformation("Handler()");

                if (verbose)
                {
                    console.Out.WriteLine($"Modbus Commandline Application: {ExecutableName}");
                    console.Out.WriteLine();
                    console.Out.WriteLine($"AppSettings: {JsonSerializer.Serialize(settings, _jsonoptions)}");
                    console.Out.WriteLine();
                }

                if (config)
                {
                    console.Out.WriteLine("Configuration:");

                    foreach (var nvp in configuration.AsEnumerable())
                    {
                        console.Out.WriteLine($"    {nvp.Key}: {nvp.Value}");
                    }

                    console.Out.WriteLine();
                }

                return ExitCodes.SuccessfullyCompleted;
            });
        }
        
        #endregion
    }
}
