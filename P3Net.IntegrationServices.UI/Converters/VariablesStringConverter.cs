/*
 * Copyright © 2016 Federation of State Medical Boards
 * All Rights Reserved
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using P3Net.IntegrationServices.UI.ComponentModel;
using Microsoft.SqlServer.Dts.Runtime;
using Microsoft.SqlServer.Dts.Runtime.Design;

namespace P3Net.IntegrationServices.UI.Converters
{
    /// <summary>Provides a converter for variables.</summary>
    /// <remarks>
    /// View properties can use this converter to provide a user with the list of available variables and the ability to 
    /// add a new variable.  To support adding a new variable ensure the view is derived from <see cref="DtsTaskUIPropertyView{T}"/>.
    /// </remarks>
    public class VariablesStringConverter : NewItemStringConverter
    {
        /// <summary>Provides the value that can be used to identify a new variable.</summary>
        public override string NewItemText
        {
            get { return "<New Variable>"; }
        }

        public override object ProcessNewItem ( INewItemContext context )
        {
            var defaultValues = GetNewVariableDefaults(context.TargetProperty, context.Target);
            var newVariable = context.Variables.PromptAndCreateVariable(context.ParentWindow, context.Container, defaultValues.Name, defaultValues.Scope, defaultValues.Type);

            if (newVariable != null)
                return newVariable.QualifiedName;

            return null;
        }

        protected override IEnumerable<string> GetValueList ( ITypeDescriptorContext context )
        {
            var provider = context.Instance.GetSpecializedObject<IDtsVariablesProvider>();
            if (provider != null)
            {
                return from v in provider.Variables.OfType<Variable>()
                       orderby v.QualifiedName
                       select v.QualifiedName;
            };

            return null;
        }

        #region Private Members

        private NewVariableAttribute GetNewVariableDefaults ( PropertyDescriptor property, object target )
        {
            var defaultAttr = property.GetAttribute<NewVariableAttribute>();

            if (defaultAttr != null)
                return defaultAttr;

            //Look for the interface
            var ifc = target as INewVariableProvider;
            if (ifc != null)
                return ifc.GetNewVariable(property.Name);

            return new NewVariableAttribute(property.Name, typeof(int));
        }
        #endregion
    }
}
