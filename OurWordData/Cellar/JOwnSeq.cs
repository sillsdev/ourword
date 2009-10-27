/**********************************************************************************************
 * App:     Josiah
 * File:    JOwnSeq.cs
 * Author:  John Wimbish
 * Created: 27 Oct 2003
 * Purpose: Implements an owning sequence attribute.
 * Legal:   Copyright (c) 2005-09, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
#region Header: Using, etc.
using System;
using System.Collections;
using System.Collections.Generic;
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

namespace OurWordData
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

	public class JOwnSeq<T> : JSeq<T> where T:JObject
	{
		// Scaffolding -----------------------------------------------------------------------
		#region Constructor(sName,objOwner) - minimal arguments
		public JOwnSeq(string sName, JObject objOwner)
			: this(sName, objOwner, true, false)
		{
		}
		#endregion
		#region Constructor(sName,objOwner,bAvoidDuplicates,bIsSorted) - specify optional arguments
		public JOwnSeq(string sName, JObject objOwner, bool bAvoidDuplicates, bool bIsSorted)
			: base(sName, objOwner, typeof(T), bAvoidDuplicates, bIsSorted)
		{
		}
		#endregion
        #region Method: bool ContentEquals(JSeq seq)
        public bool ContentEquals(JOwnSeq<T> seq)
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

		// List membership (overrides add ownership handling) --------------------------------
		#region Indexer[] - Adds ownership support to the base indexer
		override public T this [ int index ]
		{
			get
			{
				return base[index];
			}
			set
			{
				((T)m_list[index]).Owner = null;
				base[index] = value;
				value.Owner = Owner;        // The object is now owned
			}
		}
		#endregion
		#region OMethod: void InsertAt(iPos,obj) - Adds ownership support to the base method
		override public void InsertAt(int iPos, JObject obj)
		{
			// Do the insertion via the base class method
			base.InsertAt(iPos, obj);
			// Set the object to have an owner
			obj.Owner = Owner; 
		}
		#endregion
        #region Method: void InsertAt(iPos, JObject, bSuppressDeclareDirty)
        public void InsertAt(int iPos, JObject obj, bool bSuppressDeclareDirty)
        {
            // During tests, I don't necessarily have a JObjectOnDemand owning this
            if (Owner.SaveObj == null)
            {
                InsertAt(iPos, obj);
                return;
            }

            bool bDirty = this.Owner.SaveObj.IsDirty;

            InsertAt(iPos, obj);

            if (bSuppressDeclareDirty)
                Owner.SaveObj.IsDirty = bDirty;
        }
        #endregion
        #region OMethod: void Append(obj) - Adds ownership support to the base method
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
        #region Method: void Append(JObject obj, bool bSuppressDeclareDirty)
        public void Append(JObject obj, bool bSuppressDeclareDirty)
        {
            InsertAt(Count, obj, bSuppressDeclareDirty);
        }
        #endregion
        #region Method: void Append(JOwnSeq<T> seq)
        public void Append(JOwnSeq<T> seq)
        {
            foreach (T obj in seq)
            {
                obj.Owner = null;
                base.Append(obj);
                obj.Owner = Owner;
            }
        }
        #endregion
        #region OMethod: void RemoveAt(iPos) - Adds ownership support to the base method
        override public JObject RemoveAt(int iPos)
		{
			JObject obj = base.RemoveAt(iPos);
			if (null != obj)
				obj.Owner = null;
			return obj;
		}
		#endregion
		#region OMethod: void Remove(obj) - Adds ownership support to the base method
		override public void Remove(JObject obj)
		{
			base.Remove(obj);

			// The object now has no owner
			obj.Owner = null;
		}
		#endregion
		#region OMethod: void RemoveAll() - removes all of the JObjects from the list
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
		#region OMethod: void Clear() - Adds ownership support to the base method
		override public void Clear()
		{
			// Clear objects lower down, then remove owner from all objects
			foreach( JObject obj in m_list)
			{
				obj.Clear();
				obj.Owner = null;                
			}
			base.Clear();
            DeclareDirty();
		}
		#endregion
		#region OMethod: void Replace(objOld, objNew) - replaces an object in the list
		override public void Replace(JObject objOld, JObject objNew)
		{
			base.Replace(objOld, objNew);

			// The old object now has no owner
			objOld.Owner = null;

			// But the new one does
			objNew.Owner = Owner;
		}
		#endregion
        #region OMethod: bool IsOwnerOf(JObject)
        public override bool IsOwnerOf(JObject obj)
        {
            if (-1 != FindObj(obj))
                return true;
            return false;
        }
        #endregion

        // I/O -------------------------------------------------------------------------------
		const string m_cTag = "ownseq"; // xml tag for I/O
        #region OMethod: void ToXml(XElement xObject)
        public override void ToXml(XElement xObject)
        {
            if (m_list.Count > 0)
            {
                // Create an XElement for the JOwnSeq
                XElement xOwnSeq = new XElement(m_cTag);
                xOwnSeq.AddAttr("Name", Name);

                // Add it to the owning object's XElement
                xObject.AddSubItem(xOwnSeq);

                // If an owned object is a JObjectOnDemand, then we write it (if
                //    it is dirty, as determined by Write()) out in a separate operation,
                //    as it will be saved to its own file. 
                // We also save the BasicAttrs in our current file, so that we'll
                //    have the filename, so that we can know how to load it when
                //    the time comes!
                // Otherwise, for just normal JObjects, we add ALL of the contents 
                //    as an XElement
                foreach (JObject obj in m_list)
                {
                    JObjectOnDemand ood = obj as JObjectOnDemand;
                    if (null != ood)
                    {
                        xOwnSeq.AddSubItem(obj.ToXml(false));
                        ood.WriteToFile(new NullProgress());
                    }
                    else
                    {
                        xOwnSeq.AddSubItem(obj.ToXml(true));
                    }
                }
            }
        }
        #endregion
        #region OMethod: void FromXml(XElement x)
        public override void FromXml(XElement x)
        {
            if (x.Tag != m_cTag)
                return;

            // Make sure we're starting from an empty sequence (e.g., an overwrite)
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

            // Work through the subitems
            foreach (XItem xSub in x.Items)
            {
                // Convert to an XElement; it should convert
                XElement xObj = xSub as XElement;
                if (null == xObj)
                    continue;

                // Create an object of our type, and add it to the attr
                // The Type we want is in the xObj's tag. We can't use T, because
                // we may be dealing with a subclass.
                T obj = InvokeConstructor(xObj.Tag) as T;

                // Read it in
                obj.FromXml(xObj);

                // Add it to the sequence (if we re-imlement Merge, we would
                // call a Merge method here rather than Append. We'd also not
                // call Clear earlier.
                Append(obj);
            }

            // Restore appropriate values
            SuspendDuplicatesCheck = false; // Resume checking for normal operations
            IsSorted = bPushIsSorted;       // Resume sorting if appropriate
        }
        #endregion

        #region OMethod: void ResolveReferences()
        public override void ResolveReferences()
        {
            foreach (JObject obj in this)
                obj.ResolveReferences();
        }
        #endregion
        #region OMethod: string GetPathToOwnedObject(JObject)
        public override string GetPathToOwnedObject(JObject obj)
        {
            int iPos = FindObj(obj);
            if (-1 == iPos)
                return null;

            return "-" + Name + "-" + iPos.ToString();
        }
        #endregion
        #region OMethod: JObject GetObjectFromPath(sPath)
        public override JObject GetObjectFromPath(string sPath)
        {
            // Retrieve the next element in the path; it is the index of the object
            // within this sequence
            int i = 0;

            // Move past the leading hyphen
            if (sPath.Length > i && sPath[i] == '-')
                i++;

            // Collect the index
            string sIndex = "";
            while (sPath.Length > i && sPath[i] != '-')
            {
                sIndex += sPath[i];
                i++;
            }

            // Convert it into an integer
            int iIndex = Convert.ToInt16(sIndex);

            // The remaining path is what's left
            sPath = (sPath.Length > i) ? sPath.Substring(i) : "";

            // Point to the indexed object if we're at the end of the path
            if (string.IsNullOrEmpty(sPath))
                return this[iIndex];

            // Otherwise recurse on down the path
            return this[iIndex].GetObjectFromPath(sPath);
        }
        #endregion

        // Merge -----------------------------------------------------------------------------
        #region Method: T GetCorresponding(T obj)
        T GetCorresponding(T obj)
        {
            foreach (T t in this)
            {
                if (t.SortKey == obj.SortKey)
                    return t;
            }
            return null;
        }
        #endregion

        public override void Merge(JAttr Parent, JAttr Theirs, bool bWeWin)
            // Assumption for now: SortKey is unique (no duplicates), and hasn't changed
        {
            JOwnSeq<T> ownParent = Parent as JOwnSeq<T>;
            JOwnSeq<T> ownTheirs = Theirs as JOwnSeq<T>;
            Debug.Assert(null != ownParent && null != ownTheirs);

            // Make temporary lists of Theirs, as we'll whittle away at it
            var vTheirs = new List<T>();
            foreach (T t in ownTheirs)
                vTheirs.Add(t);

            // Merge those that correspond; removing them from the temporary lists
            // as having now been dealt with
            foreach (T objMine in this)
            {
                T objParent = ownParent.GetCorresponding(objMine);
                T objTheirs = ownTheirs.GetCorresponding(objMine);

                if (null != objParent && null != objTheirs)
                {
                    objMine.Merge(objParent, objTheirs, bWeWin);
                    vTheirs.Remove(objTheirs);
                }
            }

            // Scan through the parent list, to find notes in the parent that also
            // have notes in Theirs. Where these exist, we need to delete them from
            // Theirs, because they represent notes that we deleted in ours.
            foreach (T objParent in ownParent)
            {
                T objTheirs = ownTheirs.GetCorresponding(objParent);
                if (null != objTheirs && vTheirs.Contains(objTheirs))
                    vTheirs.Remove(objTheirs);
            }

            // Anything remaining in Theirs represents an insertion we need to add.
            // Easiest thing to do here is just move it over
            foreach (T objTheirs in vTheirs)
            {
                ownTheirs.Remove(objTheirs);
                if (-1 == Find(objTheirs.SortKey))
                    Append(objTheirs);
            }
        }
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

	#endregion
}
