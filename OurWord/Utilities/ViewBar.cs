using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using JWTools;
using OurWord.Dialogs.Membership;
using OurWord.Layouts;
using OurWordData.DataModel;
using OurWordData.DataModel.Membership;

namespace OurWord.Utilities
{
    public class ViewBar : ToolStrip
    {
        // Controls --------------------------------------------------------------------------
        // ViewName
        #region LabelText: string ViewName
        string ViewName
        {
            set
            {
                m_ViewName.Text = value;
            }
        }
        #endregion
        private ToolStripLabel m_ViewName;
        #region Method: void CreateViewNameCtrl()
        void CreateViewNameCtrl()
        {
            m_ViewName = new ToolStripLabel 
            {
                BackColor = Color.Transparent, 
                ForeColor = Color.White,
                Font = new Font("Arial", 12, FontStyle.Bold),
                Text = @"Drafting"
            };
            Items.Add(m_ViewName);
        }
        #endregion

        // Language Info
        #region LabelText: string LanguageInfo
        string LanguageInfo
        {
            set
            {
                m_LanguageInfo.Text = value;
            }
        }
        #endregion
        private ToolStripLabel m_LanguageInfo;
        #region Method: void CreateLanguageInfoCtrl()
        void CreateLanguageInfoCtrl()
        {
            m_LanguageInfo = new ToolStripLabel 
            {
                BackColor = Color.Transparent,
                ForeColor = Color.White,
                Font = new Font("Arial", 12),
                Text = @"(language)"
            };
            Items.Add(m_LanguageInfo);
        }
        #endregion

        // Passage
        #region LabelText: string Passage
        string Passage
        {
            set
            {
                m_Passage.Text = value;
            }
        }
        #endregion
        private ToolStripLabel m_Passage;
        #region Method: void CreatePassageCtrl()
        void CreatePassageCtrl()
        {
            m_Passage = new ToolStripLabel
            {
                BackColor = Color.Transparent,
                ForeColor = Color.Wheat,
                Font = new Font("Arial", 12, FontStyle.Bold),
                Text = @"(passage)"
            };
            Items.Add(m_Passage);
        }
        #endregion

        // Locked
        #region Attr{s}: bool ShowPadlock
        bool ShowPadlock
        {
            set
            {
                m_Locked.Visible = value;
            }
        }
        #endregion
        private ToolStripButton m_Locked;
        #region Method: void CreateLockedCtrl()
        void CreateLockedCtrl()
        {
            m_Locked = new ToolStripButton 
            {
                Alignment = ToolStripItemAlignment.Right,
                DisplayStyle = ToolStripItemDisplayStyle.Image,
                ImageTransparentColor = Color.Magenta,
                Image = JWU.GetBitmap("Padlock.ico"),
                Name = "Locked",
                ToolTipText = @"This book is locked and cannot be edited."
            };
            Items.Add(m_Locked);
        }
        #endregion

        // User
        private ToolStripDropDownButton m_User;
        #region Method: void CreateUserControl()
        void CreateUserControl()
        {
            m_User = new ToolStripDropDownButton 
            {
                Alignment = ToolStripItemAlignment.Right,
                BackColor = Color.Transparent,
                DisplayStyle = ToolStripItemDisplayStyle.Text,
                ForeColor = Color.Moccasin,
                Name = "Users",
                Text = @"<User>"
            };
            Items.Add(m_User);
        }
        #endregion
        #region Method: void SetupUserMenu()
        void SetupUserMenu()
        {
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
                if (user == Users.Current)
                    item.Checked = true;
                m_User.DropDownItems.Add(item);
            }

            SetupUserMenu_AddNewUser();

            Debug.Assert(null != Users.Current);
            m_User.Text = Users.Current.UserName;
        }
        #region Method: void SetupUserMenu_AddNewUser()
        private void SetupUserMenu_AddNewUser()
            // If the current user is an administrator, or if there is no
            // administrator in this cluster, then we enable this menu item.
        {
            if (Users.Current.IsAdministrator || !Users.HasAdministrator)
            {
                var itemAddNewUser = new ToolStripMenuItem("Add new user...", null, cmdAddNewUser);
                m_User.DropDownItems.Add(itemAddNewUser);
            }
        }
        #endregion
        #endregion
        #region Cmd: cmdAddNewUser
        private void cmdAddNewUser(object sender, EventArgs e)
        {
            var dlg = new DlgAddNewUser();
            if (DialogResult.OK != dlg.ShowDialog(G.App))
                return;

            var newUser = dlg.InitializeAs.Clone();
            newUser.UserName = dlg.FullName;
            newUser.Password = dlg.Password;
            newUser.NoteAuthorsName = dlg.FullName;

            Users.Add(newUser);

            Users.Current = newUser;

            SetupUserMenu();
            G.App.OnEnterProject();
        }
        #endregion
        #region Cmd: cmdChangeUser
        private void cmdChangeUser(object sender, EventArgs e)
        {
            var item = sender as ToolStripMenuItem;
            if (null == item)
                return;

            var newUser = item.Tag as User;
            if (null == newUser)
                return;

            // If the new user is the same as the current, then there's nothing to do
            if (newUser.UserName == Users.Current.UserName)
                return;

            // Determine if we need to ask for the password. We don't if
            // the current user is an administrator; or if we are changing
            // to the Observer user.
            bool bNeedPasswordCheck = true;
            if (newUser == Users.Observer)
                bNeedPasswordCheck = false;
            if (Users.Current.IsAdministrator)
                bNeedPasswordCheck = false;

            // Request password
            if (bNeedPasswordCheck)
            {
                var dlgPassword = new DlgPassword {UserName = newUser.UserName};
                if (DialogResult.OK != dlgPassword.ShowDialog(G.App))
                    return;
                if (!newUser.VerifyPassword(dlgPassword.Password))
                    return;
            }

            Users.Current = newUser;
            SetupUserMenu();
            G.App.OnEnterProject();
        }
        #endregion

        // Scaffolding -----------------------------------------------------------------------
        #region Constructor()
        public ViewBar()
        {
            BackColor = Color.Gray;
            GripStyle = ToolStripGripStyle.Hidden;
            Name = "ViewBar";
            Stretch = true;
            Dock = DockStyle.Top;
            Location = new Point(0,0);

            // Left-hand side
            CreateViewNameCtrl();
            CreateLanguageInfoCtrl();
            CreatePassageCtrl();

            // Right-hand side
            CreateLockedCtrl();
            CreateUserControl();
        }
        #endregion

        // Public Interface ------------------------------------------------------------------
        #region Method: void SetToContext(WLayout layout, DProject project)
        public void SetToContext(WLayout layout, DProject project)
        {
            var bIsDisplayableProject = (null != project && project.HasDataToDisplay);

            SetupUserMenu();

            if (!bIsDisplayableProject)
            {
                ViewName = G.GetLoc_GeneralUI("NoProjectDefined", "No Project Defined");
                LanguageInfo = "";
                Passage = "";
                ShowPadlock = false;
                return;
            }

            ViewName = layout.WindowName;
            LanguageInfo = string.Format(@" ({0}) ", layout.LanguageInfo);
            Passage = layout.PassageName;
            ShowPadlock = OurWordMain.TargetIsLocked;
        }
        #endregion

    }
}
