/**********************************************************************************************
 * Project: OurWord!
 * File:    EControl.cs
 * Author:  John Wimbish
 * Created: 14 Jan 2009
 * Purpose: Supports, e.g., an Edit control in a view
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
using System.Windows.Forms;
using System.Text;
using JWTools;
using OurWordData;
using OurWordData.DataModel;
#endregion

namespace OurWord.Edit
{
    public class EControl : EItem
    {
        #region Attr{g}: Control Control
        public Control Control
        {
            get
            {
                Debug.Assert(null != m_Control);
                return m_Control;
            }
        }
        Control m_Control;
        #endregion

        // Screen Region ---------------------------------------------------------------------
        const int c_nTopMargin = 4;   // Windows wants to overwrite the drawing above it.

        #region OAttr{g/s}: float Height
        public override float Height
        {
            get
            {
                return Control.Height + c_nTopMargin;
            }
            set
            {
                Control.Height = (int)value - c_nTopMargin;
            }
        }
        #endregion
        #region OAttr{g/s}: float Width
        public override float Width
        {
            get
            {
                return Control.Width;
            }
            set
            {
                Control.Width = (int)value;
            }
        }
        #endregion

        // Scaffolding -----------------------------------------------------------------------
        #region Constructor(OWWindow, Control)
        public EControl(OWWindow wnd, Control _Control)
            : base()
        {
            m_Control = _Control;
            wnd.Controls.Add(m_Control);
        }
        #endregion
        #region OMethod: EBlock GetBlockAt(pt) - return null
        public override EBlock GetBlockAt(PointF pt)
        {
            return null;
        }
        #endregion
        #region OMethod: void Clear()
        public override void Clear()
        {
            Control.Dispose();
            Window.Controls.Remove(Control);
            m_Control = null;
        }
        #endregion

        // Layout Calculations ---------------------------------------------------------------
        #region VMethod: void CalculateHorizontals()
        public virtual void CalculateHorizontals()
        {
            // The X position is given us from the EContainer that owns it
            float xleft = Owner.CalculateSubItemX(this);
            Position = new PointF(xleft, Position.Y);
            Control.Left = (int)xleft;

            // The width will be that of the control, which is set when the subclass is
            // created. Therefore we do nothing here. If we want the control to extend
            // across the entire width of the container, then we use that container's
            // AvailableWidthForOneSubitem method (see EToolStrip)
        }
        #endregion
		#region OMethod: void CalculateVerticals(y)
		public override void CalculateVerticals(float y)
        {
            // Set to the top-left position and width
            Position = new PointF(Position.X, y);
        }
        #endregion
        #region OMethod: void SetOwnedControlsVisibility(bVisible)
        public override void SetOwnedControlsVisibility(bool bVisible)
        {
            Control.Visible = bVisible;
        }
        #endregion

		// Painting --------------------------------------------------------------------------
		#region OMethod: void PaintControls()
		public override void PaintControls()
		{
			float fScrollAmount = Window.ScrollBarPosition;
			Control.Top = c_nTopMargin + (int)(Position.Y - fScrollAmount);
		}
		#endregion
	}

    public class EToolStrip : EControl
    {
        #region CLASS: ToolStripRenderedEx : ToolStripSystemRenderer
        public class ToolStripRenderedEx : ToolStripSystemRenderer
        {
            #region Constructor()
            public ToolStripRenderedEx()
                : base()
            {
            }
            #endregion
            #region OMethod: void OnRenderToolStripBorder(e) - supress the border
            protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
            {
                // Do Nothing. This gets rid of the white line that was being painted
                // at the bottom.
            }
            #endregion
        }
        #endregion

        // Screen Region ---------------------------------------------------------------------
        #region OAttr{g/s}: float Height
        public override float Height
        {
            get
            {
                return Control.Height;
            }
            set
            {
                Control.Height = (int)value;
            }
        }
        #endregion

        // Attrs -----------------------------------------------------------------------------
        #region VAttr{g}: ToolStrip ToolStrip
        public ToolStrip ToolStrip
        {
            get
            {
                Debug.Assert(null != Control as ToolStrip);
                return Control as ToolStrip;
            }
        }
        #endregion

        // Scaffolding -----------------------------------------------------------------------
        #region Constructor()
        public EToolStrip(OWWindow wnd)
            : base( wnd, new ToolStrip())
        {
            ToolStrip.Anchor = AnchorStyles.None;
            ToolStrip.GripStyle = ToolStripGripStyle.Hidden;
            ToolStrip.TabStop = false;
            ToolStrip.BackColor = wnd.BackColor;
            ToolStrip.Renderer = new ToolStripRenderedEx();
            ToolStrip.LayoutStyle = ToolStripLayoutStyle.HorizontalStackWithOverflow;

            // If the window is too small in width, this causes a dropdown button to appear 
            // by which the user can access the other buttons on the toolbar
            ToolStrip.AutoSize = false;
        }
        #endregion

        // Layout Calculations ---------------------------------------------------------------
        #region OMethod: void CalculateHorizontals()
        public override void CalculateHorizontals()
            // We want our width to be as big as the owning container allows
        {
            // Pick up the Position.X from the superclass
            base.CalculateHorizontals();

            Width = Owner.AvailableWidthForOneSubitem;
        }
        #endregion
		#region OMethod: void CalculateVerticals(y)
		public override void CalculateVerticals(float y)
        {
            // Set to the top-left position and width
            Position = new PointF(Position.X, y);
        }
        #endregion
    }

}
