#region ***** Page_TranslationStages.cs *****
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
using OurWordData;
using OurWordData.DataModel;
using OurWord;
using OurWord.Dialogs;
using OurWord.Layouts;
#endregion
#endregion

namespace OurWord.Dialogs
{
    public class Page_TranslationStages : DlgPropertySheet
	{
        // Checked List Box ------------------------------------------------------------------
        #region Attr{g}: CheckedListBox StagesCheckList
        CheckedListBox StagesCheckList
        {
            get
            {
                return m_StagesCheckList;
            }
        }
        #endregion
		#region VAttr{g}: Stage CurrentSelection
		Stage CurrentSelection
		{
			get
			{
                if (StagesCheckList.SelectedItems.Count != 1)
                    return null;

                return StagesCheckList.SelectedItems[0] as Stage;
			}
		}
		#endregion
        #region VAttr{g/s}: int SelectedListItem
        int SelectedListItem
        {
            get
            {
                if (CurrentSelection == null)
                    return -1;

                for (int i = 0; i < StagesCheckList.Items.Count; i++)
                {
                    if (CurrentSelection == StagesCheckList.Items[i])
                        return i;
                }

                Debug.Assert(false);
                return -1;
            }
            set
            {
                if (value < 0 || value > StagesCheckList.Items.Count)
                    return;

                StagesCheckList.SelectedItems.Clear();
                StagesCheckList.SelectedItems.Add(StagesCheckList.Items[value]);
            }
        }
        #endregion
        #region Method: void PopulateStagesCheckList(stageToSelect)
        void PopulateStagesCheckList(Stage stageToSelect)
		{
            StagesCheckList.Items.Clear();
            StagesCheckList.SelectedItems.Clear();

            foreach (Stage stage in DB.TeamSettings.Stages)
            {
                StagesCheckList.Items.Add(stage, stage.UsedInThisProject);
                if (stage == stageToSelect)
                    StagesCheckList.SelectedItems.Add(stage);
            }

            if (null == CurrentSelection)
                SelectedListItem = 0;

            ButtonEnabling();
		}
		#endregion

		// Scaffolding -----------------------------------------------------------------------
		#region DIALOG CONTROLS

        private Button m_btnRestoreDefaults;
		private Button m_btnMoveUp;
        private Button m_btnMoveDown;
        private CheckedListBox m_StagesCheckList;

		// Required designer variable.
		private System.ComponentModel.Container components = null;
		#endregion
		#region Constructor()
        public Page_TranslationStages(DialogProperties _ParentDlg)
            : base(_ParentDlg)
		{
			InitializeComponent();
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
            this.m_btnRestoreDefaults = new System.Windows.Forms.Button();
            this.m_btnMoveUp = new System.Windows.Forms.Button();
            this.m_btnMoveDown = new System.Windows.Forms.Button();
            this.m_StagesCheckList = new System.Windows.Forms.CheckedListBox();
            this.SuspendLayout();
            // 
            // m_btnRestoreDefaults
            // 
            this.m_btnRestoreDefaults.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.m_btnRestoreDefaults.Location = new System.Drawing.Point(356, 68);
            this.m_btnRestoreDefaults.Name = "m_btnRestoreDefaults";
            this.m_btnRestoreDefaults.Size = new System.Drawing.Size(96, 56);
            this.m_btnRestoreDefaults.TabIndex = 6;
            this.m_btnRestoreDefaults.Text = "Restore Default Values...";
            this.m_btnRestoreDefaults.Click += new System.EventHandler(this.cmdRestoreDefaultValues);
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
            this.m_btnMoveDown.Location = new System.Drawing.Point(356, 30);
            this.m_btnMoveDown.Name = "m_btnMoveDown";
            this.m_btnMoveDown.Size = new System.Drawing.Size(96, 23);
            this.m_btnMoveDown.TabIndex = 3;
            this.m_btnMoveDown.Text = "Move Down";
            this.m_btnMoveDown.Click += new System.EventHandler(this.cmdMoveDown);
            // 
            // m_StagesCheckList
            // 
            this.m_StagesCheckList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.m_StagesCheckList.FormattingEnabled = true;
            this.m_StagesCheckList.Location = new System.Drawing.Point(3, 5);
            this.m_StagesCheckList.Name = "m_StagesCheckList";
            this.m_StagesCheckList.Size = new System.Drawing.Size(342, 319);
            this.m_StagesCheckList.TabIndex = 7;
            this.m_StagesCheckList.SelectedIndexChanged += new System.EventHandler(this.cmdSelectionChanged);
            // 
            // Page_TranslationStages
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.m_StagesCheckList);
            this.Controls.Add(this.m_btnMoveDown);
            this.Controls.Add(this.m_btnMoveUp);
            this.Controls.Add(this.m_btnRestoreDefaults);
            this.Name = "Page_TranslationStages";
            this.Size = new System.Drawing.Size(453, 335);
            this.Load += new System.EventHandler(this.cmdLoad);
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
        #region OMethod: bool HarvestChanges()
        public override bool HarvestChanges()
            // Get every stage's check state
        {
            for(int i=0; i<StagesCheckList.Items.Count; i++)
            {
                var stage = StagesCheckList.Items[i] as Stage;
                stage.UsedInThisProject = StagesCheckList.GetItemChecked(i);
            }
            return true;
        }
        #endregion

        // Command Handlers ------------------------------------------------------------------
		#region Cmd: cmdLoad
		private void cmdLoad(object sender, System.EventArgs e)
		{
			// Label text in the appropriate language
            Control[] vExclude = { };
            LocDB.Localize(this, vExclude);

			// Populate the List Control
			PopulateStagesCheckList(null);
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
            DB.TeamSettings.Stages.InitializeToFactoryDefaults();

			// Update this dialog control
			PopulateStagesCheckList(null);
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
            var stage = CurrentSelection;
            if (null == stage)
                return;

            // Ask the udnerlying list to make the move
            DB.TeamSettings.Stages.MoveUp(stage);

            // Re-do the list
            PopulateStagesCheckList(stage);
		}
		#endregion
		#region Cmd: cmdMoveDown
		private void cmdMoveDown(object sender, System.EventArgs e)
		{
            var stage = CurrentSelection;
            if (null == stage)
                return;

            // Ask the udnerlying list to make the move
            DB.TeamSettings.Stages.MoveDown(stage);

            // Re-do the list
            PopulateStagesCheckList(stage);
		}
		#endregion

        // Methods ---------------------------------------------------------------------------
		#region Method: void ButtonEnabling()
		void ButtonEnabling()
		{
			// Enable Everything
			m_btnMoveUp.Enabled = true;
			m_btnMoveDown.Enabled = true;

			// Can't MoveUp if the first item is selected
			if (SelectedListItem == 0)
				m_btnMoveUp.Enabled = false;

			// Can't MoveDn if the last item is selected
			if (SelectedListItem == StagesCheckList.Items.Count - 1)
				m_btnMoveDown.Enabled = false;
		}
		#endregion

	}

}
