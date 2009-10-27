#region ***** Page_SetupFT.cs *****
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
using OurWordData;
using OurWordData.DataModel;
using OurWord;
using OurWord.Dialogs;
using OurWord.Layouts;
#endregion
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
        #region Attr{g}: List<string> Languages
        List<string> Languages
        {
            get
            {
                Debug.Assert(null != m_vLanguages);
                return m_vLanguages;
            }
        }
        List<string> m_vLanguages;
        #endregion
        #region Attr{g}: string ChosenLanguage
        string ChosenLanguage
        {
            get
            {
                return m_comboChooseLanguage.Text;
            }
        }
        #endregion

        // Scaffolding -----------------------------------------------------------------------
		#region Dialog Controls
        protected Label m_lblDefinition;
        private Label m_labelInstructions;
        private Button m_btnInitialize;
        private ComboBox m_comboChooseLanguage;
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
            this.m_labelInstructions = new System.Windows.Forms.Label();
            this.m_btnInitialize = new System.Windows.Forms.Button();
            this.m_comboChooseLanguage = new System.Windows.Forms.ComboBox();
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
            // m_labelInstructions
            // 
            this.m_labelInstructions.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.m_labelInstructions.Location = new System.Drawing.Point(6, 64);
            this.m_labelInstructions.Name = "m_labelInstructions";
            this.m_labelInstructions.Size = new System.Drawing.Size(459, 38);
            this.m_labelInstructions.TabIndex = 3;
            this.m_labelInstructions.Text = "Enter the name of the language if you wish to create a new one, or choose an exis" +
                "ting one from the list. Then click the Initialize button when you are ready.";
            this.m_labelInstructions.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // m_btnInitialize
            // 
            this.m_btnInitialize.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.m_btnInitialize.Location = new System.Drawing.Point(318, 119);
            this.m_btnInitialize.Name = "m_btnInitialize";
            this.m_btnInitialize.Size = new System.Drawing.Size(75, 23);
            this.m_btnInitialize.TabIndex = 5;
            this.m_btnInitialize.Text = "Initialize";
            this.m_btnInitialize.UseVisualStyleBackColor = true;
            this.m_btnInitialize.Click += new System.EventHandler(this.cmdInitializeTranslation);
            // 
            // m_comboChooseLanguage
            // 
            this.m_comboChooseLanguage.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.m_comboChooseLanguage.DropDownHeight = 200;
            this.m_comboChooseLanguage.FormattingEnabled = true;
            this.m_comboChooseLanguage.IntegralHeight = false;
            this.m_comboChooseLanguage.Location = new System.Drawing.Point(93, 121);
            this.m_comboChooseLanguage.MaxDropDownItems = 15;
            this.m_comboChooseLanguage.Name = "m_comboChooseLanguage";
            this.m_comboChooseLanguage.Size = new System.Drawing.Size(219, 21);
            this.m_comboChooseLanguage.Sorted = true;
            this.m_comboChooseLanguage.TabIndex = 6;
            this.m_comboChooseLanguage.SelectedIndexChanged += new System.EventHandler(this.cmdLanguageTextChanged);
            this.m_comboChooseLanguage.TextUpdate += new System.EventHandler(this.cmdLanguageTextChanged);
            // 
            // Page_SetupFT
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.m_comboChooseLanguage);
            this.Controls.Add(this.m_btnInitialize);
            this.Controls.Add(this.m_labelInstructions);
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
            Control[] vExclude = { m_lblDefinition };
            LocDB.Localize(this, vExclude);
		}
		#endregion

		// Command Handlers ------------------------------------------------------------------
		#region Handler: cmdLoad
		private void cmdLoad(object sender, System.EventArgs e)
		{
			// Label text in the appropriate language
			Localization();

            // Get the list of languages
            var clusterInfo = ClusterList.FindClusterInfo(DB.TeamSettings.DisplayName);
            if (null != clusterInfo)
                m_vLanguages = clusterInfo.GetClusterLanguageList(false);

            // Populate the combo
            m_comboChooseLanguage.Items.Clear();
            foreach (string s in Languages)
                m_comboChooseLanguage.Items.Add(s);

            // Don't enable the button until we have something in it
            m_btnInitialize.Enabled = false;
        }
		#endregion
        #region Cmd: cmdLanguageTextChanged
        private void cmdLanguageTextChanged(object sender, EventArgs e)
        {
            m_btnInitialize.Enabled = !string.IsNullOrEmpty(ChosenLanguage);
        }
        #endregion
        #region Cmd: cmdInitializeTranslation
        private void cmdInitializeTranslation(object sender, EventArgs e)
        {
            // Create and save the new translation
            Translation = new DTranslation(ChosenLanguage);
            Translation.WriteToFile(new NullProgress());
            DB.Project.WriteToFile(new NullProgress());

            // Read the books, if any, from the translation's folder
            Translation.PopulateBookListFromFolder();

            // Switch to the translations page
            ParentDlg.InitNavigation(Page_Translation.ComputeID(Translation.DisplayName));
        }
        #endregion
    }

    #region Class: Page_SetupFront : Page_SetupFT
    public class Page_SetupFront : Page_SetupFT
    {
        // Attrs -----------------------------------------------------------------------------
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

        // Scaffolding -----------------------------------------------------------------------
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
    #endregion
    #region Class: Page_SetupTarget : Page_SetupFT
    public class Page_SetupTarget : Page_SetupFT
    {
        // Attrs -----------------------------------------------------------------------------
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

        // Scaffolding -----------------------------------------------------------------------
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
    #endregion
}
