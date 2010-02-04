using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using OurWord.Utilities;

namespace OurWordTests.Setup
{
    [TestFixture]
    public class TShortcut
    {
        [Test]
        public void TCreate()
        {
            var shortcut = new Shortcut("TestApp");

            shortcut.DeleteIfExists();

            Assert.IsFalse(shortcut.Exists);

            shortcut.CreateIfDoesntExist();

            Assert.IsTrue(shortcut.Exists);

            // Cleanup
            shortcut.DeleteIfExists();
        }


    }
}
