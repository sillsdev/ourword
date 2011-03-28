namespace OurWord.Ctrls.Navigation
{
    partial class DlgFindAndReplace
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DlgFindAndReplace));
            this.m_btnClose = new System.Windows.Forms.Button();
            this.m_btnReplace = new System.Windows.Forms.Button();
            this.m_lFindWhat = new System.Windows.Forms.Label();
            this.m_textFindWhat = new System.Windows.Forms.TextBox();
            this.m_textReplaceWith = new System.Windows.Forms.TextBox();
            this.m_lReplaceWith = new System.Windows.Forms.Label();
            this.m_btnReplaceAll = new System.Windows.Forms.Button();
            this.m_btnFindNext = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // m_btnClose
            // 
            this.m_btnClose.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.m_btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.m_btnClose.Location = new System.Drawing.Point(285, 77);
            this.m_btnClose.Name = "m_btnClose";
            this.m_btnClose.Size = new System.Drawing.Size(75, 23);
            this.m_btnClose.TabIndex = 13;
            this.m_btnClose.Text = "Close";
            this.m_btnClose.Click += new System.EventHandler(this.cmdClose);
            // 
            // m_btnReplace
            // 
            this.m_btnReplace.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.m_btnReplace.Location = new System.Drawing.Point(42, 77);
            this.m_btnReplace.Name = "m_btnReplace";
            this.m_btnReplace.Size = new System.Drawing.Size(75, 23);
            this.m_btnReplace.TabIndex = 10;
            this.m_btnReplace.Text = "Replace";
            this.m_btnReplace.Click += new System.EventHandler(this.cmdReplace);
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
            this.m_textFindWhat.Location = new System.Drawing.Point(98, 6);
            this.m_textFindWhat.Name = "m_textFindWhat";
            this.m_textFindWhat.Size = new System.Drawing.Size(297, 20);
            this.m_textFindWhat.TabIndex = 1;
            this.m_textFindWhat.TextChanged += new System.EventHandler(this.onFindWhatChanged);
            // 
            // m_textReplaceWith
            // 
            this.m_textReplaceWith.Location = new System.Drawing.Point(98, 37);
            this.m_textReplaceWith.Name = "m_textReplaceWith";
            this.m_textReplaceWith.Size = new System.Drawing.Size(297, 20);
            this.m_textReplaceWith.TabIndex = 2;
            this.m_textReplaceWith.TextChanged += new System.EventHandler(this.onReplaceWithChanged);
            // 
            // m_lReplaceWith
            // 
            this.m_lReplaceWith.AutoSize = true;
            this.m_lReplaceWith.Location = new System.Drawing.Point(12, 40);
            this.m_lReplaceWith.Name = "m_lReplaceWith";
            this.m_lReplaceWith.Size = new System.Drawing.Size(75, 13);
            this.m_lReplaceWith.TabIndex = 22;
            this.m_lReplaceWith.Text = "Replace With:";
            // 
            // m_btnReplaceAll
            // 
            this.m_btnReplaceAll.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.m_btnReplaceAll.Location = new System.Drawing.Point(123, 77);
            this.m_btnReplaceAll.Name = "m_btnReplaceAll";
            this.m_btnReplaceAll.Size = new System.Drawing.Size(75, 23);
            this.m_btnReplaceAll.TabIndex = 11;
            this.m_btnReplaceAll.Text = "Replace All";
            this.m_btnReplaceAll.Click += new System.EventHandler(this.cmdReplaceAll);
            // 
            // m_btnFindNext
            // 
            this.m_btnFindNext.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.m_btnFindNext.Location = new System.Drawing.Point(204, 77);
            this.m_btnFindNext.Name = "m_btnFindNext";
            this.m_btnFindNext.Size = new System.Drawing.Size(75, 23);
            this.m_btnFindNext.TabIndex = 12;
            this.m_btnFindNext.Text = "Find Next";
            this.m_btnFindNext.Click += new System.EventHandler(this.cmdFindNext);
            // 
            // DlgFindAndReplace
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.m_btnClose;
            this.ClientSize = new System.Drawing.Size(407, 109);
            this.Controls.Add(this.m_btnFindNext);
            this.Controls.Add(this.m_btnReplaceAll);
            this.Controls.Add(this.m_textReplaceWith);
            this.Controls.Add(this.m_lReplaceWith);
            this.Controls.Add(this.m_textFindWhat);
            this.Controls.Add(this.m_lFindWhat);
            this.Controls.Add(this.m_btnClose);
            this.Controls.Add(this.m_btnReplace);
            this.DoubleBuffered = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DlgFindAndReplace";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Find and Replace";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.onLoad);
            this.Activated += new System.EventHandler(this.onActivated);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.onFormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button m_btnClose;
        private System.Windows.Forms.Button m_btnReplace;
        private System.Windows.Forms.Label m_lFindWhat;
        private System.Windows.Forms.TextBox m_textFindWhat;
        private System.Windows.Forms.TextBox m_textReplaceWith;
        private System.Windows.Forms.Label m_lReplaceWith;
        private System.Windows.Forms.Button m_btnReplaceAll;
        private System.Windows.Forms.Button m_btnFindNext;
    }
}