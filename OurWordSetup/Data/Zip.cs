#region ***** Zip.cs *****
/**********************************************************************************************
 * Project: Our Word!
 * File:    Zip.cs
 * Author:  John Wimbish
 * Created: 2 Feb 2010
 * Purpose: Encapsulates operations on the ICSharpCode.SharpZipLib zip library
 * Legal:   Copyright (c) 2003-10, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
#endregion

namespace OurWordSetup.Data
{
    public class Zip
    {
        // Zip Prefixes ----------------------------------------------------------------------
        // The prefix on the zip's filename determines where we'll extract it to during the
        // install process. All zip files must have one of these prefixes.
        const string c_sDestinationLocalApps = "app";
        const string c_sDestinationMyDocuments = "mydocs";
        const string c_sDestinationLanguageData = "langdata";
        const string c_sDestinationTestFolder = "ziptest";

        // Attrs -----------------------------------------------------------------------------
        #region Attr{g}: string ZipPath
        private string ZipPath
        {
            get
            {
                Debug.Assert(!string.IsNullOrEmpty(m_sZipPath));
                return m_sZipPath;
            }
        }
        private readonly string m_sZipPath;
        #endregion
        #region VAttr{g}: string OutputFolder
        public string OutputFolder
        {
            get
            {
                // Make sure its a zip file we're processing
                var sExtension = Path.GetExtension(ZipPath);
                if (string.IsNullOrEmpty(sExtension) || sExtension.ToLower() != ".zip")
                    throw new Exception("File is not a zip file: " + ZipPath);

                // Get its parts
                var sFileName = Path.GetFileNameWithoutExtension(ZipPath);

                var sParts = sFileName.Split(new[] { '.' });
                if (sParts.Length < 2)
                    throw new Exception("Invalid zip file basename [" +
                        sFileName + "], it must have at least two parts.");

                // First part must be a recognized destination. See the "Zip Prefixes" consts
                var sBasePart = sParts[0].ToLower();
                string sOutputFolder;
                switch (sBasePart)
                {
                    case c_sDestinationLocalApps:
                        sOutputFolder = SetupManager.ApplicationsFolder;
                        break;
                    case c_sDestinationMyDocuments:
                        sOutputFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                        break;
                    case c_sDestinationLanguageData:
                        // Must be the same as in ClusterListView.cs
                        sOutputFolder = Path.Combine(
                           Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                           "Language Data");
                        break;
                    case c_sDestinationTestFolder:
                        sOutputFolder = Path.Combine(
                            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                            "ziptest");
                        break;
                    default:
                        throw new Exception("Zip file does not start with a recognized prefix.");
                }

                // Every subsequent part, except for the last part, adds a new folder
                var i = 1;
                while(i < sParts.Length - 1)
                {
                    var sFolder = sParts[i++];
                    sOutputFolder = Path.Combine(sOutputFolder, sFolder);
                }

                return sOutputFolder;
            }
        }
        #endregion

        // Scaffolding -----------------------------------------------------------------------
        #region Constructor(sZipPath)
        public Zip(string sZipPath)
        {
            m_sZipPath = sZipPath;
        }
        #endregion

        // Operations ------------------------------------------------------------------------
        #region SAttr{g}: bool IsZipFile(string sZipPath)
        static public bool IsZipFile(string sZipPath)
        {
            var sExtension = Path.GetExtension(sZipPath);
            return (sExtension.ToLower() == ".zip");
        }
        #endregion
        #region Method: List<string> GetFileNames()
        public List<string> GetFileNames()
        {
            var v = new List<string>();
            var zipInput = new ZipInputStream(File.OpenRead(ZipPath));

            ZipEntry item;
            while ((item = zipInput.GetNextEntry()) != null)
            {
                if (!item.IsFile || string.IsNullOrEmpty(item.Name))
                    continue;

                // Convert the Unix forward slash to whatever our OS wants
                var sName = item.Name.Replace("/", Path.DirectorySeparatorChar.ToString());

                v.Add(sName);
            }

            zipInput.Close();

            return v;
        }
        #endregion
        #region Method: List<string> GetFullPathNames()
        public List<string> GetFullPathNames()
        {
            var vsFileNames = GetFileNames();

            var sFolder = Path.GetDirectoryName(ZipPath);

            return vsFileNames.Select(s => Path.Combine(sFolder, s)).ToList();
        }
        #endregion
        #region Method: void Extract()
        public void Extract()
        {
            Extract(OutputFolder);
        }
        #endregion
        #region Method: void Extract(sOutputFolder)
        public void Extract(string sOutputFolder)
        {
            var zipInput = new ZipInputStream(File.OpenRead(ZipPath));

            ZipEntry item;
            while ((item = zipInput.GetNextEntry()) != null)
            {
                if (!item.IsFile || string.IsNullOrEmpty(item.Name))
                    continue;

                // Convert the Unix forward slash to whatever our OS wants
                var sName = item.Name.Replace("/", Path.DirectorySeparatorChar.ToString());

                var sPath = Path.Combine(sOutputFolder, sName);

                var sFolder = Path.GetDirectoryName(sPath);
                if (!Directory.Exists(sFolder))
                    Directory.CreateDirectory(sFolder);

                var writer = File.Create(sPath);
                var data = new byte[2048];
                while (true)
                {
                    var size = zipInput.Read(data, 0, data.Length);
                    if (size > 0)
                        writer.Write(data, 0, size);
                    else
                        break;
                }
                writer.Close();
            }

            zipInput.Close();
        }
        #endregion


        #region Method: string MakeUnixRelativePath(string sRootPath, string sTargetPath)
        static public string MakeUnixRelativePath(string sRootPath, string sTargetPath)
        {
            if (!sRootPath.EndsWith(Path.DirectorySeparatorChar.ToString()))
                sRootPath += Path.DirectorySeparatorChar;

            Debug.Assert(Path.IsPathRooted(sTargetPath));
            var sZipCompatible = sTargetPath.Substring(sRootPath.Length).Replace("\\", "/");
            return sZipCompatible;
        }
        #endregion
        #region Method: void Create(string sRootPath, List<string> vsFullPaths)
        public void Create(string sRootPath, IEnumerable<string> vsFullPaths)
        {
            var buffer = new byte[4096];
            using (var zipOutput = new ZipOutputStream(File.Create(ZipPath)))
            {
                zipOutput.SetLevel(9); // maximum compression
               
                foreach(var sPath in vsFullPaths)
                {
                    var sUnixRelativePath = MakeUnixRelativePath(sRootPath, sPath);
                    var entry = new ZipEntry(sUnixRelativePath);
                    zipOutput.PutNextEntry(entry);

                    using (var fs = File.OpenRead(sPath))
                    {
                        StreamUtils.Copy(fs, zipOutput, buffer);
                    }

                }
            }
        }
        #endregion
        #region Method: List<string> GetFullPathNamesOfAdditionalFiles(Zip zipSuperset)
        public List<string> GetFullPathNamesOfAdditionalFiles(Zip zipSuperset)
            // Returns the pathnames of files in the other zip file that do not exist
            // in our zip file
        {
            var vsOurPaths = GetFullPathNames();
            var vsSupersetPaths = zipSuperset.GetFullPathNames();

            return vsSupersetPaths.Where(s => !vsOurPaths.Contains(s)).ToList();
        }
        #endregion
    }
}
