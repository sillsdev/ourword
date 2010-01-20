#region ***** TranslationsPane.cs *****
/**********************************************************************************************
 * Project: OurWord!
 * File:    TranslationsPane.cs
 * Author:  John Wimbish
 * Created: 26 May 2009
 * Purpose: For display reference translations for the current context
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
using System.Timers;
using System.Windows.Forms;

using JWTools;
using OurWordData;
using OurWordData.DataModel;

using OurWord.Layouts;
using OurWord.Edit;
using OurWordData.DataModel.Runs;

#endregion
#endregion

namespace OurWord.SideWnd
{
    public partial class TranslationsPane : UserControl, ISideWnd
    {
        // ISideWnd --------------------------------------------------------------------------
        #region Method: void SetSize(Size sz)
        public void SetSize(Size sz)
        {
            this.Size = sz;

            // Position the Translations Window
            Window.Location = new Point(0, 0);
            Window.SetSize(sz.Width, sz.Height);
        }
        #endregion
        #region Attr{g}: OWWindow Window
        public OWWindow Window
        {
            get
            {
                return m_Window;
            }
        }
        TranslationsWnd m_Window;
        #endregion
        #region Method: void SetControlsEnabling()
        public void SetControlsEnabling()
        {
        }
        #endregion

        // Scaffolding -----------------------------------------------------------------------
        #region Constructor()
        public TranslationsPane()
        {
            InitializeComponent();

            // Create and add the Translations Window
            m_Window = new TranslationsWnd();
            Controls.Add(m_Window);
        }
        #endregion
    }

    public class TranslationsWnd : OWWindow
    {
        #region SAttr{g/s}: string RegistryBackgroundColor - background color setting for this type of window
        static public string RegistryBackgroundColor
        {
            get
            {
                return OWWindow.GetRegistryBackgroundColor(c_sName, "LightGray");
            }
            set
            {
                OWWindow.SetRegistryBackgroundColor(c_sName, value);
            }
        }
        #endregion

        // Scaffolding -----------------------------------------------------------------------
        const string c_sName = "RelatedLanguages";
        const int c_cColumnCount = 1;
        #region Constructor()
        public TranslationsWnd()
            : base(c_sName, c_cColumnCount)
        {
        }
        #endregion
        #region Attr{g}: override string WindowName
        public override string WindowName
        {
            get
            {
                return G.GetLoc_GeneralUI("TranslationsWindowName", "Translations");
            }
        }
        #endregion

        // Secondary Window Messaging --------------------------------------------------------
        #region Attr{g}: DBasicText BasicText - vernacular's DBasicText on which to build wnd contents
        DBasicText BasicText
        {
            get
            {
                return m_BasicText;
            }
        }
        DBasicText m_BasicText = null;
        #endregion

        #region Method: void OnSelectionChanged(DBasicText)
        public override void OnSelectionChanged(DBasicText dbt)
        {
            // Do nothing if we're in the same DBasicText (an optimization)
            if (dbt == BasicText)
                return;

            // Set to the new basic text
            m_BasicText = dbt;

            // Refresh the window contents
            BuildContents();
        }
        #endregion

        #region Method: DRun[] _GetSynchronizedSiblingText(...)
        DRun[] _GetSynchronizedSiblingText(DTranslation TSibling, DBasicText dbt)
        {
            // Get the book (load it if necessary)
            DBook BSibling = DB.Project.Nav.GetLoadedBook(TSibling,
                DB.Project.SFront.Book.BookAbbrev,
                G.CreateProgressIndicator());
            if (null == BSibling)
                return null;

            // Test to see if the sections match the front translation
            if (!BSibling.AllSectionsMatchFront)
                return null;

            // See if, for the section we are interested in, the paragraphs match up
            // with the front.
            DSection SSibling = DB.Project.Nav.GetSection(TSibling);
            if (null == SSibling)
                return null;
            if (!SSibling.AllParagraphsMatchFront)
                return null;

            // Find the corresponding paragraph to the paragraph which contains the basic text
            DParagraph pOwner = dbt.Paragraph;
            DParagraph pSibling = null;
            int iP = pOwner.Section.Paragraphs.FindObj(pOwner);
            if (-1 != iP)
            {
                pSibling = SSibling.Paragraphs[iP];
            }
            else
            {
                var VernacularFootnotes = pOwner.Section.AllFootnotes;
                for (int i = 0; i < VernacularFootnotes.Count; i++)
                {
                    if (VernacularFootnotes[i] == pOwner)
                        pSibling = SSibling.AllFootnotes[i];
                }
            }
            if (null == pSibling)
                return null;

            // Find the corresponding DBasicText
            if (pOwner.Runs.Count != pSibling.Runs.Count)
                return null;
            if (pOwner.StructureCodes != pSibling.StructureCodes)
                return null;
            int iDBT = pOwner.Runs.FindObj(dbt);
            if (iDBT == -1)
                return null;
            DRun rSibling = pSibling.Runs[iDBT] as DRun;

            // Create and return the vector
            DRun[] v = new DRun[1];
            v[0] = rSibling;
            return v;
        }
        #endregion

        #region Method: DRun[] _GetReferenceLanguageParagraphs(DTranslation t)
        DRun[] _GetReferenceLanguageParagraphs(DTranslation t)
        {
            // Get this paragraph's reference
            DParagraph pOwner = BasicText.Owner as DParagraph;

            // Get the reference book, loading it if necessary
            string sBookAbbrev = DB.Project.Nav.BookAbbrev;
            DBook book = DB.Project.Nav.GetLoadedBook(t, sBookAbbrev, G.CreateProgressIndicator());
            if (null == book)
                return null;

            // Get the section(s) that contain the reference we want
            DSection[] vSections = book.GetSectionsContaining(pOwner.ReferenceSpan);
            if (null == vSections || vSections.Length == 0)
                return null;

            // Collect the paragraphs
            ArrayList aParagraphs = new ArrayList();
            foreach (DSection section in vSections)
            {
                DParagraph[] vp = section.GetParagraphs(pOwner.ReferenceSpan);
                foreach (DParagraph p in vp)
                    aParagraphs.Add(p);
            }

            // Convert to a vector of runs
            int cRuns = 0;
            foreach (DParagraph p in aParagraphs)
            {
                foreach (DRun r in p.Runs)
                {
                    if (null != r as DBasicText || null != r as DVerse)
                        cRuns++;
                }
            }
            if (cRuns == 0)
                return null;

            int i = 0;
            DRun[] vRuns = new DRun[cRuns];
            foreach (DParagraph p in aParagraphs)
            {
                foreach (DRun r in p.Runs)
                {
                    if (r as DBasicText != null || r as DVerse != null)
                        vRuns[i++] = r;

                }
            }
            return vRuns;
        }
        #endregion
        #region Method: void BuildContents()
        public void BuildContents()
        {
            Clear();

            if (BasicText == null)
                return;

            foreach (DTranslation t in DB.Project.OtherTranslations)
            {
                // First attempt as a synchronized sibling, as this gives a much tighter
                // result. If that fails, then attempt as any old Reference Language
                DRun[] vRuns = _GetSynchronizedSiblingText(t, BasicText);
                if (null == vRuns)
                    vRuns = _GetReferenceLanguageParagraphs(t);
                if (null == vRuns)
                    continue;
                if (vRuns.Length == 0)
                    continue;

                // Determine the writing system from the translation
                JWritingSystem ws = t.WritingSystemVernacular;

                // The style for Ref Translation paragraphs
                JParagraphStyle PStyle = DB.StyleSheet.FindParagraphStyleOrNormal(
                    DStyleSheet.c_StyleReferenceTranslation);

                // Create and add a OWParagraph for the translation paragraph
                OWPara p = new OWPara(
                    ws, PStyle, vRuns, t.DisplayName, OWPara.Flags.None);
                Contents.Append(p);
            }

            LoadData();
        }
        #endregion

    }


}
