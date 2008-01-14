/**********************************************************************************************
 * Project: Our Word!
 * File:    Page_Options.cs
 * Author:  John Wimbish
 * Created: 21 Sep 2007
 * Purpose: Sets up the general options
 * Legal:   Copyright (c) 2005-08, John S. Wimbish. All Rights Reserved.  
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
using OurWord.DataModel;
using OurWord.Dialogs;
using OurWord.Edit;
using OurWord.View;
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

        const string c_sGroup_BackgroundColors = "propBackgroundColors";
        const string c_sColorDrafting = "propBackColorDraftingWindow";
        const string c_sColorBackTranslation = "propBackColorBackTranslationWindow";
        const string c_sColorNaturalnessCheck = "propBackColorNaturalnessCheckWindow";
        const string c_sColorNotes = "propBackColorNotesWindow";
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
                    e.Value = G.PictureSearchPath;
                    break;

                case c_sColorDrafting:
                    e.Value = WndDrafting.RegistryBackgroundColor;
                    break;
                case c_sColorBackTranslation:
                    e.Value = WndBackTranslation.RegistryBackgroundColor;
                    break;
                case c_sColorNaturalnessCheck:
                    e.Value = WndNaturalness.RegistryBackgroundColor;
                    break;
                case c_sColorNotes:
                    e.Value = NotesWindow.RegistryBackgroundColor;
                    break;
                case c_sColorOtherTranslations:
                    e.Value = TranslationsWindow.RegistryBackgroundColor;
                    break;
            }
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
                    G.PictureSearchPath = (string)e.Value;
                    break;

                case c_sZoomFactor:
                    ZoomPropertySpec ZoomPS = e.Property as ZoomPropertySpec;
                    Debug.Assert(null != ZoomPS);
                    G.ZoomPercent = ZoomPS.GetZoomFactor(e.Value);
                    break;

                case c_sColorDrafting:
                    WndDrafting.RegistryBackgroundColor = (string)e.Value;
                    break;
                case c_sColorBackTranslation:
                    WndBackTranslation.RegistryBackgroundColor = (string)e.Value;
                    break;
                case c_sColorNaturalnessCheck:
                    WndNaturalness.RegistryBackgroundColor = (string)e.Value;
                    break;
                case c_sColorNotes:
                    NotesWindow.RegistryBackgroundColor = (string)e.Value;
                    break;
                case c_sColorOtherTranslations:
                    TranslationsWindow.RegistryBackgroundColor = (string)e.Value;
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
                "Text in the windows can be larger (or smaller) by the chosen percentage.",
                new int[] { 60, 70, 80, 90, 100, 110, 120, 130, 140, 150, 175, 200, 225, 250 },
                100
                );
            zps.DontLocalizeEnums = true;
            Bag.Properties.Add(zps);

            // Window Background Colors
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
                c_sColorNotes,
                "Notes",
                c_sGroup_BackgroundColors,
                "The color of the Notes window background.",
                "Wheat"));
            Bag.Properties.Add(PropertySpec.CreateColorPropertySpec(
                c_sColorOtherTranslations,
                "Other Translations",
                c_sGroup_BackgroundColors,
                "The color of the Other Translations window background.",
                "Wheat"));

            // Localize the bag
            LocDB.Localize(this, Bag);

            // Set the Property Grid to this PropertyBag
            m_PropGrid.SelectedObject = Bag;
        }
        #endregion

        // DlgPropertySheet overrides --------------------------------------------------------
        #region Method: void ShowHelp()
        public override void ShowHelp()
        {
            //HelpSystem.ShowPage_Translation();
        }
        #endregion
        #region Attr{g}: string TabText
        public override string TabText
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
