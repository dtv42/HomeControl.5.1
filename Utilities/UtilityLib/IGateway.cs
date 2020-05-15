// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IGateway.cs" company="DTV-Online">
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

    using System.Threading.Tasks;

    #endregion

    public interface IGateway
    {
        bool IsStartupOk { get; }
        bool IsLocked { get; }

        DataStatus Status { get; set; }

        bool Startup();
        bool CheckAccess();
        void Lock();
        Task LockAsync();
        void Unlock();
    }
}
