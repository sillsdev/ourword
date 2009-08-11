/**********************************************************************************************
 * Project: Our Word!
 * File:    Page_SetupFT.cs
 * Author:  John Wimbish
 * Created: 05 Jan 2005
 * Purpose: Create or Open settings for the Front/Target (hence "FT").
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
using OurWord.Layouts;
#endregion

namespace OurWord.Dialogs
{
    public class Page_SetupFT : DlgPropertySheet
	{
        // Attrs -----------------------------------------------------------------------------
        #region Attr{g/s}: DTranslation Translation - the trans (front or target) we're working with
        protected virtual DTranslation Translation
		{
			get
			{
                Debug.Assert(false);
                return null;
			}
			set
			{
                Debug.Assert(false);
			}
		}
		#endregion

		// Scaffolding -----------------------------------------------------------------------
		#region Dialog Controls
		private   Button m_btnCreate;
		private   Button m_btnOpen;
		protected Label  m_lblCreate;
		protected Label  m_lblOpen;
        private Label m_lblActions;
        protected Label m_lblDefinition;
		private System.ComponentModel.Container components = null;
		#endregion
		#region Constructor()
		public Page_SetupFT(DialogProperties _dlg)
            : base(_dlg)
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();
		}
		#endregion
		#region Method: void Dispose(...)
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Page_SetupFT));
			this.m_lblDefinition = new System.Windows.Forms.Label();
			this.m_btnCreate = new System.Windows.Forms.Button();
			this.m_btnOpen = new System.Windows.Forms.Button();
			this.m_lblCreate = new System.Windows.Forms.Label();
			this.m_lblOpen = new System.Windows.Forms.Label();
			this.m_lblActions = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// m_lblDefinition
			// 
			this.m_lblDefinition.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.m_lblDefinition.Location = new System.Drawing.Point(3, 0);
			this.m_lblDefinition.Name = "m_lblDefinition";
			this.m_lblDefinition.Size = new System.Drawing.Size(462, 64);
			this.m_lblDefinition.TabIndex = 0;
			this.m_lblDefinition.Text = resources.GetString("m_lblDefinition.Text");
			// 
			// m_btnCreate
			// 
			this.m_btnCreate.Location = new System.Drawing.Point(6, 91);
			this.m_btnCreate.Name = "m_btnCreate";
			this.m_btnCreate.Size = new System.Drawing.Size(122, 23);
			this.m_btnCreate.TabIndex = 1;
			this.m_btnCreate.Text = "Create New...";
			this.m_btnCreate.Click += new System.EventHandler(this.cmdCreate);
			// 
			// m_btnOpen
			// 
			this.m_btnOpen.Location = new System.Drawing.Point(6, 149);
			this.m_btnOpen.Name = "m_btnOpen";
			this.m_btnOpen.Size = new System.Drawing.Size(122, 23);
			this.m_btnOpen.TabIndex = 2;
			this.m_btnOpen.Text = "Open Existing...";
			this.m_btnOpen.Click += new System.EventHandler(this.cmdOpen);
			// 
			// m_lblCreate
			// 
			this.m_lblCreate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.m_lblCreate.Location = new System.Drawing.Point(137, 91);
			this.m_lblCreate.Name = "m_lblCreate";
			this.m_lblCreate.Size = new System.Drawing.Size(328, 58);
			this.m_lblCreate.TabIndex = 3;
			this.m_lblCreate.Text = "Start from scratch to initialize settings for your front translation. You will be" +
				" presented with blank settings, and will need to enter the name of the translati" +
				"on, its books, etc.";
			// 
			// m_lblOpen
			// 
			this.m_lblOpen.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.m_lblOpen.Location = new System.Drawing.Point(137, 149);
			this.m_lblOpen.Name = "m_lblOpen";
			this.m_lblOpen.Size = new System.Drawing.Size(328, 69);
			this.m_lblOpen.TabIndex = 4;
			this.m_lblOpen.Text = "Make use of existing settings for your front translation. If you defined the tran" +
				"slation in another project, then you can reuse it here, rather than entering the" +
				" information all over again.";
			// 
			// m_lblActions
			// 
			this.m_lblActions.Location = new System.Drawing.Point(3, 64);
			this.m_lblActions.Name = "m_lblActions";
			this.m_lblActions.Size = new System.Drawing.Size(444, 23);
			this.m_lblActions.TabIndex = 7;
			this.m_lblActions.Text = "Available UndoStack:";
			this.m_lblActions.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// Page_SetupFT
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.Controls.Add(this.m_lblActions);
			this.Controls.Add(this.m_lblOpen);
			this.Controls.Add(this.m_lblCreate);
			this.Controls.Add(this.m_btnOpen);
			this.Controls.Add(this.m_btnCreate);
			this.Controls.Add(this.m_lblDefinition);
			this.Name = "Page_SetupFT";
			this.Size = new System.Drawing.Size(468, 321);
			this.Load += new System.EventHandler(this.cmdLoad);
			this.ResumeLayout(false);

		}
		#endregion
		#region Method: virtual void Localization()
		virtual protected void Localization()
		{
            // We exclude those items whose values depend on the subclasses;
            // these localizations are obtained from the strings section.
            Control[] vExclude = { m_lblDefinition, m_lblCreate, m_lblOpen };
            LocDB.Localize(this, vExclude);
		}
		#endregion

		// Command Handlers ------------------------------------------------------------------
		#region Handler: cmdLoad
		private void cmdLoad(object sender, System.EventArgs e)
		{
			// Label text in the appropriate language
			Localization();
		}
		#endregion
		#region Handler: cmdCreate - create a blank set of translation settings
		private void cmdCreate(object sender, System.EventArgs e)
		{
			/***
			// Make sure the user understands that this will remove his current Front/Target
			// Translation from the project.
			// Note: OVERRIDE ConfirmTranslationReplacement() in subclass
            if (null != Translation && !ConfirmTranslationReplacement())
			    return;

			// Launch the dialog to prompt for crucial information: Name and
			// Settings Filename. The user has the option to abort from this dlg.
			DialogCreateTranslation dlg = new DialogCreateTranslation();
			if (DialogResult.OK != dlg.ShowDialog(this))
				return;

			// Create the DTranslation object, initialize it, place it into the Project
			// settings.
            Translation = new DTranslation(dlg.TranslationName, "Latin", "Latin");
            Translation.AbsolutePathName = dlg.SettingsPath;
            Translation.LanguageAbbrev = dlg.Abbreviation;

			// Tell the parent dialog to show the settings page for the new translation
			ParentDlg.InitNavTree(Page_Translation.ComputeID(Translation.DisplayName));
			***/
        }
		#endregion
		#region Handler: cmdOpen - open an existing translation settings file
		private void cmdOpen(object sender, System.EventArgs e)
		{
			/***
			// Make sure the user understands that this will remove his current Front
			// Translation from the project.
			// Note: OVERRIDE ConfirmTranslationReplacement() in subclass
            if (null != Translation && !ConfirmTranslationReplacement())
					return;

			// Launch the dialog to prompt for the Settings Filename. The user has the 
			// option to abort from this dlg.
			DialogOpenTranslation dlg = new DialogOpenTranslation();
			if (DialogResult.OK != dlg.ShowDialog())
				return;

			// Read in the translation object
			Translation = new DTranslation();
            Translation.AbsolutePathName = dlg.SettingsPath;  // First so we know what to load
            Translation.Load();
            Translation.AbsolutePathName = dlg.SettingsPath;  // Second because Load overwrote it

			// Tell the parent dialog to show the settings page for the newly-opened translation
			ParentDlg.InitNavTree(Page_Translation.ComputeID(Translation.DisplayName));
			***/
        }
		#endregion

		// Must be overridden in the subclass: supports Create and Open actions --------------
		#region Method: virtual bool ConfirmTranslationReplacement()
		protected virtual bool ConfirmTranslationReplacement()
		{
			Debug.Assert(false);
			return false;
		}
		#endregion
	}

	public class Page_SetupFront : Page_SetupFT
    {
        #region Attr{g/s}: override DTranslation Translation
        protected override DTranslation Translation
        {
            get
            {
                if (null != DB.Project)
                    return DB.Project.FrontTranslation;
                return null;
            }
            set
            {
                if (null != DB.Project)
                    DB.Project.FrontTranslation = value;
            }
        }
        #endregion

        #region Constructor(DlgProperties)
        public Page_SetupFront(DialogProperties _dlg)
            : base(_dlg)
		{
		}
		#endregion
		#region OMethod: void Localization()
		override protected void Localization()
		{
			// Pick up the common stuff
			base.Localization();

			// Front-Translation dependent
			m_lblDefinition.Text  = Strings.SetupFT_FrontDefinition;
            m_lblCreate.Text = Strings.SetupFT_FrontCreateExpl;
            m_lblOpen.Text = Strings.SetupFT_FrontOpenExpl;
		}
		#endregion

		// Overrides to support Create and Open actions --------------------------------------
		#region Method: override bool ConfirmTranslationReplacement()
		protected override bool ConfirmTranslationReplacement()
		{
            Debug.Assert(null != Translation);
            return Messages.ConfirmFrontReplace(Translation.DisplayName);
		}
		#endregion

        // DlgPropertySheet overrides --------------------------------------------------------
		public const string c_sID = "idSetupFront";
		#region OAttr{g}: string ID
		public override string ID
		{
			get
			{
				return c_sID;
			}
		}
		#endregion
		#region Attr{g}: string TabText
        public override string Title
        {
            get
            {
                return "Front Translation";
            }
        }
        #endregion
	}

	public class Page_SetupTarget : Page_SetupFT
    {
        #region Attr{g/s}: override DTranslation Translation
        protected override DTranslation Translation
        {
            get
            {
                if (null != DB.Project)
                    return DB.Project.TargetTranslation ;
                return null;
            }
            set
            {
                if (null != DB.Project)
                    DB.Project.TargetTranslation = value;
            }
        }
        #endregion

        #region Constructor(DlgProperties)
		public Page_SetupTarget(DialogProperties _dlg)
            : base(_dlg)
		{
		}
		#endregion
		#region OMethod: void Localization()
		override protected void Localization()
		{
			// Pick up the common stuff
			base.Localization();

			// Target-Translation dependent
            m_lblDefinition.Text = Strings.SetupFT_TargetDefinition;
            m_lblCreate.Text = Strings.SetupFT_TargetCreateExpl;
            m_lblOpen.Text = Strings.SetupFT_TargetOpenExpl;
        }
		#endregion

		// Overrides to support Create and Open actions --------------------------------------
		#region Method: override bool ConfirmTranslationReplacement()
		protected override bool ConfirmTranslationReplacement()
		{
            Debug.Assert(null != Translation);
            return Messages.ConfirmTargetReplace(Translation.DisplayName);
		}
		#endregion

        // DlgPropertySheet overrides --------------------------------------------------------
		public const string c_sID = "idSetupTarget";
		#region OAttr{g}: string ID
		public override string ID
		{
			get
			{
				return c_sID;
			}
		}
		#endregion
		#region Attr{g}: string TabText
        public override string Title
        {
            get
            {
                return "Target Translation";
            }
        }
        #endregion
	}

}
