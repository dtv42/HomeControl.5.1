// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TcpWriteCommand.cs" company="DTV-Online">
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

    internal sealed class TcpWriteCommand : Command
    {
        #region Private Data Members

        private readonly JsonSerializerOptions _jsonoptions = JsonExtensions.DefaultSerializerOptions;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TcpWriteCommand"/> class.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="logger"></param>
        public TcpWriteCommand(ITcpModbusClient client,
                               ILogger<TcpWriteCommand> logger)
            : base("write", "Supporting Modbus TCP write operations.")
        {
            // Setup command arguments and options.
            AddOption(new Option<bool>  (new string[] { "-?", "--help"    }, "Show help and usage information"));
            AddOption(new Option<string>(new string[] { "-c", "--coil"    }, "Write coil(s)."                 ).Name("Json"));
            AddOption(new Option<string>(new string[] { "-h", "--holding" }, "Writes holding register(s)."    ).Name("Json"));
            AddOption(new Option<bool>  (new string[] { "-x", "--hex"     }, "Displays the values in HEX"     ));
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
                var optionN = r.Children.Contains("n");
                var optionO = r.Children.Contains("o");
                var optionT = r.Children.Contains("t");

                if ((!optionC && !optionH) || (optionC && optionH) || optionHelp)
                {
                    return "Specify a single write option (coils or holding registers).";
                }

                return null;
            });

            // Setup execution handler.
            Handler = CommandHandler.Create<IConsole, bool, TcpWriteCommandOptions>((console, verbose, options) =>
            {
                logger.LogInformation("Handler()");

                // Run additional checks on options.
                options.CheckOptions(console);

                // Using TCP client options.
                client.TcpSlave.Address = options.Address;
                client.TcpSlave.Port = options.Port;
                client.TcpSlave.ID = options.SlaveID;
                client.TcpMaster.ReceiveTimeout = options.ReceiveTimeout;
                client.TcpMaster.SendTimeout = options.SendTimeout;

                if (verbose)
                {
                    console.Out.WriteLine($"Modbus Commandline Application: {RootCommand.ExecutableName}");
                    console.Out.WriteLine();
                    console.Out.Write("TcpMasterData: ");
                    console.Out.WriteLine(JsonSerializer.Serialize<TcpMasterData>(client.TcpMaster, _jsonoptions));
                    console.Out.Write("TcpSlaveData: ");
                    console.Out.WriteLine(JsonSerializer.Serialize<TcpSlaveData>(client.TcpSlave, _jsonoptions));
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
                        console.Out.WriteLine($"Modbus TCP slave not found at {options.Address}:{options.Port}.");
                        return ExitCodes.NotSuccessfullyCompleted;
                    }
                }
                catch (JsonException jex)
                {
                    logger.LogError(jex, $"Exception parsing JSON data values.");
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
