// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RtuSlaveData.cs" company="DTV-Online">
//   Copyright(c) 2020 Dr. Peter Trimmel. All rights reserved.
// </copyright>
// <license>
//   Licensed under the MIT license. See the LICENSE file in the project root for more information.
// </license>
// <created>13-5-2020 13:52</created>
// <author>Peter Trimmel</author>
// --------------------------------------------------------------------------------------------------------------------
namespace ModbusLib.Models
{
    /// <summary>
    /// Helper class holding Modbus TCP slave data.
    /// </summary>
    public class RtuSlaveData
    {
        #region Public Properties

        public byte ID { get; set; } = 1;

        #endregion Public Properties
    }
}