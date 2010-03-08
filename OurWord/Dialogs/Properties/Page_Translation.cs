#region ***** Page_Translation.cs *****
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
using OurWord.Edit;
using OurWord.Layouts;
using OurWordData.Styles;

#endregion
#endregion

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
        private RadioButton m_radioStartedBooks;
        private RadioButton m_radioOldTestament;
        private RadioButton m_radioNewTestament;
        private RadioButton m_radioAll;
        private LiterateSettingsWnd m_LiterateSettings;
        private ToolStrip m_ToolStrip;
        private ToolStripButton m_bCreate;
        private ToolStripButton m_bImport;
        private ToolStripButton m_bRemove;
        private ToolStripButton m_bProperties;
        private ToolStripDropDownButton m_bCopyNames;
        private ToolStripButton m_bEditRawFile;
        private ToolStripSeparator toolStripSeparator1;
        private DataGridViewTextBoxColumn m_colAbbreviation;
        private DataGridViewTextBoxColumn m_colBookName;
        private DataGridViewComboBoxColumn m_colStage;
        private ToolStripSeparator toolStripSeparator2;
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

            // Set up the Literate Settings contents
            BuildLiterateSettings();

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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Page_Translation));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.m_lblLanguageName = new System.Windows.Forms.Label();
            this.m_editLanguageName = new System.Windows.Forms.TextBox();
            this.m_btnRemoveTranslation = new System.Windows.Forms.Button();
            this.m_tabctrlTranslation = new System.Windows.Forms.TabControl();
            this.m_tabBooks = new System.Windows.Forms.TabPage();
            this.m_ToolStrip = new System.Windows.Forms.ToolStrip();
            this.m_bCreate = new System.Windows.Forms.ToolStripButton();
            this.m_bImport = new System.Windows.Forms.ToolStripButton();
            this.m_bRemove = new System.Windows.Forms.ToolStripButton();
            this.m_bProperties = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.m_bEditRawFile = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.m_bCopyNames = new System.Windows.Forms.ToolStripDropDownButton();
            this.m_radioStartedBooks = new System.Windows.Forms.RadioButton();
            this.m_radioOldTestament = new System.Windows.Forms.RadioButton();
            this.m_radioNewTestament = new System.Windows.Forms.RadioButton();
            this.m_radioAll = new System.Windows.Forms.RadioButton();
            this.m_gridBooks = new System.Windows.Forms.DataGridView();
            this.m_colAbbreviation = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.m_colBookName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.m_colStage = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.m_tabOther = new System.Windows.Forms.TabPage();
            this.m_LiterateSettings = new OurWord.Edit.LiterateSettingsWnd();
            this.m_tabctrlTranslation.SuspendLayout();
            this.m_tabBooks.SuspendLayout();
            this.m_ToolStrip.SuspendLayout();
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
            this.m_tabBooks.Controls.Add(this.m_ToolStrip);
            this.m_tabBooks.Controls.Add(this.m_radioStartedBooks);
            this.m_tabBooks.Controls.Add(this.m_radioOldTestament);
            this.m_tabBooks.Controls.Add(this.m_radioNewTestament);
            this.m_tabBooks.Controls.Add(this.m_radioAll);
            this.m_tabBooks.Controls.Add(this.m_gridBooks);
            this.m_tabBooks.Location = new System.Drawing.Point(4, 22);
            this.m_tabBooks.Name = "m_tabBooks";
            this.m_tabBooks.Padding = new System.Windows.Forms.Padding(3);
            this.m_tabBooks.Size = new System.Drawing.Size(460, 339);
            this.m_tabBooks.TabIndex = 4;
            this.m_tabBooks.Text = "Books";
            this.m_tabBooks.UseVisualStyleBackColor = true;
            // 
            // m_ToolStrip
            // 
            this.m_ToolStrip.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.m_ToolStrip.AutoSize = false;
            this.m_ToolStrip.Dock = System.Windows.Forms.DockStyle.None;
            this.m_ToolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.m_ToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_bCreate,
            this.m_bImport,
            this.m_bRemove,
            this.m_bProperties,
            this.toolStripSeparator1,
            this.m_bEditRawFile,
            this.toolStripSeparator2,
            this.m_bCopyNames});
            this.m_ToolStrip.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.VerticalStackWithOverflow;
            this.m_ToolStrip.Location = new System.Drawing.Point(343, 4);
            this.m_ToolStrip.Name = "m_ToolStrip";
            this.m_ToolStrip.Size = new System.Drawing.Size(111, 184);
            this.m_ToolStrip.TabIndex = 113;
            this.m_ToolStrip.Text = "ToolStrip";
            // 
            // m_bCreate
            // 
            this.m_bCreate.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.m_bCreate.Image = ((System.Drawing.Image)(resources.GetObject("m_bCreate.Image")));
            this.m_bCreate.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.m_bCreate.Name = "m_bCreate";
            this.m_bCreate.Size = new System.Drawing.Size(109, 19);
            this.m_bCreate.Text = "Create....";
            this.m_bCreate.Click += new System.EventHandler(this.cmdCreateBook);
            // 
            // m_bImport
            // 
            this.m_bImport.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.m_bImport.Image = ((System.Drawing.Image)(resources.GetObject("m_bImport.Image")));
            this.m_bImport.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.m_bImport.Name = "m_bImport";
            this.m_bImport.Size = new System.Drawing.Size(109, 19);
            this.m_bImport.Text = "Import...";
            this.m_bImport.Click += new System.EventHandler(this.cmdImportBook);
            // 
            // m_bRemove
            // 
            this.m_bRemove.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.m_bRemove.Image = ((System.Drawing.Image)(resources.GetObject("m_bRemove.Image")));
            this.m_bRemove.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.m_bRemove.Name = "m_bRemove";
            this.m_bRemove.Size = new System.Drawing.Size(109, 19);
            this.m_bRemove.Text = "Delete...";
            this.m_bRemove.Click += new System.EventHandler(this.cmdRemoveBook);
            // 
            // m_bProperties
            // 
            this.m_bProperties.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.m_bProperties.Image = ((System.Drawing.Image)(resources.GetObject("m_bProperties.Image")));
            this.m_bProperties.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.m_bProperties.Name = "m_bProperties";
            this.m_bProperties.Size = new System.Drawing.Size(109, 19);
            this.m_bProperties.Text = "Properties...";
            this.m_bProperties.Click += new System.EventHandler(this.cmdBookProperties);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(109, 6);
            // 
            // m_bEditRawFile
            // 
            this.m_bEditRawFile.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.m_bEditRawFile.Image = ((System.Drawing.Image)(resources.GetObject("m_bEditRawFile.Image")));
            this.m_bEditRawFile.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.m_bEditRawFile.Name = "m_bEditRawFile";
            this.m_bEditRawFile.Size = new System.Drawing.Size(109, 19);
            this.m_bEditRawFile.Text = "Edit Raw File...";
            this.m_bEditRawFile.Click += new System.EventHandler(this.cmdEditRawFile);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(109, 6);
            // 
            // m_bCopyNames
            // 
            this.m_bCopyNames.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.m_bCopyNames.Image = ((System.Drawing.Image)(resources.GetObject("m_bCopyNames.Image")));
            this.m_bCopyNames.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.m_bCopyNames.Name = "m_bCopyNames";
            this.m_bCopyNames.Size = new System.Drawing.Size(109, 19);
            this.m_bCopyNames.Text = "Copy Names";
            this.m_bCopyNames.ToolTipText = "Copy book names from  another language (the dropdown shows the possible choices);" +
                " this is a short cut over having to enter each of the 66 names one-at-a-time.";
            // 
            // m_radioStartedBooks
            // 
            this.m_radioStartedBooks.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.m_radioStartedBooks.Location = new System.Drawing.Point(343, 265);
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
            this.m_radioOldTestament.Location = new System.Drawing.Point(343, 247);
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
            this.m_radioNewTestament.Location = new System.Drawing.Point(343, 229);
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
            this.m_radioAll.Location = new System.Drawing.Point(343, 211);
            this.m_radioAll.Name = "m_radioAll";
            this.m_radioAll.Size = new System.Drawing.Size(111, 17);
            this.m_radioAll.TabIndex = 109;
            this.m_radioAll.TabStop = true;
            this.m_radioAll.Text = "All";
            this.m_radioAll.UseVisualStyleBackColor = true;
            this.m_radioAll.CheckedChanged += new System.EventHandler(this.cmdFilterOnAll);
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
            this.m_colStage.DisplayStyle = System.Windows.Forms.DataGridViewComboBoxDisplayStyle.ComboBox;
            this.m_colStage.DisplayStyleForCurrentCellOnly = true;
            this.m_colStage.DropDownWidth = 100;
            this.m_colStage.HeaderText = "Stage";
            this.m_colStage.MaxDropDownItems = 15;
            this.m_colStage.MinimumWidth = 40;
            this.m_colStage.Name = "m_colStage";
            this.m_colStage.Resizable = System.Windows.Forms.DataGridViewTriState.True;
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
            this.m_ToolStrip.ResumeLayout(false);
            this.m_ToolStrip.PerformLayout();
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
            HarvestGridData();

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
            var vsWritingSystems = new string[StyleSheet.WritingSystems.Count];
            for (var i = 0; i < StyleSheet.WritingSystems.Count; i++)
                vsWritingSystems[i] = StyleSheet.WritingSystems[i].Name;

            // Writing Systems
            var sGroupWritingSystems = G.GetLoc_String("kWritingSystems", "Writing Systems");
            LS.AddInformation("tr100", StyleSheet.LiterateHeading,
                "Writing Systems");
            LS.AddInformation("tr110", StyleSheet.LiterateParagraph,
                "Writing Systems are defined in another section of this Configuration Dialog. " +
                "They have information such as which keyboard to use when typing, autor-replace, " +
                "which letters constitute punctuation, and how to do hyphenation. Once these " +
                "have been defined, you need to tell OurWord which writing system to use for this " +
                "translation.");
            LS.AddInformation("tr120", StyleSheet.LiterateParagraph,
                "The Vernacular Writing System is the one in which the translation is done. Thus " +
                "it will be used in the right-hand column of the drafting view.");
            LS.AddInformation("tr130", StyleSheet.LiterateParagraph,
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
                "Advisor",
                "The writing system for the back translation and other materials the advisor or consultant might use.",
                Translation.ConsultantWritingSystemName,
                vsWritingSystems);
            m_WSAdvisor.Group = sGroupWritingSystems;

            // Footnotes
            var sGroupFootnotes = G.GetLoc_String("kFootnotes", "Footnotes");
            LS.AddInformation("tr200", StyleSheet.LiterateHeading,
                "Footnote Letter Sequence");
            LS.AddInformation("tr210", StyleSheet.LiterateParagraph,
                "When a footnote is encountered within the Scriptures, it typically is signaled " +
                "by a letter (such as 'a', 'b', 'c', etc.). By looking for that letter at the " +
                "bottom of the page one can quickly see the corresponding footnote paragraph.");
            LS.AddInformation("tr220", StyleSheet.LiterateParagraph,
                "OurWord defaults to the \"a, b, c, ...\" sequence, which is suitable for many " +
                "alphabets. But a great many languages will need to define a different sequence. " +
                "Thus if you cannot use one of the built-in sequences, you should enter your own.");
            LS.AddInformation("tr230", StyleSheet.LiterateParagraph,
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

            LS.AddInformation("tr240", StyleSheet.LiterateParagraph,
                "To define a custom sequence, enter its letters, with spaces in-between. This " +
                "allows a means for you to specify multi-letter combinations, e.g., the 'll' " +
                "and 'ng' in \"...j k l ll m n ng o p...\"");

            var sEditText = "";
            foreach (string s in Translation.FootnoteCustomSeq)
                sEditText += (s + ' ');
            m_FootnoteCustomSequence = LS.AddEditText(
                "trFootnoteCustom",
                "Custom Sequence",
                "If you want to use a custom sequence, set the Letter Sequence to 'custom', then " +
                    "define you sequence here, using spaces as separators.",
                sEditText);
            m_FootnoteCustomSequence.Group = sGroupFootnotes;

            LS.AddInformation("tr250", StyleSheet.LiterateParagraph,
                "If you have more footnotes than you have letters, then OurWord will just roll " +
                "over back to the beginning. E.g., after 'z' comes 'a'.");
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
                UpdateFilter(SelectedBookAbbrev);
            }
        }
        #endregion
        #region Cmd: cmdFilterOnNT
        private void cmdFilterOnNT(object sender, EventArgs e)
        {
            if (!m_radioNewTestament.Checked) 
                return;

            m_FilterOn = FilterOn.kNT;

            var sSelectBook = SelectedBookAbbrev;
            if (!DBook.GetIsNewTestamentBook(SelectedBookAbbrev))
                sSelectBook = "MAT";

            UpdateFilter(sSelectBook);
        }
        #endregion
        #region Cmd: cmdFilterOnOT
        private void cmdFilterOnOT(object sender, EventArgs e)
        {
            if (!m_radioOldTestament.Checked) 
                return;

            m_FilterOn = FilterOn.kOT;

            var sSelectBook = SelectedBookAbbrev;
            if (!DBook.GetIsOldTestamentBook(SelectedBookAbbrev))
                sSelectBook = "GEN";

            UpdateFilter(sSelectBook);
        }
        #endregion
        #region Cmd: cmdFilterOnStartedBooks
        private void cmdFilterOnStartedBooks(object sender, EventArgs e)
        {
            if (m_radioStartedBooks.Checked)
            {
                m_FilterOn = FilterOn.kStartedBooks;
                UpdateFilter(SelectedBookAbbrev);
            }
        }
        #endregion
        #region Method: void UpdateFilter(sBookToSelect)
        void UpdateFilter(string sBookToSelect)
        {
            m_radioAll.Checked = (m_FilterOn == FilterOn.kAll);
            m_radioNewTestament.Checked = (m_FilterOn == FilterOn.kNT);
            m_radioOldTestament.Checked = (m_FilterOn == FilterOn.kOT);
            m_radioStartedBooks.Checked = (m_FilterOn == FilterOn.kStartedBooks);

            // The grid will apply the filter
            PopulateGrid(sBookToSelect, true);
        }
        #endregion
        #region Method: bool GetIsStartedBook(sBookAbbrev)
        bool GetIsStartedBook(string sBookAbbrev)
        {
            if (string.IsNullOrEmpty(sBookAbbrev))
                return false;

            if (null == DB.TargetTranslation)
                return false;

            if (null == DB.TargetTranslation.FindBook(SelectedBookAbbrev))
                return false;

            return true;
        }
        #endregion

        // Books Data Grid -------------------------------------------------------------------
        #region VAttr{g}: string Planned
        static string Planned
        {
            get
            {
                return Loc.GetString("Planned", "Planned");
            }
        }
        #endregion
        #region Method: void SetupStageCombo(DataGridViewComboBoxCell, sBookAbbrev)
        void SetupStageCombo(DataGridViewComboBoxCell combo, string sBookAbbrev)
        {
            Debug.Assert(null != combo);
            var book = Translation.FindBook(sBookAbbrev);

            // If we have a book, then the combo's possibilities are the translation stages
            if (null != book)
            {
                foreach (Stage stage in DB.TeamSettings.Stages)
                    combo.Items.Add(stage.LocalizedName);
                combo.Value = book.Stage.LocalizedName;
            }

            // Otherwise, the possibilities are Planned vs nothing
            if (null == book && Translation == DB.TargetTranslation)
            {
                combo.Items.Add("");
                combo.Items.Add(Planned);
                if (DB.Project.PlannedBooks.Contains(sBookAbbrev))
                    combo.Value = Planned;
            }

        }
        #endregion
        #region Method: DataGridViewRow PopulateRow(int iBook)
        DataGridViewRow PopulateRow(int iBook)
        {
            // Cannonical 3-letter abbreviation
            string sAbbrev = DBook.BookAbbrevs[iBook];

            // Book name in the target language
            string sBookName = Translation.BookNamesTable[iBook];

            // Create the row
            var row = new DataGridViewRow();
            row.CreateCells(m_gridBooks, new object[] { sAbbrev, sBookName, "" });
            row.Tag = sAbbrev;

            // Stage
            var cellCombo = row.Cells[2] as DataGridViewComboBoxCell;
            SetupStageCombo(cellCombo, sAbbrev);

            // Locked books, set to red text
            var book = Translation.FindBook(sAbbrev);
            if (null != book && book.Locked)
            {
                foreach (DataGridViewCell cell in row.Cells)
                    cell.Style.ForeColor = Color.Red;
            }

            return row;
        }
        #endregion
        #region Method: void PopulateGrid(string sBookAbbrevToSelect)
        void PopulateGrid(string sBookAbbrevToSelect, bool bHarvestExistingBookNames)
		{
            // In case book names have been changed, we want to harvest them before we
            // regenerate this. The exception comes from our CopyBookNamesFrom method,
            // where we have new names we want to place into the grid; otherwise
            // HarvestGridData would overwrite our Newly Copied Names with the 
            // previous old names.
            if (bHarvestExistingBookNames)
                HarvestGridData();

            // Clear out whatever was there
			m_gridBooks.Rows.Clear();

			// One row per book
			for (var i = 0; i < DBook.BookAbbrevsCount; i++)
			{
				var sBookAbbrev = DBook.BookAbbrevs[i];

				// If the book exists in the translation, get its stage
				var book = Translation.FindBook(sBookAbbrev);
				var sStage = ((null == book) ? "" : book.Stage.LocalizedName);

                // Apply the Show filter
                switch (m_FilterOn)
                {
                    case FilterOn.kNT:
                        if (i < DBook.c_iMatthew)
                            continue;
                        break;
                    case FilterOn.kOT:
                        if (i >= DBook.c_iMatthew)
                            continue;
                        break;
                    case FilterOn.kStartedBooks:
                        if (null == book)
                            continue;
                        break;
                }

                var row = PopulateRow(i);
				m_gridBooks.Rows.Add(row);

				// If the book doesn't exist, gray out the stage cell
                if (null == book)
                    row.Cells[2].Style.BackColor = BackColor;

                // Select this row?
                if (sBookAbbrev == sBookAbbrevToSelect)
                    row.Selected = true;
			}

            // Make sure something is selected
            if (string.IsNullOrEmpty(SelectedBookAbbrev) && m_gridBooks.Rows.Count > 0)
                m_gridBooks.Rows[0].Selected = true;
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
                    if ((string) row.Tag != value) 
                        continue;
                    row.Selected = true;
                    return;
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
        #region Method: void HarvestGridData()
        void HarvestGridData()
        {
            // The DataGrid does not necessarily commit changes into its cache right away.
            // In particular, the Stage combo is not commited upon something being selected.
            // So this command forces any remaining commits to be done, so that we'll see 
            // them as we harvest.
            m_gridBooks.CommitEdit(DataGridViewDataErrorContexts.Commit);

            foreach (DataGridViewRow row in m_gridBooks.Rows)
            {
                var sAbbrev = (string)row.Cells[0].Value;
                var sName = (string)row.Cells[1].Value;
                var sStage = (string)row.Cells[2].Value;

                // Find the book in question
                var iBook = DBook.FindBookAbbrevIndex(sAbbrev);
                if (-1 == iBook)
                    continue;
                var book = Translation.FindBook(sAbbrev);

                // Update the name if not empty; if it is empty, assume user mistake and
                // don't update it.
                if (!string.IsNullOrEmpty(sName))
                {
                    if (Translation.BookNamesTable[iBook] != sName)
                    {
                        Translation.BookNamesTable[iBook] = sName;
                        Translation.DeclareDirty();
                    }
                    if (null != book && book.DisplayName != sName)
                    {
                        book.LoadBook(new NullProgress());
                        book.DisplayName = sName;
                        book.DeclareDirty();
                    }
                }

                // Update the stage
                if (null != book && !string.IsNullOrEmpty(sStage) && book.Stage.LocalizedName != sStage)
                {
                    book.LoadBook(new NullProgress());
                    book.Stage = DB.TeamSettings.Stages.Find(StageList.FindBy.LocalizedName, sStage);
                    book.DeclareDirty();
                }

                // Update the Planned Books
                if (null == book && Translation == DB.TargetTranslation)
                {
                    var vPlanned = DB.Project.PlannedBooks;
                    if (sStage == Planned && !vPlanned.Contains(sAbbrev))
                    {
                        vPlanned.Add(sAbbrev);
                        DB.Project.PlannedBooks = vPlanned;
                    }
                    if (sStage != Planned && vPlanned.Contains(sAbbrev))
                    {
                        vPlanned.Remove(sAbbrev);
                        DB.Project.PlannedBooks = vPlanned;
                    }

                }
            }
        }
        #endregion

        // Copy Book Names -------------------------------------------------------------------
        #region Method: void AddDropDownItem(sLanguageName)
        void AddDropDownItem(string sLanguageName)
        {
            // Valid item?
            if (string.IsNullOrEmpty(sLanguageName))
                return;

            // If it is already there, don't add it
            foreach (ToolStripItem item in m_bCopyNames.DropDownItems)
            {
                if ((string)item.Tag == sLanguageName)
                    return;
            }

            // Item text
            string sBase = Loc.GetString("kCopyBookNamesFrom", "...from {0}");
            string sText = LocDB.Insert(sBase, new string[] { sLanguageName });

            // Add the item
            ToolStripItem NewItem = m_bCopyNames.DropDownItems.Add(sText);
            NewItem.Tag = sLanguageName;
            NewItem.Click += new EventHandler(cmdCopyBookNames);
        }
        #endregion
        #region Method: void PopulateCopyNamesPossibilities()
        void PopulateCopyNamesPossibilities()
        {
            // Start with an empty dropdown so we don't double things
            m_bCopyNames.DropDownItems.Clear();

            // Put in English, as a language we always have
            AddDropDownItem("English");

            // UI Languages
            if (null != LocDB.DB.PrimaryLanguage)
                AddDropDownItem(LocDB.DB.PrimaryLanguage.Name);
            if (null != LocDB.DB.SecondaryLanguage)
                AddDropDownItem(LocDB.DB.SecondaryLanguage.Name);

            // Put in the Front Translation (if this isn't the Front)
            if (null != DB.Project.FrontTranslation && DB.Project.FrontTranslation != Translation)
                AddDropDownItem(DB.Project.FrontTranslation.DisplayName);

            // Put in the Target Translation (if this isn't the Target) 
            if (null != DB.Project.TargetTranslation && DB.Project.TargetTranslation != Translation)
                AddDropDownItem(DB.Project.TargetTranslation.DisplayName);

            // Put in any other translations
            foreach (DTranslation t in DB.Project.OtherTranslations)
                AddDropDownItem(t.DisplayName);
        }
        #endregion
        #region Cmd: cmdCopyBookNames
        private void cmdCopyBookNames(object sender, EventArgs e)
        {
            // Language Desired
            ToolStripItem item = sender as ToolStripItem;
            Debug.Assert(null != item);
            string sLanguage = (string)item.Tag;
            if (string.IsNullOrEmpty(sLanguage))
                return;

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
            PopulateGrid(SelectedBookAbbrev, false);
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

            // Count books present in each Testament
            int cOT = 0;
            int cNT = 0;
            foreach (DBook book in Translation.BookList)
            {
                if (book.IsOldTestamentBook)
                    cOT++;
                else
                    cNT++;
            }

            // If this is predonimately a NT project, then that's how we want to filter;
            // otherwise we just show all books
            if (cNT > cOT && cOT < 5)
                m_radioNewTestament.Checked = true;
            else
                m_radioAll.Checked = true;

			// Populate the list of books; select the first item in the list
			PopulateGrid("GEN", false);

			// Hide the CreateBook button if requested (Front books cannot be created,
            // they can only be imported. Move the other buttons up.
            if (SuppressCreateBook)
                m_bCreate.Visible = false;

            // Combo Box Possibilities
            PopulateCopyNamesPossibilities();
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
            Translation.WriteToFile(G.CreateProgressIndicator());

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
            var book = Translation.FindBook(SelectedBookAbbrev);

			// Disable/Enable buttons accordingly
			m_bCreate.Enabled = (book == null);
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
            bool bRemove = LocDB.Message(
                "msgVerifyDeleteBook",
                "Do you want to delete this book from the translation?\n\n" +
                "(This will delete the file from the disk; and if you synchronize with others\n" +
                "it will remove it from their computers as well.)",
                null,
                LocDB.MessageTypes.WarningYN);
            if (!bRemove)
                return;

            // Delete the book
            try
            {
                File.Delete(book.StoragePath);
                Translation.BookList.Remove(book);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to delete book: " + ex.Message);
            }

            // Update the property page
            PopulateGrid(book.BookAbbrev, true);
		}
		#endregion
        #region Cmd: cmdBookProperties - either via Button or Double-Click
        private void cmdBookProperties(object sender, System.EventArgs e)
		{
			// Get the current selection
			DBook book = SelectedBook;
			if (null == book)
				return;

            // Make sure the book is loaded, as we can neither see nor edit the 
            // properties otherwise
            book.LoadBook(G.CreateProgressIndicator());

			// Launch the dialog
			Debug.Assert(null != Translation);
			Debug.Assert(null != Translation.Project);
            DBookProperties dlg = new DBookProperties(
                Translation.Project.FrontTranslation, Translation, book);
            dlg.ShowDialog();

            // Update the property page
            PopulateGrid(book.BookAbbrev, true);
		}
		#endregion
        #region Cmd: cmdImportBook
        private void cmdImportBook(object sender, System.EventArgs e)
		{
            // Show the wizard; the user will input the needed information and
            // indicate whether or not to proceed.
            var wizard = new Dialogs.WizImportBook.WizImportBook(Translation);
            if (DialogResult.OK != wizard.ShowDialog())
                return;

			// Make sure it isn't already in the translation
			var bookExisting = Translation.FindBook(wizard.BookAbbrev);
			if (null != bookExisting)
			{
				if (Messages.VerifyReplaceBook())
				{
					if (bookExisting.StoragePath != wizard.ImportFileName &&
						File.Exists(bookExisting.StoragePath))
					{
						File.Delete(bookExisting.StoragePath);
					}
					Translation.BookList.Remove(bookExisting);
				}
				else
					return;
			}

            // Create a book object
            var book = new DBook(wizard.BookAbbrev);

            // Add it to the translation (we must do this or book.LoadData cannot
            // properly check for errors.)
            Translation.AddBook(book);

			// We can now get a filename from the book. Let's make sure that file doesn't
			// already exist. If it does, then let the user abort.
			if (File.Exists(book.StoragePath))
			{
				if (!Messages.VerifyOverwriteBook())
				{
					Translation.BookList.Remove(book);
					return;
				}
			}

            // Attempt to read it in
            Debug.Assert(!book.Loaded);
            book.LoadBook(wizard.ImportFileName, G.CreateProgressIndicator());
            if (!book.Loaded)
            {
                Translation.BookList.Remove(book);
                return;
            }

            // If successful, we now need to write it out to the desired path; this
            // will put it into our file format as well.
            book.DisplayName = book.BookName;
            book.DeclareDirty();  // Make certain this will be written to file
            book.WriteBook(G.CreateProgressIndicator());

            // Update the property page
            PopulateGrid(book.BookAbbrev, true);
		}
		#endregion
        #region Cmd: cmdCreateBook
        private void cmdCreateBook(object sender, EventArgs e)
		{
            // Get the book we wish to create from the selected row
            var sAbbrev = SelectedBookAbbrev;
            if (string.IsNullOrEmpty(sAbbrev))
                return;
            var sBookName = DBook.GetBookName(sAbbrev, Translation);

            // Make sure the book exists in the Front
            var bFront = DB.FrontTranslation.FindBook(sAbbrev);
            if (null == bFront)
            {
                LocDB.Message("msgNoCorrespondingBookInFront",
                    "OurWord uses the Source translation as a template when it creates a book\n" +
                    "in the Target. Therefore, you need to first import {0} into\n" +
                    "{1}, before you will be able to create it here in {2}.",
                    new[] { sBookName, DB.FrontTranslation.DisplayName, Translation.DisplayName },
                    LocDB.MessageTypes.Error);
                return;
            }

            // Get confirmation as a courtesy to the user
            var bProceed = LocDB.Message("msgConfirmCreateBook",
                "Do you want OurWord to create a blank book for drafting {0} into {1}?",
                new[] { sBookName, Translation.DisplayName },
                LocDB.MessageTypes.YN);
            if (!bProceed)
                return;

            // Create the book, with "Drafting" defaults
            var book = new DBook(sAbbrev);
            var iBook = DBook.FindBookAbbrevIndex(sAbbrev);
            if (-1 == iBook)
                return;
            book.DisplayName = Translation.BookNamesTable[iBook];

            Translation.AddBook(book);
            Cursor.Current = Cursors.WaitCursor;
            if (false == book.InitializeFromFrontTranslation(G.CreateProgressIndicator()))
            {
                Translation.BookList.Remove(book);
                Cursor.Current = Cursors.Default;
                return;
            }
            book.WriteBook(G.CreateProgressIndicator());
            Cursor.Current = Cursors.Default;
            PopulateGrid(sAbbrev, true);
        }
		#endregion
        #region Cmd: cmdEditRawFile
        private void cmdEditRawFile(object sender, EventArgs e)
        {
            if (null == SelectedBook)
                return;

            // Put up the dialog and allow the user to edit
            var dlg = new DlgRawFileEdit(SelectedBook);
            if (dlg.ShowDialog(ParentDlg) != DialogResult.OK)
                return;

            // If they did edit, we must unload the book so that it will be re-loaded
            SelectedBook.Unload(new NullProgress());
        }
        #endregion
    }

}
