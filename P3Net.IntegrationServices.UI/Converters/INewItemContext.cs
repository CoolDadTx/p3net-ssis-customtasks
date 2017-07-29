/*
 * Copyright © 2016 Federation of State Medical Boards
 * All Rights Reserved
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using Microsoft.SqlServer.Dts.Runtime;
using Microsoft.SqlServer.Dts.Runtime.Design;

namespace P3Net.IntegrationServices.UI.Converters
{
    /// <summary>Provides context for creating new items.</summary>
    public interface INewItemContext
    {
        /// <summary>Gets the connections.</summary>
        IDtsConnectionService Connections { get; }

        /// <summary>Gets the DTS container.</summary>
        DtsContainer Container { get; }

        /// <summary>Gets the parent window.</summary>
        IWin32Window ParentWindow { get; }
        
        /// <summary>Gets the service provider.</summary>
        IServiceProvider ServiceProvider { get; }

        /// <summary>Gets the target object.</summary>
        object Target { get; }

        /// <summary>Gets the property.</summary>
        PropertyDescriptor TargetProperty { get; }

        /// <summary>Gets the variables.</summary>
        IDtsVariableService Variables { get; }
    }
}
