using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OurWord.Dialogs
{
    partial class CopyBTfromFront
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CopyBTfromFront));
            this.m_btnCancel = new System.Windows.Forms.Button();
            this.m_btnOK = new System.Windows.Forms.Button();
            this.m_WarningIcon = new System.Windows.Forms.PictureBox();
            this.m_labelSynopsis = new System.Windows.Forms.Label();
            this.m_labelProceed = new System.Windows.Forms.Label();
            this.m_labelExp1 = new System.Windows.Forms.Label();
            this.m_labelExp2 = new System.Windows.Forms.Label();
            this.m_labelExp3 = new System.Windows.Forms.Label();
            this.m_labelExp4 = new System.Windows.Forms.Label();
            this.m_labelScope = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.m_WarningIcon)).BeginInit();
            this.SuspendLayout();
            // 
            // m_btnCancel
            // 
            this.m_btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.m_btnCancel.Location = new System.Drawing.Point(242, 381);
            this.m_btnCancel.Name = "m_btnCancel";
            this.m_btnCancel.Size = new System.Drawing.Size(75, 23);
            this.m_btnCancel.TabIndex = 14;
            this.m_btnCancel.Text = "Cancel";
            // 
            // m_btnOK
            // 
            this.m_btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.m_btnOK.Location = new System.Drawing.Point(154, 381);
            this.m_btnOK.Name = "m_btnOK";
            this.m_btnOK.Size = new System.Drawing.Size(75, 23);
            this.m_btnOK.TabIndex = 13;
            this.m_btnOK.Text = "Proceed";
            // 
            // m_WarningIcon
            // 
            this.m_WarningIcon.Location = new System.Drawing.Point(16, 16);
            this.m_WarningIcon.Name = "m_WarningIcon";
            this.m_WarningIcon.Size = new System.Drawing.Size(48, 32);
            this.m_WarningIcon.TabIndex = 16;
            this.m_WarningIcon.TabStop = false;
            // 
            // m_labelSynopsis
            // 
            this.m_labelSynopsis.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_labelSynopsis.Location = new System.Drawing.Point(72, 8);
            this.m_labelSynopsis.Name = "m_labelSynopsis";
            this.m_labelSynopsis.Size = new System.Drawing.Size(392, 40);
            this.m_labelSynopsis.TabIndex = 17;
            this.m_labelSynopsis.Text = "This process will copy the back translations from Kupang Luke to  Amarasi Luke.";
            this.m_labelSynopsis.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // m_labelProceed
            // 
            this.m_labelProceed.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_labelProceed.Location = new System.Drawing.Point(72, 352);
            this.m_labelProceed.Name = "m_labelProceed";
            this.m_labelProceed.Size = new System.Drawing.Size(384, 16);
            this.m_labelProceed.TabIndex = 18;
            this.m_labelProceed.Text = "Do you still wish to proceed?";
            this.m_labelProceed.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // m_labelExp1
            // 
            this.m_labelExp1.Location = new System.Drawing.Point(72, 88);
            this.m_labelExp1.Name = "m_labelExp1";
            this.m_labelExp1.Size = new System.Drawing.Size(392, 56);
            this.m_labelExp1.TabIndex = 19;
            this.m_labelExp1.Text = resources.GetString("m_labelExp1.Text");
            // 
            // m_labelExp2
            // 
            this.m_labelExp2.Location = new System.Drawing.Point(72, 152);
            this.m_labelExp2.Name = "m_labelExp2";
            this.m_labelExp2.Size = new System.Drawing.Size(392, 64);
            this.m_labelExp2.TabIndex = 20;
            this.m_labelExp2.Text = resources.GetString("m_labelExp2.Text");
            // 
            // m_labelExp3
            // 
            this.m_labelExp3.Location = new System.Drawing.Point(72, 216);
            this.m_labelExp3.Name = "m_labelExp3";
            this.m_labelExp3.Size = new System.Drawing.Size(392, 72);
            this.m_labelExp3.TabIndex = 21;
            this.m_labelExp3.Text = resources.GetString("m_labelExp3.Text");
            // 
            // m_labelExp4
            // 
            this.m_labelExp4.Location = new System.Drawing.Point(72, 288);
            this.m_labelExp4.Name = "m_labelExp4";
            this.m_labelExp4.Size = new System.Drawing.Size(392, 64);
            this.m_labelExp4.TabIndex = 22;
            this.m_labelExp4.Text = resources.GetString("m_labelExp4.Text");
            // 
            // m_labelScope
            // 
            this.m_labelScope.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_labelScope.Location = new System.Drawing.Point(72, 56);
            this.m_labelScope.Name = "m_labelScope";
            this.m_labelScope.Size = new System.Drawing.Size(392, 23);
            this.m_labelScope.TabIndex = 23;
            this.m_labelScope.Text = "The Entire Book will be copied.";
            this.m_labelScope.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // CopyBTfromFront
            // 
            this.AcceptButton = this.m_btnOK;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.CancelButton = this.m_btnCancel;
            this.ClientSize = new System.Drawing.Size(474, 416);
            this.Controls.Add(this.m_labelScope);
            this.Controls.Add(this.m_labelExp4);
            this.Controls.Add(this.m_labelExp3);
            this.Controls.Add(this.m_labelExp2);
            this.Controls.Add(this.m_labelExp1);
            this.Controls.Add(this.m_labelProceed);
            this.Controls.Add(this.m_labelSynopsis);
            this.Controls.Add(this.m_WarningIcon);
            this.Controls.Add(this.m_btnCancel);
            this.Controls.Add(this.m_btnOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CopyBTfromFront";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Copy Back Translation";
            this.Load += new System.EventHandler(this.cmdLoad);
            ((System.ComponentModel.ISupportInitialize)(this.m_WarningIcon)).EndInit();
            this.ResumeLayout(false);

        }
        #endregion



        private System.Windows.Forms.Button m_btnCancel;
        private System.Windows.Forms.Button m_btnOK;
        private System.Windows.Forms.PictureBox m_WarningIcon;
        private System.Windows.Forms.Label m_labelSynopsis;
        private System.Windows.Forms.Label m_labelProceed;
        private System.Windows.Forms.Label m_labelExp1;
        private System.Windows.Forms.Label m_labelExp2;
        private System.Windows.Forms.Label m_labelExp3;
        private System.Windows.Forms.Label m_labelExp4;
        private System.Windows.Forms.Label m_labelScope;

    }
}
