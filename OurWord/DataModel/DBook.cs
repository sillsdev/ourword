/**********************************************************************************************
 * Project: Our Word!
 * File:    DBook.cs
 * Author:  John Wimbish
 * Created: 25 Jan 2004
 * Purpose: A book within the translation (e.g., Mark, Philemon)
 * Legal:   Copyright (c) 2005-08, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
#region Using
using System;
using System.Diagnostics;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Windows.Forms;
using System.IO;
using JWTools;
using JWdb;
using OurWord.Dialogs;
using OurWord.View;
using OurWord.Edit;
using NUnit.Framework;
#endregion
#region TODO
/* - ctrlRemove hard-coded standard format; set up in a table somewhere.
 * - Handle case where a field marker isn't known about here.
 */
#endregion
#region Features Implemented for DBookProperties
/* Features implemented:
 * 
 * - OK complains unless there is a filename with a non-zero length
 * 
 * - OK complains unless there is a Book Name that is unique in the Translation
 * 
 * - Browse goes to the File Name (proper folder) as its initial directory, if there is a 
 *   valid Path already.
 * 
 * - Browse dialog displays a reasonable Title, rather than the system's default
 * 
 * - The Title Bar shows the book name, e.g., "Genesis Properties"; this changes when the 
 *   Book Name field is edited.
 * 
 * - The combo does not offer Abbreviations that are already used by another book
 * 
 * - Uses the OpenFileDialog for Importing a file (and for properties, in case the user wants
 *   to change files); and the SaveFileDialog for creating a new file.
 */
#endregion
#region BOOK IDS - Obsolete, but keeping around so I don't have to retype should I ever need them
/***
		#region IDs
		public enum ID
		{
			Gen=0, Exo, Lev, Num, Deu, Jos, Jdg, Rut, Sa1, Sa2, Ki1, Ki2, Ch1, Ch2,
			Ezr, Neh, Est, Job, Psa, Pro, Ecc, Sng, Isa, Jer, Lam, Ezk, Dan, Hos, 
			Jol, Amo, Oba, Jon, Mic, Nam, Hab, Zep, Hag, Zeec, Mal, Mat, Mkr, Luk, 
			Jhn, Act, Rom, Co1, Co2, Gal, Eph, Php, Col, Th1, Th2, Ti1, Ti2, Tit, 
			Phm, Heb, Jas, Pe1, Pe2, Jn1, Jn2, Jn3, Jud, Rev, kLast
		};
		#endregion
   ***/
#endregion

namespace OurWord.DataModel
{
	// Class DBookProperties - Sets up the DBook object --------------------------------------
	#region CLASS DBookProperties
	public class DBookProperties : System.Windows.Forms.Form
	{
		// Attributes ------------------------------------------------------------------------
		#region Attr{s}: string BookPath - ensures an ellipses if the pathname is too long
		public string BookPath
		{
			get
			{
				return m_sPathName;
			}
			set
			{
				m_sPathName = value;
				if (value == "")
					m_lblPathName.Text = "(No File Name)";
				m_lblPathName.Text = JWU.PathEllipses(value, 38);
			}
		}
		private string m_sPathName = "";
		#endregion
		#region Attr{g}: DBook Book - the DBook object whose properties we are editing
		public DBook Book
		{
			get { return m_book; }
		}
		private DBook m_book = null;
		#endregion
		#region Attr{g}: DTranslation TFront - the front trans, or null
		public DTranslation TFront
		{
			get { return m_TFront; }
		}
		private DTranslation m_TFront = null;
		#endregion
		#region Attr{g}: DTranslation Translation - the Translation object that may (someday) own this book
		public DTranslation Translation
		{
			get { return m_Translation; }
		}
		private DTranslation m_Translation = null;
		#endregion

		private System.Windows.Forms.TextBox m_editCopyright;
		private System.Windows.Forms.Label m_lblCopyright;
		#region Attr{g/s}: string BookName = the contents of the Name control
		public string BookName
		{
			get { return m_textName.Text; }
			set { m_textName.Text = value; }
		}
		#endregion

		#region Attr{g/s}: string Stage - the text of the Stage combo (Abbrev of TranslationStage)
		public string Stage
		{
			get
			{
				return m_comboStage.Text;
			}
			set
			{
				m_comboStage.Text = value;
			}
		}
		#endregion
		#region Attr{g/s}: char Version - the contents of the Version spin control
		public char Version
		{
			get
			{
				return m_spinVersion.Text[0];
			}
			set
			{
				m_spinVersion.Text = value.ToString();
			}
		}
		#endregion

		// Methods ---------------------------------------------------------------------------
		#region Method: void SetFocusOnName() - selects the name and sets focus to it
		public void SetFocusOnName()
		{
			m_textName.Focus();
			m_textName.Select();
		}
		#endregion
		#region Method: void SetFocusOnPathBrowse() - selects the name and sets focus to it
		public void SetFocusOnPathBrowse()
		{
			m_btnBrowseFileName.Focus();
			m_btnBrowseFileName.Select();
		}
		#endregion
		#region Method: bool ValidateData() - make sure it is OK to close the dialog
		public bool ValidateData()
			// I do this as a separate method, rather than having it in the OnClosing handler,
			// so that the NUnit tests can call it easily.
		{
			// The book's name should have a non-zero length, signifying that the user has
			// indeed entered a name.
			if ( BookName.Length == 0 )
			{
                Messages.BookNeedsName();
                SetFocusOnName();
				return false;
			}

			// The book's name cannot be identical to the title of any other book in the
			// translation; as it doesn't make sense to have identical book names.
			foreach(DBook b in Translation.Books)
			{
				if (b.DisplayName == BookName && b != Book)
				{
                    Messages.DuplicateBookName();
                    SetFocusOnName();
					return false;
				}
			}

			// The translation's path name should be to a valid file. (This should
			// only be possible for Create and Import modes; Properties mode does not
			// permit editing of the filename.)
			if ( m_sPathName.Length == 0 || Path.GetFileName(m_sPathName).Length == 0)
			{
				if (Mode.kCreate == m_Mode)
					Messages.BookNeedsFolder();
				else // kImport
					Messages.BookNeedsImportFilename();
				SetFocusOnPathBrowse();
				return false;
			}

			// No problems found
			return true;
		}
		#endregion
		#region Method: void UpdateTitleBar() - synch up Title Bar with contents of Name field
		public void UpdateTitleBar()
		{
			if (BookName.Length == 0)
				Text = StrRes.Properties_Book;
			else
			{
				Text = StrRes.Properties( BookName.Trim() );
			}
		}
		#endregion

		// Scaffolding -----------------------------------------------------------------------
		public enum Mode { kCreate, kProperties };
		Mode m_Mode = Mode.kProperties;

		#region Constructor(TFront, DTranslation, DBook, Mode)
		public DBookProperties(DTranslation tFront, DTranslation translation, 
			DBook book, Mode mode)
		{
			// Required for Windows Form Designer support
			InitializeComponent();

			// Point to the book we are modifying
			m_book = book;
			m_Translation = translation;
			m_TFront = tFront;
			m_Mode = mode;
		}
		#endregion
		#region Windows Form Designer generated code

		private System.ComponentModel.Container components = null;

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

