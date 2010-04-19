namespace OurWord.ToolTips
{
    partial class ReferenceTranslationsTip
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ReferenceTranslationsTip));
            this.m_Reference = new System.Windows.Forms.Label();
            this.m_NoteIcon = new System.Windows.Forms.PictureBox();
            this.m_btnExpandWindow = new System.Windows.Forms.Button();
            this.m_btnClose = new System.Windows.Forms.Button();
            this.m_panelClientArea = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.m_NoteIcon)).BeginInit();
            this.SuspendLayout();
            // 
            // m_Reference
            // 
            this.m_Reference.BackColor = System.Drawing.Color.Transparent;
            this.m_Reference.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_Reference.Location = new System.Drawing.Point(34, 24);
            this.m_Reference.Name = "m_Reference";
            this.m_Reference.Size = new System.Drawing.Size(293, 15);
            this.m_Reference.TabIndex = 19;
            this.m_Reference.Text = "How others translated verse 12";
            this.m_Reference.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // m_NoteIcon
            // 
            this.m_NoteIcon.BackColor = System.Drawing.Color.Transparent;
            this.m_NoteIcon.Image = ((System.Drawing.Image)(resources.GetObject("m_NoteIcon.Image")));
            this.m_NoteIcon.InitialImage = ((System.Drawing.Image)(resources.GetObject("m_NoteIcon.InitialImage")));
            this.m_NoteIcon.Location = new System.Drawing.Point(12, 23);
            this.m_NoteIcon.Name = "m_NoteIcon";
            this.m_NoteIcon.Size = new System.Drawing.Size(23, 16);
            this.m_NoteIcon.TabIndex = 18;
            this.m_NoteIcon.TabStop = false;
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
            this.m_btnExpandWindow.Location = new System.Drawing.Point(333, 23);
            this.m_btnExpandWindow.Name = "m_btnExpandWindow";
            this.m_btnExpandWindow.Size = new System.Drawing.Size(18, 16);
            this.m_btnExpandWindow.TabIndex = 17;
            this.m_btnExpandWindow.TabStop = false;
            this.m_btnExpandWindow.UseVisualStyleBackColor = false;
            this.m_btnExpandWindow.Click += new System.EventHandler(this.cmdExpandToNormalDialogWindow);
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
            this.m_btnClose.Location = new System.Drawing.Point(357, 23);
            this.m_btnClose.Name = "m_btnClose";
            this.m_btnClose.Size = new System.Drawing.Size(18, 16);
            this.m_btnClose.TabIndex = 16;
            this.m_btnClose.TabStop = false;
            this.m_btnClose.UseVisualStyleBackColor = false;
            // 
            // m_panelClientArea
            // 
            this.m_panelClientArea.BackColor = System.Drawing.Color.Cornsilk;
            this.m_panelClientArea.Location = new System.Drawing.Point(14, 52);
            this.m_panelClientArea.Name = "m_panelClientArea";
            this.m_panelClientArea.Size = new System.Drawing.Size(359, 226);
            this.m_panelClientArea.TabIndex = 20;
            // 
            // ReferenceTranslationsTip
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(387, 290);
            this.Controls.Add(this.m_panelClientArea);
            this.Controls.Add(this.m_Reference);
            this.Controls.Add(this.m_NoteIcon);
            this.Controls.Add(this.m_btnExpandWindow);
            this.Controls.Add(this.m_btnClose);
            this.Name = "ReferenceTranslationsTip";
            this.Text = "ReferenceTranslationsTip";
            ((System.ComponentModel.ISupportInitialize)(this.m_NoteIcon)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button m_btnExpandWindow;
        private System.Windows.Forms.Button m_btnClose;
        private System.Windows.Forms.Label m_Reference;
        private System.Windows.Forms.PictureBox m_NoteIcon;
        private System.Windows.Forms.Panel m_panelClientArea;
    }
}