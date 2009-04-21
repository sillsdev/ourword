namespace JWdb.DataModel
{
    partial class SynchProgressDlg
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SynchProgressDlg));
            this.m_labelInternetAccess = new System.Windows.Forms.Label();
            this.m_labelIntegrity = new System.Windows.Forms.Label();
            this.m_labelStoringRecentChanges = new System.Windows.Forms.Label();
            this.m_labelPulling = new System.Windows.Forms.Label();
            this.m_labelMerge = new System.Windows.Forms.Label();
            this.m_labelStoringMergeResults = new System.Windows.Forms.Label();
            this.m_labelPush = new System.Windows.Forms.Label();
            this.m_btnCancel = new System.Windows.Forms.Button();
            this.m_labelHeader = new System.Windows.Forms.Label();
            this.m_pictIntegrity = new System.Windows.Forms.PictureBox();
            this.m_pictInternetAccess = new System.Windows.Forms.PictureBox();
            this.m_pictStoringRecentChanges = new System.Windows.Forms.PictureBox();
            this.m_pictPulling = new System.Windows.Forms.PictureBox();
            this.m_pictMerging = new System.Windows.Forms.PictureBox();
            this.m_pictStoringMergeResults = new System.Windows.Forms.PictureBox();
            this.m_pictPush = new System.Windows.Forms.PictureBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.m_pictIntegrity)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.m_pictInternetAccess)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.m_pictStoringRecentChanges)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.m_pictPulling)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.m_pictMerging)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.m_pictStoringMergeResults)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.m_pictPush)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // m_labelInternetAccess
            // 
            this.m_labelInternetAccess.Location = new System.Drawing.Point(140, 47);
            this.m_labelInternetAccess.Name = "m_labelInternetAccess";
            this.m_labelInternetAccess.Size = new System.Drawing.Size(278, 23);
            this.m_labelInternetAccess.TabIndex = 0;
            this.m_labelInternetAccess.Text = "Checking Internet Access";
            this.m_labelInternetAccess.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // m_labelIntegrity
            // 
            this.m_labelIntegrity.Location = new System.Drawing.Point(140, 70);
            this.m_labelIntegrity.Name = "m_labelIntegrity";
            this.m_labelIntegrity.Size = new System.Drawing.Size(278, 23);
            this.m_labelIntegrity.TabIndex = 1;
            this.m_labelIntegrity.Text = "Verifying data integrity";
            this.m_labelIntegrity.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // m_labelStoringRecentChanges
            // 
            this.m_labelStoringRecentChanges.Location = new System.Drawing.Point(140, 93);
            this.m_labelStoringRecentChanges.Name = "m_labelStoringRecentChanges";
            this.m_labelStoringRecentChanges.Size = new System.Drawing.Size(278, 23);
            this.m_labelStoringRecentChanges.TabIndex = 2;
            this.m_labelStoringRecentChanges.Text = "Storing your most recent changes";
            this.m_labelStoringRecentChanges.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // m_labelPulling
            // 
            this.m_labelPulling.Location = new System.Drawing.Point(140, 116);
            this.m_labelPulling.Name = "m_labelPulling";
            this.m_labelPulling.Size = new System.Drawing.Size(278, 23);
            this.m_labelPulling.TabIndex = 3;
            this.m_labelPulling.Text = "Receiving changes from the Internet";
            this.m_labelPulling.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // m_labelMerge
            // 
            this.m_labelMerge.Location = new System.Drawing.Point(140, 139);
            this.m_labelMerge.Name = "m_labelMerge";
            this.m_labelMerge.Size = new System.Drawing.Size(278, 23);
            this.m_labelMerge.TabIndex = 4;
            this.m_labelMerge.Text = "Merging your work with theirs";
            this.m_labelMerge.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // m_labelStoringMergeResults
            // 
            this.m_labelStoringMergeResults.Location = new System.Drawing.Point(140, 162);
            this.m_labelStoringMergeResults.Name = "m_labelStoringMergeResults";
            this.m_labelStoringMergeResults.Size = new System.Drawing.Size(278, 23);
            this.m_labelStoringMergeResults.TabIndex = 5;
            this.m_labelStoringMergeResults.Text = "Storing the results of the merge";
            this.m_labelStoringMergeResults.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // m_labelPush
            // 
            this.m_labelPush.Location = new System.Drawing.Point(140, 185);
            this.m_labelPush.Name = "m_labelPush";
            this.m_labelPush.Size = new System.Drawing.Size(278, 23);
            this.m_labelPush.TabIndex = 6;
            this.m_labelPush.Text = "Sending everything back to the Internet";
            this.m_labelPush.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // m_btnCancel
            // 
            this.m_btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.m_btnCancel.Enabled = false;
            this.m_btnCancel.Location = new System.Drawing.Point(165, 227);
            this.m_btnCancel.Name = "m_btnCancel";
            this.m_btnCancel.Size = new System.Drawing.Size(103, 23);
            this.m_btnCancel.TabIndex = 7;
            this.m_btnCancel.Text = "Please Wait...";
            this.m_btnCancel.UseVisualStyleBackColor = true;
            this.m_btnCancel.Click += new System.EventHandler(this.cmdDone);
            // 
            // m_labelHeader
            // 
            this.m_labelHeader.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_labelHeader.Location = new System.Drawing.Point(12, 9);
            this.m_labelHeader.Name = "m_labelHeader";
            this.m_labelHeader.Size = new System.Drawing.Size(406, 38);
            this.m_labelHeader.TabIndex = 8;
            this.m_labelHeader.Text = "Please wait while OurWord synchronizes your data with the Internet";
            this.m_labelHeader.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // m_pictIntegrity
            // 
            this.m_pictIntegrity.Image = ((System.Drawing.Image)(resources.GetObject("m_pictIntegrity.Image")));
            this.m_pictIntegrity.Location = new System.Drawing.Point(122, 74);
            this.m_pictIntegrity.Name = "m_pictIntegrity";
            this.m_pictIntegrity.Size = new System.Drawing.Size(16, 16);
            this.m_pictIntegrity.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.m_pictIntegrity.TabIndex = 10;
            this.m_pictIntegrity.TabStop = false;
            // 
            // m_pictInternetAccess
            // 
            this.m_pictInternetAccess.Image = ((System.Drawing.Image)(resources.GetObject("m_pictInternetAccess.Image")));
            this.m_pictInternetAccess.Location = new System.Drawing.Point(122, 51);
            this.m_pictInternetAccess.Name = "m_pictInternetAccess";
            this.m_pictInternetAccess.Size = new System.Drawing.Size(16, 16);
            this.m_pictInternetAccess.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.m_pictInternetAccess.TabIndex = 9;
            this.m_pictInternetAccess.TabStop = false;
            // 
            // m_pictStoringRecentChanges
            // 
            this.m_pictStoringRecentChanges.Image = ((System.Drawing.Image)(resources.GetObject("m_pictStoringRecentChanges.Image")));
            this.m_pictStoringRecentChanges.Location = new System.Drawing.Point(122, 97);
            this.m_pictStoringRecentChanges.Name = "m_pictStoringRecentChanges";
            this.m_pictStoringRecentChanges.Size = new System.Drawing.Size(16, 16);
            this.m_pictStoringRecentChanges.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.m_pictStoringRecentChanges.TabIndex = 11;
            this.m_pictStoringRecentChanges.TabStop = false;
            // 
            // m_pictPulling
            // 
            this.m_pictPulling.Image = ((System.Drawing.Image)(resources.GetObject("m_pictPulling.Image")));
            this.m_pictPulling.Location = new System.Drawing.Point(122, 120);
            this.m_pictPulling.Name = "m_pictPulling";
            this.m_pictPulling.Size = new System.Drawing.Size(16, 16);
            this.m_pictPulling.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.m_pictPulling.TabIndex = 12;
            this.m_pictPulling.TabStop = false;
            // 
            // m_pictMerging
            // 
            this.m_pictMerging.Image = ((System.Drawing.Image)(resources.GetObject("m_pictMerging.Image")));
            this.m_pictMerging.Location = new System.Drawing.Point(122, 143);
            this.m_pictMerging.Name = "m_pictMerging";
            this.m_pictMerging.Size = new System.Drawing.Size(16, 16);
            this.m_pictMerging.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.m_pictMerging.TabIndex = 13;
            this.m_pictMerging.TabStop = false;
            // 
            // m_pictStoringMergeResults
            // 
            this.m_pictStoringMergeResults.Image = ((System.Drawing.Image)(resources.GetObject("m_pictStoringMergeResults.Image")));
            this.m_pictStoringMergeResults.Location = new System.Drawing.Point(122, 166);
            this.m_pictStoringMergeResults.Name = "m_pictStoringMergeResults";
            this.m_pictStoringMergeResults.Size = new System.Drawing.Size(16, 16);
            this.m_pictStoringMergeResults.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.m_pictStoringMergeResults.TabIndex = 14;
            this.m_pictStoringMergeResults.TabStop = false;
            // 
            // m_pictPush
            // 
            this.m_pictPush.Image = ((System.Drawing.Image)(resources.GetObject("m_pictPush.Image")));
            this.m_pictPush.Location = new System.Drawing.Point(122, 189);
            this.m_pictPush.Name = "m_pictPush";
            this.m_pictPush.Size = new System.Drawing.Size(16, 16);
            this.m_pictPush.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.m_pictPush.TabIndex = 15;
            this.m_pictPush.TabStop = false;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(13, 47);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(100, 158);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 16;
            this.pictureBox1.TabStop = false;
            // 
            // SynchProgressDlg
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.m_btnCancel;
            this.ClientSize = new System.Drawing.Size(428, 262);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.m_pictPush);
            this.Controls.Add(this.m_pictStoringMergeResults);
            this.Controls.Add(this.m_pictMerging);
            this.Controls.Add(this.m_pictPulling);
            this.Controls.Add(this.m_pictStoringRecentChanges);
            this.Controls.Add(this.m_pictIntegrity);
            this.Controls.Add(this.m_pictInternetAccess);
            this.Controls.Add(this.m_labelHeader);
            this.Controls.Add(this.m_btnCancel);
            this.Controls.Add(this.m_labelPush);
            this.Controls.Add(this.m_labelStoringMergeResults);
            this.Controls.Add(this.m_labelMerge);
            this.Controls.Add(this.m_labelPulling);
            this.Controls.Add(this.m_labelStoringRecentChanges);
            this.Controls.Add(this.m_labelIntegrity);
            this.Controls.Add(this.m_labelInternetAccess);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SynchProgressDlg";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Synchronize Progress";
            ((System.ComponentModel.ISupportInitialize)(this.m_pictIntegrity)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.m_pictInternetAccess)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.m_pictStoringRecentChanges)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.m_pictPulling)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.m_pictMerging)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.m_pictStoringMergeResults)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.m_pictPush)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label m_labelInternetAccess;
        private System.Windows.Forms.Label m_labelIntegrity;
        private System.Windows.Forms.Label m_labelStoringRecentChanges;
        private System.Windows.Forms.Label m_labelPulling;
        private System.Windows.Forms.Label m_labelMerge;
        private System.Windows.Forms.Label m_labelStoringMergeResults;
        private System.Windows.Forms.Label m_labelPush;
        private System.Windows.Forms.Button m_btnCancel;
        private System.Windows.Forms.Label m_labelHeader;
        private System.Windows.Forms.PictureBox m_pictInternetAccess;
        private System.Windows.Forms.PictureBox m_pictIntegrity;
        private System.Windows.Forms.PictureBox m_pictStoringRecentChanges;
        private System.Windows.Forms.PictureBox m_pictPulling;
        private System.Windows.Forms.PictureBox m_pictMerging;
        private System.Windows.Forms.PictureBox m_pictStoringMergeResults;
        private System.Windows.Forms.PictureBox m_pictPush;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}