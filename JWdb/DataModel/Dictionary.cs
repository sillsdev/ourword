/**********************************************************************************************
 * Project: Our Word!
 * File:    Dictionary.cs
 * Author:  John Wimbish
 * Created: 11 mar 2009
 * Purpose: Encapsulates the WeSay dictionary 
 * Legal:   Copyright (c) 2005-09, John S. Wimbish. All Rights Reserved.  
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
using JWTools;
using JWdb;
using Palaso.Services.Dictionary;
#endregion

namespace JWdb.DataModel
{
    // en, zna

    public class Dictionary
    {
        // Represents a word in the dictionary -----------------------------------------------
        #region CLASS: Item
        public class Item
        {
            #region Attr{g}: string ID
            public string ID
            {
                get
                {
                    return m_sID;
                }
            }
            string m_sID;
            #endregion
            #region Attr{g} string Form
            public string Form
            {
                get
                {
                    return m_sForm;
                }
            }
            string m_sForm;
            #endregion

            #region Constructor(sID, sForm)
            public Item(string sID, string sForm)
            {
                m_sID = sID;
                m_sForm = sForm;
            }
            #endregion
        }
        #endregion

        // Attrs -----------------------------------------------------------------------------
        #region Attr{g}: DictionaryAccessor DictionaryAccessor
        DictionaryAccessor DictionaryAccessor
        {
            get
            {
                return m_DictionaryAccessor;
            }
        }
        DictionaryAccessor m_DictionaryAccessor = null;
        #endregion
        #region Attr{g}: DProject Project
        DProject Project
        {
            get
            {
                Debug.Assert(null != m_Project);
                return m_Project;
            }
        }
        DProject m_Project;
        #endregion
        #region Attr{g}: string WSVernacular
        string WSVernacular
        {
            get
            {
                Debug.Assert(!string.IsNullOrEmpty(m_sVernacular));
                return m_sVernacular;
            }
        }
        string m_sVernacular;
        #endregion
        #region Attr{g}: string WSAnalysis
        string WSAnalysis
        {
            get
            {
                Debug.Assert(!string.IsNullOrEmpty(m_sAnalysis));
                return m_sAnalysis;
            }
        }
        string m_sAnalysis;
        #endregion

        // Scaffolding -----------------------------------------------------------------------
        #region Constructor(DProject)
        public Dictionary(DProject _project)
        {
            m_Project = _project;
            Initialize();

            // Kuldge for the time being; we need to get this from the WS object
            m_sVernacular = "v";
            m_sAnalysis = "en";
        }
        #endregion
        #region Destructor()
        ~Dictionary()
        {
            Dispose();
        }
        #endregion
        #region Method: void Dispose()
        public void Dispose()
        {
            if (null != m_DictionaryAccessor)
            {
                m_DictionaryAccessor.Dispose();
                m_DictionaryAccessor = null;
            }
        }
        #endregion
        #region Method: void Initialize()
        /// <summary>
        /// Reinitialize the Accessor; typically call when the Project's path names have changed.
        /// </summary>
        public void Initialize()
        {
            if (null != m_DictionaryAccessor)
                m_DictionaryAccessor.Dispose();

            m_DictionaryAccessor = null;

            if (string.IsNullOrEmpty(Project.PathToDictionaryData) ||
                !File.Exists(Project.PathToDictionaryData))
                return;

            if (string.IsNullOrEmpty(Project.PathToDictionaryApp) ||
                !File.Exists(Project.PathToDictionaryApp))
                return;

            m_DictionaryAccessor = new DictionaryAccessor(
                Project.PathToDictionaryData,
                Project.PathToDictionaryApp);
        }
        #endregion

        // WeSay Lookup ----------------------------------------------------------------------
        #region Method: Item[] GetMatchingEntries(sForm, FindMethods method)
        public Item[] GetMatchingEntries(string sForm, FindMethods method)
        {
            string[] vForms;
            string[] vIds;

            Cursor.Current = Cursors.WaitCursor;

            try
            {
                DictionaryAccessor.GetMatchingEntries(
                    WSVernacular,
                    sForm,
                    method,
                    out vIds,
                    out vForms);
            }
            catch (Exception)
            {
                Cursor.Current = Cursors.Default;
                return new Item[0];
            }

            Item[] vItems = new Item[vForms.Length];
            for (int i = 0; i < vItems.Length; i++)
                vItems[i] = new Item(vIds[i], vForms[i]);

            Cursor.Current = Cursors.Default;

            return vItems;
        }
        #endregion
        #region Method: string GetHtmlForEntry(string sID)
        public string GetHtmlForEntry(string sID)
        {
            Cursor.Current = Cursors.WaitCursor;
            string sHtml = "";

            try
            {
                sHtml = DictionaryAccessor.GetHtmlForEntries(new string[] { sID });
            }
            catch (Exception)
            {
                Cursor.Current = Cursors.Default;
                return sHtml;
            }

            Cursor.Current = Cursors.Default;
            return sHtml;
        }
        #endregion
        #region Method: string AddEntry(sLexemeForm, sDefinition, sExample)
        public string AddEntry(string sLexemeForm, string sDefinition, string sExample)
        {
            Cursor.Current = Cursors.WaitCursor;
            string sID = "";

            try
            {
                sID = DictionaryAccessor.AddEntry(
                    WSVernacular, sLexemeForm,
                    WSAnalysis, sDefinition,
                    WSVernacular, sExample);
            }
            catch (Exception)
            {
            }

            Cursor.Current = Cursors.Default;
            return sID;
        }
        #endregion
        #region Method: void JumpToEntry(string sID)
        public void JumpToEntry(string sID)
        {
            Cursor.Current = Cursors.WaitCursor;
            try
            {
                DictionaryAccessor.JumpToEntry(sID);
            }
            catch (Exception)
            {
            }
            Cursor.Current = Cursors.Default;
        }
        #endregion
    }
}
