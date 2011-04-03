namespace OurWord.Ctrls.Navigation
{
    partial class CtrlFindOptions
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
            this.m_comboSearchType = new System.Windows.Forms.ComboBox();
            this.m_checkMustBe = new System.Windows.Forms.CheckBox();
            this.m_checkCurrentBookOnly = new System.Windows.Forms.CheckBox();
            this.m_checkIgnoreCase = new System.Windows.Forms.CheckBox();
            this.m_groupOptions = new System.Windows.Forms.GroupBox();
            this.SuspendLayout();
            // 
            // m_comboSearchType
            // 
            this.m_comboSearchType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.m_comboSearchType.FormattingEnabled = true;
            this.m_comboSearchType.Items.AddRange(new object[] {
            "Entire Word",
            "At Beginning of Word",
            "At End of Word"});
            this.m_comboSearchType.Location = new System.Drawing.Point(93, 62);
            this.m_comboSearchType.Name = "m_comboSearchType";
            this.m_comboSearchType.Size = new System.Drawing.Size(201, 21);
            this.m_comboSearchType.TabIndex = 31;
            this.m_comboSearchType.Text = "Entire Word";
            this.m_comboSearchType.TextChanged += new System.EventHandler(this.cmdComboChanged);
            // 
            // m_checkMustBe
            // 
            this.m_checkMustBe.AutoSize = true;
            this.m_checkMustBe.Location = new System.Drawing.Point(16, 63);
            this.m_checkMustBe.Name = "m_checkMustBe";
            this.m_checkMustBe.Size = new System.Drawing.Size(67, 17);
            this.m_checkMustBe.TabIndex = 30;
            this.m_checkMustBe.Text = "Must be:";
            this.m_checkMustBe.UseVisualStyleBackColor = true;
            this.m_checkMustBe.CheckedChanged += new System.EventHandler(this.cmdOptionChanged);
            // 
            // m_checkCurrentBookOnly
            // 
            this.m_checkCurrentBookOnly.AutoSize = true;
            this.m_checkCurrentBookOnly.Location = new System.Drawing.Point(16, 40);
            this.m_checkCurrentBookOnly.Name = "m_checkCurrentBookOnly";
            this.m_checkCurrentBookOnly.Size = new System.Drawing.Size(181, 17);
            this.m_checkCurrentBookOnly.TabIndex = 29;
            this.m_checkCurrentBookOnly.Text = "Only scan through current book?";
            this.m_checkCurrentBookOnly.UseVisualStyleBackColor = true;
            this.m_checkCurrentBookOnly.CheckedChanged += new System.EventHandler(this.cmdOptionChanged);
            // 
            // m_checkIgnoreCase
            // 
            this.m_checkIgnoreCase.AutoSize = true;
            this.m_checkIgnoreCase.Location = new System.Drawing.Point(16, 17);
            this.m_checkIgnoreCase.Name = "m_checkIgnoreCase";
            this.m_checkIgnoreCase.Size = new System.Drawing.Size(89, 17);
            this.m_checkIgnoreCase.TabIndex = 28;
            this.m_checkIgnoreCase.Text = "Ignore Case?";
            this.m_checkIgnoreCase.UseVisualStyleBackColor = true;
            this.m_checkIgnoreCase.CheckedChanged += new System.EventHandler(this.cmdOptionChanged);
            // 
            // m_groupOptions
            // 
            this.m_groupOptions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_groupOptions.Location = new System.Drawing.Point(0, 0);
            this.m_groupOptions.Name = "m_groupOptions";
            this.m_groupOptions.Size = new System.Drawing.Size(306, 89);
            this.m_groupOptions.TabIndex = 32;
            this.m_groupOptions.TabStop = false;
            this.m_groupOptions.Text = "Options:";
            // 
            // CtrlFindOptions
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.m_comboSearchType);
            this.Controls.Add(this.m_checkMustBe);
            this.Controls.Add(this.m_checkCurrentBookOnly);
            this.Controls.Add(this.m_checkIgnoreCase);
            this.Controls.Add(this.m_groupOptions);
            this.Name = "CtrlFindOptions";
            this.Size = new System.Drawing.Size(306, 89);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox m_comboSearchType;
        private System.Windows.Forms.CheckBox m_checkMustBe;
        private System.Windows.Forms.CheckBox m_checkCurrentBookOnly;
        private System.Windows.Forms.CheckBox m_checkIgnoreCase;
        private System.Windows.Forms.GroupBox m_groupOptions;
    }
}
