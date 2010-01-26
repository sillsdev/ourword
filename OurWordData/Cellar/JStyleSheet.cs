/**********************************************************************************************
 * File:    JStyleSheet.cs
 * Author:  John Wimbish
 * Created: 25 Jan 2004
 * Purpose: Handles paragraph and character styles, and the containing stylesheet
 * Legal:   Copyright (c) 2005-09, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
#region Using
using System;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using System.Xml;
using JWTools;
#endregion
#region Documentation
/* Documentation
 * 
 * A Font's Height refers to line spacing; Size is the size of the letters. All specified
 * in Points. I typically use a Size of 10, which results in a Height of 16.
 */
#endregion

namespace OurWordData
{
	#region CLASS JStyleSheet
    /*
	public class JStyleSheet : JObject
	{
		// BAttrs ----------------------------------------------------------------------------
		#region BAttr{g/s}: string DisplayName - the style's name as it appears in the UI
		public string DisplayName
		{
			get
			{
				return m_sDisplayName;
			}
			set
			{
                SetValue(ref m_sDisplayName, value);
			}
		}
		private string m_sDisplayName = "Default Style Sheet";
		#endregion
		#region Method: void DeclareAttrs()
		protected override void DeclareAttrs()
		{
			base.DeclareAttrs();
			DefineAttr("DisplayName", ref m_sDisplayName);
		}
		#endregion

		// JAttributes: ----------------------------------------------------------------------
		#region Attr{g}: JOwnSeq WritingSystems - the list of writing systems
		public JOwnSeq<JWritingSystem> WritingSystems
		{
			get 
			{ 
				return j_osWritingSystems; 
			}
		}
		private JOwnSeq<JWritingSystem> j_osWritingSystems = null;
		#endregion

		// Scaffolding -----------------------------------------------------------------------
		#region Constructor()
		public JStyleSheet()
			: base()
		{
            j_osWritingSystems = new JOwnSeq<JWritingSystem>("WritingSystems", this, true, true);
		}
		#endregion
		#region Attr(g): string SortKey - overridden to enable JOWnSeq Find method support.
		public override string SortKey
			// In order to support sorting, the subclass must implement a SortKey attribute,
			// and this SortKey must return something other than an empty string. 
		{
			get 
			{ 
				return DisplayName; 
			}
		}
		#endregion

		// WritingSystems Access -------------------------------------------------------------
		#region Method: JWritingSystem AddWritingSystem(sName) - Adds a new WS
		public JWritingSystem AddWritingSystem(string sName)
		{
			JWritingSystem ws = new JWritingSystem(sName);
			WritingSystems.Append(ws);
			return ws;
		}
		#endregion
		#region Method: JWritingSystem FindWritingSystem(sName) - returns the named WS
		public JWritingSystem FindWritingSystem(string sName)
		{
			int i = WritingSystems.Find(sName);
			if (-1 == i)
				return null;
			return (JWritingSystem)WritingSystems[i];
		}
		#endregion
		#region Method: JWritingSystem FindOrAddWritingSystem(sName) - creates the WS if necessary
		public JWritingSystem FindOrAddWritingSystem(string sName)
		{
			JWritingSystem ws = FindWritingSystem(sName);
			if (null != ws)
				return ws;

			// Add the writing system
			ws = AddWritingSystem(sName);

			return ws;
		}
		#endregion
		#region Method: JWritingSystem GetWritingSystem(index) - returns the WS at the given index
		public JWritingSystem FindWritingSystem(int index)
		{
			if (index < 0 || index >= WritingSystems.Count)
				return null;
			return (JWritingSystem)WritingSystems[index];
		}
		#endregion
		#region Method: void RemoveWritingSystem(JWritingSystem)
		public void RemoveWritingSystem(JWritingSystem ws)
		{
			if (-1 == WritingSystems.FindObj(ws))
				return;


			// Remove the Writing System
			WritingSystems.Remove(ws);
		}
		#endregion

        #region Attr{g/s}: float ZoomFactor
        public float ZoomFactor
        {
            get
            {
                return m_fZoomFactor;
            }
            set
            {
                m_fZoomFactor = value;
   //             _ResetFonts();
            }
        }
        float m_fZoomFactor = 1.0F;
        #endregion
    }
    */
	#endregion



	#region CLASS: JWritingSystem
    /*
	public class JWritingSystem : JObject
	{
		// ZAttrs ----------------------------------------------------------------------------
        #region BAttr{g/s}: string Abbrev - the name of the writing system
        public string Abbrev
        {
            get
            {
                return m_sAbbrev;
            }
            set
            {
                SetValue(ref m_sAbbrev, value);
            }
        }
        private string m_sAbbrev = "";
        #endregion

		// Scaffolding -----------------------------------------------------------------------
		#region Constructor() - no parameters, used only for reading
		public JWritingSystem()
			: base()
		{
			ConstructAttrs();
		}
		#endregion
		#region Constructor(sName)
		public JWritingSystem(string sName)
			: base()
		{
			Name = sName;
		}
		#endregion
		#region Method: override bool ContentEquals(obj) - required override to prevent duplicates
		public override bool ContentEquals(JObject obj)
		{
			if (this.GetType() != obj.GetType())
				return false;

			JWritingSystem ws = (JWritingSystem)obj;
			return ws.Name == this.Name;
		}
		#endregion


		// I/O -------------------------------------------------------------------------------
        #region OMethod: void FromXml(XElement x)
        public override void FromXml(XElement x)
        {
            base.FromXml(x);

            BuildAutoReplace();
        }
        #endregion

    }
    */
	#endregion
}