		/// -----------------------------------------------------------------------------------
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		/// -----------------------------------------------------------------------------------
		private void InitializeComponent()
		{
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DBookProperties));
            this.m_btnHelp = new System.Windows.Forms.Button();
            this.m_btnOK = new System.Windows.Forms.Button();
            this.m_textComment = new System.Windows.Forms.TextBox();
            this.m_lblComment = new System.Windows.Forms.Label();
            this.m_lbName = new System.Windows.Forms.Label();
            this.m_lblAbbrevation = new System.Windows.Forms.Label();
            this.m_comboAbbreviation = new System.Windows.Forms.ComboBox();
            this.m_lblFile = new System.Windows.Forms.Label();
            this.m_lblPathName = new System.Windows.Forms.Label();
            this.m_btnBrowseFileName = new System.Windows.Forms.Button();
            this.m_btnCancel = new System.Windows.Forms.Button();
            this.m_textName = new System.Windows.Forms.Label();
            this.m_comboStage = new System.Windows.Forms.ComboBox();
            this.m_spinVersion = new System.Windows.Forms.DomainUpDown();
            this.m_lblExtension = new System.Windows.Forms.Label();
            this.m_lblLanguageAbbrev = new System.Windows.Forms.Label();
            this.m_labelLanguageAbbrev = new System.Windows.Forms.Label();
            this.m_labelBookAbbrev = new System.Windows.Forms.Label();
            this.m_labelStage = new System.Windows.Forms.Label();
            this.m_labelVersion = new System.Windows.Forms.Label();
            this.m_boxFileName = new System.Windows.Forms.GroupBox();
            this.m_lblBookAbbrev = new System.Windows.Forms.Label();
            this.m_checkLocked = new System.Windows.Forms.CheckBox();
            this.m_lblCopyright = new System.Windows.Forms.Label();
            this.m_editCopyright = new System.Windows.Forms.TextBox();
            this.m_boxFileName.SuspendLayout();
            this.SuspendLayout();
            // 
            // m_btnHelp
            // 
            this.m_btnHelp.Image = ((System.Drawing.Image)(resources.GetObject("m_btnHelp.Image")));
            this.m_btnHelp.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.m_btnHelp.Location = new System.Drawing.Point(264, 408);
            this.m_btnHelp.Name = "m_btnHelp";
            this.m_btnHelp.Size = new System.Drawing.Size(75, 23);
            this.m_btnHelp.TabIndex = 7;
            this.m_btnHelp.Text = "Help...";
            this.m_btnHelp.Click += new System.EventHandler(this.cmdHelp);
            // 
            // m_btnOK
            // 
            this.m_btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.m_btnOK.Location = new System.Drawing.Point(88, 408);
            this.m_btnOK.Name = "m_btnOK";
            this.m_btnOK.Size = new System.Drawing.Size(75, 23);
            this.m_btnOK.TabIndex = 5;
            this.m_btnOK.Text = "OK";
            // 
            // m_textComment
            // 
            this.m_textComment.Location = new System.Drawing.Point(8, 320);
            this.m_textComment.Multiline = true;
            this.m_textComment.Name = "m_textComment";
            this.m_textComment.Size = new System.Drawing.Size(440, 64);
            this.m_textComment.TabIndex = 4;
            // 
            // m_lblComment
            // 
            this.m_lblComment.Location = new System.Drawing.Point(8, 304);
            this.m_lblComment.Name = "m_lblComment";
            this.m_lblComment.Size = new System.Drawing.Size(100, 16);
            this.m_lblComment.TabIndex = 21;
            this.m_lblComment.Text = "Comment:";
            this.m_lblComment.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // m_lbName
            // 
            this.m_lbName.Location = new System.Drawing.Point(8, 48);
            this.m_lbName.Name = "m_lbName";
            this.m_lbName.Size = new System.Drawing.Size(72, 23);
            this.m_lbName.TabIndex = 22;
            this.m_lbName.Text = "Book Name:";
            this.m_lbName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // m_lblAbbrevation
            // 
            this.m_lblAbbrevation.Location = new System.Drawing.Point(8, 16);
            this.m_lblAbbrevation.Name = "m_lblAbbrevation";
            this.m_lblAbbrevation.Size = new System.Drawing.Size(72, 23);
            this.m_lblAbbrevation.TabIndex = 24;
            this.m_lblAbbrevation.Text = "Abbrevation:";
            this.m_lblAbbrevation.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // m_comboAbbreviation
            // 
            this.m_comboAbbreviation.Location = new System.Drawing.Point(88, 16);
            this.m_comboAbbreviation.MaxDropDownItems = 15;
            this.m_comboAbbreviation.Name = "m_comboAbbreviation";
            this.m_comboAbbreviation.Size = new System.Drawing.Size(360, 21);
            this.m_comboAbbreviation.TabIndex = 2;
            this.m_comboAbbreviation.TextChanged += new System.EventHandler(this.cmdOnComboSelection);
            // 
            // m_lblFile
            // 
            this.m_lblFile.Location = new System.Drawing.Point(8, 80);
            this.m_lblFile.Name = "m_lblFile";
            this.m_lblFile.Size = new System.Drawing.Size(72, 23);
            this.m_lblFile.TabIndex = 26;
            this.m_lblFile.Text = "Folder:";
            this.m_lblFile.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // m_lblPathName
            // 
            this.m_lblPathName.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.m_lblPathName.Location = new System.Drawing.Point(88, 80);
            this.m_lblPathName.Name = "m_lblPathName";
            this.m_lblPathName.Size = new System.Drawing.Size(280, 23);
            this.m_lblPathName.TabIndex = 27;
            this.m_lblPathName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // m_btnBrowseFileName
            // 
            this.m_btnBrowseFileName.Location = new System.Drawing.Point(376, 80);
            this.m_btnBrowseFileName.Name = "m_btnBrowseFileName";
            this.m_btnBrowseFileName.Size = new System.Drawing.Size(75, 23);
            this.m_btnBrowseFileName.TabIndex = 3;
            this.m_btnBrowseFileName.Text = "Browse...";
            this.m_btnBrowseFileName.Click += new System.EventHandler(this.cmd_btnBrowse_clicked);
            // 
            // m_btnCancel
            // 
            this.m_btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.m_btnCancel.Location = new System.Drawing.Point(176, 408);
            this.m_btnCancel.Name = "m_btnCancel";
            this.m_btnCancel.Size = new System.Drawing.Size(75, 23);
            this.m_btnCancel.TabIndex = 28;
            this.m_btnCancel.Text = "Cancel";
            // 
            // m_textName
            // 
            this.m_textName.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.m_textName.Location = new System.Drawing.Point(88, 48);
            this.m_textName.Name = "m_textName";
            this.m_textName.Size = new System.Drawing.Size(360, 23);
            this.m_textName.TabIndex = 29;
            this.m_textName.Text = "(Book Name)";
            this.m_textName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.m_textName.TextChanged += new System.EventHandler(this.cmd_textName_TextChanged);
            // 
            // m_comboStage
            // 
            this.m_comboStage.Location = new System.Drawing.Point(189, 32);
            this.m_comboStage.Name = "m_comboStage";
            this.m_comboStage.Size = new System.Drawing.Size(132, 21);
            this.m_comboStage.TabIndex = 30;
            this.m_comboStage.Text = "Draft";
            this.m_comboStage.TextChanged += new System.EventHandler(this.cmdOnStageChanged);
            // 
            // m_spinVersion
            // 
            this.m_spinVersion.Location = new System.Drawing.Point(330, 32);
            this.m_spinVersion.Name = "m_spinVersion";
            this.m_spinVersion.Size = new System.Drawing.Size(48, 20);
            this.m_spinVersion.TabIndex = 31;
            this.m_spinVersion.Text = "A";
            this.m_spinVersion.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // m_lblExtension
            // 
            this.m_lblExtension.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.m_lblExtension.Location = new System.Drawing.Point(392, 160);
            this.m_lblExtension.Name = "m_lblExtension";
            this.m_lblExtension.Size = new System.Drawing.Size(48, 21);
            this.m_lblExtension.TabIndex = 33;
            this.m_lblExtension.Text = ".db";
            this.m_lblExtension.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // m_lblLanguageAbbrev
            // 
            this.m_lblLanguageAbbrev.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.m_lblLanguageAbbrev.Location = new System.Drawing.Point(24, 160);
            this.m_lblLanguageAbbrev.Name = "m_lblLanguageAbbrev";
            this.m_lblLanguageAbbrev.Size = new System.Drawing.Size(64, 21);
            this.m_lblLanguageAbbrev.TabIndex = 34;
            this.m_lblLanguageAbbrev.Text = "Lan";
            this.m_lblLanguageAbbrev.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // m_labelLanguageAbbrev
            // 
            this.m_labelLanguageAbbrev.Location = new System.Drawing.Point(24, 144);
            this.m_labelLanguageAbbrev.Name = "m_labelLanguageAbbrev";
            this.m_labelLanguageAbbrev.Size = new System.Drawing.Size(72, 16);
            this.m_labelLanguageAbbrev.TabIndex = 36;
            this.m_labelLanguageAbbrev.Text = "Language:";
            // 
            // m_labelBookAbbrev
            // 
            this.m_labelBookAbbrev.Location = new System.Drawing.Point(86, 16);
            this.m_labelBookAbbrev.Name = "m_labelBookAbbrev";
            this.m_labelBookAbbrev.Size = new System.Drawing.Size(94, 16);
            this.m_labelBookAbbrev.TabIndex = 37;
            this.m_labelBookAbbrev.Text = "Book:";
            // 
            // m_labelStage
            // 
            this.m_labelStage.Location = new System.Drawing.Point(186, 16);
            this.m_labelStage.Name = "m_labelStage";
            this.m_labelStage.Size = new System.Drawing.Size(132, 16);
            this.m_labelStage.TabIndex = 38;
            this.m_labelStage.Text = "Stage:";
            // 
            // m_labelVersion
            // 
            this.m_labelVersion.Location = new System.Drawing.Point(327, 16);
            this.m_labelVersion.Name = "m_labelVersion";
            this.m_labelVersion.Size = new System.Drawing.Size(70, 16);
            this.m_labelVersion.TabIndex = 39;
            this.m_labelVersion.Text = "Version:";
            // 
            // m_boxFileName
            // 
            this.m_boxFileName.Controls.Add(this.m_spinVersion);
            this.m_boxFileName.Controls.Add(this.m_labelVersion);
            this.m_boxFileName.Controls.Add(this.m_comboStage);
            this.m_boxFileName.Controls.Add(this.m_labelStage);
            this.m_boxFileName.Controls.Add(this.m_labelBookAbbrev);
            this.m_boxFileName.Controls.Add(this.m_lblBookAbbrev);
            this.m_boxFileName.Location = new System.Drawing.Point(8, 128);
            this.m_boxFileName.Name = "m_boxFileName";
            this.m_boxFileName.Size = new System.Drawing.Size(440, 64);
            this.m_boxFileName.TabIndex = 41;
            this.m_boxFileName.TabStop = false;
            this.m_boxFileName.Text = "File Name";
            // 
            // m_lblBookAbbrev
            // 
            this.m_lblBookAbbrev.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.m_lblBookAbbrev.Location = new System.Drawing.Point(89, 32);
            this.m_lblBookAbbrev.Name = "m_lblBookAbbrev";
            this.m_lblBookAbbrev.Size = new System.Drawing.Size(91, 21);
            this.m_lblBookAbbrev.TabIndex = 42;
            this.m_lblBookAbbrev.Text = "BOOK";
            this.m_lblBookAbbrev.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // m_checkLocked
            // 
            this.m_checkLocked.Location = new System.Drawing.Point(8, 224);
            this.m_checkLocked.Name = "m_checkLocked";
            this.m_checkLocked.Size = new System.Drawing.Size(440, 24);
            this.m_checkLocked.TabIndex = 43;
            this.m_checkLocked.Text = "Locked: Editing is not permitted. (\"To Do\" notes can be added.)";
            // 
            // m_lblCopyright
            // 
            this.m_lblCopyright.Location = new System.Drawing.Point(8, 256);
            this.m_lblCopyright.Name = "m_lblCopyright";
            this.m_lblCopyright.Size = new System.Drawing.Size(56, 23);
            this.m_lblCopyright.TabIndex = 44;
            this.m_lblCopyright.Text = "Copyright:";
            this.m_lblCopyright.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // m_editCopyright
            // 
            this.m_editCopyright.Location = new System.Drawing.Point(88, 256);
            this.m_editCopyright.Name = "m_editCopyright";
            this.m_editCopyright.Size = new System.Drawing.Size(360, 20);
            this.m_editCopyright.TabIndex = 45;
            // 
            // DBookProperties
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(458, 440);
            this.Controls.Add(this.m_editCopyright);
            this.Controls.Add(this.m_textComment);
            this.Controls.Add(this.m_lblCopyright);
            this.Controls.Add(this.m_checkLocked);
            this.Controls.Add(this.m_labelLanguageAbbrev);
            this.Controls.Add(this.m_lblLanguageAbbrev);
            this.Controls.Add(this.m_lblExtension);
            this.Controls.Add(this.m_textName);
            this.Controls.Add(this.m_btnCancel);
            this.Controls.Add(this.m_btnBrowseFileName);
            this.Controls.Add(this.m_lblPathName);
            this.Controls.Add(this.m_lblFile);
            this.Controls.Add(this.m_comboAbbreviation);
            this.Controls.Add(this.m_lblAbbrevation);
            this.Controls.Add(this.m_lbName);
            this.Controls.Add(this.m_lblComment);
            this.Controls.Add(this.m_btnHelp);
            this.Controls.Add(this.m_btnOK);
            this.Controls.Add(this.m_boxFileName);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DBookProperties";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Book Properties";
            this.Closing += new System.ComponentModel.CancelEventHandler(this.cmd_OnClosing);
            this.Load += new System.EventHandler(this.cmd_OnLoad);
            this.m_boxFileName.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion
		#region Dialog Controls
		private Button   m_btnHelp;
		private CheckBox m_checkLocked;
		private TextBox  m_textComment;
		private Label    m_lblComment;
		private Button   m_btnOK;
		private Label    m_lbName;
		private Label    m_lblAbbrevation;
		private Label    m_lblFile;
		private Label    m_lblPathName;
		private ComboBox m_comboAbbreviation;
		private Button   m_btnBrowseFileName;
		private Button   m_btnCancel;
		private Label    m_textName;
		private ComboBox m_comboStage;
        private DomainUpDown m_spinVersion;
		private Label    m_lblExtension;
		private Label    m_lblLanguageAbbrev;
		private Label    m_labelLanguageAbbrev;
		private Label    m_labelBookAbbrev;
		private Label    m_labelStage;
        private Label m_labelVersion;
		private GroupBox m_boxFileName;
		private Label    m_lblBookAbbrev;
		#endregion
		#region Method: void InitializeComboBox(string sDefaultBookAbbrev)
		private void InitializeComboBox(string sDefaultBookAbbrev)
			// The combo is only enabled for Creating or Importing a book (as opposed to
			// editing Properties), so we only want to show possibilities for a new
			// book. The different scenarios are commented in the code.
		{
			m_comboAbbreviation.Items.Clear();

			// (1) No front translation. This could be a sibling or reference translation,
			// or it could be the case where the Front hasn't been declared yet. We just
			// list all of the possible books, rather than try to program each possible
			// situation, as it doesn't seem like a frequent-enough event to worry about.
			if (null == TFront)
			{
				foreach( string sBookAbbrev in DBook.BookAbbrevs)
				{
					if (null == Translation.FindBook( sBookAbbrev ))
						m_comboAbbreviation.Items.Add( sBookAbbrev );
				}
			}

			// (2) We are doing the front translation. Show all books which we have not 
			// yet started.
			else if (TFront == Translation)
			{
				if (m_Mode == Mode.kProperties)
				{
					foreach( string sBookAbbrev in DBook.BookAbbrevs)
						m_comboAbbreviation.Items.Add( sBookAbbrev );
				}
				else
				{
					foreach( string sBookAbbrev in DBook.BookAbbrevs)
					{
						if (null == TFront.FindBook( sBookAbbrev ))
							m_comboAbbreviation.Items.Add( sBookAbbrev );
					}
				}
			}

			// (3) We are doing a Target translation. Show all books in the Front which
			// have not been started in the target. If this results in no books (the 
			// target has translated everything that exists in the front), we disable the
			// combo.
			else
			{
				foreach( DBook b in TFront.Books)
				{
					if (null == Translation.FindBook( b.BookAbbrev ))
						m_comboAbbreviation.Items.Add( b.BookAbbrev );
				}
				if (0 == m_comboAbbreviation.Items.Count)
					m_comboAbbreviation.Enabled = false;
			}

			// For the Text, we want to show the default. Thus we must make sure
			// the default is one of the possibilities.
			if (-1 == m_comboAbbreviation.Items.IndexOf(sDefaultBookAbbrev))
				m_comboAbbreviation.Items.Add( sDefaultBookAbbrev );
			m_comboAbbreviation.Text = sDefaultBookAbbrev;
		}
		#endregion

