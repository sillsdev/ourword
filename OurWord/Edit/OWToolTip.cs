#region ***** OWToolTip.cs *****
/**********************************************************************************************
 * Project: OurWord!
 * File:    OWToolTip.cs
 * Author:  John Wimbish
 * Created: 02 Sep 2009
 * Purpose: Tooltip window for EBlocks within an OWWindow
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
using System.Drawing.Drawing2D;
using System.IO;
using System.Text;
using System.Timers;
using System.Threading;
using System.Windows.Forms;
using JWTools;
using JWdb;
using JWdb.DataModel;
using Palaso.UI.WindowsForms.Keyboarding;
#endregion
#endregion

namespace OurWord.Edit
{
    public partial class OWToolTip : Form
    {
        // Attrs -----------------------------------------------------------------------------
        #region Attr{g}: ToolTipContents ContentWindow
        public ToolTipContents ContentWindow
        {
            get
            {
                return m_ContentWindow;
            }
        }
        ToolTipContents m_ContentWindow;
        #endregion

        // Launch Window ---------------------------------------------------------------------
        #region Method: Screen GetScreenContainingPoint(Point pt)
        Screen GetScreenContainingPoint(Point pt)
        {
            var screen = Screen.PrimaryScreen;
            foreach (var sc in Screen.AllScreens)
            {
                if (sc.Bounds.Contains(pt))
                    screen = sc;
            }
            return screen;
        }
        #endregion
        #region Method: void MoveMouseIntoWindow()
        void MoveMouseIntoWindow()
            // We want to move the Mouse to be within the window, so that it is easier on
            // the user; otherwise a faulty mouse movement would easily dismiss the window.
            // So we want to move it vertically so that it is within the window.
        {
            int nOffset = 5;

            var rectToolTipScreenCoords = RectangleToScreen(ClientRectangle);

            var yCursor = Cursor.Position.Y;
            yCursor = Math.Max(yCursor, rectToolTipScreenCoords.Top + nOffset);
            yCursor = Math.Min(yCursor, rectToolTipScreenCoords.Bottom - nOffset);

            var xCursor = Math.Max(Cursor.Position.X, rectToolTipScreenCoords.Left + nOffset);

            Cursor.Position = new Point(xCursor, yCursor);
        }
        #endregion
        #region Method: void LaunchToolTipWindow()
        public void LaunchToolTipWindow()
        {
            // Since this can be called independently of the timer, we want to make sure
            // that it doesn't get called twice due to different threads.
            m_cTicksCountdown = -1;

            // Make sure the EBlock supports a tooltip window
            if (!Block.HasToolTip())
                return;

            // Load its contents; this sets the window's height
            ContentWindow.LoadData();

            // Get the underlying block's location on the screen
            var ptWindowScreenLocation = Block.Window.PointToScreen(Block.Window.Location);

            // Get the screen that contains this block; we want to prevent the ToolTip
            // from being split across multiple displays
            var screen = GetScreenContainingPoint(ptWindowScreenLocation);

            // We want to position the Tooltip horizontally so that it is left-aligned with 
            // the block; but moving it to the left if it will not fit on the current screen
            int nleft = ptWindowScreenLocation.X + (int)Block.Position.X;
            if (nleft + Width > screen.Bounds.Right)
                nleft = screen.Bounds.Right - Width;

            // We want to position the Tooltip vertically so it is just under the block.
            // However, if this would push us below the window, then we move it to be
            // just above the block.
            int nTop = ptWindowScreenLocation.Y +
                (int)Block.Position.Y -
                (int)Block.Window.ScrollBarPosition +
                (int)Block.Height;
            if (nTop + Height > screen.Bounds.Bottom)
            {
                nTop -= Height;
                nTop -= (int)Block.Height;
            }

            // Calculations done: move and show the window
            this.Location = new Point(nleft, nTop);
            Show();

            // Select the last thing possible, or make sure we have a null selection
            // if a selection can't be made (otherwise the window tries to use the
            // selection from the previous time.
            if (false == ContentWindow.Contents.Select_LastWord_End())
                ContentWindow.Selection = null;

            // If we don't focus the window, the selection will not flash and keyboard entry
            // will not be received.
            ContentWindow.Focus();

            // Make sure the mouse is within the window rectangle
            MoveMouseIntoWindow();
        }
        #endregion
        #region Method: void LaunchToolTipWindow(EBlock)
        public void LaunchToolTipWindow(EBlock block)
        {
            SetBlock(block);
            LaunchToolTipWindow();
        }
        #endregion
        #region Method: void CloseWindow()
        public void CloseWindow()
        {
            Hide();
        }
        #endregion

        // Timer -----------------------------------------------------------------------------
        System.Windows.Forms.Timer m_TooltipTimer;
        const int c_nTooltipTimerInterval = 400;
        #region Cmd: OnTooltipTimerTick
        void OnTooltipTimerTick(object sender, EventArgs e)
        {
            // We want to make sure we hover at least two ticks before displaying the
            // popup, rather than displaying whenever the mouse moves across the item.
            if (m_cTicksCountdown >= 0)
                m_cTicksCountdown--;

            // When the countdown reaches 0, we're ready to display the popup
            if (m_cTicksCountdown == 0)
                LaunchToolTipWindow();
        }
        #endregion

        // Set current block -----------------------------------------------------------------
        #region Attr{g}: EBlock Block
        public EBlock Block
        {
            get
            {
                return m_Block;
            }
        }
        EBlock m_Block;
        #endregion
        int m_cTicksCountdown = -1;
        #region Method: void SetBlock(EBlock block)
        public void SetBlock(EBlock block)
        {
            Debug.Assert(null != block);

            // If we've moved off of the current block, then we want to hide
            // any visible tooltip.
            if (block != Block)
            {
                m_Block = block;
                Hide();
            }

            // Start the countdown. SetBlock is only called when the mouse is moved,
            // so if the countdown makes it to zero, it means the mouse has remained
            // stationary
            m_cTicksCountdown = 2;
        }
        #endregion
        #region Method: void ClearBlock()
        public void ClearBlock()
        {
            m_cTicksCountdown = -1;
            m_Block = null;
        }
        #endregion

        // Scaffolding -----------------------------------------------------------------------
        #region SAttr{g}: OWToolTip ToolTip - the one-and-only ToolTip
        static public OWToolTip ToolTip
        {
            get
            {
                if (null == s_ToolTip)
                    s_ToolTip = new OWToolTip();
                return s_ToolTip;
            }
        }
        static OWToolTip s_ToolTip;
        #endregion
        #region Constructor()
        public OWToolTip()
        {
            InitializeComponent();

            // Create and add the content OWWindow
            m_ContentWindow = new ToolTipContents(this);
            ContentWindow.Dock = DockStyle.Fill;
            Controls.Add(ContentWindow);

            // Settings for this window
            Visible = false;

            // Turn on the timer
            m_TooltipTimer = new System.Windows.Forms.Timer();
            m_TooltipTimer.Tick += new EventHandler(OnTooltipTimerTick);
            m_TooltipTimer.Interval = c_nTooltipTimerInterval;
            m_TooltipTimer.Start();
        }
        #endregion
        #region Cmd: OnVisibleChanged
        protected override void OnVisibleChanged(EventArgs e)
            // Every time we show or hid a window, we want to restart the UndoStack.
            // The idea is that while a window is showing, subsequent actions can
            // be undone; but when the Tooltip is dismissed, the actions are no
            // longer on the stack; we return the stack back to the state prior to
            // launching the ToolTip.
        {
            base.OnVisibleChanged(e);

            var UndoStack = OurWordMain.App.URStack;

            if (Visible)
            {
                UndoStack.BookmarkStack();
            }
            else
            {
                UndoStack.RestoreBookmarkedStack();
            }
        }
        #endregion
    }

    public class ToolTipContents : OWWindow
    {
        // Attrs -----------------------------------------------------------------------------
        #region Attr{g}: OWToolTip WndToolTip
        OWToolTip WndToolTip
        {
            get
            {
                Debug.Assert(null != m_wndToolTip);
                return m_wndToolTip;
            }
        }
        OWToolTip m_wndToolTip;
        #endregion
        #region VAttr{g}: EBlock Block
        EBlock Block
        {
            get
            {
                return WndToolTip.Block;
            }
        }
        #endregion

        // Scaffolding -----------------------------------------------------------------------
        #region Constructor(OWToolTip)
        public ToolTipContents(OWToolTip _wndToolTip)
            : base(WindowClass.Tooltip)
        {
            m_wndToolTip = _wndToolTip;
        }
        #endregion

        // Layout and Dynamic Window Height --------------------------------------------------
        #region OMethod: void LoadData()
        public override void LoadData()
        {
            // Have the block load the boxes. We test for a null Block, because a mouse move
            // over the main window can cause an asynchronus call to ClearBlock, before this
            // method here is entered, e.g., if the user changed the Status and then moved the
            // mouse before the ChangeStatus.SetStatus method can call us here. (Took a while
            // to track this one down!) 
            if (null != Block)
                Block.LoadToolTip(this);

            // Tell the superclass to finish loading, which involves laying out the window 
            // with the data we've just put in, as doing the same for any secondary windows.
            base.LoadData();
        }
        #endregion
        #region OMethod: void DoLayout()
        public override void DoLayout()
            // Once a layout is done, we want to adjust the height of our window so that
            // it holds all of the contents (and thus we don't need a scrollbar. 
        {
            // Let the window calculate its height first
            base.DoLayout();

            // Then adjust our height to match it
            WndToolTip.Height = (int)Contents.Height +
                (int)WindowMargins.Height * 2;

            // Create a new Drawbuffer to reflect the changed height of the window. Normally
            // we would call SetSize, but this requires another call to DoLayout; thus this
            // situation where the layout drives the window size causes us to do things
            // like this.
            Draw = new DrawBuffer(this);
        }
        #endregion
        #region Cmd: OnSizeChanged
        protected override void OnSizeChanged(EventArgs e)
            // Override OWWindow, where a change in the window size triggers a call to DoLayout.
            // Here, we want the reverse, where a call to DoLayout triggers a size change. So
            // if we didn't override with a do-nothing method, we'd be in an endless loop.
        {
        }
        #endregion
    }

}
