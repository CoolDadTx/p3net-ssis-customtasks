/*
 * Copyright © 2016 Federation of State Medical Boards
 * All Rights Reserved
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace P3Net.IntegrationServices.UI
{
    /// <summary>Provides helper methods to show messages to the user.</summary>
    public static class MessageBoxes
    {
        /// <summary>Displays an error message.</summary>
        /// <param name="parent">The parent window.</param>
        /// <param name="title">The window title.</param>
        /// <param name="error">The error.</param>
        public static void Error ( IWin32Window parent, string title, Exception error )
        {
            MessageBox.Show(parent, error.Message, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /// <summary>Displays an error message.</summary>
        /// <param name="parent">The parent window.</param>
        /// <param name="title">The window title.</param>
        /// <param name="error">The error.</param>
        public static void Error ( IWin32Window parent, string title, string error )
        {
            MessageBox.Show(parent, error, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
