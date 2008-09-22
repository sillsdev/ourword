/**********************************************************************************************
 * Dll:      JWTools
 * File:     JW_FileMenuIO.cs
 * Author:   John Wimbish
 * Created:  03 Oct 2003
 * Modified: 01 Jul 2004 - Converted to DotNetBar (tests are now broken)
 * Purpose:  Manages a list of files, both as an MRU, and writing to/from the registry, for
 *           the File menu commands (Open, Save, etc.)
 * Legal:   Copyright (c) 2005-08, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
#region Using
using System;
using System.Diagnostics;
using System.Windows.Forms;
using System.Text;
using System.Threading;
using System.IO;
using Microsoft.Win32;
#endregion

#region Documentation
/* Encapsulation of the Open/Save/SaveAs/New/MRU functionality. That is, the calling form
 * will have menu items for File-Open, Save, etc., the JW_FileMenuIO class manages the MRU
 * and all of the logic behind making it all work. 
 
 * To use this in another application:
 *   o Implement the IFileMenuIO interface.
 *   o The MRU should be loaded in the application as part of the onLoad event; and should be 
 *     saved as part of the onClosing event.
 */
#endregion
#region STORIES STILL TO DO
//   o On File-New or File-Open, we need to prompt to save the existing configuration. 
//       - This involves an IsDirty mechanism so that we don't bug the user  if changes haven't 
//            been made. 
//       - Do we want a switch whose' meaning is that we always save without asking?
//   o GetRelatedPathName(...) should be a general utility, rather than something in this class.
//   o Program a switch where the title bar is optionally not changed.
//   o Program a multiple-document mode, rather than one-at-a time
#endregion

namespace JWTools
{
	// Interface IJW_FileMenuIO --------------------------------------------------------------
	#region Interface: IJW_FileMenuIO - caller must implement this
	public interface IJW_FileMenuIO
		// In order to use the JW_FileMenuIO class, this interface must be implemented. This
		// enables the calling code to respond to actions from the io, namely, reading and
		// saving the data file.
	{
		// Reset the target so that it is set to its default values. This is what
		// one would expect after issuing a File-New command.
		void New();

		// Given an open StreamReader, read in the data into the target. (Do not
		// close the StreamReader when done.)
		void Read(ref string sPathName);

		// Given an open StreamWriter, write out the data from the target. (Do
		// not close the StreamWriter when done.)
		void Write(string sPathName);
	}
	#endregion

	// Dinky Classes needed for the io implementation ----------------------------------------
	public class JW_FileMenuIO
	{
		// Public Attributes -----------------------------------------------------------------
		#region Attribute{g/s}: string InitialDirectory - Default directory for data files.
		public string InitialDirectory
		{
			get { return m_sInitialDirectory; }
			set { m_sInitialDirectory = value; }
		}
		string m_sInitialDirectory = "";
		#endregion
		#region Attribute{g/s}: string FileName - filename to open / save data from / to.
		public string FileName
		{
			get { return m_sFileName; }
			set { m_sFileName = value; }
		}
		string m_sFileName = "";
		#endregion
		#region Attribute{g}:   bool IsUserSuppliedFileName - If F, we need to prompt user for a name
		public bool IsUserSuppliedFileName
		{
			get { return ("" != m_sFileName); }
		}
		#endregion
		#region Method: SetToNoFileName() - give FileName a value, "", indicating no user-supplied name yet
		protected void SetToNoFileName()
		{
			FileName = "";
		}
		#endregion
		#region Attr{g/s}: string DefaultFileName - will appear on the SaveAs default
		public string DefaultFileName
		{
			get
			{
				// Check for any invalid characters
				string s = m_sFileDefaultName;
                foreach (char c in Path.GetInvalidPathChars())
				{
					if (s.IndexOf(c) >= 0)
						return "";
				}
				if (s.IndexOf(':') >= 0)
					return "";
				return m_sFileDefaultName;
			}
			set
			{
				m_sFileDefaultName = value;
			}
		}
		private string m_sFileDefaultName;            // e.g., "Default Layout.mcs"
		#endregion

		public const string c_cmdMRU  = "cmdMRU";
		public const string c_cmdMore = "cmdMore";
		public const string c_sRegistrySubkey = "MRU";

