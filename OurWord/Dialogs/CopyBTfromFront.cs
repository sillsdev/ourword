/**********************************************************************************************
 * Project: Our Word!
 * File:    CopyBTfromFront.cs
 * Author:  John Wimbish
 * Created: 16 Mar 2006
 * Purpose: Dialog to copy the back translations from the front to the daughter
 * Legal:   Copyright (c) 2004-09, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
#region Using
using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.IO;
using Microsoft.Win32;

using JWTools;
using JWdb;
using JWdb.DataModel;
#endregion


namespace OurWord
{
	public class CopyBTfromFront : System.Windows.Forms.Form
	{
		// Scaffolding -----------------------------------------------------------------------
		#region Attr{g}: bool EntireBook - If F, then we're just doing the Current Section
		bool EntireBook
		{
			get
			{
				return m_bEntireBook;
			}
		}
		bool m_bEntireBook;
		#endregion
		#region DIALOG CONTROLS
		private System.Windows.Forms.Button m_btnHelp;
		private System.Windows.Forms.Button m_btnCancel;
		private System.Windows.Forms.Button m_btnOK;
		private System.Windows.Forms.PictureBox m_WarningIcon;
		private System.Windows.Forms.Label m_labelSynopsis;
		private System.Windows.Forms.Label m_labelProceed;
		private System.Windows.Forms.Label m_labelExp1;
		private System.Windows.Forms.Label m_labelExp2;
		private System.Windows.Forms.Label m_labelExp3;
		private System.Windows.Forms.Label m_labelExp4;
		private System.Windows.Forms.Label m_labelScope;
		private System.ComponentModel.Container components = null;
		#endregion
		#region Constructor(bEntireBook)
		public CopyBTfromFront(bool _bEntireBook)
		{
			m_bEntireBook = _bEntireBook;

			// Required for Windows Form Designer support
			InitializeComponent();
		}
		#endregion
		#region Method: void Dispose( bool bDisposing) - Clean up any resources being used.
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(CopyBTfromFront));
			this.m_btnHelp = new System.Windows.Forms.Button();
			this.m_btnCancel = new System.Windows.Forms.Button();
			this.m_btnOK = new System.Windows.Forms.Button();
			this.m_WarningIcon = new System.Windows.Forms.PictureBox();
			this.m_labelSynopsis = new System.Windows.Forms.Label();
			this.m_labelProceed = new System.Windows.Forms.Label();
			this.m_labelExp1 = new System.Windows.Forms.Label();
			this.m_labelExp2 = new System.Windows.Forms.Label();
			this.m_labelExp3 = new System.Windows.Forms.Label();
			this.m_labelExp4 = new System.Windows.Forms.Label();
			this.m_labelScope = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// m_btnHelp
			// 
			this.m_btnHelp.Image = ((System.Drawing.Image)(resources.GetObject("m_btnHelp.Image")));
			this.m_btnHelp.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.m_btnHelp.Location = new System.Drawing.Point(288, 384);
			this.m_btnHelp.Name = "m_btnHelp";
			this.m_btnHelp.TabIndex = 15;
			this.m_btnHelp.Text = "Help...";
			this.m_btnHelp.Click += new System.EventHandler(this.cmdHelp);
			// 
			// m_btnCancel
			// 
			this.m_btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.m_btnCancel.Location = new System.Drawing.Point(200, 384);
			this.m_btnCancel.Name = "m_btnCancel";
			this.m_btnCancel.TabIndex = 14;
			this.m_btnCancel.Text = "Cancel";
			// 
			// m_btnOK
			// 
			this.m_btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.m_btnOK.Location = new System.Drawing.Point(112, 384);
			this.m_btnOK.Name = "m_btnOK";
			this.m_btnOK.TabIndex = 13;
			this.m_btnOK.Text = "Proceed";
			// 
			// m_WarningIcon
			// 
			this.m_WarningIcon.Location = new System.Drawing.Point(16, 16);
			this.m_WarningIcon.Name = "m_WarningIcon";
			this.m_WarningIcon.Size = new System.Drawing.Size(48, 32);
			this.m_WarningIcon.TabIndex = 16;
			this.m_WarningIcon.TabStop = false;
			// 
			// m_labelSynopsis
			// 
			this.m_labelSynopsis.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.m_labelSynopsis.Location = new System.Drawing.Point(72, 8);
			this.m_labelSynopsis.Name = "m_labelSynopsis";
			this.m_labelSynopsis.Size = new System.Drawing.Size(392, 40);
			this.m_labelSynopsis.TabIndex = 17;
			this.m_labelSynopsis.Text = "This process will copy the back translations from Kupang Luke to  Amarasi Luke.";
			this.m_labelSynopsis.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// m_labelProceed
			// 
			this.m_labelProceed.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.m_labelProceed.Location = new System.Drawing.Point(72, 352);
			this.m_labelProceed.Name = "m_labelProceed";
			this.m_labelProceed.Size = new System.Drawing.Size(384, 16);
			this.m_labelProceed.TabIndex = 18;
			this.m_labelProceed.Text = "Do you still wish to proceed?";
			this.m_labelProceed.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// m_labelExp1
			// 
			this.m_labelExp1.Location = new System.Drawing.Point(72, 88);
			this.m_labelExp1.Name = "m_labelExp1";
			this.m_labelExp1.Size = new System.Drawing.Size(392, 56);
			this.m_labelExp1.TabIndex = 19;
			this.m_labelExp1.Text = @"This may be useful to you as a time-saver if the daughter follows the front fairly closely. However, it must be used with great caution, because an accurate back translation is absolutely essential if the consultant is going to be able to effectively check the daughter translation. ";
			// 
			// m_labelExp2
			// 
			this.m_labelExp2.Location = new System.Drawing.Point(72, 152);
			this.m_labelExp2.Name = "m_labelExp2";
			this.m_labelExp2.Size = new System.Drawing.Size(392, 64);
			this.m_labelExp2.TabIndex = 20;
			this.m_labelExp2.Text = "After copying, careful work will be needed to edit the resulting Back Translation" +
				" to ensure that it actually reflects the text of the Daughter Translation. This " +
				"step requires 1) careful thought and 2) integrity. Use it as another checking pr" +
				"ocess.";
			// 
			// m_labelExp3
			// 
			this.m_labelExp3.Location = new System.Drawing.Point(72, 216);
			this.m_labelExp3.Name = "m_labelExp3";
			this.m_labelExp3.Size = new System.Drawing.Size(392, 72);
			this.m_labelExp3.TabIndex = 21;
			this.m_labelExp3.Text = @"You will find this step will perhaps highlight some verses that need to be reexamined. There will ALWAYS be differences between a Front Translation and a Daughter Translation. For example, grammatical structures may place phrases in a different part of a sentence. Idioms, e.g. for emotion terms may require ‘liver’ to be changed to ‘insides’. ";
			// 
			// m_labelExp4
			// 
			this.m_labelExp4.Location = new System.Drawing.Point(72, 288);
			this.m_labelExp4.Name = "m_labelExp4";
			this.m_labelExp4.Size = new System.Drawing.Size(392, 64);
			this.m_labelExp4.TabIndex = 22;
			this.m_labelExp4.Text = @"Again, by using this feature, even after careful post-editing, you still run the very real risk that your resultant back translation will not accurately reflect the daughter translation text. Therefore, you should only run this command if you are confident that you will indeed do a careful, detailed post-edit.";
			// 
			// m_labelScope
			// 
			this.m_labelScope.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.m_labelScope.Location = new System.Drawing.Point(72, 56);
			this.m_labelScope.Name = "m_labelScope";
			this.m_labelScope.Size = new System.Drawing.Size(392, 23);
			this.m_labelScope.TabIndex = 23;
			this.m_labelScope.Text = "The Entire Book will be copied.";
			this.m_labelScope.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// CopyBTfromFront
			// 
			this.AcceptButton = this.m_btnOK;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.m_btnCancel;
			this.ClientSize = new System.Drawing.Size(474, 416);
			this.Controls.Add(this.m_labelScope);
			this.Controls.Add(this.m_labelExp4);
			this.Controls.Add(this.m_labelExp3);
			this.Controls.Add(this.m_labelExp2);
			this.Controls.Add(this.m_labelExp1);
			this.Controls.Add(this.m_labelProceed);
			this.Controls.Add(this.m_labelSynopsis);
			this.Controls.Add(this.m_WarningIcon);
			this.Controls.Add(this.m_btnHelp);
			this.Controls.Add(this.m_btnCancel);
			this.Controls.Add(this.m_btnOK);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "CopyBTfromFront";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Copy Back Translation";
			this.Load += new System.EventHandler(this.cmdLoad);
			this.ResumeLayout(false);

		}
		#endregion

		// Command Handlers ------------------------------------------------------------------
		#region Cmd: cmdLoad
		private void cmdLoad(object sender, System.EventArgs e)
		{
			// Display the icon
			m_WarningIcon.Image = SystemIcons.Warning.ToBitmap();

			// Scope Message
			m_labelScope.Text = EntireBook ?
				"The Entire Book will be copied." :
				"Only the Current Section will be copied.";

			// Craft the message to display the names of the languages involved
			string sSummary = "This process will copy the back translation from " +
				DB.Project.FrontTranslation.DisplayName + " " +
				DB.Project.SFront.Book.DisplayName + " to " +
				DB.Project.TargetTranslation.DisplayName + " " +
				DB.Project.STarget.Book.DisplayName + ".";
			m_labelSynopsis.Text = sSummary;
		}
		#endregion
		#region Cmd: cmdHelp
		private void cmdHelp(object sender, System.EventArgs e)
		{
//			HelpSystem.Show_DlgCopyBTfromFront();
		}
		#endregion
	}
}
