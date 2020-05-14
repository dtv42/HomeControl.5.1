// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TcpCommand.cs" company="DTV-Online">
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
    using System.Net;
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
    /// This class implements the TCP command.
    /// </summary>
    [Command(Name = "tcp",
             FullName = "NModbusApp TCP Command",
             Description = "Subcommand supporting standard Modbus TCP operations.",
             ExtendedHelpText = "\nCopyright (c) 2020 Dr. Peter Trimmel - All rights reserved.")]
    [Subcommand(typeof(TcpReadCommand))]
    [Subcommand(typeof(TcpWriteCommand))]
    [Subcommand(typeof(TcpMonitorCommand))]
    public class TcpCommand : BaseCommand<TcpCommand, AppSettings>
    {
        #region Private Data Members

        private readonly JsonSerializerOptions _options = JsonExtensions.DefaultSerializerOptions;
        private readonly ITcpModbusClient _client;

        #endregion

        #region Public Properties

        /// <summary>
        /// This is a reference to the parent command <see cref="RootCommand"/>.
        /// </summary>
        public RootCommand? Parent { get; }

        [Option("--address <IP>", Description = "Sets the Modbus slave IP address.", Inherited = true)]
        [IPAddress]
        public string Address { get; } = string.Empty;

        [Option("--port <NUMBER>", Description = "Sets the Modbus slave port number.", Inherited = true)]
        [Range(1, 65535)]
        public int Port { get; }

        [Option("--slaveid <NUMBER>", Description = "Sets the Modbus slave ID.", Inherited = true)]
        public byte SlaveID { get; }

        [Range(0, Int32.MaxValue)]
        public int ReceiveTimeout { get; }

        [Range(0, Int32.MaxValue)]
        public int SendTimeout { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TcpCommand"/> class.
        /// Selected properties are initialized with data from the AppSettings instance.
        /// <summary>
        /// <param name="client"></param>
        /// <param name="console"></param>
        /// <param name="settings"></param>
        /// <param name="config"></param>
        /// <param name="environment"></param>
        /// <param name="lifetime"></param>
        /// <param name="logger"></param>
        /// <param name="application"></param>
        public TcpCommand(ITcpModbusClient client,
                          IConsole console,
                          AppSettings settings,
                          IConfiguration config,
                          IHostEnvironment environment,
                          IHostApplicationLifetime lifetime,
                          ILogger<TcpCommand> logger,
                          CommandLineApplication application)
            : base(console, settings, config, environment, lifetime, logger, application)
        {
            ReceiveTimeout = _settings.TcpMaster.ReceiveTimeout;
            SendTimeout = _settings.TcpMaster.SendTimeout;
            Address = _settings.TcpSlave.Address;
            Port = _settings.TcpSlave.Port;
            SlaveID = _settings.TcpSlave.ID;

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
                // Overriding TCP client options.
                _client.TcpMaster.ReceiveTimeout = ReceiveTimeout;
                _client.TcpMaster.SendTimeout = SendTimeout;
                _client.TcpSlave.Address = Address;
                _client.TcpSlave.Port = Port;
                _client.TcpSlave.ID = SlaveID;

                if (Parent?.ShowSettings ?? false)
                {
                    _console.WriteLine(JsonSerializer.Serialize<TcpMasterData>(_client.TcpMaster, _options));
                    _console.WriteLine(JsonSerializer.Serialize<TcpSlaveData>(_client.TcpSlave, _options));
                }

                if (_client.Connect())
                {
                    _console.WriteLine($"Modbus TCP slave found at {Address}:{Port}.");
                    return ExitCodes.SuccessfullyCompleted;
                }
                else
                {
                    _console.WriteLine($"Modbus TCP slave not found at {Address}:{Port}.");
                    return ExitCodes.IncorrectFunction;
                }
            }
            catch
            {
                _logger.LogError("TcpCommand exception");
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

        #endregion
    }
}
