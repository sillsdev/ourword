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
using JWdb.DataModel;
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

        // RoundedBorder Attrs ---------------------------------------------------------------
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
        }
        #endregion

        // Layout Calculations & Painting ----------------------------------------------------
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
        #region OMethod: void CalculateVerticals(float y, bool bRepositionOnly)
        public override void CalculateVerticals(float y, bool bRepositionOnly)
        {
            // Remember the top-left position and width
            Position = new PointF(Position.X, y);

            // Top Border
            y += Border.GetTotalWidth(BorderBase.BorderSides.Top);

            // Header
            Header.CalculateVerticals(y, bRepositionOnly);
            y += Header.Height;

            m_yContentsTop = y;

            // Allow for display of the bitmap if applicable
            y += CalculateBitmapHeightRequirement();

            // Layout the owned subitems, one below the other
            foreach (EItem item in SubItems)
            {
				item.CalculateVerticals(y, bRepositionOnly);
				y += item.Height;
            }

            m_yContentsBottom = y;

            y += Border.GetTotalWidth(BorderBase.BorderSides.Bottom);

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
            Border.Paint();

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
