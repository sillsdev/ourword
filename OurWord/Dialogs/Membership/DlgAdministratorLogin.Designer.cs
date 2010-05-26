namespace OurWord.Dialogs.Membership
{
    partial class DlgAdministratorLogin
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DlgAdministratorLogin));
            this.m_textPassword = new System.Windows.Forms.TextBox();
            this.m_labelPassword = new System.Windows.Forms.Label();
            this.m_btnCancel = new System.Windows.Forms.Button();
            this.m_btnOK = new System.Windows.Forms.Button();
            this.m_labelAdministratorRequired = new System.Windows.Forms.Label();
            this.m_labelAdministrator = new System.Windows.Forms.Label();
            this.m_comboAdministrators = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // m_textPassword
            // 
            this.m_textPassword.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_textPassword.HideSelection = false;
            this.m_textPassword.Location = new System.Drawing.Point(117, 87);
            this.m_textPassword.Name = "m_textPassword";
            this.m_textPassword.Size = new System.Drawing.Size(188, 21);
            this.m_textPassword.TabIndex = 2;
            this.m_textPassword.UseSystemPasswordChar = true;
            // 
            // m_labelPassword
            // 
            this.m_labelPassword.Location = new System.Drawing.Point(12, 85);
            this.m_labelPassword.Name = "m_labelPassword";
            this.m_labelPassword.Size = new System.Drawing.Size(88, 23);
            this.m_labelPassword.TabIndex = 26;
            this.m_labelPassword.Text = "Password:";
            this.m_labelPassword.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // m_btnCancel
            // 
            this.m_btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.m_btnCancel.Location = new System.Drawing.Point(170, 124);
            this.m_btnCancel.Name = "m_btnCancel";
            this.m_btnCancel.Size = new System.Drawing.Size(75, 23);
            this.m_btnCancel.TabIndex = 4;
            this.m_btnCancel.Text = "Cancel";
            // 
            // m_btnOK
            // 
            this.m_btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.m_btnOK.Location = new System.Drawing.Point(82, 124);
            this.m_btnOK.Name = "m_btnOK";
            this.m_btnOK.Size = new System.Drawing.Size(75, 23);
            this.m_btnOK.TabIndex = 3;
            this.m_btnOK.Text = "OK";
            // 
            // m_labelAdministratorRequired
            // 
            this.m_labelAdministratorRequired.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.m_labelAdministratorRequired.Location = new System.Drawing.Point(12, 9);
            this.m_labelAdministratorRequired.Name = "m_labelAdministratorRequired";
            this.m_labelAdministratorRequired.Size = new System.Drawing.Size(293, 47);
            this.m_labelAdministratorRequired.TabIndex = 27;
            this.m_labelAdministratorRequired.Text = "You must log in as an Administrator to access the Configuration.";
            this.m_labelAdministratorRequired.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // m_labelAdministrator
            // 
            this.m_labelAdministrator.Location = new System.Drawing.Point(12, 57);
            this.m_labelAdministrator.Name = "m_labelAdministrator";
            this.m_labelAdministrator.Size = new System.Drawing.Size(88, 23);
            this.m_labelAdministrator.TabIndex = 28;
            this.m_labelAdministrator.Text = "Administrator:";
            this.m_labelAdministrator.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // m_comboAdministrators
            // 
            this.m_comboAdministrators.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.m_comboAdministrators.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.m_comboAdministrators.DropDownHeight = 200;
            this.m_comboAdministrators.FormattingEnabled = true;
            this.m_comboAdministrators.IntegralHeight = false;
            this.m_comboAdministrators.Location = new System.Drawing.Point(117, 59);
            this.m_comboAdministrators.MaxDropDownItems = 20;
            this.m_comboAdministrators.Name = "m_comboAdministrators";
            this.m_comboAdministrators.Size = new System.Drawing.Size(188, 21);
            this.m_comboAdministrators.Sorted = true;
            this.m_comboAdministrators.TabIndex = 1;
            this.m_comboAdministrators.TextUpdate += new System.EventHandler(this.cmdAdministratorTextChanged);
            // 
            // DlgAdministratorLogin
            // 
            this.AcceptButton = this.m_btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.m_btnCancel;
            this.ClientSize = new System.Drawing.Size(321, 159);
            this.Controls.Add(this.m_comboAdministrators);
            this.Controls.Add(this.m_labelAdministrator);
            this.Controls.Add(this.m_labelAdministratorRequired);
            this.Controls.Add(this.m_textPassword);
            this.Controls.Add(this.m_labelPassword);
            this.Controls.Add(this.m_btnCancel);
            this.Controls.Add(this.m_btnOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DlgAdministratorLogin";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Enter Administrator Password";
            this.Load += new System.EventHandler(this.cmdLoad);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.cmdFormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox m_textPassword;
        private System.Windows.Forms.Label m_labelPassword;
        private System.Windows.Forms.Button m_btnCancel;
        private System.Windows.Forms.Button m_btnOK;
        private System.Windows.Forms.Label m_labelAdministratorRequired;
        private System.Windows.Forms.Label m_labelAdministrator;
        private System.Windows.Forms.ComboBox m_comboAdministrators;
    }
}