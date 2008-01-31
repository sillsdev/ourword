namespace OurWord.Dialogs.WizNewProject
{
    partial class WizNew_TeamSettings
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
            this.m_lblExplanation = new System.Windows.Forms.Label();
            this.m_btnUseExisting = new System.Windows.Forms.Button();
            this.m_btnSaveAs = new System.Windows.Forms.Button();
            this.m_btnDefault = new System.Windows.Forms.Button();
            this.m_lblExplUseExisting = new System.Windows.Forms.Label();
            this.m_lblExplSaveAs = new System.Windows.Forms.Label();
            this.m_lblExplDefault = new System.Windows.Forms.Label();
            this.m_textCurrentAction = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // m_lblExplanation
            // 
            this.m_lblExplanation.Location = new System.Drawing.Point(13, 10);
            this.m_lblExplanation.Name = "m_lblExplanation";
            this.m_lblExplanation.Size = new System.Drawing.Size(344, 39);
            this.m_lblExplanation.TabIndex = 0;
            this.m_lblExplanation.Text = "OurWord keeps certain settings, such as the StyleSheet, in a global settings file" +
                " where they can be used by multiple languagae projects.";
            // 
            // m_btnUseExisting
            // 
            this.m_btnUseExisting.Location = new System.Drawing.Point(16, 111);
            this.m_btnUseExisting.Name = "m_btnUseExisting";
            this.m_btnUseExisting.Size = new System.Drawing.Size(113, 23);
            this.m_btnUseExisting.TabIndex = 1;
            this.m_btnUseExisting.Text = "Use Existing...";
            this.m_btnUseExisting.UseVisualStyleBackColor = true;
            this.m_btnUseExisting.Click += new System.EventHandler(this.cmdUseExisting);
            // 
            // m_btnSaveAs
            // 
            this.m_btnSaveAs.Location = new System.Drawing.Point(16, 161);
            this.m_btnSaveAs.Name = "m_btnSaveAs";
            this.m_btnSaveAs.Size = new System.Drawing.Size(113, 23);
            this.m_btnSaveAs.TabIndex = 2;
            this.m_btnSaveAs.Text = "Save As...";
            this.m_btnSaveAs.UseVisualStyleBackColor = true;
            this.m_btnSaveAs.Click += new System.EventHandler(this.cmdSaveAs);
            // 
            // m_btnDefault
            // 
            this.m_btnDefault.Location = new System.Drawing.Point(16, 210);
            this.m_btnDefault.Name = "m_btnDefault";
            this.m_btnDefault.Size = new System.Drawing.Size(113, 23);
            this.m_btnDefault.TabIndex = 3;
            this.m_btnDefault.Text = "Default";
            this.m_btnDefault.UseVisualStyleBackColor = true;
            this.m_btnDefault.Click += new System.EventHandler(this.cmdDefault);
            // 
            // m_lblExplUseExisting
            // 
            this.m_lblExplUseExisting.Location = new System.Drawing.Point(135, 111);
            this.m_lblExplUseExisting.Name = "m_lblExplUseExisting";
            this.m_lblExplUseExisting.Size = new System.Drawing.Size(222, 38);
            this.m_lblExplUseExisting.TabIndex = 4;
            this.m_lblExplUseExisting.Text = "You may choose a Team Settings file that has already been created.";
            // 
            // m_lblExplSaveAs
            // 
            this.m_lblExplSaveAs.Location = new System.Drawing.Point(135, 161);
            this.m_lblExplSaveAs.Name = "m_lblExplSaveAs";
            this.m_lblExplSaveAs.Size = new System.Drawing.Size(222, 38);
            this.m_lblExplSaveAs.TabIndex = 5;
            this.m_lblExplSaveAs.Text = "You may specify a folder and file name for a new Team Settings file.";
            // 
            // m_lblExplDefault
            // 
            this.m_lblExplDefault.Location = new System.Drawing.Point(135, 210);
            this.m_lblExplDefault.Name = "m_lblExplDefault";
            this.m_lblExplDefault.Size = new System.Drawing.Size(222, 50);
            this.m_lblExplDefault.TabIndex = 6;
            this.m_lblExplDefault.Text = "The default action is that OurWord will create TeamSettings.owt in the settings f" +
                "older that you identified earlier.";
            // 
            // m_textCurrentAction
            // 
            this.m_textCurrentAction.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.m_textCurrentAction.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_textCurrentAction.Location = new System.Drawing.Point(16, 49);
            this.m_textCurrentAction.Name = "m_textCurrentAction";
            this.m_textCurrentAction.Size = new System.Drawing.Size(341, 45);
            this.m_textCurrentAction.TabIndex = 34;
            this.m_textCurrentAction.Text = "Current Action: Default";
            // 
            // WizNew_TeamSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.m_textCurrentAction);
            this.Controls.Add(this.m_lblExplDefault);
            this.Controls.Add(this.m_lblExplSaveAs);
            this.Controls.Add(this.m_lblExplUseExisting);
            this.Controls.Add(this.m_btnDefault);
            this.Controls.Add(this.m_btnSaveAs);
            this.Controls.Add(this.m_btnUseExisting);
            this.Controls.Add(this.m_lblExplanation);
            this.Name = "WizNew_TeamSettings";
            this.Size = new System.Drawing.Size(372, 306);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label m_lblExplanation;
        private System.Windows.Forms.Button m_btnUseExisting;
        private System.Windows.Forms.Button m_btnSaveAs;
        private System.Windows.Forms.Button m_btnDefault;
        private System.Windows.Forms.Label m_lblExplUseExisting;
        private System.Windows.Forms.Label m_lblExplSaveAs;
        private System.Windows.Forms.Label m_lblExplDefault;
        private System.Windows.Forms.Label m_textCurrentAction;
    }
}
