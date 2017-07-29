/*
 * Copyright © 2016 Federation of State Medical Boards
 * All Rights Reserved
 */
using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.DataTransformationServices.Controls;
using Microsoft.SqlServer.Dts.Runtime;

namespace P3Net.IntegrationServices.Tasks.GenerateSsrs.UI
{
    internal partial class GenerateSsrsTaskForm : DTSBaseTaskUI
    {        
        public GenerateSsrsTaskForm ( TaskHost taskHost, object connections ) : base(GenerateSsrsTask.TaskName, GenerateSsrsTask.Icon, GenerateSsrsTask.Description, taskHost, connections, true)
        {
            InitializeComponent();

            var startView = new GeneralView();

            DTSTaskUIHost.FastLoad = false;
            DTSTaskUIHost.AddView("General", startView, null);
            DTSTaskUIHost.AddView("Parameter Mappings", new ParameterView(), null);
            DTSTaskUIHost.AddView("Output", new OutputView(), null);            
            DTSTaskUIHost.FastLoad = true;

            DTSTaskUIHost.SelectView(startView);
        }
    }
}
