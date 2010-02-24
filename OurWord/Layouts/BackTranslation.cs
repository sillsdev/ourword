#region ***** BackTranslation.cs *****
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

using OurWordData.DataModel;
using OurWord.Edit;
using OurWord.Layouts;
using OurWordData;
using JWTools;
using OurWordData.DataModel.Annotations;

#endregion
#endregion

namespace OurWord.Layouts
{
    public class WndBackTranslation : WLayout
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

            // Load the paragraphs
            foreach (DParagraph p in DB.TargetSection.Paragraphs)
            {
                // Start the new row and add the left side (vernacular)
                EColumn colVernacular;
                EColumn colBackTranslation;
                ERowOfColumns row = WndDrafting.CreateRow(Contents,
                    out colVernacular, out colBackTranslation, false);
                row.SetPicture(GetPicture(p), true);

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
                var op = CreateFootnotePara(fn,
                    fn.Translation.WritingSystemVernacular,
                    BackColor,
                    OWPara.Flags.None);
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

        public override ENote.Flags GetNoteContext(TranslatorNote note, OWPara.Flags ParagraphFlags)
        {
            // In the back translation view, we only want to display notes in back translation 
            //  paragraphs; if this is a vernacular we don't display the note.
            // + editable (we expect dialog between, e.g., advisor and consultant)
            var bIsBackTranslation = (ParagraphFlags & OWPara.Flags.ShowBackTranslation) ==
                                     OWPara.Flags.ShowBackTranslation;
            if (bIsBackTranslation && note.Status.ThisUserCanAccess)
                return ENote.Flags.UserEditable;

            return ENote.Flags.None;
        }

    }


}
