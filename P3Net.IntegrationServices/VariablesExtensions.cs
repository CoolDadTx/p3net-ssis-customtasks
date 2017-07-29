/*
 * Copyright © 2016 Federation of State Medical Boards
 * All Rights Reserved
 */
using System;
using System.Collections.Generic;
using Microsoft.SqlServer.Dts.Runtime;

namespace P3Net.IntegrationServices
{
    /// <summary>Provides extension methods for <see cref="Variables"/>.</summary>
    public static class VariablesExtensions
    {
        /// <summary>Gets the value of a variable.</summary>
        /// <param name="source">The source.</param>
        /// <param name="index">The index.</param>
        /// <param name="allowNull"><see langword="true"/> to allow <see langword="null"/> being returned.</param>
        /// <returns>The variable's value.</returns>
        /// <exception cref="KeyNotFoundException">The variable does not exist.</exception>
        /// <exception cref="InvalidOperationException">The variable is <see langword="null"/> and <paramref name="allowNull"/> is <see langword="false"/>.</exception>
        public static object GetVar ( this Variables source, object index, bool allowNull )
        {
            if (!source.Contains(index))
                throw new KeyNotFoundException(String.Format("Variable '{0}' not found.", index));

            var v = source[index];
            if (IsNull(v))
            {
                if (!allowNull)
                    throw new InvalidOperationException(String.Format("Variable '{0}' is null.", index));

                //Null in SSIS uses new object so ensure we return null
                return null;
            };

            return v.Value;
        }

        /// <summary>Gets the value of a variable.</summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="index">The index.</param>
        /// <returns>The variable's value.</returns>
        /// <exception cref="KeyNotFoundException">The variable does not exist.</exception>
        /// <exception cref="InvalidOperationException">The variable is <see langword="null"/>.</exception>
        /// <exception cref="InvalidCastException">The value cannot be converted to <typeparamref name="T"/>.</exception>
        /// <exception cref="FormatException">The value cannot be converted to <typeparamref name="T"/>.</exception>
        public static T GetVar<T> ( this Variables source, object index )
        {            
            var value = GetVar(source, index, false);

            return ChangeType<T>(value);
        }

        /// <summary>Gets the value of a variable or its default.</summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="index">The index.</param>
        /// <returns>The variable's value or the default value if it is <see langword="null"/>.</returns>
        /// <exception cref="KeyNotFoundException">The variable does not exist.</exception>
        /// <exception cref="InvalidCastException">The value cannot be converted to <typeparamref name="T"/>.</exception>
        /// <exception cref="FormatException">The value cannot be converted to <typeparamref name="T"/>.</exception>
        public static T GetVarOrDefault<T> ( this Variables source, object index )
        {
            return GetVarOrDefault<T>(source, index, default(T));
        }

        /// <summary>Gets the value of a variable or its default.</summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="index">The index.</param>
        /// <param name="defaultValue">The default value to use.</param>
        /// <returns>The variable's value or <paramref name="defaultValue" /> if it is <see langword="null"/>.</returns>
        /// <exception cref="KeyNotFoundException">The variable does not exist.</exception>
        /// <exception cref="InvalidCastException">The value cannot be converted to <typeparamref name="T"/>.</exception>
        /// <exception cref="FormatException">The value cannot be converted to <typeparamref name="T"/>.</exception>
        public static T GetVarOrDefault<T> ( this Variables source, object index, T defaultValue )
        {
            var value = GetVar(source, index, true);
            if (IsNull(value))
                return defaultValue;

            return ChangeType<T>(value);
        }

        /// <summary>Gets the value of a variable, if it exists.</summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="index">The index.</param>        
        /// <param name="value">The variable's value.</param>
        /// <returns><see langword="true"/> if the variable's value is retrieved.</returns>
        /// <exception cref="InvalidCastException">The value cannot be converted to <typeparamref name="T"/>.</exception>
        /// <exception cref="FormatException">The value cannot be converted to <typeparamref name="T"/>.</exception>
        public static bool TryGetVar<T>(this Variables source, object index, out T value )
        {
            if (!source.Contains(index))
            {
                value = default(T);
                return false;
            };

            var v = source[index];
            if (IsNull(v))
            {
                value = default(T);
                return false;
            };

            value = ChangeType<T>(v.Value);
            return true;
        }

        /// <summary>Sets the value of a variable.</summary>
        /// <typeparam name="T">The type of the variable.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="index">The index.</param>
        /// <param name="value">The variable's value.</param>
        public static void SetVar<T>(this Variables source, object index, T value)
        {
            //TFS25033 When clearing an object, create a new object instead otherwise SSIS will change the type to Empty            
            source[index].Value = (object)value ?? new object();
        }

        #region Private Members

        private static bool IsNull ( object value )
        {
            if (value == null)
                return true;

            var dtsVar = value as Microsoft.SqlServer.Dts.Runtime.Variable;
            if (dtsVar != null)
            {
                if (dtsVar.DataType == TypeCode.DBNull || dtsVar.DataType == TypeCode.Empty)
                    return true;

                return IsNull(dtsVar.Value);
            };

            //In SSIS a "null" value is of type object so we'll assume that if it is truly just "object" then it must be null
            return value.GetType() == typeof(object);
        }

        private static T ChangeType<T> ( object value )
        {
            if (IsNull(value))
                return default(T);

            return (T)Convert.ChangeType(value, typeof(T));
        }
        #endregion
    }    
}
