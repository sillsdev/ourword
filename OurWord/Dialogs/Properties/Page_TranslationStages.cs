/**********************************************************************************************
 * Project: Our Word!
 * File:    Page_TranslationStages.cs
 * Author:  John Wimbish
 * Created: 14 June 2006
 * Purpose: Create settings for the Stage in the Translation Process (e.g., Draft, Team 
 *          Revision, etc.)
 * Legal:   Copyright (c) 2004-09, John S. Wimbish. All Rights Reserved.  
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
using JWdb.DataModel;
using OurWord;
using OurWord.Dialogs;
using OurWord.View;
#endregion

namespace OurWord.Dialogs
{
	#region Class: Page_TranslationStages
    public class Page_TranslationStages : DlgPropertySheet
	{
		// Attrs -----------------------------------------------------------------------------
		#region Attr{g}: JW_ListView LVStages
		JW_ListView LVStages
		{
			get
			{
				return m_listviewStages;
			}
		}
		#endregion
		#region Attr{g}: ComboBox DefaultLanguage
		ComboBox DefaultLanguage
		{
			get
			{
				return m_comboLanguage;
			}
		}
		#endregion
		#region Attr{g}: TranslationStage CurrentSelection
		TranslationStage CurrentSelection
		{
			get
			{
				if (LVStages.SelectedIndices.Count != 1)
					return null;

				int i =  LVStages.SelectedIndices[0];

                TranslationStage stage = DB.TeamSettings.TranslationStages.GetFromIndex(i);
				return stage;
			}
		}
		#endregion
		#region Attr{g}: int CurrentSelectionIndex
		int CurrentSelectionIndex
		{
			get
			{
				if (LVStages.SelectedIndices.Count != 1)
					return -1;

				return  LVStages.SelectedIndices[0];
			}
		}
		#endregion

		// Scaffolding -----------------------------------------------------------------------
		#region DIALOG CONTROLS

		private JW_ListView m_listviewStages;
		private ColumnHeader m_colAbbrev;
		private ColumnHeader m_colName;
		private Label m_labelInstructions;
		private Button m_btnRestoreDefaults;
		private Button m_btnAdd;
		private Button m_btnRemove;
		private Button m_btnMoveUp;
		private Button m_btnMoveDown;
		private GroupBox m_groupLanguage;
		private Label m_labelLanguage;
		private ComboBox m_comboLanguage;
		private System.Windows.Forms.Label m_labelLanguageExplanation;

		// Required designer variable.
		private System.ComponentModel.Container components = null;
		#endregion
		#region Constructor()
        public Page_TranslationStages(DialogProperties _ParentDlg)
            : base(_ParentDlg)
		{
			InitializeComponent();

			// Register the JW_ListView's handler for when an item is edited
			LVStages.SubItemEndEditing += new SubItemEditedEventHandler(cmdSubItemEdited);
		}
		#endregion
		#region Method: void Dispose(...)
		protected override void Dispose( bool disposing )
			// Clean up any resources being used.
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Page_TranslationStages));
			this.m_listviewStages = new JWTools.JW_ListView();
			this.m_colAbbrev = new System.Windows.Forms.ColumnHeader();
			this.m_colName = new System.Windows.Forms.ColumnHeader();
			this.m_btnRestoreDefaults = new System.Windows.Forms.Button();
			this.m_btnAdd = new System.Windows.Forms.Button();
			this.m_btnRemove = new System.Windows.Forms.Button();
			this.m_btnMoveUp = new System.Windows.Forms.Button();
			this.m_btnMoveDown = new System.Windows.Forms.Button();
			this.m_labelLanguage = new System.Windows.Forms.Label();
			this.m_comboLanguage = new System.Windows.Forms.ComboBox();
			this.m_labelInstructions = new System.Windows.Forms.Label();
			this.m_labelLanguageExplanation = new System.Windows.Forms.Label();
			this.m_groupLanguage = new System.Windows.Forms.GroupBox();
			this.m_groupLanguage.SuspendLayout();
			this.SuspendLayout();
			// 
			// m_listviewStages
			// 
			this.m_listviewStages.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.m_listviewStages.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.m_colAbbrev,
            this.m_colName});
			this.m_listviewStages.FullRowSelect = true;
			this.m_listviewStages.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			this.m_listviewStages.HideSelection = false;
			this.m_listviewStages.LabelEdit = true;
			this.m_listviewStages.Location = new System.Drawing.Point(3, 3);
			this.m_listviewStages.MultiSelect = false;
			this.m_listviewStages.Name = "m_listviewStages";
			this.m_listviewStages.Size = new System.Drawing.Size(342, 188);
			this.m_listviewStages.TabIndex = 1;
			this.m_listviewStages.UseCompatibleStateImageBehavior = false;
			this.m_listviewStages.View = System.Windows.Forms.View.Details;
			this.m_listviewStages.SelectedIndexChanged += new System.EventHandler(this.cmdSelectionChanged);
			// 
			// m_colAbbrev
			// 
			this.m_colAbbrev.Text = "Short Name";
			this.m_colAbbrev.Width = 119;
			// 
			// m_colName
			// 
			this.m_colName.Text = "Full Name";
			this.m_colName.Width = 179;
			// 
			// m_btnRestoreDefaults
			// 
			this.m_btnRestoreDefaults.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.m_btnRestoreDefaults.Location = new System.Drawing.Point(356, 133);
			this.m_btnRestoreDefaults.Name = "m_btnRestoreDefaults";
			this.m_btnRestoreDefaults.Size = new System.Drawing.Size(96, 56);
			this.m_btnRestoreDefaults.TabIndex = 6;
			this.m_btnRestoreDefaults.Text = "Restore Default Values...";
			this.m_btnRestoreDefaults.Click += new System.EventHandler(this.cmdRestoreDefaultValues);
			// 
			// m_btnAdd
			// 
			this.m_btnAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.m_btnAdd.Location = new System.Drawing.Point(356, 64);
			this.m_btnAdd.Name = "m_btnAdd";
			this.m_btnAdd.Size = new System.Drawing.Size(96, 23);
			this.m_btnAdd.TabIndex = 4;
			this.m_btnAdd.Text = "Add";
			this.m_btnAdd.Click += new System.EventHandler(this.cmdAddStage);
			// 
			// m_btnRemove
			// 
			this.m_btnRemove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.m_btnRemove.Location = new System.Drawing.Point(356, 93);
			this.m_btnRemove.Name = "m_btnRemove";
			this.m_btnRemove.Size = new System.Drawing.Size(96, 23);
			this.m_btnRemove.TabIndex = 5;
			this.m_btnRemove.Text = "Remove...";
			this.m_btnRemove.Click += new System.EventHandler(this.cmdRemove);
			// 
			// m_btnMoveUp
			// 
			this.m_btnMoveUp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.m_btnMoveUp.Image = ((System.Drawing.Image)(resources.GetObject("m_btnMoveUp.Image")));
			this.m_btnMoveUp.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.m_btnMoveUp.Location = new System.Drawing.Point(356, 3);
			this.m_btnMoveUp.Name = "m_btnMoveUp";
			this.m_btnMoveUp.Size = new System.Drawing.Size(96, 23);
			this.m_btnMoveUp.TabIndex = 2;
			this.m_btnMoveUp.Text = "Move Up";
			this.m_btnMoveUp.Click += new System.EventHandler(this.cmdMoveUp);
			// 
			// m_btnMoveDown
			// 
			this.m_btnMoveDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.m_btnMoveDown.Image = ((System.Drawing.Image)(resources.GetObject("m_btnMoveDown.Image")));
			this.m_btnMoveDown.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.m_btnMoveDown.Location = new System.Drawing.Point(356, 35);
			this.m_btnMoveDown.Name = "m_btnMoveDown";
			this.m_btnMoveDown.Size = new System.Drawing.Size(96, 23);
			this.m_btnMoveDown.TabIndex = 3;
			this.m_btnMoveDown.Text = "Move Down";
			this.m_btnMoveDown.Click += new System.EventHandler(this.cmdMoveDown);
			// 
			// m_labelLanguage
			// 
			this.m_labelLanguage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.m_labelLanguage.Location = new System.Drawing.Point(8, 64);
			this.m_labelLanguage.Name = "m_labelLanguage";
			this.m_labelLanguage.Size = new System.Drawing.Size(161, 23);
			this.m_labelLanguage.TabIndex = 36;
			this.m_labelLanguage.Text = "Default Language:";
			this.m_labelLanguage.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// m_comboLanguage
			// 
			this.m_comboLanguage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.m_comboLanguage.Location = new System.Drawing.Point(175, 64);
			this.m_comboLanguage.MaxDropDownItems = 15;
			this.m_comboLanguage.Name = "m_comboLanguage";
			this.m_comboLanguage.Size = new System.Drawing.Size(263, 21);
			this.m_comboLanguage.Sorted = true;
			this.m_comboLanguage.TabIndex = 7;
			this.m_comboLanguage.SelectedIndexChanged += new System.EventHandler(this.cmdNewDefaultLanguage);
			// 
			// m_labelInstructions
			// 
			this.m_labelInstructions.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.m_labelInstructions.Location = new System.Drawing.Point(3, 194);
			this.m_labelInstructions.Name = "m_labelInstructions";
			this.m_labelInstructions.Size = new System.Drawing.Size(444, 23);
			this.m_labelInstructions.TabIndex = 38;
			this.m_labelInstructions.Text = "Click twice, slowly, over a name in order to edit it.";
			this.m_labelInstructions.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// m_labelLanguageExplanation
			// 
			this.m_labelLanguageExplanation.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.m_labelLanguageExplanation.Location = new System.Drawing.Point(11, 250);
			this.m_labelLanguageExplanation.Name = "m_labelLanguageExplanation";
			this.m_labelLanguageExplanation.Size = new System.Drawing.Size(430, 45);
			this.m_labelLanguageExplanation.TabIndex = 39;
			this.m_labelLanguageExplanation.Text = "These Stage Names are used in the User Interface and in generating file names; yo" +
				"u should enter them in the language that is the most useful for the translator.";
			// 
			// m_groupLanguage
			// 
			this.m_groupLanguage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.m_groupLanguage.Controls.Add(this.m_labelLanguage);
			this.m_groupLanguage.Controls.Add(this.m_comboLanguage);
			this.m_groupLanguage.Location = new System.Drawing.Point(3, 234);
			this.m_groupLanguage.Name = "m_groupLanguage";
			this.m_groupLanguage.Size = new System.Drawing.Size(447, 96);
			this.m_groupLanguage.TabIndex = 40;
			this.m_groupLanguage.TabStop = false;
			this.m_groupLanguage.Text = "Language";
			// 
			// Page_TranslationStages
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.Controls.Add(this.m_labelLanguageExplanation);
			this.Controls.Add(this.m_labelInstructions);
			this.Controls.Add(this.m_btnMoveDown);
			this.Controls.Add(this.m_btnMoveUp);
			this.Controls.Add(this.m_btnRemove);
			this.Controls.Add(this.m_btnAdd);
			this.Controls.Add(this.m_btnRestoreDefaults);
			this.Controls.Add(this.m_listviewStages);
			this.Controls.Add(this.m_groupLanguage);
			this.Name = "Page_TranslationStages";
			this.Size = new System.Drawing.Size(453, 335);
			this.Load += new System.EventHandler(this.cmdLoad);
			this.m_groupLanguage.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

        // DlgPropertySheet overrides --------------------------------------------------------
		#region OAttr{g}: string ID
		public override string ID
		{
			get
			{
				return "idTranslationStages";
			}
		}
		#endregion
		#region Method: void ShowHelp()
		public override void ShowHelp()
		{
            HelpSystem.ShowTopic( HelpSystem.Topic.kTranslationStages );
		}
		#endregion
        #region Attr{g}: string TabText
        public override string Title
        {
            get
            {
                return Strings.PropDlgTab_TranslationStages;
            }
        }
        #endregion

        // Command Handlers ------------------------------------------------------------------
		#region Cmd: cmdLoad
		private void cmdLoad(object sender, System.EventArgs e)
		{
			// Label text in the appropriate language
            Control[] vExclude = { };
            LocDB.Localize(this, vExclude);

			// Populate the ComboBox
			PopulateLanguagesCombo();

			// Populate the List Control
			PopulateStagesList();
			SelectListItem(0);
		}
		#endregion
		#region Cmd: cmdRestoreDefaultValues
		private void cmdRestoreDefaultValues(object sender, System.EventArgs e)
			// Clear out whatever is currently in the TranslationStages list, and restore
			// it to the values we originally hard-coded.
		{
			// Make sure the user really wants to do this.
			if (!Messages.ConfirmResetTranslationStagesToDefaults())
				return;

			// Change the values in the underlying object
            DB.TeamSettings.TranslationStages.SetToFactoryDefault();

			// Update this dialog control
			PopulateStagesList();
			SelectListItem(0);
		}
		#endregion
		#region Cmd: cmdAddStage
		private void cmdAddStage(object sender, System.EventArgs e)
			// Adds a new TranslationStage, with default Abbrev and Name, to the
			// end of the list.
		{
			// Create a new TranslationStage with the default values from the 
			// language resources.
			TranslationStage stage = new TranslationStage(
                DB.TeamSettings.TranslationStages.GetAndIncrementNextID(), 
				Strings.NewTransStageAbbrev, 
				Strings.NewTransStageName);

			// Attempt to append it; adding will not be permitted if there is a
			// duplicate in the list (e.g., the user has already added one with these
			// default values, and neglectede to change it.
            if (false == DB.TeamSettings.TranslationStages.Append(stage))
				return;

			// Repopulate the listview control, and select the new object
			PopulateStagesList();
            SelectListItem(DB.TeamSettings.TranslationStages.Count - 1);
		}
		#endregion
		#region Cmd: cmdRemove
		private void cmdRemove(object sender, System.EventArgs e)
		{
			// Get the current selection
			TranslationStage stage = CurrentSelection;
			if (null == stage)
				return;

			// We don't permit the Drafting stage to be removed, as we (1) want to
			// make sure there is always something in the list, (2) it is a reasonable
			// default that should exist in all translation processes.
			if (stage.ID == BookStages.c_idDraft)
				return;

			// Make certain the user really wants to do this, as it could leave
			// existing books without a stage (in which case they revert to
			// "Draft" or whatever is first in the list.
			if (! Messages.ConfirmRemoveTranslationStage( stage.Name ) )
				return;

			// Remove the item
			int i = CurrentSelectionIndex;
            DB.TeamSettings.TranslationStages.Remove(stage);
			PopulateStagesList();

			// Select the item in the same position in the list, if possible
			i = Math.Min( i, LVStages.Items.Count - 1);
			SelectListItem(i);
		}
		#endregion
		#region Cmd: cmdSelectionChanged
		private void cmdSelectionChanged(object sender, System.EventArgs e)
			// Change the enabling of various controls, dependent on what is currently
			// selected.
		{
			ButtonEnabling();
		}
		#endregion
		#region Cmd: cmdMoveUp
		private void cmdMoveUp(object sender, System.EventArgs e)
		{
			// Move the object in the underlying
			int i = CurrentSelectionIndex;
			if (i < 1)
				return;
            DB.TeamSettings.TranslationStages.MoveTo(i, i - 1);

			// Update the combo
			PopulateStagesList();
			SelectListItem( i - 1 );
		}
		#endregion
		#region Cmd: cmdMoveDown
		private void cmdMoveDown(object sender, System.EventArgs e)
		{
			// Move the object in the underlying
			int i = CurrentSelectionIndex;
            if (i >= DB.TeamSettings.TranslationStages.Count - 1)
				return;
            DB.TeamSettings.TranslationStages.MoveTo(i, i + 1);

			// Update the combo
			PopulateStagesList();
			SelectListItem( i + 1 );
		}
		#endregion
		#region Cmd: cmdNewDefaultLanguage
		private void cmdNewDefaultLanguage(object sender, System.EventArgs e)
		{
			// Get strings representing the Current and Proposed languages
			string sCurrent = DB.TeamSettings.FileNameLanguage;
			string sProposed = DefaultLanguage.Text;

			// If these are the same, then there is nothing left to do
			if (sCurrent == sProposed)
				return;

			// See if the user is serious about doing this; if not, then change
			// it back to what it was previously.
			if (false == Messages.ConfirmChangeFileLanguage( sProposed ))
			{
				DefaultLanguage.Text = sCurrent;
				return;
			}

			// Set the FileNameLanguage to whatever the user just chose
			DB.TeamSettings.FileNameLanguage = sProposed;

			// Update the TranslationStages with the new language strings
            DB.TeamSettings.TranslationStages.UpdateFactoryLanguage();

			// Rebuild the list with these values
			int i = CurrentSelectionIndex;
			PopulateStagesList();
			SelectListItem(i);
		}
		#endregion
		#region Cmd: cmdSubItemEdited
		private void cmdSubItemEdited(object sender, JW_SubItemEditedArgs e)
		{
			// If New is the same as Old, then there is no need to do anything.
			if (e.OldText == e.NewText)
				return;

			// If the proposed NewText aldeady exists in the master list, then
			// we don't want to permit the renaming to happen, as it would cause
			// a duplicate.
            if (e.ListViewItemIndex == 0 && null != DB.TeamSettings.TranslationStages.GetFromAbbrev(e.NewText))
			{
				Messages.DuplicateStagesNotAllowed();
				e.SaveChanges = false;
				return;
			}
            if (e.ListViewItemIndex == 1 && null != DB.TeamSettings.TranslationStages.GetFromName(e.NewText))
			{
				Messages.DuplicateStagesNotAllowed();
				e.SaveChanges = false;
				return;
			}

			// Replace the item in the list
			TranslationStage stage = CurrentSelection;
			if (null == stage)
			{
				e.SaveChanges = false;
				return;
			}
			if (e.ListViewItemIndex == 0)
				stage.Abbrev = e.NewText;
			else
				stage.Name = e.NewText;
		}
		#endregion

		// Methods ---------------------------------------------------------------------------
		#region Method: void PopulateStagesList()
		void PopulateStagesList()
		{
			LVStages.Items.Clear();

            foreach (TranslationStage stage in DB.TeamSettings.TranslationStages.TranslationStages)
			{
				// Recover from missing strings
				if (stage.Abbrev.Length == 0)
					stage.Abbrev = stage.ID.ToString();
				if (stage.Name.Length == 0)
					stage.Name = stage.ID.ToString();

				// Place the stage info into a row
				string[] v = new string[2];
				v[0] = stage.Abbrev;
				v[1] = stage.Name;
				ListViewItem lvi = new ListViewItem(v);
				LVStages.Items.Add(lvi);
			}
		}
		#endregion
		#region Method: void PopulateLanguagesCombo()
		void PopulateLanguagesCombo()
		{
			// Remove anythihng that might have previously been there
			DefaultLanguage.Items.Clear();

			// Set the text to the current value in TeamSettings (For some reason
			// I don't understand, I need to set the Text first, and then add
			// the Items; otherwise I'm getting a crash.)
            DefaultLanguage.Text = DB.TeamSettings.FileNameLanguage;

			// Add the alternatives we currently support
            DefaultLanguage.Items.Add("English");
            foreach (LocLanguage language in LocDB.DB.Languages)
                DefaultLanguage.Items.Add(language.Name);
		}
		#endregion
		#region Method: void SelectListItem(int i)
		void SelectListItem(int i)
		{
			if (LVStages.Items.Count > i)
				LVStages.Items[i].Selected = true;
		}
		#endregion
		#region Method: void ButtonEnabling()
		void ButtonEnabling()
		{
			// Enable Everything
			m_btnRemove.Enabled = true;
			m_btnMoveUp.Enabled = true;
			m_btnMoveDown.Enabled = true;

			// Get the current selection
			TranslationStage stage = CurrentSelection;
			if (null == stage)
				return;

			// Disable the Remove button if Drafting is selection
			if (stage.ID == BookStages.c_idDraft)
				m_btnRemove.Enabled = false;

			// Can't MoveUp if the first item is selected
			if (CurrentSelectionIndex == 0)
				m_btnMoveUp.Enabled = false;

			// Can't MoveDn if the last item is selected
			if (CurrentSelectionIndex == LVStages.Items.Count - 1)
				m_btnMoveDown.Enabled = false;

		}
		#endregion
	}
	#endregion

}
