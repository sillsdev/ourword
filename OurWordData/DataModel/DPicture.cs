#region ***** DPicture.cs *****
/**********************************************************************************************
 * Project: Our Word!
 * File:    DPicture.cs
 * Author:  John Wimbish
 * Created: 26 Jan 2004
 * Purpose: Handles a picture (including its caption, caption BT, filename, etc.) 
 * 
 * Pictures are stored under the cluster's ".Pictures" folder, which may then hsve subfolders
 * underneath. Thus in Oxes we store the relative path name (although on export to Toolbox
 * we save an absolute path so that the pictures can be located by external software.) 
 * 
 * On encountering an absolute path, the Repair() method converts it to relative, and
 * copies the pictures into the ".Pictures" folder (if not already there) so that it can
 * be included in the repository.
 * 
 * Legal:   Copyright (c) 2005-09, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Xml;

using JWTools;
using OurWordData.Styles;
#endregion

namespace OurWordData.DataModel
{
	public class DPicture : DParagraph
	{
        // BAttrs ----------------------------------------------------------------------------
        #region BAttr{g/s}: string RelativePathName - the path/file where the picture is stored
        public string RelativePathName
		{
			get
			{
                return m_sRelativePathName;
			}
			set
			{
                SetValue(ref m_sRelativePathName, value);
			}
		}
        private string m_sRelativePathName = "";
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
            DefineAttr("PathName", ref m_sRelativePathName);
			DefineAttr("WordRtfInfo", ref m_sWordRtfInfo);
		}
		#endregion

		// JAttrs ----------------------------------------------------------------------------
		#region JAttr{g/s}: DReference Reference - the verse before this picture
	    private DReference Reference
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
		private readonly JOwn<DReference> j_Reference ;
		#endregion
        #region VAttr{g}: string FullPathName
        public string FullPathName
	    {
	        get
	        {
                return Path.Combine(DB.TeamSettings.PicturesFolder, RelativePathName);
	        }
        }
        #endregion

        // Scaffolding -----------------------------------------------------------------------
		#region Constructor()
		public DPicture()
			: base(StyleSheet.PictureCaption)
		{
			j_Reference = new JOwn<DReference>("Reference", this) {Value = new DReference()};
		}
		#endregion
		#region Method: override bool ContentEquals(obj) - required override to preventn duplicates
		public override bool ContentEquals(JObject obj)
		{
			if (GetType() != obj.GetType())
				return false;

			if (false == base.ContentEquals(obj))
				return false;

			var picture = obj as DPicture;
            if (null == picture)
                return false;

			if (RelativePathName != picture.RelativePathName)
				return false;

			if (WordRtfInfo != picture.WordRtfInfo)
				return false;

			return true;
		}
		#endregion
        #region Method: override void CopyFrom(DParagraph pFront, bTruncateText))
        public override void CopyFrom(DParagraph pFront, bool bTruncateText)
		{
			var pictFront = pFront as DPicture;
			Debug.Assert(null != pictFront);

			// Copy the paragraph contents
            base.CopyFrom(pFront, bTruncateText);

			// Copy the picture information from the Front's picture
			RelativePathName = pictFront.RelativePathName;
			WordRtfInfo = pictFront.WordRtfInfo;
			Reference.Copy( pictFront.Reference );
		}
		#endregion

        // Methods ---------------------------------------------------------------------------
        #region SMethod: Bitmap GetBitmapNotFound(Size size, string sFileName)
	    private static Bitmap GetBitmapNotFound(Size size, string sFileName)
        {
            var bmp = new Bitmap(size.Width, size.Height);

            var g = Graphics.FromImage(bmp);

            // Solid white background
            g.FillRectangle(new SolidBrush(Color.White), 0, 0, bmp.Width, bmp.Height);

            // Border around the background
            g.DrawRectangle(new Pen(Color.Black), 1, 1, bmp.Width - 2, bmp.Height - 2);

            // Retrieve the message, typically "Unable to display "filename"".
            var sMessage = "Unable to display \"" + sFileName + "\"";

            // Compute where it shall appear (centered)
            var font = SystemFonts.MenuFont;
            var szMessage = g.MeasureString(sMessage, font);
            var x = (bmp.Width - (int)szMessage.Width) / 2;
            var y = (bmp.Height - (int)szMessage.Height) / 2;

            // Draw the string
            g.DrawString(sMessage, font, new SolidBrush(Color.Black), x, y);

            // Done with the graphics object
            g.Dispose();

            return bmp;
        }
        #endregion
        #region Method: bool _LocateThePicture()
        bool LocateThePicture()
        {
            // If we have a zero pathname, then there is nothing to locate
            if (string.IsNullOrEmpty(RelativePathName))
                return false;

            // We want to move pictures from C:\graphics to be underneath ".Pictures"
            Repair();

            // If we already have a file, we're done
            if (File.Exists(FullPathName))
                return true;

            // Extract the filename from the user-supplied full path
            var sFileName = Path.GetFileName(RelativePathName);

            // Directories to search: toplevel plus any subdirectories
            var vDirectories = new List<string> {DB.TeamSettings.PicturesFolder};
            vDirectories.AddRange(Directory.GetDirectories(DB.TeamSettings.PicturesFolder));

            foreach (var sDirectory in vDirectories)
            {
                var sPath = Path.Combine(sDirectory, sFileName);
                if (!File.Exists(sPath)) 
                    continue;
                RelativePathName = sPath.Substring(DB.TeamSettings.PicturesFolder.Length);
                return true;
            }

            // If we're here, we didn't find it
            return false;
        }
        #endregion
        #region Method: Bitmap GetBitmap(int nMaxDimension)
        public Bitmap GetBitmap(int nMaxDimension)
		{
            try
			{
                // Verify that the PathName exists, or try to find it otherwise
                if (!LocateThePicture())
                {
                    return GetBitmapNotFound(new Size(nMaxDimension, nMaxDimension / 3),
                        Path.GetFileName(FullPathName));
                }

				// Read in the file into a bitmap
				var bmpFromFile = new Bitmap(FullPathName);

                // Just return the bitmap unsized
                if (nMaxDimension <= 0)
                    return bmpFromFile;

				// Calculate width based on width being greater than height
				var nWidth  = Math.Min(bmpFromFile.Width, nMaxDimension);
				var nHeight = (nWidth * bmpFromFile.Height) / bmpFromFile.Width;

				// If the height was greater, then recalculate
				if (bmpFromFile.Height > bmpFromFile.Width)
				{
					nHeight = Math.Min(bmpFromFile.Height, nMaxDimension);
					nWidth  = (nHeight * bmpFromFile.Width) / bmpFromFile.Height;
				}

				// Create a new bitmap that will fit property
				var bmpResized = new Bitmap(bmpFromFile, nWidth, nHeight);
				return bmpResized;
			}
			catch
			{
                return GetBitmapNotFound(new Size(nMaxDimension, nMaxDimension/3), 
                    Path.GetFileName(FullPathName));
            }
		}
		#endregion
        #region Method: void Repair()
        void Repair()
            // This moves any pictures referenced so that they live under the ".Pictures"
            // folder, rather than hard coded on the disk. This makes them part of the repository.
            //
            // Thus where we have a slug of pictures that look like
            //   "C:\Graphics\cook\cot\co00600c.tif"
            // they now become
            //   "cook\cot\co00600c.tif"
            // and moved to .Pictures
        {
            // If the file exists, then it means we don't have a relative path name; but rather
            // an absolute name.
            if (!File.Exists(RelativePathName))
                return;
            if (!Path.IsPathRooted(RelativePathName))
                return;

            var sRoot = Path.GetPathRoot(RelativePathName);
            var sWithoutRoot = RelativePathName.Substring(sRoot.Length);

            if (sWithoutRoot.StartsWith("graphics\\", true, CultureInfo.InvariantCulture))
                sWithoutRoot = sWithoutRoot.Substring("graphics\\".Length);

            var sDestination = Path.Combine(DB.TeamSettings.PicturesFolder, sWithoutRoot);

            if (!File.Exists(sDestination))
            {
                try 
                {
                    var sDestinationFolder = Path.GetDirectoryName(sDestination);
                    if (!Directory.Exists(sDestinationFolder))
                        Directory.CreateDirectory(sDestinationFolder);
                    File.Copy(RelativePathName, sDestination); 
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }

            RelativePathName = sWithoutRoot;
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
            picture.RelativePathName = XmlDoc.GetAttrValue(nodePicture, c_sAttrPath, "");
            if (string.IsNullOrEmpty(picture.FullPathName))
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
        public override XmlNode SaveToOxesBook(XmlDoc oxes, XmlNode nodeBook)
        {
            var nodePicture = oxes.AddNode(nodeBook, c_sTagPicture);

            oxes.AddAttr(nodePicture, c_sAttrPath, RelativePathName);

            if (!string.IsNullOrEmpty(WordRtfInfo))
                oxes.AddAttr(nodePicture, c_sAttrRtf, WordRtfInfo);

            foreach (DRun run in Runs)
                run.SaveToOxesBook(oxes, nodePicture);

            return nodePicture;
        }
        #endregion

    }
}
