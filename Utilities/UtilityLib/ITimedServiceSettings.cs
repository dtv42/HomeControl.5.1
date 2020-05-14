// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ITimedServiceSettings.cs" company="DTV-Online">
//   Copyright(c) 2020 Dr. Peter Trimmel. All rights reserved.
// </copyright>
// <license>
//   Licensed under the MIT license. See the LICENSE file in the project root for more information.
// </license>
// <created>23-4-2020 19:43</created>
// <author>Peter Trimmel</author>
// --------------------------------------------------------------------------------------------------------------------
namespace UtilityLib
{
    public interface ITimedServiceSettings
    {
        int Delay { get; set; }

        int Period { get; set; }
    }
}