		// Private Attributes ----------------------------------------------------------------
		private IJW_FileMenuIO m_target = null;       // Provides the read/write/new methods
		private Form m_form = null;                   // Used to access menu and title bar
		private string m_sAppTitle;                   // The name of the app, e.g., "Excel".
		private string m_sFileFilter;                 // e.g., "MultiColumn Scripture files (*.mcs)|*.mcs|All files (*.*)|*.*"
		private string m_sFileExtension;              // e.g., "mcs"
		#region Attr{g}: string[] PathNames - used for testing
		public string[] PathNames
		{
			get
			{
				return m_rgPathNames;
			}
		}
		#endregion
		private string[] m_rgPathNames = new string[0];

		// Public Methods --------------------------------------------------------------------
		#region Method: void UpdateWindowTitle()
		public void UpdateWindowTitle()
		{
			if (IsUserSuppliedFileName)
				m_form.Text = m_sAppTitle + " - " + Path.GetFileNameWithoutExtension(FileName);
			else
				m_form.Text = m_sAppTitle;
		}
		#endregion
		#region Constructor - initializes the appropriate attributes
		public JW_FileMenuIO(IJW_FileMenuIO target, Form form, string sAppTitle, 
			string sFileFilter, string sFileExtension, string sFileDefaultName)
			// Constructor. Sets up the various private attributes.
		{
			m_target           = target;
			m_form             = form;
			m_sAppTitle        = sAppTitle;
			m_sFileFilter      = sFileFilter;
			m_sFileExtension   = sFileExtension;
			m_sFileDefaultName = sFileDefaultName;

			string sPath = Application.ExecutablePath;
			m_sInitialDirectory = sPath.Substring(0, sPath.LastIndexOf(Path.DirectorySeparatorChar) ) 
                + Path.DirectorySeparatorChar;

			AssertWellFormedness();
		}
		#endregion
		#region Method: void AssertWellFormedness() - make sure we've been properly initialized.
		protected void AssertWellFormedness()
		{
			Debug.Assert(null != m_target);
			Debug.Assert(null != m_form);
			Debug.Assert(m_sAppTitle.Length > 0);
			Debug.Assert(m_sFileFilter.Length > 0);
			Debug.Assert(m_sFileExtension.Length > 0);
			Debug.Assert(m_sFileDefaultName.Length > 0);
		}
		#endregion
		#region Method: string GetRelatedPathName(...) - same path, new extension
		public string GetRelatedPathName(string sBaseAddition, string sExtension)
			// Returns a file name the same as FileName, except with the desired
			// extension. If there is not filename, we make up one.
		{
			// Get rid of leading '.' if it exists in sExtension
			if (sExtension.Length > 0 && sExtension[0] == '.')
				sExtension = sExtension.Substring(1);

			// If we don't have a good filename, then make up one
			if (FileName == "")
			{
				return InitialDirectory + "Default." + sExtension;
			}

			string sBaseName = Path.GetFileNameWithoutExtension(FileName);
			string sFileName = FileName.Substring(0, FileName.LastIndexOf(Path.DirectorySeparatorChar) ) 
                + Path.DirectorySeparatorChar + sBaseName + sBaseAddition + "." + sExtension;
			return sFileName;
		}
		#endregion
		#region Method: void New()    - blank configuration
		public void New()
		{
			SetToNoFileName();
			m_target.New();
			UpdateWindowTitle();
		}
		#endregion
		#region Method: void Open()   - load configuration data from file
		public void Open()
			// Presents the OpenFileDialog by which the user chooses a file. This dialog
			// defaults to the InitialDirectory if first starting, or the directory of
			// the currently open configuration if one has been previously saved or opened.
			// The callback function from m_App actually interprets the data, and must
			// be supplied.
		{
			string sFileNameSav = FileName;
			OpenFileDialog dlg = new OpenFileDialog();
			PrepareFileDialog(dlg);
			if(dlg.ShowDialog() == DialogResult.OK)
			{
				FileName = dlg.FileName;
				if (false == ReadFile())
					FileName = sFileNameSav;
			}
		}
		#endregion
		#region Method: void Save()   - save configuration data to file
		public void Save()
			// Does a save operation. If a filename does not exist, then SaveAs is
			// called so that the user can supply a name. (If he doesn't, then
			// the operation is canceled. The m_App callback actually writes out
			// the configuration, and must be supplied.
		{
			if (!IsUserSuppliedFileName)
				SaveAs("Save As");
			// If SaveAs failed to get a file name, then give up.
			if (!IsUserSuppliedFileName)
				return;

			// Write out the data (overwrite any existing file)
			m_target.Write(FileName);

            // Make sure the mru is up-to-date, in case the FileName was changed 
            // since the read was done (I'm struggling to get this to work right
            // when Rel Path Names are changed; 30jan08)
            mru_Update();
        }
		#endregion
		#region Method: void SaveAs() - prompt for file name, then do a save
		public bool SaveAs(string sDialogTitle)
			// Presents the SaveFileDialog to the user, by which he can choose a file
			// name for the save. The method then calls Save() to do the actually
			// write operation.
		{
			// Set up the SaveFileDialog
			SaveFileDialog dlg = new SaveFileDialog();
            if (!string.IsNullOrEmpty(sDialogTitle))
                dlg.Title = sDialogTitle;
			PrepareFileDialog(dlg);

			// If we got the name successfully, then store it and call onCmd_FileSave to
			// actually write out the data.
			if(dlg.ShowDialog() == DialogResult.OK)
			{
				FileName = dlg.FileName;
				Save();
				mru_Update();
				UpdateWindowTitle();
                return true;
			}

            return false;
		}
		#endregion

