/**********************************************************************************************
 * App:     Josiah
 * File:    JSeq.cs
 * Author:  John Wimbish
 * Created: 27 Oct 2003
 * Purpose: Implements the shared sequence behavior, used by subclasses such as JOwnSeq
 *            and JRefSeq.
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
#region Documentation: Features
/* - Name. The sequence has a name, e.g., "Sections", or "Authors", which is set in the
 *      constructor. 
 * 
 * - Enumeration (e.g., foreach) is supported.
 * 
 * - The owning JObject has a collection of all of its attributes, which it uses for,
 *   e.g., operations like Write where it must iterate through them all. The JSeq 
 *   constructor stores the sequence attribute in that collection.
 * 
 */
#endregion
#region Documentation: Implementation Notes
/* 
 * I use an ArrayList as a private member variable, rather than inheriting from it, because I 
 *    want to prevent the client code from directly making calls to the list (otherwise what 
 *    I am are trying to accomplish in terms of Cellar goals could be circumvented; e.g., 
 *    signature checking and ownership control.
*/
#endregion

namespace JWdb
{
	// Exceptions ----------------------------------------------------------------------------
	#region Exception: eBadSignature - Attempt to use the wrong signature in a method call
	public class eBadSignature : eJosiahException
	{
		public eBadSignature(string sMethodName)
			: base("Attempt to use a non-matching signature in a method call - " + sMethodName)
		{}
	}
	#endregion
	#region Exception: eContentDuplication - Attempt to add a content-duplicate object
	public class eContentDuplication : eJosiahException
		// An attempt was made to add a duplicate to the sequence (e.g., through Append).
		// To support duplicates, turn off the AvoidDuplicates flag. To suppress this
		// exception, turn off the ComplainIfDuplicateAttempted flag.
	{
		public eContentDuplication(string sMethodName)
			: base("Attempt to add a content-duplicate object - " + sMethodName)
		{}
	}
	#endregion
	#region Exception: eNoContentCompare - Attempt to compare content fails with empty string
	public class eNoContentCompare : eJosiahException
		// The default for JObject returns empty strings for the SortKey, which means that
		// ContentEquals() cannot really compare the objects. The subclass is expected
		// to either subclass SortKey, or subclass ContentEquals. Calling ContentEquals
		// without doing either of these results in this exception being thrown.
	{
		public eNoContentCompare(string sMethodName)
			: base("Attempt to compare content fails with empty string - " + sMethodName)
		{}
	}
	#endregion
	#region Exception: eSortedSequence - Can't call this method on a sorted sequence
	public class eSortedSequence : eJosiahException
		// An attempt was made to call a method that only works with a sorted sequence.
	{
		public eSortedSequence(string sMethodName)
			: base("Can't call this method on a sorted sequence - " + sMethodName)
		{}
	}
	#endregion
	#region Exception: eUnsortedSequence - Can't call this method on an unsorted sequence
	public class eUnsortedSequence : eJosiahException
		// An attempt was made to call a method that only works with a sorted sequence.
	{
		public eUnsortedSequence(string sMethodName)
			: base("Can't call this method on an unsorted sequence - " + sMethodName)
		{}
	}
	#endregion

	// Class JSeq ----------------------------------------------------------------------------
	public class JSeq : JAttr, IEnumerator, IComparer
	{
		// Public attributes -----------------------------------------------------------------
		#region Attribute: int Count{g} - the number of objects in the sequence
		public int Count { get { return m_list.Count; } }
		#endregion
		#region Attribute: IsSorted({g/s} - Determines whether or not the sequence is sorted
		public bool IsSorted
		{
			get
			{
				return m_bIsSorted;
			}
			set
			{
				// If we are switching from unsorted to sorted, then we need to DO the sort.
				if (m_bIsSorted == false && value == true)
					_Sort();
				m_bIsSorted = value;
			}
		}
		private bool m_bIsSorted = false;
		#endregion

		// Non-Public attributes -------------------------------------------------------------
		#region Private Attribute: ArrayList m_list - stores the sequence
		protected ArrayList m_list = null;
		#endregion
		
		// Scaffolding -----------------------------------------------------------------------
		#region public Constructor(...)
		public JSeq(string sName, JObject objOwner, Type signature, bool bAvoidDuplicates,
			bool bIsSorted)
			: base(sName, objOwner, signature)
		{
			// Initialize the list object
			Debug.Assert(m_list == null);
			m_list = new ArrayList();
			Debug.Assert(m_list != null);

			// Allow duplicates?
			AvoidDuplicates = bAvoidDuplicates;

			// Sorted list?
			m_bIsSorted = bIsSorted;
		}
		#endregion

