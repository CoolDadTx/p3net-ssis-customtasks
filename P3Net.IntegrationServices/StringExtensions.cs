/*
 * Copyright © 2011 Federation of State Medical Boards
 * All rights reserved.
 * 
 * From code copyright (c) 2004 Michael Taylor  
 */
using System;
using System.Collections.Generic;

using System.Globalization;
using System.Linq;
using System.Text;

//From Apollo

namespace P3Net.IntegrationServices
{
    /// <summary>Provides extension methods for strings.</summary>
    public static class StringExtensions
    {
        /// <summary>Gets the string value unless it is <see langword="null"/> or empty in which case it returns <see langword="null"/>.</summary>
        /// <param name="source">The source.</param>
        /// <returns>The string value or <see langword="null"/> if it is <see langword="null"/> or empty.</returns>
        public static string AsNullIfEmpty ( this string source )
        {
            return !String.IsNullOrEmpty(source) ? source : null;
        }

        #region Coalesce

        /// <summary>Returns the first value that is not <see langword="null"/>.</summary>
        /// <param name="values">The values to examine.</param>
        /// <returns>The first value that is not <see langword="null"/>.  If all values are <see langword="null"/> then 
        /// <see langword="null"/> is returned.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="values"/> is <see langword="null"/>.</exception>
        public static string Coalesce ( params string[] values )
        {
            return Coalesce(StringCoalesceOptions.None, values as IEnumerable<string>);
        }

        /// <summary>Returns the first value that is not <see langword="null"/>.</summary>
        /// <param name="options">The options to use.</param>
        /// <param name="values">The values to examine.</param>
        /// <returns>The first value that is not <see langword="null"/>.  If all values are <see langword="null"/> then 
        /// <see langword="null"/> is returned.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="values"/> is <see langword="null"/>.</exception>
        public static string Coalesce ( StringCoalesceOptions options, params string[] values )
        {
            return Coalesce(options, values as IEnumerable<string>);
        }

        /// <summary>Returns the first value that is not <see langword="null"/>.</summary>
        /// <param name="values">The values to examine.</param>
        /// <returns>The first value that is not <see langword="null"/>.  If all values are <see langword="null"/> then 
        /// <see langword="null"/> is returned.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="values"/> is <see langword="null"/>.</exception>
        public static string Coalesce ( IEnumerable<string> values )
        {
            return Coalesce(StringCoalesceOptions.None, values);
        }

        /// <summary>Returns the first value that is not <see langword="null"/>.</summary>
        /// <param name="options">The options to use.</param>
        /// <param name="values">The values to examine.</param>
        /// <returns>The first value that is not <see langword="null"/>.  If all values are <see langword="null"/> then 
        /// <see langword="null"/> is returned.</returns>
        public static string Coalesce ( StringCoalesceOptions options, IEnumerable<string> values )
        {
            values = values ?? new string[0];

            IEnumerable<string> query;
            if (options.HasFlag(StringCoalesceOptions.SkipEmpty))
                query = values.Where(x => !String.IsNullOrEmpty(x));
            else
                query = values.Where(x => x != null);

            return query.FirstOrDefault();
        }
        #endregion

        #region Combine

        /// <summary>Combines a set of strings using the given separator.</summary>
        /// <param name="separator">The separator to use.</param>        
        /// <param name="values">The list of strings to combine.</param>
        /// <returns>The combined string.</returns>
        /// <remarks>
        /// The separator is only inserted between values when the separator does not appear at the end of the left obj or the beginning of the right obj.  
        /// If one of the strings is <see langword="null"/> or empty then it is ignored.  This differs from how <see cref="O:String.Join"/> behaves.
        /// </remarks>
        /// <seealso cref="O:String.Join"/>
        public static string Combine ( string separator, IEnumerable<string> values )
        {
            if (values == null)
                return "";

            return InternalCombine(separator ?? "", values);
        }

