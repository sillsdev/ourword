/**********************************************************************************************
 * Project: Our Word!
 * File:    CreateTranslationDlg.cs
 * Author:  John Wimbish
 * Created: 05 Jan 2005
 * Purpose: Initialize a new, blank set of translation settings.
 * Legal:   Copyright (c) 2005-09, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
#region Header: Using, etc.
using System;
using System.Collections;
using System.Collections.Generic;
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
using JWdb.DataModel;
using OurWord.Dialogs;
using OurWord.Utilities;
using OurWord.Layouts;
#endregion

namespace OurWord.Dialogs
{
	public class DlgCreateTranslation : System.Windows.Forms.Form
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

		// Scaffolding -----------------------------------------------------------------------
		#region Dialog Controls

        private System.Windows.Forms.Button m_btnOK;
		private System.Windows.Forms.Button m_btnCancel;
        private System.Windows.Forms.Label m_lblName;
        private System.Windows.Forms.TextBox m_editName;
        private System.Windows.Forms.Label m_lblNameHelp;
		private System.Windows.Forms.Label m_lblMore;
		private System.ComponentModel.Container components = null;
		#endregion
		#region Constructor()
		public DlgCreateTranslation()
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
            this.m_btnOK = new System.Windows.Forms.Button();
            this.m_btnCancel = new System.Windows.Forms.Button();
            this.m_lblName = new System.Windows.Forms.Label();
            this.m_editName = new System.Windows.Forms.TextBox();
            this.m_lblNameHelp = new System.Windows.Forms.Label();
            this.m_lblMore = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // m_btnOK
            // 
            this.m_btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.m_btnOK.Location = new System.Drawing.Point(142, 165);
            this.m_btnOK.Name = "m_btnOK";
            this.m_btnOK.Size = new System.Drawing.Size(75, 23);
            this.m_btnOK.TabIndex = 22;
            this.m_btnOK.Text = "OK";
            // 
            // m_btnCancel
            // 
            this.m_btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.m_btnCancel.Location = new System.Drawing.Point(222, 165);
            this.m_btnCancel.Name = "m_btnCancel";
            this.m_btnCancel.Size = new System.Drawing.Size(75, 23);
            this.m_btnCancel.TabIndex = 24;
            this.m_btnCancel.Text = "Cancel";
            // 
            // m_lblName
            // 
            this.m_lblName.Location = new System.Drawing.Point(12, 60);
            this.m_lblName.Name = "m_lblName";
            this.m_lblName.Size = new System.Drawing.Size(108, 23);
            this.m_lblName.TabIndex = 25;
            this.m_lblName.Text = "Translation Name:";
            this.m_lblName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // m_editName
            // 
            this.m_editName.Location = new System.Drawing.Point(120, 63);
            this.m_editName.Name = "m_editName";
            this.m_editName.Size = new System.Drawing.Size(310, 20);
            this.m_editName.TabIndex = 28;
            // 
            // m_lblNameHelp
            // 
            this.m_lblNameHelp.Location = new System.Drawing.Point(12, 9);
            this.m_lblNameHelp.Name = "m_lblNameHelp";
            this.m_lblNameHelp.Size = new System.Drawing.Size(418, 42);
            this.m_lblNameHelp.TabIndex = 32;
            this.m_lblNameHelp.Text = "Enter the name of the language, e.g., \"Español\", \"Bahasa Indonesia\", or \"Lakota S" +
                "ioux.\"";
            this.m_lblNameHelp.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // m_lblMore
            // 
            this.m_lblMore.ForeColor = System.Drawing.SystemColors.ControlText;
            this.m_lblMore.Location = new System.Drawing.Point(12, 98);
            this.m_lblMore.Name = "m_lblMore";
            this.m_lblMore.Size = new System.Drawing.Size(418, 56);
            this.m_lblMore.TabIndex = 34;
            this.m_lblMore.Text = "Most people choose the name that the local speakers use, rather than an, e.g., an" +
                " anglicized or national language name, as this name will appear throughout OurWo" +
                "rd\'s user interface.";
            // 
            // DialogCreateTranslation
            // 
            this.AcceptButton = this.m_btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.m_btnCancel;
            this.ClientSize = new System.Drawing.Size(442, 202);
            this.Controls.Add(this.m_lblMore);
            this.Controls.Add(this.m_lblNameHelp);
            this.Controls.Add(this.m_editName);
            this.Controls.Add(this.m_lblName);
            this.Controls.Add(this.m_btnCancel);
            this.Controls.Add(this.m_btnOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DialogCreateTranslation";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Create Translation";
            this.Load += new System.EventHandler(this.cmdLoad);
            this.Closing += new System.ComponentModel.CancelEventHandler(this.cmdClosing);
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
        #region Method: void SetErrorText(sLocID, sEnglishDefaultText)
        void SetErrorText(string sID, string sText)
        {
            m_lblMore.ForeColor = Color.Red;
            m_lblMore.Text = Loc.GetString(sID, sText);
        }
        #endregion
        #region Method: void ValidateData() - Don't allow the dlg to exit if data not right
        public bool ValidateData()
			// I do this as a separate method, rather than having it in the OnClosing handler,
			// so that the unit tests can call it easily.
		{
            string sProposedName = TranslationName.Trim();

			// The translation's name should be a valid, non-zero-lengthed name
            if (string.IsNullOrEmpty(sProposedName))
			{
                SetErrorText("msgTranslationNeedsName",
                    "Please enter a name for the translation.");
                TranslationName = ""; // Get rid of any spaces that might still be there
				SetFocusToTranslationName();
				return false;
			}

            // The translation's name cannot conflict with other names in the cluster
            ClusterInfo ci = ClusterList.FindClusterInfo(DB.TeamSettings.DisplayName);
            var vTranslations = ci.GetClusterLanguageList(false);
            string sProposedNameUp = sProposedName.ToUpperInvariant();
            foreach (string s in vTranslations)
            {
                string sUp = s.ToUpperInvariant();

                if (sUp.CompareTo(sProposedNameUp) == 0)
                {
                    SetErrorText("msgTranslationAlreadyExists", 
                        "A translation of this name already exists, please use a different name.");
                    SetFocusToTranslationName();
                    return false;
                }
            }

            // The translation's name must be made of characters, digits, and optional internal
            // blank space.
            foreach (char ch in sProposedName)
            {
                if (!char.IsLetter(ch) && ch != ' ')
                {
                    SetErrorText("msgInvalidCharsInTranslationName",
                        "A translation name can be made up only of letters, digits and space.");
                    SetFocusToTranslationName();
                    return false;
                }
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

            // Remove any leading/trailing whitespace
            TranslationName = TranslationName.Trim();
		}
		#endregion
	}
}
