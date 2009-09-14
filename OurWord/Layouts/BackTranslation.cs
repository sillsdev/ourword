/**********************************************************************************************
 * Project: Our Word!
 * File:    Layouts\BackTranslation.cs
 * Author:  John Wimbish
 * Created: 18 Nov 2004
 * Purpose: Manages the Back Translation view.
 * Legal:   Copyright (c) 2005-09, John S. Wimbish. All Rights Reserved.  
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

using JWdb.DataModel;
using OurWord.Edit;
using OurWord.Layouts;
using JWdb;
using JWTools;
#endregion

namespace OurWord.Layouts
{

    public class WndBackTranslation : Layout
    {
        // Registry-Stored Settings ----------------------------------------------------------
        public const string c_sName = "BT";
        const string c_sRegDisplayFrontInBT = "ShowFrontInBT";
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
        #region SAttr{g/s}: bool DisplayFrontInBT
        static public bool DisplayFrontInBT
        {
            get
            {
                return JW_Registry.GetValue(c_sName, c_sRegDisplayFrontInBT, false);
            }
            set
            {
                JW_Registry.SetValue(c_sName, c_sRegDisplayFrontInBT, value);
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
                return G.GetLoc_GeneralUI("BackTranslationWindowName", "Back Translation"); 
            }
        }
        #endregion
        #region Attr{g}: override string LanguageInfo
        public override string LanguageInfo
        {
            get
            {
                string sBase = G.GetLoc_GeneralUI("BackTranslationReference", "{0}");

                string sTargetName = (null == DB.TargetTranslation) ?
                   G.GetLoc_GeneralUI("NoTargetDefined", "(no target defined)") :
                   DB.TargetTranslation.DisplayName.ToUpper();

                string s = LocDB.Insert(sBase, new string[] { sTargetName });

                return s;
            }
        }
        #endregion

        // Create the Window Contents from the data ------------------------------------------
        #region Method: override void LoadData()
        public override void LoadData()
        {
            // Start with an empty window
            Clear();

            // Nothing more to do if we don't have a completely-defined project
            if (!DB.Project.HasDataToDisplay)
                return;

            // Version of the view which shows four columns, both Front and Target translations,
            // both Vernacular and BT of both.
            if (DisplayFrontInBT)
            {
                LoadFour();
                return;
            }

            // Load the paragraphs
            foreach (DParagraph p in DB.TargetSection.Paragraphs)
            {
                // Start the new row and add the left side (vernacular)
                EColumn colVernacular;
                EColumn colBackTranslation;
                ERowOfColumns row = WndDrafting.CreateRow(Contents,
                    out colVernacular, out colBackTranslation, false);
                row.Bmp = GetPicture(p);

                // If we have no content, then we don't add the paragraphs.
                // (E.g., a picture with no caption.)
                if (p.SimpleText.Length == 0 && p.SimpleTextBT.Length == 0)
                    continue;

                // Add the vernacular paragraph to the left; we don't edit it
                var op =  new OWPara(
                    p.Translation.WritingSystemVernacular,
                    p.Style,
                    p,
                    BackColor,
                    OWPara.Flags.None);
                colVernacular.Append(op);

                // For certain types of paragraphs, we just display them on the BT side, rather
                // than back-translating them. These paragraphs are ones we generated from the 
                // Front, rather than ones the translator will have edited. (E.g., cross
                // references.)
                OWPara.Flags options = OWPara.Flags.None;
                // If the vernacular was editable, then we want to show the back translation
                if (p.IsUserEditable)
                {
                    options = (
                        OWPara.Flags.ShowBackTranslation | 
                        OWPara.Flags.IsEditable |
                        OWPara.Flags.CanItalic);
                    if (OurWordMain.TargetIsLocked)
                        options |= OWPara.Flags.IsLocked;
                }

                // Create and add the display paragraph
                op = new OWPara(
                    p.Translation.WritingSystemConsultant,
                    p.Style,
                    p,
                    ((p.IsUserEditable) ? EditableBackgroundColor : BackColor),
                    options);
                colBackTranslation.Append(op);
            }

            // Load the footnotes
            bool bFirstFootnote = true;
            var vFootnotes = DB.TargetSection.AllFootnotes;
            foreach (DFootnote fn in vFootnotes)
            {
                EColumn colVernacular;
                EColumn colBackTranslation;
                ERowOfColumns row = WndDrafting.CreateRow(Contents, 
                    out colVernacular, out colBackTranslation, bFirstFootnote);
                bFirstFootnote = false;

                // Add the vernacular paragraph to the left side
                OWPara op = CreateUneditableVernacularPara(fn);
                colVernacular.Append(op);

                // Options for the display paragraph
                OWPara.Flags options = OWPara.Flags.None;
                if (fn.IsUserEditable)
                {
                    options = (
                        OWPara.Flags.ShowBackTranslation |
                        OWPara.Flags.IsEditable |
                        OWPara.Flags.CanItalic);
                    if (OurWordMain.TargetIsLocked)
                        options |= OWPara.Flags.IsLocked;
                }

                // Create and add the display paragraph
                op = CreateFootnotePara(fn,
                    fn.Translation.WritingSystemConsultant,
                    ((fn.IsUserEditable) ? EditableBackgroundColor : BackColor),
                    options);

                colBackTranslation.Append(op);
            }

            // Tell the superclass to finish loading, which involves laying out the window 
            // with the data we've just put in, as doing the same for any secondary windows.
            base.LoadData();
        }
        #endregion
        #region OMethod: bool ShowNoteIcon(TranslatorNote, bShowingBT)
        public override bool ShowNoteIcon(TranslatorNote note, bool bShowingBT)
        {
            // In the back translation, show everything in our target translation. But
            // only show it on the BT side, not the vernacular side
            if (bShowingBT && note.IsTargetTranslationNote)
                return true;

            // In the front translation, show Exegetical and Consultant notes
            if (bShowingBT && note.IsFrontTranslationNote)
            {
                if (note.IsExegeticalNote)
                    return true;
                if (note.IsConsultantNote)
                    return true;
            }

            // But nothing else
            return false;
        }
        #endregion
        #region OMethod: void SetupInsertNoteDropdown(btnInsertNote)
        public override void SetupInsertNoteDropdown(ToolStripDropDownButton btnInsertNote)
            // For the back translation, we want all types of notes to be visible
            // and available.
        {
            foreach (ToolStripItem item in btnInsertNote.DropDownItems)
                item.Visible = true;
            btnInsertNote.ShowDropDownArrow = true;
        }
        #endregion

        // Layout with Front Translation columns ---------------------------------------------
        #region Method: var LoadFour_Picture(group)
        ERowOfColumns LoadFour_Picture(SynchronizedSection.ParagraphGroup group)
        {
            // For this to hold tree, we'll have exactly one Front and one Target paragraph
            if (group.TargetParagraphs.Count != 1 || group.FrontParagraphs.Count != 1)
                return null;

            // Get the two paragraphs (pictures) we're interested in
            var pictureFront = group.FrontParagraphs[0] as DPicture;
            var pictureTarget = group.TargetParagraphs[0]as DPicture;
            if (null == pictureFront || null == pictureTarget)
                return null;         

            // We have a simple picture, one column, no paragraph content....if there is
            // no text in any of our paragraphs
            if (string.IsNullOrEmpty(pictureFront.SimpleText) &&
                string.IsNullOrEmpty(pictureFront.SimpleTextBT) &&
                string.IsNullOrEmpty(pictureTarget.SimpleText) &&
                string.IsNullOrEmpty(pictureTarget.SimpleTextBT))
            {
                var rowSimple = new ERowOfColumns(1);
                rowSimple.Bmp = GetPicture(pictureTarget);
                return rowSimple;
            }

            // Create a row of four columns
            var row = new ERowOfColumns(4);

            // Load the picture into the row
            row.Bmp = GetPicture(pictureTarget);

            // Place the Front paragraphs into the columns (uneditable)
            row.Append(new OWPara(pictureFront.Translation.WritingSystemVernacular,
                pictureFront.Style, pictureFront, BackColor, OWPara.Flags.None));
            row.Append(new OWPara(pictureFront.Translation.WritingSystemConsultant,
                pictureFront.Style, pictureFront, BackColor, OWPara.Flags.ShowBackTranslation));

            // The Target vernacular is not editable
            row.Append(new OWPara(pictureTarget.Translation.WritingSystemVernacular,
                pictureTarget.Style, pictureTarget, BackColor, OWPara.Flags.None));

            // The Target BT is editable
            var options = OWPara.Flags.ShowBackTranslation | OWPara.Flags.IsEditable |
                OWPara.Flags.CanItalic;
            if (OurWordMain.TargetIsLocked)
                options |= OWPara.Flags.IsLocked;
            row.Append(new OWPara(
                pictureTarget.Translation.WritingSystemVernacular,
                pictureTarget.Style, 
                pictureTarget,
                ((pictureTarget.IsUserEditable) ? EditableBackgroundColor : BackColor), 
                options));

            return row;
        }
        #endregion
        #region Method: var AddPara(owp, bAddFootnoteSeparator
        public const int c_nFootnoteSeparatorWidth = 60;
        EColumn AddPara(OWPara owp, bool bAddFootnoteSeparator)
        {
            var col = new EColumn();
            col.Append(owp);

            if (bAddFootnoteSeparator)
            {
                col.Border = new EContainer.FootnoteSeparatorBorder(col,
                    c_nFootnoteSeparatorWidth);
            }

            return col;
        }
        #endregion
        #region Method: void LoadFour()
        void LoadFour()
        {
            // Synchronze the sections
            var synch = new WndBackTranslation.SynchronizedSection(DB.FrontSection, DB.TargetSection);

            // Create the view
            Clear();
            var vGroups = synch.AllGroups;
            bool bFirstFootnoteEncountered = false;
            foreach (SynchronizedSection.ParagraphGroup group in vGroups)
            {
                // Do we have a picture?
                var rowPicture = LoadFour_Picture(group);
                if (null != rowPicture)
                {
                    Contents.Append(rowPicture);
                    continue;
                }

                // Each group is in a top-level row; the front is on the left, the target on the right
                var rowTop = new ERowOfColumns(2);
                Contents.Append(rowTop);

                // First footnote
                bool bAddFootnoteSeparator = false;
                if (!bFirstFootnoteEncountered && group.IsFootnoteGroup)
                {
                    bFirstFootnoteEncountered = true;
                    bAddFootnoteSeparator = true;
                }

                // Front
                var rowFront = new ERowOfColumns(2);
                rowTop.Append(rowFront);
                foreach (DParagraph p in group.FrontParagraphs)
                {
                    // Uneditable Vernacular
                    rowFront.Append(AddPara(
                        new OWPara(p.Translation.WritingSystemVernacular,
                            p.Style, p, BackColor, OWPara.Flags.None), 
                        bAddFootnoteSeparator));

                    // Uneditable BT. (The "options" switch here is for showing things like
                    // cross references, where we just copy from the Vernacular rather than
                    // requiring a back translation.)
                    var options = (p.IsUserEditable) ?
                        OWPara.Flags.ShowBackTranslation : OWPara.Flags.None;
                    rowFront.Append( AddPara(
                        new OWPara( p.Translation.WritingSystemConsultant,
                            p.Style, p, BackColor, options),
                        bAddFootnoteSeparator));
                }

                // Target
                var rowTarget = new ERowOfColumns(2);
                rowTop.Append(rowTarget);
                foreach (DParagraph p in group.TargetParagraphs)
                {
                    // Uneditable Vernacular
                    rowTarget.Append( AddPara(
                        new OWPara( p.Translation.WritingSystemVernacular,
                            p.Style, p, BackColor, OWPara.Flags.None),
                        bAddFootnoteSeparator));

                    // Editable BT
                    OWPara.Flags options = OWPara.Flags.None;
                    if (p.IsUserEditable)
                    {
                        options = (
                            OWPara.Flags.ShowBackTranslation |
                            OWPara.Flags.IsEditable |
                            OWPara.Flags.CanItalic);
                        if (OurWordMain.TargetIsLocked)
                            options |= OWPara.Flags.IsLocked;
                    }

                    rowTarget.Append( AddPara(
                        new OWPara(
                            p.Translation.WritingSystemConsultant,
                            p.Style,
                            p,
                            ((p.IsUserEditable) ? EditableBackgroundColor : BackColor),
                            options),
                        bAddFootnoteSeparator));
                }
            }

            base.LoadData();
        }
        #endregion
    }


}