		// Command Handlers ------------------------------------------------------------------
		#region Cmd: cmd_OnLoad(...) - the dialog is loading; initialize the control contents
		private void cmd_OnLoad(object sender, System.EventArgs e)
		{
			// Label text in the appropriate language
			m_lblAbbrevation.Text      = DlgBookPropsRes.Abbreviation;
			m_lbName.Text              = DlgBookPropsRes.BookName;
			m_lblFile.Text             = DlgBookPropsRes.Folder;
			m_btnBrowseFileName.Text   = DlgBookPropsRes.Browse;
			m_boxFileName.Text         = DlgBookPropsRes.FileName;
			m_labelLanguageAbbrev.Text = DlgBookPropsRes.LanguageAbbrev;
			m_labelBookAbbrev.Text     = DlgBookPropsRes.Book;
			m_labelStage.Text          = DlgBookPropsRes.Stage;
			m_labelVersion.Text        = DlgBookPropsRes.Version;
			m_checkLocked.Text         = DlgBookPropsRes.Locked;
			m_lblComment.Text          = DlgBookPropsRes.Comment;
			m_lblCopyright.Text        = DlgBookPropsRes.Copyright;
			m_btnOK.Text               = DlgBookPropsRes.OK;
			m_btnCancel.Text           = DlgBookPropsRes.Cancel;
			m_btnHelp.Text             = DlgBookPropsRes.Help;

			// Changes depending upon the mode (Create / Import / Properties)
			switch (m_Mode)
			{
				case Mode.kCreate:
					m_comboAbbreviation.Enabled = true;
					m_btnBrowseFileName.Visible = true;
					m_checkLocked.Enabled = false;
					m_lblFile.Text  = DlgBookPropsRes.Folder;
                    if (Book.AbsolutePathName.Length > 0)
                        BookPath = Path.GetDirectoryName(Book.AbsolutePathName);
					break;

				case Mode.kProperties:
					// We don't allow much to be changed; so lots gets disabled.
					m_comboAbbreviation.Enabled = false;
					m_btnBrowseFileName.Visible = false;
					m_lblPathName.Size = m_comboAbbreviation.Size;
					m_lblFile.Text  = DlgBookPropsRes.Folder;
                    if (Book.AbsolutePathName.Length > 0)
                        BookPath = Path.GetDirectoryName(Book.AbsolutePathName);
					break;
			}

			// File Name Parts
			DTeamSettings ts = G.TeamSettings;
			m_lblLanguageAbbrev.Text = Translation.LanguageAbbrev;
            m_lblBookAbbrev.Text = G.GetLoc_BookAbbrev(Book.BookAbbrev);
			G.TranslationStages.PopulateCombo(m_comboStage);
			Stage   = Book.TranslationStage.Abbrev;
			Version = Book.Version;

			// Book Name
			m_textName.Font = new Font("Arial", 10, FontStyle.Bold);
			m_textName.Text = Book.DisplayName;
			UpdateTitleBar();

			// Book Abbreviation
			InitializeComboBox(Book.BookAbbrev);

			// IsLocked?
			m_checkLocked.Checked = Book.Locked;

			// Comment
			m_textComment.Text = Book.Comment;

			// Copyright
			m_editCopyright.Text = Book.Copyright;

			// Select and set focus on the Name
			SetFocusOnName();
		}
		#endregion

		#region Cmd: cmd_btnBrowse_clicked(...) - user wants to browse for a filename for the book
		private void cmd_btnBrowse_clicked(object sender, System.EventArgs e)
		{
			// We are creating a file, and thus all we need is the folder
			// to put it in (the filename is generated from the Stage-Version-Minor
			// settings.
			FolderBrowserDialog dlgFolder = new FolderBrowserDialog();
			dlgFolder.Description = DlgBookPropsRes.BrowseFolderDescription;

			// Default to the Data Root folder; if we've browsed before, go to that
			// folder we last browsed to.
			dlgFolder.RootFolder = Environment.SpecialFolder.MyComputer;
			dlgFolder.SelectedPath = G.BrowseDirectory;

			if (DialogResult.OK == dlgFolder.ShowDialog())
			{
				BookPath = dlgFolder.SelectedPath;
				G.BrowseDirectory =  dlgFolder.SelectedPath;
			}
		}
		#endregion
		#region Cmd: cmd_OnClosing(...) - validate, and stay open if there's a problem
		private void cmd_OnClosing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			// We're only interested in further processing if the user has hit the OK
			// button, signaling he is done and wishes to save his results.
			if (DialogResult != DialogResult.OK)
				return;

			// Make sure whatever the use has entered is acceptable (various tests)
			if (false == ValidateData())
			{
				e.Cancel = true;
				return;
			}

			// Determine the file name. In some cases, we want to be careful that we
			// don't overwrite an existing file. It depends on the Mode.
			//
			// Regarding getting the pathname, we use the "long" version of the method
			// call rather than the DBook's method because the book is not yet owned
			// by the translation if this has been invoked via the Create button.

			// For Create Mode, all we want to do is to check if the proposed file name
			// already exists, and if they therefore want to overwrite it.
            if (m_Mode == Mode.kCreate)
			{
                // We can't use book.IsTargetBook, because it isn't owned at this point.
                bool bIsTargetBook = (Translation == G.Project.TargetTranslation);

				string sPathName = DBook.ComputePathName(Translation.LanguageAbbrev,
					m_comboAbbreviation.Text, Stage, Version, BookPath,
                    bIsTargetBook);

				// Check to see if we're going to overwrite an existing file
				if (File.Exists(sPathName))
				{
                    if (!Messages.ConfirmFileOverwrite(sPathName))
					{
						e.Cancel = true;
						return;
					}
				}

                Book.AbsolutePathName = sPathName;
			}

			// For Properties Mode, we have to be most careful, because the user may
			// not have changed the filename; and we don't want to frighten him with
			// a You're Gonna Overwrite Your File! message.
			else if (m_Mode == Mode.kProperties)
			{
				// Get the pathname on entering the dialog
				string sPathNameI = DBook.ComputePathName(Translation.LanguageAbbrev,
					Book.BookAbbrev, Book.TranslationStage.Abbrev, Book.Version, BookPath,
                    Book.IsTargetBook);

				// Get the pathname as a result of user actions in the dialog
				string sPathNameF = DBook.ComputePathName(Translation.LanguageAbbrev,
					m_comboAbbreviation.Text, Stage, Version, BookPath,
                    Book.IsTargetBook);

				// If the user has asked for a different file name, and if the new
				// file name exists, then ask the user if he really wants to overwrite
				// the existing file. Abort if  he asks.
				if (sPathNameI != sPathNameF && File.Exists(sPathNameF))
				{
                    if (!Messages.ConfirmFileOverwrite(sPathNameF))
					{
						e.Cancel = true;
						return;
					}
				}

                Book.AbsolutePathName = sPathNameF;
			}

