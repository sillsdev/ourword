/**********************************************************************************************
 * Project: Our Word!
 * File:    Test.cs
 * Author:  John Wimbish
 * Created: 2 Dec 2003
 * Purpose: Testing for the app.
 * Legal:   Copyright (c) 2005-09, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
#region Using
using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using System.Text;
using JWTools;
using OurWordData;
using OurWordData.DataModel;
using OurWord.Dialogs;
using OurWord.Layouts;
#endregion

// TO ENABLE THE MENU COMMAND,
//    In the registry, under the "Our Word!" key, 
//        add the Debug key and set its value to "true".

namespace OurWord
{
	#region Class: Health
	public class Health
	{
		const string c_sClassName = "Health";
		const string c_sStopName  = "System.Windows";

        // Message Stack ---------------------------------------------------------------------
        #region EMBEDDED CLASS: Message
        class Message
		{
			// Attrs -------------------------------------------------------------------------
			bool       m_bIsParam = false;
			string     m_sMessage;
			MethodBase m_MethodInfo;
			#region Attr{g}: int Depth
			public int Depth
			{
				get
				{
					return m_nDepth;
				}
			}
			int m_nDepth;
			#endregion

			// Construction ------------------------------------------------------------------
			#region Constructor(sMessage)
			public Message(string sMessage)
			{
				m_bIsParam   = false;
				m_sMessage   = sMessage;
				m_MethodInfo = null;

				StackTrace st = new StackTrace(1, true);
				m_nDepth = st.FrameCount;

				Initialize(st);
			}
			#endregion
			#region Constructor(sMessage, bIsParam)
			public Message(string sMessage, bool bIsParam)
			{
				m_bIsParam   = bIsParam;
				m_sMessage   = sMessage;
				m_MethodInfo = null;

				StackTrace st = new StackTrace(1, true);
				m_nDepth = st.FrameCount;

				Initialize(st);
			}
			#endregion
			#region Method: void Initialize()
			private void Initialize(StackTrace st)
			{

				int i = 0;
				for(; i < st.FrameCount; i++)
				{
					StackFrame sf = st.GetFrame(i);
					MethodBase mb = sf.GetMethod();
					Type type = mb.ReflectedType;
					if (c_sClassName != type.Name)
					{
						m_MethodInfo = mb;
						break;
					}
				}
			}
			#endregion

			#region Method: void Write(TextWriter)
			public void Write(TextWriter w)
			{
				if (m_bIsParam)
				{
					string s = new string(' ', 20);
					s += m_MethodInfo.Name + ": " + m_sMessage;
					w.WriteLine(s);
					return;
				}

				w.WriteLine(m_MethodInfo.Name + ": " + m_sMessage);
			}
			#endregion
        }
        #endregion
        #region Attr{g}: ArrayList MessageStack
        static ArrayList MessageStack
		{
			get
			{
				if (null == s_MessageStack)
					s_MessageStack = new ArrayList();
				return s_MessageStack;
			}
		}
		static ArrayList s_MessageStack = null;
		#endregion
        #region SMethod: void Push(string s, bool bIsParam)
        public static void Push(string s, bool bIsParam)
		{
			// Create the new message
			Message msg = new Message(s);

			// Clean out anything which is lower down in the call stack
			for(int i=0; i<MessageStack.Count; )
			{
				Message m = MessageStack[i] as Message;
				if ( m.Depth > msg.Depth )
					MessageStack.Remove(m);
				else
					i++;
			}

			// Add the new message
			MessageStack.Add( new Message(s, bIsParam) );
        }
        #endregion
        #region SMethod: void Push(string sMessage)
        public static void Push(string s)
		{
			Push(s, false);
		}
		#endregion

        #region SMethod: void Param(string sName, int n)
        public static void Param(string sName, int n)
		{
			Push("int " + sName + " : " + n.ToString(), true);
        }
        #endregion
        #region SMethod: void Param(string sName, bool b)
        public static void Param(string sName, bool b)
		{
			Push("bool " + sName + " : " + (b ? "true" : "false"), true);
        }
        #endregion

        // Log File --------------------------------------------------------------------------
		static TextWriter w = null;
		#region Method: void OpenLogFile()
		static void OpenLogFile()
		{
			string sFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
			string sPathName = sFolder + Path.DirectorySeparatorChar + "OurWordErrorLog.txt";
			StreamWriter sw = new StreamWriter(sPathName, false, Encoding.UTF8);
			w = TextWriter.Synchronized(sw);
		}
		#endregion
		#region Method: CloseLogFile()
		static void CloseLogFile()
		{
			if (null != w)
			{
				w.Flush();
				w.Close();
				w = null;
			}
		}
		#endregion
		#region Method: void Write(int nIndent, string s)
		static void Write(int nIndent, string s)
		{
			string sIndent = "";
			while(nIndent > 0)
			{
				sIndent += ' ';
				nIndent--;
			}
			w.WriteLine(sIndent + s);
		}
		#endregion
		#region Method: void WriteBlankLine()
		static void WriteBlankLine()
		{
			w.WriteLine("");
		}
		#endregion

		// Trace Information -----------------------------------------------------------------
		#region Method: string GetVersion()
		static string GetVersion()
		{
			return "VERSION = " + G.Version;
		}
		#endregion
		#region Method: string GetDate()
		static string GetDate()
		{
			return "Log Date = " + DateTime.Today.ToString("dd-MMM-yyyy");
		}
		#endregion
		#region Method: string GetMethodParams( MethodBase mb )
		static string GetMethodParams( MethodBase mb )
		{
			string sOut = "(";

			ParameterInfo[] pars = mb.GetParameters();
			for(int k=0; k<pars.Length; k++)
			{
				ParameterInfo p = pars[k];
				sOut += p.ParameterType.Name;

				if (k < pars.Length - 1)
					sOut += ", ";
			}
			sOut += ")";

			return sOut;
		}
		#endregion
		#region Method: string AppendPadding(string s, int nPaddingAmount)
		static string AppendPadding(string s, int nPaddingAmount)
		{
			while (s.Length < nPaddingAmount)
				s += ' ';
			return s;
		}
		#endregion
		#region Method: string GetFileLine(StackFrame sf)
		static string GetFileLine(StackFrame sf)
		{
			string sOut = "";
			string sFileName = sf.GetFileName();
			if (null != sFileName && 0 != sf.GetFileLineNumber())
			{
				sOut += Path.GetFileName(sFileName);
				sOut += ( "(" + sf.GetFileLineNumber() + ")" );
			}
			return sOut;
		}
		#endregion
		#region Method: static void Log()
		static void Log()
		{
			int nIndent = 0;

			// Open the log file
			OpenLogFile();

			// Version, Date
			Write(0, GetVersion());
			Write(5, GetDate());
			WriteBlankLine();

			// Stack
			StackTrace st = new StackTrace(0, true);
			for(int i = 0; i < st.FrameCount; i++)
			{
				StackFrame sf = st.GetFrame(i);
				MethodBase mb = sf.GetMethod();
				Type type = mb.ReflectedType;

				// Skip the trace information while we're in Health's methods
				if (c_sClassName == type.Name)
					continue;

				// We've gone high enough once we get into System.Windows
				string sNameSpace = type.Namespace;
				if (sNameSpace.Length > c_sStopName.Length && 
					sNameSpace.Substring(0, c_sStopName.Length) == c_sStopName)
				{
					break;
				}

				// Class Name
				string sOut = type.Name + ".";
				sOut = AppendPadding(sOut, 20);

				// Method Name
				sOut += (mb.Name + " ");

				// Parameters
				sOut += GetMethodParams(mb);
				sOut = AppendPadding(sOut, 70);

				// File Name & Line Number
				sOut += GetFileLine(sf);

				// Write it out
				Write(nIndent++, sOut);
			}
			WriteBlankLine();

			// Messages
			foreach(Message m in MessageStack)
				m.Write(w);

			CloseLogFile();
		}
		#endregion

		// Assertions ------------------------------------------------------------------------
		#region Method: void OK(bool bTest)
		static public void OK(bool bTest)
		{
			if (!bTest)
				Log();
		}
		#endregion
	}
	#endregion

	public class TestHarness
	{
		#region Constructor()
		public TestHarness()
		{
		}
		#endregion

		#region Method: void Run()
		public void Run()
		{
//			(new Test_FileMenuIO()).Run();
			(new Test_DBookProperties()).Run();

			Test.WriteHeader("COMPLETED SUCCESSFULLY");
		}
		#endregion
	}
}
