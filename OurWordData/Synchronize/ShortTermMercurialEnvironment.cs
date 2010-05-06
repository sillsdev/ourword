#region ***** ShortTermMercurialEnvironment.cs *****
/**********************************************************************************************
 * Project: Our Word!
 * File:    ShortTermMercurialEnvironment.cs
 * Author:  John Wimbish
 * Created: 27 Oct 2009
 * Purpose: Sets environment variables for finding the local copy of Mercurial
 * Legal:   Copyright (c) 2005-10, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
using System;
using System.Collections.Generic;
using System.IO;
using JWTools;
#endregion

namespace OurWordData.Synchronize
{
    public class ShortTermMercurialEnvironment : IDisposable
    {
        class TemporaryEnvironmentVariable
        {
            private readonly string m_sVariableName;
            private readonly string m_sOldValue;

            #region Constructor(sVariableName, sNewValue)
            public TemporaryEnvironmentVariable(string sVariableName, string sNewValue)
            {
                m_sVariableName = sVariableName;

                m_sOldValue = Environment.GetEnvironmentVariable(m_sVariableName);

                Environment.SetEnvironmentVariable(m_sVariableName, sNewValue);
            }
            #endregion
            #region Method: void RestoreOldValue()
            public void RestoreOldValue()
            {
                Environment.SetEnvironmentVariable(m_sVariableName, m_sOldValue);
            }
            #endregion
        }
        private readonly List<TemporaryEnvironmentVariable> m_Variables;

        // Locations of interest
        #region SAttr{g}: string FolderContainingMercurial
        static public string FolderContainingMercurial
        {
            get
            {
                return Path.Combine(JWU.GetLocalApplicationDataFolder("OurWord"), "mercurial");
            }
        }
        #endregion
        #region SAttr{g}: string FullPathToMercurialExe
        static public string FullPathToMercurialExe
        {
            get
            {
                var sPathToMercurial = Path.Combine(FolderContainingMercurial, "hg.exe");

                return Repository.SurroundWithQuotes(sPathToMercurial);
            }
        }
        #endregion


        // Constructor, IDisposable
        #region Constructor()
        public ShortTermMercurialEnvironment()
        {
            // The Path must include Mercurial's location
            const string c_sPath = "PATH";
            var sOldPath = Environment.GetEnvironmentVariable(c_sPath);
            var sNewPath = (string.IsNullOrEmpty(sOldPath)) ?
                FolderContainingMercurial :
                string.Format("{0};{1}", sOldPath, FolderContainingMercurial);
            
            m_Variables = new List<TemporaryEnvironmentVariable>
            {
                new TemporaryEnvironmentVariable("PATH", sNewPath),
                new TemporaryEnvironmentVariable("HG", FullPathToMercurialExe)
            };
        }
        #endregion
        #region Method: void Dispose()
        public virtual void Dispose()
        {
            foreach(var variable in m_Variables)
                variable.RestoreOldValue();
        }
        #endregion
    } 


}
