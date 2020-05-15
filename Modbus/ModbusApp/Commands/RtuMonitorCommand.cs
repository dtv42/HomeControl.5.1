// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RtuMonitorCommand.cs" company="DTV-Online">
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
    using System.Collections;
    using System.CommandLine;
    using System.CommandLine.IO;
    using System.CommandLine.Invocation;
    using System.Linq;
    using System.Text.Json;
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.Extensions.Logging;

    using NModbus.Extensions;

    using UtilityLib;
    using ModbusLib;
    using ModbusLib.Models;
    using ModbusApp.Models;

    #endregion

    internal sealed class RtuMonitorCommand : Command
    {
        #region Private Data Members

        private readonly JsonSerializerOptions _jsonoptions = JsonExtensions.DefaultSerializerOptions;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RtuMonitorCommand"/> class.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="logger"></param>
        public RtuMonitorCommand(IRtuModbusClient client,
                                 ILogger<RtuMonitorCommand> logger)
            : base("monitor", "Supporting Modbus RTU monitor operations.")
        {
            // Setup command options.
            AddOption(new Option<bool>(new string[] { "-?", "--help" }, "Show help and usage information"));
            AddOption(new Option<bool>(new string[] { "-c", "--coil" }, "Reads coil(s)"));
            AddOption(new Option<bool>(new string[] { "-d", "--discrete" }, "Reads discrete input(s)"));
            AddOption(new Option<bool>(new string[] { "-h", "--holding" }, "Reads holding register(s)"));
            AddOption(new Option<bool>(new string[] { "-i", "--input" }, "Reads input register(s)"));
            AddOption(new Option<bool>(new string[] { "-x", "--hex" }, "Displays the values in HEX"));
            AddOption(new Option<ushort>(new string[] { "-n", "--number" }, "The number of items to read").Name("Number").Default(1));
            AddOption(new Option<ushort>(new string[] { "-o", "--offset" }, "The offset of the first item").Name("Offset").Default(0));
            AddOption(new Option<string>(new string[] { "-t", "--type" }, "Reads the specified data type").Name("Type")
                .FromAmong("bits", "string", "byte", "short", "ushort", "int", "uint", "float", "double", "long", "ulong"));
            AddOption(new Option<uint>(new string[] { "-r", "--repeat" }, "The number of times to read").Name("Repeat").Default(0));
            AddOption(new Option<uint>(new string[] { "-s", "--seconds" }, "The seconds between read times").Name("Seconds").Default(10));

            // Add custom validation.
            AddValidator(r =>
            {
                if (r.Children.Contains("-?")) return "Specify a single read option (coils, discrete inputs, holding registers, input registers).";

                var optionC = r.Children.Contains("-c");
                var optionD = r.Children.Contains("-d");
                var optionH = r.Children.Contains("-h");
                var optionI = r.Children.Contains("-i");
                var optionX = r.Children.Contains("-x");
                var optionN = r.Children.Contains("-n");
                var optionO = r.Children.Contains("-o");
                var optionT = r.Children.Contains("-t");

                if ((!optionC && !optionD && !optionH && !optionI) ||
                    ((optionC && (optionD || optionH || optionI)) ||
                     (optionD && (optionC || optionH || optionI)) ||
                     (optionH && (optionD || optionC || optionI)) ||
                     (optionI && (optionD || optionH || optionC))))
                {
                    return "Specify a single read option (coils, discrete inputs, holding registers, input registers).";
                }

                return null;
            });

            // Setup execution handler.
            Handler = CommandHandler.Create<IConsole, CancellationToken, bool, RtuMonitorCommandOptions>(async (console, token, verbose, options) =>
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
                        try
                        {
                            bool forever = (options.Repeat == 0);
                            bool print = true;

                            while (!token.IsCancellationRequested)
                            {
                                // Read the specified data.
                                var start = DateTime.UtcNow;
#pragma warning disable CS8604 // Possible null reference argument (logger).
                                ReadingData(client, console, logger, options, print);
#pragma warning restore CS8604 // Possible null reference argument (logger).
                                // Only first call is using verbose printing.
                                print = false;
                                var end = DateTime.UtcNow;
                                double delay = ((options.Seconds * 1000.0) - (end - start).TotalMilliseconds) / 1000.0;

                                if (options.Seconds > 0)
                                {
                                    if (delay < 0)
                                    {
                                        logger?.LogWarning("Monitoring: no time between reads.");
                                    }
                                    else
                                    {
                                        await Task.Delay(TimeSpan.FromSeconds(delay), token);
                                    }
                                }

                                if (!forever && (--options.Repeat <= 0))
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
                        catch
                        {
                            throw;
                        }
                    }
                    else
                    {
                        console.Out.WriteLine($"Modbus RTU slave not found at {options.SerialPort}.");
                        return ExitCodes.IncorrectFunction;
                    }
                }
                catch
                {
                    logger.LogError("RtuMonitorCommand exception");
                    throw;
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
        private void ReadingData(IRtuModbusClient client, IConsole console, ILogger<RtuMonitorCommand> logger,  RtuMonitorCommandOptions options, bool print = false)
        {
            logger?.LogDebug("TcpMonitor: Reading data...");

            // Reading coils.
            if (options.Coil)
            {
                if (options.Number == 1)
                {
                    if (print) console.Out.WriteLine($"Monitoring a single coil[{options.Offset}]");
                    bool[] values = client.ReadCoils(options.Offset, options.Number);
                    console.Out.WriteLine($"{DateTime.Now:HH:mm:ss.fff} Value of coil[{options.Offset}] = {values[0]}");
                }
                else
                {
                    if (print) console.Out.WriteLine($"Monitoring {options.Number} coils starting at {options.Offset}");
                    bool[] values = client.ReadCoils(options.Offset, options.Number);

                    for (int index = 0; index < values.Length; ++index)
                    {
                        console.Out.WriteLine($"{DateTime.Now:HH:mm:ss.fff}: Value of coil[{index}] = {values[index]}");
                    }

                    console.Out.WriteLine();
                }
            }

            // Reading discrete inputs.
            if (options.Discrete)
            {
                if (options.Number == 1)
                {
                    if (print) console.Out.WriteLine($"Monitoring a discrete input[{options.Offset}]");
                    bool[] values = client.ReadInputs(options.Offset, options.Number);
                    console.Out.WriteLine($"{DateTime.Now:HH:mm:ss.fff}: Value of discrete input[{options.Offset}] = {values[0]}");
                }
                else
                {
                    if (print) console.Out.WriteLine($"Monitoring {options.Number} discrete inputs starting at {options.Offset}");
                    bool[] values = client.ReadInputs(options.Offset, options.Number);

                    for (int index = 0; index < values.Length; ++index)
                    {
                        console.Out.WriteLine($"{DateTime.Now:HH:mm:ss.fff}: Value of discrete input[{index}] = {values[index]}");
                    }

                    console.Out.WriteLine();
                }
            }

            // Reading holding registers.
            if (options.Holding)
            {
                if (!string.IsNullOrEmpty(options.Type))
                {
                    switch (options.Type.ToLowerInvariant())
                    {
                        case "string":
                            {
                                if (options.Hex)
                                {
                                    if (print) console.Out.WriteLine($"Monitoring a HEX string from offset = {options.Offset}");
                                    string value = client.ReadHexString(options.Offset, options.Number);
                                    console.Out.WriteLine($"{DateTime.Now:HH:mm:ss.fff}: Value of HEX string = {value}");
                                }
                                else
                                {
                                    if (print) console.Out.WriteLine($"Monitoring an ASCII string from offset = {options.Offset}");
                                    string value = client.ReadString(options.Offset, options.Number);
                                    console.Out.WriteLine($"{DateTime.Now:HH:mm:ss.fff}: Value of ASCII string = {value}");
                                }

                                break;
                            }
                        case "bits":
                            {
                                if (print) console.Out.WriteLine($"Monitoring a 16 bit array from offset = {options.Offset}");
                                BitArray value = client.ReadBits(options.Offset);
                                console.Out.WriteLine($"{DateTime.Now:HH:mm:ss.fff}: Value of 16 bit array = {value.ToDigitString()}");
                                break;
                            }
                        case "byte":
                            {
                                if (options.Number == 1)
                                {
                                    if (print) console.Out.WriteLine($"Monitoring a single byte from offset = {options.Offset}");
                                    byte[] values = client.ReadBytes(options.Offset, options.Number);
                                    console.Out.WriteLine($"{DateTime.Now:HH:mm:ss.fff}: Value of single byte = {values[0]}");
                                }
                                else
                                {
                                    if (print) console.Out.WriteLine($"Monitoring {options.Number} bytes from offset = {options.Offset}");
                                    byte[] values = client.ReadBytes(options.Offset, options.Number);

                                    for (int index = 0; index < values.Length; ++index)
                                    {
                                        console.Out.WriteLine($"{DateTime.Now:HH:mm:ss.fff}: Value of byte array[{index}] = {values[index]}");
                                    }

                                    console.Out.WriteLine();
                                }

                                break;
                            }
                        case "short":
                            {
                                if (options.Number == 1)
                                {
                                    if (print) console.Out.WriteLine($"Monitoring a single short from offset = {options.Offset}");
                                    short value = client.ReadShort(options.Offset);
                                    console.Out.WriteLine($"{DateTime.Now:HH:mm:ss.fff}: Value of single short = {value}");
                                }
                                else
                                {
                                    if (print) console.Out.WriteLine($"Monitoring {options.Number} shorts from offset = {options.Offset}");
                                    short[] values = client.ReadShortArray(options.Offset, options.Number);

                                    for (int index = 0; index < values.Length; ++index)
                                    {
                                        console.Out.WriteLine($"{DateTime.Now:HH:mm:ss.fff}: Value of short array[{index}] = {values[index]}");
                                    }

                                    console.Out.WriteLine();
                                }

                                break;
                            }
                        case "ushort":
                            {
                                if (options.Number == 1)
                                {
                                    if (print) console.Out.WriteLine($"Monitoring a single ushort from offset = {options.Offset}");
                                    ushort value = client.ReadUShort(options.Offset);
                                    console.Out.WriteLine($"{DateTime.Now:HH:mm:ss.fff}: Value of single ushort = {value}");
                                }
                                else
                                {
                                    if (print) console.Out.WriteLine($"Monitoring {options.Number} ushorts from offset = {options.Offset}");
                                    ushort[] values = client.ReadUShortArray(options.Offset, options.Number);

                                    for (int index = 0; index < values.Length; ++index)
                                    {
                                        console.Out.WriteLine($"{DateTime.Now:HH:mm:ss.fff}: Value of ushort array[{index}] = {values[index]}");
                                    }

                                    console.Out.WriteLine();
                                }

                                break;
                            }
                        case "int":
                            {
                                if (options.Number == 1)
                                {
                                    if (print) console.Out.WriteLine($"Monitoring a single integer from offset = {options.Offset}");
                                    Int32 value = client.ReadInt32(options.Offset);
                                    console.Out.WriteLine($"{DateTime.Now:HH:mm:ss.fff}: Value of single integer = {value}");
                                }
                                else
                                {
                                    if (print) console.Out.WriteLine($"Monitoring {options.Number}  integers from offset = {options.Offset}");
                                    Int32[] values = client.ReadInt32Array(options.Offset, options.Number);

                                    for (int index = 0; index < values.Length; ++index)
                                    {
                                        console.Out.WriteLine($"{DateTime.Now:HH:mm:ss.fff}: Value of integer array[{index}] = {values[index]}");
                                    }

                                    console.Out.WriteLine();
                                }

                                break;
                            }
                        case "uint":
                            {
                                if (options.Number == 1)
                                {
                                    if (print) console.Out.WriteLine($"Monitoring a single unsigned integer from offset = {options.Offset}");
                                    UInt32 value = client.ReadUInt32(options.Offset);
                                    console.Out.WriteLine($"{DateTime.Now:HH:mm:ss.fff}: Value of single unsigned integer = {value}");
                                }
                                else
                                {
                                    if (print) console.Out.WriteLine($"Monitoring {options.Number} unsigned integers from offset = {options.Offset}");
                                    UInt32[] values = client.ReadUInt32Array(options.Offset, options.Number);

                                    for (int index = 0; index < values.Length; ++index)
                                    {
                                        console.Out.WriteLine($"{DateTime.Now:HH:mm:ss.fff}: Value of unsigned integer array[{index}] = {values[index]}");
                                    }

                                    console.Out.WriteLine();
                                }

                                break;
                            }
                        case "float":
                            {
                                if (options.Number == 1)
                                {
                                    if (print) console.Out.WriteLine($"Monitoring a single float from offset = {options.Offset}");
                                    float value = client.ReadFloat(options.Offset);
                                    console.Out.WriteLine($"{DateTime.Now:HH:mm:ss.fff}: Value of single float = {value}");
                                }
                                else
                                {
                                    if (print) console.Out.WriteLine($"Monitoring {options.Number} floats from offset = {options.Offset}");
                                    float[] values = client.ReadFloatArray(options.Offset, options.Number);

                                    for (int index = 0; index < values.Length; ++index)
                                    {
                                        console.Out.WriteLine($"{DateTime.Now:HH:mm:ss.fff}: Value of float array[{index}] = {values[index]}");
                                    }

                                    console.Out.WriteLine();
                                }

                                break;
                            }
                        case "double":
                            {
                                if (options.Number == 1)
                                {
                                    if (print) console.Out.WriteLine($"Monitoring a single double from offset = {options.Offset}");
                                    double value = client.ReadDouble(options.Offset);
                                    console.Out.WriteLine($"{DateTime.Now:HH:mm:ss.fff}: Value of single double = {value}");
                                }
                                else
                                {
                                    if (print) console.Out.WriteLine($"Monitoring {options.Number} doubles from offset = {options.Offset}");
                                    double[] values = client.ReadDoubleArray(options.Offset, options.Number);

                                    for (int index = 0; index < values.Length; ++index)
                                    {
                                        console.Out.WriteLine($"{DateTime.Now:HH:mm:ss.fff}: Value of double array[{index}] = {values[index]}");
                                    }

                                    console.Out.WriteLine();
                                }

                                break;
                            }
                        case "long":
                            {
                                if (options.Number == 1)
                                {
                                    if (print) console.Out.WriteLine($"Monitoring a single long from offset = {options.Offset}");
                                    long value = client.ReadLong(options.Offset);
                                    console.Out.WriteLine($"{DateTime.Now:HH:mm:ss.fff}: Value of single long = {value}");
                                }
                                else
                                {
                                    if (print) console.Out.WriteLine($"Monitoring {options.Number} longs from offset = {options.Offset}");
                                    long[] values = client.ReadLongArray(options.Offset, options.Number);

                                    for (int index = 0; index < values.Length; ++index)
                                    {
                                        console.Out.WriteLine($"{DateTime.Now:HH:mm:ss.fff}: Value of long array[{index}] = {values[index]}");
                                    }

                                    console.Out.WriteLine();
                                }

                                break;
                            }
                        case "ulong":
                            {
                                if (options.Number == 1)
                                {
                                    if (print) console.Out.WriteLine($"Monitoring a single ulong from offset = {options.Offset}");
                                    ulong value = client.ReadULong(options.Offset);
                                    console.Out.WriteLine($"{DateTime.Now:HH:mm:ss.fff}: Value of single ulong = {value}");
                                }
                                else
                                {
                                    if (print) console.Out.WriteLine($"Monitoring {options.Number} ulongs from offset = {options.Offset}");
                                    ulong[] values = client.ReadULongArray(options.Offset, options.Number);

                                    for (int index = 0; index < values.Length; ++index)
                                    {
                                        console.Out.WriteLine($"{DateTime.Now:HH:mm:ss.fff}: Value of ulong array[{index}] = {values[index]}");
                                    }

                                    console.Out.WriteLine();
                                }

                                break;
                            }
                    }
                }
                else if (options.Number == 1)
                {
                    if (print) console.Out.WriteLine($"Monitoring a holding register[{options.Offset}]");
                    ushort[] values = client.ReadHoldingRegisters(options.Offset, options.Number);
                    if (options.Hex) console.Out.WriteLine($"{DateTime.Now:HH:mm:ss.fff}: Value of holding register[{options.Offset}] = {values[0]:X2}");
                    else console.Out.WriteLine($"{DateTime.Now:HH:mm:ss.fff}: Value of holding register[{options.Offset}] = {values[0]}");
                }
                else
                {
                    if (print) console.Out.WriteLine($"Monitoring {options.Number} holding registers starting at {options.Offset}");
                    ushort[] values = client.ReadHoldingRegisters(options.Offset, options.Number);

                    for (int index = 0; index < values.Length; ++index)
                    {
                        if (options.Hex) console.Out.WriteLine($"{DateTime.Now:HH:mm:ss.fff}: Value of holding register[{index}] = {values[index]:X2}");
                        else console.Out.WriteLine($"{DateTime.Now:HH:mm:ss.fff}: Value of holding register[{index}] = {values[index]}");
                    }

                    console.Out.WriteLine();
                }
            }

            // Reading input registers.
            if (options.Input)
            {
                if (!string.IsNullOrEmpty(options.Type))
                {
                    switch (options.Type.ToLowerInvariant())
                    {
                        case "string":
                            {
                                if (options.Hex)
                                {
                                    if (print) console.Out.WriteLine($"Monitoring a HEX string from offset = {options.Offset}");
                                    string value = client.ReadOnlyHexString(options.Offset, options.Number);
                                    console.Out.WriteLine($"{DateTime.Now:HH:mm:ss.fff}: Value of HEX string = {value}");
                                }
                                else
                                {
                                    if (print) console.Out.WriteLine($"Monitoring an ASCII string from offset = {options.Offset}");
                                    string value = client.ReadOnlyString(options.Offset, options.Number);
                                    console.Out.WriteLine($"{DateTime.Now:HH:mm:ss.fff}: Value of ASCII string = {value}");
                                }

                                break;
                            }
                        case "bits":
                            {
                                if (print) console.Out.WriteLine($"Monitoring a 16 bit array from offset = {options.Offset}");
                                BitArray value = client.ReadOnlyBits(options.Offset);
                                console.Out.WriteLine($"{DateTime.Now:HH:mm:ss.fff}: Value of 16 bit array = {value.ToDigitString()}");
                                break;
                            }
                        case "byte":
                            {
                                if (options.Number == 1)
                                {
                                    if (print) console.Out.WriteLine($"Monitoring a single byte from offset = {options.Offset}");
                                    byte[] values = client.ReadOnlyBytes(options.Offset, options.Number);
                                    console.Out.WriteLine($"{DateTime.Now:HH:mm:ss.fff}: Value of single byte = {values[0]}");
                                }
                                else
                                {
                                    if (print) console.Out.WriteLine($"Monitoring {options.Number} bytes from offset = {options.Offset}");
                                    byte[] values = client.ReadOnlyBytes(options.Offset, options.Number);

                                    for (int index = 0; index < values.Length; ++index)
                                    {
                                        console.Out.WriteLine($"{DateTime.Now:HH:mm:ss.fff}: Value of byte array[{index}] = {values[index]}");
                                    }

                                    console.Out.WriteLine();
                                }

                                break;
                            }
                        case "short":
                            {
                                if (options.Number == 1)
                                {
                                    if (print) console.Out.WriteLine($"Monitoring a single short from offset = {options.Offset}");
                                    short value = client.ReadOnlyShort(options.Offset);
                                    console.Out.WriteLine($"{DateTime.Now:HH:mm:ss.fff}: Value of single short = {value}");
                                }
                                else
                                {
                                    if (print) console.Out.WriteLine($"Monitoring {options.Number} short values from offset = {options.Offset}");
                                    short[] values = client.ReadOnlyShortArray(options.Offset, options.Number);

                                    for (int index = 0; index < values.Length; ++index)
                                    {
                                        console.Out.WriteLine($"{DateTime.Now:HH:mm:ss.fff}: Value of short array[{index}] = {values[index]}");
                                    }

                                    console.Out.WriteLine();
                                }

                                break;
                            }
                        case "ushort":
                            {
                                if (options.Number == 1)
                                {
                                    if (print) console.Out.WriteLine($"Monitoring a single ushort from offset = {options.Offset}");
                                    ushort value = client.ReadOnlyUShort(options.Offset);
                                    console.Out.WriteLine($"{DateTime.Now:HH:mm:ss.fff}: Value of single ushort = {value}");
                                }
                                else
                                {
                                    if (print) console.Out.WriteLine($"Monitoring {options.Number} ushort values from offset = {options.Offset}");
                                    ushort[] values = client.ReadOnlyUShortArray(options.Offset, options.Number);

                                    for (int index = 0; index < values.Length; ++index)
                                    {
                                        console.Out.WriteLine($"{DateTime.Now:HH:mm:ss.fff}: Value of ushort array[{index}] = {values[index]}");
                                    }

                                    console.Out.WriteLine();
                                }

                                break;
                            }
                        case "int":
                            {
                                if (options.Number == 1)
                                {
                                    if (print) console.Out.WriteLine($"Monitoring a single int from offset = {options.Offset}");
                                    Int32 value = client.ReadOnlyInt32(options.Offset);
                                    console.Out.WriteLine($"{DateTime.Now:HH:mm:ss.fff}: Value of single integer = {value}");
                                }
                                else
                                {
                                    if (print) console.Out.WriteLine($"Monitoring {options.Number} int values from offset = {options.Offset}");
                                    Int32[] values = client.ReadOnlyInt32Array(options.Offset, options.Number);

                                    for (int index = 0; index < values.Length; ++index)
                                    {
                                        console.Out.WriteLine($"{DateTime.Now:HH:mm:ss.fff}: Value of int array[{index}] = {values[index]}");
                                    }

                                    console.Out.WriteLine();
                                }

                                break;
                            }
                        case "uint":
                            {
                                if (options.Number == 1)
                                {
                                    if (print) console.Out.WriteLine($"Monitoring a single unsigned int from offset = {options.Offset}");
                                    UInt32 value = client.ReadOnlyUInt32(options.Offset);
                                    console.Out.WriteLine($"{DateTime.Now:HH:mm:ss.fff}: Value of single unsigned int = {value}");
                                }
                                else
                                {
                                    if (print) console.Out.WriteLine($"Monitoring {options.Number} unsigned int values from offset = {options.Offset}");
                                    UInt32[] values = client.ReadOnlyUInt32Array(options.Offset, options.Number);

                                    for (int index = 0; index < values.Length; ++index)
                                    {
                                        console.Out.WriteLine($"{DateTime.Now:HH:mm:ss.fff}: Value of unsigned int array[{index}] = {values[index]}");
                                    }

                                    console.Out.WriteLine();
                                }

                                break;
                            }
                        case "float":
                            {
                                if (options.Number == 1)
                                {
                                    if (print) console.Out.WriteLine($"Monitoring a single float from offset = {options.Offset}");
                                    float value = client.ReadOnlyFloat(options.Offset);
                                    console.Out.WriteLine($"{DateTime.Now:HH:mm:ss.fff}: Value of single float = {value}");
                                }
                                else
                                {
                                    if (print) console.Out.WriteLine($"Monitoring {options.Number} float values from offset = {options.Offset}");
                                    float[] values = client.ReadOnlyFloatArray(options.Offset, options.Number);

                                    for (int index = 0; index < values.Length; ++index)
                                    {
                                        console.Out.WriteLine($"{DateTime.Now:HH:mm:ss.fff}: Value of float array[{index}] = {values[index]}");
                                    }

                                    console.Out.WriteLine();
                                }

                                break;
                            }
                        case "double":
                            {
                                if (options.Number == 1)
                                {
                                    if (print) console.Out.WriteLine($"Monitoring a single double from offset = {options.Offset}");
                                    double value = client.ReadOnlyDouble(options.Offset);
                                    console.Out.WriteLine($"{DateTime.Now:HH:mm:ss.fff}: Value of single double = {value}");
                                }
                                else
                                {
                                    if (print) console.Out.WriteLine($"Monitoring {options.Number} double values from offset = {options.Offset}");
                                    double[] values = client.ReadOnlyDoubleArray(options.Offset, options.Number);

                                    for (int index = 0; index < values.Length; ++index)
                                    {
                                        console.Out.WriteLine($"{DateTime.Now:HH:mm:ss.fff}: Value of double array[{index}] = {values[index]}");
                                    }

                                    console.Out.WriteLine();
                                }

                                break;
                            }
                        case "long":
                            {
                                if (options.Number == 1)
                                {
                                    if (print) console.Out.WriteLine($"Monitoring a single long from offset = {options.Offset}");
                                    long value = client.ReadOnlyLong(options.Offset);
                                    console.Out.WriteLine($"{DateTime.Now:HH:mm:ss.fff}: Value of single long = {value}");
                                }
                                else
                                {
                                    if (print) console.Out.WriteLine($"Monitoring {options.Number} long values from offset = {options.Offset}");
                                    long[] values = client.ReadOnlyLongArray(options.Offset, options.Number);

                                    for (int index = 0; index < values.Length; ++index)
                                    {
                                        console.Out.WriteLine($"{DateTime.Now:HH:mm:ss.fff}: Value of long array[{index}] = {values[index]}");
                                    }

                                    console.Out.WriteLine();
                                }

                                break;
                            }
                        case "ulong":
                            {
                                if (options.Number == 1)
                                {
                                    if (print) console.Out.WriteLine($"Monitoring a single unsigned long from offset = {options.Offset}");
                                    ulong value = client.ReadOnlyULong(options.Offset);
                                    console.Out.WriteLine($"{DateTime.Now:HH:mm:ss.fff}: Value of single ulong = {value}");
                                }
                                else
                                {
                                    if (print) console.Out.WriteLine($"Monitoring {options.Number} unsigned long values from offset = {options.Offset}");
                                    ulong[] values = client.ReadOnlyULongArray(options.Offset, options.Number);

                                    for (int index = 0; index < values.Length; ++index)
                                    {
                                        console.Out.WriteLine($"{DateTime.Now:HH:mm:ss.fff}: Value of ulong array[{index}] = {values[index]}");
                                    }

                                    console.Out.WriteLine();
                                }

                                break;
                            }
                    }
                }
                else
                {
                    if (options.Number == 1)
                    {
                        if (print) console.Out.WriteLine($"Monitoring a input register[{options.Offset}]");
                        ushort[] values = client.ReadInputRegisters(options.Offset, options.Number);
                        if (options.Hex) console.Out.WriteLine($"{DateTime.Now:HH:mm:ss.fff}: Value of input register[{options.Offset}] = {values[0]:X2}");
                        else console.Out.WriteLine($"{DateTime.Now:HH:mm:ss.fff}: Value of input register[{options.Offset}] = {values[0]}");
                    }
                    else
                    {
                        if (print) console.Out.WriteLine($"Monitoring {options.Number} input registers starting at {options.Offset}");
                        ushort[] values = client.ReadInputRegisters(options.Offset, options.Number);

                        for (int index = 0; index < values.Length; ++index)
                        {
                            if (options.Hex) console.Out.WriteLine($"{DateTime.Now:HH:mm:ss.fff}: Value of input register[{index}] = {values[index]:X2}");
                            else console.Out.WriteLine($"{DateTime.Now:HH:mm:ss.fff}: Value of input register[{index}] = {values[index]}");
                        }

                        console.Out.WriteLine();
                    }
                }
            }
        }

        #endregion
    }
}
