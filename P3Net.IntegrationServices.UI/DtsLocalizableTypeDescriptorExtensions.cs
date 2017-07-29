/*
 * Copyright © 2016 Federation of State Medical Boards
 * All Rights Reserved
 */
using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.DataTransformationServices.Controls;

namespace P3Net.IntegrationServices.UI
{
    /// <summary>Provides extension methods for <see cref="DTSLocalizableTypeDescriptor"/>.</summary>
    public static class DtsLocalizableTypeDescriptorExtensions
    {
        /// <summary>Gets the selected object of a DTSLocalizableTypeDescriptor if available or the original object otherwise.</summary>
        /// <param name="source">The source object.</param>
        /// <returns>The selected object or the original object.</returns>
        public static object GetSpecializedObject ( this object source )
        {
            var descriptor = source as DTSLocalizableTypeDescriptor;
            if (descriptor != null)
                return descriptor.SelectedObject;

            return source;
        }

        /// <summary>Gets the selected object of a DTSLocalizableTypeDescriptor if available or the original object otherwise.</summary>
        /// <param name="source">The source object.</param>
        /// <returns>The selected object or the original object.</returns>
        public static T GetSpecializedObject<T> ( this object source ) where T : class
        {
            return source.GetSpecializedObject() as T;
        }
    }
}
