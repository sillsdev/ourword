/**********************************************************************************************
 * Project: Our Word!
 * File:    Page_WritingSystems.cs
 * Author:  John Wimbish
 * Created: 21 Apr 2006
 * Purpose: Setup all of the Writing Systems for the Team Settings
 * Legal:   Copyright (c) 2004-09, John S. Wimbish. All Rights Reserved.  
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

using Palaso.UI.WindowsForms.Keyboarding;
using JWTools;
using JWdb;
using OurWord;
using JWdb.DataModel;
using OurWord.Dialogs;
using OurWord.Layouts;
using OurWord.Edit;
#endregion

namespace OurWord.Dialogs
{
    public partial class Page_WritingSystems : DlgPropertySheet
    {
		// Hyphenation -----------------------------------------------------------------------
		#region Attr{g}: LiterateSettingsWnd LS
		LiterateSettingsWnd LS
		{
			get
			{
				Debug.Assert(null != m_LiterateSettingsWnd);
				return m_LiterateSettingsWnd;
			}
		}
		#endregion
		YesNoSetting m_TurnOnHyphenation;
		EditTextSetting m_HyphenationCVPattern;
		EditTextSetting m_HyphenationConsonants;
		IntSetting m_HyphenationMinSplitSize;
		#region Method: BuildHyphenationWindow()
		void BuildHyphenationWindow()
		{
			// Make sure the styles have been defined
			Information.InitStyleSheet();

			// Introduction
			LS.AddInformation("ah100", Information.PStyleNormal, 
				"_OurWord_ offers a quick-and-dirty means of hyphenation, which you may find useful if " +
				"your language has long words. It is probably not sufficient if your language requires " +
				"complex hyphenation rules, but perhaps it will be enough for drafting purposes.");

			// Section: Setup
			LS.AddInformation("ah200", Information.PStyleHeading1, "A. Setup");

			LS.AddInformation("ah210", Information.PStyleNormal, 
				"*1. You must first turn on the feature.* By default it is turned off because automated " +
				"hyphenation has a high potential to comfuse new computer users.");
			m_TurnOnHyphenation = LS.AddYesNo( 
				"ahTurnOn", 
				"Turn on Automatic Hyphenation?", 
				"If Yes, long words will be automatically hyphenated during editing. Hyphens " +
                    "are not physically placed in the data; they merely appear on screen.",
				WritingSystem.UseAutomatedHyphenation);

			LS.AddInformation("ah220", Information.PStyleNormal, 
				"*2. At the heart of hyphenation you must specify a CV pattern,* in which you indicate " +
				"where the hyphen is permitted.");
			LS.AddInformation("ah221", Information.PStyleList1, 
				"\"VC-C\" works for many English words, such as \"bog-gle\", \"com-mit\", and \"" +
				"air-plane.\" (Unfortunately it fails for words such as \"pa-per,\" thus we recognize " +
				"that _OurWord_ will not completely cover all of the hyphenation rules of many languages.)");
			LS.AddInformation("ah222", Information.PStyleList1,
				"\"V-C\" works for Huichol words, where closed syllables do not exist, thus " +
				"\"p�-ca-heu-x�-ca-cai-ri.\"");
			m_HyphenationCVPattern = LS.AddEditText("ahCV",
				"CV Pattern:",
				"Indicate where the hyphen will go, e.g., V-C, or VC-CV.",
				WritingSystem.HyphenationCVPattern);

			LS.AddInformation("ah230", Information.PStyleNormal, 
				"*3. You need to tell _OurWord_ what your consonants are.* Once we know the consonants, " +
				"and because we already know the punctuation, we can infer that everything else is a " +
				"vowel; and thus we can check against the CV pattern you supplied.");
			m_HyphenationConsonants = LS.AddEditText("ahCon",
				"Consonants",
				"List the consonants for this writing systems. (Eveything that isn't a " +
					"consonant or punctuation is considered to be a vowel.)",
				WritingSystem.Consonants);

			LS.AddInformation("ah240", Information.PStyleNormal, 
				"*4. Finally, you can specify a minimum size,* so that _OurWord_ will not break a word " +
				"down into parts that are too small. Thus a minimum size of three letters would " +
				"prevent \"ap-ple\", because \"ap\" would be too small.");
			m_HyphenationMinSplitSize = LS.AddInt("ahMinSize",
				"Minimum Split Size:",
				"Hyphenation will not result in a word-part that is smaller than this value.",
				WritingSystem.MinHyphenSplit, 1);

			LS.AddInformation("ah250", Information.PStyleNormal, 
				"That should cover it. You can now go to the main drafting screen and see how words " +
				"are hyphenating, and come back here and tweak until you are happy with the result.");

			// Section: Behavior
			LS.AddInformation("ah300", Information.PStyleHeading1, "B. Behavior");
			LS.AddInformation("ah301", Information.PStyleNormal, 
				"If turned on, hyphens are automatically created, and then automatically update as " +
				"you type or otherwise edit the text. They are never inserted into the data, and thus " +
				"never are stored in the file. When editing, the hyphens appear outside of the white " +
				"background, indicating that the user cannot type on top of them. Hopefully the user " +
				"will not be tempted to type in hyphens, because doing so would cause them to be part " +
				"of the data. If you ever see a hyphen with a white background, it was inserted by the " +
				"user, not by _OurWord._");
			LS.AddInformation("ah302", Information.PStyleNormal, 
				"If a single word becomes too long to fit in a line, and if hyphenation is turned " +
				"off (or if the hyphen rule does not apply), then _OurWord_ will arbitrarily display a " +
				"hyphen in order to keep the line from overflowing from view on the screen. Again, " +
				"these hyphens are not inserted into the data, they merely appear on the screen.");

			// Section: Future
			LS.AddInformation("ah400", Information.PStyleHeading1, "C. Future");
			LS.AddInformation("ah401", Information.PStyleNormal, 
				"Depending on expressed user interest, future versions may incorporate a more " +
				"rigorous approach to the problem. Possibilities include permitting multiple " +
				"CV patterns, regular expressions, or use of the very-rigorous International " +
				"Components for Unicode (ICU) library. So please build your case for further " +
				"work in this area.");
			LS.AddInformation("ah402", Information.PStyleNormal, 
				"Secondly, automatic hyphenation is not yet implemented in the Print routine. " +
				"I know, I know......stay tuned.....and the more voices that request it, the " +
				"higher this stuff moves up on my To Do list.");
		}
		#endregion

		// Scaffolding -----------------------------------------------------------------------
        #region Constructor(ParentDlg, JWritingSystem)
        public Page_WritingSystems(DialogProperties _ParentDlg, JWritingSystem ws)
            : base(_ParentDlg)
        {
            InitializeComponent();

			m_WritingSystem = ws;

			// Setup the Hyphenation LiterateSettings control
			m_LiterateSettingsWnd.Name = "AutomaticHyphenation";
			BuildHyphenationWindow();
        }
        #endregion
        #region Attr{g}: JWritingSystem WritingSystem
        JWritingSystem WritingSystem
        {
            get
            {
				Debug.Assert(null != m_WritingSystem);
                return m_WritingSystem;
            }
        }
        JWritingSystem m_WritingSystem = null;
        #endregion

		// DlgPropertySheet overrides --------------------------------------------------------
		#region SMethod: string ComputeID(string sWritingSystemName)
		public static string ComputeID(string sWritingSystemName)
		{
			return "idWS_" + sWritingSystemName;
		}
		#endregion
		#region OAttr{g}: string ID
		public override string ID
		{
			get
			{
				return ComputeID(WritingSystem.Name);
			}
		}
		#endregion
		#region Method: void ShowHelp()
        public override void ShowHelp()
        {
            HelpSystem.ShowTopic(HelpSystem.Topic.kWritingSystems);
        }
        #endregion
        #region Attr{g}: string Title
        public override string Title
        {
            get
            {
                return WritingSystem.Name;
            }
        }
        #endregion
        #region Method: override bool HarvestChanges()
        public override bool HarvestChanges()
        {
            m_ctrlAutoReplace.Harvest();
            WritingSystem.BuildAutoReplace();
            WritingSystem.DeclareDirty();

			// Automated Hyphenation
			WritingSystem.UseAutomatedHyphenation = m_TurnOnHyphenation.Value;
			WritingSystem.HyphenationCVPattern = m_HyphenationCVPattern.Value;
			WritingSystem.Consonants = m_HyphenationConsonants.Value;
			try
			{
				WritingSystem.MinHyphenSplit = Convert.ToInt16(m_HyphenationMinSplitSize.Value);
			}
			catch (Exception)
			{
			}
            return true;
        }
        #endregion

        // Property Grid ---------------------------------------------------------------------
        #region Property Grid Constants
        const string c_sPropName = "propName";
        const string c_sPropAbbrev = "propAbbrev";
        const string c_sPropPunctuation = "propPunctuation";
        const string c_sPropEndPunctuation = "propEndPunctuation";
        const string c_sPropKeyboard = "propKeyboard";
        #endregion
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
            // General
            #region Name
            if (e.Property.ID == c_sPropName)
            {
                e.Value = WritingSystem.Name;
            }
            #endregion
            #region Abbrev
            if (e.Property.ID == c_sPropAbbrev)
            {
                e.Value = WritingSystem.Abbrev;
            }
            #endregion
            #region Keyboard
            if (e.Property.ID == c_sPropKeyboard)
            {
                e.Value = WritingSystem.KeyboardName;
            }
            #endregion
            #region Punctuation
            if (e.Property.ID == c_sPropPunctuation)
            {
                e.Value = WritingSystem.PunctuationChars;
            }
            #endregion
            #region EndPunctuation
            if (e.Property.ID == c_sPropEndPunctuation)
            {
                e.Value = WritingSystem.EndPunctuationChars;
            }
            #endregion
        }
        #endregion
        #region Method: void bag_SetValue(object sender, PropertySpecEventArgs e)
        void bag_SetValue(object sender, PropertySpecEventArgs e)
        {
            // General
            #region Name
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
                foreach (JWritingSystem ws in DB.StyleSheet.WritingSystems)
                {
                    if (ws.Name == (string)e.Value)
                        return;
                }

                // Set the new name
                WritingSystem.Name = (string)e.Value;

                // Update the list
                DB.StyleSheet.WritingSystems.ForceSort();
                ParentDlg.UpdateNavigationControls();
//                PopulateList();
//                m_listWritingSystems.SelectedItem = WritingSystem.Name;
            }
            #endregion
            #region Abbreviation
            if (e.Property.ID == c_sPropAbbrev)
            {
                WritingSystem.Abbrev = (string)e.Value;
            }
            #endregion
            #region Keyboard Name
            if (e.Property.ID == c_sPropKeyboard)
            {
                WritingSystem.KeyboardName = (string)e.Value;
            }
            #endregion
            #region Punctuation
            if (e.Property.ID == c_sPropPunctuation)
            {
                WritingSystem.PunctuationChars = (string)e.Value;
            }
            #endregion
            #region End Punctuation
            if (e.Property.ID == c_sPropEndPunctuation)
            {
                WritingSystem.EndPunctuationChars = (string)e.Value;
            }
            #endregion
        }
        #endregion
        #region Method: void SetupPropertyGrid()
        void SetupPropertyGrid()
        {
            // Create the PropertyBag for this style
            m_bag = new PropertyBag();
            Bag.GetValue += new PropertySpecEventHandler(bag_GetValue);
            Bag.SetValue += new PropertySpecEventHandler(bag_SetValue);

            // General Properties
            #region string: Name
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
            #endregion
            #region string: Abbreviation / ID
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
            #endregion
            #region List<>: Keyboard name: choose from a list
            List<KeyboardController.KeyboardDescriptor> v =
               KeyboardController.GetAvailableKeyboards(KeyboardController.Engines.All);
            string[] vNames = new string[v.Count + 1];
            for (int i = 0; i < v.Count; i++)
                vNames[i] = v[i].Name;
            vNames[v.Count] = ""; // Provide for an empty one in case we don't want to specify one.
            ps = new PropertySpec(
                c_sPropKeyboard,
                "Keyboard Name",
                WritingSystem.Name,
                "The name of the keyboard to use when typing in this writing system " +
                    "(Windows IME, Keyman, etc.) Use the full name, not the abbreviation.",
                vNames,
                "");
            ps.DontLocalizeEnums = true;
            ps.DontLocalizeCategory = true;
            Bag.Properties.Add(ps);
            #endregion
            #region string: Punctuation Charaters
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
            #endregion
            #region string: End Punctuation Charaters
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
            #endregion

            // Localize the bag
            LocDB.Localize(this, Bag);

            // Set the Property Grid to this PropertyBag
            m_PropGrid.SelectedObject = Bag;
        }
        #endregion

		// Loading ---------------------------------------------------------------------------
        #region Cmd: cmdLoad
        private void cmdLoad(object sender, EventArgs e)
        {
            // Label text in the appropriate language
            LocDB.Localize(this, new Control[] { } );

			// Set up the Grid Control
			SetupPropertyGrid();

			// Set up the AutoReplace control
			m_ctrlAutoReplace.Initialize(m_WritingSystem.AutoReplaceSource,
				m_WritingSystem.AutoReplaceResult);
		}
        #endregion

		// Command Handlers ------------------------------------------------------------------
        #region Cmd: cmdRemoveBtnClicked
        private void cmdRemoveBtnClicked(object sender, EventArgs e)
			// We prevent deletions of WS's that are in use, so as to not mess up life
			// for the current project. Of course, loading a new project, we need
			// to create a WS if it needs it and it isn't in the Team Settings.
        {
			// We can't remove the writing system if:
			// - It is Latin, which is our default WS we use when others aren't there
			// - It is in use currently by the project.
			// So display an error message to the effect. (We can't disable the button
			// for this, because Microsoft does not permit a tooltip for disabled
			// buttons.)
			if (WritingSystem.Name == DStyleSheet.c_Latin)
			{
				Messages.UnableToRemoveLatin();
				return;
			}
			foreach (DTranslation t in DB.Project.AllTranslations)
			{
				if (t.WritingSystemConsultant == WritingSystem ||
					t.WritingSystemVernacular == WritingSystem)
				{
					Messages.UnableToRemoveWritingSystem(WritingSystem.Name);
					return;
				}
			}

            // We need an AreYouSure message, since we don't Undo it
			if (!Messages.VerifyRemoveWritingSystem(WritingSystem.Name))
				return;
            
            // Remove it from the StyleSheet
			DB.StyleSheet.RemoveWritingSystem(WritingSystem);

			// Update the nav tree, and navigate to the Add page
            ParentDlg.InitNavigation(Page_AddWritingSystem.ComputeID());
        }
        #endregion
	}
}
