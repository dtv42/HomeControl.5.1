// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TcpModbusClient.cs" company="DTV-Online">
//   Copyright(c) 2020 Dr. Peter Trimmel. All rights reserved.
// </copyright>
// <license>
//   Licensed under the MIT license. See the LICENSE file in the project root for more information.
// </license>
// <created>13-5-2020 13:52</created>
// <author>Peter Trimmel</author>
// --------------------------------------------------------------------------------------------------------------------
namespace ModbusLib
{
    #region Using Directives

    using System;
    using System.Collections;
    using System.Net.Sockets;
    using System.Threading.Tasks;

    using NModbus;
    using NModbus.Extensions;

    using ModbusLib.Models;

    #endregion Using Directives

    public class TcpModbusClient : ITcpModbusClient
    {
        #region Private Data Members

        private readonly ModbusFactory _factory;
        private TcpClient? _client;

        #endregion Private Data Members

        #region Public Properties

        /// <summary>
        /// Gets the NModbus master.
        /// </summary>
        public IModbusMaster ModbusMaster { get; private set; } = NullMaster.CreateModbusMaster();

        /// <summary>
        /// Gets or sets the TCP/IP Modbus master data.
        /// </summary>
        public TcpMasterData TcpMaster { get; set; } = new TcpMasterData();

        /// <summary>
        /// Gets or sets the TCP/IP Modbus slave data.
        /// </summary>
        public TcpSlaveData TcpSlave { get; set; } = new TcpSlaveData();

        /// <summary>
        /// Gets or sets the swap byte flag.
        /// </summary>
        public bool SwapBytes { get; set; }

        /// <summary>
        /// Gets or sets the swap word flag.
        /// </summary>
        public bool SwapWords { get; set; }

        /// <summary>
        /// Gets a value indicating whether the socket is connected to a host.
        /// </summary>
        public bool Connected { get => _client?.Connected ?? false; }

        #endregion Public Properties

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TcpModbusClient"/> class.
        /// </summary>
        /// <param name="settings">The TCP client settings</param>
        public TcpModbusClient(ITcpClientSettings settings)
        {
            _factory = new ModbusFactory();
            TcpMaster = settings.TcpMaster;
            TcpSlave = settings.TcpSlave;
        }

        #endregion Constructors

        #region Public Methods

        /// <summary>
        /// Connects the client and initializes the Modbus ModbusMaster.
        /// </summary>
        /// <returns></returns>
        public bool Connect()
        {
            if (string.IsNullOrEmpty(TcpSlave.Address)) return false;
            if ((TcpSlave.Port < 0) || (TcpSlave.Port > 65535)) return false;
            if (TcpMaster.ReceiveTimeout < 0) return false;
            if (TcpMaster.SendTimeout < 0) return false;

            try
            {
                _client = new TcpClient();

                if (_client != null)
                {
                    _client.ExclusiveAddressUse = TcpMaster.ExclusiveAddressUse;
                    _client.ReceiveTimeout = TcpMaster.ReceiveTimeout;
                    _client.SendTimeout = TcpMaster.SendTimeout;
                    _client.Connect(TcpSlave.Address, TcpSlave.Port);

                    if (_client.Connected)
                    {
                        ModbusMaster = _factory.CreateMaster(_client);
                        return true;
                    }
                }
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception)
            {
                _client?.Dispose();
                _client = null;
            }
#pragma warning restore CA1031 // Do not catch general exception types

            ModbusMaster = NullMaster.CreateModbusMaster();
            return false;
        }

        /// <summary>
        /// Disconnects the client and disposes the ModbusMaster instance.
        /// </summary>
        public void Disconnect()
        {
            if (_client?.Connected ?? false)
            {
                _client?.Close();
                ModbusMaster.Dispose();
                ModbusMaster = NullMaster.CreateModbusMaster();
            }

            _client?.Dispose();
            _client = null;
        }

        #endregion Public Methods

        #region Modbus Functions

        #region Read Functions

        /// <summary>
        /// Reads from 1 to 2000 contiguous coils status.
        /// </summary>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <param name="numberOfPoints">Number of coils to read.</param>
        /// <returns>Coils status.</returns>
        public bool[] ReadCoils(ushort startAddress, ushort numberOfPoints)
            => ModbusMaster.ReadCoils(TcpSlave.ID, startAddress, numberOfPoints);

        /// <summary>
        /// Asynchronously reads from 1 to 2000 contiguous coils status.
        /// </summary>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <param name="numberOfPoints">Number of coils to read.</param>
        /// <returns>A task that represents the asynchronous read operation.</returns>
        public async Task<bool[]> ReadCoilsAsync(ushort startAddress, ushort numberOfPoints)
            => await ModbusMaster.ReadCoilsAsync(TcpSlave.ID, startAddress, numberOfPoints);

        /// <summary>
        /// Reads contiguous block of holding registers.
        /// </summary>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <param name="numberOfPoints">Number of holding registers to read.</param>
        /// <returns>Holding registers status.</returns>
        public ushort[] ReadHoldingRegisters(ushort startAddress, ushort numberOfPoints)
            => ModbusMaster.ReadHoldingRegisters(TcpSlave.ID, startAddress, numberOfPoints);

        /// <summary>
        /// Asynchronously reads contiguous block of holding registers.
        /// </summary>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <param name="numberOfPoints">Number of holding registers to read.</param>
        /// <returns>A task that represents the asynchronous read operation.</returns>
        public async Task<ushort[]> ReadHoldingRegistersAsync(ushort startAddress, ushort numberOfPoints)
            => await ModbusMaster.ReadHoldingRegistersAsync(TcpSlave.ID, startAddress, numberOfPoints);

        /// <summary>
        /// Reads contiguous block of input registers.
        /// </summary>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <param name="numberOfPoints">Number of holding registers to read.</param>
        /// <returns>Input registers status.</returns>
        public ushort[] ReadInputRegisters(ushort startAddress, ushort numberOfPoints)
            => ModbusMaster.ReadInputRegisters(TcpSlave.ID, startAddress, numberOfPoints);

        /// <summary>
        /// Asynchronously reads contiguous block of input registers.
        /// </summary>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <param name="numberOfPoints">Number of holding registers to read.</param>
        /// <returns>A task that represents the asynchronous read operation.</returns>
        public async Task<ushort[]> ReadInputRegistersAsync(ushort startAddress, ushort numberOfPoints)
            => await ModbusMaster.ReadInputRegistersAsync(TcpSlave.ID, startAddress, numberOfPoints);

        /// <summary>
        /// Reads from 1 to 2000 contiguous discrete input status.
        /// </summary>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <param name="numberOfPoints">Number of discrete inputs to read.</param>
        /// <returns>Discrete inputs status.</returns>
        public bool[] ReadInputs(ushort startAddress, ushort numberOfPoints)
            => ModbusMaster.ReadInputs(TcpSlave.ID, startAddress, numberOfPoints);

        /// <summary>
        /// Asynchronously reads from 1 to 2000 contiguous discrete input status.
        /// </summary>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <param name="numberOfPoints">Number of discrete inputs to read.</param>
        /// <returns>A task that represents the asynchronous read operation.</returns>
        public async Task<bool[]> ReadInputsAsync(ushort startAddress, ushort numberOfPoints)
            => await ModbusMaster.ReadInputsAsync(TcpSlave.ID, startAddress, numberOfPoints);

        #endregion Read Functions

        #region Write Functions

        /// <summary>
        /// Performs a combination of one read operation and one write operation in a single
        /// Modbus transaction. The write operation is performed before the read.
        /// </summary>
        /// <param name="startReadAddress">Address to begin reading (Holding registers are addressed starting at 0).</param>
        /// <param name="numberOfPointsToRead">Number of registers to read.</param>
        /// <param name="startWriteAddress">Address to begin writing (Holding registers are addressed starting at 0).</param>
        /// <param name="writeData">Register values to write.</param>
        /// <returns>Holding registers status.</returns>
        public ushort[] ReadWriteMultipleRegisters(ushort startReadAddress, ushort numberOfPointsToRead, ushort startWriteAddress, ushort[] writeData)
            => ModbusMaster.ReadWriteMultipleRegisters(TcpSlave.ID, startReadAddress, numberOfPointsToRead, startWriteAddress, writeData);

        /// <summary>
        /// Asynchronously performs a combination of one read operation and one write operation
        /// in a single Modbus transaction. The write operation is performed before the read.
        /// </summary>
        /// <param name="startReadAddress">Address to begin reading (Holding registers are addressed starting at 0).</param>
        /// <param name="numberOfPointsToRead">Number of registers to read.</param>
        /// <param name="startWriteAddress">Address to begin writing (Holding registers are addressed starting at 0).</param>
        /// <param name="writeData">Register values to write.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task<ushort[]> ReadWriteMultipleRegistersAsync(ushort startReadAddress, ushort numberOfPointsToRead, ushort startWriteAddress, ushort[] writeData)
            => await ModbusMaster.ReadWriteMultipleRegistersAsync(TcpSlave.ID, startReadAddress, numberOfPointsToRead, startWriteAddress, writeData);

        /// <summary>
        /// Writes a sequence of coils.
        /// </summary>
        /// <param name="startAddress">Address to begin writing values.</param>
        /// <param name="data">Values to write.</param>
        public void WriteMultipleCoils(ushort startAddress, bool[] data)
            => ModbusMaster.WriteMultipleCoils(TcpSlave.ID, startAddress, data);

        /// <summary>
        /// Asynchronously writes a sequence of coils.
        /// </summary>
        /// <param name="startAddress">Address to begin writing values.</param>
        /// <param name="data">Values to write.</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        public async Task WriteMultipleCoilsAsync(ushort startAddress, bool[] data)
            => await ModbusMaster.WriteMultipleCoilsAsync(TcpSlave.ID, startAddress, data);

        /// <summary>
        /// Writes a block of 1 to 123 contiguous registers.
        /// </summary>
        /// <param name="startAddress">Address to begin writing values.</param>
        /// <param name="data">Values to write.</param>
        public void WriteMultipleRegisters(ushort startAddress, ushort[] data)
            => ModbusMaster.WriteMultipleRegisters(TcpSlave.ID, startAddress, data);

        /// <summary>
        /// Asynchronously writes a block of 1 to 123 contiguous registers.
        /// </summary>
        /// <param name="startAddress">Address to begin writing values.</param>
        /// <param name="data">Values to write.</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        public async Task WriteMultipleRegistersAsync(ushort startAddress, ushort[] data)
            => await ModbusMaster.WriteMultipleRegistersAsync(TcpSlave.ID, startAddress, data);

        /// <summary>
        /// Writes a single coil value.
        /// </summary>
        /// <param name="coilAddress">Address to write value to.</param>
        /// <param name="value">Value to write.</param>
        public void WriteSingleCoil(ushort coilAddress, bool value)
            => ModbusMaster.WriteSingleCoil(TcpSlave.ID, coilAddress, value);

        /// <summary>
        /// Asynchronously writes a single coil value.
        /// </summary>
        /// <param name="coilAddress">Address to write value to.</param>
        /// <param name="value">Value to write.</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        public async Task WriteSingleCoilAsync(ushort coilAddress, bool value)
            => await ModbusMaster.WriteSingleCoilAsync(TcpSlave.ID, coilAddress, value);

        /// <summary>
        /// Writes a single holding register.
        /// </summary>
        /// <param name="registerAddress">Address to write.</param>
        /// <param name="value">Value to write.</param>
        public void WriteSingleRegister(ushort registerAddress, ushort value)
            => ModbusMaster.WriteSingleRegister(TcpSlave.ID, registerAddress, value);

        /// <summary>
        /// Asynchronously writes a single holding register.
        /// </summary>
        /// <param name="registerAddress">Address to write.</param>
        /// <param name="value">Value to write.</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        public async Task WriteSingleRegisterAsync(ushort registerAddress, ushort value)
            => await ModbusMaster.WriteSingleRegisterAsync(TcpSlave.ID, registerAddress, value);

        #endregion Write Functions

        #region Extended Read Functions

        /// <summary>
        /// <summary>
        /// Reads an ASCII string (multiple holding register).
        /// </summary>
        /// </summary>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <param name="numberOfCharacters">Number of characters to read.</param>
        /// <returns>ASCII string</returns>
        public string ReadString(ushort startAddress, ushort numberOfCharacters)
            => ModbusMaster.ReadString(TcpSlave.ID, startAddress, numberOfCharacters, SwapBytes);

        /// <summary>
        /// Reads a HEX string (multiple holding register).
        /// </summary>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <param name="numberOfHex">Number of bytes to read.</param>
        /// <returns>HEX string</returns>
        public string ReadHexString(ushort startAddress, ushort numberOfHex)
            => ModbusMaster.ReadHexString(TcpSlave.ID, startAddress, numberOfHex, SwapBytes);

        /// <summary>
        /// Reads a single boolean value.
        /// </summary>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <returns>bool value.</returns>
        public bool ReadBool(ushort startAddress)
            => ModbusMaster.ReadBool(TcpSlave.ID, startAddress);

        /// <summary>
        /// Reads a 16 bit array (single holding register)
        /// </summary>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <returns>16 bit BitArray.</returns>
        public BitArray ReadBits(ushort startAddress)
            => ModbusMaster.ReadBits(TcpSlave.ID, startAddress);

        /// <summary>
        /// Reads a 16 bit integer (single holding register).
        /// </summary>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <returns>16 bit integer.</returns>
        public short ReadShort(ushort startAddress)
            => ModbusMaster.ReadShort(TcpSlave.ID, startAddress, SwapBytes);

        /// <summary>
        /// Reads a single unsigned 16 bit integer (single holding register).
        /// </summary>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <returns>Unsigned 16 bit integer.</returns>
        public ushort ReadUShort(ushort startAddress)
            => ModbusMaster.ReadUShort(TcpSlave.ID, startAddress, SwapBytes);

        /// <summary>
        /// Reads an 32 bit integer (two holding registers).
        /// </summary>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <returns>32 bit integer.</returns>
        public int ReadInt32(ushort startAddress)
            => ModbusMaster.ReadInt32(TcpSlave.ID, startAddress, SwapBytes, SwapWords);

        /// <summary>
        /// Reads a single unsigned 32 bit integer (two holding registers).
        /// </summary>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <returns>Unsigned 32 bit integer.</returns>
        public uint ReadUInt32(ushort startAddress)
            => ModbusMaster.ReadUInt32(TcpSlave.ID, startAddress, SwapBytes, SwapWords);

        /// <summary>
        /// Reads a single float value (two holding registers).
        /// </summary>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <returns>Float value.</returns>
        public float ReadFloat(ushort startAddress)
            => ModbusMaster.ReadFloat(TcpSlave.ID, startAddress, SwapBytes, SwapWords);

        /// <summary>
        /// Reads a single double value (four holding registers).
        /// </summary>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <returns>Double value.</returns>
        public double ReadDouble(ushort startAddress)
            => ModbusMaster.ReadDouble(TcpSlave.ID, startAddress, SwapBytes, SwapWords);

        /// <summary>
        /// Reads a 64 bit integer (four holding registers).
        /// </summary>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <returns>64 bit integer.</returns>
        public long ReadLong(ushort startAddress)
            => ModbusMaster.ReadLong(TcpSlave.ID, startAddress, SwapBytes, SwapWords);

        /// <summary>
        /// Reads an unsigned 64 bit integer (four holding registers).
        /// </summary>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <returns>Unsigned 64 bit integer.</returns>
        public ulong ReadULong(ushort startAddress)
            => ModbusMaster.ReadULong(TcpSlave.ID, startAddress, SwapBytes, SwapWords);

        /// <summary>
        /// Reads an array of boolean values (multiple coils).
        /// </summary>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <param name="length">Size of array.</param>
        /// <returns>Array of Bool values.</returns>
        public bool[] ReadBoolArray(ushort startAddress, ushort length)
            => ModbusMaster.ReadBoolArray(TcpSlave.ID, startAddress, length);

        /// <summary>
        /// Reads 8 bit values (multiple holding register).
        /// </summary>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <param name="length">Size of array.</param>
        /// <returns>Array of bytes.</returns>
        public byte[] ReadBytes(ushort startAddress, ushort length)
            => ModbusMaster.ReadBytes(TcpSlave.ID, startAddress, length, SwapBytes);

        /// <summary>
        /// Reads an array of 16 bit integers (multiple holding registers).
        /// </summary>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <param name="length">Size of array.</param>
        /// <returns>Array of 16 bit integers.</returns>
        public short[] ReadShortArray(ushort startAddress, ushort length)
            => ModbusMaster.ReadShortArray(TcpSlave.ID, startAddress, length, SwapBytes);

        /// <summary>
        /// Reads an array of unsigned 16 bit integer (multiple holding registers).
        /// </summary>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <param name="length">Size of array.</param>
        /// <returns>Array of unsigned 16 bit integer.</returns>
        public ushort[] ReadUShortArray(ushort startAddress, ushort length)
            => ModbusMaster.ReadUShortArray(TcpSlave.ID, startAddress, length, SwapBytes);

        /// <summary>
        /// Reads an array of 32 bit integers (multiple holding registers).
        /// </summary>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <param name="length">Size of array.</param>
        /// <returns>Array of 32 bit integers.</returns>
        public int[] ReadInt32Array(ushort startAddress, ushort length)
            => ModbusMaster.ReadInt32Array(TcpSlave.ID, startAddress, length, SwapBytes, SwapWords);

