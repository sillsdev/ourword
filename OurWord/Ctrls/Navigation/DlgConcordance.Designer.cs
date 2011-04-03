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
            this.m_List = new System.Windows.Forms.ListView();
            this.m_colRef = new System.Windows.Forms.ColumnHeader();
            this.m_colText = new System.Windows.Forms.ColumnHeader();
            this.m_btnClose = new System.Windows.Forms.Button();
            this.m_btnBuildConcordance = new System.Windows.Forms.Button();
            this.m_labelOccurences = new System.Windows.Forms.Label();
            this.m_ctrlFindOptions = new OurWord.Ctrls.Navigation.CtrlFindOptions();
            this.SuspendLayout();
            // 
            // m_lConcordOn
            // 
            this.m_lConcordOn.AutoSize = true;
            this.m_lConcordOn.Location = new System.Drawing.Point(23, 12);
            this.m_lConcordOn.Name = "m_lConcordOn";
            this.m_lConcordOn.Size = new System.Drawing.Size(67, 13);
            this.m_lConcordOn.TabIndex = 0;
            this.m_lConcordOn.Text = "Concord On:";
            // 
            // m_tConcordOn
            // 
            this.m_tConcordOn.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.m_tConcordOn.Location = new System.Drawing.Point(96, 9);
            this.m_tConcordOn.Name = "m_tConcordOn";
            this.m_tConcordOn.Size = new System.Drawing.Size(408, 20);
            this.m_tConcordOn.TabIndex = 1;
            this.m_tConcordOn.TextChanged += new System.EventHandler(this.cmdConcordOnTextChanged);
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
            this.m_List.Location = new System.Drawing.Point(15, 146);
            this.m_List.MultiSelect = false;
            this.m_List.Name = "m_List";
            this.m_List.OwnerDraw = true;
            this.m_List.Size = new System.Drawing.Size(489, 284);
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
            this.m_colRef.Width = 81;
            // 
            // m_colText
            // 
            this.m_colText.Text = "Scripture Text";
            this.m_colText.Width = 383;
            // 
            // m_btnClose
            // 
            this.m_btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.m_btnClose.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.m_btnClose.Location = new System.Drawing.Point(429, 436);
            this.m_btnClose.Name = "m_btnClose";
            this.m_btnClose.Size = new System.Drawing.Size(75, 23);
            this.m_btnClose.TabIndex = 14;
            this.m_btnClose.Text = "Close";
            this.m_btnClose.Click += new System.EventHandler(this.cmdClose);
            // 
            // m_btnBuildConcordance
            // 
            this.m_btnBuildConcordance.Location = new System.Drawing.Point(26, 56);
            this.m_btnBuildConcordance.Name = "m_btnBuildConcordance";
            this.m_btnBuildConcordance.Size = new System.Drawing.Size(56, 43);
            this.m_btnBuildConcordance.TabIndex = 15;
            this.m_btnBuildConcordance.Text = "Build";
            this.m_btnBuildConcordance.UseVisualStyleBackColor = true;
            this.m_btnBuildConcordance.Click += new System.EventHandler(this.cmdBuildConcordance);
            // 
            // m_labelOccurences
            // 
            this.m_labelOccurences.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.m_labelOccurences.AutoSize = true;
            this.m_labelOccurences.Location = new System.Drawing.Point(12, 441);
            this.m_labelOccurences.Name = "m_labelOccurences";
            this.m_labelOccurences.Size = new System.Drawing.Size(35, 13);
            this.m_labelOccurences.TabIndex = 17;
            this.m_labelOccurences.Text = "(stats)";
            // 
            // m_ctrlFindOptions
            // 
            this.m_ctrlFindOptions.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.m_ctrlFindOptions.IgnoreCase = false;
            this.m_ctrlFindOptions.Location = new System.Drawing.Point(96, 38);
            this.m_ctrlFindOptions.Name = "m_ctrlFindOptions";
            this.m_ctrlFindOptions.OnlyScanCurrentBook = false;
            this.m_ctrlFindOptions.Size = new System.Drawing.Size(408, 89);
            this.m_ctrlFindOptions.TabIndex = 18;
            // 
            // DlgConcordance
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.m_btnClose;
            this.ClientSize = new System.Drawing.Size(516, 465);
            this.Controls.Add(this.m_ctrlFindOptions);
            this.Controls.Add(this.m_labelOccurences);
            this.Controls.Add(this.m_btnBuildConcordance);
            this.Controls.Add(this.m_btnClose);
            this.Controls.Add(this.m_List);
            this.Controls.Add(this.m_tConcordOn);
            this.Controls.Add(this.m_lConcordOn);
            this.DoubleBuffered = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(425, 350);
            this.Name = "DlgConcordance";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Concordance";
            this.Load += new System.EventHandler(this.cmdLoad);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.cmdFormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label m_lConcordOn;
        private System.Windows.Forms.TextBox m_tConcordOn;
        private System.Windows.Forms.ListView m_List;
        private System.Windows.Forms.ColumnHeader m_colRef;
        private System.Windows.Forms.ColumnHeader m_colText;
        private System.Windows.Forms.Button m_btnClose;
        private System.Windows.Forms.Button m_btnBuildConcordance;
        private System.Windows.Forms.Label m_labelOccurences;
        private CtrlFindOptions m_ctrlFindOptions;
    }
}