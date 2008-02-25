/**********************************************************************************************
 * Project: OurWord!
 * File:    DMergeBook.cs
 * Author:  John Wimbish
 * Created: 04 Feb 2008
 * Purpose: A subclass of DBook specifically for merging into a DBook
 * Legal:   Copyright (c) 2004-08, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
#region Using
using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Timers;
using System.Windows.Forms;
using JWTools;
using JWdb;
using OurWord.DataModel;
using OurWord.View;
using OurWord.Edit;
using NUnit.Framework;
#endregion

namespace OurWord.Edit
{
    public class DMergeBook : DBook
    {
        // Attrs -----------------------------------------------------------------------------
        #region Attr{g/s}: string NickName
        public string NickName
        {
            get
            {
                return m_sNickName;
            }
            set
            {
                m_sNickName = value;
            }
        }
        string m_sNickName;
        #endregion
        #region Attr{g}: DBook MasterBook
        DBook MasterBook
        {
            get
            {
                Debug.Assert(null != m_MasterBook);
                return m_MasterBook;
            }
        }
        DBook m_MasterBook;
        #endregion

        // Overrides of DBook ----------------------------------------------------------------
        #region VAttr{g}: DTranslation Translation - points to MasterBook's Translation
        public override DTranslation Translation
        {
            get
            {
                return MasterBook.Translation;
            }
        }
        #endregion

        // Scaffolding -----------------------------------------------------------------------
        #region Constructor(DBook MasterBook, string sAbsolutePath, sNickName)
        public DMergeBook(DBook _MasterBook, string sPath, string _sNickName)
            : base(_MasterBook.BookAbbrev, sPath)
        {
            m_MasterBook = _MasterBook;
            m_sNickName = _sNickName;
        }
        #endregion


    }
}
