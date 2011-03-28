namespace OurWord.Ctrls.Navigation
{
    partial class DlgConcordance
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DlgConcordance));
            this.m_lConcordOn = new System.Windows.Forms.Label();
            this.m_tConcordOn = new System.Windows.Forms.TextBox();
            this.m_checkIgnoreCase = new System.Windows.Forms.CheckBox();
            this.m_List = new System.Windows.Forms.ListView();
            this.m_colRef = new System.Windows.Forms.ColumnHeader();
            this.m_colText = new System.Windows.Forms.ColumnHeader();
            this.m_checkCurrentBookOnly = new System.Windows.Forms.CheckBox();
            this.m_btnClose = new System.Windows.Forms.Button();
            this.m_btnBuildConcordance = new System.Windows.Forms.Button();
            this.m_group = new System.Windows.Forms.GroupBox();
            this.m_labelOccurences = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // m_lConcordOn
            // 
            this.m_lConcordOn.AutoSize = true;
            this.m_lConcordOn.Location = new System.Drawing.Point(23, 21);
            this.m_lConcordOn.Name = "m_lConcordOn";
            this.m_lConcordOn.Size = new System.Drawing.Size(91, 13);
            this.m_lConcordOn.TabIndex = 0;
            this.m_lConcordOn.Text = "Concord On Text:";
            // 
            // m_tConcordOn
            // 
            this.m_tConcordOn.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.m_tConcordOn.Location = new System.Drawing.Point(120, 18);
            this.m_tConcordOn.Name = "m_tConcordOn";
            this.m_tConcordOn.Size = new System.Drawing.Size(239, 20);
            this.m_tConcordOn.TabIndex = 1;
            this.m_tConcordOn.TextChanged += new System.EventHandler(this.cmdConcordOnTextChanged);
            // 
            // m_checkIgnoreCase
            // 
            this.m_checkIgnoreCase.AutoSize = true;
            this.m_checkIgnoreCase.Location = new System.Drawing.Point(26, 45);
            this.m_checkIgnoreCase.Name = "m_checkIgnoreCase";
            this.m_checkIgnoreCase.Size = new System.Drawing.Size(89, 17);
            this.m_checkIgnoreCase.TabIndex = 2;
            this.m_checkIgnoreCase.Text = "Ignore Case?";
            this.m_checkIgnoreCase.UseVisualStyleBackColor = true;
            // 
            // m_List
            // 
            this.m_List.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.m_List.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.m_colRef,
            this.m_colText});
            this.m_List.FullRowSelect = true;
            this.m_List.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.m_List.HideSelection = false;
            this.m_List.Location = new System.Drawing.Point(15, 78);
            this.m_List.MultiSelect = false;
            this.m_List.Name = "m_List";
            this.m_List.OwnerDraw = true;
            this.m_List.Size = new System.Drawing.Size(417, 279);
            this.m_List.TabIndex = 3;
            this.m_List.UseCompatibleStateImageBehavior = false;
            this.m_List.View = System.Windows.Forms.View.Details;
            this.m_List.DrawColumnHeader += new System.Windows.Forms.DrawListViewColumnHeaderEventHandler(this.cmdDrawColumnHeader);
            this.m_List.SizeChanged += new System.EventHandler(this.cmdListViewSizeChanged);
            this.m_List.DoubleClick += new System.EventHandler(this.cmdListDoubleClicked);
            this.m_List.DrawSubItem += new System.Windows.Forms.DrawListViewSubItemEventHandler(this.cmdDrawSubItem);
            // 
            // m_colRef
            // 
            this.m_colRef.Text = "Reference";
            // 
            // m_colText
            // 
            this.m_colText.Text = "Scripture Text";
            this.m_colText.Width = 262;
            // 
            // m_checkCurrentBookOnly
            // 
            this.m_checkCurrentBookOnly.AutoSize = true;
            this.m_checkCurrentBookOnly.Location = new System.Drawing.Point(134, 46);
            this.m_checkCurrentBookOnly.Name = "m_checkCurrentBookOnly";
            this.m_checkCurrentBookOnly.Size = new System.Drawing.Size(181, 17);
            this.m_checkCurrentBookOnly.TabIndex = 4;
            this.m_checkCurrentBookOnly.Text = "Only scan through current book?";
            this.m_checkCurrentBookOnly.UseVisualStyleBackColor = true;
            // 
            // m_btnClose
            // 
            this.m_btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.m_btnClose.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.m_btnClose.Location = new System.Drawing.Point(357, 363);
            this.m_btnClose.Name = "m_btnClose";
            this.m_btnClose.Size = new System.Drawing.Size(75, 23);
            this.m_btnClose.TabIndex = 14;
            this.m_btnClose.Text = "Close";
            this.m_btnClose.Click += new System.EventHandler(this.cmdClose);
            // 
            // m_btnBuildConcordance
            // 
            this.m_btnBuildConcordance.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.m_btnBuildConcordance.Location = new System.Drawing.Point(371, 18);
            this.m_btnBuildConcordance.Name = "m_btnBuildConcordance";
            this.m_btnBuildConcordance.Size = new System.Drawing.Size(56, 43);
            this.m_btnBuildConcordance.TabIndex = 15;
            this.m_btnBuildConcordance.Text = "Build";
            this.m_btnBuildConcordance.UseVisualStyleBackColor = true;
            this.m_btnBuildConcordance.Click += new System.EventHandler(this.cmdBuildConcordance);
            // 
            // m_group
            // 
            this.m_group.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.m_group.Location = new System.Drawing.Point(15, 3);
            this.m_group.Name = "m_group";
            this.m_group.Size = new System.Drawing.Size(417, 69);
            this.m_group.TabIndex = 16;
            this.m_group.TabStop = false;
            this.m_group.Text = "Concordance Settings";
            // 
            // m_labelOccurences
            // 
            this.m_labelOccurences.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.m_labelOccurences.AutoSize = true;
            this.m_labelOccurences.Location = new System.Drawing.Point(12, 368);
            this.m_labelOccurences.Name = "m_labelOccurences";
            this.m_labelOccurences.Size = new System.Drawing.Size(35, 13);
            this.m_labelOccurences.TabIndex = 17;
            this.m_labelOccurences.Text = "(stats)";
            // 
            // DlgConcordance
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.m_btnClose;
            this.ClientSize = new System.Drawing.Size(444, 392);
            this.Controls.Add(this.m_labelOccurences);
            this.Controls.Add(this.m_btnBuildConcordance);
            this.Controls.Add(this.m_btnClose);
            this.Controls.Add(this.m_checkCurrentBookOnly);
            this.Controls.Add(this.m_List);
            this.Controls.Add(this.m_checkIgnoreCase);
            this.Controls.Add(this.m_tConcordOn);
            this.Controls.Add(this.m_lConcordOn);
            this.Controls.Add(this.m_group);
            this.DoubleBuffered = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(425, 350);
            this.Name = "DlgConcordance";
            this.Text = "Concordance";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.cmdLoad);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.cmdFormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label m_lConcordOn;
        private System.Windows.Forms.TextBox m_tConcordOn;
        private System.Windows.Forms.CheckBox m_checkIgnoreCase;
        private System.Windows.Forms.ListView m_List;
        private System.Windows.Forms.ColumnHeader m_colRef;
        private System.Windows.Forms.ColumnHeader m_colText;
        private System.Windows.Forms.CheckBox m_checkCurrentBookOnly;
        private System.Windows.Forms.Button m_btnClose;
        private System.Windows.Forms.Button m_btnBuildConcordance;
        private System.Windows.Forms.GroupBox m_group;
        private System.Windows.Forms.Label m_labelOccurences;
    }
}