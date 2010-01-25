#region ***** ParagraphStyle.cs *****
/**********************************************************************************************
 * Project: Our Word!
 * File:    ParagraphStyle.cs
 * Author:  John Wimbish
 * Created: 7 Dec 2009
 * Purpose: Attributes for displaying/printing paragraphs
 * Legal:   Copyright (c) 2005-09, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
using System;
using System.Diagnostics;
using System.Xml;
using JWTools;
#endregion

namespace OurWordData.Styles
{
    public class ParagraphStyle : CharacterStyle
    {
        // Content Attrs ---------------------------------------------------------------------
        #region Attr{g/s}: int PointsBefore
        public int PointsBefore
        {
            get
            {
                return m_ptPointsBefore;
            }
            set
            {
                if (m_ptPointsBefore == value)
                    return;
                m_ptPointsBefore = value;
                StyleSheet.DeclareDirty();
            }
        }
        private int m_ptPointsBefore;
        #endregion
        #region Attr{g/s}: int PointsAfter
        public int PointsAfter
        {
            get
            {
                return m_ptPointsAfter;
            }
            set
            {
                if (m_ptPointsAfter == value)
                    return;
                m_ptPointsAfter = value;
                StyleSheet.DeclareDirty();
            }
        }
        private int m_ptPointsAfter = 6;
        #endregion
        #region Attr{g/s}: double FirstLineIndentInches
        public double FirstLineIndentInches
        {
            get
            {
                return m_dFirstLineIndentInches;
            }
            set
            {
                if (m_dFirstLineIndentInches == value)
                    return;
                m_dFirstLineIndentInches = value;
                StyleSheet.DeclareDirty();
            }
        }
        private double m_dFirstLineIndentInches;
        #endregion
        #region Attr{g/s}: double LeftMarginInches
        public double LeftMarginInches
        {
            get
            {
                return m_dLeftMarginInches;
            }
            set
            {
                if (m_dLeftMarginInches == value)
                    return;
                m_dLeftMarginInches = value;
                StyleSheet.DeclareDirty();
            }
        }
        private double m_dLeftMarginInches;
        #endregion
        #region Attr{g/s}: double RightMarginInches
        public double RightMarginInches
        {
            get
            {
                return m_dRightMarginInches;
            }
            set
            {
                if (m_dRightMarginInches == value)
                    return;
                m_dRightMarginInches = value;
                StyleSheet.DeclareDirty();
            }
        }
        private double m_dRightMarginInches;
        #endregion
        #region Attr{g/s}: bool KeepWithNextParagraph
        public bool KeepWithNextParagraph
        {
            get
            {
                return m_bKeepWithNextParagraph;
            }
            set
            {
                if (m_bKeepWithNextParagraph == value)
                    return;
                m_bKeepWithNextParagraph = value;
                StyleSheet.DeclareDirty();
            }
        }
        private bool m_bKeepWithNextParagraph;
        #endregion
        #region Attr{g/s}: bool Bulleted
        public bool Bulleted
        {
            get
            {
                return m_bBulleted;
            }
            set
            {
                if (m_bBulleted == value)
                    return;
                m_bBulleted = value;
                StyleSheet.DeclareDirty();
            }
        }
        private bool m_bBulleted;
        #endregion
        public enum Align { Left = 0, Right, Centered, Justified };
        #region Attr{g/s}: AlignType Alignment
        public Align Alignment
        {
            get
            {
                return m_Alignment;
            }
            set
            {
                if (m_Alignment == value)
                    return;
                m_Alignment = value;
                StyleSheet.DeclareDirty();
            }
        }
        private Align m_Alignment = Align.Left;
        #endregion

        // Derived Attrs ---------------------------------------------------------------------
        #region VAttr{g/s}: book IsLeft - paragraph is left-aligned
        public bool IsLeft
        {
            get
            {
                return Alignment == Align.Left;
            }
            set
            {
                Alignment = Align.Left;
            }
        }
        #endregion
        #region VAttr{g/s}: book IsRight - paragraph is right-aligned
        public bool IsRight
        {
            get
            {
                return Alignment == Align.Right;
            }
            set
            {
                Alignment = Align.Right;
            }
        }
        #endregion
        #region VAttr{g/s}: book IsCentered - paragraph is centered
        public bool IsCentered
        {
            get
            {
                return Alignment == Align.Centered;
            }
            set
            {
                Alignment = Align.Centered;
            }
        }
        #endregion
        #region VAttr{g/s}: book IsJustified - paragraph is justified
        public bool IsJustified
        {
            get
            {
                return Alignment == Align.Justified;
            }
            set
            {
                Alignment = Align.Justified;
            }
        }
        #endregion

        // Mapping Information ---------------------------------------------------------------
        public class Mapping
        {
            public readonly string UsfmMarker;
            public string ToolboxMarker { get; set;}

            // Scripture is, e.g., p, q1, q2; but isn't things like section heads, cross
            // references, picture captions.
            public bool IsScripture { get; set; }

            // These styles are for things in the UI such as the Literate Settings paragraphs
            public bool IsUserInterface { get; set; }

            // Lines 1,2,3 and CenteredLine
            public bool IsPoetry { get; set; }

            public Mapping(string sUsfmMarker)
            {
                UsfmMarker = sUsfmMarker;

                // In most cases, the markers are identical
                ToolboxMarker = sUsfmMarker;
            }
        }
        #region Attr{g/s}: Mapping Map
        public Mapping Map
        {
            get
            {
                Debug.Assert(null != m_Mapping);
                return m_Mapping;
            }
            set
            {
                m_Mapping = value;
            }
        }
        private Mapping m_Mapping;
        #endregion

        // Scaffolding -----------------------------------------------------------------------
        #region Constructor(sStyleName)
        public ParagraphStyle(string sStyleName)
            : base(sStyleName)
        {
        }
        #endregion

        // I/O & Merge -----------------------------------------------------------------------
        #region OAttr{g}: Tag
        private const string c_sTag = "ParagraphStyle";
        protected override string Tag
        {
            get
            {
                return c_sTag;
            }
        }
        #endregion
        #region I/O Constants
        private const string c_sAttrPointsBefore = "Before";
        private const string c_sAttrPointsAfter = "After";
        private const string c_sAttrFirstLineIndentInches = "FirstLineIndent";
        private const string c_sAttrLeftMarginInches = "LeftMargin";
        private const string c_sAttrRightMarginInches = "RightMargin";
        private const string c_sAttrKeepWithNextParagraph = "KeepWithNext";
        private const string c_sAttrBulleted= "Bulleted";
        private const string c_sAttrAlignment = "Alignment";
        #endregion
        #region OMethod: void SaveContent(XmlDoc, nodeStyle)
        protected override void SaveContent(XmlDoc doc, XmlNode node)
        {
            base.SaveContent(doc, node);

            doc.AddAttr(node, c_sAttrPointsBefore, PointsBefore);
            doc.AddAttr(node, c_sAttrPointsAfter, PointsAfter);
            doc.AddAttr(node, c_sAttrFirstLineIndentInches, FirstLineIndentInches);
            doc.AddAttr(node, c_sAttrLeftMarginInches, LeftMarginInches);
            doc.AddAttr(node, c_sAttrRightMarginInches, RightMarginInches);
            doc.AddAttr(node, c_sAttrKeepWithNextParagraph, KeepWithNextParagraph);
            doc.AddAttr(node, c_sAttrBulleted, Bulleted);
            doc.AddAttr(node, c_sAttrAlignment, Alignment.ToString());
        }
        #endregion
        #region OMethod: void ReadContent(XmlNode node)
        override protected void ReadContent(XmlNode node)
        {
            base.ReadContent(node);

            // Most attributes
            PointsBefore = XmlDoc.GetAttrValue(node, c_sAttrPointsBefore, 0);
            PointsAfter = XmlDoc.GetAttrValue(node, c_sAttrPointsAfter, 6);
            FirstLineIndentInches = XmlDoc.GetAttrValue(node, c_sAttrFirstLineIndentInches, 0.0);
            LeftMarginInches = XmlDoc.GetAttrValue(node, c_sAttrLeftMarginInches, 0.0);
            RightMarginInches = XmlDoc.GetAttrValue(node, c_sAttrRightMarginInches, 0.0);
            KeepWithNextParagraph = XmlDoc.GetAttrValue(node, c_sAttrKeepWithNextParagraph, false);
            Bulleted = XmlDoc.GetAttrValue(node, c_sAttrBulleted, false);

            // Attempt caseless resolution of Alignment; default to Left if it fails
            try
            {
                Alignment = (Align)Enum.Parse(
                    typeof(Align),
                    XmlDoc.GetAttrValue(node, c_sAttrAlignment, Align.Left.ToString()),
                    true);
            }
            catch (Exception)
            {
                Alignment = Align.Left;
            }
        }
        #endregion
        #region SMethod: new ParagraphStyle Create(node)
        static new public ParagraphStyle Create(XmlNode node)
        {
            if (node.Name != c_sTag)
                return null;

            // Abort if we have an invalid style name
            var sStyleName = GetStyleNameFromXml(node);
            if (string.IsNullOrEmpty(sStyleName))
                return null;

            // Create the style with its attributes
            var style = new ParagraphStyle(sStyleName);
            style.ReadContent(node);
            return style;
        }
        #endregion
        #region Method: void Merge(ParagraphStyle parent, ParagraphStyle theirs)
        public void Merge(ParagraphStyle parent, ParagraphStyle theirs)
        {
            // Take care of the CharacterStyle stuff
            base.Merge(parent, theirs);

            // Our stuff. Algorithm: We keep theirs iff they differ from ours and ours is
            // unchanged from the parent. Otherwise we always keep ours.
            if (PointsBefore != theirs.PointsBefore && PointsBefore == parent.PointsBefore)
                PointsBefore = theirs.PointsBefore;

            if (PointsAfter != theirs.PointsAfter && PointsAfter == parent.PointsAfter)
                PointsAfter = theirs.PointsAfter;

            if (FirstLineIndentInches != theirs.FirstLineIndentInches && FirstLineIndentInches == parent.FirstLineIndentInches)
                FirstLineIndentInches = theirs.FirstLineIndentInches;

            if (LeftMarginInches != theirs.LeftMarginInches && LeftMarginInches == parent.LeftMarginInches)
                LeftMarginInches = theirs.LeftMarginInches;

            if (RightMarginInches != theirs.RightMarginInches && RightMarginInches == parent.RightMarginInches)
                RightMarginInches = theirs.RightMarginInches;

            if (KeepWithNextParagraph != theirs.KeepWithNextParagraph && KeepWithNextParagraph == parent.KeepWithNextParagraph)
                KeepWithNextParagraph = theirs.KeepWithNextParagraph;

            if (Bulleted != theirs.Bulleted && Bulleted == parent.Bulleted)
                Bulleted = theirs.Bulleted;

            if (Alignment != theirs.Alignment && Alignment == parent.Alignment)
                Alignment = theirs.Alignment;
        }
        #endregion
    }
}
