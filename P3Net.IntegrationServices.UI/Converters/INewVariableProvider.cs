/*
 * Copyright © 2016 Federation of State Medical Boards
 * All Rights Reserved
 */
using System;
using System.Collections.Generic;
using System.Linq;
using P3Net.IntegrationServices.UI.ComponentModel;

namespace P3Net.IntegrationServices.UI.Converters
{
    /// <summary>Provides an interface for customizing the generation of new variables using <see cref="VariablesStringConverter"/>.</summary>
    public interface INewVariableProvider
    {
        /// <summary>Gets the default values for a new variable.</summary>
        /// <param name="propertyName">The name of the property that the converter was applied to.</param>
        /// <returns>The variable defaults.</returns>
        NewVariableAttribute GetNewVariable ( string propertyName );
    }
}
