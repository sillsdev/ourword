namespace OurWord.Dialogs
{
    partial class Page_WritingSystems
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
//        private System.ComponentModel.IContainer components = null;


        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
			this.m_btnRemove = new System.Windows.Forms.Button();
			this.m_PropGrid = new System.Windows.Forms.PropertyGrid();
			this.m_tabWS = new System.Windows.Forms.TabControl();
			this.m_tabWSGeneral = new System.Windows.Forms.TabPage();
			this.m_tabWSHyphenation = new System.Windows.Forms.TabPage();
			this.m_tabWSAutoReplace = new System.Windows.Forms.TabPage();
			this.m_ctrlAutoReplace = new OurWord.Dialogs.Ctrl_AutoReplace();
			this.m_LiterateSettingsWnd = new OurWord.Edit.LiterateSettingsWnd();
			this.m_tabWS.SuspendLayout();
			this.m_tabWSGeneral.SuspendLayout();
			this.m_tabWSHyphenation.SuspendLayout();
			this.m_tabWSAutoReplace.SuspendLayout();
			this.SuspendLayout();
			// 
			// m_btnRemove
			// 
			this.m_btnRemove.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.m_btnRemove.Location = new System.Drawing.Point(131, 305);
			this.m_btnRemove.Name = "m_btnRemove";
			this.m_btnRemove.Size = new System.Drawing.Size(165, 23);
			this.m_btnRemove.TabIndex = 8;
			this.m_btnRemove.Text = "Remove this Writing System...";
			this.m_btnRemove.UseVisualStyleBackColor = true;
			this.m_btnRemove.Click += new System.EventHandler(this.cmdRemoveBtnClicked);
			// 
			// m_PropGrid
			// 
			this.m_PropGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.m_PropGrid.LineColor = System.Drawing.SystemColors.ScrollBar;
			this.m_PropGrid.Location = new System.Drawing.Point(3, 6);
			this.m_PropGrid.Name = "m_PropGrid";
			this.m_PropGrid.PropertySort = System.Windows.Forms.PropertySort.Categorized;
			this.m_PropGrid.Size = new System.Drawing.Size(428, 293);
			this.m_PropGrid.TabIndex = 33;
			this.m_PropGrid.ToolbarVisible = false;
			// 
			// m_tabWS
			// 
			this.m_tabWS.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.m_tabWS.Controls.Add(this.m_tabWSGeneral);
			this.m_tabWS.Controls.Add(this.m_tabWSHyphenation);
			this.m_tabWS.Controls.Add(this.m_tabWSAutoReplace);
			this.m_tabWS.Location = new System.Drawing.Point(3, 0);
			this.m_tabWS.Name = "m_tabWS";
			this.m_tabWS.SelectedIndex = 0;
			this.m_tabWS.Size = new System.Drawing.Size(448, 360);
			this.m_tabWS.TabIndex = 34;
			// 
			// m_tabWSGeneral
			// 
			this.m_tabWSGeneral.Controls.Add(this.m_PropGrid);
			this.m_tabWSGeneral.Controls.Add(this.m_btnRemove);
			this.m_tabWSGeneral.Location = new System.Drawing.Point(4, 22);
			this.m_tabWSGeneral.Name = "m_tabWSGeneral";
			this.m_tabWSGeneral.Padding = new System.Windows.Forms.Padding(3);
			this.m_tabWSGeneral.Size = new System.Drawing.Size(440, 334);
			this.m_tabWSGeneral.TabIndex = 0;
			this.m_tabWSGeneral.Text = "General";
			this.m_tabWSGeneral.UseVisualStyleBackColor = true;
			// 
			// m_tabWSHyphenation
			// 
			this.m_tabWSHyphenation.Controls.Add(this.m_LiterateSettingsWnd);
			this.m_tabWSHyphenation.Location = new System.Drawing.Point(4, 22);
			this.m_tabWSHyphenation.Name = "m_tabWSHyphenation";
			this.m_tabWSHyphenation.Padding = new System.Windows.Forms.Padding(3);
			this.m_tabWSHyphenation.Size = new System.Drawing.Size(440, 334);
			this.m_tabWSHyphenation.TabIndex = 2;
			this.m_tabWSHyphenation.Text = "Hyphenation";
			this.m_tabWSHyphenation.UseVisualStyleBackColor = true;
			// 
			// m_tabWSAutoReplace
			// 
			this.m_tabWSAutoReplace.Controls.Add(this.m_ctrlAutoReplace);
			this.m_tabWSAutoReplace.Location = new System.Drawing.Point(4, 22);
			this.m_tabWSAutoReplace.Name = "m_tabWSAutoReplace";
			this.m_tabWSAutoReplace.Padding = new System.Windows.Forms.Padding(3);
			this.m_tabWSAutoReplace.Size = new System.Drawing.Size(440, 334);
			this.m_tabWSAutoReplace.TabIndex = 1;
			this.m_tabWSAutoReplace.Text = "AutoReplace";
			this.m_tabWSAutoReplace.UseVisualStyleBackColor = true;
			// 
			// m_ctrlAutoReplace
			// 
			this.m_ctrlAutoReplace.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.m_ctrlAutoReplace.Location = new System.Drawing.Point(6, 3);
			this.m_ctrlAutoReplace.Name = "m_ctrlAutoReplace";
			this.m_ctrlAutoReplace.Size = new System.Drawing.Size(308, 287);
			this.m_ctrlAutoReplace.TabIndex = 0;
			// 
			// m_LiterateSettingsWnd
			// 
			this.m_LiterateSettingsWnd.Dock = System.Windows.Forms.DockStyle.Fill;
			this.m_LiterateSettingsWnd.Location = new System.Drawing.Point(3, 3);
			this.m_LiterateSettingsWnd.Name = "m_LiterateSettingsWnd";
			this.m_LiterateSettingsWnd.Size = new System.Drawing.Size(434, 328);
			this.m_LiterateSettingsWnd.TabIndex = 0;
			// 
			// Page_WritingSystems
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.Controls.Add(this.m_tabWS);
			this.Name = "Page_WritingSystems";
			this.Size = new System.Drawing.Size(454, 361);
			this.Load += new System.EventHandler(this.cmdLoad);
			this.m_tabWS.ResumeLayout(false);
			this.m_tabWSGeneral.ResumeLayout(false);
			this.m_tabWSHyphenation.ResumeLayout(false);
			this.m_tabWSAutoReplace.ResumeLayout(false);
			this.ResumeLayout(false);

        }

        #endregion

		private System.Windows.Forms.Button m_btnRemove;
        private System.Windows.Forms.PropertyGrid m_PropGrid;
        private System.Windows.Forms.TabControl m_tabWS;
        private System.Windows.Forms.TabPage m_tabWSGeneral;
        private System.Windows.Forms.TabPage m_tabWSAutoReplace;
        private Ctrl_AutoReplace m_ctrlAutoReplace;
		private System.Windows.Forms.TabPage m_tabWSHyphenation;
		private OurWord.Edit.LiterateSettingsWnd m_LiterateSettingsWnd;
    }
}
