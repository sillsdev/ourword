namespace OurWord.Dialogs
{
    partial class DlgNewCluster
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DlgNewCluster));
            this.m_labelName = new System.Windows.Forms.Label();
            this.m_textName = new System.Windows.Forms.TextBox();
            this.m_groupName = new System.Windows.Forms.GroupBox();
            this.m_labelError = new System.Windows.Forms.Label();
            this.m_radioMyDocuments = new System.Windows.Forms.RadioButton();
            this.m_radioAppData = new System.Windows.Forms.RadioButton();
            this.m_labelMyDocs = new System.Windows.Forms.Label();
            this.m_labelAppData = new System.Windows.Forms.Label();
            this.m_groupLocation = new System.Windows.Forms.GroupBox();
            this.m_btnCancel = new System.Windows.Forms.Button();
            this.m_btnOK = new System.Windows.Forms.Button();
            this.m_groupName.SuspendLayout();
            this.m_groupLocation.SuspendLayout();
            this.SuspendLayout();
            // 
            // m_labelName
            // 
            this.m_labelName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_labelName.Location = new System.Drawing.Point(142, 12);
            this.m_labelName.Name = "m_labelName";
            this.m_labelName.Size = new System.Drawing.Size(284, 62);
            this.m_labelName.TabIndex = 0;
            this.m_labelName.Text = resources.GetString("m_labelName.Text");
            // 
            // m_textName
            // 
            this.m_textName.Location = new System.Drawing.Point(16, 19);
            this.m_textName.Name = "m_textName";
            this.m_textName.Size = new System.Drawing.Size(120, 20);
            this.m_textName.TabIndex = 1;
            // 
            // m_groupName
            // 
            this.m_groupName.Controls.Add(this.m_labelError);
            this.m_groupName.Controls.Add(this.m_textName);
            this.m_groupName.Controls.Add(this.m_labelName);
            this.m_groupName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_groupName.Location = new System.Drawing.Point(12, 12);
            this.m_groupName.Name = "m_groupName";
            this.m_groupName.Size = new System.Drawing.Size(432, 93);
            this.m_groupName.TabIndex = 2;
            this.m_groupName.TabStop = false;
            this.m_groupName.Text = "Cluster Name";
            // 
            // m_labelError
            // 
            this.m_labelError.AutoSize = true;
            this.m_labelError.ForeColor = System.Drawing.Color.Red;
            this.m_labelError.Location = new System.Drawing.Point(13, 74);
            this.m_labelError.Name = "m_labelError";
            this.m_labelError.Size = new System.Drawing.Size(0, 13);
            this.m_labelError.TabIndex = 2;
            // 
            // m_radioMyDocuments
            // 
            this.m_radioMyDocuments.AutoSize = true;
            this.m_radioMyDocuments.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_radioMyDocuments.Location = new System.Drawing.Point(16, 15);
            this.m_radioMyDocuments.Name = "m_radioMyDocuments";
            this.m_radioMyDocuments.Size = new System.Drawing.Size(96, 17);
            this.m_radioMyDocuments.TabIndex = 3;
            this.m_radioMyDocuments.TabStop = true;
            this.m_radioMyDocuments.Text = "My Documents";
            this.m_radioMyDocuments.UseVisualStyleBackColor = true;
            // 
            // m_radioAppData
            // 
            this.m_radioAppData.AutoSize = true;
            this.m_radioAppData.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_radioAppData.Location = new System.Drawing.Point(16, 61);
            this.m_radioAppData.Name = "m_radioAppData";
            this.m_radioAppData.Size = new System.Drawing.Size(120, 17);
            this.m_radioAppData.TabIndex = 4;
            this.m_radioAppData.TabStop = true;
            this.m_radioAppData.Text = "My Application Data";
            this.m_radioAppData.UseVisualStyleBackColor = true;
            // 
            // m_labelMyDocs
            // 
            this.m_labelMyDocs.Location = new System.Drawing.Point(154, 145);
            this.m_labelMyDocs.Name = "m_labelMyDocs";
            this.m_labelMyDocs.Size = new System.Drawing.Size(284, 32);
            this.m_labelMyDocs.TabIndex = 5;
            this.m_labelMyDocs.Text = "Place the cluster in your Documents folder if you want it to be readily visible a" +
                "nd available.";
            // 
            // m_labelAppData
            // 
            this.m_labelAppData.Location = new System.Drawing.Point(154, 191);
            this.m_labelAppData.Name = "m_labelAppData";
            this.m_labelAppData.Size = new System.Drawing.Size(284, 72);
            this.m_labelAppData.TabIndex = 6;
            this.m_labelAppData.Text = resources.GetString("m_labelAppData.Text");
            // 
            // m_groupLocation
            // 
            this.m_groupLocation.Controls.Add(this.m_radioMyDocuments);
            this.m_groupLocation.Controls.Add(this.m_radioAppData);
            this.m_groupLocation.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_groupLocation.Location = new System.Drawing.Point(12, 130);
            this.m_groupLocation.Name = "m_groupLocation";
            this.m_groupLocation.Size = new System.Drawing.Size(432, 148);
            this.m_groupLocation.TabIndex = 7;
            this.m_groupLocation.TabStop = false;
            this.m_groupLocation.Text = "Folder Location";
            // 
            // m_btnCancel
            // 
            this.m_btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.m_btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.m_btnCancel.Location = new System.Drawing.Point(229, 301);
            this.m_btnCancel.Name = "m_btnCancel";
            this.m_btnCancel.Size = new System.Drawing.Size(75, 23);
            this.m_btnCancel.TabIndex = 9;
            this.m_btnCancel.Text = "Cancel";
            // 
            // m_btnOK
            // 
            this.m_btnOK.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.m_btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.m_btnOK.Location = new System.Drawing.Point(141, 301);
            this.m_btnOK.Name = "m_btnOK";
            this.m_btnOK.Size = new System.Drawing.Size(75, 23);
            this.m_btnOK.TabIndex = 8;
            this.m_btnOK.Text = "OK";
            // 
            // DlgNewCluster
            // 
            this.AcceptButton = this.m_btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.m_btnCancel;
            this.ClientSize = new System.Drawing.Size(456, 336);
            this.Controls.Add(this.m_btnCancel);
            this.Controls.Add(this.m_btnOK);
            this.Controls.Add(this.m_labelAppData);
            this.Controls.Add(this.m_labelMyDocs);
            this.Controls.Add(this.m_groupName);
            this.Controls.Add(this.m_groupLocation);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DlgNewCluster";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Create New Cluster";
            this.Load += new System.EventHandler(this.cmdLoad);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.cmdFormClosing);
            this.m_groupName.ResumeLayout(false);
            this.m_groupName.PerformLayout();
            this.m_groupLocation.ResumeLayout(false);
            this.m_groupLocation.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label m_labelName;
        private System.Windows.Forms.TextBox m_textName;
        private System.Windows.Forms.GroupBox m_groupName;
        private System.Windows.Forms.RadioButton m_radioMyDocuments;
        private System.Windows.Forms.RadioButton m_radioAppData;
        private System.Windows.Forms.Label m_labelMyDocs;
        private System.Windows.Forms.Label m_labelAppData;
        private System.Windows.Forms.GroupBox m_groupLocation;
        private System.Windows.Forms.Button m_btnCancel;
        private System.Windows.Forms.Button m_btnOK;
        private System.Windows.Forms.Label m_labelError;
    }
}