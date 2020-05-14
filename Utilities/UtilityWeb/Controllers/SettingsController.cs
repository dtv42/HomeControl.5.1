// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SettingsController.cs" company="DTV-Online">
//   Copyright(c) 2020 Dr. Peter Trimmel. All rights reserved.
// </copyright>
// <license>
//   Licensed under the MIT license. See the LICENSE file in the project root for more information.
// </license>
// <created>19-4-2020 09:19</created>
// <author>Peter Trimmel</author>
// --------------------------------------------------------------------------------------------------------------------
namespace UtilityWeb.Controllers
{
    #region Using Directives

    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;

    using UtilityLib;
    using UtilityWeb.Models;

    #endregion Using Directives

    /// <summary>
    ///  AppSettings controller providing various GET operations.
    /// </summary>
    [ApiController]
    [Route("[controller]/[action]")]
    public class SettingsController : BaseController<AppSettings>
    {
        /// <summary>
        ///  Initializes a new instance of the <see cref="SettingsController"/> class.
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="config"></param>
        /// <param name="environment"></param>
        /// <param name="lifetime"></param>
        /// <param name="logger"></param>
        public SettingsController(AppSettings settings,
                                  IConfiguration config,
                                  IHostEnvironment environment,
                                  IHostApplicationLifetime lifetime,
                                  ILogger<SettingsController> logger)
            : base(settings, config, environment, lifetime, logger)
        {
            _logger.LogDebug("SettingsController()");
        }

        [HttpGet]
        [ActionName("")]
        [Produces("application/json")]
        public IActionResult Get()
        {
            return Ok(_settings);
        }

        [HttpGet]
        [ActionName("String")]
        [Produces("application/json")]
        public IActionResult GetStringValue()
        {
            return Ok(_settings.StringValue);
        }

        [HttpGet]
        [ActionName("Boolean")]
        [Produces("application/json")]
        public IActionResult GetBooleanValue()
        {
            return Ok(_settings.BooleanValue);
        }

        [HttpGet]
        [ActionName("Integer")]
        [Produces("application/json")]
        public IActionResult GetIntegerValue()
        {
            return Ok(_settings.IntegerValue);
        }

        [HttpGet]
        [ActionName("Long")]
        [Produces("application/json")]
        public IActionResult GetLongValue()
        {
            return Ok(_settings.LongValue);
        }

        [HttpGet]
        [ActionName("Float")]
        [Produces("application/json")]
        public IActionResult GetFloatValue()
        {
            return Ok(_settings.FloatValue);
        }

        [HttpGet]
        [ActionName("Double")]
        [Produces("application/json")]
        public IActionResult GetDoubleValue()
        {
            return Ok(_settings.DoubleValue);
        }

        [HttpGet]
        [ActionName("Decimal")]
        [Produces("application/json")]
        public IActionResult GetDecimalValue()
        {
            return Ok(_settings.DecimalValue);
        }

        [HttpGet]
        [ActionName("DateTime")]
        [Produces("application/json")]
        public IActionResult GetDateTimeValue()
        {
            return Ok(_settings.DateTimeValue);
        }

        [HttpGet]
        [ActionName("DateTimeOffset")]
        [Produces("application/json")]
        public IActionResult GetDateTimeOffsetValue()
        {
            return Ok(_settings.DateTimeOffsetValue);
        }

        [HttpGet]
        [ActionName("StringList")]
        [Produces("application/json")]
        public IActionResult GetStringList()
        {
            return Ok(_settings.StringList);
        }

        [HttpGet]
        [ActionName("BooleanList")]
        [Produces("application/json")]
        public IActionResult GetBooleanList()
        {
            return Ok(_settings.BooleanList);
        }

        [HttpGet]
        [ActionName("IntegerList")]
        [Produces("application/json")]
        public IActionResult GetIntegerList()
        {
            return Ok(_settings.IntegerList);
        }

        [HttpGet]
        [ActionName("LongList")]
        [Produces("application/json")]
        public IActionResult GetLongList()
        {
            return Ok(_settings.LongList);
        }

        [HttpGet]
        [ActionName("FloatList")]
        [Produces("application/json")]
        public IActionResult GetFloatList()
        {
            return Ok(_settings.FloatList);
        }

