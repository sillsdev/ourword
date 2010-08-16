/* TemporaryZoomPercent
 * 
 * Allows the user's zoom percent to be overridden. E.g., in printing, we always print at 100%,
 * no matter what the user has set.
 */

using System;

namespace OurWordData.DataModel.Membership
{
    public class TemporaryZoomPercent : IDisposable
    {
        private readonly User m_User;
        private readonly int m_nOldZoomPercent;

        #region Constructor(int nNewZoomPercent)
        public TemporaryZoomPercent(int nZoomPercent)
        {
            // Remember the old
            m_User = Users.Current;
            m_nOldZoomPercent = m_User.ZoomPercent;

            // Set to the temporary value
            Users.Current.ZoomPercent = nZoomPercent;
        }
        #endregion
        #region Method: void Dispose()
        public void Dispose()
        {
            m_User.ZoomPercent = m_nOldZoomPercent;
        }
        #endregion
    }
}
