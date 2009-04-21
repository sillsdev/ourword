/**********************************************************************************************
 * Project: OurWord! - Tests
 * File:    T_PathConverted.cs
 * Author:  John Wimbish
 * Created: 05 Mar 2008
 * Purpose: Tests the PathConverted class
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
using JWdb;

using OurWord;
using JWdb.DataModel;
using OurWord.Dialogs;
using OurWord.Edit;
using OurWord.View;
#endregion

namespace OurWordTests.JWTools
{
    [TestFixture] public class T_PathConverter
    {
        #region TEST: TokenizePath
        [Test] public void TokenizePath()
        {
            string[] v = PathConverter.TokenizePath("C:\\Users\\JWimbish\\Desktop\\TEST\\Ama-MRK-Draft-B 2007-07-26.db",
                true);
            Assert.AreEqual(6, v.Length);
            Assert.AreEqual("C:", v[0]);
            Assert.AreEqual("Users", v[1]);
            Assert.AreEqual("JWimbish", v[2]);
            Assert.AreEqual("Desktop", v[3]);
            Assert.AreEqual("TEST", v[4]);
            Assert.AreEqual("Ama-MRK-Draft-B 2007-07-26.db", v[5]);

            v = PathConverter.TokenizePath("..\\TEST\\Ama.db", true);
            Assert.AreEqual(3, v.Length);
            Assert.AreEqual("..", v[0]);
            Assert.AreEqual("TEST", v[1]);
            Assert.AreEqual("Ama.db", v[2]);

            v = PathConverter.TokenizePath("..\\TEST\\Ama.db", false);
            Assert.AreEqual(2, v.Length);
            Assert.AreEqual("..", v[0]);
            Assert.AreEqual("TEST", v[1]);
        }
        #endregion
        #region TEST: Conversions
        [Test] public void Conversions()
        {
            string[] vOrigin = {
                "C:\\Users\\JWimbish\\Desktop\\TEST\\Amarasi.otrans",
                "C:\\Users\\JWimbish\\Desktop\\TEST\\Amarasi.otrans",
                "C:\\Users\\JWimbish\\Desktop\\TEST\\Amarasi.otrans",
                "C:\\CCC-Lg\\Rote-Ndao\\Dela\\Other\\My Dela Settings\\Dela.owp",
                "C:\\Documents and Settings\\Ti'ut�vame\\Mis documentos\\OurWord-Huichol\\Par�metros\\Huichol.owp"
            };
            string[] vAbsolute = {
                "C:\\Users\\JWimbish\\Desktop\\TEST\\Ama-MKR.db",
                "C:\\Users\\JWimbish\\Ama-MKR.db",
                "C:\\Users\\JWimbish\\Here\\Tis\\Ama-MKR.db",
                "C:\\CCC-Lg\\Kupang\\Other\\My Kupang Settings\\Kupang.otrans",
                "C:\\Documents and Settings\\Ti'ut�vame\\Mis documentos\\OurWord-Huichol\\Par�metros\\Team Settings.owt"
            };
            string[] vRelative = {
                "Ama-MKR.db",
                "..\\..\\Ama-MKR.db",
                "..\\..\\Here\\Tis\\Ama-MKR.db",
                "..\\..\\..\\..\\Kupang\\Other\\My Kupang Settings\\Kupang.otrans",
                "Team Settings.owt"
            };

			
            for (int i = 0; i < vOrigin.Length; i++)
            {
				vRelative[i] = PathConverter.ConvertDirectorySeparators(vRelative[i]);
				vAbsolute[i] = PathConverter.ConvertDirectorySeparators(vAbsolute[i]);

                Assert.AreEqual(
                    vAbsolute[i],
                    PathConverter.RelativeToAbsolute(vOrigin[i], vRelative[i]),
                    "1:" + i.ToString());

                Assert.AreEqual(
                    vRelative[i],
                    PathConverter.AbsoluteToRelative(vOrigin[i], vAbsolute[i]),
                    "2:" + i.ToString());

                //                Console.WriteLine(i.ToString() + ": " + vRelative[i]);
            }
        }
        #endregion
    }
}
