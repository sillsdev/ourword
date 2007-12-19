/**********************************************************************************************
 * App:     Josiah
 * File:    JOwnSeq.cs
 * Author:  John Wimbish
 * Created: 27 Oct 2003
 * Purpose: Implements an owning sequence attribute.
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
 *    JOwnSeq m_Sections;
 * 
 * In that class, the constructor should create the new JOwnSeq:
 *    m_Sections = new JOwnSeq("Sections", this, typeof(Section));
 * 
 * The class that will be owned, e.g., JSection, must implement...
 *   + an override of the SortKey attribute if sorting is desired.
 *   + an override of CompareEquals method if comparisons are desired based on something other
 *       than the SortKey (e.g., by the Find command and the optional code that prevents
 *       content-based duplicates.)
 *   + a "Read" constructor.
 */
#endregion
#region Documentation: Features
/* Implements the Cellar concept of an owning sequence.
 * 
 * - The type of object permitted in the sequence is enforced. This is done on all methods
 *   that add/insert/etc objects, under the logic that if we prevent their insertion, then
 *   we don't have to worry about them subsequently.
 * 
 * - An indexer allows the objects to be get/set as, e.g., m_sections[i].
 * 
 * - Ownership
 *   + When an obj is added to the JOwnSeq, its Owner attribute is set to the sequence's owner
 *   + When an obj is removed, its Owner is set to null.
 *   + The obj cannot be owned by the JOwnSeq and by anything else.
 *   + An obj cannot appear more than once in the JOwnSeq (as defined by reference).
 * 
 * - I/O to XML file
 *   + Write method iterates through all members and writes then out to the stream
 *   + Read method populates the owning sequence from the stream, creating objects of
 *       the sequence's signature.
 * 
 * - A ContentEquals method tests to objects to see if they have the same contents,
 *   as defined by the programmer for each type of object. By default, objects with the
 *   same content are not permitted in the sequence.
 *   + A switch supresses an exception being thrown if duplication is attempted.
 *   + A switch turns off this behavior so that duplicates are permitted.
 *   + The duplicate checking is turned off during a Read operation for performance.
 * 
 * - Sorting and searching
 *   + The default is an unsorted list, in which objects are stored in the order
 *       manually set by each Append/InsertAt/indexer command.
 *   + A sequence can be declared as sorted. 
 *   + When sorted, Append and InsertAt place the new object in the correct position
 *       in the list.
 *   + The indexer's set method throws an exception for a sorted sequence.
 *   + The JObjects must have a definition for SortKey.
 *   + A Find command supports locating an object via its SortKey.
 *   + A FindAll command returns an ArrayList containing all hits (that is, supports
 *       duplicates.)
 *   + Sorting can be temporarily disabled, e.g., for adding a lot of objects at once
 *       for performance reasons. 
 *   + Sorting is disabled for the Read method (for better performance.)
 * 
 * - Merging. The read method operates with both an Overwrite mode and a Merge mode.
 *   The merge mode means:
 *   a. append new objects that are not duplicates of existing ones
 *   b. keep old objects that are not in the read stream
 *   c. for new objects that are also old ones, either:
 *      - keep the original, or (kMergeOriginal)
 *      - keep the new version, or (kMergeNew)
 *   (Note: A merge happens when data has already been read in the first time. So we 
 *   can assume the data is already there, and that the mergeOption is set as part 
 *   of the conceptual model setup, rather than being read in with the data.)
 */
#endregion
#region Documentation: Stories not yet implemented
/* Add to the merge routine the following options for collisions:
 * - make a decision based on some criteria (e.g., modify date), or (kMergeCompare - IMergeCompare)
 * - ask the user which one to keep. (Must be implemented in IMergeCompare)
 */
#endregion

namespace JWdb
{
	// Exceptions ----------------------------------------------------------------------------
	#region Exception: eAlreadyOwned - Attempt to set owned obj to another owner
	public class eAlreadyOwned : eJosiahException
	{
		public eAlreadyOwned(string sMethodName)
			: base("Attempt to set an owned object to another owner - " + sMethodName)
		{}
	}
	#endregion

	// Enumerations --------------------------------------------------------------------------
	#region enum Merge - kNone, kKeepOld, kKeepNew, kAskOldObject
	public enum Merge 
	{    // In the case of a duplicates-not-allowed sequence. read options are:
		kNone,             // Clear out the old sequence, replace it entirely with the new
		kKeepOld,          // If duplicates found, keep the original version
		kKeepNew,          // If duplicates found, replace the old with the new
		kAskOldObject      // If duplicates found, ask the old object what to do
	};
	#endregion

