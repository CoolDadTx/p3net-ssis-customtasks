/*
 * Copyright © 2016 Federation of State Medical Boards
 * All Rights Reserved
 */
using System;

namespace P3Net.IntegrationServices.Logging
{
    /// <summary>Helper class for building <see cref="LogInfo" /> objects.</summary>
    public class LogInfoBuilder
    {
        public LogInfoBuilder Code ( int code )
        {
            m_info.Code = code;

            return this;
        }

        public LogInfoBuilder Component ( string name )
        {
            m_info.Component = name ?? "";

            return this;
        }

        public LogInfoBuilder Exception ( Exception error )
        {
            m_info.Exception = error;

            return this;
        }

        public LogInfoBuilder LogType ( LogType type )
        {
            m_info.Type = type;

            return this;
        }

        public LogInfoBuilder Message ( string message )
        {
            return Message(message, null);
        }

        public LogInfoBuilder Message ( string message, params object[] args )
        {
            m_info.Message = (args != null) ? String.Format(message, args) : message;

            return this;
        }

        public LogInfo GetInfo ()
        {
            return m_info;
        }

        private LogInfo m_info = new LogInfo();
    }
}
