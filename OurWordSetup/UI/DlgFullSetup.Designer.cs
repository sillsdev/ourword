namespace OurWordSetup.UI
{
    partial class DlgFullSetup
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DlgFullSetup));
            this.m_btnCancel = new System.Windows.Forms.Button();
            this.m_btnInstall = new System.Windows.Forms.Button();
            this.m_labelExplanation = new System.Windows.Forms.Label();
            this.m_labelClickToBegin = new System.Windows.Forms.Label();
            this.m_OurWordIcon = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.m_OurWordIcon)).BeginInit();
            this.SuspendLayout();
            // 
            // m_btnCancel
            // 
            this.m_btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.m_btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.m_btnCancel.Location = new System.Drawing.Point(190, 117);
            this.m_btnCancel.Name = "m_btnCancel";
            this.m_btnCancel.Size = new System.Drawing.Size(75, 23);
            this.m_btnCancel.TabIndex = 5;
            this.m_btnCancel.Text = "Cancel";
            this.m_btnCancel.UseVisualStyleBackColor = true;
            this.m_btnCancel.Click += new System.EventHandler(this.cmdCancel);
            // 
            // m_btnInstall
            // 
            this.m_btnInstall.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.m_btnInstall.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.m_btnInstall.Location = new System.Drawing.Point(109, 117);
            this.m_btnInstall.Name = "m_btnInstall";
            this.m_btnInstall.Size = new System.Drawing.Size(75, 23);
            this.m_btnInstall.TabIndex = 6;
            this.m_btnInstall.Text = "Install";
            this.m_btnInstall.UseVisualStyleBackColor = true;
            this.m_btnInstall.Click += new System.EventHandler(this.cmdInstall);
            // 
            // m_labelExplanation
            // 
            this.m_labelExplanation.Location = new System.Drawing.Point(90, 9);
            this.m_labelExplanation.Name = "m_labelExplanation";
            this.m_labelExplanation.Size = new System.Drawing.Size(277, 45);
            this.m_labelExplanation.TabIndex = 7;
            this.m_labelExplanation.Text = "Setup will download and install the latest version of OurWord onto your computer." +
                "";
            // 
            // m_labelClickToBegin
            // 
            this.m_labelClickToBegin.Location = new System.Drawing.Point(93, 54);
            this.m_labelClickToBegin.Name = "m_labelClickToBegin";
            this.m_labelClickToBegin.Size = new System.Drawing.Size(274, 25);
            this.m_labelClickToBegin.TabIndex = 8;
            this.m_labelClickToBegin.Text = "Click on the Install button to begin.";
            // 
            // m_OurWordIcon
            // 
            this.m_OurWordIcon.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.m_OurWordIcon.Image = ((System.Drawing.Image)(resources.GetObject("m_OurWordIcon.Image")));
            this.m_OurWordIcon.Location = new System.Drawing.Point(12, 9);
            this.m_OurWordIcon.Name = "m_OurWordIcon";
            this.m_OurWordIcon.Size = new System.Drawing.Size(72, 80);
            this.m_OurWordIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.m_OurWordIcon.TabIndex = 10;
            this.m_OurWordIcon.TabStop = false;
            // 
            // DlgFullSetup
            // 
            this.AcceptButton = this.m_btnInstall;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.m_btnCancel;
            this.ClientSize = new System.Drawing.Size(379, 152);
            this.Controls.Add(this.m_OurWordIcon);
            this.Controls.Add(this.m_labelClickToBegin);
            this.Controls.Add(this.m_labelExplanation);
            this.Controls.Add(this.m_btnInstall);
            this.Controls.Add(this.m_btnCancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DlgFullSetup";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "OurWord Setup";
            ((System.ComponentModel.ISupportInitialize)(this.m_OurWordIcon)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button m_btnCancel;
        private System.Windows.Forms.Button m_btnInstall;
        private System.Windows.Forms.Label m_labelExplanation;
        private System.Windows.Forms.Label m_labelClickToBegin;
        private System.Windows.Forms.PictureBox m_OurWordIcon;
    }
}