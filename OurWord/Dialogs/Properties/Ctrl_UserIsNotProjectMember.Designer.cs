namespace OurWord.Dialogs.Properties
{
    partial class Ctrl_UserIsNotProjectMember
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.m_labelInfo = new System.Windows.Forms.Label();
            this.m_bGrantMembership = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // m_labelInfo
            // 
            this.m_labelInfo.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.m_labelInfo.Location = new System.Drawing.Point(21, 18);
            this.m_labelInfo.Name = "m_labelInfo";
            this.m_labelInfo.Size = new System.Drawing.Size(244, 69);
            this.m_labelInfo.TabIndex = 0;
            this.m_labelInfo.Text = "{0} is not a member of the {1} project. Click on the button below to grant member" +
                "ship, so that you can then set editing permissions.";
            this.m_labelInfo.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // m_bGrantMembership
            // 
            this.m_bGrantMembership.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.m_bGrantMembership.Location = new System.Drawing.Point(100, 100);
            this.m_bGrantMembership.Name = "m_bGrantMembership";
            this.m_bGrantMembership.Size = new System.Drawing.Size(75, 41);
            this.m_bGrantMembership.TabIndex = 1;
            this.m_bGrantMembership.Text = "Grant Membership";
            this.m_bGrantMembership.UseVisualStyleBackColor = true;
            this.m_bGrantMembership.Click += new System.EventHandler(this.cmdGrantMembership);
            // 
            // Ctrl_UserIsNotProjectMember
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.Cornsilk;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.m_bGrantMembership);
            this.Controls.Add(this.m_labelInfo);
            this.Name = "Ctrl_UserIsNotProjectMember";
            this.Size = new System.Drawing.Size(282, 176);
            this.Load += new System.EventHandler(this.cmdLoad);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label m_labelInfo;
        private System.Windows.Forms.Button m_bGrantMembership;
    }
}
