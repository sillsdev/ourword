#region ***** TPrinter.cs *****
/**********************************************************************************************
 * File:    TPrinter.cs
 * Author:  John Wimbish
 * Created: 12 Jan 2010
 * Purpose: Tests the Printer class
 * Legal:   Copyright (c) 2004-10, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
using NUnit.Framework;
using OurWord.Printing;
#endregion

namespace OurWordTests.Printing
{
    [TestFixture]
    public class TPrinter : Printer
    {
        #region Test: TMakeReplacements
        [Test] public void TMakeReplacements()
        {
            m_bShouldMakeQuoteReplacements = true;
            Assert.AreEqual("kuna'”,", MakeQuoteReplacements("kuna'>>,"));
            Assert.AreEqual("“Au", MakeQuoteReplacements("<<Au"));
            Assert.AreEqual("“‘Au", MakeQuoteReplacements("<<<Au"));
            Assert.AreEqual("“‘Au’”", MakeQuoteReplacements("<<<Au>>>"));
            Assert.AreEqual("A‘u", MakeQuoteReplacements("A<u"));

            m_bShouldMakeQuoteReplacements = false;
            Assert.AreEqual("kuna'>>,", MakeQuoteReplacements("kuna'>>,"));
            Assert.AreEqual("<<Au", MakeQuoteReplacements("<<Au"));
            Assert.AreEqual("<<<Au", MakeQuoteReplacements("<<<Au"));
            Assert.AreEqual("<<<Au>>>", MakeQuoteReplacements("<<<Au>>>"));
            Assert.AreEqual("A<u", MakeQuoteReplacements("A<u"));
        }
        #endregion

    }
}
