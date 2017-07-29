/*
 * Copyright © 2016 Federation of State Medical Boards
 * All Rights Reserved
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;

using P3Net.IntegrationServices.UI.Converters;
using P3Net.IntegrationServices.UI.ComponentModel;

using Microsoft.DataTransformationServices.Controls;
using Microsoft.SqlServer.Dts.Runtime;
using Microsoft.SqlServer.Dts.Runtime.Design;

namespace P3Net.IntegrationServices.UI.Tasks
{
    /// <summary>Provides a base class for views that use the standard property grid.</summary>    
    /// <typeparam name="T">The type of the data shown in the property grid.</typeparam>
    /// <remarks>
    /// Derived types should do the following.
    /// <list type="number">
    ///    <item>Have internal accessibility.</item>
    ///    <item>Create a type to store the properties to be shown.</item>
    ///    <item>Override <see cref="CreateNode"/> to create the data.</item>    
    ///    <item>Override <see cref="Save" /> to save the data back to the task.</item>
    ///    <item>Optionally override <see cref="OnInitializeCore"/> to do any one-time loading of data.  Be sure to call the base method.</item>
    ///    <item>Optionally override <see cref="OnSelectionCore"/> to update data with changes from other views.</item>
    /// </list>
    /// <para />
    /// For the view data do the following.
    /// <list type="number">
    ///    <item>Create an internal type for the data.</item>
    ///    <item>Define a public get/set property for each value to be exposed.</item>
    ///    <item>For each property use the <see cref="CategoryAttribute"/> and <see cref="DescriptionAttribute"/> to provide information about the property.</item>
    ///    <item>Optionally apply the <see cref="SortPropertiesAttribute"/> to control the property ordering.</item>
    ///    <item>Optionally apply a type converter for special properties such as variables and connections.</item>
    /// </list>
    /// </remarks>
    public abstract partial class DtsTaskUIPropertyView<T> : DtsTaskUIView where T : class
    {
        #region Construction

        public DtsTaskUIPropertyView ()
        {
            InitializeComponent();
        }
        #endregion

        /// <summary>Gets the node associated with the view.</summary>
        public T Node => ((PropertyGrid?.LocalizableSelectedObject as DTSLocalizableTypeDescriptor)?.SelectedObject ?? PropertyGrid.LocalizableSelectedObject) as T;

        #region Protected Members

        /// <summary>Gets the property grid.</summary>
        protected LocalizablePropertyGrid PropertyGrid => m_propertyGrid;

        /// <summary>Called to initialize the <see cref="Node"/> property.</summary>
        /// <param name="host">The host.</param>
        /// <param name="connectionService">The connection service.</param>
        /// <returns>The object to set as the <see cref="Node"/> of the view.</returns>
        protected internal abstract T CreateNode ( TaskHost host, IDtsConnectionService connectionService );        

        /// <summary>Called to initialize the view.</summary>        
        /// <remarks>
        /// If this method is overridden in a derived class then be sure to call the base method before attempting to 
        /// use <see cref="Node"/>.
        /// </remarks>
        protected override void OnInitializeCore ( )
        {
            PropertyGrid.PropertyValueChanged += OnPropertyValueChanged;

            PropertyGrid.LocalizableSelectedObject = CreateNode(Host, ConnectionService);
        }

        /// <summary>Handles property value change notifications.</summary>
        /// <param name="e">The event arguments.</param>
        /// <remarks>
        /// The default implementation handles the creation of new items using <see cref="NewItemStringConverter"/>.
        /// </remarks>
        protected virtual void OnPropertyChanged ( PropertyValueChangedEventArgs e )
        {
            //Handle new item converters specially
            var converter = IsNewItem(e.ChangedItem);
            if (converter != null)
            {
                var target = GetTargetObject(e.ChangedItem);
                var property = e.ChangedItem.PropertyDescriptor;
                var oldValue = e.OldValue ?? "";                
                
                try
                {
                    this.Cursor = Cursors.WaitCursor;

                    var context = new NewItemContext(this)
                    {
                        Target = target,
                        TargetProperty = property
                    };
                    
                    var newValue = converter.ProcessNewItem(context);
                    if (newValue != null)
                        property.SetValue(target, newValue);
                    else
                        property.SetValue(target, oldValue);                    
                } catch (Exception ex)
                {
                    MessageBoxes.Error(this, "Error Creating New Item", ex);

                    property.SetValue(target, oldValue);                    
                } finally
                {
                    this.Cursor = Cursors.Default;
                };
            };
        }
        #endregion

        #region Private Members

        private sealed class NewItemContext : INewItemContext
        {
            public NewItemContext ( DtsTaskUIPropertyView<T> view )
            {
                View = view;
            }

            public IDtsConnectionService Connections
            {
                get { return View.ConnectionService; }
            }

            public DtsContainer Container
            {
                get { return View.Host; }                    
            }

            public IWin32Window ParentWindow
            {
                get { return View; }
            }

            public IServiceProvider ServiceProvider
            {
                get { return View.Site; }
            }

            public object Target { get; set; }

            public PropertyDescriptor TargetProperty { get; set; }

            public IDtsVariableService Variables
            {
                get { return View.VariableService; }
            }

            private readonly DtsTaskUIPropertyView<T> View;
        }

        private object GetTargetObject ( GridItem item )
        {
            //In order to set the property we need the target object associated with the property
            //For a child property its parent will be the target
            //For a root property the selected object is the target
            if (item.Parent != null && item.Parent.GridItemType == GridItemType.Property)
                return item.Parent.Value?.GetSpecializedObject();

            return Node;
        }

        private NewItemStringConverter IsNewItem ( GridItem item )
        {            
            //Must have a value
            if (item?.Value == null)
                return null;

            //Must be a string property
            var descriptor = item.PropertyDescriptor;
            if (descriptor?.PropertyType != typeof(string))
                return null;

            //Must actually be associated with the correct converter
            var attr = descriptor.Attributes.OfType<TypeConverterAttribute>().FirstOrDefault();
            if (attr == null)
                return null;

            //Must be of the correct converter type
            var type = Type.GetType(attr.ConverterTypeName);
            if (type == null || !type.IsSubclassOf(typeof(NewItemStringConverter)))
                return null;

            //The value must match the expected new item value
            var converter = Activator.CreateInstance(type) as NewItemStringConverter;
            if (converter == null || !converter.IsNewItem(item.Value.ToString()))
                return null;

            return converter;
        }        

        private void OnPropertyValueChanged ( object s, PropertyValueChangedEventArgs e )
        {
            OnPropertyChanged(e);
        }        
        #endregion
    }
}
