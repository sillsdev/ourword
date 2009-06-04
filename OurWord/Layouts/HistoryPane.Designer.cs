namespace OurWord.Layouts
{
    partial class HistoryPane
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HistoryPane));
            this.m_toolstripHistory = new System.Windows.Forms.ToolStrip();
            this.m_bAddEvent = new System.Windows.Forms.ToolStripButton();
            this.m_bDelete = new System.Windows.Forms.ToolStripButton();
            this.m_toolstripHistory.SuspendLayout();
            this.SuspendLayout();
            // 
            // m_toolstripHistory
            // 
            this.m_toolstripHistory.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.m_toolstripHistory.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_bAddEvent,
            this.m_bDelete});
            this.m_toolstripHistory.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.Flow;
            this.m_toolstripHistory.Location = new System.Drawing.Point(0, 0);
            this.m_toolstripHistory.Name = "m_toolstripHistory";
            this.m_toolstripHistory.Size = new System.Drawing.Size(189, 38);
            this.m_toolstripHistory.Stretch = true;
            this.m_toolstripHistory.TabIndex = 0;
            this.m_toolstripHistory.Text = "toolstripHistory";
            // 
            // m_bAddEvent
            // 
            this.m_bAddEvent.Image = ((System.Drawing.Image)(resources.GetObject("m_bAddEvent.Image")));
            this.m_bAddEvent.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.m_bAddEvent.Name = "m_bAddEvent";
            this.m_bAddEvent.Size = new System.Drawing.Size(33, 35);
            this.m_bAddEvent.Text = "Add";
            this.m_bAddEvent.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.m_bAddEvent.ToolTipText = "Append an event to the history.";
            this.m_bAddEvent.Click += new System.EventHandler(this.cmdAddEvent);
            // 
            // m_bDelete
            // 
            this.m_bDelete.Image = ((System.Drawing.Image)(resources.GetObject("m_bDelete.Image")));
            this.m_bDelete.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.m_bDelete.Name = "m_bDelete";
            this.m_bDelete.Size = new System.Drawing.Size(53, 35);
            this.m_bDelete.Text = "Delete...";
            this.m_bDelete.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.m_bDelete.ToolTipText = "Delete this event from the history.";
            this.m_bDelete.Click += new System.EventHandler(this.cmdDeleteEvent);
            // 
            // HistoryPane
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Controls.Add(this.m_toolstripHistory);
            this.Name = "HistoryPane";
            this.Size = new System.Drawing.Size(189, 433);
            this.Load += new System.EventHandler(this.cmdLoad);
            this.m_toolstripHistory.ResumeLayout(false);
            this.m_toolstripHistory.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip m_toolstripHistory;
        private System.Windows.Forms.ToolStripButton m_bAddEvent;
        private System.Windows.Forms.ToolStripButton m_bDelete;
    }
}
