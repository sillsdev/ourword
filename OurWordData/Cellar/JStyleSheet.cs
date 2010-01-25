/**********************************************************************************************
 * File:    JStyleSheet.cs
 * Author:  John Wimbish
 * Created: 25 Jan 2004
 * Purpose: Handles paragraph and character styles, and the containing stylesheet
 * Legal:   Copyright (c) 2005-09, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
#region Using
using System;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using System.Xml;
using JWTools;
#endregion
#region Documentation
/* Documentation
 * 
 * A Font's Height refers to line spacing; Size is the size of the letters. All specified
 * in Points. I typically use a Size of 10, which results in a Height of 16.
 */
#endregion

namespace OurWordData
{
	#region CLASS JStyleSheet
	public class JStyleSheet : JObject
	{
		// BAttrs ----------------------------------------------------------------------------
		#region BAttr{g/s}: string DisplayName - the style's name as it appears in the UI
		public string DisplayName
		{
			get
			{
				return m_sDisplayName;
			}
			set
			{
                SetValue(ref m_sDisplayName, value);
			}
		}
		private string m_sDisplayName = "Default Style Sheet";
		#endregion
		#region Method: void DeclareAttrs()
		protected override void DeclareAttrs()
		{
			base.DeclareAttrs();
			DefineAttr("DisplayName", ref m_sDisplayName);
		}
		#endregion

		// JAttributes: ----------------------------------------------------------------------
		#region Attr{g}: JOwnSeq WritingSystems - the list of writing systems
		public JOwnSeq<JWritingSystem> WritingSystems
		{
			get 
			{ 
				return j_osWritingSystems; 
			}
		}
		private JOwnSeq<JWritingSystem> j_osWritingSystems = null;
		#endregion

		// Scaffolding -----------------------------------------------------------------------
		#region Constructor()
		public JStyleSheet()
			: base()
		{
            j_osWritingSystems = new JOwnSeq<JWritingSystem>("WritingSystems", this, true, true);
		}
		#endregion
		#region Attr(g): string SortKey - overridden to enable JOWnSeq Find method support.
		public override string SortKey
			// In order to support sorting, the subclass must implement a SortKey attribute,
			// and this SortKey must return something other than an empty string. 
		{
			get 
			{ 
				return DisplayName; 
			}
		}
		#endregion

		// WritingSystems Access -------------------------------------------------------------
		#region Method: JWritingSystem AddWritingSystem(sName) - Adds a new WS
		public JWritingSystem AddWritingSystem(string sName)
		{
			JWritingSystem ws = new JWritingSystem(sName);
			WritingSystems.Append(ws);
			return ws;
		}
		#endregion
		#region Method: JWritingSystem FindWritingSystem(sName) - returns the named WS
		public JWritingSystem FindWritingSystem(string sName)
		{
			int i = WritingSystems.Find(sName);
			if (-1 == i)
				return null;
			return (JWritingSystem)WritingSystems[i];
		}
		#endregion
		#region Method: JWritingSystem FindOrAddWritingSystem(sName) - creates the WS if necessary
		public JWritingSystem FindOrAddWritingSystem(string sName)
		{
			JWritingSystem ws = FindWritingSystem(sName);
			if (null != ws)
				return ws;

			// Add the writing system
			ws = AddWritingSystem(sName);

			return ws;
		}
		#endregion
		#region Method: JWritingSystem GetWritingSystem(index) - returns the WS at the given index
		public JWritingSystem FindWritingSystem(int index)
		{
			if (index < 0 || index >= WritingSystems.Count)
				return null;
			return (JWritingSystem)WritingSystems[index];
		}
		#endregion
		#region Method: void RemoveWritingSystem(JWritingSystem)
		public void RemoveWritingSystem(JWritingSystem ws)
		{
			if (-1 == WritingSystems.FindObj(ws))
				return;


			// Remove the Writing System
			WritingSystems.Remove(ws);
		}
		#endregion

        #region Attr{g/s}: float ZoomFactor
        public float ZoomFactor
        {
            get
            {
                return m_fZoomFactor;
            }
            set
            {
                m_fZoomFactor = value;
   //             _ResetFonts();
            }
        }
        float m_fZoomFactor = 1.0F;
        #endregion
    }
	#endregion



