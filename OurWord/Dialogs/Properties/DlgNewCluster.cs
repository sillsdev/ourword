#region ***** DlgNewCluster.cs *****
/**********************************************************************************************
 * Project: Our Word!
 * File:    DlgNewCluster.cs
 * Author:  John Wimbish
 * Created: 15 May 2009
 * Purpose: Dialog for creating a new cluster.
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
    public partial class DlgNewCluster : Form
    {
        #region Attr{g}: bool StoreInMyDocuments
        public bool StoreInMyDocuments
        {
            get
            {
                return m_ClusterLocation.IsInMyDocuments;
            }
        }
        #endregion
        #region Attr{g}: string NewClusterName
        public string NewClusterName
        {
            get
            {
                return m_textName.Text;
            }
        }
        #endregion

        #region Constructor()
        public DlgNewCluster()
        {
            InitializeComponent();
        }
        #endregion
        #region Cmd: cmdLoad
        private void cmdLoad(object sender, EventArgs e)
        {
            m_ClusterLocation.IsInMyDocuments = true;
        }
        #endregion
        #region Cmd: cmdFormClosing
        private void cmdFormClosing(object sender, FormClosingEventArgs e)
        {
            // Make sure we have a non-zero name
            if (string.IsNullOrEmpty(NewClusterName))
            {
                m_labelError.Text = "Please enter a name for your new cluster.";
                e.Cancel = true;
                return;
            }

            // Make sure we have a unique name
            foreach (ClusterInfo ci in ClusterList.Clusters)
            {
                if (ci.Name == NewClusterName)
                {
                    m_labelError.Text = "There is already a cluster with this name on your computer.";
                    e.Cancel = true;
                    return;
                }
            }
        }
        #endregion
    }
}
