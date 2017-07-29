using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Microsoft.SqlServer.Dts.Runtime;

namespace P3Net.IntegrationServices
{
    /// <summary>Provides extension methods for <see cref="HttpClientConnection"/>.</summary>
    public static class HttpClientConnectionExtensions
    {
        /// <summary>Creates a credentials object for the given connection.</summary>
        /// <param name="source">The source.</param>
        /// <returns>The credentials.</returns>
        public static ICredentials CreateCredentials ( this HttpClientConnection source )
        {
            if (source.UseServerCredentials)
                return CredentialCache.DefaultCredentials;

            if (!String.IsNullOrWhiteSpace(source.ServerDomain))
                return new NetworkCredential(source.ServerUserName, source.GetServerPassword(), source.ServerDomain);
            else
                return new NetworkCredential(source.ServerUserName, source.GetServerPassword());         
        }

        /// <summary>Determines if a connection is using Windows credentials.</summary>
        /// <param name="source">The source.</param>
        /// <returns><see langword="true"/> if it is using Windows authentication.</returns>
        public static bool UseWindowsCredentials ( this HttpClientConnection source )
        {
            return !String.IsNullOrWhiteSpace(source.ServerDomain);
        }
    }
}
