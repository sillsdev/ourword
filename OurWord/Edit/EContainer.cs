#region ***** EContainer.cs *****
/**********************************************************************************************
 * Project: OurWord!
 * File:    EContainer.cs
 * Author:  John Wimbish
 * Created: 14 Oct 2008
 * Purpose: The various classes holding editor layout
 * Legal:   Copyright (c) 2004-09, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using OurWord.Edit.Blocks;
using OurWordData;
using OurWordData.DataModel;
using OurWordData.DataModel.Annotations;
using OurWordData.DataModel.Runs;

#endregion

// TODO: Implement Left and Right Borders

namespace OurWord.Edit
{
    #region CLASS: EItem
    public class EItem
    {
        // Ownership Hierarchy ---------------------------------------------------------------
        #region Attr{g/s}: EContainer Owner
        public EContainer Owner { get; set; }
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
                    var container = this as EContainer;
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
        public PointF Position { get; set; }
        #endregion
        #region VirtAttr{g/s}: float Height - pixel height of this item
        public virtual float Height { get; protected set; }
        #endregion
        #region VirtAttr{g/s}: float Width - pixel width of this item
        public virtual float Width { get; protected set; }
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
        protected Rectangle IntRectangle
        {
            get
            {
                var x = (int)Position.X;
                var y = (int)Position.Y;
                var w = (int)Width;
                var h = (int)Height;
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
            throw new Exception("Subclasses need to implement this.");
        }
        #endregion
        #region attr{g}: Point Middle
        protected Point Middle
        {
            get
            {
                return new Point((int)(Position.X + Width/2), (int)(Position.Y + Height/2));
            }
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
            // ReSharper disable ValueParameterNotUsed
            set
            {
            }
            // ReSharper restore ValueParameterNotUsed
        }
        #endregion

        // Scaffolding -----------------------------------------------------------------------
        #region Constructor()
        protected EItem()
        {
        }
        #endregion
        #region Atrr{g}: IDrawingContext Context
        virtual public IDrawingContext Context
        {
            get
            {
                return Root.Context;
            }
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
		#region VirtMethod: void CalculateVerticals(y)
		virtual public void CalculateVerticals(float y)
		// Layout any children, and calculate the Height. 
		//
		// Parameters
		//   yTop - the top pixel for the container
		{
			// This will be different for every subclass
			Debug.Assert(false);
		}
		#endregion

        // Painting --------------------------------------------------------------------------
        #region VirtMethod: void OnPaint(ClipRectangle)
        virtual public void OnPaint(IDraw draw, Rectangle clipRectangle)
        {
        }
        #endregion
		#region VirtMethod: void PaintControls()
		virtual public void PaintControls()
		{
		}
		#endregion
        #region Method: virtual void Draw(IDraw)
        virtual public void Draw(IDraw draw)
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
                public int Top { get; set; }
                #endregion
                #region Attr{g/s}: int Bottom
                public int Bottom { get; set; }
                #endregion
                #region Attr{g/s}: int Left
                public int Left { get; set; }
                #endregion
                #region Attr{g/s}: int Right
                public int Right { get; set; }
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
                    var r = new RectangleF(LeftBorder, TopBorder,
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
            [Flags]
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
            #region VMethod: void Paint(IDraw)
            virtual public void Paint(IDraw draw)
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

            #region OMethod: void Paint(IDraw)
            public override void Paint(IDraw draw)
            {
                var pen = BorderPen;

                if (Color.Empty != FillColor)
                    draw.FillRectangle(FillColor, BorderRectangle);

                var fLeftTop = new PointF(LeftBorder, TopBorder);
                var fLeftBottom = new PointF(LeftBorder, BottomBorder);
                var fRightTop = new PointF(RightBorder, TopBorder);
                var fRightBottom = new PointF(RightBorder, BottomBorder);

                if ((BorderPlacement & BorderSides.Top) == BorderSides.Top)
                    draw.DrawLine(pen, fLeftTop, fRightTop);
                if ((BorderPlacement & BorderSides.Bottom) == BorderSides.Bottom)
                    draw.DrawLine(pen, fLeftBottom, fRightBottom);

                if ((BorderPlacement & BorderSides.Left) == BorderSides.Left)
                    draw.DrawLine(pen, fLeftTop, fLeftBottom);
                if ((BorderPlacement & BorderSides.Right) == BorderSides.Right)
                    draw.DrawLine(pen, fRightTop, fRightBottom);
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

            #region OMethod: void Paint(IDraw)
            public override void Paint(IDraw draw)
            {
                draw.DrawRoundedRectangle(BorderPen, FillBrush,
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
            float m_SeparatorWidth;
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

            #region OMethod: void Paint(IDraw)
            public override void Paint(IDraw draw)
            {
                if (0 == SeparatorWidth)
                    return;

                draw.DrawLine(BorderPen,
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
        #region Attr{g}: EPicture Picture
        public EPicture Picture
        {
            get
            {
                return m_Picture;
            }
        }
        private EPicture m_Picture;
        #endregion
        #region Method: void SetPicture(Bitmap bmp)
        public void SetPicture(Bitmap bmp, bool bUseTopAndBottomBorders)
        {
            if (null == bmp)
            {
                m_Picture = null;
                return;
            }

            m_Picture = new EPicture(bmp, this);

            // We add a bottom margin (space outside the border) to account for the
            // Windows rounding error when drawing, to make sure the bottom line gets
            // drawn (and, to allow a bit of extra space before the next row.)
            if (bUseTopAndBottomBorders)
            {
                Border = new SquareBorder(this)
                    {
                        BorderPlacement = BorderBase.BorderSides.TopAndBottom,
                        Margin = new BorderBase.RectOffset {Bottom = 5}
                    };
            }
        }
        #endregion
        #region Attr{g}: bool HasBitmap
        bool HasBitmap
        {
            get
            {
                return (null != m_Picture);
            }
        }
        #endregion
        #region Method: void PaintBitmap(IDraw)
        protected void PaintBitmap(IDraw draw)
        {
            if (!HasBitmap)
                return;
            m_Picture.Draw(draw);
        }
        #endregion
        #region Method: float CalculateBitmapHeightRequirement()
        protected float CalculateBitmapHeightRequirement()
        {
            return !HasBitmap ? 0 : m_Picture.Height;
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
        public IEnumerator GetEnumerator()
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
        private bool m_bEnumeratorValid;
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
        #region VirtMethod: void Append(EItem)
        public virtual void Append(EItem item)
        {
            InsertAt(SubItems.Length, item);
        }
        #endregion
        #region Method: void Append(EItem[] vAppend)
        public void Append(EItem[] vAppend)
        {
            InsertAt(SubItems.Length, vAppend);
        }
        #endregion
        #region Method: void InsertAt(iPos, EItem)
        public void InsertAt(int iPos, EItem item)
        {
            var v = new EItem[SubItems.Length + 1];

            for (var i = 0; i < iPos; i++)
                v[i] = SubItems[i];

            v[iPos] = item;

            for (var i = iPos; i < SubItems.Length; i++)
                v[i + 1] = SubItems[i];

            m_vSubItems = v;

            // The container needs to know to whom it belongs
            item.Owner = this;

            InvalidateEnumerator();
        }
        #endregion
        #region method: void InsertAt(iPos, EItem[] vInsert)
        protected void InsertAt(int iPos, EItem[] vInsert)
        {
            // Create a new vector that will hold the entirety
            var v = new EItem[SubItems.Length + vInsert.Length];

            // Copy over all of the items prior to position iPos
            var i = 0;
            for (; i < iPos; i++)
                v[i] = SubItems[i];

            // Copy in the new items we are inserting
            for (var k = 0; k < vInsert.Length; k++)
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
        #region method: void RemoveAt(iPos)
        protected void RemoveAt(int iPos)
        {
            var v = new EItem[SubItems.Length - 1];

            for (var i = 0; i < iPos; i++)
                v[i] = SubItems[i];

            for (var i = iPos; i < SubItems.Length - 1; i++)
                v[i] = SubItems[i + 1];

            m_vSubItems = v;

            InvalidateEnumerator();
        }
        #endregion
        #region method: void RemoveAt(iPos, count)
        protected void RemoveAt(int iPos, int count)
        {
            Debug.Assert(SubItems.Length >= count);

            // Create a new array to hold our answer
            var v = new EItem[SubItems.Length - count];

            // Copy over the blocks prior to the position iPos
            var i = 0;
            for (; i < iPos; i++)
                v[i] = SubItems[i];

            // Copy over the final blocks following cBlocksToremove
            for (; i < v.Length; i++)
                v[i] = SubItems[i + count];

            // Replace the original vector with our new one
            m_vSubItems = v;
        }
        #endregion
        #region method: void Remove(EItem)
        protected void Remove(EItem item)
        {
            var iPos = Find(item);
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
            var iPos = Find(itemOld);
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
            var para = this as OWPara;
            if (null != para && para.DataSource == obj)
                return this;

            foreach (var item in SubItems)
            {
                var container = item as EContainer;
                if (null != container)
                {
                    var answer = container.FindContainerOfDataSource(obj);
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

            foreach (var i in SubItems)
            {
                if (i == item)
                    return true;

                var container = i as EContainer;
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
        #region Attr{g}: IEnumerable<OWPara> AllParagraphs
        public IEnumerable<OWPara> AllParagraphs
        {
            get
            {
                var v = new List<OWPara>();

                foreach (var item in SubItems)
                {
                    if (item as OWPara != null)
                        v.Add(item as OWPara);
                    if (item as EContainer != null)
                        v.AddRange( (item as EContainer).AllParagraphs );
                }

                return v;
            }
        }
        #endregion
        #region VirtAttr{g}: string ColumnId
        public virtual string ColumnId
        {
            get
            {
                return (null != Owner) ? Owner.ColumnId : "";
            }
        }
        #endregion

        // Scaffolding -----------------------------------------------------------------------
        #region constructor()
        protected EContainer()
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
                var i = (int)aiStack[0];
                aiStack.RemoveAt(0);
                return i;
            }

            return bForward ? 0 : SubItems.Length - 1;
        }
        #endregion
        #region VMethod: bool MoveLineDown(aiStack, ptCurrentLocation)
        virtual public bool MoveLineDown(ArrayList aiStack, string sColumnId, PointF ptCurrentLocation)
        {
            // Loop through the subitems
            for (var i = PopSelectionStack(aiStack, true); i < Count; i++)
            {
                var item = SubItems[i];

                // If uneditable, skip it
                if (!item.IsEditable)
                    continue;

                // If we're at a container, we keep working downwards
                var container = item as EContainer;
                if (null != container)
                {
                    if (container.MoveLineDown(aiStack, sColumnId, ptCurrentLocation))
                        return true;
                }
            }

            return false;
        }
        #endregion
        #region VMethod: bool MoveLineUp(aiStack, ptCurrentLocation)
        virtual public bool MoveLineUp(ArrayList aiStack, string sColumnId, PointF ptCurrentLocation)
        {
            // Loop through the subitems
            for (var i = PopSelectionStack(aiStack, false); i >= 0; i--)
            {
                var item = SubItems[i];

                // If uneditable, skip it
                if (!item.IsEditable)
                    continue;

                // If we're at a container, we keep working downwards
                var container = item as EContainer;
                if (null != container)
                {
                    if (container.MoveLineUp(aiStack, sColumnId, ptCurrentLocation))
                        return true;
                }
            }

            return false;
        }
        #endregion
        #region vmethod: bool Select_NextWord_Begin(aiStack)
        protected virtual bool Select_NextWord_Begin(ArrayList aiStack)
        {
            // Loop through the appropriate subitems
            for (var i = PopSelectionStack(aiStack, true); i < Count; i++)
            {
                var item = SubItems[i];

                // If uneditable, skip it
                if (!item.IsEditable)
                    continue;

                // If we've found the word, we're arrived at our destination
                var word = item as EWord;
                if (null != word)
                {
                    Window.Selection = new OWWindow.Sel(this as OWPara, Find(word), 0);
                    return true;
                }

                // If we're at a container, we keep working downwards
                var container = item as EContainer;
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
        #region vmethod: bool Select_PrevWord(aiStack, bSelectAtEndOfWord)
        /// <summary>
        /// Places the selection into the previous word.
        /// </summary>
        /// <param name="aiStack">The EContainer subitem indices stack.</param>
        /// <param name="bSelectAtEndOfWord">If false, places the selection at the beginning of
        /// the word; if true, the selection goes at the end of the word.</param>
        /// <returns></returns>
        protected virtual bool Select_PrevWord(ArrayList aiStack, bool bSelectAtEndOfWord)
        {
            // Loop through the appropriate subitems
            for (var i = PopSelectionStack(aiStack, false); i >= 0; i--)
            {
                var item = SubItems[i];

                // If uneditable, skip it
                if (!item.IsEditable)
                    continue;

                // If we've found the word, we're arrived at our destination
                var word = item as EWord;
                if (null != word)
                {
                    var iChar = (bSelectAtEndOfWord) ? word.Text.Length : 0;
                    Window.Selection = new OWWindow.Sel(this as OWPara, Find(word), iChar);
                    return true;
                }

                // If we're at a container, we keep working downwards
                var container = item as EContainer;
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

            for(var i=0; i < Count; i++)
            {
                var word = SubItems[i] as EWord;
                if (null != word)
                {
                    Window.Selection = new OWWindow.Sel(this as OWPara, i, 0);
                    return true;
                }

                var container = SubItems[i] as EContainer;
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
                var word = SubItems[i] as EWord;
                if (null != word)
                {
                    Window.Selection = new OWWindow.Sel(this as OWPara, i, word.Text.Length);
                    return true;
                }

                var container = SubItems[i] as EContainer;
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
            var note = obj as TranslatorNote;
            var footnote = obj as DFootnote;
            Debug.Assert(null != note || null != footnote);

            foreach (var item in SubItems)
            {
                if (!item.IsEditable)
                    continue;

                // Recurse down through the containers hierarchy
                var container = item as EContainer;
                if (null != container)
                {
                    if (container.OnSelectAndScrollFrom(obj))
                        return true;
                }

                // Do we have a selectable word? If so, we want to remember it, so that
                // it points to the word most adjacent to the annotation icon.
                var word = item as EWord;
                if (null != word)
                    wordToSelect = word;

                var bSelectionFound = false;

                // Do we have a TranslatorNote?
                var n = item as ENote;
                if (n != null && note != null && n.Note == note)
                    bSelectionFound = true;

                // Do we have a Footnote
                var foot = item as EFoot;
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
            foreach (var item in SubItems)
            {
                // Recurse to lower levels of the hierarchy
                var container = item as EContainer;
                if (null != container)
                {
                    var para = container.FindParagraph(footnote);
                    if (null != para)
                        return para;
                }

                // Test this item
                var foot = item as EFoot;
                if (foot != null && foot.Footnote == footnote) 
                {
                    return this as OWPara;
                }
            }

            return null;
        }
        #endregion
        #region Method: OWPara FindParagraph(JObject objDataSource, OWPara.Flags)
        public OWPara FindParagraph(JObject objDataSource, OWPara.Flags flags)
        {
            // We want most of the flags to be the same; though we don't care if
            // CanItalics is different.
            flags |= OWPara.Flags.CanItalic;

            foreach (var item in SubItems)
            {
                // Recurse to lower levels of the hierarchy
                var container = item as EContainer;
                if (null != container)
                {
                    var para = container.FindParagraph(objDataSource, flags);
                    if (null != para)
                        return para;
                }

                // Test this item
                var paragraph = item as OWPara;
                if (null == paragraph)
                    continue;
                if (paragraph.DataSource == objDataSource)
                {
                    if (flags == (paragraph.Options | OWPara.Flags.CanItalic))
                        return paragraph;
                    if (flags == OWPara.Flags.CanItalic) // E.g., for where None was passed in
                        return paragraph;
                }
            }

            return null;
        }
        #endregion
        #region Method: OWPara FindParagraph(JObject objDataSource, bBackTranslation)
        public OWPara FindParagraph(JObject objDataSource, bool bBackTranslation)
        {
            foreach (var item in SubItems)
            {
                // Recurse to lower levels of the hierarchy
                var container = item as EContainer;
                if (null != container)
                {
                    var para = container.FindParagraph(objDataSource, bBackTranslation);
                    if (null != para)
                        return para;
                }

                // Test this item
                var owp = item as OWPara;
                if (null == owp)
                    continue;
                if (owp.DataSource == objDataSource)
                {
                    if (bBackTranslation == owp.DisplayBT)
                        return owp;
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

        // Find ------------------------------------------------------------------------------
        #region vmethod: Sel FindFirst(sSearchFor)
        virtual public OWWindow.Sel FindFirst(string sSearchFor)
            // At this level, we don't expect to encounter words; but if we do, we don't
            // include them in the search.
        {
            foreach(var item in this)
            {
                var container = item as EContainer;
                if (null != container)
                {
                    var selection = container.FindFirst(sSearchFor);
                    if (null != selection)
                        return selection;
                }
            }

            return null;
        }
        #endregion
        #region method: Sel FindNext(aiStack, Sel current, sSearchFor)
        protected OWWindow.Sel FindNext(ArrayList aiStack, OWWindow.Sel current, string sSearchFor)
        {
            for (var i = PopSelectionStack(aiStack, true); i < Count; i++)
            {
                var item = SubItems[i];

                var paragraph = item as OWPara;
                if (null != paragraph && paragraph != current.Paragraph)
                {
                    var selection = paragraph.FindFirst(sSearchFor);
                    if (null != selection)
                        return selection;
                }

                var container = item as EContainer;
                if (null != container)
                {
                    var selection = container.FindNext(aiStack, current, sSearchFor);
                    if (null != selection)
                        return selection;
                }            
            }
            return null;
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
        #region Method: void CalculateBlockWidths() - calculate all EBlocks
        public virtual void CalculateBlockWidths()
        {
            foreach (var item in SubItems)
            {
                var container = item as EContainer;
                if (null != container)
                    container.CalculateBlockWidths();

                var block = item as EBlock;
                if (null != block)
                    block.CalculateWidth();
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

                var xleft = Owner.CalculateSubItemX(this);
                Position = new PointF(xleft, Position.Y);
            }

            // Then calculate the width of the owned items (as they may need to
            // refer to this object via the AvailableWidthForOneSubitem call.)
            foreach (var item in SubItems)
            {
                var container = item as EContainer;
                if (null != container)
                    container.CalculateContainerHorizontals();

                var control = item as EControl;
                if (null != control)
                    control.CalculateHorizontals();
            }
        }
        #endregion
        #region VirtMethod: void CalculateLineNumbers(ref nLineNo)
        virtual protected void CalculateLineNumbers(ref int nLineNo)
        {
            foreach (var item in SubItems)
            {
                var container = item as EContainer;
                if (null != container)
                    container.CalculateLineNumbers(ref nLineNo);
            }
        }
        #endregion

        // Painting --------------------------------------------------------------------------
        #region OMethod: void OnPaint(IDraw, ClipRectangle)
        override public void OnPaint(IDraw draw, Rectangle clipRectangle)
        {
            // For performance, make sure we need to paint this container
            if (!clipRectangle.IntersectsWith(IntRectangle))
                return;

            // Borders as indicated
            Border.Paint(draw);

            // Bitmap if indicated
            PaintBitmap(draw);

            // Paint the subcontainers
            foreach (var item in SubItems)
                item.OnPaint(draw, clipRectangle);
        }
        #endregion
		#region OMethod: void PaintControls()
		public override void PaintControls()
		{
			foreach (var item in SubItems)
				item.PaintControls();
		}
		#endregion
        #region OMethod: void Draw(IDraw draw)
        override public void Draw(IDraw draw)
        {
            Border.Paint(draw);

            PaintBitmap(draw);

            foreach (var item in SubItems)
                item.Draw(draw);
        }
        #endregion
    }
    #endregion

    #region CLASS: ERowOfColumns : EContainer
    public class ERowOfColumns : EContainer
    {
        // Attrs -----------------------------------------------------------------------------
        #region attr{g}: int ColumnsCount
        private int ColumnsCount
        {
            get
            {
                Debug.Assert(m_cColumnsCount > 0);
                return m_cColumnsCount;
            }
        }
        readonly int m_cColumnsCount;
        #endregion

        // Scaffolding -----------------------------------------------------------------------
        #region Constructor(cColumnsCount)
        public ERowOfColumns(int cColumnsCount)
        {
            m_cColumnsCount = cColumnsCount;
        }
        #endregion

        // Misc Methods ----------------------------------------------------------------------
        #region Method: EColumn GetColumn(iColumn)
        public EColumn GetColumn(int iColumn)
            // Creates the column if it is not there yet; otherwise returns an EColumn.
            // If there is some other time of object there instead, an assertion is fired.
        {
            Debug.Assert(iColumn < ColumnsCount);

            while (SubItems.Length <= iColumn)
                Append(new EColumn());

            var column = SubItems[iColumn] as EColumn;

            Debug.Assert(null != column);
            return column;
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
                    ((ColumnsCount - 1) * Context.WidthBetweenColumns);
                return fAvailableContentWidth / ColumnsCount;
            }
        }
        #endregion
        #region OMethod: void CalculateVerticals(y)
        public override void CalculateVerticals(float y)
        {
            // Remember the top-left position and width
            Position = new PointF(Position.X, y);

            // Top Border
            y += Border.GetTotalWidth(BorderBase.BorderSides.Top);

            // Allow for the top part of the bitmap in "y" when laying out the piles
            y += CalculateBitmapHeightRequirement();

            // Calculate for the tallest pile
            float fHighest = 0;
            foreach (var item in SubItems)
            {
                item.CalculateVerticals(y);
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
                (AvailableWidthForOneSubitem + Context.WidthBetweenColumns);

            return x;
        }
        #endregion
    }
    #endregion

    #region CLASS: EColumn : EContainer
    public class EColumn : EContainer
    {
        // Scaffolding -----------------------------------------------------------------------
        #region Attr{g}: int PositionWithinOwningRow
        int PositionWithinOwningRow
        {
            get
            {
                if (null == Owner)
                    return -1;

                var row = Owner as ERowOfColumns;
                if (null == row)
                    return -1;

                for(var i=0; i<row.SubItems.Length; i++)
                {
                    if (row.SubItems[i] == this)
                        return i;
                }

                return -1;
            }
        }
        #endregion
        #region OAttr{g}: string ColumnId
        public override string ColumnId
        {
            get
            {
                var i = PositionWithinOwningRow;
                if (i == -1)
                    return base.ColumnId;

                if (null == Owner)
                    return "";

                return Owner.ColumnId + i;
            }
        }
        #endregion

        // Layout Calculations ---------------------------------------------------------------
        #region OMethod: void CalculateVerticals(y)
        public override void CalculateVerticals(float y)
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
				item.CalculateVerticals(y);
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
        readonly OWWindow m_wndWindow;
        #endregion
        #region Atrr{g}: IDrawingContext Context
        override public IDrawingContext Context
        {
            get
            {
                Debug.Assert(null != m_Context);
                return m_Context;
            }
        }
        private readonly IDrawingContext m_Context;
        #endregion

        // Scaffolding -----------------------------------------------------------------------
        #region Constructor(IDrawingContext, OWWindow)
        public ERoot(OWWindow window, IDrawingContext context)
        {
            m_wndWindow = window;
            m_Context = context;
        }
        #endregion

        // Selections ------------------------------------------------------------------------
        #region Method: bool Select_NextWord_Begin(sel)
        public bool Select_NextWord_Begin(OWWindow.Sel selection)
        {
            // Retrieve the selection's stack
            if (null == selection)
                return false;
            var aiStack = selection.ContainerIndicesStack;
            if (aiStack.Count == 0)
                return false;

            // We want to point to the next word (potentially), which means we add one to the
            // Last's iBlock.
            aiStack.Add(selection.Last.iBlock + 1);

            // Find and select the next word
            return Select_NextWord_Begin(aiStack);
        }
        #endregion
        #region Method: bool Select_PrevWord_Begin(sel)
        public bool Select_PrevWord_Begin(OWWindow.Sel selection)
        {
            // Retrieve the selection's stack
            if (null == selection)
                return false;
            var aiStack = selection.ContainerIndicesStack;
            if (aiStack.Count == 0)
                return false;

            // We want to point to the previous word (potentially), which means we subtract one to the
            // First's iBlock.
            aiStack.Add(selection.Anchor.iBlock - 1);

            // Find and select the previous word
            return Select_PrevWord(aiStack, false);
        }
        #endregion
        #region Method: bool Select_PrevWord_End(sel)
        public bool Select_PrevWord_End(OWWindow.Sel selection)
        {
            // Retrieve the selection's stack
            if (null == selection)
                return false;
            var aiStack = selection.ContainerIndicesStack;
            if (aiStack.Count == 0)
                return false;

            // We want to point to the previous word (potentially), which means we subtract one to the
            // First's iBlock.
            aiStack.Add(selection.Anchor.iBlock - 1);

            // Find and select the previous word
            return Select_PrevWord(aiStack, true);
        }
        #endregion
        #region Method: bool Select_NextPara_Begin(sel)
        public bool Select_NextPara_Begin(OWWindow.Sel selection)
        {
            // Retrieve the selection's stack
            if (null == selection)
                return false;
            var aiStack = selection.ContainerIndicesStack;
            if (aiStack.Count == 0)
                return false;

            // Locate the level of the owning paragraph
            var iParaLevel = aiStack.Count - 1;
            var container = selection.Anchor.Word.Owner;
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
            var iPara = (int)aiStack[iParaLevel];
            aiStack[iParaLevel] = iPara + 1;

            // Find and select the next word
            return Select_NextWord_Begin(aiStack);
        }
        #endregion
        #region Method: Sel FindNext(Sel current, sSearchFor)
        public OWWindow.Sel FindNext(OWWindow.Sel current, string sSearchFor)
        {
            if (null == current)
                return FindFirst(sSearchFor);

            // A selection is at a paragraph level; see if there is another in this paragraph
            var selection = current.Paragraph.FindNext(current, sSearchFor);
            if (null != selection)
                return selection;

            // If we're here, we must go through the container hierarchy 
            var aiStack = current.ContainerIndicesStack;
            if (aiStack.Count == 0)
                return null;

            return FindNext(aiStack, current, sSearchFor);
        }
        #endregion
        #region Sel MakeSelection(DText, iOffsetIntoParagraph, iLength)
        public OWWindow.Sel MakeSelection(DBasicText text, int iOffsetIntoParagraph, int iLength)
        {
            // Locate the containing OWPara
            var owp = FindParagraph(text.Paragraph, OWPara.Flags.None);
            return (null == owp) ? 
                null :
                owp.MakeSelection(iOffsetIntoParagraph, iLength);
        }
        #endregion

        // Layout Calculations ---------------------------------------------------------------
        #region Method: void DoLayout()
        public void DoLayout()
        {
            CalculateContainerHorizontals();
            CalculateVerticals(Context.TopMargin);
        }
        #endregion
        #region OMethod: void CalculateContainerHorizontals()
        override public void CalculateContainerHorizontals()
        {
            Width = Context.AvailableWidthForContent;
            Position = new PointF(Context.LeftMargin, Position.Y);

            // Process the sub-items as per usual
            foreach (var item in SubItems)
            {
                var container = item as EContainer;
                if (null != container)
                    container.CalculateContainerHorizontals();

				var control = item as EControl;
				if (null != control)
					control.CalculateHorizontals();
            }
        }
        #endregion
		#region OMethod: void CalculateVerticals(float y)
		public override void CalculateVerticals(float y)
		{
			// Lay out the items
			foreach (var item in SubItems)
			{
				item.CalculateVerticals(y);
				y += item.Height;
			}

			Height = y - Position.Y;
		}
		#endregion
		#region Method: void CalculateLineNumbers()
		public void CalculateLineNumbers()
        {
            var nLineNo = 1;
            CalculateLineNumbers(ref nLineNo);
        }
        #endregion
    }
    #endregion

}
