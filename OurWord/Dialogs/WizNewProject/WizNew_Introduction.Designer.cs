namespace OurWord.Dialogs.WizNewProject
{
    partial class WizNew_Introduction
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
			this.m_lblDescription1 = new System.Windows.Forms.Label();
			this.m_lblDescription2 = new System.Windows.Forms.Label();
			this.m_lblClickNextToStart = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// m_lblDescription1
			// 
			this.m_lblDescription1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.m_lblDescription1.Location = new System.Drawing.Point(12, 10);
			this.m_lblDescription1.Name = "m_lblDescription1";
			this.m_lblDescription1.Size = new System.Drawing.Size(340, 37);
			this.m_lblDescription1.TabIndex = 0;
			this.m_lblDescription1.Text = "This wizard will guide you through the process of creating a new project in OurWo" +
				"rd. ";
			// 
			// m_lblDescription2
			// 
			this.m_lblDescription2.Location = new System.Drawing.Point(12, 47);
			this.m_lblDescription2.Name = "m_lblDescription2";
			this.m_lblDescription2.Size = new System.Drawing.Size(340, 55);
			this.m_lblDescription2.TabIndex = 1;
			this.m_lblDescription2.Text = "You will be identifying information about the source translation, the language yo" +
				"u are translating into, and possibly several other important items.";
			// 
			// m_lblClickNextToStart
			// 
			this.m_lblClickNextToStart.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.m_lblClickNextToStart.Location = new System.Drawing.Point(12, 265);
			this.m_lblClickNextToStart.Name = "m_lblClickNextToStart";
			this.m_lblClickNextToStart.Size = new System.Drawing.Size(340, 25);
			this.m_lblClickNextToStart.TabIndex = 2;
			this.m_lblClickNextToStart.Text = "Click \'Next\' to get started....";
			// 
			// WizNew_Introduction
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.m_lblClickNextToStart);
			this.Controls.Add(this.m_lblDescription2);
			this.Controls.Add(this.m_lblDescription1);
			this.Name = "WizNew_Introduction";
			this.Size = new System.Drawing.Size(372, 306);
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label m_lblDescription1;
        private System.Windows.Forms.Label m_lblDescription2;
        private System.Windows.Forms.Label m_lblClickNextToStart;
    }
}