        /// <summary>Combines a set of strings using the given separator.</summary>
        /// <param name="separator">The separator to use.</param>        
        /// <param name="values">The list of strings to combine.</param>
        /// <returns>The combined string.</returns>
        /// <remarks>
        /// The separator is only inserted between values when the separator does not appear at the end of the left obj or the beginning of the right obj.  
        /// If one of the strings is <see langword="null"/> or empty then it is ignored.  This differs from how <see cref="O:String.Join"/> behaves.
        /// </remarks>
        /// <seealso cref="O:String.Join"/>
        public static string Combine ( string separator, params string[] values )
        {
            if (values == null || !values.Any())
                return "";

            return InternalCombine(separator ?? "", values);
        }
        #endregion

        #region EnsureEndsWith

        /// <summary>Ensures a string ends with a specific delimiter.</summary>
        /// <param name="source">The source.</param>
        /// <param name="delimiter">The delimiter to look for.</param>
        /// <returns>The string with the delimiter added.  If the source is <see langword="null"/> or empty then the delimiter is returned.</returns>               
        /// <remarks>
        /// The comparison is done using the current culture's case sensitive comparison.
        /// </remarks>
        public static string EnsureEndsWith ( this string source, char delimiter )
        {
            return EnsureEndsWith(source, delimiter.ToString());
        }

        /// <summary>Ensures a string ends with a specific delimiter.</summary>
        /// <param name="source">The source.</param>
        /// <param name="delimiter">The delimiter to look for.</param>
        /// <param name="comparison">The comparison to perform.</param>
        /// <returns>The string with the delimiter added.  If the source is <see langword="null"/> or empty then the delimiter is returned.</returns>                 
        public static string EnsureEndsWith ( this string source, char delimiter, StringComparison comparison )
        {
            return EnsureEndsWith(source, delimiter.ToString(), comparison);
        }

        /// <summary>Ensures a string ends with a specific delimiter.</summary>
        /// <param name="source">The source.</param>
        /// <param name="delimiter">The delimiter to look for.</param>
        /// <param name="ignoreCase"><see langword="true"/> to ignore case.</param>
        /// <param name="culture">The culture to use.  If <see langword="null"/> then the current culture is used.</param>
        /// <returns>The string with the delimiter added.  If the source is <see langword="null"/> or empty then the delimiter is returned.</returns>                 
        public static string EnsureEndsWith ( this string source, char delimiter, bool ignoreCase, CultureInfo culture )
        {
            return EnsureEndsWith(source, delimiter.ToString(), ignoreCase, culture);
        }

        /// <summary>Ensures a string ends with a specific delimiter.</summary>
        /// <param name="source">The source.</param>
        /// <param name="delimiter">The delimiter to look for.</param>
        /// <returns>The string with the delimiter added.  If the source is <see langword="null"/> or empty then the delimiter is returned.</returns>               
        /// <remarks>
        /// The comparison is done using the current culture's case sensitive comparison.
        /// </remarks>
        public static string EnsureEndsWith ( this string source, string delimiter )
        {
            return EnsureEndsWith(source, delimiter, false, null);
        }

        /// <summary>Ensures a string ends with a specific delimiter.</summary>
        /// <param name="source">The source.</param>
        /// <param name="delimiter">The delimiter to look for.</param>
        /// <param name="comparison">The comparison to perform.</param>
        /// <returns>The string with the delimiter added.  If the source is <see langword="null"/> or empty then the delimiter is returned.</returns>               
        public static string EnsureEndsWith ( this string source, string delimiter, StringComparison comparison )
        {
            if (String.IsNullOrEmpty(source))
                return delimiter;

            return source.EndsWith(delimiter, comparison) ? source : source + delimiter;
        }

        /// <summary>Ensures a string ends with a specific delimiter.</summary>
        /// <param name="source">The source.</param>
        /// <param name="delimiter">The delimiter to look for.</param>
        /// <param name="ignoreCase"><see langword="true"/> to ignore case.</param>
        /// <param name="culture">The culture to use.  If <see langword="null"/> then the current culture is used.</param>
        /// <returns>The string with the delimiter added.  If the source is <see langword="null"/> or empty then the delimiter is returned.</returns>               
        public static string EnsureEndsWith ( this string source, string delimiter, bool ignoreCase, CultureInfo culture )
        {
            if (String.IsNullOrEmpty(source))
                return delimiter;

            return source.EndsWith(delimiter, ignoreCase, culture) ? source : source + delimiter;
        }
        #endregion

