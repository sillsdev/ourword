/**********************************************************************************************
 * Dll:     JWTools
 * File:    JW_Xml.cs
 * Author:  John Wimbish
 * Created: 19 Dec 2003
 * Purpose: Provides a shorthand for frequent xml operations.
 * Legal:   Copyright (c) 2005-08, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
#region Using
using System;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using System.Text;
using Microsoft.Win32;
#endregion


namespace JWTools
{
    public class XItem
    {
        static protected bool Debugging = true;

        #region VirtAttr{g}: string OneLiner
        public virtual string OneLiner
        {
            get
            {
                return "";
            }
        }
        #endregion
        #region VirtMethod: bool ContentEquals(XItem item)
        public virtual bool ContentEquals(XItem item)
        {
            Debug.Assert(false);
            return false;
        }
        #endregion
    }

    public class XString : XItem
    {
        #region Attr{g}: string Text
        public string Text
        {
            get
            {
                return m_sText;
            }
        }
        string m_sText;
        #endregion

        #region Constructor(sText)
        public XString(string sText)
            : base()
        {
            m_sText = sText;
        }
        #endregion
        #region OMethod: bool ContentEquals(XItem item)
        public override bool ContentEquals(XItem item)
        {
            XString xs = item as XString;
            if (null == xs)
                return false;

            if (Text != xs.Text)
            {
                if (XItem.Debugging)
                {
                    Console.WriteLine("TEXT MISMATCH:");
                    Console.WriteLine("   <" + Text + ">");
                    Console.WriteLine("   <" + xs.Text + ">");
                }
                return false;
            }

            return true;
        }
        #endregion

        #region OAttr{g}: string OneLiner
        public override string OneLiner
        {
            get
            {
                return Text;
            }
        }
        #endregion
    }

    public class XElement : XItem
    {
        // Attrs -----------------------------------------------------------------------------
        #region Attr{g}: string Tag
        public string Tag
        {
            get
            {
                return m_sTag;
            }
        }
        string m_sTag;
        #endregion

        // Sub Items -------------------------------------------------------------------------
        #region Attr{g}: XItem[] Items
        public XItem[] Items
        {
            get
            {
                Debug.Assert(null != m_vItems);
                return m_vItems;
            }
        }
        XItem[] m_vItems = null;
        #endregion
        #region Method: void AddSubItem(XItem)
        public void AddSubItem(XItem item)
        {
            // Append it to the vector
            XItem[] v = new XItem[Items.Length + 1];
            for (int i = 0; i < Items.Length; i++)
                v[i] = Items[i];
            v[Items.Length] = item;
            m_vItems = v;
        }
        #endregion
        #region Method: void AddSubItem(string s)
        public void AddSubItem(string s)
        {
            AddSubItem(new XString(s));
        }
        #endregion

        // Embedded Class XAttr --------------------------------------------------------------
        #region CLASS: XAttr
        public class XAttr
        {
            // Attrs -------------------------------------------------------------------------
            #region Attr{g}: string Tag
            public string Tag
            {
                get
                {
                    return m_sTag;
                }
            }
            string m_sTag;
            #endregion
            #region Attr{g}: Value
            public string Value
            {
                get
                {
                    return m_sValue;
                }
            }
            string m_sValue;
            #endregion

            // Scaffolding -------------------------------------------------------------------
            #region Constructor(sTag, sValue)
            public XAttr(string _sTag, string _sValue)
            {
                m_sTag = _sTag;
                m_sValue = _sValue;
            }
            #endregion
            #region Constructor(sTag, nValue)
            public XAttr(string _sTag, int _nValue)
            {
                m_sTag = _sTag;
                m_sValue = _nValue.ToString();
            }
            #endregion
            #region Method: bool ContentEquals(XAttr attr)
            public bool ContentEquals(XAttr attr)
            {
                if (Tag != attr.Tag)
                    return false;
                if (Value != attr.Value)
                    return false;
                return true;
            }
            #endregion

            // I/O ---------------------------------------------------------------------------
            const char c_chQuote = '\"';
            #region VAttr{g}: string SaveString
            public string SaveString
            {
                get
                {
                    string s = AmpersandsAndSuch(Value);
                    return " " + Tag + '=' + c_chQuote + s + c_chQuote;
                }
            }
            #endregion
        }
        #endregion
        #region Attr{g}: XAttr[] Attrs
        public XAttr[] Attrs
        {
            get
            {
                Debug.Assert(null != m_vXmlAttrs);
                return m_vXmlAttrs;
            }
        }
        XAttr[] m_vXmlAttrs;
        #endregion
        #region Method: void AddAttr(sTag, sValue)
        public void AddAttr(string sTag, string sValue)
        {
            // Create the attribute
            XAttr attr = new XAttr(sTag, sValue);

            // Append it to the vector
            XAttr[] v = new XAttr[Attrs.Length + 1];
            for (int i = 0; i < Attrs.Length; i++)
                v[i] = Attrs[i];
            v[Attrs.Length] = attr;
            m_vXmlAttrs = v;
        }
        #endregion
        #region Method: void AddAttr(sTag, nValue)
        public void AddAttr(string sTag, int nValue)
        {
            AddAttr(sTag, nValue.ToString());
        }
        #endregion
        #region Method: void AddAttr(sTag, bValue)
        public void AddAttr(string sTag, bool bValue)
        {
            string sValue = (bValue) ? "true" : "false";
            AddAttr(sTag, sValue);
        }
        #endregion
        #region Method: string GetAttrValue(sTag, sDefaultValue)
        public string GetAttrValue(string sTag, string sDefaultValue)
        {
            foreach (XAttr attr in Attrs)
            {
                if (attr.Tag == sTag)
                    return attr.Value;
            }
            return sDefaultValue;
        }
        #endregion
        #region Method: bool GetAttrValue(sTag, bDefaultValue)
        public bool GetAttrValue(string sTag, bool bDefaultValue)
        {
            foreach (XAttr attr in Attrs)
            {
                if (attr.Tag == sTag)
                {
                    if (attr.Value == "true")
                        return true;
                    else
                        return false;
                }
            }
            return bDefaultValue;
        }
        #endregion

        // Scaffolding -----------------------------------------------------------------------
        #region Constructor(sTag)
        public XElement(string sTag)
            : base()
        {
            m_sTag = sTag;
            m_vItems = new XItem[0];
            m_vXmlAttrs = new XAttr[0];
        }
        #endregion
        #region OMethod: bool ContentEquals(XItem item)
        public override bool ContentEquals(XItem item)
        {
            XElement xe = item as XElement;
            if (null == xe)
                return false;

            if (Tag != xe.Tag)
            {
                if (XItem.Debugging)
                    Console.WriteLine("TAG MISMATCH: <" + Tag + ">---<" + xe.Tag + ">");
                return false;
            }

            if (Attrs.Length != xe.Attrs.Length)
            {
                if (XItem.Debugging)
                    Console.WriteLine("ATTRS LENGTH MISMATCH: <" + Attrs.Length.ToString() +
                        ">---<" + xe.Attrs.Length.ToString() + ">  Tag=" + Tag);
                return false;
            }

            for (int i = 0; i < Attrs.Length; i++)
            {
                if (!Attrs[i].ContentEquals(xe.Attrs[i]))
                {
                    return false;
                }
            }
            
            if (Items.Length != xe.Items.Length)
            {
                if (XItem.Debugging)
                    Console.WriteLine("ITEMS LENGTH MISMATCH: <" + Items.Length.ToString() + 
                        ">---<" + xe.Items.Length.ToString() + ">  Tag=" + Tag);
                return false;
            }

            for (int i = 0; i < Items.Length; i++)
            {
                if (!Items[i].ContentEquals(xe.Items[i]))
                {
                    return false;
                }
            }

            return true;
        }
        #endregion

        // Virtual Attrs ---------------------------------------------------------------------
        #region VAttr{s}: bool IsEmpty
        public bool IsEmpty
        {
            get
            {
                if (HasAttrs)
                    return false;
                if (HasItems)
                    return false;
                return true;
            }
        }
        #endregion
        #region VAttr{g}: bool HasItems
        bool HasItems
        {
            get
            {
                return (Items.Length > 0);
            }
        }
        #endregion
        #region VAttr{g}: bool HasAttrs
        bool HasAttrs
        {
            get
            {
                return Attrs.Length > 0;
            }
        }
        #endregion

        // I/O -------------------------------------------------------------------------------
        #region OAttr{g}: string OneLiner
        public override string OneLiner
        {
            get
            {
                // We will put out nothing if there's nothing here to write
                if (IsEmpty)
                    return "";

                // Will we need an end tag? Yes if we have Subitems
                bool bUsesEndTag = HasItems;

                // Start with the tag
                string sLine = "<" + Tag;

                // Add the attributes
                foreach (XAttr attr in Attrs)
                    sLine += attr.SaveString;

                // Add the appropriate ending to the start entity
                if (bUsesEndTag)
                    sLine += ">";
                else
                    sLine += "/>";

                // Write out any subfields
                foreach (XItem item in Items)
                    sLine += item.OneLiner;

                // End Tag
                if (bUsesEndTag)
                    sLine += "</" + Tag + ">";

                return sLine;
            }
        }
        #endregion
        #region SMethod: string AmpersandsAndSuch(sIn) - converts, e.g., '<' to "&lt;"
        static public string AmpersandsAndSuch(string sIn)
        {
            if (string.IsNullOrEmpty(sIn))
                return "";

            XmlReplace[] replacements = XmlReplace.Replacements;

            string sOut = "";
            foreach (char ch in sIn)
            {
                bool bReplaced = false;
                foreach (XmlReplace xr in replacements)
                {
                    if (xr.m_chMemory == ch)
                    {
                        sOut += xr.m_sXML;
                        bReplaced = true;
                        break;
                    }
                }

                if (!bReplaced)
                    sOut += ch;
            }
            return sOut;
        }
        #endregion
        #region CLASS: CreateMethod
        public class CreateMethod
        {
            // Content Attrs
            #region Attr{g}: string[] VS
            string[] VS
            {
                get
                {
                    Debug.Assert(null != m_vs);
                    return m_vs;
                }
            }
            string[] m_vs;
            #endregion
            #region Attr{g}: int I
            int I
            {
                get
                {
                    return m_i;
                }
                set
                {
                    m_i = value;
                }
            }
            int m_i;
            #endregion
            #region VAttr{g}: string S
            string S
            {
                get
                {
                    Debug.Assert(!string.IsNullOrEmpty(m_vs[I]));
                    return VS[I];
                }
            }
            #endregion

            // Virtual Attrs
            #region VAttr{g}: bool IsEndTag
            bool IsEndTag
            {
                get
                {
                    if (S.Length > 2 && S.Substring(0, 2) == "</")
                        return true;
                    return false;
                }
            }
            #endregion
            #region VAttr{g}: bool IsBeginTag
            bool IsBeginTag
            {
                get
                {
                    if (S.Length > 1 && S[0] == '<' && !IsEndTag)
                        return true;
                    return false;
                }
            }
            #endregion
            #region VAttr{g}: bool IsSelfClosingTag
            bool IsSelfClosingTag
            {
                get
                {
                    if (!IsBeginTag)
                        return false;

                    if (S.Substring( S.Length-2, 2) == "/>")
                        return true;
                    return false;
                }
            }
            #endregion
            #region VAttr{g}: string Tag - retrieves both begin and end tag names
            string Tag
            {
                get
                {
                    if (!IsBeginTag && !IsEndTag)
                        return null;

                    string sTag = "";

                    for (int i = 0; i < S.Length; i++)
                    {
                        char ch = S[i];

                        if (ch == '<' || ch == '/')
                            continue;

                        if (char.IsWhiteSpace(ch) || ch=='>')
                            break;

                        sTag += ch;
                    }

                    return sTag;
                }
            }
            #endregion

            // Worker Methods
            #region SMethod: string[] ParseIntoXmlStrings(string s)
            static public string[] ParseIntoXmlStrings(string s)
            {
                ArrayList a = new ArrayList();

                // We'll build the lines here
                string sLine = "";

                // Flag so that we get rid of leading spaces, tabs, etc.
                bool bShouldEatWhitespace = true;

                foreach (char ch in s)
                {
                    // Carriage return / line feed: we'll eat whitespace from here on
                    if (char.IsControl(ch))
                    {
                        bShouldEatWhitespace = true;
                        continue;
                    }

                    // Quit eating white space once we encounter something else
                    if (!char.IsWhiteSpace(ch))
                        bShouldEatWhitespace = false;

                    // Eat whitespace if the conditions are correct
                    if (bShouldEatWhitespace)
                        continue;


                    if (ch == '<')
                    {
                        if (sLine.Length > 0)
                            a.Add(sLine);
                        sLine = "";
                    }

                    sLine += ch;

                    if (ch == '>')
                    {
                        if (sLine.Length > 0)
                            a.Add(sLine);
                        sLine = "";
                    }
                }

                // Convert the array into a vector
                string[] vs = new string[a.Count];
                for (int i = 0; i < a.Count; i++)
                {
                    vs[i] = (string)a[i];
                    Debug.Assert(!string.IsNullOrEmpty(vs[i]), "Should eat all empty data");
                }
                return vs;
            }
            #endregion
            #region Method: void ParseAttrs(XElement x, string s)
            void ParseAttrs(XElement x, string s)
            {
                int i = 0;

                // Move past the tag
                while (i < s.Length && s[i] != ' ')
                    i++;

                bool bIsAtAttr = true;
                bool bIsAtValue = false;

                // Loop until we end
                string sAttr = "";
                string sValue = "";
                while (i < s.Length)
                {
                    char ch = s[i];

                    // Break at end
                    if (ch == '>' || ch == '/')
                        break;

                    // Move past whitespace
                    if (!bIsAtValue && char.IsWhiteSpace(ch))
                        goto loop;

                    // Switches
                    if (ch == '=')
                    {
                        bIsAtAttr = false;
                        bIsAtValue = false;
                        goto loop;
                    }

                    if (ch == '\"')
                    {
                        if (bIsAtValue)
                        {
                            x.AddAttr(sAttr, sValue);
                            sAttr = "";
                            sValue = "";
                            bIsAtAttr = true;
                            bIsAtValue = false;
                        }
                        else
                            bIsAtValue = true;
                        goto loop;
                    }

                    // Collect the attr name
                    if (bIsAtAttr)
                        sAttr += ch;
                    if (bIsAtValue)
                        sValue += ch;

                loop:
                    i++;
                    
                }
            }
            #endregion
            #region Method: void ReadElement(XElement x) - recurses down
            void ReadElement(XElement x)
            {
                while (!IsEndTag)
                {
                    // Not a begin tag, then it is string data
                    if (!IsBeginTag)
                    {
                        x.AddSubItem(S);
                    }

                    // Otherwise an XElement tag
                    else
                    {
                        XElement xDaughter = new XElement(Tag);
                        ParseAttrs(xDaughter, S);
                        x.AddSubItem(xDaughter);
                        if (!IsSelfClosingTag)
                        {
                            I++;
                            ReadElement(xDaughter);
                        }
                    }

                    // Ready for the next one
                    I++;
                }
            }
            #endregion

            // Public Interface
            #region Constructor(sOneLiner)
            public CreateMethod(string s)
            {
                m_vs = ParseIntoXmlStrings(s);
                m_i = 0;
            }
            #endregion
            #region Method: XElement[] Run()
            public XElement[] Run()
            {
                ArrayList a = new ArrayList();

                for(I = 0; I<VS.Length; I++)
                {
                    if (IsBeginTag)
                    {
                        XElement x = new XElement(Tag);
                        ParseAttrs(x, S);
                        a.Add(x);
                        if (!IsSelfClosingTag)
                        {
                            I++;
                            ReadElement(x);
                        }

                    }
                }

                // Convert to an array and return the result
                XElement[] v = new XElement[a.Count];
                for (int k = 0; k < a.Count; k++)
                    v[k] = a[k] as XElement;
                return v;
            }
            #endregion
        }
        #endregion
        #region SMethod: XElement[] CreateFrom(s)
        static public XElement[] CreateFrom(string s)
        {
            CreateMethod method = new CreateMethod(s);
            return method.Run();
        }
        #endregion
    }


    //////////////////////////////////////////////////////////////////////////////////////////


    #region CLASS: XmlField
    public class XmlField
		#region Documentation
		/* Used to output a field (level) of xml. Supports several different tag formats:
		 * 
		 *	 (a) <tag>data</tag>
		 * 
		 *	 (b) <tag attr1=data1 attr2=data2>   (no end tag)
		 * 
		 *	 (c) <tag attr1=data1 attr2=data2>data</tag>
		 * 
		 *	 (d) <tag>
		 *          data
		 *	     </tag>
		 * 
		 * Each level of tags is indented automatically (with two spaces) for readability.
		 * 
		 * To use:
		 * 
		 * At the topmost level, use the constructor to create an XmlField. Then pass this
		 * XmlField down to the other levels. Anytime a new field is needed, create it with
		 * the GetDaughterXmlField method; in this manner the indentation will be properly
		 * handled.
		 * 
		 * Start the field's data with the Begin() method. When done, use the End() method
		 * to put in the end tag. The WriteDataLine() method is for writing data that 
		 * appears between the two tags. 
		 * 
		 * The following method would be found in an object. It receives the parent field,
		 * and the first thing done is to create an XmlField for this daughter object. 
		 * 
		 * 		public void Write(XmlField xmlParent)
		 *      {
		 *          XmlField xml = xmlParent.GetDaughterXmlField("MyTag", true);
		 *          xml.Begin( XmlField.BuildAttrString("Height", Height.ToString()) );
		 *          xml.WriteDataLine("Here is my data.");
		 *          m_wnd.Write(xml);
		 *          xml.End();
		 *      }
		 * 
		 * This results in
		 * 
		 *      <MyTag Height="20">
		 *      Here is my data.
		 *      </MyTag>
		 * 
		 * The OneLiner methods are used for tags where we want it all on one line. Thus:
		 *     xml.OneLiner( XmlField.BuildAttrString("Height", Height.ToString()), "Here is my data");
		 * results in:
		 *     <MyTag Height="20">Here is my data</MyTag>
		 * 
		 * It is possible to not have an end tag for those tags which only have attributes, e.g.,
		 *     <MyTag Height="20">
		 * To do this, set  bUsesEndTag to false in the GetDaughterXmlField method. (Note that
		 * at the first level, the constructor does not allow this option, as we assume that
		 * the top level will always contain other levels.)
		 */
		#endregion
	{
		// Implementational Attributes -------------------------------------------------------
		TextWriter m_writer;
		string m_sTag;
		int    m_nIndent;
		bool   m_bEnded;
		bool   m_bUsesEndTag;
		#region Attr{g} string Padding - returns a string containing the correct amt of leading spaces
		private string Padding
		{
			get
			{
				return new string(' ', m_nIndent * 2);
			}
		}
		#endregion

		// Constructors ----------------------------------------------------------------------
		#region Constructor(writer, sTag) - use this only at the first level
		public XmlField(TextWriter writer, string sTag)
		{
			m_writer = writer;
			m_sTag = sTag;
			m_nIndent = 0;
			m_bEnded = false;
			m_bUsesEndTag = true;
		}
		#endregion
		#region protected Constructor(writer, sTag, nIndent, bUsesEndTag) - used by GetDaughterXmlField
		protected XmlField(TextWriter writer, string sTag, int nIndent, bool bUsesEndTag)
		{
			m_writer = writer;
			m_sTag = sTag;
			m_nIndent = nIndent;
			m_bEnded = false;
			m_bUsesEndTag = bUsesEndTag;
		}
		#endregion
		#region Method: XmlField GetDaughterXmlField(sTag, bUsesEndTag) - constructs one with increased indent
		public XmlField GetDaughterXmlField(string sTag, bool bUsesEndTag)
		{
			Debug.Assert(true == m_bUsesEndTag);
			Debug.Assert(false == m_bEnded);
			return new XmlField(m_writer, sTag, m_nIndent + 1, bUsesEndTag);
		}
		#endregion

        // String Builder Methods ------------------------------------------------------------
        #region SMethod: string AmpersandsAndSuch(sIn) - converts, e.g., '<' to "&lt;"
        static public string AmpersandsAndSuch(string sIn)
        {
            XmlReplace[] replacements = XmlReplace.Replacements;

            string sOut = "";
            foreach (char ch in sIn)
            {
                bool bReplaced = false;
                foreach (XmlReplace xr in replacements)
                {
                    if (xr.m_chMemory == ch)
                    {
                        sOut += xr.m_sXML;
                        bReplaced = true;
                        break;
                    }
                }

                if (!bReplaced)
                    sOut += ch;
            }
            return sOut;
        }
        #endregion
        #region SMethod: string BuildAttrString(sAttr, sValue)
        public static string BuildAttrString(string sAttr, string sValue)
        {
            string sData = AmpersandsAndSuch(sValue);
            return " " + sAttr + "=\"" + sData + "\"";
        }
        #endregion
        #region SMethod: string BuildAttrString(sAttr, nValue)
        public static string BuildAttrString(string sAttr, int nValue)
        {
            string sValue = nValue.ToString();
            return BuildAttrString(sAttr, sValue);
        }
        #endregion
        #region SMethod: string BuildBeginTag(sTag, sValues, bEndsHere)
        static public string BuildBeginTag(string sTag, string sValues, bool bEndsHere)
        {
            string sEnd = (bEndsHere) ? "/>" : ">";

            if (string.IsNullOrEmpty(sValues))
                return "<" + sTag + sEnd;
            return "<" + sTag + sValues + sEnd;
        }
        #endregion
        #region SMethod: string BuildEndTag(sTag)
        static public string BuildEndTag(string sTag)
        {
            return "</" + sTag + ">";
        }
        #endregion
        #region SMethod: string BuildOneLiner(sTag, sValues, sData)
        static public string BuildOneLiner(string sTag, string sValues, string sData)
        {
            if (!string.IsNullOrEmpty(sValues) && sValues[0] != ' ')
                sValues = " " + sValues;

            return BuildBeginTag(sTag, sValues, false) +
                AmpersandsAndSuch(sData) +
                BuildEndTag(sTag);
        }
        #endregion

        // Output methods --------------------------------------------------------------------
		#region Method: void OneLiner(sData) - produces "<tag>my data</tag>"
		public void OneLiner(string sData)
		{
			Debug.Assert(true == m_bUsesEndTag);
			Debug.Assert(false == m_bEnded);

            m_writer.WriteLine(Padding + 
                BuildOneLiner(m_sTag, null, sData));

			m_bEnded = true;
		}
		#endregion
		#region Method: void OneLiner(sValues, sData) - produces "<tag name="John">my data</tag>"
		public void OneLiner(string sValues, string sData)
		{
			Debug.Assert(true == m_bUsesEndTag);
			Debug.Assert(false == m_bEnded);

            m_writer.WriteLine(Padding +
                BuildOneLiner(m_sTag, sValues, sData));

			m_bEnded = true;
		}
		#endregion
		#region Method: void Begin() - produces "<tag>"
		public void Begin()
		{
			Debug.Assert(true == m_bUsesEndTag);
			Debug.Assert(false == m_bEnded);
            m_writer.WriteLine(Padding + BuildBeginTag(m_sTag, null, false));
		}
		#endregion
		#region Method: void Begin(string sValues) - produces "<tag name="John">"
		public void Begin(string sValues)
		{
			Debug.Assert(false == m_bEnded);

            m_writer.WriteLine(Padding + BuildBeginTag(m_sTag, sValues, false));
		}
		#endregion
		#region Method: void WriteDataLine(sDataLine) - writes out a string of data (e.g., between tags)
		public void WriteDataLine(string sDataLine)
		{
			Debug.Assert(true == m_bUsesEndTag);
			Debug.Assert(false == m_bEnded);
			m_writer.WriteLine( Padding + sDataLine );
		}
		#endregion
		#region Method: void End() - writes "</tag>" (or nothing if no end-tag desired)
		public void End()
		{
			Debug.Assert(false == m_bEnded);

			if (true == m_bUsesEndTag)
                m_writer.WriteLine(Padding + BuildEndTag(m_sTag));

			m_bEnded = true;
		}
		#endregion
        #region Method: void WriteBlankLine() - desirable sometimes for aesthetic reasons
		public void WriteBlankLine()
		{
			m_writer.WriteLine("");
		}
		#endregion
    }
    #endregion

    #region CLASS: XmlReplace
    public class XmlReplace
    {
        public string m_sXML;
        public char m_chMemory;

        #region Constructor(sXML, chMemory)
        public XmlReplace(string _sXML, char _chMemory)
        {
            m_sXML = _sXML;
            m_chMemory = _chMemory;
        }
        #endregion

        #region SAttr{g}: XmlReplace[] Replacements - the actual set of replacements
        static public XmlReplace[] Replacements
        {
            get
            {
                if (null == s_vReplacments)
                {
                    s_vReplacments = new XmlReplace[] 
                    { 
                        new XmlReplace( "{n}", '\n' ),
                        new XmlReplace( "&amp;", '&' ),
                        new XmlReplace( "&quot;", '\"' ),
                        new XmlReplace( "&lt;", '<' ),
                        new XmlReplace( "&gt;", '>' )
                    };
                }

                return s_vReplacments;

                /**
                return new XmlReplace[] 
                    { 
                        new XmlReplace( "{n}", '\n' ),
                        new XmlReplace( "&amp;", '&' ),
                        new XmlReplace( "&quot;", '\"' ),
                        new XmlReplace( "&lt;", '<' ),
                        new XmlReplace( "&gt;", '>' )
                    };
                **/
            }
        }
        static XmlReplace[] s_vReplacments = null;
        #endregion
    };
    #endregion

    #region CLASS: XmlRead
    public class XmlRead
		#region Documentation
		/* Supports reading xml data. There are a series of methods to read through the
		 * file, and then another set of methods to retrieve values from a line that has
		 * been read in. As an example of the syntax:
		 * 
		 *		while (xml.ReadNextLineUntilEndTag("MyTag") )  // reads to "</MyTag>"
		 *		{
		 *			if (xml.IsTag( "AnotherTag" ))             // if line has <AnotherTag
		 *			{
		 *				string sValue = xml.GetValue("Name");  // Extract: Name="Hello"
		 *			}
		 *		}
		 */
		#endregion
	{
		// Implementational Attributes -------------------------------------------------------
		TextReader m_reader;
		#region Attr{g}: string CurrentLine
		public string CurrentLine
		{
			get { return m_sLine; }
		}
		string m_sLine;           // Current line
		#endregion

		// Scaffolding -----------------------------------------------------------------------
		#region Constructor(TextReader reader)
		public XmlRead(TextReader reader)
		{
			m_reader = reader;
		}
		#endregion

		// Read Methods (advances the StreamReader) ------------------------------------------
		#region Method: bool ReadToTag(XmlTag) - reads until the <XmlTag> is found
		public bool ReadToTag(string XmlTag)
		{
			while ( ReadNextLine() )
			{
				if (IsTag(XmlTag))
					return true;
			}
			return false;
		}
		#endregion
		#region Method: bool ReadNextLine() - sets m_sLine to the current line
		public bool ReadNextLine()
		{
			m_sLine = m_reader.ReadLine();
			return (null != m_sLine);
		}
		#endregion
		#region bool ReadNextLineUntilEndTag(string sEndTag) - returns next line, stops at EndTag
		public bool ReadNextLineUntilEndTag(string sEndTag)
		{
			m_sLine = m_reader.ReadLine();
			if (IsClosingTag(sEndTag) )
				return false;
			return (null != m_sLine);
		}
		#endregion

		// Tests and Data Retrieval ----------------------------------------------------------
		#region public bool IsTag(sTag) - T if sLine has sTag in it
		public bool IsTag(string sTag)
		{
			return (sTag == GetTag());
		}
		#endregion
		#region public static string GetTag() - retrieves the tag from the line
		public string GetTag()
		{
			// No leading spaces
			string sLine = m_sLine.Trim();

			// Move past the opening angle
			int i = 0;
			if(i < sLine.Length && sLine[i] == '<')
				++i;

			// Copy in the tag
			string sTag = "";
			while( i < sLine.Length && sLine[i] != ' ' && sLine[i]!='>')
			{
				sTag += sLine[i];
				++i;
			}
			return sTag;
		}
		#endregion
		#region public string GetValue(sAttr) - returns the value for an attribute
		public string GetValue(string sAttr)
		{
			string sValue = "";

            // Locate the attribute. We can't do IndexOf, because we must only check outside of
            // quotes.
            int i = 0;
            bool bWithinQuotes = false;
            int iEnd = m_sLine.Length - sAttr.Length;
            for (; i < iEnd; i++)
            {
                if (m_sLine[i] == '\"')
                    bWithinQuotes = !bWithinQuotes;

                if (!bWithinQuotes && m_sLine.Substring(i, sAttr.Length) == sAttr)
                    break;
            }
            if (i == iEnd)
                return null;

            // Move past any, e.g., white space
			while (i < m_sLine.Length && m_sLine[i] != '\"')
				++i;

            // Move past the opening quote mark
			if (i < m_sLine.Length && m_sLine[i] == '\"')
				++i;

            // Collect the data
			while( i < m_sLine.Length && m_sLine[i] != '\"')
			{
                sValue += m_sLine[i];
                i++;
            }

            // Special characters
            string sOut = _AmpersandsAndSuch(sValue);

            return sOut;
		}
		#endregion
        #region Method: string _AmpersandsAndSuch(string sIn)
        string _AmpersandsAndSuch(string sIn)
        {
            string sOut = "";

            XmlReplace[] replacements = XmlReplace.Replacements;

            int i = 0;
            while (i < sIn.Length)
            {
                bool bReplaced = false;
                foreach (XmlReplace xr in replacements)
                {
                    int n = xr.m_sXML.Length;
                    if (i < sIn.Length - n && sIn.Substring(i, n) == xr.m_sXML)
                    {
                        sOut += xr.m_chMemory;
                        i += n;
                        bReplaced = true;
                        break;
                    }
                }

                // Everything else
                if (!bReplaced)
                {
                    sOut += sIn[i];
                    i++;
                }
            }

            return sOut;
        }
        #endregion
        #region Method: string GetOneLinerData()
        public string GetOneLinerData()
		{
			string sRight = m_sLine.Substring(m_sLine.IndexOf('>') + 1);
			string sData  = sRight.Substring(0, sRight.IndexOf("</"));
            string sOut = _AmpersandsAndSuch(sData);
            return sOut;
		}
		#endregion
		#region public bool IsClosingTag(sTag) - T if sLine has the closing sTag in it
		public bool IsClosingTag(string sTag)
		{
			if (sTag.Length > 0 && sTag[0] != '/')
				sTag = "/" + sTag;
			return (sTag == GetTag());
		}
		#endregion
    }
    #endregion

    #region CLASS: SfRead
    public class SfRead
	{
		// Attributes ------------------------------------------------------------------------
		StreamReader m_reader = null;
		public string Marker;
		public string Data;
		public bool   AtEndOfFile = false;
		private bool  m_bSuppressReadOnce = false;
		public int LineNumber = 0;


        // Scaffolding -----------------------------------------------------------------------
        #region Constructor(StreamReader)
        public SfRead(StreamReader r)
        {
            m_reader = r;
            LineNumber = 0;
        }
        #endregion
        #region Constructor(sPathName, sFileFilter, enc) - opens the reader
        public SfRead(ref string sPathName, string sFileFilter)
		{
			m_reader = JW_Util.OpenStreamReader(ref sPathName, sFileFilter);
			LineNumber = 0;
		}
		#endregion
		#region Method: public void Close() - closes the StreamReader
		public void Close()
		{
			m_reader.Close();
		}
		#endregion

		// Data Retrieval --------------------------------------------------------------------
		#region Method: void SuppressReadNextTime() - pushes the current field so it is, in effect, read again
		public void SuppressReadNextTime()
		{
			m_bSuppressReadOnce = true;
		}
		#endregion
		#region Method: ReadNextField() - places values into Marker and Data
		public bool ReadNextField()
		{
			// Option to not do the read; this is helpful when returning from a call stack,
			// e.g., where the called method reads up through a \mkr, but the caller
			// needs to also process that \mkr as well.
			if (m_bSuppressReadOnce)
			{
				m_bSuppressReadOnce = false;
				return true;
			}

			// Reset
			Marker = "";
			Data   = "";

			// Read until we get a line with a field marker in it
			string sLine = "";
			while ( (sLine = m_reader.ReadLine()) != null)
			{
				++LineNumber;

				if (sLine.Length > 1 && sLine[0] == '\\')
					break;

			}
			if (sLine == null)
			{
				AtEndOfFile = true;
				return false;
			}

			// Extract the marker and the first line of data
			if (sLine[0] == '\\' && sLine[1] != '\0')
			{
				int i = 1;
				while(i < sLine.Length && sLine[i] != ' ')
				{
					Marker += sLine[i];
					++i;
				}
				while(i < sLine.Length && sLine[i] == ' ')
					++i;
				Data = sLine.Substring(i).Trim();
			}

			// Append any additional lines up until the next marker
			int nPeek;
			while ( (nPeek = m_reader.Peek()) != -1)
			{
				if (nPeek == '\\')
					break;

				string sAppend = m_reader.ReadLine();
				Data = Data.Trim() + (" " + sAppend );
				++LineNumber;
			}
			Data = Data.Trim();

			// End of file
			if ("" == Marker)
			{
				AtEndOfFile = true;
				return false;
			}

			// Still more to be read
			return true;
		}
		#endregion
    }
    #endregion

    #region CLASS: SfWrite
    public class SfWrite
	{
		// Attributes ------------------------------------------------------------------------
		private TextWriter m_writer = null;
		private int m_cMarkerPad = 7;
		private bool m_bUseMkrPadding = false;
		private int m_cMaxLineLength = 60;

		// Scaffolding -----------------------------------------------------------------------
		#region Constructor(sPathName)
		public SfWrite(string sPathName)
		{
			m_writer = JW_Util.GetTextWriter(sPathName);
		}
		#endregion
		#region Method: void Close() - closes the writer; no more writing is possible
		public void Close()
		{
			m_writer.Close();
			m_writer = null;
		}
		#endregion

		// Methods ---------------------------------------------------------------------------
		#region Method: string PrepareFullMarker(string sMarker) - turns "mkr" into "\mkr "
		private string PrepareFullMarker(string sMarker)
		{
			// No blank spaces in the marker for starters
			sMarker = sMarker.Trim();

			// Make sure we have a non-null marker
			if (sMarker.Length == 0)
				throw new Exception("Empty marker");
			if (sMarker.Length == 1 && sMarker[0] == '\\')
				throw new Exception("Marker is only a backslash.");;

			// Make sure the marker begins with a backslash
			if (sMarker[0] != '\\')
				sMarker = "\\" + sMarker;

			// Make sure the marker ends with a blank space
			sMarker = sMarker + " ";

			// Option to pad the marker to achieve the length
			if (m_bUseMkrPadding)
			{
				while (sMarker.Length < m_cMarkerPad)
				{
					sMarker = sMarker + " ";
				}
			}

			// Return the result
			return sMarker;
		}
		#endregion
		#region Method: ArrayList PrepareData(sData)
		private ArrayList PrepareData(string sData)
		{
			// Destination for the field once broken down into lines
			ArrayList rgLines = new ArrayList();

			// For the first line, the length we're looking for must take into account the
			// length of the marker. Afterwards, we'll switch it to using just the maxlength
			// without the marker (as the marker only appears on the first line).
			int cMaxLength = m_cMaxLineLength - m_cMarkerPad;

			// We use the bBlankFound boolean to determine when to split a line. We only
			// want to split after blanks, so that blank(s) always appear at the end of
			// a line, not at the beginning.
			bool bBlankFound = false;

			// We use sLine to build each individual line; when it is full, we add it to the
			// rgLines array, and start again with an empty sLine. We use "i" as the counter
			// to determine when it is full.
			string sLine = "";

			// Loop through each character in the input stream, breaking it down into lines
			foreach( char c in sData)
			{
				// If the line is long enough, break it.
				if (sLine.Length > m_cMaxLineLength)
				{
					// First look for a blank space; signal that we have found one
					if ( c == ' ')
						bBlankFound = true;

					// If we are now at a non-blank space, and have previously found 
					// a blank, then we are ready to break the line. We do this by adding
					// it to the array, and then re-setting our temp line back to empty.
					if ( c != ' ' && bBlankFound)
					{
						rgLines.Add(sLine.TrimEnd());
						sLine = "";
						bBlankFound = false;
						cMaxLength = m_cMaxLineLength;
					}
				}

				sLine += c;
			}

			// Add the final line (which is likely a partial)
			if (0 != sLine.Length)
				rgLines.Add(sLine);

			// Return the result
			return rgLines;
		}
		#endregion
		#region Method: void Write(sMarker, string sData, bWrapLines)
        public void Write(string sMarker, string sData, bool bWrapLines)
		{
            // Add the backslash and trailing space to the marker
			string sFullMarker = PrepareFullMarker(sMarker);

            // Option 1 - All on a single line
            if (!bWrapLines)
            {
                m_writer.WriteLine(sFullMarker + sData);
                return;
            }

            // Option 2 - Split into short lines
			// Prepare the data for output
			ArrayList rgLines = PrepareData(sData);

			// Write out the first line (which includes the marker)
			string sFirstLine = sFullMarker;
			if (rgLines.Count > 0)
				sFirstLine += rgLines[0];
			m_writer.WriteLine(sFirstLine);

            // Write out the remaining lines
			for(int i = 1; i < rgLines.Count; i++)
				m_writer.WriteLine(rgLines[i]);
		}
		#endregion
		#region Method: void WriteBlankLine() - puts out a blank line
		public void WriteBlankLine()
		{
			m_writer.WriteLine("");
		}
		#endregion
    }
    #endregion

    // WE WANT TO EVENTUALLY MAKE THIS OBSOLETE THROUGH ALL PROJECTS
	#region HEADED FOR OBSOLESCENCE - Class JW_Xml
	public class JW_Xml
	{
		#region public static string GetTag(sLine) - retrieves the tag from the xml marker
		static public string GetTag(string sLine)
		{
			// No leading spaces
			sLine = sLine.Trim();

			// Move past the opening angle
			int i = 0;
			if(i < sLine.Length && sLine[i] == '<')
				++i;

			// Copy in the tag
			string sTag = "";
			while( i < sLine.Length && sLine[i] != ' ' && sLine[i]!='>')
			{
				sTag += sLine[i];
				++i;
			}
			return sTag;
		}
		#endregion
		#region public static bool IsTag(sTag, sLine) - T if sLine has sTag in it
		static public bool IsTag(string sTag, string sLine)
		{
			return (sTag == GetTag(sLine));
		}
		#endregion
		#region public static bool IsColsingTag(sTag, sLine) - T if sLine has the closing sTag in it
		static public bool IsClosingTag(string sTag, string sLine)
		{
			if (sTag.Length > 0 && sTag[0] != '/')
				sTag = "/" + sTag;
			return (sTag == GetTag(sLine));
		}
		#endregion
		#region public static string GetValue(sAttr, sLine) - returns the value for an attribute
		static public string GetValue(string sAttr, string sLine)
		{
			string sValue = "";
			int i = sLine.IndexOf(sAttr);
			while (i < sLine.Length && sLine[i] != '\"')
				++i;
			while (i < sLine.Length && sLine[i] == '\"')
				++i;
			while( i < sLine.Length && sLine[i] != '\"')
			{
				sValue += sLine[i];
				i++;
			}
			return sValue;
		}
		#endregion

		public static string GetAttrValueString(string sAttr, string sValue)
		{
			return sAttr + "=\"" + sValue + "\" ";
		}


		public static string BeginTag(string sTag, string sValues)
		{
			m_nIndent += 2;
			return (new string(' ', m_nIndent * 2) + "<" + sTag + " " + sValues + ">");
		}
		public static string BeginTag(string sTag)
		{
			m_nIndent += 2;
			return (new string(' ', m_nIndent * 2) + "<" + sTag + ">");
		}
		public static string EndTag(string sTag)
		{
			m_nIndent -= 2;
			Debug.Assert(m_nIndent >= 0);
			return (new string(' ', (m_nIndent + 2) * 2) + "</" + sTag + ">");
		}


		private static int m_nIndent = 0;
	}
	#endregion


}
