#region ***** ENote.cs *****
/**********************************************************************************************
 * Project: OurWord!
 * File:    ENote.cs
 * Author:  John Wimbish
 * Created: 02 Sep 2009
 * Purpose: An note icon, pointing to a not
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
using System.Text;
using System.Windows.Forms;
using JWTools;
using JWdb;
using JWdb.DataModel;
#endregion
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
        TranslatorNote m_Note = null;
        #endregion
        #region OAttr{g}: float Width
        public override float Width
        {
            get
            {
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

        #region OMethod: void CalculateWidth(Graphics g)
        public override void CalculateWidth(Graphics g)
        {
            // Do-nothing override
        }
        #endregion

        #region Method: override void Paint()
        public override void Paint()
        {
            Draw.Image(Bmp, Position);
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
            OWToolTip.ToolTip.LaunchToolTipWindow();
        }
        #endregion

        // Bitmap ----------------------------------------------------------------------------
        #region Attr{g}: Bitmap Bmp - the note's bitmap
        Bitmap Bmp
        {
            get
            {
                // Get this when we first need it. We previously had this in the constructor; but the 
                // problem was that we do not have access to the Window at the time of construction.
                if (null == m_bmp)
                    InitializeBitmap();

                Debug.Assert(null != m_bmp);
                return m_bmp;
            }
        }
        Bitmap m_bmp = null;
        #endregion
        #region Method: void InitializeBitmap()
        public void InitializeBitmap()
        {
            // Get the name of the file; this depends on the note and its context
            string sResource = NoteIconResource;

            // Retrieve the bitmap from resources
            Bitmap bmp = JWU.GetBitmap(sResource);
            Debug.Assert(null != bmp);

            // Set its transparent color to the background color.
            m_bmp = JWU.ChangeBitmapBackground(bmp, Window.BackColor);
        }
        #endregion
        #region VAttr{g}: string NoteIconResource
        public string NoteIconResource
        {
            get
            {
                // If DisplayMe is set, it overrides all other logic
                if (DisplayMeIcon)
                    return Note.Behavior.IconResourceMe;

                if (Note.Status == DMessage.Closed)
                    return Note.Behavior.IconResourceClosed;

                if (Note.Status == DB.UserName)
                    return Note.Behavior.IconResourceMe;

                // Default
                return Note.Behavior.IconResourceAnyone;
            }
        }
        #endregion

        // View Context ----------------------------------------------------------------------
        #region Flags enum - UserEditable, FirstMessageOnly, DisplayMeIcon, etc.
        [Flags]
        public enum Flags
        {
            None = 0,             // Don't display the note
            UserEditable = 1,     // Display in conversational mode
            FirstMessageOnly = 2, // Don't display more than one message
            DisplayMeIcon = 4     // Override to display only Me, not Anyone or Closed
        };
        #endregion
        Flags ContextOptions;
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
        #region Method: void LoadToolTip(ToolTipContents)
        override public void LoadToolTip(ToolTipContents wnd)
        {
            var builder = new AnnotationTipBuilder(wnd, Note);

            // Title
            builder.LoadNoteTitle(NoteIconResource);

            if (FirstMessageOnly)
            {
                builder.LoadSingleMessageContents();
            }
            else if (UserEditable)
            {
                builder.LoadInteractiveMessages();
                builder.LoadToolStrip();
            }
        }
        #endregion
        #region Attr{g}: bool HasToolTip()
        public override bool HasToolTip()
        {
            return true;
        }
        #endregion
    }

}
