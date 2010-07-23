namespace OurWord.Dialogs.WizImportBook
{
    partial class DlgSpecifyEncoding
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DlgSpecifyEncoding));
            this.m_btnHelp = new System.Windows.Forms.Button();
            this.m_btnCancel = new System.Windows.Forms.Button();
            this.m_btnOK = new System.Windows.Forms.Button();
            this.m_labelInstructions = new System.Windows.Forms.Label();
            this.m_comboEncodings = new System.Windows.Forms.ComboBox();
            this.m_labelEncoding = new System.Windows.Forms.Label();
            this.m_rtbFile = new System.Windows.Forms.RichTextBox();
            this.m_labelExamineYourFile = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // m_btnHelp
            // 
            this.m_btnHelp.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.m_btnHelp.Image = ((System.Drawing.Image)(resources.GetObject("m_btnHelp.Image")));
            this.m_btnHelp.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.m_btnHelp.Location = new System.Drawing.Point(346, 404);
            this.m_btnHelp.Name = "m_btnHelp";
            this.m_btnHelp.Size = new System.Drawing.Size(75, 23);
            this.m_btnHelp.TabIndex = 20;
            this.m_btnHelp.Text = "Help...";
            // 
            // m_btnCancel
            // 
            this.m_btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.m_btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.m_btnCancel.Location = new System.Drawing.Point(258, 404);
            this.m_btnCancel.Name = "m_btnCancel";
            this.m_btnCancel.Size = new System.Drawing.Size(75, 23);
            this.m_btnCancel.TabIndex = 19;
            this.m_btnCancel.Text = "Cancel";
            // 
            // m_btnOK
            // 
            this.m_btnOK.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.m_btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.m_btnOK.Location = new System.Drawing.Point(170, 404);
            this.m_btnOK.Name = "m_btnOK";
            this.m_btnOK.Size = new System.Drawing.Size(75, 23);
            this.m_btnOK.TabIndex = 18;
            this.m_btnOK.Text = "OK";
            // 
            // m_labelInstructions
            // 
            this.m_labelInstructions.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.m_labelInstructions.Location = new System.Drawing.Point(12, 9);
            this.m_labelInstructions.Name = "m_labelInstructions";
            this.m_labelInstructions.Size = new System.Drawing.Size(573, 70);
            this.m_labelInstructions.TabIndex = 21;
            this.m_labelInstructions.Text = resources.GetString("m_labelInstructions.Text");
            this.m_labelInstructions.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // m_comboEncodings
            // 
            this.m_comboEncodings.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.m_comboEncodings.FormattingEnabled = true;
            this.m_comboEncodings.Location = new System.Drawing.Point(118, 82);
            this.m_comboEncodings.MaxDropDownItems = 30;
            this.m_comboEncodings.Name = "m_comboEncodings";
            this.m_comboEncodings.Size = new System.Drawing.Size(467, 21);
            this.m_comboEncodings.TabIndex = 23;
            this.m_comboEncodings.SelectedIndexChanged += new System.EventHandler(this.cmdNewEncodingChosen);
            // 
            // m_labelEncoding
            // 
            this.m_labelEncoding.Location = new System.Drawing.Point(12, 82);
            this.m_labelEncoding.Name = "m_labelEncoding";
            this.m_labelEncoding.Size = new System.Drawing.Size(100, 23);
            this.m_labelEncoding.TabIndex = 22;
            this.m_labelEncoding.Text = "Encoding:";
            this.m_labelEncoding.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // m_rtbFile
            // 
            this.m_rtbFile.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.m_rtbFile.Location = new System.Drawing.Point(15, 132);
            this.m_rtbFile.Name = "m_rtbFile";
            this.m_rtbFile.ReadOnly = true;
            this.m_rtbFile.Size = new System.Drawing.Size(570, 266);
            this.m_rtbFile.TabIndex = 24;
            this.m_rtbFile.Text = "";
            // 
            // m_labelExamineYourFile
            // 
            this.m_labelExamineYourFile.Location = new System.Drawing.Point(12, 106);
            this.m_labelExamineYourFile.Name = "m_labelExamineYourFile";
            this.m_labelExamineYourFile.Size = new System.Drawing.Size(415, 23);
            this.m_labelExamineYourFile.TabIndex = 25;
            this.m_labelExamineYourFile.Text = "Examine your file carefully to make sure all letters are displaying correctly.";
            this.m_labelExamineYourFile.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // DlgSpecifyEncoding
            // 
            this.AcceptButton = this.m_btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.m_btnCancel;
            this.ClientSize = new System.Drawing.Size(597, 439);
            this.Controls.Add(this.m_labelExamineYourFile);
            this.Controls.Add(this.m_rtbFile);
            this.Controls.Add(this.m_comboEncodings);
            this.Controls.Add(this.m_labelEncoding);
            this.Controls.Add(this.m_labelInstructions);
            this.Controls.Add(this.m_btnHelp);
            this.Controls.Add(this.m_btnCancel);
            this.Controls.Add(this.m_btnOK);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(450, 430);
            this.Name = "DlgSpecifyEncoding";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Specify Encoding";
            this.Load += new System.EventHandler(this.cmdLoad);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button m_btnHelp;
        private System.Windows.Forms.Button m_btnCancel;
        private System.Windows.Forms.Button m_btnOK;
        private System.Windows.Forms.Label m_labelInstructions;
        private System.Windows.Forms.ComboBox m_comboEncodings;
        private System.Windows.Forms.Label m_labelEncoding;
        private System.Windows.Forms.RichTextBox m_rtbFile;
        private System.Windows.Forms.Label m_labelExamineYourFile;
    }
}