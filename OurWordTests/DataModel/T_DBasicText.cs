/**********************************************************************************************
 * Project: OurWord! - Tests
 * File:    T_DBasicText.cs
 * Author:  John Wimbish
 * Created: 05 Mar 2008
 * Purpose: Tests the DBasicText class
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
    [TestFixture] public class T_DBasicText
    {
        #region Setup
        [SetUp] public void Setup()
        {
            JWU.NUnit_Setup();
            OurWordMain.Project = new DProject();
            G.Project.TeamSettings = new DTeamSettings();
            G.TeamSettings.EnsureInitialized();
            G.Project.DisplayName = "Project";
        }
        #endregion

        #region Test: PhrasesLength
        [Test]
        public void PhrasesLength()
        {
            string s1 = "For God so loved the ";
            string s2 = "world ";
            string s3 = "that he gave his one and only son";

            // Create a multi-phrase text
            DText text = new DText();
            text.Phrases.Append(new DPhrase(DStyleSheet.c_StyleAbbrevNormal, s1));
            text.Phrases.Append(new DPhrase(DStyleSheet.c_StyleAbbrevItalic, s2));
            text.Phrases.Append(new DPhrase(DStyleSheet.c_StyleAbbrevNormal, s3));

            // Test it's length
            int n = s1.Length + s2.Length + s3.Length;
            Assert.AreEqual(n, text.PhrasesLength);
        }
        #endregion

        #region Test: Delete
        [Test]
        public void Delete()
        {
            // Total = "For God so loved the world that he gave his one and only son";
            //          0         1         2         3         4         5         6

            // Delete "so", should have "For God loved..."
            DText text = _Delete_Setup();
            text.Phrases.Delete(8, 2);
            Assert.AreEqual("For God loved the world that he gave his one and only son", text.AsString, "1");

            // Delete "For", should have " God so loved..."
            text = _Delete_Setup();
            text.Phrases.Delete(0, 3);
            Assert.AreEqual(" God so loved the world that he gave his one and only son", text.AsString, "2");

            // Delete "the [wo" should have "the [rld ] that he..."
            text = _Delete_Setup();
            text.Phrases.Delete(17, 6);
            Assert.AreEqual("For God so loved rld that he gave his one and only son", text.AsString, "3");
            Assert.AreEqual(3, text.Phrases.Count);
            Assert.AreEqual("rld ", (text.Phrases[1] as DPhrase).Text);
            Assert.AreEqual("For God so loved ", (text.Phrases[0] as DPhrase).Text);
            Assert.AreEqual("that he gave his one and only son", (text.Phrases[2] as DPhrase).Text);

            // Delete "son" should have "...and only "
            text = _Delete_Setup();
            text.Phrases.Delete(57, 3);
            Assert.AreEqual("For God so loved the world that he gave his one and only ", text.AsString, "4");

            // Delete "[world" should have "...loved the that he...", one big phrase
            text = _Delete_Setup();
            text.Phrases.Delete(21, 5);
            Assert.AreEqual("For God so loved the that he gave his one and only son", text.AsString, "5");
            Assert.AreEqual(1, text.Phrases.Count);

            // Delete "[world ]" should have "...loved the that he..."
            text = _Delete_Setup();
            text.Phrases.Delete(21, 6);
            Assert.AreEqual("For God so loved the that he gave his one and only son", text.AsString, "6");
            Assert.AreEqual(1, text.Phrases.Count);

            // Delete "the [world ]that" should have "...loved he gave..."
            text = _Delete_Setup();
            text.Phrases.Delete(17, 14);
            Assert.AreEqual("For God so loved he gave his one and only son", text.AsString, "7");
            Assert.AreEqual(1, text.Phrases.Count);

            // Do nothing with a zero amount to delete
            text = _Delete_Setup();
            text.Phrases.Delete(17, 0);
            Assert.AreEqual("For God so loved the world that he gave his one and only son", text.AsString, "8");

            // Fail if position out of bounds
            text = _Delete_Setup();
            _Delete_TestBadInput(text, -1, 1);
            _Delete_TestBadInput(text, 59, 2);
            _Delete_TestBadInput(text, 60, 1);
            _Delete_TestBadInput(text, 61, 1);

            // Delete everything, should result in a single, empty DPhrase
            text = _Delete_Setup();
            text.Phrases.Delete(0, 60);
            Assert.AreEqual("", text.AsString, "9");
            Assert.AreEqual(1, text.Phrases.Count, "Should have 1 DPhrase here");
        }

        #region Method: DText _Delete_Setup()
        DText _Delete_Setup()
        {
            // Total = "For God so loved the world that he gave his one and only son";
            string s1 = "For God so loved the ";
            string s2 = "world ";
            string s3 = "that he gave his one and only son";

            // Create a multi-phrase text
            DText text = new DText();
            text.Phrases.Append(new DPhrase(DStyleSheet.c_StyleAbbrevNormal, s1));
            text.Phrases.Append(new DPhrase(DStyleSheet.c_StyleAbbrevItalic, s2));
            text.Phrases.Append(new DPhrase(DStyleSheet.c_StyleAbbrevNormal, s3));

            return text;
        }
        #endregion
        #region Method: void _Delete_TestBadInput(DText text, int iStart, int iCount)
        void _Delete_TestBadInput(DText text, int iStart, int iCount)
        {
            try
            {
                text.Phrases.Delete(iStart, iCount);
                Assert.Fail("Shouldn't get here... " + iStart.ToString() + ", " + iCount.ToString());
            }
            catch (ArgumentOutOfRangeException)
            {
                //Console.WriteLine("OutOfRange caught: " + iStart.ToString() + ", " + iCount.ToString());
            }
        }
        #endregion
        #endregion
    }
}
