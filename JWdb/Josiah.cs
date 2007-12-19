/**********************************************************************************************
 * App:     Josiah
 * File:    Josiah.cs
 * Author:  John Wimbish
 * Created: 27 Oct 2003
 * Purpose: Server for language project data - main file.
 * Legal:   Copyright (c) 2005-07, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
#region Header: Using, etc.
using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using JWTools;
#endregion
#region Documentation: Stories not yet implemented
/* We currently disable checking for duplicates when reading in an owning sequence. We probably
 * should check to see if the file has been modified since we wrote it, and if so, turn the
 * checking back on, in case the user has made a change that would affect integrity.
 */
#endregion

namespace JWdb  
{
	// Exceptions ----------------------------------------------------------------------------
	#region class eJosiahException - generic Josiah exception (root for all others)
	public class eJosiahException : ApplicationException
	{
		static private string m_sError = "Josiah"; 
		public eJosiahException()
			: base(m_sError)
		{
			if (!Test.IsTesting)
				Debug.Assert(false);
		}
		public eJosiahException(string message)
			: base(m_sError + " - " + message)
		{
			if (!Test.IsTesting)
				Debug.Assert(false);
		}
		public eJosiahException(string message, Exception inner)
			: base(m_sError + " - " + message, inner)
		{
			if (!Test.IsTesting)
				Debug.Assert(false);
		}
	}
	#endregion

	public class JUtil
	{
		#region Method: static void TextWriter GetTextWriter(string sPath)
		public static TextWriter GetTextWriter(string sPath)
		{
			StreamWriter w = new StreamWriter(sPath, false);
			TextWriter tw = TextWriter.Synchronized(w);
			return tw;
		}
		#endregion
		#region Method: static void TextReader GetTextReader(string sPath)
		public static TextReader GetTextReader(string sPath)
		{
			StreamReader r = new StreamReader(sPath);
			TextReader tr = TextReader.Synchronized(r);
			return tr;
		}
		#endregion
	}


}
