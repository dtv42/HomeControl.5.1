// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TcpMonitorCommand.cs" company="DTV-Online">
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
    using System.Linq;
    using System.Text.Json;
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.Extensions.Logging;

    using UtilityLib;
    using ModbusLib;
    using ModbusLib.Models;
    using ModbusApp.Models;

    #endregion

    internal sealed class TcpMonitorCommand : Command
    {
        #region Private Data Members

        private readonly JsonSerializerOptions _jsonoptions = JsonExtensions.DefaultSerializerOptions;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TcpMonitorCommand"/> class.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="logger"></param>
        public TcpMonitorCommand(ITcpModbusClient client,
                                 ILogger<TcpMonitorCommand> logger)
            : base("monitor", "Supporting Modbus TCP monitor operations.")
        {
            // Setup command options.
            AddOption(new Option<bool>  (new string[] { "-?", "--help"     }, "Show help and usage information"));
            AddOption(new Option<bool>  (new string[] { "-c", "--coil"     }, "Reads coil(s)"                  ));
            AddOption(new Option<bool>  (new string[] { "-d", "--discrete" }, "Reads discrete input(s)"        ));
            AddOption(new Option<bool>  (new string[] { "-h", "--holding"  }, "Reads holding register(s)"      ));
            AddOption(new Option<bool>  (new string[] { "-i", "--input"    }, "Reads input register(s)"        ));
            AddOption(new Option<bool>  (new string[] { "-x", "--hex"      }, "Displays the values in HEX"     ));
            AddOption(new Option<ushort>(new string[] { "-n", "--number"   }, "The number of items to read"    ).Name("Number").Default((ushort)1));
            AddOption(new Option<ushort>(new string[] { "-o", "--offset"   }, "The offset of the first item"   ).Name("Offset").Default((ushort)0));
            AddOption(new Option<string>(new string[] { "-t", "--type"     }, "Reads the specified data type"  ).Name("Type")
                .FromAmong("bits", "string", "byte", "short", "ushort", "int", "uint", "float", "double", "long", "ulong"));
            AddOption(new Option<uint>  (new string[] { "-r", "--repeat"   }, "The number of times to read"    ).Name("Repeat").Default((uint)10));
            AddOption(new Option<uint>  (new string[] { "-s", "--seconds"  }, "The seconds between read times" ).Name("Seconds").Default((uint)1));

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

                return null;
            });

            // Setup execution handler.
            Handler = CommandHandler.Create<IConsole, CancellationToken, bool, TcpMonitorCommandOptions>(async (console, token, verbose, options) =>
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
                        try
                        {
                            bool forever = (options.Repeat == 0);
                            bool header = true;
                            var time = DateTime.UtcNow;

                            while (!token.IsCancellationRequested)
                            {
                                var start = DateTime.UtcNow;
#pragma warning disable CS8604 // Possible null reference argument (logger).
                                if (verbose && !header) console.Out.WriteLine($"Time elapsed {start - time:d'.'hh':'mm':'ss'.'fff}");
                                ReadingData(client, console, logger, options, header);
#pragma warning restore CS8604 // Possible null reference argument (logger).
                                // Only first call is printing the header.
                                header = false;
                                var end = DateTime.UtcNow;
                                double delay = options.Seconds - (end - start).TotalSeconds;

                                if (delay < 0)
                                {
                                    logger?.LogWarning($"Monitoring: no time between reads (min. {delay + options.Seconds}).");
                                }

                                if ((--options.Repeat > 0) && delay > 0)
                                {
                                    await Task.Delay(TimeSpan.FromSeconds(delay), token);
                                }

                                if (!forever && (options.Repeat == 0))
                                {
                                    break;
                                }
                            }
                        }
                        catch (AggregateException aex) when (aex.InnerExceptions.All(e => e is OperationCanceledException))
                        {
                            console.Out.WriteLine("Monitoring cancelled.");
                        }
                        catch (OperationCanceledException)
                        {
                            console.Out.WriteLine("Monitoring cancelled.");
                        }
                        catch (Exception ex)
                        {
                            console.Out.WriteLine($"Exception: {ex.Message}");
                            return ExitCodes.UnhandledException;
                        }
                    }
                    else
                    {
                        console.Out.WriteLine($"Modbus TCP slave not found at {options.Address}:{options.Port}.");
                        return ExitCodes.NotSuccessfullyCompleted;
                    }
                }
                catch (Exception ex)
                {
                    console.Out.WriteLine($"Exception: {ex.Message}");
                    return ExitCodes.NotSuccessfullyCompleted;
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

        #region Private Methods

        /// <summary>
        /// Reading the specified data.
        /// </summary>
        private void ReadingData(ITcpModbusClient client,
                                 IConsole console,
                                 ILogger<TcpMonitorCommand> logger,
                                 TcpMonitorCommandOptions options,
                                 bool header = false)
        {
            logger?.LogDebug("TcpMonitor: Reading data...");

            // Reading coils.
            if (options.Coil)
            {
                CommandHelper.ReadingCoils(console,
                                          client,
                                          options.SlaveID,
                                          options.Number,
                                          options.Offset,
                                          header);
            }

            // Reading discrete inputs.
            if (options.Discrete)
            {
                CommandHelper.ReadingDiscreteInputs(console,
                                                   client,
                                                   options.SlaveID,
                                                   options.Number,
                                                   options.Offset,
                                                   header);
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
                                                     options.Hex,
                                                     header);
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
                                                   options.Hex,
                                                   header);
            }
        }

        #endregion
    }
}
