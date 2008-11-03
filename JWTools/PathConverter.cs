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
			
			sPath = ConvertDirectorySeparators(sPath);
			char[] separators = new char[] {Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar};
			string[] result = sPath.Split(separators);
			if (result.Length > 0 && (!bKeepFilename || result[result.Length - 1] == ""))
				Array.Resize(ref result, result.Length - 1);
			return result;

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
        #region SMethod: string ConvertDirectorySeparators(string sPath)
        static public string ConvertDirectorySeparators(string sPath)
		{
			if (sPath == null)
				return null;
			if (Environment.OSVersion.Platform == PlatformID.Unix)
				return sPath.Replace('\\', Path.DirectorySeparatorChar);
			return sPath.Replace('/', Path.DirectorySeparatorChar);
		}
		#endregion
    }

}
