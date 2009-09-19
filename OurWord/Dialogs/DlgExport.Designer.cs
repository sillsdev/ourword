namespace OurWord.Dialogs
{
    partial class DialogExport
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DialogExport));
            this.m_btnCancel = new System.Windows.Forms.Button();
            this.m_btnOK = new System.Windows.Forms.Button();
            this.m_radioParatext = new System.Windows.Forms.RadioButton();
            this.m_labelParatextInfo = new System.Windows.Forms.Label();
            this.m_groupExportTo = new System.Windows.Forms.GroupBox();
            this.m_labelGoBibleCreatorInfo = new System.Windows.Forms.Label();
            this.m_radioGoBible = new System.Windows.Forms.RadioButton();
            this.m_labelFolder = new System.Windows.Forms.Label();
            this.m_labelLocation = new System.Windows.Forms.Label();
            this.m_labelToolbox = new System.Windows.Forms.Label();
            this.m_radioToolbox = new System.Windows.Forms.RadioButton();
            this.m_groupExportTo.SuspendLayout();
            this.SuspendLayout();
            // 
            // m_btnCancel
            // 
            this.m_btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.m_btnCancel.Location = new System.Drawing.Point(246, 319);
            this.m_btnCancel.Name = "m_btnCancel";
            this.m_btnCancel.Size = new System.Drawing.Size(75, 23);
            this.m_btnCancel.TabIndex = 17;
            this.m_btnCancel.Text = "Cancel";
            // 
            // m_btnOK
            // 
            this.m_btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.m_btnOK.Location = new System.Drawing.Point(158, 319);
            this.m_btnOK.Name = "m_btnOK";
            this.m_btnOK.Size = new System.Drawing.Size(75, 23);
            this.m_btnOK.TabIndex = 16;
            this.m_btnOK.Text = "Export";
            // 
            // m_radioParatext
            // 
            this.m_radioParatext.AutoSize = true;
            this.m_radioParatext.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_radioParatext.Location = new System.Drawing.Point(21, 19);
            this.m_radioParatext.Name = "m_radioParatext";
            this.m_radioParatext.Size = new System.Drawing.Size(135, 17);
            this.m_radioParatext.TabIndex = 27;
            this.m_radioParatext.TabStop = true;
            this.m_radioParatext.Text = "Paratext (PTX) files";
            this.m_radioParatext.UseVisualStyleBackColor = true;
            this.m_radioParatext.CheckedChanged += new System.EventHandler(this.cmdUpdateLocation);
            // 
            // m_labelParatextInfo
            // 
            this.m_labelParatextInfo.Location = new System.Drawing.Point(50, 39);
            this.m_labelParatextInfo.Name = "m_labelParatextInfo";
            this.m_labelParatextInfo.Size = new System.Drawing.Size(399, 39);
            this.m_labelParatextInfo.TabIndex = 28;
            this.m_labelParatextInfo.Text = "This will only export the vernacular; any back translations or notes will not be " +
                "exported.";
            // 
            // m_groupExportTo
            // 
            this.m_groupExportTo.Controls.Add(this.m_labelToolbox);
            this.m_groupExportTo.Controls.Add(this.m_radioToolbox);
            this.m_groupExportTo.Controls.Add(this.m_labelGoBibleCreatorInfo);
            this.m_groupExportTo.Controls.Add(this.m_radioGoBible);
            this.m_groupExportTo.Controls.Add(this.m_radioParatext);
            this.m_groupExportTo.Controls.Add(this.m_labelParatextInfo);
            this.m_groupExportTo.Location = new System.Drawing.Point(15, 12);
            this.m_groupExportTo.Name = "m_groupExportTo";
            this.m_groupExportTo.Size = new System.Drawing.Size(466, 221);
            this.m_groupExportTo.TabIndex = 30;
            this.m_groupExportTo.TabStop = false;
            this.m_groupExportTo.Text = "Export all of your project\'s books to:";
            // 
            // m_labelGoBibleCreatorInfo
            // 
            this.m_labelGoBibleCreatorInfo.Location = new System.Drawing.Point(50, 101);
            this.m_labelGoBibleCreatorInfo.Name = "m_labelGoBibleCreatorInfo";
            this.m_labelGoBibleCreatorInfo.Size = new System.Drawing.Size(399, 39);
            this.m_labelGoBibleCreatorInfo.TabIndex = 34;
            this.m_labelGoBibleCreatorInfo.Text = "Go Bible is a viewer for Java mobile phones. GoBibleCreator\'s SFM files are a sub" +
                "set of USFM.";
            // 
            // m_radioGoBible
            // 
            this.m_radioGoBible.AutoSize = true;
            this.m_radioGoBible.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_radioGoBible.Location = new System.Drawing.Point(21, 81);
            this.m_radioGoBible.Name = "m_radioGoBible";
            this.m_radioGoBible.Size = new System.Drawing.Size(169, 17);
            this.m_radioGoBible.TabIndex = 33;
            this.m_radioGoBible.TabStop = true;
            this.m_radioGoBible.Text = "GoBibleCreator input files";
            this.m_radioGoBible.UseVisualStyleBackColor = true;
            this.m_radioGoBible.CheckedChanged += new System.EventHandler(this.cmdUpdateLocation);
            // 
            // m_labelFolder
            // 
            this.m_labelFolder.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.m_labelFolder.Location = new System.Drawing.Point(36, 275);
            this.m_labelFolder.Name = "m_labelFolder";
            this.m_labelFolder.Size = new System.Drawing.Size(445, 23);
            this.m_labelFolder.TabIndex = 31;
            this.m_labelFolder.Text = "(folder location)";
            this.m_labelFolder.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // m_labelLocation
            // 
            this.m_labelLocation.Location = new System.Drawing.Point(12, 252);
            this.m_labelLocation.Name = "m_labelLocation";
            this.m_labelLocation.Size = new System.Drawing.Size(469, 23);
            this.m_labelLocation.TabIndex = 32;
            this.m_labelLocation.Text = "The files will be placed here:";
            this.m_labelLocation.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // m_labelToolbox
            // 
            this.m_labelToolbox.Location = new System.Drawing.Point(50, 163);
            this.m_labelToolbox.Name = "m_labelToolbox";
            this.m_labelToolbox.Size = new System.Drawing.Size(399, 39);
            this.m_labelToolbox.TabIndex = 36;
            this.m_labelToolbox.Text = resources.GetString("m_labelToolbox.Text");
            // 
            // m_radioToolbox
            // 
            this.m_radioToolbox.AutoSize = true;
            this.m_radioToolbox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_radioToolbox.Location = new System.Drawing.Point(21, 143);
            this.m_radioToolbox.Name = "m_radioToolbox";
            this.m_radioToolbox.Size = new System.Drawing.Size(126, 17);
            this.m_radioToolbox.TabIndex = 35;
            this.m_radioToolbox.TabStop = true;
            this.m_radioToolbox.Text = "Toolbox (DB) files";
            this.m_radioToolbox.UseVisualStyleBackColor = true;
            // 
            // DialogExport
            // 
            this.AcceptButton = this.m_btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoSize = true;
            this.CancelButton = this.m_btnCancel;
            this.ClientSize = new System.Drawing.Size(493, 357);
            this.ControlBox = false;
            this.Controls.Add(this.m_labelLocation);
            this.Controls.Add(this.m_labelFolder);
            this.Controls.Add(this.m_btnCancel);
            this.Controls.Add(this.m_btnOK);
            this.Controls.Add(this.m_groupExportTo);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DialogExport";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Export Book";
            this.Load += new System.EventHandler(this.cmdLoad);
            this.m_groupExportTo.ResumeLayout(false);
            this.m_groupExportTo.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button m_btnCancel;
        private System.Windows.Forms.Button m_btnOK;
        private System.Windows.Forms.RadioButton m_radioParatext;
        private System.Windows.Forms.Label m_labelParatextInfo;
        private System.Windows.Forms.GroupBox m_groupExportTo;
        private System.Windows.Forms.Label m_labelFolder;
        private System.Windows.Forms.Label m_labelLocation;
        private System.Windows.Forms.Label m_labelGoBibleCreatorInfo;
        private System.Windows.Forms.RadioButton m_radioGoBible;
        private System.Windows.Forms.Label m_labelToolbox;
        private System.Windows.Forms.RadioButton m_radioToolbox;
    }
}