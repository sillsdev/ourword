/**********************************************************************************************
 * Dll:      JWTools
 * File:     JW_FileMenuIO.cs
 * Author:   John Wimbish
 * Created:  03 Oct 2003
 * Modified: 01 Jul 2004 - Converted to DotNetBar (tests are now broken)
 * Purpose:  Manages a list of files, both as an MRU, and writing to/from the registry, for
 *           the File menu commands (Open, Save, etc.)
 * Legal:   Copyright (c) 2005-09, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
#region Using
using System;
using System.Collections.Generic;
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
	public class MRU
	{
		// Attrs -----------------------------------------------------------------------------
		#region Attr{g/s}: int MaxItemsOnMRU - how large to make the MRU
		public int MaxItemsOnMRU
		{
			get 
			{
				return m_cMaxItemsOnMenu; 
			}
			set 
			{
				m_cMaxItemsOnMenu = Math.Min(value, c_cMaxItemsOnMenu); 
			}
		}
		const int c_cMaxItemsOnMenu = 9;
		private int m_cMaxItemsOnMenu = c_cMaxItemsOnMenu;
		#endregion
		#region Attr{g}: List<string> Items
		List<string> Items
		{
			get
			{
				Debug.Assert(null != m_vItems);
				return m_vItems;
			}
		}
		List<string> m_vItems = new List<string>();
		#endregion
		#region VAttr{g}: string TopItem
		public string TopItem
		{
			get
			{
				if (Items.Count > 0)
					return Items[0];
				return null;
			}
		}
		#endregion

		// Registry Persistance --------------------------------------------------------------
		public const string c_sRegistrySubkey = "MRU";
		#region Method: void LoadFromRegistry()
		public void LoadFromRegistry()
		{
			// Get the number of items in the list of files. 
			int cEntries = JW_Registry.GetValue(c_sRegistrySubkey, "", 0);

			// Collect the entries from the Registry
			for (int i = 0; i < cEntries; i++)
			{
				string sPathName = JW_Registry.GetValue(c_sRegistrySubkey, (i+1).ToString("000"), "");
				if (string.IsNullOrEmpty(sPathName))
					continue;

				// If we can't find the file on the disk, then we remove it from the MRU
				if (!File.Exists(sPathName))
					continue;

				Items.Add(sPathName);
			}
		}
		#endregion
		#region Method: void SaveToRegistry()
		public void SaveToRegistry()
		{
			// Get rid of anything previously there, so we don't worry about old entries
			// that need to be refreshed.
			JW_Registry.DeleteSubKey(c_sRegistrySubkey);

			// One registry entry for each full filename
			int i = 1;
			foreach (string s in Items)
			{
				JW_Registry.SetValue(c_sRegistrySubkey, i.ToString("000"), s);
				i++;
			}

			// Store the number of entries
			JW_Registry.SetValue(c_sRegistrySubkey, "", Items.Count);
		}
		#endregion

		// Manipulations ---------------------------------------------------------------------
		#region Method: void MoveToTop(sFileName)
		public void MoveToTop(string sFileName)
		{
			// Remove it from the list if it is currently there
			int iPos = Items.IndexOf(sFileName);
			if (iPos != -1)
				Items.RemoveAt(iPos);

			// Put it at the top
			Items.Insert(0, sFileName);
		}
		#endregion
		#region Method: void ReplaceTop(string sFileName)
		public void ReplaceTop(string sFileName)
		{
			if (Items.Count > 0)
				Items.RemoveAt(0);

			Items.Insert(0, sFileName);
		}
		#endregion
		#region Method: void AddAtTop(string sFileName)
		public void AddAtTop(string sFileName)
		{
			if (Items.IndexOf(sFileName) != -1)
				MoveToTop(sFileName);
			else
				Items.Insert(0, sFileName);
		}
		#endregion

		// Menu ------------------------------------------------------------------------------
		#region Method: ButtonItem CreateMenuItem(int iPathName, onClick)
		private ToolStripMenuItem CreateMenuItem(char chLetter, string sPathName, EventHandler onClick)
		{
			// Retrieve the portion of the name that we will display to the user. This
			// is just the filename, with no path information
			string sFileName = Path.GetFileName(sPathName);

			// Build the string that will be in the menu
			string sMenuText = "&" + chLetter.ToString() + " " + sFileName;

			// Create the menu item
			ToolStripMenuItem menuItem = new ToolStripMenuItem(sMenuText, null, onClick);
			menuItem.Name = sMenuText;
			menuItem.Tag = sPathName;

			// Return the result to the caller
			return menuItem;
		}
		#endregion
		#region Method: void RemoveMruItems(ToolStripDropDownButton)
		public void RemoveMruItems(ToolStripDropDownButton btn)
		{
			// Find the separator
			int iSeparator = 0;
			foreach (ToolStripItem item in btn.DropDownItems)
			{
				if (item as ToolStripSeparator != null)
					break;
				iSeparator++;
			}

			// If we didn't have one, then add it
			if (iSeparator == btn.DropDownItems.Count)
			{
				btn.DropDownItems.Add(new ToolStripSeparator());
			}

			// Clear out anything after the separator
			while (btn.DropDownItems.Count > iSeparator + 1)
			{
				btn.DropDownItems.RemoveAt(iSeparator + 1);
			}
		}
		#endregion
		#region Method: void BuildPopupMenu(ToolStripDropDownButton btnProject, onClick)
		public void BuildPopupMenu(ToolStripDropDownButton btnProject, EventHandler onClick)
		{
			// Remove the previous items
			RemoveMruItems(btnProject);

			// Add the MRU items
			char chLetter = '1';
			ToolStripMenuItem menuMore = null;
			foreach (string s in Items)
			{
				ToolStripMenuItem menu = CreateMenuItem(chLetter, s, onClick);
				if (null != menuMore)
					menuMore.DropDownItems.Add(menu);
				else
					btnProject.DropDownItems.Add(menu);

				if (chLetter == '9')
				{
				    menuMore = new ToolStripMenuItem(LanguageResources.MRU_More);
					btnProject.DropDownItems.Add(menuMore);
					chLetter = 'a';
				}
				chLetter++;
			}
		}
		#endregion
	}


	#region Tests - NEED TO REWORK FOR THE NEW "MRU" CLASS
	/***
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
	***/
	#endregion
}
