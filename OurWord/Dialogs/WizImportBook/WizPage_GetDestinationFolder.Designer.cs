namespace OurWord.Dialogs.WizImportBook
{
    partial class WizPage_GetDestinationFolder
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
            this.m_labelFileNameText = new System.Windows.Forms.Label();
            this.m_labelFolderText = new System.Windows.Forms.Label();
            this.m_labelFileName = new System.Windows.Forms.Label();
            this.m_btnBrowse = new System.Windows.Forms.Button();
            this.m_labelFolder = new System.Windows.Forms.Label();
            this.m_labelNote = new System.Windows.Forms.Label();
            this.m_labelWarning = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // m_labelFileNameText
            // 
            this.m_labelFileNameText.Location = new System.Drawing.Point(12, 11);
            this.m_labelFileNameText.Name = "m_labelFileNameText";
            this.m_labelFileNameText.Size = new System.Drawing.Size(364, 23);
            this.m_labelFileNameText.TabIndex = 0;
            this.m_labelFileNameText.Text = "OurWord will store the book under this file name:";
            this.m_labelFileNameText.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // m_labelFolderText
            // 
            this.m_labelFolderText.Location = new System.Drawing.Point(12, 85);
            this.m_labelFolderText.Name = "m_labelFolderText";
            this.m_labelFolderText.Size = new System.Drawing.Size(364, 23);
            this.m_labelFolderText.TabIndex = 1;
            this.m_labelFolderText.Text = "Please indicate which Folder you would like this file stored under:";
            this.m_labelFolderText.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // m_labelFileName
            // 
            this.m_labelFileName.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.m_labelFileName.Location = new System.Drawing.Point(29, 34);
            this.m_labelFileName.Name = "m_labelFileName";
            this.m_labelFileName.Size = new System.Drawing.Size(347, 23);
            this.m_labelFileName.TabIndex = 3;
            this.m_labelFileName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // m_btnBrowse
            // 
            this.m_btnBrowse.Location = new System.Drawing.Point(301, 108);
            this.m_btnBrowse.Name = "m_btnBrowse";
            this.m_btnBrowse.Size = new System.Drawing.Size(75, 23);
            this.m_btnBrowse.TabIndex = 5;
            this.m_btnBrowse.Text = "Browse...";
            this.m_btnBrowse.UseVisualStyleBackColor = true;
            this.m_btnBrowse.Click += new System.EventHandler(this.cmdBrowseFolder);
            // 
            // m_labelFolder
            // 
            this.m_labelFolder.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.m_labelFolder.Location = new System.Drawing.Point(29, 108);
            this.m_labelFolder.Name = "m_labelFolder";
            this.m_labelFolder.Size = new System.Drawing.Size(266, 23);
            this.m_labelFolder.TabIndex = 4;
            this.m_labelFolder.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // m_labelNote
            // 
            this.m_labelNote.Location = new System.Drawing.Point(12, 140);
            this.m_labelNote.Name = "m_labelNote";
            this.m_labelNote.Size = new System.Drawing.Size(364, 52);
            this.m_labelNote.TabIndex = 6;
            this.m_labelNote.Text = "(OurWord will default to using the same folder that the Import file is in; but yo" +
                "u can change it here.)";
            // 
            // m_labelWarning
            // 
            this.m_labelWarning.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_labelWarning.ForeColor = System.Drawing.Color.Red;
            this.m_labelWarning.Location = new System.Drawing.Point(12, 183);
            this.m_labelWarning.Name = "m_labelWarning";
            this.m_labelWarning.Size = new System.Drawing.Size(364, 98);
            this.m_labelWarning.TabIndex = 7;
            this.m_labelWarning.Text = "(warning)";
            this.m_labelWarning.Visible = false;
            // 
            // WizPage_GetDestinationFolder
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.m_labelWarning);
            this.Controls.Add(this.m_labelNote);
            this.Controls.Add(this.m_btnBrowse);
            this.Controls.Add(this.m_labelFolder);
            this.Controls.Add(this.m_labelFileName);
            this.Controls.Add(this.m_labelFolderText);
            this.Controls.Add(this.m_labelFileNameText);
            this.Name = "WizPage_GetDestinationFolder";
            this.Size = new System.Drawing.Size(392, 305);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label m_labelFileNameText;
        private System.Windows.Forms.Label m_labelFolderText;
        private System.Windows.Forms.Label m_labelFileName;
        private System.Windows.Forms.Button m_btnBrowse;
        private System.Windows.Forms.Label m_labelFolder;
        private System.Windows.Forms.Label m_labelNote;
        private System.Windows.Forms.Label m_labelWarning;
    }
}
