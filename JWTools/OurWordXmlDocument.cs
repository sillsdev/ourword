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
    public class OurWordXmlDocumentException : Exception
    {
        #region Constructor(sErrorMessage)
        public OurWordXmlDocumentException(string sMessage)
            : base(sMessage)
        {
        }
        #endregion
    }

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
            //Console.WriteLine(sXml);
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

        static public string GetAttrValue(XmlNode node, string sAttrName, string sDefaultValue)
        {
            foreach (XmlAttribute attr in node.Attributes)
            {
                if (attr.Name == sAttrName)
                    return attr.Value;
            }

            if (null == sDefaultValue)
                return "";
            return sDefaultValue;
        }
        static public int GetAttrValue(XmlNode node, string sAttrName, int nDefaultValue)
        {
            string sValue = GetAttrValue(node, sAttrName, nDefaultValue.ToString());

            try
            {
                return Convert.ToInt16(sValue);
            }
            catch (Exception e)
            {
                throw new OurWordXmlDocumentException("Bad integer data in oxes file: " + e.Message);
            }
        }


        // For Unit Testing ------------------------------------------------------------------
        #region Method: bool IsSame(OurWordXmlDocument other)
        public bool IsSame(OurWordXmlDocument other)
        {
            var sbThis = new StringBuilder();
            this.Save(new StringWriter(sbThis));
            string sThis = sbThis.ToString();

            var sbOther = new StringBuilder();
            other.Save(new StringWriter(sbOther));
            string sOther = sbOther.ToString();

           return (sThis == sOther);
        }
        #endregion
        #region SMethod: void DisplayDifferences(xActual, xExpected)
        static public void DisplayDifferences(OurWordXmlDocument xActual, OurWordXmlDocument xExpected)
        {
            var sbActual = new StringBuilder();
            xActual.Save(new StringWriter(sbActual));
            string sActual = sbActual.ToString();

            var sbExpected = new StringBuilder();
            xExpected.Save(new StringWriter(sbExpected));
            string sExpected = sbExpected.ToString();

            bool bIsSame = (sActual == sExpected);

            if (bIsSame)
                return;

            int iMax = Math.Min(sActual.Length, sExpected.Length);
            for (int i = 0; i < iMax; i++)
            {
                if (sActual[i] != sExpected[i])
                {
                    Console.WriteLine("The strings first diffet at position " + i.ToString());
                    int iStart = Math.Max(i - 20, 0);
                    int len = Math.Min(iMax - i, 60);

                    string sPad = "          ";
                    for (int k = iStart; k < i; k++)
                        sPad += ' ';
                    Console.WriteLine(sPad + "v");

                    Console.WriteLine("Actual   =>" + sActual.Substring(iStart, len));
                    Console.WriteLine("Expected =>" + sExpected.Substring(iStart, len));
                    break;
                }
            }
        }
        #endregion
        #region Method: void WriteToConsole(string sMessage)
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
        #endregion
    }
}
