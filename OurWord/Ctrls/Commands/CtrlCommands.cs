using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using OurWord.Layouts;
using OurWordData.DataModel;
using OurWordData.DataModel.Membership;

namespace OurWord.Ctrls.Commands
{
    public delegate void SimpleHandler();
    public delegate void SwitchLayoutHandler(string sNewLayoutName);
    public delegate void ChangeZoomPercentHandler(int nNewZoomPercent);
    public delegate void OpenProjectHandler(string sPathToProjectFile);
    public delegate void ChangeUserHandler(User newUser);

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
        #region Method: void SetVisibility()
        public void SetVisibility()
        {
            SetLayoutVisibility();
            SetProjectBisibility();
            m_PrintBook.Available = Users.Current.CanPrint;
        }
        #endregion

        // Visibility / Enabling -------------------------------------------------------------
        #region method: void SetLayoutVisibility()
        void SetLayoutVisibility()
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
        #region method: void SetProjectBisibility()
        void SetProjectBisibility()
        {
            var user = Users.Current;
            var bIsValidProject = DB.IsValidProject;

            m_New.Available = !bIsValidProject || 
                user.CanCreateProject || 
                !Users.HasAdministrator;

            m_Open.Available = !bIsValidProject ||
                user.CanCreateProject ||
                !Users.HasAdministrator;

            m_Export.Available = bIsValidProject && 
                DB.TargetTranslation.BookList.Count > 0 &&
                user.CanExportProject;

            m_Project.Available = m_New.Available ||
                m_Open.Available ||
                m_Export.Available;
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

            SetVisibility();
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
                if (ClusterList.GetUserCanAccessProject(ci.Name, sProjectName))
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
        void cmdEditDropDownOpening(object sender, EventArgs e)
        {
            
        }

        // Misc ------------------------------------------------------------------------------
        #region smethod: string GetLayoutButtonText(sMenuItemText)
        static string GetLayoutButtonText(string sMenuItemText)
        {
            var s = sMenuItemText.Replace("&", "");
            return s.ToUpperInvariant();
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
        public SimpleHandler OnSave;
        public SimpleHandler OnPrintBook;
        public SimpleHandler OnExport;
        public SimpleHandler OnDownloadRepositoryFromInternet;
        public SimpleHandler OnCreateProject;
        public SimpleHandler OnHistory;
        public SimpleHandler OnInsertNote;
        public SimpleHandler OnAddNewUser;

        public SimpleHandler OnUndo;
        public SimpleHandler OnRedo;
        public SimpleHandler OnCut;
        public SimpleHandler OnCopy;
        public SimpleHandler OnPaste;
        public SimpleHandler OnInsertFootnote;
        public SimpleHandler OnDeleteFootnote;

        public SwitchLayoutHandler OnSwitchLayout;
        public ChangeZoomPercentHandler OnChangeZoomPercent;
        public OpenProjectHandler OnOpenProject;
        public ChangeUserHandler OnChangeUser;

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
            if (null != OnSave)
                OnSave();
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
            if (null != OnExport)
                OnExport();
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

        void cmdChangeUser(object sender, EventArgs e)
        {
            var item = sender as ToolStripMenuItem;
            if (null == item)
                return;

            var user = item.Tag as User;
            if (null == user)
                return;

            if (user == Users.Current)
                return;

            if (null != OnChangeUser)
                OnChangeUser(user);
        }
        void cmdAddNewUser(object sender, EventArgs e)
        {
            if (null != OnAddNewUser)
                OnAddNewUser();
        }
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

            m_User.Text = current.UserName;
        }


    }
}
