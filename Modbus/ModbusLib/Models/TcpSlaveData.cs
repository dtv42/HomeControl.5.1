// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TcpSlaveData.cs" company="DTV-Online">
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

    #endregion Using Directives

    /// <summary>
    /// Helper class holding Modbus TCP slave data.
    /// </summary>
    public class TcpSlaveData
    {
        #region Public Properties

        [RegularExpression(@"^(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$")]
        public string Address { get; set; } = "127.0.0.1";

        [Range(0, 65535)]
        public int Port { get; set; }

        public byte ID { get; set; }

        #endregion Public Properties
    }
}