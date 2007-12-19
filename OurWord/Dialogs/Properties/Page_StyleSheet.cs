/**********************************************************************************************
 * Project: Our Word!
 * File:    Page_StyleSheet.cs
 * Author:  John Wimbish
 * Created: 17 May 2006
 * Purpose: Create settings for the Front, or open existing ones.
 * Legal:   Copyright (c) 2004-08, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
#region Header: Using, etc.
using System;
using System.Collections;
using System.ComponentModel;
using System.Globalization;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.Resources;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.IO;
using System.Reflection;
using System.Threading;

using JWTools;
using JWdb;
using OurWord;
using OurWord.DataModel;
using OurWord.Dialogs;
using OurWord.View;
#endregion

namespace OurWord.Dialogs
{
    public class Page_StyleSheet : DlgPropertySheet
    {
        // Attrs -----------------------------------------------------------------------------
        #region Attr{g/s}: JParagraphStyle PStyle
        JParagraphStyle PStyle
        {
            get
            {
                return m_PStyle;
            }
            set
            {
                m_PStyle = value;
            }
        }
        JParagraphStyle m_PStyle = null;
        #endregion
        #region Attr{g/s}: JCharacterStyle CStyle
        JCharacterStyle CStyle
        {
            get
            {
                return m_CStyle;
            }
            set
            {
                m_CStyle = value;
            }
        }
        JCharacterStyle m_CStyle = null;
        #endregion

        // Scaffolding -----------------------------------------------------------------------
        #region DIALOG CONTROLS

        // My subclasses of Dialog controls

        // Dialog Controls
        private ListView m_listStyles;
        private Label m_labelInstructions;
        private Button m_btnRestoreDefaults;
        private PropertyGrid m_PropGrid;
        private ColumnHeader m_col1;
        private Label m_labelStyleSheet;
        private System.ComponentModel.Container components = null;
        #endregion
        #region Constructor()
        public Page_StyleSheet(DialogProperties _ParentDlg)
            : base(_ParentDlg)
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();
        }
        #endregion
        #region Method: void Dispose(...)
        protected override void Dispose(bool disposing)
        // Clean up any resources being used.
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }
        #endregion
        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.m_listStyles = new System.Windows.Forms.ListView();
            this.m_col1 = new System.Windows.Forms.ColumnHeader();
            this.m_labelInstructions = new System.Windows.Forms.Label();
            this.m_btnRestoreDefaults = new System.Windows.Forms.Button();
            this.m_PropGrid = new System.Windows.Forms.PropertyGrid();
            this.m_labelStyleSheet = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // m_listStyles
            // 
            this.m_listStyles.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.m_col1});
            this.m_listStyles.FullRowSelect = true;
            this.m_listStyles.HideSelection = false;
            this.m_listStyles.Location = new System.Drawing.Point(8, 32);
            this.m_listStyles.MultiSelect = false;
            this.m_listStyles.Name = "m_listStyles";
            this.m_listStyles.Size = new System.Drawing.Size(157, 303);
            this.m_listStyles.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.m_listStyles.TabIndex = 0;
            this.m_listStyles.UseCompatibleStateImageBehavior = false;
            this.m_listStyles.View = System.Windows.Forms.View.Details;
            this.m_listStyles.SelectedIndexChanged += new System.EventHandler(this.cmdListSelectionChanged);
            // 
            // m_col1
            // 
            this.m_col1.Text = "Styles";
            this.m_col1.Width = 145;
            // 
            // m_labelInstructions
            // 
            this.m_labelInstructions.Location = new System.Drawing.Point(184, 0);
            this.m_labelInstructions.Name = "m_labelInstructions";
            this.m_labelInstructions.Size = new System.Drawing.Size(276, 52);
            this.m_labelInstructions.TabIndex = 2;
            this.m_labelInstructions.Text = "Select a style from the list on the left, then edit its settings below. Some sett" +
                "ings require you to click on the plus sign in order to edit their settings.";
            this.m_labelInstructions.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // m_btnRestoreDefaults
            // 
            this.m_btnRestoreDefaults.Location = new System.Drawing.Point(5, 337);
            this.m_btnRestoreDefaults.Name = "m_btnRestoreDefaults";
            this.m_btnRestoreDefaults.Size = new System.Drawing.Size(160, 23);
            this.m_btnRestoreDefaults.TabIndex = 30;
            this.m_btnRestoreDefaults.Text = "Restore Default Values...";
            this.m_btnRestoreDefaults.Click += new System.EventHandler(this.cmdRestoreDefaultValues);
            // 
            // m_PropGrid
            // 
            this.m_PropGrid.LineColor = System.Drawing.SystemColors.ScrollBar;
            this.m_PropGrid.Location = new System.Drawing.Point(187, 55);
            this.m_PropGrid.Name = "m_PropGrid";
            this.m_PropGrid.PropertySort = System.Windows.Forms.PropertySort.Categorized;
            this.m_PropGrid.Size = new System.Drawing.Size(273, 305);
            this.m_PropGrid.TabIndex = 32;
            this.m_PropGrid.ToolbarVisible = false;
            // 
            // m_labelStyleSheet
            // 
            this.m_labelStyleSheet.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_labelStyleSheet.Location = new System.Drawing.Point(8, 8);
            this.m_labelStyleSheet.Name = "m_labelStyleSheet";
            this.m_labelStyleSheet.Size = new System.Drawing.Size(146, 21);
            this.m_labelStyleSheet.TabIndex = 33;
            this.m_labelStyleSheet.Text = "Style Sheet:";
            this.m_labelStyleSheet.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // Page_StyleSheet
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.m_labelStyleSheet);
            this.Controls.Add(this.m_PropGrid);
            this.Controls.Add(this.m_btnRestoreDefaults);
            this.Controls.Add(this.m_labelInstructions);
            this.Controls.Add(this.m_listStyles);
            this.Name = "Page_StyleSheet";
            this.Size = new System.Drawing.Size(468, 368);
            this.Load += new System.EventHandler(this.cmdLoad);
            this.ResumeLayout(false);

        }
        #endregion

        // DlgPropertySheet overrides --------------------------------------------------------
        #region Method: void ShowHelp()
        public override void ShowHelp()
        {
            HelpSystem.ShowPage_StylesSheet();
        }
        #endregion
        #region Attr{g}: string TabText
        public override string TabText
        {
            get
            {
                return Strings.PropDlgTab_StyleSheet;
            }
        }
        #endregion

        // Property Grid ---------------------------------------------------------------------
        #region BAG CONSTANTS
        const string c_sPropCharacterSettings = "propCharacterSettings";
        const string c_sPropForeColor = "propForeColor";

        const string c_sPropParagraphSettings = "propParagraphSettings";
        const string c_sPropAlignment = "propAlignment";
        const string c_sPropKeepWithNext = "propKeepWithNext";
        const string c_sPropLeftMargin = "propLeftMargin";
        const string c_sPropRightMargin = "propRightMargin";
        const string c_sPropFirstLine = "propFirstLine";
        const string c_sPropSpaceBefore = "propSpaceBefore";
        const string c_sPropSpaceAfter = "propSpaceAfter";
        #endregion
        #region Attr{g}: PropertyBag Bag - Defines the properties to display (including localizations)
        PropertyBag Bag
        {
            get
            {
                Debug.Assert(null != m_bag);
                return m_bag;
            }
        }
        PropertyBag m_bag;
        #endregion
        #region Attr{g}: ArrayList FontBags - Inner settings for each font
        ArrayList FontBags
        {
            get
            {
                Debug.Assert(null != m_aFontBags);
                return m_aFontBags;
            }
        }
        ArrayList m_aFontBags;
        #endregion

        #region Method: double GetDoubleFromGridText(string s)
        double GetDoubleFromGridText(string s)
        {
            // Extract the numerical part of the string
            string sValue = "";
            foreach (char ch in s)
            {
                if (char.IsDigit(ch) || ch == '-' || ch == '.' || ch == ',')
                    sValue += ch;
            }

            // Convert it to a double
            try
            {
                return Convert.ToDouble(sValue);
            }
            catch (Exception)
            {
            }
            return 0.0;
        }
        #endregion
        #region Method: string SetDoubleToGridText(double d, sTag)
        string SetDoubleToGridText(double d, string sTag)
        {
            return (d.ToString("0.00") + sTag);
        }
        #endregion
        #region Method: string SetIntToGridText(double d, sTag)
        string SetIntToGridText(int n, string sTag)
        {
            return (n.ToString() + sTag);
        }
        #endregion

        #region EMBEDDED CLASS: FontPropertyBag : PropertyBag - inner properties for a font
        class FontPropertyBag : PropertyBag
        {
            const string c_sPropFontName = "propFontName";
            const string c_sPropFontHeight = "propFontHeight";
            const string c_sPropBold = "propBold";
            const string c_sPropItalic = "propItalic";

            #region Attr{g}: JFontForWritingSystem FWS
            public JFontForWritingSystem FWS
            {
                get
                {
                    Debug.Assert(null != m_fws);
                    return m_fws;
                }
            }
            JFontForWritingSystem m_fws;
            #endregion

            #region VAttr{g}: string Name
            public string Name
            {
                get
                {
                    return FWS.WritingSystem.Name;
                }
            }
            #endregion

            #region Method: void FontPropertyBag_GetValue(...)
            private void FontPropertyBag_GetValue(object sender, PropertySpecEventArgs e)
            {
                if (e.Property.ID == c_sPropFontName)
                {
                    e.Value = FWS.FontName;
                }
                else if (e.Property.ID == c_sPropFontHeight)
                {
                    e.Value = FWS.Size;
                }
                else if (e.Property.ID == c_sPropBold)
                {
                    BoolPropertySpec ps = e.Property as BoolPropertySpec;
                    Debug.Assert(null != ps);
                    e.Value = ps.GetBoolString(FWS.IsBold);
                }
                else if (e.Property.ID == c_sPropItalic)
                {
                    BoolPropertySpec ps = e.Property as BoolPropertySpec;
                    Debug.Assert(null != ps);
                    e.Value = ps.GetBoolString(FWS.IsItalic);
                }
            }
            #endregion
            #region Method: void FontPropertyBag_SetValue(...)
            private void FontPropertyBag_SetValue(object sender, PropertySpecEventArgs e)
            {
                if (e.Property.ID == c_sPropFontName)
                {
                    FWS.FontName = (string)e.Value;
                }
                else if (e.Property.ID == c_sPropFontHeight)
                {
                    FWS.Size = Convert.ToInt16(e.Value);
                }
                else if (e.Property.ID == c_sPropBold)
                {
                    BoolPropertySpec ps = e.Property as BoolPropertySpec;
                    Debug.Assert(null != ps);
                    FWS.IsBold = ps.IsTrue((string)e.Value);
                }
                else if (e.Property.ID == c_sPropItalic)
                {
                    BoolPropertySpec ps = e.Property as BoolPropertySpec;
                    Debug.Assert(null != ps);
                    FWS.IsItalic = ps.IsTrue((string)e.Value);
                }
            }
            #endregion

            #region Constructor(JFontForWritingSystem)
            public FontPropertyBag(JFontForWritingSystem fws)
            {
                // Remember the parameters
                m_fws = fws;

                // Add the properties
                Properties.Add(new PropertySpec(
                    c_sPropFontName,
                    "Font Name",
                    typeof(string),
                    "",
                    "The name of the font for displaying the style in this writing system",
                    "Arial",
                    "",
                    typeof(FontNameConverter)
                    ));

                Properties.Add(new PropertySpec(
                    c_sPropFontHeight,
                    "Height",
                    typeof(int),
                    "",
                    "The height of the font.",
                    10,
                    "",
                    typeof(FontSizeConverter)
                    ));

                Properties.Add(new BoolPropertySpec(
                    c_sPropBold,
                    "Bold?",
                    "",
                    "If Yes, the font is displayed in boldface.",
                    new string[] { "Yes", "No" },
                    "No"
                    ));

                Properties.Add(new BoolPropertySpec(
                    c_sPropItalic,
                    "Italic?",
                    "",
                    "If Yes, the font is displayed in italics.",
                    new string[] { "Yes", "No" },
                    "No"
                    ));

                // Listen for the callbacks
                GetValue += new PropertySpecEventHandler(FontPropertyBag_GetValue);
                SetValue += new PropertySpecEventHandler(FontPropertyBag_SetValue);
            }
            #endregion

            #region OMethod: string ToString()
            public override string ToString()
            {
                // Start with the font name and the height
                string s = FWS.FontName + ", " + FWS.Size.ToString();

                // Add Bold if bold is true
                if (FWS.IsBold)
                {
                    s += (", ");
                    foreach (char ch in FindPropertySpec(c_sPropBold).Name)
                    {
                        if (!char.IsPunctuation(ch))
                            s += ch;
                    }
                }

                // Add Italic if italic is true
                if (FWS.IsItalic)
                {
                    s += (", ");
                    foreach (char ch in FindPropertySpec(c_sPropItalic).Name)
                    {
                        if (!char.IsPunctuation(ch))
                            s += ch;
                    }
                }

                return s;
            }
            #endregion
        }
        #endregion

        #region Method: void bag_GetValue(object sender, PropertySpecEventArgs e)
        void bag_GetValue(object sender, PropertySpecEventArgs e)
        {
            // WritingSystem / Font Settings
            foreach (FontPropertyBag bag in m_aFontBags)
            {
                if (bag.Name == e.Property.Name)
                {
                    e.Value = bag;
                    return;
                }
            }

            // Other Character Settings
            if (e.Property.ID == c_sPropForeColor)
            {
                e.Value = CStyle.FontColor.Name;
            }

            // Paragraph Settings
            if (null != PStyle)
            {
                if (e.Property.ID == c_sPropAlignment)
                {
                    EnumPropertySpec ps = e.Property as EnumPropertySpec;
                    Debug.Assert(null != ps);
                    e.Value = ps.GetEnumValueFor((int)PStyle.Alignment);
                }
                else if (e.Property.ID == c_sPropKeepWithNext)
                {
                    BoolPropertySpec ps = e.Property as BoolPropertySpec;
                    Debug.Assert(null != ps);
                    e.Value = ps.GetBoolString(PStyle.KeepWithNext);
                }
                else if (e.Property.ID == c_sPropLeftMargin)
                {
                    e.Value = SetDoubleToGridText(PStyle.LeftMargin, "\"");
                }
                else if (e.Property.ID == c_sPropRightMargin)
                {
                    e.Value = SetDoubleToGridText(PStyle.RightMargin, "\"");
                }
                else if (e.Property.ID == c_sPropFirstLine)
                {
                    e.Value = SetDoubleToGridText(PStyle.FirstLineIndent, "\"");
                }
                else if (e.Property.ID == c_sPropSpaceBefore)
                {
                    e.Value = SetIntToGridText(PStyle.SpaceBefore, " pt");
                }
                else if (e.Property.ID == c_sPropSpaceAfter)
                {
                    e.Value = SetIntToGridText(PStyle.SpaceAfter, " pt");
                }
            }
        }
        #endregion
        #region Method: void bag_SetValue(object sender, PropertySpecEventArgs e)
        void bag_SetValue(object sender, PropertySpecEventArgs e)
        {
            // Character Settings
            if (e.Property.ID == c_sPropForeColor)
            {
                CStyle.FontColor = Color.FromName((string)e.Value);
            }

            // Paragraph Settings
            if (null != PStyle)
            {
                if (e.Property.ID == c_sPropAlignment)
                {
                    EnumPropertySpec ps = e.Property as EnumPropertySpec;
                    Debug.Assert(null != ps);
                    PStyle.Alignment = (JParagraphStyle.AlignType)
                        ps.GetEnumNumberFor((string)e.Value);
                }
                else if (e.Property.ID == c_sPropKeepWithNext)
                {
                    BoolPropertySpec ps = e.Property as BoolPropertySpec;
                    Debug.Assert(null != ps);
                    PStyle.KeepWithNext = ps.IsTrue((string)e.Value);
                }
                else if (e.Property.ID == c_sPropLeftMargin)
                {
                    PStyle.LeftMargin = GetDoubleFromGridText((string)e.Value);
                }
                else if (e.Property.ID == c_sPropRightMargin)
                {
                    PStyle.RightMargin = GetDoubleFromGridText((string)e.Value);
                }
                else if (e.Property.ID == c_sPropFirstLine)
                {
                    PStyle.FirstLineIndent = GetDoubleFromGridText((string)e.Value);
                }
                else if (e.Property.ID == c_sPropSpaceBefore)
                {
                    PStyle.SpaceBefore = (int)GetDoubleFromGridText((string)e.Value);
                }
                else if (e.Property.ID == c_sPropSpaceAfter)
                {
                    PStyle.SpaceAfter = (int)GetDoubleFromGridText((string)e.Value);
                }
            }
        }
        #endregion
        #region Method: void SetupPropertyGrid()
        void SetupPropertyGrid()
        {
            // Create the PropertyBag for this style
            m_bag = new PropertyBag();
            Bag.GetValue += new PropertySpecEventHandler(bag_GetValue);
            Bag.SetValue += new PropertySpecEventHandler(bag_SetValue);

            // WritingSystem / Font Settings
            #region WRITING SYSTEMS / FONT SETTINGS SETUP
            m_aFontBags = new ArrayList();
            CStyle.FontsForWritingSystems.ForceSort();
            foreach (JFontForWritingSystem fws in CStyle.FontsForWritingSystems)
            {
                // We only want to display those writing systems that we are actually using
                // in the project; rather than the whole universe of WS's that ship with OW.
                JWritingSystem ws = fws.WritingSystem;
                foreach (DTranslation t in G.Project.AllTranslations)
                {
                    if (t.WritingSystemConsultant != ws && t.WritingSystemVernacular != ws)
                        goto loop;
                }

                // Create the FontBag for this writing system
                FontPropertyBag FontBag = new FontPropertyBag(fws);
                PropertySpec ps = new PropertySpec(
                    "propFonts",
                    FontBag.Name,
                    typeof(FontPropertyBag),
                    c_sPropCharacterSettings,
                    "These are the font settings to use for this writing system.",
                    FontBag,
                    typeof(System.Drawing.Design.UITypeEditor),
                    typeof(PropertyBagTypeConverter));
                ps.DontLocalizeName = true;
                Bag.Properties.Add(ps);

                m_aFontBags.Add(FontBag);

            loop:
                ;
            }
            #endregion

            // Character Settings
            #region CHARACTER SETTINGS SETUP
            Bag.Properties.Add(PropertySpec.CreateColorPropertySpec(
                c_sPropForeColor,
                "Color",
                c_sPropCharacterSettings,
                "The color of the font text.",
                "Black"));
            #endregion

            // Paragraph Settings
            #region PARAGRAPH SETTINGS SETUP
            if (null != PStyle)
            {
                // The LeftMargin + FirstLine must be >= 0; we cannot have a 
                // negative first line that is not compensated for by the LeftMargin
                if (PStyle.FirstLineIndent < 0)
                    PStyle.LeftMargin = Math.Max(PStyle.LeftMargin, -PStyle.FirstLineIndent);

                // Alignment
                Bag.Properties.Add(new EnumPropertySpec(
                    c_sPropAlignment,
                    "Alignment",
                    c_sPropParagraphSettings,
                    "Specify Centered, Left, Right, or Justified paragraph alignment.",
                    typeof(JParagraphStyle.AlignType),
                    new int[] { 
                        (int)JParagraphStyle.AlignType.kLeft, 
                        (int)JParagraphStyle.AlignType.kCentered,
                        (int)JParagraphStyle.AlignType.kRight,
                        (int)JParagraphStyle.AlignType.kJustified
                        },
                    new string[] { 
                        "Left", 
                        "Centered", 
                        "Right", 
                        "Justified" 
                    },
                    "Justified"
                    ));

                // Keep With Next Paragraph
                Bag.Properties.Add( new BoolPropertySpec(
                    c_sPropKeepWithNext,
                    "Keep with Next Paragraph?",
                    c_sPropParagraphSettings,
                    "If Yes, there will not be a page break between paragraphs.",
                    new string[] {"Yes", "No"},
                    "No"
                    ));

                // Left Margin
                Bag.Properties.Add(new PropertySpec(
                    c_sPropLeftMargin,
                    "Left Margin",
                    typeof(string),
                    c_sPropParagraphSettings,
                    "The Left Margin for the paragraph.",
                    "0.00\""));

                // Right Margin
                Bag.Properties.Add(new PropertySpec(
                    c_sPropRightMargin,
                    "Right Margin",
                    typeof(string),
                    c_sPropParagraphSettings,
                    "The Right Margin for the paragraph.",
                    "0.00\""));

                // First Line Indent
                Bag.Properties.Add(new PropertySpec(
                    c_sPropFirstLine,
                    "First Line Indent",
                    typeof(string),
                    c_sPropParagraphSettings,
                    "The First Line Indentation for the paragraph.",
                    "0.00\""));

                // Space Before Paragraph
                Bag.Properties.Add(new PropertySpec(
                    c_sPropSpaceBefore,
                    "Space Before",
                    typeof(string),
                    c_sPropParagraphSettings,
                    "The points to add before the paragraph.",
                    "0 pt"));

                // Space After Paragraph
                Bag.Properties.Add(new PropertySpec(
                    c_sPropSpaceAfter,
                    "Space After",
                    typeof(string),
                    c_sPropParagraphSettings,
                    "The points to add following the paragraph.",
                    "6 pt"));
            }
            #endregion

            // Localization
            LocDB.Localize(this, Bag);

            // Set the Property Grid to this PropertyBag
            m_PropGrid.SelectedObject = Bag;
        }
        #endregion

        // Command Handlers ------------------------------------------------------------------
        #region Command: cmdLoad
        private void cmdLoad(object sender, System.EventArgs e)
        {
            // Label text in the appropriate language
            Control[] vExclude = new Control[] { m_PropGrid };
            LocDB.Localize(this, vExclude);

            // Create the image list for the list view
            ImageList images = new ImageList();
            Icon ic = JWU.GetIcon("Character.ico");
            images.Images.Add(ic);
            Icon ip = JWU.GetIcon("Paragraph.ico");
            images.Images.Add(ip);
            m_listStyles.SmallImageList = images;

            // The list of styles from our stylesheet
            PopulateList();

            // Select the first style in the list
            SelectListItem(0);
        }
        #endregion
        #region Command: cmdListSelectionChanged
        private void cmdListSelectionChanged(object sender, System.EventArgs e)
        {
            // Retrieve the stylename from the list
            if (m_listStyles.SelectedItems.Count != 1)
                return;
            ListViewItem item = m_listStyles.SelectedItems[0];
            string sStyleName = item.Text;

            // Retrieve the style from the stylesheet
            JParagraphStyle pstyle = G.StyleSheet.FindParagraphStyleByDisplayName(sStyleName);
            JCharacterStyle cstyle = (null == pstyle) ?
                G.StyleSheet.FindCharacterStyleByDisplayName(sStyleName) :
                pstyle.CharacterStyle;
            if (null == cstyle)
                return;
            PStyle = pstyle;
            CStyle = cstyle;
            CStyle.EnsureFontsForWritingSystems();

            // Set up the Grid Control
            SetupPropertyGrid();
        }
        #endregion
        #region Command: cmdRestoreDefaultValues
        private void cmdRestoreDefaultValues(object sender, System.EventArgs e)
        {
            // Make sure the user really wants to do this; as this applies to the entire
            // stylesheet, not just to the currently-selected individual style.
            if (false == Messages.ConfirmResetStylesToDefaults())
                return;

            // Zero out the current styles, so we don't attempt to harvest settings to
            // objects we no longer care about.
            CStyle = null;
            PStyle = null;

            // Rebuild the styles
            G.StyleSheet.Initialize(true);

            // Refresh this dialog (we must change the selection so that we
            // get the correct data on the right-hand side of the dialog.)
            PopulateList();
            SelectListItem(0);
        }
        #endregion
        #region Command: cmdKeyPress - restricts contents of the edit boxes
        private void cmdKeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            if (false == char.IsDigit(e.KeyChar) &&
                e.KeyChar != '.' &&
                e.KeyChar != ',' &&
                e.KeyChar != '-')
            {
                e.Handled = true;
            }
        }
        #endregion

        // Misc Methods ----------------------------------------------------------------------
        #region Method: void PopulateList()
        private void PopulateList()
        {
            // Remove any items currently in the list (else we'd be adding duplicates)
            m_listStyles.Items.Clear();

            // Add the paragraph styles
            foreach (JParagraphStyle style in G.StyleSheet.ParagraphStyles)
                m_listStyles.Items.Add(new ListViewItem(style.DisplayName, 1));

            // Add the character styles which are not OW-internal ones
            foreach (JCharacterStyle style in G.StyleSheet.CharacterStyles)
            {
                bool bInternal = false;
                if (style.Abbrev == DStyleSheet.c_StyleAbbrevUnderline)
                    bInternal = true;
                if (style.Abbrev == DStyleSheet.c_StyleAbbrevDashed)
                    bInternal = true;
                if (style.Abbrev == "ibtGloss")
                    bInternal = true;
                if (style.Abbrev == "ibtAn")
                    bInternal = true;

                if (!bInternal)
                    m_listStyles.Items.Add(new ListViewItem(style.DisplayName, 0));
            }
        }
        #endregion
        #region Method: void SelectListItem(int i)
        private void SelectListItem(int i)
        {
            if (m_listStyles.Items.Count > i)
                m_listStyles.Items[i].Selected = true;
        }
        #endregion
    }

}


