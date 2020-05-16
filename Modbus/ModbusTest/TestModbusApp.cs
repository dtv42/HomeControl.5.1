// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BasicTestApp.cs" company="DTV-Online">
//   Copyright(c) 2020 Dr. Peter Trimmel. All rights reserved.
// </copyright>
// <license>
//   Licensed under the MIT license. See the LICENSE file in the project root for more information.
// </license>
// <created>13-5-2020 13:53</created>
// <author>Peter Trimmel</author>
// --------------------------------------------------------------------------------------------------------------------
namespace ModbusTest
{
    #region Using Directives

    using System;
    using System.IO;

    using Xunit;

    using UtilityLib;
    using System.Diagnostics;

    #endregion Using Directives

    public class TestModbusApp
    {
        #region Private Methods

        /// <summary>
        /// Starts the console application. Specify empty string to run with no arguments.
        /// </summary>
        /// <param name="args">The arguments for the console application.</param>
        /// <returns>The exit code.</returns>
        private (int code, string result) StartConsoleApplication(string args, int delay = 10000)
        {
            var sw = new StringWriter();
            Console.SetOut(sw);

            // Initialize process here
            Process proc = new Process();
            proc.StartInfo.FileName = @"dotnet";

            // add arguments as whole string
            proc.StartInfo.Arguments = "run --no-restore --no-build -- " + args;

            // use it to start from testing environment
            proc.StartInfo.UseShellExecute = false;

            // redirect outputs to have it in testing console
            proc.StartInfo.RedirectStandardOutput = true;
            proc.StartInfo.RedirectStandardError = true;

            // set working directory
            proc.StartInfo.WorkingDirectory = @"C:\Users\peter\source\repos\HomeControl.5.1\Modbus\ModbusApp\";

            // start and wait for exit
            proc.Start();
            proc.WaitForExit(delay);

            // get output to testing console.
            Console.WriteLine(proc.StandardOutput.ReadToEnd());
            Console.Write(proc.StandardError.ReadToEnd());

            // return exit code and results
            return (proc.ExitCode, sw.ToString());
        }

        #endregion Private Methods

        [Theory]
        [InlineData("-?",        "ModbusApp [options] [command]",        ExitCodes.SuccessfullyCompleted)]
        [InlineData("--help",    "ModbusApp [options] [command]",        ExitCodes.SuccessfullyCompleted)]
        [InlineData("--config",  "Configuration:",                       ExitCodes.SuccessfullyCompleted)]
        [InlineData("--verbose", "Commandline Application: ModbusApp",   ExitCodes.SuccessfullyCompleted)]
        [InlineData("--version", "1.0.0",                                ExitCodes.SuccessfullyCompleted)]
        [InlineData("",          "Specify verbose or config option",     ExitCodes.IncorrectFunction)]
        [InlineData("-",         "Unrecognized command or argument '-'", ExitCodes.IncorrectFunction)]
        [InlineData("---",       "'---' was not matched.",               ExitCodes.IncorrectFunction)]
        public void TestRootCommand(string args, string text, int exit)
        {
            var (code, result) = StartConsoleApplication(args);
            Assert.Equal(exit, code);
            Assert.Contains(text, result);
        }

