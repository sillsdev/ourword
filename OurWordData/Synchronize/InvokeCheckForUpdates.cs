using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace OurWordData.Synchronize
{
    public class InvokeCheckForUpdates
    {
        private const string c_sSetupFile = "SetupOurWord.exe";
        private const string c_sCheckMethodName = "CheckForUpdates";
        private const string c_sCheckMethodClass = "OurWordSetup.Data.SetupManager";

        // Attrs -----------------------------------------------------------------------------
        #region Attr{s}: bool QuietMode
        public bool QuietMode { private get; set; }
        #endregion

        // Helper Methods --------------------------------------------------------------------
        #region SAttr{g}: string PathOfSetupExe
        static string PathOfSetupExe
            // Returns, e.g., "C:\Users\JSmith\AppData\Local\OurWord\SetupOurWord.exe"
        {
            get
            {
                var assembly = Assembly.GetExecutingAssembly();
                Debug.Assert(null != assembly);

                var sPathToExe = assembly.Location;
                Debug.Assert(!string.IsNullOrEmpty(sPathToExe));

                var sFolderContainingExe = Path.GetDirectoryName(sPathToExe);
                return Path.Combine(sFolderContainingExe, c_sSetupFile);
            }
        }
        #endregion
        #region SMethod: MethodInfo RetrieveRemoteMethod()
        static MethodInfo RetrieveRemoteMethod()
        // Retrieves the method "OurWordSetup.Data.SetupManager.CheckForUpdates()"
        {
            var setupAssembly = Assembly.LoadFrom(PathOfSetupExe);

            var setupType = setupAssembly.GetType(c_sCheckMethodClass);

            return setupType.GetMethod(c_sCheckMethodName);
        }
        #endregion

        // Public Interface ------------------------------------------------------------------
        public enum Result { UserAborted, Error, NoUpdatedNeeded, UpdateLaunched }

        #region SMethod: Result Do(parentForm)
        public Result Do(Form parentForm)
        {
            // If we're on my developer's machine, don't do the check, because we're not 
            // running from a proper installation.
            if (PathOfSetupExe.ToLowerInvariant().Contains("debug"))
                return Result.NoUpdatedNeeded;

            // Retrieve the Setup DLL
            if (!File.Exists(PathOfSetupExe))
            {
                var sError = string.Format("{0} not found in the OurWord installation folder.", 
                    c_sSetupFile);
                Console.WriteLine(sError);
                throw new Exception(sError);
            }

            // Retrieve the method within the DLL
            var method = RetrieveRemoteMethod();
            if (null == method)
            {
                var sError = string.Format("Unable to locate the {0} method in {1}", 
                    c_sCheckMethodName, c_sCheckMethodClass);
                Console.WriteLine(sError);
                throw new Exception(sError);
            }

            // Invoke the remote method
            var nResult = (int)method.Invoke(null, new object[] { parentForm, QuietMode });

            if (nResult == -2)
                return Result.UserAborted;
            if (nResult == -1)
                return Result.Error;
            if (nResult == 1)
                return Result.UpdateLaunched;
            return Result.NoUpdatedNeeded;

        }
        #endregion
    }
}
