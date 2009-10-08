namespace OurWord.Dialogs
{
    partial class TranslationProgress
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
            this.m_panelTranslationProgress = new System.Windows.Forms.Panel();
            this.m_ScrollBar = new System.Windows.Forms.VScrollBar();
            this.SuspendLayout();
            // 
            // m_panelTranslationProgress
            // 
            this.m_panelTranslationProgress.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.m_panelTranslationProgress.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.m_panelTranslationProgress.Location = new System.Drawing.Point(0, 0);
            this.m_panelTranslationProgress.Name = "m_panelTranslationProgress";
            this.m_panelTranslationProgress.Size = new System.Drawing.Size(363, 373);
            this.m_panelTranslationProgress.TabIndex = 0;
            this.m_panelTranslationProgress.Paint += new System.Windows.Forms.PaintEventHandler(this.cmdPaintProgress);
            // 
            // m_ScrollBar
            // 
            this.m_ScrollBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.m_ScrollBar.Location = new System.Drawing.Point(366, 0);
            this.m_ScrollBar.Name = "m_ScrollBar";
            this.m_ScrollBar.Size = new System.Drawing.Size(17, 373);
            this.m_ScrollBar.TabIndex = 1;
            this.m_ScrollBar.Scroll += new System.Windows.Forms.ScrollEventHandler(this.cmdScrolled);
            // 
            // TranslationProgress
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.m_ScrollBar);
            this.Controls.Add(this.m_panelTranslationProgress);
            this.Name = "TranslationProgress";
            this.Size = new System.Drawing.Size(383, 373);
            this.Load += new System.EventHandler(this.cmdLoad);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel m_panelTranslationProgress;
        private System.Windows.Forms.VScrollBar m_ScrollBar;
    }
}
