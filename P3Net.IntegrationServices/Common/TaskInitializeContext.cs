using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.SqlServer.Dts.Runtime;

namespace P3Net.IntegrationServices.Common
{
    internal sealed class TaskInitializeContext : ITaskInitializeContext
    {
        public Connections Connections { get; set; }

        public IDTSLogging Log { get; set; }

        public VariableDispenser Variables { get; set; }

        //Not exposed
        public IDTSInfoEvents Events { get; set; }
        public EventInfos EventInfos { get; set; }
        public LogEntryInfos LogEntryInfos { get; set; }
        public ObjectReferenceTracker ReferenceTracker { get; set; }
    }
}
