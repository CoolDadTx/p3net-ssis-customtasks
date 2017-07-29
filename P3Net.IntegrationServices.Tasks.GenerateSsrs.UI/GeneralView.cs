/*
 * Copyright © 2016 Federation of State Medical Boards
 * All Rights Reserved
 */
using System;
using System.Collections.Generic;
using System.Linq;

using P3Net.IntegrationServices.UI.Tasks;
using Microsoft.SqlServer.Dts.Runtime;
using Microsoft.SqlServer.Dts.Runtime.Design;

namespace P3Net.IntegrationServices.Tasks.GenerateSsrs.UI
{
    internal partial class GeneralView : DtsTaskUIPropertyView<GeneralViewNode>
    {
        protected override GeneralViewNode CreateNode ( TaskHost host, IDtsConnectionService connectionService )
        {
            return new GeneralViewNode(host, connectionService);
        }
        
        protected override void Save ( )
        {
            var task = GetTask<GenerateSsrsTask>();

            //General properties
            Host.Name = Node.Name;
            Host.Description = Node.Description;

            //Connection properties
            task.ServerConnection = Node.HttpConnection;
            task.ReportFormat = Node.ReportFormat;
            task.ReportPath = Node.ReportPath;
        }
    }
}
