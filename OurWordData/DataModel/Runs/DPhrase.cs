#region ***** DPhrase.cs *****
/**********************************************************************************************
 * Project: Our Word!
 * File:    DPhrase.cs
 * Author:  John Wimbish
 * Created: 31 Jan 2005, move to separate file 19 Jan 2010
 * Purpose: A string of text, all with the same FontStyle
 * Legal:   Copyright (c) 2005-10, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
using System.Diagnostics;
using System.Drawing;
#endregion

namespace OurWordData.DataModel.Runs
{
    public class DPhrase : JObject
    {
        // Constants -------------------------------------------------------------------------
        public const char c_chInsertionSpace = '\u2004';   // Unicode's "Four-Per-EM space"
        private const char c_chVerticalBar = '|';          // Sfm for char styles

        // BAttrs ----------------------------------------------------------------------------
        #region BAttr{g/s}: public string Text - The well-formed contents of the phrase
        public string Text
        {
            get
            {
                return m_sText;
            }
            set
            {
                SetValue(ref m_sText, value);
            }
        }
        string m_sText;
        #endregion
        #region Method void DeclareAttrs()
        protected override void DeclareAttrs()
        {
            base.DeclareAttrs();
            DefineAttr("Text", ref m_sText);
        }
        #endregion

        // Style Modifications ---------------------------------------------------------------
        /* DOCUMENTATION
         * Anything other than "Regular" is interpreted as meaning that the setting has
         * been toggled. Thus if the underlying CharacterStyle is Italic, and this value
         * is also Italic, then it means Italic has been toggled to off, and the phrase
         * should not be displayed as Italic.
         */
        #region Attr{g/s}: FontStyle FontToggles
        public FontStyle FontToggles
        {
            get
            {
                return m_FontToggles;
            }
            set
            {
                m_FontToggles = value;
                DeclareDirty();
            }
        }
        private FontStyle m_FontToggles = FontStyle.Regular;
        #endregion
        #region VAttr{g}: bool ItalicIsToggled
        public bool ItalicIsToggled
        {
            get
            {
                return ((FontToggles & FontStyle.Italic) == FontStyle.Italic);
            }
        }
        #endregion
        #region VAttr{g}: bool BoldIsToggled
        public bool BoldIsToggled
        {
            get
            {
                return ((FontToggles & FontStyle.Bold) == FontStyle.Bold);
            }
        }
        #endregion
        #region VAttr{g}: bool UnderlineIsToggled
        public bool UnderlineIsToggled
        {
            get
            {
                return ((FontToggles & FontStyle.Underline) == FontStyle.Underline);
            }
        }
        #endregion

        // Derived Attrs ---------------------------------------------------------------------
        #region VAttr{g}: public string SfmSaveString -  For saving to an SFM file
        public string SfmSaveString
        {
            get
            {
                // Double any literals in the text
                var s = "";
                foreach (var ch in Text)
                {
                    s += ch;
                    if (ch == c_chVerticalBar)
                        s += ch;
                }

                // Font Modifications
                if (BoldIsToggled)
                    s = "|b" + s + "|r";
                if (ItalicIsToggled)
                    s = "|i" + s + "|r";
                if (UnderlineIsToggled)
                    s = "|u" + s + "|r";

                return s;
            }
        }
        #endregion
        #region VAttr{g}: public bool EndsWithSpace
        public bool EndsWithSpace
        {
            get
            {
                return (Text.Length > 0 && Text[Text.Length - 1] == ' ');
            }
        }
        #endregion
        #region VAttr{g}: public bool BeginsWithSpace
        public bool BeginsWithSpace
        {
            get
            {
                return (Text.Length > 0 && Text[0] == ' ');
            }
        }
        #endregion
        #region VAttr{g}: bool HasText
        public bool HasText
        {
            get
            {
                return !string.IsNullOrEmpty(Text);
            }
        }
        #endregion
        #region VAttr{g}: DBasicText BasicText - required iff editable
        public DBasicText BasicText
        {
            get
            {
                Debug.Assert(null != Owner as DBasicText);
                return Owner as DBasicText;
            }
        }
        #endregion

        // Scaffolding -----------------------------------------------------------------------
        #region Constructor(sText)
        public DPhrase(string sText)
        {
            Text = sText;
        }
        #endregion
        #region Constructor(DPhrase source)
        public DPhrase(DPhrase source)
            : this(source.Text)
        {
            m_FontToggles = source.FontToggles;
        }
        #endregion
        #region Method: override bool ContentEquals(obj) - required override to prevent duplicates
        public override bool ContentEquals(JObject obj)
        {
            if (GetType() != obj.GetType())
                return false;

            var other = obj as DPhrase;
            if (null == other)
                return false;

            if (other.Text != Text)
                return false;
            if (other.FontToggles != FontToggles)
                return false;

            return true;
        }
        #endregion

        // Methods ---------------------------------------------------------------------------
        #region Method: void RemoveLeadingSpace()
        public void RemoveLeadingSpace()
        {
            if (BeginsWithSpace)
                Text = Text.Substring(1);
        }
        #endregion
        #region Method: void RemoveTrailingSpace()
        public void RemoveTrailingSpace()
        {
            if (EndsWithSpace)
                Text = Text.Substring(0, Text.Length - 1);
        }
        #endregion
        #region Method: void EliminateSpuriousSpaces()
        public void EliminateSpuriousSpaces()
        {
            // Eliminate the special InsertionSpace
            Text = Text.Replace(c_chInsertionSpace.ToString(), "");

            int n;
            while (-1 != (n = Text.IndexOf("  ")))
                Text = Text.Remove(n + 1, 1);
        }
        #endregion
        #region Method: void Insert(iPos, s)
        public void Insert(int iPos, string s)
        {
            Text = Text.Insert(iPos, s);
        }
        #endregion
    }
}
