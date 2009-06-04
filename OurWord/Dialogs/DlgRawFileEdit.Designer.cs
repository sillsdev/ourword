namespace OurWord.Dialogs
{
    partial class DlgRawFileEdit
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DlgRawFileEdit));
            this.m_btnHelp = new System.Windows.Forms.Button();
            this.m_btnCancel = new System.Windows.Forms.Button();
            this.m_btnSave = new System.Windows.Forms.Button();
            this.m_rtfText = new System.Windows.Forms.RichTextBox();
            this.m_labelFind = new System.Windows.Forms.Label();
            this.m_textSearch = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // m_btnHelp
            // 
            this.m_btnHelp.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.m_btnHelp.Image = ((System.Drawing.Image)(resources.GetObject("m_btnHelp.Image")));
            this.m_btnHelp.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.m_btnHelp.Location = new System.Drawing.Point(399, 648);
            this.m_btnHelp.Name = "m_btnHelp";
            this.m_btnHelp.Size = new System.Drawing.Size(96, 23);
            this.m_btnHelp.TabIndex = 11;
            this.m_btnHelp.Text = "Help...";
            // 
            // m_btnCancel
            // 
            this.m_btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.m_btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.m_btnCancel.Location = new System.Drawing.Point(295, 648);
            this.m_btnCancel.Name = "m_btnCancel";
            this.m_btnCancel.Size = new System.Drawing.Size(96, 23);
            this.m_btnCancel.TabIndex = 10;
            this.m_btnCancel.Text = "Cancel";
            // 
            // m_btnSave
            // 
            this.m_btnSave.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.m_btnSave.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.m_btnSave.Location = new System.Drawing.Point(191, 648);
            this.m_btnSave.Name = "m_btnSave";
            this.m_btnSave.Size = new System.Drawing.Size(96, 23);
            this.m_btnSave.TabIndex = 9;
            this.m_btnSave.Text = "Save";
            // 
            // m_rtfText
            // 
            this.m_rtfText.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.m_rtfText.HideSelection = false;
            this.m_rtfText.Location = new System.Drawing.Point(12, 29);
            this.m_rtfText.Name = "m_rtfText";
            this.m_rtfText.Size = new System.Drawing.Size(648, 613);
            this.m_rtfText.TabIndex = 12;
            this.m_rtfText.Text = "(File Goes here)";
            this.m_rtfText.WordWrap = false;
            // 
            // m_labelFind
            // 
            this.m_labelFind.Location = new System.Drawing.Point(12, 3);
            this.m_labelFind.Name = "m_labelFind";
            this.m_labelFind.Size = new System.Drawing.Size(48, 23);
            this.m_labelFind.TabIndex = 14;
            this.m_labelFind.Text = "Find:";
            this.m_labelFind.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // m_textSearch
            // 
            this.m_textSearch.Location = new System.Drawing.Point(66, 6);
            this.m_textSearch.Name = "m_textSearch";
            this.m_textSearch.Size = new System.Drawing.Size(120, 20);
            this.m_textSearch.TabIndex = 15;
            this.m_textSearch.KeyDown += new System.Windows.Forms.KeyEventHandler(this.cmdFindBoxKeyDown);
            // 
            // DlgRawFileEdit
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.m_btnCancel;
            this.ClientSize = new System.Drawing.Size(672, 683);
            this.ControlBox = false;
            this.Controls.Add(this.m_textSearch);
            this.Controls.Add(this.m_labelFind);
            this.Controls.Add(this.m_rtfText);
            this.Controls.Add(this.m_btnHelp);
            this.Controls.Add(this.m_btnCancel);
            this.Controls.Add(this.m_btnSave);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DlgRawFileEdit";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Edit Raw File";
            this.Load += new System.EventHandler(this.cmdLoad);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.cmdClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        protected System.Windows.Forms.Button m_btnHelp;
        protected System.Windows.Forms.Button m_btnCancel;
        protected System.Windows.Forms.Button m_btnSave;
        protected System.Windows.Forms.RichTextBox m_rtfText;
        private System.Windows.Forms.Label m_labelFind;
        private System.Windows.Forms.TextBox m_textSearch;
    }
}