        /// <summary>
        /// Reads an array of unsigned 32 bit integers (multiple holding registers).
        /// </summary>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <param name="length">Size of array.</param>
        /// <returns>Array of unsigned 32 bit integers.</returns>
        public uint[] ReadUInt32Array(ushort startAddress, ushort length)
            => ModbusMaster.ReadUInt32Array(TcpSlave.ID, startAddress, length, SwapBytes, SwapWords);

        /// <summary>
        /// Reads an array of 32 bit IEEE floating point numbers (multiple holding registers).
        /// </summary>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <param name="length">Size of array.</param>
        /// <returns>Array of 32 bit IEEE floating point numbers.</returns>
        public float[] ReadFloatArray(ushort startAddress, ushort length)
            => ModbusMaster.ReadFloatArray(TcpSlave.ID, startAddress, length, SwapBytes, SwapWords);

        /// <summary>
        /// Reads an array of 64 bit IEEE floating point numbers (multiple holding registers).
        /// </summary>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <param name="length">Size of array.</param>
        /// <returns>Array of 64 bit IEEE floating point numbers.</returns>
        public double[] ReadDoubleArray(ushort startAddress, ushort length)
            => ModbusMaster.ReadDoubleArray(TcpSlave.ID, startAddress, length, SwapBytes, SwapWords);

        /// <summary>
        /// Reads an array of 64 bit integers (multiple holding registers).
        /// </summary>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <param name="length">Size of array.</param>
        /// <returns>Array of 64 bit integers.</returns>
        public long[] ReadLongArray(ushort startAddress, ushort length)
            => ModbusMaster.ReadLongArray(TcpSlave.ID, startAddress, length, SwapBytes, SwapWords);

        /// <summary>
        /// Reads an array of unsigned 64 bit integers (multiple holding registers).
        /// </summary>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <param name="length">Size of array.</param>
        /// <returns>Array of unsigned 64 bit integers.</returns>
        public ulong[] ReadULongArray(ushort startAddress, ushort length)
            => ModbusMaster.ReadULongArray(TcpSlave.ID, startAddress, length, SwapBytes, SwapWords);

        #endregion Extended Read Functions

        #region Extended Write Functions

        /// <summary>
        /// Writes an ASCII string (multiple holding registers).
        /// </summary>
        /// <param name="startAddress">Address to begin writing.</param>
        /// <param name="text">ASCII string to be written.</param>
        /// <returns>The task representing the async void write string method.</returns>
        public void WriteString(ushort startAddress, string text)
             => ModbusMaster.WriteString(TcpSlave.ID, startAddress, text, SwapBytes);

        /// <summary>
        /// Writes a HEX string (multiple holding registers).
        /// </summary>
        /// <param name="startAddress">Address to begin writing.</param>
        /// <param name="hex">HEX string to be written.</param>
        /// <returns>The task representing the async void write HEX string method.</returns>
        public void WriteHexString(ushort startAddress, string hex)
             => ModbusMaster.WriteHexString(TcpSlave.ID, startAddress, hex, SwapBytes);

        /// <summary>
        /// Writes a single boolean value (single coil).
        /// </summary>
        /// <param name="startAddress">Address to begin writing.</param>
        /// <param name="value">uint value to be written.</param>
        /// <returns>The task representing the async void write bool method.</returns>
        public void WriteBool(ushort startAddress, bool value)
            => ModbusMaster.WriteBool(TcpSlave.ID, startAddress, value);

        /// <summary>
        /// Writes a 16 bit array (single holding register).
        /// </summary>
        /// <param name="startAddress">Address to begin writing.</param>
        /// <param name="value">BitArray value to be written.</param>
        /// <returns>The task representing the async void write bits method.</returns>
        public void WriteBits(ushort startAddress, BitArray value)
            => ModbusMaster.WriteBits(TcpSlave.ID, startAddress, value);

        /// <summary>
        /// Writes a 16 bit integer (single holding register).
        /// </summary>
        /// <param name="startAddress">Address to begin writing.</param>
        /// <param name="value">Short value to be written.</param>
        /// <returns>The task representing the async void write short method.</returns>
        public void WriteShort(ushort startAddress, short value)
            => ModbusMaster.WriteShort(TcpSlave.ID, startAddress, value, SwapBytes);

        /// <summary>
        /// Writes a single unsigned 16 bit integer value.
        /// </summary>
        /// <param name="startAddress">Address to begin writing.</param>
        /// <param name="value">uint value to be written.</param>
        /// <returns>The task representing the async void write unsigned short method.</returns>
        public void WriteUShort(ushort startAddress, ushort value)
            => ModbusMaster.WriteUShort(TcpSlave.ID, startAddress, value, SwapBytes);

        /// <summary>
        /// Writes a single 32 bit integer value (two holding registers).
        /// </summary>
        /// <param name="startAddress">Address to begin writing.</param>
        /// <param name="value">uint value to be written.</param>
        /// <returns>The task representing the async void write 32-bit integer method.</returns>
        public void WriteInt32(ushort startAddress, int value)
            => ModbusMaster.WriteInt32(TcpSlave.ID, startAddress, value, SwapBytes, SwapWords);

        /// <summary>
        /// Writes a single unsigned 32 bit integer value (two holding registers).
        /// </summary>
        /// <param name="startAddress">Address to begin writing.</param>
        /// <param name="value">uint value to be written.</param>
        /// <returns>The task representing the async void write unsigned 32-bit integer method.</returns>
        public void WriteUInt32(ushort startAddress, uint value)
            => ModbusMaster.WriteUInt32(TcpSlave.ID, startAddress, value, SwapBytes, SwapWords);

        /// <summary>
        /// Writes a single float value (two holding registers).
        /// </summary>
        /// <param name="startAddress">Address to begin writing.</param>
        /// <param name="value">float value to be written.</param>
        /// <returns>The task representing the async void write float method.</returns>
        public void WriteFloat(ushort startAddress, float value)
            => ModbusMaster.WriteFloat(TcpSlave.ID, startAddress, value, SwapBytes, SwapWords);

        /// <summary>
        /// Writes a single double value (four holding registers).
        /// </summary>
        /// <param name="startAddress">Address to begin writing.</param>
        /// <param name="value">double value to be written.</param>
        /// <returns>The task representing the async void write double method.</returns>
        public void WriteDouble(ushort startAddress, double value)
            => ModbusMaster.WriteDouble(TcpSlave.ID, startAddress, value, SwapBytes, SwapWords);

        /// <summary>
        /// Writes a 64 bit integer (four holding registers).
        /// </summary>
        /// <param name="startAddress">Address to begin writing.</param>
        /// <param name="value">Long value to be written.</param>
        /// <returns>The task representing the async void write long method.</returns>
        public void WriteLong(ushort startAddress, long value)
            => ModbusMaster.WriteLong(TcpSlave.ID, startAddress, value, SwapBytes, SwapWords);

        /// <summary>
        /// Writes an unsigned 64 bit integer (four holding registers).
        /// </summary>
        /// <param name="startAddress">Address to begin writing.</param>
        /// <param name="value">Short value to be written.</param>
        /// <returns>The task representing the async void write unsigned long method.</returns>
        public void WriteULong(ushort startAddress, ulong value)
            => ModbusMaster.WriteULong(TcpSlave.ID, startAddress, value, SwapBytes, SwapWords);

        /// <summary>
        /// Writes an array of boolean values (multiple coils)
        /// </summary>
        /// <param name="startAddress">Address to begin writing.</param>
        /// <param name="values">Array of boolean values to be written.</param>
        /// <returns>The task representing the async void write bool array method.</returns>
        public void WriteBoolArray(ushort startAddress, bool[] values)
            => ModbusMaster.WriteBoolArray(TcpSlave.ID, startAddress, values);

        /// <summary>
        /// Writes 8 bit values (multiple holding registers).
        /// </summary>
        /// <param name="startAddress">Address to begin writing.</param>
        /// <param name="values">Short value to be written.</param>
        /// <returns>The task representing the async void write byte array method.</returns>
        public void WriteBytes(ushort startAddress, byte[] values)
            => ModbusMaster.WriteBytes(TcpSlave.ID, startAddress, values);

        /// <summary>
        /// Writes an array of 16 bit integers (multiple holding registers).
        /// </summary>
        /// <param name="startAddress">Address to begin writing.</param>
        /// <param name="values">Array of short values to be written.</param>
        /// <returns>The task representing the async void write short array method.</returns>
        public void WriteShortArray(ushort startAddress, short[] values)
            => ModbusMaster.WriteShortArray(TcpSlave.ID, startAddress, values, SwapBytes);

        /// <summary>
        /// Writes an array of unsigned 16 bit integer (multiple holding registers).
        /// </summary>
        /// <param name="startAddress">Address to begin writing.</param>
        /// <param name="values">Array of unsigned short values to be written.</param>
        /// <returns>The task representing the async void write unsigned short array method.</returns>
        public void WriteUShortArray(ushort startAddress, ushort[] values)
            => ModbusMaster.WriteUShortArray(TcpSlave.ID, startAddress, values, SwapBytes);

        /// <summary>
        /// Writes an array of 32 bit integers (multiple holding registers).
        /// </summary>
        /// <param name="startAddress">Address to begin writing.</param>
        /// <param name="values">Array of Int32 values to be written.</param>
        /// <returns>The task representing the async void write 32-bit integer array method.</returns>
        public void WriteInt32Array(ushort startAddress, int[] values)
            => ModbusMaster.WriteInt32Array(TcpSlave.ID, startAddress, values, SwapBytes, SwapWords);

        /// <summary>
        /// Writes an array of unsigned 32 bit integers (multiple holding registers).
        /// </summary>
        /// <param name="startAddress">Address to begin writing.</param>
        /// <param name="values">Array of UInt32 values to be written.</param>
        /// <returns>The task representing the async void write unsigned 32-bit integer array method.</returns>
        public void WriteUInt32Array(ushort startAddress, uint[] values)
            => ModbusMaster.WriteUInt32Array(TcpSlave.ID, startAddress, values, SwapBytes, SwapWords);

        /// <summary>
        /// Writes an array of 32 bit IEEE floating point numbers (multiple holding registers).
        /// </summary>
        /// <param name="startAddress">Address to begin writing.</param>
        /// <param name="values">Short value to be written.</param>
        /// <returns>The task representing the async void write float array method.</returns>
        public void WriteFloatArray(ushort startAddress, float[] values)
            => ModbusMaster.WriteFloatArray(TcpSlave.ID, startAddress, values, SwapBytes, SwapWords);

        /// <summary>
        /// Writes an array of 64 bit IEEE floating point numbers (multiple holding registers).
        /// </summary>
        /// <param name="startAddress">Address to begin writing.</param>
        /// <param name="values">Short value to be written.</param>
        /// <returns>The task representing the async void write double array method.</returns>
        public void WriteDoubleArray(ushort startAddress, double[] values)
            => ModbusMaster.WriteDoubleArray(TcpSlave.ID, startAddress, values, SwapBytes, SwapWords);

        /// <summary>
        /// Writes an array of 64 bit integers (multiple holding registers).
        /// </summary>
        /// <param name="startAddress">Address to begin writing.</param>
        /// <param name="values">Array of long values to be written.</param>
        /// <returns>The task representing the async void write long array method.</returns>
        public void WriteLongArray(ushort startAddress, long[] values)
            => ModbusMaster.WriteLongArray(TcpSlave.ID, startAddress, values, SwapBytes, SwapWords);

        /// <summary>
        /// Writes an array of unsigned 64 bit integers (multiple holding registers).
        /// </summary>
        /// <param name="startAddress">Address to begin writing.</param>
        /// <param name="values">Array of unsigned long values to be written.</param>
        /// <returns>The task representing the async void write unsigned long array method.</returns>
        public void WriteULongArray(ushort startAddress, ulong[] values)
            => ModbusMaster.WriteULongArray(TcpSlave.ID, startAddress, values, SwapBytes, SwapWords);

        #endregion Extended Write Functions

        #region Extended Read Only Functions

        /// <summary>
        /// Reads an ASCII string (multiple input register).
        /// </summary>

        /// <param name="startAddress">Address to begin reading.</param>
        /// <param name="numberOfCharacters">Number of characters to read.</param>
        /// <returns>ASCII string</returns>
        public string ReadOnlyString(ushort startAddress, ushort numberOfCharacters)
            => ModbusMaster.ReadOnlyString(TcpSlave.ID, startAddress, numberOfCharacters, SwapBytes);

        /// <summary>
        /// Reads a HEX string (multiple input register).
        /// </summary>

        /// <param name="startAddress">Address to begin reading.</param>
        /// <param name="numberOfHex">Number of characters to read.</param>
        /// <returns>ASCII string</returns>
        public string ReadOnlyHexString(ushort startAddress, ushort numberOfHex)
            => ModbusMaster.ReadOnlyHexString(TcpSlave.ID, startAddress, numberOfHex, SwapBytes);

        /// <summary>
        /// Reads a single boolean value.
        /// </summary>

        /// <param name="startAddress">Address to begin reading.</param>
        /// <returns>bool value.</returns>
        public bool ReadOnlyBool(ushort startAddress)
            => ModbusMaster.ReadOnlyBool(TcpSlave.ID, startAddress);

        /// <summary>
        /// Reads a 16 bit array (single input register)
        /// </summary>

        /// <param name="startAddress">Address to begin reading.</param>
        /// <returns>16 bit BitArray.</returns>
        public BitArray ReadOnlyBits(ushort startAddress)
            => ModbusMaster.ReadOnlyBits(TcpSlave.ID, startAddress);

        /// <summary>
        /// Reads a 16 bit integer (single input register).
        /// </summary>

        /// <param name="startAddress">Address to begin reading.</param>
        /// <returns>16 bit integer.</returns>
        public short ReadOnlyShort(ushort startAddress)
            => ModbusMaster.ReadOnlyShort(TcpSlave.ID, startAddress, SwapBytes);

        /// <summary>
        /// Reads a single unsigned 16 bit integer (single input register).
        /// </summary>

        /// <param name="startAddress">Address to begin reading.</param>
        /// <returns>Unsigned 16 bit integer.</returns>
        public ushort ReadOnlyUShort(ushort startAddress)
            => ModbusMaster.ReadOnlyUShort(TcpSlave.ID, startAddress, SwapBytes);

        /// <summary>
        /// Reads an 32 bit integer (two input registers).
        /// </summary>

        /// <param name="startAddress">Address to begin reading.</param>
        /// <returns>32 bit integer.</returns>
        public int ReadOnlyInt32(ushort startAddress)
            => ModbusMaster.ReadOnlyInt32(TcpSlave.ID, startAddress, SwapBytes, SwapWords);

        /// <summary>
        /// Reads a single unsigned 32 bit integer (two input registers).
        /// </summary>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <returns>Unsigned 32 bit integer.</returns>
        public uint ReadOnlyUInt32(ushort startAddress)
            => ModbusMaster.ReadOnlyUInt32(TcpSlave.ID, startAddress, SwapBytes, SwapWords);

        /// <summary>
        /// Reads a single float value (two input registers).
        /// </summary>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <returns>Float value.</returns>
        public float ReadOnlyFloat(ushort startAddress)
            => ModbusMaster.ReadOnlyFloat(TcpSlave.ID, startAddress, SwapBytes, SwapWords);

        /// <summary>
        /// Reads a single double value (four input registers).
        /// </summary>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <returns>Double value.</returns>
        public double ReadOnlyDouble(ushort startAddress)
            => ModbusMaster.ReadOnlyDouble(TcpSlave.ID, startAddress, SwapBytes, SwapWords);

        /// <summary>
        /// Reads a 64 bit integer (four input registers).
        /// </summary>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <returns>64 bit integer.</returns>
        public long ReadOnlyLong(ushort startAddress)
            => ModbusMaster.ReadOnlyLong(TcpSlave.ID, startAddress, SwapBytes, SwapWords);

        /// <summary>
        /// Reads an unsigned 64 bit integer (four input registers).
        /// </summary>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <returns>Unsigned 64 bit integer.</returns>
        public ulong ReadOnlyULong(ushort startAddress)
            => ModbusMaster.ReadOnlyULong(TcpSlave.ID, startAddress, SwapBytes, SwapWords);

        /// <summary>
        /// Reads an array of boolean values (multiple discrete inputs).
        /// </summary>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <param name="length">Size of array.</param>
        /// <returns>Array of Bool values.</returns>
        public bool[] ReadOnlyBoolArray(ushort startAddress, ushort length)
            => ModbusMaster.ReadOnlyBoolArray(TcpSlave.ID, startAddress, length);

        /// <summary>
        /// Reads 8 bit values (multiple input register).
        /// </summary>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <param name="length">Size of array.</param>
        /// <returns>Arroy of bytes.</returns>
        public byte[] ReadOnlyBytes(ushort startAddress, ushort length)
            => ModbusMaster.ReadOnlyBytes(TcpSlave.ID, startAddress, length, SwapBytes);

        /// <summary>
        /// Reads an array of 16 bit integers (multiple input registers).
        /// </summary>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <param name="length">Size of array.</param>
        /// <returns>Array of 16 bit integers.</returns>
        public short[] ReadOnlyShortArray(ushort startAddress, ushort length)
            => ModbusMaster.ReadOnlyShortArray(TcpSlave.ID, startAddress, length, SwapBytes);

        /// <summary>
        /// Reads an array of unsigned 16 bit integer (multiple input registers).
        /// </summary>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <param name="length">Size of array.</param>
        /// <returns>Array of unsigned 16 bit integer.</returns>
        public ushort[] ReadOnlyUShortArray(ushort startAddress, ushort length)
            => ModbusMaster.ReadOnlyUShortArray(TcpSlave.ID, startAddress, length, SwapBytes);

