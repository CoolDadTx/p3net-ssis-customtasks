/*
 * Copyright © 2016 Federation of State Medical Boards
 * All Rights Reserved
 */
using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.SqlServer.Dts.Runtime;

namespace P3Net.IntegrationServices.UI
{
    /// <summary>Provides a simple interface for getting access to <see cref="Variables"/>.</summary>
    public interface IDtsVariablesProvider
    {
        /// <summary>Gets the <see cref="Variables" /> instance.</summary>
        Variables Variables { get; }
    }
}
