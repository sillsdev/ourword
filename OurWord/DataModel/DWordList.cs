/**********************************************************************************************
 * Project: Our Word!
 * File:    WordList.cs
 * Author:  John Wimbish
 * Created: 23 Sep 2008
 * Purpose: The wordlist and its related classes
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
using System.Windows.Forms;
using System.IO;
using System.Text;

using JWTools;
using JWdb;
#endregion

namespace OurWord.DataModel
{
    class WordList : JObjectOnDemand
    {
        // ZAttrs ----------------------------------------------------------------------------
        #region JAttr{g}: JOwnSeq Words - the list of individual Words
        public JOwnSeq<WSingleWord> Words
        {
            get
            {
                return m_osWords;
            }
        }
        private JOwnSeq<WSingleWord> m_osWords;
        #endregion
        #region JAttr{g}: JOwnSeq Phrases - the list of miltiword phrases
        public JOwnSeq<WMultiWord> Phrases
        {
            get
            {
                return m_osPhrases;
            }
        }
        private JOwnSeq<WMultiWord> m_osPhrases;
        #endregion

        // Scaffolding -----------------------------------------------------------------------
        #region Constructor()
        public WordList()
            : base()
        {
            m_osWords = new JOwnSeq<WSingleWord>("Words", this, true, true);
            m_osPhrases = new JOwnSeq<WMultiWord>("Phrases", this, true, true);
        }
        #endregion
    }

    public class WItem : JObject
    {
        // ZAttrs ----------------------------------------------------------------------------
        #region BAttr{g/s}: string Form - the wordform (regularized, e.g., no capitals)
        public string Form
        {
            get
            {
                return m_sForm;
            }
            set
            {
                SetValue(ref m_sForm, value);
            }
        }
        private string m_sForm = "";
        #endregion
        #region BAttr{g/s}: string DictionaryID - ID of this form in, e.g., WeSay
        public string DictionaryID
        {
            get
            {
                return m_sDictionaryID;
            }
            set
            {
                SetValue(ref m_sDictionaryID, value);
            }
        }
        private string m_sDictionaryID = "";
        #endregion
        #region BAttr{g}: BStringArray Glosses - The possible glosses for this form
        public BStringArray Glosses
        {
            get
            {
                Debug.Assert(null != m_bsaGlosses);
                return m_bsaGlosses;
            }
        }
        public BStringArray m_bsaGlosses = null;
        #endregion
        #region Method void DeclareAttrs()
        protected override void DeclareAttrs()
        {
            base.DeclareAttrs();
            DefineAttr("Form", ref m_sForm);
            DefineAttr("Dict", ref m_sDictionaryID);
            DefineAttr("Glosses", ref m_bsaGlosses);
        }
        #endregion

        // Scaffolding -----------------------------------------------------------------------
        #region Constructor()
        protected WItem()
            : base()
        {
        }
        #endregion
        #region Attr(g): string SortKey - overridden to enable JOWnSeq Find method support.
        public override string SortKey
        // In order to support sorting, the subclass must implement a SortKey attribute,
        // and this SortKey must return something other than an empty string. 
        {
            get
            {
                return Form;
            }
        }
        #endregion
        #region Method: override bool ContentEquals(obj) - required override to prevent duplicates
        public override bool ContentEquals(JObject obj)
        {
            if (this.GetType() != obj.GetType())
                return false;

            WItem wi = obj as WItem;
            Debug.Assert(null != wi);

            if (Form != wi.Form)
                return false;
            if (DictionaryID != wi.DictionaryID)
                return false;
            if (!Glosses.IsSameAs(wi.Glosses))
                return false;

            return true;
        }
        #endregion
    }

    public class WSingleWord : WItem
    {

        // Scaffolding -----------------------------------------------------------------------
        #region Constructor()
        public WSingleWord()
            : base()
        {
        }
        #endregion
    }

    public class WMultiWord : WItem
    {
        // ZAttrs ----------------------------------------------------------------------------
        #region JAttr{g}: JOwnSeq Words - the Words which make up this multiword phrase
        public JOwnSeq<WSingleWord> Words
        {
            get
            {
                return m_osWords;
            }
        }
        private JOwnSeq<WSingleWord> m_osWords;
        #endregion

        // Scaffolding -----------------------------------------------------------------------
        #region Constructor()
        public WMultiWord()
            : base()
        {
            m_osWords = new JOwnSeq<WSingleWord>("Words", this, false, false);
        }
        #endregion
        #region Method: override bool ContentEquals(obj) - required override to prevent duplicates
        public override bool ContentEquals(JObject obj)
        {
            if (!base.ContentEquals(obj))
                return false;

            return true;
        }
        #endregion
    }



}