        /// <summary>
        /// Reads an array of 32 bit integers (multiple input registers).
        /// </summary>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <param name="length">Size of array.</param>
        /// <returns>Array of 32 bit integers.</returns>
        public int[] ReadOnlyInt32Array(ushort startAddress, ushort length)
            => ModbusMaster.ReadOnlyInt32Array(TcpSlave.ID, startAddress, length, SwapBytes, SwapWords);

        /// <summary>
        /// Reads an array of unsigned 32 bit integers (multiple input registers).
        /// </summary>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <param name="length">Size of array.</param>
        /// <returns>Array of unsigned 32 bit integers.</returns>
        public uint[] ReadOnlyUInt32Array(ushort startAddress, ushort length)
            => ModbusMaster.ReadOnlyUInt32Array(TcpSlave.ID, startAddress, length, SwapBytes, SwapWords);

        /// <summary>
        /// Reads an array of 32 bit IEEE floating point numbers (multiple input registers).
        /// </summary>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <param name="length">Size of array.</param>
        /// <returns>Array of 32 bit IEEE floating point numbers.</returns>
        public float[] ReadOnlyFloatArray(ushort startAddress, ushort length)
            => ModbusMaster.ReadOnlyFloatArray(TcpSlave.ID, startAddress, length, SwapBytes, SwapWords);

        /// <summary>
        /// Reads an array of 64 bit IEEE floating point numbers (multiple input registers).
        /// </summary>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <param name="length">Size of array.</param>
        /// <returns>Array of 64 bit IEEE floating point numbers.</returns>
        public double[] ReadOnlyDoubleArray(ushort startAddress, ushort length)
            => ModbusMaster.ReadOnlyDoubleArray(TcpSlave.ID, startAddress, length, SwapBytes, SwapWords);

        /// <summary>
        /// Reads an array of 64 bit integers (multiple input registers).
        /// </summary>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <param name="length">Size of array.</param>
        /// <returns>Array of 64 bit integers.</returns>
        public long[] ReadOnlyLongArray(ushort startAddress, ushort length)
            => ModbusMaster.ReadOnlyLongArray(TcpSlave.ID, startAddress, length, SwapBytes, SwapWords);

        /// <summary>
        /// Reads an array of unsigned 64 bit integers (multiple input registers).
        /// </summary>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <param name="length">Size of array.</param>
        /// <returns>Array of unsigned 64 bit integers.</returns>
        public ulong[] ReadOnlyULongArray(ushort startAddress, ushort length)
            => ModbusMaster.ReadOnlyULongArray(TcpSlave.ID, startAddress, length, SwapBytes, SwapWords);

        #endregion Extended Read Only Functions

        #region Extended Async Read Functions

        /// <summary>
        /// <summary>
        /// Asynchronously reads an ASCII string (multiple holding register).
        /// </summary>
        /// </summary>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <param name="numberOfCharacters">Number of characters to read.</param>
        /// <returns>ASCII string</returns>
        public async Task<string> ReadStringAsync(ushort startAddress, ushort numberOfCharacters)
            => await ModbusMaster.ReadStringAsync(TcpSlave.ID, startAddress, numberOfCharacters, SwapBytes);

        /// <summary>
        /// Asynchronously reads a HEX string (multiple holding register).
        /// </summary>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <param name="numberOfHex">Number of bytes to read.</param>
        /// <returns>HEX string</returns>
        public async Task<string> ReadHexStringAsync(ushort startAddress, ushort numberOfHex)
            => await ModbusMaster.ReadHexStringAsync(TcpSlave.ID, startAddress, numberOfHex, SwapBytes);

        /// <summary>
        /// Asynchronously reads a single boolean value.
        /// </summary>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <returns>bool value.</returns>
        public async Task<bool> ReadBoolAsync(ushort startAddress)
            => await ModbusMaster.ReadBoolAsync(TcpSlave.ID, startAddress);

        /// <summary>
        /// Asynchronously reads a 16 bit array (single holding register)
        /// </summary>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <returns>16 bit BitArray.</returns>
        public async Task<BitArray> ReadBitsAsync(ushort startAddress)
            => await ModbusMaster.ReadBitsAsync(TcpSlave.ID, startAddress);

        /// <summary>
        /// Asynchronously reads a 16 bit integer (single holding register).
        /// </summary>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <returns>16 bit integer.</returns>
        public async Task<short> ReadShortAsync(ushort startAddress)
            => await ModbusMaster.ReadShortAsync(TcpSlave.ID, startAddress, SwapBytes);

        /// <summary>
        /// Asynchronously reads a single unsigned 16 bit integer (single holding register).
        /// </summary>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <returns>Unsigned 16 bit integer.</returns>
        public async Task<ushort> ReadUShortAsync(ushort startAddress)
            => await ModbusMaster.ReadUShortAsync(TcpSlave.ID, startAddress, SwapBytes);

        /// <summary>
        /// Asynchronously reads an 32 bit integer (two holding registers).
        /// </summary>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <returns>32 bit integer.</returns>
        public async Task<Int32> ReadInt32Async(ushort startAddress)
            => await ModbusMaster.ReadInt32Async(TcpSlave.ID, startAddress, SwapBytes, SwapWords);

        /// <summary>
        /// Asynchronously reads a single unsigned 32 bit integer (two holding registers).
        /// </summary>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <returns>Unsigned 32 bit integer.</returns>
        public async Task<uint> ReadUInt32Async(ushort startAddress)
            => await ModbusMaster.ReadUInt32Async(TcpSlave.ID, startAddress, SwapBytes, SwapWords);

        /// <summary>
        /// Asynchronously reads a single float value (two holding registers).
        /// </summary>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <returns>Float value.</returns>
        public async Task<float> ReadFloatAsync(ushort startAddress)
            => await ModbusMaster.ReadFloatAsync(TcpSlave.ID, startAddress, SwapBytes, SwapWords);

        /// <summary>
        /// Asynchronously reads a single double value (four holding registers).
        /// </summary>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <returns>Double value.</returns>
        public async Task<double> ReadDoubleAsync(ushort startAddress)
            => await ModbusMaster.ReadDoubleAsync(TcpSlave.ID, startAddress, SwapBytes, SwapWords);

        /// <summary>
        /// Asynchronously reads a 64 bit integer (four holding registers).
        /// </summary>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <returns>64 bit integer.</returns>
        public async Task<long> ReadLongAsync(ushort startAddress)
            => await ModbusMaster.ReadLongAsync(TcpSlave.ID, startAddress, SwapBytes, SwapWords);

        /// <summary>
        /// Asynchronously reads an unsigned 64 bit integer (four holding registers).
        /// </summary>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <returns>Unsigned 64 bit integer.</returns>
        public async Task<ulong> ReadULongAsync(ushort startAddress)
            => await ModbusMaster.ReadULongAsync(TcpSlave.ID, startAddress, SwapBytes, SwapWords);

        /// <summary>
        /// Asynchronously reads an array of boolean values (multiple coils).
        /// </summary>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <param name="length">Size of array.</param>
        /// <returns>Array of Bool values.</returns>
        public async Task<bool[]> ReadBoolArrayAsync(ushort startAddress, ushort length)
            => await ModbusMaster.ReadBoolArrayAsync(TcpSlave.ID, startAddress, length);

        /// <summary>
        /// Asynchronously reads 8 bit values (multiple holding register).
        /// </summary>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <param name="length">Size of array.</param>
        /// <returns>Array of bytes.</returns>
        public async Task<byte[]> ReadBytesAsync(ushort startAddress, ushort length)
            => await ModbusMaster.ReadBytesAsync(TcpSlave.ID, startAddress, length, SwapBytes);

        /// <summary>
        /// Asynchronously reads an array of 16 bit integers (multiple holding registers).
        /// </summary>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <param name="length">Size of array.</param>
        /// <returns>Array of 16 bit integers.</returns>
        public async Task<short[]> ReadShortArrayAsync(ushort startAddress, ushort length)
            => await ModbusMaster.ReadShortArrayAsync(TcpSlave.ID, startAddress, length, SwapBytes);

        /// <summary>
        /// Asynchronously reads an array of unsigned 16 bit integer (multiple holding registers).
        /// </summary>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <param name="length">Size of array.</param>
        /// <returns>Array of unsigned 16 bit integer.</returns>
        public async Task<ushort[]> ReadUShortArrayAsync(ushort startAddress, ushort length)
            => await ModbusMaster.ReadUShortArrayAsync(TcpSlave.ID, startAddress, length, SwapBytes);

        /// <summary>
        /// Asynchronously reads an array of 32 bit integers (multiple holding registers).
        /// </summary>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <param name="length">Size of array.</param>
        /// <returns>Array of 32 bit integers.</returns>
        public async Task<Int32[]> ReadInt32ArrayAsync(ushort startAddress, ushort length)
            => await ModbusMaster.ReadInt32ArrayAsync(TcpSlave.ID, startAddress, length, SwapBytes, SwapWords);

        /// <summary>
        /// Asynchronously reads an array of unsigned 32 bit integers (multiple holding registers).
        /// </summary>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <param name="length">Size of array.</param>
        /// <returns>Array of unsigned 32 bit integers.</returns>
        public async Task<UInt32[]> ReadUInt32ArrayAsync(ushort startAddress, ushort length)
            => await ModbusMaster.ReadUInt32ArrayAsync(TcpSlave.ID, startAddress, length, SwapBytes, SwapWords);

        /// <summary>
        /// Asynchronously reads an array of 32 bit IEEE floating point numbers (multiple holding registers).
        /// </summary>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <param name="length">Size of array.</param>
        /// <returns>Array of 32 bit IEEE floating point numbers.</returns>
        public async Task<float[]> ReadFloatArrayAsync(ushort startAddress, ushort length)
            => await ModbusMaster.ReadFloatArrayAsync(TcpSlave.ID, startAddress, length, SwapBytes, SwapWords);

        /// <summary>
        /// Asynchronously reads an array of 64 bit IEEE floating point numbers (multiple holding registers).
        /// </summary>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <param name="length">Size of array.</param>
        /// <returns>Array of 64 bit IEEE floating point numbers.</returns>
        public async Task<double[]> ReadDoubleArrayAsync(ushort startAddress, ushort length)
            => await ModbusMaster.ReadDoubleArrayAsync(TcpSlave.ID, startAddress, length, SwapBytes, SwapWords);

        /// <summary>
        /// Asynchronously reads an array of 64 bit integers (multiple holding registers).
        /// </summary>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <param name="length">Size of array.</param>
        /// <returns>Array of 64 bit integers.</returns>
        public async Task<long[]> ReadLongArrayAsync(ushort startAddress, ushort length)
            => await ModbusMaster.ReadLongArrayAsync(TcpSlave.ID, startAddress, length, SwapBytes, SwapWords);

        /// <summary>
        /// Asynchronously reads an array of unsigned 64 bit integers (multiple holding registers).
        /// </summary>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <param name="length">Size of array.</param>
        /// <returns>Array of unsigned 64 bit integers.</returns>
        public async Task<ulong[]> ReadULongArrayAsync(ushort startAddress, ushort length)
            => await ModbusMaster.ReadULongArrayAsync(TcpSlave.ID, startAddress, length, SwapBytes, SwapWords);

        #endregion Extended Async Read Functions

        #region Extended Async Write Functions

        /// <summary>
        /// Asynchronously writes an ASCII string (multiple holding registers).
        /// </summary>
        /// <param name="startAddress">Address to begin writing.</param>
        /// <param name="text">ASCII string to be written.</param>
        /// <returns>The task representing the async void write string method.</returns>
        public async Task WriteStringAsync(ushort startAddress, string text)
            => await ModbusMaster.WriteStringAsync(TcpSlave.ID, startAddress, text, SwapBytes);

        /// <summary>
        /// Asynchronously writes a HEX string (multiple holding registers).
        /// </summary>
        /// <param name="startAddress">Address to begin writing.</param>
        /// <param name="hex">HEX string to be written.</param>
        /// <returns>The task representing the async void write HEX string method.</returns>
        public async Task WriteHexStringAsync(ushort startAddress, string hex)
            => await ModbusMaster.WriteHexStringAsync(TcpSlave.ID, startAddress, hex, SwapBytes);

        /// <summary>
        /// Asynchronously writes a single boolean value (single coil).
        /// </summary>
        /// <param name="startAddress">Address to begin writing.</param>
        /// <param name="value">uint value to be written.</param>
        /// <returns>The task representing the async void write bool method.</returns>
        public async Task WriteBoolAsync(ushort startAddress, bool value)
            => await ModbusMaster.WriteBoolAsync(TcpSlave.ID, startAddress, value);

        /// <summary>
        /// Writes a 16 bit array (single holding register).
        /// </summary>
        /// <param name="startAddress">Address to begin writing.</param>
        /// <param name="value">BitArray value to be written.</param>
        /// <returns>The task representing the async void write bits method.</returns>
        public async Task WriteBitsAsync(ushort startAddress, BitArray value)
            => await ModbusMaster.WriteBitsAsync(TcpSlave.ID, startAddress, value);

        /// <summary>
        /// Asynchronously writes a 16 bit integer (single holding register).
        /// </summary>
        /// <param name="startAddress">Address to begin writing.</param>
        /// <param name="value">Short value to be written.</param>
        /// <returns>The task representing the async void write short method.</returns>
        public async Task WriteShortAsync(ushort startAddress, short value)
            => await ModbusMaster.WriteShortAsync(TcpSlave.ID, startAddress, value, SwapBytes);

        /// <summary>
        /// Asynchronously writes a single unsigned 16 bit integer value.
        /// </summary>
        /// <param name="startAddress">Address to begin writing.</param>
        /// <param name="value">uint value to be written.</param>
        /// <returns>The task representing the async void write unsigned short method.</returns>
        public async Task WriteUShortAsync(ushort startAddress, ushort value)
            => await ModbusMaster.WriteUShortAsync(TcpSlave.ID, startAddress, value, SwapBytes);

        /// <summary>
        /// Asynchronously writes a single 32 bit integer value (two holding registers).
        /// </summary>
        /// <param name="startAddress">Address to begin writing.</param>
        /// <param name="value">uint value to be written.</param>
        /// <returns>The task representing the async void write 32-bit integer method.</returns>
        public async Task WriteInt32Async(ushort startAddress, int value)
            => await ModbusMaster.WriteInt32Async(TcpSlave.ID, startAddress, value, SwapBytes, SwapWords);

        /// <summary>
        /// Asynchronously writes a single unsigned 32 bit integer value (two holding registers).
        /// </summary>
        /// <param name="startAddress">Address to begin writing.</param>
        /// <param name="value">uint value to be written.</param>
        /// <returns>The task representing the async void write unsigned 32-bit integer method.</returns>
        public async Task WriteUInt32Async(ushort startAddress, uint value)
            => await ModbusMaster.WriteUInt32Async(TcpSlave.ID, startAddress, value, SwapBytes, SwapWords);

        /// <summary>
        /// Asynchronously writes a single float value (two holding registers).
        /// </summary>
        /// <param name="startAddress">Address to begin writing.</param>
        /// <param name="value">float value to be written.</param>
        /// <returns>The task representing the async void write float method.</returns>
        public async Task WriteFloatAsync(ushort startAddress, float value)
            => await ModbusMaster.WriteFloatAsync(TcpSlave.ID, startAddress, value, SwapBytes, SwapWords);

        /// <summary>
        /// Asynchronously writes a single double value (four holding registers).
        /// </summary>
        /// <param name="startAddress">Address to begin writing.</param>
        /// <param name="value">double value to be written.</param>
        /// <returns>The task representing the async void write double method.</returns>
        public async Task WriteDoubleAsync(ushort startAddress, double value)
            => await ModbusMaster.WriteDoubleAsync(TcpSlave.ID, startAddress, value, SwapBytes, SwapWords);

        /// <summary>
        /// Asynchronously writes a 64 bit integer (four holding registers).
        /// </summary>
        /// <param name="startAddress">Address to begin writing.</param>
        /// <param name="value">Long value to be written.</param>
        /// <returns>The task representing the async void write long method.</returns>
        public async Task WriteLongAsync(ushort startAddress, long value)
            => await ModbusMaster.WriteLongAsync(TcpSlave.ID, startAddress, value, SwapBytes, SwapWords);

        /// <summary>
        /// Asynchronously writes an unsigned 64 bit integer (four holding registers).
        /// </summary>
        /// <param name="startAddress">Address to begin writing.</param>
        /// <param name="value">Short value to be written.</param>
        /// <returns>The task representing the async void write unsigned long method.</returns>
        public async Task WriteULongAsync(ushort startAddress, ulong value)
            => await ModbusMaster.WriteULongAsync(TcpSlave.ID, startAddress, value, SwapBytes, SwapWords);

        /// <summary>
        /// Asynchronously writes an array of boolean values (multiple coils)
        /// </summary>
        /// <param name="startAddress">Address to begin writing.</param>
        /// <param name="values">Array of boolean values to be written.</param>
        /// <returns>The task representing the async void write bool array method.</returns>
        public async Task WriteBoolArrayAsync(ushort startAddress, bool[] values)
            => await ModbusMaster.WriteBoolArrayAsync(TcpSlave.ID, startAddress, values);

