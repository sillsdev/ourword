namespace OurWordSetup.UI
{
    partial class DlgMessage
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
            this.m_pErrorIcon = new System.Windows.Forms.PictureBox();
            this.m_Message = new System.Windows.Forms.Label();
            this.m_bOk = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.m_pErrorIcon)).BeginInit();
            this.SuspendLayout();
            // 
            // m_pErrorIcon
            // 
            this.m_pErrorIcon.Location = new System.Drawing.Point(12, 12);
            this.m_pErrorIcon.Name = "m_pErrorIcon";
            this.m_pErrorIcon.Size = new System.Drawing.Size(44, 44);
            this.m_pErrorIcon.TabIndex = 0;
            this.m_pErrorIcon.TabStop = false;
            // 
            // m_Message
            // 
            this.m_Message.Location = new System.Drawing.Point(75, 12);
            this.m_Message.Name = "m_Message";
            this.m_Message.Size = new System.Drawing.Size(291, 68);
            this.m_Message.TabIndex = 1;
            this.m_Message.Text = "<message>";
            // 
            // m_bOk
            // 
            this.m_bOk.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.m_bOk.Location = new System.Drawing.Point(150, 90);
            this.m_bOk.Name = "m_bOk";
            this.m_bOk.Size = new System.Drawing.Size(75, 23);
            this.m_bOk.TabIndex = 2;
            this.m_bOk.Text = "OK";
            this.m_bOk.UseVisualStyleBackColor = true;
            // 
            // DlgMessage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.m_bOk;
            this.ClientSize = new System.Drawing.Size(377, 125);
            this.Controls.Add(this.m_bOk);
            this.Controls.Add(this.m_Message);
            this.Controls.Add(this.m_pErrorIcon);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DlgMessage";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "OurWord Setup";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.cmdLoad);
            ((System.ComponentModel.ISupportInitialize)(this.m_pErrorIcon)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox m_pErrorIcon;
        private System.Windows.Forms.Label m_Message;
        private System.Windows.Forms.Button m_bOk;
    }
}