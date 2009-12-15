using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using OurWord.Edit;

namespace OurWord.Printing
{
    public class Page
    {
        // Major Page Divisions --------------------------------------------------------------
        #region Attr{g}: ERowOfColumns Header
        ERowOfColumns Header
        {
            get
            {
                Debug.Assert(null != m_Header);
                return m_Header;
            }
        }
        private readonly ERowOfColumns m_Header;
        #endregion

        // Body
        // Footnotes

        #region Attr{g}: RunningFooter RunningFooter
        RunningFooter RunningFooter
        {
            get
            {
                Debug.Assert(null != m_RunningFooter);
                return m_RunningFooter;
            }
        }
        private readonly RunningFooter m_RunningFooter;
        #endregion

        public Page(int nPageNumber)
        {
            m_Header = new ERowOfColumns(3);

            m_RunningFooter = new RunningFooter(nPageNumber);

        }



    }
}
