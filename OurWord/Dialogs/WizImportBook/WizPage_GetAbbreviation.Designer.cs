namespace OurWord.Dialogs.WizImportBook
{
    partial class WizPage_GetAbbreviation
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
            this.m_labelWhichBook = new System.Windows.Forms.Label();
            this.m_lvBooks = new System.Windows.Forms.ListView();
            this.m_lvcAbbrev = new System.Windows.Forms.ColumnHeader();
            this.m_lvcName = new System.Windows.Forms.ColumnHeader();
            this.m_labelExplanation = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // m_labelWhichBook
            // 
            this.m_labelWhichBook.Location = new System.Drawing.Point(13, 10);
            this.m_labelWhichBook.Name = "m_labelWhichBook";
            this.m_labelWhichBook.Size = new System.Drawing.Size(344, 33);
            this.m_labelWhichBook.TabIndex = 0;
            this.m_labelWhichBook.Text = "Which book are you importing?";
            this.m_labelWhichBook.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // m_lvBooks
            // 
            this.m_lvBooks.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.m_lvcAbbrev,
            this.m_lvcName});
            this.m_lvBooks.FullRowSelect = true;
            this.m_lvBooks.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.m_lvBooks.HideSelection = false;
            this.m_lvBooks.Location = new System.Drawing.Point(16, 46);
            this.m_lvBooks.MultiSelect = false;
            this.m_lvBooks.Name = "m_lvBooks";
            this.m_lvBooks.Size = new System.Drawing.Size(341, 199);
            this.m_lvBooks.TabIndex = 1;
            this.m_lvBooks.UseCompatibleStateImageBehavior = false;
            this.m_lvBooks.View = System.Windows.Forms.View.Details;
            this.m_lvBooks.SelectedIndexChanged += new System.EventHandler(this.cmdBookSelected);
            // 
            // m_lvcAbbrev
            // 
            this.m_lvcAbbrev.Text = "Abbreviation";
            this.m_lvcAbbrev.Width = 80;
            // 
            // m_lvcName
            // 
            this.m_lvcName.Text = "Book Name";
            this.m_lvcName.Width = 230;
            // 
            // m_labelExplanation
            // 
            this.m_labelExplanation.Location = new System.Drawing.Point(13, 248);
            this.m_labelExplanation.Name = "m_labelExplanation";
            this.m_labelExplanation.Size = new System.Drawing.Size(344, 44);
            this.m_labelExplanation.TabIndex = 2;
            this.m_labelExplanation.Text = "Note: Only those books which are currently possible are shown in this list.";
            // 
            // WizPage_GetAbbreviation
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.m_labelExplanation);
            this.Controls.Add(this.m_lvBooks);
            this.Controls.Add(this.m_labelWhichBook);
            this.Name = "WizPage_GetAbbreviation";
            this.Size = new System.Drawing.Size(372, 306);
            this.Load += new System.EventHandler(this.cmdLoad);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label m_labelWhichBook;
        private System.Windows.Forms.ListView m_lvBooks;
        private System.Windows.Forms.ColumnHeader m_lvcAbbrev;
        private System.Windows.Forms.ColumnHeader m_lvcName;
        private System.Windows.Forms.Label m_labelExplanation;
    }
}
