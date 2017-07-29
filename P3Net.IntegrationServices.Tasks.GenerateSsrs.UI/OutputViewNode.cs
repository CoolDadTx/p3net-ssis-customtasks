/*
 * Copyright © 2016 Federation of State Medical Boards
 * All Rights Reserved
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using P3Net.IntegrationServices.UI;
using P3Net.IntegrationServices.UI.ComponentModel;
using P3Net.IntegrationServices.UI.Converters;
using P3Net.IntegrationServices.UI.Tasks;
using Microsoft.SqlServer.Dts.Runtime;

namespace P3Net.IntegrationServices.Tasks.GenerateSsrs.UI
{
    /// <summary>Exposes the properties for the Output tab.</summary>
    internal class OutputViewNode : IDtsVariablesProvider
    {
        #region Construction

        public OutputViewNode ( TaskHost host )
        {
            m_host = host;

            //Properties
            GenerateSsrsTask task;
            if (host.TryGetTask(out task))
            {
                Content = task.Content;
            };
        }
        #endregion

        [Category("Output")]
        [Description("Specifies the variable to store the report output into")]
        [TypeConverter(typeof(VariablesStringConverter))]
        [NewVariable("ReportContent", typeof(object))]
        public string Content { get; set; }

        [Browsable(false)]
        public Variables Variables => m_host?.Variables;

        #region Private Members

        private readonly TaskHost m_host;
        #endregion
    }
}
