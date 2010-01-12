/**********************************************************************************************
 * Project: Our Word!
 * File:    Page_AddWritingSystem.cs
 * Author:  John Wimbish
 * Created: 13 Feb 2009
 * Purpose: Permit the user to add/remove writing systems
 * Legal:   Copyright (c) 2004-09, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
#region Header: Using, etc.
using System;
using System.Diagnostics;
using JWTools;
using OurWordData.DataModel;
using OurWord.Edit;
#endregion

namespace OurWord.Dialogs
{
	public partial class Page_AddWritingSystem : DlgPropertySheet
	{
        // Settings Window -------------------------------------------------------------------
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
        #region Method: BuildLiterateSettings()
        void BuildLiterateSettings()
		{
            // Make sure the styles have been defined
            Information.InitStyleSheet();

			// Introduction
			LS.AddInformation("ws100", Information.PStyleHeading1, 
				"A. Introduction");
			LS.AddInformation("ws110", Information.PStyleNormal, 
				"We have struggled with the best way to handle writing systems for decades " +
				"now. I do not claim to have done any better than other software; in fact, " +
				"it is highly probable that what I've done here in _OurWord_ is quite " +
				"inadequate. Which likely means a revision is in the future. But in the " +
				"meantime.....here is what we have:");
			LS.AddInformation("ws120", Information.PStyleNormal, 
				"A writing system, loosely defined, is intended to be all of the information " +
				"about displaying, sorting, and keyboarding for a given alphabet. A language " +
				"might have multiple such systems; but in most cases a language has only one." +
				"Some examples of what writing systems describe:");
            LS.AddInformation("ws130", Information.PStyleList1,
				"That in Spanish, \"ll\" should be sorted as a single symbol");
            LS.AddInformation("ws140", Information.PStyleList1,
				"That in Tagalog, \"ng\", \"n\" and \"g\" are all separate symbols.");
            LS.AddInformation("ws150", Information.PStyleList1,
				"That in many Chinese languages, a \"SimSun\" font will display its fonts correctly");
            LS.AddInformation("ws160", Information.PStyleNormal, 
				"In designing _OurWord,_ I have had a goal of wanting the software to work " +
				"for many languages with as little setup as possible. Part of this strategy, and " +
				"where I depart from most other software, is that I permit a Writing System to " +
                "be defined as a stand-alone, rather than requiring one for each individual " +
				"language. Thus if all of the languages in a cluster can use a single Writing " +
				"System, it is not necessary define it over and over again each time a new " +
				"translation is begun.");
            LS.AddInformation("ws170", Information.PStyleNormal, 
				"Within the StyleSheet, for each individual style, you are able to define a " +
				"different font (font name, size, etc.) for each writing system. Thus a Book " +
				"Title in Chinese will likely have a different font than in English; and yet " +
				"for the paragraph style, you only need to define \"centered\" once.");
            LS.AddInformation("ws180", Information.PStyleNormal, 
				"I realize that as _OurWord_'s features expand I may be unable to keep using " +
                "this simple system. At that time, I will likely merge _OurWord_ writing " +
                "systems with the model that is supported by _WeSay._");

            LS.AddInformation("ws200", Information.PStyleHeading1,
                "B. Usage");
            LS.AddInformation("ws210", Information.PStyleNormal,
                "Each translation needs to know about two writing systems:");
            LS.AddInformation("ws220", Information.PStyleList1,
                "*Vernacular* represents which system you will use to enter Scripture Text " +
                "and other materials in the language");
            LS.AddInformation("ws230", Information.PStyleList1,
                "*Advisor* is the system that you will use for such items as the Back " +
                "Translation and notes for the consultant to read.");
            LS.AddInformation("ws240", Information.PStyleNormal,
                "Each translation settings page in this dialog has a tab, where you identify " +
                "the Vernacular and Advisor systems.");
            LS.AddInformation("ws250", Information.PStyleNormal,
                "For more about the types of information that you can configure in a writing " +
                "system, click on the _Latin Writing System_ to the left and examine the " +
                "settings that are available there. ");

            LS.AddInformation("ws300", Information.PStyleHeading1,
                "C. Re-Use");
            LS.AddInformation("ws310", Information.PStyleNormal,
                "As I mentioned above, it is normal in language software to set up a separate " +
                "writing system for each language. However, if you note that the various " +
                "languages in a cluster all use the same orthography, keyboardings, etc., it " +
                "is permissable to set up a single writing system and have them all use it. " +
                "_OurWord_ defaults to a \"Latin\" writing system, which will work for the " +
                "many languages that use an English orthography. Thus many languages can go " +
                "ahead and use _OurWord_ out of the box; others with more complicated " +
                "languages will need to do additional setup.");

            LS.AddInformation("ws400", Information.PStyleHeading1,
                "D. Windows Repeated Keyboard Name Bug");
            LS.AddInformation("ws410", Information.PStyleNormal,
                "(This applies to the page upon which you set up a keyboard.) Windows has " +
                "an unfortunate idea of what a program wants when it asks it for the list of " +
                "keyboards. Rather than give the actual name of the keyboard, it often gives " +
                "the name of a keyboard elsewhere in the list. As a result, you might see the " +
                "same keyboard name multiple times. This only happens when you have multiple " +
                "keyboards under a single language; thus the workaround is to move each " +
                "keyboard under its own separate language in your \"Regional and Language " +
                "Options\" control panel. That is, you probably need to pretend that your " +
                "keyboard is actually some other language. See " +
                "http://wesay.org/wiki/Windows_Repeated_Keyboard_Name_Bug for more details.");
        }
		#endregion

		// Scaffolding -----------------------------------------------------------------------
		#region Constructor(ParentDlg)
		public Page_AddWritingSystem(DialogProperties ParentDlg)
			: base(ParentDlg)
		{
			InitializeComponent();

            LS.DontAllowPropertyGrid = true;
            LS.Name = "WritingSystems";

            BuildLiterateSettings();
		}
		#endregion

		// DlgPropertySheet overrides --------------------------------------------------------
		#region SMethod: string ComputeID()
		public static string ComputeID()
		{
			return "idWritingSystems";
		}
		#endregion
		#region OAttr{g}: string ID
		public override string ID
		{
			get
			{
				return ComputeID();
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
				return "Add New";
			}
		}
		#endregion
		#region Method: override bool HarvestChanges()
		public override bool HarvestChanges()
		{
			return true;
		}
		#endregion

		// Command Handlers ------------------------------------------------------------------
		#region Cmd: cmdAddWritingSystem
		private void cmdAddWritingSystem(object sender, EventArgs e)
		{
            var sName = LocDB.GetValue(this, "NewWritingSystem", "New Writing System", null);

			// Add it to the list if it is not already there
			DB.StyleSheet.FindOrAddWritingSystem(sName);
			DB.StyleSheet.WritingSystems.ForceSort();

			// Recompute the pages, then go to its property page
            ParentDlg.InitNavigation(Page_WritingSystems.ComputeID(sName));
		}
		#endregion
	}
}
