/*
 * Copyright © 2016 Federation of State Medical Boards
 * All Rights Reserved
 */
using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.SqlServer.Dts.Runtime;

namespace P3Net.IntegrationServices.UI.Tasks
{
    /// <summary>Provides extensions for <see cref="TaskHost"/>.</summary>
    public static class TaskHostExtensions
    {
        /// <summary>Gets the task associated with the host.</summary>
        /// <typeparam name="T">The type of the task.</typeparam>
        /// <param name="source">The source.</param>
        /// <returns>The task.</returns>
        /// <exception cref="Exception">No task associated with the host.</exception>
        public static T GetTask<T> ( this TaskHost source ) where T : Task
        {
            T task;
            if (!source.TryGetTask<T>(out task))                
                throw new Exception("Task not found in host.");

            return task;
        }

        /// <summary>Gets the task associated with the host.</summary>
        /// <typeparam name="T">The type of the task.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="task">The task.</param>
        /// <returns><see langword="true"/> if the task is returned or <see langword="false"/> otherwise.</returns>
        public static bool TryGetTask<T> ( this TaskHost source, out T task ) where T : Task
        {
            task = source?.InnerObject as T;

            return task != null;
        }
    }
}
