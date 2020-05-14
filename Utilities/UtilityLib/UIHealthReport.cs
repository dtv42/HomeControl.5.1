// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UIHealthReport.cs" company="DTV-Online">
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
    using System.Collections.Generic;

    using Microsoft.Extensions.Diagnostics.HealthChecks;

    #endregion

    /// <summary>
    /// 
    /// </summary>
    public class UIHealthReport
    {
        #region Public Properties

        public UIHealthStatus Status { get; set; }
        public TimeSpan TotalDuration { get; set; }
        public Dictionary<string, UIHealthReportEntry> Entries { get; } = new Dictionary<string, UIHealthReportEntry> { };

        #endregion

        public UIHealthReport() { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entries"></param>
        /// <param name="totalDuration"></param>
        public UIHealthReport(Dictionary<string, UIHealthReportEntry> entries, TimeSpan totalDuration)
        {
            Entries = entries;
            TotalDuration = totalDuration;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="report"></param>
        /// <returns></returns>
        public static UIHealthReport CreateFrom(HealthReport report)
        {
            var uiReport = new UIHealthReport(new Dictionary<string, UIHealthReportEntry>(), report.TotalDuration)
            {
                Status = (UIHealthStatus)report.Status,
            };

            foreach (var item in report.Entries)
            {
                var entry = new UIHealthReportEntry
                {
                    Data = item.Value.Data,
                    Description = item.Value.Description ?? string.Empty,
                    Duration = item.Value.Duration,
                    Status = (UIHealthStatus)item.Value.Status
                };

                if (item.Value.Exception != null)
                {
                    var message = item.Value.Exception?
                        .Message
                        .ToString();

                    entry.Exception = message ?? string.Empty;
                    entry.Description = item.Value.Description ?? message ?? string.Empty;
                }

                uiReport.Entries.Add(item.Key, entry);
            }

            return uiReport;
        }
        public static UIHealthReport CreateFrom(Exception exception, string entryName = "Endpoint")
        {
            var uiReport = new UIHealthReport(new Dictionary<string, UIHealthReportEntry>(), TimeSpan.FromSeconds(0))
            {
                Status = UIHealthStatus.Unhealthy,
            };

            uiReport.Entries.Add(entryName, new UIHealthReportEntry
            {
                Exception = exception.Message,
                Description = exception.Message,
                Duration = TimeSpan.FromSeconds(0),
                Status = UIHealthStatus.Unhealthy
            });

            return uiReport;
        }
    }

    public enum UIHealthStatus
    {
        Unhealthy = 0,
        Degraded = 1,
        Healthy = 2
    }

    public class UIHealthReportEntry
    {
        public IReadOnlyDictionary<string, object> Data { get; set; } = new Dictionary<string, object> { };
        public string Description { get; set; } = string.Empty;
        public TimeSpan Duration { get; set; }
        public string Exception { get; set; } = string.Empty;
        public UIHealthStatus Status { get; set; }
    }
}