		// Private Implementational Methods --------------------------------------------------
		#region Private Method: void PrepareFileDialog(dlg) - set up the Open / Save dialogs
		private void PrepareFileDialog(FileDialog dlg)
			// The Open and the Save dialogs share the same superclass, and it is this
			// base class that has all of the settings for filters, etc. So we set it
			// up in this one place, so that both an Open and a Save will be 
			// consistent.
		{
			// Filter to MCS files
			dlg.Filter = m_sFileFilter;
			dlg.FilterIndex = 1;
			dlg.DefaultExt = m_sFileExtension;

			// Filename
			if (!IsUserSuppliedFileName)
			{
				dlg.InitialDirectory = InitialDirectory;
				dlg.FileName = DefaultFileName;
			}
			else
				dlg.FileName = FileName;
		}
		#endregion
		#region Private Method: void ReadFile() - read in the file's data
		private bool ReadFile()
			// We are already happy with the FileName; it is an existing file that we
			// know we want to open. So just read in the file.
		{
			try
			{
				string sFileName = FileName;
				m_target.Read(ref sFileName);
				FileName = sFileName;
				mru_Update();
				UpdateWindowTitle();
				return true;
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
				return false;
			}
		}
		#endregion

		// MRU Support -----------------------------------------------------------------------
		#region Attribute: g/s int MaxItemsOnMRU - how large to make the MRU
		public int MaxItemsOnMRU
		{ 
			get { return m_cmru_MaxItemsOnMenu; } 
			set { m_cmru_MaxItemsOnMenu = Math.Min(value,c_cMaxItemsOnMenu); } 
		}
		const int c_cMaxItemsOnMenu = 9;
		private int m_cmru_MaxItemsOnMenu = c_cMaxItemsOnMenu; 
		#endregion
		#region Method: void     LoadMRUfromRegistry() - Populates the File menu from the registry
		public void LoadMRUfromRegistry(bool bLoad)
			// Reads the MRU section in the registry and calls mru_Update to add them
			// to the File menu. We have to decrement through the keys in order that
			// mru_Update will place them in the correct order in the MRU.
		{
			// Get the number of items in the list of files. 
			int cEntries = JW_Registry.GetValue(c_sRegistrySubkey, "", 0);

			// Collect the entries from the Registry
			for(int i = cEntries; i > 0; i--)
			{
				FileName = JW_Registry.GetValue(c_sRegistrySubkey, i.ToString("000"), "");

				if (0 == FileName.Length)
					continue;

				// If we can't find the file on the disk, then we remove it from the MRU
				if ( ! File.Exists(FileName) )
					continue;

				mru_Update();
			}

			// Option to go ahead and load the most recent file
			if (bLoad && "" != FileName)
			{
				ReadFile();
			}
		}
		#endregion
		#region Method: void     SaveMRUtoRegistry()   - Stores the current MRU to registry
		public void SaveMRUtoRegistry()
			// Saves the current File menu MRU into the MRU section of the registry.
			// Any previous MRU entries are first deleted. 
		{
			// Get rid of anything previously there, so we don't worry about old entries
			// that need to be refreshed.
			JW_Registry.DeleteSubKey(c_sRegistrySubkey);

			// One registry entry for each full filename
			int i = 1;
			foreach( string s in m_rgPathNames )
			{
				JW_Registry.SetValue(c_sRegistrySubkey, i.ToString("000"), s);
				i++;
			}

			// Store the number of entries
			JW_Registry.SetValue(c_sRegistrySubkey, "", m_rgPathNames.Length);
		}
		#endregion
		#region Method: int      mru_GetFirstMRUPosition()
		public int mru_GetFirstMRUPosition(ToolStripMenuItem menuProject)
			// If we have an existing MRU, the form of the items is going to be
			// "N Item", where "N" is a digit. So all I need to do is count
			// backwards while I have this form, until I don't have that form.
			//    If the item I land on is not a separator, then we add a 
			// separator, as the implication is that we are about to add an MRU
			// and we want it separated from the rest of the File menu.
		{
			// First, decrement down to the first non-MRU item.
			int i = menuProject.DropDownItems.Count - 1;

			// Decrement past the "More..." item if it is there
			if (i >= 0)
			{
                ToolStripMenuItem menuMore = menuProject.DropDownItems[i] as ToolStripMenuItem;
                if (menuMore.Name == c_cmdMore)
					--i;
			}

			// Now examine the MRU items
			for(; i>0; i--)
			{
                ToolStripMenuItem menuItem = menuProject.DropDownItems[i] as ToolStripMenuItem;
                string s = menuItem.Text;
				if ( ! ( s.Substring(0,1) == "&" && 
					Char.IsDigit(s[1])   && 
					s.Substring(2,1) == " " ) )
				{
					break;
				}
			}

			// Increment to point to the first MRU item (which may not yet exist)
			return ++i;
		}
		#endregion
		#region Method: void     mru_Update() - Maintains the MRU list.
		public void mru_Update()
		{
			// Is the filename already in the array? If so, move it to the front of the
			// array, shifting everything else back.
			foreach(string s in m_rgPathNames)
			{
				if (s == FileName)
				{
					string[] rgs = new string[ m_rgPathNames.Length ];
					rgs[0] = FileName;

					int iDest = 1;
					for(int k=0; k<m_rgPathNames.Length; k++)
					{
						if (m_rgPathNames[k] != s)
							rgs[iDest++] = m_rgPathNames[k];
					}
					m_rgPathNames = rgs;

					return;
				}
			}

			// Otherwise, add the new filename to the beginning
			int cMax = Math.Min( m_rgPathNames.Length + 1, MaxItemsOnMRU + 26);
			string[] rg = new string[ cMax ];

			rg[0] = FileName;

			for(int i = 1; i<rg.Length; i++)
			{
				rg[i] = m_rgPathNames[i-1];
			}
			m_rgPathNames = rg;

		}
		#endregion
		#region Method: ButtonItem CreateMenuItem(int iPathName, onClick)
		private ToolStripMenuItem CreateMenuItem(int iPathName, EventHandler onClick)
		{
			// Retrieve the portion of the name that we will display to the user. This
			// is just the filename, with no path information
			string sFileName = Path.GetFileName( m_rgPathNames[iPathName] );

			// There will be a letter to the left on the menu, so that the item can
			// be selected from the keyboard. For the file menu, this will be a digit
			// from 1 to 9; for the submenu, it will be a letter from 'a' to 'z'.
			char chLetter = (iPathName < MaxItemsOnMRU) ? 
				(char)((int)'1' + iPathName) : 
				(char)((int)'a' + iPathName - MaxItemsOnMRU);

			// Build the string that will be in the menu
			string sMenuText = "&" + chLetter.ToString() + " " + sFileName;

			// Create the menu item
            ToolStripMenuItem menuItem = new ToolStripMenuItem(sMenuText, null, onClick);
            menuItem.Name = sMenuText;

			// Return the result to the caller
            return menuItem;
		}
		#endregion
        #region Method: void RemoveMRUItems(ToolStripDropDownButton btnProject)
        public void RemoveMRUItems(ToolStripDropDownButton btnProject)
        {
            // Remove everything after the Save As menu item
            int iExit = 0;
            for (; iExit < btnProject.DropDownItems.Count; iExit++)
            {
                if (btnProject.DropDownItems[iExit].Name == "m_menuSaveProjectAs")
                    break;
            }
            while (btnProject.DropDownItems.Count > iExit + 1)
                btnProject.DropDownItems.RemoveAt(iExit + 1);
        }
        #endregion
        #region Method: void BuildMRUPopupMenu(ToolStripDropDownButton btnProject, onClick, bool bVisible)
        public void BuildMRUPopupMenu(ToolStripDropDownButton btnProject, EventHandler onClick, bool bVisible)
		{
            // Remove everything after the Exit menu item
            int iExit = 0;
            for (; iExit < btnProject.DropDownItems.Count; iExit++)
            {
                if (btnProject.DropDownItems[iExit].Name == "m_menuSaveProjectAs")
                    break;
            }
            while (btnProject.DropDownItems.Count > iExit + 1)
                btnProject.DropDownItems.RemoveAt(iExit + 1);

			// Is the MRU supposed to be visible? If not, then don't process further
			if (!bVisible)
				return;

            // Add the separator
            btnProject.DropDownItems.Add(new ToolStripSeparator());

			// Index through the array
			int i = 0;

			// First, add the MRU on the main file menu
			for(; i<MaxItemsOnMRU && i<m_rgPathNames.Length; i++)
                btnProject.DropDownItems.Add(CreateMenuItem(i, onClick));

			// If any pathnames remain, add them to a submenu
			if (i < m_rgPathNames.Length)
			{
                ToolStripMenuItem menuMore = new ToolStripMenuItem(LanguageResources.MRU_More);
                btnProject.DropDownItems.Add(menuMore);

				for(; i< m_rgPathNames.Length; i++)
                    menuMore.DropDownItems.Add(CreateMenuItem(i, onClick));
			}

		}
		#endregion
        #region Method: void LoadMruItem(sMenuText) - event for MRU menu item
        public void LoadMruItem(string sMenuText) 
			// Event handler for when the user clicks on an item in the MRU. 
		{
            char chLetter = sMenuText[1];

			int i = 0;
			if (char.IsDigit(chLetter))
				i = (int)(chLetter - '1');
			else
				i = (int)(chLetter - 'a' + MaxItemsOnMRU);

			FileName = m_rgPathNames[i];
			ReadFile();
			mru_Update();
		}
		#endregion
	}

