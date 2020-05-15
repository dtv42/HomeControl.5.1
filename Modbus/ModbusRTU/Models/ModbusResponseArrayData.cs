// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModbusResponseArrayData.cs" company="DTV-Online">
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
    /// <summary>
    /// Helper class to hold all Modbus response data.
    /// </summary>
    public class ModbusResponseArrayData<T> where T : new()
    {
        public ModbusRequestData Request { get; set; } = new ModbusRequestData();
        public T[] Values { get; set; } = System.Array.Empty<T>();
    }
}
