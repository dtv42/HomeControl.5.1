// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ITcpClientSettings.cs" company="DTV-Online">
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
    public interface ITcpClientSettings
    {
        TcpMasterData TcpMaster { get; set; }
        TcpSlaveData TcpSlave { get; set; }
    }
}