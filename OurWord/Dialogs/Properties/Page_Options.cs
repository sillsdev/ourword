/**********************************************************************************************
 * Project: Our Word!
 * File:    Page_Options.cs
 * Author:  John Wimbish
 * Created: 21 Sep 2007
 * Purpose: Sets up the general options
 * Legal:   Copyright (c) 2005-09, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
#region Header: Using, etc.
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Globalization;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Resources;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using System.Threading;

using JWTools;
using JWdb;
using OurWord;
using JWdb.DataModel;
using OurWord.Dialogs;
using OurWord.Edit;
using OurWord.SideWnd;
using OurWord.Layouts;
#endregion


namespace OurWord.Dialogs
{
    public partial class Page_Options : DlgPropertySheet
    {
        // Scaffolding -----------------------------------------------------------------------
        #region Constructor(DlgProperties)
        public Page_Options(DialogProperties _ParentDlg)
            : base(_ParentDlg)
        {
            InitializeComponent();
        }
        #endregion

        // Property Grid ---------------------------------------------------------------------
        #region BAG CONSTANTS
        const string c_sGroupUILanguage = "propUserInterfaceLanguage";
        const string c_sPrimaryLanguage = "propPrimaryLanguage";
        const string c_sSecondaryLanguage = "propSecondaryLanguage";

        const string c_sMaximizeWindowOnStartup = "propMaximizeWindowOnStartup";
        const string c_sPicturesPath = "propPicturesPath";
        const string c_sBackupPath = "propBackupPath";
        const string c_sMakeBackups = "propMakeBackups";
        const string c_sZoomFactor = "propZoomFactor";

        const string c_sGroupNaturalnessCheck = "propNaturalnessCheck";
        const string c_sSuppressVerses = "propNCSuppressVerses";
        const string c_sShowLineNumbers = "propNCShowLineNumbers";
        const string c_sLineNumberColor = "propNCLineNumbebrColor";

        const string c_sGroupBackTranslation = "propBackTranslation";
        const string c_sShowFrontVernacular = "propShowFrontVernacular";
        const string c_sShowFrontBT = "propShowFrontBT";
#if FEATURE_WESAY
        const string c_sGroupWeSayDictionary = "propDictionary";
        const string c_sPathToDictionaryApp = "propDictApp";
        const string c_sPathToDictionaryData = "propDictData";
#endif
        const string c_sGroup_BackgroundColors = "propBackgroundColors";
        const string c_sColorDrafting = "propBackColorDraftingWindow";
        const string c_sColorBackTranslation = "propBackColorBackTranslationWindow";
        const string c_sColorNaturalnessCheck = "propBackColorNaturalnessCheckWindow";
        const string c_sColorOtherTranslations = "propBackColorOtherTranslationsWindow";
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
        #region Method: void bag_GetValue(object sender, PropertySpecEventArgs e)
        void bag_GetValue(object sender, PropertySpecEventArgs e)
        {
            switch (e.Property.ID)
            {
                case c_sPrimaryLanguage:
                    e.Value = (null != LocDB.DB.PrimaryLanguage) ?
                        LocDB.DB.PrimaryLanguage.Name :
                        LocItem.c_sEnglish;
                    break;
                case c_sSecondaryLanguage:
                    e.Value = (null != LocDB.DB.SecondaryLanguage) ?
                        LocDB.DB.SecondaryLanguage.Name :
                        LocItem.c_sEnglish;
                    break;

                case c_sMaximizeWindowOnStartup:
                    YesNoPropertySpec ps = e.Property as YesNoPropertySpec;
                    Debug.Assert(null != ps);
                    e.Value = ps.GetBoolString(OurWordMain.App.StartMaximized);
                    break;

                case c_sMakeBackups:
                    YesNoPropertySpec BackupPS = e.Property as YesNoPropertySpec;
                    Debug.Assert(null != BackupPS);
                    e.Value = BackupPS.GetBoolString(BackupSystem.Enabled);
                    break;

                case c_sZoomFactor:
                    ZoomPropertySpec ZoomPS = e.Property as ZoomPropertySpec;
                    Debug.Assert(null != ZoomPS);
                    e.Value = ZoomPS.GetZoomString(G.ZoomPercent);
                    break;

                case c_sBackupPath:
                    e.Value = BackupSystem.RegistryBackupFolder;
                    break;

                case c_sPicturesPath:
                    e.Value = DB.PictureSearchPath;
                    break;

                // Naturalness Check view
                case c_sSuppressVerses:
                    YesNoPropertySpec SuppressPS = e.Property as YesNoPropertySpec;
                    Debug.Assert(null != SuppressPS);
                    e.Value = SuppressPS.GetBoolString(WndNaturalness.SupressVerseNumbers);
                    break;
                case c_sShowLineNumbers:
                    YesNoPropertySpec ShowLineNumbersPS = e.Property as YesNoPropertySpec;
                    Debug.Assert(null != ShowLineNumbersPS);
                    e.Value = ShowLineNumbersPS.GetBoolString(WndNaturalness.ShowLineNumbers);
                    break;
                case c_sLineNumberColor:
                    e.Value = WndNaturalness.LineNumbersColor;
                    break;

                // Back Translation view
                case c_sShowFrontVernacular:
                    {
                        YesNoPropertySpec PS = e.Property as YesNoPropertySpec;
                        Debug.Assert(null !=PS);
                        e.Value = PS.GetBoolString(WndBackTranslation.DisplayFrontVernacular);
                        break;
                    }
                case c_sShowFrontBT:
                    {
                        YesNoPropertySpec PS = e.Property as YesNoPropertySpec;
                        Debug.Assert(null != PS);
                        e.Value = PS.GetBoolString(WndBackTranslation.DisplayFrontBT);
                        break;
                    }

#if FEATURE_WESAY
                // WeSay Dictionary Setup
                case c_sPathToDictionaryApp:
                    e.Value = new PathForPropertyGrid(
                        G.Project.PathToDictionaryApp, 
                        "Application (*.exe)| *.exe");
                    break;
                case c_sPathToDictionaryData:
                    e.Value = new PathForPropertyGrid(
                        G.Project.PathToDictionaryData, 
                        "WeSay Dictionary(.lift)| *.lift");
                    break;
#endif

                // Window background colors
                case c_sColorDrafting:
                    e.Value = WndDrafting.RegistryBackgroundColor;
                    break;
                case c_sColorBackTranslation:
                    e.Value = WndBackTranslation.RegistryBackgroundColor;
                    break;
                case c_sColorNaturalnessCheck:
                    e.Value = WndNaturalness.RegistryBackgroundColor;
                    break;
                case c_sColorOtherTranslations:
                    e.Value = TranslationsWnd.RegistryBackgroundColor;
                    break;
            }
        }
        #endregion
        #region Method: bool InterpretYesNo(PropertySpecEventArgs e)
        bool InterpretYesNo(PropertySpecEventArgs e)
        {
            YesNoPropertySpec PS = e.Property as YesNoPropertySpec;
            Debug.Assert(null != PS);
            return PS.IsTrue(e.Value);
        }
        #endregion
        #region Method: void bag_SetValue(object sender, PropertySpecEventArgs e)
        void bag_SetValue(object sender, PropertySpecEventArgs e)
        {
            switch (e.Property.ID)
            {
                case c_sPrimaryLanguage:
                    LocDB.DB.SetPrimary((string)e.Value);
                    break;
                case c_sSecondaryLanguage:
                    LocDB.DB.SetSecondary((string)e.Value);
                    break;

                case c_sMaximizeWindowOnStartup:
                    YesNoPropertySpec ps = e.Property as YesNoPropertySpec;
                    Debug.Assert(null != ps);
                    OurWordMain.App.StartMaximized = ps.IsTrue(e.Value);
                    break;

                case c_sMakeBackups:
                    YesNoPropertySpec BackupPS = e.Property as YesNoPropertySpec;
                    Debug.Assert(null != BackupPS);
                    BackupSystem.Enabled = BackupPS.IsTrue(e.Value);
                    break;

                case c_sBackupPath:
                    BackupSystem.RegistryBackupFolder = (string)e.Value;
                    break;

                case c_sPicturesPath:
                    DB.PictureSearchPath = (string)e.Value;
                    break;

                case c_sZoomFactor:
                    ZoomPropertySpec ZoomPS = e.Property as ZoomPropertySpec;
                    Debug.Assert(null != ZoomPS);
                    G.ZoomPercent = ZoomPS.GetZoomFactor(e.Value);
                    break;

                // Naturalness Check view
                case c_sSuppressVerses:
                    YesNoPropertySpec SuppressPS = e.Property as YesNoPropertySpec;
                    Debug.Assert(null != SuppressPS);
                    WndNaturalness.SupressVerseNumbers = SuppressPS.IsTrue(e.Value);
                    break;
                case c_sShowLineNumbers:
                    YesNoPropertySpec ShowLineNumbersPS = e.Property as YesNoPropertySpec;
                    Debug.Assert(null != ShowLineNumbersPS);
                    WndNaturalness.ShowLineNumbers = ShowLineNumbersPS.IsTrue(e.Value);
                    break;
                case c_sLineNumberColor:
                    WndNaturalness.LineNumbersColor = (string)e.Value;
                    break;

                // Back Translation view
                case c_sShowFrontVernacular:
                    WndBackTranslation.DisplayFrontVernacular = InterpretYesNo(e);
                    break;
                case c_sShowFrontBT:
                    WndBackTranslation.DisplayFrontBT = InterpretYesNo(e);
                    break;

#if FEATURE_WESAY
                // WeSay Dictionary Setup
                case c_sPathToDictionaryApp:
                    G.Project.PathToDictionaryApp = ((PathForPropertyGrid)e.Value).Path;
                    break;
                case c_sPathToDictionaryData:
                    G.Project.PathToDictionaryData = ((PathForPropertyGrid)e.Value).Path;
                    break;
#endif

                // Window background colors
                case c_sColorDrafting:
                    WndDrafting.RegistryBackgroundColor = (string)e.Value;
                    break;
                case c_sColorBackTranslation:
                    WndBackTranslation.RegistryBackgroundColor = (string)e.Value;
                    break;
                case c_sColorNaturalnessCheck:
                    WndNaturalness.RegistryBackgroundColor = (string)e.Value;
                    break;
                case c_sColorOtherTranslations:
                    TranslationsWnd.RegistryBackgroundColor = (string)e.Value;
                    break;
            }
        }
        #endregion
        #region Method: void SetupPropertyGrid()
        void SetupPropertyGrid()
        {
            // Create the PropertyBag for this page
            m_bag = new PropertyBag();
            Bag.GetValue += new PropertySpecEventHandler(bag_GetValue);
            Bag.SetValue += new PropertySpecEventHandler(bag_SetValue);

            // User Interface Languages
            #region (User Interface Languages)
            PropertySpec PrimaryPS = new PropertySpec(
                c_sPrimaryLanguage,
                "Preferred (primary)",
                c_sGroupUILanguage,
                "Use this language for the User Interface.",
                LocDB.DB.LanguageChoices,
                LocItem.c_sEnglish);
            PrimaryPS.DontLocalizeEnums = true;
            Bag.Properties.Add(PrimaryPS);
            PropertySpec SecondaryPS = new PropertySpec(
                c_sSecondaryLanguage,
                "Fallback (secondary)",
                c_sGroupUILanguage,
                "Use this language for the User Interface if the preferred (primary) language is unavailable.",
                LocDB.DB.LanguageChoices,
                LocItem.c_sEnglish);
            SecondaryPS.DontLocalizeEnums = true;
            Bag.Properties.Add(SecondaryPS);
            #endregion

            // Misc Options
            #region (Misc Options)
            // Maxmimze window on startup
            Bag.Properties.Add(new YesNoPropertySpec(
                c_sMaximizeWindowOnStartup,
                "Maxmimize Window on Startup?",
                "",
                "If Yes, OurWord starts up maximized. This may help newer users to have sufficient " +
                    "size on the screen for doing work.",
                true
                ));

            // Turn on the Backup System
            Bag.Properties.Add(new YesNoPropertySpec(
                c_sMakeBackups,
                "Make Automatic Backups?",
                "",
                "If Yes, OurWord will automatically back up your fields to the folder specified below.",
                true
                ));

            // Backups Path
            Bag.Properties.Add(new PropertySpec(
                c_sBackupPath,
                "Folder for Backing up files",
                typeof(string),
                "",
                "Choose a folder in which OurWord will automatically make backups of your files.",
                "",
                typeof(BackupFolderBrowseTypeEditor),
                null));

            // Pictures Path
            Bag.Properties.Add(new PropertySpec(
                c_sPicturesPath,
                "Folder to search for pictures",
                typeof(string),
                "",
                "If a picture cannot be found, OurWord will search in this folder to attempt to locate it.",
                "",
                typeof(PictureFolderBrowseTypeEditor),
                null));

            // Zoom Factor (displays as a combo, showing, e.g., "120 %")
            ZoomPropertySpec zps = new ZoomPropertySpec(
                c_sZoomFactor,
                "Zoom Factor",
                "",
                "Text in the windows can be larger (or smaller) by the chosen percentage. (You can " +
                    "also set this in the Window dropdown.)",
                new int[] { 60, 70, 80, 90, 100, 110, 120, 130, 140, 150, 175, 200, 225, 250 },
                100
                );
            zps.DontLocalizeEnums = true;
            Bag.Properties.Add(zps);
            #endregion

            // Naturalness Check options
            #region (Naturalness Check Options)
            Bag.Properties.Add(new YesNoPropertySpec(
                c_sSuppressVerses,
                "Hide verse numbers?",
                c_sGroupNaturalnessCheck,
                "If Yes, the verse numbers will not be shown in the Naturalness Check view. This may make " +
                    "it easier to see how well the discourse flows.",
                false
                ));
            Bag.Properties.Add(new YesNoPropertySpec(
                c_sShowLineNumbers,
                "Show line numbers?",
                c_sGroupNaturalnessCheck,
                "If Yes, line numbers are shown in the left margin, so that you can refer to a " +
                    "specific line as you discuss the text with others.",
                false
                ));
            Bag.Properties.Add(PropertySpec.CreateColorPropertySpec(
                c_sLineNumberColor,
                "Line Numbers Color",
                c_sGroupNaturalnessCheck,
                "The color of the line numbers in the Naturalness Check view. Choose a color that is " + 
                    "visible against the background color; yet that doesn't detract from reading the " +
                    "Scripture text.",
                "DarkGray"));
            #endregion

            // Back Translation options
            #region (Back Translation options)
            Bag.Properties.Add(new YesNoPropertySpec(
                c_sShowFrontVernacular,
                "Show Front Translation's Vernacular",
                c_sGroupBackTranslation,
                "If Yes, the vernacular text of the front translation will be shown in a column.",
                false
                ));
            Bag.Properties.Add(new YesNoPropertySpec(
                c_sShowFrontBT,
                "Show Front Translation's Back Translation",
                c_sGroupBackTranslation,
                "If Yes, the BT of the front translation will be shown in a column.",
                false
                ));
            #endregion

#if FEATURE_WESAY
            // WeSay Dictionary Setup
            #region WeSay Options
            Bag.Properties.Add(new PropertySpec(
                c_sPathToDictionaryApp,
                "Path to the WeSay App",
                typeof(PathForPropertyGrid),
                c_sGroupWeSayDictionary,
                "The full path to the application that manages the dictionary; currently WeSay.App.Exe is the only one supported.",
                "",
                typeof(FilePathEditor),
                null));
            Bag.Properties.Add(new PropertySpec(
                c_sPathToDictionaryData,
                "Path to the LIFT Dictionary",
                typeof(PathForPropertyGrid),
                c_sGroupWeSayDictionary,
                "The full path to the dictionary data as managed by WeSay, a LIFT format file.",
                null,
                typeof(FilePathEditor),
                null));
            #endregion
#endif

            // Window Background Colors
            #region (Background Colors)
            Bag.Properties.Add(PropertySpec.CreateColorPropertySpec(
                c_sColorDrafting,
                "Drafting",
                c_sGroup_BackgroundColors,
                "The color of the Drafting window background.",
                "Wheat"));
            Bag.Properties.Add(PropertySpec.CreateColorPropertySpec(
                c_sColorBackTranslation,
                "Back Translation",
                c_sGroup_BackgroundColors,
                "The color of the Back Translation window background.",
                "Wheat"));
            Bag.Properties.Add(PropertySpec.CreateColorPropertySpec(
                c_sColorNaturalnessCheck,
                "Naturalness Check",
                c_sGroup_BackgroundColors,
                "The color of the Naturalness Check window background.",
                "Wheat"));
            Bag.Properties.Add(PropertySpec.CreateColorPropertySpec(
                c_sColorOtherTranslations,
                "Other Translations",
                c_sGroup_BackgroundColors,
                "The color of the Other Translations window background.",
                "Wheat"));
            #endregion

            // Localize the bag
            LocDB.Localize(this, Bag);

            // Set the Property Grid to this PropertyBag
            m_PropGrid.SelectedObject = Bag;
        }
        #endregion

        // DlgPropertySheet overrides --------------------------------------------------------
		#region OAttr{g}: string ID
		public override string ID
		{
			get
			{
				return "idOptions";
			}
		}
		#endregion
		#region Method: void ShowHelp()
        public override void ShowHelp()
        {
            HelpSystem.ShowDefaultTopic();
        }
        #endregion
        #region Attr{g}: string TabText
        public override string Title
        {
            get
            {
                return "General Options";
            }
        }
        #endregion
        #region Method: override bool HarvestChanges()
        public override bool HarvestChanges()
        {
            return true;
        }
        #endregion

        // Command Handlers ------------------------------------------------------------------
        #region Cmd: cmdLoad
        private void cmdLoad(object sender, EventArgs e)
        {
            // Property Grid
            SetupPropertyGrid();
        }
        #endregion
    }

    // UITypeEditors used by the Property Grid
    #region CLASS: PictureFolderBrowseTypeEditor
    class PictureFolderBrowseTypeEditor : UITypeEditor
    {
        #region OMethod: GetEditStyle(...)
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }
        #endregion
        #region OMethod: EditValue(...)
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            FolderBrowserDialog dlg = new FolderBrowserDialog();
            dlg.Description = "Browse for the top-level folder that contains your pictures";
            dlg.RootFolder = Environment.SpecialFolder.MyComputer;
            if (DialogResult.OK == dlg.ShowDialog())
                return dlg.SelectedPath;

            return value;
        }
        #endregion
    }
    #endregion
    #region CLASS: BackupFolderBrowseTypeEditor
    class BackupFolderBrowseTypeEditor : UITypeEditor
    {
        #region OMethod: GetEditStyle(...)
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }
        #endregion
        #region OMethod: EditValue(...)
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            return BackupSystem.BrowseForFolder((string)value);
        }
        #endregion
    }
    #endregion

}
