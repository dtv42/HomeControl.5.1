// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RtuReadCommand.cs" company="DTV-Online">
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

    using System;
    using System.CommandLine;
    using System.CommandLine.IO;
    using System.CommandLine.Invocation;
    using System.Text.Json;

    using Microsoft.Extensions.Logging;

    using UtilityLib;
    using ModbusLib;
    using ModbusLib.Models;
    using ModbusApp.Models;

    #endregion

    internal sealed class RtuReadCommand : Command
    {
        #region Private Data Members

        private readonly JsonSerializerOptions _jsonoptions = JsonExtensions.DefaultSerializerOptions;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RtuReadCommand"/> class.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="logger"></param>
        public RtuReadCommand(IRtuModbusClient client,
                              ILogger<RtuReadCommand> logger)
            : base("read", "Supporting Modbus RTU read operations.")
        {
            // Setup command options.
            AddOption(new Option<bool>  (new string[] { "-?", "--help" },       "Show help and usage information"));
            AddOption(new Option<bool>  (new string[] { "-c", "--coil" },       "Reads coil(s)"                  ));
            AddOption(new Option<bool>  (new string[] { "-d", "--discrete" },   "Reads discrete input(s)"        ));
            AddOption(new Option<bool>  (new string[] { "-h", "--holding" },    "Reads holding register(s)"      ));
            AddOption(new Option<bool>  (new string[] { "-i", "--input" },      "Reads input register(s)"        ));
            AddOption(new Option<bool>  (new string[] { "-x", "--hex" },        "Displays the values in HEX"     ));
            AddOption(new Option<ushort>(new string[] { "-n", "--number" },     "The number of items to read"    ).Name("Number").Default((ushort)1));
            AddOption(new Option<ushort>(new string[] { "-o", "--offset" },     "The offset of the first item"   ).Name("Offset").Default((ushort)0));
            AddOption(new Option<string>(new string[] { "-t", "--type" },       "Reads the specified data type"  ).Name("Type")
                .FromAmong("bits", "string", "byte", "short", "ushort", "int", "uint", "float", "double", "long", "ulong"));

            // Add custom validation.
            AddValidator(r =>
            {
                var optionHelp = r.Children.Contains("?");
                var optionC = r.Children.Contains("c");
                var optionD = r.Children.Contains("d");
                var optionH = r.Children.Contains("h");
                var optionI = r.Children.Contains("i");
                var optionX = r.Children.Contains("x");
                var optionN = r.Children.Contains("n");
                var optionO = r.Children.Contains("o");
                var optionT = r.Children.Contains("t");

                if ((!optionC && !optionD && !optionH && !optionI) ||
                    ((optionC && (optionD || optionH || optionI)) ||
                     (optionD && (optionC || optionH || optionI)) ||
                     (optionH && (optionD || optionC || optionI)) ||
                     (optionI && (optionD || optionH || optionC))) || optionHelp)
                {
                    return "Specify a single read option (coils, discrete inputs, holding registers, input registers).";
                }

                if (optionC || optionD)
                {
                    ushort number = (ushort)r.ValueForOption("n");

                    if ((number < 1) || (number > IModbusClient.MaxBooleanPoints))
                    {
                        return $"Number {number} is out of the range of valid values (1..{IModbusClient.MaxBooleanPoints}).";
                    }
                }

                if (optionH || optionI)
                {
                    ushort number = (ushort)r.ValueForOption("n");

                    if (optionT)
                    {
                        var type = (string)r.ValueForOption("t");

                        return type switch
                        {
                            "bits"   => (number > 1) ? "Only a single bit array value is supported." : null,
                            "string" => ((number < 1) || ((number + 1) / 2 > IModbusClient.MaxRegisterPoints)) ? $"Reading string values: number {number} is out of the range (max. {IModbusClient.MaxRegisterPoints} registers)."    : null,
                            "byte"   => ((number < 1) || ((number + 1) / 2 > IModbusClient.MaxRegisterPoints)) ? $"Reading byte values: number {number} is out of the range (max. {IModbusClient.MaxRegisterPoints} registers)."      : null,
                            "short"  => ((number < 1) || (number > IModbusClient.MaxRegisterPoints))           ? $"Reading short values: number {number} is out of the range (max. {IModbusClient.MaxRegisterPoints} registers)."     : null,
                            "ushort" => ((number < 1) || (number > IModbusClient.MaxRegisterPoints))           ? $"Reading ushort values: number {number} is out of the range (max. {IModbusClient.MaxRegisterPoints} registers)."    : null,
                            "int"    => ((number < 1) || (number > IModbusClient.MaxRegisterPoints / 2))       ? $"Reading int values: number {number} is out of the range of (max. {IModbusClient.MaxRegisterPoints} registers)."    : null,
                            "uint"   => ((number < 1) || (number > IModbusClient.MaxRegisterPoints / 2))       ? $"Reading uint values: number {number} is out of the range of (max. {IModbusClient.MaxRegisterPoints} registers)."   : null,
                            "float"  => ((number < 1) || (number > IModbusClient.MaxRegisterPoints / 2))       ? $"Reading float values: number {number} is out of the range of (max. {IModbusClient.MaxRegisterPoints} registers)."  : null,
                            "double" => ((number < 1) || (number > IModbusClient.MaxRegisterPoints / 4))       ? $"Reading double values: number {number} is out of the range of (max. {IModbusClient.MaxRegisterPoints} registers)." : null,
                            "long"   => ((number < 1) || (number > IModbusClient.MaxRegisterPoints / 4))       ? $"Reading long values: number {number} is out of the range of (max. {IModbusClient.MaxRegisterPoints} registers)."   : null,
                            "ulong"  => ((number < 1) || (number > IModbusClient.MaxRegisterPoints / 4))       ? $"Reading ulong values: number {number} is out of the range of (max. {IModbusClient.MaxRegisterPoints} registers)."  : null,
                            _ => $"Unknown type '{type}' (should not happen)."
                        };
                    }
                    else
                    {
                        if ((number < 1) || (number > IModbusClient.MaxRegisterPoints))
                        {
                            return $"Reading registers: number {number} is out of the range of valid values (1..{IModbusClient.MaxRegisterPoints}).";
                        }
                    }
                }

                return null;
            });

            // Setup execution handler.
            Handler = CommandHandler.Create<IConsole, bool, bool, RtuReadCommandOptions>((console, verbose, help, options) =>
            {
                logger.LogInformation("Handler()");

                // Run additional checks on options.
                options.CheckOptions(console);

                // Using RTU client options.
                client.RtuMaster.SerialPort = options.SerialPort;
                client.RtuMaster.Baudrate = options.Baudrate;
                client.RtuMaster.Parity = options.Parity;
                client.RtuMaster.DataBits = options.DataBits;
                client.RtuMaster.StopBits = options.StopBits;
                client.RtuMaster.ReadTimeout = options.ReadTimeout;
                client.RtuMaster.WriteTimeout = options.WriteTimeout;
                client.RtuSlave.ID = options.SlaveID;

                if (verbose)
                {
                    console.Out.WriteLine($"Modbus Commandline Application: {RootCommand.ExecutableName}");
                    console.Out.WriteLine();
                    console.Out.Write("RtuMasterData: ");
                    console.Out.WriteLine(JsonSerializer.Serialize<RtuMasterData>(client.RtuMaster, _jsonoptions));
                    console.Out.Write("RtuSlaveData: ");
                    console.Out.WriteLine(JsonSerializer.Serialize<RtuSlaveData>(client.RtuSlave, _jsonoptions));
                    console.Out.WriteLine();
                }

                try
                {
                    if (client.Connect())
                    {
                        // Reading coils.
                        if (options.Coil)
                        {
                            CommandHelper.ReadingCoils(console,
                                                      client,
                                                      options.SlaveID,
                                                      options.Number,
                                                      options.Offset);
                        }

                        // Reading discrete inputs.
                        if (options.Discrete)
                        {
                            CommandHelper.ReadingDiscreteInputs(console,
                                                               client,
                                                               client.RtuSlave.ID,
                                                               options.Number,
                                                               options.Offset);
                        }

                        // Reading holding registers.
                        if (options.Holding)
                        {
                            CommandHelper.ReadingHoldingRegisters(console,
                                                                 client,
                                                                 options.SlaveID,
                                                                 options.Number,
                                                                 options.Offset,
                                                                 options.Type,
                                                                 options.Hex);
                        }

                        // Reading input registers.
                        if (options.Input)
                        {
                            CommandHelper.ReadingInputRegisters(console,
                                                               client,
                                                               options.SlaveID,
                                                               options.Number,
                                                               options.Offset,
                                                               options.Type,
                                                               options.Hex);
                        }
                    }
                    else
                    {
                        console.Out.WriteLine($"Modbus RTU slave not found at {options.SerialPort}.");
                        return ExitCodes.NotSuccessfullyCompleted;
                    }
                }
                catch (Exception ex)
                {
                    console.Out.WriteLine($"Exception: {ex.Message}");
                    return ExitCodes.UnhandledException;
                }
                finally
                {
                    if (client.Connected)
                    {
                        client.Disconnect();
                    }
                }

                return ExitCodes.SuccessfullyCompleted;
            });
        }

        #endregion
    }
}
