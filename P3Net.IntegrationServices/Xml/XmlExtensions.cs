/*
 * Copyright © 2016 Federation of State Medical Boards
 * All Rights Reserved
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;

namespace P3Net.IntegrationServices.Xml
{
    /// <summary>Provides extension methods for working with XML.</summary>
    public static class XmlExtensions
    {
        /// <summary>Creates and adds a child element to the given document.</summary>
        /// <param name="source">The source.</param>
        /// <param name="name">The element name.</param>
        /// <param name="text">The inner text.</param>
        /// <returns>The new element.</returns>
        /// <remarks>
        /// <paramref name="text" /> is encoded before being assigned to the element.
        /// </remarks>
        public static XmlElement CreateAndAddElement ( this XmlDocument source, string name, string text = null )
        {
            var element = source.CreateElement(name);
            source.AppendChild(element);

            if (!String.IsNullOrWhiteSpace(text))
                element.InnerText = HttpUtility.HtmlEncode(text);

            return element;
        }

        /// <summary>Creates and adds a child element to the given parent element.</summary>
        /// <param name="source">The source.</param>
        /// <param name="name">The element name.</param>
        /// <param name="text">The inner text.</param>
        /// <returns>The new element.</returns>
        /// <remarks>
        /// <paramref name="text" /> is encoded before being assigned to the element.
        /// </remarks>
        public static XmlElement CreateAndAddElement ( this XmlElement source, string name, string text = null )
        {
            var element = source.OwnerDocument.CreateElement(name);
            source.AppendChild(element);

            if (!String.IsNullOrWhiteSpace(text))
                element.InnerText = HttpUtility.HtmlEncode(text);

            return element;
        }

        /// <summary>Gets the value of an attribute.</summary>
        /// <param name="source">The source element.</param>
        /// <param name="name">The attribute name.</param>
        /// <returns>The attribute value (decoded) or an empty string otherwise.</returns>
        public static string GetAttributeValue ( this XmlElement source, string name )
        {
            var str = source.GetAttribute(name);

            return (str != null) ? HttpUtility.HtmlDecode(str) : "";
        }

        /// <summary>Gets the inner text of a child element.</summary>
        /// <param name="source">The parent element.</param>
        /// <param name="name">The child element name.</param>
        /// <returns>The inner text (decoded) of the child element or an empty string otherwise.</returns>
        public static string GetChildElementText ( this XmlElement source, string name )
        {
            var element = (from e in source.ChildNodes.OfType<XmlNode>()
                           where e.Name == name
                           select e).FirstOrDefault();
            if (element != null)
                return HttpUtility.HtmlDecode(element.InnerText);

            return "";
        }

        /// <summary>Gets the inner text of an element.</summary>
        /// <param name="source">The element.</param>
        /// <returns>The inner text (decoded) of the element or an empty string otherwise.</returns>
        public static string GetElementText ( this XmlElement source )
        {
            if (!String.IsNullOrEmpty(source.InnerText))
                return HttpUtility.HtmlDecode(source.InnerText);

            return "";
        }

        /// <summary>Sets the value of an attribute.</summary>
        /// <param name="source">The parent element.</param>
        /// <param name="name">The attribute name.</param>
        /// <param name="value">The value.</param>
        /// <remarks>
        /// The value is encoded before being assigned to the attribute.
        /// </remarks>
        public static void SetAttributeValue ( this XmlElement source, string name, object value )
        {
            source.SetAttribute(name, HttpUtility.HtmlEncode(value));
        }

        /// <summary>Sets the inner text of an element.</summary>
        /// <param name="source">The element.</param>
        /// <param name="text">The text.</param>
        /// <remarks>
        /// The text is encoded before being assigned to the element.
        /// </remarks>
        public static void SetElementText ( this XmlElement source, object text )
        {
            if (text != null)
                source.InnerText = HttpUtility.HtmlEncode(text);
        }
    }
}
