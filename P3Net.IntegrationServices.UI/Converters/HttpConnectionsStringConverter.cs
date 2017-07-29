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
    /// <summary>Converter for working with HTTP connections.</summary>
    /// <remarks>
    /// View properties can use this converter to provide a user with the list of available HTTP connections and the ability to 
    /// add a new connection.  To support adding a new connection ensure the view is derived from <see cref="DtsTaskUIPropertyView{T}"/>
    /// and implement the <see cref="DtsTaskUIPropertyView{T}.OnPropertyChanged">OnPropertyChanged</see> method.
    /// </remarks>
    public class HttpConnectionsStringConverter : ConnectionsStringConverter
    {
        public HttpConnectionsStringConverter ( ) : base("HTTP")
        {
        }
    }
}
