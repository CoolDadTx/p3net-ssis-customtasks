/*
 * Copyright © 2016 Federation of State Medical Boards
 * All Rights Reserved
 */
using System;
using System.ServiceModel;

using Microsoft.SqlServer.Dts.Runtime;

namespace P3Net.IntegrationServices
{
    /// <summary>Provides extension methods for working with WCF.</summary>
    public static class WcfConnectionExtensions
    {
        /// <summary>Creates a basic HTTP binding from an HTTP client connection.</summary>
        /// <param name="source">The connection.</param>
        /// <returns>The binding.</returns>
        /// <remarks>
        /// The binding uses default binding settings except as indicated below.
        /// <list type="bullet">
        /// <item>BypassProxyOnLocal = true</item>
        /// </list>
        /// <para />
        /// The binding is configured to use transport security if HTTPS is specified otherwise it uses no security.
        /// <para />
        /// The binding is configured to use NTLM credentials if the connection is using a domain for credentials.
        /// </remarks>
        public static BasicHttpBinding CreateBasicHttpBinding ( this HttpClientConnection source )
        {
            //source.UseSecureConnection isn't true when HTTPS is applied so parse this manually
            var uri = new Uri(source.ServerURL);
            var binding = uri.Scheme == "https" ? new BasicHttpBinding(BasicHttpSecurityMode.Transport) : new BasicHttpBinding(BasicHttpSecurityMode.None);

            binding.BypassProxyOnLocal = true;

            if (source.UseWindowsCredentials())
            {
                binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Ntlm;
                //binding.Security.Mode = BasicHttpSecurityMode.TransportCredentialOnly;
            };
            
            return binding;
        }

        /// <summary>Sets the client credentials on a WCF client.</summary>
        /// <typeparam name="T">The service interface.</typeparam>
        /// <param name="source">The client.</param>
        /// <param name="connection">The HTTP connection.</param>
        /// <remarks>
        /// Both Windows and basic credentials are supported.
        /// </remarks>
        public static void SetClientCredentials<T> ( this ClientBase<T> source, HttpClientConnection connection ) where T : class
        {
            //Windows
            if (connection.UseWindowsCredentials())
            {
                var creds = source.ClientCredentials.Windows;
                
                creds.ClientCredential.UserName = connection.ServerUserName;
                creds.ClientCredential.Password = connection.GetServerPassword();
                creds.ClientCredential.Domain = connection.ServerDomain;                
            } else if (!String.IsNullOrWhiteSpace(connection.ServerUserName))
            {
                var creds = source.ClientCredentials;

                creds.UserName.UserName = connection.ServerUserName;
                creds.UserName.Password = connection.GetServerPassword();
            };
        }        
    }
}
