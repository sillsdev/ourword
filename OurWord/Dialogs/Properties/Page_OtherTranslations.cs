#region ***** Page_OtherTranslations.cs *****
/**********************************************************************************************
 * Project: Our Word!
 * File:    Page_OtherTranslations.cs
 * Author:  John Wimbish
 * Created: 28 Dec 2004
 * Purpose: Sets up the members of the Siblings or the Reference translations list.
 * Legal:   Copyright (c) 2005-09, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
#region Header: Using, etc.
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Resources;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using System.Threading;

using JWTools;
using JWdb;
using JWdb.DataModel;
using OurWord;
using OurWord.Dialogs;
using OurWord.Layouts;
using OurWord.Utilities;
#endregion
#endregion

namespace OurWord.Dialogs
{
    public class Page_OtherTranslations : DlgPropertySheet
	{
        // List of available languages to choose from ----------------------------------------
        #region Attr{g}: List<string> AvailableLanguages
        List<string> AvailableLanguages
        {
            get
            {
                return m_vAvailableLanguages;
            }
        }
        List<string> m_vAvailableLanguages;
        #endregion
        #region Method: void BuildAvailableLanguages()
        void BuildAvailableLanguages()
        {
            // Get the list from the current folder contents
            ClusterInfo ci = ClusterList.FindClusterInfo(DB.TeamSettings.DisplayName);
            m_vAvailableLanguages = ci.GetClusterLanguageList(false);

            // Remove the Front and Target translations
            if (null != DB.FrontTranslation)
                AvailableLanguages.Remove(DB.FrontTranslation.DisplayName);
            if (null != DB.TargetTranslation)
                AvailableLanguages.Remove(DB.TargetTranslation.DisplayName);
        }
        #endregion

        // CheckBox List ---------------------------------------------------------------------
        #region Attr{g}: CheckedListBox List
        CheckedListBox List
		{
			get
			{
                return m_listTranslations;
			}
		}
		#endregion
        #region Method: void PopulateList()
        void PopulateList()
        {
            List.Items.Clear();

            foreach (string s in AvailableLanguages)
            {
                // Is the item already in our list?
                bool bChecked = false;
                foreach (DTranslation t in DB.Project.OtherTranslations)
                {
                    if (t.DisplayName == s)
                        bChecked = true;
                }

                // Add the item, appropriately checked.
                List.Items.Add(s, bChecked);
            }
        }
        #endregion

        // Attrs -----------------------------------------------------------------------------
        #region VAttr{g}: DTranslation SelectedTranslation
        DTranslation SelectedTranslation
        {
            get
            {
                // Nothing to do if there is nothing selected in the list
                if (List.SelectedIndex == -1)
                    return null;

                // Retrieve the name of the selected translation
                string sDisplayName = List.SelectedItem.ToString();

                // Find it in the list
                foreach (DTranslation t in DB.Project.OtherTranslations)
                {
                    if (t.DisplayName == sDisplayName)
                        return t;
                }

                return null;
            }
        }
        #endregion

        // Scaffolding -----------------------------------------------------------------------
		#region Dialog Controls
        protected System.Windows.Forms.Label m_lblSetup;
		private System.Windows.Forms.Button m_btnCreate;
        protected System.Windows.Forms.Label m_lblCreate;
        private CheckedListBox m_listTranslations;
        private Label m_labelInstructions;
		// Required designer variable.
		private System.ComponentModel.Container components = null;
		#endregion
        #region Constructor(DlgProperties)
        public Page_OtherTranslations(DialogProperties _ParentDlg)
            : base(_ParentDlg)
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();
		}
		#endregion
		#region Method: void Dispose() - Clean up any resources being used.
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}
		#endregion
		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.m_lblSetup = new System.Windows.Forms.Label();
            this.m_btnCreate = new System.Windows.Forms.Button();
            this.m_lblCreate = new System.Windows.Forms.Label();
            this.m_listTranslations = new System.Windows.Forms.CheckedListBox();
            this.m_labelInstructions = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // m_lblSetup
            // 
            this.m_lblSetup.Location = new System.Drawing.Point(8, 8);
            this.m_lblSetup.Name = "m_lblSetup";
            this.m_lblSetup.Size = new System.Drawing.Size(457, 33);
            this.m_lblSetup.TabIndex = 0;
            this.m_lblSetup.Text = "A Reference Translation is another translation that you may want to view during t" +
                "he translation process.";
            this.m_lblSetup.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // m_btnCreate
            // 
            this.m_btnCreate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.m_btnCreate.Location = new System.Drawing.Point(11, 310);
            this.m_btnCreate.Name = "m_btnCreate";
            this.m_btnCreate.Size = new System.Drawing.Size(75, 41);
            this.m_btnCreate.TabIndex = 2;
            this.m_btnCreate.Text = "Create New...";
            this.m_btnCreate.Click += new System.EventHandler(this.cmdCreate);
            // 
            // m_lblCreate
            // 
            this.m_lblCreate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.m_lblCreate.Location = new System.Drawing.Point(92, 310);
            this.m_lblCreate.Name = "m_lblCreate";
            this.m_lblCreate.Size = new System.Drawing.Size(373, 47);
            this.m_lblCreate.TabIndex = 19;
            this.m_lblCreate.Text = "Start from scratch to initialize settings for a reference translation. You will b" +
                "e presented with blank settings, and will need to enter the name of the translat" +
                "ion, import its books, etc.";
            // 
            // m_listTranslations
            // 
            this.m_listTranslations.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.m_listTranslations.FormattingEnabled = true;
            this.m_listTranslations.Location = new System.Drawing.Point(11, 58);
            this.m_listTranslations.Name = "m_listTranslations";
            this.m_listTranslations.Size = new System.Drawing.Size(269, 229);
            this.m_listTranslations.Sorted = true;
            this.m_listTranslations.TabIndex = 1;
            this.m_listTranslations.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.cmdItemCheck);
            // 
            // m_labelInstructions
            // 
            this.m_labelInstructions.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.m_labelInstructions.Location = new System.Drawing.Point(286, 58);
            this.m_labelInstructions.Name = "m_labelInstructions";
            this.m_labelInstructions.Size = new System.Drawing.Size(179, 168);
            this.m_labelInstructions.TabIndex = 24;
            this.m_labelInstructions.Text = "Place a check beside the translations you want to place in the Side Window. You c" +
                "an use the Create button to add a new translation that does not appear in this l" +
                "ist.";
            // 
            // Page_OtherTranslations
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.m_labelInstructions);
            this.Controls.Add(this.m_listTranslations);
            this.Controls.Add(this.m_lblCreate);
            this.Controls.Add(this.m_btnCreate);
            this.Controls.Add(this.m_lblSetup);
            this.Name = "Page_OtherTranslations";
            this.Size = new System.Drawing.Size(468, 368);
            this.Load += new System.EventHandler(this.cmdLoad);
            this.ResumeLayout(false);

		}
		#endregion

        // DlgPropertySheet overrides --------------------------------------------------------
		public const string c_sID = "idTranslations";
		#region OAttr{g}: string ID
		public override string ID
		{
			get
			{
				return c_sID;
			}
		}
		#endregion
		#region Method: void ShowHelp()
        public override void ShowHelp()
        {
            HelpSystem.ShowTopic(HelpSystem.Topic.kReferenceTranslations);
        }
        #endregion
        #region Attr{g}: string TabText
        public override string Title
        {
            get
            {
                return "Reference Translations";
            }
        }
        #endregion

		// Command Handlers ------------------------------------------------------------------
		#region Cmd: cmdLoad
		private void cmdLoad(object sender, System.EventArgs e)
		{
            Control[] vExclude = { m_listTranslations };
            LocDB.Localize(this, vExclude);

            BuildAvailableLanguages();

			PopulateList();
		}
		#endregion
        #region Cmd: cmdCreate - Add a new translation to the list
        private void cmdCreate(object sender, System.EventArgs e)
		{
            // Ask the user to supply a valid name for this new translation
            var dlg = new DlgCreateTranslation();
            if (dlg.ShowDialog() != DialogResult.OK)
                return;

            // Create and store the new translation
            DTranslation t = new DTranslation(dlg.TranslationName);
            DB.Project.OtherTranslations.Append(t);
            t.WriteToFile(new NullProgress());

            // Add it to the Properties dialog and go to its page
            ParentDlg.InitNavigation(Page_Translation.ComputeID(t.DisplayName));

		}
		#endregion
        #region Cmd: cmdItemCheck - turn on/off display of a translation
        private void cmdItemCheck(object sender, ItemCheckEventArgs e)
        {
            // Retrieve the item
            string sItem = (string)List.Items[e.Index];
            bool bChecked = (e.NewValue == CheckState.Checked);

            // Locate the trtanslation in the list, if it is there
            int iPos = DB.Project.OtherTranslations.Find(sItem);

            // If it is being unchecked, then remove it from the list.
            if (!bChecked)
            {
                if (-1 != iPos)
                {
                    DTranslation tRemove = DB.Project.OtherTranslations[iPos];
                    if (null != tRemove)
                        DB.Project.OtherTranslations.Remove(tRemove);
                    ParentDlg.InitNavigation(Page_OtherTranslations.c_sID);
                }
            }

            // If it is being checked, then add it to the list if it isn't there
            // (It may already be there if we're initializing the dialog)
            if (bChecked)
            {
                if (-1 == iPos)
                {
                    DTranslation tInsert = new DTranslation(sItem);
                    DB.Project.OtherTranslations.Append(tInsert);
                    tInsert.LoadFromFile(G.CreateProgressIndicator());
                    ParentDlg.InitNavigation(Page_OtherTranslations.c_sID);
                }
            }
        }
        #endregion

        // Destined for Obsolescence
        #region Destined for Obsolescence
        /***
		#region Attr{g}: static string MostRecentFolder - so we nav to the same place
		static string MostRecentFolder
		{
			get
			{
				if (0 == m_sMostRecentFolder.Length)
					m_sMostRecentFolder = DB.TeamSettings.StoragePath;
				return m_sMostRecentFolder;
			}
			set
			{
				Debug.Assert(value.Length > 0);
				m_sMostRecentFolder = value;
			}
		}
		private static string m_sMostRecentFolder = "";
		#endregion
		#region Method: void PopulateList()
		protected void PopulateList()
		{
			PopulateList("");
		}
		#endregion
		#region Method: void PopulateList(sItemToSelect)
		protected void PopulateList(string sItemToSelect)
		{
			// Empty out the list (so we don't add more items to it)
			List.Items.Clear();

			// Put the daughter names into the list
			foreach(DTranslation t in DB.Project.OtherTranslations)
			{
				List.Items.Add( t.DisplayName );
			}

			// Default: select the first item in the list
			if (List.Items.Count > 0)
				List.SelectedIndex = 0;

			// Select the requested item (if any)
			if (sItemToSelect.Length > 0)
			{
				foreach(string s in List.Items)
				{
					if (s == sItemToSelect)
					{
						List.SelectedItem = s;
						break;
					}
				}
			}
		}
		#endregion
        ***/
        #endregion
    }

}
