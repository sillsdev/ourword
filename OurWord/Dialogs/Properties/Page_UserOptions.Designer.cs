namespace OurWord.Dialogs.Properties
{
    partial class Page_UserOptions
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
            this.m_grid = new System.Windows.Forms.PropertyGrid();
            this.SuspendLayout();
            // 
            // m_pgUiLanguages
            // 
            this.m_grid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_grid.Location = new System.Drawing.Point(0, 0);
            this.m_grid.Name = "m_grid";
            this.m_grid.PropertySort = System.Windows.Forms.PropertySort.Categorized;
            this.m_grid.Size = new System.Drawing.Size(469, 284);
            this.m_grid.TabIndex = 74;
            this.m_grid.ToolbarVisible = false;
            // 
            // Page_UserOptions
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Controls.Add(this.m_grid);
            this.Name = "Page_UserOptions";
            this.Size = new System.Drawing.Size(469, 284);
            this.Load += new System.EventHandler(this.cmdLoad);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PropertyGrid m_grid;
    }
}
