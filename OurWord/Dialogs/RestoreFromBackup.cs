/**********************************************************************************************
 * Project: Our Word!
 * File:    RestoreFromBackup.cs
 * Author:  John Wimbish
 * Created: 6 Nov 2004
 * Purpose: Restore the current book from a backup file.
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

using JWTools;
using OurWord;
using OurWord.DataModel;
using OurWord.Dialogs;
using OurWord.View;
#endregion


namespace OurWord.Dialogs
{
	public class DialogRestoreFromBackup : System.Windows.Forms.Form
	{
		#region Attr{g}: DBook Book - the book to be restored
		DBook Book
		{
			get
			{
				return m_Book;
			}
		}
		DBook m_Book;
		#endregion
		#region Attr{g/s}: string BackupPathName - gets/sets the value to the control
		public string BackupPathName
		{
			get
			{
				return m_sBackupPathName;
			}
			set
			{
				m_labelBackupFilename.Text = Path.GetFileName(value);
				m_sBackupPathName = value;
			}
		}
		string m_sBackupPathName = "";
		#endregion

		// Scaffolding -----------------------------------------------------------------------
		#region Dialog Controls
		private System.Windows.Forms.Button m_btnOK;
		private System.Windows.Forms.Button m_btnCancel;
		private System.Windows.Forms.Label m_lblExplanation;
		private System.Windows.Forms.Label m_lblBook;
		private System.Windows.Forms.Label m_labelBook;
		private System.Windows.Forms.Label m_lblFilename;
		private System.Windows.Forms.Label m_labelFilename;
		private System.Windows.Forms.Label m_lblRestoreFrom;
		private System.Windows.Forms.Button m_btnBrowse;
		private System.Windows.Forms.Label m_lblBackupFilename;
		private System.Windows.Forms.Label m_labelBackupFilename;
		private System.Windows.Forms.Button m_btnHelp;

		// Required designer variable.
		private System.ComponentModel.Container components = null;
		#endregion
		#region Constructor()
		public DialogRestoreFromBackup(DBook book)
		{
			// Required for Windows Form Designer support
			InitializeComponent();

			m_Book = book;
		}
		#endregion
		#region void Dispose() -  Clean up any resources being used.
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DialogRestoreFromBackup));
            this.m_btnOK = new System.Windows.Forms.Button();
            this.m_btnCancel = new System.Windows.Forms.Button();
            this.m_lblExplanation = new System.Windows.Forms.Label();
            this.m_lblBook = new System.Windows.Forms.Label();
            this.m_labelBook = new System.Windows.Forms.Label();
            this.m_lblFilename = new System.Windows.Forms.Label();
            this.m_labelFilename = new System.Windows.Forms.Label();
            this.m_lblRestoreFrom = new System.Windows.Forms.Label();
            this.m_btnBrowse = new System.Windows.Forms.Button();
            this.m_lblBackupFilename = new System.Windows.Forms.Label();
            this.m_labelBackupFilename = new System.Windows.Forms.Label();
            this.m_btnHelp = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // m_btnOK
            // 
            this.m_btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.m_btnOK.Location = new System.Drawing.Point(88, 224);
            this.m_btnOK.Name = "m_btnOK";
            this.m_btnOK.Size = new System.Drawing.Size(75, 23);
            this.m_btnOK.TabIndex = 0;
            this.m_btnOK.Text = "Restore";
            // 
            // m_btnCancel
            // 
            this.m_btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.m_btnCancel.Location = new System.Drawing.Point(176, 224);
            this.m_btnCancel.Name = "m_btnCancel";
            this.m_btnCancel.Size = new System.Drawing.Size(75, 23);
            this.m_btnCancel.TabIndex = 1;
            this.m_btnCancel.Text = "Cancel";
            // 
            // m_lblExplanation
            // 
            this.m_lblExplanation.Location = new System.Drawing.Point(8, 8);
            this.m_lblExplanation.Name = "m_lblExplanation";
            this.m_lblExplanation.Size = new System.Drawing.Size(408, 48);
            this.m_lblExplanation.TabIndex = 2;
            this.m_lblExplanation.Text = "This will restore a previous version of the book, replacing the current version. " +
                "It will overwrite the current version, you will no longer have it on your comput" +
                "er.";
            // 
            // m_lblBook
            // 
            this.m_lblBook.Location = new System.Drawing.Point(8, 56);
            this.m_lblBook.Name = "m_lblBook";
            this.m_lblBook.Size = new System.Drawing.Size(120, 40);
            this.m_lblBook.TabIndex = 3;
            this.m_lblBook.Text = "Book to be Restored:";
            this.m_lblBook.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // m_labelBook
            // 
            this.m_labelBook.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.m_labelBook.Location = new System.Drawing.Point(128, 64);
            this.m_labelBook.Name = "m_labelBook";
            this.m_labelBook.Size = new System.Drawing.Size(280, 23);
            this.m_labelBook.TabIndex = 4;
            this.m_labelBook.Text = "(BookName)";
            this.m_labelBook.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // m_lblFilename
            // 
            this.m_lblFilename.Location = new System.Drawing.Point(8, 96);
            this.m_lblFilename.Name = "m_lblFilename";
            this.m_lblFilename.Size = new System.Drawing.Size(120, 40);
            this.m_lblFilename.TabIndex = 5;
            this.m_lblFilename.Text = "Current Filename:";
            this.m_lblFilename.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // m_labelFilename
            // 
            this.m_labelFilename.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.m_labelFilename.Location = new System.Drawing.Point(128, 104);
            this.m_labelFilename.Name = "m_labelFilename";
            this.m_labelFilename.Size = new System.Drawing.Size(280, 23);
            this.m_labelFilename.TabIndex = 6;
            this.m_labelFilename.Text = "(filename)";
            this.m_labelFilename.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // m_lblRestoreFrom
            // 
            this.m_lblRestoreFrom.Location = new System.Drawing.Point(8, 152);
            this.m_lblRestoreFrom.Name = "m_lblRestoreFrom";
            this.m_lblRestoreFrom.Size = new System.Drawing.Size(400, 23);
            this.m_lblRestoreFrom.TabIndex = 7;
            this.m_lblRestoreFrom.Text = "Previously Backed-Up File to be Restored:";
            this.m_lblRestoreFrom.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // m_btnBrowse
            // 
            this.m_btnBrowse.Location = new System.Drawing.Point(336, 176);
            this.m_btnBrowse.Name = "m_btnBrowse";
            this.m_btnBrowse.Size = new System.Drawing.Size(75, 23);
            this.m_btnBrowse.TabIndex = 8;
            this.m_btnBrowse.Text = "Browse...";
            this.m_btnBrowse.Click += new System.EventHandler(this.cmdBrowse);
            // 
            // m_lblBackupFilename
            // 
            this.m_lblBackupFilename.Location = new System.Drawing.Point(8, 176);
            this.m_lblBackupFilename.Name = "m_lblBackupFilename";
            this.m_lblBackupFilename.Size = new System.Drawing.Size(120, 23);
            this.m_lblBackupFilename.TabIndex = 9;
            this.m_lblBackupFilename.Text = "Backup Filename:";
            this.m_lblBackupFilename.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // m_labelBackupFilename
            // 
            this.m_labelBackupFilename.BackColor = System.Drawing.SystemColors.Window;
            this.m_labelBackupFilename.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.m_labelBackupFilename.Location = new System.Drawing.Point(128, 176);
            this.m_labelBackupFilename.Name = "m_labelBackupFilename";
            this.m_labelBackupFilename.Size = new System.Drawing.Size(208, 23);
            this.m_labelBackupFilename.TabIndex = 10;
            this.m_labelBackupFilename.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // m_btnHelp
            // 
            this.m_btnHelp.Image = ((System.Drawing.Image)(resources.GetObject("m_btnHelp.Image")));
            this.m_btnHelp.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.m_btnHelp.Location = new System.Drawing.Point(264, 224);
            this.m_btnHelp.Name = "m_btnHelp";
            this.m_btnHelp.Size = new System.Drawing.Size(75, 23);
            this.m_btnHelp.TabIndex = 11;
            this.m_btnHelp.Text = "Help...";
            this.m_btnHelp.Click += new System.EventHandler(this.cmdHelp);
            // 
            // DialogRestoreFromBackup
            // 
            this.AcceptButton = this.m_btnOK;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.CancelButton = this.m_btnCancel;
            this.ClientSize = new System.Drawing.Size(418, 253);
            this.Controls.Add(this.m_btnHelp);
            this.Controls.Add(this.m_labelBackupFilename);
            this.Controls.Add(this.m_lblBackupFilename);
            this.Controls.Add(this.m_btnBrowse);
            this.Controls.Add(this.m_lblRestoreFrom);
            this.Controls.Add(this.m_labelFilename);
            this.Controls.Add(this.m_lblFilename);
            this.Controls.Add(this.m_labelBook);
            this.Controls.Add(this.m_lblBook);
            this.Controls.Add(this.m_lblExplanation);
            this.Controls.Add(this.m_btnCancel);
            this.Controls.Add(this.m_btnOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DialogRestoreFromBackup";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Restore From Backup";
            this.Closing += new System.ComponentModel.CancelEventHandler(this.cmdClosing);
            this.Load += new System.EventHandler(this.cmdLoad);
            this.ResumeLayout(false);

		}
		#endregion

		// Command Handlers ------------------------------------------------------------------
		#region Cmd: cmdLoad
		private void cmdLoad(object sender, System.EventArgs e)
		{
            // Label text in the appropriate language
            Control[] vExclude = { m_labelBook, m_labelFilename };
            LocDB.Localize(this, vExclude);

			// Load the information about the book we're about to replace
			m_labelBook.Text = Book.Translation.DisplayName + " - " + Book.DisplayName;
            m_labelFilename.Text = Path.GetFileName(Book.AbsolutePathName);

			// Backup path name
			m_labelBackupFilename.Text = BackupPathName;

			// Disable the Restore button until we browse for a file
			m_btnOK.Enabled = false;
		}
		#endregion
		#region Cmd: cmdClosing
		private void cmdClosing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			// We're only interested in further processing if the user has hit the OK
			// button, signallying he is done and wishes to save his results.
			if (DialogResult != DialogResult.OK)
				return;

			// Make sure we really, really want to do the restore.
			bool bGoAhead = Messages.VerifyRestore(
                    Path.GetFileName(Book.AbsolutePathName), 
					Path.GetFileName(BackupPathName));
			if (!bGoAhead)
				DialogResult = DialogResult.Cancel;
		}
		#endregion
		#region Cmd: cmdBrowse
		private void cmdBrowse(object sender, System.EventArgs e)
		{
			OpenFileDialog dlg = new OpenFileDialog();
			dlg.Multiselect = false;

            // Dialog Title
            dlg.Title = G.GetLoc_Files("BrowseForBackupFile", "Browse for Backup File");

			// Browse directory
			if (BackupSystem.Enabled)
				dlg.InitialDirectory = BackupSystem.RegistryBackupFolder;
			else
				dlg.InitialDirectory = Environment.GetFolderPath(
					Environment.SpecialFolder.Personal);

			// First part of file name
			DTeamSettings ts = G.TeamSettings;
			string sFileNameFirstPart = Book.Translation.LanguageAbbrev;
            sFileNameFirstPart += ("-" + G.GetLoc_BookAbbrev(Book.BookAbbrev));

			// Filter
			dlg.Filter = "Backed-up Files (*.db)|" + sFileNameFirstPart + "-*.db|" + 
				"All files (*.*)|*.*";
			dlg.FilterIndex = 1;

			// Run the dialog
			if (DialogResult.OK == dlg.ShowDialog(this))
			{
				BackupPathName = dlg.FileName;
				m_btnOK.Enabled = true;
			}
		}
		#endregion
		#region Cmd: cmdHelp(...) - Help button clicked
		private void cmdHelp(object sender, System.EventArgs e)
		{
			HelpSystem.ShowTopic(HelpSystem.Topic.kAutoBackup);
		}
		#endregion
	}
}
