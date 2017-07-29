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
    public class ConnectionsStringConverter : NewItemStringConverter
    {
        public ConnectionsStringConverter ( string connectionType )
        {
            ConnectionType = connectionType;
        }

        /// <summary>Specifies the type of connection to support.</summary>
        public string ConnectionType { get; private set; }

        /// <summary>Provides the value that can be used to identify a new connection.</summary>
        public override string NewItemText
        {
            get { return "<New Connection>"; }
        }
        
        protected override IEnumerable<string> GetValueList ( ITypeDescriptorContext context )
        {
            var provider = context.Instance.GetSpecializedObject<IDtsConnectionServiceProvider>();
            if (provider != null)
            {
                var connections = provider.ConnectionService.GetConnectionsOfType(ConnectionType);

                return from c in connections.OfType<ConnectionManager>()
                       orderby c.Name
                       select c.Name;
            };

            return null;
        }

        public override object ProcessNewItem ( INewItemContext context )
        {
            var connection = context.Connections.CreateConnection(ConnectionType);
            if (connection != null && connection.Count > 0)
            {
                var item = (ConnectionManager)connection[0];

                return item?.Name;
            };

            return null;
        }
    }
}
