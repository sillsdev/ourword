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
            this.BottomTools = new System.Windows.Forms.ToolStrip();
            this.m_First = new System.Windows.Forms.ToolStripButton();
            this.m_Last = new System.Windows.Forms.ToolStripButton();
            this.m_Separator = new System.Windows.Forms.ToolStripSeparator();
            this.m_Filter = new System.Windows.Forms.ToolStripButton();
            this.FilteredNavigation = new System.Windows.Forms.ImageList(this.components);
            this.UnfilteredNavigation = new System.Windows.Forms.ImageList(this.components);
            this.m_Previous = new System.Windows.Forms.ToolStripDropDownButton();
            this.m_Next = new System.Windows.Forms.ToolStripDropDownButton();
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
            this.m_Verse});
            this.TopTools.Location = new System.Drawing.Point(0, 0);
            this.TopTools.Name = "TopTools";
            this.TopTools.Size = new System.Drawing.Size(259, 32);
            this.TopTools.TabIndex = 0;
            // 
            // m_Book
            // 
            this.m_Book.BackColor = System.Drawing.Color.Transparent;
            this.m_Book.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.m_Book.Image = ((System.Drawing.Image)(resources.GetObject("m_Book.Image")));
            this.m_Book.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.m_Book.Name = "m_Book";
            this.m_Book.Size = new System.Drawing.Size(92, 29);
            this.m_Book.Text = "Genesis";
            // 
            // m_Chapter
            // 
            this.m_Chapter.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.m_Chapter.Image = ((System.Drawing.Image)(resources.GetObject("m_Chapter.Image")));
            this.m_Chapter.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.m_Chapter.Name = "m_Chapter";
            this.m_Chapter.Size = new System.Drawing.Size(47, 29);
            this.m_Chapter.Text = "24";
            this.m_Chapter.Click += new System.EventHandler(this.cmdGoToChapter);
            // 
            // m_Verse
            // 
            this.m_Verse.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.m_Verse.Name = "m_Verse";
            this.m_Verse.Size = new System.Drawing.Size(53, 29);
            this.m_Verse.Text = "5-25";
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
            this.m_Filter});
            this.BottomTools.Location = new System.Drawing.Point(0, 32);
            this.BottomTools.Name = "BottomTools";
            this.BottomTools.Size = new System.Drawing.Size(259, 46);
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
            this.m_First.Click += new System.EventHandler(this.cmdGoToFirstSection);
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
            this.m_Last.Click += new System.EventHandler(this.cmdGoToLastSection);
            // 
            // m_Separator
            // 
            this.m_Separator.Name = "m_Separator";
            this.m_Separator.Size = new System.Drawing.Size(6, 46);
            // 
            // m_Filter
            // 
            this.m_Filter.Image = ((System.Drawing.Image)(resources.GetObject("m_Filter.Image")));
            this.m_Filter.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.m_Filter.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.m_Filter.Name = "m_Filter";
            this.m_Filter.Size = new System.Drawing.Size(37, 43);
            this.m_Filter.Text = "Filter";
            this.m_Filter.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.m_Filter.Click += new System.EventHandler(this.cmdFilter);
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
            // m_Previous
            // 
            this.m_Previous.Image = ((System.Drawing.Image)(resources.GetObject("m_Previous.Image")));
            this.m_Previous.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.m_Previous.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.m_Previous.Name = "m_Previous";
            this.m_Previous.Size = new System.Drawing.Size(65, 43);
            this.m_Previous.Text = "Previous";
            this.m_Previous.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.m_Previous.Click += new System.EventHandler(this.cmdGoToPreviousSection);
            // 
            // m_Next
            // 
            this.m_Next.Image = ((System.Drawing.Image)(resources.GetObject("m_Next.Image")));
            this.m_Next.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.m_Next.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.m_Next.Name = "m_Next";
            this.m_Next.Size = new System.Drawing.Size(44, 43);
            this.m_Next.Text = "Next";
            this.m_Next.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
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
            this.Size = new System.Drawing.Size(259, 88);
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
        private System.Windows.Forms.ToolStripDropDownButton m_Book;
        private System.Windows.Forms.ToolStripButton m_First;
        private System.Windows.Forms.ToolStripButton m_Last;
        private System.Windows.Forms.ImageList FilteredNavigation;
        private System.Windows.Forms.ImageList UnfilteredNavigation;
        private System.Windows.Forms.ToolStripSeparator m_Separator;
        private System.Windows.Forms.ToolStripButton m_Filter;
        private System.Windows.Forms.ToolStripDropDownButton m_Chapter;
        private System.Windows.Forms.ToolStripLabel m_Verse;
        private System.Windows.Forms.ToolStripDropDownButton m_Previous;
        private System.Windows.Forms.ToolStripDropDownButton m_Next;
    }
}