		// Enumerator ------------------------------------------------------------------------
		#region Attribute: IEnumerator.Current - Returns the current JObject
		public object Current
			// Returns the JObject at the current position
		{
			get 
			{ 
				if (-1 == m_iEnumeratorPos || m_iEnumeratorPos == Count || !m_bEnumeratorValid)
					throw new InvalidOperationException();
				return m_list[m_iEnumeratorPos];
			}
		}
		#endregion
		#region Method: void IEnumerator.Reset() - rewinds the position to before the first object
		public void Reset()
		{
			if (!m_bEnumeratorValid)
				throw new InvalidOperationException();
			m_iEnumeratorPos = -1;
		}
		#endregion
		#region Method: bool IEnumerator.MoveNext() - moves to the next object in the sequence
		public bool MoveNext()
		{
			if (!m_bEnumeratorValid)
				throw new InvalidOperationException();
			if (m_iEnumeratorPos < Count - 1)
			{
				++m_iEnumeratorPos;
				return true;
			}
			return false;
		}
		#endregion
		#region Method: IEnumerator.GetEnumerator() - initializes the enumeration
		public virtual IEnumerator GetEnumerator()
		{
			m_bEnumeratorValid = true;
			Reset();
			return this;
		}
		#endregion
		#region Private: int m_iEnumeratorPos - current position of the enumerator
		// The current position of the enumerator:
		//   - a value of -1 means the "reset" position
		//   - a value equal to Count is past the end.
		private int m_iEnumeratorPos = -1; 
		#endregion
		#region Private: bool m_bEnumeratorValid - lets us know if [] has been modified
		private bool m_bEnumeratorValid = false;
		#endregion
		#region Private Method: InvalidateEnumerator() - signals that the enumerator is invalid now
		private void InvalidateEnumerator()
		{
			m_bEnumeratorValid = false;
			DeclareDirty();
		}
		#endregion

		// Duplicates control - option to prevent duplicates ---------------------------------
		#region Attribute: AvoidDuplicates{g/s} - T if no content duplicates are allowed
		public bool AvoidDuplicates
		{
			get { return m_bAvoidDuplicates; }
			set { m_bAvoidDuplicates = value;}
		}
		private bool m_bAvoidDuplicates = true;
		#endregion
		#region Attribute: ComplainIfDuplicateAttempted{g/s} - T if an exception should be raised
		public bool ComplainIfDuplicateAttempted
		{
			get { return m_bComplainIfDuplicateAttempted; }
			set { m_bComplainIfDuplicateAttempted = value;}
		}
		private bool m_bComplainIfDuplicateAttempted = true;
		#endregion
		#region Method: void CheckForDuplicates(obj) - throws exception is a duplicate is found
		public void CheckForDuplicates(JObject obj)
		{
			if (SuspendDuplicatesCheck)
				return;

			if (!AvoidDuplicates)
				return;

			foreach (JObject o in m_list)
			{
				if (o.ContentEquals(obj))
				{
					if (ComplainIfDuplicateAttempted)
						throw new eContentDuplication("Validate...");
					return;
				}
			}
		}
		#endregion
		#region Attribute SuspendDuplicatesCheck{g/s} - can turn off during, e.g., Read, for performance
		public bool SuspendDuplicatesCheck
		{
			get { return m_bSuspendDuplicatesCheck;  }
			set { m_bSuspendDuplicatesCheck = value; }
		}
		private bool m_bSuspendDuplicatesCheck = false; // T for Read, to make faster
		#endregion

		// Putting objects into the list; removing them from the list ------------------------
		#region Indexer[] - provides array access (get/set)
		virtual public JObject this [ int index ]
		{
			get
			{
				if (index < 0 || index >= m_list.Count)
					return null;
				return (JObject)m_list[index];
			}
			set
			{
				// This method assumes an unsorted sequence. It is not meaningful for an
				// sorted sequence.
				if (IsSorted)
					throw new eSortedSequence("indexer");

				// Make sure the proposed object is valid for this attribute
				CheckCorrectSignature(value);
				CheckForDuplicates(value);

				// Set the value
				m_list[index] = value;

				// Any active enumerator is now invalid
				InvalidateEnumerator(); 
			}
		}
		#endregion
		#region Method: void InsertAt(iPos,obj) - places JObject in the list, scooting others down
		private void _InsertAt(int iPos, JObject obj)
			// Hopefully temporary until I can learn how to call base method from base method.
			// This need arose from the JSeq.Append method. Once solved, move the code back
			// to the regular InsertAt method.
		{
			CheckCorrectSignature(obj);
			CheckForDuplicates(obj);

			// For a sorted list, the iPos passed in is worthless. So we do a search to find the
			// correct place to insert the new object.
			if (IsSorted)
				iPos = _FindInsertionPosition(obj.SortKey);

			m_list.Insert(iPos, obj);
			InvalidateEnumerator();              // Any active enumerator is now invalid
		}
		virtual public void InsertAt(int iPos, JObject obj)
		{
			_InsertAt(iPos, obj);
		}
		#endregion
		#region Method: void Append(obj) - appends a JObject to the end of the list
		virtual public void Append(JObject obj)
			// Appends the object to the end of the list (for an unsorted list). For a
			// sorted list, the object is placed into its correct sorted position. In
			// the case of duplicates in a sorted list, the new object is placed after 
			// any that are identical.
		{
			CheckCorrectSignature(obj);
			CheckForDuplicates(obj);

			// If we have an unsorted list, just append to the bottom of the list.
			// Otherwise we call the InsertAt (with a dummy position), as this method
			// has the smarts to insert the object in the correct possition in the
			// sorted list.
			if (!IsSorted)
			{
				m_list.Add(obj);
				InvalidateEnumerator();              // Any active enumerator is now invalid
			}
			else
			{
				_InsertAt(0, obj); 
			}
		}
		#endregion
		#region Method: JObject RemoveAt(iPos) - removes the JObject at iPos, scooting others forward
		virtual public JObject RemoveAt(int iPos)
		{
			if (iPos >= 0 || iPos < m_list.Count)
			{
				JObject obj = (JObject)m_list[iPos];
				m_list.RemoveAt(iPos);
				InvalidateEnumerator();              // Any active enumerator is now invalid
				return obj;
			}
			return null;
		}
		#endregion
		#region Method: void Remove(obj) - removes the JObject from the list
		virtual public void Remove(JObject obj)
		{
			m_list.Remove(obj);
			InvalidateEnumerator();              // Any active enumerator is now invalid
		}
		#endregion
		#region Method: void Clear() - removes all JObjects from the list
		virtual public void Clear()
		{
			m_list.Clear();
			InvalidateEnumerator();              // Any active enumerator is now invalid
		}
		#endregion
		#region Method: void Replace(objOld, objNew) - replaces an object in the list
		virtual public void Replace(JObject objOld, JObject objNew)
		{
			int iPos = m_list.IndexOf(objOld);
			m_list[iPos] = objNew;
			InvalidateEnumerator();              // Any active enumerator is now invalid

			// Do the sort if a sorted list (just sort the entire list; perhaps later if
			// efficiency requires, we can re-do this to be better.
			if (IsSorted)
				m_list.Sort(this);
		}
		#endregion
		#region Method: void MoveTo(int iObj, int iNewPos)
		public void MoveTo(int iObj, int iNewPos)
		{
			if (!IsSorted)
			{
				// Is there anything to do?
				if (iObj == iNewPos)
					return;

				// If the NewPos if after the object, we need to decrement iNewPos, because
				// we are going to delete the iObj, and thus reduce the Count of the list.
//				if (iNewPos > iObj)
//					--iNewPos;


				// Remove the object from the list, then insert it in the desired position
				JObject obj = m_list[iObj] as JObject;
				m_list.RemoveAt(iObj);
				m_list.Insert(iNewPos, obj);

				// Any active enumerator is now invalid
				InvalidateEnumerator(); 
			}
		}
		#endregion

