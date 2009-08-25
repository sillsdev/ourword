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


namespace OurWordTests.JWTools
{
    [TestFixture]
    public class T_XmlDoc
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

            for (int i = 0; i < vN.Length; i++)
            {
                // Convert the integer to the string ID
                string s = XmlDoc.IntToID( vN[i] );
                Assert.AreEqual(vS[i], s, 
                    "N=" + vN[i].ToString() + " --should convert to-- S=\"" +vS[i] + "\"");

                // Convert the resultant string ID back to the original integer
                int n = XmlDoc.IdToInt(vS[i]);
                Assert.AreEqual(vN[i], n, 
                    "S=\"" + vS[i] + "\" --should convert to-- N=" + vN[i].ToString());
            }
        }
        #endregion



    }
}
