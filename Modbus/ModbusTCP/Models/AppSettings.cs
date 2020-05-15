// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppSettings.cs" company="DTV-Online">
//   Copyright(c) 2020 Dr. Peter Trimmel. All rights reserved.
// </copyright>
// <license>
//   Licensed under the MIT license. See the LICENSE file in the project root for more information.
// </license>
// <created>13-5-2020 13:53</created>
// <author>Peter Trimmel</author>
// --------------------------------------------------------------------------------------------------------------------
namespace ModbusTCP.Models
{
    #region Using Directives

    using ModbusLib.Models;

    #endregion

    /// <summary>
    /// Helper class to hold application specific settings.
    /// </summary>
    public class AppSettings : ITcpClientSettings
    {
        #region Public Properties

        /// <summary>
        /// The MODBUS TCP master configuration.
        /// </summary>
        public TcpMasterData TcpMaster { get; set; } = new TcpMasterData();

        /// <summary>
        /// The MODBUS TCP slave configuration.
        /// </summary>
        public TcpSlaveData TcpSlave { get; set; } = new TcpSlaveData();

        #endregion
    }
}
