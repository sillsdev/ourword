﻿#region ***** DlgHistory.cs *****
/**********************************************************************************************
 * Project: OurWord!
 * File:    DlgHistory.cs
 * Author:  John Wimbish
 * Created: 26 Sept 2009
 * Purpose: For displaying the History of a section or book
 * Legal:   Copyright (c) 2004-09, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using OurWordData.DataModel;
using OurWord.Edit;
using OurWordData.DataModel.Annotations;
#endregion

namespace OurWord.Dialogs.History
{
    #region Class: DlgHistory
    public partial class DlgHistory : Form
    {
        readonly WndHistory m_wndEntireBook;
        readonly WndHistory m_wndThisSection;
        readonly DSection m_section;

        #region Constructor(DSection)
        public DlgHistory(DSection section)
        {
            m_section = section;

            m_wndEntireBook = new WndHistory(section.Book.History, "BookHistory");
            m_wndThisSection = new WndHistory(section.History, "SectionHistory");

            InitializeComponent();

            m_tabBook.Controls.Add(m_wndEntireBook);
            m_tabSection.Controls.Add(m_wndThisSection);
        }
        #endregion

        #region Cmd: cmdLoad
        private void cmdLoad(object sender, EventArgs e)
        {
            m_wndEntireBook.LoadData();
            m_wndThisSection.LoadData();

            // Translation Progress initialization
            TranslationProgress.Translation = m_section.Translation;

            // Initial setting of width, height to tab control
            cmdSizeChanged(null, null);
        }
        #endregion
        #region Cmd: cmdSizeChanged
        private void cmdSizeChanged(object sender, EventArgs e)
        {
            var clientSize = m_tabs.DisplayRectangle.Size;
            m_wndEntireBook.SetSize(clientSize);
            m_wndThisSection.SetSize(clientSize);
            m_ctrlTranslationProgress.Location = new Point(0,0);
            m_ctrlTranslationProgress.Size = clientSize;
        }
        #endregion

    }
    #endregion

    public class WndHistory : OWWindow
    {
        // Attrs -----------------------------------------------------------------------------
        #region Attr{g}: TranslatorNote History
        TranslatorNote History
        {
            get
            {
                return m_history;
            }
        }
        readonly TranslatorNote m_history;
        #endregion
        #region Attr{g}: string Title
        public string Title
        {
            get
            {
                return m_sTitle;
            }
        }
        readonly string m_sTitle;
        #endregion

        // Scaffolding -----------------------------------------------------------------------
        const string c_sName = "History";
        #region Constructor(history, sWindowName)
        public WndHistory(TranslatorNote history, string sName)
            : base(sName, 1)
        {
            m_history = history;
            DontEverDim = true;

            var section = history.Owner as DSection;
            if (section != null)
            {
                m_sTitle = section.ReferenceSpan.DisplayName + " - " + section.Title;
            }

            var book = history.Owner as DBook;
            if (book != null)
            {
                m_sTitle = book.DisplayName;
            }
        }
        #endregion
        #region OMethod: Color BackgroundColor
        protected override Color BackgroundColor
        {
            get
            {
                return Color.FromName("LightPink");
            }
        }
        #endregion

        // AddEvent button depends on whetherh or not we have text entered -------------------
        #region Attr{g}: ToolStripButton AddEventButton
        ToolStripButton AddEventButton
        {
            get
            {
                Debug.Assert(null != m_btnAddEvent);
                return m_btnAddEvent;
            }
        }
        ToolStripButton m_btnAddEvent;
        #endregion
        #region Cmd: OnCursorTimerTick - updates button enabling
        protected override void OnCursorTimerTick()
        {
            AddEventButton.Enabled = !string.IsNullOrEmpty(History.LastMessage.SimpleText);
        }

        #endregion

        // View Building ---------------------------------------------------------------------
        #region OMethod: void LoadData()
        public override void LoadData()
        {
            // If this annotation has an empty message from before, delete it, because we want
            // to create a new one with today's date
            if (History.Messages.Count == 1 && string.IsNullOrEmpty(History.Messages[0].SimpleText))
                History.RemoveMessage(History.Messages[0]);

            // If this annotation had no messages, add one now
            if (!History.HasMessages)
            {
                var message = new DEventMessage(DateTime.UtcNow, DB.TeamSettings.Stages.Draft, "");
                History.Messages.Append(message);
            }

            // Builder helper class
            var builder = new HistoryBuilder(this, History);

            // Title
            builder.LoadNoteTitle(null);

            // Message List
            builder.LoadInteractiveMessages();

            // Toolbar (respond, delete)
            builder.LoadToolStrip();
            m_btnAddEvent = builder.AddEventButton;

            // Tell the superclass to finish loading, which involves laying out the window 
            // with the data we've just put in, as doing the same for any secondary windows.
            base.LoadData();

            // Position at the top
            ScrollBarPosition = 0;
        }
        #endregion
    }
}
