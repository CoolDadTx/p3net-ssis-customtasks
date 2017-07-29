/*
 * Copyright © 2016 Federation of State Medical Boards
 * All Rights Reserved
 */
using System;
using System.Text;

namespace P3Net.IntegrationServices.Logging
{
    /// <summary>Provides information for the logging extensions.</summary>
    public class LogInfo
    {
        public int Code { get; set; }
        public string Component { get; set; }
        public Exception Exception { get; set; }
        public string Message { get; set; }

        public LogType Type { get; set; }

        public string MessageAndException ()
        {
            if (Exception == null)
                return Message;

            var builder = new StringBuilder(Message);
            builder.AppendLine();

            var error = Exception;
            do
            {
                builder.AppendLine(error.Message + " (" + error.GetType().FullName + ")");
                builder.AppendLine("\t" + error.StackTrace);
                error = error.InnerException;
            } while (error != null);

            return builder.ToString();
        }
    }
}
