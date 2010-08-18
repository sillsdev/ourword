namespace OurWordSetup.UI
{
    partial class DlgCheckingForUpdates
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
            this.m_labelIsChecking = new System.Windows.Forms.Label();
            this.m_labelStatus = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // m_labelIsChecking
            // 
            this.m_labelIsChecking.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_labelIsChecking.Location = new System.Drawing.Point(16, 11);
            this.m_labelIsChecking.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.m_labelIsChecking.Name = "m_labelIsChecking";
            this.m_labelIsChecking.Size = new System.Drawing.Size(485, 47);
            this.m_labelIsChecking.TabIndex = 0;
            this.m_labelIsChecking.Text = "OurWord is checking the Internet for updates...";
            this.m_labelIsChecking.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // m_labelStatus
            // 
            this.m_labelStatus.Location = new System.Drawing.Point(16, 58);
            this.m_labelStatus.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.m_labelStatus.Name = "m_labelStatus";
            this.m_labelStatus.Size = new System.Drawing.Size(485, 34);
            this.m_labelStatus.TabIndex = 1;
            this.m_labelStatus.Text = "Checking Internet Access...";
            this.m_labelStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // DlgCheckingForUpdates
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(120F, 120F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(517, 122);
            this.ControlBox = false;
            this.Controls.Add(this.m_labelStatus);
            this.Controls.Add(this.m_labelIsChecking);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "DlgCheckingForUpdates";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "OurWord";
            this.TopMost = true;
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label m_labelIsChecking;
        private System.Windows.Forms.Label m_labelStatus;
    }
}