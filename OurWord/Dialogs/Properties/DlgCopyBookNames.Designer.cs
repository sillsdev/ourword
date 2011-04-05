namespace OurWord.Dialogs.Properties
{
    partial class DlgCopyBookNames
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
            this.m_labelInstructions = new System.Windows.Forms.Label();
            this.m_comboChoices = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // m_btnCancel
            // 
            this.m_btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.m_btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.m_btnCancel.Location = new System.Drawing.Point(215, 58);
            this.m_btnCancel.Name = "m_btnCancel";
            this.m_btnCancel.Size = new System.Drawing.Size(75, 23);
            this.m_btnCancel.TabIndex = 11;
            this.m_btnCancel.Text = "Cancel";
            // 
            // m_btnOK
            // 
            this.m_btnOK.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.m_btnOK.DialogResult = System.Windows.Forms.DialogResult.Yes;
            this.m_btnOK.Location = new System.Drawing.Point(127, 58);
            this.m_btnOK.Name = "m_btnOK";
            this.m_btnOK.Size = new System.Drawing.Size(75, 23);
            this.m_btnOK.TabIndex = 10;
            this.m_btnOK.Text = "Copy";
            // 
            // m_labelInstructions
            // 
            this.m_labelInstructions.Location = new System.Drawing.Point(12, 9);
            this.m_labelInstructions.Name = "m_labelInstructions";
            this.m_labelInstructions.Size = new System.Drawing.Size(196, 36);
            this.m_labelInstructions.TabIndex = 12;
            this.m_labelInstructions.Text = "Replace the book names in your translation with those from::";
            // 
            // m_comboChoices
            // 
            this.m_comboChoices.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.m_comboChoices.FormattingEnabled = true;
            this.m_comboChoices.Location = new System.Drawing.Point(214, 12);
            this.m_comboChoices.Name = "m_comboChoices";
            this.m_comboChoices.Size = new System.Drawing.Size(191, 21);
            this.m_comboChoices.TabIndex = 13;
            // 
            // DlgCopyBookNames
            // 
            this.AcceptButton = this.m_btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.m_btnCancel;
            this.ClientSize = new System.Drawing.Size(417, 93);
            this.Controls.Add(this.m_comboChoices);
            this.Controls.Add(this.m_labelInstructions);
            this.Controls.Add(this.m_btnCancel);
            this.Controls.Add(this.m_btnOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DlgCopyBookNames";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Copy Book Names";
            this.Load += new System.EventHandler(this.cmdLoad);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button m_btnCancel;
        private System.Windows.Forms.Button m_btnOK;
        private System.Windows.Forms.Label m_labelInstructions;
        private System.Windows.Forms.ComboBox m_comboChoices;
    }
}