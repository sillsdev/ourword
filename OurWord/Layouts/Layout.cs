#region ***** Layout.cs *****
/**********************************************************************************************
 * Project: Our Word!
 * File:    Layouts\Layout.cs
 * Author:  John Wimbish
 * Created: 10 Aug 2009
 * Purpose: Common functionality for the various views
 * Legal:   Copyright (c) 2005-09, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
#region Using
using System;
using System.Diagnostics;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.IO;
using System.Text;

using OurWord.Edit;
using OurWord.Layouts;
using OurWordData;
using OurWordData.DataModel;
using JWTools;
using OurWordData.Styles;

#endregion
#endregion

namespace OurWord.Layouts
{
    public class WLayout : OWWindow
    {
        #region Constructor(sName, cColumnCout)
        protected WLayout(string sName, int cColumnCount)
            : base(sName, cColumnCount)
        {
        }
        #endregion
        #region VirtAttr{g}: string LayoutName
        public virtual string LayoutName
        {
            get
            {
                Debug.Assert(false, "LayoutName must be overwritten in the subclass");
                return "Layout";
            }
        }
        #endregion

        #region Method: OWPara CreateFootnotePara(...)
        protected OWPara CreateFootnotePara(DFootnote footnote, 
            WritingSystem ws,
            Color backColor, 
            OWPara.Flags flags)
        {
            return new OWFootnotePara(footnote, backColor, flags);
        }
        #endregion

        const int c_xMaxPictureWidth = 300;
        #region Method: Bitmap GetPicture(DParagraph p)
        static public Bitmap GetPicture(DParagraph p)
        {
            var pict = p as DPicture;
            return null != pict ? pict.GetBitmap(c_xMaxPictureWidth) : null;
        }
        #endregion

        // Paragraph Creation
        #region Method: OWPara CreateUneditableVernacularPara(DParagraph)
        protected OWPara CreateUneditableVernacularPara(DParagraph p)
        {
            return new OWPara(
                p.Translation.WritingSystemVernacular,
                p.Style,
                p,
                BackColor,
                OWPara.Flags.None);
        }
        #endregion

        protected const int c_nFootnoteSeparatorWidth = 60;

        // Synchronize Front and Target paragraphs -------------------------------------------
        protected class SynchronizedSection
        {
            // Input Attrs
            #region Attr{g}: DSection FrontSection
            DSection FrontSection
            {
                get
                {
                    Debug.Assert(null != m_FrontSection);
                    return m_FrontSection;
                }
            }
            DSection m_FrontSection;
            #endregion
            #region Attr{g}: DSection TargetSection
            DSection TargetSection
            {
                get
                {
                    Debug.Assert(null != m_TargetSection);
                    return m_TargetSection;
                }
            }
            DSection m_TargetSection;
            #endregion

            // Paragraph groups
            #region CLASS: ParagraphGroup
            public class ParagraphGroup
            {
                // Attrs
                #region Attr{g}: List<DParagraph> FrontParagraphs
                public List<DParagraph> FrontParagraphs
                {
                    get
                    {
                        Debug.Assert(null != m_vFrontParagraphs);
                        return m_vFrontParagraphs;
                    }
                }
                List<DParagraph> m_vFrontParagraphs;
                #endregion
                #region Attr{g}: List<DParagraph> TargetParagraphs
                public List<DParagraph> TargetParagraphs
                {
                    get
                    {
                        Debug.Assert(null != m_vTargetParagraphs);
                        return m_vTargetParagraphs;
                    }
                }
                List<DParagraph> m_vTargetParagraphs;
                #endregion

                // Misc
                #region VAttr{g}: bool IsFootnoteGroup
                public bool IsFootnoteGroup
                {
                    get
                    {
                        if (TargetParagraphs.Count == 0)
                            return false;
                        if (null == TargetParagraphs[0] as DFootnote)
                            return false;
                        return true;
                    }
                }
                #endregion

                // Side-by-syde conversion
                #region VAttr{g}: bool CanSideBySide
                public bool CanSideBySide
                {
                    get
                    {
                        // If the two "blocks" do not have the same count of paragraphs, then
                        // they cannot appear in the side-by-side mode
                        if (FrontParagraphs.Count != TargetParagraphs.Count)
                            return false;
                        if (FrontParagraphs.Count == 0 || TargetParagraphs.Count == 0)
                            return false;

                        // Footnotes: just check the type
                        if (DB.FrontSection.Paragraphs[0] as DFootnote != null)
                        {
                            for (int i = 0; i < FrontParagraphs.Count; i++)
                            {
                                if ((FrontParagraphs[i] as DFootnote).NoteType !=
                                    (TargetParagraphs[i] as DFootnote).NoteType)
                                {
                                    return false;
                                }
                            }
                            return true;
                        }

                        // If the individual paragraphs to not have the same 
                        // 1. style, 
                        // 2. verse reference
                        // then they cannot appear in side-by-side mode.
                        for (int i = 0; i < FrontParagraphs.Count; i++)
                        {
                            var pFront = FrontParagraphs[i];
                            var pTarget = TargetParagraphs[i];

                            if (pFront.Style != pTarget.Style)
                                return false;
                            if (!pFront.IsSameReferenceAs(pTarget))
                                return false;
                        }

                        return true;
                    }
                }
                #endregion
                #region Method: List<ParagraphGroup> ConvertToSideBySide()
                public List<ParagraphGroup> ConvertToSideBySide()
                {
                    // If we can't align them side-by-side, then we can't split them further
                    if (!CanSideBySide)
                        return new List<ParagraphGroup>() { this };

                    var vGroup = new List<ParagraphGroup>();

                    for (int i = 0; i < FrontParagraphs.Count; i++)
                    {
                        var group = new ParagraphGroup(FrontParagraphs[i], TargetParagraphs[i]);
                        vGroup.Add(group);
                    }

                    return vGroup;
                }
                #endregion

                // Constructors
                #region Constructor()
                public ParagraphGroup()
                {
                    m_vFrontParagraphs = new List<DParagraph>();
                    m_vTargetParagraphs = new List<DParagraph>();
                }
                #endregion
                #region Constructor(pFront, pTarget)
                public ParagraphGroup(DParagraph pFront, DParagraph pTarget)
                    : this()
                {
                    FrontParagraphs.Add(pFront);
                    TargetParagraphs.Add(pTarget);
                }
                #endregion
            }
            #endregion
            #region Attr{g}: List<ParagraphGroup> ParagraphGroups
            public List<ParagraphGroup> ParagraphGroups
            {
                get
                {
                    Debug.Assert(null != m_vParagraphGroups);
                    return m_vParagraphGroups;
                }
            }
            List<ParagraphGroup> m_vParagraphGroups;
            #endregion
            #region Attr{g}: List<ParagraphGroup> FootnoteGroups
            public List<ParagraphGroup> FootnoteGroups
            {
                get
                {
                    Debug.Assert(null != m_vFootnoteGroups);
                    return m_vFootnoteGroups;
                }
            }
            List<ParagraphGroup> m_vFootnoteGroups;
            #endregion
            #region VAttr{g}: List<ParagraphGroup> AllGroups
            public List<ParagraphGroup> AllGroups
            {
                get
                {
                    var v = new List<ParagraphGroup>();
                    v.AddRange(ParagraphGroups);
                    v.AddRange(FootnoteGroups);
                    return v;
                }
            }
            #endregion

            // Scaffolding
            #region Method: void _AddExtraBlankParagraphIfNeeded(i, vParagraphs)
            void _AddExtraBlankParagraphIfNeeded(int i, JOwnSeq<DParagraph> vParagraphs)
            {
                if (i == vParagraphs.Count)
                {
                    var pNew = new DParagraph(StyleSheet.Paragraph);
                    pNew.AddedByCluster = true;
                    vParagraphs.Append(pNew);
                }
            }
            #endregion
            #region Method: int _CountMatchingParagraphTypes(int iStart, JOwnSeq vParagraphs)
            int _CountMatchingParagraphTypes(int iStart, JOwnSeq<DParagraph> vParagraphs)
            {
                if (iStart >= vParagraphs.Count)
                    return 0;

                // Determine what the first paragraph's type is
                DParagraph p = vParagraphs[iStart] as DParagraph;
                bool bIsFootnote = ((p as DFootnote) != null);
                bool bIsPicture = ((p as DPicture) != null);
                bool bIsParagraph = (!bIsFootnote && !bIsPicture);

                // We'll count how many subsequent paragraphs match it; starting
                // with "1" as that is the initial paragraph
                int c = 1;

                // Loop until we run into a non-match
                while (iStart + c < vParagraphs.Count)
                {
                    // Determine the type of the text paragraph
                    DParagraph pTest = vParagraphs[iStart + c] as DParagraph;
                    bool bTestIsFootnote = ((pTest as DFootnote) != null);
                    bool bTestIsPicture = ((pTest as DPicture) != null);
                    bool bTestIsParagraph = (!bTestIsFootnote && !bTestIsPicture);

                    // Break if "p" is a picture but "pTest" is not.
                    if (bIsPicture && !bTestIsPicture)
                        break;

                    // Break if "p" is a footnote but "pTest" is not.
                    if (bIsFootnote && !bTestIsFootnote)
                        break;

                    // Break if "p" is a paragraph but "pTest" is not.
                    if (bIsParagraph && !bTestIsParagraph)
                        break;

                    c++;
                }

                return c;
            }
            #endregion
            #region Constructor(Front, Target)
            public SynchronizedSection(DSection Front, DSection Target)
            {
                m_FrontSection = Front;
                m_TargetSection = Target;

                // 1. Break the section down into major parts, as defined by using
                // pictures as boundaries.
                var vMajorParts = new List<ParagraphGroup>();
                int iFront = 0;
                int iTarget = 0;
                while (iFront < FrontSection.Paragraphs.Count ||
                       iTarget < TargetSection.Paragraphs.Count)
                {
                    // Make sure we have at least one paragraph in this group
                    _AddExtraBlankParagraphIfNeeded(iFront, FrontSection.Paragraphs);
                    _AddExtraBlankParagraphIfNeeded(iTarget, TargetSection.Paragraphs);

                    // Get a count of the number of paragraphs that are the same type
                    // (Picture, paragraph)
                    int cFront = _CountMatchingParagraphTypes(iFront, FrontSection.Paragraphs);
                    int cTarget = _CountMatchingParagraphTypes(iTarget, TargetSection.Paragraphs);

                    // Start a new group and add the paragraphs to the group
                    var group = new ParagraphGroup();
                    vMajorParts.Add(group);
                    for (int i = 0; i < cFront; i++, iFront++)
                        group.FrontParagraphs.Add(FrontSection.Paragraphs[iFront]);
                    for (int i = 0; i < cTarget; i++, iTarget++)
                        group.TargetParagraphs.Add(TargetSection.Paragraphs[iTarget]);
                }

                // 2. Split out those which are side-by-side capable
                m_vParagraphGroups = new List<ParagraphGroup>();
                foreach (ParagraphGroup group in vMajorParts)
                    m_vParagraphGroups.AddRange(group.ConvertToSideBySide());

                // 3. Create a group with all of the footnotes
                var FrontFootnotes = DB.FrontSection.AllFootnotes;
                var TargetFootnotes = DB.TargetSection.AllFootnotes;
                var gFootnotes = new ParagraphGroup();
                foreach (DFootnote f in FrontFootnotes)
                    gFootnotes.FrontParagraphs.Add(f);
                foreach (DFootnote f in TargetFootnotes)
                    gFootnotes.TargetParagraphs.Add(f);

                // 4. Split them out if side-by-side capable
                m_vFootnoteGroups = gFootnotes.ConvertToSideBySide();
            }
            #endregion
        }

        // Registered Layouts ----------------------------------------------------------------
        /* DOCUMENTATION: Adding a new Window/Job
         * 1. Create the subclass of WLayout
         * 2. Create and Register the subclass in OW's constructor
         * 3. Create the Window menu item, make sure to set the Tag to the subclass's LayoutName
         * 4. Create the command handler for changing windows
         */
        const string c_sCurrentLayout = "CurrentJob";
        #region SAttr{g}: WLayout CurrentLayout
        static public WLayout CurrentLayout
        {
            get
            {
                return s_CurrentLayout;
            }
        }
        static WLayout s_CurrentLayout;
        #endregion
        #region List<WLayout> RegisteredLayouts
        static List<WLayout> RegisteredLayouts
        {
            get
            {
                if (null == s_vRegisteredLayouts)
                    s_vRegisteredLayouts = new List<WLayout>();
                Debug.Assert(null != s_vRegisteredLayouts);
                return s_vRegisteredLayouts;
            }
        }
        static List<WLayout> s_vRegisteredLayouts;
        #endregion
        #region SMethod: WLayout Find(sLayoutName)
        static public WLayout Find(string sLayoutName)
        {
            foreach (WLayout layout in RegisteredLayouts)
            {
                if (layout.LayoutName == sLayoutName)
                    return layout;
            }
            return null;
        }
        #endregion
        #region SMethod: void RegisterLayout(parent, WLayout layout)
        static public void RegisterLayout(Control parent, WLayout layout)
        {
            // Don't do this but once for a given layout
            if (null != Find(layout.LayoutName))
                return;

            // Add it to our list
            RegisteredLayouts.Add(layout);

            // Place it in the window correctly
            layout.Hide();
            layout.Dock = DockStyle.Fill;
            parent.Controls.Add(layout);
        }
        #endregion
        #region SMethod: bool SetCurrentLayout(string sLayoutName)
        static public bool SetCurrentLayout(string sLayoutName)
        {
            WLayout layout = Find(sLayoutName);
            Debug.Assert(null != layout, "Did you forget to register a window type?");

            // Don't do anything if we're already active
            if (CurrentLayout == layout)
                return false;

            // Hide the current one
            if (CurrentLayout != null)
                CurrentLayout.Hide();

            // Now set to the new one and show it
            s_CurrentLayout = layout;
            CurrentLayout.Show();

            // Save this to the Registry so we can restore it on OurWord startup
            JW_Registry.SetValue(c_sCurrentLayout, CurrentLayout.LayoutName);
            return true;
        }
        #endregion
        #region SMethod: string GetLayoutFromRegistry(sDefaultLayout)
        static public string GetLayoutFromRegistry(string sDefaultLayout)
        {
            return JW_Registry.GetValue(c_sCurrentLayout, sDefaultLayout);
        }
        #endregion
        #region SMethod: bool CurrentLayoutIs(string sLayoutName)
        static public bool CurrentLayoutIs(string sLayoutName)
        {
            if (CurrentLayout.LayoutName == sLayoutName)
                return true;
            return false;
        }
        #endregion
        #region SMethod: bool CurrentLayoutIs(vsLayoutName)
        static public bool CurrentLayoutIs(IEnumerable<string> vsLayoutName)
        {
            foreach (var s in vsLayoutName)
            {
                if (CurrentLayoutIs(s))
                    return true;
            }
            return false;
        }
        #endregion
        #region SMethod: void CheckWindowMenuItem(btnParent)
        static public void CheckWindowMenuItem(ToolStripDropDownButton btnParent)
        {
            if (null == CurrentLayout)
                return;

            foreach (ToolStripItem item in btnParent.DropDownItems)
            {
                // Only interested in menu items, not, e.g., separators
                var menuItem = item as ToolStripMenuItem;
                if (null == menuItem)
                    continue;

                // Retrieve the name of the layout for which this menu item corresponds
                string sMenuItemLayoutName = (string)menuItem.Tag;

                // Is thie item a Window name?
                if (null == Find(sMenuItemLayoutName))
                    continue;

                // Set the check depending on if it is current
                menuItem.Checked = (sMenuItemLayoutName == CurrentLayout.LayoutName);
            }
        }
        #endregion

    }
}
