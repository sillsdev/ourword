#region ***** Program.cs *****
/**********************************************************************************************
 * Project: Our Word!
 * File:    Program.cs
 * Author:  John Wimbish
 * Created: 28 Jan 2010
 * Purpose: Main method; switches off to the various modes of execution:
 *            - FullSetup: a complete download install of OurWord
 *            - Generate: create the Manifest file
 *            - Update: complete a CheckForUpdates process
 *            
 *          Some of the methods in the SetupManager are also called directly from within
 *          OurWord, thus treating this EXE as a DLL.
 *          
 * Legal:   Copyright (c) 2003-10, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
using System;
using System.Windows.Forms;
using OurWordSetup.Data;
using OurWordSetup.UI;

#endregion

namespace OurWordSetup
{
    static class Program
    {
        // Executable paths (actions)
        private const string c_sActionFullSetup = "CompleteSetup";
        public const string c_sActionFinishUpdate = "Update";
        private const string c_sActionGenerateManifest = "Generate";

        [STAThread]
        static void Main(string[] vArgs)
            // The main entry point for the application.
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var sActionDesired = c_sActionFullSetup;
            var sGenerateManifestInFolder = "";

            // Collect the input parameters
            for(var i=0; i<vArgs.Length; i++)
            {
                switch (vArgs[i].ToLower())
                {
                    // The caller wants to generate the manifest, based on the files
                    // in the folder which will be the following arg
                    case c_sActionGenerateManifest:
                    case "-g":
                        sActionDesired = c_sActionGenerateManifest;
                        if (i < vArgs.Length - 1)
                            sGenerateManifestInFolder = vArgs[i + 1];
                        break;

                    // The caller wants to update an existing install
                    case c_sActionFinishUpdate:
                    case "-u":
                        sActionDesired = c_sActionFinishUpdate;
                        break;
                }
            }

            // Do the requested action
            switch (sActionDesired)
            {
                case c_sActionGenerateManifest:
                    {
                        // Build a manifest file from the files in the indicated folder
                        Manifest.BuildFromFolderContents(sGenerateManifestInFolder);
                    }
                    return;

                case c_sActionFinishUpdate:
                    {
                        // Copy files from Download to App folder, restart OurWord
                        var setup = new SetupManager(null);
                        setup.FinishUpdate();
                    }
                    break;

                case c_sActionFullSetup:
                    {
                        // Do a complete, 100%, setup
                        Application.Run(new DlgFullSetup());
                    }
                    break;
            }
        }

    }
}
