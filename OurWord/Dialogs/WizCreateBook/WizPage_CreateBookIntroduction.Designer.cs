namespace OurWord.Dialogs.WizCreateBook
{
    partial class WizPage_CreateBookIntroduction
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WizPage_CreateBookIntroduction));
            this.m_labelPurpose = new System.Windows.Forms.Label();
            this.m_labelBookMustExist = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // m_labelPurpose
            // 
            this.m_labelPurpose.Location = new System.Drawing.Point(12, 11);
            this.m_labelPurpose.Name = "m_labelPurpose";
            this.m_labelPurpose.Size = new System.Drawing.Size(347, 36);
            this.m_labelPurpose.TabIndex = 0;
            this.m_labelPurpose.Text = "This Wizard will guide you through the process of creating a blank, empty book fo" +
                "r drafting.";
            // 
            // m_labelBookMustExist
            // 
            this.m_labelBookMustExist.Location = new System.Drawing.Point(12, 59);
            this.m_labelBookMustExist.Name = "m_labelBookMustExist";
            this.m_labelBookMustExist.Size = new System.Drawing.Size(347, 82);
            this.m_labelBookMustExist.TabIndex = 1;
            this.m_labelBookMustExist.Text = resources.GetString("m_labelBookMustExist.Text");
            // 
            // WizPage_CreateBookIntroduction
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.m_labelBookMustExist);
            this.Controls.Add(this.m_labelPurpose);
            this.Name = "WizPage_CreateBookIntroduction";
            this.Size = new System.Drawing.Size(372, 306);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label m_labelPurpose;
        private System.Windows.Forms.Label m_labelBookMustExist;
    }
}
