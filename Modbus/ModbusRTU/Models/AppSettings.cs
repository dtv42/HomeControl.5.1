// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppSettings.cs" company="DTV-Online">
//   Copyright(c) 2020 Dr. Peter Trimmel. All rights reserved.
// </copyright>
// <license>
//   Licensed under the MIT license. See the LICENSE file in the project root for more information.
// </license>
// <created>13-5-2020 13:52</created>
// <author>Peter Trimmel</author>
// --------------------------------------------------------------------------------------------------------------------
namespace ModbusRTU.Models
{
    #region Using Directives

    using ModbusLib.Models;

    #endregion

    /// <summary>
    /// Helper class to hold application specific settings.
    /// </summary>
    public class AppSettings : IRtuClientSettings
    {
        #region Public Properties

        /// <summary>
        /// The MODBUS RTU master configuration.
        /// </summary>
        public RtuMasterData RtuMaster { get; set; } = new RtuMasterData();

        /// <summary>
        /// The MODBUS RTU slave configuration.
        /// </summary>
        public RtuSlaveData RtuSlave { get; set; } = new RtuSlaveData();

        #endregion
    }
}
