namespace OurWord.Edit
{
    partial class MergePane
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MergePane));
            this.m_toolstripMerge = new System.Windows.Forms.ToolStrip();
            this.m_btnSetupMerge = new System.Windows.Forms.ToolStripButton();
            this.m_btnNextIssue = new System.Windows.Forms.ToolStripButton();
            this.m_Position = new System.Windows.Forms.ToolStripLabel();
            this.m_btnDiffs = new System.Windows.Forms.ToolStripButton();
            this.m_btnBrowse = new System.Windows.Forms.Button();
            this.m_btnFinished = new System.Windows.Forms.Button();
            this.m_toolstripMerge.SuspendLayout();
            this.SuspendLayout();
            // 
            // m_toolstripMerge
            // 
            this.m_toolstripMerge.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.m_toolstripMerge.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_btnSetupMerge,
            this.m_btnNextIssue,
            this.m_Position,
            this.m_btnDiffs});
            this.m_toolstripMerge.Location = new System.Drawing.Point(0, 0);
            this.m_toolstripMerge.Name = "m_toolstripMerge";
            this.m_toolstripMerge.Size = new System.Drawing.Size(224, 38);
            this.m_toolstripMerge.Stretch = true;
            this.m_toolstripMerge.TabIndex = 0;
            this.m_toolstripMerge.Text = "toolstripMerge";
            // 
            // m_btnSetupMerge
            // 
            this.m_btnSetupMerge.AutoSize = false;
            this.m_btnSetupMerge.CheckOnClick = true;
            this.m_btnSetupMerge.Image = global::OurWord.Properties.Resources.MergeConfigure;
            this.m_btnSetupMerge.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.m_btnSetupMerge.Name = "m_btnSetupMerge";
            this.m_btnSetupMerge.Size = new System.Drawing.Size(54, 35);
            this.m_btnSetupMerge.Text = "Setup";
            this.m_btnSetupMerge.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.m_btnSetupMerge.ToolTipText = "Setup the files to be merged";
            this.m_btnSetupMerge.Click += new System.EventHandler(this.cmdToggleSetupMode);
            // 
            // m_btnNextIssue
            // 
            this.m_btnNextIssue.Image = global::OurWord.Properties.Resources.MergNextDiff;
            this.m_btnNextIssue.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.m_btnNextIssue.Name = "m_btnNextIssue";
            this.m_btnNextIssue.Size = new System.Drawing.Size(35, 35);
            this.m_btnNextIssue.Text = "Next";
            this.m_btnNextIssue.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.m_btnNextIssue.ToolTipText = " Move to the Next Issue";
            // 
            // m_Position
            // 
            this.m_Position.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.m_Position.Name = "m_Position";
            this.m_Position.Size = new System.Drawing.Size(71, 35);
            this.m_Position.Text = "Issue 1 of 78";
            this.m_Position.ToolTipText = "Which issue, our of how many, you are currently displaying.";
            // 
            // m_btnDiffs
            // 
            this.m_btnDiffs.CheckOnClick = true;
            this.m_btnDiffs.Image = ((System.Drawing.Image)(resources.GetObject("m_btnDiffs.Image")));
            this.m_btnDiffs.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.m_btnDiffs.Name = "m_btnDiffs";
            this.m_btnDiffs.Size = new System.Drawing.Size(43, 35);
            this.m_btnDiffs.Text = "Diff\'s?";
            this.m_btnDiffs.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.m_btnDiffs.ToolTipText = "Show the differences (insertions and deletions) between the two versions of the t" +
                "ext?";
            this.m_btnDiffs.Click += new System.EventHandler(this.cmdToggleDiffs);
            // 
            // m_btnBrowse
            // 
            this.m_btnBrowse.Location = new System.Drawing.Point(12, 52);
            this.m_btnBrowse.Name = "m_btnBrowse";
            this.m_btnBrowse.Size = new System.Drawing.Size(139, 23);
            this.m_btnBrowse.TabIndex = 1;
            this.m_btnBrowse.Text = "Add a file to merge...";
            this.m_btnBrowse.UseVisualStyleBackColor = true;
            this.m_btnBrowse.Click += new System.EventHandler(this.cmdAddMergeFile);
            // 
            // m_btnFinished
            // 
            this.m_btnFinished.Location = new System.Drawing.Point(12, 81);
            this.m_btnFinished.Name = "m_btnFinished";
            this.m_btnFinished.Size = new System.Drawing.Size(139, 23);
            this.m_btnFinished.TabIndex = 2;
            this.m_btnFinished.Text = "Return to Display";
            this.m_btnFinished.UseVisualStyleBackColor = true;
            this.m_btnFinished.Click += new System.EventHandler(this.cmdLeaveSetupMode);
            // 
            // MergePane
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.m_btnFinished);
            this.Controls.Add(this.m_btnBrowse);
            this.Controls.Add(this.m_toolstripMerge);
            this.Name = "MergePane";
            this.Size = new System.Drawing.Size(224, 470);
            this.m_toolstripMerge.ResumeLayout(false);
            this.m_toolstripMerge.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip m_toolstripMerge;
        private System.Windows.Forms.ToolStripButton m_btnSetupMerge;
        private System.Windows.Forms.ToolStripButton m_btnNextIssue;
        private System.Windows.Forms.Button m_btnBrowse;
        private System.Windows.Forms.Button m_btnFinished;
        private System.Windows.Forms.ToolStripLabel m_Position;
        private System.Windows.Forms.ToolStripButton m_btnDiffs;


    }
}
