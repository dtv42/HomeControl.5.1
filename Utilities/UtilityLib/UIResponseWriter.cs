// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UIResponseWriter.cs" company="DTV-Online">
//   Copyright(c) 2020 Dr. Peter Trimmel. All rights reserved.
// </copyright>
// <license>
//   Licensed under the MIT license. See the LICENSE file in the project root for more information.
// </license>
// <created>23-4-2020 18:50</created>
// <author>Peter Trimmel</author>
// --------------------------------------------------------------------------------------------------------------------
namespace UtilityLib
{
    #region Using Directives

    using System;
    using System.Text.Json;
    using System.Text.Json.Serialization;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Diagnostics.HealthChecks;

    #endregion

    public class UIResponseWriter
    {
        #region Private Data Members

        private static readonly Lazy<JsonSerializerOptions> _options = new Lazy<JsonSerializerOptions>(() => CreateJsonOptions());

        #endregion

        public static async Task WriteResponse(HttpContext context, HealthReport report)
        {
            if (report is null)
            {
                await context.Response.WriteAsync("{}");
            }
            else
            {
                context.Response.ContentType = "application/json; charset=utf-8";
                var uiReport = UIHealthReport.CreateFrom(report);
                string json = JsonSerializer.Serialize(uiReport, _options.Value);
                await context.Response.WriteAsync(json);
            }
        }

        private static JsonSerializerOptions CreateJsonOptions()
        {
            var options = new JsonSerializerOptions()
            {
                WriteIndented = true,
                AllowTrailingCommas = true,
                PropertyNamingPolicy = null,
                IgnoreNullValues = true,
            };

            options.Converters.Add(new TimeSpanConverter());
            options.Converters.Add(new SpecialDoubleConverter());
            options.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));

            return options;
        }
    }
}
