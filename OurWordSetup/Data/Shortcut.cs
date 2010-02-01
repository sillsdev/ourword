using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using IWshRuntimeLibrary;

// For creating the desktop shortcut



namespace OurWordSetup.Data
{
    public class Shortcut
    {
        #region Attr{g}: string ApplicationName
        string ApplicationName
        {
            get
            {
                Debug.Assert(!string.IsNullOrEmpty(m_sApplicationName));

                // Should't be anything like a pathname
                Debug.Assert(!m_sApplicationName.Contains("."));
                Debug.Assert(!m_sApplicationName.Contains(Path.DirectorySeparatorChar.ToString()));

                return m_sApplicationName;
            }
        }
        private readonly string m_sApplicationName;
        #endregion
        #region VAttr{g}: string ShortcutPath
        string ShortcutPath
        {
            get
            {
                var desktopFolder = Environment.GetFolderPath(
                    Environment.SpecialFolder.DesktopDirectory);

                return Path.Combine(desktopFolder, ApplicationName + ".lnk");
            }
        }
        #endregion
        #region VAttr{g}: string ApplicationExecutablePath
        string ApplicationExecutablePath
        {
            get
            {
                var sBaseFolder = Environment.GetFolderPath(
                     Environment.SpecialFolder.LocalApplicationData);
                var sAppFolder = Path.Combine(sBaseFolder, ApplicationName);
                var sAppPath = Path.Combine(sAppFolder, ApplicationName + ".exe");
                return sAppPath;
            }
        }
        #endregion

        #region Constructor(sApplicationName)
        public Shortcut(string sApplicationName)
        {
            m_sApplicationName = sApplicationName;
        }
        #endregion

        public void CreateIfDoesntExist()
            // sAppName should be something like "OurWord". It is used to create the shortcut
            // filename (which is then OurWord.lnk), and to find the application
            // (e.g., "C:\....Local\OurWord\OurWord.exe".)
        {
            if (Exists)
                return;

            var shell = new WshShell();
            var shortcut = (IWshShortcut)shell.CreateShortcut(ShortcutPath);
            shortcut.TargetPath = ApplicationExecutablePath;
            shortcut.Save();
        }

        #region VAttr{g}: bool Exists
        bool Exists
        {
            get
            {
                return System.IO.File.Exists(ShortcutPath);
            }
        }
        #endregion
    }
}
