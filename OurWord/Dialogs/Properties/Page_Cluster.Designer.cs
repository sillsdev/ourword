namespace OurWord.Dialogs
{
    partial class Page_Cluster
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Page_Cluster));
            this.m_labelDescription = new System.Windows.Forms.Label();
            this.m_btnCreateNewCluster = new System.Windows.Forms.Button();
            this.m_btnRenameCluster = new System.Windows.Forms.Button();
            this.m_btnDeleteCluster = new System.Windows.Forms.Button();
            this.m_lblChoose = new System.Windows.Forms.Label();
            this.m_ClusterListView = new OurWord.Utilities.ClusterListView();
            this.m_btnMoveCluster = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // m_labelDescription
            // 
            this.m_labelDescription.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.m_labelDescription.Location = new System.Drawing.Point(3, 0);
            this.m_labelDescription.Name = "m_labelDescription";
            this.m_labelDescription.Size = new System.Drawing.Size(462, 72);
            this.m_labelDescription.TabIndex = 35;
            this.m_labelDescription.Text = resources.GetString("m_labelDescription.Text");
            // 
            // m_btnCreateNewCluster
            // 
            this.m_btnCreateNewCluster.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.m_btnCreateNewCluster.Location = new System.Drawing.Point(325, 100);
            this.m_btnCreateNewCluster.Name = "m_btnCreateNewCluster";
            this.m_btnCreateNewCluster.Size = new System.Drawing.Size(130, 23);
            this.m_btnCreateNewCluster.TabIndex = 8;
            this.m_btnCreateNewCluster.Text = "Create New Cluster...";
            this.m_btnCreateNewCluster.Click += new System.EventHandler(this.cmdCreateNewCluster);
            // 
            // m_btnRenameCluster
            // 
            this.m_btnRenameCluster.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.m_btnRenameCluster.Location = new System.Drawing.Point(325, 129);
            this.m_btnRenameCluster.Name = "m_btnRenameCluster";
            this.m_btnRenameCluster.Size = new System.Drawing.Size(130, 23);
            this.m_btnRenameCluster.TabIndex = 36;
            this.m_btnRenameCluster.Text = "Rename Cluster";
            this.m_btnRenameCluster.Click += new System.EventHandler(this.cmdRenameCluster);
            // 
            // m_btnDeleteCluster
            // 
            this.m_btnDeleteCluster.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.m_btnDeleteCluster.Location = new System.Drawing.Point(325, 158);
            this.m_btnDeleteCluster.Name = "m_btnDeleteCluster";
            this.m_btnDeleteCluster.Size = new System.Drawing.Size(130, 23);
            this.m_btnDeleteCluster.TabIndex = 37;
            this.m_btnDeleteCluster.Text = "Delete Cluster...";
            this.m_btnDeleteCluster.Click += new System.EventHandler(this.cmdDeleteCluster);
            // 
            // m_lblChoose
            // 
            this.m_lblChoose.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_lblChoose.Location = new System.Drawing.Point(3, 72);
            this.m_lblChoose.Name = "m_lblChoose";
            this.m_lblChoose.Size = new System.Drawing.Size(452, 25);
            this.m_lblChoose.TabIndex = 39;
            this.m_lblChoose.Text = "Clusters currently on this computer:";
            this.m_lblChoose.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // m_ClusterListView
            // 
            this.m_ClusterListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.m_ClusterListView.Location = new System.Drawing.Point(6, 100);
            this.m_ClusterListView.Name = "m_ClusterListView";
            this.m_ClusterListView.SelectedCluster = null;
            this.m_ClusterListView.Size = new System.Drawing.Size(313, 265);
            this.m_ClusterListView.TabIndex = 40;
            // 
            // m_btnMoveCluster
            // 
            this.m_btnMoveCluster.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.m_btnMoveCluster.Location = new System.Drawing.Point(325, 187);
            this.m_btnMoveCluster.Name = "m_btnMoveCluster";
            this.m_btnMoveCluster.Size = new System.Drawing.Size(130, 23);
            this.m_btnMoveCluster.TabIndex = 41;
            this.m_btnMoveCluster.Text = "Move Cluster...";
            this.m_btnMoveCluster.Click += new System.EventHandler(this.cmdMoveCluster);
            // 
            // Page_Cluster
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.m_btnMoveCluster);
            this.Controls.Add(this.m_ClusterListView);
            this.Controls.Add(this.m_lblChoose);
            this.Controls.Add(this.m_btnDeleteCluster);
            this.Controls.Add(this.m_btnRenameCluster);
            this.Controls.Add(this.m_btnCreateNewCluster);
            this.Controls.Add(this.m_labelDescription);
            this.Name = "Page_Cluster";
            this.Size = new System.Drawing.Size(468, 368);
            this.Load += new System.EventHandler(this.cmdLoad);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label m_labelDescription;
        private System.Windows.Forms.Button m_btnCreateNewCluster;
        private System.Windows.Forms.Button m_btnRenameCluster;
        private System.Windows.Forms.Button m_btnDeleteCluster;
        private System.Windows.Forms.Label m_lblChoose;
        private OurWord.Utilities.ClusterListView m_ClusterListView;
        private System.Windows.Forms.Button m_btnMoveCluster;
    }
}
