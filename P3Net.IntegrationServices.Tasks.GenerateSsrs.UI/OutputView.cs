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
    internal partial class OutputView : DtsTaskUIPropertyView<OutputViewNode>
    {
        protected override OutputViewNode CreateNode ( TaskHost host, IDtsConnectionService connectionService )
        {
            return new OutputViewNode(host);
        }

        protected override void Save ( )
        {
            var task = GetTask<GenerateSsrsTask>();
            
            task.Content = Node.Content;
        }
    }
}
