namespace ModbusApp.Commands
{
    #region Using Directives

    using System.Collections;
    using System.Collections.Generic;
    using System.CommandLine;
    using System.CommandLine.IO;
    using System.Text.Json;

    using ModbusLib;
    using NModbus.Extensions;

    #endregion

    public static class CommandHelper
    {
        /// <summary>
        ///  Helper function reading coils.
        /// </summary>
        /// <param name="console"></param>
        /// <param name="client"></param>
        /// <param name="slave"></param>
        /// <param name="number"></param>
        /// <param name="offset"></param>
        /// <param name="header"></param>
        public static void ReadingCoils(IConsole console,
                                        IModbusClient client,
                                        byte slave,
                                        ushort number,
                                        ushort offset,
                                        bool header = true)
        {
            if (number == 1)
            {
                if (header) console.Out.WriteLine($"Reading a single coil[{offset}]");
                bool[] values = client.ReadCoils(slave, offset, number);
                console.Out.WriteLine($"Value of coil[{offset}] = {values[0]}");
            }
            else
            {
                if (header) console.Out.WriteLine($"Reading {number} coils starting at {offset}");
                bool[] values = client.ReadCoils(slave, offset, number);

                for (int index = 0; index<values.Length; ++index)
                {
                    console.Out.WriteLine($"Value of coil[{offset + index}] = {values[index]}");
                }
            }
        }

        /// <summary>
        ///  Helper function reading discrete inputs.
        /// </summary>
        /// <param name="console"></param>
        /// <param name="client"></param>
        /// <param name="slave"></param>
        /// <param name="number"></param>
        /// <param name="offset"></param>
        /// <param name="header"></param>
        public static void ReadingDiscreteInputs(IConsole console,
                                                 IModbusClient client,
                                                 byte slave,
                                                 ushort number,
                                                 ushort offset,
                                                 bool header = true)
        {
            if (number == 1)
            {
                if (header) console.Out.WriteLine($"Reading a single discrete input[{offset}]");
                bool[] values = client.ReadCoils(slave, offset, number);
                console.Out.WriteLine($"Value of discrete input[{offset}] = {values[0]}");
            }
            else
            {
                if (header) console.Out.WriteLine($"Reading {number} discrete inputs starting at {offset}");
                bool[] values = client.ReadCoils(slave, offset, number);

                for (int index = 0; index < values.Length; ++index)
                {
                    console.Out.WriteLine($"Value of discrete input[{offset + index}] = {values[index]}");
                }
            }
        }

