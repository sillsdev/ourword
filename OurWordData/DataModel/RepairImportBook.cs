#region ***** RepairImportBook.cs *****
/**********************************************************************************************
 * Project: Our Word!
 * File:    RepairImportBook.cs
 * Author:  John Wimbish
 * Created: 15 Dec 2005
 * Purpose: Let's the user recover from various import problems. 
 * Legal:   Copyright (c) 2005-09, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
#region Using
using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

using JWTools;
using OurWordData;
using OurWordData.Styles;

#endregion
#endregion

namespace OurWordData.DataModel
{

	public class DialogRepairImportBook : System.Windows.Forms.Form
	{
		// Attrs -----------------------------------------------------------------------------
		#region Attr{g}: string PathName
		public string PathName
		{
			get
			{
				Debug.Assert(m_sPathName.Length > 0);
				return m_sPathName;
			}
		}
		string m_sPathName;
		#endregion
		#region Attr{g}: string BookName
		public string BookName
		{
			get
			{
				Debug.Assert(m_sBookName.Length > 0);
				return m_sBookName;
			}
		}
		string m_sBookName;
		#endregion
		#region Attr{g}: string TranslationName
		public string TranslationName
		{
			get
			{
				Debug.Assert(m_sTranslationName.Length > 0);
				return m_sTranslationName;
			}
		}
		string m_sTranslationName;
		#endregion
		#region Attr{g}: eBookReadException BookReadError
		protected eBookReadException BookReadError
		{
			get
			{
				return m_BookReadError;
			}
		}
		eBookReadException m_BookReadError;
		#endregion
        #region Attr{g}: WritingSystem WritingSystem
        WritingSystem WritingSystem
        {
            get
            {
                Debug.Assert(null != m_WritingSystem);
                return m_WritingSystem;
            }
        }
        WritingSystem m_WritingSystem = null;
        #endregion

        static Size  s_SaveWindowSize = new Size(0,0);
		static Point s_SaveWindowPos  = new Point(0,0);

		// Dialog Controls -------------------------------------------------------------------
		#region Attr{g/s}: string ErrorText - the user-readable description of the problem
		string ErrorText
		{
			get
			{
				return m_Error.Text;
			}
			set
			{
				Debug.Assert(value.Length > 0);
				m_Error.Text = value;
			}
		}
		#endregion
		#region Attr{g}: RichTextBox DataFile
		RichTextBox DataFile
		{
			get
			{
				return m_rtfText;
			}
		}
		#endregion
		#region Dialog Controls
		protected System.Windows.Forms.Label m_AnErrorOccured;
		protected System.Windows.Forms.Label m_Error;
		protected System.Windows.Forms.Label m_Directions;
		protected System.Windows.Forms.Button m_btnTryAgain;
		protected System.Windows.Forms.Button m_btnCancel;
		protected System.Windows.Forms.RichTextBox m_rtfText;
		protected System.Windows.Forms.Label m_PathName;
		protected System.Windows.Forms.PictureBox m_Icon;
		protected System.Windows.Forms.Button m_btnHelp;
		#endregion

		// Subclass Hooks  -------------------------------------------------------------------
		#region Method: virtual void InitializeAdditionalControls()
		protected virtual void InitializeAdditionalControls()
		{
		}
		#endregion
		#region Method: virtual void DialogWidthChanged(nDeltaWidth)
		protected virtual void DialogWidthChanged(int nDeltaWidth)
		{
		}
		#endregion
		#region Method: virtual void DialogHeightChanged(nDeltaHeight)
		protected virtual void DialogHeightChanged(int nDeltaHeight)
		{
		}
		#endregion
        #region Method: virtual void Localize()
        protected virtual void Localize()
            // The subclass has an additional set of controls to exclude; so
            // we must put the call to GetForm within this virtual/override
            // place. The other calls to LocDB should remain outside of this,
            // in the cmdLoad method.
        {
            Control[] vExclude = { m_AnErrorOccured, m_Error, m_rtfText, m_PathName };
            LocDB.Localize(this, vExclude);
        }
        #endregion

        // Command Handlers ------------------------------------------------------------------
        #region Method: string[] LoadFile(string sPathName)
        protected string[] LoadFile(string sPathName)
            // Loads the file into the RichTextBox. We can't use the RTF Box's LoadFile,
            // because it appears to want to munge up the UTF8's. 
        {
            StreamReader sr = new StreamReader(sPathName, Encoding.UTF8);
            TextReader r = TextReader.Synchronized(sr);

            ArrayList a = new ArrayList();

            do
            {
                string s = r.ReadLine();
                if (s == null)
                    break;
                a.Add(s);
            } while (true);

            r.Close();

            string[] v = new string[a.Count];
            for (int i = 0; i < a.Count; i++)
                v[i] = a[i] as string;

            return v;
        }
        #endregion
        #region Cmd: cmdLoad - dlg has been invoked; load the file, scroll to the correct line, etc.
        protected virtual void cmdLoad(object sender, System.EventArgs e)
		{
            // Localization
            Localize();

			// Header Message
            var sBase = LocDB.GetValue(this, m_AnErrorOccured.Name, 
                "An error occurred in attempting to import the book &quot;{0}&quot;:");
            m_AnErrorOccured.Text = LocDB.Insert(sBase, new string[] { BookName });

            // Use a slightly larger font than the system font for better readability of diacritics
            DataFile.Font = StyleSheet.LargeDialogFont;

			// Place the error message into the control
			ErrorText = BookReadError.UserMessage;

			// Error Icon
			m_Icon.Image = SystemIcons.Warning.ToBitmap();

			// Path Name
			m_PathName.Text = PathName;

			// Read in the file and place it in the control
			DataFile.Clear();
            DataFile.Lines = LoadFile(PathName);

			// Scroll to the offending line and select it
			SelectLine(DataFile, BookReadError.LineNo - 1);

			// Restore the previous window position
			if (s_SaveWindowPos.X != 0)
				Left = s_SaveWindowPos.X;
			if (s_SaveWindowPos.Y != 0)
				Top = s_SaveWindowPos.Y;

			// Restore the previous window size (controls respond through the Resize
			// method which gets called automatically.
			if (s_SaveWindowSize.Width != 0)
				Width = s_SaveWindowSize.Width;
			if (s_SaveWindowSize.Height != 0)
				Height = s_SaveWindowSize.Height;
        }
		#endregion
		#region Cmd: cmdClosing - save the changes to the file
		private void cmdClosing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			// Remember the window size
			s_SaveWindowSize = new Size(Width, Height);
			s_SaveWindowPos  = new Point(Left, Top);

			// If the user cancelled, then we're done
			if (DialogResult != DialogResult.OK)
				return;

			// Save the changes the user made.
			// Note: We don't use the "SaveFile" method, because it is destroying the UTF8.
			string[] v = DataFile.Lines;
			TextWriter w = JW_Util.GetTextWriter(PathName);
			foreach(string s in v)
				w.WriteLine(s);
			w.Close();
		}
		#endregion
		#region Cmd: cmdResize - dlg width/height has changed
		int m_nPreviousWidth = -1;
		int m_nPreviousHeight = -1;
		private void cmdResize(object sender, System.EventArgs e)
		{
			if (m_nPreviousWidth > 0)
			{
				int nDeltaWidth = Width - m_nPreviousWidth;

				m_AnErrorOccured.Width += nDeltaWidth;
				m_Error.Width          += nDeltaWidth;
				m_Directions.Width     += nDeltaWidth;
				m_PathName.Width       += nDeltaWidth;
				m_rtfText.Width        += nDeltaWidth;

				int nHalf = nDeltaWidth / 2;
				m_btnTryAgain.Left += nHalf;
				m_btnCancel.Left += nHalf;
				m_btnHelp.Left += nHalf;

				DialogWidthChanged(nDeltaWidth);
			}
            m_nPreviousWidth = Width;

			if (m_nPreviousHeight > 0)
			{
				int nDeltaHeight = Height - m_nPreviousHeight;

				m_rtfText.Height  += nDeltaHeight;
				m_btnTryAgain.Top += nDeltaHeight;
				m_btnCancel.Top   += nDeltaHeight;
				m_btnHelp.Top     += nDeltaHeight;

				DialogHeightChanged(nDeltaHeight);
			}
			m_nPreviousHeight = Height;
        }
		#endregion
		#region Cmd: cmdHelp- Responds to the Help button by giving more detailed info
		private void cmdHelp(object sender, System.EventArgs e)
		{
			HelpSystem.ShowTopic( BookReadError.HelpID );
		}
		#endregion

		// Helper Methods --------------------------------------------------------------------
		#region Method: void SelectLine(RichTextBox box, int nLine)
		protected void SelectLine(RichTextBox box, int nLine)
		{
			// Optimization: Retrieve Box.Lines into this local variable, and
			// what was a bottleneck according to the Profile is now gone!
			string[] v = box.Lines;

            // Calculate the offset position of the line of text
			int cStart = 0;
			for(int i=0; i < nLine && i < v.Length; i++)
				cStart += (v[i].Length + 1);

            // Select the bottom line so we force a scroll to it. Then, when we select
            // the actual text we're interested in, it will appear at the top of the control,
            // so that we can see the following text (which is what we need to see to
            // make repairs.)
            box.Select(box.Text.Length, 0);
            box.ScrollToCaret();

            // Select our target position
            if (nLine >= 0 && nLine < v.Length)
            {
                string sLine = v[nLine];

                // Standard format: select the entire line, as they tend to be short enough
                if (sLine.Length > 0 && sLine[0] == '\\')
                {
                    box.Select(cStart, v[nLine].Length);
                    return;
                }

                // Xml: select to the end of the tag
                if (sLine.Trim().Length > 0 && sLine.Trim()[0] == '<')
                {
                    int i = sLine.IndexOf('>');
                    if (i != -1)
                    {
                        box.Select(cStart, i + 1);
                        return;
                    }
                }

                // Otherwise, just select the first 20 characters
                int iLength = Math.Min(v[nLine].Length, 20);
                box.Select(cStart, iLength);
            }
		}
		#endregion

        // Scaffolding -----------------------------------------------------------------------
        #region Constructor(DBook, sPath, eBookReadException)
        public DialogRepairImportBook(DBook _book, string sPath, eBookReadException _bre)
		{
			// Required for Windows Form Designer support
			InitializeComponent();
			InitializeAdditionalControls();

			// Set up the attributes
            m_sPathName = sPath;
			m_sBookName = _book.Translation.DisplayName + ":" + _book.DisplayName;
			m_BookReadError = _bre;
			m_sTranslationName = _book.Translation.DisplayName;
            m_WritingSystem = _book.Translation.WritingSystemVernacular;

        }
		#endregion

        #region Cmd: OnLayout(LayoutEventArgs)
        protected override void OnLayout(LayoutEventArgs levent)
        {
            base.OnLayout(levent);

            // Necessary to force the correct layout for different DPI's.
            PerformAutoScale();

            if (m_nPreviousWidth == -1)
            {
                m_nPreviousWidth = Width;
                m_nPreviousHeight = Height;
            }
        }
        #endregion
        #region FRAMEWORK STUFF
        // Required designer variable.
		private System.ComponentModel.Container components = null;
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DialogRepairImportBook));
            this.m_AnErrorOccured = new System.Windows.Forms.Label();
            this.m_Error = new System.Windows.Forms.Label();
            this.m_Directions = new System.Windows.Forms.Label();
            this.m_btnTryAgain = new System.Windows.Forms.Button();
            this.m_btnCancel = new System.Windows.Forms.Button();
            this.m_btnHelp = new System.Windows.Forms.Button();
            this.m_rtfText = new System.Windows.Forms.RichTextBox();
            this.m_PathName = new System.Windows.Forms.Label();
            this.m_Icon = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.m_Icon)).BeginInit();
            this.SuspendLayout();
            // 
            // m_AnErrorOccured
            // 
            this.m_AnErrorOccured.Location = new System.Drawing.Point(72, 9);
            this.m_AnErrorOccured.Name = "m_AnErrorOccured";
            this.m_AnErrorOccured.Size = new System.Drawing.Size(592, 23);
            this.m_AnErrorOccured.TabIndex = 0;
            this.m_AnErrorOccured.Text = "An error occurred in attempting to import the book \"{0}\":";
            this.m_AnErrorOccured.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // m_Error
            // 
            this.m_Error.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.m_Error.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_Error.Location = new System.Drawing.Point(72, 32);
            this.m_Error.Name = "m_Error";
            this.m_Error.Size = new System.Drawing.Size(592, 56);
            this.m_Error.TabIndex = 1;
            this.m_Error.Text = "(Error Message Goes Here)";
            // 
            // m_Directions
            // 
            this.m_Directions.Location = new System.Drawing.Point(69, 88);
            this.m_Directions.Name = "m_Directions";
            this.m_Directions.Size = new System.Drawing.Size(584, 23);
            this.m_Directions.TabIndex = 2;
            this.m_Directions.Text = "You can fix the error and click on the Try Again button below, or you can Cancel " +
                "the import.";
            this.m_Directions.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // m_btnTryAgain
            // 
            this.m_btnTryAgain.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.m_btnTryAgain.Location = new System.Drawing.Point(192, 514);
            this.m_btnTryAgain.Name = "m_btnTryAgain";
            this.m_btnTryAgain.Size = new System.Drawing.Size(96, 23);
            this.m_btnTryAgain.TabIndex = 4;
            this.m_btnTryAgain.Text = "Try Again";
            // 
            // m_btnCancel
            // 
            this.m_btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.m_btnCancel.Location = new System.Drawing.Point(296, 514);
            this.m_btnCancel.Name = "m_btnCancel";
            this.m_btnCancel.Size = new System.Drawing.Size(96, 23);
            this.m_btnCancel.TabIndex = 5;
            this.m_btnCancel.Text = "Cancel Read";
            // 
            // m_btnHelp
            // 
            this.m_btnHelp.Image = ((System.Drawing.Image)(resources.GetObject("m_btnHelp.Image")));
            this.m_btnHelp.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.m_btnHelp.Location = new System.Drawing.Point(400, 514);
            this.m_btnHelp.Name = "m_btnHelp";
            this.m_btnHelp.Size = new System.Drawing.Size(96, 23);
            this.m_btnHelp.TabIndex = 8;
            this.m_btnHelp.Text = "Help...";
            this.m_btnHelp.Click += new System.EventHandler(this.cmdHelp);
            // 
            // m_rtfText
            // 
            this.m_rtfText.HideSelection = false;
            this.m_rtfText.Location = new System.Drawing.Point(12, 136);
            this.m_rtfText.Name = "m_rtfText";
            this.m_rtfText.Size = new System.Drawing.Size(656, 362);
            this.m_rtfText.TabIndex = 9;
            this.m_rtfText.Text = "(File Goes here)";
            this.m_rtfText.WordWrap = false;
            // 
            // m_PathName
            // 
            this.m_PathName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.m_PathName.Location = new System.Drawing.Point(12, 112);
            this.m_PathName.Name = "m_PathName";
            this.m_PathName.Size = new System.Drawing.Size(652, 23);
            this.m_PathName.TabIndex = 10;
            this.m_PathName.Text = "(pathname)";
            this.m_PathName.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // m_Icon
            // 
            this.m_Icon.Location = new System.Drawing.Point(24, 16);
            this.m_Icon.Name = "m_Icon";
            this.m_Icon.Size = new System.Drawing.Size(40, 48);
            this.m_Icon.TabIndex = 11;
            this.m_Icon.TabStop = false;
            // 
            // DialogRepairImportBook
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(680, 544);
            this.Controls.Add(this.m_Icon);
            this.Controls.Add(this.m_PathName);
            this.Controls.Add(this.m_rtfText);
            this.Controls.Add(this.m_btnHelp);
            this.Controls.Add(this.m_btnCancel);
            this.Controls.Add(this.m_btnTryAgain);
            this.Controls.Add(this.m_Directions);
            this.Controls.Add(this.m_Error);
            this.Controls.Add(this.m_AnErrorOccured);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(592, 576);
            this.Name = "DialogRepairImportBook";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Error on attempt to read book";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.cmdLoad);
            this.Closing += new System.ComponentModel.CancelEventHandler(this.cmdClosing);
            this.Resize += new System.EventHandler(this.cmdResize);
            ((System.ComponentModel.ISupportInitialize)(this.m_Icon)).EndInit();
            this.ResumeLayout(false);

		}
		#endregion

		#endregion
	}

	public class RepairImportBookStructure : DialogRepairImportBook
	{
		// Attrs -----------------------------------------------------------------------------
		#region Attr{g}: string FrontPathName
		public string FrontPathName
		{
			get
			{
				Debug.Assert(m_sFrontPathName.Length > 0);
				return m_sFrontPathName;
			}
		}
		string m_sFrontPathName;
		#endregion
		#region Attr{g}: string FrontBookName
		public string FrontBookName
		{
			get
			{
				Debug.Assert(m_sFrontBookName.Length > 0);
				return m_sFrontBookName;
			}
		}
		string m_sFrontBookName;
		#endregion
		#region Attr{g}: RichTextBox FrontFile
		RichTextBox FrontFile
		{
			get
			{
				return m_rtfFrontText;
			}
		}
		#endregion

		// Additional Dialog Controls --------------------------------------------------------
		private RichTextBox m_rtfFrontText;
		private Label m_FrontPathName;
		#region Method: override void InitializeAdditionalControls()
		protected override void InitializeAdditionalControls()
		{
			SuspendLayout();

			int nSpaceBetween = 5;
			int nLeftMargin   = m_rtfText.Left;
			int nRightMargin  = Width - m_rtfText.Right;
			int nColWidth     = (Width - (nLeftMargin + nRightMargin)) / 2 - nSpaceBetween;
			int nRightCol     = Width - nRightMargin - nColWidth;

			// Adjust target book controls
			m_PathName.Left  = nRightCol;
			m_PathName.Width = nColWidth;
			m_rtfText.Left   = nRightCol;
			m_rtfText.Width  = nColWidth;
			
			// m_FrontPathName
			m_FrontPathName = new System.Windows.Forms.Label();
			m_FrontPathName.Location = new System.Drawing.Point(16, 112);
			m_FrontPathName.Name = "m_FrontPathName";
			m_FrontPathName.Size = new System.Drawing.Size(nColWidth, 23);
			m_FrontPathName.TabIndex = 10;
			m_FrontPathName.Text = "(front pathname)";
			m_FrontPathName.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			Controls.Add(m_FrontPathName);

			// m_rtfFrontText
			m_rtfFrontText  = new System.Windows.Forms.RichTextBox();
			m_rtfFrontText.BackColor = BackColor;
			m_rtfFrontText.ReadOnly  = true;
			m_rtfFrontText.HideSelection = false;
			m_rtfFrontText.Location = new System.Drawing.Point(16, 136);
			m_rtfFrontText.Name = "m_rtfFrontText";
			m_rtfFrontText.Size = new System.Drawing.Size(nColWidth, m_rtfText.Height);
			m_rtfFrontText.TabIndex = 9;
			m_rtfFrontText.Text = "(Front File Goes here)";
			m_rtfFrontText.WordWrap = false;
			Controls.Add(m_rtfFrontText);

			ResumeLayout(false);
		}
		#endregion
		#region Method: override void DialogWidthChanged(nDeltaWidth)
		protected override void DialogWidthChanged(int nDeltaWidth)
			// For the superclass controls, we reduce the width and move the entire control 
			// to the right by that amount.
			//   Then for these controls, we increase/decreate their width by this amount.
			//   The result is that the amount of expansion is the same for both the
			// superclass controls (which are on the right) and the subclass controls
			// which are in the left column.
			//
			// What happens when nDeltaWidth is 1, and thus nDeltaWidth/2 results in zero?
			// Ans: No change takes place, but it is OK, because a one-pixel move is already
			// handled in the superclass. So margins should remain OK.
		{
			int half = nDeltaWidth / 2;

			m_rtfText.Width  -= half;
			m_rtfText.Left   += half;
			m_PathName.Width -= half;
			m_PathName.Left  += half;

			FrontFile.Width       += half;
			m_FrontPathName.Width += half;

		}
		#endregion
		#region Method: override void DialogHeightChanged(nDeltaHeight)
		protected override void DialogHeightChanged(int nDeltaHeight)
		{
			m_rtfFrontText.Height  += nDeltaHeight;
		}
		#endregion
        #region Method: override void Localize()
        protected override void Localize()
        {
            Control[] vExclude = { m_AnErrorOccured, m_Error, m_rtfText, m_PathName,
                m_rtfFrontText, m_FrontPathName};
            LocDB.Localize(this, vExclude);
        }
        #endregion

        #region Cmd: cmdLoad - initialize the controls
        protected override void cmdLoad(object sender, System.EventArgs e)
		{
			base.cmdLoad(sender, e);

			// Use a slightly larger font than the system font for better readability of diacritics
            FrontFile.Font = StyleSheet.LargeDialogFont;

			// Front Path
			m_FrontPathName.Text = FrontPathName;

			// Front File
			FrontFile.Clear();
            FrontFile.Lines = LoadFile(FrontPathName);

			// Scroll to the offending line and select it
			SelectLine(FrontFile, BookReadError.FrontLineNo - 1);
		}
		#endregion

		// Scaffolding -----------------------------------------------------------------------
		#region Construotor(DBook _FrontBook, DBook _ImportBook, eBookReadException)
		public RepairImportBookStructure(DBook _FrontBook, DBook _ImportBook, string sImportPath,
			eBookReadException _bre)
            : base(_ImportBook, sImportPath, _bre)
		{
			Debug.Assert(null != _FrontBook);

            // By using the same name, we can use the same LocDB Group
            Name = base.Name;

            m_sFrontPathName = _FrontBook.StoragePath;
			m_sFrontBookName = _FrontBook.Translation.DisplayName + ":" 
				+ _FrontBook.DisplayName;
		}
		#endregion
	}

	public class eBookReadException : Exception
	{
		// Attrs -----------------------------------------------------------------------------
		#region Attr{g/s}: string UserMessage - the user-understandable message as to what is wrong
		public string UserMessage
		{
			get
			{
				Debug.Assert(null != m_sUserMessage && m_sUserMessage.Length > 0);
				return m_sUserMessage;
			}
		}
		string m_sUserMessage;
		#endregion
		#region Attr{g}: int FrontLineNo
		public int FrontLineNo
		{
			get
			{
				return m_nFrontLineNo;
			}
		}
		int m_nFrontLineNo;
		#endregion
		#region Attr{g}: int LineNo
		public int LineNo
		{
			get
			{
				return m_nLineNo;
			}
		}
		int m_nLineNo;
		#endregion
		#region Attr{g}: bool m_bNeedToShowFront - T if error dlg should show the Front trans
		public bool NeedToShowFront
		{
			get
			{
				return m_bNeedToShowFront;
			}
		}
		bool m_bNeedToShowFront = false;
		#endregion
		#region Attr{g}: HelpSystem.Topic HelpID
        public HelpSystem.Topic HelpID
		{
			get
			{
				return m_HelpID;
			}
		}
        HelpSystem.Topic m_HelpID;
		#endregion

		// Scaffolding -----------------------------------------------------------------------
		#region Constructor(BookReadMsg.ID, nLineNo)
        public eBookReadException(string _sUserMessage, HelpSystem.Topic _HelpID, int _LineNo)
			: base("BookReadException")
		{
            m_sUserMessage = _sUserMessage;
			m_nLineNo = _LineNo;
			m_HelpID  = _HelpID;
		}
		#endregion
		#region Constructor(sMessage, HelpID, nFrontLineNo, int nLineNo)
        public eBookReadException(string _sMessage, HelpSystem.Topic _HelpID, 
			int _FrontLineNo, int _LineNo)
			: base("BookReadException")
		{
			m_sUserMessage = _sMessage;
			m_nLineNo      = _LineNo;
			m_nFrontLineNo = _FrontLineNo;
			m_bNeedToShowFront = true;
			m_HelpID  = _HelpID;
		}
		#endregion

        #region SMethod: nt GetOffendingLineNumber(sPath, DSection)
        static public int GetOffendingLineNumber(string sPath, DSection section)
        {
            // We already have this in standard format files
            if (string.IsNullOrEmpty(sPath) || !sPath.ToLower().Contains(".oxes"))
                return section.LineNoInFile;

            // Load the file
            var vs = JWU.ReadFile(sPath);

            // We'll look for the chapter and verse in the section's reference
            var cv = section.ReferenceSpan.Start;
            string sChapter = "<c n=\"" + cv.Chapter + "\" />";
            string sVerse = "<v n=\"" + cv.Verse;
            string sSection = "<p class=\"Section Head\"";

            // Scan for our target
            int iLine = 0;
            int iLineSection = 0;
            bool bChapterFound = false;
            foreach (string s in vs)
            {
                // The SelectLine routine appears to be 1-based.
                // TODO: Make it 0-based.
                if (s.Contains(sSection))
                    iLineSection = iLine + 1; 

                if (s.Contains(sChapter))
                    bChapterFound = true;

                if (bChapterFound && s.Contains(sVerse))
                    return iLineSection;

                iLine++;
            }

            return 0;
        }
        #endregion
    }

}