	#region CLASS: JWritingSystem
	public class JWritingSystem : JObject
	{
		// ZAttrs ----------------------------------------------------------------------------
		#region BAttr{g/s}: bool IsIdeaGraph - a glyph equals a word, e.g., Chinese script
		public bool IsIdeaGraph
		{
			get
			{
				return m_bIsIdeaGraph;
			}
			set
			{
                SetValue(ref m_bIsIdeaGraph, value);
			}
		}
		private bool m_bIsIdeaGraph = false;
		#endregion
		#region BAttr{g/s}: string Name - the name of the writing system
		public string Name
		{
			get 
			{ 
				return m_sName;
			}
			set 
			{
                SetValue(ref m_sName, value);
			}
		}
		private string m_sName = "";
		#endregion
        #region BAttr{g/s}: string Abbrev - the name of the writing system
        public string Abbrev
        {
            get
            {
                return m_sAbbrev;
            }
            set
            {
                SetValue(ref m_sAbbrev, value);
            }
        }
        private string m_sAbbrev = "";
        #endregion
        #region BAttr{g/s}: string PunctuationChars - a list of punctuation characters for this WS
		public string PunctuationChars
		{
			get 
			{
				return m_sPunctuationChars;
			}
			set 
			{
                SetValue(ref m_sPunctuationChars, value);
			}
		}
        private string m_sPunctuationChars = c_sDefaultPunctuationChars;
        public const string c_sDefaultPunctuationChars = "“‘.,;:<>?!()[]’”";
		#endregion
		#region BAttr{g/s}: string EndPunctuationChars - punctuation that occurs sentence-final.
		public string EndPunctuationChars
		{
			get 
			{
				return m_sEndPunctuationChars;
			}
			set 
			{
                SetValue(ref m_sEndPunctuationChars, value);
			}
		}
        private string m_sEndPunctuationChars = c_sDefaultEndPunctuationChars;
        public const string c_sDefaultEndPunctuationChars = ".>?!)],:;’”";
		#endregion
		#region BAttr{g/s}: BStringArray AutoReplaceSource
		public BStringArray AutoReplaceSource
		{
			get
			{
				return m_bsaAutoReplaceSource;
			}
			set
			{
                SetValue(ref m_bsaAutoReplaceSource, value);
			}
		}
		private BStringArray m_bsaAutoReplaceSource = null;
		#endregion
		#region BAttr{g/s}: BStringArray AutoReplaceResult
		public BStringArray AutoReplaceResult
		{
			get
			{
				return m_bsaAutoReplaceResult;
			}
			set
			{
                SetValue(ref m_bsaAutoReplaceResult, value);
			}
		}
		private BStringArray m_bsaAutoReplaceResult = null;
		#endregion
        #region BAttr{g/s}: string KeyboardName - the name of the keyboard to use with this WS
        public string KeyboardName
        {
            get
            {
                return m_sKeyboardName;
            }
            set
            {
                SetValue(ref m_sKeyboardName, value);
            }
        }
        private string m_sKeyboardName = "";
        #endregion
        // Auto Hyphen Attrs
        #region BAttr{g/s}: string Consonants - a list of Consonants for this WS
        public string Consonants
        {
            get
            {
                return m_sConsonants;
            }
            set
            {
                SetValue(ref m_sConsonants, value);
            }
        }
        private string m_sConsonants = c_sDefaultConsonants;
        public const string c_sDefaultConsonants = "bcdfghjklmnpqrstvwxyz";
        #endregion
        #region BAttr{g/s}: bool UseAutomatedHyphenation
        public bool UseAutomatedHyphenation
        {
            get
            {
                return m_bUseAutomatedHyphenation;
            }
            set
            {
                SetValue(ref m_bUseAutomatedHyphenation, value);
            }
        }
        private bool m_bUseAutomatedHyphenation = false;
        #endregion
        #region BAttr{g/s}: string HyphenationCVPattern - E.g., "V-C", or "VC-CV"
        public string HyphenationCVPattern
        {
            get
            {
                return m_sHyphenationCVPattern;
            }
            set
            {
                SetValue(ref m_sHyphenationCVPattern, value);
            }
        }
        private string m_sHyphenationCVPattern = c_sHyphenationCVPattern;
        public const string c_sHyphenationCVPattern = "V-C";
        #endregion
        #region BAttr{g/s}: int MinHyphenSplit - points of space before a paragraph
        public int MinHyphenSplit
        {
            get
            {
                return m_nMinHyphenSplit;
            }
            set
            {
                SetValue(ref m_nMinHyphenSplit, value);
            }
        }
        private int m_nMinHyphenSplit = 3;
        #endregion
        // DeclareAttrs
        #region Method: void DeclareAttrs()
		protected override void DeclareAttrs()
		{
			base.DeclareAttrs();
			DefineAttr("IdeaGraph",  ref m_bIsIdeaGraph);
			DefineAttr("Name",       ref m_sName);
            DefineAttr("Abbrev",     ref m_sAbbrev);
            DefineAttr("Punct",      ref m_sPunctuationChars);
			DefineAttr("EndPunct",   ref m_sEndPunctuationChars);
            DefineAttr("Consonants", ref m_sConsonants);
            DefineAttr("AutoHyph",   ref m_bUseAutomatedHyphenation);
            DefineAttr("AutoHyphCV", ref m_sHyphenationCVPattern);
            DefineAttr("AutoHyphMinSplit", ref m_nMinHyphenSplit);
            DefineAttr("ARSource",   ref m_bsaAutoReplaceSource);
			DefineAttr("ARResult",   ref m_bsaAutoReplaceResult);
            DefineAttr("Keyboard",   ref m_sKeyboardName);
		}
		#endregion

