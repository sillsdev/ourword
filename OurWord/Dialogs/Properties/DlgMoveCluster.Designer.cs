namespace OurWord.Dialogs
{
    partial class DlgMoveCluster
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.m_groupLocation = new System.Windows.Forms.GroupBox();
            this.m_btnCancel = new System.Windows.Forms.Button();
            this.m_btnOK = new System.Windows.Forms.Button();
            this.m_ClusterLocation = new OurWord.Utilities.ClusterLocation();
            this.m_groupLocation.SuspendLayout();
            this.SuspendLayout();
            // 
            // m_groupLocation
            // 
            this.m_groupLocation.Controls.Add(this.m_ClusterLocation);
            this.m_groupLocation.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_groupLocation.Location = new System.Drawing.Point(12, 12);
            this.m_groupLocation.Name = "m_groupLocation";
            this.m_groupLocation.Size = new System.Drawing.Size(432, 198);
            this.m_groupLocation.TabIndex = 7;
            this.m_groupLocation.TabStop = false;
            this.m_groupLocation.Text = "Where do you want the cluster to be located?";
            // 
            // m_btnCancel
            // 
            this.m_btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.m_btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.m_btnCancel.Location = new System.Drawing.Point(229, 240);
            this.m_btnCancel.Name = "m_btnCancel";
            this.m_btnCancel.Size = new System.Drawing.Size(75, 23);
            this.m_btnCancel.TabIndex = 9;
            this.m_btnCancel.Text = "Cancel";
            // 
            // m_btnOK
            // 
            this.m_btnOK.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.m_btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.m_btnOK.Location = new System.Drawing.Point(141, 240);
            this.m_btnOK.Name = "m_btnOK";
            this.m_btnOK.Size = new System.Drawing.Size(75, 23);
            this.m_btnOK.TabIndex = 8;
            this.m_btnOK.Text = "OK";
            // 
            // m_ClusterLocation
            // 
            this.m_ClusterLocation.IsInAppData = false;
            this.m_ClusterLocation.IsInMyDocuments = false;
            this.m_ClusterLocation.Location = new System.Drawing.Point(15, 19);
            this.m_ClusterLocation.Name = "m_ClusterLocation";
            this.m_ClusterLocation.Size = new System.Drawing.Size(411, 170);
            this.m_ClusterLocation.TabIndex = 0;
            // 
            // DlgMoveCluster
            // 
            this.AcceptButton = this.m_btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.m_btnCancel;
            this.ClientSize = new System.Drawing.Size(456, 275);
            this.Controls.Add(this.m_btnCancel);
            this.Controls.Add(this.m_btnOK);
            this.Controls.Add(this.m_groupLocation);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DlgMoveCluster";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Move Cluster";
            this.m_groupLocation.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox m_groupLocation;
        private System.Windows.Forms.Button m_btnCancel;
        private System.Windows.Forms.Button m_btnOK;
        private OurWord.Utilities.ClusterLocation m_ClusterLocation;
    }
}