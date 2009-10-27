/**********************************************************************************************
 * App:     Josiah
 * File:    JSeq.cs
 * Author:  John Wimbish
 * Created: 27 Oct 2003
 * Purpose: Implements the shared sequence behavior, used by subclasses such as JOwnSeq
 *            and JRefSeq.
 * Legal:   Copyright (c) 2005-09, John S. Wimbish. All Rights Reserved.  
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

namespace OurWordData
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
	public class JSeq<T> : JAttr, IEnumerator, IComparer where T:JObject
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
        #region Method: bool ContentEquals(JSeq seq)
        public bool ContentEquals(JSeq<T> seq)
        {
            if (Count != seq.Count)
                return false;

            for (int i = 0; i < Count; i++)
            {
                if (!seq[i].ContentEquals(this[i]))
                    return false;
            }

            return true;
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
		virtual public T this [ int index ]
		{
			get
			{
				if (index < 0 || index >= m_list.Count)
					return null;
				return m_list[index] as T;
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
		#region OMethod: void Clear() - removes all JObjects from the list
		public override void Clear()
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
		public int _FindInsertionPosition(string sSortKey)
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
        #region Method: int FindObj(JObject) - returns the index of the obj, or -1 if not found
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
}
