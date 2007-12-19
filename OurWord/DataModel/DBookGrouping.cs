/**********************************************************************************************
 * Project: Our Word!
 * File:    DBookGrouping.cs
 * Author:  John Wimbish
 * Created: 6 Aug 2007
 * Purpose: For navigation (GoTo Book menu), supports the user-definable ability to group
 *    books into nodes (e.g., Pentateuch, Historical, etc.) so that the menu does not become
 *    too large when the person is dealing with an entire Bible.
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
using System.Text;
using System.IO;

using JWTools;
using JWdb;

using OurWord.Dialogs;
using OurWord.View;

using NUnit.Framework;
#endregion

namespace OurWord.DataModel
{
    public class DBookGrouping : JObject
    {
        // BAttrs ----------------------------------------------------------------------------
        #region BAttr{g/s}: string LocKey - Place in Localizations.db with the UI string
        public string LocKey
        {
            get
            {
                return m_sLocKey;
            }
            set
            {
                SetValue(ref m_sLocKey, value);
            }
        }
        private string m_sLocKey;
        #endregion
        #region BAttr{g/s}: string DefaultEnglishText - English to display in GoTo menu
        public string DefaultEnglishText
        {
            get
            {
                return m_sDefaultEnglishText;
            }
            set
            {
                SetValue(ref m_sDefaultEnglishText, value);
            }
        }
        private string m_sDefaultEnglishText;
        #endregion
        #region BAttr{g}: BStringArray BookAbbrevs - The books which fall under this grouping
        public BStringArray BookAbbrevs
        {
            get
            {
                Debug.Assert(null != m_bsaBookAbbrevs);
                return m_bsaBookAbbrevs;
            }
        }
        public BStringArray m_bsaBookAbbrevs = null;
        #endregion
        #region Method: void DeclareAttrs()
        protected override void DeclareAttrs()
        {
            base.DeclareAttrs();
            DefineAttr("LocKey", ref m_sLocKey);
            DefineAttr("English", ref m_sDefaultEnglishText);
            DefineAttr("BookAbbrevs", ref m_bsaBookAbbrevs);
        }
        #endregion

        // Scaffolding -----------------------------------------------------------------------
        #region Constructor()
        public DBookGrouping()
            : base()
        {
            // Complex basic attrs
            m_bsaBookAbbrevs = new BStringArray();
        }
        #endregion
        #region Attr(g): string SortKey - overridden to enable JOWnSeq Find method support.
        public override string SortKey
        // In order to support sorting, the subclass must implement a SortKey attribute,
        // and this SortKey must return something other than an empty string. 
        {
            get
            {
                return LocKey;
            }
        }
        #endregion

        // Methods ---------------------------------------------------------------------------
        #region Method: int CountTargetBooksInThisGrouping()
        public int CountTargetBooksInThisGrouping()
        {
            int c = 0;
            foreach (DBook book in G.Project.Nav.PotentialTargetBooks)
            {
                if (-1 != BookAbbrevs.FindFirstPosition(book.BookAbbrev))
                    c++;
            }
            return c;
        }
        #endregion
        #region Method: bool IncludesBook(string sBookAbbrev)
        public bool IncludesBook(string sBookAbbrev)
        {
            if (-1 != BookAbbrevs.FindFirstPosition(sBookAbbrev))
                return true;
            return false;
        }
        #endregion
        #region Method: string GetUIText()
        public string GetUIText()
        {
            return G.GetLoc_BookGroupings( LocKey, DefaultEnglishText);
        }
        #endregion

        // Static Work Methods ---------------------------------------------------------------
        #region SMethod: void InitializeGroupings(JOwnSeq seq)
        static public void InitializeGroupings(JOwnSeq seq)
        {
            DBookGrouping bg = new DBookGrouping();
            bg.LocKey = "Pentateuch";
            bg.DefaultEnglishText = "Pentateuch";
            bg.BookAbbrevs.Read("5 {GEN} {EXO} {LEV} {NUM} {DEU}");
            seq.Append(bg);

            bg = new DBookGrouping();
            bg.LocKey = "Historical";
            bg.DefaultEnglishText = "Historical";
            bg.BookAbbrevs.Read("12 {JOS} {JDG} {RUT} {1SA} {2SA} {1KI} {2KI}" +
                "{1CH} {2CH} {EZR} {NEH} {EST}");
            seq.Append(bg);

            bg = new DBookGrouping();
            bg.LocKey = "Poetical";
            bg.DefaultEnglishText = "Poetical";
            bg.BookAbbrevs.Read("5 {JOB} {PSA} {PRO} {ECC} {SNG}");
            seq.Append(bg);

            bg = new DBookGrouping();
            bg.LocKey = "Prophetic";
            bg.DefaultEnglishText = "Prophetic";
            bg.BookAbbrevs.Read("17 {ISA} {JER} {LAM} {EZK} {DAN} {HOS} {JOL} {AMO}" +
                "{OBA} {JON} {MIC} {NAM} {HAB} {ZEP} {HAG} {ZEC} {MAL}" );
            seq.Append(bg);

            bg = new DBookGrouping();
            bg.LocKey = "Gospels";
            bg.DefaultEnglishText = "Gospels";
            bg.BookAbbrevs.Read("4 {MAT} {MRK} {LUK} {JHN}");
            seq.Append(bg);

            bg = new DBookGrouping();
            bg.LocKey = "PaulineLetters";
            bg.DefaultEnglishText = "Pauline Letters";
            bg.BookAbbrevs.Read("13 {ROM} {1CO} {2CO} {GAL} {EPH} {PHP} {COL} {1TH}" +
                "{2TH} {1TI} {2TI} {TIT} {PHM}");
            seq.Append(bg);

            bg = new DBookGrouping();
            bg.LocKey = "GeneralLetters";
            bg.DefaultEnglishText = "General Letters";
            bg.BookAbbrevs.Read("8 {HEB} {JAS} {1PE} {2PE} {1JN} {2JN} {3JN} {JUD}");
            seq.Append(bg);
        }
        #endregion

        const int cMinBooksRequiredToActivitateGroupings = 12;
        const int cMinBooksWithinGrouping = 2;

        #region SMethod: DBookGrouping FindGroupingFor(string sBookAbbrev)
        static public DBookGrouping FindGroupingFor(string sBookAbbrev)
        {
            foreach (DBookGrouping bg in G.TeamSettings.BookGroupings)
            {
                if (bg.IncludesBook(sBookAbbrev))
                    return bg;
            }
            return null;
        }
        #endregion

        #region SMethod: void PopulateGotoBookSubmenu(menuBook, btnBook, onClick)
        static public void PopulateGotoBookSubmenu(
            ToolStripMenuItem menuBook, 
            ToolStripDropDownButton btnBook,
            EventHandler onClick)
        {
            // Clear anything that is already there, so we can build from scratch
            menuBook.DropDownItems.Clear();
            btnBook.DropDownItems.Clear();

            // Loop through the books
            int cBooks = G.Project.Nav.PotentialTargetBooks.Length;
            foreach (DBook book in G.Project.Nav.PotentialTargetBooks)
            {
                // Default to placing the book as a dropdown within the miBook menu
                ToolStripMenuItem menuParent = menuBook;
                ToolStripDropDownItem btnParent = btnBook;

                // Optionally, we'll place the book into a sub-grouping?
                DBookGrouping bg = FindGroupingFor(book.BookAbbrev);
                if (cBooks > cMinBooksRequiredToActivitateGroupings &&
                    null != bg &&
                    bg.CountTargetBooksInThisGrouping() >= cMinBooksWithinGrouping)
                {
                    // If the Node already exists, then set the Parent to it
                    // and we're ready to append to it.
                    foreach (ToolStripMenuItem m in menuBook.DropDownItems)
                    {
                        if (m.Text == bg.GetUIText())
                        {
                            menuParent = menuBook;
                            btnParent = btnBook;
                            break;
                        }
                    }

                    // If the Node does not exist, then create it.
                    if (menuParent == menuBook)
                    {
                        ToolStripMenuItem menuGrouping = new ToolStripMenuItem(bg.GetUIText());
                        ToolStripMenuItem btnGrouping = new ToolStripMenuItem(bg.GetUIText());
                        menuGrouping.Name = "menu" + bg.DefaultEnglishText;
                        btnGrouping.Name = "btn" + bg.DefaultEnglishText;
                        menuBook.DropDownItems.Add(menuGrouping);
                        btnBook.DropDownItems.Add(btnGrouping);
                        menuParent = menuGrouping;
                        btnParent = btnGrouping;
                    }
                }

                // Create the menu item, showing the display name
                ToolStripMenuItem mi = new ToolStripMenuItem(book.DisplayName, null, onClick);
                ToolStripMenuItem btn = new ToolStripMenuItem(book.DisplayName, null, onClick);
                mi.Name = "menu" + book.DisplayName;
                btn.Name = "btn" + book.DisplayName;

                // For a locked book, write its text as Red color
                if (book.Locked)
                {
                    mi.ForeColor = Color.Red;
                    btn.ForeColor = Color.Red;
                }

                // Now we can add it to the menu.
                menuParent.DropDownItems.Add(mi);
                btnParent.DropDownItems.Add(btn);

                // Check it if it is the active book
                mi.Checked = (book.BookAbbrev == G.Project.Nav.BookAbbrev);
                btn.Checked = (book.BookAbbrev == G.Project.Nav.BookAbbrev);
            }
        }
        #endregion

    }
}
