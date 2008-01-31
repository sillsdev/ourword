/**********************************************************************************************
 * Project: Our Word!
 * File:    Dialogs\WizNewProject\WizNewProject.cs
 * Author:  John Wimbish
 * Created: 26 Jan 2008
 * Purpose: Wizard that manages creating a new project in OurWord
 * Legal:   Copyright (c) 2003-08, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
#region Using
using System;
using System.Collections;
using System.Collections.Generic;
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
using JWTools;
using JWdb;
using OurWord.DataModel;
#endregion

namespace OurWord.Dialogs.WizNewProject
{
    class WizNewProject : JW_Wizard
    {
        // Pages (in order of appearance) ----------------------------------------------------
        WizNew_Introduction m_pageIntroduction;
        WizNew_ProjectName m_pageProjectName;
        WizNew_TargetInfo m_pageTargetInfo;
        WizNew_FrontInfo m_pageFrontInfo;
        WizNew_TeamSettings m_pageTeamSettings;
        WizNew_Summary m_pageSummary;

        // Attrs -----------------------------------------------------------------------------
        #region VAttr{g}: string ProjectName
        public string ProjectName
        {
            get
            {
                if (null == m_pageProjectName)
                    return null;
                return m_pageProjectName.ProjectName;
            }
        }
        #endregion
        #region VAttr{g}: string TargetAbbreviation
        public string TargetAbbreviation
        {
            get
            {
                if (null == m_pageTargetInfo)
                    return null;
                return m_pageTargetInfo.Abbreviation;
            }
        }
                #endregion
        #region VAttr{g}: string TargetSettingsFolder
        public string TargetSettingsFolder
        {
            get
            {
                if (null == m_pageTargetInfo)
                    return null;
                return m_pageTargetInfo.SettingsFolder;
            }
        }
        #endregion

        #region VAttr{g}: string FrontName
        public string FrontName
        {
            get
            {
                if (null == m_pageFrontInfo)
                    return null;
                return m_pageFrontInfo.FrontName;
            }
        }
        #endregion
        #region VAttr{g}: string FrontAbbreviation
        public string FrontAbbreviation
        {
            get
            {
                if (null == m_pageFrontInfo)
                    return null;
                return m_pageFrontInfo.FrontAbbreviation;
            }
        }
        #endregion
        #region VAttr{g}: string FrontSettingsFolder
        public string FrontSettingsFolder
        {
            get
            {
                if (null == m_pageFrontInfo)
                    return null;
                return m_pageFrontInfo.FrontSettingsFolder;
            }
        }
        #endregion
        #region VAttr{g}: string ExistingFrontSettingsFilePath
        public string ExistingFrontSettingsFilePath
        {
            get
            {
                if (null == m_pageFrontInfo)
                    return null;
                return m_pageFrontInfo.ExistingFrontSettingsFilePath;
            }
        }
        #endregion

        #region VAttr{g}: WizNew_TeamSettings.Options TeamSettingsOption
        public WizNew_TeamSettings.Options TeamSettingsOption
        {
            get
            {
                if (null == m_pageTeamSettings)
                    return WizNew_TeamSettings.Options.kDefault;
                return m_pageTeamSettings.Option;
            }
        }
        #endregion
        #region VAttr{g}: string TeamSettingsPath
        public string TeamSettingsPath
        {
            get
            {
                if (null == m_pageTeamSettings)
                    return null;
                return m_pageTeamSettings.PathName;
            }
        }
        #endregion

        #region VAttr{g}: bool LaunchPropertiesDialogWhenDone
        public bool LaunchPropertiesDialogWhenDone
        {
            get
            {
                if (null == m_pageSummary)
                    return false;
                return m_pageSummary.LaunchPropertiesDialogWhenDone;
            }
        }
        #endregion

        // Scaffolding -----------------------------------------------------------------------
        public const string c_sEnglishTitle = "Create a New Project";
        #region Constructor()
        public WizNewProject()
            : base("WizNewProject", c_sEnglishTitle, JWU.GetBitmap("WizImportFile.gif"))
        {
            // Misc Wizard Dialog Settings
            NavigationColor = Color.Wheat;

            // Create and Add the wizard's pages
            m_pageIntroduction = new WizNew_Introduction();
            AddPage(m_pageIntroduction);

            m_pageProjectName = new WizNew_ProjectName();
            AddPage(m_pageProjectName);

            m_pageTargetInfo = new WizNew_TargetInfo();
            AddPage(m_pageTargetInfo);

            m_pageFrontInfo = new WizNew_FrontInfo();
            AddPage(m_pageFrontInfo);

            m_pageTeamSettings = new WizNew_TeamSettings();
            AddPage(m_pageTeamSettings);

            m_pageSummary = new WizNew_Summary();
            AddPage(m_pageSummary);
        }
        #endregion
        #region OMethod: void Localization()
        protected override void Localization()
        {
            Control[] vExclude = new Control[0];

            LocDB.Localize(this, vExclude);
        }
        #endregion
    }
}
