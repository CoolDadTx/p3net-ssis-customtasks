/*
 * Copyright © 2016 Federation of State Medical Boards
 * All Rights Reserved
 */
using System;
using System.Collections.Generic;
using Microsoft.SqlServer.Dts.Runtime;

namespace P3Net.IntegrationServices
{
    /// <summary>Provides extensions for <see cref="VariableDispenser"/>.</summary>
    public static class VariableDispenserExtensions
    {
        /// <summary>Gets a variable's definition.</summary>
        /// <param name="source">The source.</param>
        /// <param name="name">The variable name.</param>
        /// <returns>The variable.</returns>
        /// <exception cref="KeyNotFoundException">The variable does not exist.</exception>
        /// <exception cref="Exception">An error occurred getting the variable.</exception>
        /// <remarks>
        /// The variable is locked for reading while the value is read.
        /// </remarks>
        public static Variable GetInfo ( this VariableDispenser source, string name )
        {
            var variable = source.TryGetInfo(name);
            if (variable != null)
                return variable;

            throw new KeyNotFoundException("Variable not found.");
        }

        /// <summary>Reads a variable's value.</summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="name">The variable name.</param>
        /// <returns>The value of the variable.</returns>
        /// <exception cref="KeyNotFoundException">The variable could not be found.</exception>
        /// <remarks>
        /// The variable is locked for reading while the value is being read.
        /// </remarks>
        public static object GetValue ( this VariableDispenser source, string name )
        {
            source.LockForRead(name);
            Variables variables = null;
            try
            {
                source.GetVariables(ref variables);
                
                return variables.GetVar(name, true);
            } finally
            {
                if (variables != null)
                    variables.Unlock();
            };
        }

        /// <summary>Reads a variable's value.</summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="name">The variable name.</param>
        /// <returns>The value of the variable.</returns>
        /// <exception cref="KeyNotFoundException">The variable could not be found.</exception>
        /// <remarks>
        /// The variable is locked for reading while the value is being read.
        /// </remarks>
        public static T GetValue<T> ( this VariableDispenser source, string name )
        {
            source.LockForRead(name);
            Variables variables = null;
            try
            {
                source.GetVariables(ref variables);

                return variables.GetVar<T>(name);                
            } finally
            {
                if (variables != null)
                    variables.Unlock();
            };
        }

        /// <summary>Gets a variable's definition.</summary>
        /// <param name="source">The source.</param>
        /// <param name="name">The variable name.</param>
        /// <returns>The variable, if it exists or <see langword="null"/> otherwise.</returns>
        /// <remarks>
        /// The variable is locked for reading while the value is read.
        /// </remarks>
        public static Variable TryGetInfo ( this VariableDispenser source, string name )
        {
            if (!source.Contains(name))
                return null;

            source.LockForRead(name);
            Variables variables = null;
            try
            {
                source.GetVariables(ref variables);

                return variables[name];
            } finally
            {
                if (variables != null)
                    variables.Unlock();
            };
        }

        /// <summary>Reads a variable's value.</summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="name">The variable name.</param>
        /// <returns>The value of the variable.</returns>
        /// <remarks>
        /// The variable is locked for reading while the value is being read.
        /// </remarks>
        public static T TryGetValue<T> ( this VariableDispenser source, string name )
        {
            return TryGetValue<T>(source, name, default(T));
        }

        /// <summary>Reads a variable's value.</summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="name">The variable name.</param>
        /// <param name="defaultValue">The default value to return if the variable does not exist.</param>
        /// <returns>The value of the variable.</returns>
        /// <remarks>
        /// The variable is locked for reading while the value is being read.
        /// </remarks>
        public static T TryGetValue<T> ( this VariableDispenser source, string name, T defaultValue )
        {
            if (!source.Contains(name))
                return defaultValue;

            Variables variables = null;
            try
            {
                source.LockForRead(name);
                source.GetVariables(ref variables);

                T value = defaultValue;
                if (variables.TryGetVar<T>(name, out value))
                    return value;

                return defaultValue;
            } finally
            {
                if (variables != null)
                    variables.Unlock();
            };
        }

        /// <summary>Writes a value to a variable.</summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="name">The variable name.</param>
        /// <param name="value">The value.</param>
        /// <exception cref="KeyNotFoundException">The variable could not be found.</exception>
        /// <remarks>
        /// The variable is locked for writing while the write is occurring.
        /// </remarks>
        public static void SetValue<T> ( this VariableDispenser source, string name, T value )
        {
            source.LockForWrite(name);
            Variables variables = null;
            try
            {
                source.GetVariables(ref variables);

                variables.SetVar<T>(name, value);
            } finally
            {
                if (variables != null)
                    variables.Unlock();
            };
        }
    }
}
