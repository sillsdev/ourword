#region ***** DPhrase.cs *****
/**********************************************************************************************
 * Project: Our Word!
 * File:    DPhrase.cs
 * Author:  John Wimbish
 * Created: 31 Jan 2005, move to separate file 19 Jan 2010
 * Purpose: A string of text, all with the same FontStyle
 * Legal:   Copyright (c) 2005-09, John S. Wimbish. All Rights Reserved.  
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
        #region Attr{g/s}: FontStyle FontModification
        public FontStyle FontModification
        {
            get
            {
                return m_FontModification;
            }
            set
            {
                m_FontModification = value;
                DeclareDirty();
            }
        }
        private FontStyle m_FontModification = FontStyle.Regular;
        #endregion
        #region VAttr{g}: bool IsItalic
        public bool IsItalic
        {
            get
            {
                return ((FontModification & FontStyle.Italic) == FontStyle.Italic);
            }
        }
        #endregion
        #region VAttr{g}: bool IsBold
        public bool IsBold
        {
            get
            {
                return ((FontModification & FontStyle.Bold) == FontStyle.Bold);
            }
        }
        #endregion
        #region VAttr{g}: bool IsUnderline
        public bool IsUnderline
        {
            get
            {
                return ((FontModification & FontStyle.Underline) == FontStyle.Underline);
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
                if (IsBold)
                    s = "|b" + s + "|r";
                if (IsItalic)
                    s = "|i" + s + "|r";
                if (IsUnderline)
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
        #region VAttr{g}: DBasicText BasicText
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
            m_FontModification = source.FontModification;
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
            if (other.FontModification != FontModification)
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
