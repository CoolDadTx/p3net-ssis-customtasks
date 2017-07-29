/*
 * Copyright © 2016 Federation of State Medical Boards
 * All Rights Reserved
 */
using System;
using System.Windows.Forms;

using Microsoft.DataTransformationServices.Controls;
using Microsoft.SqlServer.Dts.Runtime;
using Microsoft.SqlServer.Dts.Runtime.Design;

namespace P3Net.IntegrationServices.UI.Tasks
{
    /// <summary>Provides a base implementation of <see cref="IDTSTaskUIView"/>.</summary>
    /// <remarks>
    /// For derived types do the following:
    /// <list type="number">
    ///    <item>Create the type as internal.</item>
    ///    <item>Override <see cref="OnInitializeCore" /> to initialize the view data.</item>
    ///    <item>Override <see cref="Save" /> to save the data.</item>
    ///    <item>Optionally override <see cref="OnSelectionCore"/> to update the data based upon other views' changes.</item>
    /// </list>
    /// </remarks>
    public abstract partial class DtsTaskUIView : UserControl, IDTSTaskUIView
    {
        #region Construction

        public DtsTaskUIView ()
        {
            InitializeComponent();
        }
        #endregion

        /// <summary>Called to save changes to the view.</summary>
        /// <param name="taskHost">The host.</param>
        /// <remarks>
        /// Derived classes should not override this method.  Implement <see cref="Save"/> instead.
        /// </remarks>
        public void OnCommit ( object taskHost )
        {
            Host = taskHost as TaskHost;
            if (Host == null)
                throw new ArgumentException("Host is invalid.", nameof(taskHost));            

            //Commit is called even if the view is never initialized so don't bother calling save if we haven't initialized it
            if (m_initialized)
                Save();
        }

        /// <summary>Called to initialize the view.</summary>
        /// <param name="treeHost">The host.</param>
        /// <param name="viewNode">The tree node.</param>
        /// <param name="taskHost">The task host.</param>
        /// <param name="connections">The connections.</param>
        /// <remarks>
        /// Derived classes should not override this method.  Implement <see cref="OnInitializeCore"/> instead.
        /// </remarks>
        public void OnInitialize ( IDTSTaskUIHost treeHost, TreeNode viewNode, object taskHost, object connections )
        {
            ViewHost = treeHost;
            ViewNode = viewNode;
            Host = taskHost as TaskHost;
            if (Host == null)
                throw new ArgumentException("Host is invalid.", nameof(taskHost));

            ConnectionService = connections as IDtsConnectionService;
            if (ConnectionService == null)
                throw new ArgumentException("Connection service is invalid.", nameof(connections));
            
            OnInitializeCore();
            m_initialized = true;
        }

        /// <summary>Called when the view loses selection.</summary>
        /// <param name="bCanLeaveView"><see langword="true"/> to allow the view to lose selection.</param>
        /// <param name="reason">The reason why the view cannot be changed.</param>
        /// <remarks>
        /// Derived classes should not override this method.  Implement <see cref="OnLoseSelectionCore"/> instead.
        /// </remarks>
        public void OnLoseSelection ( ref bool bCanLeaveView, ref string reason )
        {
            bCanLeaveView = OnLoseSelectionCore(ref reason);
        }

        /// <summary>Called when the view gets selection..</summary>
        /// <remarks>
        /// Derived classes should not override this method.  Implement <see cref="OnSelectionCore"/> instead.
        /// </remarks>
        public void OnSelection ()
        {
            OnSelectionCore();
        }

        /// <summary>Called to validate the view changes.</summary>
        /// <param name="bViewIsValid"><see langword="true"/> if the view is valid.</param>
        /// <param name="reason">The reason the view is not valid.</param>
        /// <remarks>
        /// Derived classes should not override this method.  Implement <see cref="OnvalidateCore"/> instead.
        /// </remarks>
        public void OnValidate ( ref bool bViewIsValid, ref string reason )
        {
            bViewIsValid = !OnValidateCore(ref reason);
        }

        #region Protected Members
        
        /// <summary>Gets the design-time connections.</summary>
        protected IDtsConnectionService ConnectionService { get; private set; }

        /// <summary>Gets the task host.</summary>
        protected TaskHost Host { get; private set; }

        /// <summary>Gets the variables at design-time.</summary>
        protected IDtsVariableService VariableService
        {
            get { return Host?.Site?.GetService(typeof(IDtsVariableService)) as IDtsVariableService; }
        }

        /// <summary>Gets the view's host.</summary>
        protected IDTSTaskUIHost ViewHost { get; private set; }

        /// <summary>Gets the node hosting the view.</summary>
        protected TreeNode ViewNode { get; private set; }

        /// <summary>Gets the task associated with the view.</summary>
        /// <typeparam name="T">The type of the task.</typeparam>
        /// <returns>The task, if available.</returns>
        protected T GetTask<T> ( ) where T : Task
        {
            return Host.GetTask<T>();
        }

        /// <summary>Gets another view in the task UI.</summary>
        /// <typeparam name="T">The type of the desired view.</typeparam>
        /// <returns>The view, if found.</returns>
        protected T GetView<T> () where T : class, IDTSTaskUIView
        {
            foreach (TreeNode node in ViewNode.TreeView.Nodes)
            {
                var view = ViewHost.GetView(node) as T;
                if (view != null)
                    return view;
            };

            return null;
        }

        /// <summary>Initializes the view.</summary>    
        /// <remarks>
        /// Derived types should use this method to initialize the view.  
        /// <para />
        /// During initialization the task can be used to initialize the view's values.
        /// </remarks>
        protected abstract void OnInitializeCore ( );

        /// <summary>Called when the view loses selection.</summary>        
        /// <param name="reason">The reason the view cannot lose selection.</param>
        /// <returns><see langword="true"/> to allow the view to lose selection.</returns>
        /// <remarks>
        /// The default implementaiton returns true.
        /// </remarks>
        protected virtual bool OnLoseSelectionCore ( ref string reason )
        {
            return true;
        }

        /// <summary>Called when the view gets selection.</summary>
        /// <remarks>
        /// The view gets selection whenever the user clicks the corresponding tab.  Other views may have changed data since
        /// the view was selected last so the view should refresh any data that is provided by other views (<see cref="GetView"/>) rather than retrieving
        /// it from the underlying task.          
        /// </remarks>        
        protected virtual void OnSelectionCore ()
        {
        }

        /// <summary>Called to validate the view settings.</summary>
        /// <param name="reason">The reason the view is not valid.</param>
        /// <returns><see langword="true"/> if the view data is valid.</returns>
        protected virtual bool OnValidateCore ( ref string reason )
        {
            return true;
        }

        /// <summary>Saves the data from the view.</summary>
        protected abstract void Save ( );
        #endregion

        #region Private Members

        private bool m_initialized;
        #endregion
    }
}
