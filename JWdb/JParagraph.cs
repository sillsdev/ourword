/**********************************************************************************************
 * App:     Josiah
 * File:    JParagraph.cs
 * Author:  John Wimbish
 * Created: 19 Dec 2003
 * Purpose: Implements a paragraph object
 * Legal:   Copyright (c) 2005-07, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
#region Using
using System;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using JWTools;
#endregion

#region Stories
/*
 * - A paragraph's main attribute is its Contents, which is a string containing the paragraph
 *      text. 
 * 
 * - A paragraph has a Style attribute, which allows it to reference the appropriate paragraph
 *      style information.
 * 
 * - Character styles are represented within the Contents string by {|StyleName text}, where
 *      StyleName refers to a character style either in the stylesheet, oro a built-in 
 *      special style.
 * 
 * - I/O
 *    - A paragraph can read/write itself from/to xml.
 *    - A paragraph can read/write itself from/to standard format.
 * 
 * - A paragraph can display itself in a window, given the left, top, right window coordinates,
 *      and returns the y of the lowest extent of the paragraph when done.
 * 
 * - Upon a click in the window where last displayed, the paragraph can determine where to
 *      place the mouse pointer.
 */
#endregion

namespace JWdb
{


	public class JParagraph : JObject
	{
		// ZAttrs ----------------------------------------------------------------------------
		#region BAttr{g/s}: string Contents - the paragraph contents
		public virtual string Contents
		{
			get
			{
				return m_sContents;
			}
			set
			{
                SetValue(ref m_sContents, value);
			}
		}
		protected string m_sContents = "";
		#endregion
		#region BAttr{g/s}: string StyleAbbrev - e.g., "p", "q", "q2".
		public string StyleAbbrev
		{
			get
			{
				return m_sStyleAbbrev;
			}
			set
			{
                SetValue(ref m_sStyleAbbrev, value);
			}
		}
		private string m_sStyleAbbrev = "p";
		#endregion
		#region Method: void DeclareAttrs()
		protected override void DeclareAttrs()
		{
			base.DeclareAttrs();
			DefineAttr("Contents", ref m_sContents);
			DefineAttr("Abbrev",  ref m_sStyleAbbrev);
		}
		#endregion

		// Attributes ------------------------------------------------------------------------
		#region Attr{g}:  bool IsEmpty - there is nothing in the paragraph
		public bool IsEmpty
		{
			get { return Contents == ""; }
		}
		#endregion
		private JWritingSystem m_WritingSystem = null;

		// Scaffolding -----------------------------------------------------------------------
		#region Constructor(WritingSystem ws)
		public JParagraph(JWritingSystem ws)
			: base()
		{
			m_WritingSystem = ws;
			Contents = "";
		}
		#endregion

		// I/O -------------------------------------------------------------------------------
		#region Documentation of IO Format
		/* A paragraph is stored as
		 *    <p>The paragraph contents are on one long line.</p>
		 * Line indendation is supported to allow for more readable ownership.
		 */
		#endregion
		#region Method: void Write(TextWriter) - writes the paragraph to xml file
		public override void Write(TextWriter tw, int nIndent)
		{
			tw.WriteLine(IndentPadding(nIndent) + "<p>" + Contents + "</p>");
		}
		#endregion
		#region Method: JParagraph(sLine, TextReader) - read constructor
		public JParagraph(string sLine, TextReader tr)
		{
			if (JW_Xml.IsTag("p", sLine))
			{
				Contents = "";
				int i=0;
				while (i < sLine.Length && sLine[i] != '>')
					++i;
				if (i < sLine.Length && sLine[i] == '>')
					++i;
				while (i < sLine.Length && sLine[i] != '<')
				{
					Contents += sLine[i];
					++i;
				}
			}
		}
		#endregion

