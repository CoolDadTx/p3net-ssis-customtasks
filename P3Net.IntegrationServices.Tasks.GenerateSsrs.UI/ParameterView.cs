/*
 * Copyright © 2016 Federation of State Medical Boards
 * All Rights Reserved
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using P3Net.IntegrationServices.UI;
using P3Net.IntegrationServices.UI.Tasks;
using Microsoft.SqlServer.Dts.Runtime;
using Microsoft.SqlServer.Dts.Runtime.Design;

namespace P3Net.IntegrationServices.Tasks.GenerateSsrs.UI
{
    internal partial class ParameterView : DtsTaskUIPropertyView<ParameterViewNode>
    {
        #region Construction

        public ParameterView ()
        {
            m_worker = new BackgroundWorker() { WorkerSupportsCancellation = true };
            m_worker.RunWorkerCompleted += OnArgumentsLoaded;
            m_worker.DoWork += OnLoadArguments;
        }
        #endregion

        protected override ParameterViewNode CreateNode ( TaskHost host, IDtsConnectionService connectionService )
        {
            return new ParameterViewNode();
        }
        
        protected override void OnInitializeCore ()
        {
            base.OnInitializeCore();

            var task = GetTask<GenerateSsrsTask>();

            if (task?.Arguments != null)
                Node.SetParameters(from a in task.Arguments select new ParameterNode(Host.Variables, a));

            PropertyGrid.PropertySort = PropertySort.NoSort;            
        }

        protected override bool OnLoseSelectionCore ( ref string reason )
        {            
            if (m_worker.IsBusy)
            {
                reason = "Waiting for an asynchronous process to complete.";
                return false;
            };

            return true;
        }

        protected override void OnSelectionCore ()
        {
            base.OnSelectionCore();

            var view = GetView<GeneralView>();
            
            //If there is no report defined yet then error
            if (String.IsNullOrWhiteSpace(view?.Node?.ReportPath) || String.IsNullOrWhiteSpace(view?.Node?.HttpConnection))
            {
                MessageBoxes.Error(this, "No Report Selected", "You must select a report before mapping the parameters.");
                PropertyGrid.Enabled = false;
                return;
            };

            //If the report has changed since the last time we loaded then refresh the arguments
            if (String.Compare(view.Node.ReportPath, m_currentReportPath, true) != 0)
            {
                RefreshArguments();
                m_currentReportPath = view.Node.ReportPath;
            };
        }

        protected override void Save ( )
        {
            var task = GetTask<GenerateSsrsTask>();
            task.Arguments.Clear();

            //Create a new argument for each parameter that was configured
            var parameters = from p in Node.Parameters
                             where (p.ValueType == ReportArgumentValueType.Value && !String.IsNullOrWhiteSpace(p.Value)) ||
                                   (p.ValueType == ReportArgumentValueType.Variable && !String.IsNullOrWhiteSpace(p.Variable))
                             select p;

            task.Arguments.AddRange(from p in parameters select new ReportArgument() { Name = p.Name, ValueType = p.ValueType, Value = (p.ValueType == ReportArgumentValueType.Value) ? p.Value : p.Variable });
        }                               
        
        #region Private Members

        private SsrsDiscovery.ReportingService2010 CreateSsrsClient ( GeneralViewNode node )
        {
            //Get the underlying connection
            var cm = ConnectionService.GetConnections().GetConnection(node.HttpConnection);
            var conn = new HttpClientConnection(cm.AcquireConnection(null));

            var proxy = new SsrsDiscovery.ReportingService2010()
            {
                Url = Path.Combine(cm.ConnectionString, "ReportService2010.asmx").Replace('\\', '/'),
                Credentials = conn.CreateCredentials()
            };

            return proxy;
        }

        private List<ParameterNode> MergeParameters ( IEnumerable<ParameterNode> existingParameters, IEnumerable<SsrsDiscovery.ItemParameter> parameters )
        {
            return (from p in parameters
                    select new ParameterNode(Host.Variables, p, existingParameters.FirstOrDefault(ep => String.Compare(p.Name, ep.Name, true) == 0))
                   ).ToList();
        }

        private void OnArgumentsLoaded ( object sender, RunWorkerCompletedEventArgs e )
        {
            try
            {
                if (e.Cancelled)
                {
                    //We'll need to reload the next time
                    m_currentReportPath = null;
                    return;
                };

                if (e.Error != null)
                    throw e.Error;

                var parameters = e.Result as IEnumerable<ParameterNode>;

                Node.SetParameters(parameters);
                PropertyGrid.Enabled = true;
                PropertyGrid.Refresh();
                PropertyGrid.ExpandAllGridItems();
            } catch (Exception ex)
            {
                MessageBoxes.Error(this, "Error Loading Parameters", ex);
            } finally
            {
                this.Cursor = Cursors.Default;
            };
        }

        private void OnLoadArguments ( object sender, DoWorkEventArgs e )
        {
            var view = GetView<GeneralView>();
            var node = view?.Node;
            if (node == null)
                return;

            //Query for the parameters of the report
            using (var client = CreateSsrsClient(node))
            {
                var parameters = client.GetItemParameters(node.ReportPath.EnsureStartsWith('/'), null, true, null, null);

                e.Result = MergeParameters(Node.Parameters, parameters);
            };
        }        

        private void RefreshArguments ()
        {
            PropertyGrid.Enabled = false;

            this.Cursor = Cursors.WaitCursor;

            m_worker.RunWorkerAsync();
        }
        
        private string m_currentReportPath;
        private BackgroundWorker m_worker;
        #endregion
    }
}