        [Theory]
        [InlineData("rtu",                   "RTU serial port found",             ExitCodes.SuccessfullyCompleted)]
        [InlineData("rtu --serialport COM1", "RTU serial port found",             ExitCodes.SuccessfullyCompleted)]
        [InlineData("rtu --baudrate 9600",   "RTU serial port found",             ExitCodes.SuccessfullyCompleted)]
        [InlineData("rtu --parity none",     "RTU serial port found",             ExitCodes.SuccessfullyCompleted)]
        [InlineData("rtu --databits 8",      "RTU serial port found",             ExitCodes.SuccessfullyCompleted)]
        [InlineData("rtu --stopbits one",    "RTU serial port found",             ExitCodes.SuccessfullyCompleted)]
        [InlineData("rtu --slaveid 1",       "RTU serial port found",             ExitCodes.SuccessfullyCompleted)]
        [InlineData("rtu --serialport COM0", "RTU serial port not found",         ExitCodes.NotSuccessfullyCompleted)]
        [InlineData("rtu --baudrate 1000",   "Invalid Baudrate",                  ExitCodes.IncorrectFunction)]
        [InlineData("rtu --parity uneven",   "Cannot parse argument",             ExitCodes.IncorrectFunction)]
        [InlineData("rtu --databits 4",      "DataBits value must be between",    ExitCodes.IncorrectFunction)]
        [InlineData("rtu --stopbits five",   "Cannot parse argument",             ExitCodes.IncorrectFunction)]
        [InlineData("rtu --slaveid 300",     "Cannot parse argument",             ExitCodes.IncorrectFunction)]
        [InlineData("rtu --verbose",         "Modbus Commandline Application:",   ExitCodes.SuccessfullyCompleted)]
        [InlineData("rtu -?",                "ModbusApp rtu [options] [command]", ExitCodes.SuccessfullyCompleted)]
        [InlineData("rtu --help",            "ModbusApp rtu [options] [command]", ExitCodes.SuccessfullyCompleted)]
        public void TestRtuCommand(string args, string text, int exit)
        {
            var (code, result) = StartConsoleApplication(args);
            Assert.Equal(exit, code);
            Assert.Contains(text, result);
        }

        [Theory]
        [InlineData("rtu read -c",           "Value of coil[0]",                      ExitCodes.SuccessfullyCompleted)]
        [InlineData("rtu read -c -n 3",      "Value of coil[2]",                      ExitCodes.SuccessfullyCompleted)]
        [InlineData("rtu read -c -o 5",      "Value of coil[5]",                      ExitCodes.SuccessfullyCompleted)]
        [InlineData("rtu read -c -n 3 -o 5", "Value of coil[7]",                      ExitCodes.SuccessfullyCompleted)]
        [InlineData("rtu read -d",           "Value of discrete input[0]",            ExitCodes.SuccessfullyCompleted)]
        [InlineData("rtu read -d -n 3",      "Value of discrete input[2]",            ExitCodes.SuccessfullyCompleted)]
        [InlineData("rtu read -d -o 5",      "Value of discrete input[5]",            ExitCodes.SuccessfullyCompleted)]
        [InlineData("rtu read -d -n 3 -o 5", "Value of discrete input[7]",            ExitCodes.SuccessfullyCompleted)]
        [InlineData("rtu read -h",           "Value of holding register[0]",          ExitCodes.SuccessfullyCompleted)]
        [InlineData("rtu read -h -n 3",      "Value of holding register[2]",          ExitCodes.SuccessfullyCompleted)]
        [InlineData("rtu read -h -o 5",      "Value of holding register[5]",          ExitCodes.SuccessfullyCompleted)]
        [InlineData("rtu read -h -n 3 -o 5", "Value of holding register[7]",          ExitCodes.SuccessfullyCompleted)]
        [InlineData("rtu read -i",           "Value of input register[0]",            ExitCodes.SuccessfullyCompleted)]
        [InlineData("rtu read -i -n 3",      "Value of input register[2]",            ExitCodes.SuccessfullyCompleted)]
        [InlineData("rtu read -i -o 5",      "Value of input register[5]",            ExitCodes.SuccessfullyCompleted)]
        [InlineData("rtu read -i -n 3 -o 5", "Value of input register[7]",            ExitCodes.SuccessfullyCompleted)]
        [InlineData("rtu read",              "Specify a single read option",          ExitCodes.IncorrectFunction)]
        [InlineData("rtu read -?",           "Supporting Modbus RTU read operations", ExitCodes.IncorrectFunction)]
        [InlineData("rtu read --help",       "Supporting Modbus RTU read operations", ExitCodes.IncorrectFunction)]
        public void TestRtuReadCommand(string args, string text, int exit)
        {
            var (code, result) = StartConsoleApplication(args);
            Assert.Equal(exit, code);
            Assert.Contains(text, result);
        }

