// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModbusController.cs" company="DTV-Online">
//   Copyright(c) 2020 Dr. Peter Trimmel. All rights reserved.
// </copyright>
// <license>
//   Licensed under the MIT license. See the LICENSE file in the project root for more information.
// </license>
// <created>13-5-2020 13:52</created>
// <author>Peter Trimmel</author>
// --------------------------------------------------------------------------------------------------------------------
namespace ModbusRTU.Controllers
{
    #region Using Directives

    using System;
    using System.Collections;
    using System.Text.Json.Serialization;
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;

    using NModbus.Extensions;

    using UtilityLib;
    using ModbusLib;
    using ModbusRTU.Models;

    #endregion

    /// <summary>
    /// All supported read request functions.
    /// </summary>
    public enum ReadRequestFunctions
    {
        ReadCoilAsync,
        ReadCoilsAsync,
        ReadInputAsync,
        ReadInputsAsync,
        ReadHoldingRegisterAsync,
        ReadHoldingRegistersAsync,
        ReadInputRegisterAsync,
        ReadInputRegistersAsync,
        ReadOnlyStringAsync,
        ReadOnlyHexStringAsync,
        ReadOnlyBoolAsync,
        ReadOnlyBitsAsync,
        ReadOnlyShortAsync,
        ReadOnlyUShortAsync,
        ReadOnlyInt32Async,
        ReadOnlyUInt32Async,
        ReadOnlyFloatAsync,
        ReadOnlyDoubleAsync,
        ReadOnlyLongAsync,
        ReadOnlyULongAsync,
        ReadOnlyBoolArrayAsync,
        ReadOnlyBytesAsync,
        ReadOnlyShortArrayAsync,
        ReadOnlyUShortArrayAsync,
        ReadOnlyInt32ArrayAsync,
        ReadOnlyUInt32ArrayAsync,
        ReadOnlyFloatArrayAsync,
        ReadOnlyDoubleArrayAsync,
        ReadOnlyLongArrayAsync,
        ReadOnlyULongArrayAsync,
        ReadStringAsync,
        ReadHexStringAsync,
        ReadBoolAsync,
        ReadBitsAsync,
        ReadShortAsync,
        ReadUShortAsync,
        ReadInt32Async,
        ReadUInt32Async,
        ReadFloatAsync,
        ReadDoubleAsync,
        ReadLongAsync,
        ReadULongAsync,
        ReadBoolArrayAsync,
        ReadBytesAsync,
        ReadShortArrayAsync,
        ReadUShortArrayAsync,
        ReadInt32ArrayAsync,
        ReadUInt32ArrayAsync,
        ReadFloatArrayAsync,
        ReadDoubleArrayAsync,
        ReadLongArrayAsync,
        ReadULongArrayAsync,
    }

    /// <summary>
    /// All supported write request functions.
    /// </summary>
    public enum WriteRequestFunctions
    {
        WriteCoilAsync,
        WriteHoldingRegisterAsync,
        WriteStringAsync,
        WriteHexStringAsync,
        WriteBoolAsync,
        WriteBitsAsync,
        WriteShortAsync,
        WriteUShortAsync,
        WriteInt32Async,
        WriteUInt32Async,
        WriteFloatAsync,
        WriteDoubleAsync,
        WriteLongAsync,
        WriteULongAsync,
    }

    /// <summary>
    /// All supported write array request functions.
    /// </summary>
    public enum WriteArrayRequestFunctions
    {
        WriteCoilsAsync,
        WriteHoldingRegistersAsync,
        WriteBoolArrayAsync,
        WriteBytesAsync,
        WriteShortArrayAsync,
        WriteUShortArrayAsync,
        WriteInt32ArrayAsync,
        WriteUInt32ArrayAsync,
        WriteFloatArrayAsync,
        WriteDoubleArrayAsync,
        WriteLongArrayAsync,
        WriteULongArrayAsync,
    }

    /// <summary>
    /// Baseclass for all Modbus Gateway MVC Controller reading and writing data.
    /// </summary>
    public class ModbusController : BaseController<AppSettings>
    {
        #region Protected Fields

        /// <summary>
        /// The Modbus RTU client instance.
        /// </summary>
        protected readonly IRtuModbusClient _client;

