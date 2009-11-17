using System;
using System.Collections.Generic;
using System.Text;

namespace OurWordData.Tools
{
    public class StringMerger
    {
        class Differences : List<Diff.Item>
        {
            #region Method: int[] GetDiffCodes(s)
            int[] GetDiffCodes(string s)
            {
                var vnCodes = new int[s.Length];
                for (var n = 0; n < s.Length; n++)
                    vnCodes[n] = (int)s[n];
                return vnCodes;
            }
            #endregion

            #region Constructor(s1, s2) 
            public Differences(string s1, string s2)
            {
                EmptyItem = new Diff.Item();

                var vn1 = GetDiffCodes(s1);
                var vn2 = GetDiffCodes(s2);

                var items = OurWordData.Diff.DiffInt(vn1, vn2);

                foreach(var item in items)
                    this.Add(item);

                CombineCloseItems();
            }
            #endregion

            #region Method: void CombineCloseItems()
            void CombineCloseItems()
            {
                const int c_DistanceBetweenItems = 2;

                // Move all of our members into a temporary one
                var v = new List<Diff.Item>();
                v.AddRange(this);
                this.Clear();

                while(v.Count > 1)
                {
                    var left = v[0];
                    var right = v[1];

                    // Combine them if they are close
                    if (left.StartB + left.deletedA + c_DistanceBetweenItems >= right.StartB)
                    {
                        var nActualDistanceBetween = right.StartA - (left.StartA + left.deletedA);
                        left.deletedA += (nActualDistanceBetween + right.deletedA);
                        left.insertedB += (nActualDistanceBetween + right.insertedB);
                        v.Remove(right);
                        v[0] = left;
                        continue;
                    }

                    // Otherwise, we're done with this one
                    Add(left);
                    v.Remove(left);
                }

                // Add the remaining one
                this.AddRange(v);
            }
            #endregion

            #region Method: void ToConsole(sTitle)
            public void ToConsole(string sTitle)
            {
                Console.WriteLine("---------------------------");
                Console.WriteLine(sTitle);
                foreach(var d in this)
                {
                    var s = string.Format("Diff: StartA={0} StartB={1} InsB={2} DelA={3}", 
                        d.StartA, d.StartB, d.insertedB, d.deletedA);
                    Console.WriteLine(s);
                }
            }
            #endregion

            static public Diff.Item EmptyItem;

            #region SMethod: string MakeChangeToSourceString(item, sA, sB)
            public static string MakeChangeToSourceString(Diff.Item item, string sA, string sB)
            {
                if (item.deletedA > 0)
                    sA = sA.Remove(item.StartA, item.deletedA);

                if (item.insertedB > 0)
                {
                    var sInsert = sB.Substring(item.StartB, item.insertedB);
                    sA = sA.Insert(item.StartA, sInsert);
                }

                return sA;
            }
            #endregion

            #region SMethod: bool IsNotEmpty(Diff.Item item)
            static public bool IsNotEmpty(Diff.Item item)
            {
                return !AreEqual(item, EmptyItem);
            }
            #endregion

            #region SMethod: AreEqual(item1, item2)
            static public bool AreEqual(Diff.Item i1, Diff.Item i2)
            {
                if (i1.StartA != i2.StartA)
                    return false;
                if (i1.StartB != i2.StartB)
                    return false;
                if (i1.insertedB != i2.insertedB)
                    return false;
                if (i1.deletedA != i2.deletedA)
                    return false;
                return true;
            }
            #endregion
            #region SMethod: IsEmpty(item)
            static public bool IsEmpty(Diff.Item item)
            {
                return AreEqual(EmptyItem, item);
            }
            #endregion
        }

        class DiffMatchTest
        {
            private Diff.Item m_ourItem;
            private Diff.Item m_theirItem;
            private readonly string m_sOurs;
            private readonly string m_sTheirs;

            #region Constructor(ourItem, theirItem, sOurs, sTheirs)
            public DiffMatchTest( Diff.Item ourItem, Diff.Item theirItem, string sOurs, string sTheirs)
            {
                m_ourItem = ourItem;
                m_theirItem = theirItem;
                m_sOurs = sOurs;
                m_sTheirs = sTheirs;
            }
            #endregion

