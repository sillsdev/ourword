/**********************************************************************************************
 * Project: Our Word!
 * File:    Page_OtherTranslations.cs
 * Author:  John Wimbish
 * Created: 28 Dec 2004
 * Purpose: Sets up the members of the Siblings or the Reference translations list.
 * Legal:   Copyright (c) 2005-08, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
#region Header: Using, etc.
using System;
using System.Collections;
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
using OurWord;
using OurWord.DataModel;
using OurWord.Dialogs;
using OurWord.View;
#endregion

namespace OurWord.Dialogs
{
    public class Page_OtherTranslations : DlgPropertySheet
	{
		// Attrs -----------------------------------------------------------------------------
        #region Attr{g}: ListBox List
		ListBox List
		{
			get
			{
				return m_list;
			}
		}
		#endregion
		#region Attr{g}: static string MostRecentFolder - so we nav to the same place
		static string MostRecentFolder
		{
			get
			{
				if (0 == m_sMostRecentFolder.Length)
					m_sMostRecentFolder = G.TeamSettings.DataRootPath;
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
                foreach (DTranslation t in G.Project.OtherTranslations)
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
		protected System.Windows.Forms.Label m_lblTranslation;
		private System.Windows.Forms.Label m_lblActions;
		private System.Windows.Forms.Button m_btnCreate;
		protected System.Windows.Forms.Label m_lblCreate;
		private System.Windows.Forms.Button m_btnOpen;
		protected System.Windows.Forms.Label m_lblOpen;
		private System.Windows.Forms.ListBox m_list;
		private System.Windows.Forms.Button m_btnRemove;
		protected System.Windows.Forms.Label m_lblRemove;
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
            this.m_lblTranslation = new System.Windows.Forms.Label();
            this.m_list = new System.Windows.Forms.ListBox();
            this.m_lblActions = new System.Windows.Forms.Label();
            this.m_btnRemove = new System.Windows.Forms.Button();
            this.m_btnCreate = new System.Windows.Forms.Button();
            this.m_lblCreate = new System.Windows.Forms.Label();
            this.m_btnOpen = new System.Windows.Forms.Button();
            this.m_lblOpen = new System.Windows.Forms.Label();
            this.m_lblRemove = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // m_lblSetup
            // 
            this.m_lblSetup.Location = new System.Drawing.Point(8, 8);
            this.m_lblSetup.Name = "m_lblSetup";
            this.m_lblSetup.Size = new System.Drawing.Size(445, 33);
            this.m_lblSetup.TabIndex = 0;
            this.m_lblSetup.Text = "A Reference Translation is another translation that you may want to view during t" +
                "he translation process.";
            this.m_lblSetup.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // m_lblTranslation
            // 
            this.m_lblTranslation.Location = new System.Drawing.Point(8, 53);
            this.m_lblTranslation.Name = "m_lblTranslation";
            this.m_lblTranslation.Size = new System.Drawing.Size(88, 85);
            this.m_lblTranslation.TabIndex = 15;
            this.m_lblTranslation.Text = "Current Reference Translations:";
            // 
            // m_list
            // 
            this.m_list.Location = new System.Drawing.Point(102, 53);
            this.m_list.Name = "m_list";
            this.m_list.Size = new System.Drawing.Size(351, 147);
            this.m_list.TabIndex = 1;
            this.m_list.DoubleClick += new System.EventHandler(this.cmdProperties);
            // 
            // m_lblActions
            // 
            this.m_lblActions.Location = new System.Drawing.Point(8, 202);
            this.m_lblActions.Name = "m_lblActions";
            this.m_lblActions.Size = new System.Drawing.Size(445, 23);
            this.m_lblActions.TabIndex = 17;
            this.m_lblActions.Text = "Available Actions:";
            this.m_lblActions.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // m_btnRemove
            // 
            this.m_btnRemove.Location = new System.Drawing.Point(11, 325);
            this.m_btnRemove.Name = "m_btnRemove";
            this.m_btnRemove.Size = new System.Drawing.Size(75, 23);
            this.m_btnRemove.TabIndex = 4;
            this.m_btnRemove.Text = "Remove...";
            this.m_btnRemove.Click += new System.EventHandler(this.cmdRemove);
            // 
            // m_btnCreate
            // 
            this.m_btnCreate.Location = new System.Drawing.Point(11, 228);
            this.m_btnCreate.Name = "m_btnCreate";
            this.m_btnCreate.Size = new System.Drawing.Size(75, 23);
            this.m_btnCreate.TabIndex = 2;
            this.m_btnCreate.Text = "Create...";
            this.m_btnCreate.Click += new System.EventHandler(this.cmdCreate);
            // 
            // m_lblCreate
            // 
            this.m_lblCreate.Location = new System.Drawing.Point(99, 228);
            this.m_lblCreate.Name = "m_lblCreate";
            this.m_lblCreate.Size = new System.Drawing.Size(354, 47);
            this.m_lblCreate.TabIndex = 19;
            this.m_lblCreate.Text = "Start from scratch to initialize settings for a reference translation. You will b" +
                "e presented with blank settings, and will need to enter the name of the translat" +
                "ion, its books, etc.";
            // 
            // m_btnOpen
            // 
            this.m_btnOpen.Location = new System.Drawing.Point(11, 275);
            this.m_btnOpen.Name = "m_btnOpen";
            this.m_btnOpen.Size = new System.Drawing.Size(75, 23);
            this.m_btnOpen.TabIndex = 3;
            this.m_btnOpen.Text = "Open...";
            this.m_btnOpen.Click += new System.EventHandler(this.cmdOpen);
            // 
            // m_lblOpen
            // 
            this.m_lblOpen.Location = new System.Drawing.Point(99, 275);
            this.m_lblOpen.Name = "m_lblOpen";
            this.m_lblOpen.Size = new System.Drawing.Size(354, 50);
            this.m_lblOpen.TabIndex = 21;
            this.m_lblOpen.Text = "Make use of existing settings for a reference translation. If you defined the tra" +
                "nslation in another project, then you can reuse it here, rather than entering th" +
                "e information all over again.";
            // 
            // m_lblRemove
            // 
            this.m_lblRemove.Location = new System.Drawing.Point(99, 325);
            this.m_lblRemove.Name = "m_lblRemove";
            this.m_lblRemove.Size = new System.Drawing.Size(354, 42);
            this.m_lblRemove.TabIndex = 22;
            this.m_lblRemove.Text = "Remove the selected translation from the list above. This does not remove the set" +
                "tings file from the disk; it only removes it from this project.";
            // 
            // Page_OtherTranslations
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.m_lblRemove);
            this.Controls.Add(this.m_lblOpen);
            this.Controls.Add(this.m_btnOpen);
            this.Controls.Add(this.m_lblCreate);
            this.Controls.Add(this.m_btnCreate);
            this.Controls.Add(this.m_lblActions);
            this.Controls.Add(this.m_list);
            this.Controls.Add(this.m_lblTranslation);
            this.Controls.Add(this.m_btnRemove);
            this.Controls.Add(this.m_lblSetup);
            this.Name = "Page_OtherTranslations";
            this.Size = new System.Drawing.Size(468, 368);
            this.Load += new System.EventHandler(this.cmdLoad);
            this.ResumeLayout(false);

		}
		#endregion

		// Methods ---------------------------------------------------------------------------
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
			foreach(DTranslation t in G.Project.OtherTranslations)
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

        // DlgPropertySheet overrides --------------------------------------------------------
        #region Method: void ShowHelp()
        public override void ShowHelp()
        {
            HelpSystem.Show_DlgProjectProperties_PageSetupReferences();
        }
        #endregion
        #region Attr{g}: string TabText
        public override string TabText
        {
            get
            {
                return "Translations List";
            }
        }
        #endregion

		// Command Handlers ------------------------------------------------------------------
		#region Handler: cmdLoad
		private void cmdLoad(object sender, System.EventArgs e)
		{
            Control[] vExclude = { };
            LocDB.Localize(this, vExclude);

			PopulateList();
		}
		#endregion
		#region Handler: cmdCreate - Add a blank translation settings to the list
		private void cmdCreate(object sender, System.EventArgs e)
		{
			// Launch the dialog to prompt for crucial information: Name and
			// Settings Filename. The user has the option to abort from this dlg.
			DialiogCreateTranslation dlg = new DialiogCreateTranslation();
			if (DialogResult.OK != dlg.ShowDialog(this))
				return;

			// Create the DTranslation object, initialize it, place it into the Project
			// settings.
			DTranslation trans = new DTranslation( dlg.TranslationName, "Latin", "Latin");
            G.Project.OtherTranslations.Append(trans);
            trans.AbsolutePathName = dlg.SettingsPath;
			trans.LanguageAbbrev = dlg.Abbreviation;
			PopulateList(trans.DisplayName);

			// Update the tabs
            ParentDlg.SetupTabControl(DialogProperties.c_navTranslations);
            ParentDlg.ActivatePage(trans);
		}
		#endregion
		#region Handler: cmdOpen - add an existing translation to the list
		private void cmdOpen(object sender, System.EventArgs e)
		{
            // Launch the dialog to prompt for the Settings Filename. The user has the 
            // option to abort from this dlg.
            DialogOpenTranslation dlg = new DialogOpenTranslation();
            if (DialogResult.OK != dlg.ShowDialog())
                return;

            // Read in the translatino object
            DTranslation trans = new DTranslation();
            G.Project.OtherTranslations.Append(trans);
            trans.AbsolutePathName = dlg.SettingsPath;  // First so we know what to load
            trans.Load();
            trans.AbsolutePathName = dlg.SettingsPath;  // Second because Load overwrote it

            // Update the list
            PopulateList(trans.DisplayName);

            // Remember the folder for next time
            MostRecentFolder = Path.GetDirectoryName(dlg.SettingsPath);

            /*** THIS IS CODE I WROTE DURING THE PROP DLG REWRITE: I've replaced it
             * with the above, having noticed that Page_SetupFT does things differently,
             * and feeling that both should operate in the same manner.
             * 
			// Create and initialize the OS's OpenFileDialog. Settings are:
			OpenFileDialog dlg = new OpenFileDialog();

			// - We only want to return a single file
			dlg.Multiselect = false;

			// - Filter on oTrans files
			dlg.Filter = StrRes.FileFilterTranslation;
			dlg.FilterIndex = 0;

			// - Use the same path as last time
			dlg.InitialDirectory = MostRecentFolder;

			// Retrieve Dialog Title from resources
			dlg.Title = StrRes.DlgOpenSiblingTranslation_Title;

			// Run the dialog
			if (DialogResult.OK != dlg.ShowDialog(this))
				return;

			// Read in the translation's settings
            DTranslation trans = new DTranslation(Path.GetFileName( dlg.FileName ), 
                "Latin", "Latin");
            G.Project.OtherTranslations.Append(trans);
            trans.AbsolutePathName = dlg.FileName;  // First so we know what to load
            trans.Load();
            trans.AbsolutePathName = dlg.FileName;  // Second because Load overwrote it

            // Update the list
            PopulateList(trans.DisplayName);

			// Remember the folder for next time
			MostRecentFolder = Path.GetDirectoryName( dlg.FileName );
            ***/

            // Update the tabs
            ParentDlg.SetupTabControl(DialogProperties.c_navTranslations);
            ParentDlg.ActivatePage(trans);
        }
		#endregion
		#region Handler: cmdRemove - remove a translation from the list
		private void cmdRemove(object sender, System.EventArgs e)
		{
			// Nothing to do if there is nothing selected in the list
            if (null == SelectedTranslation)
                return;

			// Make sure the user wants to remove the translation.
			if (!Messages.VerifyRemoveTranslation() )
				return;

			// Remove it from the Project
            ParentDlg.HarvestChangesFromCurrentSheet();
            G.Project.OtherTranslations.Remove(SelectedTranslation);

			// Refresh the listbox
			PopulateList();

            // Update the tabs
            ParentDlg.SetupTabControl(DialogProperties.c_navTranslations);
        }
		#endregion
		#region Handler: cmdProperties - on dbl click on list item, activate that page
		private void cmdProperties(object sender, System.EventArgs e)
		{
            // Nothing to do if there is nothing selected in the list
            if (null == SelectedTranslation)
                return;

            // Activate the tab
            ParentDlg.ActivatePage(SelectedTranslation);
		}
		#endregion
    }

}
