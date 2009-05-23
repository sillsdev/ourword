namespace OurWord.Dialogs
{
    partial class WizRepo_GetRepoInfo
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
            this.m_labelUrl = new System.Windows.Forms.Label();
            this.m_textURL = new System.Windows.Forms.TextBox();
            this.m_descrURL = new System.Windows.Forms.Label();
            this.m_labelErrorMsg = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.m_textUserName = new System.Windows.Forms.TextBox();
            this.m_labelUserName = new System.Windows.Forms.Label();
            this.m_textPassword = new System.Windows.Forms.TextBox();
            this.m_labelPassword = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // m_labelUrl
            // 
            this.m_labelUrl.AutoSize = true;
            this.m_labelUrl.Location = new System.Drawing.Point(13, 22);
            this.m_labelUrl.Name = "m_labelUrl";
            this.m_labelUrl.Size = new System.Drawing.Size(85, 13);
            this.m_labelUrl.TabIndex = 0;
            this.m_labelUrl.Text = "Repository URL:";
            // 
            // m_textURL
            // 
            this.m_textURL.Location = new System.Drawing.Point(117, 19);
            this.m_textURL.Name = "m_textURL";
            this.m_textURL.Size = new System.Drawing.Size(241, 20);
            this.m_textURL.TabIndex = 1;
            // 
            // m_descrURL
            // 
            this.m_descrURL.Location = new System.Drawing.Point(31, 42);
            this.m_descrURL.Name = "m_descrURL";
            this.m_descrURL.Size = new System.Drawing.Size(327, 46);
            this.m_descrURL.TabIndex = 2;
            this.m_descrURL.Text = "The location on the Internet for the repository. Do not enter \"http\". For example" +
                ", \"hg.mx.languagedepot.org/MyProject\".";
            // 
            // m_labelErrorMsg
            // 
            this.m_labelErrorMsg.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_labelErrorMsg.ForeColor = System.Drawing.Color.Red;
            this.m_labelErrorMsg.Location = new System.Drawing.Point(13, 272);
            this.m_labelErrorMsg.Name = "m_labelErrorMsg";
            this.m_labelErrorMsg.Size = new System.Drawing.Size(345, 23);
            this.m_labelErrorMsg.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(31, 149);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(327, 52);
            this.label1.TabIndex = 6;
            this.label1.Text = "Your project administrator should provide you with a unique User Name and Passwor" +
                "d for accessing the repository. Without them, you will not be able to transfer d" +
                "ata to/from the repository.";
            // 
            // m_textUserName
            // 
            this.m_textUserName.Location = new System.Drawing.Point(117, 102);
            this.m_textUserName.Name = "m_textUserName";
            this.m_textUserName.Size = new System.Drawing.Size(241, 20);
            this.m_textUserName.TabIndex = 5;
            // 
            // m_labelUserName
            // 
            this.m_labelUserName.AutoSize = true;
            this.m_labelUserName.Location = new System.Drawing.Point(13, 105);
            this.m_labelUserName.Name = "m_labelUserName";
            this.m_labelUserName.Size = new System.Drawing.Size(88, 13);
            this.m_labelUserName.TabIndex = 4;
            this.m_labelUserName.Text = "Your User Name:";
            // 
            // m_textPassword
            // 
            this.m_textPassword.Location = new System.Drawing.Point(117, 126);
            this.m_textPassword.Name = "m_textPassword";
            this.m_textPassword.Size = new System.Drawing.Size(241, 20);
            this.m_textPassword.TabIndex = 8;
            // 
            // m_labelPassword
            // 
            this.m_labelPassword.AutoSize = true;
            this.m_labelPassword.Location = new System.Drawing.Point(13, 129);
            this.m_labelPassword.Name = "m_labelPassword";
            this.m_labelPassword.Size = new System.Drawing.Size(81, 13);
            this.m_labelPassword.TabIndex = 7;
            this.m_labelPassword.Text = "Your Password:";
            // 
            // WizRepo_GetRepoInfo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.m_textPassword);
            this.Controls.Add(this.m_labelPassword);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.m_textUserName);
            this.Controls.Add(this.m_labelUserName);
            this.Controls.Add(this.m_labelErrorMsg);
            this.Controls.Add(this.m_descrURL);
            this.Controls.Add(this.m_textURL);
            this.Controls.Add(this.m_labelUrl);
            this.Name = "WizRepo_GetRepoInfo";
            this.Size = new System.Drawing.Size(372, 306);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label m_labelUrl;
        private System.Windows.Forms.TextBox m_textURL;
        private System.Windows.Forms.Label m_descrURL;
        private System.Windows.Forms.Label m_labelErrorMsg;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox m_textUserName;
        private System.Windows.Forms.Label m_labelUserName;
        private System.Windows.Forms.TextBox m_textPassword;
        private System.Windows.Forms.Label m_labelPassword;
    }
}
