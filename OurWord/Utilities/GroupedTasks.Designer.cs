namespace OurWord.Utilities
{
    partial class GroupedTasks
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GroupedTasks));
            this.m_pGroup = new System.Windows.Forms.Panel();
            this.m_labelGroupName = new System.Windows.Forms.Label();
            this.m_btnCollapse = new System.Windows.Forms.Button();
            this.m_pGroup.SuspendLayout();
            this.SuspendLayout();
            // 
            // m_pGroup
            // 
            this.m_pGroup.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.m_pGroup.BackColor = System.Drawing.Color.SlateBlue;
            this.m_pGroup.Controls.Add(this.m_labelGroupName);
            this.m_pGroup.Controls.Add(this.m_btnCollapse);
            this.m_pGroup.ForeColor = System.Drawing.Color.White;
            this.m_pGroup.Location = new System.Drawing.Point(0, 0);
            this.m_pGroup.Name = "m_pGroup";
            this.m_pGroup.Size = new System.Drawing.Size(133, 21);
            this.m_pGroup.TabIndex = 0;
            // 
            // m_labelGroupName
            // 
            this.m_labelGroupName.AutoSize = true;
            this.m_labelGroupName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_labelGroupName.Location = new System.Drawing.Point(3, 3);
            this.m_labelGroupName.Name = "m_labelGroupName";
            this.m_labelGroupName.Size = new System.Drawing.Size(77, 13);
            this.m_labelGroupName.TabIndex = 1;
            this.m_labelGroupName.Text = "Group Name";
            this.m_labelGroupName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // m_btnCollapse
            // 
            this.m_btnCollapse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.m_btnCollapse.BackColor = System.Drawing.Color.SlateBlue;
            this.m_btnCollapse.FlatAppearance.BorderSize = 0;
            this.m_btnCollapse.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.m_btnCollapse.Image = ((System.Drawing.Image)(resources.GetObject("m_btnCollapse.Image")));
            this.m_btnCollapse.Location = new System.Drawing.Point(110, 3);
            this.m_btnCollapse.Name = "m_btnCollapse";
            this.m_btnCollapse.Size = new System.Drawing.Size(20, 16);
            this.m_btnCollapse.TabIndex = 1;
            this.m_btnCollapse.TabStop = false;
            this.m_btnCollapse.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.m_btnCollapse.UseVisualStyleBackColor = false;
            this.m_btnCollapse.Click += new System.EventHandler(this.cmdExpandCollapseToggle);
            // 
            // GroupedTasks
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.m_pGroup);
            this.Name = "GroupedTasks";
            this.Size = new System.Drawing.Size(133, 93);
            this.Resize += new System.EventHandler(this.cmdLayout);
            this.m_pGroup.ResumeLayout(false);
            this.m_pGroup.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel m_pGroup;
        private System.Windows.Forms.Button m_btnCollapse;
        private System.Windows.Forms.Label m_labelGroupName;
    }
}
