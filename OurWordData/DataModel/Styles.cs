using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace OurWordData.DataModel
{
    public class CharacterStyle
    {
        public enum Variants { Normal, Superscript, Subscript };
        #region Attr{g/s}: Variants Variant
        public Variants Variant
        {
            get
            {
                return m_Variant;
            }
            set
            {
                if (m_Variant == value)
                    return;
                m_Variant = value;
                StyleSheet.DeclareDirty();
            }
        }
        private Variants m_Variant;
        #endregion

        public enum Styles { Normal, Bold=1, Italic=2, Underline=4 };
        #region Attr{g/s}: Styles Style
        public Styles Style
        {
            get
            {
                return m_Style;
            }
            set
            {
                if (m_Style == value)
                    return;
                m_Style = value;
                StyleSheet.DeclareDirty();
            }
        }
        private Styles m_Style;
        #endregion

        #region Attr{g/s}: Color TextColor - color for text; default is black
        public Color TextColor
        {
            get
            {
                var color = Color.FromName(m_sTextColorName);
                return color;
            }
            set
            {
                if (m_sTextColorName == value.Name) 
                    return;
                m_sTextColorName = value.Name;
                StyleSheet.DeclareDirty();
            }
        }
        private string m_sTextColorName;
        #endregion



        public CharacterStyle()
        {
            m_Variant = Variants.Normal;
            m_Style = Styles.Normal;
            m_sTextColorName = Color.Black.Name;
        }

    }

    public class StyleSheet
    {
        static public void DeclareDirty()
        {
            s_bIsDirty = true;
        }
        static private bool s_bIsDirty;

        static void IniitializeStyles()
        {
            if (s_bIsInitialized)
                return;

            var b = s_bIsDirty;

            s_VerseNumber = new CharacterStyle
            {
                Variant = CharacterStyle.Variants.Superscript,
                TextColor = Color.Red
            };

            s_ChapterNumber = new CharacterStyle
            {
                Style = CharacterStyle.Styles.Bold
            };

            s_bIsDirty = b;
            s_bIsInitialized = true;
        }

        private static bool s_bIsInitialized;


        static public CharacterStyle VerseNumber
        {
            get
            {
                IniitializeStyles();
                return s_VerseNumber;
            }
        }
        private static CharacterStyle s_VerseNumber;

        static public CharacterStyle ChapterNumber
        {
            get
            {
                IniitializeStyles();
                return s_ChapterNumber;
            }
        }
        private static CharacterStyle s_ChapterNumber;
  

    }
}
