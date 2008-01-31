namespace OurWord.Dialogs.WizNewProject
{
    partial class WizNew_FrontInfo
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
            this.m_lblBrowseToExistingFront = new System.Windows.Forms.Label();
            this.m_group = new System.Windows.Forms.GroupBox();
            this.m_btnSettings = new System.Windows.Forms.Button();
            this.m_textFrontSettingsFolder = new System.Windows.Forms.Label();
            this.m_lblSettings = new System.Windows.Forms.Label();
            this.m_textFrontAbbreviation = new System.Windows.Forms.TextBox();
            this.m_lblAbbrev = new System.Windows.Forms.Label();
            this.m_textFrontName = new System.Windows.Forms.TextBox();
            this.m_lblName = new System.Windows.Forms.Label();
            this.m_btnUseExisting = new System.Windows.Forms.Button();
            this.m_lblInstructions = new System.Windows.Forms.Label();
            this.m_group.SuspendLayout();
            this.SuspendLayout();
            // 
            // m_lblBrowseToExistingFront
            // 
            this.m_lblBrowseToExistingFront.Location = new System.Drawing.Point(14, 10);
            this.m_lblBrowseToExistingFront.Name = "m_lblBrowseToExistingFront";
            this.m_lblBrowseToExistingFront.Size = new System.Drawing.Size(235, 35);
            this.m_lblBrowseToExistingFront.TabIndex = 0;
            this.m_lblBrowseToExistingFront.Text = "Browse to a Front settings file, if it already exists:";
            // 
            // m_group
            // 
            this.m_group.Controls.Add(this.m_btnSettings);
            this.m_group.Controls.Add(this.m_textFrontSettingsFolder);
            this.m_group.Controls.Add(this.m_lblSettings);
            this.m_group.Controls.Add(this.m_textFrontAbbreviation);
            this.m_group.Controls.Add(this.m_lblAbbrev);
            this.m_group.Controls.Add(this.m_textFrontName);
            this.m_group.Controls.Add(this.m_lblName);
            this.m_group.Location = new System.Drawing.Point(17, 102);
            this.m_group.Name = "m_group";
            this.m_group.Size = new System.Drawing.Size(337, 183);
            this.m_group.TabIndex = 1;
            this.m_group.TabStop = false;
            // 
            // m_btnSettings
            // 
            this.m_btnSettings.Location = new System.Drawing.Point(140, 154);
            this.m_btnSettings.Name = "m_btnSettings";
            this.m_btnSettings.Size = new System.Drawing.Size(75, 23);
            this.m_btnSettings.TabIndex = 34;
            this.m_btnSettings.Text = "Browse...";
            this.m_btnSettings.Click += new System.EventHandler(this.cmdBrowse);
            // 
            // m_textFrontSettingsFolder
            // 
            this.m_textFrontSettingsFolder.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.m_textFrontSettingsFolder.Location = new System.Drawing.Point(140, 106);
            this.m_textFrontSettingsFolder.Name = "m_textFrontSettingsFolder";
            this.m_textFrontSettingsFolder.Size = new System.Drawing.Size(191, 45);
            this.m_textFrontSettingsFolder.TabIndex = 33;
            // 
            // m_lblSettings
            // 
            this.m_lblSettings.Location = new System.Drawing.Point(6, 106);
            this.m_lblSettings.Name = "m_lblSettings";
            this.m_lblSettings.Size = new System.Drawing.Size(128, 23);
            this.m_lblSettings.TabIndex = 32;
            this.m_lblSettings.Text = "Front Settings Folder:";
            this.m_lblSettings.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // m_textFrontAbbreviation
            // 
            this.m_textFrontAbbreviation.Location = new System.Drawing.Point(140, 63);
            this.m_textFrontAbbreviation.Name = "m_textFrontAbbreviation";
            this.m_textFrontAbbreviation.Size = new System.Drawing.Size(89, 20);
            this.m_textFrontAbbreviation.TabIndex = 31;
            // 
            // m_lblAbbrev
            // 
            this.m_lblAbbrev.Location = new System.Drawing.Point(6, 61);
            this.m_lblAbbrev.Name = "m_lblAbbrev";
            this.m_lblAbbrev.Size = new System.Drawing.Size(128, 23);
            this.m_lblAbbrev.TabIndex = 30;
            this.m_lblAbbrev.Text = "Front Abbreviation:";
            this.m_lblAbbrev.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // m_textFrontName
            // 
            this.m_textFrontName.Location = new System.Drawing.Point(140, 18);
            this.m_textFrontName.Name = "m_textFrontName";
            this.m_textFrontName.Size = new System.Drawing.Size(191, 20);
            this.m_textFrontName.TabIndex = 29;
            this.m_textFrontName.TextChanged += new System.EventHandler(this.cmdLanguageNameChanged);
            // 
            // m_lblName
            // 
            this.m_lblName.Location = new System.Drawing.Point(6, 16);
            this.m_lblName.Name = "m_lblName";
            this.m_lblName.Size = new System.Drawing.Size(128, 23);
            this.m_lblName.TabIndex = 26;
            this.m_lblName.Text = "Front Language Name:";
            this.m_lblName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // m_btnUseExisting
            // 
            this.m_btnUseExisting.Location = new System.Drawing.Point(255, 10);
            this.m_btnUseExisting.Name = "m_btnUseExisting";
            this.m_btnUseExisting.Size = new System.Drawing.Size(99, 23);
            this.m_btnUseExisting.TabIndex = 2;
            this.m_btnUseExisting.Text = "Use Existing...";
            this.m_btnUseExisting.UseVisualStyleBackColor = true;
            this.m_btnUseExisting.Click += new System.EventHandler(this.cmdUseExisting);
            // 
            // m_lblInstructions
            // 
            this.m_lblInstructions.Location = new System.Drawing.Point(14, 68);
            this.m_lblInstructions.Name = "m_lblInstructions";
            this.m_lblInstructions.Size = new System.Drawing.Size(340, 31);
            this.m_lblInstructions.TabIndex = 3;
            this.m_lblInstructions.Text = "Otherwise, enter the following information for the Front language, in the same ma" +
                "nner as you did earler for the Target language:";
            // 
            // WizNew_FrontInfo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.m_lblInstructions);
            this.Controls.Add(this.m_btnUseExisting);
            this.Controls.Add(this.m_group);
            this.Controls.Add(this.m_lblBrowseToExistingFront);
            this.Name = "WizNew_FrontInfo";
            this.Size = new System.Drawing.Size(372, 306);
            this.m_group.ResumeLayout(false);
            this.m_group.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label m_lblBrowseToExistingFront;
        private System.Windows.Forms.GroupBox m_group;
        private System.Windows.Forms.Button m_btnUseExisting;
        private System.Windows.Forms.Label m_lblInstructions;
        private System.Windows.Forms.Label m_lblName;
        private System.Windows.Forms.TextBox m_textFrontName;
        private System.Windows.Forms.Label m_lblAbbrev;
        private System.Windows.Forms.TextBox m_textFrontAbbreviation;
        private System.Windows.Forms.Label m_lblSettings;
        private System.Windows.Forms.Label m_textFrontSettingsFolder;
        private System.Windows.Forms.Button m_btnSettings;
    }
}
