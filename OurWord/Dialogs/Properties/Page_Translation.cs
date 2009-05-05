/**********************************************************************************************
 * Project: Our Word!
 * File:    Page_Translation.cs
 * Author:  John Wimbish
 * Created: 28 Dec 2004
 * Purpose: Edit the properties of a DTranslation.
 * Legal:   Copyright (c) 2005-09, John S. Wimbish. All Rights Reserved.  
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
using OurWord.Edit;
using OurWord.View;
#endregion

/* TODO: Make sure we can't get duplicate book names. We formerly had this as a 
 * tet in the BookProperties dialog ValidateData, calling
 *    Messages.DuplicateBookName();
 * But now that dialog no longer handles the book name, it is done here.
 *
 */

namespace OurWord.Dialogs
{
    public class Page_Translation : DlgPropertySheet
	{
        // Attrs -----------------------------------------------------------------------------
        #region Attr{g}: DTranslation Translation
        DTranslation Translation
		{
			get
			{
				Debug.Assert(null != m_Translation);
				return m_Translation;
			}
		}
		DTranslation m_Translation = null;
		#endregion
		#region Attr{g}: bool SuppressCreateBook
		bool SuppressCreateBook
		{
			get
			{
				return m_bSuppressCreateBook;
			}
		}
		bool m_bSuppressCreateBook = false;
		#endregion
		#region Attr{g}: DBook CurrentBook - the currently-displayed book
		DBook CurrentBook
		{
			get
			{
				DSection section = DB.Project.STarget;
				if (null == section)
					return null;
				return section.Book;
			}
		}
		#endregion
		#region Attr{g/s}: string LanguageName
		string LanguageName
		{
			get
			{
				return m_editLanguageName.Text;
			}
			set
			{
				m_editLanguageName.Text = value;
			}
		}
		#endregion

