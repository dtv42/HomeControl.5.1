// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPAddressAttribute.cs" company="DTV-Online">
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

    using System.ComponentModel.DataAnnotations;
    using System.Net;

    #endregion Using Directives

    /// <summary>
    ///  Validates an IPAddress option value.
    /// </summary>
    public sealed class IPAddressAttribute : ValidationAttribute
    {
        public IPAddressAttribute()
            : base("The IPAddress value must be valid: <xxx.xxx.xxx.xxx>")
        {
        }

        protected override ValidationResult IsValid(object value, ValidationContext context)
        {
            if ((value is null) || !IPAddress.TryParse((string)value, out IPAddress? _))
            {
                return new ValidationResult(FormatErrorMessage(context.DisplayName));
            }

            return ValidationResult.Success;
        }
    }
}