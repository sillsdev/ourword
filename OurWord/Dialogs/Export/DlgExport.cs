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
using OurWord.Dialogs.Export;
using OurWordData.DataModel;
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
        #region Attr{g}: ExportMethod CurrentExportMethod
        public ExportMethod CurrentExportMethod
        {
            get
            {
                foreach (var ctrl in m_groupExportTo.Controls)
                {
                    var radio = ctrl as RadioButton;
                    if (null == radio || !radio.Checked)
                        continue;

                    var method = radio.Tag as ExportMethod;
                    if (null != method)
                        return method;
                }

                return null;
            }
        }
        #endregion
        #region vattr{g}: bool ExportBackTranslation
        private bool ExportBackTranslation
        {
            get
            {
                return (m_comboWhatToExport.Text == Loc.GetString("BackTranslation", "Back Translation"));
            }
        }
        #endregion
        #region Attr{g}: bool ExportCurrentBookOnly
        public bool ExportCurrentBookOnly
        {
            get
            {
                return (m_comboScope.Text == DB.TargetBook.DisplayName);
            }
        }
        #endregion

        #region Attr{g}: DTranslation Translation
        DTranslation Translation
        {
            get
            {
                Debug.Assert(null != m_Translation);
                return m_Translation;
            }
        }
        DTranslation m_Translation;
        #endregion

        // Scaffolding -----------------------------------------------------------------------
        #region Constructor(DTranslation)
        public DialogExport(DTranslation translation)
        {
            m_Translation = translation;

            InitializeComponent();

            m_radioParatext.Tag = new ExportToParatext(translation);
            m_radioGoBible.Tag = new ExportToGoBible(translation);
            m_radioToolbox.Tag = new ExportToToolbox(translation);
            m_radioWord.Tag = new ExportToWord(translation);
        }
        #endregion

        // Command Handlers ------------------------------------------------------------------
        #region Cmd: cmdLoad
        private void cmdLoad(object sender, EventArgs e)
        {
            // Label text in the appropriate language
            Control[] vExclude = { m_comboWhatToExport, m_comboScope };
            LocDB.Localize(this, vExclude);

            m_comboWhatToExport.Items.Clear();
            m_comboWhatToExport.Items.Add(Loc.GetString("Vernacular", "Vernacular"));
            m_comboWhatToExport.Items.Add(Loc.GetString("BackTranslation", "Back Translation"));
            m_comboWhatToExport.Text = Loc.GetString("Vernacular", "Vernacular");

            m_comboScope.Items.Clear();
            m_comboScope.Items.Add(Loc.GetString("AllBooks", "All Books"));
            m_comboScope.Items.Add(DB.TargetBook.DisplayName);
            m_comboScope.Text = DB.TargetBook.DisplayName;

            // Default to Paratext
            m_radioParatext.Checked = true;
        }
        #endregion
        #region Cmd: cmdUpdateLocation
        private void cmdUpdateLocation(object sender, EventArgs e)
        {
            // Retrieve the location of My Documents within this OS
            var sMyDocs = JWU.GetMyDocumentsFolder(null);

            // Strip off folders prior to the My Documents folder
            do
            {
                var i = sMyDocs.IndexOf(Path.DirectorySeparatorChar);

                if (i == -1)
                    break;

                if (i == sMyDocs.Length - 1)
                    break;

                sMyDocs = sMyDocs.Substring(i + 1);

            } while (true);

            // Append a backslash to the end
            if (sMyDocs.Length > 0 && sMyDocs[sMyDocs.Length - 1] != Path.DirectorySeparatorChar)
                sMyDocs += Path.DirectorySeparatorChar;

            var method = CurrentExportMethod;
            if (null != method)
                m_labelFolder.Text = string.Format("{0}{1}*.{2}",
                    Path.Combine(sMyDocs, method.GetSubFolder()),
                    Path.DirectorySeparatorChar,
                    method.FileExtension);
            else
                m_labelFolder.Text = sMyDocs;
        }
        #endregion
        #region Cmd: cmdWhatToExportChanged
        private void cmdWhatToExportChanged(object sender, EventArgs e)
        {
            m_radioWord.Checked = true;

            var method = CurrentExportMethod;
            Debug.Assert(null != method);

            var wordMethod = method as ExportToWord;
            Debug.Assert(null != wordMethod);

            wordMethod.ExportBackTranslation = ExportBackTranslation;

            cmdUpdateLocation(null, null);
        }
        #endregion
    }
}