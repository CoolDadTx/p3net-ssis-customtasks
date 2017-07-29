/*
 * Copyright © 2016 Federation of State Medical Boards
 * All Rights Reserved
 */
using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.SqlServer.Dts.Runtime.Design;

namespace P3Net.IntegrationServices.UI
{
    /// <summary>Provides an interface for getting access to <see cref="Connections"/>.</summary>
    public interface IDtsConnectionServiceProvider
    {
        /// <summary>Gets the connection service.</summary>
        IDtsConnectionService ConnectionService { get; }
    }
}
