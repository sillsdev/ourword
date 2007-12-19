/**********************************************************************************************
 * Dll:     JWTools
 * File:    JW_Localization.cs
 * Author:  John Wimbish
 * Created: 12 May 2007
 * Purpose: Localization system.
 * Legal:   Copyright (c) 2005-08, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
#region Using
using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.IO;
using Microsoft.Win32;
#endregion

namespace JWTools
{
    #region CLASS: LocAlternate
    public class LocAlternate
    {
        // Attrs -----------------------------------------------------------------------------
        #region Attr{g/s}: string Value - the string in the language
        public string Value
        {
            get
            {
                Debug.Assert(null != m_sValue);
                return m_sValue;
            }
            set
            {
                m_sValue = value;
            }
        }
        string m_sValue = "";
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

        // Scaffolding -----------------------------------------------------------------------
        #region Constructor(sValue, sKey, sTip)
        public LocAlternate(string sValue, string sKey, string sTip)
        {
            m_sValue = sValue;
            m_sShortcutKey = ((null != sKey) ? sKey : "");
            m_sToolTip = ((null != sTip) ? sTip : "");
        }
        #endregion

        // I/O -------------------------------------------------------------------------------
        #region CONSTANTS
        public const string c_sTag = "Alt";
        public const string c_sID = "ID";
        public const string c_sValue = "Value";
        public const string c_sKey = "Key";
        public const string c_sTip = "Tip";
        #endregion
        #region Method: void WriteXML(XmlField xmlParent, string sLanguageID)
        public void WriteXML(XmlField xmlParent, string sLanguageID)
        {
            // Nothing to write if completely empty
            bool bHasValue = !string.IsNullOrEmpty(Value);
            bool bHasKey = !string.IsNullOrEmpty(ShortcutKey);
            bool bHasTip = !string.IsNullOrEmpty(ToolTip);
            if (!bHasValue && !bHasKey && !bHasTip)
                return;

            // Beginning tag <Item> contains the ID
            XmlField xml = xmlParent.GetDaughterXmlField(c_sTag, true);

            // The ID for the language, e.g., "sp", "inz"
            string s = xml.GetAttrString(c_sID, sLanguageID);

            // The value
            if (bHasValue)
                s += xml.GetAttrString(c_sValue, Value);

            // Shortcut if present
            if (bHasKey)
                s += xml.GetAttrString(c_sKey, ShortcutKey);

            // Tooltip if present
            if (bHasTip)
                s += xml.GetAttrString(c_sTip, ToolTip);

            // Write it out
            xml.OneLiner(s, "");
        }
        #endregion
        #region SMethod: LocAlternate ReadXML(XmlRead xml)
        static public LocAlternate ReadXML(XmlRead xml)
        {
            // Collect the parts
            string sValue = xml.GetValue(c_sValue);
            string sShortcutKey = xml.GetValue(c_sKey);
            string sToolTip = xml.GetValue(c_sTip);

            // Create and return the class
            LocAlternate alt = new LocAlternate(sValue, sShortcutKey, sToolTip);
            return alt;
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
                Debug.Assert(null != m_sID && m_sID.Length > 0);
                return m_sID;
            }
        }
        string m_sID;
        #endregion
        #region Attr{g/s}: string English - the string in English, always available
        public string English
        {
            get
            {
                Debug.Assert(null != m_sEnglish && m_sEnglish.Length > 0);
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
        public bool CanHaveShortcutKey
        {
            get
            {
                return m_bCanHaveShortcutKey;
            }
            set
            {
                m_bCanHaveShortcutKey = value;
            }
        }
        bool m_bCanHaveShortcutKey = false;
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
                return m_bCanHaveToolTip;
            }
            set
            {
                m_bCanHaveToolTip = value;
            }
        }
        bool m_bCanHaveToolTip = false;
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
                LocAlternate[] v = new LocAlternate[iIndex + 1];
                for (int i = 0; i < Alternates.Length; i++)
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
                if (null != LocDB.DB.PrimaryLanguage)
                {
                    LocAlternate alt = GetAlternate(LocDB.DB.PrimaryLanguage.Index);
                    if (null != alt)
                        return alt.Value;
                }

                // If the secondary language is null, then English was intended
                if (null == LocDB.DB.SecondaryLanguage)
                    return English;

                // Otherwise, go for one of our localized languages
                if (null != LocDB.DB.SecondaryLanguage)
                {
                    LocAlternate alt = GetAlternate(LocDB.DB.SecondaryLanguage.Index);
                    if (null != alt)
                        return alt.Value;
                }

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
                if (null != LocDB.DB.PrimaryLanguage)
                {
                    LocAlternate alt = GetAlternate(LocDB.DB.PrimaryLanguage.Index);
                    if (null != alt && !string.IsNullOrEmpty(alt.ShortcutKey))
                        return alt.ShortcutKey;
                }

                // If the secondary language is null, then English was intended
                if (null == LocDB.DB.SecondaryLanguage)
                    return ShortcutKey;

                // Otherwise, go for one of our localized languages
                if (null != LocDB.DB.SecondaryLanguage)
                {
                    LocAlternate alt = GetAlternate(LocDB.DB.SecondaryLanguage.Index);
                    if (null != alt && !string.IsNullOrEmpty(alt.ShortcutKey))
                        return alt.ShortcutKey;
                }

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
                if (null != LocDB.DB.PrimaryLanguage)
                {
                    LocAlternate alt = GetAlternate(LocDB.DB.PrimaryLanguage.Index);
                    if (null != alt && !string.IsNullOrEmpty(alt.ToolTip))
                        return alt.ToolTip;
                }

                // If the secondary language is null, then English was intended
                if (null == LocDB.DB.SecondaryLanguage)
                    return ToolTip;

                // Otherwise, go for one of our localized languages
                if (null != LocDB.DB.SecondaryLanguage)
                {
                    LocAlternate alt = GetAlternate(LocDB.DB.SecondaryLanguage.Index);
                    if (null != alt && !string.IsNullOrEmpty(alt.ToolTip))
                        return alt.ToolTip;
                }

                // If here, then the localization did not exist for either Primary or Secondary
                return ToolTip;
            }
        }
        #endregion

        // Consistency Tests -----------------------------------------------------------------
        #region Method: bool EndsWithColon(s)
        bool EndsWithColon(string s)
        {
            if (s.Length > 0 && s[s.Length - 1] == ':')
                return true;
            return false;
        }
        #endregion
        #region Method: bool EndsWithEllipsis(s)
        bool EndsWithEllipsis(string s)
        {
            if (s.Length < 3)
                return false;

            if (s.Substring(s.Length - 3) == "...")
                return true;

            return false;
        }
        #endregion
        #region Method: int ParameterCount(s)
        int ParameterCount(string s)
        {
            int c = 0;

            for (int i = 0; i < s.Length - 3; i++)
            {
                char ch1 = s[i];
                char ch2 = s[i + 1];
                char ch3 = s[i + 2];
                if (ch1 == '{' && Char.IsDigit(ch2) && ch3 == '}')
                    c++;
            }

            return c;
        }
        #endregion

        // I/O -------------------------------------------------------------------------------
        #region DOC
        /* XML Format (eventually I want the Alts in their own file)
         * 
         * <Item ID="m_menuNew" Key="true" Tip="true">
         *    <Info>"Menu command to create a new project."</Info>
         *    <En>"&Amp;New..."</En>
         *    <Key>Ctrl+N</Key>
         *    <Tip Title="New Project">Create a blank, new project</Tip>
         *    <Alt lang="inz" Value="&amp;Baru..." Key="Ctrl+B" TipTitle="Proyek Baru" TipText="Cipta proyeck yang baru"></Alt>
         *    <Alt lang="sp" Value="&amp;Nuevo..." Key="Ctrl+N"></Alt>
         *    <Alt lang="swh" Value="&amp;Mpya..."></Alt>
         * </Item>
         */
        #endregion
        #region CONSTANTS
        public const string c_sTag = "Item";
        public const string c_sID = "ID";
        public const string c_sEnglish = "English";
        public const string c_sInformation = "Info";
        public const string c_sTip = "Tip";
        public const string c_sKey = "Key";
        #endregion
        #region Method: void WriteXML(XmlField xmlParent)
        public void WriteXML(XmlField xmlParent)
        {
            // Beginning tag <Item> contains the ID
            XmlField xml = xmlParent.GetDaughterXmlField(c_sTag, true);
            string s = xml.GetAttrString(c_sID, ID);
            if (CanHaveShortcutKey)
                s += xml.GetAttrString(c_sKey, "true");
            xml.Begin(s);

            // Add the English
            xml.GetDaughterXmlField(c_sEnglish, true).OneLiner(English);

            // Add the Information, if any
            if (!string.IsNullOrEmpty(Information))
                xml.GetDaughterXmlField(c_sInformation, true).OneLiner(Information);

            // Add the shortcut key, if any
            if (!string.IsNullOrEmpty(ShortcutKey))
                xml.GetDaughterXmlField(c_sKey, true).OneLiner(ShortcutKey);

            // Add the Tooltip, if any
            if (!string.IsNullOrEmpty(ToolTip))
                xml.GetDaughterXmlField(c_sTip, true).OneLiner(ToolTip);

            // Add the Language Alternatives
            foreach (LocLanguage language in LocDB.DB.Languages)
            {
                LocAlternate alt = GetAlternate(language.Index);
                if (null != alt)
                    alt.WriteXML(xml, language.ID);
            }

            // End Tag </Item>
            xml.End();
        }
        #endregion
        #region SMethod: LocItem ReadXML(LocDB db, XmlRead xml)
        static public LocItem ReadXML(LocDB db, XmlRead xml)
        {
            // Collect the ID from the Tag line, and create an item from it
            string sID = xml.GetValue(c_sID);
            LocItem item = new LocItem(sID);

            string sCanHaveKey = xml.GetValue(c_sKey);
            if (!string.IsNullOrEmpty(sCanHaveKey) && sCanHaveKey == "true")
                item.CanHaveShortcutKey = true;

            string sCanHaveTip = xml.GetValue(c_sTip);
            if (!string.IsNullOrEmpty(sCanHaveTip) && sCanHaveKey == "true")
                item.CanHaveToolTip = true;

            // Loop through the other lines for the remaining data
            while (xml.ReadNextLineUntilEndTag(c_sTag))
            {
                if (xml.IsTag(c_sInformation))
                    item.Information = xml.GetOneLinerData();

                if (xml.IsTag(c_sEnglish))
                    item.English = xml.GetOneLinerData();

                if (xml.IsTag(c_sKey))
                    item.ShortcutKey = xml.GetOneLinerData();

                if (xml.IsTag(c_sTip))
                    item.ToolTip = xml.GetOneLinerData();

                if (xml.IsTag(LocAlternate.c_sTag))
                {
                    string sLanguageID = xml.GetValue(LocAlternate.c_sID);
                    LocAlternate alt = LocAlternate.ReadXML(xml);
                    foreach (LocLanguage language in db.Languages)
                    {
                        if (language.ID == sLanguageID)
                        {
                            item.AddAlternate(language.Index, alt);
                            break;
                        }
                    }
                }
            }

            return item;
        }
        #endregion

        // Scaffolding -----------------------------------------------------------------------
        #region Constructor(sID)
        public LocItem(string _sID)
        {
            m_sID = _sID;

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
                Debug.Assert(null != m_sID && m_sID.Length > 0);
                return m_sID;
            }
        }
        string m_sID;
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
                Debug.Assert(null != m_sTitle && m_sTitle.Length > 0);
                return m_sTitle;
            }
        }
        string m_sTitle;
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
        public const string c_sTag = "Group";
        public const string c_sID = "ID";
        public const string c_sTitle = "Title";
        public const string c_sDescription = "Des";
        #region Method: void WriteXML(XmlField xmlParent)
        public void WriteXML(XmlField xmlParent)
        {
            XmlField xml = xmlParent.GetDaughterXmlField(c_sTag, true);

            string s = xml.GetAttrString(c_sID, ID);

            s += xml.GetAttrString(c_sTitle, Title);

            if (!string.IsNullOrEmpty(Description))
                s += xml.GetAttrString(c_sDescription, Description);

            xml.Begin(s);

            foreach (LocItem item in Items)
                item.WriteXML(xml);

            foreach (LocGroup sub in Groups)
                sub.WriteXML(xml);

            xml.End();
        }
        #endregion
        #region SMethod: LocGroup ReadXML(LocDB db, XmlRead xml)
        static public LocGroup ReadXML(LocDB db, XmlRead xml)
        {
            string sID = xml.GetValue(c_sID);
            string sTitle = xml.GetValue(c_sTitle);
            string sDescription = xml.GetValue(c_sDescription);

            LocGroup group = new LocGroup(sID, sTitle);
            group.Description = sDescription;

            while (xml.ReadNextLineUntilEndTag(c_sTag))
            {
                if (xml.IsTag(LocItem.c_sTag))
                {
                    LocItem item = LocItem.ReadXML(db, xml);
                    group.AppendItem(item);
                }

                if (xml.IsTag(LocGroup.c_sTag))
                {
                    LocGroup sub = LocGroup.ReadXML(db, xml);
                    group._AppendGroup(sub);
                }
            }

            return group;
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
            m_sID = _sID;
            m_sName = _sName;
            m_iIndex = _iIndex;
        }
        #endregion

        // I/O -------------------------------------------------------------------------------
        public const string c_sTag = "Language";
        public const string c_sID = "ID";
        public const string c_sName = "Name";
        public const string c_sFontName = "Font";
        public const string c_sFontSize = "Size";
        #region Method: void WriteXML(XmlField xmlParent)
        public void WriteXML(XmlField xmlParent)
        {
            XmlField xml = xmlParent.GetDaughterXmlField(c_sTag, true);

            string s = xml.GetAttrString(c_sID, ID);
            s += xml.GetAttrString(c_sName, Name);

            if (!string.IsNullOrEmpty(FontName))
                s += xml.GetAttrString(c_sFontName, FontName);

            if (0 != FontSize)
                s += xml.GetAttrString(c_sFontSize, FontSize.ToString());

            xml.OneLiner(s, "");
        }
        #endregion
        #region SMethod: LocLanguage ReadXML(LocDB db, XmlRead xml)
        static public LocLanguage ReadXML(LocDB db, XmlRead xml)
        {
            string sID = xml.GetValue(c_sID);
            string sName = xml.GetValue(c_sName);
            int iIndex = db.Languages.Length;

            LocLanguage language = new LocLanguage(sID, sName, iIndex);

            string sFontName = xml.GetValue(c_sFontName);
            if (!string.IsNullOrEmpty(sFontName))
                language.FontName = sFontName;

            string sFontSize = xml.GetValue(c_sFontSize);
            if (!string.IsNullOrEmpty(sFontSize))
            {
                try
                {
                    int n = Convert.ToInt16(sFontSize);
                    if (n != 0)
                        language.FontSize = n;
                }
                catch (Exception)
                {
                }
            }


            return language;
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
        LocGroup[] m_vGroups = null;
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
        const string c_DialogCommon = "DialogCommon";
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
        LocGroup m_Strings = null;
        #endregion
        #region Attr{g}: LocGroup DialogCommon - the Group that contains the common dialog controls
        public LocGroup DialogCommon
        {
            get
            {
                if (null == m_DialogCommon)
                    m_DialogCommon = FindGroup(c_DialogCommon);

                Debug.Assert(null != m_DialogCommon);

                return m_DialogCommon;
            }
        }
        LocGroup m_DialogCommon;
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
        LocLanguage[] m_vLanguages = null;
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
        LocLanguage m_langPrimary = null;
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
        LocLanguage m_langSecondary = null;
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

            SetToRegistry();
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

            SetToRegistry();
        }
        #endregion

        #region Attr{g}: string[] LanguageChoices - includes English
        public string[] LanguageChoices
        {
            get
            {
                string[] v = new string[Languages.Length + 1];

                for (int i = 0; i < Languages.Length; i++)
                    v[i] = Languages[i].Name;

                v[Languages.Length] = LocItem.c_sEnglish;

                return v;
            }
        }
        #endregion

        // Registry --------------------------------------------------------------------------
        const string c_keyPrimary = "PrimaryUI";
        const string c_keySecondary = "SecondaryUI";
        #region Method: void GetFromRegistry()
        public void GetFromRegistry()
        {
            string sPrimary = JW_Registry.GetValue(c_keyPrimary, "");
            string sSecondary = JW_Registry.GetValue(c_keySecondary, "");

            foreach (LocLanguage lang in Languages)
            {
                if (lang.Name == sPrimary)
                    PrimaryLanguage = lang;

                if (lang.Name == sSecondary)
                    SecondaryLanguage = lang;
            }
        }
        #endregion
        #region Method: void SetToRegistry()
        public void SetToRegistry()
        {
            if (null != PrimaryLanguage)
                JW_Registry.SetValue(c_keyPrimary, PrimaryLanguage.Name);
            else
                JW_Registry.SetValue(c_keyPrimary, "");

            if (null != SecondaryLanguage)
                JW_Registry.SetValue(c_keySecondary, SecondaryLanguage.Name);
            else
                JW_Registry.SetValue(c_keySecondary, "");
        }
        #endregion

        // I/O -------------------------------------------------------------------------------
        #region VAttr{g}: string DataFilePath
        string DataFilePath
        {
            get
            {
                // To support both OurWord and NUnit, we get the path of this DLL file
                string sAssemblyPathName = Assembly.GetAssembly(typeof(LocDB)).Location;
                string sFolders = Path.GetDirectoryName(sAssemblyPathName);
                string sLocalizationsDBPath = sFolders + Path.DirectorySeparatorChar + "Loc.xml";

                // For NUnit, still probably will not find it, so we go to the registry's
                // "AppDir" value to find out where it is.
                if (!File.Exists(sLocalizationsDBPath))
                {
                    sLocalizationsDBPath = JW_Registry.GetValue("NUnit_LocDbDir", "C:") +
                        Path.DirectorySeparatorChar + "Localizations.xml";
                }

                return sLocalizationsDBPath;
            }
        }
        #endregion
        const string c_sTag = "LocDB";
        #region Method: void WriteXML(XmlField xmlParent)
        public void WriteXML()
        {
            string sPathName = Path.ChangeExtension(DataFilePath, "xml");
            TextWriter writer = JW_Util.GetTextWriter(sPathName);

            XmlField xml = new XmlField(writer, c_sTag);
            xml.Begin();

            foreach (LocLanguage language in Languages)
                language.WriteXML(xml);

            foreach (LocGroup group in Groups)
                group.WriteXML(xml);

            xml.End();

            writer.Close();
        }
        #endregion
        #region Method: void ReadXML()
        public void ReadXML()
        {
            string sPathName = DataFilePath;
            TextReader reader = JW_Util.GetTextReader(ref sPathName, "*.xml");
            XmlRead xml = new XmlRead(reader);

            while (xml.ReadNextLineUntilEndTag(c_sTag))
            {
                // LocLanguage
                if (xml.IsTag(LocLanguage.c_sTag))
                {
                    LocLanguage language = LocLanguage.ReadXML(this, xml);
                    AppendLanguage(language);
                }

                // LocGroup
                if (xml.IsTag(LocGroup.c_sTag))
                {
                    LocGroup group = LocGroup.ReadXML(this, xml);
                    AppendGroup(group);
                }
            }

            reader.Close();
        }
        #endregion

        // Scaffolding -----------------------------------------------------------------------
        #region SAttr{g}: LocDB DB - initializes if needed
        static public LocDB DB
        {
            get
            {
                if (null == s_LocDB)
                    s_LocDB = new LocDB();
                Debug.Assert(null != s_LocDB);
                return s_LocDB;
            }
        }
        static private LocDB s_LocDB = null;
        #endregion
        #region private Constructor() - do not call (called by the DB attr above
        private LocDB()
        {
            // Initialize the vectors
            m_vGroups = new LocGroup[0];
            m_vLanguages = new LocLanguage[0];

            // Read in the file
            ReadXML();

            // Retrieve the current settings from the registry
            GetFromRegistry();
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
            DialogResult result = MessageBox.Show(Form.ActiveForm,
                sMessageText, sAppTitle, buttons, icon);
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
        static private void Localize(ToolStripItem tsi)
        {
            // Certain controls are not localized
            if (tsi as ToolStripSeparator != null)
                return;

            // The ID is the name of the item
            string sItemID = tsi.Name;
            if (string.IsNullOrEmpty(sItemID))
                return;

            // Calculate/retrieve the group ID
            string[] vGroupID = _GetGroupID(tsi);

            // Get the ToolStripItem's text value
            tsi.Text = GetValue(
                c_DialogCommon,
                vGroupID,
                sItemID,
                tsi.Text,
                null,
                null);

            // Get its tooltip
            tsi.ToolTipText = GetToolTip(
                c_DialogCommon,
                vGroupID,
                sItemID,
                tsi.Text,
                tsi.ToolTipText);

            // Get its Shortcut key
            ToolStripMenuItem mi = tsi as ToolStripMenuItem;
            if (null != mi)
                mi.ShortcutKeys = GetShortcutKey(
                    c_DialogCommon,
                    vGroupID,
                    sItemID,
                    tsi.Text,
                    mi.ShortcutKeyDisplayString);
            
            // Recurse if this is a drop-down item
            ToolStripDropDownItem tsiDropDown = tsi as ToolStripDropDownItem;
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
                null,
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
                c_DialogCommon,
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
        #region SMethod: LocItem _UseCommonGroupItem(...)
        static LocItem _UseCommonGroupItem(string sCommonGroupID, LocGroup groupMain, string sItemID)
        {
            // If there is no common group id specified, then we obviously don't use it
            if (string.IsNullOrEmpty(sCommonGroupID))
                return null;

            // If the Item already exists in the main group, then we don't use the
            // common group, so we return null.
            if (null != groupMain.Find(sItemID))
                return null;

            // Locate the common group. We expect it to be at the top level (that is,
            // owned by the DB.
            LocGroup gCommon = DB.FindGroup(sCommonGroupID);
            if (null == gCommon)
                return null;

            // Locate the Item within the common group
            LocItem itemCommon = gCommon.Find(sItemID);
            return itemCommon;
        }
        #endregion
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
                if (null != tsi.OwnerItem)
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
        // Retrieve a Value ------------------------------------------------------------------
        #region SMethod: string GetValue(vGroupID, sItemID, sEnglish, sLanguage, vsInsert) - Workhorse method
        /// <summary>
        /// Looks up the string according to its GroupID/ItemID, and returns its localized value. 
        /// </summary>
        /// <param name="sCommonGroupID">Allows an optional search in a common group, such as
        /// a group containing common dialog controls such as OK or Cancel. The logic is that
        /// if the item exists in the vGroupID, then we use it; otherwise we search in the
        /// common group; and finally if it is still not found, we insert it into the vGroupID
        /// group.</param>
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
            string sCommonGroupID,
            string[] vGroupID, 
            string sItemID, 
            string sEnglish,
            string sLanguage,
            string[] vsInsert)
        {
            // Drill down to locate the desired group, adding it if necessary
            LocGroup group = null;
            foreach (string sGroupID in vGroupID)
            {
                if (group == null)
                    group = DB.FindOrAddGroup(sGroupID);
                else
                    group = group.FindOrAddGroup(sGroupID);
            }

            // If we have a CommonGroup, then we'll go there if the item is not in the main Group
            LocItem item = _UseCommonGroupItem(sCommonGroupID, group, sItemID);

            // If we aren't using the common group item, then we either find or add it to the
            // target main group.
            if (null == item)
                item = group.FindOrAddItem(sItemID, sEnglish);

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
                null,
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
                null,
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
            string sCommonGroupID,
            string[] vGroupID,
            string sItemID,
            string sEnglish,
            string sEnglishToolTip)
        {
            // Drill down to locate the desired group, adding it if necessary
            LocGroup group = null;
            foreach (string sGroupID in vGroupID)
            {
                if (group == null)
                    group = DB.FindOrAddGroup(sGroupID);
                else
                    group = group.FindOrAddGroup(sGroupID);
            }

            // If we have a CommonGroup, then we'll go there if the item is not in the main Group
            LocItem item = _UseCommonGroupItem(sCommonGroupID, group, sItemID);

            // If we aren't using the common group item, then we either find or add it to the
            // target main group.
            if (null == item)
                item = group.FindOrAddItem(sItemID, sEnglish);

            // Make sure the item has the default English tooltip
            if (string.IsNullOrEmpty(item.ToolTip))
                item.ToolTip = sEnglishToolTip;

            // Otherwise, we want to retrieve the string according to the Primary and
            // Secondary languages, as set up in the settings.
            return item.ToolTip;
        }
        #endregion
        #region SMethod: string GetToolTip(UserControl, sItemID, sEnglish, sEnglishToolTip) - gets GroupID from the UC
        static public string GetToolTip(UserControl uc, string sItemID, string sEnglish, string sEnglishToolTip)
        {
            // Call the workhorse
            return GetToolTip(
                c_DialogCommon,
                _GetGroupID(uc),
                sItemID,
                sEnglish,
                sEnglishToolTip);
        }
        #endregion
        // Retrieve a ShortcutKey ------------------------------------------------------------
        #region SMethod: Keys GetShortcutKey(vGroupID, sItemID, sEnglish, sEnglishShortcutKey) - Workhorse method
        static public Keys GetShortcutKey(
            string sCommonGroupID,
            string[] vGroupID,
            string sItemID,
            string sEnglish,
            string sEnglishShortcutKey)
        {
            // Drill down to locate the desired group, adding it if necessary
            LocGroup group = null;
            foreach (string sGroupID in vGroupID)
            {
                if (group == null)
                    group = DB.FindOrAddGroup(sGroupID);
                else
                    group = group.FindOrAddGroup(sGroupID);
            }

            // If we have a CommonGroup, then we'll go there if the item is not in the main Group
            LocItem item = _UseCommonGroupItem(sCommonGroupID, group, sItemID);

            // If we aren't using the common group item, then we either find or add it to the
            // target main group.
            if (null == item)
                item = group.FindOrAddItem(sItemID, sEnglish);

            // Make sure the item has the default English Shortcut Key
            if (string.IsNullOrEmpty(item.ShortcutKey))
                item.ShortcutKey = sEnglishShortcutKey;

            // Otherwise, we want to retrieve the string according to the Primary and
            // Secondary languages, as set up in the settings.
            string sKeys = item.AltShortcutKey;

            // Convert to the Keys type
            try
            {
                KeysConverter converter = new KeysConverter();
                if (!string.IsNullOrEmpty(sKeys))
                {
                    Keys k = (Keys)converter.ConvertFromString(sKeys);
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
