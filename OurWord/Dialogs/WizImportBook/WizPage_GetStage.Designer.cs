namespace OurWord.Dialogs.WizImportBook
{
    partial class WizPage_GetStage
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
            this.m_labelExplanation = new System.Windows.Forms.Label();
            this.m_labelStage = new System.Windows.Forms.Label();
            this.m_comboStage = new System.Windows.Forms.ComboBox();
            this.m_labelVersion = new System.Windows.Forms.Label();
            this.m_spinVersion = new System.Windows.Forms.DomainUpDown();
            this.SuspendLayout();
            // 
            // m_labelExplanation
            // 
            this.m_labelExplanation.Location = new System.Drawing.Point(20, 242);
            this.m_labelExplanation.Name = "m_labelExplanation";
            this.m_labelExplanation.Size = new System.Drawing.Size(360, 44);
            this.m_labelExplanation.TabIndex = 0;
            this.m_labelExplanation.Text = "(Note: All of this information is optional;  but highly recommended if the book b" +
                "eing imported is still in the process of being translated.)";
            // 
            // m_labelStage
            // 
            this.m_labelStage.Location = new System.Drawing.Point(20, 15);
            this.m_labelStage.Name = "m_labelStage";
            this.m_labelStage.Size = new System.Drawing.Size(360, 23);
            this.m_labelStage.TabIndex = 1;
            this.m_labelStage.Text = "At what stage of translation is this book?";
            this.m_labelStage.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // m_comboStage
            // 
            this.m_comboStage.FormattingEnabled = true;
            this.m_comboStage.Location = new System.Drawing.Point(40, 41);
            this.m_comboStage.Name = "m_comboStage";
            this.m_comboStage.Size = new System.Drawing.Size(171, 21);
            this.m_comboStage.TabIndex = 2;
            // 
            // m_labelVersion
            // 
            this.m_labelVersion.Location = new System.Drawing.Point(20, 86);
            this.m_labelVersion.Name = "m_labelVersion";
            this.m_labelVersion.Size = new System.Drawing.Size(360, 23);
            this.m_labelVersion.TabIndex = 3;
            this.m_labelVersion.Text = "You can optionally enter a version for this stage, e.g., \'A\', \'B\', \'C\'...";
            this.m_labelVersion.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // m_spinVersion
            // 
            this.m_spinVersion.Location = new System.Drawing.Point(40, 112);
            this.m_spinVersion.Name = "m_spinVersion";
            this.m_spinVersion.Size = new System.Drawing.Size(48, 20);
            this.m_spinVersion.TabIndex = 32;
            this.m_spinVersion.Text = "A";
            this.m_spinVersion.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // WizPage_GetStage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.m_spinVersion);
            this.Controls.Add(this.m_labelVersion);
            this.Controls.Add(this.m_comboStage);
            this.Controls.Add(this.m_labelStage);
            this.Controls.Add(this.m_labelExplanation);
            this.Name = "WizPage_GetStage";
            this.Size = new System.Drawing.Size(392, 305);
            this.Load += new System.EventHandler(this.cmdLoad);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label m_labelExplanation;
        private System.Windows.Forms.Label m_labelStage;
        private System.Windows.Forms.ComboBox m_comboStage;
        private System.Windows.Forms.Label m_labelVersion;
        private System.Windows.Forms.DomainUpDown m_spinVersion;
    }
}
