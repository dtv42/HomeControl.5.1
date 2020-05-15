// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SettingsController.cs" company="DTV-Online">
//   Copyright(c) 2020 Dr. Peter Trimmel. All rights reserved.
// </copyright>
// <license>
//   Licensed under the MIT license. See the LICENSE file in the project root for more information.
// </license>
// <created>13-5-2020 13:52</created>
// <author>Peter Trimmel</author>
// --------------------------------------------------------------------------------------------------------------------
namespace ModbusRTU.Controllers
{
    #region Using Directives

    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;

    using Swashbuckle.AspNetCore.Annotations;

    using ModbusLib;
    using ModbusLib.Models;
    using ModbusRTU.Models;

    #endregion

    /// <summary>
    /// The Modbus Gateway MVC Controller for reading and writing settings.
    /// </summary>
    /// <para>
    ///     Get Settings
    ///     Set Settings
    /// </para>
    [Route("settings")]
    [ApiController]
    public class SettingsController : ModbusController
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsController"/> class.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="settings"></param>
        /// <param name="config"></param>
        /// <param name="environment"></param>
        /// <param name="lifetime"></param>
        /// <param name="logger"></param>
        public SettingsController(IRtuModbusClient client,
                                  AppSettings settings,
                                  IConfiguration config,
                                  IHostEnvironment environment,
                                  IHostApplicationLifetime lifetime,
                                  ILogger<SettingsController> logger)
            : base(client, settings, config, environment, lifetime, logger)
        { }

        #endregion

        /// <summary>
        /// Get the Modbus Rtu client settings.
        /// </summary>
        /// <returns>The action method result.</returns>
        /// <response code="200">Returns the client settings.</response>
        /// <response code="500">If an error or an unexpected exception occurs.</response>
        [HttpGet()]
        [SwaggerOperation(Tags = new[] { "RTU Modbus Client" })]
        [ProducesResponseType(typeof(RtuClientSettings), 200)]
        public IActionResult GetClientSettings()
        {
            var settings = new RtuClientSettings
            {
                RtuMaster = _client.RtuMaster,
                RtuSlave = _client.RtuSlave
            };

            return Ok(settings);
        }

        [HttpPut()]
        [SwaggerOperation(Tags = new[] { "RTU Modbus Client" })]
        [ProducesResponseType(typeof(RtuClientSettings), 202)]
        [ProducesResponseType(typeof(RtuClientSettings), 204)]
        [ProducesResponseType(typeof(string), 400)]
        public IActionResult SetClientSettings(RtuClientSettings data)
        {
            _client.RtuMaster = data.RtuMaster;
            _client.RtuSlave = data.RtuSlave;

            return Accepted();
        }
    }
}