        [Theory]
        [InlineData("rtu write -c false",                  "Write single coil[0]",                      ExitCodes.SuccessfullyCompleted)]
        [InlineData("rtu write -c false,false,false",      "Writing 3 coils starting at 0",             ExitCodes.SuccessfullyCompleted)]
        [InlineData("rtu write -c false -o 5",             "Write single coil[5]",                      ExitCodes.SuccessfullyCompleted)]
        [InlineData("rtu write -c false,false,false -o 5", "Writing 3 coils starting at 5",             ExitCodes.SuccessfullyCompleted)]
        [InlineData("rtu write -c [false]",                "Write single coil[0]",                      ExitCodes.SuccessfullyCompleted)]
        [InlineData("rtu write -c false]",                 "Write single coil[0]",                      ExitCodes.SuccessfullyCompleted)]
        [InlineData("rtu write -c [false",                 "Write single coil[0]",                      ExitCodes.SuccessfullyCompleted)]
        [InlineData("rtu write -h 1",                      "Writing single holding register[0]",        ExitCodes.SuccessfullyCompleted)]
        [InlineData("rtu write -h 1,2,3",                  "Writing 3 holding registers starting at 0", ExitCodes.SuccessfullyCompleted)]
        [InlineData("rtu write -h 1 -o 5",                 "Writing single holding register[5]",        ExitCodes.SuccessfullyCompleted)]
        [InlineData("rtu write -h 1,2,3 -o 5",             "Writing 3 holding registers starting at 5", ExitCodes.SuccessfullyCompleted)]
        [InlineData("rtu write -h [1]",                    "Writing single holding register[0]",        ExitCodes.SuccessfullyCompleted)]
        [InlineData("rtu write -h 1]",                     "Writing single holding register[0]",        ExitCodes.SuccessfullyCompleted)]
        [InlineData("rtu write -h [1",                     "Writing single holding register[0]",        ExitCodes.SuccessfullyCompleted)]
        [InlineData("rtu write -c",                        "Required argument missing for option",      ExitCodes.IncorrectFunction)]
        [InlineData("rtu write -h",                        "Required argument missing for option",      ExitCodes.IncorrectFunction)]
        [InlineData("rtu write",                           "Specify a single write option",             ExitCodes.IncorrectFunction)]
        [InlineData("rtu write -?",                        "Supporting Modbus RTU write operations",    ExitCodes.IncorrectFunction)]
        [InlineData("rtu write --help",                    "Supporting Modbus RTU write operations",    ExitCodes.IncorrectFunction)]
        public void TestRtuWriteReadCommand(string args, string text, int exit)
        {
            var (code, result) = StartConsoleApplication(args);
            Assert.Equal(exit, code);
            Assert.Contains(text, result);
        }

