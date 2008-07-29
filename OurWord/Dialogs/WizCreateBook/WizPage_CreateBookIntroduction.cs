/**********************************************************************************************
 * Project: Our Word!
 * File:    Dialogs\WizCreateBook\WizPage_CreateBookIntroduction.cs
 * Author:  John Wimbish
 * Created: 28 Jun 2008
 * Purpose: Provides basic information about creating a new book
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

namespace OurWord.Dialogs.WizCreateBook
{
    public partial class WizPage_CreateBookIntroduction : UserControl, IJW_WizPage
    {
        // Attrs -----------------------------------------------------------------------------
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

        // Scaffolding -----------------------------------------------------------------------
        #region Constructor()
        public WizPage_CreateBookIntroduction()
        {
            InitializeComponent();
        }
        #endregion

        // IJW_WizPage Implementation --------------------------------------------------------
        #region Method: void OnActivate()
        public void OnActivate()
        {
            Wizard.PlaceFocusOnAdvanceButton();
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
            return LocDB.GetValue(this, "strNavTitle", "Introduction", null);
        }
        #endregion
        #region Method: void ShowHelp()
        public void ShowHelp()
        {
            HelpSystem.ShowTopic(HelpSystem.Topic.kNewBook);
        }
        #endregion

        // Command Handlers ------------------------------------------------------------------
        #region Cmd: cmdLoad
        private void cmdLoad(object sender, EventArgs e)
        {
        }
        #endregion
    }
}
