/**********************************************************************************************
 * Project: Our Word!
 * File:    OpenTranslationDlg.cs
 * Author:  John Wimbish
 * Created: 13 Jan 2005
 * Purpose: Open access to an existing file of translation settings.
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
	public class DialogOpenTranslation : System.Windows.Forms.Form
	{
		// Attrs -----------------------------------------------------------------------------
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
		#region Dialog Constrols
		private System.Windows.Forms.Button m_btnCancel;
		private System.Windows.Forms.Button m_btnHelp;
		private System.Windows.Forms.Button m_btnOK;
		private System.Windows.Forms.Label m_lblSettingsHelp;
		private System.Windows.Forms.Label m_lblSettings;
		private System.Windows.Forms.Label m_textSettings;
		private System.Windows.Forms.Button m_btnSettings;
		// Required designer variable.
		private System.ComponentModel.Container components = null;
		#endregion
		#region Constructor()
		public DialogOpenTranslation()
		{
			// Required for Windows Form Designer support
			InitializeComponent();
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DialogOpenTranslation));
            this.m_btnCancel = new System.Windows.Forms.Button();
            this.m_btnHelp = new System.Windows.Forms.Button();
            this.m_btnOK = new System.Windows.Forms.Button();
            this.m_lblSettingsHelp = new System.Windows.Forms.Label();
            this.m_lblSettings = new System.Windows.Forms.Label();
            this.m_textSettings = new System.Windows.Forms.Label();
            this.m_btnSettings = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // m_btnCancel
            // 
            this.m_btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.m_btnCancel.Location = new System.Drawing.Point(184, 96);
            this.m_btnCancel.Name = "m_btnCancel";
            this.m_btnCancel.Size = new System.Drawing.Size(75, 23);
            this.m_btnCancel.TabIndex = 27;
            this.m_btnCancel.Text = "Cancel";
            // 
            // m_btnHelp
            // 
            this.m_btnHelp.Image = ((System.Drawing.Image)(resources.GetObject("m_btnHelp.Image")));
            this.m_btnHelp.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.m_btnHelp.Location = new System.Drawing.Point(264, 96);
            this.m_btnHelp.Name = "m_btnHelp";
            this.m_btnHelp.Size = new System.Drawing.Size(75, 23);
            this.m_btnHelp.TabIndex = 26;
            this.m_btnHelp.Text = "Help";
            this.m_btnHelp.Click += new System.EventHandler(this.cmdHelp);
            // 
            // m_btnOK
            // 
            this.m_btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.m_btnOK.Location = new System.Drawing.Point(104, 96);
            this.m_btnOK.Name = "m_btnOK";
            this.m_btnOK.Size = new System.Drawing.Size(75, 23);
            this.m_btnOK.TabIndex = 25;
            this.m_btnOK.Text = "OK";
            // 
            // m_lblSettingsHelp
            // 
            this.m_lblSettingsHelp.Location = new System.Drawing.Point(8, 8);
            this.m_lblSettingsHelp.Name = "m_lblSettingsHelp";
            this.m_lblSettingsHelp.Size = new System.Drawing.Size(424, 40);
            this.m_lblSettingsHelp.TabIndex = 35;
            this.m_lblSettingsHelp.Text = "Navigate to the file where the settings are stored. The file name usually contain" +
                "s the name of the language, e.g., \"Spanish.otrans.\"";
            // 
            // m_lblSettings
            // 
            this.m_lblSettings.Location = new System.Drawing.Point(8, 48);
            this.m_lblSettings.Name = "m_lblSettings";
            this.m_lblSettings.Size = new System.Drawing.Size(128, 23);
            this.m_lblSettings.TabIndex = 36;
            this.m_lblSettings.Text = "Settings File Name:";
            this.m_lblSettings.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // m_textSettings
            // 
            this.m_textSettings.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.m_textSettings.Location = new System.Drawing.Point(136, 48);
            this.m_textSettings.Name = "m_textSettings";
            this.m_textSettings.Size = new System.Drawing.Size(216, 23);
            this.m_textSettings.TabIndex = 37;
            // 
            // m_btnSettings
            // 
            this.m_btnSettings.Location = new System.Drawing.Point(352, 48);
            this.m_btnSettings.Name = "m_btnSettings";
            this.m_btnSettings.Size = new System.Drawing.Size(75, 23);
            this.m_btnSettings.TabIndex = 38;
            this.m_btnSettings.Text = "Browse...";
            this.m_btnSettings.Click += new System.EventHandler(this.cmdBrowse);
            // 
            // DialogOpenTranslation
            // 
            this.AcceptButton = this.m_btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoSize = true;
            this.CancelButton = this.m_btnCancel;
            this.ClientSize = new System.Drawing.Size(442, 133);
            this.ControlBox = false;
            this.Controls.Add(this.m_btnSettings);
            this.Controls.Add(this.m_textSettings);
            this.Controls.Add(this.m_lblSettings);
            this.Controls.Add(this.m_lblSettingsHelp);
            this.Controls.Add(this.m_btnCancel);
            this.Controls.Add(this.m_btnHelp);
            this.Controls.Add(this.m_btnOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DialogOpenTranslation";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Open Translation";
            this.Closing += new System.ComponentModel.CancelEventHandler(this.cmdClosing);
            this.Load += new System.EventHandler(this.cmdLoad);
            this.ResumeLayout(false);

		}
		#endregion

		// Methods ---------------------------------------------------------------------------
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
		#region Handler: cmdLoad - Localize the controls
		private void cmdLoad(object sender, System.EventArgs e)
		{
            // Localization
            LocDB.Localize(this, new Control[] { });
		}
		#endregion
		#region Handler: cmdClosing - validate the results
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
		#region Handler: cmdBrowse - determine which file to open
		private void cmdBrowse(object sender, System.EventArgs e)
		{
			OpenFileDialog dlg = new OpenFileDialog();

			// We only want to return a single file
			dlg.Multiselect = false;

			// Filter on oTrans files
            dlg.Filter = G.GetLoc_Files("TranslationFileFilter", 
                "Our Word Translation File (*.oTrans)|*.oTrans"); 
			dlg.FilterIndex = 0;

			// Retrieve Dialog Title from resources
            dlg.Title = G.GetLoc_Files("OpenTranslationDlgTitle",
                "Open Translation Definition File"); 

			// Default path
			dlg.InitialDirectory = G.BrowseDirectory;

			// Run the dialog
			if (DialogResult.OK == dlg.ShowDialog(this))
			{
				SettingsPath = dlg.FileName;
                try
                {
                    G.BrowseDirectory = Path.GetDirectoryName(dlg.FileName);
                }
                catch (Exception)
                {
                }
			}
		}
		#endregion
		#region Handler: cmdHelp
		private void cmdHelp(object sender, System.EventArgs e)
		{
            HelpSystem.ShowDefaultTopic();
		}
		#endregion
	}
}
