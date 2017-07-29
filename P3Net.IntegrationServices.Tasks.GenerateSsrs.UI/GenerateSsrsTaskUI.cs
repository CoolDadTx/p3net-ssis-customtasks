/*
 * Copyright © 2016 Federation of State Medical Boards
 * All Rights Reserved
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

using P3Net.IntegrationServices.UI.Tasks;

using Microsoft.SqlServer.Dts.Runtime.Design;

namespace P3Net.IntegrationServices.Tasks.GenerateSsrs.UI
{
    public class GenerateSsrsTaskUI : DtsTaskUI
    {
        protected override ContainerControl GetViewCore ()
        {
            return new GenerateSsrsTaskForm(Host, ServiceProvider.GetService<IDtsConnectionService>());
        }
    }
}