        [Theory]
        [InlineData("rtu monitor -c -r 2",      "Value of coil[0]",                         ExitCodes.SuccessfullyCompleted)]
        [InlineData("rtu monitor -c -s 2",      "Value of coil[0]",                         ExitCodes.SuccessfullyCompleted)]
        [InlineData("rtu monitor -c -r 2 -s 2", "Value of coil[0]",                         ExitCodes.SuccessfullyCompleted)]
        [InlineData("rtu monitor -c",           "Value of coil[0]",                         ExitCodes.SuccessfullyCompleted)]
        [InlineData("rtu monitor -c -n 3",      "Value of coil[2]",                         ExitCodes.SuccessfullyCompleted)]
        [InlineData("rtu monitor -c -o 5",      "Value of coil[5]",                         ExitCodes.SuccessfullyCompleted)]
        [InlineData("rtu monitor -c -n 3 -o 5", "Value of coil[7]",                         ExitCodes.SuccessfullyCompleted)]
        [InlineData("rtu monitor -d",           "Value of discrete input[0]",               ExitCodes.SuccessfullyCompleted)]
        [InlineData("rtu monitor -d -n 3",      "Value of discrete input[2]",               ExitCodes.SuccessfullyCompleted)]
        [InlineData("rtu monitor -d -o 5",      "Value of discrete input[5]",               ExitCodes.SuccessfullyCompleted)]
        [InlineData("rtu monitor -d -n 3 -o 5", "Value of discrete input[7]",               ExitCodes.SuccessfullyCompleted)]
        [InlineData("rtu monitor -h",           "Value of holding register[0]",             ExitCodes.SuccessfullyCompleted)]
        [InlineData("rtu monitor -h -n 3",      "Value of holding register[2]",             ExitCodes.SuccessfullyCompleted)]
        [InlineData("rtu monitor -h -o 5",      "Value of holding register[5]",             ExitCodes.SuccessfullyCompleted)]
        [InlineData("rtu monitor -h -n 3 -o 5", "Value of holding register[7]",             ExitCodes.SuccessfullyCompleted)]
        [InlineData("rtu monitor -i",           "Value of input register[0]",               ExitCodes.SuccessfullyCompleted)]
        [InlineData("rtu monitor -i -n 3",      "Value of input register[2]",               ExitCodes.SuccessfullyCompleted)]
        [InlineData("rtu monitor -i -o 5",      "Value of input register[5]",               ExitCodes.SuccessfullyCompleted)]
        [InlineData("rtu monitor -i -n 3 -o 5", "Value of input register[7]",               ExitCodes.SuccessfullyCompleted)]
        [InlineData("rtu monitor",              "Specify a single read option",             ExitCodes.IncorrectFunction)]
        [InlineData("rtu monitor -?",           "Supporting Modbus RTU monitor operations", ExitCodes.IncorrectFunction)]
        [InlineData("rtu monitor --help",       "Supporting Modbus RTU monitor operations", ExitCodes.IncorrectFunction)]
        public void TestRtuMonitorReadCommand(string args, string text, int exit)
        {
            var (code, result) = StartConsoleApplication(args);
            Assert.Equal(exit, code);
            Assert.Contains(text, result);
        }

        [Theory]
        [InlineData("tcp",           "Modbus TCP slave found at",         ExitCodes.SuccessfullyCompleted)]
        [InlineData("tcp --verbose", "Modbus Commandline Application:",   ExitCodes.SuccessfullyCompleted)]
        [InlineData("tcp -?",        "ModbusApp tcp [options] [command]", ExitCodes.SuccessfullyCompleted)]
        [InlineData("tcp --help",    "ModbusApp tcp [options] [command]", ExitCodes.SuccessfullyCompleted)]
        public void TestTcpCommand(string args, string text, int exit)
        {
            var (code, result) = StartConsoleApplication(args);
            Assert.Equal(exit, code);
            Assert.Contains(text, result);
        }

