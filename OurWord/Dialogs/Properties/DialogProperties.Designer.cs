namespace OurWord.Dialogs
{
    partial class DialogProperties
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DialogProperties));
            this.m_btnHelp = new System.Windows.Forms.Button();
            this.m_btnOK = new System.Windows.Forms.Button();
            this.m_TasksPanel = new System.Windows.Forms.Panel();
            this.m_btnNavTeamSettings = new System.Windows.Forms.Button();
            this.m_btnNavTranslations = new System.Windows.Forms.Button();
            this.m_btnNavOptions = new System.Windows.Forms.Button();
            this.m_btnNavEssentials = new System.Windows.Forms.Button();
            this.m_NavTitle = new System.Windows.Forms.Label();
            this.m_TabControl = new System.Windows.Forms.TabControl();
            this.m_TasksPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // m_btnHelp
            // 
            this.m_btnHelp.Image = ((System.Drawing.Image)(resources.GetObject("m_btnHelp.Image")));
            this.m_btnHelp.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.m_btnHelp.Location = new System.Drawing.Point(328, 447);
            this.m_btnHelp.Name = "m_btnHelp";
            this.m_btnHelp.Size = new System.Drawing.Size(75, 23);
            this.m_btnHelp.TabIndex = 20;
            this.m_btnHelp.Text = "Help...";
            this.m_btnHelp.Click += new System.EventHandler(this.cmdHelp);
            // 
            // m_btnOK
            // 
            this.m_btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.m_btnOK.Location = new System.Drawing.Point(247, 447);
            this.m_btnOK.Name = "m_btnOK";
            this.m_btnOK.Size = new System.Drawing.Size(75, 23);
            this.m_btnOK.TabIndex = 18;
            this.m_btnOK.Text = "OK";
            // 
            // m_TasksPanel
            // 
            this.m_TasksPanel.BackColor = System.Drawing.Color.LightSeaGreen;
            this.m_TasksPanel.Controls.Add(this.m_btnNavTeamSettings);
            this.m_TasksPanel.Controls.Add(this.m_btnNavTranslations);
            this.m_TasksPanel.Controls.Add(this.m_btnNavOptions);
            this.m_TasksPanel.Controls.Add(this.m_btnNavEssentials);
            this.m_TasksPanel.Location = new System.Drawing.Point(12, 12);
            this.m_TasksPanel.Name = "m_TasksPanel";
            this.m_TasksPanel.Size = new System.Drawing.Size(121, 422);
            this.m_TasksPanel.TabIndex = 21;
            // 
            // m_btnNavTeamSettings
            // 
            this.m_btnNavTeamSettings.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.m_btnNavTeamSettings.Image = ((System.Drawing.Image)(resources.GetObject("m_btnNavTeamSettings.Image")));
            this.m_btnNavTeamSettings.Location = new System.Drawing.Point(11, 313);
            this.m_btnNavTeamSettings.Name = "m_btnNavTeamSettings";
            this.m_btnNavTeamSettings.Size = new System.Drawing.Size(98, 97);
            this.m_btnNavTeamSettings.TabIndex = 25;
            this.m_btnNavTeamSettings.Text = "Team Settings";
            this.m_btnNavTeamSettings.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.m_btnNavTeamSettings.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.m_btnNavTeamSettings.UseVisualStyleBackColor = false;
            this.m_btnNavTeamSettings.Click += new System.EventHandler(this.cmdNavTeamSettings);
            // 
            // m_btnNavTranslations
            // 
            this.m_btnNavTranslations.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.m_btnNavTranslations.Image = ((System.Drawing.Image)(resources.GetObject("m_btnNavTranslations.Image")));
            this.m_btnNavTranslations.Location = new System.Drawing.Point(11, 210);
            this.m_btnNavTranslations.Name = "m_btnNavTranslations";
            this.m_btnNavTranslations.Size = new System.Drawing.Size(98, 97);
            this.m_btnNavTranslations.TabIndex = 24;
            this.m_btnNavTranslations.Text = "Reference";
            this.m_btnNavTranslations.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.m_btnNavTranslations.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.m_btnNavTranslations.UseVisualStyleBackColor = false;
            this.m_btnNavTranslations.Click += new System.EventHandler(this.cmdNavOtherTranslations);
            // 
            // m_btnNavOptions
            // 
            this.m_btnNavOptions.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.m_btnNavOptions.Image = ((System.Drawing.Image)(resources.GetObject("m_btnNavOptions.Image")));
            this.m_btnNavOptions.Location = new System.Drawing.Point(11, 107);
            this.m_btnNavOptions.Name = "m_btnNavOptions";
            this.m_btnNavOptions.Size = new System.Drawing.Size(98, 97);
            this.m_btnNavOptions.TabIndex = 23;
            this.m_btnNavOptions.Text = "Options";
            this.m_btnNavOptions.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.m_btnNavOptions.UseVisualStyleBackColor = false;
            this.m_btnNavOptions.Click += new System.EventHandler(this.cmdNavOptions);
            // 
            // m_btnNavEssentials
            // 
            this.m_btnNavEssentials.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.m_btnNavEssentials.Image = ((System.Drawing.Image)(resources.GetObject("m_btnNavEssentials.Image")));
            this.m_btnNavEssentials.Location = new System.Drawing.Point(11, 7);
            this.m_btnNavEssentials.Name = "m_btnNavEssentials";
            this.m_btnNavEssentials.Size = new System.Drawing.Size(98, 94);
            this.m_btnNavEssentials.TabIndex = 22;
            this.m_btnNavEssentials.Text = "Essentials";
            this.m_btnNavEssentials.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.m_btnNavEssentials.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.m_btnNavEssentials.UseVisualStyleBackColor = false;
            this.m_btnNavEssentials.Click += new System.EventHandler(this.cmdNavEssentials);
            // 
            // m_NavTitle
            // 
            this.m_NavTitle.BackColor = System.Drawing.Color.LightSeaGreen;
            this.m_NavTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_NavTitle.ForeColor = System.Drawing.Color.White;
            this.m_NavTitle.Location = new System.Drawing.Point(139, 12);
            this.m_NavTitle.Name = "m_NavTitle";
            this.m_NavTitle.Size = new System.Drawing.Size(476, 26);
            this.m_NavTitle.TabIndex = 22;
            this.m_NavTitle.Text = "Navigation Title";
            this.m_NavTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // m_TabControl
            // 
            this.m_TabControl.Location = new System.Drawing.Point(139, 41);
            this.m_TabControl.Name = "m_TabControl";
            this.m_TabControl.SelectedIndex = 0;
            this.m_TabControl.Size = new System.Drawing.Size(476, 393);
            this.m_TabControl.TabIndex = 23;
            // 
            // DialogProperties
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(627, 482);
            this.Controls.Add(this.m_TabControl);
            this.Controls.Add(this.m_NavTitle);
            this.Controls.Add(this.m_TasksPanel);
            this.Controls.Add(this.m_btnHelp);
            this.Controls.Add(this.m_btnOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DialogProperties";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Project Properties";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.cmdFormClosing);
            this.Load += new System.EventHandler(this.cmdLoad);
            this.m_TasksPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button m_btnHelp;
        private System.Windows.Forms.Button m_btnOK;
        private System.Windows.Forms.Panel m_TasksPanel;
        private System.Windows.Forms.Button m_btnNavEssentials;
        private System.Windows.Forms.Button m_btnNavOptions;
        private System.Windows.Forms.Button m_btnNavTranslations;
        private System.Windows.Forms.Button m_btnNavTeamSettings;
        private System.Windows.Forms.Label m_NavTitle;
        private System.Windows.Forms.TabControl m_TabControl;
    }
}