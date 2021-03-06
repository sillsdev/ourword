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
using System.Drawing;
using OurWordData.DataModel;
using OurWord.Edit;
using JWTools;
using OurWordData.DataModel.Annotations;
using OurWordData.DataModel.Membership;

#endregion
#endregion

namespace OurWord.Layouts
{
    public class WndBackTranslation : WLayout
    {
        // Registry-Stored Settings ----------------------------------------------------------
        private const string c_sKeyCanEditTarget = "CanEditTarget";
        #region SAttr{g/s}: bool CanEditTarget
        static public bool CanEditTarget
        {
            get
            {
                return JW_Registry.GetValue(c_sName, c_sKeyCanEditTarget, false);
            }
            set
            {
                JW_Registry.SetValue(c_sName, c_sKeyCanEditTarget, value);
            }
        }
        #endregion

        // Scaffolding -----------------------------------------------------------------------
        public const string c_sName = "BT";
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
        #region OMethod: Color BackgroundColor
        protected override Color BackgroundColor
        {
            get
            {
                var sColorName = Users.Current.BackTranslationWindowBackground;
                return Color.FromName(sColorName);
            }
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
                var sBase = G.GetLoc_GeneralUI("BackTranslationReference", "{0}");

                var sTargetName = (null == DB.TargetTranslation) ?
                   G.GetLoc_GeneralUI("NoTargetDefined", "(no target defined)") :
                   DB.TargetTranslation.DisplayName.ToUpper();

                var s = LocDB.Insert(sBase, new[] { sTargetName });

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
                var row = WndDrafting.CreateRow(Contents,
                    out colVernacular, out colBackTranslation, false);
                row.SetPicture(GetPicture(p), true);

                // If we have no content, then we don't add the paragraphs.
                // (E.g., a picture with no caption.)
                if (p.SimpleText.Length == 0 && p.SimpleTextBT.Length == 0)
                    continue;

                // Add the vernacular paragraph to the left; we don't edit it except under
                // special circumstances
                var vernacularOptions = OWPara.Flags.None;
                if (p.IsUserEditable && CanEditTarget)
                {
                    vernacularOptions |= OWPara.Flags.IsEditable;
                    vernacularOptions |= OWPara.Flags.CanItalic;
                    if (OurWordMain.TargetIsLocked)
                        vernacularOptions |= OWPara.Flags.IsLocked;
                }
                var op =  new OWPara(
                    p.Translation.WritingSystemVernacular,
                    p.Style,
                    p,
                    BackColor,
                    vernacularOptions);
                colVernacular.Append(op);

                // For certain types of paragraphs, we just display them on the BT side, rather
                // than back-translating them. These paragraphs are ones we generated from the 
                // Front, rather than ones the translator will have edited. (E.g., cross
                // references.)
                var options = OWPara.Flags.None;
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
            var bFirstFootnote = true;
            var vFootnotes = DB.TargetSection.AllFootnotes;
            foreach (var fn in vFootnotes)
            {
                EColumn colVernacular;
                EColumn colBackTranslation;
                WndDrafting.CreateRow(Contents, out colVernacular, out colBackTranslation, bFirstFootnote);
                bFirstFootnote = false;

                // Add the vernacular paragraph to the left side
                var op = CreateFootnotePara(fn,
                    fn.Translation.WritingSystemVernacular,
                    BackColor,
                    OWPara.Flags.None);
                colVernacular.Append(op);

                // Options for the display paragraph
                var options = OWPara.Flags.None;
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
        #region OMethod: bool GetShouldDisplayNote(TranslatorNote, flags)
        public override bool GetShouldDisplayNote(TranslatorNote note, OWPara.Flags flags)
        {
            // Don't display if the vernacular rather than the backtranslation. If we didn't
            // do this, the TranslatorNote would show up in both places!
            var bIsBackTranslation = (flags & OWPara.Flags.ShowBackTranslation) ==
                OWPara.Flags.ShowBackTranslation;
            if (!bIsBackTranslation)
                return false;

            // For the BT side, then, we generallu only show notes that the user can 
            // actually edit. 
            //     We make an exception for Information notes, as we can't really know if it is
            // the advisor or the MTT who is using this view. The MTT will likely not want to
            // see Information notes, but the Advisor will.
            //     Likely no harm is done having Information always on here, because if a MTT
            // is creating a BT, the Information notes will not have been created yet, anyway;
            // as likely the advisor will come along and do that in a separate pass.
            if (note.Status.ThisUserCanAssignTo || note.Status == Role.Information)
                return true;

            return false;
        }
        #endregion
    }


}
