// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GatewaySettings.cs" company="DTV-Online">
//   Copyright(c) 2020 Dr. Peter Trimmel. All rights reserved.
// </copyright>
// <license>
//   Licensed under the MIT license. See the LICENSE file in the project root for more information.
// </license>
// <created>13-5-2020 13:54</created>
// <author>Peter Trimmel</author>
// --------------------------------------------------------------------------------------------------------------------
namespace UtilityWeb.Models
{
    #region Using Directives

    using System;
    using System.ComponentModel.DataAnnotations;

    using UtilityLib;

    #endregion

    public class GatewaySettings : IGatewaySettings
    {
        #region Public Properties

        [Uri]
        public string BaseAddress { get; set; } = "http://localhost";

        [Range(0, Int32.MaxValue)]
        public int Timeout { get; set; } = 100;

        #endregion
    }
}
