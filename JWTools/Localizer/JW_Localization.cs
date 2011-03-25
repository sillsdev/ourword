#region ***** JW_Localization.cs *****
/**********************************************************************************************
 * Dll:     JWTools
 * File:    JW_Localization.cs
 * Author:  John Wimbish
 * Created: 12 May 2007
 * Purpose: Localization system.
 * Legal:   Copyright (c) 2005-10, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Xml;
#endregion

namespace JWTools
{
    #region CLASS: LocAlternate
    public class LocAlternate
    {
        // Attrs -----------------------------------------------------------------------------
        #region Attr{g}: string Value - the string in the language
        public string Value
        {
            get
            {
                Debug.Assert(null != m_sValue);
                return m_sValue;
            }
        }
        readonly string m_sValue = "";
        #endregion
        #region Attr{g/s}: string ShortcutKey
        public string ShortcutKey
        {
            get
            {
                return m_sShortcutKey;
            }
            set
            {
                m_sShortcutKey = value;
            }
        }
        string m_sShortcutKey = "";
        #endregion
        #region Attr{g/s}: string ToolTip
        public string ToolTip
        {
            get
            {
                return m_sToolTip;
            }
            set
            {
                m_sToolTip = value;
            }
        }
        string m_sToolTip = "";
        #endregion
        #region VAttr{g}: bool HasData
        public bool HasData
        {
            get
            {
                if (!string.IsNullOrEmpty(Value))
                    return true;
                if (!string.IsNullOrEmpty(ShortcutKey))
                    return true;
                if (!string.IsNullOrEmpty(ToolTip))
                    return true;
                return false;
            }
        }
        #endregion

        // Scaffolding -----------------------------------------------------------------------
        #region Constructor(sValue, sKey, sTip)
        public LocAlternate(string sValue, string sKey, string sTip)
        {
            m_sValue = sValue;
            m_sShortcutKey = (sKey ?? "");
            m_sToolTip = (sTip ?? "");
        }
        #endregion

        // I/O -------------------------------------------------------------------------------
        #region CONSTANTS
        private const string c_sValue = "Value";
        private const string c_sKey = "Key";
        private const string c_sTip = "Tip";
        #endregion
        #region Method: void SaveLanguageData(doc, nodeItem)
        public void SaveLanguageData(XmlDoc doc, XmlNode nodeItem)
        {
            doc.AddAttr(nodeItem, c_sValue, Value);
            doc.AddAttr(nodeItem, c_sKey, ShortcutKey);
            doc.AddAttr(nodeItem, c_sTip, ToolTip);
        }
        #endregion
        #region SMethod: LocAlternate Create(XmlNode node)
        static public LocAlternate Create(XmlNode node)
        {
            return new LocAlternate(
                XmlDoc.GetAttrValue(node, c_sValue),
                XmlDoc.GetAttrValue(node, c_sKey),
                XmlDoc.GetAttrValue(node, c_sTip));
        }
        #endregion

        // Determine if Needs Attention ------------------------------------------------------
        #region smethod: bool EndsWithColon(s)
        static bool EndsWithColon(string s)
        {
            return s.Length > 0 && s[s.Length - 1] == ':';
        }
        #endregion
        #region smethod: bool EndsWithEllipsis(s)
        static bool EndsWithEllipsis(string s)
        {
            if (s.Length < 3)
                return false;

            return s.Substring(s.Length - 3) == "...";
        }
        #endregion
        #region smethod: int ParameterCount(s)
        static int ParameterCount(string s)
        {
            var c = 0;

            for (var i = 0; i < s.Length - 2; i++)
            {
                var ch1 = s[i];
                var ch2 = s[i + 1];
                var ch3 = s[i + 2];
                if (ch1 == '{' && Char.IsDigit(ch2) && ch3 == '}')
                    c++;
            }

            return c;
        }
        #endregion
        #region smethod: bool HasAmpersand(string s)
        static bool HasAmpersand(string s)
        {
            return s.IndexOf('&') != -1;
        }

        #endregion
        #region Method: string NeedsAttention(LocItem item)
        public string NeedsAttention(LocItem item)
            // Returns empty string if OK, otherwise, a string indicating the problem
        {
            // Is there a value?
            if (string.IsNullOrEmpty(Value))
                return "There is no Value in this language";

            // Colon handled?
            if (EndsWithColon(item.English) && !EndsWithColon(Value))
                return "The Value needs to end with a colon \":\")";
            if (!EndsWithColon(item.English) && EndsWithColon(Value))
                return "The Value should not end with a colon \":\")";

            // Ellipsis handled?
            if (EndsWithEllipsis(item.English) && !EndsWithEllipsis(Value))
                return "The Value needs to end with a ellipsis \"...\")";
            if (!EndsWithEllipsis(item.English) && EndsWithEllipsis(Value))
                return "The Value should not end with a ellipsis \"...\")";

            // Parameter count
            if (ParameterCount(item.English) != ParameterCount(Value))
            {
                return "The English and the Value need to have the same number of " +
                    "parameters, e.g., {0}, {1}, etc.";
            }

            // Ampersands
            if (HasAmpersand(item.English) && !HasAmpersand(Value))
            {
                return "This Value needs an ampersand to indicate which letter is the " +
                    "shortcut within the menu.";
            }

            return "";
        }
        #endregion
    }
    #endregion

    #region CLASS: LocItem
    public class LocItem
    {
        // Attrs -----------------------------------------------------------------------------
        #region Attr{g}: string ID - a unique identifier of the string
        public string ID
        {
            get
            {
                Debug.Assert(!string.IsNullOrEmpty(m_sID));
                return m_sID;
            }
        }
        readonly string m_sID;
        #endregion
        #region Attr{g/s}: string English - the string in English, always available
        public string English
        {
            get
            {
                Debug.Assert(!string.IsNullOrEmpty(m_sEnglish));
                return m_sEnglish;
            }
            set
            {
                m_sEnglish = value;
            }
        }
        string m_sEnglish = "";
        #endregion
        #region Attr{g/s}: string Information - additional instructions for the localizer
        public string Information
        {
            get
            {
                return m_sInformation;
            }
            set
            {
                m_sInformation = value;
            }
        }
        string m_sInformation = "";
        #endregion
        #region Attr{g/s}: bool CanHaveShortcutKey
        public bool CanHaveShortcutKey { get; private set; }
        #endregion
        #region Attr{g/s}: string ShortcutKey - the string in English, always available
        public string ShortcutKey
        {
            get
            {
                return m_sShortcutKey;
            }
            set
            {
                m_sShortcutKey = value;
            }
        }
        string m_sShortcutKey = "";
        #endregion
        #region Attr{g/s}: bool CanHaveToolTip
        public bool CanHaveToolTip
        {
            get
            {
                return !string.IsNullOrEmpty(ToolTip) || m_bCanHaveToolTip;
            }
            private set
            {
                m_bCanHaveToolTip = value;
            }
        }
        bool m_bCanHaveToolTip;
        #endregion
        #region Attr{g/s}: string ToolTip
        public string ToolTip
        {
            get
            {
                return m_sToolTip;
            }
            set
            {
                m_sToolTip = value;
            }
        }
        string m_sToolTip = "";
        #endregion

        // Alternaties in each language (other than English) ---------------------------------
        #region Attr{g}: LocAlternate[] Alternates
        public LocAlternate[] Alternates
        {
            get
            {
                Debug.Assert(null != m_vAlternates);
                return m_vAlternates;
            }
        }
        LocAlternate[] m_vAlternates;
        #endregion

        #region Method: LocAlternate GetAlternate(int iIndex)
        public LocAlternate GetAlternate(int iIndex)
        {
            // Make certain the index is within range
            if (iIndex < 0 || iIndex >= Alternates.Length)
                return null;

            // Retrieve the Alternate
            return Alternates[iIndex];
        }
        #endregion
        #region Method: void AddAlternate(iIndex, LocAlternate)
        public void AddAlternate(int iIndex, LocAlternate alt)
        {
            // Extend the vector if we need to
            if (iIndex >= Alternates.Length )
            {
                var v = new LocAlternate[iIndex + 1];
                for (var i = 0; i < Alternates.Length; i++)
                    v[i] = Alternates[i];
                m_vAlternates = v;
            }

            // Place the value in the appropriate position
            Alternates[iIndex] = alt;
        }
        #endregion

        // Access, with Primary and Secondary languages --------------------------------------
        #region VAttr{g}: string AltValue
        public string AltValue
        {
            get
            {
                // If the primary language is null, then English was intended
                if (null == LocDB.DB.PrimaryLanguage)
                    return English;
                // Otherwise, go for one of our localized languages
                var altPrimary = GetAlternate(LocDB.DB.PrimaryLanguage.Index);
                if (null != altPrimary)
                    return altPrimary.Value;

                // If the secondary language is null, then English was intended
                if (null == LocDB.DB.SecondaryLanguage)
                    return English;
                // Otherwise, go for one of our localized languages
                var altSecondary = GetAlternate(LocDB.DB.SecondaryLanguage.Index);
                if (null != altSecondary)
                    return altSecondary.Value;

                // If here, then the localization did not exist for either Primary or Secondary
                return English;
            }
        }
        #endregion
        #region VAttr{g}: string AltShortcutKey
        public string AltShortcutKey
        {
            get
            {
                // If the primary language is null, then English was intended
                if (null == LocDB.DB.PrimaryLanguage)
                    return ShortcutKey;
                // Otherwise, go for one of our localized languages
                var altPrimary = GetAlternate(LocDB.DB.PrimaryLanguage.Index);
                if (null != altPrimary && !string.IsNullOrEmpty(altPrimary.ShortcutKey))
                     return altPrimary.ShortcutKey;

                // If the secondary language is null, then English was intended
                if (null == LocDB.DB.SecondaryLanguage)
                    return ShortcutKey;
                // Otherwise, go for one of our localized languages
                var altSecondary = GetAlternate(LocDB.DB.SecondaryLanguage.Index);
                if (null != altSecondary && !string.IsNullOrEmpty(altSecondary.ShortcutKey))
                     return altSecondary.ShortcutKey;

                // If here, then the localization did not exist for either Primary or Secondary
                return ShortcutKey;
            }
        }
        #endregion
        #region VAttr{g}: string AltToolTip
        public string AltToolTip
        {
            get
            {
                // If the primary language is null, then English was intended
                if (null == LocDB.DB.PrimaryLanguage)
                    return ToolTip;
                // Otherwise, go for one of our localized languages
                var altPrimary = GetAlternate(LocDB.DB.PrimaryLanguage.Index);
                if (null != altPrimary && !string.IsNullOrEmpty(altPrimary.ToolTip))
                    return altPrimary.ToolTip;

                // If the secondary language is null, then English was intended
                if (null == LocDB.DB.SecondaryLanguage)
                    return ToolTip;
                // Otherwise, go for one of our localized languages
                var altSecondary = GetAlternate(LocDB.DB.SecondaryLanguage.Index);
                if (null != altSecondary && !string.IsNullOrEmpty(altSecondary.ToolTip))
                    return altSecondary.ToolTip;

                // If here, then the localization did not exist for either Primary or Secondary
                return ToolTip;
            }
        }
        #endregion

        // I/O -------------------------------------------------------------------------------
        #region CONSTANTS
        public const string c_sTag = "Item";
        public const string c_sID = "ID";
        public const string c_sEnglish = "English";
        private const string c_sInformation = "Info";
        private const string c_sTip = "Tip";
        private const string c_sKey = "Key";
        private const string c_sCanHaveShortcutKey = "CanKey";
        private const string c_sCanHaveTooltip = "CanTip";
        #endregion
        #region Method: void ReadLanguageData(nodeItem, LocLanguage)
        public void ReadLanguageData(XmlNode nodeItem, LocLanguage language)
        {
            var alt = LocAlternate.Create(nodeItem);
            if (null != alt)
                AddAlternate(language.Index, alt);
        }
        #endregion
        #region Method: void Save(doc, nodeGroup)
        public void Save(XmlDoc doc, XmlNode nodeGroup)
        {
            var nodeItem = doc.AddNode(nodeGroup, c_sTag);

            doc.AddAttr(nodeItem, c_sID, ID);
            doc.AddAttr(nodeItem, c_sEnglish, English);
            doc.AddAttr(nodeItem, c_sCanHaveShortcutKey, CanHaveShortcutKey);
            doc.AddAttr(nodeItem, c_sKey, ShortcutKey);
            doc.AddAttr(nodeItem, c_sCanHaveTooltip, CanHaveToolTip);
            doc.AddAttr(nodeItem, c_sTip, ToolTip);
            doc.AddAttr(nodeItem, c_sInformation, Information);
        }
        #endregion
        #region Method: void SaveLanguageData(XmlField xmlParent, LocLanguage)
        public void SaveLanguageData(XmlDoc doc, XmlNode nodeParent, LocLanguage lang)
        {
            var alt = GetAlternate(lang.Index);
            if (null == alt || !alt.HasData)
                return;

            var nodeItem = doc.AddNode(nodeParent, c_sTag);
            doc.AddAttr(nodeItem, c_sID, ID);

            alt.SaveLanguageData(doc, nodeItem);
        }
        #endregion
        #region SMethod: LocItem Create(nodeItem)
        static public LocItem Create(XmlNode nodeItem)
        {
            if (!XmlDoc.IsNode(nodeItem, c_sTag))
                return null;

            var item = new LocItem(XmlDoc.GetAttrValue(nodeItem, c_sID)) {
                English = XmlDoc.GetAttrValue(nodeItem, c_sEnglish),
                CanHaveShortcutKey = XmlDoc.GetAttrValue(nodeItem, c_sCanHaveShortcutKey, false),
                ShortcutKey = XmlDoc.GetAttrValue(nodeItem, c_sKey),
                CanHaveToolTip = XmlDoc.GetAttrValue(nodeItem, c_sCanHaveTooltip, false),
                ToolTip = XmlDoc.GetAttrValue(nodeItem, c_sTip),
                Information = XmlDoc.GetAttrValue(nodeItem, c_sInformation),
            };

            return item;
        }
        #endregion

        // Scaffolding -----------------------------------------------------------------------
        #region Constructor(sID)
        public LocItem(string sId)
        {
            CanHaveShortcutKey = false;
            m_sID = sId;

            m_vAlternates = new LocAlternate[0];
        }
        #endregion
    }
    #endregion

    #region CLASS: LocGroup
    public class LocGroup
    {
        // Attrs -----------------------------------------------------------------------------
        #region Attr{g}: string ID - a unique identifier of the Group
        public string ID
        {
            get
            {
                Debug.Assert(!string.IsNullOrEmpty(m_sID));
                return m_sID;
            }
        }
        readonly string m_sID;
        #endregion
        #region Attr{g/s}: string Description - a description for the Group
        public string Description
        {
            get
            {
                return m_sDescription;
            }
            set
            {
                m_sDescription = value;
            }
        }
        string m_sDescription = "";
        #endregion
        #region Attr{g}: string Title - The title of the group....user-friendly in the database
        public string Title
        {
            get
            {
                Debug.Assert(!string.IsNullOrEmpty(m_sTitle));
                return m_sTitle;
            }
        }
        readonly string m_sTitle;
        #endregion
        #region Attr{g/s}: bool TranslatorAudience - If F, Advisor is the primary audience for these terms
        public bool TranslatorAudience { get; private set; }
        #endregion

        // Items -----------------------------------------------------------------------------
        #region Attr{g}: LocItem[] Items
        public LocItem[] Items
        {
            get
            {
                Debug.Assert(null != m_vItems);
                return m_vItems;
            }
        }
        LocItem[] m_vItems = null;
        #endregion
        #region Method: LocItem AppendItem(LocItem)
        public LocItem AppendItem(LocItem item)
        {
            LocItem[] v = new LocItem[Items.Length + 1];

            for (int i = 0; i < Items.Length; i++)
                v[i] = Items[i];

            v[Items.Length] = item;

            m_vItems = v;

            return item;
        }
        #endregion
        #region Method: LocItem Find(sID)
        public LocItem Find(string sID)
        {
            foreach (LocItem item in Items)
            {
                if (item.ID == sID)
                    return item;
            }

            return null;
        }
        #endregion
        #region Method: LocItem FindRecursively(string sID)
        public LocItem FindRecursively(string sID)
        {
            // First, look through the items in this group
            LocItem item = Find(sID);
            if (null != item)
                return item;

            // If unsuccessful, look through the groups owned by this group, and so on
            foreach (LocGroup group in Groups)
            {
                item = group.FindRecursively(sID);
                if (null != item)
                    return item;
            }

            // Unsucessful
            return null;
        }
        #endregion
        #region Method: LocItem FindOrAddItem(sItemID, sEnglish)
        public LocItem FindOrAddItem(string sItemID, string sEnglish)
        {
            LocItem item = Find(sItemID);
            if (null == item)
            {
                item = new LocItem(sItemID);
                item.English = sEnglish;
                AppendItem(item);
            }
            Debug.Assert(null != item);
            return item;
        }
        #endregion

        // Subgroups -------------------------------------------------------------------------
        #region Attr{g}: LocGroup[] Groups
        public LocGroup[] Groups
        {
            get
            {
                Debug.Assert(null != m_vGroups);
                return m_vGroups;
            }
        }
        LocGroup[] m_vGroups = null;
        #endregion
        #region Method: LocGroup FindGroup(sID)
        public LocGroup FindGroup(string sID)
        {
            foreach (LocGroup group in Groups)
            {
                if (group.ID == sID)
                    return group;
            }
            return null;
        }
        #endregion
        #region Method: void _AppendGroup(LocGroup group)
        void _AppendGroup(LocGroup group)
        {
            if (null == group)
                return;

            LocGroup[] v = new LocGroup[Groups.Length + 1];
            for (int i = 0; i < Groups.Length; i++)
                v[i] = Groups[i];
            v[Groups.Length] = group;
            m_vGroups = v;
        }
        #endregion
        #region Method: LocGroup FindOrAddGroup(sGroupID)
        public LocGroup FindOrAddGroup(string sGroupID)
        {
            LocGroup group = FindGroup(sGroupID);

            if (null == group)
            {
                group = new LocGroup(sGroupID, sGroupID);
                _AppendGroup(group);
            }

            Debug.Assert(null != group);

            return group;
        }
        #endregion

        // I/O -------------------------------------------------------------------------------
        #region Constants
        private const string c_sTag = "Group";
        public const string c_sID = "ID";
        private const string c_sTitle = "Title";
        private const string c_sDescription = "Des";
        private const string c_sTranslatorAudience = "Translator";
        #endregion
        #region Method: void Save(doc, nodeParent)
        public void Save(XmlDoc doc, XmlNode nodeParent)
        {
            var nodeGroup = doc.AddNode(nodeParent, c_sTag);

            doc.AddAttr(nodeGroup, c_sID, ID);
            doc.AddAttr(nodeGroup, c_sTitle, Title);
            doc.AddAttr(nodeGroup, c_sDescription, Description);
            doc.AddAttr(nodeGroup, c_sTranslatorAudience, TranslatorAudience);

            foreach (var item in Items)
                item.Save(doc, nodeGroup);

            foreach(var subGroup in Groups)
                subGroup.Save(doc, nodeGroup);
        }
        #endregion
        #region Method: void SaveLanguageData(doc, nodeParent, LocLanguage)
        public void SaveLanguageData(XmlDoc doc, XmlNode nodeParent, LocLanguage language)
        {
            var nodeGroup = doc.AddNode(nodeParent, c_sTag);

            doc.AddAttr(nodeGroup, c_sID, ID);

            foreach (var item in Items)
                item.SaveLanguageData(doc, nodeGroup, language);

            foreach (var subGroup in Groups)
                subGroup.SaveLanguageData(doc, nodeGroup, language);

            // If nothing was ever added, then remove this group so we don't save anything;
            // makes the data files more compact and easier to see what's actually be localized.
            if (nodeGroup.ChildNodes.Count == 0)
                nodeParent.RemoveChild(nodeGroup);
        }
        #endregion
        #region SMethod: LocGroup Create(nodeGroup)
        public static LocGroup Create(XmlNode nodeGroup)
        {
            if (!XmlDoc.IsNode(nodeGroup, c_sTag))
                return null;

            var group = new LocGroup(
                XmlDoc.GetAttrValue(nodeGroup, c_sID),
                XmlDoc.GetAttrValue(nodeGroup, c_sTitle)) 
            {
                Description = XmlDoc.GetAttrValue(nodeGroup, c_sDescription),
                TranslatorAudience = XmlDoc.GetAttrValue(nodeGroup, c_sTranslatorAudience, false)
            };

            foreach(XmlNode child in nodeGroup.ChildNodes)
            {
                var item = LocItem.Create(child);
                if (null != item)
                {
                    group.AppendItem(item);
                    continue;
                }

                var subGroup = Create(child);
                if (null != subGroup)
                    group._AppendGroup(subGroup);
            }

            return group;
        }
        #endregion
        #region Method: void ReadLanguageData(nodeGroup, LocLanguage)
        public void ReadLanguageData(XmlNode nodeGroup, LocLanguage language)
        {
            foreach (XmlNode child in nodeGroup)
            {
                if (XmlDoc.IsNode(child, LocItem.c_sTag))
                {
                    var id = XmlDoc.GetAttrValue(child, LocItem.c_sID);
                    var item = Find(id);
                    if (null == item)
                        continue;
                    item.ReadLanguageData(child, language);
                }

                else if (XmlDoc.IsNode(child, c_sTag))
                {
                    var idGroup = XmlDoc.GetAttrValue(child, c_sID);
                    var group = FindGroup(idGroup);
                    if (null == group)
                        continue;
                    group.ReadLanguageData(child, language);
                }
            }
        }
        #endregion


        // Scaffolding -----------------------------------------------------------------------
        #region Constructor(sID, sTitle)
        public LocGroup(string _sID, string _sTitle)
        {
            m_sID = _sID;
            m_sTitle = _sTitle;
            m_vItems = new LocItem[0];
            m_vGroups = new LocGroup[0];
        }
        #endregion
    }
    #endregion

    #region CLASS: LocLanguage
    public class LocLanguage
    {
        // Attrs -----------------------------------------------------------------------------
        #region Attr{g}: string ID - the identifier of the language, e.g., Eng, Inz, Sp.
        public string ID
        {
            get
            {
                Debug.Assert(null != m_sID && m_sID.Length > 0);
                return m_sID;
            }
        }
        string m_sID;
        #endregion
        #region Attr{g}: string Name - the name of the language, e.g., "Bahasa Indonesia"
        public string Name
        {
            get
            {
                Debug.Assert(null != m_sName && m_sName.Length > 0);
                return m_sName;
            }
        }
        string m_sName;
        #endregion
        #region Attr{g}: int Index - which Alternative in LocItem to retrieve
        public int Index
        {
            get
            {
                return m_iIndex;
            }
        }
        int m_iIndex;
        #endregion
        #region Attr{g}: string FontName - the font for the UI in this language, or empty if Windows chooses
        public string FontName
        {
            get
            {
                return m_sFontName;
            }
            set
            {
                m_sFontName = value;
            }
        }
        string m_sFontName = "";
        #endregion
        #region Attr{g}: int FontSize - the font size fo the UI in this language, or empty if Windows chooses
        public int FontSize
        {
            get
            {
                return m_nFontSize;
            }
            set
            {
                m_nFontSize = value;
            }
        }
        int m_nFontSize = 0;
        #endregion

        // Scaffolding -----------------------------------------------------------------------
        #region Constructor(sID, sName, iIndex)
        public LocLanguage(string _sID, string _sName, int _iIndex)
        {
            Debug.Assert(null != _sID);
            m_sID = _sID;

            Debug.Assert(null != _sName);
            m_sName = _sName;

            m_iIndex = _iIndex;
        }
        #endregion

        // I/O -------------------------------------------------------------------------------
        #region Constants
        private const string c_sID = "ID";
        private const string c_sName = "Name";
        private const string c_sFontName = "Font";
        private const string c_sFontSize = "Size";
        #endregion
        #region SMethod: LocLanguage Create(node, iIndex)
        static public LocLanguage Create(XmlNode node, int iIndex)
        {
            var language = new LocLanguage(
                XmlDoc.GetAttrValue(node, c_sID),
                XmlDoc.GetAttrValue(node, c_sName),
                iIndex) 
                {
                    FontName = XmlDoc.GetAttrValue(node, c_sFontName, SystemFonts.DialogFont.Name),
                    FontSize = XmlDoc.GetAttrValue(node, c_sFontSize, (int)SystemFonts.DialogFont.Size)
                };
            return language;
        }
        #endregion
        #region Method: void Save(doc, node)
        public void Save(XmlDoc doc, XmlNode node)
        {
            doc.AddAttr(node, c_sID, ID);
            doc.AddAttr(node, c_sName, Name);
            doc.AddAttr(node, c_sFontName, FontName);
            doc.AddAttr(node, c_sFontSize, FontSize);
        }
        #endregion
    }
    #endregion

    #region CLASS: LocDB
    public class LocDB
    {
        // Groups ----------------------------------------------------------------------------
        #region Attr{g}: LocGroup[] Groups
        public LocGroup[] Groups
        {
            get
            {
                Debug.Assert(null != m_vGroups);
                return m_vGroups;
            }
        }
        LocGroup[] m_vGroups;
        #endregion
        #region Method: LocGroup FindGroup(sID)
        public LocGroup FindGroup(string sID)
        {
            foreach (LocGroup group in Groups)
            {
                if (group.ID == sID)
                    return group;

                LocGroup sub = group.FindGroup(sID);
                if (null != sub)
                    return sub;
            }
            return null;
        }
        #endregion
        #region Method: LocGroup ParseAndAddGroup(string sTitle)
        LocGroup ParseAndAddGroup(string sTitle)
        {
            // Build the ID from the string, getting rid of digits, whitespace and punctuation
            string sID = "";
            foreach (char c in sTitle)
            {
                if (Char.IsDigit(c))
                    continue;
                if (Char.IsWhiteSpace(c))
                    continue;
                if (Char.IsPunctuation(c))
                    continue;

                sID += c;
            }
            if (sID.Length == 0)
                return null;

            // Don't bother if it is the Language record (the first record in the database, generally)
            if (sID == c_Languages)
                return null;

            // See if we already have this group
            LocGroup group = FindGroup(sID);
            if (null != group)
                return group;

            // If not, create a new one and add it to the vector
            group = new LocGroup(sID, sTitle);
            AppendGroup(group);
            return group;
        }
        #endregion
        #region Method: void AddGroup(LocGroup group)
        void AppendGroup(LocGroup group)
        {
            if (null == group)
                return;

            LocGroup[] v = new LocGroup[Groups.Length + 1];
            for (int i = 0; i < Groups.Length; i++)
                v[i] = Groups[i];
            v[Groups.Length] = group;
            m_vGroups = v;
        }
        #endregion
        #region Method: LocGroup FindOrAddGroup(sGroupID)
        LocGroup FindOrAddGroup(string sGroupID)
        {
            LocGroup group = FindGroup(sGroupID);
            if (null == group)
            {
                group = new LocGroup(sGroupID, sGroupID);
                AppendGroup(group);
            }
            return group;
        }
        #endregion

        // Special Groups --------------------------------------------------------------------
        const string c_Strings = "Strings";
        const string c_Menus = "Menus";
        const string c_Toolbars = "ToolbarText";
        const string c_Messages = "Messages";
        #region Attr{g}: LocGroup Strings - the Group that contains misc strings
        LocGroup Strings
        {
            get
            {
                if (null == m_Strings)
                    m_Strings = FindGroup(c_Strings);

                Debug.Assert(null != m_Strings);

                return m_Strings;
            }
        }
        LocGroup m_Strings;
        #endregion      
        #region Attr{g}: LocGroup Menus - the Group that contains the menus
        LocGroup Menus
        {
            get
            {
                if (null == m_Menus)
                    m_Menus = FindGroup(c_Menus);

                Debug.Assert(null != m_Menus);

                return m_Menus;
            }
        }
        LocGroup m_Menus;
        #endregion
        #region Attr{g}: LocGroup Toolbars - the Group that contains the Toolbars
        LocGroup Toolbars
        {
            get
            {
                if (null == m_Toolbars)
                    m_Toolbars = FindGroup(c_Toolbars);

                Debug.Assert(null != m_Toolbars);

                return m_Toolbars;
            }
        }
        LocGroup m_Toolbars;
        #endregion
        #region Attr{g}: LocGroup Messages - the Group that contains the Messages
        LocGroup Messages
        {
            get
            {
                if (null == m_Messages)
                    m_Messages = FindGroup(c_Messages);

                Debug.Assert(null != m_Messages);

                return m_Messages;
            }
        }
        LocGroup m_Messages;
        #endregion

        // Languages -------------------------------------------------------------------------
        const string c_Languages = "Languages";  // the first record in the languages database
        const string c_mkrEnglish = "eng";
        #region Attr{g}: LocLanguage Languages
        public LocLanguage[] Languages
        {
            get
            {
                Debug.Assert(null != m_vLanguages);
                return m_vLanguages;
            }
        }
        LocLanguage[] m_vLanguages;
        #endregion
        #region Method: LocLanguage FindLanguage(sID)
        public LocLanguage FindLanguage(string sID)
        {
            foreach (LocLanguage lang in Languages)
            {
                if (lang.ID == sID)
                    return lang;
            }
            return null;
        }
        #endregion
        #region Method: LocLanguage FindLanguageByName(sLanguageName)
        public LocLanguage FindLanguageByName(string sLanguageName)
        {
            if (string.IsNullOrEmpty(sLanguageName))
                return null;

            foreach (LocLanguage lang in Languages)
            {
                if (lang.Name == sLanguageName)
                    return lang;
            }
            return null;
        }
        #endregion
        #region Method: int GetIndexForLanguage(string sLanguageName)
        public int GetIndexForLanguage(string sLanguageName)
        {
            for (int i = 0; i < Languages.Length; i++)
            {
                if (sLanguageName == Languages[i].Name)
                    return i;
            }
            return -1;
        }
        #endregion
        #region Method: void ParseAndAddLanguage(string s)
        void ParseAndAddLanguage(string s)
        {
            // We'll parse the ID and Name into these variables
            string sID = "";
            string sName = "";

            // Work through the string
            bool bIDFinished = false;
            foreach (char c in s)
            {
                if (c == ' ' && !bIDFinished)
                {
                    bIDFinished = true;
                    continue;
                }

                if (bIDFinished)
                    sName += c;
                else
                    sID += c;
            }
            sID = sID.Trim();
            sName = sName.Trim();
            if (sID.Length == 0 || sName.Length == 0)
                return;

            // Don't add if it is English
            if (c_mkrEnglish == sID)
                return;

            // If the language is already present, we're done
            if (FindLanguage(sID) != null)
                return;

            // Otherwise, create and add the new one
            LocLanguage lang = new LocLanguage(sID, sName, Languages.Length);
            AppendLanguage(lang);
        }
        #endregion
        #region Method: void AppendLanguage(LocLanguage lang)
        void AppendLanguage(LocLanguage lang)
        {
            LocLanguage[] v = new LocLanguage[Languages.Length + 1];
            for (int i = 0; i < Languages.Length; i++)
                v[i] = Languages[i];
            v[Languages.Length] = lang;
            m_vLanguages = v;
        }
        #endregion
        #region Method: LocLanguage AppendLanguage(string sID, string sName)
        public LocLanguage AppendLanguage(string sID, string sName)
        {
            LocLanguage lang = new LocLanguage(sID, sName, Languages.Length);
            AppendLanguage(lang);
            return lang;
        }
        #endregion

        #region Attr{g/s}: LocLanguage PrimaryLanguage
        public LocLanguage PrimaryLanguage
        {
            get
            {
                return m_langPrimary;
            }
            set
            {
                m_langPrimary = value;
            }
        }
        LocLanguage m_langPrimary;
        #endregion
        #region Attr{g/s}: LocLanguage SecondaryLanguage
        public LocLanguage SecondaryLanguage
        {
            get
            {
                return m_langSecondary;
            }
            set
            {
                m_langSecondary = value;
            }

        }
        LocLanguage m_langSecondary;
        #endregion
        #region VAttr{g}: bool PrimaryIsEnglish
        public bool PrimaryIsEnglish
        {
            get
            {
                if (PrimaryLanguage == null)
                    return true;
                return false;
            }
        }
        #endregion

        #region Method: void SetPrimary(string sName)
        public void SetPrimary(string sName)
        {
            PrimaryLanguage = null;

            foreach (LocLanguage lang in Languages)
            {
                if (lang.Name == sName)
                    PrimaryLanguage = lang;
            }
        }
        #endregion
        #region Method: void SetSecondary(string sName)
        public void SetSecondary(string sName)
        {
            SecondaryLanguage = null;

            foreach (LocLanguage lang in Languages)
            {
                if (lang.Name == sName)
                    SecondaryLanguage = lang;
            }
        }
        #endregion

        #region Attr{g}: string[] LanguageChoices - includes English
        public string[] LanguageChoices
        {
            get
            {
                var v = new string[Languages.Length + 1];

                for (var i = 0; i < Languages.Length; i++)
                    v[i] = Languages[i].Name;

                v[Languages.Length] = LocItem.c_sEnglish;

                return v;
            }
        }
        #endregion

        // Xml I/O --------------------------------------------------------------------------
        #region attr{g}: string MasterFilePath - the file containing the basic information
        static private string MasterFilePath
        {
            get
            {
                Debug.Assert(!string.IsNullOrEmpty(s_sMasterFilePath));
                return s_sMasterFilePath;
            }
        }
        static string s_sMasterFilePath;
        #endregion
        #region Attr{g}: string LanguagesFolder - the folder containing all of the loc files
        string LanguagesFolder
        {
            get
            {
                Debug.Assert(!string.IsNullOrEmpty(s_sLanguagesFolder));
                return s_sLanguagesFolder;
            }
        }
        readonly string s_sLanguagesFolder;
        #endregion
        const string c_sTag = "LocDB";
        #region Method: void Save()
        public void Save()
        {
            // Save the Master File
            var doc = new XmlDoc();
            doc.AddXmlDeclaration();
            var node = doc.AddNode(doc, c_sTag);
            foreach(var group in Groups)
                group.Save(doc, node);
            doc.Write(MasterFilePath);

            // Save the Language-Specific Files
            foreach (var lang in Languages)
                SaveLanguageData(lang);
        }
        #endregion
        #region method: void SaveLanguageData(LocLanguage lang)
        void SaveLanguageData(LocLanguage lang)
        {
            // Build the language name
            var sPath = LanguagesFolder + Path.DirectorySeparatorChar + lang.ID + ".xml";

            // Save a backup in the current folder (See Bug0282.)
            JW_Util.CreateBackup(sPath, ".bak");

            // Create and write the xml doc
            var doc = new XmlDoc();
            doc.AddXmlDeclaration();
            var node = doc.AddNode(doc, c_sTag);
            lang.Save(doc, node);
            foreach(var group in Groups)
                group.SaveLanguageData(doc, node, lang);
            doc.Write(sPath);

            // Make a backup to the remote device if enabled. We have to create a file with the
            // date in it so that the BackupSystem has something to copy; we then delete that
            // file as there's no need to keep filling up the disk. (Bug0282)
            var sRemotePath = Path.GetFileNameWithoutExtension(sPath) + " " + 
                DateTime.Today.ToString("yyyy-MM-dd") + ".xml";
            try
            {
                File.Copy(sPath, sRemotePath, true);
                (new BackupSystem(sRemotePath, null)).MakeBackup();
                File.Delete(sRemotePath);
            }
            catch (Exception)
            {
            }
        }
        #endregion
        #region method: void Read()
        private void Read()
        {
            // Read the Master File
            var doc = new XmlDoc();
            doc.Load(MasterFilePath);
            var node = XmlDoc.FindNode(doc, c_sTag);
            foreach(XmlNode child in node.ChildNodes)
            {
                var group = LocGroup.Create(child);
                if (null != group)
                    AppendGroup(group);
            }

            // Read the Language-Specific Files
            var sPaths = Directory.GetFiles(LanguagesFolder, "*.xml", SearchOption.TopDirectoryOnly);
            foreach (var s in sPaths)
            {
                if (s == MasterFilePath)
                    continue;
                ReadLanguageData(s);
            }
        }
        #endregion
        #region Method: void ReadLanguageData(string sPath)
        void ReadLanguageData(string sPath)
        {
            var doc = new XmlDoc();
            doc.Load(sPath);
            var node = XmlDoc.FindNode(doc, c_sTag);

            // Get which language we're reading
            var language = LocLanguage.Create(node, Languages.Length);
            if (null == language)
                return;
            AppendLanguage(language);

            // Get the data
            foreach(XmlNode nodeGroup in node.ChildNodes)
            {
                var idGroup = XmlDoc.GetAttrValue(nodeGroup, LocGroup.c_sID);
                var group = FindGroup(idGroup);
                if (null == group)
                    continue;
                group.ReadLanguageData(nodeGroup, language);
            }
        }
        #endregion

        // Scaffolding -----------------------------------------------------------------------
        #region SMethod: void Initialize(string sFolderContainingLocalizationFiles)
        static public void Initialize(string sFolderContainingLocalizationFiles)
        {
            if (null == s_LocDB)
                s_LocDB = new LocDB(sFolderContainingLocalizationFiles);
            Debug.Assert(null != s_LocDB);
        }
        #endregion
        #region SAttr{g}: bool IsInitialized
        static public bool IsInitialized
        {
            get
            {
                return (null != s_LocDB);
            }
        }
        #endregion
        #region SAttr{g}: LocDB DB
        static public LocDB DB
        {
            get
            {
                Debug.Assert(null != s_LocDB);
                return s_LocDB;
            }
        }
        static private LocDB s_LocDB;
        #endregion
        #region private Constructor(sFolderContainingLocalizationFiles) - do not call (called by the Initialize Method above
        private LocDB(string sFolderContainingLocalizationFiles)
        {
            // Initialize the vectors
            m_vGroups = new LocGroup[0];
            m_vLanguages = new LocLanguage[0];

            // Folder containing the localization data
            if (!Directory.Exists(sFolderContainingLocalizationFiles))
                Directory.CreateDirectory(sFolderContainingLocalizationFiles);

            // The main localization file
            s_sLanguagesFolder = sFolderContainingLocalizationFiles;
            s_sMasterFilePath = Path.Combine(LanguagesFolder, "OurWordLocalization.xml");

            // Read in the file
            Read();
        }
        #endregion

        // Messages --------------------------------------------------------------------------
        #region Method: string Insert(string sBase, string[] vsInsert)
        static public string Insert(string sBase, string[] v)
        {
            if (null == v || v.Length == 0)
                return sBase;

            string sReturn = sBase;

            for (int i = 0; i < v.Length; i++)
            {
                int iPos = sReturn.IndexOf("{" + i.ToString() + "}");
                if (iPos >= 0)
                {
                    string sFirst = sReturn.Substring(0, iPos);
                    string sLast = sReturn.Substring(iPos + 3);
                    sReturn = sFirst + v[i] + sLast;
                }
            }

            return sReturn;
        }
        #endregion
        public enum MessageTypes { Warning, WarningYN, YN, Info, Error };
        #region SMethod: bool Message(sID, sDefaultEnglish, vsInsertions, MessageType)
        static public bool Message(
            string sID, 
            string sDefaultEnglish, 
            string[] vsInsertions, 
            MessageTypes MessageType)
        {
            s_sLastMessageID = sID;
            
            // Retrieve the localized form of the message
            LocItem item = DB.Messages.Find(sID);
            if (null == item)
            {
                item = new LocItem(sID);
                item.English = sDefaultEnglish;
                DB.Messages.AppendItem(item);
            }
            string sMessageText = item.AltValue;

            // Perform the insertions
            sMessageText = Insert(sMessageText, vsInsertions);

            // Decide which button(s) and which icon to show in the message box
            MessageBoxButtons buttons = MessageBoxButtons.OK;
            MessageBoxIcon icon = MessageBoxIcon.Warning;
            switch (MessageType)
            {
                case MessageTypes.Warning:
                    buttons = MessageBoxButtons.OK;
                    icon = MessageBoxIcon.Warning;
                    break;
                case MessageTypes.WarningYN:
                    buttons = MessageBoxButtons.YesNo;
                    icon = MessageBoxIcon.Warning;
                    break;
                case MessageTypes.YN:
                    buttons = MessageBoxButtons.YesNo;
                    icon = MessageBoxIcon.Question;
                    break;
                case MessageTypes.Info:
                    buttons = MessageBoxButtons.OK;
                    icon = MessageBoxIcon.Information;
                    break;
                case MessageTypes.Error:
                    buttons = MessageBoxButtons.OK;
                    icon = MessageBoxIcon.Error;
                    break;
            }

            // Retrieve the application's title (localized)
            // TODO
            string sAppTitle = "Our Word";

            // Finally, we can show the message
            // Note: We don't give a parent window, because some messages can happen
            // during loading, before a window is available, in which case we get an
            // error, because Form.ActiveForm would return the splash screen, which is
            // running in a different process.
            DialogResult result = MessageBox.Show(null, sMessageText, sAppTitle, buttons, icon);
            return (result == DialogResult.Yes);
        }
        #endregion
        #region SAttr{g/s}: bool SuppressMessages - turns off displaying the msgs (e..g, for testing)
        static public bool SuppressMessages
        {
            get 
            { 
                return s_bSupressMessages; 
            }
            set 
            { 
                s_bSupressMessages = value; 
            }
        }
        static bool s_bSupressMessages = false;
        #endregion
        #region SAttr{g}: string LastMessageID
        static public string LastMessageID
        {
            get
            {
                return s_sLastMessageID;
            }
        }
        static string s_sLastMessageID = "";
        #endregion
        #region Method: void Reset() - clear out previous messages remembering
        static public void Reset()
        {
            s_sLastMessageID = "";
            SuppressMessages = false;
        }
        #endregion

        // Localize forms, user controls, tool strips; groupings of controls ----------------
        #region SMethod: void Localize(UserControl, PropertyBag)
        static public void Localize(UserControl uc, PropertyBag bag)
        {
            // Replace the appropriate PropertySpec attrs with their localized forms
            foreach (PropertySpec ps in bag.Properties)
            {
                // The Property's Name
                if (!ps.DontLocalizeName)
                    ps.Name = GetValue(uc, ps.ID, ps.Name, null);

                // The Property's Help / Description
                if (!ps.DontLocalizeHelp)
                    ps.Description = GetToolTip(uc, ps.ID, ps.Name, ps.Description);

                // The Property's Category, if applicable
                if (!string.IsNullOrEmpty(ps.Category) && !ps.DontLocalizeCategory)
                    ps.Category = GetValue(uc, ps.Category, ps.Category, null);

                // The Property's string enumerations, if applicable
                if (null != ps.EnumValues && !ps.DontLocalizeEnums)
                {
                    // YesNoPropertySpec is located in Common area
                    if (ps as YesNoPropertySpec != null)
                    {
                        ps.EnumValues[0] = GetValue(null, "kYes", "Yes", null, null);
                        ps.EnumValues[1] = GetValue(null, "kNo", "No", null, null);
                    }

                    // All others
                    else
                    {
                        for (int i = 0; i < ps.EnumValues.Length; i++)
                        {
                            // Default item is just appended to the property
                            string sItem = ps.ID + "_" + i.ToString();

                            // For an enumeration, we use, e.g., kLeft, kCentered, etc.
                            EnumPropertySpec eps = ps as EnumPropertySpec;
                            if (null != eps)
                                sItem = "option_" + eps.EnumIDs[i];

                            string sEnumValue = ps.EnumValues[i];

                            ps.EnumValues[i] = GetValue(uc, sItem, sEnumValue, null);

                            if ((string)ps.DefaultValue == sEnumValue)
                                ps.DefaultValue = GetValue(uc, sItem, sEnumValue, null);
                        }
                    }
                }

                // Sub-Bags (e.g., the FontBag in the StyleSheet)
                PropertyBag SubBag = ps.DefaultValue as PropertyBag;
                if (null != SubBag)
                    Localize(uc, SubBag);
            }
        }
        #endregion
        #region SMethod: void Localize(ToolStrip)
        static public void Localize(ToolStrip strip)
        {
            foreach (ToolStripItem tsi in strip.Items)
                Localize(tsi);
        }
        #endregion
        #region SMethod: void Localize(ToolStripItem)
        static public void Localize(ToolStripItem tsi)
        {
            // Visual Studio Designer chokes otherwise
            if (!IsInitialized)
                return;

            // Certain controls are not localized
            if (tsi as ToolStripSeparator != null)
                return;

            // The ID is the name of the item
            var sItemID = tsi.Name;
            if (string.IsNullOrEmpty(sItemID))
                return;

            // Calculate/retrieve the group ID
            var vGroupID = _GetGroupID(tsi);

            // Get the ToolStripItem's text value
            tsi.Text = GetValue(
                vGroupID,
                sItemID,
                tsi.Text,
                null,
                null);

            // Get its tooltip
            tsi.ToolTipText = GetToolTip(
                vGroupID,
                sItemID,
                tsi.Text,
                (DB.PrimaryIsEnglish ? tsi.ToolTipText : null));

            // Get its Shortcut key
            var mi = tsi as ToolStripMenuItem;
            if (null != mi)
            {
                // The ShortcutKeyDisplayString does not always work; we'll just fix
                // the ones that are "Ctrl+"
                if (mi.ShortcutKeyDisplayString == null && mi.ShortcutKeys != Keys.None)
                {
                    if ( (mi.ShortcutKeys & Keys.Control ) == Keys.Control)
                    {
                        var s = "Ctrl+";
                        s += mi.ShortcutKeys.ToString()[0];
                        mi.ShortcutKeyDisplayString = s;
                    }
                }

                mi.ShortcutKeys = GetShortcutKey(
                    vGroupID,
                    sItemID,
                    tsi.Text,
                    (DB.PrimaryIsEnglish ? mi.ShortcutKeyDisplayString : null));
            }
            
            // Recurse if this is a drop-down item
            var tsiDropDown = tsi as ToolStripDropDownItem;
            if (tsiDropDown != null)
            {
                foreach (ToolStripItem tsiChild in tsiDropDown.DropDownItems)
                    Localize(tsiChild);
            }
        }
        #endregion
        #region SMethod: void Localize(ColumnHeader col)
        static void Localize(ColumnHeader col)
        {
            // The Name ID would ideally be the name of the control, but the
            // Name comes in as "" at runtime, regardless of what is placed
            // into the VS editor. So I use an index value instead. Lazy of MS.
            string sItemID = col.ListView.Name + "_col" + col.DisplayIndex.ToString();
            Debug.Assert(!string.IsNullOrEmpty(sItemID));

            string[] vGroupID = _GetGroupID(col);

            col.Text = GetValue(
                vGroupID,
                sItemID,
                col.Text,
                null,
                null);
        }
        #endregion
        #region SMethod: void Localize(ListView, vExclude)
        static public void Localize(ListView lv, Control[] vExclude)
        {
            // Check through the exclude list
            if (_Exclude(lv, vExclude))
                return;

            // All we want to do here is to localize the column headers
            foreach (ColumnHeader col in lv.Columns)
                Localize(col);
        }
        #endregion
        #region SMethod: void Localize(Control, vExclude) - includes UserControls
        static public void Localize(Control ctrl, Control[] vExclude)
        {
            // Recurse for any childrem. Do this prior to the Exclude test, because we don't want an 
            // exclusion to also exclude the children. Don't do UserControls, because we expect them
            // to handle localization in their own onLoad handlers (otherwise, the loc info gets into
            // the xml file twice.) Also don't do PropertyGrids, because we are using PropertyBags
            // instead.
            foreach (Control subCtrl in ctrl.Controls)
            {
                if (subCtrl as UserControl == null && subCtrl as PropertyGrid == null)
                {
                    // For some reason, the compiler is not calling the right version of
                    // localize, so I have to force it via a type cast.
                    if (null != subCtrl as ListView)
                        Localize(subCtrl as ListView, vExclude);
                    else
                        Localize(subCtrl, vExclude);
                }
            }

            // Check through the exclude list
            if (_Exclude(ctrl, vExclude))
                return;

            // If the control has no text, then it isn't meant to be localized (e..g,
            // a combo box.)
            if (string.IsNullOrEmpty(ctrl.Text))
                return;

            // Get the address of the containing group
            string[] vGroupID = _GetGroupID(ctrl);

            // The name of the control is the ItemID
            string sItemID = ctrl.Name;
            if (sItemID == null || sItemID.Length == 0)
                return;

            // Get the control's text
            ctrl.Text = GetValue(
                vGroupID,
                sItemID,
                ctrl.Text,
                null,
                null);
        }
        #endregion
        #region SMethod: void Localize(Form, vExclude)
        static public void Localize(Form form, Control[] vExclude)
        {
            // Localize the form's title
            form.Text = GetValue(form, "Title", form.Text);

            // Loop through the controls in the form
            foreach (Control ctrl in form.Controls)
                Localize(ctrl, vExclude);
        }
        #endregion

        // Private helper methods for Localize and Retrieval Methods -------------------------
        #region SMethod: string[] _GetGroupID(Object obj)
        static string[] _GetGroupID(Object obj)
        {
            string sErrorMessage = " must have a name to serve as the GroupID for localization";

            Debug.Assert(null != obj);

            // Form
            Form form = obj as Form;
            if (null != form)
            {
                Debug.Assert(!string.IsNullOrEmpty(form.Name), "Form" + sErrorMessage);
                return new string[] { form.Name };
            }

            // User Control
            UserControl uc = obj as UserControl;
            if (null != uc)
            {
                string[] vFormGroupID = _GetGroupID(uc.ParentForm);
                Debug.Assert(!string.IsNullOrEmpty(uc.Name), "User Control" + sErrorMessage);
                return new string[] {
                    vFormGroupID[0],
                    vFormGroupID[0] + "_" + uc.Name
                    };
            }

            // ToolStrip
            ToolStrip toolstrip = obj as ToolStrip;
            if (null != toolstrip)
            {
                // If we are nested, then get the top-level ToolStrip
                if (null != toolstrip.Parent as ToolStrip)
                    return _GetGroupID(toolstrip.Parent);

                // Otherwise, we are at the top level, so return its name
                Debug.Assert(!string.IsNullOrEmpty(toolstrip.Name), "ToolStrip" + sErrorMessage);
                return new string[] { toolstrip.Name };
            }

            // ToolStripItem
            ToolStripItem tsi = obj as ToolStripItem;
            if (null != tsi)
            {
                // Work up the ownership chain while we encounter ToolStripItems
                if (null != tsi.OwnerItem && (tsi.OwnerItem as ToolStripOverflowButton == null))
                    return _GetGroupID(tsi.OwnerItem);
                // Once we get here, we should be at the owning ToolStrip
                Debug.Assert(null != tsi.Owner);
                return _GetGroupID(tsi.Owner); 
            }

            // ColumnHeader
            ColumnHeader col = obj as ColumnHeader;
            if (null != col)
            {
                ListView lv = col.ListView;
                if (null != lv)
                    return _GetGroupID( lv.Parent );
            }

            // Other type of control
            Control ctrl = obj as Control;
            if (null != ctrl)
                return _GetGroupID(ctrl.Parent);

            Debug.Assert(false);
            return null;
        }
        #endregion
        #region SMethod: bool _Exclude(Control ctrl, Control[] vExcludeList)
        static bool _Exclude(Control ctrl, Control[] vExcludeList)
        {
            if (null == vExcludeList)
                return false;

            foreach (Control ctrlExclude in vExcludeList)
            {
                if (ctrlExclude == ctrl)
                    return true;
            }
            return false;
        }
        #endregion
        #region SMethod: bool _Exclude(string sItem, string[] vExcludeList)
        static bool _Exclude(string sItem, string[] vsExcludeList)
        {
            if (null == vsExcludeList)
                return false;

            foreach (string s in vsExcludeList)
            {
                if (s == sItem)
                    return true;
            }
            return false;
        }
        #endregion

        // Retrieval of strings from the proper alternate of the LocItem ---------------------
        #region SMethod: LocItem GetLocItem(vGroupID, sItemID, sEnglish)
        static LocItem GetLocItem(
            string[] vGroupID, 
            string sItemID,
            string sEnglish
            )
        {
            // Default to not being able to find it
            LocItem item = null;

            // First, see if it is in the vGroupID path
            LocGroup group = null;
            if (vGroupID != null)
            {
                foreach (string sGroupID in vGroupID)
                {
                    if (group == null)
                        group = DB.FindOrAddGroup(sGroupID);
                    else
                        group = group.FindOrAddGroup(sGroupID);
                }
                item = group.Find(sItemID);
            }

            // If not, see if it is defined in the Strings area
            if (null == item)
            {
                LocGroup gStrings = DB.FindGroup(c_Strings);
                item = gStrings.FindRecursively(sItemID);
            }

            // If not, add it to the vGroupID group
            if (null == item && null != group)
            {
                item = group.FindOrAddItem(sItemID, sEnglish);
            }

            return item;
        }
        #endregion
        // Retrieve a Value ------------------------------------------------------------------
        #region SMethod: string GetValue(vGroupID, sItemID, sEnglish, sLanguage, vsInsert) - Workhorse method
        /// <summary>
        /// Looks up the string according to its GroupID/ItemID, and returns its localized value. 
        /// </summary>
        /// <param name="vGroupID">The path to the group containing the localization item</param>
        /// <param name="sItemID">The ID of the localization item</param>
        /// <param name="sEnglish">The English value, should the language value not be found</param>
        /// <param name="sLanguage">If non-null, the method will return the localization in the 
        /// language requested. Otherwise, the return value is in accordance with the Primary and 
        /// Secondary languages.</param>
        /// <param name="vsInsert">If non-null, these values replace {0}, {1}, ..., in the value string.</param>
        /// <returns>The localized string value, or English if it does not exist in the
        /// requested language.</returns>
        /// <remarks>This is the workhorse method that other methods (with simplified parameters) 
        /// should call. If the item does not exist in the database, then one is added.</remarks>
        static public string GetValue( 
            string[] vGroupID, 
            string sItemID, 
            string sEnglish,
            string sLanguage,
            string[] vsInsert)
        {
            // Find (or add) the item, either in vGroupID, or in the Strings hierarchy
            LocItem item = GetLocItem(vGroupID, sItemID, sEnglish);
            Debug.Assert(null != item);

            // If a language was specified, then attempt to find it; returning English otherwise.
            // We use this, e.g., for the FileNameLanguage
            // TODO: We are assuming that vsInsert is not desired for these, based on current
            //   usage in OurWord. Hence the assertion.
            if (!string.IsNullOrEmpty(sLanguage))
            {
                Debug.Assert(null == vsInsert); // See the TODO above.
                LocLanguage language = LocDB.DB.FindLanguageByName(sLanguage);
                if (null == language)
                    return sEnglish;
                LocAlternate alt = item.GetAlternate(language.Index);
                if (null == alt || string.IsNullOrEmpty(alt.Value))
                    return sEnglish;
                return alt.Value;
            }

            // Was an insertion desired?
            if (null != vsInsert)
                return Insert(item.AltValue, vsInsert);

            // Otherwise, we want to retrieve the string according to the Primary and
            // Secondary languages, as set up in the settings.
            return item.AltValue;
        }
        #endregion
        #region SMethod: string GetValue(Form, sItemID, sEnglish) - gets GroupID from Form
        static public string GetValue(Form form, string sItemID, string sEnglish)
        {
            // Call the workhorse
            return GetValue(
                _GetGroupID(form),
                sItemID,
                sEnglish,
                null,
                null);
        }
        #endregion
        #region SMethod: string GetValue(UserControl, sItemID, sEnglish, vsInsert) - gets GroupID from the UC
        static public string GetValue(UserControl uc, string sItemID, string sEnglish, string[] vsInsert)
        {
            // Call the workhorse
            return GetValue(
                _GetGroupID(uc),
                sItemID,
                sEnglish,
                null,
                vsInsert);
        }
        #endregion
        // Retrieve a ToolTip ----------------------------------------------------------------
        #region SMethod: string GetToolTip(vGroupID, sItemID, sEnglish, sEnglishToolTip) - Workhorse method
        static public string GetToolTip(
            string[] vGroupID,
            string sItemID,
            string sEnglish,
            string sEnglishToolTip)
        {
            // Find (or add) the item, either in vGroupID, or in the Strings hierarchy
            LocItem item = GetLocItem(vGroupID, sItemID, sEnglish);
            Debug.Assert(null != item);

            // Make sure the item has the default English tooltip
            if (string.IsNullOrEmpty(item.ToolTip) & !string.IsNullOrEmpty(sEnglishToolTip))
                item.ToolTip = sEnglishToolTip;

            // Otherwise, we want to retrieve the string according to the Primary and
            // Secondary languages, as set up in the settings.
            return item.AltToolTip;
        }
        #endregion
        #region SMethod: string GetToolTip(UserControl, sItemID, sEnglish, sEnglishToolTip) - gets GroupID from the UC
        static public string GetToolTip(UserControl uc, string sItemID, string sEnglish, string sEnglishToolTip)
        {
            // Call the workhorse
            return GetToolTip(
                _GetGroupID(uc),
                sItemID,
                sEnglish,
                sEnglishToolTip);
        }
        #endregion
        // Retrieve a ShortcutKey ------------------------------------------------------------
        #region SMethod: Keys GetShortcutKey(vGroupID, sItemID, sEnglish, sEnglishShortcutKey) - Workhorse method
        static public Keys GetShortcutKey(
            string[] vGroupID,
            string sItemID,
            string sEnglish,
            string sEnglishShortcutKey)
        {
            // Find (or add) the item, either in vGroupID, or in the Strings hierarchy
            LocItem item = GetLocItem(vGroupID, sItemID, sEnglish);
            Debug.Assert(null != item);

            // Make sure the item has the default English Shortcut Key
            if (string.IsNullOrEmpty(item.ShortcutKey) && !string.IsNullOrEmpty(sEnglishShortcutKey))
                item.ShortcutKey = sEnglishShortcutKey;

            // Otherwise, we want to retrieve the string according to the Primary and
            // Secondary languages, as set up in the settings.
            string sKeys = item.AltShortcutKey;

            // Convert to the Keys type
            try
            {
                var converter = new KeysConverter();
                if (!string.IsNullOrEmpty(sKeys))
                {
                    var k = (Keys)converter.ConvertFromString(sKeys);
                    return k;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Shortcut key conversion problem: " + e.Message);
            }
            return Keys.None;
        }
        #endregion

    }
    #endregion

}
