// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BasicTestApp.cs" company="DTV-Online">
//   Copyright(c) 2020 Dr. Peter Trimmel. All rights reserved.
// </copyright>
// <license>
//   Licensed under the MIT license. See the LICENSE file in the project root for more information.
// </license>
// <created>19-4-2020 18:30</created>
// <author>Peter Trimmel</author>
// --------------------------------------------------------------------------------------------------------------------
namespace UtilityTest
{
    #region Using Directives

    using System;
    using System.IO;

    using Xunit;

    using UtilityLib;
    using System.Diagnostics;

    #endregion Using Directives

    public class BasicTestApp
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
            proc.StartInfo.WorkingDirectory = @"C:\Users\peter\source\repos\HomeControl.5.1\Utilities\UtilityApp\";

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
        [InlineData("", "Time elapsed", ExitCodes.SuccessfullyCompleted)]
        [InlineData("-?", "Usage: UtilityApp [command] [options]", ExitCodes.SuccessfullyCompleted)]
        [InlineData("--help", "Usage: UtilityApp [command] [options]", ExitCodes.SuccessfullyCompleted)]
        [InlineData("--config", "Configuration:", ExitCodes.SuccessfullyCompleted)]
        [InlineData("--verbose", "Commandline application: UtilityApp", ExitCodes.SuccessfullyCompleted)]
        [InlineData("--version", "1.0.0", ExitCodes.SuccessfullyCompleted)]
        [InlineData("-", "Unrecognized command or argument '-'", ExitCodes.NotSuccessfullyCompleted)]
        [InlineData("---", "Unrecognized option '---'", ExitCodes.NotSuccessfullyCompleted)]
        public void TestRootCommand(string args, string text, int exit)
        {
            var (code, result) = StartConsoleApplication(args);
            Assert.Equal(exit, code);
            Assert.Contains(text, result);
        }

        [Theory]
        [InlineData("settings", "Settings:", ExitCodes.SuccessfullyCompleted)]
        [InlineData("settings --verbose", "Commandline Application: UtilityApp", ExitCodes.SuccessfullyCompleted)]
        [InlineData("settings -?", "Usage: UtilityApp settings [options]", ExitCodes.SuccessfullyCompleted)]
        [InlineData("settings --help", "Usage: UtilityApp settings [options]", ExitCodes.SuccessfullyCompleted)]
        public void TestSettingsCommand(string args, string text, int exit)
        {
            var (code, result) = StartConsoleApplication(args);
            Assert.Equal(exit, code);
            Assert.Contains(text, result);
        }

        [Theory]
        [InlineData("testdata", "Time elapsed", ExitCodes.SuccessfullyCompleted)]
        [InlineData("testdata -j", "TestData: {", ExitCodes.SuccessfullyCompleted)]
        [InlineData("testdata --json", "TestData: {", ExitCodes.SuccessfullyCompleted)]
        [InlineData("testdata -n", "TestData():", ExitCodes.SuccessfullyCompleted)]
        [InlineData("testdata --newdata", "TestData():", ExitCodes.SuccessfullyCompleted)]
        [InlineData("testdata -g 6dceb058-526f-4413-8fce-b60257a496a2", "Guid:     6dceb058-526f-4413-8fce-b60257a496a2", ExitCodes.SuccessfullyCompleted)]
        [InlineData("testdata --guid 6dceb058-526f-4413-8fce-b60257a496a2", "Guid:     6dceb058-526f-4413-8fce-b60257a496a2", ExitCodes.SuccessfullyCompleted)]
        [InlineData("testdata -a \"0.0.0.0\"", "Address:  0.0.0.0", ExitCodes.SuccessfullyCompleted)]
        [InlineData("testdata --address \"0.0.0.0\"", "Address:  0.0.0.0", ExitCodes.SuccessfullyCompleted)]
        [InlineData("testdata -e \"0.0.0.0:88\"", "Endpoint: 0.0.0.0:88", ExitCodes.SuccessfullyCompleted)]
        [InlineData("testdata --endpoint \"0.0.0.0:88\"", "Endpoint: 0.0.0.0:88", ExitCodes.SuccessfullyCompleted)]
        [InlineData("testdata -u \"http://0.0.0.0\"", "Uri:      http://0.0.0.0", ExitCodes.SuccessfullyCompleted)]
        [InlineData("testdata --uri \"http://0.0.0.0\"", "Uri:      http://0.0.0.0", ExitCodes.SuccessfullyCompleted)]
        [InlineData("testdata -c \"ok\"", "Code:     OK", ExitCodes.SuccessfullyCompleted)]
        [InlineData("testdata --code \"ok\"", "Code:     OK", ExitCodes.SuccessfullyCompleted)]
        [InlineData("testdata -g 6dceb058-526f-4413-8fce-b60257a496a2-xx", "Guid should contain 32 digits with 4 dashes (xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx).", ExitCodes.NotSuccessfullyCompleted)]
        [InlineData("testdata --guid 6dceb058-526f-4413-8fce-b60257a496a2-xx", "Guid should contain 32 digits with 4 dashes (xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx).", ExitCodes.NotSuccessfullyCompleted)]
        [InlineData("testdata -a \"0.0.0.0.0\"", "The IPAddress value must be valid: <xxx.xxx.xxx.xxx>", ExitCodes.IncorrectFunction)]
        [InlineData("testdata --address \"0.0.0.0.0\"", "The IPAddress value must be valid: <xxx.xxx.xxx.xxx>", ExitCodes.IncorrectFunction)]
        [InlineData("testdata -e \"0.0.0.0.0:88\"", "The IPEndPoint value must be valid: <address>:<port>", ExitCodes.IncorrectFunction)]
        [InlineData("testdata --endpoint \"0.0.0.0.0:88\"", "The IPEndPoint value must be valid: <address>:<port>", ExitCodes.IncorrectFunction)]
        [InlineData("testdata -u \"httpx://0.0.0.0\"", "The Uri value must be valid: '<scheme>//<host>:<port>'", ExitCodes.IncorrectFunction)]
        [InlineData("testdata --uri \"httpx:0.0.0.0\"", "The Uri value must be valid: '<scheme>//<host>:<port>'", ExitCodes.IncorrectFunction)]
        [InlineData("testdata -c \"xxx\"", "xxx is not a valid value for HttpStatusCode.", ExitCodes.NotSuccessfullyCompleted)]
        [InlineData("testdata --code \"xxx\"", "xxx is not a valid value for HttpStatusCode.", ExitCodes.NotSuccessfullyCompleted)]
        [InlineData("testdata --verbose", "Commandline Application: UtilityApp", ExitCodes.SuccessfullyCompleted)]
        [InlineData("testdata -?", "Usage: UtilityApp testdata [options]", ExitCodes.SuccessfullyCompleted)]
        [InlineData("testdata --help", "Usage: UtilityApp testdata [options]", ExitCodes.SuccessfullyCompleted)]
        public void TestTestdataCommand(string args, string text, int exit)
        {
            var (code, result) = StartConsoleApplication(args);
            Assert.Equal(exit, code);
            Assert.Contains(text, result);
        }

