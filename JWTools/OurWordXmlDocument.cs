﻿#region ***** XmlDoc.cs *****
/**********************************************************************************************
 * Project: Our Word!
 * File:    XmlDoc.cs
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
using System.Globalization;
using System.Windows.Forms;
using System.Text;
using System.Threading;
using System.IO;
using System.Xml;
#endregion
#endregion

namespace JWTools
{
    public class XmlDocException : Exception
    {
        #region Constructor(sErrorMessage)
        public XmlDocException(string sMessage)
            : base(sMessage)
        {
        }
        #endregion
    }

    public class XmlDoc : XmlDocument
    {
        // Constructors ----------------------------------------------------------------------
        #region Constructor()
        public XmlDoc()
            : base()
        {
        }
        #endregion
        #region Constructor(string[] vsInitializeFrom)
        public XmlDoc(string[] vsInitializeFrom)
            : this()
        {
            string sXml = "";
            foreach (string s in vsInitializeFrom)
                sXml += s;
            //Console.WriteLine(sXml);
            LoadXml(sXml);
        }
        #endregion

        #region Constructor(string sInitializeFrom)
        public XmlDoc(string sInitializeFrom)
            : this()
        {
            LoadXml(sInitializeFrom);
        }
        #endregion

        // IDs -------------------------------------------------------------------------------
        #region ID's Documentation
        /* Doc - Xml requires an id to begin with a letter. So I'm using all of the letters
         * of the alphabet, both upper and lower case, but without vowels so as to not risk
         * any swear words appearing. So 26 letters minus 5 vowels times two cases results in
         * 42 letters; or thus Base 42.
         * 
         * In base 41, two digits is sufficient for 1764 IDs, and three digits for 74000. 
         * Thus we have a fairly compact method of doing IDs. Further, because these are
         * not mnemonic in any way, I am hopefully that people who bring oxes into an editor
         * will not be tempted to mess with them.
         * 
         * Thinking in base 42 is a bit tricky. "B" is zero, "C" is one. So the next thing
         * after "z" is not "BB", but rather, "CB". In base ten, it is equivalent to saying
         * the next thing after '9' is not '00', but rather '10'. 
         * 
         * Although the default is base 42, I've made it changeable via thte Digits attribute.
         * In the test code, I do a test with base 10, where the digits are "0123456789", to
         * verify that the algorithm is multi-base capable.
        */
        #endregion
        #region SAttr{g/s}: string Digits
        static public string Digits
        {
            get
            {
                return m_sDigits;
            }
            set
            {
                m_sDigits = value;
            }
        }
        static string m_sDigits = "BCDFGHJKLMNPQRSTVWXYZbcdfghjklmnpqrstvwxyz";
        #endregion
        #region SMethod: string IntToID(int n)
        static public string IntToID(int n)
        {
            if (n < 0)
                return "err";
            if (n == 0)
                return Digits[0].ToString();

            // E.g., not base 10, but in this case, base 42.
            int cBase = Digits.Length;
            //Console.WriteLine("Base=" + cBase.ToString());

            // We'll build the string here
            string sID = "";

            // Loop through the number, dividing by the Base, figuring each digit via the remainder
            while (n != 0)
            {
                int nRemainder = n % cBase;
                n /= cBase;

                sID = Digits[nRemainder].ToString() + sID;
            }

            return sID;
        }
        #endregion
        #region SMethod: int IdToInt(string sID)
        static public int IdToInt(string sID)
        {
            // E.g., not base 10, but in this case, base 42.
            int cBase = Digits.Length;

            int n = 0;
            for (int i = 0; i < sID.Length; i++)
            {
                // Convert the letter to a digit
                int nDigit = Digits.IndexOf(sID[i]);
                if (-1 == nDigit)
                    return -1;

                // Get the position (e.g., in base 10, thousands, hundreds, tens, one, etc.)
                // The 'ones' position should equal zero.
                int nPosition = (sID.Length - 1) - i;
                int nMultiplier = 1;
                while (nPosition > 0)
                {
                    nMultiplier *= cBase;
                    nPosition--;
                }

                n += (nDigit * nMultiplier);
            }

            return n;
        }
        #endregion

        // Attrs -----------------------------------------------------------------------------
        #region Method: XmlAttribute AddAttr(node, sAttrName, string sValue)
        public XmlAttribute AddAttr(XmlNode node, string sAttrName, string sValue)
        {
            var attr = CreateAttribute(sAttrName);
            attr.Value = sValue;
            node.Attributes.Append(attr);
            return attr;
        }
        #endregion
        #region Method: XmlAttribute AddAttr(node, sAttrName, int nValue)
        public XmlAttribute AddAttr(XmlNode node, string sAttrName, int nValue)
        {
            return AddAttr(node, sAttrName, nValue.ToString());
        }
        #endregion
        #region Method: XmlAttribute AddAttr(node, sAttrName, DateTime dtValue)
        public XmlAttribute AddAttr(XmlNode node, string sAttrName, DateTime dtValue)
        {
            string sDate = dtValue.ToString("u", DateTimeFormatInfo.InvariantInfo);
            return AddAttr(node, sAttrName, sDate);
        }
        #endregion
        #region Method: XmlAttribute AddAttr(node, sAttrName, bool bValue)
        public XmlAttribute AddAttr(XmlNode node, string sAttrName, bool bValue)
        {
            string sValue = (bValue) ? "true" : "false";
            return AddAttr(node, sAttrName, sValue);
        }
        #endregion

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

            if (null == sDefaultValue)
                return "";
            return sDefaultValue;
        }
        #endregion
        #region SMethod: int GetAttrValue(node, attr, nDefaultValue)
        static public int GetAttrValue(XmlNode node, string sAttrName, int nDefaultValue)
        {
            string sValue = GetAttrValue(node, sAttrName, nDefaultValue.ToString());

            try
            {
                return Convert.ToInt16(sValue);
            }
            catch (Exception e)
            {
                throw new XmlDocException("Bad integer data in oxes file: " + e.Message);
            }
        }
        #endregion
        #region SMethod: DateTime GetAttrValue(node, attr, dtDefaultValue)
        static public DateTime GetAttrValue(XmlNode node, string sAttrName, DateTime dtDefaultValue)
        {
            string sDefault = "";
            if (null != dtDefaultValue)
                sDefault = dtDefaultValue.ToString("u", DateTimeFormatInfo.InvariantInfo);

            string sValue = GetAttrValue(node, sAttrName, sDefault);
            if (string.IsNullOrEmpty(sValue))
                return DateTime.Now;

            return DateTime.ParseExact(sValue, "u",
                DateTimeFormatInfo.InvariantInfo);
        }
        #endregion
        #region SMethod: bool GetAttrValue(node, attr, bDefaultValue)
        static public bool GetAttrValue(XmlNode node, string sAttrName, bool bDefaultValue)
        {
            string sDefaultValue = (bDefaultValue) ? "true" : "false";

            string sValue = GetAttrValue(node, sAttrName, sDefaultValue);

            if (sValue.ToUpper() == "TRUE")
                return true;
            return false;
        }
        #endregion


        static public bool IsNode(XmlNode node, string sName)
        {
            if (node.Name.ToUpper() == sName.ToUpper())
                return true;
            return false;
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


        public XmlText AddText(XmlNode node, string sValue)
        {
            XmlText text = CreateTextNode(sValue);
            node.AppendChild(text);
            return text;
        }

        static public XmlNode FindNode(XmlNode nodeParent, string sChildName)
        {
            foreach (XmlNode node in nodeParent.ChildNodes)
            {
                if (IsNode(node, sChildName))
                    return node;
            }

            return null;
        }

        // Retrieve attr values of different types
        static public int GetAttrID(XmlNode node, string sAttrName)
        {
            // Retrieve the string value
            string sID = GetAttrValue(node, sAttrName, "");
            if (string.IsNullOrEmpty(sID))
                return -1;

            return IdToInt(sID);
        }

        public string OneLiner()
        {
            var sb = new StringBuilder();
            this.Save(new StringWriter(sb));
            return sb.ToString();
        }

        static public string OneLiner(XmlNode node)
        {
            return node.OuterXml;

            /**
            var sb = new StringBuilder();
            var w = new XmlTextWriter(new StringWriter(sb));
            node.WriteContentTo(w);
            return sb.ToString();
            **/
        }

        // For Unit Testing ------------------------------------------------------------------
        #region Method: bool IsSame(XmlDoc other)
        public bool IsSame(XmlDoc other)
        {
            string sThis = OneLiner();
            string sOther = other.OneLiner();
            return (sThis == sOther);
        }
        #endregion
        #region SMethod: void DisplayDifferences(xActual, xExpected)
        static public void DisplayDifferences(XmlDoc xActual, XmlDoc xExpected)
        {
            string sActual = xActual.OneLiner();
            string sExpected = xExpected.OneLiner();

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
            Console.WriteLine(OneLiner());
            Console.WriteLine("");
        }
        #endregion
        #region Method: bool Compare(xmlExpected, xmlActual)
        static public bool Compare(XmlDoc xmlExpected, XmlDoc xmlActual)
        {
            bool bIsSame = xmlExpected.IsSame(xmlActual);
            if (!bIsSame)
            {
                xmlActual.WriteToConsole("Actual");
                xmlExpected.WriteToConsole("Expected");
                XmlDoc.DisplayDifferences(xmlActual, xmlExpected);
            }
            return bIsSame;
        }
        #endregion



    }
}
