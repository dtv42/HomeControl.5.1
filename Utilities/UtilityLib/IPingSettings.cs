// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPingSettings.cs" company="DTV-Online">
//   Copyright(c) 2020 Dr. Peter Trimmel. All rights reserved.
// </copyright>
// <license>
//   Licensed under the MIT license. See the LICENSE file in the project root for more information.
// </license>
// <created>23-4-2020 19:17</created>
// <author>Peter Trimmel</author>
// --------------------------------------------------------------------------------------------------------------------
namespace UtilityLib
{
    public interface IPingSettings
    {
        string Host { get; set; }
        int Timeout { get; set; }
        bool DontFragment { get; set; }
        int Ttl { get; set; }
    }
}