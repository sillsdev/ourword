#region ***** DlgMoveCluster.cs *****
/**********************************************************************************************
 * Project: Our Word!
 * File:    DlgMoveCluster.cs
 * Author:  John Wimbish
 * Created: 15 May 2009
 * Purpose: Dialog for specifying where to move the cluster to
 * Legal:   Copyright (c) 2003-11, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
using System;
using System.Windows.Forms;
using JWTools;
#endregion

namespace OurWord.Dialogs.Properties
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

        #region cmd: cmdLoad
        private void cmdLoad(object sender, EventArgs e)
        {
            LocDB.Localize(this, new Control[] {});
        }
        #endregion
    }
}
