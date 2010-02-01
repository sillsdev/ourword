#region ***** DlgUpdate.cs *****
/**********************************************************************************************
 * Project: Our Word!
 * File:    DlgUpdate.cs
 * Author:  John Wimbish
 * Created: 28 Jan 2010
 * Purpose: Ask the user if he wants to update
 * Legal:   Copyright (c) 2003-10, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
using System.Windows.Forms;
#endregion

namespace OurWordSetup.UI
{
    public partial class DlgDoYouWishToUpdate : Form
    {
        #region Attr{s}: string OurVersion
        public string OurVersion
        {
            set
            {
                m_labelYourVersion.Text = value;
            }
        }
        #endregion
        #region Attr{s}: string RemoteVersion
        public string RemoteVersion
        {
            set
            {
                m_labelRemoteVersion.Text = value;
            }
        }
        #endregion

        #region Constructor()
        public DlgDoYouWishToUpdate()
        {
            InitializeComponent();
        }
        #endregion
    }
}