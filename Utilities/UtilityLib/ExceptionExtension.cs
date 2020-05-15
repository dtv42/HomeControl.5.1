// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExceptionExtension.cs" company="DTV-Online">
//   Copyright(c) 2020 Dr. Peter Trimmel. All rights reserved.
// </copyright>
// <license>
//   Licensed under the MIT license. See the LICENSE file in the project root for more information.
// </license>
// <created>13-5-2020 16:32</created>
// <author>Peter Trimmel</author>
// --------------------------------------------------------------------------------------------------------------------
namespace UtilityLib
{
    #region Using Directives

    using System;

    #endregion

    /// <summary>
    ///  Extension methods for printing exception messages to the console.
    /// </summary>
    public static class ExceptionExtension
    {
        public static void WriteToConsole(this Exception exception)
        {
            var color = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(exception.Message);
            Console.WriteLine(exception.InnerException?.Message);
            Console.ForegroundColor = color;
        }
    }
}
