// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestdataOptions.cs" company="DTV-Online">
//   Copyright(c) 2020 Dr. Peter Trimmel. All rights reserved.
// </copyright>
// <license>
//   Licensed under the MIT license. See the LICENSE file in the project root for more information.
// </license>
// <created>13-5-2020 18:39</created>
// <author>Peter Trimmel</author>
// --------------------------------------------------------------------------------------------------------------------
namespace UtilityApp.Models
{
    #region Using Directives

    using System;
    using System.Net;

    using UtilityLib;

    #endregion

    /// <summary>
    ///  A collection of options for the validate command.
    /// </summary>
    internal class TestdataOptions
    {
        public bool Json { get; set; }

        public bool NewData { get; set; }

        public Guid? Guid { get; set; }

        [IPAddress]
        public string? Address { get; set; }

        [IPEndPoint]
        public string? Endpoint { get; set; }

        [Uri]
        public string? Uri { get; set; }

        public HttpStatusCode? Code { get; set; }
    }
}
