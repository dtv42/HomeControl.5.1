// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NumberConverter.cs" company="DTV-Online">
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
    using System.Text.Json;
    using System.Text.Json.Serialization;

    #endregion

    /// <summary>
    /// This converter is used to convert number values into a string (e.g. 1 => "1").
    /// </summary>
    public class NumberConverter : JsonConverter<string>
    {
        public override string Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            => reader.TokenType switch
            {
                JsonTokenType.Number => reader.GetInt32().ToString(),
                _ => reader.GetString() ?? string.Empty
            };


        public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
            => writer.WriteStringValue(value);
    }
}