		// Derived Attributes ----------------------------------------------------------------
		#region VAttr(g): string SortKey - overridden to enable JOWnSeq Find method support.
		public override string SortKey
			// In order to support sorting, the subclass must implement a SortKey attribute,
			// and this SortKey must return something other than an empty string. 
		{
			get 
			{ 
				return Name; 
			}
		}
		#endregion

		// Scaffolding -----------------------------------------------------------------------
		#region Constructor() - no parameters, used only for reading
		public JWritingSystem()
			: base()
		{
			ConstructAttrs();
		}
		#endregion
		#region Constructor(sName)
		public JWritingSystem(string sName)
			: base()
		{
			ConstructAttrs();
			Name = sName;
		}
		#endregion
		#region Method: override bool ContentEquals(obj) - required override to prevent duplicates
		public override bool ContentEquals(JObject obj)
		{
			if (this.GetType() != obj.GetType())
				return false;

			JWritingSystem ws = (JWritingSystem)obj;
			return ws.Name == this.Name;
		}
		#endregion
		#region Method: void ConstructAttrs()
		private void ConstructAttrs()
		{

			string[] rgSource = 
			{ 
				// `V
				"`a",     "`A",     "`e",     "`E",     "`i",     "`I",   
				"`o",     "`O",     "`u",     "`U" ,
				// ^V
				"^a",     "^A",     "^e",     "^E",     "^i",     "^I",   
				"^o",     "^O",     "^u",     "^U"   
			};
			string[] rgResult = 
			{ 
				// `V
				"\u00E0", "\u00C0", "\u00E8", "\u00C8", "\u00EC", "\u00CC", 
				"\u00F2", "\u00D2", "\u00F9", "\u00D9",
				// ^V
				"\u00E1", "\u00C1", "\u00E9", "\u00C9", "\u00ED", "\u00CD", 
				"\u00F3", "\u00D3", "\u00FA", "\u00DA" 
			};

			m_bsaAutoReplaceSource = new BStringArray(rgSource);
			m_bsaAutoReplaceResult = new BStringArray(rgResult);

			BuildAutoReplace();
		}
		#endregion

        // AutoReplace -----------------------------------------------------------------------
        #region Attr{g}: TreeRoot AutoReplace
        public TreeRoot AutoReplace
        {
            get
            {
                Debug.Assert(null != m_AutoReplace);
                return m_AutoReplace;
            }
        }
		TreeRoot m_AutoReplace = null;
        #endregion
        #region Method: void BuildAutoReplace()
        public void BuildAutoReplace()
		{
			m_AutoReplace = new TreeRoot();
			for(int i=0; i<AutoReplaceSource.Length; i++)
			{
				m_AutoReplace.Add( AutoReplaceSource[i], AutoReplaceResult[i] );
			}
		}
		#endregion
		#region Method: string SearchAutoReplace(string sSource, ref int cSourceLen)
		public string SearchAutoReplace(string sSource, ref int cSourceLen)
		{
			if (null == m_AutoReplace)
				BuildAutoReplace();
			return m_AutoReplace.Search(sSource, ref cSourceLen);
		}
		#endregion