		// Sorting, Finding, etc. -----------------------------------------------------------
		#region Method: void ForceSort() - Forces a sort on the data, whether needed or not
		public void ForceSort()
			// Sometimes the content within a JObject will have changed, such that its
			// SortKey will return something different than before. I include this method
			// so that sorting can be forced, even when the system might not recognize the
			// need.
		{
			m_list.Sort(this);
			InvalidateEnumerator();
		}
		#endregion
		#region Method: void _Sort() - sorts the data in the sequence
		protected void _Sort()
		{
			// Nothing to do if we are already sorted
			if (IsSorted)
				return;

			// Call the ArrayList method, passing in the IComparer interface, which calls our
			// Compare method.
			m_list.Sort(this);
			InvalidateEnumerator();
		}
		#endregion
		#region Method: int Compare(x,y) - required IComparer implementation for the Sort method
		public int Compare(object x, object y)
		{
			return ((JObject)x).SortKey.CompareTo( ((JObject)y).SortKey );
		}
		#endregion
		#region Method: int _FindInsertPosition(sSortKey) - returns insertion point in a sorted sequence
		internal int _FindInsertionPosition(string sSortKey)
		{
			// This method assumes a sorted sequence. It is not meaningful for an
			// unsorted sequence.
			if (!IsSorted)
				throw new eUnsortedSequence("_FindInsertionPosition");

			// For an empty list, the answer is 0. (If we try to proceed further, we
			// get an null references.
			if (Count == 0)
				return 0;

			// "i" will represent the position of the key if found. A value of -1 means we
			// did not find it.
			int i = -1;

			// We'll set iTop to the first object, and iBottom to the last one
			int iTop = 0;                                       // e.g., "a"
			int iBottom = Count - 1;                            // e.g., "z"

			// We keep narrowing down the range until there are no items left to look at.
			while ( iTop <= iBottom)
			{
				// We'll examine the item in the middle between iTop and iBottom
				i = (iTop + iBottom) / 2;                       // e.g., "n"
				// If the item is less than or greater than, then we know to narrow
				// the range to the appropriate half.
				if (sSortKey.CompareTo(this[i].SortKey) < 0 )    // "key" < "n"
					iBottom = i - 1;                            // e.g., "m"
				else if (sSortKey.CompareTo(this[i].SortKey) > 0)
					iTop = i + 1;                               // e.g., "o"

					// Otherwise, we found it!
				else
					break;
			}
			
			// If i is sitting on the target, keep iterating
			while (i < Count && sSortKey.CompareTo(this[i].SortKey) >= 0)
			{
				++i;
			}
			return i;
		}
		#endregion
		#region Method: int Find(sSortKey) - returns index of obj, or -1 if not found
		public int Find(string sSortKey)
			// Returns the index of the object whose SortKey is identical to the sSortKey
			// that is passed in. Returns -1 if not found. 
		{
			if (null == sSortKey || sSortKey.Length == 0)
				return -1;

			// For an unsorted list, we must go through the collection sequentially, 
			// examining each one.
			if (!IsSorted)
			{
				int k = 0;
				foreach (JObject obj in m_list)
				{
					if (sSortKey.CompareTo(obj.SortKey) == 0)
						return k;
					k++;
				}
				return -1;
			}

			// Otherwise, we have a sorted list, and can therefore do the faster binary
			// search.

			// "i" will represent the position of the key if found. A value of -1 means we
			// did not find it.
			int i = -1;

			// We'll set iTop to the first object, and iBottom to the last one
			int iTop = 0;                                       // e.g., "a"
			int iBottom = Count - 1;                            // e.g., "z"

			// We keep narrowing down the range until there are no items left to look at.
			while ( iTop <= iBottom)
			{
				// We'll examine the item in the middle between iTop and iBottom
				i = (iTop + iBottom) / 2;                       // e.g., "n"
				// If the item is less than or greater than, then we know to narrow
				// the range to the appropriate half.
				if (sSortKey.CompareTo(this[i].SortKey) < 0 )    // "key" < "n"
					iBottom = i - 1;                            // e.g., "m"
				else if (sSortKey.CompareTo(this[i].SortKey) > 0)
					iTop = i + 1;                               // e.g., "o"

				// Otherwise, we found it! If there are duplicates, backtrack to get
				// the first one.
				else
				{
					if (!AvoidDuplicates)
					{
						while (i > 0 && sSortKey.CompareTo(this[i-1].SortKey) == 0)
							--i;
					}
					return i;
				}
			}
			return -1;
		}
		#endregion
		#region Method: ArrayList FindAll(sSortKey) - returns all hits (duplicates)
		public ArrayList FindAll(string sSortKey)
		{
			ArrayList list = new ArrayList();

			// For a sorted list, we use the Binary search in the Find method to locate
			// the first hit, then iterate through to find any other matches.
			if (IsSorted)
			{
				int i = Find(sSortKey);
				if (i != -1)
				{
					while ( i < Count && sSortKey.CompareTo(this[i].SortKey) == 0)
					{
						list.Add(this[i]);
						i++;
					}
				}
			}

				// For the unsorted list, we have to check through each and every one
				// looking for matches. Thus this one can potentially take longer, depending
				// on the size of the list.
			else
			{
				foreach (JObject obj in m_list)
				{
					if (sSortKey.CompareTo(obj.SortKey) == 0)
						list.Add(obj);
				}
			}

			return list;
		}
		#endregion
		#region Method: int FindObject(JObject) - returns the index of the obj, or -1 if not found
		public int FindObj(JObject obj)
		{
			for(int i=0; i<m_list.Count; i++)
			{
				if (m_list[i] == obj)
					return i;
			}
			return -1;
		}
		#endregion
	}

