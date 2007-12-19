/**********************************************************************************************
 * Project: Our Word!
 * File:    Dialogs\WizImportBook\WizPage_GetFileName.cs
 * Author:  John Wimbish
 * Created: 1 Feb 2007
 * Purpose: First page of wizard, user enters the file name
 * Legal:   Copyright (c) 2003-08, John S. Wimbish. All Rights Reserved.  
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
using OurWord.DataModel;
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
        #region VAttr{g}: JW_Wizard Wizard - the owning wizard
        JW_Wizard Wizard
        {
            get
            {
                Debug.Assert(null != Parent as JW_Wizard);
                return Parent as JW_Wizard;
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
            HelpSystem.Show_WizImportBook_GetFilename();
        }
        #endregion

        // Misc Methods ----------------------------------------------------------------------
        #region bool ExamineFile(string sPathName) - see if the proposed file is suitable
        bool ExamineFile(string sPathName)
        {
            // Our default will be that the file does not pass muster.
            m_bCanGoToNextPage = false;
            m_sFormat = "";

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
            Cursor cursorOriginal = Cursor.Current;
            Cursor.Current = Cursors.WaitCursor;

            // Things we'll test for
            bool bCanAccessFile = true;
            bool bIsStdFormat = false;
            bool bIsEncodingUTF8 = false;
            bool bIsEncodingASCII = false;

            // Initial test is for showstopper errors:
            // Check for StdFormat, Encoding, and Ability to open
            try
            {
                StreamReader r = new StreamReader(sPathName);
                string sFirstLine = r.ReadLine();

                if (null != sFirstLine && sFirstLine.Length > 0 && sFirstLine[0] == '\\')
                    bIsStdFormat = true;

                if (r.CurrentEncoding == Encoding.UTF8)
                    bIsEncodingUTF8 = true;
                if (r.CurrentEncoding == Encoding.ASCII)
                    bIsEncodingUTF8 = true;

                r.Close();               
            }
            catch (Exception)
            {
                bCanAccessFile = false;
            }

            // If we have discovered a problem here, then display it and we're done.
            Cursor.Current = cursorOriginal;
            if (!bCanAccessFile)
            {
                string sFileName = Path.GetFileName(sPathName);
                m_labelFileStats.Text = LocDB.GetValue(this, "strUnableToAccessFile", 
                    "Unable to access file '{0}'.", new string[] { sFileName }); 
                return false;
            }
            else if (!bIsStdFormat)
            {
                string sFileName = Path.GetFileName(sPathName);
                m_labelFileStats.Text = LocDB.GetValue(this, "strNotStandardFormat",
                    "File '{0}' does not appear to be a Standard Format file. " +
                    "(It does not begin with a backslash character.)", 
                    new string[] { sFileName });
                return false;
            }
            else if (!bIsEncodingASCII && !bIsEncodingUTF8)
            {
                string sFileName = Path.GetFileName(sPathName);
                m_labelFileStats.Text = LocDB.GetValue(this, "strFileNotInSupportedEncoding",
                    "File '{0}' is not in a supported encoding. It should either be ASCII or UTF8.", 
                    new string[] { sFileName });
                return false;
            }

            // Otherwise, conduct the next level of checking (warnings):
            // For Toolbox/Paratext, and markers
            ScriptureDB DB = new ScriptureDB();
            string s = sPathName;
            DB.Read(ref s);
            DB.TransformIn();
            ArrayList aUnkonwnMarkers = DB.GetUnnownMarkersInventory();
            string sUnrecognizedMarkers = "";
            foreach (string sMkr in aUnkonwnMarkers)
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
                    new string[] { sUnrecognizedMarkers } )
                : "";

            // Display the message with the results of our scan
            m_labelFileStats.ForeColor = Color.Black;
            m_labelFileStats.Font = new Font(
                m_labelFileStats.Font, FontStyle.Regular);

            string[] v = new string[4];
            v[0] = Path.GetFileName(sPathName);
            v[1] = (bIsEncodingASCII) ? "ASCII" : "UTF8";
            v[2] = (DB.IsParatextFormat) ? "Paratext" : "Toolbox";
            v[3] = sMarkerFeedback;
            m_labelFileStats.Text = LocDB.GetValue(this, "strFileStats",
                "File: {0}\n" +
                "Encoding: {1}\n" +
                "Format: {2}\n" +
                "{3}",                 // Last row is for optional unkown markers
                v); 

            // Build the format string
            m_sFormat = (DB.IsParatextFormat) ? "Paratext" : "Toolbox";
            m_sFormat += ", ";
            m_sFormat += (bIsEncodingASCII) ? "ASCII" : "UTF8";

            // We're OK to go to the next wizard page, realizing the user
            // will have to decide on any warnings.
            m_bCanGoToNextPage = true;

            return true;
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

            // Examine the file to make sure it is OK to import (this sets the
            // value for CanGoToNextPage.)
            if (!ExamineFile(dlg.FileName))
                return;

            // Remember the PathName so we can retrieve it when the wizard is done.
            m_sPathName = dlg.FileName;

            // Set the value into the Label control, with an ellipsis if the 
            // path is too long to fit (this ellipsis method makes certain you
            // can see the filename; it places the ellipses in the middle of the
            // path.
            m_labelFileName.Text = JWU.PathEllipses(dlg.FileName, 40);

            // If we are here, we have a valid filename, so the Next button can be enabled.
            Wizard.AdvanceButtonEnabled = CanGoToNextPage();
            Wizard.Text = LocDB.GetValue(Wizard, Wizard.Name, WizImportBook.c_sEnglishTitle) + 
                " (" + Path.GetFileName(dlg.FileName) + ")";
            Wizard.PlaceFocusOnAdvanceButton();
        }
        #endregion

    }
}
