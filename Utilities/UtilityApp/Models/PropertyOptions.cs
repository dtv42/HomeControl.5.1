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
namespace UtilityApp.Models
{
    /// <summary>
    ///  A collection of options for the validate command.
    /// </summary>
    internal class PropertyOptions
    {
        #region Constructors

        /// <summary>
        ///  The constructor mapps the input parameters to the properties.
        /// </summary>
        /// <param name="pn"></param>
        /// <param name="pv"></param>
        /// <param name="p"></param>
        /// <param name="s"></param>
        /// <param name="a"></param>
        /// <param name="l"></param>
        /// <param name="d"></param>
        /// <param name="v"></param>
        public PropertyOptions(string name = "",
                               string value = "", 
                               bool p = false,
                               bool s = false,
                               bool a = false,
                               bool l = false,
                               bool d = false,
                               bool v = false)
        {
            PropertyName = name;
            PropertyValue = value;

            ShowAll = p;
            ShowSimple = s;
            ShowArrays = a;
            ShowLists = l;
            ShowDictionaries = d;
            ShowValue = v;
        }

        #endregion

        public string PropertyName { get; set; }
        public string PropertyValue { get; set; }
        public bool ShowAll { get; set; }
        public bool ShowSimple { get; set; }
        public bool ShowArrays { get; set; }
        public bool ShowLists { get; set; }
        public bool ShowDictionaries { get; set; }
        public bool ShowValue { get; set; }
    }
}
