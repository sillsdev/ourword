#region ***** Manifest.cs *****
/**********************************************************************************************
 * Project: Our Word!
 * File:    Manifest.cs
 * Author:  John Wimbish
 * Created: 28 Jan 2010
 * Purpose: List of files for the updater, their target destination, etc.
 * Legal:   Copyright (c) 2003-10, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Xml;
#endregion

namespace OurWordSetup.Data
{
    public class ManifestItem
    {
        // Identifying attributes ------------------------------------------------------------
        public string Filename { get; set; }
        public string Destination { get; set; }
        public long Length { get; set; }
        public string Hash { get; set; }

        // Possible Destinations -------------------------------------------------------------
        public const string c_sAppFolder = "{app}";
        public const string c_sMhDocumentsFolder = "{docs}";

        // I/O -------------------------------------------------------------------------------
        #region I/O Constants
        private const string c_sTag = "Item";
        private const string c_sAttrFilename = "filename";
        private const string c_sAttrDestination = "destination";
        private const string c_sAttrLength = "length";
        private const string c_sAttrHash = "hash";
        #endregion
        #region Method: XmlNode Save(XDoc, nodeParent)
        public XmlNode Save(XDoc doc, XmlNode nodeParent)
        {
            var node = doc.AddNode(nodeParent, c_sTag);

            doc.AddAttr(node, c_sAttrFilename, Filename);
            doc.AddAttr(node, c_sAttrDestination, Destination);
            doc.AddAttr(node, c_sAttrLength, Length);
            doc.AddAttr(node, c_sAttrHash, Hash);

            return node;
        }
        #endregion
        #region SMethod: ManifestItem Create(node)
        static public ManifestItem Create(XmlNode node)
        {
            if (node.Name != c_sTag)
                return null;

            var item = new ManifestItem
                {
                    Filename = XDoc.GetAttrValue(node, c_sAttrFilename, ""),
                    Destination = XDoc.GetAttrValue(node, c_sAttrDestination, c_sAppFolder),
                    Length = XDoc.GetAttrValue(node, c_sAttrLength, 0L),
                    Hash = XDoc.GetAttrValue(node, c_sAttrHash, "")
                };

            return item;
        }
        #endregion
    }

    public class Manifest : List<ManifestItem>
    {
        public int Major { get; protected set; }
        public int Minor { get; protected set; }
        public int Build { get; protected set; }
        #region VAttr{g/s}: Version Version
        public Version Version
        {
            get
            {
                return new Version(Major, Minor, Build);
            }
            set
            {
                Major = value.Major;
                Minor = value.Minor;
                Build = value.Build;
            }
        }
        #endregion

        // Scaffolding ----------------------------------------------------------------------
        public const string ManifestFileName = "OurWordManifest.xml";
        #region Constructor(sFilePath)
        public Manifest(string sFilePath)
        {
            Debug.Assert(!string.IsNullOrEmpty(sFilePath));
            m_sFilePath = sFilePath;
        }
        #endregion

        // I/O -------------------------------------------------------------------------------
        #region Attr{g}: string FilePath
        public string FilePath
        {
            get
            {
                Debug.Assert(!string.IsNullOrEmpty(m_sFilePath));
                return m_sFilePath;
            }
        }
        private string m_sFilePath;
        #endregion
        #region I/O Constants
        private const string c_sTag = "Manifest";
        private const string c_sMajor = "major";
        private const string c_sMinor = "minor";
        private const string c_sBuild = "build";
        #endregion
        #region Method: void Save()
        public void Save()
        {
            var doc = new XDoc(FilePath);

            var nodeManifest = doc.AddNode(null, c_sTag);
            doc.AddAttr(nodeManifest, c_sMajor, Major);
            doc.AddAttr(nodeManifest, c_sMinor, Minor);
            doc.AddAttr(nodeManifest, c_sBuild, Build);

            foreach (var item in this)
                item.Save(doc, nodeManifest);

            doc.Write();
        }
        #endregion
        #region Method: void Interpret(XmlDocument)
        void Interpret(XmlNode doc)
        {
            Clear();

            var nodeManifest = XDoc.FindNode(doc, c_sTag);

            Major = XDoc.GetAttrValue(nodeManifest, c_sMajor, 0);
            Minor = XDoc.GetAttrValue(nodeManifest, c_sMinor, 0);
            Build = XDoc.GetAttrValue(nodeManifest, c_sBuild, 0);

            foreach (XmlNode child in nodeManifest.ChildNodes)
            {
                var item = ManifestItem.Create(child);
                if (null != item)
                    Add(item);
            }
        }
        #endregion
        #region Method: void ReadXml(sXmlData)
        public void ReadXml(string sXmlData)
        {
            var doc = new XmlDocument();
            doc.LoadXml(sXmlData);

            Interpret(doc);
        }
        #endregion
        #region Method: string ReadFile()
        public string ReadFile()
        {
            var sXmlData = File.ReadAllText(FilePath);
            ReadXml(sXmlData);
            return sXmlData;
        }
        #endregion

        // Methods ---------------------------------------------------------------------------
        #region Method: bool ContainsFile(string sFilename)
        public bool ContainsFile(string sFilename)
        {
            return (Find(sFilename) != null);
        }
        #endregion
        #region Method: ManifestItem Find(string sFilename)
        public ManifestItem Find(string sFilename)
        {
            // If we're passed a path, extract its filename
            if (sFilename.Contains(Path.DirectorySeparatorChar.ToString()))
                sFilename = Path.GetFileName(sFilename);

            foreach (var item in this)
            {
                if (item.Filename == sFilename)
                    return item;
            }

            return null;
        }
        #endregion

        // Generate Manifest -----------------------------------------------------------------
        #region SMethod: void BuildFromFolderContents(string sFolder)
        public static void BuildFromFolderContents(string sFolder)
        {
            var sManifestPath = Path.Combine(sFolder, ManifestFileName);

            // If there is a manifest file in this folder, delete it, as we don't
            // want it to be in the new manifest
            if (File.Exists(sManifestPath))
                File.Delete(sManifestPath);

            var manifest = new Manifest(sManifestPath);

            // Collect and store uniquely identifying info about each file
            var vsFilePaths = Directory.GetFiles(sFolder);
            foreach (var sFilePath in vsFilePaths)
            {
                var info = new FileInfo(sFilePath);

                var item = new ManifestItem() 
                {
                    Filename = Path.GetFileName(sFilePath),
                    Destination = ManifestItem.c_sAppFolder,
                    Length = info.Length,
                    Hash = ComputeHash(sFilePath)
                };

                manifest.Add(item);
            }

            // Get the OurWord version info
            try
            {
                var sPathToOurWordExe = Path.Combine(sFolder, 
                    SetupManager.c_sOurWordApplication);

                var assembly = Assembly.LoadFrom(sPathToOurWordExe);
                var version = assembly.GetName().Version;
                manifest.Version = version;
            }
            catch (Exception)
            {
            }
        
            manifest.Save();
        }
        #endregion
        #region SMethod: string ComputeHash(string sPath)
        static string ComputeHash(string sPath)
        {
            try
            {
                var reader = new StreamReader(sPath);

                var hasher = SHA1.Create();
                var hash = hasher.ComputeHash(reader.BaseStream);

                reader.Close();

                // Convert bytes to hex
                var s = "";
                foreach (var b in hash)
                    s += b.ToString("X4");

                return s;
            }
            catch
            {
                return "UnavailableHash";
            }
        }
        #endregion

    }
}