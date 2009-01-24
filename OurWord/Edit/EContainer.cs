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
using OurWord.DataModel;
#endregion

// TODO: Get rid of StartNewRow, so that we deal with the Piles as we build the container

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

        // Painting --------------------------------------------------------------------------
        #region VirtMethod: void OnPaint(ClipRectangle)
        virtual public void OnPaint(Rectangle ClipRectangle)
        {
        }
        #endregion
    }
    #endregion

    #region CLASS: EContainer
    public class EContainer : EItem, IEnumerator
    {
        // Optional Footnote Separator at top of container -----------------------------------
        #region Attr{g}: bool DisplayFootnoteSeparator - if T, shows line btwn para's and footnotes
        virtual public bool DisplayFootnoteSeparator
        {
            get
            {
                return m_bDisplayFootnoteSeparator;
            }
            set
            {
                m_bDisplayFootnoteSeparator = value;
            }
        }
        bool m_bDisplayFootnoteSeparator = false;
        #endregion
        #region Method: void PaintFootnoteSeparator()
        protected void PaintFootnoteSeparator()
            // Display the footnote separator if requested
        {
            if (!DisplayFootnoteSeparator)
                return;

            float xSeparatorWidth = Rectangle.Width / 3.0F;
            Pen pen = new Pen(Color.Black);
            Window.Draw.Line(pen, Position,
                new PointF(Position.X + xSeparatorWidth, Position.Y));
        }
        #endregion
        #region VAttr{g}: int FootnoteSeparatorHeight
        protected int FootnoteSeparatorHeight
        {
            get
            {
                return (DisplayFootnoteSeparator) ? 1 : 0;
            }
        }
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

            for (int i = iPos; i < SubItems.Length; i++)
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
        #region Method: void Clear()
        public void Clear()
        {
            m_vSubItems = new EItem[0];
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

        // Scaffolding -----------------------------------------------------------------------
        #region Constructor()
        public EContainer()
            : base()
        {
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
        #region Method: bool Select_NextWord_Begin(aiStack)
        public bool Select_NextWord_Begin(ArrayList aiStack)
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
        #region Method: bool Select_PrevWord(aiStack, bSelectAtEndOfWord)
        /// <summary>
        /// Places the selection into the previous word.
        /// </summary>
        /// <param name="aiStack">The EContainer subitem indices stack.</param>
        /// <param name="bSelectAtEndOfWord">If false, places the selection at the beginning of
        /// the word; if true, the selection goes at the end of the word.</param>
        /// <returns></returns>
        public bool Select_PrevWord(ArrayList aiStack, bool bSelectAtEndOfWord)
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
        #region Method: bool Select_FirstWord()
        public bool Select_FirstWord()
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
        #region Method: bool Select_LastWord_End()
        public bool Select_LastWord_End()
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
                OWPara.ESeeAlso also = item as OWPara.ESeeAlso;
                OWPara.EFootLetter letter = item as OWPara.EFootLetter;
                if (null != footnote && (
                    (also != null && also.Footnote == footnote) ||
                    (letter != null && letter.Footnote == footnote)))
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
                OWPara.ESeeAlso also = item as OWPara.ESeeAlso;
                OWPara.EFootLetter letter = item as OWPara.EFootLetter;
                if ((also != null && also.Footnote == footnote) ||
                     (letter != null && letter.Footnote == footnote))
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

        // Layout Calculations ---------------------------------------------------------------
        #region VirtMethod: float CalculateSubItemX(EItem subitem)
        public virtual float CalculateSubItemX(EItem subitem)
            // Return the X position for the subitem in question. In most cases
            // this will be our own X, but in, e.g., the RowOfColumns, it will
            // depend on which column the subitem represents.
        {
            return Position.X;
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
                return Width;
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
                Position = new PointF(Owner.CalculateSubItemX(this), Position.Y);
            }

            // Then calculate the width of the owned items (as they may need to
            // refer to this object via the AvailableWidthForOneSubitem call.)
            foreach (EItem item in SubItems)
            {
                EContainer container = item as EContainer;
                if (null != container)
                    container.CalculateContainerHorizontals();
            }
        }
        #endregion
        #region VirtMethod: void CalculateContainerVerticals(y, bRepositionOnly)
        virtual public void CalculateContainerVerticals(float y, bool bRepositionOnly)
        // Layout the children, and calclate the Height
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
        #region VirtMethod: void CalculateLineNumbers(ref nLineNo)
        virtual protected void CalculateLineNumbers(ref int nLineNo)
        {
            foreach (EContainer container in SubItems)
                container.CalculateLineNumbers(ref nLineNo);
        }
        #endregion

        // Painting --------------------------------------------------------------------------
        #region OMethod: void OnPaint(ClipRectangle)
        override public void OnPaint(Rectangle ClipRectangle)
        {
            // For performance, make sure we need to paint this container
            if (!ClipRectangle.IntersectsWith(IntRectangle))
                return;

            // Footnote Separator if indicated
            PaintFootnoteSeparator();

            // Bitmap if indicated
            PaintBitmap();

            // Paint the subcontainers
            foreach (EItem item in SubItems)
                item.OnPaint(ClipRectangle);
        }
        #endregion
    }
    #endregion

    //////////////////////////////////////////////////////////////////////////////////////////

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
        public ERowOfColumns(/*EContainer Owner, */ int cColumnsCount)
            : base(/*Owner*/)
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
                float fAvailableContentWidth = Width - 
                    ((ColumnsCount - 1) * Window.WidthBetweenColumns);
                return fAvailableContentWidth / ColumnsCount;
            }
        }
        #endregion
        #region OMethod: void CalculateContainerVerticals(y, bRepositionOnly)
        public override void CalculateContainerVerticals(float y, bool bRepositionOnly)
        {
            Position = new PointF(Position.X, y);

            // Allow for the top part of the bitmap in "y" when laying out the piles
            float fHeightBmp = CalculateBitmapHeightRequirement();

            // Calculate for the tallest pile
            float fHeightPiles = 0;
            foreach (EContainer container in SubItems)
            {
                container.CalculateContainerVerticals(y + fHeightBmp, bRepositionOnly);
                fHeightPiles = Math.Max(container.Height, fHeightPiles);
            }

            // The Row's Height is the sum of PileHeight and BmpHeight
            Height = fHeightBmp + fHeightPiles + FootnoteSeparatorHeight;
        }
        #endregion
        #region OMethod: float CalculateSubItemX(EItem subitem)
        public override float CalculateSubItemX(EItem subitem)
            // For the X, we need to figure out which column it is, and then multiply
            // by the width of a column plus the space in-between columns
        {
            int iColumn = Find(subitem);
            Debug.Assert(-1 != iColumn);

            float x = Position.X + iColumn *
                (AvailableWidthForOneSubitem + Window.WidthBetweenColumns);

            return x;
        }
        #endregion
    }
    #endregion

    #region CLASS: Row
    public class Row : ERowOfColumns
    {
        // Scaffolding -------------------------------------------------------------------
        #region Constructor(Owner, cColumns, bDisplayFootnoteSeparator)
        public Row(/*EContainer _Owner,*/ int cColumns, bool bDisplayFootnoteSeparator)
            : base( /* _Owner, */ cColumns)
        {
            // Create a pile for each column
            for (int i = 0; i < cColumns; i++)
            {
                Pile pile = new Pile();
                pile.DisplayFootnoteSeparator = bDisplayFootnoteSeparator;
                Append(pile);
            }
        }
        #endregion
    }
    #endregion

    //////////////////////////////////////////////////////////////////////////////////////////

    #region CLASS: EColumn : EContainer
    public class EColumn : EContainer
        // TODO: Should the DisplayFootnoteSeparator option be possible on all EContainers?
    {
        // Scaffolding -----------------------------------------------------------------------
        #region Constructor()
        public EColumn()
            : base()
        {
        }
        #endregion

        // Layout Calculations ---------------------------------------------------------------
        #region OMethod: void CalculateContainerHorizontals()
        public override void CalculateContainerHorizontals()
            // Override to account for the positions of individual columns within the row
        {
            Debug.Assert(Owner != null);

            // Set the width as per usual
            Width = Owner.AvailableWidthForOneSubitem;

            // The X depends upon which column we are, which the owner must tell us
            Position = new PointF(Owner.CalculateSubItemX(this), Position.Y);

            // Process the sub-items as per usual
            foreach (EItem item in SubItems)
            {
                EContainer container = item as EContainer;
                if (null != container)
                    container.CalculateContainerHorizontals();
            }
        }
        #endregion
        #region OMethod: void CalculateContainerVerticals(y, bRepositionOnly)
        public override void CalculateContainerVerticals(float y, bool bRepositionOnly)
        {
            // Remember the top-left position and width
            Position = new PointF(Position.X, y);

            // If we are displaying the footnote separator, then add a pixel to the
            // height to make room for it.
            y += FootnoteSeparatorHeight;

            // Allow for display of the bitmap if applicable
            y += CalculateBitmapHeightRequirement();

            // Layout the owned subitems, one below the other
            foreach (EContainer container in SubItems)
            {
                container.CalculateContainerVerticals(y, bRepositionOnly);
                y += container.Height;
            }

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
            }
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

    #region CLASS: Pile
    public class Pile : EColumn
    {
        // Scaffolding ---------------------------------------------------------------
        #region Constructor(Row)
        public Pile(/*Row row*/)
            : base(/*row*/)
        {
        }
        #endregion

    }
    #endregion


}
