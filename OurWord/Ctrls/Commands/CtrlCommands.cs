using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using JWTools;
using OurWord.Edit;
using OurWord.Layouts;
using OurWordData.DataModel;
using OurWordData.DataModel.Membership;
using OurWordData.Styles;

namespace OurWord.Ctrls.Commands
{
    public delegate void SimpleHandler();
    public delegate void SwitchLayoutHandler(string sNewLayoutName);
    public delegate void ChangeZoomPercentHandler(int nNewZoomPercent);
    public delegate void OpenProjectHandler(string sPathToProjectFile);
    public delegate void ChangeUserHandler(User newUser);
    public delegate void ChangeParagraphStyleHandler(ParagraphStyle newStyle);

    public partial class CtrlCommands : UserControl
    {
        // Public Interface ------------------------------------------------------------------
        #region Constructor()
        public CtrlCommands()
        {
            InitializeComponent();
        }
        #endregion
        #region Method: void SetLayout(string sLayoutName)
        public void SetLayout(string sLayoutName)
        {
            foreach(var item in m_Layout.DropDownItems)
            {
                var menuItem = item as ToolStripMenuItem;
                if (null == menuItem)
                    continue;

                var sTag = (string)menuItem.Tag;
                if (!string.IsNullOrEmpty(sTag) && sTag == sLayoutName)
                {
                    SetLayoutDropDown(menuItem);
                    return;
                }
            }
        }
        #endregion
        #region Method: void Setup()
        public void Setup()
        {
            SetupLayout();
            SetupProject();
            SetupPrint();
            SetupEdit();
            SetupUser();
            SetupNotes();

            // On TopTools, just localize the Layout's dropdown items. We can't localize 
            // m_Layout.Text directly, as the LocDB makes that into "Drafting" every time.
            foreach(ToolStripItem item in m_Layout.DropDownItems)
                LocDB.Localize(item);

            // Localize all of the BottomTools item
            LocDB.Localize(BottomTools);
        }
        #endregion
        #region Method: UndoRedoStack SetupUndoRedoStack()
        public UndoRedoStack SetupUndoRedoStack()
        {
            const int c_nUndoRedoMaxDepth = 15;
            return new UndoRedoStack(c_nUndoRedoMaxDepth, m_menuUndo, m_menuRedo);
        }
        #endregion
        #region Method: void EnableItalics(bool bEnabled)
        public void EnableItalics(bool bEnabled)
        {
            m_Italic.Enabled = bEnabled;
        }
        #endregion