			// If we've made it this far, then we are happy to accept all the changes.
			// So set the attributes to the main DBook object.
			Book.DisplayName = BookName;
			Book.BookAbbrev  = m_comboAbbreviation.Text;
			Book.Locked      = m_checkLocked.Checked;
			Book.Comment     = m_textComment.Text;
			Book.Copyright   = m_editCopyright.Text;
			Book.TranslationStage = G.TranslationStages.GetFromAbbrev( Stage );
			Book.Version     = Version;
            Book.DeclareDirty();
		}
		#endregion
		#region Cmd: cmd_textName_TextChanged(...) - when Name is edited, update the title bar
		private void cmd_textName_TextChanged(object sender, System.EventArgs e)
		{
			UpdateTitleBar();
		}
		#endregion
		#region Cmd: cmdOnComboSelection(...)
		private void cmdOnComboSelection(object sender, System.EventArgs e)
		{
			// Get the selection
			string sAbbrev = m_comboAbbreviation.Text;

			// Look up the corresponding book name
			int iBook = DBook.FindBookAbbrevIndex(sAbbrev);
			if (-1 == iBook)
				return;
			BookName = Translation.BookNamesTable[iBook];

			// Update the filename information
            m_lblBookAbbrev.Text = G.GetLoc_BookAbbrev(sAbbrev);
		}
		#endregion
		#region Cmd: cmdOnStageChanged(...)
		private void cmdOnStageChanged(object sender, System.EventArgs e)
		{
			Version = 'A';
		}
		#endregion
		#region Cmd: cmdHelp(...) - Help button clicked
		private void cmdHelp(object sender, System.EventArgs e)
		{
			HelpSystem.Show_DlgBookProperties();
		}
		#endregion

		// Special Test Support --------------------------------------------------------------
		#region Method: bool Test_ComboOffersAbbrev(sAbbrev) - see if combo has, e.g., "GEN" as a choice
		public bool Test_ComboOffersAbbrev(string sAbbrev)
		{
			foreach (string s in m_comboAbbreviation.Items)
			{
				if (s == sAbbrev)
					return true;
			}
			return false;
		}
		#endregion
	}
	#endregion

	public class DBook : JObjectOnDemand
	{
		// BAttrs ----------------------------------------------------------------------------
		#region BAttr{g/s}: string BookAbbrev - a 3-letter abbreviation for the book
		public string BookAbbrev
		{
			get
			{
				return m_sBookAbbrev;
			}
			set
			{
                SetValue(ref m_sBookAbbrev, value);
			}
		}
		private string m_sBookAbbrev = "";
		#endregion
		#region BAttr{g/s}: string ID - an ID for the file / book
		public string ID
		{
			get
			{
                // Each book should have an ID line, so default to the book's abbrev
                if (string.IsNullOrEmpty(m_sID))
                    m_sID = BookAbbrev;

				return m_sID;
			}
			set
			{
                SetValue(ref m_sID, value);
			}
		}
		private string m_sID = "";
		#endregion
		#region BAttr{g/s}: string Comment - a miscellaneous comment about the book
		public string Comment
		{
			get
			{
				return m_sComment;
			}
			set
			{
                SetValue(ref m_sComment, value);
			}
		}
		private string m_sComment = "";
		#endregion
		#region BAttr{g/s} TranslationStage TranslationStage
		public TranslationStage TranslationStage
		{
			get
			{
				BookStages Stages = G.TeamSettings.TranslationStages;
				TranslationStage ts = Stages.GetFromID(m_nTranslationStage);
				if (null == ts)
					ts = Stages.GetFromIndex(0);
				Debug.Assert(null != ts);
				return ts;
			}
			set
			{
				Debug.Assert(null != value);
                SetValue(ref m_nTranslationStage, value.ID);
			}
		}
		private int m_nTranslationStage = BookStages.c_idDraft;
		#endregion
		#region BAttr{g/s}: char Version - incremental version, 'A', 'B', etc.
		public char Version
		{
			get
			{
				return m_chVersion;
			}
			set
			{
                SetValue(ref m_chVersion, value);
			}
		}
		private char m_chVersion = 'A';
		#endregion
		#region BAttr{g/s}: bool Locked - T if user is not allowed to edit.
		public bool Locked
		{
			get
			{
				return m_bLocked;
			}
			set
			{
                SetValue(ref m_bLocked, value);
			}
		}
		private bool m_bLocked = false;
		#endregion
		#region BAttr{g/s}: string Copyright - Available to print in the footnote
		public string Copyright
		{
			get
			{
				return m_sCopyright;
			}
			set
			{
                SetValue(ref m_sCopyright, value);
			}
		}
		private string m_sCopyright = "";
		#endregion
		#region Method: void DeclareAttrs()
		protected override void DeclareAttrs()
		{
			base.DeclareAttrs();
			DefineAttr("Abbrev",    ref m_sBookAbbrev);
			DefineAttr("ID",        ref m_sID);
			DefineAttr("Comment",   ref m_sComment);
			DefineAttr("Stage",     ref m_nTranslationStage);
			DefineAttr("Version",   ref m_chVersion);
			DefineAttr("Locked",    ref m_bLocked);
			DefineAttr("Copyright", ref m_sCopyright);
		}
		#endregion

		// JAttrs ----------------------------------------------------------------------------
		#region JAttr{g}: JOwnSeq Sections - the list of sections in this book
		public JOwnSeq Sections
		{
			get 
			{ 
				return j_osSections; 
			}
		}
		private JOwnSeq j_osSections = null;
		#endregion
		#region JAttr{g}: JOwnSeq History - seq of paragraphs given notes about translation history
		public JOwnSeq History
		{
			get 
			{ 
				return j_osHistory; 
			}
		}
		private JOwnSeq j_osHistory = null;
		#endregion
		#region JAttr{g}: JOwnSeq Notes - seq of paragraphs given notes about misc notes
		public JOwnSeq Notes
		{
			get 
			{ 
				return j_osNotes; 
			}
		}
		private JOwnSeq j_osNotes = null;
		#endregion

		// Temporary (run-time) attrs --------------------------------------------------------
		#region Attr{g/s}: bool UserHasSeenLockedMessage - T if Info_BookIsLocked has been shown
		public bool UserHasSeenLockedMessage
		{
			get
			{
				return m_bUserHasSeenLockedMessage;
			}
			set
			{
				m_bUserHasSeenLockedMessage = value;
			}
		}
		bool m_bUserHasSeenLockedMessage = false;
		#endregion

        // Merge Support ---------------------------------------------------------------------
        #region CLASS MergeDiff
        public class MergeDiff
        {
            #region Attr{g}: int SectionNo
            public int SectionNo
            {
                get
                {
                    return m_nSection;
                }
            }
            int m_nSection;
            #endregion
            #region Attr{g}: int ParagraphNo
            public int ParagraphNo
            {
                get
                {
                    return m_nParagraph;
                }
            }
            int m_nParagraph;
            #endregion
            #region Attr{g}: int DBTNo
            public int DBTNo
            {
                get
                {
                    return m_nDBT;
                }
            }
            int m_nDBT;
            #endregion

            #region Constructor(nSection, nParagraph, nDBT)
            public MergeDiff(int nSection, int nParagraph, int nDBT)
            {
                m_nSection = nSection;
                m_nParagraph = nParagraph;
                m_nDBT = nDBT;
            }
            #endregion

            #region I/O
            public const string c_sTag = "Diff";
            const string c_sSection = "Section";
            const string c_sPara = "Para";
            const string c_sDBT = "DBT";

            #region Method: void SaveXML(XmlField xmlParent)
            public void SaveXML(XmlField xmlParent)
            {
                string s = xmlParent.GetAttrString(c_sSection, SectionNo);
                s += xmlParent.GetAttrString(c_sPara, ParagraphNo);
                s += xmlParent.GetAttrString(c_sDBT, DBTNo);
                XmlField xml = xmlParent.GetDaughterXmlField(c_sTag, true);
                xml.OneLiner(s, "");
            }
            #endregion
            #region SMethod: MergeDiff ReadXML(XmlRead xml)
            static public MergeDiff ReadXML(XmlRead xml)
            {
                string sSection = xml.GetValue(c_sSection);
                string sPara = xml.GetValue(c_sPara);
                string sDBT = xml.GetValue(c_sDBT);

                try
                {
                    int nSection = Convert.ToInt16(sSection);
                    int nPara = Convert.ToInt16(sPara);
                    int nDBT = Convert.ToInt16(sDBT);

                    return new MergeDiff(nSection, nPara, nDBT);
                }
                catch (Exception)
                {
                }
                return null;
            }
            #endregion
            #endregion
        }
        #endregion
        #region Attr{g}: MergeDiff[] MergeDiffs
        public MergeDiff[] MergeDiffs
        {
            get
            {
                Debug.Assert(null != m_vMergeDiffs);
                return m_vMergeDiffs;
            }
        }
        MergeDiff[] m_vMergeDiffs;
        #endregion
        #region Method: void AddMergeDiff(MergeDiff diff)
        public void AddMergeDiff(MergeDiff diff)
        {
            MergeDiff[] v = new MergeDiff[MergeDiffs.Length + 1];
            for (int i = 0; i < MergeDiffs.Length; i++)
                v[i] = MergeDiffs[i];
            v[MergeDiffs.Length] = diff;
            m_vMergeDiffs = v;
        }
        #endregion
        #region Method: void ClearMergeDiffs()
        public void ClearMergeDiffs()
        {
            m_vMergeDiffs = new MergeDiff[0];
        }
        #endregion
        #region Method: int IsInMergeDiffs(int nSection, int nPara, int nDBT)
        public int IsInMergeDiffs(int nSection, int nPara, int nDBT)
        {
            for(int i=0; i<MergeDiffs.Length; i++)
            {
                if (MergeDiffs[i].SectionNo == nSection &&
                    MergeDiffs[i].ParagraphNo == nPara &&
                    MergeDiffs[i].DBTNo == nDBT)
                {
                    return i;
                }
            }
            return -1;
        }
        #endregion

        #region Attr{g}: DMergeBook[] MergeBooks
        public DMergeBook[] MergeBooks
        {
            get
            {
                Debug.Assert(null != m_vMergeBooks);
                return m_vMergeBooks;
            }
        }
        DMergeBook[] m_vMergeBooks;
        #endregion
        #region Method: void AddMergeBook(DMergeBook book)
        public void AddMergeBook(DMergeBook book)
        {
            DMergeBook[] v = new DMergeBook[MergeBooks.Length + 1];
            for (int i = 0; i < MergeBooks.Length; i++)
                v[i] = MergeBooks[i];
            v[MergeBooks.Length] = book;
            m_vMergeBooks = v;
        }
        #endregion
        #region Method: void RemoveMergeBook(DMergeBook book)
        public void RemoveMergeBook(DMergeBook book)
        {
            Debug.Assert(MergeBooks.Length > 0);
            DMergeBook[] v = new DMergeBook[MergeBooks.Length - 1];

            int k = 0;
            foreach (DMergeBook b in MergeBooks)
            {
                if (b != book)
                    v[k++] = b;
            }

            m_vMergeBooks = v;
        }
        #endregion
        #region Method: void ClearMergeBooks()
        public void ClearMergeBooks()
        {
            foreach (DMergeBook book in MergeBooks)
                book.Unload();

            m_vMergeBooks = new DMergeBook[0];

            ClearMergeDiffs();
        }
        #endregion
        #region Method: void BuildMergeDiffs()
        public void BuildMergeDiffs()
        {
            G.SetWaitCursor();

            ClearMergeDiffs();

            for (int iSection = 0; iSection < Sections.Count; iSection++)
            {
                // Retrieve the Master section
                DSection Section = Sections[iSection] as DSection;

                // Retrieve all of the Merge sections
                DSection[] vSections = new DSection[MergeBooks.Length];
                for (int k = 0; k < MergeBooks.Length; k++)
                    vSections[k] = MergeBooks[k].Sections[iSection] as DSection;

                // Loop to compare the paragraphs
                for (int iPara = 0; iPara < Section.Paragraphs.Count; iPara++)
                {
                    // Retrieve the Master paragraph
                    DParagraph Paragraph = Section.Paragraphs[iPara] as DParagraph;

                    // Retrieve all of the Merge paragraphs
                    DParagraph[] vParas = new DParagraph[MergeBooks.Length];
                    for (int k = 0; k < MergeBooks.Length; k++)
                        vParas[k] = vSections[k].Paragraphs[iPara] as DParagraph;

                    // Loop to compare the DBTs
                    for (int iDBT = 0; iDBT < Paragraph.Runs.Count; iDBT++)
                    {
                        // Retrieve the master DBT
                        DBasicText DBT = Paragraph.Runs[iDBT] as DBasicText;
                        if (null == DBT)
                            continue;

                        // Compare the Merge DBTs
                        foreach (DParagraph p in vParas)
                        {
                            p.SynchRunsToModelParagraph(Paragraph);
                            DBasicText dbtMerge = p.Runs[iDBT] as DBasicText;
                            if (dbtMerge.AsString != DBT.AsString)
                            {
                                MergeDiff diff = new MergeDiff(iSection, iPara, iDBT);
                                AddMergeDiff(diff);
                                break;
                            }
                        }

                    } // endloop DBT
                } // endloop Para
            } // endloop Section

            G.SetNormalCursor();
        }
        #endregion

        #region MERGE I/O
        #region VAttr{g}: string MergeSettingsFileName - "*.aux" for "auxiliary"
        public string MergeSettingsFileName
        {
            get
            {
                return Path.ChangeExtension(AbsolutePathName, ".aux");
            }
        }
        #endregion
        const string c_xmlFile = "MergeBooks";
        const string c_xmlBook = "Book";
        const string c_xmlNickName = "NickName";
        const string c_xmlPath = "Path";
        #region Method: void SaveAuxiliarySettings()
        public void SaveAuxiliarySettings()
        {
            // If there are no Merge Files, then we don't want to store a settings file
            if (MergeBooks.Length == 0)
            {
                if (File.Exists(MergeSettingsFileName))
                    File.Delete(MergeSettingsFileName);
                return;
            }

            // Write the settings out
            TextWriter writer = JW_Util.GetTextWriter(MergeSettingsFileName);
            XmlField xmlParent = new XmlField(writer, c_xmlFile);
            xmlParent.Begin();
            foreach (DMergeBook book in MergeBooks)
            {
                string s = xmlParent.GetAttrString(c_xmlNickName, book.NickName);
                s += xmlParent.GetAttrString(c_xmlPath, book.AbsolutePathName);
                XmlField xml = xmlParent.GetDaughterXmlField(c_xmlBook, true);
                xml.OneLiner(s, "");
            }
            foreach (MergeDiff diff in MergeDiffs)
                diff.SaveXML(xmlParent);
            xmlParent.End();
            writer.Close();
        }
        #endregion
        #region Method: void ReadAuxiliarySettings()
        public void ReadAuxiliarySettings()
        {
            // Start with a blank slate
            ClearMergeBooks();

            // Nothing to do if there is no settings file
            if (!File.Exists(MergeSettingsFileName))
                return;

            // Read the file and populate books
            string sPathName = MergeSettingsFileName;
            TextReader reader = JW_Util.GetTextReader(ref sPathName, "*.xml");
            XmlRead xml = new XmlRead(reader);
            while (xml.ReadNextLineUntilEndTag(c_xmlFile))
            {
                if (xml.IsTag(c_xmlBook))
                {
                    string sNickName = xml.GetValue(c_xmlNickName);
                    string sPath = xml.GetValue(c_xmlPath);
                    DMergeBook book = new DMergeBook(this, sPath, sNickName);
                    AddMergeBook(book);
                }

                if (xml.IsTag(MergeDiff.c_sTag))
                {
                    MergeDiff diff = MergeDiff.ReadXML(xml);
                    AddMergeDiff(diff);
                }
            }
            reader.Close();
        }
        #endregion
        #endregion

        // Derived Attributes: ---------------------------------------------------------------
        #region Attr{g}: DTranslation Translation - returns the translation that owns this book
        virtual public DTranslation Translation 
		{
			get
			{
				DTranslation translation = (DTranslation)Owner;
				Debug.Assert(null != translation);
				return translation;
			}
		}
		#endregion
		#region Attr{g}: DProject Project - returns the Project that owns this book
		public DProject Project
		{
			get 
			{ 
				Debug.Assert(null != Translation.Project);
				return Translation.Project; 
			}
		}
		#endregion
		#region Attr(g): string SortKey - overridden to enable JOwnSeq Find method support.
		public override string SortKey
			// In order to support sorting, the subclass must implement a SortKey attribute,
			// and this SortKey must return something other than an empty string. 
		{
			get 
			{
                // Return this as two digits, so that, e.g., LEV is 02 and thus sorts
                // properly after Exodus rather than after Proverbs.
				return BookNumber.ToString("00"); 
			}
		}
		#endregion
		#region Attr{g}: int BookNumber - e.g., Genesis = 1, Exodus = 2, etc.
		public int BookNumber
		{
			get
			{
				return FindBookAbbrevIndex(BookAbbrev);
			}
		}
		#endregion
		#region Attr{g}: public DSFMapping Map - used only during SF Read operation
		// This object allows us to map from the read.Marker to the way to handle each
		// field, e.g., which one is a Section Title, which one is a back translation,
		// etc.
		public DSFMapping Map
		{
			get
			{
				return Project.TeamSettings.SFMapping;
			}
		}
		#endregion
		#region Attr{g}: DBook FrontBook - the corresponding book in the Front translation
		public DBook FrontBook
			// We cannot assume that the FrontBook is the currently-loaded book,
			// and thus use the Current Front Section to get it, because we may be
			// working with a book which is not the current one; thus we do a lookup
			// instead. We use m_FrontBookSaved to remember the lookup, so that we don't
			// have to repeat the lookup over and over.
			//    We also make sure the book as been loaded.
		{
			get
			{
				if (null != m_FrontBookSaved)
					return m_FrontBookSaved;

				if (null == Project.FrontTranslation)
					return null;

				foreach( DBook b in Project.FrontTranslation.Books)
				{
					if (b.BookAbbrev == this.BookAbbrev)
					{
						m_FrontBookSaved = b;
                        m_FrontBookSaved.Load();
						return b;
					}
				}

				return null;
			}
		}
		private DBook m_FrontBookSaved = null;
		#endregion
		#region Attr{g}: AllSectionsMatchFront - determined by Read, T if a true daughter
		public bool AllSectionsMatchFront
		{
			get
			{
				// Optimization; we only need do this test once per book
				if (m_AllSectionsMatchFrontTestDone)
					return m_AllSectionsMatchFront;
				m_AllSectionsMatchFrontTestDone = true;

				// If the section counts are not the same, then we know they are different.
				if (FrontBook.Sections.Count != this.Sections.Count)
					return false;

				// If "this" is the front, then by definition we are the same.
				if (FrontBook == this)
				{
					m_AllSectionsMatchFront = true;
					return true;
				}

				// Loop through each section, testing for identical references
				for(int i = 0; i < Sections.Count; i++)
				{
					DSection SFront = (DSection)FrontBook.Sections[i];
					DSection SThis  = (DSection)Sections[i];

					if (!SFront.ReferenceSpan.ContentEquals(SThis.ReferenceSpan))
						return false;
				}

				// If we got this far, all sections have identical references; thus
				// this book's section matches the Front.
				m_AllSectionsMatchFront = true;
				return true;
			}
		}
		private bool m_AllSectionsMatchFront = false;
		private bool m_AllSectionsMatchFrontTestDone = false;
		#endregion
		#region Attr{g}: bool IsFrontBook - T if this book is part of the Front translation
		bool IsFrontBook
		{
			get
			{
				if (Translation == Project.FrontTranslation)
					return true;
				return false;
			}
		}
		#endregion
        #region Attr{g}: bool IsTargetBook - T if this book is part of the Target translation
        public bool IsTargetBook
        {
            get
            {
                if (Translation == Project.TargetTranslation)
                    return true;
                return false;
            }
        }
        #endregion
        #region Attr{g}: string SfRecordKey - The data for the book's first \rcrd field
		string SfRecordKey
		{
			get
			{
				string s = "";

				// If this is the Front, then we want whatever we have stored. Otherwise,
				// we pass the request on to the Front
				if (IsFrontBook)
					s = m_sSfRecordKey;
				else if (null != FrontBook)
					s = FrontBook.SfRecordKey;

				// If we get this far and still have nothing, then make up something!
				if (s.Length == 0)
					s = BookAbbrev;

				return s;
			}
			set
			{
				if (IsFrontBook)
					m_sSfRecordKey = value;
			}
		}
		string m_sSfRecordKey = "";
		#endregion
		#region Attr{g}: int FinalChapterNo - the largest chapter no currently in this book
		public int FinalChapterNo
		{
			get
			{
				int n = 1;
				foreach(DSection section in Sections)
				{
					n = Math.Max(n, section.ReferenceSpan.End.Chapter);
				}
				return n;
			}
		}
		#endregion
		#region Attr{g}: bool HasCopyrightNotice - T if the user has defined a string
		public bool HasCopyrightNotice
		{
			get
			{
				if (Copyright != null && Copyright.Length > 0)
					return true;
				return false;
			}
		}
		#endregion

		// BookAbbrev-To-BookNumber Conversions ----------------------------------------------
		public const int BookAbbrevsCount = 66;
		#region SAttr{g} string[] BookAbbrevs - e.g., "GEN", "EXO", "LEV", etc.
		static public string[] BookAbbrevs = { "GEN", "EXO", "LEV", "NUM", "DEU", "JOS",
			"JDG", "RUT", "1SA", "2SA", "1KI", "2KI", "1CH", "2CH", "EZR", "NEH",
			"EST", "JOB", "PSA", "PRO", "ECC", "SNG", "ISA", "JER", "LAM", "EZK",
			"DAN", "HOS", "JOL", "AMO", "OBA", "JON", "MIC", "NAM", "HAB", "ZEP",
			"HAG", "ZEC", "MAL", "MAT", "MRK", "LUK", "JHN", "ACT", "ROM", "1CO",
			"2CO", "GAL", "EPH", "PHP", "COL", "1TH", "2TH", "1TI", "2TI", "TIT",
			"PHM", "HEB", "JAS", "1PE", "2PE", "1JN", "2JN", "3JN", "JUD", "REV" } ;
		#endregion
		#region SMethod: static int FindBookAbbrevIndex(string sAbbrev)
		public static int FindBookAbbrevIndex(string sAbbrev)
		{
			for(int i=0; i < BookAbbrevs.Length; i++)
			{
				if (BookAbbrevs[i] == sAbbrev)
					return i;
			}
			return -1;
		}
		#endregion
		#region Attr{g}: int BookIndex
		public int BookIndex
		{
			get
			{
				return FindBookAbbrevIndex( BookAbbrev );
			}
		}
		#endregion
        #region VAttr{g}: string BookName - looks up the book's name from the table
        public string BookName
        {
            get
            {
                int i = BookIndex;

                if (i >= 0 && i < Translation.BookNamesTable.Length)
                    return Translation.BookNamesTable[i];

                return "";
            }
        }
        #endregion

        // Scaffolding -----------------------------------------------------------------------
		#region Constructor() - only for reading from xml
		public DBook()
			: base()
		{
			ConstructAttrs();
		}
		#endregion
		#region Constructor(sBookAbbrev, sPath)
		public DBook(string sBookAbbrev, string sPath)
			: base()
		{
			// Initialize the attributes
			ConstructAttrs();

			// Initial values for some attrs
			BookAbbrev = sBookAbbrev;
            AbsolutePathName = sPath;
		}
		#endregion
		#region Method: void ConstructAttrs() - constructs the JObject's attributes
		private void ConstructAttrs()
		{
			// Owning Sequence Attrs
			j_osSections   = new JOwnSeq("Sections",  this, typeof(DSection), true, false);
			j_osHistory    = new JOwnSeq("History",   this, typeof(JParagraph), false, false);
			j_osNotes      = new JOwnSeq("Notes",     this, typeof(JParagraph), false, false);
            m_vMergeBooks = new DMergeBook[0];
		}
		#endregion
		#region Method: override bool ContentEquals(obj) - required override to prevent duplicates
		public override bool ContentEquals(JObject obj)
		{
			if (this.GetType() != obj.GetType())
				return false;

			DBook book = (DBook)obj;
			return book.BookAbbrev == this.BookAbbrev;
		}
		#endregion
        #region VAttr{g}: override string DefaultFileExtension
        public override string DefaultFileExtension
        {
            get
            {
                return ".db";
            }
        }
        #endregion

		// Methods ---------------------------------------------------------------------------
		#region Method: DialogResult EditProperties(translation, bImporting)
		public DialogResult EditProperties(DTranslation TFront,
			DTranslation translation, DBookProperties.Mode mode)
		{
			DBookProperties dlg = new DBookProperties(TFront,
				translation, this, mode);
			return dlg.ShowDialog();
		}
		#endregion
		#region Method: void InitializeFromFrontTranslation()
		public bool InitializeFromFrontTranslation()
		{
			if (Sections.Count > 0)
				return false;
			if (null == FrontBook)
				return false;

            FrontBook.Load();
			if (!FrontBook.Loaded)
				return false;

			int i = 1;
			foreach(DSection sfront in FrontBook.Sections)
			{
				DSection starget = new DSection(i++);
				Sections.Append(starget);
				starget.InitializeFromFrontSection(sfront);
			}

            // Set the ID line
            ID = BookAbbrev;
            ID += " - ";
            ID += Translation.DisplayName;
            if (Translation.LanguageAbbrev.Length > 0)
                ID += (" (" + Translation.LanguageAbbrev + ")");

			m_bIsLoaded = true;
            DeclareDirty();
			return true;
		}
		#endregion
		#region Method: void UpdateFromFront(DBook BFront)
		public void UpdateFromFront(DBook BFront)
		{
			// If we don't have a Front book, or if we are not the TargetTranslation,
			// then we have nothing to do here.
			if (null == BFront || Translation != OurWordMain.Project.TargetTranslation)
				return;

			// If the two books do not have the same section count, then we don't dare
			// proceed.
			if (BFront.Sections.Count != Sections.Count)
				return;

			// Proceed through each DSection
			for(int i=0; i < Sections.Count; i++)
			{
				DSection sTarget = Sections[i] as DSection;
				DSection sFront  = BFront.Sections[i] as DSection;

				sTarget.UpdateFromFront(sFront);
			}
		}
		#endregion

        #region Method: string GetSectionMiscountMsg(SFront, STarget)
        string GetSectionMiscountMsg(DSection SFront, DSection STarget)
        {
            string sDiagnosis = "";

            // Diagnosis where the sections have different verses
            if (null != SFront && null != STarget)
            {
                string sBase = G.GetLoc_StructureMessages( "diagnosis1",
                    "The verses first get off at Section {0}.");

                sDiagnosis = LocDB.Insert(sBase, 
                    new string[] { SFront.ReferenceSpan.Start.FullName });
            }

            // Diagnosis where there there are sections added at the end
            else
            {
                string sBase = G.GetLoc_StructureMessages("diagnosis2",
                    "{0} has extra sections appended at the end.");

                string sName = (null == SFront) ?
                    STarget.Translation.DisplayName :
                    SFront.Translation.DisplayName;

                sDiagnosis = LocDB.Insert(sBase,new string[] { sName  });
            }

            // Retrieve the base string
            string sInner = G.GetLoc_StructureMessages( "msgSectionMiscount",
                "The two books appear to not have the same number of sections.\n" +
                "    {0} has {1}; {2} appears to have {3}.\n" +
                "{4}");

            // Do the insertions
            string sMsg = LocDB.Insert(sInner, new string[] { 
                SFront.Translation.DisplayName,
                SFront.Book.Sections.Count.ToString(),
                STarget.Translation.DisplayName,
                STarget.Book.Sections.Count.ToString(),
                sDiagnosis
            });
            return sMsg;
        }
        #endregion

        #region Method: string GetStructureMismatchMsg(DSection SFront, DSection STarget)
        string GetStructureMismatchMsg(DSection SFront, DSection STarget)
        {
            // Retrieve the Section Stucture localizations
            string sParagraph = G.GetLoc_StructureMessages("paragraphs", "Paragraph(s)");
            string sPicture = G.GetLoc_StructureMessages("picture", "Picture");

            // Build the strings representing the section structures
            string sStructFront = "";
            foreach (char ch in SFront.SectionStructure)
            {
                if (sStructFront.Length > 0)
                    sStructFront += "-";
                if (ch == DSection.c_Text)
                    sStructFront += sParagraph;
                else
                    sStructFront += sPicture;
            }
            string sStructTarget = "";
            foreach (char ch in STarget.SectionStructure)
            {
                if (sStructTarget.Length > 0)
                    sStructTarget += "-";
                if (ch == DSection.c_Text)
                    sStructTarget += sParagraph;
                else
                    sStructTarget += sPicture;
            }

            // Retrieve the message base string
            string sBase = G.GetLoc_StructureMessages("msgStructureMismatch",
                "There is a structure mistmatch in section {0}:\n" +
                "     {1} has {2},\n" +
                "     {3} has {4}.\n\n");

            // Make the substitutions
            string sMsg = LocDB.Insert(sBase, new string[] {
                STarget.ReferenceSpan.DisplayName, 
                SFront.Translation.DisplayName,
                sStructFront,
                STarget.Translation.DisplayName,
                sStructTarget
            });
            return sMsg;
        }
        #endregion

		#region Method: bool CheckSectionStructureAgainstFront(DBook BFront)
		public void CheckSectionStructureAgainstFront(DBook BFront)
		{
			// If we don't have a Front book, or if we are not the TargetTranslation,
			// then we have nothing to do here.
			if (null == BFront || Translation != OurWordMain.Project.TargetTranslation)
				return;

			// Make sure we have the same number of sections
			if (Sections.Count != BFront.Sections.Count)
			{
				// Find the sections where the counts get off
				DSection SFront  = null;
				DSection STarget = null;
				for(int i = 0; i < BFront.Sections.Count && i < Sections.Count; i++)
				{
					SFront  = BFront.Sections[i] as DSection;
					STarget = Sections[i] as DSection;
					if (! SFront.ReferenceSpan.ContentEquals( STarget.ReferenceSpan ) )
						break;
				}

				// Display the error
				throw new eBookReadException(
                    GetSectionMiscountMsg(SFront, STarget),
					HelpSystem.Topic.kErrStructureSectionMiscount,
					SFront.LineNoInFile, 
                    STarget.LineNoInFile
					);
			}

			// Loop through all of the sections, looking for structures that do not match
			for(int i = 0; i < Sections.Count; i++)
			{
				DSection SFront  = BFront.Sections[i] as DSection;
				DSection STarget = Sections[i] as DSection;

				if ( SFront.SectionStructure != STarget.SectionStructure )
				{
					throw new eBookReadException(
						GetStructureMismatchMsg(SFront, STarget),
						HelpSystem.Topic.kErrStructureMismatch,
						SFront.LineNoInFile, 
                        STarget.LineNoInFile
						);
				}
			}

			return;
		}
		#endregion
		#region Method: void CopyBackTranslationFromFront(bool bEntireBook)
		public void CopyBackTranslationFromFront(bool bEntireBook)
		{
			// Display a very intense warning to the user.
			CopyBTfromFront dlg = new CopyBTfromFront(bEntireBook);
			if (DialogResult.OK != dlg.ShowDialog())
				return;

            // Set the default actions to take
            DialogCopyBTConflict.ApplyToAll = false;
            DialogCopyBTConflict.CopyBTAction = DialogCopyBTConflict.Actions.kAppendToTarget;

			// Do the copy for each section in the book
			bool bMismatchErrorShown = false;
			for(int i=0; i<Sections.Count; i++)
			{
				DSection SFront  = G.FBook.Sections[i] as DSection;
				DSection STarget = G.TBook.Sections[i] as DSection;

				// If we're only doing the current section, then skip all other sections
				if (!bEntireBook && STarget != G.STarget)
					continue;

                // Do the copy (displaying the Conflicts Dialog if necessary)
				bool bOK = STarget.CopyBackTranslationsFromFront(SFront);

                // Did the user abort during the Conflicts Dialog?
                if (DialogCopyBTConflict.Actions.kCancel == DialogCopyBTConflict.CopyBTAction)
                    return;

                // We only show the Mismatched Sections dialog once, if needed
				if (!bOK && false == bMismatchErrorShown)
				{
					MessageBox.Show( Form.ActiveForm, 
						"Unable to completely copy the BT's in one (or more) sections because\n" +
						"the structures do not line up exactly. You'll have to do these manually.", 
						"Our Word!", MessageBoxButtons.OK, MessageBoxIcon.Information);
					bMismatchErrorShown = true;
				}
			}
		}
		#endregion

		// Filters ---------------------------------------------------------------------------
		#region Method: void RemoveFilter() - Turn off any active filter
		public void RemoveFilter()
		{
			DSection.FilterIsActive = false;
			foreach(DSection section in Sections)
				section.MatchesFilter = false;
		}
		#endregion
		#region Method: int IndexOfFirstFilterMatch
		public int IndexOfFirstFilterMatch
		{
			get
			{
				if (DSection.FilterIsActive)
				{
					for(int i=0; i<Sections.Count; i++)
					{
						DSection section = Sections[i] as DSection;
						if (section.MatchesFilter)
							return i;
					}
					Debug.Assert(false, "No sections match the filter");
				}
				return 0;
			}
		}
		#endregion
		#region Method: int IndexOfLastFilterMatch
		public int IndexOfLastFilterMatch
		{
			get
			{
				if (DSection.FilterIsActive)
				{
					for(int i=Sections.Count-1; i>=0; i--)
					{
						DSection section = Sections[i] as DSection;
						if (section.MatchesFilter)
							return i;
					}
					Debug.Assert(false, "No sections match the filter");
				}
				return Sections.Count - 1;
			}
		}
		#endregion

		// Read Standard Format --------------------------------------------------------------
		#region Attr{g}: bool IsParatextFormat - T if the SF file is a paratext file
		public bool IsParatextFormat
		{
			get
			{
                if (AbsolutePathName.Length > 4 && AbsolutePathName.ToLower().EndsWith(".ptx"))
					return true;
				return false;
			}
		}
		#endregion

		#region DReference GetDefaultReference(sData)
		private DReference GetDefaultReference(string sData)
		{
			int i = 0;

			// Skip past the book abbreviation
			while( i < sData.Length && sData[i] != ' ')
				++i;
			while( i < sData.Length && sData[i] == ' ')
				++i;
			while( i < sData.Length && sData[i] == '0')
				++i;

			// Get the chapter number
			string sChapter = "";
			while( i < sData.Length && sData[i] != '.')
			{
				sChapter += sData[i];
				++i;
			}
			while( i < sData.Length && sData[i] == '.')
				++i;
			while( i < sData.Length && sData[i] == '0')
				++i;

			// Get the verse number
			string sVerse = "";
			while( i < sData.Length && sData[i] != '-')
			{
				sVerse += sData[i];
				++i;
			}

			// Build the reference
			DReference reference = new DReference();
			if (sChapter.Length > 0)
				reference.Chapter = Convert.ToInt16(sChapter);
			if (sVerse.Length > 0)
				reference.Verse   = Convert.ToInt16(sVerse);
			return reference;
		}
		#endregion

        // I/O (Standard Format) -------------------------------------------------------------
        #region EMBEDDED CLASS IO
        class IO
		{
			// Attrs -------------------------------------------------------------------------
			#region Attr{g}: DBook Book - the book we're reading/writing
			DBook Book
			{
				get
				{
					Debug.Assert(null != m_book);
					return m_book;
				}
			}
			private DBook m_book = null;
			#endregion
			#region Attr{g}: DSFMapping Map
			DSFMapping Map
			{
				get 
				{
					return G.TeamSettings.SFMapping;
				}
			}
			#endregion
			#region Attr{g}: ScriptureDB DB
			ScriptureDB DB
			{
				get
				{
					Debug.Assert(null != m_db);
					return m_db;
				}
				set
				{
					Debug.Assert(null != value);
					m_db = value;
				}
			}
			ScriptureDB m_db = null;
			#endregion

			// Progress Indiccator -----------------------------------------------------------
			#region Method: void StartProgress(string sAction)
			private void StartProgress(string sAction, int nCount)
			{
				if (null == OurWordMain.App)
					return;

				string sMessage = sAction + " " + Book.Translation.DisplayName + 
					":" + Book.DisplayName + "...";

				G.ProgressStart(sMessage, nCount);
			}
			#endregion
			#region Method: void StepProgress()
			void StepProgress()
			{
				G.ProgressStep();
			}
			#endregion
			#region Method: void EndProgress()
			private void EndProgress()
			{
				G.ProgressEnd();
			}
			#endregion

			// ID Field ----------------------------------------------------------------------
			#region Method: bool ID_in(SfField field)
			private bool ID_in(SfField field)
			{
				if (Map.IsFileIdMarker( field.Mkr ))
				{
					Book.ID = field.Data;
					return true;
				}
				return false;
			}
			#endregion
			#region Method: void ID_out()
			private void ID_out()
			{
				if ( Book.ID.Length > 0)
					DB.Append(new SfField(Map.MkrFileId, Book.ID));
			}
			#endregion

			// Comment Field -----------------------------------------------------------------
			#region Method: bool Comment_in(SfField field)
			private bool Comment_in(SfField field)
			{
				if (Map.IsCommentMarker( field.Mkr ))
				{
					Book.Comment += field.Data;
					return true;
				}
				return false;
			}
			#endregion
			#region Method: void Comment_out()
			private void Comment_out()
			{
				if ( Book.Comment.Length > 0 )
					DB.Append( new SfField(Map.MkrComment, Book.Comment) );
			}
			#endregion

			// Copyright Field ---------------------------------------------------------------
			#region Method: bool Copyright_in(SfField field)
			private bool Copyright_in(SfField field)
			{
				if (Map.IsCopyrightMarker( field.Mkr ))
				{
					Book.Copyright = field.Data;
					return true;
				}
				return false;
			}
			#endregion
			#region Method: void Copyright_out()
			private void Copyright_out()
			{
				if ( Book.Copyright.Length > 0 )
					DB.Append( new SfField(Map.MkrCopyright, Book.Copyright) );
			}
			#endregion

			// History Field -----------------------------------------------------------------
			#region Method: bool History_in(SfField field)
			private bool History_in(SfField field)
			{
				if (Map.IsBookHistoryMarker(field.Mkr))
				{
					DParagraph p = new DParagraph(Book.Translation);
					Book.History.Append( p );
					p.StyleAbbrev = "p";
					p.SimpleText = field.Data;
					return true;
				}
				return false;
			}
			#endregion
			#region Method: void History_out()
			void History_out()
			{
				foreach(DParagraph p in Book.History)
					DB.Append( new SfField(Map.MkrBookHistory, p.SimpleText ));
			}
			#endregion

			// Notes Field -------------------------------------------------------------------
			#region Method: bool Notes_in(SfField field)
			private bool Notes_in(SfField field)
			{
				if (Map.IsBookNotesMarker(field.Mkr))
				{
					DParagraph p = new DParagraph(Book.Translation);
					Book.Notes.Append( p );
					p.StyleAbbrev = "p";
					p.SimpleText = field.Data;
					return true;
				}
				return false;
			}
			#endregion
			#region Method: void Notes_out()
			void Notes_out()
			{
				foreach(DParagraph p in Book.Notes)
					DB.Append( new SfField(Map.MkrBookNotes, p.SimpleText) );
			}
			#endregion

			// SfDb Read/Write ---------------------------------------------------------------
            #region Method: ScriptureDB _CreateDB()
            ScriptureDB _CreateDB()
            {
				// Create a SfDB to put everything into
				DB = new ScriptureDB();

				// The first record holds the general info about the book
                DB.Append(new SfField(DB.RecordMkr, Book.SfRecordKey));
				ID_out();
				Comment_out();
				Copyright_out();
				History_out();
				Notes_out();

				// Each of the sections is a separate record
				foreach(DSection s in Book.Sections)
				{
					s.WriteStandardFormat(DB);
					StepProgress();
				}

                return DB;
            }
            #endregion

			// Public Operations -------------------------------------------------------------
			#region Constructor()
			public IO(DBook book)
			{
				m_book = book;
			}
			#endregion
            #region Method: bool Read(sAbsolutePathName)
            public bool Read(ref string sAbsolutePathName)
			{
				// We'll keep track of the current reference for the benefit of error messages
				DSection.IO.ResetChapterVerse();

				// Read the SF file into a SfDb object (which will do standard transformations)
                DB = new ScriptureDB();
                if (!DB.Read(ref sAbsolutePathName))
                    return false;
                DB.TransformIn();

                // Setuo Progress indicator(s)
                StartProgress("Reading", DB.RecordCount);
                SplashScreen.SetProgressBound(DB.RecordCount);
                SplashScreen.SetStatus(Book.Translation.DisplayName + ": " + Book.DisplayName);

				// Look to read the first record, which is general information about the entire book.
				DB.AdvanceToFirstRecord();
				if (null == DB.GetCurrentField())
					return false;
				if (DB.CurrentFieldIsRecordMarker)
					Book.SfRecordKey = DB.GetCurrentField().Data;
				Book.Comment = "";
				while ( null != DB.GetNextField() )
				{
					// Get the new field; exit the loop if we've reached the next record
					if (DB.CurrentFieldIsRecordMarker)
						break;
					SfField field = DB.GetCurrentField();

					// Process the field according to its marker
					if (ID_in(field))
						continue;
					if (Comment_in(field))
						continue;
					if (History_in(field))
						continue;
					if (Notes_in(field))
						continue;
					if (Copyright_in(field))
						continue;
				}
			
				// Read in the sections
				int nSectionNumber = 1;
				while ( null != DB.GetCurrentField() )
				{
					DSection section = new DSection(nSectionNumber++);
					section.LineNoInFile = DB.GetCurrentField().LineNo;
					Book.Sections.Append(section);

					section.ReadStandardFormat(DB);

                    StepProgress();
					SplashScreen.IncrementProgress();
				}

				// Each paragraph needs to know what verses are in it
				int nChapter = 0;
				int nVerse   = 0;
				foreach(DSection section in Book.Sections)
					section.CalculateVersification(ref nChapter, ref nVerse);

                // Items that depend on the Front translation
				if (Book.Translation == OurWordMain.Project.TargetTranslation &&
					Book.FrontBook != null)
				{
					// Make sure that each section has the same structure as the
					// front.
					Book.CheckSectionStructureAgainstFront(Book.FrontBook);

					// E.g., Generate the \ref and \cf's from the Front Translation
					// We do the test here to prevent the FrontBook from attempting
					// to load itself recursively here (via an embedded Loaded call.)
					Book.UpdateFromFront(Book.FrontBook);
				}

                // Read any auxiliary info
                Book.ReadAuxiliarySettings();

				// Done
				return true;
			}
			#endregion
            #region Method: bool Write()
            public bool Write()
			{
				StartProgress("Saving", Book.Sections.Count);

                DB = _CreateDB();
                DB.Format = ScriptureDB.Formats.kToolbox;

				// Assemble the pathname based upon the Stage/Version/Minor settings
                Book.AbsolutePathName = Book.ComputePathName();

				// Create a backup file in case writing fails
                JW_Util.CreateBackup(Book.AbsolutePathName, ".bak");

				// Write out the DB
                bool bSuccessful = DB.Write(Book.AbsolutePathName);

                // Save any auxiliary info
                Book.SaveAuxiliarySettings();

				// If we failed, restore from our backup
				if (!bSuccessful)
				{
                    JW_Util.RestoreFromBackup(Book.AbsolutePathName, ".bak");
					Book.IsDirty = true;  // Still needs to be saved
					EndProgress();
					return false;
				}

				Book.IsDirty = false;
				EndProgress();
				return true;
			}
			#endregion
            #region Method: void Export(sPathName, ScriptureDB.Formats)
            public void Export(string sPathName, ScriptureDB.Formats format)
            {
                StartProgress("Exporting", Book.Sections.Count);

                DB = _CreateDB();
                DB.Format = format;

                DB.Write(sPathName);

                EndProgress();
            }
            #endregion

        } // End: Embedded Class IO
		#endregion
        #region Method: void Export(string sPathName)
        public void Export(string sPathName)
        {
            (new IO(this)).Export(sPathName, ScriptureDB.Formats.kParatext);
        }
        #endregion
        #region Method: override void OnLoad(sOriginPath) - overridden to read Std Format
        protected override bool OnLoad(ref string sAbsolutePathName)
		{
            bool bResult = false;

			// Load the book
            do
            {
                try
                {
                    IO io = new IO(this);
                    if (true == io.Read(ref sAbsolutePathName))
                    {
                        bResult = true;
                        // We'll consider an object unchanged following a read. 
                        IsDirty = false;  
                    }
                    break;
                }
                catch (eBookReadException bre)
                {
                    // Decide which version of the Repair dialog to show
                    DialogRepairImportBook dlgRepair = (bre.NeedToShowFront) ?
                        new RepairImportBookStructure(FrontBook, this, bre) :
                        new DialogRepairImportBook(this, bre);

                    // Give the user the opportunity to fix the problem
                    DialogResult result = dlgRepair.ShowDialog();

                    // If he doesn't fix it, then we must remove the book from
                    // the project. (Thus we return "false")
                    if (DialogResult.Cancel == result)
                        goto done;

                    // Prepare to try the import again
                    Sections.Clear();
                }
                catch (Exception e)
                {
                    Console.WriteLine("Exception in DBook.Read: " + e.Message);
                    goto done;
                }
            } while (true);

			// Done with progress dialog
        done:
            G.ProgressEnd();
    		SplashScreen.ResetProgress();
			return bResult;
		}
		#endregion
        #region Method: override void OnWrite()
        protected override void OnWrite()
        {
            if (!Loaded)
                return;

            // Save the file
            bool bSuccessful = (new IO(this)).Write();

            // Make the backup
            if (bSuccessful)
                (new BackupSystem(AbsolutePathName)).MakeBackup();

            // Save the OTrans, since the pathnames have probably changed (due to the
            // dates being part of the file name
            Translation.Write();
        }
        #endregion

        // Read/Write Standard Format --------------------------------------------------------
        #region SMethod: string GetBookAbbrevInFileNameLanguage(string sBookAbbrev)
        static public string GetBookAbbrevInFileNameLanguage(string sBookAbbrev)
        {
            return G.GetLoc_BookAbbrev(sBookAbbrev);
        }
        #endregion
        #region Method: string ComputePathName()
        public string ComputePathName()
		{
			return ComputePathName(Translation.LanguageAbbrev,
				BookAbbrev, TranslationStage.Abbrev, Version,
                Path.GetDirectoryName(AbsolutePathName), IsTargetBook);
		}
		#endregion
		#region Method: static string ComputePathName(...)
		static public string ComputePathName(string sLangAbbrev, string sBookAbbrev,
			string sStageAbbrev, char chVersion, string sFolderName, bool bAppendDate)
		{
            // Start with the language code/abbreviation, (e.g., "AMA")
			string sFileName = sLangAbbrev;

            // Append the book's code/abbreviation, (e.g., "MRK")
			DTeamSettings ts = G.TeamSettings;
			int iBook = DBook.FindBookAbbrevIndex(sBookAbbrev);
            sFileName += ("-" + G.GetLoc_BookAbbrev(sBookAbbrev));

            // Append the stage (e.g., "Draft")
			sFileName += ("-" + sStageAbbrev);

            // Append the version letter (e.g., "B")
			sFileName += ("-" + chVersion.ToString());

            // Append the date
            if (bAppendDate)
                sFileName += (" " + G.Today);

            // Append the extension
			sFileName += ".db";

			return sFolderName + Path.DirectorySeparatorChar + sFileName;
		}
		#endregion

		#region Method: static void RestoreFromBackup(DBook book, string sBackupPathName)
		static public void RestoreFromBackup(DBook book, string sBackupPathName)
		{
			// Post a status message
			G.ProgressStart("Restoring from backup...", 0);

			// Make sure the current book has been saved
            book.Write();

			// Unload it from memory. We call Clear() even though Loaded=false does this,
			// because from the test code it isn't activated (no harm done to have
			// it execute twice.)
            book.Unload();
			book.Clear(); 
			Debug.Assert(0 == book.Sections.Count);

			// Make a copy of it (to *.sav). The ability to use this will not be in the
			// user interface, but it may come in handy for computer techies.
            JW_Util.CreateBackup(book.AbsolutePathName, ".sav");

			// Put together the resultant file name. We want the folder from the current
			// path, but the filename from the backup path (minus the date.)
            string sDestFolder = Path.GetDirectoryName(book.AbsolutePathName);
			string sDestFileName = Path.GetFileName(sBackupPathName);
			int iPosDate = sDestFileName.IndexOf(' ');
			if (-1 != iPosDate)
				sDestFileName = sDestFileName.Substring(0, iPosDate) + ".db";
			string sDestFullPath = sDestFolder + Path.DirectorySeparatorChar + sDestFileName;

			// Copy over the backup file and give it the correct name (strip the date off)
			File.Copy(sBackupPathName, sDestFullPath, true);
            book.AbsolutePathName = sDestFullPath;

			// Update the Status Info. Here we must parse out the information from the
			// backup file name.
			int iPos = sDestFileName.IndexOf('-', 0);
			if (-1 != iPos && iPos < sDestFileName.Length - 1)
			{
				iPos++;
				iPos = sDestFileName.IndexOf('-', iPos);
				if (-1 != iPos && iPos < sDestFileName.Length - 1)
					iPos++;
			}
			if (-1 != iPos)
			{
				// Stage
				string sStage = "";
				for(; iPos < sDestFileName.Length && sDestFileName[iPos] != '-'; iPos++)
				{
					sStage += sDestFileName[iPos];
				}
				if (iPos < sDestFileName.Length && sDestFileName[iPos] == '-')
					++iPos;
				book.SetTranslationStageTo(sStage);

				// Version
				if (iPos < sDestFileName.Length)
				{
					book.Version = Char.ToUpper(sDestFileName[iPos]);
					iPos++;
				}
			}

			// Load.
            book.Load();
			G.ProgressEnd();

			// Calling method (command handler) is responsible to navigate to the
			// first section and put everything into the cache, etc.
		}
		#endregion

		// Book Status -----------------------------------------------------------------------
		#region Method: void SetTranslationStageTo(string sStageName
		public void SetTranslationStageTo(string sStageName)
		{
			TranslationStage stage = G.TeamSettings.TranslationStages.GetFromName(sStageName);
			if (stage == null)
				stage = G.TeamSettings.TranslationStages.GetFirstStage;

			TranslationStage = stage;
		}
		#endregion
		#region Attr{g}: string StatusPhrase - for displaying in the pane's title bar
		public string StatusPhrase
		{
			get
			{
				string s = "(" + TranslationStage.Abbrev + '-' + Version.ToString() + ")";
				return s;
			}
		}
		#endregion

		// Data Access -----------------------------------------------------------------------
		#region Method: DSection GetFirstSection() - returns this book's first section
		public DSection GetFirstSection()
		{
			return Sections.Count > 0 ? (DSection)Sections[0] : null;
		}
		#endregion
		#region Method: DSection GetSection(i) - returns the i'th section
		public DSection GetSection(int i)
		{
			if (i < 0 || i >= Sections.Count)
				return null;
			return (DSection)Sections[i];
		}
		#endregion
        #region Method: DSection[] GetSectionsContaining(DReferenceSpan span)
        public DSection[] GetSectionsContaining(DReferenceSpan span)
        {
            // For convienance we'll temporarily store the sections in an ArrayList
            ArrayList a = new ArrayList();

            // Scan for sections containing the endpoints of the span. 
            bool bFound = false;
            foreach (DSection s in Sections)
            {
                // Start adding sections once we locate the section containing the span's start
                if (s.ReferenceSpan.ContainsReference(span.Start))
                {
                    a.Add(s);
                    bFound = true;
                }

                // Once we find the section containing the span's end, we're done adding
                if (s.ReferenceSpan.ContainsReference(span.End))
                {
                    if (!a.Contains(s))
                        a.Add(s);
                    break;
                }

                // If here, we've found the start section, but not yet encountered the ending one
                if (bFound && !a.Contains(s))
                    a.Add(s);
            }

            // Convert into a vector
            DSection[] v = new DSection[a.Count];
            for (int i = 0; i < a.Count; i++)
                v[i] = a[i] as DSection;
            return v;
        }
        #endregion
        #region Method: ArrayList ExtractParagraphs(DParagraph pgFront)
        /***
		public ArrayList ExtractParagraphsQ(DParagraph pgFront)
		{
			// Destination we'll be returning
			ArrayList rgSections = new ArrayList();

			// Handle section title and cross references. If pgFront represents
			// one of these, then we find the corresponding section in this
			// book and return its section head (or cross reference) as appropriate.
			// It is possible that we will not return the corresponding 
			// paragraph if the sections are at a mismatch; but we shall wait
			// until we have some examples of this before deciding how to
			// modify the logic.
			DSFMapping sfm = OurWordMain.TeamSettings.SFMapping;
			if (sfm.StyleSection == pgFront.StyleAbbrev ||
				sfm.StyleCrossRef == pgFront.StyleAbbrev )
			{
				DReference RefStart = pgFront.Section.ReferenceSpan.Start;
				foreach(DSection s in Sections)
				{
					if (s.ReferenceSpan.ContainsReference(RefStart))
					{
						foreach(DParagraph pg in s.Paragraphs)
						{
							if (pg.StyleAbbrev == pgFront.StyleAbbrev)
							{
								rgSections.Add(pg.Contents);
								return rgSections;
							}
						}
					}
				}
				return rgSections;
			}

			// If we get here, then we are dealing with content paragraphs.
			// Retrieve the verse references that we'll be looking for.
			DReferenceSpan RefSpan = pgFront.ReferenceSpan;

			// Build a list of the section(s) that participate in the span
			int iS = 0;
			for(iS = 0; iS < SectionCount; iS++)
			{
				DSection s = (DSection)Sections[iS];
				if (s.ReferenceSpan.ContainsReference( RefSpan.Start ))
				{
					rgSections.Add(s);
					break;
				}
			}
			for( ++iS; iS < SectionCount; iS++)
			{
				DSection s = (DSection)Sections[iS];
				if (s.ReferenceSpan.ContainsReference( RefSpan.End ))
					rgSections.Add(s);
				else
					break;
			}

			// Build a list of the Paragraph Groups in these sections
			ArrayList rgParagraphs = new ArrayList();
			foreach( DSection s in rgSections )
			{
				foreach( DParagraph pg in s.Paragraphs)
					rgParagraphs.Add(pg);
			}

			// Build a list of paragraphs from the PG's that contain the RefSpan
			ArrayList rgParas = new ArrayList();
			foreach( DParagraph pg in rgParagraphs)
			{
				if ( pg.ReferenceSpan.ContainsReference( RefSpan.Start ) ||
					pg.ReferenceSpan.ContainsReference( RefSpan.End ) )
				{
					rgParas.Add(pg.Contents);
				}
			}
			return rgParas;
		}
		****/
		#endregion
	}

	#region TEST
    // NUnit Tests
    [TestFixture] public class Test_DBook
    {
        #region TEST: BookSortKeys
        [Test] public void BookSortKeys()
            // Make certain we have a two-digit sort key, so that, e.g.,
            // Leviticus follows Exodus rather than Proverbs.
        {
            DBook book = new DBook("LEV", "");
            Assert.AreEqual("02", book.SortKey);
        }
        #endregion
    }

    // Built In Tests
	public class Test_DBookProperties : Test
	{
		#region Attributes
		DProject m_project = null;
		DTranslation m_translation = null;
		DBook m_book = null;
		DBookProperties m_dlg = null;
		#endregion
		#region Constructor()
		public Test_DBookProperties()
			: base("DBookProperties")
		{
			AddTest( new IndividualTest( UpdateTitleBar ),           "UpdateTitleBar" );
//			AddTest( new IndividualTest( EnforcesValidBookName ),    "EnforcesValidBookName" );
			AddTest( new IndividualTest( ComboDuplicatesPrevented ), "ComboDuplicatesPrevented" );
//			AddTest( new IndividualTest( EnforcesValidPath ),        "EnforcesValidPath" );
			AddTest( new IndividualTest( RestoreFromBackup),         "Restore From Backup" );
		}
		#endregion
		#region Method: override void Setup()
		public override void Setup()
		{
			// We don't want message dialogs to show up during the tests
			LocDB.Reset();
            LocDB.SuppressMessages = true;

			// Create the dialog
			m_project = new DProject();
			m_translation = new DTranslation("Kupang Malay", "Latin", "Latin");
			m_translation.LanguageAbbrev = "Test";
			m_project.FrontTranslation = m_translation;
			m_book = new DBook("GEN", "");
			m_translation.Books.Append(m_book);
			m_dlg = new DBookProperties(null, m_translation, m_book, 
				DBookProperties.Mode.kCreate);
		}
		#endregion
		#region Method: override void TearDown()
		public override void TearDown()
		{
			LocDB.Reset();
			m_translation = null;
			m_book = null;
			m_dlg = null;
		}
		#endregion

		#region UpdateTitleBar
		public void UpdateTitleBar()
			// Make sure that the dialog's title bar updates when the text of the
			// BookName attribute is changed. A command handler fires when the text
			// is changed; and this handler updates the title bar. This test makes
			// sure that the title bar is computed as expected, and that the command
			// handler is indeed firing.
		{
			// Set name to an empty string
			m_dlg.BookName = "";
			AreSame( StrRes.Properties_Book, m_dlg.Text);

			// Set name to the title of a book
			m_dlg.BookName = "Test";
			AreSame( StrRes.Properties("Test"), m_dlg.Text);
		}
		#endregion
		#region EnforcesValidBookName
		public void EnforcesValidBookName()
			// We want to make certain that the user does not define a DBook with an invalid
			// book name. A book name is invalid if:
			//   1. It is empty (zero length), or
			//   2. It is a duplicate with another bookname in the translation.
			// In both cases, we expect a message to be sent to the user, and the dialog
			// should not permit closing via the OK button (only cancelling is allowed.)
			//
			// When an error occurs, the focus should be placed on the offending control.
			// Unfortunately, I cannot seem to get the test to work; apparently the dialog
			// must actually be invoked for the focusing to work; I'm electing to not worry
			// about testing it further here; I've coded it correctly, and it is not a 
			// showstopper bug if somehow the code gets broken.
		{
			// Simulate OK button clicked
			m_dlg.BookName = "";
			m_dlg.ValidateData();

			// Should have gotten error that Name has no length; and dialog should not be exited
            IsTrue(LocDB.LastMessageID == "msgBookNeedsName");

			// Add a Name that is identical to another already in the translation
			DBook book2 = new DBook("EXO", "");
			book2.DisplayName = "Genesis";
			m_translation.AddBook(book2);
			m_dlg.BookName = "Genesis";

			// Simulate OK button clicked
			m_dlg.ValidateData();

			// Should have gotten an error that Name is a Duplicate, and dlg should not be exited
            IsTrue("msgDuplicateBookName" == LocDB.LastMessageID);
		}
		#endregion
		#region ComboDuplicatesPrevented
		public void ComboDuplicatesPrevented()
		{
			// Make sure the combo box does not have "GEN" as a choice
			IsFalse( m_dlg.Test_ComboOffersAbbrev("GEN") );
		}
		#endregion
		#region EnforcesValidPath
		public void EnforcesValidPath()
			// We want to make certain that the user does not define a DBook with an invalid
			// path. A path is invalid if the filename portion has zero length.
			//
			// When an error occurs, the focus should be placed on the offending control.
			// Unfortunately, I cannot seem to get the test to work; apparently the dialog
			// must actually be invoked for the focusing to work; I'm electing to not worry
			// about testing it further here; I've coded it correctly, and it is not a 
			// showstopper bug if somehow the code gets broken.
		{
			m_dlg.BookName = "test";
			m_dlg.BookPath = "";
			m_dlg.ValidateData();

			// Should have gotten error that Path is not valid; and dialog should not be exited
            IsTrue(LocDB.LastMessageID == "msgBookNeedsFolder");
		}
		#endregion

		// RestoreFromBackup -----------------------------------------------------------------
		#region Method: string GetSimpleParagraphText(DParagraph p)
		string GetSimpleParagraphText(DParagraph p)
		{
			if (p.Runs.Count == 0)
				p.AddRun( new DText() );
			DText text = p.Runs[0] as DText;

			if (text.Phrases.Count == 0)
				text.Phrases.Append( new DPhrase( p.StyleAbbrev, "") );
			DPhrase phrase = text.Phrases[0] as DPhrase;

			return phrase.Text;
		}
		#endregion
		#region Method: void SetSimpleParagraphText(DParagraph p, string s)
		void SetSimpleParagraphText(DParagraph p, string s)
		{
			if (p.Runs.Count == 0)
				p.AddRun( new DText() );
			DText text = p.Runs[0] as DText;

			if (text.Phrases.Count == 0)
				text.Phrases.Append( new DPhrase( p.StyleAbbrev, "") );
			DPhrase phrase = text.Phrases[0] as DPhrase;

			phrase.Text = s;
		}
		#endregion
		#region RestoreFromBackup
		public void RestoreFromBackup()
		{
			EnableTracing = true;

			// Original settings
			string sTextO = "Original";

			// Later, changed settings (that backup will erase)
			string sTextC = "Changed";

			// Create an initial book which we will save as a backup
            Test_DSection.InitializeBook(m_book, Test_DSection.m_SectionTest1);
            m_book.TranslationStage = G.TranslationStages.GetFromID(BookStages.c_idDraft);
            m_book.Version = 'A';

            // Append an extra paragraph
            DParagraph p = new DParagraph(m_translation);
            DSection section = m_book.Sections[0] as DSection;
            section.Paragraphs.Append(p);
            p.StyleAbbrev = "q";
            SetSimpleParagraphText(p, sTextO);

            // Save the book
            m_book.AbsolutePathName = PathNameA; // Sets the folder
            m_book.Write();

			// Add a date to the path name
            string sBackupPathName = m_book.AbsolutePathName;
			int iPos = sBackupPathName.IndexOf(".db");
			sBackupPathName = sBackupPathName.Substring(0, iPos) + " 2004-12-11.db";
			if (File.Exists(sBackupPathName))
				File.Delete(sBackupPathName);
            File.Move(m_book.AbsolutePathName, sBackupPathName);

			// Change the book and the pathname and save it as a current version
			SetSimpleParagraphText(p, sTextC);
			m_book.TranslationStage = G.TranslationStages.GetFromID(BookStages.c_idConsultantCheck);
			m_book.Version = 'C';
            m_book.Write();

			// Restore the original
			DBook.RestoreFromBackup(m_book, sBackupPathName);

			// Check for original data, stage, version, minor, pathname.
			section = m_book.Sections[0] as DSection;
            int iParaLast = section.Paragraphs.Count - 1;
            DParagraph pg = section.Paragraphs[iParaLast] as DParagraph;
			string sActual = GetSimpleParagraphText(pg);
			AreSame(sTextO, sActual);
			AreSame(BookStages.c_idDraft, m_book.TranslationStage.ID);
			AreSame('A', m_book.Version );
		}
		#endregion
	}
	#endregion
}
