#region ***** ShortTermMercurialEnvironment.cs *****
/**********************************************************************************************
 * Project: Our Word!
 * File:    TShortTermMercurialEnvironment.cs
 * Author:  John Wimbish
 * Created: 27 Oct 2009
 * Purpose: Tests for ShortTermMercurialEnvironment
 * Legal:   Copyright (c) 2005-10, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
using System;
using System.IO;
using NUnit.Framework;
using OurWordData.Synchronize;
#endregion

namespace OurWordTests.Utilities
{
    [TestFixture]
    public class TShortTermMercurialEnvironment
    {
        #region Test: TFolderContainingMercurial
        [Test]
        public void TFolderContainingMercurial()
            // Trivial test, just to alert me if I ever change the path
        {
            var sLocData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var sOurWordRoot = Path.Combine(sLocData, "OurWord");
            var sMercurualRoot = Path.Combine(sOurWordRoot, "mercurial");

            Assert.AreEqual( sMercurualRoot, ShortTermMercurialEnvironment.FolderContainingMercurial);
        }
        #endregion
        #region Test: TFullPathToMercurialExe
        [Test]
        public void TFullPathToMercurialExe()
            // Trivial test, to alert me if I ever change mercurial's filename hg.exe
        {
            var sPathToMercurial = Path.Combine(ShortTermMercurialEnvironment.FolderContainingMercurial, "hg.exe");
            sPathToMercurial = Repository.SurroundWithQuotes(sPathToMercurial);

            Assert.AreEqual(sPathToMercurial, ShortTermMercurialEnvironment.FullPathToMercurialExe);
        }
        #endregion
        #region Test: TPathIsSet
        [Test]
        public void TPathIsSet()
        {
            const string c_sPath = "PATH";

            var sPathPart = "OurWord" + Path.DirectorySeparatorChar + "mercurial";

            // Going in, should not be there
            var sPath = Environment.GetEnvironmentVariable(c_sPath);
            Assert.IsTrue(string.IsNullOrEmpty(sPath) || !sPath.Contains(sPathPart));

            using (new ShortTermMercurialEnvironment())
            {
                // Should be there now
                sPath = Environment.GetEnvironmentVariable(c_sPath);
                Assert.IsTrue(!string.IsNullOrEmpty(sPath) && sPath.Contains(sPathPart));
            }

            // Should be gone again
            sPath = Environment.GetEnvironmentVariable(c_sPath);
            Assert.IsTrue(string.IsNullOrEmpty(sPath) || !sPath.Contains(sPathPart));
        }
        #endregion
        #region Test: THgIsSet
        [Test]
        public void THgIsSet()
        {
            const string c_sHg = "HG";

            var sValuePart = "mercurial" + Path.DirectorySeparatorChar + "hg.exe";

            // Going in, should not be there
            var sValue = Environment.GetEnvironmentVariable(c_sHg);
            Assert.IsTrue(string.IsNullOrEmpty(sValue) || !sValue.Contains(sValuePart));

            using (new ShortTermMercurialEnvironment())
            {
                // Should be there now
                sValue = Environment.GetEnvironmentVariable(c_sHg);
                Assert.IsTrue(!string.IsNullOrEmpty(sValue) && sValue.Contains(sValuePart));
            }

            // Should be gone again
            sValue = Environment.GetEnvironmentVariable(c_sHg);
            Assert.IsTrue(string.IsNullOrEmpty(sValue) || !sValue.Contains(sValuePart));
        }
        #endregion
    }
}
