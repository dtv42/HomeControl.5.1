// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RtuWriteCommand.cs" company="DTV-Online">
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

    internal sealed class RtuWriteCommand : Command
    {
        #region Private Data Members

        private readonly JsonSerializerOptions _jsonoptions = JsonExtensions.DefaultSerializerOptions;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RtuWriteCommand"/> class.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="logger"></param>
        public RtuWriteCommand(IRtuModbusClient client,
                              ILogger<RtuReadCommand> logger)
            : base("write", "Supporting Modbus RTU write operations.")
        {
            // Setup command arguments and options.
            AddOption(new Option<bool>  (new string[] { "-?", "--help"    }, "Show help and usage information"));
            AddOption(new Option<string>(new string[] { "-c", "--coil"    }, "Write coil(s)."                 ).Name("Json"));
            AddOption(new Option<string>(new string[] { "-h", "--holding" }, "Writes holding register(s)."    ).Name("Json"));
            AddOption(new Option<bool>  (new string[] { "-x", "--hex"     }, "Writes the HEX values (string)" ));
            AddOption(new Option<ushort>(new string[] { "-o", "--offset"  }, "The offset of the first item."  ).Name("Offset").Default((ushort)0));
            AddOption(new Option<string>(new string[] { "-t", "--type"    }, "Reads the specified data type"  ).Name("Type")
                .FromAmong("bits", "string", "byte", "short", "ushort", "int", "uint", "float", "double", "long", "ulong"));

            // Add custom validation.
            AddValidator(r =>
            {
                var optionHelp = r.Children.Contains("?");
                var optionC = r.Children.Contains("c");
                var optionH = r.Children.Contains("h");
                var optionX = r.Children.Contains("x");
                var optionO = r.Children.Contains("o");
                var optionT = r.Children.Contains("t");

                if ((!optionC && !optionH) || (optionC && optionH) || optionHelp)
                {
                    return "Specify a single write option (coils or holding registers).";
                }

                return null;
            });

            // Setup execution handler.
            Handler = CommandHandler.Create<IConsole, bool, RtuWriteCommandOptions>((console, verbose, options) =>
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
                        // Writing coils.
                        CommandHelper.WritingCoils(console,
                                                  client,
                                                  options.SlaveID,
                                                  options.Offset,
                                                  options.Coil);

                        // Writing holding registers.
                        CommandHelper.WritingHoldingRegisters(console,
                                                             client,
                                                             options.SlaveID,
                                                             options.Offset,
                                                             options.Holding,
                                                             options.Type,
                                                             options.Hex);
                    }
                    else
                    {
                        Console.WriteLine($"Modbus RTU slave not found at {options.SerialPort}.");
                        return ExitCodes.NotSuccessfullyCompleted;
                    }
                }
                catch (JsonException jex)
                {
                    console.Out.WriteLine(jex.Message);
                    return ExitCodes.NotSuccessfullyCompleted;
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