        [Theory]
        [InlineData("tcp read -c",           "Value of coil[0]",                      ExitCodes.SuccessfullyCompleted)]
        [InlineData("tcp read -c -n 3",      "Value of coil[2]",                      ExitCodes.SuccessfullyCompleted)]
        [InlineData("tcp read -c -o 5",      "Value of coil[5]",                      ExitCodes.SuccessfullyCompleted)]
        [InlineData("tcp read -c -n 3 -o 5", "Value of coil[7]",                      ExitCodes.SuccessfullyCompleted)]
        [InlineData("tcp read -d",           "Value of discrete input[0]",            ExitCodes.SuccessfullyCompleted)]
        [InlineData("tcp read -d -n 3",      "Value of discrete input[2]",            ExitCodes.SuccessfullyCompleted)]
        [InlineData("tcp read -d -o 5",      "Value of discrete input[5]",            ExitCodes.SuccessfullyCompleted)]
        [InlineData("tcp read -d -n 3 -o 5", "Value of discrete input[7]",            ExitCodes.SuccessfullyCompleted)]
        [InlineData("tcp read -h",           "Value of holding register[0]",          ExitCodes.SuccessfullyCompleted)]
        [InlineData("tcp read -h -n 3",      "Value of holding register[2]",          ExitCodes.SuccessfullyCompleted)]
        [InlineData("tcp read -h -o 5",      "Value of holding register[5]",          ExitCodes.SuccessfullyCompleted)]
        [InlineData("tcp read -h -n 3 -o 5", "Value of holding register[7]",          ExitCodes.SuccessfullyCompleted)]
        [InlineData("tcp read -i",           "Value of input register[0]",            ExitCodes.SuccessfullyCompleted)]
        [InlineData("tcp read -i -n 3",      "Value of input register[2]",            ExitCodes.SuccessfullyCompleted)]
        [InlineData("tcp read -i -o 5",      "Value of input register[5]",            ExitCodes.SuccessfullyCompleted)]
        [InlineData("tcp read -i -n 3 -o 5", "Value of input register[7]",            ExitCodes.SuccessfullyCompleted)]
        [InlineData("tcp read",              "Specify a single read option",          ExitCodes.IncorrectFunction)]
        [InlineData("tcp read -?",           "Supporting Modbus TCP read operations", ExitCodes.IncorrectFunction)]
        [InlineData("tcp read --help",       "Supporting Modbus TCP read operations", ExitCodes.IncorrectFunction)]
        public void TestTcpReadCommand(string args, string text, int exit)
        {
            var (code, result) = StartConsoleApplication(args);
            Assert.Equal(exit, code);
            Assert.Contains(text, result);
        }

        [Theory]
        [InlineData("tcp write -c false",                  "Write single coil[0]",                      ExitCodes.SuccessfullyCompleted)]
        [InlineData("tcp write -c false,false,false",      "Writing 3 coils starting at 0",             ExitCodes.SuccessfullyCompleted)]
        [InlineData("tcp write -c false -o 5",             "Write single coil[5]",                      ExitCodes.SuccessfullyCompleted)]
        [InlineData("tcp write -c false,false,false -o 5", "Writing 3 coils starting at 5",             ExitCodes.SuccessfullyCompleted)]
        [InlineData("tcp write -c [false]",                "Write single coil[0]",                      ExitCodes.SuccessfullyCompleted)]
        [InlineData("tcp write -c false]",                 "Write single coil[0]",                      ExitCodes.SuccessfullyCompleted)]
        [InlineData("tcp write -c [false",                 "Write single coil[0]",                      ExitCodes.SuccessfullyCompleted)]
        [InlineData("tcp write -h 1",                      "Writing single holding register[0]",        ExitCodes.SuccessfullyCompleted)]
        [InlineData("tcp write -h 1,2,3",                  "Writing 3 holding registers starting at 0", ExitCodes.SuccessfullyCompleted)]
        [InlineData("tcp write -h 1 -o 5",                 "Writing single holding register[5]",        ExitCodes.SuccessfullyCompleted)]
        [InlineData("tcp write -h 1,2,3 -o 5",             "Writing 3 holding registers starting at 5", ExitCodes.SuccessfullyCompleted)]
        [InlineData("tcp write -h [1]",                    "Writing single holding register[0]",        ExitCodes.SuccessfullyCompleted)]
        [InlineData("tcp write -h 1]",                     "Writing single holding register[0]",        ExitCodes.SuccessfullyCompleted)]
        [InlineData("tcp write -h [1",                     "Writing single holding register[0]",        ExitCodes.SuccessfullyCompleted)]
        [InlineData("tcp write -c",                        "Required argument missing for option",      ExitCodes.IncorrectFunction)]
        [InlineData("tcp write -h",                        "Required argument missing for option",      ExitCodes.IncorrectFunction)]
        [InlineData("tcp write",                           "Specify a single write option",             ExitCodes.IncorrectFunction)]
        [InlineData("tcp write -?",                        "Supporting Modbus TCP write operations",    ExitCodes.IncorrectFunction)]
        [InlineData("tcp write --help",                    "Supporting Modbus TCP write operations",    ExitCodes.IncorrectFunction)]
        public void TestTcpWriteCommand(string args, string text, int exit)
        {
            var (code, result) = StartConsoleApplication(args);
            Assert.Equal(exit, code);
            Assert.Contains(text, result);
        }

