// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NullMaster.cs" company="DTV-Online">
//   Copyright(c) 2020 Dr. Peter Trimmel. All rights reserved.
// </copyright>
// <license>
//   Licensed under the MIT license. See the LICENSE file in the project root for more information.
// </license>
// <created>13-5-2020 13:52</created>
// <author>Peter Trimmel</author>
// --------------------------------------------------------------------------------------------------------------------
namespace ModbusLib.Models
{
    using System;
    using System.Threading.Tasks;

    using NModbus;
    using NModbus.IO;

    public class NullMaster : IModbusMaster
    {
        private class NullModbusMessage : IModbusMessage
        {
            public byte FunctionCode { get; set; }
            public byte SlaveAddress { get; set; }
            public byte[] MessageFrame { get; } = Array.Empty<byte>();
            public byte[] ProtocolDataUnit { get; } = Array.Empty<byte>();
            public ushort TransactionId { get; set; }

            public void Initialize(byte[] frame) { }
        }

        private class NullStreamResource : IStreamResource
        {
            public int InfiniteTimeout { get; }
            public int ReadTimeout { get; set; }
            public int WriteTimeout { get; set; }

            public void DiscardInBuffer() { }

            public int Read(byte[] buffer, int offset, int count) { return 0; }

            public void Write(byte[] buffer, int offset, int count) { }

            public void Dispose() { }
        }

        private class NullTransport : IModbusTransport
        {
            public int Retries { get; set; }
            public uint RetryOnOldResponseThreshold { get; set; }
            public bool SlaveBusyUsesRetryCount { get; set; }
            public int WaitToRetryMilliseconds { get; set; }
            public int ReadTimeout { get; set; }
            public int WriteTimeout { get; set; }
            public IStreamResource StreamResource { get; } = new NullStreamResource();

            public byte[] BuildMessageFrame(IModbusMessage message) { return Array.Empty<byte>(); }

            public byte[] ReadRequest() { return Array.Empty<byte>(); }

            public T UnicastMessage<T>(IModbusMessage message) where T : IModbusMessage, new() { return new T(); }

            public void Write(IModbusMessage message) { }

            public void Dispose() { }
        }

        private static readonly NullMaster _master = new NullMaster();

        public static IModbusMaster CreateModbusMaster() { return _master; }

        public IModbusTransport Transport { get; } = new NullTransport();

        public TResponse ExecuteCustomMessage<TResponse>(IModbusMessage request) where TResponse : IModbusMessage, new() { return (TResponse)request; }

        public bool[] ReadCoils(byte slaveAddress, ushort startAddress, ushort numberOfPoints) { return Array.Empty<bool>(); }

        public Task<bool[]> ReadCoilsAsync(byte slaveAddress, ushort startAddress, ushort numberOfPoints) { return Task.FromResult(Array.Empty<bool>()); }

        public ushort[] ReadHoldingRegisters(byte slaveAddress, ushort startAddress, ushort numberOfPoints) { return Array.Empty<ushort>(); }

        public Task<ushort[]> ReadHoldingRegistersAsync(byte slaveAddress, ushort startAddress, ushort numberOfPoints) { return Task.FromResult(Array.Empty<ushort>()); }

        public ushort[] ReadInputRegisters(byte slaveAddress, ushort startAddress, ushort numberOfPoints) { return Array.Empty<ushort>(); }

        public Task<ushort[]> ReadInputRegistersAsync(byte slaveAddress, ushort startAddress, ushort numberOfPoints) { return Task.FromResult(Array.Empty<ushort>()); }

        public bool[] ReadInputs(byte slaveAddress, ushort startAddress, ushort numberOfPoints) { return Array.Empty<bool>(); }

        public Task<bool[]> ReadInputsAsync(byte slaveAddress, ushort startAddress, ushort numberOfPoints) { return Task.FromResult(Array.Empty<bool>()); }

        public ushort[] ReadWriteMultipleRegisters(byte slaveAddress, ushort startReadAddress, ushort numberOfPointsToRead, ushort startWriteAddress, ushort[] writeData) { return Array.Empty<ushort>(); }

        public Task<ushort[]> ReadWriteMultipleRegistersAsync(byte slaveAddress, ushort startReadAddress, ushort numberOfPointsToRead, ushort startWriteAddress, ushort[] writeData) { return Task.FromResult(Array.Empty<ushort>()); }

        public void WriteMultipleCoils(byte slaveAddress, ushort startAddress, bool[] data) { }

        public Task WriteMultipleCoilsAsync(byte slaveAddress, ushort startAddress, bool[] data) { return Task.CompletedTask; }

        public void WriteMultipleRegisters(byte slaveAddress, ushort startAddress, ushort[] data) { }

        public Task WriteMultipleRegistersAsync(byte slaveAddress, ushort startAddress, ushort[] data) { return Task.CompletedTask; }

        public void WriteSingleCoil(byte slaveAddress, ushort coilAddress, bool value) { }

        public Task WriteSingleCoilAsync(byte slaveAddress, ushort coilAddress, bool value) { return Task.CompletedTask; }

        public void WriteSingleRegister(byte slaveAddress, ushort registerAddress, ushort value) { }

        public Task WriteSingleRegisterAsync(byte slaveAddress, ushort registerAddress, ushort value) { return Task.CompletedTask; }

        void IModbusMaster.WriteFileRecord(byte slaveAdress, ushort fileNumber, ushort startingAddress, byte[] data) { }

        public void Dispose() { Dispose(true); GC.SuppressFinalize(this); }

        protected virtual void Dispose(bool disposing) { if (disposing) { } }
    }
}