	public class JOwnSeq : JSeq
	{
		// Public attributes -----------------------------------------------------------------
		#region Attribute: MergeOption{g/s} - how to do a merge when reading in
		public Merge MergeOption
		{
			get { return m_mergeOption; }
			set { m_mergeOption = value; }
		}
		private Merge m_mergeOption = Merge.kNone;
		#endregion

		// Private attributes ----------------------------------------------------------------
		static private string m_sTag = "ownseq"; // xml tag for I/O

		// Scaffolding -----------------------------------------------------------------------
		#region Constructor(sName,objOwner,type) - minimal arguments
		public JOwnSeq(string sName, JObject objOwner, Type signature)
			: base(sName, objOwner, signature, true, false)
		{
		}
		#endregion
		#region Constructor(sName,objOwner,type,bAvoidDuplicates,bIsSorted) - specifify optional arguments
		public JOwnSeq(string sName, JObject objOwner, Type signature, 
			bool bAvoidDuplicates, bool bIsSorted)
			: base(sName, objOwner, signature, bAvoidDuplicates, bIsSorted)
		{
		}
		#endregion

		// List membership (overrides add ownership handling) --------------------------------
		#region Indexer[] - Adds ownership support to the base indexer
		override public JObject this [ int index ]
		{
			get
			{
				return base[index];
			}
			set
			{
				((JObject)m_list[index]).Owner = null;
				base[index] = value;
				value.Owner = Owner;        // The object is now owned
			}
		}
		#endregion
		#region Method: void InsertAt(iPos,obj) - Adds ownership support to the base method
		override public void InsertAt(int iPos, JObject obj)
		{
			// Do the insertion via the base class method
			base.InsertAt(iPos, obj);
			// Set the object to have an owner
			obj.Owner = Owner; 
		}
		#endregion
		#region Method: void Append(obj) - Adds ownership support to the base method
		override public void Append(JObject obj)
			// Appends the object to the end of the list (for an unsorted list). For a
			// sorted list, the object is placed into its correct sorted position. In
			// the case of duplicates in a sorted list, the new object is placed after 
			// any that are identical.
		{
			base.Append(obj);
			obj.Owner = Owner;              // The object is now owned
		}
		#endregion
		#region Method: void RemoveAt(iPos) - Adds ownership support to the base method
		override public JObject RemoveAt(int iPos)
		{
			JObject obj = base.RemoveAt(iPos);
			if (null != obj)
				obj.Owner = null;
			return obj;
		}
		#endregion
		#region Method: void Remove(obj) - Adds ownership support to the base method
		override public void Remove(JObject obj)
		{
			base.Remove(obj);

			// The object now has no owner
			obj.Owner = null;
		}
		#endregion
		#region Method: void RemoveAll() - removes all of the JObjects from the list
		virtual public void RemoveAll()
			// With Clear(), we actually clear out all owned objets in the objects.
			// With RemoveAll, we merely empty out the list, but we don't do anything
			// about the owned objects; thus the object is left intact and can be
			// still used elsewhere.
		{
			// Clear out the owners
			foreach( JObject obj in m_list)
				obj.Owner = null;

			// Remove them from the list
			base.Clear();
		}
		#endregion
		#region Method: void Clear() - Adds ownership support to the base method
		override public void Clear()
		{
			// Clear objects lower down, then remove owner from all objects
			foreach( JObject obj in m_list)
			{
				obj.Clear();
				obj.Owner = null;                
			}
			base.Clear();
		}
		#endregion
		#region Method: void Replace(objOld, objNew) - replaces an object in the list
		override public void Replace(JObject objOld, JObject objNew)
		{
			base.Replace(objOld, objNew);

			// The old object now has no owner
			objOld.Owner = null;

			// But the new one does
			objNew.Owner = Owner;
		}
		#endregion

