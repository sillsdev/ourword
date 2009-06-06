namespace OurWord.Edit
{
	partial class LiterateSettingsWnd
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
            this.m_cbShowDocumentation = new System.Windows.Forms.CheckBox();
            this.m_panelSettingsContainer = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // m_cbShowDocumentation
            // 
            this.m_cbShowDocumentation.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.m_cbShowDocumentation.AutoSize = true;
            this.m_cbShowDocumentation.Checked = true;
            this.m_cbShowDocumentation.CheckState = System.Windows.Forms.CheckState.Checked;
            this.m_cbShowDocumentation.Location = new System.Drawing.Point(69, 271);
            this.m_cbShowDocumentation.Name = "m_cbShowDocumentation";
            this.m_cbShowDocumentation.Size = new System.Drawing.Size(176, 17);
            this.m_cbShowDocumentation.TabIndex = 0;
            this.m_cbShowDocumentation.Text = "Show Verbose Documentation?";
            this.m_cbShowDocumentation.UseVisualStyleBackColor = true;
            this.m_cbShowDocumentation.CheckedChanged += new System.EventHandler(this.cmdShowDocumentationChanged);
            // 
            // m_panelSettingsContainer
            // 
            this.m_panelSettingsContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.m_panelSettingsContainer.Location = new System.Drawing.Point(3, 0);
            this.m_panelSettingsContainer.Name = "m_panelSettingsContainer";
            this.m_panelSettingsContainer.Size = new System.Drawing.Size(313, 265);
            this.m_panelSettingsContainer.TabIndex = 1;
            this.m_panelSettingsContainer.Resize += new System.EventHandler(this.cmdResize);
            // 
            // LiterateSettingsWnd
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.m_panelSettingsContainer);
            this.Controls.Add(this.m_cbShowDocumentation);
            this.Name = "LiterateSettingsWnd";
            this.Size = new System.Drawing.Size(319, 291);
            this.Load += new System.EventHandler(this.cmdLoad);
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.CheckBox m_cbShowDocumentation;
		private System.Windows.Forms.Panel m_panelSettingsContainer;
	}
}
