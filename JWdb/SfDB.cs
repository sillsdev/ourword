/**********************************************************************************************
 * Project: JWdb
 * File:    SfDB.cs
 * Author:  John Wimbish
 * Created: 25 Nov 2004
 * Purpose: A standard format database
 * Legal:   Copyright (c) 2005-07, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
#region Using
using System;
using System.Diagnostics;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.IO;
using JWTools;
using NUnit.Framework;
#endregion

namespace JWdb
{
	#region Exception: ReadFailedException
	public class ReadFailedException : Exception
	{
	}
	#endregion

    public class SfField
    {
        // Content Attrs ---------------------------------------------------------------------
        #region Attr{g}: string Mkr - without the backslash, e.g., "s" or "rcrd"
        public string Mkr
        {
            get
            {
                return m_sMkr;
            }
            set
            {
                m_sMkr = value;
            }
        }
        string m_sMkr;
        #endregion
        #region Attr{g}: string Data - the field's data
        public string Data
        {
            get
            {
                return m_sData;
            }
            set
            {
                m_sData = value;
            }
        }
        string m_sData;
        #endregion
        #region Attr{g}: string LineNo - line number of the field in the read file (if any)
        public int LineNo
        {
            get
            {
                return m_nLineNo;
            }
            set
            {
                m_nLineNo = value;
            }
        }
        int m_nLineNo = 0;
        #endregion

        #region Attr{g}: string BT - the back translation of the field's data
        public string BT
        {
            get
            {
                return m_sBT;
            }
            set
            {
                m_sBT = value;
            }
        }
        string m_sBT = "";
        #endregion
        #region Attr{g}: string IBT - the interlinear BT of the field's data
        public string IBT
        {
            get
            {
                return m_sIBT;
            }
            set
            {
                m_sIBT = value;
            }
        }
        string m_sIBT = "";
        #endregion

        // Scaffolding -----------------------------------------------------------------------
        #region Constructor(sMkr)
        public SfField(string sMkr)
            : this(sMkr, "", 0)
        {
        }
        #endregion
        #region Constructor(sMkr, sData)
        public SfField(string sMkr, string sData)
            : this(sMkr, sData, 0)
        {
        }
        #endregion
        #region Constructor(sMkr, sData, nLineNo)
        public SfField(string sMkr, string sData, int nLineNo)
        {
            m_sMkr = sMkr;
            m_sData = sData;
            m_nLineNo = nLineNo;
        }
        #endregion

        #region Constructor(sMkr, sData, sProseBT, sIntBT)
        public SfField(string sMkr, string sData, string _sBT, string _sIntBT)
            : this(sMkr, sData, 0)
        {
            m_sBT = _sBT;
            m_sIBT = _sIntBT;
        }
        #endregion

        #region Method: bool ContentEquals(SfField field)
        public bool ContentEquals(SfField field)
        {
            if (null == field)
                return false;

            if (Mkr != field.Mkr)
                return false;

            if (Data != field.Data)
                return false;

            if (BT != field.BT)
                return false;

            if (IBT != field.IBT)
                return false;

            return true;
        }
        #endregion

        // Public Methods --------------------------------------------------------------------
        #region Method: void Write(SfWrite)
        public void Write(SfWrite W)
        {
            W.Write(Mkr, Data);
        }
        #endregion
    }

	public class SfDb
	{
		// Attrs -----------------------------------------------------------------------------
		#region Attr{g}: protected ArrayList Fields - No outside access permitted
		protected ArrayList Fields
		{
			get
			{
				Debug.Assert(null != m_vFields);
				return m_vFields;
			}
		}
		private ArrayList m_vFields = null;
		#endregion
        #region Attr{g}: string RecordMkr
        public string RecordMkr
        {
            get
            {
                Debug.Assert(0 != m_sRecordMkr.Length);
                return m_sRecordMkr;
            }
        }
        string m_sRecordMkr;
        #endregion

		// Derived Attrs ---------------------------------------------------------------------
		#region Attr{g}: int Count - the number of elements in the Fields arraylist
		public int Count
		{
			get
			{
				return Fields.Count;
			}
		}
		#endregion

		// Scaffolding -----------------------------------------------------------------------
		#region Constructor()
		public SfDb()
		{
            m_sRecordMkr = "rcrd";
			m_vFields = new ArrayList();
		}
		#endregion
		#region Method: bool ContentEquals(SfDb db)
		public bool ContentEquals(SfDb db)
		{
			if (db.Count != Count)
				return false;

			for(int i=0; i<Count; i++)
			{
				SfField f1 = Fields[i] as SfField;
				SfField f2 = db.Fields[i] as SfField;

				if (null == f1 || null == f2)
					return false;

				if (false == f1.ContentEquals(f2))
					return false;
			}

			return true;
		}
		#endregion
		#region Virtual: void Initialize(string[] vData) - populate from an array of strings
		public virtual void Initialize(string[] vData)
		{
			int nLineNo = 1;
			foreach(string sLine in vData)
			{
				string sData   = "";

				if (sLine.Length == 0 || sLine[0] != '\\')
					continue;
				int i = 1;

				// Extract the marker
				string sMarker = "";
				while( i < sLine.Length && sLine[i] != ' ')
					sMarker += sLine[i++];
				while( i < sLine.Length && sLine[i] == ' ')
					i++;

				// Extract the data
				sData = sLine.Substring(i);

				// Place it into the DB
				Append( new SfField(sMarker, sData, nLineNo++) );
			}
		}
		#endregion
		#region Virtual: bool Read(ref string sPathName) - populate from StFmt disk file
		public virtual bool Read(ref string sPathName)
		{
			// Open the file for reading (if the user has to browse, then sPathName
			// will get changed via the call to the SfRead constructor
			SfRead read = null;
			try
			{
				string sFileFilter = 
					"Shoebox Database File (*.db)|*.db|" + 
					"All files (*.*)|*.*";
				read = new SfRead(ref sPathName, sFileFilter);
			}
			catch (Exception)
			{
				// The SfRead has already presented the user with the option of
				// browsing to locate the file. So if we get here, then we have
				// failed with nothing we can do.
				return false;
			}

			// Read in all of the fields
			try
			{
				while (read.ReadNextField())
				{
					Append( new SfField(read.Marker, read.Data, read.LineNumber)  );
				}
			}
			catch (ReadFailedException)  // Exceptions we've already told the user about
			{
				read.Close();
				return false;
			}

			// We're done with reading
			read.Close();
			return true;
		}
		#endregion
		#region Method: string[] ExtractData() - for testing
		public string[] ExtractData()
		{
			string[] vData = new string[Count];

			for(int i=0; i<Count; i++)
			{
				string sMkr  = GetFieldAt(i).Mkr;
				string sData = GetFieldAt(i).Data;
				vData[i] = "\\" + sMkr;
				if (sData.Length > 0)
					vData[i] += (" " + sData);
			}

			return vData;
		}
		#endregion
		#region Virtual: bool Write(string sPathName)
		string m_sMessage = "";
		public string Message
		{
			get
			{
				return m_sMessage;
			}
		}
		public virtual bool Write(string sPathName)
		{
			SfWrite W = null;

			try
			{
				W = new SfWrite(sPathName);

                foreach (SfField field in Fields)
                {
                    if (field.Mkr == RecordMkr)
                        W.WriteBlankLine();

                    field.Write(W);
                }

				W.Close();

				return true;
			}
			catch (Exception e)
			{
				// Make sure the file is closed
				try { if (null != W) W.Close(); } 
				catch (Exception) {}

				m_sMessage = e.Message;
			}

			return false;
		}
		#endregion

		// Methods: Operations on the Fields arraylist ---------------------------------------
		#region Method: SfField GetFieldAt(int i)
		public SfField GetFieldAt(int i)
		{
			Debug.Assert(i >= 0 && i < Count);
			return Fields[i] as SfField;
		}
		#endregion
        #region Method: void InsertAt(iPos, SfField)
        public void InsertAt(int iPos, SfField field)
        {
            if (null != field)
                Fields.Insert(iPos, field);
        }
        #endregion
		#region Method: void RemoveAt(i)
		protected void RemoveAt(int i)
		{
			Fields.RemoveAt(i);
		}
		#endregion
        #region Method: void Append(SfField)
        public void Append(SfField field)
		{
			if (null != field)
				Fields.Add(field);
		}
		#endregion
		#region Method: void Remove(SfField)
		protected void Remove(SfField field)
		{
			Fields.Remove(field);
		}
		#endregion
		#region Method: void MoveTo(int iFrom, int iTo)
		public void MoveTo(int iFrom, int iTo)
		{
			SfField field = GetFieldAt(iFrom);

			Fields.RemoveAt(iFrom);

			if (iFrom < iTo)
				--iTo;

			Fields.Insert(iTo, field);
		}
		#endregion

        // Methods: Process through the fields -----------------------------------------------
		int m_iCurrentField = -1;
		#region Method: SfField AdvanceToFirstRecord()
		public SfField AdvanceToFirstRecord()
		{
			Reset();
			while (null != GetNextField())
			{
				SfField field = GetCurrentField();
				if (field == null || field.Mkr == RecordMkr)
					return field;
			}
			return null;
		}
		#endregion
		#region Method: SfField GetNextField()
		public SfField GetNextField()
		{
			m_iCurrentField++;
			if (m_iCurrentField >= Count)
				return null;
			SfField field = GetFieldAt(m_iCurrentField);
			return field;
		}
		#endregion
		#region Method: SfField PeekNextField()
		public SfField PeekNextField()
		{
			if ( (m_iCurrentField + 1) >= Count)
				return null;
			SfField field = GetFieldAt(m_iCurrentField + 1);
			return field;
		}
		#endregion
		#region Method: SfField GetCurrentField()
		public SfField GetCurrentField()
		{
			if (m_iCurrentField >= Count)
				return null;
			return GetFieldAt(m_iCurrentField);
		}
		#endregion
        #region Attr{g}: bool CurrentFieldIsRecordMarker
        public bool CurrentFieldIsRecordMarker
        {
            get
            {
                SfField f = GetCurrentField();
                if (null != f && f.Mkr == RecordMkr)
                    return true;
                return false;
            }
        }
        #endregion
        #region Method: Reset()
        public void Reset()
		{
			m_iCurrentField = -1;
		}
		#endregion
    }


    #region Test_SfField
    [TestFixture] public class Test_SfField
    {
        #region TEST: Construction - all contructors propertly initialize all attrs
        [Test]
        public void Construction()
        // Test that all of the attributes are initialized properly, no matter which
        // version of the constructor is called
        {
            SfField f = new SfField("v");
            ConstructionValidation(f, "v", "", 0, "", "");

            f = new SfField("vt", "This is some verse text.");
            ConstructionValidation(f, "vt", "This is some verse text.", 0, "", "");

            f = new SfField("c", "1", 233);
            ConstructionValidation(f, "c", "1", 233, "", "");

            f = new SfField("vt", "pigi", "pergi", "ibt");
            ConstructionValidation(f, "vt", "pigi", 0, "pergi", "ibt");
        }
        private void ConstructionValidation(SfField f, string sMkr, string sData,
            int nLineNo, string sBT, string sIBT)
        {
            Assert.AreEqual(f.Mkr, sMkr);
            Assert.AreEqual(f.Data, sData);
            Assert.AreEqual(f.LineNo, nLineNo);
            Assert.AreEqual(f.BT,     sBT);
            Assert.AreEqual(f.IBT,    sIBT);
        }
        #endregion
        #region TEST: Comparison - The ContentEquals method works correctly
        [Test]
        public void Comparison()
        {
            SfField f1 = new SfField("vt", "pigi", 27);
            SfField f2 = new SfField("vt", "pigi", 27);
            Assert.IsTrue(f1.ContentEquals(f2));

            f2 = new SfField("v");
            Assert.IsFalse(f1.ContentEquals(f2));

            f1 = new SfField("vt", "pigi", "pergi", "ibt");
            f2 = new SfField("vt", "pigi", "pergi", "ibt");
            Assert.IsTrue(f1.ContentEquals(f2));
        }
        #endregion
    }
    #endregion

}
