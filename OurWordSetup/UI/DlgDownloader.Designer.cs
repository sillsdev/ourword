namespace OurWordSetup.UI
{
    partial class DlgDownloader
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DlgDownloader));
            this.m_labelPleaseWait = new System.Windows.Forms.Label();
            this.m_labelCurrentStatus = new System.Windows.Forms.Label();
            this.m_btnCancel = new System.Windows.Forms.Button();
            this.m_OurWordIcon = new System.Windows.Forms.PictureBox();
            this.m_ProgressBar = new System.Windows.Forms.ProgressBar();
            ((System.ComponentModel.ISupportInitialize)(this.m_OurWordIcon)).BeginInit();
            this.SuspendLayout();
            // 
            // m_labelPleaseWait
            // 
            this.m_labelPleaseWait.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.m_labelPleaseWait.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_labelPleaseWait.Location = new System.Drawing.Point(90, 9);
            this.m_labelPleaseWait.Name = "m_labelPleaseWait";
            this.m_labelPleaseWait.Size = new System.Drawing.Size(403, 46);
            this.m_labelPleaseWait.TabIndex = 0;
            this.m_labelPleaseWait.Text = "Please wait while OurWord downloads and installs the update...";
            // 
            // m_labelCurrentStatus
            // 
            this.m_labelCurrentStatus.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.m_labelCurrentStatus.Location = new System.Drawing.Point(93, 66);
            this.m_labelCurrentStatus.Name = "m_labelCurrentStatus";
            this.m_labelCurrentStatus.Size = new System.Drawing.Size(400, 23);
            this.m_labelCurrentStatus.TabIndex = 3;
            this.m_labelCurrentStatus.Text = "Status: Preparing to download...";
            this.m_labelCurrentStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // m_btnCancel
            // 
            this.m_btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.m_btnCancel.Location = new System.Drawing.Point(218, 139);
            this.m_btnCancel.Name = "m_btnCancel";
            this.m_btnCancel.Size = new System.Drawing.Size(75, 23);
            this.m_btnCancel.TabIndex = 4;
            this.m_btnCancel.Text = "Cancel";
            this.m_btnCancel.UseVisualStyleBackColor = true;
            this.m_btnCancel.Click += new System.EventHandler(this.cmdCancel);
            // 
            // m_OurWordIcon
            // 
            this.m_OurWordIcon.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.m_OurWordIcon.Image = ((System.Drawing.Image)(resources.GetObject("m_OurWordIcon.Image")));
            this.m_OurWordIcon.Location = new System.Drawing.Point(12, 9);
            this.m_OurWordIcon.Name = "m_OurWordIcon";
            this.m_OurWordIcon.Size = new System.Drawing.Size(72, 80);
            this.m_OurWordIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.m_OurWordIcon.TabIndex = 11;
            this.m_OurWordIcon.TabStop = false;
            // 
            // m_ProgressBar
            // 
            this.m_ProgressBar.Location = new System.Drawing.Point(93, 92);
            this.m_ProgressBar.Name = "m_ProgressBar";
            this.m_ProgressBar.Size = new System.Drawing.Size(397, 23);
            this.m_ProgressBar.TabIndex = 12;
            // 
            // DlgDownloader
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(505, 174);
            this.ControlBox = false;
            this.Controls.Add(this.m_ProgressBar);
            this.Controls.Add(this.m_OurWordIcon);
            this.Controls.Add(this.m_btnCancel);
            this.Controls.Add(this.m_labelCurrentStatus);
            this.Controls.Add(this.m_labelPleaseWait);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DlgDownloader";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "OurWord Setup";
            this.Shown += new System.EventHandler(this.cmdFormFirstShown);
            ((System.ComponentModel.ISupportInitialize)(this.m_OurWordIcon)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label m_labelPleaseWait;
        private System.Windows.Forms.Label m_labelCurrentStatus;
        private System.Windows.Forms.Button m_btnCancel;
        private System.Windows.Forms.PictureBox m_OurWordIcon;
        private System.Windows.Forms.ProgressBar m_ProgressBar;
    }
}