	#region TEST
	#region class TObj - dummy class for populating a sequence
	public class TObj : JObject
		// Test Object for use in testing out the sequence methods
	{
		public string m_sName;
		public TObj() {}
		public TObj(string sName)
		{
			m_sName = sName;
		}
		public override string SortKey 
		{ 
			get { return m_sName; } 
		}
	}
	#endregion
	#region class TObjSub - dummy class for testing signature control
	public class TObjSub : TObj
	{
		public TObjSub(string sName) : base(sName) {}
	}
	#endregion

	public class Test_JSeq : Test
	{
		#region Constructor()
		public Test_JSeq()
			: base("JSeq")
		{
			AddTest( new IndividualTest( SignatureControl_Append ),     "SignatureControl_Append" );
			AddTest( new IndividualTest( SignatureControl_InsertAt ),   "SignatureControl_InsertAt" );
			AddTest( new IndividualTest( SignatureControl_Indexer ),    "SignatureControl_Indexer" );
			AddTest( new IndividualTest( EnumeratorBasics ),            "EnumeratorBasics" );
			AddTest( new IndividualTest( EnumeratorIllegalAppend ),     "EnumeratorIllegalAppend" );
			AddTest( new IndividualTest( EnumeratorIllegalInsertAt ),   "EnumeratorIllegalInsertAt" );
			AddTest( new IndividualTest( EnumeratorIllegalIndexerSet ), "EnumeratorIllegalIndexerSet" );
			AddTest( new IndividualTest( EnumeratorIllegalRemove ),     "EnumeratorIllegalRemove" );
			AddTest( new IndividualTest( EnumeratorIllegalRemoveAt ),   "EnumeratorIllegalRemoveAt" );
			AddTest( new IndividualTest( EnumeratorIllegalClear ),      "EnumeratorIllegalClear" );
			AddTest( new IndividualTest( AvoidDuplicatesAppend ),       "AvoidDuplicatesAppend" );
			AddTest( new IndividualTest( AvoidDuplicatesInsertAt ),     "AvoidDuplicatesInsertAt" );
			AddTest( new IndividualTest( AvoidDuplicatesIndexerSet ),   "AvoidDuplicatesIndexerSet" );
			AddTest( new IndividualTest( FindSorted ),                  "FindSorted" );
			AddTest( new IndividualTest( FindUnsorted ),                "FindUnsorted" );
			AddTest( new IndividualTest( FindInsertPosition ),          "FindInsertPosition" );
			AddTest( new IndividualTest( SortedInsert ),                "SortedInsert" );
			AddTest( new IndividualTest( SortedList_CantIndexer ),      "SortedList_CantIndexer" );
			AddTest( new IndividualTest( SwitchSorting ),               "SwitchSorting" );
			AddTest( new IndividualTest( DuplicateFinds ),              "DuplicateFinds" );
		}
		#endregion
		#region Method: SetUpUnsortedOwningSequence(JOwnSeq seq)
		public void SetUpUnsortedOwningSequence(JOwnSeq seq)
			// Creates a sequence, unsorted, containing (in order):
			//   Emily
			//   Robert
			//   David
			//   Christiane
		{
			seq.Clear();
			seq.Append( new TObject("Emily"));
			seq.Append( new TObject("Robert"));
			seq.Append( new TObject("David"));
			seq.Append( new TObject("Christiane"));
			AreSame(4, seq.Count);
		}
		#endregion

