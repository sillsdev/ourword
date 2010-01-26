#region ***** Page_StyleSheet.cs *****
/**********************************************************************************************
 * Project: Our Word!
 * File:    Page_StyleSheet.cs
 * Author:  John Wimbish
 * Created: 17 May 2006
 * Purpose: Create settings for the Front, or open existing ones.
 * Legal:   Copyright (c) 2004-09, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;
using JWTools;
using OurWordData.DataModel;
using OurWordData.Styles;
#endregion

namespace OurWord.Dialogs
{
    public class Page_StyleSheet : DlgPropertySheet
    {
        // Attrs -----------------------------------------------------------------------------
        #region Attr{g/s}: CharacterStyle CurrentStyle
        CharacterStyle CurrentStyle
        {
            get
            {
                Debug.Assert(null != m_Style);
                return m_Style;
            }
            set
            {
                m_Style = value;
            }
        }
        private CharacterStyle m_Style;
        #endregion
        #region VAttr{g}: ParagraphStyle CurrentParagraphStyle
        ParagraphStyle CurrentParagraphStyle
        {
            get
            {
                return CurrentStyle as ParagraphStyle;
            }
        }
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
        private System.ComponentModel.Container components = null;
        #endregion
        #region Constructor()
        public Page_StyleSheet(DialogProperties parentDlg)
            : base(parentDlg)
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
			this.SuspendLayout();
			// 
			// m_listStyles
			// 
			this.m_listStyles.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)));
			this.m_listStyles.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.m_col1});
			this.m_listStyles.FullRowSelect = true;
			this.m_listStyles.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			this.m_listStyles.HideSelection = false;
			this.m_listStyles.Location = new System.Drawing.Point(0, 32);
			this.m_listStyles.MultiSelect = false;
			this.m_listStyles.Name = "m_listStyles";
			this.m_listStyles.ShowItemToolTips = true;
			this.m_listStyles.Size = new System.Drawing.Size(157, 287);
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
			this.m_labelInstructions.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.m_labelInstructions.Location = new System.Drawing.Point(0, 0);
			this.m_labelInstructions.Name = "m_labelInstructions";
			this.m_labelInstructions.Size = new System.Drawing.Size(468, 29);
			this.m_labelInstructions.TabIndex = 2;
			this.m_labelInstructions.Text = "Select a style from the list on the left, then edit its settings below. Some sett" +
				"ings require you to click on the plus sign in order to edit their settings.";
			this.m_labelInstructions.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// m_btnRestoreDefaults
			// 
			this.m_btnRestoreDefaults.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.m_btnRestoreDefaults.Location = new System.Drawing.Point(0, 325);
			this.m_btnRestoreDefaults.Name = "m_btnRestoreDefaults";
			this.m_btnRestoreDefaults.Size = new System.Drawing.Size(160, 23);
			this.m_btnRestoreDefaults.TabIndex = 30;
			this.m_btnRestoreDefaults.Text = "Restore Default Values...";
			this.m_btnRestoreDefaults.Click += new System.EventHandler(this.cmdRestoreDefaultValues);
			// 
			// m_PropGrid
			// 
			this.m_PropGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.m_PropGrid.LineColor = System.Drawing.SystemColors.ScrollBar;
			this.m_PropGrid.Location = new System.Drawing.Point(163, 32);
			this.m_PropGrid.Name = "m_PropGrid";
			this.m_PropGrid.PropertySort = System.Windows.Forms.PropertySort.Categorized;
			this.m_PropGrid.Size = new System.Drawing.Size(305, 316);
			this.m_PropGrid.TabIndex = 32;
			this.m_PropGrid.ToolbarVisible = false;
			// 
			// Page_StyleSheet
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.Controls.Add(this.m_PropGrid);
			this.Controls.Add(this.m_btnRestoreDefaults);
			this.Controls.Add(this.m_labelInstructions);
			this.Controls.Add(this.m_listStyles);
			this.Name = "Page_StyleSheet";
			this.Size = new System.Drawing.Size(468, 349);
			this.Load += new System.EventHandler(this.cmdLoad);
			this.ResumeLayout(false);

        }
        #endregion

        // DlgPropertySheet overrides --------------------------------------------------------
		#region OAttr{g}: string ID
		public override string ID
		{
			get
			{
				return "idStyleSheet";
			}
		}
		#endregion
		#region Method: void ShowHelp()
        public override void ShowHelp()
        {
            HelpSystem.ShowTopic(HelpSystem.Topic.kStyleSheet);
        }
        #endregion
        #region Attr{g}: string TabText
        public override string Title
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

        #region SMethod: double GetDoubleFromGridText(string s)
        private static double GetDoubleFromGridText(IEnumerable<char> s)
        {
            // Extract the numerical part of the string
            var sValue = "";
            foreach (var ch in s)
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
        #region SMethod: string SetDoubleToGridText(double d, sTag)
        static string SetDoubleToGridText(double d, string sTag)
        {
            return (d.ToString("0.00") + sTag);
        }
        #endregion
        #region SMethod: string SetIntToGridText(double d, sTag)
        static string SetIntToGridText(int n, string sTag)
        {
            return (n + sTag);
        }
        #endregion

        #region EMBEDDED CLASS: FontPropertyBag : PropertyBag - inner properties for a font
        class FontPropertyBag : PropertyBag
        {
            const string c_sPropFontName = "propFontName";
            const string c_sPropFontHeight = "propFontHeight";
            const string c_sPropBold = "propBold";
            const string c_sPropItalic = "propItalic";
            const string c_sPropStrikeout = "propStrikeout";

            #region Attr{g}: FontFactory Factory
            private FontFactory Factory
            {
                get
                {
                    Debug.Assert(null != m_factory);
                    return m_factory;
                }
            }
            readonly FontFactory m_factory;
            #endregion
            #region VAttr{g}: string Name
            public string Name
            {
                get
                {
                    return Factory.WritingSystemName;
                }
            }
            #endregion

            #region Method: void FontPropertyBag_GetValue(...)
            private void FontPropertyBag_GetValue(object sender, PropertySpecEventArgs e)
            {
                switch (e.Property.ID)
                {
                    case c_sPropFontName:
                        e.Value = Factory.FontName;
                        break;
                    case c_sPropFontHeight:
                        e.Value = Factory.FontSize;
                        break;
                    case c_sPropBold:
                        {
                            var ps = e.Property as YesNoPropertySpec;
                            Debug.Assert(null != ps);
                            e.Value = ps.GetBoolString(Factory.IsBold);
                        }
                        break;
                    case c_sPropItalic:
                        {
                            var ps = e.Property as YesNoPropertySpec;
                            Debug.Assert(null != ps);
                            e.Value = ps.GetBoolString(Factory.IsItalic);
                        }
                        break;
                    case c_sPropStrikeout:
                        {
                            var ps = e.Property as YesNoPropertySpec;
                            Debug.Assert(null != ps);
                            e.Value = ps.GetBoolString(Factory.IsStrikeout);
                        }
                        break;
                }
            }

            #endregion
            #region Method: void FontPropertyBag_SetValue(...)
            private void FontPropertyBag_SetValue(object sender, PropertySpecEventArgs e)
            {
                switch (e.Property.ID)
                {
                    case c_sPropFontName:
                        Factory.FontName = (string)e.Value;
                        break;
                    case c_sPropFontHeight:
                        Factory.FontSize = Convert.ToInt16(e.Value);
                        break;
                    case c_sPropBold:
                        {
                            var ps = e.Property as YesNoPropertySpec;
                            Debug.Assert(null != ps);
                            Factory.IsBold = ps.IsTrue(e.Value);
                        }
                        break;
                    case c_sPropItalic:
                        {
                            var ps = e.Property as YesNoPropertySpec;
                            Debug.Assert(null != ps);
                            Factory.IsItalic = ps.IsTrue(e.Value);
                        }
                        break;
                    case c_sPropStrikeout:
                        {
                            var ps = e.Property as YesNoPropertySpec;
                            Debug.Assert(null != ps);
                            Factory.IsStrikeout = ps.IsTrue(e.Value);
                        }
                        break;
                }
            }

            #endregion

            #region Constructor(FontFactory)
            public FontPropertyBag(FontFactory factory)
            {
                // Remember the parameters
                Debug.Assert(null != factory);
                m_factory = factory;

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

                Properties.Add(new YesNoPropertySpec(
                    c_sPropBold,
                    "Bold?",
                    "",
                    "If Yes, the font is displayed in boldface.",
                    false
                    ));

                Properties.Add(new YesNoPropertySpec(
                    c_sPropItalic,
                    "Italic?",
                    "",
                    "If Yes, the font is displayed in italics.",
                    false
                    ));

                Properties.Add(new YesNoPropertySpec(
                   c_sPropStrikeout,
                   "Strikeout?",
                   "",
                   "If Yes, the font is displayed with a line through the middle.",
                   false
                   ));

                // Listen for the callbacks
                GetValue += FontPropertyBag_GetValue;
                SetValue += FontPropertyBag_SetValue;
            }
            #endregion

            #region OMethod: string ToString()
            public override string ToString()
            {
                // Start with the font name and the height
                var s = Factory.FontName + ", " + Factory.FontSize;

                // Add Bold if bold is true
                if (Factory.IsBold)
                {
                    s += (", ");
                    foreach (var ch in FindPropertySpec(c_sPropBold).Name)
                    {
                        if (!char.IsPunctuation(ch))
                            s += ch;
                    }
                }

                // Add Italic if italic is true
                if (Factory.IsItalic)
                {
                    s += (", ");
                    foreach (var ch in FindPropertySpec(c_sPropItalic).Name)
                    {
                        if (!char.IsPunctuation(ch))
                            s += ch;
                    }
                }

                // Add Strikeout if strikeout is true
                if (Factory.IsStrikeout)
                {
                    s += (", ");
                    foreach (var ch in FindPropertySpec(c_sPropStrikeout).Name)
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
                e.Value = CurrentStyle.FontColor.Name;
            }

            // Paragraph Settings
            if (null == CurrentParagraphStyle) 
                return;

            switch (e.Property.ID)
            {
                case c_sPropAlignment:
                    {
                        var ps = e.Property as EnumPropertySpec;
                        Debug.Assert(null != ps);
                        e.Value = ps.GetEnumValueFor((int)CurrentParagraphStyle.Alignment);
                    }
                    break;
                case c_sPropKeepWithNext:
                    {
                        var ps = e.Property as YesNoPropertySpec;
                        Debug.Assert(null != ps);
                        e.Value = ps.GetBoolString(CurrentParagraphStyle.KeepWithNextParagraph);
                    }
                    break;
                case c_sPropLeftMargin:
                    e.Value = SetDoubleToGridText(CurrentParagraphStyle.LeftMarginInches, "\"");
                    break;
                case c_sPropRightMargin:
                    e.Value = SetDoubleToGridText(CurrentParagraphStyle.RightMarginInches, "\"");
                    break;
                case c_sPropFirstLine:
                    e.Value = SetDoubleToGridText(CurrentParagraphStyle.FirstLineIndentInches, "\"");
                    break;
                case c_sPropSpaceBefore:
                    e.Value = SetIntToGridText(CurrentParagraphStyle.PointsBefore, " pt");
                    break;
                case c_sPropSpaceAfter:
                    e.Value = SetIntToGridText(CurrentParagraphStyle.PointsAfter, " pt");
                    break;
            }
        }
        #endregion
        #region Method: void bag_SetValue(object sender, PropertySpecEventArgs e)
        void bag_SetValue(object sender, PropertySpecEventArgs e)
        {
            // Character Settings
            if (e.Property.ID == c_sPropForeColor)
            {
                CurrentStyle.FontColor = Color.FromName((string)e.Value);
            }

            // Paragraph Settings
            if (null == CurrentParagraphStyle) 
                return;
            switch (e.Property.ID)
            {
                case c_sPropAlignment:
                    {
                        var ps = e.Property as EnumPropertySpec;
                        Debug.Assert(null != ps);
                        CurrentParagraphStyle.Alignment = (ParagraphStyle.Align)
                                                          ps.GetEnumNumberFor((string)e.Value);
                    }
                    break;
                case c_sPropKeepWithNext:
                    {
                        var ps = e.Property as YesNoPropertySpec;
                        Debug.Assert(null != ps);
                        CurrentParagraphStyle.KeepWithNextParagraph = ps.IsTrue(e.Value);
                    }
                    break;
                case c_sPropLeftMargin:
                    CurrentParagraphStyle.LeftMarginInches = GetDoubleFromGridText((string)e.Value);
                    break;
                case c_sPropRightMargin:
                    CurrentParagraphStyle.RightMarginInches = GetDoubleFromGridText((string)e.Value);
                    break;
                case c_sPropFirstLine:
                    CurrentParagraphStyle.FirstLineIndentInches = GetDoubleFromGridText((string)e.Value);
                    break;
                case c_sPropSpaceBefore:
                    CurrentParagraphStyle.PointsBefore = (int)GetDoubleFromGridText((string)e.Value);
                    break;
                case c_sPropSpaceAfter:
                    CurrentParagraphStyle.PointsAfter = (int)GetDoubleFromGridText((string)e.Value);
                    break;
            }
        }
        #endregion
        #region Method: void SetupPropertyGrid()
        void SetupPropertyGrid()
        {
            // Create the PropertyBag for this style
            m_bag = new PropertyBag();
            Bag.GetValue += bag_GetValue;
            Bag.SetValue += bag_SetValue;

            // WritingSystem / Font Settings
            #region WRITING SYSTEMS / FONT SETTINGS SETUP
            m_aFontBags = new ArrayList();

            foreach (var factory in CurrentStyle.FontFactories)
            {
                // Create the FontBag for this writing system
                var fontBag = new FontPropertyBag(factory);
                var ps = new PropertySpec(
                    "propFonts",
                    fontBag.Name,
                    typeof(FontPropertyBag),
                    c_sPropCharacterSettings,
                    "These are the font settings to use for this writing system.",
                    fontBag,
                    typeof(UITypeEditor),
                    typeof(PropertyBagTypeConverter)) 
                    {DontLocalizeName = true};
                Bag.Properties.Add(ps);

                m_aFontBags.Add(fontBag);
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
            if (null != CurrentParagraphStyle)
            {
                // The HorzMargin + FirstLine must be >= 0; we cannot have a 
                // negative first line that is not compensated for by the HorzMargin
                if (CurrentParagraphStyle.FirstLineIndentInches < 0)
                    CurrentParagraphStyle.LeftMarginInches = Math.Max(
                        CurrentParagraphStyle.LeftMarginInches, 
                        -CurrentParagraphStyle.FirstLineIndentInches);

                // Alignment
                Bag.Properties.Add(new EnumPropertySpec(
                    c_sPropAlignment,
                    "Alignment",
                    c_sPropParagraphSettings,
                    "Specify Centered, Left, Right, or Justified paragraph alignment.",
                    typeof(ParagraphStyle.Align),
                    new[] { 
                        (int)ParagraphStyle.Align.Left, 
                        (int)ParagraphStyle.Align.Centered,
                        (int)ParagraphStyle.Align.Right,
                        (int)ParagraphStyle.Align.Justified
                        },
                    new[] { 
                        "Left", 
                        "Centered", 
                        "Right", 
                        "Justified" 
                    },
                    "Justified"
                    ));

                // Keep With Next Paragraph
                Bag.Properties.Add( new YesNoPropertySpec(
                    c_sPropKeepWithNext,
                    "Keep with Next Paragraph?",
                    c_sPropParagraphSettings,
                    "If Yes, there will not be a page break between paragraphs.",
                    false
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
        private void cmdLoad(object sender, EventArgs e)
        {
            // Label text in the appropriate language
            var vExclude = new Control[] { m_PropGrid };
            LocDB.Localize(this, vExclude);

            // Create the image list for the list view
            var images = new ImageList();
            var ic = JWU.GetIcon("Character.ico");
            images.Images.Add(ic);
            var ip = JWU.GetIcon("Paragraph.ico");
            images.Images.Add(ip);
            m_listStyles.SmallImageList = images;

            // The list of styles from our stylesheet
            PopulateList();

            // Select the first style in the list
            SelectListItem(0);
        }
        #endregion
        #region Command: cmdListSelectionChanged
        private void cmdListSelectionChanged(object sender, EventArgs e)
        {
            // Retrieve the stylename from the list
            if (m_listStyles.SelectedItems.Count != 1)
                return;
            var item = m_listStyles.SelectedItems[0];
            var sStyleName = item.Text;

            // Retrieve the style from the stylesheet
            CurrentStyle = StyleSheet.Find(sStyleName);
            CurrentStyle.EnsureFontsForWritingSystems();

            // Set up the Grid Control
            SetupPropertyGrid();
        }
        #endregion
        #region Command: cmdRestoreDefaultValues
        private void cmdRestoreDefaultValues(object sender, EventArgs e)
        {
            // Make sure the user really wants to do this; as this applies to the entire
            // stylesheet, not just to the currently-selected individual style.
            if (false == Messages.ConfirmResetStylesToDefaults())
                return;

            // Zero out the current style, so we don't attempt to harvest settings to
            // objects we no longer care about.
            CurrentStyle = null;

            // Rebuild the styles
            StyleSheet.ResetStylesToFactory();

            // Reset the underlying window. (See B0292). The issue is that when we rebuild
            // the styles, we leave the underlying window in a bad state; the blocks are 
            // pointing to obsolete JFontForWritingSystem objects. If OW is minimized (prior
            // to the dialog being closed), then restored, we get a crash if we don't first
            // reset these contents. An unlikely scenario, but it happened once, and so.....
            G.App.ResetWindowContents();

            // Refresh this dialog (we must change the selection so that we
            // get the correct data on the right-hand side of the dialog.)
            PopulateList();
            SelectListItem(0);
        }
        #endregion
        #region Command: cmdKeyPress - restricts contents of the edit boxes
        private void cmdKeyPress(object sender, KeyPressEventArgs e)
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

            foreach(var style in StyleSheet.StyleList)
            {
                var iImage = style.IsParagraphStyle ? 1 : 0;
                var item = new ListViewItem(style.StyleName, iImage);
                m_listStyles.Items.Add(item);
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


