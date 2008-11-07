/**********************************************************************************************
 * Project: Our Word!
 * File:    DTranslatorNote.cs
 * Author:  John Wimbish
 * Created: 04 Nov 2008
 * Purpose: Handles a translator's (or consultant's) note in Scripture. 
 * Legal:   Copyright (c) 2005-08, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
#region Using
using System;
using System.Diagnostics;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.IO;

using JWTools;
using JWdb;
#endregion


namespace OurWord.DataModel
{
    class DTranslatorNote : JObject
    {
        // ZAttrs ----------------------------------------------------------------------------
        #region BAttr{g/s}: string Category
        public string Category
        {
            get
            {
                return m_sCategory;
            }
            set
            {
                SetValue(ref m_sCategory, value);
            }
        }
        private string m_sCategory = "";
        #endregion
        #region BAttr{g/s}: bool IsClosed
        public bool IsClosed
        {
            get
            {
                return m_bIsClosed;
            }
            set
            {
                SetValue(ref m_bIsClosed, value);
            }
        }
        private bool m_bIsClosed = false;
        #endregion

        #region Method: void DeclareAttrs()
        protected override void DeclareAttrs()
        {
            base.DeclareAttrs();

            DefineAttr("Category", ref m_sCategory);
            DefineAttr("Closed", ref m_bIsClosed);
        }
        #endregion



        #region Constructor()
        public DTranslatorNote()
            : base()
        {
        }
        #endregion
    }
}
