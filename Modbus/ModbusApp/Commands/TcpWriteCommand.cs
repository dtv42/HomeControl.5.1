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

    using System.ComponentModel.DataAnnotations;
    using System.Collections.Generic;
    using System.Text.Json;

    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;

    using McMaster.Extensions.CommandLineUtils;
    using NModbus.Extensions;

    using UtilityLib;
    using ModbusLib;
    using ModbusLib.Models;
    using ModbusApp.Models;

    #endregion

    [Command(Name = "write",
             FullName = "NModbusApp TCP Write Command",
             Description = "Supporting Modbus TCP write operations.",
             ExtendedHelpText = "\nPlease specify the write option (coils or holding registers).")]
    public class TcpWriteCommand : BaseCommand<TcpWriteCommand, AppSettings>
    {
        #region Private Data Members

        private readonly JsonSerializerOptions _options = JsonExtensions.DefaultSerializerOptions;
        private readonly ITcpModbusClient _client;

        #endregion

        #region Private Properties

        /// <summary>
        /// This is a reference to the parent command <see cref="TcpCommand"/>.
        /// </summary>
        private TcpCommand? Parent { get; }

        #endregion

        #region Public Properties

        [Required]
        [Argument(0, Description = "Data values (JSON array format).")]
        public string Values { get; } = "[]";

        [Option("-c|--coil", Description = "Writes coil(s).")]
        public bool OptionC { get; }

        [Option("-h|--holding", Description = "Writes holding register(s).")]
        public bool OptionH { get; }

        [Option(Description = "The offset of the first item to write.")]
        public ushort Offset { get; set; } = 0;

        [Option(Description = "Writes the specified data type")]
        [AllowedValues("bits", "string", "byte", "short", "ushort", "int", "uint", "float", "double", "long", "ulong", IgnoreCase = true)]
        public (bool HasValue, string Value) Type { get; set; } = (false, string.Empty);

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TcpWriteCommand"/> class.
        /// Selected properties are initialized with data from the AppSettings instance.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="console"></param>
        /// <param name="settings"></param>
        /// <param name="config"></param>
        /// <param name="environment"></param>
        /// <param name="lifetime"></param>
        /// <param name="logger"></param>
        /// <param name="application"></param>
        public TcpWriteCommand(ITcpModbusClient client,
                               IConsole console,
                               AppSettings settings,
                               IConfiguration config,
                               IHostEnvironment environment,
                               IHostApplicationLifetime lifetime,
                               ILogger<TcpWriteCommand> logger,
                               CommandLineApplication application)
            : base(console, settings, config, environment, lifetime, logger, application)
        {
            // Setting the TCP client instance.
            _client = client;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Runs when the commandline application command is executed.
        /// </summary>
        /// <returns>The exit code</returns>
        public int OnExecute()
        {
            try
            {
                if (!(Parent is null))
                {
                    // Overriding TCP client options.
                    _client.TcpMaster.ReceiveTimeout = Parent.ReceiveTimeout;
                    _client.TcpMaster.SendTimeout = Parent.SendTimeout;
                    _client.TcpSlave.Address = Parent.Address;
                    _client.TcpSlave.Port = Parent.Port;
                    _client.TcpSlave.ID = Parent.SlaveID;
                }

                if (Parent?.Parent?.ShowSettings ?? false)
                {
                    _console.WriteLine(JsonSerializer.Serialize<TcpMasterData>(_settings.TcpMaster, _options));
                    _console.WriteLine(JsonSerializer.Serialize<TcpSlaveData>(_settings.TcpSlave, _options));
                }

                if (_client.Connect())
                {
                    // Writing coils.
                    if (OptionC)
                    {
                        List<bool>? values = JsonSerializer.Deserialize<List<bool>>(Values);

                        if (!(values is null))
                        {
                            if (values.Count == 0)
                            {
                                _logger.LogWarning($"No values specified.");
                            }
                            else
                            {
                                if (values.Count == 1)
                                {
                                    _console.WriteLine($"Write single coil[{Offset}] = {values[0]}");
                                    _client.WriteSingleCoil(Offset, values[0]);
                                }
                                else
                                {
                                    _console.WriteLine($"Writing {values.Count} coils starting at {Offset}");

                                    for (int index = 0; index < values.Count; ++index)
                                        _console.WriteLine($"Value of coil[{Offset + index}] = {values[index]}");

                                    _client.WriteMultipleCoils(Offset, values.ToArray());
                                }
                            }
                        }
                    }

                    // Writing holding registers.
                    if (OptionH)
                    {
                        if (Type.HasValue)
                        {
                            switch (Type.Value.ToLowerInvariant())
                            {
                                case "string":
                                    {
                                        _console.WriteLine($"Writing an ASCII string at offset = {Offset}");
                                        _client.WriteString(Offset, Values);
                                        break;
                                    }
                                case "bits":
                                    {
                                        _console.WriteLine($"Writing a 16 bit array at offset = {Offset}");
                                        _client.WriteBits(Offset, Values.ToBitArray());
                                        break;
                                    }
                                case "byte":
                                    {
                                        List<byte>? bytes = JsonSerializer.Deserialize<List<byte>>(Values);

                                        if (!(bytes is null))
                                        {
                                            _console.WriteLine($"Writing {bytes.Count} bytes at offset = {Offset}");
                                            _client.WriteBytes(Offset, bytes.ToArray());
                                        }

                                        break;
                                    }
                                case "short":
                                    {
                                        List<short>? values = JsonSerializer.Deserialize<List<short>>(Values);

                                        if (!(values is null))
                                        {
                                            if (values.Count == 1)
                                            {
                                                _console.WriteLine($"Writing a single short value at offset = {Offset}");
                                                _client.WriteShort(Offset, values[0]);
                                            }
                                            else
                                            {
                                                _console.WriteLine($"Writing {values.Count} short values at offset = {Offset}");
                                                _client.WriteShortArray(Offset, values.ToArray());
                                            }
                                        }

                                        break;
                                    }
                                case "ushort":
                                    {
                                        List<ushort>? values = JsonSerializer.Deserialize<List<ushort>>(Values);

                                        if (!(values is null))
                                        {
                                            if (values.Count == 1)
                                            {
                                                _console.WriteLine($"Writing a single unsigned short value at offset = {Offset}");
                                                _client.WriteUShort(Offset, values[0]);
                                            }
                                            else
                                            {
                                                _console.WriteLine($"Writing {values.Count} unsigned short values at offset = {Offset}");
                                                _client.WriteUShortArray(Offset, values.ToArray());
                                            }
                                        }

                                        break;
                                    }
                                case "int":
                                    {
                                        List<int>? values = JsonSerializer.Deserialize<List<int>>(Values);

                                        if (!(values is null))
                                        {
                                            if (values.Count == 1)
                                            {
                                                _console.WriteLine($"Writing a single int value at offset = {Offset}");
                                                _client.WriteInt32(Offset, values[0]);
                                            }
                                            else
                                            {
                                                _console.WriteLine($"Writing {values.Count} int values at offset = {Offset}");
                                                _client.WriteInt32Array(Offset, values.ToArray());
                                            }
                                        }

                                        break;
                                    }
                                case "uint":
                                    {
                                        List<uint>? values = JsonSerializer.Deserialize<List<uint>>(Values);

                                        if (!(values is null))
                                        {
                                            if (values.Count == 1)
                                            {
                                                _console.WriteLine($"Writing a single unsigned int value at offset = {Offset}");
                                                _client.WriteUInt32(Offset, values[0]);
                                            }
                                            else
                                            {
                                                _console.WriteLine($"Writing {values.Count} unsigned int values at offset = {Offset}");
                                                _client.WriteUInt32Array(Offset, values.ToArray());
                                            }
                                        }

                                        break;
                                    }
                                case "float":
                                    {
                                        List<float>? values = JsonSerializer.Deserialize<List<float>>(Values);

                                        if (!(values is null))
                                        {
                                            if (values.Count == 1)
                                            {
                                                _console.WriteLine($"Writing a single float value at offset = {Offset}");
                                                _client.WriteFloat(Offset, values[0]);
                                            }
                                            else
                                            {
                                                _console.WriteLine($"Writing {values.Count} float values at offset = {Offset}");
                                                _client.WriteFloatArray(Offset, values.ToArray());
                                            }
                                        }

                                        break;
                                    }
                                case "double":
                                    {
                                        List<double>? values = JsonSerializer.Deserialize<List<double>>(Values);

                                        if (!(values is null))
                                        {
                                            if (values.Count == 1)
                                            {
                                                _console.WriteLine($"Writing a single double value at offset = {Offset}");
                                                _client.WriteDouble(Offset, values[0]);
                                            }
                                            else
                                            {
                                                _console.WriteLine($"Writing {values.Count} double values at offset = {Offset}");
                                                _client.WriteDoubleArray(Offset, values.ToArray());
                                            }
                                        }

                                        break;
                                    }
                                case "long":
                                    {
                                        List<long>? values = JsonSerializer.Deserialize<List<long>>(Values);

                                        if (!(values is null))
                                        {
                                            if (values.Count == 1)
                                            {
                                                _console.WriteLine($"Writing a single long value at offset = {Offset}");
                                                _client.WriteLong(Offset, values[0]);
                                            }
                                            else
                                            {
                                                _console.WriteLine($"Writing {values.Count} long values at offset = {Offset}");
                                                _client.WriteLongArray(Offset, values.ToArray());
                                            }
                                        }

                                        break;
                                    }
                                case "ulong":
                                    {
                                        List<ulong>? values = JsonSerializer.Deserialize<List<ulong>>(Values);

                                        if (!(values is null))
                                        {
                                            if (values.Count == 1)
                                            {
                                                _console.WriteLine($"Writing a single unsigned long value at offset = {Offset}");
                                                _client.WriteULong(Offset, values[0]);
                                            }
                                            else
                                            {
                                                _console.WriteLine($"Writing {values.Count} unsigned long values at offset = {Offset}");
                                                _client.WriteULongArray(Offset, values.ToArray());
                                            }
                                        }

                                        break;
                                    }
                            }
                        }
                        else
                        {
                            List<ushort>? values = JsonSerializer.Deserialize<List<ushort>>(Values);

                            if (!(values is null))
                            {
                                if (values.Count == 0)
                                {
                                    _logger.LogWarning($"No values specified.");
                                }
                                else
                                {
                                    if (values.Count == 1)
                                    {
                                        _console.WriteLine($"Writing single holding register[{Offset}] = {values[0]}");
                                        _client.WriteSingleRegister(Offset, values[0]);
                                    }
                                    else
                                    {
                                        _console.WriteLine($"Writing {values.Count} holding registers starting at {Offset}");

                                        for (int index = 0; index < values.Count; ++index)
                                            _console.WriteLine($"Value of holding register[{Offset + index}] = {values[index]}");

                                        _client.WriteMultipleRegisters(Offset, values.ToArray());
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    _console.WriteLine($"Modbus TCP slave not found at {_client.TcpSlave.Address}:{_client.TcpSlave.Port}.");
                    return ExitCodes.IncorrectFunction;
                }
            }
            catch (JsonException jex)
            {
                _logger.LogError(jex, $"Exception parsing JSON data values.");
                return ExitCodes.NotSuccessfullyCompleted;
            }
            catch
            {
                _logger.LogError("TcpWriteCommand exception");
                throw;
            }
            finally
            {
                if (_client.Connected)
                {
                    _client.Disconnect();
                }
            }

            return ExitCodes.SuccessfullyCompleted;
        }

        /// <summary>
        /// Helper method to check options.
        /// </summary>
        /// <returns>True if options are OK.</returns>
        public override bool CheckOptions()
        {
            if (Parent?.CheckOptions() ?? false)
            {
                if (!OptionC && !OptionH)
                {
                    throw new CommandParsingException(_application, $"Specify the write option (coils or holding registers).");
                }

                if (OptionC && OptionH)
                {
                    throw new CommandParsingException(_application, $"Specify only a single write option (coils or holding registers).");
                }

                if (Type.HasValue && OptionC)
                {
                    _console.WriteLine($"Specified type '{Type.Value}' is ignored.");
                }
            }
            else
            {
                return false;
            }

            return true;
        }

        #endregion
    }
}
