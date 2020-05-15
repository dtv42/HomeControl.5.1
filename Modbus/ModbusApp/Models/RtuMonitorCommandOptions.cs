namespace ModbusApp.Models
{
    #region Using Directives

    using System.CommandLine;
    using System.CommandLine.IO;

    #endregion

    internal class RtuMonitorCommandOptions : RtuCommandOptions
    {
        public bool Coil { get; set; }
        public bool Discrete { get; set; }
        public bool Holding { get; set; }
        public bool Input { get; set; }
        public bool Hex { get; set; }
        public ushort Number { get; set; } = 1;
        public ushort Offset { get; set; } = 0;
        public string Type { get; set; } = string.Empty;
        public uint Repeat { get; set; } = 0;
        public uint Seconds { get; set; } = 10;

        /// <summary>
        ///  Additional check on options.
        /// </summary>
        /// <returns></returns>
        public void CheckOptions(IConsole console)
        {
            if ((Coil || Discrete) && Hex)
            {
                console.Out.WriteLine("HEX output option is ignored.");
            }

            if (Input || Holding)
            {
                var message = Type.ToLower() switch
                {
                    "bits" => (Number > 1) ? "Only a single bit array value is supported." : null,
                    "string" => null,
                    _ => Hex ? "HEX output option is ignored." : null
                };

                if (!string.IsNullOrEmpty(message)) console.Out.WriteLine(message);
            }

            if (!string.IsNullOrEmpty(Type) && (Coil || Discrete))
            {
                console.Out.WriteLine($"Specified type '{Type}' is ignored.");
            }
        }
    }
}