		// I/O -------------------------------------------------------------------------------
		#region Attribute: OpeningXmlTagLine - e.g., "<ownseq Name="Section">
		public override string OpeningXmlTagLine
		{
			get { return "<" + m_sTag + " Name=\"" + Name + "\">"; }
		}
		#endregion
		#region Attribute: ClosingXmlTagLine - e.g., "</ownseq>
		private string ClosingXmlTagLine
		{
			get { return "</" + m_sTag + ">"; }
		}
		#endregion
		#region Method: Write(TextWriter) - writes the owning sequence and its objects to xml file
		public override void Write(TextWriter tw, int nIndent)
		{
			if (m_list.Count > 0)
			{
				tw.WriteLine(IndentPadding(nIndent) + OpeningXmlTagLine);
				foreach (JObject obj in m_list)
				{
					obj.Write(tw, nIndent + 1);
				}
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

			// Make sure we're starting from an empty sequence (e.g., an overwrite)
			if (Merge.kNone == MergeOption)
				Clear();

			// A really long sequence (e.g., a dictionary) could get bogged down by this
			// checking. So we are making an assumption that the xml file has not been
			// corrupted, and is thus the well-formed data we put out--and therefore that
			// validity checking is not necessary.
			SuspendDuplicatesCheck = true; 

			// A really long sequence would take longer if each and every Append required
			// the sorting code to execute. So we suspend sorting during the read, and
			// then resume it when done.
			bool bPushIsSorted = IsSorted;
			IsSorted = false;

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
				_MergeObject(obj);
			}
			SuspendDuplicatesCheck = false; // Resume checking for normal operations
			IsSorted = bPushIsSorted;       // Resume sorting if appropriate
		}
		#endregion
		#region Method: void _MergeObject(JObject) - internal merge guts
		private void _MergeObject(JObject objNew)
		{
			// If kNone is the merge option, then we aren't doing a merge; so just append the
			// new one and return.
			if (Merge.kNone == MergeOption)
			{
				Append(objNew);
				return;
			}

			// For duplicates, we need to do a Content-based compare, so we first see if
			// we have any duplicates. If not, then go ahead and append and return.
			ArrayList listOldObjsPreliminary = FindAll(objNew.SortKey);
			ArrayList listOldObjsFinal = new ArrayList();
			foreach (JObject obj in listOldObjsPreliminary)
			{
				if (obj.ContentEquals(objNew))
					listOldObjsFinal.Add(obj);
			}
			if (listOldObjsFinal.Count == 0)
			{
				Append(objNew);
				return;
			}

			// If the MergeOption is kKeepOld, then now that we've verified that we have
			// and old one, we know we can just return without doing anything, and
			// therefor tossing the new one.
			if (Merge.kKeepOld == MergeOption)
				return;

			// At this point, we have at least one match with the proposed new one. If we
			// have exactly one match, we can readily make the decision.
			if (listOldObjsFinal.Count == 1)
			{
				switch (MergeOption)
				{
					case Merge.kKeepNew: 
						Remove((JObject)listOldObjsFinal[0]);
						Append(objNew);
						break;
					case Merge.kAskOldObject: 
						// TODO: Implement this.
						break;
				}
				return;
			}

			// At this point, we have a match with multiple duplicates. We have no way to
			// know if the proposed new object matches one that is already there, except
			// if we ask the user to examine each and every one.

			// TODO: Implement multiple-duplicate merge code. kKeepNew is probably best
			// done by pretending like it is kAsk. We need to ask one at a time going
			// through the list; once the user answers "Yes", we replace the one we're
			// currently presenting to him and quit iterating.
			// Meanwhile....

			// All else fails, just append the new one.
			Append(objNew);
		}
		#endregion
	}

	#region TEST
	#region Dummy Classes for Testing
	public class TObject1 : TObject 
	{ 
		public int m_n = 0;
		public TObject1(string s) : base(s) {}
		public TObject1(string sLine, TextReader tr)
		{
			if (sLine.StartsWith("<test "))
			{
				int iStart = 12;
				int nLength = sLine.IndexOf("\">") - iStart;
				Name = sLine.Substring(iStart, nLength);
			}
		}
	}

	public class TObject : JObject
	{
		// ZAttrs ----------------------------------------------------------------------------
		#region BAttr{g/s}: string Name
		public string Name
		{
			get
			{
				return m_sName;
			}
			set
			{
				m_sName = value;
			}
		}
		private string m_sName;
		#endregion
		#region BAttr{g/s}: string Description
		public string Description
		{
			get
			{
				return m_sDescription;
			}
			set
			{
				m_sDescription = value;
			}
		}
		private string m_sDescription;
		#endregion
		#region Method void DeclareAttrs()
		protected override void DeclareAttrs()
		{
			base.DeclareAttrs();
			DefineAttr("Name",        ref m_sName);
			DefineAttr("Description", ref m_sDescription);
		}
		#endregion

