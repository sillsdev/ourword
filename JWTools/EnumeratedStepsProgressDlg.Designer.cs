namespace JWTools
{
    partial class EnumeratedStepsProgressDlg
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EnumeratedStepsProgressDlg));
            this.m_progressBar = new System.Windows.Forms.ProgressBar();
            this.m_ProcessName = new System.Windows.Forms.Label();
            this.m_bOK = new System.Windows.Forms.Button();
            this.m_labelError = new System.Windows.Forms.Label();
            this.m_ErrorIcon = new System.Windows.Forms.PictureBox();
            this.m_ErrorPanel = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.m_ErrorIcon)).BeginInit();
            this.m_ErrorPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // m_progressBar
            // 
            this.m_progressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.m_progressBar.Location = new System.Drawing.Point(12, 299);
            this.m_progressBar.MarqueeAnimationSpeed = 50;
            this.m_progressBar.Name = "m_progressBar";
            this.m_progressBar.Size = new System.Drawing.Size(333, 23);
            this.m_progressBar.Step = 1;
            this.m_progressBar.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.m_progressBar.TabIndex = 0;
            // 
            // m_ProcessName
            // 
            this.m_ProcessName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.m_ProcessName.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_ProcessName.Location = new System.Drawing.Point(9, 9);
            this.m_ProcessName.Name = "m_ProcessName";
            this.m_ProcessName.Size = new System.Drawing.Size(336, 23);
            this.m_ProcessName.TabIndex = 1;
            this.m_ProcessName.Text = "Working...";
            this.m_ProcessName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // m_bOK
            // 
            this.m_bOK.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.m_bOK.Location = new System.Drawing.Point(146, 141);
            this.m_bOK.Name = "m_bOK";
            this.m_bOK.Size = new System.Drawing.Size(75, 23);
            this.m_bOK.TabIndex = 3;
            this.m_bOK.Text = "OK";
            this.m_bOK.UseVisualStyleBackColor = true;
            this.m_bOK.Click += new System.EventHandler(this.OnDone);
            // 
            // m_labelError
            // 
            this.m_labelError.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.m_labelError.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_labelError.ForeColor = System.Drawing.Color.Red;
            this.m_labelError.Location = new System.Drawing.Point(45, 3);
            this.m_labelError.Name = "m_labelError";
            this.m_labelError.Size = new System.Drawing.Size(282, 90);
            this.m_labelError.TabIndex = 4;
            this.m_labelError.Text = "(Error)";
            // 
            // m_ErrorIcon
            // 
            this.m_ErrorIcon.Location = new System.Drawing.Point(3, 3);
            this.m_ErrorIcon.Name = "m_ErrorIcon";
            this.m_ErrorIcon.Size = new System.Drawing.Size(36, 36);
            this.m_ErrorIcon.TabIndex = 5;
            this.m_ErrorIcon.TabStop = false;
            // 
            // m_ErrorPanel
            // 
            this.m_ErrorPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.m_ErrorPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.m_ErrorPanel.Controls.Add(this.m_ErrorIcon);
            this.m_ErrorPanel.Controls.Add(this.m_labelError);
            this.m_ErrorPanel.ForeColor = System.Drawing.Color.Red;
            this.m_ErrorPanel.Location = new System.Drawing.Point(12, 35);
            this.m_ErrorPanel.Name = "m_ErrorPanel";
            this.m_ErrorPanel.Size = new System.Drawing.Size(333, 100);
            this.m_ErrorPanel.TabIndex = 6;
            // 
            // EnumeratedStepsProgressDlg
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.m_bOK;
            this.ClientSize = new System.Drawing.Size(357, 334);
            this.ControlBox = false;
            this.Controls.Add(this.m_ErrorPanel);
            this.Controls.Add(this.m_bOK);
            this.Controls.Add(this.m_ProcessName);
            this.Controls.Add(this.m_progressBar);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "EnumeratedStepsProgressDlg";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Progress";
            ((System.ComponentModel.ISupportInitialize)(this.m_ErrorIcon)).EndInit();
            this.m_ErrorPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ProgressBar m_progressBar;
        private System.Windows.Forms.Label m_ProcessName;
        private System.Windows.Forms.Button m_bOK;
        private System.Windows.Forms.Label m_labelError;
        private System.Windows.Forms.PictureBox m_ErrorIcon;
        private System.Windows.Forms.Panel m_ErrorPanel;
    }
}