// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SpecialDoubleConverter.cs" company="DTV-Online">
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

    public class SpecialDoubleConverter : JsonConverter<double>
    {
        public override double Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            => reader.TokenType switch
            {
                JsonTokenType.String => reader.GetString() switch
                {
                    "Infinity" => double.PositiveInfinity,
                    "-Infinity" => double.NegativeInfinity,
                    "NaN" => double.NaN,
                    _ => 0
                },
                JsonTokenType.Number => reader.GetDouble(),
                _ => 0
            };

        private static readonly JsonEncodedText NAN = JsonEncodedText.Encode("NaN");
        private static readonly JsonEncodedText INFINITY = JsonEncodedText.Encode("Infinity");
        private static readonly JsonEncodedText NEGATIVE_INFINITY = JsonEncodedText.Encode("-Infinity");

        public override void Write(Utf8JsonWriter writer, double value, JsonSerializerOptions options)
        {
            if (double.IsFinite(value))
            {
                writer.WriteNumberValue(value);
            }
            else
            {
                if (double.IsPositiveInfinity(value))
                {
                    writer.WriteStringValue(INFINITY);
                }
                else if (double.IsNegativeInfinity(value))
                {
                    writer.WriteStringValue(NEGATIVE_INFINITY);
                }
                else
                {
                    writer.WriteStringValue(NAN);
                }
            }
        }
    }
}
