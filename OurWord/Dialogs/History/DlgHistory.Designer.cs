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
            this.m_tabChart = new System.Windows.Forms.TabPage();
            this.m_ctrlTranslationProgress = new OurWord.Dialogs.TranslationProgress();
            this.m_btnClose = new System.Windows.Forms.Button();
            this.m_tabs.SuspendLayout();
            this.m_tabChart.SuspendLayout();
            this.SuspendLayout();
            // 
            // m_tabs
            // 
            this.m_tabs.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.m_tabs.Controls.Add(this.m_tabSection);
            this.m_tabs.Controls.Add(this.m_tabBook);
            this.m_tabs.Controls.Add(this.m_tabChart);
            this.m_tabs.Location = new System.Drawing.Point(6, 7);
            this.m_tabs.Name = "m_tabs";
            this.m_tabs.SelectedIndex = 0;
            this.m_tabs.Size = new System.Drawing.Size(682, 486);
            this.m_tabs.TabIndex = 0;
            // 
            // m_tabSection
            // 
            this.m_tabSection.Location = new System.Drawing.Point(4, 22);
            this.m_tabSection.Name = "m_tabSection";
            this.m_tabSection.Padding = new System.Windows.Forms.Padding(3);
            this.m_tabSection.Size = new System.Drawing.Size(674, 460);
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
            // m_tabChart
            // 
            this.m_tabChart.Controls.Add(this.m_ctrlTranslationProgress);
            this.m_tabChart.Location = new System.Drawing.Point(4, 22);
            this.m_tabChart.Name = "m_tabChart";
            this.m_tabChart.Size = new System.Drawing.Size(465, 460);
            this.m_tabChart.TabIndex = 2;
            this.m_tabChart.Text = "Chart";
            this.m_tabChart.UseVisualStyleBackColor = true;
            // 
            // m_ctrlTranslationProgress
            // 
            this.m_ctrlTranslationProgress.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_ctrlTranslationProgress.Location = new System.Drawing.Point(0, 0);
            this.m_ctrlTranslationProgress.Name = "m_ctrlTranslationProgress";
            this.m_ctrlTranslationProgress.Size = new System.Drawing.Size(465, 460);
            this.m_ctrlTranslationProgress.TabIndex = 1;
            // 
            // m_btnClose
            // 
            this.m_btnClose.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.m_btnClose.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.m_btnClose.Location = new System.Drawing.Point(307, 499);
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
            this.ClientSize = new System.Drawing.Size(693, 530);
            this.Controls.Add(this.m_btnClose);
            this.Controls.Add(this.m_tabs);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(650, 450);
            this.Name = "DlgHistory";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "History";
            this.Load += new System.EventHandler(this.cmdLoad);
            this.SizeChanged += new System.EventHandler(this.cmdSizeChanged);
            this.m_tabs.ResumeLayout(false);
            this.m_tabChart.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl m_tabs;
        private System.Windows.Forms.TabPage m_tabSection;
        private System.Windows.Forms.TabPage m_tabBook;
        private System.Windows.Forms.Button m_btnClose;
        private System.Windows.Forms.TabPage m_tabChart;
        public OurWord.Dialogs.TranslationProgress m_ctrlTranslationProgress;
    }
}