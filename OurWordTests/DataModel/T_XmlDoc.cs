#region ***** T_XmlDoc.cs *****
/**********************************************************************************************
 * Project: OurWord! - Tests
 * File:    T_XmlDoc.cs
 * Author:  John Wimbish
 * Created: 15 Aug 2009
 * Purpose: Tests the XmlDoc class
 * Legal:   Copyright (c) 2004-09, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
#region Using
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;

using NUnit.Framework;

using JWTools;
using JWdb;

using OurWord;
using JWdb.DataModel;
using OurWord.Dialogs;
using OurWord.Layouts;
#endregion
#endregion

namespace OurWordTests.DataModel
{
    [TestFixture]
    public class T_XmlDoc : TestCommon
    {
        #region Setup
        [SetUp]
        public void Setup()
        {
            JWU.NUnit_Setup();
        }
        #endregion

        #region Test: IdConversionsBase10
        [Test] public void IdConversionsBase10()
        {
            var vN = new int[] { 
                0, 1, 9, 
                10, 19, 20, 10*10-1, 
                10*10, 10*10+1, 10*10*10-1,
                10*10*10
            };
            var vS = new string[] { 
                "0" , "1", "9", 
                "10", "19", "20", "99", 
                "100", "101", "999",
                "1000"
            };

            XmlDoc.Digits = "0123456789";

            for (int i = 0; i < vN.Length; i++)
            {
                // Convert the integer to the string ID
                string s = XmlDoc.IntToID(vN[i]);
                Assert.AreEqual(vS[i], s,
                    "N=" + vN[i].ToString() + " --should convert to-- S=\"" + vS[i] + "\"");

                // Convert the resultant string ID back to the original integer
                int n = XmlDoc.IdToInt(vS[i]);
                Assert.AreEqual(vN[i], n,
                    "S=\"" + vS[i] + "\" --should convert to-- N=" + vN[i].ToString());
            }
        }
        #endregion
        #region Test: IdConversions
        [Test] public void IdConversions()
            // Leave XmlDoc.Digits at the default. Changing this would have the effect
            // of messing with existing oxes data out there.
        {
            // Corresponding integers and string IDs
            var vN = new int[] { 
                0, 1, 41, 
                42, 83, 84, 42*42-1, 
                42*42, 42*42+1, 42*42*42-1,
                42*42*42
            };
            var vS = new string[] { 
                "B" , "C", "z", 
                "CB", "Cz", "DB", "zz", 
                "CBB", "CBC", "zzz",
                "CBBB"
            };

            for (var i = 0; i < vN.Length; i++)
            {
                // Convert the integer to the string ID
                var s = XmlDoc.IntToID( vN[i] );
                Assert.AreEqual(vS[i], s, 
                    "N=" + vN[i].ToString() + " --should convert to-- S=\"" +vS[i] + "\"");

                // Convert the resultant string ID back to the original integer
                var n = XmlDoc.IdToInt(vS[i]);
                Assert.AreEqual(vN[i], n, 
                    "S=\"" + vS[i] + "\" --should convert to-- N=" + vN[i].ToString());
            }
        }
        #endregion

        #region Test: UrlAttrList_Parsing1
        [Test] public void UrlAttrList_Parsing1()
        {
            string sUrl = "oxes://book=Mrk+Chapter=3+Verse=2+selectedWord=carakiau";

            var v = new UrlAttrList("oxes://", sUrl);

            Assert.AreEqual(4, v.Count);

            Assert.AreEqual("book", v[0].Name);
            Assert.AreEqual("Mrk",  v[0].Value);

            Assert.AreEqual("Chapter", v[1].Name);
            Assert.AreEqual("3", v[1].Value);

            Assert.AreEqual("Verse", v[2].Name);
            Assert.AreEqual("2", v[2].Value);

            Assert.AreEqual("selectedWord", v[3].Name);
            Assert.AreEqual("carakiau", v[3].Value);
        }
        #endregion
        #region Test: UrlAttrList_Parsing2
        [Test] public void UrlAttrList_Parsing2()
        {
            string sUrl = "oxes://testChorus.lift/entryguid='8bbee099-1a7e-44a2-99fc-fbbc6ad20894'";

            var v = new UrlAttrList("oxes://", sUrl);

            Assert.AreEqual(1, v.Count);

            Assert.AreEqual("entryguid", v[0].Name);
            Assert.AreEqual("8bbee099-1a7e-44a2-99fc-fbbc6ad20894", v[0].Value);
        }
        #endregion
        #region Test: UrlAttrList_Finding
        [Test] public void UrlAttrList_Finding()
        {
            string sUrl = "oxes://book=Mrk+Chapter=3+Verse=2+selectedWord=carakiau";

            var v = new UrlAttrList("oxes://", sUrl);

            Assert.AreEqual("Mrk", v.GetValueFor("book"));
            Assert.AreEqual("3", v.GetValueFor("Chapter"));
            Assert.AreEqual("2", v.GetValueFor("Verse"));
            Assert.AreEqual("carakiau", v.GetValueFor("selectedWord"));
        }
        #endregion
        #region Test: MakeUrl
        [Test] public void MakeUrl()
        {
            var ua = new UrlAttrList();

            ua.Add(new UrlAttr("book", "John"));
            ua.Add(new UrlAttr("chapter", "3"));
            ua.Add(new UrlAttr("verse", "16"));

            string sUrl = ua.MakeUrl("oxes://");

            Assert.AreEqual("oxes://book=John+chapter=3+verse=16", sUrl, "Should be identical.");
        }
        #endregion

        #region Test: FindNodeIndex
        [Test] public void FindNodeIndex()
        {
            // Load in some oxes data
            var xmlOxes = new XmlDoc(SectionTestData.BaikenoMark0101_oxes);
            xmlOxes.NormalizeCreatedDates();

            // We'll look for the <bt> in the book's 3rd paragraph
            var nodeBible = XmlDoc.FindNode(xmlOxes, "bible");
            var nodeBook = XmlDoc.FindNode(nodeBible, "book");
            var nodeThirdPara = nodeBook.ChildNodes[2];
            var nodeBT = XmlDoc.FindNode(nodeThirdPara, "bt");

            // Find the node's position; that is, how many occurences of <bt> have
            // happened by the time we get to the one in question. 
            var e = new XmlDocException(nodeBT, "test");
            int c = 0;
            e.FindNodeIndex(xmlOxes, ref c);

            // We expect it to be 3
            Assert.AreEqual(3, c);

            // We need a temporary file with the oxes data, that we can then read in
            var vsOxes = SectionTestData.BaikenoMark0101_oxes;
            TextWriter W = JWU.NUnit_OpenTextWriter("test.x");
            foreach (string s in vsOxes)
                W.WriteLine(s);
            W.Close();

            // Get the actual line number
            int iLine = e.GetProblemLineNo(JWU.NUnit_TestFilePathName, xmlOxes);

            // We expect it to be 22
            Assert.AreEqual(22, iLine);
        }
        #endregion
    }
}
