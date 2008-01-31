/**********************************************************************************************
 * Project: Our Word!
 * File:    PathConverter.cs
 * Author:  John Wimbish
 * Created: 10 Sep 2007
 * Purpose: Handle conversions between Relative and Absolute path names
 * Legal:   Copyright (c) 2005-07, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
#region Header: Using, etc.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using NUnit.Framework;
#endregion

namespace JWTools
{
    public class PathConverter
    {
        #region SMethod: string[] TokenizePath(string sPath)
        public static string[] TokenizePath(string sPath, bool bKeepFilename)
        {
            if (string.IsNullOrEmpty(sPath))
                return null;

            // Temporary place to hold the path components
            ArrayList a = new ArrayList();

            // Break the path down into its parts
            while (sPath.Length > 0)
            {
                int i = sPath.IndexOf(Path.DirectorySeparatorChar);

                if (-1 == i)
                {
                    a.Add(sPath);
                    sPath = "";
                }
                else
                {
                    a.Add( sPath.Substring(0, i) );
                    sPath = sPath.Substring(i + 1);
                }
            }

            // Remove the filename
            if (!bKeepFilename && a.Count > 0)
                a.RemoveAt(a.Count - 1);

            // Convert to a string vector and return the result
            string[] v = new string[a.Count];
            for (int i = 0; i < a.Count; ++i)
                v[i] = (string)a[i];
            return v;
        }
        #endregion
        #region SMethod: string RelativeToAbsolute(string sOriginPath, string sRelativePath)
        static public string RelativeToAbsolute(string sOriginPath, string sRelativePath)
        {
            // We'll store the result here
            string sResultPath = "";

            // Error checking
            if (string.IsNullOrEmpty(sOriginPath) || string.IsNullOrEmpty(sRelativePath))
                return sResultPath;

            // Get the Origin Path components, minus the filename itself
            string[] vOriginPath = TokenizePath(sOriginPath, false);

            // Get all of the Relative Path components
            string[] vRelativePath = TokenizePath(sRelativePath, true);

            // Start with the assumption that we'll use the entire origin path
            int iOriginEnd = vOriginPath.Length;
            int iRelBegin = 0;

            // Back out origin components, to correspond with ".." in the ObjectPath name.
            while (iRelBegin < vRelativePath.Length && vRelativePath[iRelBegin] == "..")
            {
                ++iRelBegin;
                --iOriginEnd;
                if (iOriginEnd < 0)
                    return sResultPath;
            }

            // Build a string with the origin components
            for(int k=0; k<iOriginEnd; k++)
                sResultPath += (vOriginPath[k] + Path.DirectorySeparatorChar);

            // Append the Path Components
            while (iRelBegin < vRelativePath.Length)
            {
                sResultPath += vRelativePath[iRelBegin];
                if (iRelBegin < vRelativePath.Length - 1)
                    sResultPath += Path.DirectorySeparatorChar;
                iRelBegin++;
            }

            return sResultPath;
        }
        #endregion
        #region SMethod: string AbsoluteToRelative(string sOriginPath, string sAbsolutePath)
        static public string AbsoluteToRelative(string sOriginPath, string sAbsolutePath)
        {
            // We'll store the result here
            string sResultPath = "";

            // Valid Parameters?
            if (string.IsNullOrEmpty(sOriginPath) || string.IsNullOrEmpty(sAbsolutePath))
                return sResultPath;

            // Get the Origin Path components, minus the filename itself
            string[] vOriginPath = TokenizePath(sOriginPath, false);
            if (null == vOriginPath)
                return sResultPath;

            // Get all of the Absolute Path components
            string[] vAbsolutePath = TokenizePath(sAbsolutePath, true);
            if (null == vAbsolutePath)
                return sResultPath;

            // Discard all components that are in common
            int iCommon = 0;
            while (vOriginPath.Length > iCommon &&
                vAbsolutePath.Length > iCommon &&
                vOriginPath[iCommon] == vAbsolutePath[iCommon])
            {
                iCommon++;
            }

            // Add ".."s for every remaining component in the Origin
            for (int i = iCommon; i < vOriginPath.Length; i++)
                sResultPath += (".." + Path.DirectorySeparatorChar);

            // Add the Absolute components
            for (int i = iCommon; i < vAbsolutePath.Length; i++)
            {
                sResultPath += vAbsolutePath[i];
                if (i < vAbsolutePath.Length - 1)
                    sResultPath += Path.DirectorySeparatorChar;
            }

            return sResultPath;
        }
        #endregion
    }

    #region TEST
    // NUnit Tests
    [TestFixture] public class Test_PathConverter
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
                "C:\\Documents and Settings\\Ti'utüvame\\Mis documentos\\OurWord-Huichol\\Parámetros\\Huichol.owp"
            };
            string[] vAbsolute = {
                "C:\\Users\\JWimbish\\Desktop\\TEST\\Ama-MKR.db",
                "C:\\Users\\JWimbish\\Ama-MKR.db",
                "C:\\Users\\JWimbish\\Here\\Tis\\Ama-MKR.db",
                "C:\\CCC-Lg\\Kupang\\Other\\My Kupang Settings\\Kupang.otrans",
                "C:\\Documents and Settings\\Ti'utüvame\\Mis documentos\\OurWord-Huichol\\Parámetros\\Team Settings.owt"
            };
            string[] vRelative = {
                "Ama-MKR.db",
                "..\\..\\Ama-MKR.db",
                "..\\..\\Here\\Tis\\Ama-MKR.db",
                "..\\..\\..\\..\\Kupang\\Other\\My Kupang Settings\\Kupang.otrans",
                "Team Settings.owt"
            };

            for(int i=0; i<vOrigin.Length; i++)
            {
                Assert.AreEqual( 
                    vAbsolute[i],
                    PathConverter.RelativeToAbsolute(vOrigin[i], vRelative[i]), 
                    "1:" + i.ToString() );

                Assert.AreEqual( 
                    vRelative[i],
                    PathConverter.AbsoluteToRelative(vOrigin[i], vAbsolute[i]), 
                    "2:" + i.ToString());

//                Console.WriteLine(i.ToString() + ": " + vRelative[i]);
            }
        }
        #endregion
    }
    #endregion

}
