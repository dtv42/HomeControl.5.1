// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CommandLineService.cs" company="DTV-Online">
//   Copyright(c) 2020 Dr. Peter Trimmel. All rights reserved.
// </copyright>
// <license>
//   Licensed under the MIT license. See the LICENSE file in the project root for more information.
// </license>
// <created>13-5-2020 09:11</created>
// <author>Peter Trimmel</author>
// --------------------------------------------------------------------------------------------------------------------
namespace UtilityLib
{
    #region Using Directives

    using System.CommandLine;
    using System.CommandLine.Builder;
    using System.CommandLine.Parsing;

    #endregion

    /// <summary>
    ///  CommandLineService interface providing a default create parser method.
    /// </summary>
    public interface ICommandLineService
    {
        Parser CreateParser();
    }

    /// <summary>
    ///  Helper class to create a parser, using a command line builer and a root command.
    /// </summary>
    /// <typeparam name="TCommand">The root command type</typeparam>
    public class CommandLineService<TCommand> : ICommandLineService where TCommand : RootCommand
    {
        #region Private Data Members

        private readonly TCommand _command;

        #endregion

        /// <summary>
        ///  Initializes a new instance of the <see cref="CommandLineService"/> class.
        /// </summary>
        /// <param name="command">The command line root command</param>
        public CommandLineService(TCommand command)
        {
            _command = command;
        }

        /// <summary>
        ///  Method to create a new commandline parser utilizing the root command.
        ///  Note this implementation uses the CommandLineBuilder default setup:
        ///  
        ///     UseVersionOption()
        ///     UseHelp()
        ///     UseParseDirective()
        ///     UseDebugDirective()
        ///     UseSuggestDirective()
        ///     RegisterWithDotnetSuggest()
        ///     UseTypoCorrections()
        ///     UseParseErrorReporting()
        ///     UseExceptionHandler()
        ///     CancelOnProcessTermination()
        ///     
        /// </summary>
        /// <returns></returns>
        public Parser CreateParser()
        {
            return new CommandLineBuilder(_command)
                .UseDefaults()
                .Build()
                ;
        }
    }
}
