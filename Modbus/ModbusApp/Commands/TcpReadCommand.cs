// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TcpReadCommand.cs" company="DTV-Online">
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
    using System.Globalization;
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

    [Command(Name = "read",
             FullName = "ModbusApp TCP Read Command",
             Description = "Supporting Modbus TCP read operations.",
             ExtendedHelpText = "\nPlease specify the read option (coils, discrete inputs, holding registers, or input registers).")]
    public class TcpReadCommand : BaseCommand<TcpReadCommand, AppSettings>
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

        [Option("-c|--coil", Description = "Reads coil(s).")]
        public bool OptionC { get; }

        [Option("-d|--discrete", Description = "Reads discrete input(s).")]
        public bool OptionD { get; }

        [Option("-h|--holding", Description = "Reads holding register(s).")]
        public bool OptionH { get; }

        [Option("-i|--input", Description = "Reads input register(s).")]
        public bool OptionI { get; }

        [Option("-x|--hex", Description = "Displays the values in HEX.")]
        public bool OptionX { get; }

        [Option(Description = "The number of items to read (default: 1).")]
        public ushort Number { get; set; } = 1;

        [Option(Description = "The offset of the first item to read  (default: 0).")]
        public ushort Offset { get; set; } = 0;

        [Option(Description = "Reads the specified data type")]
        [AllowedValues("bits", "string", "byte", "short", "ushort", "int", "uint", "float", "double", "long", "ulong", IgnoreCase = true)]
        public (bool HasValue, string Value) Type { get; set; } = (false, string.Empty);

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TcpReadCommand"/> class.
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
        public TcpReadCommand(ITcpModbusClient client,
                              IConsole console,
                              AppSettings settings,
                              IConfiguration config,
                              IHostEnvironment environment,
                              IHostApplicationLifetime lifetime,
                              ILogger<TcpReadCommand> logger,
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
                    // Reading coils.
                    if (OptionC)
                    {
                        if (Number == 1)
                        {
                            _console.WriteLine($"Reading a single coil[{Offset}]");
                            bool[] values = _client.ReadCoils(Offset, Number);
                            _console.WriteLine($"Value of coil[{Offset}] = {values[0]}");
                        }
                        else
                        {
                            _console.WriteLine($"Reading {Number} coils starting at {Offset}");
                            bool[] values = _client.ReadCoils(Offset, Number);

                            for (int index = 0; index < values.Length; ++index)
                            {
                                _console.WriteLine($"Value of coil[{Offset + index}] = {values[index]}");
                            }
                        }
                    }

                    // Reading discrete inputs.
                    if (OptionD)
                    {
                        if (Number == 1)
                        {
                            _console.WriteLine($"Reading a discrete input[{Offset}]");
                            bool[] values = _client.ReadInputs(Offset, Number);
                            _console.WriteLine($"Value of discrete input[{Offset}] = {values[0]}");
                        }
                        else
                        {
                            _console.WriteLine($"Reading {Number} discrete inputs starting at {Offset}");
                            bool[] values = _client.ReadInputs(Offset, Number);

                            for (int index = 0; index < values.Length; ++index)
                            {
                                _console.WriteLine($"Value of discrete input[{Offset + index}] = {values[index]}");
                            }
                        }
                    }

                    // Reading holding registers.
                    if (OptionH)
                    {
                        if (Type.HasValue)
                        {
                            switch (Type.Value.ToLowerInvariant())
                            {
                                case "string":
                                    {
                                        if (OptionX)
                                        {
                                            _console.WriteLine($"Reading a HEX string from offset = {Offset}");
                                            string value = _client.ReadHexString(Offset, Number);
                                            _console.WriteLine($"Value of HEX string = {value}");
                                        }
                                        else
                                        {
                                            _console.WriteLine($"Reading an ASCII string from offset = {Offset}");
                                            string value = _client.ReadString(Offset, Number);
                                            _console.WriteLine($"Value of ASCII string = {value}");
                                        }

                                        break;
                                    }
                                case "bits":
                                    {
                                        _console.WriteLine($"Reading a 16 bit array from offset = {Offset}");
                                        BitArray value = _client.ReadBits(Offset);
                                        _console.WriteLine($"Value of 16 bit array = {value.ToDigitString()}");
                                        break;
                                    }
                                case "byte":
                                    {
                                        if (Number == 1)
                                        {
                                            _console.WriteLine($"Reading a single byte from offset = {Offset}");
                                            byte[] values = _client.ReadBytes(Offset, Number);
                                            _console.WriteLine($"Value of single byte = {values[0]}");
                                        }
                                        else
                                        {
                                            _console.WriteLine($"Reading {Number} bytes from offset = {Offset}");
                                            byte[] values = _client.ReadBytes(Offset, Number);

                                            for (int index = 0; index < values.Length; ++index)
                                            {
                                                _console.WriteLine($"Value of byte array[{index}] = {values[index]}");
                                            }
                                        }

                                        break;
                                    }
                                case "short":
                                    {
                                        if (Number == 1)
                                        {
                                            _console.WriteLine($"Reading a single short from offset = {Offset}");
                                            short value = _client.ReadShort(Offset);
                                            _console.WriteLine($"Value of single short = {value}");
                                        }
                                        else
                                        {
                                            _console.WriteLine($"Reading {Number} shorts from offset = {Offset}");
                                            short[] values = _client.ReadShortArray(Offset, Number);

                                            for (int index = 0; index < values.Length; ++index)
                                            {
                                                _console.WriteLine($"Value of short array[{index}] = {values[index]}");
                                            }
                                        }

                                        break;
                                    }
                                case "ushort":
                                    {
                                        if (Number == 1)
                                        {
                                            _console.WriteLine($"Reading a single ushort from offset = {Offset}");
                                            ushort value = _client.ReadUShort(Offset);
                                            _console.WriteLine($"Value of single ushort = {value}");
                                        }
                                        else
                                        {
                                            _console.WriteLine($"Reading {Number} ushorts from offset = {Offset}");
                                            ushort[] values = _client.ReadUShortArray(Offset, Number);

                                            for (int index = 0; index < values.Length; ++index)
                                            {
                                                _console.WriteLine($"Value of ushort array[{index}] = {values[index]}");
                                            }
                                        }

                                        break;
                                    }
                                case "int":
                                    {
                                        if (Number == 1)
                                        {
                                            _console.WriteLine($"Reading a single integer from offset = {Offset}");
                                            Int32 value = _client.ReadInt32(Offset);
                                            _console.WriteLine($"Value of single integer = {value}");
                                        }
                                        else
                                        {
                                            _console.WriteLine($"Reading {Number}  integers from offset = {Offset}");
                                            Int32[] values = _client.ReadInt32Array(Offset, Number);

                                            for (int index = 0; index < values.Length; ++index)
                                            {
                                                _console.WriteLine($"Value of integer array[{index}] = {values[index]}");
                                            }
                                        }

                                        break;
                                    }
                                case "uint":
                                    {
                                        if (Number == 1)
                                        {
                                            _console.WriteLine($"Reading a single unsigned integer from offset = {Offset}");
                                            UInt32 value = _client.ReadUInt32(Offset);
                                            _console.WriteLine($"Value of single unsigned integer = {value}");
                                        }
                                        else
                                        {
                                            _console.WriteLine($"Reading {Number} unsigned integers from offset = {Offset}");
                                            UInt32[] values = _client.ReadUInt32Array(Offset, Number);

                                            for (int index = 0; index < values.Length; ++index)
                                            {
                                                _console.WriteLine($"Value of unsigned integer array[{index}] = {values[index]}");
                                            }
                                        }

                                        break;
                                    }
                                case "float":
                                    {
                                        if (Number == 1)
                                        {
                                            _console.WriteLine($"Reading a single float from offset = {Offset}");
                                            float value = _client.ReadFloat(Offset);
                                            _console.WriteLine($"Value of single float = {value}");
                                        }
                                        else
                                        {
                                            _console.WriteLine($"Reading {Number} floats from offset = {Offset}");
                                            float[] values = _client.ReadFloatArray(Offset, Number);

                                            for (int index = 0; index < values.Length; ++index)
                                            {
                                                _console.WriteLine($"Value of float array[{index}] = {values[index]}");
                                            }
                                        }

                                        break;
                                    }
                                case "double":
                                    {
                                        if (Number == 1)
                                        {
                                            _console.WriteLine($"Reading a single double from offset = {Offset}");
                                            double value = _client.ReadDouble(Offset);
                                            _console.WriteLine($"Value of single double = {value}");
                                        }
                                        else
                                        {
                                            _console.WriteLine($"Reading {Number} doubles from offset = {Offset}");
                                            double[] values = _client.ReadDoubleArray(Offset, Number);

                                            for (int index = 0; index < values.Length; ++index)
                                            {
                                                _console.WriteLine($"Value of double array[{index}] = {values[index]}");
                                            }
                                        }

                                        break;
                                    }
                                case "long":
                                    {
                                        if (Number == 1)
                                        {
                                            _console.WriteLine($"Reading a single long from offset = {Offset}");
                                            long value = _client.ReadLong(Offset);
                                            _console.WriteLine($"Value of single long = {value}");
                                        }
                                        else
                                        {
                                            _console.WriteLine($"Reading {Number} longs from offset = {Offset}");
                                            long[] values = _client.ReadLongArray(Offset, Number);

                                            for (int index = 0; index < values.Length; ++index)
                                            {
                                                _console.WriteLine($"Value of long array[{index}] = {values[index]}");
                                            }
                                        }

                                        break;
                                    }
                                case "ulong":
                                    {
                                        if (Number == 1)
                                        {
                                            _console.WriteLine($"Reading a single ulong from offset = {Offset}");
                                            ulong value = _client.ReadULong(Offset);
                                            _console.WriteLine($"Value of single ulong = {value}");
                                        }
                                        else
                                        {
                                            _console.WriteLine($"Reading {Number} ulongs from offset = {Offset}");
                                            ulong[] values = _client.ReadULongArray(Offset, Number);

                                            for (int index = 0; index < values.Length; ++index)
                                            {
                                                _console.WriteLine($"Value of ulong array[{index}] = {values[index]}");
                                            }
                                        }
                                        break;
                                    }
                            }
                        }
                        else
                        {
                            if (Number == 1)
                            {
                                _console.WriteLine($"Reading a holding register[{Offset}]");
                                ushort[] values = _client.ReadHoldingRegisters(Offset, Number);
                                if (OptionX) _console.WriteLine($"Value of holding register[{Offset}] = {values[0]:X2}");
                                else _console.WriteLine($"Value of holding register[{Offset}] = {values[0]}");
                            }
                            else
                            {
                                _console.WriteLine($"Reading {Number} holding registers starting at {Offset}");
                                ushort[] values = _client.ReadHoldingRegisters(Offset, Number);

                                for (int index = 0; index < values.Length; ++index)
                                {
                                    if (OptionX) _console.WriteLine($"Value of holding register[{index}] = {values[index]:X2}");
                                    else _console.WriteLine($"Value of holding register[{index}] = {values[index]}");
                                }
                            }
                        }
                    }

                    // Reading input registers.
                    if (OptionI)
                    {
                        if (Type.HasValue)
                        {
                            switch (Type.Value.ToLowerInvariant())
                            {
                                case "string":
                                    {
                                        if (OptionX)
                                        {
                                            _console.WriteLine($"Reading a HEX string from offset = {Offset}");
                                            string value = _client.ReadOnlyHexString(Offset, Number);
                                            _console.WriteLine($"Value of HEX string = {value}");
                                        }
                                        else
                                        {
                                            _console.WriteLine($"Reading an ASCII string from offset = {Offset}");
                                            string value = _client.ReadOnlyString(Offset, Number);
                                            _console.WriteLine($"Value of ASCII string = {value}");
                                        }

                                        break;
                                    }
                                case "bits":
                                    {
                                        _console.WriteLine($"Reading a 16 bit array from offset = {Offset}");
                                        BitArray value = _client.ReadOnlyBits(Offset);
                                        _console.WriteLine($"Value of 16 bit array = {value.ToDigitString()}");

                                        break;
                                    }
                                case "byte":
                                    {
                                        if (Number == 1)
                                        {
                                            _console.WriteLine($"Reading a single byte from offset = {Offset}");
                                            byte[] values = _client.ReadOnlyBytes(Offset, Number);
                                            _console.WriteLine($"Value of single byte = {values[0]}");
                                        }
                                        else
                                        {
                                            _console.WriteLine($"Reading {Number} bytes from offset = {Offset}");
                                            byte[] values = _client.ReadOnlyBytes(Offset, Number);

                                            for (int index = 0; index < values.Length; ++index)
                                            {
                                                _console.WriteLine($"Value of byte array[{index}] = {values[index]}");
                                            }
                                        }

                                        break;
                                    }
                                case "short":
                                    {
                                        if (Number == 1)
                                        {
                                            _console.WriteLine($"Reading a single short from offset = {Offset}");
                                            short value = _client.ReadOnlyShort(Offset);
                                            _console.WriteLine($"Value of single short = {value}");
                                        }
                                        else
                                        {
                                            _console.WriteLine($"Reading {Number} short values from offset = {Offset}");
                                            short[] values = _client.ReadOnlyShortArray(Offset, Number);

                                            for (int index = 0; index < values.Length; ++index)
                                            {
                                                _console.WriteLine($"Value of short array[{index}] = {values[index]}");
                                            }
                                        }

                                        break;
                                    }
                                case "ushort":
                                    {
                                        if (Number == 1)
                                        {
                                            _console.WriteLine($"Reading a single ushort from offset = {Offset}");
                                            ushort value = _client.ReadOnlyUShort(Offset);
                                            _console.WriteLine($"Value of single ushort = {value}");
                                        }
                                        else
                                        {
                                            _console.WriteLine($"Reading {Number} ushort values from offset = {Offset}");
                                            ushort[] values = _client.ReadOnlyUShortArray(Offset, Number);

                                            for (int index = 0; index < values.Length; ++index)
                                            {
                                                _console.WriteLine($"Value of ushort array[{index}] = {values[index]}");
                                            }
                                        }

                                        break;
                                    }
                                case "int":
                                    {
                                        if (Number == 1)
                                        {
                                            _console.WriteLine($"Reading a single int from offset = {Offset}");
                                            Int32 value = _client.ReadOnlyInt32(Offset);
                                            _console.WriteLine($"Value of single integer = {value}");
                                        }
                                        else
                                        {
                                            _console.WriteLine($"Reading {Number} int values from offset = {Offset}");
                                            Int32[] values = _client.ReadOnlyInt32Array(Offset, Number);

                                            for (int index = 0; index < values.Length; ++index)
                                            {
                                                _console.WriteLine($"Value of int array[{index}] = {values[index]}");
                                            }
                                        }

                                        break;
                                    }
                                case "uint":
                                    {
                                        if (Number == 1)
                                        {
                                            _console.WriteLine($"Reading a single unsigned int from offset = {Offset}");
                                            UInt32 value = _client.ReadOnlyUInt32(Offset);
                                            _console.WriteLine($"Value of single unsigned int = {value}");
                                        }
                                        else
                                        {
                                            _console.WriteLine($"Reading {Number} unsigned int values from offset = {Offset}");
                                            UInt32[] values = _client.ReadOnlyUInt32Array(Offset, Number);

                                            for (int index = 0; index < values.Length; ++index)
                                            {
                                                _console.WriteLine($"Value of unsigned int array[{index}] = {values[index]}");
                                            }
                                        }

                                        break;
                                    }
                                case "float":
                                    {
                                        if (Number == 1)
                                        {
                                            _console.WriteLine($"Reading a single float from offset = {Offset}");
                                            float value = _client.ReadOnlyFloat(Offset);
                                            _console.WriteLine($"Value of single float = {value}");
                                        }
                                        else
                                        {
                                            _console.WriteLine($"Reading {Number} float values from offset = {Offset}");
                                            float[] values = _client.ReadOnlyFloatArray(Offset, Number);

                                            for (int index = 0; index < values.Length; ++index)
                                            {
                                                _console.WriteLine($"Value of float array[{index}] = {values[index]}");
                                            }
                                        }

                                        break;
                                    }
                                case "double":
                                    {
                                        if (Number == 1)
                                        {
                                            _console.WriteLine($"Reading a single double from offset = {Offset}");
                                            double value = _client.ReadOnlyDouble(Offset);
                                            _console.WriteLine($"Value of single double = {value}");
                                        }
                                        else
                                        {
                                            _console.WriteLine($"Reading {Number} double values from offset = {Offset}");
                                            double[] values = _client.ReadOnlyDoubleArray(Offset, Number);

                                            for (int index = 0; index < values.Length; ++index)
                                            {
                                                _console.WriteLine($"Value of double array[{index}] = {values[index]}");
                                            }
                                        }

                                        break;
                                    }
                                case "long":
                                    {
                                        if (Number == 1)
                                        {
                                            _console.WriteLine($"Reading a single long from offset = {Offset}");
                                            long value = _client.ReadOnlyLong(Offset);
                                            _console.WriteLine($"Value of single long = {value}");
                                        }
                                        else
                                        {
                                            _console.WriteLine($"Reading {Number} long values from offset = {Offset}");
                                            long[] values = _client.ReadOnlyLongArray(Offset, Number);

                                            for (int index = 0; index < values.Length; ++index)
                                            {
                                                _console.WriteLine($"Value of long array[{index}] = {values[index]}");
                                            }
                                        }

                                        break;
                                    }
                                case "ulong":
                                    {
                                        if (Number == 1)
                                        {
                                            _console.WriteLine($"Reading a single unsigned long from offset = {Offset}");
                                            ulong value = _client.ReadOnlyULong(Offset);
                                            _console.WriteLine($"Value of single ulong = {value}");
                                        }
                                        else
                                        {
                                            _console.WriteLine($"Reading {Number} unsigned long values from offset = {Offset}");
                                            ulong[] values = _client.ReadOnlyULongArray(Offset, Number);

                                            for (int index = 0; index < values.Length; ++index)
                                            {
                                                _console.WriteLine($"Value of ulong array[{index}] = {values[index]}");
                                            }
                                        }

                                        break;
                                    }
                            }
                        }
                        else
                        {
                            if (Number == 1)
                            {
                                _console.WriteLine($"Reading a input register[{Offset}]");
                                ushort[] values = _client.ReadInputRegisters(Offset, Number);
                                if (OptionX) _console.WriteLine($"Value of input register[{Offset}] = {values[0]:X2}");
                                else _console.WriteLine($"Value of input register[{Offset}] = {values[0]}");
                            }
                            else
                            {
                                _console.WriteLine($"Reading {Number} input registers starting at {Offset}");
                                ushort[] values = _client.ReadInputRegisters(Offset, Number);

                                for (int index = 0; index < values.Length; ++index)
                                {
                                    if (OptionX) _console.WriteLine($"Value of input register[{index}] = {values[index]:X2}");
                                    else _console.WriteLine($"Value of input register[{index}] = {values[index]}");
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
            catch
            {
                _logger.LogError("TcpReadCommand exception");
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
                if (!OptionC && !OptionD && !OptionH && !OptionI)
                {
                    throw new CommandParsingException(_application, $"Specify the read option (coils, discrete inputs, holding registers, input registers).");
                }

                if ((OptionC && (OptionD || OptionH || OptionI)) ||
                    (OptionD && (OptionC || OptionH || OptionI)) ||
                    (OptionH && (OptionD || OptionC || OptionI)) ||
                    (OptionI && (OptionD || OptionH || OptionC)))
                {
                    throw new CommandParsingException(_application, $"Specify only a single type (coils, discrete inputs, holding registers, input register).");
                }

                if ((OptionC || OptionD) && OptionX)
                {
                    _console.WriteLine($"HEX output option is ignored.");
                }

                if (Type.HasValue)
                {
                    if (OptionI || OptionH)
                    {
                        switch (Type.Value.ToLower(CultureInfo.CurrentCulture))
                        {
                            case "bits":
                                if (Number > 1)
                                {
                                    _console.WriteLine($"Only a single bit array value is supported (Number == 1).");
                                    Number = 1;
                                }
                                break;
                            case "string":
                                break;
                            case "byte":
                            case "short":
                            case "ushort":
                            case "int":
                            case "uint":
                            case "float":
                            case "double":
                            case "long":
                            case "ulong":
                                if (OptionX)
                                {
                                    _console.WriteLine($"HEX output option is ignored.");
                                }

                                break;
                            default:
                                throw new CommandParsingException(_application, $"Unsupported data type '{Type}'.");
                        }
                    }
                    else if (OptionC || OptionD)
                    {
                        _console.WriteLine($"Specified type '{Type.Value}' is ignored.");
                    }
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