        // Visibility / Enabling -------------------------------------------------------------
        #region method: void SetupLayout()
        void SetupLayout()
        {
            var user = Users.Current;

            // Three user-optional layouts
            m_BackTranslation.Available = user.CanDoBackTranslation;
            m_NaturalnessCheck.Available = user.CanDoNaturalnessCheck;
            m_ConsultantPreparation.Available = user.CanDoConsultantPreparation;

            var bMultipleLayoutsAvailable = m_BackTranslation.Available ||
                m_NaturalnessCheck.Available ||
                m_ConsultantPreparation.Available;

            // We only show Drafting if we're also showing one of the other ones
            m_Drafting.Available = bMultipleLayoutsAvailable;

            m_Zoom.Available = user.CanZoom;

            // If we are showing Layouts AND Zoom, then we want a separator between them
            m_LayoutSeparator.Available = (m_Zoom.Available && bMultipleLayoutsAvailable);
        }
        #endregion
        #region method: void SetupProject()
        void SetupProject()
        {
            var user = Users.Current;
            var bIsValidProject = DB.IsValidProject;

            m_New.Available = !bIsValidProject || 
                user.CanCreateProject || 
                !Users.HasAdministrator;

            m_Open.Available = !bIsValidProject ||
                user.CanCreateProject ||
                !Users.HasAdministrator;

            m_Save.Available = bIsValidProject;
            m_menuSave.Available = bIsValidProject;

            m_Export.Available = bIsValidProject && 
                DB.TargetTranslation.BookList.Count > 0 &&
                user.CanExportProject;

            m_Project.Available = m_New.Available ||
                m_Open.Available ||
                m_Export.Available;
        }
        #endregion
        #region method: void SetupPrint()
        void SetupPrint()
        {
            m_PrintBook.Available = Users.Current.CanPrint;

            m_PrintBook.Enabled = DB.IsValidProject &&
                null != DB.FrontSection && 
                null != DB.TargetSection;
        }
        #endregion
        #region method: void SetupEdit()
        void SetupEdit()
        {
            // Structural editing includes adding/deleting footnotes and changing paragraph styles
            var bCanEditStructure = Users.Current.CanEditStructure &&
                WLayout.CurrentLayoutIs(WndDrafting.c_sName);
            m_menuInsertFootnote.Available = bCanEditStructure;
            m_menuDeleteFootnote.Available = bCanEditStructure;
            m_menuChangeParagraphStyle.Available = bCanEditStructure;

            // Undo / Redo
            m_menuUndo.Available = Users.Current.CanUndoRedo;
            m_menuRedo.Available = Users.Current.CanUndoRedo;

            // Don't show the Edit menu if there isn't anything there of signifcance the user can do
            m_Edit.Available = (bCanEditStructure || Users.Current.CanUndoRedo);

            // The Cut/Copy/Paste toolbar buttons are not shown if the Edit dropdown is shown
            m_Cut.Available = !m_Edit.Available;
            m_Copy.Available = !m_Edit.Available;
            m_Paste.Available = !m_Edit.Available;

            // A special feature requested by Timor, but normally disabled for most people
            m_menuCopyBTFromFront.Available = (
                 Users.Current.CanDoConsultantPreparation &&
                 WLayout.CurrentLayoutIs(new[] {
                     WndConsultantPreparation.c_sName, 
                     WndBackTranslation.c_sName })
                 );

            // Enabling
            var bCannotEdit = (null == WLayout.CurrentLayout ||
                !WLayout.CurrentLayout.Focused || 
                Users.Current.GetEditability(DB.TargetBook) != User.TranslationSettings.Editability.Full);

            m_Cut.Enabled = !bCannotEdit;
            m_Copy.Enabled = !bCannotEdit;
            m_Paste.Enabled = !bCannotEdit;

            m_menuCut.Enabled = !bCannotEdit;
            m_menuCopy.Enabled = !bCannotEdit;
            m_menuPaste.Enabled = !bCannotEdit;           
        }
        #endregion
        #region method: void SetupUser()
        void SetupUser()
        {
            if (null == Users.Current)
                return;

            m_User.Text = Users.Current.UserName;
        }
        #endregion
        #region method: void SetupNotes()
        void SetupNotes()
        {
            m_InsertNote.Available = Users.Current.CanMakeNotes &&
                DB.IsValidProject &&
                DB.TargetBook != null &&
                Users.Current.GetEditability(DB.TargetBook) != User.TranslationSettings.Editability.ReadOnly;

            m_InsertNote.Enabled = DB.IsValidProject &&
                null != DB.FrontSection &&
                null != DB.TargetSection;
        }
        #endregion

