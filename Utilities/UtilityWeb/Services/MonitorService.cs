// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MonitorService.cs" company="DTV-Online">
//   Copyright(c) 2020 Dr. Peter Trimmel. All rights reserved.
// </copyright>
// <license>
//   Licensed under the MIT license. See the LICENSE file in the project root for more information.
// </license>
// <created>19-4-2020 11:08</created>
// <author>Peter Trimmel</author>
// --------------------------------------------------------------------------------------------------------------------
namespace UtilityWeb.Services
{
    #region Using Directives

    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Threading.Tasks;

    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;

    using UtilityLib;
    using UtilityWeb.Models;

    #endregion Using Directives

    /// <summary>
    ///  Example of a timed service to update values in the AppSettings instance.
    /// </summary>
    public class MonitorService : TimedService
    {
        #region Private Data Members

        private readonly TestData _testdata;
        private readonly Random _random;
        private readonly int _minimum;
        private readonly int _maximum;

        #endregion Private Data Members

        /// <summary>
        ///  Initializes a new instance of the <see cref="MonitorService"/> class.
        ///  Set the period to 10 seconds.
        /// </summary>
        /// <param name="testdata"></param>
        /// <param name="config"></param>
        /// <param name="environment"></param>
        /// <param name="lifetime"></param>
        /// <param name="logger"></param>
        public MonitorService(TestData testdata,
                              IConfiguration config,
                              IHostEnvironment environment,
                              IHostApplicationLifetime lifetime,
                              ILogger<MonitorService> logger)
            : base(new TimedServiceSettings() { Period = 10000 }, config, environment, lifetime, logger)
        {
            _random = new Random();
            _minimum = 0;
            _maximum = 100;

            _testdata = testdata;

            // Getting the range values from the attribute.
            var attributes = typeof(TestData).GetProperty("Value").GetCustomAttributes(true);

            foreach (object attribute in attributes)
            {
                if (attribute is RangeAttribute range)
                {
                    _minimum = (int)range.Minimum;
                    _maximum = (int)range.Maximum;
                    break;
                }
            }

            _logger.LogDebug("MonitorService()");
        }

        /// <summary>
        ///  Updates a field value (every 10 seconds).
        /// </summary>
        /// <returns></returns>
        protected override async Task DoWork()
        {
            await Task.Run(() => _testdata.Value = _random.Next(_minimum, _maximum));
        }
    }
}