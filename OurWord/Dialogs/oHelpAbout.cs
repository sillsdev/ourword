/**********************************************************************************************
 * Project: Our Word!
 * File:    oHelpAbout.cs
 * Author:  John Wimbish
 * Created: 01 Dec 2003
 * Purpose: Provides the About dialog for the app. This dialog gives a summary about the app,
 *            contact info for The Seed Company, copyright notice; and permaps most importantly
 *            the version information.
 * Legal:   Copyright (c) 2005-08, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
#region Using
using System;
using System.Collections;
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
using JWdb;
using OurWord.DataModel;
#endregion

namespace OurWord
{
	public class DialogHelpAbout : System.Windows.Forms.Form
	{
		#region Form Controls
		private PictureBox m_imgSeedCompanyLogo;
		private Label      m_lblTheSeedCompany;
		private Label      m_lblProgramName;
		private Label      m_lblDescription;
		private Label      m_lblTSCInfo;
		private Button     m_btnClose;
		private Label      m_lblVersion;
		private LinkLabel  m_link;
		private Label      m_lblCopyright;
		#endregion
        private PictureBox m_OurWordIcon;

        #region Required Designer Variable
        private System.ComponentModel.Container components = null;
		#endregion

		#region Constructor
		public DialogHelpAbout()
			// Constructor
		{
			InitializeComponent();
		}
		#endregion

		#region Method: void Dispose(...) - Clean up any resources being used.
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

		#region Windows Form Designer generated code
		/// -----------------------------------------------------------------------------------
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		/// -----------------------------------------------------------------------------------
		private void InitializeComponent()
		{
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DialogHelpAbout));
            this.m_imgSeedCompanyLogo = new System.Windows.Forms.PictureBox();
            this.m_lblTheSeedCompany = new System.Windows.Forms.Label();
            this.m_lblProgramName = new System.Windows.Forms.Label();
            this.m_lblDescription = new System.Windows.Forms.Label();
            this.m_lblTSCInfo = new System.Windows.Forms.Label();
            this.m_btnClose = new System.Windows.Forms.Button();
            this.m_lblVersion = new System.Windows.Forms.Label();
            this.m_link = new System.Windows.Forms.LinkLabel();
            this.m_lblCopyright = new System.Windows.Forms.Label();
            this.m_OurWordIcon = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.m_imgSeedCompanyLogo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.m_OurWordIcon)).BeginInit();
            this.SuspendLayout();
            // 
            // m_imgSeedCompanyLogo
            // 
            this.m_imgSeedCompanyLogo.Image = ((System.Drawing.Image)(resources.GetObject("m_imgSeedCompanyLogo.Image")));
            this.m_imgSeedCompanyLogo.Location = new System.Drawing.Point(12, 120);
            this.m_imgSeedCompanyLogo.Name = "m_imgSeedCompanyLogo";
            this.m_imgSeedCompanyLogo.Size = new System.Drawing.Size(72, 80);
            this.m_imgSeedCompanyLogo.TabIndex = 0;
            this.m_imgSeedCompanyLogo.TabStop = false;
            // 
            // m_lblTheSeedCompany
            // 
            this.m_lblTheSeedCompany.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_lblTheSeedCompany.Location = new System.Drawing.Point(12, 192);
            this.m_lblTheSeedCompany.Name = "m_lblTheSeedCompany";
            this.m_lblTheSeedCompany.Size = new System.Drawing.Size(72, 40);
            this.m_lblTheSeedCompany.TabIndex = 1;
            this.m_lblTheSeedCompany.Text = "The Seed Company";
            this.m_lblTheSeedCompany.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // m_lblProgramName
            // 
            this.m_lblProgramName.Font = new System.Drawing.Font("Papyrus", 21.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_lblProgramName.Location = new System.Drawing.Point(96, 16);
            this.m_lblProgramName.Name = "m_lblProgramName";
            this.m_lblProgramName.Size = new System.Drawing.Size(288, 40);
            this.m_lblProgramName.TabIndex = 2;
            this.m_lblProgramName.Text = "Our Word!";
            // 
            // m_lblDescription
            // 
            this.m_lblDescription.Location = new System.Drawing.Point(96, 64);
            this.m_lblDescription.Name = "m_lblDescription";
            this.m_lblDescription.Size = new System.Drawing.Size(288, 56);
            this.m_lblDescription.TabIndex = 3;
            this.m_lblDescription.Text = "A program which assists Mother-Tongue Bible Translators and their supporting team" +
                ", who are making use of a quality Front Translation.";
            // 
            // m_lblTSCInfo
            // 
            this.m_lblTSCInfo.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_lblTSCInfo.Location = new System.Drawing.Point(96, 152);
            this.m_lblTSCInfo.Name = "m_lblTSCInfo";
            this.m_lblTSCInfo.Size = new System.Drawing.Size(288, 24);
            this.m_lblTSCInfo.TabIndex = 4;
            this.m_lblTSCInfo.Text = "The Seed Company, 3030 Matlock Road, Suite 104, Arlington, Texas 76105, USA. (817" +
                ")557-2121";
            // 
            // m_btnClose
            // 
            this.m_btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.m_btnClose.Location = new System.Drawing.Point(168, 240);
            this.m_btnClose.Name = "m_btnClose";
            this.m_btnClose.Size = new System.Drawing.Size(75, 23);
            this.m_btnClose.TabIndex = 5;
            this.m_btnClose.Text = "Close";
            // 
            // m_lblVersion
            // 
            this.m_lblVersion.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_lblVersion.Location = new System.Drawing.Point(96, 120);
            this.m_lblVersion.Name = "m_lblVersion";
            this.m_lblVersion.Size = new System.Drawing.Size(288, 16);
            this.m_lblVersion.TabIndex = 6;
            this.m_lblVersion.Text = "Version {0}";
            // 
            // m_link
            // 
            this.m_link.Location = new System.Drawing.Point(96, 208);
            this.m_link.Name = "m_link";
            this.m_link.Size = new System.Drawing.Size(288, 16);
            this.m_link.TabIndex = 7;
            this.m_link.TabStop = true;
            this.m_link.Text = "Send Email";
            this.m_link.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.onSendEmail);
            // 
            // m_lblCopyright
            // 
            this.m_lblCopyright.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_lblCopyright.Location = new System.Drawing.Point(96, 184);
            this.m_lblCopyright.Name = "m_lblCopyright";
            this.m_lblCopyright.Size = new System.Drawing.Size(288, 24);
            this.m_lblCopyright.TabIndex = 8;
            this.m_lblCopyright.Text = "Copyright © 2004-08 John Wimbish. All rights reserved.";
            // 
            // m_OurWordIcon
            // 
            this.m_OurWordIcon.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.m_OurWordIcon.Image = ((System.Drawing.Image)(resources.GetObject("m_OurWordIcon.Image")));
            this.m_OurWordIcon.Location = new System.Drawing.Point(12, 16);
            this.m_OurWordIcon.Name = "m_OurWordIcon";
            this.m_OurWordIcon.Size = new System.Drawing.Size(72, 80);
            this.m_OurWordIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.m_OurWordIcon.TabIndex = 9;
            this.m_OurWordIcon.TabStop = false;
            // 
            // DialogHelpAbout
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.CancelButton = this.m_btnClose;
            this.ClientSize = new System.Drawing.Size(394, 269);
            this.Controls.Add(this.m_OurWordIcon);
            this.Controls.Add(this.m_lblCopyright);
            this.Controls.Add(this.m_link);
            this.Controls.Add(this.m_lblVersion);
            this.Controls.Add(this.m_btnClose);
            this.Controls.Add(this.m_lblTSCInfo);
            this.Controls.Add(this.m_lblDescription);
            this.Controls.Add(this.m_lblProgramName);
            this.Controls.Add(this.m_imgSeedCompanyLogo);
            this.Controls.Add(this.m_lblTheSeedCompany);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DialogHelpAbout";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "About Our Word!";
            this.Load += new System.EventHandler(this.oHelpAbout_Load);
            ((System.ComponentModel.ISupportInitialize)(this.m_imgSeedCompanyLogo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.m_OurWordIcon)).EndInit();
            this.ResumeLayout(false);

		}
		#endregion

		#region Cmd: oHelpAbout_Load
		private void oHelpAbout_Load(object sender, System.EventArgs e)
		{
			// Localization
            Control[] vExclude = { m_lblVersion, m_lblTSCInfo, m_lblTheSeedCompany };
            LocDB.Localize(this, vExclude);

            // The version information is built at runtime
            m_lblVersion.Text = G.GetLoc_DialogCommon(
                m_lblVersion.Name,
                "Version {0}",
                new string[] { G.Version });
		}
		#endregion

		#region Method: void onSendEmail(...)
		private void onSendEmail(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
		{
			string sEmail = "John_Wimbish@tsco.org";
			string sSubject = LanguageResources.AppTitle + " - " + G.Version;

			string sProcess = "mailto:" + sEmail + "?subject=" + sSubject;

			System.Diagnostics.Process.Start(sProcess);
		}
		#endregion
	}

	public class HelpSystem : JW_Help
	{
		#region Method: Initialize()
		static public void Initialize()
		{
			JW_Help.Initialize(OurWordMain.App, "OurWordMain.chm");

			JW_Help.AddTopic( (int)Topic.kDlgBookProperties, 
				"Reference\\Dialogs\\BookProperties\\Dlg-BookProperties.html");

			JW_Help.AddTopic( (int)Topic.kDlgIncrementBookStatus, 
				"Reference\\Dialogs\\IncrementBookStatus\\Dlg-IncrementBookStatus.html");

			JW_Help.AddTopic( (int)Topic.kDlgRestoreFromBackup, 
				"Reference\\Dialogs\\RestoreFromBackup\\Dlg-RestoreFromBackup.html");

			JW_Help.AddTopic( (int)Topic.kDlgOptions, 
				"Reference\\Dialogs\\Options\\Dlg-Options.html");

			JW_Help.AddTopic( (int)Topic.kDlgProjectProps, 
				"Reference\\Dialogs\\ProjectProperties\\Dlg-ProjectProperties.html");

			JW_Help.AddTopic( (int)Topic.kDlgProjectProps_PageOverview, 
				"Reference\\Dialogs\\ProjectProperties\\Page-Overview.html");
            JW_Help.AddTopic((int)Topic.kPage_SetupFront, 
				"Reference\\Dialogs\\ProjectProperties\\Page-SetupFront.html");
			JW_Help.AddTopic( (int)Topic.kPage_SetupTarget, 
				"Reference\\Dialogs\\ProjectProperties\\Page-SetupTarget.html");
			JW_Help.AddTopic( (int)Topic.kDlgProjectProps_PageSetupSiblings, 
				"Reference\\Dialogs\\ProjectProperties\\Page-SetupSiblings.html");
			JW_Help.AddTopic( (int)Topic.kDlgProjectProps_PageSetupReferences, 
				"Reference\\Dialogs\\ProjectProperties\\Page-SetupRefs.html");
            JW_Help.AddTopic((int)Topic.kPage_Translation, 
				"Reference\\Dialogs\\ProjectProperties\\Page-TransProps.html");
            JW_Help.AddTopic((int)Topic.kPage_StyleSheet, 
				"Reference\\Dialogs\\ProjectProperties\\Page-Styles.html");
            JW_Help.AddTopic((int)Topic.kPage_AdvancedPrintOptions, 
				"Reference\\Dialogs\\ProjectProperties\\ProjProp-PrintOptions.html");
            JW_Help.AddTopic((int)Topic.kPage_TranslationStages, 
				"Reference\\Dialogs\\ProjectProperties\\ProjProp-TranslationStages.html");
            JW_Help.AddTopic((int)Topic.kPage_WritingSystems,
                "Tasks\\WritingSystems.html");

			JW_Help.AddTopic( (int)Topic.kDlgCreateTranslation, 
				"Reference\\Dialogs\\ProjectProperties\\Dlg-CreateTranslation.html");

			JW_Help.AddTopic( (int)Topic.kDlgOpenTranslation, 
				"Reference\\Dialogs\\ProjectProperties\\Dlg-OpenTranslation.html");

			JW_Help.AddTopic( (int)Topic.kDlgBookNames, 
				"Reference\\Dialogs\\ProjectProperties\\Dlg-BookNames.html");

			JW_Help.AddTopic( (int)Topic.kDlgWritingSystems, 
				"Reference\\Dialogs\\ProjectProperties\\Dlg-WritingSystems.html");

			JW_Help.AddTopic( (int)Topic.kDlgPrint, 
				"Reference\\Dialogs\\Print\\Dlg-Print.html");

            JW_Help.AddTopic((int)Topic.kDlgExportBook,
                "Reference\\Dialogs\\Dlg-ExportBook.html");

			JW_Help.AddTopic( (int)Topic.kDlgFilter, 
				"Reference\\Dialogs\\OnlyShowSectionsThat\\OnlyShowSectionsThat.html");

			// Errors in the RepairImportBook dialog
			JW_Help.AddTopic( (int)Topic.kErrMissingParagraphMarker, 
				"Reference\\Dialogs\\RepairImportBook\\Err_MissingParagraphMarker.html");
			JW_Help.AddTopic( (int)Topic.kErrMissingVerseNumber, 
				"Reference\\Dialogs\\RepairImportBook\\Err_MissingVerseNumber.html");
			JW_Help.AddTopic( (int)Topic.kErrMissingParagraphMarkerForCF, 
				"Reference\\Dialogs\\RepairImportBook\\Err_MissingParagraphMarkerForCF.html");
			JW_Help.AddTopic( (int)Topic.kErrChapterNotInParagraph, 
				"Reference\\Dialogs\\RepairImportBook\\Err_ChapterNotInParagraph.html");
			JW_Help.AddTopic( (int)Topic.kErrBadChapterNo, 
				"Reference\\Dialogs\\RepairImportBook\\Err_BadChapterNo.html");
			JW_Help.AddTopic( (int)Topic.kErrMissingParagraphMarkerForNote, 
				"Reference\\Dialogs\\RepairImportBook\\Err_MissingParagraphMarkerForNote.html");
			JW_Help.AddTopic( (int)Topic.kErrNoVerseInSection, 
				"Reference\\Dialogs\\RepairImportBook\\Err_NoVerseInSection.html");
			JW_Help.AddTopic( (int)Topic.kErrStructureSectionMiscount, 
				"Reference\\Dialogs\\RepairImportBook\\Err_StructureSectionMiscount.html");
			JW_Help.AddTopic( (int)Topic.kErrStructureMismatch, 
				"Reference\\Dialogs\\RepairImportBook\\Err_StructureMismatch.html");

            JW_Help.AddTopic((int)Topic.kWizImportBook_GetFilename,
                "Reference\\Dialogs\\WizImportBook\\WizImportBook-GetFilename.html");
            JW_Help.AddTopic((int)Topic.kWizImportBook_IdentifyBook,
                "Reference\\Dialogs\\WizImportBook\\WizImportBook-IdentifyTheBook.html");
            JW_Help.AddTopic((int)Topic.kWizImportBook_IdentifyStage,
                "Reference\\Dialogs\\WizImportBook\\WizImportBook-IdentifyTheStage.html");
            JW_Help.AddTopic((int)Topic.kWizImportBook_WhereToStore,
                "Reference\\Dialogs\\WizImportBook\\WizImportBook-WhereToStore.html");
            JW_Help.AddTopic((int)Topic.kWizImportBook_Summary,
                "Reference\\Dialogs\\WizImportBook\\WizImportBook-Summary.html");

		}
		#endregion

		// Topics ----------------------------------------------------------------------------
		#region enum Topic ID's
		public enum Topic
		{
			kDlgBookProperties = 0,
			kDlgIncrementBookStatus,
			kDlgRestoreFromBackup,
			kDlgOptions,
			kDlgProjectProps,
			kDlgProjectProps_PageOverview,
            kPage_SetupFront,
			kPage_SetupTarget,
			kDlgProjectProps_PageSetupSiblings,
			kDlgProjectProps_PageSetupReferences,
            kPage_Translation,
            kPage_StyleSheet,
            kPage_AdvancedPrintOptions,
            kPage_TranslationStages,
            kPage_WritingSystems,
			kDlgCreateTranslation,
			kDlgOpenTranslation,
			kDlgBookNames,
			kDlgWritingSystems,
			kDlgPrint,
            kDlgExportBook,
			kDlgFilter,
			kErrMissingParagraphMarker,
			kErrMissingVerseNumber,
			kErrMissingParagraphMarkerForCF,
			kErrChapterNotInParagraph,
			kErrBadChapterNo,
			kErrMissingParagraphMarkerForNote,
			kErrNoVerseInSection,
			kErrStructureSectionMiscount,
			kErrStructureMismatch,
            kWizImportBook_GetFilename,
            kWizImportBook_IdentifyBook,
            kWizImportBook_IdentifyStage,
            kWizImportBook_WhereToStore,
            kWizImportBook_Summary,
            kLast
		}
		#endregion

		// Invoking individual topics --------------------------------------------------------
		#region Method: void ShowTopic(Topic nID)
		static public void ShowTopic(Topic nID)
		{
			JW_Help.ShowTopic((int)nID);
		}
		#endregion
		#region Method: void Show_DlgBookProperties()
		static public void Show_DlgBookProperties()
		{
			ShowTopic(Topic.kDlgBookProperties);
		}
		#endregion
		#region Method: void Show_DlgIncrementBookStatus()
		static public void Show_DlgIncrementBookStatus()
		{
			ShowTopic(Topic.kDlgIncrementBookStatus);
		}
		#endregion
		#region Method: void Show_DlgRestoreFromBackup()
		static public void Show_DlgRestoreFromBackup()
		{
			ShowTopic(Topic.kDlgRestoreFromBackup);
		}
		#endregion
		#region Method: void Show_DlgPrint()
		static public void Show_DlgPrint()
		{
			ShowTopic(Topic.kDlgPrint);
		}
		#endregion
        #region Method: void Show_DlgExportBook()
        static public void Show_DlgExportBook()
        {
            ShowTopic(Topic.kDlgExportBook);
        }
        #endregion

		// Project Properties ----------------------------------------------------------------
		#region Method: void Show_DlgProjectProperties()
		static public void Show_DlgProjectProperties()
		{
			ShowTopic(Topic.kDlgProjectProps);
		}
		#endregion
		#region Method: void Show_DlgProjectProperties_PageOverview()
		static public void Show_DlgProjectProperties_PageOverview()
		{
			ShowTopic(Topic.kDlgProjectProps_PageOverview);
		}
		#endregion
        #region Method: void ShowPage_SetupFront()
        static public void ShowPage_SetupFront()
		{
			ShowTopic(Topic.kPage_SetupFront);
		}
		#endregion
        #region Method: void ShowPage_SetupTarget()
        static public void ShowPage_SetupTarget()
		{
			ShowTopic(Topic.kPage_SetupTarget);
		}
		#endregion
		#region Method: void Show_DlgProjectProperties_PageSetupSiblings()
		static public void Show_DlgProjectProperties_PageSetupSiblings()
		{
			ShowTopic(Topic.kDlgProjectProps_PageSetupSiblings);
		}
		#endregion
		#region Method: void Show_DlgProjectProperties_PageSetupReferences()
		static public void Show_DlgProjectProperties_PageSetupReferences()
		{
			ShowTopic(Topic.kDlgProjectProps_PageSetupReferences);
		}
		#endregion
        #region Method: void ShowPage_Translation()
        static public void ShowPage_Translation()
		{
            ShowTopic(Topic.kPage_Translation);
		}
		#endregion
        #region Method: void ShowPage_StylesSheet()
        static public void ShowPage_StylesSheet()
		{
			ShowTopic(Topic.kPage_StyleSheet);
		}
		#endregion
        #region Method: void ShowPage_AdvancedPrintOptions()
        static public void ShowPage_AdvancedPrintOptions()
		{
			ShowTopic(Topic.kPage_AdvancedPrintOptions);
		}
		#endregion
        #region Method: void ShowPage_TranslationStages()
        static public void ShowPage_TranslationStages()
		{
			ShowTopic(Topic.kPage_TranslationStages);
		}
		#endregion
        #region Method: void ShowPage_WritingSystems()
        static public void ShowPage_WritingSystems()
        {
            ShowTopic(Topic.kPage_WritingSystems);
        }
        #endregion

        // Import Book Wizard ----------------------------------------------------------------
        #region Method: void Show_WizImportBook_GetFilename()
        static public void Show_WizImportBook_GetFilename()
        {
            ShowTopic(Topic.kWizImportBook_GetFilename);
        }
        #endregion
        #region Method: void Show_WizImportBook_IdentifyBook()
        static public void Show_WizImportBook_IdentifyBook()
        {
            ShowTopic(Topic.kWizImportBook_IdentifyBook);
        }
        #endregion
        #region Method: void Show_WizImportBook_IdentifyStage()
        static public void Show_WizImportBook_IdentifyStage()
        {
            ShowTopic(Topic.kWizImportBook_IdentifyStage);
        }
        #endregion
        #region Method: void Show_WizImportBook_WhereToStore()
        static public void Show_WizImportBook_WhereToStore()
        {
            ShowTopic(Topic.kWizImportBook_WhereToStore);
        }
        #endregion
        #region Method: void Show_WizImportBook_Summary()
        static public void Show_WizImportBook_Summary()
        {
            ShowTopic(Topic.kWizImportBook_Summary);
        }
        #endregion

		#region Method: void Show_DlgCreateTranslation()
		static public void Show_DlgCreateTranslation()
		{
			ShowTopic(Topic.kDlgCreateTranslation);
		}
		#endregion
		#region Method: void Show_DlgOpenTranslation()
		static public void Show_DlgOpenTranslation()
		{
			ShowTopic(Topic.kDlgOpenTranslation);
		}
		#endregion
		#region Method: void Show_DlgBookNames()
		static public void Show_DlgBookNames()
		{
			ShowTopic(Topic.kDlgBookNames);
		}
		#endregion
		#region Method: void Show_DlgWritingSystems()
		static public void Show_DlgWritingSystems()
		{
			ShowTopic(Topic.kDlgWritingSystems);
		}
		#endregion

		#region Method: void Show_DlgFilter()
		static public void Show_DlgFilter()
		{
			ShowTopic(Topic.kDlgFilter);
		}
		#endregion

	}
}