        /// <summary>
        /// Asynchronously writes 8 bit values (multiple holding registers).
        /// </summary>
        /// <param name="startAddress">Address to begin writing.</param>
        /// <param name="values">Short value to be written.</param>
        /// <returns>The task representing the async void write byte array method.</returns>
        public async Task WriteBytesAsync(ushort startAddress, byte[] values)
            => await ModbusMaster.WriteBytesAsync(TcpSlave.ID, startAddress, values);

        /// <summary>
        /// Asynchronously writes an array of 16 bit integers (multiple holding registers).
        /// </summary>
        /// <param name="startAddress">Address to begin writing.</param>
        /// <param name="values">Array of short values to be written.</param>
        /// <returns>The task representing the async void write short array method.</returns>
        public async Task WriteShortArrayAsync(ushort startAddress, short[] values)
            => await ModbusMaster.WriteShortArrayAsync(TcpSlave.ID, startAddress, values, SwapBytes);

        /// <summary>
        /// Asynchronously writes an array of unsigned 16 bit integer (multiple holding registers).
        /// </summary>
        /// <param name="startAddress">Address to begin writing.</param>
        /// <param name="values">Array of unsigned short values to be written.</param>
        /// <returns>The task representing the async void write unsigned short array method.</returns>
        public async Task WriteUShortArrayAsync(ushort startAddress, ushort[] values)
            => await ModbusMaster.WriteUShortArrayAsync(TcpSlave.ID, startAddress, values, SwapBytes);

        /// <summary>
        /// Asynchronously writes an array of 32 bit integers (multiple holding registers).
        /// </summary>
        /// <param name="startAddress">Address to begin writing.</param>
        /// <param name="values">Array of Int32 values to be written.</param>
        /// <returns>The task representing the async void write 32-bit integer array method.</returns>
        public async Task WriteInt32ArrayAsync(ushort startAddress, Int32[] values)
            => await ModbusMaster.WriteInt32ArrayAsync(TcpSlave.ID, startAddress, values, SwapBytes, SwapWords);

        /// <summary>
        /// Asynchronously writes an array of unsigned 32 bit integers (multiple holding registers).
        /// </summary>
        /// <param name="startAddress">Address to begin writing.</param>
        /// <param name="values">Array of UInt32 values to be written.</param>
        /// <returns>The task representing the async void write unsigned 32-bit integer array method.</returns>
        public async Task WriteUInt32ArrayAsync(ushort startAddress, UInt32[] values)
            => await ModbusMaster.WriteUInt32ArrayAsync(TcpSlave.ID, startAddress, values, SwapBytes, SwapWords);

        /// <summary>
        /// Asynchronously writes an array of 32 bit IEEE floating point numbers (multiple holding registers).
        /// </summary>
        /// <param name="startAddress">Address to begin writing.</param>
        /// <param name="values">Short value to be written.</param>
        /// <returns>The task representing the async void write float array method.</returns>
        public async Task WriteFloatArrayAsync(ushort startAddress, float[] values)
            => await ModbusMaster.WriteFloatArrayAsync(TcpSlave.ID, startAddress, values, SwapBytes, SwapWords);

        /// <summary>
        /// Asynchronously writes an array of 64 bit IEEE floating point numbers (multiple holding registers).
        /// </summary>
        /// <param name="startAddress">Address to begin writing.</param>
        /// <param name="values">Short value to be written.</param>
        /// <returns>The task representing the async void write double array method.</returns>
        public async Task WriteDoubleArrayAsync(ushort startAddress, double[] values)
            => await ModbusMaster.WriteDoubleArrayAsync(TcpSlave.ID, startAddress, values, SwapBytes, SwapWords);

        /// <summary>
        /// Asynchronously writes an array of 64 bit integers (multiple holding registers).
        /// </summary>
        /// <param name="startAddress">Address to begin writing.</param>
        /// <param name="values">Array of long values to be written.</param>
        /// <returns>The task representing the async void write long array method.</returns>
        public async Task WriteLongArrayAsync(ushort startAddress, long[] values)
            => await ModbusMaster.WriteLongArrayAsync(TcpSlave.ID, startAddress, values, SwapBytes, SwapWords);

        /// <summary>
        /// Asynchronously writes an array of unsigned 64 bit integers (multiple holding registers).
        /// </summary>
        /// <param name="startAddress">Address to begin writing.</param>
        /// <param name="values">Array of unsigned long values to be written.</param>
        /// <returns>The task representing the async void write unsigned long array method.</returns>
        public async Task WriteULongArrayAsync(ushort startAddress, ulong[] values)
            => await ModbusMaster.WriteULongArrayAsync(TcpSlave.ID, startAddress, values, SwapBytes, SwapWords);

        #endregion Extended Async Write Functions

        #region Extended Async Read Only Functions

        /// <summary>
        /// Asynchronously reads an ASCII string (multiple input register).
        /// </summary>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <param name="numberOfCharacters">Number of characters to read.</param>
        /// <returns>ASCII string</returns>
        public async Task<string> ReadOnlyStringAsync(ushort startAddress, ushort numberOfCharacters)
            => await ModbusMaster.ReadOnlyStringAsync(TcpSlave.ID, startAddress, numberOfCharacters, SwapBytes);

        /// <summary>
        /// Asynchronously reads a HEX string (multiple input register).
        /// </summary>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <param name="numberOfHex">Number of characters to read.</param>
        /// <returns>ASCII string</returns>
        public async Task<string> ReadOnlyHexStringAsync(ushort startAddress, ushort numberOfHex)
            => await ModbusMaster.ReadOnlyHexStringAsync(TcpSlave.ID, startAddress, numberOfHex, SwapBytes);

        /// <summary>
        /// Asynchronously reads a single boolean value.
        /// </summary>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <returns>bool value.</returns>
        public async Task<bool> ReadOnlyBoolAsync(ushort startAddress)
            => await ModbusMaster.ReadOnlyBoolAsync(TcpSlave.ID, startAddress);

        /// <summary>
        /// Asynchronously reads a 16 bit array (single input register)
        /// </summary>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <returns>16 bit BitArray.</returns>
        public async Task<BitArray> ReadOnlyBitsAsync(ushort startAddress)
            => await ModbusMaster.ReadOnlyBitsAsync(TcpSlave.ID, startAddress, SwapBytes);

        /// <summary>
        /// Asynchronously reads a 16 bit integer (single input register).
        /// </summary>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <returns>16 bit integer.</returns>
        public async Task<short> ReadOnlyShortAsync(ushort startAddress)
            => await ModbusMaster.ReadOnlyShortAsync(TcpSlave.ID, startAddress, SwapBytes);

        /// <summary>
        /// Asynchronously reads a single unsigned 16 bit integer (single input register).
        /// </summary>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <returns>Unsigned 16 bit integer.</returns>
        public async Task<ushort> ReadOnlyUShortAsync(ushort startAddress)
            => await ModbusMaster.ReadOnlyUShortAsync(TcpSlave.ID, startAddress, SwapBytes);

        /// <summary>
        /// Asynchronously reads an 32 bit integer (two input registers).
        /// </summary>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <returns>32 bit integer.</returns>
        public async Task<Int32> ReadOnlyInt32Async(ushort startAddress)
            => await ModbusMaster.ReadOnlyInt32Async(TcpSlave.ID, startAddress, SwapBytes, SwapWords);

        /// <summary>
        /// Asynchronously reads a single unsigned 32 bit integer (two input registers).
        /// </summary>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <returns>Unsigned 32 bit integer.</returns>
        public async Task<uint> ReadOnlyUInt32Async(ushort startAddress)
            => await ModbusMaster.ReadOnlyUInt32Async(TcpSlave.ID, startAddress, SwapBytes, SwapWords);

        /// <summary>
        /// Asynchronously reads a single float value (two input registers).
        /// </summary>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <returns>Float value.</returns>
        public async Task<float> ReadOnlyFloatAsync(ushort startAddress)
            => await ModbusMaster.ReadOnlyFloatAsync(TcpSlave.ID, startAddress, SwapBytes, SwapWords);

        /// <summary>
        /// Asynchronously reads a single double value (four input registers).
        /// </summary>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <returns>Double value.</returns>
        public async Task<double> ReadOnlyDoubleAsync(ushort startAddress)
            => await ModbusMaster.ReadOnlyDoubleAsync(TcpSlave.ID, startAddress, SwapBytes, SwapWords);

        /// <summary>
        /// Asynchronously reads a 64 bit integer (four input registers).
        /// </summary>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <returns>64 bit integer.</returns>
        public async Task<long> ReadOnlyLongAsync(ushort startAddress)
            => await ModbusMaster.ReadOnlyLongAsync(TcpSlave.ID, startAddress, SwapBytes, SwapWords);

        /// <summary>
        /// Asynchronously reads an unsigned 64 bit integer (four input registers).
        /// </summary>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <returns>Unsigned 64 bit integer.</returns>
        public async Task<ulong> ReadOnlyULongAsync(ushort startAddress)
            => await ModbusMaster.ReadOnlyULongAsync(TcpSlave.ID, startAddress, SwapBytes, SwapWords);

        /// <summary>
        /// Asynchronously reads an array of boolean values (multiple discrete inputs).
        /// </summary>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <param name="length">Size of array.</param>
        /// <returns>Array of Bool values.</returns>
        public async Task<bool[]> ReadOnlyBoolArrayAsync(ushort startAddress, ushort length)
            => await ModbusMaster.ReadOnlyBoolArrayAsync(TcpSlave.ID, startAddress, length);

        /// <summary>
        /// Asynchronously reads 8 bit values (multiple input register).
        /// </summary>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <param name="length">Size of array.</param>
        /// <returns>Arroy of bytes.</returns>
        public async Task<byte[]> ReadOnlyBytesAsync(ushort startAddress, ushort length)
            => await ModbusMaster.ReadOnlyBytesAsync(TcpSlave.ID, startAddress, length, SwapBytes);

        /// <summary>
        /// Asynchronously reads an array of 16 bit integers (multiple input registers).
        /// </summary>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <param name="length">Size of array.</param>
        /// <returns>Array of 16 bit integers.</returns>
        public async Task<short[]> ReadOnlyShortArrayAsync(ushort startAddress, ushort length)
            => await ModbusMaster.ReadOnlyShortArrayAsync(TcpSlave.ID, startAddress, length, SwapBytes);

        /// <summary>
        /// Asynchronously reads an array of unsigned 16 bit integer (multiple input registers).
        /// </summary>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <param name="length">Size of array.</param>
        /// <returns>Array of unsigned 16 bit integer.</returns>
        public async Task<ushort[]> ReadOnlyUShortArrayAsync(ushort startAddress, ushort length)
            => await ModbusMaster.ReadOnlyUShortArrayAsync(TcpSlave.ID, startAddress, length, SwapBytes);

        /// <summary>
        /// Asynchronously reads an array of 32 bit integers (multiple input registers).
        /// </summary>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <param name="length">Size of array.</param>
        /// <returns>Array of 32 bit integers.</returns>
        public async Task<Int32[]> ReadOnlyInt32ArrayAsync(ushort startAddress, ushort length)
            => await ModbusMaster.ReadOnlyInt32ArrayAsync(TcpSlave.ID, startAddress, length, SwapBytes, SwapWords);

        /// <summary>
        /// Asynchronously reads an array of unsigned 32 bit integers (multiple input registers).
        /// </summary>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <param name="length">Size of array.</param>
        /// <returns>Array of unsigned 32 bit integers.</returns>
        public async Task<UInt32[]> ReadOnlyUInt32ArrayAsync(ushort startAddress, ushort length)
            => await ModbusMaster.ReadOnlyUInt32ArrayAsync(TcpSlave.ID, startAddress, length, SwapBytes, SwapWords);

        /// <summary>
        /// Asynchronously reads an array of 32 bit IEEE floating point numbers (multiple input registers).
        /// </summary>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <param name="length">Size of array.</param>
        /// <returns>Array of 32 bit IEEE floating point numbers.</returns>
        public async Task<float[]> ReadOnlyFloatArrayAsync(ushort startAddress, ushort length)
            => await ModbusMaster.ReadOnlyFloatArrayAsync(TcpSlave.ID, startAddress, length, SwapBytes, SwapWords);

        /// <summary>
        /// Asynchronously reads an array of 64 bit IEEE floating point numbers (multiple input registers).
        /// </summary>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <param name="length">Size of array.</param>
        /// <returns>Array of 64 bit IEEE floating point numbers.</returns>
        public async Task<double[]> ReadOnlyDoubleArrayAsync(ushort startAddress, ushort length)
            => await ModbusMaster.ReadOnlyDoubleArrayAsync(TcpSlave.ID, startAddress, length, SwapBytes, SwapWords);

        /// <summary>
        /// Asynchronously reads an array of 64 bit integers (multiple input registers).
        /// </summary>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <param name="length">Size of array.</param>
        /// <returns>Array of 64 bit integers.</returns>
        public async Task<long[]> ReadOnlyLongArrayAsync(ushort startAddress, ushort length)
            => await ModbusMaster.ReadOnlyLongArrayAsync(TcpSlave.ID, startAddress, length, SwapBytes, SwapWords);

        /// <summary>
        /// Asynchronously reads an array of unsigned 64 bit integers (multiple input registers).
        /// </summary>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <param name="length">Size of array.</param>
        /// <returns>Array of unsigned 64 bit integers.</returns>
        public async Task<ulong[]> ReadOnlyULongArrayAsync(ushort startAddress, ushort length)
            => await ModbusMaster.ReadOnlyULongArrayAsync(TcpSlave.ID, startAddress, length, SwapBytes, SwapWords);

        #endregion Extended Async Read Only Functions

        #endregion Modbus Functions

        #region Modbus Functions (slave id)

        #region Read Functions

        /// <summary>
        /// Reads from 1 to 2000 contiguous coils status.
        /// </summary>
        /// <param name="slaveAddress">Address of device to read values from.</param>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <param name="numberOfPoints">Number of coils to read.</param>
        /// <returns>Coils status.</returns>
        public bool[] ReadCoils(byte slaveAddress, ushort startAddress, ushort numberOfPoints)
            => ModbusMaster.ReadCoils(slaveAddress, startAddress, numberOfPoints);

        /// <summary>
        /// Asynchronously reads from 1 to 2000 contiguous coils status.
        /// </summary>
        /// <param name="slaveAddress">Address of device to read values from.</param>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <param name="numberOfPoints">Number of coils to read.</param>
        /// <returns>A task that represents the asynchronous read operation.</returns>
        public async Task<bool[]> ReadCoilsAsync(byte slaveAddress, ushort startAddress, ushort numberOfPoints)
            => await ModbusMaster.ReadCoilsAsync(slaveAddress, startAddress, numberOfPoints);

        /// <summary>
        /// Reads contiguous block of holding registers.
        /// </summary>
        /// <param name="slaveAddress">Address of device to read values from.</param>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <param name="numberOfPoints">Number of holding registers to read.</param>
        /// <returns>Holding registers status.</returns>
        public ushort[] ReadHoldingRegisters(byte slaveAddress, ushort startAddress, ushort numberOfPoints)
            => ModbusMaster.ReadHoldingRegisters(slaveAddress, startAddress, numberOfPoints);

        /// <summary>
        /// Asynchronously reads contiguous block of holding registers.
        /// </summary>
        /// <param name="slaveAddress">Address of device to read values from.</param>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <param name="numberOfPoints">Number of holding registers to read.</param>
        /// <returns>A task that represents the asynchronous read operation.</returns>
        public async Task<ushort[]> ReadHoldingRegistersAsync(byte slaveAddress, ushort startAddress, ushort numberOfPoints)
            => await ModbusMaster.ReadHoldingRegistersAsync(slaveAddress, startAddress, numberOfPoints);

        /// <summary>
        /// Reads contiguous block of input registers.
        /// </summary>
        /// <param name="slaveAddress">Address of device to read values from.</param>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <param name="numberOfPoints">Number of holding registers to read.</param>
        /// <returns>Input registers status.</returns>
        public ushort[] ReadInputRegisters(byte slaveAddress, ushort startAddress, ushort numberOfPoints)
            => ModbusMaster.ReadInputRegisters(slaveAddress, startAddress, numberOfPoints);

        /// <summary>
        /// Asynchronously reads contiguous block of input registers.
        /// </summary>
        /// <param name="slaveAddress">Address of device to read values from.</param>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <param name="numberOfPoints">Number of holding registers to read.</param>
        /// <returns>A task that represents the asynchronous read operation.</returns>
        public async Task<ushort[]> ReadInputRegistersAsync(byte slaveAddress, ushort startAddress, ushort numberOfPoints)
            => await ModbusMaster.ReadInputRegistersAsync(slaveAddress, startAddress, numberOfPoints);

        /// <summary>
        /// Reads from 1 to 2000 contiguous discrete input status.
        /// </summary>
        /// <param name="slaveAddress">Address of device to read values from.</param>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <param name="numberOfPoints">Number of discrete inputs to read.</param>
        /// <returns>Discrete inputs status.</returns>
        public bool[] ReadInputs(byte slaveAddress, ushort startAddress, ushort numberOfPoints)
            => ModbusMaster.ReadInputs(slaveAddress, startAddress, numberOfPoints);

        /// <summary>
        /// Asynchronously reads from 1 to 2000 contiguous discrete input status.
        /// </summary>
        /// <param name="slaveAddress">Address of device to read values from.</param>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <param name="numberOfPoints">Number of discrete inputs to read.</param>
        /// <returns>A task that represents the asynchronous read operation.</returns>
        public async Task<bool[]> ReadInputsAsync(byte slaveAddress, ushort startAddress, ushort numberOfPoints)
            => await ModbusMaster.ReadInputsAsync(slaveAddress, startAddress, numberOfPoints);

