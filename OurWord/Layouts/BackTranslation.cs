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
        public const string c_sRegDisplayInterlinearBT = "DisplayIBT";
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
        #region SAttr{g}: bool DisplayInterlinearBT
        static public bool DisplayInterlinearBT
        {
            get
            {
                if (s_nDisplayInterlinearBT == -1)
                {
                    s_nDisplayInterlinearBT = JW_Registry.GetValue(c_sName, c_sRegDisplayInterlinearBT, 0);
                }
                return (s_nDisplayInterlinearBT == 1) ? true : false;
            }
            set
            {
                s_nDisplayInterlinearBT = (value == true) ? 1 : 0;
                JW_Registry.SetValue(c_sName, c_sRegDisplayInterlinearBT, s_nDisplayInterlinearBT);
            }
        }
        static int s_nDisplayInterlinearBT = -1;
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

                string sTargetName = (null == G.TTranslation) ?
                   G.GetLoc_GeneralUI("NoTargetDefined", "(no target defined)") :
                   G.TTranslation.DisplayName.ToUpper();

                string s = LocDB.Insert(sBase, new string[] { sTargetName });

                return s;
            }
        }
        #endregion

        // Interlinear BT Option -------------------------------------------------------------
        void LoadData_IBT()
        {
            // Loop through the paragraphs
            foreach (DParagraph p in G.STarget.Paragraphs)
            {
                // Start the new row 
                EContainer container = StartNewRow(false);
                container.Bmp = GetPicture(p);

                // Add the IBT
                OWPara opIBT = new OWPara(
                    this,
                    LastRow.SubItems[0] as EContainer,
                    p.Translation.WritingSystemVernacular,
                    p.Style,
                    p,
                    BackColor,
                    OWPara.Flags.ShowIBT);
                AddParagraph(0, opIBT);

            }

            // Tell the superclass to finish loading, which involves laying out the window 
            // with the data we've just put in, as doing the same for any secondary windows.
            base.LoadData();
        }

        // Create the Window Contents from the data ------------------------------------------
        const int c_xMaxPictureWidth = 300;
        #region Method: Bitmap GetPicture(DParagraph p)
        Bitmap GetPicture(DParagraph p)
        {
            DPicture pict = p as DPicture;
            if (null != pict)
                return pict.GetBitmap(c_xMaxPictureWidth);
            return null;
        }
        #endregion

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
                // Start the new row and add the left side (vernacular)
                EContainer container = StartNewRow(false);
                container.Bmp = GetPicture(p);

                // If we have no content, then we don't add the paragraphs.
                // (E.g., a picture with no caption.)
                if (p.SimpleText.Length == 0 && p.SimpleTextBT.Length == 0)
                    continue;

                // Add the vernacular paragraph to the left; we don't edit it
                OWPara op = new OWPara(
                    this,
                    LastRow.SubItems[0] as EContainer,
                    p.Translation.WritingSystemVernacular,
                    p.Style,
                    p,
                    BackColor,
                    OWPara.Flags.None);
                AddParagraph(0, op);

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
                    this,
                    LastRow.SubItems[1] as EContainer,
                    p.Translation.WritingSystemConsultant,
                    p.Style,
                    p,
                    ((p.IsUserEditable) ? EditableBackgroundColor : BackColor),
                    options);
                AddParagraph(1, op);
            }

            // Load the footnotes
            bool bFirstFootnote = true;
            foreach (DFootnote fn in G.STarget.Footnotes)
            {
                StartNewRow(bFirstFootnote);
                bFirstFootnote = false;

                // Add the vernacular paragraph to the left side
                OWPara op = new OWPara(
                    this,
                    LastRow.SubItems[0] as EContainer,
                    fn.Translation.WritingSystemVernacular,
                    fn.Style,
                    fn,
                    BackColor,
                    OWPara.Flags.None);
                AddParagraph(0, op);

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
                op = new OWPara(
                    this,
                    LastRow.SubItems[1] as EContainer,
                    fn.Translation.WritingSystemConsultant,
                    fn.Style,
                    fn,
                    ((fn.IsUserEditable) ? EditableBackgroundColor : BackColor),
                    options);
                AddParagraph(1, op);
            }

            // Tell the superclass to finish loading, which involves laying out the window 
            // with the data we've just put in, as doing the same for any secondary windows.
            base.LoadData();
        }
        #endregion
    }


}
