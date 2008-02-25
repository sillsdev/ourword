/**********************************************************************************************
 * Dll:     JWTools
 * File:    JW_Utilities.cs
 * Author:  John Wimbish
 * Created: 25 Sep 2005
 * Purpose: Utilities - common functions that don't really need to be part of a data instance
 * Legal:   Copyright (c) 2005-07, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
#region Using
using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.IO;
using Microsoft.Win32;
#endregion

namespace JWTools
{
	public class JWU
	{
		// Given a pathname, removes letters from the middle so that there is a max
		// of cMaxChars letters. Thus:
		//   C:\Documents and Settings\John\My Documents\Projects\JWTools\MyFile.db
		// becomes
		//   C:\Documents and Settings\John\My...\MyFile.db
		#region Method: static public PathEllipses(string sPath, int cMaxChars)
		static public string PathEllipses(string sPath, int cMaxLen)
		{
			if (sPath.Length <= cMaxLen)
				return sPath;

			if (cMaxLen <= 2)
				return "...";

			string sBaseName = Path.GetFileName(sPath);
			string sDirectory = Path.GetDirectoryName(sPath);

			if (sBaseName.Length > cMaxLen)
				return sBaseName.Substring(0, cMaxLen - 2) + "...";

			int nLength = Math.Max(0, cMaxLen - 2 - sBaseName.Length);

			return sDirectory.Substring(0, nLength) + "...\\" + sBaseName;
		}
		#endregion

		// Given an ArrayList that contains int's, converts to an int[]
		#region Method: static int[] ArrayListToIntArray(ArrayList list)
		static public int[] ArrayListToIntArray(ArrayList list)
		{
			int[] v = new int[ list.Count ];
			for(int i=0; i < list.Count; i++)
				v[i] = (int)list[i];
			return v;
		}
		#endregion
		// Given an ArrayList that contains strings's, converts to an string[]
		#region Method: static string[] ArrayListToStringArray(ArrayList list)
		static public string[] ArrayListToStringArray(ArrayList list)
		{
			string[] v = new string[ list.Count ];
			for(int i=0; i < list.Count; i++)
				v[i] = (string)list[i];
			return v;
		}
		#endregion

		// Converts a Windows Color to a RGB value
		#region Method: uint RGB(Color)
		static public uint RGB(Color c)
		{
			return ((uint)(((byte)(c.R)|((short)((byte)(c.G))<<8))|(((short)(byte)(c.B))<<16)));
		}
		#endregion

		// Is this a Paratext file? (Based on the file name, e.g., ends in ".PT_"
		#region Method: bool IsParatextScriptureFile(string sPath)
		static public bool IsParatextScriptureFile(string sPath)
		{
			// If the extension is PT_, then we'll consider it to be Paratext
			string sExtension = Path.GetExtension(sPath);
			if (sExtension.Length >= 3 && sExtension.Substring(0, 3).ToUpper() == ".PT")
				return true;

			// TODO: We could also open the file and look for Paratext-specific formatting.
			// Perhaps someday if it becomes an issue.

			return false;
		}
		#endregion

        // Calculate the room needed to display a string on the screen in a given font
        #region Method: static int CalculateDisplayWidth(g, sText, font)
        static public float CalculateDisplayWidth(Graphics g, string sText, Font font)
		{
			if (sText.Length == 0)
				return 0;
			StringFormat format  = new StringFormat();
			RectangleF   rect    = new RectangleF(0, 0, 1000, 1000);
			CharacterRange[] ranges  =  { new CharacterRange(0, sText.Length) };
			Region[] regions = new Region[1];
			format.SetMeasurableCharacterRanges(ranges);
			regions = g.MeasureCharacterRanges( sText, font, rect, format);
			rect = regions[0].GetBounds(g);
			return (rect.Right + 1.0f);
		}
		#endregion

		// Resources. Retrieve a bitmap or an icon from the program's embedded resources.
		// 1. ResourceLocation must be set first, e.g., "OurWord.Res." Note that there
		//      is a trailing period. This is typically set up as part of the Main()
		//      method, so that resources are immediately available.
		// 2. The resource must be embedded in the main exe (not a lower dll)
		#region Attr{g/s}: void ResourceLocation
		static public string ResourceLocation
		{
			get
			{
				Debug.Assert(0 != s_sResourceLocation.Length);
				return s_sResourceLocation;
			}
			set
			{
				Debug.Assert(0 != value.Length);
				s_sResourceLocation = value;
			}
		}
		static private string s_sResourceLocation = "";
		#endregion
		#region Method: Bitmap GetBitmap(string sResourceFile) - file must be in the \res folder.
		static public Bitmap GetBitmap(string sResourceFile)
			// TO INCLUDE A BITMAP IN THE PROJECT:
			// 1. Place it in the \res directory
			// 2. Use "Add Existing Item" to add it to the project
			// 3. In Solution Explorer, right-click over it and choose "Properties"; in the
			//     properties form, declare the Build Action as an "Embedded Resource". 
		{
			Assembly ExecutingAssembly = Assembly.GetEntryAssembly();

			string sResourcePath = ResourceLocation + sResourceFile;

			Stream stream = ExecutingAssembly.GetManifestResourceStream(sResourcePath);
			Debug.Assert(null != stream, "Resource " + sResourceFile + " not found.");

			return new Bitmap(stream);
		}
		#endregion
		#region Method: Icon GetIcon(string sResourceFile) - file must be in the \res folder.
		static public Icon GetIcon(string sResourceFile)
			// TO INCLUDE A BITMAP IN THE PROJECT:
			// 1. Place it in the \res directory
			// 2. Use "Add Existing Item" to add it to the project
			// 3. In Solution Explorer, right-click over it and choose "Properties"; in the
			//     properties form, declare the Build Action as an "Embedded Resource". 
		{
			Assembly ExecutingAssembly = Assembly.GetEntryAssembly();

			string sResourcePath = ResourceLocation + sResourceFile;

			Stream stream = ExecutingAssembly.GetManifestResourceStream(sResourcePath);

			return new Icon(stream);
		}
		#endregion

        // Round a float up to the nearest int value
        #region Method: int RoundUpToInt(float f)
        public int RoundUpToInt(float f)
        {
            f += 0.999999999F;
            return (int)f;
        }
        #endregion

        // Convert a color to a hex string (e.g., suitable for an html page)
        #region SMethod: string ColorToHexString(Color color)
        static char[] hexDigits = {
            '0', '1', '2', '3', '4', '5', '6', '7',
            '8', '9', 'A', 'B', 'C', 'D', 'E', 'F'
        };
        public static string ColorToHexString(Color color)
        {
            byte[] bytes = new byte[3];
            bytes[0] = color.R;
            bytes[1] = color.G;
            bytes[2] = color.B;

            char[] chars = new char[bytes.Length * 2];

            for (int i = 0; i < bytes.Length; i++)
            {
                int b = bytes[i];
                chars[i * 2] = hexDigits[b >> 4];
                chars[i * 2 + 1] = hexDigits[b & 0xF];
            }
            return new string(chars);
        }
        #endregion

        // NUnit support ---------------------------------------------------------------------
        #if DEBUG
        #region SMethod: void NUnit_Setup()
        static public void NUnit_Setup()
        {
            // Initialize the registry (the Map process needs it)
            JW_Registry.RootKey = "SOFTWARE\\The Seed Company\\Our Word!";

            // Localization DB
            LocDB.Initialize(GetApplicationDataFolder("OurWord"));
        }
        #endregion
        #region SAttr{g}: string NUnit_TestFileFolder - returns Path of folder, creates if necessary
        static public string NUnit_TestFileFolder
        {
            get
            {
                string sPath = JW_Registry.GetValue("NUnit_LocDbDir",
                    "C:\\Users\\JWimbish\\Documents\\Visual Studio 2005\\Projects\\" +
                    "OurWord\\trunk\\OurWord\\bin\\Debug\\Testing");

                if (!Directory.Exists(sPath))
                    Directory.CreateDirectory(sPath);

                Debug.Assert(Directory.Exists(sPath));

                return sPath;
            }
        }
        #endregion
        #region SMethod: void NUnit_RemoveTestFileFolder()
        static public void NUnit_RemoveTestFileFolder()
        {
            Directory.Delete(NUnit_TestFileFolder, true);
        }
        #endregion
        #region SMethod: TextWriter NUnit_OpenTextWriter(sFileName)
        static public TextWriter NUnit_OpenTextWriter(string sFileName)
        {
            string sPath = NUnit_TestFileFolder + Path.DirectorySeparatorChar + sFileName;
            StreamWriter w = new StreamWriter(sPath, false, Encoding.UTF8);
            TextWriter tw = TextWriter.Synchronized(w);
            return tw;
        }
        #endregion
        #region SMethod: TextReader NUnit_OpenTextReader(string sFileName)
        static public TextReader NUnit_OpenTextReader(string sFileName)
        {
            string sPath = NUnit_TestFileFolder + Path.DirectorySeparatorChar + sFileName;
            StreamReader r = new StreamReader(sPath, Encoding.UTF8);
            TextReader tr = TextReader.Synchronized(r);
            return tr;
        }
        #endregion
        #endif

        // Retrieves the OS-specific data folder for all users
        #region SMethod: string GetApplicationDataFolder(string sSubFolder)
        static public string GetApplicationDataFolder(string sSubFolder)
        {
            // Build the path
            string sBase = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            string sPath = sBase + Path.DirectorySeparatorChar + sSubFolder;
            if (string.IsNullOrEmpty(sBase))
                return null;

            // If the folder does not exist, then create it
            if (!Directory.Exists(sPath))
                Directory.CreateDirectory(sPath);
            if (!Directory.Exists(sPath))
                return null;

            return sPath;
        }
        #endregion

    }

	#region TEST
	public class Test_JWU : Test
	{
		#region Constructor() - Register all of the tests in this class
		public Test_JWU()
			: base( "JW Utilities" )
		{
			AddTest( new IndividualTest( Test_PathEllipses ),          "Path Ellipses" );
			AddTest( new IndividualTest( Test_ArrayListToIntArray ),   "ArrayList to int[]" );
			AddTest( new IndividualTest( Test_ArrayListToStringArray), "ArrayList to string[]" );
			AddTest( new IndividualTest( Test_IsParatextFile ),        "Is Paratext Scripture File" );
		}
		#endregion

		#region Test: Test_PathEllipses
		public void Test_PathEllipses()
		{
			string sIn = "C:\\Documents and Settings\\John\\My Documents\\Projects\\MyFile.db";
			string sExpected = "C:\\Documents and Se...\\MyFile.db";
			string sOut = JWU.PathEllipses(sIn, 30);

			AreSame(sExpected, sOut);
		}
		#endregion

		#region Test: Test_ArrayListToIntArray
		public void Test_ArrayListToIntArray()
		{
			// An array of starting data
			int[] vExpected = new int[9];
			vExpected[0] =   10;
			vExpected[1] =   12;
			vExpected[2] =   33;
			vExpected[3] =   52;
			vExpected[4] =   33;
			vExpected[5] = 1442;
			vExpected[6] =  152;
			vExpected[7] =    3;
			vExpected[8] =   15;

			// Place it into the array list
			ArrayList aIn = new ArrayList();
			foreach(int n in vExpected)
				aIn.Add(n);

			// Do the conversion
			int[] vActual = JWU.ArrayListToIntArray(aIn);

			// Does the output match the original?
			AreSame( vExpected.Length, vActual.Length );
			for(int i=0; i < vExpected.Length; i++)
				AreSame(vExpected[i], vActual[i]);
		}
		#endregion
		#region Test: Test_ArrayListToStringArray
		public void Test_ArrayListToStringArray()
		{
			// An array of starting data
			string[] vExpected = new string[9];
			vExpected[0] =   "Christiane";
			vExpected[1] =   "Robert";
			vExpected[2] =   "David";
			vExpected[3] =   "Emily";
			vExpected[4] =   "Robert";
			vExpected[5] =   "John";
			vExpected[6] =   "David";
			vExpected[7] =   "Sandra";
			vExpected[8] =   "Robert";

			// Place it into the array list
			ArrayList aIn = new ArrayList();
			foreach(string s in vExpected)
				aIn.Add(s);

			// Do the conversion
			string[] vActual = JWU.ArrayListToStringArray(aIn);

			// Does the output match the original?
			AreSame( vExpected.Length, vActual.Length );
			for(int i=0; i < vExpected.Length; i++)
				AreSame(vExpected[i], vActual[i]);
		}
		#endregion

		#region Test: Test_IsParatextFile
		public void Test_IsParatextFile()
		{
			IsTrue( JWU.IsParatextScriptureFile( "43LUKNIV84.PTX" ) );

			IsTrue( JWU.IsParatextScriptureFile( "43LUKNIV84.PTW" ) );

			IsTrue( JWU.IsParatextScriptureFile( 
				"C:\\Documents and Settings\\John\\My Documents\\Projects\\JWTools\\" +
				"TeraLuk.PTW" ) );

			IsFalse( JWU.IsParatextScriptureFile( "Kup-MRK-Final-Mu.db" ) );

			IsFalse( JWU.IsParatextScriptureFile( 
				"C:\\CCC-Lg\\Kupang\\02-Mrk\\Kup-MRK-Final-Mu.db" ) );
		}
		#endregion
	}
	#endregion
}
