/**********************************************************************************************
 * Project: OurWord!
 * File:    EInterlinear.cs
 * Author:  John Wimbish
 * Created: 27 Sep 2008
 * Purpose: An interlinear bundle within a paragraph, for the back translation
 * Legal:   Copyright (c) 2004-08, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
#region Using
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using JWTools;
using JWdb;
using OurWord.DataModel;
#endregion

namespace OurWord.Edit
{
    public class EInterlinear : OWPara.EBlock
    {
        // Attrs -----------------------------------------------------------------------------
        #region Attr{g/s}: string Meaning
        public string Meaning
        {
            get
            {
                return m_sMeaning;
            }
            set
            {
                m_sMeaning = value;
            }
        }
        string m_sMeaning;
        #endregion

        // Bundles ---------------------------------------------------------------------------
        #region CLASS: EBundle
        public class EBundle
        {
            #region Attr{g}: string Text
            public string Text
            {
                get
                {
                    return m_sText;
                }
            }
            string m_sText;
            #endregion
            #region Attr{g}: string Meaning
            public string Meaning
            {
                get
                {
                    return m_sMeaning;
                }
            }
            string m_sMeaning;
            #endregion

            // Scaffolding -------------------------------------------------------------------
            #region Constructor(sText, sMeaning)
            public EBundle(string _sText, string _sMeaning)
            {
                m_sText = _sText;
                m_sMeaning = _sMeaning;
            }
            #endregion
            #region Method: bool ContentEquals(EBundle item)
            public bool ContentEquals(EBundle item)
            {
                if (null == item)
                    return false;

                if (item.Text != Text)
                    return false;

                if (item.Meaning != Meaning)
                    return false;

                return true;
            }
            #endregion

            // I/O ---------------------------------------------------------------------------
            #region I/O CONSTANTS
            const string c_sTag = "B";
            const string c_sAttrText = "T";
            const string c_sAttrMeaning = "M";
            #endregion
            #region VAttr{g}: XElement ToXml
            public XElement ToXml
            {
                get
                {
                    XElement x = new XElement(c_sTag);
                    x.AddAttr(c_sAttrText, Text);
                    x.AddAttr(c_sAttrMeaning, Meaning);
                    return x;
                }
            }
            #endregion
            #region SMethod: EBundle CreateFromXml(XElement x)
            static public EBundle CreateFromXml(XElement x)
            {
                if (x.Tag != c_sTag)
                    return null;

                string sText = x.GetAttrValue(c_sAttrText, "");
                string sMeaning = x.GetAttrValue(c_sAttrMeaning, "");

                EBundle bundle = new EBundle(sText, sMeaning);

                return bundle;
            }
            #endregion
        }
        #endregion
        #region Attr{g}: EBundle[] Bundles
        EBundle[] Bundles
        {
            get
            {
                Debug.Assert(null != m_vBundles);
                return m_vBundles;
            }
        }
        EBundle[] m_vBundles;
        #endregion
        #region Method: void AppendBundle(EBundle bundle)
        public void AppendBundle(EBundle bundle)
        {
            EBundle[] v = new EBundle[Bundles.Length + 1];
            for (int i = 0; i < Bundles.Length; i++)
                v[i] = Bundles[i];
            v[Bundles.Length] = bundle;
            m_vBundles = v;
        }
        #endregion

        // VAttrs ----------------------------------------------------------------------------
        #region VAttr{g}: bool IsEmpty
        public bool IsEmpty
        {
            get
            {
                if (!string.IsNullOrEmpty(Meaning))
                    return false;

                if (Bundles.Length != 0)
                    return false;

                return true;
            }
        }
        #endregion

        // Scaffolding -----------------------------------------------------------------------
        #region Constructor(OWPara)
        public EInterlinear(OWPara _para)
            : base(_para, "")
        {
            m_vBundles = new EBundle[0];
        }
        #endregion
        #region OMethod: bool ContentEquals(EBlock block)
        public override bool ContentEquals(OWPara.EBlock block)
        {
            EInterlinear item = block as EInterlinear;
            if (null == item)
                return false;

            if (!base.ContentEquals(item))
                return false;

            if (item.Meaning != Meaning)
                return false;

            if (item.Bundles.Length != Bundles.Length)
                return false;

            for (int i = 0; i < Bundles.Length; i++)
            {
                if (!Bundles[i].ContentEquals(item.Bundles[i]))
                    return false;
            }

            return true;
        }
        #endregion

        // I/O -------------------------------------------------------------------------------
        #region I/O CONSTANTS
        const string c_sTag = "I";
        const string c_sAttrText = "T";
        const string c_sAttrMeaning = "M";
        const string c_sAttrGlue = "G";
        #endregion
        #region VAttr{g}: XElement ToXml
        public XElement ToXml
        {
            get
            {
                if (IsEmpty)
                    return null;

                XElement x = new XElement(c_sTag);

                x.AddAttr(c_sAttrText, Text);
                x.AddAttr(c_sAttrMeaning, Meaning);
                x.AddAttr(c_sAttrGlue, GlueToNext);

                foreach (EBundle bundle in Bundles)
                    x.AddSubItem(bundle.ToXml);

                return x;
            }
        }
        #endregion
        #region VAttr{g}: string XmlOneLiner
        public string XmlOneLiner
        {
            get
            {
                XElement x = ToXml;

                return (null == x) ? "" : x.OneLiner;
            }
        }
        #endregion
        #region SMethod: EInterlinear CreateFromXml(OWPara para, string s)
        static public EInterlinear CreateFromXml(OWPara para, string s)
        {
            // Parse into an element tree
            XElement[] vx = XElement.CreateFrom(s);
            if (null == vx || vx.Length != 1)
                return null;
            XElement x = vx[0];
            if (x.Tag != c_sTag)
                return null;

            // Retrieve the Meaning
            EInterlinear ei = new EInterlinear(para);
            ei.m_sMeaning = x.GetAttrValue(c_sAttrMeaning, "");
            ei.m_sText = x.GetAttrValue(c_sAttrText, "");
            ei.GlueToNext = x.GetAttrValue(c_sAttrGlue, false);

            // Retrieve the bundles
            foreach (XItem item in x.Items)
            {
                XElement xSub = item as XElement;
                if (null == xSub)
                    continue;

                EBundle bundle = EBundle.CreateFromXml(xSub);
                if (null != bundle)
                    ei.AppendBundle(bundle);
            }

            return ei;
        }
        #endregion

    }
}
