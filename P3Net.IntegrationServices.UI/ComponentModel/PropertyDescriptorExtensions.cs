/*
 * Copyright © 2016 Federation of State Medical Boards
 * All Rights Reserved
 */
using System;
using System.ComponentModel;
using System.Linq;

namespace P3Net.IntegrationServices.UI.ComponentModel
{
    /// <summary>Provides extension methods for <see cref="PropertyDescriptor"/>.</summary>
    public static class PropertyDescriptorExtensions
    {
        /// <summary>Gets an attribute from the descriptor, if available.</summary>
        /// <typeparam name="T">The attribute type.</typeparam>
        /// <param name="source">The source.</param>
        /// <returns>The attribute, if found.</returns>
        public static T GetAttribute<T> ( this PropertyDescriptor source ) where T : Attribute
        {
            return source.Attributes.OfType<T>().FirstOrDefault();
        }
    }
}
