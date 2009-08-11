/**********************************************************************************************
 * Project: OurWord! - Tests
 * File:    T_Utilities.cs
 * Author:  John Wimbish
 * Created: 10 Jul 2008
 * Purpose: Tests the JWU class
 * Legal:   Copyright (c) 2004-09, John S. Wimbish. All Rights Reserved.  
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

using OurWord;
using JWdb.DataModel;
using OurWord.Dialogs;
using OurWord.Edit;
using OurWord.Layouts;
#endregion

namespace OurWordTests.JWTools
{
    [TestFixture] public class T_Utilities
    {
        #region Test: PathEllipses
        [Test] public void PathEllipses()
        {
            string sIn = "C:\\Documents and Settings\\John\\My Documents\\Projects\\MyFile.db";
            string sExpected = PathConverter.ConvertDirectorySeparators("C:\\Documents and Se...\\MyFile.db");
            string sOut = JWU.PathEllipses(sIn, 30);

            Assert.AreEqual(sExpected, sOut);
        }
        #endregion

        #region Test: ArrayListToIntArray
        [Test] public void ArrayListToIntArray()
        {
            // An array of starting data
            int[] vExpected = new int[9];
            vExpected[0] = 10;
            vExpected[1] = 12;
            vExpected[2] = 33;
            vExpected[3] = 52;
            vExpected[4] = 33;
            vExpected[5] = 1442;
            vExpected[6] = 152;
            vExpected[7] = 3;
            vExpected[8] = 15;

            // Place it into the array list
            ArrayList aIn = new ArrayList();
            foreach (int n in vExpected)
                aIn.Add(n);

            // Do the conversion
            int[] vActual = JWU.ArrayListToIntArray(aIn);

            // Does the output match the original?
            Assert.AreEqual(vExpected.Length, vActual.Length);
            for (int i = 0; i < vExpected.Length; i++)
                Assert.AreEqual(vExpected[i], vActual[i]);
        }
        #endregion
        #region Test: ArrayListToStringArray
        [Test] public void ArrayListToStringArray()
        {
            // An array of starting data
            string[] vExpected = new string[9];
            vExpected[0] = "Christiane";
            vExpected[1] = "Robert";
            vExpected[2] = "David";
            vExpected[3] = "Emily";
            vExpected[4] = "Robert";
            vExpected[5] = "John";
            vExpected[6] = "David";
            vExpected[7] = "Sandra";
            vExpected[8] = "Robert";

            // Place it into the array list
            ArrayList aIn = new ArrayList();
            foreach (string s in vExpected)
                aIn.Add(s);

            // Do the conversion
            string[] vActual = JWU.ArrayListToStringArray(aIn);

            // Does the output match the original?
            Assert.AreEqual(vExpected.Length, vActual.Length);
            for (int i = 0; i < vExpected.Length; i++)
                Assert.AreEqual(vExpected[i], vActual[i]);
        }
        #endregion

        #region Test: IsParatextFile
        [Test] public void IsParatextFile()
        {
            Assert.IsTrue(JWU.IsParatextScriptureFile("43LUKNIV84.PTX"));

            Assert.IsTrue(JWU.IsParatextScriptureFile("43LUKNIV84.PTW"));

            Assert.IsTrue(JWU.IsParatextScriptureFile(
                "C:\\Documents and Settings\\John\\My Documents\\Projects\\JWTools\\" +
                "TeraLuk.PTW"));

            Assert.IsFalse(JWU.IsParatextScriptureFile("Kup-MRK-Final-Mu.db"));

            Assert.IsFalse(JWU.IsParatextScriptureFile(
                "C:\\CCC-Lg\\Kupang\\02-Mrk\\Kup-MRK-Final-Mu.db"));
        }
        #endregion

		#region Test: RightmostSubfolder
		[Test] public void RightmostSubfolder()
		{
			string sTarget = "My Folder";

			string sPath = "C:" + Path.DirectorySeparatorChar +
				"Documents and Settings" + Path.DirectorySeparatorChar +
				"AllUsers" + Path.DirectorySeparatorChar +
				"MyStuff" + Path.DirectorySeparatorChar +
				sTarget;

			// Try it once without a trailing separator character
			string sFolder = JWU.ExtractRightmostSubFolder(sPath);
			Assert.AreEqual(sTarget, sFolder);

			// Try again, this time with a trailing separator char
			sPath += Path.DirectorySeparatorChar;
			sFolder = JWU.ExtractRightmostSubFolder(sPath);
			Assert.AreEqual(sTarget, sFolder);
		}
		#endregion
	}
}
