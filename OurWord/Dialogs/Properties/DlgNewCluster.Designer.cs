namespace OurWord.Dialogs
{
    partial class DlgNewCluster
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DlgNewCluster));
            this.m_labelName = new System.Windows.Forms.Label();
            this.m_textName = new System.Windows.Forms.TextBox();
            this.m_groupName = new System.Windows.Forms.GroupBox();
            this.m_labelError = new System.Windows.Forms.Label();
            this.m_groupLocation = new System.Windows.Forms.GroupBox();
            this.m_btnCancel = new System.Windows.Forms.Button();
            this.m_btnOK = new System.Windows.Forms.Button();
            this.m_ClusterLocation = new OurWord.Utilities.ClusterLocation();
            this.m_groupName.SuspendLayout();
            this.m_groupLocation.SuspendLayout();
            this.SuspendLayout();
            // 
            // m_labelName
            // 
            this.m_labelName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_labelName.Location = new System.Drawing.Point(50, 42);
            this.m_labelName.Name = "m_labelName";
            this.m_labelName.Size = new System.Drawing.Size(376, 62);
            this.m_labelName.TabIndex = 0;
            this.m_labelName.Text = resources.GetString("m_labelName.Text");
            // 
            // m_textName
            // 
            this.m_textName.Location = new System.Drawing.Point(16, 19);
            this.m_textName.Name = "m_textName";
            this.m_textName.Size = new System.Drawing.Size(120, 20);
            this.m_textName.TabIndex = 1;
            // 
            // m_groupName
            // 
            this.m_groupName.Controls.Add(this.m_labelError);
            this.m_groupName.Controls.Add(this.m_textName);
            this.m_groupName.Controls.Add(this.m_labelName);
            this.m_groupName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_groupName.Location = new System.Drawing.Point(12, 12);
            this.m_groupName.Name = "m_groupName";
            this.m_groupName.Size = new System.Drawing.Size(432, 109);
            this.m_groupName.TabIndex = 2;
            this.m_groupName.TabStop = false;
            this.m_groupName.Text = "Cluster Name";
            // 
            // m_labelError
            // 
            this.m_labelError.AutoSize = true;
            this.m_labelError.ForeColor = System.Drawing.Color.Red;
            this.m_labelError.Location = new System.Drawing.Point(13, 74);
            this.m_labelError.Name = "m_labelError";
            this.m_labelError.Size = new System.Drawing.Size(0, 13);
            this.m_labelError.TabIndex = 2;
            // 
            // m_groupLocation
            // 
            this.m_groupLocation.Controls.Add(this.m_ClusterLocation);
            this.m_groupLocation.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_groupLocation.Location = new System.Drawing.Point(12, 142);
            this.m_groupLocation.Name = "m_groupLocation";
            this.m_groupLocation.Size = new System.Drawing.Size(432, 197);
            this.m_groupLocation.TabIndex = 7;
            this.m_groupLocation.TabStop = false;
            this.m_groupLocation.Text = "Folder Location";
            // 
            // m_btnCancel
            // 
            this.m_btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.m_btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.m_btnCancel.Location = new System.Drawing.Point(229, 373);
            this.m_btnCancel.Name = "m_btnCancel";
            this.m_btnCancel.Size = new System.Drawing.Size(75, 23);
            this.m_btnCancel.TabIndex = 9;
            this.m_btnCancel.Text = "Cancel";
            // 
            // m_btnOK
            // 
            this.m_btnOK.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.m_btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.m_btnOK.Location = new System.Drawing.Point(141, 373);
            this.m_btnOK.Name = "m_btnOK";
            this.m_btnOK.Size = new System.Drawing.Size(75, 23);
            this.m_btnOK.TabIndex = 8;
            this.m_btnOK.Text = "OK";
            // 
            // m_ClusterLocation
            // 
            this.m_ClusterLocation.IsInAppData = false;
            this.m_ClusterLocation.IsInMyDocuments = false;
            this.m_ClusterLocation.Location = new System.Drawing.Point(16, 19);
            this.m_ClusterLocation.Name = "m_ClusterLocation";
            this.m_ClusterLocation.Size = new System.Drawing.Size(410, 176);
            this.m_ClusterLocation.TabIndex = 0;
            // 
            // DlgNewCluster
            // 
            this.AcceptButton = this.m_btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.m_btnCancel;
            this.ClientSize = new System.Drawing.Size(456, 408);
            this.Controls.Add(this.m_btnCancel);
            this.Controls.Add(this.m_btnOK);
            this.Controls.Add(this.m_groupName);
            this.Controls.Add(this.m_groupLocation);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DlgNewCluster";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Create New Cluster";
            this.Load += new System.EventHandler(this.cmdLoad);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.cmdFormClosing);
            this.m_groupName.ResumeLayout(false);
            this.m_groupName.PerformLayout();
            this.m_groupLocation.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label m_labelName;
        private System.Windows.Forms.TextBox m_textName;
        private System.Windows.Forms.GroupBox m_groupName;
        private System.Windows.Forms.GroupBox m_groupLocation;
        private System.Windows.Forms.Button m_btnCancel;
        private System.Windows.Forms.Button m_btnOK;
        private System.Windows.Forms.Label m_labelError;
        private OurWord.Utilities.ClusterLocation m_ClusterLocation;
    }
}