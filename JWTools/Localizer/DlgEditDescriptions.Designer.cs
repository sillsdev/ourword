namespace JWTools
{
    partial class DlgEditDescriptions
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
            this.m_btnOK = new System.Windows.Forms.Button();
            this.m_lblGroupDescription = new System.Windows.Forms.Label();
            this.m_lblItemDescription = new System.Windows.Forms.Label();
            this.m_textGroupDescription = new System.Windows.Forms.TextBox();
            this.m_textItemDescription = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // m_btnOK
            // 
            this.m_btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.m_btnOK.Location = new System.Drawing.Point(244, 348);
            this.m_btnOK.Name = "m_btnOK";
            this.m_btnOK.Size = new System.Drawing.Size(75, 23);
            this.m_btnOK.TabIndex = 29;
            this.m_btnOK.Text = "OK";
            // 
            // m_lblGroupDescription
            // 
            this.m_lblGroupDescription.AutoSize = true;
            this.m_lblGroupDescription.Location = new System.Drawing.Point(12, 9);
            this.m_lblGroupDescription.Name = "m_lblGroupDescription";
            this.m_lblGroupDescription.Size = new System.Drawing.Size(92, 13);
            this.m_lblGroupDescription.TabIndex = 30;
            this.m_lblGroupDescription.Text = "Group Description";
            // 
            // m_lblItemDescription
            // 
            this.m_lblItemDescription.AutoSize = true;
            this.m_lblItemDescription.Location = new System.Drawing.Point(12, 178);
            this.m_lblItemDescription.Name = "m_lblItemDescription";
            this.m_lblItemDescription.Size = new System.Drawing.Size(83, 13);
            this.m_lblItemDescription.TabIndex = 31;
            this.m_lblItemDescription.Text = "Item Description";
            // 
            // m_textGroupDescription
            // 
            this.m_textGroupDescription.Location = new System.Drawing.Point(15, 25);
            this.m_textGroupDescription.Multiline = true;
            this.m_textGroupDescription.Name = "m_textGroupDescription";
            this.m_textGroupDescription.Size = new System.Drawing.Size(529, 131);
            this.m_textGroupDescription.TabIndex = 32;
            // 
            // m_textItemDescription
            // 
            this.m_textItemDescription.Location = new System.Drawing.Point(12, 194);
            this.m_textItemDescription.Multiline = true;
            this.m_textItemDescription.Name = "m_textItemDescription";
            this.m_textItemDescription.Size = new System.Drawing.Size(529, 131);
            this.m_textItemDescription.TabIndex = 33;
            // 
            // DlgEditDescriptions
            // 
            this.AcceptButton = this.m_btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(556, 383);
            this.ControlBox = false;
            this.Controls.Add(this.m_textItemDescription);
            this.Controls.Add(this.m_textGroupDescription);
            this.Controls.Add(this.m_lblItemDescription);
            this.Controls.Add(this.m_lblGroupDescription);
            this.Controls.Add(this.m_btnOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "DlgEditDescriptions";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Edit Descriptions";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button m_btnOK;
        private System.Windows.Forms.Label m_lblGroupDescription;
        private System.Windows.Forms.Label m_lblItemDescription;
        private System.Windows.Forms.TextBox m_textGroupDescription;
        private System.Windows.Forms.TextBox m_textItemDescription;
    }
}