// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IHttpClientSettings.cs" company="DTV-Online">
//   Copyright(c) 2020 Dr. Peter Trimmel. All rights reserved.
// </copyright>
// <license>
//   Licensed under the MIT license. See the LICENSE file in the project root for more information.
// </license>
// <created>22-4-2020 12:54</created>
// <author>Peter Trimmel</author>
// --------------------------------------------------------------------------------------------------------------------
namespace UtilityLib
{
    public interface IHttpClientSettings
    {
        string BaseAddress { get; set; }
        int Timeout { get; set; }
    }
}