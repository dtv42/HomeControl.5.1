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

    using System.CommandLine;
    using System.CommandLine.IO;
    using System.CommandLine.Invocation;
    using System.Text.Json;

    using Microsoft.Extensions.Logging;

    using NModbus.Extensions;

    using UtilityLib;
    using ModbusLib;
    using ModbusLib.Models;
    using ModbusApp.Models;
    using System.Collections.Generic;

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
            AddArgument(new Argument<string>("Values", "Data values (JSON array format).").Default("[]"));
            AddOption(new Option<bool>(new string[] { "-?", "--help" }, "Show help and usage information"));
            AddOption(new Option<bool>(new string[] { "-c", "--coil" }, "Write coil(s)."));
            AddOption(new Option<bool>(new string[] { "-h", "--holding" }, "Writes holding register(s)."));
            AddOption(new Option<ushort>(new string[] { "-o", "--offset" }, "The offset of the first item.").Name("Offset").Default(0));
            AddOption(new Option<string>(new string[] { "-t", "--type" }, "Reads the specified data type").Name("Type")
                .FromAmong("bits", "string", "byte", "short", "ushort", "int", "uint", "float", "double", "long", "ulong"));

            // Add custom validation.
            AddValidator(r =>
            {
                if (r.Children.Contains("-?")) return "Specify a single write option (coils or holding registers).";

                var optionC = r.Children.Contains("-c");
                var optionH = r.Children.Contains("-h");
                var optionN = r.Children.Contains("-n");
                var optionO = r.Children.Contains("-o");
                var optionT = r.Children.Contains("-t");

                if ((!optionC && !optionH) || (optionC && optionH))
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
                        if (options.Coil)
                        {
                            List<bool>? values = JsonSerializer.Deserialize<List<bool>>(options.Values);

                            if (!(values is null))
                            {
                                if (values.Count == 0)
                                {
                                    logger.LogWarning($"No values specified.");
                                }
                                else
                                {
                                    if (values.Count == 1)
                                    {
                                        console.Out.WriteLine($"Write single coil[{options.Offset}] = {values[0]}");
                                        client.WriteSingleCoil(options.Offset, values[0]);
                                    }
                                    else
                                    {
                                        console.Out.WriteLine($"Writing {values.Count} coils starting at {options.Offset}");

                                        for (int index = 0; index < values.Count; ++index)
                                            console.Out.WriteLine($"Value of coil[{options.Offset + index}] = {values[index]}");

                                        client.WriteMultipleCoils(options.Offset, values.ToArray());
                                    }
                                }
                            }
                        }

                        // Writing holding registers.
                        if (options.Holding)
                        {
                            if (!string.IsNullOrEmpty(options.Type))
                            {
                                switch (options.Type.ToLowerInvariant())
                                {
                                    case "string":
                                        {
                                            console.Out.WriteLine($"Writing an ASCII string at offset = {options.Offset}");
                                            client.WriteString(options.Offset, options.Values);
                                            break;
                                        }
                                    case "bits":
                                        {
                                            console.Out.WriteLine($"Writing a 16 bit array at offset = {options.Offset}");
                                            client.WriteBits(options.Offset, options.Values.ToBitArray());
                                            break;
                                        }
                                    case "byte":
                                        {
                                            List<byte>? bytes = JsonSerializer.Deserialize<List<byte>>(options.Values);

                                            if (!(bytes is null))
                                            {
                                                console.Out.WriteLine($"Writing {bytes.Count} bytes at offset = {options.Offset}");
                                                client.WriteBytes(options.Offset, bytes.ToArray());
                                            }

                                            break;
                                        }
                                    case "short":
                                        {
                                            List<short>? values = JsonSerializer.Deserialize<List<short>>(options.Values);

                                            if (!(values is null))
                                            {
                                                if (values.Count == 1)
                                                {
                                                    console.Out.WriteLine($"Writing a single short value at offset = {options.Offset}");
                                                    client.WriteShort(options.Offset, values[0]);
                                                }
                                                else
                                                {
                                                    console.Out.WriteLine($"Writing {values.Count} short values at offset = {options.Offset}");
                                                    client.WriteShortArray(options.Offset, values.ToArray());
                                                }
                                            }

                                            break;
                                        }
                                    case "ushort":
                                        {
                                            List<ushort>? values = JsonSerializer.Deserialize<List<ushort>>(options.Values);

                                            if (!(values is null))
                                            {
                                                if (values.Count == 1)
                                                {
                                                    console.Out.WriteLine($"Writing a single unsigned short value at offset = {options.Offset}");
                                                    client.WriteUShort(options.Offset, values[0]);
                                                }
                                                else
                                                {
                                                    console.Out.WriteLine($"Writing {values.Count} unsigned short values at offset = {options.Offset}");
                                                    client.WriteUShortArray(options.Offset, values.ToArray());
                                                }
                                            }

                                            break;
                                        }
                                    case "int":
                                        {
                                            List<int>? values = JsonSerializer.Deserialize<List<int>>(options.Values);

                                            if (!(values is null))
                                            {
                                                if (values.Count == 1)
                                                {
                                                    console.Out.WriteLine($"Writing a single int value at offset = {options.Offset}");
                                                    client.WriteInt32(options.Offset, values[0]);
                                                }
                                                else
                                                {
                                                    console.Out.WriteLine($"Writing {values.Count} int values at offset = {options.Offset}");
                                                    client.WriteInt32Array(options.Offset, values.ToArray());
                                                }
                                            }

                                            break;
                                        }
                                    case "uint":
                                        {
                                            List<uint>? values = JsonSerializer.Deserialize<List<uint>>(options.Values);

                                            if (!(values is null))
                                            {
                                                if (values.Count == 1)
                                                {
                                                    console.Out.WriteLine($"Writing a single unsigned int value at offset = {options.Offset}");
                                                    client.WriteUInt32(options.Offset, values[0]);
                                                }
                                                else
                                                {
                                                    console.Out.WriteLine($"Writing {values.Count} unsigned int values at offset = {options.Offset}");
                                                    client.WriteUInt32Array(options.Offset, values.ToArray());
                                                }
                                            }

                                            break;
                                        }
                                    case "float":
                                        {
                                            List<float>? values = JsonSerializer.Deserialize<List<float>>(options.Values);

                                            if (!(values is null))
                                            {
                                                if (values.Count == 1)
                                                {
                                                    console.Out.WriteLine($"Writing a single float value at offset = {options.Offset}");
                                                    client.WriteFloat(options.Offset, values[0]);
                                                }
                                                else
                                                {
                                                    console.Out.WriteLine($"Writing {values.Count} float values at offset = {options.Offset}");
                                                    client.WriteFloatArray(options.Offset, values.ToArray());
                                                }
                                            }

                                            break;
                                        }
                                    case "double":
                                        {
                                            List<double>? values = JsonSerializer.Deserialize<List<double>>(options.Values);

                                            if (!(values is null))
                                            {
                                                if (values.Count == 1)
                                                {
                                                    console.Out.WriteLine($"Writing a single double value at offset = {options.Offset}");
                                                    client.WriteDouble(options.Offset, values[0]);
                                                }
                                                else
                                                {
                                                    console.Out.WriteLine($"Writing {values.Count} double values at offset = {options.Offset}");
                                                    client.WriteDoubleArray(options.Offset, values.ToArray());
                                                }
                                            }

                                            break;
                                        }
                                    case "long":
                                        {
                                            List<long>? values = JsonSerializer.Deserialize<List<long>>(options.Values);

                                            if (!(values is null))
                                            {
                                                if (values.Count == 1)
                                                {
                                                    console.Out.WriteLine($"Writing a single long value at offset = {options.Offset}");
                                                    client.WriteLong(options.Offset, values[0]);
                                                }
                                                else
                                                {
                                                    console.Out.WriteLine($"Writing {values.Count} long values at offset = {options.Offset}");
                                                    client.WriteLongArray(options.Offset, values.ToArray());
                                                }
                                            }

                                            break;
                                        }
                                    case "ulong":
                                        {
                                            List<ulong>? values = JsonSerializer.Deserialize<List<ulong>>(options.Values);

                                            if (!(values is null))
                                            {
                                                if (values.Count == 1)
                                                {
                                                    console.Out.WriteLine($"Writing a single unsigned long value at offset = {options.Offset}");
                                                    client.WriteULong(options.Offset, values[0]);
                                                }
                                                else
                                                {
                                                    console.Out.WriteLine($"Writing {values.Count} unsigned long values at offset = {options.Offset}");
                                                    client.WriteULongArray(options.Offset, values.ToArray());
                                                }
                                            }

                                            break;
                                        }
                                }
                            }
                            else
                            {
                                List<ushort>? values = JsonSerializer.Deserialize<List<ushort>>(options.Values);

                                if (!(values is null))
                                {
                                    if (values.Count == 0)
                                    {
                                        logger.LogWarning($"No values specified.");
                                    }
                                    else
                                    {
                                        if (values.Count == 1)
                                        {
                                            console.Out.WriteLine($"Writing single holding register[{options.Offset}] = {values[0]}");
                                            client.WriteSingleRegister(options.Offset, values[0]);
                                        }
                                        else
                                        {
                                            console.Out.WriteLine($"Writing {values.Count} holding registers starting at {options.Offset}");

                                            for (int index = 0; index < values.Count; ++index)
                                                console.Out.WriteLine($"Value of holding register[{options.Offset + index}] = {values[index]}");

                                            client.WriteMultipleRegisters(options.Offset, values.ToArray());
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        console.Out.WriteLine($"Modbus TCP slave not found at {options.Address}:{options.Port}.");
                        return ExitCodes.IncorrectFunction;
                    }
                }
                catch (JsonException jex)
                {
                    logger.LogError(jex, $"Exception parsing JSON data values.");
                    return ExitCodes.NotSuccessfullyCompleted;
                }
                catch
                {
                    logger.LogError("TcpWriteCommand exception");
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
    }
}
