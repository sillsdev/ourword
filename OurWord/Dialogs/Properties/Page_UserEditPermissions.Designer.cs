﻿namespace OurWord.Dialogs.Properties
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
            this.m_UserIsNotProjectMember = new OurWord.Dialogs.Properties.Ctrl_UserIsNotProjectMember();
            this.SuspendLayout();
            // 
            // m_gridBookByBook
            // 
            this.m_gridBookByBook.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.m_gridBookByBook.HelpVisible = false;
            this.m_gridBookByBook.Location = new System.Drawing.Point(3, 3);
            this.m_gridBookByBook.Name = "m_gridBookByBook";
            this.m_gridBookByBook.PropertySort = System.Windows.Forms.PropertySort.Categorized;
            this.m_gridBookByBook.Size = new System.Drawing.Size(462, 295);
            this.m_gridBookByBook.TabIndex = 47;
            this.m_gridBookByBook.ToolbarVisible = false;
            // 
            // m_btnFullEditing
            // 
            this.m_btnFullEditing.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.m_btnFullEditing.Location = new System.Drawing.Point(137, 304);
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
            this.m_labelMakeAll.Location = new System.Drawing.Point(50, 304);
            this.m_labelMakeAll.Name = "m_labelMakeAll";
            this.m_labelMakeAll.Size = new System.Drawing.Size(81, 23);
            this.m_labelMakeAll.TabIndex = 49;
            this.m_labelMakeAll.Text = "Make all:";
            this.m_labelMakeAll.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // m_btnNotesOnly
            // 
            this.m_btnNotesOnly.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.m_btnNotesOnly.Location = new System.Drawing.Point(223, 304);
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
            this.m_btnReadOnly.Location = new System.Drawing.Point(309, 304);
            this.m_btnReadOnly.Name = "m_btnReadOnly";
            this.m_btnReadOnly.Size = new System.Drawing.Size(80, 23);
            this.m_btnReadOnly.TabIndex = 51;
            this.m_btnReadOnly.Text = "Read Only";
            this.m_btnReadOnly.UseVisualStyleBackColor = true;
            this.m_btnReadOnly.Click += new System.EventHandler(this.cmdMakeAllReadOnly);
            // 
            // m_UserIsNotProjectMember
            // 
            this.m_UserIsNotProjectMember.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.m_UserIsNotProjectMember.BackColor = System.Drawing.Color.Cornsilk;
            this.m_UserIsNotProjectMember.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.m_UserIsNotProjectMember.Location = new System.Drawing.Point(97, 63);
            this.m_UserIsNotProjectMember.Name = "m_UserIsNotProjectMember";
            this.m_UserIsNotProjectMember.Size = new System.Drawing.Size(282, 176);
            this.m_UserIsNotProjectMember.TabIndex = 52;
            // 
            // Page_UserEditPermissions
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.Controls.Add(this.m_UserIsNotProjectMember);
            this.Controls.Add(this.m_btnReadOnly);
            this.Controls.Add(this.m_btnNotesOnly);
            this.Controls.Add(this.m_labelMakeAll);
            this.Controls.Add(this.m_btnFullEditing);
            this.Controls.Add(this.m_gridBookByBook);
            this.Name = "Page_UserEditPermissions";
            this.Size = new System.Drawing.Size(468, 330);
            this.Load += new System.EventHandler(this.cmdLoad);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PropertyGrid m_gridBookByBook;
        private System.Windows.Forms.Button m_btnFullEditing;
        private System.Windows.Forms.Label m_labelMakeAll;
        private System.Windows.Forms.Button m_btnNotesOnly;
        private System.Windows.Forms.Button m_btnReadOnly;
        private Ctrl_UserIsNotProjectMember m_UserIsNotProjectMember;
    }
}