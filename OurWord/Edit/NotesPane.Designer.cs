namespace OurWord.Edit
{
    partial class NotesPane
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NotesPane));
            this.m_toolstripNotes = new System.Windows.Forms.ToolStrip();
            this.m_btnInsert = new System.Windows.Forms.ToolStripButton();
            this.m_btnDeleteNote = new System.Windows.Forms.ToolStripButton();
            this.m_toolstripNotes.SuspendLayout();
            this.SuspendLayout();
            // 
            // m_toolstripNotes
            // 
            this.m_toolstripNotes.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.m_toolstripNotes.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_btnInsert,
            this.m_btnDeleteNote});
            this.m_toolstripNotes.Location = new System.Drawing.Point(0, 0);
            this.m_toolstripNotes.Name = "m_toolstripNotes";
            this.m_toolstripNotes.Size = new System.Drawing.Size(224, 38);
            this.m_toolstripNotes.Stretch = true;
            this.m_toolstripNotes.TabIndex = 0;
            this.m_toolstripNotes.Text = "toolstripNotes";
            // 
            // m_btnInsert
            // 
            this.m_btnInsert.Image = ((System.Drawing.Image)(resources.GetObject("m_btnInsert.Image")));
            this.m_btnInsert.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.m_btnInsert.Name = "m_btnInsert";
            this.m_btnInsert.Size = new System.Drawing.Size(40, 35);
            this.m_btnInsert.Text = "Insert";
            this.m_btnInsert.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.m_btnInsert.Click += new System.EventHandler(this.cmdInsertNote);
            // 
            // m_btnDeleteNote
            // 
            this.m_btnDeleteNote.Image = ((System.Drawing.Image)(resources.GetObject("m_btnDeleteNote.Image")));
            this.m_btnDeleteNote.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.m_btnDeleteNote.Name = "m_btnDeleteNote";
            this.m_btnDeleteNote.Size = new System.Drawing.Size(53, 35);
            this.m_btnDeleteNote.Text = "Delete...";
            this.m_btnDeleteNote.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.m_btnDeleteNote.Click += new System.EventHandler(this.cmdDeleteNote);
            // 
            // NotesPane
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.m_toolstripNotes);
            this.Name = "NotesPane";
            this.Size = new System.Drawing.Size(224, 470);
            this.Load += new System.EventHandler(this.cmdLoad);
            this.m_toolstripNotes.ResumeLayout(false);
            this.m_toolstripNotes.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip m_toolstripNotes;
        private System.Windows.Forms.ToolStripButton m_btnDeleteNote;
        private System.Windows.Forms.ToolStripButton m_btnInsert;
    }
}
