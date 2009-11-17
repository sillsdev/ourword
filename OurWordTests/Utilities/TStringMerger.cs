using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using OurWordData.Tools;

namespace OurWordTests.Utilities
{
    [TestFixture]
    public class TStringMerger
    {
        #region Test: Merge_OursChanged
        [Test]
        public void Merge_OursChanged()
        {
            var parent = "'Equipitu 'Ixaherisixi quevemüte'uta'aitüarie";
            var ours = "'Equipitu 'ixaherisixi quevemüte'uta'aitüarie";
            const string theirs = "'Equipitu 'Ixaherisixi quevemüte'uta'aitüarie";
            const string expected = "'Equipitu 'ixaherisixi quevemüte'uta'aitüarie";

            var success = StringMerger.Merge3Way(ref parent, ref ours, theirs);
            var actual = ours;
            Assert.AreEqual(expected, actual, "OursChanged");
            Assert.IsTrue(success, "Merge should have been successful");
        }
        #endregion
        #region Test: Merge_TheirsChanged
        [Test]
        public void Merge_TheirsChanged()
        {
            var parent = "'Equipitu 'Ixaherisixi quevemüte'uta'aitüarie";
            var ours = "'Equipitu 'Ixaherisixi quevemüte'uta'aitüarie";
            const string theirs = "'Equipitu 'ixaherisixi quevemüte'uta'aitüarie";
            const string expected = "'Equipitu 'ixaherisixi quevemüte'uta'aitüarie";

            var success = StringMerger.Merge3Way(ref parent, ref ours, theirs);
            var actual = ours;
            Assert.AreEqual(expected, actual, "TheirsChanged");
            Assert.IsTrue(success, "Merge should have been successful");
        }
        #endregion
        #region Test: Merge_ChildrenChangedIdentically
        [Test]
        public void Merge_ChildrenChangedIdentically()
        {
            var parent = "'Equipitu 'Ixaherisixi quevemüte'uta'aitüarie";
            var ours = "'Equipitu 'ixaherisixi quevemüte'uta'aitüarie";
            const string theirs = "'Equipitu 'ixaherisixi quevemüte'uta'aitüarie";
            const string expected = "'Equipitu 'ixaherisixi quevemüte'uta'aitüarie";

            var success = StringMerger.Merge3Way(ref parent, ref ours, theirs);
            var actual = ours;
            Assert.AreEqual(expected, actual, "TheirsChanged");
            Assert.IsTrue(success, "Merge should have been successful");
        }
        #endregion

        // Complicated merges
        #region Class: MergeDataSet
        private class MergeDataSet
        {
            private readonly string m_reference;
            private string m_parent;
            private string m_ours;
            private readonly string m_theirs;
            private readonly string m_expected;
            private readonly bool m_bCanCompletelyMerge;

