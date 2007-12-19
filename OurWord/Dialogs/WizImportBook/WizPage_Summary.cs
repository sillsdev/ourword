/**********************************************************************************************
 * Project: Our Word!
 * File:    Dialogs\WizImportBook\WizPage_Summary.cs
 * Author:  John Wimbish
 * Created: 13 Feb 2007
 * Purpose: User Finishes the process
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
    public partial class WizPage_Summary : UserControl, IJW_WizPage
    {
        // Attrs -----------------------------------------------------------------------------
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

        // IJW_WizPage Implementation --------------------------------------------------------
        #region Method: void OnActivate()
        public void OnActivate()
        {
            m_textBook.Text = Wizard.BookAbbrev;
            m_textFileName.Text = JWU.PathEllipses(Wizard.ImportFileName, 40);
            m_textFormat.Text = Wizard.Format;
            m_textStage.Text = Wizard.Stage + ", " + Wizard.Version.ToString();
            m_textDestination.Text = JWU.PathEllipses(Wizard.DestinationFolder, 40);
        }
        #endregion
        #region Method: bool CanGoToNextPage()
        public bool CanGoToNextPage()
        {
            return true;
        }
        #endregion
        #region Method: string PageNavigationTitle()
        public string PageNavigationTitle()
        {
            return LocDB.GetValue(this, "strNavTitle", "Summary", null);
        }
        #endregion
        #region Method: void ShowHelp()
        public void ShowHelp()
        {
            HelpSystem.Show_WizImportBook_Summary();
        }
        #endregion

        // Scaffolding -----------------------------------------------------------------------
        #region Constructor()
        public WizPage_Summary()
        {
            InitializeComponent();
        }
        #endregion
        #region Attr{g}: Control[] vExclude
        public Control[] vExclude
        {
            get
            {
                return new Control[] { m_textStage, m_textFileName, m_textDestination, m_textFormat, m_textBook };
            }
        }
        #endregion

    }
}
