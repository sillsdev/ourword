namespace OurWord.Ctrls
{
    partial class TestDlg
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
            this.contentPanel = new System.Windows.Forms.Panel();
            this.nav = new OurWord.Ctrls.Navigation.CtrlNavigation();
            this.m_Commands = new OurWord.Ctrls.Commands.CtrlCommands();
            this.SuspendLayout();
            // 
            // contentPanel
            // 
            this.contentPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.contentPanel.BackColor = System.Drawing.Color.PaleGreen;
            this.contentPanel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.contentPanel.Location = new System.Drawing.Point(55, 92);
            this.contentPanel.Name = "contentPanel";
            this.contentPanel.Size = new System.Drawing.Size(597, 100);
            this.contentPanel.TabIndex = 1;
            // 
            // nav
            // 
            this.nav.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.nav.BackColor = System.Drawing.Color.DarkGray;
            this.nav.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.nav.Location = new System.Drawing.Point(571, 0);
            this.nav.Name = "nav";
            this.nav.Size = new System.Drawing.Size(287, 78);
            this.nav.TabIndex = 0;
            // 
            // m_Commands
            // 
            this.m_Commands.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.m_Commands.BackColor = System.Drawing.Color.DarkGray;
            this.m_Commands.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.m_Commands.Location = new System.Drawing.Point(0, 0);
            this.m_Commands.Name = "m_Commands";
            this.m_Commands.Size = new System.Drawing.Size(571, 86);
            this.m_Commands.TabIndex = 2;
            // 
            // TestDlg
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.MintCream;
            this.ClientSize = new System.Drawing.Size(857, 204);
            this.Controls.Add(this.m_Commands);
            this.Controls.Add(this.contentPanel);
            this.Controls.Add(this.nav);
            this.Name = "TestDlg";
            this.Text = "TestDlg";
            this.Load += new System.EventHandler(this.cmdLoad);
            this.ResumeLayout(false);

        }

        #endregion

        private OurWord.Ctrls.Navigation.CtrlNavigation nav;
        private System.Windows.Forms.Panel contentPanel;
        private OurWord.Ctrls.Commands.CtrlCommands m_Commands;
    }
}