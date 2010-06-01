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
            this.SuspendLayout();
            // 
            // contentPanel
            // 
            this.contentPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.contentPanel.BackColor = System.Drawing.Color.PaleGreen;
            this.contentPanel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.contentPanel.Location = new System.Drawing.Point(12, 56);
            this.contentPanel.Name = "contentPanel";
            this.contentPanel.Size = new System.Drawing.Size(200, 100);
            this.contentPanel.TabIndex = 1;
            // 
            // nav
            // 
            this.nav.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.nav.BackColor = System.Drawing.Color.DarkGray;
            this.nav.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.nav.Location = new System.Drawing.Point(174, 0);
            this.nav.Name = "nav";
            this.nav.Size = new System.Drawing.Size(287, 70);
            this.nav.TabIndex = 0;
            // 
            // TestDlg
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.MintCream;
            this.ClientSize = new System.Drawing.Size(460, 204);
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
    }
}