/*
 * Copyright © 2016 Federation of State Medical Boards
 * All Rights Reserved
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;

using P3Net.IntegrationServices.Tasks.GenerateSsrs.UI.Converters;
using P3Net.IntegrationServices.UI;
using P3Net.IntegrationServices.UI.Converters;
using P3Net.IntegrationServices.UI.Tasks;
using Microsoft.DataTransformationServices.Controls;
using Microsoft.SqlServer.Dts.Runtime;
using Microsoft.SqlServer.Dts.Runtime.Design;

namespace P3Net.IntegrationServices.Tasks.GenerateSsrs.UI
{
    /// <summary>Exposes the properties for the General tab.</summary>
    [SortProperties(new string[] { "Name", "Description", "HttpConnection", "ReportPath", "ReportFormat" })]
    internal class GeneralViewNode : IDtsConnectionServiceProvider
    {
        #region Construction

        public GeneralViewNode ( TaskHost host, IDtsConnectionService connectionService )
        {            
            ConnectionService = connectionService;

            //General properties
            m_name = host.Name;
            m_description = host.Description;

            //Connection properties
            GenerateSsrsTask task;
            if (host.TryGetTask(out task))            
            {
                HttpConnection = task.ServerConnection;
                m_format = task.ReportFormat;
                m_path = task.ReportPath;
            };
        }
        #endregion

        [Browsable(false)]
        public IDtsConnectionService ConnectionService { get; private set; }

        [Category("General")]
        [Description("Specifies the name of the task.")]
        public string Name
        {
            get { return m_name ?? ""; }
            set {
                if (String.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Name cannot be empty.", "value");

                m_name = value.Trim();
            }
        }

        [Category("General")]
        [Description("Specifies the description of the task.")]
        public string Description
        {
            get { return m_description ?? ""; }
            set { m_description = value.Trim(); }
        }

        [Category("Connection")]
        [Description("Specifies the HTTP connection for the reporting server.")]
        [TypeConverter(typeof(HttpConnectionsStringConverter))]
        public string HttpConnection
        {
            get { return m_connection ?? ""; }
            set { m_connection = value; }
        }

        [Category("Connection")]
        [Description("Specifies the format of the report.")]
        [DefaultValue("PDF")]
        [TypeConverter(typeof(ReportFormatStringConverter))]
        public string ReportFormat
        {
            get { return m_format ?? ""; }
            set { m_format = value; }
        }

        [Category("Connection")]
        [Description("Specifies the path and name of the report.")]        
        [Editor(typeof(ReportPathEditor), typeof(UITypeEditor))]
        public string ReportPath
        {
            get { return m_path ?? ""; }
            set 
            {
                if (value != null)
                    value = value.EnsureStartsWith("/");

                m_path = value;
            }
        }           

        #region Private Members        
        
        private string m_name, m_description;

        private string m_connection;
        private string m_format = "PDF", m_path;
        #endregion
    }
}
