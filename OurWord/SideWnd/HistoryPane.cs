#region ***** HistoryPane.cs *****
/**********************************************************************************************
 * Project: OurWord!
 * File:    HistoryPane.cs
 * Author:  John Wimbish
 * Created: 26 May 2009
 * Purpose: For display the History of a section or book
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

namespace OurWord.SideWnd
{
    public partial class HistoryPane : UserControl, ISideWnd
    {
        // ISideWnd --------------------------------------------------------------------------
        #region Method: void SetSize(Size sz)
        public void SetSize(Size sz)
        {
            this.Size = sz;

            // Extend the toolstrip across the entire size
            m_toolstripHistory.Location = new Point(0, 0);
            m_toolstripHistory.Width = sz.Width;

            // Position the History Window
            Window.Location = new Point(0, m_toolstripHistory.Height);
            Window.SetSize(sz.Width, sz.Height - m_toolstripHistory.Height);
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
        HistoryWnd m_Window;
        #endregion
        #region Method: void SetControlsEnabling()
        public void SetControlsEnabling()
        // The idea is that we insert notes when we are in the main window, but
        // not when we are in the notes pane. And we can only delete notes if
        // we are in the notes pane.
        {
            m_bDelete.Enabled = Window.Focused;
        }
        #endregion

        // Scaffolding -----------------------------------------------------------------------
        #region Constructor()
        public HistoryPane()
        {
            InitializeComponent();

            // Create and add the History OWWindow
            m_Window = new HistoryWnd(this);
            Controls.Add(m_Window);
        }
        #endregion

        // Commands --------------------------------------------------------------------------
        #region Cmd: cmdAddEvent
        private void cmdAddEvent(object sender, EventArgs e)
        {
            (new InsertHistoryAction(Window as HistoryWnd)).Do();
        }
        #endregion
        #region Cmd: cmdDeleteEvent
        private void cmdDeleteEvent(object sender, EventArgs e)
        {
            // Shouldn't get here if button is not enabled
            if (m_bDelete.Enabled == false)
                return;

            // Get the Event we wish to delete
            DEventMessage Event = (Window as HistoryWnd).GetSelectedEvent();
            if (Event == null)
                return;

            // Are you sure?
            string sContents = Event.EventDate.ToShortDateString() + " - " + 
                Event.Stage + " - " + Event.SimpleText.Trim();
            if (sContents.Length > 60)
                sContents = sContents.Substring(0, 60) + "...";
            if (!LocDB.Message("kDeleteEvent",
                "Are you sure you want to delete:\n\n\"{0}\"?",
                new string[] { sContents },
                LocDB.MessageTypes.YN))
            {
                return;
            }

            // Delete it
            (new DeleteHistoryAction(Window as HistoryWnd, Event)).Do();
        }
        #endregion
        #region Cmd: cmdLoad
        private void cmdLoad(object sender, EventArgs e)
        {
            LocDB.Localize(m_toolstripHistory);
        }
        #endregion
    }

    public class HistoryWnd : OWWindow
    {
        // Attrs -----------------------------------------------------------------------------
        #region Attr{g}: HistoryPane HistoryPane
        HistoryPane HistoryPane
        {
            get
            {
                Debug.Assert(null != m_HistoryPane);
                return m_HistoryPane;
            }
        }
        HistoryPane m_HistoryPane;
        #endregion
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

        // Scaffolding -----------------------------------------------------------------------
        const string c_sName = "History";
        const int c_cColumnCount = 1;
        #region Constructor(HistoryPane)
        public HistoryWnd(HistoryPane _HistoryPane)
            : base(c_sName, c_cColumnCount)
        {
            m_HistoryPane = _HistoryPane;
        }
        #endregion
        #region Attr{g}: override string WindowName
        public override string WindowName
        {
            get
            {
                return G.GetLoc_GeneralUI("HistoryWindowName", "History");
            }
        }
        #endregion

        // Commands --------------------------------------------------------------------------
        #region OMethod: void OnGotFocus(EventArgs)
        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
            HistoryPane.SetControlsEnabling();
        }
        #endregion
        #region OMethod: void OnLostFocus(EventArgs)
        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);
            HistoryPane.SetControlsEnabling();
        }
        #endregion
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

        // Methods ---------------------------------------------------------------------------
        #region Method: ECollapsableHeaderColumn GetCollapsableFromEvent(Event)
        public ECollapsableHeaderColumn GetCollapsableFromEvent(DEventMessage Event)
        {
            foreach (EContainer container in Contents)
            {
                var collapsable = container as ECollapsableHeaderColumn;
                if (null == collapsable)
                    continue;

                if (Event == collapsable.Tag as DEventMessage)
                    return collapsable;
            }
            return null;
        }
        #endregion
        #region Method: EToolStrip GetToolStripFromEvent(Event)
        ToolStrip GetToolStripFromEvent(DEventMessage Event)
        {
            var container = GetCollapsableFromEvent(Event);
            if (null == container)
                return null;

            var cHeader = container.Header as EColumn;
            if (null == cHeader || cHeader.SubItems.Length == 0)
                return null;

            var cToolStrip = cHeader.SubItems[0] as EToolStrip;
            if (null == cToolStrip)
                return null;
            return cToolStrip.ToolStrip;
        }
        #endregion
        #region Method: ToolStripDropDownButton GetStageDropDownFromEvent(DEvent)
        public ToolStripDropDownButton GetStageDropDownFromEvent(DEventMessage Event)
        {
            var ts = GetToolStripFromEvent(Event);
            if (null == ts)
                return null;

            foreach (ToolStripItem tsi in ts.Items)
            {
                if (tsi as ToolStripDropDownButton != null)
                    return tsi as ToolStripDropDownButton;
            }

            return null;
        }
        #endregion
        #region Method: DateTimePicker GetPickerFromEvent(Event)
        public DateTimePicker GetPickerFromEvent(DEventMessage Event)
        {
            var ts = GetToolStripFromEvent(Event);
            if (null == ts)
                return null;

            foreach (ToolStripItem tsi in ts.Items)
            {
                var cHost = tsi as ToolStripControlHost;
                if (cHost != null)
                    return cHost.Control as DateTimePicker;
            }

            return null;
        }
        #endregion

        #region Method: DEventMessage GetSelectedEvent()
        public DEventMessage GetSelectedEvent()
        {
            if (Selection == null)
                return null;

            DParagraph p = Selection.Paragraph.DataSource as DParagraph;
            Debug.Assert(null != p);

            var Event = p.Owner as DEventMessage;

            return Event;
        }
        #endregion

        // View Building ---------------------------------------------------------------------
        #region OMethod: void LoadData()
        public override void LoadData()
        {
            // Start with an empty window
            Clear();

            // Nothing more to do if we don't have a completely-defined project
            if (!DB.Project.HasDataToDisplay)
                return;

            // Retrieve the history we'll be showing
            DHistory history = DB.TargetSection.History;

            // Place them in the window
            foreach (DEventMessage e in history.Events)
                Contents.Append(BuildView(e));

            // Tell the superclass to finish loading, which involves laying out the window 
            // with the data we've just put in, as doing the same for any secondary windows.
            base.LoadData();

            // Make sure we're positioned at the top.
            ScrollBarPosition = 0;
        }
        #endregion
        #region Method: EContainer BuildView(DEventMessage e)
        EContainer BuildView(DEventMessage e)
        {
            Color cHeader = Color.LightYellow;

            // We'll put the event into a collapsable container, so that they can be collapsed 
            // by default and thus save space.

            // The header will be the Date and the Stage, changeable by the user
            var eHeader = new EColumn();
            EToolStrip ts = BuildToolStrip(e);
            eHeader.Append(ts);

            // Now that we have a header, we can create the collapseable container
            var eEventContainer = new ECollapsableHeaderColumn(eHeader);
            eEventContainer.Tag = e;

            // We'll place a line beneath it to separate items visually
            eEventContainer.Border = new EContainer.SquareBorder(eEventContainer);
            eEventContainer.Border.BorderPlacement = EContainer.BorderBase.BorderSides.Bottom;
            eEventContainer.Border.BorderColor = Color.DarkGray;
            eEventContainer.Border.BorderWidth = 2;
            eEventContainer.Border.Margin.Bottom = 5;
            eEventContainer.Border.Padding.Bottom = 5;

            // We need a box to hold the description, in order to get the rounded
            // corners effect
            int nRoundedCornerInset = 8;
            var eDescrContainer = new EColumn();
            eDescrContainer.Border = new EContainer.RoundedBorder(eDescrContainer, 12);
            eDescrContainer.Border.BorderColor = Color.DarkGray;
            eDescrContainer.Border.FillColor = Color.White;
            eDescrContainer.Border.Padding.Left = nRoundedCornerInset;
            eDescrContainer.Border.Padding.Right = nRoundedCornerInset;
            eEventContainer.Append(eDescrContainer);

            // Place the description into its main area
            OWPara pDescription = new OWPara(
                DB.TargetTranslation.WritingSystemConsultant,
                DB.StyleSheet.FindParagraphStyle(DStyleSheet.c_StyleAnnotationMessage),
                e,
                Color.White,
                OWPara.Flags.IsEditable);
            eDescrContainer.Append(pDescription);

            return eEventContainer;
        }
        #endregion

        #region Method: ToolStripMenuItem AddStageMenuItem(...)
        ToolStripMenuItem AddStageMenuItem(ToolStripDropDownButton menu, 
            string sMenuText,
            DEventMessage Event, 
            bool bChecked)
        {
            var item = new ToolStripMenuItem(sMenuText);
            item.Tag = Event;
            item.Click += new EventHandler(OnChangeStage);
            item.Checked = bChecked;
            menu.DropDownItems.Add(item);
            return item;
        }
        #endregion
        #region Method: EToolStrip BuildToolStrip(Event)
        EToolStrip BuildToolStrip(DEventMessage Event)
        {
            // Create the EToolStrip
            EToolStrip toolstrip = new EToolStrip(this);
            ToolStrip ts = toolstrip.ToolStrip; // Shorthand

            // Add the Date
            ts.Items.Add(BuildDatePicker(Event));

            // Some space in-between
            ts.Items.Add(new ToolStripLabel("   "));

            // Add the Stages control as a dropdown button
            bool bCurrentStageFound = false;
            ToolStripDropDownButton menuStage = new ToolStripDropDownButton(Event.Stage);
            foreach (TranslationStage stage in DB.TeamSettings.TranslationStages.TranslationStages)
            {
                AddStageMenuItem(menuStage, stage.Name, Event, (stage.Name == Event.Stage));
                if (stage.Name == Event.Stage)
                    bCurrentStageFound = true;
            }
            if (!bCurrentStageFound)
                AddStageMenuItem(menuStage, Event.Stage, Event, true);
            ts.Items.Add(menuStage);

            // If we didn't add anything, then dispose of it
            if (ts.Items.Count == 0)
            {
                ts.Dispose();
                return null;
            }

            return toolstrip;
        }
        #endregion
        #region Method: ToolStripItem BuildDatePicker(Event)
        ToolStripItem BuildDatePicker(DEventMessage Event)
        {
            // Create a date-time picker
            var ctrl = new DateTimePicker();
            ctrl.Format = DateTimePickerFormat.Custom;
            ctrl.CustomFormat = "yyyy-MM-dd";
            ctrl.Value = Event.EventDate;
            ctrl.Width = 100;
            ctrl.ValueChanged += new EventHandler(OnDateChanged);
            ctrl.Tag = Event;

            // Place it in a control  host
            return new ToolStripControlHost(ctrl);
        }
        #endregion
    }


}
