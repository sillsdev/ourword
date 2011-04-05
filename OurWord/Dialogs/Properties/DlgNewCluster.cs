#region ***** DlgNewCluster.cs *****
/**********************************************************************************************
 * Project: Our Word!
 * File:    DlgNewCluster.cs
 * Author:  John Wimbish
 * Created: 15 May 2009
 * Purpose: Dialog for creating a new cluster.
 * Legal:   Copyright (c) 2003-09, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
using System;
using System.Windows.Forms;
using JWTools;
using OurWordData.DataModel;
#endregion

namespace OurWord.Dialogs.Properties
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
            LocDB.Localize(this, new Control[] { });

            m_ClusterLocation.IsInMyDocuments = true;
        }
        #endregion
        #region Cmd: cmdFormClosing
        private void cmdFormClosing(object sender, FormClosingEventArgs e)
        {
            if (DialogResult != DialogResult.OK)
                return;

            // Make sure we have a non-zero name
            if (string.IsNullOrEmpty(NewClusterName))
            {
                m_labelError.Text = LocDB.GetValue(this, "kNeedClusterName", 
                    "Please enter a name for your new cluster.");
                e.Cancel = true;
                return;
            }

            // Make sure we have a unique name
            foreach (var ci in ClusterList.Clusters)
            {
                if (ci.Name == NewClusterName)
                {
                    m_labelError.Text = LocDB.GetValue(this, "kAlreadyExists",
                        "There is already a cluster with this name on your computer.");
                    e.Cancel = true;
                    return;
                }
            }
        }
        #endregion
    }
}
