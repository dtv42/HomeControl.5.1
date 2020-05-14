// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPEndPointAttribute.cs" company="DTV-Online">
//   Copyright(c) 2020 Dr. Peter Trimmel. All rights reserved.
// </copyright>
// <license>
//   Licensed under the MIT license. See the LICENSE file in the project root for more information.
// </license>
// <created>17-4-2020 15:51</created>
// <author>Peter Trimmel</author>
// --------------------------------------------------------------------------------------------------------------------
namespace UtilityLib
{
    #region Using Directives

    using System.ComponentModel.DataAnnotations;
    using System.Net;

    #endregion Using Directives

    /// <summary>
    ///  Validates an IPEndpoint option value.
    /// </summary>
    public sealed class IPEndPointAttribute : ValidationAttribute
    {
        public IPEndPointAttribute()
            : base("The IPEndPoint value must be valid: <address>:<port>")
        {
        }

        protected override ValidationResult IsValid(object value, ValidationContext context)
        {
            if ((value is null) || !IPEndPoint.TryParse((string)value, out IPEndPoint? _))
            {
                return new ValidationResult(FormatErrorMessage(context.DisplayName));
            }

            return ValidationResult.Success;
        }
    }
}