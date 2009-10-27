using System;
using System.IO;
using Chorus.merge;
using System.Linq;

namespace OurWordData.DataModel
{
    public class Merger
    {
        private const string OxesFileExtension = ".oxes";
        private const string OurWordTranslationFileExtension = ".otrans";
        private const string OurWordProjectFileExtension = ".owp";
        private const string OurWordSettingsFileExtension = ".owt";

        public static bool CanMergeFile(string pathToFile)
        {
            var recognizedExtensions = new[]
            {
                OxesFileExtension, 
                OurWordTranslationFileExtension,
                OurWordProjectFileExtension,
                OurWordSettingsFileExtension
            };

            var extension = Path.GetExtension(pathToFile).ToLower();

            return recognizedExtensions.Contains(extension);
        }

        public static void Do3WayMerge(MergeOrder mergeOrder)
        {
            if (mergeOrder == null) throw new ArgumentNullException("mergeOrder");

            var extension = Path.GetExtension(mergeOrder.pathToOurs).ToLower();

            switch (extension)
            {
                case OxesFileExtension:
                    // TODO
                    break;

                case OurWordTranslationFileExtension:
                    // TODO
                    break;

                case OurWordProjectFileExtension:
                    // TODO
                    break;

                case OurWordSettingsFileExtension:
                    // TODO
                    break;
            }


        }

    }
}
