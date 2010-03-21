#region ***** Naturalness.cs *****
/**********************************************************************************************
 * Project: Our Word!
 * File:    Layouts\Naturalness.cs
 * Author:  John Wimbish
 * Created: 16 Aug 2007
 * Purpose: Manages the Naturalness Check view.
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

#endregion
#endregion

namespace OurWord.Layouts
{
    class WndNaturalness : WLayout
    {
        // Registry-Stored Settings ----------------------------------------------------------
        public const string c_sName = "Naturalness";
        const string c_sRegNameLineNumbersColor = "LineNumbersColor";
        const string c_RegNameSuppressVerseNumbers = "SuppressVerseNos";
        const string c_RegNameShowLineNumbers = "ShowLineNumbers";
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
        #region SAttr{g/s}: string LineNumbersColor - color for the line numbers
        static public string LineNumbersColor
        {
            get
            {
                return JW_Registry.GetValue(c_sName, c_sRegNameLineNumbersColor, "DarkGray");
            }
            set
            {
                JW_Registry.SetValue(c_sName, c_sRegNameLineNumbersColor, value); 
            }
        }
        #endregion
        #region SAttr{g/s}: bool SupressVerseNumbers
        public static bool SupressVerseNumbers
        {
            get
            {
                if (-1 == s_nSupressVerseNumbers)
                {
                    s_nSupressVerseNumbers = JW_Registry.GetValue(c_sName,
                        c_RegNameSuppressVerseNumbers, 0);
                }
                return (s_nSupressVerseNumbers == 1) ? true : false;
            }
            set
            {
                s_nSupressVerseNumbers = (value == true) ? 1 : 0;
                JW_Registry.SetValue(c_sName, c_RegNameSuppressVerseNumbers, s_nSupressVerseNumbers);
            }
        }
        static int s_nSupressVerseNumbers = -1;
        #endregion
        #region SAttr{g/s}: bool ShowLineNumbers
        public static bool ShowLineNumbers
        {
            get
            {
                if (-1 == s_nShowLineNumbers)
                {
                    s_nShowLineNumbers = JW_Registry.GetValue(c_sName,
                        c_RegNameShowLineNumbers, 0);
                }
                return (s_nShowLineNumbers == 1) ? true : false;
            }
            set
            {
                s_nShowLineNumbers = (value == true) ? 1 : 0;
                JW_Registry.SetValue(c_sName, c_RegNameShowLineNumbers, s_nShowLineNumbers);
            }
        }
        static int s_nShowLineNumbers = -1;
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

            // Background color for the window
            BackColor = Color.Honeydew;

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
                return G.GetLoc_GeneralUI("NaturalnessCheckWindowName", "Naturalness Check");
            }
        }
        #endregion
        #region Attr{g}: override string LanguageInfo
        public override string LanguageInfo
        {
            get
            {
                if (!DB.IsValidProject)
                    return "";
                if (null == DB.Project.TargetTranslation)
                    return "";

                return DB.Project.TargetTranslation.DisplayName.ToUpper();
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
            if (!DB.Project.HasDataToDisplay)
                return;

            // Save calculating this over and over
            bool bAllParagraphsMatchFront = DB.TargetSection.AllParagraphsMatchFront;

            // Load the paragraphs
            for (int ip = 0; ip < DB.TargetSection.Paragraphs.Count; ip++)
            {
                DParagraph p = DB.TargetSection.Paragraphs[ip] as DParagraph;

                // If we have no content, then we don't add the paragraphs.
                // (E.g., a picture with no caption.)
                if (p.SimpleText.Length == 0 && p.SimpleTextBT.Length == 0)
                    continue;

                // Add the vernacular paragraph
                OWPara.Flags options = OWPara.Flags.None;
                if (p.IsUserEditable)
                    options |= OWPara.Flags.IsEditable;
                if (WndNaturalness.SupressVerseNumbers)
                    options |= OWPara.Flags.SuppressVerseNumbers;
                if (WndNaturalness.ShowLineNumbers)
                    options |= OWPara.Flags.ShowLineNumbers;
                if (OurWordMain.TargetIsLocked)
                    options |= OWPara.Flags.IsLocked;

                // Italics?
                if (bAllParagraphsMatchFront)
                {
                    DParagraph pFront = DB.FrontSection.Paragraphs[ip] as DParagraph;
                    if (pFront.HasItalicsToggled)
                        options |= OWPara.Flags.CanItalic;
                }
                else
                    options |= OWPara.Flags.CanItalic;

                // Create and add the paragraph
                OWPara op = new OWPara(
                    p.Translation.WritingSystemVernacular,
                    p.Style,
                    p,
                    ((OurWordMain.TargetIsLocked) ? BackColor : EditableBackgroundColor),
                    options);
                op.SetPicture(GetPicture(p), true);
                Contents.Append(op);
            }

            // Load the footnotes
            bool bFirstFootnote = true;
            var TargetFootnotes = DB.TargetSection.AllFootnotes;
            var FrontFootnotes = DB.FrontSection.AllFootnotes;
            for (int iFn = 0; iFn < TargetFootnotes.Count; iFn++)
            {
                DFootnote fn = TargetFootnotes[iFn] as DFootnote;

                OWPara.Flags options = OWPara.Flags.None;
                if (fn.IsUserEditable)
                    options |= OWPara.Flags.IsEditable;
                if (WndNaturalness.ShowLineNumbers)
                    options |= OWPara.Flags.ShowLineNumbers;
                if (OurWordMain.TargetIsLocked)
                    options |= OWPara.Flags.IsLocked;

                // Italics?
                if (bAllParagraphsMatchFront)
                {
                    if (FrontFootnotes[iFn].HasItalicsToggled)
                        options |= OWPara.Flags.CanItalic;
                }
                else
                    options |= OWPara.Flags.CanItalic;

                OWPara op = CreateFootnotePara(fn,
                    fn.Translation.WritingSystemVernacular,
                    ((OurWordMain.TargetIsLocked) ? BackColor : EditableBackgroundColor),
                    options);

                if (bFirstFootnote)
                {
                    op.Border = new EContainer.FootnoteSeparatorBorder(op, WndDrafting.c_nFootnoteSeparatorWidth);
                    bFirstFootnote = false;
                }

                Contents.Append(op);
            }

            // Tell the superclass to finish loading, which involves laying out the window 
            // with the data we've just put in, as doing the same for any secondary windows.
            base.LoadData();

            // Override the Line Numbers color by this window's setting
            Color clr = Color.FromName(LineNumbersColor);
            base.LineNumberAttrs.Brush = new SolidBrush(clr);
        }
        #endregion
        #region OMethod: bool GetShouldDisplayNote(TranslatorNote, flags)
        public override bool GetShouldDisplayNote(TranslatorNote note, OWPara.Flags flags)
        {
            // Only display TargetTranslation notes (since the Target is all we're showing)
            if (!note.IsTargetTranslationNote)
                return false;

            // Only notes this user has permission for)
            // + Editable (user action desired)
            if (note.Status.ThisUserCanAssignTo)
                return true;

            return false;
        }
        #endregion
    }
}
