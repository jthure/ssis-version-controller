#region License and Copyright
/*
 * Dotnet Commons Xml
 *
 *
 * This library is free software; you can redistribute it and/or modify it 
 * under the terms of the GNU Lesser General Public License as published by 
 * the Free Software Foundation; either version 2.1 of the License, or 
 * (at your option) any later version.
 *
 * This library is distributed in the hope that it will be useful, but 
 * WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY 
 * or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License 
 * for more details. 
 *
 * You should have received a copy of the GNU Lesser General Public License 
 * along with this library; if not, write to the 
 * Free Software Foundation, Inc., 
 * 59 Temple Place, 
 * Suite 330, 
 * Boston, 
 * MA 02111-1307 
 * USA 
 * 
 */
#endregion

using System;
using System.Collections;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Xsl;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Linq;

//using Dotnet.Commons.Reflection;

namespace utils.Xml
{

    ///  
    /// <summary>
    /// This utility class contains wrapper functions that help to ease the handling and 
    /// manipulation of Xml documents, such as adding an element, adding an attribute
    /// to an element, copying and cloning of nodes, etc.
    ///
    /// </summary>
    /// 

    public static class XmlUtils
    {
        // #################################################################### //
        // These code is derived from Mainsoft.com                              //
        // http://www.koders.com/csharp/fid439BB5BEF93D1AEFAF0B9206236AB0ECE49BC229.aspx
        // #################################################################### //


        /// -----------------------------------------------------------
        /// <summary>
        /// Alphabetical sorting of the XmlNodes 
        /// and their  attributes in the <see cref="System.Xml.XmlDocument"/>.
        /// </summary>
        /// <param name="document"><see cref="System.Xml.XmlDocument"/> to be sorted</param>
        /// -----------------------------------------------------------
        public static void SortXml(XmlDocument document, bool sortAttributes = false, IEnumerable<string> elementsToSort = null)
        {
            SortXml(document.DocumentElement, sortAttributes, elementsToSort);
        }


        /// -----------------------------------------------------------
        /// <summary>
        /// Inplace pre-order recursive alphabetical sorting of the XmlNodes child 
        /// elements and <see cref="System.Xml.XmlAttributeCollection" />.
        /// </summary>
        /// <param name="rootNode">The root to be sorted.</param>
        /// <param name="sortAttributes"></param>
        /// <param name="elementsToSort"></param>
        /// -----------------------------------------------------------
        public static void SortXml(XmlNode rootNode, bool sortAttributes = false, IEnumerable<string> elementsToSort = null)
        {
            if(sortAttributes) SortAttributes(rootNode.Attributes);
            if(elementsToSort!= null && elementsToSort.Contains(rootNode.Name)) SortElements(rootNode);
            foreach (XmlNode childNode in rootNode.ChildNodes)
            {
                SortXml(childNode, false, elementsToSort);
            }
        }

        /// -----------------------------------------------------------
        /// <summary>
        /// Sorts an attributes collection alphabetically.
        /// It uses the bubble sort algorithm.
        /// </summary>
        /// <param name="attribCol">The attribute collection to be sorted.</param>
        /// -----------------------------------------------------------
        public static void SortAttributes(XmlAttributeCollection attribCol)
        {
            if (attribCol == null)
                return;

            bool hasChanged = true;
            while (hasChanged)
            {
                hasChanged = false;
                for (int i = 1; i < attribCol.Count; i++)
                {
                    if (String.Compare(attribCol[i].Name, attribCol[i - 1].Name, true) < 0)
                    {
                        //Replace
                        attribCol.InsertBefore(attribCol[i], attribCol[i - 1]);
                        hasChanged = true;
                    }
                }
            }

        }

        /// -----------------------------------------------------------
        /// <summary>
        /// Sorts a <see cref="XmlNodeList" /> alphabetically, by the names of the elements.
        /// It uses the bubble sort algorithm.
        /// </summary>
        /// <param name="node">The node in which its childNodes are to be sorted.</param>
        /// -----------------------------------------------------------
        public static void SortElements(XmlNode node)
        {
            bool changed = true;
            while (changed)
            {
                changed = false;
                for (int i = 1; i < node.ChildNodes.Count; i++)
                {

                    if (node.ChildNodes[i].Compare(node.ChildNodes[i - 1]) < 0)
                    {
                        //Replace:
                        node.InsertBefore(node.ChildNodes[i], node.ChildNodes[i - 1]);
                        changed = true;
                    }
                }
            }
        }
        public static int Compare(this XmlNode first, XmlNode second) =>
            string.CompareOrdinal(first.GetComparingString(), second.GetComparingString());


        private static IEnumerable<string> ComparingAttributes = new string[] {
            "DTS:refId",
            "refId",
            "DTS:ObjectName",
            "name"
        };

        private static string GetComparingString(this XmlNode n)
        {
            var attrs = n.Attributes.ToGenericEnumerable();
            var attrComparingString = ComparingAttributes.Select(s => attrs.FirstOrDefault(x => x.Name == s)?.Value).Where(x => x != null);
            return new string[] { n.Name }.Concat(attrComparingString).ConcatString();
        }

        //private static bool HasDTSObjectNameAttribute(this IEnumerable<XmlAttribute> attributes) => attributes.HasAttribute("DTS:ObjectName");
        //private static bool HasAttribute(this IEnumerable<XmlAttribute> attributes, string attribute) => attributes.Any(e => e.Name == attribute);
        //private static XmlAttribute GetAttribute(this IEnumerable<XmlAttribute> attributes, string attribute) => attributes.FirstOrDefault(e => e.Name == attribute);
        private static string ConcatString(this IEnumerable<string> strings, string separator = "") => string.Join(separator, strings);
        public static IEnumerable<XmlAttribute> ToGenericEnumerable(this XmlAttributeCollection attributes)
        {
            var result = new List<XmlAttribute>();
            foreach (XmlAttribute attribute in attributes)
            {
                result.Add(attribute);
            }
            return result;

        }
    }

}



