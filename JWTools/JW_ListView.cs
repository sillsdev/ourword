/**********************************************************************************************
 * Dll:     JWTools
 * File:    JW_ListView.cs
 * Author:  John Wimbish
 * Created: 19 Jan 2006
 * Purpose: Extended ListView that includes editing of subitems.
 * Legal:   Copyright (c) 2004-06, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
#region Using
using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.IO;
using Microsoft.Win32;
#endregion

namespace JWTools
{
	#region Class: JWLVTextBox - TextBox subclass that captures the Return key
	public class JWLVTextBox : TextBox
	{
		#region Attr{g/s}: bool PreventSpace
		public bool PreventSpace
		{
			get
			{
				return m_bPreventSpace;
			}
			set
			{
				m_bPreventSpace = value;
			}
		}
		bool m_bPreventSpace = false;
		#endregion

		#region Method: override bool IsInputKey(Keys keyData)
		protected override bool IsInputKey(Keys keyData)
			// Test for Enter being pressed. By returning True, it means that this
			// will be passed on to the TextBox as a trigger for KeyDown, KeyPress,
			// etc. Otherwise, the TextBox handles it internally and passes it
			// on to the parent as well (which in my testing has the annoying
			// efect of closing the dialog box.)
		{
			if (keyData == Keys.Enter)
				return true;

			return base.IsInputKey (keyData);
		}
		#endregion
		#region Method: override void OnKeyPress(KeyPressEventArgs e)
		protected override void OnKeyPress(KeyPressEventArgs e)
		{
			// Prevent most types of whitespace
			if (e.KeyChar != (char)Keys.Space && char.IsWhiteSpace(e.KeyChar))
				e.Handled = true;

			// Prevent punctuation
			if (char.IsPunctuation(e.KeyChar))
				e.Handled = true;

			// Preventing the spacebar is dependent on the PreventSpace attribute
			if (PreventSpace && e.KeyChar == (char)Keys.Space)
				e.Handled = true;

			base.OnKeyPress (e);
		}
		#endregion
	}
	#endregion

	public delegate void SubItemEditedEventHandler(object sender, JW_SubItemEditedArgs e);

	#region Class: JW_SubItemEditedArgs - used in the SubItemEdited event
	public class JW_SubItemEditedArgs
		// Event args for the SubItemEdited events
	{
		// Attrs -----------------------------------------------------------------------------
		#region Attr{g}: string NewText
		public string NewText
		{
			get
			{
				return m_sNewText;
			}
		}
		string m_sNewText;
		#endregion
		#region Attr{g}: string OldText
		public string OldText
		{
			get
			{
				return m_sOldText;
			}
		}
		string m_sOldText;
		#endregion
		#region Attr{g}: ListViewItem ListViewItem
		public ListViewItem ListViewItem
		{
			get
			{
				return m_item;
			}
		}
		ListViewItem m_item;
		#endregion
		#region Attr{g}: int ListViewItemIndex
		public int ListViewItemIndex
		{
			get
			{
				return m_iListViewItemIndex;
			}
		}
		int m_iListViewItemIndex = -1;
		#endregion
		#region Attr{g/s}: bool SaveChanges 
		public bool SaveChanges
		{
			get
			{
				return m_bSaveChanges;
			}
			set
			{
				m_bSaveChanges = value;
			}
		}
		bool m_bSaveChanges = true;
		#endregion

		// Scaffolding -----------------------------------------------------------------------
		#region Constructor(...)
		public JW_SubItemEditedArgs(
			string _sNewText, 
			string _sOldText, 
			ListViewItem _item,
			int _iListViewItemIndex)
		{
			m_sNewText = _sNewText;
			m_sOldText = _sOldText;
			m_item = _item;
			m_iListViewItemIndex = _iListViewItemIndex;
		}
		#endregion
	}
	#endregion

	public class JW_ListView : ListView
	{
		// Attrs -----------------------------------------------------------------------------
		JWLVTextBox m_TextBox = null;       // The control for doing the editing
		ListViewItem m_Item = null;         // The item currently selected (or being edited)
		int m_idxItem = -1;                 // The subitem currently selected / edited
		public event SubItemEditedEventHandler SubItemEndEditing;

		// Scaffolding -----------------------------------------------------------------------
		#region Constructor()
		public JW_ListView()
		{
			// Required by	the	Windows.Forms Form Designer.
			InitializeComponent();

			// ListView properties we require to be set
			base.FullRowSelect = true;
			base.View = View.Details;
			base.AllowColumnReorder = false;
		}
		#endregion
		#region Method: void Dispose(...)
		protected override void	Dispose( bool disposing	)
			//	Clean up any resources being used.
		{
			if(	disposing )
			{
				if(	components != null )
					components.Dispose();
			}
			base.Dispose( disposing	);
		}
		#endregion
		#region Component	Designer generated code
		//	Required designer variable.
		private	System.ComponentModel.Container	components = null;

		///	<summary>
		///	Required method	for	Designer support - do not modify 
		///	the	contents of	this method	with the code editor.
		///	</summary>
		private	void InitializeComponent()
		{
			components = new System.ComponentModel.Container();
		}
		#endregion

		// Start/End Editing (heavy lifting) -------------------------------------------------
		#region Method: int GetSubItemAt(Point pt, out ListViewItem item)
		public int GetSubItemAt(Point pt, out ListViewItem item, out Rectangle rectBounds)
			// Find the ListViewItem (item) and the subitem's index at the given position.
			//
			// Parameters: pt - the x,y location of the click relative to the ListView
			//
			// Out: item - the containing ListViewItem (or null)
			//
			// Returns: the 0-based index of the subitem, or -1 if not found.
		{
			// Default out values
			item = null;
			rectBounds = Rectangle.Empty;

			// Get the item (for the entire row)
			item = GetItemAt(pt.X, pt.Y);
			if (null == item)
				return -1;
			rectBounds = item.GetBounds(ItemBoundsPortion.Entire);
			if (null == item)
				return -1;

			// Look in each column for a match in the hit test
			int xLeft = 0;
			foreach( ColumnHeader header in Columns)
			{
				// Is the Point contained within this column? Return its index.
				// and figure out its bounds
				if (pt.X < xLeft + header.Width)
				{
					rectBounds.X     = xLeft;
					rectBounds.Width = header.Width;

					return header.Index;
				}

				// Otherwise, increment to the next column.
				xLeft += header.Width;
			}
			
			// Didn't find it
			return -1;
		}
		#endregion
		#region Method: void StartEditing(Point pt)
		private void StartEditing(Point pt)
			// Begin editing of the item under the clicked-upon Point.
			//
			// pt - Point of click/doubleclick
		{
			// Figure out what subitem we're at; abort if anything does not line up
			ListViewItem item;
			Rectangle rectBounds;
			int idx = GetSubItemAt(pt, out item, out rectBounds);
			if (null == item || idx == -1)
				return;

			// We only edit if the subitem we've just looked up is the one
			// we were currently sitting at.
			bool bEditThisItem = true;
			if (item != m_Item || idx != m_idxItem)
				bEditThisItem = false;
			m_Item = item;
			m_idxItem = idx;
			if (!bEditThisItem)
				return;

			// Adjust the rectangle to make sure it will be visible
			rectBounds.X = Math.Max( rectBounds.X, 0);
			if (rectBounds.X + rectBounds.Width > Width)
				rectBounds.Width = Width - rectBounds.Left;

			// Create the TextBox editor if we don't have one already
			if (null == m_TextBox)
			{
				SuspendLayout();
				m_TextBox = new JWLVTextBox();
				m_TextBox.BorderStyle = BorderStyle.FixedSingle;
				m_TextBox.Multiline = false;
				m_TextBox.Name = "SubItemEditor";
				m_TextBox.Size = new System.Drawing.Size(80, 16);
				m_TextBox.TabIndex = 1;
				m_TextBox.Text = "";
				m_TextBox.Visible = false;
				m_TextBox.KeyPress += new KeyPressEventHandler( editor_KeyPress );
				m_TextBox.Leave += new EventHandler( editor_Leave );
				Controls.Add( m_TextBox );
				ResumeLayout(false);
			}

			// Position and show the editor
			m_TextBox.Text = item.SubItems[idx].Text;
			m_TextBox.Bounds = rectBounds;
			m_TextBox.PreventSpace = ((idx == 0) ? true : false);
			m_TextBox.Visible = true;
			m_TextBox.BringToFront();
			m_TextBox.Focus();
		}
		#endregion
		#region Method: void EndEditing(bool bAcceptChanges)
		private void EndEditing(bool bAcceptChanges)
		{
			if (null == m_TextBox || m_TextBox.Visible == false)
				return;

			// If we are ending the editing with the desire to keep the changes (e.g., 
			// the Return key was hit...
			if (bAcceptChanges)
			{
				// Build the args for the event, then raise the event
				JW_SubItemEditedArgs e = new JW_SubItemEditedArgs(
					m_TextBox.Text, 
					m_Item.SubItems[m_idxItem].Text,
					m_Item, 
					m_idxItem);
				if (null != SubItemEndEditing)
					SubItemEndEditing(this, e);

				// This gives the user the opportunity to abort the changes just made.
				if (e.SaveChanges)
					m_Item.SubItems[m_idxItem].Text = m_TextBox.Text;
			}

			m_TextBox.Visible = false;
		}
		#endregion

		// ListView Handlers ----------------------------------------------------------------
		#region Cmd: OnMouseUp - Start Editing the item the mouse is over (if appropriate)
		protected override void OnMouseUp(System.Windows.Forms.MouseEventArgs e)
		{
			base.OnMouseUp(e);

			StartEditing( new Point(e.X, e.Y) );
		}
		#endregion

		#region Windows Messages that Abort Editing
		// Message Header for WM_NOTIFY
		private struct NMHDR 
		{ 
			public IntPtr hwndFrom; 
			public Int32  idFrom; 
			public Int32  code; 
		}

		// Windows messages that will abort editing
		private	const int WM_HSCROLL = 0x114;
		private	const int WM_VSCROLL = 0x115;
		private const int WM_SIZE	 = 0x05;
		private const int WM_NOTIFY	 = 0x4E;
		private const int HDN_FIRST = -300;
		private const int HDN_BEGINDRAG = (HDN_FIRST-10);
		private const int HDN_ITEMCHANGINGA = (HDN_FIRST-0);
		private const int HDN_ITEMCHANGINGW = (HDN_FIRST-20);
		#endregion
		#region Method: override void WndProc(ref Message m)
		protected override void WndProc(ref Message m)
			// Look for windows messages that indicate that editing should be halted.
			// These include:
			// - Scro(ling
			// - Resizing
			// - Notifications (e.g., coloum resizing)
		{
			switch (m.Msg)
			{
				case WM_VSCROLL:
				case WM_HSCROLL:
				case WM_SIZE:
					EndEditing(false);
					break;

				case WM_NOTIFY:
					NMHDR h = (NMHDR)Marshal.PtrToStructure(m.LParam, typeof(NMHDR));
					if (h.code == HDN_BEGINDRAG ||
						h.code == HDN_ITEMCHANGINGA ||
						h.code == HDN_ITEMCHANGINGW)
						EndEditing(false);
					break;
			}

			base.WndProc (ref m);
		}
		#endregion

		// TextBox Handlers ------------------------------------------------------------------
		#region Handler: editor_KeyPress
		private void editor_KeyPress(object sender, KeyPressEventArgs e)
		{
			switch (e.KeyChar)
			{
				case (char)(int)Keys.Escape:
				{
					EndEditing(false);
					e.Handled = true;
					break;
				}

				case (char)(int)Keys.Enter:
				{
					EndEditing(true);
					e.Handled = true;
					break;
				}
			}
		}
		#endregion
		#region Handler: editor_Leave
		private void editor_Leave(object sender, EventArgs e)
			// cell editor losing focus
		{
			EndEditing(false);
		}
		#endregion
	}
}
