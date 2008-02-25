namespace OurWord.Edit
{
    partial class DictionaryPane
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DictionaryPane));
            this.m_lblWord = new System.Windows.Forms.Label();
            this.m_textWord = new System.Windows.Forms.TextBox();
            this.m_btnSearch = new System.Windows.Forms.Button();
            this.m_Html = new System.Windows.Forms.WebBrowser();
            this.m_checkExactSearch = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // m_lblWord
            // 
            this.m_lblWord.Location = new System.Drawing.Point(12, 10);
            this.m_lblWord.Name = "m_lblWord";
            this.m_lblWord.Size = new System.Drawing.Size(52, 23);
            this.m_lblWord.TabIndex = 0;
            this.m_lblWord.Text = "Word:";
            this.m_lblWord.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // m_textWord
            // 
            this.m_textWord.Location = new System.Drawing.Point(70, 13);
            this.m_textWord.Name = "m_textWord";
            this.m_textWord.Size = new System.Drawing.Size(112, 20);
            this.m_textWord.TabIndex = 1;
            // 
            // m_btnSearch
            // 
            this.m_btnSearch.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.m_btnSearch.Image = ((System.Drawing.Image)(resources.GetObject("m_btnSearch.Image")));
            this.m_btnSearch.Location = new System.Drawing.Point(188, 10);
            this.m_btnSearch.Name = "m_btnSearch";
            this.m_btnSearch.Size = new System.Drawing.Size(30, 24);
            this.m_btnSearch.TabIndex = 2;
            this.m_btnSearch.UseVisualStyleBackColor = true;
            this.m_btnSearch.Click += new System.EventHandler(this.cmdLookupWord);
            // 
            // m_Html
            // 
            this.m_Html.IsWebBrowserContextMenuEnabled = false;
            this.m_Html.Location = new System.Drawing.Point(15, 60);
            this.m_Html.MinimumSize = new System.Drawing.Size(20, 20);
            this.m_Html.Name = "m_Html";
            this.m_Html.ScrollBarsEnabled = false;
            this.m_Html.Size = new System.Drawing.Size(203, 421);
            this.m_Html.TabIndex = 3;
            this.m_Html.TabStop = false;
            // 
            // m_checkExactSearch
            // 
            this.m_checkExactSearch.AutoSize = true;
            this.m_checkExactSearch.Location = new System.Drawing.Point(15, 36);
            this.m_checkExactSearch.Name = "m_checkExactSearch";
            this.m_checkExactSearch.Size = new System.Drawing.Size(139, 17);
            this.m_checkExactSearch.TabIndex = 4;
            this.m_checkExactSearch.Text = "Require an exact match";
            this.m_checkExactSearch.UseVisualStyleBackColor = true;
            // 
            // DictionaryPane
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.m_checkExactSearch);
            this.Controls.Add(this.m_Html);
            this.Controls.Add(this.m_btnSearch);
            this.Controls.Add(this.m_textWord);
            this.Controls.Add(this.m_lblWord);
            this.Name = "DictionaryPane";
            this.Size = new System.Drawing.Size(226, 484);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label m_lblWord;
        private System.Windows.Forms.TextBox m_textWord;
        private System.Windows.Forms.Button m_btnSearch;
        private System.Windows.Forms.WebBrowser m_Html;
        private System.Windows.Forms.CheckBox m_checkExactSearch;

    }
}
