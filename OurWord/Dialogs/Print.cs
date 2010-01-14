/**********************************************************************************************
 * Project: Our Word!
 * File:    Print.cs
 * Author:  John Wimbish
 * Created: 20 Feb 2006
 * Purpose: Printing (direct to printer)
 * Legal:   Copyright (c) 2004-09, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
#region Using
using System;
using System.Collections;
using System.Collections.Generic;
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
using OurWord.Printing;
using OurWordData;
using OurWordData.DataModel;
#endregion

namespace OurWord
{
    #region CLASS: DialogPrint - the dialog for the user entering his print desires
    public class DialogPrint : System.Windows.Forms.Form
	{
        // Strings ---------------------------------------------------------------------------
        #region VAttr{g}: string SingleSpace
        string SingleSpace
        {
            get
            {
                return LocDB.GetValue(this, "strSingle", "Single");
            }
        }
        #endregion
        #region VAttr{g}: string MediumSpace - space at 1.5 lines
        string MediumSpace
        {
            get
            {
                return LocDB.GetValue(this, "strMedium", "1.5");
            }
        }
        #endregion
        #region VAttr{g}: string DoubleSpace
        string DoubleSpace
        {
            get
            {
                return LocDB.GetValue(this, "strDouble", "Double");
            }
        }
        #endregion

		// Attrs -----------------------------------------------------------------------------
		#region Attr{g}: PrintDocument PDoc
		public PrintDocument PDoc
		{
			get
			{
				Debug.Assert(null != m_pdoc);
				return m_pdoc;
			}
		}
		PrintDocument m_pdoc;
		#endregion
		#region Attr{g/s}: bool EntireBook
		public bool EntireBook
		{
			get
			{
				return (m_radioEntireBook.Checked);
			}
			set
			{
				m_radioEntireBook.Checked = value;

				m_radioThisSection.Checked = !value;
				m_radioChapters.Checked = !value;
			}
		}
		#endregion
		#region Attr{g/s}: bool CurrentSection
		public bool CurrentSection
		{
			get
			{
				return (m_radioThisSection.Checked == true);
			}
			set
			{
				m_radioThisSection.Checked = value;

				m_radioEntireBook.Checked = !value;
				m_radioChapters.Checked = !value;
			}
		}
		#endregion
		#region Attr{g/s}: bool Chapters
		public bool Chapters
		{
			get
			{
				return (m_radioChapters.Checked == true);
			}
			set
			{
				m_radioChapters.Checked = value;

				m_radioEntireBook.Checked = !value;
				m_radioThisSection.Checked = !value;
			}
		}
		#endregion

        #region Attr{g}: bool Vernacular
        public bool Vernacular
	    {
	        get
	        {
	            return m_radioVernacular.Checked;
	        }
            private set
            {
                m_radioVernacular.Checked = value;
                m_radioBackTranslation.Checked = !value;
            }
        }
        #endregion
        #region Attr{g}: bool BackTranslation
        public bool BackTranslation
        {
            get
            {
                return m_radioBackTranslation.Checked;
            }
            private set
            {
                m_radioBackTranslation.Checked = value;
                m_radioVernacular.Checked = !value;
            }
        }
        #endregion

        #region Attr{g}: string PrinterName
        public string PrinterName
		{
			get
			{
				return m_comboPrinter.Text;
			}
		}
		#endregion
		#region Attr{g/s}: int StartChapter
		public int StartChapter
		{
			get
			{
				string s = m_editStartChapter.Text;

				try
				{
					int n = Convert.ToInt16(s);
					return n;
				}
				catch (Exception)
				{
				}
				return 0;
			}
			set
			{
				m_editStartChapter.Text = value.ToString();
			}
		}
		#endregion
		#region Attr{g/s}: int EndChapter
		public int EndChapter
		{
			get
			{
				string s = m_editEndChapter.Text;

				try
				{
					int n = Convert.ToInt16(s);
					return n;
				}
				catch (Exception)
				{
				}
				return 0;
			}
			set
			{
				m_editEndChapter.Text = value.ToString();
			}
		}
		#endregion

		#region Attr{g}: bool PrintWaterMark
		public bool PrintWaterMark
		{
			get
			{
				return m_checkWaterMark.Checked;
			}
		}
		#endregion
        #region Attr{g}: string WaterMarkText
        public string WaterMarkText
        {
            get
            {
                return m_textWatermark.Text;
            }
        }
        #endregion

        #region Attr{g}: float LineSpacing
        public float LineSpacing
		{
			get
			{
                if (m_comboLineSpacing.Text == MediumSpace)
					return 1.5F;
                if (m_comboLineSpacing.Text == DoubleSpace)
					return 2.0F;
				return 1.0F;
			}
		}
		#endregion
		#region Attr{g/s}: bool MakeQuoteSubstitutions
		public bool MakeQuoteSubstitutions
		{
			get
			{
				return m_checkReplacements.Checked;
			}
			set
			{
				m_checkReplacements.Checked = value;
			}
		}
		#endregion
		#region Attr{g/s}: bool PrintPictures
		public bool PrintPictures
		{
			get
			{
				return m_checkPrintPictures.Checked;
			}
			set
			{
				m_checkPrintPictures.Checked = value;
			}
		}
		#endregion

		// Scaffolding -----------------------------------------------------------------------
		#region Constructor(PrintDocument)
		public DialogPrint(PrintDocument pdoc)
		{
			InitializeComponent();

			m_pdoc = pdoc;

            if (DB.IsValidProject)
		        m_textWatermark.Text = DB.TargetBook.Stage.LocalizedName;
		}
		#endregion
		#region DIALOG CONTROLS
		private Button m_btnHelp;
		private Button m_btnCancel;
		private Button m_btnOK;

		private Label m_lblPrinter;
		private ComboBox m_comboPrinter;

		private GroupBox m_groupRange;
		private RadioButton m_radioEntireBook;
        private RadioButton m_radioThisSection;
        private RadioButton m_radioChapters;
		private TextBox m_editStartChapter;
		private Label   m_labelToChapter;
		private TextBox m_editEndChapter;

		private CheckBox m_checkWaterMark;
		private Label m_labelLineSpacing;
		private ComboBox m_comboLineSpacing;
		private GroupBox m_groupOptions;
		private CheckBox m_checkReplacements;
		private System.Windows.Forms.CheckBox m_checkPrintPictures;
        private RadioButton m_radioVernacular;
        private RadioButton m_radioBackTranslation;
        private GroupBox m_groupWhat;
        private TextBox m_textWatermark;

		private System.ComponentModel.Container components = null;
		#endregion
		#region Method: void Dispose( bool disposing )
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
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DialogPrint));
            this.m_btnHelp = new System.Windows.Forms.Button();
            this.m_btnCancel = new System.Windows.Forms.Button();
            this.m_btnOK = new System.Windows.Forms.Button();
            this.m_lblPrinter = new System.Windows.Forms.Label();
            this.m_comboPrinter = new System.Windows.Forms.ComboBox();
            this.m_radioEntireBook = new System.Windows.Forms.RadioButton();
            this.m_radioThisSection = new System.Windows.Forms.RadioButton();
            this.m_groupRange = new System.Windows.Forms.GroupBox();
            this.m_editEndChapter = new System.Windows.Forms.TextBox();
            this.m_labelToChapter = new System.Windows.Forms.Label();
            this.m_radioChapters = new System.Windows.Forms.RadioButton();
            this.m_editStartChapter = new System.Windows.Forms.TextBox();
            this.m_checkWaterMark = new System.Windows.Forms.CheckBox();
            this.m_labelLineSpacing = new System.Windows.Forms.Label();
            this.m_comboLineSpacing = new System.Windows.Forms.ComboBox();
            this.m_groupOptions = new System.Windows.Forms.GroupBox();
            this.m_textWatermark = new System.Windows.Forms.TextBox();
            this.m_checkPrintPictures = new System.Windows.Forms.CheckBox();
            this.m_checkReplacements = new System.Windows.Forms.CheckBox();
            this.m_radioVernacular = new System.Windows.Forms.RadioButton();
            this.m_radioBackTranslation = new System.Windows.Forms.RadioButton();
            this.m_groupWhat = new System.Windows.Forms.GroupBox();
            this.m_groupRange.SuspendLayout();
            this.m_groupOptions.SuspendLayout();
            this.SuspendLayout();
            // 
            // m_btnHelp
            // 
            this.m_btnHelp.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.m_btnHelp.Image = ((System.Drawing.Image)(resources.GetObject("m_btnHelp.Image")));
            this.m_btnHelp.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.m_btnHelp.Location = new System.Drawing.Point(255, 382);
            this.m_btnHelp.Name = "m_btnHelp";
            this.m_btnHelp.Size = new System.Drawing.Size(75, 23);
            this.m_btnHelp.TabIndex = 15;
            this.m_btnHelp.Text = "Help...";
            this.m_btnHelp.Click += new System.EventHandler(this.cmdHelp);
            // 
            // m_btnCancel
            // 
            this.m_btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.m_btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.m_btnCancel.Location = new System.Drawing.Point(167, 382);
            this.m_btnCancel.Name = "m_btnCancel";
            this.m_btnCancel.Size = new System.Drawing.Size(75, 23);
            this.m_btnCancel.TabIndex = 14;
            this.m_btnCancel.Text = "Cancel";
            // 
            // m_btnOK
            // 
            this.m_btnOK.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.m_btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.m_btnOK.Location = new System.Drawing.Point(79, 382);
            this.m_btnOK.Name = "m_btnOK";
            this.m_btnOK.Size = new System.Drawing.Size(75, 23);
            this.m_btnOK.TabIndex = 13;
            this.m_btnOK.Text = "Print";
            // 
            // m_lblPrinter
            // 
            this.m_lblPrinter.Location = new System.Drawing.Point(16, 16);
            this.m_lblPrinter.Name = "m_lblPrinter";
            this.m_lblPrinter.Size = new System.Drawing.Size(80, 23);
            this.m_lblPrinter.TabIndex = 15;
            this.m_lblPrinter.Text = "Printer:";
            this.m_lblPrinter.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // m_comboPrinter
            // 
            this.m_comboPrinter.Location = new System.Drawing.Point(104, 16);
            this.m_comboPrinter.Name = "m_comboPrinter";
            this.m_comboPrinter.Size = new System.Drawing.Size(288, 21);
            this.m_comboPrinter.TabIndex = 1;
            // 
            // m_radioEntireBook
            // 
            this.m_radioEntireBook.Location = new System.Drawing.Point(32, 69);
            this.m_radioEntireBook.Name = "m_radioEntireBook";
            this.m_radioEntireBook.Size = new System.Drawing.Size(352, 24);
            this.m_radioEntireBook.TabIndex = 2;
            this.m_radioEntireBook.TabStop = true;
            this.m_radioEntireBook.Text = "Entire Book";
            this.m_radioEntireBook.Click += new System.EventHandler(this.cmdEntireBookChecked);
            // 
            // m_radioThisSection
            // 
            this.m_radioThisSection.Location = new System.Drawing.Point(32, 91);
            this.m_radioThisSection.Name = "m_radioThisSection";
            this.m_radioThisSection.Size = new System.Drawing.Size(352, 24);
            this.m_radioThisSection.TabIndex = 3;
            this.m_radioThisSection.TabStop = true;
            this.m_radioThisSection.Text = "Current Section";
            this.m_radioThisSection.Click += new System.EventHandler(this.cmdCurrentSectionChecked);
            // 
            // m_groupRange
            // 
            this.m_groupRange.Controls.Add(this.m_editEndChapter);
            this.m_groupRange.Controls.Add(this.m_labelToChapter);
            this.m_groupRange.Controls.Add(this.m_radioChapters);
            this.m_groupRange.Controls.Add(this.m_editStartChapter);
            this.m_groupRange.Location = new System.Drawing.Point(16, 56);
            this.m_groupRange.Name = "m_groupRange";
            this.m_groupRange.Size = new System.Drawing.Size(376, 90);
            this.m_groupRange.TabIndex = 21;
            this.m_groupRange.TabStop = false;
            this.m_groupRange.Text = "Range";
            // 
            // m_editEndChapter
            // 
            this.m_editEndChapter.Location = new System.Drawing.Point(161, 60);
            this.m_editEndChapter.Name = "m_editEndChapter";
            this.m_editEndChapter.Size = new System.Drawing.Size(40, 20);
            this.m_editEndChapter.TabIndex = 6;
            this.m_editEndChapter.TextChanged += new System.EventHandler(this.cmdToChapterChanged);
            // 
            // m_labelToChapter
            // 
            this.m_labelToChapter.Location = new System.Drawing.Point(129, 59);
            this.m_labelToChapter.Name = "m_labelToChapter";
            this.m_labelToChapter.Size = new System.Drawing.Size(32, 23);
            this.m_labelToChapter.TabIndex = 28;
            this.m_labelToChapter.Text = "to";
            this.m_labelToChapter.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // m_radioChapters
            // 
            this.m_radioChapters.Location = new System.Drawing.Point(16, 59);
            this.m_radioChapters.Name = "m_radioChapters";
            this.m_radioChapters.Size = new System.Drawing.Size(72, 24);
            this.m_radioChapters.TabIndex = 4;
            this.m_radioChapters.TabStop = true;
            this.m_radioChapters.Text = "Chapters:";
            this.m_radioChapters.Click += new System.EventHandler(this.cmdChaptersChecked);
            // 
            // m_editStartChapter
            // 
            this.m_editStartChapter.Location = new System.Drawing.Point(89, 60);
            this.m_editStartChapter.Name = "m_editStartChapter";
            this.m_editStartChapter.Size = new System.Drawing.Size(40, 20);
            this.m_editStartChapter.TabIndex = 5;
            this.m_editStartChapter.TextChanged += new System.EventHandler(this.cmdFromChapterChanged);
            // 
            // m_checkWaterMark
            // 
            this.m_checkWaterMark.Location = new System.Drawing.Point(16, 19);
            this.m_checkWaterMark.Name = "m_checkWaterMark";
            this.m_checkWaterMark.Size = new System.Drawing.Size(182, 24);
            this.m_checkWaterMark.TabIndex = 9;
            this.m_checkWaterMark.Text = "Print Background Watermark:";
            // 
            // m_labelLineSpacing
            // 
            this.m_labelLineSpacing.Location = new System.Drawing.Point(16, 91);
            this.m_labelLineSpacing.Name = "m_labelLineSpacing";
            this.m_labelLineSpacing.Size = new System.Drawing.Size(100, 23);
            this.m_labelLineSpacing.TabIndex = 22;
            this.m_labelLineSpacing.Text = "Line Spacing:";
            this.m_labelLineSpacing.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // m_comboLineSpacing
            // 
            this.m_comboLineSpacing.Location = new System.Drawing.Point(120, 91);
            this.m_comboLineSpacing.Name = "m_comboLineSpacing";
            this.m_comboLineSpacing.Size = new System.Drawing.Size(128, 21);
            this.m_comboLineSpacing.TabIndex = 12;
            this.m_comboLineSpacing.Text = "Single";
            // 
            // m_groupOptions
            // 
            this.m_groupOptions.Controls.Add(this.m_textWatermark);
            this.m_groupOptions.Controls.Add(this.m_checkPrintPictures);
            this.m_groupOptions.Controls.Add(this.m_checkWaterMark);
            this.m_groupOptions.Controls.Add(this.m_checkReplacements);
            this.m_groupOptions.Controls.Add(this.m_labelLineSpacing);
            this.m_groupOptions.Controls.Add(this.m_comboLineSpacing);
            this.m_groupOptions.Location = new System.Drawing.Point(19, 244);
            this.m_groupOptions.Name = "m_groupOptions";
            this.m_groupOptions.Size = new System.Drawing.Size(376, 119);
            this.m_groupOptions.TabIndex = 24;
            this.m_groupOptions.TabStop = false;
            this.m_groupOptions.Text = "Options";
            // 
            // m_textWatermark
            // 
            this.m_textWatermark.Location = new System.Drawing.Point(204, 21);
            this.m_textWatermark.Name = "m_textWatermark";
            this.m_textWatermark.Size = new System.Drawing.Size(161, 20);
            this.m_textWatermark.TabIndex = 23;
            // 
            // m_checkPrintPictures
            // 
            this.m_checkPrintPictures.Location = new System.Drawing.Point(16, 67);
            this.m_checkPrintPictures.Name = "m_checkPrintPictures";
            this.m_checkPrintPictures.Size = new System.Drawing.Size(352, 24);
            this.m_checkPrintPictures.TabIndex = 11;
            this.m_checkPrintPictures.Text = "Print Pictures?";
            // 
            // m_checkReplacements
            // 
            this.m_checkReplacements.Location = new System.Drawing.Point(16, 43);
            this.m_checkReplacements.Name = "m_checkReplacements";
            this.m_checkReplacements.Size = new System.Drawing.Size(352, 24);
            this.m_checkReplacements.TabIndex = 10;
            this.m_checkReplacements.Text = "Make substitutions (e.g., <<Hi>> becomes \"Hi\") ?";
            // 
            // m_radioVernacular
            // 
            this.m_radioVernacular.AutoSize = true;
            this.m_radioVernacular.Location = new System.Drawing.Point(32, 179);
            this.m_radioVernacular.Name = "m_radioVernacular";
            this.m_radioVernacular.Size = new System.Drawing.Size(76, 17);
            this.m_radioVernacular.TabIndex = 7;
            this.m_radioVernacular.TabStop = true;
            this.m_radioVernacular.Text = "Vernacular";
            this.m_radioVernacular.UseVisualStyleBackColor = true;
            // 
            // m_radioBackTranslation
            // 
            this.m_radioBackTranslation.AutoSize = true;
            this.m_radioBackTranslation.Location = new System.Drawing.Point(32, 202);
            this.m_radioBackTranslation.Name = "m_radioBackTranslation";
            this.m_radioBackTranslation.Size = new System.Drawing.Size(105, 17);
            this.m_radioBackTranslation.TabIndex = 8;
            this.m_radioBackTranslation.TabStop = true;
            this.m_radioBackTranslation.Text = "Back Translation";
            this.m_radioBackTranslation.UseVisualStyleBackColor = true;
            // 
            // m_groupWhat
            // 
            this.m_groupWhat.Location = new System.Drawing.Point(19, 163);
            this.m_groupWhat.Name = "m_groupWhat";
            this.m_groupWhat.Size = new System.Drawing.Size(373, 65);
            this.m_groupWhat.TabIndex = 27;
            this.m_groupWhat.TabStop = false;
            this.m_groupWhat.Text = "What do you want to print?";
            // 
            // DialogPrint
            // 
            this.AcceptButton = this.m_btnOK;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.CancelButton = this.m_btnCancel;
            this.ClientSize = new System.Drawing.Size(410, 414);
            this.Controls.Add(this.m_radioBackTranslation);
            this.Controls.Add(this.m_radioVernacular);
            this.Controls.Add(this.m_radioThisSection);
            this.Controls.Add(this.m_radioEntireBook);
            this.Controls.Add(this.m_comboPrinter);
            this.Controls.Add(this.m_lblPrinter);
            this.Controls.Add(this.m_btnHelp);
            this.Controls.Add(this.m_btnCancel);
            this.Controls.Add(this.m_btnOK);
            this.Controls.Add(this.m_groupRange);
            this.Controls.Add(this.m_groupOptions);
            this.Controls.Add(this.m_groupWhat);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DialogPrint";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Print";
            this.Load += new System.EventHandler(this.cmdLoad);
            this.Closing += new System.ComponentModel.CancelEventHandler(this.cmdClosing);
            this.m_groupRange.ResumeLayout(false);
            this.m_groupRange.PerformLayout();
            this.m_groupOptions.ResumeLayout(false);
            this.m_groupOptions.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		// Command Handlers ------------------------------------------------------------------
		#region Cmd: cmdLoad
		private void cmdLoad(object sender, System.EventArgs e)
		{
			// Localization
            Control[] vExclude = { };
            LocDB.Localize(this, vExclude);

			// Set up the Printer Combo to the list of printers
			foreach(string s in PrinterSettings.InstalledPrinters)
				m_comboPrinter.Items.Add(s);
			m_comboPrinter.Text = PDoc.PrinterSettings.PrinterName;

			// Set up the Line Spacing combo with the possibilities
			m_comboLineSpacing.Items.Add( SingleSpace );
            m_comboLineSpacing.Items.Add( MediumSpace );
            m_comboLineSpacing.Items.Add( DoubleSpace );
            m_comboLineSpacing.Text = SingleSpace;

			// Water Mark
			m_checkWaterMark.Checked = true;

			// Radio controls
			m_radioEntireBook.AutoCheck = false;
			m_radioThisSection.AutoCheck = false;

			// Substitutions
			MakeQuoteSubstitutions = true;

			// Pictures
			PrintPictures = true;

			// Range
			EntireBook = true;

            // What to print
		    Vernacular = true;
		}
		#endregion
		#region Cmd: cmdClosing
		private void cmdClosing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			// We're only interested in further processing if the user has hit the OK
			// button, signaling he is done and wishes to save his results.
			if (DialogResult != DialogResult.OK)
				return;

			// If the user has specified to print a range of chapters, then make sure 
			// he has also specified chapter numbers
			if (Chapters && (StartChapter == 0 || EndChapter == 0) )
			{
				Messages.MissingChapterRange();
				e.Cancel = true;
				return;
			}
		}
		#endregion
		#region Cmd: cmdHelp
		private void cmdHelp(object sender, System.EventArgs e)
		{
			HelpSystem.ShowTopic(HelpSystem.Topic.kPrinting);
		}
		#endregion
		#region Cmd: cmdEntireBookChecked
		private void cmdEntireBookChecked(object sender, System.EventArgs e)
		{
			EntireBook = true;
		}
		#endregion
		#region Cmd: cmdCurrentSectionChecked
		private void cmdCurrentSectionChecked(object sender, System.EventArgs e)
		{
			CurrentSection = true;
		}
		#endregion
		#region Cmd: cmdChaptersChecked
		private void cmdChaptersChecked(object sender, System.EventArgs e)
		{
			Chapters = true;
		}
		#endregion
		#region Cmd: cmdToChapterChanged
		private void cmdToChapterChanged(object sender, System.EventArgs e)
		{
			Chapters = true;
		}
		#endregion
		#region Cmd: cmdFromChapterChanged
		private void cmdFromChapterChanged(object sender, System.EventArgs e)
		{
			Chapters = true;		
		}
		#endregion
	}
	#endregion

	#region CLASS: PPosition - a pos within a section/paragraph
	public class PPosition
	{
		// Public Virtual Attrs --------------------------------------------------------------
		#region Attr{g}: DBook Book
		public DBook Book
		{
			get
			{
				return Section.Book;
			}
		}
		#endregion
		#region Attr{g}: DSection Section - the current section
		public DSection Section
		{
			get
			{
				return Paragraph.Section;
			}
		}
		#endregion
		#region Attr{g}: JParagraphStyle ParagraphStyle
		public JParagraphStyle ParagraphStyle
		{
			get
			{
				return DB.StyleSheet.FindParagraphStyleOrNormal(Paragraph.StyleAbbrev);
			}
		}
		#endregion
		#region Attr{g}: bool WordIsDropCap
		public bool WordIsDropCap
			// A word is a dropcap if it is the first word in the paragraph, and
			// if it has the DChapter as its run.
		{
			get
			{
				if (PWords.Count == 0)
					return false;

				if (iWord != 0)
					return false;

				if (Paragraph.Runs[0] as DChapter == null)
					return false;

				return true;
			}
		}
		#endregion
		#region Attr{g}: PWord Word
		public PWord Word
		{
			get
			{
				if (PWords.Count == 0)
					return null;
				if (iWord >= PWords.Count)
					return null;
				return PWords[iWord];
			}
		}
		#endregion
		#region Attr{g}: int Length
		public int Length
		{
			get
			{
				return PWords.Count;
			}
		}
		#endregion
		#region Attr{g}: bool IsParagraphBegin
		public bool IsParagraphBegin
		{
			get
			{
				// First word in the paragraph
				if (iWord == 0)
					return true;

				// Second word, if the first word is a drop cap
				if (iWord == 1 && Paragraph.Runs[0] as DChapter != null)
					return true;

				return false;
			}
		}
		#endregion

		// Content Attributes ----------------------------------------------------------------
		#region Attr{g}: DParagraph Paragraph
		public DParagraph Paragraph
		{
			get
			{
				Debug.Assert(null != m_Paragraph);
				return m_Paragraph;
			}
		}
		protected DParagraph m_Paragraph;
		#endregion
        #region Attr{g}: List<PWord> PWords
        public List<PWord> PWords
		{
			get
			{
				Debug.Assert(null != m_vPWords);
				return m_vPWords;
			}
		}
	    private List<PWord> m_vPWords;
		#endregion
		#region Attr{g/s}: int iWord - we allow it to be incremented
		public int iWord
		{
			get
			{
				return m_iWord;
			}
			set
			{
				m_iWord = value;
			}
		}
		protected int m_iWord = 0;
		#endregion

		// Helper Methods --------------------------------------------------------------------
		#region Method: void ApplyGlueToWords()
		void ApplyGlueToWords()
		{
			for(int i=0; i<m_vPWords.Count - 1; i++)
			{
				// Get this word, and the next word
				PWord word = m_vPWords[i];
				PWord next = m_vPWords[i+1];

			    bool bNeedsGlue = false;
                if (word.NeedsGlueToNext || next.NeedsGlueToPrevious)
                    bNeedsGlue = true;

                // Determine if glue required: based on following punctuation
                if (!string.IsNullOrEmpty(next.Text) &&  
                    DB.TargetTranslation.WritingSystemVernacular.IsPunctuation(next.Text[0]))
                {
                    bNeedsGlue = true;
                }

				// If so, then glue it and close up the array
				if (bNeedsGlue)
				{
					word.GlueTo = next;
				    PWords.RemoveAt(i + 1);
				}
			}
		}
		#endregion
		#region Method: bool InitializeParagraph()
		protected void InitializeParagraph()
		{
			// Get its individual PWords
			m_vPWords = Paragraph.GetPWords();

			// Apply Glue by looking at styles
			ApplyGlueToWords();
		}
		#endregion

		// Public Interface ------------------------------------------------------------------
		#region Constructor( DParagraph, iWord)
		public PPosition(DParagraph _paragraph, int _iWord)
		{
			Debug.Assert(null != _paragraph);
			m_Paragraph = _paragraph;

			InitializeParagraph();

			m_iWord = _iWord;
		}
		#endregion
		#region private Constructor() - used by Clone()
		private PPosition()
		{
		}
		#endregion
		#region Method: PWord GetWord(int i)
		public PWord GetWord(int i)
		{
			Debug.Assert(i >= 0 && i < PWords.Count );
			return PWords[i];
		}
		#endregion
		#region Method: PPosition Clone()
		public PPosition Clone()
		{
			// Create an empty object. We can't use the public constructor, because
			// we can't be certain that PWords hasn't been changed (e.g., via
			// the PrependLetter method), and using the public constructor would
			// erase any changes made.
			var pos = new PPosition();

			// Set the attributes
			pos.m_iWord = iWord;
			pos.m_Paragraph = Paragraph;

			// Make a copy of the array (but don't worry about copying the
			// contents; garbage collection should handle these ok.
            pos.m_vPWords = new List<PWord>();
            foreach(var pw in PWords)
                pos.m_vPWords.Add(pw);

			return pos;
		}
		#endregion
        #region Method: void Prepend(PWord newWord)
        public void Prepend(PWord newWord)
            // Purpose is to support adding a footnote's letter to the beginning of
            // the set of words, as the letter is not stored within the DFootnote
            // object, and thus the routine to initialise PWords does not include it.
            //
            // We want to add the letter to the front; and because we're doing it via
            // glue, we just take the previous front word and make it a GlueTo on the
            // new letter we're inserting.
        {
            if (PWords.Count == 0)
                return;

            newWord.GlueTo = m_vPWords[0];
            m_vPWords[0] = newWord;
        }
        #endregion
    }
	#endregion
	#region CLASS: DynamicPosition - Like PPosition, but can increment the DParagraph & DSection
	public class DynamicPosition : PPosition
	{
		// Content Attributes ----------------------------------------------------------------
		#region Attr{g}: bool JustPrintOneSection
		public bool JustPrintOneSection
		{
			get
			{
				return m_bJustPrintOneSection;
			}
		}
		bool  m_bJustPrintOneSection = false;
		#endregion
		#region Attr{g}: bool PrintPictures - T if pictures are desired in the printout
		public bool PrintPictures
		{
			get
			{
				return m_bPrintPictures;
			}
		}
		bool  m_bPrintPictures = true;
		#endregion

		// Public Methods --------------------------------------------------------------------
		#region Constructor(DParagraph, iWord, bJustPrintOneSection)
		public DynamicPosition(DParagraph _paragraph, bool _bJustPrintOneSection,
			bool _bPrintPictures)
			: base( _paragraph, 0)
		{
			// Only legal for paragraphs & pictures, not for the DFootnote subclass
			Debug.Assert( _paragraph as DFootnote == null );

			m_bJustPrintOneSection = _bJustPrintOneSection;
			m_bPrintPictures = _bPrintPictures;
		}
		#endregion
		#region Method: bool IncrementWord()
		public bool IncrementWord()
		{
			// If there are no more words, then we are done
			if ( m_iWord == PWords.Count)
				return false;

			// Otherwise, increment the indexer
			++m_iWord;
			return true;
		}
		#endregion
        #region Method: bool IncrementParagraph(IProgressIndicator)
        public bool IncrementParagraph(IProgressIndicator progress)
		{
            // Footnote
            if (null != Paragraph as DFootnote)
            {
                var Footnotes = Section.AllFootnotes;

                // Find the footnote's index within all of them
                int iFootnote = 0;
                foreach (DFootnote f in Footnotes)
                {
                    if (f == Paragraph as DFootnote)
                        break;
                    iFootnote++;
                }
                Debug.Assert(iFootnote < Footnotes.Count);

                // If there are no more in this section, we must increment to the next section
                if (iFootnote >= Footnotes.Count - 1)
                    return IncrementSection(progress);

                // Increment to the next footnote
                iFootnote++;
                m_Paragraph = Footnotes[iFootnote] as DParagraph;
                InitializeParagraph();
                m_iWord = 0;
                return true;
            }

			// Find the paragraph's index within the JOwnSeq
			int iParagraph = 0;
            foreach (DParagraph p in Section.Paragraphs)
			{
				if (p == Paragraph)
					break;
				iParagraph++;
			}
            Debug.Assert(iParagraph < Section.Paragraphs.Count);

			// If there are no more paragraphs in this section, then we must 
			// increment to the next section.
            if (iParagraph >= Section.Paragraphs.Count - 1)
                return IncrementSection(progress);

			// Increment to the next paragraph
			++iParagraph;
            m_Paragraph = Section.Paragraphs[iParagraph];
			InitializeParagraph();
			m_iWord = 0;

			// If pictures are not desired, then increment past them
			if (!PrintPictures && Paragraph as DPicture != null)
                return IncrementParagraph(progress);

			// Successful
			return true;
		}
		#endregion
        #region Method: bool IncrementSection(IProgressIndicator)
        public bool IncrementSection(IProgressIndicator progress)
		{
			// If the user only wants the current section, then we don't increment.
			if (m_bJustPrintOneSection)
				return false;

			// Find the section's index within the book
			int iSection = 0;
			foreach(DSection s in Book.Sections)
			{
				if (s == Section)
					break;
				iSection++;
			}
			Debug.Assert(iSection < Book.Sections.Count);

			// If there are no more sections in the book, then we can't increment
			if (iSection == Book.Sections.Count - 1)
				return false;

			// Increment to the next section (which is defined by getting the
			// first paragraph in that next section)
			DSection sectionNext = Book.Sections[ iSection + 1] as DSection;
			m_Paragraph = sectionNext.Paragraphs[0] as DParagraph;
			InitializeParagraph();
			m_iWord = 0;
            progress.Step();

			// Successful
			return true;
		}
		#endregion
		#region Method: void SetTo( PPosition pos )
		public void SetTo( PPosition pos )
		{
			this.m_Paragraph = pos.Paragraph;
			InitializeParagraph();

			this.m_iWord = pos.iWord;
		}
		#endregion
		#region Attr{g}: bool CanIncrement
		public bool CanIncrement
		{
			get
			{
				// Do we have more words?
				if (iWord < PWords.Count - 1)
					return true;

                // Do we have more footnotes? We scan through the list of all but the last
                // footnote; if we encounter our currentn Paragraph, then we know we have
                // at least one more footnote following it.
                if (null == Paragraph as DFootnote)
                {
                    var AllFootnotes = Section.AllFootnotes;
                    for (int i = 0; i < AllFootnotes.Count - 1; i++)
                    {
                        if (AllFootnotes[i] == Paragraph as DFootnote)
                            return true;
                    }
                }

				// Do we have more paragraphs?
                int iPara = Section.Paragraphs.FindObj(Paragraph);
                if (iPara < Section.Paragraphs.Count - 1)
                    return true;

				// Do we have more sections?
				if (!JustPrintOneSection)
				{
					int iSection = Book.Sections.FindObj( Section );
					if (iSection < Book.Sections.Count - 1)
						return true;
				}

				return false;
			}
		}
		#endregion
	}
	#endregion

	#region CLASS: PLine
	public class PLine
	{
		// Private Attrs ---------------------------------------------------------------------
		#region Attr{g/s}: float Y - The y coordinate for printing the line
		public float Y
		{
			get
			{
				return m_y;
			}
			set
			{
				m_y = value;
			}
		}
		float m_y;
		#endregion
		float m_xl;                 // The left-side x coordinate for the line
		float m_xr;                 // The right-side x coordinate for the line
		int m_cWords = 0;
		float m_fJustificationFillNeeded = 0;
		Bitmap m_bmp;
		const int yPointsBetweenPictureAndCaption = 10;

		#region Attr{g}: PPosition LinePos - the Pos where this line starts
		public PPosition LinePos
		{
			get
			{
				Debug.Assert(null != m_LinePos);
				return m_LinePos;
			}
		}
		PPosition m_LinePos;       // A clone of the Print's Pos, frozen in time
		#endregion

		// Public Attrs ----------------------------------------------------------------------
		#region Attr{g}: float Height
		public float Height
		{
			get
			{
				if (IsPicture)
					return m_yLineHeight;
				return m_yLineHeight * Print.LineSpacing;
			}
		}
		float m_yLineHeight = 0;
		#endregion
		#region Attr{g}: bool IsParagraphEnd
		public bool IsParagraphEnd
		{
			get
			{
				return m_bIsParagraphEnd;
			}
		}
		bool m_bIsParagraphEnd = true;
		#endregion
		#region Attr{g}: bool IsParagraphBegin
		public bool IsParagraphBegin
		{
			get
			{
				return m_bIsParagraphBegin;
			}
		}
		bool m_bIsParagraphBegin = false;
		#endregion
		#region Attr{g}: bool IsPicture
		public bool IsPicture
		{
			get
			{
				bool bIsDPicture = ( Paragraph as DPicture != null );
				return (bIsDPicture && IsParagraphBegin);
			}
		}
		#endregion
		#region Attr{g}: DRun[] FootnoteRuns
		public DRun[] FootnoteRuns
		{
			get
			{
				ArrayList a = new ArrayList();

				foreach(PWord word in Words)
				{
					var vRuns = word.FootnoteRuns;

                    foreach (var r in vRuns)
						a.Add(r);
				}

				DRun[] v = new DRun[ a.Count ];

				for(int i=0; i<a.Count; i++)
					v[i] = a[i] as DRun;

				return v;
			}
		}
		#endregion
		#region Attr{g}: DParagraph Paragraph
		public DParagraph Paragraph
		{
			get
			{
				return m_Paragraph;
			}
		}
		DParagraph m_Paragraph;
		#endregion
		#region Attr{g}: JParagraphStyle ParagraphStyle
		public JParagraphStyle ParagraphStyle
		{
			get
			{
				return DB.StyleSheet.FindParagraphStyleOrNormal(Paragraph.StyleAbbrev);
			}
		}
		#endregion
        #region Attr{g}: List<PWord> Words
        public List<PWord> Words
		{
			get
			{
                Debug.Assert(null != m_vWords);
				return m_vWords;
			}
		}
	    readonly List<PWord> m_vWords;
		#endregion
		#region Attr{g}: PDropCap DropCap - the DropCap that goes with this line, or null
		public PDropCap DropCap
		{
			get
			{
				return m_dropcap;
			}
		}
		PDropCap m_dropcap;
		#endregion
		#region Attr{g}: int FirstVerseNo
		public int FirstVerseNo
		{
			get
			{
				string sVerse = "";
				foreach(PWord w in Words)
				{
					if (w.IsVerseNumber)
					{
						sVerse = w.Text;
						break;
					}
				}

				string sDigits = "";
				foreach(char ch in sVerse)
				{
					if (char.IsDigit(ch))
						sDigits += ch;
					else break;
				}

				if (sDigits.Length == 0)
					return 0;

				return Convert.ToInt16(sDigits);
			}
		}
		#endregion
		#region Attr{g}: char FirstFootnoteLetter - first Footnote Letter amongst the Words array
		public char FirstFootnoteLetter
		{
			get
			{
				foreach(PWord w in Words)
				{
					char ch = w.FirstFootnoteLetter;
					if (ch != ' ')
						return ch;
				}

				return ' ';
			}
		}
		#endregion

		// Public Methods --------------------------------------------------------------------
		#region Method: void Draw(g)
		public void Draw(Graphics g)
		{
			// The left coordinate depends upon the pargraph style (centered, justified, etc.)
			float x = m_xl;
			if (ParagraphStyle.IsCentered)
				x += (m_fJustificationFillNeeded / 2);
			if (ParagraphStyle.IsRight)
				x += m_fJustificationFillNeeded;
			float fJustificationBetween = m_fJustificationFillNeeded / (m_cWords - 1);

			// Draw the picture if applicable
			if (IsPicture && null != m_bmp)
			{
				float xMid = ( m_xr + m_xl ) / 2;
				float xImage = xMid - m_bmp.Width / 2;
				g.DrawImage(m_bmp, xImage, m_y);
				m_y += m_bmp.Height;
				m_y += yPointsBetweenPictureAndCaption;
			}

			// Draw the DropCap, if applicable
			if (null != DropCap)
				DropCap.Draw(g);

			// Loop through all the words in this line
			foreach(PWord word in Words)
			{
				// Output it to the printer
				word.Draw(g, x, m_y);

				// Position for the next word
				x += word.Measure(g).Width;
				x += word.MeasureSpace(g).Width;
				if (ParagraphStyle.IsJustified && !m_bIsParagraphEnd)
					x += fJustificationBetween;
			}
		}
		#endregion
		#region Constructor(...)
		public PLine( Graphics g, PPosition _pos, float _y, float _xl, float _xr, 
			PDropCap _dc, ref char chFootnoteLetter)
		{
			// Set up the attrs that we'll need to remember later for the Draw method
			m_LinePos = _pos.Clone();
			m_y = _y;
			m_xl = _xl;
			m_xr = _xr;
			m_Paragraph = LinePos.Paragraph;
			m_dropcap = _dc;

            m_vWords = new List<PWord>();

			// Beginning of a paragraph?
			m_bIsParagraphBegin = LinePos.IsParagraphBegin;

			// If a picture, then get its bitmap
			if (IsPicture)
			{
				var picture = Paragraph as DPicture;
				if (null != picture)
					m_bmp = picture.GetBitmap(300);
			}

			// Populate the line with words: Loop through all of the words, adding them
			// to the line as long as there is space
			float x = m_xl;
			for(var i = LinePos.iWord; i < LinePos.PWords.Count; i++)
			{
				// Get the next word in the paragraph
				PWord word = LinePos.PWords[i];

				// Change it if it is a footnote letter
				char chFootnoteLetterInitial = chFootnoteLetter;
				word.SetFootnoteLetter(ref chFootnoteLetter);

				// Calculate its width, and the width of a space in its same font
				float fWordWidth  = word.Measure(g).Width;
				float fWordHeight = word.Measure(g).Height;
				float fSpaceWidth = word.MeasureSpace(g).Width;

				// Will this word fit on the line? If not, we can't add anymore. (Restore
				// the chFootnoteLetter to what it was, since we didn't use the word and
				// it will be calculated for the next time this constructor is called.)
				if (x + fWordWidth > m_xr)
				{
					m_bIsParagraphEnd = false;
					chFootnoteLetter = chFootnoteLetterInitial;
					break;
				}
                Words.Add(word);

				// Increment x to account for the word's width
				x += fWordWidth;

				// Recalc the amount of justification fill that would be needed for
				// this amount of line.
				m_fJustificationFillNeeded = m_xr - x;

				// Increment x to account for the space after the word. (We don't
				// want this final space to be included in the justification 
				// calculation.)
				x += fSpaceWidth;

				// Keep track of how many words are in this line
				m_cWords ++;

				// The line height will be the height of the tallest font
				m_yLineHeight = Math.Max(m_yLineHeight, fWordHeight);
			}

			// Add the height of the picture, if any
			if (IsPicture && null != m_bmp)
				m_yLineHeight += (m_bmp.Height + yPointsBetweenPictureAndCaption);
		}
		#endregion
	}
	#endregion

	#region CLASS: PDropCap
	public class PDropCap
	{
		// Content Attrs ---------------------------------------------------------------------
		#region Attr{g]: PWord Word
		public PWord Word
		{
			get
			{
				Debug.Assert(null != m_word);
				return m_word;
			}
		}
		PWord m_word;
		#endregion
		#region Attr{g}: PointF Location
		PointF Location
		{
			get
			{
				return m_Location;
			}
		}
		PointF m_Location;
		#endregion

		// Public Interface ------------------------------------------------------------------
		#region Constructor(PWord, PointF)
		public PDropCap(PWord _word, PointF _Location)
		{
			m_word = _word;
			m_Location = _Location;
		}
		#endregion
		#region Method: float GetWidth(Graphics g)
		public float GetWidth(Graphics g)
		{
			return Word.Measure(g).Width;
		}
		#endregion
		#region Method: float GetHeight(Graphics g)
		public float GetHeight(Graphics g)
		{
			return Word.Measure(g).Height;
		}
		#endregion
		#region Method: void Draw(Graphics)
		public void Draw(Graphics g)
		{
			g.DrawString( Word.Text, Word.Font, Word.TextBrush, Location);
		}
		#endregion
		#region Attr{g}: int TextAsNumber
		public int TextAsNumber
		{
			get
			{
				string sDigits = "";
				foreach(char ch in Word.Text)
				{
					if (char.IsDigit(ch))
						sDigits += ch;
					else break;
				}

				if (sDigits.Length == 0)
					return 0;

				return Convert.ToInt16(sDigits);
			}
		}
		#endregion
	}
	#endregion

	#region CLASS: PPage
	class PPage
	{
		// Virtual Attrs ---------------------------------------------------------------------
		#region Attr{g}: bool PageHasFootnotes
		bool PageHasFootnotes
		{
			get
			{
				return (FootnoteLines.Count > 0);
			}
		}
		#endregion
		#region Attr{g}: int FirstVerseNo
		public int FirstVerseNo
		{
			get
			{
				foreach(PLine line in BodyLines)
				{
					int nVerse = line.FirstVerseNo;
					if (nVerse != 0)
						return nVerse;
				}
				return 0;
			}
		}
		#endregion
		#region Attr{g}: int FirstChapterNo
		public int FirstChapterNo
		{
			get
			{
				if (1 == FirstVerseNo)
				{
					if (DropCaps.Count == 0)
						return 1;
					PDropCap cap = DropCaps[0] as PDropCap;
					return cap.TextAsNumber;
				}

				if (null == PreviousPage)
					return 1;

				return PreviousPage.LastChapterNo;
			}
		}
		#endregion
		#region Attr{g}: int LastChapterNo
		public int LastChapterNo
		{
			get
			{
				if (DropCaps.Count == 0)
				{
					if (null == PreviousPage)
						return 1;
					return PreviousPage.LastChapterNo;
				}

				int i = DropCaps.Count - 1;
				PDropCap cap = DropCaps[i] as PDropCap;
				return cap.TextAsNumber;
			}
		}
		#endregion
		#region Attr{g}: bool IsEvenPage
		public bool IsEvenPage
		{
			get
			{
				// Divide the page number in half, any remainder (e.g., as will be true
				// with an odd number) will be dropped
				int nHalf = m_nPageNo / 2;

				if (nHalf * 2 == m_nPageNo)
					return true;
				return false;
			}
		}
		#endregion

		// Content Attrs ---------------------------------------------------------------------
		#region Attr{g}: ArrayList BodyLines
		ArrayList BodyLines
		{
			get
			{
				Debug.Assert(null != m_aBodyLines);
				return m_aBodyLines;
			}
		}
		ArrayList m_aBodyLines;
		#endregion
		#region Attr{g}: ArrayList FootnoteLines
		ArrayList FootnoteLines
		{
			get
			{
				Debug.Assert(null != m_aFootnoteLines);
				return m_aFootnoteLines;
			}
		}
		ArrayList m_aFootnoteLines;
		#endregion
		#region Attr{g}: ArrayList DropCaps
		ArrayList DropCaps
		{
			get
			{
				Debug.Assert(null != m_aDropCaps);
				return m_aDropCaps;
			}
		}
		ArrayList m_aDropCaps;
		#endregion

		#region Attr{g}: Rectangle RectPage - the printable area of the page
		Rectangle RectPage
		{
			get
			{
				return m_rectPage;
			}
		}
		Rectangle m_rectPage;
		#endregion
		#region Attr{g}: PrintDocument Doc
		PrintDocument Doc
		{
			get
			{
				Debug.Assert(null != m_pdoc);
				return m_pdoc;
			}
		}
		PrintDocument m_pdoc;
		#endregion
		#region Attr{g}: bool WaterMarkDesired
		bool WaterMarkDesired
		{
			get
			{
				return m_bWaterMarkDesired;
			}
		}
		bool m_bWaterMarkDesired = false;
		#endregion
		#region Attr{g}: string PageNo - the number of this page
		string PageNo
		{
			get
			{
				return m_nPageNo.ToString();
			}
		}
		int m_nPageNo = 0;
		#endregion
		#region Attr{g/s}: float yFootnotes
		float yFootnotes
		{
			get
			{
				return m_yFootnotes;
			}
			set
			{
				m_yFootnotes = value;
			}
		}
		float m_yFootnotes;
		#endregion
		#region Attr{g}: PPage PreviousPage
		public PPage PreviousPage
		{
			get
			{
				return m_PreviousPage;
			}
		}
		PPage m_PreviousPage;
		#endregion

		// Water Mark -------------------------------------------------------------------------
		#region Method: void PrintWatermark(Graphics)
		void PrintWatermark(Graphics g)
		{
			// Don't proceed if the water mark isn't wanted
			if (!WaterMarkDesired)
				return;

			// Get the string to print
			string sBookStatus = DB.TargetBook.Stage.LocalizedName;

			// Calculate the maximum length this text should appear (non-rotated). We'll
			// assume a 30-degree angle. 
			float fAngle = 30;
			double fRadians = fAngle * Math.PI / 180.0;
			float PageWidth = RectPage.Width;
			float TextWidthMax = PageWidth / (float)Math.Cos( fRadians );
			TextWidthMax *= 0.90F; // Fudge down so we're sure to fit the margins.

			// Find the largest font size that does not exceed this width
			float fSize = 20;
			do
			{
				Font f = new Font("Arial", fSize, FontStyle.Bold);

				float fw = g.MeasureString(sBookStatus, f).Width;
				if (fw > TextWidthMax)
					break;

				fSize += 1;
			} while (true);

			// Create the font we'll actually use
			Font font = new Font("Arial", fSize, FontStyle.Bold);

			// We want a very light gray
			int nGray = 225;
			Brush brush = new SolidBrush( Color.FromArgb( nGray, nGray, nGray) );

			// Figure out where to put the text
			float x = RectPage.Left;
			SizeF szText = g.MeasureString(sBookStatus, font);
			float fAngledHeight = szText.Width  * (float)Math.Tan( fRadians );
			float fTextHeight   = szText.Height * (float)Math.Cos( fRadians );
			float y = RectPage.Top + 
				(RectPage.Height / 2) +
				(fAngledHeight / 2) -
				(fTextHeight / 2);
			
			// Draw the text
			g.TranslateTransform(x, y);
			g.RotateTransform( -fAngle );
			g.DrawString(sBookStatus, font, brush, 0, 0);
			g.ResetTransform();
		}
		#endregion

		// Footer ----------------------------------------------------------------------------
		#region Method: string _GetFooterPartString( DTeamSettings.FooterParts kFooterPart)
		string _GetFooterPartString( DTeamSettings.FooterParts kFooterPart)
		{
			// The Page Number
			if ( kFooterPart == DTeamSettings.FooterParts.kPageNumber)
			{
				return  "- " + PageNo + " -";
			}

			// Copyright notice
			if ( kFooterPart == DTeamSettings.FooterParts.kCopyrightNotice)
			{
				string sCopyright = DB.TeamSettings.CopyrightNotice;

				if (DB.TargetBook.HasCopyrightNotice)
					sCopyright = DB.TargetBook.Copyright;

				return sCopyright;
			}

			// Scripture Reference
			if ( kFooterPart == DTeamSettings.FooterParts.kScriptureReference)
			{
				string s = FooterBookName + " " + 
					FirstChapterNo.ToString() +	":" + 
					FirstVerseNo.ToString();
				return s;
			}

			// Stage and Date
			if ( kFooterPart == DTeamSettings.FooterParts.kStageAndDate)
			{
				string sBookStatus = DB.TargetBook.Stage.LocalizedName;
				string sDate = DateTime.Today.ToShortDateString();
				return sBookStatus + " - " + sDate;
			}

			// Language Name, Stage and Date
			if ( kFooterPart == DTeamSettings.FooterParts.kLanguageStageAndDate)
			{
				string sName       = DB.TargetBook.Translation.DisplayName + " ";
				string sBookStatus = DB.TargetBook.Stage.LocalizedName;
				string sDate       = DateTime.Today.ToShortDateString();

				return sName + " - " + sBookStatus + " - " + sDate;
			}

			// Leave Blank (or unknown kFooterPart)
			return "";
		}
		#endregion
		#region Method: float GetFooterAreaHeight(Graphics g)
		float GetFooterAreaHeight(Graphics g)
		{
			// Retrieve the font for the footer
			JParagraphStyle ps = DB.StyleSheet.FindParagraphStyleOrNormal("h");

			// Measure arbitrary text in that font.
            Font font = ps.CharacterStyle.FindOrAddFontForWritingSystem(
                DB.TargetTranslation.WritingSystemVernacular).DefaultFont;
			float yTextHeight =  g.MeasureString("ABCDE", font).Height;

			// Allow double this line height
			return yTextHeight * 2;
		}
		#endregion
		#region Attr{g/s}: string FooterBookName
		public string FooterBookName
		{
			get
			{
				return m_sFooterBookName;
			}
			set
			{
				m_sFooterBookName = value;
			}
		}
		static string m_sFooterBookName;
		#endregion
        #region Method: RetrieveFooterBookName(IProgressIndicator)
        void RetrieveFooterBookName(ref DynamicPosition Pos, IProgressIndicator progress)
		{
			if (null == Pos.Word)
				return;

			if (Pos.ParagraphStyle.SFM == "h")
			{
				FooterBookName = Pos.Paragraph.SimpleText;
                Pos.IncrementParagraph(progress);
			}
		}
		#endregion
		#region Method: void DrawRunningFooter(Graphics g)
		void DrawRunningFooter(Graphics g)
		{
			// Is this an Odd or an Even page?
			DTeamSettings.FooterParts kLeft  = DB.TeamSettings.OddLeft;
			DTeamSettings.FooterParts kMid   = DB.TeamSettings.OddMiddle;
			DTeamSettings.FooterParts kRight = DB.TeamSettings.OddRight;
			if (IsEvenPage)
			{
				kLeft  = DB.TeamSettings.EvenLeft;
				kMid   = DB.TeamSettings.EvenMiddle;
				kRight = DB.TeamSettings.EvenRight;
			}

			// Retrieve the font for the footer
			JParagraphStyle ps = DB.StyleSheet.FindParagraphStyleOrNormal("h");
            Font font = ps.CharacterStyle.FindOrAddFontForWritingSystem(
                DB.TargetTranslation.WritingSystemVernacular).DefaultFont;

			// Left and Right Margins
			float xLeft  = RectPage.Left;
			float xRight = RectPage.Right;
			float xMid   = (xLeft + xRight) / 2;

			// Middle
			string sMid = _GetFooterPartString( kMid );
			SizeF szMid = g.MeasureString(sMid, font);
			float y = RectPage.Bottom - szMid.Height;
			float x = xMid - (szMid.Width / 2);
			g.DrawString(sMid, font, Brushes.Black, x, y);

			// Left side.
			string sLeft = _GetFooterPartString( kLeft );
			g.DrawString(sLeft, font, Brushes.Black, xLeft, y);

			// Right side
			string sRight = _GetFooterPartString( kRight );
			float  fDateWidth = g.MeasureString(sRight, font).Width;
			g.DrawString(sRight, font, Brushes.Black, xRight - fDateWidth, y);
		}
		#endregion

		// DropCaps --------------------------------------------------------------------------
		float m_fCurrentDropCapHeight = 0;
		#region Method: void HandleDropCap(ref DynamicPosition, Graphics g, float y)
		PDropCap HandleDropCap(ref DynamicPosition Pos, Graphics g, float y)
		{
			if (Pos.WordIsDropCap)
			{
				PDropCap dc = new PDropCap(Pos.Word, new PointF(RectPage.Left, y));
				DropCaps.Add(dc);
				m_fCurrentDropCapHeight = dc.GetHeight(g);
				Pos.IncrementWord();
				return dc;
			}
			return null;
		}
		#endregion

		// Indents, Spacing Before/After, etc. -----------------------------------------------
		#region Method: float InchesToDevMeasX(Graphics, float fInches)
		float InchesToDevMeasX(Graphics g, float fInches)
		{
			float x = fInches * g.DpiX;

			// KLUDGE: for some reason we're too large by a factor of 10.
			x /= 10;

			return x;
		}
		#endregion
		#region Method: float GetLineLeftMargin(Graphics g, DynamicPosition Pos)
		float GetLineLeftMargin(Graphics g, DynamicPosition Pos)
		{
			// Start with the print boundary of the page
			float xLeft = RectPage.Left;

			// Add in any paragraph style left margin
			xLeft += InchesToDevMeasX( g, (float)Pos.ParagraphStyle.LeftMargin );

			// Are we working with a DropCap?
			bool bDropCap = (m_fCurrentDropCapHeight > 0);

			// Add in any offset due to making room for a drop cap
			if (bDropCap)
			{
				PDropCap dropcap = DropCaps[ DropCaps.Count - 1 ] as PDropCap;
				xLeft += dropcap.GetWidth(g);
			}

			// If the first line of a paragraph, and if there was no Drop Cap, then add
			// in the indentation for the first line
			if (Pos.IsParagraphBegin && !bDropCap)
				xLeft += InchesToDevMeasX(g, (float)Pos.ParagraphStyle.FirstLineIndent );

			// Return the result
			return xLeft;
		}
		#endregion
		#region Method: float GetLineRightMargin(Graphics g, DynamicPosition Pos)
		float GetLineRightMargin(Graphics g, DynamicPosition Pos)
		{
			// Start with the print boundary of the page
			float xRight = RectPage.Right;

			// Subtract any paragraph style right margin
			xRight -= InchesToDevMeasX(g, (float)Pos.ParagraphStyle.RightMargin );

			// Return the result
			return xRight;
		}
		#endregion
		#region Method: float GetParagraphSpaceBefore(DynamicPosition, float yBody)
		float GetParagraphSpaceBefore(DynamicPosition Pos, float yBody)
		{
			// Don't add vertical space if we are at the top of the page, because the purpose
			// of SpaceBefore is to separate from a preceeding paragraph; and there is no
			// preceeding paragraph at the top of a page.
			if (yBody == RectPage.Top)
				return 0;

			// Don't add vertical space if we are not at the beginning of a paragraph.
			if (!Pos.IsParagraphBegin)
				return 0;

			// If we are here, then vertical space is desired. Retrieve it from the 
			// style of the paragraph.
			float yPointsBefore = Pos.ParagraphStyle.SpaceBefore;
			return yPointsBefore;
		}
		#endregion
		#region Method: float GetParagraphSpaceAfter(DynamicPosition)
		float GetParagraphSpaceAfter(DynamicPosition Pos)
		{
			float yPointsAfter = Pos.ParagraphStyle.SpaceAfter;

			return yPointsAfter;
		}
		#endregion

		// Footnotes --------------------------------------------------------------------------
		const float c_fFootnoteDividerHeight = 10;
		#region Method: PLine[] FootnotesToPLines(Graphics, DynamicPosition, DLine TextBodyLine)
		PLine[] FootnotesToPLines( Graphics g, DynamicPosition Pos, PLine TextBodyLine)
		{
			// Given the line in text, collect all of the DRuns that are related to
			// it. These will either be DFootnote or DSeeAlso's. 
			DRun[] vFootnoteRuns = TextBodyLine.FootnoteRuns;

			// Get the first footnote letter that appears in this line
			char chFootnoteLetter = TextBodyLine.FirstFootnoteLetter;

			// A temporary destination for the PLine's we're about to create
			ArrayList a = new ArrayList();

			// A dummy y coordinate; we'll overwrite it later when it comes time
			// to actually draw the lines.
			float y = 1;

			// Loop through all of the Footnote / SeeAlso's we have
			foreach( DRun run in vFootnoteRuns)
			{
                DFoot foot = run as DFoot;
                Debug.Assert(null != foot);

				// Retrieve the footnote text. This is a subclass of DParagraph
				DFootnote footnote = foot.Footnote;
				var PosFN = new PPosition(footnote, 0);
			    PosFN.Prepend(new PWord(footnote.VerseReference,
			        DB.StyleSheet.FindCharacterStyleOrNormal(DStyleSheet.c_sfmParagraph),
			        DB.StyleSheet.FindParagraphStyleOrNormal(DStyleSheet.c_sfmParagraph))
                    {NeedsGlueToNext = true}
			        );
                PosFN.Prepend(new PWord(chFootnoteLetter.ToString(),
                    DB.StyleSheet.FindCharacterStyleOrNormal(DStyleSheet.c_StyleAbbrevFootLetter),
                    null) 
                    {
                        IsFootnoteLetter = true,
                        NeedsGlueToNext = true
                    }
                    );

				// Create PLines for it
				char chDummy = chFootnoteLetter;
				bool bFirstLine = true;
				while (PosFN.iWord < PosFN.Length)
				{
					// Determine the left margin for the footnote line (it must
					// be computed differently from Body Text, which, e.g., can
					// handle drop caps.
					float xLeft = RectPage.Left;
					xLeft += InchesToDevMeasX( g, (float)footnote.Style.LeftMargin );
					if (bFirstLine)
						xLeft += InchesToDevMeasX(g,  (float)footnote.Style.FirstLineIndent );
					bFirstLine = false;

					// Measure out the line (decides how many words are on the line)
					PLine line = new PLine(g, PosFN, y, xLeft, GetLineRightMargin(g, Pos), 
						null, ref chDummy);
					a.Add(line);

					// Ready for the next one
					y += line.Height;
					PosFN.iWord += line.Words.Count;
					chFootnoteLetter++;
				}
			}

			// Convert to a PLine vector
			PLine[] v = new PLine[ a.Count ];
			for(int i=0; i<a.Count; i++)
				v[i] = a[i] as PLine;
			return v;
		}
		#endregion
		#region Method: float MeasureFootnoteHeight( PLine[] vFootnoteLines )
		float MeasureFootnoteHeight( PLine[] vFootnoteLines )
		{
			float fFootnoteHeight = 0;

			// If this is our first encounter with footnotes on this page, then we
			// wantn to leave room for the divider line. This includes not just the
			// line, but spacing above and below it.
			if (vFootnoteLines.Length > 0 && PageHasFootnotes == false)
				fFootnoteHeight = c_fFootnoteDividerHeight; 

			// Sum up the individual line heights
			foreach(PLine line in vFootnoteLines)
				fFootnoteHeight += line.Height;

			return fFootnoteHeight;
		}
		#endregion
		#region Method: void DrawFootnotes(Graphics, float yFootnotes)
		void DrawFootnotes(Graphics g, float yFootnotes)
		{
			// Do we have something to do?
			if (!PageHasFootnotes)
				return;

			// Draw a small horizonal line to indicate where the footnotes start
			float y = yFootnotes + 5;
			float xl = RectPage.Left;
			g.DrawLine(Pens.Black, xl, y, xl + 100, y);
			y += 5;

			// Loop through to draw all of the footnotes
			foreach(PLine line in FootnoteLines)
			{
				line.Y = y;
				line.Draw(g);
				y += line.Height;
			}
		}
		#endregion

		// Widows, Orphans, KeepWithNext -----------------------------------------------------
		#region Method: int EliminateOrphan()
		int EliminateOrphan()
			// An orphan is defined as the first line of paragraph being the last line
			// on a page.If we encounrer an orphan, we want to not print it on this
			// page, but rather, have it be the first thing on the following page.
		{
			// If we only have a single line, then we want to print regardless
			if (BodyLines.Count < 1)
				return 0;

			// Retrieve the line
			PLine line = BodyLines[ BodyLines.Count - 1 ] as PLine;

			// Is it the first line of a paragraph?
			if (! line.IsParagraphBegin)
				return 0;

			// Is there more to the paragraph? If this is the only line, then
			// we are happy to print it.
			int cWordsInParagraph = line.LinePos.PWords.Count;
			int cWordsInLine      = line.Words.Count;
			if (cWordsInLine == cWordsInParagraph)
				return 0;

			// If we are here, then we have an orphan. 
			return 1;
		}
		#endregion
		#region Method: int EliminateWidow(Graphics, ref DynamicPosition)
		int EliminateWidow(Graphics g, ref DynamicPosition Pos)
			// A widow is defined as the final line of a paragraph being the first
			// line on a page. To avoid it, we back up one line so that at least
			// two will appear on the following page.
		{
			// If we only have a single line, then we want to print regardless
			if (BodyLines.Count < 1)
				return 0;

			// Retrieve the last line
			PLine line = BodyLines[ BodyLines.Count - 1 ] as PLine;

			// If there are no remaining words in the paragraph, then there is no 
			// widow.
			if (line.IsParagraphEnd)
				return 0;

			// Format the next line
			char chDummy = 'a';
			PLine lineNext = new PLine(g, Pos, 100, 
				GetLineLeftMargin(g, Pos), GetLineRightMargin(g, Pos), 
				null, ref chDummy);

			// Reset the Pos, otherwise this line will not be printed on the next page
			Pos.SetTo( lineNext.LinePos );

			// If the next line was the paragraph's end, then we have a widow
			if (!lineNext.IsParagraphEnd)
				return 0;

			// So if we are here, we have a widow. Return "1" to the caller, signalling
			// that the last line on this page needs to be moved to the following page.
			return 1;
		}
		#endregion
		#region Method: int EliminateKeepWithNext( )
		int EliminateKeepWithNext()
		{
			int cLinesToDelete = 0;
			int iLine = BodyLines.Count - 1;

			do
			{
				// If we only have a single line, then we want to print regardless
				if (iLine < 1)
					return 0;

				// Retrieve the line we'll examine
				PLine line = BodyLines[ iLine ] as PLine;

				// If the line's style is to keep with the next paragraph, then
				// we increment how many lines we need to delete; Otherwise,
				// we are done checking.
				if (line.Paragraph.Style.KeepWithNext)
				{
					++cLinesToDelete;
					--iLine;
				}
				else
					break;

			} while (true);

			return cLinesToDelete;
		}
		#endregion
		#region Method: void SendLinesToNextPage(Graphics, ref DynamicPosition)
		void SendLinesToNextPage(Graphics g, ref DynamicPosition Pos)
		{
			do
			{
				// If we only have one line left, we need to print it; otherwise
				// we'll be in an infinite loop, printing out blank page after page.
				if (BodyLines.Count < 2)
					return;

				// Test to see if there are lines that need to be removed. As long as
				// any of these turns up positive, we stay in this loop.
				int cRemove = EliminateOrphan();
				if (0 == cRemove)
					cRemove = EliminateWidow(g, ref Pos);
				if (0 == cRemove)
					cRemove = EliminateKeepWithNext();

				// If the tests did not indicate any lines to remove, then we are 
				// done here.
				if (0 == cRemove)
					return;

				// Loop to remove each line indicated by the above tests
				while (cRemove > 0)
				{
					// The index of the line we'll remove
					int iBody = BodyLines.Count - 1;

					// The line we'll remove
					PLine line = BodyLines[ iBody ] as PLine;

					// The footnote lines this line has, their count, & their height.
					PLine[] vFnLines = FootnotesToPLines(g, Pos, line);
					int cFn = vFnLines.Length;
					float fFnHeight = MeasureFootnoteHeight( vFnLines );

					// Remove the count of footnote lines from the end of the array,
					// and increase the y coordinate for the footnote area by the
					// height of the lines we are removing
					FootnoteLines.RemoveRange(
						FootnoteLines.Count - vFnLines.Length,
						vFnLines.Length);
					yFootnotes += fFnHeight;

					// Remove any DropCaps associated with this PLine
					if (null != line.DropCap)
						DropCaps.Remove(line.DropCap);

					// Remove the body line
					BodyLines.RemoveAt( iBody );

					// Reset the Position object to where it was at the beginning of
					// this line we just removed, so that it will calculate properly
					// when the next page is printed.
					Pos.SetTo( line.LinePos );

					// Loop Counter of how many still to remove
					--cRemove;
				}
			} while (true);
		}
		#endregion

		// Misc Helper Methods ---------------------------------------------------------------
		#region Method: vool IncrementToNextParagraphIfNecessary()
		bool IncrementToNextParagraphIfNecessary(ref DynamicPosition Pos, IProgressIndicator progress)
		{
			if (null == Pos.Word)
			{
                bool bOK = Pos.IncrementParagraph(progress);

				if (!bOK)
                    bOK = Pos.IncrementSection(progress);

				return bOK;
			}
			return true;
		}
		#endregion
		#region Method: IncrementY(float yBody, float yIncr)
		float IncrementY(float yBody, float yIncr)
		{
			yBody += yIncr;
			m_fCurrentDropCapHeight = Math.Max( m_fCurrentDropCapHeight - yIncr, 0.0F);
			return yBody;
		}
		#endregion
		#region Method: Rectangle AdjustForPrinter(Graphics g, Rectangle r)
		[DllImport("gdi32.dll")] private static extern Int32 
			GetDeviceCaps(IntPtr hdc, Int32 capindex);

		private const int c_PHYSICALOFFSETX = 112;
		private const int c_PHYSICALOFFSETY = 113;

		Rectangle AdjustForPrinter(Graphics g, Rectangle r)
		{
			IntPtr hDC = g.GetHdc();
			int nHardMarginLeft = GetDeviceCaps(hDC , c_PHYSICALOFFSETX);
			int nHardMarginTop  = GetDeviceCaps(hDC , c_PHYSICALOFFSETY);

			g.ReleaseHdc(hDC);

			nHardMarginLeft = (int)(nHardMarginLeft * 100.0 / g.DpiX);
			nHardMarginTop  = (int)(nHardMarginTop  * 100.0 / g.DpiY);

			r.Offset(-nHardMarginLeft , -nHardMarginTop);

			return r;
		}
		#endregion

		// Public Interface ------------------------------------------------------------------
		#region Constructor(PrintDocument pdoc, PPage PreviousPage, nPageNo, bWaterMarkDesired)
		public PPage(PrintDocument _pdoc, PPage _PreviousPage, int _nPageNo, bool _bWaterMarkDesired)
		{
			// Remember the various input parameters
			m_pdoc = _pdoc;
			m_bWaterMarkDesired = _bWaterMarkDesired;
			m_nPageNo = _nPageNo;
			m_PreviousPage = _PreviousPage;

			// Initialize the arrays
			m_aBodyLines = new ArrayList();
			m_aFootnoteLines = new ArrayList();
			m_aDropCaps = new ArrayList();

			// Default for the footer book name
			FooterBookName = DB.Project.STarget.Book.DisplayName;

			// Calculate the boundaries for the printable area. We do this by getting
			// the boundaries for the entire page, and then adding in the margins.
			Rectangle rectBounds = Doc.PrinterSettings.DefaultPageSettings.Bounds;
			Margins margins = Doc.PrinterSettings.DefaultPageSettings.Margins;
			m_rectPage = new Rectangle(0,0,0,0);
			m_rectPage.X      = rectBounds.Left   + margins.Left;
			m_rectPage.Y      = rectBounds.Top    + margins.Top;
			m_rectPage.Width  = rectBounds.Right  - margins.Right  - m_rectPage.X;
			m_rectPage.Height = rectBounds.Bottom - margins.Bottom - m_rectPage.Y;

			// Adjust for real printer boundaries; this covers a problem in dot net.
			Graphics g = _pdoc.PrinterSettings.CreateMeasurementGraphics();
            if (G.IsLinux)
            {
                m_rectPage = AdjustForPrinter(g, m_rectPage);
            }
		}
		#endregion
        #region Method: void Layout(ref DynamicPosition Pos, IProgressIndicator)
        public void Layout(ref DynamicPosition Pos, IProgressIndicator progress)
		{
			// Get a graphics object through which we can measure text size
			Graphics g = Doc.PrinterSettings.CreateMeasurementGraphics();

			// An initial value for the footnotes is the bottom of the page
			// (since we have none yet)
			yFootnotes = RectPage.Bottom - GetFooterAreaHeight(g);

			// We will start renumbering (relettering) footnotes with 'a' at the
			// beginning of each page.
			var chFootnoteLetter = 'a';

			// yBody is where we currently are in layout out the body lines
			float yBody = RectPage.Top;

			// Loop to construct the page
			while (yBody < yFootnotes)
			{
				// Retrieve the running footer if we are on that paragraph.
				RetrieveFooterBookName(ref Pos, progress);

				// If the CurrentWord is null, it means we need to increment
				// to a new paragraph (and possibly a new section)
				if (!IncrementToNextParagraphIfNecessary(ref Pos, progress))
					break;

				// If the para's first word is a Chapter Number, then it's a DropCap
				PDropCap dropcap = HandleDropCap(ref Pos, g, yBody);

				// Create and populate a line with as many words as will fit in it.
				float xl = GetLineLeftMargin(g, Pos);
				float xr = GetLineRightMargin(g, Pos);
				PLine line = new PLine(g, Pos, yBody, xl, xr, dropcap, ref chFootnoteLetter);

				// See how much room the corresponding footnotes will take
				PLine[] vFootnoteLines = FootnotesToPLines(g, Pos, line);
				float fFootnoteHeight = MeasureFootnoteHeight( vFootnoteLines );

				// Do we have room for this line (and any associated footnotes) 
				// on this page? Exit the loop if not
				if (yBody + line.Height + fFootnoteHeight > yFootnotes)
					break;

				// Add the line and its footnotes to our ArrayLists
				BodyLines.Add(line);
				foreach(PLine fl in vFootnoteLines)
					FootnoteLines.Add(fl);

				// Increment the various y values (yFootnotes gets higher, yBody gets lower)
				yFootnotes -= fFootnoteHeight;
				yBody = IncrementY(yBody, line.Height);
				if (line.IsParagraphEnd)
					yBody = IncrementY(yBody, GetParagraphSpaceAfter(Pos));

				// Increment Pos to get ready for the next line
				for(int i=0; i<line.Words.Count; i++)
					Pos.IncrementWord();
			}

			// Eliminate Widows, Orphans, KeepWithNext
			SendLinesToNextPage(g, ref Pos);
		}
		#endregion
		#region Method: void Draw(Graphics g)
		public void Draw(Graphics g)
		{
			// A diagonal background, e.g., "Draft", on the page
			PrintWatermark(g);

			// Draw the body lines
			foreach(PLine line in BodyLines)
				line.Draw(g);

			// Draw any footnotes
			DrawFootnotes(g, yFootnotes);

			// The page's footer
			DrawRunningFooter(g);
		}
		#endregion
	}
	#endregion

	#region CLASS: Print
	public class Print
	{
		// Attrs -----------------------------------------------------------------------------
		#region Attr{g}: bool CanPrint - T if there is data that can be printed
	    private static bool CanPrint
	    {
			get
			{
				return DB.Project.HasDataToDisplay;
			}
	    }
		#endregion
		#region Attr{g}: ArrayList Pages
		ArrayList Pages
		{
			get
			{
				Debug.Assert(null != m_aPages);
				return m_aPages;
			}
		}
		ArrayList m_aPages;
		#endregion

		int m_iCurrentPageNo = 0;
		DialogPrint m_dlg;

        IProgressIndicator m_Progress;

		#region Attr{g/s}: static float LineSpacing
		static public float LineSpacing
		{
			get
			{
				return s_fLineSpacing;
			}
			set
			{
				s_fLineSpacing = value;
			}
		}
		static float s_fLineSpacing = 1.0F;
		#endregion
		#region Attr{g}: DynamicPosition Pos
		DynamicPosition Pos
		{
			get
			{
				Debug.Assert(null != m_position);
				return m_position;
			}
		}
		DynamicPosition m_position = null;
		#endregion


		// Main Page Printing Method ---------------------------------------------------------
		#region Attr{g}: bool DoWePrintThisPage(PPage page, int nPageNo)
		bool DoWePrintThisPage(PPage page, int nPageNo)
		{
			// If the entire book was requested, then yes
			if (m_dlg.EntireBook)
				return true;

			// If the current section was requested, then yes
			if (m_dlg.CurrentSection)
				return true;

			// If Chapters were requested, then the answer depends upon which chapters
			// are on this page.
			if (m_dlg.Chapters)
			{
				if (page.LastChapterNo < m_dlg.StartChapter)
					return false;
				if (page.FirstChapterNo > m_dlg.EndChapter)
					return false;
				return true;
			}

			return true;
		}
		#endregion
		#region Attr{g}: bool AreThereMorePagesToLayout(int nPageNo)
		bool AreThereMorePagesToLayout(int nPageNo)
		{
			// Find out if the Pos object has more data that can be printed
			return Pos.CanIncrement;
		}
		#endregion
		#region Method: void PrintPage(...)
		private void PrintPage(object sender, PrintPageEventArgs ev) 
		{
			// Retrieve the current page
			Debug.Assert(m_iCurrentPageNo < Pages.Count);
			var page = Pages[ m_iCurrentPageNo ] as PPage;
			Debug.Assert(null != page);

			// Print it
			page.Draw(ev.Graphics);

			// Increment to the next page
			m_iCurrentPageNo++;

			// Whether or not we're done depends on whether we still have pages left
			ev.HasMorePages = (m_iCurrentPageNo < Pages.Count) ? true : false;

			// Update the progress indicator
            m_Progress.Step();
		}
		#endregion

		// Scaffolding -----------------------------------------------------------------------
		#region Constructor()
		public Print()
		{
			m_aPages = new ArrayList();
            m_Progress = new NullProgress();

			PWord.BuildReplaceTree();
		}
		#endregion
		#region Method: bool Do() - Process the Print command (dialog, then print)
		public bool Do()
		{
			// Abort if there is nothing to print
			if (!CanPrint)
				return false;

			// The PrintDocument stores many of the user settings (e.g., which printer)
			var pdoc = new PrintDocument();

			// Determine what the user wants to do; abandon if he cancels
			m_dlg = new DialogPrint(pdoc);
			m_dlg.ShowDialog();
			if (m_dlg.DialogResult != DialogResult.OK)
				return false;

			// Name of the document
			pdoc.DocumentName = DB.TargetBook.DisplayName;

			// Printer to use
			pdoc.PrinterSettings.PrinterName = m_dlg.PrinterName;

			// Disable the little "Print Progress" dialog, by using StandardPrintController
			// instead of the PrintControllerWithStatusDialog that would normally be
			// shown by default.
			pdoc.PrintController = new StandardPrintController();

			// Initial position in the document (which may be All Sections or just the
			// current Section, depending on the print settings)
			var InitialSection = DB.TargetSection;
			if (!m_dlg.CurrentSection)
				InitialSection = DB.TargetBook.Sections[0];
			if (null == InitialSection)
				return false;
			var InitialPara = InitialSection.Paragraphs[0];
			if (null == InitialPara)
				return false;

			// Create a DynamicPosition object, for incrementing through the data
			m_position = new DynamicPosition(InitialPara, m_dlg.CurrentSection == true,
				m_dlg.PrintPictures);

			// Layout the pages. We must lay out all of the pages through the
			// EndPage, even if StartPage is later, so that we know correctly
			// what the pages look like.
            m_Progress = G.CreateProgressIndicator();
            m_Progress.Start(G.GetLoc_String("strFormattingPages", "Formatting Pages..."),
                InitialSection.Book.Sections.Count);
			var nPageNo = 1;
			LineSpacing = m_dlg.LineSpacing;
			PPage PreviousPage = null;
			do
			{
				// Layout the page
				var page = new PPage(pdoc, PreviousPage, nPageNo, m_dlg.PrintWaterMark);
				PreviousPage = page;
                page.Layout(ref m_position, m_Progress);

				// If this is a page we print, then add it to the array
				if (DoWePrintThisPage(page, nPageNo))
					Pages.Add(page);

				// Done with layout?
				++nPageNo;
				if (!AreThereMorePagesToLayout(nPageNo))
					break;

            } while (true);
            m_Progress.End();

			// Print the document
            m_Progress.Start(G.GetLoc_String("strPrintingPages", "Printing Pages..."),
                Pages.Count);
			m_iCurrentPageNo = 0;
            try
            {
                pdoc.PrintPage += new PrintPageEventHandler(PrintPage);
                pdoc.Print();
            }
            catch (Exception e)
            {
                LocDB.Message("msgPrintFailed",
                    "Printing failed with Windows message:\n\n{0}.", 
                    new string[] {e.Message}, 
                    LocDB.MessageTypes.Error);
            }
            m_Progress.End();

			return true;
		}
		#endregion
	}
	#endregion
}
