/**********************************************************************************************
 * Project: OurWord! - Tests
 * File:    T_DPhrase.cs
 * Author:  John Wimbish
 * Created: 05 Mar 2008
 * Purpose: Tests the DPhrase class
 * Legal:   Copyright (c) 2004-09, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
#region Using
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using NUnit.Framework;

using JWTools;
using OurWordData;

using OurWord;
using OurWordData.DataModel;
using OurWord.Dialogs;
using OurWord.Layouts;
using OurWordData.Tools;
using OurWordTests;
#endregion

namespace OurWordTests.DataModel
{
    [TestFixture] public class T_DPhrase
    {
        #region Setup
        [SetUp] public void Setup()
        {
            TestCommon.GlobalTestSetup();
            DB.Project = new DProject();
            DB.Project.TeamSettings = new DTeamSettings();
            DB.TeamSettings.EnsureInitialized();
            DB.Project.DisplayName = "Project";
        }
        #endregion

        #region Test: SplitPhraseInMiddle
        [Test]  public void SplitPhraseInMiddle()
        {
            // Create a test phrase
            DText text = DText.CreateSimple("This is a phrase.");
            Assert.AreEqual(1, text.Phrases.Count);
            DPhrase phrase = text.Phrases[0] as DPhrase;
            Assert.IsNotNull(phrase);

            // Do the split
            text.Phrases.Split(phrase, 5);

            // Should have two phrases now
            Assert.AreEqual(2, text.Phrases.Count);

            DPhrase phraseLeft = text.Phrases[0] as DPhrase;
            Assert.AreEqual("This ", phraseLeft.Text);

            DPhrase phraseRight = text.Phrases[1] as DPhrase;
            Assert.AreEqual("is a phrase.", phraseRight.Text);
        }
        #endregion
        #region Test: SplitPhraseAtStart
        [Test] public void SplitPhraseAtStart()
        {
            // Create a test phrase
            DText text = DText.CreateSimple("This is a phrase.");
            Assert.AreEqual(1, text.Phrases.Count);
            DPhrase phrase = text.Phrases[0] as DPhrase;
            Assert.IsNotNull(phrase);

            // Do the split
            text.Phrases.Split(phrase, 0);

            // Should have one phrase now
            Assert.AreEqual(1, text.Phrases.Count);
            DPhrase phraseLeft = text.Phrases[0] as DPhrase;
            Assert.AreEqual("This is a phrase.", phraseLeft.Text);
        }
        #endregion
        #region Test: SplitPhraseAtEnd
        [Test] public void SplitPhraseAtEnd()
        {
            // Create a test phrase
            DText text = DText.CreateSimple("Hello");
            Assert.AreEqual(1, text.Phrases.Count);
            DPhrase phrase = text.Phrases[0] as DPhrase;
            Assert.IsNotNull(phrase);

            // Do the split
            text.Phrases.Split(phrase, 5);

            // Should have one phrase now
            Assert.AreEqual(1, text.Phrases.Count);
            DPhrase phraseLeft = text.Phrases[0] as DPhrase;
            Assert.AreEqual("Hello", phraseLeft.Text);
        }
        #endregion

        #region Test: JoinPhrases
        [Test] public void JoinPhrases()
        {
            // Start with a single phrase
            string sText = "This is a phrase";
            DText text = DText.CreateSimple(sText);

            // Split into two
            DPhrase phrase = text.Phrases[0] as DPhrase;
            Assert.IsNotNull(phrase);
            text.Phrases.Split(phrase, 5);
            Assert.AreEqual(2, text.Phrases.Count);

            // Now join back together
            text.Join(0);

            // Should have a single phrase
            Assert.AreEqual(1, text.Phrases.Count);
            Assert.AreEqual(sText, (text.Phrases[0] as DPhrase).Text);
        }
        #endregion

        #region Test: TSfmSaveString
        [Test] public void TSfmSaveString()
        {
            var phrase = new DPhrase 
            {
                Text="Hi",
                FontModification = FontStyle.Italic
            };
            Assert.AreEqual("|iHi|r", phrase.SfmSaveString);

            phrase = new DPhrase
            {
                Text = "Hi",
                FontModification = FontStyle.Bold
            };
            Assert.AreEqual("|bHi|r", phrase.SfmSaveString);

            phrase = new DPhrase
            {
                Text = "Hi",
                FontModification = FontStyle.Underline
            };
            Assert.AreEqual("|uHi|r", phrase.SfmSaveString);
        }
        #endregion
    }
}
