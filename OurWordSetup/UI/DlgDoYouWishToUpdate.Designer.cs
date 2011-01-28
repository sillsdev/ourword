namespace OurWordSetup.UI
{
    partial class DlgDoYouWishToUpdate
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
            this.m_labelUpdateIsAvailable = new System.Windows.Forms.Label();
            this.m_btnCancel = new System.Windows.Forms.Button();
            this.m_btnYes = new System.Windows.Forms.Button();
            this.m_labelYoursIs = new System.Windows.Forms.Label();
            this.m_labelRemoteIs = new System.Windows.Forms.Label();
            this.m_labelYourVersion = new System.Windows.Forms.Label();
            this.m_labelRemoteVersion = new System.Windows.Forms.Label();
            this.m_labelShallWe = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // m_labelUpdateIsAvailable
            // 
            this.m_labelUpdateIsAvailable.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_labelUpdateIsAvailable.Location = new System.Drawing.Point(16, 9);
            this.m_labelUpdateIsAvailable.Name = "m_labelUpdateIsAvailable";
            this.m_labelUpdateIsAvailable.Size = new System.Drawing.Size(330, 22);
            this.m_labelUpdateIsAvailable.TabIndex = 0;
            this.m_labelUpdateIsAvailable.Text = "A newer version of OurWord is available.";
            this.m_labelUpdateIsAvailable.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // m_btnCancel
            // 
            this.m_btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.m_btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.m_btnCancel.Location = new System.Drawing.Point(186, 139);
            this.m_btnCancel.Name = "m_btnCancel";
            this.m_btnCancel.Size = new System.Drawing.Size(80, 22);
            this.m_btnCancel.TabIndex = 19;
            this.m_btnCancel.Text = "Cancel";
            // 
            // m_btnYes
            // 
            this.m_btnYes.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.m_btnYes.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.m_btnYes.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_btnYes.Location = new System.Drawing.Point(93, 139);
            this.m_btnYes.Name = "m_btnYes";
            this.m_btnYes.Size = new System.Drawing.Size(80, 22);
            this.m_btnYes.TabIndex = 18;
            this.m_btnYes.Text = "Update";
            // 
            // m_labelYoursIs
            // 
            this.m_labelYoursIs.Location = new System.Drawing.Point(16, 44);
            this.m_labelYoursIs.Name = "m_labelYoursIs";
            this.m_labelYoursIs.Size = new System.Drawing.Size(161, 22);
            this.m_labelYoursIs.TabIndex = 20;
            this.m_labelYoursIs.Text = "Your version is:";
            this.m_labelYoursIs.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // m_labelRemoteIs
            // 
            this.m_labelRemoteIs.Location = new System.Drawing.Point(19, 74);
            this.m_labelRemoteIs.Name = "m_labelRemoteIs";
            this.m_labelRemoteIs.Size = new System.Drawing.Size(158, 22);
            this.m_labelRemoteIs.TabIndex = 21;
            this.m_labelRemoteIs.Text = "The new version is:";
            this.m_labelRemoteIs.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // m_labelYourVersion
            // 
            this.m_labelYourVersion.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.m_labelYourVersion.Location = new System.Drawing.Point(179, 44);
            this.m_labelYourVersion.Name = "m_labelYourVersion";
            this.m_labelYourVersion.Size = new System.Drawing.Size(73, 22);
            this.m_labelYourVersion.TabIndex = 22;
            this.m_labelYourVersion.Text = "1.0";
            this.m_labelYourVersion.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // m_labelRemoteVersion
            // 
            this.m_labelRemoteVersion.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.m_labelRemoteVersion.Location = new System.Drawing.Point(179, 74);
            this.m_labelRemoteVersion.Name = "m_labelRemoteVersion";
            this.m_labelRemoteVersion.Size = new System.Drawing.Size(73, 22);
            this.m_labelRemoteVersion.TabIndex = 23;
            this.m_labelRemoteVersion.Text = "1.0b";
            this.m_labelRemoteVersion.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // m_labelShallWe
            // 
            this.m_labelShallWe.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_labelShallWe.Location = new System.Drawing.Point(19, 106);
            this.m_labelShallWe.Name = "m_labelShallWe";
            this.m_labelShallWe.Size = new System.Drawing.Size(326, 22);
            this.m_labelShallWe.TabIndex = 24;
            this.m_labelShallWe.Text = "Do you wish to update?";
            this.m_labelShallWe.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // DlgDoYouWishToUpdate
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(358, 175);
            this.ControlBox = false;
            this.Controls.Add(this.m_labelShallWe);
            this.Controls.Add(this.m_labelRemoteVersion);
            this.Controls.Add(this.m_labelYourVersion);
            this.Controls.Add(this.m_labelRemoteIs);
            this.Controls.Add(this.m_labelYoursIs);
            this.Controls.Add(this.m_btnCancel);
            this.Controls.Add(this.m_btnYes);
            this.Controls.Add(this.m_labelUpdateIsAvailable);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "DlgDoYouWishToUpdate";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Update OurWord?";
            this.TopMost = true;
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label m_labelUpdateIsAvailable;
        private System.Windows.Forms.Button m_btnCancel;
        private System.Windows.Forms.Button m_btnYes;
        private System.Windows.Forms.Label m_labelYoursIs;
        private System.Windows.Forms.Label m_labelRemoteIs;
        private System.Windows.Forms.Label m_labelYourVersion;
        private System.Windows.Forms.Label m_labelRemoteVersion;
        private System.Windows.Forms.Label m_labelShallWe;
    }
}