// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BaseClass.cs" company="DTV-Online">
//   Copyright(c) 2020 Dr. Peter Trimmel. All rights reserved.
// </copyright>
// <license>
//   Licensed under the MIT license. See the LICENSE file in the project root for more information.
// </license>
// <created>15-4-2020 18:20</created>
// <author>Peter Trimmel</author>
// --------------------------------------------------------------------------------------------------------------------
namespace UtilityLib
{
    #region Using Directives

    using Microsoft.Extensions.Logging;

    #endregion Using Directives

    /// <summary>
    /// Base class providing a logger data member.
    /// </summary>
    public class BaseClass
    {
        #region Protected Data Members

        /// <summary>
        /// The logger instance.
        /// </summary>
        protected readonly ILogger<BaseClass> _logger;

        #endregion Protected Data Members

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseClass"/> class.
        /// </summary>
        /// <param name="logger">The logger instance.</param>
        public BaseClass(ILogger<BaseClass> logger)
        {
            _logger = logger;
            _logger?.LogDebug("BaseClass()");
        }

        #endregion Constructors
    }

    /// <summary>
    /// Base class providing settings and logger data members.
    /// </summary>
    /// <typeparam name="TSettings">The settings class.</typeparam>
    public class BaseClass<TSettings> : BaseClass where TSettings : class, new()
    {
        #region Protected Data Members

        /// <summary>
        /// The settings instance.
        /// </summary>
        protected readonly TSettings _settings;

        #endregion Protected Data Members

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseClass"/> class.
        /// </summary>
        /// <param name="logger">The logger instance.</param>
        /// <param name="settings">The setting instance.</param>
        public BaseClass(TSettings settings, ILogger<BaseClass> logger) : base(logger)
        {
            _settings = settings;
            _logger?.LogDebug($"BaseClass<{typeof(TSettings)}>()");
        }

        #endregion Constructors
    }
}