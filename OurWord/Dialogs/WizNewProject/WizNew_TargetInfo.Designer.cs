namespace OurWord.Dialogs.WizNewProject
{
    partial class WizNew_TargetInfo
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
            this.m_lblDescription = new System.Windows.Forms.Label();
            this.m_lblAbbrev = new System.Windows.Forms.Label();
            this.m_textAbbrev = new System.Windows.Forms.TextBox();
            this.m_lblAbbrevHelp = new System.Windows.Forms.Label();
            this.m_lblSettingsHelp = new System.Windows.Forms.Label();
            this.m_lblSettings = new System.Windows.Forms.Label();
            this.m_textSettingsFolder = new System.Windows.Forms.Label();
            this.m_btnSettings = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // m_lblDescription
            // 
            this.m_lblDescription.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_lblDescription.Location = new System.Drawing.Point(13, 10);
            this.m_lblDescription.Name = "m_lblDescription";
            this.m_lblDescription.Size = new System.Drawing.Size(346, 33);
            this.m_lblDescription.TabIndex = 0;
            this.m_lblDescription.Text = "We need some additional information about the \'{0}\' language.";
            // 
            // m_lblAbbrev
            // 
            this.m_lblAbbrev.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_lblAbbrev.Location = new System.Drawing.Point(13, 95);
            this.m_lblAbbrev.Name = "m_lblAbbrev";
            this.m_lblAbbrev.Size = new System.Drawing.Size(108, 23);
            this.m_lblAbbrev.TabIndex = 27;
            this.m_lblAbbrev.Text = "Abbreviation:";
            this.m_lblAbbrev.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // m_textAbbrev
            // 
            this.m_textAbbrev.Location = new System.Drawing.Point(127, 95);
            this.m_textAbbrev.Name = "m_textAbbrev";
            this.m_textAbbrev.Size = new System.Drawing.Size(155, 20);
            this.m_textAbbrev.TabIndex = 30;
            this.m_textAbbrev.TextChanged += new System.EventHandler(this.cmdAbbreviationChanged);
            // 
            // m_lblAbbrevHelp
            // 
            this.m_lblAbbrevHelp.Location = new System.Drawing.Point(13, 55);
            this.m_lblAbbrevHelp.Name = "m_lblAbbrevHelp";
            this.m_lblAbbrevHelp.Size = new System.Drawing.Size(346, 40);
            this.m_lblAbbrevHelp.TabIndex = 34;
            this.m_lblAbbrevHelp.Text = "1. A short abbreviation of rhe language name. The Ethnologue offers suitable 3-le" +
                "tter abbreviations for most of the world\'s languages.";
            this.m_lblAbbrevHelp.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // m_lblSettingsHelp
            // 
            this.m_lblSettingsHelp.Location = new System.Drawing.Point(13, 139);
            this.m_lblSettingsHelp.Name = "m_lblSettingsHelp";
            this.m_lblSettingsHelp.Size = new System.Drawing.Size(346, 43);
            this.m_lblSettingsHelp.TabIndex = 35;
            this.m_lblSettingsHelp.Text = "2. Navigate to the folder where you want to store the settings files for this lan" +
                "guage.";
            this.m_lblSettingsHelp.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // m_lblSettings
            // 
            this.m_lblSettings.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_lblSettings.Location = new System.Drawing.Point(13, 182);
            this.m_lblSettings.Name = "m_lblSettings";
            this.m_lblSettings.Size = new System.Drawing.Size(108, 23);
            this.m_lblSettings.TabIndex = 36;
            this.m_lblSettings.Text = "Settings Folder:";
            this.m_lblSettings.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // m_textSettingsFolder
            // 
            this.m_textSettingsFolder.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.m_textSettingsFolder.Location = new System.Drawing.Point(127, 182);
            this.m_textSettingsFolder.Name = "m_textSettingsFolder";
            this.m_textSettingsFolder.Size = new System.Drawing.Size(232, 40);
            this.m_textSettingsFolder.TabIndex = 37;
            // 
            // m_btnSettings
            // 
            this.m_btnSettings.Location = new System.Drawing.Point(127, 225);
            this.m_btnSettings.Name = "m_btnSettings";
            this.m_btnSettings.Size = new System.Drawing.Size(75, 23);
            this.m_btnSettings.TabIndex = 38;
            this.m_btnSettings.Text = "Browse...";
            this.m_btnSettings.Click += new System.EventHandler(this.cmdBrowse);
            // 
            // WizNew_TargetInfo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.m_btnSettings);
            this.Controls.Add(this.m_textSettingsFolder);
            this.Controls.Add(this.m_lblSettings);
            this.Controls.Add(this.m_lblSettingsHelp);
            this.Controls.Add(this.m_lblAbbrevHelp);
            this.Controls.Add(this.m_textAbbrev);
            this.Controls.Add(this.m_lblAbbrev);
            this.Controls.Add(this.m_lblDescription);
            this.Name = "WizNew_TargetInfo";
            this.Size = new System.Drawing.Size(372, 306);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label m_lblDescription;
        private System.Windows.Forms.Label m_lblAbbrev;
        private System.Windows.Forms.TextBox m_textAbbrev;
        private System.Windows.Forms.Label m_lblAbbrevHelp;
        private System.Windows.Forms.Label m_lblSettingsHelp;
        private System.Windows.Forms.Label m_lblSettings;
        private System.Windows.Forms.Label m_textSettingsFolder;
        private System.Windows.Forms.Button m_btnSettings;
    }
}
