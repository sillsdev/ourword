/**********************************************************************************************
 * Dll:     JWTools
 * File:    JW_Registry.cs
 * Author:  John Wimbish
 * Created: 03 Oct 2003
 * Purpose: Provides a shorthand for frequent Registry operations.
 * Legal:   Copyright (c) 2005-09, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
#region Using
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Security.Permissions;
using Microsoft.Win32;
#endregion

namespace JWTools
{
	// JW_Registry Exceptions ----------------------------------------------------------------
	#region Exception: InvalidRootKeyException - RootKey has not been set
	public class InvalidRootKeyException: ApplicationException
	{
		static private string m_sError = "JWTools.JW_Registry: RootKey must be set before " +
			"using methods in this class.";

		public InvalidRootKeyException()
			: base(m_sError)
		{
		}
		public InvalidRootKeyException(string message)
			: base(m_sError + " - " + message)
		{
		}
		public InvalidRootKeyException(string message, Exception inner)
			: base(m_sError + " - " + message, inner)
		{
		}
	}
	#endregion

	// Class JW_Registry ---------------------------------------------------------------------
	public class JW_Registry
	{
		// Attributes ------------------------------------------------------------------------
		#region Attribute(GS): static string RootKey - root for all entries for this app
		// For example, "SOFTWARE\\The Seed Company\\My App"
		static public string RootKey
		{
			get 
			{ 
				return s_sRootKey; 
			}
			set 
			{ 
				s_sRootKey = value; 
			}
		}
		static private string s_sRootKey = "";
		#endregion

		// SetValue (string, int, double, char, bool, DateTime, Guid) ------------------------
		#region Method: void SetValue(sSubKey, sName, sValue) - Workhorse, others all use this one
		static public void SetValue(string sSubKey, string sName, string sValue)
		{
			CheckValidRootKey();
			RegistryKey key = OpenRegistryKey(sSubKey);
			key.SetValue(sName, sValue);
			key.Close();
		}
		#endregion
		#region Methods: void SetValue(sSubKey, sName, _Value) - Extensions
		static public void SetValue(string sSubKey, string sName, int nValue)
		{
			SetValue(sSubKey, sName, nValue.ToString());
		}
		static public void SetValue(string sSubKey, string sName, double dValue)
		{
			SetValue(sSubKey, sName, dValue.ToString());
		}
		static public void SetValue(string sSubKey, string sName, char cValue)
		{
			SetValue(sSubKey, sName, cValue.ToString());
		}
		static public void SetValue(string sSubKey, string sName, bool bValue)
		{
			SetValue(sSubKey, sName, bValue ? "true" : "false");
		}
		static public void SetValue(string sSubKey, string sName, DateTime dt)
		{
			SetValue(sSubKey, sName, dt.ToString());
		}
		static public void SetValue(string sSubKey, string sName, Guid guid)
		{
			SetValue(sSubKey, sName, guid.ToString());
		}
		#endregion
		#region Methods: void SetValue(sName, _Value) - Extensions, stored in Root Key
		static public void SetValue(string sName, string sValue)
		{
			SetValue("", sName, sValue);
		}
		static public void SetValue(string sName, int nValue)
		{
			SetValue("", sName, nValue);
		}
		static public void SetValue(string sName, double dValue)
		{
			SetValue("", sName, dValue);
		}
		static public void SetValue(string sName, char cValue)
		{
			SetValue("", sName, cValue);
		}
		static public void SetValue(string sName, bool bValue)
		{
			SetValue("", sName, bValue);
		}
		static public void SetValue(string sName, DateTime dt)
		{
			SetValue("", sName, dt);
		}
		static public void SetValue(string sName, Guid guid)
		{
			SetValue("", sName, guid);
		}
		#endregion
		#region Methods: void SetValue(_Value) - Extensions, stored in Root Key Default
		static public void SetValue(string sValue)
		{
			SetValue("", "", sValue);
		}
		static public void SetValue(int nValue)
		{
			SetValue("", "", nValue);
		}
		static public void SetValue(double dValue)
		{
			SetValue("", "", dValue);
		}
		static public void SetValue(char cValue)
		{
			SetValue("", "", cValue);
		}
		static public void SetValue(bool bValue)
		{
			SetValue("", "", bValue);
		}
		static public void SetValue(DateTime dt)
		{
			SetValue("", "", dt);
		}
		static public void SetValue(Guid guid)
		{
			SetValue("", "", guid);
		}
		#endregion

		// GetValue (string, int, double, char, bool, DateTime, Guid) ------------------------
		#region Method: string GetValue(sSubKey, sName, sDefaultValue) - Workhorse, others all use this one
		static public string GetValue(string sSubKey, string sName, string sDefaultValue)
		{
			CheckValidRootKey();
			RegistryKey key = OpenRegistryKey(sSubKey);
			string sValue = key.GetValue(sName, sDefaultValue).ToString();
			if (sValue == sDefaultValue)
				SetValue(sSubKey, sName, sValue);
			key.Close();
			return sValue;
		}
		#endregion
		#region Methods: object GetValue(sSubKey, sName, _DefaultValue) - Extensions
		static public int GetValue(string sSubKey, string sName, int nDefaultValue)
		{
			string sValue = GetValue(sSubKey, sName, nDefaultValue.ToString());
			return Convert.ToInt16(sValue);
		}
		static public double GetValue(string sSubKey, string sName, double dDefaultValue)
		{
			string sValue = GetValue(sSubKey, sName, dDefaultValue.ToString());
			return Convert.ToDouble(sValue);
		}
		static public char GetValue(string sSubKey, string sName, char cDefaultValue)
		{
			string sValue = GetValue(sSubKey, sName, cDefaultValue.ToString());
			return Convert.ToChar(sValue);
		}
		static public bool GetValue(string sSubKey, string sName, bool bDefaultValue)
		{
			string sValue = GetValue(sSubKey, sName, bDefaultValue ? "true" : "false");
			return (sValue == "true") ? true : false ;
		}
		static public DateTime GetValue(string sSubKey, string sName, DateTime dtDefaultValue)
		{
			string sValue = GetValue(sSubKey, sName, dtDefaultValue.ToString());
			return DateTime.Parse(sValue);
		}
		static public Guid GetValue(string sSubKey, string sName, Guid guidDefaultValue)
		{
			string sValue = GetValue(sSubKey, sName, guidDefaultValue.ToString());
			return new Guid(sValue);
		}
		#endregion
		#region Methods: object GetValue(sName, _DefaultValue) - Extensions, stored in Root Key
		static public string GetValue(string sName, string sDefaultValue)
		{
			return GetValue("", sName, sDefaultValue);
		}
		static public int GetValue(string sName, int nDefaultValue)
		{
			return GetValue("", sName, nDefaultValue);
		}
		static public double GetValue(string sName, double dDefaultValue)
		{
			return GetValue("", sName, dDefaultValue);
		}
		static public char GetValue(string sName, char cDefaultValue)
		{
			return GetValue("", sName, cDefaultValue);
		}
		static public bool GetValue(string sName, bool bDefaultValue)
		{
			return GetValue("", sName, bDefaultValue);
		}
		static public DateTime GetValue(string sName, DateTime dtDefaultValue)
		{
			return GetValue("", sName, dtDefaultValue);
		}
		static public Guid GetValue(string sName, Guid guidDefaultValue)
		{
			return GetValue("", sName, guidDefaultValue);
		}
		#endregion
		#region Methods: object GetValue(_DefaultValue) - Extensions, stored in Root Key Default
		static public string GetValue(string sDefaultValue)
		{
			return GetValue("", sDefaultValue);
		}
		static public int GetValue(int nDefaultValue)
		{
			return GetValue("", "", nDefaultValue);
		}
		static public double GetValue(double dDefaultValue)
		{
			return GetValue("", "", dDefaultValue);
		}
		static public char GetValue(char cDefaultValue)
		{
			return GetValue("", cDefaultValue);
		}
		static public bool GetValue(bool bDefaultValue)
		{
			return GetValue("", bDefaultValue);
		}
		static public DateTime GetValue(DateTime dtDefaultValue)
		{
			return GetValue("", dtDefaultValue);
		}
		static public Guid GetValue(Guid guidDefaultValue)
		{
			return GetValue("", guidDefaultValue);
		}
		#endregion

		// Get/SetValue for Complext Data Types (point, size. rectangle) ---------------------
		#region void SetValue(sSubKey, point)
		static public void SetValue(string sSubKey, Point pt)
		{
			Debug.Assert(null != sSubKey && "" != sSubKey);
			SetValue(sSubKey, "x", pt.X);
			SetValue(sSubKey, "y", pt.Y);
		}
		#endregion
		#region Point GetValue(sSubKey, pointDefault)
		static public Point GetValue(string sSubKey, Point ptDefault)
		{
			Debug.Assert(null != sSubKey && "" != sSubKey);
			Point pt = new Point();
			pt.X = GetValue(sSubKey, "x", ptDefault.X);
			pt.Y = GetValue(sSubKey, "y", ptDefault.Y);
			return pt;
		}
		#endregion
		#region void SetValue(sSubKey, size)
		static public void SetValue(string sSubKey, Size sz)
		{
			Debug.Assert(null != sSubKey && "" != sSubKey);
			SetValue(sSubKey, "width",  sz.Width);
			SetValue(sSubKey, "height", sz.Height);
		}
		#endregion
		#region Size GetValue(sSubKey, sizeDefault)
		static public Size GetValue(string sSubKey, Size szDefault)
		{
			Debug.Assert(null != sSubKey && "" != sSubKey);
			Size sz = new Size();
			sz.Width  = GetValue(sSubKey, "width",  szDefault.Width);
			sz.Height = GetValue(sSubKey, "height", szDefault.Height);
			return sz;
		}
		#endregion
		#region void SetValue(sSubKey, rect)
		static public void SetValue(string sSubKey, Rectangle rect)
		{
			Debug.Assert(null != sSubKey && "" != sSubKey);
			SetValue(sSubKey, "x",      rect.X);
			SetValue(sSubKey, "y",      rect.Y);
			SetValue(sSubKey, "width",  rect.Width);
			SetValue(sSubKey, "height", rect.Height);
		}
		#endregion
        #region Rectangle GetValue(sSubKey, rectDefault)
        static public Rectangle GetValue(string sSubKey, Rectangle rectDefault)
		{
			Debug.Assert(null != sSubKey && "" != sSubKey);
			Rectangle rect = new Rectangle();
			rect.X      = GetValue(sSubKey, "x",      rectDefault.X);
			rect.Y      = GetValue(sSubKey, "y",      rectDefault.Y);
			rect.Width  = GetValue(sSubKey, "width",  rectDefault.Width);
			rect.Height = GetValue(sSubKey, "height", rectDefault.Height);
			return rect;
		}
		#endregion

		// Misc Operations -------------------------------------------------------------------
		#region Method: RegistryKey OpenRegistryKey(sSubKey)
		static public RegistryKey OpenRegistryKey(string sSubKey)
		{
			CheckValidRootKey();

			string sKey = RootKey + "\\" + sSubKey;

			RegistryKey key = Registry.CurrentUser.OpenSubKey(sKey, true);

            if (null == key)
                key = Registry.CurrentUser.CreateSubKey(sKey);

            if (null == key)
                Console.WriteLine("Key not created for <" + sKey + ">");

			return key;
		}
		#endregion
		#region Method: RegistryKey OpenParentRegistryKey(sChildSubKey)
		static public RegistryKey OpenParentRegistryKey(string sChildSubKey)
		{
			int nParentEnd = sChildSubKey.LastIndexOf("\\");
			string sParentSubKey = nParentEnd > 0 ? sChildSubKey.Substring(0, nParentEnd) : "";
			RegistryKey keyParent = OpenRegistryKey(sParentSubKey);
			return keyParent;
		}
		#endregion
		#region Method: void DeleteSubKey(sSubKey) - deletes this key and its subkeys
		static public void DeleteSubKey(string sSubKey)
			// Delete a key and all of its subkeys
		{
			CheckValidRootKey();

			RegistryKey keyParent = OpenRegistryKey("");
			RegistryKey keyDelete = OpenRegistryKey(sSubKey);
			if (null != keyDelete)
			{
				keyDelete.Close();
				keyParent.DeleteSubKeyTree(sSubKey);
			}
		}
		#endregion

		// Implementation --------------------------------------------------------------------
		#region Constructor
		public JW_Registry()
		{
		}
		#endregion
		#region Method: CheckValidRootKey() - throws if RootKey has not been initialized
		static public void CheckValidRootKey()
			// All JW_Registry public methods should call this to make sure that the
			// programmer has properly initialized the system.
		{
			if ("" == s_sRootKey)
				throw new InvalidRootKeyException();
		}
		#endregion
        #region SAttr{g}: bool HasValidRootKey
        static public bool HasValidRootKey
        {
            get
            {
                if (string.IsNullOrEmpty(s_sRootKey))
                    return false;
                return true;
            }
        }
        #endregion
    }

	// Testing -------------------------------------------------------------------------------
	#region OBSOLETE - NUnit Testing - Need to replace!
	/***
	[TestFixture]
	public class JW_RegistryTest
	{
		// Attributes ------------------------------------------------------------------------
		string m_sSuperKey = "SOFTWARE\\The Seed Company";
		string m_sAppName  = "JW_Registry TestApp";
		string KeyName { get { return m_sSuperKey + "\\" + m_sAppName; } }

		// Scaffolding -----------------------------------------------------------------------
		#region Constructor - required by NUnit
		public JW_RegistryTest()
		{
		}
		#endregion
		#region Method: void Reset() - resets to initial conditions
		private void Reset()
		{
			// Remove any registry keys that a previous test might have created.
			RegistryKey key = Registry.CurrentUser.OpenSubKey(m_sSuperKey, true);
			if (null != key)
			{
				try
				{
					key.DeleteSubKeyTree(m_sAppName);
				}
				catch (ArgumentException) 
					// Don't care is m_sAppName subkey isn't there; it just means our desired
					// end has been met; the subkey isn't there; whether we deleted it or
					// whether it wasn't there to begin with.
				{}
			}
			key.Close();

			// Set the RootKey to "". It will be this way when an app first starts, and the
			// app is expected to set it to something meeaningful.
			JW_Registry.RootKey = "";
		}
		#endregion

		// Tests -----------------------------------------------------------------------------
		#region Test_ExceptionIfEmptyRootKey()
		[Test]
		[ExpectedException(typeof(InvalidRootKeyException))]
		public void Test_ExceptionIfEmptyRootKey()
			// On startup, we expect the RootKey to be empty. An empty RootKey should
			// throw an exception, signalling that the programmer has not done the
			// required initialization.
		{
			Reset();
			JW_Registry.CheckValidRootKey();
			Reset();
		}
		#endregion
		#region Test_DifferentRegistryLocations
		[Test]
		public void Test_DifferentRegistryLocations()
			// These tests are all based on string storage, which is sufficient because all
			// of the other storage types make use of string.
		{
			Reset();
			JW_Registry.RootKey = KeyName;
			string s;

			// Check with regard to the default value for the root key
			JW_Registry.SetValue("Hello World");
			s = JW_Registry.GetValue("", "", "");
			Assert.IsTrue(s == "Hello World");

			// Check for a non-default value for the root key
			JW_Registry.SetValue("1", "Hello Planet");
			s = JW_Registry.GetValue("", "1", "");
			Assert.IsTrue(s == "Hello Planet");

			// Check for a default value in a sub key
			JW_Registry.SetValue("subkey", "", "Hello Earth");
			s = JW_Registry.GetValue("subkey", "", "");
			Assert.IsTrue(s == "Hello Earth");

			// Check for a non-default value in a sub key
			JW_Registry.SetValue("subkey", "1", "Hello Everyone");
			s = JW_Registry.GetValue("subkey", "1", "");
			Assert.IsTrue(s == "Hello Everyone");
			Reset();
		}
		#endregion
		#region Test_SimpleDataTypes
		[Test]
		public void Test_SimpleDataTypes()
		{
			// Setup
			Reset();
			JW_Registry.RootKey = KeyName;

			// String
			string sTest = "Hi, folks!";
			JW_Registry.SetValue(sTest);
			Assert.IsTrue(JW_Registry.GetValue("Not This") == sTest);

			// Int
			int nTest = -3415;
			JW_Registry.SetValue(nTest);
			Assert.IsTrue(JW_Registry.GetValue(45) == nTest);

			// Double
			double dTest = -526.23356612345;
			JW_Registry.SetValue(dTest);
			Assert.IsTrue(JW_Registry.GetValue(45.3) == dTest);

			// Char
			char cTest = 'j';
			JW_Registry.SetValue(cTest);
			Assert.IsTrue(JW_Registry.GetValue('w') == cTest);

			// Bool
			bool bTest = true;
			JW_Registry.SetValue(bTest);
			Assert.IsTrue(JW_Registry.GetValue(false) == bTest);

			// DateTime
			DateTime dtNow = DateTime.Now;
			dtNow = dtNow.AddTicks(- dtNow.Ticks); // zero out the ticks
			JW_Registry.SetValue(dtNow);
			Assert.IsTrue(JW_Registry.GetValue(DateTime.UtcNow) == dtNow);

			// Guid
			Guid guidTest = Guid.NewGuid();
			JW_Registry.SetValue(guidTest);
			Assert.IsTrue(JW_Registry.GetValue(Guid.NewGuid()) == guidTest);
			Reset();
		}
		#endregion
		#region Test_ComplextDataTypes()
		[Test]
		public void Test_ComplextDataTypes()
		{
			// Setup
			Reset();
			JW_Registry.RootKey = KeyName;

			// Point
			Point pt = new Point(345, 678);
			JW_Registry.SetValue("point", pt);
			Assert.IsTrue(JW_Registry.GetValue("point", new Point(-2,5)) == pt);

			// Size
			Size sz = new Size(12, 45);
			JW_Registry.SetValue("size", sz);
			Assert.IsTrue(JW_Registry.GetValue("size", new Size(2,5)) == sz);

			// Rectangle
			Rectangle rect = new Rectangle(12, 45, 445, 6334);
			JW_Registry.SetValue("rect", rect);
			Assert.IsTrue(JW_Registry.GetValue("rect", new Rectangle(8,9,18,19)) == rect);
			Reset();
		}
		#endregion
		#region Test_ParentKey()
		[Test]
		public void Test_ParentKey()
		{
			// Setup
			Reset();
			JW_Registry.RootKey = KeyName;

			// Create a parent and a child key
			JW_Registry.SetValue("parent", "", "parent");
			JW_Registry.SetValue("parent\\child", "", "child");

			// Open the parent directly
			RegistryKey keyParent = JW_Registry.OpenRegistryKey("parent");

			// Open the parent indirectly: are they they same?
			RegistryKey keyParentIndirect = JW_Registry.OpenParentRegistryKey("parent\\child");
			Assert.IsTrue(keyParent.Name == keyParentIndirect.Name);
			Reset();
		}
		#endregion
		#region Test_DeleteKey()
		[Test]
		public void Test_DeleteKey()
		{
			// Setup
			Reset();
			JW_Registry.RootKey = KeyName;

			// Create a parent key and a subkey
			JW_Registry.SetValue("Parent\\Child", "", "just created");
			RegistryKey keyParent = JW_Registry.OpenRegistryKey("Parent");
			int nSubkeys = keyParent.SubKeyCount;
			Assert.IsTrue(1 == keyParent.SubKeyCount);
			JW_Registry.DeleteSubKey("Parent\\Child");
			Assert.IsTrue(0 == keyParent.SubKeyCount);
			Reset();
		}
		#endregion
	}
	***/
	#endregion
}
