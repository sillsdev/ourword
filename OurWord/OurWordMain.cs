/**********************************************************************************************
 * Project: Our Word!
 * File:    OurWordMain.cs
 * Author:  John Wimbish
 * Created: 2 Dec 2003
 * Purpose: Main window and app for the application.
 * Legal:   Copyright (c) 2004-10, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
#region Using
using System;
using System.Diagnostics;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using System.Threading;

using JWTools;
using OurWord.Ctrls;
using OurWord.Dialogs.Membership;
using OurWord.Edit.Blocks;
using OurWord.Printing;
using OurWordData;
using OurWordData.DataModel;
using OurWordData.DataModel.Annotations;
using OurWordData.DataModel.Membership;
using OurWordData.Styles;

using OurWord.Edit;
using OurWord.Layouts;
using OurWord.Dialogs;
using OurWord.Dialogs.History;
using OurWord.Utilities;
using OurWordData.Synchronize;
using Shortcut=OurWord.Utilities.Shortcut;

#endregion

namespace OurWord
{
	public class OurWordMain : Form
		// Main application window and top-level message routing
	{
		// Static Objects & Globals ----------------------------------------------------------
		#region Attr{g}: bool TargetIsLocked
		static public bool TargetIsLocked
		{
			get
			{
				Debug.Assert(null != DB.Project);

				var section = DB.Project.STarget;

				var book = (null == section) ? null : section.Book;
				if (null == book)
					return false;

				return book.Locked;
			}
		}
		#endregion
        #region Attr{g}: UndoRedoStack URStack
        public UndoRedoStack URStack
        {
            get
            {
                Debug.Assert(null != m_URStack);
                return m_URStack;
            }
        }
	    readonly UndoRedoStack m_URStack;
        #endregion

        // Client Windows --------------------------------------------------------------------
        #region CLIENT WINDOWS
        #region Method: void SetCurrentLayout(sLayoutName)
        public void SetCurrentLayout(string sLayoutName)
        {
            if (WLayout.SetCurrentLayout(sLayoutName))
                ResetWindowContents();
        }
        #endregion

        private ToolStripMenuItem checkForUpdatesToolStripMenuItem;
        private ViewBar m_ViewBar;
        #region VAttr{g}: Layout CurrentLayout
        public WLayout CurrentLayout
        {
            get
            {
                return WLayout.CurrentLayout;
            }
        }
        #endregion
        private Panel m_panelContents;

        #region VAttr{g}: Control FocusedWindow - the window with current Focus, or null
        OWWindow FocusedWindow
        {
            get
            {
                if (null == CurrentLayout)
                    return null;
                if (CurrentLayout.Focused)
                    return CurrentLayout;
                return null;
            }
        }
        #endregion

        #region ToolStripContentPanel ContentPanel - Where the TitleWindow, SideWindows, and the MainWindow reside
        ToolStripContentPanel ContentPanel
        {
            get
            {
                return m_toolStripContainer.ContentPanel;
            }
        }
        #endregion

		#region Method: void SetTitleBarText()
		private void SetTitleBarText()
			// Titlebar - sets title bar text to "OurWord - ProjName"
		{
            // First, Display "Our Word"
			Text = LanguageResources.AppTitle;

            // If a book is loaded, then display its name
            if (null != DB.Project && null != DB.TargetBook)
            {
                Text += (" - " + DB.TargetBook.DisplayName);
                return;
            }

            // If a project exists, then display its name
			if (null != DB.Project)
				Text += (" - " + DB.Project.DisplayName);
		}
		#endregion
        #region Method: void ResetWindowContents() - called whenever content changes (OnEnterSection)
        public void ResetWindowContents()
        {
            // Set the TitleBar text
            SetTitleBarText();

            // Toolbar button enabling
            SetupMenusAndToolbarsVisibility();

            // Information about the current view situation
            m_ViewBar.SetToContext(CurrentLayout, DB.Project);

            // Do we have a valid project? Can't do much if not.
            if (null == DB.Project || null == DB.FrontSection || null == DB.TargetSection)
            {
                CurrentLayout.Clear();
                CurrentLayout.Invalidate();
                return;
            }

            // Loading the main window should also load the data in the side windows, as this
            // is generally built as the main window is loaded (or as items there are selected)
            CurrentLayout.LoadData();

            // Place focus in the main window, so that it is ready for editing
            CurrentLayout.Focus();
        }
        #endregion
        #region Method: void Dim()
        void Dim()
        {
            if (ScreenDraw.Dim)
                return;

            ScreenDraw.Dim = true;
            CurrentLayout.Invalidate();
        }
        #endregion
        #region Method: void UnDim()
        void UnDim()
        {
            if (!ScreenDraw.Dim)
                return;

            ScreenDraw.Dim = false;
            CurrentLayout.Invalidate();
        }
        #endregion
        #endregion

        // Autosave --------------------------------------------------------------------------
		private System.Windows.Forms.Timer m_AutoSaveTimer;
		#region Method: void InitializeAutoSave()
		private void InitializeAutoSave()
		{
			// We'll have an autosave every 10 minutes
			const int c_AutoSaveMinutes = 10;

			m_AutoSaveTimer = new System.Windows.Forms.Timer(this.components);
			m_AutoSaveTimer.Tick += new System.EventHandler(OnTimerAutoSave);
			m_AutoSaveTimer.Interval = c_AutoSaveMinutes * 60 * 1000;

			m_AutoSaveTimer.Start();
		}
		#endregion
		#region Handler: void OnTimerAutoSave(object sender, System.EventArgs e)
		private void OnTimerAutoSave(object sender, System.EventArgs e)
		{
			//Console.WriteLine("--AutoSave Timer Tick--");

            if (null == DB.Project)
                return;

            // Do the save
            DB.Project.Save(G.CreateProgressIndicator());
		}
		#endregion

        // Toolbar, MenuBar, Taskbar & StatusBar ---------------------------------------------
        #region Toolbar, Taskbar & StatusBar
        #region Menu/Toolbar attributes
        private ToolStripMenuItem m_menuInitializeFromAnInternetRepositoryToolStripMenuItem;
        private ToolStripMenuItem m_menuCreateANewProjectOnThisComputerToolStripMenuItem;
        private ToolStripButton m_btnHistory;
        private ToolStripMenuItem m_menuCopyBTFromFrontTranslation;
        private ToolStripMenuItem m_menuConsultantPreparationToolStripMenuItem;
        private ToolStripDropDownButton m_btnChapter;
        private ToolStripMenuItem m_menuUndo;
        private ToolStripMenuItem m_menuRedo;
        private ToolStripMenuItem m_menuInsertFootnote;
        private ToolStripMenuItem m_menuDeleteFootnote;
        private ToolStripSeparator m_separatorUndoRedo;
        private ToolStripMenuItem m_menuPreviousSection;
        private ToolStripMenuItem m_menuNextSection;
        private ToolStripMenuItem m_menuToggleItalics;
        private ToolStripDropDownButton m_btnProject;
        private ToolStripMenuItem m_menuNewProject;
        private ToolStripMenuItem m_menuOpenProject;
		private ToolStripMenuItem m_menuSaveProject;
        private ToolStripButton m_btnPrint;
        private ToolStripMenuItem m_menuConfigure;
        private ToolStripSeparator m_separatorDebug;
        private ToolStripMenuItem m_menuRunDebugTestSuite;
        private ToolStripMenuItem m_menuOnlyShowSectionsThat;
        private ToolStripMenuItem m_Synchronize;
        private ToolStripMenuItem m_menuLocalizerTool;
        private ToolStripButton m_btnGotoFirstSection;
        private ToolStripButton m_btnGotoLastSection;
        private ToolStripDropDownButton m_btnWindow;
        private ToolStripMenuItem m_menuDrafting;
        private ToolStripMenuItem m_menuBackTranslation;
        private ToolStripMenuItem m_menuNaturalnessCheck;
        private ToolStripSeparator m_separatorWindow;
        private ToolStripMenuItem m_menuZoom;
        private ToolStripDropDownButton m_btnHelp;
        private ToolStripMenuItem m_menuHelpTopics;
        private ToolStripMenuItem m_menuAbout;
        private ToolStripDropDownButton m_menuEdit;
        private ToolStripMenuItem m_menuChangeParagraphTo;
        private ToolStripMenuItem m_menuCut;
        private ToolStripMenuItem m_menuCopy;
        private ToolStripMenuItem m_menuPaste;
        private ToolStripSeparator m_seperatorEdit;
        private ToolStripContainer m_toolStripContainer;
        private ToolStrip m_ToolStrip;
        private StatusStrip m_StatusStrip;
        private ToolStripSeparator m_separator3;

        private ToolStripMenuItem m_menuExportProject;
        private ToolStripDropDownButton m_btnInsertNote;

        private ToolStripButton m_btnExit;
        private ToolStripButton m_btnProjectSave;
        private ToolStripSeparator m_separator1;
        private ToolStripButton m_btnEditCut;
        private ToolStripButton m_btnEditCopy;
        private ToolStripButton m_btnEditPaste;
        private ToolStripButton m_btnItalic;
        private ToolStripSplitButton m_btnGotoPreviousSection;
        private ToolStripSplitButton m_btnGotoNextSection;
        private ToolStripDropDownButton m_btnGoToBook;
        private ToolStripDropDownButton m_btnTools;
        private ToolStripMenuItem m_menuIncrementBookStatus;
        private ToolStripMenuItem m_menuRestoreFromBackup;
        private ToolStripSeparator m_separator4;

        private ToolStripStatusLabel m_StatusMessage1;
        private ToolStripProgressBar m_ProgressBar;
        private ToolStripStatusLabel m_StatusMessage2;

        // Taskbar
        #endregion
        #region Method: void SetupNavigationButtons()
        private const int c_cMaxMenuLength = 60; // Keep sub-menus from getting too long
        public void SetupNavigationButtons()
        {
            // Remove any subitems
            m_btnGotoPreviousSection.DropDownItems.Clear();
            m_btnGotoNextSection.DropDownItems.Clear(); 

            // Get our current position within the book
            if (null == DB.FrontSection)
                return;
            int iPos = DB.FrontBook.Sections.FindObj(DB.FrontSection);

            // Loop to populate the Subitems
            ArrayList aPrevious = new ArrayList();
            ArrayList aNext = new ArrayList();

            for (int i = 0; i < DB.FrontBook.Sections.Count; i++)
            {
                // Get the two sections
                DSection SFront = DB.FrontBook.Sections[i] as DSection;
                DSection STarget = DB.TargetBook.Sections[i] as DSection;

                // If a filter is active, then we only care about sections that match
                if (DSection.FilterIsActive && !STarget.MatchesFilter)
                    continue;

                // Get the reference
                string sReference = SFront.ReferenceSpan.Start.FullName;

                // Get a section Title
                string sTitle = STarget.Title.Trim();
                if (sTitle.Length == 0)
                    sTitle = SFront.Title;

                // Keep its length from being too long (otherwise the menu gets
                // too cluttered.
                if (sTitle.Length > c_cMaxMenuLength)
                    sTitle = (sTitle.Substring(0, c_cMaxMenuLength) + "...");

                // Put together a menu name
                string sMenuText = sReference;
                if (!string.IsNullOrEmpty(sTitle))
                    sMenuText += (" - " + sTitle);

                // Add it to the appropriate button
                if (i == iPos)
                    continue;
                ArrayList a = (i < iPos) ? aPrevious : aNext;
                ToolStripMenuItem item = new ToolStripMenuItem(sMenuText, null, cmdGoToSection, sMenuText);
                a.Add(item);
            }

            ToolStripMenuItem[] v = new ToolStripMenuItem[aPrevious.Count];
            for (int k = 0; k < aPrevious.Count; k++)
                v[k] = aPrevious[k] as ToolStripMenuItem;
            m_btnGotoPreviousSection.DropDownItems.AddRange(v);

            v = new ToolStripMenuItem[aNext.Count];
            for (int k = 0; k < aNext.Count; k++)
                v[k] = aNext[k] as ToolStripMenuItem;
            m_btnGotoNextSection.DropDownItems.AddRange(v);

        }
        #endregion
        #region Method: void EnableItalicsButton() - unique function for faster speed
        public void EnableItalicsButton()
        {
            m_btnItalic.Enabled = canItalic;
        }
        #endregion
        #region Method: void SetupChangeParagraphToDropdown()
        private void SetupChangeParagraphToDropdown()
        {
            // If not visible, nothing to do (the Available attr is what Windows uses,
            // because the item isn't Visible until it is actually displayed; but
            // Available means it will be displayed when the menu it put together
            // for display upon dropdown.
            if (m_menuChangeParagraphTo.Available == false)
                return;

            // Clear out any previous subitems
            m_menuChangeParagraphTo.DropDownItems.Clear();

            // Get the currently-selected paragraph
            OWWindow wnd = FocusedWindow;
            if (null == wnd)
                return;
            OWWindow.Sel selection = wnd.Selection;
            if (null == selection)
                return;
            DParagraph p = selection.DBT.Paragraph;
            if (null == p)
                return;

            // Get the paragraph's possible styles
            var vPossibileStyles = p.CanChangeParagraphStyleTo;

            // Create menu items for each of these
            foreach (var style in vPossibileStyles)
            {
                var mi = new ToolStripMenuItem(
                    style.StyleName,
                    null,
                    cmdChangeParagraphStyle,
                    "m_menuChangeParagraphStyle_" + style.StyleName);
                mi.Tag = style;

                if (style == p.Style)
                    mi.Checked = true;

                m_menuChangeParagraphTo.DropDownItems.Add(mi);
            }
        }
        #endregion
        #region Method: void EnableMenusAndToolbars()
        public void EnableMenusAndToolbars()
        {
            bool bValidProjectWithData = DB.IsValidProject &&
                null != DB.FrontSection && null != DB.TargetSection;

            // Project
            m_menuSaveProject.Enabled = DB.IsValidProject;

            // Print
            m_btnPrint.Enabled = bValidProjectWithData;

            // Editing
            bool bCanEdit = (CurrentLayout.Focused && TargetIsLocked) ? false : true;
            m_btnEditCut.Enabled = bCanEdit;
            m_btnEditPaste.Enabled = bCanEdit;
            EnableItalicsButton();
            m_menuCut.Enabled = bCanEdit;
            m_menuPaste.Enabled = bCanEdit;
            m_menuChangeParagraphTo.Enabled = bCanEdit;
            m_menuInsertFootnote.Enabled = bCanEdit;
            m_menuDeleteFootnote.Enabled = bCanEdit;

            // Go To menu
            bool bIsAtFirstSection = bValidProjectWithData && DB.Project.Nav.IsAtFirstSection;
            m_btnGotoFirstSection.Enabled = bValidProjectWithData && !bIsAtFirstSection;
            m_btnGotoPreviousSection.Enabled = bValidProjectWithData && !bIsAtFirstSection;
            bool bIsAtLastSection = bValidProjectWithData && DB.Project.Nav.IsAtLastSection;
            m_btnGotoNextSection.Enabled = bValidProjectWithData && !bIsAtLastSection;
            m_btnGotoLastSection.Enabled = bValidProjectWithData && !bIsAtLastSection;
            m_btnChapter.Enabled = DB.IsValidProject;
            m_btnGoToBook.Enabled = DB.IsValidProject;

            // Tools menu
            m_menuIncrementBookStatus.Enabled = canIncrementBookStatus;

            // Check the current "job" on the Window menu
            WLayout.CheckWindowMenuItem(m_btnWindow);
        }
        #endregion
        #region Method: void SetupMenusAndToolbarsVisibility()
        void SetupMenusAndToolbarsVisibility()
            // Turn features on/off according to settings and environment
            //
            // Notes:
            //  - The Tools menu has GotoNext and GotoPrevious items. These are defined as
            //      NotVisible always. They exist only to provide the ShortcutKey methods
            //      of quickly going to Next/Previous sections.
        {
            // Project - If we have an invalid project, we turn the Project menu on regardless
            // of the user setting. As for Export, we don't turn it on unless we do have
            // a valid project.
            m_menuNewProject.Available = (!DB.IsValidProject || 
                Users.Current.CanCreateProject ||
                !Users.HasAdministrator);
            m_menuOpenProject.Available = (!DB.IsValidProject || 
                Users.Current.CanOpenProject ||
                !Users.HasAdministrator);
            m_menuExportProject.Available = (
                DB.IsValidProject && 
                DB.TargetTranslation.BookList.Count > 0 &&
                Users.Current.CanExportProject);
            m_btnProject.Available = (m_menuNewProject.Available ||
                                    m_menuOpenProject.Available ||
                                    m_menuExportProject.Available);

            // Print
            m_btnPrint.Visible = Users.Current.CanPrint;

            // Go To First / Last Section
            m_btnGotoFirstSection.Visible = DB.IsValidProject && Users.Current.CanNavigateFirstLast;
            m_btnGotoLastSection.Visible = DB.IsValidProject && Users.Current.CanNavigateFirstLast;

            // Go To Chapter
            m_btnChapter.Visible = DB.IsValidProject && Users.Current.CanNavigateChapter;

            // TEMP: TURN OFF UNTIL IMPLEMENTED
            //m_btnHistory.Visible = false;

            // Tools Menu
            // Configure - Always visible, but password protected
            m_menuConfigure.Visible = true;
            // Restore from Backup
            m_menuRestoreFromBackup.Visible = Users.Current.CanRestoreBackups;
            // Debug Test Suite
            var bShowDebugItems = JW_Registry.GetValue("Debug", false);
            m_separatorDebug.Visible = bShowDebugItems;
            m_menuRunDebugTestSuite.Visible = bShowDebugItems;
            // Filters
            m_menuOnlyShowSectionsThat.Visible = (DB.IsValidProject && Users.Current.CanFilter);
            // Localizer Tool
            m_menuLocalizerTool.Visible = Users.Current.CanLocalize;
            // Synchronize: Only if synch information is set up
            m_Synchronize.Visible = Users.Current.CanSendReceive;

            // Translator Notes
            m_btnInsertNote.Visible = Users.Current.CanMakeNotes;

            // Window Menu in its entirety
            var bShowMainWindowSection = Users.Current.CanDoBackTranslation || 
                Users.Current.CanDoNaturalnessCheck ||
                Users.Current.CanDoConsultantPreparation;
            m_menuDrafting.Visible = bShowMainWindowSection;
            m_menuBackTranslation.Visible = (bShowMainWindowSection && Users.Current.CanDoBackTranslation);
            m_menuConsultantPreparationToolStripMenuItem.Visible =
                (bShowMainWindowSection && Users.Current.CanDoConsultantPreparation);
            m_menuNaturalnessCheck.Visible = (bShowMainWindowSection && Users.Current.CanDoNaturalnessCheck);
            // Separate Zoom from switch-task if both are present
            m_separatorWindow.Visible = bShowMainWindowSection && Users.Current.CanZoom;
            m_menuZoom.Visible = Users.Current.CanZoom;
            m_btnWindow.Visible = bShowMainWindowSection || Users.Current.CanZoom; 

            // Edit Menu / Structured Editing
            var bStructuralEditing = Users.Current.CanEditStructure &&
                WLayout.CurrentLayoutIs(WndDrafting.c_sName);
            var bEditMenuVisible = bStructuralEditing || Users.Current.CanUndoRedo;
            m_seperatorEdit.Visible = bStructuralEditing;
            m_menuChangeParagraphTo.Visible = bStructuralEditing;
            m_menuInsertFootnote.Visible = bStructuralEditing;
            m_menuDeleteFootnote.Visible = bStructuralEditing;
            m_menuEdit.Visible = bEditMenuVisible;
            m_menuUndo.Visible = Users.Current.CanUndoRedo;
            m_menuRedo.Visible = Users.Current.CanUndoRedo;
            m_btnEditCopy.Visible = !bEditMenuVisible;
            m_btnEditCut.Visible = !bEditMenuVisible;
            m_btnEditPaste.Visible = !bEditMenuVisible;

            m_menuCopyBTFromFrontTranslation.Visible = (
                Users.Current.CanDoConsultantPreparation && 
                WLayout.CurrentLayoutIs( new string[] {
                    WndConsultantPreparation.c_sName, WndBackTranslation.c_sName } )
                );

            // Clear dropdown subitems so we don't attempt to localize them
            m_btnGotoPreviousSection.DropDownItems.Clear();
            m_btnGotoNextSection.DropDownItems.Clear();
            m_btnGoToBook.DropDownItems.Clear();

            // Localization
            LocDB.DB.SetPrimary(Users.Current.PrimaryUiLanguage);
            LocDB.DB.SetSecondary(Users.Current.SecondaryUiLanguage);
            LocDB.Localize(m_ToolStrip); 

            // Some submenus happen after localization (otherwise, we'd be adding spurious
            // entries into the localization database)
            SetupNavigationButtons();
            DBookGrouping.PopulateGotoBook(m_btnGoToBook, cmdGotoBook);
            m_btnGoToBook.Visible = (m_btnGoToBook.DropDownItems.Count > 1);

            // Enabling depends on the current editing context
            EnableMenusAndToolbars();
        }
        #endregion

        // Status Bar
        #region Attr{g}: ToolStripStatusLabel StatusLabel1
        public ToolStripStatusLabel StatusLabel1
        {
            get
            {
                return m_StatusMessage1;
            }
        }
        #endregion
        #region Attr{g/s}: string StatusMessage1
        public string StatusMessage1
        {
            get
            {
                return m_StatusMessage1.Text;
            }
            set
            {
                m_StatusMessage1.Text = value;
                m_StatusStrip.Refresh();
            }
        }
        #endregion
        #region Attr{g/s}: string StatusMessage2
        public string StatusMessage2
        {
            get
            {
                return m_StatusMessage2.Text;
            }
            set
            {
                m_StatusMessage2.Text = value;
                m_StatusStrip.Refresh();
            }
        }
        #endregion
        #region Attr{g}: ToolStripProgressBar ProgressBar
        public ToolStripProgressBar ProgressBar
        {
            get
            {
                return m_ProgressBar;
            }
        }
        #endregion
        #endregion

        // Private attributes ----------------------------------------------------------------
		private readonly JW_WindowState m_WindowState;   // Save/restore state on close/launch of app

		// Scaffolding -----------------------------------------------------------------------
		#region Constructor()
		public OurWordMain()
			// Constructor, initializes the application
		{
			// Required for Windows Form Designer support
            this.components = new System.ComponentModel.Container();
			InitializeComponent();

            // Suspend the layout while we create the windows. We create them here in order
            // to get the propert z-order
            // TO DO: Still necessary? Can we move this to OnLoad?
            SuspendLayout();

            // Create the "job" windows
            WLayout.RegisterLayout(m_panelContents, new WndDrafting());
            WLayout.RegisterLayout(m_panelContents, new WndNaturalness());
            WLayout.RegisterLayout(m_panelContents, new WndBackTranslation());
            WLayout.RegisterLayout(m_panelContents, new WndConsultantPreparation());

            // Let the window go ahead and proceed with the layout
            ResumeLayout();

			// Initialize the window state mechanism. We'll default to a full screen
			// the first time we are launched.
			m_WindowState = new JW_WindowState(this, true);

            // Create the Undo/Redo Stack
            const int nUndoRedoMaxDepth = 10;
            m_URStack = new UndoRedoStack(nUndoRedoMaxDepth, m_menuUndo, m_menuRedo);

			// Initialize to a blank project (if there is a recent project
			// in the MRU, this will get overridden.)
			DB.Project = new DProject();
		}
		#endregion
		#region Method: void Dispose(...) - cleans up any resources being used
		protected override void Dispose( bool disposing )
			// Clean up any resources being used.
			// disposing - true to release both managed and unmanaged resources; false to
			//    release only unmanaged resources.
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}
		#endregion
		#region Windows Form Designer generated code

		private System.ComponentModel.IContainer components;

		/// -----------------------------------------------------------------------------------
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		/// -----------------------------------------------------------------------------------
		private void InitializeComponent()
		{
            System.Windows.Forms.ToolStripSeparator m_separator2;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OurWordMain));
            this.m_ToolStrip = new System.Windows.Forms.ToolStrip();
            this.m_btnExit = new System.Windows.Forms.ToolStripButton();
            this.m_btnProjectSave = new System.Windows.Forms.ToolStripButton();
            this.m_btnProject = new System.Windows.Forms.ToolStripDropDownButton();
            this.m_menuNewProject = new System.Windows.Forms.ToolStripMenuItem();
            this.m_menuInitializeFromAnInternetRepositoryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.m_menuCreateANewProjectOnThisComputerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.m_menuOpenProject = new System.Windows.Forms.ToolStripMenuItem();
            this.m_menuSaveProject = new System.Windows.Forms.ToolStripMenuItem();
            this.m_menuExportProject = new System.Windows.Forms.ToolStripMenuItem();
            this.m_btnPrint = new System.Windows.Forms.ToolStripButton();
            this.m_separator1 = new System.Windows.Forms.ToolStripSeparator();
            this.m_btnEditCut = new System.Windows.Forms.ToolStripButton();
            this.m_btnEditCopy = new System.Windows.Forms.ToolStripButton();
            this.m_btnEditPaste = new System.Windows.Forms.ToolStripButton();
            this.m_menuEdit = new System.Windows.Forms.ToolStripDropDownButton();
            this.m_menuUndo = new System.Windows.Forms.ToolStripMenuItem();
            this.m_menuRedo = new System.Windows.Forms.ToolStripMenuItem();
            this.m_separatorUndoRedo = new System.Windows.Forms.ToolStripSeparator();
            this.m_menuCut = new System.Windows.Forms.ToolStripMenuItem();
            this.m_menuCopy = new System.Windows.Forms.ToolStripMenuItem();
            this.m_menuPaste = new System.Windows.Forms.ToolStripMenuItem();
            this.m_seperatorEdit = new System.Windows.Forms.ToolStripSeparator();
            this.m_menuChangeParagraphTo = new System.Windows.Forms.ToolStripMenuItem();
            this.m_menuInsertFootnote = new System.Windows.Forms.ToolStripMenuItem();
            this.m_menuDeleteFootnote = new System.Windows.Forms.ToolStripMenuItem();
            this.m_menuCopyBTFromFrontTranslation = new System.Windows.Forms.ToolStripMenuItem();
            this.m_btnItalic = new System.Windows.Forms.ToolStripButton();
            this.m_btnInsertNote = new System.Windows.Forms.ToolStripDropDownButton();
            this.m_btnGotoFirstSection = new System.Windows.Forms.ToolStripButton();
            this.m_btnGotoPreviousSection = new System.Windows.Forms.ToolStripSplitButton();
            this.m_btnGotoNextSection = new System.Windows.Forms.ToolStripSplitButton();
            this.m_btnGotoLastSection = new System.Windows.Forms.ToolStripButton();
            this.m_btnChapter = new System.Windows.Forms.ToolStripDropDownButton();
            this.m_btnGoToBook = new System.Windows.Forms.ToolStripDropDownButton();
            this.m_separator3 = new System.Windows.Forms.ToolStripSeparator();
            this.m_btnHistory = new System.Windows.Forms.ToolStripButton();
            this.m_btnTools = new System.Windows.Forms.ToolStripDropDownButton();
            this.m_menuIncrementBookStatus = new System.Windows.Forms.ToolStripMenuItem();
            this.m_menuRestoreFromBackup = new System.Windows.Forms.ToolStripMenuItem();
            this.m_menuOnlyShowSectionsThat = new System.Windows.Forms.ToolStripMenuItem();
            this.m_menuConfigure = new System.Windows.Forms.ToolStripMenuItem();
            this.checkForUpdatesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.m_Synchronize = new System.Windows.Forms.ToolStripMenuItem();
            this.m_menuLocalizerTool = new System.Windows.Forms.ToolStripMenuItem();
            this.m_separatorDebug = new System.Windows.Forms.ToolStripSeparator();
            this.m_menuRunDebugTestSuite = new System.Windows.Forms.ToolStripMenuItem();
            this.m_menuPreviousSection = new System.Windows.Forms.ToolStripMenuItem();
            this.m_menuNextSection = new System.Windows.Forms.ToolStripMenuItem();
            this.m_menuToggleItalics = new System.Windows.Forms.ToolStripMenuItem();
            this.m_btnWindow = new System.Windows.Forms.ToolStripDropDownButton();
            this.m_menuDrafting = new System.Windows.Forms.ToolStripMenuItem();
            this.m_menuNaturalnessCheck = new System.Windows.Forms.ToolStripMenuItem();
            this.m_menuBackTranslation = new System.Windows.Forms.ToolStripMenuItem();
            this.m_menuConsultantPreparationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.m_separatorWindow = new System.Windows.Forms.ToolStripSeparator();
            this.m_menuZoom = new System.Windows.Forms.ToolStripMenuItem();
            this.m_separator4 = new System.Windows.Forms.ToolStripSeparator();
            this.m_btnHelp = new System.Windows.Forms.ToolStripDropDownButton();
            this.m_menuHelpTopics = new System.Windows.Forms.ToolStripMenuItem();
            this.m_menuAbout = new System.Windows.Forms.ToolStripMenuItem();
            this.m_toolStripContainer = new System.Windows.Forms.ToolStripContainer();
            this.m_StatusStrip = new System.Windows.Forms.StatusStrip();
            this.m_StatusMessage1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.m_ProgressBar = new System.Windows.Forms.ToolStripProgressBar();
            this.m_StatusMessage2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.m_panelContents = new System.Windows.Forms.Panel();
            this.m_ViewBar = new OurWord.Utilities.ViewBar();
            m_separator2 = new System.Windows.Forms.ToolStripSeparator();
            this.m_ToolStrip.SuspendLayout();
            this.m_toolStripContainer.BottomToolStripPanel.SuspendLayout();
            this.m_toolStripContainer.ContentPanel.SuspendLayout();
            this.m_toolStripContainer.TopToolStripPanel.SuspendLayout();
            this.m_toolStripContainer.SuspendLayout();
            this.m_StatusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // m_separator2
            // 
            m_separator2.Name = "m_separator2";
            m_separator2.Size = new System.Drawing.Size(6, 38);
            // 
            // m_ToolStrip
            // 
            this.m_ToolStrip.Dock = System.Windows.Forms.DockStyle.None;
            this.m_ToolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.m_ToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_btnExit,
            this.m_btnProjectSave,
            this.m_btnProject,
            this.m_btnPrint,
            this.m_separator1,
            this.m_btnEditCut,
            this.m_btnEditCopy,
            this.m_btnEditPaste,
            this.m_menuEdit,
            this.m_btnItalic,
            this.m_btnInsertNote,
            m_separator2,
            this.m_btnGotoFirstSection,
            this.m_btnGotoPreviousSection,
            this.m_btnGotoNextSection,
            this.m_btnGotoLastSection,
            this.m_btnChapter,
            this.m_btnGoToBook,
            this.m_separator3,
            this.m_btnHistory,
            this.m_btnTools,
            this.m_btnWindow,
            this.m_separator4,
            this.m_btnHelp});
            this.m_ToolStrip.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
            this.m_ToolStrip.Location = new System.Drawing.Point(3, 25);
            this.m_ToolStrip.Name = "m_ToolStrip";
            this.m_ToolStrip.Size = new System.Drawing.Size(943, 38);
            this.m_ToolStrip.TabIndex = 1;
            // 
            // m_btnExit
            // 
            this.m_btnExit.Image = ((System.Drawing.Image)(resources.GetObject("m_btnExit.Image")));
            this.m_btnExit.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.m_btnExit.Name = "m_btnExit";
            this.m_btnExit.Size = new System.Drawing.Size(29, 35);
            this.m_btnExit.Text = "Exit";
            this.m_btnExit.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.m_btnExit.ToolTipText = "Save any changes and then exit OurWord.";
            this.m_btnExit.Click += new System.EventHandler(this.cmdExit);
            // 
            // m_btnProjectSave
            // 
            this.m_btnProjectSave.Image = ((System.Drawing.Image)(resources.GetObject("m_btnProjectSave.Image")));
            this.m_btnProjectSave.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.m_btnProjectSave.Name = "m_btnProjectSave";
            this.m_btnProjectSave.Size = new System.Drawing.Size(35, 35);
            this.m_btnProjectSave.Text = "Save";
            this.m_btnProjectSave.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.m_btnProjectSave.ToolTipText = "Saves your work to disk.";
            this.m_btnProjectSave.Click += new System.EventHandler(this.cmdSaveProject);
            // 
            // m_btnProject
            // 
            this.m_btnProject.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_menuNewProject,
            this.m_menuOpenProject,
            this.m_menuSaveProject,
            this.m_menuExportProject});
            this.m_btnProject.Image = ((System.Drawing.Image)(resources.GetObject("m_btnProject.Image")));
            this.m_btnProject.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.m_btnProject.Name = "m_btnProject";
            this.m_btnProject.Size = new System.Drawing.Size(57, 35);
            this.m_btnProject.Text = "Project";
            this.m_btnProject.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.m_btnProject.ToolTipText = "Shows the menu for working with a project (New, Open, etc.).";
            this.m_btnProject.DropDownOpening += new System.EventHandler(this.cmdSetupOpening);
            // 
            // m_menuNewProject
            // 
            this.m_menuNewProject.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_menuInitializeFromAnInternetRepositoryToolStripMenuItem,
            this.m_menuCreateANewProjectOnThisComputerToolStripMenuItem});
            this.m_menuNewProject.Image = ((System.Drawing.Image)(resources.GetObject("m_menuNewProject.Image")));
            this.m_menuNewProject.Name = "m_menuNewProject";
            this.m_menuNewProject.Size = new System.Drawing.Size(146, 22);
            this.m_menuNewProject.Text = "&New";
            this.m_menuNewProject.ToolTipText = "Create a brand new project.";
            // 
            // m_menuInitializeFromAnInternetRepositoryToolStripMenuItem
            // 
            this.m_menuInitializeFromAnInternetRepositoryToolStripMenuItem.Name = "m_menuInitializeFromAnInternetRepositoryToolStripMenuItem";
            this.m_menuInitializeFromAnInternetRepositoryToolStripMenuItem.Size = new System.Drawing.Size(287, 22);
            this.m_menuInitializeFromAnInternetRepositoryToolStripMenuItem.Text = "Initialize from an Internet Repository...";
            this.m_menuInitializeFromAnInternetRepositoryToolStripMenuItem.Click += new System.EventHandler(this.cmdDownloadRepository);
            // 
            // m_menuCreateANewProjectOnThisComputerToolStripMenuItem
            // 
            this.m_menuCreateANewProjectOnThisComputerToolStripMenuItem.Name = "m_menuCreateANewProjectOnThisComputerToolStripMenuItem";
            this.m_menuCreateANewProjectOnThisComputerToolStripMenuItem.Size = new System.Drawing.Size(287, 22);
            this.m_menuCreateANewProjectOnThisComputerToolStripMenuItem.Text = "Create a New Project on this computer...";
            this.m_menuCreateANewProjectOnThisComputerToolStripMenuItem.Click += new System.EventHandler(this.cmdNewProject);
            // 
            // m_menuOpenProject
            // 
            this.m_menuOpenProject.Image = ((System.Drawing.Image)(resources.GetObject("m_menuOpenProject.Image")));
            this.m_menuOpenProject.Name = "m_menuOpenProject";
            this.m_menuOpenProject.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.m_menuOpenProject.Size = new System.Drawing.Size(146, 22);
            this.m_menuOpenProject.Text = "&Open";
            this.m_menuOpenProject.ToolTipText = "Open an existing project.";
            // 
            // m_menuSaveProject
            // 
            this.m_menuSaveProject.Image = ((System.Drawing.Image)(resources.GetObject("m_menuSaveProject.Image")));
            this.m_menuSaveProject.Name = "m_menuSaveProject";
            this.m_menuSaveProject.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.m_menuSaveProject.Size = new System.Drawing.Size(146, 22);
            this.m_menuSaveProject.Text = "&Save";
            this.m_menuSaveProject.ToolTipText = "Save this project and any edited books to the disk.";
            this.m_menuSaveProject.Click += new System.EventHandler(this.cmdSaveProject);
            // 
            // m_menuExportProject
            // 
            this.m_menuExportProject.Name = "m_menuExportProject";
            this.m_menuExportProject.Size = new System.Drawing.Size(146, 22);
            this.m_menuExportProject.Text = "Export...";
            this.m_menuExportProject.Click += new System.EventHandler(this.cmdExportProject);
            // 
            // m_btnPrint
            // 
            this.m_btnPrint.Image = ((System.Drawing.Image)(resources.GetObject("m_btnPrint.Image")));
            this.m_btnPrint.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.m_btnPrint.Name = "m_btnPrint";
            this.m_btnPrint.Size = new System.Drawing.Size(45, 35);
            this.m_btnPrint.Text = "Print...";
            this.m_btnPrint.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.m_btnPrint.ToolTipText = "Print this book (or parts of this book.)";
            this.m_btnPrint.Click += new System.EventHandler(this.cmdPrint);
            // 
            // m_separator1
            // 
            this.m_separator1.Name = "m_separator1";
            this.m_separator1.Size = new System.Drawing.Size(6, 38);
            // 
            // m_btnEditCut
            // 
            this.m_btnEditCut.Image = ((System.Drawing.Image)(resources.GetObject("m_btnEditCut.Image")));
            this.m_btnEditCut.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.m_btnEditCut.Name = "m_btnEditCut";
            this.m_btnEditCut.Size = new System.Drawing.Size(30, 35);
            this.m_btnEditCut.Text = "Cut";
            this.m_btnEditCut.TextDirection = System.Windows.Forms.ToolStripTextDirection.Horizontal;
            this.m_btnEditCut.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.m_btnEditCut.ToolTipText = "Removes the selected text and places it on the clipboard.";
            this.m_btnEditCut.Click += new System.EventHandler(this.cmdEditCut);
            // 
            // m_btnEditCopy
            // 
            this.m_btnEditCopy.Image = ((System.Drawing.Image)(resources.GetObject("m_btnEditCopy.Image")));
            this.m_btnEditCopy.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.m_btnEditCopy.Name = "m_btnEditCopy";
            this.m_btnEditCopy.Size = new System.Drawing.Size(39, 35);
            this.m_btnEditCopy.Text = "Copy";
            this.m_btnEditCopy.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.m_btnEditCopy.ToolTipText = "Copies the selected text to the clipboard.";
            this.m_btnEditCopy.Click += new System.EventHandler(this.cmdEditCopy);
            // 
            // m_btnEditPaste
            // 
            this.m_btnEditPaste.Image = ((System.Drawing.Image)(resources.GetObject("m_btnEditPaste.Image")));
            this.m_btnEditPaste.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.m_btnEditPaste.Name = "m_btnEditPaste";
            this.m_btnEditPaste.Size = new System.Drawing.Size(39, 35);
            this.m_btnEditPaste.Text = "Paste";
            this.m_btnEditPaste.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.m_btnEditPaste.ToolTipText = "Pastes the text at the clipboard to the cursor.";
            this.m_btnEditPaste.Click += new System.EventHandler(this.cmdEditPaste);
            // 
            // m_menuEdit
            // 
            this.m_menuEdit.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_menuUndo,
            this.m_menuRedo,
            this.m_separatorUndoRedo,
            this.m_menuCut,
            this.m_menuCopy,
            this.m_menuPaste,
            this.m_seperatorEdit,
            this.m_menuChangeParagraphTo,
            this.m_menuInsertFootnote,
            this.m_menuDeleteFootnote,
            this.m_menuCopyBTFromFrontTranslation});
            this.m_menuEdit.Image = ((System.Drawing.Image)(resources.GetObject("m_menuEdit.Image")));
            this.m_menuEdit.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.m_menuEdit.Name = "m_menuEdit";
            this.m_menuEdit.Size = new System.Drawing.Size(40, 35);
            this.m_menuEdit.Text = "Edit";
            this.m_menuEdit.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.m_menuEdit.ToolTipText = "Displays the Edit menu items.";
            this.m_menuEdit.DropDownOpening += new System.EventHandler(this.cmdEditDropdownOpening);
            // 
            // m_menuUndo
            // 
            this.m_menuUndo.Image = ((System.Drawing.Image)(resources.GetObject("m_menuUndo.Image")));
            this.m_menuUndo.Name = "m_menuUndo";
            this.m_menuUndo.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Z)));
            this.m_menuUndo.Size = new System.Drawing.Size(189, 22);
            this.m_menuUndo.Text = "&Undo";
            this.m_menuUndo.Click += new System.EventHandler(this.cmdUndo);
            // 
            // m_menuRedo
            // 
            this.m_menuRedo.Image = ((System.Drawing.Image)(resources.GetObject("m_menuRedo.Image")));
            this.m_menuRedo.Name = "m_menuRedo";
            this.m_menuRedo.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Y)));
            this.m_menuRedo.Size = new System.Drawing.Size(189, 22);
            this.m_menuRedo.Text = "&Redo";
            this.m_menuRedo.Click += new System.EventHandler(this.cmdRedo);
            // 
            // m_separatorUndoRedo
            // 
            this.m_separatorUndoRedo.Name = "m_separatorUndoRedo";
            this.m_separatorUndoRedo.Size = new System.Drawing.Size(186, 6);
            // 
            // m_menuCut
            // 
            this.m_menuCut.Image = ((System.Drawing.Image)(resources.GetObject("m_menuCut.Image")));
            this.m_menuCut.Name = "m_menuCut";
            this.m_menuCut.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.X)));
            this.m_menuCut.Size = new System.Drawing.Size(189, 22);
            this.m_menuCut.Text = "Cu&t";
            this.m_menuCut.ToolTipText = "Removes the selected text and places it on the clipboard.";
            this.m_menuCut.Click += new System.EventHandler(this.cmdEditCut);
            // 
            // m_menuCopy
            // 
            this.m_menuCopy.Image = ((System.Drawing.Image)(resources.GetObject("m_menuCopy.Image")));
            this.m_menuCopy.Name = "m_menuCopy";
            this.m_menuCopy.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.m_menuCopy.Size = new System.Drawing.Size(189, 22);
            this.m_menuCopy.Text = "&Copy";
            this.m_menuCopy.ToolTipText = "Copies the selected text to the clipboard.";
            this.m_menuCopy.Click += new System.EventHandler(this.cmdEditCopy);
            // 
            // m_menuPaste
            // 
            this.m_menuPaste.Image = ((System.Drawing.Image)(resources.GetObject("m_menuPaste.Image")));
            this.m_menuPaste.Name = "m_menuPaste";
            this.m_menuPaste.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V)));
            this.m_menuPaste.Size = new System.Drawing.Size(189, 22);
            this.m_menuPaste.Text = "&Paste";
            this.m_menuPaste.ToolTipText = "Pastes the text at the clipboard to the cursor.";
            this.m_menuPaste.Click += new System.EventHandler(this.cmdEditPaste);
            // 
            // m_seperatorEdit
            // 
            this.m_seperatorEdit.Name = "m_seperatorEdit";
            this.m_seperatorEdit.Size = new System.Drawing.Size(186, 6);
            // 
            // m_menuChangeParagraphTo
            // 
            this.m_menuChangeParagraphTo.Name = "m_menuChangeParagraphTo";
            this.m_menuChangeParagraphTo.Size = new System.Drawing.Size(189, 22);
            this.m_menuChangeParagraphTo.Text = "C&hange Paragraph To";
            this.m_menuChangeParagraphTo.ToolTipText = "Changes the style of the selected paragraph to another style.";
            // 
            // m_menuInsertFootnote
            // 
            this.m_menuInsertFootnote.Image = ((System.Drawing.Image)(resources.GetObject("m_menuInsertFootnote.Image")));
            this.m_menuInsertFootnote.Name = "m_menuInsertFootnote";
            this.m_menuInsertFootnote.Size = new System.Drawing.Size(189, 22);
            this.m_menuInsertFootnote.Text = "Insert Footnote";
            this.m_menuInsertFootnote.Click += new System.EventHandler(this.cmdInsertFootnote);
            // 
            // m_menuDeleteFootnote
            // 
            this.m_menuDeleteFootnote.Image = ((System.Drawing.Image)(resources.GetObject("m_menuDeleteFootnote.Image")));
            this.m_menuDeleteFootnote.Name = "m_menuDeleteFootnote";
            this.m_menuDeleteFootnote.Size = new System.Drawing.Size(189, 22);
            this.m_menuDeleteFootnote.Text = "Delete Footnote";
            this.m_menuDeleteFootnote.Click += new System.EventHandler(this.cmdDeleteFootnote);
            // 
            // m_menuCopyBTFromFrontTranslation
            // 
            this.m_menuCopyBTFromFrontTranslation.Name = "m_menuCopyBTFromFrontTranslation";
            this.m_menuCopyBTFromFrontTranslation.Size = new System.Drawing.Size(189, 22);
            this.m_menuCopyBTFromFrontTranslation.Text = "Copy BT from &Front...";
            this.m_menuCopyBTFromFrontTranslation.Click += new System.EventHandler(this.cmdCopyBTFromFront);
            // 
            // m_btnItalic
            // 
            this.m_btnItalic.Image = ((System.Drawing.Image)(resources.GetObject("m_btnItalic.Image")));
            this.m_btnItalic.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.m_btnItalic.Name = "m_btnItalic";
            this.m_btnItalic.Size = new System.Drawing.Size(36, 35);
            this.m_btnItalic.Text = "Italic";
            this.m_btnItalic.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.m_btnItalic.ToolTipText = "Changes the selected text to Italic; this is only enabled if Italic is allowed in" +
                " the current context.";
            this.m_btnItalic.Click += new System.EventHandler(this.cmdItalic);
            // 
            // m_btnInsertNote
            // 
            this.m_btnInsertNote.Image = ((System.Drawing.Image)(resources.GetObject("m_btnInsertNote.Image")));
            this.m_btnInsertNote.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.m_btnInsertNote.Name = "m_btnInsertNote";
            this.m_btnInsertNote.ShowDropDownArrow = false;
            this.m_btnInsertNote.Size = new System.Drawing.Size(40, 35);
            this.m_btnInsertNote.Tag = "General";
            this.m_btnInsertNote.Text = "Insert";
            this.m_btnInsertNote.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.m_btnInsertNote.ToolTipText = "Insert a Translator Note";
            this.m_btnInsertNote.Click += new System.EventHandler(this.cmdInsertNote);
            // 
            // m_btnGotoFirstSection
            // 
            this.m_btnGotoFirstSection.Image = ((System.Drawing.Image)(resources.GetObject("m_btnGotoFirstSection.Image")));
            this.m_btnGotoFirstSection.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.m_btnGotoFirstSection.Name = "m_btnGotoFirstSection";
            this.m_btnGotoFirstSection.Size = new System.Drawing.Size(33, 35);
            this.m_btnGotoFirstSection.Text = "First";
            this.m_btnGotoFirstSection.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.m_btnGotoFirstSection.ToolTipText = "Go to the First section in the book.";
            this.m_btnGotoFirstSection.Click += new System.EventHandler(this.cmdGoToFirstSection);
            // 
            // m_btnGotoPreviousSection
            // 
            this.m_btnGotoPreviousSection.Image = ((System.Drawing.Image)(resources.GetObject("m_btnGotoPreviousSection.Image")));
            this.m_btnGotoPreviousSection.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.m_btnGotoPreviousSection.Name = "m_btnGotoPreviousSection";
            this.m_btnGotoPreviousSection.Size = new System.Drawing.Size(68, 35);
            this.m_btnGotoPreviousSection.Text = "Previous";
            this.m_btnGotoPreviousSection.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.m_btnGotoPreviousSection.ToolTipText = "Go to the Previous section in the book.";
            this.m_btnGotoPreviousSection.ButtonClick += new System.EventHandler(this.cmdGoToPreviousSection);
            // 
            // m_btnGotoNextSection
            // 
            this.m_btnGotoNextSection.Image = ((System.Drawing.Image)(resources.GetObject("m_btnGotoNextSection.Image")));
            this.m_btnGotoNextSection.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.m_btnGotoNextSection.Name = "m_btnGotoNextSection";
            this.m_btnGotoNextSection.Size = new System.Drawing.Size(47, 35);
            this.m_btnGotoNextSection.Text = "Next";
            this.m_btnGotoNextSection.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.m_btnGotoNextSection.ToolTipText = "Go to the Next section in the book.";
            this.m_btnGotoNextSection.ButtonClick += new System.EventHandler(this.cmdGoToNextSection);
            // 
            // m_btnGotoLastSection
            // 
            this.m_btnGotoLastSection.Image = ((System.Drawing.Image)(resources.GetObject("m_btnGotoLastSection.Image")));
            this.m_btnGotoLastSection.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.m_btnGotoLastSection.Name = "m_btnGotoLastSection";
            this.m_btnGotoLastSection.Size = new System.Drawing.Size(32, 35);
            this.m_btnGotoLastSection.Text = "Last";
            this.m_btnGotoLastSection.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.m_btnGotoLastSection.ToolTipText = "Go to the final section in the book.";
            this.m_btnGotoLastSection.Click += new System.EventHandler(this.cmdGoToLastSection);
            // 
            // m_btnChapter
            // 
            this.m_btnChapter.Image = ((System.Drawing.Image)(resources.GetObject("m_btnChapter.Image")));
            this.m_btnChapter.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.m_btnChapter.Name = "m_btnChapter";
            this.m_btnChapter.Size = new System.Drawing.Size(62, 35);
            this.m_btnChapter.Text = "Chapter";
            this.m_btnChapter.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.m_btnChapter.ToolTipText = "Go to a Chapter in the book";
            this.m_btnChapter.Click += new System.EventHandler(this.cmdGoToChapter);
            // 
            // m_btnGoToBook
            // 
            this.m_btnGoToBook.Image = ((System.Drawing.Image)(resources.GetObject("m_btnGoToBook.Image")));
            this.m_btnGoToBook.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.m_btnGoToBook.Name = "m_btnGoToBook";
            this.m_btnGoToBook.Size = new System.Drawing.Size(47, 35);
            this.m_btnGoToBook.Text = "Book";
            this.m_btnGoToBook.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.m_btnGoToBook.ToolTipText = "Go to a different book";
            // 
            // m_separator3
            // 
            this.m_separator3.Name = "m_separator3";
            this.m_separator3.Size = new System.Drawing.Size(6, 38);
            // 
            // m_btnHistory
            // 
            this.m_btnHistory.Image = ((System.Drawing.Image)(resources.GetObject("m_btnHistory.Image")));
            this.m_btnHistory.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.m_btnHistory.Name = "m_btnHistory";
            this.m_btnHistory.Size = new System.Drawing.Size(49, 35);
            this.m_btnHistory.Text = "History";
            this.m_btnHistory.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.m_btnHistory.Click += new System.EventHandler(this.cmdHistory);
            // 
            // m_btnTools
            // 
            this.m_btnTools.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_menuIncrementBookStatus,
            this.m_menuRestoreFromBackup,
            this.m_menuOnlyShowSectionsThat,
            this.m_menuConfigure,
            this.checkForUpdatesToolStripMenuItem,
            this.m_Synchronize,
            this.m_menuLocalizerTool,
            this.m_separatorDebug,
            this.m_menuRunDebugTestSuite,
            this.m_menuPreviousSection,
            this.m_menuNextSection,
            this.m_menuToggleItalics});
            this.m_btnTools.Image = ((System.Drawing.Image)(resources.GetObject("m_btnTools.Image")));
            this.m_btnTools.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.m_btnTools.Name = "m_btnTools";
            this.m_btnTools.Size = new System.Drawing.Size(49, 35);
            this.m_btnTools.Text = "Tools";
            this.m_btnTools.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.m_btnTools.ToolTipText = "Opens the menu showing the different tools that are available.";
            // 
            // m_menuIncrementBookStatus
            // 
            this.m_menuIncrementBookStatus.Name = "m_menuIncrementBookStatus";
            this.m_menuIncrementBookStatus.Size = new System.Drawing.Size(211, 22);
            this.m_menuIncrementBookStatus.Text = "&Increment Book Status...";
            this.m_menuIncrementBookStatus.ToolTipText = "Change the book\'s status to the next one in the translation process.";
            this.m_menuIncrementBookStatus.Click += new System.EventHandler(this.cmdIncrementBookStatus);
            // 
            // m_menuRestoreFromBackup
            // 
            this.m_menuRestoreFromBackup.Name = "m_menuRestoreFromBackup";
            this.m_menuRestoreFromBackup.Size = new System.Drawing.Size(211, 22);
            this.m_menuRestoreFromBackup.Text = "Restore from &Backup...";
            this.m_menuRestoreFromBackup.ToolTipText = "Restore to a former version of the book.";
            this.m_menuRestoreFromBackup.Click += new System.EventHandler(this.cmdRestoreBackup);
            // 
            // m_menuOnlyShowSectionsThat
            // 
            this.m_menuOnlyShowSectionsThat.Name = "m_menuOnlyShowSectionsThat";
            this.m_menuOnlyShowSectionsThat.Size = new System.Drawing.Size(211, 22);
            this.m_menuOnlyShowSectionsThat.Text = "Only &Show Sections that...";
            this.m_menuOnlyShowSectionsThat.ToolTipText = "Filter out the sections which do not conform to a criteria.";
            this.m_menuOnlyShowSectionsThat.Click += new System.EventHandler(this.cmdFilter);
            // 
            // m_menuConfigure
            // 
            this.m_menuConfigure.Image = ((System.Drawing.Image)(resources.GetObject("m_menuConfigure.Image")));
            this.m_menuConfigure.Name = "m_menuConfigure";
            this.m_menuConfigure.Size = new System.Drawing.Size(211, 22);
            this.m_menuConfigure.Text = "&Configure...";
            this.m_menuConfigure.ToolTipText = "Edit the settings for OurWord and for the current project.";
            this.m_menuConfigure.Click += new System.EventHandler(this.cmdConfigure);
            // 
            // checkForUpdatesToolStripMenuItem
            // 
            this.checkForUpdatesToolStripMenuItem.Name = "checkForUpdatesToolStripMenuItem";
            this.checkForUpdatesToolStripMenuItem.Size = new System.Drawing.Size(211, 22);
            this.checkForUpdatesToolStripMenuItem.Text = "Check for &Updates...";
            this.checkForUpdatesToolStripMenuItem.Click += new System.EventHandler(this.cmdCheckForUpdates);
            // 
            // m_Synchronize
            // 
            this.m_Synchronize.Image = ((System.Drawing.Image)(resources.GetObject("m_Synchronize.Image")));
            this.m_Synchronize.Name = "m_Synchronize";
            this.m_Synchronize.Size = new System.Drawing.Size(211, 22);
            this.m_Synchronize.Text = "Send / &Receive";
            this.m_Synchronize.Click += new System.EventHandler(this.cmdSynchronize);
            // 
            // m_menuLocalizerTool
            // 
            this.m_menuLocalizerTool.Name = "m_menuLocalizerTool";
            this.m_menuLocalizerTool.Size = new System.Drawing.Size(211, 22);
            this.m_menuLocalizerTool.Text = "&Localizer Tool...";
            this.m_menuLocalizerTool.ToolTipText = "Create a versin of OurWord in another language.";
            this.m_menuLocalizerTool.Click += new System.EventHandler(this.cmdLocalizer);
            // 
            // m_separatorDebug
            // 
            this.m_separatorDebug.Name = "m_separatorDebug";
            this.m_separatorDebug.Size = new System.Drawing.Size(208, 6);
            // 
            // m_menuRunDebugTestSuite
            // 
            this.m_menuRunDebugTestSuite.Name = "m_menuRunDebugTestSuite";
            this.m_menuRunDebugTestSuite.Size = new System.Drawing.Size(211, 22);
            this.m_menuRunDebugTestSuite.Text = "Run Debug Test Suite...";
            this.m_menuRunDebugTestSuite.ToolTipText = "Only programmers will generally see this; you should ignore it!";
            this.m_menuRunDebugTestSuite.Click += new System.EventHandler(this.cmdDebugTesting);
            // 
            // m_menuPreviousSection
            // 
            this.m_menuPreviousSection.Image = ((System.Drawing.Image)(resources.GetObject("m_menuPreviousSection.Image")));
            this.m_menuPreviousSection.Name = "m_menuPreviousSection";
            this.m_menuPreviousSection.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.P)));
            this.m_menuPreviousSection.Size = new System.Drawing.Size(211, 22);
            this.m_menuPreviousSection.Text = "Previous Section";
            this.m_menuPreviousSection.Visible = false;
            this.m_menuPreviousSection.Click += new System.EventHandler(this.cmdGoToPreviousSection);
            // 
            // m_menuNextSection
            // 
            this.m_menuNextSection.Image = ((System.Drawing.Image)(resources.GetObject("m_menuNextSection.Image")));
            this.m_menuNextSection.Name = "m_menuNextSection";
            this.m_menuNextSection.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.m_menuNextSection.Size = new System.Drawing.Size(211, 22);
            this.m_menuNextSection.Text = "Next Section";
            this.m_menuNextSection.Visible = false;
            this.m_menuNextSection.Click += new System.EventHandler(this.cmdGoToNextSection);
            // 
            // m_menuToggleItalics
            // 
            this.m_menuToggleItalics.Image = ((System.Drawing.Image)(resources.GetObject("m_menuToggleItalics.Image")));
            this.m_menuToggleItalics.Name = "m_menuToggleItalics";
            this.m_menuToggleItalics.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.I)));
            this.m_menuToggleItalics.Size = new System.Drawing.Size(211, 22);
            this.m_menuToggleItalics.Text = "Toggle Italics";
            this.m_menuToggleItalics.Visible = false;
            this.m_menuToggleItalics.Click += new System.EventHandler(this.cmdItalic);
            // 
            // m_btnWindow
            // 
            this.m_btnWindow.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_menuDrafting,
            this.m_menuNaturalnessCheck,
            this.m_menuBackTranslation,
            this.m_menuConsultantPreparationToolStripMenuItem,
            this.m_separatorWindow,
            this.m_menuZoom});
            this.m_btnWindow.Image = ((System.Drawing.Image)(resources.GetObject("m_btnWindow.Image")));
            this.m_btnWindow.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.m_btnWindow.Name = "m_btnWindow";
            this.m_btnWindow.Size = new System.Drawing.Size(64, 35);
            this.m_btnWindow.Text = "Window";
            this.m_btnWindow.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.m_btnWindow.ToolTipText = "Shows the various windows you can turn on and off.";
            this.m_btnWindow.DropDownOpening += new System.EventHandler(this.cmdWindowDropDownOpening);
            // 
            // m_menuDrafting
            // 
            this.m_menuDrafting.Image = ((System.Drawing.Image)(resources.GetObject("m_menuDrafting.Image")));
            this.m_menuDrafting.Name = "m_menuDrafting";
            this.m_menuDrafting.Size = new System.Drawing.Size(196, 22);
            this.m_menuDrafting.Tag = "Draft";
            this.m_menuDrafting.Text = "&Drafting";
            this.m_menuDrafting.ToolTipText = "Set the main window to do Drafting.";
            this.m_menuDrafting.Click += new System.EventHandler(this.cmdJobDrafting);
            // 
            // m_menuNaturalnessCheck
            // 
            this.m_menuNaturalnessCheck.Image = ((System.Drawing.Image)(resources.GetObject("m_menuNaturalnessCheck.Image")));
            this.m_menuNaturalnessCheck.Name = "m_menuNaturalnessCheck";
            this.m_menuNaturalnessCheck.Size = new System.Drawing.Size(196, 22);
            this.m_menuNaturalnessCheck.Tag = "Naturalness";
            this.m_menuNaturalnessCheck.Text = "&Naturalness Check";
            this.m_menuNaturalnessCheck.ToolTipText = "Set the main window to do a Naturalness Check (where the Front Translation is not" +
                " displayed.)";
            this.m_menuNaturalnessCheck.Click += new System.EventHandler(this.cmdJobNaturalness);
            // 
            // m_menuBackTranslation
            // 
            this.m_menuBackTranslation.Image = ((System.Drawing.Image)(resources.GetObject("m_menuBackTranslation.Image")));
            this.m_menuBackTranslation.Name = "m_menuBackTranslation";
            this.m_menuBackTranslation.Size = new System.Drawing.Size(196, 22);
            this.m_menuBackTranslation.Tag = "BT";
            this.m_menuBackTranslation.Text = "&Back Translation";
            this.m_menuBackTranslation.ToolTipText = "Set the main window to work on the Back Translation.";
            this.m_menuBackTranslation.Click += new System.EventHandler(this.cmdJobBackTranslation);
            // 
            // m_menuConsultantPreparationToolStripMenuItem
            // 
            this.m_menuConsultantPreparationToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("m_menuConsultantPreparationToolStripMenuItem.Image")));
            this.m_menuConsultantPreparationToolStripMenuItem.Name = "m_menuConsultantPreparationToolStripMenuItem";
            this.m_menuConsultantPreparationToolStripMenuItem.Size = new System.Drawing.Size(196, 22);
            this.m_menuConsultantPreparationToolStripMenuItem.Tag = "ConsultantPreparation";
            this.m_menuConsultantPreparationToolStripMenuItem.Text = "Consultant &Preparation";
            this.m_menuConsultantPreparationToolStripMenuItem.Click += new System.EventHandler(this.cmdJobConsultantPreparation);
            // 
            // m_separatorWindow
            // 
            this.m_separatorWindow.Name = "m_separatorWindow";
            this.m_separatorWindow.Size = new System.Drawing.Size(193, 6);
            // 
            // m_menuZoom
            // 
            this.m_menuZoom.Name = "m_menuZoom";
            this.m_menuZoom.Size = new System.Drawing.Size(196, 22);
            this.m_menuZoom.Text = "Zoom Text";
            // 
            // m_separator4
            // 
            this.m_separator4.Name = "m_separator4";
            this.m_separator4.Size = new System.Drawing.Size(6, 38);
            // 
            // m_btnHelp
            // 
            this.m_btnHelp.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_menuHelpTopics,
            this.m_menuAbout});
            this.m_btnHelp.Image = ((System.Drawing.Image)(resources.GetObject("m_btnHelp.Image")));
            this.m_btnHelp.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.m_btnHelp.Name = "m_btnHelp";
            this.m_btnHelp.Size = new System.Drawing.Size(45, 35);
            this.m_btnHelp.Text = "Help";
            this.m_btnHelp.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.m_btnHelp.ToolTipText = "Show the Help menu.";
            // 
            // m_menuHelpTopics
            // 
            this.m_menuHelpTopics.Image = ((System.Drawing.Image)(resources.GetObject("m_menuHelpTopics.Image")));
            this.m_menuHelpTopics.Name = "m_menuHelpTopics";
            this.m_menuHelpTopics.Size = new System.Drawing.Size(162, 22);
            this.m_menuHelpTopics.Text = "Help &Topics";
            this.m_menuHelpTopics.ToolTipText = "Show the Help window.";
            this.m_menuHelpTopics.Click += new System.EventHandler(this.cmdHelpTopics);
            // 
            // m_menuAbout
            // 
            this.m_menuAbout.Name = "m_menuAbout";
            this.m_menuAbout.Size = new System.Drawing.Size(162, 22);
            this.m_menuAbout.Text = "&About Our Word";
            this.m_menuAbout.ToolTipText = "See information about this version of Our Word.";
            this.m_menuAbout.Click += new System.EventHandler(this.cmdHelpAbout);
            // 
            // m_toolStripContainer
            // 
            // 
            // m_toolStripContainer.BottomToolStripPanel
            // 
            this.m_toolStripContainer.BottomToolStripPanel.Controls.Add(this.m_StatusStrip);
            // 
            // m_toolStripContainer.ContentPanel
            // 
            this.m_toolStripContainer.ContentPanel.AutoScroll = true;
            this.m_toolStripContainer.ContentPanel.Controls.Add(this.m_panelContents);
            this.m_toolStripContainer.ContentPanel.Size = new System.Drawing.Size(946, 441);
            this.m_toolStripContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_toolStripContainer.LeftToolStripPanelVisible = false;
            this.m_toolStripContainer.Location = new System.Drawing.Point(0, 0);
            this.m_toolStripContainer.Name = "m_toolStripContainer";
            this.m_toolStripContainer.RightToolStripPanelVisible = false;
            this.m_toolStripContainer.Size = new System.Drawing.Size(946, 526);
            this.m_toolStripContainer.TabIndex = 2;
            this.m_toolStripContainer.Text = "toolStripContainer1";
            // 
            // m_toolStripContainer.TopToolStripPanel
            // 
            this.m_toolStripContainer.TopToolStripPanel.Controls.Add(this.m_ViewBar);
            this.m_toolStripContainer.TopToolStripPanel.Controls.Add(this.m_ToolStrip);
            // 
            // m_StatusStrip
            // 
            this.m_StatusStrip.Dock = System.Windows.Forms.DockStyle.None;
            this.m_StatusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_StatusMessage1,
            this.m_ProgressBar,
            this.m_StatusMessage2});
            this.m_StatusStrip.Location = new System.Drawing.Point(0, 0);
            this.m_StatusStrip.Name = "m_StatusStrip";
            this.m_StatusStrip.Size = new System.Drawing.Size(946, 22);
            this.m_StatusStrip.SizingGrip = false;
            this.m_StatusStrip.TabIndex = 0;
            // 
            // m_StatusMessage1
            // 
            this.m_StatusMessage1.Name = "m_StatusMessage1";
            this.m_StatusMessage1.Size = new System.Drawing.Size(56, 17);
            this.m_StatusMessage1.Text = "OurWord";
            // 
            // m_ProgressBar
            // 
            this.m_ProgressBar.Name = "m_ProgressBar";
            this.m_ProgressBar.Size = new System.Drawing.Size(100, 16);
            this.m_ProgressBar.Visible = false;
            // 
            // m_StatusMessage2
            // 
            this.m_StatusMessage2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_StatusMessage2.ForeColor = System.Drawing.Color.Red;
            this.m_StatusMessage2.Name = "m_StatusMessage2";
            this.m_StatusMessage2.Size = new System.Drawing.Size(0, 17);
            // 
            // m_panelContents
            // 
            this.m_panelContents.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_panelContents.Location = new System.Drawing.Point(0, 0);
            this.m_panelContents.Name = "m_panelContents";
            this.m_panelContents.Size = new System.Drawing.Size(946, 441);
            this.m_panelContents.TabIndex = 0;
            // 
            // m_ViewBar
            // 
            this.m_ViewBar.BackColor = System.Drawing.Color.Gray;
            this.m_ViewBar.Dock = System.Windows.Forms.DockStyle.None;
            this.m_ViewBar.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.m_ViewBar.Location = new System.Drawing.Point(0, 0);
            this.m_ViewBar.Name = "m_ViewBar";
            this.m_ViewBar.Size = new System.Drawing.Size(946, 25);
            this.m_ViewBar.Stretch = true;
            this.m_ViewBar.TabIndex = 1;
            this.m_ViewBar.Text = "viewBar1";
            // 
            // OurWordMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(946, 526);
            this.Controls.Add(this.m_toolStripContainer);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "OurWordMain";
            this.Text = "Our Word!";
            this.Load += new System.EventHandler(this.cmdLoad);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.cmdClosing);
            this.Resize += new System.EventHandler(this.cmdResize);
            this.m_ToolStrip.ResumeLayout(false);
            this.m_ToolStrip.PerformLayout();
            this.m_toolStripContainer.BottomToolStripPanel.ResumeLayout(false);
            this.m_toolStripContainer.BottomToolStripPanel.PerformLayout();
            this.m_toolStripContainer.ContentPanel.ResumeLayout(false);
            this.m_toolStripContainer.TopToolStripPanel.ResumeLayout(false);
            this.m_toolStripContainer.TopToolStripPanel.PerformLayout();
            this.m_toolStripContainer.ResumeLayout(false);
            this.m_toolStripContainer.PerformLayout();
            this.m_StatusStrip.ResumeLayout(false);
            this.m_StatusStrip.PerformLayout();
            this.ResumeLayout(false);

		}
		#endregion
		#region Attr{g}: static OurWordMain App
		public static OurWordMain App
		{
			get
			{
				return s_App;
			}
            set  // Used by NUnit tests; otherwise, stay away!
            {
                s_App = value;
            }
		}
		private static OurWordMain s_App = null;
		#endregion
        #region SMethod: bool _GrabTokenForThisInstance() - only run OurWord one-at-a-time
        private static Mutex s_EnsureOneInstanceOnlyMutex;

        static private bool _GrabTokenForThisInstance()
        {
            bool bMutexCreated;

            string sMutexID = "OneInstanceOnlyAllowedForOurWord";

            s_EnsureOneInstanceOnlyMutex = new Mutex(true, sMutexID, out bMutexCreated);

            if (!bMutexCreated)
            {
                return false;
            }

            return true;
        }
        #endregion
        #region Method: static void Main() - main entry point for the application
        [STAThread]
		static void Main(string[] vArgs)
		{
            // Initialize the registry (Needed by, e.g., Splash Screen and the
            // single-instance error message.)
            JW_Registry.RootKey = "SOFTWARE\\The Seed Company\\Our Word!";

            // We only want a single instance of OurWord to be running at a time, 
			// Because of the confusion that could reign if we had multiple 
            // instances modifying the Translation and Project settings file, 
            // we elect instead to just have a single instance.
            if (!_GrabTokenForThisInstance())
                return;

            // Initialize the Localizations Database. We need this prior to the
            // splash screen being activated.
            LocDB.Initialize(Loc.FolderOfLocFiles);

            // Set the resource location (so the splash picture will be visible)
            JWU.ResourceLocation = "OurWord.Res.";

            // Display a splash screen while we're loading. We want to retrieve everything
            // needed from the localization database, so that the SplashScreen, which runs
            // on its own thread, isn't having to go cross-thread to get them.
			foreach (var s in vArgs)
			{
				if (s == "-nosplash")
					SplashScreen.IsEnabled = false;
			}

            SplashScreen.Additional = G.GetLoc_Splash("sOptionalAdditional", "-");
            SplashScreen.StatusMessage = G.GetLoc_Splash("sLoadingOurWord", "Loading Our Word...");
            SplashScreen.Version = G.GetLoc_DialogCommon("m_lblVersion", "Version {0}", 
                new string[] { G.Version });
            SplashScreen.ProgramName = G.GetLoc_DialogCommon("m_lblProgramName", "Our Word!", null);
            SplashScreen.StatusBase = G.GetLoc_Splash("sLoading", "Loading {0}...");
            SplashScreen.Start();

            // First time we run, we create a shortcut. Since the first time is a result of being
            // called from the Setup program, the user should have a shortcut immediately after
            // installing.
            const string sShortcutCreated = "ShortcutCreated";
            if (JW_Registry.GetValue(sShortcutCreated, false) == false)
                (new Shortcut("OurWord")).CreateIfDoesntExist();
            JW_Registry.SetValue(sShortcutCreated, true);

            // Now start loading & run the program
            OurWordMain.s_App = new OurWordMain();
            Application.EnableVisualStyles();
            Application.Run(s_App);

            // All done, release the Mutex
            if (s_EnsureOneInstanceOnlyMutex != null)
               s_EnsureOneInstanceOnlyMutex.ReleaseMutex();
        }
		#endregion

		// Misc Methods ---------------------------------------------------------------------
		#region Method: void OnLeaveSection()
		private void OnLeaveSection()
		{
            // Update the filter (if a filter is active) in case this section no
            // longer matches it.
            UpdateFilterForCurrentSection();
		}
		#endregion
		#region Method: void OnEnterSection()
		private void OnEnterSection()
		{
            // Load the data into the individual windows on the screen
            ResetWindowContents();

			// Set up the UI (menu, toolbars) to reflect the current section and
			// status (e.g., the Navigation toolbar buttons dropdown contents depends
			// upon which section is currently being displayed.)
            SetupMenusAndToolbarsVisibility();

            // Clear the Undo stack since we now have new data we're working with
            G.URStack.Clear();
        }
		#endregion
        #region Method: void OnLeaveProject()
        private void OnLeaveProject()
		{
			// Do everything we would normally do on leaving a section (e.g., getting
			// the data from the cache.)
			OnLeaveSection();

			// Prompt so that we save to disk
            DB.Project.Save(G.CreateProgressIndicator());

            // Dispose of the Dictionary object if necessary
            DB.Project.Dispose();
		}
		#endregion
		#region Method: void OnEnterProject()
	    internal void OnEnterProject()
		{
            // If we aren't already navigated to a book, then attempt to find one.
			DB.Project.Nav.GoToReasonableBook(G.CreateProgressIndicator());

            // Window contents, etc.
			OnEnterSection();
        }
		#endregion

        // Event Handlers --------------------------------------------------------------------
        #region Event: cmdLoad
        private void cmdLoad(object sender, System.EventArgs e)
		{
            // Init the Help system
            HelpSystem.Initialize(this, "OurWordHelp.chm");

            // Retrieve the most recent project (and go to the book-section), 
            // or load the Sample Project if the Most Recent is not available
            if (DProject.LoadMostRecentProject(G.CreateProgressIndicator()))
            {
                SetCurrentLayout(WLayout.GetLayoutFromRegistry(WndDrafting.c_sName));
            }
            else
            {
                DProject.LoadSampleProject();
                SetCurrentLayout(WndDrafting.c_sName);
            }

            // Set up the views, make the initial selection, etc.
            OnEnterProject();

			// Remember the previous placement of the window on the screen (Do this late
			// in the load sequence so that we avoid a multiple screen redraw.) 
            m_WindowState.StartMaximized = Users.Current.MaximizeWindowOnStartup;
			m_WindowState.RestoreWindowState();

			// Initialize AutoSave Timer
			InitializeAutoSave();

            // Loading all done: Close the splash screen
			if (SplashScreen.IsEnabled)
			{
				if (SplashScreen.Wnd.InvokeRequired)
				{
					var d = new SplashScreen.StopSplashScreen_Callback(SplashScreen.Wnd.Stop);
					this.Invoke(d, new object[] { this });
				}
				else
				{
					SplashScreen.Wnd.Stop(this);
				}
			}

            // Leave everything in a state where the main window has focus, so that the
            // Text Selection will be appropriately flashing its readiness
            CurrentLayout.Focus();
        }
		#endregion

        #region Event: cmdClosing - save window state, data, etc.
        private void cmdClosing(object sender, FormClosingEventArgs e)
        {
			// Save data
			OnLeaveProject();

			// Save the current project's registry info so we can return to the same
            // place the next time OW is launched.
            DB.Project.SaveMostRecentRegistryInfo();
		
			// Save the window position
			m_WindowState.SaveWindowState();

			DB.TeamSettings.WriteToFile(G.CreateProgressIndicator());
        }
        #endregion
        #region Event: OnResize - prevent the size from becomming too small
        protected override void OnResize(EventArgs e)
            // The form is being resized, but internals are not yet set. So
            // we can't set our own internal sizes here; rather than must
            // be done in the cmdResize method, which is called after the
            // sizing is finished.
        {
            // We want to preserve a minimum size. We'll set this based on the OLPC spec,
            // which as of 6 Apr 07 at http://wiki.laptop.org/go/Hardware_specification 
            // is 800 x 600 in its smallest (color) mode. We will permit a size just 
            // smaller than this OLPC minimum.
            if (WindowState != FormWindowState.Minimized)
            {
                Width = Math.Max(Width, 770);
                Height = Math.Max(Height, 570);
            }

            base.OnResize(e);
        }
        #endregion
        #region Event: cmdResize - Adjust our controls to the window's new size
        private void cmdResize(object sender, EventArgs e)
            // The form has been resized and we now need to adjust our internal windows
            // to match the correct sizes
        {
            // Tests to see if we can proceed with layout and sizing
            if (null != G.App && G.App.WindowState == FormWindowState.Minimized)
                return;
            if (null == CurrentLayout)
                return;

            // the MainWindow needs to have a size for its double buffering
            CurrentLayout.SetSize(m_panelContents.Width, m_panelContents.Height);

            // Re-do the layout and redraw
            CurrentLayout.DoLayout();
            CurrentLayout.Invalidate();
        }
        #endregion

        // Commands --------------------------------------------------------------------------
        #region COMMANDS
        // Stand-Alone Toolbar Buttons
		#region Cmd: cmdPrint
        private void cmdPrint(Object sender, EventArgs e)
		{
     //       var test = new TestDlg();
     //       test.ShowDialog();
            /////////////////////////////////////////////////////////////

            Dim();;
			OnLeaveSection();

            var p = new Printer(DB.TargetSection);
			p.Do();

            UnDim();
        }
		#endregion
        #region Cmd: cmdExit
        private void cmdExit(Object sender, EventArgs e)
		{
			Application.Exit();		
		}
		#endregion

        // Project Dropdown
		#region Cmd: cmdNewProject
        private void cmdNewProject(Object sender, EventArgs e)
        {
            // Don't allow if the menu item is hidden (Microsoft allows a Shortcut key to work,
            // even though the menu command is hidden!)
            if (!m_btnProject.Visible)
                return;
            Dim();

            // Walk through the wizard; we do nothing unless the User makes it through
            // (as signaled by DialogResult.OK).
            var wiz = new Dialogs.WizNewProject.WizNewProject();
            if (DialogResult.OK != wiz.ShowDialog())
                goto done;

            // Make sure the current project is saved and up-to-date, before we create
            // the new one. 
            OnLeaveProject();

            // Create and initialize the new project according to the settings
            var project = new DProject(wiz.ProjectName)
            {
                TeamSettings = new DTeamSettings(wiz.ChosenCluster.Name)
            };

            // Team Settings: start with the factory default; load over it if a file already exists,
            // otherwise create the new cluster
            DTeamSettings.EnsureInitialized();
            project.TeamSettings.InitialCreation(G.CreateProgressIndicator());
            StyleSheet.Initialize(null);

            // Create the front translation. If the settings file exists, load it; otherwise
            // create its folder, settings file, etc.
            project.FrontTranslation = new DTranslation(wiz.FrontName);
            project.FrontTranslation.Initialize();

            // Target Translation
            project.TargetTranslation = new DTranslation(wiz.ProjectName);
            project.TargetTranslation.Initialize();

            // Set OW to this project
            DB.Project = project;

            // Save everything
            DB.Project.WriteToFile(G.CreateProgressIndicator());
            DB.TargetTranslation.WriteToFile(G.CreateProgressIndicator());

            // Update the UI, views, etc
            DB.Project.Nav.GoToFirstAvailableBook(G.CreateProgressIndicator());
            OnEnterProject();

            // Edit properties?
            if (wiz.LaunchPropertiesDialogWhenDone)
                cmdConfigure(null, null);

        done:
            UnDim();
        }
		#endregion
        #region Cmd: cmdDownloadRepository

	    private string m_sClusterName;
	    private string m_sUserName;
	    private string m_sPassword;

        private void cmdDownloadRepository(Object sender, EventArgs e)
        {
            Dim();

            // Make sure the current project is saved and up-to-date, before we create
            // the new one. 
            OnLeaveProject();

            // To make life easier in case things fail, we'll remember what the user
            // entered in case he has to try again.
            var wiz = new WizInitializeFromRepository() 
            {
                InitialUserName = m_sUserName,
                InitialPassword = m_sPassword,
                InitialClusterName = m_sClusterName
            };

            // Get the user's settings for the to-be-downloaded cluster
            var wizResult = wiz.ShowDialog(this);
            m_sClusterName = wiz.ClusterName;
            m_sUserName = wiz.UserName;
            m_sPassword = wiz.Password;
            if (DialogResult.OK != wizResult)
                goto done;

            // The ClusterInfo tells us where the new cluster will be stored
            var sParentFolder = (wiz.IsInMyDocuments) ?
                JWU.GetMyDocumentsFolder(null) :
                JWU.GetLocalApplicationDataFolder(ClusterInfo.c_sLanguageDataFolder);
            var ci = new ClusterInfo(wiz.ClusterName, sParentFolder);

            // Attempt the clone
            var internetRepository = new InternetRepository(m_sClusterName,
                m_sUserName, m_sPassword)
            {
                Server = wiz.Url,
            };
            var localRepository = new LocalRepository(ci.ClusterFolder);
            var method = new Synchronize(localRepository, internetRepository, 
                Users.Current.UserName);
            var bSuccessful = method.CloneFromOther();

            // If an error, then abort
            if (!bSuccessful)
            {
                JWU.SafeFolderDelete(ci.ClusterFolder);
                goto done;
            }

            // Open the first project we find
            var vsProjects = ci.GetClusterLanguageList(true);
            if (null != vsProjects && vsProjects.Count > 0)
            {
                var sPath = ci.GetProjectPath(vsProjects[0]);
                DB.Project = new DProject();
                DB.Project.LoadFromFile(ref sPath, G.CreateProgressIndicator());
                DB.Project.Nav.GoToFirstAvailableBook(G.CreateProgressIndicator());
                Users.Current = Users.Observer;
                OnEnterProject();
            }

            // Display a dialog declaring Success
            LocDB.Message("msgRepoCreated",
                "The Cluster has been successfully downloaded to your computer.",
                null, LocDB.MessageTypes.Info);

            done:
                UnDim();
        }
        #endregion

        #region Cmd: cmdOpenProject
        private void cmdOpenProject(Object sender, EventArgs e)
        {
            // Get the menu item
            var m = sender as ToolStripMenuItem;
            if (null == m)
                return;

            // Get the path from the tag
            var sPath = (string)m.Tag;
            if (string.IsNullOrEmpty(sPath))
                return;

            // Open the requested project
            OnLeaveProject();
            DB.Project = new DProject();
            DB.Project.LoadFromFile(ref sPath, G.CreateProgressIndicator());
            DB.Project.Nav.GoToFirstAvailableBook(G.CreateProgressIndicator());
            OnEnterProject();
        }
        #endregion
        #region Method: BuildClusterSubMenu
        void BuildClusterSubMenu(
            ToolStripDropDownItem miParent, 
            ClusterInfo ci, 
            EventHandler onClick)
        {
            // Get the list of files in this cluster, and sort them
            var vsRaw = ci.GetClusterLanguageList(true);

            // Remove those where access is denied
            var vs = new List<string>();
            if (!ClusterList.UserCanAccessAllProjects)
            {
                foreach (var s in vsRaw)
                {
                    var bUserCanAccess = ClusterList.GetUserCanAccessProject(ci.Name, s);
                    if (bUserCanAccess)
                        vs.Add(s);
                }
            }
            else
            {
                vs = vsRaw;
            }

            // Sorted list for easy scanning
            vs.Sort();

            // No items in the list?
            if (vs.Count == 0)
            {
                miParent.DropDownItems.Add("(none defined)");
                return;
            }

            // Add them to the menu
            foreach (var s in vs)
            {
                var mi = new ToolStripMenuItem(s, null, onClick) 
                {
                    Tag = ci.GetProjectPath(s)
                };
                miParent.DropDownItems.Add(mi);
            }
        }
        #endregion

        #region Cmd: cmdSetupOpening
        private void cmdSetupOpening(object sender, EventArgs e)
        {
            // Hourglass, while we retrieve clusters from disk
            Cursor.Current = Cursors.WaitCursor;

            // Clear any subitems from the Open menu
            m_menuOpenProject.DropDownItems.Clear();

            // Get the list of clusters that we have
            ClusterList.ScanForClusters();

            // If there are no clusters, then there's nothing to open
            if (ClusterList.Clusters.Count == 0)
            {
                m_menuOpenProject.DropDownItems.Add("(none defined)");
            }

            // If there's only one cluster, then just add to the Open menu
            else if (ClusterList.Clusters.Count == 1)
            {
                BuildClusterSubMenu(m_menuOpenProject,
                    ClusterList.Clusters[0],
                    cmdOpenProject);
            }

            // Otherwise, we go to submenus
            else
            {
                foreach (ClusterInfo ci in ClusterList.Clusters)
                {
                    var miCluster = new ToolStripMenuItem(ci.Name);
                    BuildClusterSubMenu(miCluster, ci, cmdOpenProject);
                    m_menuOpenProject.DropDownItems.Add(miCluster);
                }
            }

            // Restore cursors
            Cursor.Current = Cursors.Default;
        }
        #endregion

        #region Cmd: cmdSaveProject
        private void cmdSaveProject(Object sender, EventArgs e)
		{
			OnLeaveSection();
			DB.Project.Save(G.CreateProgressIndicator());
		}
		#endregion
        #region Cmd: cmdExportProject
        private void cmdExportProject(object sender, EventArgs e)
        {
            // If we don't have an active project, with at least one book in
            // the target translation, then abort
            if (!DB.IsValidProject)
                return;
            if (DB.TargetTranslation.BookList.Count == 0)
                return;
            Dim();

            // Get the user's desires (or cancel)
            var dlgDesires = new DialogExport(DB.TargetTranslation);
            if (DialogResult.OK != dlgDesires.ShowDialog(this))
                goto done;

            // Create and display the progress dialog
            DlgExportProgress.Start();
            DlgExportProgress.SetCurrentBook("Setting up...");

            // Loop through the books of the requested translation
            foreach (var book in DB.TargetTranslation.BookList)
            {
                // Does the user wish to cancel?
                if (DlgExportProgress.UserSaysCancel)
                    break;

                // Upload the status dialog with the correct book
                DlgExportProgress.SetCurrentBook(DB.TargetTranslation.DisplayName + " - " +
                    book.DisplayName);

                // Load the book if not already in memory
                var bIsLoaded = book.Loaded;
                book.LoadBook(G.CreateProgressIndicator());
                if (!book.Loaded)
                    continue;

                // BEGIN HUICHOL FIX *******************************************************
            //    book.OneOffForHuichol_StripOutOldTranslatorNotes();
            //    book.DeclareDirty();
            //    book.WriteBook(G.CreateProgressIndicator());
                // END HUICHOL FIX *********************************************************

                // Compute the file name
                var sExportPath = JWU.GetMyDocumentsFolder(dlgDesires.ExportSubFolderName);
                sExportPath += book.BaseName;

                // Export it to paratext if requested
                if (dlgDesires.ExportToParatext)
                {
                    book.ExportToParatext(sExportPath + ".ptx", G.CreateProgressIndicator());
                }

                // Export it to GoBibleCreator if requested
                if (dlgDesires.ExportToGoBibleCreator)
                {
                    book.ExportToGoBible(sExportPath + ".GoBible.Ptx", G.CreateProgressIndicator());
                }

                // Toolbox
                if (dlgDesires.ExportToToolbox)
                {
                    book.ExportToToolbox(sExportPath + ".db", G.CreateProgressIndicator());
                }

                // Word 2007
                if (dlgDesires.ExportToWord)
                {
                    var whatToExport = (dlgDesires.ExportBackTranslation) ?
                            WordExport.Target.BackTranslation :
                            WordExport.Target.Vernacular;

                    if (whatToExport == WordExport.Target.BackTranslation)
                        sExportPath += ".bt";
                    sExportPath += ".docx";

                    using (var export = new WordExport(book, sExportPath, whatToExport))
                        export.Do();
                }

                // Unload the book if it was previously unloaded, so that we don't clog
                // up memory
                if (!bIsLoaded)
                    book.Unload(new NullProgress());
            }

            // Done with the progress dialog
            DlgExportProgress.Stop();

        done:
            UnDim();

        }
        #endregion

        // Edit Dropdown
        #region Cmd: cmdUndo
        private void cmdUndo(object sender, EventArgs e)
        {
            if (Users.Current.CanUndoRedo)
                URStack.Undo();
        }
        #endregion
        #region Cmd: cmdRedo
        private void cmdRedo(object sender, EventArgs e)
        {
            if (Users.Current.CanUndoRedo)
                URStack.Redo();
        }
        #endregion
        #region Cmd: cmdEditCut
        private void cmdEditCut(Object sender, EventArgs e)
		{
            OWWindow wnd = FocusedWindow;
            if (null != wnd)
                wnd.cmdCut();
		}
		#endregion
		#region Cmd: cmdEditCopy
        private void cmdEditCopy(Object sender, EventArgs e)
		{
            OWWindow wnd = FocusedWindow;
            if (null != wnd)
                wnd.cmdCopy();
		}
		#endregion
		#region Cmd: cmdEditPaste
        private void cmdEditPaste(Object sender, EventArgs e)
		{
            var wnd = FocusedWindow;
            if (null != wnd)
                wnd.cmdPaste();
		}
		#endregion
        #region Cmd: cmdChangeParagraphStyle
        private void cmdChangeParagraphStyle(Object sender, EventArgs e)
        {
            var mi = sender as ToolStripMenuItem;
            if (null == mi)
                return;

            var style = mi.Tag as ParagraphStyle;
            if (null == style)
                return;

            var wnd = FocusedWindow;
            if (null != wnd)
                wnd.cmdChangeParagraphTo(style);
        }
        #endregion
		#region Can: canItalic
		public bool canItalic
		{
			get
			{
                OWWindow wnd = FocusedWindow;
                if (null == wnd)
                    return false;

                return wnd.canItalic;
			}
		}
		#endregion
		#region Cmd: cmdItalic
        private void cmdItalic(Object sender, EventArgs e)
		{
            OWWindow wnd = FocusedWindow;
            if (null != wnd)
                wnd.cmdToggleItalics();
		}
		#endregion
        #region Cmd: cmdEditDropdownOpening - place a checkmark beside the current style
        private void cmdEditDropdownOpening(object sender, EventArgs e)
        {
            // Submenu for changing paragraph styles
            SetupChangeParagraphToDropdown();

            // Enable the Insert/Delete footnote commands
            m_menuDeleteFootnote.Enabled = (null == FocusedWindow) ?
                false : FocusedWindow.canDeleteFootnote;
            m_menuInsertFootnote.Enabled = (null == FocusedWindow) ?
                false : FocusedWindow.canInsertFootnote;
        }
		#endregion

        #region Cmd: cmdInsertFootnote
        private void cmdInsertFootnote(object sender, EventArgs e)
        {
            OWWindow wnd = FocusedWindow;
            if (null != wnd)
                wnd.cmdInsertFootnote();
        }
        #endregion
        #region Cmd: cmdDeleteFootnote
        private void cmdDeleteFootnote(object sender, EventArgs e)
        {
            OWWindow wnd = FocusedWindow;
            if (null != wnd)
                wnd.cmdDeleteFootnote();
        }
        #endregion

        #region Cmd: cmdCopyBTFromFront
        private void cmdCopyBTFromFront(object sender, EventArgs e)
        {
            // We only do this in select views
            if (!WLayout.CurrentLayoutIs(new string[] {
                WndBackTranslation.c_sName, 
                WndConsultantPreparation.c_sName }))
            {
                return;
            }

            OnLeaveSection();

            (new CopyBtFromFrontMethod()).Run();

            OnEnterSection();
        }
        #endregion

        // Navigation
		#region Cmd: cmdGoToFirstSection
        private void cmdGoToFirstSection(Object sender, EventArgs e)
		{
            // Ignore if we're at the beginning already
            if (DB.Project.Nav.IsAtFirstSection)
                return;

            // Go to the first section
            OnLeaveSection();
			DB.Project.Nav.GoToFirstSection();
			OnEnterSection();
		}
		#endregion
		#region Cmd: cmdGoToPreviousSection
        private void cmdGoToPreviousSection(Object sender, EventArgs e)
		{
            ToolStripSplitButton btn = sender as ToolStripSplitButton;
            if (null == btn || !btn.DropDownButtonPressed)
            {
                // Ignore if we're at the beginning already
                if (DB.Project.Nav.IsAtFirstSection)
                    return;

                // Go to the previous section
                OnLeaveSection();
                DB.Project.Nav.GoToPreviousSection();
                OnEnterSection();
            }
		}
		#endregion
		#region Cmd: cmdGoToNextSection
        private void cmdGoToNextSection(Object sender, EventArgs e)
		{
            ToolStripSplitButton btn = sender as ToolStripSplitButton;
            if (null == btn || !btn.DropDownButtonPressed)
            {
                // Ignore if we're at the end already
                if (DB.Project.Nav.IsAtLastSection)
                    return;

                // Go to the next section
                OnLeaveSection();
                DB.Project.Nav.GoToNextSection();
                OnEnterSection();
            }
		}
		#endregion
		#region Cmd: cmdGoToLastSection
        private void cmdGoToLastSection(Object sender, EventArgs e)
		{
            // Ignore if we're at the end already
            if (DB.Project.Nav.IsAtLastSection)
                return;

            // Go to the final section
            OnLeaveSection();
			DB.Project.Nav.GoToLastSection();
			OnEnterSection();
		}
		#endregion
		#region Cmd: cmdGoToSection
        private void cmdGoToSection(Object sender, EventArgs e)
		{
            ToolStripMenuItem item = (sender as ToolStripMenuItem);
            Debug.Assert(null != item);

            string sMenuText = item.Text;

			OnLeaveSection();

			// The first part of the menu name is the start reference of the desired section
			DBook book = DB.Project.SFront.Book;
			int i = 0;
			foreach(DSection section in book.Sections)
			{
				string sReference = section.ReferenceSpan.Start.FullName + " ";

                if (sMenuText.StartsWith(sReference))
				{
					DB.Project.Nav.GoToSection(i);
					OnEnterSection();
					break;
				}
				i++;
			}
		}
		#endregion
        #region Cmd: cmdGoToChapter
        private void cmdGoToChapter(object sender, EventArgs e)
        {
            // Make sure we have a valid project
            if (!DB.IsValidProject)
                return;

            // Retrieve the Chapter button
            ToolStripDropDownButton btn = sender as ToolStripDropDownButton;
            if (null == btn)
                return;

            // Determine the location for the popup
            Point pt = new Point(btn.Bounds.X, btn.Bounds.Y + btn.Size.Height);
            pt = PointToScreen(pt);

            // What is the current chapter?
            int nCurrent = DB.TargetSection.ReferenceSpan.Start.Chapter;

            // Create and display the popup
            ChapterMenu cm = new ChapterMenu(DB.TargetBook, pt, nCurrent);
            DialogResult result = cm.ShowDialog(this);
            if (DialogResult.OK != result)
                return;

            // Nothing to do if the same chapter is requested
            if (cm.Chapter == nCurrent)
                return;

            // Retrieve the number of the section we're interested in
            int iSection = DB.TargetBook.Sections.FindObj(cm.Section);
            if (-1 == iSection)
                return;

            // Navigate to that section
            OnLeaveSection();
            DB.Project.Nav.GoToSection(iSection);
            OnEnterSection();
        }
        #endregion
		#region Cmd: cmdGotoBook
        private void cmdGotoBook(Object sender, EventArgs e)
        {
            // Retrieve the target book from the menu item's text
            ToolStripMenuItem mi = (sender as ToolStripMenuItem);
            Debug.Assert(null != mi);
            string sBookAbbrev = (string)mi.Tag;
            Debug.Assert(!string.IsNullOrEmpty(sBookAbbrev));

            // Go to that book
            OnLeaveSection();
            DB.Project.Save(G.CreateProgressIndicator());
            DB.Project.Nav.GoToBook(sBookAbbrev, G.CreateProgressIndicator());
            OnEnterSection();
        }
		#endregion

        // Windows
        #region Cmd: cmdJobDrafting
        private void cmdJobDrafting(Object sender, EventArgs e)
		{
            SetCurrentLayout(WndDrafting.c_sName);
            G.URStack.Clear();
		}
		#endregion
		#region Cmd: cmdJobBackTranslation
        private void cmdJobBackTranslation(Object sender, EventArgs e)
		{
            SetCurrentLayout(WndBackTranslation.c_sName);
            G.URStack.Clear();
        }
		#endregion
        #region Cmd: cmdJobNaturalness
        private void cmdJobNaturalness(Object sender, EventArgs e)
        {
            SetCurrentLayout(WndNaturalness.c_sName);
            G.URStack.Clear();
        }
        #endregion
        #region Cmd: cmdJobConsultantPreparation
        private void cmdJobConsultantPreparation(object sender, EventArgs e)
        {
            SetCurrentLayout(WndConsultantPreparation.c_sName);
            G.URStack.Clear();
        }
        #endregion

        #region Cmd: cmdWindowDropDownOpening
        private void cmdWindowDropDownOpening(object sender, EventArgs e)
        {
            // Populate the Zoom Factor subwindow, if it hasn't been populated already
            if (0 == m_menuZoom.DropDownItems.Count)
            {
                foreach (var n in User.PossibleZoomPercents)
                {
                    var mi = new ToolStripMenuItem(n.ToString() + "%", null,
                        cmdChangeZoomPercent, "zzom_" + n.ToString());
                    mi.Tag = n;
                    m_menuZoom.DropDownItems.Add(mi);
                }
            }

            // Check the one that is current
            foreach (ToolStripMenuItem mi in m_menuZoom.DropDownItems)
            {
                mi.Checked = ((int)mi.Tag == Users.Current.ZoomPercent) ? true : false;
            }
        }
        #endregion
        #region Cmd: cmdChangeZoomPercent
        private void cmdChangeZoomPercent(Object sender, EventArgs e)
        {
            var mi = sender as ToolStripMenuItem;
            if (null == mi)
                return;

            if (Users.Current.ZoomPercent != (int)mi.Tag)
            {

                Users.Current.ZoomPercent = (int)mi.Tag;
                OnEnterSection();
            }
        }
        #endregion

        // Tools
		#region Can: canIncrementBookStatus
        private bool canIncrementBookStatus
		{
			get
			{
				if (null == DB.Project)
					return false;
				if (null == DB.Project.FrontTranslation)
					return false;
				if (null == DB.Project.TargetTranslation)
					return false;
				if (null == DB.Project.Nav.STarget)
					return false;
				return true;
			}
		}
		#endregion
		#region Cmd: cmdIncrementBookStatus
        private void cmdIncrementBookStatus(Object sender, EventArgs e)
		{
			// Check that we can increment (we need to have a valid project)
			if (!canIncrementBookStatus)
				return;

			// Perform the dialog to get the increment desired
			var book = DB.Project.STarget.Book;
			var dlg = new IncrementBookStatus(book);
            Dim();
            if (dlg.ShowDialog() != DialogResult.OK)
            {
                UnDim(); 
                return; 
            }

			// If we incremented, then we need to save the file, so that it will be
			// stored on the disk under the new filename (and thus synchronized with
			// what is in the Project. (See Bug0084)
            book.DeclareDirty();
			cmdSaveProject(null, null);
            UnDim();

            // Make sure the UI updates to show the correct file name
            ResetWindowContents();

            G.URStack.Clear();
		}
		#endregion
		#region Cmd: cmdRestoreBackup
        private void cmdRestoreBackup(Object sender, EventArgs e)
		{
			if (!canIncrementBookStatus)
				return;

			var BTarget = DB.Project.STarget.Book;

            Dim();
			var dlg = new DialogRestoreFromBackup(BTarget);
			if (DialogResult.OK == dlg.ShowDialog())
			{
				OnLeaveProject();

				DBook.RestoreFromBackup(BTarget, dlg.BackupPathName, G.CreateProgressIndicator());

				DB.Project.Nav.GoToFirstSection();
				OnEnterSection();
			}
            UnDim();
		}
		#endregion
		#region Cmd: cmdFilter

		// We make the dialog static, so that its settings persist throughout the OW
		// session. (We don't want to save them beyond, thus we don't write them
		// to the registry or anywhere.)
		static DialogFilter m_dlgFilter = null;

		#region Method: bool FilterTest(DSection section) - perform the test on a given section
		bool FilterTest(DSection section)
		{
			if (null == m_dlgFilter)
				return false;

			return section.FilterTest( 
				m_dlgFilter.OneOnly, 
				m_dlgFilter.Filter_VernacularText, m_dlgFilter.Filter_VernacularSearchString,
				m_dlgFilter.Filter_FrontText,      m_dlgFilter.Filter_FrontSearchString,
				m_dlgFilter.Filter_VernacularBT,   m_dlgFilter.Filter_VernacularBTSearchString,
				m_dlgFilter.Filter_UntranslatedText,
				m_dlgFilter.Filter_MismatchedQuotes,
				m_dlgFilter.Filter_PictureWithCaption, 
				m_dlgFilter.Filter_ParagraphHasQuote,
				m_dlgFilter.Filter_PunctuationProblem,
				m_dlgFilter.Filter_PictureCannotBeLocatedOnDisk);
		}
		#endregion
		#region Method: void UpdateFilterForCurrentSection()
		void UpdateFilterForCurrentSection()
		{
			// Don't bother if there is no target section, no filter, etc.
			if (null == DB.TargetSection)
				return;
			if (!DSection.FilterIsActive)
				return;
			if (!DB.TargetSection.MatchesFilter)
				return;

			// Run the test on the current section to update it.
			DB.Project.STarget.MatchesFilter = FilterTest(DB.Project.STarget);

			// If the filter no longer matches this section...
			if (!DB.Project.STarget.MatchesFilter)
			{
				// Are there any other sections that still match the filter?
				bool bNoMatches = true;
				foreach(DSection section in DB.TargetBook.Sections)
				{
					if (section.MatchesFilter)
						bNoMatches = false;
				}

				// If not, then we must turn off filters
				if (bNoMatches)
				{
					DB.TargetBook.RemoveFilter();
					G.StatusSecondaryMessage = "";
				}
			}

			// Update the navigation bar
            SetupMenusAndToolbarsVisibility();
		}
		#endregion

        #region Cmd: cmdFilter(...)
        private void cmdFilter(Object sender, EventArgs e)
		{
			// Make sure we have a project with Front and Target book
			if (!DB.IsValidProject)
				return;
            Dim();

			// Get the user's pleasure
			if (null == m_dlgFilter)
				m_dlgFilter = new DialogFilter();
			var result = m_dlgFilter.ShowDialog(this);

			// If the user Canceled out, then we remove any existing filter
			if (result == DialogResult.Cancel || m_dlgFilter.NothingIsChecked)
			{
				DB.TargetBook.RemoveFilter();
				G.StatusSecondaryMessage = "";
                SetupMenusAndToolbarsVisibility();    // Reset the Navigation toolbar popups
			    goto done;
			}

			// Calculate the filter according to the user's requests, and position
			// at the first match.
            var progress = G.CreateProgressIndicator();
            progress.Start(
                G.GetLoc_String("strCalculatingMatchingSections", "Calculating matching sections..."), 
                DB.TargetBook.Sections.Count);
			OnLeaveSection();
			int cMatches = 0;
			foreach(DSection section in DB.TargetBook.Sections)
			{
				// Make sure the default is that there isn't a match
				section.MatchesFilter = false;

				// Conduct the test for this section
				var b = FilterTest(section);

				// If the test passed, then indicate it in the Section
				if (b)
				{
					section.MatchesFilter = true;
					cMatches++;
				}

				// Feedback to the user that progress is taking place
                progress.Step();
			}
            progress.End();

			// If at least one section passed, then turn on filters and position at a
			// matching section (if we aren't already at one.).
			if (cMatches > 0)
			{
				DSection.FilterIsActive = true;
                G.StatusSecondaryMessage = G.GetLoc_String("strViewingASubset", "Viewing a Subset.");
				if (!DB.TargetSection.MatchesFilter)
					cmdGoToFirstSection(null,null);
				else
                    SetupMenusAndToolbarsVisibility();
			}
			else
			{
				Messages.NoFilterMatches();
				G.StatusSecondaryMessage = "";
			}

        done:
            UnDim();
		}
		#endregion

		#endregion
		#region Cmd: cmdConfigure
        private void cmdConfigure(Object sender, EventArgs e)
		{
            if (!Users.HasAdministrator)
            {
                Messages.NeedAdministrator();
                return;
            }

            Dim();

            // An administrator login is required to access the Configuration
            if (!Users.Current.IsAdministrator)
            {
                var dlgAuthenticate = new DlgAdministratorLogin();
                var bAuthenticated = (DialogResult.OK == dlgAuthenticate.ShowDialog(this));
                if (!bAuthenticated)
                {
                    UnDim();
                    return;
                }
            }


            // Retrieve data and save the project to disk. We don't know if the
			// user might remove the book from the project, so we need to make sure
			// it was saved just in case.
			OnLeaveProject();

            // Let the user change the properties
            var dlg = new DialogProperties();
            dlg.ShowDialog(this);
            UnDim();

            // Re-initialize everything
			OnEnterProject();
		}
		#endregion
		#region Cmd: cmdDebugTesting
        private void cmdDebugTesting(Object sender, EventArgs e)
		{
			// Run the tests
			TestHarness th = new TestHarness();
			th.Run();

			// Re-activate the project
			OnEnterProject();
		}
		#endregion
        #region Cmd: cmdLocalizer
        private void cmdLocalizer(Object sender, EventArgs e)
        {
            Dim();

            OnLeaveProject();

            Localizer dlg = new Localizer(LocDB.DB);
            if (DialogResult.OK == dlg.ShowDialog())
                OnEnterProject();

            UnDim();
        }
        #endregion
        #region Cmd: cmdSynchronize
        private void cmdSynchronize(object sender, EventArgs e)
        {
            Dim();

            // Save everything, but don't commit, cause Synchronize will do a commit
            DB.Project.Nav.SavePositionToRegistry();
            OnLeaveProject();

            // Do the Synchronize
            var local = DB.TeamSettings.GetLocalRepository();
            var remote = DB.TeamSettings.GetInternetRepository();
            var synch = new Synchronize(local, remote, Users.Current.UserName);
            using (new Chorus.Utilities.ShortTermEnvironmentalVariable("OurWordExeVersion", G.Version))
                synch.SynchLocalToOther();

            // We have to unload, then reload everything
            var sPath = DB.Project.StoragePath;
            DB.Project = new DProject();
            DB.Project.LoadFromFile(ref sPath, G.CreateProgressIndicator());
            DB.Project.Nav.RetrievePositionFromRegistry(G.CreateProgressIndicator());
            OnEnterProject();

            UnDim();
        }
        #endregion
        #region Cmd: cmdHistory
        private void cmdHistory(object sender, EventArgs e)
        {
            Dim();
            var history = new DlgHistory(DB.TargetSection);
            history.ShowDialog(this);
            UnDim();
        }
        #endregion
        #region Cmd: cmdCheckForUpdates
        private void cmdCheckForUpdates(object sender, EventArgs e)
        {
            DB.Project.Nav.SavePositionToRegistry();
            OnLeaveProject();

            // Invoke
            var checkForUpdateMethod = new InvokeCheckForUpdates
            {
                QuietMode = false
            };
            
            checkForUpdateMethod.Do(this);
        }
        #endregion

        // Help
        #region Cmd: cmdHelpTopics
        private void cmdHelpTopics(Object sender, EventArgs e)
		{
			HelpSystem.ShowDefaultTopic();
		}
		#endregion
		#region Cmd: cmdHelpAbout
        private void cmdHelpAbout(Object sender, EventArgs e)
		{
            Dim();
			DialogHelpAbout dlg = new DialogHelpAbout();
			dlg.ShowDialog(this);
            UnDim();
		}
		#endregion

        // Misc
        #region Cmd: cmdInsertNote
        private void cmdInsertNote(object sender, EventArgs e)
        {
            // Retrieve the user interface item. If it was the top-level dropdown button, and 
            // if there are dropdown items that are visible, then we don't want to execute the
            // command; rather, we want the dropdown to take place so that the user can choose
            // which type of note he wants.
            var button = sender as ToolStripDropDownButton;
            if (null != button && button.HasDropDownItems)
                return;

            // If we're here, we have a bonafide Insert command; either the top-level button
            // (without dropdowns), or a dropdown itself.
            var uiItem = sender as ToolStripItem;
            Debug.Assert(null != uiItem, "cmdInsertNote expects a ToolStripItem");

            // If the Main Window is not focused, we don't have a context for inserting.
            Debug.Assert(null != CurrentLayout);
            if (!CurrentLayout.Focused)
                return;

            // If we don't have a selection, we inform the user; as we want our notes to be
            // about something (and it gives us an initial title for our annotation.)
            var selection = CurrentLayout.Selection;
            if (null == selection || !selection.IsContentSelection)
            {
                LocDB.Message(
                    "msgNeedTextSelectionForInsertNote",
                    "Please select the text for which your note will be about.",
                    null,
                    LocDB.MessageTypes.Warning);
                return;
            }

            // Get the Class of annotation from the Tag
            var sClass = (string)uiItem.Tag;
            var properties = TranslatorNote.Properties.Find(sClass);

            // Perform the undoable action
            var action = new InsertNoteAction(CurrentLayout, properties);
            action.Do();

            // Launch the tooltip window for the new annotation
            var vParas = CurrentLayout.Contents.AllParagraphs;
            foreach (var owp in vParas)
            {
                foreach (var item in owp.SubItems)
                {
                    var en = item as ENote;
                    if (null != en && en.Note == action.Note)
                    {
                        en.LaunchToolTip();
                    }
                }
            }
        }
        #endregion
        #endregion


    }

	#region CLASS G - Globals for convenient access
	public static class G
	{
        // Stuff still in OurWordMain
        #region Attr{g}: OurWordMain App
        static public OurWordMain App
        {
            get
            {
                return OurWordMain.App;
            }
        }
        #endregion
        #region SAttr{g}:  UndoRedoStack URStack
        static public UndoRedoStack URStack
        {
            get
            {
                return App.URStack;
            }
        }
        #endregion

        // Status Bar ------------------------------------------------------------------------
        #region SMethod: IProgressIndicator CreateProgressIndicator()
        static public IProgressIndicator CreateProgressIndicator()
        {
            if (SplashScreen.IsShowing)
                return new SplashProgress();

            if (null == OurWordMain.App)
                return new NullProgress();

            return new ToolStripProgress(OurWordMain.App,
                OurWordMain.App.ProgressBar,
                OurWordMain.App.StatusLabel1);
        }
        #endregion

        #region Attr{g/s}: string StatusSecondaryMessage
        public static string StatusSecondaryMessage
		{
			get
			{
                if (null != OurWordMain.App)
                {
                    return OurWordMain.App.StatusMessage2;
                }
                return "";
			}
			set
			{
                if (null != OurWordMain.App)
                {
                    OurWordMain.App.StatusMessage2 = value;
                }
			}
		}
		#endregion

		// Browse Directory ------------------------------------------------------------------
		#region Attr{g/s}: string BrowseDirectory - last navigated-to directory (runtime only)
		public static string BrowseDirectory
		{
			get
			{
				if (s_sBrowseDirectory.Length == 0)
					s_sBrowseDirectory = DB.TeamSettings.ClusterFolder;
				return s_sBrowseDirectory;
			}
			set
			{
				s_sBrowseDirectory = value;
			}
		}
		static string s_sBrowseDirectory = "";
		#endregion
        #region SAttr{g}: string FolderOfExe
        static public string FolderOfExe
        {
            get
            {
                string sPathOfExe;
                var bUnitTesting = Assembly.GetEntryAssembly() == null;
                if (bUnitTesting)
                {
                    sPathOfExe = new Uri(Assembly.GetExecutingAssembly().CodeBase).AbsolutePath;
                    sPathOfExe = Uri.UnescapeDataString(sPathOfExe);
                }
                else
                {
                    sPathOfExe = Assembly.GetExecutingAssembly().Location;
                }
                Debug.Assert(!string.IsNullOrEmpty(sPathOfExe));

                var sFolderOfExe = Path.GetDirectoryName(sPathOfExe);
                return sFolderOfExe;
            }
        }
        #endregion

        // Misc ------------------------------------------------------------------------------
        #region SAttr{g}: bool IsLinux
        static public bool IsLinux
        {
            get
            {
                if (Environment.OSVersion.Platform == PlatformID.Unix)
                    return true;
                return false;
            }
        }
        #endregion
        #region SAttr{g}: string Version
        static public string Version
        {
            get
            {
                var v = Assembly.GetExecutingAssembly().GetName().Version;
                return JWU.BuildFriendlyVersion(v);
            }
        }
        #endregion

        // LocDB strings - access to various Groups in the LocDB -----------------------------
        #region SMethod: string GetLoc_String(sItemID, sEnglish) -        "Strings" 
        static public string GetLoc_String(string sItemId, string sEnglishDefault)
        {
            return LocDB.GetValue(
                new[] { "Strings" },
                sItemId,
                sEnglishDefault,
                null,
                null);
        }
        #endregion
        #region SMethod: string GetLoc_GeneralUI(sItemID, sEnglish) -     "Strings\GeneralUI"
        static public string GetLoc_GeneralUI(string sItemId, string sEnglishDefault)
        {
            return LocDB.GetValue(
                new[] { "Strings", "GeneralUI" },
                sItemId, 
                sEnglishDefault,
                null,
                null);
        }
        #endregion
        #region SMethod: string GetLoc_Files(sItemID, sEnglish) -         "Strings\Files"
        static public string GetLoc_Files(string sItemId, string sEnglishDefault)
        {
            return LocDB.GetValue(
                new[] { "Strings", "Files" },
                sItemId,
                sEnglishDefault,
                null,
                null);
        }
        #endregion
        #region SMethod: string GetLoc_UndoRedo(sItemID, sEnglish) -      "Strings/UndoRedo"
        static public string GetLoc_UndoRedo(string sItemId, string sEnglishDefault)
        {
            return LocDB.GetValue(
                new[] { "Strings", "UndoRedo" },
                sItemId,
                sEnglishDefault,
                null,
                null);
        }
        #endregion
        #region SMethod: string GetLoc_DialogCommon(sItemID, sEnglish, vsIns) -  "Strings\DialogCommon"
        static public string GetLoc_DialogCommon(string sItemId, string sEnglish, string[] vsInsert)
        {
            return LocDB.GetValue(
                new[] { "Strings", "DialogCommon" },
                sItemId,
                sEnglish,
                null,
                vsInsert);
        }
        #endregion
        #region SMethod: string GetLoc_Splash(sItemID, sEnglish) -     "Strings\GeneralUI"
        static public string GetLoc_Splash(string sItemId, string sEnglishDefault)
        {
            return LocDB.GetValue(
                new[] { "SplashScreen" },
                sItemId,
                sEnglishDefault,
                null,
                null);
        }
        #endregion
    }
	#endregion

    public class SplashProgress : IProgressIndicator
    {
        #region Method: void Start(string sMessage, int nCount)
        public void Start(string sMessage, int nCount)
        {
            if (SplashScreen.IsShowing)
            {
                if (SplashScreen.Wnd.InvokeRequired)
                {
                    var d = new SplashScreen.SetStatus_Callback(SplashScreen.Wnd.SetStatus);
                    G.App.Invoke(d, new object[] { sMessage, nCount });
                }
                else
                {
                    SplashScreen.Wnd.SetStatus(sMessage, nCount);
                }
            }
        }
        #endregion
        #region Method: void Step()
        public void Step()
        {
            if (SplashScreen.IsShowing)
            {
                if (SplashScreen.Wnd.InvokeRequired)
                {
                    var d = new SplashScreen.IncrementProgress_Callback(SplashScreen.Wnd.IncrementProgress);
                    G.App.Invoke(d, new object[] { });
                }
                else
                {
                    SplashScreen.Wnd.IncrementProgress();
                }
            }
        }
        #endregion
        #region Method: void End()
        public void End()
        {
            if (SplashScreen.IsShowing)
            {
                if (SplashScreen.Wnd.InvokeRequired)
                {
                    var d = new SplashScreen.ResetProgress_Callback(SplashScreen.Wnd.ResetProgress);
                    G.App.Invoke(d, new object[] { });
                }
                else
                {
                    SplashScreen.Wnd.ResetProgress();
                }
            }
        }
        #endregion
    }


}






