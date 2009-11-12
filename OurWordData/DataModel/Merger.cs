#region ***** Merger.cs *****
/**********************************************************************************************
 * Project: Our Word!
 * File:    Merger.cs
 * Author:  John Wimbish
 * Created: 27 Oct 2009
 * Purpose: Dispatcher for merging various types of OurWord files
 * Legal:   Copyright (c) 2005-09, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
#region Using
using System;
using System.Collections.Generic;
using System.IO;
using Chorus.merge;
#endregion
#endregion

namespace OurWordData.DataModel
{
    public class Merger
    {
        // Extensions we handle
        private const string OxesFileExtension = ".oxes";
        private const string OurWordTranslationFileExtension = ".otrans";
        private const string OurWordProjectFileExtension = ".owp";
        private const string OurWordSettingsFileExtension = ".owt";

        private static readonly List<string> OurWordFileExtensions = new List<string>
        {
            OxesFileExtension, 
            OurWordTranslationFileExtension,
            OurWordProjectFileExtension,
            OurWordSettingsFileExtension
        };

        #region static IEnumerable<string> GetExtensionsOfKnownTextFileTypes()
        public static IEnumerable<string> GetExtensionsOfKnownTextFileTypes()
        {
            return OurWordFileExtensions;
        }
        #endregion

        #region static bool CanMergeFile(sPathToFile)
        public static bool CanMergeFile(string pathToFile)
        {
            var extension = Path.GetExtension(pathToFile).ToLower();

            return OurWordFileExtensions.Contains(extension);
        }
        #endregion

        #region static void Do3WayMerge(MergeOrder)
        public static void Do3WayMerge(MergeOrder mergeOrder)
        {
            if (mergeOrder == null) throw new ArgumentNullException("mergeOrder");

            var extension = Path.GetExtension(mergeOrder.pathToOurs).ToLower();

            switch (extension)
            {
                case OxesFileExtension:
                    DBook.Merge(mergeOrder);
                    break;

                case OurWordTranslationFileExtension:
                    DTranslation.Merge(mergeOrder);
                    break;

                case OurWordProjectFileExtension:
                    // TODO
                    break;

                case OurWordSettingsFileExtension:
                    // TODO
                    break;
            }
        }
        #endregion
    }
}
