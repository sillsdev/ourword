namespace OurWord.Dialogs
{
    partial class WizRepo_GetClusterName
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
            this.m_descrClusterName = new System.Windows.Forms.Label();
            this.m_textClusterName = new System.Windows.Forms.TextBox();
            this.m_labelClusterName = new System.Windows.Forms.Label();
            this.m_labelErrorMsg = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // m_descrClusterName
            // 
            this.m_descrClusterName.Location = new System.Drawing.Point(33, 36);
            this.m_descrClusterName.Name = "m_descrClusterName";
            this.m_descrClusterName.Size = new System.Drawing.Size(327, 55);
            this.m_descrClusterName.TabIndex = 5;
            this.m_descrClusterName.Text = "Enter the name of the cluster, e.g.,  \"Timor\" or \"Southern Languages.\" (You can r" +
                "ename it later in the Configuration Dialog if you wish.)";
            // 
            // m_textClusterName
            // 
            this.m_textClusterName.Location = new System.Drawing.Point(119, 13);
            this.m_textClusterName.Name = "m_textClusterName";
            this.m_textClusterName.Size = new System.Drawing.Size(241, 20);
            this.m_textClusterName.TabIndex = 4;
            this.m_textClusterName.TextChanged += new System.EventHandler(this.cmdTextChanged);
            // 
            // m_labelClusterName
            // 
            this.m_labelClusterName.AutoSize = true;
            this.m_labelClusterName.Location = new System.Drawing.Point(15, 16);
            this.m_labelClusterName.Name = "m_labelClusterName";
            this.m_labelClusterName.Size = new System.Drawing.Size(73, 13);
            this.m_labelClusterName.TabIndex = 3;
            this.m_labelClusterName.Text = "Cluster Name:";
            // 
            // m_labelErrorMsg
            // 
            this.m_labelErrorMsg.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_labelErrorMsg.ForeColor = System.Drawing.Color.Red;
            this.m_labelErrorMsg.Location = new System.Drawing.Point(15, 269);
            this.m_labelErrorMsg.Name = "m_labelErrorMsg";
            this.m_labelErrorMsg.Size = new System.Drawing.Size(345, 23);
            this.m_labelErrorMsg.TabIndex = 6;
            // 
            // WizRepo_GetClusterName
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.m_labelErrorMsg);
            this.Controls.Add(this.m_descrClusterName);
            this.Controls.Add(this.m_textClusterName);
            this.Controls.Add(this.m_labelClusterName);
            this.Name = "WizRepo_GetClusterName";
            this.Size = new System.Drawing.Size(372, 306);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label m_descrClusterName;
        private System.Windows.Forms.TextBox m_textClusterName;
        private System.Windows.Forms.Label m_labelClusterName;
        private System.Windows.Forms.Label m_labelErrorMsg;
    }
}
