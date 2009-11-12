#region ***** XmlDoc.cs *****
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
        #region Attr{g}: XmlNode ProblemNode
        XmlNode ProblemNode
        {
            get
            {
                return m_ProblemNode;
            }
        }
        XmlNode m_ProblemNode;
        #endregion

        #region Constructor(sErrorMessage)
        public XmlDocException(XmlNode _ProblemNode, string sMessage)
            : base(sMessage)
        {
            m_ProblemNode = _ProblemNode;
        }
        #endregion

        #region Method: bool FindNodeIndex(node, ref int c)
        public bool FindNodeIndex(XmlNode node, ref int c)
        {
            if (node == ProblemNode)
                return true;

            if (node.Name == ProblemNode.Name)
                c++;

            foreach (XmlNode child in node.ChildNodes)
            {
                if (FindNodeIndex(child, ref c))
                    return true;
            }

            return false;
        }
        #endregion

        #region Method: int GetProblemLineNo(sPath, XmlDocument)
        public int GetProblemLineNo(string sPath, XmlDocument xml)
        {
            // Scan to see the sequential index of the problem node in the xml hierarchy
            int c = 0;
            if (!FindNodeIndex(xml, ref c))
                return 0;

            // Scan the corresponding input file to find the line number containing it
            var sr = new StreamReader(sPath, Encoding.UTF8);
            var tr = TextReader.Synchronized(sr);
            int iLine = 0;
            string sLookFor = "<" + ProblemNode.Name;
            string s;
            while (c >= 0 && (s = tr.ReadLine()) != null)
            {
                while (c >= 0 && s.IndexOf(sLookFor) != -1)
                {
                    s = s.Remove(s.IndexOf(sLookFor), sLookFor.Length);
                    c--;
                }

                iLine++;
            }
            tr.Close();

            return iLine;
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

            // Watch out for discrepancies in the data due to improper xml in now-obsolete store
            // routines; once the upgrade to 2.0 is complete, I can probably remove this.
            // The & character is illegal in Xml, reserved for: &lt; &gt; &amp; &nbsp etc.
            string sOK = "";
            string[] vReserve = new string[] { "&lt;", "&gt;", "&amp;", "&nbsp;", "&quot;" };
            for (int i = 0; i < sXml.Length; i++)
            {
                // If we encoutner an ampersand....
                if (sXml[i] == '&')
                {
                    // See if it is one of our reserve "words"
                    bool bReserveFound = false;
                    foreach (string sReserve in vReserve)
                    {
                        int iReserve = sReserve.Length;
                        if (i + iReserve >= sXml.Length)
                            continue;
                        if (sXml.Substring(i, iReserve) == sReserve)
                        {
                            bReserveFound = true;
                            continue;
                        }
                    }

                    // If not one of our reserved words, convert to an ampersand
                    if (!bReserveFound)
                    {
                        sOK += "&amp;";
                        continue;
                    }

                }

                sOK += sXml[i];
            }

            //Console.WriteLine(sXml);
            //Console.WriteLine(sOK);

            LoadXml(sOK);
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
        #region SMethod: int GetAttrID(XmlNode node, string sAttrName)
        static public int GetAttrID(XmlNode node, string sAttrName)
        {
            // Retrieve the string value
            string sID = GetAttrValue(node, sAttrName, "");
            if (string.IsNullOrEmpty(sID))
                return -1;

            return IdToInt(sID);
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
                throw new XmlDocException(node, "Bad integer data in oxes file: " + e.Message);
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

        #region SMethod: string GetAttrValue(node, vsAttr, sDefaultValue)
        static public string GetAttrValue(XmlNode node, string[] vsAttrName, string sDefaultValue)
        {
            foreach (string sAttrName in vsAttrName)
            {
                string sOut = GetAttrValue(node, sAttrName, sDefaultValue);
                if (!string.IsNullOrEmpty(sOut))
                    return sOut;
            }

            return sDefaultValue;
        }
        #endregion
        #region SMethod: DateTime GetAttrValue(node, vsAttr, dtDefaultValue)
        static public DateTime GetAttrValue(XmlNode node, string[] vsAttrName, DateTime dtDefaultValue)
        {
            string sDefaultValue = dtDefaultValue.ToString("u", DateTimeFormatInfo.InvariantInfo);

            foreach (string sAttrName in vsAttrName)
            {
                DateTime dtOut = GetAttrValue(node, sAttrName, dtDefaultValue);

                string sOut = dtOut.ToString("u", DateTimeFormatInfo.InvariantInfo);

                if (sOut != sDefaultValue)
                    return dtOut;
            }

            return dtDefaultValue;
        }
        #endregion

        #region Method: bool HasAttr(XmlNode node, string sAttrName)
        static public bool HasAttr(XmlNode node, string sAttrName)
        {
            foreach (XmlAttribute attr in node.Attributes)
            {
                if (attr.Name.ToUpper() == sAttrName.ToUpper())
                    return true;
            }
            return false;
        }
        #endregion

        // Misc ops
        #region SMethod: bool IsNode(XmlNode node, string sName)
        static public bool IsNode(XmlNode node, string sName)
        {
            if (node.Name.ToUpper() == sName.ToUpper())
                return true;
            return false;
        }
        #endregion
        #region Method: void AddXmlDeclaration()
        public void AddXmlDeclaration()
            // Creates "<?xml version="1.0" encoding="UTF-8"?>"  
        {
            var decl = CreateXmlDeclaration("1.0", "UTF-8", null);
            var root = DocumentElement;
            InsertBefore(decl, root);
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
        #region Method: XmlText AddText(XmlNode node, string sValue)
        public XmlText AddText(XmlNode node, string sValue)
        {
            XmlText text = CreateTextNode(sValue);
            node.AppendChild(text);
            return text;
        }
        #endregion

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
        #region SMethod: XmlNode FindNode(nodeParent, vsChildName)
        static public XmlNode FindNode(XmlNode nodeParent, string[] vsChildName)
        {
            foreach(string sChildName in vsChildName)
            {
                var node = FindNode(nodeParent, sChildName);
                if (null != node)
                    return node;

            }
            return null;
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
        #region Method: void Write(string sPath)
        public void Write(string sPath)
        {
            // Create the directory if it doesn't exist
            var sFolder = Path.GetDirectoryName(sPath);
            if (!Directory.Exists(sFolder))
                Directory.CreateDirectory(sFolder);

            // Write out the file
            Save(sPath);
        }
        #endregion

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
        #region Method: public void NormalizeCreatedDates()
        void NormalizeCreatedDates(XmlNode node)
        {
            if (null != node.Attributes)
            {
                foreach (XmlAttribute attr in node.Attributes)
                {
                    if (attr.Name.ToUpper() == "CREATED")
                        attr.Value = s_date.ToString("u", DateTimeFormatInfo.InvariantInfo);
                }
            }

            foreach(XmlNode child in node.ChildNodes)
                NormalizeCreatedDates(child);
        }
        static DateTime s_date = DateTime.Now;
        public void NormalizeCreatedDates()
        {
            foreach(XmlNode child in ChildNodes)
                NormalizeCreatedDates(child);
        }
        #endregion
    }


    public class UrlAttr
    {
        #region Attr{g}: string Name
        public string Name
        {
            get
            {
                return m_sName;
            }
        }
        string m_sName;
        #endregion
        #region Attr{g}: string Value
        public string Value
        {
            get
            {
                return m_sValue;
            }
        }
        string m_sValue;
        #endregion

        #region Constructor(sName, sValue)
        public UrlAttr(string sName, string sValue)
        {
            m_sName = sName;
            m_sValue = sValue;
        }
        #endregion
    }

    public class UrlAttrList : List<UrlAttr>
    {
        #region Attr{g}: string IsOxesUrl
        public bool IsOxesUrl
        {
            get
            {
                return m_bIsOxesUrl;
            }
        }
        bool m_bIsOxesUrl = false;
        #endregion
        #region Attr{g}: string OxesFilePath
        public string OxesFilePath
        {
            get
            {
                return m_sOxesFilePath;
            }
        }
        string m_sOxesFilePath;
        #endregion

        #region Constructor
        public UrlAttrList()
        {
        }
        #endregion
        #region Constructor(sPrefix, sUrl)
        public UrlAttrList(string sPrefix, string sUrl)
            : this()
        {
            // Start with the assumption that we don't have a good Oxes Url
            m_bIsOxesUrl = false;

            // Bad Url
            if (string.IsNullOrEmpty(sUrl))
                return;

            // Make sure it is a the right kind of url (e.g., oxes); otherwise we can't identify it
            if (!string.IsNullOrEmpty(sPrefix) && 
                !sUrl.StartsWith(sPrefix, true, CultureInfo.InvariantCulture))
                return;

            // We want to consider what's to the right of the Oxes part.
            string sUrlData = sUrl.Substring(sPrefix.Length);
            var vsSections = sUrlData.Split(new char[] { '/' });
            if (vsSections.Length == 0 || vsSections.Length > 2)
                return;
            string m_sOxesFilePath = (vsSections.Length == 2) ? vsSections[0] : "";
            string sAttributes = (vsSections.Length == 1) ? vsSections[0] : vsSections[1];

            // Parse into name/value pairs
            if (string.IsNullOrEmpty(sAttributes))
                return;
            var vsParts = sAttributes.Split(new char[] { c_chDelimiter });
            if (vsParts.Length == 0)
                return;

            // If we're here, then we've got a Url we can work with 
            m_bIsOxesUrl = true;

            // Read each one into UrlAttrs
            foreach (string s in vsParts)
            {
                int k = s.IndexOf('=');
                if (-1 == k || s.Length == k)
                    continue;

                string sName = s.Substring(0, k);
                string sValue = s.Substring(k+1);

                int n = sValue.Length;
                if (n > 2 && (sValue[0] == '\'' && sValue[n-1] == '\''))
                    sValue = sValue.Substring(1, n-2);

                var ua = new UrlAttr(sName, sValue);
                Add(ua);
            }
        }
        #endregion
        #region Constructor(sUrl)
        public UrlAttrList(string sUrl)
            : this("", sUrl)
        {
        }
        #endregion

        #region Method: string GetValueFor(string sName)
        public string GetValueFor(string sName)
        {
            foreach (var ua in this)
            {
                if (ua.Name == sName)
                    return ua.Value;
            }

            return null;
        }
        #endregion

        public const char c_chDelimiter = '+';

        #region Method: string MakeUrl(sPrefix)
        public string MakeUrl(string sPrefix)
        {
            if (null == sPrefix)
                sPrefix = "";

            string sUrl = sPrefix;

            if (!string.IsNullOrEmpty(OxesFilePath))
                sUrl += (OxesFilePath + "/");

            bool bAmpersand = false;

            foreach (var ua in this)
            {
                if (bAmpersand)
                    sUrl += c_chDelimiter;
                bAmpersand = true;

                sUrl += ua.Name;
                sUrl += '=';
                sUrl += ua.Value;
            }

            return sUrl;
        }
        #endregion
        #region Method: string MakeUrl()
        public string MakeUrl()
        {
            return MakeUrl("");
        }
        #endregion
    }

}