        [Theory]
        [InlineData("property", "Please select at least a property type", ExitCodes.NotSuccessfullyCompleted)]
        [InlineData("property -p", "Time elapsed", ExitCodes.SuccessfullyCompleted)]
        [InlineData("property -a", "Property StringArray", ExitCodes.SuccessfullyCompleted)]
        [InlineData("property -l", "Property StringList", ExitCodes.SuccessfullyCompleted)]
        [InlineData("property -s", "Property StringValue", ExitCodes.SuccessfullyCompleted)]
        [InlineData("property -d", "Dictionary", ExitCodes.SuccessfullyCompleted)]
        [InlineData("property StringValue", "Property StringValue", ExitCodes.SuccessfullyCompleted)]
        [InlineData("property StringValue -v", "Value:         a string", ExitCodes.SuccessfullyCompleted)]
        [InlineData("property StringValue --value", "Value:         a string", ExitCodes.SuccessfullyCompleted)]
        [InlineData("property -p --verbose", "Commandline Application: UtilityApp", ExitCodes.SuccessfullyCompleted)]
        [InlineData("property -?", "Usage: UtilityApp property [options] <PropertyName> <PropertyValue>", ExitCodes.SuccessfullyCompleted)]
        [InlineData("property --help", "Usage: UtilityApp property [options] <PropertyName> <PropertyValue>", ExitCodes.SuccessfullyCompleted)]
        public void TestPropertyCommand(string args, string text, int exit)
        {
            var (code, result) = StartConsoleApplication(args);
            Assert.Equal(exit, code);
            Assert.Contains(text, result);
        }

        [Theory]
        [InlineData("async", "Time elapsed", ExitCodes.SuccessfullyCompleted)]
        [InlineData("async --verbose", "Commandline Application: UtilityApp", ExitCodes.SuccessfullyCompleted)]
        [InlineData("async -?", "Usage: UtilityApp async [options]", ExitCodes.SuccessfullyCompleted)]
        [InlineData("async --help", "Usage: UtilityApp async [options]", ExitCodes.SuccessfullyCompleted)]
        public void TestAsyncCommand(string args, string text, int exit)
        {
            var (code, result) = StartConsoleApplication(args, 15000);
            Assert.Equal(exit, code);
            Assert.Contains(text, result);
        }

        [Theory]
        [InlineData("log", "Time elapsed", ExitCodes.SuccessfullyCompleted)]
        [InlineData("log --verbose", "Commandline Application: UtilityApp", ExitCodes.SuccessfullyCompleted)]
        [InlineData("log -?", "Usage: UtilityApp log [options]", ExitCodes.SuccessfullyCompleted)]
        [InlineData("log --help", "Usage: UtilityApp log [options]", ExitCodes.SuccessfullyCompleted)]
        public void TestLogCommand(string args, string text, int exit)
        {
            var (code, result) = StartConsoleApplication(args);
            Assert.Equal(exit, code);
            Assert.Contains(text, result);
        }
    }
}