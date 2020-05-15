// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PingCheck.cs" company="DTV-Online">
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

    using System.Collections.Generic;
    using System.Net.NetworkInformation;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Diagnostics.HealthChecks;

    #endregion

    /// <summary>
    /// 
    /// </summary>
    public class PingCheck : BaseClass<PingSettings>, IHealthCheck
    {
        #region Private Data Members

        private readonly IHttpContextAccessor _accessor;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="StatusCheck"/> class.
        /// </summary>
        /// <param name="options">The application options.</param>
        /// <param name="logger">The application logger.</param>
        public PingCheck(IHttpContextAccessor accessor,
                         PingSettings settings,
                         ILogger<PingCheck> logger)
            : base(settings, logger)
        {
            _accessor = accessor;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Checks the current health state using the gateway Status property.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            var parameter = _accessor?.HttpContext?.Request?.Query["time"].ToString();
            int time = _settings.Roundtrip;

            if (!string.IsNullOrEmpty(parameter) && int.TryParse(parameter, out int value))
            {
                time = (value > 0) ? value : time;
            }

            try
            {
                Ping sender = new Ping();
                var options = new PingOptions(_settings.Ttl, _settings.DontFragment);
                var buffer = Encoding.ASCII.GetBytes(new string('x', 56));
                var reply = sender.Send(_settings.Host, _settings.Timeout, buffer, options);

                if (reply.Status == IPStatus.Success)
                {
                    if (reply.RoundtripTime > time)
                    {
                        return Task.FromResult(HealthCheckResult.Degraded($"Gateway ping OK ({reply.RoundtripTime} msec)", null,
                            new Dictionary<string, object>() { { "Status", reply.Status } }));
                    }

                    return Task.FromResult(HealthCheckResult.Healthy("Gateway ping OK",
                        new Dictionary<string, object>() { { "Status", reply.Status } }));
                }
                else
                {
                    return Task.FromResult(HealthCheckResult.Unhealthy("Gateway ping not OK", null,
                        new Dictionary<string, object>() { { "Status", reply.Status } }));
                }
            }
            catch (PingException pex)
            {
                return Task.FromResult(HealthCheckResult.Unhealthy("Gateway ping exception", pex));
            }
        }

        #endregion
    }
}
