#region ***** JObjectOnDemand.cs *****
/**********************************************************************************************
 * File:    JObjectOnDemand.cs
 * Author:  John Wimbish
 * Created: 27 Mar 2004
 * Purpose: A subclass of JObject, which stays on the disk rather than in memory until
 *           needed.
 * Legal:   Copyright (c) 2005-09, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
#region Header: Using, etc.
using System;
using System.Collections;
using System.Diagnostics;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using JWTools;
#endregion
#endregion

/* TASKS
 * - Error on file not found, followed by browse for file
 * 
 * - Ideally, the subclasses would Load automatically when any of their attributes are
 *    called. I spent a lot of time on this and was unsuccessful; some kind of wierd
 *    recursion that messes up things royally in the debugger. On reflection, decided
 *    that loading books in Scripture is something I can do pretty much without needing
 *    an automatic mechanism, so, KISS.
 */

namespace JWdb
{
	public class JObjectOnDemand : JObject
	{
		// BAttrs ----------------------------------------------------------------------------
		#region BAttr{g/s}: string DisplayName - the object's name as it appears in the UI
		public virtual string DisplayName
			// This name will appear in the UI, e.g., in an error message stating that
			// the object can't be found and asking if the user wants to navigate to it.
		{
			get
			{
				return m_sDisplayName;
			}
			set
			{
                SetValue(ref m_sDisplayName, value);
            }
		}
		private string m_sDisplayName;
		#endregion
        #region Method: void DeclareAttrs()
		protected override void DeclareAttrs()
		{
			base.DeclareAttrs();
			DefineAttr("DisplayName", ref m_sDisplayName);
        }
		#endregion
	
		// Derived Attributes ----------------------------------------------------------------
		#region VAttr(g): string SortKey - must be overridden if sorting is desired.
		public override string SortKey
			// In order to support sorting, the subclass must implement a SortKey attribute,
			// and this SortKey must return something other than an empty string. 
		{
			get 
			{ 
				return DisplayName; 
			}
		}
		#endregion
        #region VAttr{g}: virtual string FileFilter - override for BrowseForFile dialog
        protected virtual string FileFilter
        {
            get
            {
                return "";
            }
        }
        #endregion

        // Dirty - Need to Save? -------------------------------------------------------------
		#region Attr{g}: JObjectOnDemand SaveObj - return Save Obj that owns this object
		public override JObjectOnDemand SaveObj
		{
			get
			{
				return this;
			}
		}
		#endregion
		#region Attr{g/s}: bool IsDirty - does this obj (and its children) need to be saved?
		public bool IsDirty
		{
			get
			{
				return m_bIsDirty;
			}
			set
			{
                /***
                // Diagnostics: comment out unless needed
                if (this.GetType().ToString() == "OurWord.DataModel.DBook")
                {
                    string sDisplayName = DisplayName;
                    if (Owner != null && (Owner as JObjectOnDemand != null))
                        sDisplayName = (Owner as JObjectOnDemand).DisplayName + " - " + sDisplayName;
                   if ("English - Mark" == sDisplayName)
                    {
                        if (m_bIsDirty == false && value == true)
                            Console.WriteLine("DIRTY SET..." + sDisplayName);
                        if (m_bIsDirty == true && value == false)
                            Console.WriteLine("DIRTY CLEARED..." + sDisplayName);
                    }
                }
                ***/

                m_bIsDirty = value;
			}
		}
		private bool m_bIsDirty = false;
		#endregion
		#region Method: void DeclareDirty() - mark the Save Obj as needing to be saved
		public override void DeclareDirty()
		{
			IsDirty = true;

            // Declare everything up the hierarchy as dirty, too
            if (null != Owner)
                Owner.DeclareDirty();
		}
		#endregion

		// Scaffolding -----------------------------------------------------------------------
		#region Constructor()
		public JObjectOnDemand()
			: base()
		{
		}
		#endregion
		#region Constructor(sDisplayName)
		public JObjectOnDemand(string sDisplayName)
			: this()
		{
			DisplayName = sDisplayName;
		}
		#endregion

        // Load / Release / I/O --------------------------------------------------------------
		#region Attr{g}: bool Loaded - Whether or not the data has been loaded
		public bool Loaded
		{
			get 
			{
				return m_bIsLoaded;
			}
		}
		protected bool m_bIsLoaded = false;
		#endregion

