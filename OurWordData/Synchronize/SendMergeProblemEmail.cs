#region ***** SendMergeProblemEmail.cs *****
/**********************************************************************************************
 * Project: Our Word!
 * File:    SendMergeProblemEmail.cs
 * Author:  John Wimbish
 * Created: 15 Dec 2009
 * Purpose: Sends email of merge problem, including the three files that would not merge
 * Legal:   Copyright (c) 2005-09, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
using System;
using System.IO;
using System.Reflection;
using Chorus.merge;
using Chorus.Utilities;
using Palaso.Email;
#endregion

namespace OurWordData.Synchronize
{
    public class SendMergeProblemEmail
    {
        // The temporary files we'll create
        private readonly string m_PathToOurs;
        private readonly string m_PathToTheirs;
        private readonly string m_PathToParent;

        private readonly string m_BaseFileName;
        private readonly Exception m_Exception;
        private readonly MergeOrder m_MergeOrder;

        // Public interface ------------------------------------------------------------------
        #region Constructor(MergeOrder, Exception)
        public SendMergeProblemEmail(MergeOrder mergeOrder, Exception e)
        {
            m_MergeOrder = mergeOrder;
            m_Exception = e;

            // The pathToOurs is our actual file, and thus has the filename we want to 
            // use; the other files are temporary files and gobbledegook names.
            m_BaseFileName = Path.GetFileNameWithoutExtension(mergeOrder.pathToOurs);

            // This places a copy of the file in the temporary folder, but with meaningful
            // filenames that we can thus attachh to the email.
            m_PathToOurs = CopyFileToTempLocation(mergeOrder.pathToOurs, "ours");
            m_PathToTheirs = CopyFileToTempLocation(mergeOrder.pathToTheirs, "theirs");
            m_PathToParent = CopyFileToTempLocation(mergeOrder.pathToCommonAncestor, "parent");
        }
        #endregion
        #region Method: void Do()
        public void Do()
        {
            var emailProvider = EmailProviderFactory.PreferredEmailProvider();
            var emailMessage = emailProvider.CreateMessage();
            PopulateMessage(emailMessage);
            emailMessage.Send(emailProvider);
            Cleanup();
        }
        #endregion

        // Implementation --------------------------------------------------------------------
        #region Method: string CopyFileToTempLocation(sSourcePath, sWhichOne)
        string CopyFileToTempLocation(string sSourcePath, string sWhichOne)
        {
            var temporaryFolder = Path.GetTempPath();
            var sFileName = string.Format("{0}.{1}.oxes", m_BaseFileName, sWhichOne);
            var sDestinationPath = Path.Combine(temporaryFolder, sFileName);

            if (File.Exists(sDestinationPath))
                File.Delete(sDestinationPath);

            File.Copy(sSourcePath, sDestinationPath);

            return sDestinationPath;
        }
        #endregion
        #region Method: void PopulateMessage(IEmailMessage emailMessage)
        void PopulateMessage(IEmailMessage emailMessage)
        {
            emailMessage.To.Add("John_Wimbish@tsco.org");
            emailMessage.Subject = "OurWord Merge Failure";

            var s = "Automated OurWord Merge Error Report" + Environment.NewLine + Environment.NewLine;
            s += AddRow("OurWord Version", GetOurWordExeVersion());
            s += AddRow("OurWord Data Version", GetOurWordDataVersion());
            s += AddRow("Base file", m_BaseFileName);
            s += AddRow("Exception message", m_Exception.Message);
            if (null != m_Exception.InnerException)
                s += AddRow("Inner Exception", m_Exception.InnerException.Message);
            s += AddRow("Our File", m_MergeOrder.pathToOurs);

            s += AddRow("Operating System", Environment.OSVersion.VersionString);
            s += AddRow("Computer Name", Environment.MachineName);
            s += AddRow("User Name", Environment.UserName);

            s += Environment.NewLine + Environment.NewLine;
            s += "--- Stack Trace ----------------------------------------------" + Environment.NewLine;
            s += m_Exception.StackTrace;

            emailMessage.Body = s;

            emailMessage.AttachmentFilePath.Add(m_PathToOurs);
            emailMessage.AttachmentFilePath.Add(m_PathToTheirs);
            emailMessage.AttachmentFilePath.Add(m_PathToParent);
        }
        #endregion
        #region Method: string AddRow(string sLabel, string sData)
        static string AddRow(string sLabel, string sData)
        {
            if (sLabel[sLabel.Length - 1] != ':')
                sLabel += ':';

            while (sLabel.Length < 20)
                sLabel += ' ';

            if (string.IsNullOrEmpty(sData))
                sData = "na";

            return sLabel + sData + Environment.NewLine;
        }
        #endregion
        #region Method: string GetOurWordExeVersion()
        static string GetOurWordExeVersion()
        {
            return Environment.GetEnvironmentVariable("OurWordExeVersion");
        }
        #endregion
        #region Method: string GetOurWordDataVersion()
        static string GetOurWordDataVersion()
        {
            return Environment.GetEnvironmentVariable("OurWordDataVersion");
        }
        #endregion
        #region Method: void Cleanup()
        void Cleanup()
            // Remove the files we created in the temporary folder
        {
            if (File.Exists(m_PathToOurs))
                File.Delete(m_PathToOurs);
            if (File.Exists(m_PathToTheirs))
                File.Delete(m_PathToTheirs);
            if (File.Exists(m_PathToParent))
                File.Delete(m_PathToParent);
        }
        #endregion
    }
}
