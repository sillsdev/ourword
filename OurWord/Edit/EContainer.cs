/**********************************************************************************************
 * Project: OurWord!
 * File:    EContainer.cs
 * Author:  John Wimbish
 * Created: 14 Oct 2008
 * Purpose: The various classes holding editor layout
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
using JWTools;
using JWdb;
using JWdb.DataModel;
#endregion

// TODO: Implement Left and Right Borders

namespace OurWord.Edit
{
    #region CLASS: EItem
    public class EItem
    {
        // Ownership Hierarchy ---------------------------------------------------------------
        #region Attr{g/s}: EContainer Owner
        public EContainer Owner
        {
            get
            {
                return m_Owner;
            }
            set
            {
                m_Owner = value;
            }
        }
        protected EContainer m_Owner;
        #endregion
        #region VirtVAttr{g}: ERoot Root - top container of the entire hierarchy; owns All others
        virtual public ERoot Root
        {
            get
            {
                Debug.Assert(null != Owner);
                return Owner.Root;
            }
        }
        #endregion
        #region Attr{g}: EContainer TopContainer - Directly owned by the Root
        public EContainer TopContainer
        {
            get
            {
                Debug.Assert(null != Owner);

                if (null != Owner as ERoot)
                {
                    EContainer container = this as EContainer;
                    Debug.Assert(null != container);
                    return container;
                }

                return Owner.TopContainer;
            }
        }
        #endregion
        #region VMethod: void SetOwnedControlsVisibility(bVisible)
        public virtual void SetOwnedControlsVisibility(bool bVisible)
            // top-level: do nothing
        {
        }
        #endregion

        // Screen Region ---------------------------------------------------------------------
        #region Attr{g/s}: PointF Position - top,left coord
        virtual public PointF Position
        {
            get
            {
                return m_ptPosition;
            }
            set
            {
                m_ptPosition = value;
            }
        }
        private PointF m_ptPosition;
        #endregion
        #region VirtAttr{g/s}: float Height - pixel height of this item
        virtual public float Height
        {
            get
            {
                return m_fHeight;
            }
            set
            {
                m_fHeight = value;
            }
        }
        float m_fHeight = 0;
        #endregion
        #region VirtAttr{g/s}: float Width - pixel width of this item
        virtual public float Width
        {
            get
            {
                return m_fWidth;
            }
            set
            {
                m_fWidth = value;
            }
        }
        float m_fWidth = 0;
        #endregion
        #region VAttr{g}: RectangleF Rectangle
        public RectangleF Rectangle
        {
            get
            {
                return new RectangleF(Position, new SizeF(Width, Height));
            }
        }
        #endregion
        #region VAttr{g}: Rectangle IntRectangle
        public Rectangle IntRectangle
        {
            get
            {
                int x = (int)Position.X;
                int y = (int)Position.Y;
                int w = (int)Width;
                int h = (int)Height;
                return new Rectangle(x, y, w, h);
            }
        }
        #endregion
        #region VirtMethod: bool ContainsPoint(PointF)
        virtual public bool ContainsPoint(PointF pt)
        {
            return Rectangle.Contains(pt);
        }
        #endregion
        #region VirtMethod: EBlock GetBlockAt(PointF)
        public virtual EBlock GetBlockAt(PointF pt)
        {
            Debug.Assert(false, "Subclasses need to implement this.");
            return null;
        }
        #endregion

        // Attrs -----------------------------------------------------------------------------
        #region Attr{g}: bool IsEditable - if F, blocks editing in this and its owned objects
        virtual public bool IsEditable
        {
            get
            {
                return true;
            }
            set
            {
            }
        }
        #endregion
        #region Attr{g/s}: object Tag - Like Microsoft does in their controls
        public object Tag
        {
            get
            {
                return m_Tag;
            }
            set
            {
                m_Tag = value;
            }
        }
        object m_Tag;
        #endregion

        // Scaffolding -----------------------------------------------------------------------
        #region Constructor()
        public EItem()
        {
        }
        #endregion
        #region VAttr{g}: OWWindow Window
        public virtual OWWindow Window
        {
            get
            {
                OWWindow wnd = Root.Window;
                Debug.Assert(null != wnd);
                return wnd;
            }
        }
        #endregion
        #region VMethod: void Clear()
        public virtual void Clear()
        {
            // Used (in subclasses) to clear out any subitems and to dispose of any resources
        }
        #endregion

		// Layout Calculations ---------------------------------------------------------------
		#region VirtMethod: void CalculateVerticals(y, bRepositionOnly)
		virtual public void CalculateVerticals(float y, bool bRepositionOnly)
		// Layout any children, and calculate the Height. 
		//
		// Parameters
		//   yTop - the top pixel for the container
		//   bRepositionOnly - we just adjust the y values, we don't recalculate the heights,
		//        or, e.g., in the case of a paragraph, re-layout all of the elements.
		//        (e.g., when one paragraph has gotten larger, and the ones below it need to 
		//        shift down.
		{
			// This will be different for every subclass
			Debug.Assert(false);
		}
		#endregion

        // Painting --------------------------------------------------------------------------
        #region VirtMethod: void OnPaint(ClipRectangle)
        virtual public void OnPaint(Rectangle ClipRectangle)
        {
        }
        #endregion
		#region VirtMethod: void PaintControls()
		virtual public void PaintControls()
		{
		}
		#endregion
	}
    #endregion

    #region CLASS: EContainer
    public class EContainer : EItem, IEnumerator
    {
        // Optional Borders ------------------------------------------------------------------
        #region CLASS: BorderBase
        public class BorderBase
        {
            // Margin and Padding ------------------------------------------------------------
            #region CLASS: RectOffset
            public class RectOffset
            {
                #region Attr{g/s}: int Top
                public int Top
                {
                    get
                    {
                        return m_nTop;
                    }
                    set
                    {
                        m_nTop = value;
                    }
                }
                int m_nTop;
                #endregion
                #region Attr{g/s}: int Bottom
                public int Bottom
                {
                    get
                    {
                        return m_nBottom;
                    }
                    set
                    {
                        m_nBottom = value;
                    }
                }
                int m_nBottom;
                #endregion
                #region Attr{g/s}: int Left
                public int Left
                {
                    get
                    {
                        return m_nLeft;
                    }
                    set
                    {
                        m_nLeft = value;
                    }
                }
                int m_nLeft;
                #endregion
                #region Attr{g/s}: int Right
                public int Right
                {
                    get
                    {
                        return m_nRight;
                    }
                    set
                    {
                        m_nRight = value;
                    }
                }
                int m_nRight;
                #endregion

                #region Constructor()
                public RectOffset()
                {
                }
                #endregion
            }
            #endregion
            #region Attr{g/s}: RectOffset Margin - outside of border
            public RectOffset Margin
            {
                get
                {
                    Debug.Assert(null != m_Margin);
                    return m_Margin;
                }
                set
                {
                    m_Margin = value;
                }
            }
            RectOffset m_Margin;
            #endregion
            #region Attr{g/s}: RectOffset Padding - inside of border
            public RectOffset Padding
            {
                get
                {
                    Debug.Assert(null != m_Padding);
                    return m_Padding;
                }
                set
                {
                    m_Padding = value;
                }
            }
            RectOffset m_Padding;
            #endregion
            #region VAttr{g}: RectangleF BorderRectangle
            protected RectangleF BorderRectangle
            {
                get
                {
                    RectangleF r = new RectangleF(LeftBorder, TopBorder,
                        RightBorder - LeftBorder + 1, BottomBorder - TopBorder + 1);
                    return r;
                }
            }
            #endregion
            #region Method: int GetTotalWidth(BorderSides side)
            public int GetTotalWidth(BorderSides side)
            {
                int c = 0;

                // Are we wanting to get the top side?
                if ((side & BorderSides.Top) == BorderSides.Top)
                {
                    // Add the Margin
                    c += Margin.Top;

                    // If the Top Border is turned on, then add its width
                    if ((BorderPlacement & BorderSides.Top) == BorderSides.Top)
                        c += BorderWidth;

                    // Add the padding
                    c += Padding.Top;
                }

                // Same logic for the other three sides
                if ((side & BorderSides.Bottom) == BorderSides.Bottom)
                {
                    c += Margin.Bottom;
                    if ((BorderPlacement & BorderSides.Bottom) == BorderSides.Bottom)
                        c += BorderWidth;
                    c += Padding.Bottom;
                }

                if ((side & BorderSides.Left) == BorderSides.Left)
                {
                    c += Margin.Left;
                    if ((BorderPlacement & BorderSides.Left) == BorderSides.Left)
                        c += BorderWidth;
                    c += Padding.Left;
                }

                if ((side & BorderSides.Right) == BorderSides.Right)
                {
                    c += Margin.Right;
                    if ((BorderPlacement & BorderSides.Right) == BorderSides.Right)
                        c += BorderWidth;
                    c += Padding.Right;
                }

                return c;
            }
            #endregion

            // Drawing Attributes ------------------------------------------------------------
            #region Attr{g/s}; Color BorderColor
            public Color BorderColor
            {
                get
                {
                    return m_BorderColor;
                }
                set
                {
                    m_BorderColor = value;
                }
            }
            Color m_BorderColor = Color.Black;
            #endregion
            #region Attr{g/s}; Color FillColor
            public Color FillColor
            {
                get
                {
                    return m_FillColor;
                }
                set
                {
                    m_FillColor = value;
                }
            }
            Color m_FillColor = Color.Empty;
            #endregion
            #region Attr{g/s}: int BorderWidth
            public int BorderWidth
            {
                get
                {
                    return m_nBorderWidth;
                }
                set
                {
                    Debug.Assert(value >= 0 && value < 5);
                    m_nBorderWidth = value;
                }
            }
            int m_nBorderWidth = 0;
            #endregion
            #region VAttr{g}: Pen BorderPen
            protected Pen BorderPen
            {
                get
                {
                    return new Pen(BorderColor, BorderWidth);
                }
            }
            #endregion
            #region VAttr{g}: Brush FillBrush
            protected Brush FillBrush
            {
                get
                {
                    if (Color.Empty != FillColor)
                        return new SolidBrush(FillColor);
                    return Brushes.Transparent;
                }
            }
            #endregion

            // Which Sides to Display --------------------------------------------------------
            #region Enum: BorderSides - Top, Bottom, All, None, etc.
            public enum BorderSides
            {
                Top = 1,
                Bottom = 2,
                Left = 4,
                Right = 8,
                All = Top | Bottom | Left | Right,
                LeftAndRight = Left | Right,
                TopAndBottom = Top | Bottom,
                None = 0
            }
            #endregion
            #region Attr{g/s}: BorderSides BorderPlacement
            public BorderSides BorderPlacement
            {
                get
                {
                    return m_BorderPlacement;
                }
                set
                {
                    m_BorderPlacement = value;
                }
            }
            BorderSides m_BorderPlacement = BorderSides.None;
            #endregion

            // Scaffolding -------------------------------------------------------------------
            #region Constructor(EItem)
            public BorderBase(EItem item)
            {
                m_Item = item;

                m_Margin = new RectOffset();
                m_Padding = new RectOffset();
            }
            #endregion
            #region Attr{g}: EItem Item
            EItem Item
            {
                get
                {
                    Debug.Assert(null != m_Item);
                    return m_Item;
                }
            }
            EItem m_Item;
            #endregion
            #region VAttr{g}: OWWindow.DrawBuffer Draw
            protected OWWindow.DrawBuffer Draw
            {
                get
                {
                    return Item.Window.Draw;
                }
            }
            #endregion

            // Border locations (i.e., adjusted for margin) ----------------------------------
            #region Attr{g}: float LeftBorder
            protected float LeftBorder
            {
                get
                {
                    return Item.Rectangle.Left + Margin.Left;
                }
            }
            #endregion
            #region Attr{g}: float RightBorder
            protected float RightBorder
            {
                get
                {
                    return Item.Rectangle.Right - Margin.Right;
                }
            }
            #endregion
            #region Attr{g}: float TopBorder
            protected float TopBorder
            {
                get
                {
                    return Item.Rectangle.Top + Margin.Top;
                }
            }
            #endregion
            #region Attr{g}: float BottomBorder
            protected float BottomBorder
            {
                get
                {
                    return Item.Rectangle.Bottom - Margin.Bottom;
                }
            }
            #endregion

            // Layout and Drawing ------------------------------------------------------------
            #region VMethod: void Paint()
            virtual public void Paint()
            {
            }
            #endregion
        }
        #endregion
        #region CLASS: SquareBorder : BorderBase
        public class SquareBorder : BorderBase
        {
            #region Constructor(EItem)
            public SquareBorder(EItem item)
                : base(item)
            {
                BorderPlacement = BorderSides.All;
                BorderWidth = 1;
                BorderColor = Color.Black;
            }
            #endregion

            #region OMethod: void Paint()
            public override void Paint()
            {
                Pen pen = BorderPen;
                OWWindow.DrawBuffer d = Draw;

                PointF LeftTop = new PointF(LeftBorder, TopBorder);
                PointF LeftBottom = new PointF(LeftBorder, BottomBorder);
                PointF RightTop = new PointF(RightBorder, TopBorder);
                PointF RightBottom = new PointF(RightBorder, BottomBorder);

                if ((BorderPlacement & BorderSides.Top) == BorderSides.Top)
                    d.Line(pen, LeftTop, RightTop);
                if ((BorderPlacement & BorderSides.Bottom) == BorderSides.Bottom)
                    d.Line(pen, LeftBottom, RightBottom);

                if ((BorderPlacement & BorderSides.Left) == BorderSides.Left)
                    d.Line(pen, LeftTop, LeftBottom);
                if ((BorderPlacement & BorderSides.Right) == BorderSides.Right)
                    d.Line(pen, RightTop, RightBottom);
            }
            #endregion
        }
        #endregion
        #region CLASS: RoundedBorder : BorderBase
        public class RoundedBorder : BorderBase
        {
            #region Attr{g/s}: int RoundedBorderRadius
            public int RoundedBorderRadius
            {
                get
                {
                    return m_nRoundedBorderRadius;
                }
                set
                {
                    m_nRoundedBorderRadius = value;
                }
            }
            int m_nRoundedBorderRadius = 10;
            #endregion

            #region Constructor(EItem)
            public RoundedBorder(EItem item, int nRoundedBorderRadius)
                : base(item)
            {
                m_nRoundedBorderRadius = nRoundedBorderRadius;

                BorderPlacement = BorderSides.All;
                BorderWidth = 1;
                BorderColor = Color.Black;
            }
            #endregion

            #region OMethod: void Paint()
            public override void Paint()
            {
                Draw.DrawRoundedRectangle(BorderPen, FillBrush,
                    BorderRectangle, RoundedBorderRadius);
            }
            #endregion
        }
        #endregion
        #region CLASS: FootnoteSeparatorBorder : BorderBase
        public class FootnoteSeparatorBorder : BorderBase
        {
            #region Attr{g/s}: float SeparatorWidth
            public float SeparatorWidth
            {
                get
                {
                    return m_SeparatorWidth;
                }
                set
                {
                    m_SeparatorWidth = value;
                }
            }
            float m_SeparatorWidth = 0;
            #endregion

            #region Constructor(EItem)
            public FootnoteSeparatorBorder(EItem item, float fSeparatorWidth)
                : base(item)
            {
                m_SeparatorWidth = fSeparatorWidth;

                BorderPlacement = BorderSides.All;
                BorderWidth = 1;
                BorderColor = Color.Black;
            }
            #endregion

            #region OMethod: void Paint()
            public override void Paint()
            {
                if (0 == SeparatorWidth)
                    return;

                Draw.Line(BorderPen,
                   new PointF(LeftBorder, TopBorder),
                   new PointF(LeftBorder + SeparatorWidth, TopBorder));
            }
            #endregion
        }
        #endregion
        #region Attr{g/s}: BorderBase Border
        public BorderBase Border
        {
            get
            {
                Debug.Assert(null != m_Border);
                return m_Border;
            }
            set
            {
                Debug.Assert(null != value);
                m_Border = value;
            }
        }
        BorderBase m_Border;
        #endregion

        // Optional Bitmap at top of container -----------------------------------------------
        #region Attr{g/s}: Bitmap Bmp - the picture's bitmap
        public Bitmap Bmp
        {
            get
            {
                return m_bmp;
            }
            set
            {
                m_bmp = value;
            }
        }
        Bitmap m_bmp = null;
        #endregion
        protected const int c_BitmapMargin = 5;         // vert marg above/below the bitmap
        protected const int c_BitmapPadAtBottom = 2;    // pixels at bottom to ensure line gets drawn
        #region Attr{g}: bool HasBitmap
        public bool HasBitmap
        {
            get
            {
                return (Bmp != null);
            }
        }
        #endregion
        #region Method: void PaintBitmap()
        protected void PaintBitmap()
        {
            if (!HasBitmap)
                return;

            // Line above and below
            Pen pen = new Pen(Color.Black);
            Window.Draw.Line(pen,
                Position,
                new PointF(Position.X + Rectangle.Width, Position.Y));
            Window.Draw.Line(pen,
                new PointF(Position.X, Position.Y + Rectangle.Height - 2),
                new PointF(Position.X + Rectangle.Width, Position.Y + Rectangle.Height - 2));

            // Draw the bitmap
            float xBmp = Position.X + (Rectangle.Width - Bmp.Width) / 2;
            float yBmp = Position.Y + 1 + c_BitmapMargin;
            Window.Draw.Image(Bmp, new PointF(xBmp, yBmp));
        }
        #endregion
        #region Method: float CalculateBitmapHeightRequirement()
        protected float CalculateBitmapHeightRequirement()
        {
            if (!HasBitmap)
                return 0;

            // Allow one pixel room for the line above the drawing
            float fHeightBmp = 1;

            // Allow room for the margin above the bitmap
            fHeightBmp += c_BitmapMargin;

            // Allow room for the bitmap itself
            fHeightBmp += Bmp.Height;

            // Allow room for the margin below the bitmap
            fHeightBmp += c_BitmapMargin;

            // Allow one pixel for the line below the drawing
            fHeightBmp += 1;

            // Allow a touch of extra padding so the line draws correctly
            fHeightBmp += c_BitmapPadAtBottom;

            return fHeightBmp;
        }
        #endregion

        // Owned containers ------------------------------------------------------------------
        #region Attr{g}: EItem[] SubItems
        public EItem[] SubItems
        {
            get
            {
                Debug.Assert(null != m_vSubItems);
                return m_vSubItems;
            }
        }
        EItem[] m_vSubItems;
        #endregion
        #region IEnumerator for SubItems
        #region Attribute: IEnumerator.Current - Returns the current subcontainer
        public object Current
        {
            get
            {
                if (-1 == m_iEnumeratorPos ||
                    m_iEnumeratorPos == m_vSubItems.Length ||
                    !m_bEnumeratorValid)
                {
                    throw new InvalidOperationException();
                }
                return m_vSubItems[m_iEnumeratorPos];
            }
        }
        #endregion
        #region Method: void IEnumerator.Reset() - rewinds the position to before the first object
        public void Reset()
        {
            if (!m_bEnumeratorValid)
                throw new InvalidOperationException();
            m_iEnumeratorPos = -1;
        }
        #endregion
        #region Method: bool IEnumerator.MoveNext() - moves to the next object in the sequence
        public bool MoveNext()
        {
            if (!m_bEnumeratorValid)
                throw new InvalidOperationException();
            if (m_iEnumeratorPos < m_vSubItems.Length - 1)
            {
                ++m_iEnumeratorPos;
                return true;
            }
            return false;
        }
        #endregion
        #region Method: IEnumerator.GetEnumerator() - initializes the enumeration
        public virtual IEnumerator GetEnumerator()
        {
            m_bEnumeratorValid = true;
            Reset();
            return this;
        }
        #endregion
        #region Private: int m_iEnumeratorPos - current position of the enumerator
        // The current position of the enumerator:
        //   - a value of -1 means the "reset" position
        //   - a value equal to Count is past the end.
        private int m_iEnumeratorPos = -1;
        #endregion
        #region Private: bool m_bEnumeratorValid - lets us know if [] has been modified
        private bool m_bEnumeratorValid = false;
        #endregion
        #region Private Method: InvalidateEnumerator() - signals that the enumerator is invalid now
        private void InvalidateEnumerator()
        {
            m_bEnumeratorValid = false;
        }
        #endregion
        #endregion
        #region Indexer[] - provides array access for subcontainers (get/set)
        virtual public EItem this[int index]
        {
            get
            {
                if (index < 0 || index >= m_vSubItems.Length)
                    return null;
                return m_vSubItems[index];
            }
            set
            {
                // Set the value
                m_vSubItems[index] = value;
                value.Owner = this;

                // Any active enumerator is now invalid
                InvalidateEnumerator();
            }
        }
        #endregion
        #region VirtMethod: void AddParagraph(EItem)
        public virtual void Append(EItem item)
        {
            InsertAt(SubItems.Length, item);
        }
        #endregion
        #region Method: void AddParagraph(EItem[] vAppend)
        public void Append(EItem[] vAppend)
        {
            InsertAt(SubItems.Length, vAppend);
        }
        #endregion
        #region Method: void InsertAt(iPos, EItem)
        public void InsertAt(int iPos, EItem item)
        {
            EItem[] v = new EItem[SubItems.Length + 1];

            for (int i = 0; i < iPos; i++)
                v[i] = SubItems[i];

            v[iPos] = item;

            for (int i = iPos; i < SubItems.Length; i++)
                v[i + 1] = SubItems[i];

            m_vSubItems = v;

            // The container needs to know to whom it belongs
            item.Owner = this;

            InvalidateEnumerator();
        }
        #endregion
        #region Method: void InsertAt(iPos, EItem[] vInsert)
        public void InsertAt(int iPos, EItem[] vInsert)
        {
            // Create a new vector that will hold the entirety
            EItem[] v = new EItem[SubItems.Length + vInsert.Length];

            // Copy over all of the items prior to position iPos
            int i = 0;
            for (; i < iPos; i++)
                v[i] = SubItems[i];

            // Copy in the new items we are inserting
            for (int k = 0; k < vInsert.Length; k++)
            {
                v[i + k] = vInsert[k];
                vInsert[k].Owner = this;
            }

            // Copy the remaining items from the original vector
            for (; i < SubItems.Length; i++)
                v[i + vInsert.Length] = SubItems[i];

            // Replace the original vector with our new one
            m_vSubItems = v;
        }
        #endregion
        #region Method: void RemoveAt(iPos)
        public void RemoveAt(int iPos)
        {
            EItem[] v = new EItem[SubItems.Length - 1];

            for (int i = 0; i < iPos; i++)
                v[i] = SubItems[i];

            for (int i = iPos; i < SubItems.Length - 1; i++)
                v[i] = SubItems[i + 1];

            m_vSubItems = v;

            InvalidateEnumerator();
        }
        #endregion
        #region Method: void RemoveAt(iPos, count)
        public void RemoveAt(int iPos, int count)
        {
            Debug.Assert(SubItems.Length >= count);

            // Create a new array to hold our answer
            EItem[] v = new EItem[SubItems.Length - count];

            // Copy over the blocks prior to the position iPos
            int i = 0;
            for (; i < iPos; i++)
                v[i] = SubItems[i];

            // Copy over the final blocks following cBlocksToremove
            for (; i < v.Length; i++)
                v[i] = SubItems[i + count];

            // Replace the original vector with our new one
            m_vSubItems = v;
        }
        #endregion
        #region Method: void Remove(EItem)
        public void Remove(EItem item)
        {
            int iPos = Find(item);
            if (-1 != iPos)
            {
                RemoveAt(iPos);
                InvalidateEnumerator();
            }
        }
        #endregion
        #region Method: int Find(EItem) - returns -1 if not found (doesn't search down the hierarchy)
        public int Find(EItem item)
        {
            for (int i = 0; i < SubItems.Length; i++)
            {
                if (SubItems[i] == item)
                    return i;
            }
            return -1;
        }
        #endregion
        #region Method: void Replace(itemOld, itemNew)
        public void Replace(EItem itemOld, EItem itemNew)
        {
            int iPos = Find(itemOld);
            if (iPos != -1)
                SubItems[iPos] = itemNew;
        }
        #endregion
        #region Attr{g}: int Count
        public int Count
        {
            get
            {
                return SubItems.Length;
            }
        }
        #endregion
        #region Method: EContainer FindContainerOfDataSource(JObject obj)
        public EContainer FindContainerOfDataSource(JObject obj)
        {
            OWPara para = this as OWPara;
            if (null != para && para.DataSource == obj)
                return this;

            foreach (EItem item in SubItems)
            {
                EContainer container = item as EContainer;
                if (null != container)
                {
                    EContainer answer = container.FindContainerOfDataSource(obj);
                    if (null != answer)
                        return answer;
                }
            }

            return null;
        }
        #endregion
        #region OMethod: EBlock GetBlockAt(PointF)
        public override EBlock GetBlockAt(PointF pt)
        {
            // If the point is not in this container, then return null
            if (!ContainsPoint(pt))
                return null;

            // Work through the subitems
            foreach (EItem item in SubItems)
            {
                EBlock block = item.GetBlockAt(pt);
                if (null != block)
                    return block;
            }

            return null;
        }
        #endregion
        #region VirtMethod: bool Contains(EItem item)
        public virtual bool Contains(EItem item)
        {
            if (item == this)
                return true;

            foreach (EItem i in SubItems)
            {
                if (i == item)
                    return true;

                EContainer container = i as EContainer;
                if (null != container && container.Contains(item))
                    return true;
            }

            return false;
        }
        #endregion
        #region OMethod: void SetOwnedControlsVisibility(bVisible)
        public override void SetOwnedControlsVisibility(bool bVisible)
            // Apply to all subitems
        {
            foreach (EItem item in SubItems)
            {
                item.SetOwnedControlsVisibility(bVisible);
            }
        }
        #endregion

        // Scaffolding -----------------------------------------------------------------------
        #region Constructor()
        public EContainer()
            : base()
        {
            m_vSubItems = new EItem[0];

            // Default to an empty border
            m_Border = new BorderBase(this);
        }
        #endregion
        #region OMethod: void Clear()
        public override void Clear()
            // Recursively works down, because we need to dispose of any EControls
        {
            foreach (EItem item in SubItems)
            {
                item.Clear();
            }

            m_vSubItems = new EItem[0];
        }
        #endregion

        // Selections ------------------------------------------------------------------------
        #region Method: int PopSelectionStack(ArrayList aiStack, int iDefault)
        protected int PopSelectionStack(ArrayList aiStack, bool bForward)
            // Determine where we'll start. If we have something in aiStack, then it means
            // we're still working down through the hierarchy to the correct position.
            // 
            // If the stack is empty, then in initializing the calling func's loop...
            //  - if we're moving forward, return 0
            //  - if we're moving backward, return SubItems.Length - 1
        {
            if (aiStack.Count > 0)
            {
                int i = (int)aiStack[0];
                aiStack.RemoveAt(0);
                return i;
            }

            return bForward ? 0 : SubItems.Length - 1;
        }
        #endregion
        #region VMethod: bool MoveLineDown(aiStack, ptCurrentLocation)
        virtual public bool MoveLineDown(ArrayList aiStack, PointF ptCurrentLocation)
        {
            // Loop through the subitems
            for (int i = PopSelectionStack(aiStack, true); i < Count; i++)
            {
                EItem item = SubItems[i] as EItem;

                // If uneditable, skip it
                if (!item.IsEditable)
                    continue;

                // If we're at a container, we keep working downwards
                EContainer container = item as EContainer;
                if (null != container)
                {
                    if (container.MoveLineDown(aiStack, ptCurrentLocation))
                        return true;
                }
            }

            return false;
        }
        #endregion
        #region VMethod: bool MoveLineUp(aiStack, ptCurrentLocation)
        virtual public bool MoveLineUp(ArrayList aiStack, PointF ptCurrentLocation)
        {
            // Loop through the subitems
            for (int i = PopSelectionStack(aiStack, false); i >= 0; i--)
            {
                EItem item = SubItems[i] as EItem;

                // If uneditable, skip it
                if (!item.IsEditable)
                    continue;

                // If we're at a container, we keep working downwards
                EContainer container = item as EContainer;
                if (null != container)
                {
                    if (container.MoveLineUp(aiStack, ptCurrentLocation))
                        return true;
                }
            }

            return false;
        }
        #endregion
        #region VMethod: bool Select_NextWord_Begin(aiStack)
        public virtual bool Select_NextWord_Begin(ArrayList aiStack)
        {
            // Loop through the appropriate subitems
            for (int i = PopSelectionStack(aiStack, true); i < Count; i++)
            {
                EItem item = SubItems[i] as EItem;

                // If uneditable, skip it
                if (!item.IsEditable)
                    continue;

                // If we've found the word, we're arrived at our destination
                EWord word = item as EWord;
                if (null != word)
                {
                    Window.Selection = new OWWindow.Sel(this as OWPara, this.Find(word), 0);
                    return true;
                }

                // If we're at a container, we keep working downwards
                EContainer container = item as EContainer;
                if (null != container)
                {
                    if (container.Select_NextWord_Begin(aiStack))
                        return true;
                }
            }

            // Unable to make a selection
            return false;
        }
        #endregion
        #region VMethod: bool Select_PrevWord(aiStack, bSelectAtEndOfWord)
        /// <summary>
        /// Places the selection into the previous word.
        /// </summary>
        /// <param name="aiStack">The EContainer subitem indices stack.</param>
        /// <param name="bSelectAtEndOfWord">If false, places the selection at the beginning of
        /// the word; if true, the selection goes at the end of the word.</param>
        /// <returns></returns>
        public virtual bool Select_PrevWord(ArrayList aiStack, bool bSelectAtEndOfWord)
        {
            // Loop through the appropriate subitems
            for (int i = PopSelectionStack(aiStack, false); i >= 0; i--)
            {
                EItem item = SubItems[i] as EItem;

                // If uneditable, skip it
                if (!item.IsEditable)
                    continue;

                // If we've found the word, we're arrived at our destination
                EWord word = item as EWord;
                if (null != word)
                {
                    int iChar = (bSelectAtEndOfWord) ? word.Text.Length : 0;
                    Window.Selection = new OWWindow.Sel(this as OWPara, this.Find(word), iChar);
                    return true;
                }

                // If we're at a container, we keep working downwards
                EContainer container = item as EContainer;
                if (null != container)
                {
                    if (container.Select_PrevWord(aiStack, bSelectAtEndOfWord))
                        return true;
                }
            }

            // Unable to make a selection
            return false;
        }
        #endregion
        #region VMethod: bool Select_FirstWord()
        public virtual bool Select_FirstWord()
        {
            if (!IsEditable)
                return false;

            for(int i=0; i < Count; i++)
            {
                EWord word = SubItems[i] as EWord;
                if (null != word)
                {
                    Window.Selection = new OWWindow.Sel(this as OWPara, i, 0);
                    return true;
                }

                EContainer container = SubItems[i] as EContainer;
                if (null != container)
                {
                    if (container.Select_FirstWord())
                        return true;
                }
            }

            return false;
        }
        #endregion
        #region VMethod: bool Select_LastWord_End()
        public virtual bool Select_LastWord_End()
        {
            if (!IsEditable)
                return false;

            for (int i = Count -1; i >= 0; i--)
            {
                EWord word = SubItems[i] as EWord;
                if (null != word)
                {
                    Window.Selection = new OWWindow.Sel(this as OWPara, i, word.Text.Length);
                    return true;
                }

                EContainer container = SubItems[i] as EContainer;
                if (null != container)
                {
                    if (container.Select_LastWord_End())
                        return true;
                }
            }

            return false;
        }
        #endregion
        #region Method: bool OnSelectAndScrollFrom(JObject)
        public bool OnSelectAndScrollFrom(JObject obj)
        {
            EWord wordToSelect = null;

            // We support either of the following
            TranslatorNote note = obj as TranslatorNote;
            DFootnote footnote = obj as DFootnote;
            Debug.Assert(null != note || null != footnote);

            foreach (EItem item in SubItems)
            {
                if (!item.IsEditable)
                    continue;

                // Recurse down through the containers hierarchy
                EContainer container = item as EContainer;
                if (null != container)
                {
                    if (container.OnSelectAndScrollFrom(obj))
                        return true;
                }

                // Do we have a selectable word? If so, we want to remember it, so that
                // it points to the word most adjacent to the note icon.
                EWord word = item as EWord;
                if (null != word)
                    wordToSelect = word;

                bool bSelectionFound = false;

                // Do we have a Translator Note?
                OWPara.ENote n = item as OWPara.ENote;
                if (n != null && note != null && n.Note == note)
                    bSelectionFound = true;

                // Do we have a Footnote
                OWPara.EFoot foot = item as OWPara.EFoot;
                if (null != footnote && (foot != null && foot.Footnote == footnote) )
                {
                    bSelectionFound = true;
                }

                // Do we have a viable place to select?
                if (bSelectionFound && null != wordToSelect)
                {
                    Window.Selection = new OWWindow.Sel(
                        wordToSelect.Para,
                        wordToSelect.PositionWithinPara,
                        wordToSelect.Text.Length);
                    Window.Focus();
                    return true;
                }
            }

            return false;
        }
        #endregion
        #region Method: OWPara FindParagraph(DFootnote)
        public OWPara FindParagraph(DFootnote footnote)
        {
            foreach (EItem item in SubItems)
            {
                // Recurse to lower levels of the hierarchy
                EContainer container = item as EContainer;
                if (null != container)
                {
                    OWPara para = container.FindParagraph(footnote);
                    if (null != para)
                        return para;
                }

                // Test this item
                OWPara.EFoot foot = item as OWPara.EFoot;
                if (foot != null && foot.Footnote == footnote) 
                {
                    return this as OWPara;
                }

            }

            return null;
        }
        #endregion
        #region Method: OWPara FindParagraph(JObject objDataSource, OWPara.Flags)
        public OWPara FindParagraph(JObject objDataSource, OWPara.Flags Flags)
        {
            // We want most of the flags to be the same; though we don't care if
            // CanItalics is different.
            Flags |= OWPara.Flags.CanItalic;

            foreach (EItem item in SubItems)
            {
                // Recurse to lower levels of the hierarchy
                EContainer container = item as EContainer;
                if (null != container)
                {
                    OWPara para = container.FindParagraph(objDataSource, Flags);
                    if (null != para)
                        return para;
                }

                // Test this item
                OWPara paragraph = item as OWPara;
                if (null == paragraph)
                    continue;
                if (paragraph.DataSource == objDataSource)
                {
                    if (Flags == (paragraph.Options | OWPara.Flags.CanItalic))
                        return paragraph;
                    else if (Flags == OWPara.Flags.CanItalic) // E.g., for where None was passed in
                        return paragraph;
                }
            }

            return null;
        }
        #endregion
        #region Attr{g}:  bool ContainsSelection
        public bool ContainsSelection
        {
            get
            {
                if (!Window.HasSelection)
                    return false;

                List<EContainer> vContainers = Window.Selection.ContainerStack;
                if (vContainers.Contains(this))
                    return true;

                return false;
            }
        }
        #endregion

        // Layout Calculations ---------------------------------------------------------------
        #region VirtMethod: float CalculateSubItemX(EItem subitem)
        public virtual float CalculateSubItemX(EItem subitem)
            // Return the X position for the subitem in question. In most cases
            // this will be our own X, but in, e.g., the RowOfColumns, it will
            // depend on which column the subitem represents.
        {
            return Position.X + Border.GetTotalWidth(BorderBase.BorderSides.Left);
        }
        #endregion
        #region Method: void CalculateBlockWidths(g) - calculate all EBlocks
        public virtual void CalculateBlockWidths(Graphics g)
        {
            foreach (EItem item in SubItems)
            {
                EContainer container = item as EContainer;
                if (null != container)
                    container.CalculateBlockWidths(g);

                EBlock block = item as EBlock;
                if (null != block)
                    block.CalculateWidth(g);
            }
        }
        #endregion
        #region VirtAttr{g}: float AvailableWidthForOneSubitem
        public virtual float AvailableWidthForOneSubitem
            // The default is that the width available for an item is the entire width of the
            // this container. In most cases this is true; but, e.g., a row with multiple 
            // columns will return smaller value, such that all columns are supported.
        {
            get
            {
                return Width - Border.GetTotalWidth(BorderBase.BorderSides.LeftAndRight);
            }
        }
        #endregion
        #region VirtMethod: void CalculateContainerHorizontals()
        virtual public void CalculateContainerHorizontals()
        {
            // Always calculate our width and Left first....
            // (Default is the Width and Left of the owner)
            if (null != Owner)
            {
                Width = Owner.AvailableWidthForOneSubitem;

                float xleft = Owner.CalculateSubItemX(this);
                Position = new PointF(xleft, Position.Y);
            }

            // Then calculate the width of the owned items (as they may need to
            // refer to this object via the AvailableWidthForOneSubitem call.)
            foreach (EItem item in SubItems)
            {
                EContainer container = item as EContainer;
                if (null != container)
                    container.CalculateContainerHorizontals();

                EControl control = item as EControl;
                if (null != control)
                    control.CalculateHorizontals();
            }
        }
        #endregion
        #region VirtMethod: void CalculateLineNumbers(ref nLineNo)
        virtual protected void CalculateLineNumbers(ref int nLineNo)
        {
            foreach (EItem item in SubItems)
            {
                EContainer container = item as EContainer;
                if (null != container)
                    container.CalculateLineNumbers(ref nLineNo);
            }
        }
        #endregion

        // Painting --------------------------------------------------------------------------
        #region OMethod: void OnPaint(ClipRectangle)
        override public void OnPaint(Rectangle ClipRectangle)
        {
            // For performance, make sure we need to paint this container
            if (!ClipRectangle.IntersectsWith(IntRectangle))
                return;

            // Borders as indicated
            Border.Paint();

            // Bitmap if indicated
            PaintBitmap();

            // Paint the subcontainers
            foreach (EItem item in SubItems)
                item.OnPaint(ClipRectangle);
        }
        #endregion
		#region OMethod: void PaintControls()
		public override void PaintControls()
		{
			foreach (EItem item in SubItems)
				item.PaintControls();
		}
		#endregion
	}
    #endregion

    #region CLASS: ERowOfColumns : EContainer
    public class ERowOfColumns : EContainer
    {
        // Attrs -----------------------------------------------------------------------------
        #region Attr{g}: int ColumnsCount
        public int ColumnsCount
        {
            get
            {
                Debug.Assert(m_cColumnsCount > 0);
                return m_cColumnsCount;
            }
        }
        int m_cColumnsCount;
        #endregion

        // Scaffolding -----------------------------------------------------------------------
        #region Constructor(EContainer Owner, cColumnsCount)
        public ERowOfColumns(int cColumnsCount)
            : base()
        {
            m_cColumnsCount = cColumnsCount;
        }
        #endregion

        // Layout Calculations ---------------------------------------------------------------
        #region OAttr{g}: float AvailableWidthForOneSubitem
        public override float AvailableWidthForOneSubitem
        {
            get
            {
                // Get the toal width available for content
                float fTotalContentWidth = Width - 
                    Border.GetTotalWidth(BorderBase.BorderSides.LeftAndRight);

                // Get the width available for a single subitem
                float fAvailableContentWidth = fTotalContentWidth - 
                    ((ColumnsCount - 1) * Window.WidthBetweenColumns);
                return fAvailableContentWidth / ColumnsCount;
            }
        }
        #endregion
        #region OMethod: void CalculateVerticals(y, bRepositionOnly)
        public override void CalculateVerticals(float y, bool bRepositionOnly)
        {
            // Remember the top-left position and width
            Position = new PointF(Position.X, y);

            // Top Border
            y += Border.GetTotalWidth(BorderBase.BorderSides.Top);

            // Allow for the top part of the bitmap in "y" when laying out the piles
            y += CalculateBitmapHeightRequirement();

            // Calculate for the tallest pile
            float fHighest = 0;
            foreach (EItem item in SubItems)
            {
				item.CalculateVerticals(y, bRepositionOnly);
				fHighest = Math.Max(fHighest, item.Height);
            }
            y += fHighest;

            // Bottom Border
            y += Border.GetTotalWidth(BorderBase.BorderSides.Bottom);

            // The Row's Height is the sum of PileHeight and BmpHeight
            Height = (y - Position.Y);
        }
        #endregion
        #region OMethod: float CalculateSubItemX(EItem subitem)
        public override float CalculateSubItemX(EItem subitem)
            // For the X, we need to figure out which column it is, and then multiply
            // by the width of a column plus the space in-between columns
        {
            // Locate which column we're in
            int iColumn = Find(subitem);
            Debug.Assert(-1 != iColumn);

            // The Leftmost x (the x of the first column) takes into account borders
            float xLeftMost = Position.X + Border.GetTotalWidth(BorderBase.BorderSides.Left);

            float x = xLeftMost + iColumn *
                (AvailableWidthForOneSubitem + Window.WidthBetweenColumns);

            return x;
        }
        #endregion
    }
    #endregion

    #region CLASS: EColumn : EContainer
    public class EColumn : EContainer
    {
        // Scaffolding -----------------------------------------------------------------------
        #region Constructor()
        public EColumn()
            : base()
        {
        }
        #endregion

        // Layout Calculations ---------------------------------------------------------------
        #region OMethod: void CalculateVerticals(y, bRepositionOnly)
        public override void CalculateVerticals(float y, bool bRepositionOnly)
        {
            // Remember the top-left position and width
            Position = new PointF(Position.X, y);

            // Top Border
            y += Border.GetTotalWidth(BorderBase.BorderSides.Top);

            // Allow for display of the bitmap if applicable
            y += CalculateBitmapHeightRequirement();

            // Layout the owned subitems, one below the other
            foreach (EItem item in SubItems)
            {
				item.CalculateVerticals(y, bRepositionOnly);
				y += item.Height;
            }

            // Bottom Border
            y += Border.GetTotalWidth(BorderBase.BorderSides.Bottom);

            // Calculate the resulting height
            Height = (y - Position.Y);
        }
        #endregion
    }
    #endregion

    #region CLASS: ERoot : EColumn
    public class ERoot : EColumn
    {
        // Attrs -------------------------------------------------------------------------
        #region OAttr{g}: ERoot Root
        public override ERoot Root
        {
            get
            {
                return this;
            }
        }
        #endregion
        #region Attr{g}: OWWindow Window - the owning window
        override public OWWindow Window
        {
            get
            {
                Debug.Assert(null != m_wndWindow);
                return m_wndWindow;
            }
        }
        OWWindow m_wndWindow = null;
        #endregion

        // Scaffolding -----------------------------------------------------------------------
        #region Constructor(OWWindow)
        public ERoot(OWWindow _Window)
            : base()
        {
            m_wndWindow = _Window;
        }
        #endregion

        // Selections ------------------------------------------------------------------------
        #region Method: bool Select_NextWord_Begin(sel)
        public bool Select_NextWord_Begin(OWWindow.Sel selection)
        {
            // Retrieve the selection's stack
            if (null == selection)
                return false;
            ArrayList aiStack = selection.ContainerIndicesStack;
            if (aiStack.Count == 0)
                return false;

            // We want to point to the next word (potentially), which means we add one to the
            // Last's iBlock.
            aiStack.Add(selection.Last.iBlock + 1);

            // Find and select the next word
            if (Select_NextWord_Begin(aiStack))
                return true;
            return false;
        }
        #endregion
        #region Method: bool Select_PrevWord_Begin(sel)
        public bool Select_PrevWord_Begin(OWWindow.Sel selection)
        {
            // Retrieve the selection's stack
            if (null == selection)
                return false;
            ArrayList aiStack = selection.ContainerIndicesStack;
            if (aiStack.Count == 0)
                return false;

            // We want to point to the previous word (potentially), which means we subtract one to the
            // First's iBlock.
            aiStack.Add(selection.Anchor.iBlock - 1);

            // Find and select the previous word
            if (Select_PrevWord(aiStack, false))
                return true;
            return false;
        }
        #endregion
        #region Method: bool Select_PrevWord_End(sel)
        public bool Select_PrevWord_End(OWWindow.Sel selection)
        {
            // Retrieve the selection's stack
            if (null == selection)
                return false;
            ArrayList aiStack = selection.ContainerIndicesStack;
            if (aiStack.Count == 0)
                return false;

            // We want to point to the previous word (potentially), which means we subtract one to the
            // First's iBlock.
            aiStack.Add(selection.Anchor.iBlock - 1);

            // Find and select the previous word
            if (Select_PrevWord(aiStack, true))
                return true;
            return false;
        }
        #endregion
        #region Method: bool Select_NextPara_Begin(sel)
        public bool Select_NextPara_Begin(OWWindow.Sel selection)
        {
            // Retrieve the selection's stack
            if (null == selection)
                return false;
            ArrayList aiStack = selection.ContainerIndicesStack;
            if (aiStack.Count == 0)
                return false;

            // Locate the level of the owning paragraph
            int iParaLevel = aiStack.Count - 1;
            EContainer container = selection.Anchor.Word.Owner;
            while (container as OWPara == null && iParaLevel > 0)
            {
                container = container.Owner;
                iParaLevel--;
            }
            if (iParaLevel == 0)
                return false;

            // Remove everthing in the stack beyond iParaLevel
            while (aiStack.Count > iParaLevel + 1)
                aiStack.RemoveAt(iParaLevel + 1);

            // Increment our starting point so we'll move to the next selectable entity (which
            // we are assuming is owned in a paragraph.)
            int iPara = (int)aiStack[iParaLevel];
            aiStack[iParaLevel] = iPara + 1;

            // Find and select the next word
            if (Select_NextWord_Begin(aiStack))
                return true;
            return false;
        }
        #endregion

        // Layout Calculations ---------------------------------------------------------------
        #region OMethod: void CalculateContainerHorizontals()
        override public void CalculateContainerHorizontals()
        {
            // The width of this root container is the width of the window, less the
            // left and right margins.
            Width = Window.Width - Window.m_ScrollBar.Width - Window.WindowMargins.Width * 2;

            // The Left is the margin width
            Position = new PointF(Window.WindowMargins.Width, Position.Y);

            // Process the sub-items as per usual
            foreach (EItem item in SubItems)
            {
                EContainer container = item as EContainer;
                if (null != container)
                    container.CalculateContainerHorizontals();

				EControl control = item as EControl;
				if (null != control)
					control.CalculateHorizontals();
            }
        }
        #endregion
		#region OMethod:  void CalculateVerticals(float y, bool bRepositionOnly)
		public override void CalculateVerticals(float y, bool bRepositionOnly)
		{
			// Lay out the items
			foreach (EItem item in SubItems)
			{
				item.CalculateVerticals(y, false);
				y += item.Height;
			}

			Height = y - Position.Y;
		}
		#endregion
		#region Method: void CalculateLineNumbers()
		public void CalculateLineNumbers()
        {
            int nLineNo = 1;
            CalculateLineNumbers(ref nLineNo);
        }
        #endregion
    }
    #endregion

}
