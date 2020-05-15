// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ArgumentExtensions.cs" company="DTV-Online">
//   Copyright(c) 2020 Dr. Peter Trimmel. All rights reserved.
// </copyright>
// <license>
//   Licensed under the MIT license. See the LICENSE file in the project root for more information.
// </license>
// <created>13-5-2020 19:38</created>
// <author>Peter Trimmel</author>
// --------------------------------------------------------------------------------------------------------------------
namespace UtilityLib
{
    #region Using Directives

    using System.CommandLine;
    using System.CommandLine.Parsing;
    using System.Linq;
    using System.Text.RegularExpressions;

    #endregion

    /// <summary>
    ///  Extension methods for command line arguments.
    /// </summary>
    public static class ArgumentExtensions
    {
        public static TArgument Name<TArgument>(this TArgument argument, string value)
            where TArgument : Argument
        {
            argument.Name = value;
            return argument;
        }

        public static TArgument Hide<TArgument>(this TArgument argument, bool value = true)
            where TArgument : Argument
        {
            argument.IsHidden = value;
            return argument;
        }

        public static TArgument Default<TArgument>(this TArgument argument, object value)
            where TArgument : Argument
        {
            argument.SetDefaultValue(value);
            return argument;
        }

        public static TArgument Arity<TArgument>(this TArgument argument, IArgumentArity arity)
            where TArgument : Argument
        {
            argument.Arity = arity;
            return argument;
        }

        public static TArgument FromAmong<TArgument>(this TArgument argument, params int[] values)
            where TArgument : Argument
        {
            argument.FromAmong(values.ToList<int>().Select(v => v.ToString()).ToArray());
            return argument;
        }

        public static TArgument Range<TArgument>(this TArgument argument, double minimum, double maximum)
            where TArgument : Argument
        {
            argument.AddValidator(r =>
            {
                var value = r.GetValueOrDefault<double>();
                if ((value >= minimum) && (value <= maximum)) return null;
                return $"{r.Symbol.Name} value must be between {minimum} and {maximum} (incl.)";
            });

            return argument;
        }

        public static TArgument Range<TArgument>(this TArgument argument, int minimum, int maximum)
            where TArgument : Argument
        {
            argument.AddValidator(r =>
            {
                var value = r.GetValueOrDefault<int>();
                if ((value >= minimum) && (value <= maximum)) return null;
                return $"{r.Symbol.Name} value must be between {minimum} and {maximum} (incl.)";
            });

            return argument;
        }

        public static TArgument Range<TArgument>(this TArgument argument, long minimum, long maximum)
            where TArgument : Argument
        {
            argument.AddValidator(r =>
            {
                var value = r.GetValueOrDefault<long>();
                if ((value >= minimum) && (value <= maximum)) return null;
                return $"{r.Symbol.Name} value must be between {minimum} and {maximum} (incl.)";
            });

            return argument;
        }

        public static TArgument StringLength<TArgument>(this TArgument argument, int length)
            where TArgument : Argument
        {
            argument.AddValidator(r =>
            {
                var value = r.GetValueOrDefault<string>();
                if (value is null) return $"{r.Symbol.Name} value is null";
                if (value.Length <= length) return null;
                return $"{r.Symbol.Name} value length must be max. {length}";
            });

            return argument;
        }

        public static TArgument NotEmpty<TArgument>(this TArgument argument)
            where TArgument : Argument
        {
            argument.AddValidator(r =>
            {
                var value = r.GetValueOrDefault<string>();
                if (value is null) return $"{r.Symbol.Name} value is null";
                if (!string.IsNullOrEmpty(value)) return null;
                return $"{r.Symbol.Name} value must not be empty";
            });

            return argument;
        }

        public static TArgument NotWhiteSpace<TArgument>(this TArgument argument)
            where TArgument : Argument
        {
            argument.AddValidator(r =>
            {
                var value = r.GetValueOrDefault<string>();
                if (value is null) return $"{r.Symbol.Name} value is null";
                if (!string.IsNullOrWhiteSpace(value)) return null;
                return $"{r.Symbol.Name} value must not be white space only";
            });

            return argument;
        }

        public static TArgument Regex<TArgument>(this TArgument argument, string pattern)
            where TArgument : Argument
        {
            argument.AddValidator(r =>
            {
                var value = r.GetValueOrDefault<string>();
                if (value is null) return $"{r.Symbol.Name} value is null";
                var regex = new Regex(pattern);
                if (regex.IsMatch(value)) return null;
                return $"{r.Symbol.Name} value must match the regular expression: {pattern}";
            });

            return argument;
        }

        public static TArgument IPAddress<TArgument>(this TArgument argument)
            where TArgument : Argument
        {
            argument.AddValidator(r =>
            {
                var value = r.GetValueOrDefault<string>();
                if (value is null) return $"{r.Symbol.Name} value is null";
                if (System.Net.IPAddress.TryParse(value, out _)) return null;
                return $"{r.Symbol.Name} value is not a valid IP address";
            });

            return argument;
        }

        public static TArgument IPEndpoint<TArgument>(this TArgument argument)
            where TArgument : Argument
        {
            argument.AddValidator(r =>
            {
                var value = r.GetValueOrDefault<string>();
                if (value is null) return $"{r.Symbol.Name} value is null";
                if (System.Net.IPEndPoint.TryParse(value, out _)) return null;
                return $"{r.Symbol.Name} value is not a valid IP endpoint";
            });

            return argument;
        }

        public static TArgument Uri<TArgument>(this TArgument argument, System.UriKind kind = System.UriKind.Absolute)
            where TArgument : Argument
        {
            argument.AddValidator(r =>
            {
                var value = r.GetValueOrDefault<string>();
                if (value is null) return $"{r.Symbol.Name} value is null";
                if (System.Uri.TryCreate(value, kind, out _)) return null;
                return $"{r.Symbol.Name} value is not a valid URI";
            });

            return argument;
        }
    }
}