        /// <summary>
        ///  Helper function reading holding registers.
        /// </summary>
        /// <param name="console"></param>
        /// <param name="client"></param>
        /// <param name="slave"></param>
        /// <param name="number"></param>
        /// <param name="offset"></param>
        /// <param name="type"></param>
        /// <param name="hex"></param>
        /// <param name="header"></param>
        public static void ReadingHoldingRegisters(IConsole console,
                                                   IModbusClient client,
                                                   byte slave,
                                                   ushort number,
                                                   ushort offset,
                                                   string type,
                                                   bool hex,
                                                   bool header = true)
        {
            if (!string.IsNullOrEmpty(type))
            {
                switch (type.ToLowerInvariant())
                {
                    case "string":
                        {
                            if (hex)
                            {
                                if (header) console.Out.WriteLine($"Reading a HEX string from offset = {offset}");
                                string value = client.ReadHexString(slave, offset, number);
                                console.Out.WriteLine($"Value of HEX string = {value}");
                            }
                            else
                            {
                                if (header) console.Out.WriteLine($"Reading an ASCII string from offset = {offset}");
                                string value = client.ReadString(slave, offset, number);
                                console.Out.WriteLine($"Value of ASCII string = {value}");
                            }

                            break;
                        }
                    case "bits":
                        {
                            if (header) console.Out.WriteLine($"Reading a 16 bit array from offset = {offset}");
                            BitArray value = client.ReadBits(slave, offset);
                            console.Out.WriteLine($"Value of 16 bit array = {value.ToDigitString()}");
                            break;
                        }
                    case "byte":
                        {
                            if (number == 1)
                            {
                                if (header) console.Out.WriteLine($"Reading a single byte from offset = {offset}");
                                byte[] values = client.ReadBytes(slave, offset, number);
                                console.Out.WriteLine($"Value of single byte = {values[0]}");
                            }
                            else
                            {
                                if (header) console.Out.WriteLine($"Reading {number} bytes from offset = {offset}");
                                byte[] values = client.ReadBytes(slave, offset, number);

                                for (int index = 0; index < values.Length; ++index)
                                {
                                    console.Out.WriteLine($"Value of byte array[{index}] = {values[index]}");
                                }
                            }

                            break;
                        }
                    case "short":
                        {
                            if (number == 1)
                            {
                                if (header) console.Out.WriteLine($"Reading a single short from offset = {offset}");
                                short value = client.ReadShort(slave, offset);
                                console.Out.WriteLine($"Value of single short = {value}");
                            }
                            else
                            {
                                if (header) console.Out.WriteLine($"Reading {number} shorts from offset = {offset}");
                                short[] values = client.ReadShortArray(slave, offset, number);

                                for (int index = 0; index < values.Length; ++index)
                                {
                                    console.Out.WriteLine($"Value of short array[{index}] = {values[index]}");
                                }
                            }

                            break;
                        }
                    case "ushort":
                        {
                            if (number == 1)
                            {
                                if (header) console.Out.WriteLine($"Reading a single ushort from offset = {offset}");
                                ushort value = client.ReadUShort(slave, offset);
                                console.Out.WriteLine($"Value of single ushort = {value}");
                            }
                            else
                            {
                                if (header) console.Out.WriteLine($"Reading {number} ushorts from offset = {offset}");
                                ushort[] values = client.ReadUShortArray(slave, offset, number);

                                for (int index = 0; index < values.Length; ++index)
                                {
                                    console.Out.WriteLine($"Value of ushort array[{index}] = {values[index]}");
                                }
                            }

                            break;
                        }
                    case "int":
                        {
                            if (number == 1)
                            {
                                if (header) console.Out.WriteLine($"Reading a single integer from offset = {offset}");
                                int value = client.ReadInt32(slave, offset);
                                console.Out.WriteLine($"Value of single integer = {value}");
                            }
                            else
                            {
                                if (header) console.Out.WriteLine($"Reading {number}  integers from offset = {offset}");
                                int[] values = client.ReadInt32Array(slave, offset, number);

                                for (int index = 0; index < values.Length; ++index)
                                {
                                    console.Out.WriteLine($"Value of integer array[{index}] = {values[index]}");
                                }
                            }

                            break;
                        }
                    case "uint":
                        {
                            if (number == 1)
                            {
                                if (header) console.Out.WriteLine($"Reading a single unsigned integer from offset = {offset}");
                                uint value = client.ReadUInt32(slave, offset);
                                console.Out.WriteLine($"Value of single unsigned integer = {value}");
                            }
                            else
                            {
                                if (header) console.Out.WriteLine($"Reading {number} unsigned integers from offset = {offset}");
                                uint[] values = client.ReadUInt32Array(slave, offset, number);

                                for (int index = 0; index < values.Length; ++index)
                                {
                                    console.Out.WriteLine($"Value of unsigned integer array[{index}] = {values[index]}");
                                }
                            }

                            break;
                        }
                    case "float":
                        {
                            if (number == 1)
                            {
                                if (header) console.Out.WriteLine($"Reading a single float from offset = {offset}");
                                float value = client.ReadFloat(slave, offset);
                                console.Out.WriteLine($"Value of single float = {value}");
                            }
                            else
                            {
                                if (header) console.Out.WriteLine($"Reading {number} floats from offset = {offset}");
                                float[] values = client.ReadFloatArray(slave, offset, number);

                                for (int index = 0; index < values.Length; ++index)
                                {
                                    console.Out.WriteLine($"Value of float array[{index}] = {values[index]}");
                                }
                            }

                            break;
                        }
                    case "double":
                        {
                            if (number == 1)
                            {
                                if (header) console.Out.WriteLine($"Reading a single double from offset = {offset}");
                                double value = client.ReadDouble(slave, offset);
                                console.Out.WriteLine($"Value of single double = {value}");
                            }
                            else
                            {
                                if (header) console.Out.WriteLine($"Reading {number} doubles from offset = {offset}");
                                double[] values = client.ReadDoubleArray(slave, offset, number);

                                for (int index = 0; index < values.Length; ++index)
                                {
                                    console.Out.WriteLine($"Value of double array[{index}] = {values[index]}");
                                }
                            }

                            break;
                        }
                    case "long":
                        {
                            if (number == 1)
                            {
                                if (header) console.Out.WriteLine($"Reading a single long from offset = {offset}");
                                long value = client.ReadLong(slave, offset);
                                console.Out.WriteLine($"Value of single long = {value}");
                            }
                            else
                            {
                                if (header) console.Out.WriteLine($"Reading {number} longs from offset = {offset}");
                                long[] values = client.ReadLongArray(slave, offset, number);

                                for (int index = 0; index < values.Length; ++index)
                                {
                                    console.Out.WriteLine($"Value of long array[{index}] = {values[index]}");
                                }
                            }

                            break;
                        }
                    case "ulong":
                        {
                            if (number == 1)
                            {
                                if (header) console.Out.WriteLine($"Reading a single ulong from offset = {offset}");
                                ulong value = client.ReadULong(slave, offset);
                                console.Out.WriteLine($"Value of single ulong = {value}");
                            }
                            else
                            {
                                if (header) console.Out.WriteLine($"Reading {number} ulongs from offset = {offset}");
                                ulong[] values = client.ReadULongArray(slave, offset, number);

                                for (int index = 0; index < values.Length; ++index)
                                {
                                    console.Out.WriteLine($"Value of ulong array[{index}] = {values[index]}");
                                }
                            }
                            break;
                        }
                }
            }
            else
            {
                if (number == 1)
                {
                    if (header) console.Out.WriteLine($"Reading a holding register[{offset}]");
                    ushort[] values = client.ReadHoldingRegisters(slave, offset, number);
                    if (hex) console.Out.WriteLine($"Value of holding register[{offset}] = {values[0]:X04}");
                    else console.Out.WriteLine($"Value of holding register[{offset}] = {values[0]}");
                }
                else
                {
                    if (header) console.Out.WriteLine($"Reading {number} holding registers starting at {offset}");
                    ushort[] values = client.ReadHoldingRegisters(slave, offset, number);

                    for (int index = 0; index < values.Length; ++index)
                    {
                        if (hex) console.Out.WriteLine($"Value of holding register[{offset + index}] = {values[index]:X04}");
                        else console.Out.WriteLine($"Value of holding register[{offset + index}] = {values[index]}");
                    }
                }
            }
        }

