/**********************************************************************************************
 * Project: Our Word!
 * File:    DNote.cs
 * Author:  John Wimbish
 * Created: 26 Jan 2004
 * Purpose: Handles a translator's (or consultant's) note in Scripture. 
 * Legal:   Copyright (c) 2005-09, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
#region Using
using System;
using System.Diagnostics;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Windows.Forms;
using System.IO;

using JWTools;
using JWdb;
using OurWord.View;
#endregion

namespace OurWord.DataModel 
{
	#region Class: DNoteDef - Information about each type of DNote
	public class DNoteDef
	{
		// Content Attrs ---------------------------------------------------------------------
		#region Attr{g}: DNote.Types NoteType
		public DNote.Types NoteType
		{
			get
			{
				Debug.Assert(m_NoteType != DNote.Types.kUnknown);
				return m_NoteType;
			}
		}
		DNote.Types m_NoteType = DNote.Types.kUnknown;
		#endregion
        #region Attr{g}: string EnglishName
        public string EnglishName
		{
			get
			{
                Debug.Assert("" != m_sEnglishName);
                return m_sEnglishName;
			}
		}
        string m_sEnglishName = "";
		#endregion
		#region Attr{g}: string Marker
		public string Marker
		{
			get
			{
				Debug.Assert("" != m_sMarker);
				return m_sMarker;
			}
		}
		string m_sMarker = "";
		#endregion
		#region Attr{g}: string BitmapName
		public string BitmapName
		{
			get
			{
				Debug.Assert("" != m_sBitmapName);
				return m_sBitmapName;
			}
		}
		string m_sBitmapName = "";
		#endregion
		#region Attr{g/s}: Color BackgroundColor
		public Color BackgroundColor
		{
			get
			{
				if (IsCombined)
					return DNote.GetNoteDef( CombineWithNoteType ).BackgroundColor;
				return m_clrBackground;
			}
			set
			{
				if (!IsCombined)
				{
					m_clrBackground = value;
					JW_Registry.SetValue(c_RegKeyBgColor, CombineWithNoteType.ToString(), value.Name);
				}
			}
		}
		Color m_clrBackground;
		#endregion
		#region Attr{g/s}: string BackgroundColorName
		public string BackgroundColorName
		{
			get
			{
				return m_clrBackground.Name;
			}
			set
			{
				BackgroundColor = Color.FromName(value);
			}
		}
		#endregion
        #region Attr{g}: bool ShowDefault
        public bool ShowDefault
        {
            get
            {
                return m_bShowDefault;
            }
        }
        bool m_bShowDefault = false;
        #endregion
        #region Attr{g}: bool ShowInDraftingLayout
		public bool ShowInDraftingLayout
		{
			get
			{
				return m_bShowInDraftingLayout;
			}
		}
		bool m_bShowInDraftingLayout = true;
		#endregion
		#region VAttr{g}: string DisplayName - returns the localized name 
		public string DisplayName
		{
			get
			{
                return G.GetLoc_NoteDefs(NoteType.ToString(), EnglishName);
			}
		}
		#endregion

		#region Attr{g}: DNote.Types CombineWithNoteType
		public DNote.Types CombineWithNoteType
		{
			get
			{
				if (DNote.Types.kUnknown == m_CombineWithNoteType)
					return NoteType;
				return m_CombineWithNoteType;
			}
		}
		DNote.Types m_CombineWithNoteType = DNote.Types.kUnknown;
		#endregion
		#region Attr{g}: bool IsCombined
		public bool IsCombined
		{
			get
			{
				if (DNote.Types.kUnknown != m_CombineWithNoteType)
					return true;
				return false;
			}
		}
		#endregion

        // Virtual Attrs ---------------------------------------------------------------------
        #region Method: Bitmap GetTransparentBitmap(Color clrBackground)
        public Bitmap GetTransparentBitmap(Color clrBackground)
        {
            // Retrieve the bitmap from resources
            Bitmap bmp = JWU.GetBitmap(BitmapName);

            // Set its transparent color to the background color. We assume that the
            // pixel at 0,0 is a background pixel.
            Color clrTransparent = bmp.GetPixel(0, 0);
            for (int h = 0; h < bmp.Height; h++)
            {
                for (int w = 0; w < bmp.Width; w++)
                {
                    if (bmp.GetPixel(w, h) == clrTransparent)
                        bmp.SetPixel(w, h, clrBackground);
                }
            }

            return bmp;
        }
        #endregion

        #region Attr{g/s}: bool Show - whether or not the note is visible / active
        public bool Show
		{
			get
			{
				return JW_Registry.GetValue(c_RegKey, CombineWithNoteType.ToString(), false);
			}
			set
			{
				JW_Registry.SetValue(c_RegKey, CombineWithNoteType.ToString(), value);
			}
		}
		#endregion
		#region Method: static bool ShowHintsFromFront
		static public bool ShowHintsFromFront
		{
			get
			{
				return JW_Registry.GetValue(c_RegKey, "HintsFromFront", false);
			}
			set
			{
				JW_Registry.SetValue(c_RegKey, "HintsFromFront", value);
			}
		}
		#endregion

		// Scaffolding -----------------------------------------------------------------------
		const string c_RegKey = "Notes";
		const string c_RegKeyBgColor = "Notes\\BgColors";
		#region Constructor(type, sName, sMarker, sBitmapName, clrBackground)
        public DNoteDef(DNote.Types type, string sEnglishName, string sMarker, 
			string sBitmapName, Color clrBackground, bool bShowDefault,
			bool bShowInDraftingLayout, 
			DNote.Types _CombineWithType)
		{
			m_NoteType              = type;
            m_sEnglishName          = sEnglishName;
			m_sMarker               = sMarker;
			m_sBitmapName           = sBitmapName;
			m_bShowInDraftingLayout = bShowInDraftingLayout;
            m_bShowDefault          = bShowDefault;
			m_CombineWithNoteType   = _CombineWithType;

			// By calling the Registry via Get, if there is no value currently
			// stored, then the default we pass in will be stored there.
			JW_Registry.GetValue(c_RegKey, NoteType.ToString(), bShowDefault);

			// Retrieves the background color, using the default if nothing is there
			BackgroundColorName = JW_Registry.GetValue(c_RegKeyBgColor, 
				CombineWithNoteType.ToString(), clrBackground.Name);
		}
		#endregion
	}
	#endregion

	#region DOC: To Add a new Note Type
	/* To Add a new Note Type
	 * 
	 * In DNote:
	 * - Create an icon
	 * - Define it in the Types enumration
	 * - Add it in the InitializeNoteDefs() method
	 * - Create a ShowX attr for it
	 * - Give it any appropriate insertion behavior in the GetDefaultNoteText() method
	 * 
	 * Options Dialog (JobConfigure.cs) is now automatic; nothing is needed to be done.
	 * 
	 * OBS? ToolBar.cs / Setup_NotesDropdown()
	 * OBS? - Add a ButtonItem for the note, use the ShowX attr to decide whether to display it
	 * 
	 * OurWordMain / BarEx / Setup_NoteVisibility()
	 * - Create a Constant, e.g., c_InsertNoteToDo
	 * - Need a menu item for inserting the note
	 * - Need a toolbar button for inserting the note
	 * - Create a line in the UserCommandHandler...switch() statement
	 * - Set this in the Setup_NoteVisibilty() method
	 * - Add the menu item and toolbar item in DotNetBar
	 * - add to BarEx.Enable_Notes
	 * - Misc other odds and ends, look at General Note for example
	 */
	#endregion

	#region Class: DNote : JObject
	public class DNote : JObject
	{
		// BAttrs ----------------------------------------------------------------------------
		#region BAttr{g/s}: Types NoteType - (kGeneral, kToDo, kOldVersion, etc.)
		public Types NoteType
		{
			get
			{
				return (Types)m_nNoteType;
			}
			set
			{
                SetValue(ref m_nNoteType, (int)value);
			}
		}
		private int m_nNoteType = (int)Types.kGeneral;
		#endregion
		#region BAttr{g/s}: string Reference - e.g., "12", or "Title"
		public string Reference
		{
			get
			{
				return m_sReference;
			}
			set
			{
                SetValue(ref m_sReference, value);
			}
		}
		private string m_sReference = "";
		#endregion
		#region Method: void DeclareAttrs()
		protected override void DeclareAttrs()
		{
			base.DeclareAttrs();

			DefineAttr("NoteType", ref m_nNoteType);
			DefineAttr("Ref",      ref m_sReference);
		}
		#endregion

        // JAttrs ----------------------------------------------------------------------------
		#region JAttr{g/s}: DBasicText NoteText
		public DBasicText NoteText
		{
			get
			{
				return j_ownNoteText.Value;
			}
			set
			{
				j_ownNoteText.Value = value;
			}
		}
		private JOwn<DBasicText> j_ownNoteText = null;
		#endregion

		// Note Definitions ------------------------------------------------------------------
		#region SAttr{g}: ArrayList NoteDefs
		static public ArrayList NoteDefs
		{
			get
			{
                DNote.InitializeNoteDefs();
                Debug.Assert(null != s_rgNoteDefs);
				return s_rgNoteDefs;
			}
		}
		static ArrayList s_rgNoteDefs = null;
		#endregion
		#region enum Types - kHint, kToDo, kReason, kFront, kBT, etc.
		public enum Types 
		{
			kGeneral = 0, 
			kHintForDaughter, 
			kToDo, 
			kReason, 
			kFront, 
			kDefinition, 
			kOldVersion,
			kBT,
			kAskUns,
			kGreek,
			kHebrew,
			kExegesis,
			kUnknown 
		};
		#endregion
        #region SMethod: bool IsNoteMarker(string sMarker) - sMarker is one of the notes markers
        static public bool IsNoteMarker(string sMarker)
        {
            InitializeNoteDefs();

            foreach (DNoteDef def in NoteDefs)
            {
                if (def.Marker == sMarker)
                    return true;
            }

            return false;
        }
        #endregion
        #region Method: static void InitializeNoteDefs()
        static private void InitializeNoteDefs()
		{
			if (null != s_rgNoteDefs)
				return;

			s_rgNoteDefs = new ArrayList(); 

			s_rgNoteDefs.Add( new DNoteDef(Types.kGeneral, 
                "General", "nt", 
				"NoteGeneric.ico", Color.White, true, true, DNote.Types.kUnknown)
				);

            s_rgNoteDefs.Add(new DNoteDef(Types.kHintForDaughter, 
                "Hint for Drafting Daughters", "ntHint", 
				"Note_Hint.ico", Color.White, false, true, DNote.Types.kUnknown )
				);

			s_rgNoteDefs.Add( new DNoteDef(Types.kToDo, 
                "To Do", "ntck", 
				"Note_ToDo.ico", Color.LightYellow, true, true, DNote.Types.kUnknown )
				);

			s_rgNoteDefs.Add( new DNoteDef(Types.kAskUns, 
                "Ask UNS", "ntUns", 
				"Note_AskUns.ico", Color.LightPink, false, true, DNote.Types.kUnknown )
				);

			s_rgNoteDefs.Add( new DNoteDef(Types.kReason, 
                "Reason", "ntReas", 
				"Note_Reason.ico", Color.White, false, true, DNote.Types.kUnknown )
				);

			s_rgNoteDefs.Add( new DNoteDef(Types.kFront, 
                "Front Issues", "ntFT", 
				"Note_Front.ico", Color.Honeydew, true, true, DNote.Types.kUnknown )
				);

			s_rgNoteDefs.Add( new DNoteDef(Types.kDefinition, 
                "Definitions", "ntDef",
				"Note_Definition.ico", Color.White, true, true, DNote.Types.kUnknown )
				);

			s_rgNoteDefs.Add( new DNoteDef(Types.kOldVersion, 
                "Old Versions", "ov", 
				"Note_OldVersions.ico", Color.LightCyan, true, true, DNote.Types.kUnknown )
				);

			s_rgNoteDefs.Add( new DNoteDef(Types.kBT,
                "Back Translation", "ntBT", 
				"Note_BT.ico", Color.LightPink, false, false, DNote.Types.kUnknown )
				);

			// Three types of Exegesis Notes
			s_rgNoteDefs.Add( new DNoteDef(Types.kGreek, 
                "Greek", "ntgk", 
				"Note_Greek.ico", Color.PapayaWhip, false, false, DNote.Types.kExegesis )
				);
			s_rgNoteDefs.Add( new DNoteDef(Types.kHebrew, 
                "Hebrew", "nthb", 
				"Note_Hebrew.ico", Color.PapayaWhip, false, false, DNote.Types.kExegesis )
				);
			s_rgNoteDefs.Add( new DNoteDef(Types.kExegesis, 
                "Exegesis", "ntcn", 
				"Note_Exegesis.ico", Color.PapayaWhip, false, false, DNote.Types.kUnknown)
				);
		}
		#endregion
		#region Attr{g}: DNoteDef NoteDef - the DNoteDef for this DNote
		public DNoteDef NoteDef
		{
			get
			{
				InitializeNoteDefs();
				foreach(DNoteDef def in s_rgNoteDefs)
				{
					if (def.NoteType == NoteType)
						return def;
				}
				Debug.Assert(false, "Note has a Type that is not in the NoteDefs array");
				return null;
			}
		}
		#endregion
		#region Method: static DNoteDef GetNoteDef(Types type)
		static public DNoteDef GetNoteDef(Types type)
		{
			InitializeNoteDefs();
			foreach( DNoteDef def in s_rgNoteDefs )
			{
				if (def.NoteType == type)
					return def;
			}
			return null;
		}
		#endregion

		// Show Individual Note Types --------------------------------------------------------
		#region Attr{g/s}: bool ShowGeneral
		static public bool ShowGeneral
		{
			get
			{
				bool bUserDesire = GetNoteDef(Types.kGeneral).Show;
				bool bEnvironmentOverride = !OurWordMain.TargetIsLocked;
				return bUserDesire && bEnvironmentOverride;
			}
			set
			{
				GetNoteDef(Types.kGeneral).Show = value;
			}
		}
		#endregion
		#region Attr{g/s}: bool ShowHintForDaughter
		static public bool ShowHintForDaughter
		{
			get
			{
				bool bUserDesire = GetNoteDef(Types.kHintForDaughter).Show;
				bool bEnvironmentOverride = !OurWordMain.TargetIsLocked;
				return bUserDesire && bEnvironmentOverride;
			}
			set
			{
				GetNoteDef(Types.kHintForDaughter).Show = value;
			}
		}
		#endregion
		#region Attr{g/s}: bool ShowToDo
		static public bool ShowToDo
		{
			get
			{
				bool bUserDesire = GetNoteDef(Types.kToDo).Show;
				bool bEnvironmentOverride = true;
				return bUserDesire && bEnvironmentOverride;
			}
			set
			{
				GetNoteDef(Types.kToDo).Show = value;
			}
		}
		#endregion
		#region Attr{g/s}: bool ShowReason
		static public bool ShowReason
		{
			get
			{
				bool bUserDesire = GetNoteDef(Types.kReason).Show;
				bool bEnvironmentOverride = !OurWordMain.TargetIsLocked;
				return bUserDesire && bEnvironmentOverride;
			}
			set
			{
				GetNoteDef(Types.kReason).Show = value;
			}
		}
		#endregion
		#region Attr{g/s}: bool ShowFront
		static public bool ShowFront
		{
			get
			{
				bool bUserDesire = GetNoteDef(Types.kFront).Show;
				bool bEnvironmentOverride = !OurWordMain.TargetIsLocked;
				return bUserDesire && bEnvironmentOverride;
			}
			set
			{
				GetNoteDef(Types.kFront).Show = value;
			}
		}
		#endregion
		#region Attr{g/s}: bool ShowDefinition
		static public bool ShowDefinition
		{
			get
			{
				bool bUserDesire = GetNoteDef(Types.kDefinition).Show;
				bool bEnvironmentOverride = !OurWordMain.TargetIsLocked;
				return bUserDesire && bEnvironmentOverride;
			}
			set
			{
				GetNoteDef(Types.kDefinition).Show = value;
			}
		}
		#endregion
		#region Attr{g/s}: bool ShowOldVersion
		static public bool ShowOldVersion
		{
			get
			{
				bool bUserDesire = GetNoteDef(Types.kOldVersion).Show;
				bool bEnvironmentOverride = !OurWordMain.TargetIsLocked;
				return bUserDesire && bEnvironmentOverride;
			}
			set
			{
				GetNoteDef(Types.kOldVersion).Show = value;
			}
		}
		#endregion
		#region Attr{g/s}: bool ShowBT
		static public bool ShowBT
		{
			get
			{
				bool bUserDesire = GetNoteDef(Types.kBT).Show;
				bool bEnvironmentOverride = !OurWordMain.TargetIsLocked;
				return bUserDesire && bEnvironmentOverride;
			}
			set
			{
				GetNoteDef(Types.kBT).Show = value;
			}
		}
		#endregion
		#region Attr{g/s}: ShowAskUns ShowAskUns
		static public bool ShowAskUns
		{
			get
			{
				bool bUserDesire = GetNoteDef(Types.kAskUns).Show;
				bool bEnvironmentOverride = !OurWordMain.TargetIsLocked;
				return bUserDesire && bEnvironmentOverride;
			}
			set
			{
				GetNoteDef(Types.kAskUns).Show = value;
			}
		}
		#endregion

		#region Attr{g/s}: static bool ShowHintsFromFront
		static public bool ShowHintsFromFront
		{
			get
			{
				bool bUserDesire = DNoteDef.ShowHintsFromFront;
				bool bEnvironmentOverride = !OurWordMain.TargetIsLocked;
				return bUserDesire && bEnvironmentOverride;
			}
			set
			{
				DNoteDef.ShowHintsFromFront = value;
			}
		}
		#endregion

        #region VAttr{g}: bool IsShowingAny
        public static bool IsShowingAny
        {
            get
            {
                foreach (DNoteDef nd in NoteDefs)
                {
                    if (nd.Show)
                        return true;
                }

                if (ShowHintsFromFront)
                    return true;

                return false;
            }
        }
        #endregion

        // Derived Attrs ---------------------------------------------------------------------
		#region Attr{g}: DText Text - the owning DText
		public DText Text
		{
			get
			{
				DText text = Owner as DText;
				Debug.Assert(null != text);
				return text;
			}
		}
		#endregion
		#region Attr{g}: DParagraph Paragraph - the owning paragraph
		public DParagraph Paragraph
		{
			get
			{
				return Text.Paragraph;
			}
		}
		#endregion
		#region Attr{g}: static string StyleAbbrev - returns, e.g., "nt"
		static public string StyleAbbrev
		{
			get
			{
				return G.TeamSettings.SFMapping.StyleNoteParagraph; 
			}
		}
		#endregion
        #region VAttr{g}: JParagraphStyle Style
        public JParagraphStyle Style
        {
            get
            {
                return G.StyleSheet.FindParagraphStyle(StyleAbbrev);
            }
        }
        #endregion
        #region VAttr{g}: bool IsUserEditable
        public bool IsUserEditable
        {
            get
            {
                if (G.Map.IsSeeAlsoFootnoteParaStyle(StyleAbbrev))
                    return false;
                return true;
            }
        }
        #endregion
		#region VAttr{g}: bool Show - T if settings indicate this note should be shown
		public bool Show
		{
			get
			{
                // If the Notes Window is not visible, then we don't show notes.
                if (!G.App.HasSideWindows)
                    return false;
                if (!G.App.SideWindows.HasNotesWindow)
                    return false;

                // Some notes (e.g., BT) are not shown if the current layout is Drafting
                if (G.App.MainWindowIsDrafting && !NoteDef.ShowInDraftingLayout)
                    return false;

                // Determine if this is a Front Translation "Hint For Daughter" note; and if
                // so, whether it should be shown. Criteria:
                // - It is owned by the front translation
                // - It is the "Hint" type
                // - The ShowHintsFromFront user setting is set to true
                if (Paragraph.Translation == G.FTranslation)
                {
                    if ((NoteType == Types.kHintForDaughter) && DNote.ShowHintsFromFront)
                        return true;
                    return false;
                }

                // Finally after all of this, see what the current user setting is.
				if (NoteDef.Show)
					return true;

				return false;
			}
		}
		#endregion

        // Scaffolding -----------------------------------------------------------------------
		#region Method: Constructor() - creates the attrs
		private DNote()
			: base()
		{
			// Owning atomic
			j_ownNoteText = new JOwn<DBasicText>("NoteText", this);

			// Note definitions
			InitializeNoteDefs();
		}
		#endregion
		#region Method: Constructor(DNote)
		public DNote( DNote noteSource )
			: this()
		{
			NoteType  = noteSource.NoteType;
			Reference = noteSource.Reference;

			NoteText  = new DBasicText( noteSource.NoteText );
		}
		#endregion
		#region Constructor(sReference, sNoteText, Types t)
		public DNote(string sReference, DBasicText noteText, Types t)
			: this()
		{
			// Make sure we did the DBasicText/DText conversion correctly
			Debug.Assert(null == noteText as DText);

			NoteText = noteText;

			Reference = sReference;
			NoteType = t;
		}
		#endregion

		#region Method: override bool ContentEquals(obj) - required override to prevent duplicates
		public override bool ContentEquals(JObject obj)
		{
			if (this.GetType() != obj.GetType())
				return false;

			DNote noteToCompare = obj as DNote;
			if (null == noteToCompare)
				return false;

			// Compare the note types
			if (noteToCompare.NoteType != this.NoteType)
				return false;

			// Compare the contents
			string s1 = NoteText.ContentsSfmSaveString;
			string s2 = noteToCompare.NoteText.ContentsSfmSaveString;
			if (s1 != s2)
				return false;

			return true;
		}
		#endregion

		// I/O -------------------------------------------------------------------------------
		#region Method: void ToDB(ScriptureDB DB)
		public virtual void ToDB(ScriptureDB DB)
		{
			DB.Append( new SfField(NoteDef.Marker, NoteText.ContentsSfmSaveString));
		}
		#endregion
		#region Method: static Types GetTypeFromMarker(string sMarker)
		static public Types GetTypeFromMarker(string sMarker)
		{
			// Is it defined in our known note types?
			DNote.InitializeNoteDefs();
			foreach( DNoteDef def in s_rgNoteDefs )
			{
				if (def.Marker == sMarker)
					return def.NoteType;
			}

			// Otherwise, it is not a note marker
			return Types.kUnknown;
		}
		#endregion

		// Methods ---------------------------------------------------------------------------
		#region Method: static string GetDefaultNoteText(...)
		static public string GetDefaultNoteText(Types type, string sSelectionText, DParagraph pOwner)
			// Given the command to create a new note from the Draft View, this method
			// composes the appropriate note text according to the type of note. 
		{
			// Old Version notes: Create a string consisting of today's date plus the
			// vernacular text.
			if (Types.kOldVersion == type)
			{
				string sDate = DateTime.Today.ToString("dd/MMM/yyyy") + " - ";
                if (sSelectionText.Length == 0)
                    sSelectionText = pOwner.SimpleText;
                return sDate + sSelectionText;
			}

			// To Do notes: Create a string consisting of the words "To Do: "
			if (Types.kToDo == type)
			{
                return G.GetLoc_Notes("ToDoDefault", "To Do:").Trim() + " " + sSelectionText;
			}

			// Front notes
			if (Types.kFront == type)
			{
                string sLabel = G.GetLoc_Notes("FrontDefault", "Front:").Trim() + " "; 

				if (null != OurWordMain.Project && null != OurWordMain.Project.FrontTranslation)
				{
					DTranslation t = OurWordMain.Project.FrontTranslation;
					if (t.DisplayName.Length > 0)
						sLabel = t.DisplayName + ": ";
				}

                return sLabel + sSelectionText;
			}

			// Hint for daughter note:
			if (Types.kHintForDaughter == type)
			{
                return G.GetLoc_Notes("HintDefault", "Hint:").Trim() + " " + sSelectionText;
			}

			// Back Translation Note
			if (Types.kBT == type)
			{
                return G.GetLoc_Notes("BackTTranslationDefault", "BT:").Trim() + " " + sSelectionText;
			}

			// Ask UNS Note
			if (Types.kAskUns == type)
			{
                return G.GetLoc_Notes("AskUnsDefault", "Ask UNS").Trim() + " " + sSelectionText;
			}

			// Greek Note
			if (Types.kGreek == type)
			{
                return G.GetLoc_Notes("GreekDefault", "Greek:").Trim() + " " + sSelectionText;
			}

			// Hebrew Note
			if (Types.kHebrew == type)
			{
                return G.GetLoc_Notes("HebrewDefault", "Hebrew:").Trim() + " " + sSelectionText;
			}

			// Exegesis Note
			if (Types.kExegesis == type)
			{
                return G.GetLoc_Notes("ExegesisDefault", "Exegesis:").Trim() + " " + sSelectionText;
			}

			// All other notes
            if (sSelectionText.Length == 0)
				return " ";
            return sSelectionText;
		}
		#endregion
		#region Method: static string ConvertOldVerseReferences(string s)
		public static string ConvertOldVerseReferences(string s)
			// From 0.6b and before, we had all DNotes attached to a paragraph, and used
			// {v 2} in the data to create a "2" in the Verse Text stype. Now that notes
			// are attached to the DText, it is no longer possible for a DNote to have
			// more than one verse reference. But the old data is still around (well, in 
			// Timor anyway.) So what this does is convert {v 2} into "|b2:|r " so that
			// it appears as bold. 
		{
			string sOut = "";

			for(int i=0; i < s.Length; i++)
			{
				if (i < s.Length - 3 && s.Substring(i, 3) == "{v ")
				{
					sOut += "|b";
					i += 3;
					while (i < s.Length && s[i] != '}')
					{
						sOut += s[i];
						i++;
					}
					sOut += ":|r ";
				}
				else
					sOut += s[i];
			}

			return sOut;
		}
		#endregion

	}
	#endregion


}
