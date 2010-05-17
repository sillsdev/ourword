namespace OurWord.Dialogs.Properties
{
    partial class CtrlPageContent
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
            this.m_NavTitle = new System.Windows.Forms.Label();
            this.m_panelContent = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // m_NavTitle
            // 
            this.m_NavTitle.BackColor = System.Drawing.Color.LightSeaGreen;
            this.m_NavTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.m_NavTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_NavTitle.ForeColor = System.Drawing.Color.White;
            this.m_NavTitle.Location = new System.Drawing.Point(0, 0);
            this.m_NavTitle.Name = "m_NavTitle";
            this.m_NavTitle.Size = new System.Drawing.Size(422, 26);
            this.m_NavTitle.TabIndex = 23;
            this.m_NavTitle.Text = "Navigation Title";
            this.m_NavTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // m_panelContent
            // 
            this.m_panelContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_panelContent.Location = new System.Drawing.Point(0, 26);
            this.m_panelContent.Name = "m_panelContent";
            this.m_panelContent.Size = new System.Drawing.Size(422, 299);
            this.m_panelContent.TabIndex = 24;
            // 
            // CtrlPropertiesContent
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.Controls.Add(this.m_panelContent);
            this.Controls.Add(this.m_NavTitle);
            this.Name = "CtrlPageContent";
            this.Size = new System.Drawing.Size(422, 325);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label m_NavTitle;
        private System.Windows.Forms.Panel m_panelContent;
    }
}