        #region EnsureStartsWith

        /// <summary>Ensures a string starts with a specific delimiter.</summary>
        /// <param name="source">The source.</param>
        /// <param name="delimiter">The delimiter to look for.</param>
        /// <returns>The string with the delimiter added.  If the source is <see langword="null"/> or empty then the delimiter is returned.</returns>               
        /// <remarks>
        /// The comparison is done using the current culture's case sensitive comparison.
        /// </remarks>
        public static string EnsureStartsWith ( this string source, char delimiter )
        {
            return EnsureStartsWith(source, delimiter.ToString());
        }

        /// <summary>Ensures a string starts with a specific delimiter.</summary>
        /// <param name="source">The source.</param>
        /// <param name="delimiter">The delimiter to look for.</param>
        /// <param name="comparison">The comparison to do.</param>
        /// <returns>The string with the delimiter added.  If the source is <see langword="null"/> or empty then the delimiter is returned.</returns>               
        public static string EnsureStartsWith ( this string source, char delimiter, StringComparison comparison )
        {
            return EnsureStartsWith(source, delimiter.ToString(), comparison);
        }

        /// <summary>Ensures a string starts with a specific delimiter.</summary>
        /// <param name="source">The source.</param>
        /// <param name="delimiter">The delimiter to look for.</param>
        /// <param name="ignoreCase"><see langword="true"/> to ignore case.</param>
        /// <param name="culture">The culture to use.  If <see langword="null"/> then the current culture is used.</param>
        /// <returns>The string with the delimiter added.  If the source is <see langword="null"/> or empty then the delimiter is returned.</returns>               
        public static string EnsureStartsWith ( this string source, char delimiter, bool ignoreCase, CultureInfo culture )
        {
            return EnsureStartsWith(source, delimiter.ToString(), ignoreCase, culture);
        }

        /// <summary>Ensures a string starts with a specific delimiter.</summary>
        /// <param name="source">The source.</param>
        /// <param name="delimiter">The delimiter to look for.</param>
        /// <returns>The string with the delimiter added.  If the source is <see langword="null"/> or empty then the delimiter is returned.</returns>               
        /// <remarks>
        /// The comparison is done using the current culture's case sensitive comparison.
        /// </remarks>
        public static string EnsureStartsWith ( this string source, string delimiter )
        {
            return EnsureStartsWith(source, delimiter, false, null);
        }

        /// <summary>Ensures a string starts with a specific delimiter.</summary>
        /// <param name="source">The source.</param>
        /// <param name="delimiter">The delimiter to look for.</param>
        /// <param name="comparison">The comparison to do.</param>
        /// <returns>The string with the delimiter added.  If the source is <see langword="null"/> or empty then the delimiter is returned.</returns>               
        public static string EnsureStartsWith ( this string source, string delimiter, StringComparison comparison )
        {
            if (String.IsNullOrEmpty(source))
                return delimiter;

            return source.StartsWith(delimiter, comparison) ? source : delimiter + source;
        }

        /// <summary>Ensures a string starts with a specific delimiter.</summary>
        /// <param name="source">The source.</param>
        /// <param name="delimiter">The delimiter to look for.</param>
        /// <param name="ignoreCase"><see langword="true"/> to ignore case.</param>
        /// <param name="culture">The culture to use.  If <see langword="null"/> then the current culture is used.</param>
        /// <returns>The string with the delimiter added.  If the source is <see langword="null"/> or empty then the delimiter is returned.</returns>               
        public static string EnsureStartsWith ( this string source, string delimiter, bool ignoreCase, CultureInfo culture )
        {
            if (String.IsNullOrEmpty(source))
                return delimiter;

            return source.StartsWith(delimiter, ignoreCase, culture) ? source : delimiter + source;
        }
        #endregion        
                
        /// <summary>Gets the leftmost <paramref name="count"/> characters from a string.</summary>
        /// <param name="source">The string to retrieve the substring from.</param>
        /// <param name="count">The number of characters to retrieve.</param>
        /// <returns>The substring.</returns>
        /// <remarks>
        /// If <paramref name="count"/> is greater than the length of the string then the entire string is returned.
        /// </remarks>
        /// <seealso cref="O:LeftOf"/>
        /// <seealso cref="Right"/>
        /// <seealso cref="Mid"/>
        public static string Left ( this string source, int count )
        {
            return (count < source.Length) ? source.Substring(0, count) : source;
        }

