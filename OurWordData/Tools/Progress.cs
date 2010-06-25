using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;

namespace OurWordData
{
    public interface IProgressIndicator
    {
        void Start(string sMessage, int nCount);
        void Step();
        void End();
    }

    #region Class: NullProgress : IProgressIndicator
    public class NullProgress : IProgressIndicator
    {
        public void Start(string sMessage, int nCount)
        {
        }
        public void Step()
        {
        }
        public void End()
        {
        }
    }
    #endregion


}
