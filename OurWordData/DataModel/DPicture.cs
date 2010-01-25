/**********************************************************************************************
 * Project: Our Word!
 * File:    DPicture.cs
 * Author:  John Wimbish
 * Created: 26 Jan 2004
 * Purpose: Handles a picture (including its caption, caption BT, filename, etc.) 
 * Legal:   Copyright (c) 2005-09, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
#region Using
using System;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Xml;

using JWTools;
using OurWordData;
using OurWordData.Styles;

#endregion

namespace OurWordData.DataModel
{
	public class DPicture : DParagraph
	{
		// BAttrs ----------------------------------------------------------------------------
		#region BAttr{g/s}: string PathName - the path/file where the picture is stored
		public string PathName
		{
			get
			{
				return m_sPathName;
			}
			set
			{
                SetValue(ref m_sPathName, value);
			}
		}
		private string m_sPathName = "";
		#endregion
		#region BAttr{g/s}: string WordRtfInfo - Info needed by Word to properly display the pic
		public string WordRtfInfo
		{
			get
			{
				return m_sWordRtfInfo;
			}
			set
			{
                SetValue(ref m_sWordRtfInfo, value);
			}
		}
		private string m_sWordRtfInfo = "";
		#endregion
		#region Method: void DeclareAttrs()
		protected override void DeclareAttrs()
		{
			base.DeclareAttrs();
			DefineAttr("PathName",    ref m_sPathName);
			DefineAttr("WordRtfInfo", ref m_sWordRtfInfo);
		}
		#endregion

		// JAttrs ----------------------------------------------------------------------------
		#region JAttr{g/s}: DReference Reference - the verse before this picture
		public DReference Reference
		{
			get 
			{ 
				return j_Reference.Value; 
			}
			set
			{
                j_Reference.Value.Copy(value);
			}
		}
		private JOwn<DReference> j_Reference = null;
		#endregion

		// Scaffolding -----------------------------------------------------------------------
		#region Constructor()
		public DPicture()
			: base(StyleSheet.PictureCaption)
		{
			j_Reference = new JOwn<DReference>("Reference", this);
			j_Reference.Value = new DReference();
		}
		#endregion
		#region Method: override bool ContentEquals(obj) - required override to preventn duplicates
		public override bool ContentEquals(JObject obj)
		{
			if (this.GetType() != obj.GetType())
				return false;

			if (false == base.ContentEquals(obj))
				return false;

			DPicture picture = obj as DPicture;

			if (PathName != picture.PathName)
				return false;

			if (WordRtfInfo != picture.WordRtfInfo)
				return false;

			return true;
		}
		#endregion
        #region Method: override void CopyFrom(DParagraph pFront, bTruncateText))
        public override void CopyFrom(DParagraph pFront, bool bTruncateText)
		{
			DPicture pictFront = pFront as DPicture;
			Debug.Assert(null != pictFront);

			// Copy the paragraph contents
            base.CopyFrom(pFront, bTruncateText);

			// Copy the picture information from the Front's picture
			PathName = pictFront.PathName;
			WordRtfInfo = pictFront.WordRtfInfo;
			Reference.Copy( pictFront.Reference );
		}
		#endregion

        // Methods ---------------------------------------------------------------------------
        #region Method: Bitmap GetBitmapNotFound(Size size, string sFileName)
        public Bitmap GetBitmapNotFound(Size size, string sFileName)
        {
            Bitmap bmp = new Bitmap(size.Width, size.Height);

            Graphics g = Graphics.FromImage(bmp);

            // Solid white background
            g.FillRectangle(new SolidBrush(Color.White), 0, 0, bmp.Width, bmp.Height);

            // Border around the background
            g.DrawRectangle(new Pen(Color.Black), 1, 1, bmp.Width - 2, bmp.Height - 2);

            // Retrieve the message, typically "Unable to display "filename"".
            string sMessage = "Unable to display \"" + sFileName + "\"";

            // Compute where it shall appear (centered)
            Font font = SystemFonts.MenuFont;
            SizeF szMessage = g.MeasureString(sMessage, font);
            int x = (bmp.Width - (int)szMessage.Width) / 2;
            int y = (bmp.Height - (int)szMessage.Height) / 2;

            // Draw the string
            g.DrawString(sMessage, font, new SolidBrush(Color.Black), x, y);

            // Done with the graphics object
            g.Dispose();

            return bmp;
        }
        #endregion
        #region Method: bool _LocateThePicture()
        bool _LocateThePicture()
        {
            // If we have a zero pathname, then there is nothing to locate
            if (string.IsNullOrEmpty(PathName))
                return false;

            // See if we have the file on disk; we're done if we do.
            if (File.Exists(PathName))
                return true;

            // Do we have a PictureSearchPath to work with?
            string sPictureSearchPath = DB.PictureSearchPath;
            if (string.IsNullOrEmpty(sPictureSearchPath))
                return false;

            // Extract the filename
            string sFileName = Path.GetFileName(PathName);

            // First check the PictureSearchDirectory
            string sTop = sPictureSearchPath + Path.DirectorySeparatorChar + sFileName;
            if (File.Exists(sTop))
            {
                PathName = sTop;
                return true;
            }

            // Otherwise, check all of the subdirectories
            string[] vsDirectories = Directory.GetDirectories(sPictureSearchPath);
            foreach (string sDir in vsDirectories)
            {
                string s = sDir + Path.DirectorySeparatorChar + sFileName;
                if (File.Exists(s))
                {
                    PathName = s;
                    return true;
                }
            }

            // If we're here, we're out of options
            return false;
        }
        #endregion
        #region Method: Bitmap GetBitmap(int nMaxDimension)
        public Bitmap GetBitmap(int nMaxDimension)
		{
            try
			{
                // Verify that the PathName exists, or try to find it otherwise
                if (!_LocateThePicture())
                {
                    return GetBitmapNotFound(new Size(nMaxDimension, nMaxDimension / 3),
                        Path.GetFileName(PathName));
                }

				// Read in the file into a bitmap
				Bitmap bmpFromFile = new Bitmap(PathName);

                // Just return the bitmap unsized
                if (nMaxDimension <= 0)
                    return bmpFromFile;

				// Calculate width based on width being greater than height
				int nWidth  = Math.Min(bmpFromFile.Width, nMaxDimension);
				int nHeight = (nWidth * bmpFromFile.Height) / bmpFromFile.Width;

				// If the height was greater, then recalculate
				if (bmpFromFile.Height > bmpFromFile.Width)
				{
					nHeight = Math.Min(bmpFromFile.Height, nMaxDimension);
					nWidth  = (nHeight * bmpFromFile.Width) / bmpFromFile.Height;
				}

				// Create a new bitmap that will fit property
				Bitmap bmpResized = new Bitmap(bmpFromFile, nWidth, nHeight);
				return bmpResized;
			}
			catch
			{
                return GetBitmapNotFound(new Size(nMaxDimension, nMaxDimension/3), 
                    Path.GetFileName(PathName));
            }
		}
		#endregion

        // Oxes ------------------------------------------------------------------------------
        #region Constants
        const string c_sTagPicture = "fig";
        const string c_sAttrPath = "path";
        const string c_sAttrRtf = "rtfFormat";
        #endregion
        #region SMethod: DPicture CreatePicture(nodePicture)
        static public DPicture CreatePicture(XmlNode nodePicture)
        {
            if (nodePicture.Name != c_sTagPicture)
                return null;

            // Create the new picture object
            var picture = new DPicture();

            // We expect to have a path name
            picture.PathName = XmlDoc.GetAttrValue(nodePicture, c_sAttrPath, "");
            if (string.IsNullOrEmpty(picture.PathName))
                throw new XmlDocException(nodePicture, "Missing picture path in oxes file.");

            // The Rtf info is optional
            picture.WordRtfInfo = XmlDoc.GetAttrValue(nodePicture, c_sAttrRtf, "");

            // The superclass takes care of the rest of it
            picture.ReadOxes(nodePicture);

            // Except that we override the style with out own
            picture.Style = StyleSheet.PictureCaption;

            return picture;
        }
        #endregion
        #region OMethod: XmlNode SaveToOxesBook(oxes, nodeBook)
        public override XmlNode SaveToOxesBook(XmlDoc oxes, System.Xml.XmlNode nodeBook)
        {
            var nodePicture = oxes.AddNode(nodeBook, c_sTagPicture);

            oxes.AddAttr(nodePicture, c_sAttrPath, PathName);

            if (!string.IsNullOrEmpty(WordRtfInfo))
                oxes.AddAttr(nodePicture, c_sAttrRtf, WordRtfInfo);

            foreach (DRun run in Runs)
                run.SaveToOxesBook(oxes, nodePicture);

            return nodePicture;
        }
        #endregion

    }
}
