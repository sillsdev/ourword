#region ***** EIcon.cs *****
/**********************************************************************************************
 * Project: OurWord!
 * File:    EIcon.cs
 * Author:  John Wimbish
 * Created: 27 Sep 2008
 * Purpose: A icon with an optional associated action
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
using System.Text;
using System.Windows.Forms;
using JWTools;
using OurWordData;
using OurWordData.DataModel;
#endregion
#endregion

namespace OurWord.Edit
{
    public class EIcon : EBlock
    {
        #region Attr{g}: Bitmap Bmp - the note's bitmap
        Bitmap Bmp
        {
            get
            {
                // Get this when we first need it. We can't do this in the constructor
                // because we do not have access to the Window at the time of construction.
                if (null == m_bmp)
                {
                    m_bmp = JWU.GetBitmap(IconResource);
                    Debug.Assert(null != m_bmp, "Unable to load resource: " + IconResource);
                    JWU.ChangeBitmapBackground(m_bmp, Window.BackColor);
                }

                return m_bmp;
            }
        }
        Bitmap m_bmp = null;
        #endregion
        #region OAttr{g}: float Width
        public override float Width
        {
            get
            {
                return Bmp.Width;
            }
            set
            {
                // Can't be set; its the nature of the bitmap
            }
        }
        #endregion
        #region Attr{g/s}: string IconResource
        public string IconResource
        {
            get
            {
                return m_sIconResource;
            }
            set
            {
                m_sIconResource = value;
                m_bmp = null;
            }
        }
        string m_sIconResource;
        #endregion

        #region Constructor()
        public EIcon(string sIconResource)
            : base(null, "")
        {
            m_sIconResource = sIconResource;
        }
        #endregion
        #region OMethod: void CalculateWidth(Graphics g)
        public override void CalculateWidth(Graphics g)
        {
            // Do-nothing override
        }
        #endregion
        #region Method: override void Paint()
        public override void Paint()
        {
            Draw.Image(Bmp, Position);
        }
        #endregion

    }
}
