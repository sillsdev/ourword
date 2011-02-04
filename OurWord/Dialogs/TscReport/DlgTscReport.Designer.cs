namespace OurWord.Dialogs.TscReport
{
    partial class DlgTscReport
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
            this.m_lYourName = new System.Windows.Forms.Label();
            this.m_tYourName = new System.Windows.Forms.TextBox();
            this.m_lPeriod = new System.Windows.Forms.Label();
            this.m_cPeriod = new System.Windows.Forms.ComboBox();
            this.m_btnGenerate = new System.Windows.Forms.Button();
            this.m_btnClose = new System.Windows.Forms.Button();
            this.m_lSheduledCompletion = new System.Windows.Forms.Label();
            this.m_cScheduledCompletion = new System.Windows.Forms.ComboBox();
            this.m_grid = new System.Windows.Forms.DataGridView();
            this.m_cBook = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.m_cStage = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.m_cCurrentQuarter = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.m_cNextQuarter = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.m_lGoals = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.m_grid)).BeginInit();
            this.SuspendLayout();
            // 
            // m_lYourName
            // 
            this.m_lYourName.AutoSize = true;
            this.m_lYourName.Location = new System.Drawing.Point(12, 9);
            this.m_lYourName.Name = "m_lYourName";
            this.m_lYourName.Size = new System.Drawing.Size(63, 13);
            this.m_lYourName.TabIndex = 0;
            this.m_lYourName.Text = "Your Name:";
            // 
            // m_tYourName
            // 
            this.m_tYourName.Location = new System.Drawing.Point(81, 6);
            this.m_tYourName.Name = "m_tYourName";
            this.m_tYourName.Size = new System.Drawing.Size(148, 20);
            this.m_tYourName.TabIndex = 1;
            // 
            // m_lPeriod
            // 
            this.m_lPeriod.AutoSize = true;
            this.m_lPeriod.Location = new System.Drawing.Point(249, 9);
            this.m_lPeriod.Name = "m_lPeriod";
            this.m_lPeriod.Size = new System.Drawing.Size(40, 13);
            this.m_lPeriod.TabIndex = 2;
            this.m_lPeriod.Text = "Period:";
            // 
            // m_cPeriod
            // 
            this.m_cPeriod.FormattingEnabled = true;
            this.m_cPeriod.Location = new System.Drawing.Point(295, 6);
            this.m_cPeriod.Name = "m_cPeriod";
            this.m_cPeriod.Size = new System.Drawing.Size(121, 21);
            this.m_cPeriod.TabIndex = 3;
            this.m_cPeriod.SelectedIndexChanged += new System.EventHandler(this.cmdPeriodChanged);
            // 
            // m_btnGenerate
            // 
            this.m_btnGenerate.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.m_btnGenerate.Location = new System.Drawing.Point(243, 356);
            this.m_btnGenerate.Name = "m_btnGenerate";
            this.m_btnGenerate.Size = new System.Drawing.Size(75, 23);
            this.m_btnGenerate.TabIndex = 4;
            this.m_btnGenerate.Text = "Generate";
            this.m_btnGenerate.UseVisualStyleBackColor = true;
            this.m_btnGenerate.Click += new System.EventHandler(this.cmdGenerate);
            // 
            // m_btnClose
            // 
            this.m_btnClose.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.m_btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.m_btnClose.Location = new System.Drawing.Point(324, 356);
            this.m_btnClose.Name = "m_btnClose";
            this.m_btnClose.Size = new System.Drawing.Size(75, 23);
            this.m_btnClose.TabIndex = 5;
            this.m_btnClose.Text = "Close";
            this.m_btnClose.UseVisualStyleBackColor = true;
            // 
            // m_lSheduledCompletion
            // 
            this.m_lSheduledCompletion.AutoSize = true;
            this.m_lSheduledCompletion.Location = new System.Drawing.Point(457, 8);
            this.m_lSheduledCompletion.Name = "m_lSheduledCompletion";
            this.m_lSheduledCompletion.Size = new System.Drawing.Size(116, 13);
            this.m_lSheduledCompletion.TabIndex = 6;
            this.m_lSheduledCompletion.Text = "Scheduled Completion:";
            // 
            // m_cScheduledCompletion
            // 
            this.m_cScheduledCompletion.FormattingEnabled = true;
            this.m_cScheduledCompletion.Location = new System.Drawing.Point(579, 5);
            this.m_cScheduledCompletion.Name = "m_cScheduledCompletion";
            this.m_cScheduledCompletion.Size = new System.Drawing.Size(52, 21);
            this.m_cScheduledCompletion.TabIndex = 7;
            // 
            // m_grid
            // 
            this.m_grid.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.m_grid.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.m_grid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.m_grid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.m_grid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.m_grid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.m_cBook,
            this.m_cStage,
            this.m_cCurrentQuarter,
            this.m_cNextQuarter});
            this.m_grid.Location = new System.Drawing.Point(12, 59);
            this.m_grid.MultiSelect = false;
            this.m_grid.Name = "m_grid";
            this.m_grid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.m_grid.ShowEditingIcon = false;
            this.m_grid.Size = new System.Drawing.Size(619, 278);
            this.m_grid.TabIndex = 8;
            this.m_grid.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.cmdCellValueChanged);
            this.m_grid.DefaultValuesNeeded += new System.Windows.Forms.DataGridViewRowEventHandler(this.cmdDefaultValuesNeeded);
            // 
            // m_cBook
            // 
            this.m_cBook.DisplayStyle = System.Windows.Forms.DataGridViewComboBoxDisplayStyle.ComboBox;
            this.m_cBook.HeaderText = "Book";
            this.m_cBook.MaxDropDownItems = 20;
            this.m_cBook.Name = "m_cBook";
            this.m_cBook.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.m_cBook.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // m_cStage
            // 
            this.m_cStage.DisplayStyle = System.Windows.Forms.DataGridViewComboBoxDisplayStyle.ComboBox;
            this.m_cStage.HeaderText = "Goal";
            this.m_cStage.Name = "m_cStage";
            // 
            // m_cCurrentQuarter
            // 
            this.m_cCurrentQuarter.DisplayStyle = System.Windows.Forms.DataGridViewComboBoxDisplayStyle.ComboBox;
            this.m_cCurrentQuarter.HeaderText = "Done This Quarter";
            this.m_cCurrentQuarter.Name = "m_cCurrentQuarter";
            // 
            // m_cNextQuarter
            // 
            this.m_cNextQuarter.DisplayStyle = System.Windows.Forms.DataGridViewComboBoxDisplayStyle.ComboBox;
            this.m_cNextQuarter.HeaderText = "Planned Next Quarter?";
            this.m_cNextQuarter.Name = "m_cNextQuarter";
            // 
            // m_lGoals
            // 
            this.m_lGoals.AutoSize = true;
            this.m_lGoals.Location = new System.Drawing.Point(12, 41);
            this.m_lGoals.Name = "m_lGoals";
            this.m_lGoals.Size = new System.Drawing.Size(241, 13);
            this.m_lGoals.TabIndex = 9;
            this.m_lGoals.Text = "Enter Project Goals Here for this and next quarter:";
            // 
            // DlgTscReport
            // 
            this.AcceptButton = this.m_btnClose;
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.m_btnClose;
            this.ClientSize = new System.Drawing.Size(643, 389);
            this.Controls.Add(this.m_lGoals);
            this.Controls.Add(this.m_grid);
            this.Controls.Add(this.m_cScheduledCompletion);
            this.Controls.Add(this.m_lSheduledCompletion);
            this.Controls.Add(this.m_btnClose);
            this.Controls.Add(this.m_btnGenerate);
            this.Controls.Add(this.m_cPeriod);
            this.Controls.Add(this.m_lPeriod);
            this.Controls.Add(this.m_tYourName);
            this.Controls.Add(this.m_lYourName);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DlgTscReport";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Quarterly Report for The Seed Company";
            this.Load += new System.EventHandler(this.cmdLoad);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.cmdFormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.m_grid)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label m_lYourName;
        private System.Windows.Forms.TextBox m_tYourName;
        private System.Windows.Forms.Label m_lPeriod;
        private System.Windows.Forms.ComboBox m_cPeriod;
        private System.Windows.Forms.Button m_btnGenerate;
        private System.Windows.Forms.Button m_btnClose;
        private System.Windows.Forms.Label m_lSheduledCompletion;
        private System.Windows.Forms.ComboBox m_cScheduledCompletion;
        private System.Windows.Forms.DataGridView m_grid;
        private System.Windows.Forms.Label m_lGoals;
        private System.Windows.Forms.DataGridViewComboBoxColumn m_cBook;
        private System.Windows.Forms.DataGridViewComboBoxColumn m_cStage;
        private System.Windows.Forms.DataGridViewComboBoxColumn m_cCurrentQuarter;
        private System.Windows.Forms.DataGridViewComboBoxColumn m_cNextQuarter;
    }
}