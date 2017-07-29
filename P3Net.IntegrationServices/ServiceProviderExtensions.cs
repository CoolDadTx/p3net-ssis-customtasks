/*
 * Copyright © 2016 Federation of State Medical Boards
 * All Rights Reserved
 */
using System;

namespace P3Net.IntegrationServices
{
    /// <summary>Extension methods for <see cref="IServiceProvider"/>.</summary>
    public static class ServiceProviderExtensions
    {
        /// <summary>Gets an instance of a service.</summary>
        /// <typeparam name="T">The service type or interface.</typeparam>
        /// <param name="source">The source.</param>
        /// <returns>The service, if found.</returns>
        public static T GetService<T> ( this IServiceProvider source ) where T : class
        {
            return source.GetService(typeof(T)) as T;
        }
    }
}
