/*
 * Copyright © 2013 Federation of State Medical Boards
 * All Rights Reserved
 */
using System;
using System.Collections.Generic;

namespace P3Net.IntegrationServices.ServiceModel
{
    /// <summary>Provides an interface for keeping a persisted connection to a WCF service open.</summary>
    public interface ISupportsPersistentChannel
    {
        /// <summary>Closes the channel if it is open.</summary>
        void Close ();

        /// <summary>Opens the channel if it is not already open.</summary>
        void Open ();
    }
}
