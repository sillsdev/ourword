namespace OurWord.Utilities
{
    partial class ClusterLocation
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ClusterLocation));
            this.m_radioMyDocuments = new System.Windows.Forms.RadioButton();
            this.m_labelMyDocs = new System.Windows.Forms.Label();
            this.m_radioAppData = new System.Windows.Forms.RadioButton();
            this.m_labelAppData = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // m_radioMyDocuments
            // 
            this.m_radioMyDocuments.AutoSize = true;
            this.m_radioMyDocuments.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_radioMyDocuments.Location = new System.Drawing.Point(3, 3);
            this.m_radioMyDocuments.Name = "m_radioMyDocuments";
            this.m_radioMyDocuments.Size = new System.Drawing.Size(96, 17);
            this.m_radioMyDocuments.TabIndex = 6;
            this.m_radioMyDocuments.TabStop = true;
            this.m_radioMyDocuments.Text = "My Documents";
            this.m_radioMyDocuments.UseVisualStyleBackColor = true;
            // 
            // m_labelMyDocs
            // 
            this.m_labelMyDocs.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.m_labelMyDocs.Location = new System.Drawing.Point(24, 23);
            this.m_labelMyDocs.Name = "m_labelMyDocs";
            this.m_labelMyDocs.Size = new System.Drawing.Size(268, 52);
            this.m_labelMyDocs.TabIndex = 8;
            this.m_labelMyDocs.Text = "Place the cluster in your Documents folder if you want it to be readily visible a" +
                "nd available.";
            // 
            // m_radioAppData
            // 
            this.m_radioAppData.AutoSize = true;
            this.m_radioAppData.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_radioAppData.Location = new System.Drawing.Point(3, 78);
            this.m_radioAppData.Name = "m_radioAppData";
            this.m_radioAppData.Size = new System.Drawing.Size(120, 17);
            this.m_radioAppData.TabIndex = 9;
            this.m_radioAppData.TabStop = true;
            this.m_radioAppData.Text = "My Application Data";
            this.m_radioAppData.UseVisualStyleBackColor = true;
            // 
            // m_labelAppData
            // 
            this.m_labelAppData.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.m_labelAppData.Location = new System.Drawing.Point(24, 98);
            this.m_labelAppData.Name = "m_labelAppData";
            this.m_labelAppData.Size = new System.Drawing.Size(268, 95);
            this.m_labelAppData.TabIndex = 10;
            this.m_labelAppData.Text = resources.GetString("m_labelAppData.Text");
            // 
            // ClusterLocation
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.m_labelAppData);
            this.Controls.Add(this.m_radioAppData);
            this.Controls.Add(this.m_labelMyDocs);
            this.Controls.Add(this.m_radioMyDocuments);
            this.Name = "ClusterLocation";
            this.Size = new System.Drawing.Size(295, 193);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RadioButton m_radioMyDocuments;
        private System.Windows.Forms.Label m_labelMyDocs;
        private System.Windows.Forms.RadioButton m_radioAppData;
        private System.Windows.Forms.Label m_labelAppData;
    }
}
