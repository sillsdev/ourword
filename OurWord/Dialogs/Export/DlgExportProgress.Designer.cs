namespace OurWord.Dialogs.Export
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
            this.m_labelCurrentActivity = new System.Windows.Forms.Label();
            this.m_ProgressBar = new System.Windows.Forms.ProgressBar();
            this.SuspendLayout();
            // 
            // m_btnCancel
            // 
            this.m_btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.m_btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.m_btnCancel.Location = new System.Drawing.Point(159, 124);
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
            this.m_labelHeader.Text = "Please wait while OurWord exports your translation.";
            this.m_labelHeader.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // m_labelCurrentActivity
            // 
            this.m_labelCurrentActivity.Location = new System.Drawing.Point(15, 46);
            this.m_labelCurrentActivity.Name = "m_labelCurrentActivity";
            this.m_labelCurrentActivity.Size = new System.Drawing.Size(390, 23);
            this.m_labelCurrentActivity.TabIndex = 32;
            this.m_labelCurrentActivity.Text = "Setting Up...";
            this.m_labelCurrentActivity.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // m_ProgressBar
            // 
            this.m_ProgressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.m_ProgressBar.Location = new System.Drawing.Point(15, 72);
            this.m_ProgressBar.Name = "m_ProgressBar";
            this.m_ProgressBar.Size = new System.Drawing.Size(390, 23);
            this.m_ProgressBar.Step = 1;
            this.m_ProgressBar.TabIndex = 33;
            // 
            // DlgExportProgress
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.m_btnCancel;
            this.ClientSize = new System.Drawing.Size(417, 159);
            this.ControlBox = false;
            this.Controls.Add(this.m_ProgressBar);
            this.Controls.Add(this.m_labelCurrentActivity);
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
        private System.Windows.Forms.Label m_labelCurrentActivity;
        private System.Windows.Forms.ProgressBar m_ProgressBar;
    }
}