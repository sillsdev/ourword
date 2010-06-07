namespace OurWord.Ctrls.Commands
{
    partial class CtrlCommands
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CtrlCommands));
            this.TopTools = new System.Windows.Forms.ToolStrip();
            this.m_Layout = new System.Windows.Forms.ToolStripDropDownButton();
            this.m_Drafting = new System.Windows.Forms.ToolStripMenuItem();
            this.m_NaturalnessCheck = new System.Windows.Forms.ToolStripMenuItem();
            this.m_BackTranslation = new System.Windows.Forms.ToolStripMenuItem();
            this.m_ConsultantPreparation = new System.Windows.Forms.ToolStripMenuItem();
            this.m_LayoutSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.m_Zoom = new System.Windows.Forms.ToolStripMenuItem();
            this.BottomTools = new System.Windows.Forms.ToolStrip();
            this.m_Exit = new System.Windows.Forms.ToolStripButton();
            this.m_Save = new System.Windows.Forms.ToolStripButton();
            this.m_Project = new System.Windows.Forms.ToolStripDropDownButton();
            this.m_New = new System.Windows.Forms.ToolStripMenuItem();
            this.initializeFromAnInternetRepositoryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.createANewProjectOnThisComputerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.m_Open = new System.Windows.Forms.ToolStripMenuItem();
            this.m_itemSave = new System.Windows.Forms.ToolStripMenuItem();
            this.m_Export = new System.Windows.Forms.ToolStripMenuItem();
            this.m_PrintBook = new System.Windows.Forms.ToolStripButton();
            this.m_Edit = new System.Windows.Forms.ToolStripDropDownButton();
            this.m_Undo = new System.Windows.Forms.ToolStripMenuItem();
            this.m_Redo = new System.Windows.Forms.ToolStripMenuItem();
            this.m_EditSeparatorUndo = new System.Windows.Forms.ToolStripSeparator();
            this.m_Cut = new System.Windows.Forms.ToolStripMenuItem();
            this.m_Copy = new System.Windows.Forms.ToolStripMenuItem();
            this.m_Paste = new System.Windows.Forms.ToolStripMenuItem();
            this.m_EditSeparatorPaste = new System.Windows.Forms.ToolStripSeparator();
            this.m_ChangeParagraphStyle = new System.Windows.Forms.ToolStripMenuItem();
            this.m_InsertFootnote = new System.Windows.Forms.ToolStripMenuItem();
            this.m_DeleteFootnote = new System.Windows.Forms.ToolStripMenuItem();
            this.m_Italic = new System.Windows.Forms.ToolStripButton();
            this.m_InsertNote = new System.Windows.Forms.ToolStripButton();
            this.m_History = new System.Windows.Forms.ToolStripButton();
            this.m_Configuration = new System.Windows.Forms.ToolStripButton();
            this.m_User = new System.Windows.Forms.ToolStripDropDownButton();
            this.TopTools.SuspendLayout();
            this.BottomTools.SuspendLayout();
            this.SuspendLayout();
            // 
            // TopTools
            // 
            this.TopTools.BackColor = System.Drawing.Color.Transparent;
            this.TopTools.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TopTools.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.TopTools.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_Layout,
            this.m_User});
            this.TopTools.Location = new System.Drawing.Point(0, 0);
            this.TopTools.Name = "TopTools";
            this.TopTools.Size = new System.Drawing.Size(430, 32);
            this.TopTools.TabIndex = 0;
            // 
            // m_Layout
            // 
            this.m_Layout.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_Drafting,
            this.m_NaturalnessCheck,
            this.m_BackTranslation,
            this.m_ConsultantPreparation,
            this.m_LayoutSeparator,
            this.m_Zoom});
            this.m_Layout.Image = ((System.Drawing.Image)(resources.GetObject("m_Layout.Image")));
            this.m_Layout.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.m_Layout.Name = "m_Layout";
            this.m_Layout.Size = new System.Drawing.Size(116, 29);
            this.m_Layout.Text = "Drafting";
            this.m_Layout.DropDownOpening += new System.EventHandler(this.cmdLayoutDropDownOpening);
            // 
            // m_Drafting
            // 
            this.m_Drafting.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_Drafting.Image = ((System.Drawing.Image)(resources.GetObject("m_Drafting.Image")));
            this.m_Drafting.Name = "m_Drafting";
            this.m_Drafting.Size = new System.Drawing.Size(201, 22);
            this.m_Drafting.Text = "&Drafting";
            this.m_Drafting.Click += new System.EventHandler(this.cmdSwitchLayout);
            // 
            // m_NaturalnessCheck
            // 
            this.m_NaturalnessCheck.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_NaturalnessCheck.Image = ((System.Drawing.Image)(resources.GetObject("m_NaturalnessCheck.Image")));
            this.m_NaturalnessCheck.Name = "m_NaturalnessCheck";
            this.m_NaturalnessCheck.Size = new System.Drawing.Size(201, 22);
            this.m_NaturalnessCheck.Text = "&Naturalness Check";
            this.m_NaturalnessCheck.Click += new System.EventHandler(this.cmdSwitchLayout);
            // 
            // m_BackTranslation
            // 
            this.m_BackTranslation.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_BackTranslation.Image = ((System.Drawing.Image)(resources.GetObject("m_BackTranslation.Image")));
            this.m_BackTranslation.Name = "m_BackTranslation";
            this.m_BackTranslation.Size = new System.Drawing.Size(201, 22);
            this.m_BackTranslation.Text = "&Back Translation";
            this.m_BackTranslation.Click += new System.EventHandler(this.cmdSwitchLayout);
            // 
            // m_ConsultantPreparation
            // 
            this.m_ConsultantPreparation.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_ConsultantPreparation.Image = ((System.Drawing.Image)(resources.GetObject("m_ConsultantPreparation.Image")));
            this.m_ConsultantPreparation.Name = "m_ConsultantPreparation";
            this.m_ConsultantPreparation.Size = new System.Drawing.Size(201, 22);
            this.m_ConsultantPreparation.Text = "Consultant &Preparation";
            this.m_ConsultantPreparation.Click += new System.EventHandler(this.cmdSwitchLayout);
            // 
            // m_LayoutSeparator
            // 
            this.m_LayoutSeparator.Name = "m_LayoutSeparator";
            this.m_LayoutSeparator.Size = new System.Drawing.Size(198, 6);
            // 
            // m_Zoom
            // 
            this.m_Zoom.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_Zoom.Name = "m_Zoom";
            this.m_Zoom.Size = new System.Drawing.Size(201, 22);
            this.m_Zoom.Text = "Zoom";
            // 
            // BottomTools
            // 
            this.BottomTools.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.BottomTools.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_Exit,
            this.m_Save,
            this.m_Project,
            this.m_PrintBook,
            this.m_Edit,
            this.m_Italic,
            this.m_InsertNote,
            this.m_History,
            this.m_Configuration});
            this.BottomTools.Location = new System.Drawing.Point(0, 32);
            this.BottomTools.Name = "BottomTools";
            this.BottomTools.Size = new System.Drawing.Size(430, 46);
            this.BottomTools.TabIndex = 1;
            // 
            // m_Exit
            // 
            this.m_Exit.Image = ((System.Drawing.Image)(resources.GetObject("m_Exit.Image")));
            this.m_Exit.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.m_Exit.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.m_Exit.Name = "m_Exit";
            this.m_Exit.Size = new System.Drawing.Size(29, 43);
            this.m_Exit.Text = "Exit";
            this.m_Exit.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.m_Exit.Click += new System.EventHandler(this.cmdExit);
            // 
            // m_Save
            // 
            this.m_Save.Image = ((System.Drawing.Image)(resources.GetObject("m_Save.Image")));
            this.m_Save.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.m_Save.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.m_Save.Name = "m_Save";
            this.m_Save.Size = new System.Drawing.Size(35, 43);
            this.m_Save.Text = "Save";
            this.m_Save.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.m_Save.Click += new System.EventHandler(this.cmdSave);
            // 
            // m_Project
            // 
            this.m_Project.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_New,
            this.m_Open,
            this.m_itemSave,
            this.m_Export});
            this.m_Project.Image = ((System.Drawing.Image)(resources.GetObject("m_Project.Image")));
            this.m_Project.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.m_Project.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.m_Project.Name = "m_Project";
            this.m_Project.Size = new System.Drawing.Size(57, 43);
            this.m_Project.Text = "Project";
            this.m_Project.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.m_Project.DropDownOpening += new System.EventHandler(this.cmdProjectDropDownOpening);
            // 
            // m_New
            // 
            this.m_New.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.initializeFromAnInternetRepositoryToolStripMenuItem,
            this.createANewProjectOnThisComputerToolStripMenuItem});
            this.m_New.Image = ((System.Drawing.Image)(resources.GetObject("m_New.Image")));
            this.m_New.Name = "m_New";
            this.m_New.Size = new System.Drawing.Size(138, 22);
            this.m_New.Text = "&New";
            this.m_New.Click += new System.EventHandler(this.cmdDownloadRepositoryFromInternet);
            // 
            // initializeFromAnInternetRepositoryToolStripMenuItem
            // 
            this.initializeFromAnInternetRepositoryToolStripMenuItem.Name = "initializeFromAnInternetRepositoryToolStripMenuItem";
            this.initializeFromAnInternetRepositoryToolStripMenuItem.Size = new System.Drawing.Size(287, 22);
            this.initializeFromAnInternetRepositoryToolStripMenuItem.Text = "Initialize from an Internet Repository...";
            this.initializeFromAnInternetRepositoryToolStripMenuItem.Click += new System.EventHandler(this.cmdDownloadRepositoryFromInternet);
            // 
            // createANewProjectOnThisComputerToolStripMenuItem
            // 
            this.createANewProjectOnThisComputerToolStripMenuItem.Name = "createANewProjectOnThisComputerToolStripMenuItem";
            this.createANewProjectOnThisComputerToolStripMenuItem.Size = new System.Drawing.Size(287, 22);
            this.createANewProjectOnThisComputerToolStripMenuItem.Text = "Create a New Project on this computer...";
            this.createANewProjectOnThisComputerToolStripMenuItem.Click += new System.EventHandler(this.cmdCreateProject);
            // 
            // m_Open
            // 
            this.m_Open.Image = ((System.Drawing.Image)(resources.GetObject("m_Open.Image")));
            this.m_Open.Name = "m_Open";
            this.m_Open.Size = new System.Drawing.Size(138, 22);
            this.m_Open.Text = "&Open";
            // 
            // m_itemSave
            // 
            this.m_itemSave.Image = ((System.Drawing.Image)(resources.GetObject("m_itemSave.Image")));
            this.m_itemSave.Name = "m_itemSave";
            this.m_itemSave.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.m_itemSave.Size = new System.Drawing.Size(138, 22);
            this.m_itemSave.Text = "&Save";
            this.m_itemSave.Click += new System.EventHandler(this.cmdSave);
            // 
            // m_Export
            // 
            this.m_Export.Name = "m_Export";
            this.m_Export.Size = new System.Drawing.Size(138, 22);
            this.m_Export.Text = "E&xport...";
            this.m_Export.Click += new System.EventHandler(this.cmdExport);
            // 
            // m_PrintBook
            // 
            this.m_PrintBook.Image = ((System.Drawing.Image)(resources.GetObject("m_PrintBook.Image")));
            this.m_PrintBook.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.m_PrintBook.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.m_PrintBook.Name = "m_PrintBook";
            this.m_PrintBook.Size = new System.Drawing.Size(36, 43);
            this.m_PrintBook.Text = "Print";
            this.m_PrintBook.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.m_PrintBook.Click += new System.EventHandler(this.cmdPrintBook);
            // 
            // m_Edit
            // 
            this.m_Edit.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_Undo,
            this.m_Redo,
            this.m_EditSeparatorUndo,
            this.m_Cut,
            this.m_Copy,
            this.m_Paste,
            this.m_EditSeparatorPaste,
            this.m_ChangeParagraphStyle,
            this.m_InsertFootnote,
            this.m_DeleteFootnote});
            this.m_Edit.Image = ((System.Drawing.Image)(resources.GetObject("m_Edit.Image")));
            this.m_Edit.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.m_Edit.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.m_Edit.Name = "m_Edit";
            this.m_Edit.Size = new System.Drawing.Size(40, 43);
            this.m_Edit.Text = "Edit";
            this.m_Edit.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.m_Edit.DropDownOpening += new System.EventHandler(this.cmdEditDropDownOpening);
            // 
            // m_Undo
            // 
            this.m_Undo.Image = ((System.Drawing.Image)(resources.GetObject("m_Undo.Image")));
            this.m_Undo.Name = "m_Undo";
            this.m_Undo.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Z)));
            this.m_Undo.Size = new System.Drawing.Size(189, 22);
            this.m_Undo.Text = "&Undo";
            this.m_Undo.Click += new System.EventHandler(this.cmdUndo);
            // 
            // m_Redo
            // 
            this.m_Redo.Image = ((System.Drawing.Image)(resources.GetObject("m_Redo.Image")));
            this.m_Redo.Name = "m_Redo";
            this.m_Redo.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Y)));
            this.m_Redo.Size = new System.Drawing.Size(189, 22);
            this.m_Redo.Text = "&Redo";
            this.m_Redo.Click += new System.EventHandler(this.cmdRedo);
            // 
            // m_EditSeparatorUndo
            // 
            this.m_EditSeparatorUndo.Name = "m_EditSeparatorUndo";
            this.m_EditSeparatorUndo.Size = new System.Drawing.Size(186, 6);
            // 
            // m_Cut
            // 
            this.m_Cut.Image = ((System.Drawing.Image)(resources.GetObject("m_Cut.Image")));
            this.m_Cut.Name = "m_Cut";
            this.m_Cut.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.X)));
            this.m_Cut.Size = new System.Drawing.Size(189, 22);
            this.m_Cut.Text = "Cu&t";
            this.m_Cut.Click += new System.EventHandler(this.cmdCut);
            // 
            // m_Copy
            // 
            this.m_Copy.Image = ((System.Drawing.Image)(resources.GetObject("m_Copy.Image")));
            this.m_Copy.Name = "m_Copy";
            this.m_Copy.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.m_Copy.Size = new System.Drawing.Size(189, 22);
            this.m_Copy.Text = "&Copy";
            this.m_Copy.Click += new System.EventHandler(this.cmdCopy);
            // 
            // m_Paste
            // 
            this.m_Paste.Image = ((System.Drawing.Image)(resources.GetObject("m_Paste.Image")));
            this.m_Paste.Name = "m_Paste";
            this.m_Paste.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V)));
            this.m_Paste.Size = new System.Drawing.Size(189, 22);
            this.m_Paste.Text = "&Paste";
            this.m_Paste.Click += new System.EventHandler(this.cmdPaste);
            // 
            // m_EditSeparatorPaste
            // 
            this.m_EditSeparatorPaste.Name = "m_EditSeparatorPaste";
            this.m_EditSeparatorPaste.Size = new System.Drawing.Size(186, 6);
            // 
            // m_ChangeParagraphStyle
            // 
            this.m_ChangeParagraphStyle.Name = "m_ChangeParagraphStyle";
            this.m_ChangeParagraphStyle.Size = new System.Drawing.Size(189, 22);
            this.m_ChangeParagraphStyle.Text = "C&hange Paragraph To";
            // 
            // m_InsertFootnote
            // 
            this.m_InsertFootnote.Image = ((System.Drawing.Image)(resources.GetObject("m_InsertFootnote.Image")));
            this.m_InsertFootnote.Name = "m_InsertFootnote";
            this.m_InsertFootnote.Size = new System.Drawing.Size(189, 22);
            this.m_InsertFootnote.Text = "Insert Footnote";
            this.m_InsertFootnote.Click += new System.EventHandler(this.cmdInsertFootnote);
            // 
            // m_DeleteFootnote
            // 
            this.m_DeleteFootnote.Image = ((System.Drawing.Image)(resources.GetObject("m_DeleteFootnote.Image")));
            this.m_DeleteFootnote.Name = "m_DeleteFootnote";
            this.m_DeleteFootnote.Size = new System.Drawing.Size(189, 22);
            this.m_DeleteFootnote.Text = "Delete Footnote";
            this.m_DeleteFootnote.Click += new System.EventHandler(this.cmdDeleteFootnote);
            // 
            // m_Italic
            // 
            this.m_Italic.Image = ((System.Drawing.Image)(resources.GetObject("m_Italic.Image")));
            this.m_Italic.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.m_Italic.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.m_Italic.Name = "m_Italic";
            this.m_Italic.Size = new System.Drawing.Size(36, 43);
            this.m_Italic.Text = "Italic";
            this.m_Italic.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            // 
            // m_InsertNote
            // 
            this.m_InsertNote.Image = ((System.Drawing.Image)(resources.GetObject("m_InsertNote.Image")));
            this.m_InsertNote.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.m_InsertNote.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.m_InsertNote.Name = "m_InsertNote";
            this.m_InsertNote.Size = new System.Drawing.Size(37, 43);
            this.m_InsertNote.Text = "Note";
            this.m_InsertNote.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.m_InsertNote.Click += new System.EventHandler(this.cmdInsertNote);
            // 
            // m_History
            // 
            this.m_History.Image = ((System.Drawing.Image)(resources.GetObject("m_History.Image")));
            this.m_History.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.m_History.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.m_History.Name = "m_History";
            this.m_History.Size = new System.Drawing.Size(49, 43);
            this.m_History.Text = "History";
            this.m_History.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.m_History.Click += new System.EventHandler(this.cmdHistory);
            // 
            // m_Configuration
            // 
            this.m_Configuration.Image = ((System.Drawing.Image)(resources.GetObject("m_Configuration.Image")));
            this.m_Configuration.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.m_Configuration.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.m_Configuration.Name = "m_Configuration";
            this.m_Configuration.Size = new System.Drawing.Size(85, 43);
            this.m_Configuration.Text = "Configuration";
            this.m_Configuration.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            // 
            // m_User
            // 
            this.m_User.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.m_User.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.m_User.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_User.ForeColor = System.Drawing.Color.Moccasin;
            this.m_User.Image = ((System.Drawing.Image)(resources.GetObject("m_User.Image")));
            this.m_User.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.m_User.Name = "m_User";
            this.m_User.Size = new System.Drawing.Size(59, 29);
            this.m_User.Text = "<User>";
            this.m_User.DropDownOpening += new System.EventHandler(this.cmdUserDropDownOpening);
            // 
            // CtrlCommands
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.DarkGray;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.BottomTools);
            this.Controls.Add(this.TopTools);
            this.Name = "CtrlCommands";
            this.Size = new System.Drawing.Size(430, 86);
            this.Load += new System.EventHandler(this.cmdLoad);
            this.TopTools.ResumeLayout(false);
            this.TopTools.PerformLayout();
            this.BottomTools.ResumeLayout(false);
            this.BottomTools.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip TopTools;
        private System.Windows.Forms.ToolStrip BottomTools;
        private System.Windows.Forms.ToolStripButton m_Exit;
        private System.Windows.Forms.ToolStripButton m_Save;
        private System.Windows.Forms.ToolStripDropDownButton m_Project;
        private System.Windows.Forms.ToolStripButton m_PrintBook;
        private System.Windows.Forms.ToolStripDropDownButton m_Edit;
        private System.Windows.Forms.ToolStripButton m_Italic;
        private System.Windows.Forms.ToolStripButton m_InsertNote;
        private System.Windows.Forms.ToolStripButton m_History;
        private System.Windows.Forms.ToolStripButton m_Configuration;
        private System.Windows.Forms.ToolStripDropDownButton m_Layout;
        private System.Windows.Forms.ToolStripMenuItem m_Drafting;
        private System.Windows.Forms.ToolStripMenuItem m_NaturalnessCheck;
        private System.Windows.Forms.ToolStripMenuItem m_BackTranslation;
        private System.Windows.Forms.ToolStripMenuItem m_ConsultantPreparation;
        private System.Windows.Forms.ToolStripSeparator m_LayoutSeparator;
        private System.Windows.Forms.ToolStripMenuItem m_Zoom;
        private System.Windows.Forms.ToolStripMenuItem m_New;
        private System.Windows.Forms.ToolStripMenuItem m_Open;
        private System.Windows.Forms.ToolStripMenuItem m_itemSave;
        private System.Windows.Forms.ToolStripMenuItem m_Export;
        private System.Windows.Forms.ToolStripMenuItem initializeFromAnInternetRepositoryToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem createANewProjectOnThisComputerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem m_Undo;
        private System.Windows.Forms.ToolStripMenuItem m_Redo;
        private System.Windows.Forms.ToolStripSeparator m_EditSeparatorUndo;
        private System.Windows.Forms.ToolStripMenuItem m_Cut;
        private System.Windows.Forms.ToolStripMenuItem m_Copy;
        private System.Windows.Forms.ToolStripMenuItem m_Paste;
        private System.Windows.Forms.ToolStripSeparator m_EditSeparatorPaste;
        private System.Windows.Forms.ToolStripMenuItem m_ChangeParagraphStyle;
        private System.Windows.Forms.ToolStripMenuItem m_InsertFootnote;
        private System.Windows.Forms.ToolStripMenuItem m_DeleteFootnote;
        private System.Windows.Forms.ToolStripDropDownButton m_User;
    }
}
