/**********************************************************************************************
 * App:     Josiah
 * File:    JRef.cs
 * Author:  John Wimbish
 * Created: 15 Mar 2004
 * Purpose: Implements JRef, an atomic reference attribute.
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

#region Documentation
/*
 * 
 * - I/O - we store a path, which looks like:
 *           3-Entries-23-Senses-4
 * 
 *         Which means:
 *           1. Go up three levels in the ownership hierarchy
 *           2. Find the 23rd object in the Entries attribute
 *           3. From that object, find the 4th object in the Senses attribute.
 * 
 *         Storing a path means that it is not necessary to store unique ID's on objects,
 *         keep track of incrementing a NextAvailableId counter, etc. In my experience, 
 *         references are relatively rare, and I'd rather pay the price of expanding the
 *         xml file as opposed to keeping an integer (or long) value on each and every
 *         object.
 */
#endregion

namespace JWdb
{
	#region Class JRefHolder - temporarily holds the xml imput file path during a Read operation
	public class JRefHolder : JObject
	{
		// ZAttrs ----------------------------------------------------------------------------
		#region BAttr{g/s}: string RefPath - the path to the referenced object
		public string RefPath
		{
			get
			{
				return m_sRefPath;
			}
			set
			{
				m_sRefPath = value;
			}
		}
		private string m_sRefPath;
		#endregion
		#region Method: void DeclareAttrs()
		protected override void DeclareAttrs()
		{
			base.DeclareAttrs();
			DefineAttr("RefPath", ref m_sRefPath);
		}
		#endregion

		// Scaffolding -----------------------------------------------------------------------
		#region Constructor(string sRefPath)
		public JRefHolder(string sRefPath) 
			: base()
		{
			RefPath = sRefPath;
		}
		#endregion
	}
	#endregion

	public class JRef : JAttr
	{
		// Private attributes ----------------------------------------------------------------
		#region Private Attribute: JObject m_referencedObject - stores the object referenced
		protected JObject m_referencedObject = null;
		#endregion
		static private string m_sTag = "ref";    // xml tag for I/O

		// Scaffolding -----------------------------------------------------------------------
		#region Constructor(sName, objOwner, signature) - sets up the attribute
		public JRef(string sName, JObject objOwner, Type signature)
			: base(sName, objOwner, signature)
		{
		}
		#endregion

		// Getting / Setting -----------------------------------------------------------------
		#region Attr{g/s} JObject Value - main method for getting / setting the attr's value
		public JObject Value
		{
			get
			{
				return m_referencedObject;
			}
			set
			{
				if (null == value)
					Clear();
				else
				{
					// Integrity check
					CheckCorrectSignature(value);

					// Now insert the target object and give it an owner
					m_referencedObject = value;

					DeclareDirty();
				}
			}
		}
		#endregion
		#region Method: void Clear() - removes the object
		public void Clear()
		{
			m_referencedObject = null;
			DeclareDirty();
		}
		#endregion

		// I/O -------------------------------------------------------------------------------
		#region Attribute: OpeningXmlTagLine - e.g., "<ref Name="CurrentWritingSystem">
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
		#region Method: Write(TextWriter) - writes the reference attr
		public override void Write(TextWriter tw, int nIndent)
		{
			// If not pointing to anything, then there is nothing to write
			if (null == Value)
				return;

			string s = PathToReferencedObject;
			tw.WriteLine(IndentPadding(nIndent) + OpeningXmlTagLine + 
				PathToReferencedObject + ClosingXmlTagLine);
		}
		#endregion
		#region Method: Read(string sFirstLine, TextReader tr) - reads from xml file
		public override void Read(string s, TextReader tr)
			// We create a temporary object, a JRefHolder, and place it in the ReferencedObject
			// attribute here. Then at the end of the read (from the topmost object), we
			// call ResolveReference recursively, which goes through and converts these
			// paths into real references. It is necessary to wait to do this, because
			// we must be sure that all objects have been read in, before attempting to
			// reference to them.
		{
			// Extract the path
			string sRefPath = s.Substring( s.IndexOf('>') + 1 );
			sRefPath = sRefPath.Substring(0, sRefPath.IndexOf('<'));

			// Store it for now, we'll resolve it later (after all objects have been read in).
			m_referencedObject = new JRefHolder(sRefPath);
		}
		#endregion
		#region Method: void ResolveReference()
		public void ResolveReference()
		{
			// The attribute does not have a value set
			if (null == m_referencedObject)
				return;

			// Convert the place holder into a real reference
			if (m_referencedObject.GetType() == typeof(JRefHolder))
			{
				// Extract our RefPath, and remove the JRefHolder object
				string sRefPath = ((JRefHolder)m_referencedObject).RefPath;
				m_referencedObject = null;

				// Extract from the RefPath how high up the ownership hiererchy 
				string sUp = sRefPath.Substring(0, sRefPath.IndexOf('-') );
				int cUp = Convert.ToInt16(sUp);
				string sDown = sRefPath.Substring( sRefPath.IndexOf('-') + 1);

				// Move up the hierarchy as directed
				JObject obj = this.Owner;
				while (cUp > 0)
				{
					obj = obj.Owner;
					--cUp;
				}

				// Find the target object
				JObject objTarget = obj.GetObjectFromPath(sDown);
				Value = objTarget;
			}
		}
		#endregion
		#region Attr{g}: string PathToReferencedObject - returns human-readable path for xml file
		public string PathToReferencedObject
			// Returns a path, e,g, "3-Entries-432-Senses-4-PartOfSpeech"
		{
			get
			{
				// Get a list of the ownership hierarchy for both objects
				ArrayList rgSourceHierarchy = (null != Owner) ? Owner.AllOwners : new ArrayList();
				ArrayList rgTargetHierarchy = Value.AllOwners;

				// Determine how far up the hierarchy we must go in order to get a common owner.
				int nDistanceUp = rgSourceHierarchy.Count;

				// We need one common owner (hence i==1 to start the loop); but if we have more
				// than one in common, then we can skip the upper ones.
				for(int i = 1; i < Math.Min(rgSourceHierarchy.Count,rgTargetHierarchy.Count); i++)
				{
					if ( rgSourceHierarchy[i] != rgTargetHierarchy[i])
						break;
					--nDistanceUp;
				}
				string sPath = nDistanceUp.ToString();

				// Point to the nearies object that the two have in common
				JObject objTop = Owner;
				for(int i=0; i<nDistanceUp; i++)
				{
					objTop = objTop.Owner;
				}

				// Build path to ref'd obj recursively from bottom
				sPath += Value.GetPathFromOwningObject( objTop );
				return sPath;
			}
		}
		#endregion
	}
}
