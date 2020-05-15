// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PingSettings.cs" company="DTV-Online">
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
    #region Using Directives

    using System;
    using System.ComponentModel.DataAnnotations;

    #endregion

    public class PingSettings : IPingSettings
    {
        #region Public Properties

        public string Host { get; set; } = "localhost";

        [Range(0, Int32.MaxValue)]
        public int Timeout { get; set; } = 100;

        public bool DontFragment { get; set; }

        [Range(1, Int32.MaxValue)]
        public int Ttl { get; set; } = 128;

        [Range(1, Int32.MaxValue)]
        public int Roundtrip { get; set; } = 100;

        #endregion
    }
}