/*
 * Copyright © 2016 Federation of State Medical Boards
 * All Rights Reserved
 */
using System;

using Microsoft.SqlServer.Dts.Runtime;

namespace P3Net.IntegrationServices.Common
{
    /// <summary>Provides context for initializing a task.</summary>
    public interface ITaskInitializeContext
    {
        /// <summary>Gets the available connections.</summary>
        Connections Connections { get; }

        /// <summary>Gets the logger.</summary>
        IDTSLogging Log { get; }

        /// <summary>Gets the available variables.</summary>
        VariableDispenser Variables { get; }
    }
}
