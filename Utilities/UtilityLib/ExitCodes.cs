// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExitCodes.cs" company="DTV-Online">
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

    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.InteropServices;

    #endregion Using Directives

    /// <summary>
    ///  Helper class to define standard program exit codes.
    /// </summary>
    public static class ExitCodes
    {
        #region Constants

        // values from http://www.febooti.com/products/automation-workshop/online-help/events/run-dos-cmd-command/exit-codes/
        public const int SuccessfullyCompleted = 0;

        public const int IncorrectFunction = 1;
        public const int FileNotFound = 2;
        public const int PathNotFound = 3;
        public const int CantOpenFile = 4;
        public const int AccessDenied = 5;
        public const int InvalidData = 13;
        public const int ProgramNotRecognized = 9009;
        public const int NotSuccessfullyCompleted = -1;

        #endregion Constants

        #region Public Statics

        public static readonly int OperationCanceled =
            RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? unchecked((int)0xC000013A) : 130; // SIGINT

        public static readonly int UnhandledException =
            RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? unchecked((int)0xE0434F4D) : 134;  // SIGABRT

        #endregion Public Statics
    }
}