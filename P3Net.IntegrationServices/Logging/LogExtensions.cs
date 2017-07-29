/*
 * Copyright © 2016 Federation of State Medical Boards
 * All Rights Reserved
 */
using System;
using System.Linq;
using Microsoft.SqlServer.Dts.Runtime;

namespace P3Net.IntegrationServices.Logging
{
    /// <summary>Provides extension methods for raising events.</summary>
    public static class LogExtensions
    {        
        /// <summary>Logs a message.</summary>
        /// <param name="source">The source.</param>
        /// <param name="info">The message information.</param>
        public static void Log ( this IDTSComponentEvents source, LogInfo info )
        {
            LogCore(source, info);
        }

        /// <summary>Logs a message.</summary>
        /// <param name="source">The source.</param>
        /// <param name="info">The message information.</param>
        public static void Log ( this IDTSInfoEvents source, LogInfo info )
        {
            LogCore(source, info);
        }

        /// <summary>Logs an error.</summary>
        /// <param name="source">The source.</param>
        /// <param name="action">The action to produce the message.</param>
        public static void LogError ( this IDTSComponentEvents source, Action<LogInfoBuilder> action )
        {
            var builder = CreateLogInfoBuilder(LogType.Error);
            
            action(builder);

            LogCore(source, builder.GetInfo());
        }

        /// <summary>Logs an error.</summary>
        /// <param name="source">The source.</param>
        /// <param name="action">The action to produce the message.</param>
        public static void LogError ( this IDTSInfoEvents source, Action<LogInfoBuilder> action )
        {
            var builder = CreateLogInfoBuilder(LogType.Error);

            action(builder);

            LogCore(source, builder.GetInfo());
        }

        /// <summary>Logs an error.</summary>
        /// <param name="source">The source.</param>
        /// <param name="message">The message.</param>
        /// <param name="arguments">Optional message arguments.</param>
        public static void LogError ( this IDTSComponentEvents source, string message, params object[] arguments )
        {
            if (arguments != null && arguments.Any())
                message = String.Format(message, arguments);

            LogCore(source, new LogInfo() { Type = LogType.Error, Message = message });
        }

        /// <summary>Logs an error.</summary>
        /// <param name="source">The source.</param>
        /// <param name="message">The message.</param>
        /// <param name="arguments">Optional message arguments.</param>
        public static void LogError ( this IDTSInfoEvents source, string message, params object[] arguments )
        {
            if (arguments != null && arguments.Any())
                message = String.Format(message, arguments);

            LogCore(source, new LogInfo() { Type = LogType.Error, Message = message });
        }        

        /// <summary>Logs an informational message.</summary>
        /// <param name="source">The source.</param>
        /// <param name="action">The action to produce the message.</param>
        public static void LogInformation(this IDTSComponentEvents source, Action<LogInfoBuilder> action )
        {
            var builder = CreateLogInfoBuilder(LogType.Information);

            action(builder);

            LogCore(source, builder.GetInfo());
        }

        /// <summary>Logs an informational message.</summary>
        /// <param name="source">The source.</param>
        /// <param name="action">The action to produce the message.</param>
        public static void LogInformation ( this IDTSInfoEvents source, Action<LogInfoBuilder> action )
        {
            var builder = CreateLogInfoBuilder(LogType.Information);

            action(builder);

            LogCore(source, builder.GetInfo());
        }

        /// <summary>Logs an informational message.</summary>
        /// <param name="source">The source.</param>
        /// <param name="message">The message.</param>
        /// <param name="arguments">Optional message arguments.</param>
        public static void LogInformation ( this IDTSComponentEvents source, string message, params object[] arguments )
        {
            if (arguments != null && arguments.Any())
                message = String.Format(message, arguments);

            LogCore(source, new LogInfo() { Type = LogType.Information, Message = message });
        }

        /// <summary>Logs an informational message.</summary>
        /// <param name="source">The source.</param>
        /// <param name="message">The message.</param>
        /// <param name="arguments">Optional message arguments.</param>
        public static void LogInformation ( this IDTSInfoEvents source, string message, params object[] arguments )
        {
            if (arguments != null && arguments.Any())
                message = String.Format(message, arguments);

            LogCore(source, new LogInfo() { Type = LogType.Information, Message = message });
        }

        /// <summary>Logs a warning.</summary>
        /// <param name="source">The source.</param>
        /// <param name="action">The action to produce the message.</param>
        public static void LogWarning(this IDTSComponentEvents source, Action<LogInfoBuilder> action )
        {
            var builder = CreateLogInfoBuilder(LogType.Warning);

            action(builder);

            LogCore(source, builder.GetInfo());
        }

        /// <summary>Logs a warning.</summary>
        /// <param name="source">The source.</param>
        /// <param name="action">The action to produce the message.</param>
        public static void LogWarning ( this IDTSInfoEvents source, Action<LogInfoBuilder> action )
        {
            var builder = CreateLogInfoBuilder(LogType.Warning);

            action(builder);

            LogCore(source, builder.GetInfo());
        }

        /// <summary>Logs a warning.</summary>
        /// <param name="source">The source.</param>
        /// <param name="message">The message.</param>
        /// <param name="arguments">Optional message arguments.</param>
        public static void LogWarning ( this IDTSComponentEvents source, string message, params object[] arguments )
        {
            if (arguments != null && arguments.Any())
                message = String.Format(message, arguments);

            LogCore(source, new LogInfo() { Type = LogType.Warning, Message = message });
        }

        /// <summary>Logs a warning.</summary>
        /// <param name="source">The source.</param>
        /// <param name="message">The message.</param>
        /// <param name="arguments">Optional message arguments.</param>
        public static void LogWarning ( this IDTSInfoEvents source, string message, params object[] arguments )
        {
            if (arguments != null && arguments.Any())
                message = String.Format(message, arguments);

            LogCore(source, new LogInfo() { Type = LogType.Warning, Message = message });
        }

        #region Private Members

        private static LogInfoBuilder CreateLogInfoBuilder( LogType type)
        {
            var builder = new LogInfoBuilder();            
            builder.LogType(type);               

            return builder;
        }
        
        private static void LogCore(IDTSComponentEvents source, LogInfo info)
        {                     
            switch (info.Type)
            {
                case LogType.Error: source.FireError(info.Code, info.Component, info.MessageAndException(), null, 0); break;
                case LogType.Warning: source.FireWarning(info.Code, info.Component, info.MessageAndException(), null, 0); break;
                case LogType.Information:
                {
                    bool fireAgain = false;
                    source.FireInformation(info.Code, info.Component, info.MessageAndException(), null, 0, ref fireAgain); break;
                };
            };
        }

        private static void LogCore ( IDTSInfoEvents source, LogInfo info )
        {
            switch (info.Type)
            {
                case LogType.Error: source.FireError(info.Code, info.Component, info.MessageAndException(), null, 0); break;
                case LogType.Warning: source.FireWarning(info.Code, info.Component, info.MessageAndException(), null, 0); break;
                case LogType.Information:
                {
                    bool fireAgain = false;
                    source.FireInformation(info.Code, info.Component, info.MessageAndException(), null, 0, ref fireAgain);
                    break;
                };
            };
        }
        #endregion
    }
}
