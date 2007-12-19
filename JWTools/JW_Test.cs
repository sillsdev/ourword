/**********************************************************************************************
 * Dll:      JWTools
 * File:     JW_FTest.cs
 * Author:   John Wimbish
 * Created:  15 Feb 2004
 * Purpose:  Automated Testing
 * Legal:    Copyright (c) 2005-08, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
#region Using
using System;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using System.Text;
using System.Threading;
using System.IO;
#endregion

namespace JWTools
{
	public class Test
	{
		static public bool IsTesting = false;

		string m_sName;
		ArrayList m_rgTests;
		ArrayList m_rgNames;

		public delegate void IndividualTest();

		// Pathnames for temporary files as needed -------------------------------------------
		#region Method: string TempFolder - creates it if it does not exist
		static protected string TempFolder
		{
			get
			{
				string sFolder = Path.GetDirectoryName(Application.ExecutablePath)
					+ "\\Testing";
				if ( !Directory.Exists(sFolder) )
					Directory.CreateDirectory(sFolder);
				return sFolder;
			}
		}
		#endregion
		#region Attribute: PathNameA - temporary (c:\Program Files\NUnit\bin\TestNameA.x)
		protected string PathNameA
		{
			get 
			{
				return TempFolder + "\\" + m_sName + "A.x";
			}
		}
		#endregion
		#region Attribute: PathNameB - temporary (c:\Program Files\NUnit\bin\TestNameB.x)
		protected string PathNameB
		{
			get 
			{
				return TempFolder + "\\" + m_sName + "B.x";
			}
		}
		#endregion
		#region Attribute: PathNameC - temporary (c:\Program Files\NUnit\bin\TestNameC.x)
		protected string PathNameC
		{
			get 
			{
				return TempFolder + "\\" + m_sName + "C.x";
			}
		}
		#endregion
		#region Method: static string GetPathName(string sBaseName)
		static public string GetPathName(string sBaseName)
		{
			return TempFolder + "\\" + sBaseName + ".x";
		}
		#endregion


		#region Constructor(sTestSectionName)
		public Test(string sName)
		{
			m_sName = sName;
			m_rgTests = new ArrayList();
			m_rgNames = new ArrayList();
		}
		#endregion
		#region Method: void AddTest(IndividualTest, sTestName) - Add tests in the sub's contructor
		public void AddTest(IndividualTest test, string sTestName)
		{
			m_rgTests.Add(test);
			m_rgNames.Add(sTestName);
		}
		#endregion

		#region Method: void _WriteTestName(string sName)
		private void _WriteTestName(string sName)
		{
			string sTestName = ">>> " + sName + " ";
			int k = 60 - sTestName.Length;
			while ( (k--) > 0)
				sTestName += ">";
			Console.WriteLine(sTestName);
		}
		#endregion
		#region Method: void WriteHeader(string sName)
		public static void WriteHeader(string sName)
		{
			Console.WriteLine("");
			int k = 45 - sName.Length;
			Console.WriteLine("############################################################");
			string sHeader =  "# TESTING: \"" + sName + "\" ";
			while ( (k--) > 0)
				sHeader += " ";
			sHeader += '#';
			Console.WriteLine(sHeader);
			Console.WriteLine("############################################################");
		}
		#endregion

		#region Method: void Run() - TestHarness should call this to run all the tests
		public void Run()
		{
			IsTesting = true;

			// Do a pretty header
			WriteHeader(m_sName);

			// Run through each of the tests
			for(int i=0; i<m_rgTests.Count; i++)
			{
				_WriteTestName( m_rgNames[i] as string );
				EnableTracing = false;

				Setup();

				IndividualTest t = m_rgTests[i] as IndividualTest;
				t();

				TearDown();
			}

			IsTesting = false;
		}
		#endregion
		#region Method: override void Setup()
		public virtual void Setup()
		{
		}
		#endregion
		#region Method: override void TearDown()
		public virtual void TearDown()
		{
		}
		#endregion

		// Tracing ---------------------------------------------------------------------------
		char m_chStage = 'a';
		#region Attr{g}: bool EnableTracing
		public bool EnableTracing
		{
			get
			{
				return m_bEnableTracing;
			}
			set
			{
				if (value == true)
					m_chStage = 'a';
				m_bEnableTracing = value;
			}
		}
		private bool m_bEnableTracing = false;
		#endregion
		#region Method: void WriteLine(string s)
		public void WriteLine(string s)
		{
			if (EnableTracing)
			{
				Console.WriteLine("       " + s);
			}
		}
		#endregion
		#region Method: void Trace(string s)
		public void Trace(string s)
		{
			if (EnableTracing)
			{
				WriteLine(m_chStage + " - " + s);
				m_chStage++;
			}
		}
		#endregion

		// Assertions ------------------------------------------------------------------------
		#region Method: void IsTrue(bool b)
		public void IsTrue(bool b)
		{
			if (!b)
			{
				WriteLine("FAILURE: IsTrue");
				Debug.Assert(false);
			}
		}
		#endregion
		#region Method: void IsFalse(bool b)
		public void IsFalse(bool b)
		{
			if (b)
			{
				WriteLine("FAILURE: IsFalse");
				Debug.Assert(false);
			}
		}
		#endregion
		#region Method: void IsNotNull(object obj)
		public void IsNotNull(object obj)
		{
			if (null == obj)
			{
				WriteLine("FAILURE: NULL Object");
				Debug.Assert(false);
			}
		}
		#endregion
		#region Method: void IsNull(object obj)
		public void IsNull(object obj)
		{
			if (null != obj)
			{
				WriteLine("FAILURE: NON-NULL Object");
				Debug.Assert(false);
			}
		}
		#endregion

		#region Method: void AreSame(string sExpected, string sActual)
		public void AreSame(string sExpected, string sActual)
		{
			if (sExpected != sActual)
			{
				WriteLine("FAILURE:");
				WriteLine("  Expected: \"" + sExpected + "\"");
				WriteLine("  Actual:   \"" + sActual + "\"");
				Debug.Assert(false);
			}
		}
		#endregion
		#region Method: void AreSame(int nExpected, int nActual)
		public void AreSame(int nExpected, int nActual)
		{
			if (nExpected != nActual)
			{
				WriteLine("FAILURE:");
				WriteLine("  Expected: \"" + nExpected.ToString() + "\"");
				WriteLine("  Actual:   \"" + nActual.ToString() + "\"");
				Debug.Assert(false);
			}
		}
		#endregion
		#region Method: void AreSame(float fExpected, float fActual)
		public void AreSame(float fExpected, float fActual)
		{
			if (fExpected != fActual)
			{
				WriteLine("FAILURE:");
				WriteLine("  Expected: \"" + fExpected.ToString() + "\"");
				WriteLine("  Actual:   \"" + fActual.ToString() + "\"");
				Debug.Assert(false);
			}
		}
		#endregion
		#region Method: void AreSame(char chExpected, char chActual)
		public void AreSame(char chExpected, char chActual)
		{
			if (chExpected != chActual)
			{
				WriteLine("FAILURE:");
				WriteLine("  Expected: '" + chExpected.ToString() + "'");
				WriteLine("  Actual:   '" + chActual.ToString() + "'");
				Debug.Assert(false);
			}
		}
		#endregion
		#region Method: void AreSame(bool bExpected, bool bActual)
		public void AreSame(bool bExpected, bool bActual)
		{
			if (bExpected != bActual)
			{
				WriteLine("FAILURE:");
				WriteLine("  Expected: '" + bExpected.ToString() + "'");
				WriteLine("  Actual:   '" + bActual.ToString() + "'");
				Debug.Assert(false);
			}
		}
		#endregion
		#region Method: void AreSame(PointF ptExpected, PointF ptActual)
		public void AreSame(PointF ptExpected, PointF ptActual)
		{
			AreSame(ptExpected.X, ptActual.X);
			AreSame(ptExpected.Y, ptActual.Y);
		}
		#endregion
	}
}
