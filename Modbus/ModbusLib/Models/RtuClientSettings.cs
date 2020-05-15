// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RtuClientSettings.cs" company="DTV-Online">
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
    public class RtuClientSettings : IRtuClientSettings
    {
        public RtuMasterData RtuMaster { get; set; } = new RtuMasterData();
        public RtuSlaveData RtuSlave { get; set; } = new RtuSlaveData();
    }
}