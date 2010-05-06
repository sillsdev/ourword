using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace OurWord.Utilities
{
    public class InvokeOurWordSetup
    {
        private const string c_sSetupFile = "SetupOurWord.exe";
        private const string c_sCheckMethodName = "CheckForUpdates";
        private const string c_sCheckMethodClass = "OurWordSetup.Data.SetupManager";

        #region SAttr{g}: string PathOfSetupExe
        static string PathOfSetupExe
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
        {
            var setupAssembly = Assembly.LoadFrom(PathOfSetupExe);

            var setupType = setupAssembly.GetType(c_sCheckMethodClass);

            return setupType.GetMethod(c_sCheckMethodName);
        }
        #endregion

        #region SMethod: bool CheckForUpdates(parentForm, bInformUserIfThereWereNoUpdates)
        static public bool CheckForUpdates(Form parentForm, bool bInformUserIfThereWereNoUpdates)
        {
            if (!File.Exists(PathOfSetupExe))
            {
                var sError = string.Format("{0} not found in the OurWord installation folder.", 
                    c_sSetupFile);
                Console.WriteLine(sError);
                throw new Exception(sError);
            }

            var method = RetrieveRemoteMethod();
            if (null == method)
            {
                var sError = string.Format("Unable to locate the {0} method in {1}", 
                    c_sCheckMethodName, c_sCheckMethodClass);
                Console.WriteLine(sError);
                throw new Exception(sError);
            }

            return (bool)method.Invoke(null, new object[] { parentForm, bInformUserIfThereWereNoUpdates });
        }
        #endregion
    }
}
