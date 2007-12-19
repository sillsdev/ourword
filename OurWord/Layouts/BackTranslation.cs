/**********************************************************************************************
 * Project: Our Word!
 * File:    Layouts\BackTranslation.cs
 * Author:  John Wimbish
 * Created: 18 Nov 2004
 * Purpose: Manages the Back Translation view.
 * Legal:   Copyright (c) 2005-08, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
#region Using
using System;
using System.Diagnostics;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.IO;

using OurWord.DataModel;
using OurWord.Edit;
using OurWord.View;
using JWdb;
using JWTools;
#endregion

namespace OurWord.View
{

    public class WndBackTranslation : OWWindow
    {
        // Registry-Stored Settings ----------------------------------------------------------
        public const string c_sName = "BT";
        #region SAttr{g/s}: string RegistryBackgroundColor - background color for this type of window
        static public string RegistryBackgroundColor
        {
            get
            {
                return OWWindow.GetRegistryBackgroundColor(c_sName, "Linen");
            }
            set
            {
                OWWindow.SetRegistryBackgroundColor(c_sName, value);
            }
        }
        #endregion

        // Scaffolding -----------------------------------------------------------------------
        const int c_cColumnCount = 2;
        #region Constructor()
        public WndBackTranslation()
            : base(c_sName, c_cColumnCount)
        {
            // We want to maintain text below the cursor so the user does not think
            // there is nothing below when there actually is.
            ScrollPositionBufferMargin = 50;

            // It seems to appear better without a line between the columns
            DrawLineBetweenColumns = false;

            // Establish a few pixels around the edges
            ColumnMargins = new SizeF(5, 5);

            // Background color for those parts that are editable
            EditableBackgroundColor = Color.White;
        }
        #endregion
        #region Cmd: OnGotFocus - make sure commands are properly enabled
        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
            G.App.EnableMenusAndToolbars();
        }
        #endregion
        #region Attr{g}: override string WindowName
        public override string WindowName
        {
            get
            {
                return StrRes.BackTransJobTitle;
            }
        }
        #endregion
        #region Attr{g}: override string PassageName
        public override string PassageName
        {
            get
            {
                return StrRes.BackTransJobReference;
            }
        }
        #endregion

        // Create the Window Contents from the data ------------------------------------------
        const int c_xMaxPictureWidth = 300;
        #region Method: override void LoadData()
        public override void LoadData()
        {
            // Start with an empty window
            Clear();

            // Nothing more to do if we don't have a completely-defined project
            if (!G.Project.HasDataToDisplay)
                return;

            // Load the paragraphs
            foreach (DParagraph p in G.STarget.Paragraphs)
            {
                // Retrieve the bitmap, if a picture is involved
                Bitmap bmp = null;
                DPicture pict = p as DPicture;
                if (null != pict)
                    bmp = pict.GetBitmap(c_xMaxPictureWidth);

                // Start the new row and add the left side (vernacular)
                StartNewRow(false, bmp);

                // If we have no content, then we don't add the paragraphs.
                // (E.g., a picture with no caption.)
                if (p.SimpleText.Length == 0 && p.SimpleTextBT.Length == 0)
                    continue;

                // Add the vernacular paragraph
                AddParagraph(0, p, false, false);

                // For certain types of paragraphs, we just display them on the BT side, rather
                // than back-translating them. These paragraphs are ones we generated from the 
                // Front, rather than ones the translator will have edited. (E.g., cross
                // references.)
                bool bUseBT =  ((p.IsUserEditable) ? true : false);
                AddParagraph(1, p, bUseBT, true);
            }

            // Load the footnotes
            bool bFirstFootnote = true;
            foreach (DFootnote fn in G.STarget.Footnotes)
            {
                StartNewRow(bFirstFootnote, null);
                bFirstFootnote = false;

                AddParagraph(0, fn, false, false);

                bool bUseBT = ((fn.IsUserEditable) ? true : false);
                AddParagraph(1, fn, bUseBT, true);
            }

            // Tell the superclass to finish loading, which involves laying out the window 
            // with the data we've just put in, as doing the same for any secondary windows.
            base.LoadData();
        }
        #endregion
    }


}
