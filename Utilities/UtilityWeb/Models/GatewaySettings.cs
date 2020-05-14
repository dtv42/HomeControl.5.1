namespace UtilityWeb.Models
{
    #region Using Directives

    using System;
    using System.ComponentModel.DataAnnotations;

    using UtilityLib;

    #endregion

    public class GatewaySettings : IGatewaySettings
    {
        #region Public Properties

        [Uri]
        public string BaseAddress { get; set; } = "http://localhost";

        [Range(0, Int32.MaxValue)]
        public int Timeout { get; set; } = 100;

        #endregion
    }
}
