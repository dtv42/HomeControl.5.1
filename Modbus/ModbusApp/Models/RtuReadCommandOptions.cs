namespace ModbusApp.Models
{
    #region Using Directives

    using System;
    using System.CommandLine;
    using System.CommandLine.IO;

    #endregion

    internal class RtuReadCommandOptions: RtuCommandOptions
    {
        public bool Coil { get; set; }
        public bool Discrete { get; set; }
        public bool Holding { get; set; }
        public bool Input { get; set; }
        public bool Hex { get; set; }
        public ushort Number { get; set; }
        public ushort Offset { get; set; }
        public string Type { get; set; } = string.Empty;

        /// <summary>
        ///  Additional check on options.
        /// </summary>
        /// <returns></returns>
        public void CheckOptions(IConsole console)
        {
            // Ignoring some of the options
            if (Hex && !string.IsNullOrEmpty(Type) && !Type.Equals("string", StringComparison.InvariantCultureIgnoreCase))
            {
                console.Out.WriteLine("HEX output option is ignored (-x can only be used with type 'string').");
            }

            if (Hex && (Coil || Discrete))
            {
                console.Out.WriteLine($"HEX output option is ignored (-x can only be used with -h or -i).");
            }

            if (!string.IsNullOrEmpty(Type) && (Coil || Discrete))
            {
                console.Out.WriteLine($"Specified type '{Type}' is ignored (-t can only be used with -h or -i).");
            }
        }
    }
}
