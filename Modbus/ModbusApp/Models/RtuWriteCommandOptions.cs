namespace ModbusApp.Models
{
    #region Using Directives

    using System.CommandLine;
    using System.CommandLine.IO;

    #endregion

    internal class RtuWriteCommandOptions : RtuCommandOptions
    {
        public string Values { get; set; } = "[]";
        public bool Coil { get; set; }
        public bool Holding { get; set; }
        public ushort Offset { get; set; } = 0;
        public string Type { get; set; } = string.Empty;

        /// <summary>
        ///  Additional check on options.
        /// </summary>
        /// <returns></returns>
        public void CheckOptions(IConsole console)
        {
            if (!string.IsNullOrEmpty(Type) && Coil)
            {
                console.Out.WriteLine($"Specified type '{Type}' is ignored.");
            }
        }
    }
}