        #endregion Read Functions

        #region Write Functions

        /// <summary>
        /// Performs a combination of one read operation and one write operation in a single
        /// Modbus transaction. The write operation is performed before the read.
        /// </summary>
        /// <param name="slaveAddress">Address of device to read values from.</param>
        /// <param name="startReadAddress">Address to begin reading (Holding registers are addressed starting at 0).</param>
        /// <param name="numberOfPointsToRead">Number of registers to read.</param>
        /// <param name="startWriteAddress">Address to begin writing (Holding registers are addressed starting at 0).</param>
        /// <param name="writeData">Register values to write.</param>
        /// <returns>Holding registers status.</returns>
        public ushort[] ReadWriteMultipleRegisters(byte slaveAddress, ushort startReadAddress, ushort numberOfPointsToRead, ushort startWriteAddress, ushort[] writeData)
            => ModbusMaster.ReadWriteMultipleRegisters(slaveAddress, startReadAddress, numberOfPointsToRead, startWriteAddress, writeData);

        /// <summary>
        /// Asynchronously performs a combination of one read operation and one write operation
        /// in a single Modbus transaction. The write operation is performed before the read.
        /// </summary>
        /// <param name="slaveAddress">Address of device to read values from.</param>
        /// <param name="startReadAddress">Address to begin reading (Holding registers are addressed starting at 0).</param>
        /// <param name="numberOfPointsToRead">Number of registers to read.</param>
        /// <param name="startWriteAddress">Address to begin writing (Holding registers are addressed starting at 0).</param>
        /// <param name="writeData">Register values to write.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task<ushort[]> ReadWriteMultipleRegistersAsync(byte slaveAddress, ushort startReadAddress, ushort numberOfPointsToRead, ushort startWriteAddress, ushort[] writeData)
            => await ModbusMaster.ReadWriteMultipleRegistersAsync(slaveAddress, startReadAddress, numberOfPointsToRead, startWriteAddress, writeData);

        /// <summary>
        /// Writes a sequence of coils.
        /// </summary>
        /// <param name="slaveAddress">Address of device to read values from.</param>
        /// <param name="startAddress">Address to begin writing values.</param>
        /// <param name="data">Values to write.</param>
        public void WriteMultipleCoils(byte slaveAddress, ushort startAddress, bool[] data)
            => ModbusMaster.WriteMultipleCoils(slaveAddress, startAddress, data);

        /// <summary>
        /// Asynchronously writes a sequence of coils.
        /// </summary>
        /// <param name="slaveAddress">Address of device to read values from.</param>
        /// <param name="startAddress">Address to begin writing values.</param>
        /// <param name="data">Values to write.</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        public async Task WriteMultipleCoilsAsync(byte slaveAddress, ushort startAddress, bool[] data)
            => await ModbusMaster.WriteMultipleCoilsAsync(slaveAddress, startAddress, data);

        /// <summary>
        /// Writes a block of 1 to 123 contiguous registers.
        /// </summary>
        /// <param name="slaveAddress">Address of device to read values from.</param>
        /// <param name="startAddress">Address to begin writing values.</param>
        /// <param name="data">Values to write.</param>
        public void WriteMultipleRegisters(byte slaveAddress, ushort startAddress, ushort[] data)
            => ModbusMaster.WriteMultipleRegisters(slaveAddress, startAddress, data);

        /// <summary>
        /// Asynchronously writes a block of 1 to 123 contiguous registers.
        /// </summary>
        /// <param name="slaveAddress">Address of device to read values from.</param>
        /// <param name="startAddress">Address to begin writing values.</param>
        /// <param name="data">Values to write.</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        public async Task WriteMultipleRegistersAsync(byte slaveAddress, ushort startAddress, ushort[] data)
            => await ModbusMaster.WriteMultipleRegistersAsync(slaveAddress, startAddress, data);

        /// <summary>
        /// Writes a single coil value.
        /// </summary>
        /// <param name="slaveAddress">Address of device to read values from.</param>
        /// <param name="coilAddress">Address to write value to.</param>
        /// <param name="value">Value to write.</param>
        public void WriteSingleCoil(byte slaveAddress, ushort coilAddress, bool value)
            => ModbusMaster.WriteSingleCoil(slaveAddress, coilAddress, value);

        /// <summary>
        /// Asynchronously writes a single coil value.
        /// </summary>
        /// <param name="slaveAddress">Address of device to read values from.</param>
        /// <param name="coilAddress">Address to write value to.</param>
        /// <param name="value">Value to write.</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        public async Task WriteSingleCoilAsync(byte slaveAddress, ushort coilAddress, bool value)
            => await ModbusMaster.WriteSingleCoilAsync(slaveAddress, coilAddress, value);

        /// <summary>
        /// Writes a single holding register.
        /// </summary>
        /// <param name="slaveAddress">Address of device to read values from.</param>
        /// <param name="registerAddress">Address to write.</param>
        /// <param name="value">Value to write.</param>
        public void WriteSingleRegister(byte slaveAddress, ushort registerAddress, ushort value)
            => ModbusMaster.WriteSingleRegister(slaveAddress, registerAddress, value);

        /// <summary>
        /// Asynchronously writes a single holding register.
        /// </summary>
        /// <param name="slaveAddress">Address of device to read values from.</param>
        /// <param name="registerAddress">Address to write.</param>
        /// <param name="value">Value to write.</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        public async Task WriteSingleRegisterAsync(byte slaveAddress, ushort registerAddress, ushort value)
            => await ModbusMaster.WriteSingleRegisterAsync(slaveAddress, registerAddress, value);

        #endregion Write Functions

        #region Extended Read Functions

        /// <summary>
        /// Reads an ASCII string (multiple holding register).
        /// </summary>
        /// <param name="slaveAddress">Address of device to read values from.</param>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <param name="numberOfCharacters">Number of characters to read.</param>
        /// <returns>ASCII string</returns>
        public string ReadString(byte slaveAddress, ushort startAddress, ushort numberOfCharacters)
            => ModbusMaster.ReadString(slaveAddress, startAddress, numberOfCharacters, SwapBytes);

        /// <summary>
        /// Reads a HEX string (multiple holding register).
        /// </summary>
        /// <param name="slaveAddress">Address of device to read values from.</param>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <param name="numberOfHex">Number of bytes to read.</param>
        /// <returns>HEX string</returns>
        public string ReadHexString(byte slaveAddress, ushort startAddress, ushort numberOfHex)
            => ModbusMaster.ReadHexString(slaveAddress, startAddress, numberOfHex, SwapBytes);

        /// <summary>
        /// Reads a single boolean value.
        /// </summary>
        /// <param name="slaveAddress">Address of device to read values from.</param>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <returns>bool value.</returns>
        public bool ReadBool(byte slaveAddress, ushort startAddress)
            => ModbusMaster.ReadBool(slaveAddress, startAddress);

        /// <summary>
        /// Reads a 16 bit array (single holding register)
        /// </summary>
        /// <param name="slaveAddress">Address of device to read values from.</param>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <returns>16 bit BitArray.</returns>
        public BitArray ReadBits(byte slaveAddress, ushort startAddress)
            => ModbusMaster.ReadBits(slaveAddress, startAddress);

        /// <summary>
        /// Reads a 16 bit integer (single holding register).
        /// </summary>
        /// <param name="slaveAddress">Address of device to read values from.</param>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <returns>16 bit integer.</returns>
        public short ReadShort(byte slaveAddress, ushort startAddress)
            => ModbusMaster.ReadShort(slaveAddress, startAddress, SwapBytes);

        /// <summary>
        /// Reads a single unsigned 16 bit integer (single holding register).
        /// </summary>
        /// <param name="slaveAddress">Address of device to read values from.</param>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <returns>Unsigned 16 bit integer.</returns>
        public ushort ReadUShort(byte slaveAddress, ushort startAddress)
            => ModbusMaster.ReadUShort(slaveAddress, startAddress, SwapBytes);

        /// <summary>
        /// Reads an 32 bit integer (two holding registers).
        /// </summary>
        /// <param name="slaveAddress">Address of device to read values from.</param>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <returns>32 bit integer.</returns>
        public int ReadInt32(byte slaveAddress, ushort startAddress)
            => ModbusMaster.ReadInt32(slaveAddress, startAddress, SwapBytes, SwapWords);

        /// <summary>
        /// Reads a single unsigned 32 bit integer (two holding registers).
        /// </summary>
        /// <param name="slaveAddress">Address of device to read values from.</param>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <returns>Unsigned 32 bit integer.</returns>
        public uint ReadUInt32(byte slaveAddress, ushort startAddress)
            => ModbusMaster.ReadUInt32(slaveAddress, startAddress, SwapBytes, SwapWords);

        /// <summary>
        /// Reads a single float value (two holding registers).
        /// </summary>
        /// <param name="slaveAddress">Address of device to read values from.</param>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <returns>Float value.</returns>
        public float ReadFloat(byte slaveAddress, ushort startAddress)
            => ModbusMaster.ReadFloat(slaveAddress, startAddress, SwapBytes, SwapWords);

        /// <summary>
        /// Reads a single double value (four holding registers).
        /// </summary>
        /// <param name="slaveAddress">Address of device to read values from.</param>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <returns>Double value.</returns>
        public double ReadDouble(byte slaveAddress, ushort startAddress)
            => ModbusMaster.ReadDouble(slaveAddress, startAddress, SwapBytes, SwapWords);

        /// <summary>
        /// Reads a 64 bit integer (four holding registers).
        /// </summary>
        /// <param name="slaveAddress">Address of device to read values from.</param>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <returns>64 bit integer.</returns>
        public long ReadLong(byte slaveAddress, ushort startAddress)
            => ModbusMaster.ReadLong(slaveAddress, startAddress, SwapBytes, SwapWords);

        /// <summary>
        /// Reads an unsigned 64 bit integer (four holding registers).
        /// </summary>
        /// <param name="slaveAddress">Address of device to read values from.</param>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <returns>Unsigned 64 bit integer.</returns>
        public ulong ReadULong(byte slaveAddress, ushort startAddress)
            => ModbusMaster.ReadULong(slaveAddress, startAddress, SwapBytes, SwapWords);

        /// <summary>
        /// Reads an array of boolean values (multiple coils).
        /// </summary>
        /// <param name="slaveAddress">Address of device to read values from.</param>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <param name="length">Size of array.</param>
        /// <returns>Array of Bool values.</returns>
        public bool[] ReadBoolArray(byte slaveAddress, ushort startAddress, ushort length)
            => ModbusMaster.ReadBoolArray(slaveAddress, startAddress, length);

        /// <summary>
        /// Reads 8 bit values (multiple holding register).
        /// </summary>
        /// <param name="slaveAddress">Address of device to read values from.</param>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <param name="length">Size of array.</param>
        /// <returns>Array of bytes.</returns>
        public byte[] ReadBytes(byte slaveAddress, ushort startAddress, ushort length)
            => ModbusMaster.ReadBytes(slaveAddress, startAddress, length, SwapBytes);

        /// <summary>
        /// Reads an array of 16 bit integers (multiple holding registers).
        /// </summary>
        /// <param name="slaveAddress">Address of device to read values from.</param>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <param name="length">Size of array.</param>
        /// <returns>Array of 16 bit integers.</returns>
        public short[] ReadShortArray(byte slaveAddress, ushort startAddress, ushort length)
            => ModbusMaster.ReadShortArray(slaveAddress, startAddress, length, SwapBytes);

        /// <summary>
        /// Reads an array of unsigned 16 bit integer (multiple holding registers).
        /// </summary>
        /// <param name="slaveAddress">Address of device to read values from.</param>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <param name="length">Size of array.</param>
        /// <returns>Array of unsigned 16 bit integer.</returns>
        public ushort[] ReadUShortArray(byte slaveAddress, ushort startAddress, ushort length)
            => ModbusMaster.ReadUShortArray(slaveAddress, startAddress, length, SwapBytes);

        /// <summary>
        /// Reads an array of 32 bit integers (multiple holding registers).
        /// </summary>
        /// <param name="slaveAddress">Address of device to read values from.</param>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <param name="length">Size of array.</param>
        /// <returns>Array of 32 bit integers.</returns>
        public int[] ReadInt32Array(byte slaveAddress, ushort startAddress, ushort length)
            => ModbusMaster.ReadInt32Array(slaveAddress, startAddress, length, SwapBytes, SwapWords);

        /// <summary>
        /// Reads an array of unsigned 32 bit integers (multiple holding registers).
        /// </summary>
        /// <param name="slaveAddress">Address of device to read values from.</param>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <param name="length">Size of array.</param>
        /// <returns>Array of unsigned 32 bit integers.</returns>
        public uint[] ReadUInt32Array(byte slaveAddress, ushort startAddress, ushort length)
            => ModbusMaster.ReadUInt32Array(slaveAddress, startAddress, length, SwapBytes, SwapWords);

        /// <summary>
        /// Reads an array of 32 bit IEEE floating point numbers (multiple holding registers).
        /// </summary>
        /// <param name="slaveAddress">Address of device to read values from.</param>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <param name="length">Size of array.</param>
        /// <returns>Array of 32 bit IEEE floating point numbers.</returns>
        public float[] ReadFloatArray(byte slaveAddress, ushort startAddress, ushort length)
            => ModbusMaster.ReadFloatArray(slaveAddress, startAddress, length, SwapBytes, SwapWords);

        /// <summary>
        /// Reads an array of 64 bit IEEE floating point numbers (multiple holding registers).
        /// </summary>
        /// <param name="slaveAddress">Address of device to read values from.</param>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <param name="length">Size of array.</param>
        /// <returns>Array of 64 bit IEEE floating point numbers.</returns>
        public double[] ReadDoubleArray(byte slaveAddress, ushort startAddress, ushort length)
            => ModbusMaster.ReadDoubleArray(slaveAddress, startAddress, length, SwapBytes, SwapWords);

        /// <summary>
        /// Reads an array of 64 bit integers (multiple holding registers).
        /// </summary>
        /// <param name="slaveAddress">Address of device to read values from.</param>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <param name="length">Size of array.</param>
        /// <returns>Array of 64 bit integers.</returns>
        public long[] ReadLongArray(byte slaveAddress, ushort startAddress, ushort length)
            => ModbusMaster.ReadLongArray(slaveAddress, startAddress, length, SwapBytes, SwapWords);

        /// <summary>
        /// Reads an array of unsigned 64 bit integers (multiple holding registers).
        /// </summary>
        /// <param name="slaveAddress">Address of device to read values from.</param>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <param name="length">Size of array.</param>
        /// <returns>Array of unsigned 64 bit integers.</returns>
        public ulong[] ReadULongArray(byte slaveAddress, ushort startAddress, ushort length)
            => ModbusMaster.ReadULongArray(slaveAddress, startAddress, length, SwapBytes, SwapWords);

        #endregion Extended Read Functions

        #region Extended Write Functions

        /// <summary>
        /// Writes an ASCII string (multiple holding registers).
        /// </summary>
        /// <param name="slaveAddress">Address of device to read values from.</param>
        /// <param name="startAddress">Address to begin writing.</param>
        /// <param name="text">ASCII string to be written.</param>
        /// <returns>The task representing the async void write string method.</returns>
        public void WriteString(byte slaveAddress, ushort startAddress, string text)
             => ModbusMaster.WriteString(slaveAddress, startAddress, text, SwapBytes);

        /// <summary>
        /// Writes a HEX string (multiple holding registers).
        /// </summary>
        /// <param name="slaveAddress">Address of device to read values from.</param>
        /// <param name="startAddress">Address to begin writing.</param>
        /// <param name="hex">HEX string to be written.</param>
        /// <returns>The task representing the async void write HEX string method.</returns>
        public void WriteHexString(byte slaveAddress, ushort startAddress, string hex)
             => ModbusMaster.WriteHexString(slaveAddress, startAddress, hex, SwapBytes);

        /// <summary>
        /// Writes a single boolean value (single coil).
        /// </summary>
        /// <param name="slaveAddress">Address of device to read values from.</param>
        /// <param name="startAddress">Address to begin writing.</param>
        /// <param name="value">uint value to be written.</param>
        /// <returns>The task representing the async void write bool method.</returns>
        public void WriteBool(byte slaveAddress, ushort startAddress, bool value)
            => ModbusMaster.WriteBool(slaveAddress, startAddress, value);

        /// <summary>
        /// Writes a 16 bit array (single holding register).
        /// </summary>
        /// <param name="slaveAddress">Address of device to read values from.</param>
        /// <param name="startAddress">Address to begin writing.</param>
        /// <param name="value">BitArray value to be written.</param>
        /// <returns>The task representing the async void write bits method.</returns>
        public void WriteBits(byte slaveAddress, ushort startAddress, BitArray value)
            => ModbusMaster.WriteBits(slaveAddress, startAddress, value);

        /// <summary>
        /// Writes a 16 bit integer (single holding register).
        /// </summary>
        /// <param name="slaveAddress">Address of device to read values from.</param>
        /// <param name="startAddress">Address to begin writing.</param>
        /// <param name="value">Short value to be written.</param>
        /// <returns>The task representing the async void write short method.</returns>
        public void WriteShort(byte slaveAddress, ushort startAddress, short value)
            => ModbusMaster.WriteShort(slaveAddress, startAddress, value, SwapBytes);

