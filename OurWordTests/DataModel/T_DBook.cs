/**********************************************************************************************
 * Project: OurWord! - Tests
 * File:    T_DBook.cs
 * Author:  John Wimbish
 * Created: 05 Mar 2008
 * Purpose: Tests the DBook class
 * Legal:   Copyright (c) 2004-08, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
#region Using
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

using NUnit.Framework;

using JWTools;
using JWdb;

using OurWord;
using OurWord.DataModel;
using OurWord.Dialogs;
using OurWord.View;
#endregion

namespace OurWordTests.DataModel
{
    [TestFixture] public class T_DBook
    {
        #region TEST: BookSortKeys
        [Test]  public void BookSortKeys()
        // Make certain we have a two-digit sort key, so that, e.g.,
        // Leviticus follows Exodus rather than Proverbs.
        {
            DBook book = new DBook("LEV", "");
            Assert.AreEqual("02", book.SortKey);
        }
        #endregion
    }

}
