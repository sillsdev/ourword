#region ***** ConsultantPreparation.cs *****
/**********************************************************************************************
 * Project: Our Word!
 * File:    Layouts\ConsultantPreparation.cs
 * Author:  John Wimbish
 * Created: 21 Oct 2009
 * Purpose: Manages the ConsultantPreparation view.
 * Legal:   Copyright (c) 2005-09, John S. Wimbish. All Rights Reserved.  
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

using OurWordData.DataModel;
using OurWord.Edit;
using OurWord.Layouts;
using OurWordData;
using JWTools;
using OurWordData.DataModel.Annotations;
using OurWordData.Styles;

#endregion
#endregion

namespace OurWord.Layouts
{
    class WndConsultantPreparation : WLayout
    {
        // Registry-Stored Settings ----------------------------------------------------------
        public const string c_sName = "ConsultantPreparation";
        #region SAttr{g/s}: string RegistryBackgroundColor - background color for this type of window
        static public string RegistryBackgroundColor
        {
            get
            {
                return OWWindow.GetRegistryBackgroundColor(c_sName, "LightYellow");
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
        public WndConsultantPreparation()
            : base(c_sName, c_cColumnCount)
        {
            // We want to maintain text below the cursor so the user does not think
            // there is nothing below when there actually is.
            ScrollPositionBufferMargin = 50;

            // Background color for those parts that are editable
            EditableBackgroundColor = Color.White;
        }
        #endregion
        #region OAttr{g}: string LayoutName
        public override string LayoutName
        {
            get
            {
                return c_sName;
            }
        }
        #endregion
        #region Attr{g}: override string WindowName
        public override string WindowName
        {
            get
            {
                return G.GetLoc_GeneralUI("ConsultantPreparationWindowName", "Consultant Preparation");
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
        #region Method: bool LoadPicture(group)
        bool LoadPicture(SynchronizedSection.ParagraphGroup group)
        {
            // For this to hold tree, we'll have exactly one Front and one Target paragraph
            if (group.TargetParagraphs.Count != 1 || group.FrontParagraphs.Count != 1)
                return false;

            // Get the two paragraphs (pictures) we're interested in
            var pictureFront = group.FrontParagraphs[0] as DPicture;
            var pictureTarget = group.TargetParagraphs[0] as DPicture;
            if (null == pictureFront || null == pictureTarget)
                return false;

            // We have a simple picture, one column, no paragraph content....if there is
            // no text in any of our paragraphs
            if (string.IsNullOrEmpty(pictureFront.SimpleText) &&
                string.IsNullOrEmpty(pictureFront.SimpleTextBT) &&
                string.IsNullOrEmpty(pictureTarget.SimpleText) &&
                string.IsNullOrEmpty(pictureTarget.SimpleTextBT))
            {
                var rowSimple = new ERowOfColumns(1);
                rowSimple.SetPicture(GetPicture(pictureTarget), true);
                Contents.Append(rowSimple);
                return true;
            }

            // Create a row of four columns
            var row = new ERowOfColumns(4);

            // Load the picture into the row
            row.SetPicture(GetPicture(pictureTarget), true);

            // Place the Front paragraphs into the columns (uneditable)
            row.Append(new OWPara(pictureFront.Translation.WritingSystemVernacular,
                pictureFront.Style, pictureFront, BackColor, OWPara.Flags.None));
            row.Append(new OWPara(pictureFront.Translation.WritingSystemConsultant,
                pictureFront.Style, pictureFront, BackColor,
                OWPara.Flags.ShowBackTranslation | OWPara.Flags.IsLocked));

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

            Contents.Append(row);

            return true;
        }
        #endregion
        #region Method: OWPara BuildParagraph(p, ws, backColor, flags)
        OWPara BuildParagraph(DParagraph p, WritingSystem ws, Color backColor, OWPara.Flags flags)
            // Creates an OWPara, but switches based on whether we're dealing with a footnote or
            // a normal paragraph (the Footnote code inserts the footnote reference/label.)
        {
            var fn = p as DFootnote;
            if (null != fn)
                return CreateFootnotePara(fn, ws, backColor, flags);

            return new OWPara(ws, p.Style, p, backColor, flags);
        }
        #endregion
        #region Method: OWPara BuildParagraph(p, bMakeEditable, bShowBackTranslation)
        OWPara BuildParagraph(DParagraph p, bool bMakeEditable, bool bShowBackTranslation)
        {
            var writingSystem = (bShowBackTranslation) ?
                p.Translation.WritingSystemConsultant :
                p.Translation.WritingSystemVernacular;

            var backgroundColor = BackColor;

            // IsUserEditable here returns false for cross references and see-also footnotes,
            // resulting in the vernacular being shown for these on both sides of the
            // display (as we don't do a BT of these types of paragraphs.)
            var options = (bShowBackTranslation && p.IsUserEditable) ? 
                OWPara.Flags.ShowBackTranslation :
                OWPara.Flags.None;

            if (bMakeEditable && p.IsUserEditable)
            {
                options |= OWPara.Flags.IsEditable | OWPara.Flags.CanItalic;
                if (OurWordMain.TargetIsLocked)
                    options |= OWPara.Flags.IsLocked;

                backgroundColor = EditableBackgroundColor;
            }
            else
            {
                options |= OWPara.Flags.IsLocked;
            }

            return BuildParagraph(p, writingSystem, backgroundColor, options);
        }
        #endregion
        #region Method: void LoadLanguage(EColumn, group, bUseFront)
        void LoadLanguage(EContainer column, SynchronizedSection.ParagraphGroup group, bool bUseFront)
        {
            // Decide which paragraphs we are wanting to display in this row
            var vParagraphs = (bUseFront) ?
                group.FrontParagraphs :
                group.TargetParagraphs;

            // Each paragraph will have its own row
            foreach (var p in vParagraphs)
            {
                // We want a two-column row; the left will be the vernacular, the right the BT
                var row = new ERowOfColumns(2);
                column.Append(row);

                // Both the vernacular and the BT are editable in this view if they are the daughter;
                // but the Front translation we don't make editable.
                var bMakeEditable = !bUseFront;

                // Vernacular
                var pVernacular = BuildParagraph(p, bMakeEditable, false);
                row.GetColumn(0).Append(pVernacular);

                // Back Translation
                var pBackTranslation = BuildParagraph(p, bMakeEditable, true);
                row.GetColumn(1).Append(pBackTranslation);

                // Footnote Separator
                if (FootnotesEncountered != 1) 
                    continue;
                foreach (EColumn col in row.SubItems)
                {
                    col.Border = new EContainer.FootnoteSeparatorBorder(col,
                        c_nFootnoteSeparatorWidth);
                }
            }
        }
        #endregion
        #region Attr{g/s}: int FootnotesEncountered
        int FootnotesEncountered
        {
            get
            {
                Debug.Assert(-1 != m_cFootnotesEncountered);
                return m_cFootnotesEncountered;
            }
            set
            {
                m_cFootnotesEncountered = value;
            }
        }
        int m_cFootnotesEncountered = -1;
        #endregion
        #region Method: override void LoadData()
        public override void LoadData()
        {
            // Start with an empty window
            Clear();

            // Nothing more to do if we don't have a completely-defined project
            if (!DB.Project.HasDataToDisplay)
                return;

            // Synchronze the sections (break it down into corresponding paragraph groups)
            var synch = new SynchronizedSection(DB.FrontSection, DB.TargetSection);
            var vGroups = synch.AllGroups;

            // Create each group
            FootnotesEncountered = 0;
            foreach (var group in vGroups)
            {
                // Handle a picture if this is what the group is
                if (LoadPicture(group))
                    continue;

                // Create the top-level row, two columns (one for each language)
                var rowTop = new ERowOfColumns(2);
                Contents.Append(rowTop);

                // When FootnotesEncoutnered==1, we know to draw the footnote separator
                if (group.IsFootnoteGroup)
                    ++FootnotesEncountered;

                // Front Translation
                LoadLanguage(rowTop.GetColumn(0), group, true);

                // Target Translation
                LoadLanguage(rowTop.GetColumn(1), group, false);
            }

            // Tell the superclass to finish loading, which involves laying out the window 
            // with the data we've just put in, as doing the same for any secondary windows.
            base.LoadData();
        }
        #endregion

        // Notes -----------------------------------------------------------------------------
        #region OMethod: void SetupInsertNoteDropdown(ToolStripDropDownButton btnInsertNote)
        public override void SetupInsertNoteDropdown(ToolStripDropDownButton btnInsertNote)
        {
            foreach (ToolStripItem item in btnInsertNote.DropDownItems)
                item.Visible = true;
            btnInsertNote.ShowDropDownArrow = true;
        }
        #endregion
        #region OMethod: bool GetShouldDisplayNote(TranslatorNote, flags)
        public override bool GetShouldDisplayNote(TranslatorNote note, OWPara.Flags flags)
        {
            // Is the containing paragraph displaying the back translation?
            var bIsBT = ((flags & OWPara.Flags.ShowBackTranslation)
                == OWPara.Flags.ShowBackTranslation);

            // In the Target Translation, we display all back translation notes, whether
            // editable or not.
            if (note.IsTargetTranslationNote && bIsBT)
                return true;

            // In the Front Translation's Back Translation paragraph, we are only interested 
            // in notes that the consultant might want to see. 
            if (note.IsFrontTranslationNote && bIsBT)
            {
                if (note.Status == Role.Information)
                    return true;
            }

            return false;
        }
        #endregion
    }


}
