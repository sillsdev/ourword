/**********************************************************************************************
 * Project: Our Word!
 * File:    BackupSystem.cs
 * Author:  John Wimbish
 * Created: 7 May 2009
 * Purpose: Test backup system
 * Legal:   Copyright (c) 2005-09, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
#region Using
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;

using NUnit.Framework;

using JWTools;
using JWdb;

using OurWord;
using JWdb.DataModel;
using OurWord.Dialogs;
using OurWord.Edit;
using OurWord.View;
#endregion

namespace OurWordTests.JWTools
{
    [TestFixture]
    public class T_BackupSystem
    {

        #region Method: void Setup()
        [SetUp] public void Setup()
        {
            JWU.NUnit_Setup();
        }
        #endregion

        #region Test: CleanUpOldFiles
        [Test] public void CleanUpOldFiles()
        {
			int cFilesToCreate = 400;

			// Create an empty working directory
            JWU.NUnit_RemoveTestFileFolder();
            string sTestPath = JWU.NUnit_TestFileFolder;

			// Give the system time to get the directory stuff done
			Thread.Sleep(1000);

			// Base name & extension
			string sBaseName = "TestFile";
			string sExtension = ".db";

			// Get today's date; we'll do it based on "today"
			DateTime today = DateTime.Today;
			int nYear  = today.Year;
			int nMonth = today.Month;
			int nDay   = today.Day;

			// Create a whole bunch of files
			DateTime dt = today;
			for(int i = 0; i < cFilesToCreate; i++)
			{
				string sDate = dt.ToString("yyyy-MM-dd");
				string sPath = sTestPath + Path.DirectorySeparatorChar + sBaseName + " " + sDate + sExtension;

				FileStream f = File.Create(sPath);
				f.Flush();
				f.Close();
				File.SetCreationTime(sPath, dt);

				--nDay;
				if (nDay < 1)
				{
					--nMonth;
					if (nMonth < 1)
					{
						nMonth += 12;
						--nYear;
					}
					nDay = DateTime.DaysInMonth(nYear, nMonth);
				}
				dt = new DateTime(nYear, nMonth, nDay);
			}

			// We should have a lot of files
			Assert.AreEqual( cFilesToCreate, Directory.GetFiles(sTestPath).Length );

			// Call the cleanup routine
			string sSourcePath = sTestPath + Path.DirectorySeparatorChar + 
                sBaseName + sExtension;
            BackupSystem.CleanUpOldFiles(sTestPath, sBaseName);

			// The number of files we now have depends on the day of week, day of 
			// month, etc. There should be 14-to-21 daily files, 6-to-11 weekly
			// files, and then 1 per month before that. Taking the minimum,
			// and assuming an initial count of 400, we expect there to be
			// at least 30 files. 
			//   Then assuming the maximums, and an initial count of 400, we would
			// expect there to be fewer than 50 files.
			int cFiles = Directory.GetFiles(sTestPath).Length;
			Assert.IsTrue(cFiles > 30);
			Assert.IsTrue(cFiles < 50);

			// Clean up my disk!
            JWU.NUnit_RemoveTestFileFolder();
        }
        #endregion
        #region Test: PruneDateFromFileName
        [Test] public void PruneDateFromFileName()
        {
            Assert.AreEqual("01 GEN - Amarasi - Draft-A", 
                BackupSystem.PruneDateFromFileName("01 GEN - Amarasi - Draft-A 2008-11-23.db"));

            Assert.AreEqual("02 EXO - Huichol - Draft-B",
                BackupSystem.PruneDateFromFileName("02 EXO - Huichol - Draft-B 2009-01-02.db"));
        }
        #endregion
    }
}
