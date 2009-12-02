using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace OurWordData.DataModel
{
    public class FontForWritingSystem
    {
        #region Attr{g/s}: string Name
        public string Name
        {
            get
            {
                return m_sName;
            }
            set
            {
                if (m_sName == value || string.IsNullOrEmpty(value))
                    return;
                m_sName = value;
                Reset();
                StyleSheet.DeclareDirty();
            }
        }
        private string m_sName = "Arial";
        #endregion
        #region Attr{g/s}: FontStyle Style
        public FontStyle Style
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
                Reset();
                StyleSheet.DeclareDirty();
            }
        }
        private FontStyle m_Style;
        #endregion
        #region Attr{g/s}: float Size
        public float Size
        {
            get
            {
                return m_fSize;
            }
            set
            {
                if (m_fSize == value)
                    return;
                m_fSize = value;
                Reset();
                StyleSheet.DeclareDirty();
            }
        }
        private float m_fSize = 10;
        #endregion

        public Font Regular
        {
            get
            {
                if (null == m_Regular)
                    m_Regular = new Font(Name, Size, Style);
                return m_Regular;
            }
        }
        private Font m_Regular;

        public Font Zoomed
        {
            get
            {
                if (null == m_Zoomed)
                {
                    var zoomedSize = Size * 1.5;
                    m_Zoomed = new Font(Name, Size, Style);
                }
                return m_Zoomed;
            }
        }
        private Font m_Zoomed;

        #region Method: void Reset()
        public void Reset()
        {
            m_Regular = null;
        }
        #endregion
    }


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

        #region Attr{g/s}: Styles Style
        public FontStyle Style
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
        private FontStyle m_Style;
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
            m_Style = FontStyle.Regular;
            m_sTextColorName = Color.Black.Name;
        }

    }

    public class StyleSheet
    {
        #region SMethod: void DeclareDirty()
        static public void DeclareDirty()
        {
            s_bIsDirty = true;
        }
        #endregion
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
                Style = FontStyle.Bold
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


        #region SAttr{g}: Font LargeDialogFont
        public static Font LargeDialogFont
            // This font is used for examining raw oxes files. I use a slightly larger
            // font due to the possible presence of diacritics which can otherwise be
            // difficult to read.
        {
            get
            {
                if (null == s_LargeDialogFont)
                {
                    s_LargeDialogFont = new Font(SystemFonts.DialogFont.FontFamily,
                        SystemFonts.DialogFont.Size * 1.2F,
                        FontStyle.Regular);
                }
                return s_LargeDialogFont;
            }
        }
        private static Font s_LargeDialogFont;
        #endregion
    }
}
