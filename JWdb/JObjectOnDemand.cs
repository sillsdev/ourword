/**********************************************************************************************
 * App:     Josiah
 * File:    JObjectOnDemand.cs
 * Author:  John Wimbish
 * Created: 27 Mar 2004
 * Purpose: A subclass of JObject, which stays on the disk rather than in memory until
 *           needed.
 * Legal:   Copyright (c) 2005-07, John S. Wimbish. All Rights Reserved.  
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
		#region BAttr{g/s}: (private) FileType FileFormat (XML, Shoebox, Paratext)
		private enum FileType { Shoebox = 0, XML, Paratext };
		private FileType FileFormat
		{
			get
			{
				return (FileType)m_nFileFormat;
			}
			set
			{
                SetValue(ref m_nFileFormat, (int)value);
            }
		}
		private int m_nFileFormat = (int)FileType.Shoebox;
		#endregion
		#region BAttr{g/s}: string DisplayName - the object's name as it appears in the UI
		public string DisplayName
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
        #region BAttr{g/s}: string RelativePathName - The relative filename under which the object is stored
        public string RelativePathName
            // Leave public, even though only JObjectOnDemand calls this, because we're getting
            // a MethodAccessException otherwise.
        {
            get
            {
                return m_sRelativePathName;
            }
        }
        private string m_sRelativePathName;
        #endregion
        #region Method: void DeclareAttrs()
		protected override void DeclareAttrs()
		{
			base.DeclareAttrs();
			DefineAttr("FileFormat",  ref m_nFileFormat);
			DefineAttr("DisplayName", ref m_sDisplayName);
            DefineAttr("Path",        ref m_sAbsolutePathName);
            DefineAttr("RelPath",     ref m_sRelativePathName);
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

        // Runtime Attrs ---------------------------------------------------------------------
		#region Attr{g/s}: string AbsolutePathName - The filename under which the object is stored
        public string AbsolutePathName
		{
			get
			{
                return m_sAbsolutePathName;
			}
			set
			{
                if (m_sAbsolutePathName != value)
                {
                    m_sAbsolutePathName = value;
                    DeclareDirty();
                }

                // Update the Relative Pathname if necessary
                if (null != Owner)
                {
                    string sRelativePathName = PathConverter.AbsoluteToRelative(
                        Owner.SaveObj.AbsolutePathName,
                        AbsolutePathName);

                    // If there was no change, then we don't need to update (as an update
                    // will trigger a file-save of both the object and its owning object.)
                    if (sRelativePathName != RelativePathName)
                    {
                        /****
                        MessageBox.Show("ABSOLUTE -- \n" +
                            "DisplayName = <" + this.DisplayName + ">\n" +
                            "Owner's ABS name = <" + Owner.SaveObj.AbsolutePathName + ">\n" +
                            "This's ABS name  = <" + AbsolutePathName + ">\n" +
                            "This's REL name  = <" + RelativePathName + ">\n" +
                            "Proposed REL nam = <" + sRelativePathName + ">", 
                            "OurWord Temporary Debugging", 
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        ****/

                        SetValue(ref m_sRelativePathName, sRelativePathName);

                        // Se we'll save the relative pathname
                        DeclareDirty();
                    }
                }
			}
		}
        private string m_sAbsolutePathName;
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
			: base()
		{
			DisplayName = sDisplayName;
		}
		#endregion
		#region Constructor(sDisplayName, sPath)
        public JObjectOnDemand(string sDisplayName, string sAbsolutePathName)
			: base()
		{
			DisplayName = sDisplayName;
            AbsolutePathName = sAbsolutePathName;
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

        // Load / Release / I/O --------------------------------------------------------------
		#region Attr{g/s}: bool Loaded - determines whether or not the data has been loaded
		public bool Loaded
		{
			get 
			{
				return m_bIsLoaded;
			}
		}
		protected bool m_bIsLoaded = false;
		#endregion

        #region Method: void Load()
        public void Load()
		{
            // No point if it is already loaded
            if (Loaded)
                return;

            // The "OriginPath" is the absolute path of the owning SaveObj
            string sOriginPath = "";
            if (Owner != null)
                sOriginPath = Owner.SaveObj.AbsolutePathName;

            // Compute a pathname from the relative path if possible
            string sPathName = PathConverter.RelativeToAbsolute(sOriginPath, RelativePathName);
            if (string.IsNullOrEmpty(sPathName))
                sPathName = AbsolutePathName;

            // If we still have nothing, then build something from the display name
            if (string.IsNullOrEmpty(sPathName))
                sPathName = DisplayName + DefaultFileExtension;

            // Open and read the file, navigating to it if necessary
            try
            {
                if (OnLoad(ref sPathName))
                    m_bIsLoaded = true;
            }
            catch (Exception e)
            {
                m_bIsLoaded = false;  // Should still be false, but make certain
                return;
            }

            // Note that the file, now that it is read in, will be considered "Dirty" and
            // in need of save. The DBook override of OnLoad clears this; but we don't
            // do that here because path names may have been changed, and we want to be
            // certain they get saved. It doesn't take long to save these settings files,
            // so it really isn't a performance issue.
           
            // Store the Absolute and Relative Paths (in case they changed)
            // (Probably no longer needed, now that we do this in the OnLoad method; but
            // because OnLoad can be overridden in subclasses, I'm leaving it in because
            // it is important that it be called.)
            AbsolutePathName = sPathName;
		}
		#endregion
        #region Method: virtual bool OnLoad(ref string sAbsolutePathName)
        protected virtual bool OnLoad(ref string sAbsolutePathName)
        {
            XElement[] vx = XElement.CreateFrom(ref sAbsolutePathName, FileFilter);
            if (vx.Length != 1)
                return false;

            // Store the Absolute and Relative Paths (in case they changed). We MUST do it here,
            // because ObjOnDemands may embed other ObjOnDemands, and attempts to read these
            // lower level ones via the Read method below need for this upper-level JObjOnDemand
            // to have its path name properly set.
            AbsolutePathName = sAbsolutePathName;

            // Populate the owning object hierarchy
            FromXml(vx[0]);

            // Set any Reference attributes
            ResolveReferences();

            return true;
        }
        #endregion

        #region Method: void Unload()
        public void Unload()
        {
            if (Loaded)
            {
                Write();
                Clear();
                m_bIsLoaded = false;
            }
        }
        #endregion

		#region Method: void Write() - topmost level Write, writes all data
		public void Write()
		{
            // Give the owned JObjectOnDemand's an opportunity to be written
            foreach (JAttr attr in AllAttrs)
                attr.WriteOwnedObjectsOnDemand();

            // Do nothing if no changes have been made
            if (!IsDirty)
                return;

            // Perform the object-specific write; default is our 
            // JObject built-in XML write
            OnWrite();
		}
		#endregion
        #region Method: virtual void OnWrite()
        protected virtual void OnWrite()
        {
			// Create a backup file by copying this one, if it exists
            string sExtension = Path.GetExtension(AbsolutePathName);
			string sBackupExtension = sExtension + "bak";
            JW_Util.CreateBackup(AbsolutePathName, sBackupExtension);

			TextWriter W = null;
			try
			{
                // Create an Xml representation of the object
                XElement x = ToXml(true);

				// Open the file for writing
                W = JUtil.GetTextWriter(AbsolutePathName);

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
                JW_Util.RestoreFromBackup(AbsolutePathName, sBackupExtension);
			}
        }
        #endregion
	}

}
