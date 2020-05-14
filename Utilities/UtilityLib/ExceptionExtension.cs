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
