#region ***** EWord.cs *****
/**********************************************************************************************
 * Project: OurWord!
 * File:    EWord.cs
 * Author:  John Wimbish
 * Created: 27 Sep 2008
 * Purpose: An individual word in a paragraph
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
using OurWordData;
using OurWordData.DataModel;
#endregion
#endregion

namespace OurWord.Edit
{
    #region CLASS: EBlock
    public class EBlock : EItem
    {
        // Main Content Attrs ------------------------------------------------------------
        #region Attr{g/s}: string Text
        public string Text
        {
            get
            {
                return m_sText;
            }
            set
            {
                m_sText = value;
            }
        }
        protected string m_sText = "";
        #endregion
        #region Attr{g}: bool GlueToNext - T if this block must be beside the next one
        public virtual bool GlueToNext
        {
            get
            {
                return m_bGlueToNext;
            }
            set
            {
                m_bGlueToNext = value;
            }
        }
        bool m_bGlueToNext = false;
        #endregion

        // Screen Region -----------------------------------------------------------------
        #region OAttr{g}: float Height
        override public float Height
        {
            get
            {
                // we use the paragraph's line height, as some individual elements could
                // be using different fonts.
                return Para.LineHeight;
            }
            set
            {
                Debug.Assert(false, "Can't set the line height of an EBlock");
            }
        }
        #endregion
        #region OMethod: bool ContainsPoint(PointF pt)
        public override bool ContainsPoint(PointF pt)
        {
            RectangleF r = new RectangleF(Position,
                new SizeF(Width + JustificationPaddingAdded, Height));
            return r.Contains(pt);
        }
        #endregion
        #region OMethod: EBlock GetBlockAt(PointF)
        public override EBlock GetBlockAt(PointF pt)
        {
            return this;
        }
        #endregion
        #region Attr{g}: int JustificationPaddingAdded
        public int JustificationPaddingAdded
        {
            get
            {
                return m_nJustificationPaddingAdded;
            }
            set
            {
                m_nJustificationPaddingAdded = value;
            }
        }
        int m_nJustificationPaddingAdded = 0;
        #endregion

        // Scaffolding -------------------------------------------------------------------
        #region VAttr{g}: OWPara Para - Keep a pointer back to the owner so we can get, e.g., the PStyle
        public OWPara Para
        {
            get
            {
                OWPara para = Owner as OWPara;
                Debug.Assert(null != para);
                return para;
            }
        }
        #endregion
        #region VAttr{g}: OWWindow.DrawBuffer Draw
        public OWWindow.DrawBuffer Draw
        {
            get
            {
                return Owner.Window.Draw;
            }
        }
        #endregion
        #region VAttr{g}: int PositionWithinPara
        public int PositionWithinPara
        {
            get
            {
                for (int i = 0; i < Para.SubItems.Length; i++)
                {
                    if (Para.SubItems[i] as EBlock == this)
                        return i;
                }
                return -1;
            }
        }
        #endregion
        #region Constructor(JFontForWritingSystem, sText)
        public EBlock(JFontForWritingSystem _FontForWS, string _sText)
            : base()
        {
            m_sText = _sText;
            m_FontForWS = _FontForWS;
        }
        #endregion
        #region VirtMethod: bool ContentEquals(EBlock block)
        public virtual bool ContentEquals(EBlock block)
        {
            if (null == block)
                return false;

            if (Text != block.Text)
                return false;

            if (GlueToNext != block.GlueToNext)
                return false;

            return true;
        }
        #endregion

        // Painting ----------------------------------------------------------------------
        #region Attr{g}: JFontForWritingSystem FontForWS - remember it here for performance
        public JFontForWritingSystem FontForWS
        {
            get
            {
                Debug.Assert(null != m_FontForWS);
                return m_FontForWS;
            }
        }
        protected JFontForWritingSystem m_FontForWS = null;
        #endregion
        #region Method: Font GetSuperscriptFont()
        protected Font GetSuperscriptFont()
        {
            Font f = FontForWS.DefaultFontZoomed;
            return new Font(f.FontFamily,
                f.Size * 0.8F,
                f.Style);
        }
        #endregion
        #region Method: Brush GetBrush()
        protected Brush GetBrush()
        {
            return new SolidBrush(FontForWS.FontColor);
        }
        #endregion
        #region Method: Pen GetPen()
        protected Pen GetPen()
        {
            return new Pen(FontForWS.FontColor);
        }
        #endregion

        #region Method: virtual void Paint() - for the editor
        virtual public void Paint()
        {
        }
        #endregion
        #region Method: virtual void Print(g) - for the printer
        virtual public void Print(Graphics g)
        {
        }
        #endregion

        // Mousing
        #region Attr{g}: Cursor MouseOverCursor - Indicates what a Left-Click will do
        public virtual Cursor MouseOverCursor
        {
            get
            {
                return Cursors.Arrow;
            }
        }
        #endregion
        #region Cmd: cmdLeftMouseClick
        public virtual void cmdLeftMouseClick(PointF pt)
        {
        }
        #endregion
        #region Cmd: cmdLeftMouseDoubleClick
        public virtual void cmdLeftMouseDoubleClick(PointF pt)
        {
        }
        #endregion
        #region Cmd: cmdMouseMove
        public virtual void cmdMouseMove(PointF pt)
        {
        }
        #endregion

        // Tooltips
        #region Attr{g}: HasTooltip
        virtual public bool HasToolTip()
        {
            // Return false if no tooltip is desired
            return false;
        }
        #endregion
        #region Method: void LoadToolTip(ToolTipContents)
        virtual public void LoadToolTip(ToolTipContents wnd)
        {
        }
        #endregion

        // Layout Calculations ---------------------------------------------------------------
        #region VirtMethod: void CalculateWidth()
        virtual public void CalculateWidth()
            // Those subclasses which override will not need to call this base method.
        {
            // For the most cases, we'll measure the width of Text according to the
            // font stored in the CharacterStyle.
            var fmt = StringFormat.GenericTypographic;
            fmt.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces;
            Width = Context.Graphics.MeasureString(Text, FontForWS.DefaultFontZoomed, 1000, fmt).Width;
        }
        #endregion

    }
    #endregion

    #region CLASS: EWord : EBlock
    public class EWord : EBlock
    {
        // Attrs -----------------------------------------------------------------------------
        #region VAttr{g}: Font Font - returns the font for the WS, including Italic, Bold, etc.
        Font Font
        {
            get
            {
                // Optimization
                if (FontMods == FontStyle.Regular)
                    return FontForWS.DefaultFontZoomed;

                // Generic find-or-create font as needed
                return FontForWS.FindOrAddFont(true, FontMods);
            }
        }
        #endregion
        #region Attr{g}: DPhrase Phrase - the phrase from which this EWord was generated
        public DPhrase Phrase
        {
            get
            {
                Debug.Assert(null != m_Phrase);
                return m_Phrase;
            }
        }
        DPhrase m_Phrase = null;
        #endregion
        #region Attr{g}: FontStyle FontMods
        FontStyle FontMods
        {
            get
            {
                return m_FontMods;
            }
        }
        FontStyle m_FontMods;
        #endregion

        // Hyphenation
        #region Attr{g/s}: bool Hyphenated
        public bool Hyphenated
        {
            get
            {
                return m_Hyphenated;
            }
            set
            {
                m_Hyphenated = value;
            }
        }
        bool m_Hyphenated = false;
        #endregion
        #region Attr{g/s}: float HyphenWidth
        float HyphenWidth
        {
            get
            {
                return m_fHyphenWidth;
            }
            set
            {
                m_fHyphenWidth = value;
            }
        }
        float m_fHyphenWidth = 0;
        #endregion

        // Scaffolding -----------------------------------------------------------------------
        #region Constructor(...)
        public EWord( 
            JFontForWritingSystem _FontForWS,
            DPhrase phrase, 
            string sText, 
            FontStyle _FontMods)
            : base(_FontForWS, sText)
        {
            // Remember the phrase from which this EWord was generated
            m_Phrase = phrase;

            // Retrieve the JFont as passed in.
            m_FontMods = _FontMods;
        }
        #endregion
        #region Method: EWord Clone()
        public virtual EWord Clone()
        {
            EWord word = new EWord(
                FontForWS,
                Phrase,
                Text, 
                FontMods);
            return word;
        }
        #endregion
        #region static EWord CreateAsInsertionIcon(OWPara, JCharacterStyle, DPhrase)
        static public EWord CreateAsInsertionIcon(
            JFontForWritingSystem _FontForWS,
            DPhrase phrase)
        {
            EWord word = new EWord(_FontForWS, phrase, c_chInsertionSpace.ToString(),
                FontStyle.Regular);
            return word;
        }
        #endregion

        // Painting --------------------------------------------------------------------------
        #region Method: void PaintBackgroundRectangle(Color color)
        void PaintBackgroundRectangle(Color color)
        {
            RectangleF r = new RectangleF(Position,
                new SizeF(Width + JustificationPaddingAdded - HyphenWidth, Height));
            Draw.FillRectangle(color, r);
        }
        #endregion
        #region Method: override void Paint()
        public override void Paint()
        {
            // The white background
            PaintBackgroundRectangle(
                (Para.IsEditable && !Para.IsLocked) ?
                    Para.EditableBackgroundColor :
                    Para.NonEditableBackgroundColor);

            // The text
            if (!IsInsertionIcon)
            {
                Draw.DrawString(Text, Font, GetBrush(), Position);

                if (Hyphenated)
                {
                    Draw.DrawString("-", Font, GetBrush(), 
                        new PointF(Position.X + Width - HyphenWidth, Position.Y));
                }
            }
        }
        #endregion
        #region Method: void PaintSelection(int iCharLeft, int iCharRight)
        public void PaintSelection(int iCharLeft, int iCharRight)
        {
            // Create the colors and brushes we'll need
            Color clrSelectedBackground = SystemColors.Highlight;
            Color clrSelectedText = SystemColors.HighlightText;
            Brush brushSelectedText = new SolidBrush(clrSelectedText);
            Brush brushNormalText = new SolidBrush(FontForWS.FontColor);

            // Insertion Icon
            if (IsInsertionIcon)
            {
                PaintBackgroundRectangle(clrSelectedBackground);
                Draw.DrawString(G.GetLoc_String("TypeHere", "[Type Here]"),
                    Font, brushSelectedText, Position);
                return;
            }

            // Interpret the default values
            if (iCharLeft == -1)
                iCharLeft = 0;
            if (iCharRight == -1)
                iCharRight = Text.Length;

            // Optimization/Shortcut: Paint the entire word as selected if apropriate
            if (iCharLeft == 0 && iCharRight == Text.Length)
            {
                PaintBackgroundRectangle(clrSelectedBackground);
                Draw.DrawString(Text, Font, brushSelectedText, Position);
                return;
            }

            // If the two parameters are still the same, then we have no selection to paint
            if (iCharLeft == iCharRight)
                return;

            // Figure out the selection texts
            string sLeft = (iCharLeft == 0) ? "" : Text.Substring(0, iCharLeft);
            string sTemp = (iCharLeft == 0) ? Text : Text.Substring(iCharLeft);
            string sSelected = (iCharRight == Text.Length) ? sTemp :
                sTemp.Substring(0, iCharRight - iCharLeft);
            string sRight = (iCharRight == Text.Length) ? "" :
                sTemp.Substring(iCharRight - iCharLeft);

            // Figure out the boundaries
            float fTotalWidth = Width + JustificationPaddingAdded;
            float xSelLeft = Position.X +
                ((iCharLeft == 0) ? 0 : Context.Measure(sLeft, Font));
            float xSelRight = xSelLeft + ((iCharRight == Text.Length) ?
                fTotalWidth - Context.Measure(sLeft, Font) :
                Context.Measure(sSelected, Font));

            // Paint the white background, for those portions that are not selected
            PaintBackgroundRectangle(
                Para.IsLocked ? Para.NonEditableBackgroundColor : Para.EditableBackgroundColor);

            // Paint the selected background
            RectangleF rectSelected = new RectangleF(xSelLeft, Position.Y, xSelRight - xSelLeft, Height);
            Draw.FillRectangle(clrSelectedBackground, rectSelected);

            // Paint the text
            if (sLeft.Length > 0)
                Draw.DrawString(sLeft, Font, brushNormalText, Position);

            Draw.DrawString(sSelected, Font, brushSelectedText,
                new PointF(xSelLeft, Position.Y));

            if (sRight.Length > 0)
            {
                Draw.DrawString(sRight, Font, brushNormalText,
                    new PointF(xSelRight, Position.Y));
            }
        }
        #endregion
        #region Method: float GetXat(int i)
        public float GetXat(int i)
        {
            if (i == 0)
                return 0;

            return Context.Measure(Text.Substring(0, i), Font);
        }
        #endregion

        public override void Print(Graphics g)
        {
            var s = Text;
            if (Hyphenated)
                s += "-";

            g.DrawString(s, Font, GetBrush(), Position);
        }


        // Insertion Point ---------------------------------------------------------------
        public const char c_chInsertionSpace = '\u2004';   // Unicode's "Four-Per-EM space"
        public const float c_xInsertionSize = 50;          // Pixels for insertion display
        #region Attr{g}: bool IsInsertionIcon
        public bool IsInsertionIcon
        {
            get
            {
                if (Text.Length == 1 && Text[0] == c_chInsertionSpace)
                    return true;
                return false;
            }
        }
        #endregion

        // Queries -----------------------------------------------------------------------
        #region Method: int GetCharacterIndex(PointF pt)
        public int GetCharacterIndex(PointF pt)
        // Find out the index of the character we've just clicked over
        {
            int iChar = 0;
            float x1 = Position.X;
            for (; iChar < Text.Length; iChar++)
            {
                // Get the substring up to iChar +1 letters (this will max out at the
                // total possible length of Text.
                string sPortion = Text.Substring(0, iChar + 1);

                // Get the position right after this character
                float x2 = Position.X + Context.Measure(sPortion, Font);

                // The average should thus be right in the midst of this character,
                // since x1 represents the position to the char's left.
                float xAvg = (x1 + x2) / 2;

                // So if we are less than the midpoint, then we'll be returning the
                // value of this character; otherwise we loop to the next one.
                if (pt.X < xAvg)
                    break;

                x1 = x2;
            }
            return iChar;
        }
        #endregion
        #region Method: bool IsBesideEWord(bool bOnRight)
        public bool IsBesideEWord(bool bOnRight)
        {
            int iBlock = Para.Find(this);

            EBlock blockBeside = null;

            if (bOnRight)
            {
                if (iBlock == Para.SubItems.Length - 1)
                    return false;
                blockBeside = Para.SubItems[iBlock + 1] as EBlock;
            }
            else
            {
                if (iBlock == 0)
                    return false;
                blockBeside = Para.SubItems[iBlock - 1] as EBlock;
            }

            if (null != blockBeside as EWord)
                return true;

            return false;
        }
        #endregion
        #region Attr{g}: bool EndsWithWhiteSpace
        public bool EndsWithWhiteSpace
        {
            get
            {
                if (Text.Length == 0)
                    return false;
                if (char.IsWhiteSpace(Text[Text.Length - 1]))
                    return true;
                return false;
            }
        }
        #endregion

        // Command Processing ----------------------------------------------------------------
        #region Attr{g}: Cursor MouseOverCursor - Shape of cursor indicates what a Left-Click will do
        public override Cursor MouseOverCursor
        {
            get
            {
                if (!Para.IsEditable)
                    return Cursors.Default;
                return Cursors.IBeam;
            }
        }
        #endregion
        #region Cmd: cmdLeftMouseClick - Make a selection at this coordinate
        public override void cmdLeftMouseClick(PointF pt)
        // Create a selection Anchor at this point. Given a click in the middle of
        // a letter, we decide whether to select at the beginning or end of that
        // letter (by finding the average of the begin/end measurements of that
        // letter.)
        {
            // If the paragraph is not editable, then it is a moot point; no sel is possible
            if (!Para.IsEditable)
                return;

            // Find out the index of the character we've just clicked over
            int iChar = GetCharacterIndex(pt);

            // Get the number of this block within the paragraph
            int iBlock = PositionWithinPara;

            // For an insertion icon, just select the entire word
            if (IsInsertionIcon)
            {
                Para.Window.Selection = new OWWindow.Sel(Para,
                    new OWWindow.Sel.SelPoint(iBlock, 0),
                    new OWWindow.Sel.SelPoint(iBlock, Text.Length));
                return;
            }

            // Create the Selection
            OWWindow.Sel.SelPoint Anchor = new OWWindow.Sel.SelPoint(iBlock, iChar);
            OWWindow.Sel selection = new OWWindow.Sel(Para, Anchor);
            Para.Window.Selection = Para.NormalizeSelection(selection);
        }
        #endregion
        #region Cmd: cmdMouseMove
        public override void cmdMouseMove(PointF pt)
        {
            // Do we already have a selection?
            if (Window.Selection == null)
                return;

            // Is the selection within this paragraph?
            if (Window.Selection.Paragraph != Para)
                return;

            // If this paragraph is not editable, then we can't do anything (probably
            // should never get to this code anyway.)
            if (!Para.IsEditable)
                return;

            // Is there anything between Here and the Anchor that is NOT an EWord?
            int iEnd = PositionWithinPara;
            int iAnchor = Window.Selection.Anchor.iBlock;
            int iFirst = (iAnchor < iEnd) ? iAnchor : iEnd;
            int iLast = (iEnd > iAnchor) ? iEnd : iAnchor;
            for (int i = iFirst; i < iLast; i++)
            {
                if (Para.SubItems[i] as EWord == null)
                    return;
            }
            int iChar = GetCharacterIndex(pt);

            // If this is the same position as our previous mouse move, then
            // do nothing. Seems that Windows keeps feeding these messages even when the
            // mouse doesn't move, and this prevents the screen from being repainted,
            // and thus the selection never shows up.
            if (null != Window.Selection.End && Window.Selection.End.iChar == iChar)
                return;

            // Passed the test; so extend the selection to this new End point
            OWWindow.Sel.SelPoint end = new OWWindow.Sel.SelPoint(iEnd, iChar);
            OWWindow.Sel selection = new OWWindow.Sel(Para, Window.Selection.Anchor, end);
            Window.Selection = Para.NormalizeSelection(selection);
        }
        #endregion
        #region Cmd: cmdLeftMouseDoubleClick
        public override void cmdLeftMouseDoubleClick(PointF pt)
        {
            // If the paragraph is not editable, then it is a moot point; no sel is possible
            if (!Para.IsEditable)
                return;

            // Get the number of this block within the paragraph
            int iBlock = PositionWithinPara;

            // Select the entire word
            Para.Window.Selection = new OWWindow.Sel(Para,
                new OWWindow.Sel.SelPoint(iBlock, 0),
                new OWWindow.Sel.SelPoint(iBlock, Text.Length));
        }
        #endregion

        // Layout Calculations ---------------------------------------------------------------
        #region OMethod: void CalculateWidth()
        override public void CalculateWidth()
        {
			HyphenWidth = 0;

            if (IsInsertionIcon)
            {
                Width = Context.Measure(G.GetLoc_String("TypeHere", "[Type Here]"), Font);
            }
            else if (Hyphenated)
            {
                Width = Context.Measure(Text, Font);

                HyphenWidth = Context.Measure("-", Font);
                Width += HyphenWidth;
            }
            else
            {
                base.CalculateWidth();
            }
        }
        #endregion
    }
    #endregion

    #region CLASS: EInterlinear : EBlock
    public class EInterlinear : EBlock
    {
        // Attrs -----------------------------------------------------------------------------
        #region Attr{g/s}: string Meaning
        public string Meaning
        {
            get
            {
                return m_sMeaning;
            }
            set
            {
                m_sMeaning = value;
            }
        }
        string m_sMeaning;
        #endregion

        // Bundles ---------------------------------------------------------------------------
        #region CLASS: EBundle
        public class EBundle
        {
            #region Attr{g}: string Text
            public string Text
            {
                get
                {
                    return m_sText;
                }
            }
            string m_sText;
            #endregion
            #region Attr{g}: string Meaning
            public string Meaning
            {
                get
                {
                    return m_sMeaning;
                }
            }
            string m_sMeaning;
            #endregion

            // Scaffolding -------------------------------------------------------------------
            #region Constructor(sText, sMeaning)
            public EBundle(string _sText, string _sMeaning)
            {
                m_sText = _sText;
                m_sMeaning = _sMeaning;
            }
            #endregion
            #region Method: bool ContentEquals(EBundle item)
            public bool ContentEquals(EBundle item)
            {
                if (null == item)
                    return false;

                if (item.Text != Text)
                    return false;

                if (item.Meaning != Meaning)
                    return false;

                return true;
            }
            #endregion

            // I/O ---------------------------------------------------------------------------
            #region I/O CONSTANTS
            const string c_sTag = "B";
            const string c_sAttrText = "T";
            const string c_sAttrMeaning = "M";
            #endregion
            #region VAttr{g}: XElement ToXml
            public XElement ToXml
            {
                get
                {
                    XElement x = new XElement(c_sTag);
                    x.AddAttr(c_sAttrText, Text);
                    x.AddAttr(c_sAttrMeaning, Meaning);
                    return x;
                }
            }
            #endregion
            #region SMethod: EBundle CreateFromXml(XElement x)
            static public EBundle CreateFromXml(XElement x)
            {
                if (x.Tag != c_sTag)
                    return null;

                string sText = x.GetAttrValue(c_sAttrText, "");
                string sMeaning = x.GetAttrValue(c_sAttrMeaning, "");

                EBundle bundle = new EBundle(sText, sMeaning);

                return bundle;
            }
            #endregion
        }
        #endregion
        #region Attr{g}: EBundle[] Bundles
        EBundle[] Bundles
        {
            get
            {
                Debug.Assert(null != m_vBundles);
                return m_vBundles;
            }
        }
        EBundle[] m_vBundles;
        #endregion
        #region Method: void AppendBundle(EBundle bundle)
        public void AppendBundle(EBundle bundle)
        {
            EBundle[] v = new EBundle[Bundles.Length + 1];
            for (int i = 0; i < Bundles.Length; i++)
                v[i] = Bundles[i];
            v[Bundles.Length] = bundle;
            m_vBundles = v;
        }
        #endregion

        // VAttrs ----------------------------------------------------------------------------
        #region VAttr{g}: bool IsEmpty
        public bool IsEmpty
        {
            get
            {
                if (!string.IsNullOrEmpty(Meaning))
                    return false;

                if (Bundles.Length != 0)
                    return false;

                return true;
            }
        }
        #endregion

        // Scaffolding -----------------------------------------------------------------------
        #region Constructor()
        public EInterlinear(JFontForWritingSystem f)
            : base(f, "")
        {
            m_vBundles = new EBundle[0];
        }
        #endregion
        #region OMethod: bool ContentEquals(EBlock block)
        public override bool ContentEquals(EBlock block)
        {
            EInterlinear item = block as EInterlinear;
            if (null == item)
                return false;

            if (!base.ContentEquals(item))
                return false;

            if (item.Meaning != Meaning)
                return false;

            if (item.Bundles.Length != Bundles.Length)
                return false;

            for (int i = 0; i < Bundles.Length; i++)
            {
                if (!Bundles[i].ContentEquals(item.Bundles[i]))
                    return false;
            }

            return true;
        }
        #endregion

        // I/O -------------------------------------------------------------------------------
        #region I/O CONSTANTS
        const string c_sTag = "I";
        const string c_sAttrText = "T";
        const string c_sAttrMeaning = "M";
        const string c_sAttrGlue = "G";
        #endregion
        #region VAttr{g}: XElement ToXml
        public XElement ToXml
        {
            get
            {
                if (IsEmpty)
                    return null;

                XElement x = new XElement(c_sTag);

                x.AddAttr(c_sAttrText, Text);
                x.AddAttr(c_sAttrMeaning, Meaning);
                x.AddAttr(c_sAttrGlue, GlueToNext);

                foreach (EBundle bundle in Bundles)
                    x.AddSubItem(bundle.ToXml);

                return x;
            }
        }
        #endregion
        #region VAttr{g}: string XmlOneLiner
        public string XmlOneLiner
        {
            get
            {
                XElement x = ToXml;

                return (null == x) ? "" : x.OneLiner;
            }
        }
        #endregion
        #region SMethod: EInterlinear CreateFromXml(string s)
        static public EInterlinear CreateFromXml(OWPara p, string s)
        {
            // Parse into an element tree
            XElement[] vx = XElement.CreateFrom(s);
            if (null == vx || vx.Length != 1)
                return null;
            XElement x = vx[0];
            if (x.Tag != c_sTag)
                return null;

            // Retrieve the Meaning
            EInterlinear ei = new EInterlinear(null);
            ei.m_sMeaning = x.GetAttrValue(c_sAttrMeaning, "");
            ei.m_sText = x.GetAttrValue(c_sAttrText, "");
            ei.GlueToNext = x.GetAttrValue(c_sAttrGlue, false);

            // Retrieve the bundles
            foreach (XItem item in x.Items)
            {
                XElement xSub = item as XElement;
                if (null == xSub)
                    continue;

                EBundle bundle = EBundle.CreateFromXml(xSub);
                if (null != bundle)
                    ei.AppendBundle(bundle);
            }

            return ei;
        }
        #endregion
    }
    #endregion
}
