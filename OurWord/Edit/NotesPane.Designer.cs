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
            this.m_btnInsert = new System.Windows.Forms.ToolStripDropDownButton();
            this.m_menuInsertGeneral = new System.Windows.Forms.ToolStripMenuItem();
            this.m_menuInsertToDo = new System.Windows.Forms.ToolStripMenuItem();
            this.m_menuInsertAskUNS = new System.Windows.Forms.ToolStripMenuItem();
            this.m_menuInsertDefinition = new System.Windows.Forms.ToolStripMenuItem();
            this.m_menuInsertOldVersion = new System.Windows.Forms.ToolStripMenuItem();
            this.m_menuInsertReason = new System.Windows.Forms.ToolStripMenuItem();
            this.m_menuInsertFrontIssue = new System.Windows.Forms.ToolStripMenuItem();
            this.m_menuInsertHintForDaughter = new System.Windows.Forms.ToolStripMenuItem();
            this.m_menuInsertBT = new System.Windows.Forms.ToolStripMenuItem();
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
            this.m_btnInsert.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_menuInsertGeneral,
            this.m_menuInsertToDo,
            this.m_menuInsertAskUNS,
            this.m_menuInsertDefinition,
            this.m_menuInsertOldVersion,
            this.m_menuInsertReason,
            this.m_menuInsertFrontIssue,
            this.m_menuInsertHintForDaughter,
            this.m_menuInsertBT});
            this.m_btnInsert.Image = ((System.Drawing.Image)(resources.GetObject("m_btnInsert.Image")));
            this.m_btnInsert.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.m_btnInsert.Name = "m_btnInsert";
            this.m_btnInsert.Size = new System.Drawing.Size(49, 35);
            this.m_btnInsert.Text = "Insert";
            this.m_btnInsert.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            // 
            // m_menuInsertGeneral
            // 
            this.m_menuInsertGeneral.Image = ((System.Drawing.Image)(resources.GetObject("m_menuInsertGeneral.Image")));
            this.m_menuInsertGeneral.Name = "m_menuInsertGeneral";
            this.m_menuInsertGeneral.Size = new System.Drawing.Size(228, 22);
            this.m_menuInsertGeneral.Tag = "kGeneral";
            this.m_menuInsertGeneral.Text = "Insert General &Note";
            this.m_menuInsertGeneral.Click += new System.EventHandler(this.cmdInsertNote);
            // 
            // m_menuInsertToDo
            // 
            this.m_menuInsertToDo.Image = ((System.Drawing.Image)(resources.GetObject("m_menuInsertToDo.Image")));
            this.m_menuInsertToDo.Name = "m_menuInsertToDo";
            this.m_menuInsertToDo.Size = new System.Drawing.Size(228, 22);
            this.m_menuInsertToDo.Tag = "kToDo";
            this.m_menuInsertToDo.Text = "Insert &To Do Note";
            this.m_menuInsertToDo.Click += new System.EventHandler(this.cmdInsertNote);
            // 
            // m_menuInsertAskUNS
            // 
            this.m_menuInsertAskUNS.Image = ((System.Drawing.Image)(resources.GetObject("m_menuInsertAskUNS.Image")));
            this.m_menuInsertAskUNS.Name = "m_menuInsertAskUNS";
            this.m_menuInsertAskUNS.Size = new System.Drawing.Size(228, 22);
            this.m_menuInsertAskUNS.Tag = "kAskUns";
            this.m_menuInsertAskUNS.Text = "Insert &Ask UNS Note";
            this.m_menuInsertAskUNS.Click += new System.EventHandler(this.cmdInsertNote);
            // 
            // m_menuInsertDefinition
            // 
            this.m_menuInsertDefinition.Image = ((System.Drawing.Image)(resources.GetObject("m_menuInsertDefinition.Image")));
            this.m_menuInsertDefinition.Name = "m_menuInsertDefinition";
            this.m_menuInsertDefinition.Size = new System.Drawing.Size(228, 22);
            this.m_menuInsertDefinition.Tag = "kDefinition";
            this.m_menuInsertDefinition.Text = "Insert &Definition Note";
            this.m_menuInsertDefinition.Click += new System.EventHandler(this.cmdInsertNote);
            // 
            // m_menuInsertOldVersion
            // 
            this.m_menuInsertOldVersion.Image = ((System.Drawing.Image)(resources.GetObject("m_menuInsertOldVersion.Image")));
            this.m_menuInsertOldVersion.Name = "m_menuInsertOldVersion";
            this.m_menuInsertOldVersion.Size = new System.Drawing.Size(228, 22);
            this.m_menuInsertOldVersion.Tag = "kOldVersion";
            this.m_menuInsertOldVersion.Text = "Insert Old &Version Note";
            this.m_menuInsertOldVersion.Click += new System.EventHandler(this.cmdInsertNote);
            // 
            // m_menuInsertReason
            // 
            this.m_menuInsertReason.Image = ((System.Drawing.Image)(resources.GetObject("m_menuInsertReason.Image")));
            this.m_menuInsertReason.Name = "m_menuInsertReason";
            this.m_menuInsertReason.Size = new System.Drawing.Size(228, 22);
            this.m_menuInsertReason.Tag = "kReason";
            this.m_menuInsertReason.Text = "Insert &Reason Note";
            this.m_menuInsertReason.Click += new System.EventHandler(this.cmdInsertNote);
            // 
            // m_menuInsertFrontIssue
            // 
            this.m_menuInsertFrontIssue.Image = ((System.Drawing.Image)(resources.GetObject("m_menuInsertFrontIssue.Image")));
            this.m_menuInsertFrontIssue.Name = "m_menuInsertFrontIssue";
            this.m_menuInsertFrontIssue.Size = new System.Drawing.Size(228, 22);
            this.m_menuInsertFrontIssue.Tag = "kFront";
            this.m_menuInsertFrontIssue.Text = "Insert &Front Issue Note";
            this.m_menuInsertFrontIssue.Click += new System.EventHandler(this.cmdInsertNote);
            // 
            // m_menuInsertHintForDaughter
            // 
            this.m_menuInsertHintForDaughter.Image = ((System.Drawing.Image)(resources.GetObject("m_menuInsertHintForDaughter.Image")));
            this.m_menuInsertHintForDaughter.Name = "m_menuInsertHintForDaughter";
            this.m_menuInsertHintForDaughter.Size = new System.Drawing.Size(228, 22);
            this.m_menuInsertHintForDaughter.Tag = "kHintForDaughter";
            this.m_menuInsertHintForDaughter.Text = "Insert &Hint for Daughter Note";
            this.m_menuInsertHintForDaughter.Click += new System.EventHandler(this.cmdInsertNote);
            // 
            // m_menuInsertBT
            // 
            this.m_menuInsertBT.Image = ((System.Drawing.Image)(resources.GetObject("m_menuInsertBT.Image")));
            this.m_menuInsertBT.Name = "m_menuInsertBT";
            this.m_menuInsertBT.Size = new System.Drawing.Size(228, 22);
            this.m_menuInsertBT.Tag = "kBT";
            this.m_menuInsertBT.Text = "Insert &Back Translation Note";
            this.m_menuInsertBT.Click += new System.EventHandler(this.cmdInsertNote);
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
            this.m_toolstripNotes.ResumeLayout(false);
            this.m_toolstripNotes.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip m_toolstripNotes;
        private System.Windows.Forms.ToolStripButton m_btnDeleteNote;
        private System.Windows.Forms.ToolStripDropDownButton m_btnInsert;
        private System.Windows.Forms.ToolStripMenuItem m_menuInsertGeneral;
        private System.Windows.Forms.ToolStripMenuItem m_menuInsertToDo;
        private System.Windows.Forms.ToolStripMenuItem m_menuInsertAskUNS;
        private System.Windows.Forms.ToolStripMenuItem m_menuInsertDefinition;
        private System.Windows.Forms.ToolStripMenuItem m_menuInsertOldVersion;
        private System.Windows.Forms.ToolStripMenuItem m_menuInsertReason;
        private System.Windows.Forms.ToolStripMenuItem m_menuInsertFrontIssue;
        private System.Windows.Forms.ToolStripMenuItem m_menuInsertHintForDaughter;
        private System.Windows.Forms.ToolStripMenuItem m_menuInsertBT;
    }
}
