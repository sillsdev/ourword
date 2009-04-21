namespace OurWord.Dialogs
{
    partial class Ctrl_AutoReplace
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
			this.m_listview = new System.Windows.Forms.ListView();
			this.m_colReplace = new System.Windows.Forms.ColumnHeader();
			this.m_colWith = new System.Windows.Forms.ColumnHeader();
			this.m_labelReplace = new System.Windows.Forms.Label();
			this.m_editReplace = new System.Windows.Forms.TextBox();
			this.m_labelWith = new System.Windows.Forms.Label();
			this.m_editWith = new System.Windows.Forms.TextBox();
			this.m_btnAdd = new System.Windows.Forms.Button();
			this.m_btnRemove = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// m_listview
			// 
			this.m_listview.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.m_listview.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.m_colReplace,
            this.m_colWith});
			this.m_listview.FullRowSelect = true;
			this.m_listview.GridLines = true;
			this.m_listview.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			this.m_listview.HideSelection = false;
			this.m_listview.Location = new System.Drawing.Point(6, 65);
			this.m_listview.MultiSelect = false;
			this.m_listview.Name = "m_listview";
			this.m_listview.Size = new System.Drawing.Size(243, 219);
			this.m_listview.Sorting = System.Windows.Forms.SortOrder.Ascending;
			this.m_listview.TabIndex = 3;
			this.m_listview.UseCompatibleStateImageBehavior = false;
			this.m_listview.View = System.Windows.Forms.View.Details;
			this.m_listview.SelectedIndexChanged += new System.EventHandler(this.cmdListSelectionChanged);
			// 
			// m_colReplace
			// 
			this.m_colReplace.Text = "Replace";
			this.m_colReplace.Width = 114;
			// 
			// m_colWith
			// 
			this.m_colWith.Text = "With";
			this.m_colWith.Width = 109;
			// 
			// m_labelReplace
			// 
			this.m_labelReplace.Location = new System.Drawing.Point(3, 0);
			this.m_labelReplace.Name = "m_labelReplace";
			this.m_labelReplace.Size = new System.Drawing.Size(72, 24);
			this.m_labelReplace.TabIndex = 4;
			this.m_labelReplace.Text = "Replace:";
			this.m_labelReplace.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// m_editReplace
			// 
			this.m_editReplace.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.m_editReplace.Location = new System.Drawing.Point(55, 4);
			this.m_editReplace.Name = "m_editReplace";
			this.m_editReplace.Size = new System.Drawing.Size(100, 20);
			this.m_editReplace.TabIndex = 6;
			this.m_editReplace.TextChanged += new System.EventHandler(this.cmdReplaceTextChanged);
			// 
			// m_labelWith
			// 
			this.m_labelWith.Location = new System.Drawing.Point(3, 23);
			this.m_labelWith.Name = "m_labelWith";
			this.m_labelWith.Size = new System.Drawing.Size(48, 24);
			this.m_labelWith.TabIndex = 7;
			this.m_labelWith.Text = "With:";
			this.m_labelWith.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// m_editWith
			// 
			this.m_editWith.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.m_editWith.Location = new System.Drawing.Point(55, 27);
			this.m_editWith.Name = "m_editWith";
			this.m_editWith.Size = new System.Drawing.Size(100, 20);
			this.m_editWith.TabIndex = 8;
			this.m_editWith.TextChanged += new System.EventHandler(this.cmdWithTextChanged);
			this.m_editWith.KeyDown += new System.Windows.Forms.KeyEventHandler(this.cmdWithKeyDown);
			// 
			// m_btnAdd
			// 
			this.m_btnAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.m_btnAdd.Location = new System.Drawing.Point(161, 4);
			this.m_btnAdd.Name = "m_btnAdd";
			this.m_btnAdd.Size = new System.Drawing.Size(88, 23);
			this.m_btnAdd.TabIndex = 9;
			this.m_btnAdd.Text = "Add";
			this.m_btnAdd.Click += new System.EventHandler(this.cmdAdd);
			// 
			// m_btnRemove
			// 
			this.m_btnRemove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.m_btnRemove.Location = new System.Drawing.Point(161, 27);
			this.m_btnRemove.Name = "m_btnRemove";
			this.m_btnRemove.Size = new System.Drawing.Size(88, 23);
			this.m_btnRemove.TabIndex = 10;
			this.m_btnRemove.Text = "Remove";
			this.m_btnRemove.Click += new System.EventHandler(this.cmdRemove);
			// 
			// Ctrl_AutoReplace
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.Controls.Add(this.m_btnRemove);
			this.Controls.Add(this.m_btnAdd);
			this.Controls.Add(this.m_editWith);
			this.Controls.Add(this.m_labelWith);
			this.Controls.Add(this.m_editReplace);
			this.Controls.Add(this.m_labelReplace);
			this.Controls.Add(this.m_listview);
			this.Name = "Ctrl_AutoReplace";
			this.Size = new System.Drawing.Size(257, 287);
			this.Load += new System.EventHandler(this.cmdLoad);
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView m_listview;
        private System.Windows.Forms.ColumnHeader m_colReplace;
        private System.Windows.Forms.ColumnHeader m_colWith;
        private System.Windows.Forms.Label m_labelReplace;
        private System.Windows.Forms.TextBox m_editReplace;
        private System.Windows.Forms.Label m_labelWith;
        private System.Windows.Forms.TextBox m_editWith;
        private System.Windows.Forms.Button m_btnAdd;
        private System.Windows.Forms.Button m_btnRemove;
    }
}
