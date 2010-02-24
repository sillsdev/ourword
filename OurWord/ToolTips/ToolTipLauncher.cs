#region ***** ToolTipLauncher.cs *****
/**********************************************************************************************
 * Project: Our Word!
 * File:    ToolTipLauncher.cs
 * Author:  John Wimbish
 * Created: 11 Feb 2010
 * Purpose: Implements a mouse hover to determine when to launch the tooltip
 * Legal:   Copyright (c) 2005-10, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
using System;
using System.Windows.Forms;
using OurWord.Edit;
#endregion

namespace OurWord.ToolTips
{
    class ToolTipLauncher
    {
        static private EBlock s_Block;
        #region VAttr{g}: bool IsBlockWithTooltip
        static bool IsBlockWithTooltip
        {
            get
            {
                return (null != s_Block && s_Block.HasToolTip());
            }
        }
        #endregion

        // Timer -----------------------------------------------------------------------------
        static Timer s_Timer;
        const int c_nTooltipTimerInterval = 500;
        #region Cmd: OnTooltipTimerTick
        static void OnTooltipTimerTick(object sender, EventArgs e)
        {
            if (IsBlockWithTooltip)
            {
                // Remember the block before we remove it below
                var block = s_Block;

                // If we don't remove the block, the timer will continue
                // launching the tooltip! Because we're launching the tip
                // as a modal, we have to do this prior to calling Launch
                SetBlock(null);

                // The block code calls the model ShowDialog() method
                block.LaunchToolTip();
            }
        }
        #endregion

        // Scaffolding -----------------------------------------------------------------------
        #region Constructor()
        public ToolTipLauncher()
        {
            s_Timer = new Timer { Interval = c_nTooltipTimerInterval };
            s_Timer.Tick += OnTooltipTimerTick;
        }
        #endregion

        // Public Interface ------------------------------------------------------------------
        #region SMethod: void SetBlock(EBlock block)
        public static void SetBlock(EBlock block)
        {
            s_Block = block;

            if (s_Timer.Enabled)
                s_Timer.Stop();

            if (IsBlockWithTooltip)
                s_Timer.Start();
        }
        #endregion
        #region SMethod: void LaunchNow()
        static public void LaunchNow()
        {
            if (s_Timer.Enabled)
                s_Timer.Stop();
            OnTooltipTimerTick(null, null);
        }
        #endregion
        #region SMethod: void LaunchNow(EBlock)
        static public void LaunchNow(EBlock block)
        {
            s_Block = block;
            LaunchNow();
        }
        #endregion
    }
}
