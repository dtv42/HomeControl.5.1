// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RootCommand.cs" company="DTV-Online">
//   Copyright(c) 2020 Dr. Peter Trimmel. All rights reserved.
// </copyright>
// <license>
//   Licensed under the MIT license. See the LICENSE file in the project root for more information.
// </license>
// <created>15-4-2020 21:43</created>
// <author>Peter Trimmel</author>
// --------------------------------------------------------------------------------------------------------------------
namespace UtilityApp.Commands
{
    #region Using Directives

    using System.CommandLine;
    using System.CommandLine.IO;
    using System.CommandLine.Invocation;
    using System.Text.Json;

    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;

    using UtilityLib;
    using UtilityApp.Models;

    #endregion Using Directives

    /// <summary>
    ///  Root command class providing common options.
    ///  Note that the options are public properties and allowing inheritance to sub commands.
    /// </summary>
    internal sealed class AppCommand : RootCommand
    {
        #region Private Data Members

        private readonly JsonSerializerOptions _jsonoptions = JsonExtensions.DefaultSerializerOptions;

        #endregion Private Data Members

        #region Constructors

        /// <summary>
        ///  Initializes a new instance of the <see cref="AppCommand"/> class.
        /// </summary>
        /// <param name="asyncCommand"></param>
        /// <param name="settingsCommand"></param>
        /// <param name="testdataCommand"></param>
        /// <param name="propertyCommand"></param>
        /// <param name="validateCommand"></param>
        /// <param name="logCommand"></param>
        /// <param name="configuration"></param>
        /// <param name="settings"></param>
        /// <param name="logger"></param>
        public AppCommand(AsyncCommand asyncCommand,
                          SettingsCommand settingsCommand,
                          TestdataCommand testdataCommand,
                          PropertyCommand propertyCommand,
                          ValidateCommand validateCommand,
                          LogCommand logCommand,
                          IConfiguration configuration,
                          AppSettings settings,
                          ILogger<AppCommand> logger)
            : base("A sample dotnet console application.")
        {
            logger.LogDebug("AppCommand()");

            // Setup global and command options.
            AddGlobalOption(new Option<bool>("--verbose", "Show verbose information"));
            AddOption(new Option<bool>("--config", "Show configuration data"));

            // Add sub commands.
            AddCommand(asyncCommand);
            AddCommand(settingsCommand);
            AddCommand(testdataCommand);
            AddCommand(propertyCommand);
            AddCommand(validateCommand);
            AddCommand(logCommand);

            // Setup execution handler.
            Handler = CommandHandler.Create<IConsole, bool, bool>((console, config, verbose) =>
            {
                logger.LogInformation("Handler()");

                if (verbose)
                {
                    console.Out.WriteLine($"Commandline Application: {ExecutableName}");
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

                console.Out.WriteLine("Hello Console!");
            });
        }

        #endregion Constructors
    }
}