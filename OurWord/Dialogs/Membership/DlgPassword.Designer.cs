namespace OurWord.Dialogs.Membership
{
    partial class DlgPassword
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DlgPassword));
            this.m_btnCancel = new System.Windows.Forms.Button();
            this.m_btnOK = new System.Windows.Forms.Button();
            this.m_labelEnterPassword = new System.Windows.Forms.Label();
            this.m_textPassword = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // m_btnCancel
            // 
            this.m_btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.m_btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.m_btnCancel.Location = new System.Drawing.Point(150, 76);
            this.m_btnCancel.Name = "m_btnCancel";
            this.m_btnCancel.Size = new System.Drawing.Size(75, 23);
            this.m_btnCancel.TabIndex = 3;
            this.m_btnCancel.Text = "Cancel";
            // 
            // m_btnOK
            // 
            this.m_btnOK.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.m_btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.m_btnOK.Location = new System.Drawing.Point(62, 76);
            this.m_btnOK.Name = "m_btnOK";
            this.m_btnOK.Size = new System.Drawing.Size(75, 23);
            this.m_btnOK.TabIndex = 2;
            this.m_btnOK.Text = "OK";
            // 
            // m_labelEnterPassword
            // 
            this.m_labelEnterPassword.Location = new System.Drawing.Point(12, 9);
            this.m_labelEnterPassword.Name = "m_labelEnterPassword";
            this.m_labelEnterPassword.Size = new System.Drawing.Size(268, 23);
            this.m_labelEnterPassword.TabIndex = 22;
            this.m_labelEnterPassword.Text = "Please enter the password for \"{0}\":";
            this.m_labelEnterPassword.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // m_textPassword
            // 
            this.m_textPassword.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_textPassword.HideSelection = false;
            this.m_textPassword.Location = new System.Drawing.Point(62, 35);
            this.m_textPassword.Name = "m_textPassword";
            this.m_textPassword.Size = new System.Drawing.Size(163, 21);
            this.m_textPassword.TabIndex = 1;
            this.m_textPassword.UseSystemPasswordChar = true;
            this.m_textPassword.KeyDown += new System.Windows.Forms.KeyEventHandler(this.cmdKeyDown);
            // 
            // DlgPassword
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(292, 107);
            this.Controls.Add(this.m_textPassword);
            this.Controls.Add(this.m_labelEnterPassword);
            this.Controls.Add(this.m_btnCancel);
            this.Controls.Add(this.m_btnOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DlgPassword";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Enter Password";
            this.Load += new System.EventHandler(this.cmdLoad);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button m_btnCancel;
        private System.Windows.Forms.Button m_btnOK;
        private System.Windows.Forms.Label m_labelEnterPassword;
        private System.Windows.Forms.TextBox m_textPassword;
    }
}