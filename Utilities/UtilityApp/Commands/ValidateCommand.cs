// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ValidateCommand.cs" company="DTV-Online">
//   Copyright(c) 2020 Dr. Peter Trimmel. All rights reserved.
// </copyright>
// <license>
//   Licensed under the MIT license. See the LICENSE file in the project root for more information.
// </license>
// <created>8-5-2020 07:27</created>
// <author>Peter Trimmel</author>
// --------------------------------------------------------------------------------------------------------------------
namespace UtilityApp.Commands
{
    #region Using Directives

    using System.CommandLine;
    using System.CommandLine.IO;
    using System.CommandLine.Invocation;

    using Microsoft.Extensions.Logging;

    using UtilityLib;
    using UtilityApp.Models;

    #endregion

    /// <summary>
    ///  Sample of a sub command with various options and a custom validation method.
    /// </summary>
    internal sealed class ValidateCommand : Command
    {
        #region Constructors

        /// <summary>
        ///  Initializes a new instance of the <see cref="ValidateCommand"/> class.
        /// </summary>
        /// <param name="logger"></param>
        public ValidateCommand(ILogger<ValidateCommand> logger)
            : base("validate", "A sample dotnet console application - validate command")
        {
            logger.LogDebug("ValidateCommand()");

            // Setup command options.
            AddOption(new Option<int> ("-d", "default value"            ).Name("Number").Default(1));
            AddOption(new Option<int> ("-v", "required value"           ).Name("Number").Required());
            AddOption(new Option<int> ("-o", "zero or one value"        ).Name("Number").Arity(ArgumentArity.ZeroOrOne));
            AddOption(new Option<int> ("-n", "number value"             ).Name("Number").FromAmong(1, 2, 3));
            AddOption(new Option<int> ("-r", "range value [0..10]"      ).Name("Number").Range(0, 10));
            AddOption(new Option<long>("-l", "range value [0, 10000000]").Name("Number").Range(0, 10000000L));

            AddOption(new Option<string>("-s", "string value"           ).Name("String"));
            AddOption(new Option<string>("-m", "string value [max: 10]" ).Name("String").StringLength(10));
            AddOption(new Option<string>("-e", "not empty value"        ).Name("String").NotEmpty());
            AddOption(new Option<string>("-w", "not whitespace value"   ).Name("String").NotWhiteSpace());
            AddOption(new Option<string>("-x", "regex value"            ).Name("String").Regex(@"^[a-zA-Z\.\-_]+@([a-zA-Z\.\-_]+\.)+[a-zA-Z]{2,4}$"));
            AddOption(new Option<string>("-a", "IP address value"       ).Name("Address").IPAddress());
            AddOption(new Option<string>("-p", "IP endpoint value"      ).Name("Endpoint").IPEndpoint());
            AddOption(new Option<string>("-u", "URI value"              ).Name("URI").Uri());

            // Setup execution handler.
            Handler = CommandHandler.Create<IConsole, bool, ValidateOptions>((console, verbose, options) =>
            {
                logger.LogInformation("Handler()");

                if (verbose)
                {
                    console.Out.WriteLine($"Commandline Application: {RootCommand.ExecutableName}");
                    console.Out.WriteLine();
                }

                console.Out.WriteLine($"D Integer Value: {options.IntegerD}");
                console.Out.WriteLine($"V Integer Value: {options.IntegerV}");
                console.Out.WriteLine($"O Integer Value: {options.IntegerO}");
                console.Out.WriteLine($"N Integer Value: {options.IntegerN}");
                console.Out.WriteLine($"R Integer Value: {options.IntegerR}");
                console.Out.WriteLine($"L Integer Value: {options.IntegerL}");

                console.Out.WriteLine($"S String  Value: '{options.StringS}'");
                console.Out.WriteLine($"M String  Value: '{options.StringM}'");
                console.Out.WriteLine($"E String  Value: '{options.StringE}'");
                console.Out.WriteLine($"W String  Value: '{options.StringW}'");
                console.Out.WriteLine($"X String  Value: '{options.StringX}'");
                console.Out.WriteLine($"A String  Value: '{options.StringA}'");
                console.Out.WriteLine($"P String  Value: '{options.StringP}'");
                console.Out.WriteLine($"U String  Value: '{options.StringU}'");

                console.Out.WriteLine();

                return ExitCodes.SuccessfullyCompleted;
            });
        }

        #endregion
    }
}
