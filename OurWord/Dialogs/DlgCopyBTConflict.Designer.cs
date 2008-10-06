namespace OurWord.Dialogs
{
    partial class DialogCopyBTConflict
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
            this.m_labelInfo = new System.Windows.Forms.Label();
            this.m_labelFrontVernacular = new System.Windows.Forms.Label();
            this.m_labelFrontBT = new System.Windows.Forms.Label();
            this.m_textFrontVernacular = new System.Windows.Forms.TextBox();
            this.m_textFrontBT = new System.Windows.Forms.TextBox();
            this.m_btnKeepTarget = new System.Windows.Forms.Button();
            this.m_btnReplaceWithFront = new System.Windows.Forms.Button();
            this.m_btnAppendFrontToTarget = new System.Windows.Forms.Button();
            this.m_checkDoAll = new System.Windows.Forms.CheckBox();
            this.m_groupFront = new System.Windows.Forms.GroupBox();
            this.m_textTargetBT = new System.Windows.Forms.TextBox();
            this.m_textTargetVernacular = new System.Windows.Forms.TextBox();
            this.m_labelTargetBT = new System.Windows.Forms.Label();
            this.m_labelTargetVernacular = new System.Windows.Forms.Label();
            this.m_groupTarget = new System.Windows.Forms.GroupBox();
            this.m_btnCancel = new System.Windows.Forms.Button();
            this.m_groupFront.SuspendLayout();
            this.m_groupTarget.SuspendLayout();
            this.SuspendLayout();
            // 
            // m_labelInfo
            // 
            this.m_labelInfo.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_labelInfo.Location = new System.Drawing.Point(12, 9);
            this.m_labelInfo.Name = "m_labelInfo";
            this.m_labelInfo.Size = new System.Drawing.Size(476, 23);
            this.m_labelInfo.TabIndex = 0;
            this.m_labelInfo.Text = "A Back Translation already exists for this paragraph";
            this.m_labelInfo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // m_labelFrontVernacular
            // 
            this.m_labelFrontVernacular.Location = new System.Drawing.Point(3, 14);
            this.m_labelFrontVernacular.Name = "m_labelFrontVernacular";
            this.m_labelFrontVernacular.Size = new System.Drawing.Size(256, 19);
            this.m_labelFrontVernacular.TabIndex = 1;
            this.m_labelFrontVernacular.Text = "Vernacular";
            this.m_labelFrontVernacular.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // m_labelFrontBT
            // 
            this.m_labelFrontBT.Location = new System.Drawing.Point(3, 226);
            this.m_labelFrontBT.Name = "m_labelFrontBT";
            this.m_labelFrontBT.Size = new System.Drawing.Size(256, 21);
            this.m_labelFrontBT.TabIndex = 2;
            this.m_labelFrontBT.Text = "Proposed Back Translation";
            this.m_labelFrontBT.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // m_textFrontVernacular
            // 
            this.m_textFrontVernacular.BackColor = System.Drawing.Color.LightYellow;
            this.m_textFrontVernacular.Location = new System.Drawing.Point(21, 71);
            this.m_textFrontVernacular.Multiline = true;
            this.m_textFrontVernacular.Name = "m_textFrontVernacular";
            this.m_textFrontVernacular.ReadOnly = true;
            this.m_textFrontVernacular.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.m_textFrontVernacular.Size = new System.Drawing.Size(262, 187);
            this.m_textFrontVernacular.TabIndex = 3;
            this.m_textFrontVernacular.TabStop = false;
            // 
            // m_textFrontBT
            // 
            this.m_textFrontBT.BackColor = System.Drawing.Color.LightYellow;
            this.m_textFrontBT.Location = new System.Drawing.Point(6, 250);
            this.m_textFrontBT.Multiline = true;
            this.m_textFrontBT.Name = "m_textFrontBT";
            this.m_textFrontBT.ReadOnly = true;
            this.m_textFrontBT.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.m_textFrontBT.Size = new System.Drawing.Size(262, 213);
            this.m_textFrontBT.TabIndex = 4;
            this.m_textFrontBT.TabStop = false;
            // 
            // m_btnKeepTarget
            // 
            this.m_btnKeepTarget.Location = new System.Drawing.Point(604, 12);
            this.m_btnKeepTarget.Name = "m_btnKeepTarget";
            this.m_btnKeepTarget.Size = new System.Drawing.Size(135, 86);
            this.m_btnKeepTarget.TabIndex = 5;
            this.m_btnKeepTarget.Text = "Do Nothing (Keep the Existing)";
            this.m_btnKeepTarget.UseVisualStyleBackColor = true;
            this.m_btnKeepTarget.Click += new System.EventHandler(this.cmdKeepExisting);
            // 
            // m_btnReplaceWithFront
            // 
            this.m_btnReplaceWithFront.Location = new System.Drawing.Point(604, 104);
            this.m_btnReplaceWithFront.Name = "m_btnReplaceWithFront";
            this.m_btnReplaceWithFront.Size = new System.Drawing.Size(135, 86);
            this.m_btnReplaceWithFront.TabIndex = 6;
            this.m_btnReplaceWithFront.Text = "Replace the Target with the Front";
            this.m_btnReplaceWithFront.UseVisualStyleBackColor = true;
            this.m_btnReplaceWithFront.Click += new System.EventHandler(this.cmdReplaceTargetWithFront);
            // 
            // m_btnAppendFrontToTarget
            // 
            this.m_btnAppendFrontToTarget.Location = new System.Drawing.Point(604, 196);
            this.m_btnAppendFrontToTarget.Name = "m_btnAppendFrontToTarget";
            this.m_btnAppendFrontToTarget.Size = new System.Drawing.Size(135, 86);
            this.m_btnAppendFrontToTarget.TabIndex = 7;
            this.m_btnAppendFrontToTarget.Text = "Add the Front to the Target";
            this.m_btnAppendFrontToTarget.UseVisualStyleBackColor = true;
            this.m_btnAppendFrontToTarget.Click += new System.EventHandler(this.cmdAppendFrontToTarget);
            // 
            // m_checkDoAll
            // 
            this.m_checkDoAll.Location = new System.Drawing.Point(604, 440);
            this.m_checkDoAll.Name = "m_checkDoAll";
            this.m_checkDoAll.Size = new System.Drawing.Size(135, 58);
            this.m_checkDoAll.TabIndex = 8;
            this.m_checkDoAll.Text = "Do this for any remaining conflicts.";
            this.m_checkDoAll.UseVisualStyleBackColor = true;
            this.m_checkDoAll.CheckedChanged += new System.EventHandler(this.cmdCheckChanged);
            // 
            // m_groupFront
            // 
            this.m_groupFront.Controls.Add(this.m_textFrontBT);
            this.m_groupFront.Controls.Add(this.m_labelFrontBT);
            this.m_groupFront.Controls.Add(this.m_labelFrontVernacular);
            this.m_groupFront.Location = new System.Drawing.Point(15, 35);
            this.m_groupFront.Name = "m_groupFront";
            this.m_groupFront.Size = new System.Drawing.Size(274, 469);
            this.m_groupFront.TabIndex = 9;
            this.m_groupFront.TabStop = false;
            this.m_groupFront.Text = "Front";
            // 
            // m_textTargetBT
            // 
            this.m_textTargetBT.BackColor = System.Drawing.Color.LightYellow;
            this.m_textTargetBT.Location = new System.Drawing.Point(6, 250);
            this.m_textTargetBT.Multiline = true;
            this.m_textTargetBT.Name = "m_textTargetBT";
            this.m_textTargetBT.ReadOnly = true;
            this.m_textTargetBT.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.m_textTargetBT.Size = new System.Drawing.Size(281, 213);
            this.m_textTargetBT.TabIndex = 13;
            this.m_textTargetBT.TabStop = false;
            // 
            // m_textTargetVernacular
            // 
            this.m_textTargetVernacular.BackColor = System.Drawing.Color.LightYellow;
            this.m_textTargetVernacular.Location = new System.Drawing.Point(6, 36);
            this.m_textTargetVernacular.Multiline = true;
            this.m_textTargetVernacular.Name = "m_textTargetVernacular";
            this.m_textTargetVernacular.ReadOnly = true;
            this.m_textTargetVernacular.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.m_textTargetVernacular.Size = new System.Drawing.Size(281, 187);
            this.m_textTargetVernacular.TabIndex = 12;
            this.m_textTargetVernacular.TabStop = false;
            // 
            // m_labelTargetBT
            // 
            this.m_labelTargetBT.Location = new System.Drawing.Point(6, 226);
            this.m_labelTargetBT.Name = "m_labelTargetBT";
            this.m_labelTargetBT.Size = new System.Drawing.Size(276, 21);
            this.m_labelTargetBT.TabIndex = 11;
            this.m_labelTargetBT.Text = "Existing Back Translation";
            this.m_labelTargetBT.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // m_labelTargetVernacular
            // 
            this.m_labelTargetVernacular.Location = new System.Drawing.Point(6, 14);
            this.m_labelTargetVernacular.Name = "m_labelTargetVernacular";
            this.m_labelTargetVernacular.Size = new System.Drawing.Size(273, 19);
            this.m_labelTargetVernacular.TabIndex = 10;
            this.m_labelTargetVernacular.Text = "Vernacular";
            this.m_labelTargetVernacular.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // m_groupTarget
            // 
            this.m_groupTarget.Controls.Add(this.m_labelTargetVernacular);
            this.m_groupTarget.Controls.Add(this.m_textTargetVernacular);
            this.m_groupTarget.Controls.Add(this.m_textTargetBT);
            this.m_groupTarget.Controls.Add(this.m_labelTargetBT);
            this.m_groupTarget.Location = new System.Drawing.Point(295, 35);
            this.m_groupTarget.Name = "m_groupTarget";
            this.m_groupTarget.Size = new System.Drawing.Size(293, 469);
            this.m_groupTarget.TabIndex = 14;
            this.m_groupTarget.TabStop = false;
            this.m_groupTarget.Text = "Target";
            // 
            // m_btnCancel
            // 
            this.m_btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.m_btnCancel.Location = new System.Drawing.Point(604, 288);
            this.m_btnCancel.Name = "m_btnCancel";
            this.m_btnCancel.Size = new System.Drawing.Size(135, 86);
            this.m_btnCancel.TabIndex = 15;
            this.m_btnCancel.Text = "Cancel Copying";
            this.m_btnCancel.UseVisualStyleBackColor = true;
            this.m_btnCancel.Click += new System.EventHandler(this.cmdCancel);
            // 
            // DialogCopyBTConflict
            // 
            this.AcceptButton = this.m_btnAppendFrontToTarget;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.m_btnCancel;
            this.ClientSize = new System.Drawing.Size(751, 516);
            this.Controls.Add(this.m_btnCancel);
            this.Controls.Add(this.m_groupTarget);
            this.Controls.Add(this.m_checkDoAll);
            this.Controls.Add(this.m_btnAppendFrontToTarget);
            this.Controls.Add(this.m_btnReplaceWithFront);
            this.Controls.Add(this.m_btnKeepTarget);
            this.Controls.Add(this.m_textFrontVernacular);
            this.Controls.Add(this.m_labelInfo);
            this.Controls.Add(this.m_groupFront);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DialogCopyBTConflict";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Copying the Back Translation from the Front Translation";
            this.Load += new System.EventHandler(this.cmdLoad);
            this.m_groupFront.ResumeLayout(false);
            this.m_groupFront.PerformLayout();
            this.m_groupTarget.ResumeLayout(false);
            this.m_groupTarget.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label m_labelInfo;
        private System.Windows.Forms.Label m_labelFrontVernacular;
        private System.Windows.Forms.Label m_labelFrontBT;
        private System.Windows.Forms.TextBox m_textFrontVernacular;
        private System.Windows.Forms.TextBox m_textFrontBT;
        private System.Windows.Forms.Button m_btnKeepTarget;
        private System.Windows.Forms.Button m_btnReplaceWithFront;
        private System.Windows.Forms.Button m_btnAppendFrontToTarget;
        private System.Windows.Forms.CheckBox m_checkDoAll;
        private System.Windows.Forms.GroupBox m_groupFront;
        private System.Windows.Forms.TextBox m_textTargetBT;
        private System.Windows.Forms.TextBox m_textTargetVernacular;
        private System.Windows.Forms.Label m_labelTargetBT;
        private System.Windows.Forms.Label m_labelTargetVernacular;
        private System.Windows.Forms.GroupBox m_groupTarget;
        private System.Windows.Forms.Button m_btnCancel;
    }
}