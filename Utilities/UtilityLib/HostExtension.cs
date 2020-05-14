namespace UtilityLib
{
    #region Using Directives

    using System;
    using System.CommandLine.Parsing;
    using System.Diagnostics;
    using System.Globalization;
    using System.Threading.Tasks;

    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;

    #endregion

    /// <summary>
    ///  Extension methods for command line execution.
    /// </summary>
    public static class HostExtension
    {
        public static async Task<int> RunCommandLineAsync(this IHost host, string[] args)
        {
            using IServiceScope scope = host.Services.CreateScope();
            CultureInfo.CurrentCulture = new CultureInfo("en-US");
            var stopWatch = new Stopwatch();
            stopWatch.Start();

            try
            {
                var service = scope.ServiceProvider.GetRequiredService<ICommandLineService>();
                var parser = service.CreateParser();
                return await parser.InvokeAsync(args);
            }
            finally
            {
                stopWatch.Stop();
                Console.WriteLine($"Time elapsed {stopWatch.Elapsed}");
            }
        }
    }
}
