namespace OurWord.ToolTips
{
    partial class EditableNoteTip
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EditableNoteTip));
            this.m_panelClientArea = new System.Windows.Forms.Panel();
            this.m_NoteIcon = new System.Windows.Forms.PictureBox();
            this.m_toolStrip = new System.Windows.Forms.ToolStrip();
            this.m_btnCut = new System.Windows.Forms.ToolStripButton();
            this.m_btnCopy = new System.Windows.Forms.ToolStripButton();
            this.m_btnPaste = new System.Windows.Forms.ToolStripButton();
            this.m_btnItalics = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.m_AssignedTo = new System.Windows.Forms.ToolStripDropDownButton();
            this.m_Reference = new System.Windows.Forms.Label();
            this.m_Title = new System.Windows.Forms.TextBox();
            this.m_btnClose = new System.Windows.Forms.Button();
            this.m_btnExpandWindow = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.m_NoteIcon)).BeginInit();
            this.m_toolStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // m_panelClientArea
            // 
            this.m_panelClientArea.BackColor = System.Drawing.Color.Cornsilk;
            this.m_panelClientArea.Location = new System.Drawing.Point(12, 76);
            this.m_panelClientArea.Name = "m_panelClientArea";
            this.m_panelClientArea.Size = new System.Drawing.Size(359, 187);
            this.m_panelClientArea.TabIndex = 7;
            // 
            // m_NoteIcon
            // 
            this.m_NoteIcon.BackColor = System.Drawing.Color.Transparent;
            this.m_NoteIcon.Image = ((System.Drawing.Image)(resources.GetObject("m_NoteIcon.Image")));
            this.m_NoteIcon.InitialImage = ((System.Drawing.Image)(resources.GetObject("m_NoteIcon.InitialImage")));
            this.m_NoteIcon.Location = new System.Drawing.Point(12, 29);
            this.m_NoteIcon.Name = "m_NoteIcon";
            this.m_NoteIcon.Size = new System.Drawing.Size(23, 16);
            this.m_NoteIcon.TabIndex = 5;
            this.m_NoteIcon.TabStop = false;
            // 
            // m_toolStrip
            // 
            this.m_toolStrip.BackColor = System.Drawing.Color.Transparent;
            this.m_toolStrip.Dock = System.Windows.Forms.DockStyle.None;
            this.m_toolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.m_toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_btnCut,
            this.m_btnCopy,
            this.m_btnPaste,
            this.m_btnItalics,
            this.toolStripSeparator1,
            this.m_AssignedTo});
            this.m_toolStrip.Location = new System.Drawing.Point(12, 48);
            this.m_toolStrip.Name = "m_toolStrip";
            this.m_toolStrip.Size = new System.Drawing.Size(192, 25);
            this.m_toolStrip.TabIndex = 11;
            this.m_toolStrip.Text = "toolStrip1";
            // 
            // m_btnCut
            // 
            this.m_btnCut.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.m_btnCut.Image = ((System.Drawing.Image)(resources.GetObject("m_btnCut.Image")));
            this.m_btnCut.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.m_btnCut.Name = "m_btnCut";
            this.m_btnCut.Size = new System.Drawing.Size(23, 22);
            this.m_btnCut.Text = "Cut";
            this.m_btnCut.Click += new System.EventHandler(this.cmdCut);
            // 
            // m_btnCopy
            // 
            this.m_btnCopy.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.m_btnCopy.Image = ((System.Drawing.Image)(resources.GetObject("m_btnCopy.Image")));
            this.m_btnCopy.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.m_btnCopy.Name = "m_btnCopy";
            this.m_btnCopy.Size = new System.Drawing.Size(23, 22);
            this.m_btnCopy.Text = "Copy";
            this.m_btnCopy.Click += new System.EventHandler(this.cmdCopy);
            // 
            // m_btnPaste
            // 
            this.m_btnPaste.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.m_btnPaste.Image = ((System.Drawing.Image)(resources.GetObject("m_btnPaste.Image")));
            this.m_btnPaste.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.m_btnPaste.Name = "m_btnPaste";
            this.m_btnPaste.Size = new System.Drawing.Size(23, 22);
            this.m_btnPaste.Text = "Paste";
            this.m_btnPaste.Click += new System.EventHandler(this.cmdPaste);
            // 
            // m_btnItalics
            // 
            this.m_btnItalics.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.m_btnItalics.Image = ((System.Drawing.Image)(resources.GetObject("m_btnItalics.Image")));
            this.m_btnItalics.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.m_btnItalics.Name = "m_btnItalics";
            this.m_btnItalics.Size = new System.Drawing.Size(23, 22);
            this.m_btnItalics.Text = "Italics";
            this.m_btnItalics.Click += new System.EventHandler(this.cmdItalics);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // m_AssignedTo
            // 
            this.m_AssignedTo.BackColor = System.Drawing.Color.Transparent;
            this.m_AssignedTo.Image = ((System.Drawing.Image)(resources.GetObject("m_AssignedTo.Image")));
            this.m_AssignedTo.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.m_AssignedTo.Name = "m_AssignedTo";
            this.m_AssignedTo.Size = new System.Drawing.Size(91, 22);
            this.m_AssignedTo.Text = "(assign to)";
            // 
            // m_Reference
            // 
            this.m_Reference.BackColor = System.Drawing.Color.Transparent;
            this.m_Reference.Font = new System.Drawing.Font("Arial Narrow", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_Reference.Location = new System.Drawing.Point(34, 30);
            this.m_Reference.Name = "m_Reference";
            this.m_Reference.Size = new System.Drawing.Size(43, 15);
            this.m_Reference.TabIndex = 12;
            this.m_Reference.Text = "119:176:";
            this.m_Reference.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // m_Title
            // 
            this.m_Title.BackColor = System.Drawing.Color.Pink;
            this.m_Title.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.m_Title.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_Title.Location = new System.Drawing.Point(85, 30);
            this.m_Title.Name = "m_Title";
            this.m_Title.Size = new System.Drawing.Size(238, 15);
            this.m_Title.TabIndex = 13;
            this.m_Title.Text = "<Title>";
            this.m_Title.TextChanged += new System.EventHandler(this.cmdTitleTextChanged);
            // 
            // m_btnClose
            // 
            this.m_btnClose.BackColor = System.Drawing.Color.Transparent;
            this.m_btnClose.CausesValidation = false;
            this.m_btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.m_btnClose.FlatAppearance.BorderSize = 0;
            this.m_btnClose.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.m_btnClose.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.m_btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.m_btnClose.Image = ((System.Drawing.Image)(resources.GetObject("m_btnClose.Image")));
            this.m_btnClose.Location = new System.Drawing.Point(353, 26);
            this.m_btnClose.Name = "m_btnClose";
            this.m_btnClose.Size = new System.Drawing.Size(18, 16);
            this.m_btnClose.TabIndex = 14;
            this.m_btnClose.TabStop = false;
            this.m_btnClose.UseVisualStyleBackColor = false;
            this.m_btnClose.Click += new System.EventHandler(this.cmdClose);
            // 
            // m_btnExpandWindow
            // 
            this.m_btnExpandWindow.BackColor = System.Drawing.Color.Transparent;
            this.m_btnExpandWindow.CausesValidation = false;
            this.m_btnExpandWindow.FlatAppearance.BorderSize = 0;
            this.m_btnExpandWindow.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.m_btnExpandWindow.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.m_btnExpandWindow.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.m_btnExpandWindow.Image = ((System.Drawing.Image)(resources.GetObject("m_btnExpandWindow.Image")));
            this.m_btnExpandWindow.Location = new System.Drawing.Point(329, 26);
            this.m_btnExpandWindow.Name = "m_btnExpandWindow";
            this.m_btnExpandWindow.Size = new System.Drawing.Size(18, 16);
            this.m_btnExpandWindow.TabIndex = 15;
            this.m_btnExpandWindow.TabStop = false;
            this.m_btnExpandWindow.UseVisualStyleBackColor = false;
            this.m_btnExpandWindow.Click += new System.EventHandler(this.cmdExpandToNormalDialogWindow);
            // 
            // EditableNoteTip
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.CancelButton = this.m_btnClose;
            this.CausesValidation = false;
            this.ClientSize = new System.Drawing.Size(383, 275);
            this.Controls.Add(this.m_btnExpandWindow);
            this.Controls.Add(this.m_btnClose);
            this.Controls.Add(this.m_Title);
            this.Controls.Add(this.m_Reference);
            this.Controls.Add(this.m_toolStrip);
            this.Controls.Add(this.m_panelClientArea);
            this.Controls.Add(this.m_NoteIcon);
            this.Name = "EditableNoteTip";
            this.ShowIcon = false;
            this.Shown += new System.EventHandler(this.cmdFirstShown);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.cmdClosing);
            ((System.ComponentModel.ISupportInitialize)(this.m_NoteIcon)).EndInit();
            this.m_toolStrip.ResumeLayout(false);
            this.m_toolStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox m_NoteIcon;
        private System.Windows.Forms.Panel m_panelClientArea;
        private System.Windows.Forms.ToolStrip m_toolStrip;
        private System.Windows.Forms.ToolStripButton m_btnCut;
        private System.Windows.Forms.ToolStripButton m_btnCopy;
        private System.Windows.Forms.ToolStripButton m_btnPaste;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripDropDownButton m_AssignedTo;
        private System.Windows.Forms.Label m_Reference;
        private System.Windows.Forms.TextBox m_Title;
        private System.Windows.Forms.Button m_btnClose;
        private System.Windows.Forms.ToolStripButton m_btnItalics;
        private System.Windows.Forms.Button m_btnExpandWindow;
    }
}
