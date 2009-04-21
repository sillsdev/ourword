/**********************************************************************************************
 * Project: Our Word!
 * File:    Dialogs\WizImportBook\WizCreateBook.cs
 * Author:  John Wimbish
 * Created: 28 Jun 2008
 * Purpose: Wizard that manages creating a blank book in OurWord
 * Legal:   Copyright (c) 2003-09, John S. Wimbish. All Rights Reserved.  
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
using JWdb.DataModel;
using OurWord.Dialogs.WizImportBook;
#endregion

namespace OurWord.Dialogs.WizCreateBook
{
    class WizCreateBook : JW_Wizard
    {
        // Individual Pages in the Wizard, in order of appearance ----------------------------
        WizPage_CreateBookIntroduction m_pageIntroduction;
        WizPage_GetAbbreviation m_pageGetAbbreviation;
        WizPage_GetDestinationFolder m_pageDestinationFolder;

        // Attrs -----------------------------------------------------------------------------
        #region Attr{g}: DBook FrontBook
        public DBook FrontBook
        {
            get
            {
                string sAbbrev = m_pageGetAbbreviation.SelectedBookAbbrev;
                return DB.FrontTranslation.FindBook(sAbbrev);
            }
        }
        #endregion
        #region Attr{g}: DTranslation Translation
        public DTranslation Translation
        {
            get
            {
                Debug.Assert(null != m_Translation);
                return m_Translation;
            }
        }
        DTranslation m_Translation = null;
        #endregion

        // User choices ----------------------------------------------------------------------
        #region VAttr{g}: string NewBookAbbrev
        public string NewBookAbbrev
        {
            get
            {
                return m_pageGetAbbreviation.SelectedBookAbbrev;
            }
        }
        #endregion
        #region VAttr{g}: string NewBookPath
        public string NewBookPath
        {
            get
            {
                return m_pageDestinationFolder.BookFolder;
            }
        }
        #endregion

        // Scaffolding -----------------------------------------------------------------------
        public const string c_sEnglishTitle = "Create an empty Scripture book ready for Drafting";
        #region Constructor(DTranslation)
        public WizCreateBook(DTranslation _translation)
            : base("WizCreateBook", c_sEnglishTitle, JWU.GetBitmap("WizImportFile.gif"))
        {
            // The translation for which we are importing a book
            m_Translation = _translation;

            // Misc Wizard Dialog Settings
            NavigationColor = Color.Wheat;

            // Add the wizard's pages
            m_pageIntroduction = new WizPage_CreateBookIntroduction();
            AddPage(m_pageIntroduction);

            string sWhichBook = LocDB.GetValue(this, "idWhichCreate",
                "Which book do you wish to use to create an empty shell for drafting?");
            m_pageGetAbbreviation = new WizPage_GetAbbreviation(Translation, sWhichBook);
            AddPage(m_pageGetAbbreviation);

            m_pageDestinationFolder = new WizPage_GetDestinationFolder();
            AddPage(m_pageDestinationFolder);
        }
        #endregion
        #region OMethod: void Localization()
        protected override void Localization()
        {
            // Create a master vector of exclusions of the subpages
            ArrayList a = new ArrayList();
            a.AddRange(m_pageGetAbbreviation.vExclude);
            a.AddRange(m_pageDestinationFolder.vExclude);
            Control[] vExclude = new Control[a.Count];
            for (int i = 0; i < a.Count; i++)
                vExclude[i] = a[i] as Control;

            // Localize the form and all of its pages
            LocDB.Localize(this, vExclude);
        }
        #endregion

    }
}
