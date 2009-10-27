#region ***** T_DBook.cs *****
/**********************************************************************************************
 * Project: OurWord! - Tests
 * File:    T_Stage.cs
 * Author:  John Wimbish
 * Created: 17 Oct 2009
 * Purpose: Tests the Stage class
 * Legal:   Copyright (c) 2004-09, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
#region Using
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;

using NUnit.Framework;

using JWTools;
using OurWordData;
using OurWordData.DataModel;

using OurWord;
using OurWord.Dialogs;
using OurWord.Layouts;
#endregion
#endregion

namespace OurWordTests.DataModel
{
    [TestFixture] public class T_Stage : TestCommon
    {
        #region Method: void Setup()
        [SetUp] public void Setup()
        {
            SetupTest();
        }
        #endregion

        #region Test: StagesIO
        [Test] public void StagesIO()
        {
            // Create the stages list
            var stages = new StageList();

            // See if the save string is what we expect
            Assert.AreEqual("+Draft +Team +Adv +Comm +BT +Consult +Final +Rev", 
                stages.ToSaveString());

            // Change the order and Use stages of several stages
            stages.MoveUp(stages.Find("Adv"));
            stages.MoveUp(stages.Find("Final"));
            stages.Find("Adv").UsedInThisProject = false;
            stages.Find("Rev").UsedInThisProject = false;
            string sSave = stages.ToSaveString();
            Assert.AreEqual("+Draft -Adv +Team +Comm +BT +Final +Consult -Rev", sSave);

            // Create a new list (which will have the factory default); read in our
            // save string and see if things get reordered.
            var stages2 = new StageList();
            stages2.FromSaveString(sSave);

            Assert.AreEqual(true, stages2.Find("Draft").UsedInThisProject);
            Assert.AreEqual(false, stages2.Find("Adv").UsedInThisProject);
            Assert.AreEqual(true, stages2.Find("Team").UsedInThisProject);
            Assert.AreEqual(true, stages2.Find("Comm").UsedInThisProject);
            Assert.AreEqual(true, stages2.Find("BT").UsedInThisProject);
            Assert.AreEqual(true, stages2.Find("Final").UsedInThisProject);
            Assert.AreEqual(true, stages2.Find("Consult").UsedInThisProject);
            Assert.AreEqual(false, stages2.Find("Rev").UsedInThisProject);

            Assert.AreEqual("Draft", stages2[0].EnglishAbbrev);
            Assert.AreEqual("Adv", stages2[1].EnglishAbbrev);
            Assert.AreEqual("Team", stages2[2].EnglishAbbrev);
            Assert.AreEqual("Comm", stages2[3].EnglishAbbrev);
            Assert.AreEqual("BT", stages2[4].EnglishAbbrev);
            Assert.AreEqual("Final", stages2[5].EnglishAbbrev);
            Assert.AreEqual("Consult", stages2[6].EnglishAbbrev);
            Assert.AreEqual("Rev", stages2[7].EnglishAbbrev);
        }
        #endregion

        #region Test: MoveUp
        [Test] public void MoveUp()
        {
            // Create a stages list, which we expect to have factory defaults
            var stages = new StageList();
            Assert.AreEqual("+Draft +Team +Adv +Comm +BT +Consult +Final +Rev",
                stages.ToSaveString());

            // We'll be moving the Adv stage up
            var stage = stages.Find("Adv");
            Assert.IsNotNull(stage);

            // Move "Adv" up until it will not move any more
            stages.MoveUp(stage);
            Assert.AreEqual("+Draft +Adv +Team +Comm +BT +Consult +Final +Rev",
                stages.ToSaveString());

            stages.MoveUp(stage);
            Assert.AreEqual("+Adv +Draft +Team +Comm +BT +Consult +Final +Rev",
                stages.ToSaveString());

            // This one should not move anything
            stages.MoveUp(stage);
            Assert.AreEqual("+Adv +Draft +Team +Comm +BT +Consult +Final +Rev",
                stages.ToSaveString());
        }
        #endregion
        #region Test: MoveDown
        [Test] public void MoveDown()
        {
            // Create a stages list, which we expect to have factory defaults
            var stages = new StageList();
            Assert.AreEqual("+Draft +Team +Adv +Comm +BT +Consult +Final +Rev",
                stages.ToSaveString());

            // We'll be moving the Consult stage doen
            var stage = stages.Find("Consult");
            Assert.IsNotNull(stage);

            // Move "Adv" up until it will not move any more
            stages.MoveDown(stage);
            Assert.AreEqual("+Draft +Team +Adv +Comm +BT +Final +Consult +Rev",
                stages.ToSaveString());

            stages.MoveDown(stage);
            Assert.AreEqual("+Draft +Team +Adv +Comm +BT +Final +Rev +Consult",
                stages.ToSaveString());

            // This one should not move anything
            stages.MoveDown(stage);
            Assert.AreEqual("+Draft +Team +Adv +Comm +BT +Final +Rev +Consult",
                stages.ToSaveString());
        }
        #endregion

        #region Test: FindByAbbrev
        [Test] public void FindByAbbrev()
        {
            // Create a stages list, which we expect to have factory defaults
            var stages = new StageList();
            Assert.AreEqual("+Draft +Team +Adv +Comm +BT +Consult +Final +Rev",
                stages.ToSaveString());

            // Should find ones that are in it; check at boundaries as well as middle
            Assert.IsNotNull(stages.Find("Draft"));
            Assert.IsNotNull(stages.Find("BT"));
            Assert.IsNotNull(stages.Find("Rev"));

            // Should not find things that aren't in it
            Assert.IsNull(stages.Find("jsw"));
        }
        #endregion
        #region Test: FindByID
        [Test] public void FindByID()
        {
            // Create a stages list, which we expect to have factory defaults
            var stages = new StageList();
            Assert.AreEqual("+Draft +Team +Adv +Comm +BT +Consult +Final +Rev",
                stages.ToSaveString());

            // Should find ones that are in it; check at boundaries as well as middle
            Assert.IsNotNull(stages.Find(Stage.c_idDraft));
            Assert.IsNotNull(stages.Find(Stage.c_idBackTranslation));
            Assert.IsNotNull(stages.Find(Stage.c_idFinalRevision));

            // Should not find things that aren't in it
            Assert.IsNull(stages.Find(42));
        }
        #endregion
    }
}
