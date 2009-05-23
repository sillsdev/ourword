/**********************************************************************************************
 * Project: OurWord! - Tests
 * File:    T_DTranslatorNote.cs
 * Author:  John Wimbish
 * Created: 01 Dec 2008
 * Purpose: Tests the TranslatorNote class
 * Legal:   Copyright (c) 2004-09, John S. Wimbish. All Rights Reserved.  
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
using JWdb.DataModel;
using OurWord.Dialogs;
using OurWord.View;
#endregion
#region Doc: Paratext7's Notes xml format
   /* Per email from Nathan (Nov 08), Paratext7's notes will be stored as:
    * 
    * <Comment>
    * <Thread>de0d8491-6857-454e-8533-f8b10aa6de94</Thread>
    * <User>Nathan Miles</User>
    * <Date>2008-08-17T20:15:04.8940064-05:00</Date>
    * <VerseRef>LUK 10:9</VerseRef>
    * <SelectedText>God Bizibagh Ativamin Dughiam</SelectedText>
    * <StartPosition>76</StartPosition>
    * <ContextBefore />
    * <ContextAfter />
    * <Status>todo</Status>
    * <Contents>This phrase needs to be in the next verse also</Contents>
    * </Comment>
    */
#endregion

namespace OurWordTests.DataModel
{
    [TestFixture] public class T_DTranslatorNote
    {
        DSection m_Section;

        // Test Data & Etc
        #region Setup 
        [SetUp] public void Setup()
        {
            JWU.NUnit_Setup();
            DB.Project = new DProject();
            DB.Project.TeamSettings = new DTeamSettings();
            DB.TeamSettings.EnsureInitialized();
            DB.Project.DisplayName = "Test Project";
            DB.Project.TargetTranslation = new DTranslation();

            DBook book = new DBook("LUK");
            DB.Project.TargetTranslation.Books.Append(book);

            m_Section = new DSection(1);
            book.Sections.Append(m_Section);
        }
        #endregion
        #region Method: TranslatorNote CreateTestTranslatorNote()
        TranslatorNote CreateTestTranslatorNote()
        {
            TranslatorNote tn = new TranslatorNote("003:016", "so loved the world");
            tn.Category = "To Do";
            tn.AssignedTo = "John";
            tn.Discussions.Append(new Discussion("John", new DateTime(2008, 11, 1), 
                "Check exegesis here."));
            tn.Discussions.Append(new Discussion("Sandra", new DateTime(2008, 11, 3), 
                "Exegesis is fine."));
            return tn;
        }
        #endregion
        #region Method: string GetExpectedParagraphString(string sText)
        string GetExpectedParagraphString(string sText)
        {
            /***
             * This code represents the XML of a complete paragraph, with runs, dtext,
             * etc. In order to keep files human-readable and sizes down, I made it
             * possible for some of the simple paragraphs to be presented in a 
             * more abbreviated manner. But I keep this around both as a reminder,
             * and in case I have to go back at some point.
             * 
            string s = "<ownseq Name=\"paras\">";
            s += "<DParagraph Abbrev=\"NoteDiscussion\">";
            s += "<ownseq Name=\"Runs\">";
            s += "<DText>";

            s += "<ownseq Name=\"Phrase\">";
            s += "<DPhrase Text=\"" + sText + "\" Style=\"p\"/>";
            s += "</ownseq>";

            s += "</DText>";
            s += "</ownseq>";
            s += "</DParagraph>";
            s += "</ownseq>";
            ***/

            string s = "<ownseq Name=\"paras\">";
            s += "<DParagraph Abbrev=\"NoteDiscussion\" ";
            s += "Contents=\"" + sText + "\"/>";
            s += "</ownseq>";

            return s;
        }
        #endregion
        #region Method: TranslatorNote CreateFromXmlString(string s)
        TranslatorNote CreateFromXmlString(string s)
        {
            XElement x = XElement.CreateFrom(s)[0];
            TranslatorNote note = new TranslatorNote();
            note.FromXml(x);
            return note;
        }
        #endregion
        // Content Equals
        #region Test: ContentEquals_Discussion
        [Test] public void ContentEquals_Discussion()
        {
            // Create a base discussion object
            Discussion discussion = new Discussion("John", new DateTime(2008, 11, 23),
                "Is bibit the correct term for seed here?");

            // Test equality
            Assert.IsTrue(discussion.ContentEquals(discussion));

            // Test if author gets changed
            Discussion d2 = new Discussion("Sandra", discussion.Created,
                "Is bibit the correct term for seed here?");
            Assert.IsFalse(discussion.ContentEquals(d2));

            // Test if date gets changed
            Discussion d3 = new Discussion("John", DateTime.Now,
                "Is bibit the correct term for seed here?");
            Assert.IsFalse(discussion.ContentEquals(d3));

            // Test if content gets changed
            Discussion d4 = new Discussion("John", discussion.Created,
                "This is different");
            Assert.IsFalse(discussion.ContentEquals(d4));
        }
        #endregion
        #region Test: ContentEquals_Note
        [Test] public void ContentEquals_Note()
        {
            // Set up a Translator Note
            TranslatorNote tn1 = new TranslatorNote("003:016", "so loved the world");
            tn1.Category = "To Do";
            tn1.AssignedTo = "John";
            tn1.Discussions.Append(new Discussion("John", new DateTime(2008, 11, 1), "Check exegesis here."));

            // Set up a duplicate
            TranslatorNote tn2 = new TranslatorNote("003:016", "so loved the world");
            tn2.Category = "To Do";
            tn2.AssignedTo = "John";
            tn2.Discussions.Append(new Discussion("John", new DateTime(2008, 11, 1), "Check exegesis here."));

            // Equality
            Assert.IsTrue(tn1.ContentEquals(tn2));

            // Category differs
            tn2.Category = "Old Version";
            Assert.IsFalse(tn1.ContentEquals(tn2));
            tn2.Category = tn1.Category;

            // AssignedTo differs
            tn2.AssignedTo = "Sandra";
            Assert.IsFalse(tn1.ContentEquals(tn2));
            tn2.AssignedTo = tn1.AssignedTo;

            // Context differs
            tn2.Context = "loved the world";
            Assert.IsFalse(tn1.ContentEquals(tn2));
            tn2.Context = tn1.Context;

            // Reference differs
            tn2.Reference = "004:016";
            Assert.IsFalse(tn1.ContentEquals(tn2));
            tn2.Reference = tn1.Reference;

            // Discussion differs
            tn2.Discussions.Clear();
            Assert.IsFalse(tn1.ContentEquals(tn2));
        }
        #endregion

        // I/O
        #region Test: IO_Discussion
        [Test] public void IO_Discussion()
        {
            // Create a discussion object with attributes
            Discussion discussion = new Discussion("John", new DateTime(2008, 11, 23),
                "Is bibit the correct term for seed here?");

            // Get the Xml element
            XElement x = discussion.ToXml(true);

            // Is it what we expect?
            Assert.AreEqual(
                "<Discussion Author=\"John\" Created=\"2008-11-23 00:00:00Z\">" +
                    GetExpectedParagraphString("Is bibit the correct term for seed here?") +
                    "</Discussion>", 
                 x.OneLiner);

            // Create a Discussion object from this Xml element
            Discussion discussionRead = new Discussion();
            discussionRead.FromXml(x);

            // They should be identical
            Assert.IsNotNull(discussionRead, "is not null: discussionRead");
            Assert.IsTrue(discussion.ContentEquals(discussionRead), "ContentEquals");
        }
        #endregion
        #region Test: IO_Note
        [Test] public void IO_Note()
        {
            // Set up a Translator Note
            TranslatorNote tn = CreateTestTranslatorNote();

            // Do we get the XML save that we expect?
            XElement x = tn.ToXml(true);
            Assert.AreEqual(
                "<TranslatorNote Category=\"To Do\" AssignedTo=\"John\" " +
                    "Context=\"so loved the world\" Reference=\"003:016\" " +
                     "ShowInDaughter=\"false\">" +
                    "<ownseq Name=\"Discussions\">" +
                    "<Discussion Author=\"John\" Created=\"2008-11-01 00:00:00Z\">" +
                        GetExpectedParagraphString("Check exegesis here.") + "</Discussion>" +
                    "<Discussion Author=\"Sandra\" Created=\"2008-11-03 00:00:00Z\">" +
                        GetExpectedParagraphString("Exegesis is fine.") + "</Discussion>" +
                    "</ownseq></TranslatorNote>",
                x.OneLiner);

            // Create a new Translator Note from this Xml element; they should be identical
            TranslatorNote tnNew = new TranslatorNote();
            tnNew.FromXml(x);
            Assert.IsTrue(tn.ContentEquals(tnNew), "Should be identical");
        }
        #endregion
        #region Test: IO_Section
        [Test] public void IO_Section()
        {
            // Create the note we'll test
            TranslatorNote tn = CreateTestTranslatorNote();

            // Standard Format representation of a tiny Section
            string[] vs = new string[]
            {
			    "\\_sh v3.0 2 SHW-Scripture", 
			    "\\_DateStampHasFourDigitYear",
			    "\\rcrd MRK",
			    "\\mt Mark",
                "\\id MRK",
			    "\\rcrd Mark 4.1-4.1",
			    "\\c 4",
			    "\\s Tulu-tulu agama las haman Petrus nol Yohanis maas tala",
			    "\\bts The heads of religion summon Petrus and Yohanis to come appear.before [them]",
			    "\\p",
			    "\\v 1",
			    "\\vt Dedeng na, Petrus nol Yohanis nahdeh nabael nol atuli las sam, " +
				    "atuil tene kas at ila lo maas. Oen nas tulu-tulu Agama Yahudi, nol " +
				    "tulu in doh Um in Kohe kanas Tene ka, nol atuil deng partai" +
				    "agama Saduki. Oen maas komali le ahan Petrus nol Yohanis.",
			    "\\btvt At that time, Petrus and Yohanis still were talking with those people, " +
				    "several big/important people came. Those them(=They in focus), " +
				    "[were] heads of the Yahudi religion, and the head of guarding the " +
				    "*Temple, and people from the religious party Saduki. They came " +
				    "angry to scream at Petrus and Yohanis.",
			    "\\tn ",
            };
            vs[vs.Length - 1] += tn.ToXml(true).OneLiner;

            // Make sure we have something that looks like a note
            Assert.AreEqual("\\tn <Trans", vs[vs.Length-1].Substring(0, 10));

            // Do the test
            T_DSection t = new T_DSection();
            t.IO_TestEngine(vs, vs);
        }
        #endregion

        // Categories
        #region Test: Categories
        [Test] public void Categories()
        {
            TranslatorNote.InitClassifications();

            // Add a category twice
            TranslatorNote.Categories.AddItem("New Category", false);
            TranslatorNote.Categories.AddItem("New Category", false);

            // We expect to have the default categories plus our new one
            Assert.AreEqual(3, TranslatorNote.Categories.Count);
            Assert.AreNotEqual(null, TranslatorNote.Categories.FindItem("Exegesis"));
            Assert.AreNotEqual(null, TranslatorNote.Categories.FindItem("To Do"));
            Assert.AreNotEqual(null, TranslatorNote.Categories.FindItem("New Category"));
        }
        #endregion
        #region Test: CommaDelimitedString
        [Test] public void CommaDelimitedString()
        {
            // Initialize Categories to "To Do" and "Exegesis". 
            TranslatorNote.InitClassifications();

            // Add a few more
            TranslatorNote.Categories.AddItem("Hebrew", true);
            TranslatorNote.Categories.AddItem("Greek", true);

            // Check the produced string
            Assert.AreEqual("Exegesis, Greek, Hebrew, To Do, ",
                TranslatorNote.Categories.CommaDelimitedString,
                "Get CommaDelimitedString");

            // Create a different string
            string sCategories = "Emily, David, Robert, Christiane, Sandra, John, ";
            TranslatorNote.Categories.CommaDelimitedString = sCategories;

            // If it loaded correctly, we will have a rearranged string; plus "To Do"
            // added back in as the DefaultValue
            Assert.AreEqual("Christiane, David, Emily, John, Robert, Sandra, To Do, ",
                TranslatorNote.Categories.CommaDelimitedString,
                "Set CommaDelimitedString");
        }
        #endregion

        // Conversion from 1.0-styled notes
        #region Test: OldStyleNotesConversion_General
        [Test] public void OldStyleNotesConversion_General()
        {
            string sNoteText = "This is a general note.";
            SfField f = new SfField("nt", sNoteText);

            TranslatorNote tn = TranslatorNote.ImportFromOldStyle(2, 15, f);

            Assert.AreEqual("002:015", tn.Reference);
            Assert.AreEqual(sNoteText, tn.Discussions[0].Paragraphs[0].SimpleText);
            Assert.AreEqual("Unknown Author", tn.Discussions[0].Author);
            Assert.AreEqual("General", tn.Category);
        }
        #endregion
        #region Test: OldStyleNotesConversion_ToDo
        [Test] public void OldStyleNotesConversion_ToDo()
        {
            string sNoteText = "This is a To Do note.";
            SfField f = new SfField("ntck", sNoteText);

            TranslatorNote tn = TranslatorNote.ImportFromOldStyle(2, 15, f);

            Assert.AreEqual("002:015", tn.Reference);
            Assert.AreEqual(sNoteText, tn.Discussions[0].Paragraphs[0].SimpleText);
            Assert.AreEqual("Unknown Author", tn.Discussions[0].Author);
            Assert.AreEqual("To Do", tn.Category);
        }
        #endregion

        // Computing the Header Text
        #region Test: GetDisplayableReference
        [Test] public void GetDisplayableReference()
        {
            TranslatorNote tn = new TranslatorNote("010:020", "hello");
            Assert.AreEqual("10:20", tn.GetDisplayableReference());

            tn = new TranslatorNote("011:021", "hello");
            Assert.AreEqual("11:21", tn.GetDisplayableReference());
        }
        #endregion
        #region Test: GetWordsLeft
        [Test] public void GetWordsLeft()
        {
            string s = "For God so loved the world that he gave his only son.";

            Assert.AreEqual("so loved the world",
                TranslatorNote.GetWordsLeft(s, 27, 4), "1");

            Assert.AreEqual("God so loved the world",
                TranslatorNote.GetWordsLeft(s, 26, 4), "2");

            Assert.AreEqual("For God s",
                TranslatorNote.GetWordsLeft(s, 9, 4), "3");
        }
        #endregion
        #region Test: GetWordsRight
        [Test] public void GetWordsRight()
        {
            string s = "For God so loved the world that he gave his only son.";

            Assert.AreEqual("that he gave his only",
                TranslatorNote.GetWordsRight(s, 27, 4), "1");

            Assert.AreEqual("that he gave his",
                TranslatorNote.GetWordsRight(s, 26, 4), "2");

            Assert.AreEqual("s only son.",
                TranslatorNote.GetWordsRight(s, 42, 4), "3");
        }
        #endregion
        #region Test: ComputeHeader
        [Test] public void ComputeHeader()
        {
            char chSpace = DPhrase.c_chInsertionSpace;
            string sVerse = "For God so loved the world, that he gave his one and only son.";

            // Case 1 - A context within a containing text
            TranslatorNote tn = new TranslatorNote("003:016", "loved");
            DBasicText text = tn.GetCollapsableHeaderText(sVerse);
            Assert.AreEqual("3:16:" + chSpace + "For God so loved the world, that he", text.AsString);
            Assert.AreEqual(4, text.Phrases.Count);

            // Case 2 - No containing text
            text = tn.GetCollapsableHeaderText("");
            Assert.AreEqual("3:16:" + chSpace + "loved", text.AsString);

            // Case 3 - No Context
            tn.Context = "";
            text = tn.GetCollapsableHeaderText("");
            Assert.AreEqual("3:16:" + chSpace, text.AsString);
        }
        #endregion
        #region Test: RemoveInitialRefFromText
        [Test] public void RemoveInitialRefFromText()
        {
            TranslatorNote tn = new TranslatorNote("010:020", "");

            Assert.AreEqual("This how it is.",
                tn.RemoveInitialReferenceFromText("10:20: This how it is."));

            Assert.AreEqual("This how it is.",
                tn.RemoveInitialReferenceFromText("10:20 This how it is."));

            Assert.AreEqual("10:21 This how it is.",
                tn.RemoveInitialReferenceFromText("10:21 This how it is."));


        }
        #endregion

        // Merge
        #region Test: MergeDiscussion_WeChanged
        [Test] public void MergeDiscussion_WeChanged()
        {
            Discussion Parent = new Discussion("John", new DateTime(2008, 11, 23),
                "Is bibit the correct term for seed here?");
            Discussion Mine = new Discussion("John", new DateTime(2008, 11, 23),
                "I say Bibit is the correct term.");
            Discussion Theirs = new Discussion("John", new DateTime(2008, 11, 23),
                "Is bibit the correct term for seed here?");

            Mine.Merge(Parent, Theirs);

            Assert.AreEqual(
                "D: Author={John} Created={11/23/2008} + Content={p:<I say Bibit is the correct term.>}",
                Mine.DebugString);
        }
        #endregion
        #region Test: MergeDiscussion_TheyChanged
        [Test] public void MergeDiscussion_TheyChanged()
        {
            Discussion Parent = new Discussion("John", new DateTime(2008, 11, 23),
                "Is bibit the correct term for seed here?");
            Discussion Theirs = new Discussion("John", new DateTime(2008, 11, 23),
                "They say Bibit is the correct term.");
            Discussion Mine = new Discussion("John", new DateTime(2008, 11, 23),
                "Is bibit the correct term for seed here?");

            Mine.Merge(Parent, Theirs);

            Assert.AreEqual(
                "D: Author={John} Created={11/23/2008} + Content={p:<They say Bibit is the correct term.>}",
                Mine.DebugString);
        }
        #endregion
        #region Test: MergeDiscussion_BothChanged
        [Test] public void MergeDiscussion_BothChanged()
        {
            Discussion Parent = new Discussion("John", new DateTime(2008, 11, 23),
                "Is bibit the correct term for seed here?");
            Discussion Theirs = new Discussion("John", new DateTime(2008, 11, 23),
                "They say Bibit is the correct term.");
            Discussion Mine = new Discussion("John", new DateTime(2008, 11, 23),
                "I say Bibit is the wrong term.");

            // TEST 1: They Win
            // Set the DefaultAuthor to "Mary" so that "They" should win
            string sDefaultAuthor = DB.UserName;
            DB.UserName = "Mary";
            Mine.Merge(Parent, Theirs);
            DB.UserName = sDefaultAuthor;
            Assert.AreEqual(
                "D: Author={John} Created={11/23/2008} + " +
                    "Content={p:<They say Bibit is the correct term.>}",
                Mine.DebugString);

            // TEST 2: We Win
            // Go again, but this time with "John" so that "We" win
            Mine = new Discussion("John", new DateTime(2008, 11, 23),
                "I say Bibit is the wrong term.");
            DB.UserName = "John";
            Mine.Merge(Parent, Theirs);
            DB.UserName = sDefaultAuthor;
            Assert.AreEqual(
                "D: Author={John} Created={11/23/2008} + " +
                    "Content={p:<I say Bibit is the wrong term.>}",
                Mine.DebugString);
        }
        #endregion
        #region Test: void MergeNote()
        [Test] public void MergeNote()
            // Tests that:
            // - Both Mine and Theirs created a new Discussion; tests that both were
            //     added.
            // - Both Mine and Theirs changed the AssignedTo, but since Mine was the
            //    most recent Discussion, the result should use Mine's AssignedTo.
        {
            string sParent = "<TranslatorNote Category=\"To Do\" AssignedTo=\"John\" " +
                "Context=\"so loved the world\" Reference=\"003:016\" ShowInDaughter=\"false\">" +
                "<ownseq Name=\"Discussions\">" +
                  "<Discussion Author=\"John\" Created=\"2008-11-01 00:00:00Z\">" +
                    GetExpectedParagraphString("Check exegesis here.") + "</Discussion>" +
                  "<Discussion Author=\"Sandra\" Created=\"2008-11-03 00:00:00Z\">" +
                    GetExpectedParagraphString("Exegesis is fine.") + "</Discussion>" +
                "</ownseq></TranslatorNote>";
            string sMine = "<TranslatorNote Category=\"To Do\" AssignedTo=\"Sandra\" " +
                "Context=\"so loved the world\" Reference=\"003:016\" ShowInDaughter=\"false\">" +
                "<ownseq Name=\"Discussions\">" +
                  "<Discussion Author=\"John\" Created=\"2008-11-01 00:00:00Z\">" +
                    GetExpectedParagraphString("Check exegesis here.") + "</Discussion>" +
                  "<Discussion Author=\"Sandra\" Created=\"2008-11-03 00:00:00Z\">" +
                    GetExpectedParagraphString("Exegesis is fine.") + "</Discussion>" +
                  "<Discussion Author=\"John\" Created=\"2008-11-15 00:00:00Z\">" +
                    GetExpectedParagraphString("I'm not convinced.") + "</Discussion>" +
                "</ownseq></TranslatorNote>";
            string sTheirs = "<TranslatorNote Category=\"To Do\" AssignedTo=\"Emily\" " +
                "Context=\"so loved the world\" Reference=\"003:016\" ShowInDaughter=\"false\">" +
                "<ownseq Name=\"Discussions\">" +
                  "<Discussion Author=\"John\" Created=\"2008-11-01 00:00:00Z\">" +
                    GetExpectedParagraphString("Check exegesis here.") + "</Discussion>" +
                  "<Discussion Author=\"Sandra\" Created=\"2008-11-03 00:00:00Z\">" +
                    GetExpectedParagraphString("Exegesis is fine.") + "</Discussion>" +
                  "<Discussion Author=\"Sandra\" Created=\"2008-11-14 00:00:00Z\">" +
                    GetExpectedParagraphString("Really, it is!") + "</Discussion>" +
                "</ownseq></TranslatorNote>";

            TranslatorNote Parent = CreateFromXmlString(sParent);
            TranslatorNote Mine = CreateFromXmlString(sMine);
            TranslatorNote Theirs = CreateFromXmlString(sTheirs);

            Mine.Merge(Parent, Theirs);

            string sExpected = "<TranslatorNote Category=\"To Do\" AssignedTo=\"Sandra\" " +
                "Context=\"so loved the world\" Reference=\"003:016\" ShowInDaughter=\"false\">" +
                "<ownseq Name=\"Discussions\">" +
                  "<Discussion Author=\"John\" Created=\"2008-11-01 00:00:00Z\">" +
                    GetExpectedParagraphString("Check exegesis here.") + "</Discussion>" +
                  "<Discussion Author=\"Sandra\" Created=\"2008-11-03 00:00:00Z\">" +
                    GetExpectedParagraphString("Exegesis is fine.") + "</Discussion>" +
                  "<Discussion Author=\"Sandra\" Created=\"2008-11-14 00:00:00Z\">" +
                    GetExpectedParagraphString("Really, it is!") + "</Discussion>" +
                  "<Discussion Author=\"John\" Created=\"2008-11-15 00:00:00Z\">" +
                    GetExpectedParagraphString("I'm not convinced.") + "</Discussion>" +
                "</ownseq></TranslatorNote>";

            XElement xOut = Mine.ToXml(true);
            string sOut = xOut.OneLiner;
            Assert.AreEqual(sExpected, sOut);
        }
        #endregion

    }
}
