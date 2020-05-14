// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModbusResponseStringData.cs" company="DTV-Online">
//   Copyright(c) 2020 Dr. Peter Trimmel. All rights reserved.
// </copyright>
// <license>
//   Licensed under the MIT license. See the LICENSE file in the project root for more information.
// </license>
// <created>20-4-2020 16:50</created>
// <author>Peter Trimmel</author>
// --------------------------------------------------------------------------------------------------------------------
namespace ModbusTCP.Models
{
    /// <summary>
    /// Helper class to hold Modbus string response data.
    /// </summary>
    public class ModbusResponseStringData
    {
        public ModbusRequestData Request { get; set; } = new ModbusRequestData();
        public string Value { get; set; } = string.Empty;
    }
}