        [HttpGet]
        [ActionName("DoubleList")]
        [Produces("application/json")]
        public IActionResult GetDoubleList()
        {
            return Ok(_settings.DoubleList);
        }

        [HttpGet]
        [ActionName("DecimalList")]
        [Produces("application/json")]
        public IActionResult GetDecimalList()
        {
            return Ok(_settings.DecimalList);
        }

        [HttpGet]
        [ActionName("DateTimeList")]
        [Produces("application/json")]
        public IActionResult GetDateTimeList()
        {
            return Ok(_settings.DateTimeList);
        }

        [HttpGet]
        [ActionName("DateTimeOffsetList")]
        [Produces("application/json")]
        public IActionResult GetDateTimeOffsetList()
        {
            return Ok(_settings.DateTimeOffsetList);
        }

        [HttpGet("{i}")]
        [ActionName("StringList")]
        [Produces("application/json")]
        public IActionResult GetStringList(ushort i)
        {
            if (i < _settings.StringList.Count)
                return Ok(_settings.StringList[i]);
            else
                return NotFound();
        }

        [HttpGet("{i}")]
        [ActionName("BooleanList")]
        [Produces("application/json")]
        public IActionResult GetBooleanList(ushort i)
        {
            if (i < _settings.BooleanList.Count)
                return Ok(_settings.BooleanList[i]);
            else
                return NotFound();
        }

        [HttpGet("{i}")]
        [ActionName("IntegerList")]
        [Produces("application/json")]
        public IActionResult GetIntegerList(ushort i)
        {
            if (i < _settings.IntegerList.Count)
                return Ok(_settings.IntegerList[i]);
            else
                return NotFound();
        }

        [HttpGet("{i}")]
        [ActionName("LongList")]
        [Produces("application/json")]
        public IActionResult GetLongList(ushort i)
        {
            if (i < _settings.LongList.Count)
                return Ok(_settings.LongList[i]);
            else
                return NotFound();
        }

        [HttpGet("{i}")]
        [ActionName("FloatList")]
        [Produces("application/json")]
        public IActionResult GetFloatList(ushort i)
        {
            if (i < _settings.FloatList.Count)
                return Ok(_settings.FloatList[i]);
            else
                return NotFound();
        }

        [HttpGet("{i}")]
        [ActionName("DoubleList")]
        [Produces("application/json")]
        public IActionResult GetDoubleList(ushort i)
        {
            if (i < _settings.DoubleList.Count)
                return Ok(_settings.DoubleList[i]);
            else
                return NotFound();
        }

        [HttpGet("{i}")]
        [ActionName("DecimalList")]
        [Produces("application/json")]
        public IActionResult GetDecimalList(ushort i)
        {
            if (i < _settings.DecimalList.Count)
                return Ok(_settings.DecimalList[i]);
            else
                return NotFound();
        }

        [HttpGet("{i}")]
        [ActionName("DateTimeList")]
        [Produces("application/json")]
        public IActionResult GetDateTimeList(ushort i)
        {
            if (i < _settings.DateTimeList.Count)
                return Ok(_settings.DateTimeList[i]);
            else
                return NotFound();
        }

        [HttpGet("{i}")]
        [ActionName("DateTimeOffsetList")]
        [Produces("application/json")]
        public IActionResult GetDateTimeOffsetList(ushort i)
        {
            if (i < _settings.DateTimeOffsetList.Count)
                return Ok(_settings.DateTimeOffsetList[i]);
            else
                return NotFound();
        }

        [HttpGet]
        [ActionName("Dictionary")]
        [Produces("application/json")]
        public IActionResult GetDictionary()
        {
            return Ok(_settings.Dictionary);
        }

        [HttpGet("{i}")]
        [ActionName("Dictionary")]
        [Produces("application/json")]
        public IActionResult GetDictionary(ushort i)
        {
            if (i < _settings.Dictionary.Count)
                return Ok(new KeyValuePair<string, string>
                (_settings.Dictionary.Keys.ToArray()[i],
                 _settings.Dictionary.Values.ToArray()[i]));
            else
                return NotFound();
        }

        [HttpGet]
        [ActionName("Settings")]
        [Produces("application/json")]
        public IActionResult GetSettings()
        {
            return Ok(_settings.Settings);
        }
    }
}