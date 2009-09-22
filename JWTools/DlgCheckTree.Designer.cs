namespace JWTools
{
    partial class DlgCheckTree
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
            this.m_Tree = new System.Windows.Forms.TreeView();
            this.m_lblInstructions = new System.Windows.Forms.Label();
            this.m_btnCancel = new System.Windows.Forms.Button();
            this.m_btnOK = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // m_Tree
            // 
            this.m_Tree.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.m_Tree.CheckBoxes = true;
            this.m_Tree.HideSelection = false;
            this.m_Tree.Location = new System.Drawing.Point(12, 48);
            this.m_Tree.Name = "m_Tree";
            this.m_Tree.ShowNodeToolTips = true;
            this.m_Tree.ShowRootLines = false;
            this.m_Tree.Size = new System.Drawing.Size(321, 334);
            this.m_Tree.TabIndex = 12;
            this.m_Tree.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.cmdItemChecked);
            this.m_Tree.BeforeCollapse += new System.Windows.Forms.TreeViewCancelEventHandler(this.cmdBeforeCollapse);
            // 
            // m_lblInstructions
            // 
            this.m_lblInstructions.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.m_lblInstructions.Location = new System.Drawing.Point(9, 9);
            this.m_lblInstructions.Name = "m_lblInstructions";
            this.m_lblInstructions.Size = new System.Drawing.Size(324, 36);
            this.m_lblInstructions.TabIndex = 13;
            this.m_lblInstructions.Text = "Place a check beside the features you want to turn on.";
            this.m_lblInstructions.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // m_btnCancel
            // 
            this.m_btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.m_btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.m_btnCancel.Location = new System.Drawing.Point(176, 388);
            this.m_btnCancel.Name = "m_btnCancel";
            this.m_btnCancel.Size = new System.Drawing.Size(75, 23);
            this.m_btnCancel.TabIndex = 15;
            this.m_btnCancel.Text = "Cancel";
            // 
            // m_btnOK
            // 
            this.m_btnOK.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.m_btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.m_btnOK.Location = new System.Drawing.Point(95, 388);
            this.m_btnOK.Name = "m_btnOK";
            this.m_btnOK.Size = new System.Drawing.Size(75, 23);
            this.m_btnOK.TabIndex = 14;
            this.m_btnOK.Text = "OK";
            // 
            // DlgCheckTree
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(343, 423);
            this.ControlBox = false;
            this.Controls.Add(this.m_btnCancel);
            this.Controls.Add(this.m_btnOK);
            this.Controls.Add(this.m_lblInstructions);
            this.Controls.Add(this.m_Tree);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DlgCheckTree";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Turn On / Off";
            this.Load += new System.EventHandler(this.cmdLoad);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView m_Tree;
        private System.Windows.Forms.Button m_btnCancel;
        private System.Windows.Forms.Button m_btnOK;
        private System.Windows.Forms.Label m_lblInstructions;
    }
}