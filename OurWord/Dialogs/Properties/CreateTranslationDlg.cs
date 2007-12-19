/**********************************************************************************************
 * Project: Our Word!
 * File:    CreateTranslationDlg.cs
 * Author:  John Wimbish
 * Created: 05 Jan 2005
 * Purpose: Initialize a new, blank set of translation settings.
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
using JWdb;
using OurWord;
using OurWord.DataModel;
using OurWord.Dialogs;
using OurWord.View;
#endregion

namespace OurWord.Dialogs
{
	public class DialiogCreateTranslation : System.Windows.Forms.Form
	{
		// Attrs -----------------------------------------------------------------------------
		#region Attr{g/s}: string TranslationName
		public string TranslationName
		{
			get
			{
				return m_editName.Text;
			}
			set
			{
				m_editName.Text = value;
			}
		}
		#endregion
		#region Attr{g/s}: string Abbreviation
		public string Abbreviation
		{
			get
			{
				return m_editAbbrev.Text;
			}
			set
			{
				m_editAbbrev.Text = value;
			}
		}
		#endregion
		#region Attr{g/s}: string SettingsText
		public string SettingsText
		{
			get
			{
				return m_textSettings.Text;
			}
			set
			{
				m_textSettings.Text = value;
			}
		}
		#endregion
		#region Attr{g/s}: string SettingsPath
		public string SettingsPath
		{
			get
			{
				return m_sSettingsPath;
			}
			set
			{
				m_sSettingsPath = value;
				SettingsText = JWU.PathEllipses( SettingsPath, 35);
			}
		}
		private string m_sSettingsPath = "";
		#endregion

		// Scaffolding -----------------------------------------------------------------------
		#region Dialog Controls
		private System.Windows.Forms.Button m_btnHelp;
		private System.Windows.Forms.Button m_btnOK;
		private System.Windows.Forms.Button m_btnCancel;
		private System.Windows.Forms.Label m_lblName;
		private System.Windows.Forms.Label m_lblAbbrev;
		private System.Windows.Forms.Label m_lblSettings;
		private System.Windows.Forms.TextBox m_editName;
		private System.Windows.Forms.TextBox m_editAbbrev;
		private System.Windows.Forms.Label m_textSettings;
		private System.Windows.Forms.Button m_btnSettings;
		private System.Windows.Forms.Label m_lblNameHelp;
		private System.Windows.Forms.Label m_lblAbbrevHelp;
		private System.Windows.Forms.Label m_lblSettingsHelp;
		private System.ComponentModel.Container components = null;
		#endregion
		#region Constructor()
		public DialiogCreateTranslation()
		{
			// Required for Windows Form Designer support
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}
		#endregion
		#region Method: void Dispose(...) -  Clean up any resources being used.
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DialiogCreateTranslation));
            this.m_btnHelp = new System.Windows.Forms.Button();
            this.m_btnOK = new System.Windows.Forms.Button();
            this.m_btnCancel = new System.Windows.Forms.Button();
            this.m_lblName = new System.Windows.Forms.Label();
            this.m_lblAbbrev = new System.Windows.Forms.Label();
            this.m_lblSettings = new System.Windows.Forms.Label();
            this.m_editName = new System.Windows.Forms.TextBox();
            this.m_editAbbrev = new System.Windows.Forms.TextBox();
            this.m_textSettings = new System.Windows.Forms.Label();
            this.m_btnSettings = new System.Windows.Forms.Button();
            this.m_lblNameHelp = new System.Windows.Forms.Label();
            this.m_lblAbbrevHelp = new System.Windows.Forms.Label();
            this.m_lblSettingsHelp = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // m_btnHelp
            // 
            this.m_btnHelp.Image = ((System.Drawing.Image)(resources.GetObject("m_btnHelp.Image")));
            this.m_btnHelp.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.m_btnHelp.Location = new System.Drawing.Point(256, 264);
            this.m_btnHelp.Name = "m_btnHelp";
            this.m_btnHelp.Size = new System.Drawing.Size(75, 23);
            this.m_btnHelp.TabIndex = 23;
            this.m_btnHelp.Text = "Help";
            this.m_btnHelp.Click += new System.EventHandler(this.cmdHelp);
            // 
            // m_btnOK
            // 
            this.m_btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.m_btnOK.Location = new System.Drawing.Point(96, 264);
            this.m_btnOK.Name = "m_btnOK";
            this.m_btnOK.Size = new System.Drawing.Size(75, 23);
            this.m_btnOK.TabIndex = 22;
            this.m_btnOK.Text = "OK";
            // 
            // m_btnCancel
            // 
            this.m_btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.m_btnCancel.Location = new System.Drawing.Point(176, 264);
            this.m_btnCancel.Name = "m_btnCancel";
            this.m_btnCancel.Size = new System.Drawing.Size(75, 23);
            this.m_btnCancel.TabIndex = 24;
            this.m_btnCancel.Text = "Cancel";
            // 
            // m_lblName
            // 
            this.m_lblName.Location = new System.Drawing.Point(8, 16);
            this.m_lblName.Name = "m_lblName";
            this.m_lblName.Size = new System.Drawing.Size(108, 23);
            this.m_lblName.TabIndex = 25;
            this.m_lblName.Text = "Translation Name:";
            this.m_lblName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // m_lblAbbrev
            // 
            this.m_lblAbbrev.Location = new System.Drawing.Point(8, 88);
            this.m_lblAbbrev.Name = "m_lblAbbrev";
            this.m_lblAbbrev.Size = new System.Drawing.Size(108, 23);
            this.m_lblAbbrev.TabIndex = 26;
            this.m_lblAbbrev.Text = "Abbreviation:";
            this.m_lblAbbrev.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // m_lblSettings
            // 
            this.m_lblSettings.Location = new System.Drawing.Point(8, 176);
            this.m_lblSettings.Name = "m_lblSettings";
            this.m_lblSettings.Size = new System.Drawing.Size(108, 23);
            this.m_lblSettings.TabIndex = 27;
            this.m_lblSettings.Text = "Settings File:";
            this.m_lblSettings.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // m_editName
            // 
            this.m_editName.Location = new System.Drawing.Point(122, 16);
            this.m_editName.Name = "m_editName";
            this.m_editName.Size = new System.Drawing.Size(310, 20);
            this.m_editName.TabIndex = 28;
            this.m_editName.TextChanged += new System.EventHandler(this.cmdTranslationNameEdited);
            // 
            // m_editAbbrev
            // 
            this.m_editAbbrev.Location = new System.Drawing.Point(122, 88);
            this.m_editAbbrev.Name = "m_editAbbrev";
            this.m_editAbbrev.Size = new System.Drawing.Size(118, 20);
            this.m_editAbbrev.TabIndex = 29;
            // 
            // m_textSettings
            // 
            this.m_textSettings.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.m_textSettings.Location = new System.Drawing.Point(122, 176);
            this.m_textSettings.Name = "m_textSettings";
            this.m_textSettings.Size = new System.Drawing.Size(230, 23);
            this.m_textSettings.TabIndex = 30;
            // 
            // m_btnSettings
            // 
            this.m_btnSettings.Location = new System.Drawing.Point(360, 176);
            this.m_btnSettings.Name = "m_btnSettings";
            this.m_btnSettings.Size = new System.Drawing.Size(75, 23);
            this.m_btnSettings.TabIndex = 31;
            this.m_btnSettings.Text = "Browse...";
            this.m_btnSettings.Click += new System.EventHandler(this.cmdBrowse);
            // 
            // m_lblNameHelp
            // 
            this.m_lblNameHelp.Location = new System.Drawing.Point(32, 40);
            this.m_lblNameHelp.Name = "m_lblNameHelp";
            this.m_lblNameHelp.Size = new System.Drawing.Size(400, 32);
            this.m_lblNameHelp.TabIndex = 32;
            this.m_lblNameHelp.Text = "Enter the name of the language, e.g., \"Spanish\", \"Lakota Sioux.\"";
            // 
            // m_lblAbbrevHelp
            // 
            this.m_lblAbbrevHelp.Location = new System.Drawing.Point(32, 112);
            this.m_lblAbbrevHelp.Name = "m_lblAbbrevHelp";
            this.m_lblAbbrevHelp.Size = new System.Drawing.Size(400, 40);
            this.m_lblAbbrevHelp.TabIndex = 33;
            this.m_lblAbbrevHelp.Text = "A short abbreviation of rhe language name. The Ethnologue offers suitable 3-lette" +
                "r abbreviations for most of the world\'s languages.";
            // 
            // m_lblSettingsHelp
            // 
            this.m_lblSettingsHelp.Location = new System.Drawing.Point(32, 200);
            this.m_lblSettingsHelp.Name = "m_lblSettingsHelp";
            this.m_lblSettingsHelp.Size = new System.Drawing.Size(400, 40);
            this.m_lblSettingsHelp.TabIndex = 34;
            this.m_lblSettingsHelp.Text = "Navigate to the folder where you want to store the settings file. The file name u" +
                "sually contains the name of the language, e.g., \"Spanish.otrans.\"";
            // 
            // DialiogCreateTranslation
            // 
            this.AcceptButton = this.m_btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.m_btnCancel;
            this.ClientSize = new System.Drawing.Size(442, 301);
            this.Controls.Add(this.m_lblSettingsHelp);
            this.Controls.Add(this.m_lblAbbrevHelp);
            this.Controls.Add(this.m_lblNameHelp);
            this.Controls.Add(this.m_btnSettings);
            this.Controls.Add(this.m_textSettings);
            this.Controls.Add(this.m_editAbbrev);
            this.Controls.Add(this.m_editName);
            this.Controls.Add(this.m_lblSettings);
            this.Controls.Add(this.m_lblAbbrev);
            this.Controls.Add(this.m_lblName);
            this.Controls.Add(this.m_btnCancel);
            this.Controls.Add(this.m_btnHelp);
            this.Controls.Add(this.m_btnOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DialiogCreateTranslation";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Create Translation";
            this.Closing += new System.ComponentModel.CancelEventHandler(this.cmdClosing);
            this.Load += new System.EventHandler(this.cmdLoad);
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		// Methods ---------------------------------------------------------------------------
		#region Method: void SetFocusToTranslationName()
		void SetFocusToTranslationName()
		{
			m_editName.Focus();
			m_editName.Select();
		}
		#endregion
		#region Method: void SetFocusToAbbreviation()
		void SetFocusToAbbreviation()
		{
			m_editAbbrev.Focus();
			m_editAbbrev.Select();
		}
		#endregion
		#region Method: void SetFocusToBrowse()
		public void SetFocusToBrowse()
		{
			m_btnSettings.Focus();
			m_btnSettings.Select();
		}
		#endregion
		#region Method: void ValidateData() - Don't allow the dlg to exit if data not right
		public bool ValidateData()
			// I do this as a separate method, rather than having it in the OnClosing handler,
			// so that the unit tests can call it easily.
		{
			// The translation's name should be a valid, non-zero-lengthed name
			if (TranslationName.Length == 0)
			{
                Messages.TranslationNeedsName();
				SetFocusToTranslationName();
				return false;
			}

			// The translation's abbreviation must be non-zero length
			if (Abbreviation.Length == 0)
			{
                Messages.TranslationNeedsAbbrev();
				SetFocusToAbbreviation();
				return false;
			}

			// The translation's path name should be to a valid file.
			if ( SettingsPath.Length == 0 || Path.GetFileName(SettingsPath).Length == 0)
			{
                Messages.TranslationNeedsPath();               
                SetFocusToBrowse();
				return false;
			}

			// No problems found
			return true;
		}
		#endregion

		// Command Handlers ------------------------------------------------------------------
		#region Handler: cmdLoad - localize the controls
		private void cmdLoad(object sender, System.EventArgs e)
		{
			// Localizations
            Control[] vExclude = { };
            LocDB.Localize(this, vExclude);

			// Place focus in the Name field
			SetFocusToTranslationName();
		}
		#endregion
		#region Handler: cmdClosing - abort if data is not acceptable
		private void cmdClosing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			// We're only interested in further processing if the user has hit the OK
			// button, signaling he is done and wishes to save his results.
			if (DialogResult != DialogResult.OK)
				return;

			// Check for valid data; if not, don't close
			if (false == ValidateData())
			{
				e.Cancel = true;
				return;
			}
		}
		#endregion
		#region Handler: cmdTranslationNameEdited - Provide a default Abbreviation
		private void cmdTranslationNameEdited(object sender, System.EventArgs e)
		{
			int cAbbrevLength = 3;

			if (TranslationName.Length == 0)
				return;				

			// Get the default abbreviation as the first three letters of the name
			string sDefaultAbbrev = "";
			if (TranslationName.Length > 0)
			{
				foreach(char ch in TranslationName)
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
			for(; i < Abbreviation.Length && i < sDefaultAbbrev.Length; i++)
			{
				if ( Abbreviation[i] != sDefaultAbbrev[i] )
					break;
			}
			if (i == Abbreviation.Length && i < sDefaultAbbrev.Length)
				Abbreviation = sDefaultAbbrev;
		}
		#endregion
		#region Handler: cmdBrowse - Get a file name for the settings file
		private void cmdBrowse(object sender, System.EventArgs e)
		{
			SaveFileDialog dlg = new SaveFileDialog();

			dlg.Filter = StrRes.FileFilterTranslation;
			dlg.FilterIndex = 0;
			dlg.InitialDirectory = G.BrowseDirectory;

			dlg.Title = StrRes.DlgSaveTranslationAs_Title;

			if (TranslationName.Length > 0)
				dlg.FileName = TranslationName + ".oTrans";
			else
				dlg.FileName = StrRes.DefaultFileName_Translation;

			if (DialogResult.OK == dlg.ShowDialog(this))
			{
				SettingsPath = dlg.FileName;
			}
			G.BrowseDirectory = Path.GetDirectoryName( dlg.FileName );
		}
		#endregion
		#region Handler: cmdHelp
		private void cmdHelp(object sender, System.EventArgs e)
		{
			HelpSystem.Show_DlgCreateTranslation();
		}
		#endregion
	}
}
