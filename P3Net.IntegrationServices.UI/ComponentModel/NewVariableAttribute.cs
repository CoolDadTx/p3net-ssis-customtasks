/*
 * Copyright © 2016 Federation of State Medical Boards
 * All Rights Reserved
 */
using System;


namespace P3Net.IntegrationServices.UI.ComponentModel
{
    /// <summary>Provides default values for a new variable.</summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class NewVariableAttribute : Attribute
    {
        #region Construction

        /// <summary>Initializes an instance of the <see cref="NewVariableAttribute"/> class.</summary>
        /// <param name="name">The name of the new variable.</param>
        /// <param name="type">The type of the new variable.</param>
        public NewVariableAttribute ( string name, Type type )
        {
            Name = name ?? "";
            Scope = "User";
            Type = type;
        }
        #endregion

        /// <summary>Gets the name of the variable.</summary>
        public string Name { get; set; }

        /// <summary>Gets or sets the default namespace.</summary>
        /// <value>The default is User.</value>
        public string Scope { get; set; }

        /// <summary>Gets the default type.</summary>
        public Type Type { get; set; }
    }
}
