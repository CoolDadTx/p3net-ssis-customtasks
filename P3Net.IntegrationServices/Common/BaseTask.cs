/*
 * Copyright © 2016 Federation of State Medical Boards
 * All Rights Reserved
 */
using System;
using System.Collections.Generic;
using System.Linq;

using P3Net.IntegrationServices.Logging;

using Microsoft.SqlServer.Dts.Runtime;

namespace P3Net.IntegrationServices.Common
{
    /// <summary>Provides a base class for custom tasks.</summary>
    public abstract class BaseTask : Task
    {
        /// <summary>Gets the available connections.</summary>
        public Connections Connections { get; private set; }

        /// <summary>Gets the display name of the task.</summary>
        public abstract string TaskDisplayName { get; }

        /// <summary>Executes the task.</summary>
        /// <param name="connections">The connections.</param>
        /// <param name="variableDispenser">The variables.</param>
        /// <param name="componentEvents">The event subsystem.</param>
        /// <param name="log">The logging subsystem.</param>
        /// <param name="transaction">The active transaction.</param>
        /// <returns>The results of the execution.</returns>
        /// <remarks>
        /// Derived types should not override this method.  Use <see cref="ExecuteCore"/> instead.
        /// </remarks>
        public override DTSExecResult Execute ( Connections connections, VariableDispenser variableDispenser, IDTSComponentEvents componentEvents, IDTSLogging log, object transaction )
        {
            try
            {
                var context = new TaskExecuteContext()
                {
                    Connections = connections,
                    Log = log,
                    Transaction = transaction,
                    Variables = variableDispenser,

                    Events = componentEvents
                };
                                
                return ExecuteCore(context);
            } catch (Exception e)
            {
                if (componentEvents != null)
                    componentEvents.LogError(b => b.Message("Unhandled exception during execution.")
                                                   .Exception(e));
                return DTSExecResult.Failure;
            };
        }

        /// <summary>Initializes the task.</summary>
        /// <param name="connections">The connections.</param>
        /// <param name="variableDispenser">The variables.</param>
        /// <param name="events">The event subsystem.</param>
        /// <param name="log">The logging subsystem.</param>
        /// <param name="eventInfos">The event info subsystem.</param>
        /// <param name="logEntryInfos">The log entry subsystem.</param>
        /// <param name="refTracker">Reference tracker.</param>
        /// <remarks>
        /// Derived types should not override this method.  Use <see cref="InitializeCore"/> instead.
        /// </remarks>
        public override void InitializeTask ( Connections connections, VariableDispenser variableDispenser, IDTSInfoEvents events, IDTSLogging log, EventInfos eventInfos, LogEntryInfos logEntryInfos, ObjectReferenceTracker refTracker )
        {
            var context = new TaskInitializeContext()
            {
                Connections = connections,
                Log = log,
                Variables = variableDispenser,

                Events = events,
                EventInfos = eventInfos,
                LogEntryInfos = logEntryInfos,
                ReferenceTracker = refTracker
            };

            //Set the connections for later use
            Connections = connections;

            InitializeCore(context);
        }        

        /// <summary>Validates the task before execution.</summary>
        /// <param name="connections">The connections.</param>
        /// <param name="variableDispenser">The variables.</param>
        /// <param name="componentEvents">The event subsystem.</param>
        /// <param name="log">The log subsystem.</param>
        /// <returns>The results of validation.</returns>
        /// <remarks>
        /// Derived types should not override this method. Use <see cref="ValidateCore"/> instead.
        /// </remarks>
        public override DTSExecResult Validate ( Connections connections, VariableDispenser variableDispenser, IDTSComponentEvents componentEvents, IDTSLogging log )
        {
            try
            {
                var context = new TaskValidateContext()
                {                    
                    Connections = connections,
                    Log = log,
                    Variables = variableDispenser,

                    Events = componentEvents
                };
                
                return ValidateCore(context);
            } catch (Exception e)
            {
                if (componentEvents != null)
                    componentEvents.LogError(b => b.Message("Unhandled exception during validation.")
                                                   .Exception(e));
                return DTSExecResult.Failure;
            };
        }

        #region Protected Members        

        /// <summary>Executes the task.</summary>
        /// <param name="context">The context.</param>
        /// <returns>The results.</returns>
        /// <remarks>
        /// The method is wrapped in an exception handler to prevent unhandled exceptions from boiling up to the runtime.
        /// </remarks>
        protected abstract DTSExecResult ExecuteCore ( ITaskExecuteContext context );    

        /// <summary>Initializes the task.</summary>
        /// <param name="context">The context.</param>
        /// <remarks>
        /// The default implementation calls the base implementation.
        /// </remarks>
        protected virtual void InitializeCore ( ITaskInitializeContext context )
        {
            var realContext = context as TaskInitializeContext;

            base.InitializeTask(context.Connections, context.Variables, realContext?.Events, context.Log, realContext?.EventInfos, realContext?.LogEntryInfos, realContext?.ReferenceTracker);
        }
        
        /// <summary>Gets a connection ID given its name or ID.</summary>
        /// <param name="connections">The connections.</param>
        /// <param name="nameOrId">The name or ID of the connection.</param>
        /// <returns>The connection ID or <see langword="null"/> if no connections are available.</returns>
        protected string TryGetConnectionId ( Connections connections, string nameOrId )
        {
            return (connections != null) ? base.GetConnectionID(connections, nameOrId) : null;
        }

        /// <summary>Gets a connection ID given its name or ID.</summary>
        /// <param name="nameOrId">The name or ID of the connection.</param>
        /// <returns>The connection ID or <see langword="null"/> if no connections are available.</returns>
        protected string TryGetConnectionId ( string nameOrId )
        {
            return TryGetConnectionId(Connections, nameOrId);
        }

        /// <summary>Gets a connection name given its name or ID.</summary>        
        /// <param name="connections">The connections.</param>
        /// <param name="nameOrId">The name or ID of the connection.</param>
        /// <returns>The connection name or <see langword="null"/> if no connections are available.</returns>
        protected string TryGetConnectionName ( Connections connections, string nameOrId )
        {
            return (connections != null) ? base.GetConnectionName(connections, nameOrId) : null;
        }

        /// <summary>Gets a connection name given its name or ID.</summary>        
        /// <param name="nameOrId">The name or ID of the connection.</param>
        /// <returns>The connection name or <see langword="null"/> if no connections are available.</returns>
        protected string TryGetConnectionName ( string nameOrId )
        {
            return TryGetConnectionName(Connections, nameOrId);
        }

        /// <summary>Validates the task.</summary>
        /// <param name="context">The context.</param>
        /// <remarks>
        /// The method is wrapped in an exception handler to prevent unhandled exceptions from boiling up to the runtime.
        /// <para/>
        /// The default implementation calls the base implementation.
        /// </remarks>
        protected virtual DTSExecResult ValidateCore ( ITaskValidateContext context )
        {            
            return base.Validate(context.Connections, context.Variables, context.Events, context.Log);
        }
        #endregion
    }
}
