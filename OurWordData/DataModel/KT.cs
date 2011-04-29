using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using JWTools;

namespace OurWordData.DataModel
{

    public class Term 
    {
        public readonly List<string> Verses = new List<string>();

        public string Id;
        public string Gloss;

        #region Method: string GetBookName(string sVerse)
        static public string GetBookName(string sVerse)
        {
            var sBookNo = sVerse.Substring(0, 3);
            var iBookNo = Convert.ToInt32(sBookNo) - 1;  // make zero-based
            if (iBookNo >= 66)
                return "Apocropha";
            return BookNames.GetName(iBookNo);
        }
        #endregion
        static public int GetChapter(string sVerse)
        {
            var sChapterNo = sVerse.Substring(3, 3);
            return Convert.ToInt32(sChapterNo);
        }
        static public int GetVerse(string sVerse)
        {
            var sVerseNo = sVerse.Substring(6, 3);
            return Convert.ToInt32(sVerseNo);
        }

        #region SMethod: Term ReadNode(XmlNode node)
        static public Term ReadNode(XmlNode node)
        {
            if (!XmlDoc.IsNode(node, "Term"))
                return null;

            var kt = new Term {
                Id = XmlDoc.GetAttrValue(node, "Id")
            };

            var nodeGloss = XmlDoc.FindNode(node, "Gloss");
            if (null != nodeGloss)
                kt.Gloss = nodeGloss.InnerText;

            var nodeRefs = XmlDoc.FindNode(node, "References");
            if (null != nodeRefs)
            {
                foreach(XmlNode child in nodeRefs.ChildNodes)
                {
                    if (!XmlDoc.IsNode(child, "Verse")) 
                        continue;
                    var sVerse = child.InnerText;
                    if (GetBookName(sVerse) != "Luke")
                        continue;
                    kt.Verses.Add(sVerse);
                }
            }

            if (kt.Verses.Count == 0)
                return null;

            return kt;
        }
        #endregion
    }

    public class TermsDb : List<Term>
    {
        #region Method: void Read()
        public void Read()
        {
            var doc = new XmlDoc();
            doc.Load("C:\\Users\\JWimbish\\Desktop\\BiblicalTerms.xml");

            var list = XmlDoc.FindNode(doc, "BiblicalTermsList");

            foreach (XmlNode child in list.ChildNodes)
            {
                if (!XmlDoc.IsNode(child, "Term")) 
                    continue;
                var term = Term.ReadNode(child);
                if (null != term)
                    Add(term);
            }
        }
        #endregion

        static public void OnOffAnalysisForHiDef()
        {
            var db = new TermsDb();
            db.Read();


            const string sPath = "C:\\Users\\JWimbish\\Desktop\\LukeAnalysis.txt";
            using (var w = TextWriter.Synchronized(new StreamWriter(sPath, false)))
            {
                var sHeader = "Reference";
                foreach(var term in db)
                    sHeader += "," + term.Gloss;
                w.WriteLine(sHeader);

                foreach(DSection section in DB.TargetBook.Sections)
                {
                    var s = section.ReferenceSpan.DisplayName;

                    foreach(var term in db)
                    {
                        var count = 0;
                        foreach(var verse in term.Verses)
                        {
                            var nChapter = Term.GetChapter(verse);
                            var nVerse = Term.GetVerse(verse);
                            var r = new DReference(nChapter, nVerse);

                            if (section.ReferenceSpan.ContainsReference(r))
                                count++;
                        }

                        s += ",";
                        if (count > 0)
                            s += count;
                    }

                    w.WriteLine(s);
                }
            }


        }


    }
}
