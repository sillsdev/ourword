/**********************************************************************************************
 * Project: OurWord! - Tests
 * File:    T_JParagraph.cs
 * Author:  John Wimbish
 * Created: 5 Nov 2008
 * Purpose: Tests the T_JParagraph implementation
 * Legal:   Copyright (c) 2004-08, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
#region Using
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

using NUnit.Framework;

using JWTools;
using JWdb;
#endregion

namespace OurWordTests.JWdb
{
    [TestFixture] public class T_JParagraph
    {
        #region Setup
        [SetUp] public void Setup()
        {
            // Unit Test Setup
            JWU.NUnit_Setup();
        }
        #endregion

        #region Test: IO
        [Test] public void IO()
        {
            // Create a paragraph, put some data into it
            JWritingSystem jws = new JWritingSystem("English");
            JParagraph pgOut = new JParagraph(jws);
            pgOut.Contents = "Once upon a time there was a horse named Bob.";

            // Write it out
            string sPath = JWU.NUnit_TestFilePathName;
            TextWriter tw = JUtil.GetTextWriter(sPath);
            pgOut.Write(tw, 0);
            tw.Close();

            // Read it into a new paragraph
            TextReader tr = JUtil.GetTextReader(sPath);
            string sIn = tr.ReadLine();
            JParagraph pgIn = new JParagraph(sIn, tr);
            tr.Close();

            // Compare the contents; should be the same
            Assert.AreEqual(pgIn.Contents, pgOut.Contents);
        }
        #endregion

    }
}
