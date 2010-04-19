#region ***** EFoot.cs *****
/**********************************************************************************************
 * Project: OurWord!
 * File:    EFoot.cs
 * Author:  John Wimbish
 * Created: 27 Sep 2008 (separate file 19 Apr 2010)
 * Purpose: Callout letter for a footnote
 * Legal:   Copyright (c) 2004-10, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using OurWordData.DataModel;
using OurWordData.Styles;
#endregion

namespace OurWord.Edit.Blocks
{
    public class EFoot : EBlock
    {
        #region Attr{g}: DFoot Foot
        DFoot Foot
        {
            get
            {
                Debug.Assert(null != m_Foot);
                return m_Foot;
            }
        }
        readonly DFoot m_Foot;
        #endregion
        #region VAttr{g}: DFootnote Footnote
        public DFootnote Footnote
        {
            get
            {
                Debug.Assert(null != Foot.Footnote);
                return Foot.Footnote;
            }
        }
        #endregion

        #region Constructor(font, DFoot)
        public EFoot(Font font, DFoot foot)
            : base(font, foot.Text)
        {
            m_Foot = foot;
            TextColor = StyleSheet.FootnoteLetter.FontColor;
        }
        #endregion

        #region Method: override void Draw(IDraw)
        public override void Draw(IDraw draw)
        {
            draw.DrawString(Text, m_Font, GetBrush(), Position);
        }
        #endregion

        // Explanatory Footnote
        #region VAttr{g}: bool FootnoteIsEditable - if T, we can jump to it by clicking on the letter
        bool FootnoteIsEditable
        // Test to see if the footnote is editable. If it is, then we
        // 1. Show the Hand cursor when we hover over it,
        // 2. Can jump to it
        //
        // This test will not work until the entire window has been laid out. 
        // If we don't have a selection in the window, then it is safe to
        // assume that the window is not ready.
        {
            get
            {
                if (!m_bFootnoteIsEditableComputed)
                {
                    if (!Foot.IsExplanatory)
                        return false;

                    // Can't do this if we don't have a selection
                    if (!Window.HasSelection)
                        return false;

                    // Remember our current selection
                    var bm = Window.CreateBookmark();

                    // Attempt to select the footnote
                    var container = Window.Contents.FindContainerOfDataSource(Footnote);
                    m_bFootnoteIsEditable = container.Select_FirstWord();

                    // Restore the original selection
                    bm.RestoreWindowSelectionAndScrollPosition();

                    // We did the analysis
                    m_bFootnoteIsEditableComputed = true;
                }

                return m_bFootnoteIsEditable;
            }
        }
        bool m_bFootnoteIsEditable;
        bool m_bFootnoteIsEditableComputed;
        #endregion

        #region Attr{g}: Cursor MouseOverCursor - Indicates what a Left-Click will do
        public override Cursor MouseOverCursor
        {
            get
            {
                return FootnoteIsEditable ? Cursors.Hand : Cursors.Arrow;
            }
        }
        #endregion
        #region Method: override void cmdLeftMouseClick(PointF pt)
        public override void cmdLeftMouseClick(PointF pt)
        {
            if (!Foot.IsExplanatory) 
                return;

            var container = Window.Contents.FindContainerOfDataSource(Footnote);
            container.Select_FirstWord();
        }
        #endregion
    }
}
