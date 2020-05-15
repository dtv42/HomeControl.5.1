// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StatusCheck.cs" company="DTV-Online">
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
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Diagnostics.HealthChecks;

    #endregion

    /// <summary>
    /// 
    /// </summary>
    public class StatusCheck<TGateway> : BaseClass, IHealthCheck
        where TGateway : class, IGateway
    {
        #region Private Fields

        private readonly IHttpContextAccessor _accessor;
        private readonly TGateway _gateway;

        #endregion Private Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="StatusCheck"/> class.
        /// </summary>
        /// <param name="gateway">The EM300LR instance.</param>
        /// <param name="options">The application options.</param>
        /// <param name="logger">The application logger.</param>
        public StatusCheck(IHttpContextAccessor accessor,
                           TGateway gateway,
                           ILogger<StatusCheck<TGateway>> logger)
            : base(logger)
        {
            _accessor = accessor;
            _gateway = gateway;
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
            var parameter = _accessor?.HttpContext?.Request?.Query["access"].ToString();
            bool access = false;

            if (!string.IsNullOrEmpty(parameter) && bool.TryParse(parameter, out bool value)) access = value;

            if (access)
            {
                _gateway.CheckAccess();
            }

            if (_gateway.Status.IsGood)
            {
                return Task.FromResult(HealthCheckResult.Healthy("Gateway status is Good",
                    new Dictionary<string, object>() { { "Status", _gateway.Status } }));
            }

            if (_gateway.Status.IsUncertain)
            {
                return Task.FromResult(HealthCheckResult.Degraded("Gateway status is Uncertain", null,
                    new Dictionary<string, object>() { { "Status", _gateway.Status } }));
            }

            if (_gateway.Status.IsBad)
            {
                return Task.FromResult(HealthCheckResult.Unhealthy("Gateway status is Bad", null,
                    new Dictionary<string, object>() { { "Status", _gateway.Status } }));
            }

            return Task.FromResult(HealthCheckResult.Unhealthy());
        }

        #endregion
    }
}