        /// <summary>
        /// Writes a single unsigned 16 bit integer value.
        /// </summary>
        /// <param name="slaveAddress">Address of device to read values from.</param>
        /// <param name="startAddress">Address to begin writing.</param>
        /// <param name="value">uint value to be written.</param>
        /// <returns>The task representing the async void write unsigned short method.</returns>
        public void WriteUShort(byte slaveAddress, ushort startAddress, ushort value)
            => ModbusMaster.WriteUShort(slaveAddress, startAddress, value, SwapBytes);

        /// <summary>
        /// Writes a single 32 bit integer value (two holding registers).
        /// </summary>
        /// <param name="slaveAddress">Address of device to read values from.</param>
        /// <param name="startAddress">Address to begin writing.</param>
        /// <param name="value">uint value to be written.</param>
        /// <returns>The task representing the async void write 32-bit integer method.</returns>
        public void WriteInt32(byte slaveAddress, ushort startAddress, int value)
            => ModbusMaster.WriteInt32(slaveAddress, startAddress, value, SwapBytes, SwapWords);

        /// <summary>
        /// Writes a single unsigned 32 bit integer value (two holding registers).
        /// </summary>
        /// <param name="slaveAddress">Address of device to read values from.</param>
        /// <param name="startAddress">Address to begin writing.</param>
        /// <param name="value">uint value to be written.</param>
        /// <returns>The task representing the async void write unsigned 32-bit integer method.</returns>
        public void WriteUInt32(byte slaveAddress, ushort startAddress, uint value)
            => ModbusMaster.WriteUInt32(slaveAddress, startAddress, value, SwapBytes, SwapWords);

        /// <summary>
        /// Writes a single float value (two holding registers).
        /// </summary>
        /// <param name="slaveAddress">Address of device to read values from.</param>
        /// <param name="startAddress">Address to begin writing.</param>
        /// <param name="value">float value to be written.</param>
        /// <returns>The task representing the async void write float method.</returns>
        public void WriteFloat(byte slaveAddress, ushort startAddress, float value)
            => ModbusMaster.WriteFloat(slaveAddress, startAddress, value, SwapBytes, SwapWords);

        /// <summary>
        /// Writes a single double value (four holding registers).
        /// </summary>
        /// <param name="slaveAddress">Address of device to read values from.</param>
        /// <param name="startAddress">Address to begin writing.</param>
        /// <param name="value">double value to be written.</param>
        /// <returns>The task representing the async void write double method.</returns>
        public void WriteDouble(byte slaveAddress, ushort startAddress, double value)
            => ModbusMaster.WriteDouble(slaveAddress, startAddress, value, SwapBytes, SwapWords);

        /// <summary>
        /// Writes a 64 bit integer (four holding registers).
        /// </summary>
        /// <param name="slaveAddress">Address of device to read values from.</param>
        /// <param name="startAddress">Address to begin writing.</param>
        /// <param name="value">Long value to be written.</param>
        /// <returns>The task representing the async void write long method.</returns>
        public void WriteLong(byte slaveAddress, ushort startAddress, long value)
            => ModbusMaster.WriteLong(slaveAddress, startAddress, value, SwapBytes, SwapWords);

        /// <summary>
        /// Writes an unsigned 64 bit integer (four holding registers).
        /// </summary>
        /// <param name="slaveAddress">Address of device to read values from.</param>
        /// <param name="startAddress">Address to begin writing.</param>
        /// <param name="value">Short value to be written.</param>
        /// <returns>The task representing the async void write unsigned long method.</returns>
        public void WriteULong(byte slaveAddress, ushort startAddress, ulong value)
            => ModbusMaster.WriteULong(slaveAddress, startAddress, value, SwapBytes, SwapWords);

        /// <summary>
        /// Writes an array of boolean values (multiple coils)
        /// </summary>
        /// <param name="slaveAddress">Address of device to read values from.</param>
        /// <param name="startAddress">Address to begin writing.</param>
        /// <param name="values">Array of boolean values to be written.</param>
        /// <returns>The task representing the async void write bool array method.</returns>
        public void WriteBoolArray(byte slaveAddress, ushort startAddress, bool[] values)
            => ModbusMaster.WriteBoolArray(slaveAddress, startAddress, values);

        /// <summary>
        /// Writes 8 bit values (multiple holding registers).
        /// </summary>
        /// <param name="slaveAddress">Address of device to read values from.</param>
        /// <param name="startAddress">Address to begin writing.</param>
        /// <param name="values">Short value to be written.</param>
        /// <returns>The task representing the async void write byte array method.</returns>
        public void WriteBytes(byte slaveAddress, ushort startAddress, byte[] values)
            => ModbusMaster.WriteBytes(slaveAddress, startAddress, values);

        /// <summary>
        /// Writes an array of 16 bit integers (multiple holding registers).
        /// </summary>
        /// <param name="slaveAddress">Address of device to read values from.</param>
        /// <param name="startAddress">Address to begin writing.</param>
        /// <param name="values">Array of short values to be written.</param>
        /// <returns>The task representing the async void write short array method.</returns>
        public void WriteShortArray(byte slaveAddress, ushort startAddress, short[] values)
            => ModbusMaster.WriteShortArray(slaveAddress, startAddress, values, SwapBytes);

        /// <summary>
        /// Writes an array of unsigned 16 bit integer (multiple holding registers).
        /// </summary>
        /// <param name="slaveAddress">Address of device to read values from.</param>
        /// <param name="startAddress">Address to begin writing.</param>
        /// <param name="values">Array of unsigned short values to be written.</param>
        /// <returns>The task representing the async void write unsigned short array method.</returns>
        public void WriteUShortArray(byte slaveAddress, ushort startAddress, ushort[] values)
            => ModbusMaster.WriteUShortArray(slaveAddress, startAddress, values, SwapBytes);

        /// <summary>
        /// Writes an array of 32 bit integers (multiple holding registers).
        /// </summary>
        /// <param name="slaveAddress">Address of device to read values from.</param>
        /// <param name="startAddress">Address to begin writing.</param>
        /// <param name="values">Array of Int32 values to be written.</param>
        /// <returns>The task representing the async void write 32-bit integer array method.</returns>
        public void WriteInt32Array(byte slaveAddress, ushort startAddress, int[] values)
            => ModbusMaster.WriteInt32Array(slaveAddress, startAddress, values, SwapBytes, SwapWords);

        /// <summary>
        /// Writes an array of unsigned 32 bit integers (multiple holding registers).
        /// </summary>
        /// <param name="slaveAddress">Address of device to read values from.</param>
        /// <param name="startAddress">Address to begin writing.</param>
        /// <param name="values">Array of UInt32 values to be written.</param>
        /// <returns>The task representing the async void write unsigned 32-bit integer array method.</returns>
        public void WriteUInt32Array(byte slaveAddress, ushort startAddress, uint[] values)
            => ModbusMaster.WriteUInt32Array(slaveAddress, startAddress, values, SwapBytes, SwapWords);

        /// <summary>
        /// Writes an array of 32 bit IEEE floating point numbers (multiple holding registers).
        /// </summary>
        /// <param name="slaveAddress">Address of device to read values from.</param>
        /// <param name="startAddress">Address to begin writing.</param>
        /// <param name="values">Short value to be written.</param>
        /// <returns>The task representing the async void write float array method.</returns>
        public void WriteFloatArray(byte slaveAddress, ushort startAddress, float[] values)
            => ModbusMaster.WriteFloatArray(slaveAddress, startAddress, values, SwapBytes, SwapWords);

        /// <summary>
        /// Writes an array of 64 bit IEEE floating point numbers (multiple holding registers).
        /// </summary>
        /// <param name="slaveAddress">Address of device to read values from.</param>
        /// <param name="startAddress">Address to begin writing.</param>
        /// <param name="values">Short value to be written.</param>
        /// <returns>The task representing the async void write double array method.</returns>
        public void WriteDoubleArray(byte slaveAddress, ushort startAddress, double[] values)
            => ModbusMaster.WriteDoubleArray(slaveAddress, startAddress, values, SwapBytes, SwapWords);

        /// <summary>
        /// Writes an array of 64 bit integers (multiple holding registers).
        /// </summary>
        /// <param name="slaveAddress">Address of device to read values from.</param>
        /// <param name="startAddress">Address to begin writing.</param>
        /// <param name="values">Array of long values to be written.</param>
        /// <returns>The task representing the async void write long array method.</returns>
        public void WriteLongArray(byte slaveAddress, ushort startAddress, long[] values)
            => ModbusMaster.WriteLongArray(slaveAddress, startAddress, values, SwapBytes, SwapWords);

        /// <summary>
        /// Writes an array of unsigned 64 bit integers (multiple holding registers).
        /// </summary>
        /// <param name="slaveAddress">Address of device to read values from.</param>
        /// <param name="startAddress">Address to begin writing.</param>
        /// <param name="values">Array of unsigned long values to be written.</param>
        /// <returns>The task representing the async void write unsigned long array method.</returns>
        public void WriteULongArray(byte slaveAddress, ushort startAddress, ulong[] values)
            => ModbusMaster.WriteULongArray(slaveAddress, startAddress, values, SwapBytes, SwapWords);

        #endregion Extended Write Functions

        #region Extended Read Only Functions

        /// <summary>
        /// Reads an ASCII string (multiple input register).
        /// </summary>
        /// <param name="slaveAddress">Address of device to read values from.</param>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <param name="numberOfCharacters">Number of characters to read.</param>
        /// <returns>ASCII string</returns>
        public string ReadOnlyString(byte slaveAddress, ushort startAddress, ushort numberOfCharacters)
            => ModbusMaster.ReadOnlyString(slaveAddress, startAddress, numberOfCharacters, SwapBytes);

        /// <summary>
        /// Reads a HEX string (multiple input register).
        /// </summary>
        /// <param name="slaveAddress">Address of device to read values from.</param>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <param name="numberOfHex">Number of characters to read.</param>
        /// <returns>ASCII string</returns>
        public string ReadOnlyHexString(byte slaveAddress, ushort startAddress, ushort numberOfHex)
            => ModbusMaster.ReadOnlyHexString(slaveAddress, startAddress, numberOfHex, SwapBytes);

        /// <summary>
        /// Reads a single boolean value.
        /// </summary>
        /// <param name="slaveAddress">Address of device to read values from.</param>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <returns>bool value.</returns>
        public bool ReadOnlyBool(byte slaveAddress, ushort startAddress)
            => ModbusMaster.ReadOnlyBool(slaveAddress, startAddress);

        /// <summary>
        /// Reads a 16 bit array (single input register)
        /// </summary>
        /// <param name="slaveAddress">Address of device to read values from.</param>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <returns>16 bit BitArray.</returns>
        public BitArray ReadOnlyBits(byte slaveAddress, ushort startAddress)
            => ModbusMaster.ReadOnlyBits(slaveAddress, startAddress);

        /// <summary>
        /// Reads a 16 bit integer (single input register).
        /// </summary>
        /// <param name="slaveAddress">Address of device to read values from.</param>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <returns>16 bit integer.</returns>
        public short ReadOnlyShort(byte slaveAddress, ushort startAddress)
            => ModbusMaster.ReadOnlyShort(slaveAddress, startAddress, SwapBytes);

        /// <summary>
        /// Reads a single unsigned 16 bit integer (single input register).
        /// </summary>
        /// <param name="slaveAddress">Address of device to read values from.</param>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <returns>Unsigned 16 bit integer.</returns>
        public ushort ReadOnlyUShort(byte slaveAddress, ushort startAddress)
            => ModbusMaster.ReadOnlyUShort(slaveAddress, startAddress, SwapBytes);

        /// <summary>
        /// Reads an 32 bit integer (two input registers).
        /// </summary>
        /// <param name="slaveAddress">Address of device to read values from.</param>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <returns>32 bit integer.</returns>
        public int ReadOnlyInt32(byte slaveAddress, ushort startAddress)
            => ModbusMaster.ReadOnlyInt32(slaveAddress, startAddress, SwapBytes, SwapWords);

        /// <summary>
        /// Reads a single unsigned 32 bit integer (two input registers).
        /// </summary>
        /// <param name="slaveAddress">Address of device to read values from.</param>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <returns>Unsigned 32 bit integer.</returns>
        public uint ReadOnlyUInt32(byte slaveAddress, ushort startAddress)
            => ModbusMaster.ReadOnlyUInt32(slaveAddress, startAddress, SwapBytes, SwapWords);

        /// <summary>
        /// Reads a single float value (two input registers).
        /// </summary>
        /// <param name="slaveAddress">Address of device to read values from.</param>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <returns>Float value.</returns>
        public float ReadOnlyFloat(byte slaveAddress, ushort startAddress)
            => ModbusMaster.ReadOnlyFloat(slaveAddress, startAddress, SwapBytes, SwapWords);

        /// <summary>
        /// Reads a single double value (four input registers).
        /// </summary>
        /// <param name="slaveAddress">Address of device to read values from.</param>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <returns>Double value.</returns>
        public double ReadOnlyDouble(byte slaveAddress, ushort startAddress)
            => ModbusMaster.ReadOnlyDouble(slaveAddress, startAddress, SwapBytes, SwapWords);

        /// <summary>
        /// Reads a 64 bit integer (four input registers).
        /// </summary>
        /// <param name="slaveAddress">Address of device to read values from.</param>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <returns>64 bit integer.</returns>
        public long ReadOnlyLong(byte slaveAddress, ushort startAddress)
            => ModbusMaster.ReadOnlyLong(slaveAddress, startAddress, SwapBytes, SwapWords);

        /// <summary>
        /// Reads an unsigned 64 bit integer (four input registers).
        /// </summary>
        /// <param name="slaveAddress">Address of device to read values from.</param>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <returns>Unsigned 64 bit integer.</returns>
        public ulong ReadOnlyULong(byte slaveAddress, ushort startAddress)
            => ModbusMaster.ReadOnlyULong(slaveAddress, startAddress, SwapBytes, SwapWords);

        /// <summary>
        /// Reads an array of boolean values (multiple discrete inputs).
        /// </summary>
        /// <param name="slaveAddress">Address of device to read values from.</param>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <param name="length">Size of array.</param>
        /// <returns>Array of Bool values.</returns>
        public bool[] ReadOnlyBoolArray(byte slaveAddress, ushort startAddress, ushort length)
            => ModbusMaster.ReadOnlyBoolArray(slaveAddress, startAddress, length);

        /// <summary>
        /// Reads 8 bit values (multiple input register).
        /// </summary>
        /// <param name="slaveAddress">Address of device to read values from.</param>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <param name="length">Size of array.</param>
        /// <returns>Arroy of bytes.</returns>
        public byte[] ReadOnlyBytes(byte slaveAddress, ushort startAddress, ushort length)
            => ModbusMaster.ReadOnlyBytes(slaveAddress, startAddress, length, SwapBytes);

        /// <summary>
        /// Reads an array of 16 bit integers (multiple input registers).
        /// </summary>
        /// <param name="slaveAddress">Address of device to read values from.</param>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <param name="length">Size of array.</param>
        /// <returns>Array of 16 bit integers.</returns>
        public short[] ReadOnlyShortArray(byte slaveAddress, ushort startAddress, ushort length)
            => ModbusMaster.ReadOnlyShortArray(slaveAddress, startAddress, length, SwapBytes);

        /// <summary>
        /// Reads an array of unsigned 16 bit integer (multiple input registers).
        /// </summary>
        /// <param name="slaveAddress">Address of device to read values from.</param>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <param name="length">Size of array.</param>
        /// <returns>Array of unsigned 16 bit integer.</returns>
        public ushort[] ReadOnlyUShortArray(byte slaveAddress, ushort startAddress, ushort length)
            => ModbusMaster.ReadOnlyUShortArray(slaveAddress, startAddress, length, SwapBytes);

        /// <summary>
        /// Reads an array of 32 bit integers (multiple input registers).
        /// </summary>
        /// <param name="slaveAddress">Address of device to read values from.</param>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <param name="length">Size of array.</param>
        /// <returns>Array of 32 bit integers.</returns>
        public int[] ReadOnlyInt32Array(byte slaveAddress, ushort startAddress, ushort length)
            => ModbusMaster.ReadOnlyInt32Array(slaveAddress, startAddress, length, SwapBytes, SwapWords);

        /// <summary>
        /// Reads an array of unsigned 32 bit integers (multiple input registers).
        /// </summary>
        /// <param name="slaveAddress">Address of device to read values from.</param>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <param name="length">Size of array.</param>
        /// <returns>Array of unsigned 32 bit integers.</returns>
        public uint[] ReadOnlyUInt32Array(byte slaveAddress, ushort startAddress, ushort length)
            => ModbusMaster.ReadOnlyUInt32Array(slaveAddress, startAddress, length, SwapBytes, SwapWords);

        /// <summary>
        /// Reads an array of 32 bit IEEE floating point numbers (multiple input registers).
        /// </summary>
        /// <param name="slaveAddress">Address of device to read values from.</param>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <param name="length">Size of array.</param>
        /// <returns>Array of 32 bit IEEE floating point numbers.</returns>
        public float[] ReadOnlyFloatArray(byte slaveAddress, ushort startAddress, ushort length)
            => ModbusMaster.ReadOnlyFloatArray(slaveAddress, startAddress, length, SwapBytes, SwapWords);