        /// <summary>
        ///  Helper function reading input registers.
        /// </summary>
        /// <param name="console"></param>
        /// <param name="client"></param>
        /// <param name="slave"></param>
        /// <param name="number"></param>
        /// <param name="offset"></param>
        /// <param name="type"></param>
        /// <param name="hex"></param>
        /// <param name="header"></param>
        public static void ReadingInputRegisters(IConsole console,
                                                 IModbusClient client,
                                                 byte slave,
                                                 ushort number,
                                                 ushort offset,
                                                 string type,
                                                 bool hex,
                                                 bool header = true)
        {
            if (!string.IsNullOrEmpty(type))
            {
                switch (type.ToLowerInvariant())
                {
                    case "string":
                        {
                            if (hex)
                            {
                                if (header) console.Out.WriteLine($"Reading a HEX string from offset = {offset}");
                                string value = client.ReadOnlyHexString(slave, offset, number);
                                console.Out.WriteLine($"Value of HEX string = {value}");
                            }
                            else
                            {
                                if (header) console.Out.WriteLine($"Reading an ASCII string from offset = {offset}");
                                string value = client.ReadOnlyString(slave, offset, number);
                                console.Out.WriteLine($"Value of ASCII string = {value}");
                            }

                            break;
                        }
                    case "bits":
                        {
                            if (header) console.Out.WriteLine($"Reading a 16 bit array from offset = {offset}");
                            BitArray value = client.ReadOnlyBits(slave, offset);
                            console.Out.WriteLine($"Value of 16 bit array = {value.ToDigitString()}");

                            break;
                        }
                    case "byte":
                        {
                            if (number == 1)
                            {
                                if (header) console.Out.WriteLine($"Reading a single byte from offset = {offset}");
                                byte[] values = client.ReadOnlyBytes(slave, offset, number);
                                console.Out.WriteLine($"Value of single byte = {values[0]}");
                            }
                            else
                            {
                                if (header) console.Out.WriteLine($"Reading {number} bytes from offset = {offset}");
                                byte[] values = client.ReadOnlyBytes(slave, offset, number);

                                for (int index = 0; index < values.Length; ++index)
                                {
                                    console.Out.WriteLine($"Value of byte array[{index}] = {values[index]}");
                                }
                            }

                            break;
                        }
                    case "short":
                        {
                            if (number == 1)
                            {
                                if (header) console.Out.WriteLine($"Reading a single short from offset = {offset}");
                                short value = client.ReadOnlyShort(slave, offset);
                                console.Out.WriteLine($"Value of single short = {value}");
                            }
                            else
                            {
                                if (header) console.Out.WriteLine($"Reading {number} short values from offset = {offset}");
                                short[] values = client.ReadOnlyShortArray(slave, offset, number);

                                for (int index = 0; index < values.Length; ++index)
                                {
                                    console.Out.WriteLine($"Value of short array[{index}] = {values[index]}");
                                }
                            }

                            break;
                        }
                    case "ushort":
                        {
                            if (number == 1)
                            {
                                if (header) console.Out.WriteLine($"Reading a single ushort from offset = {offset}");
                                ushort value = client.ReadOnlyUShort(slave, offset);
                                console.Out.WriteLine($"Value of single ushort = {value}");
                            }
                            else
                            {
                                if (header) console.Out.WriteLine($"Reading {number} ushort values from offset = {offset}");
                                ushort[] values = client.ReadOnlyUShortArray(slave, offset, number);

                                for (int index = 0; index < values.Length; ++index)
                                {
                                    console.Out.WriteLine($"Value of ushort array[{index}] = {values[index]}");
                                }
                            }

                            break;
                        }
                    case "int":
                        {
                            if (number == 1)
                            {
                                if (header) console.Out.WriteLine($"Reading a single int from offset = {offset}");
                                int value = client.ReadOnlyInt32(slave, offset);
                                console.Out.WriteLine($"Value of single integer = {value}");
                            }
                            else
                            {
                                if (header) console.Out.WriteLine($"Reading {number} int values from offset = {offset}");
                                int[] values = client.ReadOnlyInt32Array(slave, offset, number);

                                for (int index = 0; index < values.Length; ++index)
                                {
                                    console.Out.WriteLine($"Value of int array[{index}] = {values[index]}");
                                }
                            }

                            break;
                        }
                    case "uint":
                        {
                            if (number == 1)
                            {
                                if (header) console.Out.WriteLine($"Reading a single unsigned int from offset = {offset}");
                                uint value = client.ReadOnlyUInt32(slave, offset);
                                console.Out.WriteLine($"Value of single unsigned int = {value}");
                            }
                            else
                            {
                                if (header) console.Out.WriteLine($"Reading {number} unsigned int values from offset = {offset}");
                                uint[] values = client.ReadOnlyUInt32Array(slave, offset, number);

                                for (int index = 0; index < values.Length; ++index)
                                {
                                    console.Out.WriteLine($"Value of unsigned int array[{index}] = {values[index]}");
                                }
                            }

                            break;
                        }
                    case "float":
                        {
                            if (number == 1)
                            {
                                if (header) console.Out.WriteLine($"Reading a single float from offset = {offset}");
                                float value = client.ReadOnlyFloat(slave, offset);
                                console.Out.WriteLine($"Value of single float = {value}");
                            }
                            else
                            {
                                if (header) console.Out.WriteLine($"Reading {number} float values from offset = {offset}");
                                float[] values = client.ReadOnlyFloatArray(slave, offset, number);

                                for (int index = 0; index < values.Length; ++index)
                                {
                                    console.Out.WriteLine($"Value of float array[{index}] = {values[index]}");
                                }
                            }

                            break;
                        }
                    case "double":
                        {
                            if (number == 1)
                            {
                                if (header) console.Out.WriteLine($"Reading a single double from offset = {offset}");
                                double value = client.ReadOnlyDouble(slave, offset);
                                console.Out.WriteLine($"Value of single double = {value}");
                            }
                            else
                            {
                                if (header) console.Out.WriteLine($"Reading {number} double values from offset = {offset}");
                                double[] values = client.ReadOnlyDoubleArray(slave, offset, number);

                                for (int index = 0; index < values.Length; ++index)
                                {
                                    console.Out.WriteLine($"Value of double array[{index}] = {values[index]}");
                                }
                            }

                            break;
                        }
                    case "long":
                        {
                            if (number == 1)
                            {
                                if (header) console.Out.WriteLine($"Reading a single long from offset = {offset}");
                                long value = client.ReadOnlyLong(slave, offset);
                                console.Out.WriteLine($"Value of single long = {value}");
                            }
                            else
                            {
                                if (header) console.Out.WriteLine($"Reading {number} long values from offset = {offset}");
                                long[] values = client.ReadOnlyLongArray(slave, offset, number);

                                for (int index = 0; index < values.Length; ++index)
                                {
                                    console.Out.WriteLine($"Value of long array[{index}] = {values[index]}");
                                }
                            }

                            break;
                        }
                    case "ulong":
                        {
                            if (number == 1)
                            {
                                if (header) console.Out.WriteLine($"Reading a single unsigned long from offset = {offset}");
                                ulong value = client.ReadOnlyULong(slave, offset);
                                console.Out.WriteLine($"Value of single ulong = {value}");
                            }
                            else
                            {
                                if (header) console.Out.WriteLine($"Reading {number} unsigned long values from offset = {offset}");
                                ulong[] values = client.ReadOnlyULongArray(slave, offset, number);

                                for (int index = 0; index < values.Length; ++index)
                                {
                                    console.Out.WriteLine($"Value of ulong array[{index}] = {values[index]}");
                                }
                            }

                            break;
                        }
                }
            }
            else
            {
                if (number == 1)
                {
                    if (header) console.Out.WriteLine($"Reading a input register[{offset}]");
                    ushort[] values = client.ReadInputRegisters(slave, offset, number);
                    if (hex) console.Out.WriteLine($"Value of input register[{offset}] = {values[0]:X2}");
                    else console.Out.WriteLine($"Value of input register[{offset}] = {values[0]}");
                }
                else
                {
                    if (header) console.Out.WriteLine($"Reading {number} input registers starting at {offset}");
                    ushort[] values = client.ReadInputRegisters(slave, offset, number);

                    for (int index = 0; index < values.Length; ++index)
                    {
                        if (hex) console.Out.WriteLine($"Value of input register[{offset + index}] = {values[index]:X2}");
                        else console.Out.WriteLine($"Value of input register[{offset + index}] = {values[index]}");
                    }
                }
            }
        }

