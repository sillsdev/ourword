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
using JWdb;

using OurWord.DataModel;
using OurWord.Edit;
using OurWord.View;
using OurWord.Dialogs;
#endregion

namespace OurWord
{
	public class OurWordMain : System.Windows.Forms.Form, IJW_FileMenuIO
		// Main application window and top-level message routing
	{
		// Static Objects & Globals ----------------------------------------------------------
		#region Attr{g/s}: DProject Project - the current project we're editing / displaying / etc.
		static public DProject Project 
		{ 
			get 
			{ 
				return s_project; 
			} 
			set
			{
				s_project = value;
				// Note: Caller should reset the layout content.
			}
		}
		static private DProject s_project = null;
		#endregion
		#region Attr{g}: bool TargetIsLocked
		static public bool TargetIsLocked
		{
			get
			{
				Debug.Assert(null != Project);

				DSection section = Project.STarget;

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
				if (null != Project && null != Project.STarget)
					return Project.STarget.Book;
				return null;
			}
		}
		#endregion

        // Client Windows --------------------------------------------------------------------
        #region CLIENT WINDOWS
        #region VAttr{g}: bool HasSideWindows
        public bool HasSideWindows
        {
            get
            {
                return (SideWindows.PagesCount > 0);
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

        #region Attr{g}: OWWindow MainWindow - set{} changes it to another OWWindow subclass
        public OWWindow MainWindow
        {
            get
            {
                return m_MainWindow;
            }
            set
            {
                // Don't do anything if we're already active
                if (m_MainWindow == value)
                    return;

                // Hide the current one
                if (m_MainWindow != null)
                    m_MainWindow.Hide();

                // Now set to the new one and show it
                m_MainWindow = value;
                m_MainWindow.Show();

                // Save this to the Registry so we can restore it
                // on OurWord startup
                JW_Registry.SetValue("CurrentJob", MainWindow.Name);

                // Update the contents of all of the windows
                SetupSideWindows();
                ResetWindowContents();
            }
        }
        OWWindow m_MainWindow = null;
        #endregion

        #region Attr{g}: WndDrafting WndDrafting
        WndDrafting WndDrafting
        {
            get
            {
                Debug.Assert(null != m_wndDrafting);
                return m_wndDrafting;
            }
        }
        WndDrafting m_wndDrafting = null;
        #endregion
        #region Attr{g}: WndBackTranslation WndBackTranslation
        WndBackTranslation WndBackTranslation
        {
            get
            {
                Debug.Assert(null != m_wndBackTranslation);
                return m_wndBackTranslation;
            }
        }
        WndBackTranslation m_wndBackTranslation = null;
        #endregion
        #region Attr{g}: WndNaturalness WndNaturalness
        WndNaturalness WndNaturalness
        {
            get
            {
                Debug.Assert(null != m_wndNaturalness);
                return m_wndNaturalness;
            }
        }
        WndNaturalness m_wndNaturalness = null;
        #endregion

        #region VAttr{g}:  bool MainWindowIsDrafting
        public bool MainWindowIsDrafting
        {
            get
            {
                if (MainWindow == WndDrafting)
                    return true;
                return false;
            }
        }
        #endregion
        #region VAttr{g}:  bool MainWindowIsBackTranslation
        public bool MainWindowIsBackTranslation
        {
            get
            {
                if (MainWindow == WndBackTranslation)
                    return true;
                return false;
            }
        }
        #endregion
        #region VAttr{g}:  bool MainWindowIsNaturalness
        public bool MainWindowIsNaturalness
        {
            get
            {
                if (MainWindow == WndNaturalness)
                    return true;
                return false;
            }
        }
        #endregion

        #region VAttr{g}: OWWindow FocusedWindow - the window with current Focus, or null
        OWWindow FocusedWindow
        {
            get
            {
                if (MainWindow.Focused)
                    return MainWindow;
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
            if (MainWindow.Focused)
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
            if (MainWindow.Focused)
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

        #region Method: void CreateClientWindows() - called once, when OW first starts
        void CreateClientWindows()
        {
            // Suspend the layout while we monkey around with the child windows
            SuspendLayout();

            // Create the SideWindows; hide them initially
            m_SideWindows = new SideWindows();
            m_SplitContainer.Panel2.Controls.Add(SideWindows);
            SideWindows.Dock = DockStyle.Fill;

            // Create the Drafting Window 
            m_wndDrafting = new WndDrafting();
            WndDrafting.Show();
            m_SplitContainer.Panel1.Controls.Add(WndDrafting);
            WndDrafting.Dock = DockStyle.Fill;
            m_MainWindow = WndDrafting;

            // Create the Back Translation Window
            m_wndBackTranslation = new WndBackTranslation();
            WndBackTranslation.Hide();
            m_SplitContainer.Panel1.Controls.Add(WndBackTranslation);
            WndBackTranslation.Dock = DockStyle.Fill;

            // Create the Naturalness Window
            m_wndNaturalness = new WndNaturalness();
            m_wndNaturalness.Hide();
            m_SplitContainer.Panel1.Controls.Add(WndNaturalness);
            WndNaturalness.Dock = DockStyle.Fill;

            // Let the window go ahead and proceed with the layout
            ResumeLayout();
        }
        #endregion
        #region Method: void SetupSideWindows() - called whenever different SideWindows are desired (startup, Tools-Options)
        void SetupSideWindows()
        {
            SideWindows.Clear();

            if (DProject.VD_ShowNotesPane)
                SideWindows.CreateNotesWindow();

            if (null != G.Project && G.Project.ShowTranslationsPane && MainWindowIsDrafting)
                SideWindows.CreateTranslationsWindow();

            // Tell the system which side windows are being displayed; thereafter events will be 
            // automatically routed to these windows.
            SideWindows.RegisterWindows(MainWindow);

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

            // If a book is loaded, then display its filename
            if (null != Project && null != G.TBook)
            {
                Text += (" - " + Path.GetFileNameWithoutExtension(G.TBook.AbsolutePathName));
                return;
            }

            // If a project exists, then display its name
			if (null != Project)
				Text += (" - " + Project.DisplayName);
		}
		#endregion
        #region Method: void ResetWindowContents() - called whenever content changes (OnEnterSection)
        void ResetWindowContents()
        {
            // Set the TitleBar text
            SetTitleBarText();

            // Toolbar button enabling
            SetupMenusAndToolbarsVisibility();

            // Do we have a valid project? Can't do much if not.
            if (null == Project || null == Project.SFront || null == Project.STarget)
            {
                MainWindow.Clear();
                MainWindow.Invalidate();
                TaskName = G.GetLoc_GeneralUI("NoProjectDefined", "No Project Defined"); 
                Passage = "";
                ShowPadlock = false;
                if (HasSideWindows)
                    SideWindows.Invalidate();
                return;
            }

            // Is the book locked from editing? Meaning that the user can select and add
            // "ToDo" notes, but nothing else.
            MainWindow.LockedFromEditing = TargetIsLocked;

            // Update the Title Window contents
            TaskName = MainWindow.WindowName;
            Passage = MainWindow.PassageName;
            ShowPadlock = MainWindow.LockedFromEditing;

            // Loading the main window should also load the data in the side windows, as this
            // is generally built as the main window is loaded (or as items there are selected)
            MainWindow.LoadData();

            // Place focus in the main window, so that it is ready for editing
            MainWindow.Focus();
        }
        #endregion
        #region Method: void SetZoomFactor()
        void SetZoomFactor()
        {
            if ( null != MainWindow)
                MainWindow.ZoomFactor = G.ZoomFactor;

            if (null != SideWindows)
                SideWindows.SetZoomFactor(G.ZoomFactor);

            G.StyleSheet.ZoomFactor = G.ZoomFactor;
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

			// No point if we're not showing an active project
			if (null == Project || null == Project.SFront || null == Project.STarget)
				return;

			// Get the active book
			DBook book = Project.STarget.Book;

			// Save it
			if (null != book && book.BookAbbrev.Length > 0 && book.IsDirty)
			{
                book.Write();
				//Console.WriteLine("   --Autosaved " + book.DisplayName);
			}
		}
		#endregion

        // Toolbar, MenuBar, Taskbar & StatusBar ---------------------------------------------
        #region Toolbar, MenuBar, Taskbar & StatusBar
        #region Menu/Toolbar attributes
        private SplitContainer m_SplitContainer;
        private ToolStripContainer m_toolStripContainer;
        private ToolStrip m_ToolStrip;
        private MenuStrip m_MenuStrip;
        private StatusStrip m_StatusStrip;

        private ToolStripMenuItem m_menuProject;
        private ToolStripMenuItem m_menuNew;
        private ToolStripMenuItem m_menuOpen;
        private ToolStripMenuItem m_menuSave;
        private ToolStripMenuItem m_menuSaveAs;
        private ToolStripMenuItem m_menuProperties;
        private ToolStripMenuItem m_menuPrint;
        private ToolStripMenuItem m_menuExit;

        private ToolStripMenuItem m_menuEdit;
        private ToolStripMenuItem m_menuCut;
        private ToolStripMenuItem m_menuCopy;
        private ToolStripMenuItem m_menuPaste;

        private ToolStripMenuItem m_menuNotes;
        private ToolStripMenuItem m_menuNoteGeneral;
        private ToolStripMenuItem m_menuNoteToDo;
        private ToolStripMenuItem m_menuNoteAskUNS;
        private ToolStripMenuItem m_menuNoteDefinition;
        private ToolStripMenuItem m_menuNoteOldVersion;
        private ToolStripMenuItem m_menuNoteReason;
        private ToolStripMenuItem m_menuNoteFrontIssue;
        private ToolStripMenuItem m_menuNoteHintForDaughter;
        private ToolStripMenuItem m_menuNoteBackTranslation;
        private ToolStripSeparator m_menuNotesSeparator2;
        private ToolStripMenuItem m_menuDeleteNote;

        private ToolStripMenuItem m_menuGoTo;
        private ToolStripMenuItem m_menuFirstSection;
        private ToolStripMenuItem m_menuPreviousSection;
        private ToolStripMenuItem m_menuNextSection;
        private ToolStripMenuItem m_menuLastSection;
        private ToolStripMenuItem m_menuGoToBook;

        private ToolStripMenuItem m_menuWindow;
        private ToolStripMenuItem m_menuShowNotesPane;
        private ToolStripMenuItem m_menuShowOtherTranslationsPane;
        private ToolStripSeparator m_menuSeparatorTasks;
        private ToolStripSeparator m_SeparatorTasks;
        private ToolStripMenuItem m_menuDrafting;
        private ToolStripMenuItem m_menuNaturalnessCheck;
        private ToolStripMenuItem m_menuBackTranslation;

        private ToolStripMenuItem m_menuTools;
        private ToolStripMenuItem m_menuIncrementBookStatus;
        private ToolStripMenuItem m_menuRestoreFromBackup;
        private ToolStripMenuItem m_menuOnlyShowSectionsThat;
        private ToolStripMenuItem m_menuCopyBTfromFront;
        private ToolStripMenuItem m_menuEntireBook;
        private ToolStripMenuItem m_menuCurrentSectionOnly;
        private ToolStripMenuItem m_menuSetUpFeatures;
        private ToolStripSeparator m_separatorDebugItems;
        private ToolStripMenuItem m_menuRunDebugTestSuite;
        private ToolStripMenuItem m_menuLocalizerTool;

        private ToolStripMenuItem m_menuHelp;
        private ToolStripMenuItem m_menuHelpTopics;
        private ToolStripMenuItem m_menuAboutOurWord;

        private ToolStripButton m_btnExit;
        private ToolStripButton m_btnProjectSave;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripButton m_btnEditCut;
        private ToolStripButton m_btnEditCopy;
        private ToolStripButton m_btnEditPaste;
        private ToolStripButton m_btnItalic;
        private ToolStripButton m_btnGotoFirstSection;
        private ToolStripSplitButton m_btnGotoPreviousSection;
        private ToolStripSplitButton m_btnGotoNextSection;
        private ToolStripButton m_btnGotoLastSection;
        private ToolStripDropDownButton m_btnGoToBook;
        private ToolStripButton m_btnDrafting;
        private ToolStripButton m_btnBackTranslation;
        private ToolStripButton m_btnNaturalnessCheck;
        private ToolStripDropDownButton m_btnNotes;
        private ToolStripMenuItem m_bmShowNotesPane;
        private ToolStripSeparator m_toolSeparatorNotes;
        private ToolStripButton m_btnNoteGeneral;
        private ToolStripMenuItem m_bmNoteGeneral;
        private ToolStripButton m_btnNoteToDo;
        private ToolStripMenuItem m_bmNoteToDo;
        private ToolStripButton m_btnNoteAskUNS;
        private ToolStripMenuItem m_bmNoteAskUNS;
        private ToolStripButton m_btnNoteDefinition;
        private ToolStripMenuItem m_bmNoteDefinition;
        private ToolStripButton m_btnNoteOldVersion;
        private ToolStripMenuItem m_bmNoteOldVersion;
        private ToolStripButton m_btnNoteReason;
        private ToolStripMenuItem m_bmNoteReason;
        private ToolStripButton m_btnNoteFrontIssue;
        private ToolStripMenuItem m_bmNoteFrontIssue;
        private ToolStripButton m_btnNoteHintForDaughter;
        private ToolStripButton m_btnNoteBackTranslation;
        private ToolStripButton m_btnDeleteNote;
        private ToolStripMenuItem m_bmDeleteNote;

        private ToolStripSeparator m_bmNotesSeparator1;
        private ToolStripSeparator m_bmNotesSeparator2;
        private ToolStripDropDownButton m_bmTools;
        private ToolStripMenuItem m_bmIncrementBookStatus;
        private ToolStripMenuItem m_bmRestoreFromBackup;
        private ToolStripMenuItem m_bmSetUpFeatures;
        private ToolStripSeparator toolStripSeparator7;
        private ToolStripButton m_bmHelp;

        private ToolStripStatusLabel m_StatusMessage1;
        private ToolStripProgressBar m_ProgressBar;
        private ToolStripStatusLabel m_StatusMessage2;

        // Taskbar
        private ToolStrip m_Taskbar;
        private ToolStripLabel m_tbTaskName;
        private ToolStripButton m_tbPadlock;
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
            if (null == G.SFront)
                return;
            int iPos = G.FBook.Sections.FindObj(G.SFront);

            // Loop to populate the Subitems
            ArrayList aPrevious = new ArrayList();
            ArrayList aNext = new ArrayList();

            for (int i = 0; i < G.FBook.Sections.Count; i++)
            {
                // Get the two sections
                DSection SFront = G.FBook.Sections[i] as DSection;
                DSection STarget = G.TBook.Sections[i] as DSection;

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
                string sMenuText = sReference + " - " + sTitle;

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
        #region Method: void EnableMenusAndToolbars()
        public void EnableMenusAndToolbars()
        {
            bool bValidProjectWithData = G.IsValidProject &&
                null != G.SFront && null != G.STarget;

            // Print
            m_menuSave.Enabled = G.IsValidProject;
            m_menuSaveAs.Enabled = G.IsValidProject;
            m_menuPrint.Enabled = bValidProjectWithData;

            // Editing
            bool bCanEdit = (MainWindow.Focused && TargetIsLocked) ? false : true;
            m_menuCut.Enabled = bCanEdit;
            m_btnEditCut.Enabled = bCanEdit;
            m_menuPaste.Enabled = bCanEdit;
            m_btnEditPaste.Enabled = bCanEdit;
            m_btnItalic.Enabled = canItalic;

            // Notes
            bool bCanInsertNote = canInsertNote;
            bool bCanDeleteNote = canDeleteNote;
            m_menuNoteGeneral.Enabled = bCanInsertNote;
            m_btnNoteGeneral.Enabled = m_menuNoteGeneral.Enabled;
            m_bmNoteGeneral.Enabled = bCanInsertNote;
            m_menuNoteToDo.Enabled = bCanInsertNote;
            m_btnNoteToDo.Enabled = m_menuNoteToDo.Enabled;
            m_bmNoteToDo.Enabled = bCanInsertNote;
            m_menuNoteAskUNS.Enabled = bCanInsertNote;
            m_btnNoteAskUNS.Enabled = m_menuNoteAskUNS.Enabled;
            m_bmNoteAskUNS.Enabled = bCanInsertNote;
            m_menuNoteDefinition.Enabled = bCanInsertNote;
            m_btnNoteDefinition.Enabled = m_menuNoteDefinition.Enabled;
            m_bmNoteDefinition.Enabled = bCanInsertNote;
            m_menuNoteOldVersion.Enabled = bCanInsertNote;
            m_btnNoteOldVersion.Enabled = m_menuNoteOldVersion.Enabled;
            m_bmNoteOldVersion.Enabled = bCanInsertNote;
            m_menuNoteReason.Enabled = bCanInsertNote;
            m_btnNoteReason.Enabled = m_menuNoteReason.Enabled;
            m_bmNoteReason.Enabled = bCanInsertNote;
            m_menuNoteFrontIssue.Enabled = bCanInsertNote;
            m_btnNoteFrontIssue.Enabled = m_menuNoteFrontIssue.Enabled;
            m_bmNoteFrontIssue.Enabled = bCanInsertNote;
            m_menuNoteHintForDaughter.Enabled = bCanInsertNote;
            m_btnNoteHintForDaughter.Enabled = m_menuNoteHintForDaughter.Enabled;
            m_menuNoteBackTranslation.Enabled = bCanInsertNote;
            m_btnNoteBackTranslation.Enabled = m_menuNoteBackTranslation.Enabled;
            m_menuDeleteNote.Enabled = bCanDeleteNote;
            m_bmDeleteNote.Enabled = bCanDeleteNote;

            // Go To menu
            bool bIsAtFirstSection = bValidProjectWithData && Project.Nav.IsAtFirstSection;
            m_menuFirstSection.Enabled = bValidProjectWithData && !bIsAtFirstSection;
            m_btnGotoFirstSection.Enabled = bValidProjectWithData && !bIsAtFirstSection;
            m_menuPreviousSection.Enabled = bValidProjectWithData && !bIsAtFirstSection;
            m_btnGotoPreviousSection.Enabled = bValidProjectWithData && !bIsAtFirstSection;
            bool bIsAtLastSection = bValidProjectWithData && Project.Nav.IsAtLastSection;
            m_menuNextSection.Enabled = bValidProjectWithData && !bIsAtLastSection;
            m_btnGotoNextSection.Enabled = bValidProjectWithData && !bIsAtLastSection;
            m_menuLastSection.Enabled = bValidProjectWithData && !bIsAtLastSection;
            m_btnGotoLastSection.Enabled = bValidProjectWithData && !bIsAtLastSection;
            m_menuGoToBook.Enabled = G.IsValidProject;

            // Tools menu
            m_menuIncrementBookStatus.Enabled = canIncrementBookStatus;

            // Window menu
            m_menuDrafting.Checked = MainWindowIsDrafting;
            m_menuNaturalnessCheck.Checked = MainWindowIsNaturalness;
            m_menuBackTranslation.Checked = MainWindowIsBackTranslation;
            m_btnDrafting.Checked = MainWindowIsDrafting;
            m_btnNaturalnessCheck.Checked = MainWindowIsNaturalness;
            m_btnBackTranslation.Checked = MainWindowIsBackTranslation;
        }
        #endregion
        #region Method: void SetupMenusAndToolbarsVisibility()
        void SetupMenusAndToolbarsVisibility()
        {
            // JustTheBasics mode
            bool bJustTheBasics = OurWordMain.Features.F_JustTheBasics && G.IsValidProject;
            m_MenuStrip.Visible = !bJustTheBasics;
            foreach (ToolStripItem item in m_ToolStrip.Items)
            {
                if (item as ToolStripItem != null)
                {
                    (item as ToolStripItem).DisplayStyle = bJustTheBasics ?
                        ToolStripItemDisplayStyle.ImageAndText :
                        ToolStripItemDisplayStyle.Image;
                }
            }

            // NewOpenEtc Visibility - if we have a valid project, we can choose to turn off 
            //   the menu items. If we don't have a valid project; then we MUST show the menu 
            //   items so that we can create a valid one.
            bool bShowNewOpenEtc = (!G.IsValidProject || OurWordMain.Features.F_Project);
            m_menuNew.Visible = bShowNewOpenEtc;
            m_menuOpen.Visible = bShowNewOpenEtc;
            m_menuSaveAs.Visible = bShowNewOpenEtc;
            m_btnExit.Visible = bJustTheBasics;

            // ProjectPropertiesDialog Visibility - Same logic asNewOpenEtc. If we have a valid
            //   project, then it is ok for this to be turned off.
            bool bShowPropertiesDlg = (!G.IsValidProject || OurWordMain.Features.F_PropertiesDialog);
            m_menuProperties.Visible = bShowPropertiesDlg;

            // Print Dialog Visibility
            m_menuPrint.Visible = OurWordMain.Features.F_Print;

            // Notes
            #region (Notes Visibility)
            bool bNotesPaneIsVisible = DProject.VD_ShowNotesPane;
            m_bmShowNotesPane.Checked = bNotesPaneIsVisible;

            m_menuNotes.Visible = bNotesPaneIsVisible;
            m_bmNotesSeparator1.Visible = bNotesPaneIsVisible;

            m_menuNoteGeneral.Visible = (bNotesPaneIsVisible && DNote.ShowGeneral);
            m_btnNoteGeneral.Visible = (!bJustTheBasics && bNotesPaneIsVisible && DNote.ShowGeneral);
            m_bmNoteGeneral.Visible = (bJustTheBasics && bNotesPaneIsVisible && DNote.ShowGeneral);

            m_menuNoteToDo.Visible = (bNotesPaneIsVisible && DNote.ShowToDo);
            m_btnNoteToDo.Visible = (!bJustTheBasics && bNotesPaneIsVisible && DNote.ShowToDo);
            m_bmNoteToDo.Visible = (bJustTheBasics && bNotesPaneIsVisible && DNote.ShowToDo);

            m_menuNoteAskUNS.Visible = (bNotesPaneIsVisible && DNote.ShowAskUns);
            m_btnNoteAskUNS.Visible = (!bJustTheBasics && bNotesPaneIsVisible && DNote.ShowAskUns);
            m_bmNoteAskUNS.Visible = (bJustTheBasics && bNotesPaneIsVisible && DNote.ShowAskUns);

            m_menuNoteDefinition.Visible = (bNotesPaneIsVisible && DNote.ShowDefinition);
            m_btnNoteDefinition.Visible = (!bJustTheBasics && bNotesPaneIsVisible && DNote.ShowDefinition);
            m_bmNoteDefinition.Visible = (bJustTheBasics && bNotesPaneIsVisible && DNote.ShowDefinition);

            m_menuNoteOldVersion.Visible = (bNotesPaneIsVisible && DNote.ShowOldVersion);
            m_btnNoteOldVersion.Visible = (!bJustTheBasics && bNotesPaneIsVisible && DNote.ShowOldVersion);
            m_bmNoteOldVersion.Visible = (bJustTheBasics && bNotesPaneIsVisible && DNote.ShowOldVersion);

            m_menuNoteReason.Visible = (bNotesPaneIsVisible && DNote.ShowReason);
            m_btnNoteReason.Visible = (!bJustTheBasics && bNotesPaneIsVisible && DNote.ShowReason);
            m_bmNoteReason.Visible = (bJustTheBasics && bNotesPaneIsVisible && DNote.ShowReason);

            m_menuNoteFrontIssue.Visible = (bNotesPaneIsVisible && DNote.ShowFront);
            m_btnNoteFrontIssue.Visible = (!bJustTheBasics && bNotesPaneIsVisible && DNote.ShowFront);
            m_bmNoteFrontIssue.Visible = (bJustTheBasics && bNotesPaneIsVisible && DNote.ShowFront);

            m_menuNoteHintForDaughter.Visible = (!bJustTheBasics && bNotesPaneIsVisible && DNote.ShowHintForDaughter);
            m_btnNoteHintForDaughter.Visible = m_menuNoteHintForDaughter.Visible;

            m_menuNoteBackTranslation.Visible = (!bJustTheBasics && bNotesPaneIsVisible && DNote.ShowBT && 
                OurWordMain.App.MainWindowIsBackTranslation);
            m_btnNoteBackTranslation.Visible = m_menuNoteBackTranslation.Visible;

            m_menuNotesSeparator2.Visible = bNotesPaneIsVisible;
            m_bmNotesSeparator2.Visible = bNotesPaneIsVisible;

            m_menuDeleteNote.Visible = bNotesPaneIsVisible;
            m_btnDeleteNote.Visible = bNotesPaneIsVisible && !bJustTheBasics;
            m_bmDeleteNote.Visible = bJustTheBasics && bNotesPaneIsVisible;

            m_btnNotes.Visible = bJustTheBasics;
            #endregion

            // GoTo Menu
            m_btnGotoFirstSection.Visible = !bJustTheBasics;
            m_btnGotoLastSection.Visible = !bJustTheBasics;
            m_btnGoToBook.Visible = bJustTheBasics;

            // Window Menu (show these if more than Drafting is turned on)
            m_menuShowNotesPane.Checked = bNotesPaneIsVisible;
            m_menuShowOtherTranslationsPane.Visible = (G.IsValidProject &&
                (G.Project.OtherTranslations.Count > 0));
            m_menuShowOtherTranslationsPane.Checked = (
                G.IsValidProject &&
                (G.Project.OtherTranslations.Count > 0) &&
                DProject.VD_ShowTranslationsPane);
            bool bShowTasks = s_Features.F_JobBT || s_Features.F_JobNaturalness;
            m_menuSeparatorTasks.Visible = bShowTasks;
            m_SeparatorTasks.Visible = bShowTasks;
            m_menuDrafting.Visible = bShowTasks;
            m_btnDrafting.Visible = bShowTasks;
            m_menuNaturalnessCheck.Visible = (bShowTasks && s_Features.F_JobNaturalness);
            m_btnNaturalnessCheck.Visible = (bShowTasks && s_Features.F_JobNaturalness);
            m_menuBackTranslation.Visible = (bShowTasks && s_Features.F_JobBT);
            m_btnBackTranslation.Visible = (bShowTasks && s_Features.F_JobBT);

            // Tools
            m_bmTools.Visible = bJustTheBasics;
            m_menuRestoreFromBackup.Visible = OurWordMain.Features.F_RestoreBackup;
            m_bmRestoreFromBackup.Visible = OurWordMain.Features.F_RestoreBackup;
            m_menuOnlyShowSectionsThat.Visible = (G.IsValidProject && OurWordMain.Features.F_Filter);
            m_menuCopyBTfromFront.Visible = OurWordMain.Features.F_CopyBTfromFront;
            m_menuLocalizerTool.Visible = OurWordMain.Features.F_Localizer;

            // Debugging
            bool bShowDebugItems = JW_Registry.GetValue("Debug", false);
            m_separatorDebugItems.Visible = bShowDebugItems;
            m_menuRunDebugTestSuite.Visible = bShowDebugItems;

            // Clear dropdown subitems so we don't attempt to localize them
            m_Config.RemoveMRUItems(m_menuProject);
            m_btnGotoPreviousSection.DropDownItems.Clear();
            m_btnGotoNextSection.DropDownItems.Clear();
            m_menuGoToBook.DropDownItems.Clear();
            m_btnGoToBook.DropDownItems.Clear();

            // Localization
            LocDB.Localize(m_MenuStrip); //, "Menus");
            LocDB.Localize(m_ToolStrip); //, "ToolbarText");

            // Some submenus happen after localization (otherwise, we'd be adding spurious
            // entries into the localization database)
            m_Config.BuildMRUPopupMenu(m_menuProject, cmdMRU, bShowNewOpenEtc);
            SetupNavigationButtons();
            DBookGrouping.PopulateGotoBook(m_menuGoToBook, cmdGotoBook);
            DBookGrouping.PopulateGotoBook(m_btnGoToBook, cmdGotoBook);

            // Enabling depends on the current editing context
            EnableMenusAndToolbars();
        }
        #endregion
        // Status Bar
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
        #region Attr{g}: string TaskName - e.g., "Drafting", "Back Translation"
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
        #region Attr{g}: string Passage - e.g., "- Kupang to AMARASSI John 3:16"
        public string Passage
        {
            set
            {
                m_tbCurrentPassage.Text = " - " + value;
            }
        }
        #endregion
        #region Attr{g}: bool ShowPAdlock - true if the book is locked for editing
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
		private JW_FileMenuIO   m_Config;	     // Handles I/O of the configuration (project) file

		// Scaffolding -----------------------------------------------------------------------
		#region Method: void ShowLoadState(string s)
		static public void ShowLoadState(string s)
		{
			bool bShow = false;
			if (bShow)
			{
				SplashScreen.SetStatus(s);
				Console.WriteLine(s);
			}
		}
		#endregion
		#region Constructor()
		public OurWordMain()
			// Constructor, initializes the application
		{
			// Required for Windows Form Designer support
			ShowLoadState("Init Component");
            this.components = new System.ComponentModel.Container();
			InitializeComponent();

            // Initialize the Client Window (do now, to establish proper z-order)
            ShowLoadState("Init Client Windows");
            CreateClientWindows();
            SetupSideWindows();

			// Initialize the window state mechanism. We'll default to a full screen
			// the first time we are launched.
			ShowLoadState("Init Window State");
			m_WindowState = new JW_WindowState(this, true);

			// Initialize the features we will make available
			ShowLoadState("Init Features Mgr");
			s_Features = new FeaturesMgr();

			// Initialize the Project File Configuration system
			ShowLoadState("Init Configuration System");
			m_Config = new JW_FileMenuIO(this, this,
				LanguageResources.AppTitle,
                G.GetLoc_Files("ProjectFileFilter", "Our Word! Project Files (*.owp)|*.owp"), 
				"owp",
				G.GetLoc_Files("DefaultProjectFileName","New Project.owp")); 

			// Initialize to a blank project (if there is a recent project
			// in the MRU, this will get overridden.)
			ShowLoadState("Init new project");
			s_project = new DProject();
			ShowLoadState("Construction Complete");
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
            System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
            System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
            System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
            System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OurWordMain));
            this.m_menuHelp = new System.Windows.Forms.ToolStripMenuItem();
            this.m_menuHelpTopics = new System.Windows.Forms.ToolStripMenuItem();
            this.m_menuAboutOurWord = new System.Windows.Forms.ToolStripMenuItem();
            this.m_menuEdit = new System.Windows.Forms.ToolStripMenuItem();
            this.m_menuCut = new System.Windows.Forms.ToolStripMenuItem();
            this.m_menuCopy = new System.Windows.Forms.ToolStripMenuItem();
            this.m_menuPaste = new System.Windows.Forms.ToolStripMenuItem();
            this.m_menuGoTo = new System.Windows.Forms.ToolStripMenuItem();
            this.m_menuFirstSection = new System.Windows.Forms.ToolStripMenuItem();
            this.m_menuPreviousSection = new System.Windows.Forms.ToolStripMenuItem();
            this.m_menuNextSection = new System.Windows.Forms.ToolStripMenuItem();
            this.m_menuLastSection = new System.Windows.Forms.ToolStripMenuItem();
            this.m_menuGoToBook = new System.Windows.Forms.ToolStripMenuItem();
            this.m_menuSave = new System.Windows.Forms.ToolStripMenuItem();
            this.m_MenuStrip = new System.Windows.Forms.MenuStrip();
            this.m_menuProject = new System.Windows.Forms.ToolStripMenuItem();
            this.m_menuNew = new System.Windows.Forms.ToolStripMenuItem();
            this.m_menuOpen = new System.Windows.Forms.ToolStripMenuItem();
            this.m_menuSaveAs = new System.Windows.Forms.ToolStripMenuItem();
            this.m_menuProperties = new System.Windows.Forms.ToolStripMenuItem();
            this.m_menuPrint = new System.Windows.Forms.ToolStripMenuItem();
            this.m_menuExit = new System.Windows.Forms.ToolStripMenuItem();
            this.m_menuNotes = new System.Windows.Forms.ToolStripMenuItem();
            this.m_menuNoteGeneral = new System.Windows.Forms.ToolStripMenuItem();
            this.m_menuNoteToDo = new System.Windows.Forms.ToolStripMenuItem();
            this.m_menuNoteAskUNS = new System.Windows.Forms.ToolStripMenuItem();
            this.m_menuNoteDefinition = new System.Windows.Forms.ToolStripMenuItem();
            this.m_menuNoteOldVersion = new System.Windows.Forms.ToolStripMenuItem();
            this.m_menuNoteReason = new System.Windows.Forms.ToolStripMenuItem();
            this.m_menuNoteFrontIssue = new System.Windows.Forms.ToolStripMenuItem();
            this.m_menuNoteHintForDaughter = new System.Windows.Forms.ToolStripMenuItem();
            this.m_menuNoteBackTranslation = new System.Windows.Forms.ToolStripMenuItem();
            this.m_menuNotesSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.m_menuDeleteNote = new System.Windows.Forms.ToolStripMenuItem();
            this.m_menuTools = new System.Windows.Forms.ToolStripMenuItem();
            this.m_menuIncrementBookStatus = new System.Windows.Forms.ToolStripMenuItem();
            this.m_menuRestoreFromBackup = new System.Windows.Forms.ToolStripMenuItem();
            this.m_menuOnlyShowSectionsThat = new System.Windows.Forms.ToolStripMenuItem();
            this.m_menuCopyBTfromFront = new System.Windows.Forms.ToolStripMenuItem();
            this.m_menuEntireBook = new System.Windows.Forms.ToolStripMenuItem();
            this.m_menuCurrentSectionOnly = new System.Windows.Forms.ToolStripMenuItem();
            this.m_menuSetUpFeatures = new System.Windows.Forms.ToolStripMenuItem();
            this.m_separatorDebugItems = new System.Windows.Forms.ToolStripSeparator();
            this.m_menuRunDebugTestSuite = new System.Windows.Forms.ToolStripMenuItem();
            this.m_menuLocalizerTool = new System.Windows.Forms.ToolStripMenuItem();
            this.m_menuWindow = new System.Windows.Forms.ToolStripMenuItem();
            this.m_menuShowNotesPane = new System.Windows.Forms.ToolStripMenuItem();
            this.m_menuShowOtherTranslationsPane = new System.Windows.Forms.ToolStripMenuItem();
            this.m_menuSeparatorTasks = new System.Windows.Forms.ToolStripSeparator();
            this.m_menuDrafting = new System.Windows.Forms.ToolStripMenuItem();
            this.m_menuNaturalnessCheck = new System.Windows.Forms.ToolStripMenuItem();
            this.m_menuBackTranslation = new System.Windows.Forms.ToolStripMenuItem();
            this.m_ToolStrip = new System.Windows.Forms.ToolStrip();
            this.m_btnExit = new System.Windows.Forms.ToolStripButton();
            this.m_btnProjectSave = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.m_btnEditCut = new System.Windows.Forms.ToolStripButton();
            this.m_btnEditCopy = new System.Windows.Forms.ToolStripButton();
            this.m_btnEditPaste = new System.Windows.Forms.ToolStripButton();
            this.m_btnItalic = new System.Windows.Forms.ToolStripButton();
            this.m_btnGotoFirstSection = new System.Windows.Forms.ToolStripButton();
            this.m_btnGotoPreviousSection = new System.Windows.Forms.ToolStripSplitButton();
            this.m_btnGotoNextSection = new System.Windows.Forms.ToolStripSplitButton();
            this.m_btnGotoLastSection = new System.Windows.Forms.ToolStripButton();
            this.m_btnGoToBook = new System.Windows.Forms.ToolStripDropDownButton();
            this.m_SeparatorTasks = new System.Windows.Forms.ToolStripSeparator();
            this.m_btnDrafting = new System.Windows.Forms.ToolStripButton();
            this.m_btnBackTranslation = new System.Windows.Forms.ToolStripButton();
            this.m_btnNaturalnessCheck = new System.Windows.Forms.ToolStripButton();
            this.m_toolSeparatorNotes = new System.Windows.Forms.ToolStripSeparator();
            this.m_btnNotes = new System.Windows.Forms.ToolStripDropDownButton();
            this.m_bmShowNotesPane = new System.Windows.Forms.ToolStripMenuItem();
            this.m_bmNotesSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.m_bmNoteGeneral = new System.Windows.Forms.ToolStripMenuItem();
            this.m_bmNoteToDo = new System.Windows.Forms.ToolStripMenuItem();
            this.m_bmNoteAskUNS = new System.Windows.Forms.ToolStripMenuItem();
            this.m_bmNoteDefinition = new System.Windows.Forms.ToolStripMenuItem();
            this.m_bmNoteOldVersion = new System.Windows.Forms.ToolStripMenuItem();
            this.m_bmNoteReason = new System.Windows.Forms.ToolStripMenuItem();
            this.m_bmNoteFrontIssue = new System.Windows.Forms.ToolStripMenuItem();
            this.m_bmNotesSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.m_bmDeleteNote = new System.Windows.Forms.ToolStripMenuItem();
            this.m_btnNoteGeneral = new System.Windows.Forms.ToolStripButton();
            this.m_btnNoteToDo = new System.Windows.Forms.ToolStripButton();
            this.m_btnNoteAskUNS = new System.Windows.Forms.ToolStripButton();
            this.m_btnNoteDefinition = new System.Windows.Forms.ToolStripButton();
            this.m_btnNoteOldVersion = new System.Windows.Forms.ToolStripButton();
            this.m_btnNoteReason = new System.Windows.Forms.ToolStripButton();
            this.m_btnNoteFrontIssue = new System.Windows.Forms.ToolStripButton();
            this.m_btnNoteHintForDaughter = new System.Windows.Forms.ToolStripButton();
            this.m_btnNoteBackTranslation = new System.Windows.Forms.ToolStripButton();
            this.m_btnDeleteNote = new System.Windows.Forms.ToolStripButton();
            this.m_bmTools = new System.Windows.Forms.ToolStripDropDownButton();
            this.m_bmIncrementBookStatus = new System.Windows.Forms.ToolStripMenuItem();
            this.m_bmRestoreFromBackup = new System.Windows.Forms.ToolStripMenuItem();
            this.m_bmSetUpFeatures = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
            this.m_bmHelp = new System.Windows.Forms.ToolStripButton();
            this.m_toolStripContainer = new System.Windows.Forms.ToolStripContainer();
            this.m_StatusStrip = new System.Windows.Forms.StatusStrip();
            this.m_StatusMessage1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.m_ProgressBar = new System.Windows.Forms.ToolStripProgressBar();
            this.m_StatusMessage2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.m_SplitContainer = new System.Windows.Forms.SplitContainer();
            this.m_Taskbar = new System.Windows.Forms.ToolStrip();
            this.m_tbTaskName = new System.Windows.Forms.ToolStripLabel();
            this.m_tbPadlock = new System.Windows.Forms.ToolStripButton();
            this.m_tbCurrentPassage = new System.Windows.Forms.ToolStripLabel();
            toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.m_MenuStrip.SuspendLayout();
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
            // toolStripSeparator3
            // 
            toolStripSeparator3.Name = "toolStripSeparator3";
            toolStripSeparator3.Size = new System.Drawing.Size(143, 6);
            // 
            // toolStripSeparator4
            // 
            toolStripSeparator4.Name = "toolStripSeparator4";
            toolStripSeparator4.Size = new System.Drawing.Size(143, 6);
            // 
            // toolStripSeparator5
            // 
            toolStripSeparator5.Name = "toolStripSeparator5";
            toolStripSeparator5.Size = new System.Drawing.Size(199, 6);
            // 
            // toolStripSeparator2
            // 
            toolStripSeparator2.Name = "toolStripSeparator2";
            toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // m_menuHelp
            // 
            this.m_menuHelp.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_menuHelpTopics,
            this.m_menuAboutOurWord});
            this.m_menuHelp.Name = "m_menuHelp";
            this.m_menuHelp.Size = new System.Drawing.Size(44, 20);
            this.m_menuHelp.Text = "&Help";
            // 
            // m_menuHelpTopics
            // 
            this.m_menuHelpTopics.Image = ((System.Drawing.Image)(resources.GetObject("m_menuHelpTopics.Image")));
            this.m_menuHelpTopics.Name = "m_menuHelpTopics";
            this.m_menuHelpTopics.Size = new System.Drawing.Size(162, 22);
            this.m_menuHelpTopics.Text = "Help &Topics";
            this.m_menuHelpTopics.Click += new System.EventHandler(this.cmdHelpTopics);
            // 
            // m_menuAboutOurWord
            // 
            this.m_menuAboutOurWord.Name = "m_menuAboutOurWord";
            this.m_menuAboutOurWord.Size = new System.Drawing.Size(162, 22);
            this.m_menuAboutOurWord.Text = "&About Our Word";
            this.m_menuAboutOurWord.Click += new System.EventHandler(this.cmdHelpAbout);
            // 
            // m_menuEdit
            // 
            this.m_menuEdit.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_menuCut,
            this.m_menuCopy,
            this.m_menuPaste});
            this.m_menuEdit.Name = "m_menuEdit";
            this.m_menuEdit.Size = new System.Drawing.Size(39, 20);
            this.m_menuEdit.Text = "&Edit";
            // 
            // m_menuCut
            // 
            this.m_menuCut.Image = ((System.Drawing.Image)(resources.GetObject("m_menuCut.Image")));
            this.m_menuCut.Name = "m_menuCut";
            this.m_menuCut.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.X)));
            this.m_menuCut.Size = new System.Drawing.Size(144, 22);
            this.m_menuCut.Text = "Cu&t";
            this.m_menuCut.Click += new System.EventHandler(this.cmdEditCut);
            // 
            // m_menuCopy
            // 
            this.m_menuCopy.Image = ((System.Drawing.Image)(resources.GetObject("m_menuCopy.Image")));
            this.m_menuCopy.Name = "m_menuCopy";
            this.m_menuCopy.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.m_menuCopy.Size = new System.Drawing.Size(144, 22);
            this.m_menuCopy.Text = "&Copy";
            this.m_menuCopy.Click += new System.EventHandler(this.cmdEditCopy);
            // 
            // m_menuPaste
            // 
            this.m_menuPaste.Image = ((System.Drawing.Image)(resources.GetObject("m_menuPaste.Image")));
            this.m_menuPaste.Name = "m_menuPaste";
            this.m_menuPaste.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V)));
            this.m_menuPaste.Size = new System.Drawing.Size(144, 22);
            this.m_menuPaste.Text = "&Paste";
            this.m_menuPaste.Click += new System.EventHandler(this.cmdEditPaste);
            // 
            // m_menuGoTo
            // 
            this.m_menuGoTo.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_menuFirstSection,
            this.m_menuPreviousSection,
            this.m_menuNextSection,
            this.m_menuLastSection,
            toolStripSeparator5,
            this.m_menuGoToBook});
            this.m_menuGoTo.Name = "m_menuGoTo";
            this.m_menuGoTo.Size = new System.Drawing.Size(51, 20);
            this.m_menuGoTo.Text = "&Go To";
            // 
            // m_menuFirstSection
            // 
            this.m_menuFirstSection.Image = ((System.Drawing.Image)(resources.GetObject("m_menuFirstSection.Image")));
            this.m_menuFirstSection.Name = "m_menuFirstSection";
            this.m_menuFirstSection.Size = new System.Drawing.Size(202, 22);
            this.m_menuFirstSection.Text = "&First Section";
            this.m_menuFirstSection.Click += new System.EventHandler(this.cmdGoToFirstSection);
            // 
            // m_menuPreviousSection
            // 
            this.m_menuPreviousSection.Image = ((System.Drawing.Image)(resources.GetObject("m_menuPreviousSection.Image")));
            this.m_menuPreviousSection.Name = "m_menuPreviousSection";
            this.m_menuPreviousSection.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.P)));
            this.m_menuPreviousSection.Size = new System.Drawing.Size(202, 22);
            this.m_menuPreviousSection.Text = "&Previous Section";
            this.m_menuPreviousSection.Click += new System.EventHandler(this.cmdGoToPreviousSection);
            // 
            // m_menuNextSection
            // 
            this.m_menuNextSection.Image = ((System.Drawing.Image)(resources.GetObject("m_menuNextSection.Image")));
            this.m_menuNextSection.Name = "m_menuNextSection";
            this.m_menuNextSection.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.m_menuNextSection.Size = new System.Drawing.Size(202, 22);
            this.m_menuNextSection.Text = "&Next Section";
            this.m_menuNextSection.Click += new System.EventHandler(this.cmdGoToNextSection);
            // 
            // m_menuLastSection
            // 
            this.m_menuLastSection.Image = ((System.Drawing.Image)(resources.GetObject("m_menuLastSection.Image")));
            this.m_menuLastSection.Name = "m_menuLastSection";
            this.m_menuLastSection.Size = new System.Drawing.Size(202, 22);
            this.m_menuLastSection.Text = "&Last Section";
            this.m_menuLastSection.Click += new System.EventHandler(this.cmdGoToLastSection);
            // 
            // m_menuGoToBook
            // 
            this.m_menuGoToBook.Image = ((System.Drawing.Image)(resources.GetObject("m_menuGoToBook.Image")));
            this.m_menuGoToBook.Name = "m_menuGoToBook";
            this.m_menuGoToBook.Size = new System.Drawing.Size(202, 22);
            this.m_menuGoToBook.Text = "&Book";
            // 
            // m_menuSave
            // 
            this.m_menuSave.Image = ((System.Drawing.Image)(resources.GetObject("m_menuSave.Image")));
            this.m_menuSave.Name = "m_menuSave";
            this.m_menuSave.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.m_menuSave.Size = new System.Drawing.Size(146, 22);
            this.m_menuSave.Text = "&Save";
            this.m_menuSave.Click += new System.EventHandler(this.cmdSaveProject);
            // 
            // m_MenuStrip
            // 
            this.m_MenuStrip.Dock = System.Windows.Forms.DockStyle.None;
            this.m_MenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_menuProject,
            this.m_menuEdit,
            this.m_menuNotes,
            this.m_menuGoTo,
            this.m_menuTools,
            this.m_menuWindow,
            this.m_menuHelp});
            this.m_MenuStrip.Location = new System.Drawing.Point(0, 0);
            this.m_MenuStrip.Name = "m_MenuStrip";
            this.m_MenuStrip.Size = new System.Drawing.Size(748, 24);
            this.m_MenuStrip.TabIndex = 0;
            this.m_MenuStrip.Text = "menuStrip1";
            // 
            // m_menuProject
            // 
            this.m_menuProject.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_menuNew,
            this.m_menuOpen,
            this.m_menuSave,
            this.m_menuSaveAs,
            toolStripSeparator3,
            this.m_menuProperties,
            this.m_menuPrint,
            toolStripSeparator4,
            this.m_menuExit});
            this.m_menuProject.Name = "m_menuProject";
            this.m_menuProject.Size = new System.Drawing.Size(56, 20);
            this.m_menuProject.Text = "&Project";
            // 
            // m_menuNew
            // 
            this.m_menuNew.Image = ((System.Drawing.Image)(resources.GetObject("m_menuNew.Image")));
            this.m_menuNew.Name = "m_menuNew";
            this.m_menuNew.Size = new System.Drawing.Size(146, 22);
            this.m_menuNew.Text = "&New";
            this.m_menuNew.Click += new System.EventHandler(this.cmdNewProject);
            // 
            // m_menuOpen
            // 
            this.m_menuOpen.Image = ((System.Drawing.Image)(resources.GetObject("m_menuOpen.Image")));
            this.m_menuOpen.Name = "m_menuOpen";
            this.m_menuOpen.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.m_menuOpen.Size = new System.Drawing.Size(146, 22);
            this.m_menuOpen.Text = "&Open";
            this.m_menuOpen.Click += new System.EventHandler(this.cmdOpenProject);
            // 
            // m_menuSaveAs
            // 
            this.m_menuSaveAs.Name = "m_menuSaveAs";
            this.m_menuSaveAs.Size = new System.Drawing.Size(146, 22);
            this.m_menuSaveAs.Text = "Save &As";
            this.m_menuSaveAs.Click += new System.EventHandler(this.cmdSaveProjectAs);
            // 
            // m_menuProperties
            // 
            this.m_menuProperties.Image = ((System.Drawing.Image)(resources.GetObject("m_menuProperties.Image")));
            this.m_menuProperties.Name = "m_menuProperties";
            this.m_menuProperties.Size = new System.Drawing.Size(146, 22);
            this.m_menuProperties.Text = "P&roperties";
            this.m_menuProperties.Click += new System.EventHandler(this.cmdProjectProperties);
            // 
            // m_menuPrint
            // 
            this.m_menuPrint.Image = ((System.Drawing.Image)(resources.GetObject("m_menuPrint.Image")));
            this.m_menuPrint.Name = "m_menuPrint";
            this.m_menuPrint.Size = new System.Drawing.Size(146, 22);
            this.m_menuPrint.Text = "&Print";
            this.m_menuPrint.Click += new System.EventHandler(this.cmdPrint);
            // 
            // m_menuExit
            // 
            this.m_menuExit.Image = ((System.Drawing.Image)(resources.GetObject("m_menuExit.Image")));
            this.m_menuExit.Name = "m_menuExit";
            this.m_menuExit.Size = new System.Drawing.Size(146, 22);
            this.m_menuExit.Text = "E&xit";
            this.m_menuExit.Click += new System.EventHandler(this.cmdExit);
            // 
            // m_menuNotes
            // 
            this.m_menuNotes.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_menuNoteGeneral,
            this.m_menuNoteToDo,
            this.m_menuNoteAskUNS,
            this.m_menuNoteDefinition,
            this.m_menuNoteOldVersion,
            this.m_menuNoteReason,
            this.m_menuNoteFrontIssue,
            this.m_menuNoteHintForDaughter,
            this.m_menuNoteBackTranslation,
            this.m_menuNotesSeparator2,
            this.m_menuDeleteNote});
            this.m_menuNotes.Name = "m_menuNotes";
            this.m_menuNotes.Size = new System.Drawing.Size(50, 20);
            this.m_menuNotes.Text = "&Notes";
            // 
            // m_menuNoteGeneral
            // 
            this.m_menuNoteGeneral.Image = ((System.Drawing.Image)(resources.GetObject("m_menuNoteGeneral.Image")));
            this.m_menuNoteGeneral.Name = "m_menuNoteGeneral";
            this.m_menuNoteGeneral.Size = new System.Drawing.Size(228, 22);
            this.m_menuNoteGeneral.Tag = "kGeneral";
            this.m_menuNoteGeneral.Text = "Insert General &Note";
            this.m_menuNoteGeneral.Click += new System.EventHandler(this.cmdInsertNote);
            // 
            // m_menuNoteToDo
            // 
            this.m_menuNoteToDo.Image = ((System.Drawing.Image)(resources.GetObject("m_menuNoteToDo.Image")));
            this.m_menuNoteToDo.Name = "m_menuNoteToDo";
            this.m_menuNoteToDo.Size = new System.Drawing.Size(228, 22);
            this.m_menuNoteToDo.Tag = "kToDo";
            this.m_menuNoteToDo.Text = "Insert &To Do Note";
            this.m_menuNoteToDo.Click += new System.EventHandler(this.cmdInsertNote);
            // 
            // m_menuNoteAskUNS
            // 
            this.m_menuNoteAskUNS.Image = ((System.Drawing.Image)(resources.GetObject("m_menuNoteAskUNS.Image")));
            this.m_menuNoteAskUNS.Name = "m_menuNoteAskUNS";
            this.m_menuNoteAskUNS.Size = new System.Drawing.Size(228, 22);
            this.m_menuNoteAskUNS.Tag = "kAskUns";
            this.m_menuNoteAskUNS.Text = "Insert &Ask UNS Note";
            this.m_menuNoteAskUNS.Click += new System.EventHandler(this.cmdInsertNote);
            // 
            // m_menuNoteDefinition
            // 
            this.m_menuNoteDefinition.Image = ((System.Drawing.Image)(resources.GetObject("m_menuNoteDefinition.Image")));
            this.m_menuNoteDefinition.Name = "m_menuNoteDefinition";
            this.m_menuNoteDefinition.Size = new System.Drawing.Size(228, 22);
            this.m_menuNoteDefinition.Tag = "kDefinition";
            this.m_menuNoteDefinition.Text = "Insert D&efinition Note";
            this.m_menuNoteDefinition.Click += new System.EventHandler(this.cmdInsertNote);
            // 
            // m_menuNoteOldVersion
            // 
            this.m_menuNoteOldVersion.Image = ((System.Drawing.Image)(resources.GetObject("m_menuNoteOldVersion.Image")));
            this.m_menuNoteOldVersion.Name = "m_menuNoteOldVersion";
            this.m_menuNoteOldVersion.Size = new System.Drawing.Size(228, 22);
            this.m_menuNoteOldVersion.Tag = "kOldVersion";
            this.m_menuNoteOldVersion.Text = "Insert Old &Version Note";
            this.m_menuNoteOldVersion.Click += new System.EventHandler(this.cmdInsertNote);
            // 
            // m_menuNoteReason
            // 
            this.m_menuNoteReason.Image = ((System.Drawing.Image)(resources.GetObject("m_menuNoteReason.Image")));
            this.m_menuNoteReason.Name = "m_menuNoteReason";
            this.m_menuNoteReason.Size = new System.Drawing.Size(228, 22);
            this.m_menuNoteReason.Tag = "kReason";
            this.m_menuNoteReason.Text = "Insert &Reason Note";
            this.m_menuNoteReason.Click += new System.EventHandler(this.cmdInsertNote);
            // 
            // m_menuNoteFrontIssue
            // 
            this.m_menuNoteFrontIssue.Image = ((System.Drawing.Image)(resources.GetObject("m_menuNoteFrontIssue.Image")));
            this.m_menuNoteFrontIssue.Name = "m_menuNoteFrontIssue";
            this.m_menuNoteFrontIssue.Size = new System.Drawing.Size(228, 22);
            this.m_menuNoteFrontIssue.Tag = "kFront";
            this.m_menuNoteFrontIssue.Text = "Insert &Front Issue Note";
            this.m_menuNoteFrontIssue.Click += new System.EventHandler(this.cmdInsertNote);
            // 
            // m_menuNoteHintForDaughter
            // 
            this.m_menuNoteHintForDaughter.Image = ((System.Drawing.Image)(resources.GetObject("m_menuNoteHintForDaughter.Image")));
            this.m_menuNoteHintForDaughter.Name = "m_menuNoteHintForDaughter";
            this.m_menuNoteHintForDaughter.Size = new System.Drawing.Size(228, 22);
            this.m_menuNoteHintForDaughter.Tag = "kHintForDaughter";
            this.m_menuNoteHintForDaughter.Text = "Insert &Hint for Daughter Note";
            this.m_menuNoteHintForDaughter.Click += new System.EventHandler(this.cmdInsertNote);
            // 
            // m_menuNoteBackTranslation
            // 
            this.m_menuNoteBackTranslation.Image = ((System.Drawing.Image)(resources.GetObject("m_menuNoteBackTranslation.Image")));
            this.m_menuNoteBackTranslation.Name = "m_menuNoteBackTranslation";
            this.m_menuNoteBackTranslation.Size = new System.Drawing.Size(228, 22);
            this.m_menuNoteBackTranslation.Tag = "kBT";
            this.m_menuNoteBackTranslation.Text = "Insert &Back Translation Note";
            this.m_menuNoteBackTranslation.Click += new System.EventHandler(this.cmdInsertNote);
            // 
            // m_menuNotesSeparator2
            // 
            this.m_menuNotesSeparator2.Name = "m_menuNotesSeparator2";
            this.m_menuNotesSeparator2.Size = new System.Drawing.Size(225, 6);
            // 
            // m_menuDeleteNote
            // 
            this.m_menuDeleteNote.Image = ((System.Drawing.Image)(resources.GetObject("m_menuDeleteNote.Image")));
            this.m_menuDeleteNote.Name = "m_menuDeleteNote";
            this.m_menuDeleteNote.Size = new System.Drawing.Size(228, 22);
            this.m_menuDeleteNote.Text = "&Delete Note...";
            this.m_menuDeleteNote.Click += new System.EventHandler(this.cmdDeleteNote);
            // 
            // m_menuTools
            // 
            this.m_menuTools.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_menuIncrementBookStatus,
            this.m_menuRestoreFromBackup,
            this.m_menuOnlyShowSectionsThat,
            this.m_menuCopyBTfromFront,
            this.m_menuSetUpFeatures,
            this.m_separatorDebugItems,
            this.m_menuRunDebugTestSuite,
            this.m_menuLocalizerTool});
            this.m_menuTools.Name = "m_menuTools";
            this.m_menuTools.Size = new System.Drawing.Size(48, 20);
            this.m_menuTools.Text = "&Tools";
            // 
            // m_menuIncrementBookStatus
            // 
            this.m_menuIncrementBookStatus.Name = "m_menuIncrementBookStatus";
            this.m_menuIncrementBookStatus.Size = new System.Drawing.Size(211, 22);
            this.m_menuIncrementBookStatus.Text = "&Increment Book Status...";
            this.m_menuIncrementBookStatus.Click += new System.EventHandler(this.cmdIncrementBookStatus);
            // 
            // m_menuRestoreFromBackup
            // 
            this.m_menuRestoreFromBackup.Name = "m_menuRestoreFromBackup";
            this.m_menuRestoreFromBackup.Size = new System.Drawing.Size(211, 22);
            this.m_menuRestoreFromBackup.Text = "Restore from &Backup...";
            this.m_menuRestoreFromBackup.Click += new System.EventHandler(this.cmdRestoreBackup);
            // 
            // m_menuOnlyShowSectionsThat
            // 
            this.m_menuOnlyShowSectionsThat.Name = "m_menuOnlyShowSectionsThat";
            this.m_menuOnlyShowSectionsThat.Size = new System.Drawing.Size(211, 22);
            this.m_menuOnlyShowSectionsThat.Text = "Only &Show Sections that...";
            this.m_menuOnlyShowSectionsThat.Click += new System.EventHandler(this.cmdFilter);
            // 
            // m_menuCopyBTfromFront
            // 
            this.m_menuCopyBTfromFront.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_menuEntireBook,
            this.m_menuCurrentSectionOnly});
            this.m_menuCopyBTfromFront.Name = "m_menuCopyBTfromFront";
            this.m_menuCopyBTfromFront.Size = new System.Drawing.Size(211, 22);
            this.m_menuCopyBTfromFront.Text = "&Copy BT from Front";
            // 
            // m_menuEntireBook
            // 
            this.m_menuEntireBook.Name = "m_menuEntireBook";
            this.m_menuEntireBook.Size = new System.Drawing.Size(193, 22);
            this.m_menuEntireBook.Text = "Entire &Book...";
            this.m_menuEntireBook.Click += new System.EventHandler(this.cmdCopyBTfromFront_Book);
            // 
            // m_menuCurrentSectionOnly
            // 
            this.m_menuCurrentSectionOnly.Name = "m_menuCurrentSectionOnly";
            this.m_menuCurrentSectionOnly.Size = new System.Drawing.Size(193, 22);
            this.m_menuCurrentSectionOnly.Text = "Current &Section Only...";
            this.m_menuCurrentSectionOnly.Click += new System.EventHandler(this.cmdCopyBTfromFront_Section);
            // 
            // m_menuSetUpFeatures
            // 
            this.m_menuSetUpFeatures.Image = ((System.Drawing.Image)(resources.GetObject("m_menuSetUpFeatures.Image")));
            this.m_menuSetUpFeatures.Name = "m_menuSetUpFeatures";
            this.m_menuSetUpFeatures.Size = new System.Drawing.Size(211, 22);
            this.m_menuSetUpFeatures.Text = "Set Up &Features";
            this.m_menuSetUpFeatures.Click += new System.EventHandler(this.cmdSetUpFeatures);
            // 
            // m_separatorDebugItems
            // 
            this.m_separatorDebugItems.Name = "m_separatorDebugItems";
            this.m_separatorDebugItems.Size = new System.Drawing.Size(208, 6);
            // 
            // m_menuRunDebugTestSuite
            // 
            this.m_menuRunDebugTestSuite.Name = "m_menuRunDebugTestSuite";
            this.m_menuRunDebugTestSuite.Size = new System.Drawing.Size(211, 22);
            this.m_menuRunDebugTestSuite.Text = "&Run Debug Test Suite";
            this.m_menuRunDebugTestSuite.Click += new System.EventHandler(this.cmdDebugTesting);
            // 
            // m_menuLocalizerTool
            // 
            this.m_menuLocalizerTool.Name = "m_menuLocalizerTool";
            this.m_menuLocalizerTool.Size = new System.Drawing.Size(211, 22);
            this.m_menuLocalizerTool.Text = "&Localizer Tool...";
            this.m_menuLocalizerTool.Click += new System.EventHandler(this.cmdLocalizer);
            // 
            // m_menuWindow
            // 
            this.m_menuWindow.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_menuShowNotesPane,
            this.m_menuShowOtherTranslationsPane,
            this.m_menuSeparatorTasks,
            this.m_menuDrafting,
            this.m_menuNaturalnessCheck,
            this.m_menuBackTranslation});
            this.m_menuWindow.Name = "m_menuWindow";
            this.m_menuWindow.Size = new System.Drawing.Size(63, 20);
            this.m_menuWindow.Text = "&Window";
            // 
            // m_menuShowNotesPane
            // 
            this.m_menuShowNotesPane.Name = "m_menuShowNotesPane";
            this.m_menuShowNotesPane.Size = new System.Drawing.Size(232, 22);
            this.m_menuShowNotesPane.Text = "Show &Notes Pane";
            this.m_menuShowNotesPane.Click += new System.EventHandler(this.cmdToggleNotesPane);
            // 
            // m_menuShowOtherTranslationsPane
            // 
            this.m_menuShowOtherTranslationsPane.Name = "m_menuShowOtherTranslationsPane";
            this.m_menuShowOtherTranslationsPane.Size = new System.Drawing.Size(232, 22);
            this.m_menuShowOtherTranslationsPane.Text = "Show &Other Translations Pane";
            this.m_menuShowOtherTranslationsPane.Click += new System.EventHandler(this.cmdToggleOtherTranslationsPane);
            // 
            // m_menuSeparatorTasks
            // 
            this.m_menuSeparatorTasks.Name = "m_menuSeparatorTasks";
            this.m_menuSeparatorTasks.Size = new System.Drawing.Size(229, 6);
            // 
            // m_menuDrafting
            // 
            this.m_menuDrafting.Image = ((System.Drawing.Image)(resources.GetObject("m_menuDrafting.Image")));
            this.m_menuDrafting.Name = "m_menuDrafting";
            this.m_menuDrafting.Size = new System.Drawing.Size(232, 22);
            this.m_menuDrafting.Text = "&Drafting";
            this.m_menuDrafting.Click += new System.EventHandler(this.cmdJobDrafting);
            // 
            // m_menuNaturalnessCheck
            // 
            this.m_menuNaturalnessCheck.Image = ((System.Drawing.Image)(resources.GetObject("m_menuNaturalnessCheck.Image")));
            this.m_menuNaturalnessCheck.Name = "m_menuNaturalnessCheck";
            this.m_menuNaturalnessCheck.Size = new System.Drawing.Size(232, 22);
            this.m_menuNaturalnessCheck.Text = "&Naturalness Check";
            this.m_menuNaturalnessCheck.Click += new System.EventHandler(this.cmdJobNaturalness);
            // 
            // m_menuBackTranslation
            // 
            this.m_menuBackTranslation.Image = ((System.Drawing.Image)(resources.GetObject("m_menuBackTranslation.Image")));
            this.m_menuBackTranslation.Name = "m_menuBackTranslation";
            this.m_menuBackTranslation.Size = new System.Drawing.Size(232, 22);
            this.m_menuBackTranslation.Text = "&Back Translation";
            this.m_menuBackTranslation.Click += new System.EventHandler(this.cmdJobBackTranslation);
            // 
            // m_ToolStrip
            // 
            this.m_ToolStrip.Dock = System.Windows.Forms.DockStyle.None;
            this.m_ToolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.m_ToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_btnExit,
            this.m_btnProjectSave,
            this.toolStripSeparator1,
            this.m_btnEditCut,
            this.m_btnEditCopy,
            this.m_btnEditPaste,
            this.m_btnItalic,
            toolStripSeparator2,
            this.m_btnGotoFirstSection,
            this.m_btnGotoPreviousSection,
            this.m_btnGotoNextSection,
            this.m_btnGotoLastSection,
            this.m_btnGoToBook,
            this.m_SeparatorTasks,
            this.m_btnDrafting,
            this.m_btnBackTranslation,
            this.m_btnNaturalnessCheck,
            this.m_toolSeparatorNotes,
            this.m_btnNotes,
            this.m_btnNoteGeneral,
            this.m_btnNoteToDo,
            this.m_btnNoteAskUNS,
            this.m_btnNoteDefinition,
            this.m_btnNoteOldVersion,
            this.m_btnNoteReason,
            this.m_btnNoteFrontIssue,
            this.m_btnNoteHintForDaughter,
            this.m_btnNoteBackTranslation,
            this.m_btnDeleteNote,
            this.m_bmTools,
            this.toolStripSeparator7,
            this.m_bmHelp});
            this.m_ToolStrip.Location = new System.Drawing.Point(3, 24);
            this.m_ToolStrip.Name = "m_ToolStrip";
            this.m_ToolStrip.Size = new System.Drawing.Size(721, 25);
            this.m_ToolStrip.TabIndex = 1;
            // 
            // m_btnExit
            // 
            this.m_btnExit.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.m_btnExit.Image = ((System.Drawing.Image)(resources.GetObject("m_btnExit.Image")));
            this.m_btnExit.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.m_btnExit.Name = "m_btnExit";
            this.m_btnExit.Size = new System.Drawing.Size(23, 22);
            this.m_btnExit.Text = "Exit";
            this.m_btnExit.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.m_btnExit.Click += new System.EventHandler(this.cmdExit);
            // 
            // m_btnProjectSave
            // 
            this.m_btnProjectSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.m_btnProjectSave.Image = ((System.Drawing.Image)(resources.GetObject("m_btnProjectSave.Image")));
            this.m_btnProjectSave.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.m_btnProjectSave.Name = "m_btnProjectSave";
            this.m_btnProjectSave.Size = new System.Drawing.Size(23, 22);
            this.m_btnProjectSave.Text = "Save";
            this.m_btnProjectSave.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.m_btnProjectSave.Click += new System.EventHandler(this.cmdSaveProject);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // m_btnEditCut
            // 
            this.m_btnEditCut.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.m_btnEditCut.Image = ((System.Drawing.Image)(resources.GetObject("m_btnEditCut.Image")));
            this.m_btnEditCut.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.m_btnEditCut.Name = "m_btnEditCut";
            this.m_btnEditCut.Size = new System.Drawing.Size(23, 22);
            this.m_btnEditCut.Text = "Cut";
            this.m_btnEditCut.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.m_btnEditCut.Click += new System.EventHandler(this.cmdEditCut);
            // 
            // m_btnEditCopy
            // 
            this.m_btnEditCopy.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.m_btnEditCopy.Image = ((System.Drawing.Image)(resources.GetObject("m_btnEditCopy.Image")));
            this.m_btnEditCopy.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.m_btnEditCopy.Name = "m_btnEditCopy";
            this.m_btnEditCopy.Size = new System.Drawing.Size(23, 22);
            this.m_btnEditCopy.Text = "Copy";
            this.m_btnEditCopy.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.m_btnEditCopy.Click += new System.EventHandler(this.cmdEditCopy);
            // 
            // m_btnEditPaste
            // 
            this.m_btnEditPaste.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.m_btnEditPaste.Image = ((System.Drawing.Image)(resources.GetObject("m_btnEditPaste.Image")));
            this.m_btnEditPaste.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.m_btnEditPaste.Name = "m_btnEditPaste";
            this.m_btnEditPaste.Size = new System.Drawing.Size(23, 22);
            this.m_btnEditPaste.Text = "Paste";
            this.m_btnEditPaste.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.m_btnEditPaste.Click += new System.EventHandler(this.cmdEditPaste);
            // 
            // m_btnItalic
            // 
            this.m_btnItalic.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.m_btnItalic.Image = ((System.Drawing.Image)(resources.GetObject("m_btnItalic.Image")));
            this.m_btnItalic.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.m_btnItalic.Name = "m_btnItalic";
            this.m_btnItalic.Size = new System.Drawing.Size(23, 22);
            this.m_btnItalic.Text = "Italic";
            this.m_btnItalic.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.m_btnItalic.Click += new System.EventHandler(this.cmdItalic);
            // 
            // m_btnGotoFirstSection
            // 
            this.m_btnGotoFirstSection.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.m_btnGotoFirstSection.Image = ((System.Drawing.Image)(resources.GetObject("m_btnGotoFirstSection.Image")));
            this.m_btnGotoFirstSection.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.m_btnGotoFirstSection.Name = "m_btnGotoFirstSection";
            this.m_btnGotoFirstSection.Size = new System.Drawing.Size(23, 22);
            this.m_btnGotoFirstSection.Text = "First";
            this.m_btnGotoFirstSection.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.m_btnGotoFirstSection.Click += new System.EventHandler(this.cmdGoToFirstSection);
            // 
            // m_btnGotoPreviousSection
            // 
            this.m_btnGotoPreviousSection.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.m_btnGotoPreviousSection.Image = ((System.Drawing.Image)(resources.GetObject("m_btnGotoPreviousSection.Image")));
            this.m_btnGotoPreviousSection.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.m_btnGotoPreviousSection.Name = "m_btnGotoPreviousSection";
            this.m_btnGotoPreviousSection.Size = new System.Drawing.Size(32, 22);
            this.m_btnGotoPreviousSection.Text = "Previous";
            this.m_btnGotoPreviousSection.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.m_btnGotoPreviousSection.ButtonClick += new System.EventHandler(this.cmdGoToPreviousSection);
            // 
            // m_btnGotoNextSection
            // 
            this.m_btnGotoNextSection.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.m_btnGotoNextSection.Image = ((System.Drawing.Image)(resources.GetObject("m_btnGotoNextSection.Image")));
            this.m_btnGotoNextSection.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.m_btnGotoNextSection.Name = "m_btnGotoNextSection";
            this.m_btnGotoNextSection.Size = new System.Drawing.Size(32, 22);
            this.m_btnGotoNextSection.Text = "Next";
            this.m_btnGotoNextSection.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.m_btnGotoNextSection.ButtonClick += new System.EventHandler(this.cmdGoToNextSection);
            // 
            // m_btnGotoLastSection
            // 
            this.m_btnGotoLastSection.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.m_btnGotoLastSection.Image = ((System.Drawing.Image)(resources.GetObject("m_btnGotoLastSection.Image")));
            this.m_btnGotoLastSection.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.m_btnGotoLastSection.Name = "m_btnGotoLastSection";
            this.m_btnGotoLastSection.Size = new System.Drawing.Size(23, 22);
            this.m_btnGotoLastSection.Text = "Last";
            this.m_btnGotoLastSection.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.m_btnGotoLastSection.Click += new System.EventHandler(this.cmdGoToLastSection);
            // 
            // m_btnGoToBook
            // 
            this.m_btnGoToBook.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.m_btnGoToBook.Image = ((System.Drawing.Image)(resources.GetObject("m_btnGoToBook.Image")));
            this.m_btnGoToBook.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.m_btnGoToBook.Name = "m_btnGoToBook";
            this.m_btnGoToBook.Size = new System.Drawing.Size(29, 22);
            this.m_btnGoToBook.Text = "Book";
            this.m_btnGoToBook.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.m_btnGoToBook.ToolTipText = "Go to a different book";
            // 
            // m_SeparatorTasks
            // 
            this.m_SeparatorTasks.Name = "m_SeparatorTasks";
            this.m_SeparatorTasks.Size = new System.Drawing.Size(6, 25);
            // 
            // m_btnDrafting
            // 
            this.m_btnDrafting.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.m_btnDrafting.Image = ((System.Drawing.Image)(resources.GetObject("m_btnDrafting.Image")));
            this.m_btnDrafting.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.m_btnDrafting.Name = "m_btnDrafting";
            this.m_btnDrafting.Size = new System.Drawing.Size(23, 22);
            this.m_btnDrafting.Text = "Drafting";
            this.m_btnDrafting.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.m_btnDrafting.Click += new System.EventHandler(this.cmdJobDrafting);
            // 
            // m_btnBackTranslation
            // 
            this.m_btnBackTranslation.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.m_btnBackTranslation.Image = ((System.Drawing.Image)(resources.GetObject("m_btnBackTranslation.Image")));
            this.m_btnBackTranslation.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.m_btnBackTranslation.Name = "m_btnBackTranslation";
            this.m_btnBackTranslation.Size = new System.Drawing.Size(23, 22);
            this.m_btnBackTranslation.Text = "Back Translation";
            this.m_btnBackTranslation.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.m_btnBackTranslation.Click += new System.EventHandler(this.cmdJobBackTranslation);
            // 
            // m_btnNaturalnessCheck
            // 
            this.m_btnNaturalnessCheck.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.m_btnNaturalnessCheck.Image = ((System.Drawing.Image)(resources.GetObject("m_btnNaturalnessCheck.Image")));
            this.m_btnNaturalnessCheck.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.m_btnNaturalnessCheck.Name = "m_btnNaturalnessCheck";
            this.m_btnNaturalnessCheck.Size = new System.Drawing.Size(23, 22);
            this.m_btnNaturalnessCheck.Text = "Naturalness Check";
            this.m_btnNaturalnessCheck.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.m_btnNaturalnessCheck.Click += new System.EventHandler(this.cmdJobNaturalness);
            // 
            // m_toolSeparatorNotes
            // 
            this.m_toolSeparatorNotes.Name = "m_toolSeparatorNotes";
            this.m_toolSeparatorNotes.Size = new System.Drawing.Size(6, 25);
            // 
            // m_btnNotes
            // 
            this.m_btnNotes.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.m_btnNotes.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_bmShowNotesPane,
            this.m_bmNotesSeparator1,
            this.m_bmNoteGeneral,
            this.m_bmNoteToDo,
            this.m_bmNoteAskUNS,
            this.m_bmNoteDefinition,
            this.m_bmNoteOldVersion,
            this.m_bmNoteReason,
            this.m_bmNoteFrontIssue,
            this.m_bmNotesSeparator2,
            this.m_bmDeleteNote});
            this.m_btnNotes.Image = ((System.Drawing.Image)(resources.GetObject("m_btnNotes.Image")));
            this.m_btnNotes.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.m_btnNotes.Name = "m_btnNotes";
            this.m_btnNotes.Size = new System.Drawing.Size(29, 22);
            this.m_btnNotes.Text = "Notes";
            this.m_btnNotes.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            // 
            // m_bmShowNotesPane
            // 
            this.m_bmShowNotesPane.Name = "m_bmShowNotesPane";
            this.m_bmShowNotesPane.Size = new System.Drawing.Size(196, 22);
            this.m_bmShowNotesPane.Text = "&Show Notes Pane";
            this.m_bmShowNotesPane.Click += new System.EventHandler(this.cmdToggleNotesPane);
            // 
            // m_bmNotesSeparator1
            // 
            this.m_bmNotesSeparator1.Name = "m_bmNotesSeparator1";
            this.m_bmNotesSeparator1.Size = new System.Drawing.Size(193, 6);
            // 
            // m_bmNoteGeneral
            // 
            this.m_bmNoteGeneral.Image = ((System.Drawing.Image)(resources.GetObject("m_bmNoteGeneral.Image")));
            this.m_bmNoteGeneral.Name = "m_bmNoteGeneral";
            this.m_bmNoteGeneral.Size = new System.Drawing.Size(196, 22);
            this.m_bmNoteGeneral.Tag = "kGeneral";
            this.m_bmNoteGeneral.Text = "Insert General &Note";
            this.m_bmNoteGeneral.Click += new System.EventHandler(this.cmdInsertNote);
            // 
            // m_bmNoteToDo
            // 
            this.m_bmNoteToDo.Image = ((System.Drawing.Image)(resources.GetObject("m_bmNoteToDo.Image")));
            this.m_bmNoteToDo.Name = "m_bmNoteToDo";
            this.m_bmNoteToDo.Size = new System.Drawing.Size(196, 22);
            this.m_bmNoteToDo.Tag = "kToDo";
            this.m_bmNoteToDo.Text = "Insert &To Do Note";
            this.m_bmNoteToDo.Click += new System.EventHandler(this.cmdInsertNote);
            // 
            // m_bmNoteAskUNS
            // 
            this.m_bmNoteAskUNS.Image = ((System.Drawing.Image)(resources.GetObject("m_bmNoteAskUNS.Image")));
            this.m_bmNoteAskUNS.Name = "m_bmNoteAskUNS";
            this.m_bmNoteAskUNS.Size = new System.Drawing.Size(196, 22);
            this.m_bmNoteAskUNS.Tag = "kAskUns";
            this.m_bmNoteAskUNS.Text = "Insert &Ask UNS Note";
            this.m_bmNoteAskUNS.Click += new System.EventHandler(this.cmdInsertNote);
            // 
            // m_bmNoteDefinition
            // 
            this.m_bmNoteDefinition.Image = ((System.Drawing.Image)(resources.GetObject("m_bmNoteDefinition.Image")));
            this.m_bmNoteDefinition.Name = "m_bmNoteDefinition";
            this.m_bmNoteDefinition.Size = new System.Drawing.Size(196, 22);
            this.m_bmNoteDefinition.Tag = "kDefinition";
            this.m_bmNoteDefinition.Text = "Insert &Definition Note";
            this.m_bmNoteDefinition.Click += new System.EventHandler(this.cmdInsertNote);
            // 
            // m_bmNoteOldVersion
            // 
            this.m_bmNoteOldVersion.Image = ((System.Drawing.Image)(resources.GetObject("m_bmNoteOldVersion.Image")));
            this.m_bmNoteOldVersion.Name = "m_bmNoteOldVersion";
            this.m_bmNoteOldVersion.Size = new System.Drawing.Size(196, 22);
            this.m_bmNoteOldVersion.Tag = "kOldVersion";
            this.m_bmNoteOldVersion.Text = "Insert Old &Version Note";
            this.m_bmNoteOldVersion.Click += new System.EventHandler(this.cmdInsertNote);
            // 
            // m_bmNoteReason
            // 
            this.m_bmNoteReason.Image = ((System.Drawing.Image)(resources.GetObject("m_bmNoteReason.Image")));
            this.m_bmNoteReason.Name = "m_bmNoteReason";
            this.m_bmNoteReason.Size = new System.Drawing.Size(196, 22);
            this.m_bmNoteReason.Tag = "kReason";
            this.m_bmNoteReason.Text = "Insert &Reason Note";
            this.m_bmNoteReason.Click += new System.EventHandler(this.cmdInsertNote);
            // 
            // m_bmNoteFrontIssue
            // 
            this.m_bmNoteFrontIssue.Image = ((System.Drawing.Image)(resources.GetObject("m_bmNoteFrontIssue.Image")));
            this.m_bmNoteFrontIssue.Name = "m_bmNoteFrontIssue";
            this.m_bmNoteFrontIssue.Size = new System.Drawing.Size(196, 22);
            this.m_bmNoteFrontIssue.Tag = "kFront";
            this.m_bmNoteFrontIssue.Text = "Insert &Front Issue Note";
            this.m_bmNoteFrontIssue.Click += new System.EventHandler(this.cmdInsertNote);
            // 
            // m_bmNotesSeparator2
            // 
            this.m_bmNotesSeparator2.Name = "m_bmNotesSeparator2";
            this.m_bmNotesSeparator2.Size = new System.Drawing.Size(193, 6);
            // 
            // m_bmDeleteNote
            // 
            this.m_bmDeleteNote.Image = ((System.Drawing.Image)(resources.GetObject("m_bmDeleteNote.Image")));
            this.m_bmDeleteNote.Name = "m_bmDeleteNote";
            this.m_bmDeleteNote.Size = new System.Drawing.Size(196, 22);
            this.m_bmDeleteNote.Text = "&Delete Note";
            this.m_bmDeleteNote.Click += new System.EventHandler(this.cmdDeleteNote);
            // 
            // m_btnNoteGeneral
            // 
            this.m_btnNoteGeneral.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.m_btnNoteGeneral.Image = ((System.Drawing.Image)(resources.GetObject("m_btnNoteGeneral.Image")));
            this.m_btnNoteGeneral.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.m_btnNoteGeneral.Name = "m_btnNoteGeneral";
            this.m_btnNoteGeneral.Size = new System.Drawing.Size(23, 22);
            this.m_btnNoteGeneral.Tag = "kGeneral";
            this.m_btnNoteGeneral.Text = "General Note";
            this.m_btnNoteGeneral.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.m_btnNoteGeneral.Click += new System.EventHandler(this.cmdInsertNote);
            // 
            // m_btnNoteToDo
            // 
            this.m_btnNoteToDo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.m_btnNoteToDo.Image = ((System.Drawing.Image)(resources.GetObject("m_btnNoteToDo.Image")));
            this.m_btnNoteToDo.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.m_btnNoteToDo.Name = "m_btnNoteToDo";
            this.m_btnNoteToDo.Size = new System.Drawing.Size(23, 22);
            this.m_btnNoteToDo.Tag = "kToDo";
            this.m_btnNoteToDo.Text = "To Do";
            this.m_btnNoteToDo.Click += new System.EventHandler(this.cmdInsertNote);
            // 
            // m_btnNoteAskUNS
            // 
            this.m_btnNoteAskUNS.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.m_btnNoteAskUNS.Image = ((System.Drawing.Image)(resources.GetObject("m_btnNoteAskUNS.Image")));
            this.m_btnNoteAskUNS.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.m_btnNoteAskUNS.Name = "m_btnNoteAskUNS";
            this.m_btnNoteAskUNS.Size = new System.Drawing.Size(23, 22);
            this.m_btnNoteAskUNS.Tag = "kAskUns";
            this.m_btnNoteAskUNS.Text = "Ask UNS";
            this.m_btnNoteAskUNS.Click += new System.EventHandler(this.cmdInsertNote);
            // 
            // m_btnNoteDefinition
            // 
            this.m_btnNoteDefinition.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.m_btnNoteDefinition.Image = ((System.Drawing.Image)(resources.GetObject("m_btnNoteDefinition.Image")));
            this.m_btnNoteDefinition.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.m_btnNoteDefinition.Name = "m_btnNoteDefinition";
            this.m_btnNoteDefinition.Size = new System.Drawing.Size(23, 22);
            this.m_btnNoteDefinition.Tag = "kDefinition";
            this.m_btnNoteDefinition.Text = "Definition";
            this.m_btnNoteDefinition.Click += new System.EventHandler(this.cmdInsertNote);
            // 
            // m_btnNoteOldVersion
            // 
            this.m_btnNoteOldVersion.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.m_btnNoteOldVersion.Image = ((System.Drawing.Image)(resources.GetObject("m_btnNoteOldVersion.Image")));
            this.m_btnNoteOldVersion.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.m_btnNoteOldVersion.Name = "m_btnNoteOldVersion";
            this.m_btnNoteOldVersion.Size = new System.Drawing.Size(23, 22);
            this.m_btnNoteOldVersion.Tag = "kOldVersion";
            this.m_btnNoteOldVersion.Text = "Old Version";
            this.m_btnNoteOldVersion.Click += new System.EventHandler(this.cmdInsertNote);
            // 
            // m_btnNoteReason
            // 
            this.m_btnNoteReason.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.m_btnNoteReason.Image = ((System.Drawing.Image)(resources.GetObject("m_btnNoteReason.Image")));
            this.m_btnNoteReason.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.m_btnNoteReason.Name = "m_btnNoteReason";
            this.m_btnNoteReason.Size = new System.Drawing.Size(23, 22);
            this.m_btnNoteReason.Tag = "kReason";
            this.m_btnNoteReason.Text = "Reason";
            this.m_btnNoteReason.Click += new System.EventHandler(this.cmdInsertNote);
            // 
            // m_btnNoteFrontIssue
            // 
            this.m_btnNoteFrontIssue.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.m_btnNoteFrontIssue.Image = ((System.Drawing.Image)(resources.GetObject("m_btnNoteFrontIssue.Image")));
            this.m_btnNoteFrontIssue.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.m_btnNoteFrontIssue.Name = "m_btnNoteFrontIssue";
            this.m_btnNoteFrontIssue.Size = new System.Drawing.Size(23, 22);
            this.m_btnNoteFrontIssue.Tag = "kFront";
            this.m_btnNoteFrontIssue.Text = "Front Issue";
            this.m_btnNoteFrontIssue.Click += new System.EventHandler(this.cmdInsertNote);
            // 
            // m_btnNoteHintForDaughter
            // 
            this.m_btnNoteHintForDaughter.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.m_btnNoteHintForDaughter.Image = ((System.Drawing.Image)(resources.GetObject("m_btnNoteHintForDaughter.Image")));
            this.m_btnNoteHintForDaughter.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.m_btnNoteHintForDaughter.Name = "m_btnNoteHintForDaughter";
            this.m_btnNoteHintForDaughter.Size = new System.Drawing.Size(23, 22);
            this.m_btnNoteHintForDaughter.Tag = "kHintForDaughter";
            this.m_btnNoteHintForDaughter.Text = "Hint for Daughter";
            this.m_btnNoteHintForDaughter.Click += new System.EventHandler(this.cmdInsertNote);
            // 
            // m_btnNoteBackTranslation
            // 
            this.m_btnNoteBackTranslation.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.m_btnNoteBackTranslation.Image = ((System.Drawing.Image)(resources.GetObject("m_btnNoteBackTranslation.Image")));
            this.m_btnNoteBackTranslation.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.m_btnNoteBackTranslation.Name = "m_btnNoteBackTranslation";
            this.m_btnNoteBackTranslation.Size = new System.Drawing.Size(23, 22);
            this.m_btnNoteBackTranslation.Tag = "kBT";
            this.m_btnNoteBackTranslation.Text = "Back Translation";
            this.m_btnNoteBackTranslation.Click += new System.EventHandler(this.cmdInsertNote);
            // 
            // m_btnDeleteNote
            // 
            this.m_btnDeleteNote.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.m_btnDeleteNote.Image = ((System.Drawing.Image)(resources.GetObject("m_btnDeleteNote.Image")));
            this.m_btnDeleteNote.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.m_btnDeleteNote.Name = "m_btnDeleteNote";
            this.m_btnDeleteNote.Size = new System.Drawing.Size(23, 22);
            this.m_btnDeleteNote.Text = "Delete Note";
            this.m_btnDeleteNote.ToolTipText = "Delete this note.";
            this.m_btnDeleteNote.Click += new System.EventHandler(this.cmdDeleteNote);
            // 
            // m_bmTools
            // 
            this.m_bmTools.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.m_bmTools.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_bmIncrementBookStatus,
            this.m_bmRestoreFromBackup,
            this.m_bmSetUpFeatures});
            this.m_bmTools.Image = ((System.Drawing.Image)(resources.GetObject("m_bmTools.Image")));
            this.m_bmTools.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.m_bmTools.Name = "m_bmTools";
            this.m_bmTools.Size = new System.Drawing.Size(29, 22);
            this.m_bmTools.Text = "Tools";
            this.m_bmTools.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            // 
            // m_bmIncrementBookStatus
            // 
            this.m_bmIncrementBookStatus.Name = "m_bmIncrementBookStatus";
            this.m_bmIncrementBookStatus.Size = new System.Drawing.Size(202, 22);
            this.m_bmIncrementBookStatus.Text = "&Increment Book Status...";
            this.m_bmIncrementBookStatus.Click += new System.EventHandler(this.cmdIncrementBookStatus);
            // 
            // m_bmRestoreFromBackup
            // 
            this.m_bmRestoreFromBackup.Name = "m_bmRestoreFromBackup";
            this.m_bmRestoreFromBackup.Size = new System.Drawing.Size(202, 22);
            this.m_bmRestoreFromBackup.Text = "Restore from &Backup...";
            this.m_bmRestoreFromBackup.Click += new System.EventHandler(this.cmdRestoreBackup);
            // 
            // m_bmSetUpFeatures
            // 
            this.m_bmSetUpFeatures.Image = ((System.Drawing.Image)(resources.GetObject("m_bmSetUpFeatures.Image")));
            this.m_bmSetUpFeatures.Name = "m_bmSetUpFeatures";
            this.m_bmSetUpFeatures.Size = new System.Drawing.Size(202, 22);
            this.m_bmSetUpFeatures.Text = "Set Up &Features...";
            this.m_bmSetUpFeatures.Click += new System.EventHandler(this.cmdSetUpFeatures);
            // 
            // toolStripSeparator7
            // 
            this.toolStripSeparator7.Name = "toolStripSeparator7";
            this.toolStripSeparator7.Size = new System.Drawing.Size(6, 25);
            // 
            // m_bmHelp
            // 
            this.m_bmHelp.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.m_bmHelp.Image = ((System.Drawing.Image)(resources.GetObject("m_bmHelp.Image")));
            this.m_bmHelp.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.m_bmHelp.Name = "m_bmHelp";
            this.m_bmHelp.Size = new System.Drawing.Size(23, 22);
            this.m_bmHelp.Text = "Help";
            this.m_bmHelp.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.m_bmHelp.Click += new System.EventHandler(this.cmdHelpTopics);
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
            this.m_toolStripContainer.ContentPanel.Size = new System.Drawing.Size(748, 430);
            this.m_toolStripContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_toolStripContainer.LeftToolStripPanelVisible = false;
            this.m_toolStripContainer.Location = new System.Drawing.Point(0, 0);
            this.m_toolStripContainer.Name = "m_toolStripContainer";
            this.m_toolStripContainer.RightToolStripPanelVisible = false;
            this.m_toolStripContainer.Size = new System.Drawing.Size(748, 526);
            this.m_toolStripContainer.TabIndex = 2;
            this.m_toolStripContainer.Text = "toolStripContainer1";
            // 
            // m_toolStripContainer.TopToolStripPanel
            // 
            this.m_toolStripContainer.TopToolStripPanel.Controls.Add(this.m_MenuStrip);
            this.m_toolStripContainer.TopToolStripPanel.Controls.Add(this.m_ToolStrip);
            this.m_toolStripContainer.TopToolStripPanel.Controls.Add(this.m_Taskbar);
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
            this.m_StatusStrip.Size = new System.Drawing.Size(748, 22);
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
            this.m_StatusMessage2.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
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
            this.m_SplitContainer.Size = new System.Drawing.Size(748, 430);
            this.m_SplitContainer.SplitterDistance = 508;
            this.m_SplitContainer.TabIndex = 0;
            // 
            // m_Taskbar
            // 
            this.m_Taskbar.BackColor = System.Drawing.Color.Gray;
            this.m_Taskbar.Dock = System.Windows.Forms.DockStyle.None;
            this.m_Taskbar.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.m_Taskbar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_tbTaskName,
            this.m_tbPadlock,
            this.m_tbCurrentPassage});
            this.m_Taskbar.Location = new System.Drawing.Point(0, 49);
            this.m_Taskbar.Name = "m_Taskbar";
            this.m_Taskbar.Size = new System.Drawing.Size(748, 25);
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
            // m_tbCurrentPassage
            // 
            this.m_tbCurrentPassage.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_tbCurrentPassage.ForeColor = System.Drawing.Color.Wheat;
            this.m_tbCurrentPassage.Name = "m_tbCurrentPassage";
            this.m_tbCurrentPassage.Size = new System.Drawing.Size(79, 22);
            this.m_tbCurrentPassage.Text = "(passage)";
            // 
            // OurWordMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(748, 526);
            this.Controls.Add(this.m_toolStripContainer);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "OurWordMain";
            this.Text = "Our Word!";
            this.Closing += new System.ComponentModel.CancelEventHandler(this.cmd_OnClosing);
            this.Load += new System.EventHandler(this.cmd_OnLoad);
            this.m_MenuStrip.ResumeLayout(false);
            this.m_MenuStrip.PerformLayout();
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
                LocDB.Message("msgAlreadyRunning", "OurWord is already running.", null, LocDB.MessageTypes.Error);
                return false;
            }

            return true;
        }
        #endregion
        #region Method: static void Main() - main entry point for the application
        [STAThread]
		static void Main() 
		{
            // Initialize the registry (Needed by, e.g., Splash Screen and the
            // single-instance error message.)
            JW_Registry.RootKey = "SOFTWARE\\The Seed Company\\Our Word!";

            // We only want a single instance of OurWord to be running at a time, 
			// Because of the confusion that could reign if we had multiple 
            // instances modifying the Translation and Project settings file, 
            // we elect instead to just have a single instance.
            ShowLoadState("Check for multiple instances");
            if (!_GrabTokenForThisInstance())
                return;

            // Initialize the Localizations Database
            ShowLoadState("Init LocDB");
            LocDB.Initialize(G.GetApplicationDataFolder());

            // Set the resource location (so the splash picture will be visible)
            ShowLoadState("Init Resource Location");
            JWU.ResourceLocation = "OurWord.Res.";

            // Retrieve localizations from the Localizations database. (Must be done
            // here so that the Splash window will be localized.
//            ShowLoadState("Init Language Resources");
//            Options.InitLanguageResources();

            // Display a splash screen while we're loading
            ShowLoadState("Init Splash Screen");
            SplashScreen.Start();

            // Now start loading & run the program
            ShowLoadState("Construct and Run");
            OurWordMain.s_App = new OurWordMain();
            Application.Run(s_App);

            // All done, release the Mutex
            ShowLoadState("Release Mutex");
            if (s_EnsureOneInstanceOnlyMutex != null)
               s_EnsureOneInstanceOnlyMutex.ReleaseMutex();
           ShowLoadState("Done");

        }
		#endregion

		// Features --------------------------------------------------------------------------
		#region EMBEDDED CLASS: FeaturesMgr
		public class FeaturesMgr
		{
			// Attrs -------------------------------------------------------------------------
			DialogSetupFeatures m_Dlg = null;

			// Values (obtain from current project) ------------------------------------------
			#region Attr{g}: bool F_JobBT
			public bool F_JobBT
			{
				get
				{
					return m_Dlg.GetEnabledState( ID.fJobBT.ToString());
				}
			}
			#endregion
            #region Attr{g}: bool F_JobNaturalness
            public bool F_JobNaturalness
            {
                get
                {
                    return m_Dlg.GetEnabledState(ID.fJobNaturalness.ToString());
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
                    return m_Dlg.GetEnabledState(ID.fPropertiesDialog.ToString());
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
			#region Attr{g}: bool F_CopyBTfromFront
			public bool F_CopyBTfromFront
			{
				get
				{
					return m_Dlg.GetEnabledState( ID.fCopyBTfromFront.ToString());
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
            #region Attr{g}: bool F_JustTheBasics
			public bool F_JustTheBasics
			{
				get
				{
					return m_Dlg.GetEnabledState( ID.fJustTheBasics.ToString());
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
            // 2. Add it into the Setup method (don't forget the JustTheBasics dependencies)
            // 3. Add it to the Localizations database
            // 4. Set up an Attribute above for access elsewhere in the software
            // 5. Typical other areas to code:
            //    - Menu & toolbar visibility / enabling
            #region IDs
            public enum ID
            {
                fProject,
                fPropertiesDialog,
                fPrint,
                fJobBT,
                fJobNaturalness,
                fRestoreBackup,
                fCopyBTfromFront,
                fFilter,
                fJustTheBasics,
                fStructuralEditing,
                fLocalizer,
                kLast
            };
            #endregion
			#region Method: void Setup()
			private void Setup()
			{
				Debug.Assert(null != m_Dlg);

                // Clear out everything previous
				m_Dlg.Clear();

                // Add the various features
                m_Dlg.Add(ID.fJobBT.ToString(),
                    false,
                    "Back Translation Job",
                    "A layout where you can do a Back Translation; that is, where you provide " +
                        "a translation  in the language the consultant understands.");

                m_Dlg.Add(ID.fJobNaturalness.ToString(),           
                    false, 
                    "Naturalness Check Job",
                    "A layout where only the translation visible, so that you can read through " +
                        "for naturalness, without being influenced by the front translation.");

                m_Dlg.Add(ID.fProject.ToString(),  
                    true, 
                    "Project New / Open / etc.",
                    "Turn this on if you are working with multiple projects.");

                m_Dlg.Add(ID.fPropertiesDialog.ToString(),
                    true,
                    "Project Properties Dialog",
                    "Turn this on if you are want to adjust the properties for a project.");

                m_Dlg.Add(ID.fPrint.ToString(),            
                    false,
                    "Printing",
                    "The book can be formatted and printed. ");

                m_Dlg.Add(ID.fRestoreBackup.ToString(),    
                    false,
                    "Restore Backed-up Files",
                    "Files can be automatically backed up (via the Tools-Options command) " +
                        "to a flash card or other storage device. If you turn on this Restore " +
                        "feature, the Tools menu will provide access to a dialog by which the " +
                        "current file can be replaced by a previously stored backup.");

                m_Dlg.Add(ID.fCopyBTfromFront.ToString(),  
                    false,
                    "Copy the BT from the Front Translation",
                    "Copies the back translation from the Front to the Target. This provides " +
                        "a convenient starting place for a back translation which you should then " +
                        "plan on carefully reviewing and editing, so that it accurately matches " +
                        "the actual vernacular translation.");

                m_Dlg.Add(ID.fFilter.ToString(),           
                    false,
                    "Only show Sections that have (Filter)",
                    "The \"Go To\" Menu (e.g., Next, Previous commands) will only go to those " +
                        "sections which match certain criteria. E.g., they must have a certain word " +
                        "defined, or must have mismatched quotes. This can be a good way to locate " +
                        "errors, or To Do Notes.");

                m_Dlg.Add(ID.fLocalizer.ToString(),
                    false,
                    "Localization Dialog",
                    "This dialog, appearing in the Tools menu, allows you to translate the " +
                        "user interface of OurWord into another language.");

                m_Dlg.Add(ID.fStructuralEditing.ToString(),
                    false,
                    "Structural Editing",
                    "Enables the translator to split and join paragraphs, or to assign different " +
                        "styles to a paragraph. By doing this the translation will depart from " +
                        "the paragraph structure of the front translation.");

                m_Dlg.Add(ID.fJustTheBasics.ToString(),    
                    false,
                    "Just the Basics",
                    "Designed for newer computer users, this feature sets up the user interface " +
                        "to support a very basic level of work, turning off many of the features. " +
                        "The menu is hidden, and in its place the toolbar buttons show descriptive " +
                        "text in addition to their pictures. Users only need to be familiar with about " +
                        "a dozen commands.");


				// The JustTheBasics feature cannot be on if any of the following
				// are on; so when the user clicks JustTheBasics, the dialog
				// will turn these others off.
                m_Dlg.AddDependency(ID.fJustTheBasics.ToString(), ID.fProject.ToString());
                m_Dlg.AddDependency(ID.fJustTheBasics.ToString(), ID.fPropertiesDialog.ToString());
                m_Dlg.AddDependency(ID.fJustTheBasics.ToString(), ID.fJobBT.ToString());
                m_Dlg.AddDependency(ID.fJustTheBasics.ToString(), ID.fJobNaturalness.ToString());
                m_Dlg.AddDependency(ID.fJustTheBasics.ToString(), ID.fCopyBTfromFront.ToString());
                m_Dlg.AddDependency(ID.fJustTheBasics.ToString(), ID.fFilter.ToString());
                m_Dlg.AddDependency(ID.fJustTheBasics.ToString(), ID.fPrint.ToString());
                m_Dlg.AddDependency(ID.fJustTheBasics.ToString(), ID.fStructuralEditing.ToString());
                m_Dlg.AddDependency(ID.fJustTheBasics.ToString(), ID.fLocalizer.ToString());
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
		}
		#endregion
		#region Method: void OnLeaveProject()
		private void OnLeaveProject()
		{
			// Do everything we would normally do on leaving a section (e.g., getting
			// the data from the cache.)
			OnLeaveSection();

			// Prompt so that we save to disk
            if (Project.HasContent)
            {
                Cursor = Cursors.WaitCursor;
                m_Config.Save();
                Cursor = Cursors.Default;
            }
		}
		#endregion
		#region Method: void OnEnterProject()
		private void OnEnterProject()
		{
			m_Config.InitialDirectory = G.TeamSettings.DataRootPath;

			// If we aren't already navigated to a book, then attempt to find one.
			Project.Nav.GoToReasonableBook();

            // A new project may have different settings for whether or not the
            // Side Windows are displayed.
            SetupSideWindows();

            // Window Zoom factor
            SetZoomFactor();

            // Window contents, etc.
			OnEnterSection();
		}
		#endregion

        // Interface IFileMenuIO -------------------------------------------------------------
        #region Method: void TemporaryFixes()
        void TemporaryFixes()
			// Use this to behind-the-scenes upgrade Timor and Manado styles....
			// remove it for 1.0.
		{
            int nCurrentSettingsVersion = 1;

            if (G.TeamSettings.SettingsVersion < nCurrentSettingsVersion)
            {
                // Fix Hebrews being associated with the wrong grouping
                DBookGrouping.InitializeGroupings(G.TeamSettings.BookGroupings);
            }

            G.TeamSettings.SettingsVersion = nCurrentSettingsVersion;
		}
		#endregion
		#region Attr{g}: string XmlTag - e.g., the "tag" within "<tag>"
		public string XmlTag
		{
			get
			{
				return "OurWordProjectFile";
			}
		}
		#endregion
		#region Method: void New() - start with blank configuration (default values)
		public void New()
		{
			// Nothing is done here; it is all handled in cmdNewProject
		}
		#endregion
        #region Method: void Read(ref string sPathName) - read the configuration
        public void Read(ref string sPathName)
		{
            Project = new DProject();
            Project.AbsolutePathName = sPathName;
            Project.Load();

            // Make sure the registry is aware of the new file name, as it might have been changed.
            sPathName = Project.AbsolutePathName;

            TemporaryFixes();
		}
		#endregion
		#region Method: void Write(sPathName) - writes configuration to project file
		public void Write(string sPathName)
		{
			// Save the current book (in case changes have been made) We do this before
			// writing the project settings, because the name of the book may be changed
			// (e.g., a Paratext filename is saved as a DB) and we want the OTrans file
			// to reflect the proper name.
			Project.SaveCurrentBook();

            // Store the Path Name (It may be different due to a SaveAs command)
            Project.AbsolutePathName = sPathName;

            // Let JObjectOnDemand handle the rest
            Project.Write();

		}
		#endregion

		// Event Handlers --------------------------------------------------------------------
		#region Event: cmd_OnLoad(...)    - on loading the app, restore the window state
		private void cmd_OnLoad(object sender, System.EventArgs e)
		{
			// Init the Help system
            ShowLoadState("Load Help");
			HelpSystem.Initialize();

			// Populate the MRU List; read in the project
			ShowLoadState("Load MRU");
			m_Config.LoadMRUfromRegistry(true);   // Reads in the most recent project

            // Restore which layout is active
            string sPreferredWindowName = JW_Registry.GetValue("CurrentJob",
                WndDrafting.c_sName);
            if (sPreferredWindowName == WndDrafting.c_sName)
                MainWindow = WndDrafting;
            if (sPreferredWindowName == WndBackTranslation.c_sName)
                MainWindow = WndBackTranslation;
            if (sPreferredWindowName == WndNaturalness.c_sName)
                MainWindow = WndNaturalness;

			// Restore to where we last were.
			ShowLoadState("Load Window Position");
			Project.Nav.RetrievePositionFromRegistry();

			// Set up the views, make the initial selection, etc.
			ShowLoadState("Load Project into windows");
			OnEnterProject();

			// Remember the previous placement of the window on the screen (Do this late
			// in the load sequence so that we avoid a multiple screen redraw.) 
			ShowLoadState("Load Window State");
			m_WindowState.RestoreWindowState();

			// Initialize AutoSave Timer
			ShowLoadState("Load AutoSave");
			InitializeAutoSave();

			// Loading all done: Close the splash screen
            ShowLoadState("Close Splash Screen");
            SplashScreen.Close(this);

            // Leave everything in a state where the main window has focus, so that the
            // Text Selection will be appropriately flashing its readiness
            MainWindow.Focus();
            ShowLoadState("Loading Complete");
        }
		#endregion
		#region Event: cmd_OnClosing(...) - on closing the app, save the window state
		private void cmd_OnClosing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			// Save data
			OnLeaveProject();

			// Save the current project's registry info
			m_Config.SaveMRUtoRegistry();
			Project.Nav.SavePositionToRegistry();
			
			// Save the window position
			m_WindowState.SaveWindowState();

			G.TeamSettings.Write();
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
            if (null == MainWindow)
                return;
            if (m_SplitContainer.Panel1.Width == 0 || m_SplitContainer.Panel1.Height == 0)
                return;   // Happens in Mono, not in Windows

            // the MainWindow needs to have a size for its double buffering
            MainWindow.SetSize(m_SplitContainer.Panel1.Width, m_SplitContainer.Panel1.Height);

            // Re-do the layout and redraw
            MainWindow.DoLayout();
            MainWindow.Invalidate();
        }
        #endregion

        // Commands --------------------------------------------------------------------------
		#region COMMANDS
		#region Cmd: cmdNewProject
        private void cmdNewProject(Object sender, EventArgs e)
		{
            // Walk through the wizard
            Dialogs.WizNewProject.WizNewProject wiz = new Dialogs.WizNewProject.WizNewProject();
            if (DialogResult.OK != wiz.ShowDialog())
                return;

            // Make sure this current project is saved and up-to-date
            OnLeaveProject();

            // Create and initialize the new project according to the settings
            DProject project = new DProject();
            project.DisplayName = wiz.ProjectName;
            project.AbsolutePathName = wiz.TargetSettingsFolder +
                Path.DirectorySeparatorChar + project.DisplayName + ".owp";

            DTeamSettings ts = new DTeamSettings();
            ts.InitializeFactoryStyleSheet();
            project.TeamSettings = ts;
            ts.AbsolutePathName = wiz.TeamSettingsPath;
            if (File.Exists(ts.AbsolutePathName))
                ts.Load();

            DTranslation tFront = new DTranslation(wiz.FrontName, DStyleSheet.c_Latin, DStyleSheet.c_Latin);
            if (!string.IsNullOrEmpty(wiz.ExistingFrontSettingsFilePath))
            {
                tFront.AbsolutePathName = wiz.ExistingFrontSettingsFilePath;
                tFront.Load();
                if (tFront.DisplayName != wiz.FrontName ||
                    tFront.LanguageAbbrev != wiz.FrontAbbreviation ||
                    Path.GetDirectoryName(tFront.AbsolutePathName) != wiz.FrontSettingsFolder)
                {
                    tFront = new DTranslation(wiz.FrontName, DStyleSheet.c_Latin, DStyleSheet.c_Latin);
                }
            }
            project.FrontTranslation = tFront;
            tFront.LanguageAbbrev = wiz.FrontAbbreviation;
            tFront.AbsolutePathName = wiz.FrontSettingsFolder +
                Path.DirectorySeparatorChar + wiz.FrontName + ".otrans";

            DTranslation tTarget = new DTranslation(wiz.ProjectName, DStyleSheet.c_Latin, DStyleSheet.c_Latin);
            project.TargetTranslation = tTarget;
            tTarget.DisplayName = wiz.ProjectName;
            tTarget.LanguageAbbrev = wiz.TargetAbbreviation;
            tTarget.AbsolutePathName = wiz.TargetSettingsFolder +
                Path.DirectorySeparatorChar + wiz.ProjectName + ".otrans";

            Project = project;

            // Save everything, including placing into the MRU
            m_Config.FileName = Project.AbsolutePathName;
            m_Config.mru_Update();
            m_Config.Save();

            // Update the UI, views, etc
            Project.Nav.GoToFirstAvailableBook();
            OnEnterProject();

            // Edit properties?
            if (wiz.LaunchPropertiesDialogWhenDone)
                cmdProjectProperties(null, null);


            /***
            // OLD
			// Make sure this current project is saved and up-to-date
			OnLeaveProject();

			// Set the Config to a new project
			m_Config.New();

			// Create a new, blank project
			Project = new DProject();

			// When we go to save the project, we'll default to something meaningful
			m_Config.DefaultFileName = Project.DisplayName + ".owp";

            // Save the project to disk (otherwise, the Relative Pathnames for further
            // down objects will not work.
            if (!m_Config.SaveAs("Enter a filename for this new project"))
            {
                OnEnterProject();
                return;
            }

			// Update the UI to the new project
			OnEnterProject();

			// Go ahead and edit its properties (thus present the Properties dialog)
            DialogProperties dlg = new DialogProperties();
            dlg.ShowDialog(this);

			// Update the UI, views, etc.
			Project.Nav.GoToFirstAvailableBook();
			OnEnterProject();
            ***/
		}
		#endregion
		#region Cmd: cmdOpenProject
        private void cmdOpenProject(Object sender, EventArgs e)
		{
			// Make sure this project is saved and up-to-date
			OnLeaveProject();

			// Present the dialog to find the desired settings file; then calls
			// IFileMenuIO.Read to bring it in.
			m_Config.Open();

			// Update the UI, views, etc.
			OnEnterProject();
		}
		#endregion
		#region Cmd: cmdSaveProject
        private void cmdSaveProject(Object sender, EventArgs e)
		{
			OnLeaveSection();
			m_Config.InitialDirectory = G.BrowseDirectory;
            m_Config.Save();
		}
		#endregion
		#region Cmd: cmdSaveProjectAs
        private void cmdSaveProjectAs(Object sender, EventArgs e)
		{
			OnLeaveSection();
			m_Config.InitialDirectory = G.BrowseDirectory;
			m_Config.SaveAs("Save this Project as");
		}
		#endregion
		#region Cmd: cmdPrint
        private void cmdPrint(Object sender, EventArgs e)
		{
			OnLeaveSection();

			Print p = new Print();
			p.Do();
		}
		#endregion
		#region Cmd: cmdProjectProperties
        private void cmdProjectProperties(Object sender, EventArgs e)
		{
			// Retrieve data and save the project to disk. We don't know if the
			// user might remove the book from the project, so we need to make sure
			// it was saved just in case.
			OnLeaveProject();

            // Let the user change the properties
            DialogProperties dlg = new DialogProperties();
            dlg.ShowDialog(this);

            // The zoom factor may have changed, so we need to recalculate the fonts
            SetZoomFactor();

            // Re-initialize everything
			OnEnterProject();
		}
		#endregion
        #region Cmd: cmdExit
        private void cmdExit(Object sender, EventArgs e)
		{
			Application.Exit();		
		}
		#endregion
		#region Cmd: cmdMRU - opens Project from MRU list
        private void cmdMRU(Object sender, EventArgs e)
        {
            OnLeaveProject();
            m_Config.LoadMruItem( (sender as ToolStripMenuItem).Text );
            Project.Nav.GoToFirstAvailableBook();
            OnEnterProject();
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

		#region Can: canInsertNote
		public bool canInsertNote
		{
			get
			{
                // If there is no Notes pane in the side window, then we can't
                // insert any notes
                Debug.Assert(null != SideWindows);
                if (!SideWindows.HasNotesWindow)
                    return false;

                // If the Main Window is not focused, then we don't have a
                // context for inserting notes.
                Debug.Assert(null != MainWindow);
                if (!MainWindow.Focused)
                    return false;

                return true;
			}
		}
		#endregion
		#region Cmd: cmdInsertNote
        private void cmdInsertNote(Object sender, EventArgs e)
        {
            string sNoteType = (string)((sender as ToolStripItem).Tag);

            for (DNote.Types k = 0; k != DNote.Types.kUnknown; k++)
            {
                if (k.ToString() == sNoteType)
                {
                    _InsertNote(k);
                    return;
                }
            }
        }
        private void _InsertNote(DNote.Types type)
		{
            // Double-check that we think we can insert. In theory the menu
            // item would have been disabled and so we would not get here if
            // canInsertNote returns false.
            if (!canInsertNote)
                return;

            // Retrieve the DText to which this note will be attached
            if (MainWindow.Selection == null)
                return;
            DText text = MainWindow.Selection.Anchor.BasicText as DText;
            if (null == text)
                return;

            // Insert the note
            DNote note = text.InsertNote(type, MainWindow.Selection.SelectionString);
            if (null != note)
            {
                // Update the display
                ResetWindowContents();

                // Select the new note and bring the focus to it
                SideWindows.NotesWindow.SelectEndOfNote(note);
                SideWindows.NotesWindow.Focus();
            }
		}
		#endregion
		#region Can: canDeleteNote
		public bool canDeleteNote
		{
			get
			{
                // If there is no Notes pane in the side window, then we can't
                // delete any notes
                if (!SideWindows.HasNotesWindow)
                    return false;

                // If the notes window does not have focus, then we can't
                // delete notes, because the editing context is not there.
                //
                // By implication, if there is focus here, then it means
                // we have an InsertionPoint, which thus means we have
                // a Note that can conceivably be deleted.
                if (!SideWindows.NotesWindow.Focused)
                    return false;

                // Otherwise, we return a tentative "true", recognizing that the
                // Delete method will have to do further error condition checking.
                return true;
			}
		}
		#endregion
		#region Cmd: cmdDeleteNote
        private void cmdDeleteNote(Object sender, EventArgs e)
		{
            // Double-check that we think we can delete. In theory the menu
            // item would have been disabled and so we would not get here if
            // canDeleteNote returns false.
            if (!canDeleteNote)
                return;

            // Request the targeted DNote from the NotesWindow
            Debug.Assert(null != SideWindows);
            Debug.Assert(null != SideWindows.NotesWindow);
            DNote note = SideWindows.NotesWindow.GetSelectedNote();
            if (null == note)
                return;

            // Give the user the opportunity to change his/her mind
            string sText = note.NoteText.ContentsAsString;
            if (sText.Length > 40)
                sText = sText.Substring(0, 40) + "...";
            string sMsgAddition = "\n\n\"" + sText + "\"";
            if (false == Messages.ConfirmNoteDeletion(sMsgAddition))
                return;

            // Remove the note from the paragraph (we delete it from the
            // owning DText)
            DText text = note.Text;
            Debug.Assert(null != text);
            text.Notes.Remove(note);

            // Regenerate the windows
            ResetWindowContents();
		}
		#endregion

		#region Cmd: cmdGoToFirstSection
        private void cmdGoToFirstSection(Object sender, EventArgs e)
		{
            // Ignore if we're at the beginning already
            if (Project.Nav.IsAtFirstSection)
                return;

            // Go to the first section
            OnLeaveSection();
			Project.Nav.GoToFirstSection();
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
                if (Project.Nav.IsAtFirstSection)
                    return;

                // Go to the previous section
                OnLeaveSection();
                Project.Nav.GoToPreviousSection();
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
                if (Project.Nav.IsAtLastSection)
                    return;

                // Go to the next section
                OnLeaveSection();
                Project.Nav.GoToNextSection();
                OnEnterSection();
            }
		}
		#endregion
		#region Cmd: cmdGoToLastSection
        private void cmdGoToLastSection(Object sender, EventArgs e)
		{
            // Ignore if we're at the end already
            if (Project.Nav.IsAtLastSection)
                return;

            // Go to the final section
            OnLeaveSection();
			Project.Nav.GoToLastSection();
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
			DBook book = Project.SFront.Book;
			int i = 0;
			foreach(DSection section in book.Sections)
			{
				string sReference = section.ReferenceSpan.Start.FullName + " ";

                if (sMenuText.StartsWith(sReference))
				{
					Project.Nav.GoToSection(i);
					OnEnterSection();
					break;
				}
				i++;
			}
		}
		#endregion
		#region Cmd: cmdGotoBook
        private void cmdGotoBook(Object sender, EventArgs e)
        {
            // Retrieve the target book from the menu item's text
            ToolStripMenuItem mi = (sender as ToolStripMenuItem);
            Debug.Assert(null != mi);
            string sBookDisplayName = mi.Text;
            Debug.Assert(!string.IsNullOrEmpty(sBookDisplayName));

            // Go to that book
            OnLeaveSection();
            Project.Nav.GoToBook(sBookDisplayName);
            OnEnterSection();
        }
		#endregion

        #region Cmds: cmdToggleNotesPane, cmdToggleOtherTranslationsPane
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

        #region Cmd: cmdToggleNotesPane
        private void cmdToggleNotesPane(Object sender, EventArgs e)
		{
            DProject.VD_ShowNotesPane = !DProject.VD_ShowNotesPane;

            _UpdateSideWindows();
		}
		#endregion
        #region Cmd: cmdToggleOtherTranslationsPane
        private void cmdToggleOtherTranslationsPane(Object sender, EventArgs e)
        {
            DProject.VD_ShowTranslationsPane = !DProject.VD_ShowTranslationsPane;

            _UpdateSideWindows();
        }
        #endregion
        #endregion

        #region Cmd: cmdJobDrafting
        private void cmdJobDrafting(Object sender, EventArgs e)
		{
            MainWindow = WndDrafting;
		}
		#endregion
		#region Cmd: cmdJobBackTranslation
        private void cmdJobBackTranslation(Object sender, EventArgs e)
		{
            MainWindow = WndBackTranslation;
		}
		#endregion
        #region Cmd: cmdJobNaturalness
        private void cmdJobNaturalness(Object sender, EventArgs e)
        {
            MainWindow = WndNaturalness;
        }
        #endregion

		#region Can: canIncrementBookStatus
        private bool canIncrementBookStatus
		{
			get
			{
				if (null == Project)
					return false;
				if (null == Project.FrontTranslation)
					return false;
				if (null == Project.TargetTranslation)
					return false;
				if (null == Project.Nav.STarget)
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
			DBook book = Project.STarget.Book;
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
		}
		#endregion
		#region Cmd: cmdRestoreBackup
        private void cmdRestoreBackup(Object sender, EventArgs e)
		{
			if (!canIncrementBookStatus)
				return;

			DBook BTarget = Project.STarget.Book;

			DialogRestoreFromBackup dlg = new DialogRestoreFromBackup(BTarget);
			if (DialogResult.OK == dlg.ShowDialog())
			{
				OnLeaveProject();

				DBook.RestoreFromBackup(BTarget, dlg.BackupPathName);

				Project.Nav.GoToFirstSection();
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
				m_dlgFilter.Filter_NoteToDo,
				m_dlgFilter.Filter_NoteAskUNS,
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
			if (null == G.STarget)
				return;
			if (!DSection.FilterIsActive)
				return;
			if (!G.STarget.MatchesFilter)
				return;

			// Run the test on the current section to update it.
			Project.STarget.MatchesFilter = FilterTest(Project.STarget);

			// If the filter no longer matches this section...
			if (!Project.STarget.MatchesFilter)
			{
				// Are there any other sections that still match the filter?
				bool bNoMatches = true;
				foreach(DSection section in G.TBook.Sections)
				{
					if (section.MatchesFilter)
						bNoMatches = false;
				}

				// If not, then we must turn off filters
				if (bNoMatches)
				{
					G.TBook.RemoveFilter();
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
			if (!G.IsValidProject)
				return;

			// Get the user's pleasure
			if (null == m_dlgFilter)
				m_dlgFilter = new DialogFilter();
			DialogResult result = m_dlgFilter.ShowDialog(this);

			// If the user Canceled out, then we remove any existing filter
			if (result == DialogResult.Cancel || m_dlgFilter.NothingIsChecked)
			{
				G.TBook.RemoveFilter();
				G.StatusSecondaryMessage = "";
                SetupMenusAndToolbarsVisibility();    // Reset the Navigation toolbar popups
				return;
			}

			// Calculate the filter according to the user's requests, and position
			// at the first match.
            G.ProgressStart(
                G.GetLoc_String("strCalculatingMatchingSections", "Calculating matching sections..."), 
                G.TBook.Sections.Count);
			OnLeaveSection();
			int cMatches = 0;
			foreach(DSection section in G.TBook.Sections)
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
				G.ProgressStep();
			}
			G.ProgressEnd();

			// If at least one section passed, then turn on filters and position at a
			// matching section (if we aren't already at one.).
			if (cMatches > 0)
			{
				DSection.FilterIsActive = true;
                G.StatusSecondaryMessage = G.GetLoc_String("strViewingASubset", "Viewing a Subset.");
				if (!G.STarget.MatchesFilter)
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
		#region Cmd: cmdCopyBTfromFront_Book/Section

        private void cmdCopyBTfromFront_Book(Object sender, EventArgs e)
        {
            _cmdCopyBTfromFront(true);
        }
        private void cmdCopyBTfromFront_Section(Object sender, EventArgs e)
        {
            _cmdCopyBTfromFront(false);
        }

		private void _cmdCopyBTfromFront(bool bEntireBook)
		{
            if (!MainWindowIsBackTranslation)
                return;

			OnLeaveSection();
			Project.STarget.Book.CopyBackTranslationFromFront(bEntireBook);
			OnEnterSection();
		}
		#endregion

		#region Cmd: cmdSetUpFeatures
        private void cmdSetUpFeatures(Object sender, EventArgs e)
		{
			if (true == s_Features.ShowDialog(this) )
			{
				OnLeaveSection();
                SetupMenusAndToolbarsVisibility();
				OnEnterSection();
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
            OnLeaveProject();

            Localizer dlg = new Localizer(LocDB.DB);
            if (DialogResult.OK == dlg.ShowDialog())
                OnEnterProject();
        }
        #endregion

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

		#region Can: canItalic
		public bool canItalic
		{
			get
			{
/***
				if (null == CurrentLayout)
					return false;
				return CurrentLayout.canItalicizeSelection;
***/
return false;
			}
		}
		#endregion
		#region Cmd: cmdItalic
        private void cmdItalic(Object sender, EventArgs e)
		{
/***
			if (null == CurrentLayout)
				return;
			CurrentLayout.cmdItalicizeSelection();
***/
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
        const string c_keyPictureSearchPath = "PictureSearchPath";
        const string c_keySuppressVerseNumbers = "SuppressVerseNos";
        const string c_keyShowLineNumbers = "ShowLineNumbers";
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
        #region SAttr{g/s}: string PictureSearchPath
        static public string PictureSearchPath
        {
            get
            {
                return JW_Registry.GetValue(c_sSubKey, c_keyPictureSearchPath, "");
            }
            set
            {
                JW_Registry.SetValue(c_sSubKey, c_keyPictureSearchPath, value);
            }
        }
        #endregion
        #region SAttr{g/s}: bool SupressVerseNumbers
        public static bool SupressVerseNumbers
        {
            get
            {
                if (-1 == s_nSupressVerseNumbers)
                {
                    s_nSupressVerseNumbers = JW_Registry.GetValue(c_sSubKey, 
                        c_keySuppressVerseNumbers, 0);
                }
                return (s_nSupressVerseNumbers == 1) ? true : false;
            }
            set
            {
                s_nSupressVerseNumbers = (value == true) ? 1 : 0;
                JW_Registry.SetValue(c_sSubKey, c_keySuppressVerseNumbers, s_nSupressVerseNumbers);
            }
        }
        static int s_nSupressVerseNumbers = -1;
        #endregion
        #region SAttr{g/s}: bool ShowLineNumbers
        public static bool ShowLineNumbers
        {
            get
            {
                if (-1 == s_nShowLineNumbers)
                {
                    s_nShowLineNumbers = JW_Registry.GetValue(c_sSubKey,
                        c_keyShowLineNumbers, 0);
                }
                return (s_nShowLineNumbers == 1) ? true : false;
            }
            set
            {
                s_nShowLineNumbers = (value == true) ? 1 : 0;
                JW_Registry.SetValue(c_sSubKey, c_keyShowLineNumbers, s_nShowLineNumbers);
            }
        }
        static int s_nShowLineNumbers = -1;
        #endregion

        // Stuff still in OurWordMain
		#region Attr{g}: DProject Project - the current project we're editing / displaying / etc.
		static public DProject Project 
		{ 
			get 
			{ 
				return OurWordMain.Project; 
			} 
		}
		#endregion
		#region Attr{g}: static DTeamSettings TeamSettings - global settings for, e.g., the TimorTeam
		static public DTeamSettings TeamSettings 
		{ 
			get 
			{ 
				return Project.TeamSettings; 
			} 
		}
		#endregion
        #region Attr{g}: OurWordMain App
        static public OurWordMain App
        {
            get
            {
                return OurWordMain.App;
            }
        }
        #endregion

        // Parts of a Project ----------------------------------------------------------------
		#region Attr{g}: DTranslation FTranslation
		static public DTranslation FTranslation
		{
			get
			{
				if (null == Project)
					return null;
				return Project.FrontTranslation;
			}
		}
		#endregion
		#region Attr{g}: DTranslation TTranslation
		static public DTranslation TTranslation
		{
			get
			{
				if (null == Project)
					return null;
				return Project.TargetTranslation;
			}
		}
		#endregion
		#region Attr{g}: DBook FBook
		static public DBook FBook
		{
			get
			{
				if (null == Project || null == Project.SFront)
					return null;
				return Project.SFront.Book;

			}
		}
		#endregion
		#region Attr{g}: DBook TBook
		static public DBook TBook
		{
			get
			{
				if (null == Project || null == Project.STarget)
					return null;
				return Project.STarget.Book;

			}
		}
		#endregion
		#region Attr{g}: bool TBookIsLocked - T if target book is Locked
		static public bool TBookIsLocked
		{
			get
			{
				if (null == TBook)
					return false;
				return TBook.Locked;
			}
		}
		#endregion

		#region Attr{g}: DSection STarget
		static public DSection STarget
		{
			get
			{
				if (null == Project)
					return null;
				return Project.STarget;

			}
		}
		#endregion
		#region Attr{g}: DSection SFront
		static public DSection SFront
		{
			get
			{
				if (null == Project)
					return null;
				return Project.SFront;

			}
		}
		#endregion
		#region Attr{g}: bool IsValidProject
		static public bool IsValidProject
		{
			get
			{
				if (null == Project)
					return false;
				if (null == FBook)
					return false;
				if (null == TBook)
					return false;
				return true;
			}
		}
		#endregion

		// Styles & Markers ------------------------------------------------------------------
		#region Attr{g}: static public DSFMapping Map - Styles <> Sf Markers
		public static DSFMapping Map
		{
			get
			{
				return TeamSettings.SFMapping;
			}
		}
		#endregion
		#region Attr{g}: static public DStyleSheet StyleSheet
		static public DStyleSheet StyleSheet
		{
			get
			{
				return TeamSettings.StyleSheet;
			}
		}
		#endregion
		#region Attr{g}: static BookStages TranslationStages
		static public BookStages TranslationStages
		{
			get
			{
				return G.TeamSettings.TranslationStages;
			}
		}
		#endregion

		// Status Bar ------------------------------------------------------------------------
		#region Method: void ProgressStart(string sProgressMessage, int nCount)
		public static void ProgressStart(string sProgressMessage, int nCount)
		{
            if (null != OurWordMain.App)
            {
                OurWordMain.App.StatusMessage1 = sProgressMessage;
                ToolStripProgressBar bar = OurWordMain.App.ProgressBar;
                bar.Invalidate();
                bar.Minimum = 0;
                bar.Maximum = nCount;
                bar.Value = 0;
                bar.Step = 1;
                bar.Visible = true;
                SetWaitCursor();
            }
		}
		#endregion
		#region Method: void ProgressStep()
		public static void ProgressStep()
		{
            if (null != OurWordMain.App)
            {
                OurWordMain.App.ProgressBar.PerformStep();
            }
		}
		#endregion
		#region Method: void ProgressEnd()
		public static void ProgressEnd()
		{
            if (null != OurWordMain.App)
            {
                SetNormalCursor();
                OurWordMain.App.ProgressBar.Visible = false;
                OurWordMain.App.StatusMessage1 = "";
            }
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

        // Wait Cursor -----------------------------------------------------------------------
		#region Method: void SetWaitCursor()
		static public void SetWaitCursor()
		{
			OurWordMain.App.Cursor = Cursors.WaitCursor;
		}
		#endregion
		#region Method: void SetNormalCursor()
		static public void SetNormalCursor()
		{
			OurWordMain.App.Cursor = Cursors.Default;
		}
		#endregion

		// Browse Directory ------------------------------------------------------------------
		#region Attr{g/s}: string BrowseDirectory - last navigated-to directory (runtime only)
		public static string BrowseDirectory
		{
			get
			{
				if ( s_sBrowseDirectory.Length == 0 )
					s_sBrowseDirectory = G.TeamSettings.DataRootPath;
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
        #region SAttr{g}: string Today - returns today's date as "2005-08-21" format.
        static public string Today
        {
            get
            {
                DateTime dt = DateTime.Today;
                return dt.ToString("yyyy-MM-dd");
            }
        }
        #endregion
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

                return "Beta " + sVersionNo;
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
        #region SMethod: string GetLoc_Notes(sItemID, sEnglish) -         "Strings\Notes"
        static public string GetLoc_Notes(string sItemID, string sEnglishDefault)
        {
            return LocDB.GetValue(
                new string[] { "Strings", "Notes" },
                sItemID,
                sEnglishDefault,
                null,
                null);
        }
        #endregion
        #region SMethod: string GetLoc_NoteDefs(sItemID, sEnglish) -         "Strings\NoteDefs"
        static public string GetLoc_NoteDefs(string sItemID, string sEnglishDefault)
        {
            return LocDB.GetValue(
                new string[] { "Strings", "NoteDefs" },
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

        #region SMethod: string GetLoc_BookAbbrev(string sBookAbbrev) -   "BookAbbrevs"
        static public string GetLoc_BookAbbrev(string sBookAbbrev)
        {
            return LocDB.GetValue(
                new string[] { "BookAbbrevs" },
                sBookAbbrev,
                sBookAbbrev,
                G.TeamSettings.FileNameLanguage,
                null);
        }
        #endregion
        #region SMethod: string GetLoc_BookGroupings(sItemID, sEnglish) - "BookGroupings"
        static public string GetLoc_BookGroupings(string sItemID, string sEnglishDefault)
        {
            return LocDB.GetValue(
                new string[] { "BookGroupings" },
                sItemID,
                sEnglishDefault,
                null,
                null);
        }
        #endregion
        #region SMethod: string GetLoc_Messages(sItemID, sEnglish) -      "Messages"
        static public string GetLoc_Messages(string sItemID, string sEnglishDefault)
        {
            return LocDB.GetValue(
                new string[] { "Messages" },
                sItemID,
                sEnglishDefault,
                null,
                null);
        }
        #endregion
        #region SMethod: string GetLoc_Messages(sItemID, sEnglish) -      "Messages\FileStructureMessages"
        static public string GetLoc_StructureMessages(string sItemID, string sEnglishDefault)
        {
            return LocDB.GetValue(
                new string[] { "Messages", "FileStructureMessages" },
                sItemID,
                sEnglishDefault,
                null,
                null);
        }
        #endregion
        #region SMethod: string GetLoc_TranslationStage(sItemID, sEnglish) - "TranslationStages" 
        static public string GetLoc_TranslationStage(string sItemID, string sEnglish)
        {
            return LocDB.GetValue(
                new string[] { "TranslationStages" },
                sItemID,
                sEnglish,
                G.TeamSettings.FileNameLanguage,
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
}






