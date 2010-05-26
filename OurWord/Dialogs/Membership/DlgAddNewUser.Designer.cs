namespace OurWord.Dialogs.Membership
{
    partial class DlgAddNewUser
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DlgAddNewUser));
            this.m_btnCancel = new System.Windows.Forms.Button();
            this.m_btnOK = new System.Windows.Forms.Button();
            this.m_labelFullName = new System.Windows.Forms.Label();
            this.m_textFullName = new System.Windows.Forms.TextBox();
            this.m_textPassword = new System.Windows.Forms.TextBox();
            this.m_labelPassword = new System.Windows.Forms.Label();
            this.m_radioAdministrator = new System.Windows.Forms.RadioButton();
            this.m_radioTranslator = new System.Windows.Forms.RadioButton();
            this.m_radioConsultant = new System.Windows.Forms.RadioButton();
            this.m_radioObserver = new System.Windows.Forms.RadioButton();
            this.m_groupInitializeSettingsAs = new System.Windows.Forms.GroupBox();
            this.m_groupInitializeSettingsAs.SuspendLayout();
            this.SuspendLayout();
            // 
            // m_btnCancel
            // 
            this.m_btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.m_btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.m_btnCancel.Location = new System.Drawing.Point(163, 176);
            this.m_btnCancel.Name = "m_btnCancel";
            this.m_btnCancel.Size = new System.Drawing.Size(75, 23);
            this.m_btnCancel.TabIndex = 8;
            this.m_btnCancel.Text = "Cancel";
            // 
            // m_btnOK
            // 
            this.m_btnOK.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.m_btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.m_btnOK.Location = new System.Drawing.Point(75, 176);
            this.m_btnOK.Name = "m_btnOK";
            this.m_btnOK.Size = new System.Drawing.Size(75, 23);
            this.m_btnOK.TabIndex = 7;
            this.m_btnOK.Text = "Add";
            // 
            // m_labelFullName
            // 
            this.m_labelFullName.Location = new System.Drawing.Point(12, 9);
            this.m_labelFullName.Name = "m_labelFullName";
            this.m_labelFullName.Size = new System.Drawing.Size(87, 23);
            this.m_labelFullName.TabIndex = 20;
            this.m_labelFullName.Text = "Full Name:";
            this.m_labelFullName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // m_textFullName
            // 
            this.m_textFullName.Location = new System.Drawing.Point(105, 11);
            this.m_textFullName.Name = "m_textFullName";
            this.m_textFullName.Size = new System.Drawing.Size(178, 20);
            this.m_textFullName.TabIndex = 1;
            // 
            // m_textPassword
            // 
            this.m_textPassword.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_textPassword.Location = new System.Drawing.Point(105, 44);
            this.m_textPassword.Name = "m_textPassword";
            this.m_textPassword.Size = new System.Drawing.Size(178, 20);
            this.m_textPassword.TabIndex = 2;
            // 
            // m_labelPassword
            // 
            this.m_labelPassword.Location = new System.Drawing.Point(12, 42);
            this.m_labelPassword.Name = "m_labelPassword";
            this.m_labelPassword.Size = new System.Drawing.Size(87, 23);
            this.m_labelPassword.TabIndex = 22;
            this.m_labelPassword.Text = "Password:";
            this.m_labelPassword.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // m_radioAdministrator
            // 
            this.m_radioAdministrator.AutoSize = true;
            this.m_radioAdministrator.Location = new System.Drawing.Point(19, 18);
            this.m_radioAdministrator.Name = "m_radioAdministrator";
            this.m_radioAdministrator.Size = new System.Drawing.Size(85, 17);
            this.m_radioAdministrator.TabIndex = 3;
            this.m_radioAdministrator.TabStop = true;
            this.m_radioAdministrator.Text = "Administrator";
            this.m_radioAdministrator.UseVisualStyleBackColor = true;
            // 
            // m_radioTranslator
            // 
            this.m_radioTranslator.AutoSize = true;
            this.m_radioTranslator.Location = new System.Drawing.Point(19, 41);
            this.m_radioTranslator.Name = "m_radioTranslator";
            this.m_radioTranslator.Size = new System.Drawing.Size(72, 17);
            this.m_radioTranslator.TabIndex = 5;
            this.m_radioTranslator.TabStop = true;
            this.m_radioTranslator.Text = "Translator";
            this.m_radioTranslator.UseVisualStyleBackColor = true;
            // 
            // m_radioConsultant
            // 
            this.m_radioConsultant.AutoSize = true;
            this.m_radioConsultant.Location = new System.Drawing.Point(133, 18);
            this.m_radioConsultant.Name = "m_radioConsultant";
            this.m_radioConsultant.Size = new System.Drawing.Size(75, 17);
            this.m_radioConsultant.TabIndex = 4;
            this.m_radioConsultant.TabStop = true;
            this.m_radioConsultant.Text = "Consultant";
            this.m_radioConsultant.UseVisualStyleBackColor = true;
            // 
            // m_radioObserver
            // 
            this.m_radioObserver.AutoSize = true;
            this.m_radioObserver.Location = new System.Drawing.Point(133, 41);
            this.m_radioObserver.Name = "m_radioObserver";
            this.m_radioObserver.Size = new System.Drawing.Size(68, 17);
            this.m_radioObserver.TabIndex = 6;
            this.m_radioObserver.TabStop = true;
            this.m_radioObserver.Text = "Observer";
            this.m_radioObserver.UseVisualStyleBackColor = true;
            // 
            // m_groupInitializeSettingsAs
            // 
            this.m_groupInitializeSettingsAs.Controls.Add(this.m_radioObserver);
            this.m_groupInitializeSettingsAs.Controls.Add(this.m_radioAdministrator);
            this.m_groupInitializeSettingsAs.Controls.Add(this.m_radioConsultant);
            this.m_groupInitializeSettingsAs.Controls.Add(this.m_radioTranslator);
            this.m_groupInitializeSettingsAs.Location = new System.Drawing.Point(15, 86);
            this.m_groupInitializeSettingsAs.Name = "m_groupInitializeSettingsAs";
            this.m_groupInitializeSettingsAs.Size = new System.Drawing.Size(268, 69);
            this.m_groupInitializeSettingsAs.TabIndex = 32;
            this.m_groupInitializeSettingsAs.TabStop = false;
            this.m_groupInitializeSettingsAs.Text = "Initialize Settings As:";
            // 
            // DlgAddNewUser
            // 
            this.AcceptButton = this.m_btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.m_btnCancel;
            this.ClientSize = new System.Drawing.Size(304, 204);
            this.Controls.Add(this.m_textPassword);
            this.Controls.Add(this.m_labelPassword);
            this.Controls.Add(this.m_textFullName);
            this.Controls.Add(this.m_labelFullName);
            this.Controls.Add(this.m_btnCancel);
            this.Controls.Add(this.m_btnOK);
            this.Controls.Add(this.m_groupInitializeSettingsAs);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DlgAddNewUser";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Add New User";
            this.Load += new System.EventHandler(this.cmdLoad);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.cmdClosing);
            this.m_groupInitializeSettingsAs.ResumeLayout(false);
            this.m_groupInitializeSettingsAs.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button m_btnCancel;
        private System.Windows.Forms.Button m_btnOK;
        private System.Windows.Forms.Label m_labelFullName;
        private System.Windows.Forms.TextBox m_textFullName;
        private System.Windows.Forms.TextBox m_textPassword;
        private System.Windows.Forms.Label m_labelPassword;
        private System.Windows.Forms.RadioButton m_radioAdministrator;
        private System.Windows.Forms.RadioButton m_radioTranslator;
        private System.Windows.Forms.RadioButton m_radioConsultant;
        private System.Windows.Forms.RadioButton m_radioObserver;
        private System.Windows.Forms.GroupBox m_groupInitializeSettingsAs;
    }
}