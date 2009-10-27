/**********************************************************************************************
 * App:     Josiah
 * File:    JObject.cs
 * Author:  John Wimbish
 * Created: 27 Oct 2003
 * Purpose: Implements JObject, which is a base class for all data objects.
 * Legal:   Copyright (c) 2005-09, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
#region Header: Using, etc.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using System.IO;
using System.Reflection;

using JWTools;
#endregion

namespace OurWordData
{
    #region DOC - Defining a new type of Simple BAttr
    /* DOC: To Define a new type of BAttr, do the following in JObject
     * 
     * - Add the test case for this type to the test: T_JObject.SimpleBAttrIO()
     * 
     * - Create a SetValue(ref YourNewType attr, YourNewType, newValue); its purpose
     *     is to call DeclareDirty.
     * 
     * - Add DefineAttr methods to everything in the I/O:BAttrs section:
     *    _LoopAttrsMethod
     *    _SaveBasicAttrsMethod
     *    to the JObject class itself
     * 
     */
    #endregion

    // Exceptions ----------------------------------------------------------------------------
	#region class eJosiahException - generic Josiah exception (root for all others)
	public class eJosiahException : ApplicationException
	{
		static private string m_sError = "Josiah"; 
		public eJosiahException()
			: base(m_sError)
		{
		}
		public eJosiahException(string message)
			: base(m_sError + " - " + message)
		{
		}
		public eJosiahException(string message, Exception inner)
			: base(m_sError + " - " + message, inner)
		{
		}
	}
	#endregion
    #region Exception: eDuplicateAttrName - Attempt to create two attrs with the same name
	public class eDuplicateAttrName : eJosiahException
	{
		public eDuplicateAttrName(string sMethodName)
			: base("Attempt to create multiple attributes with the same name - " + sMethodName)
		{}
	}
	#endregion

	public class JObject : Object
		#region Documentation
		// Requirements for subclasses:
		// 1. Implement a public "Read" constructor that takes the arguments:
		//       string sFirstLine - a line verifying the type of object
		//       StreadReader r    - for getting subsequent lines, if necessary
		// This is necessary for the class to read in from xml.
		#endregion
	{
		// Public attributes -----------------------------------------------------------------
		#region Attr(g): JObject Owner - returns the owning JObject
		public JObject Owner
		{
			get 
			{
				return m_objOwner;
			}
			set
				// Only code crafted with Josiah should set the owner; client code should
				// not ever do this. The assertion says that if the object already has
				// an owner, it is illegal to try to give it a new owner. E.g., we don't want
				// to get away with inserting the obj into two different owning sequences.
			{
				if (m_objOwner != null && value != null)
					throw new eAlreadyOwned("Owner{set}");
				m_objOwner = value;
			}
		}
		private JObject m_objOwner = null;
		#endregion
		#region Attr{g}: JObject RootOwner - returns the topmost owner in the hierarchy
		public JObject RootOwner
		{
			get
			{
				ArrayList list = AllOwners;
				return (JObject)list[0];
			}
		}
		#endregion
		#region Attr{g}: ArrayList AllOwners - returns the ownership hierarchy
		public ArrayList AllOwners
		{
			get
			{
				ArrayList rg = new ArrayList();
				_RecurseAllOwners(rg);
				return rg;
			}
		}
		private ArrayList _RecurseAllOwners(ArrayList rg)
			// Helper method for recursion, only AllOwners should call this. 
			// If this object has an owner, we insert that owner into the front of the
			// array; then we call _RecurseAllOwners on the owner itself, and thus
			// recurse up the ownership chain. The goal is to return the ArrayList with
			// the root owner being the first element in the list.
		{
			Debug.Assert(null != rg);
			if (null != Owner)
			{
				rg.Insert(0, Owner);
				Owner._RecurseAllOwners(rg);
			}
			return rg;
		}
		#endregion
		#region Attr{g}: bool IsRoot - returns T if this object is the root (has no owner)
		public bool IsRoot
		{
			get
			{
				return (Owner == null);
			}
		}
		#endregion
		#region Attr(g): string SortKey - must be overridden if sorting is desired.
		public virtual string SortKey
			// In order to support sorting, the subclass must implement a SortKey attribute,
			// and this SortKey must return something other than an empty string. 
		{
			get 
			{ 
				return ""; 
			}
		}
		#endregion

		// Dirty - Need to Save? -------------------------------------------------------------
		#region Attr{g}: JObjectOnDemand SaveObj - return Save Obj that owns this object
		public virtual JObjectOnDemand SaveObj
		{
			get
			{
				if (null == Owner)
					return null;
				return Owner.SaveObj;
			}
		}
		#endregion
		#region Method: void DeclareDirty() - mark the Save Obj as needing to be saved
		public virtual void DeclareDirty()
		{
			JObjectOnDemand obj = SaveObj;
			if (null != obj)
				obj.DeclareDirty();
		}
		#endregion

        #region Method: void SetValue(ref sAttr, sNewValue)
        protected void SetValue(ref string sAttr, string sNewValue)
        {
            if (sAttr != sNewValue)
            {
                sAttr = sNewValue;
                DeclareDirty();
            }
        }
        #endregion
        #region Method: void SetValue(ref bAttr, bNewValue)
        protected void SetValue(ref bool bAttr, bool bNewValue)
        {
            if (bAttr != bNewValue)
            {
                bAttr = bNewValue;
                DeclareDirty();
            }
        }
        #endregion
        #region Method: void SetValue(ref nAttr, nNewValue)
        protected void SetValue(ref int nAttr, int nNewValue)
        {
            if (nAttr != nNewValue)
            {
                nAttr = nNewValue;
                DeclareDirty();
            }
        }
        #endregion
        #region Method: void SetValue(ref dAttr, dNewValue)
        protected void SetValue(ref double dAttr, double dNewValue)
        {
            if (dAttr != dNewValue)
            {
                dAttr = dNewValue;
                DeclareDirty();
            }
        }
        #endregion
        #region Method: void SetValue(ref dtAttr, dtNewValue)
        protected void SetValue(ref DateTime dtAttr, DateTime dtNewValue)
        {
            if (dtAttr != dtNewValue)
            {
                dtAttr = dtNewValue;
                DeclareDirty();
            }
        }
        #endregion
        #region Method: void SetValue(ref chAttr, chNewValue)
        protected void SetValue(ref Guid guidAttr, Guid guidNewValue)
        {
            if (guidAttr == guidNewValue)
            {
                guidAttr = guidNewValue;
                DeclareDirty();
            }
        }
        #endregion
        #region Method: void SetValue(ref chAttr, chNewValue)
        protected void SetValue(ref char chAttr, char chNewValue)
        {
            if (chAttr != chNewValue)
            {
                chAttr = chNewValue;
                DeclareDirty();
            }
        }
        #endregion
        #region Method: void SetValue(ref BStringArray vAttr, BStringArray vNewValue)
        protected void SetValue(ref BStringArray vAttr, BStringArray vNewValue)
        {
            if (vAttr != vNewValue)
            {
                vAttr = vNewValue;
                DeclareDirty();
            }
        }
        #endregion

		// Equality based on content (not reference) -----------------------------------------
		#region Method: bool ContentEquals(a,b) - must be overridden for non SortKey comparisons
		public virtual bool ContentEquals(JObject obj)
		{
			if (this.GetType() != obj.GetType())
				return false;
			if (this.SortKey.Length == 0 || obj.SortKey.Length == 0)
				throw new eNoContentCompare("JObject.ContentEquals()");
			return this.SortKey == obj.SortKey;
		}
		#endregion
		#region Method: bool ContentEquals(a,b) - static version (not necessary to override)
		public static bool ContentEquals(JObject a, JObject b) 
		{
			return a.ContentEquals(b);
		}
		#endregion

		// Collection of all (non-basic) attributes ------------------------------------------
		#region Attr{g}: ArrayList AllAttrs - list of all this obj's non-basic attributes
		// The purpose of this list is to provide a means of iterating through all of the
		// objects attributes; e.g., for doing a Write operation.
		public ArrayList AllAttrs
		{
			get
			{
				return m_Attributes;
			}
		}
		private ArrayList m_Attributes = new ArrayList();
		#endregion
		#region Method: void AddAttribute(attribute) - append an, e.g., JOwnSeq to the list
		public void AddAttribute(JAttr attribute)
		{
			Debug.Assert(null != attribute);
			Debug.Assert(null != m_Attributes);

			// Make sure we don't have multiple attrs of the same name
			if( !m_bSurpressDuplicateAttrTest && null != FindAttrByName( attribute.Name ) )
				throw new eDuplicateAttrName("AddAttribute");

			m_Attributes.Add(attribute);
		}
		#endregion
		#region TestAccess

		// KLUDGE: Did this to get the OwnSeq to pass without throwing the eDuplicateAttrName.
		// This can probably be reworked to get rid of this.
		public bool m_bSurpressDuplicateAttrTest = false;   // Needed for some of the tests

		public bool _test_ContainsAttribute(object obj)
		{
			return m_Attributes.Contains(obj);
		}
		#endregion
		#region Method: JAttr FindAttrByName(sName) - returns the JAttr which has the requested name
		public JAttr FindAttrByName(string sName)
		{
			foreach( JAttr attr in AllAttrs )
			{
				if (attr.Name == sName)
					return attr;
			}
			return null;
		}
		#endregion
        #region Method: JAttr GetMyOwningAttr()
        public JAttr GetMyOwningAttr()
        {
            // IF we don't have an owner, then we can't do this.
            if (null == Owner)
                return null;

            foreach (JAttr attr in Owner.AllAttrs)
            {
                if (attr.IsOwnerOf(this))
                    return attr;
            }

            return null;
        }
        #endregion

        // Methods ---------------------------------------------------------------------------
		#region Constructor()
		public JObject()
		{
			// The object, when first created, has no owner.
			Owner = null;
		}
		#endregion
		#region Method: Clear() - Removes all objects down the hierarchy
		public void Clear()
			// Removes all objects down the hierarchy, so that the garbage collector has a less
			// ambigious time of figuring out what it can dispose of. (To be honest, I've no idea
			// if this is really needed; but it only costs some processor time.)
			//   The result is that the JObject's attributes still exist (e.g., the JOwnSeq is not
			// destroyed), but it no longer has any contents.
		{
            foreach (JAttr attr in AllAttrs)
                attr.Clear();

			DeclareDirty();
		}
		#endregion
        #region Attr{g}: int BAttrCount
        public int BAttrCount
        {
            get
            {
                if (-1 == m_cBAttrCount)
                {
                    m_cBAttrCount = 0;
                    m_ioOperation = Ops.kCount;
                    DeclareAttrs();
                }

                return m_cBAttrCount;
            }
        }
        int m_cBAttrCount = -1;
        #endregion

        // I/O -------------------------------------------------------------------------------
        protected enum Ops { kSave, kRead, kCount };
        protected Ops m_ioOperation = Ops.kSave;
        protected XElement m_ioX;
        #region Methods: void DefineAttr(sName, ref X)
        #region Method: void DefineAttr(sName, ref n)
        protected void DefineAttr(string sName, ref int nValue)
		{
            switch (m_ioOperation)
            {
                case Ops.kRead:
                    nValue = m_ioX.GetAttrValue(sName, nValue);
                    return;
                case Ops.kSave:
                    m_ioX.AddAttr(sName, nValue);
                    return;
                case Ops.kCount:
                    m_cBAttrCount++;
                    return;
            }
            Debug.Assert(false, "missing DefineAttr op");
        }
        #endregion
        #region Method: void DefineAttr(sName, ref d)
        protected void DefineAttr(string sName, ref double dblValue)
		{
            switch (m_ioOperation)
            {
                case Ops.kRead:
                    dblValue = m_ioX.GetAttrValue(sName, dblValue);
                    return;
                case Ops.kSave:
                    m_ioX.AddAttr(sName, dblValue);
                    return;
                case Ops.kCount:
                    m_cBAttrCount++;
                    return;
            }
            Debug.Assert(false, "missing DefineAttr op");
        }
        #endregion
        #region Method: void DefineAttr(sName, ref b)
        protected void DefineAttr(string sName, ref bool bValue)
		{
            switch (m_ioOperation)
            {
                case Ops.kRead:
                    bValue = m_ioX.GetAttrValue(sName, bValue);
                    return;
                case Ops.kSave:
                    m_ioX.AddAttr(sName, bValue);
                    return;
                case Ops.kCount:
                    m_cBAttrCount++;
                    return;
            }
            Debug.Assert(false, "missing DefineAttr op");
        }
        #endregion
        #region Method: void DefineAttr(sName, ref s)
        protected void DefineAttr(string sName, ref string sValue)
		{
            switch (m_ioOperation)
            {
                case Ops.kRead:
                    sValue = m_ioX.GetAttrValue(sName, sValue);
                    return;
                case Ops.kSave:
                    m_ioX.AddAttr(sName, sValue);
                    return;
                case Ops.kCount:
                    m_cBAttrCount++;
                    return;
            }
            Debug.Assert(false, "missing DefineAttr op");
        }
        #endregion
        #region Method: void DefineAttr(sName, ref ch)
        protected void DefineAttr(string sName, ref char chValue)
		{
            switch (m_ioOperation)
            {
                case Ops.kRead:
                    chValue = m_ioX.GetAttrValue(sName, chValue);
                    return;
                case Ops.kSave:
                    m_ioX.AddAttr(sName, chValue);
                    return;
                case Ops.kCount:
                    m_cBAttrCount++;
                    return;
            }
            Debug.Assert(false, "missing DefineAttr op");
        }
        #endregion
        #region Method: void DefineAttr(sName, ref dt)
        protected void DefineAttr(string sName, ref DateTime dtValue)
        {
            switch (m_ioOperation)
            {
                case Ops.kRead:
                    dtValue = m_ioX.GetAttrValue(sName, dtValue);
                    return;
                case Ops.kSave:
                    m_ioX.AddAttr(sName, dtValue);
                    return;
                case Ops.kCount:
                    m_cBAttrCount++;
                    return;
            }
            Debug.Assert(false, "missing DefineAttr op");
        }
        #endregion
        #region Method: void DefineAttr(sName, ref bsa)
        protected void DefineAttr(string sName, ref BStringArray bsa)
		{
            Debug.Assert(null != bsa);

            switch (m_ioOperation)
            {
                case Ops.kRead:
                    bsa.Read(m_ioX.GetAttrValue(sName, "0"));
                    return;
                case Ops.kSave:
                    m_ioX.AddAttr(sName, bsa.SaveLine);
                    return;
                case Ops.kCount:
                    m_cBAttrCount++;
                    return;
            }
            Debug.Assert(false, "missing DefineAttr op");
        }
        #endregion
        #region Method: void DefineAttr(sName, ref bna)
        protected void DefineAttr(string sName, ref BIntArray bna)
		{
            switch (m_ioOperation)
            {
                case Ops.kRead:
                    bna.Read(m_ioX.GetAttrValue(sName, ""));
                    return;
                case Ops.kSave:
                    m_ioX.AddAttr(sName, bna.SaveLine);
                    return;
                case Ops.kCount:
                    m_cBAttrCount++;
                    return;
            }
            Debug.Assert(false, "missing DefineAttr op");
        }
        #endregion
        #region Method: void DefineAttr(sName, ref Guid)
        protected void DefineAttr(string sName, ref Guid guidValue)
        {
            switch (m_ioOperation)
            {
                case Ops.kRead:
                    guidValue = m_ioX.GetAttrValue(sName, guidValue);
                    return;
                case Ops.kSave:
                    m_ioX.AddAttr(sName, guidValue);
                    return;
                case Ops.kCount:
                    m_cBAttrCount++;
                    return;
            }
            Debug.Assert(false, "missing DefineAttr op");
        }
        #endregion
        #endregion
        #region Method: virtual void DeclareAttrs() - sub's must override to register their attrs
        protected virtual void DeclareAttrs()
		{
		}
		#endregion
        #region VMethod: XElement ToXml(bool bIncludeNonBasicAttrs)
        public virtual XElement ToXml(bool bIncludeNonBasicAttrs)
        {
            // Create the XElement for this object
            XElement x = new XElement(GetType().Name);

            // Add the basic attributes
            m_ioX = x;
            m_ioOperation = Ops.kSave;
            DeclareAttrs();

            // Add the non-basic attributes
            if (bIncludeNonBasicAttrs)
            {
                foreach (JAttr attr in m_Attributes)
                    attr.ToXml(x);
            }

            // Done
            return x;
        }
        #endregion
        #region VMethod: void FromXml(XElement x)
        public virtual void FromXml(XElement x)
            // If the subclass has an XString in the items, then it will need to
            // override this in order to handle it.
        {
            // Extract the basic attributes
            m_ioX = x;
            m_ioOperation = Ops.kRead;
            DeclareAttrs();

            // Process the non-basic attributes
            foreach (XItem item in x.Items)
            {
                XElement element = item as XElement;
                Debug.Assert(null != element, "Must override if data has an XString");

                // Get the name of the attribute
                XElement.XAttr aName = element.FindAttr("Name");
                if (null == aName)
                    continue;
                string sName = aName.Value;

                // Find the attribute corresponding to this name
                JAttr attr = FindAttrByName(sName);
                if (attr == null)
                    continue;

                // Read in its value
                attr.FromXml(element);
            }
        }
        #endregion

        // Merge -----------------------------------------------------------------------------
        #region Method: void MergeBasicAttrs(objParent, objTheirs, bWeWin)
        public void MergeBasicAttrs(JObject Parent, JObject Theirs, bool bWeWin)
        {
            // Collect the basic attr values within the xml element
            XElement xMine = ToXml(false);
            XElement xParent = Parent.ToXml(false);
            XElement xTheirs = Theirs.ToXml(false);

            // Loop through the attrs. We can assume they are the same number, because
            // this list is produced by the object, not by the incoming data stream.
            Debug.Assert(BAttrCount == Parent.BAttrCount);
            Debug.Assert(BAttrCount == Theirs.BAttrCount);
            foreach (XElement.XAttr attrMine in xMine.Attrs)
            {
                XElement.XAttr attrParent = xParent.FindAttr(attrMine.Tag);
                XElement.XAttr attrTheirs = xTheirs.FindAttr(attrMine.Tag);

                bool bWeDiffer = (attrMine.Value != attrParent.Value);
                bool bTheyDiffer = (attrTheirs.Value != attrParent.Value);

                // If no one differs, we're done
                if (!bWeDiffer && !bTheyDiffer)
                    continue;

                // If we differ from parent, but they are same, keep ours.
                if (bWeDiffer && !bTheyDiffer)
                    continue;

                // If they differ, but we are the same, keep theirs
                if (bTheyDiffer && !bWeDiffer)
                {
                    attrMine.Value = attrTheirs.Value;
                    continue;
                }

                // We have a conflict: let WeWin decide
                if (!bWeWin)
                    attrMine.Value = attrTheirs.Value;
            }

            // Place the answer back into the object
            m_ioX = xMine;
            m_ioOperation = Ops.kRead;
            DeclareAttrs();
        }
        #endregion

        public virtual void Merge(JObject Parent, JObject Theirs, bool bWeWin)
        {
            // Basic Attrs
            MergeBasicAttrs(Parent, Theirs, bWeWin);

            // Non-Basic Attrs
            foreach (JAttr attrMine in AllAttrs)
            {
                JAttr attrParent = Parent.FindAttrByName(attrMine.Name);
                JAttr attrTheirs = Theirs.FindAttrByName(attrMine.Name);

                attrMine.Merge(attrParent, attrTheirs, bWeWin);
            }
        }
       
		// I/O: Support Methods for resolving reference attributes ---------------------------
		#region Method: string GetPathFromOwningObject(objAtTop) - returns, e.g., "LexEntries-234-Senses-5"
		public string GetPathFromOwningObject(JObject objAtTop)
		{
			return _GetPathFromOwningObject(objAtTop, "");
		}
		#endregion
		#region Method: string GetPathFromRoot() - returns the save-path from the root object
		public string GetPathFromRoot()
		{
			return _GetPathFromOwningObject(RootOwner, "");
		}
		#endregion
		#region Method: void _GetPathFromOwningObject(...) - private helper method
		private string _GetPathFromOwningObject(JObject objAtTop, string sPathSoFar)
			// The path looks like 
			//     "-JOwnName" for an atomic owning attr, and
			//     "-JOwnSeqName-N" for a sequence (where N is the number as stored in the sequence)
		{
			if (IsRoot || this == objAtTop)
				return sPathSoFar;

            foreach (JAttr attr in Owner.AllAttrs)
            {
                string sAdditon = attr.GetPathToOwnedObject(this);
                if (!string.IsNullOrEmpty(sAdditon))
                {
                    sPathSoFar = sAdditon + sPathSoFar;
                    return Owner._GetPathFromOwningObject(objAtTop, sPathSoFar);
                }
            }

			// Should have been found by now
			Debug.Assert(false);

			return sPathSoFar;
		}
		#endregion
		#region Method: JObject GetObjectFromPath(sPath) - returns JObject described by path
		public JObject GetObjectFromPath(string sPath)
			// Given a path of the form returned from _GetPathFromOwningObject, returns the
			// object at the end of that path. Thus these two methods much work in tandem.
		{
			// If there is nothing in the path, then return "this"
			if (sPath.Length == 0)
				return this;

			// The first element of the path is the attr, in the form of "-AttrName-"
			string sAttrName = "";
			int i=0;
			if(sPath.Length > i && sPath[i]=='-')
				i++;
			while(sPath.Length > i && sPath[i]!='-')
			{
				sAttrName += sPath[i];
				i++;
			}

			// Find the attribute
			JAttr attr = FindAttrByName(sAttrName);
			if (null == attr)
				return null;

            // Remove what we've just extracted from the path
            sPath = (sPath.Length > i) ? sPath.Substring(i) : "";

            // Call the attribute to recurse and find the desired object
            return attr.GetObjectFromPath(sPath);
		}
		#endregion
		#region Method: void ResolveReferences() - at end of Read operation, set the Reference pointers
		public void ResolveReferences()
		{
            foreach (JAttr attr in AllAttrs)
                attr.ResolveReferences();
		}
		#endregion
	}



}
