#region ***** WizRepo_Introduction.cs *****
/**********************************************************************************************
 * Project: Our Word!
 * File:    WizRepo_Introduction.cs
 * Author:  John Wimbish
 * Created: 16 May 2009
 * Purpose: Introduces the InitializeFromRepo Wizard
 * Legal:   Copyright (c) 2003-09, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
#region Using
using System;
using System.Collections;
using System.Collections.Generic;
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
#endregion

namespace OurWord.Dialogs
{
    public partial class WizRepo_Introduction : UserControl, IJW_WizPage
    {
        // Scaffolding -----------------------------------------------------------------------
        #region Constructor()
        public WizRepo_Introduction()
        {
            InitializeComponent();
        }
        #endregion

        // IJW_WizPage Implementation --------------------------------------------------------
        #region Method: void OnActivate()
        public void OnActivate()
        {
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
            return LocDB.GetValue(this, "strIntroduction", "Introduction", null);
        }
        #endregion
        #region Method: void ShowHelp()
        public void ShowHelp()
        {
            //HelpSystem.Show_WizImportBook_IdentifyBook();
        }
        #endregion

    }
}
