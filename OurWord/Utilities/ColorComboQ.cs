/**********************************************************************************************
 * Project: Our Word!
 * File:    ColorCombo.cs
 * Author:  John Wimbish
 * Created: 22 Nov 2004
 * Purpose: A color picking combo, displays the "web" colors (all of which have names).
 * Legal:   Copyright (c) 2005-08, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
#region Using
using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

using OurWord.DataModel;
using OurWord.View;
using JWdb;
using JWTools;
#endregion


namespace OurWord.View
{

	/* USAGE:
	 * - Create a normal combo box in the forms designer
	 * - Go into the code, and change the the following to ColorCombo:
	 *     + the variable declaration
	 *     + the call to "new"
	 */
	public class ColorCombo : ComboBox
	{
		#region Attr{g/s}: string InitialColor - Will be selected in the combo when created
		public string InitialColor
		{
			get
			{
				return m_sInitialColor;
			}
			set
			{
				m_sInitialColor = value;
			}
		}
		public string m_sInitialColor = "Black";
		#endregion
		#region Attr{g/s}: string ChosenColor - The color the user has selected
		public string ChosenColor
		{
			get
			{
				if (-1 == SelectedIndex)
					return InitialColor;

				string sColor = "";
				string sFull = Items[ SelectedIndex ].ToString();

				int i = 0;
				while (sFull[i] != '\0' && sFull[i] != '[')
					i++;
				if (sFull[i] != '\0' && sFull[i] == '[')
					i++;
				while (sFull[i] != '\0' && sFull[i] != ']')
					sColor += sFull[i++];
				return sColor;
			}
			set
			{
				for(int i = 0; i < Items.Count; i++)
				{
					Color clr = (Color)Items[i];

					if (clr.Name == value)
					{
						SelectedIndex = i;
						Invalidate();
						break;
					}
				}
			}
		}
		#endregion

		#region Constructor()
		public ColorCombo()
			: base()
		{
			DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
			DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			MaxDropDownItems = 16;
		}
		#endregion

		#region Method: override void OnDrawItem(DrawItemEventArgs e)
		protected override void OnDrawItem(DrawItemEventArgs e)
		{
			// Not anything valid to draw
			if (e.Index == -1)
				return;

			// The item's color
			Color clr = (Color)Items[e.Index];

			// The rectangle of the entire combo-box item
			Rectangle r = e.Bounds;

			// The brush for drawing text 
			Brush textbrush = SystemBrushes.ControlText;

			// Draw the background; change the textbrush if the item is drawn
			// as selected.
			if( (e.State & DrawItemState.Selected) !=0 )
			{
				textbrush = SystemBrushes.HighlightText;
				e.Graphics.FillRectangle(SystemBrushes.Highlight, e.Bounds);
			}
			else
				e.Graphics.FillRectangle(SystemBrushes.Window, e.Bounds);

			// Draw the color sample rectangle at the left
			Rectangle rectClrSample = new Rectangle(r.X+1, r.Y+2, 24, r.Height-4);
			e.Graphics.FillRectangle(new SolidBrush(clr), rectClrSample);
			e.Graphics.DrawRectangle(SystemPens.ControlText, rectClrSample);

			// Draw the text representing the name of the color
			r.Offset(30,0);
			r.Width -= 30;
			e.Graphics.DrawString(clr.Name, Font,
				textbrush, r, StringFormat.GenericTypographic);
		}
		#endregion

		#region Method: override void OnCreateControl()
		protected override void OnCreateControl()
		{
			// Superclass processing
			base.OnCreateControl();

			// Clear out any items (in case this gets called multiple times)
			Items.Clear();

			// Retrieve all of the "web" colors in the system
			Type type = typeof(Color);
			System.Reflection.PropertyInfo[] fields = type.GetProperties(
				System.Reflection.BindingFlags.Public | 
				System.Reflection.BindingFlags.Static);

			// Build the string for the initial color, e.g., "Color [Black]"
			string sInitialColor = "Color [" + InitialColor + "]";

			// This will hold the index corresponding to sInitialColor
			int nInitialIndex = 0;

			// Loop through all of the colors, adding to the combo box items
			Color clr = new Color();
			int i = 0;
			foreach(System.Reflection.PropertyInfo pi in fields)
			{
				// The first one is "transparent", which isn't supported. So
				// we skip it.
				if (i > 0)
				{
					Color c = (Color)pi.GetValue(clr, null);

					Items.Add(c);

					if (sInitialColor == c.ToString())
						nInitialIndex = i - 1;
				}
				i++;
			}

			// Select the initial color in the combo box
			SelectedIndex = nInitialIndex;
		}
		#endregion

	}
}
