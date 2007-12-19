/**********************************************************************************************
 * Dll:     JWTools
 * File:    JW_Help.cs
 * Author:  John Wimbish
 * Created: 06 Dec 2004
 * Purpose: Help system.
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
	class HelpTopic
	{
		#region Attr{g}: int ID - the ID of the topic; should be unique within the list
		public int ID
		{
			get
			{
				return m_nID;
			}
		}
		int    m_nID;
		#endregion
		#region Attr{g}: string Url - the URL that corresponds to the ID
		public string Url
		{
			get
			{
				Debug.Assert(null != m_sUrl);
				Debug.Assert(0 != m_sUrl.Length);
				return m_sUrl;
			}
		}
		string m_sUrl;
		#endregion

		#region Constructor(nID, sUrl)
		public HelpTopic(int nID, string sUrl)
		{
			m_nID = nID;
			m_sUrl = sUrl;
		}
		#endregion
	}


	public class JW_Help
	{
		// Attrs -----------------------------------------------------------------------------
		#region Attr{g}: Form Form - the application's window
		static public Form Form
		{
			get
			{
				Debug.Assert(null != s_Form);
				return s_Form;
			}
		}
		static Form s_Form = null;
		#endregion
		#region Attr{g}: string FileName
		static public string FileName
		{
			get
			{
				Debug.Assert(null != s_FileName);
				Debug.Assert(0 != s_FileName.Length);
				return s_FileName;
			}
		}
		static string s_FileName = "";
		#endregion
		#region Attr{g/s}: static string RegistryHelpFile
		static public string RegistryHelpFile
		{
			get
			{
				string sPath =  JW_Registry.GetValue(c_sRegName, "");

				// If we got nothing, we need to prompt the user; if the user chickens out
				// (via the Cancel button), then we cannot supply help.
				if (0 == sPath.Length)
				{
					OpenFileDialog dlg = new OpenFileDialog();

					// We only want to return a single file
					dlg.Multiselect = false;

					// Default to the install directory
					dlg.FileName = Path.GetDirectoryName(Application.ExecutablePath) +
						"\\" + FileName;

					// Filter on oTrans files
					dlg.Filter = "Help Files (*.chm)|*.chm";
					dlg.FilterIndex = 1;

					// Retrieve Dialog Title from resources
					dlg.Title = "Locate the \"" + FileName + "\" help file";

					if (DialogResult.OK == dlg.ShowDialog(Form))
					{
						sPath = dlg.FileName;
						RegistryHelpFile = dlg.FileName;
					}
				}

				return sPath;
			}
			set
			{
				JW_Registry.SetValue(c_sRegName, value);
			}
		}
		const string c_sRegName  = "HelpFile";
		#endregion
		#region Attr{g}: ArrayList Topics - the list of help topics
		static public ArrayList Topics
		{
			get
			{
				Debug.Assert(null != s_Topics);
				return s_Topics;
			}
		}
		static ArrayList s_Topics = null;
		#endregion

		// Topics ----------------------------------------------------------------------------
		const int c_nJWToolsReservedLo = 30000;
		const int c_nJWToolsReservedHi = 31000;
		#region enum JWTopic ID's
		public enum JWTopic
		{
			kDlgSetupFeatures = c_nJWToolsReservedLo,
			kDlgPasswordProtect,
			kLast
		}
		#endregion

		// Setup -----------------------------------------------------------------------------
		#region Method: void Initialize(AppForm)
		static public void Initialize(Form form, string sFileName)
		{
			s_Topics   = new ArrayList();
			s_FileName = sFileName;
			s_Form     = form;

			// Initialize topics for the JWTools DLL
			AddTopic( JWTopic.kDlgSetupFeatures, 
				"Reference\\Dialogs\\Features\\Dlg-Features.html");
			AddTopic( JWTopic.kDlgPasswordProtect, 
				"Reference\\Dialogs\\Features\\Dlg-PasswordProtect.html");
		}
		#endregion
		#region Method: void AddTopic(JWTopic nID, string sUrl)
		static private void AddTopic(JWTopic nID, string sUrl)
		{
			// Make sure the ID isn't already in the list
			foreach(HelpTopic t in Topics)
			{
				if (t.ID == (int)nID)
				{
					Debug.Assert(false, "Topic ID already exists: " + nID.ToString() +
						" - " + sUrl);
				}
			}

			// Add the ID
			Topics.Add( new HelpTopic((int)nID, sUrl) );
		}
		#endregion
		#region Method: void AddTopic(int nID, string sUrl)
		static public void AddTopic(int nID, string sUrl)
		{
			// Make sure the ID's are not in our reserved range
			if (nID >= c_nJWToolsReservedLo && nID < c_nJWToolsReservedHi)
			{
				Debug.Assert(false, "This range of IDs is reserved for JW_Tools.");
			}

			// Make sure the ID isn't already in the list
			foreach(HelpTopic t in Topics)
			{
				if (t.ID == nID)
				{
					Debug.Assert(false, "Topic ID already exists: " + nID.ToString() +
						" - " + sUrl);
				}
			}

			// Add the ID
			Topics.Add( new HelpTopic(nID, sUrl) );
		}
		#endregion

		// Show help topics ------------------------------------------------------------------
		#region Method: static void ShowDefaultTopic()
		static public void ShowDefaultTopic()
		{
			Help.ShowHelp(Form, RegistryHelpFile);
		}
		#endregion
		#region Method: void ShowTopic(int nID)
		static public void ShowTopic(int nID)
		{
			foreach(HelpTopic topic in Topics)
			{
				if (topic.ID == nID)
				{
					Help.ShowHelp(Form, RegistryHelpFile, topic.Url);
					return;
				}
			}

			ShowDefaultTopic();
		}
		#endregion

		// Individual topics -----------------------------------------------------------------
		#region Method: void Show_DlgFeatures()
		static public void Show_DlgFeatures()
		{
			ShowTopic((int)JWTopic.kDlgSetupFeatures);
		}
		#endregion
		#region Method: void Show_DlgPasswordProtect()
		static public void Show_DlgPasswordProtect()
		{
			ShowTopic((int)JWTopic.kDlgPasswordProtect);
		}
		#endregion
	}
}