		// Correct signature of owned objects ------------------------------------------------
		#region SignatureControl_Append
		class SignatureControl_AppendMethod : JObject
		{
			JOwnSeq seq;
			public SignatureControl_AppendMethod(Test_JSeq parent)
			{
				// Start with our standard sequence
				seq = new JOwnSeq("test", this, typeof(TObject));
				parent.SetUpUnsortedOwningSequence(seq);

				// Attempt to append an obj of a different type
				TObjSub obj = new TObjSub("hello");
				seq.Append(obj);
			}
		}
		public void SignatureControl_Append()
			// We declare a JOwnSeq with a signature of one type, and then attempt to
			// add an object to it of a different type. We expect an exception. If we check
			// anytime an object is being added, then we prevent the sequence from ever
			// having bad objects.
		{
			bool bCaught = false;
			try
			{
				new SignatureControl_AppendMethod(this);
			}
			catch (eBadSignature)
			{
				bCaught = true;
			}
			IsTrue(bCaught);
		}
		#endregion
		#region SignatureControl_InsertAt
		class SignatureControl_InsertAtMethod : JObject
		{
			JOwnSeq seq;
			public SignatureControl_InsertAtMethod(Test_JSeq parent)
			{
				// Start with our standard sequence
				seq = new JOwnSeq("test", this, typeof(TObject));
				parent.SetUpUnsortedOwningSequence(seq);

				// Attempt to insert an obj of a different type
				TObjSub obj = new TObjSub("hello");
				seq.InsertAt(0, obj);
			}
		}
		public void SignatureControl_InsertAt()
			// Same as Test_SignatureControl_Append; except this time for the InsertAt
			// method.
		{
			bool bCaught = false;
			try
			{
				new SignatureControl_InsertAtMethod(this);
			}
			catch (eBadSignature)
			{
				bCaught = true;
			}
			IsTrue(bCaught);
		}
		#endregion
		#region SignatureControl_Indexer
		class SignatureControl_IndexerMethod : JObject
		{
			JOwnSeq seq;
			public SignatureControl_IndexerMethod(Test_JSeq parent)
			{
				// Start with our standard sequence
				seq = new JOwnSeq("test", this, typeof(TObject));
				parent.SetUpUnsortedOwningSequence(seq);

				// Attempt to replace an obj of a different type
				TObjSub obj = new TObjSub("hello");
				seq[0] = obj;
				AllAttrs.Clear();
			}
		}
		public void SignatureControl_Indexer()
			// Same as Test_SignatureControl_Append; except this time for the indexer
			// method.
		{
			bool bCaught = false;
			try
			{
				new SignatureControl_IndexerMethod(this);
			}
			catch (eBadSignature)
			{
				bCaught = true;
			}
			IsTrue(bCaught);
		}
		#endregion

