/**********************************************************************************************
 * App:     Josiah
 * File:    JObject.cs
 * Author:  John Wimbish
 * Created: 27 Oct 2003
 * Purpose: Implements JObject, which is a base class for all data objects.
 * Legal:   Copyright (c) 2005-08, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
#region Header: Using, etc.
using System;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using System.IO;
using System.Reflection;

using JWTools;
#endregion

namespace JWdb
{
    #region DOC - Defining a new type of Simple BAttr
    /* DOC: To Define a new type of BAttr, do the following in JObject
     * 
     * - Add the test case for this type to the test: T_JObject.SimpleBAttrIO()
     * 
     * - Create a SetValue(ref YourNewType attr, YourNewType, newValue); its purpose
     *     is to call DeclareDirty.
     * 
     * - Add DefineAttr methods to everything in the I/O:BAttrs section:
     *    _LoopAttrsMethod
     *    _SaveBasicAttrsMethod
     *    to the JObject class itself
     * 
     */
    #endregion

    // Exceptions ----------------------------------------------------------------------------
	#region Exception: eDuplicateAttrName - Attempt to create two attrs with the same name
	public class eDuplicateAttrName : eJosiahException
	{
		public eDuplicateAttrName(string sMethodName)
			: base("Attempt to create multiple attributes with the same name - " + sMethodName)
		{}
	}
	#endregion


	public class JObject : Object
		#region Documentation
		// Requirements for subclasses:
		// 1. Implement a public "Read" constructor that takes the arguments:
		//       string sFirstLine - a line verifying the type of object
		//       StreadReader r    - for getting subsequent lines, if necessary
		// This is necessary for the class to read in from xml.
		#endregion
	{
		// Public attributes -----------------------------------------------------------------
		#region Attr(g): JObject Owner - returns the owning JObject
		public JObject Owner
		{
			get 
			{
				return m_objOwner;
			}
			set
				// Only code crafted with Josiah should set the owner; client code should
				// not ever do this. The assertion says that if the object already has
				// an owner, it is illegal to try to give it a new owner. E.g., we don't want
				// to get away with inserting the obj into two different owning sequences.
			{
				if (m_objOwner != null && value != null)
					throw new eAlreadyOwned("Owner{set}");
				m_objOwner = value;
			}
		}
		private JObject m_objOwner = null;
		#endregion
		#region Attr{g}: JObject RootOwner - returns the topmost owner in the hierarchy
		public JObject RootOwner
		{
			get
			{
				ArrayList list = AllOwners;
				return (JObject)list[0];
			}
		}
		#endregion
		#region Attr{g}: ArrayList AllOwners - returns the ownership hierarchy
		public ArrayList AllOwners
		{
			get
			{
				ArrayList rg = new ArrayList();
				_RecurseAllOwners(rg);
				return rg;
			}
		}
		private ArrayList _RecurseAllOwners(ArrayList rg)
			// Helper method for recursion, only AllOwners should call this. 
			// If this object has an owner, we insert that owner into the front of the
			// array; then we call _RecurseAllOwners on the owner itself, and thus
			// recurse up the ownership chain. The goal is to return the ArrayList with
			// the root owner being the first element in the list.
		{
			Debug.Assert(null != rg);
			if (null != Owner)
			{
				rg.Insert(0, Owner);
				Owner._RecurseAllOwners(rg);
			}
			return rg;
		}
		#endregion
		#region Attr{g}: bool IsRoot - returns T if this object is the root (has no owner)
		public bool IsRoot
		{
			get
			{
				return (Owner == null);
			}
		}
		#endregion
		#region Attr(g): string SortKey - must be overridden if sorting is desired.
		public virtual string SortKey
			// In order to support sorting, the subclass must implement a SortKey attribute,
			// and this SortKey must return something other than an empty string. 
		{
			get 
			{ 
				return ""; 
			}
		}
		#endregion

		// Dirty - Need to Save? -------------------------------------------------------------
		#region Attr{g}: JObjectOnDemand SaveObj - return Save Obj that owns this object
		public virtual JObjectOnDemand SaveObj
		{
			get
			{
				if (null == Owner)
					return null;
				return Owner.SaveObj;
			}
		}
		#endregion
		#region Method: void DeclareDirty() - mark the Save Obj as needing to be saved
		public virtual void DeclareDirty()
		{
			JObjectOnDemand obj = SaveObj;
			if (null != obj)
				obj.DeclareDirty();
		}
		#endregion

        #region Method: void SetValue(ref sAttr, sNewValue)
        protected void SetValue(ref string sAttr, string sNewValue)
        {
            if (sAttr != sNewValue)
            {
                sAttr = sNewValue;
                DeclareDirty();
            }
        }
        #endregion
        #region Method: void SetValue(ref bAttr, bNewValue)
        protected void SetValue(ref bool bAttr, bool bNewValue)
        {
            if (bAttr != bNewValue)
            {
                bAttr = bNewValue;
                DeclareDirty();
            }
        }
        #endregion
        #region Method: void SetValue(ref nAttr, nNewValue)
        protected void SetValue(ref int nAttr, int nNewValue)
        {
            if (nAttr != nNewValue)
            {
                nAttr = nNewValue;
                DeclareDirty();
            }
        }
        #endregion
        #region Method: void SetValue(ref dAttr, dNewValue)
        protected void SetValue(ref double dAttr, double dNewValue)
        {
            if (dAttr != dNewValue)
            {
                dAttr = dNewValue;
                DeclareDirty();
            }
        }
        #endregion
        #region Method: void SetValue(ref DateTime dtAttr, dtNewValue)
        protected void SetValue(ref DateTime dtAttr, DateTime dtNewValue)
        {
            if (dtAttr != dtNewValue)
            {
                dtAttr = dtNewValue;
                DeclareDirty();
            }
        }
        #endregion
        #region Method: void SetValue(ref chAttr, chNewValue)
        protected void SetValue(ref char chAttr, char chNewValue)
        {
            if (chAttr != chNewValue)
            {
                chAttr = chNewValue;
                DeclareDirty();
            }
        }
        #endregion
        #region Method: void SetValue(ref BStringArray vAttr, BStringArray vNewValue)
        protected void SetValue(ref BStringArray vAttr, BStringArray vNewValue)
        {
            if (vAttr != vNewValue)
            {
                vAttr = vNewValue;
                DeclareDirty();
            }
        }
        #endregion

        // Retrieve Content Attribute collections --------------------------------------------
		#region Attr{g}: ArrayList AllAttrs - list of all this obj's non-basic attributes
		// The purpose of this list is to provide a means of iterating through all of the
		// objects attributes; e.g., for doing a Write operation.
		public ArrayList AllAttrs
		{
			get
			{
				return m_Attributes;
			}
		}
		private ArrayList m_Attributes = new ArrayList();
		#endregion
		#region Attr{g}: ArrayList AllOwningAttrs - list containing all JOwnSeq and JOwn attrs
		public ArrayList AllOwningAttrs
		{
			get
			{
				ArrayList list = AllJOwnAttrs;
				list.AddRange( AllJOwnSeqAttrs );
				return list;
			}
		}
		#endregion
		#region Attr{g}: ArrayList AllJOwnAttrs - list of all this obj's JOwn attrs
		public ArrayList AllJOwnAttrs
		{
			get
			{
				ArrayList list = new ArrayList();
				foreach (JAttr attr in AllAttrs)
				{
					if (attr.GetType() == typeof(JOwn))
						list.Add(attr);
				}
				return list;
			}
		}
		#endregion
		#region Attr{g}: ArrayList AllJOwnSeqAttrs - list of all this obj's JOwnSeq attrs
		public ArrayList AllJOwnSeqAttrs
		{
			get
			{
				ArrayList list = new ArrayList();
				foreach (JAttr attr in AllAttrs)
				{
					if (attr.GetType() == typeof(JOwnSeq))
						list.Add(attr);
				}
				return list;
			}
		}
		#endregion
		#region Attr{g}: ArrayList AllJRefAttrs - list of all this obj's JRef attrs
		public ArrayList AllJRefAttrs
		{
			get
			{
				ArrayList list = new ArrayList();
				foreach (JAttr attr in AllAttrs)
				{
					if (attr.GetType() == typeof(JRef))
						list.Add(attr);
				}
				return list;
			}
		}

		#endregion

		// Equality based on content (not reference) -----------------------------------------
		#region Method: bool ContentEquals(a,b) - must be overridden for non SortKey comparisons
		public virtual bool ContentEquals(JObject obj)
		{
			if (this.GetType() != obj.GetType())
				return false;
			if (this.SortKey.Length == 0 || obj.SortKey.Length == 0)
				throw new eNoContentCompare("JObject.ContentEquals()");
			return this.SortKey == obj.SortKey;
		}
		#endregion
		#region Method: bool ContentEquals(a,b) - static version (not necessary to override)
		public static bool ContentEquals(JObject a, JObject b) 
		{
			return a.ContentEquals(b);
		}
		#endregion

		// Collection of all (non-basic) attributes ------------------------------------------
		#region Method: void AddAttribute(attribute) - append an, e.g., JOwnSeq to the list
		public void AddAttribute(JAttr attribute)
		{
			Debug.Assert(null != attribute);
			Debug.Assert(null != m_Attributes);

			// Make sure we don't have multiple attrs of the same name
			if( !m_bSurpressDuplicateAttrTest && null != FindAttrByName( attribute.Name ) )
				throw new eDuplicateAttrName("AddAttribute");

			m_Attributes.Add(attribute);
		}
		#endregion
		#region TestAccess

		// KLUDGE: Did this to get the OwnSeq to pass without throwing the eDuplicateAttrName.
		// This can probably be reworked to get rid of this.
		public bool m_bSurpressDuplicateAttrTest = false;   // Needed for some of the tests

		public bool _test_ContainsAttribute(object obj)
		{
			return m_Attributes.Contains(obj);
		}
		#endregion
		#region Method: JAttr FindAttrByName(sName) - returns the JAttr which has the requested name
		public JAttr FindAttrByName(string sName)
		{
			foreach( JAttr attr in AllAttrs )
			{
				if (attr.Name == sName)
					return attr;
			}
			return null;
		}
		#endregion

		// Methods ---------------------------------------------------------------------------
		#region Constructor()
		public JObject()
		{
			// The object, when first created, has no owner.
			Owner = null;
		}
		#endregion
		#region Method: Clear() - Removes all objects down the hierarchy
		public void Clear()
			// Removes all objects down the hierarchy, so that the garbage collector has a less
			// ambigious time of figuring out what it can dispose of. (To be honest, I've no idea
			// if this is really needed; but it only costs some processor time.)
			//   The result is that the JObject's attributes still exist (e.g., the JOwnSeq is not
			// destroyed), but it no longer has any contents.
		{
			foreach (JRef r in AllJRefAttrs)
				r.Clear();

			foreach(JOwn own in AllJOwnAttrs)
			{
				own.Clear();
			}

			foreach (JOwnSeq os in AllJOwnSeqAttrs)
			{
				foreach (JObject obj in os)
					obj.Clear();
				os.Clear();
			}

			DeclareDirty();
		}
		#endregion
		protected const int TAttrBase = 20; // Temporary basic attrs within subclasses
		protected const int BAttrBase = 30; // Persisted basic attrs within subclasses

		// I/O: BAttrs -----------------------------------------------------------------------
		private _LoopAttrsMethod m_LoopMethod = null;  // Active loop method
		#region EMBEDDED CLASS: _LoopAttrsMethod - virtual save/read superclass
		private class _LoopAttrsMethod
		{
			public _LoopAttrsMethod() {}

			public virtual void DefineAttr(string sName, ref int n)            {}
			public virtual void DefineAttr(string sName, ref double dbl)       {}
			public virtual void DefineAttr(string sName, ref bool b)           {}
			public virtual void DefineAttr(string sName, ref string s)         {}
			public virtual void DefineAttr(string sName, ref char ch)          {}
            public virtual void DefineAttr(string sName, ref DateTime dt)      {}
			public virtual void DefineAttr(string sName, ref BStringArray bsa) {}
			public virtual void DefineAttr(string sName, ref BIntArray bna)    {}

			public virtual void DeclareBAttr(int tag, string sName, Type type) {}
			public virtual void Finish() {}
		}
		#endregion
		#region EMBEDDED CLASS: _SaveBasicAttrsMethod - for saving
		private class _SaveBasicAttrsMethod : _LoopAttrsMethod
		{
			string m_sSaveLine = "";         // We build this line one attr at a time, then save it
			string m_sInsertPadding = "";    // Leading blank spaces
			TextWriter m_tw = null;          // The stream we're saving to
			#region Constructor(...)
			public _SaveBasicAttrsMethod(string sOpeningTag, 
				TextWriter tw, string sIndentPadding)
				: base()
			{
				m_sSaveLine = sOpeningTag;
				m_sInsertPadding = sIndentPadding;
				m_tw = tw;
			}
			#endregion

			#region Method: void AppendField(string sName, string sField)
			private void AppendField(string sName, string sField)
			{
				m_sSaveLine += (" " +  sName + "=\"" + sField + "\"");
			}
			#endregion
			#region Method: void DefineAttr(sName, ref int)
			public override void DefineAttr(string sName, ref int nValue)
			{
				AppendField(sName, nValue.ToString());
			}
			#endregion
			#region Method: void DefineAttr(sName, ref double)
			public override void DefineAttr(string sName, ref double dblValue)
			{
				// Store as an integer, so as to avoid localization problems
				// with doubles.
				int nValue = (int)(dblValue * 1000000.0);
				AppendField(sName, nValue.ToString());
			}
			#endregion
			#region Method: void DefineAttr(sName, ref bool)
			public override void DefineAttr(string sName, ref bool bValue)
			{
				AppendField(sName, (bValue ? "true" : "false"));
			}
			#endregion
			#region Method: void DefineAttr(sName, ref string)
			public override void DefineAttr(string sName, ref string sValue)
			{
				AppendField(sName, sValue);
			}
			#endregion
			#region Method: void DefineAttr(sName, ref char)
			public override void DefineAttr(string sName, ref char chValue)
			{
				AppendField(sName, chValue.ToString());
			}
			#endregion
            #region Method: void DefineAttr(sName, ref DateTime)
            public override void DefineAttr(string sName, ref DateTime dt)
            {
                string s = dt.ToString("u", DateTimeFormatInfo.InvariantInfo);
                //Console.WriteLine("Out Date = <" + s + ">");
                AppendField(sName, s);
            }
            #endregion
            #region Method: void DefineAttr(sName, ref BStringArray)
            public override void DefineAttr(string sName, ref BStringArray bsaValue)
			{
				AppendField(sName, bsaValue.SaveLine);
			}
			#endregion
			#region Method: void DefineAttr(sName, ref BIntArray)
			public override void DefineAttr(string sName, ref BIntArray bnaValue)
			{
				AppendField(sName, bnaValue.SaveLine);
			}
			#endregion
			#region Method: void Finish() - writes the string to disk
			public override void Finish()
			{
				m_sSaveLine += ">";
				m_tw.WriteLine(m_sInsertPadding + m_sSaveLine);
			}
			#endregion
		}
		#endregion
		#region EMBEDDED CLASS: _ReadBasicAttrsMethod - for reading
		private class _ReadBasicAttrsMethod : _LoopAttrsMethod
		{
			private JObject m_obj = null;
			private string m_sReadName  = "";
			private string m_sReadValue = "";
			#region Constructor...)
			public _ReadBasicAttrsMethod(JObject obj)
				: base()
			{
				m_obj = obj;
			}
			#endregion
			#region Method: void ParseLine(sLine) - creates Name:Value pairs
			public void ParseLine(string sLine)
			{
				int i=0;

				// Move past the signature
				sLine = sLine.Trim();
				if (sLine.Length > 0 && sLine[i] == '<')
				{
					while( sLine.Length > i && sLine[i] != ' ')
						i++;
				}

				// Move past any spaces
				while (sLine.Length > i && sLine[i] == ' ')
					i++;
				if (sLine.Length == i)
					return;

				// Retrieve the name
				string sAttrName = "";
				while (sLine.Length > i && sLine[i] != ' ' && sLine[i] != '=')
				{
					sAttrName += sLine[i];
					i++;
				}

				// Move to the value
				while (sLine.Length > i && sLine[i] != '"')
					i++;
				if (sLine.Length > i && sLine[i] == '"')
					i++;

				// Retrieve the value
				string sValue = "";
				while (sLine.Length > i && sLine[i] != '"')
				{
					sValue += sLine[i];
					i++;
				}

				// Move past the value and any trailing spaces
				if (sLine.Length > i && sLine[i] == '"')
					i++;
				while (sLine.Length > i && sLine[i] == ' ')
					i++;
				if (sLine.Length > i && sLine[i] == '>')
					i++;
				while (sLine.Length > i && sLine[i] == ' ')
					i++;

				// Find the attr, and set its value
				m_sReadName = sAttrName;
				m_sReadValue = sValue;
				m_obj.DeclareAttrs();

				// Recurse if there is still anything in the string (besides "/>")
				if (sLine.Length > 0)
					ParseLine( sLine.Substring(i) );
			}
			#endregion

			#region Method: void DefineAttr(sName, ref int)
			public override void DefineAttr(string sName, ref int nValue)
			{
				if (m_sReadName == sName)
					nValue = Convert.ToInt16(m_sReadValue);
			}
			#endregion
			#region Method: void DefineAttr(sName, ref double)
			public override void DefineAttr(string sName, ref double dblValue)
			{
				if (m_sReadName == sName)
				{
					// Stored as an integer, so as to avoid localization problems
					// with doubles.
					int nValue = Convert.ToInt32(m_sReadValue);
					dblValue = ((double)nValue) / 1000000;
				}
			}
			#endregion
			#region Method: void DefineAttr(sName, ref bool)
			public override void DefineAttr(string sName, ref bool bValue)
			{
				if (m_sReadName == sName)
				{
					bValue = (m_sReadValue == "true") ? true : false;
				}
			}
			#endregion
			#region Method: void DefineAttr(sName, ref string)
			public override void DefineAttr(string sName, ref string sValue)
			{
				if (m_sReadName == sName)
					sValue = m_sReadValue;
			}
			#endregion
			#region Method: void DefineAttr(sName, ref char)
			public override void DefineAttr(string sName, ref char chValue)
			{
				if (m_sReadName == sName)
				{
					if (0 < m_sReadValue.Length)
						chValue = m_sReadValue[0];
				}
			}
			#endregion
            #region Method: void DefineAttr(sName, ref datetime)
            public override void DefineAttr(string sName, ref DateTime dt)
            {
                if (m_sReadName == sName)
                {
                    try
                    {
                        dt = DateTime.ParseExact(m_sReadValue, "u", DateTimeFormatInfo.InvariantInfo);
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            #endregion
            #region Method: void DefineAttr(sName, ref BStringArray)
            public override void DefineAttr(string sName, ref BStringArray bsaValue)
			{
				if (m_sReadName == sName)
					bsaValue.Read(m_sReadValue);
			}
			#endregion
			#region Method: void DefineAttr(sName, ref BIntArray)
			public override void DefineAttr(string sName, ref BIntArray bnaValue)
			{
				if (m_sReadName == sName)
					bnaValue.Read(m_sReadValue);
			}
			#endregion
			#region Method: int[]  ReadToIntArray(string sValue)
			protected int[] ReadToIntArray(string s)
			{
				int i = 0;
				string sNumber = "";

				// Retrieve the first number, which is the size of the array
				while (i<s.Length && s[i] != ' ')
				{
					sNumber += s[i];
					i++;
				}
				while (i<s.Length && s[i] == ' ')
					i++;
				int nArraySize = Convert.ToInt32(sNumber);
				int[] rg  = new int[nArraySize];

				// Retrieve the rest of the numbers
				int iPos = 0;
				while (i<s.Length)
				{
					// Reset to an empty string
					sNumber = "";

					// Extract the next number
					while (i<s.Length && s[i] != ' ')
					{
						sNumber += s[i];
						i++;
					}
					while (i<s.Length && s[i] == ' ')
						i++;

					// Place it in the array
					rg[iPos] = Convert.ToInt32(sNumber);
					iPos++;
				}

				return rg;
			}
			#endregion

		}
		#endregion
		#region Method: void SaveBasicAttrs(TextWriter tw, int nIndent)
		protected void SaveBasicAttrs(TextWriter tw, int nIndent)
		{
			m_LoopMethod = new _SaveBasicAttrsMethod(XmlBegin, 
				tw, IndentPadding(nIndent));
			DeclareAttrs();
			m_LoopMethod.Finish();
			m_LoopMethod = null;
		}
		#endregion
		#region Method: void ReadBasicAttrs(sLine)
		protected void ReadBasicAttrs(string sLine)
		{
			m_LoopMethod = new _ReadBasicAttrsMethod(this);
			(m_LoopMethod as _ReadBasicAttrsMethod).ParseLine(sLine);
			m_LoopMethod = null;
		}
		#endregion
		#region Method: void DeclareBAttr(tag, sName, type) -  called by sub's to register their attrs
		protected void DeclareBAttr(int tag, string sName, Type type)
		{
			Debug.Assert(null != m_LoopMethod);
			m_LoopMethod.DeclareBAttr(tag, sName, type);
		}
		#endregion

		#region Methods: void DefineAttr(sName, ref "int, double, bool, string, DateTime, bsa, bna");
		protected void DefineAttr(string sName, ref int nValue)
		{
			Debug.Assert(null != m_LoopMethod);
			m_LoopMethod.DefineAttr(sName, ref nValue);
		}
		protected void DefineAttr(string sName, ref double dblValue)
		{
			Debug.Assert(null != m_LoopMethod);
			m_LoopMethod.DefineAttr(sName, ref dblValue);
		}
		protected void DefineAttr(string sName, ref bool bValue)
		{
			Debug.Assert(null != m_LoopMethod);
			m_LoopMethod.DefineAttr(sName, ref bValue);
		}
		protected void DefineAttr(string sName, ref string sValue)
		{
			Debug.Assert(null != m_LoopMethod);
			m_LoopMethod.DefineAttr(sName, ref sValue);
		}
		protected void DefineAttr(string sName, ref char chValue)
		{
			Debug.Assert(null != m_LoopMethod);
			m_LoopMethod.DefineAttr(sName, ref chValue);
		}
        protected void DefineAttr(string sName, ref DateTime dtValue)
        {
            Debug.Assert(null != m_LoopMethod);
            m_LoopMethod.DefineAttr(sName, ref dtValue);
        }
        protected void DefineAttr(string sName, ref BStringArray bsa)
		{
			Debug.Assert(null != m_LoopMethod);
			m_LoopMethod.DefineAttr(sName, ref bsa);
		}
		protected void DefineAttr(string sName, ref BIntArray bna)
		{
			Debug.Assert(null != m_LoopMethod);
			m_LoopMethod.DefineAttr(sName, ref bna);
		}
		#endregion

		#region Method: virtual void DeclareAttrs() - sub's must override to register their attrs
		protected virtual void DeclareAttrs()
		{
		}
		#endregion

		// I/O -------------------------------------------------------------------------------
		public string XmlEndTag { get { return "</" + GetType().Name + ">"; } }
		public string XmlBegin  { get { return "<"  + GetType().Name; } }
		enum BAttrs { bSignature = 1 };
		#region Method: string IndentPadding(nIndent) - spaces for indentation
		public string IndentPadding(int nIndent)
		{
			return new string(' ', nIndent * 2);
		}
		#endregion
		#region Method: void Write(TextWriter, nIndent) - recursive workhorse method for the write operation
		public virtual void Write(TextWriter tw, int nIndent)
			// Generic method to write out an object. Should suffice for most objects, unless
			// something special is desired.
		{
			SaveBasicAttrs(tw, nIndent);

			// Now the major attributes
			foreach (JAttr attr in m_Attributes)
			{
				attr.Write(tw, nIndent + 1);
			}

			// End Tag
			tw.WriteLine( IndentPadding(nIndent) + XmlEndTag );
		}
		#endregion
		#region Method: void Read(sFirstLine, TextReader) - recursive workhouse method for the read operation
		public virtual void Read(string sLine, TextReader tr)
			// Subclasses may want to override this to do their own processing. These overridden
			// methods should first call this base.Read, then perform their processing. See
			// the override on JCharFont as an example.
		{
            // If this is an JObjectOnDemand that is not the root of the hierarchy, the BasicAttrs are
            // read in when the owning object does its read. We do NOT want to read them in when we now
            // load the data file, because we will potentially overwrite (and thus change) their values
            // on the read in. This is especially problematic for AbsolutePathName, where e.g., if a file
            // has been moved it will be found via the RelativePathName mechanism; but then if overridden,
            // the file cannot be found the next time around.
            if (IsRoot || null != this as JObjectOnDemand)
                ReadBasicAttrs(sLine);

			// Loop to read the content attributes
			while ( (sLine = tr.ReadLine()) != null)
			{
				sLine = sLine.Trim();
				if (sLine == XmlEndTag)
					break;

				foreach( JAttr attr in m_Attributes )
				{
					// Test to see if there is a match with the attribute, read if so
					if (attr.IsOpeningXmlTagLine(sLine))
					{
						attr.Read(sLine, tr);
						break;
					}
				}
			}
		}
		#endregion

		// I/O: Support Methods for resolving reference attributes ---------------------------
		#region Method: string GetPathFromOwningObject(objAtTop) - returns, e.g., "LexEntries-234-Senses-5"
		public string GetPathFromOwningObject(JObject objAtTop)
		{
			return _GetPathFromOwningObject(objAtTop, "");
		}
		#endregion
		#region Method: string GetPathFromRoot() - returns the save-path from the root object
		public string GetPathFromRoot()
		{
			return _GetPathFromOwningObject(RootOwner, "");
		}
		#endregion
		#region Method: void _GetPathFromOwningObject(...) - private helper method
		private string _GetPathFromOwningObject(JObject objAtTop, string sPathSoFar)
			// The path looks like 
			//     "-JOwnName" for an atomic owning attr, and
			//     "-JOwnSeqName-N" for a sequence (where N is the number as stored in the sequence)
		{
			if (IsRoot || this == objAtTop)
				return sPathSoFar;

			// Check through atomic owning attributes
			foreach (JOwn own in Owner.AllJOwnAttrs)
			{
				if (own.Value == this)
				{
					sPathSoFar = "-" + own.Name + sPathSoFar;
					return Owner._GetPathFromOwningObject(objAtTop, sPathSoFar);
				}
			}

			// If not found, check through owning sequence attributes
			foreach (JOwnSeq os in Owner.AllJOwnSeqAttrs)
			{
				int iPos = os.FindObj(this);
				if (-1 != iPos)
				{
					sPathSoFar = "-" + os.Name + "-" + iPos.ToString() + sPathSoFar;
					return Owner._GetPathFromOwningObject(objAtTop, sPathSoFar);
				}
			}

			// Should have been found by now
			Debug.Assert(false);

			return sPathSoFar;
		}
		#endregion
		#region Method: JObject GetObjectFromPath(sPath) - returns JObject described by path
		public JObject GetObjectFromPath(string sPath)
			// Given a path of the form returned from _GetPathFromOwningObject, returns the
			// object at the end of that path. Thus these two methods much work in tandem.
		{
			// If there is nothing in the path, then return "this"
			if (sPath.Length == 0)
				return this;

			// The first element of the path is the attr, in the form of "-AttrName-"
			string sAttrName = "";
			int i=0;
			if(sPath.Length > i && sPath[i]=='-')
				i++;
			while(sPath.Length > i && sPath[i]!='-')
			{
				sAttrName += sPath[i];
				i++;
			}

			// Find the attribute
			JAttr attr = FindAttrByName(sAttrName);
			if (null == attr)
				return null;

			// If it is a JOwn, point to it's object (or call recursively on that object)
			if (attr.GetType() == typeof(JOwn))
			{
				return (sPath.Length > i) ? ((JOwn)attr).Value.GetObjectFromPath(sPath.Substring(i)) : ((JOwn)attr).Value;
			}

			// Otherwise, it is a JOwnSeq. 
			if (attr.GetType() != typeof(JOwnSeq))
				return null;

			// Retrieve the next element in the path: it is the index of the object within the attribute
			string sIndex = "";
			if(sPath.Length > i && sPath[i]=='-')
				i++;
			while(sPath.Length > i && sPath[i]!='-')
			{
				sIndex += sPath[i];
				i++;
			}
			int iIndex = Convert.ToInt16(sIndex);

			// point to the indexed object (or call recursively on that object)
			JObject obj = ((JOwnSeq)attr)[iIndex];
			return (sPath.Length > i) ? obj.GetObjectFromPath(sPath.Substring(i)) : obj;
		}
		#endregion
		#region Method: void ResolveReferences() - at end of Read operation, set the Reference pointers
		public void ResolveReferences()
		{
			foreach (JRef r in AllJRefAttrs)
				r.ResolveReference();

			foreach (JOwnSeq os in this.AllJOwnSeqAttrs)
			{
				foreach (JObject obj in os)
					obj.ResolveReferences();
			}

			foreach (JOwn own in this.AllJOwnAttrs)
			{
				if (null != own.Value) // LOD will not have a value yet
					own.Value.ResolveReferences();
			}
		}
		#endregion
	}



}
