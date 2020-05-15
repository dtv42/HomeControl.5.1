// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IUdpClientSettings.cs" company="DTV-Online">
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
    public interface IUdpClientSettings
    {
        string EndPoint { get; set; }
        int Port { get; set; }
        double Timeout { get; set; }
    }
}