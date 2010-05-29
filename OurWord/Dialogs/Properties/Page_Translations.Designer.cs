namespace OurWord.Dialogs.Properties
{
    partial class Page_Translations
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Page_Translations));
            this.m_lTargetAndSource = new System.Windows.Forms.Label();
            this.m_lReference = new System.Windows.Forms.Label();
            this.m_lTargetDefinition = new System.Windows.Forms.Label();
            this.m_bSource = new System.Windows.Forms.Button();
            this.m_bTarget = new System.Windows.Forms.Button();
            this.m_lSource = new System.Windows.Forms.Label();
            this.m_listTranslations = new System.Windows.Forms.CheckedListBox();
            this.m_lOther = new System.Windows.Forms.Label();
            this.m_btnCreate = new System.Windows.Forms.Button();
            this.m_bReference = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // m_lTargetAndSource
            // 
            this.m_lTargetAndSource.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.m_lTargetAndSource.BackColor = System.Drawing.Color.Silver;
            this.m_lTargetAndSource.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_lTargetAndSource.ForeColor = System.Drawing.Color.DarkBlue;
            this.m_lTargetAndSource.Location = new System.Drawing.Point(6, 5);
            this.m_lTargetAndSource.Name = "m_lTargetAndSource";
            this.m_lTargetAndSource.Size = new System.Drawing.Size(454, 22);
            this.m_lTargetAndSource.TabIndex = 48;
            this.m_lTargetAndSource.Text = "Target And Source Translations";
            this.m_lTargetAndSource.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // m_lReference
            // 
            this.m_lReference.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.m_lReference.BackColor = System.Drawing.Color.Silver;
            this.m_lReference.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_lReference.ForeColor = System.Drawing.Color.DarkBlue;
            this.m_lReference.Location = new System.Drawing.Point(6, 130);
            this.m_lReference.Name = "m_lReference";
            this.m_lReference.Size = new System.Drawing.Size(454, 22);
            this.m_lReference.TabIndex = 50;
            this.m_lReference.Text = "Reference Translations";
            this.m_lReference.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // m_lTargetDefinition
            // 
            this.m_lTargetDefinition.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.m_lTargetDefinition.Location = new System.Drawing.Point(229, 30);
            this.m_lTargetDefinition.Name = "m_lTargetDefinition";
            this.m_lTargetDefinition.Size = new System.Drawing.Size(231, 40);
            this.m_lTargetDefinition.TabIndex = 51;
            this.m_lTargetDefinition.Text = "This is the vernacular translation that you are producing. ";
            // 
            // m_bSource
            // 
            this.m_bSource.Image = ((System.Drawing.Image)(resources.GetObject("m_bSource.Image")));
            this.m_bSource.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.m_bSource.Location = new System.Drawing.Point(24, 75);
            this.m_bSource.Name = "m_bSource";
            this.m_bSource.Size = new System.Drawing.Size(199, 27);
            this.m_bSource.TabIndex = 53;
            this.m_bSource.Text = "Edit Source Properties";
            this.m_bSource.UseVisualStyleBackColor = true;
            this.m_bSource.Click += new System.EventHandler(this.cmdEditSourceProperties);
            // 
            // m_bTarget
            // 
            this.m_bTarget.Image = ((System.Drawing.Image)(resources.GetObject("m_bTarget.Image")));
            this.m_bTarget.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.m_bTarget.Location = new System.Drawing.Point(24, 30);
            this.m_bTarget.Name = "m_bTarget";
            this.m_bTarget.Size = new System.Drawing.Size(199, 27);
            this.m_bTarget.TabIndex = 52;
            this.m_bTarget.Text = "Edit Target Properties";
            this.m_bTarget.UseVisualStyleBackColor = true;
            this.m_bTarget.Click += new System.EventHandler(this.cmdEditTargetProperties);
            // 
            // m_lSource
            // 
            this.m_lSource.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.m_lSource.Location = new System.Drawing.Point(232, 75);
            this.m_lSource.Name = "m_lSource";
            this.m_lSource.Size = new System.Drawing.Size(228, 40);
            this.m_lSource.TabIndex = 54;
            this.m_lSource.Text = "This is the front / source translation that you are using as the model for produc" +
                "ing the Target.";
            // 
            // m_listTranslations
            // 
            this.m_listTranslations.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.m_listTranslations.IntegralHeight = false;
            this.m_listTranslations.Location = new System.Drawing.Point(24, 155);
            this.m_listTranslations.Name = "m_listTranslations";
            this.m_listTranslations.Size = new System.Drawing.Size(199, 199);
            this.m_listTranslations.TabIndex = 55;
            this.m_listTranslations.SelectedIndexChanged += new System.EventHandler(this.cmdReferenceSelectionChanged);
            this.m_listTranslations.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.cmdReferenceLanguageChecked);
            // 
            // m_lOther
            // 
            this.m_lOther.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.m_lOther.Location = new System.Drawing.Point(232, 155);
            this.m_lOther.Name = "m_lOther";
            this.m_lOther.Size = new System.Drawing.Size(228, 51);
            this.m_lOther.TabIndex = 56;
            this.m_lOther.Text = "Place a check beside the othe translations you wish to see while translating. The" +
                "y show in a popup when you hover over a verse number.";
            // 
            // m_btnCreate
            // 
            this.m_btnCreate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.m_btnCreate.Location = new System.Drawing.Point(235, 331);
            this.m_btnCreate.Name = "m_btnCreate";
            this.m_btnCreate.Size = new System.Drawing.Size(199, 24);
            this.m_btnCreate.TabIndex = 57;
            this.m_btnCreate.Text = "Create New...";
            this.m_btnCreate.Click += new System.EventHandler(this.cmdCreateNewReferenceTranslation);
            // 
            // m_bReference
            // 
            this.m_bReference.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.m_bReference.Image = ((System.Drawing.Image)(resources.GetObject("m_bReference.Image")));
            this.m_bReference.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.m_bReference.Location = new System.Drawing.Point(235, 298);
            this.m_bReference.Name = "m_bReference";
            this.m_bReference.Size = new System.Drawing.Size(199, 27);
            this.m_bReference.TabIndex = 59;
            this.m_bReference.Text = "Edit Translation Properties";
            this.m_bReference.UseVisualStyleBackColor = true;
            this.m_bReference.Click += new System.EventHandler(this.cmdEditReferenceProperties);
            // 
            // Page_Translations
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.m_bReference);
            this.Controls.Add(this.m_btnCreate);
            this.Controls.Add(this.m_lOther);
            this.Controls.Add(this.m_listTranslations);
            this.Controls.Add(this.m_lSource);
            this.Controls.Add(this.m_bSource);
            this.Controls.Add(this.m_bTarget);
            this.Controls.Add(this.m_lTargetDefinition);
            this.Controls.Add(this.m_lReference);
            this.Controls.Add(this.m_lTargetAndSource);
            this.Name = "Page_Translations";
            this.Size = new System.Drawing.Size(468, 363);
            this.Load += new System.EventHandler(this.cmdLoad);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label m_lTargetAndSource;
        private System.Windows.Forms.Label m_lReference;
        private System.Windows.Forms.Label m_lTargetDefinition;
        private System.Windows.Forms.Button m_bTarget;
        private System.Windows.Forms.Button m_bSource;
        private System.Windows.Forms.Label m_lSource;
        private System.Windows.Forms.CheckedListBox m_listTranslations;
        private System.Windows.Forms.Label m_lOther;
        private System.Windows.Forms.Button m_btnCreate;
        private System.Windows.Forms.Button m_bReference;
    }
}