        #region LeftOf

        /// <summary>Gets the portion of the string to the left of any of the given tokens.</summary>
        /// <param name="source">The string to search.</param>
        /// <param name="tokens">The tokens to find.</param>
        /// <returns>All characters to the left of any of the tokens.</returns>
        /// <seealso cref="Left"/>
        /// <seealso cref="O:RightOf"/>
        public static string LeftOf ( this string source, params char[] tokens )
        {
            return LeftOf(source, (IList<char>)tokens);
        }

        /// <summary>Gets the portion of the string to the left of any of the given tokens.</summary>
        /// <param name="source">The string to search.</param>
        /// <param name="tokens">The tokens to find.</param>
        /// <returns>All characters to the left of any of the given characters.</returns>
        /// <remarks>
        /// If <paramref name="tokens"/> is <see langword="null"/> or empty then the entire string is returned.
        /// </remarks>
        /// <seealso cref="Left"/>
        /// <seealso cref="O:RightOf"/>
        public static string LeftOf ( this string source, IList<char> tokens )
        {
            if ((tokens == null) || tokens.Count == 0)
                return source;

            //Find the token
            int index = source.IndexOfAny(tokens.ToArray());

            return (index >= 0) ? source.Substring(0, index) : source;
        }

        /// <summary>Gets the portion of the string to the left of the given token.</summary>
        /// <param name="source">The string to search.</param>
        /// <param name="token">The token to find.</param>
        /// <returns>All characters to the left of the given string, if found.</returns>
        /// <remarks>        
        /// If <paramref name="token"/> is <see langword="null"/> or empty then the entire string is returned.  The current culture is used.
        /// </remarks>
        /// <seealso cref="Left"/>
        /// <seealso cref="O:RightOf"/>
        public static string LeftOf ( this string source, string token )
        {
            return LeftOf(source, token, StringComparison.CurrentCulture);
        }

        /// <summary>Gets the portion of the string to the left of the given token.</summary>
        /// <param name="source">The string to search.</param>
        /// <param name="token">The token to find.</param>
        /// <param name="comparison">The type of comparison to do.</param>
        /// <returns>All characters to the left of the given string, if found.</returns>
        /// <remarks>
        /// If <paramref name="token"/> is <see langword="null"/> or empty then the entire string is returned.
        /// </remarks>
        /// <seealso cref="Left"/>
        /// <seealso cref="O:RightOf"/>
        public static string LeftOf ( this string source, string token, StringComparison comparison )
        {
            //Empty source or token is easy
            if ((source.Length == 0) || String.IsNullOrEmpty(token))
                return source;

            //Find the token
            int index = source.IndexOf(token, comparison);

            return (index >= 0) ? source.Substring(0, index) : source;
        }
        #endregion

        #region Mid

        /// <summary>Gets the characters in the range <paramref name="startIndex"/> to <paramref name="endIndex"/> inclusive.</summary>
        /// <param name="source">The string to retrieve the substring from.</param>
        /// <param name="startIndex">The starting index of the string.</param>
        /// <param name="endIndex">The ending index of the string.  The character at the given index is returned.</param>
        /// <returns>The substring.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="startIndex"/> is negative.
        /// <para>-or-</para>
        /// <paramref name="endIndex"/> is less than <paramref name="startIndex"/>.
        /// </exception>
        /// <remarks>
        /// If <paramref name="startIndex"/> is larger than the length of the string then an empty string is returned.  If
        /// <paramref name="endIndex"/> is greater than the length of the string then the remainder of the string is returned.
        /// </remarks>
        /// <seealso cref="Left"/>
        /// <seealso cref="Right"/>
        public static string Mid ( this string source, int startIndex, int endIndex )
        {
            if (startIndex >= source.Length)
                return "";

            if (endIndex >= source.Length)
                endIndex = source.Length - 1;

            return source.Substring(startIndex, endIndex - startIndex + 1);
        }
        #endregion