		public TObject() {}
		public TObject(string sName)
		{
			Name = sName;
		}
		public TObject(string sLine,TextReader tr)
		{
			if (sLine.StartsWith("<test "))
			{
				int iStart = 12;
				int nLength = sLine.IndexOf("\">") - iStart;
				Name = sLine.Substring(iStart, nLength);
			}
		}
		public override string SortKey { get { return Name; } }
	}
	#endregion

	public class Test_JOwnSeq : Test
	{
		// Scaffolding ----------------------------------------------------------------------
		#region Constructor()
		public Test_JOwnSeq()
			: base("JOwnSeq")
		{
			AddTest( new IndividualTest( Test_Ownership ),      "Test_Ownership" );
			AddTest( new IndividualTest( OwnershipUniqueness ), "OwnershipUniqueness" );
			AddTest( new IndividualTest( ObjectUniqueness ),    "ObjectUniqueness" );
			AddTest( new IndividualTest( OwnerStoresSelf ),     "OwnerStoresSelf" );
			AddTest( new IndividualTest( ReadWrite ),           "ReadWrite" );
			AddTest( new IndividualTest( MergeNone ),           "MergeNone" );
			AddTest( new IndividualTest( MergeKeepOld ),        "MergeKeepOld" );
			AddTest( new IndividualTest( MergeKeepNew ),        "MergeKeepNew" );
		}
		#endregion
		#region SetUpAnOwningSequence
		void SetUpAnOwningSequence(JOwnSeq os)
			// If anything is changed here, I need to also change the FindUnsorted test.
		{
			os.Clear();
			TObject objA = new TObject("Emily");
			TObject objB = new TObject("David");
			TObject objC = new TObject("Robert");
			TObject objD = new TObject("Christiane");
			os.Append(objA);
			os.Append(objB);
			os.Append(objC);
			os.Append(objD);
		}
		#endregion

		// Ownership -------------------------------------------------------------------------
		#region Test_Ownership
		class TestOwnershipMethod : JObject
		{
			JOwnSeq ownseq;
			public TestOwnershipMethod(Test t)
			{
				ownseq = new JOwnSeq("Ownership", this, typeof(TObject1));

				// Test the Append method
				TObject1 objA = new TObject1("orange");
				ownseq.Append(objA);
				t.IsTrue(objA.Owner == this);

				// Test the InsertAt method
				TObject1 objB = new TObject1("yello");
				ownseq.InsertAt(0,objB);
				t.IsTrue(objB.Owner == this);

				// Test the indexer
				TObject1 objC = new TObject1("red");
				ownseq[0] = objC;
				t.IsTrue(objC.Owner == this);

				// Test that RemoveAt sets owner back to null. 
				ownseq.RemoveAt(0);
				t.IsTrue(objC.Owner == null);

				// When we replaced objB with objC via the indexer, objB should now have a null owner
				t.IsTrue(objB.Owner == null);

				// When we clear out the list, ObjA should have no owner
				ownseq.Clear();
				t.IsTrue(objA.Owner == null);

				// To test Remove, add the objects all back in, them remove one of them.
				ownseq.Append(objA);
				ownseq.Append(objB);
				ownseq.Append(objC);
				t.IsTrue(objB.Owner == this);
				ownseq.Remove(objB);
				t.IsTrue(objB.Owner == null);
			}
		}
		public void Test_Ownership()
		{
			new TestOwnershipMethod(this);
		}
		#endregion
		#region OwnershipUniqueness
		class OwnershipUniquenessMethod : JObject
		{
			JOwnSeq ownseq1;
			JOwnSeq ownseq2;
			public OwnershipUniquenessMethod()
			{
				ownseq1 = new JOwnSeq("unique1", this, typeof(TObject1));
				ownseq2 = new JOwnSeq("unique2", this, typeof(TObject1));
				TObject1 obj = new TObject1("hello");
				ownseq1.Append(obj);
				ownseq2.Append(obj); // should fail, because obj is already owned by ownseq1.
			}
		}
		public void OwnershipUniqueness()
		{
			bool bCaught = false;
			try
			{
				new OwnershipUniquenessMethod();
			}
			catch (eAlreadyOwned)
			{
				bCaught = true;
			}
			IsTrue(bCaught);
		}
		#endregion
		#region ObjectUniqueness
		class ObjectUniquenessMethod : JObject
		{
			JOwnSeq ownseq;
			public ObjectUniquenessMethod()
			{
				ownseq = new JOwnSeq("Unique3", this, typeof(TObject1));

				// We're testing for referential duplicates, not content duplicates, so we
				// must turn off this code.
				ownseq.AvoidDuplicates = false;  

				// Attempt to append the same object twice.
				TObject1 obj = new TObject1("hello");
				ownseq.Append(obj);
				ownseq.Append(obj); // should fail, because obj is already owned by ownseq1.
			}
		}
		public void ObjectUniqueness()
			// The code that protects against ownership uniquess also prevents the same
			// object from appearing more than once in the sequence. So here we just
			// have a test that attempts to place it twice in the sequence, and we
			// test to make sure the exception from OwnershipUniqueness is fired.
		{
			bool bCaught = false;
			try
			{
				new ObjectUniquenessMethod();
			}
			catch (eAlreadyOwned)
			{
				bCaught = true;
			}
			IsTrue(bCaught);
		}
		#endregion
		#region OwnerStoresSelf
		class OwnerStoresSelfMethod : JObject
		{
			JOwnSeq ownseq;
			public OwnerStoresSelfMethod(Test t)
			{
				ownseq = new JOwnSeq("StoresSelf", this, typeof(TObject1));
				t.IsTrue( _test_ContainsAttribute(ownseq) );
			}
		}
		public void OwnerStoresSelf()
		{
			new OwnerStoresSelfMethod(this);
		}
		#endregion

