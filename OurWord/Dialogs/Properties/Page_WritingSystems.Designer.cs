namespace OurWord.Dialogs
{
    partial class Page_WritingSystems
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;


        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.m_labelWritingSystems = new System.Windows.Forms.Label();
            this.m_listWritingSystems = new System.Windows.Forms.ListBox();
            this.m_labelInstructions = new System.Windows.Forms.Label();
            this.m_btnAdd = new System.Windows.Forms.Button();
            this.m_btnRemove = new System.Windows.Forms.Button();
            this.m_PropGrid = new System.Windows.Forms.PropertyGrid();
            this.m_tabWS = new System.Windows.Forms.TabControl();
            this.m_tabWSGeneral = new System.Windows.Forms.TabPage();
            this.m_tabWSAutoReplace = new System.Windows.Forms.TabPage();
            this.m_ctrlAutoReplace = new OurWord.Dialogs.Ctrl_AutoReplace();
            this.m_tabWS.SuspendLayout();
            this.m_tabWSGeneral.SuspendLayout();
            this.m_tabWSAutoReplace.SuspendLayout();
            this.SuspendLayout();
            // 
            // m_labelWritingSystems
            // 
            this.m_labelWritingSystems.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_labelWritingSystems.Location = new System.Drawing.Point(8, 8);
            this.m_labelWritingSystems.Name = "m_labelWritingSystems";
            this.m_labelWritingSystems.Size = new System.Drawing.Size(146, 21);
            this.m_labelWritingSystems.TabIndex = 0;
            this.m_labelWritingSystems.Text = "Writing Systems:";
            this.m_labelWritingSystems.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // m_listWritingSystems
            // 
            this.m_listWritingSystems.FormattingEnabled = true;
            this.m_listWritingSystems.Location = new System.Drawing.Point(8, 32);
            this.m_listWritingSystems.Name = "m_listWritingSystems";
            this.m_listWritingSystems.Size = new System.Drawing.Size(157, 303);
            this.m_listWritingSystems.TabIndex = 1;
            this.m_listWritingSystems.SelectedIndexChanged += new System.EventHandler(this.cmdSelectedWSChanged);
            // 
            // m_labelInstructions
            // 
            this.m_labelInstructions.Location = new System.Drawing.Point(184, 8);
            this.m_labelInstructions.Name = "m_labelInstructions";
            this.m_labelInstructions.Size = new System.Drawing.Size(268, 33);
            this.m_labelInstructions.TabIndex = 2;
            this.m_labelInstructions.Text = "Select a writing system from the list, then edit its settings  below.";
            this.m_labelInstructions.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // m_btnAdd
            // 
            this.m_btnAdd.Location = new System.Drawing.Point(11, 337);
            this.m_btnAdd.Name = "m_btnAdd";
            this.m_btnAdd.Size = new System.Drawing.Size(68, 23);
            this.m_btnAdd.TabIndex = 7;
            this.m_btnAdd.Text = "Add";
            this.m_btnAdd.UseVisualStyleBackColor = true;
            this.m_btnAdd.Click += new System.EventHandler(this.cmdAddBtnClicked);
            // 
            // m_btnRemove
            // 
            this.m_btnRemove.Location = new System.Drawing.Point(85, 337);
            this.m_btnRemove.Name = "m_btnRemove";
            this.m_btnRemove.Size = new System.Drawing.Size(69, 23);
            this.m_btnRemove.TabIndex = 8;
            this.m_btnRemove.Text = "ctrlRemove...";
            this.m_btnRemove.UseVisualStyleBackColor = true;
            this.m_btnRemove.Click += new System.EventHandler(this.cmdRemoveBtnClicked);
            // 
            // m_PropGrid
            // 
            this.m_PropGrid.LineColor = System.Drawing.SystemColors.ScrollBar;
            this.m_PropGrid.Location = new System.Drawing.Point(6, 6);
            this.m_PropGrid.Name = "m_PropGrid";
            this.m_PropGrid.PropertySort = System.Windows.Forms.PropertySort.Categorized;
            this.m_PropGrid.Size = new System.Drawing.Size(255, 278);
            this.m_PropGrid.TabIndex = 33;
            this.m_PropGrid.ToolbarVisible = false;
            // 
            // m_tabWS
            // 
            this.m_tabWS.Controls.Add(this.m_tabWSGeneral);
            this.m_tabWS.Controls.Add(this.m_tabWSAutoReplace);
            this.m_tabWS.Location = new System.Drawing.Point(187, 44);
            this.m_tabWS.Name = "m_tabWS";
            this.m_tabWS.SelectedIndex = 0;
            this.m_tabWS.Size = new System.Drawing.Size(278, 316);
            this.m_tabWS.TabIndex = 34;
            // 
            // m_tabWSGeneral
            // 
            this.m_tabWSGeneral.Controls.Add(this.m_PropGrid);
            this.m_tabWSGeneral.Location = new System.Drawing.Point(4, 22);
            this.m_tabWSGeneral.Name = "m_tabWSGeneral";
            this.m_tabWSGeneral.Padding = new System.Windows.Forms.Padding(3);
            this.m_tabWSGeneral.Size = new System.Drawing.Size(270, 290);
            this.m_tabWSGeneral.TabIndex = 0;
            this.m_tabWSGeneral.Text = "General";
            this.m_tabWSGeneral.UseVisualStyleBackColor = true;
            // 
            // m_tabWSAutoReplace
            // 
            this.m_tabWSAutoReplace.Controls.Add(this.m_ctrlAutoReplace);
            this.m_tabWSAutoReplace.Location = new System.Drawing.Point(4, 22);
            this.m_tabWSAutoReplace.Name = "m_tabWSAutoReplace";
            this.m_tabWSAutoReplace.Padding = new System.Windows.Forms.Padding(3);
            this.m_tabWSAutoReplace.Size = new System.Drawing.Size(270, 290);
            this.m_tabWSAutoReplace.TabIndex = 1;
            this.m_tabWSAutoReplace.Text = "AutoReplace";
            this.m_tabWSAutoReplace.UseVisualStyleBackColor = true;
            // 
            // m_ctrlAutoReplace
            // 
            this.m_ctrlAutoReplace.Location = new System.Drawing.Point(6, 3);
            this.m_ctrlAutoReplace.Name = "m_ctrlAutoReplace";
            this.m_ctrlAutoReplace.Size = new System.Drawing.Size(257, 287);
            this.m_ctrlAutoReplace.TabIndex = 0;
            // 
            // Page_WritingSystems
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.m_tabWS);
            this.Controls.Add(this.m_btnRemove);
            this.Controls.Add(this.m_btnAdd);
            this.Controls.Add(this.m_labelInstructions);
            this.Controls.Add(this.m_listWritingSystems);
            this.Controls.Add(this.m_labelWritingSystems);
            this.Name = "Page_WritingSystems";
            this.Size = new System.Drawing.Size(468, 368);
            this.Load += new System.EventHandler(this.cmdLoad);
            this.m_tabWS.ResumeLayout(false);
            this.m_tabWSGeneral.ResumeLayout(false);
            this.m_tabWSAutoReplace.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label m_labelWritingSystems;
        private System.Windows.Forms.ListBox m_listWritingSystems;
        private System.Windows.Forms.Label m_labelInstructions;
        private System.Windows.Forms.Button m_btnAdd;
        private System.Windows.Forms.Button m_btnRemove;
        private System.Windows.Forms.PropertyGrid m_PropGrid;
        private System.Windows.Forms.TabControl m_tabWS;
        private System.Windows.Forms.TabPage m_tabWSGeneral;
        private System.Windows.Forms.TabPage m_tabWSAutoReplace;
        private Ctrl_AutoReplace m_ctrlAutoReplace;
    }
}
