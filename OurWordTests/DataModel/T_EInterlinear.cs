/**********************************************************************************************
 * Project: OurWord! - Tests
 * File:    T_Xml.cs
 * Author:  John Wimbish
 * Created: 14 May 2008
 * Purpose: Tests the xml classes
 * Legal:   Copyright (c) 2004-08, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
#region Using
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

using NUnit.Framework;

using JWTools;
using JWdb;

using OurWord;
using OurWord.DataModel;
using OurWord.Dialogs;
using OurWord.Edit;
using OurWord.View;
#endregion

namespace OurWordTests.DataModel
{
    [TestFixture] public class T_EInterlinear
    {
        // Attrs from Setup ------------------------------------------------------------------
        #region Attr{g}: OWWindow Wnd
        OWWindow Wnd
        {
            get
            {
                return m_Window;
            }
        }
        OWWindow m_Window;
        #endregion
        Form m_Form;
        #region Attr{g}: DSection Section
        DSection Section
        {
            get
            {
                return m_section;
            }
        }
        DSection m_section;
        #endregion
        #region VAttr{g}: JWritingSystem WSVernacular
        JWritingSystem WSVernacular
        {
            get
            {
                return G.Project.TargetTranslation.WritingSystemVernacular;
            }
        }
        #endregion
        OWPara[] m_vParas;

        #region Method: void SetupSection()
        void SetupSection()
        {
            // Section Head
            DParagraph p = new DParagraph(G.Project.TargetTranslation);
            p.StyleAbbrev = DStyleSheet.c_StyleSectionTitle;
            p.AddRun(DText.CreateSimple(
                "Tuhan Yesus kasi umpama so'al biji yang paling kici",
                "Lord Yesus gives a *parable about the seed that is very small"));
            Section.Paragraphs.Append(p);

            // Cross Reference
            p = new DParagraph(G.Project.TargetTranslation);
            p.StyleAbbrev = DStyleSheet.c_StyleCrossReference;
            p.AddRun(DText.CreateSimple(
                "(Mateos 13:31-32, 34; Lukas 13:18-19)"));
            Section.Paragraphs.Append(p);

            // Paragraph starting at verse 30
            p = new DParagraph(G.Project.TargetTranslation);
            p.StyleAbbrev = DStyleSheet.c_StyleAbbrevNormal;
            p.AddRun(DVerse.Create("30"));
            p.AddRun(DText.CreateSimple(
                "Abis itu, Yesus omong sambung, bilang, <<Beta tamba satu umpama lai, bagini: " +
                    "bosong bisa banding Tuhan Allah pung orang dong. Dong mulai deng sadiki sa, " +
                    "ma tamba lama, dong tamba banya.",
                "After that, Yesus spoke further2, saying, <<I add one more *parable again like " +
                    "this: you-pl can compare God's *kingdom2 (people). They begin with just a " +
                    "few, but adding long.time, they add many."));
            p.AddRun(DVerse.Create("31"));
            p.AddRun(DText.CreateSimple(
                "Itu tingka ke biji yang paling kici ana.",
                "That is like a seed that is very teeny-tiny."));
            p.AddRun(DVerse.Create("32"));
            p.AddRun(DText.CreateSimple(
                "Ma kalo kotong su tanam itu biji, dia idop jadi pohon yang paling bésar. Ju " +
                    "burung-burung datang cari sombar ko basarang di situ.>>",
                "But (contrary to expectation) if we-inc have planted that seed, it lives to " +
                    "become the biggest tree. And birds come seek shade and.so make nests there.>>"));
            Section.Paragraphs.Append(p);

            // Paragraph starting at verse 33
            p = new DParagraph(G.Project.TargetTranslation);
            p.StyleAbbrev = DStyleSheet.c_StyleAbbrevNormal;
            p.AddRun(DVerse.Create("33"));
            p.AddRun(DText.CreateSimple(
                "Itu, Yesus pung cara ajar sang dong iko dong pung mangarti.",
                "That [was] Yesus' way to teach them according to their understanding."));
            p.AddRun(DVerse.Create("34"));
            p.AddRun(DText.CreateSimple(
                "Dia biasa pake umpama kalo ajar sang orang dong. Ma kalo deng Dia pung ana " +
                    "bua dong, Dia kasi tau itu umpama pung arti samua.",
                "He usually used *parables if he taught them. But if {Ma kalo} with His " +
                    "*disciples, He told them the full meaning of that parable."));
            Section.Paragraphs.Append(p);
        }
        #endregion
        #region SetUp
        [SetUp] public void Setup()
        {
            // Unit Test Setup
            JWU.NUnit_Setup();

            // Application and Project initialization
            OurWordMain.App = new OurWordMain();
            OurWordMain.Project = new DProject();
            G.Project.TeamSettings = new DTeamSettings();
            G.TeamSettings.InitializeFactoryStyleSheet();
            G.Project.DisplayName = "Project";
            G.Project.TargetTranslation = new DTranslation("Test Translation", "Latin", "Latin");
            DBook book = new DBook("MRK", "");
            G.Project.TargetTranslation.AddBook(book);
            G.URStack.Clear();
            m_section = new DSection(1);
            book.Sections.Append(m_section);

            // Window with a form
            m_Window = new OWWindow("TestWindow", 1);
            m_Form = new Form();
            m_Form.Name = "TestForm";
            m_Form.Controls.Add(m_Window);

            // Create the section data
            SetupSection();

            // Set up the window contents
            m_vParas = new OWPara[Section.Paragraphs.Count];
            int i = 0;
            foreach (DParagraph p in Section.Paragraphs)
            {
                Wnd.StartNewRow();
                OWPara para = new OWPara(Wnd, 
                    Wnd.LastRow.SubItems[0] as EContainer,
                    WSVernacular,
                    p.Style, 
                    p, 
                    Color.Wheat,
                    OWPara.Flags.IsEditable);
                m_vParas[i] = para;
                i++;

                Wnd.AddParagraph(0, para);
            }
            Wnd.LoadData();
        }
        #endregion
        #region TearDown
        [TearDown] public void TearDown()
        {
            OurWordMain.Project = null;
            m_Form.Dispose();
            m_Form = null;
        }
        #endregion

        // Tests -----------------------------------------------------------------------------
        #region Test: XmlIO
        [Test] public void XmlIO()
        {
            // Create an empty EInterlinear
            EInterlinear ei = new EInterlinear(m_vParas[2]);

            // Set it up with values
            ei.Text = "Tuhan Allah pung orang dong";
            ei.Meaning = "God's kingdom";
            ei.AppendBundle(new EInterlinear.EBundle("Tuhan", "Lord"));
            ei.AppendBundle(new EInterlinear.EBundle("Allah", "God"));
            ei.AppendBundle(new EInterlinear.EBundle("pung", "has"));
            ei.AppendBundle(new EInterlinear.EBundle("orang", "person"));
            ei.AppendBundle(new EInterlinear.EBundle("dong", "3P"));

            // Convert to XML one-liner
            string sOneLiner = ei.XmlOneLiner;
            // Console.WriteLine(sOneLiner);
            Assert.AreEqual("<I T=\"Tuhan Allah pung orang dong\" M=\"God's kingdom\" G=\"false\">" +
                "<B T=\"Tuhan\" M=\"Lord\"/>" +
                "<B T=\"Allah\" M=\"God\"/>" +
                "<B T=\"pung\" M=\"has\"/>" +
                "<B T=\"orang\" M=\"person\"/>" +
                "<B T=\"dong\" M=\"3P\"/></I>",
                sOneLiner);

            // Parse the one-liner into another EInterlinear
            EInterlinear ei2 = EInterlinear.CreateFromXml(m_vParas[2], sOneLiner);
            Assert.IsTrue(ei2.ContentEquals(ei));
        }
        #endregion
    }
}
