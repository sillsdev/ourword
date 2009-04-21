namespace OurWord.Dialogs.WizNewProject
{
    partial class WizNew_FrontInfo
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WizNew_FrontInfo));
			this.m_lblBrowseToExistingFront = new System.Windows.Forms.Label();
			this.m_group = new System.Windows.Forms.GroupBox();
			this.m_comboChooseLanguage = new System.Windows.Forms.ComboBox();
			this.m_lblName = new System.Windows.Forms.Label();
			this.m_lblInstructions = new System.Windows.Forms.Label();
			this.m_group.SuspendLayout();
			this.SuspendLayout();
			// 
			// m_lblBrowseToExistingFront
			// 
			this.m_lblBrowseToExistingFront.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.m_lblBrowseToExistingFront.Location = new System.Drawing.Point(14, 10);
			this.m_lblBrowseToExistingFront.Name = "m_lblBrowseToExistingFront";
			this.m_lblBrowseToExistingFront.Size = new System.Drawing.Size(340, 19);
			this.m_lblBrowseToExistingFront.TabIndex = 0;
			this.m_lblBrowseToExistingFront.Text = "What Front/Source language do you wish to use?";
			this.m_lblBrowseToExistingFront.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// m_group
			// 
			this.m_group.Controls.Add(this.m_comboChooseLanguage);
			this.m_group.Controls.Add(this.m_lblName);
			this.m_group.Location = new System.Drawing.Point(17, 32);
			this.m_group.Name = "m_group";
			this.m_group.Size = new System.Drawing.Size(337, 51);
			this.m_group.TabIndex = 1;
			this.m_group.TabStop = false;
			// 
			// m_comboChooseLanguage
			// 
			this.m_comboChooseLanguage.DropDownHeight = 200;
			this.m_comboChooseLanguage.FormattingEnabled = true;
			this.m_comboChooseLanguage.IntegralHeight = false;
			this.m_comboChooseLanguage.Location = new System.Drawing.Point(140, 18);
			this.m_comboChooseLanguage.MaxDropDownItems = 15;
			this.m_comboChooseLanguage.Name = "m_comboChooseLanguage";
			this.m_comboChooseLanguage.Size = new System.Drawing.Size(191, 21);
			this.m_comboChooseLanguage.Sorted = true;
			this.m_comboChooseLanguage.TabIndex = 1;
			this.m_comboChooseLanguage.SelectedIndexChanged += new System.EventHandler(this.cmdComboTextChanged);
			this.m_comboChooseLanguage.TextUpdate += new System.EventHandler(this.cmdComboTextChanged);
			// 
			// m_lblName
			// 
			this.m_lblName.Location = new System.Drawing.Point(6, 16);
			this.m_lblName.Name = "m_lblName";
			this.m_lblName.Size = new System.Drawing.Size(128, 23);
			this.m_lblName.TabIndex = 26;
			this.m_lblName.Text = "Front Language Name:";
			this.m_lblName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// m_lblInstructions
			// 
			this.m_lblInstructions.Location = new System.Drawing.Point(14, 98);
			this.m_lblInstructions.Name = "m_lblInstructions";
			this.m_lblInstructions.Size = new System.Drawing.Size(340, 103);
			this.m_lblInstructions.TabIndex = 3;
			this.m_lblInstructions.Text = resources.GetString("m_lblInstructions.Text");
			// 
			// WizNew_FrontInfo
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.m_lblInstructions);
			this.Controls.Add(this.m_group);
			this.Controls.Add(this.m_lblBrowseToExistingFront);
			this.Name = "WizNew_FrontInfo";
			this.Size = new System.Drawing.Size(372, 306);
			this.m_group.ResumeLayout(false);
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label m_lblBrowseToExistingFront;
		private System.Windows.Forms.GroupBox m_group;
        private System.Windows.Forms.Label m_lblInstructions;
		private System.Windows.Forms.Label m_lblName;
		private System.Windows.Forms.ComboBox m_comboChooseLanguage;
    }
}
