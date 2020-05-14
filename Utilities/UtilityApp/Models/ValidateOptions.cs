// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ValidateOptions.cs" company="DTV-Online">
//   Copyright(c) 2020 Dr. Peter Trimmel. All rights reserved.
// </copyright>
// <license>
//   Licensed under the MIT license. See the LICENSE file in the project root for more information.
// </license>
// <created>8-5-2020 08:36</created>
// <author>Peter Trimmel</author>
// --------------------------------------------------------------------------------------------------------------------
using System;

namespace UtilityApp.Models
{
    /// <summary>
    ///  A collection of options for the validate command.
    /// </summary>
    internal class ValidateOptions
    {
        #region Constructors

        /// <summary>
        ///  The constructor mapps the input parameters to the properties.
        /// </summary>
        /// <param name="d"></param>
        /// <param name="v"></param>
        /// <param name="o"></param>
        /// <param name="n"></param>
        /// <param name="r"></param>
        /// <param name="l"></param>
        /// <param name="s"></param>
        /// <param name="m"></param>
        /// <param name="e"></param>
        /// <param name="w"></param>
        /// <param name="x"></param>
        /// <param name="a"></param>
        /// <param name="p"></param>
        /// <param name="u"></param>
        public ValidateOptions(int? d = null,
                               int? v = null,
                               int? o = null,
                               int? n = null,
                               int? r = null,
                               long? l = null,
                               string? s = null,
                               string? m = null,
                               string? e = null,
                               string? w = null,
                               string? x = null,
                               string? a = null,
                               string? p = null,
                               string? u = null)
        {
            IntegerD = d;
            IntegerV = v;
            IntegerO = o;
            IntegerN = n;
            IntegerR = r;
            IntegerL = l;

            StringS = s;
            StringM = m;
            StringE = e;
            StringW = w;
            StringX = x;
            StringA = a;
            StringP = p;
            StringU = u;
        }

        #endregion

        public int? IntegerD { get; set; }
        public int? IntegerV { get; set; }
        public int? IntegerO { get; set; }
        public int? IntegerN { get; set; }
        public int? IntegerR { get; set; }
        public long? IntegerL { get; set; }

        public string? StringS { get; set; }
        public string? StringM { get; set; }
        public string? StringE { get; set; }
        public string? StringW { get; set; }
        public string? StringX { get; set; }
        public string? StringA { get; set; }
        public string? StringP { get; set; }
        public string? StringU { get; set; }
    }
}
