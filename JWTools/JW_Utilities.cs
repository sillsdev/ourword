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
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Management.Instrumentation;
using System.Management;
using System.Reflection;
using System.Runtime.InteropServices;
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
			sPath = PathConverter.ConvertDirectorySeparators(sPath);
			if (sPath.Length <= cMaxLen)
				return sPath;

			if (cMaxLen <= 2)
				return "...";

			string sBaseName = Path.GetFileName(sPath);
			string sDirectory = Path.GetDirectoryName(sPath);

			if (sBaseName.Length > cMaxLen)
				return sBaseName.Substring(0, cMaxLen - 2) + "...";

			int nLength = Math.Max(0, cMaxLen - 2 - sBaseName.Length);

			return sDirectory.Substring(0, nLength) + "..." + Path.DirectorySeparatorChar + sBaseName;
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
			var ExecutingAssembly = Assembly.GetEntryAssembly();

            // From NUnit, ExecutingAssembly returns null, so we just create a dummy
            // bitmap as we don't care about the appearance during testing.
            if (null == ExecutingAssembly)
                return new Bitmap(16, 16);

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
			var ExecutingAssembly = Assembly.GetEntryAssembly();

			string sResourcePath = ResourceLocation + sResourceFile;

			Stream stream = ExecutingAssembly.GetManifestResourceStream(sResourcePath);

			return new Icon(stream);
		}
		#endregion
        #region SMethod: Bitmap ChangeBitmapBackground(Bitmap, clrBackground)
        static public Bitmap ChangeBitmapBackground(Bitmap bmp, Color clrBackground)
        {
            Debug.Assert(null != bmp);

            // Set its transparent color to the background color. We assume that the
            // pixel at 0,0 is a background pixel.
            Color clrTransparent = bmp.GetPixel(0, 0);
            for (int h = 0; h < bmp.Height; h++)
            {
                for (int w = 0; w < bmp.Width; w++)
                {
                    if (bmp.GetPixel(w, h) == clrTransparent)
                        bmp.SetPixel(w, h, clrBackground);
                }
            }

            return bmp;
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

            // Set the resource location
            JWU.ResourceLocation = "OurWord.Res.";
        }
        #endregion
        #region SAttr{g}: string NUnit_TestFileFolder - returns Path of folder, creates if necessary
        static public string NUnit_TestFileFolder
        {
            get
            {
                string sPath = JW_Registry.GetValue("NUnit_LocDbDir",
                    "C:\\Users\\JWimbish\\Documents\\Visual Studio 2008\\Projects\\" +
                    "ourword\\OurWord\\bin\\Debug\\Testing");

                if (!Directory.Exists(sPath))
                    Directory.CreateDirectory(sPath);

                Debug.Assert(Directory.Exists(sPath));

                return sPath;
            }
        }
        #endregion
        #region SAttr{g}: string NUnit_TestFilePathName - returns a test filename
        static public string NUnit_TestFilePathName
        {
            get
            {
                return NUnit_TestFileFolder + Path.DirectorySeparatorChar + "test.x";
            }
        }
        #endregion
        #region SMethod: void NUnit_RemoveTestFileFolder()
        static public void NUnit_RemoveTestFileFolder()
        {
            if (Directory.Exists(NUnit_TestFileFolder))
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
        #region SAttr{g}: string NUnit_ClusterFolderName
        static public string NUnit_ClusterFolderName
        {
            get
            {
                return "UnitTest";
            }
        }
        #endregion
        #region SMethod: void NUnit_SetupClusterFolder()
        static public void NUnit_SetupClusterFolder()
        {
            string s = NUnit_ClusterFolderName + Path.DirectorySeparatorChar + ".Settings";
            GetMyDocumentsFolder(s);
        }
        #endregion
        #region SMethod: void NUnit_TeardownClusterFolder()
        static public void NUnit_TeardownClusterFolder()
        {
            string sFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            sFolder += (Path.DirectorySeparatorChar + NUnit_ClusterFolderName);
            if (Directory.Exists(sFolder))
                Directory.Delete(sFolder, true);
        }
        #endregion
        #endif

        // Retrieves the OS-specific data folder for all users
        #region SMethod: string GetSpecialFolder(Environment.SpecialFolder, sSubFolder)
        static public string GetSpecialFolder(Environment.SpecialFolder special, string sSubFolder)
        {
            string sFolder = Environment.GetFolderPath(special);

            if (sFolder[sFolder.Length - 1] != Path.DirectorySeparatorChar)
                sFolder += Path.DirectorySeparatorChar;

            if (!string.IsNullOrEmpty(sSubFolder))
                sFolder += (sSubFolder);

            if (sFolder[sFolder.Length - 1] != Path.DirectorySeparatorChar)
                sFolder += Path.DirectorySeparatorChar;

            if (!string.IsNullOrEmpty(sSubFolder))
            {
                if (!Directory.Exists(sFolder))
                    Directory.CreateDirectory(sFolder);
            }

            return sFolder;
        }
        #endregion
        #region SMethod: string GetLocalApplicationDataFolder(string sSubFolder)
        static public string GetLocalApplicationDataFolder(string sSubFolder)
        {
            return GetSpecialFolder(Environment.SpecialFolder.LocalApplicationData, sSubFolder);
        }
        #endregion
        #region SMethod: string GetApplicationDataFolder(string sSubFolder)
        static public string GetApplicationDataFolder(string sSubFolder)
        {
            if (Environment.OSVersion.Platform == PlatformID.Unix)
                return GetSpecialFolder(Environment.SpecialFolder.ApplicationData, sSubFolder);
            else
                return GetSpecialFolder(Environment.SpecialFolder.CommonApplicationData, sSubFolder);
        }
        #endregion
		#region SMethod: string GetMyDocumentsFolder(string sSubFolder)
		static public string GetMyDocumentsFolder(string sSubFolder)
		{
            return GetSpecialFolder(Environment.SpecialFolder.MyDocuments, sSubFolder);
		}
		#endregion
		#region SMethod: GetAllUsersDocumentsFolder() - (not currently used)
		/*** KEEPING THE CODE, SHOULD I EVER WANT TO DO THIS
		 * 
		[DllImport("shell32.dll")]
		public static extern Int32 SHGetFolderPath(IntPtr hwndOwner, Int32 nFolder, IntPtr hToken,
			UInt32 dwFlags, StringBuilder pszPath); 
		static public string GetAllUsersDocumentsFolder()
		{
			/////////////////////////////////////////////////////////
			// TODO - Gonna need a LINUX equivalent for this mess. //
			/////////////////////////////////////////////////////////
			if (Environment.OSVersion.Platform == PlatformID.Unix)
				Debug.Assert(false, "NEED A LINUX EQUIVALENT in GetAllUsersDocumentsFolder");

			// The file system directory that contains documents that are common to all users. 
			// A typical paths is "C:\Documents and Settings\All Users\Documents." Valid for 
			// Windows NT systems and Microsoft Windows® 95 and Windows 98 systems with 
			// Shfolder.dll installed.
			Int32 nCommonDocs = (0x002e);

			// SHGFP_TYPE_CURRENT is the current value for user, verify it exists
            UInt32 dwFlag = 0; 

			// Call the Windows Shell function
			StringBuilder path = new System.Text.StringBuilder(256);
			SHGetFolderPath(IntPtr.Zero, nCommonDocs, IntPtr.Zero, dwFlag, path);

			// Add the path separator and return the result
			return path.ToString() + Path.DirectorySeparatorChar;
		}
		***/
		#endregion
		#region SMethod: string ExtractRightmostSubFolder(sPath)
		static public string ExtractRightmostSubFolder(string sPath)
			// Given, e.g., 
			//     "c:\Documents and Settings\All Users\Hello\",
			// returns
			//     "Hello"
		{
			// We'll put the answer here
			string sFolderName = "";

			// Nothing to do if empty
			if (string.IsNullOrEmpty(sPath))
				return "";

			// If the final character is a Directory Separator, remove it
			if (sPath[sPath.Length - 1] == Path.DirectorySeparatorChar)
				sPath = sPath.Substring(0, sPath.Length - 1);

			// Process through the path. Whenever we encounter a '\', reset
			// so that when we finally end, we'll have retreived the final one.
			foreach (char ch in sPath)
			{
				if (ch == Path.DirectorySeparatorChar)
					sFolderName = "";
				else
					sFolderName += ch;
			}

			return sFolderName;
		}
		#endregion

        #region SMethod: void SafeFolderDelete(string sFolderPath)
        static public void SafeFolderDelete(string sFolderPath)
            // There are certain folders we don't want to permit deletion, e.g., 
            // My Documents (as we know by unpleasane experience)
        {
            // For consistency, put a SeparatorCharacter on the end of the target folder
            if (sFolderPath[sFolderPath.Length - 1] != Path.DirectorySeparatorChar)
                sFolderPath += Path.DirectorySeparatorChar;

            // Folders we're protecting
            var v = new List<string>();
            v.Add(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
            v.Add(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
            v.Add(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData));
            v.Add(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData));
            v.Add(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles));
            v.Add(Environment.GetFolderPath(Environment.SpecialFolder.Desktop));

            // Loop to make sure we're not trying to delete 
            foreach (string s in v)
            {
                if (s == sFolderPath)
                    return;
                if (s + Path.DirectorySeparatorChar == sFolderPath)
                    return;
            }

            // We can now delete the file without worry
            if (Directory.Exists(sFolderPath))
                Directory.Delete(sFolderPath, true);
        }
        #endregion
        #region SMethod: void Copy(string sSourceFolder, string sDestinationFolder)
        static void Copy(string sSourceFolder, string sDestinationFolder)
        {
            // Get the info about this folder
            DirectoryInfo dir = new DirectoryInfo(sSourceFolder);

            // Create the destination folder
            Directory.CreateDirectory(sDestinationFolder);

            // Copy the files over
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(sDestinationFolder, file.Name);
                file.CopyTo(temppath, true);
            }

            // Copy the subfolders over via recursion
            DirectoryInfo[] dirs = dir.GetDirectories();
            foreach (DirectoryInfo subdir in dirs)
            {
                string temppath = Path.Combine(sDestinationFolder, subdir.Name);
                Copy(subdir.FullName, temppath);
            }
        }
        #endregion
        #region SMethod: bool Move(sSourceFolder, sDestinationFolder)
        static public bool Move(string sSourceFolder, string sDestinationFolder)
            // We want this to be safe, so instead of a strict move, we first do a copy of
            // everything. If the copy succeeds, then we delete from the source. 
        {
            // Abort if the destination already exists
            if (Directory.Exists(sDestinationFolder))
                return false;

            // Copy the files over; restore back on any failure
            try
            {
                Copy(sSourceFolder, sDestinationFolder);
            }
            catch (Exception)
            {
                Directory.Delete(sDestinationFolder, true);
                return false;
            }

            // Remove the source folder; restore back on any failure
            try
            {
                Directory.Delete(sSourceFolder, true);
            }
            catch (Exception)
            {
                Copy(sDestinationFolder, sSourceFolder);
                Directory.Delete(sDestinationFolder, true);
                return false;
            }

            return true;
        }
        #endregion

        #region SMethod: List<string> ReadFile(sPath)
        static public List<string> ReadFile(string sPath)
        {
            var v = new List<string>();

            var sr = new StreamReader(sPath, Encoding.UTF8);
            var tr = TextReader.Synchronized(sr);

            string s;
            while ((s = tr.ReadLine()) != null)
                v.Add(s);

            tr.Close();

            return v;
        }
        #endregion
        #region SMethod: void WriteFile(string sPath, string[] vs)
        static public void WriteFile(string sPath, string[] vs)
        {
            var sw = new StreamWriter(sPath, false, Encoding.UTF8);
            var tw = TextWriter.Synchronized(sw);

            foreach(string s in vs)
                tw.WriteLine(s);

            tw.Close();
        }
        #endregion
        #region SMethod: void WriteFile(string sPath, string s)
        static public void WriteFile(string sPath, string s)
        {
            var sw = new StreamWriter(sPath, false, Encoding.UTF8);
            var tw = TextWriter.Synchronized(sw);
            tw.WriteLine(s);
            tw.Close();
        }
        #endregion

    }

    #region CLASS: JW_Util
    public class JW_Util
	{
		#region Method: static void TextWriter GetTextWriter(string sPath)
		public static TextWriter GetTextWriter(string sPath)
		{
            // Delete any existing file, so that we can avoid the Read-Only
            // problems that have been happening in Timor.
            try
            {
                File.Delete(sPath);
            }
            catch (Exception)
            {
            }

            // Create a new file to write to
			StreamWriter w = new StreamWriter(sPath, false, Encoding.UTF8);
			TextWriter tw = TextWriter.Synchronized(w);
			return tw;
		}
		#endregion
		#region Method: static void TextReader GetTextReader(ref sPath, sFileFilter)
		public static TextReader GetTextReader(ref string sPath, string sFileFilter)
		{
			StreamReader r = OpenStreamReader(ref sPath, sFileFilter);
			TextReader tr = TextReader.Synchronized(r);
			return tr;
		}
		#endregion

		#region Method: StreamReader OpenStreamReader(...) - provides opportunity to browse
		static public StreamReader OpenStreamReader(
			ref string sPathName, 
			string sFileFilter)
		{
			if (!File.Exists(sPathName))
			{
				// Tell the user, and see if he wants to browse for it
				string sMessage = "Unable to locate file \n\"" + sPathName +
					".\" \n\nDo you want to browse for it?";
				DialogResult result = MessageBox.Show( Form.ActiveForm, sMessage, 
					"File Not Found", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
				if (result != DialogResult.Yes)
					throw new IOException("Unable to locate file" + sPathName);

				// Browse for the file
				OpenFileDialog dlg = new OpenFileDialog();

				// We only want to return a single file
				dlg.Multiselect = false;

				// Filter on the desired type of files
				if (null != sFileFilter && sFileFilter.Length > 0)
				{
					dlg.Filter = sFileFilter;
					dlg.FilterIndex = 0;
				}

				// Retrieve Dialog Title from resources
				dlg.Title = "Browse for " + Path.GetFileName(sPathName);

				// Get the default pathname
				if (sPathName.Length > 0)
				{
					dlg.InitialDirectory = Path.GetDirectoryName(sPathName);
					dlg.FileName = Path.GetFileName(sPathName);
				}

				// Browse for the file
				if (DialogResult.OK != dlg.ShowDialog(Form.ActiveForm))
					throw new IOException("Unable to locate file" + sPathName);
				sPathName = dlg.FileName;
			}

            return new StreamReader(sPathName, Encoding.UTF8);
		}
		#endregion

		#region Method: static void CreateBackup(string sPathName, string sNewExtension)
		static public string CreateBackup(string sPathName, string sNewExtension)
		{
			string sBackupPathName = Path.ChangeExtension(sPathName, sNewExtension);
			if (File.Exists(sPathName))
			{
				try
				{
					if (File.Exists(sBackupPathName))
						File.Delete(sBackupPathName);
					File.Move(sPathName, sBackupPathName);
				}
				catch (Exception)
				{}
			}
			return sBackupPathName;
		}
		#endregion
		#region Method: static void RestoreFromBackup(string sPathName, string sExtension)
		static public void RestoreFromBackup(string sPathName, string sExtension)
		{
			try
			{
				// Give time for the OS to finish any cleanup it is doing
				Thread.Sleep(1000);

				// Copy the backup onto the filename (provided one exists, of course)
				string sBackupPathName = Path.ChangeExtension(sPathName, sExtension);
				if (File.Exists(sBackupPathName))
					File.Copy(sBackupPathName, sPathName, true);
			}
			catch (Exception)
			{}
		}
		#endregion

		#region Method: static long GetFreeDiskSpace(string sDrive)
		static public long GetFreeDiskSpace(string sDrive)
			// Borrowed from a message by Roman Rodov, 5mar04, on the CodeGuru
			// message board. 
		{
			long lFreeSpace = 0L;

			// Get the management class holding Logical Drive information
			ManagementClass mcDriveClass = new ManagementClass("Win32_LogicalDisk");

			// Enumerate all logical drives available
//			Console.WriteLine("GetFreeDiskSpace thread exits here...");
			ManagementObjectCollection mocDrives = mcDriveClass.GetInstances();
			foreach(ManagementObject moDrive in mocDrives)
			{
				// sDeviceId will hold the drive name eg "C:"
				String sDeviceId = moDrive.Properties["DeviceId"].Value.ToString() + "\\";
				if (sDeviceId == sDrive)
				{
					PropertyData pd = moDrive.Properties["FreeSpace"];
					lFreeSpace = long.Parse(pd.Value.ToString());
					break;
				}
			}

			// Done
			mocDrives.Dispose();
			mcDriveClass.Dispose();

			return lFreeSpace;
		}
		#endregion

		// XML Utilities ---------------------------------------------------------------------
		const char c_chDelim = '\"';
		const char c_chBlank = ' ';
		#region Method: static string XmlGetStringAttr(string sAttr, string s)
		static public string XmlGetStringAttr(string sAttr, string s)
			// Given a string s, such as
			//    <SR ID="3425" Gloss="return home" Form="pulang">
			// if "Form" is requested, returns
			//    pulang
		{
			// Find the Attribute within the string
			int i = s.IndexOf(sAttr);
			if (-1 == i)
				return "";

			// Move to the opening delimiter
			while (i < s.Length && s[i] != c_chDelim)
				++i;
			if (i < s.Length && s[i] == c_chDelim)
				++i;

			// Extract the value
			string sValue = "";
			while( i < s.Length && s[i] != c_chDelim)
			{
				sValue += s[i];
				i++;
			}
			return sValue;
		}
		#endregion
		#region Method: static int XmlGetIntAttr(string sAttr, string s)
		static public int XmlGetIntAttr(string sAttr, string s)
		{
			string sValue = XmlGetStringAttr(sAttr,s);

			try
			{
				int nValue = Convert.ToInt16(sValue);
				return nValue;
			}
			catch(Exception)
			{
				return 0;
			}
		}
		#endregion
		#region Method: static int XmlGetBoolAttr(string sAttr, string s)
		static public bool XmlGetBoolAttr(string sAttr, string s)
		{
			string sValue = XmlGetStringAttr(sAttr,s);

			if (sValue.ToLower() == "true")
				return true;

			return false;
		}
		#endregion
		#region Method: static string[] XmlGetEmbeddedObjects(string sAttr, string s)
		static public string[] XmlGetEmbeddedObjects(string sAttr, string s)
		{
			// The sTag will start each new embedded object
			string sTag = "<" + sAttr + " ";

			// Find the first one; if there isn't one, then return an empty array
			int i = s.IndexOf(sTag);
			if (-1 == i)
				return new string[0];
			s = s.Substring(i);

			// We'll temporarily put the answers in an array list
			ArrayList a = new ArrayList();

			// Extract all of the strings. We assume that they are all here in a row,
			// together. If this turns out not to be the case, then we'll need to
			// rework the logic to use s.IndexOf(sTag) to find subsequent ones.
			while (s.Length > 0 && s.IndexOf(sTag) == 0)
			{
				int nLen = XmlDistanceToClosingBrace(s);

				string sObject = s.Substring(0, nLen + 1);

				a.Add(sObject);

				s = s.Substring(nLen + 1);

				while (s.Length > 0 && s[0] == ' ')
					s = s.Substring(1);
			}

			// Convert to a string array
			string[] v = new string[ a.Count ];
			for(int k=0; k<a.Count; k++)
				v[k] = (string)a[k];
			return v;
		}
		#endregion
		#region Method: static int XmlDistanceToClosingBrace(string s)
		static public int XmlDistanceToClosingBrace(string s)
		{
			// We should be sitting at an opening brace
			if (s.Length == 0 || s[0] != '<')
				return 0;

			// Use this to ignore any '>' that might be in a field
			bool bIsInField = false;

			// Use this to skip over any embedded data
			int cDepth = 0;

			// Loop through the line until we find it
			int i = 1;
			while( i < s.Length )
			{
				// We've found the closing bracket
				if (s[i] == '>' &&  !bIsInField && cDepth == 0)
					break;

				if ( s[i] == c_chDelim )
					bIsInField = !bIsInField;

				if ( s[i] == '<')
					cDepth++;
				if (s[i] == '>')
					cDepth--;

				i++;
			}
			return i;
		}
		#endregion

        #region SMethod: string XmlValue(string sAttr, string sValue, bool bSpaceAfter)
        static public string XmlValue(string sAttr, string sValue, bool bSpaceAfter)
		{
			Debug.Assert(sAttr.Length > 0);
			Debug.Assert(sAttr[ sAttr.Length - 1] != '=');

			string s = sAttr + "=" + c_chDelim + sValue + c_chDelim;

			if (bSpaceAfter)
				s += c_chBlank;

			return s;
        }
        #endregion
        #region SMethod: string XmlValue(string sAttr, int nValue, bool bSpaceAfter)
        static public string XmlValue(string sAttr, int nValue, bool bSpaceAfter)
		{
			return XmlValue(sAttr, nValue.ToString(), bSpaceAfter );
        }
        #endregion
        #region SMethod: string XmlValue(string sAttr, bool bValue, bool bSpaceAfter)
        static public string XmlValue(string sAttr, bool bValue, bool bSpaceAfter)
		{
			return XmlValue(sAttr,(bValue ? "true" : "false" ), bSpaceAfter );
        }
        #endregion

        #region SMethod: Encoding GetUnicodeFileEncoding(String FileName)
        public static Encoding GetUnicodeFileEncoding(String FileName)
            // Return the Encoding of a text file.  Return Encoding.Default if no Unicode
            // BOM (byte order mark) is found.
        {
            Encoding enc = null;

            FileInfo info = new FileInfo(FileName);

            FileStream stream = null;

            try
            {
                stream = info.OpenRead();

                Encoding[] UnicodeEncodings = { Encoding.BigEndianUnicode, Encoding.Unicode, Encoding.UTF8 };

                for (int i = 0; enc == null && i < UnicodeEncodings.Length; i++)
                {
                    stream.Position = 0;

                    byte[] Preamble = UnicodeEncodings[i].GetPreamble();

                    bool PreamblesAreEqual = true;

                    for (int j = 0; PreamblesAreEqual && j < Preamble.Length; j++)
                    {
                        PreamblesAreEqual = Preamble[j] == stream.ReadByte();
                    }

                    if (PreamblesAreEqual)
                    {
                        enc = UnicodeEncodings[i];
                    }
                }
            }
            catch (System.IO.IOException)
            {
                return null;
            }
            finally
            {
                if (stream != null)
                {
                    stream.Close();
                }
            }

            if (enc == null)
            {
                enc = Encoding.Default;
            }

            return enc;
        }
    #endregion
    }
    #endregion

}
