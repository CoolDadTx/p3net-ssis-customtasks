/*
 * Copyright © 2016 Federation of State Medical Boards
 * All Rights Reserved
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

using Microsoft.SqlServer.Dts.Runtime;

namespace P3Net.IntegrationServices.UI.Converters
{
    /// <summary>Converter for working with connections.</summary>
    /// <remarks>
    /// View properties can use this converter to provide a user with the list of available HTTP connections and the ability to 
    /// add a new connection.  To support adding a new connection ensure the view is derived from <see cref="DtsTaskUIPropertyView{T}"/>.
    /// </remarks>
    public class ValueStringConverter : NewItemStringConverter
    {

        /// <summary>Specifies the type of connection to support.</summary>

        /// <summary>Provides the value that can be used to identify a new connection.</summary>
        public override string NewItemText
        {
            get { return null; }
        }
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            var instance = context.Instance.GetSpecializedObject<IDtsListProvider>();
            return (instance?.ValueList != null && instance.ValueList.Count() > 0);
        }

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            var instance = context.Instance.GetSpecializedObject<IDtsListProvider>();
            return (instance?.ValueList != null && instance.ValueList.Count() > 0);
        }


        protected override IEnumerable<string> GetValueList ( ITypeDescriptorContext context )
        {
            var instance = context.Instance.GetSpecializedObject<IDtsListProvider>();
            if (instance != null)
            {
                return instance.ValueList;
            }
            return null;

        }

        public override object ProcessNewItem ( INewItemContext context )
        {
            return null;
        }
    }
}
