#region ***** ClusterLocation.cs *****
/**********************************************************************************************
 * Project: Our Word!
 * File:    ClusterLocation.cs
 * Author:  John Wimbish
 * Created: 17 May 2009
 * Purpose: Control to set the location of the cluster (used in multiple places in the UI)
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

namespace OurWord.Utilities
{
    public partial class ClusterLocation : UserControl
    {
        // User-Entered Information ----------------------------------------------------------
        #region Attr{g/s}: bool IsInMyDocuments
        public bool IsInMyDocuments
        {
            get
            {
                return m_radioMyDocuments.Checked;
            }
            set
            {
                m_radioMyDocuments.Checked = value;
                m_radioAppData.Checked = !value;
            }
        }
        #endregion
        #region Attr{g/s}: bool IsInAppData
        public bool IsInAppData
        {
            get
            {
                return m_radioAppData.Checked;
            }
            set
            {
                m_radioAppData.Checked = value;
                m_radioMyDocuments.Checked = !value;
            }
        }
        #endregion

        // Scaffolding -----------------------------------------------------------------------
        #region Constructor()
        public ClusterLocation()
        {
            InitializeComponent();
        }
        #endregion
    }
}
