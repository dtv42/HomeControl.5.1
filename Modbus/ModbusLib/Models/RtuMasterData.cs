﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RtuMasterData.cs" company="DTV-Online">
//   Copyright(c) 2020 Dr. Peter Trimmel. All rights reserved.
// </copyright>
// <license>
//   Licensed under the MIT license. See the LICENSE file in the project root for more information.
// </license>
// <created>20-4-2020 10:59</created>
// <author>Peter Trimmel</author>
// --------------------------------------------------------------------------------------------------------------------
namespace ModbusLib.Models
{
    #region Using Directives

    using System;
    using System.ComponentModel.DataAnnotations;

    #endregion

    /// <summary>
    /// Helper class holding Modbus RTU communcation data.
    /// </summary>
    public class RtuMasterData
    {
        #region Public Properties

        public string SerialPort { get; set; } = string.Empty;
        [RegularExpression(@"110|300|600|1200|2400|4800|9600|14400|19200|38400|57600|115200|128000|256000")]
        public int Baudrate { get; set; } = 9600;
        public System.IO.Ports.Parity Parity { get; set; } = System.IO.Ports.Parity.None;
        [Range(5, 8)]
        public int DataBits { get; set; } = 8;
        public System.IO.Ports.StopBits StopBits { get; set; } = System.IO.Ports.StopBits.One;
        [Range(-1, Int32.MaxValue)]
        public int ReadTimeout { get; set; } = -1;
        [Range(-1, Int32.MaxValue)]
        public int WriteTimeout { get; set; } = -1;
        [Range(0, Int32.MaxValue)]
        public int Retries { get; set; }
        [Range(0, Int32.MaxValue)]
        public int WaitToRetryMilliseconds { get; set; }
        public bool SlaveBusyUsesRetryCount { get; set; }

        #endregion Public Properties
    }
}