namespace OurWord.Dialogs.Properties
{
    partial class DlgTranslationProperties
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DlgTranslationProperties));
            this.m_tabctrlTranslation = new System.Windows.Forms.TabControl();
            this.m_tabBooks = new System.Windows.Forms.TabPage();
            this.m_radioStartedBooks = new System.Windows.Forms.RadioButton();
            this.m_radioOldTestament = new System.Windows.Forms.RadioButton();
            this.m_radioNewTestament = new System.Windows.Forms.RadioButton();
            this.m_radioAll = new System.Windows.Forms.RadioButton();
            this.m_gridBooks = new System.Windows.Forms.DataGridView();
            this.m_colAbbreviation = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.m_colBookName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.m_colStage = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.m_tabOther = new System.Windows.Forms.TabPage();
            this.m_lblLanguageName = new System.Windows.Forms.Label();
            this.m_editLanguageName = new System.Windows.Forms.TextBox();
            this.m_bOK = new System.Windows.Forms.Button();
            this.m_btnCreate = new System.Windows.Forms.Button();
            this.m_btnImport = new System.Windows.Forms.Button();
            this.m_btnRemove = new System.Windows.Forms.Button();
            this.m_btnProperties = new System.Windows.Forms.Button();
            this.m_btnEditRawFile = new System.Windows.Forms.Button();
            this.m_btnCopyNames = new System.Windows.Forms.Button();
            this.m_LiterateSettings = new OurWord.Edit.LiterateSettingsWnd();
            this.m_tabctrlTranslation.SuspendLayout();
            this.m_tabBooks.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.m_gridBooks)).BeginInit();
            this.m_tabOther.SuspendLayout();
            this.SuspendLayout();
            // 
            // m_tabctrlTranslation
            // 
            this.m_tabctrlTranslation.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.m_tabctrlTranslation.Controls.Add(this.m_tabBooks);
            this.m_tabctrlTranslation.Controls.Add(this.m_tabOther);
            this.m_tabctrlTranslation.Location = new System.Drawing.Point(2, 2);
            this.m_tabctrlTranslation.Name = "m_tabctrlTranslation";
            this.m_tabctrlTranslation.SelectedIndex = 0;
            this.m_tabctrlTranslation.Size = new System.Drawing.Size(475, 360);
            this.m_tabctrlTranslation.TabIndex = 7;
            // 
            // m_tabBooks
            // 
            this.m_tabBooks.Controls.Add(this.m_btnCopyNames);
            this.m_tabBooks.Controls.Add(this.m_btnEditRawFile);
            this.m_tabBooks.Controls.Add(this.m_btnProperties);
            this.m_tabBooks.Controls.Add(this.m_btnRemove);
            this.m_tabBooks.Controls.Add(this.m_btnImport);
            this.m_tabBooks.Controls.Add(this.m_btnCreate);
            this.m_tabBooks.Controls.Add(this.m_radioStartedBooks);
            this.m_tabBooks.Controls.Add(this.m_radioOldTestament);
            this.m_tabBooks.Controls.Add(this.m_radioNewTestament);
            this.m_tabBooks.Controls.Add(this.m_radioAll);
            this.m_tabBooks.Controls.Add(this.m_gridBooks);
            this.m_tabBooks.Location = new System.Drawing.Point(4, 22);
            this.m_tabBooks.Name = "m_tabBooks";
            this.m_tabBooks.Padding = new System.Windows.Forms.Padding(3);
            this.m_tabBooks.Size = new System.Drawing.Size(467, 334);
            this.m_tabBooks.TabIndex = 4;
            this.m_tabBooks.Text = "Books";
            this.m_tabBooks.UseVisualStyleBackColor = true;
            // 
            // m_radioStartedBooks
            // 
            this.m_radioStartedBooks.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.m_radioStartedBooks.Location = new System.Drawing.Point(350, 240);
            this.m_radioStartedBooks.Name = "m_radioStartedBooks";
            this.m_radioStartedBooks.Size = new System.Drawing.Size(111, 17);
            this.m_radioStartedBooks.TabIndex = 112;
            this.m_radioStartedBooks.TabStop = true;
            this.m_radioStartedBooks.Text = "Started Books";
            this.m_radioStartedBooks.UseVisualStyleBackColor = true;
            this.m_radioStartedBooks.CheckedChanged += new System.EventHandler(this.cmdFilterOnStartedBooks);
            // 
            // m_radioOldTestament
            // 
            this.m_radioOldTestament.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.m_radioOldTestament.Location = new System.Drawing.Point(350, 222);
            this.m_radioOldTestament.Name = "m_radioOldTestament";
            this.m_radioOldTestament.Size = new System.Drawing.Size(111, 17);
            this.m_radioOldTestament.TabIndex = 111;
            this.m_radioOldTestament.TabStop = true;
            this.m_radioOldTestament.Text = "Old Testament";
            this.m_radioOldTestament.UseVisualStyleBackColor = true;
            this.m_radioOldTestament.CheckedChanged += new System.EventHandler(this.cmdFilterOnOT);
            // 
            // m_radioNewTestament
            // 
            this.m_radioNewTestament.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.m_radioNewTestament.Location = new System.Drawing.Point(350, 204);
            this.m_radioNewTestament.Name = "m_radioNewTestament";
            this.m_radioNewTestament.Size = new System.Drawing.Size(111, 17);
            this.m_radioNewTestament.TabIndex = 110;
            this.m_radioNewTestament.TabStop = true;
            this.m_radioNewTestament.Text = "New Testament";
            this.m_radioNewTestament.UseVisualStyleBackColor = true;
            this.m_radioNewTestament.CheckedChanged += new System.EventHandler(this.cmdFilterOnNT);
            // 
            // m_radioAll
            // 
            this.m_radioAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.m_radioAll.Location = new System.Drawing.Point(350, 186);
            this.m_radioAll.Name = "m_radioAll";
            this.m_radioAll.Size = new System.Drawing.Size(111, 17);
            this.m_radioAll.TabIndex = 109;
            this.m_radioAll.TabStop = true;
            this.m_radioAll.Text = "All";
            this.m_radioAll.UseVisualStyleBackColor = true;
            this.m_radioAll.CheckedChanged += new System.EventHandler(this.cmdFilterOnAll);
            // 
            // m_gridBooks
            // 
            this.m_gridBooks.AllowUserToAddRows = false;
            this.m_gridBooks.AllowUserToDeleteRows = false;
            this.m_gridBooks.AllowUserToResizeRows = false;
            this.m_gridBooks.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.m_gridBooks.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.m_gridBooks.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.m_colAbbreviation,
            this.m_colBookName,
            this.m_colStage});
            this.m_gridBooks.Location = new System.Drawing.Point(6, 6);
            this.m_gridBooks.MultiSelect = false;
            this.m_gridBooks.Name = "m_gridBooks";
            this.m_gridBooks.RowHeadersVisible = false;
            this.m_gridBooks.RowTemplate.Height = 19;
            this.m_gridBooks.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.m_gridBooks.Size = new System.Drawing.Size(338, 322);
            this.m_gridBooks.TabIndex = 0;
            this.m_gridBooks.Resize += new System.EventHandler(this.cmdDataGridResized);
            this.m_gridBooks.SelectionChanged += new System.EventHandler(this.cmdGridSelectionChanged);
            // 
            // m_colAbbreviation
            // 
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_colAbbreviation.DefaultCellStyle = dataGridViewCellStyle1;
            this.m_colAbbreviation.HeaderText = "Abbrev";
            this.m_colAbbreviation.Name = "m_colAbbreviation";
            this.m_colAbbreviation.ReadOnly = true;
            this.m_colAbbreviation.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.m_colAbbreviation.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.m_colAbbreviation.ToolTipText = "The standard 3-letter abbreviation for this book, commonly used in Bible translat" +
                "ion organizations to identify the book.";
            this.m_colAbbreviation.Width = 50;
            // 
            // m_colBookName
            // 
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_colBookName.DefaultCellStyle = dataGridViewCellStyle2;
            this.m_colBookName.HeaderText = "Book Name";
            this.m_colBookName.Name = "m_colBookName";
            this.m_colBookName.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.m_colBookName.ToolTipText = "The name of the book in your language.";
            this.m_colBookName.Width = 170;
            // 
            // m_colStage
            // 
            this.m_colStage.DisplayStyle = System.Windows.Forms.DataGridViewComboBoxDisplayStyle.ComboBox;
            this.m_colStage.DisplayStyleForCurrentCellOnly = true;
            this.m_colStage.DropDownWidth = 100;
            this.m_colStage.HeaderText = "Stage";
            this.m_colStage.MaxDropDownItems = 15;
            this.m_colStage.MinimumWidth = 40;
            this.m_colStage.Name = "m_colStage";
            this.m_colStage.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.m_colStage.ToolTipText = "E.g., Draft, Revision, Consultant Checked, etc.";
            this.m_colStage.Width = 90;
            // 
            // m_tabOther
            // 
            this.m_tabOther.Controls.Add(this.m_LiterateSettings);
            this.m_tabOther.Controls.Add(this.m_lblLanguageName);
            this.m_tabOther.Controls.Add(this.m_editLanguageName);
            this.m_tabOther.Location = new System.Drawing.Point(4, 22);
            this.m_tabOther.Name = "m_tabOther";
            this.m_tabOther.Padding = new System.Windows.Forms.Padding(3);
            this.m_tabOther.Size = new System.Drawing.Size(467, 334);
            this.m_tabOther.TabIndex = 0;
            this.m_tabOther.Text = "Other";
            this.m_tabOther.UseVisualStyleBackColor = true;
            // 
            // m_lblLanguageName
            // 
            this.m_lblLanguageName.Location = new System.Drawing.Point(6, 3);
            this.m_lblLanguageName.Name = "m_lblLanguageName";
            this.m_lblLanguageName.Size = new System.Drawing.Size(100, 27);
            this.m_lblLanguageName.TabIndex = 0;
            this.m_lblLanguageName.Text = "Language Name:";
            this.m_lblLanguageName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // m_editLanguageName
            // 
            this.m_editLanguageName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.m_editLanguageName.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_editLanguageName.Location = new System.Drawing.Point(112, 6);
            this.m_editLanguageName.Name = "m_editLanguageName";
            this.m_editLanguageName.Size = new System.Drawing.Size(339, 26);
            this.m_editLanguageName.TabIndex = 1;
            this.m_editLanguageName.Text = "Translation Name";
            // 
            // m_bOK
            // 
            this.m_bOK.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.m_bOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.m_bOK.Location = new System.Drawing.Point(210, 368);
            this.m_bOK.Name = "m_bOK";
            this.m_bOK.Size = new System.Drawing.Size(75, 23);
            this.m_bOK.TabIndex = 8;
            this.m_bOK.Text = "OK";
            this.m_bOK.UseVisualStyleBackColor = true;
            // 
            // m_btnCreate
            // 
            this.m_btnCreate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.m_btnCreate.FlatAppearance.BorderSize = 0;
            this.m_btnCreate.FlatAppearance.MouseDownBackColor = System.Drawing.SystemColors.MenuHighlight;
            this.m_btnCreate.FlatAppearance.MouseOverBackColor = System.Drawing.SystemColors.InactiveCaption;
            this.m_btnCreate.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.m_btnCreate.Location = new System.Drawing.Point(350, 8);
            this.m_btnCreate.Name = "m_btnCreate";
            this.m_btnCreate.Size = new System.Drawing.Size(111, 20);
            this.m_btnCreate.TabIndex = 114;
            this.m_btnCreate.Text = "Create...";
            this.m_btnCreate.UseVisualStyleBackColor = true;
            this.m_btnCreate.Click += new System.EventHandler(this.cmdCreateBook);
            // 
            // m_btnImport
            // 
            this.m_btnImport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.m_btnImport.FlatAppearance.BorderSize = 0;
            this.m_btnImport.FlatAppearance.MouseDownBackColor = System.Drawing.SystemColors.MenuHighlight;
            this.m_btnImport.FlatAppearance.MouseOverBackColor = System.Drawing.SystemColors.InactiveCaption;
            this.m_btnImport.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.m_btnImport.Location = new System.Drawing.Point(350, 34);
            this.m_btnImport.Name = "m_btnImport";
            this.m_btnImport.Size = new System.Drawing.Size(111, 20);
            this.m_btnImport.TabIndex = 115;
            this.m_btnImport.Text = "Import...";
            this.m_btnImport.UseVisualStyleBackColor = true;
            this.m_btnImport.Click += new System.EventHandler(this.cmdImportBook);
            // 
            // m_btnRemove
            // 
            this.m_btnRemove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.m_btnRemove.FlatAppearance.BorderSize = 0;
            this.m_btnRemove.FlatAppearance.MouseDownBackColor = System.Drawing.SystemColors.MenuHighlight;
            this.m_btnRemove.FlatAppearance.MouseOverBackColor = System.Drawing.SystemColors.InactiveCaption;
            this.m_btnRemove.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.m_btnRemove.Location = new System.Drawing.Point(350, 60);
            this.m_btnRemove.Name = "m_btnRemove";
            this.m_btnRemove.Size = new System.Drawing.Size(111, 20);
            this.m_btnRemove.TabIndex = 116;
            this.m_btnRemove.Text = "Delete...";
            this.m_btnRemove.UseVisualStyleBackColor = true;
            this.m_btnRemove.Click += new System.EventHandler(this.cmdRemoveBook);
            // 
            // m_btnProperties
            // 
            this.m_btnProperties.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.m_btnProperties.FlatAppearance.BorderSize = 0;
            this.m_btnProperties.FlatAppearance.MouseDownBackColor = System.Drawing.SystemColors.MenuHighlight;
            this.m_btnProperties.FlatAppearance.MouseOverBackColor = System.Drawing.SystemColors.InactiveCaption;
            this.m_btnProperties.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.m_btnProperties.Location = new System.Drawing.Point(350, 86);
            this.m_btnProperties.Name = "m_btnProperties";
            this.m_btnProperties.Size = new System.Drawing.Size(111, 20);
            this.m_btnProperties.TabIndex = 117;
            this.m_btnProperties.Text = "Properties...";
            this.m_btnProperties.UseVisualStyleBackColor = true;
            this.m_btnProperties.Click += new System.EventHandler(this.cmdBookProperties);
            // 
            // m_btnEditRawFile
            // 
            this.m_btnEditRawFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.m_btnEditRawFile.FlatAppearance.BorderSize = 0;
            this.m_btnEditRawFile.FlatAppearance.MouseDownBackColor = System.Drawing.SystemColors.MenuHighlight;
            this.m_btnEditRawFile.FlatAppearance.MouseOverBackColor = System.Drawing.SystemColors.InactiveCaption;
            this.m_btnEditRawFile.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.m_btnEditRawFile.Location = new System.Drawing.Point(350, 112);
            this.m_btnEditRawFile.Name = "m_btnEditRawFile";
            this.m_btnEditRawFile.Size = new System.Drawing.Size(111, 20);
            this.m_btnEditRawFile.TabIndex = 118;
            this.m_btnEditRawFile.Text = "Edit Raw File...";
            this.m_btnEditRawFile.UseVisualStyleBackColor = true;
            this.m_btnEditRawFile.Click += new System.EventHandler(this.cmdEditRawFile);
            // 
            // m_btnCopyNames
            // 
            this.m_btnCopyNames.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.m_btnCopyNames.FlatAppearance.BorderSize = 0;
            this.m_btnCopyNames.FlatAppearance.MouseDownBackColor = System.Drawing.SystemColors.MenuHighlight;
            this.m_btnCopyNames.FlatAppearance.MouseOverBackColor = System.Drawing.SystemColors.InactiveCaption;
            this.m_btnCopyNames.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.m_btnCopyNames.Location = new System.Drawing.Point(350, 138);
            this.m_btnCopyNames.Name = "m_btnCopyNames";
            this.m_btnCopyNames.Size = new System.Drawing.Size(111, 20);
            this.m_btnCopyNames.TabIndex = 119;
            this.m_btnCopyNames.Text = "Copy Names...";
            this.m_btnCopyNames.UseVisualStyleBackColor = true;
            this.m_btnCopyNames.Click += new System.EventHandler(this.cmdCopyBookNames);
            // 
            // m_LiterateSettings
            // 
            this.m_LiterateSettings.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.m_LiterateSettings.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.m_LiterateSettings.DontAllowPropertyGrid = false;
            this.m_LiterateSettings.Location = new System.Drawing.Point(9, 38);
            this.m_LiterateSettings.Name = "m_LiterateSettings";
            this.m_LiterateSettings.Size = new System.Drawing.Size(442, 290);
            this.m_LiterateSettings.TabIndex = 7;
            // 
            // DlgTranslationProperties
            // 
            this.AcceptButton = this.m_bOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(480, 400);
            this.Controls.Add(this.m_bOK);
            this.Controls.Add(this.m_tabctrlTranslation);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DlgTranslationProperties";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Translation Properties";
            this.Load += new System.EventHandler(this.cmdLoad);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.cmdFormClosing);
            this.m_tabctrlTranslation.ResumeLayout(false);
            this.m_tabBooks.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.m_gridBooks)).EndInit();
            this.m_tabOther.ResumeLayout(false);
            this.m_tabOther.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl m_tabctrlTranslation;
        private System.Windows.Forms.TabPage m_tabBooks;
        private System.Windows.Forms.RadioButton m_radioStartedBooks;
        private System.Windows.Forms.RadioButton m_radioOldTestament;
        private System.Windows.Forms.RadioButton m_radioNewTestament;
        private System.Windows.Forms.RadioButton m_radioAll;
        private System.Windows.Forms.DataGridView m_gridBooks;
        private System.Windows.Forms.DataGridViewTextBoxColumn m_colAbbreviation;
        private System.Windows.Forms.DataGridViewTextBoxColumn m_colBookName;
        private System.Windows.Forms.DataGridViewComboBoxColumn m_colStage;
        private System.Windows.Forms.TabPage m_tabOther;
        private OurWord.Edit.LiterateSettingsWnd m_LiterateSettings;
        private System.Windows.Forms.Label m_lblLanguageName;
        private System.Windows.Forms.TextBox m_editLanguageName;
        private System.Windows.Forms.Button m_bOK;
        private System.Windows.Forms.Button m_btnCreate;
        private System.Windows.Forms.Button m_btnImport;
        private System.Windows.Forms.Button m_btnRemove;
        private System.Windows.Forms.Button m_btnProperties;
        private System.Windows.Forms.Button m_btnEditRawFile;
        private System.Windows.Forms.Button m_btnCopyNames;
    }
}