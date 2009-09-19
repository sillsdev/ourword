/**********************************************************************************************
 * App:     Josiah
 * File:    JOwn.cs
 * Author:  John Wimbish
 * Created: 27 Oct 2003
 * Purpose: Implements JOwn, an atomic owning attribute.
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
	public class JOwn<T> : JAttr where T:JObject
	{
		// Scaffolding -----------------------------------------------------------------------
		#region Constructor(sName, objOwner) - sets up the attribute
		public JOwn(string sName, JObject objOwner)
			: base(sName, objOwner, typeof(T))
		{
		}
		#endregion

		// Getting / Setting -----------------------------------------------------------------
		#region Attr{g/s}: T Value - main method for getting / setting the attr's value
		public T Value
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
		protected T m_object = null;
		#endregion
		#region Method: void Clear() - sets the value to null
		public override void Clear()
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
		const string c_sTag = "own";    // xml tag for I/O
        #region OMethod: void ToXml(XElement xObject)
        public override void ToXml(XElement xObject)
        {
            if (null != Value)
            {
                // Create an XElement for the JOwn
                XElement xOwn = new XElement(c_sTag);
                xOwn.AddAttr("Name", Name);

                // Add it to the owning object's XElement
                xObject.AddSubItem(xOwn);

                // If an owned object is a JObjectOnDemand, then we write it (if
                //    it is dirty, as determined by Write()) out in a separate operation,
                //    as it will be saved to its own file. 
                // We also save the BasicAttrs in our current file, so that we'll
                //    have the filename, so that we can know how to load it when
                //    the time comes!
                // Otherwise, for just normal JObjects, we add ALL of the contents 
                //    as an XElement
                JObjectOnDemand ood = Value as JObjectOnDemand;
                if (null != ood)
                {
                    xOwn.AddSubItem(Value.ToXml(false));
                    ood.WriteToFile(new NullProgress());
                }
                else
                    xOwn.AddSubItem(Value.ToXml(true));
            }
        }
        #endregion
        #region OMethod: void FromXml(XElement x)
        public override void FromXml(XElement x)
        {
            if (x.Tag != c_sTag)
                return;

            // Get the XElement that is the value for this; there should be
            // exactly one subitem
            if (x.Items.Count != 1)
                return;
            XElement xObj = x.Items[0] as XElement;
            if (null == xObj)
                return;

            // Create an object of our type
            T obj = InvokeConstructor(xObj.Tag) as T;

            // Read it in
            obj.FromXml(xObj);

            // Set the ownership
            Value = obj;
        }
        #endregion

        #region OMethod: void ResolveReferences()
        public override void ResolveReferences()
        {
            if (null != Value)
                Value.ResolveReferences();
        }
        #endregion
        #region OMethod: string GetPathToOwnedObject(JObject)
        public override string GetPathToOwnedObject(JObject obj)
        {
            if (Value != obj)
                return null;

            return "-" + Name;
        }
        #endregion
        #region OMethod: void WriteOwnedObjectsOnDemand()
        public override void WriteOwnedObjectsOnDemand()
        {
            JObjectOnDemand ood = Value as JObjectOnDemand;
            if (null != ood)
                ood.WriteToFile(new NullProgress());
        }
        #endregion
        #region OMethod: JObject GetObjectFromPath(sPath)
        public override JObject GetObjectFromPath(string sPath)
        {
            // If there is no more path left, then return our current object
            if (string.IsNullOrEmpty(sPath))
                return Value;

            // Otherwise, we continue recursing down the ownership hierarchy
            return Value.GetObjectFromPath(sPath);
        }
        #endregion

        #region OMethod: bool IsOwnerOf(JObject)
        public override bool IsOwnerOf(JObject obj)
        {
            if (obj == Value)
                return true;
            return false;
        }
        #endregion

        // Merge -----------------------------------------------------------------------------
        #region OMethod: void Merge(JAttr Parent, JAttr Theirs, bool bWeWin)
        public override void Merge(JAttr Parent, JAttr Theirs, bool bWeWin)
            // An owning attr, as used in OW, generally either owns an object or is
            // clear. I think merging is mostly a matter of checking whether it should
            // be clear or not, and then having the owned object do its thing for
            // the merge if not clear.
        {
            JOwn<T> ownParent = Parent as JOwn<T>;
            JOwn<T> ownTheirs = Theirs as JOwn<T>;
            Debug.Assert(null != ownParent && null != ownTheirs);

            // Check for Null set/cleared
            bool bOurNullChanged = (
                (ownParent.Value == null && Value != null) ||
                (ownParent.Value != null && Value == null));
            bool bTheirNullChanged = (
                (ownTheirs.Value == null && Value != null) ||
                (ownTheirs.Value != null && Value == null));
            // Theirs changed, so we need to become as them
            if (bTheirNullChanged && !bOurNullChanged)
            {
                if (ownTheirs.Value != null)
                {
                    T obj = ownTheirs.Value;
                    ownTheirs.Value = null;
                    Value = obj;
                }
                else
                    Value = null;
                return;
            }

            // A merge only if we have values in all three
            if (null != ownParent.Value && null != ownTheirs.Value && null != Value)
                Value.Merge(ownParent.Value, ownTheirs.Value, true);
        }
        #endregion
    }

}