            #region Constructor(reference, parent, ours, theirs)
            public MergeDataSet(string reference, bool bCanCompletelyMerge, 
                string parent, string ours, string theirs, string expected)
            {
                m_reference = reference;
                m_bCanCompletelyMerge = bCanCompletelyMerge;
                m_parent = parent;
                m_ours = ours;
                m_theirs = theirs;
                m_expected = expected;
            }
            #endregion
            #region Method: void TestMerge3Way()
            public void TestMerge3Way()
            {
                var success = StringMerger.Merge3Way(ref m_parent, ref m_ours, m_theirs);
                var actual = m_ours;
                Assert.AreEqual(m_expected, actual);
                Assert.AreEqual(m_bCanCompletelyMerge, success);
            }
            #endregion
        }
        #endregion
        #region Test: Merge_ChildrenChangedDifferently1
        [Test] public void Merge_ChildrenChangedDifferently1()
        {
            // Changes:
            // "[]memü" > "memü"  (both), then
            // "memü[ ]" > "memü" and "memü ", so can't reconsile
            var ds = new MergeDataSet("1:6-7", false,
                "Meniucuini Cuse, meta 'ivamama, yunaitü mücü nuivari[]memü[ ]'axüacai.",
                "Meniucuini Cuse, meta 'ivamama, yunaitü mücü nuivari memü 'axüacai.",
                "Meniucuini Cuse, meta 'ivamama, yunaitü mücü nuivari memü'axüacai.",
                "Meniucuini Cuse, meta 'ivamama, yunaitü mücü nuivari memü 'axüacai.");

            ds.TestMerge3Way();
        }
        #endregion
        #region Test: Merge_ChildrenChangedDifferently2
        [Test] public void Merge_ChildrenChangedDifferently2()
        {
            // Changes:
            // "mepü[ ]hücütücai" > " mepühücütücai"    (both children)
            // "tamamata."        > "tamamata,"         (theirs)
            // "xiüyarimama,"     > "xiüyarimama."      (theirs)
            var ds = new MergeDataSet("1:1-5", true,
                "Yunaitü me'inüaritü mepü[ ]hücütücai, haicateviyari heimana tamamata. Cacuvu xiüyarimama, Cuse 'Equipitu pucateiri.",
                "Yunaitü me'inüaritü mepühücütücai, haicateviyari heimana tamamata. Cacuvu xiüyarimama, Cuse 'Equipitu niucateitüniri.",
                "Yunaitü me'inüaritü mepühücütücai, haicateviyari heimana tamamata, Cacuvu xiüyarimama. Cuse 'Equipitu niucateitüniri.",
                "Yunaitü me'inüaritü mepühücütücai, haicateviyari heimana tamamata, Cacuvu xiüyarimama. Cuse 'Equipitu niucateitüniri.");

            ds.TestMerge3Way();
        }
        #endregion
        #region Test: Merge_ChildrenChangedDifferently3
        [Test] public void Merge_ChildrenChangedDifferently3()
        {
            var ds = new MergeDataSet("1:8-10", false,
                "Hicü 'Equipitu xevitü püta nanucaqueni ti'aitame Cuse müca[ ]hetima, quemütiuyuri.",
                "Hicü 'Equipitu xevitü püta nanucaqueni ti'aitame Cuse müca hetima, quemütiuyuri.",
                "Hicü 'Equipitu xevitü püta nanucaqueni ti'aitame Cuse mücahetima, quemütiuyuri.",
                "Hicü 'Equipitu xevitü püta nanucaqueni ti'aitame Cuse müca hetima, quemütiuyuri.");

            ds.TestMerge3Way();
        }
        #endregion
        #region Test: Merge_ChildrenChangedDifferently4
        [Test] public void Merge_ChildrenChangedDifferently4()
        {
            var ds = new MergeDataSet("2:1-4", true,
                "Xevitü niuyeicacaitüni Revi xiüyarieyatütü, mutineüq[iu]xü mana siere mieme mevitü.",
                "Xevitü niuyeicacaitüni Revi xiüyarieyatütü, mutineüquixü mana siere mieme mevitü.",
                "Xevitü niuyeicacaitüni Revi xiüyarieyatütü, quepaucua mutineüquixü mana siere mieme mevitüni.",
                "Xevitü niuyeicacaitüni Revi xiüyarieyatütü, quepaucua mutineüquixü mana siere mieme mevitüni.");

            ds.TestMerge3Way();
        }
        #endregion
        #region Test: Merge_ChildrenChangedDifferently5
        [Test] public void Merge_ChildrenChangedDifferently5()
        {
            var ds = new MergeDataSet("2:1-4", true,
                "Quepaucuari mücayüvecai mi'avietanicü, sicüiviti niutavevieni, xürüca nita[ ]virieni, 'asiparitu mame, mana nunusi catecaca hatesita neicueni, tupiriya sata nihüani hapa[]müye'ucai Niru hatesitayarisie.",
                "Quepaucuari mücayüvecai mi'avietanicü, sicüiviti niutavevieni, xürüca nitavirieni, 'asiparitu mame, mana nunusi catecaca hatesita neicueni, tupiriya sata nihüani hapa müye'ucai Niru hatesitayarisie.",
                "Quepaucuari mücayüvecai mi'avietanicü, sicüiviti niutavevieni, xürüca nitavirieni, 'asiparitu mame, mana nunusi catecaca hatesita neicueni, tupiriyasata nihüani hapa müye'ucai, Niru hatesitayarisie.",
                "Quepaucuari mücayüvecai mi'avietanicü, sicüiviti niutavevieni, xürüca nitavirieni, 'asiparitu mame, mana nunusi catecaca hatesita neicueni, tupiriyasata nihüani hapa müye'ucai, Niru hatesitayarisie.");

            ds.TestMerge3Way();
        }
        #endregion
        #region Test: Merge_ChildrenChangedDifferently6
        [Test] public void Merge_ChildrenChangedDifferently6()
        {
            var ds = new MergeDataSet("2:11-13", false,
                "Quepacua Muisexi 'amuyutama, yu'ivama nivarecuxeiya, niuniere que[v]emüte[ ]'uximatüariecai. [x]eime niuxeiya 'Equipitutanaca,[][']Epürayu teviyari tivayacacu.",
                "Quepacua Muisexi 'amuyutama, yu'ivama nivarecuxeiya, niuniere quememüte'uximatüariecai. Xeime niuxeiya 'Equipitutanaca, 'Epürayu teviyari tivayacacu.",
                "Quepacua Muisexi 'amuyutama, yu'ivama nivarecuxeiya, niuniere quememüte'uximatüariecai. Xeime niuxeiya 'equipitutanaca, hepürayu tivayacacu.",
                "Quepacua Muisexi 'amuyutama, yu'ivama nivarecuxeiya, niuniere quememüte'uximatüariecai. Xeime niuxeiya 'Equipitutanaca, 'Epürayu teviyari tivayacacu.");

            ds.TestMerge3Way();
        }
        #endregion
        #region Test: Merge_ChildrenChangedDifferently7
        [Test] public void Merge_ChildrenChangedDifferently7()
        {
            var ds = new MergeDataSet("2:11-13", false,
                "Hicü hanieretüyaca xevitü [vai]caheyeicacacü, mücü 'Equipitutanaca niumieni, niti'avieta xiecarita.",
                "Hicü hanieretüyaca xevitü havaicücareyeicacacü, mücü 'Equipitutanaca niumieni, niti'avieta xiecarita.",
                "Hicü hanieretüyaca xevitü havaicü careyeicacacü, mücü 'equipitutanaca niumieni, niti'avieta xiecarita.",
                "Hicü hanieretüyaca xevitü havaicücareyeicacacü, mücü 'Equipitutanaca niumieni, niti'avieta xiecarita.");

            ds.TestMerge3Way();
        }
        #endregion

    }
}
