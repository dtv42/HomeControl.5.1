// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TimedServiceSettings.cs" company="DTV-Online">
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

    /// <summary>
    ///  TimedService specific settings.
    /// </summary>
    public class TimedServiceSettings : ITimedServiceSettings
    {
        [Range(0, Int32.MaxValue)]
        public int Delay { get; set; }

        [Range(0, Int32.MaxValue)]
        public int Period { get; set; } = 1000;
    }
}
