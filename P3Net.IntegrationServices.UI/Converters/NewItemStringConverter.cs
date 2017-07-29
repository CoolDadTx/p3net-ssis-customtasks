using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace P3Net.IntegrationServices.UI.Converters
{
    /// <summary>Provides a base class for working with string converters that can create new objects.</summary>    
    public abstract class NewItemStringConverter : StringConverter
    {
        /// <summary>Gets the text for a new item.</summary>
        public abstract string NewItemText { get; }
        
        /// <summary>Determines if the given value is the <see cref="NewItemText"/>.</summary>
        /// <param name="value">The value to check.</param>
        /// <returns><see langword="true"/> if they match.</returns>
        public bool IsNewItem ( string value )
        {
            return String.Compare(value, NewItemText, true) == 0;
        }

        /// <summary>Gets the standard values.</summary>
        /// <param name="context">The context.</param>
        /// <returns>The list of values.</returns>
        /// <remarks>
        /// This converter returns all the existing variables and an option to create a new variable.
        /// </remarks>
        public override TypeConverter.StandardValuesCollection GetStandardValues ( ITypeDescriptorContext context )
        {           
            var values = new[] { NewItemText }.Union(GetValueList(context) ?? new string[0]);
            
            return new TypeConverter.StandardValuesCollection(values.ToArray());
        }

        /// <summary>Determines if the values are exclusive.</summary>
        /// <param name="context">The context.</param>
        /// <returns>Returns <see langword="true"/>.</returns>
        public override bool GetStandardValuesExclusive ( ITypeDescriptorContext context )
        {
            return true;
        }

        /// <summary>Determines if standard values are supported.</summary>
        /// <param name="context">The context.</param>
        /// <returns>Returns <see langword="true"/>.</returns>
        public override bool GetStandardValuesSupported ( ITypeDescriptorContext context )
        {
            return true;
        }

        /// <summary>Processes a request for a new item.</summary>
        /// <param name="context">The context for the new item.</param>
        /// <returns>The new item value, or <see langword="null"/> if none is selected.</returns>
        public abstract object ProcessNewItem ( INewItemContext context );

        #region Protected Members

        /// <summary>Gets the list of values to include in the list.</summary>
        /// <param name="context">The context.</param>
        /// <returns>The list of values.</returns>
        /// <remarks>
        /// The value from <see cref="NewItemDisplayText"/> is automatically added to the beginning of the list.
        /// </remarks>
        protected abstract IEnumerable<string> GetValueList ( ITypeDescriptorContext context );
        #endregion
    }
}
