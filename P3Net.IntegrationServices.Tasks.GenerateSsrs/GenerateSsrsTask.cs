using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml;
using P3Net.IntegrationServices.Common;
using P3Net.IntegrationServices.Logging;
using P3Net.IntegrationServices.Tasks.GenerateSsrs.Properties;
using P3Net.IntegrationServices.Xml;
using Microsoft.SqlServer.Dts.Runtime;

namespace P3Net.IntegrationServices.Tasks.GenerateSsrs
{    
    [DtsTask(DisplayName=GenerateSsrsTask.TaskName, RequiredProductLevel = DTSProductLevel.None, Description = GenerateSsrsTask.Description, IconResource = "P3Net.IntegrationServices.Tasks.GenerateSsrs.Images.GenerateSsrsTask.ico"
           , UITypeName = GenerateSsrsTask.UITypeName)]        
    public class GenerateSsrsTask : BaseTask, IDTSComponentPersist
    {        
        #region Common Attributes

        public const string Description = "Generates an SSRS report";    
        public const string TaskName = "Generate SSRS Report Task (P3Net)";
        public const string UITypeName = "P3Net.IntegrationServices.Tasks.GenerateSsrs.UI.GenerateSsrsTaskUI, P3Net.IntegrationServices.Tasks.GenerateSsrs.UI, Version=" + AssemblyMetadata.ProductVersion + ", Culture=Neutral, PublicKeyToken=0b943396c101760f";

        public static Icon Icon = Resources.GenerateSsrsTask;
        #endregion

        public List<ReportArgument> Arguments { get; private set; } = new List<ReportArgument>();

        public string Content { get; set; }
        
        public string ReportFormat
        {
            get { return m_reportFormat ?? "PDF"; }
            set { m_reportFormat = value ?? "PDF"; }
        }

        public string ReportPath
        {
            get { return m_reportPath ?? ""; }
            set { m_reportPath = value; }
        }

        public string ServerConnection
        {
            get { return TryGetConnectionName(m_connectionId) ?? ""; }
            set { m_connectionId = TryGetConnectionId(value); }
        }

        [Browsable(false)]
        public override string TaskDisplayName
        {
            get { return TaskName; }
        }

        public void LoadFromXML ( XmlElement node, IDTSInfoEvents infoEvents )
        {
            Content = node.GetAttributeValue("Content");
            ReportFormat = node.GetAttributeValue("ReportFormat");
            ReportPath = node.GetAttributeValue("ReportPath");
            m_connectionId = node.GetAttributeValue("ServerConnection");

            var elements = node.SelectNodes("Arguments/Argument").OfType<XmlElement>();
            foreach (var element in elements)
            {
                var arg = new ReportArgument()
                {
                    Name = element.GetAttributeValue("name"),
                    Value = element.GetAttributeValue("value"),
                    ValueType = Parse(element.GetAttributeValue("valueType"), ReportArgumentValueType.Variable)
                };

                Arguments.Add(arg);
            };
        }

        public void SaveToXML ( XmlDocument doc, IDTSInfoEvents infoEvents )
        {
            var root = doc.CreateAndAddElement(GetType().Name);

            root.SetAttributeValue("Content", Content);
            root.SetAttributeValue("ReportFormat", ReportFormat);
            root.SetAttributeValue("ReportPath", ReportPath);
            root.SetAttributeValue("ServerConnection", m_connectionId);

            var element = root.CreateAndAddElement("Arguments");
            foreach (var arg in Arguments)
            {
                var argumentElement = element.CreateAndAddElement("Argument");
                argumentElement.SetAttributeValue("name", arg.Name);
                argumentElement.SetAttributeValue("value", arg.Value);
                argumentElement.SetAttributeValue("valueType", arg.ValueType);
            };
        }        

        #region Protected Members

