namespace OurWord.Dialogs
{
    partial class DlgExportProgress
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
            this.m_labelHeader = new System.Windows.Forms.Label();
            this.m_labelCurrently = new System.Windows.Forms.Label();
            this.m_labelBookName = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // m_btnCancel
            // 
            this.m_btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.m_btnCancel.Location = new System.Drawing.Point(158, 109);
            this.m_btnCancel.Name = "m_btnCancel";
            this.m_btnCancel.Size = new System.Drawing.Size(103, 23);
            this.m_btnCancel.TabIndex = 8;
            this.m_btnCancel.Text = "Cancel";
            this.m_btnCancel.UseVisualStyleBackColor = true;
            this.m_btnCancel.Click += new System.EventHandler(this.cmdCancel);
            // 
            // m_labelHeader
            // 
            this.m_labelHeader.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_labelHeader.Location = new System.Drawing.Point(12, 9);
            this.m_labelHeader.Name = "m_labelHeader";
            this.m_labelHeader.Size = new System.Drawing.Size(393, 29);
            this.m_labelHeader.TabIndex = 9;
            this.m_labelHeader.Text = "Please wait while OurWord exports your project.";
            this.m_labelHeader.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // m_labelCurrently
            // 
            this.m_labelCurrently.Location = new System.Drawing.Point(12, 50);
            this.m_labelCurrently.Name = "m_labelCurrently";
            this.m_labelCurrently.Size = new System.Drawing.Size(123, 23);
            this.m_labelCurrently.TabIndex = 10;
            this.m_labelCurrently.Text = "Currently Exporting:";
            this.m_labelCurrently.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // m_labelBookName
            // 
            this.m_labelBookName.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.m_labelBookName.Location = new System.Drawing.Point(141, 50);
            this.m_labelBookName.Name = "m_labelBookName";
            this.m_labelBookName.Size = new System.Drawing.Size(264, 23);
            this.m_labelBookName.TabIndex = 32;
            this.m_labelBookName.Text = "Setting Up...";
            this.m_labelBookName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // DlgExportProgress
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.m_btnCancel;
            this.ClientSize = new System.Drawing.Size(417, 142);
            this.ControlBox = false;
            this.Controls.Add(this.m_labelBookName);
            this.Controls.Add(this.m_labelCurrently);
            this.Controls.Add(this.m_labelHeader);
            this.Controls.Add(this.m_btnCancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DlgExportProgress";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Export";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button m_btnCancel;
        private System.Windows.Forms.Label m_labelHeader;
        private System.Windows.Forms.Label m_labelCurrently;
        private System.Windows.Forms.Label m_labelBookName;
    }
}