namespace OurWord.Dialogs
{
	partial class Page_AddWritingSystem
	{
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.m_btnAdd = new System.Windows.Forms.Button();
            this.m_LiterateSettingsWnd = new OurWord.Edit.LiterateSettingsWnd();
            this.SuspendLayout();
            // 
            // m_btnAdd
            // 
            this.m_btnAdd.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.m_btnAdd.Location = new System.Drawing.Point(90, 265);
            this.m_btnAdd.Name = "m_btnAdd";
            this.m_btnAdd.Size = new System.Drawing.Size(163, 23);
            this.m_btnAdd.TabIndex = 1;
            this.m_btnAdd.Text = "Add New Writing System...";
            this.m_btnAdd.UseVisualStyleBackColor = true;
            this.m_btnAdd.Click += new System.EventHandler(this.cmdAddWritingSystem);
            // 
            // m_LiterateSettingsWnd
            // 
            this.m_LiterateSettingsWnd.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.m_LiterateSettingsWnd.DontAllowPropertyGrid = false;
            this.m_LiterateSettingsWnd.Location = new System.Drawing.Point(3, 4);
            this.m_LiterateSettingsWnd.Name = "m_LiterateSettingsWnd";
            this.m_LiterateSettingsWnd.ShowDocumentation = true;
            this.m_LiterateSettingsWnd.Size = new System.Drawing.Size(337, 257);
            this.m_LiterateSettingsWnd.TabIndex = 2;
            // 
            // Page_AddWritingSystem
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.Controls.Add(this.m_LiterateSettingsWnd);
            this.Controls.Add(this.m_btnAdd);
            this.Name = "Page_AddWritingSystem";
            this.Size = new System.Drawing.Size(343, 293);
            this.Load += new System.EventHandler(this.cmdLoad);
            this.ResumeLayout(false);

		}

		#endregion

        private System.Windows.Forms.Button m_btnAdd;
        private OurWord.Edit.LiterateSettingsWnd m_LiterateSettingsWnd;

	}
}
