namespace UtilityApp.Models
{
    #region Using Directives

    using System;
    using System.Net;

    using UtilityLib;

    #endregion

    /// <summary>
    ///  A collection of options for the validate command.
    /// </summary>
    internal class TestdataOptions
    {
        public bool Json { get; set; }

        public bool NewData { get; set; }

        public Guid? Guid { get; set; }

        [IPAddress]
        public string? Address { get; set; }

        [IPEndPoint]
        public string? Endpoint { get; set; }

        [Uri]
        public string? Uri { get; set; }

        public HttpStatusCode? Code { get; set; }
    }
}
