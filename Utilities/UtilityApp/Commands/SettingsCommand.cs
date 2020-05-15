// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SettingsCommand.cs" company="DTV-Online">
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
    using System.Linq;
    using System.Text.Json;

    using Microsoft.Extensions.Logging;

    using UtilityLib;
    using UtilityApp.Models;

    #endregion Using Directives

    /// <summary>
    ///  Sample of a command using the application settings.
    /// </summary>
    public class SettingsCommand : Command
    {
        #region Private Data Members

        private readonly JsonSerializerOptions _jsonoptions = JsonExtensions.DefaultSerializerOptions;

        #endregion

        #region Constructors

        /// <summary>
        ///  Initializes a new instance of the <see cref="SettingsCommand"/> class.
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="logger"></param>
        public SettingsCommand(AppSettings settings,
                               ILogger<SettingsCommand> logger)
            : base("settings", "A dotnet console application sub command - settings command")
        {
            logger.LogDebug("SettingsCommand()");

            // Setup execution handler.
            Handler = CommandHandler.Create<IConsole, bool, PropertyOptions>((console, verbose, options) =>
            {
                logger.LogInformation("Handler()");

                if (verbose)
                {
                    console.Out.WriteLine($"Commandline Application: {RootCommand.ExecutableName}");
                    console.Out.WriteLine($"Console Log level: {CommandLineHost.ConsoleSwitch.MinimumLevel}");
                    console.Out.WriteLine($"File Log level: {CommandLineHost.FileSwitch.MinimumLevel}");
                    console.Out.WriteLine($"AppSettings: {JsonSerializer.Serialize(settings, _jsonoptions)}");
                    console.Out.WriteLine();
                }

                console.Out.WriteLine($"Settings:");
                console.Out.WriteLine($"    StringValue:         {settings.StringValue}");
                console.Out.WriteLine($"    BooleanValue:        {settings.BooleanValue}");
                console.Out.WriteLine($"    IntegerValue:        {settings.IntegerValue}");
                console.Out.WriteLine($"    LongValue:           {settings.LongValue}");
                console.Out.WriteLine($"    FloatValue:          {settings.FloatValue}");
                console.Out.WriteLine($"    DoubleValue:         {settings.DoubleValue}");
                console.Out.WriteLine($"    DecimalValue:        {settings.DecimalValue}");
                console.Out.WriteLine($"    DateTimeValue:       {settings.DateTimeValue}");
                console.Out.WriteLine($"    DateTimeOffsetValue: {settings.DateTimeOffsetValue}");
                console.Out.WriteLine();
                console.Out.WriteLine($"    Settings:");
                console.Out.WriteLine($"        StringValue:     {settings.Settings.StringValue}");
                console.Out.WriteLine($"        BooleanValue:    {settings.Settings.BooleanValue}");
                console.Out.WriteLine($"        IntegerValue:    {settings.Settings.IntegerValue}");
                console.Out.WriteLine($"        LongValue:       {settings.Settings.LongValue}");
                console.Out.WriteLine($"        FloatValue:      {settings.Settings.FloatValue}");
                console.Out.WriteLine($"        DoubleValue:     {settings.Settings.DoubleValue}");
                console.Out.WriteLine($"        DecimalValue:    {settings.Settings.DecimalValue}");
                console.Out.WriteLine();

                if (verbose)
                {
                    for (var i = 0; i < settings.StringArray.Length;         ++i) console.Out.WriteLine($"    StringArray         [{i}]: {settings.StringArray[i]}");
                    for (var i = 0; i < settings.BooleanArray.Length;        ++i) console.Out.WriteLine($"    BooleanArray        [{i}]: {settings.BooleanArray[i]}");
                    for (var i = 0; i < settings.IntegerArray.Length;        ++i) console.Out.WriteLine($"    IntegerArray        [{i}]: {settings.IntegerArray[i]}");
                    for (var i = 0; i < settings.LongArray.Length;           ++i) console.Out.WriteLine($"    LongArray           [{i}]: {settings.LongArray[i]}");
                    for (var i = 0; i < settings.FloatArray.Length;          ++i) console.Out.WriteLine($"    FloatArray          [{i}]: {settings.FloatArray[i]}");
                    for (var i = 0; i < settings.DoubleArray.Length;         ++i) console.Out.WriteLine($"    DoubleArray         [{i}]: {settings.DoubleArray[i]}");
                    for (var i = 0; i < settings.DecimalArray.Length;        ++i) console.Out.WriteLine($"    DecimalArray        [{i}]: {settings.DecimalArray[i]}");
                    for (var i = 0; i < settings.DateTimeArray.Length;       ++i) console.Out.WriteLine($"    DateTimeArray       [{i}]: {settings.DateTimeArray[i]}");
                    for (var i = 0; i < settings.DateTimeOffsetArray.Length; ++i) console.Out.WriteLine($"    DateTimeOffsetArray [{i}]: {settings.DateTimeOffsetArray[i]}");

                    console.Out.WriteLine();

                    for (var i = 0; i < settings.StringList.Count;         ++i) console.Out.WriteLine($"    StringList         [{i}]: {settings.StringList[i]}");
                    for (var i = 0; i < settings.BooleanList.Count;        ++i) console.Out.WriteLine($"    BooleanList        [{i}]: {settings.BooleanList[i]}");
                    for (var i = 0; i < settings.IntegerList.Count;        ++i) console.Out.WriteLine($"    IntegerList        [{i}]: {settings.IntegerList[i]}");
                    for (var i = 0; i < settings.LongList.Count;           ++i) console.Out.WriteLine($"    LongList           [{i}]: {settings.LongList[i]}");
                    for (var i = 0; i < settings.FloatList.Count;          ++i) console.Out.WriteLine($"    FloatList          [{i}]: {settings.FloatList[i]}");
                    for (var i = 0; i < settings.DoubleList.Count;         ++i) console.Out.WriteLine($"    DoubleList         [{i}]: {settings.DoubleList[i]}");
                    for (var i = 0; i < settings.DecimalList.Count;        ++i) console.Out.WriteLine($"    DecimalList        [{i}]: {settings.DecimalList[i]}");
                    for (var i = 0; i < settings.DateTimeList.Count;       ++i) console.Out.WriteLine($"    DateTimeList       [{i}]: {settings.DateTimeList[i]}");
                    for (var i = 0; i < settings.DateTimeOffsetList.Count; ++i) console.Out.WriteLine($"    DateTimeOffsetList [{i}]: {settings.DateTimeOffsetList[i]}");
                    for (var i = 0; i < settings.Dictionary.Count;         ++i) console.Out.WriteLine($"    Dictionary         [{i}]: {settings.Dictionary.Keys.ToArray()[i]}, {settings.Dictionary.Values.ToArray()[i]}");

                    console.Out.WriteLine();
                }

                return ExitCodes.SuccessfullyCompleted;
            });
        }

        #endregion Constructors
    }
}