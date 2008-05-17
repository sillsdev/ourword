/**********************************************************************************************
 * Project: Our Word!
 * File:    DlgSpecifyEncoding.cs
 * Author:  John Wimbish
 * Created: 14 May 2004
 * Purpose: Allows an import file's encoding to be specified (and thus converted to UTF8)
 * Legal:   Copyright (c) 2005-08, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
#region Using
using System;
using System.Diagnostics;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Text;
using System.IO;
using JWTools;
using JWdb;
using OurWord.Dialogs;
using OurWord.View;
using OurWord.Edit;
#endregion

namespace OurWord.Dialogs
{
    public partial class DlgSpecifyEncoding : Form
    {
        // Attrs -----------------------------------------------------------------------------
        #region VAttr{g}: string PathName
        string PathName
        {
            get
            {
                return m_sPathName;
            }
        }
        string m_sPathName;
        #endregion
        #region Attr{g} Encoding Encoding
        public Encoding Encoding
        {
            get
            {
                return m_Encoding;
            }
        }
        Encoding m_Encoding;
        #endregion
        #region VAttr{g}: string[] FileLines
        public string[] FileLines
        {
            get
            {
                return m_rtbFile.Lines;
            }
        }
        #endregion

        // Scaffolding -----------------------------------------------------------------------
        #region Constructor()
        public DlgSpecifyEncoding(string sPathName)
        {
            Debug.Assert(!string.IsNullOrEmpty(sPathName));
            m_sPathName = sPathName;

            m_Encoding = Encoding.UTF8;

            InitializeComponent();
        }
        #endregion
        #region Attr{g}: Control[] vExclude
        public Control[] vExclude
        {
            get
            {
                return new Control[] { m_comboEncodings };
            }
        }
        #endregion

        // Misc Methods ----------------------------------------------------------------------
        #region SMethod: string GenerateEncodingDisplayString(Encoding enc)
        static public string GenerateEncodingDisplayString(Encoding enc)
        {
            return (enc.CodePage + " - " + enc.EncodingName);
        }
        #endregion
        #region Method: void LoadFileUsingCurrentEncoding()
        void LoadFileUsingCurrentEncoding()
        {
            // Change to the wait cursor
            Cursor cursor = Cursor.Current;
            Cursor.Current = Cursors.WaitCursor;

            // Open the file and read it into an array of strings
            ArrayList a = new ArrayList();
            StreamReader sr = new StreamReader(PathName, Encoding);
            TextReader r = TextReader.Synchronized(sr);
            do
            {
                string s = r.ReadLine();
                if (s == null)
                    break;
                a.Add(s);
            } while (true);
            r.Close();

            // Convert to a vector of strings
            string[] v = new string[a.Count];
            for (int i = 0; i < a.Count; i++)
                v[i] = a[i] as string;

            // Place it into the RTF box
            m_rtbFile.Clear();
            m_rtbFile.Lines = v;

            // Set a large Unicode font so that it is easy to see if the letters appear
            // correctly
            m_rtbFile.Font = new Font("Arial Unicode MS", 16);

            // Restore to the previous cursor
            Cursor.Current = cursor;
        }
        #endregion

        // Command Handlers ------------------------------------------------------------------
        #region Cmd: cmdNewEncodingChosen
        private void cmdNewEncodingChosen(object sender, EventArgs e)
        {
            EncodingInfo[] vInfo = Encoding.GetEncodings();
            foreach (EncodingInfo Info in vInfo)
            {
                if (GenerateEncodingDisplayString(Info.GetEncoding()) == m_comboEncodings.Text)
                {
                    m_Encoding = Info.GetEncoding();
                    LoadFileUsingCurrentEncoding();
                    break;
                }
            }
        }
        #endregion
        #region Cmd: cmdLoad
        private void cmdLoad(object sender, EventArgs e)
        {
            // Label text in the appropriate language
            LocDB.Localize(this, vExclude);

            // Populate the list of encodings
            EncodingInfo[] vInfo = Encoding.GetEncodings();
            foreach (EncodingInfo Info in vInfo)
                m_comboEncodings.Items.Add(GenerateEncodingDisplayString(Info.GetEncoding()));

            // Select UTF8
            m_comboEncodings.Text = GenerateEncodingDisplayString(Encoding.UTF8);

            // Load the file
            LoadFileUsingCurrentEncoding();
        }
        #endregion
        #region Cmd: cmdResize
        int m_nPreviousWidth = -1;
        int m_nPreviousHeight = -1;
        private void cmdResize(object sender, EventArgs e)
        {
            // Handle horizontal changes 
            if (m_nPreviousWidth > 0)
            {
                int nDeltaWidth = Width - m_nPreviousWidth;

                // Most controls change their width
                m_labelInstructions.Width += nDeltaWidth;
                m_comboEncodings.Width += nDeltaWidth;
                m_labelExamineYourFile.Width += nDeltaWidth;
                m_rtbFile.Width += nDeltaWidth;

                // The buttons move in order to remain centered
                m_btnOK.Left += (nDeltaWidth / 2);
                m_btnCancel.Left += (nDeltaWidth / 2);
                m_btnHelp.Left += (nDeltaWidth / 2);
            }
            m_nPreviousWidth = Width;

            // Handle verticle changes
            if (m_nPreviousHeight > 0)
            {
                int nDeltaHeight = Height - m_nPreviousHeight;

                // The RTF control expands to show more(less) of the file
                m_rtbFile.Height += nDeltaHeight;

                // The buttons move to make room
                m_btnOK.Top += nDeltaHeight;
                m_btnCancel.Top += nDeltaHeight;
                m_btnHelp.Top += nDeltaHeight;
            }
            m_nPreviousHeight = Height;
        }
        #endregion
    }
}