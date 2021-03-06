﻿/**********************************************************************************************
 * Project: OurWord!
 * File:    ECollapsableHeaderColumn.cs
 * Author:  John Wimbish
 * Created: 08 Jan 2009
 * Purpose: Column with an plus/minus sign for collapsing/expanding contents
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
using OurWord.Edit.Blocks;
using OurWordData;
using OurWordData.DataModel;
#endregion

namespace OurWord.Edit
{
    public class ECollapsableHeaderColumn : EHeaderColumn
    {
        // Attrs -----------------------------------------------------------------------------
        #region Attr{g}: ECollapsableIcon PlusMinusIcon
        ECollapsableIcon PlusMinusIcon
        {
            get
            {
                Debug.Assert(null != m_PlusMinusIcon);
                return m_PlusMinusIcon;
            }
        }
        ECollapsableIcon m_PlusMinusIcon;
        #endregion
        #region VAttr{g/s}: bool IsCollapsed
        public bool IsCollapsed
        {
            get
            {
                return PlusMinusIcon.IsCollapsed;
            }
            set
            {
                PlusMinusIcon.IsCollapsed = value;
            }
        }
        #endregion
        #region Attr{g/s}: bool ShowHeaderWhenExpanded
        public bool ShowHeaderWhenExpanded
        {
            get
            {
                return m_bShowHeaderWhenExpanded;
            }
            set
            {
                m_bShowHeaderWhenExpanded = value;
            }
        }
        bool m_bShowHeaderWhenExpanded;
        #endregion

        // Scaffolding -----------------------------------------------------------------------
        #region Constructor()
        public ECollapsableHeaderColumn(EContainer _Header)
            : base(_Header)
        {
            // The icon to expand/collapse the column
            m_PlusMinusIcon = new ECollapsableIcon();
            m_PlusMinusIcon.Owner = this;

            // By default, we'll start out collapsed
            PlusMinusIcon.IsCollapsed = true;

            // By default, we'll show the header when expanded
            m_bShowHeaderWhenExpanded = true;
        }
        #endregion

        // Misc ------------------------------------------------------------------------------
        #region Method: void ToggleCollapsed()
        public void ToggleCollapsed()
        {
            (new ToggleCollapsedHeader(Window, this)).Do();
        }
        #endregion

        // Selections: Override to account for IsCollapsed -----------------------------------
        #region OMethod: bool Select_FirstWord()
        public override bool Select_FirstWord()
        {
            if (IsCollapsed)
                return false;

            return base.Select_FirstWord();
        }
        #endregion
        #region OMethod: bool Select_NextWord_Begin(aiStack)
        public override bool Select_NextWord_Begin(ArrayList aiStack)
        {
            if (IsCollapsed)
                return false;

            return base.Select_NextWord_Begin(aiStack);
        }
        #endregion
        #region OMethod: bool Select_PrevWord(aiStack, bSelectAtEndOfWord)
        public override bool Select_PrevWord(ArrayList aiStack, bool bSelectAtEndOfWord)
        {
            if (IsCollapsed)
                return false;

            return base.Select_PrevWord(aiStack, bSelectAtEndOfWord);
        }
        #endregion
        #region VMethod: bool Select_LastWord_End()
        public override bool Select_LastWord_End()
        {
            if (IsCollapsed)
                return false;

            return base.Select_LastWord_End();
        }
        #endregion

        // Layout Calculations & Painting ----------------------------------------------------
        const int c_SpaceAfterPlusMinusIcon = 3;
        #region OAttr{g}: float AvailableWidthForOneSubitem
        public override float AvailableWidthForOneSubitem
            // Our available width for subitems is our width less what is alloted for
            // the PlusMinus Icon
        {
            get
            {
                return Width - PlusMinusIcon.Width - c_SpaceAfterPlusMinusIcon;
            }
        }
        #endregion
        #region OMethod: float CalculateSubItemX(EItem subitem)
        public override float CalculateSubItemX(EItem subitem)
        {
            return Position.X + PlusMinusIcon.Width + c_SpaceAfterPlusMinusIcon;
        }
        #endregion
        #region OMethod: void CalculateContainerHorizontals()
        public override void CalculateContainerHorizontals()
        {
            // We must call the base method, as it has the logic to correctly position
            // us within the row of columns. It also calculates for subitems, which 
            // is a waste of time if we are collapsed, but no harm is otherwise done.
            base.CalculateContainerHorizontals();

            // PlusMinus icon position
            PlusMinusIcon.Position = new PointF(
                Owner.CalculateSubItemX(this),
                Position.Y);
        }
        #endregion
        #region OMethod: void CalculateVerticals(y)
        public override void CalculateVerticals(float y)
        {
            // Remember the top-left position and width
            Position = new PointF(Position.X, y);

            // Top Border
            y += Border.GetTotalWidth(BorderBase.BorderSides.Top);

            // PlusMinus icon position
            PlusMinusIcon.Position = new PointF(
                Owner.CalculateSubItemX(this),
                y);

            // Header
            if (ShowHeaderWhenExpanded || IsCollapsed)
            {
                Header.CalculateVerticals(y);
                y += Header.Height;
            }

            // If we're expanded, then show the subitems
            if (!IsCollapsed)
            {
                // Allow for display of the bitmap if applicable
                y += CalculateBitmapHeightRequirement();

                // Layout the owned subitems, one below the other
                foreach (EItem item in SubItems)
                {
					item.CalculateVerticals(y);
					y += item.Height;
                }
            }

            // Bottom Border
            y += Border.GetTotalWidth(BorderBase.BorderSides.Bottom);

            // Calculate the resulting height
            Height = (y - Position.Y);
        }
        #endregion
        #region OMethod: void OnPaint(IDraw, ClipRectangle)
        public override void OnPaint(IDraw draw, Rectangle clipRectangle)
        {
            // Set the owned controls visibility according to whether or not
            // we are collapsed. We want them invisible if collapsed, so that
            // the Window does not attempt to draw them.
            // For owned controls, we need to set their visibility regardless of
            // whether the clip rectangle; otherwise they'll always be visible because
            // the code to hide them would never be called (e.g., if the container has
            // been scrolled off the screen.
            SetOwnedControlsVisibility(!IsCollapsed);

            // For performance, make sure we need to paint this container
            if (!clipRectangle.IntersectsWith(IntRectangle))
                return;

            // Borders if indicated
            Border.Paint(draw);

            // PlusMinus Control
            PlusMinusIcon.OnPaint(draw, clipRectangle);

            // Header
            if (ShowHeaderWhenExpanded || IsCollapsed)
                Header.OnPaint(draw, clipRectangle);

            // If we're expanded, then show the subitems
            if (!IsCollapsed)
            {
                // Bitmap if indicated
                PaintBitmap(draw);

                // Paint the subcontainers
                foreach (var item in SubItems)
                    item.OnPaint(draw, clipRectangle);
            }
        }
        #endregion
        #region OMethod: EBlock GetBlockAt(PointF pt)
        public override EBlock GetBlockAt(PointF pt)
        {
            // Is it our PlusMinus icon?
            if (PlusMinusIcon.ContainsPoint(pt))
                return PlusMinusIcon;

            // Otherwise, revert to superclass behavior of looking at subitems
            return base.GetBlockAt(pt);
        }
        #endregion
    }

    #region Class: ECollapsableIcon
    public class ECollapsableIcon : EBlock
    {
        // Attrs -----------------------------------------------------------------------------
        #region VAttr{g}: ECollapsableHeaderColumn CollapsableHeaderColumn
        public ECollapsableHeaderColumn CollapsableHeaderColumn
            // We want to make sure that (1) the Owner has been set, and (2) that it
            // it an ECollapsableHeaderColumn.
        {
            get
            {
                ECollapsableHeaderColumn col = Owner as ECollapsableHeaderColumn;
                Debug.Assert(null != col);
                return col;
            }
        }
        #endregion
        #region Attr{g/s}: bool IsCollapsed
        public bool IsCollapsed
        {
            get
            {
                return m_bIsCollapsed;
            }
            set
            {
                m_bIsCollapsed = value;
            }
        }
        bool m_bIsCollapsed;
        #endregion

        #region OAttr{g}: float Height
        public override float Height
        {
            get
            {
                return m_xyDimension;
            }
            set
            {
                Debug.Assert(false, "Can't set the height of an ECollapsableIcon");
            }
        }
        #endregion
        #region OAttr{g}: float Width
        public override float Width
        {
            get
            {
                return m_xyDimension;
            }
            set
            {
                Debug.Assert(false, "Can't set the weight of an ECollapsableIcon");
            }
        }
        #endregion
        int m_xyDimension;

        // Scaffolding -----------------------------------------------------------------------
        #region Constructor()
        public ECollapsableIcon()
            : base(null, "")
        {
            // Set the width and height of the icon
            m_xyDimension = 10;
        }
        #endregion

        // Behavior Overrides ----------------------------------------------------------------
        #region OAttr{g}: Cursor MouseOverCursor - Display a Hand on a mouse-over
        public override Cursor MouseOverCursor
        {
            get
            {
                return Cursors.Hand;
            }
        }
        #endregion
        #region Cmd: cmdLeftMouseClick - Toggle expand/collapse mode
        public override void cmdLeftMouseClick(PointF pt)
        {
            CollapsableHeaderColumn.ToggleCollapsed();
        }
        #endregion
        #region Cmd: cmdLeftMouseDoubleClick - same as single click
        public override void cmdLeftMouseDoubleClick(PointF pt)
        {
            cmdLeftMouseClick(pt);
        }
        #endregion
        #region OMethod: override void Paint(IDraw, ClipRectangle)
        public override void OnPaint(IDraw draw, Rectangle clipRectangle)
        {
            const int cOffset = 2;

            var pen = Pens.Navy;

            // Draw the rectangle
            draw.DrawRectangle(pen, Rectangle);

            // Draw the horizontal line
            var yMid = Rectangle.Y + (Height / 2.0F);
            draw.DrawLine(pen,
                Position.X + cOffset, yMid,
                Position.X + Width - cOffset, yMid);

            // If collapsed, add the vertical line
            if (!IsCollapsed) 
                return;
            var xMid = Rectangle.X + (Width / 2.0F);
            draw.DrawLine(pen,
                xMid, Position.Y + cOffset,
                xMid, Position.Y + Height - cOffset);
        }
        #endregion
        #region OMethod: void CalculateWidth()
        public override void CalculateWidth()
        {
            // No need to do anyting
        }
        #endregion
    }
    #endregion
}
