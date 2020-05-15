// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HttpClientSettings.cs" company="DTV-Online">
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

    public class HttpClientSettings : IHttpClientSettings
    {
        #region Public Properties

        [Uri]
        public string BaseAddress { get; set; } = "http://localhost";

        [Range(0, Int32.MaxValue)]
        public int Timeout { get; set; } = 100;

        #endregion
    }
}