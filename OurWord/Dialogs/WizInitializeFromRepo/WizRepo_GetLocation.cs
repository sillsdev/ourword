#region ***** WizRepo_GetAuthorName.cs *****
/**********************************************************************************************
 * Project: Our Word!
 * File:    WizRepo_GetAuthorName.cs
 * Author:  John Wimbish
 * Created: 16 May 2009
 * Purpose: Allows the user to set the disk Location for the cluster
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
using OurWordData;
using OurWordData.DataModel;
#endregion
#endregion

namespace OurWord.Dialogs
{
    public partial class WizRepo_GetLocation : UserControl, IJW_WizPage
    {

        // Scaffolding -----------------------------------------------------------------------
        #region Constructor()
        public WizRepo_GetLocation()
        {
            InitializeComponent();

            // Default to My Documents
            m_ClusterLocation.IsInMyDocuments = true;
        }
        #endregion

        // User-Entered Information ----------------------------------------------------------
        #region Attr{g/s}: bool IsInMyDocuments
        public bool IsInMyDocuments
        {
            get
            {
                return m_ClusterLocation.IsInMyDocuments;
            }
            set
            {
                m_ClusterLocation.IsInMyDocuments = value;
            }
        }
        #endregion
        #region Attr{g/s}: bool IsInAppData
        public bool IsInAppData
        {
            get
            {
                return m_ClusterLocation.IsInAppData;
            }
            set
            {
                m_ClusterLocation.IsInAppData = value;
            }
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
            return LocDB.GetValue(this, "strLocation", "Location", null);
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