	#region Tests
	#region CLASS: Test_FileMenuIOForm - Used by testing
	public class Test_FileMenuIOForm : Form, IJW_FileMenuIO
	{
		#region Form Attributes
		private System.Windows.Forms.MainMenu m_menuMain;
		private System.Windows.Forms.MenuItem m_menuFile;
		private System.Windows.Forms.MenuItem m_miNew; 
		private System.Windows.Forms.MenuItem m_miOpen;
		private System.Windows.Forms.MenuItem m_miSave;
		private System.Windows.Forms.MenuItem m_miSaveAs;
		private System.Windows.Forms.MenuItem m_miExit;
		private System.ComponentModel.IContainer components;
		#endregion
		#region Constructor
		public Test_FileMenuIOForm()
		{
			InitializeForm();
			jw = new JW_FileMenuIO(this, this, "TestApp", 
				"test files (*.x)|*.x|All files (*.*)|*.*", "x", "Default.x");
		}
		#endregion
		#region Method: InitializeForm
		private void InitializeForm()
		{
			// Set up the form
			components = new System.ComponentModel.Container();
			m_menuMain = new System.Windows.Forms.MainMenu();
			m_menuFile = new System.Windows.Forms.MenuItem();
			m_miNew    = new System.Windows.Forms.MenuItem();
			m_miOpen   = new System.Windows.Forms.MenuItem();
			m_miSave   = new System.Windows.Forms.MenuItem();
			m_miSaveAs = new System.Windows.Forms.MenuItem();
			m_miExit   = new System.Windows.Forms.MenuItem();

			SuspendLayout();

			m_menuMain.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] 
				{m_menuFile} );

