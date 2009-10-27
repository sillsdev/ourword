/**********************************************************************************************
 * Project: Our Word!
 * File:    Dialogs\WizImportBook\WizImportBook.cs
 * Author:  John Wimbish
 * Created: 1 Feb 2007
 * Purpose: Wizard that manages importing a book into OurWord
 * Legal:   Copyright (c) 2003-09, John S. Wimbish. All Rights Reserved.  
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
using JWTools;
using OurWordData;
using OurWordData.DataModel;
#endregion

namespace OurWord.Dialogs.WizImportBook
{
    class WizImportBook : JW_Wizard
    {
        // Individual Pages in the Wizard, in order of appearance ----------------------------
        WizPage_GetFileName m_pageGetFileName;
        WizPage_Summary m_pageSummary;

        // Attrs -----------------------------------------------------------------------------
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
        #region Attr{g}: string ImportFileName
        public string ImportFileName
        {
            get
            {
                return m_pageGetFileName.PathName;
            }
        }
        #endregion
        #region Attr{g}: string Format
        public string Format
        {
            get
            {
                return m_pageGetFileName.Format;
            }
        }
        #endregion
        #region Attr{g}: string BookAbbrev
        public string BookAbbrev
        {
            get
            {
				return m_sBookAbbrev;
            }
			set
			{
				m_sBookAbbrev = value;
			}
        }
		string m_sBookAbbrev;
        #endregion
		#region Attr{g}: string BookName
		public string BookName
        {
            get
            {
				return m_sBookName;
            }
			set
			{
				m_sBookName = value;
			}
        }
		string m_sBookName;
        #endregion

        // Scaffolding -----------------------------------------------------------------------
        public const string c_sEnglishTitle = "Import a Scripture Book";
        #region Constructor()
        public WizImportBook(DTranslation _translation)
            : base("WizImportBook", c_sEnglishTitle, JWU.GetBitmap("WizImportFile.gif"))
        {
            // The translation for which we are importing a book
            m_Translation = _translation;

            // Misc Wizard Dialog Settings
            NavigationColor = Color.Wheat;

            // Add the wizard's pages
            m_pageGetFileName = new WizPage_GetFileName();
            AddPage(m_pageGetFileName);

            m_pageSummary = new WizPage_Summary();
            AddPage(m_pageSummary);
        }
        #endregion
        #region OMethod: void Localization()
        protected override void Localization()
        {
            // Create a master vector of exclusions of the subpages
            ArrayList a = new ArrayList();
            a.AddRange(m_pageSummary.vExclude);
            Control[] vExclude = new Control[a.Count];
            for (int i = 0; i < a.Count; i++)
                vExclude[i] = a[i] as Control;

            // Localize the form and all of its pages
            LocDB.Localize(this, vExclude);
        }
        #endregion

    }
}
