/**********************************************************************************************
 * App:     JWdb
 * File:    JAttr.cs
 * Author:  John Wimbish
 * Created: 27 Oct 2003
 * Purpose: Implements the shared cc ontent attribute behavior.
 * Legal:   Copyright (c) 2005-07, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
#region Header: Using, etc.
using System;
using System.Collections;
using System.Diagnostics;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
#endregion

namespace JWdb
{

	public class JAttr
	{
		// Public attributes -----------------------------------------------------------------
		Type m_signature = null;
		#region Attr{g}: string Name - the attribute's name (as set in the constructor)
		public string Name
		{
			get
			{
				return m_sName;
			}
		}
		private string m_sName;                  // The name for this owning sequence
		#endregion
		#region Attr{g}: JObject Owner - the owner of this attribute
		protected JObject Owner
		{
			get { return m_objOwner; }
		}
		private JObject m_objOwner = null;       // Points to the owner
		#endregion

		// Signature control - don't allow objs of wrong type into array ---------------------
		#region Method: void CheckCorrectSignature(obj) - make sure signature is ok for insertion
		public void CheckCorrectSignature(JObject obj)
		{
			// See if the object's type, or one of its base types, matches the signature
			Type t = obj.GetType();
			while (null != t)
			{
				if (t == m_signature)
					return;
				t = t.BaseType;
			}

/****OLDWAY
			// Check for the obvious type
//			if (obj.GetType() == m_signature)
//				return;

			// TODO: Make this recursive
//			if (obj.GetType().BaseType == m_signature)
//				return;
OLDWAY ****/

			// Failing that, throw an exception
			throw new eBadSignature("JSeq:CheckCorrectSignature...");
		}
		#endregion

		// Dirty - Need to Save? -------------------------------------------------------------
		#region Method: void DeclareDirty() - mark the Save Obj as needing to be saved
		public virtual void DeclareDirty()
		{
			if (null == Owner)
				return;
			JObjectOnDemand obj = Owner.SaveObj;
			if (null != obj)
				obj.DeclareDirty();
		}
		#endregion

		// Scaffolding -----------------------------------------------------------------------
		#region Constructor(sAttrName, JObject objOwner, Type signature)
		public JAttr(string sAttrName, JObject objOwner, Type signature) 
		{
			// The name of the sequence, e.g., will be used in <name> file tag
			Debug.Assert(sAttrName != null && sAttrName.Length > 0);
			m_sName = sAttrName;

			// Keep track of the valid signature of objects we'll place into the list
			Debug.Assert(null != signature);
			m_signature = signature;

			// Keep track of the owner of this attribute
			m_objOwner = objOwner;
			Debug.Assert(m_objOwner != null);

			// Add this attribute into the parent object's list of attributes
			objOwner.AddAttribute(this);
		}
		#endregion
		#region Method: string IndentPadding(nIndent) - spaces for indentation
		public string IndentPadding(int nIndent)
		{
			return new string(' ', nIndent * 2);
		}
		#endregion

		// I/O Support -----------------------------------------------------------------------
		public string XmlBegin  { get { return "<"  + GetType().Name; } }
		#region Attribute: OpeningXmlTagLine - e.g., "<ownseq Name="Section">
		public virtual string OpeningXmlTagLine
		{
			get 
			{
				Debug.Assert(false);
				return ""; 
			}
		}
		#endregion
		#region Method: bool IsOpeningXmlTagLine(sLine) - T if sLine has the opening tag
		public bool IsOpeningXmlTagLine(string sLine)
		{
			sLine = sLine.Trim();

			int iEnd = sLine.IndexOf('>');
			if (iEnd < 0)
				return false;
			string sTest = sLine.Substring(0, iEnd + 1);

			if (OpeningXmlTagLine == sTest)
				return true;
			return false;
		}
		#endregion
		#region Method: virtual void Write(tw, nIndent) - do-nothing virtual
		public virtual void Write(TextWriter tw, int nIndent)
		{
		}
		#endregion
		#region Method: virtual void Read(sFirstLine, tr) - do-nothing virtual
		public virtual void Read(string sFirstLine, TextReader tr)
		{
		}
		#endregion
		#region JObject InvokeConstructor()
		protected JObject InvokeConstructor()
			// Locates the "Read" constructor for an object (or throws an exception if there
			// isn't one; then invokes it with the appropriate parameters. Used by the
			// Read methods on the owning attributes (atomic and sequence)
		{
			// Get the type
//			Type t = JObject.GetTypeFromSignatureID(SignatureID);
			Type t = m_signature;

			// Find the constructor
			ConstructorInfo ci = t.GetConstructor(Type.EmptyTypes);

			// Return the resultant object
			JObject obj = (JObject)ci.Invoke(new Object[0]);
			return obj;
		}
		#endregion
	}

	public class BIntArray : IEnumerator
	{
		private int[] m_rgInts;

		#region Constructor()
		public BIntArray()
		{
			m_rgInts = new int[0];
		}
		#endregion
		#region Constructor(int[] rgSource)
		public BIntArray(int[] rgSource)
		{
			m_rgInts = new int[rgSource.Length];
			for(int i=0; i<rgSource.Length; i++)
				this[i] = rgSource[i];
		}
		#endregion

		// Basic Access ----------------------------------------------------------------------
		#region Attr{g}: int Length - the number of items in the array
		public int Length
		{
			get
			{
				return m_rgInts.Length;
			}
		}
		#endregion
		#region Indexer[] - provides array access (get/set); array must already be large enough
		virtual public int this [ int index ]
		{
			get
			{
				return m_rgInts[index];
			}
			set
			{
				m_rgInts[index] = value;
			}
		}
		#endregion
		#region Method: void Append(nValue) - appends an int to the end of the list
		public void Append(int nValue)
		{
			int[] rgNew = new int[ Length + 1 ];

			for(int i=0; i < Length; i++)
				rgNew[i] = this[i];

			rgNew[ Length ] = nValue;
			m_rgInts = rgNew;
		}
		#endregion
		#region Method: void InsertAt(int iPos, int nValue)
		public void InsertAt(int iPos, int nValue)
		{
			int[] rgNew = new int[ Length + 1];

			int i = 0;
			for(i=0; i<iPos; i++)
				rgNew[i] = this[i];

			rgNew[iPos] = nValue;

			for( ; i<Length; i++)
				rgNew[i+1] = this[i];

			m_rgInts = rgNew;
		}
		#endregion
		#region Method: void Clear()
		public void Clear()
		{
			m_rgInts = new int[0];
		}
		#endregion
		#region Method: void RemoveAt(int iPos) - removes the int, closing up the space
		public void RemoveAt(int iPos)
		{
			// No point if no length or iPos out of range
			if ( Length == 0)
				return;
			if ( iPos < 0 || iPos >= Length )
				return;

			int[] rgNew = new int[ Length - 1 ];

			int i = 0;
			while (i < iPos)
			{
				rgNew[i] = this[i];
				i++;
			}
			while (i < Length - 1)
			{
				rgNew[i] = this[i+1];
				i++;
			}

			m_rgInts = rgNew;
		}
		#endregion
		#region int FindFirstPosition(int nValue) - returns -1 if not found.
		public int FindFirstPosition(int nValue)
		{
			for(int i=0; i<Length; i++)
			{
				if (this[i] == nValue)
					return i;
			}
			return -1;
		}
		#endregion

		// Enumerator ------------------------------------------------------------------------
		#region Attribute: IEnumerator.Current - Returns the current JObject
		public object Current
			// Returns the int at the current position
		{
			get 
			{ 
				if (-1 == m_iEnumeratorPos || m_iEnumeratorPos == Length || !m_bEnumeratorValid)
					throw new InvalidOperationException();
				return this[m_iEnumeratorPos];
			}
		}
		#endregion
		#region Method: void IEnumerator.Reset() - rewinds the position to before the first object
		public void Reset()
		{
			if (!m_bEnumeratorValid)
				throw new InvalidOperationException();
			m_iEnumeratorPos = -1;
		}
		#endregion
		#region Method: bool IEnumerator.MoveNext() - moves to the next object in the sequence
		public bool MoveNext()
		{
			if (!m_bEnumeratorValid)
				throw new InvalidOperationException();
			if (m_iEnumeratorPos < Length - 1)
			{
				++m_iEnumeratorPos;
				return true;
			}
			return false;
		}
		#endregion
		#region Method: IEnumerator.GetEnumerator() - initializes the enumeration
		public virtual IEnumerator GetEnumerator()
		{
			m_bEnumeratorValid = true;
			Reset();
			return this;
		}
		#endregion
		#region Private: int m_iEnumeratorPos - current position of the enumerator
		// The current position of the enumerator:
		//   - a value of -1 means the "reset" position
		//   - a value equal to Count is past the end.
		private int m_iEnumeratorPos = -1; 
		#endregion
		#region Private: bool m_bEnumeratorValid - lets us know if [] has been modified
		private bool m_bEnumeratorValid = false;
		#endregion
		#region Method: InvalidateEnumerator() - signals that the enumerator is invalid now
		protected void InvalidateEnumerator()
		{
			m_bEnumeratorValid = false;
		}
		#endregion

		// I/O -------------------------------------------------------------------------------
		#region Attr{g}: string SaveLine
		public string SaveLine
		{
			get
			{
				string sSaveLine = Length.ToString();

				foreach(int n in m_rgInts)
					sSaveLine += ( " " + n.ToString() );
			
				return sSaveLine;
			}
		}
		#endregion
		#region Method: void Read(string sReadLine)
		public void Read(string s)
		{
			int i = 0;

			// Retrieve the first number, which is the size of the array; create an
			// array to read the values into.
			string sCount = "";
			while (i<s.Length && s[i] != ' ')
			{
				sCount += s[i];
				i++;
			}
			while (i<s.Length && s[i] == ' ')
				i++;
			int cArraySize = Convert.ToInt32(sCount);
			m_rgInts = new int[cArraySize];

			// Retrieve the rest of the numbers
			int iPos = 0;
			string sValue;
			while (i < s.Length)
			{
				// Reset to an empty string
				sValue = "";

				// Extract the number
				while (i<s.Length && s[i] != ' ')
				{
					sValue += s[i];
					i++;
				}

				// Place it in the array
				int nValue = Convert.ToInt32(sValue);
				m_rgInts[iPos] = nValue;
				iPos++;

				// Read past blanks
				while (i<s.Length && s[i] == ' ')
					i++;
			}
		}
		#endregion
	}

	public class BStringArray : IEnumerator
	{
		private string[] m_rgStrings;

		#region Constructor()
		public BStringArray()
		{
			m_rgStrings = new string[0];
		}
		#endregion
		#region Constructor(string rgSource)
		public BStringArray(string[] rgSource)
		{
			m_rgStrings = new string[rgSource.Length];
			for(int i=0; i<Length; i++)
				m_rgStrings[i] = rgSource[i];
		}
		#endregion

        // Basic Access ----------------------------------------------------------------------
		#region Attr{g}: int Length - the number of items in the array
		public int Length
		{
			get
			{
				return m_rgStrings.Length;
			}
		}
		#endregion
		#region Indexer[] - provides array access (get/set); array must already be large enough
		virtual public string this [ int index ]
		{
			get
			{
				return m_rgStrings[index];
			}
			set
			{
				m_rgStrings[index] = value;
			}
		}
		#endregion
		#region Method: void Append(sValue) - appends a string to the end of the list
		public void Append(string sValue)
		{
			string[] rgNew = new string[Length + 1];

			for(int i=0; i < Length; i++)
				rgNew[i] = m_rgStrings[i];
			rgNew[ Length ] = sValue;

			m_rgStrings = rgNew;
		}
		#endregion
		#region Method: void InsertAt(int iPos, string sValue)
		public void InsertAt(int iPos, string sValue)
		{
			string[] vNew = new string[ Length + 1];

			// Copy the values over prior to the insertion point
			int i = 0;
			for(i=0; i<iPos; i++)
				vNew[i] = this[i];

			// Set the new value at the desired position
			vNew[iPos] = sValue;

			// Copy over the values following the insertion point
			for( ; i<Length; i++)
				vNew[i+1] = this[i];

			m_rgStrings = vNew;
		}
		#endregion
		#region Method: void InsertSortedIfUnique(sValue)
		public void InsertSortedIfUnique(string sValue)
			// Note: Undefined behvaior if the array is not already sorted
		{
			for(int i=0; i<Length; i++)
			{
				// If we encounter the value already in the array, then we don't
				// do anything.
				if (this[i] == sValue)
					return;

				// If we encounter something in the array that is greater than the
				// target value, then we've located the place where insertion is desired.
				if ( this[i].CompareTo(sValue) > 0)
				{
					InsertAt(i, sValue);
					return;
				}
			}

			// If we get here, then we haven't inserted, so we just append to the end.
			Append(sValue);
		}
		#endregion
		#region Method: void RemoveAt(int iPos) - removes the int, closing up the space
		void RemoveAt(int iPos)
		{
			// No point if no length or iPos out of range
			if ( Length == 0)
				return;
			if (iPos < 0 || iPos >= Length)
				return;

			string[] rgNew = new string[ Length - 1 ];

			int i = 0;
			while (i < iPos)
			{
				rgNew[i] = this[i];
				i++;
			}
			while (i < Length - 1)
			{
				rgNew[i] = this[i+1];
				i++;
			}

			m_rgStrings = rgNew;
		}
		#endregion
		#region Method: void Remove(string sValue) - Returns F if sValue was not present
		public bool Remove(string s)
		{
			for(int i=0; i<Length; i++)
			{
				if (this[i] == s)
				{
					RemoveAt(i);
					return true;
				}
			}
			return false;
		}
		#endregion
		#region Method: void Clear()
		public void Clear()
		{
			m_rgStrings = new string[0];
		}
		#endregion
		#region int FindFirstPosition(string sValue) - returns -1 if not found.
		public int FindFirstPosition(string sValue)
		{
			for(int i=0; i<Length; i++)
			{
				if (this[i] == sValue)
					return i;
			}
			return -1;
		}
		#endregion
        #region Method: string[] GetCopy()
        public string[] GetCopy()
        {
            string[] v = new string[m_rgStrings.Length];
            for (int i = 0; i < m_rgStrings.Length; i++)
                v[i] = m_rgStrings[i];
            return v;
        }
        #endregion
        #region void ReplaceAll(string[] vsNew)
        public void ReplaceAll(string[] vsNew)
        {
            m_rgStrings = new string[vsNew.Length];
            for (int i = 0; i < Length; i++)
                m_rgStrings[i] = vsNew[i];
        }
        #endregion
        #region Method: bool IsSameAs(BStringArray a)
        public bool IsSameAs(BStringArray a)
        {
            if (null == a)
                return false;
            if (a.Length != Length)
                return false;
            for (int i = 0; i < Length; i++)
            {
                if (this[i] != a[i])
                    return false;
            }
            return true;
        }
        #endregion

        // Find a match somewhere within a string --------------------------------------------
        #region Method: int FindSubstringMatch(sLongString)
        public int FindSubstringMatch(string sLongString)
		{
			return FindSubstringMatch(sLongString, 0, false);
		}
		#endregion
		#region Method: int FindSubstringMatch(sLongString, iStartPos)
		public int FindSubstringMatch(string sLongString, int iStartPos)
		{
			return FindSubstringMatch(sLongString, iStartPos, false);
		}
		#endregion
		#region Method: int FindSubstringMatch(sLongString, iStartPos, bEndAtWordBoundary)
		public int FindSubstringMatch(string sLongString, int iStartPos, bool bEndAtWordBoundary)
		{
			for(int i=0; i<Length; i++)
			{
				// Is the string long enough for the test?
				//
				// This is a test.
				// 0123456789 1234  - Length = 15
				//           ^
				// "test." length = 5
				// If iPos = 10, then iPos + test.length = 15.
				if (iStartPos + this[i].Length <= sLongString.Length)
				{
					// Do we have a match with one of our strings?
					if (this[i] == sLongString.Substring(iStartPos, this[i].Length) )
					{
						if (bEndAtWordBoundary)
						{
							// If we're at the end of the long string, then we succeeed.
							if (iStartPos + this[i].Length == sLongString.Length)
								return i;
							// Otherwise, the long string exceeds the length, and we can
							// examine the next character to see if it is a word boundary.
							char ch = sLongString[iStartPos + this[i].Length ];
							if (!Char.IsLetter(ch))
								return i;
						}
						else
							return i;
					}
				}
			}
			return -1;
		}
		#endregion

		// Enumerator ------------------------------------------------------------------------
		#region Attribute: IEnumerator.Current - Returns the current string
		public object Current
			// Returns the int at the current position
		{
			get 
			{ 
				if (-1 == m_iEnumeratorPos || m_iEnumeratorPos == Length || !m_bEnumeratorValid)
					throw new InvalidOperationException();
				return this[m_iEnumeratorPos];
			}
		}
		#endregion
		#region Method: void IEnumerator.Reset() - rewinds the position to before the first object
		public void Reset()
		{
			if (!m_bEnumeratorValid)
				throw new InvalidOperationException();
			m_iEnumeratorPos = -1;
		}
		#endregion
		#region Method: bool IEnumerator.MoveNext() - moves to the next object in the sequence
		public bool MoveNext()
		{
			if (!m_bEnumeratorValid)
				throw new InvalidOperationException();
			if (m_iEnumeratorPos < Length - 1)
			{
				++m_iEnumeratorPos;
				return true;
			}
			return false;
		}
		#endregion
		#region Method: IEnumerator.GetEnumerator() - initializes the enumeration
		public virtual IEnumerator GetEnumerator()
		{
			m_bEnumeratorValid = true;
			Reset();
			return this;
		}
		#endregion
		#region Private: int m_iEnumeratorPos - current position of the enumerator
		// The current position of the enumerator:
		//   - a value of -1 means the "reset" position
		//   - a value equal to Count is past the end.
		private int m_iEnumeratorPos = -1; 
		#endregion
		#region Private: bool m_bEnumeratorValid - lets us know if [] has been modified
		private bool m_bEnumeratorValid = false;
		#endregion
		#region Method: InvalidateEnumerator() - signals that the enumerator is invalid now
		protected void InvalidateEnumerator()
		{
			m_bEnumeratorValid = false;
		}
		#endregion

		// I/O -------------------------------------------------------------------------------
		#region Attr{g/s}: char SaveDelimiterI
		public char SaveDelimiterI
		{
			get
			{
				return m_chSaveDelimiterI;
			}
			set
			{
				m_chSaveDelimiterI = value;
			}
		}
		char m_chSaveDelimiterI = '{';
		#endregion
		#region Attr{g/s}: char SaveDelimiterF
		public char SaveDelimiterF
		{
			get
			{
				return m_chSaveDelimiterF;
			}
			set
			{
				m_chSaveDelimiterF = value;
			}
		}
		char m_chSaveDelimiterF = '}';
		#endregion
		#region Attr{g}: string SaveLine
		public string SaveLine
		{
			get
			{
				string sSaveLine = Length.ToString();

				foreach(string s in m_rgStrings)
					sSaveLine += ( " " + SaveDelimiterI + s + SaveDelimiterF );
			
				return sSaveLine;
			}
		}
		#endregion
		#region Method: void Read(string sReadLine)
		public void Read(string s)
		{
			int i = 0;

			// Retrieve the first number, which is the size of the array; create an
			// array to read the values into.
			string sCount = "";
			while (i<s.Length && s[i] != ' ')
			{
				sCount += s[i];
				i++;
			}
			while (i<s.Length && s[i] == ' ')
				i++;
			int cArraySize = Convert.ToInt32(sCount);
			string[] rg  = new string[cArraySize];

			// Retrieve the rest of the strings
			int iPos = 0;
			string sValue;
			while (i < s.Length)
			{
				// Reset to an empty string
				sValue = "";

				// Read to the begining marker
				while(i<s.Length && s[i] != SaveDelimiterI)
					i++;
				if(i<s.Length && s[i] == SaveDelimiterI)
					i++;

				// Extract the string
				while(i<s.Length && s[i] != SaveDelimiterF)
				{
					sValue += s[i];
					i++;
				}

				// Place it in the array
				if(i<s.Length && s[i] == SaveDelimiterF)
				{
					rg[iPos] = sValue;
					iPos++;
					i++;
				}
			}

			m_rgStrings = rg;
		}
		#endregion
	}
}
