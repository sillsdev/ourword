/**********************************************************************************************
 * App:     Josiah
 * File:    JRef.cs
 * Author:  John Wimbish
 * Created: 15 Mar 2004
 * Purpose: Implements JRef, an atomic reference attribute.
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

namespace OurWordData
{
	public class JRef<T> : JAttr where T:JObject
	{
		// Private attributes ----------------------------------------------------------------
        string m_sTemporaryReferencePath;        // Used during read process

		// Scaffolding -----------------------------------------------------------------------
		#region Constructor(sName, objOwner) - sets up the attribute
		public JRef(string sName, JObject objOwner)
			: base(sName, objOwner, typeof(T))
		{
		}
		#endregion

		// Getting / Setting -----------------------------------------------------------------
		#region Attr{g/s} T Value - main method for getting / setting the attr's value
		public T Value
		{
			get
			{
                return m_objValue;
			}
			set
			{
				if (null == value)
					Clear();
				else
				{
					// Insert the target object and give it an owner
                    m_objValue = value;

					DeclareDirty();
				}
			}
		}
        protected T m_objValue = null;
		#endregion
		#region OMethod: void Clear() - removes the object
		public override void Clear()
		{
            m_objValue = null;
			DeclareDirty();
		}
		#endregion

		// I/O -------------------------------------------------------------------------------
		const string c_sTag = "ref";    // xml tag for I/O
        #region OMethod: void ToXml(XElement xObject)
        public override void ToXml(XElement xObject)
        {
            // If not pointing to anything, then there is nothing to write
            if (null == Value)
                return;

            // Create an XElement for the JRef
            XElement xRef = new XElement(c_sTag);
            xRef.AddAttr("Name", Name);

            // Add it to the owning object's XElement
            xObject.AddSubItem(xRef);

            // Add the contents
            string s = PathToReferencedObject;
            xRef.AddSubItem(new XString(s));
        }
        #endregion
        #region OMethod: void FromXml(XElement x)
        public override void FromXml(XElement x)
        {
            if (x.Tag != c_sTag)
                return;

            // The path is stored as an XString data object
            if (x.Items.Count != 1)
                return;
            XString xs = x.Items[0] as XString;
            if (null == xs)
                return;

            // Store it for now, we'll resolve it later (after all objects have been read in)
            // via the ResolveReferences method
            m_sTemporaryReferencePath = xs.Text;
        }
        #endregion

        #region OMethod: void ResolveReferences()
        public override void ResolveReferences()
		{
            if (string.IsNullOrEmpty(m_sTemporaryReferencePath))
                return;

            // Extract from the RefPath how high up the ownership hiererchy 
            string sUp = m_sTemporaryReferencePath.Substring(0, 
                m_sTemporaryReferencePath.IndexOf('-'));
            int cUp = Convert.ToInt16(sUp);
            string sDown = m_sTemporaryReferencePath.Substring(
                m_sTemporaryReferencePath.IndexOf('-') + 1);

            // Move up the hierarchy as directed
            JObject obj = this.Owner;
            while (cUp > 0)
            {
                obj = obj.Owner;
                --cUp;
            }

            // Find the target object
            Value = obj.GetObjectFromPath(sDown) as T;

            // Don't need to take up memory with this any longer
            m_sTemporaryReferencePath = null;
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

				// Point to the nearest object that the two have in common
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

        public override void Merge(JAttr Parent, JAttr Theirs, bool bWeWin)
        {
            JRef<T> refParent = Parent as JRef<T>;
            JRef<T> refTheirs = Theirs as JRef<T>;
            Debug.Assert(null != refParent && null != refTheirs);

            // TODO: How do we measure "IsSameAs" here? Guids?

        }
	}
}
