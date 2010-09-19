namespace OurWord.Dialogs.Properties
{
    partial class Page_UserEditPermissions
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
            this.m_gridBookByBook = new System.Windows.Forms.PropertyGrid();
            this.m_btnFullEditing = new System.Windows.Forms.Button();
            this.m_labelMakeAll = new System.Windows.Forms.Label();
            this.m_btnNotesOnly = new System.Windows.Forms.Button();
            this.m_btnReadOnly = new System.Windows.Forms.Button();
            this.m_comboMembership = new System.Windows.Forms.ComboBox();
            this.m_group = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.m_lGlobal = new System.Windows.Forms.Label();
            this.m_lPermissionsFor = new System.Windows.Forms.Label();
            this.m_cPermissionsFor = new System.Windows.Forms.ComboBox();
            this.m_group.SuspendLayout();
            this.SuspendLayout();
            // 
            // m_gridBookByBook
            // 
            this.m_gridBookByBook.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.m_gridBookByBook.HelpVisible = false;
            this.m_gridBookByBook.Location = new System.Drawing.Point(41, 76);
            this.m_gridBookByBook.Name = "m_gridBookByBook";
            this.m_gridBookByBook.PropertySort = System.Windows.Forms.PropertySort.Categorized;
            this.m_gridBookByBook.Size = new System.Drawing.Size(390, 165);
            this.m_gridBookByBook.TabIndex = 47;
            this.m_gridBookByBook.ToolbarVisible = false;
            // 
            // m_btnFullEditing
            // 
            this.m_btnFullEditing.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.m_btnFullEditing.Location = new System.Drawing.Point(125, 247);
            this.m_btnFullEditing.Name = "m_btnFullEditing";
            this.m_btnFullEditing.Size = new System.Drawing.Size(80, 23);
            this.m_btnFullEditing.TabIndex = 48;
            this.m_btnFullEditing.Text = "Full Editing";
            this.m_btnFullEditing.UseVisualStyleBackColor = true;
            this.m_btnFullEditing.Click += new System.EventHandler(this.cmdMakeAllFullEditing);
            // 
            // m_labelMakeAll
            // 
            this.m_labelMakeAll.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.m_labelMakeAll.Location = new System.Drawing.Point(38, 247);
            this.m_labelMakeAll.Name = "m_labelMakeAll";
            this.m_labelMakeAll.Size = new System.Drawing.Size(81, 23);
            this.m_labelMakeAll.TabIndex = 49;
            this.m_labelMakeAll.Text = "Make all:";
            this.m_labelMakeAll.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // m_btnNotesOnly
            // 
            this.m_btnNotesOnly.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.m_btnNotesOnly.Location = new System.Drawing.Point(211, 247);
            this.m_btnNotesOnly.Name = "m_btnNotesOnly";
            this.m_btnNotesOnly.Size = new System.Drawing.Size(80, 23);
            this.m_btnNotesOnly.TabIndex = 50;
            this.m_btnNotesOnly.Text = "Notes Only";
            this.m_btnNotesOnly.UseVisualStyleBackColor = true;
            this.m_btnNotesOnly.Click += new System.EventHandler(this.cmdMakeAllNotesOnly);
            // 
            // m_btnReadOnly
            // 
            this.m_btnReadOnly.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.m_btnReadOnly.Location = new System.Drawing.Point(297, 247);
            this.m_btnReadOnly.Name = "m_btnReadOnly";
            this.m_btnReadOnly.Size = new System.Drawing.Size(80, 23);
            this.m_btnReadOnly.TabIndex = 51;
            this.m_btnReadOnly.Text = "Read Only";
            this.m_btnReadOnly.UseVisualStyleBackColor = true;
            this.m_btnReadOnly.Click += new System.EventHandler(this.cmdMakeAllReadOnly);
            // 
            // m_comboMembership
            // 
            this.m_comboMembership.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.m_comboMembership.FormattingEnabled = true;
            this.m_comboMembership.Location = new System.Drawing.Point(41, 34);
            this.m_comboMembership.Name = "m_comboMembership";
            this.m_comboMembership.Size = new System.Drawing.Size(390, 21);
            this.m_comboMembership.TabIndex = 54;
            this.m_comboMembership.SelectedIndexChanged += new System.EventHandler(this.cmdGlobalEditingChanged);
            // 
            // m_group
            // 
            this.m_group.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.m_group.Controls.Add(this.label1);
            this.m_group.Controls.Add(this.m_lGlobal);
            this.m_group.Controls.Add(this.m_labelMakeAll);
            this.m_group.Controls.Add(this.m_gridBookByBook);
            this.m_group.Controls.Add(this.m_btnFullEditing);
            this.m_group.Controls.Add(this.m_comboMembership);
            this.m_group.Controls.Add(this.m_btnReadOnly);
            this.m_group.Controls.Add(this.m_btnNotesOnly);
            this.m_group.Location = new System.Drawing.Point(8, 49);
            this.m_group.Name = "m_group";
            this.m_group.Size = new System.Drawing.Size(447, 282);
            this.m_group.TabIndex = 55;
            this.m_group.TabStop = false;
            this.m_group.Text = "This User\'s Permission for This Language:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 60);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(111, 13);
            this.label1.TabIndex = 56;
            this.label1.Text = "Or Set Book By Book:";
            // 
            // m_lGlobal
            // 
            this.m_lGlobal.AutoSize = true;
            this.m_lGlobal.Location = new System.Drawing.Point(9, 16);
            this.m_lGlobal.Name = "m_lGlobal";
            this.m_lGlobal.Size = new System.Drawing.Size(126, 13);
            this.m_lGlobal.TabIndex = 55;
            this.m_lGlobal.Text = "Set for Entire Translation:";
            // 
            // m_lPermissionsFor
            // 
            this.m_lPermissionsFor.Location = new System.Drawing.Point(5, 7);
            this.m_lPermissionsFor.Name = "m_lPermissionsFor";
            this.m_lPermissionsFor.Size = new System.Drawing.Size(122, 23);
            this.m_lPermissionsFor.TabIndex = 56;
            this.m_lPermissionsFor.Text = "Set Permissions For:";
            this.m_lPermissionsFor.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // m_cPermissionsFor
            // 
            this.m_cPermissionsFor.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.m_cPermissionsFor.FormattingEnabled = true;
            this.m_cPermissionsFor.Location = new System.Drawing.Point(133, 9);
            this.m_cPermissionsFor.Name = "m_cPermissionsFor";
            this.m_cPermissionsFor.Size = new System.Drawing.Size(322, 21);
            this.m_cPermissionsFor.TabIndex = 57;
            this.m_cPermissionsFor.SelectedIndexChanged += new System.EventHandler(this.cmdPermissionsForChanged);
            // 
            // Page_UserEditPermissions
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.Controls.Add(this.m_cPermissionsFor);
            this.Controls.Add(this.m_lPermissionsFor);
            this.Controls.Add(this.m_group);
            this.Name = "Page_UserEditPermissions";
            this.Size = new System.Drawing.Size(468, 343);
            this.Load += new System.EventHandler(this.cmdLoad);
            this.m_group.ResumeLayout(false);
            this.m_group.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PropertyGrid m_gridBookByBook;
        private System.Windows.Forms.Button m_btnFullEditing;
        private System.Windows.Forms.Label m_labelMakeAll;
        private System.Windows.Forms.Button m_btnNotesOnly;
        private System.Windows.Forms.Button m_btnReadOnly;
        private System.Windows.Forms.ComboBox m_comboMembership;
        private System.Windows.Forms.GroupBox m_group;
        private System.Windows.Forms.Label m_lPermissionsFor;
        private System.Windows.Forms.ComboBox m_cPermissionsFor;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label m_lGlobal;
    }
}
