using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.SqlServer.Dts.Runtime;

namespace P3Net.IntegrationServices
{
    /// <summary>Provides extension methods for <see cref="Connections"/>.</summary>
    public static class ConnectionsExtensions
    {                
        /// <summary>Gets a connection given its name.</summary>
        /// <param name="source">The source.</param>
        /// <param name="connectionName">The connection name.</param>
        /// <returns>The connection.</returns>
        /// <exception cref="Exception">Connection could not be found.</exception>
        public static ConnectionManager GetConnection ( this Connections source, string connectionName )
        {
            var conn = TryGetConnection(source, connectionName);
            if (conn == null)
                throw new Exception($"Connection '{connectionName}' not found.");

            return conn;
        }

        /// <summary>Gets a connection given its name.</summary>
        /// <param name="source">The source.</param>
        /// <param name="connectionName">The connection name.</param>
        /// <returns>The connection, if found, or <see langword="null"/> otherwise.</returns>
        public static ConnectionManager TryGetConnection ( this Connections source, string connectionName )
        {
            if (source == null)
                return null;

            if (!String.IsNullOrWhiteSpace(connectionName))
            {
                try
                {
                    return source[connectionName];
                } catch
                { /* Ignore */ };
            };

            return null;
        }
    }
}
