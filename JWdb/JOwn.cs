/**********************************************************************************************
 * App:     Josiah
 * File:    JOwn.cs
 * Author:  John Wimbish
 * Created: 27 Oct 2003
 * Purpose: Implements JOwn, an atomic owning attribute.
 * Legal:   Copyright (c) 2005-07, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
#region Header: Using, etc.
using System;
using System.Collections;
using System.Diagnostics;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using JWTools;
#endregion

#region Documentation: How to use.
/* When used in another class, the syntax is:
 *    JOwn m_Title;
 * 
 * In that class, the constructor should create the new JOwnSeq:
 *    m_Title = new JOwn("Title", this, typeof(JParagraph));
 * 
 * The class that will be owned, e.g., JParagraph, must implement...
 *   + a "Read" constructor.
 */
#endregion
#region Documentation: Features
/* Implements the Cellar concept of an atomic owning attribute.
 * 
 * - The type of object permitted in the sequence is enforced. This is done on all methods
 *   that set an object in the attribute, under the logic that if we prevent invalid objects,
 *   then we don't have to worry about them subsequently.
 * 
 * - Ownership
 *   + When an obj is added to the JOwn, its Owner attribute is set to the attribute's owner
 *   + When an obj is removed, its Owner is set to null.
 *   + The obj cannot be owned by the JOwn and by anything else.
 * 
 * - I/O to XML file
 *   + Write method writes the attribute (and its object) out to the stream
 *   + Read method populates the attribute from the stream, creating an object of
 *       the sequence's signature.
 * 
 */
#endregion
#region Documentation: Stories not yet implemented
/*
 * - Merging as options:
 *   + Replace the existing object
 *   + Keep the existing object
 *   + Ask what to do
 *   (Note: A merge happens when data has already been read in the first time. So we 
 *   can assume the data is already there, and that the mergeOption is set as part 
 *   of the conceptual model setup, rather than being read in with the data.)
 */
#endregion

namespace JWdb
{
	public class JOwn : JAttr
	{
		// Attributes ------------------------------------------------------------------------

		// Private attributes ----------------------------------------------------------------
		#region Private Attribute: JObject m_object - stores the object
		protected JObject m_object = null;
		#endregion
		static private string m_sTag = "own";    // xml tag for I/O

		// Scaffolding -----------------------------------------------------------------------
		#region Constructor(sName, objOwner, signature) - sets up the attribute
		public JOwn(string sName, JObject objOwner, Type signature)
			: base(sName, objOwner, signature)
		{
		}
		#endregion

		// Getting / Setting -----------------------------------------------------------------
		#region Attr{g/s}: JObject Value - main method for getting / setting the attr's value
		public JObject Value
		{
			get
			{
				return m_object;
			}
			set
			{
				if (null == value)
				{
					Clear();
				}
				else
				{
					// Integrity check
					CheckCorrectSignature(value);

					// Remove ownership from the object we're about to remove
					if (null != m_object)
						m_object.Owner = null;

					// Now insert the target object and give it an owner
					m_object = value;
					m_object.Owner = Owner;

					// Will need to be saved
					DeclareDirty();
				}
			}
		}
		#endregion
		#region Method: void Clear() - sets the value to null
		public void Clear()
		{
			if (null != m_object)
			{
				m_object.Clear();
				m_object.Owner = null;
				m_object = null;
				DeclareDirty();
			}
		}
		#endregion

		// I/O -------------------------------------------------------------------------------
		#region Attribute: OpeningXmlTagLine - e.g., "<own Name="Section">
		public override string OpeningXmlTagLine
		{
			get { return "<" + m_sTag + " Name=\"" + Name + "\">"; }
		}
		#endregion
		#region Attribute: ClosingXmlTagLine - e.g., "</own>
		private string ClosingXmlTagLine
		{
			get { return "</" + m_sTag + ">"; }
		}
		#endregion
		#region Method: Write(TextWriter) - writes the owning attr and its object to xml file
		public override void Write(TextWriter tw, int nIndent)
		{
			// No reason to write the attr unless it has a value
			if (null != m_object)
			{
				tw.WriteLine(IndentPadding(nIndent) + OpeningXmlTagLine);
				m_object.Write(tw, nIndent + 1);
				tw.WriteLine(IndentPadding(nIndent) + ClosingXmlTagLine);
			}
		}
		#endregion
		#region Method: Read(string sFirstLine, TextReader tr) - reads from xml file
		public override void Read(string sFirstLine, TextReader tr)
		{
			// If the contents of the first line are not this owning sequence, then return.
			if (sFirstLine != OpeningXmlTagLine)
				return;

			// Read lines from the stream until the Closing Tag is found
			string sLine;
			while ( (sLine = tr.ReadLine()) != null)
			{
				// We're done when we see the end of the sequence
				sLine = sLine.Trim();
				if (sLine == ClosingXmlTagLine)
					break;

				// Otherwise, create a new object of the signature type and read its data
				JObject obj = InvokeConstructor();
				obj.Read(sLine, tr);
				Debug.Assert(obj != null);
				Value = obj;
			}
		}
		#endregion
	}

}