        /// <summary>
        /// Reads an array of 64 bit IEEE floating point numbers (multiple input registers).
        /// </summary>
        /// <param name="slaveAddress">Address of device to read values from.</param>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <param name="length">Size of array.</param>
        /// <returns>Array of 64 bit IEEE floating point numbers.</returns>
        public double[] ReadOnlyDoubleArray(byte slaveAddress, ushort startAddress, ushort length)
            => ModbusMaster.ReadOnlyDoubleArray(slaveAddress, startAddress, length, SwapBytes, SwapWords);

        /// <summary>
        /// Reads an array of 64 bit integers (multiple input registers).
        /// </summary>
        /// <param name="slaveAddress">Address of device to read values from.</param>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <param name="length">Size of array.</param>
        /// <returns>Array of 64 bit integers.</returns>
        public long[] ReadOnlyLongArray(byte slaveAddress, ushort startAddress, ushort length)
            => ModbusMaster.ReadOnlyLongArray(slaveAddress, startAddress, length, SwapBytes, SwapWords);

        /// <summary>
        /// Reads an array of unsigned 64 bit integers (multiple input registers).
        /// </summary>
        /// <param name="slaveAddress">Address of device to read values from.</param>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <param name="length">Size of array.</param>
        /// <returns>Array of unsigned 64 bit integers.</returns>
        public ulong[] ReadOnlyULongArray(byte slaveAddress, ushort startAddress, ushort length)
            => ModbusMaster.ReadOnlyULongArray(slaveAddress, startAddress, length, SwapBytes, SwapWords);

        #endregion Extended Read Only Functions

        #region Extended Async Read Functions

        /// <summary>
        /// Asynchronously reads an ASCII string (multiple holding register).
        /// </summary>
        /// <param name="slaveAddress">Address of device to read values from.</param>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <param name="numberOfCharacters">Number of characters to read.</param>
        /// <returns>ASCII string</returns>
        public async Task<string> ReadStringAsync(byte slaveAddress, ushort startAddress, ushort numberOfCharacters)
            => await ModbusMaster.ReadStringAsync(slaveAddress, startAddress, numberOfCharacters, SwapBytes);

        /// <summary>
        /// Asynchronously reads a HEX string (multiple holding register).
        /// </summary>
        /// <param name="slaveAddress">Address of device to read values from.</param>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <param name="numberOfHex">Number of bytes to read.</param>
        /// <returns>HEX string</returns>
        public async Task<string> ReadHexStringAsync(byte slaveAddress, ushort startAddress, ushort numberOfHex)
            => await ModbusMaster.ReadHexStringAsync(slaveAddress, startAddress, numberOfHex, SwapBytes);

        /// <summary>
        /// Asynchronously reads a single boolean value.
        /// </summary>
        /// <param name="slaveAddress">Address of device to read values from.</param>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <returns>bool value.</returns>
        public async Task<bool> ReadBoolAsync(byte slaveAddress, ushort startAddress)
            => await ModbusMaster.ReadBoolAsync(slaveAddress, startAddress);

        /// <summary>
        /// Asynchronously reads a 16 bit array (single holding register)
        /// </summary>
        /// <param name="slaveAddress">Address of device to read values from.</param>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <returns>16 bit BitArray.</returns>
        public async Task<BitArray> ReadBitsAsync(byte slaveAddress, ushort startAddress)
            => await ModbusMaster.ReadBitsAsync(slaveAddress, startAddress);

        /// <summary>
        /// Asynchronously reads a 16 bit integer (single holding register).
        /// </summary>
        /// <param name="slaveAddress">Address of device to read values from.</param>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <returns>16 bit integer.</returns>
        public async Task<short> ReadShortAsync(byte slaveAddress, ushort startAddress)
            => await ModbusMaster.ReadShortAsync(slaveAddress, startAddress, SwapBytes);

        /// <summary>
        /// Asynchronously reads a single unsigned 16 bit integer (single holding register).
        /// </summary>
        /// <param name="slaveAddress">Address of device to read values from.</param>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <returns>Unsigned 16 bit integer.</returns>
        public async Task<ushort> ReadUShortAsync(byte slaveAddress, ushort startAddress)
            => await ModbusMaster.ReadUShortAsync(slaveAddress, startAddress, SwapBytes);

        /// <summary>
        /// Asynchronously reads an 32 bit integer (two holding registers).
        /// </summary>
        /// <param name="slaveAddress">Address of device to read values from.</param>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <returns>32 bit integer.</returns>
        public async Task<Int32> ReadInt32Async(byte slaveAddress, ushort startAddress)
            => await ModbusMaster.ReadInt32Async(slaveAddress, startAddress, SwapBytes, SwapWords);

        /// <summary>
        /// Asynchronously reads a single unsigned 32 bit integer (two holding registers).
        /// </summary>
        /// <param name="slaveAddress">Address of device to read values from.</param>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <returns>Unsigned 32 bit integer.</returns>
        public async Task<uint> ReadUInt32Async(byte slaveAddress, ushort startAddress)
            => await ModbusMaster.ReadUInt32Async(slaveAddress, startAddress, SwapBytes, SwapWords);

        /// <summary>
        /// Asynchronously reads a single float value (two holding registers).
        /// </summary>
        /// <param name="slaveAddress">Address of device to read values from.</param>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <returns>Float value.</returns>
        public async Task<float> ReadFloatAsync(byte slaveAddress, ushort startAddress)
            => await ModbusMaster.ReadFloatAsync(slaveAddress, startAddress, SwapBytes, SwapWords);

        /// <summary>
        /// Asynchronously reads a single double value (four holding registers).
        /// </summary>
        /// <param name="slaveAddress">Address of device to read values from.</param>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <returns>Double value.</returns>
        public async Task<double> ReadDoubleAsync(byte slaveAddress, ushort startAddress)
            => await ModbusMaster.ReadDoubleAsync(slaveAddress, startAddress, SwapBytes, SwapWords);

        /// <summary>
        /// Asynchronously reads a 64 bit integer (four holding registers).
        /// </summary>
        /// <param name="slaveAddress">Address of device to read values from.</param>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <returns>64 bit integer.</returns>
        public async Task<long> ReadLongAsync(byte slaveAddress, ushort startAddress)
            => await ModbusMaster.ReadLongAsync(slaveAddress, startAddress, SwapBytes, SwapWords);

        /// <summary>
        /// Asynchronously reads an unsigned 64 bit integer (four holding registers).
        /// </summary>
        /// <param name="slaveAddress">Address of device to read values from.</param>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <returns>Unsigned 64 bit integer.</returns>
        public async Task<ulong> ReadULongAsync(byte slaveAddress, ushort startAddress)
            => await ModbusMaster.ReadULongAsync(slaveAddress, startAddress, SwapBytes, SwapWords);

        /// <summary>
        /// Asynchronously reads an array of boolean values (multiple coils).
        /// </summary>
        /// <param name="slaveAddress">Address of device to read values from.</param>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <param name="length">Size of array.</param>
        /// <returns>Array of Bool values.</returns>
        public async Task<bool[]> ReadBoolArrayAsync(byte slaveAddress, ushort startAddress, ushort length)
            => await ModbusMaster.ReadBoolArrayAsync(slaveAddress, startAddress, length);

        /// <summary>
        /// Asynchronously reads 8 bit values (multiple holding register).
        /// </summary>
        /// <param name="slaveAddress">Address of device to read values from.</param>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <param name="length">Size of array.</param>
        /// <returns>Array of bytes.</returns>
        public async Task<byte[]> ReadBytesAsync(byte slaveAddress, ushort startAddress, ushort length)
            => await ModbusMaster.ReadBytesAsync(slaveAddress, startAddress, length, SwapBytes);

        /// <summary>
        /// Asynchronously reads an array of 16 bit integers (multiple holding registers).
        /// </summary>
        /// <param name="slaveAddress">Address of device to read values from.</param>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <param name="length">Size of array.</param>
        /// <returns>Array of 16 bit integers.</returns>
        public async Task<short[]> ReadShortArrayAsync(byte slaveAddress, ushort startAddress, ushort length)
            => await ModbusMaster.ReadShortArrayAsync(slaveAddress, startAddress, length, SwapBytes);

        /// <summary>
        /// Asynchronously reads an array of unsigned 16 bit integer (multiple holding registers).
        /// </summary>
        /// <param name="slaveAddress">Address of device to read values from.</param>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <param name="length">Size of array.</param>
        /// <returns>Array of unsigned 16 bit integer.</returns>
        public async Task<ushort[]> ReadUShortArrayAsync(byte slaveAddress, ushort startAddress, ushort length)
            => await ModbusMaster.ReadUShortArrayAsync(slaveAddress, startAddress, length, SwapBytes);

        /// <summary>
        /// Asynchronously reads an array of 32 bit integers (multiple holding registers).
        /// </summary>
        /// <param name="slaveAddress">Address of device to read values from.</param>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <param name="length">Size of array.</param>
        /// <returns>Array of 32 bit integers.</returns>
        public async Task<Int32[]> ReadInt32ArrayAsync(byte slaveAddress, ushort startAddress, ushort length)
            => await ModbusMaster.ReadInt32ArrayAsync(slaveAddress, startAddress, length, SwapBytes, SwapWords);

        /// <summary>
        /// Asynchronously reads an array of unsigned 32 bit integers (multiple holding registers).
        /// </summary>
        /// <param name="slaveAddress">Address of device to read values from.</param>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <param name="length">Size of array.</param>
        /// <returns>Array of unsigned 32 bit integers.</returns>
        public async Task<UInt32[]> ReadUInt32ArrayAsync(byte slaveAddress, ushort startAddress, ushort length)
            => await ModbusMaster.ReadUInt32ArrayAsync(slaveAddress, startAddress, length, SwapBytes, SwapWords);

        /// <summary>
        /// Asynchronously reads an array of 32 bit IEEE floating point numbers (multiple holding registers).
        /// </summary>
        /// <param name="slaveAddress">Address of device to read values from.</param>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <param name="length">Size of array.</param>
        /// <returns>Array of 32 bit IEEE floating point numbers.</returns>
        public async Task<float[]> ReadFloatArrayAsync(byte slaveAddress, ushort startAddress, ushort length)
            => await ModbusMaster.ReadFloatArrayAsync(slaveAddress, startAddress, length, SwapBytes, SwapWords);

        /// <summary>
        /// Asynchronously reads an array of 64 bit IEEE floating point numbers (multiple holding registers).
        /// </summary>
        /// <param name="slaveAddress">Address of device to read values from.</param>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <param name="length">Size of array.</param>
        /// <returns>Array of 64 bit IEEE floating point numbers.</returns>
        public async Task<double[]> ReadDoubleArrayAsync(byte slaveAddress, ushort startAddress, ushort length)
            => await ModbusMaster.ReadDoubleArrayAsync(slaveAddress, startAddress, length, SwapBytes, SwapWords);

        /// <summary>
        /// Asynchronously reads an array of 64 bit integers (multiple holding registers).
        /// </summary>
        /// <param name="slaveAddress">Address of device to read values from.</param>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <param name="length">Size of array.</param>
        /// <returns>Array of 64 bit integers.</returns>
        public async Task<long[]> ReadLongArrayAsync(byte slaveAddress, ushort startAddress, ushort length)
            => await ModbusMaster.ReadLongArrayAsync(slaveAddress, startAddress, length, SwapBytes, SwapWords);

        /// <summary>
        /// Asynchronously reads an array of unsigned 64 bit integers (multiple holding registers).
        /// </summary>
        /// <param name="slaveAddress">Address of device to read values from.</param>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <param name="length">Size of array.</param>
        /// <returns>Array of unsigned 64 bit integers.</returns>
        public async Task<ulong[]> ReadULongArrayAsync(byte slaveAddress, ushort startAddress, ushort length)
            => await ModbusMaster.ReadULongArrayAsync(slaveAddress, startAddress, length, SwapBytes, SwapWords);

        #endregion Extended Async Read Functions

        #region Extended Async Write Functions

        /// <summary>
        /// Asynchronously writes an ASCII string (multiple holding registers).
        /// </summary>
        /// <param name="slaveAddress">Address of device to read values from.</param>
        /// <param name="startAddress">Address to begin writing.</param>
        /// <param name="text">ASCII string to be written.</param>
        /// <returns>The task representing the async void write string method.</returns>
        public async Task WriteStringAsync(byte slaveAddress, ushort startAddress, string text)
            => await ModbusMaster.WriteStringAsync(slaveAddress, startAddress, text, SwapBytes);

        /// <summary>
        /// Asynchronously writes a HEX string (multiple holding registers).
        /// </summary>
        /// <param name="slaveAddress">Address of device to read values from.</param>
        /// <param name="startAddress">Address to begin writing.</param>
        /// <param name="hex">HEX string to be written.</param>
        /// <returns>The task representing the async void write HEX string method.</returns>
        public async Task WriteHexStringAsync(byte slaveAddress, ushort startAddress, string hex)
            => await ModbusMaster.WriteHexStringAsync(slaveAddress, startAddress, hex, SwapBytes);

        /// <summary>
        /// Asynchronously writes a single boolean value (single coil).
        /// </summary>
        /// <param name="slaveAddress">Address of device to read values from.</param>
        /// <param name="startAddress">Address to begin writing.</param>
        /// <param name="value">uint value to be written.</param>
        /// <returns>The task representing the async void write bool method.</returns>
        public async Task WriteBoolAsync(byte slaveAddress, ushort startAddress, bool value)
            => await ModbusMaster.WriteBoolAsync(slaveAddress, startAddress, value);

        /// <summary>
        /// Writes a 16 bit array (single holding register).
        /// </summary>
        /// <param name="slaveAddress">Address of device to read values from.</param>
        /// <param name="startAddress">Address to begin writing.</param>
        /// <param name="value">BitArray value to be written.</param>
        /// <returns>The task representing the async void write bits method.</returns>
        public async Task WriteBitsAsync(byte slaveAddress, ushort startAddress, BitArray value)
            => await ModbusMaster.WriteBitsAsync(slaveAddress, startAddress, value);

        /// <summary>
        /// Asynchronously writes a 16 bit integer (single holding register).
        /// </summary>
        /// <param name="slaveAddress">Address of device to read values from.</param>
        /// <param name="startAddress">Address to begin writing.</param>
        /// <param name="value">Short value to be written.</param>
        /// <returns>The task representing the async void write short method.</returns>
        public async Task WriteShortAsync(byte slaveAddress, ushort startAddress, short value)
            => await ModbusMaster.WriteShortAsync(slaveAddress, startAddress, value, SwapBytes);

        /// <summary>
        /// Asynchronously writes a single unsigned 16 bit integer value.
        /// </summary>
        /// <param name="slaveAddress">Address of device to read values from.</param>
        /// <param name="startAddress">Address to begin writing.</param>
        /// <param name="value">uint value to be written.</param>
        /// <returns>The task representing the async void write unsigned short method.</returns>
        public async Task WriteUShortAsync(byte slaveAddress, ushort startAddress, ushort value)
            => await ModbusMaster.WriteUShortAsync(slaveAddress, startAddress, value, SwapBytes);

        /// <summary>
        /// Asynchronously writes a single 32 bit integer value (two holding registers).
        /// </summary>
        /// <param name="slaveAddress">Address of device to read values from.</param>
        /// <param name="startAddress">Address to begin writing.</param>
        /// <param name="value">uint value to be written.</param>
        /// <returns>The task representing the async void write 32-bit integer method.</returns>
        public async Task WriteInt32Async(byte slaveAddress, ushort startAddress, int value)
            => await ModbusMaster.WriteInt32Async(slaveAddress, startAddress, value, SwapBytes, SwapWords);

        /// <summary>
        /// Asynchronously writes a single unsigned 32 bit integer value (two holding registers).
        /// </summary>
        /// <param name="slaveAddress">Address of device to read values from.</param>
        /// <param name="startAddress">Address to begin writing.</param>
        /// <param name="value">uint value to be written.</param>
        /// <returns>The task representing the async void write unsigned 32-bit integer method.</returns>
        public async Task WriteUInt32Async(byte slaveAddress, ushort startAddress, uint value)
            => await ModbusMaster.WriteUInt32Async(slaveAddress, startAddress, value, SwapBytes, SwapWords);

        /// <summary>
        /// Asynchronously writes a single float value (two holding registers).
        /// </summary>
        /// <param name="slaveAddress">Address of device to read values from.</param>
        /// <param name="startAddress">Address to begin writing.</param>
        /// <param name="value">float value to be written.</param>
        /// <returns>The task representing the async void write float method.</returns>
        public async Task WriteFloatAsync(byte slaveAddress, ushort startAddress, float value)
            => await ModbusMaster.WriteFloatAsync(slaveAddress, startAddress, value, SwapBytes, SwapWords);

        /// <summary>
        /// Asynchronously writes a single double value (four holding registers).
        /// </summary>
        /// <param name="slaveAddress">Address of device to read values from.</param>
        /// <param name="startAddress">Address to begin writing.</param>
        /// <param name="value">double value to be written.</param>
        /// <returns>The task representing the async void write double method.</returns>
        public async Task WriteDoubleAsync(byte slaveAddress, ushort startAddress, double value)
            => await ModbusMaster.WriteDoubleAsync(slaveAddress, startAddress, value, SwapBytes, SwapWords);

        /// <summary>
        /// Asynchronously writes a 64 bit integer (four holding registers).
        /// </summary>
        /// <param name="slaveAddress">Address of device to read values from.</param>
        /// <param name="startAddress">Address to begin writing.</param>
        /// <param name="value">Long value to be written.</param>
        /// <returns>The task representing the async void write long method.</returns>
        public async Task WriteLongAsync(byte slaveAddress, ushort startAddress, long value)
            => await ModbusMaster.WriteLongAsync(slaveAddress, startAddress, value, SwapBytes, SwapWords);

