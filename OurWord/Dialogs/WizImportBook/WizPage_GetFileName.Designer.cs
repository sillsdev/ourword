namespace OurWord.Dialogs.WizImportBook
{
    partial class WizPage_GetFileName
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
            this.m_labelBrowse = new System.Windows.Forms.Label();
            this.m_labelOverview = new System.Windows.Forms.Label();
            this.m_labelFileName = new System.Windows.Forms.Label();
            this.m_btnBrowse = new System.Windows.Forms.Button();
            this.m_labelFileStats = new System.Windows.Forms.Label();
            this.m_btnSpecifyEncoding = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // m_labelBrowse
            // 
            this.m_labelBrowse.AutoSize = true;
            this.m_labelBrowse.Location = new System.Drawing.Point(13, 67);
            this.m_labelBrowse.Name = "m_labelBrowse";
            this.m_labelBrowse.Size = new System.Drawing.Size(233, 13);
            this.m_labelBrowse.TabIndex = 0;
            this.m_labelBrowse.Text = "Please brwose to the file that you wish to import:";
            // 
            // m_labelOverview
            // 
            this.m_labelOverview.Location = new System.Drawing.Point(13, 12);
            this.m_labelOverview.Name = "m_labelOverview";
            this.m_labelOverview.Size = new System.Drawing.Size(385, 46);
            this.m_labelOverview.TabIndex = 1;
            this.m_labelOverview.Text = "This Wizard will guide you through the process of importing a Scripture file into" +
                " OurWord.";
            // 
            // m_labelFileName
            // 
            this.m_labelFileName.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.m_labelFileName.Location = new System.Drawing.Point(16, 89);
            this.m_labelFileName.Name = "m_labelFileName";
            this.m_labelFileName.Size = new System.Drawing.Size(301, 23);
            this.m_labelFileName.TabIndex = 2;
            this.m_labelFileName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // m_btnBrowse
            // 
            this.m_btnBrowse.Location = new System.Drawing.Point(323, 89);
            this.m_btnBrowse.Name = "m_btnBrowse";
            this.m_btnBrowse.Size = new System.Drawing.Size(75, 23);
            this.m_btnBrowse.TabIndex = 3;
            this.m_btnBrowse.Text = "Browse...";
            this.m_btnBrowse.UseVisualStyleBackColor = true;
            this.m_btnBrowse.Click += new System.EventHandler(this.cmdBrowse);
            // 
            // m_labelFileStats
            // 
            this.m_labelFileStats.Location = new System.Drawing.Point(13, 130);
            this.m_labelFileStats.Name = "m_labelFileStats";
            this.m_labelFileStats.Size = new System.Drawing.Size(385, 89);
            this.m_labelFileStats.TabIndex = 4;
            this.m_labelFileStats.Text = "Please wait while OurWord examines this file...";
            this.m_labelFileStats.Visible = false;
            // 
            // m_btnSpecifyEncoding
            // 
            this.m_btnSpecifyEncoding.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_btnSpecifyEncoding.ForeColor = System.Drawing.Color.Red;
            this.m_btnSpecifyEncoding.Location = new System.Drawing.Point(16, 222);
            this.m_btnSpecifyEncoding.Name = "m_btnSpecifyEncoding";
            this.m_btnSpecifyEncoding.Size = new System.Drawing.Size(160, 23);
            this.m_btnSpecifyEncoding.TabIndex = 5;
            this.m_btnSpecifyEncoding.Text = "Specify Encoding...";
            this.m_btnSpecifyEncoding.UseVisualStyleBackColor = true;
            this.m_btnSpecifyEncoding.Click += new System.EventHandler(this.cmdSpecifyEncoding);
            // 
            // WizPage_GetFileName
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.m_btnSpecifyEncoding);
            this.Controls.Add(this.m_labelFileStats);
            this.Controls.Add(this.m_btnBrowse);
            this.Controls.Add(this.m_labelFileName);
            this.Controls.Add(this.m_labelOverview);
            this.Controls.Add(this.m_labelBrowse);
            this.Name = "WizPage_GetFileName";
            this.Size = new System.Drawing.Size(410, 258);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label m_labelBrowse;
        private System.Windows.Forms.Label m_labelOverview;
        private System.Windows.Forms.Label m_labelFileName;
        private System.Windows.Forms.Button m_btnBrowse;
        private System.Windows.Forms.Label m_labelFileStats;
        private System.Windows.Forms.Button m_btnSpecifyEncoding;
    }
}
