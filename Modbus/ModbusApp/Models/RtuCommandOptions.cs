namespace ModbusApp.Models
{
    #region Using Directives

    using System.IO.Ports;

    #endregion

    internal class RtuCommandOptions
    {
        public string SerialPort { get; set; } = string.Empty;
        public int Baudrate { get; set; } = 9600;
        public Parity Parity { get; set; } = Parity.None;
        public int DataBits { get; set; } = 8;
        public StopBits StopBits { get; set; } = StopBits.One;
        public byte SlaveID { get; set; } = 0;
        public int ReadTimeout { get; set; } = 0;
        public int WriteTimeout { get; set; } = 0;
    }
}
