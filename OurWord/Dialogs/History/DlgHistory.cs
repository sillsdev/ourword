#region ***** DlgHistory.cs *****
/**********************************************************************************************
 * Project: OurWord!
 * File:    DlgHistory.cs
 * Author:  John Wimbish
 * Created: 26 Sept 2009
 * Purpose: For displaying the History of a section or book
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
using JWdb;
using JWdb.DataModel;

using OurWord.Layouts;
using OurWord.Edit;
#endregion
#endregion

namespace OurWord.Dialogs
{
    #region Class: DlgHistory
    public partial class DlgHistory : Form
    {
        WndHistory m_wndEntireBook;
        WndHistory m_wndThisSection;
        DSection m_section;

        #region Constructor(DSection)
        public DlgHistory(DSection section)
        {
            m_section = section;

            InitializeComponent();

            m_wndEntireBook = new WndHistory(section.Book.History, "BookHistory");
            m_tabBook.Controls.Add(m_wndEntireBook);

            m_wndThisSection = new WndHistory(section.History, "SectionHistory");
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
            m_wndEntireBook.SetSize(m_tabs.DisplayRectangle.Size);
            m_wndThisSection.SetSize(m_tabs.DisplayRectangle.Size);
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
                return m_History;
            }
        }
        TranslatorNote m_History;
        #endregion
        #region Attr{g}: string Title
        public string Title
        {
            get
            {
                return m_sTitle;
            }
        }
        string m_sTitle;
        #endregion
        #region Attr{g}: JWritingSystem WS
        JWritingSystem WS
        {
            get
            {
                return m_ws;
            }
        }
        JWritingSystem m_ws;
        #endregion

        // Scaffolding -----------------------------------------------------------------------
        #region Constructor(history, sWindowName)
        public WndHistory(TranslatorNote history, string sName)
            : base(sName, 1)
        {
            m_History = history;

            var section = history.Owner as DSection;
            if (section != null)
            {
                m_sTitle = section.ReferenceSpan.DisplayName + " - " + section.Title;
                m_ws = section.Translation.WritingSystemVernacular;
            }

            var book = history.Owner as DBook;
            if (book != null)
            {
                m_sTitle = book.DisplayName;
                m_ws = book.Translation.WritingSystemVernacular;
            }
        }
        #endregion
        const string c_sName = "History";
        #region SAttr{g/s}: string RegistryBackgroundColor - background color for this type of window
        static public string RegistryBackgroundColor
        {
            get
            {
                return OWWindow.GetRegistryBackgroundColor(c_sName, "LightPink");
            }
            set
            {
                OWWindow.SetRegistryBackgroundColor(c_sName, value);
            }
        }
        #endregion

        // Controls (set during LoadData) ----------------------------------------------------
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
        #region Attr{g}: ToolStripButton DeleteEventButton
        ToolStripButton DeleteEventButton
        {
            get
            {
                Debug.Assert(null != m_btnDeleteEvent);
                return m_btnDeleteEvent;
            }
        }
        ToolStripButton m_btnDeleteEvent;
        #endregion

        // Commands --------------------------------------------------------------------------
        #region Cmd: OnChangeStage
        public void OnChangeStage(object sender, EventArgs e)
        {
            ToolStripMenuItem item = sender as ToolStripMenuItem;
            if (null == item)
                return;

            DEventMessage Event = item.Tag as DEventMessage;
            if (null == Event)
                return;

            (new ChangeStage(this, Event, item)).Do();
        }
        #endregion
        #region Cmd: OnDateChanged
        public void OnDateChanged(Object sender, EventArgs e)
        {
            var ctrl = (sender as DateTimePicker);
            if (null == ctrl)
                return;

            DEventMessage Event = ctrl.Tag as DEventMessage;
            if (null == Event)
                return;

            (new ChangeEventDate(this, Event, ctrl)).Do();
        }
        #endregion
        #region Cmd: OnCursorTimerTick - updates button enabling
        protected override void OnCursorTimerTick()
        {
            if (string.IsNullOrEmpty(History.LastMessage.SimpleText))
                AddEventButton.Enabled = false;
            else
                AddEventButton.Enabled = true;
        }
        #endregion

        // View Building ---------------------------------------------------------------------
        #region OMethod: void LoadData()
        public override void LoadData()
        {
            // If this note has an empty message from before, delete it, because we want
            // to create a new one with today's date
            if (History.Messages.Count == 1 && string.IsNullOrEmpty(History.Messages[0].SimpleText))
                History.RemoveMessage(History.Messages[0]);

            // If this note had no messages, add one now
            if (!History.HasMessages)
            {
                var message = new DEventMessage(DateTime.UtcNow, DB.TeamSettings.Stages.Draft, "");
                History.Messages.Append(message);
            }

            // Builder helper class
            var build = new BuildToolTip(this, History);

            // Title
            build.LoadNoteTitle(null);

            // Message List
            bool bDarkBackground = true;
            foreach (DEventMessage message in History.Messages)
            {
                var eMessage = build.BuildMessage(message, bDarkBackground);
                Contents.Append(eMessage);
                bDarkBackground = !bDarkBackground;
            }

            // Toolbar (respond, delete)
            var eToolstrip = new EToolStrip(this);
            m_btnAddEvent = build.BuildAddEventControl();
            eToolstrip.ToolStrip.Items.Add(m_btnAddEvent);
            eToolstrip.ToolStrip.Items.Add(new ToolStripLabel("  "));
            m_btnDeleteEvent = build.BuildDeleteEventButton(History.LastMessage);
            eToolstrip.ToolStrip.Items.Add(m_btnDeleteEvent);
            Contents.Append(eToolstrip);

            // Tell the superclass to finish loading, which involves laying out the window 
            // with the data we've just put in, as doing the same for any secondary windows.
            base.LoadData();

            // Position at the top
            ScrollBarPosition = 0;
        }
        #endregion
    }
}
