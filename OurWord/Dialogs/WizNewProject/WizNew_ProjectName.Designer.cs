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
			this.m_lblError = new System.Windows.Forms.Label();
			this.m_group = new System.Windows.Forms.GroupBox();
			this.m_group.SuspendLayout();
			this.SuspendLayout();
			// 
			// m_lblInstruction
			// 
			this.m_lblInstruction.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.m_lblInstruction.Location = new System.Drawing.Point(14, 10);
			this.m_lblInstruction.Name = "m_lblInstruction";
			this.m_lblInstruction.Size = new System.Drawing.Size(346, 23);
			this.m_lblInstruction.TabIndex = 0;
			this.m_lblInstruction.Text = "Please give a name for this language project.";
			this.m_lblInstruction.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// m_lblExplanation
			// 
			this.m_lblExplanation.Location = new System.Drawing.Point(14, 98);
			this.m_lblExplanation.Name = "m_lblExplanation";
			this.m_lblExplanation.Size = new System.Drawing.Size(346, 123);
			this.m_lblExplanation.TabIndex = 1;
			this.m_lblExplanation.Text = resources.GetString("m_lblExplanation.Text");
			// 
			// m_lblProjectName
			// 
			this.m_lblProjectName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.m_lblProjectName.Location = new System.Drawing.Point(6, 16);
			this.m_lblProjectName.Name = "m_lblProjectName";
			this.m_lblProjectName.Size = new System.Drawing.Size(128, 23);
			this.m_lblProjectName.TabIndex = 2;
			this.m_lblProjectName.Text = "Language Project Name:";
			this.m_lblProjectName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// m_textProjectName
			// 
			this.m_textProjectName.Location = new System.Drawing.Point(140, 19);
			this.m_textProjectName.Name = "m_textProjectName";
			this.m_textProjectName.Size = new System.Drawing.Size(191, 20);
			this.m_textProjectName.TabIndex = 1;
			this.m_textProjectName.TextChanged += new System.EventHandler(this.cmdProjectNameChanged);
			// 
			// m_lblError
			// 
			this.m_lblError.ForeColor = System.Drawing.Color.Red;
			this.m_lblError.Location = new System.Drawing.Point(14, 240);
			this.m_lblError.Name = "m_lblError";
			this.m_lblError.Size = new System.Drawing.Size(346, 52);
			this.m_lblError.TabIndex = 2;
			this.m_lblError.Text = "Please enter a different name, as you already have a language with this name. ";
			// 
			// m_group
			// 
			this.m_group.Controls.Add(this.m_lblProjectName);
			this.m_group.Controls.Add(this.m_textProjectName);
			this.m_group.Location = new System.Drawing.Point(17, 32);
			this.m_group.Name = "m_group";
			this.m_group.Size = new System.Drawing.Size(337, 51);
			this.m_group.TabIndex = 3;
			this.m_group.TabStop = false;
			// 
			// WizNew_ProjectName
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.m_group);
			this.Controls.Add(this.m_lblError);
			this.Controls.Add(this.m_lblExplanation);
			this.Controls.Add(this.m_lblInstruction);
			this.Name = "WizNew_ProjectName";
			this.Size = new System.Drawing.Size(372, 306);
			this.m_group.ResumeLayout(false);
			this.m_group.PerformLayout();
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label m_lblInstruction;
        private System.Windows.Forms.Label m_lblExplanation;
        private System.Windows.Forms.Label m_lblProjectName;
        private System.Windows.Forms.TextBox m_textProjectName;
		private System.Windows.Forms.Label m_lblError;
		private System.Windows.Forms.GroupBox m_group;
    }
}
