/**********************************************************************************************
 * Project: Our Word!
 * File:    Page_Options.cs
 * Author:  John Wimbish
 * Created: 21 Sep 2007
 * Purpose: Sets up the general options
 * Legal:   Copyright (c) 2005-10, John S. Wimbish. All Rights Reserved.  
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
using OurWordData;
using OurWord;
using OurWordData.DataModel;
using OurWord.Dialogs;
using OurWord.Edit;
using OurWord.Layouts;
using OurWordData.DataModel.Annotations;

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
        /*
        */

        const string c_sBackupPath = "propBackupPath";
        const string c_sMakeBackups = "propMakeBackups";
        const string c_sProjectAccess = "propProjectAccess";

        private const string c_sGroupBackTranlation = "BackTranslation";
        private const string c_sCanEditTargetInBackTransView = "propCanEditTargetInBackTransView";

        const string c_sGroupNaturalnessCheck = "propNaturalnessCheck";
        const string c_sSuppressVerses = "propNCSuppressVerses";
        const string c_sShowLineNumbers = "propNCShowLineNumbers";
        const string c_sLineNumberColor = "propNCLineNumbebrColor";


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
/*
*/
                case c_sMakeBackups:
                    SetPropertySpecValue(e, BackupSystem.Enabled);
                    break;


                case c_sBackupPath:
                    e.Value = BackupSystem.RegistryBackupFolder;
                    break;

                case c_sProjectAccess:
                    e.Value = ClusterList.UserCanAccessAllProjectsFriendly;
                    break;

                // Back Translation View
                case c_sCanEditTargetInBackTransView:
                    SetPropertySpecValue(e, WndBackTranslation.CanEditTarget);
                    break;

                // Naturalness Check view
                case c_sSuppressVerses:
                    SetPropertySpecValue(e, WndNaturalness.SupressVerseNumbers);
                    break;
                case c_sShowLineNumbers:
                    SetPropertySpecValue(e, WndNaturalness.ShowLineNumbers);
                    break;
                case c_sLineNumberColor:
                    e.Value = WndNaturalness.LineNumbersColor;
                    break;

            }
        }
        #endregion
        #region SMethod: void SetPropertySpecValue(PropertySpecEventArgs e, bool b)
        static void SetPropertySpecValue(PropertySpecEventArgs e, bool b)
        {
            var ps = e.Property as YesNoPropertySpec;
            Debug.Assert(null != ps);
            e.Value = ps.GetBoolString(b);

        }
        #endregion
        #region SMethod: bool InterpretYesNo(PropertySpecEventArgs e)
        static bool InterpretYesNo(PropertySpecEventArgs e)
        {
            var ps = e.Property as YesNoPropertySpec;
            Debug.Assert(null != ps);
            return ps.IsTrue(e.Value);
        }
        #endregion
        #region Method: void bag_SetValue(object sender, PropertySpecEventArgs e)
        void bag_SetValue(object sender, PropertySpecEventArgs e)
        {
            switch (e.Property.ID)
            {
/*
*/
                case c_sMakeBackups:
                    BackupSystem.Enabled = InterpretYesNo(e);
                    break;

                case c_sBackupPath:
                    BackupSystem.RegistryBackupFolder = (string)e.Value;
                    break;


                case c_sProjectAccess:
                    break;

                // Back Translation view
                case c_sCanEditTargetInBackTransView:
                    WndBackTranslation.CanEditTarget = InterpretYesNo(e);
                    break;

                // Naturalness Check view
                case c_sSuppressVerses:
                    WndNaturalness.SupressVerseNumbers = InterpretYesNo(e);
                    break;
                case c_sShowLineNumbers:
                    WndNaturalness.ShowLineNumbers = InterpretYesNo(e);
                    break;
                case c_sLineNumberColor:
                    WndNaturalness.LineNumbersColor = (string)e.Value;
                    break;

            }
        }
        #endregion
        #region Method: void SetupPropertyGrid()
        void SetupPropertyGrid()
        {
            // Create the PropertyBag for this page
            m_bag = new PropertyBag();
            Bag.GetValue += bag_GetValue;
            Bag.SetValue += bag_SetValue;

/*
*/

            // Misc Options
            #region (Misc Options)
            // Turn on the Backup System
            Bag.Properties.Add(new YesNoPropertySpec(
                c_sMakeBackups,
                "Make Automatic Backups?",
                "",
                "If Yes, OurWord will automatically back up your fields to the folder specified below.",
                false
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

            // Project Access
            Bag.Properties.Add(new PropertySpec(
                c_sProjectAccess,
                "Projects that this user can access",
                typeof(string),
                "",
                "In a large cluster with numerous projects, you can limit the user of this computer to only a few of them.",
                "All",
                typeof(CheckTreeEditor),
                null));

            #endregion

            // Back Translation options
            #region (Back Translation Options)
            Bag.Properties.Add(new YesNoPropertySpec(
                c_sCanEditTargetInBackTransView,
                "Can edit Draft in Back Translation view?",
                c_sGroupBackTranlation,
                "If Yes, you will be able to edit the vernacular while in the Back Translation window, rather than " +
                    "having to flip back to the Draft window.",
                false
                ));
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
        #region Attr{g}: string Title
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


    // DlgCheckTree
    public class CheckTreeEditor : UITypeEditor
    {
        #region Method: void CreateCheckTreeItems(DlgCheckTree dlg)
        void CreateCheckTreeItems(DlgCheckTree dlg)
        {
            foreach (var ci in ClusterList.Clusters)
            {
                var ciItem = new CheckTreeItem(ci.Name, false, ci);
                dlg.Items.Add(ciItem);

                foreach (var sProject in ci.GetClusterLanguageList(true))
                {
                    var item = new CheckTreeItem(sProject, ci.GetUserCanAccess(sProject), sProject);
                    ciItem.SubItems.Add(item);
                }
            }
        }
        #endregion
        #region Method: void HarvestCheckTreeItems(DlgCheckTree dlg)
        void HarvestCheckTreeItems(DlgCheckTree dlg)
        {
            foreach (var ctiCluster in dlg.Items)
            {
                var ci = ClusterList.FindClusterInfo(ctiCluster.Name);
                if (null == ci)
                    continue;

                foreach (var ctiProject in ctiCluster.SubItems)
                {
                    ci.SetUserCanAccess(ctiProject.Name, ctiProject.Checked);
                }
            }
        }
        #endregion

        #region OMethod: UITypeEditorEditStyle GetEditStyle(context)
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }
        #endregion
        #region OMethod: object EditValue(...)
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            // Set up the dialog
            var dlg = new DlgCheckTree();
            dlg.Label_Instruction = "Place a check beside the projects this user can access; or uncheck them all if they can access any project.";
            CreateCheckTreeItems(dlg);

            // Perform the dialog
            if (dlg.ShowDialog() == DialogResult.OK)
                HarvestCheckTreeItems(dlg);
            return ClusterList.UserCanAccessAllProjectsFriendly; ;
        }
        #endregion
    }

}
