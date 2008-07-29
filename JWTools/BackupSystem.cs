/**********************************************************************************************
 * Project: Our Word!
 * File:    BackupSystem.cs
 * Author:  John Wimbish
 * Created: 13 Aug 2004
 * Purpose: Routinely creates a backup on another device
 * Legal:   Copyright (c) 2005-08, John S. Wimbish. All Rights Reserved.  
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
	public class BackupSystem
	{
		const string c_sRegKey  = "Backup";
		const string c_sFolder  = "Folder";
		const string c_sEnabled = "Enabled";
		const int c_cWeeksPast = 2;
		const int c_cMonthsPast = 2;

		// Attrs -----------------------------------------------------------------------------
		#region Attr{g}: string SourcePathName
		string SourcePathName
		{
			get
			{
				return m_sSourcePathName;
			}
		}
		string m_sSourcePathName = "";
		#endregion
		#region Attr{g/s}: static bool Enabled
		static public bool Enabled
		{
			get
			{
				return JW_Registry.GetValue(c_sRegKey, c_sEnabled, true);
			}
			set
			{
				JW_Registry.SetValue(c_sRegKey, c_sEnabled, value);
			}
		}
		#endregion

		// Derived Attrs ---------------------------------------------------------------------
		#region Attr{g}: string BackupFolder
		string BackupFolder
		{
			get
			{
				// Retrieve this from the registry
				string sFolder = RegistryBackupFolder;

				// If we got nothing, we need to prompt the user; if the user chickens out
				// (via the Cancel button), then we create a folder under his My Documents
				// folder.
				if (0 == sFolder.Length)
				{
					sFolder = BrowseForFolder("");
					if (0 == sFolder.Length)
						sFolder = FallbackBackupFolder;
					RegistryBackupFolder = sFolder;
				}

				return sFolder;
			}
		}
		#endregion
		#region Attr{g}: BackupPathName
		string BackupPathName
		{
			get
			{
				// Get the base name
				string sName = Path.GetFileNameWithoutExtension(SourcePathName);

				// Append the extension to it (Note: GetExtension returns the '.')
				sName += Path.GetExtension(SourcePathName);

				return BackupFolder + Path.DirectorySeparatorChar + sName;
			}
		}
		#endregion
		#region Attr{g}: FallbackBackupFolder - if all else fails, hopefully we can backup here
		string FallbackBackupFolder
		{
			get
			{
				string sFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
				sFolder += (Path.DirectorySeparatorChar + "OurWordBackups");
				return sFolder;
			}
		}
		#endregion

		// Scaffolding -----------------------------------------------------------------------
		#region Constructor(sSourcePathName)
		public BackupSystem(string sSourcePathName)
		{
			m_sSourcePathName = sSourcePathName;
		}
		#endregion

		// Methods ---------------------------------------------------------------------------
		#region Method: bool _EnsureValidDestinationDirectory() - get a place to backup to
		private bool _EnsureValidDestinationDirectory()
		{
			// Does the destination device & path exist?
			int cAttempts = 3;
			while (!Directory.Exists(BackupFolder))
			{
				// Attempt to create it
				try
				{
					Directory.CreateDirectory(BackupFolder);
				}

				// If we fail, the device is likely not present. Ask the user to do 
				// something about it; a No answer means he is aborting.
				catch (Exception)
				{
					// If we have already tried twice, then we quietly change the
					// backup folder to a valid place on the data directory. This keeps
					// the backup happening, without further troubling the user.
					--cAttempts;
					if (cAttempts == 0)
					{
						RegistryBackupFolder = FallbackBackupFolder;
					}

					// If even the fallback fails, then we have no choice but to just
					// forget it; and completely turn off the feature
					else if (cAttempts < 0)
					{
						Enabled = false;
						return false;
					}

					// Display the message complaining about the missing flash card, and
					// offering to try again
                    else
                    {
                        if (false == LocDB.Message("msgNeedFloppyForBackup",
                            "Unable to write backup file: \n\n" +
                            "     {0}\n\n" +
                            "Please make sure there is something in the drive (e.g., a " +
                            "floppy disk, or flash memory; or whatever is appropriate\n" +
                            "for the type of drive.\n\n" +
                            "Press Yes to try again, or No to cancel the backup.",
                            new string[] { BackupPathName },
                            LocDB.MessageTypes.YN))
                        {
                            return false;
                        }
                    }
				}
			}

			// Make sure we have enough disk space
			try
			{
				string sDrive = Directory.GetDirectoryRoot(BackupPathName);
				long lFreeDiskSpace = JW_Util.GetFreeDiskSpace(sDrive);
				FileInfo fi = new FileInfo(SourcePathName);
				long lNeededSpace = fi.Length;
				if (lNeededSpace + 1000 > lFreeDiskSpace)
				{
                    LocDB.Message(
                        "msgInsufficentSpaceForBackup",
                        "There was not enough space on drive {0} for the backup.",
                        null,
                        LocDB.MessageTypes.Warning);
					return false;
				}
			}
			catch (Exception)
			{
			}

			return true;
		}
		#endregion
		#region Method: void MakeBackup()
		public void MakeBackup()
		{
			// Don't execute if the feature has been turned off
			if (!Enabled)
				return;

			// Make sure we have a place to write the backup
			if (!_EnsureValidDestinationDirectory())
				return;

			// Copy the file to the backup filename
			try
			{
				File.Copy( SourcePathName, BackupPathName, true);
			}
			catch (UnauthorizedAccessException)
			{
                LocDB.Message("msgNoPermissionToWriteFile",
                    "You do not have system permission to write the file: \n\n    {0}.",
                    new string[] { BackupPathName },
                    LocDB.MessageTypes.Error);
			}
			catch (Exception)
			{
                LocDB.Message("msgUnableToSaveFile",
                    "Unable to save the file: '{0}.'",
                    new string[] { BackupPathName },
                    LocDB.MessageTypes.Error);
			}

			// Delete older files if appropriate
			CleanUpOldFiles(BackupFolder);
		}
		#endregion

		// Helper methods for CleanUpOldFiles ------------------------------------------------
		#region Method: DateTime DateFromFileName(string sPath)
		DateTime DateFromFileName(string sPath)
			// We use the Create date, rather than fool with parsing the file's name.
		{
			return File.GetCreationTime(sPath);
		}
		#endregion
		#region Method: void CleanUpOldFiles(string sBackupFolder)
		public void CleanUpOldFiles(string sBackupFolder)
		{
			// Get the file name (less the date)
			string sPathPartial = sBackupFolder + Path.DirectorySeparatorChar + 
				Path.GetFileNameWithoutExtension(SourcePathName);

			// Get the list of files in the directory
			string[] sAllFiles = Directory.GetFiles( sBackupFolder );

			// Narrow it down to just those files that match ours
			ArrayList FileList = new ArrayList();
			foreach(string s in sAllFiles)
			{
				if (s.StartsWith( sPathPartial ))
					FileList.Add(s); 
			}

			// Get the date that ends this week
			DateTime dateThisWeekEnds = DateTime.Today;
			while (dateThisWeekEnds.DayOfWeek != DayOfWeek.Saturday)
				dateThisWeekEnds = dateThisWeekEnds.AddDays(1);

			// Get the cutoff before which we go to weekly updates
			DateTime dateDaily = dateThisWeekEnds.AddDays( -((c_cWeeksPast+1) * 7) );

			// Get the cutoff before which we go to monthly updates
			int nYear = dateThisWeekEnds.Year;
			int nMonth = dateThisWeekEnds.Month - ( c_cMonthsPast + 1);
			if (nMonth < 1)
			{
				nMonth += 12;
				nYear--;
			}
			int nDay = DateTime.DaysInMonth(nYear, nMonth);
			DateTime dateMonthly = new DateTime(nYear, nMonth, nDay);

			// Loop through the filenames
			for(int i = 0; i < FileList.Count; i++)
			{
				// Get the pathname
				string sName = FileList[i] as String;
				if (!File.Exists(sName))
					continue;

				// Get the file's date
				DateTime date = DateFromFileName(sName);

				// If it is after the Daily cutoff, then we're done.
				if (DateTime.Compare( dateDaily, date ) < 0 )
					continue;

				// If it is after the Weekly cutoff (therefore a Weekly File)
				if (DateTime.Compare( dateMonthly, date) < 0 )
				{
					DeleteOnWeeklyBasis(sName, ref FileList);
					continue;
				}

				// If we are here, it is a monthly file
				DeleteOnMonthlyBasis(sName, ref FileList);

			}

		}
		#endregion
		#region Method: void DeleteOnWeeklyBasis(string sName, ref ArrayList list)
		private void DeleteOnWeeklyBasis(string sName, ref ArrayList list)
		{
			DateTime date = DateFromFileName(sName);

			DateTime dateBegin = date;
			while (dateBegin.DayOfWeek != DayOfWeek.Sunday)
				dateBegin = dateBegin.AddDays(-1);

			DateTime dateEnd = date;
			while (dateEnd.DayOfWeek != DayOfWeek.Saturday)
				dateEnd = dateEnd.AddDays(-1);

			DeleteForPeriod(sName, ref list, dateBegin, dateEnd);
		}
		#endregion
		#region Method: void DeleteOnMonthlyBasis(string sName, ref ArrayList list)
		private void DeleteOnMonthlyBasis(string sName, ref ArrayList list)
		{
			DateTime date = DateFromFileName(sName);

			DateTime dateEnd = date;
			while ( (dateEnd.AddDays(1)).Month == date.Month)
				dateEnd = dateEnd.AddDays(1);

			DateTime dateBegin = new DateTime( date.Year, date.Month, 1);

			DeleteForPeriod(sName, ref list, dateBegin, dateEnd);
		}
		#endregion
		#region Method: void DeleteForPeriod(...)
		private void DeleteForPeriod(string sName, ref ArrayList list, 
			DateTime dateBegin, DateTime dateEnd)
		{
			DateTime date = DateFromFileName(sName);

			string sLatest = sName;
			for(int i = 0; i < list.Count; i++)
			{
				// Empty strings represent files we've already deleted
				string s = list[i] as string;
				if (!File.Exists(s))
					continue;

				// Get the file's date
				DateTime d = DateFromFileName(s);

				// Is the file (s) within this week?
				if ( DateTime.Compare( dateBegin, d) > 0)
					continue;
				if (DateTime.Compare( dateEnd, d) < 0)
					continue;

				// If the date is earlier, then we delete the file
				if (DateTime.Compare( d, date) < 0)
				{
					File.Delete(s);
					list[i] = "";
				}

					// If the date is later, then we delete the earlier file
				else if (DateTime.Compare( d, date) > 0)
				{
					if (File.Exists(sLatest))
						File.Delete(sLatest);
					sLatest = s;
				}
			}
		}
		#endregion

		// Registry --------------------------------------------------------------------------
		#region Attr{g/s}: static string RegistryBackupFolder
		static public string RegistryBackupFolder
		{
			get
			{
				return JW_Registry.GetValue(c_sRegKey, c_sFolder, "");
			}
			set
			{
				JW_Registry.SetValue(c_sRegKey, c_sFolder, value);
			}
		}
		#endregion
		#region Method: static string BrowseForFolder(string sOriginalFolder)
		static public string BrowseForFolder(string sOriginalFolder)
		{
			FolderBrowserDialog dlg = new FolderBrowserDialog();

            dlg.Description = LocDB.GetValue(
                new string[] { "Strings", "Files" },
                "BrowseForBackupFolderDescr",
                "Select the folder where you wish to place your backup files. If " +
                    "possible, this should not be your hard drive. A flash card is ideal; " +
                    "or a floppy drive can also be used.",  
                null,
                null);

			dlg.RootFolder  = Environment.SpecialFolder.MyComputer;
			if (DialogResult.OK == dlg.ShowDialog())
				return dlg.SelectedPath;
			return sOriginalFolder;
		}
		#endregion
	}

	#region TEST
	public class Test_BackupSystem : Test
	{
		#region Constructor()
		public Test_BackupSystem()
			: base("BackupSystem")
		{
			AddTest( new IndividualTest( CleanUpFileNames ), "CleanUpFileNames" );
		}
		#endregion

		#region void CleanUpFileNames()
		public void CleanUpFileNames()
		{
			int cFilesToCreate = 400;

			// Create an empty working directory
			string sTestPath = Path.GetDirectoryName(Application.ExecutablePath) + 
				Path.DirectorySeparatorChar + "BackupTest";
			if (Directory.Exists(sTestPath))
				Directory.Delete(sTestPath, true);
			Directory.CreateDirectory(sTestPath);

			// Give the system time to get the directory stuff done
			Thread.Sleep(1000);

			// Base name & extension
			string sBaseName = "TestFile";
			string sExtension = ".db";

			// Get today's date; we'll do it based on "today"
			DateTime today = DateTime.Today;
			int nYear  = today.Year;
			int nMonth = today.Month;
			int nDay   = today.Day;

			// Create a whole bunch of files
			DateTime dt = today;
			for(int i = 0; i < cFilesToCreate; i++)
			{
				string sDate = dt.ToString("yyyy-MM-dd");
				string sPath = sTestPath + Path.DirectorySeparatorChar + sBaseName + " " + sDate + sExtension;

				FileStream f = File.Create(sPath);
				f.Flush();
				f.Close();
				File.SetCreationTime(sPath, dt);

				--nDay;
				if (nDay < 1)
				{
					--nMonth;
					if (nMonth < 1)
					{
						nMonth += 12;
						--nYear;
					}
					nDay = DateTime.DaysInMonth(nYear, nMonth);
				}
				dt = new DateTime(nYear, nMonth, nDay);
			}

			// We should have a lot of files
			AreSame( cFilesToCreate, Directory.GetFiles(sTestPath).Length );

			// Call the cleanup routine
			string sSourcePath = sTestPath + Path.DirectorySeparatorChar + sBaseName + sExtension;
			BackupSystem backup = new BackupSystem(sSourcePath);
			backup.CleanUpOldFiles(sTestPath);

			// The number of files we now have depends on the day of week, day of 
			// month, etc. There should be 14-to-21 daily files, 6-to-11 weekly
			// files, and then 1 per month before that. Taking the minimum,
			// and assuming an initial count of 400, we expect there to be
			// at least 30 files. 
			//   Then assuming the maximums, and an initial count of 400, we would
			// expect there to be fewer than 50 files.
			int cFiles = Directory.GetFiles(sTestPath).Length;
			IsTrue(cFiles > 30);
			IsTrue(cFiles < 50);

			// Clean up my disk!
			Directory.Delete(sTestPath, true);
		}
		#endregion
	}
	#endregion
}
