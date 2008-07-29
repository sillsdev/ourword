/**********************************************************************************************
 * Project: Our Word!
 * File:    Page_WritingSystems.cs
 * Author:  John Wimbish
 * Created: 21 Apr 2006
 * Purpose: Setup all of the Writing Systems for the Team Settings
 * Legal:   Copyright (c) 2004-08, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
#region Header: Using, etc.
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Resources;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;

using JWTools;
using JWdb;
using OurWord;
using OurWord.DataModel;
using OurWord.Dialogs;
using OurWord.View;
#endregion

namespace OurWord.Dialogs
{
    public partial class Page_WritingSystems : DlgPropertySheet
    {
        // Scaffolding -----------------------------------------------------------------------
        #region Constructor()
        public Page_WritingSystems(DialogProperties _ParentDlg)
            : base(_ParentDlg)
        {
            InitializeComponent();
        }
        #endregion
        #region Attr{g}: JWritingSystem WritingSystem
        JWritingSystem WritingSystem
        {
            get
            {
                return m_WritingSystem;
            }
        }
        JWritingSystem m_WritingSystem = null;
        #endregion

        // Misc Methods ----------------------------------------------------------------------
        #region Method: void PopulateList()
        void PopulateList()
        {
            m_listWritingSystems.Items.Clear();

            foreach (JWritingSystem ws in G.StyleSheet.WritingSystems)
            {
                m_listWritingSystems.Items.Add(ws.Name);
            }
        }
        #endregion
        #region Method: void SelectListItem(int i)
        private void SelectListItem(int i)
        {
            if (m_listWritingSystems.Items.Count > i)
                m_listWritingSystems.SelectedIndex = i;
        }
        #endregion

        // DlgPropertySheet overrides --------------------------------------------------------
        #region Method: void ShowHelp()
        public override void ShowHelp()
        {
            HelpSystem.ShowTopic(HelpSystem.Topic.kWritingSystems);
        }
        #endregion
        #region Attr{g}: string TabText
        public override string TabText
        {
            get
            {
                return Strings.PropDlgTab_WritingSystems;
            }
        }
        #endregion
        #region Method: override bool HarvestChanges()
        public override bool HarvestChanges()
        {
            m_ctrlAutoReplace.Harvest();
            WritingSystem.BuildAutoReplace();
            WritingSystem.DeclareDirty();
            return true;
        }
        #endregion

        // Property Grid ---------------------------------------------------------------------
        const string c_sPropName = "propName";
        const string c_sPropAbbrev = "propAbbrev";
        const string c_sPropPunctuation = "propPunctuation";
        const string c_sPropEndPunctuation = "propEndPunctuation";
        const string c_sPropKeyboard = "propKeyboard";
        #region Attr{g}: PropertyBag Bag - Defines the properties to display (including localizations)
        PropertyBag Bag
        {
            get
            {
                Debug.Assert(null != m_bag);
                return m_bag;
            }
        }
        PropertyBag m_bag;
        #endregion
        #region Method: void bag_GetValue(object sender, PropertySpecEventArgs e)
        void bag_GetValue(object sender, PropertySpecEventArgs e)
        {
            if (e.Property.ID == c_sPropName)
            {
                e.Value = WritingSystem.Name;
            }

            else if (e.Property.ID == c_sPropAbbrev)
            {
                e.Value = WritingSystem.Abbrev;
            }

            else if (e.Property.ID == c_sPropKeyboard)
            {
                e.Value = WritingSystem.KeyboardName;
            }

            else if (e.Property.ID == c_sPropPunctuation)
            {
                e.Value = WritingSystem.PunctuationChars;
            }

            else if (e.Property.ID == c_sPropEndPunctuation)
            {
                e.Value = WritingSystem.EndPunctuationChars;
            }
        }
        #endregion
        #region Method: void bag_SetValue(object sender, PropertySpecEventArgs e)
        void bag_SetValue(object sender, PropertySpecEventArgs e)
        {
            // Name
            if (e.Property.ID == c_sPropName)
            {
                // Nothing to do if they are the same
                if (WritingSystem.Name == (string)e.Value)
                    return;

                // We don't permit "Latin" to be changed, as we need it elsewhere
                // in the system
                if (WritingSystem.Name == DStyleSheet.c_Latin)
                    return;

                // We cannot accept the name change if it would result
                // in a duplicate
                foreach (JWritingSystem ws in G.StyleSheet.WritingSystems)
                {
                    if (ws.Name == (string)e.Value)
                        return;
                }

                // Set the new name
                WritingSystem.Name = (string)e.Value;

                // Update the list
                G.StyleSheet.WritingSystems.ForceSort();
                PopulateList();
                m_listWritingSystems.SelectedItem = WritingSystem.Name;
            }

            // Abbreviation
            if (e.Property.ID == c_sPropAbbrev)
            {
                WritingSystem.Abbrev = (string)e.Value;
            }

            // Keyboard Name
            if (e.Property.ID == c_sPropKeyboard)
                WritingSystem.KeyboardName = (string)e.Value;

            // Punctuation
            if (e.Property.ID == c_sPropPunctuation)
            {
                WritingSystem.PunctuationChars = (string)e.Value;
            }

            // End Punctuation
            if (e.Property.ID == c_sPropEndPunctuation)
            {
                WritingSystem.EndPunctuationChars = (string)e.Value;
            }
        }
        #endregion
        #region Method: void SetupPropertyGrid()
        void SetupPropertyGrid()
        {
            // Create the PropertyBag for this style
            m_bag = new PropertyBag();
            Bag.GetValue += new PropertySpecEventHandler(bag_GetValue);
            Bag.SetValue += new PropertySpecEventHandler(bag_SetValue);

            // Name
            PropertySpec ps = new PropertySpec(
                c_sPropName,
                "Name", 
                typeof(string),
                WritingSystem.Name,
                "Provide a unique name for this Writing System.", 
                "",
                "",
                null);
            if (WritingSystem.Name == DStyleSheet.c_Latin)
                ps.Attributes = new Attribute[] { ReadOnlyAttribute.Yes };
            ps.DontLocalizeCategory = true;
            Bag.Properties.Add(ps);

            // Abbreviation / ID
            ps = new PropertySpec(
                c_sPropAbbrev,
                "Abbreviation / ID",
                typeof(string),
                WritingSystem.Name,
                "A short abbreviation of the Writing System's name. This is used as an " +
                    "ID for the WeSay dictionary.",
                "",
                "",
                null);
            ps.DontLocalizeCategory = true;
            Bag.Properties.Add(ps);

            // Keyboard name
            ps = new PropertySpec(
                c_sPropKeyboard,
                "Keyboard Name",
                typeof(string),
                WritingSystem.Name,
                "The name of the keyboard to use when typing in this writing system " +
                    "(Windows IME, Keyman, etc.)",
                "",
                "",
                null);
            ps.DontLocalizeCategory = true;
            Bag.Properties.Add(ps);

            // Punctuation Charaters
            ps = new PropertySpec(
                c_sPropPunctuation,
                "Punctuation", 
                typeof(string),
                WritingSystem.Name,
                "A list of those letters that are punctuation in this writing system.", 
                JWritingSystem.c_sDefaultPunctuationChars,
                "",
                null);
            ps.DontLocalizeCategory = true;
            Bag.Properties.Add(ps);

            // End Punctuation Charaters
            ps = new PropertySpec(
                c_sPropEndPunctuation,
                "End Punctuation", 
                typeof(string),
                WritingSystem.Name,
                "A list of punctuation that can occur at the end of a sentence.", 
                JWritingSystem.c_sDefaultEndPunctuationChars,
                "",
                null);
            ps.DontLocalizeCategory = true;
            Bag.Properties.Add(ps);

            // Localize the bag
            LocDB.Localize(this, Bag);

            // Set the Property Grid to this PropertyBag
            m_PropGrid.SelectedObject = Bag;
        }
        #endregion

        // Command Handlers ------------------------------------------------------------------
        #region Cmd: cmdLoad
        private void cmdLoad(object sender, EventArgs e)
        {
            // Label text in the appropriate language
            LocDB.Localize(this, new Control[] { } );

            // The writing systems list
            PopulateList();

            // Select the first writing system in the list
            SelectListItem(0);
        }
        #endregion
        #region Cmd: cmdSelectedWSChanged
        private void cmdSelectedWSChanged(object sender, EventArgs e)
        {
            // Retrieve the writing system name from the list
            if (m_listWritingSystems.SelectedItems.Count != 1)
                return;
            string sName = (string)m_listWritingSystems.SelectedItem;

            // Retrieve the WS from the stylesheet
            m_WritingSystem = G.StyleSheet.FindWritingSystem(sName);
            Debug.Assert(null != WritingSystem);

            // Set up the Grid Control
            SetupPropertyGrid();

            // Set up the AutoReplace control
            m_ctrlAutoReplace.Initialize(m_WritingSystem.AutoReplaceSource,
                m_WritingSystem.AutoReplaceResult);

            // If we're doing Latin, we don't allow the name to be edited, so
            // that we can be sure we can find it elsewhere in the program as a
            // default writing system.
            m_btnRemove.Enabled = ((WritingSystem.Name == DStyleSheet.c_Latin) ? false : true);
        }
        #endregion
        #region Cmd: cmdAddBtnClicked
        private void cmdAddBtnClicked(object sender, EventArgs e)
        {
            string sName = "New Writing System";

            // Don't add if there is already one of this name in the list
            foreach (JWritingSystem ws in G.StyleSheet.WritingSystems)
            {
                if (ws.Name == sName)
                    return;
            }

            // Add the new one to the stylesheet
            G.StyleSheet.AddWritingSystem(sName);
            G.StyleSheet.WritingSystems.ForceSort();

            // Update the dialog to edit its settings
            int iPos = m_listWritingSystems.Items.Add(sName);
            SelectListItem(iPos);
        }
        #endregion
        #region Cmd: cmdRemoveBtnClicked
        private void cmdRemoveBtnClicked(object sender, EventArgs e)
        {
            if (WritingSystem.Name == DStyleSheet.c_Latin)
                return;

            // TODO: Need an AreYouSure message
            
            // Remove it from the StyleSheet
            G.StyleSheet.WritingSystems.Remove(WritingSystem);

            // Rebuild the list and select the first item in it
            PopulateList();
            SelectListItem(0);
        }
        #endregion
    }
}