        // Infrastructure handlers -----------------------------------------------------------
        #region cmd: cmdLoad
        private void cmdLoad(object sender, EventArgs e)
        {
            Height = TopTools.Height + BottomTools.Height;

            m_Drafting.Tag = WndDrafting.c_sName;
            m_BackTranslation.Tag = WndBackTranslation.c_sName;
            m_NaturalnessCheck.Tag = WndNaturalness.c_sName;
            m_ConsultantPreparation.Tag = WndConsultantPreparation.c_sName;

            Setup();
        }
        #endregion
        #region cmd: cmdLayoutDropDownOpening
        private void cmdLayoutDropDownOpening(object sender, EventArgs e)
        {
            m_Zoom.DropDownItems.Clear();

            foreach (var n in User.PossibleZoomPercents)
            {
                var mi = new ToolStripMenuItem(n + "%", null, cmdChangeZoomPercent, "zoom_" + n)
                {
                    Tag = n,
                    Checked = (n == Users.Current.ZoomPercent)
                };
                m_Zoom.DropDownItems.Add(mi);
            }
        }
        #endregion
        #region cmd: cmdProjectDropDownOpening
        #region smethod: List<string> GetUserAllowedProjects(ClusterInfo)
        static List<string> GetUserAllowedProjects(ClusterInfo ci)
        {
            var vsAllProjects = ci.GetClusterLanguageList(true);

            var vsUserAllowedProjects = new List<string>();
            foreach(var sProjectName in vsAllProjects)
            {
                if (Users.Current.IsMemberOf(sProjectName))
                    vsUserAllowedProjects.Add(sProjectName);
            }

            vsUserAllowedProjects.Sort();

            return vsUserAllowedProjects;
        }
        #endregion
        #region method: void BuildProjectsSubMenu(miParent, ClusterInfo)
        void BuildProjectsSubMenu(ToolStripDropDownItem miParent, ClusterInfo ci)
        {
            var vsUserAllowedProjects = GetUserAllowedProjects(ci);

            if (vsUserAllowedProjects.Count == 0)
            {
                miParent.DropDownItems.Add("(none)");
                return;               
            }

            foreach (var sProjectName in vsUserAllowedProjects)
            {
                var mi = new ToolStripMenuItem(sProjectName, null, cmdOpenProject)
                {
                    Tag = ci.GetProjectPath(sProjectName)
                };
                miParent.DropDownItems.Add(mi);
            }
        }
        #endregion

        void cmdProjectDropDownOpening(object sender, EventArgs e)
        {
            // Scan the disk for the list of clusters that we currently have
            Cursor.Current = Cursors.WaitCursor;
            ClusterList.ScanForClusters();
            Cursor.Current = Cursors.Default;

            // We should always have clusters (as the Sample Data should be there; but in 
            // the event we don't, then we hide the Open menu.
            if (ClusterList.Clusters.Count == 0)
            {
                m_Open.Available = false;
                return;
            }

            m_Open.DropDownItems.Clear();

            // If exactly one cluster, then just add it directly to the Open menu
            if (ClusterList.Clusters.Count == 1)
            {
                BuildProjectsSubMenu(m_Open, ClusterList.Clusters[0]);
                return;
            }

            // Otherwise, we do submenus
            foreach(var clusterInfo in ClusterList.Clusters)
            {
                var miCluster = new ToolStripMenuItem(clusterInfo.Name);
                BuildProjectsSubMenu(miCluster, clusterInfo);
                m_Open.DropDownItems.Add(miCluster);              
            }
        }
        #endregion
        #region cmd: cmdEditDropDownOpening
        #region method: void PopulateChangeStyleDropDown()
        void PopulateChangeStyleDropDown()
        {
            // If not visible, nothing to do
            if (m_menuChangeParagraphStyle.Available == false)
                return;

            // Clear out any previous subitems
            m_menuChangeParagraphStyle.DropDownItems.Clear();

            // Get the currently-selected paragraph
            OWWindow wnd = WLayout.CurrentLayout;
            if (null == wnd || !wnd.Focused)
                return;
            var selection = wnd.Selection;
            if (null == selection)
                return;
            var p = selection.DBT.Paragraph;
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
                    @"changeParagraphStyle_" + style.StyleName) 
                    {
                        Tag = style
                    };

                if (style == p.Style)
                    mi.Checked = true;