        #region RemoveAll

        /// <summary>Removes all specified characters from a string.</summary>
        /// <param name="source">The string to modify.</param>
        /// <param name="value">The value to remove.</param>
        /// <returns>The updated string.</returns>
        public static string RemoveAll ( this string source, char value )
        {
            return RemoveAll(source, value.ToString(), 0, StringComparison.CurrentCulture);
        }

        /// <summary>Removes all specified characters from a string.</summary>
        /// <param name="source">The string to modify.</param>
        /// <param name="value">The value to remove.</param>
        /// <param name="startIndex">The index to start removing from.</param>
        /// <returns>The updated string.</returns>
        public static string RemoveAll ( this string source, char value, int startIndex )
        {
            return RemoveAll(source, value.ToString(), startIndex, StringComparison.CurrentCulture);
        }

        /// <summary>Removes all specified characters from a string.</summary>
        /// <param name="source">The string to modify.</param>
        /// <param name="value">The value to remove.</param>
        /// <param name="comparison">The comparer to use.</param>
        /// <returns>The updated string.</returns>
        public static string RemoveAll ( this string source, char value, StringComparison comparison )
        {
            return RemoveAll(source, value.ToString(), 0, comparison);
        }

        /// <summary>Removes all specified characters from a string.</summary>
        /// <param name="source">The string to modify.</param>
        /// <param name="value">The value to remove.</param>
        /// <param name="startIndex">The index to start removing from.</param>
        /// <param name="comparison">The comparer to use.</param>
        /// <returns>The updated string.</returns>
        public static string RemoveAll ( this string source, char value, int startIndex, StringComparison comparison )
        {
            return RemoveAll(source, value.ToString(), startIndex, comparison);
        }
        
        /// <summary>Removes the specified value from a string.</summary>
        /// <param name="source">The string to modify.</param>
        /// <param name="value">The value to remove.</param>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="value"/> is empty.</exception>
        /// <returns>The updated string.</returns>
        public static string RemoveAll ( this string source, string value )
        {
            return RemoveAll(source, value, 0, StringComparison.CurrentCulture);
        }

        /// <summary>Removes the specified value from a string.</summary>
        /// <param name="source">The string to modify.</param>
        /// <param name="value">The value to remove.</param>
        /// <param name="startIndex">The index to start removing from.</param>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="value"/> is empty.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="startIndex"/> is less than zero or larger than the length of the string.</exception>
        /// <returns>The updated string.</returns>
        public static string RemoveAll ( this string source, string value, int startIndex )
        {
            return RemoveAll(source, value, startIndex, StringComparison.CurrentCulture);
        }

        /// <summary>Removes the specified value from a string.</summary>
        /// <param name="source">The string to modify.</param>
        /// <param name="value">The value to remove.</param>
        /// <param name="comparison">The comparer to use.</param>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="value"/> is empty.</exception>
        /// <returns>The updated string.</returns>
        public static string RemoveAll ( this string source, string value, StringComparison comparison )
        {
            return RemoveAll(source, value, 0, comparison);
        }

        /// <summary>Removes the specified value from a string.</summary>
        /// <param name="source">The string to modify.</param>
        /// <param name="value">The value to remove.</param>
        /// <param name="startIndex">The index to start removing from.</param>
        /// <param name="comparison">The comparer to use.</param>
        /// <returns>The updated string.</returns>
        public static string RemoveAll ( this string source, string value, int startIndex, StringComparison comparison )
        {
            return RemoveAll(source, new[] { value }, startIndex, comparison);
        }

        /// <summary>Removes all the specified values from a string.</summary>
        /// <param name="source">The string to modify.</param>
        /// <param name="values">The values to remove.</param>
        /// <param name="startIndex">The index to start removing from.</param>
        /// <param name="comparison">The comparer to use.</param>
        /// <returns>The updated string.</returns>
        public static string RemoveAll ( this string source, IEnumerable<string> values, int startIndex, StringComparison comparison )
        {
            values = values.Where(x => !String.IsNullOrEmpty(x));

            var str = source;
            foreach (var value in values)
            {
                int index = str.IndexOf(value, startIndex, comparison);
                while (index >= 0)
                {
                    str = str.Remove(index, value.Length);

                    index = str.IndexOf(value, index, comparison);
                };
            };

            return str;
        }
        #endregion