        #region Method: void LoadFromFile(ref sPath, IProgressIndicator)
        public void LoadFromFile(ref string sPath, IProgressIndicator progress)
			// This version permits us to load the file with a different location
			// from that specified in the StoragePath. We use this, e.g., on
			// loading a new Project, or for our test suite.
		{
			// Nothing to do if it is already loaded, or no path we can deal with
			if (Loaded)
				return;
			if (string.IsNullOrEmpty(sPath))
				return;

			// Open and read the file, navigating to it if necessary
			TextReader tr = null;
			try
			{
				// Open the file. This may change the filename
				tr = JW_Util.GetTextReader(ref sPath, FileFilter);

				// Load it
				if (OnLoad(tr, sPath, progress))
					m_bIsLoaded = true;

				// Close the text reader
				tr.Close();
				tr = null;
			}
			catch (Exception)
			{
				if (null != tr)
					tr.Close();
				m_bIsLoaded = false;  // Should still be false, but make certain
				return;
			}
		}
		#endregion
        #region Method: void LoadFromFile(IProgressIndicator)
        public void LoadFromFile(IProgressIndicator progress)
			// This is the normal version we'll use, where we already know
			// the path and don't need it to be supplied.
		{
            // Nothing to do if it is already loaded
            if (Loaded)
                return;

            // Store a copy of the DisplayName going in
            string sOriginalDisplayName = DisplayName;

			// Call the path-aware version of Load
			string sPathActuallyLoaded = StoragePath;
            LoadFromFile(ref sPathActuallyLoaded, progress);

            // By default, we are using the DisplayName to come up with the path.
            // If the DisplayName as stored within the file is different from what
            // we thought prior to the load, it likely means something was changed
            // external to OurWord. E.g., using Windows Explorer and/or Notepad. 
            // If we are doing a Load, it means the DisplayName was known going in,
            // because we store these in the parent object. 
            //    So I restore it if needed. This came up in a crash, where I had
            // created a new cluster by renaming an old one, and renaming the OWT
            // file. Then calling "New Project", which correctly went to load the
            // OWT file in the right place; but because there was a different
            // displayname, the StoragePath was different after the load, and
            // the File.Move code below was triggered (and failed).
            DisplayName = sOriginalDisplayName;

			// If the filename was changed, then we need to move the file to where
			// it belongs, and then try again
            if (sPathActuallyLoaded != StoragePath)
			{
				Unload(progress);
                File.Move(sPathActuallyLoaded, StoragePath);
				LoadFromFile(progress);
			}
		}
		#endregion
        #region Method: virtual bool OnLoad(TextReader, IProgressIndicator)
        protected virtual bool OnLoad(TextReader tr, string sPath, IProgressIndicator progress)
        {
            XElement[] vx = XElement.CreateFrom(tr);
            if (vx.Length != 1)
                return false;

            // Populate the owning object hierarchy
            FromXml(vx[0]);

            // Set any Reference attributes
            ResolveReferences();

            return true;
        }
        #endregion

        #region Method: void Unload(IProgressIndicator)
        public void Unload(IProgressIndicator progress)
        {
            if (Loaded)
            {
                WriteToFile(progress);
                Clear();
                m_bIsLoaded = false;
            }
        }
        #endregion

        #region Method: void WriteToFile() - topmost level Write, writes all data
        public virtual void WriteToFile(IProgressIndicator progress)
		{
            // Give the owned JObjectOnDemand's an opportunity to be written
            foreach (JAttr attr in AllAttrs)
                attr.WriteOwnedObjectsOnDemand();

            // Do nothing if no changes have been made
            if (!IsDirty)
                return;

			// Create a backup file by copying this one, if it exists
            string sExtension = Path.GetExtension(StoragePath);
			string sBackupExtension = sExtension + "bak";
			string sBackupPath = JW_Util.CreateBackup(StoragePath, sBackupExtension);

            // Perform JObject built-in XML write
			TextWriter W = null;
			try
			{
                // Create an Xml representation of the object
                XElement x = ToXml(true);

				// Create the directory if it doesn't exist
				string sFolder = Path.GetDirectoryName(StoragePath);
				if (!Directory.Exists(sFolder))
					Directory.CreateDirectory(sFolder);

				// Open the file for writing
                W = JW_Util.GetTextWriter(StoragePath);

                // Write it out
                x.Out(W, 0);

				// Done writing
				W.Close();
			}
			catch (Exception)
			{
				// Make sure the file is closed
				try { if (null != W) W.Close(); }
				catch(Exception) {}

				// Restore from the backup
				JW_Util.RestoreFromBackup(StoragePath, sBackupExtension);
			}

			// Remove the backup file now that, one way or another, we're done with it.
			if (File.Exists(sBackupPath))
				File.Delete(sBackupPath);
        }
        #endregion

        #region Method: void InitialCreation(IProgressIndicator)
        public virtual void InitialCreation(IProgressIndicator progress)
            // Done as part of Project-New, we load a file if it exists; but otherwise
            // make sure its directory and settings file are present and ready.
        {
            // If the file exists where we expect it, then load its settings
            if (File.Exists(StoragePath))
            {
                LoadFromFile(progress);
                if (Loaded)
                    return;
            }

            // Otherwise, create its directory, and write its (most likely empty) file
            string sFolder = Path.GetDirectoryName(StoragePath);
            if (!Directory.Exists(sFolder))
                Directory.CreateDirectory(sFolder);
            WriteToFile(progress);
        }
        #endregion

        #region VAttr{g}: virtual string DefaultFileExtension - e.g., ".owp"
        public virtual string DefaultFileExtension
        {
            get
            {
                return ".txt";
            }
        }
        #endregion
		#region VAttr{g}: string StorageFileName
		public virtual string StorageFileName
		{
			get
			{
				// Make sure we'll have a valid filename; which we can't do if the
				// display name is empty
				Debug.Assert(!string.IsNullOrEmpty(DisplayName));

				return DisplayName + DefaultFileExtension;
			}
		}
		#endregion
		#region Vattr{g}: string StoragePath - full path for this obj's file
		virtual public string StoragePath
			// As of OurWord2, we now place files in fixed places on the disk, rather
			// than permit the user to specify the location. 
		{
			get
			{
				Debug.Assert(false, "Subclass must override StoragePath");
				return JWU.GetMyDocumentsFolder(null) + StorageFileName;
			}
		}
		#endregion

        // Merge -----------------------------------------------------------------------------
	}

}