		// I/O -------------------------------------------------------------------------------
		#region Attr{g}: ReadWritePathName - pathname for test file
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
		class ReadWriteMethod : JObject
		{
			JOwnSeq ownseqW;
			JOwnSeq ownseqR;
			JOwnSeq ownseqRS;
			public ReadWriteMethod(Test t, string sPathName)
			{
				// Set up a unsorted owning sequence
				m_bSurpressDuplicateAttrTest = true;
				ownseqW = new JOwnSeq("test", this, typeof(TObject));
				ownseqW.Append(new TObject("Verse"));
				ownseqW.Append(new TObject("Chapter"));
				ownseqW.Append(new TObject("Inscription"));
				ownseqW.Append(new TObject("Lord"));

				// Write it out
				TextWriter tw = JUtil.GetTextWriter(sPathName);
				ownseqW.Write(tw, 0);
				tw.Close();

				// Read in to another unsorted sequence
				ownseqR = new JOwnSeq("test", this, typeof(TObject));
				TextReader tr = JUtil.GetTextReader(sPathName);
				string sLine;
				while ( (sLine = tr.ReadLine()) != null)
				{
					ownseqR.Read(sLine, tr);
				}
				tr.Close();

				// Compare the two
				t.AreSame(4, ownseqR.Count);
				t.AreSame(ownseqW.Count, ownseqR.Count);
				for(int i=0; i<4; ++i)
				{
					TObject objW = ownseqW[i] as TObject;
					TObject objR = ownseqR[i] as TObject;
					t.AreSame( objW.Name,objR.Name );
				}

				// Create an empty sorted sequence
				ownseqRS = new JOwnSeq("test", this, typeof(TObject), false, true);
				tr = JUtil.GetTextReader(sPathName);
				while ( (sLine = tr.ReadLine()) != null)
				{
					ownseqRS.Read(sLine, tr);
				}
				tr.Close();

				// Verify that the sorted sequence is indeed sorted
				for(int i=0; i<ownseqRS.Count - 2; i++)									   
				{
					TObject obj1 = ownseqRS[i] as TObject;
					TObject obj2 = ownseqRS[i+1] as TObject;
					t.IsTrue( obj1.SortKey.CompareTo( obj2.SortKey ) < 0);
				}

				// Cleanup
				File.Delete(sPathName);
			}
		}
		public void ReadWrite()
		{
			new ReadWriteMethod(this, ReadWritePathName);
		}
		#endregion
		#region MergeNone
		class MergeNoneMethod : JObject
		{
			JOwnSeq os;
			JOwnSeq osM;
			public MergeNoneMethod(Test_JOwnSeq parent, string sPathName)
			{
				// Set up a unsorted owning sequence  that we can merge into
				m_bSurpressDuplicateAttrTest = true;
				os = new JOwnSeq("test", this, typeof(TObject), true, false);
				parent.SetUpAnOwningSequence(os);
				string sPath = sPathName + "mn";

				// Set up a merge OS: Test kNone.
				osM = new JOwnSeq("test", this, typeof(TObject), true, false);
				os.MergeOption = Merge.kNone;
				osM.Append(new TObject("Fred"));
				osM.Append(new TObject("Emily"));
				TextWriter tw = JUtil.GetTextWriter(sPath);
				osM.Write(tw,0);
				tw.Close();

				// Do the merge.
				TextReader tr = JUtil.GetTextReader(sPath);
				string sLine;
				while ( (sLine = tr.ReadLine()) != null)
				{
					os.Read(sLine, tr);
				}
				tr.Close();
				File.Delete(sPath);
				parent.AreSame(2, os.Count);
			}
		}
		public void MergeNone()
			// Creates a sequence with several members, writes it,then reads
			// in a different sequence. Tests with MergeOption = kNone,
			// which should result in the first sequence being discarded
			// and the second one read replacing it.
		{
			new MergeNoneMethod(this, ReadWritePathName);
		}
		#endregion
		#region MergeKeepOld
		class MergeKeepOldMethod : JObject
		{
			JOwnSeq os;
			JOwnSeq osM;
			public MergeKeepOldMethod(Test_JOwnSeq parent, string sPathName)
			{
				// Set up a unsorted owning sequence  that we can merge into
				os = new JOwnSeq("Old", this, typeof(TObject), true, false);
				parent.SetUpAnOwningSequence(os);
				foreach (TObject o in os)  // So we can tell the old from the new
					o.Description = "1";
				string sPath = sPathName + "mko";

				// Set up a merge OS: Test kKeepOld.
				osM = new JOwnSeq("OldM", this, typeof(TObject), true, false);
				osM.Append(new TObject("David"));
				osM.Append(new TObject("Emily"));
				TextWriter tw = JUtil.GetTextWriter(sPath);
				osM.Write(tw, 0);
				tw.Close();

				// Do the merge.
				os.MergeOption = Merge.kKeepOld;
				TextReader tr =JUtil.GetTextReader(sPath);
				string sLine;
				while ( (sLine = tr.ReadLine()) != null)
				{
					os.Read(sLine, tr);
				}
				tr.Close();
				File.Delete(sPath);
				parent.AreSame(4, os.Count);
				foreach(TObject o in os)
				{
					parent.AreSame("1", o.Description);
				}			
			}
		}
		public void MergeKeepOld()
			// Creates a sequence with several members, writes it,then reads
			// in a different sequence. Tests with MergeOption = kKeepOld,
			// which should result in the first sequence being nleft intact,
			// which is evidenced by m_n == 1 rather than the default of 0.
		{
			new MergeKeepOldMethod(this, ReadWritePathName);
		}
		#endregion
		#region MergeKeepNew
		class MergeKeepNewMethod : JObject
		{
			JOwnSeq os;
			JOwnSeq osM;
			public MergeKeepNewMethod(Test_JOwnSeq parent, string sPathName)
			{
				// Set up a unsorted owning sequence  that we can merge into
				m_bSurpressDuplicateAttrTest = true;
				os = new JOwnSeq("test", this, typeof(TObject), true, false);
				parent.SetUpAnOwningSequence(os);
				foreach (TObject o in os)  // So we can tell the old from the new
					o.Description = "1";
				string sPath = sPathName + "mkn";

				// Set up a merge OS: Test kKeepNew.
				osM = new JOwnSeq("test", this, typeof(TObject), true, false);
				osM.Append(new TObject("David"));
				osM.Append(new TObject("Emily"));
				TextWriter tw =JUtil.GetTextWriter(sPath);
				osM.Write(tw, 0);
				tw.Close();

				// Do the merge.
				os.MergeOption = Merge.kKeepNew;
				TextReader tr = JUtil.GetTextReader(sPath);
				string sLine;
				while ( (sLine = tr.ReadLine()) != null)
				{
					os.Read(sLine, tr);
				}
				tr.Close();
				File.Delete(sPath);
				parent.AreSame(4, os.Count);
				parent.AreSame( "", ((TObject)os[os.Find("David")]).Description);
				parent.AreSame( "", ((TObject)os[os.Find("Emily")]).Description);
			}
		}
		public void MergeKeepNew()
			// Creates a sequence with several members, writes it,then reads
			// in a different sequence. Tests with MergeOption = kKeepOld,
			// which should result in the first sequence being nleft intact,
			// which is evidenced by m_n == 1 rather than the default of 0.
		{
			new MergeKeepNewMethod(this, ReadWritePathName);
		}
		#endregion
	}
	#endregion
}
