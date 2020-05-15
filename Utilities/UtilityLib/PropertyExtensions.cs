// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PropertyExtensions.cs" company="DTV-Online">
//   Copyright(c) 2020 Dr. Peter Trimmel. All rights reserved.
// </copyright>
// <license>
//   Licensed under the MIT license. See the LICENSE file in the project root for more information.
// </license>
// <created>13-5-2020 13:53</created>
// <author>Peter Trimmel</author>
// --------------------------------------------------------------------------------------------------------------------
using System;

namespace UtilityLib
{
    public static class PropertyExtensions
    {
        /// <summary>
        ///  Returns true if property with the specified name can be found on the class.
        /// </summary>
        /// <param name="type">The class type</param>
        /// <param name="name">The property name</param>
        /// <returns></returns>
        public static bool IsProperty(this Type type, string name)
        {
            if (type is null) throw new ArgumentNullException("Type cannot be null.", nameof(type));
            if (name is null) throw new ArgumentNullException("Name cannot be null.", nameof(name));

            return !(type.GetProperty(name) is null);
        }

        /// <summary>
        ///  Gets the value of the specified property.
        /// </summary>
        /// <param name="obj">The object instance</param>
        /// <param name="name">The property name</param>
        /// <returns></returns>
        public static object? GetPropertyValue(this object obj, string name)
        {
            if (obj is null) throw new ArgumentNullException("Object cannot be null.", nameof(obj));
            if (name is null) throw new ArgumentNullException("Name cannot be null.", nameof(name));

            var info = obj?.GetType().GetProperty(name);
            return info?.GetValue(obj, null);
        }

        /// <summary>
        ///  Sets the value of the specified property.
        /// </summary>
        /// <param name="obj">The object instance</param>
        /// <param name="name">The property name</param>
        /// <param name="value">The property value</param>
        public static void SetPropertyValue(this object obj, string name, object value)
        {
            if (obj is null) throw new ArgumentNullException("Object cannot be null.", nameof(obj));
            if (name is null) throw new ArgumentNullException("Name cannot be null.", nameof(name));
            if (value is null) throw new ArgumentNullException("Value cannot be null.", nameof(value));

            var info = obj?.GetType().GetProperty(name);
            info?.SetValue(obj, value);
        }
    }
}
