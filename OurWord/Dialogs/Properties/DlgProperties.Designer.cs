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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DialogProperties));
            this.m_btnOK = new System.Windows.Forms.Button();
            this.m_Images = new System.Windows.Forms.ImageList(this.components);
            this.m_btnHelp = new System.Windows.Forms.Button();
            this.PageContent = new OurWord.Dialogs.Properties.CtrlPageContent();
            this.m_NavTasks = new OurWord.Utilities.GroupedTasksList();
            this.SuspendLayout();
            // 
            // m_btnOK
            // 
            this.m_btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.m_btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.m_btnOK.Location = new System.Drawing.Point(12, 406);
            this.m_btnOK.Name = "m_btnOK";
            this.m_btnOK.Size = new System.Drawing.Size(75, 23);
            this.m_btnOK.TabIndex = 10;
            this.m_btnOK.Text = "OK";
            // 
            // m_Images
            // 
            this.m_Images.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("m_Images.ImageStream")));
            this.m_Images.TransparentColor = System.Drawing.Color.Transparent;
            this.m_Images.Images.SetKeyName(0, "Blank.ico");
            this.m_Images.Images.SetKeyName(1, "Print.ico");
            this.m_Images.Images.SetKeyName(2, "Notes.ico");
            this.m_Images.Images.SetKeyName(3, "Options.ico");
            this.m_Images.Images.SetKeyName(4, "Note_Hebrew.ico");
            this.m_Images.Images.SetKeyName(5, "Note_Greek.ico");
            this.m_Images.Images.SetKeyName(6, "Italic.ico");
            this.m_Images.Images.SetKeyName(7, "Note_Hint.ico");
            this.m_Images.Images.SetKeyName(8, "TurnFeaturesOnOff.ico");
            this.m_Images.Images.SetKeyName(9, "UserEditPermissions.ico");
            // 
            // m_btnHelp
            // 
            this.m_btnHelp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.m_btnHelp.Image = ((System.Drawing.Image)(resources.GetObject("m_btnHelp.Image")));
            this.m_btnHelp.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.m_btnHelp.Location = new System.Drawing.Point(93, 406);
            this.m_btnHelp.Name = "m_btnHelp";
            this.m_btnHelp.Size = new System.Drawing.Size(75, 23);
            this.m_btnHelp.TabIndex = 11;
            this.m_btnHelp.Text = "Help...";
            this.m_btnHelp.Click += new System.EventHandler(this.cmdHelp);
            // 
            // PageContent
            // 
            this.PageContent.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.PageContent.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.PageContent.Location = new System.Drawing.Point(183, 9);
            this.PageContent.Name = "PageContent";
            this.PageContent.Size = new System.Drawing.Size(489, 420);
            this.PageContent.TabIndex = 26;
            // 
            // m_NavTasks
            // 
            this.m_NavTasks.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.m_NavTasks.Images = null;
            this.m_NavTasks.LastSelectedButton = null;
            this.m_NavTasks.Location = new System.Drawing.Point(12, 9);
            this.m_NavTasks.Name = "m_NavTasks";
            this.m_NavTasks.OnItemSelected = null;
            this.m_NavTasks.Size = new System.Drawing.Size(154, 391);
            this.m_NavTasks.TabIndex = 25;
            // 
            // DialogProperties
            // 
            this.AcceptButton = this.m_btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(684, 438);
            this.Controls.Add(this.PageContent);
            this.Controls.Add(this.m_NavTasks);
            this.Controls.Add(this.m_btnHelp);
            this.Controls.Add(this.m_btnOK);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DialogProperties";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Project Properties";
            this.Load += new System.EventHandler(this.cmdLoad);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.cmdFormClosing);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button m_btnHelp;
        private System.Windows.Forms.Button m_btnOK;
        private OurWord.Utilities.GroupedTasksList m_NavTasks;
        private System.Windows.Forms.ImageList m_Images;
    }
}