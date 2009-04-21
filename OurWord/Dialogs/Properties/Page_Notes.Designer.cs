namespace OurWord.Dialogs
{
    partial class Page_Notes
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
			this.m_PropGrid = new System.Windows.Forms.PropertyGrid();
			this.SuspendLayout();
			// 
			// m_PropGrid
			// 
			this.m_PropGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.m_PropGrid.Location = new System.Drawing.Point(3, 3);
			this.m_PropGrid.Name = "m_PropGrid";
			this.m_PropGrid.PropertySort = System.Windows.Forms.PropertySort.Categorized;
			this.m_PropGrid.Size = new System.Drawing.Size(462, 355);
			this.m_PropGrid.TabIndex = 46;
			this.m_PropGrid.ToolbarVisible = false;
			// 
			// Page_Notes
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.Controls.Add(this.m_PropGrid);
			this.Name = "Page_Notes";
			this.Size = new System.Drawing.Size(468, 361);
			this.Load += new System.EventHandler(this.cmdLoad);
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PropertyGrid m_PropGrid;
    }
}
