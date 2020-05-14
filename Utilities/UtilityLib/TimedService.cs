// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TimedService.cs" company="DTV-Online">
//   Copyright(c) 2020 Dr. Peter Trimmel. All rights reserved.
// </copyright>
// <license>
//   Licensed under the MIT license. See the LICENSE file in the project root for more information.
// </license>
// <created>19-4-2020 08:42</created>
// <author>Peter Trimmel</author>
// --------------------------------------------------------------------------------------------------------------------
namespace UtilityLib
{
    #region Using Directives

    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;

    #endregion Using Directives

    /// <summary>
    /// Timed background task based on code from https://docs.microsoft.com/en-us/aspnet/core/fundamentals/host/hosted-services?view=aspnetcore-2.1
    /// </summary>
    public abstract class TimedService : BackgroundService
    {
        #region Private Data Members

        private readonly TimedServiceSettings _settings;

        #endregion Private Data Members

        #region Protected Data Members

        protected readonly ILogger<TimedService> _logger;
        protected readonly IHostApplicationLifetime _lifetime;
        protected readonly IHostEnvironment _environment;
        protected readonly IConfiguration _config;

        #endregion Protected Data Members

        #region Constructors

        /// <summary>
        ///  Initializes a new instance of the <see cref="TimedService"/> class using dependency injection.
        ///  Note that the settings are specific to the TimedService and not AppSettings.
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="config"></param>
        /// <param name="environment"></param>
        /// <param name="lifetime"></param>
        /// <param name="logger"></param>
        protected TimedService(TimedServiceSettings settings,
                               IConfiguration config,
                               IHostEnvironment environment,
                               IHostApplicationLifetime lifetime,
                               ILogger<TimedService> logger)
        {
            _settings = settings;
            _config = config;
            _environment = environment;
            _lifetime = lifetime;
            _logger = logger;

            _logger?.LogDebug("TimedService()");
        }

        #endregion Constructors

        #region Public Methods

        /// <summary>
        ///  Start the async operation. A timer is used to spin off the initialization (DoStartAsync).
        /// </summary>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>A <see cref="Task"/> object that represents an asynchronous operation.</returns>
        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger?.LogInformation("TimedService starting.");
            await base.StartAsync(cancellationToken);
            await Task.Delay(_settings.Delay);
            await DoStart();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="stoppingToken"></param>
        /// <returns></returns>
        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            _logger?.LogInformation("TimedService executing.");

            while (!cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(_settings.Period, cancellationToken);
                await DoWork();
            }
        }

        /// <summary>
        ///  Stop the async operation. All timers are stopped.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>A <see cref="Task"/> object that represents an asynchronous operation.</returns>
        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger?.LogInformation("TimedService stopping.");
            await DoStop();
            await base.StopAsync(cancellationToken);
        }

        #endregion Public Methods

        #region Virtual Methods

        /// <summary>
        ///  Derived classes should override this.
        /// </summary>
        protected virtual async Task DoStart()
            => await Task.Delay(1);

        /// <summary>
        /// Derived classes should override this.
        /// </summary>
        protected virtual async Task DoWork()
            => await Task.Delay(1);

        /// <summary>
        ///  Derived classes should override this.
        /// </summary>
        protected virtual async Task DoStop()
            => await Task.Delay(1);

        #endregion Virtual Methods
    }
}