        /// <summary>
        /// Instantiate a Singleton of the Semaphore with a value of 1.
        /// This means that only 1 thread can be granted access at a time.
        /// </summary>
        static SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        #endregion Private Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ModbusController"/> class.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="settings"></param>
        /// <param name="config"></param>
        /// <param name="environment"></param>
        /// <param name="lifetime"></param>
        /// <param name="logger"></param>
        public ModbusController(IRtuModbusClient client,
                                AppSettings settings,
                                IConfiguration config,
                                IHostEnvironment environment,
                                IHostApplicationLifetime lifetime,
                                ILogger<ModbusController> logger)
            : base(settings, config, environment, lifetime, logger)
        {
            _client = client;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Returns true if no tasks can enter the semaphore.
        /// </summary>
        [JsonIgnore]
        public bool IsLocked { get => !(_semaphore.CurrentCount == 0); }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Method to send a Modbus read request to a Modbus slave and returns the requested value(s)
        /// providing common communication setup and associated exception handling (logging).
        /// A TCP client is created and used to send the request to the Modbus TCP client.
        /// The following requests are supported:
        ///
        ///     Single Coil
        ///     Single Discrete Input
        ///     Single Holding Register
        ///     Single Input Register
        ///
        ///     Multiple Coils
        ///     Multiple Discrete Inputs
        ///     Multiple Holding Registers
        ///     Multiple Input Registers
        ///
        /// Additional datatypes (single values, value arrays and strings) with read
        /// only access (discrete inputs and input registers) and read / write access
        /// (coils and holding registers) are supported:
        ///
        ///     ASCII String    (multiple registers)
        ///     Hex String      (multiple registers)
        ///     Bool            (single coil)
        ///     Bits            (single register)
        ///     Short           (single register)
        ///     UShort          (single register)
        ///     Int32           (two registers)
        ///     UInt32          (two registers)
        ///     Float           (two registers)
        ///     Double          (four registers)
        ///     Long            (four registers)
        ///     ULong           (four registers)
        ///
        /// </summary>
        /// <param name="request">The <see cref="ModbusRequestData"/> data.</param>
        /// <param name="function">The function enum.</param>
        /// <returns>A task returning an action method result.</returns>
        protected async Task<IActionResult> ModbusReadRequest(ModbusRequestData request, ReadRequestFunctions function)
        {
            request.Slave = _client.RtuSlave;
            request.Master = _client.RtuMaster;

            try
            {
                if (request is null) throw new ArgumentNullException("request");

                await _semaphore.WaitAsync();

                if (_client.Connect())
                {
                    switch (function)
                    {
                        case ReadRequestFunctions.ReadCoilAsync:
                            {
                                bool[] values = await _client.ReadCoilsAsync(request.Slave.ID, request.Offset, request.Number);
                                _logger.LogTrace($"{function}(): {values}");
                                return Ok(new ModbusResponseData<bool>() { Request = request, Value = values[0] });
                            }
                        case ReadRequestFunctions.ReadCoilsAsync:
                            {
                                bool[] values = await _client.ReadCoilsAsync(request.Slave.ID, request.Offset, request.Number);
                                _logger.LogTrace($"{function}(): {values}");
                                return Ok(new ModbusResponseArrayData<bool>() { Request = request, Values = values });
                            }
                        case ReadRequestFunctions.ReadInputAsync:
                            {
                                bool[] values = await _client.ReadInputsAsync(request.Slave.ID, request.Offset, request.Number);
                                _logger.LogTrace($"{function}(): {values}");
                                return Ok(new ModbusResponseData<bool>() { Request = request, Value = values[0] });
                            }
                        case ReadRequestFunctions.ReadInputsAsync:
                            {
                                bool[] values = await _client.ReadInputsAsync(request.Slave.ID, request.Offset, request.Number);
                                _logger.LogTrace($"{function}(): {values}");
                                return Ok(new ModbusResponseArrayData<bool>() { Request = request, Values = values });
                            }
                        case ReadRequestFunctions.ReadHoldingRegisterAsync:
                            {
                                ushort[] values = await _client.ReadHoldingRegistersAsync(request.Slave.ID, request.Offset, request.Number);
                                _logger.LogTrace($"{function}(): {values}");
                                return Ok(new ModbusResponseData<ushort>() { Request = request, Value = values[0] });
                            }
                        case ReadRequestFunctions.ReadHoldingRegistersAsync:
                            {
                                ushort[] values = await _client.ReadHoldingRegistersAsync(request.Slave.ID, request.Offset, request.Number);
                                _logger.LogTrace($"{function}(): {values}");
                                return Ok(new ModbusResponseArrayData<ushort>() { Request = request, Values = values });
                            }
                        case ReadRequestFunctions.ReadInputRegisterAsync:
                            {
                                ushort[] values = await _client.ReadInputRegistersAsync(request.Slave.ID, request.Offset, request.Number);
                                _logger.LogTrace($"{function}(): {values}");
                                return Ok(new ModbusResponseData<ushort>() { Request = request, Value = values[0] });
                            }
                        case ReadRequestFunctions.ReadInputRegistersAsync:
                            {
                                ushort[] values = await _client.ReadInputRegistersAsync(request.Slave.ID, request.Offset, request.Number);
                                _logger.LogTrace($"{function}(): {values}");
                                return Ok(new ModbusResponseArrayData<ushort>() { Request = request, Values = values });
                            }
                        case ReadRequestFunctions.ReadOnlyStringAsync:
                            {
                                string value = await _client.ReadOnlyStringAsync(request.Slave.ID, request.Offset, request.Number);
                                _logger.LogTrace($"{function}(): {value}");
                                return Ok(new ModbusResponseStringData() { Request = request, Value = value });

                            }
                        case ReadRequestFunctions.ReadOnlyHexStringAsync:
                            {
                                string value = await _client.ReadOnlyHexStringAsync(request.Slave.ID, request.Offset, request.Number);
                                _logger.LogTrace($"{function}(): {value}");
                                return Ok(new ModbusResponseStringData() { Request = request, Value = value });
                            }
                        case ReadRequestFunctions.ReadOnlyBoolAsync:
                            {
                                bool value = await _client.ReadOnlyBoolAsync(request.Slave.ID, request.Offset);
                                _logger.LogTrace($"{function}(): {value}");
                                return Ok(new ModbusResponseData<bool>() { Request = request, Value = value });
                            }
                        case ReadRequestFunctions.ReadOnlyBitsAsync:
                            {
                                BitArray value = await _client.ReadOnlyBitsAsync(request.Slave.ID, request.Offset);
                                _logger.LogTrace($"{function}(): {value}");
                                return Ok(new ModbusResponseStringData() { Request = request, Value = value.ToDigitString() });
                            }
                        case ReadRequestFunctions.ReadOnlyShortAsync:
                            {
                                short value = await _client.ReadOnlyShortAsync(request.Slave.ID, request.Offset);
                                _logger.LogTrace($"{function}(): {value}");
                                return Ok(new ModbusResponseData<short>() { Request = request, Value = value });
                            }
                        case ReadRequestFunctions.ReadOnlyUShortAsync:
                            {
                                ushort value = await _client.ReadOnlyUShortAsync(request.Slave.ID, request.Offset);
                                _logger.LogTrace($"{function}(): {value}");
                                return Ok(new ModbusResponseData<ushort>() { Request = request, Value = value });
                            }
                        case ReadRequestFunctions.ReadOnlyInt32Async:
                            {
                                int value = await _client.ReadOnlyInt32Async(request.Slave.ID, request.Offset);
                                _logger.LogTrace($"{function}(): {value}");
                                return Ok(new ModbusResponseData<int>() { Request = request, Value = value });
                            }
                        case ReadRequestFunctions.ReadOnlyUInt32Async:
                            {
                                uint value = await _client.ReadOnlyUInt32Async(request.Slave.ID, request.Offset);
                                _logger.LogTrace($"{function}(): {value}");
                                return Ok(new ModbusResponseData<uint>() { Request = request, Value = value });
                            }
                        case ReadRequestFunctions.ReadOnlyFloatAsync:
                            {
                                float value = await _client.ReadOnlyFloatAsync(request.Slave.ID, request.Offset);
                                _logger.LogTrace($"{function}(): {value}");
                                return Ok(new ModbusResponseData<float>() { Request = request, Value = value });
                            }
                        case ReadRequestFunctions.ReadOnlyDoubleAsync:
                            {
                                double value = await _client.ReadOnlyDoubleAsync(request.Slave.ID, request.Offset);
                                _logger.LogTrace($"{function}(): {value}");
                                return Ok(new ModbusResponseData<double>() { Request = request, Value = value });
                            }
                        case ReadRequestFunctions.ReadOnlyLongAsync:
                            {
                                long value = await _client.ReadOnlyLongAsync(request.Slave.ID, request.Offset);
                                _logger.LogTrace($"{function}(): {value}");
                                return Ok(new ModbusResponseData<long>() { Request = request, Value = value });
                            }
                        case ReadRequestFunctions.ReadOnlyULongAsync:
                            {
                                ulong value = await _client.ReadOnlyULongAsync(request.Slave.ID, request.Offset);
                                _logger.LogTrace($"{function}(): {value}");
                                return Ok(new ModbusResponseData<ulong>() { Request = request, Value = value });
                            }
                        case ReadRequestFunctions.ReadOnlyBoolArrayAsync:
                            {
                                bool[] values = await _client.ReadOnlyBoolArrayAsync(request.Slave.ID, request.Offset, request.Number);
                                _logger.LogTrace($"{function}(): {values}");
                                return Ok(new ModbusResponseArrayData<bool>() { Request = request, Values = values });
                            }
                        case ReadRequestFunctions.ReadOnlyBytesAsync:
                            {
                                byte[] values = await _client.ReadOnlyBytesAsync(request.Slave.ID, request.Offset, request.Number);
                                _logger.LogTrace($"{function}(): {values}");
                                return Ok(new ModbusResponseArrayData<byte>() { Request = request, Values = values });
                            }
                        case ReadRequestFunctions.ReadOnlyShortArrayAsync:
                            {
                                short[] values = await _client.ReadOnlyShortArrayAsync(request.Slave.ID, request.Offset, request.Number);
                                _logger.LogTrace($"{function}(): {values}");
                                return Ok(new ModbusResponseArrayData<short>() { Request = request, Values = values });
                            }
                        case ReadRequestFunctions.ReadOnlyUShortArrayAsync:
                            {
                                ushort[] values = await _client.ReadOnlyUShortArrayAsync(request.Slave.ID, request.Offset, request.Number);
                                _logger.LogTrace($"{function}(): {values}");
                                return Ok(new ModbusResponseArrayData<ushort>() { Request = request, Values = values });
                            }
                        case ReadRequestFunctions.ReadOnlyInt32ArrayAsync:
                            {
                                int[] values = await _client.ReadOnlyInt32ArrayAsync(request.Slave.ID, request.Offset, request.Number);
                                _logger.LogTrace($"{function}(): {values}");
                                return Ok(new ModbusResponseArrayData<int>() { Request = request, Values = values });
                            }
                        case ReadRequestFunctions.ReadOnlyUInt32ArrayAsync:
                            {
                                uint[] values = await _client.ReadOnlyUInt32ArrayAsync(request.Slave.ID, request.Offset, request.Number);
                                _logger.LogTrace($"{function}(): {values}");
                                return Ok(new ModbusResponseArrayData<uint>() { Request = request, Values = values });
                            }
                        case ReadRequestFunctions.ReadOnlyFloatArrayAsync:
                            {
                                float[] values = await _client.ReadOnlyFloatArrayAsync(request.Slave.ID, request.Offset, request.Number);
                                _logger.LogTrace($"{function}(): {values}");
                                return Ok(new ModbusResponseArrayData<float>() { Request = request, Values = values });
                            }
                        case ReadRequestFunctions.ReadOnlyDoubleArrayAsync:
                            {
                                double[] values = await _client.ReadOnlyDoubleArrayAsync(request.Slave.ID, request.Offset, request.Number);
                                _logger.LogTrace($"{function}(): {values}");
                                return Ok(new ModbusResponseArrayData<double>() { Request = request, Values = values });
                            }
                        case ReadRequestFunctions.ReadOnlyLongArrayAsync:
                            {
                                long[] values = await _client.ReadOnlyLongArrayAsync(request.Slave.ID, request.Offset, request.Number);
                                _logger.LogTrace($"{function}(): {values}");
                                return Ok(new ModbusResponseArrayData<long>() { Request = request, Values = values });
                            }
                        case ReadRequestFunctions.ReadOnlyULongArrayAsync:
                            {
                                ulong[] values = await _client.ReadOnlyULongArrayAsync(request.Slave.ID, request.Offset, request.Number);
                                _logger.LogTrace($"{function}(): {values}");
                                return Ok(new ModbusResponseArrayData<ulong>() { Request = request, Values = values });
                            }
                        case ReadRequestFunctions.ReadStringAsync:
                            {
                                string value = await _client.ReadStringAsync(request.Slave.ID, request.Offset, request.Number);
                                _logger.LogTrace($"{function}(): {value}");
                                return Ok(new ModbusResponseStringData() { Request = request, Value = value });
                            }
                        case ReadRequestFunctions.ReadHexStringAsync:
                            {
                                string value = await _client.ReadHexStringAsync(request.Slave.ID, request.Offset, request.Number);
                                _logger.LogTrace($"{function}(): {value}");
                                return Ok(new ModbusResponseStringData() { Request = request, Value = value });
                            }
                        case ReadRequestFunctions.ReadBoolAsync:
                            {
                                bool value = await _client.ReadBoolAsync(request.Slave.ID, request.Offset);
                                _logger.LogTrace($"{function}(): {value}");
                                return Ok(new ModbusResponseData<bool>() { Request = request, Value = value });
                            }
                        case ReadRequestFunctions.ReadBitsAsync:
                            {
                                BitArray value = await _client.ReadBitsAsync(request.Slave.ID, request.Offset);
                                _logger.LogTrace($"{function}(): {value}");
                                return Ok(new ModbusResponseStringData() { Request = request, Value = value.ToDigitString() });
                            }
                        case ReadRequestFunctions.ReadShortAsync:
                            {
                                short value = await _client.ReadShortAsync(request.Slave.ID, request.Offset);
                                _logger.LogTrace($"{function}(): {value}");
                                return Ok(new ModbusResponseData<short>() { Request = request, Value = value });
                            }
                        case ReadRequestFunctions.ReadUShortAsync:
                            {
                                ushort value = await _client.ReadUShortAsync(request.Slave.ID, request.Offset);
                                _logger.LogTrace($"{function}(): {value}");
                                return Ok(new ModbusResponseData<ushort>() { Request = request, Value = value });
                            }
                        case ReadRequestFunctions.ReadInt32Async:
                            {
                                int value = await _client.ReadInt32Async(request.Slave.ID, request.Offset);
                                _logger.LogTrace($"{function}(): {value}");
                                return Ok(new ModbusResponseData<int>() { Request = request, Value = value });
                            }
                        case ReadRequestFunctions.ReadUInt32Async:
                            {
                                uint value = await _client.ReadUInt32Async(request.Slave.ID, request.Offset);
                                _logger.LogTrace($"{function}(): {value}");
                                return Ok(new ModbusResponseData<uint>() { Request = request, Value = value });
                            }
                        case ReadRequestFunctions.ReadFloatAsync:
                            {
                                float value = await _client.ReadFloatAsync(request.Slave.ID, request.Offset);
                                _logger.LogTrace($"{function}(): {value}");
                                return Ok(new ModbusResponseData<float>() { Request = request, Value = value });
                            }
                        case ReadRequestFunctions.ReadDoubleAsync:
                            {
                                double value = await _client.ReadDoubleAsync(request.Slave.ID, request.Offset);
                                _logger.LogTrace($"{function}(): {value}");
                                return Ok(new ModbusResponseData<double>() { Request = request, Value = value });
                            }
                        case ReadRequestFunctions.ReadLongAsync:
                            {
                                long value = await _client.ReadLongAsync(request.Slave.ID, request.Offset);
                                _logger.LogTrace($"{function}(): {value}");
                                return Ok(new ModbusResponseData<long>() { Request = request, Value = value });
                            }
                        case ReadRequestFunctions.ReadULongAsync:
                            {
                                ulong value = await _client.ReadULongAsync(request.Slave.ID, request.Offset);
                                _logger.LogTrace($"{function}(): {value}");
                                return Ok(new ModbusResponseData<ulong>() { Request = request, Value = value });
                            }
                        case ReadRequestFunctions.ReadBoolArrayAsync:
                            {
                                bool[] values = await _client.ReadBoolArrayAsync(request.Slave.ID, request.Offset, request.Number);
                                _logger.LogTrace($"{function}(): {values}");
                                return Ok(new ModbusResponseArrayData<bool>() { Request = request, Values = values });
                            }
                        case ReadRequestFunctions.ReadBytesAsync:
                            {
                                byte[] values = await _client.ReadBytesAsync(request.Slave.ID, request.Offset, request.Number);
                                _logger.LogTrace($"{function}(): {values}");
                                return Ok(new ModbusResponseArrayData<byte>() { Request = request, Values = values });
                            }
                        case ReadRequestFunctions.ReadShortArrayAsync:
                            {
                                short[] values = await _client.ReadShortArrayAsync(request.Slave.ID, request.Offset, request.Number);
                                _logger.LogTrace($"{function}(): {values}");
                                return Ok(new ModbusResponseArrayData<short>() { Request = request, Values = values });
                            }
                        case ReadRequestFunctions.ReadUShortArrayAsync:
                            {
                                ushort[] values = await _client.ReadUShortArrayAsync(request.Slave.ID, request.Offset, request.Number);
                                _logger.LogTrace($"{function}(): {values}");
                                return Ok(new ModbusResponseArrayData<ushort>() { Request = request, Values = values });
                            }
                        case ReadRequestFunctions.ReadInt32ArrayAsync:
                            {
                                int[] values = await _client.ReadInt32ArrayAsync(request.Slave.ID, request.Offset, request.Number);
                                _logger.LogTrace($"{function}(): {values}");
                                return Ok(new ModbusResponseArrayData<int>() { Request = request, Values = values });
                            }
                        case ReadRequestFunctions.ReadUInt32ArrayAsync:
                            {
                                uint[] values = await _client.ReadUInt32ArrayAsync(request.Slave.ID, request.Offset, request.Number);
                                _logger.LogTrace($"{function}(): {values}");
                                return Ok(new ModbusResponseArrayData<uint>() { Request = request, Values = values });
                            }
                        case ReadRequestFunctions.ReadFloatArrayAsync:
                            {
                                float[] values = await _client.ReadFloatArrayAsync(request.Slave.ID, request.Offset, request.Number);
                                _logger.LogTrace($"{function}(): {values}");
                                return Ok(new ModbusResponseArrayData<float>() { Request = request, Values = values });
                            }
                        case ReadRequestFunctions.ReadDoubleArrayAsync:
                            {
                                double[] values = await _client.ReadDoubleArrayAsync(request.Slave.ID, request.Offset, request.Number);
                                _logger.LogTrace($"{function}(): {values}");
                                return Ok(new ModbusResponseArrayData<double>() { Request = request, Values = values });
                            }
                        case ReadRequestFunctions.ReadLongArrayAsync:
                            {
                                long[] values = await _client.ReadLongArrayAsync(request.Slave.ID, request.Offset, request.Number);
                                _logger.LogTrace($"{function}(): {values}");
                                return Ok(new ModbusResponseArrayData<long>() { Request = request, Values = values });
                            }
                        case ReadRequestFunctions.ReadULongArrayAsync:
                            {
                                ulong[] values = await _client.ReadULongArrayAsync(request.Slave.ID, request.Offset, request.Number);
                                _logger.LogTrace($"{function}(): {values}");
                                return Ok(new ModbusResponseArrayData<ulong>() { Request = request, Values = values });
                            }
                        default:
                            _logger.LogError($"RTU master read request {function}() not supported.");
                            return NotFound($"RTU master read request {function}() not supported.");
                    }
                }
                else
                {
                    _logger.LogError($"RTU master ({request.Master.SerialPort}) not open.");
                    return NotFound("RTU master COM port not open.");
                }
            }
            catch (UnauthorizedAccessException uae)
            {
                _logger.LogError(uae, $"{function}() Unauthorized Access Exception.");
                return NotFound($"Unauthorized Access Exception: {uae.Message}");
            }
            catch (ArgumentNullException anx)
            {
                _logger.LogError(anx, $"{function}() ArgumentNullException.");
                return BadRequest($"Argument Null Exception: {anx.Message}");
            }
            catch (ArgumentOutOfRangeException are)
            {
                _logger.LogError(are, $"{function}() ArgumentOutOfRangeException.");
                return BadRequest($"Argument out of Range Exception: {are.Message}");
            }
            catch (ArgumentException aex)
            {
                _logger.LogError(aex, $"{function}() ArgumentException.");
                return BadRequest($"Argument Exception: {aex.Message}");
            }
            catch (FormatException fex)
            {
                _logger.LogError(fex, $"{function}() FormatException.");
                return BadRequest($"Format Exception: {fex.Message}");
            }
            catch (NModbus.SlaveException mse)
            {
                _logger.LogError(mse, $"{function}() Modbus SlaveException.");
                return StatusCode(502, $"Modbus SlaveException: {mse.Message}");
            }
            catch (InvalidOperationException ioe)
            {
                _logger.LogError(ioe, $"{function}() InvalidOperationException.");
                return StatusCode(500, $"Invalid Operation Exception: {ioe.Message}");
            }
            catch (System.IO.IOException ioe)
            {
                _logger.LogError(ioe, $"{function}() IO Exception.");
                return StatusCode(500, $"IO Exception: {ioe.Message}");
            }
            catch (TimeoutException tex)
            {
                _logger.LogError(tex, $"{function}() TimeoutException.");
                return StatusCode(500, $"Timeout Exception: {tex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{function}() Exception.");
                return StatusCode(500, $"Exception: {ex.Message}");
            }
            finally
            {
                if (_client.Connected)
                {
                    _client.Disconnect();
                }

                _semaphore.Release();
            }
        }

        /// <summary>
        /// Method to send a Modbus write request to a Modbus slave. providing common
        /// communication setup and associated exception handling (logging).
        /// A TCP client is created and used to send the request to the Modbus TCP client.
        /// The following requests are supported:
        ///
        ///     Single Coil
        ///     Single Holding Register
        ///
        /// Additional datatypes with read / write access (coils and holding registers) are supported:
        ///
        ///     ASCII String    (multiple holding registers)
        ///     Hex String      (multiple holding registers)
        ///     Bool            (single coil)
        ///     Bits            (single holding register)
        ///     Short           (single holding register)
        ///     UShort          (single holding register)
        ///     Int32           (two holding registers)
        ///     UInt32          (two holding registers)
        ///     Float           (two holding registers)
        ///     Double          (four holding registers)
        ///     Long            (four holding registers)
        ///     ULong           (four holding registers)
        ///
        /// </summary>
        /// <param name="request">The <see cref="ModbusRequestData"/> data.</param>
        /// <param name="data">The data value.</param>
        /// <param name="function">The function enum.</param>
        /// <returns>A task returning an action method result.</returns>
        protected async Task<IActionResult> ModbusWriteSingleRequest<T>(ModbusRequestData request, T data, WriteRequestFunctions function)
        {
            request.Slave = _client.RtuSlave;
            request.Master = _client.RtuMaster;

            try
            {
                if (request is null) throw new ArgumentNullException("request");
                if (data is null) throw new ArgumentNullException("data");

                await _semaphore.WaitAsync();

                if (_client.Connect())
                {
                    switch (function)
                    {
                        case WriteRequestFunctions.WriteCoilAsync:
                            {
                                bool? value = (bool?)Convert.ChangeType(data, typeof(bool));

                                if (value.HasValue)
                                {
                                    await _client.WriteSingleCoilAsync(request.Slave.ID, request.Offset, value.Value);
                                    _logger.LogTrace($"{function}() OK.");
                                    return Ok(request);
                                }

                                _logger.LogTrace($"{function}() not OK.");
                                return Conflict(request);
                            }
                        case WriteRequestFunctions.WriteHoldingRegisterAsync:
                            {
                                ushort? value = (ushort?)Convert.ChangeType(data, typeof(ushort));

                                if (value.HasValue)
                                {
                                    await _client.WriteSingleRegisterAsync(request.Slave.ID, request.Offset, value.Value);
                                    _logger.LogTrace($"{function}() OK.");
                                    return Ok(request);
                                }

                                _logger.LogTrace($"{function}() not OK.");
                                return Conflict(request);
                            }
                        case WriteRequestFunctions.WriteStringAsync:
                            {
                                string? value = (string?)Convert.ChangeType(data, typeof(string));

                                if (!string.IsNullOrEmpty(value))
                                {
                                    await _client.WriteStringAsync(request.Slave.ID, request.Offset, value);
                                    _logger.LogTrace($"{function}() OK.");
                                    return Ok(request);
                                }

                                _logger.LogTrace($"{function}() not OK.");
                                return Conflict(request);
                            }
                        case WriteRequestFunctions.WriteHexStringAsync:
                            {
                                string? value = (string?)Convert.ChangeType(data, typeof(string));

                                if (!string.IsNullOrEmpty(value))
                                {
                                    await _client.WriteHexStringAsync(request.Slave.ID, request.Offset, value);
                                    _logger.LogTrace($"{function}() OK.");
                                    return Ok(request);
                                }

                                _logger.LogTrace($"{function}() not OK.");
                                return Conflict(request);
                            }
                        case WriteRequestFunctions.WriteBoolAsync:
                            {
                                bool? value = (bool?)Convert.ChangeType(data, typeof(bool));

                                if (value.HasValue)
                                {
                                    await _client.WriteBoolAsync(request.Slave.ID, request.Offset, value.Value);
                                    _logger.LogTrace($"{function}() OK.");
                                    return Ok(request);
                                }

                                _logger.LogTrace($"{function}() not OK.");
                                return Conflict(request);
                            }
                        case WriteRequestFunctions.WriteBitsAsync:
                            {
                                var bitstring = (string?)Convert.ChangeType(data, typeof(string));

                                if (!string.IsNullOrEmpty(bitstring))
                                {
                                    BitArray value = bitstring.ToBitArray();
                                    await _client.WriteBitsAsync(request.Slave.ID, request.Offset, value);
                                    _logger.LogTrace($"{function}() OK.");
                                    return Ok(request);
                                }

                                _logger.LogTrace($"{function}() not OK.");
                                return Conflict(request);
                            }
                        case WriteRequestFunctions.WriteShortAsync:
                            {
                                short? value = (short?)Convert.ChangeType(data, typeof(short));

                                if (value.HasValue)
                                {
                                    await _client.WriteShortAsync(request.Slave.ID, request.Offset, value.Value);
                                    _logger.LogTrace($"{function}() OK.");
                                    return Ok(request);
                                }

                                _logger.LogTrace($"{function}() not OK.");
                                return Conflict(request);
                            }
                        case WriteRequestFunctions.WriteUShortAsync:
                            {
                                ushort? value = (ushort?)Convert.ChangeType(data, typeof(ushort));

                                if (value.HasValue)
                                {
                                    await _client.WriteUShortAsync(request.Slave.ID, request.Offset, value.Value);
                                    _logger.LogTrace($"{function}() OK.");
                                    return Ok(request);
                                }

                                _logger.LogTrace($"{function}() not OK.");
                                return Conflict(request);
                            }
                        case WriteRequestFunctions.WriteInt32Async:
                            {
                                int? value = (int?)Convert.ChangeType(data, typeof(int));

                                if (value.HasValue)
                                {
                                    await _client.WriteInt32Async(request.Slave.ID, request.Offset, value.Value);
                                    _logger.LogTrace($"{function}() OK.");
                                    return Ok(request);
                                }

                                _logger.LogTrace($"{function}() not OK.");
                                return Conflict(request);
                            }
                        case WriteRequestFunctions.WriteUInt32Async:
                            {
                                uint? value = (uint?)Convert.ChangeType(data, typeof(uint));

                                if (value.HasValue)
                                {
                                    await _client.WriteUInt32Async(request.Slave.ID, request.Offset, value.Value);
                                    _logger.LogTrace($"{function}() OK.");
                                    return Ok(request);
                                }

                                _logger.LogTrace($"{function}() not OK.");
                                return Conflict(request);
                            }
                        case WriteRequestFunctions.WriteFloatAsync:
                            {
                                float? value = (float?)Convert.ChangeType(data, typeof(float));

                                if (value.HasValue)
                                {
                                    await _client.WriteFloatAsync(request.Slave.ID, request.Offset, value.Value);
                                    _logger.LogTrace($"{function}() OK.");
                                    return Ok(request);
                                }

                                _logger.LogTrace($"{function}() not OK.");
                                return Conflict(request);
                            }
                        case WriteRequestFunctions.WriteDoubleAsync:
                            {
                                double? value = (double?)Convert.ChangeType(data, typeof(double));

                                if (value.HasValue)
                                {
                                    await _client.WriteDoubleAsync(request.Slave.ID, request.Offset, value.Value);
                                    _logger.LogTrace($"{function}() OK.");
                                    return Ok(request);
                                }

                                _logger.LogTrace($"{function}() not OK.");
                                return Conflict(request);
                            }
                        case WriteRequestFunctions.WriteLongAsync:
                            {
                                long? value = (long?)Convert.ChangeType(data, typeof(long));

                                if (value.HasValue)
                                {
                                    await _client.WriteLongAsync(request.Slave.ID, request.Offset, value.Value);
                                    _logger.LogTrace($"{function}() OK.");
                                    return Ok(request);
                                }

                                _logger.LogTrace($"{function}() not OK.");
                                return Conflict(request);
                            }
                        case WriteRequestFunctions.WriteULongAsync:
                            {
                                ulong? value = (ulong?)Convert.ChangeType(data, typeof(ulong));

                                if (value.HasValue)
                                {
                                    await _client.WriteULongAsync(request.Slave.ID, request.Offset, value.Value);
                                    _logger.LogTrace($"{function}() OK.");
                                    return Ok(request);
                                }

                                _logger.LogTrace($"{function}() not OK.");
                                return Conflict(request);
                            }
                        default:
                            _client.Disconnect();
                            _logger.LogError($"RTU master write request {function}() not supported.");
                            return NotFound($"RTU master write request {function}() not supported.");
                    }
                }
                else
                {
                    _logger.LogError($"RTU master ({request.Master.SerialPort}) not open.");
                    return NotFound("RTU master COM port not open.");
                }
            }
            catch (UnauthorizedAccessException uae)
            {
                _logger.LogError(uae, $"{function}() Unauthorized Access Exception.");
                return NotFound($"Unauthorized Access Exception: {uae.Message}");
            }
            catch (ArgumentNullException anx)
            {
                _logger.LogError(anx, $"{function}() ArgumentNullException.");
                return BadRequest($"Argument Null Exception: {anx.Message}");
            }
            catch (ArgumentOutOfRangeException are)
            {
                _logger.LogError(are, $"{function}() ArgumentOutOfRangeException.");
                return BadRequest($"Argument out of Range Exception: {are.Message}");
            }
            catch (ArgumentException aex)
            {
                _logger.LogError(aex, $"{function}() Argument Exception.");
                return BadRequest($"Argument Exception: {aex.Message}");
            }
            catch (FormatException fex)
            {
                _logger.LogError(fex, $"{function}() Format Exception.");
                return BadRequest($"Format Exception: {fex.Message}");
            }
            catch (NModbus.SlaveException mse)
            {
                _logger.LogError(mse, $"{function}() Modbus SlaveException.");
                return StatusCode(502, $"Modbus SlaveException: {mse.Message}");
            }
            catch (InvalidOperationException ioe)
            {
                _logger.LogError(ioe, $"{function}() Invalid Operation Exception.");
                return StatusCode(500, $"Invalid Operation Exception: {ioe.Message}");
            }
            catch (System.IO.IOException ioe)
            {
                _logger.LogError(ioe, $"{function}() IO Exception.");
                return StatusCode(500, $"IO Exception: {ioe.Message}");
            }
            catch (TimeoutException tex)
            {
                _logger.LogError(tex, $"{function}() Timeout Exception.");
                return StatusCode(500, $"Timeout Exception: {tex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{function}() Exception.");
                return StatusCode(500, $"Exception: {ex.Message}");
            }
            finally
            {
                if (_client.Connected)
                {
                    _client.Disconnect();
                }

                _semaphore.Release();
            }
        }

        /// <summary>
        /// Method to send a Modbus write request to a Modbus slave providing common
        /// communication setup and associated exception handling (logging).
        /// A TCP client is created and used to send the request to the Modbus TCP client.
        /// The following requests are supported:
        ///
        ///     Multiple Coils
        ///     Multiple Holding Register
        ///
        /// Additional datatypes with read / write access (coils and holding registers) are supported:
        ///
        ///     Bool            (multiple coils)
        ///     Bytes           (multiple holding registers)
        ///     Short           (multiple holding registers)
        ///     UShort          (multiple holding registers)
        ///     Int32           (multiple holding registers)
        ///     UInt32          (multiple holding registers)
        ///     Float           (multiple holding registers)
        ///     Double          (multiple holding registers)
        ///     Long            (multiple holding registers)
        ///     ULong           (multiple holding registers)
        ///
        /// </summary>
        /// <param name="request">The <see cref="ModbusRequestData"/> data.</param>
        /// <param name="data">The data value.</param>
        /// <param name="function">The function enum.</param>
        /// <returns>A task returning an action method result.</returns>
        protected async Task<IActionResult> ModbusWriteArrayRequest<T>(ModbusRequestData request, T[] data, WriteArrayRequestFunctions function)
        {
            request.Slave = _client.RtuSlave;
            request.Master = _client.RtuMaster;

            try
            {
                if (request is null) throw new ArgumentNullException("request");
                if (data is null) throw new ArgumentNullException("data");
                if (data.Length == 0) throw new ArgumentException("Length of data array is zero");

                await _semaphore.WaitAsync();

                if (_client.Connect())
                {
                    switch (function)
                    {
                        case WriteArrayRequestFunctions.WriteCoilsAsync:
                            {
                                bool[] values = (bool[])Convert.ChangeType(data, typeof(bool[]));
                                await _client.WriteMultipleCoilsAsync(request.Slave.ID, request.Offset, values);
                                _logger.LogTrace($"{function}() OK.");
                                return Ok(request);
                            }
                        case WriteArrayRequestFunctions.WriteHoldingRegistersAsync:
                            {
                                ushort[] values = (ushort[])Convert.ChangeType(data, typeof(ushort[]));
                                await _client.WriteMultipleRegistersAsync(request.Slave.ID, request.Offset, values);
                                _logger.LogTrace($"{function}() OK.");
                                return Ok(request);
                            }
                        case WriteArrayRequestFunctions.WriteBoolArrayAsync:
                            {
                                bool[] values = (bool[])Convert.ChangeType(data, typeof(bool[]));
                                await _client.WriteBoolArrayAsync(request.Slave.ID, request.Offset, values);
                                _logger.LogTrace($"{function}() OK.");
                                return Ok(request);
                            }
                        case WriteArrayRequestFunctions.WriteBytesAsync:
                            {
                                byte[] values = (byte[])Convert.ChangeType(data, typeof(byte[]));
                                await _client.WriteBytesAsync(request.Slave.ID, request.Offset, values);
                                _logger.LogTrace($"{function}() OK.");
                                return Ok(request);
                            }
                        case WriteArrayRequestFunctions.WriteShortArrayAsync:
                            {
                                short[] values = (short[])Convert.ChangeType(data, typeof(short[]));
                                await _client.WriteShortArrayAsync(request.Slave.ID, request.Offset, values);
                                _logger.LogTrace($"{function}() OK.");
                                return Ok(request);
                            }
                        case WriteArrayRequestFunctions.WriteUShortArrayAsync:
                            {
                                ushort[] values = (ushort[])Convert.ChangeType(data, typeof(ushort[]));
                                await _client.WriteUShortArrayAsync(request.Slave.ID, request.Offset, values);
                                _logger.LogTrace($"{function}() OK.");
                                return Ok(request);
                            }
                        case WriteArrayRequestFunctions.WriteInt32ArrayAsync:
                            {
                                int[] values = (int[])Convert.ChangeType(data, typeof(int[]));
                                await _client.WriteInt32ArrayAsync(request.Slave.ID, request.Offset, values);
                                _logger.LogTrace($"{function}() OK.");
                                return Ok(request);
                            }
                        case WriteArrayRequestFunctions.WriteUInt32ArrayAsync:
                            {
                                uint[] values = (uint[])Convert.ChangeType(data, typeof(uint[]));
                                await _client.WriteUInt32ArrayAsync(request.Slave.ID, request.Offset, values);
                                _logger.LogTrace($"{function}() OK.");
                                return Ok(request);
                            }
                        case WriteArrayRequestFunctions.WriteFloatArrayAsync:
                            {
                                float[] values = (float[])Convert.ChangeType(data, typeof(float[]));
                                await _client.WriteFloatArrayAsync(request.Slave.ID, request.Offset, values);
                                _logger.LogTrace($"{function}() OK.");
                                return Ok(request);
                            }
                        case WriteArrayRequestFunctions.WriteDoubleArrayAsync:
                            {
                                double[] values = (double[])Convert.ChangeType(data, typeof(double[]));
                                await _client.WriteDoubleArrayAsync(request.Slave.ID, request.Offset, values);
                                _logger.LogTrace($"{function}() OK.");
                                return Ok(request);
                            }
                        case WriteArrayRequestFunctions.WriteLongArrayAsync:
                            {
                                long[] values = (long[])Convert.ChangeType(data, typeof(long[]));
                                await _client.WriteLongArrayAsync(request.Slave.ID, request.Offset, values);
                                _logger.LogTrace($"{function}() OK.");
                                return Ok(request);
                            }
                        case WriteArrayRequestFunctions.WriteULongArrayAsync:
                            {
                                ulong[] values = (ulong[])Convert.ChangeType(data, typeof(ulong[]));
                                await _client.WriteULongArrayAsync(request.Slave.ID, request.Offset, values);
                                _logger.LogTrace($"{function}() OK.");
                                return Ok(request);
                            }
                        default:
                            _client.Disconnect();
                            _logger.LogError($"RTU master write request {function}() not supported.");
                            return NotFound($"RTU master write request {function}() not supported.");
                    }
                }
                else
                {
                    _logger.LogError($"RTU master ({request.Master.SerialPort}) not open.");
                    return NotFound("RTU master COM port not open.");
                }
            }
            catch (UnauthorizedAccessException uae)
            {
                _logger.LogError(uae, $"{function}() Unauthorized Access Exception.");
                return NotFound($"Unauthorized Access Exception: {uae.Message}");
            }
            catch (ArgumentNullException anx)
            {
                _logger.LogError(anx, $"{function}() ArgumentNullException.");
                return BadRequest($"Argument Null Exception: {anx.Message}");
            }
            catch (InvalidCastException icx)
            {
                _logger.LogError(icx, $"{function}() InvalidCastException.");
                return BadRequest($"Error in argument conversion: {icx.Message}");
            }
            catch (FormatException fex)
            {
                _logger.LogError(fex, $"{function}() FormatException.");
                return BadRequest($"Format Exception: {fex.Message}");
            }
            catch (OverflowException ofx)
            {
                _logger.LogError(ofx, $"{function}() OverflowException.");
                return BadRequest($"Error in argument conversion: {ofx.Message}");
            }
            catch (ArgumentOutOfRangeException arx)
            {
                _logger.LogError(arx, $"{function}() ArgumentOutOfRangeException.");
                return BadRequest($"Argument out of Range Exception: {arx.Message}");
            }
            catch (ArgumentException aex)
            {
                _logger.LogError(aex, $"{function}() ArgumentException.");
                return BadRequest($"Argument Exception: {aex.Message}");
            }
            catch (NModbus.SlaveException mse)
            {
                _logger.LogError(mse, $"{function}() Modbus SlaveException.");
                return StatusCode(502, $"Modbus SlaveException: {mse.Message}");
            }
            catch (InvalidOperationException ioe)
            {
                _logger.LogError(ioe, $"{function}() InvalidOperationException.");
                return StatusCode(500, $"Invalid Operation Exception: {ioe.Message}");
            }
            catch (System.IO.IOException ioe)
            {
                _logger.LogError(ioe, $"{function}() IO Exception.");
                return StatusCode(500, $"IO Exception: {ioe.Message}");
            }
            catch (TimeoutException tex)
            {
                _logger.LogError(tex, $"{function}() TimeoutException.");
                return StatusCode(500, $"Timeout Exception: {tex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{function}() Exception.");
                return StatusCode(500, $"Exception: {ex.Message}");
            }
            finally
            {
                if (_client.Connected)
                {
                    _client.Disconnect();
                }

                _semaphore.Release();
            }
        }

        #endregion
    }
}
