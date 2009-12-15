#region ***** WritingSystem.cs *****
/**********************************************************************************************
 * Project: Our Word!
 * File:    WritingSystem.cs
 * Author:  John Wimbish
 * Created: 12 Dec 2009
 * Purpose: Information OurWord needs about writing systems
 * Legal:   Copyright (c) 2005-09, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
using System.Diagnostics;
using System.Xml;
using JWTools;
#endregion

namespace OurWordData.Styles
{
    public class WritingSystem
    {
        // Content Attrs ---------------------------------------------------------------------
        #region Attr{g/s}: string Name - serves as the Unique ID
        public string Name
        {
            get
            {
                return m_sName;
            }
            set
            {
                if (m_sName == value)
                    return;
                m_sName = value;
                StyleSheet.DeclareDirty();
            }
        }
        private string m_sName = "";
        #endregion
        #region Attr{g/s}: bool IsIdeaGraph - a glyph equals a word, e.g., Chinese script
        public bool IsIdeaGraph
        {
            get
            {
                return m_bIsIdeaGraph;
            }
            set
            {
                if (m_bIsIdeaGraph == value)
                    return;
                m_bIsIdeaGraph = value;
                StyleSheet.DeclareDirty();
            }
        }
        private bool m_bIsIdeaGraph;
        #endregion
        #region Attr{g/s}: string PunctuationChars - a list of punctuation characters for this WS
        public string PunctuationChars
        {
            get
            {
                return m_sPunctuationChars;
            }
            set
            {
                if (m_sPunctuationChars == value)
                    return;
                m_sPunctuationChars = value;
                StyleSheet.DeclareDirty();
            }
        }
        private string m_sPunctuationChars = c_sDefaultPunctuationChars;
        const string c_sDefaultPunctuationChars = "“‘.,;:<>?!()[]’”";
        #endregion
        #region Attr{g/s}: string EndPunctuationChars - punctuation that occurs sentence-final.
        public string EndPunctuationChars
        {
            get
            {
                return m_sEndPunctuationChars;
            }
            set
            {
                if (m_sEndPunctuationChars == value)
                    return;
                m_sEndPunctuationChars = value;
                StyleSheet.DeclareDirty();
            }
        }
        private string m_sEndPunctuationChars = c_sDefaultEndPunctuationChars;
        const string c_sDefaultEndPunctuationChars = ".>?!)],:;’”";
        #endregion
        #region Attr{g/s}: string KeyboardName - the name of the keyboard to use with this WS
        public string KeyboardName
        {
            get
            {
                return m_sKeyboardName;
            }
            set
            {
                if (m_sKeyboardName == value)
                    return;
                m_sKeyboardName = value;
                StyleSheet.DeclareDirty();
            }
        }
        private string m_sKeyboardName = "";
        #endregion

        // AutoHyphen Attrs
        #region Attr{g/s}: string Consonants - a list of Consonants for this WS
        public string Consonants
        {
            get
            {
                return m_sConsonants;
            }
            set
            {
                if (m_sConsonants == value)
                    return;
                m_sConsonants = value;
                StyleSheet.DeclareDirty();
            }
        }
        private string m_sConsonants = c_sDefaultConsonants;
        const string c_sDefaultConsonants = "bcdfghjklmnpqrstvwxyz";
        #endregion
        #region Attr{g/s}: bool UseAutomatedHyphenation
        public bool UseAutomatedHyphenation
        {
            get
            {
                return m_bUseAutomatedHyphenation;
            }
            set
            {
                if (m_bUseAutomatedHyphenation == value)
                    return;
                m_bUseAutomatedHyphenation = value;
                StyleSheet.DeclareDirty();
            }
        }
        private bool m_bUseAutomatedHyphenation;
        #endregion
        #region Attr{g/s}: string HyphenationCVPattern - E.g., "V-C", or "VC-CV"
        public string HyphenationCVPattern
        {
            get
            {
                return m_sHyphenationCVPattern;
            }
            set
            {
                if (m_sHyphenationCVPattern == value)
                    return;
                m_sHyphenationCVPattern = value;
                StyleSheet.DeclareDirty();
            }
        }
        private string m_sHyphenationCVPattern = c_sHyphenationCVPattern;
        const string c_sHyphenationCVPattern = "V-C";
        #endregion
        #region Attr{g/s}: int MinHyphenSplit - points of space before a paragraph
        public int MinHyphenSplit
        {
            get
            {
                return m_nMinHyphenSplit;
            }
            set
            {
                if (m_nMinHyphenSplit == value)
                    return;
                m_nMinHyphenSplit = value;
                StyleSheet.DeclareDirty();
            }
        }
        private int m_nMinHyphenSplit = c_DefaultMinHiphenSplit;
        private const int c_DefaultMinHiphenSplit = 3;
        #endregion

        // AutoReplace Attrs
        #region Attr{g}: BStringArray AutoReplaceSource
        public BStringArray AutoReplaceSource
        {
            get
            {
                Debug.Assert(null != m_bsaAutoReplaceSource);
                return m_bsaAutoReplaceSource;
            }
        }
        private readonly BStringArray m_bsaAutoReplaceSource;
        #endregion
        #region Attr{g}: BStringArray AutoReplaceResult
        public BStringArray AutoReplaceResult
        {
            get
            {
                Debug.Assert(null != m_bsaAutoReplaceResult);
                return m_bsaAutoReplaceResult;
            }
        }
        private readonly BStringArray m_bsaAutoReplaceResult;
        #endregion

        // Scaffolding -----------------------------------------------------------------------
        public const string DefaultWritingSystemName = "Latin";
        #region Constructor()
        public WritingSystem()
        {
            m_bsaAutoReplaceSource = new BStringArray();
            m_bsaAutoReplaceResult = new BStringArray();
        }
        #endregion

        // I/O & Merge -----------------------------------------------------------------------
        #region I/O Constants
        protected const string c_sTag = "WritingSystem";

        private const string c_sAttrName = "Name";
        private const string c_sAttrIdeaGraph = "IdeaGraph";
        private const string c_sAttrPunctuationCharacters = "Punct";
        private const string c_sAttrEndPunctuationCharacters = "EndPunct";
        private const string c_sAttrKeyboardName = "Keyboard";

        private const string c_sAttrConsonants = "Consonants";
        private const string c_sAttrUseAutoHyphenation = "UseAutoHyph";
        private const string c_sAttrAutoHyphenationCVPattern = "AutoHyphCV";
        private const string c_sAttrAutoHyphenationMinSplit = "AutoHyphMinSplit";

        private const string c_sAttrAutoReplaceSource = "AutoReplaceSource";
        private const string c_sAttrAutoReplaceResult = "AutoReplaceResult";
        #endregion
        #region Method: XmlNode Save(XmlDoc, nodeParent)
        public XmlNode Save(XmlDoc doc, XmlNode nodeParent)
        {
            var nodeFont = doc.AddNode(nodeParent, c_sTag);

            doc.AddAttr(nodeFont, c_sAttrName, Name);
            doc.AddAttr(nodeFont, c_sAttrIdeaGraph, IsIdeaGraph);
            doc.AddAttr(nodeFont, c_sAttrPunctuationCharacters, PunctuationChars);
            doc.AddAttr(nodeFont, c_sAttrEndPunctuationCharacters, EndPunctuationChars);
            doc.AddAttr(nodeFont, c_sAttrKeyboardName, KeyboardName);

            doc.AddAttr(nodeFont, c_sAttrConsonants, Consonants);
            doc.AddAttr(nodeFont, c_sAttrUseAutoHyphenation, UseAutomatedHyphenation);
            doc.AddAttr(nodeFont, c_sAttrAutoHyphenationCVPattern, HyphenationCVPattern);
            doc.AddAttr(nodeFont, c_sAttrAutoHyphenationMinSplit, MinHyphenSplit);

            doc.AddAttr(nodeFont, c_sAttrAutoReplaceSource, AutoReplaceSource.SaveLine);
            doc.AddAttr(nodeFont, c_sAttrAutoReplaceResult, AutoReplaceResult.SaveLine);

            return nodeFont;
        }
        #endregion
        #region SMethod: WritingSystem Create(node)
        static public WritingSystem Create(XmlNode node)
        {
            if (node.Name != c_sTag)
                return null;

            var ws = new WritingSystem
            {
                Name = XmlDoc.GetAttrValue(node, c_sAttrName, DefaultWritingSystemName),
                IsIdeaGraph = XmlDoc.GetAttrValue(node, c_sAttrIdeaGraph,false),
                PunctuationChars = XmlDoc.GetAttrValue(node, c_sAttrPunctuationCharacters, 
                    c_sDefaultPunctuationChars),
                EndPunctuationChars = XmlDoc.GetAttrValue(node, c_sAttrEndPunctuationCharacters, 
                    c_sDefaultEndPunctuationChars),
                KeyboardName = XmlDoc.GetAttrValue(node, c_sAttrKeyboardName, ""),

                Consonants = XmlDoc.GetAttrValue(node, c_sAttrConsonants, c_sDefaultConsonants),
                UseAutomatedHyphenation = XmlDoc.GetAttrValue(node, c_sAttrUseAutoHyphenation, 
                    false),
                HyphenationCVPattern = XmlDoc.GetAttrValue(node, c_sAttrAutoHyphenationCVPattern,
                    c_sHyphenationCVPattern),
                MinHyphenSplit = XmlDoc.GetAttrValue(node, c_sAttrAutoHyphenationMinSplit, 
                    c_DefaultMinHiphenSplit)
            };

            var sSource = XmlDoc.GetAttrValue(node, c_sAttrAutoReplaceSource, "");
            ws.AutoReplaceSource.Read(sSource);

            var sResult = XmlDoc.GetAttrValue(node, c_sAttrAutoReplaceResult, "");
            ws.AutoReplaceResult.Read(sResult);

            return ws;
        }
        #endregion
        #region Method: void Merge(parent, theirs)
        public void Merge(WritingSystem parent, WritingSystem theirs)
        {
            Debug.Assert(parent != null);
            Debug.Assert(theirs != null);

            // We require them to all be the same name (that's their Unique ID)
            Debug.Assert(Name == parent.Name);
            Debug.Assert(Name == theirs.Name);

            // Algorithm: We keep theirs iff they differ from ours and ours is
            // unchanged from the parent. Otherwise we always keep ours.
            if (IsIdeaGraph != theirs.IsIdeaGraph && IsIdeaGraph == parent.IsIdeaGraph)
                IsIdeaGraph = theirs.IsIdeaGraph;

            if (PunctuationChars != theirs.PunctuationChars && PunctuationChars == parent.PunctuationChars)
                PunctuationChars = theirs.PunctuationChars;

            if (EndPunctuationChars != theirs.EndPunctuationChars && EndPunctuationChars == parent.EndPunctuationChars)
                EndPunctuationChars = theirs.EndPunctuationChars;

            if (KeyboardName != theirs.KeyboardName && KeyboardName == parent.KeyboardName)
                KeyboardName = theirs.KeyboardName;

            if (Consonants != theirs.Consonants && Consonants == parent.Consonants)
                Consonants = theirs.Consonants;

            if (UseAutomatedHyphenation != theirs.UseAutomatedHyphenation && UseAutomatedHyphenation == parent.UseAutomatedHyphenation)
                UseAutomatedHyphenation = theirs.UseAutomatedHyphenation;

            if (HyphenationCVPattern != theirs.HyphenationCVPattern && HyphenationCVPattern == parent.HyphenationCVPattern)
                HyphenationCVPattern = theirs.HyphenationCVPattern;

            if (MinHyphenSplit != theirs.MinHyphenSplit && MinHyphenSplit == parent.MinHyphenSplit)
                MinHyphenSplit = theirs.MinHyphenSplit;

            if (AutoReplaceSource.SaveLine != theirs.AutoReplaceSource.SaveLine &&
                AutoReplaceSource.SaveLine == parent.AutoReplaceSource.SaveLine)
                AutoReplaceSource.Read(theirs.AutoReplaceSource.SaveLine);

            if (AutoReplaceResult.SaveLine != theirs.AutoReplaceResult.SaveLine &&
                AutoReplaceResult.SaveLine == parent.AutoReplaceResult.SaveLine)
                AutoReplaceResult.Read(theirs.AutoReplaceResult.SaveLine);
        }
        #endregion
    }
}
