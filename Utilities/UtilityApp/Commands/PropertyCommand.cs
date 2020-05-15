// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PropertyCommand.cs" company="DTV-Online">
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

    using System;
    using System.CommandLine;
    using System.CommandLine.IO;
    using System.CommandLine.Invocation;
    using System.CommandLine.Parsing;
    using System.Collections;
    using System.Reflection;
    using System.Text.Json;

    using Microsoft.Extensions.Logging;

    using UtilityLib;
    using UtilityApp.Models;

    #endregion Using Directives

    /// <summary>
    ///  Sample of a property command showing property infos using the AppSettings instance.
    ///  Note that for named properties only simple types are supported (no arrays or lists).
    /// </summary>
    public sealed class PropertyCommand : Command
    {
        #region Private Data Members

        private readonly JsonSerializerOptions _jsonoptions = JsonExtensions.DefaultSerializerOptions;

        #endregion

        #region Constructors

        /// <summary>
        ///  Initializes a new instance of the <see cref="PropertyCommand"/> class.
        /// </summary>
        /// <param name="console"></param>
        /// <param name="settings"></param>
        /// <param name="config"></param>
        /// <param name="environment"></param>
        /// <param name="lifetime"></param>
        /// <param name="logger"></param>
        /// <param name="application"></param>
        public PropertyCommand(AppSettings settings,                              
                               ILogger<PropertyCommand> logger)
            : base("property", "A dotnet console application sub command - property command")
        {
            logger.LogDebug("PropertyCommand()");

            // Setup command arguments and options.
            AddArgument(new Argument<string>("Name", "The property name.").Arity(ArgumentArity.ZeroOrOne));
            AddArgument(new Argument<string>("Value", "The property value.").Arity(ArgumentArity.ZeroOrOne));

            AddOption(new Option<bool>(new string[] { "-p", "--properties"   }, "show all properties"));
            AddOption(new Option<bool>(new string[] { "-s", "--simple"       }, "show simple properties"));
            AddOption(new Option<bool>(new string[] { "-a", "--arrays"       }, "show arrays"));
            AddOption(new Option<bool>(new string[] { "-l", "--lists"        }, "show lists"));
            AddOption(new Option<bool>(new string[] { "-d", "--dictionaries" }, "show dictionaries"));
            AddOption(new Option<bool>(new string[] { "-v", "--value"        }, "show value"));

            // Add custom validation.
            AddValidator(r =>
            {
                if (string.IsNullOrEmpty(r.GetArgumentValueOrDefault<string>("Name")) &&
                    !r.Children.Contains("-p") && !r.Children.Contains("-s") &&
                    !r.Children.Contains("-a") && !r.Children.Contains("-l") && !r.Children.Contains("-d"))
                {
                        return "Please select at least a property type (-p|-s|-a|-l|-d) or specify a property name.";
                }

                return null;
            });

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

                if (options.PropertyName is null)
                {
                    var properties = typeof(AppSettings).GetProperties(BindingFlags.Public | BindingFlags.Instance);

                    foreach (var info in properties)
                    {
                        if ((options.ShowAll || options.ShowArrays) && (info?.PropertyType.IsArray ?? false) && (info.GetValue(settings) is Array))
                        {
                            console.Out.WriteLine($"Property {info.Name}");
                            console.Out.WriteLine($"    CanRead:       {info.CanRead}");
                            console.Out.WriteLine($"    CanWrite:      {info.CanWrite}");
                            console.Out.WriteLine($"    DeclaringType: {info.DeclaringType}");
                            console.Out.WriteLine($"    PropertyType:  {info.PropertyType}");
                            console.Out.WriteLine($"    MemberType:    {info.MemberType}");

                            Array? value = (Array?)info.GetValue(settings);
                            console.Out.WriteLine($"    Array:         [{value?.Length}]");

                            if (options.ShowValue)
                            {
                                for (int i = 0; i < value?.Length; ++i)
                                {
                                    console.Out.WriteLine($"    Value[{i}]:      {((Array?)info?.GetValue(settings))?.GetValue(i)}");
                                }
                            }
                        }
                        else if ((options.ShowAll || options.ShowLists) && (info?.PropertyType.IsGenericType ?? false) && (info.GetValue(settings) is IList))
                        {
                            console.Out.WriteLine($"Property {info.Name}");
                            console.Out.WriteLine($"    CanRead:       {info.CanRead}");
                            console.Out.WriteLine($"    CanWrite:      {info.CanWrite}");
                            console.Out.WriteLine($"    DeclaringType: {info.DeclaringType}");
                            console.Out.WriteLine($"    PropertyType:  {info.PropertyType}");
                            console.Out.WriteLine($"    MemberType:    {info.MemberType}");

                            IList? value = (IList?)info.GetValue(settings);
                            console.Out.WriteLine($"    List:          [{value?.Count}]");

                            if (options.ShowValue)
                            {
                                for (int i = 0; i < value?.Count; ++i)
                                {
                                    console.Out.WriteLine($"    Value[{i}]:      {((IList?)info?.GetValue(settings))?[i]}");
                                }
                            }
                        }
                        else if ((options.ShowAll || options.ShowDictionaries) && (info?.PropertyType.IsGenericType ?? false) && (info.GetValue(settings) is IDictionary))
                        {
                            console.Out.WriteLine($"Property {info.Name}");
                            console.Out.WriteLine($"    CanRead:       {info.CanRead}");
                            console.Out.WriteLine($"    CanWrite:      {info.CanWrite}");
                            console.Out.WriteLine($"    DeclaringType: {info.DeclaringType}");
                            console.Out.WriteLine($"    PropertyType:  {info.PropertyType}");
                            console.Out.WriteLine($"    MemberType:    {info.MemberType}");

                            IDictionary? value = (IDictionary?)info.GetValue(settings);
                            console.Out.WriteLine($"    Dictionary:    [{value?.Count}]");

                            if (options.ShowValue)
                            {
                                var dictionary = (IDictionary?)info?.GetValue(settings);
                                int i = 0;

                                if (!(dictionary is null))
                                {
                                    foreach (DictionaryEntry? item in dictionary)
                                    {
                                        console.Out.WriteLine($"    Value[{i}]:      {item?.Key}, {item?.Value}");
                                    }
                                }
                            }
                        }
                        else if ((options.ShowAll || options.ShowSimple) && !(info?.PropertyType.IsArray ?? false) && !(info?.PropertyType.IsGenericType ?? false))
                        {
                            console.Out.WriteLine($"Property {info?.Name}");
                            console.Out.WriteLine($"    CanRead:       {info?.CanRead}");
                            console.Out.WriteLine($"    CanWrite:      {info?.CanWrite}");
                            console.Out.WriteLine($"    DeclaringType: {info?.DeclaringType}");
                            console.Out.WriteLine($"    PropertyType:  {info?.PropertyType}");
                            console.Out.WriteLine($"    MemberType:    {info?.MemberType}");

                            if (options.ShowValue)
                            {
                                console.Out.WriteLine($"    Value:         {info?.GetValue(settings)}");
                            }
                        }
                    }

                    console.Out.WriteLine();
                }
                else
                {
                    var info = typeof(AppSettings).GetProperty(options.PropertyName, BindingFlags.Public | BindingFlags.Instance);

                    if (info is null)
                    {
                        console.Out.WriteLine($"Property '{options.PropertyName}' not found.");
                        return ExitCodes.InvalidData;
                    }
                    else if (options.PropertyValue is null)
                    {
                        console.Out.WriteLine($"Property {info?.Name}");
                        console.Out.WriteLine($"    CanRead:       {info?.CanRead}");
                        console.Out.WriteLine($"    CanWrite:      {info?.CanWrite}");
                        console.Out.WriteLine($"    DeclaringType: {info?.DeclaringType}");
                        console.Out.WriteLine($"    PropertyType:  {info?.PropertyType}");
                        console.Out.WriteLine($"    MemberType:    {info?.MemberType}");

                        if ((info?.PropertyType.IsArray ?? false) && (info.GetValue(settings) is Array))
                        {
                            Array? value = (Array?)info.GetValue(settings);
                            console.Out.WriteLine($"    Array:         [{value?.Length}]");

                            if (options.ShowValue)
                            {
                                for (int i = 0; i < value?.Length; ++i)
                                {
                                    console.Out.WriteLine($"    Value[{i}]:      {((Array?)info?.GetValue(settings))?.GetValue(i)}");
                                }
                            }
                        }
                        else if ((info?.PropertyType.IsGenericType ?? false) && (info.GetValue(settings) is IList))
                        {
                            IList? value = (IList?)info.GetValue(settings);
                            console.Out.WriteLine($"    List:          [{value?.Count}]");

                            for (int i = 0; i < value?.Count; ++i)
                            {
                                console.Out.WriteLine($"    Value[{i}]:      {((IList?)info?.GetValue(settings))?[i]}");
                            }
                        }
                        else if ((info?.PropertyType.IsGenericType ?? false) && (info.GetValue(settings) is IDictionary))
                        {
                            IDictionary? value = (IDictionary?)info.GetValue(settings);
                            console.Out.WriteLine($"    Dictionary:    [{value?.Count}]");

                            if (options.ShowValue)
                            {
                                var dictionary = (IDictionary?)info?.GetValue(settings);
                                int i = 0;

                                if (!(dictionary is null))
                                {
                                    foreach (DictionaryEntry? item in dictionary)
                                    {
                                        console.Out.WriteLine($"    Value[{i}]:      {item?.Key}, {item?.Value}");
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (options.ShowValue)
                            {
                                console.Out.WriteLine($"    Value:         {info?.GetValue(settings)}");
                            }
                        }

                        console.Out.WriteLine();
                    }
                    else
                    {
                        if (!(info?.PropertyType.IsArray ?? false) && !(info?.PropertyType.IsGenericType ?? false))
                        {
                            TypeCode typeCode = Type.GetTypeCode(info?.PropertyType);

                            switch (typeCode)
                            {
                                case TypeCode.String:
                                    info?.SetValue(settings, options.PropertyValue);
                                    break;

                                case TypeCode.Boolean:
                                    if (bool.TryParse(options.PropertyValue, out bool boolValue)) info?.SetValue(settings, boolValue);
                                    else console.Out.WriteLine($"Property value '{options.PropertyValue}' invalid.");
                                    break;

                                case TypeCode.Int32:
                                    if (int.TryParse(options.PropertyValue, out int intValue)) info?.SetValue(settings, intValue);
                                    else console.Out.WriteLine($"Property value '{options.PropertyValue}' invalid.");
                                    break;

                                case TypeCode.Int64:
                                    if (long.TryParse(options.PropertyValue, out long longValue)) info?.SetValue(settings, longValue);
                                    else console.Out.WriteLine($"Property value '{options.PropertyValue}' invalid.");
                                    break;

                                case TypeCode.Single:
                                    if (float.TryParse(options.PropertyValue, out float floatValue)) info?.SetValue(settings, floatValue);
                                    else console.Out.WriteLine($"Property value '{options.PropertyValue}' invalid.");
                                    break;

                                case TypeCode.Double:
                                    if (double.TryParse(options.PropertyValue, out double doubleValue)) info?.SetValue(settings, doubleValue);
                                    else console.Out.WriteLine($"Property value '{options.PropertyValue}' invalid.");
                                    break;

                                case TypeCode.Decimal:
                                    if (decimal.TryParse(options.PropertyValue, out decimal decimalValue)) info?.SetValue(settings, decimalValue);
                                    else console.Out.WriteLine($"Property value '{options.PropertyValue}' invalid.");
                                    break;

                                case TypeCode.DateTime:
                                    if (DateTime.TryParse(options.PropertyValue, out DateTime datetimeValue)) info?.SetValue(settings, datetimeValue);
                                    else console.Out.WriteLine($"Property value '{options.PropertyValue}' invalid.");
                                    break;

                                default:
                                    console.Out.WriteLine($"Property type '{info?.PropertyType}' not supported.");
                                    break;
                            }

                            console.Out.WriteLine($"Property {info?.Name}");
                            console.Out.WriteLine($"    CanRead:       {info?.CanRead}");
                            console.Out.WriteLine($"    CanWrite:      {info?.CanWrite}");
                            console.Out.WriteLine($"    DeclaringType: {info?.DeclaringType}");
                            console.Out.WriteLine($"    PropertyType:  {info?.PropertyType}");
                            console.Out.WriteLine($"    MemberType:    {info?.MemberType}");
                            console.Out.WriteLine($"    Value:         {info?.GetValue(settings)}");
                        }
                        else
                        {
                            console.Out.WriteLine($"Only simple property types supported.");
                        }
                    }
                }

                return ExitCodes.SuccessfullyCompleted;
            });
        }

        #endregion Constructors
    }
}