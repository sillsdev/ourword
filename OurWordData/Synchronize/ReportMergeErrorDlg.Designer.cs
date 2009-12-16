namespace OurWordData.Synchronize
{
    partial class ReportMergeErrorDlg
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ReportMergeErrorDlg));
            this.m_labelTitle = new System.Windows.Forms.Label();
            this.m_labelExplanation = new System.Windows.Forms.Label();
            this.m_labelSendEmail = new System.Windows.Forms.Label();
            this.m_checkSendEmail = new System.Windows.Forms.CheckBox();
            this.m_btnOK = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // m_labelTitle
            // 
            this.m_labelTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_labelTitle.Location = new System.Drawing.Point(12, 9);
            this.m_labelTitle.Name = "m_labelTitle";
            this.m_labelTitle.Size = new System.Drawing.Size(420, 21);
            this.m_labelTitle.TabIndex = 0;
            this.m_labelTitle.Text = "A bug in OurWord is preventing your Send/Receive request.";
            // 
            // m_labelExplanation
            // 
            this.m_labelExplanation.Location = new System.Drawing.Point(12, 40);
            this.m_labelExplanation.Name = "m_labelExplanation";
            this.m_labelExplanation.Size = new System.Drawing.Size(420, 58);
            this.m_labelExplanation.TabIndex = 1;
            this.m_labelExplanation.Text = "This does not affect your data; nothing is lost and you can still translate. But " +
                "until the bug is fixed (and you receive a new version of OurWord) you will not b" +
                "e able to Send/Receive to the Internet.";
            // 
            // m_labelSendEmail
            // 
            this.m_labelSendEmail.Location = new System.Drawing.Point(12, 99);
            this.m_labelSendEmail.Name = "m_labelSendEmail";
            this.m_labelSendEmail.Size = new System.Drawing.Size(420, 48);
            this.m_labelSendEmail.TabIndex = 2;
            this.m_labelSendEmail.Text = "Please permit OurWord to send an email with the information needed to diagnose an" +
                "d fix the problem. (Note that your email client may require a separate confirmat" +
                "ion; just answer \'ok\' or \'yes\'.)";
            // 
            // m_checkSendEmail
            // 
            this.m_checkSendEmail.AutoSize = true;
            this.m_checkSendEmail.Location = new System.Drawing.Point(36, 145);
            this.m_checkSendEmail.Name = "m_checkSendEmail";
            this.m_checkSendEmail.Size = new System.Drawing.Size(263, 17);
            this.m_checkSendEmail.TabIndex = 3;
            this.m_checkSendEmail.Text = "Allow OurWord to send the bug report in an email?";
            this.m_checkSendEmail.UseVisualStyleBackColor = true;
            // 
            // m_btnOK
            // 
            this.m_btnOK.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.m_btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.m_btnOK.Location = new System.Drawing.Point(179, 186);
            this.m_btnOK.Name = "m_btnOK";
            this.m_btnOK.Size = new System.Drawing.Size(75, 23);
            this.m_btnOK.TabIndex = 4;
            this.m_btnOK.Text = "OK";
            this.m_btnOK.UseVisualStyleBackColor = true;
            // 
            // ReportMergeErrorDlg
            // 
            this.AcceptButton = this.m_btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.PaleGreen;
            this.CancelButton = this.m_btnOK;
            this.ClientSize = new System.Drawing.Size(444, 221);
            this.Controls.Add(this.m_btnOK);
            this.Controls.Add(this.m_checkSendEmail);
            this.Controls.Add(this.m_labelSendEmail);
            this.Controls.Add(this.m_labelExplanation);
            this.Controls.Add(this.m_labelTitle);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ReportMergeErrorDlg";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Send/Receive Error";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.cmdLoad);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label m_labelTitle;
        private System.Windows.Forms.Label m_labelExplanation;
        private System.Windows.Forms.Label m_labelSendEmail;
        private System.Windows.Forms.CheckBox m_checkSendEmail;
        private System.Windows.Forms.Button m_btnOK;
    }
}