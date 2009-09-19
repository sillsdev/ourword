#region ***** DlgExport.cs *****
/**********************************************************************************************
 * Project: Our Word!
 * File:    DlgExport.cs
 * Author:  John Wimbish
 * Created: 26 July 2007
 * Purpose: Export the current project to, e.g., Paratext format.
 * Legal:   Copyright (c) 2005-09, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
#region Header: Using, etc.
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Resources;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using System.Threading;

using JWTools;
using JWdb.DataModel;
using OurWord;
using OurWord.Dialogs;
using OurWord.Layouts;
#endregion
#endregion

namespace OurWord.Dialogs
{
    public partial class DialogExport : Form
    {
        // Attrs -----------------------------------------------------------------------------
        #region VAttr{g}: bool ExportToToolbox
        public bool ExportToToolbox
        {
            get
            {
                return (m_radioToolbox.Checked == true);
            }
        }
        #endregion
        #region VAttr{g}: bool ExportToParatext
        public bool ExportToParatext
        {
            get
            {
                return (m_radioParatext.Checked == true);
            }
        }
        #endregion
        #region VAttr{g}: bool ExportToGoBibleCreator
        public bool ExportToGoBibleCreator
        {
            get
            {
                return (m_radioGoBible.Checked == true);
            }
        }
        #endregion
        #region VAttr{g}: string ExportSubFolderName
        public string ExportSubFolderName
        {
            get
            {
                string sSubFolder = "OurWordExport" + Path.DirectorySeparatorChar;

                if (ExportToParatext)
                    sSubFolder += "Paratext" + Path.DirectorySeparatorChar;

                if (ExportToGoBibleCreator)
                    sSubFolder += "GoBibleCreator" + Path.DirectorySeparatorChar;

                if (ExportToToolbox)
                    sSubFolder += "Toolbox" + Path.DirectorySeparatorChar;

                return sSubFolder;
            }
        }
        #endregion

        // Scaffolding -----------------------------------------------------------------------
        #region Constructor()
        public DialogExport()
        {
            InitializeComponent();
        }
        #endregion

        // Command Handlers ------------------------------------------------------------------
        #region Cmd: cmdLoad
        private void cmdLoad(object sender, EventArgs e)
        {
            // Label text in the appropriate language
            Control[] vExclude = { };
            LocDB.Localize(this, vExclude);

            // Default to Paratext
            m_radioParatext.Checked = true;
        }
        #endregion
        #region Cmd: cmdUpdateLocation
        private void cmdUpdateLocation(object sender, EventArgs e)
        {
            // Retrieve the location of My Documents within this OS
            string sMyDocs = JWU.GetMyDocumentsFolder(null);

            // Strip off folders prior to the My Documents folder
            do
            {
                int i = sMyDocs.IndexOf(Path.DirectorySeparatorChar);

                if (i == -1)
                    break;

                if (i == sMyDocs.Length - 1)
                    break;

                sMyDocs = sMyDocs.Substring(i + 1);

            } while (true);

            // Append a backslash to the end
            if (sMyDocs.Length > 0 && sMyDocs[sMyDocs.Length - 1] != Path.DirectorySeparatorChar)
                sMyDocs += Path.DirectorySeparatorChar;

            m_labelFolder.Text = sMyDocs + ExportSubFolderName;
        }
        #endregion
    }
}