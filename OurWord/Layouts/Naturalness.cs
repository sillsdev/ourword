/**********************************************************************************************
 * Project: Our Word!
 * File:    Layouts\Naturalness.cs
 * Author:  John Wimbish
 * Created: 16 Aug 2007
 * Purpose: Manages the Naturalness Check view.
 * Legal:   Copyright (c) 2005-08, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
#region Using
using System;
using System.Diagnostics;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
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
    class WndNaturalness : OWWindow
    {
        // Registry-Stored Settings ----------------------------------------------------------
        public const string c_sName = "Naturalness";
        #region SAttr{g/s}: string RegistryBackgroundColor - background color for this type of window
        static public string RegistryBackgroundColor
        {
            get
            {
                return OWWindow.GetRegistryBackgroundColor(c_sName, "Honeydew");
            }
            set
            {
                OWWindow.SetRegistryBackgroundColor(c_sName, value);
            }
        }
        #endregion

        // Scaffolding -----------------------------------------------------------------------
        const int c_cColumnCount = 1;
        #region Constructor()
        public WndNaturalness()
            : base(c_sName, c_cColumnCount)
        {
            // We want to maintain text below the cursor so the user does not think
            // there is nothing below when there actually is.
            ScrollPositionBufferMargin = 50;

            // It seems to appear better without a line between the columns
            DrawLineBetweenColumns = false;

            // Establish a few pixels around the edges
            ColumnMargins = new SizeF(5, 5);

            // Background color for the window
            BackColor = Color.Honeydew;

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
                return G.GetLoc_GeneralUI("NaturalnessCheckWindowName", "Naturalness Check");
            }
        }
        #endregion
        #region Attr{g}: override string PassageName
        public override string PassageName
        {
            get
            {
                if (null == OurWordMain.Project.TargetTranslation)
                    return "";
                if (null == OurWordMain.Project.STarget)
                    return "";

                string sBase = G.GetLoc_GeneralUI("NaturalnessCheckReference", "{0} - {1}");

                string sTarget = OurWordMain.Project.TargetTranslation.DisplayName.ToUpper();
                string sReference = OurWordMain.Project.STarget.ReferenceName;

                string s = LanguageResources.Insert(sBase, sTarget, sReference);

                if (null != G.FTranslation && null != G.TTranslation && null != G.STarget)
                    s += (" - " + G.STarget.ReferenceName);

                return s;
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

                // Start the new row
                StartNewRow(false, bmp);

                // If we have no content, then we don't add the paragraphs.
                // (E.g., a picture with no caption.)
                if (p.SimpleText.Length == 0 && p.SimpleTextBT.Length == 0)
                    continue;

                // Add the vernacular paragraph
                OWPara.Flags options = OWPara.Flags.None;
                if (p.IsUserEditable)
                    options |= OWPara.Flags.IsEditable;
                if (G.SupressVerseNumbers)
                    options |= OWPara.Flags.SuppressVerseNumbers;
                if (G.ShowLineNumbers)
                    options |= OWPara.Flags.ShowLineNumbers;
                AddParagraph(0, p, options);
            }

            // Load the footnotes
            bool bFirstFootnote = true;
            foreach (DFootnote fn in G.STarget.Footnotes)
            {
                StartNewRow(bFirstFootnote, null);
                bFirstFootnote = false;

                OWPara.Flags options = OWPara.Flags.None;
                if (fn.IsUserEditable)
                    options |= OWPara.Flags.IsEditable;
                if (G.ShowLineNumbers)
                    options |= OWPara.Flags.ShowLineNumbers;
                AddParagraph(0, fn, options);
            }

            // Tell the superclass to finish loading, which involves laying out the window 
            // with the data we've just put in, as doing the same for any secondary windows.
            base.LoadData();
        }
        #endregion
    }
}
