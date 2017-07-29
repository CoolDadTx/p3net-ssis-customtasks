/*
 * Copyright © 2016 Federation of State Medical Boards
 * All Rights Reserved
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using System.Windows.Forms.Design;

using P3Net.IntegrationServices.UI;

namespace P3Net.IntegrationServices.Tasks.GenerateSsrs.UI
{
    internal class ReportPathEditor : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle ( ITypeDescriptorContext context )
        {
            return UITypeEditorEditStyle.Modal;
        }

        public override object EditValue ( ITypeDescriptorContext context, IServiceProvider provider, object value )
        {
            if (provider != null)
                m_editorService = provider.GetService<IWindowsFormsEditorService>();

            if (m_editorService != null)
            {
                var instance = context?.Instance?.GetSpecializedObject<GeneralViewNode>();
                if (instance == null)
                {
                    MessageBoxes.Error(null, "Error", "Unable to find connection information.");
                    return value;
                };

                if (String.IsNullOrWhiteSpace(instance.HttpConnection))
                {
                    MessageBoxes.Error(null, "Error", "No report server connection specified.");
                    return value;
                };

                var cm = instance.ConnectionService.GetConnections().GetConnection(instance.HttpConnection);
                var tree = new ReportPathTreeView(cm);
                tree.NodeMouseDoubleClick += (o, e) =>
                {
                    tree.SelectedNode = e.Node;
                    if (!String.IsNullOrWhiteSpace(tree.SelectedReportPath))
                        m_editorService.CloseDropDown();
                };

                m_editorService.DropDownControl(tree);

                var selectedPath = tree.SelectedReportPath;
                if (!String.IsNullOrWhiteSpace(selectedPath))
                    return selectedPath;

                return value;
            };

            return value;
        }

        private IWindowsFormsEditorService m_editorService;
    }
}
