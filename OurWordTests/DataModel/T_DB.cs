using System;
using NUnit.Framework;
using OurWordData.DataModel;

namespace OurWordTests.DataModel
{
    [TestFixture]
    public class T_DB
    {
        #region Test: GetYearThisAssemblyWasCompiled
        [Test] public void GetYearThisAssemblyWasCompiled()
        {
            // We'll assume that if we're running the test, then we compiled it this year.
            var thisYear = DateTime.Now.Year;

            Assert.AreEqual(thisYear, DB.GetYearThisAssemblyWasCompiled());
        }
        #endregion
    }
}
