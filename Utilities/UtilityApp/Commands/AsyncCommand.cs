// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AsyncCommand.cs" company="DTV-Online">
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
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.Extensions.Logging;

    using UtilityLib;

    #endregion Using Directives

    /// <summary>
    ///  Sample of a command using async execution. The command simply waits for the specified delay.
    /// </summary>
    public sealed class AsyncCommand : Command
    {
        #region Constructors

        /// <summary>
        ///  Initializes a new instance of the <see cref="AsyncCommand"/> class.
        /// </summary>
        /// <param name="logger"></param>
        public AsyncCommand(ILogger<AsyncCommand> logger)
            : base("async", "A sample dotnet console application - async command")
        {
            logger.LogDebug("AsyncCommand()");

            // Setup command options.
            AddOption(new Option<int?>(new string[] { "-d", "--delay" }, "the delay in seconds").Default(10));

            // Setup execution handler.
            Handler = CommandHandler.Create<IConsole, CancellationToken, bool, int>(async (console, token, verbose, delay) =>
            {
                logger.LogInformation("Handler()");

                if (verbose)
                {
                    console.Out.WriteLine($"Commandline Application: {RootCommand.ExecutableName}");
                    console.Out.WriteLine();
                }

                try
                {
                    console.Out.WriteLine($"Waiting for {delay} seconds...");
                    await Task.Delay(1000 * delay, token);
                    return ExitCodes.SuccessfullyCompleted;
                }
                catch (TaskCanceledException tcx)
                {
                    logger.LogDebug(tcx, "Operation canceled");
                    return ExitCodes.OperationCanceled;
                }
            });
        }

        #endregion Constructors
    }
}