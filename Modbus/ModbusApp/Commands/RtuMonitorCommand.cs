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
    using System.Globalization;
    using System.Linq;
    using System.Text.Json;
    using System.Threading;
    using System.Threading.Tasks;

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

    [Command(Name = "monitor",
             FullName = "ModbusApp RTU Monitor Command",
             Description = "Supporting Modbus RTU monitor operations.",
             ExtendedHelpText = "\nPlease specify the monitor option (coils, discrete inputs, holding registers, or input registers).")]
    public class RtuMonitorCommand : BaseCommand<RtuMonitorCommand, AppSettings>
    {
        #region Private Data Members

        private readonly JsonSerializerOptions _options = JsonExtensions.DefaultSerializerOptions;
        private static readonly AutoResetEvent _closing = new AutoResetEvent(false);
        private readonly IRtuModbusClient _client;

        #endregion

        #region Private Properties

        /// <summary>
        /// This is a reference to the parent command <see cref="RtuCommand"/>.
        /// </summary>
        private RtuCommand? Parent { get; }

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

        [Option("-x|--hex", Description = "Displays the register values in HEX.")]
        public bool OptionX { get; }

        [Option(Description = "The number of items to read.")]
        public ushort Number { get; set; } = 1;

        [Option(Description = "The offset of the first item to read.")]
        public ushort Offset { get; set; } = 0;

        [Option(Description = "Reads the specified data type")]
        [AllowedValues("bits", "string", "byte", "short", "ushort", "int", "uint", "float", "double", "long", "ulong", IgnoreCase = true)]
        public (bool HasValue, string Value) Type { get; set; } = (false, string.Empty);

        [Option(Description = "The number of times to read (default: forever).")]
        public uint Repeat { get; set; } = 0;

        [Option(Description = "The seconds between times to read (default: 10).")]
        public uint Seconds { get; set; } = 10;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RtuMonitorCommand"/> class.
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
        public RtuMonitorCommand(IRtuModbusClient client,
                                 IConsole console,
                                 AppSettings settings,
                                 IConfiguration config,
                                 IHostEnvironment environment,
                                 IHostApplicationLifetime lifetime,
                                 ILogger<RtuMonitorCommand> logger,
                                 CommandLineApplication application)
            : base(console, settings, config, environment, lifetime, logger, application)
        {
            // Setting the RTU client instance.
            _client = client;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Runs when the commandline application command is executed.
        /// </summary>
        /// <returns>The exit code</returns>
        public async Task<int> OnExecuteAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                if (!(Parent is null))
                {
                    // Overriding TCP client options.
                    _client.RtuMaster.SerialPort = Parent.SerialPort;
                    _client.RtuMaster.Baudrate = Parent.Baudrate;
                    _client.RtuMaster.Parity = Parent.Parity;
                    _client.RtuMaster.DataBits = Parent.DataBits;
                    _client.RtuMaster.StopBits = Parent.StopBits;
                    _client.RtuMaster.ReadTimeout = Parent.ReadTimeout;
                    _client.RtuMaster.WriteTimeout = Parent.WriteTimeout;
                    _client.RtuSlave.ID = Parent.SlaveID;
                }

                if (Parent?.Parent?.ShowSettings ?? false)
                {
                    _console.WriteLine(JsonSerializer.Serialize<RtuMasterData>(_settings.RtuMaster, _options));
                    _console.WriteLine(JsonSerializer.Serialize<RtuSlaveData>(_settings.RtuSlave, _options));
                }

                if (_client.Connect())
                {
                    try
                    {
                        bool forever = (Repeat == 0);
                        bool verbose = true;

                        await Task.Factory.StartNew(async () =>
                        {
                            while (!cancellationToken.IsCancellationRequested)
                            {
                                    // Read the specified data.
                                    var start = DateTime.UtcNow;
                                    ReadingData(verbose);
                                    // Only first call is verbose.
                                    verbose = false;
                                    var end = DateTime.UtcNow;
                                    double delay = ((Seconds * 1000.0) - (end - start).TotalMilliseconds) / 1000.0;

                                if (Seconds > 0)
                                {
                                    if (delay < 0)
                                    {
                                        _logger?.LogWarning("Monitoring: no time between reads.");
                                    }
                                    else
                                    {
                                        await Task.Delay(TimeSpan.FromSeconds(delay), cancellationToken);
                                    }
                                }

                                if (!forever && (--Repeat <= 0))
                                {
                                    _closing.Set();
                                    break;
                                }
                            }

                        }, cancellationToken);

                        _console.CancelKeyPress += new ConsoleCancelEventHandler((sender, args) =>
                        {
                            _console.WriteLine("Monitoring cancelled.");
                            _closing.Set();
                        });

                        _closing.WaitOne();
                    }
                    catch (AggregateException aex) when (aex.InnerExceptions.All(e => e is OperationCanceledException))
                    {
                        _console.WriteLine("Monitoring cancelled.");
                    }
                    catch (OperationCanceledException)
                    {
                        _console.WriteLine("Monitoring cancelled.");
                        throw;
                    }
                    catch
                    {
                        throw;
                    }
                }
                else
                {
                    _console.WriteLine($"Modbus RTU slave not found at {Parent?.SerialPort}.");
                    return ExitCodes.IncorrectFunction;
                }
            }
            catch
            {
                _logger.LogError("RtuMonitorCommand exception");
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
                    throw new CommandParsingException(_application, $"Specify only a single read option (coils, discrete inputs, holding registers, input register).");
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

        #region Private Methods

        /// <summary>
        /// Reading the specified data.
        /// </summary>
        private void ReadingData(bool verbose = false)
        {
            _logger?.LogDebug("TcpMonitor: Reading data...");

            // Reading coils.
            if (OptionC)
            {
                if (Number == 1)
                {
                    if (verbose) _console.WriteLine($"Monitoring a single coil[{Offset}]");
                    bool[] values = _client.ReadCoils(Offset, Number);
                    _console.WriteLine($"{DateTime.Now:HH:mm:ss.fff} Value of coil[{Offset}] = {values[0]}");
                }
                else
                {
                    if (verbose) _console.WriteLine($"Monitoring {Number} coils starting at {Offset}");
                    bool[] values = _client.ReadCoils(Offset, Number);

                    for (int index = 0; index < values.Length; ++index)
                    {
                        _console.WriteLine($"{DateTime.Now:HH:mm:ss.fff}: Value of coil[{index}] = {values[index]}");
                    }

                    _console.WriteLine();
                }
            }

            // Reading discrete inputs.
            if (OptionD)
            {
                if (Number == 1)
                {
                    if (verbose) _console.WriteLine($"Monitoring a discrete input[{Offset}]");
                    bool[] values = _client.ReadInputs(Offset, Number);
                    _console.WriteLine($"{DateTime.Now:HH:mm:ss.fff}: Value of discrete input[{Offset}] = {values[0]}");
                }
                else
                {
                    if (verbose) _console.WriteLine($"Monitoring {Number} discrete inputs starting at {Offset}");
                    bool[] values = _client.ReadInputs(Offset, Number);

                    for (int index = 0; index < values.Length; ++index)
                    {
                        _console.WriteLine($"{DateTime.Now:HH:mm:ss.fff}: Value of discrete input[{index}] = {values[index]}");
                    }

                    _console.WriteLine();
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
                                    if (verbose) _console.WriteLine($"Monitoring a HEX string from offset = {Offset}");
                                    string value = _client.ReadHexString(Offset, Number);
                                    _console.WriteLine($"{DateTime.Now:HH:mm:ss.fff}: Value of HEX string = {value}");
                                }
                                else
                                {
                                    if (verbose) _console.WriteLine($"Monitoring an ASCII string from offset = {Offset}");
                                    string value = _client.ReadString(Offset, Number);
                                    _console.WriteLine($"{DateTime.Now:HH:mm:ss.fff}: Value of ASCII string = {value}");
                                }

                                break;
                            }
                        case "bits":
                            {
                                if (verbose) _console.WriteLine($"Monitoring a 16 bit array from offset = {Offset}");
                                BitArray value = _client.ReadBits(Offset);
                                _console.WriteLine($"{DateTime.Now:HH:mm:ss.fff}: Value of 16 bit array = {value.ToDigitString()}");
                                break;
                            }
                        case "byte":
                            {
                                if (Number == 1)
                                {
                                    if (verbose) _console.WriteLine($"Monitoring a single byte from offset = {Offset}");
                                    byte[] values = _client.ReadBytes(Offset, Number);
                                    _console.WriteLine($"{DateTime.Now:HH:mm:ss.fff}: Value of single byte = {values[0]}");
                                }
                                else
                                {
                                    if (verbose) _console.WriteLine($"Monitoring {Number} bytes from offset = {Offset}");
                                    byte[] values = _client.ReadBytes(Offset, Number);

                                    for (int index = 0; index < values.Length; ++index)
                                    {
                                        _console.WriteLine($"{DateTime.Now:HH:mm:ss.fff}: Value of byte array[{index}] = {values[index]}");
                                    }

                                    _console.WriteLine();
                                }

                                break;
                            }
                        case "short":
                            {
                                if (Number == 1)
                                {
                                    if (verbose) _console.WriteLine($"Monitoring a single short from offset = {Offset}");
                                    short value = _client.ReadShort(Offset);
                                    _console.WriteLine($"{DateTime.Now:HH:mm:ss.fff}: Value of single short = {value}");
                                }
                                else
                                {
                                    if (verbose) _console.WriteLine($"Monitoring {Number} shorts from offset = {Offset}");
                                    short[] values = _client.ReadShortArray(Offset, Number);

                                    for (int index = 0; index < values.Length; ++index)
                                    {
                                        _console.WriteLine($"{DateTime.Now:HH:mm:ss.fff}: Value of short array[{index}] = {values[index]}");
                                    }

                                    _console.WriteLine();
                                }

                                break;
                            }
                        case "ushort":
                            {
                                if (Number == 1)
                                {
                                    if (verbose) _console.WriteLine($"Monitoring a single ushort from offset = {Offset}");
                                    ushort value = _client.ReadUShort(Offset);
                                    _console.WriteLine($"{DateTime.Now:HH:mm:ss.fff}: Value of single ushort = {value}");
                                }
                                else
                                {
                                    if (verbose) _console.WriteLine($"Monitoring {Number} ushorts from offset = {Offset}");
                                    ushort[] values = _client.ReadUShortArray(Offset, Number);

                                    for (int index = 0; index < values.Length; ++index)
                                    {
                                        _console.WriteLine($"{DateTime.Now:HH:mm:ss.fff}: Value of ushort array[{index}] = {values[index]}");
                                    }

                                    _console.WriteLine();
                                }

                                break;
                            }
                        case "int":
                            {
                                if (Number == 1)
                                {
                                    if (verbose) _console.WriteLine($"Monitoring a single integer from offset = {Offset}");
                                    Int32 value = _client.ReadInt32(Offset);
                                    _console.WriteLine($"{DateTime.Now:HH:mm:ss.fff}: Value of single integer = {value}");
                                }
                                else
                                {
                                    if (verbose) _console.WriteLine($"Monitoring {Number}  integers from offset = {Offset}");
                                    Int32[] values = _client.ReadInt32Array(Offset, Number);

                                    for (int index = 0; index < values.Length; ++index)
                                    {
                                        _console.WriteLine($"{DateTime.Now:HH:mm:ss.fff}: Value of integer array[{index}] = {values[index]}");
                                    }

                                    _console.WriteLine();
                                }

                                break;
                            }
                        case "uint":
                            {
                                if (Number == 1)
                                {
                                    if (verbose) _console.WriteLine($"Monitoring a single unsigned integer from offset = {Offset}");
                                    UInt32 value = _client.ReadUInt32(Offset);
                                    _console.WriteLine($"{DateTime.Now:HH:mm:ss.fff}: Value of single unsigned integer = {value}");
                                }
                                else
                                {
                                    if (verbose) _console.WriteLine($"Monitoring {Number} unsigned integers from offset = {Offset}");
                                    UInt32[] values = _client.ReadUInt32Array(Offset, Number);

                                    for (int index = 0; index < values.Length; ++index)
                                    {
                                        _console.WriteLine($"{DateTime.Now:HH:mm:ss.fff}: Value of unsigned integer array[{index}] = {values[index]}");
                                    }

                                    _console.WriteLine();
                                }

                                break;
                            }
                        case "float":
                            {
                                if (Number == 1)
                                {
                                    if (verbose) _console.WriteLine($"Monitoring a single float from offset = {Offset}");
                                    float value = _client.ReadFloat(Offset);
                                    _console.WriteLine($"{DateTime.Now:HH:mm:ss.fff}: Value of single float = {value}");
                                }
                                else
                                {
                                    if (verbose) _console.WriteLine($"Monitoring {Number} floats from offset = {Offset}");
                                    float[] values = _client.ReadFloatArray(Offset, Number);

                                    for (int index = 0; index < values.Length; ++index)
                                    {
                                        _console.WriteLine($"{DateTime.Now:HH:mm:ss.fff}: Value of float array[{index}] = {values[index]}");
                                    }

                                    _console.WriteLine();
                                }

                                break;
                            }
                        case "double":
                            {
                                if (Number == 1)
                                {
                                    if (verbose) _console.WriteLine($"Monitoring a single double from offset = {Offset}");
                                    double value = _client.ReadDouble(Offset);
                                    _console.WriteLine($"{DateTime.Now:HH:mm:ss.fff}: Value of single double = {value}");
                                }
                                else
                                {
                                    if (verbose) _console.WriteLine($"Monitoring {Number} doubles from offset = {Offset}");
                                    double[] values = _client.ReadDoubleArray(Offset, Number);

                                    for (int index = 0; index < values.Length; ++index)
                                    {
                                        _console.WriteLine($"{DateTime.Now:HH:mm:ss.fff}: Value of double array[{index}] = {values[index]}");
                                    }

                                    _console.WriteLine();
                                }

                                break;
                            }
                        case "long":
                            {
                                if (Number == 1)
                                {
                                    if (verbose) _console.WriteLine($"Monitoring a single long from offset = {Offset}");
                                    long value = _client.ReadLong(Offset);
                                    _console.WriteLine($"{DateTime.Now:HH:mm:ss.fff}: Value of single long = {value}");
                                }
                                else
                                {
                                    if (verbose) _console.WriteLine($"Monitoring {Number} longs from offset = {Offset}");
                                    long[] values = _client.ReadLongArray(Offset, Number);

                                    for (int index = 0; index < values.Length; ++index)
                                    {
                                        _console.WriteLine($"{DateTime.Now:HH:mm:ss.fff}: Value of long array[{index}] = {values[index]}");
                                    }

                                    _console.WriteLine();
                                }

                                break;
                            }
                        case "ulong":
                            {
                                if (Number == 1)
                                {
                                    if (verbose) _console.WriteLine($"Monitoring a single ulong from offset = {Offset}");
                                    ulong value = _client.ReadULong(Offset);
                                    _console.WriteLine($"{DateTime.Now:HH:mm:ss.fff}: Value of single ulong = {value}");
                                }
                                else
                                {
                                    if (verbose) _console.WriteLine($"Monitoring {Number} ulongs from offset = {Offset}");
                                    ulong[] values = _client.ReadULongArray(Offset, Number);

                                    for (int index = 0; index < values.Length; ++index)
                                    {
                                        _console.WriteLine($"{DateTime.Now:HH:mm:ss.fff}: Value of ulong array[{index}] = {values[index]}");
                                    }

                                    _console.WriteLine();
                                }

                                break;
                            }
                    }
                }
                else if (Number == 1)
                {
                    if (verbose) _console.WriteLine($"Monitoring a holding register[{Offset}]");
                    ushort[] values = _client.ReadHoldingRegisters(Offset, Number);
                    if (OptionX) _console.WriteLine($"{DateTime.Now:HH:mm:ss.fff}: Value of holding register[{Offset}] = {values[0]:X2}");
                    else _console.WriteLine($"{DateTime.Now:HH:mm:ss.fff}: Value of holding register[{Offset}] = {values[0]}");
                }
                else
                {
                    if (verbose) _console.WriteLine($"Monitoring {Number} holding registers starting at {Offset}");
                    ushort[] values = _client.ReadHoldingRegisters(Offset, Number);

                    for (int index = 0; index < values.Length; ++index)
                    {
                        if (OptionX) _console.WriteLine($"{DateTime.Now:HH:mm:ss.fff}: Value of holding register[{index}] = {values[index]:X2}");
                        else _console.WriteLine($"{DateTime.Now:HH:mm:ss.fff}: Value of holding register[{index}] = {values[index]}");
                    }

                    _console.WriteLine();
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
                                    if (verbose) _console.WriteLine($"Monitoring a HEX string from offset = {Offset}");
                                    string value = _client.ReadOnlyHexString(Offset, Number);
                                    _console.WriteLine($"{DateTime.Now:HH:mm:ss.fff}: Value of HEX string = {value}");
                                }
                                else
                                {
                                    if (verbose) _console.WriteLine($"Monitoring an ASCII string from offset = {Offset}");
                                    string value = _client.ReadOnlyString(Offset, Number);
                                    _console.WriteLine($"{DateTime.Now:HH:mm:ss.fff}: Value of ASCII string = {value}");
                                }

                                break;
                            }
                        case "bits":
                            {
                                if (verbose) _console.WriteLine($"Monitoring a 16 bit array from offset = {Offset}");
                                BitArray value = _client.ReadOnlyBits(Offset);
                                _console.WriteLine($"{DateTime.Now:HH:mm:ss.fff}: Value of 16 bit array = {value.ToDigitString()}");
                                break;
                            }
                        case "byte":
                            {
                                if (Number == 1)
                                {
                                    if (verbose) _console.WriteLine($"Monitoring a single byte from offset = {Offset}");
                                    byte[] values = _client.ReadOnlyBytes(Offset, Number);
                                    _console.WriteLine($"{DateTime.Now:HH:mm:ss.fff}: Value of single byte = {values[0]}");
                                }
                                else
                                {
                                    if (verbose) _console.WriteLine($"Monitoring {Number} bytes from offset = {Offset}");
                                    byte[] values = _client.ReadOnlyBytes(Offset, Number);

                                    for (int index = 0; index < values.Length; ++index)
                                    {
                                        _console.WriteLine($"{DateTime.Now:HH:mm:ss.fff}: Value of byte array[{index}] = {values[index]}");
                                    }

                                    _console.WriteLine();
                                }

                                break;
                            }
                        case "short":
                            {
                                if (Number == 1)
                                {
                                    if (verbose) _console.WriteLine($"Monitoring a single short from offset = {Offset}");
                                    short value = _client.ReadOnlyShort(Offset);
                                    _console.WriteLine($"{DateTime.Now:HH:mm:ss.fff}: Value of single short = {value}");
                                }
                                else
                                {
                                    if (verbose) _console.WriteLine($"Monitoring {Number} short values from offset = {Offset}");
                                    short[] values = _client.ReadOnlyShortArray(Offset, Number);

                                    for (int index = 0; index < values.Length; ++index)
                                    {
                                        _console.WriteLine($"{DateTime.Now:HH:mm:ss.fff}: Value of short array[{index}] = {values[index]}");
                                    }

                                    _console.WriteLine();
                                }

                                break;
                            }
                        case "ushort":
                            {
                                if (Number == 1)
                                {
                                    if (verbose) _console.WriteLine($"Monitoring a single ushort from offset = {Offset}");
                                    ushort value = _client.ReadOnlyUShort(Offset);
                                    _console.WriteLine($"{DateTime.Now:HH:mm:ss.fff}: Value of single ushort = {value}");
                                }
                                else
                                {
                                    if (verbose) _console.WriteLine($"Monitoring {Number} ushort values from offset = {Offset}");
                                    ushort[] values = _client.ReadOnlyUShortArray(Offset, Number);

                                    for (int index = 0; index < values.Length; ++index)
                                    {
                                        _console.WriteLine($"{DateTime.Now:HH:mm:ss.fff}: Value of ushort array[{index}] = {values[index]}");
                                    }

                                    _console.WriteLine();
                                }

                                break;
                            }
                        case "int":
                            {
                                if (Number == 1)
                                {
                                    if (verbose) _console.WriteLine($"Monitoring a single int from offset = {Offset}");
                                    Int32 value = _client.ReadOnlyInt32(Offset);
                                    _console.WriteLine($"{DateTime.Now:HH:mm:ss.fff}: Value of single integer = {value}");
                                }
                                else
                                {
                                    if (verbose) _console.WriteLine($"Monitoring {Number} int values from offset = {Offset}");
                                    Int32[] values = _client.ReadOnlyInt32Array(Offset, Number);

                                    for (int index = 0; index < values.Length; ++index)
                                    {
                                        _console.WriteLine($"{DateTime.Now:HH:mm:ss.fff}: Value of int array[{index}] = {values[index]}");
                                    }

                                    _console.WriteLine();
                                }

                                break;
                            }
                        case "uint":
                            {
                                if (Number == 1)
                                {
                                    if (verbose) _console.WriteLine($"Monitoring a single unsigned int from offset = {Offset}");
                                    UInt32 value = _client.ReadOnlyUInt32(Offset);
                                    _console.WriteLine($"{DateTime.Now:HH:mm:ss.fff}: Value of single unsigned int = {value}");
                                }
                                else
                                {
                                    if (verbose) _console.WriteLine($"Monitoring {Number} unsigned int values from offset = {Offset}");
                                    UInt32[] values = _client.ReadOnlyUInt32Array(Offset, Number);

                                    for (int index = 0; index < values.Length; ++index)
                                    {
                                        _console.WriteLine($"{DateTime.Now:HH:mm:ss.fff}: Value of unsigned int array[{index}] = {values[index]}");
                                    }

                                    _console.WriteLine();
                                }

                                break;
                            }
                        case "float":
                            {
                                if (Number == 1)
                                {
                                    if (verbose) _console.WriteLine($"Monitoring a single float from offset = {Offset}");
                                    float value = _client.ReadOnlyFloat(Offset);
                                    _console.WriteLine($"{DateTime.Now:HH:mm:ss.fff}: Value of single float = {value}");
                                }
                                else
                                {
                                    if (verbose) _console.WriteLine($"Monitoring {Number} float values from offset = {Offset}");
                                    float[] values = _client.ReadOnlyFloatArray(Offset, Number);

                                    for (int index = 0; index < values.Length; ++index)
                                    {
                                        _console.WriteLine($"{DateTime.Now:HH:mm:ss.fff}: Value of float array[{index}] = {values[index]}");
                                    }

                                    _console.WriteLine();
                                }

                                break;
                            }
                        case "double":
                            {
                                if (Number == 1)
                                {
                                    if (verbose) _console.WriteLine($"Monitoring a single double from offset = {Offset}");
                                    double value = _client.ReadOnlyDouble(Offset);
                                    _console.WriteLine($"{DateTime.Now:HH:mm:ss.fff}: Value of single double = {value}");
                                }
                                else
                                {
                                    if (verbose) _console.WriteLine($"Monitoring {Number} double values from offset = {Offset}");
                                    double[] values = _client.ReadOnlyDoubleArray(Offset, Number);

                                    for (int index = 0; index < values.Length; ++index)
                                    {
                                        _console.WriteLine($"{DateTime.Now:HH:mm:ss.fff}: Value of double array[{index}] = {values[index]}");
                                    }

                                    _console.WriteLine();
                                }

                                break;
                            }
                        case "long":
                            {
                                if (Number == 1)
                                {
                                    if (verbose) _console.WriteLine($"Monitoring a single long from offset = {Offset}");
                                    long value = _client.ReadOnlyLong(Offset);
                                    _console.WriteLine($"{DateTime.Now:HH:mm:ss.fff}: Value of single long = {value}");
                                }
                                else
                                {
                                    if (verbose) _console.WriteLine($"Monitoring {Number} long values from offset = {Offset}");
                                    long[] values = _client.ReadOnlyLongArray(Offset, Number);

                                    for (int index = 0; index < values.Length; ++index)
                                    {
                                        _console.WriteLine($"{DateTime.Now:HH:mm:ss.fff}: Value of long array[{index}] = {values[index]}");
                                    }

                                    _console.WriteLine();
                                }

                                break;
                            }
                        case "ulong":
                            {
                                if (Number == 1)
                                {
                                    if (verbose) _console.WriteLine($"Monitoring a single unsigned long from offset = {Offset}");
                                    ulong value = _client.ReadOnlyULong(Offset);
                                    _console.WriteLine($"{DateTime.Now:HH:mm:ss.fff}: Value of single ulong = {value}");
                                }
                                else
                                {
                                    if (verbose) _console.WriteLine($"Monitoring {Number} unsigned long values from offset = {Offset}");
                                    ulong[] values = _client.ReadOnlyULongArray(Offset, Number);

                                    for (int index = 0; index < values.Length; ++index)
                                    {
                                        _console.WriteLine($"{DateTime.Now:HH:mm:ss.fff}: Value of ulong array[{index}] = {values[index]}");
                                    }

                                    _console.WriteLine();
                                }

                                break;
                            }
                    }
                }
                else
                {
                    if (Number == 1)
                    {
                        if (verbose) _console.WriteLine($"Monitoring a input register[{Offset}]");
                        ushort[] values = _client.ReadInputRegisters(Offset, Number);
                        if (OptionX) _console.WriteLine($"{DateTime.Now:HH:mm:ss.fff}: Value of input register[{Offset}] = {values[0]:X2}");
                        else _console.WriteLine($"{DateTime.Now:HH:mm:ss.fff}: Value of input register[{Offset}] = {values[0]}");
                    }
                    else
                    {
                        if (verbose) _console.WriteLine($"Monitoring {Number} input registers starting at {Offset}");
                        ushort[] values = _client.ReadInputRegisters(Offset, Number);

                        for (int index = 0; index < values.Length; ++index)
                        {
                            if (OptionX) _console.WriteLine($"{DateTime.Now:HH:mm:ss.fff}: Value of input register[{index}] = {values[index]:X2}");
                            else _console.WriteLine($"{DateTime.Now:HH:mm:ss.fff}: Value of input register[{index}] = {values[index]}");
                        }

                        _console.WriteLine();
                    }
                }
            }
        }

        #endregion
    }
}
