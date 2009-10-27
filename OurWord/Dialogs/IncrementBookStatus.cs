/**********************************************************************************************
 * App:     OurWord
 * File:    IncrementBookStatus.cs
 * Author:  John Wimbish
 * Created: 03 November 2004
 * Purpose: Provides a dialog for incrementing the book's Stage, Version or Minor.
 * Legal:   Copyright (c) 2005-09, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
#region Header: Using, etc.
using System;
using System.Collections;
using System.ComponentModel;
using System.Globalization;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Resources;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using System.Threading;

using JWTools;
using OurWordData.DataModel;
using OurWord;
#endregion

namespace OurWord.Dialogs
{
	public class IncrementBookStatus : System.Windows.Forms.Form
	{
		#region Attr{g}: DBook Book - the book whose stage we are editing
		private DBook Book
		{
			get
			{
				return m_TargetBook;
			}
		}
		DBook m_TargetBook = null;
		#endregion
		int m_iStage;

		// Scaffolding -----------------------------------------------------------------------
		#region Dialog Controls
		private System.Windows.Forms.Label m_lblBook;
		private System.Windows.Forms.Label m_lblStage;
		private System.Windows.Forms.Label m_lblVersion;
		private System.Windows.Forms.Label m_Book;
		private System.Windows.Forms.Label m_Stage;
        private System.Windows.Forms.Label m_Version;
		private System.Windows.Forms.Button m_btnStage;
        private System.Windows.Forms.Button m_btnVersion;
		private System.Windows.Forms.Label m_lblComment;
		private System.Windows.Forms.TextBox textBox1;
		private System.Windows.Forms.Button m_btnOK;
        private System.Windows.Forms.Button m_btnCancel;
		private System.Windows.Forms.Button m_btnHelp;
		private System.ComponentModel.Container components = null;
		#endregion
		#region Constructor(DBook)
		public IncrementBookStatus(DBook book)
		{
			// Required for Windows Form Designer support
			InitializeComponent();

			m_TargetBook = book;
		}
		#endregion
		#region void Dispose(disposing) -  Clean up any resources being used.
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(IncrementBookStatus));
            this.m_lblBook = new System.Windows.Forms.Label();
            this.m_lblStage = new System.Windows.Forms.Label();
            this.m_lblVersion = new System.Windows.Forms.Label();
            this.m_Book = new System.Windows.Forms.Label();
            this.m_Stage = new System.Windows.Forms.Label();
            this.m_Version = new System.Windows.Forms.Label();
            this.m_btnStage = new System.Windows.Forms.Button();
            this.m_btnVersion = new System.Windows.Forms.Button();
            this.m_lblComment = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.m_btnOK = new System.Windows.Forms.Button();
            this.m_btnCancel = new System.Windows.Forms.Button();
            this.m_btnHelp = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // m_lblBook
            // 
            this.m_lblBook.Location = new System.Drawing.Point(8, 16);
            this.m_lblBook.Name = "m_lblBook";
            this.m_lblBook.Size = new System.Drawing.Size(64, 23);
            this.m_lblBook.TabIndex = 0;
            this.m_lblBook.Text = "Book:";
            this.m_lblBook.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // m_lblStage
            // 
            this.m_lblStage.Location = new System.Drawing.Point(8, 64);
            this.m_lblStage.Name = "m_lblStage";
            this.m_lblStage.Size = new System.Drawing.Size(56, 23);
            this.m_lblStage.TabIndex = 1;
            this.m_lblStage.Text = "Stage:";
            this.m_lblStage.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // m_lblVersion
            // 
            this.m_lblVersion.Location = new System.Drawing.Point(8, 96);
            this.m_lblVersion.Name = "m_lblVersion";
            this.m_lblVersion.Size = new System.Drawing.Size(64, 23);
            this.m_lblVersion.TabIndex = 2;
            this.m_lblVersion.Text = "Version:";
            this.m_lblVersion.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // m_Book
            // 
            this.m_Book.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.m_Book.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_Book.Location = new System.Drawing.Point(72, 16);
            this.m_Book.Name = "m_Book";
            this.m_Book.Size = new System.Drawing.Size(328, 23);
            this.m_Book.TabIndex = 3;
            this.m_Book.Text = "Book";
            this.m_Book.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // m_Stage
            // 
            this.m_Stage.BackColor = System.Drawing.Color.White;
            this.m_Stage.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.m_Stage.Location = new System.Drawing.Point(72, 64);
            this.m_Stage.Name = "m_Stage";
            this.m_Stage.Size = new System.Drawing.Size(296, 23);
            this.m_Stage.TabIndex = 4;
            this.m_Stage.Text = "Stage";
            this.m_Stage.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // m_Version
            // 
            this.m_Version.BackColor = System.Drawing.Color.White;
            this.m_Version.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.m_Version.Location = new System.Drawing.Point(72, 96);
            this.m_Version.Name = "m_Version";
            this.m_Version.Size = new System.Drawing.Size(88, 23);
            this.m_Version.TabIndex = 5;
            this.m_Version.Text = "A";
            this.m_Version.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // m_btnStage
            // 
            this.m_btnStage.Font = new System.Drawing.Font("Wingdings", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.m_btnStage.Image = ((System.Drawing.Image)(resources.GetObject("m_btnStage.Image")));
            this.m_btnStage.Location = new System.Drawing.Point(376, 64);
            this.m_btnStage.Name = "m_btnStage";
            this.m_btnStage.Size = new System.Drawing.Size(24, 23);
            this.m_btnStage.TabIndex = 8;
            this.m_btnStage.Click += new System.EventHandler(this.cmdIncrementStage);
            // 
            // m_btnVersion
            // 
            this.m_btnVersion.Font = new System.Drawing.Font("Wingdings", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.m_btnVersion.Image = ((System.Drawing.Image)(resources.GetObject("m_btnVersion.Image")));
            this.m_btnVersion.Location = new System.Drawing.Point(168, 96);
            this.m_btnVersion.Name = "m_btnVersion";
            this.m_btnVersion.Size = new System.Drawing.Size(24, 23);
            this.m_btnVersion.TabIndex = 9;
            this.m_btnVersion.Click += new System.EventHandler(this.cmdIncrementVersion);
            // 
            // m_lblComment
            // 
            this.m_lblComment.Location = new System.Drawing.Point(8, 138);
            this.m_lblComment.Name = "m_lblComment";
            this.m_lblComment.Size = new System.Drawing.Size(100, 16);
            this.m_lblComment.TabIndex = 11;
            this.m_lblComment.Text = "Comment";
            this.m_lblComment.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(8, 154);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(392, 104);
            this.textBox1.TabIndex = 12;
            // 
            // m_btnOK
            // 
            this.m_btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.m_btnOK.Location = new System.Drawing.Point(79, 275);
            this.m_btnOK.Name = "m_btnOK";
            this.m_btnOK.Size = new System.Drawing.Size(75, 23);
            this.m_btnOK.TabIndex = 13;
            this.m_btnOK.Text = "OK";
            // 
            // m_btnCancel
            // 
            this.m_btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.m_btnCancel.Location = new System.Drawing.Point(167, 275);
            this.m_btnCancel.Name = "m_btnCancel";
            this.m_btnCancel.Size = new System.Drawing.Size(75, 23);
            this.m_btnCancel.TabIndex = 14;
            this.m_btnCancel.Text = "Cancel";
            // 
            // m_btnHelp
            // 
            this.m_btnHelp.Image = ((System.Drawing.Image)(resources.GetObject("m_btnHelp.Image")));
            this.m_btnHelp.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.m_btnHelp.Location = new System.Drawing.Point(255, 275);
            this.m_btnHelp.Name = "m_btnHelp";
            this.m_btnHelp.Size = new System.Drawing.Size(75, 23);
            this.m_btnHelp.TabIndex = 17;
            this.m_btnHelp.Text = "Help...";
            this.m_btnHelp.Click += new System.EventHandler(this.cmdHelp);
            // 
            // IncrementBookStatus
            // 
            this.AcceptButton = this.m_btnOK;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.CancelButton = this.m_btnCancel;
            this.ClientSize = new System.Drawing.Size(410, 306);
            this.Controls.Add(this.m_btnHelp);
            this.Controls.Add(this.m_btnCancel);
            this.Controls.Add(this.m_btnOK);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.m_lblComment);
            this.Controls.Add(this.m_btnVersion);
            this.Controls.Add(this.m_btnStage);
            this.Controls.Add(this.m_Version);
            this.Controls.Add(this.m_Stage);
            this.Controls.Add(this.m_Book);
            this.Controls.Add(this.m_lblVersion);
            this.Controls.Add(this.m_lblStage);
            this.Controls.Add(this.m_lblBook);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "IncrementBookStatus";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Increment Book Status";
            this.Load += new System.EventHandler(this.cmdLoad);
            this.Closing += new System.ComponentModel.CancelEventHandler(this.cmdClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		// Handlers --------------------------------------------------------------------------
		#region Cmd: cmdLoad - Populate the controls
		private void cmdLoad(object sender, System.EventArgs e)
		{
			// Label text in the appropriate language. Everything except the dialog title
			// is defined in the BookProperties dialog, so we just use the DlgBookPropsRes
			// class for this.
			Text = DlgBookPropsRes.IncrementBookStatusTitle;
			m_lblBook.Text     = DlgBookPropsRes.Book;
			m_lblStage.Text    = DlgBookPropsRes.Stage;
			m_lblVersion.Text  = DlgBookPropsRes.Version;
			m_lblComment.Text  = DlgBookPropsRes.Comment;
			m_btnOK.Text       = DlgBookPropsRes.OK;
			m_btnCancel.Text   = DlgBookPropsRes.Cancel;

			// Set the control contents to reflect the current book's status
			m_iStage        = DB.TeamSettings.Stages.IndexOf(Book.Stage);
			m_Book.Text     = Book.DisplayName;
			m_Stage.Text    = Book.Stage.LocalizedName;
			m_Version.Text  = Book.Version.ToString();
		}
		#endregion
		#region Cmd: cmdClosing - Place dialog data into DBook
		private void cmdClosing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			// We're only interested in further processing if the user has hit the OK
			// button, signallying he is done and wishes to save his results.
			if (DialogResult != DialogResult.OK)
				return;

            foreach(Stage stage in DB.TeamSettings.Stages)
            {
                if (stage.LocalizedName == m_Stage.Text)
                    Book.Stage = stage;
            }

            Book.Version = m_Version.Text;
		}
		#endregion
		#region Cmd: cmdIncrementStage
		private void cmdIncrementStage(object sender, System.EventArgs e)
		{
			// Make sure incrementing is possible
            if (m_iStage >= DB.TeamSettings.Stages.Count)
				return;

			// Increment the stage
			m_iStage++;
            var stage = DB.TeamSettings.Stages[m_iStage];
			Debug.Assert(null != stage);
			m_Stage.Text = stage.LocalizedName;

			// Reset the Version
			m_Version.Text = "A";

			// Disable all of the buttons
			DisableAllButtons();
		}
		#endregion
		#region Cmd: cmdIncrementVersion
		private void cmdIncrementVersion(object sender, System.EventArgs e)
		{
			// Increment the Version
			m_Version.Text = ((char)( (int)m_Version.Text[0] + 1)).ToString();

			// Disable all of the buttons
			DisableAllButtons();
		}
		#endregion
		#region Cmd: cmdHelp(...) - Help button clicked
		private void cmdHelp(object sender, System.EventArgs e)
		{
			HelpSystem.ShowTopic(HelpSystem.Topic.kTranslationStages);
		}
		#endregion

		// Methods ---------------------------------------------------------------------------
		#region Method: void DisableAllButtons()
		private void DisableAllButtons()
		{
			m_btnStage.Enabled   = false;
			m_btnVersion.Enabled = false;
		}
		#endregion

	}
}
