/**********************************************************************************************
 * Project: Our Word!
 * File:    DBookGrouping.cs
 * Author:  John Wimbish
 * Created: 6 Aug 2007
 * Purpose: For navigation (GoTo Book menu), supports the user-definable ability to group
 *    books into nodes (e.g., Pentateuch, Historical, etc.) so that the menu does not become
 *    too large when the person is dealing with an entire Bible.
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
using System.Text;
using System.IO;

using JWTools;
using JWdb;
#endregion

namespace JWdb.DataModel
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
            foreach (DBook book in DB.Project.Nav.PotentialTargetBooks)
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
            return Loc.GetBookGroupings( LocKey, DefaultEnglishText);
        }
        #endregion

        // Static Work Methods ---------------------------------------------------------------
        #region SMethod: void InitializeGroupings(JOwnSeq seq)
        static public void InitializeGroupings(JOwnSeq<DBookGrouping> seq)
        {
            seq.Clear();

            DBookGrouping bg = new DBookGrouping();
            bg.LocKey = "Pentateuch";
            bg.DefaultEnglishText = "Pentateuch";
            bg.BookAbbrevs.Read("5 {GEN} {EXO} {LEV} {NUM} {DEU}");
            seq.Append(bg);

            bg = new DBookGrouping();
            bg.LocKey = "Historical";
            bg.DefaultEnglishText = "Historical";
            bg.BookAbbrevs.Read("12 {JOS} {JDG} {RUT} {1SA} {2SA} {1KI} {2KI} " +
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
            bg.BookAbbrevs.Read("17 {ISA} {JER} {LAM} {EZK} {DAN} {HOS} {JOL} {AMO} " +
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
            bg.BookAbbrevs.Read("13 {ROM} {1CO} {2CO} {GAL} {EPH} {PHP} {COL} {1TH} " +
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
            foreach (DBookGrouping bg in DB.TeamSettings.BookGroupings)
            {
                if (bg.IncludesBook(sBookAbbrev))
                    return bg;
            }
            return null;
        }
        #endregion

        #region SMethod: void PopulateGotoBook(ToolStripDropDownItem, onClick)
        /// <summary>
        /// Creates the subitems for the GoTo menu or button
        /// </summary>
        /// <param name="itemGoTo">The GoTo Book item, which may either be a menu item, or a drop-down button,</param>
        /// <param name="onClick">The event handler to call for any Book menu items that will be inserted.</param>
        static public void PopulateGotoBook(ToolStripDropDownItem itemGoToBook, EventHandler onClick)
        {
            // Clear anything that is already there, so we can build from scratch
            itemGoToBook.DropDownItems.Clear();

            // How many books do we have? (If too few, we will not want to nest with subitems)
            int cBooks = DB.Project.Nav.PotentialTargetBooks.Length;

            // Loop through the books, adding them to the appropriate place in the menu
            foreach (DBook book in DB.Project.Nav.PotentialTargetBooks)
            {
                // Default to placing the book as a dropdown within the top-level GoToBook item
                ToolStripDropDownItem itemParent = itemGoToBook;

                // Find the sub-grouping (if any) to which this book belongs
                DBookGrouping group = FindGroupingFor(book.BookAbbrev);

                // Optionally, we'll place the book into a sub-grouping. Conditions are:
                // 1. There are enough books in the project to justify subgroupings,
                // 2. A DBookGrouping was found for this book, and
                // 3. There are enough books in this grouping to justify using it.
                if (cBooks > cMinBooksRequiredToActivitateGroupings &&
                    null != group &&
                    group.CountTargetBooksInThisGrouping() >= cMinBooksWithinGrouping)
                {
                    // If the Node for this group already exists, then set the Parent to it
                    // and we're ready to append to it.
                    foreach (ToolStripMenuItem iGrouping in itemGoToBook.DropDownItems)
                    {
                        if (iGrouping.Text == group.GetUIText())
                        {
                            itemParent = iGrouping;
                            break;
                        }
                    }

                    // If the Node does not exist, then create it.
                    if (itemParent == itemGoToBook)
                    {
                        ToolStripMenuItem itemGrouping = new ToolStripMenuItem(group.GetUIText());
                        itemGrouping.Name = "menu" + group.DefaultEnglishText;
                        itemGoToBook.DropDownItems.Add(itemGrouping);
                        itemParent = itemGrouping;
                    }
                }

                // Create the menu item, showing the display name
                ToolStripMenuItem itemBook = new ToolStripMenuItem(book.DisplayName, null, onClick);
                itemBook.Name = "menu" + book.BookAbbrev;
                itemBook.Tag = book.BookAbbrev;

                // For a locked book, write its text as Red color
                if (book.Locked)
                    itemBook.ForeColor = Color.Red;

                // Now we can add it to the menu hierarchy.
                itemParent.DropDownItems.Add(itemBook);

                // Check it if it is the active book
                itemBook.Checked = (book.BookAbbrev == DB.Project.Nav.BookAbbrev);

            } // endloop
        }
        #endregion
    }
}
