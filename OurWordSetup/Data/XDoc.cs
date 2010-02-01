#region ***** XDoc.cs *****
/**********************************************************************************************
 * Project: Our Word!
 * File:    XDoc.cs
 * Author:  John Wimbish
 * Created: 28 Jan 2010
 * Purpose: Shorthand for XmlDocument operations
 * Legal:   Copyright (c) 2003-10, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;
#endregion

namespace OurWordSetup.Data
{
    public class XDoc : XmlDocument
    {
        // Scaffolding -----------------------------------------------------------------------
        private readonly string m_sPath;
        #region Constructor(sPath)
        public XDoc(string sPath)
        {
            Debug.Assert(!string.IsNullOrEmpty(sPath));
            m_sPath = sPath;
        }
        #endregion

        // Adding Hierarchy ------------------------------------------------------------------
        #region Method: void AddXmlDeclaration()
        private bool m_bHasXmlDeclaration;
        private void AddXmlDeclaration()
            // Creates "<?xml version="1.0" encoding="UTF-8"?>"  
        {
            var decl = CreateXmlDeclaration("1.0", "UTF-8", null);
            var root = DocumentElement;
            InsertBefore(decl, root);
            m_bHasXmlDeclaration = true;
        }
        #endregion
        #region Method: XmlNode AddNode(XmlNode nodeParent, string sName)
        public XmlNode AddNode(XmlNode nodeParent, string sName)
        {
            var node = CreateNode(XmlNodeType.Element, sName, null);

            if (nodeParent == null)
                nodeParent = this;

            nodeParent.AppendChild(node);
            return node;
        }
        #endregion
        
        // Adding Attributes -----------------------------------------------------------------
        #region Method: void AddAttr(node, sAttrName, sValue)
        public void AddAttr(XmlNode node, string sAttrName, string sValue)
            // Empty strings are not output to the file; thus a missing value
            // is understood to be an empty string.
        {
            if (string.IsNullOrEmpty(sValue))
                return;

            var attr = CreateAttribute(sAttrName);
            attr.Value = sValue;
            node.Attributes.Append(attr);
        }
        #endregion
        #region Method: void AddAttr(node, sAttrName, nValue)
        public void AddAttr(XmlNode node, string sAttrName, int nValue)
        {
            if (0 != nValue)
                AddAttr(node, sAttrName, nValue.ToString());
        }
        #endregion
        #region Method: void AddAttr(node, sAttrName, lalue)
        public void AddAttr(XmlNode node, string sAttrName, long lValue)
        {
            if (0 != lValue)
                AddAttr(node, sAttrName, lValue.ToString());
        }
        #endregion
        #region Method: void AddAttr(node, sAttrName, dtValue)
        public void AddAttr(XmlNode node, string sAttrName, DateTime dtValue)
        {
            var sDate = dtValue.ToString("u", DateTimeFormatInfo.InvariantInfo);
            AddAttr(node, sAttrName, sDate);
        }
        #endregion
        #region Method: void AddAttr(node, sAttrName, bValue)
        public void AddAttr(XmlNode node, string sAttrName, bool bValue)
        // False values are not output to the file; thus a missing value
        // is understood to be false.
        {
            if (bValue)
                AddAttr(node, sAttrName, "true");
        }
        #endregion
        #region Method: void AddAttr(node, sAttrName, fValue)
        public void AddAttr(XmlNode node, string sAttrName, float fValue)
        {
            AddAttr(node, sAttrName, fValue.ToString());
        }
        #endregion
        #region Method: void AddAttr(node, sAttrName, dValue)
        public void AddAttr(XmlNode node, string sAttrName, double dValue)
        {
            AddAttr(node, sAttrName, dValue.ToString());
        }
        #endregion

        // Reading Hierarchy -----------------------------------------------------------------
        #region SMethod: XmlNode FindNode(XmlNode nodeParent, string sChildName)
        static public XmlNode FindNode(XmlNode nodeParent, string sChildName)
        {
            foreach (XmlNode node in nodeParent.ChildNodes)
            {
                if (IsNode(node, sChildName))
                    return node;
            }

            return null;
        }
        #endregion
        #region SMethod: bool IsNode(XmlNode node, string sName)
        static public bool IsNode(XmlNode node, string sName)
        {
            return (node.Name.ToUpper() == sName.ToUpper());
        }
        #endregion

        // Reading Attributes ----------------------------------------------------------------
        #region SMethod: string GetAttrValue(node, attr, sDefaultValue)
        static public string GetAttrValue(XmlNode node, string sAttrName, string sDefaultValue)
        {
            if (null == node || string.IsNullOrEmpty(sAttrName))
                return sDefaultValue;

            foreach (XmlAttribute attr in node.Attributes)
            {
                if (attr.Name.ToUpper() == sAttrName.ToUpper())
                    return attr.Value;
            }

            return (string.IsNullOrEmpty(sDefaultValue)) ? "" : sDefaultValue;
        }
        #endregion
        #region SMethod: int GetAttrValue(node, attr, nDefaultValue)
        static public int GetAttrValue(XmlNode node, string sAttrName, int nDefaultValue)
        {
            var sValue = GetAttrValue(node, sAttrName, nDefaultValue.ToString());

            try { return Convert.ToInt16(sValue); } catch {}

            return nDefaultValue;
        }
        #endregion
        #region SMethod: long GetAttrValue(node, attr, nDefaultValue)
        static public long GetAttrValue(XmlNode node, string sAttrName, long lDefaultValue)
        {
            var sValue = GetAttrValue(node, sAttrName, lDefaultValue.ToString());

            try { return Convert.ToInt64(sValue); }
            catch { }

            return lDefaultValue;
        }
        #endregion
        #region SMethod: DateTime GetAttrValue(node, attr, dtDefaultValue)
        static public DateTime GetAttrValue(XmlNode node, string sAttrName, DateTime dtDefaultValue)
        {
            var sDefault = "";
            sDefault = dtDefaultValue.ToString("u", DateTimeFormatInfo.InvariantInfo);

            var sValue = GetAttrValue(node, sAttrName, sDefault);
            if (string.IsNullOrEmpty(sValue))
                return DateTime.Now;

            return DateTime.ParseExact(sValue, "u",
                DateTimeFormatInfo.InvariantInfo);
        }
        #endregion

        // I/O -------------------------------------------------------------------------------
        #region Method: void Write()
        public void Write()
        {
            if (!m_bHasXmlDeclaration)
                AddXmlDeclaration();

            // Create the directory if it doesn't exist
            var sFolder = Path.GetDirectoryName(m_sPath);
            if (!Directory.Exists(sFolder))
                Directory.CreateDirectory(sFolder);

            // Write out the file
            Save(m_sPath);
        }
        #endregion


        #region Method: string OneLiner()
        public string OneLiner()
        {
            var sb = new StringBuilder();
            this.Save(new StringWriter(sb));
            return sb.ToString();
        }
        #endregion
        #region SMethod: string OneLiner(XmlNode node)
        static public string OneLiner(XmlNode node)
        {
            return node.OuterXml;
        }
        #endregion
    }

}