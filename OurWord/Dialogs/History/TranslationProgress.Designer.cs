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
            this.m_panelHeader = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // m_panelTranslationProgress
            // 
            this.m_panelTranslationProgress.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.m_panelTranslationProgress.Location = new System.Drawing.Point(0, 59);
            this.m_panelTranslationProgress.Margin = new System.Windows.Forms.Padding(4);
            this.m_panelTranslationProgress.Name = "m_panelTranslationProgress";
            this.m_panelTranslationProgress.Size = new System.Drawing.Size(462, 406);
            this.m_panelTranslationProgress.TabIndex = 0;
            this.m_panelTranslationProgress.Paint += new System.Windows.Forms.PaintEventHandler(this.cmdPaintProgress);
            // 
            // m_ScrollBar
            // 
            this.m_ScrollBar.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.m_ScrollBar.Location = new System.Drawing.Point(462, 55);
            this.m_ScrollBar.Name = "m_ScrollBar";
            this.m_ScrollBar.Size = new System.Drawing.Size(17, 411);
            this.m_ScrollBar.TabIndex = 1;
            this.m_ScrollBar.Scroll += new System.Windows.Forms.ScrollEventHandler(this.cmdScrolled);
            // 
            // m_panelHeader
            // 
            this.m_panelHeader.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.m_panelHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.m_panelHeader.Location = new System.Drawing.Point(0, 0);
            this.m_panelHeader.Margin = new System.Windows.Forms.Padding(4);
            this.m_panelHeader.Name = "m_panelHeader";
            this.m_panelHeader.Size = new System.Drawing.Size(479, 51);
            this.m_panelHeader.TabIndex = 1;
            this.m_panelHeader.Paint += new System.Windows.Forms.PaintEventHandler(this.cmdPaintHeader);
            // 
            // TranslationProgress
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(120F, 120F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.m_panelHeader);
            this.Controls.Add(this.m_ScrollBar);
            this.Controls.Add(this.m_panelTranslationProgress);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "TranslationProgress";
            this.Size = new System.Drawing.Size(479, 466);
            this.Resize += new System.EventHandler(this.cmdResize);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel m_panelTranslationProgress;
        private System.Windows.Forms.VScrollBar m_ScrollBar;
        private System.Windows.Forms.Panel m_panelHeader;
    }
}
