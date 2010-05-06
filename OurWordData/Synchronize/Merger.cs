#region ***** Merger.cs *****
/**********************************************************************************************
 * Project: Our Word!
 * File:    Merger.cs
 * Author:  John Wimbish
 * Created: 27 Oct 2009
 * Purpose: Dispatcher for merging various types of OurWord files
 * Legal:   Copyright (c) 2005-09, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
using System;
using System.Collections.Generic;
using System.IO;
using Chorus.merge;
using OurWordData.DataModel;
#endregion

namespace OurWordData.Synchronize
{
    public class Merger
    {
        // Extensions we handle
        private const string OxesFileExtension = ".oxes";
        private const string OurWordTranslationFileExtension = ".otrans";
        private const string OurWordProjectFileExtension = ".owp";
        private const string OurWordSettingsFileExtension = ".owt";

        private static readonly List<string> s_OurWordFileExtensions = new List<string>
        {
            OxesFileExtension, 
            OurWordTranslationFileExtension,
            OurWordProjectFileExtension,
            OurWordSettingsFileExtension
        };

        #region static IEnumerable<string> GetExtensionsOfKnownTextFileTypes()
        public static IEnumerable<string> GetExtensionsOfKnownTextFileTypes()
        {
            return s_OurWordFileExtensions;
        }
        #endregion

        #region static bool CanMergeFile(sPathToFile)
        public static bool CanMergeFile(string pathToFile)
        {
            var extension = Path.GetExtension(pathToFile).ToLower();

            return s_OurWordFileExtensions.Contains(extension);
        }
        #endregion

        #region static void Do3WayMerge(MergeOrder)
        public static void Do3WayMerge(MergeOrder mergeOrder)
        {
            if (mergeOrder == null) 
                throw new ArgumentNullException("mergeOrder");
            //Debug.Fail("Wanna debug OurWord's Do3WayMerge?");

            // This hook allows a merge to fail for the benefit of unit testing. 
            // The test in question seeks to roll-back a failed merge. We want to
            // fail outside of the try block so that we don't send the error email.
            if (mergeOrder.pathToOurs.Contains(".MustFail."))
                throw new Exception("Merge Failed for Unit Testing");

            var extension = Path.GetExtension(mergeOrder.pathToOurs).ToLower();
            try
            {
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
            catch (Exception e)
            {
                ReportError(mergeOrder, e);
            }
        }
        #endregion

        #region SMethod: void ReportError(MergeOrder mergeOrder, Exception e)
        static private void ReportError(MergeOrder mergeOrder, Exception e)
        {
            var dialog = new ReportMergeErrorDlg();
            dialog.ShowDialog();
            if (!dialog.CanSendEmail)
                return;

            (new SendMergeProblemEmail(mergeOrder, e)).Do();
        }
        #endregion
    }
}
