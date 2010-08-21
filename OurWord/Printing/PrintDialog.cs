#region ***** PrintDialog.cs *****
/**********************************************************************************************
 * Project: Our Word!
 * File:    PrintDialog.cs
 * Author:  John Wimbish
 * Created: 20 Feb 2006
 * Purpose: SEts up Printing (direct to printer)
 * Legal:   Copyright (c) 2004-10, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
using System;
using System.Diagnostics;
using System.Drawing.Printing;
using System.Windows.Forms;
using JWTools;
using OurWordData.DataModel;
#endregion

namespace OurWord.Printing
{
    public class DialogPrint : Form
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
        readonly PrintDocument m_pdoc;
        #endregion
        #region Attr{g/s}: bool EntireBook
        public bool EntireBook
        {
            get
            {
                return (m_radioEntireBook.Checked);
            }
            private set
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
                return m_radioThisSection.Checked;
            }
            private set
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
                return m_radioChapters.Checked;
            }
            private set
            {
                m_radioChapters.Checked = value;

                m_radioEntireBook.Checked = !value;
                m_radioThisSection.Checked = !value;
            }
        }
        #endregion

        #region Attr{g}: bool Vernacular
        private bool Vernacular
        {
            set
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
        #region Attr{g}: int StartChapter
        public int StartChapter
        {
            get
            {
                var s = m_editStartChapter.Text;

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
        }
        #endregion
        #region Attr{g}: int EndChapter
        public int EndChapter
        {
            get
            {
                var s = m_editEndChapter.Text;

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
            private set
            {
                m_checkPrintPictures.Checked = value;
            }
        }
        #endregion
        #region Attr{g/s}: bool AllowPicturesToFloatOnPage
        public bool AllowPicturesToFloatOnPage
        {
            get
            {
                return m_checkAllowPictureFloat.Checked;
            }
        }
        #endregion

        public static readonly PageSettings m_PageSettings = new PageSettings();

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
        private CheckBox m_checkAllowPictureFloat;
        private Button m_bSetup;

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
            this.m_checkAllowPictureFloat = new System.Windows.Forms.CheckBox();
            this.m_textWatermark = new System.Windows.Forms.TextBox();
            this.m_checkPrintPictures = new System.Windows.Forms.CheckBox();
            this.m_checkReplacements = new System.Windows.Forms.CheckBox();
            this.m_radioVernacular = new System.Windows.Forms.RadioButton();
            this.m_radioBackTranslation = new System.Windows.Forms.RadioButton();
            this.m_groupWhat = new System.Windows.Forms.GroupBox();
            this.m_bSetup = new System.Windows.Forms.Button();
            this.m_groupRange.SuspendLayout();
            this.m_groupOptions.SuspendLayout();
            this.SuspendLayout();
            // 
            // m_btnHelp
            // 
            this.m_btnHelp.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.m_btnHelp.Image = ((System.Drawing.Image)(resources.GetObject("m_btnHelp.Image")));
            this.m_btnHelp.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.m_btnHelp.Location = new System.Drawing.Point(255, 402);
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
            this.m_btnCancel.Location = new System.Drawing.Point(167, 402);
            this.m_btnCancel.Name = "m_btnCancel";
            this.m_btnCancel.Size = new System.Drawing.Size(75, 23);
            this.m_btnCancel.TabIndex = 14;
            this.m_btnCancel.Text = "Cancel";
            // 
            // m_btnOK
            // 
            this.m_btnOK.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.m_btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.m_btnOK.Location = new System.Drawing.Point(79, 402);
            this.m_btnOK.Name = "m_btnOK";
            this.m_btnOK.Size = new System.Drawing.Size(75, 23);
            this.m_btnOK.TabIndex = 13;
            this.m_btnOK.Text = "Print";
            // 
            // m_lblPrinter
            // 
            this.m_lblPrinter.Location = new System.Drawing.Point(16, 16);
            this.m_lblPrinter.Name = "m_lblPrinter";
            this.m_lblPrinter.Size = new System.Drawing.Size(66, 23);
            this.m_lblPrinter.TabIndex = 15;
            this.m_lblPrinter.Text = "Printer:";
            this.m_lblPrinter.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // m_comboPrinter
            // 
            this.m_comboPrinter.Location = new System.Drawing.Point(88, 16);
            this.m_comboPrinter.Name = "m_comboPrinter";
            this.m_comboPrinter.Size = new System.Drawing.Size(207, 21);
            this.m_comboPrinter.TabIndex = 1;
            // 
            // m_radioEntireBook
            // 
            this.m_radioEntireBook.Location = new System.Drawing.Point(32, 65);
            this.m_radioEntireBook.Name = "m_radioEntireBook";
            this.m_radioEntireBook.Size = new System.Drawing.Size(352, 24);
            this.m_radioEntireBook.TabIndex = 2;
            this.m_radioEntireBook.TabStop = true;
            this.m_radioEntireBook.Text = "Entire Book";
            this.m_radioEntireBook.Click += new System.EventHandler(this.cmdEntireBookChecked);
            // 
            // m_radioThisSection
            // 
            this.m_radioThisSection.Location = new System.Drawing.Point(32, 87);
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
            this.m_groupRange.Location = new System.Drawing.Point(16, 52);
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
            this.m_labelLineSpacing.Location = new System.Drawing.Point(13, 118);
            this.m_labelLineSpacing.Name = "m_labelLineSpacing";
            this.m_labelLineSpacing.Size = new System.Drawing.Size(100, 23);
            this.m_labelLineSpacing.TabIndex = 22;
            this.m_labelLineSpacing.Text = "Line Spacing:";
            this.m_labelLineSpacing.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // m_comboLineSpacing
            // 
            this.m_comboLineSpacing.Location = new System.Drawing.Point(117, 118);
            this.m_comboLineSpacing.Name = "m_comboLineSpacing";
            this.m_comboLineSpacing.Size = new System.Drawing.Size(128, 21);
            this.m_comboLineSpacing.TabIndex = 12;
            this.m_comboLineSpacing.Text = "Single";
            // 
            // m_groupOptions
            // 
            this.m_groupOptions.Controls.Add(this.m_checkAllowPictureFloat);
            this.m_groupOptions.Controls.Add(this.m_textWatermark);
            this.m_groupOptions.Controls.Add(this.m_checkPrintPictures);
            this.m_groupOptions.Controls.Add(this.m_checkWaterMark);
            this.m_groupOptions.Controls.Add(this.m_checkReplacements);
            this.m_groupOptions.Controls.Add(this.m_labelLineSpacing);
            this.m_groupOptions.Controls.Add(this.m_comboLineSpacing);
            this.m_groupOptions.Location = new System.Drawing.Point(19, 240);
            this.m_groupOptions.Name = "m_groupOptions";
            this.m_groupOptions.Size = new System.Drawing.Size(376, 150);
            this.m_groupOptions.TabIndex = 24;
            this.m_groupOptions.TabStop = false;
            this.m_groupOptions.Text = "Options";
            // 
            // m_checkAllowPictureFloat
            // 
            this.m_checkAllowPictureFloat.Location = new System.Drawing.Point(16, 92);
            this.m_checkAllowPictureFloat.Name = "m_checkAllowPictureFloat";
            this.m_checkAllowPictureFloat.Size = new System.Drawing.Size(352, 24);
            this.m_checkAllowPictureFloat.TabIndex = 24;
            this.m_checkAllowPictureFloat.Text = "Allow pictures to move on page to save paper?";
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
            this.m_radioVernacular.Location = new System.Drawing.Point(32, 175);
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
            this.m_radioBackTranslation.Location = new System.Drawing.Point(32, 198);
            this.m_radioBackTranslation.Name = "m_radioBackTranslation";
            this.m_radioBackTranslation.Size = new System.Drawing.Size(105, 17);
            this.m_radioBackTranslation.TabIndex = 8;
            this.m_radioBackTranslation.TabStop = true;
            this.m_radioBackTranslation.Text = "Back Translation";
            this.m_radioBackTranslation.UseVisualStyleBackColor = true;
            // 
            // m_groupWhat
            // 
            this.m_groupWhat.Location = new System.Drawing.Point(19, 159);
            this.m_groupWhat.Name = "m_groupWhat";
            this.m_groupWhat.Size = new System.Drawing.Size(373, 65);
            this.m_groupWhat.TabIndex = 27;
            this.m_groupWhat.TabStop = false;
            this.m_groupWhat.Text = "What do you want to print?";
            // 
            // m_bSetup
            // 
            this.m_bSetup.Location = new System.Drawing.Point(301, 14);
            this.m_bSetup.Name = "m_bSetup";
            this.m_bSetup.Size = new System.Drawing.Size(91, 23);
            this.m_bSetup.TabIndex = 28;
            this.m_bSetup.Text = "Page Setup...";
            this.m_bSetup.UseVisualStyleBackColor = true;
            this.m_bSetup.Click += new System.EventHandler(this.cmdPageSetup);
            // 
            // DialogPrint
            // 
            this.AcceptButton = this.m_btnOK;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.CancelButton = this.m_btnCancel;
            this.ClientSize = new System.Drawing.Size(410, 434);
            this.Controls.Add(this.m_bSetup);
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
        private void cmdLoad(object sender, EventArgs e)
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
            m_checkAllowPictureFloat.Checked = true;

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
        private void cmdHelp(object sender, EventArgs e)
        {
            HelpSystem.ShowTopic(HelpSystem.Topic.kPrinting);
        }
        #endregion
        #region Cmd: cmdEntireBookChecked
        private void cmdEntireBookChecked(object sender, EventArgs e)
        {
            EntireBook = true;
        }
        #endregion
        #region Cmd: cmdCurrentSectionChecked
        private void cmdCurrentSectionChecked(object sender, EventArgs e)
        {
            CurrentSection = true;
        }
        #endregion
        #region Cmd: cmdChaptersChecked
        private void cmdChaptersChecked(object sender, EventArgs e)
        {
            Chapters = true;
        }
        #endregion
        #region Cmd: cmdToChapterChanged
        private void cmdToChapterChanged(object sender, EventArgs e)
        {
            Chapters = true;
        }
        #endregion
        #region Cmd: cmdFromChapterChanged
        private void cmdFromChapterChanged(object sender, EventArgs e)
        {
            Chapters = true;		
        }
        #endregion
        #region cmd: cmdPageSetup
        private void cmdPageSetup(object sender, EventArgs e)
        {
            var dlg = new PageSetupDialog {
                Document = PDoc,
                PrinterSettings = PDoc.PrinterSettings,
                PageSettings = m_PageSettings,
            };
            dlg.ShowDialog(this);
        }
        #endregion
    }
}


