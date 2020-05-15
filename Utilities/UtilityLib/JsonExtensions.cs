// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JsonExtensions.cs" company="DTV-Online">
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

    using System.Text.Json;
    using System.Text.Json.Serialization;

    #endregion Using Directives

    /// <summary>
    ///  Extensions for supporting common Json serializer options.
    /// </summary>
    public static class JsonExtensions
    {
        /// <summary>
        ///  Common JSON serializer option.
        /// </summary>
        public static JsonSerializerOptions DefaultSerializerOptions
        {
            get
            {
                var options = new JsonSerializerOptions() { WriteIndented = true };
                options.Converters.Add(new TimeSpanConverter());
                options.Converters.Add(new SpecialDoubleConverter());
                options.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));

                return options;
            }
        }

        /// <summary>
        ///  Adding a default converter set to options. 
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public static void AddDefaultOptions(this JsonSerializerOptions options)
        {
            options.WriteIndented = true;
            options.PropertyNamingPolicy = null;
            options.Converters.Add(new TimeSpanConverter());
            options.Converters.Add(new SpecialDoubleConverter());
            options.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
        }
    }
}