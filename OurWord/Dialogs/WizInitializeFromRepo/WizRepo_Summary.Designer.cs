namespace OurWord.Dialogs
{
    partial class WizRepo_Summary
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
            this.m_labelReady = new System.Windows.Forms.Label();
            this.m_labelMoreInfo = new System.Windows.Forms.Label();
            this.m_labelClickToStart = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // m_labelReady
            // 
            this.m_labelReady.Location = new System.Drawing.Point(12, 10);
            this.m_labelReady.Name = "m_labelReady";
            this.m_labelReady.Size = new System.Drawing.Size(346, 23);
            this.m_labelReady.TabIndex = 0;
            this.m_labelReady.Text = "OurWord is ready to download the respository to your computer.";
            // 
            // m_labelMoreInfo
            // 
            this.m_labelMoreInfo.Location = new System.Drawing.Point(12, 49);
            this.m_labelMoreInfo.Name = "m_labelMoreInfo";
            this.m_labelMoreInfo.Size = new System.Drawing.Size(346, 59);
            this.m_labelMoreInfo.TabIndex = 1;
            this.m_labelMoreInfo.Text = "This can take several minutes, depending on the speed of your Internet connection" +
                ". If the process fails, you will have the opportunity to review your settings an" +
                "d try again.";
            // 
            // m_labelClickToStart
            // 
            this.m_labelClickToStart.Location = new System.Drawing.Point(12, 272);
            this.m_labelClickToStart.Name = "m_labelClickToStart";
            this.m_labelClickToStart.Size = new System.Drawing.Size(346, 23);
            this.m_labelClickToStart.TabIndex = 2;
            this.m_labelClickToStart.Text = "Click on \"Finish\" to begin the process.";
            // 
            // WizRepo_Summary
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.m_labelClickToStart);
            this.Controls.Add(this.m_labelMoreInfo);
            this.Controls.Add(this.m_labelReady);
            this.Name = "WizRepo_Summary";
            this.Size = new System.Drawing.Size(372, 306);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label m_labelReady;
        private System.Windows.Forms.Label m_labelMoreInfo;
        private System.Windows.Forms.Label m_labelClickToStart;
    }
}
