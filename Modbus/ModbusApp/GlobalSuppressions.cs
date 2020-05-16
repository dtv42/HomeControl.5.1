// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "<Pending>", Scope = "member", Target = "~M:ModbusApp.Commands.RootCommand.OnExecute~System.Int32")]
[assembly: SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "<Pending>", Scope = "member", Target = "~M:ModbusApp.Commands.TcpMonitorCommand.OnExecuteAsync(System.Threading.CancellationToken)~System.Threading.Tasks.Task{System.Int32}")]
[assembly: SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>", Scope = "member", Target = "~M:ModbusApp.Commands.RtuCommand.#ctor(ModbusLib.IRtuModbusClient,ModbusApp.Commands.RtuReadCommand,ModbusApp.Commands.RtuWriteCommand,ModbusApp.Commands.RtuMonitorCommand,ModbusApp.Models.AppSettings,Microsoft.Extensions.Logging.ILogger{ModbusApp.Commands.RtuCommand})")]
[assembly: SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>", Scope = "member", Target = "~M:ModbusApp.Commands.RtuMonitorCommand.#ctor(ModbusLib.IRtuModbusClient,Microsoft.Extensions.Logging.ILogger{ModbusApp.Commands.RtuMonitorCommand})")]
[assembly: SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>", Scope = "member", Target = "~M:ModbusApp.Commands.RtuReadCommand.#ctor(ModbusLib.IRtuModbusClient,Microsoft.Extensions.Logging.ILogger{ModbusApp.Commands.RtuReadCommand})")]
[assembly: SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>", Scope = "member", Target = "~M:ModbusApp.Commands.RtuWriteCommand.#ctor(ModbusLib.IRtuModbusClient,Microsoft.Extensions.Logging.ILogger{ModbusApp.Commands.RtuReadCommand})")]
[assembly: SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>", Scope = "member", Target = "~M:ModbusApp.Commands.TcpMonitorCommand.#ctor(ModbusLib.ITcpModbusClient,Microsoft.Extensions.Logging.ILogger{ModbusApp.Commands.TcpMonitorCommand})")]
[assembly: SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>", Scope = "member", Target = "~M:ModbusApp.Commands.TcpReadCommand.#ctor(ModbusLib.ITcpModbusClient,Microsoft.Extensions.Logging.ILogger{ModbusApp.Commands.TcpReadCommand})")]
[assembly: SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>", Scope = "member", Target = "~M:ModbusApp.Commands.TcpWriteCommand.#ctor(ModbusLib.ITcpModbusClient,Microsoft.Extensions.Logging.ILogger{ModbusApp.Commands.TcpWriteCommand})")]
[assembly: SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>", Scope = "member", Target = "~M:ModbusApp.Commands.TcpCommand.#ctor(ModbusLib.ITcpModbusClient,ModbusApp.Commands.TcpReadCommand,ModbusApp.Commands.TcpWriteCommand,ModbusApp.Commands.TcpMonitorCommand,ModbusApp.Models.AppSettings,Microsoft.Extensions.Logging.ILogger{ModbusApp.Commands.TcpCommand})")]
