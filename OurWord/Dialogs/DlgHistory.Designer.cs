namespace OurWord.Dialogs
{
    partial class DlgHistory
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DlgHistory));
            this.m_tabs = new System.Windows.Forms.TabControl();
            this.m_tabSection = new System.Windows.Forms.TabPage();
            this.m_tabBook = new System.Windows.Forms.TabPage();
            this.m_btnClose = new System.Windows.Forms.Button();
            this.m_tabs.SuspendLayout();
            this.SuspendLayout();
            // 
            // m_tabs
            // 
            this.m_tabs.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.m_tabs.Controls.Add(this.m_tabSection);
            this.m_tabs.Controls.Add(this.m_tabBook);
            this.m_tabs.Location = new System.Drawing.Point(6, 7);
            this.m_tabs.Name = "m_tabs";
            this.m_tabs.SelectedIndex = 0;
            this.m_tabs.Size = new System.Drawing.Size(473, 486);
            this.m_tabs.TabIndex = 0;
            // 
            // m_tabSection
            // 
            this.m_tabSection.Location = new System.Drawing.Point(4, 22);
            this.m_tabSection.Name = "m_tabSection";
            this.m_tabSection.Padding = new System.Windows.Forms.Padding(3);
            this.m_tabSection.Size = new System.Drawing.Size(465, 460);
            this.m_tabSection.TabIndex = 0;
            this.m_tabSection.Text = "This Section";
            this.m_tabSection.UseVisualStyleBackColor = true;
            // 
            // m_tabBook
            // 
            this.m_tabBook.Location = new System.Drawing.Point(4, 22);
            this.m_tabBook.Name = "m_tabBook";
            this.m_tabBook.Padding = new System.Windows.Forms.Padding(3);
            this.m_tabBook.Size = new System.Drawing.Size(465, 460);
            this.m_tabBook.TabIndex = 1;
            this.m_tabBook.Text = "Entire Book";
            this.m_tabBook.UseVisualStyleBackColor = true;
            // 
            // m_btnClose
            // 
            this.m_btnClose.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.m_btnClose.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.m_btnClose.Location = new System.Drawing.Point(203, 499);
            this.m_btnClose.Name = "m_btnClose";
            this.m_btnClose.Size = new System.Drawing.Size(75, 23);
            this.m_btnClose.TabIndex = 1;
            this.m_btnClose.Text = "Close";
            this.m_btnClose.UseVisualStyleBackColor = true;
            // 
            // DlgHistory
            // 
            this.AcceptButton = this.m_btnClose;
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(484, 530);
            this.Controls.Add(this.m_btnClose);
            this.Controls.Add(this.m_tabs);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DlgHistory";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "History";
            this.Load += new System.EventHandler(this.cmdLoad);
            this.SizeChanged += new System.EventHandler(this.cmdSizeChanged);
            this.m_tabs.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl m_tabs;
        private System.Windows.Forms.TabPage m_tabSection;
        private System.Windows.Forms.TabPage m_tabBook;
        private System.Windows.Forms.Button m_btnClose;
    }
}