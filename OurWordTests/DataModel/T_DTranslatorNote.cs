#region ***** T_DTranslatorNote.cs *****
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
using OurWord.Layouts;
#endregion
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
    #region T_DMessage
    [TestFixture] public class T_DMessage : TestCommon
    {
        DSection m_Section;
        #region Setup
        [SetUp] public void Setup()
        {
            JWU.NUnit_Setup();
            m_Section = CreateHierarchyThroughTargetSection("MRK");
        }
        #endregion

        // General
        #region Test: ContentEquals
        [Test] public void ContentEquals()
        {
            // Create a base Message object
            var sAuthor = "John";
            var dt = new DateTime(2008, 11, 23);
            var sStatus = "David";
            var sMessage = "Is bibit the correct term for seed here?";
            var message = new DMessage(sAuthor, dt, sStatus, sMessage);

            // Test equality
            Assert.IsTrue(message.ContentEquals(message));

            // Test if author gets changed
            var m2 = new DMessage("Sandra", dt, sStatus, sMessage);
            Assert.IsFalse(message.ContentEquals(m2));

            // Test if date gets changed
            var m3 = new DMessage(sAuthor, DateTime.Now, sStatus, sMessage);
            Assert.IsFalse(message.ContentEquals(m3));

            // Test if status gets changed
            var m4 = new DMessage(sAuthor, dt, "Emily", sMessage);
            Assert.IsFalse(message.ContentEquals(m3));

            // Test if content gets changed
            var m5 = new DMessage(sAuthor, dt, sStatus, "This is different");
            Assert.IsFalse(message.ContentEquals(m4));
        }
        #endregion

        // I/O
        #region Test: ImportMessageFromToolboxXml_Simple
        [Test] public void ImportMessageFromToolboxXml_Simple()
            // Simple is the shortened version, where a paragraph has a single SimpleText.
            // Complex would be where there are multiple runs for, e.g., Italic.
        {
            var vsToImport = new string[] {
                "<Discussion Author=\"Ibu Linda\" Created=\"2009-05-07 09:27:24Z\">",
                    "<ownseq Name=\"paras\">",
                        "<DParagraph Abbrev=\"NoteDiscussion\" Contents=\"Inanayanambe nyo raura kakavimbe indamu syo ranaun. Weti no kai, weramu syare wamo raporar taiso: &quot;mbambunin ai&quot;.\"/>",
                    "</ownseq>",
                "</Discussion>",
            };
            var vsOxesExpected = new string[] {
                "<Message author=\"Ibu Linda\" created=\"2009-05-07 09:27:24Z\">",
                     "Inanayanambe nyo raura kakavimbe indamu syo ranaun. Weti no kai, weramu syare wamo raporar taiso: &quot;mbambunin ai&quot;.",
                "</Message>",
           };

            // Create an Oxes object for Expected
            var xmlOxesExpected = new XmlDoc(vsOxesExpected);

            // Import into a Message object
            var xmlImported = new XmlDoc(vsToImport);
            var nodeMessage = XmlDoc.FindNode(xmlImported, "Discussion");
            var message = new DMessage(nodeMessage);

            // Create an Oxes object for saving
            var xmlOxesActual = new XmlDoc();
            message.Save(xmlOxesActual, xmlOxesActual);

            // Are they the same?
            bool bIsSame = XmlDoc.Compare(xmlOxesExpected, xmlOxesActual);
            Assert.IsTrue(bIsSame, "Message xmls should be the same.");
        }
        #endregion
        #region Test: OxesIO
        [Test] public void OxesIO()
        {
            // Cannonical form of a Message object
            string[] vsOxesExpected = new string[] { 
                "<Message author=\"John\" created=\"2008-11-23 00:00:00Z\">" ,
                    "Is <span class=\"Italic\">bibit </span>the correct term for seed here?",
                    "<bt>Memang mau pakai bibit di sini?</bt>",
                "</Message>"
            };

            // Create the XmlDoc
            var xmlOxesExpected = new XmlDoc(vsOxesExpected);
            var nodeMessage = XmlDoc.FindNode(xmlOxesExpected, DMessage.c_sTagMessage);
            //xmlOxesExpected.WriteToConsole("Expected");

            // Create the Message object from the Xml node
            var message = new DMessage(nodeMessage);

            // Save this new Message to oxes
            var xmlOxesActual = new XmlDoc();
            message.Save(xmlOxesActual, xmlOxesActual);

            // Are they the same?
            bool bIsSame = XmlDoc.Compare(xmlOxesExpected, xmlOxesActual);
            Assert.IsTrue(bIsSame, "Message xmls should be the same.");
        }
        #endregion

        // Merge
        #region Test: MergeMessage_WeChanged
        [Test] public void MergeMessage_WeChanged()
        {
            var Parent = new DMessage("John", new DateTime(2008, 11, 23), "David",
                "Is bibit the correct term for seed here?");
            var Mine = new DMessage("John", new DateTime(2008, 11, 23), "Emily",
                "I say Bibit is the correct term.");
            var Theirs = new DMessage("John", new DateTime(2008, 11, 23), "David",
                "Is bibit the correct term for seed here?");

            Mine.Merge(Parent, Theirs);

            Assert.AreEqual(
                "M: Author={John} Created={11/23/2008} Status={Emily} Content={I say Bibit is the correct term.}",
                Mine.DebugString);
        }
        #endregion
        #region Test: MergeMessage_TheyChanged
        [Test] public void MergeMessage_TheyChanged()
        {
            var Parent = new DMessage("John", new DateTime(2008, 11, 23), "David",
                "Is bibit the correct term for seed here?");
            var Theirs = new DMessage("John", new DateTime(2008, 11, 23), "Emily",
                "They say Bibit is the correct term.");
            var Mine = new DMessage("John", new DateTime(2008, 11, 23), "David",
                "Is bibit the correct term for seed here?");

            Mine.Merge(Parent, Theirs);

            Assert.AreEqual(
                "M: Author={John} Created={11/23/2008} Status={Emily} Content={They say Bibit is the correct term.}",
                Mine.DebugString);
        }
        #endregion
        #region Test: MergeMessage_BothChanged
        [Test] public void MergeMessage_BothChanged()
        {
            var Parent = new DMessage("John", new DateTime(2008, 11, 23), "David",
                "Is bibit the correct term for seed here?");
            var Theirs = new DMessage("John", new DateTime(2008, 11, 23), "Emily",
                "They say Bibit is the correct term.");
            var Mine = new DMessage("John", new DateTime(2008, 11, 23), "Robert",
                "I say Bibit is the wrong term.");

            // TEST 1: They Win
            // Set the DefaultAuthor to "Mary" so that "They" should win
            string sDefaultAuthor = DB.UserName;
            DB.UserName = "Mary";
            Mine.Merge(Parent, Theirs);
            DB.UserName = sDefaultAuthor;
            Assert.AreEqual(
                "M: Author={John} Created={11/23/2008} Status={Emily} Content={They say Bibit is the correct term.}",
                Mine.DebugString);

            // TEST 2: We Win
            // Go again, but this time with "John" so that "We" win
            Mine = new DMessage("John", new DateTime(2008, 11, 23), "Robert",
                "I say Bibit is the wrong term.");
            DB.UserName = "John";
            Mine.Merge(Parent, Theirs);
            DB.UserName = sDefaultAuthor;
            Assert.AreEqual(
                "M: Author={John} Created={11/23/2008} Status={Robert} Content={I say Bibit is the wrong term.}",
                Mine.DebugString);
        }
        #endregion
    }
    #endregion

    #region T_DTranslatorNote
    [TestFixture] public class T_DTranslatorNote : TestCommon
    {
        DSection m_Section;

        // Test Data & Etc
        #region Setup 
        [SetUp] public void Setup()
        {
            JWU.NUnit_Setup();
            DB.Project = new DProject();
            DB.Project.TeamSettings = new DTeamSettings(JWU.NUnit_ClusterFolderName);
            DB.TeamSettings.EnsureInitialized();
            DB.Project.DisplayName = "Test Project";
            DB.Project.TargetTranslation = new DTranslation();

            DBook book = new DBook("LUK");
            DB.Project.TargetTranslation.Books.Append(book);

            m_Section = new DSection();
            book.Sections.Append(m_Section);

            JWU.NUnit_SetupClusterFolder();
        }
        #endregion
        #region TearDown
        [TearDown] public void TearDown()
        {
            JWU.NUnit_TeardownClusterFolder();
        }
        #endregion

        #region Method: TranslatorNote CreateTestTranslatorNote()
        TranslatorNote CreateTestTranslatorNote()
        {
            TranslatorNote tn = new TranslatorNote("so loved the world");
            tn.Class = TranslatorNote.NoteClass.General;
            tn.Messages.Append(new DMessage("John", new DateTime(2008, 11, 1), "Sandra",
                "Check exegesis here."));
            tn.Messages.Append(new DMessage("Sandra", new DateTime(2008, 11, 3), DMessage.Closed,
                "Exegesis is fine."));
            return tn;
        }
        #endregion
        #region Method: TranslatorNote CreateFromXmlString(s, sTag)
        TranslatorNote CreateFromXmlString(string s, string sTag)
        {
            var oxes = new XmlDoc(s);
            var node = XmlDoc.FindNode(oxes, sTag);
            return TranslatorNote.Create(node);
        }
        #endregion

        // Content Equals
        #region Test: ContentEquals_Note
        [Test] public void ContentEquals_Note()
        {
            // Set up a Translator Note
            TranslatorNote tn1 = new TranslatorNote("so loved the world");
            tn1.Class = TranslatorNote.NoteClass.General;
            tn1.Messages.Append(
                new DMessage("John", new DateTime(2008, 11, 1), "David",
                    "Check exegesis here."));

            // Set up a duplicate
            TranslatorNote tn2 = new TranslatorNote("so loved the world");
            tn2.Class = TranslatorNote.NoteClass.General;
            tn2.Messages.Append(
                new DMessage("John", new DateTime(2008, 11, 1), "David",
                    "Check exegesis here."));

            // Equality
            Assert.IsTrue(tn1.ContentEquals(tn2));

            // Category differs
            tn2.Class = TranslatorNote.NoteClass.Exegetical;
            Assert.IsFalse(tn1.ContentEquals(tn2));
            tn2.Class = tn1.Class;

            // AssignedTo differs
            tn2.Status = "Sandra";
            Assert.IsFalse(tn1.ContentEquals(tn2));
            tn2.Status = tn1.Status;

            // Context differs
            tn2.SelectedText = "loved the world";
            Assert.IsFalse(tn1.ContentEquals(tn2));
            tn2.SelectedText = tn1.SelectedText;

            // Message differs
            tn2.Messages.Clear();
            Assert.IsFalse(tn1.ContentEquals(tn2));
        }
        #endregion

        // I/O
        #region Test: ImportNoteFromToolboxXml
        [Test] public void ImportNoteFromToolboxXml()
        {
            var vsToImport = new string[] {
                "<TranslatorNote Category=\"To Do\" AssignedTo=\"\" Context=\"mbambuninda\" Reference=\"001:005\" ShowInDaughter=\"false\">",
                    "<ownseq Name=\"Discussions\">",
                        "<Discussion Author=\"Mandowen\" Created=\"2009-04-29 02:32:42Z\"><ownseq Name=\"paras\"><DParagraph Abbrev=\"NoteDiscussion\" Contents=\"ratoe tumaimbe &quot;mbambuninda&quot;=yara gwaravainy=&quot;mbambunin indamu&quot; weramu tumainy ngkov.\"/></ownseq></Discussion>",
                        "<Discussion Author=\"Ibu Linda\" Created=\"2009-05-07 09:27:24Z\"><ownseq Name=\"paras\"><DParagraph Abbrev=\"NoteDiscussion\" Contents=\"Inanayanambe nyo raura kakavimbe indamu syo ranaun. Weti no kai,  weramu syare wamo raporar taiso: &quot;mbambunin dai&quot;.\"/></ownseq></Discussion>",
                        "<Discussion Author=\"Mandowen\" Created=\"2009-05-22 04:06:54Z\"><ownseq Name=\"paras\"><DParagraph Abbrev=\"NoteDiscussion\" Contents=\"wamo ratoe &quot;mbambunin da.&quot; yara vemo ratoe &quot;mbambunin dai&quot; wenora.\"/></ownseq></Discussion>",
                    "</ownseq>",
                "</TranslatorNote>"
            };
            var vsOxesExpected = new string[] {
                "<Annotation class=\"General\" selectedText=\"mbambuninda\">",
                    "<Message author=\"Mandowen\" created=\"2009-04-29 02:32:42Z\">ratoe tumaimbe &quot;mbambuninda&quot;=yara gwaravainy=&quot;mbambunin indamu&quot; weramu tumainy ngkov.</Message>",
                    "<Message author=\"Ibu Linda\" created=\"2009-05-07 09:27:24Z\">Inanayanambe nyo raura kakavimbe indamu syo ranaun. Weti no kai,  weramu syare wamo raporar taiso: &quot;mbambunin dai&quot;.</Message>",
                    "<Message author=\"Mandowen\" created=\"2009-05-22 04:06:54Z\">wamo ratoe &quot;mbambunin da.&quot; yara vemo ratoe &quot;mbambunin dai&quot; wenora.</Message>",
                "</Annotation>"
           };

            // Create an Oxes object for Expected
            var xmlOxesExpected = new XmlDoc(vsOxesExpected);

            // Import into a TranslatorNote object
            var xmlImported = new XmlDoc(vsToImport);
            var nodeNote = XmlDoc.FindNode(xmlImported, "TranslatorNote");
            var note = TranslatorNote.Create(nodeNote);

            // Create an new Oxes object for saving
            var xmlOxesActual = new XmlDoc();
            note.Save(xmlOxesActual, xmlOxesActual);

            // Are they the same?
            bool bIsSame = XmlDoc.Compare(xmlOxesExpected, xmlOxesActual);
            Assert.IsTrue(bIsSame, "Note xmls should be the same.");
        }
        #endregion
        #region Test: OxesIO
        [Test] public void OxesIO()
        {
            // Set up a Translator Note
            TranslatorNote tn = CreateTestTranslatorNote();

            // Do we get the XML save that we expect?
            var oxes = new XmlDoc();
            tn.Save(oxes, oxes);
            var nodeNote = XmlDoc.FindNode(oxes, TranslatorNote.c_sTagTranslatorNote);
            string sSaved = XmlDoc.OneLiner(nodeNote);
            Assert.AreEqual(
                "<Annotation class=\"General\" selectedText=\"so loved the world\">" +
                    "<Message author=\"John\" created=\"2008-11-01 00:00:00Z\" status=\"Sandra\">Check exegesis here.</Message>" +
                    "<Message author=\"Sandra\" created=\"2008-11-03 00:00:00Z\">Exegesis is fine.</Message>" +
                "</Annotation>",
                sSaved,
                "Does the XML save as we expect?");

            // Create a new Translator Note from this Oxes element; they should be identical
            var tnNew = TranslatorNote.Create(nodeNote);
            var oxesNew = new XmlDoc();
            tnNew.Save(oxesNew, oxesNew);
            var bIsSame = XmlDoc.Compare(oxes, oxesNew);
            Assert.IsTrue(bIsSame, "TranslatorNote xmls should be the same.");
        }
        #endregion
        #region Test: IO_Section
        [Test] public void IO_Section()
        {
            // Create the note we'll test
            TranslatorNote tn = CreateTestTranslatorNote();
            var oxes = new XmlDoc();
            var nodeNote = tn.Save(oxes, oxes);

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
            vs[vs.Length - 1] += XmlDoc.OneLiner(nodeNote);

            // Make sure we have something that looks like a note
            Assert.AreEqual("\\tn <Annot", vs[vs.Length-1].Substring(0, 10));

            // Do the test
            T_DSection t = new T_DSection();
            t.IO_TestEngine(vs, vs);
        }
        #endregion

        // Conversion from 1.0-styled notes
        #region Test: OldStyleNotesConversion_General
        [Test] public void OldStyleNotesConversion_General()
        {
            string sNoteText = "This is a general note.";
            SfField f = new SfField("nt", sNoteText);

            TranslatorNote tn = TranslatorNote.ImportFromOldStyle(2, 15, f);

            Assert.AreEqual(sNoteText, tn.Messages[0].SimpleText);
            Assert.AreEqual("Unknown Author", tn.Messages[0].Author);
            Assert.AreEqual(TranslatorNote.NoteClass.General, tn.Class);
            Assert.AreEqual("nt", tn.SfmMarker);
        }
        #endregion
        #region Test: OldStyleNotesConversion_ToDo
        [Test] public void OldStyleNotesConversion_ToDo()
        {
            string sNoteText = "This is a To Do note.";
            SfField f = new SfField("ntck", sNoteText);

            TranslatorNote tn = TranslatorNote.ImportFromOldStyle(2, 15, f);

            Assert.AreEqual(sNoteText, tn.Messages[0].SimpleText);
            Assert.AreEqual("Unknown Author", tn.Messages[0].Author);
            Assert.AreEqual(TranslatorNote.NoteClass.General, tn.Class);
            Assert.AreEqual("ntck", tn.SfmMarker);
        }
        #endregion

        // Merge
        #region Test: void MergeNote()
        [Test] public void MergeNote()
            // Tests that:
            // - Both Mine and Theirs created a new Message; tests that both were added.
            // - Both Mine and Theirs changed the AssignedTo, but since Mine was the
            //    most recent Message, the result should use Mine's AssignedTo.
        {
            string sParent = "<TranslatorNote class=\"General\" " +
                "Context=\"so loved the world\" Reference=\"003:016\" ShowInDaughter=\"false\">" +
                  "<Message Author=\"John\" Created=\"2008-11-01 00:00:00Z\">Check exegesis here.</Message>" +
                  "<Message Author=\"Sandra\" Created=\"2008-11-03 00:00:00Z\">Exegesis is fine.</Message>" +
                "</TranslatorNote>";
            string sMine = "<TranslatorNote class=\"General\" " +
                "Context=\"so loved the world\" Reference=\"003:016\" ShowInDaughter=\"false\">" +
                  "<Message Author=\"John\" Created=\"2008-11-01 00:00:00Z\">Check exegesis here.</Message>" +
                  "<Message Author=\"Sandra\" Created=\"2008-11-03 00:00:00Z\">Exegesis is fine.</Message>" +
                  "<Message Author=\"John\" Created=\"2008-11-15 00:00:00Z\">I'm not convinced.</Message>" +
                "</TranslatorNote>";
            string sTheirs = "<TranslatorNote class=\"General\" " +
                "Context=\"so loved the world\" Reference=\"003:016\" ShowInDaughter=\"false\">" +
                  "<Message Author=\"John\" Created=\"2008-11-01 00:00:00Z\">Check exegesis here.</Message>" +
                  "<Message Author=\"Sandra\" Created=\"2008-11-03 00:00:00Z\">Exegesis is fine.</Message>" +
                  "<Message Author=\"Sandra\" Created=\"2008-11-14 00:00:00Z\">Really, it is!</Message>" +
                "</TranslatorNote>";

            TranslatorNote Parent = CreateFromXmlString(sParent, "TranslatorNote");
            TranslatorNote Mine = CreateFromXmlString(sMine, "TranslatorNote");
            TranslatorNote Theirs = CreateFromXmlString(sTheirs, "TranslatorNote");

            Mine.Merge(Parent, Theirs);

            // Save the result; is it what we expect?
            var oxes = new XmlDoc();
            var nodeOut = Mine.Save(oxes, oxes);
            var sOut = XmlDoc.OneLiner(nodeOut);
            string sExpected = "<Annotation class=\"General\" selectedText=\"so loved the world\">" +
                  "<Message author=\"John\" created=\"2008-11-01 00:00:00Z\">Check exegesis here.</Message>" +
                  "<Message author=\"Sandra\" created=\"2008-11-03 00:00:00Z\">Exegesis is fine.</Message>" +
                  "<Message author=\"Sandra\" created=\"2008-11-14 00:00:00Z\">Really, it is!</Message>" +
                  "<Message author=\"John\" created=\"2008-11-15 00:00:00Z\">I'm not convinced.</Message>" +
                "</Annotation>";
            Assert.AreEqual(sExpected, sOut);
        }
        #endregion
    }
    #endregion
}
