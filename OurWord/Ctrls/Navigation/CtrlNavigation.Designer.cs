namespace OurWord.Ctrls.Navigation
{
    partial class CtrlNavigation
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CtrlNavigation));
            this.TopTools = new System.Windows.Forms.ToolStrip();
            this.m_Book = new System.Windows.Forms.ToolStripDropDownButton();
            this.m_Chapter = new System.Windows.Forms.ToolStripDropDownButton();
            this.m_Verse = new System.Windows.Forms.ToolStripLabel();
            this.m_Locked = new System.Windows.Forms.ToolStripButton();
            this.BottomTools = new System.Windows.Forms.ToolStrip();
            this.m_First = new System.Windows.Forms.ToolStripButton();
            this.m_Previous = new System.Windows.Forms.ToolStripSplitButton();
            this.m_Next = new System.Windows.Forms.ToolStripSplitButton();
            this.m_Last = new System.Windows.Forms.ToolStripButton();
            this.m_Separator = new System.Windows.Forms.ToolStripSeparator();
            this.m_Find = new System.Windows.Forms.ToolStripDropDownButton();
            this.m_FindNext = new System.Windows.Forms.ToolStripMenuItem();
            this.m_AdvancedFind = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.m_Filter = new System.Windows.Forms.ToolStripMenuItem();
            this.createConcordanceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.m_separatorBookmarks = new System.Windows.Forms.ToolStripSeparator();
            this.m_SetBookmark = new System.Windows.Forms.ToolStripMenuItem();
            this.m_GoToBookmark = new System.Windows.Forms.ToolStripMenuItem();
            this.FilteredNavigation = new System.Windows.Forms.ImageList(this.components);
            this.UnfilteredNavigation = new System.Windows.Forms.ImageList(this.components);
            this.m_FindAndReplace = new System.Windows.Forms.ToolStripMenuItem();
            this.TopTools.SuspendLayout();
            this.BottomTools.SuspendLayout();
            this.SuspendLayout();
            // 
            // TopTools
            // 
            this.TopTools.BackColor = System.Drawing.Color.Transparent;
            this.TopTools.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TopTools.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.TopTools.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_Book,
            this.m_Chapter,
            this.m_Verse,
            this.m_Locked});
            this.TopTools.Location = new System.Drawing.Point(0, 0);
            this.TopTools.Name = "TopTools";
            this.TopTools.Size = new System.Drawing.Size(449, 30);
            this.TopTools.TabIndex = 0;
            // 
            // m_Book
            // 
            this.m_Book.BackColor = System.Drawing.Color.Transparent;
            this.m_Book.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.m_Book.Font = new System.Drawing.Font("Segoe UI", 12.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_Book.Image = ((System.Drawing.Image)(resources.GetObject("m_Book.Image")));
            this.m_Book.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.m_Book.Name = "m_Book";
            this.m_Book.Size = new System.Drawing.Size(82, 27);
            this.m_Book.Text = "Genesis";
            // 
            // m_Chapter
            // 
            this.m_Chapter.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.m_Chapter.Font = new System.Drawing.Font("Segoe UI", 12.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_Chapter.Image = ((System.Drawing.Image)(resources.GetObject("m_Chapter.Image")));
            this.m_Chapter.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.m_Chapter.Name = "m_Chapter";
            this.m_Chapter.Size = new System.Drawing.Size(43, 27);
            this.m_Chapter.Text = "24";
            this.m_Chapter.Click += new System.EventHandler(this.cmdGoToChapter);
            // 
            // m_Verse
            // 
            this.m_Verse.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.m_Verse.Font = new System.Drawing.Font("Segoe UI", 12.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_Verse.Name = "m_Verse";
            this.m_Verse.Size = new System.Drawing.Size(47, 27);
            this.m_Verse.Text = "5-25";
            // 
            // m_Locked
            // 
            this.m_Locked.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.m_Locked.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.m_Locked.Image = ((System.Drawing.Image)(resources.GetObject("m_Locked.Image")));
            this.m_Locked.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.m_Locked.Name = "m_Locked";
            this.m_Locked.Size = new System.Drawing.Size(23, 27);
            this.m_Locked.Text = "Locked";
            this.m_Locked.ToolTipText = "This book is locked and cannot be edited.";
            // 
            // BottomTools
            // 
            this.BottomTools.BackColor = System.Drawing.SystemColors.Control;
            this.BottomTools.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.BottomTools.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_First,
            this.m_Previous,
            this.m_Next,
            this.m_Last,
            this.m_Separator,
            this.m_Find});
            this.BottomTools.Location = new System.Drawing.Point(0, 30);
            this.BottomTools.Name = "BottomTools";
            this.BottomTools.Size = new System.Drawing.Size(449, 46);
            this.BottomTools.TabIndex = 1;
            // 
            // m_First
            // 
            this.m_First.Image = ((System.Drawing.Image)(resources.GetObject("m_First.Image")));
            this.m_First.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.m_First.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.m_First.Name = "m_First";
            this.m_First.Size = new System.Drawing.Size(33, 43);
            this.m_First.Text = "First";
            this.m_First.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.m_First.ToolTipText = "Go to the First section in the book.";
            this.m_First.Click += new System.EventHandler(this.cmdGoToFirstSection);
            // 
            // m_Previous
            // 
            this.m_Previous.Image = ((System.Drawing.Image)(resources.GetObject("m_Previous.Image")));
            this.m_Previous.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.m_Previous.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.m_Previous.Name = "m_Previous";
            this.m_Previous.Size = new System.Drawing.Size(68, 43);
            this.m_Previous.Text = "Previous";
            this.m_Previous.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.m_Previous.ToolTipText = "Go to the Previous section in the book.";
            this.m_Previous.ButtonClick += new System.EventHandler(this.cmdGoToPreviousSection);
            // 
            // m_Next
            // 
            this.m_Next.Image = ((System.Drawing.Image)(resources.GetObject("m_Next.Image")));
            this.m_Next.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.m_Next.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.m_Next.Name = "m_Next";
            this.m_Next.Size = new System.Drawing.Size(47, 43);
            this.m_Next.Text = "Next";
            this.m_Next.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.m_Next.ToolTipText = "Go to the Next section in the book.";
            this.m_Next.ButtonClick += new System.EventHandler(this.cmdGoToNextSection);
            // 
            // m_Last
            // 
            this.m_Last.Image = ((System.Drawing.Image)(resources.GetObject("m_Last.Image")));
            this.m_Last.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.m_Last.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.m_Last.Name = "m_Last";
            this.m_Last.Size = new System.Drawing.Size(32, 43);
            this.m_Last.Text = "Last";
            this.m_Last.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.m_Last.ToolTipText = "Go to the final section in the book.";
            this.m_Last.Click += new System.EventHandler(this.cmdGoToLastSection);
            // 
            // m_Separator
            // 
            this.m_Separator.Name = "m_Separator";
            this.m_Separator.Size = new System.Drawing.Size(6, 46);
            // 
            // m_Find
            // 
            this.m_Find.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_FindNext,
            this.m_AdvancedFind,
            this.m_FindAndReplace,
            this.toolStripSeparator1,
            this.m_Filter,
            this.createConcordanceToolStripMenuItem,
            this.m_separatorBookmarks,
            this.m_SetBookmark,
            this.m_GoToBookmark});
            this.m_Find.Image = ((System.Drawing.Image)(resources.GetObject("m_Find.Image")));
            this.m_Find.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.m_Find.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.m_Find.Name = "m_Find";
            this.m_Find.Size = new System.Drawing.Size(43, 43);
            this.m_Find.Text = "Find";
            this.m_Find.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.m_Find.DropDownOpening += new System.EventHandler(this.cmdFindDropDownOpening);
            // 
            // m_FindNext
            // 
            this.m_FindNext.Name = "m_FindNext";
            this.m_FindNext.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F)));
            this.m_FindNext.Size = new System.Drawing.Size(191, 22);
            this.m_FindNext.Text = "Find &Next";
            this.m_FindNext.Click += new System.EventHandler(this.cmdFindNext);
            // 
            // m_AdvancedFind
            // 
            this.m_AdvancedFind.Name = "m_AdvancedFind";
            this.m_AdvancedFind.Size = new System.Drawing.Size(191, 22);
            this.m_AdvancedFind.Text = "&Advanced Find...";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(188, 6);
            // 
            // m_Filter
            // 
            this.m_Filter.Image = ((System.Drawing.Image)(resources.GetObject("m_Filter.Image")));
            this.m_Filter.Name = "m_Filter";
            this.m_Filter.Size = new System.Drawing.Size(191, 22);
            this.m_Filter.Text = "Set Fi&lter...";
            this.m_Filter.ToolTipText = "Filter out the sections which do not conform to a criteria.";
            this.m_Filter.Click += new System.EventHandler(this.cmdFilter);
            // 
            // createConcordanceToolStripMenuItem
            // 
            this.createConcordanceToolStripMenuItem.Name = "createConcordanceToolStripMenuItem";
            this.createConcordanceToolStripMenuItem.Size = new System.Drawing.Size(191, 22);
            this.createConcordanceToolStripMenuItem.Text = "Create &Concordance...";
            this.createConcordanceToolStripMenuItem.Click += new System.EventHandler(this.cmdConcordance);
            // 
            // m_separatorBookmarks
            // 
            this.m_separatorBookmarks.Name = "m_separatorBookmarks";
            this.m_separatorBookmarks.Size = new System.Drawing.Size(188, 6);
            // 
            // m_SetBookmark
            // 
            this.m_SetBookmark.Name = "m_SetBookmark";
            this.m_SetBookmark.Size = new System.Drawing.Size(191, 22);
            this.m_SetBookmark.Text = "Set &Bookmark here";
            this.m_SetBookmark.Click += new System.EventHandler(this.cmdSetBookmark);
            // 
            // m_GoToBookmark
            // 
            this.m_GoToBookmark.Name = "m_GoToBookmark";
            this.m_GoToBookmark.Size = new System.Drawing.Size(191, 22);
            this.m_GoToBookmark.Text = "Go To Boo&kmark";
            // 
            // FilteredNavigation
            // 
            this.FilteredNavigation.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("FilteredNavigation.ImageStream")));
            this.FilteredNavigation.TransparentColor = System.Drawing.Color.Transparent;
            this.FilteredNavigation.Images.SetKeyName(0, "GoFirstFiltered.ico");
            this.FilteredNavigation.Images.SetKeyName(1, "GoPreviousFiltered.ico");
            this.FilteredNavigation.Images.SetKeyName(2, "GoNextFiltered.ico");
            this.FilteredNavigation.Images.SetKeyName(3, "GoLastFiltered.ico");
            // 
            // UnfilteredNavigation
            // 
            this.UnfilteredNavigation.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("UnfilteredNavigation.ImageStream")));
            this.UnfilteredNavigation.TransparentColor = System.Drawing.Color.Transparent;
            this.UnfilteredNavigation.Images.SetKeyName(0, "GoFirst.ico");
            this.UnfilteredNavigation.Images.SetKeyName(1, "GoPrevious.ico");
            this.UnfilteredNavigation.Images.SetKeyName(2, "GoNext.ico");
            this.UnfilteredNavigation.Images.SetKeyName(3, "GoLast.ico");
            // 
            // m_FindAndReplace
            // 
            this.m_FindAndReplace.Name = "m_FindAndReplace";
            this.m_FindAndReplace.Size = new System.Drawing.Size(191, 22);
            this.m_FindAndReplace.Text = "Find and &Replace...";
            this.m_FindAndReplace.Click += new System.EventHandler(this.cmdFindAndReplace);
            // 
            // CtrlNavigation
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.DarkGray;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.BottomTools);
            this.Controls.Add(this.TopTools);
            this.Name = "CtrlNavigation";
            this.Size = new System.Drawing.Size(449, 88);
            this.Load += new System.EventHandler(this.cmdLoad);
            this.TopTools.ResumeLayout(false);
            this.TopTools.PerformLayout();
            this.BottomTools.ResumeLayout(false);
            this.BottomTools.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip TopTools;
        private System.Windows.Forms.ToolStrip BottomTools;
        private System.Windows.Forms.ToolStripButton m_First;
        private System.Windows.Forms.ToolStripButton m_Last;
        private System.Windows.Forms.ImageList FilteredNavigation;
        private System.Windows.Forms.ImageList UnfilteredNavigation;
        private System.Windows.Forms.ToolStripSeparator m_Separator;
        private System.Windows.Forms.ToolStripDropDownButton m_Chapter;
        private System.Windows.Forms.ToolStripLabel m_Verse;
        private System.Windows.Forms.ToolStripDropDownButton m_Find;
        private System.Windows.Forms.ToolStripMenuItem m_AdvancedFind;
        private System.Windows.Forms.ToolStripMenuItem m_Filter;
        private System.Windows.Forms.ToolStripMenuItem m_FindNext;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem createConcordanceToolStripMenuItem;
        private System.Windows.Forms.ToolStripSplitButton m_Previous;
        private System.Windows.Forms.ToolStripSplitButton m_Next;
        private System.Windows.Forms.ToolStripButton m_Locked;
        public System.Windows.Forms.ToolStripDropDownButton m_Book;
        private System.Windows.Forms.ToolStripSeparator m_separatorBookmarks;
        private System.Windows.Forms.ToolStripMenuItem m_SetBookmark;
        private System.Windows.Forms.ToolStripMenuItem m_GoToBookmark;
        private System.Windows.Forms.ToolStripMenuItem m_FindAndReplace;
    }
}
