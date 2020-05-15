// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UdpClientSettings.cs" company="DTV-Online">
//   Copyright(c) 2020 Dr. Peter Trimmel. All rights reserved.
// </copyright>
// <license>
//   Licensed under the MIT license. See the LICENSE file in the project root for more information.
// </license>
// <created>13-5-2020 13:53</created>
// <author>Peter Trimmel</author>
// --------------------------------------------------------------------------------------------------------------------
namespace UtilityLib
{
    using System.ComponentModel.DataAnnotations;

    public class UdpClientSettings : IUdpClientSettings
    {
        [IPEndPoint]
        public string EndPoint { get; set; } = string.Empty;

        [Range(0, 65535)]
        public int Port { get; set; }

        [Range(0, double.MaxValue)]
        public double Timeout { get; set; } = 1.0;
    }
}