namespace JWTools
{
    partial class JW_Wizard
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(JW_Wizard));
            this.m_btnHelp = new System.Windows.Forms.Button();
            this.m_btnNext = new System.Windows.Forms.Button();
            this.m_panelButtons = new System.Windows.Forms.Panel();
            this.m_btnFinish = new System.Windows.Forms.Button();
            this.m_btnCancel = new System.Windows.Forms.Button();
            this.m_btnPrev = new System.Windows.Forms.Button();
            this.m_panelPicture = new System.Windows.Forms.Panel();
            this.m_panelButtons.SuspendLayout();
            this.SuspendLayout();
            // 
            // m_btnHelp
            // 
            this.m_btnHelp.BackColor = System.Drawing.SystemColors.Control;
            this.m_btnHelp.Image = ((System.Drawing.Image)(resources.GetObject("m_btnHelp.Image")));
            this.m_btnHelp.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.m_btnHelp.Location = new System.Drawing.Point(12, 8);
            this.m_btnHelp.Name = "m_btnHelp";
            this.m_btnHelp.Size = new System.Drawing.Size(75, 23);
            this.m_btnHelp.TabIndex = 101;
            this.m_btnHelp.Text = "Help...";
            this.m_btnHelp.UseVisualStyleBackColor = false;
            this.m_btnHelp.Click += new System.EventHandler(this.cmdHelp);
            // 
            // m_btnNext
            // 
            this.m_btnNext.BackColor = System.Drawing.SystemColors.Control;
            this.m_btnNext.Location = new System.Drawing.Point(497, 8);
            this.m_btnNext.Name = "m_btnNext";
            this.m_btnNext.Size = new System.Drawing.Size(75, 23);
            this.m_btnNext.TabIndex = 100;
            this.m_btnNext.Text = "Next";
            this.m_btnNext.UseVisualStyleBackColor = false;
            this.m_btnNext.Click += new System.EventHandler(this.cmdNextPage);
            // 
            // m_panelButtons
            // 
            this.m_panelButtons.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.m_panelButtons.Controls.Add(this.m_btnFinish);
            this.m_panelButtons.Controls.Add(this.m_btnCancel);
            this.m_panelButtons.Controls.Add(this.m_btnPrev);
            this.m_panelButtons.Controls.Add(this.m_btnNext);
            this.m_panelButtons.Controls.Add(this.m_btnHelp);
            this.m_panelButtons.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.m_panelButtons.Location = new System.Drawing.Point(0, 418);
            this.m_panelButtons.Name = "m_panelButtons";
            this.m_panelButtons.Size = new System.Drawing.Size(594, 40);
            this.m_panelButtons.TabIndex = 102;
            // 
            // m_btnFinish
            // 
            this.m_btnFinish.BackColor = System.Drawing.SystemColors.Control;
            this.m_btnFinish.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.m_btnFinish.Location = new System.Drawing.Point(497, 8);
            this.m_btnFinish.Name = "m_btnFinish";
            this.m_btnFinish.Size = new System.Drawing.Size(75, 23);
            this.m_btnFinish.TabIndex = 106;
            this.m_btnFinish.Text = "Finish";
            this.m_btnFinish.UseVisualStyleBackColor = false;
            this.m_btnFinish.Visible = false;
            this.m_btnFinish.Click += new System.EventHandler(this.cmdFinish);
            // 
            // m_btnCancel
            // 
            this.m_btnCancel.BackColor = System.Drawing.SystemColors.Control;
            this.m_btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.m_btnCancel.Location = new System.Drawing.Point(93, 8);
            this.m_btnCancel.Name = "m_btnCancel";
            this.m_btnCancel.Size = new System.Drawing.Size(75, 23);
            this.m_btnCancel.TabIndex = 105;
            this.m_btnCancel.Text = "Cancel";
            this.m_btnCancel.UseVisualStyleBackColor = false;
            // 
            // m_btnPrev
            // 
            this.m_btnPrev.BackColor = System.Drawing.SystemColors.Control;
            this.m_btnPrev.Location = new System.Drawing.Point(416, 8);
            this.m_btnPrev.Name = "m_btnPrev";
            this.m_btnPrev.Size = new System.Drawing.Size(75, 23);
            this.m_btnPrev.TabIndex = 104;
            this.m_btnPrev.Text = "Previous";
            this.m_btnPrev.UseVisualStyleBackColor = false;
            this.m_btnPrev.Click += new System.EventHandler(this.cmdPreviousPage);
            // 
            // m_panelPicture
            // 
            this.m_panelPicture.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.m_panelPicture.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.m_panelPicture.Dock = System.Windows.Forms.DockStyle.Left;
            this.m_panelPicture.Location = new System.Drawing.Point(0, 0);
            this.m_panelPicture.Name = "m_panelPicture";
            this.m_panelPicture.Size = new System.Drawing.Size(188, 418);
            this.m_panelPicture.TabIndex = 103;
            this.m_panelPicture.Paint += new System.Windows.Forms.PaintEventHandler(this.cmdPaintPicture);
            // 
            // JW_Wizard
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.CancelButton = this.m_btnCancel;
            this.ClientSize = new System.Drawing.Size(594, 458);
            this.Controls.Add(this.m_panelPicture);
            this.Controls.Add(this.m_panelButtons);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "JW_Wizard";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Wizard";
            this.Activated += new System.EventHandler(this.cmdActivated);
            this.Load += new System.EventHandler(this.cmdLoad);
            this.m_panelButtons.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button m_btnHelp;
        private System.Windows.Forms.Button m_btnNext;
        private System.Windows.Forms.Panel m_panelButtons;
        private System.Windows.Forms.Panel m_panelPicture;
        private System.Windows.Forms.Button m_btnCancel;
        private System.Windows.Forms.Button m_btnPrev;
        private System.Windows.Forms.Button m_btnFinish;

    }
}