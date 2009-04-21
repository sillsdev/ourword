/**********************************************************************************************
 * Dll:     JWTools
 * File:    JW_Wizard.cs
 * Author:  John Wimbish
 * Created: 01 Feb 2007
 * Purpose: Wizard superclass
 * Legal:   Copyright (c) 2004-09, John S. Wimbish. All Rights Reserved.  
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
using System.Threading;
using JWTools;
#endregion

namespace JWTools
{
    #region INTERFACE: IJW_WizPage
    public interface IJW_WizPage
    {
        // OnActivate ------------------------------------------------------------------------
        // - Place the focus on the appropriate control
        void OnActivate();

        // CanGoToNextPage -------------------------------------------------------------------
        // - This is used to determine whether the AdvanceButton (Next/Finish) is enabled
        bool CanGoToNextPage(); 

        // ShowHelp --------------------------------------------------------------------------
        // - Bring up the appropriate Help page
        void ShowHelp();

        // PageNavigationTitle ---------------------------------------------------------------
        // - Return a string that is the title of this page, which we then overlay over
        //    the drawing to the left.
        string PageNavigationTitle();
    }
    #endregion

    public partial class JW_Wizard : Form
    {
        // Active Page -----------------------------------------------------------------------
        #region Attr{g}: IJW_WizPage ActivePage - Set via the ActivatePage method
        IJW_WizPage ActivePage
        {
            get
            {
                Debug.Assert(null != m_ActivePage);
                return m_ActivePage;
            }
        }
        IJW_WizPage m_ActivePage;
        #endregion
        #region Attr{g}: bool IsFirstPage - T if ActivePage is the first one
        bool IsFirstPage
        {
            get
            {
                if (PageCount == 0)
                    return false;
                if (Pages[0] as IJW_WizPage == ActivePage)
                    return true;
                return false;
            }
        }
        #endregion
        #region Attr{g}: bool IsLastPage - T if ActivePage is the final one
        bool IsLastPage
        {
            get
            {
                if (PageCount == 0)
                    return false;
                if (Pages[ PageCount - 1] as IJW_WizPage == ActivePage)
                    return true;
                return false;
            }
        }
        #endregion
        #region Attr{g}: IJW_WizPage PreviousPage
        IJW_WizPage PreviousPage
        {
            get
            {
                // Scan through the ArrayList to find the position of the page
                int iPage = 0;
                for (; iPage < PageCount; iPage++)
                {
                    if (Pages[iPage] as IJW_WizPage == ActivePage)
                        break;
                }

                // Decrement to the pr4eceeding page
                iPage--;

                // Return the page if it is not out of range
                if (iPage >= 0)
                    return Pages[iPage] as IJW_WizPage;
                else
                    return null;
            }
        }
        #endregion
        #region Attr{g}: IJW_WizPage NextPage
        IJW_WizPage NextPage
        {
            get
            {
                // Scan through the ArrayList to find the position of the page
                int iPage = 0;
                for (; iPage < PageCount; iPage++)
                {
                    if (Pages[iPage] as IJW_WizPage == ActivePage)
                        break;
                }

                // Increment to the following page
                iPage++;

                // Return the page if it is not out of range
                if (iPage < PageCount)
                    return Pages[iPage] as IJW_WizPage;
                else
                    return null;
            }
        }
        #endregion
        #region Method: void ActivatePage( IJW_WizPage pageToActivate )
        void ActivatePage(IJW_WizPage pageToActivate)
        {
            // Set the current page to the desired one
            m_ActivePage = pageToActivate;

            // Hide all other pages; make the Active one visible
            foreach (IJW_WizPage page in Pages)
            {
                UserControl control = page as UserControl;

                if (page == ActivePage)
                    control.Visible = true;
                else
                    control.Visible = false;
            }

            // Button visibility depends upon which page is being displayed.
            SetButtonVisibility();

            // Tell the Page that it is being activated
            (ActivePage as UserControl).Focus();
            ActivePage.OnActivate();

            // Redraw the Picture Panel so we get the proper navigation
            m_panelPicture.Invalidate();
        }
        #endregion

        // Ordered List of Pages -------------------------------------------------------------
        #region Attr{g}: ArrayList Pages
        ArrayList Pages
        {
            get
            {
                Debug.Assert(null != m_aPages);
                return m_aPages;
            }
        }
        ArrayList m_aPages;
        #endregion
        #region Method: void AddPage(IJW_WizPage)
        public void AddPage(IJW_WizPage page)
        {
            // Add the page to our own arraylist
            m_aPages.Add(page);

            // Add the page as a Control to the parent dialog
            Controls.Add(page as UserControl);
        }
        #endregion
        #region Attr{g}: int PageCount
        public int PageCount
        {
            get
            {
                return Pages.Count;
            }
        }
        #endregion
        
        // Scaffolding -----------------------------------------------------------------------
        #region Constructor(sWizardTitle, Image imgBackground)
        public JW_Wizard(string sName, string sEnglishTitle, Image imgBackground)
        {
            // Required for Windows Form Designer support
            InitializeComponent();

            //The Name that the LocDB will use
            this.Name = sName;
            this.Text = sEnglishTitle;

            // The array of pages
            m_aPages = new ArrayList();

            // Place the background picture into the panel
            if (null != imgBackground)
            {
                m_panelPicture.BackgroundImage = imgBackground;
                m_panelPicture.BackgroundImageLayout = ImageLayout.Center;
            }
        }
        #endregion
        #region Constructor() - only used by the VisualStudio 
        public JW_Wizard()
        {
            InitializeComponent();
        }
        #endregion

        // Misc Methods ----------------------------------------------------------------------
        #region Method: void Localization()
        virtual protected void Localization()
        {
            // Subclass does it all
            Debug.Assert(false);
        }
        #endregion
        #region Method: void SetButtonVisibility()
        void SetButtonVisibility()
            // This is called from the ActivatePage method
        {
            // The Previous button is not visible on the first page
            m_btnPrev.Visible = (IsFirstPage) ? false : true;

            // The Next button is not visible on the last page. Rather, the 
            // Finish button is visible there.
            m_btnNext.Visible = (IsLastPage) ? false : true;
            m_btnFinish.Visible = (IsLastPage) ? true : false;

            // Determine whether the Next button (if visible) should be enabled.
            AdvanceButtonEnabled = ActivePage.CanGoToNextPage();
        }
        #endregion
        #region Attr{g/s}: bool AdvanceButtonEnabled
        public bool AdvanceButtonEnabled
        {
            get
            {
                if (IsLastPage)
                    return m_btnFinish.Enabled;
                else
                    return m_btnNext.Enabled;
            }
            set
            {
                if (IsLastPage)
                    m_btnFinish.Enabled = value;
                else
                    m_btnNext.Enabled = value;
            }
        }
        #endregion
        #region Method: void PlaceFocusOnAdvanceButton()
        public void PlaceFocusOnAdvanceButton()
        {
            if (m_btnNext.Enabled)
                m_btnNext.Focus();
            else if (m_btnFinish.Enabled)
                m_btnFinish.Focus();
        }
        #endregion
        #region Attr{g/s}: Color NavigationColor
        public Color NavigationColor
        {
            get
            {
                return m_NavigationColor;
            }
            set
            {
                m_NavigationColor = value;
            }
        }
        Color m_NavigationColor = Color.Black;
        #endregion
		#region VirtMethod: void OnWizardFinished()
		protected virtual void OnWizardFinished()
		{
		}
		#endregion

		// Command Handlers ------------------------------------------------------------------
        #region Cmd: cmdLoad
        private void cmdLoad(object sender, EventArgs e)
        {
            // Handle the UI Language
            Localization();

            // Calculate the dimensions of the largest page
            int nWidthMax = 0;
            int nHeightMax = 0;
            foreach (IJW_WizPage page in Pages)
            {
                UserControl control = page as UserControl;
                Debug.Assert(null != control);

                // While we're at it, place into the proper left,top position
                control.Left = m_panelPicture.Width;
                control.Top = 0;

                // Dimensions of the largest page
                nWidthMax = Math.Max(nWidthMax, control.Width);
                nHeightMax = Math.Max(nHeightMax, control.Height);
            }

            // Set our size to exactly accomdate the largest page
            int nWidthNonclient = Width - ClientRectangle.Width;
            int nHeightNonclient = Height - ClientRectangle.Height;
            int nWidth = nWidthMax + m_panelPicture.Width + nWidthNonclient;
            int nHeight = nHeightMax + m_panelButtons.Height + nHeightNonclient;
            Size = new Size(nWidth, nHeight);

            // Adjust the buttons in the bottom panel 
            int nBtnSpace = m_btnNext.Left - m_btnPrev.Right;
            m_btnNext.Left = m_panelButtons.Right - nBtnSpace - m_btnNext.Width;
            m_btnPrev.Left = m_btnNext.Left - nBtnSpace - m_btnPrev.Width;
            m_btnFinish.Left = m_btnNext.Left;

            // Activate the first page (turns off all others)
            ActivatePage( Pages[0] as IJW_WizPage);

        }
        #endregion
        #region Cmd: cmdNextPage
        private void cmdNextPage(object sender, EventArgs e)
        {
            if (null != NextPage)
                ActivatePage(NextPage);
        }
        #endregion
        #region Cmd: cmdPreviousPage
        private void cmdPreviousPage(object sender, EventArgs e)
        {
            if (null != PreviousPage)
                ActivatePage(PreviousPage);
        }
        #endregion
        #region Cmd: cmdHelp
        private void cmdHelp(object sender, EventArgs e)
        {
            ActivePage.ShowHelp();
        }
        #endregion
        #region Cmd: cmdFinish
        private void cmdFinish(object sender, EventArgs e)
        {
			OnWizardFinished();
        }
        #endregion
        #region Cmd: cmdActivated
        private void cmdActivated(object sender, EventArgs e)
            // Makes certain that the appropriate control is in Focus. Unfortunately we
            // cannot do this as part of the cmdLoad handler, so we use this event which
            // comes later.
        {
            (ActivePage as UserControl).Focus();
            ActivePage.OnActivate();
        }
        #endregion
        #region Cmd: cmdPaintPicture - overlay the navigation information
        private void cmdPaintPicture(object sender, PaintEventArgs e)
        {
            // Bail if we have no pages
            if (PageCount == 0)
                return;

            // Font
            Font font = new Font( Font.FontFamily, Font.Size * 1.3F,
                FontStyle.Bold);

            // The Height of the Text Boxes
            int cyHeight =  (int)e.Graphics.MeasureString("Hi", font).Height;

            // The Width of the Text Boxes
            int cxWidth = m_panelPicture.Width - 20;

            // The Left Margin of the Text Boxes
            int xLeft = 10;

            // The spacing between the Text Boxes
            int cySpacing = (int)((float)cyHeight * 0.6F);

            // Calculate the total space needed
            int cyTotal = PageCount * cyHeight +
                (PageCount - 1) * cySpacing;

            // We want to center this in the picture's height
            int yTop = (m_panelPicture.Height - cyTotal) / 2;

            // The color for all of this
            Brush brush = new SolidBrush(NavigationColor);
            Pen pen = new Pen(brush, 2);

            // Loop through to display the pages
            int nPage = 1;
            int y = yTop;
            foreach (IJW_WizPage page in Pages)
            {
                // Build the string we'll display
                string s = nPage.ToString() + ". " + page.PageNavigationTitle();

                // Draw it
                e.Graphics.DrawString(s, font, brush, xLeft, y);

                // Rectangle around the active one
                if (page == ActivePage)
                {
                    e.Graphics.DrawRectangle(pen, new Rectangle(xLeft - 1, y - 1,
                        cxWidth + 2, cyHeight + 2));
                }

                // Ready for the next one
                y += (cyHeight + cySpacing);
                nPage++;
            }
        }
        #endregion

    }
}