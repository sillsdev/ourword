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
using System.Text;
using JWTools;
#endregion

namespace JWdb
{
	#region Exception: ReadFailedException
	public class ReadFailedException : Exception
	{
	}
	#endregion

    #region CLASS: SfField
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
        #region Method: void Write(SfWrite, bWrapLines)
        public void Write(SfWrite W, bool bWrapLines)
        {
            W.Write(Mkr, Data, bWrapLines);
        }
        #endregion
    }
    #endregion

    #region CLASS: SfDb
    public class SfDb
	{
		// Attrs -----------------------------------------------------------------------------
		#region Attr{g}: protected ArrayList Fields - No outside access permitted
		public ArrayList Fields
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
		#region Virtual: bool Read(TextReader) - populate from StFmt disk file
		public virtual bool Read(TextReader tr)
		{
			SfRead read = new SfRead(tr);

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
		#region Method: bool Write(sPathName, bWrapLines)
		string m_sMessage = "";
		public string Message
		{
			get
			{
				return m_sMessage;
			}
		}
		public bool Write(string sPathName, bool bWrapLines)
		{
			SfWrite W = null;

			try
			{
				W = new SfWrite(sPathName);

                foreach (SfField field in Fields)
                {
                    if (field.Mkr == RecordMkr)
                        W.WriteBlankLine();

                    field.Write(W, bWrapLines);
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
        #region Method: SfField InsertAt(iPos, SfField)
        public SfField InsertAt(int iPos, SfField field)
        {
            if (null != field)
                Fields.Insert(iPos, field);
            return field;
        }
        #endregion

        public void InsertBefore(SfField fNew, SfField fTarget)
        {
            for (int i = 0; i < Count; i++)
            {
                if (Fields[i] == fTarget)
                {
                    InsertAt(i, fNew);
                    return;
                }
            }
        }

        #region Method: void RemoveAt(i)
        public void RemoveAt(int i)
		{
			Fields.RemoveAt(i);
		}
		#endregion
        #region Method: SfField Append(SfField)
        public SfField Append(SfField field)
		{
			if (null != field)
				Fields.Add(field);
            return field;
		}
		#endregion
		#region Method: void Remove(SfField)
		public void Remove(SfField field)
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
    #endregion
}
