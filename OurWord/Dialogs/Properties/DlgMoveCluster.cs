#region ***** DlgMoveCluster.cs *****
/**********************************************************************************************
 * Project: Our Word!
 * File:    DlgMoveCluster.cs
 * Author:  John Wimbish
 * Created: 15 May 2009
 * Purpose: Dialog for specifying where to move the cluster to
 * Legal:   Copyright (c) 2003-09, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
#region Using
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using JWdb.DataModel;
using OurWord.Utilities;
#endregion
#endregion

namespace OurWord.Dialogs
{
    public partial class DlgMoveCluster : Form
    {
        #region Attr{g}: bool StoreInMyDocuments
        public bool StoreInMyDocuments
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

        #region Constructor()
        public DlgMoveCluster()
        {
            InitializeComponent();
        }
        #endregion
    }
}
