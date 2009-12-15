using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using OurWordData.DataModel;

namespace OurWordTests.DataModel
{
    [TestFixture]
    public class TPWord : PWord
    {
        #region Constructor()
        public TPWord()
            : base("hello", null, null)
        {
        }
        #endregion

        #region Test: TMakeSubstitutions
        [Test] public void TMakeSubstitutions()
        {
            PWord.ShouldMakeReplacements = true;
            Assert.AreEqual("kuna'”,", MakeReplacements("kuna'>>,"));
            Assert.AreEqual("“Au", MakeReplacements("<<Au"));
            Assert.AreEqual("“‘Au", MakeReplacements("<<<Au"));
            Assert.AreEqual("“‘Au’”", MakeReplacements("<<<Au>>>"));
            Assert.AreEqual("A‘u", MakeReplacements("A<u"));

            PWord.ShouldMakeReplacements = false;
            Assert.AreEqual("kuna'>>,", MakeReplacements("kuna'>>,"));
            Assert.AreEqual("<<Au", MakeReplacements("<<Au"));
            Assert.AreEqual("<<<Au", MakeReplacements("<<<Au"));
            Assert.AreEqual("<<<Au>>>", MakeReplacements("<<<Au>>>"));
            Assert.AreEqual("A<u", MakeReplacements("A<u"));

        }
        #endregion
    }
}
