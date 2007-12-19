/**********************************************************************************************
 * Dll:      JWTools
 * File:     JW_More.cs
 * Author:   John Wimbish
 * Created:  14 Feb 2005
 * Purpose:  The "More" dialog for when the MRU has more than 9 items in it
 * Legal:   Copyright (c) 2004-05, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/

#region Using
using System;
using System.Diagnostics;
using System.Windows.Forms;
using System.Text;
using System.Threading;
using System.IO;
using NUnit.Framework;
using DevComponents.DotNetBar;
#endregion

namespace JWTools
{
	/// <summary>
	/// Summary description for JW_More.
	/// </summary>
	public class JW_More : System.Windows.Forms.Form
	{
		private System.Windows.Forms.ListBox listBox1;
		private System.Windows.Forms.Label m_lblPrompt;
		private System.Windows.Forms.Button m_btnHelp;
		private System.Windows.Forms.Button m_btnCancel;
		private System.Windows.Forms.Button m_btnOK;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public JW_More()
		{
			// Required for Windows Form Designer support
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

		#region Method: void Dispose(...) - Clean up any resources being used.
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(JW_More));
			this.listBox1 = new System.Windows.Forms.ListBox();
			this.m_lblPrompt = new System.Windows.Forms.Label();
			this.m_btnHelp = new System.Windows.Forms.Button();
			this.m_btnCancel = new System.Windows.Forms.Button();
			this.m_btnOK = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// listBox1
			// 
			this.listBox1.Location = new System.Drawing.Point(8, 24);
			this.listBox1.Name = "listBox1";
			this.listBox1.Size = new System.Drawing.Size(272, 277);
			this.listBox1.TabIndex = 0;
			// 
			// m_lblPrompt
			// 
			this.m_lblPrompt.Location = new System.Drawing.Point(8, 8);
			this.m_lblPrompt.Name = "m_lblPrompt";
			this.m_lblPrompt.Size = new System.Drawing.Size(272, 16);
			this.m_lblPrompt.TabIndex = 1;
			this.m_lblPrompt.Text = "What file do you wish to open?";
			// 
			// m_btnHelp
			// 
			this.m_btnHelp.Image = ((System.Drawing.Image)(resources.GetObject("m_btnHelp.Image")));
			this.m_btnHelp.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.m_btnHelp.Location = new System.Drawing.Point(192, 320);
			this.m_btnHelp.Name = "m_btnHelp";
			this.m_btnHelp.TabIndex = 11;
			this.m_btnHelp.Text = "Help...";
			// 
			// m_btnCancel
			// 
			this.m_btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.m_btnCancel.Location = new System.Drawing.Point(112, 320);
			this.m_btnCancel.Name = "m_btnCancel";
			this.m_btnCancel.TabIndex = 10;
			this.m_btnCancel.Text = "Cancel";
			// 
			// m_btnOK
			// 
			this.m_btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.m_btnOK.Location = new System.Drawing.Point(32, 320);
			this.m_btnOK.Name = "m_btnOK";
			this.m_btnOK.TabIndex = 9;
			this.m_btnOK.Text = "OK";
			// 
			// JW_More
			// 
			this.AcceptButton = this.m_btnOK;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.m_btnCancel;
			this.ClientSize = new System.Drawing.Size(292, 357);
			this.Controls.Add(this.m_btnHelp);
			this.Controls.Add(this.m_btnCancel);
			this.Controls.Add(this.m_btnOK);
			this.Controls.Add(this.m_lblPrompt);
			this.Controls.Add(this.listBox1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "JW_More";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "More...";
			this.ResumeLayout(false);

		}
		#endregion
	}
}
