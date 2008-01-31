namespace OurWord.Dialogs.WizNewProject
{
    partial class WizNew_ProjectName
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WizNew_ProjectName));
            this.m_lblInstruction = new System.Windows.Forms.Label();
            this.m_lblExplanation = new System.Windows.Forms.Label();
            this.m_lblProjectName = new System.Windows.Forms.Label();
            this.m_textProjectName = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // m_lblInstruction
            // 
            this.m_lblInstruction.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_lblInstruction.Location = new System.Drawing.Point(13, 9);
            this.m_lblInstruction.Name = "m_lblInstruction";
            this.m_lblInstruction.Size = new System.Drawing.Size(346, 23);
            this.m_lblInstruction.TabIndex = 0;
            this.m_lblInstruction.Text = "Please give a name for this language project.";
            this.m_lblInstruction.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // m_lblExplanation
            // 
            this.m_lblExplanation.Location = new System.Drawing.Point(13, 32);
            this.m_lblExplanation.Name = "m_lblExplanation";
            this.m_lblExplanation.Size = new System.Drawing.Size(346, 57);
            this.m_lblExplanation.TabIndex = 1;
            this.m_lblExplanation.Text = resources.GetString("m_lblExplanation.Text");
            // 
            // m_lblProjectName
            // 
            this.m_lblProjectName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_lblProjectName.Location = new System.Drawing.Point(13, 103);
            this.m_lblProjectName.Name = "m_lblProjectName";
            this.m_lblProjectName.Size = new System.Drawing.Size(346, 23);
            this.m_lblProjectName.TabIndex = 2;
            this.m_lblProjectName.Text = "Language Project Name:";
            this.m_lblProjectName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // m_textProjectName
            // 
            this.m_textProjectName.Location = new System.Drawing.Point(16, 129);
            this.m_textProjectName.Name = "m_textProjectName";
            this.m_textProjectName.Size = new System.Drawing.Size(343, 20);
            this.m_textProjectName.TabIndex = 3;
            this.m_textProjectName.TextChanged += new System.EventHandler(this.cmdProjectNameChanged);
            // 
            // WizNew_ProjectName
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.m_textProjectName);
            this.Controls.Add(this.m_lblProjectName);
            this.Controls.Add(this.m_lblExplanation);
            this.Controls.Add(this.m_lblInstruction);
            this.Name = "WizNew_ProjectName";
            this.Size = new System.Drawing.Size(372, 306);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label m_lblInstruction;
        private System.Windows.Forms.Label m_lblExplanation;
        private System.Windows.Forms.Label m_lblProjectName;
        private System.Windows.Forms.TextBox m_textProjectName;
    }
}
