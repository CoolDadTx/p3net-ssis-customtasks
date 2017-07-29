using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.SqlServer.Dts.Runtime;

namespace P3Net.IntegrationServices.Common
{
    internal sealed class TaskExecuteContext : ITaskExecuteContext
    {
        public Connections Connections { get; set; }

        public IDTSComponentEvents Events { get; set; }

        public IDTSLogging Log { get; set; }

        public object Transaction { get; set; }

        public VariableDispenser Variables { get; set; }        
    }
}
