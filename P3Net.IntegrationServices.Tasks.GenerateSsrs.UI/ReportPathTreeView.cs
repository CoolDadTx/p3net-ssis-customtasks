/*
 * Copyright © 2016 Federation of State Medical Boards
 * All Rights Reserved
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using P3Net.IntegrationServices.UI;
using P3Net.IntegrationServices.Tasks.GenerateSsrs.UI.SsrsDiscovery;
using Microsoft.SqlServer.Dts.Runtime;

namespace P3Net.IntegrationServices.Tasks.GenerateSsrs.UI
{
    internal class ReportPathTreeView : TreeView
    {
        #region Construction

        public ReportPathTreeView ( ConnectionManager connectionManager )
        {
            m_connectionManager = connectionManager;

            m_client = new Lazy<SsrsDiscovery.ReportingService2010>(CreateSsrsClient);

            ShowRootLines = true;
            ShowPlusMinus = true;
            ShowLines = true;

            //Add dummy loading node
            Nodes.Add(new FolderNode(m_connectionManager.Name, "/"));
        }
        #endregion        

        public string SelectedReportPath { get; set; }
               
        protected override void Dispose ( bool disposing )
        {
            if (disposing)
            {
                try
                {
                    if (m_client.IsValueCreated)
                        m_client.Value.Dispose();
                } catch
                { /* Ignore */ };
            };

            base.Dispose(disposing);            
        }

        protected override void OnAfterSelect ( TreeViewEventArgs e )
        {
            base.OnAfterSelect(e);

            var item = e.Node as ReportNode;
            if (item != null)
                SelectedReportPath = item.ItemPath;
        }

        protected override void OnBeforeExpand ( TreeViewCancelEventArgs e )
        {
            base.OnBeforeExpand(e);

            var folder = e.Node as FolderNode;
            if (folder != null && !folder.HasLoaded)
            {
                try
                {
                    Cursor = Cursors.WaitCursor;

                    folder.Load(Client);
                } catch (Exception ex)
                {
                    MessageBoxes.Error(this, "Error Loading Folder", ex);

                    folder.AddErrorNode(ex.Message);
                } finally
                {
                    Cursor = Cursors.Default;
                };
            };
        }

        #region Private Members

        private SsrsDiscovery.ReportingService2010 Client
        {
            get { return m_client.Value; }
        }

        private readonly ConnectionManager m_connectionManager;

        private readonly Lazy<SsrsDiscovery.ReportingService2010> m_client;

        private SsrsDiscovery.ReportingService2010 CreateSsrsClient ( )
        {
            //Get the underlying connection
            var conn = new HttpClientConnection(m_connectionManager.AcquireConnection(null));

            var proxy = new SsrsDiscovery.ReportingService2010()
            {
                Url = Path.Combine(m_connectionManager.ConnectionString, "ReportService2010.asmx").Replace('\\', '/'),
                Credentials = conn.CreateCredentials()
            };

            return proxy;
        }        
        #endregion

        #region Types

        private sealed class DummyNode : TreeNode
        {
            public DummyNode ( string text ) : base(text)
            { }
        }

        private sealed class FolderNode : TreeNode
        {
            public FolderNode ( string text, string itemPath ) : base(text)
            {
                Nodes.Add(new DummyNode("Loading..."));

                ItemPath = itemPath;
            }

            public void AddErrorNode ( string text )
            {
                Nodes.Clear();

                Nodes.Add(new DummyNode(text));
                HasLoaded = true;
            }

            public bool HasLoaded { get; private set; }

            public string ItemPath { get; private set; }
            
            public void Load ( SsrsDiscovery.ReportingService2010 client )
            {
                Nodes.Clear();

                var items = client.ListChildren(ItemPath, false);
                foreach (CatalogItem item in items)
                {
                    if (item.Hidden)
                        continue;

                    if (String.Compare(item.TypeName, "Folder", true) == 0)
                        Nodes.Add(new FolderNode(item.Name, item.Path));
                    else if (String.Compare(item.TypeName, "Report", true) == 0)
                        Nodes.Add(new ReportNode(item.Name, item.Path));
                };

                HasLoaded = true;
            }
        }

        private sealed class ReportNode : TreeNode
        {
            public ReportNode ( string text, string itemPath ) : base(text)
            {
                ItemPath = itemPath;
            }

            public string ItemPath { get; private set; }
        }
        #endregion
    }
}