        #region ReplaceAll

        /// <summary>Replaces a group of values with another obj.</summary>
        /// <param name="source">The string to modify.</param>
        /// <param name="valuesToReplace">The list of values to replace.</param>
        /// <param name="newValue">The new obj to use.</param>
        /// <returns>The updated string.</returns>
        /// <remarks>
        /// Each element in <paramref name="valuesToReplace"/> is replaced by <paramref name="newValue"/> in the returned string.  The values are replaced in the
        /// order in which they appear.
        /// </remarks>
        public static string ReplaceAll ( this string source, char[] valuesToReplace, char newValue )
        {
            if (source.Length == 0 || valuesToReplace.Length == 0)
                return source;

            StringBuilder bldr = new StringBuilder();
            foreach (var ch in source)
            {
                if (valuesToReplace.Contains(ch))
                    bldr.Append(newValue);
                else
                    bldr.Append(ch);
            };

            return bldr.ToString();
        }

        /// <summary>Replaces a group of values with another obj.</summary>
        /// <param name="source">The string to modify.</param>
        /// <param name="valuesToReplace">The list of values to replace.</param>
        /// <param name="newValue">The new obj to use.</param>
        /// <returns>The updated string.</returns>
        /// <remarks>
        /// Each element in <paramref name="valuesToReplace"/> is replaced by <paramref name="newValue"/> in the returned string.  The values are replaced in the
        /// order in which they appear.
        /// </remarks>
        public static string ReplaceAll ( this string source, IEnumerable<string> valuesToReplace, string newValue )
        {
            if (source.Length == 0 || valuesToReplace.Count() == 0)
                return source;

            newValue = newValue ?? "";

            StringBuilder bldr = new StringBuilder(source);
            foreach (var value in valuesToReplace)
            {
                if (!String.IsNullOrEmpty(value))
                    bldr.Replace(value, newValue);
            };

            return bldr.ToString();
        }

        /// <summary>Replaces a group of values with another obj.</summary>
        /// <param name="source">The string to modify.</param>
        /// <param name="valuesToReplace">The list of values to replace.</param>
        /// <param name="replacementValues">The list of replacement values.</param>
        /// <returns>The updated string.</returns>
        /// <remarks>
        /// <paramref name="replacementValues"/> must be at least as large as <paramref name="valuesToReplace"/>.  Each element in <paramref name="valuesToReplace"/>
        /// is replaced in the string with the corresponding obj in <paramref name="replacementValues"/>.  The values are replaced in the order they appear in <paramref name="valuesToReplace"/>.
        /// </remarks>
        public static string ReplaceAll ( this string source, IEnumerable<string> valuesToReplace, IEnumerable<string> replacementValues )
        {
            if (replacementValues.Count() < valuesToReplace.Count())
                throw new ArgumentException("Replacement values is smaller than values to replace.", "replacementValues");

            if (source.Length == 0)
                return "";

            StringBuilder bldr = new StringBuilder(source);

            int index = 0;
            foreach (var oldValue in valuesToReplace)
            {
                if (!String.IsNullOrEmpty(oldValue))
                    bldr.Replace(oldValue, replacementValues.ElementAt(index) ?? "");
                ++index;
            };

            return bldr.ToString();
        }
        #endregion

        #region Right

        /// <summary>Gets the rightmost <paramref name="count"/> characters from a string.</summary>
        /// <param name="source">The string to retrieve the substring from.</param>
        /// <param name="count">The number of characters to retrieve.</param>
        /// <returns>The substring.</returns>
        /// <remarks>
        /// If <paramref name="count"/> is greater than the length of the string then the entire string is returned.  If <paramref name="count"/> is 
        /// zero then none of the string is returned.
        /// </remarks>
        /// <seealso cref="Left"/>
        /// <seealso cref="Mid"/>
        /// <seealso cref="O:RightOf"/>
        public static string Right ( this string source, int count )
        {
            return (count < source.Length) ? source.Substring(source.Length - count) : source;
        }
        #endregion