		// Scaffolding -----------------------------------------------------------------------
		#region Controls
		private System.Windows.Forms.Label m_lblLanguageName;
        private System.Windows.Forms.TextBox m_editLanguageName;
        private Button m_btnRemoveTranslation;
        private TabControl m_tabctrlTranslation;
        private TabPage m_tabOther;
		private TabPage m_tabBooks;
        private DataGridView m_gridBooks;
		private Button m_bProperties;
		private Button m_bRemove;
		private Button m_bExport;
		private Button m_bImport;
        private Button m_bCreate;
		private DataGridViewTextBoxColumn m_colAbbreviation;
		private DataGridViewTextBoxColumn m_colBookName;
        private ComboBox m_comboLanguage;
        private Button m_btnCopyBookNames;
        private RadioButton m_radioStartedBooks;
        private RadioButton m_radioOldTestament;
        private RadioButton m_radioNewTestament;
        private RadioButton m_radioAll;
        private LiterateSettingsWnd m_LiterateSettings;
        private DataGridViewTextBoxColumn m_colStage;
		#endregion
        #region Constructor(DlgProperties, DTranslation, bSuppressCreateBook)
        public Page_Translation(DialogProperties _ParentDlg, 
            DTranslation trans, 
            bool bSuppressCreateBook)
            : base(_ParentDlg)
        {
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			// Initialize attributes
			m_Translation = trans;
			m_bSuppressCreateBook = bSuppressCreateBook;
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
		// Required designer variable.
		private System.ComponentModel.Container components = null;

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.m_lblLanguageName = new System.Windows.Forms.Label();
            this.m_editLanguageName = new System.Windows.Forms.TextBox();
            this.m_btnRemoveTranslation = new System.Windows.Forms.Button();
            this.m_tabctrlTranslation = new System.Windows.Forms.TabControl();
            this.m_tabBooks = new System.Windows.Forms.TabPage();
            this.m_radioStartedBooks = new System.Windows.Forms.RadioButton();
            this.m_radioOldTestament = new System.Windows.Forms.RadioButton();
            this.m_radioNewTestament = new System.Windows.Forms.RadioButton();
            this.m_radioAll = new System.Windows.Forms.RadioButton();
            this.m_comboLanguage = new System.Windows.Forms.ComboBox();
            this.m_btnCopyBookNames = new System.Windows.Forms.Button();
            this.m_bProperties = new System.Windows.Forms.Button();
            this.m_bRemove = new System.Windows.Forms.Button();
            this.m_bExport = new System.Windows.Forms.Button();
            this.m_bImport = new System.Windows.Forms.Button();
            this.m_bCreate = new System.Windows.Forms.Button();
            this.m_gridBooks = new System.Windows.Forms.DataGridView();
            this.m_colAbbreviation = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.m_colBookName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.m_colStage = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.m_tabOther = new System.Windows.Forms.TabPage();
            this.m_LiterateSettings = new OurWord.Edit.LiterateSettingsWnd();
            this.m_tabctrlTranslation.SuspendLayout();
            this.m_tabBooks.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.m_gridBooks)).BeginInit();
            this.m_tabOther.SuspendLayout();
            this.SuspendLayout();
            // 
            // m_lblLanguageName
            // 
            this.m_lblLanguageName.Location = new System.Drawing.Point(6, 3);
            this.m_lblLanguageName.Name = "m_lblLanguageName";
            this.m_lblLanguageName.Size = new System.Drawing.Size(100, 27);
            this.m_lblLanguageName.TabIndex = 0;
            this.m_lblLanguageName.Text = "Language Name:";
            this.m_lblLanguageName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // m_editLanguageName
            // 
            this.m_editLanguageName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.m_editLanguageName.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_editLanguageName.Location = new System.Drawing.Point(112, 6);
            this.m_editLanguageName.Name = "m_editLanguageName";
            this.m_editLanguageName.Size = new System.Drawing.Size(339, 26);
            this.m_editLanguageName.TabIndex = 1;
            this.m_editLanguageName.Text = "Translation Name";
            // 
            // m_btnRemoveTranslation
            // 
            this.m_btnRemoveTranslation.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.m_btnRemoveTranslation.Location = new System.Drawing.Point(6, 310);
            this.m_btnRemoveTranslation.Name = "m_btnRemoveTranslation";
            this.m_btnRemoveTranslation.Size = new System.Drawing.Size(159, 23);
            this.m_btnRemoveTranslation.TabIndex = 5;
            this.m_btnRemoveTranslation.Text = "Remove This Translation...";
            this.m_btnRemoveTranslation.UseVisualStyleBackColor = true;
            this.m_btnRemoveTranslation.Click += new System.EventHandler(this.cmdRemoveTranslation);
            // 
            // m_tabctrlTranslation
            // 
            this.m_tabctrlTranslation.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.m_tabctrlTranslation.Controls.Add(this.m_tabBooks);
            this.m_tabctrlTranslation.Controls.Add(this.m_tabOther);
            this.m_tabctrlTranslation.Location = new System.Drawing.Point(0, 0);
            this.m_tabctrlTranslation.Name = "m_tabctrlTranslation";
            this.m_tabctrlTranslation.SelectedIndex = 0;
            this.m_tabctrlTranslation.Size = new System.Drawing.Size(468, 365);
            this.m_tabctrlTranslation.TabIndex = 6;
            // 
            // m_tabBooks
            // 
            this.m_tabBooks.Controls.Add(this.m_radioStartedBooks);
            this.m_tabBooks.Controls.Add(this.m_radioOldTestament);
            this.m_tabBooks.Controls.Add(this.m_radioNewTestament);
            this.m_tabBooks.Controls.Add(this.m_radioAll);
            this.m_tabBooks.Controls.Add(this.m_comboLanguage);
            this.m_tabBooks.Controls.Add(this.m_btnCopyBookNames);
            this.m_tabBooks.Controls.Add(this.m_bProperties);
            this.m_tabBooks.Controls.Add(this.m_bRemove);
            this.m_tabBooks.Controls.Add(this.m_bExport);
            this.m_tabBooks.Controls.Add(this.m_bImport);
            this.m_tabBooks.Controls.Add(this.m_bCreate);
            this.m_tabBooks.Controls.Add(this.m_gridBooks);
            this.m_tabBooks.Location = new System.Drawing.Point(4, 22);
            this.m_tabBooks.Name = "m_tabBooks";
            this.m_tabBooks.Padding = new System.Windows.Forms.Padding(3);
            this.m_tabBooks.Size = new System.Drawing.Size(460, 339);
            this.m_tabBooks.TabIndex = 4;
            this.m_tabBooks.Text = "Books";
            this.m_tabBooks.UseVisualStyleBackColor = true;
            // 
            // m_radioStartedBooks
            // 
            this.m_radioStartedBooks.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.m_radioStartedBooks.Location = new System.Drawing.Point(343, 285);
            this.m_radioStartedBooks.Name = "m_radioStartedBooks";
            this.m_radioStartedBooks.Size = new System.Drawing.Size(111, 17);
            this.m_radioStartedBooks.TabIndex = 112;
            this.m_radioStartedBooks.TabStop = true;
            this.m_radioStartedBooks.Text = "Started Books";
            this.m_radioStartedBooks.UseVisualStyleBackColor = true;
            this.m_radioStartedBooks.CheckedChanged += new System.EventHandler(this.cmdFilterOnStartedBooks);
            // 
            // m_radioOldTestament
            // 
            this.m_radioOldTestament.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.m_radioOldTestament.Location = new System.Drawing.Point(343, 267);
            this.m_radioOldTestament.Name = "m_radioOldTestament";
            this.m_radioOldTestament.Size = new System.Drawing.Size(111, 17);
            this.m_radioOldTestament.TabIndex = 111;
            this.m_radioOldTestament.TabStop = true;
            this.m_radioOldTestament.Text = "Old Testament";
            this.m_radioOldTestament.UseVisualStyleBackColor = true;
            this.m_radioOldTestament.CheckedChanged += new System.EventHandler(this.cmdFilterOnOT);
            // 
            // m_radioNewTestament
            // 
            this.m_radioNewTestament.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.m_radioNewTestament.Location = new System.Drawing.Point(343, 249);
            this.m_radioNewTestament.Name = "m_radioNewTestament";
            this.m_radioNewTestament.Size = new System.Drawing.Size(111, 17);
            this.m_radioNewTestament.TabIndex = 110;
            this.m_radioNewTestament.TabStop = true;
            this.m_radioNewTestament.Text = "New Testament";
            this.m_radioNewTestament.UseVisualStyleBackColor = true;
            this.m_radioNewTestament.CheckedChanged += new System.EventHandler(this.cmdFilterOnNT);
            // 
            // m_radioAll
            // 
            this.m_radioAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.m_radioAll.Location = new System.Drawing.Point(343, 231);
            this.m_radioAll.Name = "m_radioAll";
            this.m_radioAll.Size = new System.Drawing.Size(111, 17);
            this.m_radioAll.TabIndex = 109;
            this.m_radioAll.TabStop = true;
            this.m_radioAll.Text = "All";
            this.m_radioAll.UseVisualStyleBackColor = true;
            this.m_radioAll.CheckedChanged += new System.EventHandler(this.cmdFilterOnAll);
            // 
            // m_comboLanguage
            // 
            this.m_comboLanguage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.m_comboLanguage.Location = new System.Drawing.Point(343, 192);
            this.m_comboLanguage.Name = "m_comboLanguage";
            this.m_comboLanguage.Size = new System.Drawing.Size(111, 21);
            this.m_comboLanguage.TabIndex = 108;
            // 
            // m_btnCopyBookNames
            // 
            this.m_btnCopyBookNames.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.m_btnCopyBookNames.Location = new System.Drawing.Point(343, 155);
            this.m_btnCopyBookNames.Name = "m_btnCopyBookNames";
            this.m_btnCopyBookNames.Size = new System.Drawing.Size(111, 36);
            this.m_btnCopyBookNames.TabIndex = 106;
            this.m_btnCopyBookNames.Text = "Copy Book Names from";
            this.m_btnCopyBookNames.Click += new System.EventHandler(this.cmdCopyBookNames);
            // 
            // m_bProperties
            // 
            this.m_bProperties.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.m_bProperties.Location = new System.Drawing.Point(343, 114);
            this.m_bProperties.Name = "m_bProperties";
            this.m_bProperties.Size = new System.Drawing.Size(111, 23);
            this.m_bProperties.TabIndex = 13;
            this.m_bProperties.Text = "Properties...";
            this.m_bProperties.Click += new System.EventHandler(this.cmdBookProperties);
            // 
            // m_bRemove
            // 
            this.m_bRemove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.m_bRemove.Location = new System.Drawing.Point(343, 87);
            this.m_bRemove.Name = "m_bRemove";
            this.m_bRemove.Size = new System.Drawing.Size(111, 23);
            this.m_bRemove.TabIndex = 12;
            this.m_bRemove.Text = "Remove...";
            this.m_bRemove.Click += new System.EventHandler(this.cmdRemoveBook);
            // 
            // m_bExport
            // 
            this.m_bExport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.m_bExport.Location = new System.Drawing.Point(343, 60);
            this.m_bExport.Name = "m_bExport";
            this.m_bExport.Size = new System.Drawing.Size(111, 23);
            this.m_bExport.TabIndex = 11;
            this.m_bExport.Text = "Export...";
            this.m_bExport.Click += new System.EventHandler(this.cmdExportBook);
            // 
            // m_bImport
            // 
            this.m_bImport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.m_bImport.Location = new System.Drawing.Point(343, 33);
            this.m_bImport.Name = "m_bImport";
            this.m_bImport.Size = new System.Drawing.Size(111, 23);
            this.m_bImport.TabIndex = 8;
            this.m_bImport.Text = "Import...";
            this.m_bImport.Click += new System.EventHandler(this.cmdImportBook);
            // 
            // m_bCreate
            // 
            this.m_bCreate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.m_bCreate.Location = new System.Drawing.Point(343, 6);
            this.m_bCreate.Name = "m_bCreate";
            this.m_bCreate.Size = new System.Drawing.Size(111, 23);
            this.m_bCreate.TabIndex = 7;
            this.m_bCreate.Text = "Create...";
            this.m_bCreate.Click += new System.EventHandler(this.cmdCreateBook);
            // 
            // m_gridBooks
            // 
            this.m_gridBooks.AllowUserToAddRows = false;
            this.m_gridBooks.AllowUserToDeleteRows = false;
            this.m_gridBooks.AllowUserToResizeRows = false;
            this.m_gridBooks.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.m_gridBooks.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.m_gridBooks.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.m_colAbbreviation,
            this.m_colBookName,
            this.m_colStage});
            this.m_gridBooks.Location = new System.Drawing.Point(6, 6);
            this.m_gridBooks.MultiSelect = false;
            this.m_gridBooks.Name = "m_gridBooks";
            this.m_gridBooks.RowHeadersVisible = false;
            this.m_gridBooks.RowTemplate.Height = 19;
            this.m_gridBooks.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.m_gridBooks.Size = new System.Drawing.Size(331, 327);
            this.m_gridBooks.TabIndex = 0;
            this.m_gridBooks.Resize += new System.EventHandler(this.cmdDataGridResized);
            this.m_gridBooks.SelectionChanged += new System.EventHandler(this.cmdGridSelectionChanged);
            // 
            // m_colAbbreviation
            // 
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_colAbbreviation.DefaultCellStyle = dataGridViewCellStyle1;
            this.m_colAbbreviation.HeaderText = "Abbrev";
            this.m_colAbbreviation.Name = "m_colAbbreviation";
            this.m_colAbbreviation.ReadOnly = true;
            this.m_colAbbreviation.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.m_colAbbreviation.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.m_colAbbreviation.ToolTipText = "The standard 3-letter abbreviation for this book, commonly used in Bible translat" +
                "ion organizations to identify the book.";
            this.m_colAbbreviation.Width = 50;
            // 
            // m_colBookName
            // 
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_colBookName.DefaultCellStyle = dataGridViewCellStyle2;
            this.m_colBookName.HeaderText = "Book Name";
            this.m_colBookName.Name = "m_colBookName";
            this.m_colBookName.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.m_colBookName.ToolTipText = "The name of the book in your language.";
            this.m_colBookName.Width = 170;
            // 
            // m_colStage
            // 
            this.m_colStage.HeaderText = "Stage";
            this.m_colStage.Name = "m_colStage";
            this.m_colStage.ReadOnly = true;
            this.m_colStage.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.m_colStage.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.m_colStage.ToolTipText = "E.g., Draft, Revision, Consultant Checked, etc.";
            this.m_colStage.Width = 90;
            // 
            // m_tabOther
            // 
            this.m_tabOther.Controls.Add(this.m_LiterateSettings);
            this.m_tabOther.Controls.Add(this.m_btnRemoveTranslation);
            this.m_tabOther.Controls.Add(this.m_lblLanguageName);
            this.m_tabOther.Controls.Add(this.m_editLanguageName);
            this.m_tabOther.Location = new System.Drawing.Point(4, 22);
            this.m_tabOther.Name = "m_tabOther";
            this.m_tabOther.Padding = new System.Windows.Forms.Padding(3);
            this.m_tabOther.Size = new System.Drawing.Size(460, 339);
            this.m_tabOther.TabIndex = 0;
            this.m_tabOther.Text = "Other";
            this.m_tabOther.UseVisualStyleBackColor = true;
            // 
            // m_LiterateSettings
            // 
            this.m_LiterateSettings.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.m_LiterateSettings.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.m_LiterateSettings.DontAllowPropertyGrid = false;
            this.m_LiterateSettings.Location = new System.Drawing.Point(9, 38);
            this.m_LiterateSettings.Name = "m_LiterateSettings";
            this.m_LiterateSettings.ShowDocumentation = true;
            this.m_LiterateSettings.Size = new System.Drawing.Size(442, 266);
            this.m_LiterateSettings.TabIndex = 7;
            // 
            // Page_Translation
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.m_tabctrlTranslation);
            this.Name = "Page_Translation";
            this.Size = new System.Drawing.Size(468, 368);
            this.Load += new System.EventHandler(this.cmdLoad);
            this.m_tabctrlTranslation.ResumeLayout(false);
            this.m_tabBooks.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.m_gridBooks)).EndInit();
            this.m_tabOther.ResumeLayout(false);
            this.m_tabOther.PerformLayout();
            this.ResumeLayout(false);

		}
		#endregion

		// DlgPropertySheet overrides --------------------------------------------------------
		#region SMethod: string ComputeID(string sTranslationDisplayName)
		public static string ComputeID(string sTranslationDisplayName)
		{
			return "idTranslation_" + sTranslationDisplayName;
		}
		#endregion
		#region OAttr{g}: string ID
		public override string ID
		{
			get
			{
				// For Visual Studio
				if (null == m_Translation) 
					return ""; 

				return ComputeID(Translation.DisplayName);
			}
		}
		#endregion
		#region Attr{g}: string TabText
        public override string Title
        {
            get
            {
                string sLeader = "";
                if (Translation == DB.FrontTranslation)
                    sLeader = "Front: ";
                else if (Translation == DB.TargetTranslation)
                    sLeader = "Target: ";

                return sLeader + Translation.DisplayName;
            }
        }
        #endregion
        #region Method: override bool HarvestChanges()
        public override bool HarvestChanges()
        {
            HarvestBookNames();

			// The translation's name should be a valid, non-zero-lengthed name
			if (LanguageName.Length == 0)
			{
                m_tabctrlTranslation.SelectedTab = m_tabOther;
                Messages.TranslationNeedsName();
                m_editLanguageName.Focus();
				m_editLanguageName.Select();
				return false;
			}
			Translation.DisplayName = LanguageName;

            // Writing Systems
            Translation.ConsultantWritingSystemName = m_WSAdvisor.Value;
            Translation.VernacularWritingSystemName = m_WSVernacular.Value;

            // Footnotes
            Translation.FootnoteSequenceType = DFoot.TypeFromString(
                m_FootnoteLetterType.Value);
            ParseFootnoteCustomSequence();

            return true;
        }
        #endregion

        // Literate Settings: Other ----------------------------------------------------------
        #region Attr{g}: LiterateSettingsWnd LS
        LiterateSettingsWnd LS
        {
            get
            {
                return m_LiterateSettings;
            }
        }
        #endregion
        StringChoiceSetting m_WSAdvisor;
        StringChoiceSetting m_WSVernacular;
        StringChoiceSetting m_FootnoteLetterType;
        EditTextSetting m_FootnoteCustomSequence;
        #region Method: void ParseFootnoteCustomSequence()
        void ParseFootnoteCustomSequence()
        {
            string sIn = m_FootnoteCustomSequence.Value;
            string[] vs = sIn.Split(new char[] { ' ', ',' });

            if (null == vs || vs.Length == 0)
                return;

            Translation.FootnoteCustomSeq.Clear();
            foreach (string s in vs)
            {
                string sTrimmed = s.Trim();
                if (!string.IsNullOrEmpty(sTrimmed))
                    Translation.FootnoteCustomSeq.Append(sTrimmed);
            }
        }
        #endregion
        #region Method: void BuildLiterateSettings()
        void BuildLiterateSettings()
        {
            // Get the list of currently-defined writing systems
            string[] vsWritingSystems = new string[DB.StyleSheet.WritingSystems.Count];
            for (int i = 0; i < DB.StyleSheet.WritingSystems.Count; i++)
                vsWritingSystems[i] = (DB.StyleSheet.WritingSystems[i] as JWritingSystem).Name;

            // Writing Systems
            string sGroupWritingSystems = G.GetLoc_String("kWritingSystems", "Writing Systems");
            LS.AddInformation("tr100", Information.PStyleHeading1,
                "Writing Systems");
            LS.AddInformation("tr110", Information.PStyleNormal,
                "Writing Systems are defined in another section of this Configuration Dialog. " +
                "They have information such as which keyboard to use when typing, autor-replace, " +
                "which letters constitute punctuation, and how to do hyphenation. Once these " +
                "have been defined, you need to tell OurWord which writing system to use for this " +
                "translation.");
            LS.AddInformation("tr120", Information.PStyleNormal,
                "The Vernacular Writing System is the one in which the translation is done. Thus " +
                "it will be used in the right-hand column of the drafting view.");
            LS.AddInformation("tr130", Information.PStyleNormal,
                "The Advisor Writing System is the one in which materials are typically created " +
                "for the advisor or consultant. Thus it is the one used in the right-hand column " +
                "of the back translation view.");

            m_WSVernacular = LS.AddAtringChoice(
                "trVernacular",
                "Vernacular",
                "The writing system that this translation is being drafted in.",
                Translation.VernacularWritingSystemName,
                vsWritingSystems);
            m_WSVernacular.Group = sGroupWritingSystems;

            m_WSAdvisor = LS.AddAtringChoice(
                "trAdvisor",
                "Advvisor",
                "The writing system for the back translation and other materials the advisor or consultant might use.",
                Translation.ConsultantWritingSystemName,
                vsWritingSystems);
            m_WSAdvisor.Group = sGroupWritingSystems;

            // Footnotes
            string sGroupFootnotes = G.GetLoc_String("kFootnotes", "Footnotes");
            LS.AddInformation("tr200", Information.PStyleHeading1,
                "Footnote Letter Sequence");
            LS.AddInformation("tr210", Information.PStyleNormal,
                "When a footnote is encountered within the Scriptures, it typically is signaled " +
                "by a letter (such as 'a', 'b', 'c', etc.). By looking for that letter at the " +
                "bottom of the page one can quickly see the corresponding footnote paragraph.");
            LS.AddInformation("tr220", Information.PStyleNormal,
                "OurWord defaults to the \"a, b, c, ...\" sequence, which is suitable for many " +
                "alphabets. But a great many languages will need to define a different sequence. " +
                "Thus if you cannot use one of the built-in sequences, you should enter your own.");
            LS.AddInformation("tr230", Information.PStyleNormal,
                "First, choose either one of the built-in sequences, or indicate that you are " +
                "using a \"custom\" sequence that you will define.");

            m_FootnoteLetterType = LS.AddAtringChoice(
                "trFootnoteLetterType",
                "Letter Sequence",
                "Define the sequence of footnote callout letters. Use one of the OurWord built-in " +
                    " sequences, or set to 'custom' to define your own sequence.",
                DFoot.TypeToString(Translation.FootnoteSequenceType),
                DFoot.FootnoteSequenceChoices);
            m_FootnoteLetterType.Group = sGroupFootnotes;

            LS.AddInformation("tr240", Information.PStyleNormal,
                "To define a custom sequence, enter its letters, with spaces in-between. This " +
                "allows a means for you to specify multi-letter combinations, e.g., the 'll' " +
                "and 'ng' in \"...j k l ll m n ng o p...\"");

            string sEditText = "";
            foreach (string s in Translation.FootnoteCustomSeq)
                sEditText += (s + ' ');
            m_FootnoteCustomSequence = LS.AddEditText(
                "trFootnoteCustom",
                "Custom Sequence",
                "If you want to use a custom sequence, set the Letter Sequence to 'custom', then " +
                    "define you sequence here, using spaces as separators.",
                sEditText);
            m_FootnoteCustomSequence.Group = sGroupFootnotes;

            LS.AddInformation("tr250", Information.PStyleNormal,
                "If you have more footnotes than you have letters, then OurWord will just roll " +
                "over back to the beginning. E.g., after 'z' comes 'a'.");

            // Layout, etc
            LS.LoadContents();
        }
        #endregion

        // Show Filter -----------------------------------------------------------------------
        enum FilterOn { kAll, kNT, kOT, kStartedBooks };
        FilterOn m_FilterOn = FilterOn.kNT;
        #region Cmd: cmdFilterOnAll
        private void cmdFilterOnAll(object sender, EventArgs e)
        {
            if (m_radioAll.Checked)
            {
                m_FilterOn = FilterOn.kAll;
                UpdateFilter();
            }
        }
        #endregion
        #region Cmd: cmdFilterOnNT
        private void cmdFilterOnNT(object sender, EventArgs e)
        {
            if (m_radioNewTestament.Checked)
            {
                m_FilterOn = FilterOn.kNT;
                UpdateFilter();
            }
        }
        #endregion
        #region Cmd: cmdFilterOnOT
        private void cmdFilterOnOT(object sender, EventArgs e)
        {
            if (m_radioOldTestament.Checked)
            {
                m_FilterOn = FilterOn.kOT;
                UpdateFilter();
            }
        }
        #endregion
        #region Cmd: cmdFilterOnStartedBooks
        private void cmdFilterOnStartedBooks(object sender, EventArgs e)
        {
            if (m_radioStartedBooks.Checked)
            {
                m_FilterOn = FilterOn.kStartedBooks;
                UpdateFilter();
            }
        }
        #endregion
        #region Method: void UpdateFilter()
        void UpdateFilter()
        {
            m_radioAll.Checked = (m_FilterOn == FilterOn.kAll);
            m_radioNewTestament.Checked = (m_FilterOn == FilterOn.kNT);
            m_radioOldTestament.Checked = (m_FilterOn == FilterOn.kOT);
            m_radioStartedBooks.Checked = (m_FilterOn == FilterOn.kStartedBooks);

            // The grid will apply the filter
            PopulateGrid(SelectedBookAbbrev);
        }
        #endregion

        // Books Data Grid -------------------------------------------------------------------
        #region Method: void PopulateGrid(string sBookAbbrevToSelect)
        void PopulateGrid(string sBookAbbrevToSelect)
		{
            // In case book names have been changed, we want to harvest them before we
            // regenerate this.
            HarvestBookNames();

            // Clear out whatever was there
			m_gridBooks.Rows.Clear();

            int iMatthew = 39;

			// One row per book
			for (int i = 0; i < DBook.BookAbbrevsCount; i++)
			{
				string sBookAbbrev = DBook.BookAbbrevs[i];

				// If the book exists in the translation, get its stage
				DBook book = Translation.FindBook(sBookAbbrev);
				string sStage = ((null == book) ? "" : book.TranslationStage.Name);

                // Apply the Show filter
                switch (m_FilterOn)
                {
                    case FilterOn.kNT:
                        if (i < iMatthew)
                            continue;
                        break;
                    case FilterOn.kOT:
                        if (i >= iMatthew)
                            continue;
                        break;
                    case FilterOn.kStartedBooks:
                        if (null == book)
                            continue;
                        break;
                }

                // Put together the objects that go into the row's cells, then
                // create and add the row
				object[] v = 
				{
					sBookAbbrev,
					Translation.BookNamesTable[i],
					sStage
				};
				DataGridViewRow row = new DataGridViewRow();
				row.CreateCells(m_gridBooks, v);
                row.Tag = sBookAbbrev;
				m_gridBooks.Rows.Add(row);

				// If the book doesn't exist, gray out the stage cell
                if (null == book)
                    row.Cells[2].Style.BackColor = BackColor;

                // For locked books, set the forecolor to red
                foreach (DataGridViewCell cell in row.Cells)
                {
                    cell.Style.ForeColor =
                        (null != book && book.Locked) ?
                        Color.Red :
                        Color.Black;
                }

                // Select this row?
                if (sBookAbbrev == sBookAbbrevToSelect)
                    row.Selected = true;
			}
        }
        #endregion
        #region Attr{g/s}: string SelectedBookAbbrev
        string SelectedBookAbbrev
        {
            get
            {
                if (m_gridBooks.SelectedRows.Count != 1)
                    return null;
                return (string)m_gridBooks.SelectedRows[0].Tag;
            }
            set
            {
                // Find the row in question, and select it
                foreach (DataGridViewRow row in m_gridBooks.Rows)
                {
                    if ((string)row.Tag == value)
                    {
                        row.Selected = true;
                        return;
                    }
                }
            }
        }
        #endregion
        #region VAttr{g}: DBook SelectedBook
        DBook SelectedBook
        {
            get
            {
                string sAbbrev = SelectedBookAbbrev;
                if (!string.IsNullOrEmpty(sAbbrev))
                    return Translation.FindBook(sAbbrev);
                return null;
            }
        }
        #endregion
        #region Method: void HarvestBookNames()
        void HarvestBookNames()
        {
            foreach (DataGridViewRow row in m_gridBooks.Rows)
            {
                string sAbbrev = (string)row.Cells[0].Value;
                string sName = (string)row.Cells[1].Value;

                // Don't do anything if it was set to empty; assume that
                // was a user mistake.
                if (string.IsNullOrEmpty(sName))
                    continue;

                int iBook = DBook.FindBookAbbrevIndex(sAbbrev);
                if (-1 != iBook)
                {
                    // Update the name in the table
                    Translation.BookNamesTable[iBook] = sName;

                    // Update the book's display name
                    DBook book = Translation.FindBook(sAbbrev);
                    if (null != book)
                        book.DisplayName = sName;
                }
            }
        }
        #endregion

        // Copy Book Names -------------------------------------------------------------------
        #region ComboBox ComboLanguage
        ComboBox ComboLanguage
        {
            get
            {
                return m_comboLanguage;
            }
        }
        #endregion
        #region Method: bool ComboBoxHasPossiblity(string sLanguageName)
        bool ComboBoxHasPossiblity(string sPossibility)
        {
            foreach (string s in ComboLanguage.Items)
            {
                if (s == sPossibility)
                    return true;
            }
            return false;
        }
        #endregion
        #region Method: void PopulateComboBoxPossibilities()
        void PopulateComboBoxPossibilities()
        {
            // Start with an empty box
            ComboLanguage.Items.Clear();

            // Put in English, as a language we always have
            ComboLanguage.Items.Add("English");
            ComboLanguage.Text = "English";

            // UI Languages
            if (null != LocDB.DB.PrimaryLanguage)
            {
                if (!ComboBoxHasPossiblity(LocDB.DB.PrimaryLanguage.Name))
                {
                    ComboLanguage.Items.Add(LocDB.DB.PrimaryLanguage.Name);
                    ComboLanguage.Text = LocDB.DB.PrimaryLanguage.Name;
                }
            }
            if (null != LocDB.DB.SecondaryLanguage)
            {
                if (!ComboBoxHasPossiblity(LocDB.DB.SecondaryLanguage.Name))
                {
                    ComboLanguage.Items.Add(LocDB.DB.SecondaryLanguage.Name);
                }
            }

            // Put in the FileName language (if different)
            DTeamSettings ts = DB.TeamSettings;
            string sFileNameLang = ts.FileNameLanguage;
            if (!ComboBoxHasPossiblity(sFileNameLang))
            {
                ComboLanguage.Items.Add(sFileNameLang);
                ComboLanguage.Text = sFileNameLang;
            }

            // Put in the Front Translation (if this isn't the Front)
            if (null != DB.Project.FrontTranslation && DB.Project.FrontTranslation != Translation)
            {
                string sFrontLang = DB.Project.FrontTranslation.DisplayName;
                if (!ComboBoxHasPossiblity(sFrontLang))
                {
                    ComboLanguage.Items.Add(sFrontLang);
                    ComboLanguage.Text = sFrontLang;
                }
            }

            // Put in the Target Translation (if this isn't the Target) but from here
            // on out, we don't select it in the combo box (thus leaving a Resources
            // language as the default.
            if (null != DB.Project.TargetTranslation && DB.Project.TargetTranslation != Translation)
            {
                string sTargetLang = DB.Project.TargetTranslation.DisplayName;
                if (!ComboBoxHasPossiblity(sTargetLang))
                    ComboLanguage.Items.Add(sTargetLang);
            }

            // Put in any other translations
            foreach (DTranslation t in DB.Project.OtherTranslations)
            {
                if (t != Translation && !ComboBoxHasPossiblity(t.DisplayName))
                    ComboLanguage.Items.Add(t.DisplayName);
            }
        }
        #endregion
        #region Cmd: cmdCopyBookNames
        private void cmdCopyBookNames(object sender, EventArgs e)
        {
            // Language Desired
            string sLanguage = ComboLanguage.Text;

            // We will put the source table here
            string[] vsBookNamesSource = null;

            // Locate the source BookNames table: DTranslations as source
            if (null != DB.Project.FrontTranslation && DB.Project.FrontTranslation.DisplayName == sLanguage)
                vsBookNamesSource = DB.Project.FrontTranslation.BookNamesTable.GetCopy();
            if (null != DB.Project.TargetTranslation && DB.Project.TargetTranslation.DisplayName == sLanguage)
                vsBookNamesSource = DB.Project.TargetTranslation.BookNamesTable.GetCopy();
            foreach (DTranslation t in DB.Project.OtherTranslations)
            {
                if (t.DisplayName == sLanguage)
                    vsBookNamesSource = t.BookNamesTable.GetCopy();
            }

            // Locate the source BookNames table: UI Languages as source
            foreach (LocLanguage language in LocDB.DB.Languages)
            {
                if (sLanguage != language.Name)
                    continue;

                vsBookNamesSource = BookNames.GetTable(sLanguage);
            }

            // English
            if (sLanguage == "English")
                vsBookNamesSource = BookNames.English;

            // Give up if still not found
            if (null == vsBookNamesSource)
                return;

            // Populate the translation
            Translation.BookNamesTable.ReplaceAll(vsBookNamesSource);

            // Update the cross references in the loaded book
            if (null != DB.Project.FrontTranslation &&
                DB.Project.FrontTranslation.DisplayName == sLanguage &&
                Translation == DB.Project.TargetTranslation)
            {
                DB.Project.TargetTranslation.UpdateFromFront();
            }

            // Recalculate the grid
            PopulateGrid(SelectedBookAbbrev);
        }
        #endregion

		// Command Handlers ------------------------------------------------------------------
		#region Cmd: cmdLoad - Populate the controls
		private void cmdLoad(object sender, System.EventArgs e)
		{
            // Localization
            Control[] vExclude = { };
            LocDB.Localize(this, vExclude);

			// Language Name
			LanguageName = Translation.DisplayName;

            // Determine how much to filter. Are there any OT books present? (if so, the
            // first one will be, since they are stored in cannonical order.)
            bool bOTBookPresent = false;
            if (Translation.Books.Count > 0 && Translation.Books[0].IsOldTestamentBook)
                bOTBookPresent = true;
            m_FilterOn = ((bOTBookPresent) ? FilterOn.kAll : FilterOn.kNT);

            if (bOTBookPresent)
                m_radioAll.Checked = true;
            else
                m_radioNewTestament.Checked = true;

			// Populate the list of books; select the first item in the list
			PopulateGrid("GEN");

			// Hide the CreateBook button if requested (Front books cannot be created,
            // they can only be imported. Move the other buttons up.
            if (SuppressCreateBook)
            {
                m_bCreate.Visible = false;
                m_bProperties.Location = m_bRemove.Location;
                m_bRemove.Location = m_bExport.Location;
                m_bExport.Location = m_bImport.Location;
                m_bImport.Location = m_bCreate.Location;
            }

            // Combo Box Possibilities
            PopulateComboBoxPossibilities();

            BuildLiterateSettings();
		}
		#endregion
		#region Cmd: cmdRemoveTranslation
		private void cmdRemoveTranslation(object sender, EventArgs e)
        {
            // Query the user to make certain
            if (!Messages.VerifyRemoveTranslation())
                return;

            // Save the OTrans file in case anything has changed
            ParentDlg.HarvestChangesFromCurrentPage();
            Translation.Write(G.CreateProgressIndicator());

            // Remove it from the appropriate object in the Properties
            bool bWasFront = false;
            bool bWasOther = false;
            if (DB.Project.FrontTranslation == Translation)
            {
                DB.Project.FrontTranslation = null;
                bWasFront = true;
            }
            else if (DB.Project.TargetTranslation == Translation)
            {
                DB.Project.TargetTranslation = null;
            }
            else
            {
                DB.Project.OtherTranslations.Remove(Translation);
                bWasOther = true;
            }

            // Regenerate the dialog 
			if (bWasOther)
			{
                ParentDlg.InitNavigation(Page_OtherTranslations.c_sID);
			}
			else
			{
				string sActivatePage = (bWasFront ?
					Page_SetupFront.c_sID : Page_SetupTarget.c_sID);
                ParentDlg.InitNavigation(sActivatePage);
			}
        }
        #endregion

		#region Cmd: cmdDataGridResized - adjust internal column width so we don't horizontal scroll
		private void cmdDataGridResized(object sender, EventArgs e)
		{
			int iBookNameColumn = 1;

			// Start with our available width
			int nAvailableWidth = m_gridBooks.ClientRectangle.Width;

			// Subtract a little kludge for the borders
			nAvailableWidth -= 4;

			// Subtract the scroll bar's width
			nAvailableWidth -= SystemInformation.VerticalScrollBarWidth;

			// Subtract the widths of the columns, except for the BookNames
			// column (column[1]), which is the one we'll be setting
			for (int i = 0; i < m_gridBooks.ColumnCount; i++)
			{
				if (i != iBookNameColumn)
					nAvailableWidth -= m_gridBooks.Columns[i].Width;
			}

			// Stick with a minimum size in case the grid gets too small
			// Of course, this will cause the horizontal scroll bar to appear
			nAvailableWidth = Math.Max(nAvailableWidth, 100);

			// Set the book-name column
			m_gridBooks.Columns[iBookNameColumn].Width = nAvailableWidth;
		}
		#endregion
		#region Cmd: cmdGridSelectionChanged
		private void cmdGridSelectionChanged(object sender, EventArgs e)
		{
			// From the abbreviation, we can see if the translation defines this book
            DBook book = Translation.FindBook(SelectedBookAbbrev);

			// Disable/Enable buttons accordingly
			m_bCreate.Enabled = (book == null);
			m_bImport.Enabled = (book == null);
			m_bExport.Enabled = (book != null);
			m_bRemove.Enabled = (book != null);
			m_bProperties.Enabled = (book != null);
		}
		#endregion

		// Book Command Handlers -------------------------------------------------------------
		#region Cmd: cmdRemoveBook - Remove Book button clicked
		private void cmdRemoveBook(object sender, System.EventArgs e)
		{
			// Get the selection
			DBook book = SelectedBook;
			if (null == book)
				return;

			// Make sure the user wants to remove the book.
			if (!Messages.VerifyRemoveBook())  
				return;

			// Remove the book from the translation & refresh the listview
			Translation.Books.Remove(book);

            // Update the property page
            PopulateGrid(book.BookAbbrev);
		}
		#endregion
        #region Cmd: cmdBookProperties - either via Button or Double-Click
        private void cmdBookProperties(object sender, System.EventArgs e)
		{
			// Get the current selection
			DBook book = SelectedBook;
			if (null == book)
				return;

			// Launch the dialog
			Debug.Assert(null != Translation);
			Debug.Assert(null != Translation.Project);
            DBookProperties dlg = new DBookProperties(
                Translation.Project.FrontTranslation, Translation, book);
            dlg.ShowDialog();

            // Update the property page
            PopulateGrid(book.BookAbbrev);
		}
		#endregion
        #region Cmd: cmdImportBook
        private void cmdImportBook(object sender, System.EventArgs e)
		{
            // Show the wizard; the user will input the needed information and
            // indicate whether or not to proceed.
            Dialogs.WizImportBook.WizImportBook wizard =
                new Dialogs.WizImportBook.WizImportBook(Translation);
            if (DialogResult.OK != wizard.ShowDialog())
                return;

			// Make sure it isn't already in the translation
			DBook bookExisting = Translation.FindBook(wizard.BookAbbrev);
			if (null != bookExisting)
			{
				if (Messages.VerifyReplaceBook())
				{
					if (bookExisting.StoragePath != wizard.ImportFileName &&
						File.Exists(bookExisting.StoragePath))
					{
						File.Delete(bookExisting.StoragePath);
					}
					Translation.Books.Remove(bookExisting);
				}
				else
					return;
			}

            // Create a book object
            DBook book = new DBook(wizard.BookAbbrev);

            // Add it to the translation (we must do this or book.LoadData cannot
            // properly check for errors.)
            Translation.AddBook(book);

			// We can now get a filename from the book. Let's make sure that file doesn't
			// already exist. If it does, then let the user abort.
			if (File.Exists(book.StoragePath) && book.StoragePath != wizard.ImportFileName)
			{
				if (!Messages.VerifyOverwriteBook())
				{
					Translation.Books.Remove(book);
					return;
				}
			}

            // Attempt to read it in
            Debug.Assert(!book.Loaded);
			string sImportPath = wizard.ImportFileName;
            book.Load(ref sImportPath, G.CreateProgressIndicator());
            if (!book.Loaded)
            {
                Translation.Books.Remove(book);
                return;
            }

            // If successful, we now need to write it out to the desired path; this
            // will put it into our file format as well.
            book.DisplayName = book.BookName;
            book.DeclareDirty();  // Make certain this will be written to file
            book.Unload(G.CreateProgressIndicator());    // Writes the file

            // Update the property page
            PopulateGrid(book.BookAbbrev);
		}
		#endregion
        #region Cmd: cmdCreateBook
        private void cmdCreateBook(object sender, System.EventArgs e)
		{
            // Get the book we wish to create from the selected row
            string sAbbrev = SelectedBookAbbrev;
            if (string.IsNullOrEmpty(sAbbrev))
                return;
            string sBookName = DBook.GetBookName(sAbbrev, Translation);

            // Make sure the book exists in the Front
            DBook bFront = DB.FrontTranslation.FindBook(sAbbrev);
            if (null == bFront)
            {
                LocDB.Message("msgNoCorrespondingBookInFront",
                    "OurWord uses the Source translation as a template when it creates a book\n" +
                    "in the Target. Therefore, you need to first import {0} into\n" +
                    "{1}, before you will be able to create it here in {2}.",
                    new string[] { sBookName, DB.FrontTranslation.DisplayName, Translation.DisplayName },
                    LocDB.MessageTypes.Error);
                return;
            }

            // Get confirmation as a courtesy to the user
            bool bProceed = LocDB.Message("msgConfirmCreateBook",
                "Do you want OurWord to create a blank book for drafting {0} into {1}?",
                new string[] { sBookName, Translation.DisplayName },
                LocDB.MessageTypes.YN);
            if (!bProceed)
                return;

            // Create the book, with "Drafting" defaults
            DBook book = new DBook(sAbbrev);
            int iBook = DBook.FindBookAbbrevIndex(sAbbrev);
            if (-1 == iBook)
                return;
            book.DisplayName = Translation.BookNamesTable[iBook];

            Translation.AddBook(book);
            if (false == book.InitializeFromFrontTranslation(G.CreateProgressIndicator()))
            {
                Translation.Books.Remove(book);
                return;
            }
            book.Write(G.CreateProgressIndicator());
            PopulateGrid(sAbbrev);
        }
		#endregion
        #region Cmd: cmdExportBook
        private void cmdExportBook(object sender, EventArgs e)
        {
            // Get the selection
            DBook book = SelectedBook;
            if (null == book)
                return;

            // Get the user's desired destination (or cancel)
            DialogExportBook dlg = new DialogExportBook(book);
            if (DialogResult.OK != dlg.ShowDialog(ParentDlg))
                return;

            // Export the book
            IProgressIndicator progress = G.CreateProgressIndicator();
            book.Export(dlg.ExportPathName, ScriptureDB.Formats.kParatext, progress);
        }
        #endregion

	}

}
