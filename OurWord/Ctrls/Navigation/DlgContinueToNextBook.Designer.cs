namespace OurWord.Ctrls.Navigation
{
    partial class DlgContinueToNextBook
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
            this.m_checkDontAskAgain = new System.Windows.Forms.CheckBox();
            this.m_lQuestion = new System.Windows.Forms.Label();
            this.m_btnYes = new System.Windows.Forms.Button();
            this.m_btnNo = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // m_checkDontAskAgain
            // 
            this.m_checkDontAskAgain.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.m_checkDontAskAgain.AutoSize = true;
            this.m_checkDontAskAgain.Location = new System.Drawing.Point(12, 52);
            this.m_checkDontAskAgain.Name = "m_checkDontAskAgain";
            this.m_checkDontAskAgain.Size = new System.Drawing.Size(100, 17);
            this.m_checkDontAskAgain.TabIndex = 0;
            this.m_checkDontAskAgain.Text = "Don\'t ask again";
            this.m_checkDontAskAgain.UseVisualStyleBackColor = true;
            // 
            // m_lQuestion
            // 
            this.m_lQuestion.AutoSize = true;
            this.m_lQuestion.Location = new System.Drawing.Point(12, 9);
            this.m_lQuestion.Name = "m_lQuestion";
            this.m_lQuestion.Size = new System.Drawing.Size(244, 13);
            this.m_lQuestion.TabIndex = 1;
            this.m_lQuestion.Text = "Do you wish to continue searching the next book?";
            // 
            // m_btnYes
            // 
            this.m_btnYes.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.m_btnYes.DialogResult = System.Windows.Forms.DialogResult.Yes;
            this.m_btnYes.Location = new System.Drawing.Point(165, 46);
            this.m_btnYes.Name = "m_btnYes";
            this.m_btnYes.Size = new System.Drawing.Size(75, 23);
            this.m_btnYes.TabIndex = 2;
            this.m_btnYes.Text = "Yes";
            this.m_btnYes.UseVisualStyleBackColor = true;
            // 
            // m_btnNo
            // 
            this.m_btnNo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.m_btnNo.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.m_btnNo.Location = new System.Drawing.Point(246, 46);
            this.m_btnNo.Name = "m_btnNo";
            this.m_btnNo.Size = new System.Drawing.Size(75, 23);
            this.m_btnNo.TabIndex = 3;
            this.m_btnNo.Text = "No";
            this.m_btnNo.UseVisualStyleBackColor = true;
            // 
            // DlgContinueToNextBook
            // 
            this.AcceptButton = this.m_btnYes;
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.m_btnNo;
            this.ClientSize = new System.Drawing.Size(332, 81);
            this.ControlBox = false;
            this.Controls.Add(this.m_btnNo);
            this.Controls.Add(this.m_btnYes);
            this.Controls.Add(this.m_lQuestion);
            this.Controls.Add(this.m_checkDontAskAgain);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "DlgContinueToNextBook";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Find";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.cmdLoad);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox m_checkDontAskAgain;
        private System.Windows.Forms.Label m_lQuestion;
        private System.Windows.Forms.Button m_btnYes;
        private System.Windows.Forms.Button m_btnNo;
    }
}