/**********************************************************************************************
 * Project: Our Word!
 * File:    DlgFilter.cs
 * Author:  John Wimbish
 * Created: 20 Feb 2006
 * Purpose: Supports Filters (menu item "Show Sections that Have..."
 * Legal:   Copyright (c) 2004-09, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
#region Using
using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.IO;
using Microsoft.Win32;

using JWTools;
using OurWordData;
using OurWordData.DataModel;
#endregion

namespace OurWord.Dialogs
{
	public class DialogFilter : System.Windows.Forms.Form
	{
        // Strings ---------------------------------------------------------------------------
        #region VAttr{g}: string AnyOneAreTrueText
        string AnyOneAreTrueText
        {
            get
            {
                return LocDB.GetValue(this, "strAnyOneOfThese", "Any One of These are True");
            }
        }
        #endregion
        #region VAttr{g}: string AllAreTrueText
        string AllAreTrueText
        {
            get
            {
                return LocDB.GetValue(this, "strAllOfThese", "All of These are True");
            }
        }
        #endregion

        // Attrs -----------------------------------------------------------------------------
		#region Attr{g}: bool OneOnly
		public bool OneOnly
		{
			get
			{
                if (m_comboQualifies.Text == AnyOneAreTrueText)
					return true;
				return false;
			}
		}
		#endregion
		#region Attr{g}: bool NothingIsChecked
		public bool NothingIsChecked
		{
			get
			{
				if (Filter_VernacularText)
					return false;
				if (Filter_FrontText)
					return false;
				if (Filter_VernacularBT)
					return false;
				if (Filter_UntranslatedText)
					return false;
				if (Filter_MismatchedQuotes)
					return false;
				if (Filter_PictureWithCaption)
					return false;
				if (Filter_ParagraphHasQuote)
					return false;
				if (Filter_PunctuationProblem)
					return false;
				if (Filter_PictureCannotBeLocatedOnDisk)
					return false;
				return true;
			}
		}
		#endregion

		// A Word or Phrase
		#region VAttr{g}: bool Filter_VernacularText
		public bool Filter_VernacularText
		{
			get
			{
                return Filter_VernacularSearchString.Length > 0;
			}
		}
		#endregion
		#region Attr{g}: string Filter_VernacularSearchString
		public string Filter_VernacularSearchString
		{
			get
			{
				return m_textVernacular.Text;
			}
            set
            {
                m_textVernacular.Text = value;
            }
		}
		#endregion
		#region VAttr{g}: bool Filter_FrontText
		public bool Filter_FrontText
		{
			get
			{
                return Filter_FrontSearchString.Length > 0;
			}
		}
		#endregion
		#region Attr{g}: string Filter_FrontSearchString
		public string Filter_FrontSearchString
		{
			get
			{
				return m_textFront.Text;
			}
            set
            {
                m_textFront.Text = value;
            }
		}
		#endregion
		#region VAttr{g}: bool Filter_VernacularBT
		public bool Filter_VernacularBT
		{
			get
			{
                return Filter_VernacularBTSearchString.Length > 0;
			}
		}
		#endregion
		#region Attr{g}: string Filter_VernacularBTSearchString
		public string Filter_VernacularBTSearchString
		{
			get
			{
				return m_textBT.Text;
			}
            set
            {
                m_textBT.Text = value;
            }
		}
		#endregion

		// Possible Problems
		#region Attr{g}: bool Filter_UntranslatedText
		public bool Filter_UntranslatedText
		{
			get
			{
				return m_checkUntranslatedText.Checked;
			}
		}
		#endregion
		#region Attr{g}: bool Filter_MismatchedQuotes
		public bool Filter_MismatchedQuotes
		{
			get
			{
				return m_checkMismatchedQuotes.Checked;
			}
		}
		#endregion
		#region Attr{g}: bool Filter_PunctuationProblem
		public bool Filter_PunctuationProblem
		{
			get
			{
				return m_checkPunctuationProblem.Checked;
			}
		}
		#endregion
		#region Attr{g}: bool Filter_PictureCannotBeLocatedOnDisk
		public bool Filter_PictureCannotBeLocatedOnDisk
		{
			get
			{
				return m_checkCannotLocatePicture.Checked;
			}
		}
		#endregion

		// Structure
		#region Attr{g}: bool Filter_PictureWithCaption
		public bool Filter_PictureWithCaption
		{
			get
			{
				return m_checkPictureWithCaption.Checked;
			}
		}
		#endregion
		#region Attr{g}: bool Filter_ParagraphHasQuote
		public bool Filter_ParagraphHasQuote
		{
			get
			{
				return m_checkQuote.Checked;
			}
		}
		#endregion

		// Scaffolding -----------------------------------------------------------------------
		#region Constructor()
		public DialogFilter()
		{
			// Required for Windows Form Designer support
			InitializeComponent();

			// Do it when we construct, as Load gets repeatedly called
            m_comboQualifies.Text = AnyOneAreTrueText;
		}
		#endregion
		#region DIALOG CONTROLS
		private System.Windows.Forms.Label m_labelHeading;
		private System.Windows.Forms.Button m_btnHelp;
		private System.Windows.Forms.Button m_btnCancel;
		private System.Windows.Forms.Button m_btnOK;
		private System.Windows.Forms.Label m_labelQualifies;
		private System.Windows.Forms.ComboBox m_comboQualifies;

        private GroupBox m_groupWordOrPhrase;
        private TextBox m_textVernacular;
        private TextBox m_textFront;
        private TextBox m_textBT;

		private GroupBox m_groupProblems;
		private CheckBox m_checkUntranslatedText;
		private CheckBox m_checkMismatchedQuotes;
		private CheckBox m_checkPunctuationProblem;
		private CheckBox m_checkCannotLocatePicture;

		private GroupBox m_groupStructure;
		private CheckBox m_checkPictureWithCaption;
		private CheckBox m_checkQuote;
        private Label m_labelInTheVernacular;
        private Label m_labelInTheFront;
        private Label m_labelInTheBackTranslation;

		// Required designer variable.
		private System.ComponentModel.Container components = null;
		#endregion
		#region Method: void Dispose( bool disposing ) - Clean up any resources being used.
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
		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DialogFilter));
            this.m_labelHeading = new System.Windows.Forms.Label();
            this.m_checkPictureWithCaption = new System.Windows.Forms.CheckBox();
            this.m_btnHelp = new System.Windows.Forms.Button();
            this.m_btnCancel = new System.Windows.Forms.Button();
            this.m_btnOK = new System.Windows.Forms.Button();
            this.m_labelQualifies = new System.Windows.Forms.Label();
            this.m_comboQualifies = new System.Windows.Forms.ComboBox();
            this.m_checkMismatchedQuotes = new System.Windows.Forms.CheckBox();
            this.m_checkUntranslatedText = new System.Windows.Forms.CheckBox();
            this.m_textVernacular = new System.Windows.Forms.TextBox();
            this.m_textFront = new System.Windows.Forms.TextBox();
            this.m_textBT = new System.Windows.Forms.TextBox();
            this.m_groupWordOrPhrase = new System.Windows.Forms.GroupBox();
            this.m_labelInTheBackTranslation = new System.Windows.Forms.Label();
            this.m_labelInTheFront = new System.Windows.Forms.Label();
            this.m_labelInTheVernacular = new System.Windows.Forms.Label();
            this.m_groupProblems = new System.Windows.Forms.GroupBox();
            this.m_checkCannotLocatePicture = new System.Windows.Forms.CheckBox();
            this.m_checkPunctuationProblem = new System.Windows.Forms.CheckBox();
            this.m_checkQuote = new System.Windows.Forms.CheckBox();
            this.m_groupStructure = new System.Windows.Forms.GroupBox();
            this.m_groupWordOrPhrase.SuspendLayout();
            this.m_groupProblems.SuspendLayout();
            this.m_groupStructure.SuspendLayout();
            this.SuspendLayout();
            // 
            // m_labelHeading
            // 
            this.m_labelHeading.Location = new System.Drawing.Point(8, 8);
            this.m_labelHeading.Name = "m_labelHeading";
            this.m_labelHeading.Size = new System.Drawing.Size(376, 23);
            this.m_labelHeading.TabIndex = 0;
            this.m_labelHeading.Text = "Only show those sections that have....";
            this.m_labelHeading.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // m_checkPictureWithCaption
            // 
            this.m_checkPictureWithCaption.Location = new System.Drawing.Point(27, 259);
            this.m_checkPictureWithCaption.Name = "m_checkPictureWithCaption";
            this.m_checkPictureWithCaption.Size = new System.Drawing.Size(144, 24);
            this.m_checkPictureWithCaption.TabIndex = 30;
            this.m_checkPictureWithCaption.Text = "A picture with a caption";
            // 
            // m_btnHelp
            // 
            this.m_btnHelp.Image = ((System.Drawing.Image)(resources.GetObject("m_btnHelp.Image")));
            this.m_btnHelp.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.m_btnHelp.Location = new System.Drawing.Point(259, 355);
            this.m_btnHelp.Name = "m_btnHelp";
            this.m_btnHelp.Size = new System.Drawing.Size(75, 23);
            this.m_btnHelp.TabIndex = 52;
            this.m_btnHelp.Text = "Help...";
            this.m_btnHelp.Click += new System.EventHandler(this.cmdHelp);
            // 
            // m_btnCancel
            // 
            this.m_btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.m_btnCancel.Location = new System.Drawing.Point(171, 355);
            this.m_btnCancel.Name = "m_btnCancel";
            this.m_btnCancel.Size = new System.Drawing.Size(75, 23);
            this.m_btnCancel.TabIndex = 51;
            this.m_btnCancel.Text = "Turn Off";
            this.m_btnCancel.Click += new System.EventHandler(this.cmdTurnOff);
            // 
            // m_btnOK
            // 
            this.m_btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.m_btnOK.Location = new System.Drawing.Point(83, 355);
            this.m_btnOK.Name = "m_btnOK";
            this.m_btnOK.Size = new System.Drawing.Size(75, 23);
            this.m_btnOK.TabIndex = 50;
            this.m_btnOK.Text = "Turn On";
            // 
            // m_labelQualifies
            // 
            this.m_labelQualifies.Location = new System.Drawing.Point(11, 307);
            this.m_labelQualifies.Name = "m_labelQualifies";
            this.m_labelQualifies.Size = new System.Drawing.Size(136, 23);
            this.m_labelQualifies.TabIndex = 19;
            this.m_labelQualifies.Text = "The Section qualifies if:";
            this.m_labelQualifies.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // m_comboQualifies
            // 
            this.m_comboQualifies.Location = new System.Drawing.Point(147, 307);
            this.m_comboQualifies.Name = "m_comboQualifies";
            this.m_comboQualifies.Size = new System.Drawing.Size(240, 21);
            this.m_comboQualifies.TabIndex = 40;
            this.m_comboQualifies.Text = "Any one of these are true.";
            // 
            // m_checkMismatchedQuotes
            // 
            this.m_checkMismatchedQuotes.Location = new System.Drawing.Point(160, 16);
            this.m_checkMismatchedQuotes.Name = "m_checkMismatchedQuotes";
            this.m_checkMismatchedQuotes.Size = new System.Drawing.Size(208, 24);
            this.m_checkMismatchedQuotes.TabIndex = 21;
            this.m_checkMismatchedQuotes.Text = "Mismatched quotes";
            // 
            // m_checkUntranslatedText
            // 
            this.m_checkUntranslatedText.Location = new System.Drawing.Point(27, 171);
            this.m_checkUntranslatedText.Name = "m_checkUntranslatedText";
            this.m_checkUntranslatedText.Size = new System.Drawing.Size(136, 24);
            this.m_checkUntranslatedText.TabIndex = 20;
            this.m_checkUntranslatedText.Text = "Untranslated Text";
            // 
            // m_textVernacular
            // 
            this.m_textVernacular.Location = new System.Drawing.Point(168, 56);
            this.m_textVernacular.Name = "m_textVernacular";
            this.m_textVernacular.Size = new System.Drawing.Size(200, 20);
            this.m_textVernacular.TabIndex = 2;
            // 
            // m_textFront
            // 
            this.m_textFront.Location = new System.Drawing.Point(168, 80);
            this.m_textFront.Name = "m_textFront";
            this.m_textFront.Size = new System.Drawing.Size(200, 20);
            this.m_textFront.TabIndex = 4;
            // 
            // m_textBT
            // 
            this.m_textBT.Location = new System.Drawing.Point(168, 104);
            this.m_textBT.Name = "m_textBT";
            this.m_textBT.Size = new System.Drawing.Size(200, 20);
            this.m_textBT.TabIndex = 6;
            // 
            // m_groupWordOrPhrase
            // 
            this.m_groupWordOrPhrase.Controls.Add(this.m_labelInTheBackTranslation);
            this.m_groupWordOrPhrase.Controls.Add(this.m_labelInTheFront);
            this.m_groupWordOrPhrase.Controls.Add(this.m_labelInTheVernacular);
            this.m_groupWordOrPhrase.Location = new System.Drawing.Point(8, 40);
            this.m_groupWordOrPhrase.Name = "m_groupWordOrPhrase";
            this.m_groupWordOrPhrase.Size = new System.Drawing.Size(376, 96);
            this.m_groupWordOrPhrase.TabIndex = 31;
            this.m_groupWordOrPhrase.TabStop = false;
            this.m_groupWordOrPhrase.Text = "A Word or Phrase";
            // 
            // m_labelInTheBackTranslation
            // 
            this.m_labelInTheBackTranslation.Location = new System.Drawing.Point(13, 60);
            this.m_labelInTheBackTranslation.Name = "m_labelInTheBackTranslation";
            this.m_labelInTheBackTranslation.Size = new System.Drawing.Size(141, 23);
            this.m_labelInTheBackTranslation.TabIndex = 2;
            this.m_labelInTheBackTranslation.Text = "...In the Back Translation:";
            this.m_labelInTheBackTranslation.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // m_labelInTheFront
            // 
            this.m_labelInTheFront.Location = new System.Drawing.Point(13, 37);
            this.m_labelInTheFront.Name = "m_labelInTheFront";
            this.m_labelInTheFront.Size = new System.Drawing.Size(141, 23);
            this.m_labelInTheFront.TabIndex = 1;
            this.m_labelInTheFront.Text = "...In the Front Translation:";
            this.m_labelInTheFront.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // m_labelInTheVernacular
            // 
            this.m_labelInTheVernacular.Location = new System.Drawing.Point(13, 16);
            this.m_labelInTheVernacular.Name = "m_labelInTheVernacular";
            this.m_labelInTheVernacular.Size = new System.Drawing.Size(141, 23);
            this.m_labelInTheVernacular.TabIndex = 0;
            this.m_labelInTheVernacular.Text = "...In the Vernacular:";
            this.m_labelInTheVernacular.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // m_groupProblems
            // 
            this.m_groupProblems.Controls.Add(this.m_checkCannotLocatePicture);
            this.m_groupProblems.Controls.Add(this.m_checkPunctuationProblem);
            this.m_groupProblems.Controls.Add(this.m_checkMismatchedQuotes);
            this.m_groupProblems.Location = new System.Drawing.Point(11, 155);
            this.m_groupProblems.Name = "m_groupProblems";
            this.m_groupProblems.Size = new System.Drawing.Size(376, 72);
            this.m_groupProblems.TabIndex = 33;
            this.m_groupProblems.TabStop = false;
            this.m_groupProblems.Text = "Possible Problems:";
            // 
            // m_checkCannotLocatePicture
            // 
            this.m_checkCannotLocatePicture.Location = new System.Drawing.Point(160, 40);
            this.m_checkCannotLocatePicture.Name = "m_checkCannotLocatePicture";
            this.m_checkCannotLocatePicture.Size = new System.Drawing.Size(208, 24);
            this.m_checkCannotLocatePicture.TabIndex = 23;
            this.m_checkCannotLocatePicture.Text = "Picture Not on Disk";
            // 
            // m_checkPunctuationProblem
            // 
            this.m_checkPunctuationProblem.Location = new System.Drawing.Point(16, 40);
            this.m_checkPunctuationProblem.Name = "m_checkPunctuationProblem";
            this.m_checkPunctuationProblem.Size = new System.Drawing.Size(136, 24);
            this.m_checkPunctuationProblem.TabIndex = 22;
            this.m_checkPunctuationProblem.Text = "Punctuation Problem";
            // 
            // m_checkQuote
            // 
            this.m_checkQuote.Location = new System.Drawing.Point(160, 16);
            this.m_checkQuote.Name = "m_checkQuote";
            this.m_checkQuote.Size = new System.Drawing.Size(208, 24);
            this.m_checkQuote.TabIndex = 31;
            this.m_checkQuote.Text = "A Quote Paragraph";
            // 
            // m_groupStructure
            // 
            this.m_groupStructure.Controls.Add(this.m_checkQuote);
            this.m_groupStructure.Location = new System.Drawing.Point(11, 243);
            this.m_groupStructure.Name = "m_groupStructure";
            this.m_groupStructure.Size = new System.Drawing.Size(376, 48);
            this.m_groupStructure.TabIndex = 35;
            this.m_groupStructure.TabStop = false;
            this.m_groupStructure.Text = "Structure";
            // 
            // DialogFilter
            // 
            this.AcceptButton = this.m_btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.m_btnCancel;
            this.ClientSize = new System.Drawing.Size(394, 391);
            this.Controls.Add(this.m_textBT);
            this.Controls.Add(this.m_textFront);
            this.Controls.Add(this.m_textVernacular);
            this.Controls.Add(this.m_checkUntranslatedText);
            this.Controls.Add(this.m_comboQualifies);
            this.Controls.Add(this.m_labelQualifies);
            this.Controls.Add(this.m_btnHelp);
            this.Controls.Add(this.m_btnCancel);
            this.Controls.Add(this.m_btnOK);
            this.Controls.Add(this.m_checkPictureWithCaption);
            this.Controls.Add(this.m_labelHeading);
            this.Controls.Add(this.m_groupWordOrPhrase);
            this.Controls.Add(this.m_groupProblems);
            this.Controls.Add(this.m_groupStructure);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DialogFilter";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Only Show Sections That";
            this.Load += new System.EventHandler(this.cmdLoad);
            this.Closing += new System.ComponentModel.CancelEventHandler(this.cmdClosing);
            this.m_groupWordOrPhrase.ResumeLayout(false);
            this.m_groupProblems.ResumeLayout(false);
            this.m_groupStructure.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		// Command Handlers ------------------------------------------------------------------
		#region Cmd: cmdLoad
		private void cmdLoad(object sender, System.EventArgs e)
		{
			// Localization
            Control[] vExclude = { m_comboQualifies };
            LocDB.Localize(this, vExclude);

			// Set up the combo box
			m_comboQualifies.Items.Clear();
            m_comboQualifies.Items.Add(AnyOneAreTrueText);
            m_comboQualifies.Items.Add(AllAreTrueText);
		}
		#endregion
		#region Cmd: cmdClosing
		private void cmdClosing(object sender, System.ComponentModel.CancelEventArgs e)
		{
		}
		#endregion
		#region Cmd: cmdHelp
		private void cmdHelp(object sender, System.EventArgs e)
		{
            HelpSystem.ShowTopic(HelpSystem.Topic.kFilters);
		}
		#endregion
        #region Cmd: cmdTurnOff - when the Turn Off button is clicked, clear out the text search boxes
        private void cmdTurnOff(object sender, EventArgs e)
        {
            Filter_VernacularSearchString = "";
            Filter_VernacularBTSearchString = "";
            Filter_FrontSearchString = "";
        }
        #endregion

    }
}
