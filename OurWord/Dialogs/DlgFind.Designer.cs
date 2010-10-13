namespace OurWord.Dialogs
{
    partial class DlgFind
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DlgFind));
            this.m_lFind = new System.Windows.Forms.Label();
            this.m_tFind = new System.Windows.Forms.TextBox();
            this.m_bFindNext = new System.Windows.Forms.Button();
            this.m_bClose = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // m_lFind
            // 
            this.m_lFind.Location = new System.Drawing.Point(12, 9);
            this.m_lFind.Name = "m_lFind";
            this.m_lFind.Size = new System.Drawing.Size(78, 23);
            this.m_lFind.TabIndex = 0;
            this.m_lFind.Text = "Search For:";
            this.m_lFind.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // m_tFind
            // 
            this.m_tFind.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.m_tFind.Location = new System.Drawing.Point(96, 11);
            this.m_tFind.Name = "m_tFind";
            this.m_tFind.Size = new System.Drawing.Size(147, 20);
            this.m_tFind.TabIndex = 1;
            // 
            // m_bFindNext
            // 
            this.m_bFindNext.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.m_bFindNext.Image = ((System.Drawing.Image)(resources.GetObject("m_bFindNext.Image")));
            this.m_bFindNext.Location = new System.Drawing.Point(33, 43);
            this.m_bFindNext.Name = "m_bFindNext";
            this.m_bFindNext.Size = new System.Drawing.Size(90, 23);
            this.m_bFindNext.TabIndex = 2;
            this.m_bFindNext.Text = "Find Next";
            this.m_bFindNext.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.m_bFindNext.UseVisualStyleBackColor = true;
            this.m_bFindNext.Click += new System.EventHandler(this.cmdFindNext);
            // 
            // m_bClose
            // 
            this.m_bClose.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.m_bClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.m_bClose.Location = new System.Drawing.Point(129, 43);
            this.m_bClose.Name = "m_bClose";
            this.m_bClose.Size = new System.Drawing.Size(90, 23);
            this.m_bClose.TabIndex = 3;
            this.m_bClose.Text = "Close";
            this.m_bClose.UseVisualStyleBackColor = true;
            this.m_bClose.Click += new System.EventHandler(this.cmdClose);
            // 
            // DlgFind
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.m_bClose;
            this.ClientSize = new System.Drawing.Size(255, 75);
            this.Controls.Add(this.m_bClose);
            this.Controls.Add(this.m_bFindNext);
            this.Controls.Add(this.m_tFind);
            this.Controls.Add(this.m_lFind);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DlgFind";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Find";
            this.Load += new System.EventHandler(this.cmdLoad);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.cmdFormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label m_lFind;
        private System.Windows.Forms.TextBox m_tFind;
        private System.Windows.Forms.Button m_bFindNext;
        private System.Windows.Forms.Button m_bClose;
    }
}