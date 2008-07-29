namespace OurWord.Dialogs
{
    partial class DialogExportBook
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
            this.m_btnCancel = new System.Windows.Forms.Button();
            this.m_btnOK = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.m_labelBook = new System.Windows.Forms.Label();
            this.m_labelBookname = new System.Windows.Forms.Label();
            this.m_labelFilename = new System.Windows.Forms.Label();
            this.m_lblFilename = new System.Windows.Forms.Label();
            this.m_btnBrowse = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // m_btnCancel
            // 
            this.m_btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.m_btnCancel.Location = new System.Drawing.Point(263, 134);
            this.m_btnCancel.Name = "m_btnCancel";
            this.m_btnCancel.Size = new System.Drawing.Size(75, 23);
            this.m_btnCancel.TabIndex = 17;
            this.m_btnCancel.Text = "Cancel";
            // 
            // m_btnOK
            // 
            this.m_btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.m_btnOK.Location = new System.Drawing.Point(175, 134);
            this.m_btnOK.Name = "m_btnOK";
            this.m_btnOK.Size = new System.Drawing.Size(75, 23);
            this.m_btnOK.TabIndex = 16;
            this.m_btnOK.Text = "Export";
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(469, 48);
            this.label1.TabIndex = 19;
            this.label1.Text = "This process will export the book to USFM (Paratext) format. It will only export " +
                "the vernacular; any back translations or notes will not be exported.";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // m_labelBook
            // 
            this.m_labelBook.Location = new System.Drawing.Point(12, 57);
            this.m_labelBook.Name = "m_labelBook";
            this.m_labelBook.Size = new System.Drawing.Size(75, 23);
            this.m_labelBook.TabIndex = 20;
            this.m_labelBook.Text = "Book:";
            this.m_labelBook.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // m_labelBookname
            // 
            this.m_labelBookname.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.m_labelBookname.Location = new System.Drawing.Point(93, 57);
            this.m_labelBookname.Name = "m_labelBookname";
            this.m_labelBookname.Size = new System.Drawing.Size(388, 23);
            this.m_labelBookname.TabIndex = 21;
            this.m_labelBookname.Text = "(bookname)";
            this.m_labelBookname.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // m_labelFilename
            // 
            this.m_labelFilename.BackColor = System.Drawing.SystemColors.Window;
            this.m_labelFilename.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.m_labelFilename.Location = new System.Drawing.Point(93, 90);
            this.m_labelFilename.Name = "m_labelFilename";
            this.m_labelFilename.Size = new System.Drawing.Size(307, 23);
            this.m_labelFilename.TabIndex = 24;
            this.m_labelFilename.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // m_lblFilename
            // 
            this.m_lblFilename.Location = new System.Drawing.Point(12, 90);
            this.m_lblFilename.Name = "m_lblFilename";
            this.m_lblFilename.Size = new System.Drawing.Size(75, 23);
            this.m_lblFilename.TabIndex = 23;
            this.m_lblFilename.Text = "Filename:";
            this.m_lblFilename.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // m_btnBrowse
            // 
            this.m_btnBrowse.Location = new System.Drawing.Point(406, 90);
            this.m_btnBrowse.Name = "m_btnBrowse";
            this.m_btnBrowse.Size = new System.Drawing.Size(75, 23);
            this.m_btnBrowse.TabIndex = 22;
            this.m_btnBrowse.Text = "Browse...";
            this.m_btnBrowse.Click += new System.EventHandler(this.cmdBrowse);
            // 
            // DialogExportBook
            // 
            this.AcceptButton = this.m_btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoSize = true;
            this.CancelButton = this.m_btnCancel;
            this.ClientSize = new System.Drawing.Size(493, 169);
            this.ControlBox = false;
            this.Controls.Add(this.m_labelFilename);
            this.Controls.Add(this.m_lblFilename);
            this.Controls.Add(this.m_btnBrowse);
            this.Controls.Add(this.m_labelBookname);
            this.Controls.Add(this.m_labelBook);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.m_btnCancel);
            this.Controls.Add(this.m_btnOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DialogExportBook";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Export Book";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.cmdClosing);
            this.Load += new System.EventHandler(this.cmdLoad);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button m_btnCancel;
        private System.Windows.Forms.Button m_btnOK;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label m_labelBook;
        private System.Windows.Forms.Label m_labelBookname;
        private System.Windows.Forms.Label m_labelFilename;
        private System.Windows.Forms.Label m_lblFilename;
        private System.Windows.Forms.Button m_btnBrowse;
    }
}