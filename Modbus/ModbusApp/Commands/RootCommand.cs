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

    using System.Text.Json;

    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;

    using McMaster.Extensions.CommandLineUtils;

    using UtilityLib;

    using ModbusApp.Models;

    #endregion

    /// <summary>
    /// Root command for the application providing an inherited option.
    /// Note that the default value is set from the application settings.
    /// </summary>
    [Command(Name = "ModbusApp",
             FullName = "Modbus Application",
             Description = "Allows to read and write Modbus data using Modbus TCP or Modbus RTU.",
             ExtendedHelpText = "\nCopyright (c) 2020 Dr. Peter Trimmel - All rights reserved.")]
    [Subcommand(typeof(RtuCommand))]
    [Subcommand(typeof(TcpCommand))]
    public class RootCommand : BaseCommand<RootCommand, AppSettings>
    {
        #region Private Data Members

        private readonly JsonSerializerOptions _options = JsonExtensions.DefaultSerializerOptions;

        #endregion

        #region Public Properties

        [Option("--verbose", Inherited = true, Description = "Verbose output...")]
        public bool Verbose { get; }

        [Option("--config", Inherited = true, Description = "Show configuration...")]
        public bool ShowConfig { get; }

        [Option("--settings", Inherited = true, Description = "Show settings...")]
        public bool ShowSettings { get; }

        #endregion Public Properties

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RootCommand"/> class.
        /// </summary>
        /// <param name="console"></param>
        /// <param name="settings"></param>
        /// <param name="config"></param>
        /// <param name="environment"></param>
        /// <param name="lifetime"></param>
        /// <param name="logger"></param>
        /// <param name="application"></param>
        public RootCommand(IConsole console,
                           AppSettings settings,
                           IConfiguration config,
                           IHostEnvironment environment,
                           IHostApplicationLifetime lifetime,
                           ILogger<RootCommand> logger,
                           CommandLineApplication application)
            : base(console, settings, config, environment, lifetime, logger, application)
        { }

        #endregion

        #region Private Methods

        /// <summary>
        ///  Implements the command execution.
        /// </summary>
        /// <returns>The exit code</returns>
        private int OnExecute()
        {
            _logger.LogDebug("OnExecute()");

            if (Verbose)
            {
                _console.WriteLine($"Commandline application: {_application.Name}");
                _console.WriteLine($"Console Log level: {CommandLineHost.ConsoleSwitch.MinimumLevel}");
                _console.WriteLine($"File Log level: {CommandLineHost.FileSwitch.MinimumLevel}");
            }

            if (ShowConfig)
            {
                _console.WriteLine("Configuration:");

                foreach (var item in _config.AsEnumerable())
                {
                    _console.WriteLine($"    {item.Key}: {item.Value}");
                }

                _console.WriteLine();
            }

            if (ShowSettings)
            {
                _console.WriteLine($"AppSettings: {JsonSerializer.Serialize(_settings, _options)}");
                _console.WriteLine();
            }

            if (!ShowConfig && !ShowSettings)
            {
                _application.ShowHelp();
            }

            return ExitCodes.SuccessfullyCompleted;
        }

        #endregion
    }
}