        protected override DTSExecResult ExecuteCore ( ITaskExecuteContext context )
        {
            // Concatenate Querystring            
            var reportPath = HttpUtility.UrlEncode(ReportPath.EnsureStartsWith("/"));
            var builder = new StringBuilder(reportPath);

            foreach (var arg in Arguments)
            {
                string strValue = arg.Value;

                //Get the actual value
                if (arg.ValueType == ReportArgumentValueType.Variable)
                    strValue = context.Variables.GetValue(arg.Value)?.ToString();

                if (strValue == null)
                    builder.AppendFormat("&{0}:isnull=true", HttpUtility.UrlEncode(arg.Name));
                else
                    builder.AppendFormat("&{0}={1}", HttpUtility.UrlEncode(arg.Name), HttpUtility.UrlEncode(strValue));
            };
            builder.AppendFormat("&rs:Command=Render&rs:Format={0}", ReportFormat);

            //Get the connection
            var cm = context.Connections.GetConnection(ServerConnection);

            //Create a copy of the connection because we're going to change the URL
            var conn = new HttpClientConnection(cm.AcquireConnection(context.Transaction)).Clone();
            if (conn == null)
                throw new Exception("Unable to acquire connection.");

            // Configure Full Report Url
            //Format: <serverUrl>?/<reportPath><parameters><options>
            var uri = new UriBuilder(conn.ServerURL) { Query = builder.ToString() };
            conn.ServerURL = uri.Uri.ToString();

            // Generate Report            
            context.Events.LogInformation(b => b.Message("Generating report: {0}", conn.ServerURL));
            try
            {
                var data = conn.DownloadData();
                if (!String.IsNullOrWhiteSpace(Content))
                    context.Variables.SetValue(Content, data);

                context.Events.LogInformation(b => b.Message("Report generated with size {1}: {0}", conn.ServerURL, data?.Length));
            } catch (Exception e)
            {
                //Handle some common exceptions
                throw HandleCommonExceptions(e);
            };            

            return DTSExecResult.Success;
        }        

        protected override DTSExecResult ValidateCore ( ITaskValidateContext context )
        {
            //Validate the connection
            var cm = context.Connections.TryGetConnection(ServerConnection);
            if (cm == null)
            {
                context.Events.LogError("Server connection could not be found.");
                return DTSExecResult.Failure;
            };

            //Validate report path
            if (String.IsNullOrWhiteSpace(ReportPath))
            {
                context.Events.LogError("Report Path is required.");
                return DTSExecResult.Failure;
            };

            //Validate report format
            if (String.IsNullOrWhiteSpace(ReportFormat))
            {
                context.Events.LogError("Report Format is required.");
                return DTSExecResult.Failure;
            };

            //Validate the content
            if (!String.IsNullOrWhiteSpace(Content))
            {
                //Get the variable
                var variable = context.Variables.TryGetInfo(Content);
                if (variable == null)
                {
                    context.Events.LogError("Content variable does not exist.");
                    return DTSExecResult.Failure;
                };

                //Must be an object
                if (variable.DataType != TypeCode.Object)
                {
                    context.Events.LogError("Content must be of type Object.");
                    return DTSExecResult.Failure;
                };

                //Must be writable
                if (variable.ReadOnly)
                {
                    context.Events.LogError("Content must be writable.");
                    return DTSExecResult.Failure;
                };
            };

            return base.ValidateCore(context);
        }
        #endregion

        #region Private Members

        private string m_connectionId;

        private string m_reportPath;
        private string m_reportFormat;

        private Exception HandleCommonExceptions ( Exception error )
        {
            var dtsError = error as DtsRuntimeException;
            if (dtsError != null)
            {
                if ((ulong)dtsError.ErrorCode == 0xFFFFFFFFC001600E)
                    throw new Exception("Report was not found.", error);
            };

            return error;
        }        

        private T Parse<T> ( string value, T defaultValue ) where T : struct
        {
            T result;
            if (Enum.TryParse<T>(value, out result))
                return result;

            return defaultValue;
        }
        #endregion
    }
}
