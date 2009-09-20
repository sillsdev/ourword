/**********************************************************************************************
 * Project: OurWord! - Tests
 * File:    T_DTHistory.cs
 * Author:  John Wimbish
 * Created: 25 May 2009
 * Purpose: Tests the DHistory class
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
using JWdb.DataModel;

using OurWord;
using OurWord.Dialogs;
using OurWord.Layouts;
#endregion


namespace OurWordTests.DataModel
{
    [TestFixture] public class T_DEventMessage : TestCommon
    {
        DSection m_Section;
        #region Setup
        [SetUp]
        public void Setup()
        {
            JWU.NUnit_Setup();
            m_Section = CreateHierarchyThroughTargetSection("MRK");
        }
        #endregion

        // General
        #region Test: ContentEquals
        [Test]
        public void ContentEquals()
        {
            // Data 
            string sStage = "Revision";
            DateTime dtEventDate = new DateTime(2008, 11, 22);

            // Create a base Message object
            var message = new DEventMessage();
            message.Author = "John";
            message.Created = new DateTime(2008, 11, 23);
            message.Status = "David";
            message.SimpleText = "Revisi kadua by Yuli deng Yohanis berdasarkan masukan dari Ibu Jackline.";
            message.Stage = sStage;
            message.EventDate = dtEventDate;

            // Test equality
            Assert.IsTrue(message.ContentEquals(message));

            // Test if Stage gets changed
            DEventMessage m2 = message.Clone() as DEventMessage;
            m2.Stage = "Draft";
            Assert.IsFalse(message.ContentEquals(m2));

            // Test if EventDate gets changed
            m2 = message.Clone() as DEventMessage;
            m2.EventDate = new DateTime(2007, 5, 3);
            Assert.IsFalse(message.ContentEquals(m2));
        }
        #endregion

        // I/O
        #region Test: ImportFromToolboxXml
        [Test] public void ImportFromToolboxXml()
        {
            var vsToImport = new string[] {
                "<DEvent Created=\"2009-06-03 11:45:29Z\" Date=\"2006-05-08 00:00:00Z\" Stage=\"Revisi\">",
                    "Revisi kadua by Yuli deng Yohanis berdasarkan masukan dari Ibu Jackline.",
                "</DEvent>"
            };
            var vsOxesExpected = new string[] {
                "<Event created=\"2009-06-03 11:45:29Z\" when=\"2006-05-08 00:00:00Z\" stage=\"Revisi\">",
                    "Revisi kadua by Yuli deng Yohanis berdasarkan masukan dari Ibu Jackline.",
                "</Event>"
            };

            // Create an Oxes object for Expected
            var xmlOxesExpected = new XmlDoc(vsOxesExpected);

            // Import into a EventMessage object
            var xmlImported = new XmlDoc(vsToImport);
            var nodeMessage = XmlDoc.FindNode(xmlImported, "DEvent");
            var message = new DEventMessage(nodeMessage);

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
                "<Event author=\"John\" created=\"2008-11-23 00:00:00Z\" when=\"2008-11-21 00:00:00Z\" stage=\"Revision\">" ,
                    "Revisi kadua by Yuli deng Yohanis berdasarkan masukan dari Ibu Jackline.",
                "</Event>"
            };

            // Create the XmlDoc
            var xmlOxesExpected = new XmlDoc(vsOxesExpected);
            var nodeMessage = XmlDoc.FindNode(xmlOxesExpected, DEventMessage.c_sTagEventMessage);
            //xmlOxesExpected.WriteToConsole("Expected");

            // Create the Message object from the Xml node
            var message = new DEventMessage(nodeMessage);

            // Save this new Message to oxes
            var xmlOxesActual = new XmlDoc();
            message.Save(xmlOxesActual, xmlOxesActual);

            // Are they the same?
            bool bIsSame = XmlDoc.Compare(xmlOxesExpected, xmlOxesActual);
            Assert.IsTrue(bIsSame, "Message xmls should be the same.");
        }
        #endregion

    }

    [TestFixture] public class T_DHistory
    {
        #region Setup
        [SetUp] public void Setup()
        {
            JWU.NUnit_Setup();
            DB.Project = new DProject();
            DB.Project.TeamSettings = new DTeamSettings();
            DB.TeamSettings.EnsureInitialized();
            DB.Project.DisplayName = "Test Project";
        }
        #endregion

        #region Test: EventIO
        [Test] public void EventIO()
        {
            // Create an event
            DEventMessage e = new DEventMessage();
            e.Author = "John";
            e.EventDate = new DateTime(2009, 5, 25);
            e.Stage = "Draft";
            e.SimpleText = "Drafted by John";
            e.Created = new DateTime(2000, 1, 1);

            // Get its xml representation
            var oxes = new XmlDoc();
            var node = e.Save(oxes, oxes);
            string s = XmlDoc.OneLiner(node);

            Assert.AreEqual(
                "<Event author=\"John\" created=\"2000-01-01 00:00:00Z\" when=\"2009-05-25 00:00:00Z\" stage=\"Draft\">Drafted by John</Event>", 
                s);

            // Create a new event and  populate it from the xml
            var eNew = new DEventMessage(node);           

            // Should be identical
            Assert.IsTrue(eNew.ContentEquals(e), "Should be identical");
        }
        #endregion
        #region Test: ContentEquals
        [Test] public void ContentEquals()
        {
            var e1 = new DEventMessage();
            e1.EventDate = new DateTime(2009, 5, 25);
            e1.Created = e1.EventDate;
            e1.Stage = "Draft";
            e1.SimpleText = "Drafted by John";
            e1.Author = "John";

            var e2 = new DEventMessage();
            e2.EventDate = new DateTime(2009, 5, 25);
            e2.Created = e2.EventDate;
            e2.Stage = "Draft";
            e2.SimpleText = "Drafted by John";
            e2.Author = "John";

            // Should be identical
            Assert.IsTrue(e2.ContentEquals(e1), "Should be identical");

            // Change the date
            e2.EventDate = new DateTime(1999, 5, 25);
            Assert.IsFalse(e2.ContentEquals(e1), "Dates differ");

            // Change the Stage
            e2.EventDate = new DateTime(2009, 5, 25);
            e2.Stage = "Revision";
            Assert.IsFalse(e2.ContentEquals(e1), "Stages differ");

            // Change the paragraph
            e2.Stage = "Draft";
            e2.SimpleText = "Drafted by Sandra";
            Assert.IsFalse(e2.ContentEquals(e1), "Descriptions differ");
        }
        #endregion
        #region Test: EventsAreSorted
        [Test] public void EventsAreSorted()
        {
            // Add events, but out of date order
            DHistory history = new DHistory();
            history.AddEvent(new DateTime(2005, 3, 8), "Draft", "Drafted by John");
            history.AddEvent(new DateTime(2007, 11, 23), "Trial", "Taken to Sosol by John");
            history.AddEvent(new DateTime(2006, 8, 23), "Team Check", "Checked by John, Sandra");

            // Years should now be in order
            Assert.AreEqual(2005, history.Events[0].Created.Year);
            Assert.AreEqual(2006, history.Events[1].Created.Year);
            Assert.AreEqual(2007, history.Events[2].Created.Year);
        }
        #endregion
        #region Test: HistorySfmIO
        [Test] public void HistorySfmIO()
        {
            // Three create dates
            DateTime d1 = new DateTime(2000, 1, 1);
            DateTime d2 = new DateTime(2000, 1, 2);
            DateTime d3 = new DateTime(2000, 1, 3);

            // Build a 3-event history
            DHistory history = new DHistory();
            history.AddEvent(new DateTime(2005, 3, 8), "Draft", "Drafted by John").Created = d1;
            history.AddEvent(new DateTime(2006, 8, 23), "Team Check", "Checked by John, Sandra").Created = d2;
            history.AddEvent(new DateTime(2007, 11, 23), "Trial", "Taken to Sosol by John").Created = d3;
            foreach (DEventMessage e in history.Events)
                e.Author = "John";

            // Save to an SfField and see that it is ok
            var oxes = new XmlDoc();
            var node = history.Save(oxes, oxes);
            var f = new SfField(DHistory.c_sTag, XmlDoc.OneLiner(node));

            Assert.AreEqual("History", f.Mkr);
            Assert.AreEqual(
                "<History>" + 
                "<Event author=\"John\" created=\"2000-01-01 00:00:00Z\" when=\"2005-03-08 00:00:00Z\" stage=\"Draft\">Drafted by John</Event>" +
                "<Event author=\"John\" created=\"2000-01-02 00:00:00Z\" when=\"2006-08-23 00:00:00Z\" stage=\"Team Check\">Checked by John, Sandra</Event>" +
                "<Event author=\"John\" created=\"2000-01-03 00:00:00Z\" when=\"2007-11-23 00:00:00Z\" stage=\"Trial\">Taken to Sosol by John</Event>" +
                "</History>",
                f.Data);

            // Now bring it back in and see what we have
            DHistory historyNew = new DHistory();
            historyNew.Read(f);

            Assert.AreEqual(3, historyNew.Events.Count);
            Assert.AreEqual(2005, historyNew.Events[0].EventDate.Year);
            Assert.AreEqual("Team Check", historyNew.Events[1].Stage);
            Assert.AreEqual("Taken to Sosol by John", historyNew.Events[2].SimpleText);
        }
        #endregion

        #region Test: Merge
        [Test] public void Merge()
        {
            // Build a 3-event parent
            DHistory Parent = new DHistory();
            Parent.AddEvent(new DateTime(2005, 3, 8), "Draft", "Drafted by John").Created = new DateTime(2000, 1, 1);
            Parent.AddEvent(new DateTime(2006, 8, 23), "Team Check", "Checked by Sandra").Created = new DateTime(2000, 1, 2);
            Parent.AddEvent(new DateTime(2007, 11, 23), "Trial", "Tested by John").Created = new DateTime(2000, 1, 3);
            foreach (DEventMessage e in Parent.Events)
                e.Author = "John";

            // Clone to Ours, Theirs
            DHistory Ours = Parent.Clone();
            DHistory Theirs = Parent.Clone();

            // Add an event to Theirs
            Theirs.AddEvent(new DateTime(2008, 1, 1), "Consultant", "Consultant checked by Marge").Created = new DateTime(2000, 1, 4);

            // Change an event in Ours
            Ours.Events[1].SimpleText = "Checked by Sandra and John";

            // Change the same event in both....We expect Ours to win
            Ours.Events[0].Stage = "Konsul";
            Theirs.Events[0].Stage = "Draf";

            // Do the merge
            Ours.Merge(Parent, Theirs);
            // Not testing authors, and the system, left to itself, automatically asigns it to
            // DB.UserName, so we just override it here so that the test will work.
            foreach (DEventMessage e in Ours.Events)
                e.Author = "John";

            // Check the result
            var oxes = new XmlDoc();
            var node = Ours.Save(oxes, oxes);
            var f = new SfField(DHistory.c_sTag, XmlDoc.OneLiner(node));
            Assert.AreEqual(
                "<History>" +
                "<Event author=\"John\" created=\"2000-01-01 00:00:00Z\" when=\"2005-03-08 00:00:00Z\" stage=\"Konsul\">Drafted by John</Event>" +
                "<Event author=\"John\" created=\"2000-01-02 00:00:00Z\" when=\"2006-08-23 00:00:00Z\" stage=\"Team Check\">Checked by Sandra and John</Event>" +
                "<Event author=\"John\" created=\"2000-01-03 00:00:00Z\" when=\"2007-11-23 00:00:00Z\" stage=\"Trial\">Tested by John</Event>" +
                "<Event author=\"John\" created=\"2000-01-04 00:00:00Z\" when=\"2008-01-01 00:00:00Z\" stage=\"Consultant\">Consultant checked by Marge</Event>" +
                "</History>",
                f.Data);
        }
        #endregion

    }
}
