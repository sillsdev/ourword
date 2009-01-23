/**********************************************************************************************
 * Project: OurWord!
 * File:    EControl.cs
 * Author:  John Wimbish
 * Created: 14 Jan 2009
 * Purpose: Supports, e.g., an Edit control in a view
 * Legal:   Copyright (c) 2004-09, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
#region Using
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Text;
using JWTools;
using JWdb;
using OurWord.DataModel;
#endregion

namespace OurWord.Edit
{
    public class EControl : EItem
    {
        #region Attr{g}: Control Control
        protected Control Control
        {
            get
            {
                Debug.Assert(null != m_Control);
                return m_Control;
            }
        }
        Control m_Control;
        #endregion

        // Screen Region ---------------------------------------------------------------------
        #region OAttr{g/s}: PointF Position
        public override PointF Position
        {
            get
            {
                return Control.Location;
            }
            set
            {
                Control.Location = new Point( (int)value.X, (int)value.Y);
            }
        }
        #endregion
        #region OAttr{g/s}: float Height
        public override float Height
        {
            get
            {
                return Control.Height;
            }
            set
            {
                Control.Height = (int)value;
            }
        }
        #endregion
        #region OAttr{g/s}: float Width
        public override float Width
        {
            get
            {
                return Control.Width;
            }
            set
            {
                Control.Width = (int)value;
            }
        }
        #endregion

        // Scaffolding -----------------------------------------------------------------------
        #region Constructor(Control)
        public EControl(Control _Control)
            : base()
        {
            m_Control = _Control;
        }
        #endregion
        #region OMethod: EBlock GetBlockAt(pt) - return null
        public override EBlock GetBlockAt(PointF pt)
        {
            return null;
        }
        #endregion
    }


    public class ETextBox : EControl
    {
        #region VAttr{g}: TextBox TextBox
        TextBox TextBox
        {
            get
            {
                Debug.Assert(null != Control as TextBox);
                return Control as TextBox;
            }
        }
        #endregion
        #region VAttr{g/s}: string Text
        public string Text
        {
            get
            {
                return TextBox.Text;
            }
            set
            {
                TextBox.Text = value;
            }
        }
        #endregion

        // Scaffolding -----------------------------------------------------------------------
        #region Constructor()
        public ETextBox()
            : base( new TextBox())
        {
        }
        #endregion
        #region Constructor(sText)
        public ETextBox(string sText)
            : this()
        {
            Text = sText;
        }
        #endregion

    }


}
