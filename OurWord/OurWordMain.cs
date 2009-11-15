/**********************************************************************************************
 * Project: Our Word!
 * File:    OurWordMain.cs
 * Author:  John Wimbish
 * Created: 2 Dec 2003
 * Purpose: Main window and app for the application.
 * Legal:   Copyright (c) 2004-08, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
#region Using
using System;
using System.Diagnostics;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.IO;
using System.Data;
using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

using JWTools;
using OurWordData;
using OurWordData.DataModel;

using OurWord.Edit;
using OurWord.Layouts;
using OurWord.Dialogs;
using OurWord.Dialogs.History;
using OurWord.SideWnd;
using OurWord.Utilities;
#endregion

namespace OurWord
{
	public class OurWordMain : System.Windows.Forms.Form
		// Main application window and top-level message routing
	{
		// Static Objects & Globals ----------------------------------------------------------
		#region Attr{g}: bool TargetIsLocked
		static public bool TargetIsLocked
		{
			get
			{
				Debug.Assert(null != DB.Project);

				DSection section = DB.Project.STarget;

				DBook book = (null == section) ? null : section.Book;
				if (null == book)
					return false;

				return book.Locked;
			}
		}
		#endregion
		#region Attr{g}: static DBook BTarget
		static public DBook BTarget
		{
			get
			{
				if (null != DB.Project && null != DB.Project.STarget)
					return DB.Project.STarget.Book;
				return null;
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
        UndoRedoStack m_URStack;
        #endregion

        // Client Windows --------------------------------------------------------------------
        #region CLIENT WINDOWS
        #region VAttr{g}: bool HasSideWindows
        public bool HasSideWindows
        {
            get
            {
                return SideWindows.HasSideWindows;
            }
        }
        #endregion
        #region Attr{g}: SideWindows SideWindows
        public SideWindows SideWindows
        {
            get
            {
                Debug.Assert(null != m_SideWindows);
                return m_SideWindows;
            }
        }
        SideWindows m_SideWindows = null;
        #endregion

        #region Method: void SetCurrentLayout(sLayoutName)
        public void SetCurrentLayout(string sLayoutName)
        {
            if (WLayout.SetCurrentLayout(sLayoutName))
            {
                SetupSideWindows();
                ResetWindowContents();
            }
        }
        #endregion
        #region VAttr{g}: Layout CurrentLayout
        public WLayout CurrentLayout
        {
            get
            {
                return WLayout.CurrentLayout;
            }
        }
        #endregion

        #region VAttr{g}: Control FocusedWindow - the window with current Focus, or null
        OWWindow FocusedWindow
        {
            get
            {
                if (null == CurrentLayout)
                    return null;
                if (CurrentLayout.Focused)
                    return CurrentLayout;
                return SideWindows.FocusedWindow;
            }
        }
        #endregion
        #region Method: void CycleFocusToNextWindow()
        public void CycleFocusToNextWindow()
        {
            // Do nothing if we don't have side windows
            if (!HasSideWindows)
                return;

            // If this window is focused, then send focus to the first tab
            if (CurrentLayout.Focused)
            {
                SideWindows.SelectFirstTab();
                return;
            }

            // Otherwise, tell the SideWindow to cycle to its next tab
            SideWindows.CycleTabToNextWindow();
        }
        #endregion
        #region Method: void CycleFocusToPreviousWindow()
        public void CycleFocusToPreviousWindow()
        {
            // Do nothing if we don't have side windows
            if (!HasSideWindows)
                return;

            // If this window is focused, then send focus to the first tab
            if (CurrentLayout.Focused)
            {
                SideWindows.SelectLastTab();
                return;
            }

            // Otherwise, tell the SideWindow to cycle to its previous tab
            SideWindows.CycleTabToPreviousWindow();
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
        #region Method: void SetupSideWindows() - called whenever different SideWindows are desired (startup, Tools-Options)
        void SetupSideWindows()
        {
            SideWindows.ClearPages();

            if (DB.IsValidProject)
            {
                if (DB.Project.ShowTranslationsPane && WLayout.CurrentLayoutIs(WndDrafting.c_sName))
                    SideWindows.AddPage(new TranslationsPane(), "Translations");
            }

            // Tell the system which side windows are being displayed; thereafter events will be 
            // automatically routed to these windows.
            SideWindows.RegisterWindows(CurrentLayout);

            if (HasSideWindows)
            {
                m_SplitContainer.Panel2Collapsed = false;
                SideWindows.SetChildrenSizes();
            }
            else
                m_SplitContainer.Panel2Collapsed = true;

            SizeAndLayoutContentWindows();
        }
        #endregion
		#region Method: void SetTitleBarText()
		public void SetTitleBarText()
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

            // Do we have a valid project? Can't do much if not.
            if (null == DB.Project || null == DB.FrontSection || null == DB.TargetSection)
            {
                CurrentLayout.Clear();
                CurrentLayout.Invalidate();
                TaskName = G.GetLoc_GeneralUI("NoProjectDefined", "No Project Defined");
                LanguageInfo = "";
                Passage = "";
                ShowPadlock = false;
                if (HasSideWindows)
                    SideWindows.Invalidate();
                return;
            }

            // Update the Title Window contents
            TaskName = CurrentLayout.WindowName;
            LanguageInfo = CurrentLayout.LanguageInfo;
            Passage = CurrentLayout.PassageName;
            ShowPadlock = TargetIsLocked;

            // Loading the main window should also load the data in the side windows, as this
            // is generally built as the main window is loaded (or as items there are selected)
            CurrentLayout.LoadData();

            // Place focus in the main window, so that it is ready for editing
            CurrentLayout.Focus();
        }
        #endregion
        #region Method: void SetZoomFactor()
        void SetZoomFactor()
        {
            if (null != CurrentLayout)
                CurrentLayout.ZoomFactor = G.ZoomFactor;

            if (null != SideWindows)
                SideWindows.SetZoomFactor(G.ZoomFactor);

            DB.StyleSheet.ZoomFactor = G.ZoomFactor;
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
        private ToolStripMenuItem m_menuShowTranslationsPane;
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
        private SplitContainer m_SplitContainer;
        private ToolStripContainer m_toolStripContainer;
        private ToolStrip m_ToolStrip;
        private StatusStrip m_StatusStrip;
        private ToolStripSeparator m_separator3;

        private ToolStripMenuItem m_menuExportProject;
        private ToolStripDropDownButton m_btnInsertNote;
        private ToolStripMenuItem m_itemInsertGeneralNote;
        private ToolStripMenuItem m_itemInsertExegeticalNote;
        private ToolStripMenuItem m_itemInsertHintNote;
        private ToolStripMenuItem m_itemInsertConsultantNote;

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
        private ToolStripMenuItem m_menuSetUpFeatures;
        private ToolStripSeparator m_separator4;

        private ToolStripStatusLabel m_StatusMessage1;
        private ToolStripProgressBar m_ProgressBar;
        private ToolStripStatusLabel m_StatusMessage2;

        // Taskbar
        private ToolStrip m_Taskbar;
        private ToolStripLabel m_tbTaskName;
        private ToolStripButton m_tbPadlock;
        private ToolStripLabel m_tbLanguageInfo;
        private ToolStripLabel m_tbCurrentPassage;
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
            List<string> vPossibilities = p.CanChangeParagraphStyleTo;

            // Create menu items for each of these
            foreach (string sAbbrev in vPossibilities)
            {
                JParagraphStyle pstyle = DB.StyleSheet.FindParagraphStyle(sAbbrev);
                if (null == pstyle)
                    continue;

                ToolStripMenuItem mi = new ToolStripMenuItem(
                    pstyle.DisplayName,
                    null,
                    cmdChangeParagraphStyle,
                    "m_menuChangeParagraphStyle_" + sAbbrev);
                mi.Tag = sAbbrev;

                if (sAbbrev == p.StyleAbbrev)
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

            // Side Windows controls
            SideWindows.SetControlsEnabling();

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
            bool bShowNewOpenEtc = (!DB.IsValidProject || OurWordMain.Features.F_Project);
            m_btnProject.Visible = bShowNewOpenEtc;
            m_menuExportProject.Visible = (
                DB.IsValidProject && 
                DB.TargetTranslation.BookList.Count > 0 &&
                Features.F_Export);

            // Print
            m_btnPrint.Visible = OurWordMain.Features.F_Print;

            // Go To First / Last Section
            m_btnGotoFirstSection.Visible = DB.IsValidProject && Features.F_GoTo_FirstLast;
            m_btnGotoLastSection.Visible = DB.IsValidProject && Features.F_GoTo_FirstLast;

            // Go To Chapter
            m_btnChapter.Visible = DB.IsValidProject && Features.F_GoTo_Chapter;

            // TEMP: TURN OFF UNTIL IMPLEMENTED
            //m_btnHistory.Visible = false;

            // Configure - If we have an invalid project, we turn this on regardless
            bool bShowConfigureDlg = (!DB.IsValidProject || OurWordMain.Features.F_PropertiesDialog);
            m_menuConfigure.Visible = bShowConfigureDlg;

            // Restore from Backup
            m_menuRestoreFromBackup.Visible = OurWordMain.Features.F_RestoreBackup;

            // Debug Test Suite
            bool bShowDebugItems = JW_Registry.GetValue("Debug", false);
            m_separatorDebug.Visible = bShowDebugItems;
            m_menuRunDebugTestSuite.Visible = bShowDebugItems;

            // Filters
            m_menuOnlyShowSectionsThat.Visible = (DB.IsValidProject && OurWordMain.Features.F_Filter);

            // Localizer Tool
            m_menuLocalizerTool.Visible = OurWordMain.Features.F_Localizer;

            // Translator Notes
            m_btnInsertNote.Visible = s_Features.TranslatorNotes;

            // Window Menu in its entirety
            bool bShowMainWindowSection = s_Features.F_JobBT || 
                s_Features.F_JobNaturalness ||
                s_Features.F_ConsultantPreparation;
            bool bShowTranslatorNotesMenu = (DB.IsValidProject && s_Features.TranslatorNotes);
            bool bShowTranslationsPane = (DB.IsValidProject && DB.Project.OtherTranslations.Count > 0);
            bool bShowHistoryMenu = false; // (DB.IsValidProject && s_Features.SectionHistory);
            bool bShowSideWindowsSection = 
                bShowTranslatorNotesMenu || 
                bShowTranslationsPane || 
                bShowHistoryMenu;

            m_btnWindow.Visible = (bShowMainWindowSection || bShowSideWindowsSection);

            // Main Window items: Drafting, Naturalness, BT
            m_menuDrafting.Visible = bShowMainWindowSection;
            m_menuBackTranslation.Visible = (bShowMainWindowSection && s_Features.F_JobBT);
            m_menuConsultantPreparationToolStripMenuItem.Visible = 
                (bShowMainWindowSection && s_Features.F_ConsultantPreparation);
            m_menuNaturalnessCheck.Visible = (bShowMainWindowSection && s_Features.F_JobNaturalness);
            m_separatorWindow.Visible = bShowMainWindowSection && bShowSideWindowsSection;

            // Side Window Items
            m_menuShowTranslationsPane.Visible = bShowTranslationsPane;
            m_menuShowTranslationsPane.Checked = DProject.VD_ShowTranslationsPane;

            // Edit Menu / Structured Editing
            bool bStructuralEditing = s_Features.F_StructuralEditing &&
                WLayout.CurrentLayoutIs(WndDrafting.c_sName);
            bool bEditMenuVisible = bStructuralEditing || Features.F_UndoRedo;
            m_seperatorEdit.Visible = bStructuralEditing;
            m_menuChangeParagraphTo.Visible = bStructuralEditing;
            m_menuInsertFootnote.Visible = bStructuralEditing;
            m_menuDeleteFootnote.Visible = bStructuralEditing;
            m_menuEdit.Visible = bEditMenuVisible;
            m_menuUndo.Visible = Features.F_UndoRedo;
            m_menuRedo.Visible = Features.F_UndoRedo;
            m_btnEditCopy.Visible = !bEditMenuVisible;
            m_btnEditCut.Visible = !bEditMenuVisible;
            m_btnEditPaste.Visible = !bEditMenuVisible;

            m_menuCopyBTFromFrontTranslation.Visible = (
                OurWordMain.Features.F_ConsultantPreparation && 
                WLayout.CurrentLayoutIs( new string[] {
                    WndConsultantPreparation.c_sName, WndBackTranslation.c_sName } )
                );

            // Insert dropdown
            CurrentLayout.SetupInsertNoteDropdown(m_btnInsertNote);

            // Clear dropdown subitems so we don't attempt to localize them
            m_btnGotoPreviousSection.DropDownItems.Clear();
            m_btnGotoNextSection.DropDownItems.Clear();
            m_btnGoToBook.DropDownItems.Clear();

            // Localization
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
        // Taskbar
        #region Attr{g/s}: string TaskName - e.g., "Drafting", "Back Translation"
        public string TaskName
        {
            get
            {
                return m_tbTaskName.Text;
            }
            set
            {
                m_tbTaskName.Text = value;
            }
        }
        #endregion
        #region Attr{s}: string LanguageInfo - e.g., "Kupang to AMARASI"
        public string LanguageInfo
        {
            set
            {
                if (string.IsNullOrEmpty(value))
                    m_tbLanguageInfo.Text = "";
                else
                    m_tbLanguageInfo.Text = " (" + value + ") ";
            }
        }
        #endregion
        #region Attr{s}: string Passage - e.g., "John 3:16"
        public string Passage
        {
            set
            {
                m_tbCurrentPassage.Text = value;
            }
        }
        #endregion
        #region Attr{g}: bool ShowPadlock - true if the book is locked for editing
        bool ShowPadlock
        {
            get
            {
                return m_tbPadlock.Visible;
            }
            set
            {
                m_tbPadlock.Visible = value;
            }
        }
        #endregion
        #endregion

        // Private attributes ----------------------------------------------------------------
		#region Attr{g/s}: bool StartMaximized - If T, maximize the window on startup
		public bool StartMaximized
		{
			get
			{
				return m_WindowState.StartMaximized;
			}
			set
			{
				m_WindowState.StartMaximized = value;
			}
		}
		#endregion
		private JW_WindowState  m_WindowState;   // Save/restore state on close/launch of app

		// Scaffolding -----------------------------------------------------------------------
        const string c_sLastProjectOpened = "LastProjectOpened";
		#region Constructor()
		public OurWordMain()
			// Constructor, initializes the application
		{
			// Required for Windows Form Designer support
            this.components = new System.ComponentModel.Container();
			InitializeComponent();

			// Initialize the features we will make available
			s_Features = new FeaturesMgr();

            // Suspend the layout while we create the windows. We create them here in order
            // to get the propert z-order
            // TO DO: Still necessary? Can we move this to OnLoad?
            SuspendLayout();

            // Create the SideWindows; hide them initially
            // TODO: Once we get rid of sidewindows, we  no longer need this layer
            m_SideWindows = new SideWindows();
            m_SplitContainer.Panel2.Controls.Add(SideWindows);
            SideWindows.Dock = DockStyle.Fill;

            // Create the "job" windows
            WLayout.RegisterLayout(m_SplitContainer.Panel1, new WndDrafting());
            WLayout.RegisterLayout(m_SplitContainer.Panel1, new WndNaturalness());
            WLayout.RegisterLayout(m_SplitContainer.Panel1, new WndBackTranslation());
            WLayout.RegisterLayout(m_SplitContainer.Panel1, new WndConsultantPreparation());

            // Let the window go ahead and proceed with the layout
            ResumeLayout();

			// Initialize the window state mechanism. We'll default to a full screen
			// the first time we are launched.
			m_WindowState = new JW_WindowState(this, true);

            // Create the Undo/Redo Stack
            int nUndoRedoMaxDepth = 10;
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
            this.m_itemInsertGeneralNote = new System.Windows.Forms.ToolStripMenuItem();
            this.m_itemInsertExegeticalNote = new System.Windows.Forms.ToolStripMenuItem();
            this.m_itemInsertHintNote = new System.Windows.Forms.ToolStripMenuItem();
            this.m_itemInsertConsultantNote = new System.Windows.Forms.ToolStripMenuItem();
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
            this.m_menuSetUpFeatures = new System.Windows.Forms.ToolStripMenuItem();
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
            this.m_menuShowTranslationsPane = new System.Windows.Forms.ToolStripMenuItem();
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
            this.m_SplitContainer = new System.Windows.Forms.SplitContainer();
            this.m_Taskbar = new System.Windows.Forms.ToolStrip();
            this.m_tbTaskName = new System.Windows.Forms.ToolStripLabel();
            this.m_tbPadlock = new System.Windows.Forms.ToolStripButton();
            this.m_tbLanguageInfo = new System.Windows.Forms.ToolStripLabel();
            this.m_tbCurrentPassage = new System.Windows.Forms.ToolStripLabel();
            m_separator2 = new System.Windows.Forms.ToolStripSeparator();
            this.m_ToolStrip.SuspendLayout();
            this.m_toolStripContainer.BottomToolStripPanel.SuspendLayout();
            this.m_toolStripContainer.ContentPanel.SuspendLayout();
            this.m_toolStripContainer.TopToolStripPanel.SuspendLayout();
            this.m_toolStripContainer.SuspendLayout();
            this.m_StatusStrip.SuspendLayout();
            this.m_SplitContainer.SuspendLayout();
            this.m_Taskbar.SuspendLayout();
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
            this.m_btnInsertNote.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_itemInsertGeneralNote,
            this.m_itemInsertExegeticalNote,
            this.m_itemInsertHintNote,
            this.m_itemInsertConsultantNote});
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
            // m_itemInsertGeneralNote
            // 
            this.m_itemInsertGeneralNote.Image = global::OurWord.Properties.Resources.NoteGeneric_Me;
            this.m_itemInsertGeneralNote.Name = "m_itemInsertGeneralNote";
            this.m_itemInsertGeneralNote.Size = new System.Drawing.Size(196, 22);
            this.m_itemInsertGeneralNote.Tag = "General";
            this.m_itemInsertGeneralNote.Text = "General Note";
            this.m_itemInsertGeneralNote.ToolTipText = "Insert a general note.";
            this.m_itemInsertGeneralNote.Click += new System.EventHandler(this.cmdInsertNote);
            // 
            // m_itemInsertExegeticalNote
            // 
            this.m_itemInsertExegeticalNote.Image = global::OurWord.Properties.Resources.NoteExegesis_Me;
            this.m_itemInsertExegeticalNote.Name = "m_itemInsertExegeticalNote";
            this.m_itemInsertExegeticalNote.Size = new System.Drawing.Size(196, 22);
            this.m_itemInsertExegeticalNote.Tag = "Exegetical";
            this.m_itemInsertExegeticalNote.Text = "Exegetical Note";
            this.m_itemInsertExegeticalNote.ToolTipText = "Insert a note explaining your exegesis";
            this.m_itemInsertExegeticalNote.Click += new System.EventHandler(this.cmdInsertNote);
            // 
            // m_itemInsertHintNote
            // 
            this.m_itemInsertHintNote.Image = global::OurWord.Properties.Resources.NoteHint_Me;
            this.m_itemInsertHintNote.Name = "m_itemInsertHintNote";
            this.m_itemInsertHintNote.Size = new System.Drawing.Size(196, 22);
            this.m_itemInsertHintNote.Tag = "HintForDrafting";
            this.m_itemInsertHintNote.Text = "Hint for Daughter Note";
            this.m_itemInsertHintNote.ToolTipText = "Insert a note to be used in drafting a daughter translation, when this translatio" +
                "n is used as a front.";
            this.m_itemInsertHintNote.Click += new System.EventHandler(this.cmdInsertNote);
            // 
            // m_itemInsertConsultantNote
            // 
            this.m_itemInsertConsultantNote.Image = global::OurWord.Properties.Resources.NoteConsultant_Me;
            this.m_itemInsertConsultantNote.Name = "m_itemInsertConsultantNote";
            this.m_itemInsertConsultantNote.Size = new System.Drawing.Size(196, 22);
            this.m_itemInsertConsultantNote.Tag = "Consultant";
            this.m_itemInsertConsultantNote.Text = "Consultant Note";
            this.m_itemInsertConsultantNote.ToolTipText = "Insert a note for the Consultant";
            this.m_itemInsertConsultantNote.Click += new System.EventHandler(this.cmdInsertNote);
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
            this.m_menuSetUpFeatures,
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
            // m_menuSetUpFeatures
            // 
            this.m_menuSetUpFeatures.Image = ((System.Drawing.Image)(resources.GetObject("m_menuSetUpFeatures.Image")));
            this.m_menuSetUpFeatures.Name = "m_menuSetUpFeatures";
            this.m_menuSetUpFeatures.Size = new System.Drawing.Size(211, 22);
            this.m_menuSetUpFeatures.Text = "Set Up &Features...";
            this.m_menuSetUpFeatures.ToolTipText = "Set which features are turned on and off.";
            this.m_menuSetUpFeatures.Click += new System.EventHandler(this.cmdSetUpFeatures);
            // 
            // m_Synchronize
            // 
            this.m_Synchronize.Image = global::OurWord.Properties.Resources.MoveDown;
            this.m_Synchronize.Name = "m_Synchronize";
            this.m_Synchronize.Size = new System.Drawing.Size(211, 22);
            this.m_Synchronize.Text = "S&ynchronize";
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
            this.m_menuRunDebugTestSuite.Text = "&Run Debug Test Suite...";
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
            this.m_menuShowTranslationsPane,
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
            this.m_menuDrafting.Size = new System.Drawing.Size(232, 22);
            this.m_menuDrafting.Tag = "Draft";
            this.m_menuDrafting.Text = "&Drafting";
            this.m_menuDrafting.ToolTipText = "Set the main window to do Drafting.";
            this.m_menuDrafting.Click += new System.EventHandler(this.cmdJobDrafting);
            // 
            // m_menuNaturalnessCheck
            // 
            this.m_menuNaturalnessCheck.Image = ((System.Drawing.Image)(resources.GetObject("m_menuNaturalnessCheck.Image")));
            this.m_menuNaturalnessCheck.Name = "m_menuNaturalnessCheck";
            this.m_menuNaturalnessCheck.Size = new System.Drawing.Size(232, 22);
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
            this.m_menuBackTranslation.Size = new System.Drawing.Size(232, 22);
            this.m_menuBackTranslation.Tag = "BT";
            this.m_menuBackTranslation.Text = "&Back Translation";
            this.m_menuBackTranslation.ToolTipText = "Set the main window to work on the Back Translation.";
            this.m_menuBackTranslation.Click += new System.EventHandler(this.cmdJobBackTranslation);
            // 
            // m_menuConsultantPreparationToolStripMenuItem
            // 
            this.m_menuConsultantPreparationToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("m_menuConsultantPreparationToolStripMenuItem.Image")));
            this.m_menuConsultantPreparationToolStripMenuItem.Name = "m_menuConsultantPreparationToolStripMenuItem";
            this.m_menuConsultantPreparationToolStripMenuItem.Size = new System.Drawing.Size(232, 22);
            this.m_menuConsultantPreparationToolStripMenuItem.Tag = "ConsultantPreparation";
            this.m_menuConsultantPreparationToolStripMenuItem.Text = "Consultant &Preparation";
            this.m_menuConsultantPreparationToolStripMenuItem.Click += new System.EventHandler(this.cmdJobConsultantPreparation);
            // 
            // m_separatorWindow
            // 
            this.m_separatorWindow.Name = "m_separatorWindow";
            this.m_separatorWindow.Size = new System.Drawing.Size(229, 6);
            // 
            // m_menuShowTranslationsPane
            // 
            this.m_menuShowTranslationsPane.Name = "m_menuShowTranslationsPane";
            this.m_menuShowTranslationsPane.Size = new System.Drawing.Size(232, 22);
            this.m_menuShowTranslationsPane.Text = "Show &Other Translations Pane";
            this.m_menuShowTranslationsPane.ToolTipText = "Show the Other Translations Pane";
            this.m_menuShowTranslationsPane.Click += new System.EventHandler(this.cmdToggleOtherTranslationsPane);
            // 
            // m_menuZoom
            // 
            this.m_menuZoom.Name = "m_menuZoom";
            this.m_menuZoom.Size = new System.Drawing.Size(232, 22);
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
            this.m_toolStripContainer.ContentPanel.Controls.Add(this.m_SplitContainer);
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
            this.m_toolStripContainer.TopToolStripPanel.Controls.Add(this.m_Taskbar);
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
            // m_SplitContainer
            // 
            this.m_SplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_SplitContainer.Location = new System.Drawing.Point(0, 0);
            this.m_SplitContainer.Name = "m_SplitContainer";
            // 
            // m_SplitContainer.Panel1
            // 
            this.m_SplitContainer.Panel1.Resize += new System.EventHandler(this.onMainWindowResize);
            this.m_SplitContainer.Panel1MinSize = 100;
            this.m_SplitContainer.Panel2MinSize = 100;
            this.m_SplitContainer.Size = new System.Drawing.Size(946, 441);
            this.m_SplitContainer.SplitterDistance = 630;
            this.m_SplitContainer.TabIndex = 0;
            this.m_SplitContainer.SplitterMoved += new System.Windows.Forms.SplitterEventHandler(this.cmdSplitterMoved);
            // 
            // m_Taskbar
            // 
            this.m_Taskbar.BackColor = System.Drawing.Color.Gray;
            this.m_Taskbar.Dock = System.Windows.Forms.DockStyle.None;
            this.m_Taskbar.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.m_Taskbar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_tbTaskName,
            this.m_tbPadlock,
            this.m_tbLanguageInfo,
            this.m_tbCurrentPassage});
            this.m_Taskbar.Location = new System.Drawing.Point(0, 0);
            this.m_Taskbar.Name = "m_Taskbar";
            this.m_Taskbar.Size = new System.Drawing.Size(946, 25);
            this.m_Taskbar.Stretch = true;
            this.m_Taskbar.TabIndex = 2;
            // 
            // m_tbTaskName
            // 
            this.m_tbTaskName.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_tbTaskName.ForeColor = System.Drawing.Color.White;
            this.m_tbTaskName.Name = "m_tbTaskName";
            this.m_tbTaskName.Size = new System.Drawing.Size(70, 22);
            this.m_tbTaskName.Text = "Drafting";
            // 
            // m_tbPadlock
            // 
            this.m_tbPadlock.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.m_tbPadlock.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.m_tbPadlock.Image = ((System.Drawing.Image)(resources.GetObject("m_tbPadlock.Image")));
            this.m_tbPadlock.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.m_tbPadlock.Name = "m_tbPadlock";
            this.m_tbPadlock.Size = new System.Drawing.Size(23, 22);
            this.m_tbPadlock.Text = "Locked";
            this.m_tbPadlock.ToolTipText = "This book is locked and cannot be edited.";
            // 
            // m_tbLanguageInfo
            // 
            this.m_tbLanguageInfo.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_tbLanguageInfo.ForeColor = System.Drawing.Color.White;
            this.m_tbLanguageInfo.Name = "m_tbLanguageInfo";
            this.m_tbLanguageInfo.Size = new System.Drawing.Size(88, 22);
            this.m_tbLanguageInfo.Text = "(Language)";
            // 
            // m_tbCurrentPassage
            // 
            this.m_tbCurrentPassage.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_tbCurrentPassage.ForeColor = System.Drawing.Color.Wheat;
            this.m_tbCurrentPassage.Name = "m_tbCurrentPassage";
            this.m_tbCurrentPassage.Size = new System.Drawing.Size(84, 22);
            this.m_tbCurrentPassage.Text = "(passage)";
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
            this.m_SplitContainer.ResumeLayout(false);
            this.m_Taskbar.ResumeLayout(false);
            this.m_Taskbar.PerformLayout();
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
            LocDB.Initialize(G.GetApplicationDataFolder());

            // Set the resource location (so the splash picture will be visible)
            JWU.ResourceLocation = "OurWord.Res.";

            // Display a splash screen while we're loading. We want to retrieve everything
            // needed from the localization database, so that the SplashScreen, which runs
            // on its own thread, isn't having to go cross-thread to get them.
			foreach (string s in vArgs)
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

            // Now start loading & run the program
            OurWordMain.s_App = new OurWordMain();
            Application.EnableVisualStyles();
            Application.Run(s_App);

            // All done, release the Mutex
            if (s_EnsureOneInstanceOnlyMutex != null)
               s_EnsureOneInstanceOnlyMutex.ReleaseMutex();
        }
		#endregion

		// Features --------------------------------------------------------------------------
		#region EMBEDDED CLASS: FeaturesMgr
		public class FeaturesMgr
		{
			// Attrs -------------------------------------------------------------------------
			DialogSetupFeatures m_Dlg = null;

			// Values (obtain from current project) ------------------------------------------
            #region Attr{g}: bool F_JobNaturalness
            public bool F_JobNaturalness
            {
                get
                {
                    return m_Dlg.GetEnabledState(ID.fJobNaturalness.ToString());
                }
            }
            #endregion
			#region Attr{g}: bool F_JobBT
			public bool F_JobBT
			{
				get
				{
					return m_Dlg.GetEnabledState( ID.fJobBT.ToString());
				}
			}
			#endregion
            #region Attr{g}: bool F_ConsultantPreparation
            public bool F_ConsultantPreparation
            {
                get
                {
                    return m_Dlg.GetEnabledState(ID.fJobConsultantPreparation.ToString());
                }
            }
            #endregion
            #region Attr{g}: bool TranslatorNotes
            public bool TranslatorNotes
            {
                get
                {
                    return m_Dlg.GetEnabledState(ID.fTranslatorNotes.ToString());
                }
            }
            #endregion

            #region Attr{g}: bool F_Project
			public bool F_Project
			{
				get
				{
					return m_Dlg.GetEnabledState( ID.fProject.ToString());
				}
			}
			#endregion
            #region Attr{g}: bool F_PropertiesDialog
            public bool F_PropertiesDialog
            {
                get
                {
                    return m_Dlg.GetEnabledState(ID.fConfigurationDialog.ToString());
                }
            }
            #endregion
			#region Attr{g}: bool F_Print
			public bool F_Print
			{
				get
				{
					return m_Dlg.GetEnabledState( ID.fPrint.ToString());
				}
			}
			#endregion
			#region Attr{g}: bool F_RestoreBackup
			public bool F_RestoreBackup
			{
				get
				{
					return m_Dlg.GetEnabledState( ID.fRestoreBackup.ToString());
				}
			}
			#endregion
			#region Attr{g}: bool F_Filter
			public bool F_Filter
			{
				get
				{
					return m_Dlg.GetEnabledState( ID.fFilter.ToString());
				}
			}
			#endregion
            #region Attr{g}: bool F_Localizer
            public bool F_Localizer
            {
                get
                {
                    return m_Dlg.GetEnabledState(ID.fLocalizer.ToString());
                }
            }
            #endregion
            #region Attr{g}: bool F_StructuralEditing
            public bool F_StructuralEditing
            {
                get
                {
                    return m_Dlg.GetEnabledState(ID.fStructuralEditing.ToString());
                }
            }
            #endregion
            #region Attr{g}: bool F_UndoRedo
            public bool F_UndoRedo
            {
                get
                {
                    return m_Dlg.GetEnabledState(ID.fUndoRedo.ToString());
                }
            }
            #endregion
            #region Attr{g}: bool F_GoTo_FirstLast
            public bool F_GoTo_FirstLast
            {
                get
                {
                    return m_Dlg.GetEnabledState(ID.fGoTo_FirstLast.ToString());
                }
            }
            #endregion
            #region Attr{g}: bool F_GoTo_Chapter
            public bool F_GoTo_Chapter
            {
                get
                {
                    return m_Dlg.GetEnabledState(ID.fGoTo_Chapter.ToString());
                }
            }
            #endregion
            #region Attr{g}: bool F_JustTheBasics
			public bool F_JustTheBasics
			{
				get
				{
					return m_Dlg.GetEnabledState( ID.fJustTheBasics.ToString());
				}
			}
			#endregion
            #region Attr{g}: bool F_Export
            public bool F_Export
            {
                get
                {
                    return m_Dlg.GetEnabledState(ID.fExport.ToString());
                }
            }
            #endregion

            // Scaffolding -------------------------------------------------------------------
			#region Constructor()
			public FeaturesMgr()
			{
				m_Dlg = new DialogSetupFeatures();
				Setup();
			}
			#endregion
			#region Method: bool ShowDialog(Form formParent)
			public bool ShowDialog(Form formParent)
			{
				// Make sure we are up-to-date with the settings
				Setup();

				// Show the dialog (return if the user canceled)
				if (DialogResult.OK != m_Dlg.ShowDialog(formParent) )
					return false;

				return true;
			}
			#endregion

            // Set up the features OW currently supports
            // 1. Define an ID
            // 2. Add it into the Setup method
            // 3. Add it to the Localizations database
            // 4. Set up an Attribute above for access elsewhere in the software
            // 5. Typical other areas to code:
            //    - Menu & toolbar visibility / enabling
            #region IDs
            public enum ID
            {
                fProject,
                fConfigurationDialog,
                fPrint,
                fJobBT,
                fJobNaturalness,
                fJobConsultantPreparation,
                fRestoreBackup,
                fFilter,
                fJustTheBasics,
                fStructuralEditing,
                fUndoRedo,
                fLocalizer,
                fGoTo_FirstLast,
                fGoTo_Chapter,
                fTranslatorNotes,
                fExport,
                kLast
            };
            #endregion
            const string c_sNodeWindows = "Windows";
            const string c_sNodeTools = "Tools";
            const string c_sNodeNavigation = "Navigation";
            const string c_sNodeEditing = "Editing";
			#region Method: void Setup()
			private void Setup()
			{
				Debug.Assert(null != m_Dlg);

                // Clear out everything previous
				m_Dlg.Clear();

                // Add the various features
                #region WINDOWS FEATURES
                m_Dlg.Add(ID.fJobBT.ToString(),
                    false,
                    false,
                    c_sNodeWindows,
                    "Back Translation Window",
                    "A layout where you can do a Back Translation; that is, where you provide " +
                        "a translation  in the language the consultant understands.");

                m_Dlg.Add(ID.fJobNaturalness.ToString(),           
                    false, 
                    false,
                    c_sNodeWindows,
                    "Naturalness Check Window",
                    "A layout where only the translation is visible, so that you can read through " +
                        "for naturalness, without being influenced by the front translation.");
                       
                m_Dlg.Add(ID.fJobConsultantPreparation.ToString(),  
                    false,
                    false,
                    c_sNodeWindows,
                    "Consultant Preparation Window",
                    "Displays the Front/Model translation and the Target translation, with both " +
                        "the vernacular and back translations. Use this to preparefor the " +
                        "consultant (e.g., exegetical notes.)");

                #endregion
                #region EDITING FEATURES
                m_Dlg.Add(ID.fStructuralEditing.ToString(),
                    false,
                    false,
                    c_sNodeEditing,
                    "Structural Editing",
                    "Enables the translator to split and join paragraphs, or to assign different " +
                        "styles to a paragraph. By doing this the translation will depart from " +
                        "the paragraph structure of the front translation.");

                m_Dlg.Add(ID.fUndoRedo.ToString(),
                    true,
                    false,
                    c_sNodeEditing,
                    "Undo",
                    "Enables the Undo and Redo menus, by which you can undo actions such as " +
                        "typing, deleting, splitting and joining paragraphs, etc.");

                m_Dlg.Add(ID.fTranslatorNotes.ToString(),
                    false,
                    false,
                    c_sNodeEditing,
                    "Translator Notes",
                    "Enables the Translator Notes button and icons, through which you can make notes " +
                        "about the translation and share them with others on your team.");

                #endregion
                #region TOOLS FEATURES
                m_Dlg.Add(ID.fRestoreBackup.ToString(),    
                    false,
                    false,
                    c_sNodeTools,
                    "Restore Backed-up Files",
                    "Files can be automatically backed up (via the Tools-Options command) " +
                        "to a flash card or other storage device. If you turn on this Restore " +
                        "feature, the Tools menu will provide access to a dialog by which the " +
                        "current file can be replaced by a previously stored backup.");

                m_Dlg.Add(ID.fConfigurationDialog.ToString(),
                    true,
                    false,
                    c_sNodeTools,
                    "Configuration Dialog",
                    "Turn this on if you are want to adjust the settings for a project.");

                m_Dlg.Add(ID.fFilter.ToString(),           
                    false,
                    false,
                    c_sNodeTools,
                    "Only show Sections that have (Filter)",
                    "The \"Go To\" Menu (e.g., Next, Previous commands) will only go to those " +
                        "sections which match certain criteria. E.g., they must have a certain word " +
                        "defined, or must have mismatched quotes. This can be a good way to locate " +
                        "errors, or To Do Notes.");

                m_Dlg.Add(ID.fLocalizer.ToString(),
                    false,
                    false,
                    c_sNodeTools,
                    "Localization Dialog",
                    "This dialog, appearing in the Tools menu, allows you to translate the " +
                        "user interface of OurWord into another language.");

                m_Dlg.Add(ID.fExport.ToString(),
                    false,
                    false,
                    c_sNodeTools,
                    "Export",
                    "Provides export to Toolbox, USFM, GoBible, and potentially other file formats.");
                #endregion
                #region NAVIGATION FEATURES
                m_Dlg.Add(ID.fGoTo_FirstLast.ToString(),
                    true,
                    false,
                    c_sNodeNavigation,
                    "First & Last Buttons",
                    "Makes the First and Last Buttons visible, by which you can navigate to the " +
                        "first and last sections in the book. Experienced users may want to have " +
                        "these buttons visible for easier movement around the book.");

                m_Dlg.Add(ID.fGoTo_Chapter.ToString(),
                    true,
                    false,
                    c_sNodeNavigation,
                    "Chapter Button",
                    "Makes the Chapter button visible, by which you can navigate directly to " +
                        "the first section in the desired chapter. Experienced users may want " +
                        "to have this button visible for easier movement around the book.");
                #endregion

                m_Dlg.Add(ID.fProject.ToString(),  
                    true, 
                    false,
                    "",
                    "Project New / Open / etc.",
                    "Turn this on if you are working with multiple projects.");

                m_Dlg.Add(ID.fPrint.ToString(),            
                    false,
                    false,
                    "",
                    "Printing",
                    "The book can be formatted and printed. ");

            }
			#endregion
		};
		#endregion
		#region Attr{g}: FeaturesMgr Features
		static public FeaturesMgr Features
		{
			get
			{
				return s_Features;
			}
		}
		static public FeaturesMgr s_Features = null;
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
        #region Method: void OnLeaveProject(bCommitToRepository)
        private void OnLeaveProject(bool bCommitToRepository)
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
		private void OnEnterProject()
		{
			// If we aren't already navigated to a book, then attempt to find one.
			DB.Project.Nav.GoToReasonableBook(G.CreateProgressIndicator());

            // A new project may have different settings for whether or not the
            // Side Windows are displayed.
            SetupSideWindows();

            // Window Zoom factor
            SetZoomFactor();

            // Window contents, etc.
			OnEnterSection();
		}
		#endregion

        // Event Handlers --------------------------------------------------------------------
        #region Event: cmdSplitterMoved
        float m_fSplitterPercent = 65;
        private void cmdSplitterMoved(object sender, SplitterEventArgs e)
            // Set minimum, maximum, and initial positions for the splitter, using a Percent
            // basis rather than the pixel basis that DotNet provides.
        {
            if (!m_SplitContainer.Panel2Collapsed)
            {
                m_fSplitterPercent = ((float)m_SplitContainer.SplitterDistance / (float)Width) * 100.0F;
                m_fSplitterPercent = Math.Max(m_fSplitterPercent, 50);
                m_fSplitterPercent = Math.Min(m_fSplitterPercent, 80);
            }
        }
        #endregion
        #region Event: cmdLoad
        private void cmdLoad(object sender, System.EventArgs e)
		{
			// Init the Help system
            HelpSystem.Initialize(this, "OurWordMain.chm");

            // Retrieve the most recent item
            string sPath = JW_Registry.GetValue(c_sLastProjectOpened, "" );

			// Read in the project
			DB.Project = new DProject();
            if (!string.IsNullOrEmpty(sPath) && File.Exists(sPath))
                DB.Project.LoadFromFile(ref sPath, G.CreateProgressIndicator());

            // Initial Splitter Position
            m_SplitContainer.SplitterDistance = (int)((float)m_fSplitterPercent * (float)Width / 100.0F);

            // Restore which layout is active
            SetCurrentLayout(WLayout.GetLayoutFromRegistry(WndDrafting.c_sName));

            // Restore to where we last were.
            DB.Project.Nav.RetrievePositionFromRegistry(G.CreateProgressIndicator());

            // Set up the views, make the initial selection, etc.
            OnEnterProject();

			// Remember the previous placement of the window on the screen (Do this late
			// in the load sequence so that we avoid a multiple screen redraw.) 
			m_WindowState.RestoreWindowState();

			// Initialize AutoSave Timer
			InitializeAutoSave();

            // Loading all done: Close the splash screen
			if (SplashScreen.IsEnabled)
			{
				if (SplashScreen.Wnd.InvokeRequired)
				{
					SplashScreen.StopSplashScreen_Callback d =
						new SplashScreen.StopSplashScreen_Callback(SplashScreen.Wnd.Stop);
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
			OnLeaveProject(true);

			// Save the current project's registry info
            if (!string.IsNullOrEmpty(DB.Project.StoragePath))
                JW_Registry.SetValue(c_sLastProjectOpened, DB.Project.StoragePath);
			DB.Project.Nav.SavePositionToRegistry();
			
			// Save the window position
			m_WindowState.SaveWindowState();

			DB.TeamSettings.WriteToFile(G.CreateProgressIndicator());
        }
        #endregion
        #region Event: OnResize - prevent the size from becomming too small
        protected override void OnResize(EventArgs e)
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
        #region Event: onMainWindowResize - its time to re-lay out the content window
        private void onMainWindowResize(object sender, EventArgs e)
        {
            SizeAndLayoutContentWindows();
        }
        #endregion
        #region Method: void SizeAndLayoutContentWindows()
        void SizeAndLayoutContentWindows()
        {
            // Tests to see if we can proceed with layout and sizing
            if (null != G.App && G.App.WindowState == FormWindowState.Minimized)
                return;
            if (null == CurrentLayout)
                return;
            if (m_SplitContainer.Panel1.Width == 0 || m_SplitContainer.Panel1.Height == 0)
                return;   // Happens in Mono, not in Windows

            // the MainWindow needs to have a size for its double buffering
            CurrentLayout.SetSize(m_SplitContainer.Panel1.Width, m_SplitContainer.Panel1.Height);

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
			OnLeaveSection();

			Print p = new Print();
			p.Do();
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

            // Walk through the wizard; we do nothing unless the User makes it through
            // (as signaled by DialogResult.OK).
            Dialogs.WizNewProject.WizNewProject wiz = new Dialogs.WizNewProject.WizNewProject();
            if (DialogResult.OK != wiz.ShowDialog())
                return;

            // Make sure the current project is saved and up-to-date, before we create
            // the new one. Commit to the Repository, to have a restoreo  point if we need it.
            OnLeaveProject(true);

            // Create and initialize the new project according to the settings
            DProject project = new DProject(wiz.ProjectName);

            // Team Settings: start with the factory default; load over it if a file already exists,
            // otherwise create the new cluster
            project.TeamSettings = new DTeamSettings(wiz.ChosenCluster.Name);
            project.TeamSettings.EnsureInitialized();
            project.TeamSettings.InitialCreation(G.CreateProgressIndicator());

            // Create the front translation. If the settings file exists, load it; otherwise
            // create its folder, settings file, etc.
            project.FrontTranslation = new DTranslation(wiz.FrontName);
            project.FrontTranslation.InitialCreation(G.CreateProgressIndicator());

            // Target Translation
            project.TargetTranslation = new DTranslation(wiz.ProjectName);
            project.TargetTranslation.InitialCreation(G.CreateProgressIndicator());

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
        }
		#endregion
        #region Cmd: cmdDownloadRepository
        const string c_sCloneFailedMsg = "Repository.CloneTo() failed.";
        const string c_sPullFailedMsg = "Repository.Pull() failed.";
        private void cmdDownloadRepository(Object sender, EventArgs e)
        {
            // Is Mercurial Installed?
            if (!HgRepositoryBase.CheckMercialIsInstalled())
            {
                LocDB.Message("msgHgNotInstalled",
                    "It appears that Mercurial is not installed on this computer.\n" +
                    "Please install it, and then try again.",
                    null,
                    LocDB.MessageTypes.Error);
                return;
            }

            // We'll contruct the wizard outside of the loop, in case we have to go
            // back and change settings (e.g., on an error)
            var wiz = new WizInitializeFromRepository();

            // Loop until success or give up
            while (true)
            {
                // Get the user's settings for the to-be-downloaded cluster
                if (DialogResult.OK != wiz.ShowDialog(this))
                    return;

                // Make sure the current project is saved and up-to-date, before we create
                // the new one. Commit to the Repository, to have a restore  point if we need it.
                OnLeaveProject(true);

                // Create the ClusterInfo object
                string sParentFolder = (wiz.IsInMyDocuments) ?
                    JWU.GetMyDocumentsFolder(null) :
                    JWU.GetLocalApplicationDataFolder(ClusterInfo.c_sLanguageDataFolder);
                ClusterInfo ci = new ClusterInfo(wiz.ClusterName, sParentFolder);

                // If the Cluster already exists, we don't continue, else we'd overwrite it.
                if (Directory.Exists(ci.ClusterFolder))
                {
                    bool bTryAgain = LocDB.Message("msgClusterAlreadyExists",
                        "We cannot create cluster {0} because it already exists.\n\n" + 
                        "Do you want to try again?",
                        new string[] { ci.Name },
                        LocDB.MessageTypes.WarningYN);
                    if (!bTryAgain)
                        return;
                    continue;
                }

                // Can we access the Internet?
                bool bCanAccessInternet = Repository.CanAccessInternet();
                if (!bCanAccessInternet)
                {
                    bool bTryAgain = LocDB.Message("msgCannotAccessInternet",
                        "OurWord is unable to access the Internet.\n\n" + 
                        "Please check that you have an Internet connection, then press " +
                        "\"Yes\" to try again; or \"No\" to cancel.",
                        null,
                        LocDB.MessageTypes.WarningYN);
                    if (!bTryAgain)
                        return;
                    continue;
                }

                // Make changes to the disk
                try
                {
                    // Progress Dialog
                    SynchProgressDlg.Start(true);
                    while (!SynchProgressDlg.IsCreated)
                        Thread.Sleep(500);
                    Thread.Sleep(2000);
                    SynchProgressDlg.SetStepSuccess(SynchProgressDlg.Steps.InternetAccess);

                    // Create the Internet Repo and save the wizard's information
                    var internetRepository = new HgInternetRepository(wiz.ClusterName);
                    internetRepository.Server = wiz.Url;
                    internetRepository.UserName = wiz.UserName;
                    internetRepository.Password = wiz.Password;

                    // Clone the repository (thus creating the Cluster folder and
                    // the .Hg subfolder)
                    SynchProgressDlg.SetStepStart(SynchProgressDlg.Steps.Pulling);
                    string sRepository = Repository.BuildRemoteRepositoryString(
                        wiz.Url, wiz.UserName, wiz.Password);
                    if (!Repository.CloneTo(ci.ClusterFolder, sRepository))
                    {
                        SynchProgressDlg.SetStepFailed(SynchProgressDlg.Steps.Pulling);
                        throw new Exception(c_sCloneFailedMsg);
                    }

                    // Open the first project we find (if any); we need it in order to
                    // save the repository settings
                    var vsProjects = ci.GetClusterLanguageList(true);
                    if (null != vsProjects && vsProjects.Count > 0)
                    {
                        // Open the project
                        string sPath = ci.GetProjectPath(vsProjects[0]);
                        DB.Project = new DProject();
                        DB.Project.LoadFromFile(ref sPath, G.CreateProgressIndicator());
                        DB.Project.Nav.GoToFirstAvailableBook(G.CreateProgressIndicator());
                        OnEnterProject();

                        // Save the Collaboration repository settings
//                        HgInternetRepository.Server = wiz.Url;
//                        HgInternetRepository.UserName = wiz.UserName;
//                        HgInternetRepository.Password = wiz.Password;
                    }

                }
                catch (Exception ex)
                {
                    // Clean Up
                    if (Directory.Exists(ci.ClusterFolder))
                        Directory.Delete(ci.ClusterFolder, true);

                    if (ex.Message == c_sCloneFailedMsg)
                    {
                        bool bTryAgain = LocDB.Message("msgCloneFailed",
                            "OurWord was unable to retrieve the data from the Internet.\n\n" +
                            "Do you wish to try again?",
                            null,
                            LocDB.MessageTypes.WarningYN);
                        if (!bTryAgain)
                            return;
                        continue;
                    }

                    if (ex.Message == c_sPullFailedMsg)
                    {
                         bool bTryAgain = LocDB.Message("msgPullFailed",
                            "OurWord was unable to build folders on your disk.\n\n" +
                            "Do you wish to try again?",
                            null,
                            LocDB.MessageTypes.WarningYN);
                        if (!bTryAgain)
                            return;
                        continue;
                   }

                    bool bAgain = LocDB.Message("msgDownloadClusterFailedGeneric",
                        "OurWord was unable to download / create the cluster on your computer, " +
                        "for unknown reason.\n\n" +
                            "Do you wish to try again?",
                            null,
                            LocDB.MessageTypes.WarningYN);
                    if (!bAgain)
                        return;
                    continue;
                }
                finally
                {
                    SynchProgressDlg.Stop();
                }

                // If here, then we were successful
                break;
            }

            // Display a dialog declaring Success, and giving a choice of which project to open
            LocDB.Message("msgRepoCreated",
                "The Respository has been sucessfully downloaded to your computer.",
                null, LocDB.MessageTypes.Info);
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
            string sPath = (string)m.Tag;
            if (string.IsNullOrEmpty(sPath))
                return;

            // Open the requested project
            OnLeaveProject(true);
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
                foreach (string s in vsRaw)
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
            foreach (string s in vs)
            {
 //               string sPath = ci.ClusterFolder + 
 //                   ".Settings" + Path.DirectorySeparatorChar +
 //                   s + ".owp";

                var mi = new ToolStripMenuItem(s, null, onClick);
 //               mi.Tag = sPath;
                mi.Tag = ci.GetProjectPath(s);
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

            // Get the user's desires (or cancel)
            DialogExport dlgDesires = new DialogExport(DB.TargetTranslation);
            if (DialogResult.OK != dlgDesires.ShowDialog(this))
                return;

            // Create and display the progress dialog
            DlgExportProgress.Start();
            DlgExportProgress.SetCurrentBook("Setting up...");

            // Loop through the books of the requested translation
            foreach (DBook book in DB.TargetTranslation.BookList)
            {
                // Does the user wish to cancel?
                if (DlgExportProgress.UserSaysCancel)
                    break;

                // Upload the status dialog with the correct book
                DlgExportProgress.SetCurrentBook(DB.TargetTranslation.DisplayName + " - " +
                    book.DisplayName);

                // Load the book if not already in memory
                bool bIsLoaded = book.Loaded;
                book.LoadBook(G.CreateProgressIndicator());
                if (!book.Loaded)
                    continue;

                // Compute the file name
                string sExportPath = JWU.GetMyDocumentsFolder(dlgDesires.ExportSubFolderName);
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

                if (dlgDesires.ExportToToolbox)
                {
                    book.ExportToToolbox(sExportPath + ".db", G.CreateProgressIndicator());
                }

                // Unload the book if it was previously unloaded, so that we don't clog
                // up memory
                if (!bIsLoaded)
                    book.Unload(new NullProgress());
            }

            // Done with the progress dialog
            DlgExportProgress.Stop();

        }
        #endregion

        // Edit Dropdown
        #region Cmd: cmdUndo
        private void cmdUndo(object sender, EventArgs e)
        {
            if (Features.F_UndoRedo)
                URStack.Undo();
        }
        #endregion
        #region Cmd: cmdRedo
        private void cmdRedo(object sender, EventArgs e)
        {
            if (Features.F_UndoRedo)
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
            OWWindow wnd = FocusedWindow;
            if (null != wnd)
                wnd.cmdPaste();
		}
		#endregion
        #region Cmd: cmdChangeParagraphStyle
        private void cmdChangeParagraphStyle(Object sender, EventArgs e)
        {
            ToolStripMenuItem mi = sender as ToolStripMenuItem;
            if (null == mi)
                return;

            string sStyleAbbrev = mi.Tag as string;
            if (string.IsNullOrEmpty(sStyleAbbrev))
                return;

            OWWindow wnd = FocusedWindow;
            if (null != wnd)
                wnd.cmdChangeParagraphTo(sStyleAbbrev);
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
        #region Method: void _UpdateSideWindows()
        private void _UpdateSideWindows()
        {
            // Add/Remove the various pane from the side windows
            SetupSideWindows();

            // Reset the window contents (1) to populate the Notes window if
            // present, and (2) to show/hide the notes icons in the main 
            // window.
            ResetWindowContents();

            // Reset the menus, so that the checkmark will appear
            // next to the appropriate Toggle commands.
            SetupMenusAndToolbarsVisibility();
        }
        #endregion
        #region Cmd: cmdToggleOtherTranslationsPane
        private void cmdToggleOtherTranslationsPane(Object sender, EventArgs e)
        {
            DProject.VD_ShowTranslationsPane = !DProject.VD_ShowTranslationsPane;
            _UpdateSideWindows();
        }
        #endregion

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
                int[] v = new int[] { 
                60, 70, 80, 90, 100, 110, 120, 130, 140, 150, 175, 200, 225, 250 };

                foreach (int n in v)
                {
                    ToolStripMenuItem mi = new ToolStripMenuItem(n.ToString() + "%", null,
                        cmdChangeZoomPercent, "zzom_" + n.ToString());
                    mi.Tag = n;
                    m_menuZoom.DropDownItems.Add(mi);
                }
            }

            // Check the one that is current
            foreach (ToolStripMenuItem mi in m_menuZoom.DropDownItems)
            {
                mi.Checked = ((int)mi.Tag == G.ZoomPercent) ? true : false;
            }
        }
        #endregion
        #region Cmd: cmdChangeZoomPercent
        private void cmdChangeZoomPercent(Object sender, EventArgs e)
        {
            ToolStripMenuItem mi = sender as ToolStripMenuItem;
            if (null == mi)
                return;

            if (G.ZoomPercent != (int)mi.Tag)
            {

                G.ZoomPercent = (int)mi.Tag;
                SetZoomFactor();
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
			DBook book = DB.Project.STarget.Book;
			IncrementBookStatus dlg = new IncrementBookStatus(book);
			DialogResult result = dlg.ShowDialog();

			// If we incremented, then we need to save the file, so that it will be
			// stored on the disk under the new filename (and thus synchronized with
			// what is in the Project. (See Bug0084)
			if (result != DialogResult.OK)
				return;
            book.DeclareDirty();
			cmdSaveProject(null, null);

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

			DBook BTarget = DB.Project.STarget.Book;

			DialogRestoreFromBackup dlg = new DialogRestoreFromBackup(BTarget);
			if (DialogResult.OK == dlg.ShowDialog())
			{
				OnLeaveProject(true);

				DBook.RestoreFromBackup(BTarget, dlg.BackupPathName, G.CreateProgressIndicator());

				DB.Project.Nav.GoToFirstSection();
				OnEnterSection();
			}
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

			// Get the user's pleasure
			if (null == m_dlgFilter)
				m_dlgFilter = new DialogFilter();
			DialogResult result = m_dlgFilter.ShowDialog(this);

			// If the user Canceled out, then we remove any existing filter
			if (result == DialogResult.Cancel || m_dlgFilter.NothingIsChecked)
			{
				DB.TargetBook.RemoveFilter();
				G.StatusSecondaryMessage = "";
                SetupMenusAndToolbarsVisibility();    // Reset the Navigation toolbar popups
				return;
			}

			// Calculate the filter according to the user's requests, and position
			// at the first match.
            IProgressIndicator progress = G.CreateProgressIndicator();
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
				bool b = FilterTest(section);

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
		}
		#endregion

		#endregion
		#region Cmd: cmdConfigure
        private void cmdConfigure(Object sender, EventArgs e)
		{
            // Retrieve data and save the project to disk. We don't know if the
			// user might remove the book from the project, so we need to make sure
			// it was saved just in case.
			OnLeaveProject(true);

            // Let the user change the properties
            DialogProperties dlg = new DialogProperties();
            dlg.ShowDialog(this);

            // The zoom factor may have changed, so we need to recalculate the fonts
            SetZoomFactor();

            // Re-initialize everything
			OnEnterProject();
		}
		#endregion
		#region Cmd: cmdSetUpFeatures
        private void cmdSetUpFeatures(Object sender, EventArgs e)
		{
			if (true == s_Features.ShowDialog(this) )
			{
				OnLeaveSection();
                SetupMenusAndToolbarsVisibility();
                OnEnterProject();  // Makes sure we reset the side windows
            }
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
            OnLeaveProject(false);

            Localizer dlg = new Localizer(LocDB.DB);
            if (DialogResult.OK == dlg.ShowDialog())
                OnEnterProject();
        }
        #endregion
        #region Cmd: cmdSynchronize
        private void cmdSynchronize(object sender, EventArgs e)
        {
            // Save everything, but don't commit, cause Synchronize will do a commit
            DB.Project.Nav.SavePositionToRegistry();
            OnLeaveProject(false);

            // Do the Synchronize
            var local = new HgLocalRepository(DB.TeamSettings.ClusterFolder);
            var remote = new HgInternetRepository(DB.TeamSettings.DisplayName);
            var username = DB.UserName;
            var synch = new Synchronize(local, remote, username);
            synch.Do();

            // We have to unload, then reload everything
            var sPath = DB.Project.StoragePath;
            DB.Project = new DProject();
            DB.Project.LoadFromFile(ref sPath, G.CreateProgressIndicator());
            DB.Project.Nav.RetrievePositionFromRegistry(G.CreateProgressIndicator());
            OnEnterProject();
        }
        #endregion
        #region Cmd: cmdHistory
        private void cmdHistory(object sender, EventArgs e)
        {
            var history = new DlgHistory(DB.TargetSection);
            history.ShowDialog(this);
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
			DialogHelpAbout dlg = new DialogHelpAbout();
			dlg.ShowDialog(this);
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
            if (null == uiItem)
                return;

            // If the Main Window is not focused, we don't have a context for inserting.
            Debug.Assert(null != CurrentLayout);
            if (!CurrentLayout.Focused)
                return;

            // If we don't have a selection, we inform the user; as we want our notes to be
            // about someohing (and it gives us a title for our annotation.)
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
                        OWToolTip.ToolTip.LaunchToolTipWindow(en);
                        return;
                    }
                }
            }
        }
        #endregion
        #endregion
    }

	#region CLASS G - Globals for convenient access
	public class G
	{
        // Options stored in the registry ----------------------------------------------------
        const string c_sSubKey = "Options";
        const string c_keyZoom = "Zoom";
        #region SAttr{g}: float ZoomPercent
        public static int ZoomPercent
        {
            get
            {
                if (-1 == s_nZoomPercent)
                    s_nZoomPercent = JW_Registry.GetValue(c_sSubKey, c_keyZoom, 100);
                return s_nZoomPercent;
            }
            set
            {
                s_nZoomPercent = value;
                JW_Registry.SetValue(c_sSubKey, c_keyZoom, s_nZoomPercent);
            }
        }
        private static int s_nZoomPercent = -1;
        #endregion
        #region SVAttr{g}: float ZoomFactor
        public static float ZoomFactor
        {
            get
            {
                return ((float)ZoomPercent / 100.0F);
            }
        }
        #endregion

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
        #region SMethod: string GetApplicationDataFolder()
        static public string GetApplicationDataFolder()
        {
            return JWU.GetApplicationDataFolder("OurWord");
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
                Version v = Assembly.GetExecutingAssembly().GetName().Version;

                char chBuild = (char)((int)'a' + v.Build);

                string sVersionNo = v.Major.ToString() + "." + 
                    v.Minor.ToString() +
                    ((v.Build == 0) ? "" : chBuild.ToString());

                return sVersionNo;
            }
        }
        #endregion

        // LocDB strings - access to various Groups in the LocDB -----------------------------
        #region SMethod: string GetLoc_String(sItemID, sEnglish) -        "Strings" 
        static public string GetLoc_String(string sItemID, string sEnglishDefault)
        {
            return LocDB.GetValue(
                new string[] { "Strings" },
                sItemID,
                sEnglishDefault,
                null,
                null);
        }
        #endregion
        #region SMethod: string GetLoc_GeneralUI(sItemID, sEnglish) -     "Strings\GeneralUI"
        static public string GetLoc_GeneralUI(string sItemID, string sEnglishDefault)
        {
            return LocDB.GetValue(
                new string[] { "Strings", "GeneralUI" },
                sItemID, 
                sEnglishDefault,
                null,
                null);
        }
        #endregion
        #region SMethod: string GetLoc_Files(sItemID, sEnglish) -         "Strings\Files"
        static public string GetLoc_Files(string sItemID, string sEnglishDefault)
        {
            return LocDB.GetValue(
                new string[] { "Strings", "Files" },
                sItemID,
                sEnglishDefault,
                null,
                null);
        }
        #endregion
        #region SMethod: string GetLoc_UndoRedo(sItemID, sEnglish) -      "Strings/UndoRedo"
        static public string GetLoc_UndoRedo(string sItemID, string sEnglishDefault)
        {
            return LocDB.GetValue(
                new string[] { "Strings", "UndoRedo" },
                sItemID,
                sEnglishDefault,
                null,
                null);
        }
        #endregion
        #region SMethod: string GetLoc_StyleName(sEnglish) -              "Strings\Styles"
        static public string GetLoc_StyleName(string sEnglish)
        {
            // Compute an ItemID from the name of the style by removing all whitespace
            string sItemID = "";
            foreach (char ch in sEnglish)
            {
                if (!char.IsWhiteSpace(ch))
                    sItemID += ch;
            }
            Debug.Assert(!string.IsNullOrEmpty(sItemID));

            // Retrieve the value
            return LocDB.GetValue(
                new string[] { "Strings", "Styles" },
                sItemID,
                sEnglish,
                null,
                null);
        }
        #endregion
        #region SMethod: string GetLoc_Merge(sEnglish) -                  "Strings\Merge"
        static public string GetLoc_Merge(string sEnglish)
        {
            // Compute an ItemID from the name of the style by removing all whitespace
            string sItemID = "";
            foreach (char ch in sEnglish)
            {
                if (!char.IsWhiteSpace(ch))
                    sItemID += ch;
            }
            Debug.Assert(!string.IsNullOrEmpty(sItemID));

            // Retrieve the value
            return LocDB.GetValue(
                new string[] { "Strings", "MergeWindow" },
                sItemID,
                sEnglish,
                null,
                null);
        }
        #endregion
        #region SMethod: string GetLoc_DialogCommon(sItemID, sEnglish, vsIns) -  "Strings\DialogCommon"
        static public string GetLoc_DialogCommon(string sItemID, string sEnglish, string[] vsInsert)
        {
            return LocDB.GetValue(
                new string[] { "Strings", "DialogCommon" },
                sItemID,
                sEnglish,
                null,
                vsInsert);
        }
        #endregion
        #region SMethod: string GetLoc_Splash(sItemID, sEnglish) -     "Strings\GeneralUI"
        static public string GetLoc_Splash(string sItemID, string sEnglishDefault)
        {
            return LocDB.GetValue(
                new string[] { "SplashScreen" },
                sItemID,
                sEnglishDefault,
                null,
                null);
        }
        #endregion
    }
	#endregion

	#region Exception: eInvalidProjectFile - Project file was the wrong format
	public class eInvalidProjectFile : Exception
	{
		public eInvalidProjectFile(string sFileName)
			: base("Attempt to open a file with the wrong format - " + sFileName)
		{}
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
                    SplashScreen.SetStatus_Callback d = new
                        SplashScreen.SetStatus_Callback(SplashScreen.Wnd.SetStatus);
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
                    SplashScreen.IncrementProgress_Callback d = new
                        SplashScreen.IncrementProgress_Callback(SplashScreen.Wnd.IncrementProgress);
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
                    SplashScreen.ResetProgress_Callback d = new
                        SplashScreen.ResetProgress_Callback(SplashScreen.Wnd.ResetProgress);
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






