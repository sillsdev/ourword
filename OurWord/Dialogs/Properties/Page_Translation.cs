/**********************************************************************************************
 * Project: Our Word!
 * File:    Page_Translation.cs
 * Author:  John Wimbish
 * Created: 28 Dec 2004
 * Purpose: Edit the properties of a DTranslation.
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

using NUnit.Framework;

using JWTools;
using JWdb;
using OurWord;
using OurWord.DataModel;
using OurWord.Dialogs;
using OurWord.View;
#endregion

namespace OurWord.Dialogs
{
    public class Page_Translation : DlgPropertySheet
	{
		// Attrs -----------------------------------------------------------------------------
		#region Attr{g}: DTranslation Trans
		DTranslation Trans
		{
			get
			{
				Debug.Assert(null != m_Trans);
				return m_Trans;
			}
		}
		DTranslation m_Trans = null;
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
		#region Attr{g}: DBook CurrentlySelectedBook - null if nothing is selected
		DBook CurrentlySelectedBook
		{
			get
			{
				// If nothing selected, there is nothing to do.
				if (m_listviewBooks.SelectedItems.Count == 0)
					return null;

				// Get the current selection
				ListViewItem item = m_listviewBooks.SelectedItems[0];
				return Trans.FindBook(item.Text);
			}
		}
		#endregion
		#region Attr{g}: DBook CurrentBook - the currently-displayed book
		DBook CurrentBook
		{
			get
			{
				DSection section = OurWordMain.Project.STarget;
				if (null == section)
					return null;
				return section.Book;
			}
		}
		#endregion

		// Control Contents ------------------------------------------------------------------
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
		#region Attr{g}: ListViewItemCollection BookList
		ListView.ListViewItemCollection BookList
		{
			get
			{
				return m_listviewBooks.Items;
			}
		}
		#endregion

		// Scaffolding -----------------------------------------------------------------------
		#region Controls
		private System.Windows.Forms.Label m_lblLanguageName;
        private System.Windows.Forms.TextBox m_editLanguageName;
		private System.Windows.Forms.Button m_btnImportBook;
		private System.Windows.Forms.Button m_btnRemove;
		private System.Windows.Forms.Button m_btnCreate;
		private System.Windows.Forms.Button m_btnProperties;
		private System.Windows.Forms.ListView m_listviewBooks;
		private System.Windows.Forms.ColumnHeader m_columnAbbrev;
        private System.Windows.Forms.ColumnHeader m_columnName;
        private Button m_btnRemoveTranslation;
        private TabControl m_tabctrlTranslation;
        private TabPage m_tabGeneral;
        private TabPage m_tabBooks;
        private TabPage m_tabBookNames;
        private PropertyGrid m_PropGridGeneral;
        private PropertyGrid m_PropGridBookNames;
        private Button m_btnCopyBookNames;
        private Label m_lblFrom;
        private ComboBox m_comboLanguage;
        private ColumnHeader m_columnFilename;
        private Button m_btnExport;
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
			m_Trans = trans;
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
            this.m_lblLanguageName = new System.Windows.Forms.Label();
            this.m_editLanguageName = new System.Windows.Forms.TextBox();
            this.m_btnImportBook = new System.Windows.Forms.Button();
            this.m_btnRemove = new System.Windows.Forms.Button();
            this.m_btnCreate = new System.Windows.Forms.Button();
            this.m_btnProperties = new System.Windows.Forms.Button();
            this.m_listviewBooks = new System.Windows.Forms.ListView();
            this.m_columnAbbrev = new System.Windows.Forms.ColumnHeader();
            this.m_columnName = new System.Windows.Forms.ColumnHeader();
            this.m_columnFilename = new System.Windows.Forms.ColumnHeader();
            this.m_btnExport = new System.Windows.Forms.Button();
            this.m_btnRemoveTranslation = new System.Windows.Forms.Button();
            this.m_tabctrlTranslation = new System.Windows.Forms.TabControl();
            this.m_tabGeneral = new System.Windows.Forms.TabPage();
            this.m_PropGridGeneral = new System.Windows.Forms.PropertyGrid();
            this.m_tabBooks = new System.Windows.Forms.TabPage();
            this.m_tabBookNames = new System.Windows.Forms.TabPage();
            this.m_comboLanguage = new System.Windows.Forms.ComboBox();
            this.m_lblFrom = new System.Windows.Forms.Label();
            this.m_btnCopyBookNames = new System.Windows.Forms.Button();
            this.m_PropGridBookNames = new System.Windows.Forms.PropertyGrid();
            this.m_tabctrlTranslation.SuspendLayout();
            this.m_tabGeneral.SuspendLayout();
            this.m_tabBooks.SuspendLayout();
            this.m_tabBookNames.SuspendLayout();
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
            this.m_editLanguageName.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_editLanguageName.Location = new System.Drawing.Point(112, 6);
            this.m_editLanguageName.Name = "m_editLanguageName";
            this.m_editLanguageName.Size = new System.Drawing.Size(322, 26);
            this.m_editLanguageName.TabIndex = 1;
            this.m_editLanguageName.Text = "Translation Name";
            this.m_editLanguageName.TextChanged += new System.EventHandler(this.cmdLangNameChanged);
            // 
            // m_btnImportBook
            // 
            this.m_btnImportBook.Location = new System.Drawing.Point(358, 35);
            this.m_btnImportBook.Name = "m_btnImportBook";
            this.m_btnImportBook.Size = new System.Drawing.Size(76, 23);
            this.m_btnImportBook.TabIndex = 7;
            this.m_btnImportBook.Text = "Import...";
            this.m_btnImportBook.Click += new System.EventHandler(this.cmdImportBook);
            // 
            // m_btnRemove
            // 
            this.m_btnRemove.Location = new System.Drawing.Point(358, 93);
            this.m_btnRemove.Name = "m_btnRemove";
            this.m_btnRemove.Size = new System.Drawing.Size(76, 23);
            this.m_btnRemove.TabIndex = 8;
            this.m_btnRemove.Text = "ctrlRemove...";
            this.m_btnRemove.Click += new System.EventHandler(this.cmdRemoveBook);
            // 
            // m_btnCreate
            // 
            this.m_btnCreate.Location = new System.Drawing.Point(358, 6);
            this.m_btnCreate.Name = "m_btnCreate";
            this.m_btnCreate.Size = new System.Drawing.Size(76, 23);
            this.m_btnCreate.TabIndex = 6;
            this.m_btnCreate.Text = "Create...";
            this.m_btnCreate.Click += new System.EventHandler(this.cmdCreateBook);
            // 
            // m_btnProperties
            // 
            this.m_btnProperties.Location = new System.Drawing.Point(358, 122);
            this.m_btnProperties.Name = "m_btnProperties";
            this.m_btnProperties.Size = new System.Drawing.Size(76, 23);
            this.m_btnProperties.TabIndex = 9;
            this.m_btnProperties.Text = "Properties...";
            this.m_btnProperties.Click += new System.EventHandler(this.cmdBookProperties);
            // 
            // m_listviewBooks
            // 
            this.m_listviewBooks.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.m_columnAbbrev,
            this.m_columnName,
            this.m_columnFilename});
            this.m_listviewBooks.FullRowSelect = true;
            this.m_listviewBooks.HideSelection = false;
            this.m_listviewBooks.LabelWrap = false;
            this.m_listviewBooks.Location = new System.Drawing.Point(6, 6);
            this.m_listviewBooks.MultiSelect = false;
            this.m_listviewBooks.Name = "m_listviewBooks";
            this.m_listviewBooks.ShowItemToolTips = true;
            this.m_listviewBooks.Size = new System.Drawing.Size(346, 307);
            this.m_listviewBooks.TabIndex = 5;
            this.m_listviewBooks.UseCompatibleStateImageBehavior = false;
            this.m_listviewBooks.View = System.Windows.Forms.View.Details;
            this.m_listviewBooks.DoubleClick += new System.EventHandler(this.cmdBookProperties);
            // 
            // m_columnAbbrev
            // 
            this.m_columnAbbrev.Text = "Abbreviation";
            this.m_columnAbbrev.Width = 70;
            // 
            // m_columnName
            // 
            this.m_columnName.Text = "Book Name";
            this.m_columnName.Width = 115;
            // 
            // m_columnFilename
            // 
            this.m_columnFilename.Text = "Filename";
            this.m_columnFilename.Width = 147;
            // 
            // m_btnExport
            // 
            this.m_btnExport.Location = new System.Drawing.Point(358, 64);
            this.m_btnExport.Name = "m_btnExport";
            this.m_btnExport.Size = new System.Drawing.Size(76, 23);
            this.m_btnExport.TabIndex = 10;
            this.m_btnExport.Text = "Export...";
            this.m_btnExport.Click += new System.EventHandler(this.cmdExportBook);
            // 
            // m_btnRemoveTranslation
            // 
            this.m_btnRemoveTranslation.Location = new System.Drawing.Point(6, 290);
            this.m_btnRemoveTranslation.Name = "m_btnRemoveTranslation";
            this.m_btnRemoveTranslation.Size = new System.Drawing.Size(159, 23);
            this.m_btnRemoveTranslation.TabIndex = 5;
            this.m_btnRemoveTranslation.Text = "ctrlRemove This Translation...";
            this.m_btnRemoveTranslation.UseVisualStyleBackColor = true;
            this.m_btnRemoveTranslation.Click += new System.EventHandler(this.cmdRemoveTranslation);
            // 
            // m_tabctrlTranslation
            // 
            this.m_tabctrlTranslation.Controls.Add(this.m_tabGeneral);
            this.m_tabctrlTranslation.Controls.Add(this.m_tabBooks);
            this.m_tabctrlTranslation.Controls.Add(this.m_tabBookNames);
            this.m_tabctrlTranslation.Location = new System.Drawing.Point(10, 10);
            this.m_tabctrlTranslation.Name = "m_tabctrlTranslation";
            this.m_tabctrlTranslation.SelectedIndex = 0;
            this.m_tabctrlTranslation.Size = new System.Drawing.Size(448, 345);
            this.m_tabctrlTranslation.TabIndex = 6;
            // 
            // m_tabGeneral
            // 
            this.m_tabGeneral.Controls.Add(this.m_PropGridGeneral);
            this.m_tabGeneral.Controls.Add(this.m_btnRemoveTranslation);
            this.m_tabGeneral.Controls.Add(this.m_lblLanguageName);
            this.m_tabGeneral.Controls.Add(this.m_editLanguageName);
            this.m_tabGeneral.Location = new System.Drawing.Point(4, 22);
            this.m_tabGeneral.Name = "m_tabGeneral";
            this.m_tabGeneral.Padding = new System.Windows.Forms.Padding(3);
            this.m_tabGeneral.Size = new System.Drawing.Size(440, 319);
            this.m_tabGeneral.TabIndex = 0;
            this.m_tabGeneral.Text = "General";
            this.m_tabGeneral.UseVisualStyleBackColor = true;
            // 
            // m_PropGridGeneral
            // 
            this.m_PropGridGeneral.Location = new System.Drawing.Point(9, 38);
            this.m_PropGridGeneral.Name = "m_PropGridGeneral";
            this.m_PropGridGeneral.Size = new System.Drawing.Size(425, 245);
            this.m_PropGridGeneral.TabIndex = 6;
            this.m_PropGridGeneral.ToolbarVisible = false;
            // 
            // m_tabBooks
            // 
            this.m_tabBooks.Controls.Add(this.m_btnExport);
            this.m_tabBooks.Controls.Add(this.m_listviewBooks);
            this.m_tabBooks.Controls.Add(this.m_btnProperties);
            this.m_tabBooks.Controls.Add(this.m_btnCreate);
            this.m_tabBooks.Controls.Add(this.m_btnRemove);
            this.m_tabBooks.Controls.Add(this.m_btnImportBook);
            this.m_tabBooks.Location = new System.Drawing.Point(4, 22);
            this.m_tabBooks.Name = "m_tabBooks";
            this.m_tabBooks.Padding = new System.Windows.Forms.Padding(3);
            this.m_tabBooks.Size = new System.Drawing.Size(440, 319);
            this.m_tabBooks.TabIndex = 1;
            this.m_tabBooks.Text = "Books";
            this.m_tabBooks.UseVisualStyleBackColor = true;
            // 
            // m_tabBookNames
            // 
            this.m_tabBookNames.Controls.Add(this.m_comboLanguage);
            this.m_tabBookNames.Controls.Add(this.m_lblFrom);
            this.m_tabBookNames.Controls.Add(this.m_btnCopyBookNames);
            this.m_tabBookNames.Controls.Add(this.m_PropGridBookNames);
            this.m_tabBookNames.Location = new System.Drawing.Point(4, 22);
            this.m_tabBookNames.Name = "m_tabBookNames";
            this.m_tabBookNames.Size = new System.Drawing.Size(440, 319);
            this.m_tabBookNames.TabIndex = 3;
            this.m_tabBookNames.Text = "Book Names";
            this.m_tabBookNames.UseVisualStyleBackColor = true;
            // 
            // m_comboLanguage
            // 
            this.m_comboLanguage.Location = new System.Drawing.Point(220, 295);
            this.m_comboLanguage.Name = "m_comboLanguage";
            this.m_comboLanguage.Size = new System.Drawing.Size(121, 21);
            this.m_comboLanguage.TabIndex = 107;
            // 
            // m_lblFrom
            // 
            this.m_lblFrom.Location = new System.Drawing.Point(166, 295);
            this.m_lblFrom.Name = "m_lblFrom";
            this.m_lblFrom.Size = new System.Drawing.Size(48, 23);
            this.m_lblFrom.TabIndex = 106;
            this.m_lblFrom.Text = "from";
            this.m_lblFrom.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // m_btnCopyBookNames
            // 
            this.m_btnCopyBookNames.Location = new System.Drawing.Point(48, 295);
            this.m_btnCopyBookNames.Name = "m_btnCopyBookNames";
            this.m_btnCopyBookNames.Size = new System.Drawing.Size(112, 23);
            this.m_btnCopyBookNames.TabIndex = 105;
            this.m_btnCopyBookNames.Text = "Copy Book Names";
            this.m_btnCopyBookNames.Click += new System.EventHandler(this.cmdCopyBookNames);
            // 
            // m_PropGridBookNames
            // 
            this.m_PropGridBookNames.Location = new System.Drawing.Point(14, 13);
            this.m_PropGridBookNames.Name = "m_PropGridBookNames";
            this.m_PropGridBookNames.PropertySort = System.Windows.Forms.PropertySort.NoSort;
            this.m_PropGridBookNames.Size = new System.Drawing.Size(414, 276);
            this.m_PropGridBookNames.TabIndex = 0;
            this.m_PropGridBookNames.ToolbarVisible = false;
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
            this.m_tabGeneral.ResumeLayout(false);
            this.m_tabGeneral.PerformLayout();
            this.m_tabBooks.ResumeLayout(false);
            this.m_tabBookNames.ResumeLayout(false);
            this.ResumeLayout(false);

		}
		#endregion

        // DlgPropertySheet overrides --------------------------------------------------------
        #region Method: void ShowHelp()
        public override void ShowHelp()
        {
            HelpSystem.ShowPage_Translation();
        }
        #endregion
        #region Attr{g}: string TabText
        public override string TabText
        {
            get
            {
                string sLeader = "";
                if (Trans == G.FTranslation)
                    sLeader = "Front: ";
                else if (Trans == G.TTranslation)
                    sLeader = "Target: ";

                return sLeader + Trans.DisplayName;
            }
        }
        #endregion
        #region Method: override bool HarvestChanges()
        public override bool HarvestChanges()
        {
			// The translation's name should be a valid, non-zero-lengthed name
			if (LanguageName.Length == 0)
			{
                m_tabctrlTranslation.SelectedTab = m_tabGeneral;
                Messages.TranslationNeedsName();
                m_editLanguageName.Focus();
				m_editLanguageName.Select();
				return false;
			}
			Trans.DisplayName = LanguageName;

			// The translation's abbreviation must be non-zero length
			if (m_sAbbreviation.Length == 0)
			{
                m_tabctrlTranslation.SelectedTab = m_tabGeneral;
                Messages.TranslationNeedsAbbrev();
                m_PropGridGeneral.Focus();
                m_PropGridGeneral.Select();
				return false;
			}
            Trans.LanguageAbbrev = m_sAbbreviation;

            return true;
        }
        #endregion

        // Property Grid: General ------------------------------------------------------------
        const string c_sPropAbbreviation = "propAbbreviation";
        const string c_sPropWSAdvisor = "propWSAdvisor";
        const string c_sPropWSVernacular = "propWSVernacular";
        string m_sAbbreviation;
        #region Attr{g}: PropertyBag BagGeneral
        PropertyBag BagGeneral
        {
            get
            {
                Debug.Assert(null != m_BagGeneral);
                return m_BagGeneral;
            }
        }
        PropertyBag m_BagGeneral;
        #endregion
        #region Method: void bagGeneral_GetValue(...)
        void bagGeneral_GetValue(object sender, PropertySpecEventArgs e)
        {
            switch (e.Property.ID)
            {
                case c_sPropAbbreviation:
                    e.Value = m_sAbbreviation;
                    break;

                case c_sPropWSAdvisor:
                    e.Value = Trans.ConsultantWritingSystemName;
                    break;

                case c_sPropWSVernacular:
                    e.Value = Trans.VernacularWritingSystemName;
                    break;
            }
        }
        #endregion
        #region Method: void bagGeneral_SetValue(...)
        void bagGeneral_SetValue(object sender, PropertySpecEventArgs e)
        {
            switch (e.Property.ID)
            {
                case c_sPropAbbreviation:
                    m_sAbbreviation = (string)e.Value;
                    break;

                case c_sPropWSAdvisor:
                    Trans.ConsultantWritingSystemName = (string)e.Value;
                    break;

                case c_sPropWSVernacular:
                    Trans.VernacularWritingSystemName = (string)e.Value;
                    break;
            }
        }
        #endregion
        #region Method: void SetupPropGrid_General()
        void SetupPropGrid_General()
        {
            // Create the PropertyBag for this style
            m_BagGeneral = new PropertyBag();
            BagGeneral.GetValue += new PropertySpecEventHandler(bagGeneral_GetValue);
            BagGeneral.SetValue += new PropertySpecEventHandler(bagGeneral_SetValue);

            string[] vsWritingSystems = new string[G.StyleSheet.WritingSystems.Count];
            for (int i = 0; i < G.StyleSheet.WritingSystems.Count; i++)
                vsWritingSystems[i] = (G.StyleSheet.WritingSystems[i] as JWritingSystem).Name;

            // Abbreviation
            BagGeneral.Properties.Add( new PropertySpec(
                c_sPropAbbreviation,
                "Language Abbreviation",
                typeof(string),
                "",
                "Typically the Ethnoloque Code, a short (e.g., 3-letter) abbreviation of the " +
                    "language name. One of its uses is in composing the filenames.",
                "",
                "", 
                null
                ));

            // Vernacular Writing System
            PropertySpec ps = new PropertySpec(
                c_sPropWSVernacular,
                "Writing System for Vernacular Text",
                "Writing Systems",
                "The Writing System for the translation, you define these under " +
                    "Team Settings.",
                vsWritingSystems,
                ""
                );
            ps.DontLocalizeEnums = true;
            BagGeneral.Properties.Add(ps);

            // Advisor Writing System
            ps = new PropertySpec(
                c_sPropWSAdvisor,
                "Writing System for Advisor Text",
                "Writing Systems",
                "The Writing System for advisor or consultant text (e.g., the back translation), " +
                    "you define these under Team Settings.",
                vsWritingSystems,
                ""
                );
            ps.DontLocalizeEnums = true;
            BagGeneral.Properties.Add(ps);

            // Localize the bag
            LocDB.Localize(this, BagGeneral);

            // Set the Property Grid to this PropertyBag
            m_PropGridGeneral.SelectedObject = BagGeneral;
        }
        #endregion

        // Property Grid: BookNames ----------------------------------------------------------
        #region Attr{g}: PropertyBag BagBookNames
        PropertyBag BagBookNames
        {
            get
            {
                Debug.Assert(null != m_BagBookNames);
                return m_BagBookNames;
            }
        }
        PropertyBag m_BagBookNames;
        #endregion
        #region Method: void bagBookNames_GetValue(...)
        void bagBookNames_GetValue(object sender, PropertySpecEventArgs e)
        {
            // The PropertyID is the book's abbrev
            string sAbbrev = e.Property.ID;

            // Get the book's index
            int iBook = DBook.FindBookAbbrevIndex(sAbbrev);

            // Look up the bookname in the tabe
            e.Value = Trans.BookNamesTable[iBook];
        }
        #endregion
        #region Method: void bagBookNames_SetValue(...)
        void bagBookNames_SetValue(object sender, PropertySpecEventArgs e)
        {
            // The PropertyID is the book's abbrev
            string sAbbrev = e.Property.ID;

            // Get the book's index
            int iBook = DBook.FindBookAbbrevIndex(sAbbrev);

            // Set the value to the table
           Trans.BookNamesTable[iBook] = (string)e.Value; ;
        }
        #endregion
        #region Method: void SetupPropGrid_BookNames()
        void SetupPropGrid_BookNames()
        {
            // Create the PropertyBag for this style
            m_BagBookNames = new PropertyBag();
            BagBookNames.GetValue += new PropertySpecEventHandler(bagBookNames_GetValue);
            BagBookNames.SetValue += new PropertySpecEventHandler(bagBookNames_SetValue);

            // One line per book
            for (int i = 0; i < DBook.BookAbbrevsCount; i++)
            {
                PropertySpec ps = new PropertySpec(
                    DBook.BookAbbrevs[i],
                    BookNames.GetName(i),
                    typeof(string),
                    null,
                    "Enter the name of the book in this language for " + BookNames.GetName(i),
                    "",
                    "",
                    null);
                ps.DontLocalizeName = true;
                ps.DontLocalizeCategory = true;
                ps.DontLocalizeHelp = true;
                BagBookNames.Properties.Add(ps);
            }

            // Localize the bag
            LocDB.Localize(this, BagBookNames);

            // Set the Property Grid to this PropertyBag
            m_PropGridBookNames.SelectedObject = BagBookNames;

        }
        #endregion

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
            DTeamSettings ts = G.TeamSettings;
            string sFileNameLang = ts.FileNameLanguage;
            if (!ComboBoxHasPossiblity(sFileNameLang))
            {
                ComboLanguage.Items.Add(sFileNameLang);
                ComboLanguage.Text = sFileNameLang;
            }

            // Put in the Front Translation (if this isn't the Front)
            if (null != G.Project.FrontTranslation && G.Project.FrontTranslation != Trans)
            {
                string sFrontLang = G.Project.FrontTranslation.DisplayName;
                if (!ComboBoxHasPossiblity(sFrontLang))
                {
                    ComboLanguage.Items.Add(sFrontLang);
                    ComboLanguage.Text = sFrontLang;
                }
            }

            // Put in the Target Translation (if this isn't the Target) but from here
            // on out, we don't select it in the combo box (thus leaving a Resources
            // language as the default.
            if (null != G.Project.TargetTranslation && G.Project.TargetTranslation != Trans)
            {
                string sTargetLang = G.Project.TargetTranslation.DisplayName;
                if (!ComboBoxHasPossiblity(sTargetLang))
                    ComboLanguage.Items.Add(sTargetLang);
            }

            // Put in any other translations
            foreach (DTranslation t in G.Project.OtherTranslations)
            {
                if (t != Trans && !ComboBoxHasPossiblity(t.DisplayName))
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
            if (null != G.Project.FrontTranslation && G.Project.FrontTranslation.DisplayName == sLanguage)
                vsBookNamesSource = G.Project.FrontTranslation.BookNamesTable.GetCopy();
            if (null != G.Project.TargetTranslation && G.Project.TargetTranslation.DisplayName == sLanguage)
                vsBookNamesSource = G.Project.TargetTranslation.BookNamesTable.GetCopy();
            foreach (DTranslation t in G.Project.OtherTranslations)
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
            Trans.BookNamesTable.ReplaceAll(vsBookNamesSource);

            // Update the cross references in the loaded book
            if (null != G.Project.FrontTranslation &&
                G.Project.FrontTranslation.DisplayName == sLanguage &&
                Trans == G.Project.TargetTranslation)
            {
                G.Project.TargetTranslation.UpdateFromFront();
            }

            // Recalculate the grid
            SetupPropGrid_BookNames();
            Invalidate();
        }
        #endregion

        // Command Handlers ------------------------------------------------------------------
        #region Handler: cmdLoad - Populate the controls
        private void cmdLoad(object sender, System.EventArgs e)
		{
            // Localization
            Control[] vExclude = { m_PropGridGeneral, m_PropGridBookNames };
            LocDB.Localize(this, vExclude);

			// Translation Abbrev. (Load this before the Name, because if it is empty,
			// the process of loading the name will supply a meaningful default.)
            m_sAbbreviation = Trans.LanguageAbbrev;

			// Language Name
			LanguageName = Trans.DisplayName;

            // Initialze the PropertyGrids
            SetupPropGrid_General();
            SetupPropGrid_BookNames();

			// Populate the list of books; select the first item in the list
			_PopulateBookList("GEN");

			// Hide the CreateBook button if requested (Front books cannot be created here)
			if (SuppressCreateBook)
				m_btnCreate.Visible = false;

            // Combo Box Possibilities
            PopulateComboBoxPossibilities();
		}
		#endregion
		#region Handler: cmdLangNameChanged - Update Abbrev when LanguageName changes
		private void cmdLangNameChanged(object sender, System.EventArgs e)
		{
            /*** 
			// Don't erase an existing Abbrev should the user delete the Lang Name.
			if (LanguageName.Length == 0)
				return;				

			// Get the default abbreviation as the first three letters of the name
			int cAbbrevLength = 3;
			string sDefaultAbbrev = "";
			if (LanguageName.Length > 0)
			{
				foreach(char ch in LanguageName)
				{
					if (ch != ' ')
					{
						sDefaultAbbrev += ch;
						cAbbrevLength--;
					}
					if (0 == cAbbrevLength)
						break;
				}
			}

			// If the abbreviation currently entered matches this default (for as
			// many letters as it has), then add any remaining letters
			int i=0;
			if (Abbrev == "MyT" || Abbrev == "Fro")  // The default from "My Translation"
				Abbrev = "";
			for(; i < Abbrev.Length && i < sDefaultAbbrev.Length; i++)
			{
				if ( Abbrev[i] != sDefaultAbbrev[i] )
					break;
			}
			if (i == Abbrev.Length && i < sDefaultAbbrev.Length)
				Abbrev = sDefaultAbbrev;

            // Update the tab page's text and title bar if appropriate
            ParentDlg.UpdateActiveTabText();
            ParentDlg.SetTitleBarText();
            ***/
		}
		#endregion
        #region Handler: cmdRemoveTranslation
        private void cmdRemoveTranslation(object sender, EventArgs e)
        {
            // Query the user to make certain
            if (!Messages.VerifyRemoveTranslation())
                return;

            // Save the OTrans file in case anything has changed
            ParentDlg.HarvestChangesFromCurrentSheet();
            Trans.Write();

            // ctrlRemove it from the appropriate object in the Properties
            bool bWasFront = false;
            bool bWasOther = false;
            if (G.Project.FrontTranslation == Trans)
            {
                G.Project.FrontTranslation = null;
                bWasFront = true;
            }
            else if (G.Project.TargetTranslation == Trans)
            {
                G.Project.TargetTranslation = null;
            }
            else
            {
                G.Project.OtherTranslations.Remove(Trans);
                bWasOther = true;
            }

            // Regenerate the dialog 
            if (bWasOther)
                ParentDlg.SetupTabControl(DialogProperties.c_navTranslations);
            else
            {
                ParentDlg.SetupTabControl(DialogProperties.c_navEssentials);
                ParentDlg.ActivatePage( bWasFront ?
                    DialogProperties.c_tagEssentialsFront : DialogProperties.c_tagEssentialsTarget);
            }
        }
        #endregion

        // Books Page ------------------------------------------------------------------------
		#region Handler: cmdRemoveBook - ctrlRemove Book button clicked
		private void cmdRemoveBook(object sender, System.EventArgs e)
		{
			// Get the selection
			DBook book = CurrentlySelectedBook;
			if (null == book)
				return;

			// Make sure the user wants to remove the book.
			if (!Messages.VerifyRemoveBook())  
				return;

			// ctrlRemove the book from the translation & refresh the listview
			Trans.Books.Remove(book);
			_PopulateBookList("");
		}
		#endregion
		#region Handler: cmdBookProperties - either via Button or Double-Click
		private void cmdBookProperties(object sender, System.EventArgs e)
		{
			// Get the current selection
			DBook book = CurrentlySelectedBook;
			if (null == book)
				return;

			// Launch the dialog
			Debug.Assert(null != Trans);
			Debug.Assert(null != Trans.Project);
			book.EditProperties(Trans.Project.FrontTranslation, 
				Trans, DBookProperties.Mode.kProperties);

			// Update the list
			_PopulateBookList(book.BookAbbrev);
		}
		#endregion
		#region Handler: cmdImportBook
		private void cmdImportBook(object sender, System.EventArgs e)
		{
            // Show the wizard; the user will input the needed information and
            // indicate whether or not to proceed.
            Dialogs.WizImportBook.WizImportBook wizard =
                new Dialogs.WizImportBook.WizImportBook(Trans);
            if (DialogResult.OK != wizard.ShowDialog())
                return;

            // Create a book object
            DBook book = new DBook(wizard.BookAbbrev, wizard.ImportFileName);

            // Add it to the translation (we must do this or book.LoadData cannot
            // properly check for errors.)
            Trans.AddBook(book);

            // Attempt to read it in
            Debug.Assert(!book.Loaded);
            book.Load();
            if (!book.Loaded)
            {
                Trans.Books.Remove(book);
                return;
            }

            // If successful, we now need to write it out to the desired path; this
            // will put it into our file format as well.
            string sFileName = Path.GetFileName(book.AbsolutePathName);
            book.AbsolutePathName = wizard.DestinationFolder + Path.DirectorySeparatorChar + sFileName;
            book.TranslationStage = G.TranslationStages.GetFromAbbrev(wizard.Stage);
            book.Version = wizard.Version;
            book.DisplayName = book.BookName;
            book.DeclareDirty();  // Make certain this will be written to file
            book.Unload();    // Writes the file

            // Update the property page
            _PopulateBookList(book.BookAbbrev);
		}
		#endregion
		#region Handler: cmdCreateBook
		private void cmdCreateBook(object sender, System.EventArgs e)
		{
			// Get an available book (one that hasn't already been created)
			string sAbbrev = Trans.NextAvailableBookAbbrev;
			if (sAbbrev.Length == 0)
				return;

			// Present the dialog so the user can decide what to do. We're done
			// if the user cancels.
			DBook book = new DBook(sAbbrev, "");
			if (DialogResult.OK != book.EditProperties(
				Trans.Project.FrontTranslation, Trans, 
				DBookProperties.Mode.kCreate))
			{
				return;
			}

			// Create the book from the Front translation's template
			Trans.AddBook(book);
			if (false == book.InitializeFromFrontTranslation())
			{
				Trans.Books.Remove(book);
				return;
			}
			_PopulateBookList(sAbbrev);
            book.Write();
		}
		#endregion
        #region Handler: cmdExportBook
        private void cmdExportBook(object sender, EventArgs e)
        {
            // Get the selection
            DBook book = CurrentlySelectedBook;
            if (null == book)
                return;

            // Get the user's desired destination (or cancel)
            DialogExportBook dlg = new DialogExportBook(book);
            if (DialogResult.OK != dlg.ShowDialog(ParentDlg))
                return;

            // Export the book
            book.Export(dlg.ExportPathName);
        }
        #endregion

        // Methods ---------------------------------------------------------------------------
		#region Method: void _PopulateBookList() - puts the Books into the ListView control
		private void _PopulateBookList(string sSelectedAbbrev)
		{
			int i = 0;
			int iSelect = -1;

			BookList.Clear();
			foreach (DBook book in Trans.Books)
			{
				if (book.BookAbbrev == sSelectedAbbrev)
					iSelect = i;
				++i;


                // First column is the book's abbreviation
				ListViewItem item = new ListViewItem( book.BookAbbrev );

				// We show an item as being "Locked" by giving it a red color in the
				// list view.
				if (book.Locked)
					item.ForeColor = Color.Red;

                // 2nd Column is the book's display name
				item.SubItems.Add( book.DisplayName );

                // 3rd Column is the book's file name
                string sFilename = book.RelativePathName;
                if (string.IsNullOrEmpty(sFilename))
                    sFilename = book.AbsolutePathName;
                if (!string.IsNullOrEmpty(sFilename))
                    sFilename = Path.GetFileName(sFilename);
                item.SubItems.Add(sFilename);
               

                // Tooltip
//                item.ToolTipText = "Howdy!";
//                string sToolTip = book.BookAbbrev + " - " + book.DisplayName + "\n" +
//                    book.AbsolutePathName;
//                item.ToolTipText = sToolTip;

                // Add the row to the list
				BookList.Add(item);
			}

			if (BookList.Count > 0 && iSelect == -1)
				iSelect = 0;
            if (iSelect != -1)
            {
                BookList[iSelect].Selected = true;
                BookList[iSelect].Focused = true;
            }

			// We need to return focus back to the list control (and away from wherever it was).
			// This also results in the control being re-drawn. E.g., if the Locked setting
			// was changed via the Properties dialog, the item will not be drawn in red without
			// our first doing this.
			m_listviewBooks.Focus();
		}
		#endregion
    }


    #region CLASS BookNames - E.g., Genesis, Exodus, Leviticus, etc.
    class BookNames
    {
        // Attrs -----------------------------------------------------------------------------
        #region SAttr{g}: LocGroup LocGroup - the localization group containing the book names (LocItems)
        const string c_LocGroupID = "BookNames";
        static LocGroup LocGroup
        {
            get
            {
                if (null == s_LocGroup)
                    s_LocGroup = LocDB.DB.FindGroup(c_LocGroupID);
                return s_LocGroup;
            }
        }
        static LocGroup s_LocGroup = null;
        #endregion

        // Retrieve a single name, according to the current language preferences
        #region Method: string GetName(int index)
        static public string GetName(int index)
        {
            Debug.Assert(index >= 0 && index < 66);

            // The LocItem's lookup ID is the English form of the book
            string sLocItemID = English[index];

            // If for some reason the Group was not found, then return the English value
            if (null == LocGroup)
                return English[index];

            // Find the LocItem containing the localizations; return English if not found
            LocItem item = LocGroup.Find(sLocItemID);
            if (null == item)
                return English[index];

            // The LocDB will either return the string in the requested language, or
            // English if not found.
            return item.AltValue;
        }
        #endregion

        // Retrieve a table of Book Names ----------------------------------------------------
        const int c_cTableSize = 66;      // Number of books in the Bible
        #region Method: string[] GetTable(LanguageResources.Languages lang)
        static public string[] GetTable(LanguageResources.Languages lang)
        {
            // Retrieve the name of the language
            string sLanguageName = LanguageResources.GetLanguageName(lang);

            // The GetTable(sLanguageName) method will do the rest of the work
            return GetTable(sLanguageName);
        }
        #endregion
        #region Method: string[] GetTable(string sLanguageName)
        static public string[] GetTable(string sLanguageName)
        // Return a vector of strings for the 66 books, corresponding to the
        // requested language name.
        {
            // We'll build the table here
            string[] vs = new string[c_cTableSize];

            // Get the index of the alternative we'll want
            LocLanguage lang = LocDB.DB.FindLanguageByName(sLanguageName);
            if (null == lang)
                return English;
            int iLanguage = lang.Index;

            // Make sure we found the LocGroup
            if (null == LocGroup)
                return English;

            // Fill up the table
            for (int i = 0; i < c_cTableSize; i++)
            {
                LocItem item = LocGroup.Find(English[i]);
                if (null == item)
                    vs[i] = English[i];
                else
                    vs[i] = item.Alternates[iLanguage].Value;
            }

            return vs;
        }
        #endregion

        // Localized list of booknames -------------------------------------------------------
        #region Attr{g} string[] English - if we can't find a language, we always have English here
        static public string[] English = 
	{ 
		"Genesis", "Exodus", "Leviticus", "Numbers", "Deuteronomy", "Joshua",
		"Judges", "Ruth", "1 Samuel", "2 Samuel", "1 Kings", "2 Kings", 
		"1 Chronicles", "2 Chronicles", "Ezra", "Nehemiah", "Esther", "Job", 
		"Psalms", "Proverbs", "Ecclesiastes", "Song of Songs", "Isaiah", 
		"Jeremiah", "Lamentations", "Ezekiel", "Daniel", "Hosea", "Joel", 
		"Amos", "Obadiah", "Jonah", "Micah", "Nahum", "Habakkuk", "Zephaniah",
		"Haggai", "Zechariah", "Malachi", "Matthew", "Mark", "Luke", "John", 
		"Acts", "Romans", "1 Corinthians", "2 Corinthians", "Galatians", 
		"Ephesians", "Philippians", "Colossians", "1 Thessalonians", 
		"2 Thessalonians", "1 Timothy", "2 Timothy", "Titus", "Philemon", 
		"Hebrews", "James", "1 Peter", "2 Peter", "1 John", "2 John", "3 John", 
		"Jude", "Revelation" 
	};
        #endregion
    }
    #endregion

    #region NUnit Test_BookNames
    [TestFixture]
    public class Test_BookNames
    {
        #region Method: void Setup()
        [SetUp]
        public void Setup()
        {
            JWU.NUnit_Setup();
        }
        #endregion

        #region Test: BookNameRetrieval
        [Test]
        public void BookNameRetrieval()
        {
            string[] vSpanish = BookNames.GetTable("Español");

            Assert.AreEqual("Éxodo", vSpanish[1]);
        }
        #endregion
    }
    #endregion
}
