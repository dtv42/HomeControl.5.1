// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BaseGateway.cs" company="DTV-Online">
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
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.Extensions.Logging;

    #endregion Using Directives

    /// <summary>
    ///
    /// </summary>
    /// <typeparam name="TSettings">The settings class.</typeparam>
    public class BaseGateway<TSettings> : BaseClass<TSettings>, IGateway
        where TSettings : class, new()
    {
        #region Private Data Members

        /// <summary>
        ///  Instantiate a Singleton of the Semaphore with a value of 1.
        ///  This means that only 1 thread can be granted access at a time.
        /// </summary>
        private static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        /// <summary>
        ///  The data status is modified using the public property Status
        ///  (updating the timestamp automatically).
        /// </summary>
        private DataStatus _status = new DataStatus();

        #endregion Private Data Members

        #region Public Properties

        public bool IsStartupOk { get => true; }

        public bool IsLocked { get => (_semaphore.CurrentCount == 0); }

        public DataStatus Status 
        { 
            get => _status;
            set
            {
                _status = value;
                _status.Timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();
            }
        }

        #endregion Public Properties

        #region Constructors

        /// <summary>
        ///
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="options"></param>
        public BaseGateway(TSettings settings, ILogger<BaseGateway<TSettings>> logger) : base(settings, logger)
        {
            _logger?.LogDebug($"BaseGateway<{typeof(TSettings)}>()");
        }

        #endregion Constructors

        #region Public Methods

        public virtual bool Startup() => true;

        public virtual bool CheckAccess() => true;

        /// <summary>
        ///
        /// </summary>
        public void Lock()
        {
            _logger.LogTrace("Enter semaphore");
            _semaphore.Wait();
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public async Task LockAsync()
        {
            _logger.LogTrace("Enter semaphore");
            await _semaphore.WaitAsync();
        }

        /// <summary>
        ///
        /// </summary>
        public void Unlock()
        {
            _logger.LogTrace("Release semaphore");
            _semaphore.Release();
        }

        #endregion Public Methods
    }
}