        [Theory] 
        [InlineData("tcp monitor -c -r 2",      "Value of coil[0]",                         ExitCodes.SuccessfullyCompleted)]
        [InlineData("tcp monitor -c -s 2",      "Value of coil[0]",                         ExitCodes.SuccessfullyCompleted)]
        [InlineData("tcp monitor -c -r 2 -s 2", "Value of coil[0]",                         ExitCodes.SuccessfullyCompleted)]
        [InlineData("tcp monitor -c",           "Value of coil[0]",                         ExitCodes.SuccessfullyCompleted)]
        [InlineData("tcp monitor -c -n 3",      "Value of coil[2]",                         ExitCodes.SuccessfullyCompleted)]
        [InlineData("tcp monitor -c -o 5",      "Value of coil[5]",                         ExitCodes.SuccessfullyCompleted)]
        [InlineData("tcp monitor -c -n 3 -o 5", "Value of coil[7]",                         ExitCodes.SuccessfullyCompleted)]
        [InlineData("tcp monitor -d",           "Value of discrete input[0]",               ExitCodes.SuccessfullyCompleted)]
        [InlineData("tcp monitor -d -n 3",      "Value of discrete input[2]",               ExitCodes.SuccessfullyCompleted)]
        [InlineData("tcp monitor -d -o 5",      "Value of discrete input[5]",               ExitCodes.SuccessfullyCompleted)]
        [InlineData("tcp monitor -d -n 3 -o 5", "Value of discrete input[7]",               ExitCodes.SuccessfullyCompleted)]
        [InlineData("tcp monitor -h",           "Value of holding register[0]",             ExitCodes.SuccessfullyCompleted)]
        [InlineData("tcp monitor -h -n 3",      "Value of holding register[2]",             ExitCodes.SuccessfullyCompleted)]
        [InlineData("tcp monitor -h -o 5",      "Value of holding register[5]",             ExitCodes.SuccessfullyCompleted)]
        [InlineData("tcp monitor -h -n 3 -o 5", "Value of holding register[7]",             ExitCodes.SuccessfullyCompleted)]
        [InlineData("tcp monitor -i",           "Value of input register[0]",               ExitCodes.SuccessfullyCompleted)]
        [InlineData("tcp monitor -i -n 3",      "Value of input register[2]",               ExitCodes.SuccessfullyCompleted)]
        [InlineData("tcp monitor -i -o 5",      "Value of input register[5]",               ExitCodes.SuccessfullyCompleted)]
        [InlineData("tcp monitor -i -n 3 -o 5", "Value of input register[7]",               ExitCodes.SuccessfullyCompleted)]
        [InlineData("tcp monitor",              "Specify a single read option",             ExitCodes.IncorrectFunction)]
        [InlineData("tcp monitor -?",           "Supporting Modbus TCP monitor operations", ExitCodes.IncorrectFunction)]
        [InlineData("tcp monitor --help",       "Supporting Modbus TCP monitor operations", ExitCodes.IncorrectFunction)]
        public void TestTcpMonitorCommand(string args, string text, int exit)
        {
            var (code, result) = StartConsoleApplication(args);
            Assert.Equal(exit, code);
            Assert.Contains(text, result);
        }
    }
}