        #region RightOf

        /// <summary>Gets the portion of the string to the right of any of the given tokens.</summary>
        /// <param name="source">The string to search.</param>
        /// <param name="tokens">The tokens to find.</param>
        /// <returns>All characters to the right of any of the given tokens or an empty string if the token is not found.</returns>
        /// <seealso cref="O:LeftOf"/>
        /// <seealso cref="Right"/>
        public static string RightOf ( this string source, params char[] tokens )
        {
            return RightOf(source, (IList<char>)tokens);
        }

        /// <summary>Gets the portion of the string to the right of any of the given tokens.</summary>
        /// <param name="source">The string to search.</param>
        /// <param name="tokens">The tokens to find.</param>
        /// <returns>All characters to the right of the first found token or an empty string if the tokens are not found.</returns>
        /// <remarks>
        /// If <paramref name="tokens"/> is <see langword="null"/> or empty then an empty string is returned.  
        /// </remarks>
        /// <seealso cref="O:LeftOf"/>
        /// <seealso cref="Right"/>
        public static string RightOf ( this string source, IList<char> tokens )
        {
            if ((source.Length == 0) || (tokens == null) || (tokens.Count == 0))
                return source;

            //Find it
            int index = source.IndexOfAny(tokens.ToArray());

            return (index >= 0) && (index < source.Length - 1) ? source.Substring(index + 1) : "";
        }

        /// <summary>Gets the portion of the string to the right of the given token.</summary>
        /// <param name="source">The string to search.</param>
        /// <param name="token">The token to find.</param>
        /// <returns>All characters to the right of the given token or an empty string if the token is not found.</returns>
        /// <remarks>
        /// If <paramref name="token"/> is <see langword="null"/> or empty then an empty string is returned.  The current culture is used.
        /// </remarks>
        /// <seealso cref="O:LeftOf"/>
        public static string RightOf ( this string source, string token )
        {
            return RightOf(source, token, StringComparison.CurrentCulture);
        }

        /// <summary>Gets the portion of the string to the right of the given token.</summary>
        /// <param name="source">The string to search.</param>
        /// <param name="token">The token to find.</param>
        /// <param name="comparisonType">The type of comparison to do.</param>
        /// <returns>All characters to the right of the given token or an empty string if the token is not found.</returns>
        /// <remarks>
        /// If <paramref name="token"/> is <see langword="null"/> or empty then an empty string is returned.
        /// </remarks>
        /// <seealso cref="O:LeftOf"/>
        public static string RightOf ( this string source, string token, StringComparison comparisonType )
        {
            if ((source.Length == 0) || String.IsNullOrEmpty(token))
                return source;

            //Find it
            int index = source.IndexOf(token, comparisonType);

            int start = index + token.Length;
            return (index >= 0) && (start < source.Length - 1) ? source.Substring(start) : "";
        }
        #endregion
        
        #region Private Members

        private static string InternalCombine ( string separator, IEnumerable<string> values )
        {
            //Special case an empty separator
            if (separator.Length == 0)
                return String.Join("", values);

            StringBuilder bldr = new StringBuilder();
            bool endsWithSeparator = true;   //Default to true so we won't add one initially

            foreach (var value in values)
            {
                if (!String.IsNullOrEmpty(value))
                {
                    bool valueHasDelimiter = value.StartsWith(separator);

                    //Add separator as needed
                    if (!endsWithSeparator && !valueHasDelimiter)
                        bldr.Append(separator);
                    else if (endsWithSeparator && valueHasDelimiter && bldr.Length > 0)
                        bldr.Remove(bldr.Length - 1, 1);

                    //Add the next obj
                    bldr.Append(value);

                    endsWithSeparator = value.EndsWith(separator);
                };
            };

            return bldr.ToString();
        }        
        #endregion 
    }

    /// <summary>Defines options for string coalesce.</summary>
    /// <seealso cref="O:StringExtensions.Coalesce"/>
    [Flags]
    public enum StringCoalesceOptions
    {
        /// <summary>No options.</summary>
        None = 0,

        /// <summary>Skip empty strings as well.</summary>
        SkipEmpty = 1,
    }

}
