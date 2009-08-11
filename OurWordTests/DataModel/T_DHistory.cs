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
            DEvent e = new DEvent();
            e.Date = new DateTime(2009, 5, 25);
            e.Stage = "Draft";
            e.Description.SimpleText = "Drafted by John";
            e.DateCreated = new DateTime(2000, 1, 1);

            // Get its xml representation
            XElement x = e.ToXml(true);
            Assert.AreEqual(
                "<DEvent Created=\"2000-01-01 00:00:00Z\" Date=\"2009-05-25 00:00:00Z\" Stage=\"Draft\">Drafted by John</DEvent>", 
                x.OneLiner);

            // Create a new event and  populate it from the xml
            DEvent eNew = new DEvent();
            eNew.FromXml(x);

            // Should be identical
            Assert.IsTrue(eNew.ContentEquals(e), "Should be identical");
        }
        #endregion
        #region Test: ContentEquals
        [Test] public void ContentEquals()
        {
            DEvent e1 = new DEvent();
            e1.Date = new DateTime(2009, 5, 25);
            e1.Stage = "Draft";
            e1.Description.SimpleText = "Drafted by John";

            DEvent e2 = new DEvent();
            e2.Date = new DateTime(2009, 5, 25);
            e2.Stage = "Draft";
            e2.Description.SimpleText = "Drafted by John";

            // Should be identical
            Assert.IsTrue(e2.ContentEquals(e1), "Should be identical");

            // Change the date
            e2.Date = new DateTime(1999, 5, 25);
            Assert.IsFalse(e2.ContentEquals(e1), "Dates differ");

            // Change the Stage
            e2.Date = new DateTime(2009, 5, 25);
            e2.Stage = "Revision";
            Assert.IsFalse(e2.ContentEquals(e1), "Stages differ");

            // Change the paragraph
            e2.Stage = "Draft";
            e2.Description.SimpleText = "Drafted by Sandra";
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
            Assert.AreEqual(2005, history.Events[0].Date.Year);
            Assert.AreEqual(2006, history.Events[1].Date.Year);
            Assert.AreEqual(2007, history.Events[2].Date.Year);
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
            history.AddEvent(new DateTime(2005, 3, 8), "Draft", "Drafted by John").DateCreated = d1;
            history.AddEvent(new DateTime(2006, 8, 23), "Team Check", "Checked by John, Sandra").DateCreated = d2;
            history.AddEvent(new DateTime(2007, 11,23), "Trial", "Taken to Sosol by John").DateCreated = d3;

            // Save to an SfField and see that it is ok
            SfField f = history.ToSfm();

            Assert.AreEqual("History", f.Mkr);
            Assert.AreEqual(
                "<DEvent Created=\"2000-01-01 00:00:00Z\" Date=\"2005-03-08 00:00:00Z\" Stage=\"Draft\">Drafted by John</DEvent>" +
                "<DEvent Created=\"2000-01-02 00:00:00Z\" Date=\"2006-08-23 00:00:00Z\" Stage=\"Team Check\">Checked by John, Sandra</DEvent>" +
                "<DEvent Created=\"2000-01-03 00:00:00Z\" Date=\"2007-11-23 00:00:00Z\" Stage=\"Trial\">Taken to Sosol by John</DEvent>", 
                f.Data);

            // Now bring it back in and see what we have
            DHistory historyNew = new DHistory();
            historyNew.FromSfm(f);

            Assert.AreEqual(3, historyNew.Events.Count);
            Assert.AreEqual(2005, historyNew.Events[0].Date.Year);
            Assert.AreEqual("Team Check", historyNew.Events[1].Stage);
            Assert.AreEqual("Taken to Sosol by John", historyNew.Events[2].Description.SimpleText);
        }
        #endregion

        #region Test: Merge
        [Test] public void Merge()
        {
            // Build a 3-event parent
            DHistory Parent = new DHistory();
            Parent.AddEvent(new DateTime(2005, 3, 8), "Draft", "Drafted by John").DateCreated = new DateTime(2000, 1, 1);
            Parent.AddEvent(new DateTime(2006, 8, 23), "Team Check", "Checked by Sandra").DateCreated = new DateTime(2000, 1, 2);
            Parent.AddEvent(new DateTime(2007, 11, 23), "Trial", "Tested by John").DateCreated = new DateTime(2000, 1, 3);

            // Clone to Ours, Theirs
            DHistory Ours = Parent.Clone();
            DHistory Theirs = Parent.Clone();

            // Add an event to Theirs
            Theirs.AddEvent(new DateTime(2008, 1, 1), "Consultant", "Consultant checked by Marge").DateCreated = new DateTime(2000, 1, 4);

            // Change an event in Ours
            Ours.Events[1].Description.SimpleText = "Checked by Sandra and John";

            // Change the same event in both....Ours wins for Stage
            Ours.Events[0].Stage = "Konsul";
            Theirs.Events[0].Stage = "Draf";
            // Special merge text for Descriptions
            Ours.Events[0].Description.SimpleText = "John";
            Theirs.Events[0].Description.SimpleText = "Sandra";
            string sConflictExpected = "John -From Merge: Sandra";

            // Do the merge
            Ours.Merge(Parent, Theirs);

            // Check the result
            SfField f = Ours.ToSfm();
            Assert.AreEqual(
                "<DEvent Created=\"2000-01-01 00:00:00Z\" Date=\"2005-03-08 00:00:00Z\" Stage=\"Konsul\">" + sConflictExpected + "</DEvent>" +
                "<DEvent Created=\"2000-01-02 00:00:00Z\" Date=\"2006-08-23 00:00:00Z\" Stage=\"Team Check\">Checked by Sandra and John</DEvent>" +
                "<DEvent Created=\"2000-01-03 00:00:00Z\" Date=\"2007-11-23 00:00:00Z\" Stage=\"Trial\">Tested by John</DEvent>" +
                "<DEvent Created=\"2000-01-04 00:00:00Z\" Date=\"2008-01-01 00:00:00Z\" Stage=\"Consultant\">Consultant checked by Marge</DEvent>",
                f.Data);
        }
        #endregion

    }
}