            #region Method: bool GetAreMatchingChanges()
            public bool GetAreMatchingChanges()
            {
                if (m_ourItem.StartA != m_theirItem.StartA)
                    return false;
                if (m_ourItem.deletedA != m_theirItem.deletedA)
                    return false;
                if (m_ourItem.insertedB != m_theirItem.insertedB)
                    return false;

                if (m_ourItem.insertedB > 0)
                {
                    var ourInserted = m_sOurs.Substring(m_ourItem.StartB, m_ourItem.insertedB);
                    var theirInserted = m_sTheirs.Substring(m_theirItem.StartB, m_theirItem.insertedB);
                    if (ourInserted.CompareTo(theirInserted) != 0)
                        return false;
                }

                return true;
            }
            #endregion
            #region Method: bool GetAreMatchingMatchingOffsetChanges()
            public bool GetAreMatchingMatchingOffsetChanges()
            {
                /* The Diff method can return differences that are functionally the same,
                 * but have different numbers. Thus:
                 * 
                 * After   = niucateiri.
                 * Ours    = niucateitüniri.
                 *         |         BBBB
                 * Theirs  = niucateitüniri.
                 *         |        BBBB
                 *
                 * inserted into Ours   = itun
                 * inserted into Theirs = tuni
                 *
                 * Tests
                 * - First char of Ours = last char of Theirs
                 * - Parent(start + 0) = i
                 * - Parent(start + 4) = i
                 * 
                */

                // Insertions must be the same length
                if (m_ourItem.insertedB != m_theirItem.insertedB)
                    return false;
                int length = m_ourItem.insertedB;

                // Need at least 2 characters; one for the "same" test, one for the "middle" test
                if (length < 2)
                    return false;

                // Deletions must be the same length
                if (m_ourItem.deletedA != m_theirItem.deletedA)
                    return false;

                // The starts must differ by only one position
                if (m_ourItem.StartA != m_theirItem.StartA - 1 &&
                    m_ourItem.StartB != m_theirItem.StartA + 1)
                    return false;

                var ours = m_sOurs.Substring(m_ourItem.StartB, m_ourItem.insertedB);
                var theirs = m_sTheirs.Substring(m_theirItem.StartB, m_theirItem.insertedB);

                var earlier = (m_ourItem.StartA < m_theirItem.StartB) ? ours : theirs;
                var later = (m_ourItem.StartA < m_theirItem.StartB) ? theirs : ours;

                // First char of Earlier must equal final character of Later
                if (earlier[0] != later[ length - 1])
                    return false;

                // Middle characters must equal
                if (earlier.Substring(1) != later.Substring(0, length - 1))
                    return false;

                return true;
            }
            #endregion
        }

        #region Method: string ReconcileIdenticalChanges(string sParent, string sOurs, string sTheirs)
        static string ReconcileIdenticalChanges(string sParent, string sOurs, string sTheirs)
        {
            //Console.WriteLine(sParent);

            var bDifferencesFound = true;
            while (bDifferencesFound)
            {
                bDifferencesFound = false;
                var ourDiffs = new Differences(sParent, sOurs);
                var theirDiffs = new Differences(sParent, sTheirs);

                //ourDiffs.ToConsole("Ours");
                //theirDiffs.ToConsole("Theirs");
                
                foreach (var ourItem in ourDiffs)
                {
                    var theirItem = Differences.EmptyItem;
                    foreach (var item in theirDiffs)
                    {
                        var test = new DiffMatchTest(ourItem, item, sOurs, sTheirs);
                        if (test.GetAreMatchingChanges())
                        {
                            theirItem = item;
                            break;
                        }
                        if (test.GetAreMatchingMatchingOffsetChanges())
                        {
                            theirItem = item;
                            break;
                        }
                    }

                    if (Differences.IsNotEmpty(theirItem))
                    {
                        sParent = Differences.MakeChangeToSourceString(ourItem, sParent, sOurs);
                        bDifferencesFound = true;
                        //Console.WriteLine(sParent);
                        break;
                    }
                }
            }

            return sParent;
        }
        #endregion

        #region Method: bool GetHasDifferences(string sParent, string sOurs, string sTheirs)
        static bool GetHasDifferences(string sParent, string sOurs, string sTheirs)
        {
            var ourDiffs = new Differences(sParent, sOurs);
            var theirDiffs = new Differences(sParent, sTheirs);
            if(ourDiffs.Count > 0 || theirDiffs.Count > 0)
                return true;
            return false;
        }
        #endregion

        #region SMethod: bool ReconcileWhereSomeAreSame(string sParent, ref string sOurs, string sTheirs)
        static bool ReconcileWhereSomeAreSame(string sParent, ref string sOurs, string sTheirs)
        {
            // If Theirs and Parent are the same, then keep ours (whether it changed or not)
            if (sParent.CompareTo(sTheirs) == 0)
                return true;

            // If Ours and Parent are the same, then keep theirs (whether it changed or not)
            if (sParent.CompareTo(sOurs) == 0)
            {
                sOurs = sTheirs;
                return true;
            }

            // If here, both Ours and Theirs changed from the parent. Perhaps they made
            // identical change? (e.g., both people corrected an obvious typo)
            if (sOurs.CompareTo(sTheirs) == 0)
                return true;

            // If we're here, both changed, and they changed in different ways. So now
            // we need to go through diff-by-diff
            return false;
        }
        #endregion
        #region SMethod: bool Merge3Way(ref string sParent, ref string sOurs, string sTheirs)
        static public bool Merge3Way(ref string sParent, ref string sOurs, string sTheirs)
            // Returns true if successful, and no ConflictNote needed
        {
            // Quick option: where changes are minimul
            if(ReconcileWhereSomeAreSame(sParent, ref sOurs, sTheirs))
                return true;

            // Take care of any diff's that were made into both ours and theirs, then try again 
            sParent = ReconcileIdenticalChanges(sParent, sOurs, sTheirs);
            if (ReconcileWhereSomeAreSame(sParent, ref sOurs, sTheirs))
                return true;

            // Finally, take care of differences that we consider minor

            return false;
        }
        #endregion
    }
}
