// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TcpMasterData.cs" company="DTV-Online">
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
    #region Using Directives

    using System;
    using System.ComponentModel.DataAnnotations;

    #endregion

    /// <summary>
    /// Helper class holding Modbus TCP communcation data.
    /// </summary>
    public class TcpMasterData
    {
        #region Public Properties

        public bool ExclusiveAddressUse { get; set; }

        [Range(0, Int32.MaxValue)]
        public int ReceiveTimeout { get; set; } = 1000;

        [Range(0, Int32.MaxValue)]
        public int SendTimeout { get; set; } = 1000;

        #endregion Public Properties
    }
}