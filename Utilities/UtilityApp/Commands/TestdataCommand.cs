// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestdataCommand.cs" company="DTV-Online">
//   Copyright(c) 2020 Dr. Peter Trimmel. All rights reserved.
// </copyright>
// <license>
//   Licensed under the MIT license. See the LICENSE file in the project root for more information.
// </license>
// <created>16-4-2020 11:27</created>
// <author>Peter Trimmel</author>
// --------------------------------------------------------------------------------------------------------------------
namespace UtilityApp.Commands
{
    #region Using Directives

    using System;
    using System.CommandLine;
    using System.CommandLine.IO;
    using System.CommandLine.Invocation;
    using System.Text.Json;

    using Microsoft.Extensions.Logging;

    using UtilityLib;
    using UtilityApp.Models;
    using System.Net;

    #endregion Using Directives

    /// <summary>
    ///  Sample of a command using the custom TestData settings.
    /// </summary>
    public class TestdataCommand : Command
    {
        #region Private Data Members

        private readonly JsonSerializerOptions _jsonoptions = JsonExtensions.DefaultSerializerOptions;

        #endregion Private Data Members

        #region Constructors

        /// <summary>
        ///  Initializes a new instance of the <see cref="TestdataCommand"/> class.
        ///  Note that the code option is hidden in the help output (see https://github.com/dotnet/command-line-api/issues/629).
        /// </summary>
        /// <param name="testdata"></param>
        /// <param name="logger"></param>
        public TestdataCommand(Testdata testdata,
                               ILogger<TestdataCommand> logger)
            : base("testdata", "A dotnet console application sub command - testdata command")
        {
            logger.LogDebug("TestCommand()");

            // Setup command options.
            AddOption(new Option<bool>          (new string[] { "-j", "--json"     }, "show json output"));
            AddOption(new Option<bool>          (new string[] { "-n", "--new"      }, "show new data"));
            AddOption(new Option<Guid>          (new string[] { "-g", "--guid"     }, "specify GUID"));
            AddOption(new Option<string>        (new string[] { "-a", "--address"  }, "specify IP address").IPAddress());
            AddOption(new Option<string>        (new string[] { "-e", "--endpoint" }, "specify IP endpoint").IPEndpoint());
            AddOption(new Option<string>        (new string[] { "-u", "--uri"      }, "specify absolute URI").Uri());
            AddOption(new Option<HttpStatusCode>(new string[] { "-c", "--code"     }, "specify HTTP status code").Hide());

            // Setup execution handler.
            Handler = CommandHandler.Create<IConsole, bool, TestdataOptions>((console, verbose, options) =>
            {
                logger.LogInformation("Handler()");

                if (verbose)
                {
                    console.Out.WriteLine($"Commandline Application: {RootCommand.ExecutableName}");
                    console.Out.WriteLine($"Console Log level: {CommandLineHost.ConsoleSwitch.MinimumLevel}");
                    console.Out.WriteLine($"File Log level: {CommandLineHost.FileSwitch.MinimumLevel}");
                    console.Out.WriteLine();
                }

                if (options.NewData)
                {
                    var data = new Testdata();

                    console.Out.WriteLine($"TestData():");
                    console.Out.WriteLine($"    Value:    {data.Value}");
                    console.Out.WriteLine($"    Name:     {data.Name}");
                    console.Out.WriteLine($"    Uuid:     {data.Uuid}");
                    console.Out.WriteLine($"    Address:  {data.Address}");
                    console.Out.WriteLine($"    Endpoint: {data.Endpoint}");
                    console.Out.WriteLine($"    Uri:      {data.Uri}");
                    console.Out.WriteLine($"    Code:     {data.Code}");

                    console.Out.WriteLine();
                }

                if (options.Json)
                {
                    console.Out.WriteLine($"TestData: {JsonSerializer.Serialize(testdata, _jsonoptions)}");
                    console.Out.WriteLine();
                }

                console.Out.WriteLine($"TestData:");
                console.Out.WriteLine($"    Value:    {testdata.Value}");
                console.Out.WriteLine($"    Name:     {testdata.Name}");
                console.Out.WriteLine($"    Uuid:     {testdata.Uuid}");
                console.Out.WriteLine($"    Address:  {testdata.Address}");
                console.Out.WriteLine($"    Endpoint: {testdata.Endpoint}");
                console.Out.WriteLine($"    Uri:      {testdata.Uri}");
                console.Out.WriteLine($"    Code:     {testdata.Code}");

                console.Out.WriteLine();

                console.Out.WriteLine($"CommandLine:");

                if (options.Guid.HasValue)       console.Out.WriteLine($"    Guid:     {options.Guid}");
                if (!(options.Address is null))  console.Out.WriteLine($"    Address:  {options.Address}");
                if (!(options.Endpoint is null)) console.Out.WriteLine($"    Endpoint: {options.Endpoint}");
                if (!(options.Uri is null))      console.Out.WriteLine($"    Uri:      {options.Uri}");
                if (!(options.Code is null))     console.Out.WriteLine($"    Code:     {options.Code}");

                console.Out.WriteLine();

                return ExitCodes.SuccessfullyCompleted;
            });
        }

        #endregion Constructors
    }
}