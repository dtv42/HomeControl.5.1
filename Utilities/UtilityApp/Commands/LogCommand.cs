// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogCommand.cs" company="DTV-Online">
//   Copyright(c) 2020 Dr. Peter Trimmel. All rights reserved.
// </copyright>
// <license>
//   Licensed under the MIT license. See the LICENSE file in the project root for more information.
// </license>
// <created>13-5-2020 13:53</created>
// <author>Peter Trimmel</author>
// --------------------------------------------------------------------------------------------------------------------
namespace UtilityApp.Commands
{
    #region Using Directives

    using System.CommandLine;
    using System.CommandLine.IO;
    using System.CommandLine.Invocation;

    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;

    using Serilog.Events;

    using UtilityLib;

    #endregion Using Directives

    /// <summary>
    ///  Sample of a command using logging statements. Various log statements are generated.
    /// </summary>
    public sealed class LogCommand : Command
    {
        #region Constructors

        /// <summary>
        ///  Initializes a new instance of the <see cref="LogCommand"/> class.
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="logger"></param>
        public LogCommand(IConfiguration configuration,
                          ILogger<LogCommand> logger)
            : base("log", "A sample dotnet console application - log command")
        {
            logger.LogDebug("LogCommand()");

            // Setup execution handler.
            Handler = CommandHandler.Create<IConsole, bool>((console, verbose) =>
            {
                logger.LogInformation("Handler()");

                if (verbose)
                {
                    console.Out.WriteLine($"Commandline Application: {RootCommand.ExecutableName}");
                    console.Out.WriteLine($"Console   Log level:    {CommandLineHost.ConsoleSwitch.MinimumLevel}");
                    console.Out.WriteLine($"File      Log level:    {CommandLineHost.FileSwitch.MinimumLevel}");
                    console.Out.WriteLine($"MinimumLevel Default:   {configuration.GetValue<LogEventLevel>("Serilog:MinimumLevel:Default")}");
                    console.Out.WriteLine($"MinimumLevel System:    {configuration.GetValue<LogEventLevel>("Serilog:MinimumLevel:Override:System")}");
                    console.Out.WriteLine($"MinimumLevel Microsoft: {configuration.GetValue<LogEventLevel>("Serilog:MinimumLevel:Override:Microsoft")}");
                    console.Out.WriteLine();
                }

                logger.LogTrace("Trace Message");
                logger.LogDebug("Debug Message");
                logger.LogInformation("Information Message");
                logger.LogWarning("Warning Message");
                logger.LogError("Error Message");
                logger.LogCritical("Critical Message");

                console.Out.WriteLine();

                return ExitCodes.SuccessfullyCompleted;
            });
        }

        #endregion Constructors
    }
}