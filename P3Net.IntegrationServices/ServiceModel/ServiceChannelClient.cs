/*
 * Copyright © 2013 Federation of State Medical Boards
 * All Rights Reserved
 */
using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading;

using Microsoft.SqlServer.Dts.Runtime;

namespace P3Net.IntegrationServices.ServiceModel
{
    /// <summary>Provides a base class for WCF service proxies.</summary>
    /// <typeparam name="T">The service interface.</typeparam>
    public abstract class ServiceChannelClient<T> : ISupportsPersistentChannel where T: class
    {
        #region Construction

        /// <summary>Initializes an instance of the <see cref="ServiceChannelClient"/> class.</summary>
        /// <param name="connection">The HTTP client connection.</param>
        /// <exception cref="ArgumentNullException"><paramref name="connection"/> is <see langword="null"/>.</exception>
        protected ServiceChannelClient ( HttpClientConnection connection )
        {
            if (connection == null)
                throw new ArgumentNullException(nameof(connection));

            ClientConnection = connection;
        }        
        #endregion

        #region Protected Members

        /// <summary>Gets the underlying connection.</summary>
        protected HttpClientConnection ClientConnection { get; private set; }

        /// <summary>Determines if the channel is already open.</summary>
        protected bool HasOpenChannel
        {
            get { return m_channel != null; }
        }

        /// <summary>Closes the persistent channel if it is open.</summary>
        protected virtual void CloseChannel ()
        {
            var channel = Interlocked.Exchange(ref m_channel, null);
            if (channel != null)
                channel.Close();
        }

        /// <summary>Creates the binding for the connection.</summary>
        /// <returns>The binding.</returns>
        /// <remarks>
        /// The default implementation creates a basic HTTP binding based upon the client connection settings.  The 
        /// credentials and HTTPS options are used to configure security.  The binding will use the default message
        /// properties.  Derived types should override this method to alter the bindings being used.
        /// </remarks>
        protected virtual Binding CreateBinding ()
        {
            return ClientConnection.CreateBasicHttpBinding();
        }
                
        /// <summary>Creates the endpoint for the connection.</summary>
        /// <returns>The endpoint address.</returns>
        /// <remarks>
        /// The default implementation uses the client connection URL.
        /// </remarks>
        protected virtual EndpointAddress CreateEndpoint ( )
        {
            return new EndpointAddress(ClientConnection.ServerURL);
        }

        /// <summary>Creates an instance of the client wrapper.</summary>
        /// <returns>The client wrapper.</returns>
        protected virtual ServiceClientWrapper<T> CreateInstance ()
        {
            var binding = CreateBinding();
            var address = CreateEndpoint();
            
            var client = new ServiceClientWrapper<T>(binding, address);

            //Apply any credentials as needed
            client.SetClientCredentials(ClientConnection);

            return client;
        }

        /// <summary>Invokes a method on the channel.</summary>
        /// <param name="action">The action to perform.</param>
        protected virtual void InvokeMethod ( Action<T> action )
        {
            if (HasOpenChannel)
                action(m_channel.Client);
            else
            {
                using (var proxy = CreateInstance())
                {
                    action(proxy.Client);
                };
            };
        }

        /// <summary>Invokes a method on the channel.</summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="action">The action to perform.</param>
        protected virtual TResult InvokeMethod<TResult> ( Func<T, TResult> action )
        {
            if (HasOpenChannel)
                return action(m_channel.Client);
            else
            {
                using (var proxy = CreateInstance())
                {
                    return action(proxy.Client);
                };
            };
        }

        /// <summary>Opens the persistent channel if it is not open.</summary>
        protected virtual void OpenChannel ()
        {
            if (m_channel == null)
            {
                var channel = CreateInstance();
                var oldChannel = Interlocked.CompareExchange(ref m_channel, channel, null);
                if (oldChannel != null)
                    channel.Close();
            };
        }
        #endregion

        #region ISupportsPersistentChannel
        
        void ISupportsPersistentChannel.Close ()
        {
            CloseChannel();
        }
        
        void ISupportsPersistentChannel.Open ()
        {
            OpenChannel();
        }
        #endregion

        #region Private Members
        
        private ServiceClientWrapper<T> m_channel;
        
        #endregion
    }
}
