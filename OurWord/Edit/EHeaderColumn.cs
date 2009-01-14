/**********************************************************************************************
 * Project: OurWord!
 * File:    EHeaderColumn.cs
 * Author:  John Wimbish
 * Created: 12 Jan 2009
 * Purpose: Column with an header region on top
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
using JWdb;
using OurWord.DataModel;
#endregion

namespace OurWord.Edit
{
    public class EHeaderColumn : EColumn
    {
        // Attrs -----------------------------------------------------------------------------
        #region Attr{g}: EContainer Header
        protected EContainer Header
        {
            get
            {
                Debug.Assert(null != m_Header);
                return m_Header;
            }
        }
        EContainer m_Header;
        #endregion

        #region Attr{g/s}: Styles Style
        public Styles Style
        {
            get
            {
                return m_Style;
            }
            set
            {
                m_Style = value;
            }
        }
        Styles m_Style;
        #endregion
        public enum Styles { Normal, RoundedBorder };

        // RoundedBorder Attrs ---------------------------------------------------------------
        #region Attr{g/s}: Color OuterBorderColor
        public Color OuterBorderColor
        {
            get
            {
                return m_OuterBorderColor;
            }
            set
            {
                m_OuterBorderColor = value;
            }
        }
        Color m_OuterBorderColor;
        #endregion
        #region Attr{g/s}: Color InnerBorderColor
        public Color InnerBorderColor
        {
            get
            {
                return m_InnerBorderColor;
            }
            set
            {
                m_InnerBorderColor = value;
            }
        }
        Color m_InnerBorderColor;
        #endregion
        #region Attr{g/s}: Color HeaderBackgroundColor
        public Color HeaderBackgroundColor
        {
            get
            {
                return m_HeaderBackgroundColor;
            }
            set
            {
                m_HeaderBackgroundColor = value;
            }
        }
        Color m_HeaderBackgroundColor;
        #endregion
        #region Attr{g/s}: Color ContentsBackgroundColor
        public Color ContentsBackgroundColor
        {
            get
            {
                return m_ContentsBackgroundColor;
            }
            set
            {
                m_ContentsBackgroundColor = value;
            }
        }
        Color m_ContentsBackgroundColor;
        #endregion
        #region Attr{g/s}: int RoundedBorderInternalMargin
        public int RoundedBorderInternalMargin
        {
            get
            {
                return m_nRoundedBorderInternalMargin;
            }
            set
            {
                m_nRoundedBorderInternalMargin = value;
            }
        }
        int m_nRoundedBorderInternalMargin;
        #endregion
        #region Attr{g/s}: int OuterRoundedBorderRadius
        public int OuterRoundedBorderRadius
        {
            get
            {
                return m_nOuterRoundedBorderRadius;
            }
            set
            {
                m_nOuterRoundedBorderRadius = value;
            }
        }
        int m_nOuterRoundedBorderRadius;
        #endregion
        #region VAttr{g}: bool IsRoundedBorder
        public bool IsRoundedBorder
        {
            get
            {
                return (Style == Styles.RoundedBorder);
            }
        }
        #endregion
        float m_yContentsTop;
        float m_yContentsBottom;

        // Scaffolding -----------------------------------------------------------------------
        #region Constructor(EContainer Header)
        public EHeaderColumn(EContainer _Header)
            : base()
        {
            // Save the header
            m_Header = _Header;
            m_Header.Owner = this;

            // Default Style is Normal (no borders)
            m_Style = Styles.Normal;

            // Rounded Border Style attrs
            m_OuterBorderColor = Color.DarkGray;
            m_InnerBorderColor = Color.DarkGray;
            m_HeaderBackgroundColor = Color.LightGreen;
            m_ContentsBackgroundColor = Color.LightYellow;
            m_nRoundedBorderInternalMargin = 4;
            m_nOuterRoundedBorderRadius = 12;
        }
        #endregion

        // Layout Calculations & Painting ----------------------------------------------------
        #region OAttr{g}: float AvailableWidthForOneSubitem
        public override float AvailableWidthForOneSubitem
        {
            get
            {
                float fAdjust = 0;

                if (IsRoundedBorder)
                    fAdjust = OuterRoundedBorderRadius * 2;

                return base.AvailableWidthForOneSubitem - fAdjust;
            }
        }
        #endregion
        #region OMethod: float CalculateSubItemX(EItem subitem)
        public override float CalculateSubItemX(EItem subitem)
        {
            float fAdjust = 0;

            if (IsRoundedBorder)
                fAdjust = OuterRoundedBorderRadius;

            return base.CalculateSubItemX(subitem) + fAdjust;
        }
        #endregion
        #region OMethod: void CalculateBlockWidths(Graphics g) - Include Header blocks
        public override void CalculateBlockWidths(Graphics g)
        {
            // Subitems as usual
            base.CalculateBlockWidths(g);

            // Header
            Header.CalculateBlockWidths(g);
        }
        #endregion
        #region OMethod: void CalculateContainerHorizontals()
        public override void CalculateContainerHorizontals()
        {
            // Base method correctly positions us within our owner, and calculates
            // for our subitems.
            base.CalculateContainerHorizontals();

            // Calculate for the header
            Header.CalculateContainerHorizontals();
        }
        #endregion
        #region OMethod: void CalculateContainerVerticals(float y, bool bRepositionOnly)
        public override void CalculateContainerVerticals(float y, bool bRepositionOnly)
        {
            // Remember the top-left position and width
            Position = new PointF(Position.X, y);

            // If we are displaying the footnote separator, then add a pixel to the
            // height to make room for it.
            y += FootnoteSeparatorHeight;

            // Rounded Borders: Allow room for the outer border top
            if (IsRoundedBorder)
                y += 1;

            // Header
            Header.CalculateContainerVerticals(y, bRepositionOnly);
            y += Header.Height;

            // Rounded Borders: Allow room for the inner border top
            if (IsRoundedBorder)
                y += 1;
            m_yContentsTop = y;

            // Allow for display of the bitmap if applicable
            y += CalculateBitmapHeightRequirement();

            // Layout the owned subitems, one below the other
            foreach (EContainer container in SubItems)
            {
                container.CalculateContainerVerticals(y, bRepositionOnly);
                y += container.Height;
            }

            m_yContentsBottom = y;

            // Rounded Borders: Allow room for the bottom border
            if (IsRoundedBorder)
            {
                y += 1;
                y += RoundedBorderInternalMargin;
                y += 1;
            }

            // Calculate the resulting height
            Height = (y - Position.Y);
        }
        #endregion
        #region OMethod: void OnPaint(Rectangle ClipRectangle)
        public override void OnPaint(Rectangle ClipRectangle)
        {
            // For performance, make sure we need to paint this container
            if (!ClipRectangle.IntersectsWith(IntRectangle))
                return;

            // Footnote Separator if indicated
            PaintFootnoteSeparator();

            // Rounded Rectangle Style: 
            if (IsRoundedBorder)
            {
                // Draw a filled rounded rectangle around the entire container 
                Pen penOuter = new Pen(OuterBorderColor);
                Brush brushOuter = new SolidBrush(HeaderBackgroundColor);
                Window.Draw.DrawRoundedRectangle(penOuter, brushOuter,
                    Rectangle, OuterRoundedBorderRadius);

                // Draw a filled rounder rectangle around the contents area
                Pen penInner = new Pen(InnerBorderColor);
                Brush brushInner = new SolidBrush(ContentsBackgroundColor);
                float fHeight = m_yContentsBottom - m_yContentsTop + 2; // allow 2 for border lines
                RectangleF rectInner = new RectangleF(
                    Position.X + RoundedBorderInternalMargin, m_yContentsTop,
                    Width - (RoundedBorderInternalMargin * 2), fHeight);
                Window.Draw.DrawRoundedRectangle(penInner, brushInner,
                    rectInner, OuterRoundedBorderRadius - RoundedBorderInternalMargin);
            }

            // Header
            Header.OnPaint(ClipRectangle);

            // Bitmap if indicated
            PaintBitmap();

            // Paint the subcontainers
            foreach (EItem item in SubItems)
                item.OnPaint(ClipRectangle);
        }
        #endregion
        #region OMethod: EBlock GetBlockAt(PointF pt)
        public override EBlock GetBlockAt(PointF pt)
        {
            // Is it our header?
            if (Header.ContainsPoint(pt))
                return Header.GetBlockAt(pt);

            // Otherwise, revert to superclass behavior of looking at subitems
            return base.GetBlockAt(pt);
        }
        #endregion
    }
}
