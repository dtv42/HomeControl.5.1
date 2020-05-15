// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ValidationExtensions.cs" company="DTV-Online">
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
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public static class ValidationExtensions
    {
        /// <summary>
        /// Validates an object based on its DataAnnotations and throws an exception if the object is not valid.
        /// </summary>
        /// <param name="obj">The object to validate.</param>
        public static T ValidateAndThrow<T>(this T obj)
        {
            Validator.ValidateObject(obj, new ValidationContext(obj), true);
            return obj;
        }

        /// <summary>
        /// Validates an object based on its DataAnnotations and returns a list of validation errors.
        /// </summary>
        /// <param name="obj">The object to validate.</param>
        /// <returns>A list of validation errors.</returns>
        public static ICollection<ValidationResult> Validate<T>(this T obj)
        {
            var Results = new List<ValidationResult>();
            var Context = new ValidationContext(obj);
            if (!Validator.TryValidateObject(obj, Context, Results, true))
                return Results;
            return new List<ValidationResult>();
        }
    }
}