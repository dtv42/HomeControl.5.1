// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RtuCommand.cs" company="DTV-Online">
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
    using System.ComponentModel.DataAnnotations;
    using System.IO.Ports;
    using System.Linq;
    using System.Text.Json;

    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;

    using McMaster.Extensions.CommandLineUtils;

    using UtilityLib;
    using ModbusLib;
    using ModbusLib.Models;
    using ModbusApp.Models;

    #endregion

    /// <summary>
    /// This class implements the RTU command.
    /// </summary>
    [Command(Name = "rtu",
             FullName = "ModbusApp RTU Command",
             Description = "Subcommand supporting standard Modbus RTU operations.",
             ExtendedHelpText = "\nCopyright (c) 2020 Dr. Peter Trimmel - All rights reserved.")]
    [Subcommand(typeof(RtuReadCommand))]
    [Subcommand(typeof(RtuWriteCommand))]
    [Subcommand(typeof(RtuMonitorCommand))]
    public class RtuCommand : BaseCommand<RtuCommand, AppSettings>
    {
        #region Private Data Members

        private readonly JsonSerializerOptions _options = JsonExtensions.DefaultSerializerOptions;
        private readonly IRtuModbusClient _client;

        #endregion

        #region Public Properties

        /// <summary>
        /// This is a reference to the parent command <see cref="RootCommand"/>.
        /// </summary>
        public RootCommand? Parent { get; }

        [Option("--com <string>", Description = "Sets the Modbus master COM port.", Inherited = true)]
        public string SerialPort { get; } = string.Empty;

        [Option("--baudrate <NUMBER>", Description = "Sets the Modbus COM port baud rate.", Inherited = true)]
        [RegularExpression(@"(110|150|300|600|1200|1800|2400|4800|7200|9600|14400|19200|31250|38400|56000|57600|76800|115200|128000|230400|256000)")]
        public int Baudrate { get; } = 9600;

        [Option("--parity <STRING>", Description = "Sets the Modbus COM port parity.", Inherited = true)]
        public Parity Parity { get; } = Parity.None;

        [Option("--databits <NUMBER>", Description = "Sets the Modbus COM port data bits.", Inherited = true)]
        [Range(5, 8)]
        public int DataBits { get; } = 8;

        [Option("--stopbits <STRING>", Description = "Sets the Modbus COM port stop bits.", Inherited = true)]
        public StopBits StopBits { get; } = StopBits.One;

        [Option("--slaveid <NUMBER>", Description = "Sets the Modbus slave ID.", Inherited = true)]
        public byte SlaveID { get; }

        [RegularExpression(@"(-1|[0-9]+)")]
        public int ReadTimeout { get; }

        [RegularExpression(@"(-1|[0-9]+)")]
        public int WriteTimeout { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RtuCommand"/> class.
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
        public RtuCommand(IRtuModbusClient client, 
                          IConsole console,
                          AppSettings settings,
                          IConfiguration config,
                          IHostEnvironment environment,
                          IHostApplicationLifetime lifetime,
                          ILogger<RtuCommand> logger,
                          CommandLineApplication application)
            : base(console, settings, config, environment, lifetime, logger, application)
        {
            // Setting the RTU client instance.
            _client = client;

            SerialPort = _settings.RtuMaster.SerialPort;
            Baudrate = _settings.RtuMaster.Baudrate;
            Parity = _settings.RtuMaster.Parity;
            DataBits = _settings.RtuMaster.DataBits;
            StopBits = _settings.RtuMaster.StopBits;
            ReadTimeout = _settings.RtuMaster.ReadTimeout;
            WriteTimeout = _settings.RtuMaster.WriteTimeout;
            SlaveID = _settings.RtuSlave.ID;
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
                // Overriding RTU client options.
                _client.RtuMaster.SerialPort = SerialPort;
                _client.RtuMaster.Baudrate = Baudrate;
                _client.RtuMaster.Parity = Parity;
                _client.RtuMaster.DataBits = DataBits;
                _client.RtuMaster.StopBits = StopBits;
                _client.RtuMaster.ReadTimeout = ReadTimeout;
                _client.RtuMaster.WriteTimeout = WriteTimeout;
                _client.RtuSlave.ID = SlaveID;

                if (Parent?.ShowSettings ?? false)
                {
                    _console.WriteLine(JsonSerializer.Serialize<RtuMasterData>(_client.RtuMaster, _options));
                    _console.WriteLine(JsonSerializer.Serialize<RtuSlaveData>(_client.RtuSlave, _options));
                }

                if (_client.Connect())
                {
                    Console.WriteLine($"RTU serial port found at {SerialPort}.");
                    return ExitCodes.SuccessfullyCompleted;
                }
                else
                {
                    Console.WriteLine($"RTU serial port not found at {SerialPort}.");
                    return ExitCodes.IncorrectFunction;
                }
            }
            catch
            {
                _logger.LogError("RtuCommand exception");
                throw;
            }
            finally
            {
                if (_client.Connected)
                {
                    _client.Disconnect();
                }
            }
        }

        /// <summary>
        /// Helper method to check options.
        /// </summary>
        /// <returns>True if options are OK.</returns>
        public override bool CheckOptions()
        {
            if (string.IsNullOrEmpty(SerialPort))
            {
                throw new CommandParsingException(_application, "Missing serial port name");
            }

            var ports = System.IO.Ports.SerialPort.GetPortNames();

            if (!ports.Any(s => SerialPort.Equals(s)))
            {
                throw new CommandParsingException(_application, "Invalid serial port name");
            }

            return true;
        }

        #endregion Public Methods
    }
}
