namespace OurWord.Dialogs
{
    partial class Page_TeamSettings
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Page_TeamSettings));
            this.m_btnOpenExisting = new System.Windows.Forms.Button();
            this.m_textTeamSettingsFile = new System.Windows.Forms.Label();
            this.m_lblTeamSettingsFile = new System.Windows.Forms.Label();
            this.m_labelInstructions = new System.Windows.Forms.Label();
            this.m_lblActions = new System.Windows.Forms.Label();
            this.m_lblOpenExisting = new System.Windows.Forms.Label();
            this.m_btnCreateNew = new System.Windows.Forms.Button();
            this.m_lblCreateNew = new System.Windows.Forms.Label();
            this.m_groupTeamSettingsFile = new System.Windows.Forms.GroupBox();
            this.SuspendLayout();
            // 
            // m_btnOpenExisting
            // 
            this.m_btnOpenExisting.Location = new System.Drawing.Point(32, 181);
            this.m_btnOpenExisting.Name = "m_btnOpenExisting";
            this.m_btnOpenExisting.Size = new System.Drawing.Size(111, 23);
            this.m_btnOpenExisting.TabIndex = 34;
            this.m_btnOpenExisting.Text = "Open Existing...";
            this.m_btnOpenExisting.Click += new System.EventHandler(this.cmdOpenExisting);
            // 
            // m_textTeamSettingsFile
            // 
            this.m_textTeamSettingsFile.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.m_textTeamSettingsFile.Location = new System.Drawing.Point(143, 86);
            this.m_textTeamSettingsFile.Name = "m_textTeamSettingsFile";
            this.m_textTeamSettingsFile.Size = new System.Drawing.Size(299, 23);
            this.m_textTeamSettingsFile.TabIndex = 33;
            this.m_textTeamSettingsFile.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // m_lblTeamSettingsFile
            // 
            this.m_lblTeamSettingsFile.Location = new System.Drawing.Point(29, 86);
            this.m_lblTeamSettingsFile.Name = "m_lblTeamSettingsFile";
            this.m_lblTeamSettingsFile.Size = new System.Drawing.Size(108, 23);
            this.m_lblTeamSettingsFile.TabIndex = 32;
            this.m_lblTeamSettingsFile.Text = "Team Settings File:";
            this.m_lblTeamSettingsFile.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // m_labelInstructions
            // 
            this.m_labelInstructions.Location = new System.Drawing.Point(29, 36);
            this.m_labelInstructions.Name = "m_labelInstructions";
            this.m_labelInstructions.Size = new System.Drawing.Size(413, 50);
            this.m_labelInstructions.TabIndex = 35;
            this.m_labelInstructions.Text = resources.GetString("m_labelInstructions.Text");
            // 
            // m_lblActions
            // 
            this.m_lblActions.Location = new System.Drawing.Point(29, 121);
            this.m_lblActions.Name = "m_lblActions";
            this.m_lblActions.Size = new System.Drawing.Size(413, 23);
            this.m_lblActions.TabIndex = 36;
            this.m_lblActions.Text = "Available Actions:";
            this.m_lblActions.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // m_lblOpenExisting
            // 
            this.m_lblOpenExisting.Location = new System.Drawing.Point(152, 181);
            this.m_lblOpenExisting.Name = "m_lblOpenExisting";
            this.m_lblOpenExisting.Size = new System.Drawing.Size(290, 32);
            this.m_lblOpenExisting.TabIndex = 37;
            this.m_lblOpenExisting.Text = "Open an already-existing team settings file (that is, use existing team settings " +
                "that have already been created.)";
            // 
            // m_btnCreateNew
            // 
            this.m_btnCreateNew.Location = new System.Drawing.Point(32, 147);
            this.m_btnCreateNew.Name = "m_btnCreateNew";
            this.m_btnCreateNew.Size = new System.Drawing.Size(111, 23);
            this.m_btnCreateNew.TabIndex = 8;
            this.m_btnCreateNew.Text = "Create New...";
            this.m_btnCreateNew.Click += new System.EventHandler(this.cmdCreateNew);
            // 
            // m_lblCreateNew
            // 
            this.m_lblCreateNew.Location = new System.Drawing.Point(149, 147);
            this.m_lblCreateNew.Name = "m_lblCreateNew";
            this.m_lblCreateNew.Size = new System.Drawing.Size(293, 34);
            this.m_lblCreateNew.TabIndex = 38;
            this.m_lblCreateNew.Text = "Provide the filename under which you wish to store this project\'s team settings.";
            // 
            // m_groupTeamSettingsFile
            // 
            this.m_groupTeamSettingsFile.Location = new System.Drawing.Point(15, 14);
            this.m_groupTeamSettingsFile.Name = "m_groupTeamSettingsFile";
            this.m_groupTeamSettingsFile.Size = new System.Drawing.Size(440, 213);
            this.m_groupTeamSettingsFile.TabIndex = 39;
            this.m_groupTeamSettingsFile.TabStop = false;
            this.m_groupTeamSettingsFile.Text = "Team Settings File";
            // 
            // Page_TeamSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.m_lblCreateNew);
            this.Controls.Add(this.m_btnCreateNew);
            this.Controls.Add(this.m_lblOpenExisting);
            this.Controls.Add(this.m_lblActions);
            this.Controls.Add(this.m_labelInstructions);
            this.Controls.Add(this.m_btnOpenExisting);
            this.Controls.Add(this.m_textTeamSettingsFile);
            this.Controls.Add(this.m_lblTeamSettingsFile);
            this.Controls.Add(this.m_groupTeamSettingsFile);
            this.Name = "Page_TeamSettings";
            this.Size = new System.Drawing.Size(468, 368);
            this.Load += new System.EventHandler(this.cmdLoad);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button m_btnOpenExisting;
        private System.Windows.Forms.Label m_textTeamSettingsFile;
        private System.Windows.Forms.Label m_lblTeamSettingsFile;
        private System.Windows.Forms.Label m_labelInstructions;
        private System.Windows.Forms.Label m_lblActions;
        protected System.Windows.Forms.Label m_lblOpenExisting;
        private System.Windows.Forms.Button m_btnCreateNew;
        protected System.Windows.Forms.Label m_lblCreateNew;
        private System.Windows.Forms.GroupBox m_groupTeamSettingsFile;
    }
}
