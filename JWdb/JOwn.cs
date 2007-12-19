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

	#region TEST
	public class Test_JOwn : Test
	{
		#region Constructor()
		public Test_JOwn()
			: base("JOwn")
		{
			AddTest( new IndividualTest( Ownership ),           "Ownership" );
			AddTest( new IndividualTest( OwnershipUniqueness ), "OwnershipUniqueness" );
			AddTest( new IndividualTest( OwnerStoresSelf ),     "OwnerStoresSelf" );
			AddTest( new IndividualTest( SignatureControl ),    "SignatureControl" );
			AddTest( new IndividualTest( ReadWrite ),           "ReadWrite" );
		}
		#endregion

		// Ownership -------------------------------------------------------------------------
		#region Ownership
		public void Ownership()
		{
			// Create a couple of objects. They should have no owner
			TObjB obj1 = new TObjB("b1");
			IsTrue(obj1.Owner == null);
			TObjB obj2 = new TObjB("b2");
			IsTrue(obj2.Owner == null);

			// Set the first obj into the attr, and see if it has an owner
			TObjA objOwner = new TObjA("a");
			objOwner.m_own1.Value = obj1;
			IsTrue(obj1.Owner == objOwner);

			// Put the other obj into the attr, and see that the owner is the second obj.
			objOwner.m_own1.Value = obj2;
			IsTrue(obj2.Owner == objOwner);
			IsTrue(obj1.Owner == null);		
		}
		#endregion
		#region OwnershipUniqueness
		public void OwnershipUniqueness()
		{
			//EnableTracing = true;
			bool bCaught = false;
			try
			{
				// Create an owning object
				TObjA objOwner = new TObjA("a");

				// Place an object into the first attribute
				TObjB objB = new TObjB("b1");
				objOwner.m_own1.Value = objB;

				// Now attempt to place it into the second attr. Should fail because
				// object is already owned.
				objOwner.m_own2.Value = objB;
			}
			catch (eAlreadyOwned)
			{
				Trace("Exception Caught");
				bCaught = true;
			}
			IsTrue(bCaught);
		}
		#endregion
		#region OwnerStoresSelf
		public void OwnerStoresSelf()
		{
			TObjA objOwner = new TObjA("a");
			IsTrue( objOwner._test_ContainsAttribute(objOwner.m_own1) );
		}
		#endregion

		// Correct signature of owned object -------------------------------------------------
		#region SignatureControl
		public void SignatureControl()
			// We declare a JOwn with a signature of one type, and then attempt to
			// add an object to it of a different type. We expect an exception. If we check
			// anytime an object is being added, then we prevent the sequence from ever
			// having bad objects.
		{
			bool bCaught = false;
			try
			{
				// Create an owning object
				TObjA objOwner = new TObjA("a");

				// Attempt to add the wrong kind of object
				objOwner.m_own1.Value = new TObjD("d");
			}
			catch (eBadSignature)
			{
				bCaught = true;
			}
			IsTrue(bCaught);
		}
		#endregion

		// I/O -------------------------------------------------------------------------------
		#region Attr: ReadWritePathName - pathname for test file
		private string ReadWritePathName
		{
			get 
			{
				string sPath = Application.ExecutablePath;
				sPath = sPath.Substring(0, sPath.LastIndexOf("\\") ) + "\\Test.x";
				return sPath;
			}
		}
		#endregion
		#region ReadWrite
		public void ReadWrite()
		{
			// Create owning objects
			TObjA objOwner1 = new TObjA("a1");
			TObjA objOwner2 = new TObjA("a2");

			// Set up an owning attr and write it out
			objOwner1.m_own1.Value = new TObjB("VerseNo");
			TextWriter tw = JUtil.GetTextWriter(ReadWritePathName);
			objOwner1.m_own1.Write(tw, 0);
			tw.Close();

			// Read it into anouther owning attr
			TextReader tr = JUtil.GetTextReader(ReadWritePathName);
			string sLine;
			while ( (sLine = tr.ReadLine()) != null)
			{
				objOwner2.m_own1.Read(sLine, tr);
			}
			tr.Close();

			// Compare the two
			TObjB b1 =  objOwner1.m_own1.Value as TObjB;
			TObjB b2 =  objOwner2.m_own1.Value as TObjB;
			AreSame(b1.Name, b2.Name);

			// Cleanup
			File.Delete(ReadWritePathName);
		}
		#endregion
	}
	#endregion

}