		// Enumerator ------------------------------------------------------------------------
		#region EnumeratorBasics
		class EnumeratorBasicsMethod : JObject
		{
			JOwnSeq seq;
			public EnumeratorBasicsMethod(Test_JSeq parent)
			{
				// Start with our standard sequence
				seq = new JOwnSeq("test", this, typeof(TObject));
				parent.SetUpUnsortedOwningSequence(seq);

				// Make sure that the foreach statement returns the elements in the order we expect
				int i = 0;
				foreach (JObject obj in seq)
				{
					if (i == 0)
						parent.IsTrue(obj.SortKey == "Emily");
					if (i == 1)
						parent.IsTrue(obj.SortKey == "Robert");
					if (i == 2)
						parent.IsTrue(obj.SortKey == "David");
					if (i == 3)
						parent.IsTrue(obj.SortKey == "Christiane");
					++i;
				}

				// Make sure the enumerator resets correctly for a second pass
				i = 0;
				foreach (JObject obj in seq)
				{
					if (i == 2)
						parent.IsTrue(obj.SortKey == "David");
				}
			}
		}
		public void EnumeratorBasics()
			// Purpose: See that we can enumerate sequentially through a set of objects.
			//
			// Test:
			// 1. Create our standard ordered sequence containing four objects
			// 2. Using the foreach command, see that the four objects appear
			//     in the enumeration in the order expected.
			// 3. Run through the foreach command again, to make sure Reset works correctly.
		{
			new EnumeratorBasicsMethod(this);
		}
		#endregion
		#region EnumeratorIllegalAppend
		class EnumeratorIllegalAppendMethod : JObject
		{
			JOwnSeq seq;
			public EnumeratorIllegalAppendMethod(Test_JSeq parent)
			{
				// Start with our standard sequence
				seq = new JOwnSeq("test", this, typeof(TObject));
				parent.SetUpUnsortedOwningSequence(seq);

				// Adding an object in the midst of a foreach should be illegal
				TObject objNew = new TObject("New");
				int i = 0;
				foreach (JObject o in seq)
				{
					if (i++ == 2)
					{
						seq.Append(objNew);
					}
					// meaningless statement to avoid the compiler warning
					parent.IsTrue(o != objNew); 
				}			
			}
		}
		public void EnumeratorIllegalAppend()
			// Purpose: One an object has been appended to the list, it is illegal to
			// continue with the enumeration. 
			//
			// Test:
			// 1. Create our standard ordered sequence containing four objects
			// 2. While in the midst of the foreach, add a fifth object
			// 3. The next call in the foreach should thrown an exception.
		{
			bool bCaught = false;
			try
			{
				new EnumeratorIllegalAppendMethod(this);
			}
			catch (InvalidOperationException)
			{
				bCaught = true;
			}
			IsTrue(bCaught);
		}
		#endregion
		#region EnumeratorIllegalInsertAt
		class EnumeratorIllegalInsertAtMethod : JObject
		{
			JOwnSeq seq;
			public EnumeratorIllegalInsertAtMethod(Test_JSeq parent)
			{
				// Start with our standard sequence
				seq = new JOwnSeq("test", this, typeof(TObject));
				parent.SetUpUnsortedOwningSequence(seq);

				// Adding an object in the midst of a foreach should be illegal
				TObject objNew = new TObject("New");
				int i = 0;
				foreach (JObject o in seq)
				{
					if (i++ == 2)
					{
						seq.InsertAt(1,objNew);
					}
					// meaningless statement to avoid the compiler warning
					parent.IsTrue(o != objNew); 
				}			
			}
		}
		public void EnumeratorIllegalInsertAt()
		{
			bool bCaught = false;
			try
			{
				new EnumeratorIllegalInsertAtMethod(this);
			}
			catch (InvalidOperationException)
			{
				bCaught = true;
			}
			IsTrue(bCaught);
		}
		#endregion
		#region EnumeratorIllegalIndexerSet
		class EnumeratorIllegalIndexerSetMethod : JObject
		{
			JOwnSeq seq;
			public EnumeratorIllegalIndexerSetMethod(Test_JSeq parent)
			{
				// Start with our standard sequence
				seq = new JOwnSeq("test", this, typeof(TObject));
				parent.SetUpUnsortedOwningSequence(seq);

				// Adding an object in the midst of a foreach should be illegal
				TObject objNew = new TObject("New");
				int i = 0;
				foreach (JObject o in seq)
				{
					if (i++ == 2)
					{
						seq[2] = objNew;
					}
					parent.IsTrue(o != objNew); // meaningless, just avoid the compiler warning
				}			
			}
		}
		public void EnumeratorIllegalIndexerSet()
		{
			bool bCaught = false;
			try
			{
				new EnumeratorIllegalIndexerSetMethod(this);
			}
			catch (InvalidOperationException)
			{
				bCaught = true;
			}
			IsTrue(bCaught);
		}
		#endregion
		#region EnumeratorIllegalRemove
		class EnumeratorIllegalRemoveMethod : JObject
		{
			JOwnSeq seq;
			public EnumeratorIllegalRemoveMethod(Test_JSeq parent)
			{
				// Start with our standard sequence
				seq = new JOwnSeq("test", this, typeof(TObject));
				parent.SetUpUnsortedOwningSequence(seq);
				TObject objNew = new TObject("New");
				seq.Append(objNew);

				// Removing an object in the midst of a foreach should be illegal
				int i = 0;
				foreach (JObject o in seq)
				{
					if (i++ == 2)
					{
						seq.Remove(objNew);
					}
					parent.IsTrue(o != objNew); // meaningless, just avoid the compiler warning
				}
			}
		}
		public void EnumeratorIllegalRemove()
			// Purpose: Once an object has been removed from the list, it is illegal to
			// continue with the enumeration. 
			// Test: Refer to Test_EnumeratorIllegalAppend().
		{
			bool bCaught = false;
			try
			{
				new EnumeratorIllegalRemoveMethod(this);
			}
			catch (InvalidOperationException)
			{
				bCaught = true;
			}
			IsTrue(bCaught);
		}
		#endregion
		#region EnumeratorIllegalRemoveAt
		class EnumeratorIllegalRemoveAtMethod : JObject
		{
			JOwnSeq seq;
			public EnumeratorIllegalRemoveAtMethod(Test_JSeq parent)
			{
				// Start with our standard sequence
				seq = new JOwnSeq("test", this, typeof(TObject));
				parent.SetUpUnsortedOwningSequence(seq);
				TObject objNew = new TObject("New");
				seq.Append(objNew);

				// Removing an object in the midst of a foreach should be illegal
				int i = 0;
				foreach (JObject o in seq)
				{
					if (i++ == 2)
					{
						seq.RemoveAt(2);
					}
					parent.IsTrue(o != objNew); // meaningless, just avoid the compiler warning
				}
			}
		}
		public void EnumeratorIllegalRemoveAt()
		{
			bool bCaught = false;
			try
			{
				new EnumeratorIllegalRemoveAtMethod(this);
			}
			catch (InvalidOperationException)
			{
				bCaught = true;
			}
			IsTrue(bCaught);
		}
		#endregion
		#region EnumeratorIllegalClear
		class EnumeratorIllegalClearMethod : JObject
		{
			JOwnSeq seq;
			public EnumeratorIllegalClearMethod(Test_JSeq parent)
			{
				// Start with our standard sequence
				seq = new JOwnSeq("test", this, typeof(TObject));
				parent.SetUpUnsortedOwningSequence(seq);

				// Clearing the list in the midst of a foreach should be illegal
				int i = 0;
				foreach (JObject o in seq)
				{
					if (i++ == 2)
					{
						seq.Clear();
					}
					parent.IsTrue(o != null); // meaningless, just avoid the compiler warning
				}			
			}
		}
		public void EnumeratorIllegalClear()
		{
			bool bCaught = false;
			try
			{
				new EnumeratorIllegalClearMethod(this);
			}
			catch (InvalidOperationException)
			{
				bCaught = true;
			}
			IsTrue(bCaught);
		}
		#endregion

