/**********************************************************************************************
 * Dll:     JWTools
 * File:    JW_UI.cs
 * Author:  John Wimbish
 * Created: 16 Dec 2004
 * Purpose: User Interface stuff
 * Legal:   Copyright (c) 2005-09, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/

#region Using
using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Win32;
#endregion

namespace JWTools
{
	#region CLASS: LanguageResources - Base class for all of these
	public class LanguageResources
	{
		// Language --------------------------------------------------------------------------
		public enum Languages { English=0, Indonesian, Spanish, Swahili, kLast };
		#region Attr{g/s}: Languages Language
		static public Languages Language
		{
			get
			{
				return m_Language;
			}
			set
			{
				m_Language = value;
			}
		}
		static Languages m_Language = Languages.English;
		#endregion
		#region Method: static Initialize(sRegistrySubKey, sName)
		static public void Initialize(string sRegistrySubKey, string sName)
		{
			Language = (Languages) JW_Registry.GetValue(sRegistrySubKey,
				sName, (int)Languages.English);
		}
		#endregion

		// Resources -------------------------------------------------------------------------
		#region Attr{g}: static string AppTitle
		static string[] AppTitleAlts =
		{
			"Our Word",
			"Sabda Kita",
            "Palabra Nuestra",
            "Neno Letu",
            "คามคองราว"
		};

		public static string AppTitle
		{
			get
			{
				return GetAlternative(AppTitleAlts);
			}
		}
		#endregion
		#region Attr{g}: static string[] LanguageNames
		static public string[] LanguageNames =
			{
				"English",
				"Bahasa Indonesia",
                "Español",
                "Kiswahili"
			};
		#endregion
		#region Attr{g}: static string LanguageName
		static public string LanguageName
		{
			get
			{
				return GetAlternative(LanguageNames);
			}
		}
		#endregion
		#region Method: GetLanguageName(Languages lang)
		public static string GetLanguageName(Languages lang)
		{
			return GetAlternative(LanguageNames, lang);
		}
		#endregion
		#region Method: static  Languages GetLanguage(string sLanguageName)
		static public Languages GetLanguage(string sLanguageName)
		{
			for(int i=0; i< (int)Languages.kLast; i++)
			{
				if (LanguageNames[i] == sLanguageName)
					return (Languages)i;
			}
			return Languages.English;
		}
		#endregion

		// Methods ---------------------------------------------------------------------------
		#region Method: string Insert(string sBase, string sInsert)
		static public string Insert(string sBase, string sInsert1)
		{
			string sReturn = sBase;

			// Substitution
			int iPos = sReturn.IndexOf("{0}");
			if (iPos >= 0)
			{
				string sFirst = sReturn.Substring(0, iPos);
				string sLast = sReturn.Substring(iPos+3);
				sReturn = sFirst + sInsert1 + sLast;
			}

			return sReturn;
		}
		#endregion
		#region Method: string Insert(string sBase, string sInsert, string sInsert2)
		static public string Insert(string sBase, string sInsert1, string sInsert2)
		{
			string sReturn = sBase;

			// First substitution
			int iPos = sReturn.IndexOf("{0}");
			if (iPos >= 0)
			{
				string sFirst = sReturn.Substring(0, iPos);
				string sLast = sReturn.Substring(iPos+3);
				sReturn = sFirst + sInsert1 + sLast;
			}

			// Second substitution
			iPos = sReturn.IndexOf("{1}");
			if (iPos >= 0)
			{
				string sFirst = sReturn.Substring(0, iPos);
				string sLast = sReturn.Substring(iPos+3);
				sReturn = sFirst + sInsert2 + sLast;
			}

			return sReturn;
		}
		#endregion
		#region Method: string Insert(string sBase, string[] vsInsert)
		static public string Insert(string sBase, string[] v)
		{
			string sReturn = sBase;

			for(int i=0; i<v.Length; i++)
			{
				int iPos = sReturn.IndexOf("{" + i.ToString() + "}");
				if (iPos >= 0)
				{
					string sFirst = sReturn.Substring(0, iPos);
					string sLast = sReturn.Substring(iPos + 3);
					sReturn = sFirst + v[i] + sLast;
				}
			}

			return sReturn;
		}
		#endregion

		#region Method: string GetAlternative( string[] Alts )
		static protected string GetAlternative( string[] Alts )
		{
			return GetAlternative( Alts, Language );
		}
		#endregion
		#region Method: string GetAlternative( string[] Alts, Languages Lang )
		static protected string GetAlternative( string[] Alts, Languages Lang )
		{
			// Handle the case where we forgot to add the alternative. We don't want to fire
			// an assertion; because some of these might occur as error conditions that we
			// the user (rather than me) will be the first to discover.
			if ( (int)Lang >= Alts.Length )
			{
				Console.WriteLine("Resource Not Found for " + Alts[ (int)Languages.English ]);
				return Alts[ (int)Languages.English ];
			}

			// Get the desired string
			string s = Alts[ (int)Lang ];

			// If it has zero length, then we use English as the default
			if (0 == s.Length)
				return Alts[ (int)Languages.English ];

			// Return the resultant string
			return s;
		}
		#endregion

		// Misc JW Stuff ---------------------------------------------------------------------
		#region string: More - The MRU's more menu
		static public string MRU_More
		{
			get
			{
				string[] res = 
					{
						"&More...",
						"",
                        "&Más..."
					};
				return GetAlternative(res);
			}
		}
		#endregion
	}
	#endregion

	#region DIALOG SUPERCLASS: DlgRes - Some common dialog button labels
	public class DlgRes : LanguageResources
	{
		#region Button: OK
		static public string OK
		{
			get
			{
				string[] res = 
					{
						"OK",
						""
					};
				return GetAlternative(res);
			}
		}
		#endregion
		#region Button: Cancel
		static public string Cancel
		{
			get
			{
				string[] res = 
					{
						"Cancel",
						"Batal"
					};
				return GetAlternative(res);
			}
		}
		#endregion
		#region Button: Help
		static public string Help
		{
			get
			{
				string[] res =         // Add two leading spaces to leave room for the icon
					{
						"  Help...",
						"  Tolong...",
                        "  Ayuda..."
					};
				return GetAlternative(res);
			}
		}
		#endregion
		#region Button: Close
		static public string Close
		{
			get
			{
				string[] res = 
					{
						"Close",
						"Tutup",
                        "Cerrar"
					};
				return GetAlternative(res);
			}
		}
		#endregion

		#region Button: Browse
		static public string Browse
		{
			get
			{
				string[] res = 
					{
						"Browse...",
						"Cari...",
                        "Navegar..."
					};
				return GetAlternative(res);
			}
		}
		#endregion

		#region Button: Create
		static public string Create
		{
			get
			{
				string[] res = 
					{
						"Create...",
						"Cipta Baru..."
					};
				return GetAlternative(res);
			}
		}
		#endregion
		#region Button: Properties
		static public string Properties
		{
			get
			{
				string[] res = 
					{
						"Properties...",
						"Ciri-ciri..."
					};
				return GetAlternative(res);
			}
		}
		#endregion
		#region Button: Add
		static public string Add
		{
			get
			{
				string[] res = 
					{
						"Add...",
						"Tambah..."
					};
				return GetAlternative(res);
			}
		}
		#endregion
		#region Button: Remove
		static public string Remove
		{
			get
			{
				string[] res = 
					{
						"Remove...",
						"Keluarkan..."
					};
				return GetAlternative(res);
			}
		}
		#endregion
		#region Button: Open
		static public string Open
		{
			get
			{
				string[] res = 
					{
						"Open...",
						"Buka..."
					};
				return GetAlternative(res);
			}
		}
		#endregion
		#region Button: Import
		static public string Import
		{
			get
			{
				string[] res = 
					{
						"Import...",
						"Buka Lama..."
					};
				return GetAlternative(res);
			}
		}
		#endregion
		#region Label: Comment
		static public string Comment
		{
			get
			{
				string[] res = 
					{
						"Comment:",
						"Catatan:"
					};
				return GetAlternative(res);
			}
		}
		#endregion

        #region Button: Next
        static public string Next
        {
            get
            {
                string[] res = 
					{
						"Next >>",
						"Berikut >>"
					};
                return GetAlternative(res);
            }
        }
        #endregion
        #region Button: Previous
        static public string Previous
        {
            get
            {
                string[] res = 
					{
						"<< Previous",
						"<< Dulu"
					};
                return GetAlternative(res);
            }
        }
        #endregion
        #region Button: Finish
        static public string Finish
        {
            get
            {
                string[] res = 
					{
						"Finish",
						"Sudah"
					};
                return GetAlternative(res);
            }
        }
        #endregion

	}
	#endregion

}
