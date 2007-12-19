/**********************************************************************************************
 * Dll:     JWTools
 * File:    JW_FeaturesPassword.cs
 * Author:  John Wimbish
 * Created: 06 Dec 2004
 * Purpose: Password protection for the User Features dialog (moved to a separate file
 *           because of a compiler problem with the image on the Help button.
 * Legal:   Copyright (c) 2005-07, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/

#region Using
using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Win32;
#endregion

namespace JWTools
{
    #region FORM CLASS DialogPasswordProtect - Sets up password protection for features dlg
    public class DialogPasswordProtect : System.Windows.Forms.Form
	{
		// Scaffolding -----------------------------------------------------------------------
		#region Constructor()
		public DialogPasswordProtect()
		{
			InitializeComponent();

			// Set values and enabling
			m_checkProtect.Checked = DialogPassword.IsPasswordProtected;
			m_editPassword.Enabled = DialogPassword.IsPasswordProtected;

			if (DialogPassword.IsPasswordProtected)
			{
				m_editPassword.Text = DialogPassword.GetPassword();
			}
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
		// Required designer variable.
		private System.ComponentModel.Container components = null;

		private System.Windows.Forms.CheckBox m_checkProtect;
		private System.Windows.Forms.Label m_lblHelps;
		private System.Windows.Forms.Label m_lblPassword;
		private System.Windows.Forms.TextBox m_editPassword;
		private System.Windows.Forms.Button m_btnOK;
		private System.Windows.Forms.Button m_btnCancel;
		private System.Windows.Forms.Button m_btnHelp;

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DialogPasswordProtect));
            this.m_btnOK = new System.Windows.Forms.Button();
            this.m_btnCancel = new System.Windows.Forms.Button();
            this.m_checkProtect = new System.Windows.Forms.CheckBox();
            this.m_lblPassword = new System.Windows.Forms.Label();
            this.m_lblHelps = new System.Windows.Forms.Label();
            this.m_editPassword = new System.Windows.Forms.TextBox();
            this.m_btnHelp = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // m_btnOK
            // 
            this.m_btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.m_btnOK.Location = new System.Drawing.Point(32, 160);
            this.m_btnOK.Name = "m_btnOK";
            this.m_btnOK.Size = new System.Drawing.Size(75, 23);
            this.m_btnOK.TabIndex = 0;
            this.m_btnOK.Text = "OK";
            // 
            // m_btnCancel
            // 
            this.m_btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.m_btnCancel.Location = new System.Drawing.Point(112, 160);
            this.m_btnCancel.Name = "m_btnCancel";
            this.m_btnCancel.Size = new System.Drawing.Size(75, 23);
            this.m_btnCancel.TabIndex = 1;
            this.m_btnCancel.Text = "Cancel";
            // 
            // m_checkProtect
            // 
            this.m_checkProtect.Location = new System.Drawing.Point(16, 16);
            this.m_checkProtect.Name = "m_checkProtect";
            this.m_checkProtect.Size = new System.Drawing.Size(264, 32);
            this.m_checkProtect.TabIndex = 2;
            this.m_checkProtect.Text = "Password protect the \"Setup Features\" dialog?";
            this.m_checkProtect.Click += new System.EventHandler(this.cmd_onCheckClicked);
            // 
            // m_lblPassword
            // 
            this.m_lblPassword.Location = new System.Drawing.Point(16, 56);
            this.m_lblPassword.Name = "m_lblPassword";
            this.m_lblPassword.Size = new System.Drawing.Size(256, 23);
            this.m_lblPassword.TabIndex = 3;
            this.m_lblPassword.Text = "Enter a password:";
            this.m_lblPassword.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // m_lblHelps
            // 
            this.m_lblHelps.Location = new System.Drawing.Point(16, 112);
            this.m_lblHelps.Name = "m_lblHelps";
            this.m_lblHelps.Size = new System.Drawing.Size(256, 40);
            this.m_lblHelps.TabIndex = 4;
            this.m_lblHelps.Text = "(Take care to remember this password. E.g., Upper vs lower case is signifant.)";
            // 
            // m_editPassword
            // 
            this.m_editPassword.Location = new System.Drawing.Point(16, 80);
            this.m_editPassword.Name = "m_editPassword";
            this.m_editPassword.Size = new System.Drawing.Size(256, 20);
            this.m_editPassword.TabIndex = 5;
            // 
            // m_btnHelp
            // 
            this.m_btnHelp.Image = ((System.Drawing.Image)(resources.GetObject("m_btnHelp.Image")));
            this.m_btnHelp.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.m_btnHelp.Location = new System.Drawing.Point(192, 160);
            this.m_btnHelp.Name = "m_btnHelp";
            this.m_btnHelp.Size = new System.Drawing.Size(75, 23);
            this.m_btnHelp.TabIndex = 8;
            this.m_btnHelp.Text = "Help...";
            this.m_btnHelp.Click += new System.EventHandler(this.cmdHelp);
            // 
            // DialogPasswordProtect
            // 
            this.AcceptButton = this.m_btnOK;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.CancelButton = this.m_btnCancel;
            this.ClientSize = new System.Drawing.Size(290, 189);
            this.Controls.Add(this.m_btnHelp);
            this.Controls.Add(this.m_editPassword);
            this.Controls.Add(this.m_lblHelps);
            this.Controls.Add(this.m_lblPassword);
            this.Controls.Add(this.m_checkProtect);
            this.Controls.Add(this.m_btnCancel);
            this.Controls.Add(this.m_btnOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DialogPasswordProtect";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Password Protect";
            this.Closing += new System.ComponentModel.CancelEventHandler(this.onClosing);
            this.Load += new System.EventHandler(this.cmdLoad);
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		// Events ----------------------------------------------------------------------------
		#region Event: cmd_onCheckClicked(...) - enables the edit box
		private void cmd_onCheckClicked(object sender, System.EventArgs e)
		{
			m_editPassword.Enabled = m_checkProtect.Checked;
		}
		#endregion
		#region Event: onClosing(...) - makes sure we have a valid password
		private void onClosing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if (!m_checkProtect.Checked)
			{
				DialogPassword.SetDefaultPassword();
				return;
			}

			string sPassword = m_editPassword.Text;

			// Check: that they haven't entered the default password
			if (sPassword == DialogPassword.c_DefaultPassword)
			{
				string sMessage = "\"" + sPassword + 
					"\" is reserved and cannot be used as a password.";
				MessageBox.Show( this, sMessage, "Password Protect", 
					MessageBoxButtons.OK, MessageBoxIcon.Error);
				e.Cancel = true;
				return;
			}

			// Check: that they've entered something
			if (sPassword.Length < 6)
			{
				string sMessage = "The password needs to be at least 6 letters in length.";
				MessageBox.Show( this, sMessage, "Password Protect", 
					MessageBoxButtons.OK, MessageBoxIcon.Error);
				e.Cancel = true;
				return;
			}

			// Check that there are no spaces
			foreach(char ch in sPassword)
			{
				if (Char.IsPunctuation(ch) || Char.IsWhiteSpace(ch))
				{
					string sMessage = "The password cannot have punctuation or whitespace.";
					MessageBox.Show( this, sMessage, "Password Protect", 
						MessageBoxButtons.OK, MessageBoxIcon.Error);
					e.Cancel = true;
					return;
				}
			}

			// If we got this far, we're happy! Save the password.
			DialogPassword.SetPassword(sPassword);
		}
		#endregion
		#region Event: cmdLoad(...) - sets the control text in their appropriate language
		private void cmdLoad(object sender, System.EventArgs e)
		{
			// Label text in the appropriate language
            Control[] vExclude = { m_editPassword };
            LocDB.Localize(this, vExclude);
		}
		#endregion
		#region Event: cmdHelp(...)
		private void cmdHelp(object sender, System.EventArgs e)
		{
			JW_Help.Show_DlgPasswordProtect();
		}
		#endregion
	}
	#endregion

    #region FORM CLASS DialogPassword - Obtain a password from the user
    public class DialogPassword : System.Windows.Forms.Form
	{
		public const string c_RegistryKey  = "Features";
		public const string c_RegistryName = "password";
		public const string c_DefaultPassword = "Welcome";

		#region Method: string GetPassword()
		static public string GetPassword()
		{
			return JW_Registry.GetValue(c_RegistryKey, c_RegistryName, 
				c_DefaultPassword);
		}
		#endregion
		#region Method: void SetPassword(string sPassword)
		static public void SetPassword(string sPassword)
		{
			JW_Registry.SetValue(c_RegistryKey, c_RegistryName, sPassword);
		}
		#endregion
		#region Method: void SetDefaultPassword()
		static public void SetDefaultPassword()
		{
			JW_Registry.SetValue(c_RegistryKey, c_RegistryName, c_DefaultPassword);
		}
		#endregion

		#region Attr{g}: bool IsPasswordProtected
		static public bool IsPasswordProtected
		{
			get
			{
				return (GetPassword() != c_DefaultPassword);
			}
		}
		#endregion

		private const string c_sBackDoor = "MasukSajah";

		// Scaffolding -----------------------------------------------------------------------
		#region Constructor()
		public DialogPassword()
		{
			InitializeComponent();

			// The OK button is not enabled until the password is correct
			m_btnOK.Enabled = false;
		}
		#endregion
		#region Dispose, Controls, Designer code, etc.
		private System.Windows.Forms.Label m_lblPrompt;
		private System.Windows.Forms.TextBox m_editPassword;
		private System.Windows.Forms.Button m_btnOK;
		private System.Windows.Forms.Button m_btnCancel;

		// Required designer variable.
		private System.ComponentModel.Container components = null;

		// Clean up any resources being used.
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

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.m_lblPrompt = new System.Windows.Forms.Label();
			this.m_editPassword = new System.Windows.Forms.TextBox();
			this.m_btnOK = new System.Windows.Forms.Button();
			this.m_btnCancel = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// m_lblPrompt
			// 
			this.m_lblPrompt.Location = new System.Drawing.Point(8, 8);
			this.m_lblPrompt.Name = "m_lblPrompt";
			this.m_lblPrompt.Size = new System.Drawing.Size(240, 40);
			this.m_lblPrompt.TabIndex = 0;
			this.m_lblPrompt.Text = "Please enter your password to access the \"Setup Features\" dialog.";
			this.m_lblPrompt.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// m_editPassword
			// 
			this.m_editPassword.Location = new System.Drawing.Point(8, 56);
			this.m_editPassword.Name = "m_editPassword";
			this.m_editPassword.PasswordChar = '*';
			this.m_editPassword.Size = new System.Drawing.Size(240, 20);
			this.m_editPassword.TabIndex = 1;
			this.m_editPassword.Text = "";
			this.m_editPassword.TextChanged += new System.EventHandler(this.onTextChanged);
			// 
			// m_btnOK
			// 
			this.m_btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.m_btnOK.Location = new System.Drawing.Point(48, 96);
			this.m_btnOK.Name = "m_btnOK";
			this.m_btnOK.TabIndex = 2;
			this.m_btnOK.Text = "OK";
			// 
			// m_btnCancel
			// 
			this.m_btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.m_btnCancel.Location = new System.Drawing.Point(136, 96);
			this.m_btnCancel.Name = "m_btnCancel";
			this.m_btnCancel.TabIndex = 3;
			this.m_btnCancel.Text = "Cancel";
			// 
			// DialogPassword
			// 
			this.AcceptButton = this.m_btnOK;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.m_btnCancel;
			this.ClientSize = new System.Drawing.Size(258, 133);
			this.Controls.Add(this.m_btnCancel);
			this.Controls.Add(this.m_btnOK);
			this.Controls.Add(this.m_editPassword);
			this.Controls.Add(this.m_lblPrompt);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "DialogPassword";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Password";
			this.Load += new System.EventHandler(this.cmdLoad);
			this.ResumeLayout(false);

		}
		#endregion
		#endregion

		#region Cmd: onTextChanged(...) - enables the OK button if the password matches
		private void onTextChanged(object sender, System.EventArgs e)
		{
			string sPassword = DialogPassword.GetPassword();

			// Check against the user's password
			bool bMatch = (sPassword == m_editPassword.Text);

			// Provide an additional password in case we need to provide emergency help
			if (!bMatch)
				bMatch = (c_sBackDoor == m_editPassword.Text);

			m_btnOK.Enabled = bMatch;
		}
		#endregion

		#region Cmd: cmdLoad(...) - sets the control text in their appropriate language
		private void cmdLoad(object sender, System.EventArgs e)
		{
			// Label text in the appropriate language
            Control[] vExclude = { m_editPassword };
            LocDB.Localize(this, vExclude);
		}
		#endregion
	}
	#endregion


}