        /// <summary>
        ///  Helper function writing coils.
        /// </summary>
        /// <param name="console"></param>
        /// <param name="client"></param>
        /// <param name="slave"></param>
        /// <param name="offset"></param>
        /// <param name="data"></param>
        public static void WritingCoils(IConsole console,
                                        IModbusClient client,
                                        byte slave,
                                        ushort offset,
                                        string data)
        {
            // Writing datas.
            if (!string.IsNullOrEmpty(data))
            {
                List<bool>? values = JsonSerializer.Deserialize<List<bool>>(data);

                if (!(values is null))
                {
                    if (values.Count == 1)
                    {
                        console.Out.WriteLine($"Write single coil[{offset}] = {values[0]}");
                        client.WriteSingleCoil(slave, offset, values[0]);
                    }
                    else
                    {
                        console.Out.WriteLine($"Writing {values.Count} coils starting at {offset}");

                        for (int index = 0; index < values.Count; ++index)
                            console.Out.WriteLine($"Value of coil[{offset + index}] = {values[index]}");

                        client.WriteMultipleCoils(slave, offset, values.ToArray());
                    }
                }
            }
        }

        /// <summary>
        ///  Helper function writing holding registers.
        /// </summary>
        /// <param name="console"></param>
        /// <param name="client"></param>
        /// <param name="slave"></param>
        /// <param name="offset"></param>
        /// <param name="data"></param>
        /// <param name="type"></param>
        /// <param name="hex"></param>
        public static void WritingHoldingRegisters(IConsole console,
                                                   IModbusClient client,
                                                   byte slave,
                                                   ushort offset,
                                                   string data,
                                                   string type,
                                                   bool hex)
        {
            if (!string.IsNullOrEmpty(data))
            {
                if (!string.IsNullOrEmpty(type))
                {
                    switch (type.ToLowerInvariant())
                    {
                        case "string":
                            {
                                if (hex)
                                {
                                    console.Out.WriteLine($"Writing a HEX string at offset = {offset}");
                                    client.WriteHexString(slave, offset, data);
                                }
                                else
                                {
                                    console.Out.WriteLine($"Writing an ASCII string at offset = {offset}");
                                    client.WriteString(slave, offset, data);
                                }
                                break;
                            }
                        case "bits":
                            {
                                console.Out.WriteLine($"Writing a 16 bit array at offset = {offset}");
                                client.WriteBits(slave, offset, data.ToBitArray());
                                break;
                            }
                        case "byte":
                            {
                                List<byte>? bytes = JsonSerializer.Deserialize<List<byte>>(data);

                                if (!(bytes is null))
                                {
                                    console.Out.WriteLine($"Writing {bytes.Count} bytes at offset = {offset}");
                                    client.WriteBytes(slave, offset, bytes.ToArray());
                                }

                                break;
                            }
                        case "short":
                            {
                                List<short>? values = JsonSerializer.Deserialize<List<short>>(data);

                                if (!(values is null))
                                {
                                    if (values.Count == 1)
                                    {
                                        console.Out.WriteLine($"Writing a single short value at offset = {offset}");
                                        client.WriteShort(slave, offset, values[0]);
                                    }
                                    else
                                    {
                                        console.Out.WriteLine($"Writing {values.Count} short values at offset = {offset}");
                                        client.WriteShortArray(slave, offset, values.ToArray());
                                    }
                                }

                                break;
                            }
                        case "ushort":
                            {
                                List<ushort>? values = JsonSerializer.Deserialize<List<ushort>>(data);

                                if (!(values is null))
                                {
                                    if (values.Count == 1)
                                    {
                                        console.Out.WriteLine($"Writing a single unsigned short value at offset = {offset}");
                                        client.WriteUShort(slave, offset, values[0]);
                                    }
                                    else
                                    {
                                        console.Out.WriteLine($"Writing {values.Count} unsigned short values at offset = {offset}");
                                        client.WriteUShortArray(slave, offset, values.ToArray());
                                    }
                                }

                                break;
                            }
                        case "int":
                            {
                                List<int>? values = JsonSerializer.Deserialize<List<int>>(data);

                                if (!(values is null))
                                {
                                    if (values.Count == 1)
                                    {
                                        console.Out.WriteLine($"Writing a single int value at offset = {offset}");
                                        client.WriteInt32(slave, offset, values[0]);
                                    }
                                    else
                                    {
                                        console.Out.WriteLine($"Writing {values.Count} int values at offset = {offset}");
                                        client.WriteInt32Array(slave, offset, values.ToArray());
                                    }
                                }

                                break;
                            }
                        case "uint":
                            {
                                List<uint>? values = JsonSerializer.Deserialize<List<uint>>(data);

                                if (!(values is null))
                                {
                                    if (values.Count == 1)
                                    {
                                        console.Out.WriteLine($"Writing a single unsigned int value at offset = {offset}");
                                        client.WriteUInt32(slave, offset, values[0]);
                                    }
                                    else
                                    {
                                        console.Out.WriteLine($"Writing {values.Count} unsigned int values at offset = {offset}");
                                        client.WriteUInt32Array(slave, offset, values.ToArray());
                                    }
                                }

                                break;
                            }
                        case "float":
                            {
                                List<float>? values = JsonSerializer.Deserialize<List<float>>(data);

                                if (!(values is null))
                                {
                                    if (values.Count == 1)
                                    {
                                        console.Out.WriteLine($"Writing a single float value at offset = {offset}");
                                        client.WriteFloat(slave, offset, values[0]);
                                    }
                                    else
                                    {
                                        console.Out.WriteLine($"Writing {values.Count} float values at offset = {offset}");
                                        client.WriteFloatArray(slave, offset, values.ToArray());
                                    }
                                }

                                break;
                            }
                        case "double":
                            {
                                List<double>? values = JsonSerializer.Deserialize<List<double>>(data);

                                if (!(values is null))
                                {
                                    if (values.Count == 1)
                                    {
                                        console.Out.WriteLine($"Writing a single double value at offset = {offset}");
                                        client.WriteDouble(slave, offset, values[0]);
                                    }
                                    else
                                    {
                                        console.Out.WriteLine($"Writing {values.Count} double values at offset = {offset}");
                                        client.WriteDoubleArray(slave, offset, values.ToArray());
                                    }
                                }

                                break;
                            }
                        case "long":
                            {
                                List<long>? values = JsonSerializer.Deserialize<List<long>>(data);

                                if (!(values is null))
                                {
                                    if (values.Count == 1)
                                    {
                                        console.Out.WriteLine($"Writing a single long value at offset = {offset}");
                                        client.WriteLong(slave, offset, values[0]);
                                    }
                                    else
                                    {
                                        console.Out.WriteLine($"Writing {values.Count} long values at offset = {offset}");
                                        client.WriteLongArray(slave, offset, values.ToArray());
                                    }
                                }

                                break;
                            }
                        case "ulong":
                            {
                                List<ulong>? values = JsonSerializer.Deserialize<List<ulong>>(data);

                                if (!(values is null))
                                {
                                    if (values.Count == 1)
                                    {
                                        console.Out.WriteLine($"Writing a single unsigned long value at offset = {offset}");
                                        client.WriteULong(slave, offset, values[0]);
                                    }
                                    else
                                    {
                                        console.Out.WriteLine($"Writing {values.Count} unsigned long values at offset = {offset}");
                                        client.WriteULongArray(slave, offset, values.ToArray());
                                    }
                                }

                                break;
                            }
                    }
                }
                else
                {
                    List<ushort>? values = JsonSerializer.Deserialize<List<ushort>>(data);

                    if (!(values is null))
                    {
                        if (values.Count == 1)
                        {
                            console.Out.WriteLine($"Writing single holding register[{offset}] = {values[0]}");
                            client.WriteSingleRegister(slave, offset, values[0]);
                        }
                        else
                        {
                            console.Out.WriteLine($"Writing {values.Count} holding registers starting at {offset}");

                            for (int index = 0; index < values.Count; ++index)
                                console.Out.WriteLine($"Value of holding register[{offset + index}] = {values[index]}");

                            client.WriteMultipleRegisters(slave, offset, values.ToArray());
                        }
                    }
                }
            }
        }
    }
}