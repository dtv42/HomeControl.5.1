// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CoilController.cs" company="DTV-Online">
//   Copyright(c) 2020 Dr. Peter Trimmel. All rights reserved.
// </copyright>
// <license>
//   Licensed under the MIT license. See the LICENSE file in the project root for more information.
// </license>
// <created>13-5-2020 13:53</created>
// <author>Peter Trimmel</author>
// --------------------------------------------------------------------------------------------------------------------
namespace ModbusTCP.Controllers
{
    #region Using Directives

    using System.ComponentModel.DataAnnotations;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;

    using Swashbuckle.AspNetCore.Annotations;

    using ModbusLib;
    using ModbusTCP.Models;

    #endregion

    /// <summary>
    /// The Modbus Gateway MVC Controller for reading and writing coils.
    /// </summary>
    /// <para>
    ///     Read Coils                      (fc 1)
    ///     Write Single Coils              (fc 5)
    /// </para>
    [Route("[controller]")]
    [ApiController]
    public class CoilController : ModbusController
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CoilController"/> class.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="settings"></param>
        /// <param name="config"></param>
        /// <param name="environment"></param>
        /// <param name="lifetime"></param>
        /// <param name="logger"></param>
        public CoilController(ITcpModbusClient client,
                              AppSettings settings,
                              IConfiguration config,
                              IHostEnvironment environment,
                              IHostApplicationLifetime lifetime,
                              ILogger<CoilController> logger)
            : base(client, settings, config, environment, lifetime, logger)
        { }

        #endregion

        /// <summary>
        /// Reading a single coil from a Modbus slave.
        /// </summary>
        /// <param name="offset">The Modbus address (offset) of the data item.</param>
        /// <param name="slave">The slave ID of the Modbus TCP slave.</param>
        /// <returns>The action method result.</returns>
        /// <response code="200">Returns the request data and the coil.</response>
        /// <response code="400">If the request data are invalid.</response>
        /// <response code="404">If the Modbus gateway cannot connect to the slave.</response>
        /// <response code="500">If an error or an unexpected exception occurs.</response>
        /// <response code="502">If an unexpected exception occured in the slave.</response>
        [HttpGet("{offset}")]
        [SwaggerOperation(Tags = new[] { "Coils & Discrete Inputs" })]
        [ProducesResponseType(typeof(ModbusResponseData<bool>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 500)]
        [ProducesResponseType(typeof(string), 502)]
        public async Task<IActionResult> ReadCoilAsync(ushort offset = 0, byte? slave = null)
        {
            ModbusRequestData request = new ModbusRequestData()
            {
                Offset = offset,
                Number = 1
            };

            if (slave.HasValue) request.Slave.ID = slave.Value;

            return await ModbusReadRequest(request, ReadRequestFunctions.ReadCoilAsync);
        }

        /// <summary>
        /// Writing a single coil to a Modbus slave.
        /// </summary>
        /// <param name="offset">The Modbus address (offset) of the data item.</param>
        /// <param name="data">The Modbus coil data value.</param>
        /// <param name="slave">The slave ID of the Modbus TCP slave.</param>
        /// <returns>The action method result.</returns>
        /// <response code="200">Returns the request data if OK.</response>
        /// <response code="400">If the request data are invalid.</response>
        /// <response code="404">If the Modbus gateway cannot connect to the slave.</response>
        /// <response code="500">If an error or an unexpected exception occurs.</response>
        /// <response code="502">If an unexpected exception occured in the slave.</response>
        [HttpPut("{offset}")]
        [ProducesResponseType(typeof(ModbusRequestData), 200)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 500)]
        [ProducesResponseType(typeof(string), 502)]
        public async Task<IActionResult> WriteCoilAsync([FromBody, Required] bool data, ushort offset = 0, byte? slave = null)
        {
            ModbusRequestData request = new ModbusRequestData()
            {
                Offset = offset,
                Number = 1
            };

            if (slave.HasValue) request.Slave.ID = slave.Value;

            return await ModbusWriteSingleRequest(request, data, WriteRequestFunctions.WriteCoilAsync);
        }
    }
}

