namespace OurWord.Utilities
{
    partial class ClusterListView
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
            this.m_lvClusters = new System.Windows.Forms.ListView();
            this.SuspendLayout();
            // 
            // m_lvClusters
            // 
            this.m_lvClusters.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_lvClusters.FullRowSelect = true;
            this.m_lvClusters.HideSelection = false;
            this.m_lvClusters.LabelEdit = true;
            this.m_lvClusters.Location = new System.Drawing.Point(0, 0);
            this.m_lvClusters.MultiSelect = false;
            this.m_lvClusters.Name = "m_lvClusters";
            this.m_lvClusters.ShowItemToolTips = true;
            this.m_lvClusters.Size = new System.Drawing.Size(299, 256);
            this.m_lvClusters.TabIndex = 39;
            this.m_lvClusters.UseCompatibleStateImageBehavior = false;
            this.m_lvClusters.View = System.Windows.Forms.View.List;
            this.m_lvClusters.AfterLabelEdit += new System.Windows.Forms.LabelEditEventHandler(this.cmdAfterLabelEdit);
            this.m_lvClusters.SelectedIndexChanged += new System.EventHandler(this.cmdSelectedIndexChanged);
            // 
            // ClusterListView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.m_lvClusters);
            this.Name = "ClusterListView";
            this.Size = new System.Drawing.Size(299, 256);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView m_lvClusters;
    }
}