        /// <summary>
        /// Asynchronously writes an unsigned 64 bit integer (four holding registers).
        /// </summary>
        /// <param name="slaveAddress">Address of device to read values from.</param>
        /// <param name="startAddress">Address to begin writing.</param>
        /// <param name="value">Short value to be written.</param>
        /// <returns>The task representing the async void write unsigned long method.</returns>
        public async Task WriteULongAsync(byte slaveAddress, ushort startAddress, ulong value)
            => await ModbusMaster.WriteULongAsync(slaveAddress, startAddress, value, SwapBytes, SwapWords);

        /// <summary>
        /// Asynchronously writes an array of boolean values (multiple coils)
        /// </summary>
        /// <param name="slaveAddress">Address of device to read values from.</param>
        /// <param name="startAddress">Address to begin writing.</param>
        /// <param name="values">Array of boolean values to be written.</param>
        /// <returns>The task representing the async void write bool array method.</returns>
        public async Task WriteBoolArrayAsync(byte slaveAddress, ushort startAddress, bool[] values)
            => await ModbusMaster.WriteBoolArrayAsync(slaveAddress, startAddress, values);

        /// <summary>
        /// Asynchronously writes 8 bit values (multiple holding registers).
        /// </summary>
        /// <param name="slaveAddress">Address of device to read values from.</param>
        /// <param name="startAddress">Address to begin writing.</param>
        /// <param name="values">Short value to be written.</param>
        /// <returns>The task representing the async void write byte array method.</returns>
        public async Task WriteBytesAsync(byte slaveAddress, ushort startAddress, byte[] values)
            => await ModbusMaster.WriteBytesAsync(slaveAddress, startAddress, values);

        /// <summary>
        /// Asynchronously writes an array of 16 bit integers (multiple holding registers).
        /// </summary>
        /// <param name="slaveAddress">Address of device to read values from.</param>
        /// <param name="startAddress">Address to begin writing.</param>
        /// <param name="values">Array of short values to be written.</param>
        /// <returns>The task representing the async void write short array method.</returns>
        public async Task WriteShortArrayAsync(byte slaveAddress, ushort startAddress, short[] values)
            => await ModbusMaster.WriteShortArrayAsync(slaveAddress, startAddress, values, SwapBytes);

        /// <summary>
        /// Asynchronously writes an array of unsigned 16 bit integer (multiple holding registers).
        /// </summary>
        /// <param name="slaveAddress">Address of device to read values from.</param>
        /// <param name="startAddress">Address to begin writing.</param>
        /// <param name="values">Array of unsigned short values to be written.</param>
        /// <returns>The task representing the async void write unsigned short array method.</returns>
        public async Task WriteUShortArrayAsync(byte slaveAddress, ushort startAddress, ushort[] values)
            => await ModbusMaster.WriteUShortArrayAsync(slaveAddress, startAddress, values, SwapBytes);

        /// <summary>
        /// Asynchronously writes an array of 32 bit integers (multiple holding registers).
        /// </summary>
        /// <param name="slaveAddress">Address of device to read values from.</param>
        /// <param name="startAddress">Address to begin writing.</param>
        /// <param name="values">Array of Int32 values to be written.</param>
        /// <returns>The task representing the async void write 32-bit integer array method.</returns>
        public async Task WriteInt32ArrayAsync(byte slaveAddress, ushort startAddress, Int32[] values)
            => await ModbusMaster.WriteInt32ArrayAsync(slaveAddress, startAddress, values, SwapBytes, SwapWords);

        /// <summary>
        /// Asynchronously writes an array of unsigned 32 bit integers (multiple holding registers).
        /// </summary>
        /// <param name="slaveAddress">Address of device to read values from.</param>
        /// <param name="startAddress">Address to begin writing.</param>
        /// <param name="values">Array of UInt32 values to be written.</param>
        /// <returns>The task representing the async void write unsigned 32-bit integer array method.</returns>
        public async Task WriteUInt32ArrayAsync(byte slaveAddress, ushort startAddress, UInt32[] values)
            => await ModbusMaster.WriteUInt32ArrayAsync(slaveAddress, startAddress, values, SwapBytes, SwapWords);

        /// <summary>
        /// Asynchronously writes an array of 32 bit IEEE floating point numbers (multiple holding registers).
        /// </summary>
        /// <param name="slaveAddress">Address of device to read values from.</param>
        /// <param name="startAddress">Address to begin writing.</param>
        /// <param name="values">Short value to be written.</param>
        /// <returns>The task representing the async void write float array method.</returns>
        public async Task WriteFloatArrayAsync(byte slaveAddress, ushort startAddress, float[] values)
            => await ModbusMaster.WriteFloatArrayAsync(slaveAddress, startAddress, values, SwapBytes, SwapWords);

        /// <summary>
        /// Asynchronously writes an array of 64 bit IEEE floating point numbers (multiple holding registers).
        /// </summary>
        /// <param name="slaveAddress">Address of device to read values from.</param>
        /// <param name="startAddress">Address to begin writing.</param>
        /// <param name="values">Short value to be written.</param>
        /// <returns>The task representing the async void write double array method.</returns>
        public async Task WriteDoubleArrayAsync(byte slaveAddress, ushort startAddress, double[] values)
            => await ModbusMaster.WriteDoubleArrayAsync(slaveAddress, startAddress, values, SwapBytes, SwapWords);

        /// <summary>
        /// Asynchronously writes an array of 64 bit integers (multiple holding registers).
        /// </summary>
        /// <param name="slaveAddress">Address of device to read values from.</param>
        /// <param name="startAddress">Address to begin writing.</param>
        /// <param name="values">Array of long values to be written.</param>
        /// <returns>The task representing the async void write long array method.</returns>
        public async Task WriteLongArrayAsync(byte slaveAddress, ushort startAddress, long[] values)
            => await ModbusMaster.WriteLongArrayAsync(slaveAddress, startAddress, values, SwapBytes, SwapWords);

        /// <summary>
        /// Asynchronously writes an array of unsigned 64 bit integers (multiple holding registers).
        /// </summary>
        /// <param name="slaveAddress">Address of device to read values from.</param>
        /// <param name="startAddress">Address to begin writing.</param>
        /// <param name="values">Array of unsigned long values to be written.</param>
        /// <returns>The task representing the async void write unsigned long array method.</returns>
        public async Task WriteULongArrayAsync(byte slaveAddress, ushort startAddress, ulong[] values)
            => await ModbusMaster.WriteULongArrayAsync(slaveAddress, startAddress, values, SwapBytes, SwapWords);

        #endregion Extended Async Write Functions

        #region Extended Async Read Only Functions

        /// <summary>
        /// Asynchronously reads an ASCII string (multiple input register).
        /// </summary>
        /// <param name="slaveAddress">Address of device to read values from.</param>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <param name="numberOfCharacters">Number of characters to read.</param>
        /// <returns>ASCII string</returns>
        public async Task<string> ReadOnlyStringAsync(byte slaveAddress, ushort startAddress, ushort numberOfCharacters)
            => await ModbusMaster.ReadOnlyStringAsync(slaveAddress, startAddress, numberOfCharacters, SwapBytes);

        /// <summary>
        /// Asynchronously reads a HEX string (multiple input register).
        /// </summary>
        /// <param name="slaveAddress">Address of device to read values from.</param>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <param name="numberOfHex">Number of characters to read.</param>
        /// <returns>ASCII string</returns>
        public async Task<string> ReadOnlyHexStringAsync(byte slaveAddress, ushort startAddress, ushort numberOfHex)
            => await ModbusMaster.ReadOnlyHexStringAsync(slaveAddress, startAddress, numberOfHex, SwapBytes);

        /// <summary>
        /// Asynchronously reads a single boolean value.
        /// </summary>
        /// <param name="slaveAddress">Address of device to read values from.</param>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <returns>bool value.</returns>
        public async Task<bool> ReadOnlyBoolAsync(byte slaveAddress, ushort startAddress)
            => await ModbusMaster.ReadOnlyBoolAsync(slaveAddress, startAddress);

        /// <summary>
        /// Asynchronously reads a 16 bit array (single input register)
        /// </summary>
        /// <param name="slaveAddress">Address of device to read values from.</param>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <returns>16 bit BitArray.</returns>
        public async Task<BitArray> ReadOnlyBitsAsync(byte slaveAddress, ushort startAddress)
            => await ModbusMaster.ReadOnlyBitsAsync(slaveAddress, startAddress, SwapBytes);

        /// <summary>
        /// Asynchronously reads a 16 bit integer (single input register).
        /// </summary>
        /// <param name="slaveAddress">Address of device to read values from.</param>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <returns>16 bit integer.</returns>
        public async Task<short> ReadOnlyShortAsync(byte slaveAddress, ushort startAddress)
            => await ModbusMaster.ReadOnlyShortAsync(slaveAddress, startAddress, SwapBytes);

        /// <summary>
        /// Asynchronously reads a single unsigned 16 bit integer (single input register).
        /// </summary>
        /// <param name="slaveAddress">Address of device to read values from.</param>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <returns>Unsigned 16 bit integer.</returns>
        public async Task<ushort> ReadOnlyUShortAsync(byte slaveAddress, ushort startAddress)
            => await ModbusMaster.ReadOnlyUShortAsync(slaveAddress, startAddress, SwapBytes);

        /// <summary>
        /// Asynchronously reads an 32 bit integer (two input registers).
        /// </summary>
        /// <param name="slaveAddress">Address of device to read values from.</param>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <returns>32 bit integer.</returns>
        public async Task<Int32> ReadOnlyInt32Async(byte slaveAddress, ushort startAddress)
            => await ModbusMaster.ReadOnlyInt32Async(slaveAddress, startAddress, SwapBytes, SwapWords);

        /// <summary>
        /// Asynchronously reads a single unsigned 32 bit integer (two input registers).
        /// </summary>
        /// <param name="slaveAddress">Address of device to read values from.</param>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <returns>Unsigned 32 bit integer.</returns>
        public async Task<uint> ReadOnlyUInt32Async(byte slaveAddress, ushort startAddress)
            => await ModbusMaster.ReadOnlyUInt32Async(slaveAddress, startAddress, SwapBytes, SwapWords);

        /// <summary>
        /// Asynchronously reads a single float value (two input registers).
        /// </summary>
        /// <param name="slaveAddress">Address of device to read values from.</param>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <returns>Float value.</returns>
        public async Task<float> ReadOnlyFloatAsync(byte slaveAddress, ushort startAddress)
            => await ModbusMaster.ReadOnlyFloatAsync(slaveAddress, startAddress, SwapBytes, SwapWords);

        /// <summary>
        /// Asynchronously reads a single double value (four input registers).
        /// </summary>
        /// <param name="slaveAddress">Address of device to read values from.</param>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <returns>Double value.</returns>
        public async Task<double> ReadOnlyDoubleAsync(byte slaveAddress, ushort startAddress)
            => await ModbusMaster.ReadOnlyDoubleAsync(slaveAddress, startAddress, SwapBytes, SwapWords);

        /// <summary>
        /// Asynchronously reads a 64 bit integer (four input registers).
        /// </summary>
        /// <param name="slaveAddress">Address of device to read values from.</param>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <returns>64 bit integer.</returns>
        public async Task<long> ReadOnlyLongAsync(byte slaveAddress, ushort startAddress)
            => await ModbusMaster.ReadOnlyLongAsync(slaveAddress, startAddress, SwapBytes, SwapWords);

        /// <summary>
        /// Asynchronously reads an unsigned 64 bit integer (four input registers).
        /// </summary>
        /// <param name="slaveAddress">Address of device to read values from.</param>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <returns>Unsigned 64 bit integer.</returns>
        public async Task<ulong> ReadOnlyULongAsync(byte slaveAddress, ushort startAddress)
            => await ModbusMaster.ReadOnlyULongAsync(slaveAddress, startAddress, SwapBytes, SwapWords);

        /// <summary>
        /// Asynchronously reads an array of boolean values (multiple discrete inputs).
        /// </summary>
        /// <param name="slaveAddress">Address of device to read values from.</param>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <param name="length">Size of array.</param>
        /// <returns>Array of Bool values.</returns>
        public async Task<bool[]> ReadOnlyBoolArrayAsync(byte slaveAddress, ushort startAddress, ushort length)
            => await ModbusMaster.ReadOnlyBoolArrayAsync(slaveAddress, startAddress, length);

        /// <summary>
        /// Asynchronously reads 8 bit values (multiple input register).
        /// </summary>
        /// <param name="slaveAddress">Address of device to read values from.</param>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <param name="length">Size of array.</param>
        /// <returns>Arroy of bytes.</returns>
        public async Task<byte[]> ReadOnlyBytesAsync(byte slaveAddress, ushort startAddress, ushort length)
            => await ModbusMaster.ReadOnlyBytesAsync(slaveAddress, startAddress, length, SwapBytes);

        /// <summary>
        /// Asynchronously reads an array of 16 bit integers (multiple input registers).
        /// </summary>
        /// <param name="slaveAddress">Address of device to read values from.</param>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <param name="length">Size of array.</param>
        /// <returns>Array of 16 bit integers.</returns>
        public async Task<short[]> ReadOnlyShortArrayAsync(byte slaveAddress, ushort startAddress, ushort length)
            => await ModbusMaster.ReadOnlyShortArrayAsync(slaveAddress, startAddress, length, SwapBytes);

        /// <summary>
        /// Asynchronously reads an array of unsigned 16 bit integer (multiple input registers).
        /// </summary>
        /// <param name="slaveAddress">Address of device to read values from.</param>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <param name="length">Size of array.</param>
        /// <returns>Array of unsigned 16 bit integer.</returns>
        public async Task<ushort[]> ReadOnlyUShortArrayAsync(byte slaveAddress, ushort startAddress, ushort length)
            => await ModbusMaster.ReadOnlyUShortArrayAsync(slaveAddress, startAddress, length, SwapBytes);

        /// <summary>
        /// Asynchronously reads an array of 32 bit integers (multiple input registers).
        /// </summary>
        /// <param name="slaveAddress">Address of device to read values from.</param>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <param name="length">Size of array.</param>
        /// <returns>Array of 32 bit integers.</returns>
        public async Task<Int32[]> ReadOnlyInt32ArrayAsync(byte slaveAddress, ushort startAddress, ushort length)
            => await ModbusMaster.ReadOnlyInt32ArrayAsync(slaveAddress, startAddress, length, SwapBytes, SwapWords);

        /// <summary>
        /// Asynchronously reads an array of unsigned 32 bit integers (multiple input registers).
        /// </summary>
        /// <param name="slaveAddress">Address of device to read values from.</param>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <param name="length">Size of array.</param>
        /// <returns>Array of unsigned 32 bit integers.</returns>
        public async Task<UInt32[]> ReadOnlyUInt32ArrayAsync(byte slaveAddress, ushort startAddress, ushort length)
            => await ModbusMaster.ReadOnlyUInt32ArrayAsync(slaveAddress, startAddress, length, SwapBytes, SwapWords);

        /// <summary>
        /// Asynchronously reads an array of 32 bit IEEE floating point numbers (multiple input registers).
        /// </summary>
        /// <param name="slaveAddress">Address of device to read values from.</param>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <param name="length">Size of array.</param>
        /// <returns>Array of 32 bit IEEE floating point numbers.</returns>
        public async Task<float[]> ReadOnlyFloatArrayAsync(byte slaveAddress, ushort startAddress, ushort length)
            => await ModbusMaster.ReadOnlyFloatArrayAsync(slaveAddress, startAddress, length, SwapBytes, SwapWords);

        /// <summary>
        /// Asynchronously reads an array of 64 bit IEEE floating point numbers (multiple input registers).
        /// </summary>
        /// <param name="slaveAddress">Address of device to read values from.</param>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <param name="length">Size of array.</param>
        /// <returns>Array of 64 bit IEEE floating point numbers.</returns>
        public async Task<double[]> ReadOnlyDoubleArrayAsync(byte slaveAddress, ushort startAddress, ushort length)
            => await ModbusMaster.ReadOnlyDoubleArrayAsync(slaveAddress, startAddress, length, SwapBytes, SwapWords);

        /// <summary>
        /// Asynchronously reads an array of 64 bit integers (multiple input registers).
        /// </summary>
        /// <param name="slaveAddress">Address of device to read values from.</param>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <param name="length">Size of array.</param>
        /// <returns>Array of 64 bit integers.</returns>
        public async Task<long[]> ReadOnlyLongArrayAsync(byte slaveAddress, ushort startAddress, ushort length)
            => await ModbusMaster.ReadOnlyLongArrayAsync(slaveAddress, startAddress, length, SwapBytes, SwapWords);

        /// <summary>
        /// Asynchronously reads an array of unsigned 64 bit integers (multiple input registers).
        /// </summary>
        /// <param name="slaveAddress">Address of device to read values from.</param>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <param name="length">Size of array.</param>
        /// <returns>Array of unsigned 64 bit integers.</returns>
        public async Task<ulong[]> ReadOnlyULongArrayAsync(byte slaveAddress, ushort startAddress, ushort length)
            => await ModbusMaster.ReadOnlyULongArrayAsync(slaveAddress, startAddress, length, SwapBytes, SwapWords);

        #endregion Extended Async Read Only Functions

        #endregion Modbus Functions (slave id)
    }
}