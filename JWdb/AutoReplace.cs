/**********************************************************************************************
 * Project: Our Word!
 * File:    AutoReplace.cs
 * Author:  John Wimbish
 * Created: 07 Oct 2004
 * Purpose: Replace while typing; a poor man's way to set up a keyboard for special char's.
 * Legal:   Copyright (c) 2005-08, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
#region Using
using System;
using System.Diagnostics;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Windows.Forms;
using System.IO;
using JWTools;
#endregion

namespace JWdb
{
	public class JSubstitution : JObject
	{
		// BAttrs ----------------------------------------------------------------------------
		#region BAttr{g}: string Source - the pattern to look for
		public string Source
		{
			get
			{
				return m_sSource;
			}
			set
			{
                SetValue(ref m_sSource, value);
			}
		}
		string m_sSource;
		#endregion
		#region BAttr{g}: string Replacement - the string to replace it with
		public string Replacement
		{
			get
			{
				return m_sReplacement;
			}
			set
			{
                SetValue(ref m_sReplacement, value);
			}
		}
		string m_sReplacement;
		#endregion
		#region Method void DeclareAttrs()
		protected override void DeclareAttrs()
		{
			base.DeclareAttrs();
			DefineAttr("src", ref m_sSource);
			DefineAttr("rpl", ref m_sReplacement);
		}
		#endregion

		// Scaffolding -----------------------------------------------------------------------
		#region Constructor() - for I/O
		public JSubstitution()
			: base()
		{
		}
		#endregion
		#region Constructor(sSource, sReplacement)
		public JSubstitution(string _sSource, string _sReplacement)
			: base()
		{
			Source = _sSource;
			Replacement = _sReplacement;
		}
		#endregion
		#region Method: override bool ContentEquals(obj) - required override to prevent duplicates
		public override bool ContentEquals(JObject obj)
		{
			if (this.GetType() != obj.GetType())
				return false;

			JSubstitution objSub = (JSubstitution)obj;
			if (objSub.Source != this.Source)
				return false;
			if (objSub.Replacement != this.Replacement)
				return false;
			return true;
		}
		#endregion
		#region Attr{g}: string SortKey - supports sorting when owned in a JOwnSeq
		public override string SortKey
		{
			get
			{
				return Source;
			}
		}
		#endregion
	}

	public class JSubstitutionList : JOwnSeq
	{

		// Scaffolding -----------------------------------------------------------------------
		#region Constructor(sName, JObject objOwner)
		public JSubstitutionList(string sName, JObject objOwner)
			: base(sName, objOwner, typeof(JSubstitution), true, true)
		{
		}
		#endregion
	}


	public class TreeNode
	{
		char[]     m_rgLetter;
		TreeNode[] m_rgLower;

		TreeNode   m_parent;

		#region Attr{g}: int Depth
		protected int Depth
		{
			get
			{
				if (null == m_parent)
					return 0;
				return m_parent.Depth + 1;
			}
		}
		#endregion

		protected bool m_bCanEndHere = false;
		protected string m_sResult   = "";

		#region Method: void Add(string sSource, string sResult)
		public void Add(string sSource, string sResult)
		{
			if (sSource.Length == 0)
			{
				m_sResult = sResult;
				m_bCanEndHere = true;
				return;
			}

			char chFirst = sSource[ sSource.Length - 1 ];

			// Search to see if the character exists in the list
			int iPos = 0;
			foreach(char ch in m_rgLetter)
			{
				if (ch == chFirst)
					break;
				iPos++;
			}

			// If not, then insert it
			if (iPos == m_rgLetter.Length)
			{
				char[] rgLetter    = new char    [ m_rgLetter.Length + 1];
				TreeNode[] rgLower = new TreeNode[ m_rgLetter.Length + 1];

				int i = 0;

				while ( i < m_rgLetter.Length && m_rgLetter[i] < chFirst )
				{
					rgLetter[i] = m_rgLetter[i];
					rgLower [i] = m_rgLower [i];
					i++;
				}

				iPos = i;
				rgLetter[iPos] = chFirst;
				rgLower [iPos] = new TreeNode(this);

				while ( i < m_rgLetter.Length )
				{
					rgLetter[i+1] = m_rgLetter[i];
					rgLower [i+1] = m_rgLower [i];
					i++;
				}

				m_rgLetter = rgLetter;
				m_rgLower  = rgLower;
			}

			// Go down the next level
			string sLower = sSource.Substring(0, sSource.Length - 1);
			m_rgLower[iPos].Add(sLower, sResult);
		}
		#endregion

		#region Method: string Search(string sSource, int iPos, ref int Depth)
		protected string Search(string sSource, int iPos, ref int cSourceLen)
		{
			if (iPos >= 0)
			{
				char chSource = sSource[iPos];

				// Search through the letters in this node
				int iTop = 0;                                       // e.g., 'a'
				int iBottom = m_rgLetter.Length - 1;                // e.g., 'z'

				while( iTop <= iBottom)
				{
					// We'll examine the item in the middle between iTop and iBottom
					int i = (iTop + iBottom) / 2;                   // e.g., "n"
					char ch = m_rgLetter[i];

					// If the item is less than or greater than, then we know to narrow
					// the range to the appropriate half.
					if (chSource < ch)                              // "Target" < "n"
						iBottom = i - 1;                            // e.g., "m"
					else if (chSource > ch)
						iTop = i + 1;                               // e.g., "o"

					// Otherwise, we found it! If we can end here, then we return the
					// result; otherwise we search deeper.
					else
					{
						string sLowerResult = m_rgLower[i].Search(sSource, --iPos, ref cSourceLen);
						if (null != sLowerResult && sLowerResult.Length > 0)
							return sLowerResult;
						break;
					}
				}
			}

			if (m_bCanEndHere)
			{
				cSourceLen = Depth;
				return m_sResult;
			}
			return null;
		}
		#endregion

		#region Constructor()
		public TreeNode(TreeNode parent)
		{
			m_rgLetter = new char[0];
			m_rgLower  = new TreeNode[0];
			m_parent   = parent;
		}
		#endregion

	}

	public class TreeRoot : TreeNode
	{
		#region Method: string Search(string sSearchFor, ref int cSourceLen)
		public string Search(string sSource, ref int cSourceLen)
		{
			int i = sSource.Length - 1;
			cSourceLen = 0;
			return Search(sSource, i, ref cSourceLen);
		}
		#endregion
		#region Constructor()
		public TreeRoot()
			: base(null)
		{
		}
		#endregion
		#region Method: string MakeReplacements(string s)
		public string MakeReplacements(string s)
		{
			// Use this counter to work through the source string. We start at the
			// end and work backwards, as this is how Search works (since it was
			// designed to support AutoReplace.
			int i = s.Length;

			// Loop through the source string
			while (i > 0)
			{
				// We'll examine from the beginning of the string to the current position
				string sSource = s.Substring(0, i);

				// See if there is a match (and a replacement) in the Tree. If not, 
				// decrement the position and try again.
				int cReplaceCount = 0;
				string sResult = Search(sSource, ref cReplaceCount);
				if (null == sResult || sResult.Length == 0 || cReplaceCount == 0)
				{
					i--;
					continue;
				}

				// If we are here, then we have a replacement to make
				string sLeft = s.Substring(0, i - cReplaceCount);
				string sRight = s.Substring(i);
				s = sLeft + sResult + sRight;;

				// Decrement the counter past the newly replaced text
				i -= sResult.Length;
			}

			return s;
		}
		#endregion
	}
}