                m_menuChangeParagraphStyle.DropDownItems.Add(mi);
            }
        }
        #endregion
        #region method:  void SetFootnoteEnabling()
        void SetFootnoteEnabling()
        {
            var wnd = WLayout.CurrentLayout;
            var bCanEdit = (null != wnd && 
                wnd.Focused && 
                Users.Current.GetEditability(DB.TargetBook) == User.TranslationSettings.Editability.Full);

            m_menuInsertFootnote.Enabled = (bCanEdit && wnd.canInsertFootnote);
            m_menuDeleteFootnote.Enabled = (bCanEdit && wnd.canDeleteFootnote);
        }
        #endregion

        void cmdEditDropDownOpening(object sender, EventArgs e)
        {
            PopulateChangeStyleDropDown();
            SetFootnoteEnabling();
        }
        #endregion
        #region cmd: cmdUserDropDownOpening
        private void cmdUserDropDownOpening(object sender, EventArgs e)
        {
            var current = Users.Current;
            Debug.Assert(null != current);

            m_User.DropDownItems.Clear();

            var allUsers = new List<User>();
            allUsers.AddRange(Users.Members);
            allUsers.Add(Users.Observer);

            foreach (var user in allUsers)
            {
                var item = new ToolStripMenuItem(user.UserName, null, cmdChangeUser)
                {
                    Tag = user
                };
                if (user == current)
                    item.Checked = true;
                m_User.DropDownItems.Add(item);
            }

            if (current.IsAdministrator || !Users.HasAdministrator)
            {
                m_User.DropDownItems.Add(new ToolStripSeparator());
                m_User.DropDownItems.Add(
                    new ToolStripMenuItem("Add new user...", null, cmdAddNewUser));
            }
        }
        #endregion
        #region cmd: cmdToolsDropDownOpening
        private void cmdToolsDropDownOpening(object sender, EventArgs e)
        {
            // Synchronize: Only if synch information is set up
            m_menuSendReceive.Available = Users.Current.CanSendReceive;

            // Dependent on current user permissions
            m_menuRestoreFromBackup.Available = Users.Current.CanRestoreBackups;
            m_menuLocalizerTool.Available = Users.Current.CanLocalize;

            // Dependent on loaded data
            m_menuIncrementBookStatus.Available = DB.IsValidProject &&
                null != DB.Project.STarget;

            // Display the separator for these three?
            var bLowerToolsAvailable = m_menuRestoreFromBackup.Available ||
                m_menuLocalizerTool.Available ||
                m_menuIncrementBookStatus.Available;
            m_separatorTools.Available = bLowerToolsAvailable;          
        }
        #endregion

        // Misc ------------------------------------------------------------------------------
        #region smethod: string GetLayoutButtonText(sMenuItemText)
        static string GetLayoutButtonText(string sMenuItemText)
        {
            var s = sMenuItemText.Replace("&", "");
            return s;
        }
        #endregion
        #region method: void SetLayoutDropDown(ToolStripItem item)
        void SetLayoutDropDown(ToolStripItem item)
        {
            m_Layout.Text = GetLayoutButtonText(item.Text);
            m_Layout.Image = item.Image;
            m_Layout.Tag = item.Tag;
        }
        #endregion

        // Handlers --------------------------------------------------------------------------
        public SimpleHandler OnExit;
        public SimpleHandler OnSaveProject;
        public SimpleHandler OnPrintBook;
        public SimpleHandler OnExportTranslation;
        public SimpleHandler OnDownloadRepositoryFromInternet;
        public SimpleHandler OnCreateProject;
        public SimpleHandler OnHistory;
        public SimpleHandler OnAddNewUser;
        public SimpleHandler OnInsertNote;

        public SimpleHandler OnUndo;
        public SimpleHandler OnRedo;
        public SimpleHandler OnCut;
        public SimpleHandler OnCopy;
        public SimpleHandler OnPaste;
        public SimpleHandler OnItalic;
        public SimpleHandler OnInsertFootnote;
        public SimpleHandler OnDeleteFootnote;
        public SimpleHandler OnCopyBtFromFront;

        public SimpleHandler OnSendReceive;
        public SimpleHandler OnCheckForUpdates;
        public SimpleHandler OnConfigure;
        public SimpleHandler OnIncrementBookStatus;
        public SimpleHandler OnRestoreFromBackup;
        public SimpleHandler OnLocalizerTool;
        public SimpleHandler OnHelpTopics;
        public SimpleHandler OnAbout;

        public SwitchLayoutHandler OnSwitchLayout;
        public ChangeZoomPercentHandler OnChangeZoomPercent;
        public OpenProjectHandler OnOpenProject;
        public ChangeUserHandler OnChangeUser;
        public ChangeParagraphStyleHandler OnChangeParagraphStyle;

        // Simple Handlers
        #region cmd: cmdExit
        void cmdExit(object sender, EventArgs e)
        {
            if (null != OnExit)
                OnExit();
        }
        #endregion
        #region cmd: cmdSave
        void cmdSave(object sender, EventArgs e)
        {
            if (null != OnSaveProject)
                OnSaveProject();
        }
        #endregion
        #region cmd: cmdPrintBook
        void cmdPrintBook(object sender, EventArgs a)
        {
            if (null != OnPrintBook)
                OnPrintBook();
        }
        #endregion
        #region cmd: cmdExport
        private void cmdExport(object sender, EventArgs e)
        {
            if (null != OnExportTranslation)
                OnExportTranslation();
        }
        #endregion
        #region cmd: cmdDownloadRepositoryFromInternet
        private void cmdDownloadRepositoryFromInternet(object sender, EventArgs e)
        {
            if (null != OnDownloadRepositoryFromInternet)
                OnDownloadRepositoryFromInternet();
        }
        #endregion
        #region cmd: cmdCreateProject
        private void cmdCreateProject(object sender, EventArgs e)
        {
            if (null != OnCreateProject)
                OnCreateProject();
        }
        #endregion
        #region cmd: cmdInsertNote
        private void cmdInsertNote(object sender, EventArgs e)
        {
            if (null != OnInsertNote)
                OnInsertNote();
        }
        #endregion
        #region cmd: cmdHistory
        private void cmdHistory(object sender, EventArgs e)
        {
            if (null != OnHistory)
                OnHistory();
        }
        #endregion

        // Simple Handlers: Editing
        #region cmd: cmdUndo
        private void cmdUndo(object sender, EventArgs e)
        {
            if (null != OnUndo)
                OnUndo();
        }
        #endregion
        #region cmd: cmdRedo
        private void cmdRedo(object sender, EventArgs e)
        {
            if (null != OnRedo)
                OnRedo();
        }
        #endregion
        #region cmd: cmdCut
        private void cmdCut(object sender, EventArgs e)
        {
            if (null != OnCut)
                OnCut();
        }
        #endregion
        #region cmd: cmdCopy
        private void cmdCopy(object sender, EventArgs e)
        {
            if (null != OnCopy)
                OnCopy();
        }
        #endregion
        #region cmd: cmdPaste
        private void cmdPaste(object sender, EventArgs e)
        {
            if (null != OnPaste)
                OnPaste();
        }
        #endregion
        #region cmd: cmdOnItalic
        void cmdOnItalic(Object sender, EventArgs e)
        {
            if (null != OnItalic)
                OnItalic();
        }
        #endregion
        #region cmd: cmdInsertFootnote
        private void cmdInsertFootnote(object sender, EventArgs e)
        {
            if (null != OnInsertFootnote)
                OnInsertFootnote();
        }
        #endregion
        #region cmd: cmdDeleteFootnote
        private void cmdDeleteFootnote(object sender, EventArgs e)
        {
            if (null != OnDeleteFootnote)
                OnDeleteFootnote();
        }
        #endregion
        #region cmd: cmdCopyBtFromFront
        private void cmdCopyBtFromFront(object sender, EventArgs e)
        {
            if (null != OnCopyBtFromFront)
                OnCopyBtFromFront();
        }
        #endregion

        // Simple Handlers: Tools Menu
        #region cmd: cmdSendReceive
        private void cmdSendReceive(object sender, EventArgs e)
        {
            if (null != OnSendReceive)
                OnSendReceive();
        }
        #endregion
        #region cmd: cmdCheckForUpdates
        private void cmdCheckForUpdates(object sender, EventArgs e)
        {
            if (null != OnCheckForUpdates)
                OnCheckForUpdates();
        }
        #endregion
        #region cmd: cmdConfigure
        private void cmdConfigure(object sender, EventArgs e)
        {
            if (null != OnConfigure)
                OnConfigure();
        }
        #endregion
        #region cmd: cmdIncrementBookStatus
        private void cmdIncrementBookStatus(object sender, EventArgs e)
        {
            if (null != OnIncrementBookStatus)
                OnIncrementBookStatus();
        }
        #endregion
        #region cmd: cmdRestoreFromBackup
        private void cmdRestoreFromBackup(object sender, EventArgs e)
        {
            if (null != OnRestoreFromBackup)
                OnRestoreFromBackup();
        }
        #endregion
        #region cmd: cmdLocalizerTool
        private void cmdLocalizerTool(object sender, EventArgs e)
        {
            if (null != OnLocalizerTool)
                OnLocalizerTool();
        }
        #endregion
        #region cmd: cmdHelpTopics
        private void cmdHelpTopics(object sender, EventArgs e)
        {
            if (null != OnHelpTopics)
                OnHelpTopics();
        }
        #endregion
        #region cmd: cmdAbout
        private void cmdAbout(object sender, EventArgs e)
        {
            if (null != OnAbout)
                OnAbout();
        }
        #endregion

        // Handlers that pass data
        #region cmd: cmdSwitchLayout
        private void cmdSwitchLayout(object sender, EventArgs e)
        {
            var item = sender as ToolStripMenuItem;
            if (null == item)
                return;

            if (GetLayoutButtonText(item.Text) == m_Layout.Text)
                return;

            var sLayoutName = (string) item.Tag;
            if (string.IsNullOrEmpty(sLayoutName))
                return;

            SetLayoutDropDown(item);

            if (null != OnSwitchLayout)
                OnSwitchLayout(sLayoutName);
        }
        #endregion
        #region cmd: cmdChangeZoomPercent
        void cmdChangeZoomPercent(object sender, EventArgs e)
        {
            var item = sender as ToolStripMenuItem;
            if (null == item)
                return;

            var nNewZoomPercent = (int) item.Tag;
            if (nNewZoomPercent == Users.Current.ZoomPercent)
                return;

            if (null != OnChangeZoomPercent)
                OnChangeZoomPercent(nNewZoomPercent);
        }
        #endregion
        #region cmd: cmdOpenProject
        void cmdOpenProject(object sender, EventArgs e)
        {
            var item = sender as ToolStripMenuItem;
            if (null == item)
                return;

            var sPathToProjectFile = (string) item.Tag;
            if (string.IsNullOrEmpty(sPathToProjectFile))
                return;

            if (null != OnOpenProject)
                OnOpenProject(sPathToProjectFile);
        }
        #endregion
        #region cmd: cmdChangeParagraphStyle
        void cmdChangeParagraphStyle(object sender, EventArgs e)
        {
            var item = sender as ToolStripMenuItem;
            if (null == item)
                return;

            var newStyle = item.Tag as ParagraphStyle;
            if (null == newStyle)
                return;

            if (null != OnChangeParagraphStyle)
                OnChangeParagraphStyle(newStyle);
        }
        #endregion

        // Users
        #region cmd: cmdChangeUser
        void cmdChangeUser(object sender, EventArgs e)
        {
            if (null == OnChangeUser)
                return;

            var item = sender as ToolStripMenuItem;
            if (null == item)
                return;

            var user = item.Tag as User;
            if (null == user)
                return;

            if (user == Users.Current)
                return;

            OnChangeUser(user);

            // If sucessful, we'll have a new user now
            SetupUser();
        }
        #endregion
        #region cmd: cmdAddNewUser
        void cmdAddNewUser(object sender, EventArgs e)
        {
            if (null == OnAddNewUser)
                return;

            OnAddNewUser();

            // If sucessful, we'll have a new user now
            SetupUser();
        }
        #endregion



    }
}
