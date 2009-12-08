#region ***** Style.cs *****
/**********************************************************************************************
 * Project: Our Word!
 * File:    Style.cs
 * Author:  John Wimbish
 * Created: 8 Dec 2009
 * Purpose: Common functionality for CharacterStyle and ParagraphStyle
 * Legal:   Copyright (c) 2005-09, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
using System.Diagnostics;
using System.Xml;
using JWTools;
#endregion


namespace OurWordData.Styles
{
    public class Style
    {
        // Content Attrs ---------------------------------------------------------------------
        #region Attr{g/s}: string StyleName - serves as the style's unique ID
        public string StyleName
        {
            get
            {
                return m_sStyleName;
            }
        }
        private readonly string m_sStyleName;
        #endregion

        // Scaffolding -----------------------------------------------------------------------
        #region Constructor(sStyleName)
        protected Style(string sStyleName)
        {
            m_sStyleName = sStyleName;
        }
        #endregion

        // I/O & Merge -----------------------------------------------------------------------
        private const string c_sAttrStyleName = "Name";
        #region VirtMethod: XmlNode Save(XmlDoc doc, XmlNode nodeParent)
        public virtual XmlNode Save(XmlDoc doc, XmlNode nodeParent)
        {
            Debug.Assert(false, "Subclass must override");
            return null;
        }
        #endregion
        #region Method: void SaveStyleAttrs(XmlDoc, nodeStyle)
        protected void SaveStyleAttrs(XmlDoc doc, XmlNode nodeStyle)
        {
            doc.AddAttr(nodeStyle, c_sAttrStyleName, StyleName);
        }
        #endregion
        #region SMethod: string GetStyleNameFromXml(XmlNode node)
        static protected string GetStyleNameFromXml(XmlNode node)
        {
           return XmlDoc.GetAttrValue(node, c_sAttrStyleName, "");
        }
        #endregion
        #region SMethod: bool CanMerge(nodeOurs, nodeTheirs, nodeParent)
        static protected bool CanMerge(XmlNode ours, XmlNode theirs, XmlNode parent)
            // The StyleName (unique id in the list of styles) for the three must be identical
            // if a merge is to take place
        {
            Debug.Assert(ours != null);
            Debug.Assert(theirs != null);
            Debug.Assert(parent != null);

            var sStyleName = GetStyleNameFromXml(ours);
            return (sStyleName == GetStyleNameFromXml(theirs) &&
                    sStyleName == GetStyleNameFromXml(parent));
        }
        #endregion
    }
}
