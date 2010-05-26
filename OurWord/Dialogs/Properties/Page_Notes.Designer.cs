namespace OurWord.Dialogs
{
    partial class Page_Notes
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
            this.m_PropGrid = new System.Windows.Forms.PropertyGrid();
            this.m_labelBehavior = new System.Windows.Forms.Label();
            this.m_labelPermissions = new System.Windows.Forms.Label();
            this.m_labelAuthorName = new System.Windows.Forms.Label();
            this.m_textNoteAuthorName = new System.Windows.Forms.TextBox();
            this.m_checkCloseWindowWhemMouseLeaves = new System.Windows.Forms.CheckBox();
            this.m_checkShowTitleBesideIcon = new System.Windows.Forms.CheckBox();
            this.m_checkCanDeleteNotes = new System.Windows.Forms.CheckBox();
            this.m_checkCanCreateHintForDaughterNotes = new System.Windows.Forms.CheckBox();
            this.m_checkCanCreateInformationNotes = new System.Windows.Forms.CheckBox();
            this.m_checkCanAssignToConsultant = new System.Windows.Forms.CheckBox();
            this.m_checkCreateFrontNotes = new System.Windows.Forms.CheckBox();
            this.m_checkTurnOnNotes = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // m_PropGrid
            // 
            this.m_PropGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.m_PropGrid.Location = new System.Drawing.Point(3, 382);
            this.m_PropGrid.Name = "m_PropGrid";
            this.m_PropGrid.PropertySort = System.Windows.Forms.PropertySort.Categorized;
            this.m_PropGrid.Size = new System.Drawing.Size(462, 207);
            this.m_PropGrid.TabIndex = 46;
            this.m_PropGrid.ToolbarVisible = false;
            // 
            // m_labelBehavior
            // 
            this.m_labelBehavior.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.m_labelBehavior.BackColor = System.Drawing.Color.Silver;
            this.m_labelBehavior.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_labelBehavior.ForeColor = System.Drawing.Color.DarkBlue;
            this.m_labelBehavior.Location = new System.Drawing.Point(6, 34);
            this.m_labelBehavior.Name = "m_labelBehavior";
            this.m_labelBehavior.Size = new System.Drawing.Size(454, 22);
            this.m_labelBehavior.TabIndex = 47;
            this.m_labelBehavior.Text = "Behavior";
            this.m_labelBehavior.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // m_labelPermissions
            // 
            this.m_labelPermissions.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.m_labelPermissions.BackColor = System.Drawing.Color.Silver;
            this.m_labelPermissions.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_labelPermissions.ForeColor = System.Drawing.Color.DarkBlue;
            this.m_labelPermissions.Location = new System.Drawing.Point(6, 148);
            this.m_labelPermissions.Name = "m_labelPermissions";
            this.m_labelPermissions.Size = new System.Drawing.Size(454, 22);
            this.m_labelPermissions.TabIndex = 48;
            this.m_labelPermissions.Text = "Permissions";
            this.m_labelPermissions.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // m_labelAuthorName
            // 
            this.m_labelAuthorName.AutoSize = true;
            this.m_labelAuthorName.Location = new System.Drawing.Point(11, 64);
            this.m_labelAuthorName.Name = "m_labelAuthorName";
            this.m_labelAuthorName.Size = new System.Drawing.Size(105, 13);
            this.m_labelAuthorName.TabIndex = 49;
            this.m_labelAuthorName.Text = "Note Author\'s Name:";
            // 
            // m_textNoteAuthorName
            // 
            this.m_textNoteAuthorName.Location = new System.Drawing.Point(160, 61);
            this.m_textNoteAuthorName.Name = "m_textNoteAuthorName";
            this.m_textNoteAuthorName.Size = new System.Drawing.Size(164, 20);
            this.m_textNoteAuthorName.TabIndex = 50;
            // 
            // m_checkCloseWindowWhemMouseLeaves
            // 
            this.m_checkCloseWindowWhemMouseLeaves.AutoSize = true;
            this.m_checkCloseWindowWhemMouseLeaves.Location = new System.Drawing.Point(14, 87);
            this.m_checkCloseWindowWhemMouseLeaves.Name = "m_checkCloseWindowWhemMouseLeaves";
            this.m_checkCloseWindowWhemMouseLeaves.Size = new System.Drawing.Size(385, 17);
            this.m_checkCloseWindowWhemMouseLeaves.TabIndex = 51;
            this.m_checkCloseWindowWhemMouseLeaves.Text = "Automatically close the Notes Window when the mouse moves outside of it?";
            this.m_checkCloseWindowWhemMouseLeaves.UseVisualStyleBackColor = true;
            // 
            // m_checkShowTitleBesideIcon
            // 
            this.m_checkShowTitleBesideIcon.AutoSize = true;
            this.m_checkShowTitleBesideIcon.Location = new System.Drawing.Point(14, 110);
            this.m_checkShowTitleBesideIcon.Name = "m_checkShowTitleBesideIcon";
            this.m_checkShowTitleBesideIcon.Size = new System.Drawing.Size(321, 17);
            this.m_checkShowTitleBesideIcon.TabIndex = 52;
            this.m_checkShowTitleBesideIcon.Text = "Show an expanded icon which includes part of the note\'s title?";
            this.m_checkShowTitleBesideIcon.UseVisualStyleBackColor = true;
            // 
            // m_checkCanDeleteNotes
            // 
            this.m_checkCanDeleteNotes.AutoSize = true;
            this.m_checkCanDeleteNotes.Location = new System.Drawing.Point(14, 176);
            this.m_checkCanDeleteNotes.Name = "m_checkCanDeleteNotes";
            this.m_checkCanDeleteNotes.Size = new System.Drawing.Size(354, 17);
            this.m_checkCanDeleteNotes.TabIndex = 53;
            this.m_checkCanDeleteNotes.Text = "Can delete Notes and Messages that were authored by other people?";
            this.m_checkCanDeleteNotes.UseVisualStyleBackColor = true;
            // 
            // m_checkCanCreateHintForDaughterNotes
            // 
            this.m_checkCanCreateHintForDaughterNotes.AutoSize = true;
            this.m_checkCanCreateHintForDaughterNotes.Location = new System.Drawing.Point(14, 199);
            this.m_checkCanCreateHintForDaughterNotes.Name = "m_checkCanCreateHintForDaughterNotes";
            this.m_checkCanCreateHintForDaughterNotes.Size = new System.Drawing.Size(207, 17);
            this.m_checkCanCreateHintForDaughterNotes.TabIndex = 54;
            this.m_checkCanCreateHintForDaughterNotes.Text = "Can author \"Hint for Daughter\" notes?";
            this.m_checkCanCreateHintForDaughterNotes.UseVisualStyleBackColor = true;
            // 
            // m_checkCanCreateInformationNotes
            // 
            this.m_checkCanCreateInformationNotes.AutoSize = true;
            this.m_checkCanCreateInformationNotes.Location = new System.Drawing.Point(14, 222);
            this.m_checkCanCreateInformationNotes.Name = "m_checkCanCreateInformationNotes";
            this.m_checkCanCreateInformationNotes.Size = new System.Drawing.Size(178, 17);
            this.m_checkCanCreateInformationNotes.TabIndex = 55;
            this.m_checkCanCreateInformationNotes.Text = "Can author \"Information\" notes?";
            this.m_checkCanCreateInformationNotes.UseVisualStyleBackColor = true;
            // 
            // m_checkCanAssignToConsultant
            // 
            this.m_checkCanAssignToConsultant.AutoSize = true;
            this.m_checkCanAssignToConsultant.Location = new System.Drawing.Point(14, 245);
            this.m_checkCanAssignToConsultant.Name = "m_checkCanAssignToConsultant";
            this.m_checkCanAssignToConsultant.Size = new System.Drawing.Size(187, 17);
            this.m_checkCanAssignToConsultant.TabIndex = 56;
            this.m_checkCanAssignToConsultant.Text = "Can assign notes to a Consultant?";
            this.m_checkCanAssignToConsultant.UseVisualStyleBackColor = true;
            // 
            // m_checkCreateFrontNotes
            // 
            this.m_checkCreateFrontNotes.AutoSize = true;
            this.m_checkCreateFrontNotes.Location = new System.Drawing.Point(14, 268);
            this.m_checkCreateFrontNotes.Name = "m_checkCreateFrontNotes";
            this.m_checkCreateFrontNotes.Size = new System.Drawing.Size(224, 17);
            this.m_checkCreateFrontNotes.TabIndex = 57;
            this.m_checkCreateFrontNotes.Text = "Can create notes in the Front Translation?";
            this.m_checkCreateFrontNotes.UseVisualStyleBackColor = true;
            // 
            // m_checkTurnOnNotes
            // 
            this.m_checkTurnOnNotes.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.m_checkTurnOnNotes.AutoSize = true;
            this.m_checkTurnOnNotes.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_checkTurnOnNotes.Location = new System.Drawing.Point(124, 6);
            this.m_checkTurnOnNotes.Name = "m_checkTurnOnNotes";
            this.m_checkTurnOnNotes.Size = new System.Drawing.Size(208, 19);
            this.m_checkTurnOnNotes.TabIndex = 58;
            this.m_checkTurnOnNotes.Text = "Turn On Notes for this User?";
            this.m_checkTurnOnNotes.UseVisualStyleBackColor = true;
            this.m_checkTurnOnNotes.CheckedChanged += new System.EventHandler(this.cmdTurnOnNotesCheckChanged);
            // 
            // Page_Notes
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.Controls.Add(this.m_checkTurnOnNotes);
            this.Controls.Add(this.m_checkCreateFrontNotes);
            this.Controls.Add(this.m_checkCanAssignToConsultant);
            this.Controls.Add(this.m_checkCanCreateInformationNotes);
            this.Controls.Add(this.m_checkCanCreateHintForDaughterNotes);
            this.Controls.Add(this.m_checkCanDeleteNotes);
            this.Controls.Add(this.m_checkShowTitleBesideIcon);
            this.Controls.Add(this.m_checkCloseWindowWhemMouseLeaves);
            this.Controls.Add(this.m_textNoteAuthorName);
            this.Controls.Add(this.m_labelAuthorName);
            this.Controls.Add(this.m_labelPermissions);
            this.Controls.Add(this.m_labelBehavior);
            this.Controls.Add(this.m_PropGrid);
            this.Name = "Page_Notes";
            this.Size = new System.Drawing.Size(468, 592);
            this.Load += new System.EventHandler(this.cmdLoad);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PropertyGrid m_PropGrid;
        private System.Windows.Forms.Label m_labelBehavior;
        private System.Windows.Forms.Label m_labelPermissions;
        private System.Windows.Forms.Label m_labelAuthorName;
        private System.Windows.Forms.TextBox m_textNoteAuthorName;
        private System.Windows.Forms.CheckBox m_checkCloseWindowWhemMouseLeaves;
        private System.Windows.Forms.CheckBox m_checkShowTitleBesideIcon;
        private System.Windows.Forms.CheckBox m_checkCanDeleteNotes;
        private System.Windows.Forms.CheckBox m_checkCanCreateHintForDaughterNotes;
        private System.Windows.Forms.CheckBox m_checkCanCreateInformationNotes;
        private System.Windows.Forms.CheckBox m_checkCanAssignToConsultant;
        private System.Windows.Forms.CheckBox m_checkCreateFrontNotes;
        private System.Windows.Forms.CheckBox m_checkTurnOnNotes;
    }
}
