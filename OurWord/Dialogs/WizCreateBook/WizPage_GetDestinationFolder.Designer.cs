namespace OurWord.Dialogs.WizCreateBook
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
            this.m_labelFolder = new System.Windows.Forms.Label();
            this.m_labelFolderName = new System.Windows.Forms.Label();
            this.m_btnBrowseFolder = new System.Windows.Forms.Button();
            this.m_labelYouHaveChosen = new System.Windows.Forms.Label();
            this.m_labelBook = new System.Windows.Forms.Label();
            this.m_labelPressFinish = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // m_labelFolder
            // 
            this.m_labelFolder.Location = new System.Drawing.Point(13, 70);
            this.m_labelFolder.Name = "m_labelFolder";
            this.m_labelFolder.Size = new System.Drawing.Size(349, 23);
            this.m_labelFolder.TabIndex = 27;
            this.m_labelFolder.Text = "In what Folder do you want to store this file?";
            this.m_labelFolder.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // m_labelFolderName
            // 
            this.m_labelFolderName.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.m_labelFolderName.Location = new System.Drawing.Point(15, 93);
            this.m_labelFolderName.Name = "m_labelFolderName";
            this.m_labelFolderName.Size = new System.Drawing.Size(265, 23);
            this.m_labelFolderName.TabIndex = 28;
            this.m_labelFolderName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // m_btnBrowseFolder
            // 
            this.m_btnBrowseFolder.Location = new System.Drawing.Point(287, 93);
            this.m_btnBrowseFolder.Name = "m_btnBrowseFolder";
            this.m_btnBrowseFolder.Size = new System.Drawing.Size(75, 23);
            this.m_btnBrowseFolder.TabIndex = 29;
            this.m_btnBrowseFolder.Text = "Browse...";
            this.m_btnBrowseFolder.Click += new System.EventHandler(this.cmdBrowse);
            // 
            // m_labelYouHaveChosen
            // 
            this.m_labelYouHaveChosen.Location = new System.Drawing.Point(11, 8);
            this.m_labelYouHaveChosen.Name = "m_labelYouHaveChosen";
            this.m_labelYouHaveChosen.Size = new System.Drawing.Size(350, 23);
            this.m_labelYouHaveChosen.TabIndex = 30;
            this.m_labelYouHaveChosen.Text = "You have chosen to create:";
            this.m_labelYouHaveChosen.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // m_labelBook
            // 
            this.m_labelBook.Location = new System.Drawing.Point(26, 31);
            this.m_labelBook.Name = "m_labelBook";
            this.m_labelBook.Size = new System.Drawing.Size(335, 23);
            this.m_labelBook.TabIndex = 31;
            this.m_labelBook.Text = "(book)";
            this.m_labelBook.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // m_labelPressFinish
            // 
            this.m_labelPressFinish.Location = new System.Drawing.Point(16, 273);
            this.m_labelPressFinish.Name = "m_labelPressFinish";
            this.m_labelPressFinish.Size = new System.Drawing.Size(345, 23);
            this.m_labelPressFinish.TabIndex = 32;
            this.m_labelPressFinish.Text = "Press Finish to create this book.";
            this.m_labelPressFinish.TextAlign = System.Drawing.ContentAlignment.BottomRight;
            // 
            // WizPage_GetDestinationFolder
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.m_labelPressFinish);
            this.Controls.Add(this.m_labelBook);
            this.Controls.Add(this.m_labelYouHaveChosen);
            this.Controls.Add(this.m_btnBrowseFolder);
            this.Controls.Add(this.m_labelFolderName);
            this.Controls.Add(this.m_labelFolder);
            this.Name = "WizPage_GetDestinationFolder";
            this.Size = new System.Drawing.Size(372, 306);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label m_labelFolder;
        private System.Windows.Forms.Label m_labelFolderName;
        private System.Windows.Forms.Button m_btnBrowseFolder;
        private System.Windows.Forms.Label m_labelYouHaveChosen;
        private System.Windows.Forms.Label m_labelBook;
        private System.Windows.Forms.Label m_labelPressFinish;
    }
}