		// Words as painted on the display ---------------------------------------------------
		#region Attr{g}: ArrayList Words - returns m_rgWords to, e.g., the BT mechanism
		public ArrayList Words
		{
			get
			{
				return m_rgWords;
			}
		}
		#endregion
		#region Method: void _AddWordTest(...) - Helper method for _BuildWordList()
		private bool _AddWordTest(int i, char chBeginCharStyle, char chEndCharStyle)
			// This is the test in _BuildWordList() that decides whether it is time to 
			// cut off building the current sWord, and add it to the list. The test
			// got complicated enough that it needed its own method so that I could
			// better document the various conditions that are being tested for.
		{
			// If we are at the end of the line, then we want to add the current
			// word to the wordlist.
			if (i == Contents.Length)
				return true;

			// Shorthand
			char ch = Contents[i];

			// If we are about to start a new character style, then add the word
			// we've been working on.
			if (ch == chBeginCharStyle)
				return true;

			// If we are about to end a character style, then add the word
			// we've been working on.
			if (ch == chEndCharStyle)
				return true;

			// If we have come to a blank, then add the word.
			if (ch == ' ')
				return true;

			// If we are processing a script such as Chinese, then every non-numeric
			// character is its own word. However, if the glyph is punctuation,
			// then we don't return true, because we want to include the punctuation
			// with the word we're currently building.
			if (m_WritingSystem.IsIdeaGraph)
			{
				if (ch == '-' || Char.IsDigit(ch))
					return false;

				if (m_WritingSystem.IsPunctuation( ch ))
					return false;

				return true;
			}
			return false;
		}
		#endregion
		#region OBSOLETE - Method: void BuildWordList() - parse the para into an ArrayList of CWord's
/****
		public void BuildWordList()
		{
			// Prepare the word list destination
			if (null == m_rgWords)
				m_rgWords = new ArrayList();
			m_rgWords.Clear();
			if (Contents.Length == 0)
				return;

			// Setup
			char chBeginCharStyle = '{';
			char chEndCharStyle = '}';
			JCharacterStyle CharStyle = Style.CharacterStyle;

			// Build word loop
			string sWord = "";
			int i = 0;
			int iWordStart = 0;
			while(true)
			{
				// Add the word if appropriate
				if (sWord.Length > 0 && _AddWordTest( i, chBeginCharStyle, chEndCharStyle) )
				{
					if (i < Contents.Length && Contents[i] == ' ')
					{
						sWord += ' ';
						i++;
					}

					// If this word is ended by a chBeginCharStyle, then we want
					// to "glue" it to that next word. (This will, e.g., glue
					// footnotes to their preceeding word.)
					bool bGlueToNext = false;
					if (i < Contents.Length && Contents[i] == chBeginCharStyle)
						bGlueToNext = true;

					// If this word is ended by a chEndCharStyle, and if the letter
					// following it is not blank, then we want to "glue" it to the 
					// next word. (This will, e.g., glue verse numbers to their
					// following word.
					if (i < Contents.Length-1 && 
						Contents[i] == chEndCharStyle &&
						Contents[i+1] != ' ')
					{
						bGlueToNext = true;
					}

					CWord w = new CWord(sWord, CharStyle, bGlueToNext, i - iWordStart);
					iWordStart = i;
					m_rgWords.Add( w );
					sWord = "";
				}

				// Are we done?
				if (i == Contents.Length)
					break;

				// Beginning a character style
				if (Contents[i] == chBeginCharStyle)
				{
					CharStyle = _RetrieveCharStyle(Contents.Substring(i));
					i = _IncrementTo(Contents, i, ' ');
					i = _IncrementPast(Contents, i, ' ');
					continue;
				}

				// Ending a character style
				if (Contents[i] == chEndCharStyle)
				{
					CharStyle = Style.CharacterStyle;
					i = _IncrementPast(Contents, i, chEndCharStyle);
					i = _IncrementPast(Contents, i, ' ');
					continue;
				}

				// Otherwise, build the word
				sWord += Contents[i];

				// Increment to the next position
				i++;
			} // endfor
		}
		****/
		#endregion
		#region Method: int _IncrementTo(s, iPos, chTarget) - increment to the chTarget character
		private int _IncrementTo(string s, int iPos, char chTarget)
		{
			while (iPos < s.Length && s[iPos] != chTarget)
				++iPos;
			return iPos;
		}
		#endregion
		#region Method: int _IncrementPast(s, iPos, chTarget) - increment past any chTarget characters
		private int _IncrementPast(string s, int iPos, char chTarget)
		{
			while (iPos < s.Length && s[iPos] == chTarget)
				++iPos;
			return iPos;
		}
		#endregion
		protected ArrayList m_rgWords;           // temp: the para broken down into words
		#region OBSOLETE Method: private Font _RetrieveCharStyle(string s)
/***
		private JCharacterStyle _RetrieveCharStyle(string s)
			// Assumes that <s> is at a character style, e.g., "{v " for the "verse" style,
			// looks up that style and returns its font information.
		{
			// Copy out the name of the character style
			int i=1;
			string sCharStyle = "";
			while (i < s.Length && s[i] != ' ')
			{
				sCharStyle += s[i];
				i++;
			}

			// Retrieve the character style from the style sheet
			JCharacterStyle charStyle = Style.StyleSheet.FindCharacterStyle(sCharStyle);

			// Return the character style's font; or the paragraph's font if not found
			if (null != charStyle)
				return charStyle;
			return Style.CharacterStyle;
		}
****/
		#endregion

		// Data Access -----------------------------------------------------------------------
		#region Method: void AppendToContents(string) - appends the string to the paragraph
		public void AppendToContents(string s)
			// We add s to whatever is currently in the Contents paragraph. 
			//
			// Formerly, I was trimming these; but have decided instead to let the
			// caller deal with whether or not there should be leading/trailing
			// blanks.
		{
			if (IsEmpty)
				Contents = s;
			else
				Contents = Contents + s;
		}
		#endregion
	}

}
