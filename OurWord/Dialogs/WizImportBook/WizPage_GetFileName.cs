/**********************************************************************************************
 * Project: Our Word!
 * File:    Dialogs\WizImportBook\WizPage_GetFileName.cs
 * Author:  John Wimbish
 * Created: 1 Feb 2007
 * Purpose: First page of wizard, user enters the file name
 * Legal:   Copyright (c) 2003-09, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
#region Using
using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.IO;
using Microsoft.Win32;
using JWTools;
using JWdb;
using JWdb.DataModel;
#endregion

namespace OurWord.Dialogs.WizImportBook
{
    public partial class WizPage_GetFileName : UserControl, IJW_WizPage
    {
        // Attrs -----------------------------------------------------------------------------
        #region Attr{g}: string PathName
        public string PathName
        {
            get
            {
                Debug.Assert(null != m_sPathName);
                return m_sPathName;
            }
        }
        string m_sPathName = "";
        #endregion
		#region VAttr{g}: WizImportBook Wizard - the owning wizard
		WizImportBook Wizard
        {
            get
            {
				Debug.Assert(null != Parent as WizImportBook);
				return Parent as WizImportBook;
            }
        }
        #endregion
        #region Attr{g}: string Format
        public string Format
        {
            get
            {
                return m_sFormat;
            }
        }
        string m_sFormat = "";
        #endregion

        // Scaffolding -----------------------------------------------------------------------
        #region Constructor()
        public WizPage_GetFileName()
        {
            InitializeComponent();
            m_btnSpecifyEncoding.Visible = false;
        }
        #endregion

        // IJW_WizPage Implementation --------------------------------------------------------
        #region Method: void OnActivate()
        public void OnActivate()
            // Place the Focus on the Browse button or the Next button, depending on 
            // which type of input is appropriate from the user.
        {
            if (CanGoToNextPage())
                Wizard.PlaceFocusOnAdvanceButton();
            else
                m_btnBrowse.Focus();
        }
        #endregion
        #region Method: bool CanGoToNextPage()
        public bool CanGoToNextPage()
            // The ExamineFile method is used to set this.
        {
            return m_bCanGoToNextPage;
        }
        bool m_bCanGoToNextPage = false;
        #endregion
        #region Method: string PageNavigationTitle()
        public string PageNavigationTitle()
        {
            return LocDB.GetValue(this, "strNavTitle", "Get the Filename", null);
        }
        #endregion
        #region Method: void ShowHelp()
        public void ShowHelp()
        {
            HelpSystem.ShowTopic(HelpSystem.Topic.kImportBook);
        }
        #endregion

        // Misc Methods ----------------------------------------------------------------------
        #region bool ExamineFile(string sPathName) - see if the proposed file is suitable
        bool ExamineFile(string sPathName)
        {
            // Our default will be that the file does not pass muster.
            m_bCanGoToNextPage = false;
            m_sFormat = "";

            // Turn off the Specify Encoding button; we'll turn it on below if it turns
            // out that we need it.
            m_btnSpecifyEncoding.Visible = false;
            m_labelFileStats.TextAlign = ContentAlignment.TopLeft;

            // Do we have a half-way reasonable pathname?
            if (null == sPathName || sPathName.Length == 0)
                return false;

            // Display the message that we are examining the file. Use an attention-getting
            // font and Wait Cursor so the user will be patient.
            m_labelFileStats.Visible = true;
            m_labelFileStats.Text = LocDB.GetValue(this, "strPleaseWait",
                "Please wait while OurWord examines this file...", null);
            m_labelFileStats.ForeColor = Color.Red;
            m_labelFileStats.Font = new Font(
                m_labelFileStats.Font, FontStyle.Bold);

            // Get the file's encoding from the BOM
            Encoding enc = JW_Util.GetUnicodeFileEncoding(sPathName);

            // A null encoding means that we couldn't access the file
            if (enc == null)
            {
                string sFileName = Path.GetFileName(sPathName);
                m_labelFileStats.Text = LocDB.GetValue(this, "strUnableToAccessFile",
                    "Unable to access file '{0}'.", new string[] { sFileName });
                return false;
            }

            // Anything other than UTF8 is a problem
            if (enc != Encoding.UTF8)
            {
                string sFileName = Path.GetFileName(sPathName);
                m_labelFileStats.Text = LocDB.GetValue(this, "strNotUTF8",
                    "File '{0}' does not appear to be in the UTF-8 Encoding. " +
                    "As an Advanced task, you can specify the encoding; but " +
                    "don't try this if you are unfamiliar with codepages and " +
                    "encodings; rather, seek some help.", 
                    new string[] { sFileName });
                m_labelFileStats.TextAlign = ContentAlignment.BottomLeft;
                m_btnSpecifyEncoding.Visible = true;
                return false;
            }

            // Is this a standard format file?
            string sFirstLine = "";
            try
            {
                StreamReader r = new StreamReader(sPathName, true);
                sFirstLine = r.ReadLine();
                r.Close();
            }
            catch (Exception)
            {
            }
            if (string.IsNullOrEmpty(sFirstLine) || sFirstLine[0] != '\\')
            {
                string sFileName = Path.GetFileName(sPathName);
                m_labelFileStats.Text = LocDB.GetValue(this, "strNotStandardFormat",
                    "File '{0}' does not appear to be a Standard Format file. " +
                    "(It does not begin with a backslash character.)",
                    new string[] { sFileName });
                return false;
            }

			// Read in the file; the rest of what we do is based on internals
            ScriptureDB DB = new ScriptureDB();
            string s = sPathName;
			TextReader tr = JW_Util.GetTextReader(ref s, "");
			DB.Read(tr);
			tr.Close();
            DB.TransformIn();

			// Determine the book from the ID line
			Wizard.BookAbbrev = DB.GetAbbrevFromIdLine();
			int iBook = DBook.FindBookAbbrevIndex(Wizard.BookAbbrev);
			if (-1 == iBook)
			{
				m_labelFileStats.Text = LocDB.GetValue(this, "strUnrecognizedBook",
					"The file either does not have an \\id line, or does not have a " +
					"recognized 3-Letter book abbreviation in it. OurWord expects " +
					"something like \"\\id LUK\"", null);
				return false;
			}
			Wizard.BookName = DBook.GetBookName(iBook, Wizard.Translation);

            // See that it is either Paratext or Toolbox; and check  for any unknown markers.
            ArrayList aUnknownMarkers = DB.GetUnnownMarkersInventory();
            string sUnrecognizedMarkers = "";
            foreach (string sMkr in aUnknownMarkers)
            {
                if (sUnrecognizedMarkers.Length > 0)
                    sUnrecognizedMarkers += ", ";
                if (sUnrecognizedMarkers.Length > 20)
                {
                    sUnrecognizedMarkers += LocDB.GetValue(this, "strMore", "(more)", null);
                    break;
                }
                sUnrecognizedMarkers += sMkr;
            }
            string sMarkerFeedback = (sUnrecognizedMarkers.Length > 0) ?
                LocDB.GetValue(this, "strUnknownMarkers",
                    "Unknown Markers: {0}",
                    new string[] { sUnrecognizedMarkers })
                : "";

            // Display the message with the results of our scan
            m_labelFileStats.ForeColor = Color.Black;
            m_labelFileStats.Font = new Font(
                m_labelFileStats.Font, FontStyle.Regular);

            string[] v = new string[3];
            v[0] = Path.GetFileName(sPathName);
            v[1] = (DB.IsParatextFormat) ? "Paratext" : "Toolbox";
            v[2] = sMarkerFeedback;
            m_labelFileStats.Text = LocDB.GetValue(this, "strFileStats",
                "File: {0}\n" +
                "Format: {1}\n" +
                "{2}",                 // Last row is for optional unkown markers
                v);

            // Build the format string
            m_sFormat = (DB.IsParatextFormat) ? "Paratext" : "Toolbox";

            // If we are here, we have a valid filename, so the Next button can be enabled.
            m_bCanGoToNextPage = true;
            Wizard.AdvanceButtonEnabled = CanGoToNextPage();
            Wizard.Text = LocDB.GetValue(Wizard, Wizard.Name, WizImportBook.c_sEnglishTitle) +
                " (" + Path.GetFileName(PathName) + ")";
            Wizard.PlaceFocusOnAdvanceButton();

            return true;
        }
        #endregion
        #region Method: void SetPathLabelText()
        void SetPathLabelText()
        {
            m_labelFileName.Text = JWU.PathEllipses(PathName, 40);
        }
        #endregion

        // Command Handlers ------------------------------------------------------------------
        #region Cmd: cmdBrowse - Browse for the file to be imported
        private void cmdBrowse(object sender, EventArgs e)
        {
            // Create an OpenFileDialog
            OpenFileDialog dlg = new OpenFileDialog();

            // Only permit a single file to be selected
            dlg.Multiselect = false;

            // The title reinforces the instructions ("Choose the file you wish
            // to import")
            dlg.Title = LocDB.GetValue(this, "strOpenDialogTitle",
                "Choose the file you wish to import", null);

            // The Filter lists the possible types of files:
            // - Shoebox/Toolbox (db)
            // - Standard Format (sf)
            // - Paratext (ptx)
            // - All files (which we default to)
            dlg.Filter = DlgBookPropsRes.FindFileDlgFilter;
            dlg.FilterIndex = 4;

            // Default to the Data Root folder; if we've browsed before, go to that
            // folder we last browsed to.
            dlg.InitialDirectory = G.BrowseDirectory;

            // We cannot have a FileName because we have not browsed for anything yet
            dlg.FileName = "";

            // Do the dialog; if the user cancels, we leave things as they are
            if (DialogResult.OK != dlg.ShowDialog(this))
                return;

            // Update the Browse Directory, so that the next time we browse we'll
            // start at this point where the user navigated.
            G.BrowseDirectory = Path.GetDirectoryName(dlg.FileName);

            // Remember the PathName so we can retrieve it when the wizard is done.
            m_sPathName = dlg.FileName;

            // Set the value into the Label control, with an ellipsis if the 
            // path is too long to fit (this ellipsis method makes certain you
            // can see the filename; it places the ellipses in the middle of the
            // path.
            SetPathLabelText();

            // Examine the file to make sure it is OK to import (this sets the
            // value for CanGoToNextPage.)
            if (!ExamineFile(dlg.FileName))
                return;
        }
        #endregion
        #region Cmd: cmdSpecifyEncoding
        private void cmdSpecifyEncoding(object sender, EventArgs e)
        {
            // Run the dialog to let the user figure out the encoding
            DlgSpecifyEncoding dlg = new DlgSpecifyEncoding(PathName);
            if (DialogResult.OK != dlg.ShowDialog())
                return;

            // Create the new path name
            string sPathNew = PathName + ".utf8";

            // The properly interpreted file currently exists in the dialog
            string[] vLines = dlg.FileLines;

            // Write out the new UTF8 file
            TextWriter w = JW_Util.GetTextWriter(sPathNew);
            foreach (string s in vLines)
                w.WriteLine(s);
            w.Close();

            // Update everything to reflect the new file
            m_sPathName = sPathNew;
            SetPathLabelText();

            // Examine the file for suitability
            if (!ExamineFile(PathName))
                return;
        }
        #endregion

    }
}
