// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModbusRequestData.cs" company="DTV-Online">
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
    /// Helper class to hold all Modbus request data.
    /// </summary>
    public class ModbusRequestData
    {
        /// <summary>
        /// The Modbus master data.
        /// </summary>
        public RtuMasterData Master { get; set; } = new RtuMasterData();

        /// <summary>
        /// The Modbus slave data.
        /// </summary>
        public RtuSlaveData Slave { get; set; } = new RtuSlaveData();

        /// <summary>
        /// The Modbus address of the first data item (offset).
        /// </summary>
        public ushort Offset { get; set; }

        /// <summary>
        /// The number of Modbus data items requested.
        /// </summary>
        public ushort Number { get; set; }
    }
}
