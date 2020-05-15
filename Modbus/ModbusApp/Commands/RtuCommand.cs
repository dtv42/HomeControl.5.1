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
    using System.CommandLine;
    using System.CommandLine.IO;
    using System.CommandLine.Invocation;
    using System.IO.Ports;
    using System.Text.Json;

    using Microsoft.Extensions.Logging;

    using UtilityLib;
    using ModbusLib;
    using ModbusLib.Models;
    using ModbusApp.Models;

    #endregion

    /// <summary>
    /// This class implements the RTU command.
    /// </summary>
    internal sealed class RtuCommand : Command
    {
        #region Private Data Members

        private readonly JsonSerializerOptions _jsonoptions = JsonExtensions.DefaultSerializerOptions;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RtuCommand"/> class.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="rtuReadCommand"></param>
        /// <param name="rtuWriteCommand"></param>
        /// <param name="rtuMonitorCommand"></param>
        /// <param name="settings"></param>
        /// <param name="logger"></param>
        public RtuCommand(IRtuModbusClient client,
                          RtuReadCommand rtuReadCommand,
                          RtuWriteCommand rtuWriteCommand,
                          RtuMonitorCommand rtuMonitorCommand,
                          AppSettings settings,
                          ILogger<RtuCommand> logger)
            : base("rtu", "Subcommand supporting standard Modbus RTU operations.")
        {
            // Setup command options.
            AddGlobalOption(new Option<string>  ("--serialport",    "Sets the Modbus master COM port"   ).Name("SerialPort"  ).Default(settings.RtuMaster.SerialPort  ));
            AddGlobalOption(new Option<int>     ("--baudrate",      "Sets the Modbus COM port baud rate").Name("Baudrate"    ).Default(settings.RtuMaster.Baudrate    ));
            AddGlobalOption(new Option<Parity>  ("--parity",        "Sets the Modbus COM port parity"   ).Name("Parity"      ).Default(settings.RtuMaster.Parity      ));
            AddGlobalOption(new Option<int>     ("--databits",      "Sets the Modbus COM port data bits").Name("DataBits"    ).Default(settings.RtuMaster.DataBits    ).Range(5, 8));
            AddGlobalOption(new Option<StopBits>("--stopbits",      "Sets the Modbus COM port stop bits").Name("StopBits"    ).Default(settings.RtuMaster.StopBits    ));
            AddGlobalOption(new Option<byte>    ("--slaveid",       "Sets the Modbus slave ID"          ).Name("SlaveID"     ).Default(settings.RtuSlave.ID           ));
            AddGlobalOption(new Option<int>     ("--read-timeout",  "Sets the read timeout"             ).Name("ReadTimeout" ).Default(settings.RtuMaster.ReadTimeout ).Range(-1, Int32.MaxValue).Hide());
            AddGlobalOption(new Option<int>     ("--write-timeout", "Sets the read timeout"             ).Name("WriteTimeout").Default(settings.RtuMaster.WriteTimeout).Range(-1, Int32.MaxValue).Hide());

            // Add sub commands.
            AddCommand(rtuReadCommand);
            AddCommand(rtuWriteCommand);
            AddCommand(rtuMonitorCommand);

            // Add custom validation.
            AddValidator(r =>
            {
                var valid = new System.Collections.Generic.List<int> { 110, 150, 300, 600, 1200, 1800, 2400, 4800, 7200, 9600, 14400, 19200, 31250, 38400, 56000, 57600, 76800, 115200, 128000, 230400, 256000 };
                var result = r.Children["baudrate"];
                var value = (result.Tokens.Count == 0) ? settings.RtuMaster.Baudrate : r.ValueForOption<int>("baudrate");
                if (valid.Contains(value)) return null;
                return "Invalid Baudrate (select from: 110|150|300|600|1200|1800|2400|4800|7200|9600|14400|19200|31250|38400|56000|57600|76800|115200|128000|230400|256000).";
            });

            // Setup execution handler.
            Handler = CommandHandler.Create<IConsole, bool, RtuCommandOptions>((console, verbose, options) =>
            {
                logger.LogInformation("Handler()");

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
                        Console.WriteLine($"RTU serial port found at {options.SerialPort}.");
                        return ExitCodes.SuccessfullyCompleted;
                    }
                    else
                    {
                        Console.WriteLine($"RTU serial port not found at {options.SerialPort}.");
                        return ExitCodes.IncorrectFunction;
                    }
                }
                catch
                {
                    logger.LogError("RtuCommand exception");
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
