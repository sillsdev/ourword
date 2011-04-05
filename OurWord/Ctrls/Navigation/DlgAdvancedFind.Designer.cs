namespace OurWord.Ctrls.Navigation
{
    partial class DlgAdvancedFind
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DlgAdvancedFind));
            this.m_btnClose = new System.Windows.Forms.Button();
            this.m_lFindWhat = new System.Windows.Forms.Label();
            this.m_textFindWhat = new System.Windows.Forms.TextBox();
            this.m_btnFindNext = new System.Windows.Forms.Button();
            this.m_ctrlFindOptions = new OurWord.Ctrls.Navigation.CtrlFindOptions();
            this.SuspendLayout();
            // 
            // m_btnClose
            // 
            this.m_btnClose.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.m_btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.m_btnClose.Location = new System.Drawing.Point(320, 143);
            this.m_btnClose.Name = "m_btnClose";
            this.m_btnClose.Size = new System.Drawing.Size(75, 23);
            this.m_btnClose.TabIndex = 13;
            this.m_btnClose.Text = "Close";
            this.m_btnClose.Click += new System.EventHandler(this.cmdClose);
            // 
            // m_lFindWhat
            // 
            this.m_lFindWhat.AutoSize = true;
            this.m_lFindWhat.Location = new System.Drawing.Point(12, 9);
            this.m_lFindWhat.Name = "m_lFindWhat";
            this.m_lFindWhat.Size = new System.Drawing.Size(59, 13);
            this.m_lFindWhat.TabIndex = 20;
            this.m_lFindWhat.Text = "Find What:";
            // 
            // m_textFindWhat
            // 
            this.m_textFindWhat.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.m_textFindWhat.Location = new System.Drawing.Point(98, 6);
            this.m_textFindWhat.Name = "m_textFindWhat";
            this.m_textFindWhat.Size = new System.Drawing.Size(297, 20);
            this.m_textFindWhat.TabIndex = 1;
            this.m_textFindWhat.TextChanged += new System.EventHandler(this.onFindWhatChanged);
            // 
            // m_btnFindNext
            // 
            this.m_btnFindNext.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.m_btnFindNext.Location = new System.Drawing.Point(239, 143);
            this.m_btnFindNext.Name = "m_btnFindNext";
            this.m_btnFindNext.Size = new System.Drawing.Size(75, 23);
            this.m_btnFindNext.TabIndex = 12;
            this.m_btnFindNext.Text = "Find Next";
            this.m_btnFindNext.Click += new System.EventHandler(this.cmdFindNext);
            // 
            // m_ctrlFindOptions
            // 
            this.m_ctrlFindOptions.IgnoreCase = false;
            this.m_ctrlFindOptions.Location = new System.Drawing.Point(98, 31);
            this.m_ctrlFindOptions.Name = "m_ctrlFindOptions";
            this.m_ctrlFindOptions.OnlyScanCurrentBook = false;
            this.m_ctrlFindOptions.Size = new System.Drawing.Size(297, 91);
            this.m_ctrlFindOptions.TabIndex = 23;
            // 
            // DlgAdvancedFind
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.m_btnClose;
            this.ClientSize = new System.Drawing.Size(407, 175);
            this.Controls.Add(this.m_textFindWhat);
            this.Controls.Add(this.m_btnFindNext);
            this.Controls.Add(this.m_lFindWhat);
            this.Controls.Add(this.m_btnClose);
            this.Controls.Add(this.m_ctrlFindOptions);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DlgAdvancedFind";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Find";
            this.Load += new System.EventHandler(this.onLoad);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.onFormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button m_btnClose;
        private System.Windows.Forms.Label m_lFindWhat;
        private System.Windows.Forms.TextBox m_textFindWhat;
        private System.Windows.Forms.Button m_btnFindNext;
        private CtrlFindOptions m_ctrlFindOptions;
    }
}