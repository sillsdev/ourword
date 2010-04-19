#region ***** ReferenceTranslationsTip.cs *****
/**********************************************************************************************
 * Project: Our Word!
 * File:    ReferenceTranslationsTip.cs
 * Author:  John Wimbish
 * Created: 19 Apr 2010
 * Purpose: ToolTip window for showing reference translations
 * Legal:   Copyright (c) 2005-10, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using JWTools;
using OurWord.Edit;
using OurWordData.DataModel;
using OurWordData.Styles;

#endregion

namespace OurWord.ToolTips
{
    public partial class ReferenceTranslationsTip : ShadowedBalloon
    {
        // Attrs -----------------------------------------------------------------------------
        #region Attr{g}: DVerse Verse
        private DVerse Verse
        {
            get
            {
                Debug.Assert(null != m_Verse);
                return m_Verse;
            }
        }
        readonly DVerse m_Verse;
        #endregion
        #region Attr{g}: OWWindow ContentWindow
        OWWindow ContentWindow
        {
            get
            {
                return m_ContentWindow;
            }
        }
        readonly OWWindow m_ContentWindow;
        #endregion

        // Scaffolding -----------------------------------------------------------------------
        #region Constructor(DVerse)
        public ReferenceTranslationsTip(DVerse verse)
        {
            m_Verse = verse;

            // Need to have created our controls prior to InitComponent
            var definition = new OWWindow.WindowDefinition("ToolTip") 
            { 
                BackgroundColor = BackgroundColor,
                BorderStyle = BorderStyle.FixedSingle,
                HasScrollBar = true,
                OnLayoutFinished = OnContentWindowLayoutFinished
            };
            m_ContentWindow = new OWWindow(definition);

            InitializeComponent();

            m_panelClientArea.Controls.Add(ContentWindow);
        }
        #endregion

        // Content Window --------------------------------------------------------------------
        #region Cmd: OnContentWindowLayoutFinished
        bool s_IsDoingLayout;
        void OnContentWindowLayoutFinished(object sender, EventArgs e)
        {
            // Prevent recursion, as setting the Height causes OWWindow.DoLayout
            // to be called, which then calls this method over and over otherwise.
            if (s_IsDoingLayout)
                return;
            s_IsDoingLayout = true;

            // Get the Height required by the ContentWindow
            var nContentHeight = (int)ContentWindow.Contents.Height;

            // We don't want to allow this height to be too big. We define 'too big'
            // as 1/3 of the screen's height, realizing that the title bar will add
            // a bit more.
            var screen = Screen.FromPoint(Location);
            var nMaxHeight = screen.Bounds.Height / 3;
            nContentHeight = Math.Min(nContentHeight, nMaxHeight);

            // How much does the ContentWindow need to shrink/grow?
            var diff = ContentWindow.Height - nContentHeight;
            if (diff == 0)
                return;

            // Change our height. 
            // CAUTION: If observing strange  behavior, it is necessary to set the 
            // ShadowedBalloon's AutoSizeMode to "GrowAndShrink" instead of "GrowOnly",
            // and its AutoSize to false. It appears that when you use Visual Studio
            // to set AutoSizeMode, it automatically turns AutoSize to true. So these
            // asserts let us know if we change something someday.
            Debug.Assert(AutoSizeMode == AutoSizeMode.GrowAndShrink);
            Debug.Assert(!AutoSize);
            Height -= diff;

            s_IsDoingLayout = false;
        }
        #endregion
        #region Method: void BuildContentWindow()
        void BuildContentWindow()
        {
            ContentWindow.Clear();

            foreach (DTranslation translation in DB.Project.OtherTranslations)
                BuildTranslation(translation);

            ContentWindow.LoadData();
        }
        #endregion
        #region Method: void BuildTranslation(DTranslation)
        void BuildTranslation(DTranslation translation)
        {
            var book = DB.Project.Nav.GetLoadedBook(translation, 
                DB.Project.Nav.BookAbbrev, G.CreateProgressIndicator());
            if (null == book)
                return;

            var vRunsToDisplay = CollectDisplayRuns(book);

            var owp = new OWPara(translation.WritingSystemVernacular, 
                StyleSheet.ReferenceTranslation, vRunsToDisplay.ToArray(),
                translation.DisplayName, OWPara.Flags.None);

            ContentWindow.Contents.Append(owp);
        }
        #endregion
        #region Method: int GetChapterNumber()
        int GetChapterNumber()
        {
            var paragraph = Verse.Owner as DParagraph;
            if (null == paragraph)
                return -1;

            var nChapter = paragraph.ChapterI;
            foreach(DRun run in paragraph.Runs)
            {
                var chapter = run as DChapter;
                if (null != chapter)
                    nChapter = chapter.ChapterNo;

                var verse = run as DVerse;
                if (null == verse) 
                    continue;
                if (verse == Verse)
                    return nChapter;
            }

            Debug.Assert(false, "Should have encountered the verse within the loop");
            return nChapter;
        }
        #endregion
        #region Method: List<DRun> CollectDisplayRuns(DBook)
        List<DRun> CollectDisplayRuns(DBook book)
        {
            // Determine the chapter:verse(s) we want to display
            var nChapter = GetChapterNumber();
            var nVerseStart = Verse.VerseNo;
            var nVerseEnd = Verse.VerseNoFinal;

            // Collect the runs we want to display
            var vAllRuns = book.AllRuns;
            var vDisplayRuns = new List<DRun>();
            var bChapterFound = false;
            var bVerseFound = false;
            foreach (var run in vAllRuns)
            {
                // Find the target chapter
                var chapter = run as DChapter;
                if (null == chapter && !bChapterFound)
                    continue;
                if (null != chapter)
                {
                    if (chapter.ChapterNo == nChapter)
                        bChapterFound = true;
                    else
                        break;
                }

                // Find the target verse
                var verse = run as DVerse;
                if (null == verse && !bVerseFound)
                    continue;
                if (null != verse)
                {
                    if (verse.VerseNo == nVerseStart)
                        bVerseFound = true;
                    if (verse.VerseNo > nVerseEnd)
                        break;
                }

                // Collect the runs
                if (bVerseFound)
                    vDisplayRuns.Add(run);
            }

            return vDisplayRuns;
        }
        #endregion

        // ToolTip Overrides -----------------------------------------------------------------
        #region OMethod: void OnPopulateControls()
        protected override void OnPopulateControls()
        {
            // The Title tells what verse we're working with
            var sBase = Loc.GetString("ReferenceTranslationBase", 
                "How others translated verse {0}");
            m_Reference.Text = LocDB.Insert(sBase, new[] {Verse.Text});

            // Window contents
            BuildContentWindow();
        }
        #endregion
        #region Cmd: cmdExpandToNormalDialogWindow
        private void cmdExpandToNormalDialogWindow(object sender, EventArgs e)
        {
            PaintAsBalloon = false;
            Text = m_Reference.Text;

            m_NoteIcon.Hide();
            m_Reference.Hide();
            m_btnExpandWindow.Hide();
            m_btnClose.Hide();

            m_panelClientArea.Dock = DockStyle.Fill;
            OnLayoutControls();
        }
        #endregion
        #region OMethod: void OnLayoutControls()
        protected override void OnLayoutControls()
        {
            if (!PaintAsBalloon)
            {
                m_panelClientArea.Size = ClientSize;
            }
            else
            {
                const int nMargin = 10;
                m_panelClientArea.Height = ClientRectangle.Bottom - nMargin - m_panelClientArea.Top;
            }

            ContentWindow.SetSize(m_panelClientArea.Width, m_panelClientArea.Height);
            ContentWindow.Invalidate();
        }
        #endregion
    }
}