			m_menuFile.Index = 0;
			m_menuFile.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] 
				{m_miNew, m_miOpen, m_miSave, m_miSaveAs, m_miExit} );

			m_miNew.Index    = 0;
			m_miNew.Text     = "&New";
			m_miOpen.Index   = 1;
			m_miOpen.Text    = "&Open...";
			m_miSave.Index   = 2;
			m_miSave.Text    = "&Save";
			m_miSaveAs.Index = 3;
			m_miSaveAs.Text  = "Save &As...";
			m_miExit.Index   = 4;
			m_miExit.Text    = "E&xit";

			Menu = this.m_menuMain;
			ResumeLayout(false);
		}
		#endregion
		public JW_FileMenuIO jw;   // We initialize a single JW_FileMenuIO, and use it for all tests

		// Interface IFileMenuIO -------------------------------------------------------------
		#region Method: void New() - start with blank configuration
		public void New()
		{
		}
		#endregion
        #region Method: void Read(ref string sFileName) - read the configuration
        public void Read(ref string sFileName)
		{
		}
		#endregion
        #region Method: void Write(sPathName) - write the configuration
        public void Write(string sPathName)
		{
		}
		#endregion
	}
	#endregion

	public class Test_FileMenuIO : Test
	{
		string m_sSaveOriginalRootKey = "";
		string m_sRegistryRoot = "SOFTWARE\\The Seed Company\\Our Word Test";

		// Scaffolding -----------------------------------------------------------------------
		#region Constructor()
		public Test_FileMenuIO()
			: base("JW_FileMenuIO")
		{
			AddTest( new IndividualTest( Test_MRU_Additions ), "MRU Additions" );
		}
		#endregion
		#region Method: override void Setup()
		public override void Setup()
		{
			// Need a devoted registry area; save the original so we can restore it
			m_sSaveOriginalRootKey = JW_Registry.RootKey;
			JW_Registry.RootKey = m_sRegistryRoot;

			// Remove anything that was formerly in it
			if (Directory.Exists(TempFolder))
				Directory.Delete(TempFolder, true);

			// Create an empty directory for testing
			Directory.CreateDirectory(TempFolder);
		}
		#endregion
		#region Method: override void TearDown()
		public override void TearDown()
		{
			Registry.CurrentUser.DeleteSubKeyTree(m_sRegistryRoot);
			JW_Registry.RootKey = m_sSaveOriginalRootKey;
			Directory.Delete(TempFolder, true);
		}
		#endregion

		// Tests -----------------------------------------------------------------------------
		#region Test: Test_MRU_Additions
		public void Test_MRU_Additions()
		{
			Test_FileMenuIOForm f = new Test_FileMenuIOForm();

			// Create some filenames and corresponding files on the hard drive. We'll creaete
			// more than the capacity of 40, to test that overflow does not happen.
			int cFiles = 40;
			string[] rgFileName = new string[cFiles];
			for(int i = 0; i < cFiles; i++)
			{
				rgFileName[i] = TempFolder + Path.DirectorySeparatorChar + "FileMenuIO" + 
                    (i+1).ToString("00") + ".x";

				TextWriter tw = JW_Util.GetTextWriter( rgFileName[i] );
				tw.WriteLine(rgFileName[i]);
				tw.Close();
			}

			// Save the files to registry. Note we use reverse order, so that the
			// last file added is saved at the top of the list
			for(int i = cFiles - 1; i >=0; i--)
			{
				f.jw.FileName = rgFileName[i];
				f.jw.mru_Update();
			}
			f.jw.SaveMRUtoRegistry();

			// There should only be 35 files in the registry (9 digits + 26 letters).
			int cEntries = JW_Registry.GetValue(JW_FileMenuIO.c_sRegistrySubkey, "", 0);
			AreSame(35, cEntries);

			// The filenames, as stored in the registry, should correspond to the
			// bottom 26 names in the filenames array
			for(int i=0; i<35; i++)
			{
				string s = JW_Registry.GetValue(JW_FileMenuIO.c_sRegistrySubkey, 
					(i+1).ToString("000"), "");
				AreSame( rgFileName[i], s);
			}

			// Load the filenames from the registry; the names loaded should match
			// our array
			f.jw.LoadMRUfromRegistry(false);
			for(int i=0; i<35; i++)
			{
				AreSame( rgFileName[i], f.jw.PathNames[i] );
			}

			// Create a new insert file; add it to the MRU. 
			// It should now be the first filename there
			string sNewFileName = TempFolder + Path.DirectorySeparatorChar + "FileMenuIO_New.x";
			f.jw.FileName = sNewFileName;
			f.jw.mru_Update();
			AreSame(sNewFileName, f.jw.PathNames[0]);

			// Now set the filename to the 3rd file
			// The one we just created should have moved down one slot
			f.jw.FileName = f.jw.PathNames[2];
			f.jw.mru_Update();
			AreSame(sNewFileName, f.jw.PathNames[1]);
		}
		#endregion
	}

	#endregion
}
