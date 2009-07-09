namespace OurWord.SideWnd
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
            this.m_labelDefinition = new System.Windows.Forms.Label();
            this.m_labelExampleSentence = new System.Windows.Forms.Label();
            this.m_textDefinition = new System.Windows.Forms.TextBox();
            this.m_textExampleSentence = new System.Windows.Forms.TextBox();
            this.m_btnAdd = new System.Windows.Forms.Button();
            this.m_btnCancel = new System.Windows.Forms.Button();
            this.m_btnOpenInWeSay = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // m_lblWord
            // 
            this.m_lblWord.Location = new System.Drawing.Point(12, 10);
            this.m_lblWord.Name = "m_lblWord";
            this.m_lblWord.Size = new System.Drawing.Size(62, 23);
            this.m_lblWord.TabIndex = 0;
            this.m_lblWord.Text = "Word:";
            this.m_lblWord.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // m_textWord
            // 
            this.m_textWord.Location = new System.Drawing.Point(80, 13);
            this.m_textWord.Name = "m_textWord";
            this.m_textWord.Size = new System.Drawing.Size(102, 20);
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
            this.m_Html.Size = new System.Drawing.Size(20, 80);
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
            // m_labelDefinition
            // 
            this.m_labelDefinition.Location = new System.Drawing.Point(12, 60);
            this.m_labelDefinition.Name = "m_labelDefinition";
            this.m_labelDefinition.Size = new System.Drawing.Size(206, 23);
            this.m_labelDefinition.TabIndex = 5;
            this.m_labelDefinition.Text = "Definition:";
            this.m_labelDefinition.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // m_labelExampleSentence
            // 
            this.m_labelExampleSentence.Location = new System.Drawing.Point(12, 158);
            this.m_labelExampleSentence.Name = "m_labelExampleSentence";
            this.m_labelExampleSentence.Size = new System.Drawing.Size(206, 23);
            this.m_labelExampleSentence.TabIndex = 6;
            this.m_labelExampleSentence.Text = "Example Sentence:";
            this.m_labelExampleSentence.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // m_textDefinition
            // 
            this.m_textDefinition.Location = new System.Drawing.Point(15, 86);
            this.m_textDefinition.Multiline = true;
            this.m_textDefinition.Name = "m_textDefinition";
            this.m_textDefinition.Size = new System.Drawing.Size(203, 69);
            this.m_textDefinition.TabIndex = 7;
            // 
            // m_textExampleSentence
            // 
            this.m_textExampleSentence.Location = new System.Drawing.Point(15, 184);
            this.m_textExampleSentence.Multiline = true;
            this.m_textExampleSentence.Name = "m_textExampleSentence";
            this.m_textExampleSentence.Size = new System.Drawing.Size(203, 102);
            this.m_textExampleSentence.TabIndex = 8;
            // 
            // m_btnAdd
            // 
            this.m_btnAdd.Location = new System.Drawing.Point(15, 306);
            this.m_btnAdd.Name = "m_btnAdd";
            this.m_btnAdd.Size = new System.Drawing.Size(149, 23);
            this.m_btnAdd.TabIndex = 9;
            this.m_btnAdd.Text = "Add to Dictionary";
            this.m_btnAdd.UseVisualStyleBackColor = true;
            this.m_btnAdd.Click += new System.EventHandler(this.cmdAddToDictionary);
            // 
            // m_btnCancel
            // 
            this.m_btnCancel.Location = new System.Drawing.Point(15, 335);
            this.m_btnCancel.Name = "m_btnCancel";
            this.m_btnCancel.Size = new System.Drawing.Size(149, 23);
            this.m_btnCancel.TabIndex = 10;
            this.m_btnCancel.Text = "Cancel";
            this.m_btnCancel.UseVisualStyleBackColor = true;
            this.m_btnCancel.Click += new System.EventHandler(this.cmdCancelNewEntry);
            // 
            // m_btnOpenInWeSay
            // 
            this.m_btnOpenInWeSay.Location = new System.Drawing.Point(15, 364);
            this.m_btnOpenInWeSay.Name = "m_btnOpenInWeSay";
            this.m_btnOpenInWeSay.Size = new System.Drawing.Size(149, 23);
            this.m_btnOpenInWeSay.TabIndex = 11;
            this.m_btnOpenInWeSay.Text = "Open in WeSay...";
            this.m_btnOpenInWeSay.UseVisualStyleBackColor = true;
            this.m_btnOpenInWeSay.Click += new System.EventHandler(this.cmdOpenInDictionary);
            // 
            // DictionaryPane
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.m_btnOpenInWeSay);
            this.Controls.Add(this.m_btnCancel);
            this.Controls.Add(this.m_btnAdd);
            this.Controls.Add(this.m_textExampleSentence);
            this.Controls.Add(this.m_textDefinition);
            this.Controls.Add(this.m_labelExampleSentence);
            this.Controls.Add(this.m_labelDefinition);
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
        private System.Windows.Forms.Label m_labelDefinition;
        private System.Windows.Forms.Label m_labelExampleSentence;
        private System.Windows.Forms.TextBox m_textDefinition;
        private System.Windows.Forms.TextBox m_textExampleSentence;
        private System.Windows.Forms.Button m_btnAdd;
        private System.Windows.Forms.Button m_btnCancel;
        private System.Windows.Forms.Button m_btnOpenInWeSay;

    }
}
