/*
 * Copyright © 2016 Federation of State Medical Boards
 * All Rights Reserved
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using P3Net.IntegrationServices.UI;
using P3Net.IntegrationServices.UI.ComponentModel;
using P3Net.IntegrationServices.UI.Converters;
using Microsoft.DataTransformationServices.Controls;
using Microsoft.SqlServer.Dts.Runtime;

namespace P3Net.IntegrationServices.Tasks.GenerateSsrs.UI
{
    [SortProperties(new string[] { "Type", "IsNullable", "DefaultValue", "ValueType", "Variable", "Value" })]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    internal class ParameterNode : ICustomTypeDescriptor, IDtsVariablesProvider, INewVariableProvider
    {
        #region Construction

        public ParameterNode ( Variables variables, ReportArgument parameter )
        {
            m_variables = variables;

            if (parameter != null)
            {
                Name = parameter.Name;
                ValueType = parameter.ValueType;

                if (ValueType == ReportArgumentValueType.Variable)                    
                    Variable = parameter.Value;
                else
                    Value = parameter.Value;
            };
        }

        public ParameterNode ( Variables variables, SsrsDiscovery.ItemParameter parameter, ParameterNode existingParameter )
        {
            m_variables = variables;

            Name = parameter?.Name;
            Type = ParameterTypeNameToClrType(parameter?.ParameterTypeName);
            IsNullable = parameter?.Nullable ?? false;

            DefaultValue = parameter?.DefaultValues?.FirstOrDefault();

            if (existingParameter != null)
            {
                ValueType = existingParameter.ValueType;
                if (ValueType == ReportArgumentValueType.Variable)
                    Variable = existingParameter.Variable;
                else
                    Value = existingParameter.Value;
            };
        }
        #endregion

        [Browsable(false)]
        public string Name { get; set; }

        [ReadOnly(true)]
        public Type Type { get; set; }

        [ReadOnly(true)]
        public bool IsNullable { get; set; }

        [ReadOnly(true)]
        public string DefaultValue { get; set; }

        [TypeConverter(typeof(VariablesStringConverter))]
        [RefreshProperties(RefreshProperties.Repaint)]
        public string Variable { get; set; }

        [RefreshProperties(RefreshProperties.Repaint)]
        public string Value { get; set; }

        [RefreshProperties(RefreshProperties.All)]
        [DefaultValue(ReportArgumentValueType.Variable)]
        public ReportArgumentValueType ValueType { get; set; }

        public override string ToString ()
        {
            if (ValueType == ReportArgumentValueType.Variable)
                return Variable;

            return Value;
        }

        public static Type ParameterTypeNameToClrType ( string typeName )
        {
            if (String.Compare(typeName, "Boolean", true) == 0)
                return typeof(bool);

            if (String.Compare(typeName, "DateTime", true) == 0)
                return typeof(DateTime);

            if (String.Compare(typeName, "Float", true) == 0)
                return typeof(double);

            if (String.Compare(typeName, "Integer", true) == 0)
                return typeof(int);

            return typeof(string);
        }

        #region ICustomTypeDescriptor Members

        AttributeCollection ICustomTypeDescriptor.GetAttributes ()
        {
            return TypeDescriptor.GetAttributes(this, true);
        }

        string ICustomTypeDescriptor.GetClassName ()
        {
            return TypeDescriptor.GetClassName(this, true);
        }

        string ICustomTypeDescriptor.GetComponentName ()
        {
            return TypeDescriptor.GetComponentName(this, true);
        }

        TypeConverter ICustomTypeDescriptor.GetConverter ()
        {
            return TypeDescriptor.GetConverter(this, true);
        }

        EventDescriptor ICustomTypeDescriptor.GetDefaultEvent ()
        {
            return TypeDescriptor.GetDefaultEvent(this, true);
        }

        PropertyDescriptor ICustomTypeDescriptor.GetDefaultProperty ()
        {
            return TypeDescriptor.GetDefaultProperty(this, true);
        }

        object ICustomTypeDescriptor.GetEditor ( Type editorBaseType )
        {
            return TypeDescriptor.GetEditor(this, editorBaseType, true);
        }

        EventDescriptorCollection ICustomTypeDescriptor.GetEvents ()
        {
            return TypeDescriptor.GetEvents(this, true);
        }

        EventDescriptorCollection ICustomTypeDescriptor.GetEvents ( Attribute[] attributes )
        {
            return TypeDescriptor.GetEvents(this, attributes, true);
        }

        PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties ()
        {
            return ((ICustomTypeDescriptor)this).GetProperties(new Attribute[0]);
        }

        PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties ( Attribute[] attributes )
        {
            var properties = TypeDescriptor.GetProperties(this, attributes, true)?.OfType<PropertyDescriptor>();
            if (properties == null)
                return null;

            var actualProperties = new List<PropertyDescriptor>();
            foreach (var property in properties)
            {
                //Toggle Variable and Value properties based upon selected value type
                if (property.Name == "Variable")
                {
                    if (ValueType == ReportArgumentValueType.Variable)
                        actualProperties.Add(property);
                } else if (property.Name == "Value")
                {
                    if (ValueType == ReportArgumentValueType.Value)
                        actualProperties.Add(property);
                } else
                    actualProperties.Add(property);
            };

            return new PropertyDescriptorCollection(actualProperties.ToArray());
        }

        object ICustomTypeDescriptor.GetPropertyOwner ( PropertyDescriptor pd )
        {
            return this;
        }
        #endregion

        #region IDtsVariablesProvider Members

        Variables IDtsVariablesProvider.Variables 
        {  
            get { return m_variables; }
        }

        #endregion

        #region INewVariableProvider Members

        public NewVariableAttribute GetNewVariable ( string ppropertyName )
        {
            return new NewVariableAttribute(this.Name, this.Type);
        }
        #endregion

        private readonly Variables m_variables;
    }
}
