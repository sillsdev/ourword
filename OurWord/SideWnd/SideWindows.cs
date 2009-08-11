#region ***** SideWindows.cs *****
/**********************************************************************************************
 * Project: OurWord!
 * File:    SideWindows.cs
 * Author:  John Wimbish
 * Created: 21 Mar 2007
 * Purpose: Tab Control for displaying the various OW Side-Windows (e.g., Notes, History)
 * Legal:   Copyright (c) 2004-09, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
#region Using
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Timers;
using System.Windows.Forms;
using JWTools;
using JWdb;
using JWdb.DataModel;
using OurWord.Layouts;
using OurWord.Edit;
#endregion
#endregion

namespace OurWord.SideWnd
{
    interface ISideWnd
    {
        // For positioning the child windows, toolbars, etc.
        void SetSize(Size sz);

        // Return the child OWWindow
        OWWindow Window { get; }

        // Respond to events in the main window, to set toolbar button enabling
        void SetControlsEnabling();
    }

    public class SideWindows : TabControl
    {
        // Page creation, manipulation -------------------------------------------------------
        #region Method: void ClearPages()
        public void ClearPages()
        {
            TabPages.Clear();
        }
        #endregion
        #region Method: void AddPage(UserControl, sTabTextEnglish)
        public void AddPage(UserControl uc, string sTabTextEnglish)
        {
            // The TabPage name is the English text less any spaces or such. For now,
            // since that's all we're doing, we'll just do an assertion.
            Debug.Assert(-1 == sTabTextEnglish.IndexOf(' '));
            string sName = sTabTextEnglish;

            // Create and add a tab page to this tab control
            TabPage page = new TabPage();
            page.Name = sName;
            page.Text = G.GetLoc_GeneralUI(sName, sTabTextEnglish);
            TabPages.Add(page);

            // Our control is the one-and-only child of this new page
            page.Controls.Add(uc);
        }
        #endregion
        #region Method: void ActivatePage(string sName)
        public void ActivatePage(string sName)
        {
            foreach (TabPage page in TabPages)
            {
                if (page.Name == sName)
                {
                    SelectTab(page);
                    return;
                }
            }
        }
        #endregion
        #region VAttr{g}: bool HasSideWindows
        public bool HasSideWindows
        {
            get
            {
                return (TabPages.Count > 0);
            }
        }
        #endregion

        // Operations on ISideWnd collection -------------------------------------------------
        #region VAttr{g}: List<ISideWnd> SideWnds
        List<ISideWnd> SideWnds
        {
            get
            {
                var v = new List<ISideWnd>();

                foreach (TabPage page in TabPages)
                {
                    foreach (Control ctrl in page.Controls)
                    {
                        if (ctrl as ISideWnd != null)
                            v.Add(ctrl as ISideWnd);
                        break;
                    }
                }

                return v;
            }
        }
        #endregion
        #region Method: void SetChildrenSizes()
        public void SetChildrenSizes()
        {
            foreach (ISideWnd sw in SideWnds)
                sw.SetSize(DisplayRectangle.Size);
        }

        #endregion
        #region Method: void RegisterWindows(OWWindow wndMain)
        public void RegisterWindows(OWWindow wndMain)
        {
            Debug.Assert(null != wndMain);

            wndMain.ResetSecondaryWindows();

            foreach (ISideWnd sw in SideWnds)
                wndMain.RegisterSecondaryWindow(sw.Window);
        }
        #endregion
        #region Method: void SetZoomFactor(float fZoomFactor)
        public void SetZoomFactor(float fZoomFactor)
        {
            foreach (ISideWnd sw in SideWnds)
                sw.Window.ZoomFactor = fZoomFactor;
        }
        #endregion
        #region Attr{g}: OWWindow FocusedWindow
        public OWWindow FocusedWindow
        {
            get
            {
                foreach (ISideWnd sw in SideWnds)
                {
                    if (sw.Window.Focused)
                        return sw.Window;
                }
                return null;
            }
        }
        #endregion
        #region Method: void SetControlsEnabling()
        public void SetControlsEnabling()
        {
            foreach (ISideWnd sw in SideWnds)
                sw.SetControlsEnabling();
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
        #region Method: void InitializeComponent()
        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // SideWindows
            // 
            this.ShowToolTips = true;
            this.ResumeLayout(false);
        }
        #endregion

        // Notes Window ----------------------------------------------------------------------
        #region Attr{g}: bool HasNotesWindow
        public bool HasNotesWindow
        {
            get
            {
                foreach (TabPage page in TabPages)
                {
                    if (page.Name == "Notes")
                        return true;
                }
                return false;
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

}