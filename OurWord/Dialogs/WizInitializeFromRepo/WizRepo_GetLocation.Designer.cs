namespace OurWord.Dialogs
{
    partial class WizRepo_GetLocation
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
            this.m_labelInstruction = new System.Windows.Forms.Label();
            this.m_ClusterLocation = new OurWord.Utilities.ClusterLocation();
            this.SuspendLayout();
            // 
            // m_labelInstruction
            // 
            this.m_labelInstruction.Location = new System.Drawing.Point(13, 10);
            this.m_labelInstruction.Name = "m_labelInstruction";
            this.m_labelInstruction.Size = new System.Drawing.Size(344, 23);
            this.m_labelInstruction.TabIndex = 9;
            this.m_labelInstruction.Text = "Choose the location for this cluster on your computer:";
            this.m_labelInstruction.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // m_ClusterLocation
            // 
            this.m_ClusterLocation.IsInAppData = false;
            this.m_ClusterLocation.IsInMyDocuments = false;
            this.m_ClusterLocation.Location = new System.Drawing.Point(16, 36);
            this.m_ClusterLocation.Name = "m_ClusterLocation";
            this.m_ClusterLocation.Size = new System.Drawing.Size(341, 193);
            this.m_ClusterLocation.TabIndex = 10;
            // 
            // WizRepo_GetLocation
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.m_ClusterLocation);
            this.Controls.Add(this.m_labelInstruction);
            this.Name = "WizRepo_GetLocation";
            this.Size = new System.Drawing.Size(372, 306);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label m_labelInstruction;
        private OurWord.Utilities.ClusterLocation m_ClusterLocation;
    }
}
