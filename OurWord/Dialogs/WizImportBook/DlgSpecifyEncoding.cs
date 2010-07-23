/**********************************************************************************************
 * Project: Our Word!
 * File:    DlgSpecifyEncoding.cs
 * Author:  John Wimbish
 * Created: 14 May 2004
 * Purpose: Allows an import file's encoding to be specified (and thus converted to UTF8)
 * Legal:   Copyright (c) 2005-10, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
#region Using
using System;
using System.Diagnostics;
using System.Drawing;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Text;
using System.IO;
using JWTools;
#endregion

namespace OurWord.Dialogs.WizImportBook
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
        readonly string m_sPathName;
        #endregion
        #region Attr{g} Encoding Encoding
        private Encoding Encoding { get; set; }
        #endregion
        #region VAttr{g}: List<string> FileLines
        public List<string> FileLines
        {
            get
            {
                Debug.Assert(null != m_vFileLines);
                return m_vFileLines;
            }
        }
        private readonly List<string> m_vFileLines;
        #endregion

        // Scaffolding -----------------------------------------------------------------------
        #region Constructor()
        public DlgSpecifyEncoding(string sPathName)
        {
            Debug.Assert(!string.IsNullOrEmpty(sPathName));
            m_sPathName = sPathName;

            m_vFileLines = new List<string>();

            Encoding = Encoding.UTF8;

            InitializeComponent();
        }
        #endregion

        // Misc Methods ----------------------------------------------------------------------
        #region smethod: string GenerateEncodingDisplayString(Encoding enc)
        private static string GenerateEncodingDisplayString(Encoding enc)
        {
            return (enc.CodePage + " - " + enc.EncodingName);
        }
        #endregion
        #region Method: void LoadFileUsingCurrentEncoding()
        void LoadFileUsingCurrentEncoding()
        {
            // Change to the wait cursor
            var cursor = Cursor.Current;
            Cursor.Current = Cursors.WaitCursor;

            // Read the file into a list of strings
            FileLines.Clear();
            var sr = new StreamReader(PathName, Encoding);
            var r = TextReader.Synchronized(sr);
            do
            {
                var s = r.ReadLine();
                if (s == null)
                    break;
                FileLines.Add(s);
            } while (true);
            r.Close();

            // IMPORTANT: We keep what we read in separate from the RTF box. For some reason,
            // we ran into a problem in Huichol data that a emdash was being converted to
            // hyphen upon szve. It is probably due to assigning the "Arial Unicode MS" font
            // to the RTF box? The Repair dialog does not have a problem saving. So if we
            // ever decide to permit editing within this dialog, we need to do everything
            // the way the RepairImport dialog does, from whatever control it uses to
            // the font it uses.

            // Place it into the RTF box
            m_rtbFile.Clear();
            m_rtbFile.Lines = FileLines.ToArray();

            // Set a large Unicode font so that it is easy to see if the letters appear
            // correctly
            m_rtbFile.Font = new Font("Arial Unicode MS", 12);

            // Restore to the previous cursor
            Cursor.Current = cursor;
        }
        #endregion

        // Command Handlers ------------------------------------------------------------------
        #region Cmd: cmdNewEncodingChosen
        private void cmdNewEncodingChosen(object sender, EventArgs e)
        {
            var vInfo = Encoding.GetEncodings();
            foreach (var info in vInfo)
            {
                if (GenerateEncodingDisplayString(info.GetEncoding()) == m_comboEncodings.Text)
                {
                    Encoding = info.GetEncoding();
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
            LocDB.Localize(this, new Control[] { m_comboEncodings });

            // Populate the list of encodings
            var vInfo = Encoding.GetEncodings();
            foreach (var info in vInfo)
                m_comboEncodings.Items.Add(GenerateEncodingDisplayString(info.GetEncoding()));

            // Select UTF8
            m_comboEncodings.Text = GenerateEncodingDisplayString(Encoding.UTF8);

            // Load the file
            LoadFileUsingCurrentEncoding();
        }
        #endregion
    }
}