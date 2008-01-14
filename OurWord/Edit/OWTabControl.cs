/**********************************************************************************************
 * Project: OurWord!
 * File:    OWTabControl.cs
 * Author:  John Wimbish
 * Created: 21 Mar 2007
 * Purpose: Tab Control for displaying the various OW Side-Windows (e.g., Notes)
 * Legal:   Copyright (c) 2004-08, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
#region Using
using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Timers;
using System.Windows.Forms;
using JWTools;
using JWdb;
using OurWord.DataModel;
using OurWord.View;
using OurWord.Edit;
using NUnit.Framework;
#endregion

namespace OurWord.Edit
{

    public class SideWindows : TabControl
    {
        // Individual Windows (Pages) --------------------------------------------------------
        #region Attr{g}: int PagesCount
        public int PagesCount
        {
            get
            {
                return TabPages.Count;
            }
        }
        #endregion
        #region Method: void Clear() - remove all TabPages
        public void Clear()
        {
            TabPages.Clear();
        }
        #endregion
        #region Method: void AddPage(string sKey, string sTitle)
        public void AddPage(string sKey, string sTitle)
        {
            TabPage page = new TabPage();

            page.Name = sKey;
            page.Text = sTitle;

            TabPages.Add(page);
        }
        #endregion

        // Scaffolding -----------------------------------------------------------------------
        #region Constructor()
        public SideWindows()
            : base()
        {
            // Dock on the right side of the OurWord window
            Dock = DockStyle.Fill;

            // Allow multiple rows of tabs if needed
            Multiline = true;
        }
        #endregion
        #region Method: void RegisterWindows(OWWindow wndMain) - tell Main what windows are present
        public void RegisterWindows(OWWindow wndMain)
        {
            Debug.Assert(null != wndMain);

            wndMain.ResetSecondaryWindows();

            if (HasNotesWindow)
                wndMain.RegisterSecondaryWindow(NotesWindow);
            if (HasTranslationsWindow)
                wndMain.RegisterSecondaryWindow(TranslationsWindow);

        }
        #endregion
        #region Method: void SetChildrenSizes()
        public void SetChildrenSizes()
        {
            if (HasNotesWindow)
            {
                NotesWindow.SetSize(DisplayRectangle.Width, DisplayRectangle.Height);
            }
            if (HasTranslationsWindow)
            {
                TranslationsWindow.SetSize(DisplayRectangle.Width, DisplayRectangle.Height);
            }
        }
        #endregion
        #region Method: void SetZoomFactor(float fZoomFactor)
        public void SetZoomFactor(float fZoomFactor)
        {
            if (HasNotesWindow)
                NotesWindow.ZoomFactor = fZoomFactor;
            if (HasTranslationsWindow)
                TranslationsWindow.ZoomFactor = fZoomFactor;
        }
        #endregion
        #region Attr{g}: OWWindow FocusedWindow
        public OWWindow FocusedWindow
        {
            get
            {
                if (HasNotesWindow && NotesWindow.Focused)
                    return NotesWindow;
                if (HasTranslationsWindow && TranslationsWindow.Focused)
                    return TranslationsWindow;
                return null;
            }
        }
        #endregion

        // Notes Window ----------------------------------------------------------------------
 //       NotesTabPage m_NotesTabPage;
        #region Attr{g}: NotesWindow NotesWindow
        public NotesWindow NotesWindow
        {
            get
            {
                Debug.Assert(null != m_NotesWindow);
                return m_NotesWindow;
            }
        }
        NotesWindow m_NotesWindow = null;
        #endregion
        #region Method: void CreateNotesWindow()
        public void CreateNotesWindow()
        {
            // Create a Notes Window
            m_NotesWindow = new NotesWindow();

            // Create a TabPage to contain it
            TabPage page1 = new TabPage();
            page1.Name = "Notes";
            page1.Text = NotesWindow.WindowName;
            page1.Controls.Add(m_NotesWindow);

            // Add it to the TabControl
            TabPages.Add(page1);

            // ---NEW--------------------------
            /***

            // Create the Notes Control
            NotesTabPage m_NotesTabPage = new NotesTabPage();
            m_NotesWindow = m_NotesTabPage.NotesWindow;

            // Create a TabPage to contain it
            TabPage page = new TabPage();
            page.Name = "Notes";
            page.Text = NotesWindow.WindowName;
            page.Controls.Add(m_NotesTabPage);

            // Add it to the TabControl
            TabPages.Add(page);
            ***/
        }
        #endregion
        #region Attr{g}: bool HasNotesWindow
        public bool HasNotesWindow
        {
            get
            {
                return (m_NotesWindow != null);
            }
        }
        #endregion

        // Translations Window ---------------------------------------------------------------
        #region Attr{g}: TranslationsWindow TranslationsWindow
        public TranslationsWindow TranslationsWindow
        {
            get
            {
                Debug.Assert(null != m_TranslationsWindow);
                return m_TranslationsWindow;
            }
        }
        TranslationsWindow m_TranslationsWindow = null;
        #endregion
        #region Method: void CreateTranslationsWindow()
        public void CreateTranslationsWindow()
        {
            // Create a Notes Window
            m_TranslationsWindow = new TranslationsWindow();

            // Create a TabPage to contain it
            TabPage page = new TabPage();
            page.Name = "Translations";
            page.Text = TranslationsWindow.WindowName;
            page.Controls.Add(m_TranslationsWindow);

            // Add it to the TabControl
            TabPages.Add(page);
        }
        #endregion
        #region Attr{g}: bool HasTranslationsWindow
        public bool HasTranslationsWindow
        {
            get
            {
                return (m_TranslationsWindow != null);
            }
        }
        #endregion

        // Commands from the owner -----------------------------------------------------------
        #region Cmd: OnResize
        protected override void OnResize(EventArgs e)
        {
            // Console.WriteLine("TAB RESIZED - Width = " + Width.ToString());
            base.OnResize(e);

            // Ignore the message if we're minimized
            if (null != G.App && G.App.WindowState == FormWindowState.Minimized)
                return;

            SetChildrenSizes();
        }
        #endregion

        // Active (selected) Tab -------------------------------------------------------------
        #region Method: void SelectFirstTab()
        public void SelectFirstTab()
        {
            // If there are no Tabs, then we can do nothing.
            if (TabPages.Count == 0)
                return;

            // Select the first tab
            SelectedTab = TabPages[0];
            SelectedTab.Focus();
        }
        #endregion
        #region Method: void SelectLastTab()
        public void SelectLastTab()
        {
            // If there are no Tabs, then we can do nothing.
            if (TabPages.Count == 0)
                return;

            // Select the final tab
            SelectedTab = TabPages[TabPages.Count - 1];
            SelectedTab.Focus();
        }
        #endregion
        #region Method: void CycleTabToNextWindow()
        public void CycleTabToNextWindow()
        {
            // If there are no Tabs, then we can do nothing.
            if (TabPages.Count == 0)
                return;

            // If we're already at the final Tab, then focus the main window
            if (SelectedTab == TabPages[TabPages.Count - 1])
            {
                G.App.MainWindow.Focus();
                return;
            }

            // Otherwise, cycle through to the next Tab
            DeselectTab(SelectedTab);
            SelectedTab.Focus();
        }
        #endregion
        #region Method: void CycleTabToPreviousWindow()
        public void CycleTabToPreviousWindow()
        {
            // If there are no Tabs, then we can do nothing.
            if (TabPages.Count == 0)
                return;

            // If we're already at the first Tab, then focus the main window
            if (SelectedTab == TabPages[0])
            {
                G.App.MainWindow.Focus();
                return;
            }

            // Otherwise, cycle through to the previous Tab
            SelectedIndex = SelectedIndex - 1;
            SelectedTab.Focus();
        }
        #endregion
        #region Cmd: OnKeyDown
        protected override void OnKeyDown(KeyEventArgs ke)
        {
            // The correct combination of Tab key means we want to cycle through the tabs
            if (ke.KeyCode == Keys.Tab)
            {
                if (ke.Modifiers == Keys.Control)
                {
                    CycleTabToNextWindow();
                    ke.Handled = true;
                    return;
                }
                if (ke.Modifiers == (Keys.Shift | Keys.Control))
                {
                    CycleTabToPreviousWindow();
                    ke.Handled = true;
                    return;
                }
            }

            // Otherwise, proceed with normal processing
            base.OnKeyDown(ke);
        }
        #endregion
    }

    public class NotesWindow : OWWindow
    {
        #region SAttr{g/s}: string RegistryBackgroundColor - background color setting for this type of window
        static public string RegistryBackgroundColor
        {
            get
            {
                return OWWindow.GetRegistryBackgroundColor(c_sName, "LightGray");
            }
            set
            {
                OWWindow.SetRegistryBackgroundColor(c_sName, value);
            }
        }
        #endregion

        // Scaffolding -----------------------------------------------------------------------
        const string c_sName = "Notes";
        const int c_cColumnCount = 1;
        #region Constructor()
        public NotesWindow()
            : base(c_sName, c_cColumnCount)
        {
            // We want to maintain a tiny bit of text below the cursor so the user does not think
            // there is nothing below when there actually is.
            ScrollPositionBufferMargin = 20;
        }
        #endregion
        #region Cmd: OnGotFocus - make sure commands are properly enabled
        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
            G.App.EnableMenusAndToolbars();
        }
        #endregion
        #region Attr{g}: override string WindowName
        public override string WindowName
        {
            get
            {
                return G.GetLoc_GeneralUI("NotesWindowName", "Notes");
            }
        }
        #endregion

        // Secondary Window Messaging --------------------------------------------------------
        #region Method: override void OnAddNote(DNote note, bool bIsEditable)
        protected override void OnAddNote(DNote note, bool bIsEditable)
        {
            // Add the note to its own unique row
            StartNewRow();
            OWPara.Flags options = (bIsEditable) ? OWPara.Flags.IsEditable : OWPara.Flags.None;
            AddNote(note, options);
        }
        #endregion
        #region Method: override void OnSelectAndScrollToNote(DNote note)
        public override void OnSelectAndScrollToNote(DNote note)
        {
            foreach (Row r in Rows)
            {
                // Retrieve the one and only pile in this row
                if (r.Piles.Length == 0)
                    continue;
                Row.Pile pile = r.Piles[0];

                // Examine all of the OWPara's in this pile (should just be one)
                foreach (OWPara para in pile.Paragraphs)
                {
                    // If we find the paragraph which represents the note, attempt
                    // to select with it (if we can, then focus this window); at
                    // any rate, we're done.
                    if (para.DataSource == note)
                    {
                        if (para.Select_BeginningOfFirstWord())
                            Focus();
                        return;
                    }
                }
            }
        }
        #endregion

        // Misc Methods ----------------------------------------------------------------------
        #region Method: DNote GetSelectedNote()
        public DNote GetSelectedNote()
        {
            if (Selection == null)
                return null;

            DNote note = Selection.Paragraph.DataSource as DNote;
            return note;
        }
        #endregion
        #region Method: void SelectEndOfNote(DNote note)
        public void SelectEndOfNote(DNote note)
        {
            foreach (Row row in Rows)
            {
                foreach (Row.Pile pile in row.Piles)
                {
                    foreach (OWPara para in pile.Paragraphs)
                    {
                        if (para.DataSource == note)
                        {
                            para.Select_EndOfLastWord();
                            return;
                        }
                    }
                }
            }
        }
        #endregion
    }

    public class TranslationsWindow : OWWindow
    {
        #region SAttr{g/s}: string RegistryBackgroundColor - background color setting for this type of window
        static public string RegistryBackgroundColor
        {
            get
            {
                return OWWindow.GetRegistryBackgroundColor(c_sName, "LightGray");
            }
            set
            {
                OWWindow.SetRegistryBackgroundColor(c_sName, value);
            }
        }
        #endregion

        // Scaffolding -----------------------------------------------------------------------
        const string c_sName = "RelatedLanguages";
        const int c_cColumnCount = 1;
        #region Constructor()
        public TranslationsWindow()
            : base(c_sName, c_cColumnCount)
        {
        }
        #endregion
        #region Attr{g}: override string WindowName
        public override string WindowName
        {
            get
            {
                return G.GetLoc_GeneralUI("TranslationsWindowName", "Translations");
            }
        }
        #endregion

        // Secondary Window Messaging --------------------------------------------------------
        #region Attr{g}: DBasicText BasicText - vernacular's DBasicText on which to build wnd contents
        DBasicText BasicText
        {
            get
            {
                return m_BasicText;
            }
        }
        DBasicText m_BasicText = null;
        #endregion

        #region Method: void OnSelectionChanged(DBasicText)
        public override void OnSelectionChanged(DBasicText dbt)
        {
            // Do nothing if we're in the same DBasicText (an optimization)
            if (dbt == BasicText)
                return;

            // Set to the new basic text
            m_BasicText = dbt;

            // Refresh the window contents
            BuildContents();
        }
        #endregion

        #region Method: DRun[] _GetSynchronizedSiblingText(...)
        DRun[] _GetSynchronizedSiblingText(DTranslation TSibling, DBasicText dbt)
        {			
            // Get the book (load it if necessary)
            DBook BSibling = G.Project.Nav.GetLoadedBook(TSibling, 
                G.Project.SFront.Book.BookAbbrev);
            if (null == BSibling)
                return null;

            // Test to see if the sections match the front translation
            if (!BSibling.AllSectionsMatchFront)
                return null;

            // See if, for the section we are interested in, the paragraphs match up
            // with the front.
            DSection SSibling = G.Project.Nav.GetSection(TSibling);
            if (null == SSibling)
                return null;
            if (!SSibling.AllParagraphsMatchFront)
                return null;

            // Find which vernacular paragraph contains the basic text
            DParagraph pOwner = dbt.Paragraph;
            int iP = pOwner.Section.Paragraphs.FindObj(pOwner);
            int iF = pOwner.Section.Footnotes.FindObj(pOwner);

            // Find the corresponding sibling paragraph
            DParagraph pSibling = null;
            if (iP != -1)
                pSibling = SSibling.Paragraphs[iP] as DParagraph;
            if (iF != -1)
                pSibling = SSibling.Footnotes[iF] as DParagraph;
            if (pSibling == null)
                return null;

            // Find the corresponding DBasicText
            if (pOwner.Runs.Count != pSibling.Runs.Count)
                return null;
            if (pOwner.StructureCodes != pSibling.StructureCodes)
                return null;
            int iDBT = pOwner.Runs.FindObj(dbt);
            if (iDBT == -1)
                return null;
            DRun rSibling = pSibling.Runs[iDBT] as DRun;

            // Create and return the vector
            DRun[] v = new DRun[1];
            v[0] = rSibling;
            return v;
        }
        #endregion

        #region Method: DRun[] _GetReferenceLanguageParagraphs(DTranslation t)
        DRun[] _GetReferenceLanguageParagraphs(DTranslation t)
        {
            // Get this paragraph's reference
            DParagraph pOwner = BasicText.Owner as DParagraph;

            // Get the reference book, loading it if necessary
            string sBookAbbrev = G.Project.Nav.BookAbbrev;
            DBook book = G.Project.Nav.GetLoadedBook(t, sBookAbbrev);
            if (null == book)
                return null;

            // Get the section(s) that contain the reference we want
            DSection[] vSections = book.GetSectionsContaining(pOwner.ReferenceSpan);
            if (null == vSections || vSections.Length == 0)
                return null;

            // Collect the paragraphs
            ArrayList aParagraphs = new ArrayList();
            foreach (DSection section in vSections)
            {
                DParagraph[] vp = section.GetParagraphs(pOwner.ReferenceSpan);
                foreach (DParagraph p in vp)
                    aParagraphs.Add(p);
            }

            // Convert to a vector of runs
            int cRuns = 0;
            foreach (DParagraph p in aParagraphs)
            {
                foreach (DRun r in p.Runs)
                {
                    if (null != r as DBasicText || null != r as DVerse)
                        cRuns++;
                }
            }
            if (cRuns == 0)
                return null;

            int i = 0;
            DRun[] vRuns = new DRun[cRuns];
            foreach (DParagraph p in aParagraphs)
            {
                foreach (DRun r in p.Runs)
                {
                    if (r as DBasicText != null || r as DVerse != null)
                        vRuns[i++] = r;

                }
            }
            return vRuns;
        }
        #endregion
        #region Method: void BuildContents()
        public void BuildContents()
        {
            Clear();

            if (BasicText == null)
                return;

            foreach (DTranslation t in G.Project.OtherTranslations)
            {
                // First attempt as a synchronized sibling, as this gives a much tighter
                // result. If that fails, then attempt as any old Reference Language
                DRun[] vRuns = _GetSynchronizedSiblingText(t, BasicText);
                if (null == vRuns)
                    vRuns = _GetReferenceLanguageParagraphs(t);
                if (null == vRuns)
                    continue;
         
                // Append the row to the window
                if (vRuns.Length > 0)
                {
                    StartNewRow();
                    AddLabeledText(t, vRuns, t.DisplayName);
                }
            }

            LoadData();
        }
        #endregion

    }

}