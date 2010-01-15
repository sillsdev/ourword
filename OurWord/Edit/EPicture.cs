#region ***** EPicture.cs *****
/**********************************************************************************************
 * Project: OurWord!
 * File:    EPicture.cs
 * Author:  John Wimbish
 * Created: 08 Jan 2010
 * Purpose: A picture to draw on screen or printer
 * Legal:   Copyright (c) 2004-09, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
using System.Drawing;
#endregion

namespace OurWord.Edit
{
    public class EPicture
    {
        private readonly Bitmap m_Picture;
        private readonly EContainer m_OwningContainer;

        // Scaffolding -----------------------------------------------------------------------
        #region Constructor(bmp, EContainer)
        public EPicture(Bitmap bmp, EContainer owningContainer)
        {
            m_Picture = bmp;
            m_OwningContainer = owningContainer;
        }
        #endregion

        // Measure, Layout & Draw ------------------------------------------------------------
        private const int c_yMargin = 4;         // vert marg above/below the bitmap
        #region Method: void Draw(IDraw)
        public void Draw(IDraw draw)
        {
            var xBitmapLeft = m_OwningContainer.Rectangle.Left +
                (m_OwningContainer.Width - m_Picture.Width) / 2;

            var yBorderWidth = (null == m_OwningContainer.Border) ? 0 : 
                m_OwningContainer.Border.BorderWidth;
            var yBitmapTop = m_OwningContainer.Rectangle.Top + yBorderWidth + c_yMargin;

            draw.DrawImage(m_Picture, new PointF(xBitmapLeft, yBitmapTop));
        }
        #endregion
        #region VAttr{g}: float Height
        public float Height
        {
            get
            {
                float fHeightBmp = 0;

                fHeightBmp += c_yMargin;

                fHeightBmp += m_Picture.Height;

                fHeightBmp += c_yMargin;

                return fHeightBmp;
            }
        }
        #endregion
        #region Method:  void SetY(float yNew)
        public void SetY(float yNew)
        {
            var x = m_OwningContainer.Position.X;
            m_OwningContainer.Position = new PointF(x, yNew);
        }
        #endregion
    }
}
