/*
 * Copyright © 2016 Federation of State Medical Boards
 * All Rights Reserved
 */
using System;
using System.Windows.Forms;

using Microsoft.SqlServer.Dts.Runtime;
using Microsoft.SqlServer.Dts.Runtime.Design;

namespace P3Net.IntegrationServices.UI.Tasks
{
    /// <summary>Provides a base implementation of <see cref="IDtsTaskUI"/>.</summary>
    /// <remarks>
    /// Derived types should do the following.
    /// <list type="number">
    ///    <item>Create the type as public.</item>
    ///    <item>Create a Form to hold the view, deriving from <see cref="DTSBaseTaskUI"/>.</item>
    ///    <item>Override <see cref="GetViewCore"/> to return the form.</item>
    /// </list>
    /// </remarks>
    public abstract class DtsTaskUI : IDtsTaskUI
    {
        /// <summary>??</summary>
        /// <param name="parent">The parent window.</param>
        public void Delete ( IWin32Window parent )
        {
            DeleteCore(parent);
        }

        /// <summary>Called to get the view.</summary>
        /// <returns>The view control.</returns>        
        public ContainerControl GetView ()
        {
            return GetViewCore();
        }

        /// <summary>Called to initialize the UI.</summary>
        /// <param name="taskHost">The task host.</param>
        /// <param name="serviceProvider">The service provider.</param>
        public void Initialize ( TaskHost taskHost, IServiceProvider serviceProvider )
        {
            Host = taskHost;
            ServiceProvider = serviceProvider;

            InitializeCore();
        }

        /// <summary>??</summary>
        /// <param name="parent">The parent window.</param>
        public void New ( IWin32Window parent )
        {
            NewCore(parent);
        }

        #region Protected Members

        /// <summary>Gets the host.</summary>
        protected TaskHost Host { get; private set; }

        /// <summary>Gets the service provider.</summary>
        protected IServiceProvider ServiceProvider { get; private set; }
        
        /// <summary>???</summary>
        /// <param name="parent">The parent window.</param>
        protected virtual void DeleteCore ( IWin32Window parent )
        { }

        /// <summary>Gets the underlying view.</summary>
        /// <returns>The view.</returns>
        /// <remarks>
        /// Derived types should return the form for the task.
        /// </remarks>
        protected abstract ContainerControl GetViewCore ();

        /// <summary>Initializes the UI.</summary>
        protected virtual void InitializeCore ()
        {
        }

        /// <summary>??</summary>
        /// <param name="parent">The parent window.</param>
        protected virtual void NewCore ( IWin32Window parent )
        { }
        #endregion
    }
}
