/*
 * Copyright © 2016 Federation of State Medical Boards
 * All Rights Reserved
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace P3Net.IntegrationServices.UI.ComponentModel
{
    /// <summary>Property descriptor that uses delegates for getting and setting a property value.</summary>
    public class DelegatePropertyDescriptor : PropertyDescriptor
    {
        /// <summary>Initializes an instance of the <see cref="DelegatePropertyDescriptor"/> class.</summary>
        /// <param name="componentType">The parent type.</param>
        /// <param name="name">The property name.</param>
        /// <param name="type">The property type.</param>
        /// <param name="getter">The property getter delegate.</param>
        /// <param name="setter">The optional property setter delegate.</param>
        /// <param name="attributes">Attributes to apply to the property.</param>
        public DelegatePropertyDescriptor ( Type componentType, string name, Type type, Func<object, object> getter, Action<object, object> setter, params Attribute[] attributes ) : base(name, attributes)
        {
            m_componentType = componentType;
            m_type = type;

            m_getter = getter;
            m_setter = setter;
        }

        public override bool CanResetValue ( object component )
        {
            return true;
        }

        public override Type ComponentType
        {
            get { return m_componentType; }
        }

        public override bool IsReadOnly
        {
            get { return m_setter != null; }
        }

        public override Type PropertyType
        {
            get { return m_type; }
        }

        public override object GetValue ( object component )
        {
            return m_getter(component);
        }

        public override void ResetValue ( object component )
        {
            m_setter(component, null);
        }

        public override void SetValue ( object component, object value )
        {
            m_setter(component, value);
        }

        public override bool ShouldSerializeValue ( object component )
        {
            return false;
        }

        private readonly Type m_type;
        private readonly Type m_componentType;

        private readonly Func<object, object> m_getter;
        private readonly Action<object, object> m_setter;
    }
}
