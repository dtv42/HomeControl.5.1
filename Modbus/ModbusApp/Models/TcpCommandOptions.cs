namespace ModbusApp.Models
{
    #region Using Directives

    using System.IO.Ports;

    #endregion

    internal class TcpCommandOptions
    {
        public string Address { get; set; } = string.Empty;
        public int Port { get; set; } = 502;
        public byte SlaveID { get; set; } = 0;
        public int ReceiveTimeout { get; set; } = 0;
        public int SendTimeout { get; set; } = 0;
    }
}
