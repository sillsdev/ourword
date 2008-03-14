namespace OurWord
{
    partial class DlgDebug
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.m_DebugText = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // m_DebugText
            // 
            this.m_DebugText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_DebugText.Location = new System.Drawing.Point(0, 0);
            this.m_DebugText.Multiline = true;
            this.m_DebugText.Name = "m_DebugText";
            this.m_DebugText.Size = new System.Drawing.Size(617, 264);
            this.m_DebugText.TabIndex = 0;
            // 
            // DlgDebug
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(617, 264);
            this.Controls.Add(this.m_DebugText);
            this.Name = "DlgDebug";
            this.Text = "Debug";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.cmdClosed);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox m_DebugText;
    }
}