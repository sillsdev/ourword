﻿namespace OurWord.Dialogs
{
    partial class Page_Collaboration
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
            this.m_LiterateSettings = new OurWord.Edit.LiterateSettingsWnd();
            this.SuspendLayout();
            // 
            // m_LiterateSettings
            // 
            this.m_LiterateSettings.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.m_LiterateSettings.DontAllowPropertyGrid = false;
            this.m_LiterateSettings.Location = new System.Drawing.Point(3, 3);
            this.m_LiterateSettings.Name = "m_LiterateSettings";
            this.m_LiterateSettings.ShowDocumentation = true;
            this.m_LiterateSettings.Size = new System.Drawing.Size(444, 351);
            this.m_LiterateSettings.TabIndex = 0;
            // 
            // Page_Collaboration
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.Controls.Add(this.m_LiterateSettings);
            this.Name = "Page_Collaboration";
            this.Size = new System.Drawing.Size(450, 357);
            this.Load += new System.EventHandler(this.cmdLoad);
            this.ResumeLayout(false);

        }

        #endregion

        private OurWord.Edit.LiterateSettingsWnd m_LiterateSettings;
    }
}