namespace OurWord.Dialogs.WizNewProject
{
    partial class WizNew_Summary
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
			this.m_labelClickFinish = new System.Windows.Forms.Label();
			this.m_lblLanguageName = new System.Windows.Forms.Label();
			this.m_textLanguageName = new System.Windows.Forms.Label();
			this.m_lblSourceTranslation = new System.Windows.Forms.Label();
			this.m_textSourceTranslation = new System.Windows.Forms.Label();
			this.m_textCluster = new System.Windows.Forms.Label();
			this.m_lblCluster = new System.Windows.Forms.Label();
			this.m_checkLaunchProperties = new System.Windows.Forms.CheckBox();
			this.SuspendLayout();
			// 
			// m_labelReady
			// 
			this.m_labelReady.Location = new System.Drawing.Point(12, 10);
			this.m_labelReady.Name = "m_labelReady";
			this.m_labelReady.Size = new System.Drawing.Size(341, 23);
			this.m_labelReady.TabIndex = 2;
			this.m_labelReady.Text = "OurWord is now ready to create the new project:";
			this.m_labelReady.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// m_labelClickFinish
			// 
			this.m_labelClickFinish.Location = new System.Drawing.Point(12, 190);
			this.m_labelClickFinish.Name = "m_labelClickFinish";
			this.m_labelClickFinish.Size = new System.Drawing.Size(341, 38);
			this.m_labelClickFinish.TabIndex = 3;
			this.m_labelClickFinish.Text = "Click the Finish button to create the new project, or press the Previous button t" +
				"o make changes..";
			// 
			// m_lblLanguageName
			// 
			this.m_lblLanguageName.Location = new System.Drawing.Point(12, 33);
			this.m_lblLanguageName.Name = "m_lblLanguageName";
			this.m_lblLanguageName.Size = new System.Drawing.Size(108, 23);
			this.m_lblLanguageName.TabIndex = 4;
			this.m_lblLanguageName.Text = "Language Name:";
			this.m_lblLanguageName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// m_textLanguageName
			// 
			this.m_textLanguageName.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.m_textLanguageName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.m_textLanguageName.Location = new System.Drawing.Point(126, 33);
			this.m_textLanguageName.Name = "m_textLanguageName";
			this.m_textLanguageName.Size = new System.Drawing.Size(227, 23);
			this.m_textLanguageName.TabIndex = 5;
			this.m_textLanguageName.Text = "(Language Name)";
			this.m_textLanguageName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// m_lblSourceTranslation
			// 
			this.m_lblSourceTranslation.Location = new System.Drawing.Point(12, 62);
			this.m_lblSourceTranslation.Name = "m_lblSourceTranslation";
			this.m_lblSourceTranslation.Size = new System.Drawing.Size(108, 23);
			this.m_lblSourceTranslation.TabIndex = 10;
			this.m_lblSourceTranslation.Text = "Source Translation:";
			this.m_lblSourceTranslation.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// m_textSourceTranslation
			// 
			this.m_textSourceTranslation.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.m_textSourceTranslation.Location = new System.Drawing.Point(126, 62);
			this.m_textSourceTranslation.Name = "m_textSourceTranslation";
			this.m_textSourceTranslation.Size = new System.Drawing.Size(227, 23);
			this.m_textSourceTranslation.TabIndex = 11;
			this.m_textSourceTranslation.Text = "(Source Translation)";
			this.m_textSourceTranslation.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// m_textCluster
			// 
			this.m_textCluster.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.m_textCluster.Location = new System.Drawing.Point(126, 92);
			this.m_textCluster.Name = "m_textCluster";
			this.m_textCluster.Size = new System.Drawing.Size(227, 23);
			this.m_textCluster.TabIndex = 13;
			this.m_textCluster.Text = "(Cluster)";
			this.m_textCluster.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// m_lblCluster
			// 
			this.m_lblCluster.Location = new System.Drawing.Point(12, 92);
			this.m_lblCluster.Name = "m_lblCluster";
			this.m_lblCluster.Size = new System.Drawing.Size(108, 23);
			this.m_lblCluster.TabIndex = 12;
			this.m_lblCluster.Text = "Cluster:";
			this.m_lblCluster.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// m_checkLaunchProperties
			// 
			this.m_checkLaunchProperties.Location = new System.Drawing.Point(15, 135);
			this.m_checkLaunchProperties.Name = "m_checkLaunchProperties";
			this.m_checkLaunchProperties.Size = new System.Drawing.Size(338, 52);
			this.m_checkLaunchProperties.TabIndex = 14;
			this.m_checkLaunchProperties.Text = "Do you want to go ahead and launch the Configuration Dialog to enter other settin" +
				"gs (such as adding Books to the translations)?";
			this.m_checkLaunchProperties.UseVisualStyleBackColor = true;
			// 
			// WizNew_Summary
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.m_checkLaunchProperties);
			this.Controls.Add(this.m_textCluster);
			this.Controls.Add(this.m_lblCluster);
			this.Controls.Add(this.m_textSourceTranslation);
			this.Controls.Add(this.m_lblSourceTranslation);
			this.Controls.Add(this.m_textLanguageName);
			this.Controls.Add(this.m_lblLanguageName);
			this.Controls.Add(this.m_labelClickFinish);
			this.Controls.Add(this.m_labelReady);
			this.Name = "WizNew_Summary";
			this.Size = new System.Drawing.Size(372, 306);
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label m_labelReady;
        private System.Windows.Forms.Label m_labelClickFinish;
        private System.Windows.Forms.Label m_lblLanguageName;
		private System.Windows.Forms.Label m_textLanguageName;
        private System.Windows.Forms.Label m_lblSourceTranslation;
        private System.Windows.Forms.Label m_textSourceTranslation;
        private System.Windows.Forms.Label m_textCluster;
        private System.Windows.Forms.Label m_lblCluster;
        private System.Windows.Forms.CheckBox m_checkLaunchProperties;
    }
}
