/**********************************************************************************************
 * Project: Our Word!
 * File:    DBookProperties.cs
 * Author:  John Wimbish
 * Created: 25 Jan 2004
 * Purpose: A book within the translation (e.g., Mark, Philemon)
 * Legal:   Copyright (c) 2005-09, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
#region Using
using System;
using System.Diagnostics;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Windows.Forms;
using System.Text;
using System.Threading;
using System.IO;
using JWTools;
using JWdb;
using JWdb.DataModel;
using OurWord.Dialogs;
using OurWord.Layouts;
using OurWord.Edit;
#endregion
#region TODO
/* - Remove hard-coded standard format; set up in a table somewhere.
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

namespace OurWord.Dialogs
{
	// Class DBookProperties - Sets up the DBook object --------------------------------------
	public class DBookProperties : System.Windows.Forms.Form
	{
        // General Properties ----------------------------------------------------------------
        #region Attr{g}: LiterateSettingsWnd LS
        LiterateSettingsWnd LS
        {
            get
            {
                Debug.Assert(null != m_LiterateSettingsWnd);
                return m_LiterateSettingsWnd;
            }
        }
        #endregion
        StringChoiceSetting m_Stage;
        EditTextSetting m_Version;
        YesNoSetting    m_LockedForEditing;
        EditTextSetting m_Copyright;
   		#region Method: BuildLsWindow()
        void BuildLsWindow()
        {
            // Make sure the styles have been defined
            Information.InitStyleSheet();

            // Intro
            LS.AddInformation("Bp100", Information.PStyleNormal,
                "This dialog allows you to input optional additional information about " +
                "the status of the book: _" + Book.DisplayName + "._");

            // Book Stage
            LS.AddInformation("Bp200", Information.PStyleNormal,
                "The Stage is where you classify your progress in working through a book. " +
                "Books start with a Draft, and work their way through such stages as " +
                "Community Check, Team Check, Back Translation, etc., until they finally " +
                "are ready to publish. You can customize the stages to match the way you " +
                "work in the Configuration Dialog.");
            m_Stage = LS.AddAtringChoice(
                "BpStage",
                "Translation Stage",
                "Enter this book's stage, e.g., Draft, Team Check, Consultant Checked",
                Book.TranslationStage.Name,
                DB.TeamSettings.TranslationStages.AllNames);

            // Version
            LS.AddInformation("Bp300", Information.PStyleNormal,
                "You can add an optional Version Letter to further help you keep track " +
                "of your progress in the book. Thus the Stage might be at Revision, but " +
                "you can declare that you are at Version A, B, C, etc. You can then click " +
                "on the Comments tab, and enter information about what you did at each " +
                "version. In this way you can keep track of the nature of the work you are " +
                "doing.");
            m_Version = LS.AddEditText("BpVersion",
                "Version:",
                "Give an optional version for this book, e.g., It could be Revision \"B\", " +
                "where \"B\"B is the version letter.",
                Book.Version);


            // Locked from Editing
            LS.AddInformation("Bp500", Information.PStyleNormal,
                "If you lock a book from editing on this machine, then the user will not " +
                "be able to make any changes to the text. He can still enter notes, " +
                "however. The Collaboration feature makes this Locked feature less of " +
                "a need than it was in previous versions of _OurWord,_ but you may still " +
                "find it useful if you do not wish to deal with merging Scripture text.");
            m_LockedForEditing = LS.AddYesNo(
                "BpLockForEditing",
                "Lock For Editing?",
                "If Yes, the book cannot be edited by the user. Only Translator Notes can " +
                    "be entered.",
                Book.Locked);

            // Copyright
            LS.AddInformation("Bp600", Information.PStyleNormal,
                "You can give a broad copyright for the entire work, in the Print Options " +
                "section of the Configuration Dialog. But you can override that here, if " +
                "you want to do something different when you print out this book. Most " +
                "people will prefer to leave this blank, and just use the global setting " +
                "under Print Options. The copyright text is used in printouts.");
            m_Copyright = LS.AddEditText("BpCopyright",
                "Copyright Text:",
                "Give an optional copyright text for this book, to be used when printing.",
                Book.Copyright);
        }
        #endregion

        // Attributes ------------------------------------------------------------------------
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

		// Methods ---------------------------------------------------------------------------
		#region Method: void UpdateTitleBar() - synch up Title Bar with contents of Name field
		public void UpdateTitleBar()
		{
				Text = StrRes.Properties( Book.DisplayName );
		}
		#endregion

		// Scaffolding -----------------------------------------------------------------------
		#region Constructor(TFront, DTranslation, DBook, Mode)
		public DBookProperties(DTranslation tFront, DTranslation translation, DBook book)
		{
			// Required for Windows Form Designer support
			InitializeComponent();

			// Point to the book we are modifying
			m_book = book;
			m_Translation = translation;
			m_TFront = tFront;

            // Build the general settings window
            BuildLsWindow();
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
            this.m_btnOK = new System.Windows.Forms.Button();
            this.m_textComment = new System.Windows.Forms.TextBox();
            this.m_btnCancel = new System.Windows.Forms.Button();
            this.m_TabControl = new System.Windows.Forms.TabControl();
            this.m_tabGeneral = new System.Windows.Forms.TabPage();
            this.m_LiterateSettingsWnd = new OurWord.Edit.LiterateSettingsWnd();
            this.m_tabComment = new System.Windows.Forms.TabPage();
            this.m_TabControl.SuspendLayout();
            this.m_tabGeneral.SuspendLayout();
            this.m_tabComment.SuspendLayout();
            this.SuspendLayout();
            // 
            // m_btnOK
            // 
            this.m_btnOK.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.m_btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.m_btnOK.Location = new System.Drawing.Point(193, 436);
            this.m_btnOK.Name = "m_btnOK";
            this.m_btnOK.Size = new System.Drawing.Size(75, 23);
            this.m_btnOK.TabIndex = 6;
            this.m_btnOK.Text = "OK";
            // 
            // m_textComment
            // 
            this.m_textComment.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.m_textComment.Location = new System.Drawing.Point(3, 6);
            this.m_textComment.Multiline = true;
            this.m_textComment.Name = "m_textComment";
            this.m_textComment.Size = new System.Drawing.Size(512, 372);
            this.m_textComment.TabIndex = 5;
            // 
            // m_btnCancel
            // 
            this.m_btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.m_btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.m_btnCancel.Location = new System.Drawing.Point(281, 436);
            this.m_btnCancel.Name = "m_btnCancel";
            this.m_btnCancel.Size = new System.Drawing.Size(75, 23);
            this.m_btnCancel.TabIndex = 7;
            this.m_btnCancel.Text = "Cancel";
            // 
            // m_TabControl
            // 
            this.m_TabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.m_TabControl.Controls.Add(this.m_tabGeneral);
            this.m_TabControl.Controls.Add(this.m_tabComment);
            this.m_TabControl.Location = new System.Drawing.Point(12, 12);
            this.m_TabControl.Name = "m_TabControl";
            this.m_TabControl.SelectedIndex = 0;
            this.m_TabControl.Size = new System.Drawing.Size(520, 416);
            this.m_TabControl.TabIndex = 45;
            // 
            // m_tabGeneral
            // 
            this.m_tabGeneral.Controls.Add(this.m_LiterateSettingsWnd);
            this.m_tabGeneral.Location = new System.Drawing.Point(4, 22);
            this.m_tabGeneral.Name = "m_tabGeneral";
            this.m_tabGeneral.Padding = new System.Windows.Forms.Padding(3);
            this.m_tabGeneral.Size = new System.Drawing.Size(512, 390);
            this.m_tabGeneral.TabIndex = 0;
            this.m_tabGeneral.Text = "General";
            this.m_tabGeneral.UseVisualStyleBackColor = true;
            // 
            // m_LiterateSettingsWnd
            // 
            this.m_LiterateSettingsWnd.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.m_LiterateSettingsWnd.DontAllowPropertyGrid = false;
            this.m_LiterateSettingsWnd.Location = new System.Drawing.Point(3, 3);
            this.m_LiterateSettingsWnd.Name = "BookProperties";
            this.m_LiterateSettingsWnd.Size = new System.Drawing.Size(503, 381);
            this.m_LiterateSettingsWnd.TabIndex = 0;
            // 
            // m_tabComment
            // 
            this.m_tabComment.Controls.Add(this.m_textComment);
            this.m_tabComment.Location = new System.Drawing.Point(4, 22);
            this.m_tabComment.Name = "m_tabComment";
            this.m_tabComment.Padding = new System.Windows.Forms.Padding(3);
            this.m_tabComment.Size = new System.Drawing.Size(521, 384);
            this.m_tabComment.TabIndex = 1;
            this.m_tabComment.Text = "Comment";
            this.m_tabComment.UseVisualStyleBackColor = true;
            // 
            // DBookProperties
            // 
            this.AcceptButton = this.m_btnOK;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.CancelButton = this.m_btnCancel;
            this.ClientSize = new System.Drawing.Size(544, 471);
            this.Controls.Add(this.m_TabControl);
            this.Controls.Add(this.m_btnCancel);
            this.Controls.Add(this.m_btnOK);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DBookProperties";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Book Properties";
            this.Load += new System.EventHandler(this.cmdLoad);
            this.Closing += new System.ComponentModel.CancelEventHandler(this.cmdClosing);
            this.m_TabControl.ResumeLayout(false);
            this.m_tabGeneral.ResumeLayout(false);
            this.m_tabComment.ResumeLayout(false);
            this.m_tabComment.PerformLayout();
            this.ResumeLayout(false);

		}
		#endregion
		#region Dialog Controls
        private Button m_btnOK;
        private Button m_btnCancel;
        private TabControl m_TabControl;
        private TabPage m_tabGeneral;
        private TabPage m_tabComment;
        private LiterateSettingsWnd m_LiterateSettingsWnd;
        private TextBox m_textComment;
		#endregion

		// Command Handlers ------------------------------------------------------------------
		#region Cmd: cmdLoad(...) - the dialog is loading; initialize the control contents
		private void cmdLoad(object sender, System.EventArgs e)
		{
			// Comment
			m_textComment.Text = Book.Comment;

			// Label text in the appropriate language
			m_btnOK.Text = DlgBookPropsRes.OK;
			m_btnCancel.Text = DlgBookPropsRes.Cancel;
			UpdateTitleBar();
		}
		#endregion
		#region Cmd: cmdClosing(...) - validate, and stay open if there's a problem
		private void cmdClosing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			// We're only interested in further processing if the user has hit the OK
			// button, signaling he is done and wishes to save his results.
			if (DialogResult != DialogResult.OK)
				return;

			// If we've made it this far, then we are happy to accept all the changes.
			// So set the attributes to the main DBook object.
			Book.Locked = m_LockedForEditing.Value;
            Book.Copyright = m_Copyright.Value;
			Book.Version = m_Version.Value;

            BookStages Stages = DB.TeamSettings.TranslationStages;
            Book.TranslationStage = Stages.GetFromName( m_Stage.Value);

			Book.Comment = m_textComment.Text;

            Book.DeclareDirty();
		}
		#endregion
	}

    // TODO: Move to NUnit tests
	#region TEST

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
//			AddTest( new IndividualTest( EnforcesValidBookName ),    "EnforcesValidBookName" );
//			AddTest( new IndividualTest( EnforcesValidPath ),        "EnforcesValidPath" );
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
			m_project.FrontTranslation = m_translation;
			m_book = new DBook("GEN");
			m_translation.AddBook(m_book);
			m_dlg = new DBookProperties(null, m_translation, m_book);
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

        /***
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
        ***/

	}
	#endregion
}
