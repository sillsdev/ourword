/**********************************************************************************************
 * Project: OurWord! - Tests
 * File:    T_Localization.cs
 * Author:  John Wimbish
 * Created: 05 Mar 2008
 * Purpose: Tests the Localization classes
 * Legal:   Copyright (c) 2004-09, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
#region Using
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

using NUnit.Framework;

using JWTools;
using OurWordData;

using OurWord;
using OurWordData.DataModel;
using OurWord.Dialogs;
using OurWord.Edit;
using OurWord.Layouts;
#endregion

namespace OurWordTests.JWTools
{
    [TestFixture] public class T_Localization
    {
        #region Method: void Setup()
        [SetUp]
        public void Setup()
        {
            TestCommon.GlobalTestSetup();
        }
        #endregion

    }
}
