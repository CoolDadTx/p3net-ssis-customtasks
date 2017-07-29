/*
 * Copyright © 2016 Federation of State Medical Boards
 * All Rights Reserved
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using P3Net.IntegrationServices.UI.ComponentModel;

namespace P3Net.IntegrationServices.Tasks.GenerateSsrs.UI
{
    internal class ParameterViewNode : ICustomTypeDescriptor
    {
        public IEnumerable<ParameterNode> Parameters
        {
            get { return m_parameters; }
        }

        public void SetParameters ( IEnumerable<ParameterNode> parameters )
        {
            lock(m_parameters)
            {
                m_parameters.Clear();

                if (parameters != null)
                    m_parameters.AddRange(parameters.OrderBy(p => p.Name));
            };
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
            lock(m_parameters)
            {
                var properties = new PropertyDescriptor[m_parameters.Count];

                for (int i = 0; i < m_parameters.Count; ++i)
                {
                    var parm = m_parameters[i];

                    properties[i] = new DelegatePropertyDescriptor(GetType(), parm.Name, typeof(ParameterNode), o => GetParameterValue(parm.Name), null);
                };

                return new PropertyDescriptorCollection(properties);
            };
        }

        object ICustomTypeDescriptor.GetPropertyOwner ( PropertyDescriptor pd )
        {
            return this;
        }

        private object GetParameterValue ( string name )
        {
            return m_parameters.FirstOrDefault(p => p.Name == name);
        }
        #endregion

        #region Private Members

        private readonly List<ParameterNode> m_parameters = new List<ParameterNode>();
        #endregion
    }
}
