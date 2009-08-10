/**********************************************************************************************
 * Project: Our Word!
 * File:    OurWordXmlDocument.cs
 * Author:  John Wimbish
 * Created: 6 Aug 2009
 * Purpose: Useful additions to XmlDocument
 * Legal:   Copyright (c) 2005-09, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
#region Using
using System;
using System.Diagnostics;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Windows.Forms;
using System.Text;
using System.Threading;
using System.IO;
using System.Xml;
#endregion


namespace JWTools
{
    public class OurWordXmlDocument : XmlDocument
    {
        // Constructors ----------------------------------------------------------------------
        #region Constructor()
        public OurWordXmlDocument()
            : base()
        {
        }
        #endregion
        #region Constructor(string[] vsInitializeFrom)
        public OurWordXmlDocument(string[] vsInitializeFrom)
            : this()
        {
            string sXml = "";
            foreach (string s in vsInitializeFrom)
                sXml += s;
            LoadXml(sXml);
        }
        #endregion


        public string IntToID(int n)
        {
            if (n < 0)
                return "err";
            if (n == 0)
                return "A";

            string sID = "";

            while (n != 0)
            {
                int nRemainder = n % 26;
                n /= 26;

                sID = ((char)('A' + nRemainder)).ToString() + sID;
            }

            return sID;
        }


        public void AddXmlDeclaration()
            // Creates "<?xml version="1.0" encoding="utf-16"?>"  
        {
            var decl = CreateXmlDeclaration("1.0", null, null);
            var root = DocumentElement;
            InsertBefore(decl, root);
        }

        public XmlNode AddNode(XmlNode nodeParent, string sName)
        {
            var node = CreateNode(XmlNodeType.Element, sName, null);

            if (nodeParent == null)
                nodeParent = this;

            nodeParent.AppendChild(node);
            return node;
        }

        public XmlAttribute AddAttr(XmlNode node, string sName, string sValue)
        {
            var attr = CreateAttribute(sName);
            attr.Value = sValue;
            node.Attributes.Append(attr);
            return attr;
        }

        public XmlText AddText(XmlNode node, string sValue)
        {
            XmlText text = CreateTextNode(sValue);
            node.AppendChild(text);
            return text;
        }


        // For Unit Testing ------------------------------------------------------------------
        public bool IsSame(OurWordXmlDocument other)
        {
            return (this.InnerText == other.InnerText);
        }

        public void WriteToConsole(string sMessage)
            // Breaks it down into meaningful lines as it will appear in the file,
            // as opposed to InnerText, which has no line breaks.
        {
            Console.WriteLine("----- " + sMessage + " -----");

            var sb = new StringBuilder();
            Save(new StringWriter(sb));
            Console.WriteLine(sb.ToString());

            Console.WriteLine("");
        }
    }
}
