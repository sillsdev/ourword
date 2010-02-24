#region ***** ENote.cs *****
/**********************************************************************************************
 * Project: OurWord!
 * File:    ENote.cs
 * Author:  John Wimbish
 * Created: 02 Sep 2009
 * Purpose: An icon, pointing to a TranslatorNote
 * Legal:   Copyright (c) 2004-09, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using JWTools;
using OurWord.ToolTips;
using OurWordData.DataModel;
using OurWordData.DataModel.Annotations;
using OurWordData.Styles;
#endregion

namespace OurWord.Edit
{
    public class ENote : EBlock
    {
        #region Attr{g}: TranslatorNote Note
        public TranslatorNote Note
        {
            get
            {
                Debug.Assert(null != m_Note);
                return m_Note;
            }
        }
        readonly TranslatorNote m_Note;
        #endregion
        #region OAttr{g}: float Width
        public override float Width
        {
            get
            {
                if (Context.IsSendingToPrinter)
                    return 0;
                return Bmp.Width;
            }
            set
            {
                // Can't be set; its the nature of the bitmap
            }
        }
        #endregion

        #region Constructor(TranslatorNote)
        public ENote(TranslatorNote _Note, Flags _ContextOptions)
            : base(null, "")
        {
            m_Note = _Note;

            // Don't create the ENote for "None" notes
            Debug.Assert(_ContextOptions != Flags.None);
            ContextOptions = _ContextOptions;
        }
        #endregion

        #region OMethod: void CalculateWidth()
        public override void CalculateWidth()
        {
            // Do-nothing override
        }
        #endregion

        #region Method: override void Draw(IDraw)
        public override void Draw(IDraw draw)
        {
            if (!draw.IsSendingToPrinter)
                draw.DrawImage(Bmp, Position);
        }
        #endregion

        #region Attr{g}: Cursor MouseOverCursor - Indicates what a Left-Click will do
        public override Cursor MouseOverCursor
        {
            get
            {
                return Cursors.Hand;
            }
        }
        #endregion
        #region Method: override void cmdLeftMouseClick(PointF pt)
        public override void cmdLeftMouseClick(PointF pt)
        {
            ToolTipLauncher.LaunchNow(this);
        }
        #endregion

        // Bitmap ----------------------------------------------------------------------------
        #region Attr{g}: Bitmap Bmp - the note's bitmap
        public Bitmap Bmp
        {
            get
            {
                // Get this when we first need it. We previously had this in the constructor; but the 
                // problem was that we do not have access to the Window at the time of construction.
                if (null == m_bmp)
                    InitializeBitmap(Context.BackgroundColor); 

                Debug.Assert(null != m_bmp);
                return m_bmp;
            }
        }
        Bitmap m_bmp;
        #endregion

        static public Bitmap BuildBitmap(Color backgroundColor, Color internalColor, bool bUseCheckedVersion)
        {
            var bmp = (bUseCheckedVersion) ? 
                TranslatorNote.GetCheckedIcon() : 
                TranslatorNote.GetIcon();

            // Set its transparent color to the background color.
            JWU.ChangeBitmapBackground(bmp, backgroundColor);

            // Set its internal color
            JWU.ChangeBitmapColor(bmp, TranslatorNote.OriginalBitmapNoteColor,
                internalColor);

            return bmp;
        }

        #region Method: bool InitializeBitmapWithNoteTitle()
        bool InitializeBitmapWithNoteTitle()
            // Returns true if this expanded style of TranslatorNote is what was drawn.
            // Otherwise the caller should draw the icon only.
        {
            // We do this if (1) the user setting requires it, and (2) the TranslatorNote
            // does not have the Closed status
            if (!TranslatorNote.ShowTitleWithNoteIcon)
                return false;
            if (Note.Status == Role.Closed)
                return false;

            const int xIconPosition = 3;
            const int xSpaceBetweenBitmapAndTitle = 2;

            // Retrieve the bitmap
            var bmpIcon = BuildBitmap(Note.Status.IconColor, Note.Status.IconColor, false);
            Debug.Assert(null != bmpIcon);

            // The maximum width will be arbitrarily set to 150 pixels
            const int maxWidth = 150;

            // The desired width is the length of the TranslatorNote's title
            var title = Note.Title;
            var graphics = Window.CreateGraphics();
            var font = StyleSheet.TipText.GetFont(Note.Behavior.GetWritingSystem(Note).Name, 
                FontStyle.Regular, G.ZoomPercent);
            var desiredWidth = JWU.MeasureTextDisplayWidth(title, graphics, font);
            graphics.Dispose();

            // Add room for the picture
            desiredWidth += xIconPosition;
            desiredWidth += bmpIcon.Width;
            desiredWidth += xSpaceBetweenBitmapAndTitle;

            // Our actual width is the smaller of the two
            var actualWidth = Math.Min(maxWidth, desiredWidth);

            // The height will be the containing line's line height; but larger if
            // needed in order to accomodate the icon
            var paragraph = Owner as OWPara;
            if (null == paragraph)
                return false;
            var actualHeight = (int)Math.Max(paragraph.LineHeight, bmpIcon.Height + 2);
            
            // Create the full bitmap
            var bitmap = new Bitmap(actualWidth, actualHeight);

            // Its a pain to get the Graphics to draw predictably
            graphics = Graphics.FromImage(bitmap);
            graphics.CompositingQuality = CompositingQuality.HighQuality;
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

            // We'll fill the color depending on the TranslatorNote's assign-to, 
            // and add a border to help it stand out
            var colorBackground = Note.Status.IconColor;
            graphics.FillRectangle(new SolidBrush(colorBackground), 0, 0, bitmap.Width, bitmap.Height);
            graphics.DrawRectangle(new Pen(Color.Brown), 1, 1, bitmap.Width-1, bitmap.Height-1 );

            // Draw the contents
            graphics.DrawImage(bmpIcon, xIconPosition, 2);
            graphics.DrawString(title, font, new SolidBrush(Color.Brown),
                xIconPosition + bmpIcon.Width + xSpaceBetweenBitmapAndTitle, 2);

            // Done
            graphics.Dispose();
            m_bmp = bitmap;
            return true;
        }
        #endregion
        #region Method: void InitializeBitmap(backgroundColor)
        public void InitializeBitmap(Color backgroundColor)
        {
            if (InitializeBitmapWithNoteTitle())
                return;

            m_bmp = BuildBitmap(backgroundColor, Note.Status.IconColor, false);
        }
        #endregion

        // View Context ----------------------------------------------------------------------
        #region Flags enum - UserEditable, FirstMessageOnly, DisplayMeIcon, etc.
        [Flags]
        public enum Flags
        {
            None = 0,             // Don't display the TranslatorNote
            UserEditable = 1,     // Display in conversational mode
            FirstMessageOnly = 2, // Don't display more than one message
            DisplayMeIcon = 4     // Override to display only Me, not Anyone or Closed
        };
        #endregion
        readonly Flags ContextOptions;
        #region VAttr{g}: bool UserEditable
        public bool UserEditable
        // If T, display all messages in conversational mode; else display just
        // the first message, non-editable.
        {
            get
            {
                return ((ContextOptions & Flags.UserEditable) == Flags.UserEditable);
            }
        }
        #endregion
        #region VAttr{g}: bool FirstMessageOnly
        public bool FirstMessageOnly
        {
            get
            {
                return ((ContextOptions & Flags.FirstMessageOnly) == Flags.FirstMessageOnly);
            }
        }
        #endregion
        #region VAttr{g}: bool DisplayMeIcon
        public bool DisplayMeIcon
        // If T, display the "Me" icon rather than "Anyone" or "Closed", regardless
        // of the context. (We do this for HintsForDaughters in the Drafting
        // window, to make sure the user sees the note.)
        {
            get
            {
                return ((ContextOptions & Flags.DisplayMeIcon) == Flags.DisplayMeIcon);
            }
        }
        #endregion

        // Tooltip ---------------------------------------------------------------------------
        #region Attr{g}: bool HasToolTip()
        public override bool HasToolTip()
        {
            return true;
        }
        #endregion        
        #region OMethod: void LaunchToolTip()
        public override void LaunchToolTip()
        {
            var y = (int)(Middle.Y - Window.ScrollBarPosition);
            var ptScreenLocation = Window.PointToScreen(new Point(Middle.X, y));

            var tip = new EditableNoteTip(this);
            tip.Launch(ptScreenLocation);
        }
        #endregion
    }

}
