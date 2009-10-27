/**********************************************************************************************
 * Project: Our Word!
 * File:    Dialogs\WizNewProject\WizNew_FrontInfo.cs
 * Author:  John Wimbish
 * Created: 26 Jan 2008
 * Purpose: Obtains the name, abbreviation and settings file for the front translation
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

namespace OurWord.Dialogs.WizNewProject
{
    public partial class WizNew_FrontInfo : UserControl, IJW_WizPage
    {
        // Attrs -----------------------------------------------------------------------------
        #region VAttr{g}: WizNewProject Wizard - the owning wizard
        WizNewProject Wizard
        {
            get
            {
                Debug.Assert(null != Parent as WizNewProject);
                return Parent as WizNewProject;
            }
        }
        #endregion
		#region VAttr{g}: bool CreatingNewFront
		public bool CreatingNewFront
			// T if the name in the combo box is not currently on the disk
		{
			get
			{
				var v = Wizard.Languages;
				foreach (string s in v)
				{
					if (s == Wizard.FrontName)
						return false;
				}
				return true;
			}
		}
		#endregion

		// Scaffolding -----------------------------------------------------------------------
        #region Constructor()
        public WizNew_FrontInfo()
        {
            InitializeComponent();
        }
        #endregion

        // IJW_WizPage Implementation --------------------------------------------------------
        #region Method: void OnActivate()
        public void OnActivate()
        {
			// Populate the combo box
			var v = Wizard.Languages;
			m_comboChooseLanguage.Items.Clear();
			foreach (string s in v)
			{
				m_comboChooseLanguage.Items.Add(s);

				if (Wizard.FrontName == s)
					m_comboChooseLanguage.Text = Wizard.FrontName;
			}

			Wizard.AdvanceButtonEnabled = CanGoToNextPage();
		}
        #endregion
        #region Method: bool CanGoToNextPage()
        public bool CanGoToNextPage()
        {
            if (string.IsNullOrEmpty(Wizard.FrontName))
                return false;
            return true;
        }
        #endregion
        #region Method: string PageNavigationTitle()
        public string PageNavigationTitle()
        {
            return LocDB.GetValue(this, "strFrontInfo", "Front Information", null);
        }
        #endregion
        #region Method: void ShowHelp()
        public void ShowHelp()
        {
            //HelpSystem.Show_WizImportBook_IdentifyBook();
        }
        #endregion

		// Command Handlers ------------------------------------------------------------------
		#region Cmd: cmdComboTextChanged
		private void cmdComboTextChanged(object sender, EventArgs e)
		{
			Wizard.FrontName = m_comboChooseLanguage.Text;
			Wizard.AdvanceButtonEnabled = CanGoToNextPage();
		}
		#endregion
	}
}