		// Duplicates and sorting ------------------------------------------------------------
		#region AvoidDuplicatesAppend
		class AvoidDuplicatesAppendMethod : JObject
		{
			JOwnSeq seq;
			public AvoidDuplicatesAppendMethod(Test_JSeq parent)
			{
				// Start with our standard sequence
				seq = new JOwnSeq("test", this, typeof(TObject));
				parent.SetUpUnsortedOwningSequence(seq);

				// Add a duplicate (based on content). Should fail to be added.
				seq.AvoidDuplicates = true;
				seq.ComplainIfDuplicateAttempted = true;
				seq.Append(new TObject("David"));
			}
		}
		public void AvoidDuplicatesAppend()
		{
			bool bCaught = false;
			try
			{
				new AvoidDuplicatesAppendMethod(this);
			}
			catch (eContentDuplication)
			{
				bCaught = true;
			}
			IsTrue(bCaught);
		}
		#endregion
		#region AvoidDuplicatesInsertAt
		class AvoidDuplicatesInsertAtMethod : JObject
		{
			JOwnSeq seq;
			public AvoidDuplicatesInsertAtMethod(Test_JSeq parent)
			{
				// Start with our standard sequence
				seq = new JOwnSeq("test", this, typeof(TObject));
				parent.SetUpUnsortedOwningSequence(seq);

				// Add a duplicate (based on content). Should fail to be added.
				seq.AvoidDuplicates = true;
				seq.ComplainIfDuplicateAttempted = true;
				seq.InsertAt(2, new TObject("David"));
			}
		}
		public void AvoidDuplicatesInsertAt()
		{
			bool bCaught = false;
			try
			{
				new AvoidDuplicatesInsertAtMethod(this);
			}
			catch (eContentDuplication)
			{
				bCaught = true;
			}
			IsTrue(bCaught);
		}
		#endregion
		#region AvoidDuplicatesIndexerSet
		class AvoidDuplicatesIndexerSetMethod : JObject
		{
			JOwnSeq seq;
			public AvoidDuplicatesIndexerSetMethod(Test_JSeq parent)
			{
				// Start with our standard sequence
				seq = new JOwnSeq("test", this, typeof(TObject));
				parent.SetUpUnsortedOwningSequence(seq);

				// Add a duplicate (based on content). Should fail to be added.
				seq.AvoidDuplicates = true;
				seq.ComplainIfDuplicateAttempted = true;
				seq[2] = new TObject("David");
			}
		}
		public void AvoidDuplicatesIndexerSet()
		{
			bool bCaught = false;
			try
			{
				new AvoidDuplicatesIndexerSetMethod(this);
			}
			catch (eContentDuplication)
			{
				bCaught = true;
			}
			IsTrue(bCaught);
		}
		#endregion
		#region FindSorted
		class FindSortedMethod : JObject
		{
			JOwnSeq seq;
			public FindSortedMethod(Test_JSeq parent)
			{
				// Start with our standard sequence
				seq = new JOwnSeq("test", this, typeof(TObject));
				parent.SetUpUnsortedOwningSequence(seq);

				// Switch it to be sorted
				seq.IsSorted = true;

				// Can we find everything?
				parent.IsTrue( seq.Find("Christiane") == 0);
				parent.IsTrue( seq.Find("David") == 1);
				parent.IsTrue( seq.Find("Emily") == 2);
				parent.IsTrue( seq.Find("Robert") == 3);

				// Do we fail on things not there?
				parent.IsTrue( seq.Find("A") == -1);
				parent.IsTrue( seq.Find("Da") == -1);
				parent.IsTrue( seq.Find("Emil") == -1);
				parent.IsTrue( seq.Find("Emily Who") == -1);
				parent.IsTrue( seq.Find("Fred") == -1);
				parent.IsTrue( seq.Find("Z") == -1);
			}
		}
		public void FindSorted()
			// Purpose: Test that the a sorted list, even with randomly-ordered additions,
			// results in objects being in their correct sort order.
		{
			new FindSortedMethod(this);
		}
		#endregion
		#region FindUnsorted
		class FindUnsortedMethod : JObject
		{
			JOwnSeq seq;
			public FindUnsortedMethod(Test_JSeq parent)
			{
				// Start with our standard sequence
				seq = new JOwnSeq("test", this, typeof(TObject));
				parent.SetUpUnsortedOwningSequence(seq);

				// Can we find everything?
				parent.IsTrue( seq.Find("Emily") == 0);
				parent.IsTrue( seq.Find("Robert") == 1);
				parent.IsTrue( seq.Find("David") == 2);
				parent.IsTrue( seq.Find("Christiane") == 3);

				// Do we fail on things not there?
				parent.IsTrue( seq.Find("A") == -1);
				parent.IsTrue( seq.Find("Da") == -1);
				parent.IsTrue( seq.Find("Emil") == -1);
				parent.IsTrue( seq.Find("Emily Who") == -1);
				parent.IsTrue( seq.Find("Fred") == -1);
				parent.IsTrue( seq.Find("Z") == -1);
			}
		}
		public void FindUnsorted()
			// Purpose: Test that the unsorted list does not move things around
			// from the ordered inserted.
		{
			new FindUnsortedMethod(this);
		}
		#endregion
		#region FindInsertPosition
		class FindInsertPositionMethod : JObject
		{
			JOwnSeq seq;
			public FindInsertPositionMethod(Test_JSeq parent)
			{
				// Start with our standard sequence
				seq = new JOwnSeq("test", this, typeof(TObject));
				parent.SetUpUnsortedOwningSequence(seq);
				seq.IsSorted = true;

				// Can we find everything?
				parent.IsTrue( seq._FindInsertionPosition("Christiane") == 1);
				parent.IsTrue( seq._FindInsertionPosition("David") == 2);
				parent.IsTrue( seq._FindInsertionPosition("Emily") == 3);
				parent.IsTrue( seq._FindInsertionPosition("Robert") == 4);

				// Do we fail on things not there?
				parent.IsTrue( seq._FindInsertionPosition("A") == 0);
				parent.IsTrue( seq._FindInsertionPosition("Da") == 1);
				parent.IsTrue( seq._FindInsertionPosition("Emil") == 2);
				parent.IsTrue( seq._FindInsertionPosition("Emily Who") == 3);
				parent.IsTrue( seq._FindInsertionPosition("Fred") == 3);
				parent.IsTrue( seq._FindInsertionPosition("Z") == 4);
			}
		}
		public void FindInsertPosition()
			// Test that we find the correct spot for an insertion, so that we can
			// keep the list sorted as we add records (rather than having to re-sort the
			// whole list every time.)
		{
			new FindInsertPositionMethod(this);
		}
		#endregion
		#region SortedInsert
		class SortedInsertMethod : JObject
		{
			JOwnSeq seq;
			public SortedInsertMethod(Test_JSeq parent)
			{
				// Start with our standard sequence
				seq = new JOwnSeq("test", this, typeof(TObject));
				parent.SetUpUnsortedOwningSequence(seq);
				seq.IsSorted = true;

				// Check the Append method
				TObject objJohn = new TObject("John");
				seq.Append(objJohn);
				parent.IsTrue(seq[3] == objJohn);

				// Check the InsertAt method
				TObject objSandra = new TObject("Sandra");
				seq.InsertAt(1,objSandra);
				parent.IsTrue(seq[5] == objSandra);	
			}
		}
		public void SortedInsert()
			// Test that records inserted into the list wind up in the proper position.
		{
			new SortedInsertMethod(this);
		}
		#endregion
		#region SortedList_CantIndexer
		class SortedList_CantIndexerMethod : JObject
		{
			JOwnSeq seq;
			public SortedList_CantIndexerMethod(Test_JSeq parent)
			{
				// Start with our standard sequence
				seq = new JOwnSeq("test", this, typeof(TObject));
				parent.SetUpUnsortedOwningSequence(seq);
				seq.IsSorted = true;
				TObject obj1 = new TObject("hi");
				seq[2] = obj1;
			}
		}
		public void SortedList_CantIndexer()
			// If we have a sorted list, it is illegal to replace an object in it via the
			// indexer, as this would most likely invalidate the sorting.
		{
			bool bCaught = false;
			try
			{
				new SortedList_CantIndexerMethod(this);
			}
			catch (eSortedSequence)
			{
				bCaught = true;
			}
			IsTrue(bCaught);
		}
		#endregion
		#region SwitchSorting
		class SwitchSortingMethod : JObject
		{
			JOwnSeq seq;
			public SwitchSortingMethod(Test_JSeq parent)
			{
				// Start with our standard sequence
				seq = new JOwnSeq("test", this, typeof(TObject));
				parent.SetUpUnsortedOwningSequence(seq);
				seq.IsSorted = true;

				// Turn sorting off and insert a record
				seq.IsSorted = false;
				TObject obj = new TObject("John");
				seq.InsertAt(1, obj);
				parent.IsTrue( seq[1] == obj);

				// Now turn on sorting and verify that the object has moved to its proper position
				seq.IsSorted = true;
				parent.IsTrue( seq[3] == obj);			}
		}
		public void SwitchSorting()
		{
			new SwitchSortingMethod(this);
		}
		#endregion
		#region DuplicateFinds
		class DuplicateFindsMethod : JObject
		{
			JOwnSeq seq;
			public DuplicateFindsMethod(Test_JSeq parent)
			{
				// Start with our standard sequence
				seq = new JOwnSeq("test", this, typeof(TObject));
				parent.SetUpUnsortedOwningSequence(seq);
				seq.IsSorted = true;
				seq.AvoidDuplicates = false;

				// Add a bunch of duplicates
				seq.Append( new TObject("David") );
				seq.Append( new TObject("David") );
				seq.Append( new TObject("David") );
				seq.Append( new TObject("David") );
				seq.Append( new TObject("David") );
				seq.Append( new TObject("David") );
				seq.Append( new TObject("David") );
				parent.AreSame(1, seq.Find("David"));
				parent.AreSame(8, seq.FindAll("David").Count);

				// Repease, this time unsorted
				seq.Clear();
				seq.IsSorted = false;
				parent.SetUpUnsortedOwningSequence(seq);
				seq.AvoidDuplicates = false;
				seq.Append( new TObject("David") );
				seq.Append( new TObject("David") );
				seq.Append( new TObject("David") );
				seq.Append( new TObject("David") );
				seq.Append( new TObject("David") );
				parent.AreSame(2, seq.Find("David"));
				parent.AreSame(6, seq.FindAll("David").Count);
			}
		}
		public void DuplicateFinds()
		{
			new DuplicateFindsMethod(this);
		}
		#endregion
	}
	#endregion
}
