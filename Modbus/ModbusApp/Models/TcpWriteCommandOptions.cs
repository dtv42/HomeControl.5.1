namespace ModbusApp.Models
{
    #region Using Directives

    using System;
    using System.CommandLine;
    using System.CommandLine.IO;

    #endregion

    internal class TcpWriteCommandOptions : TcpCommandOptions
    {
        public string Coil { get; set; } = string.Empty;
        public string Holding { get; set; } = string.Empty;
        public bool Hex { get; set; }
        public ushort Offset { get; set; } = 0;
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

            if (Hex && !string.IsNullOrEmpty(Coil))
            {
                console.Out.WriteLine($"HEX output option is ignored (-x can only be used with -h -t string).");
            }

            if (!string.IsNullOrEmpty(Type) && !string.IsNullOrEmpty(Coil))
            {
                console.Out.WriteLine($"Specified type '{Type}' is ignored (-t can only be used with -h).");
            }

            if (!string.IsNullOrEmpty(Coil))
            {
                if (!Coil.Contains("["))
                {
                    Coil = "[" + Coil;
                }
                if (!Coil.Contains("]"))
                {
                    Coil += "]";
                }
            }

            if (!string.IsNullOrEmpty(Holding))
            {
                if (!string.IsNullOrEmpty(Type) && Type.Equals("string", StringComparison.InvariantCultureIgnoreCase)) return;

                if (!Holding.Contains("["))
                {
                    Holding = "[" + Holding;
                }
                if (!Holding.Contains("]"))
                {
                    Holding += "]";
                }
            }
        }
    }
}
