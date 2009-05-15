namespace OurWord.Dialogs.WizNewProject
{
    partial class WizNew_Cluster
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
            this.m_lblExplanation = new System.Windows.Forms.Label();
            this.m_lblChoose = new System.Windows.Forms.Label();
            this.m_lblMoreInfo = new System.Windows.Forms.Label();
            this.m_ClusterListView = new OurWord.Utilities.ClusterListView();
            this.SuspendLayout();
            // 
            // m_lblExplanation
            // 
            this.m_lblExplanation.Location = new System.Drawing.Point(14, 178);
            this.m_lblExplanation.Name = "m_lblExplanation";
            this.m_lblExplanation.Size = new System.Drawing.Size(344, 64);
            this.m_lblExplanation.TabIndex = 0;
            this.m_lblExplanation.Text = "OurWord organizes language projects within Clusters. This allows certain settings" +
                ", such as the StyleSheet, to be used by all projects within a cluster. ";
            // 
            // m_lblChoose
            // 
            this.m_lblChoose.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_lblChoose.Location = new System.Drawing.Point(14, 10);
            this.m_lblChoose.Name = "m_lblChoose";
            this.m_lblChoose.Size = new System.Drawing.Size(344, 33);
            this.m_lblChoose.TabIndex = 8;
            this.m_lblChoose.Text = "Choose the Cluster this new Language Project will be a part of:";
            this.m_lblChoose.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // m_lblMoreInfo
            // 
            this.m_lblMoreInfo.Location = new System.Drawing.Point(13, 242);
            this.m_lblMoreInfo.Name = "m_lblMoreInfo";
            this.m_lblMoreInfo.Size = new System.Drawing.Size(344, 51);
            this.m_lblMoreInfo.TabIndex = 11;
            this.m_lblMoreInfo.Text = "(Note: In order to rename a cluster or add an additional cluster, first exit this" +
                " wizard and then use the Tools-Configuration Dialog.)";
            // 
            // m_ClusterListView
            // 
            this.m_ClusterListView.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.m_ClusterListView.Location = new System.Drawing.Point(16, 50);
            this.m_ClusterListView.Name = "m_ClusterListView";
            this.m_ClusterListView.SelectedCluster = null;
            this.m_ClusterListView.Size = new System.Drawing.Size(341, 125);
            this.m_ClusterListView.TabIndex = 12;
            // 
            // WizNew_Cluster
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.m_ClusterListView);
            this.Controls.Add(this.m_lblMoreInfo);
            this.Controls.Add(this.m_lblChoose);
            this.Controls.Add(this.m_lblExplanation);
            this.Name = "WizNew_Cluster";
            this.Size = new System.Drawing.Size(372, 306);
            this.ResumeLayout(false);

        }

        #endregion

		private System.Windows.Forms.Label m_lblExplanation;
		private System.Windows.Forms.Label m_lblChoose;
        private System.Windows.Forms.Label m_lblMoreInfo;
        private OurWord.Utilities.ClusterListView m_ClusterListView;
    }
}
