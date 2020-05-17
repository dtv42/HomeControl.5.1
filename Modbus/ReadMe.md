# Modbus

The *Modbus* projects are comprised of a library, a console application, a test project, and two web applications.

## ModbusLib

The *Modbus* library provides a set of helper classes to facilitate data access to a Modbus slave.
This library is using the NModbus library (found at GitHub (https://github.com/NModbus/NModbus) and has been recompiled as a .NET Standard 2.0 library.

### NModbus

NModbus is a fork of NModbus4 which itself is a fork of the original NModbus (https://code.google.com/p/nmodbus) and supports serial ASCII, serial RTU, TCP, and UDP protocols.
NModbus is a C# implementation of the Modbus protocol.
Here only the Modbus TCP master is used to communicate with the Modbus TCP slaves.

The APIs used are:

| Function                  | Function Code        |
| ------------------------- | -------------------- |
| Read Discrete Inputs      |     (fc 2)           |
| Read Coils                |     (fc 1)           |
| Write Single Coil         |     (fc 5)           |
| Write Multiple Coils      |     (fc 15)          |
| Read Input Registers      |     (fc 4)           |
| Read Multiple Registers   |     (fc 3)           |
| Write Single Register     |     (fc 6)           |
| Write Multiple Registers  |     (fc 16)          |

#### Modbus Conversion
The additional datatypes supported are:

- BitArray
- Short
- UShort
- Int32
- UInt32
- Float
- Double
- Long
- ULong
- ASCII
- Hex

Conversion routines from and to Modbus registers are provided.

#### NModbus Extensions
Additonal data types (using read/write coils and holding registers) are supported:

| Function          | Description                                                                        |
| ----------------- | ---------------------------------------------------------------------------------- |
| ReadString        | Reads an ASCII string (multiple holding registers)                                 |
| ReadHexString     | Reads an HEX string (multiple holding registers)                                   |
| ReadBool          | Reads a boolean value (single coil)                                                |
| ReadBits          | Reads a 16 bit array (single holding register)                                     |
| ReadShort         | Reads a 16 bit integer (single holding register)                                   |
| ReadUShort        | Reads an unsigned 16 bit integer (single holding register)                         |
| ReadInt32         | Reads a 32 bit integer (two holding registers)                                     |
| ReadUInt32        | Reads an unsigned 32 bit integer (two holding registers)                           |
| ReadFloat         | Reads a 32 bit IEEE floating point number (two holding registers)                  |
| ReadDouble        | Reads a 64 bit IEEE floating point number (four holding registers)                 |
| ReadLong          | Reads a 64 bit integer (four holding registers)                                    |
| ReadULong         | Reads an unsigned 64 bit integer (four holding registers)                          |
| ReadBoolArray     | Reads an array of boolean values (multiple coils)                                  |
| ReadBytes         | Reads 8 bit values (multiple holding register)                                     |
| ReadShortArray    | Reads an array of 16 bit integers (multiple holding register)                      |
| ReadUShortArray   | Reads an array of unsigned 16 bit integer (multiple holding register)              |
| ReadInt32Array    | Reads an array of 32 bit integers (multiple holding registers)                     |
| ReadUInt32Array   | Reads an array of unsigned 32 bit integers (multiple holding registers)            |
| ReadFloatArray    | Reads an array of 32 bit IEEE floating point numbers (multiple holding registers)  |
| ReadDoubleArray   | Reads an array of 64 bit IEEE floating point numbers (multiple holding registers)  |
| ReadLongArray     | Reads an array of 64 bit integers (multiple holding registers)                     |
| ReadULongArray    | Reads an array of unsigned 64 bit integers (multiple holding registers)            |
|                   |                                                                                    |
| WriteString       | Writes an ASCII string (multiple holding registers)                                |
| WriteHexString    | Writes an HEX string (multiple holding registers)                                  |
| WriteBool         | Writes a boolean value (single coil)                                               |
| WriteBits         | Writes a 16 bit array (single holding register)                                    |
| WriteShort        | Writes a 16 bit integer (single holding register)                                  |
| WriteUShort       | Writes an unsigned 16 bit integer (single holding register)                        |
| WriteInt32        | Writes a 32 bit integer (two holding registers)                                    |
| WriteUInt32       | Writes an unsigned 32 bit integer (two holding registers)                          |
| WriteFloat        | Writes a 32 bit IEEE floating point number (two holding registers)                 |
| WriteDouble       | Writes a 64 bit IEEE floating point number (four holding registers)                |
| WriteLong         | Writes a 64 bit integer (four holding registers)                                   |
| WriteULong        | Writes an unsigned 64 bit integer (four holding registers)                         |
| WriteBoolArray    | Writes an array of boolean values (multiple coils)                                 |
| WriteBytes        | Writes 8 bit values (multiple holding register)                                    |
| WriteShortArray   | Writes an array of 16 bit integers (multiple holding register)                     |
| WriteUShortArray  | Writes an array of unsigned 16 bit integer (multiple holding register)             |
| WriteInt32Array   | Writes an array of 32 bit integers (multiple holding registers)                    |
| WriteUInt32Array  | Writes an array of unsigned 32 bit integers (multiple holding registers)           |
| WriteFloatArray   | Writes an array of 32 bit IEEE floating point numbers (multiple holding registers) |
| WriteDoubleArray  | Writes an array of 64 bit IEEE floating point numbers (multiple holding registers) |
| WriteLongArray    | Writes an array of 64 bit integers (multiple holding registers)                    |
| WriteULongArray   | Writes an array of unsigned 64 bit integers (multiple holding registers)           |

Additonal data types (read discrete inputs and input registers) are supported:

| Function            | Description                                                                     |
| ------------------- | ------------------------------------------------------------------------------- |
| ReadOnlyString      | Reads an ASCII string (multiple input registers)                                |
| ReadOnlyHexString   | Reads an HEX string (multiple input registers)                                  |
| ReadOnlyBool        | Reads a boolean value (single discrete input)                                   |
| ReadOnlyBits        | Reads a 16 bit array (single holding register)                                  |
| ReadOnlyShort       | Reads a 16 bit integer (single holding register)                                |
| ReadOnlyUShort      | Reads an unsigned 16 bit integer (single input register)                        |
| ReadOnlyInt32       | Reads a 32 bit integer (two input registers)                                    |
| ReadOnlyUInt32      | Reads an unsigned 32 bit integer (two input registers)                          |
| ReadOnlyFloat       | Reads a 32 bit IEEE floating point number (two input registers)                 |
| ReadOnlyDouble      | Reads a 64 bit IEEE floating point number (four input registers)                |
| ReadOnlyLong        | Reads a 64 bit integer (four input registers)                                   |
| ReadOnlyULong       | Reads an unsigned 64 bit integer (four input registers)                         |
| ReadOnlyBoolArray   | Reads an array of boolean values (multiple discrete inputs)                     |
| ReadOnlyBytes       | Reads 8 bit values (multiple input register)                                    |
| ReadOnlyShortArray  | Reads an array of 16 bit integers (multiple input register)                     |
| ReadOnlyUShortArray | Reads an array of unsigned 16 bit integer (multiple input register)             |
| ReadOnlyInt32Array  | Reads an array of 32 bit integers (multiple input registers)                    |
| ReadOnlyUInt32Array | Reads an array of unsigned 32 bit integers (multiple input registers)           |
| ReadOnlyFloatArray  | Reads an array of 32 bit IEEE floating point numbers (multiple input registers) |
| ReadOnlyDoubleArray | Reads an array of 64 bit IEEE floating point numbers (multiple input registers) |
| ReadOnlyLongArray   | Reads an array of 64 bit integers (multiple input registers)                    |
| ReadOnlyULongArray  | Reads an array of unsigned 64 bit integers (multiple input registers)           |

#### Modbus Attribute
This attribute allows to mark a property with a Modbus specific offset, length, and access mode.

~~~CSHarp
    [Modbus(1)]
    public ushort Value { get; set; }

    [Modbus(1, AccessModes.ReadOnly)]
    public ushort Value { get; set; }

    [Modbus(1, AccessModes.ReadOnly, 4)]
    public long Value { get; set; }

    [Modbus(offset: 1, access: AccessModes.ReadOnly, length: 4)]
    public double Value { get; set; }
~~~

Those attributes can be used to access the actual modbus registers (at the specified offset, using the specified numbers).

## ModbusApp

The *NModbus* console application is used to access Modbus slave devices over ethernet (TCP) or serial lines (RTU).
Several commandline options are provided to facilitate the device access and optionally to override settings from the JSON configuration file ("appsettings.json").
All commands have *-?* and *--help* options to display usage information.

### Root Command

| Option    |                           Description                           |
| --------- | --------------------------------------------------------------- |
| --verbose | Shows the current settings (typically from "appsettings.json")  |
| --config  | Shows the all configuration items (environment, settings, etc.) |

The root command option *--verbose* can also be used in all sub commands.

| Command |                 Description               |
| ------- | ----------------------------------------- |
| rtu     | Supporting standard Modbus RTU operations |
| tcp     | Supporting standard Modbus TCP operations |

### rtu Command

|             Option             |              Description           |
| ------------------------------ | ---------------------------------- |
| --com &lt;string&gt;           | Sets the Modbus master COM port    |
| --baudrate &lt;number&gt;      | Sets the Modbus COM port baud rate |
| --parity &lt;string&gt;        | Sets the Modbus COM port parity    |
| --databits &lt;number&gt;      | Sets the Modbus COM port data bits |
| --stopbits &lt;string&gt;      | Sets the Modbus COM port stop bits |
| --slaveid &lt;number&gt;       | Sets the Modbus slave ID           |
| --read-timeout &lt;number&gt;  | Sets the timeout in msec (hidden)  |
| --write-timeout &lt;number&gt; | Sets the timeout in msec (hidden)  |

The *rtu* command options can also be used in the following sub commands.

| Command |                   Description                     |
| ------- | ------------------------------------------------- |
| read    | Supporting standard Modbus RTU read operations    |
| write   | Supporting standard Modbus RTU write operations   |
| monitor | Supporting standard Modbus RTU monitor operations |

#### read Command

| Option |                         |              Description             |
| ------ | ----------------------- | ------------------------------------ |
| -c     | --coil                  | Reads coil(s)                        |
| -d     | --discrete              | Reads discrete input(s)              |
| -h     | --holding               | Reads holding register(s)            |
| -i     | --input                 | Reads input register(s)              |
| -x     | --hex                   | Displays the register values in HEX  |
| -n     | --number &lt;number&gt; | The number of items to read          |
| -o     | --offset &lt;number&gt; | The offset of the first item to read |
| -t     | --type &lt;string&gt;   | Reads the specified data type        |

#### write Command

| Option |                         |              Description              |
| ------ | ----------------------- | ------------------------------------- |
| -c     | --coil &lt;json&gt;     | Writes coil(s)                        |
| -h     | --holding &lt;json&gt;  | Writes holding register(s)            |
| -x     | --hex                   | Writes the HEX values (string)        |
| -o     | --offset &lt;number&gt; | The offset of the first item to write |
| -t     | --type &lt;string&gt;   | Writes the specified data type        |

#### monitor Command

| Option |                            |             Description              |
| ------ | -------------------------- | ------------------------------------ |
| -c     | --coil                     | Reads coil(s)                        |
| -d     | --discrete                 | Reads discrete input(s)              |
| -h     | --holding                  | Reads holding register(s)            |
| -i     | --input                    | Reads input register(s)              |
| -x     | --hex                      | Displays the register values in HEX  |
| -n     | --number &lt;number&gt;    | The number of items to read          |
| -o     | --offset &lt;number&gt;    | The offset of the first item to read |
| -t     | --type &lt;string&gt;      | Reads the specified data type        |
| -r     | --repeat  &lt;number&gt;   | The number of times to read          |
| -s     | --seconds  &lt;number>&gt; | The number of seconds between reads  |

The *monitor* command runs for the number of times specified and can be terminated using *Control-C*.

### tcp Command

|               Option             |            Description            |
| -------------------------------- | --------------------------------- |
| --address &lt;string&gt;         | Sets the Modbus slave IP address  |
| --port &lt;number&gt;            | Sets the Modbus slave port number |
| --slaveid &lt;number&gt;         | Sets the Modbus slave ID          |
| --receive-timeout &lt;number&gt; | Sets the timeout in msec (hidden) |
| --send-timeout &lt;number&gt;    | Sets the timeout in msec (hidden) |

The *tcp* command options can also be used in the following sub commands.

| Command |                 Description                       |
| ------- | ------------------------------------------------- |
| read    | Supporting standard Modbus TCP read operations    |
| write   | Supporting standard Modbus TCP write operations   |
| monitor | Supporting standard Modbus TCP monitor operations |

#### read Command

| Option |                         |              Description             |
| ------ | ----------------------- | ------------------------------------ |
| -c     | --coil                  | Reads coil(s)                        |
| -d     | --discrete              | Reads discrete input(s)              |
| -h     | --holding               | Reads holding register(s)            |
| -i     | --input                 | Reads input register(s)              |
| -x     | --hex                   | Displays the register values in HEX  |
| -n     | --number &lt;number&gt; | The number of items to read          |
| -o     | --offset &lt;number&gt; | The offset of the first item to read |
| -t     | --type &lt;string&gt;   | Reads the specified data type        |

#### write Command

| Option |                         |              Description              |
| ------ | ----------------------- | ------------------------------------- |
| -c     | --coil &lt;json&gt;     | Writes coil(s)                        |
| -h     | --holding &lt;json&gt;  | Writes holding register(s)            |
| -x     | --hex                   | Writes the HEX values (string)        |
| -o     | --offset &lt;number&gt; | The offset of the first item to write |
| -t     | --type &lt;string&gt;   | Writes the specified data type        |

#### monitor Command

| Option |                            |             Description              |
| ------ | -------------------------- | ------------------------------------ |
| -c     | --coil                     | Reads coil(s)                        |
| -d     | --discrete                 | Reads discrete input(s)              |
| -h     | --holding                  | Reads holding register(s)            |
| -i     | --input                    | Reads input register(s)              |
| -x     | --hex                      | Displays the register values in HEX  |
| -n     | --number &lt;number&gt;    | The number of items to read          |
| -o     | --offset &lt;number&gt;    | The offset of the first item to read |
| -t     | --type &lt;string&gt;      | Reads the specified data type        |
| -r     | --repeat  &lt;number&gt;   | The number of times to read          |
| -s     | --seconds  &lt;number>&gt; | The number of seconds between reads  |

The *monitor* command runs for the number of times specified and can be terminated using *Control-C*.

## ModbusRTU

The *ModbusRTU* web application is used to access Modbus RTU slave devices providing a REST based web service.
The Web service can act as a HTTP gateway to Modbus slave devices providing a common well known protocol and the use of JSON for the data exchange.

The Web API page provides a common Web interface for the REST web API using Swagger.
The configuration is stored in the *appsettings.json* file.

#### appsettings.json

The typical configuration file contains application settings, the database connection string and the *Serilog* logger settings.
The *TcpSlave* settings allow to specifiy the Modbus TCP slave device.

~~~JSON
{
  "AppSettings": {
    "RtuMaster": {
      "SerialPort": "COM1",
      "Baudrate": 19200,
      "ReadTimeout": 10000,
      "WriteTimeout": 10000
    },
    "RtuSlave": {
      "ID": 1
    }
  },
  "Serilog": {...}
  },
  "AllowedHosts": "*"
}

~~~

## ModbusTCP

The *ModbusTCP* web application is used to access Modbus TCP slave devices providing a REST based web service.
The Web service can act as a HTTP gateway to Modbus slave devices providing a common well known protocol and the use of JSON for the data exchange.

The Web API page provides a common Web interface for the REST web API using Swagger.
The configuration is stored in the *appsettings.json* file.

#### appsettings.json

The typical configuration file contains application settings, the database connection string and the *Serilog* logger settings.
The *RtuMaster* and *RtuSlave* settings allow to specifiy the Modbus RTU master and slave device.

~~~JSON
{
  "AppSettings": {
    "TcpSlave": {
      "Address": "10.0.1.77",
      "Port": 502,
      "ID": 1
    }
  },
  "Serilog": {...}
  },
  "AllowedHosts": "*"
}

~~~

## REST Web API

### Coils & Discrete Inputs

| HTTP | Address | Description |
| ---- | ------- | ----------- |
| GET | /Coil/{offset} | Reading a single coil from a Modbus slave.
| PUT | /Coil/{offset} | Writing a single coil to a Modbus slave.
| GET | /Coils/{offset} | Reading multiple coils from a Modbus slave.
| PUT | /Coils/{offset} | Writing multiple coils to a Modbus slave.
| GET | /DiscreteInput/{offset} | Reading a single discrete input from a Modbus slave.
| GET | /DiscreteInputs/{offset} | Reading multiple discrete inputs from a Modbus slave.

### Holding & Input Registers

| HTTP | Address | Description |
| ---- | ------- | ----------- |
| GET | /HoldingRegister/{offset} | Reading a single holding register from a Modbus slave.
| PUT | /HoldingRegister/{offset} | Writing a single holding registers to a Modbus slave. 
| GET | /HoldingRegisters/{offset} | Reading multiple holding registers from a Modbus slave.
| PUT | /HoldingRegisters/{offset} | Writing multiple holding registers to a Modbus slave.
| GET | /InputRegister/{offset} | Reading a single input register from a Modbus slave.
| GET | /InputRegisters/{offset} | Reading multiple input registers from a Modbus slave.

### Boolean Data Values

| HTTP | Address | Description |
| ---- | ------- | ----------- |
| GET| /ROArray/Bool/{offset} | Reads an array of boolean values from a Modbus slave.
| GET| /ROSingle/Bool/{offset} | Reads a boolean value from a Modbus slave.
| GET| /RWArray/Bool/{offset} | Reads an array of boolean values from a Modbus slave.
| PUT| /RWArray/Bool/{offset} | Writes an array of boolean values to a Modbus slave.
| GET| /RWSingle/Bool/{offset} | Reads a boolean value from a Modbus slave.
| PUT| /RWSingle/Bool/{offset} | Writes a boolean value to a Modbus slave.

### 8-Bit Data Values

| HTTP | Address | Description |
| ---- | ------- | ----------- |
| GET| /ROArray/Bytes/{offset} | Reads 8-bit values from a Modbus slave.
| GET| /RWArray/Bytes/{offset} | Reads 8-bit values from a Modbus slave.
| PUT| /RWArray/Bytes/{offset} | Writes 8-bit values to a Modbus slave.

### Short Data Values

| HTTP | Address | Description |
| ---- | ------- | ----------- |
| GET| /ROArray/Short/{offset} | Reads an array of 16 bit integers from a Modbus slave.
| GET| /ROSingle/Short/{offset} | Reads a 16 bit integer from a Modbus slave.
| GET| /RWArray/Short/{offset} | Reads an array of 16 bit integers from a Modbus slave.
| PUT| /RWArray/Short/{offset} | Writes an array of 16 bit integers to a Modbus slave.
| GET| /RWSingle/Short/{offset} | Reads a 16 bit integer from a Modbus slave.
| PUT| /RWSingle/Short/{offset} | Writes a 16 bit integer to a Modbus slave.

Note that this writes a single holding register.

### UShort Data Values

| HTTP | Address | Description |
| ---- | ------- | ----------- |
| GET| /ROArray/UShort/{offset} | Reads an array of unsigned 16 bit integers from a Modbus slave.
| GET| /ROSingle/UShort/{offset} | Reads an unsigned 16 bit integer from a Modbus slave.
| GET| /RWArray/UShort/{offset} | Reads an array of unsigned 16 bit integer from a Modbus slave.
| PUT| /RWArray/UShort/{offset} | Writes an array of unsigned 16 bit integer to a Modbus slave.
| GET| /RWSingle/UShort/{offset} | Reads an unsigned 16 bit integer from a Modbus slave.
| PUT| /RWSingle/UShort/{offset} | Writes an unsigned 16 bit integer (single holding register) to a Modbus slave.

### Int32 Data Values

| HTTP | Address | Description |
| ---- | ------- | ----------- |
| GET| /ROArray/Int32/{offset} | Reads an array of 32 bit integers from a Modbus slave.
| GET| /ROSingle/Int32/{offset} | Reads a 32 bit integer from a Modbus slave.
| GET| /RWArray/Int32/{offset} | Reads an array of 32 bit integers from a Modbus slave.
| PUT| /RWArray/Int32/{offset} | Writes an array of 32 bit integers to a Modbus slave.
| GET| /RWSingle/Int32/{offset} | Reads a 32 bit integer from a Modbus slave.
| PUT| /RWSingle/Int32/{offset} | Writes a 32 bit integer to a Modbus slave.

### UInt32 Data Values

| HTTP | Address | Description |
| ---- | ------- | ----------- |
| GET| /ROArray/UInt32/{offset} | Reads an array of unsigned 32 bit integers from a Modbus slave.
| GET| /ROSingle/UInt32/{offset} | Reads an unsigned 32 bit integer from a Modbus slave.
| GET| /RWArray/UInt32/{offset} | Reads an array of unsigned 32 bit integers from a Modbus slave.
| PUT| /RWArray/UInt32/{offset} | Writes an array of unsigned 32 bit integers to a Modbus slave.
| GET| /RWSingle/UInt32/{offset} | Reads an unsigned 32 bit integer from a Modbus slave.
| PUT| /RWSingle/UInt32/{offset} | Writes an unsigned 32 bit integer to a Modbus slave.

### Float Data Values

| HTTP | Address | Description |
| ---- | ------- | ----------- |
| GET| /ROArray/Float/{offset} | Reads an array of 32 bit IEEE floating point numbers from a Modbus slave.
| GET| /ROSingle/Float/{offset} | Reads a 32 bit IEEE floating point number from a Modbus slave.
| GET| /RWArray/Float/{offset} | Reads an array of 32 bit IEEE floating point numbers from a Modbus slave.
| PUT| /RWArray/Float/{offset} | Writes an array of 32 bit IEEE floating point numbers to a Modbus slave.
| GET| /RWSingle/Float/{offset} | Reads a 32 bit IEEE floating point number from a Modbus slave.
| PUT| /RWSingle/Float/{offset} | Writes a 32 bit IEEE floating point number to a Modbus slave.

### Double Data Values

| HTTP | Address | Description |
| ---- | ------- | ----------- |
| GET| /ROArray/Double/{offset} | Reads an array of 64 bit IEEE floating point numbers from a Modbus slave.
| GET| /ROSingle/Double/{offset} | Reads a 64 bit IEEE floating point number from a Modbus slave.
| GET| /RWArray/Double/{offset} | Reads an array of 64 bit IEEE floating point numbers from a Modbus slave.
| PUT| /RWArray/Double/{offset} | Writes an array of 64 bit IEEE floating point numbers to a Modbus slave.
| GET| /RWSingle/Double/{offset} | Reads a 64 bit IEEE floating point number from a Modbus slave.
| PUT| /RWSingle/Double/{offset} | Writes a 64 bit IEEE floating point number to a Modbus slave.

### Long Data Values

| HTTP | Address | Description |
| ---- | ------- | ----------- |
| GET| /ROArray/Long/{offset} | Reads an array of 64 bit integers from a Modbus slave.
| GET| /ROSingle/Long/{offset} | Reads a 64 bit integer from a Modbus slave.
| GET| /RWArray/Long/{offset} | Reads an array of 64 bit integers from a Modbus slave.
| PUT| /RWArray/Long/{offset} | Writes an array of 64 bit integers to a Modbus slave.
| GET| /RWSingle/Long/{offset} | Reads a 64 bit integer from a Modbus slave.
| PUT| /RWSingle/Long/{offset} | Writes a 64 bit integer to a Modbus slave.

### ULong Data Values

| HTTP | Address | Description |
| ---- | ------- | ----------- |
| GET| /ROArray/ULong/{offset} | Reads an array of unsigned 64 bit integers from a Modbus slave.
| GET| /ROSingle/ULong/{offset} | Reads an unsigned 64 bit integer from a Modbus slave.
| GET| /RWArray/ULong/{offset} | Reads an array of unsigned 64 bit integers from a Modbus slave.
| PUT| /RWArray/ULong/{offset} | Writes an array of unsigned 64 bit integers to a Modbus slave.
| GET| /RWSingle/ULong/{offset} | Reads an unsigned 64 bit integer from a Modbus slave.
| PUT| /RWSingle/ULong/{offset} | Writes an unsigned 64 bit integer to a Modbus slave.

### String Data Values

| HTTP | Address | Description |
| ---- | ------- | ----------- |
| GET| /ROSingle/String/{offset} | Reads an ASCII string from a Modbus slave.
| GET| /RWSingle/String/{offset} | Reads an ASCII string from a Modbus slave.
| PUT| /RWSingle/String/{offset} | Writes an ASCII string to a Modbus slave.

### HEX String Data Values

| HTTP | Address | Description |
| ---- | ------- | ----------- |
| GET| /ROSingle/HexString/{offset} | Reads an HEX string from a Modbus slave.
| GET| /RWSingle/HexString/{offset} | Reads an HEX string from a Modbus slave.
| PUT| /RWSingle/HexString/{offset} | Writes an HEX string to a Modbus slave.

### Bits Data Values

| HTTP | Address | Description |
| ---- | ------- | ----------- |
| GET| /ROSingle/Bits/{offset} | Reads a 16-bit bit array value from a Modbus slave.
| GET| /RWSingle/Bits/{offset} | Reads a 16-bit bit array value from a Modbus slave.
| PUT| /RWSingle/Bits/{offset} | Writes a 16-bit bit array value to a Modbus slave.

### Settings

| HTTP | Address | Description |
| ---- | ------- | ----------- |
| GET| /Settings | Reads the current settings (Modbus Master and Slave).
| PUT| /Settings | Writes the current settings (Modbus Master and Slave).