		// Methods ---------------------------------------------------------------------------
		#region Method: bool IsPunctuation(char) - T if char is a punct character
		public bool IsPunctuation(char cTest)
		{
			foreach( char c in PunctuationChars)
			{
				if (c == cTest)
					return true;
			}
			return false;
		}
		#endregion
        #region Method: bool IsEndPunctuation(char) - T if char is a punct character
        public bool IsEndPunctuation(char cTest)
        {
            foreach (char c in EndPunctuationChars)
            {
                if (c == cTest)
                    return true;
            }
            return false;
        }
        #endregion
        #region Method: bool IsWhiteSpace(char) - T if char is a whitespace character
        public bool IsWhiteSpace(char ch)
        {
            if (ch == ' ')
                return true;
            return false;
        }
        #endregion
        #region Method: bool IsWordBreak(s, iPos)
        public bool IsWordBreak(string s, int iPos)
            // At this point, I define a word break as occuring at either the beginning of the
            // string, or following a whitespace. This method is more of a place-holder, as I
            // anticipate that there will be writing systems where other types of word break
            // rules will be necessary.
        {
            // Make sure we have valid input
            Debug.Assert(iPos >= 0);
            Debug.Assert(iPos < s.Length);
            Debug.Assert(null != s);

            // If at beginning of string, this is a word break.
            if (iPos == 0)
                return true;

            // Get the two characters of interest
            var chWhitespace = s[iPos - 1];
            var chNonWhitespace = s[iPos];

            // Do the test
            if (char.IsWhiteSpace(chWhitespace) && !char.IsWhiteSpace(chNonWhitespace))
                return true;

            return false;
        }
        #endregion
        #region Method: bool IsHyphenBreak(sWord, iPosWithinWord)
        public bool IsHyphenBreak(string s, int iPos)
            /* OK, this is quick-and-dirty, to see if I can get a simple hyphenation
             * support working. I will need to do more than this. But this implementation
             * will be that I can hyphen if:
             * - I'm four letters from either end of a word
             * - I'm located at a consonant
             * - The preveeding letter is not a consonant
             * 
             * E.g., this is a V-C type of rule.
             * 
             * I'm going to need to install that ICU stuff (sigh) to do this right.
             * Pity the poor user that must download it.
             * 
             * Or maybe I can do something where users can enter rules for simple
             * stuff. Joe tells me that Huichol hyphens after CV's. So saying CV-
             * plus enumerating the consonants is sufficient for Huichol.
             */
        {
            // Don't bother if automated hyphenation is not turned on
            if (UseAutomatedHyphenation == false)
                return false;

            // 1 - Don't be too close to an end of a word
            if (iPos < MinHyphenSplit)
                return false;
            if (iPos > s.Length - MinHyphenSplit)
                return false;

            // Find the position of the hyphen in our string
            int iHyphenPos = HyphenationCVPattern.IndexOf('-');
            if (-1 == iHyphenPos)
                return false;

            // Get the pattern string, minus the hyphen
            string sPattern = HyphenationCVPattern.Remove(iHyphenPos, 1);

            // Determine where to start, and if we have room for the test
            int iStart = iPos - iHyphenPos;
            if (iStart < 0)
                return false;
            int cPositions = sPattern.Length;
            if (iStart + cPositions > s.Length)
                return false;

            // Check against our pattern
            for(int k=0; k<cPositions; k++)
            {
                char chPattern = sPattern[k];
                char chTest = s[iStart + k];

                // Sitting on a consonant?
                if (char.ToUpper(chPattern) == 'C')
                {
                    if (Consonants.IndexOf( char.ToLower(chTest)) == -1)
                        return false;
                }

                // Sitting on a vowel?
                if (char.ToUpper(chPattern) == 'V')
                {
                    if (Consonants.IndexOf( char.ToLower(chTest)) != -1)
                        return false;
                }
            }

            return true;
        }
        #endregion

		// I/O -------------------------------------------------------------------------------
        #region OMethod: void FromXml(XElement x)
        public override void FromXml(XElement x)
        {
            base.FromXml(x);

            BuildAutoReplace();
        }
        #endregion

    }
	#endregion
}
