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

    /// <summary>
    /// This class implements the TCP command.
    /// </summary>
    internal sealed class TcpCommand : Command
    {
        #region Private Data Members

        private readonly JsonSerializerOptions _jsonoptions = JsonExtensions.DefaultSerializerOptions;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TcpCommand"/> class.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="tcpReadCommand"></param>
        /// <param name="tcpWriteCommand"></param>
        /// <param name="tcpMonitorCommand"></param>
        /// <param name="settings"></param>
        /// <param name="logger"></param>
        public TcpCommand(ITcpModbusClient client,
                          TcpReadCommand tcpReadCommand,
                          TcpWriteCommand tcpWriteCommand,
                          TcpMonitorCommand tcpMonitorCommand,
                          AppSettings settings,
                          ILogger<TcpCommand> logger)
            : base("tcp", "Subcommand supporting standard Modbus TCP operations.")
        {
            // Setup command options.
            AddGlobalOption(new Option<string>("--address", "Sets the Modbus slave IP address").Name("Address").Default(settings.TcpSlave.Address).IPAddress());
            AddGlobalOption(new Option<int>("--port", "Sets the Modbus slave IP port").Name("Port").Default(settings.TcpSlave.Port).Range(0, 65535));
            AddGlobalOption(new Option<byte>("--slaveid", "Sets the Modbus slave ID").Name("SlaveID").Default(settings.TcpSlave.ID));
            AddGlobalOption(new Option<int>("--receive-timeout", "Sets the receive timeout").Name("ReceiveTimeout").Default(settings.TcpMaster.ReceiveTimeout).Range(0, Int32.MaxValue).Hide());
            AddGlobalOption(new Option<int>("--send-timeout", "Sets the send timeout").Name("SendTimeout").Default(settings.TcpMaster.SendTimeout).Range(0, Int32.MaxValue).Hide());

            // Add sub commands.
            AddCommand(tcpReadCommand);
            AddCommand(tcpWriteCommand);
            AddCommand(tcpMonitorCommand);

            // Setup execution handler.
            Handler = CommandHandler.Create<IConsole, bool, TcpCommandOptions>((console, verbose, options) =>
            {
                logger.LogInformation("Handler()");

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
                        console.Out.WriteLine($"Modbus TCP slave found at {options.Address}:{options.Port}.");
                        return ExitCodes.SuccessfullyCompleted;
                    }
                    else
                    {
                        console.Out.WriteLine($"Modbus TCP slave not found at {options.Address}:{options.Port}.");
                        return ExitCodes.IncorrectFunction;
                    }
                }
                catch
                {
                    logger.LogError("TcpCommand exception");
                    throw;
                }
                finally
                {
                    if (client.Connected)
                    {
                        client.Disconnect();
                    }
                }
            });
        }

        #endregion
    }
}
