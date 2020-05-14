// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AbsoluteUriAttribute.cs" company="DTV-Online">
//   Copyright(c) 2020 Dr. Peter Trimmel. All rights reserved.
// </copyright>
// <license>
//   Licensed under the MIT license. See the LICENSE file in the project root for more information.
// </license>
// <created>17-4-2020 15:53</created>
// <author>Peter Trimmel</author>
// --------------------------------------------------------------------------------------------------------------------
namespace UtilityLib
{
    #region Using Directives

    using System;
    using System.ComponentModel.DataAnnotations;

    #endregion Using Directives

    /// <summary>
    ///  Validates an abolute Uri option value.
    /// </summary>
    public sealed class UriAttribute : ValidationAttribute
    {
        public UriAttribute(UriKind kind = UriKind.Absolute, bool httpOnly = true)
            : base("The Uri value must be valid: '<scheme>//<host>:<port>'")
        {
            Kind = kind;
            HttpOnly = httpOnly;
        }

        public UriKind Kind { get; }
        public bool HttpOnly { get; }

        protected override ValidationResult IsValid(object value, ValidationContext context)
        {
            if ((value is null) || !Uri.TryCreate((string)value, Kind, out Uri? uri))
            {
                return new ValidationResult(FormatErrorMessage(context.DisplayName));
            }

            return HttpOnly ? ValidationResult.Success : uri.Scheme.ToLower() switch
            {
                "http" => ValidationResult.Success,
                "https" => ValidationResult.Success,
                _ => new ValidationResult(FormatErrorMessage(context.DisplayName)),
            };
        }
    }
}